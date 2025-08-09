// Source Executable: c:\windows\system32\bisrv.dll
// Interface ID: 8bfc3be1-6def-4e2d-af74-7c47cd0ade4a
// Interface Version: 1.0
// Client Generated: 8.08.2025 03:36:58
// NtCoreLib Version: 2.0.0+a18af727ad9dc5ac0ff0bbe4a9f03e744f69ae85

namespace rpc_8bfc3be1_6def_4e2d_af74_7c47cd0ade4a_1_0
{

    #region Marshal Helpers
    internal sealed class _Marshal_Helper : NtCoreLib.Ndr.Marshal.NdrMarshalBufferDelegator
    {
        public _Marshal_Helper() :
                this(new NtCoreLib.Ndr.Marshal.NdrMarshalBuffer())
        {
        }
        public _Marshal_Helper(NtCoreLib.Ndr.Marshal.INdrMarshalBuffer m) :
                base(m)
        {
        }
        public void Write_0(byte[] p0, long p1)
        {
            WriteConformantArray<byte>(p0, p1);
        }
        public void Write_1(char[] p0, long p1)
        {
            WriteConformantArray<char>(p0, p1);
        }
        public void Write_2(System.Guid[] p0, long p1)
        {
            WriteConformantArrayCallback<System.Guid>(p0, new System.Action<System.Guid>(this.WriteGuid), p1);
        }
        public void Write_3(NtCoreLib.NtEvent p0)
        {
            WriteSystemHandle<NtCoreLib.NtEvent>(p0);
        }
        public void Write_4(byte[] p0, long p1)
        {
            WriteConformantArray<byte>(p0, p1);
        }
        public void Write_5(byte[] p0, long p1)
        {
            WriteConformantArray<byte>(p0, p1);
        }
    }
    internal sealed class _Unmarshal_Helper : NtCoreLib.Ndr.Marshal.NdrUnmarshalBufferDelegator
    {
        public _Unmarshal_Helper(NtCoreLib.Ndr.Marshal.INdrUnmarshalBuffer u) :
                base(u)
        {
        }
        public byte[] Read_0()
        {
            return ReadConformantArray<byte>();
        }
        public char[] Read_1()
        {
            return ReadConformantArray<char>();
        }
        public System.Guid[] Read_2()
        {
            return ReadConformantArrayCallback<System.Guid>(new System.Func<System.Guid>(this.ReadGuid));
        }
        public NtCoreLib.NtEvent Read_3()
        {
            return ReadSystemHandle<NtCoreLib.NtEvent>();
        }
        public byte[] Read_4()
        {
            return ReadConformantArray<byte>();
        }
        public byte[] Read_5()
        {
            return ReadConformantArray<byte>();
        }
    }
    #endregion
    #region Client Implementation
    public sealed class Client : NtCoreLib.Win32.Rpc.Client.RpcClientBase
    {
        public Client() :
                base("8bfc3be1-6def-4e2d-af74-7c47cd0ade4a", 1, 0)
        {
        }
        private _Unmarshal_Helper SendReceive(int p, _Marshal_Helper @__m)
        {
            return new _Unmarshal_Helper(SendReceiveTransport(p, @__m));
        }
        public uint PsmBiExtServerCleanup(byte[] p0, int p1, System.Guid p2)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.WriteGuid(p2);
            _Unmarshal_Helper @__u = SendReceive(0, @__m);
            try
            {
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtNotifyAppState(byte[] p0, int p1, string p2, NtCoreLib.Ndr.Marshal.NdrEnum16 p3, out sbyte p4)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.WriteTerminatedString(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p2, "p2"));
            @__m.WriteEnum16(p3);
            _Unmarshal_Helper @__u = SendReceive(1, @__m);
            try
            {
                p4 = @__u.ReadSByte();
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_2(byte[] p0, int p1, NtCoreLib.Ndr.Marshal.NdrEnum16 p2)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.WriteEnum16(p2);
            _Unmarshal_Helper @__u = SendReceive(2, @__m);
            try
            {
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_3(byte[] p0, int p1, char[] p2, int p3, int p4, out int p5, out System.Guid[] p6)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.WriteReferent(p2, new System.Action<char[], long>(@__m.Write_1), p3);
            @__m.WriteInt32(p3);
            @__m.WriteInt32(p4);
            _Unmarshal_Helper @__u = SendReceive(3, @__m);
            try
            {
                p5 = @__u.ReadInt32();
                p6 = @__u.ReadReferent<System.Guid[]>(new System.Func<System.Guid[]>(@__u.Read_2), false);
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_4(byte[] p0, int p1, char[] p2, int p3, out NtCoreLib.NtEvent p4)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.Write_1(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p2, "p2"), p3);
            @__m.WriteInt32(p3);
            _Unmarshal_Helper @__u = SendReceive(4, @__m);
            try
            {
                p4 = @__u.Read_3();
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public int PsmBiExtServerCleanup_5()
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            _Unmarshal_Helper @__u = SendReceive(5, @__m);
            try
            {
                return @__u.ReadInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_6(byte[] p0, int p1, char[] p2, int p3, out sbyte p4)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.Write_1(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p2, "p2"), p3);
            @__m.WriteInt32(p3);
            _Unmarshal_Helper @__u = SendReceive(6, @__m);
            try
            {
                p4 = @__u.ReadSByte();
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_7(ref NtCoreLib.Ndr.Marshal.NdrContextHandle p0)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteContextHandle(p0);
            _Unmarshal_Helper @__u = SendReceive(7, @__m);
            try
            {
                p0 = @__u.ReadContextHandle();
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtNotifyAppState_8(int p0, byte[] p1, out NtCoreLib.Ndr.Marshal.NdrContextHandle p2)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteInt32(p0);
            @__m.Write_4(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p1, "p1"), p0);
            _Unmarshal_Helper @__u = SendReceive(8, @__m);
            try
            {
                p2 = @__u.ReadContextHandle();
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtNotifyAppState_9()
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            _Unmarshal_Helper @__u = SendReceive(9, @__m);
            try
            {
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtNotifyAppState_10(byte[] p0, int p1)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            _Unmarshal_Helper @__u = SendReceive(10, @__m);
            try
            {
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_11(byte[] p0, int p1, System.Guid p2, out int p3, out byte[] p4)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.WriteGuid(p2);
            _Unmarshal_Helper @__u = SendReceive(11, @__m);
            try
            {
                p3 = @__u.ReadInt32();
                p4 = @__u.ReadReferent<byte[]>(new System.Func<byte[]>(@__u.Read_5), false);
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_12(string p0)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteTerminatedString(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p0, "p0"));
            _Unmarshal_Helper @__u = SendReceive(12, @__m);
            try
            {
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_13(string p0, int p1)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteTerminatedString(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p0, "p0"));
            @__m.WriteInt32(p1);
            _Unmarshal_Helper @__u = SendReceive(13, @__m);
            try
            {
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
        public uint PsmBiExtServerCleanup_14(byte[] p0, int p1, char[] p2, int p3, int p4, long p5, NtCoreLib.Ndr.Marshal.NdrEnum16 p6, out sbyte p7)
        {
            _Marshal_Helper @__m = new _Marshal_Helper(CreateMarshalBuffer());
            @__m.WriteReferent(p0, new System.Action<byte[], long>(@__m.Write_0), p1);
            @__m.WriteInt32(p1);
            @__m.Write_1(NtCoreLib.Ndr.Marshal.NdrMarshalUtils.CheckNull(p2, "p2"), p3);
            @__m.WriteInt32(p3);
            @__m.WriteInt32(p4);
            @__m.WriteInt64(p5);
            @__m.WriteEnum16(p6);
            _Unmarshal_Helper @__u = SendReceive(14, @__m);
            try
            {
                p7 = @__u.ReadSByte();
                return @__u.ReadUInt32();
            }
            finally
            {
                @__u.Dispose();
            }
        }
    }
    #endregion
}
