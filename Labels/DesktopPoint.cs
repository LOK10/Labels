using System.Runtime.InteropServices;

namespace Labels
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DesktopPoint
    {
        public int X;
        public int Y;

        public DesktopPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }

    public struct NamedDesktopPoint
    {
        public string Name;
        public int X;
        public int Y;

        public NamedDesktopPoint(string name, int x, int y)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return Name + ", " + X + ", " + Y;
        }
    }
}
