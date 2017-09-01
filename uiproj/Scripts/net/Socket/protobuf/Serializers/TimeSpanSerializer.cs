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
    sealed class TimeSpanSerializer : IProtoSerializer
    {
#if FEATIKVM
        readonly Type expectedType;
#else
        static readonly Type expectedType = typeof(TimeSpan);
#endif
        public TimeSpanSerializer(ProtoBuf.Meta.TypeModel model)
        {
#if FEATIKVM
            expectedType = model.MapType(typeof(TimeSpan));
#endif
        }
        public Type ExpectedType { get { return expectedType; } }

        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }
#if !FEATIKVM
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return BclHelpers.ReadTimeSpan(source);
        }
        public void Write(object value, ProtoWriter dest)
        {
            BclHelpers.WriteTimeSpan((TimeSpan)value, dest);
        }
#endif
#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)), "WriteTimeSpan", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)), "ReadTimeSpan", ExpectedType);
        }
#endif

    }
}
#endif