using System;

namespace ProtoBuf
{
    /// <summary>
    /// Indicates that a type is defined for protocol-buffer serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
        AllowMultiple = false, Inherited = false)]
    public sealed class ProtoContractAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the defined name of the type.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        private string name;

        /// <summary>
        /// Gets or sets the fist offset to use with implicit field tags;
        /// only uesd if ImplicitFields is set.
        /// </summary>
        public int ImplicitFirstTag
        {
            get { return implicitFirstTag; }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("ImplicitFirstTag");
                implicitFirstTag = value;
            }
        }
        private int implicitFirstTag;

        /// <summary>
        /// If specified, alternative contract markers (such as markers for XmlSerailizer or DataContractSerializer) are ignored.
        /// </summary>
        public bool UseProtoMembersOnly
        {
            get { return HasFlag(OPTIONSUseProtoMembersOnly); }
            set { SetFlag(OPTIONSUseProtoMembersOnly, value); }
        }

        /// <summary>
        /// If specified, do NOT treat this type as a list, even if it looks like one.
        /// </summary>
        public bool IgnoreListHandling
        {
            get { return HasFlag(OPTIONSIgnoreListHandling); }
            set { SetFlag(OPTIONSIgnoreListHandling, value); }
        }


        /// <summary>
        /// Gets or sets the mechanism used to automatically infer field tags
        /// for members. This option should be used in advanced scenarios only.
        /// Please review the important notes against the ImplicitFields enumeration.
        /// </summary>
        public ImplicitFields ImplicitFields { get { return implicitFields; } set { implicitFields = value; } }
        private ImplicitFields implicitFields;


        /// <summary>
        /// Enables/disables automatic tag generation based on the existing name / order
        /// of the defined members. This option is not used for members marked
        /// with ProtoMemberAttribute, as intended to provide compatibility with
        /// WCF serialization. WARNING: when adding new fields you must take
        /// care to increase the Order for new elements, otherwise data corruption
        /// may occur.
        /// </summary>
        /// <remarks>If not explicitly specified, the default is assumed from Serializer.GlobalOptions.InferTagFromName.</remarks>
        public bool InferTagFromName
        {
            get { return HasFlag(OPTIONSInferTagFromName); }
            set {
                SetFlag(OPTIONSInferTagFromName, value);
                SetFlag(OPTIONSInferTagFromNameHasValue, true);
            }
        }

        /// <summary>
        /// Has a InferTagFromName value been explicitly set? if not, the default from the type-model is assumed.
        /// </summary>
        internal bool InferTagFromNameHasValue
        { // note that this property is accessed via reflection and should not be removed
            get { return HasFlag(OPTIONSInferTagFromNameHasValue); }
        }

        private int dataMemberOffset;

        /// <summary>
        /// Specifies an offset to apply to [DataMember(Order=...)] markers;
        /// this is useful when working with mex-generated classes that have
        /// a different origin (usually 1 vs 0) than the original data-contract.
        /// 
        /// This value is added to the Order of each member.
        /// </summary>
        public int DataMemberOffset
        {
            get { return dataMemberOffset; }
            set { dataMemberOffset = value; }
        }


        /// <summary>
        /// If true, the constructor for the type is bypassed during deserialization, meaning any field initializers
        /// or other initialization code is skipped.
        /// </summary>
        public bool SkipConstructor
        {
            get { return HasFlag(OPTIONSSkipConstructor); }
            set { SetFlag(OPTIONSSkipConstructor, value); }
        }

        /// <summary>
        /// Should this type be treated as a reference by default? Please also see the implications of this,
        /// as recorded on ProtoMemberAttribute.AsReference
        /// </summary>
        public bool AsReferenceDefault
        {
            get { return HasFlag(OPTIONSAsReferenceDefault); }
            set {
                SetFlag(OPTIONSAsReferenceDefault, value);
            }
        }

        private bool HasFlag(byte flag) { return (flags & flag) == flag; }
        private void SetFlag(byte flag, bool value)
        {
            if (value) flags |= flag;
            else flags = (byte)(flags & ~flag);
        }

        private byte flags;

        private const byte
            OPTIONSInferTagFromName = 1,
            OPTIONSInferTagFromNameHasValue = 2,
            OPTIONSUseProtoMembersOnly = 4,
            OPTIONSSkipConstructor = 8,
            OPTIONSIgnoreListHandling = 16,
            OPTIONSAsReferenceDefault = 32,
            OPTIONSEnumPassthru = 64,
            OPTIONSEnumPassthruHasValue = 128;

        /// <summary>
        /// Applies only to enums (not to DTO classes themselves); gets or sets a value indicating that an enum should be treated directly as an int/short/etc, rather
        /// than enforcing .proto enum rules. This is useful *in particul* for [Flags] enums.
        /// </summary>
        public bool EnumPassthru
        {
            get { return HasFlag(OPTIONSEnumPassthru); }
            set {
                SetFlag(OPTIONSEnumPassthru, value);
                SetFlag(OPTIONSEnumPassthruHasValue, true);
            }
        }

        /// <summary>
        /// Has a EnumPassthru value been explicitly set?
        /// </summary>
        internal bool EnumPassthruHasValue
        { // note that this property is accessed via reflection and should not be removed
            get { return HasFlag(OPTIONSEnumPassthruHasValue); }
        }
    }
}