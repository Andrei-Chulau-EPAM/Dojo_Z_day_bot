using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBattleNetLibrary;

namespace CodeBattleNet.Core
{
    class MapPoint : Point
    {
        public Elements ElementAtPoint { get; }

        public MapPoint(int x, int y, Elements elementAtPoint) : base(x, y)
        {
            ElementAtPoint = elementAtPoint;
        }
    }
}
