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
    abstract class ProtoDecoratorBase : IProtoSerializer
    {
        public abstract Type ExpectedType { get; }
        protected readonly IProtoSerializer Tail;
        protected ProtoDecoratorBase(IProtoSerializer tail) { this.Tail = tail; }
        public abstract bool ReturnsValue { get; }
        public abstract bool RequiresOldValue { get; }
#if !FEATIKVM
        public abstract void Write(object value, ProtoWriter dest);
        public abstract object Read(object value, ProtoReader source);
#endif

#if FEATCOMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom) { EmitWrite(ctx, valueFrom); }
        protected abstract void EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom);
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom) { EmitRead(ctx, valueFrom); }
        protected abstract void EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom);
#endif
    }
}
#endif