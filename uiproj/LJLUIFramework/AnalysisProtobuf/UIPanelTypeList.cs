using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace UIFrameworkForProtobuf
{
    [ProtoContract]
    class UIPanelTypeList
    {
        [ProtoMember(1)]
        public List<UIPanelType> uiPanelTypes;
        
        public UIPanelTypeList()
        {
            uiPanelTypes = new List<UIPanelType>();
        }
    }
}
