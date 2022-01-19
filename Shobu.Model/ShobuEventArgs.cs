using System;

namespace Shobu.Model
{
    public class ShobuEventArgs : EventArgs
    {
        public int Victor { get; }

        public ShobuEventArgs(int victor)
        {
            Victor = victor;
        }
    }
}
