using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace UIFrameworkForProtobuf
{
    [ProtoContract]
    class UIPanelType
    {
        [ProtoMember(1)]
        public string panelName
        {
            get;
            set;
        }
        [ProtoMember(2)]
        public string panelPath
        {
            get;
            set;
        }
    }
}
