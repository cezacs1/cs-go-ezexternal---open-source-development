using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ezexternal
{
    public static class memory
    {
        public static class imports
        {
            [DllImport("kernel32.dll")]
            public static extern bool ReadProcessMemory(int Process, int Address, byte[] Buffer, int BufferSize, ref int ByteCount);
            [DllImport("kernel32.dll")]
            public static extern bool WriteProcessMemory(int Process, int Address, byte[] Buffer, int BufferSize, out int ByteCount);
            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(int DesiredAccess, bool InheritHandle, int ProcessID);
        }

        public static Process handlecsgo()
        {
            Process CSGO;
            if (Process.GetProcessesByName("csgo").Length > 0) CSGO = Process.GetProcessesByName("csgo")[0]; else return null;
            foreach (ProcessModule module in CSGO.Modules)
            {
                string modulename = module.ModuleName;
                switch (modulename)
                {
                    case "client.dll": client = (int)module.BaseAddress; break;
                    case "engine.dll": engine = (int)module.BaseAddress; break;
                    default:
                        break;
                }
            }
            processHandle = (int)imports.OpenProcess(0x8 | 0x10 | 0x20, false, CSGO.Id);
            if (processHandle > 0 && client > 0 && engine > 0)
                return CSGO;
            else return null;
        }

        public static int processHandle = 0;
        public static int client = 0;
        public static int engine = 0;
        private static int byteswritten = 0;
        private static int bytesread = 0;

        public static T Read<T>(int Address) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));
            byte[] Buffer = new byte[ByteSize];
            imports.ReadProcessMemory(processHandle, Address, Buffer, Buffer.Length, ref bytesread);
            return ByteArrayToStructure<T>(Buffer);
        }

        public static void Write<T>(int Address, object Value) where T : struct
        {
            byte[] Buffer = StructureToByteArray(Value);
            imports.WriteProcessMemory(processHandle, Address, Buffer, Buffer.Length, out byteswritten);
        }

        private static T ByteArrayToStructure<T>(byte[] Bytes) where T : struct
        {
            GCHandle var_Handle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            try { return (T)Marshal.PtrToStructure(var_Handle.AddrOfPinnedObject(), typeof(T)); }
            finally { var_Handle.Free(); }
        }

        private static byte[] StructureToByteArray(object Object)
        {
            int Length = Marshal.SizeOf(Object);
            byte[] Array = new byte[Length];
            IntPtr Pointer =
                Marshal.AllocHGlobal(Length);
            Marshal.StructureToPtr(Object, Pointer, true);
            Marshal.Copy(Pointer, Array, 0, Length);
            Marshal.FreeHGlobal(Pointer);
            return Array;
        }
    }
}
