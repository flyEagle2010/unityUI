#if !NORUNTIME
using System;

#if FEATIKVM
using Type = IKVM.Reflection.Type;
using IKVM.Reflection;
#else
using System.Reflection;
#endif

namespace ProtoBuf.Serializers
{
    sealed class SByteSerializer : IProtoSerializer
    {
#if FEATIKVM
        readonly Type expectedType;
#else
        static readonly Type expectedType = typeof(sbyte);
#endif
        public SByteSerializer(ProtoBuf.Meta.TypeModel model)
        {
#if FEATIKVM
            expectedType = model.MapType(typeof(sbyte));
#endif
        }
        public Type ExpectedType { get { return expectedType; } }


        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }
#if !FEATIKVM
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return source.ReadSByte();
        }
        public void Write(object value, ProtoWriter dest)
        {
            ProtoWriter.WriteSByte((sbyte)value, dest);
        }
#endif
#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteSByte", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadSByte", ExpectedType);
        }
#endif

    }
}
#endif