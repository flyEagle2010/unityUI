#if !NORUNTIME
using System;

#if FEATIKVM
using Type = IKVM.Reflection.Type;
#endif



namespace ProtoBuf.Serializers
{
    sealed class ByteSerializer : IProtoSerializer
    {
        public Type ExpectedType { get { return expectedType; } }

#if FEATIKVM
        readonly Type expectedType;
#else
        static readonly Type expectedType = typeof(byte);
#endif
        public ByteSerializer(ProtoBuf.Meta.TypeModel model)
        {
#if FEATIKVM
            expectedType = model.MapType(typeof(byte));
#endif
        }
        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }
#if !FEATIKVM
        public void Write(object value, ProtoWriter dest)
        {
            ProtoWriter.WriteByte((byte)value, dest);
        }
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return source.ReadByte();
        }
#endif

#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicWrite("WriteByte", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead("ReadByte", ExpectedType);
        }
#endif

    }
}
#endif