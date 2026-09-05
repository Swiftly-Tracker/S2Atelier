namespace S2Atelier.Ida.Generated;

public static unsafe partial class IdaNative
{
    public static readonly IdaSdkVersion[] GeneratedVersions = [IdaSdkVersion.V92, IdaSdkVersion.V93];

    public static void BindAll(nint idaHandle, nint idalibHandle, IdaSdkVersion version)
    {
        switch (version)
        {
            case IdaSdkVersion.V92: IdaNativeBinder_V92.BindAll(idaHandle, idalibHandle); break;
            case IdaSdkVersion.V93: IdaNativeBinder_V93.BindAll(idaHandle, idalibHandle); break;
            default:
                throw new System.NotSupportedException($"No generated bindings for IDA SDK {version}.");
        }
    }
}
