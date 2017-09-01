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
    sealed class SingleSerializer : IProtoSerializer
    {
#if FEATIKVM
        readonly Type expectedType;
#else
        static readonly Type expectedType = typeof(float);
#endif
        public Type ExpectedType { get { return expectedType; } }

        public SingleSerializer(TypeModel model)
        {
#if FEATIKVM
            expectedType = model.MapType(typeof(float));
#endif
        }
        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }
#if !FEATIKVM
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return source.ReadSingle();
        }
        public void Write(object value, ProtoWriter dest)
        {
            ProtoWriter.WriteSingle((float)value, dest);
        }
#endif

#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteSingle", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadSingle", ExpectedType);
        }
#endif
    }
}
#endif