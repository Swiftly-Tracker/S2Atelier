# S2Atelier.Automation

.NET 10 HTTP 服务，在 Linux VPS 上下载 Steam depot 的指定 manifest，生成带类型信息的 `.i64`。Linux 使用原生 IDA Docker 容器，Windows 使用 Docker + Wine 执行 Windows 版 S2Atelier/IDA。Docker 本身不是 Windows 内核虚拟机。

## HTTP API

除 `/health` 外全部接口要求 `Authorization: Bearer <S2A_API_KEY>`。服务默认监听 `127.0.0.1:5080`，可通过 SSH 隧道使用：

```sh
ssh -L 5080:127.0.0.1:5080 root@YOUR_VPS
```

密钥保存在 VPS `/opt/s2atelier/service.env`（仅 root 可读），不要放入 Git。

```sh
curl -X POST http://127.0.0.1:5080/jobs \
  -H "Authorization: Bearer $S2A_API_KEY" -H 'Content-Type: application/json' \
  -d '{"dumpsCommit":"019567718b116e69277f7d05275a082431f465d6","binaryRegex":"(^|/)libschemasystem\\.so$","platform":"linux"}'
```

Windows 任务使用相同 `dumpsCommit`，设置 `platform: "windows"` 和相应 regex，例如 `(^|/)schemasystem\.dll$`。双平台任务分别提交一次，共享持久化来源仓库。

| 接口 | 用途 |
|---|---|
| `POST /jobs` | 返回 202、任务和 Location；队列满返回 429 |
| `GET /jobs` | 最近 100 个任务 |
| `GET /jobs/{id}` | queued / running / succeeded / failed，错误和产物相对路径 |
| `GET /jobs/{id}/log` | 下载完整执行日志 |
| `GET /jobs/{id}/provenance` | SDK/Dumps 提交、manifest、二进制和 i64 SHA-256 |
| `GET /jobs/{id}/artifacts/{index}` | 下载成功任务的第 index 个 i64，支持 Range |
| `GET /health` | HTTP 进程存活检查（不表示 Steam/IDA 可用） |

必填 `dumpsCommit`（40 位 Git commit SHA）、`platform`（`windows` / `linux`）、`binaryRegex`。可选 `importSchema`（默认 true）。`appId`、`depotId`、`manifestId` 由该提交的 manifest 内容读取，不再接受手工传入（返回 400）。解析后的 ID 和 manifest 文件路径可在 `/jobs/{id}/provenance` 查看；manifest ID 保留为字符串以避免 64 位整数精度损失。regex 与 downloader 一致，匹配完整 manifest 相对路径；服务只接受 .NET NonBacktracking 支持的语法，限制长度 512。下载后检查 PE/ELF x64 文件头，非目标平台文件不会送入 IDA。

## 流程与持久化

1. `sources/hl2sdk` 固定 cs2 分支；`sources/CS2-Dumps` 固定 main 分支。首次 clone，之后每任务 `git pull --ff-only` 和递归 submodule 更新。保持 `.git` 和工作树，不重新下载整个仓库。
2. 归档指定 `dumpsCommit` 的 protobufs/dump/manifests。在该快照内选择 `SourceRevision` 最大的 manifest 文件，再按 Windows/Linux 分别选取 2347771/2347773 depot 段，读取其中的 depot ID、manifest ID 和 app ID。旧版本提交不会使用当前工作树的 manifest。缺失、格式错误或同一 SourceRevision 存在冲突时任务失败，不回退到其他版本。
3. SteamDepotDownload 根据 app/depot/manifest 和 JSON regex 文件列表下载。保留下载记录和原始 binaries 供复查。
4. 使用 HL2SDK protobuf **3.21.8** 构建的 protoc，按 binary 对应模块编译 `.proto` → `.pb.h`。不把不同模块的同名 proto 混合编译。
5. 每个二进制单独运行容器，导入 protobuf/schema/接口并运行 ConVar、函数指针命名和 PLT 修复。没有对应 protobuf/schema 项目的模块会跳过相应导入。容器无网络，最多 2 CPU、4 GiB 内存；一次运行一个二进制。
6. 验证实际存在非空 i64，记录哈希并开放下载。任何下载/编译/分析错误均使任务失败，日志保留。

队列最多 32 个未完成任务，串行执行以避免来源更新与 IDA 争用。状态写入临时 JSON 后原子替换；服务升级后，旧格式 queued 任务标记失败并提示用 commit 重新提交，历史任务仍可查看和下载。服务重启后恢复新格式 queued 任务，running 任务标记失败，可重新提交。每任务 6 小时超时，服务停止/超时会清理对应容器。失败时生成的诊断头文件保存在任务的 `diagnostics/` 目录。数据不自动删除，管理员可在任务终止后归档或清理 jobs 目录。不要在服务运行时删除该目录。

Dumps 的 sdk.json 不附带可验证的跨平台布局保证；导入器按目标 PE/ELF 选择 ABI，但仍应检查日志中的 schema/clang 诊断，不能把“成功生成 i64”等同于全部类型完全准确。

## 部署

要求 Debian 13 x64、Docker、至少 8 GiB 内存以及足够的数据库存储。项目内已加入官方 IDA SDK 9.4 绑定，仍保留 9.2/9.3。

```sh
# 在 VPS 的项目根目录，root 执行
bash S2Atelier.Automation/deploy/provision.sh
```

`provision.sh` 安装构建工具，持久化 clone 来源、编译 protoc、下载 SteamDepotDownload，使用 .NET SDK Docker 镜像测试并发布程序，通过 xwin 持久化下载 Microsoft CRT/Windows SDK，构建两种执行镜像。容器显式配置 与 IDAClang 21 匹配的 LLVM 21.1.8 内置头文件路径；Linux 使用 GCC 系统头文件，Windows 使用 xwin 的 CRT/UCRT/UM/shared 头文件。启用 systemd 前检查以下 IDA 安装路径，缺失则停止：

- `/opt/s2atelier/ida/linux`：包含 libida.so、libidalib.so、IDAClang、cfg/til/procs/loaders 等完整安装内容及有效许可证。
- `/opt/s2atelier/ida/windows`：包含 ida.dll、idalib.dll、IDAClang 等完整 Windows 安装内容及有效许可证。

如使用已授权的指定分发来源，可先运行 `bash S2Atelier.Automation/deploy/install_ida.sh --apply-source-patches`；该步骤下载并校验 IDA 安装包、安装 Windows/Linux 版本，显式启用参数时应用所选来源的补丁/许可证。安装脚本固定该来源提交，IDA 运行文件不进入 Git。首次运行的 EULA 状态按官方 HCLI 方式初始化并持久化。

安装包来源可使用用户指定的 [IDA 9.4 分发仓库](https://github.com/Yigods/ida94_b1)，或自行提供 IDA 安装。程序包和许可证不包含在本仓库，也不会由每个任务重复下载。`deploy/build.sh` 可独立用于更新应用；更新前停止服务，更新后启动。

```sh
systemctl stop s2atelier
bash S2Atelier.Automation/deploy/build.sh
systemctl start s2atelier
journalctl -u s2atelier -n 100 --no-pager
```

默认部署根路径 `/opt/s2atelier`。如修改 `S2A_ROOT`，需同步修改 systemd 的 WorkingDirectory/EnvironmentFile/ExecStart。直接暴露 HTTP 时自行配置 `ASPNETCORE_URLS` 和 HTTPS 反向代理；默认 SSH 隧道不需要公网监听端口。

## 验证

```sh
dotnet build S2Atelier.slnx -c Release
# 在 x64 环境运行现有测试（S2Atelier.Ida 限定 x64）
dotnet run --project S2Atelier.Tests -c Release
# 构建框架依赖版 Automation 后，运行真实 HTTP + 持久化队列测试
python3 S2Atelier.Automation/tests/test_manifest.py
python3 S2Atelier.Automation/tests/test_service.py S2Atelier.Automation/bin/Release/net10.0
```

构建验证不能代替实际 IDA/Steam 测试。真实双平台结果与部署版本记录见 `DEPLOYMENT.md`。
