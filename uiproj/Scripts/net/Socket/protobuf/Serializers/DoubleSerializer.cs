#if !NORUNTIME
using System;
using ProtoBuf.Meta;

#if FEATIKVM
using Type = IKVM.Reflection.Type;
using IKVM.Reflection;
#else
using System.Reflection;
#endif

namespace ProtoBuf.Serializers
{
    sealed class DoubleSerializer : IProtoSerializer
    {
#if FEATIKVM
        readonly Type expectedType;
#else
        static readonly Type expectedType = typeof(double);
#endif
        public DoubleSerializer(ProtoBuf.Meta.TypeModel model)
        {
#if FEATIKVM
            expectedType = model.MapType(typeof(double));
#endif
        }

        public Type ExpectedType { get { return expectedType; } }
        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }
#if !FEATIKVM
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return source.ReadDouble();
        }
        public void Write(object value, ProtoWriter dest)
        {
            ProtoWriter.WriteDouble((double)value, dest);
        }
#endif
#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteDouble", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadDouble", ExpectedType);
        }
#endif
    }
}
#endif