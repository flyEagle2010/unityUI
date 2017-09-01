#if !NORUNTIME
using ProtoBuf.Meta;
namespace ProtoBuf.Serializers
{
    interface IProtoTypeSerializer : IProtoSerializer
    {
        bool HasCallbacks(TypeModel.CallbackType callbackType);
        bool CanCreateInstance();
#if !FEATIKVM
        object CreateInstance(ProtoReader source);
        void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context);
#endif
#if FEATCOMPILER
        void EmitCallback(Compiler.CompilerContext ctx, Compiler.Local valueFrom, TypeModel.CallbackType callbackType);
#endif
#if FEATCOMPILER
        void EmitCreateInstance(Compiler.CompilerContext ctx);
#endif
    }
}
#endif