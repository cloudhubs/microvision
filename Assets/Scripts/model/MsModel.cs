using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.model
{
    [Serializable]
    class MsModel
    {
        public List<MsNode> nodes;
        public List<MsEdge> edges;
    }
}
