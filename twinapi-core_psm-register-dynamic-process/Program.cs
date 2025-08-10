using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NtCoreLib.Ndr.Marshal;
using NtCoreLib.Ndr.Rpc;
using NtCoreLib.Win32.Rpc.EndpointMapper;
using NtCoreLib.Win32.Rpc.Transport;
using rpc_bdaa0970_413b_4a3e_9e5d_f6dc9d7e0760_1_0;

class Program
{
    [DllImport("advapi32.dll", SetLastError = true)]
    static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    static extern bool GetTokenInformation(IntPtr TokenHandle, int TokenInformationClass,
        IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool DuplicateHandle(
        IntPtr hSourceProcessHandle,
        IntPtr hSourceHandle,
        IntPtr hTargetProcessHandle,
        out IntPtr lpTargetHandle,
        uint dwDesiredAccess,
        bool bInheritHandle,
        uint dwOptions);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool QueryFullProcessImageName(
        IntPtr hProcess,
        int dwFlags,
        StringBuilder lpExeName,
        ref int lpdwSize);

    const uint TOKEN_QUERY = 0x0008;
    const uint DUPLICATE_SAME_ACCESS = 0x00000002;
    const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
    const int TokenSecurityAttributes = 46;
    const int TokenCapabilities = 47;

    static readonly short[] StateCandidates =
    {
        0, 1, 2, unchecked((short)-1), 0x7FFF, unchecked((short)0x8000), unchecked((short)0xFFFF)
    };

    static void Main()
    {
        var ifUuid = new Guid("bdaa0970-413b-4a3e-9e5d-f6dc9d7e0760");
        var syntax = new RpcSyntaxIdentifier(ifUuid, 1, 0);
        var endpoints = RpcEndpointMapper.QueryEndpointsForInterface(null, syntax);

        foreach (var endpoint in endpoints)
        {
            Console.WriteLine($"\n[*] Endpoint: {endpoint.Binding}");

            using var client = new Client();
            var security = new RpcTransportSecurity
            {
                AuthenticationLevel = RpcAuthenticationLevel.PacketIntegrity
            };
            client.Connect(endpoint, security, null);
            NdrContextHandle ctx;
            client.Proc0(out ctx);

            foreach (var proc in Process.GetProcesses())
            {
                if (!IsAppContainerProcess(proc))
                    continue;

                Console.WriteLine($"\n=== {proc.ProcessName} (PID {proc.Id}) ===");

                var before = DumpTokenInfo(proc.Handle);

                foreach (var state in StateCandidates)
                {
                    try
                    {
                        IntPtr currentProcess = Process.GetCurrentProcess().Handle;
                        if (!DuplicateHandle(currentProcess, proc.Handle, currentProcess, out IntPtr dupHandle,
                            PROCESS_QUERY_LIMITED_INFORMATION, false, DUPLICATE_SAME_ACCESS))
                        {
                            Console.WriteLine($"[!] DuplicateHandle failed: {Marshal.GetLastWin32Error()}");
                            continue;
                        }

                        var ndrHandle = new NdrUInt3264(dupHandle);
                        int result = client.Ordinal3(ctx, ndrHandle, state);
                        Console.WriteLine($"[State={state}] NTSTATUS=0x{result:X}");
                        CloseHandle(dupHandle);

                        var after = DumpTokenInfo(proc.Handle);
                        PrintDiff($"SecurityAttributes (State={state})", before.SecurityAttributes, after.SecurityAttributes);
                        PrintDiff($"Capabilities (State={state})", before.Capabilities, after.Capabilities);

                        before = after;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[!] RPC call failed (State={state}): {ex.Message}");
                    }
                }
            }
        }
    }

    static bool IsAppContainerProcess(Process proc)
    {
        try
        {
            StringBuilder sb = new StringBuilder(1024);
            int size = sb.Capacity;
            if (!QueryFullProcessImageName(proc.Handle, 0, sb, ref size))
                return false;
            string path = sb.ToString();
            return path.IndexOf("WindowsApps", StringComparison.OrdinalIgnoreCase) >= 0;
        }
        catch
        {
            return false;
        }
    }

    static (List<string> SecurityAttributes, List<string> Capabilities) DumpTokenInfo(IntPtr processHandle)
    {
        var result = (SecurityAttributes: new List<string>(), Capabilities: new List<string>());

        if (!OpenProcessToken(processHandle, TOKEN_QUERY, out IntPtr token))
            return result;

        try
        {
            result.SecurityAttributes = GetStringListFromToken(token, TokenSecurityAttributes);
            result.Capabilities = GetStringListFromToken(token, TokenCapabilities);
        }
        finally
        {
            CloseHandle(token);
        }
        return result;
    }

    static List<string> GetStringListFromToken(IntPtr token, int infoClass)
    {
        var list = new List<string>();
        GetTokenInformation(token, infoClass, IntPtr.Zero, 0, out int len);
        if (len == 0) return list;

        IntPtr buffer = Marshal.AllocHGlobal(len);
        try
        {
            if (GetTokenInformation(token, infoClass, buffer, len, out _))
            {
                byte[] data = new byte[len];
                Marshal.Copy(buffer, data, 0, len);
                string asString = Encoding.Unicode.GetString(data).TrimEnd('\0');
                if (!string.IsNullOrWhiteSpace(asString))
                    list.Add(asString);
                else
                    list.Add(BitConverter.ToString(data));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
        return list;
    }

    static void PrintDiff(string title, List<string> before, List<string> after)
    {
        Console.WriteLine($"\n[{title} DIFF]");
        foreach (var item in after)
            if (!before.Contains(item))
                Console.WriteLine($"+ {item}");
        foreach (var item in before)
            if (!after.Contains(item))
                Console.WriteLine($"- {item}");
    }
}
