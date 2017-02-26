using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;

namespace Labels
{
    class DesktopElementsManager
    {
        private IntPtr desktopHandle;
        List<string> currentElementNames;

        public DesktopElementsManager()
        {
            desktopHandle = Win32.GetDesktopWindow(Win32.DesktopWindow.SysListView32);
            currentElementNames = GetElementNames();
        }

        public int GetElementsCount()
        {
            var _desktopHandle = Win32.GetDesktopWindow(Win32.DesktopWindow.SysListView32);
           return (int)Win32.SendMessage(_desktopHandle, Win32.LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
        }

        public List<string> GetElementNames()
        {
            List<string> elementNames = new List<string>();
            AutomationElement el = AutomationElement.FromHandle(desktopHandle);

            TreeWalker walker = TreeWalker.ContentViewWalker;
            elementNames = new List<string>();
            for (AutomationElement child = walker.GetFirstChild(el); child != null;
                child = walker.GetNextSibling(child))
            {
                elementNames.Add(child.Current.Name);
            }
            return elementNames;
        }

        public NamedDesktopPoint[] GetElementLocations()
        {
            List<NamedDesktopPoint> deskPoints = new List<NamedDesktopPoint>();

            uint desktopProcessId;
            Win32.GetWindowThreadProcessId(desktopHandle, out desktopProcessId);

            IntPtr desktopProcessHandle = IntPtr.Zero;

            desktopProcessHandle = Win32.OpenProcess(Win32.ProcessAccess.VmOperation | Win32.ProcessAccess.VmRead |
                Win32.ProcessAccess.VmWrite, false, desktopProcessId);

            IntPtr sharedMemoryPointer = Win32.VirtualAllocEx(desktopProcessHandle, 
                IntPtr.Zero, 4096, Win32.AllocationType.Reserve | Win32.AllocationType.Commit, Win32.MemoryProtection.ReadWrite);

            var elementsCount = GetElementsCount();
            for (int i = 0; i < elementsCount; i++)
            {
                uint numberOfBytes = 0;
                DesktopPoint[] point = new DesktopPoint[1];

                Win32.WriteProcessMemory(desktopProcessHandle, sharedMemoryPointer,
                    Marshal.UnsafeAddrOfPinnedArrayElement(point, 0),
                    Marshal.SizeOf(typeof(DesktopPoint)),
                    ref numberOfBytes);

                Win32.SendMessage(desktopHandle, Win32.LVM_GETITEMPOSITION, i, sharedMemoryPointer);

                Win32.ReadProcessMemory(desktopProcessHandle, sharedMemoryPointer,
                    Marshal.UnsafeAddrOfPinnedArrayElement(point, 0),
                    Marshal.SizeOf(typeof(DesktopPoint)),
                    ref numberOfBytes);
                deskPoints.Add(new NamedDesktopPoint(currentElementNames[i], point[0].X, point[0].Y));
            }

            return deskPoints.ToArray();
        }

        public void SetElementLocations(IEnumerable<NamedDesktopPoint> elementLocations)
        {
            foreach (var position in elementLocations)
            {
                var elementIndex = currentElementNames.IndexOf(position.Name);
                if (elementIndex == -1)
                { continue; }

                Win32.SendMessage(desktopHandle, Win32.LVM_SETITEMPOSITION, elementIndex, Win32.MakeLParam(position.X, position.Y));
            }
        }

        public void ShufleElementLocations(List<NamedDesktopPoint> elementLocations)
        {
            var rand = new Random();
            int width = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenWidth);
            int height = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenHeight);
            for (int i = 0; i < elementLocations.Count; i++)
            {
                Win32.SendMessage(desktopHandle, Win32.LVM_SETITEMPOSITION, i, Win32.MakeLParam(rand.Next(width - 200), rand.Next(height - 200)));
            }
        }
    }
}
