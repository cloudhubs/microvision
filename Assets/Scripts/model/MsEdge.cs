using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.model
{
    [Serializable]
    class MsEdge
    {
        public MsNode from;
        public MsNode to;
        public MsLabel label;
        public int width;
        public int length;
    }
}
