#if !NORUNTIME
using System;

#if FEATIKVM
using Type = IKVM.Reflection.Type;
#endif


namespace ProtoBuf.Serializers
{
    sealed class GuidSerializer : IProtoSerializer
    {
#if FEATIKVM
        readonly Type expectedType;
#else
        static readonly Type expectedType = typeof(Guid);
#endif
        public GuidSerializer(ProtoBuf.Meta.TypeModel model)
        {
#if FEATIKVM
            expectedType = model.MapType(typeof(Guid));
#endif
        }

        public Type ExpectedType { get { return expectedType; } }

        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }

#if !FEATIKVM
        public void Write(object value, ProtoWriter dest)
        {
            BclHelpers.WriteGuid((Guid)value, dest);
        }
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return BclHelpers.ReadGuid(source);
        }
#endif

#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)), "WriteGuid", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)), "ReadGuid", ExpectedType);
        }
#endif

    }
}
#endif