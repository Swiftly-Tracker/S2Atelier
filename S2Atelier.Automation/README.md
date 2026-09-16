# S2Atelier.Automation

.NET 10 持久化 HTTP 队列，在 Linux VM 上运行原生 IDA 和 Wine/Windows IDA，生成并发布 CS2 数据库。

## 自动发布

`CS2-Dumps/.github/workflows/update.yml` 调用 `Dumps/.github/workflows/dumper.yml`，取得 **实际推送的提交 SHA**，再调用 `Dumps/.github/workflows/trigger_update.yml`。后者通过 HTTP 提交 `{ "dumpsCommit": "<40-character SHA>", "platform": "all" }`。Actions 提交后即结束，VM 依次完成 Windows/Linux 两个平台，每个平台内并行解析模块，最长 72 小时。

在 **CS2-Dumps** 的 Actions secrets 配置 `S2A_API_URL` 和 `S2A_API_KEY`，由调用方传入 reusable workflow。当前部署地址为 `http://31.56.39.201/s2atelier`。API key 保存在 VM 的 `/opt/s2atelier/service.env`。发布所用 `S2A_GITHUB_TOKEN` 只保存在 VM 的该文件中，需对 `Swiftly-Tracker/CS2-IDA-Dumps` 有 contents 写权限。不要将密钥放进 Git。

手动运行 CS2-Dumps 的 Dumping workflow 时，可填 `dumps_commit` 直接解析已有提交；留空则执行原来的 Steam dump 流程。

1. 持久化更新 HL2SDK cs2 分支和 CS2-Dumps main 分支；归档指定提交的 protobufs/dump/manifests，并从同一提交读取 `tracked_files.json`。
2. 根据快照中最大的 SourceRevision 选择 manifest。Windows 使用 2347771 和共享 depot 2347770/2347779；Linux 使用 2347773 和共享 depot。保留每个 depot 的上游 regex，并与 `.dll/.exe` 或 `.so` 扩展名求交集；不会下载 VPK、图片或脚本作为解析输入。下载后检查 PE/ELF x64 文件头。
3. 按模块编译 protobuf，导入 protobuf/schema/HL2SDK 接口，运行 ConVar、函数指针、PLT 等分析。容器无网络，每路限制为 2 CPU、3 GiB 内存，默认两路并行。通过 service.env 的 `S2A_ANALYSIS_WORKERS`（1–16）和 `S2A_ANALYSIS_MEMORY` 调整；应按 VM 的可用 CPU/内存设置。每路使用独立容器名、protobuf/诊断目录及 IDA 状态目录。
4. 每个数据库用 7-Zip LZMA2 `-mx=9 -md=64m -mfb=273 -ms=on -mmt=2` 压缩并执行 `7z t`。资产命名为 `server.dll.i64.7z`、`libserver.so.i64.7z` 等（exe 同样保留扩展名）。不同目录出现同名模块时合并进同名压缩包，包内保留相对路径，避免覆盖。
5. 同时生成 `common-windows.7z` 和 `common-linux.7z`，各包含 server、engine2、tier0 三个 **i64 数据库**，不嵌套单文件压缩包。优先选择 game/csgo/bin 下的 server 和 game/bin 下的 engine2/tier0。
6. 在 `Swiftly-Tracker/CS2-IDA-Dumps` 创建草稿 Release，tag 为 `cs2-<完整 dumpsCommit>`，标题取提交首行并去掉 `| X modified`。所有单文件包、两个 common 包和 provenance.json 上传后，检查完整附件集合、大小、上传状态和 GitHub SHA-256，再公开 Release。
7. 确认 Release 已公开并保存 release.json 回执后，删除该任务的 Windows/Linux 工作目录（下载文件、生成头文件、i64、7z、快照与诊断临时文件）。保留 job.json、pipeline.log、provenance.json 和 release.json。失败时保留工作目录；不会删除 IDA、许可证、SDK、运行工具或其他任务。

同一请求在 queued/running/succeeded 时重复提交会返回已有任务。失败后提交相同请求会续跑原任务，校验 binary/7z 哈希后复用已完成模块；输入 SDK、分析器或镜像变化时拒绝混用。重试可以复用同一 tag 的草稿附件，公开的 Release 内容不自动覆盖。哈希校验或上传失败不会公开草稿，也不会清理本地工作目录。

## Windows 两阶段分析

Windows 模块先用 Linux 原生 IDA 完成基础分析并保存 `.i64`，再用 Wine/Windows IDA 显式打开该数据库执行现有类型导入与命名。第二阶段显式指定 Schema 项目，避免 `.dll.i64` 导致项目名识别错误。两阶段顺序执行，使用独立的 IDA 状态目录；模块间仍按配置并行。Linux 模块保持原生完整流程。

黑名单排除 `assetrename.dll`、`assetrename.exe`、`libassetrename.so` 和 `assetrename.so`，大小写不敏感。下载 regex、缓存输入和旧附件均执行过滤。升级前已完成并通过哈希校验的其他模块可以复用。

Windows provenance 额外记录 Linux 分析器及镜像身份。新结果记录 `analysisMode=linux-base-wine-import`、`baseAnalysisSeconds` 和 `importSeconds`；耗时包含容器启动和数据库保存，不包含压缩。两边应部署相同版本的 IDA。

运行 `python3 S2Atelier.Automation/tests/test_hybrid.py` 验证两阶段顺序、显式数据库交接、Schema 项目和黑名单过滤。

## Steam 授权 depot

Workshop Tools depot 2347779 需要拥有权限的 Steam 账号。完整跟踪模式不会忽略该 depot 的下载失败。首次由 root 在 VM 交互登录并完成 Steam Guard：

```sh
/opt/s2atelier/tools/downloader/SteamDepotDownload.App -app 730 -depot 2347779 -manifest-only -username YOUR_STEAM_ACCOUNT -remember-password -dir /opt/s2atelier/state/steam-login
```

然后在 service.env 设置 `S2A_STEAM_USERNAME=YOUR_STEAM_ACCOUNT`，任务空闲时重启服务。下载器使用同一 root 用户保存的登录令牌；无需将 Steam 密码上传到 GitHub 或放进任务请求。登录令牌过期时需重新交互登录。

## HTTP API

除 `/health` 外均需 `Authorization: Bearer <S2A_API_KEY>`。服务本身只监听 `127.0.0.1:5080`，Caddy 提供上述外部路径。

| 接口 | 用途 |
|---|---|
| POST /jobs | 提交，返回 202；队列满返回 429 |
| GET /jobs | 最近 100 个任务 |
| GET /jobs/{id} | queued/running/succeeded/failed、错误、产物地址 |
| GET /jobs/{id}/log | 执行日志 |
| GET /jobs/{id}/provenance | 提交、manifest、工具和原始 i64 哈希 |
| GET /jobs/{id}/artifacts/{index} | 正式发布任务跳转 GitHub；诊断任务下载本地 7z |

诊断任务可以设置 `publishRelease: false`，使用 `platform: windows/linux/all` 和可选 `binaryRegex`。正式发布必须使用 `all` 且不能指定覆盖 regex，避免发布不完整的 common 包。`appId/depotId/manifestId` 一律从快照读取，不接受请求覆盖。

服务重启后 queued 任务恢复；running 任务标记失败，可重新提交。旧格式不兼容的 queued 任务标记失败，历史记录保留。SDK 的跨平台类型布局仍应结合 schema/clang 诊断检查。

## 部署与验证

需要 Debian 13 x64、Docker、Python 3.12+、7-Zip（Debian `7zip` 包）、至少 8 GiB 内存，以及完整的 Windows/Linux IDA 9.4 安装和许可证。`deploy/provision.sh` 配置依赖；`deploy/build.sh` 构建分析器和服务。升级前停止服务，升级后启动；已有运行任务应先完成。

```sh
python3 S2Atelier.Automation/tests/test_manifest.py
python3 S2Atelier.Automation/tests/test_release.py
python3 S2Atelier.Automation/tests/test_parallel.py
dotnet publish S2Atelier.Automation/S2Atelier.Automation.csproj -c Release -r linux-x64 --self-contained true -o /tmp/s2a-app
python3 S2Atelier.Automation/tests/test_service.py /tmp/s2a-app
```

测试覆盖快照隔离、manifest 选择、上游 regex、真实 7z 解压内容、重复文件名、Release 验证失败保护、HTTP 鉴权、持久化队列、发布任务去重与重启恢复。真实 Steam/IDA 执行和 GitHub 发布仍需检查任务日志与最终 Release。
