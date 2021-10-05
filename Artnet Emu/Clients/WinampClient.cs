using ArtnetEmu.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Clients
{
    public enum WinampPlayState
    {
        Stopped = 0,
        Playing = 1,
        Paused = 3
    }
    public class WinampClient
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct CopyDataStruct
        {
            public uint dwData;
            public uint cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        private IntPtr _winampHwnd;
        public WinampClient()
        {
            _winampHwnd = FindWindow("Winamp v1.x", null);
        }

        private IntPtr WinampHwnd
        {
            get
            {
                if (_winampHwnd == IntPtr.Zero)
                {
                    _winampHwnd = FindWindow("Winamp v1.x", null);
                }
                return _winampHwnd;
            }
        }

        protected int SendInternalCommand(int command, int param = 0)
        {
            return SendMessage(WinampHwnd, Constants.Windows.WM_USER, new IntPtr(param), new IntPtr(command));
        }

        protected int SendExternalCommand(int command, int param = 0)
        {
            return SendMessage(WinampHwnd, Constants.Windows.WM_COMMAND, new IntPtr(command), new IntPtr(param));
        }

        public int Version
        {
            get
            {
                return SendInternalCommand(Constants.Winamp.IPC_GETVERSION);
            }
        }

        public byte Volume
        {
            get
            {
                return Convert.ToByte(SendInternalCommand(Constants.Winamp.IPC_SETVOLUME, -666));
            }
            set
            {
                SendInternalCommand(Constants.Winamp.IPC_SETVOLUME, value);
            }
        }

        public void EnqueueFile(string filename)
        {
            CopyDataStruct cds = new CopyDataStruct();
            cds.dwData = Constants.Winamp.IPC_ENQUEUEFILE;
            cds.lpData = filename;
            cds.cbData = Convert.ToUInt32(filename.Length + 1);
            IntPtr pointer = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
            Marshal.StructureToPtr(cds, pointer, false);
            int result = SendMessage(WinampHwnd, Constants.Windows.WM_COPYDATA, IntPtr.Zero, pointer);
            Marshal.FreeCoTaskMem(pointer);
        }

        public void Previous()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON1);
        }

        public void Rewind5Seconds()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON1_SHIFT);
        }

        public void StartOfList()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON1_CTRL);
        }

        public void Play()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON2);
        }

        public void OpenFileDialog()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON2_SHIFT);
        }

        public void OpenUrlDialog()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON2_CTRL);
        }

        public void TogglePause()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON3);
        }

        public void Stop()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON4);
        }

        public void StopWithFadeout()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON4_SHIFT);
        }

        public void StopAfterCurrent()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON4_CTRL);
        }

        public void Next()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON5);
        }

        public void FastForward5Seconds()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON5_SHIFT);
        }

        public void EndOfList()
        {
            SendExternalCommand(Constants.Winamp.WINAMP_BUTTON5_CTRL);
        }

        public bool ManualPlaylistAdvance
        {
            get
            {
                return SendInternalCommand(Constants.Winamp.IPC_GET_MANUALPLADVANCE) == 1;
            }
            set
            {
                SendInternalCommand(Constants.Winamp.IPC_SET_MANUALPLADVANCE, value ? 1 : 0);
            }
        }

        // Thanks to misterhenson for the inspiration
        // https://misterhenson.wordpress.com/2015/02/09/delphi-xe-6-get-currently-playing-title-from-winamp/
        private string ReadInternalMemory(IntPtr pointer, int maxSize = 500)
        {
            var rawString = new byte[maxSize];
            IntPtr processId = IntPtr.Zero;
            IntPtr threadId = GetWindowThreadProcessId(WinampHwnd, out processId);
            IntPtr insideHwnd = OpenProcess(Constants.Windows.PROCESS_VM_READ, false, processId);
            IntPtr counter = IntPtr.Zero;
            bool didRead = ReadProcessMemory(insideHwnd, pointer, rawString, maxSize, out counter);
            CloseHandle(insideHwnd);
            int index = Array.FindIndex(rawString, 0, (x) => x == 0);
            return Encoding.Default.GetString(rawString, 0, index);
        }

        public string PlaylistFilename(int index)
        {
            var pointer = new IntPtr(SendInternalCommand(Constants.Winamp.IPC_GETPLAYLISTFILE, index));
            return ReadInternalMemory(pointer);
        }

        public string PlaylistTitle(int index)
        {
            var pointer = new IntPtr(SendInternalCommand(Constants.Winamp.IPC_GETPLAYLISTTITLE, index));
            return ReadInternalMemory(pointer);
        }

        public int PlaylistPosition
        {
            get
            {
                return SendInternalCommand(Constants.Winamp.IPC_GETLISTPOS);
            }
            set
            {
                SendInternalCommand(Constants.Winamp.IPC_SETPLAYLISTPOS, value);
            }
        }

        public int PlaylistLength
        {
            get
            {
                return SendInternalCommand(Constants.Winamp.IPC_GETLISTLENGTH);
            }
        }

        public WinampPlayState State
        {
            get
            {
                return (WinampPlayState)SendInternalCommand(Constants.Winamp.IPC_ISPLAYING);
            }
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = false)]
        private static extern int SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr ProcessId);
        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, IntPtr dwProcessId);
        [DllImport("kernel32.dll", SetLastError = false)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}
