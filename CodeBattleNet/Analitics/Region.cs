using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Analitics
{
    struct Region
    {
        public int Top { get; }
        public int Bottom { get; }
        public int Left { get; }
        public int Right { get; }

        public Region(int top, int bottom, int left, int right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Region(Region region, int expandTo)
        {
            Top = region.Top - expandTo;
            Bottom = region.Bottom + expandTo;
            Left = region.Left - expandTo;
            Right = region.Right + expandTo;
        }

        public override string ToString()
        {
            return $"[T:{Top},B:{Bottom},L:{Left},R:{Right}]";
        }
    };
}
