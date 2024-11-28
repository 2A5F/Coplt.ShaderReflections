using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;

namespace Coplt.ShaderReflections;

// ReSharper disable NonReadonlyMemberInGetHashCode
public record ShaderMeta
{
    public ShaderStage Stage { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint[]? ThreadGroupSize { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, ShaderBindMeta>? Res { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, ConstantBufferMeta>? Cbs { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, ShaderTypeDef>? Types { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderParam[]? Inputs { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderParam[]? Outputs { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderParam[]? Patchs { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderMetaExt[]? Exts { get; set; } = [];
}

[JsonDerivedType(typeof(Dx12ShaderMetaExt), typeDiscriminator: "dx12")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "t")]
public abstract record ShaderMetaExt;

public record Dx12ShaderMetaExt : ShaderMetaExt
{
    public required string Version { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Creator { get; set; }
    public uint Flags { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MinFeatureLevel { get; set; }
    public ulong RequiresFlags { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsSampleFrequencyShader { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderStage>))]
public enum ShaderStage
{
    Other,
    Compute,
    Pixel,
    Vertex,
    Mesh,
    Task,
    Library,
}

public record ShaderResourceMeta
{
    public required string Name { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderResourceMetaExt[]? Exts { get; set; }
}

public record ConstantBufferMeta : ShaderResourceMeta
{
    public uint Size { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, ShaderVariableMeta>? Vars { get; set; }
}

[JsonDerivedType(typeof(Dx12ConstantBufferMetaExt), typeDiscriminator: "dx12")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "t")]
public abstract record ShaderResourceMetaExt;

public record Dx12ConstantBufferMetaExt : ShaderResourceMetaExt
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }
    public uint Flags { get; set; }
}

public record ShaderVariableMeta
{
    public required string Name { get; set; }
    public string? Type { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderVariableMetaOffsetSize? Data { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderVariableMetaOffsetSize? Texture { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderVariableMetaOffsetSize? Sampler { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte[]? Defv { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderVariableMetaExt[]? Exts { get; set; }
}

public record ShaderVariableMetaOffsetSize
{
    public uint Offset { get; set; }
    public uint Size { get; set; }
}

[JsonDerivedType(typeof(Dx12ShaderVariableMetaExt), typeDiscriminator: "dx12")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "t")]
public abstract record ShaderVariableMetaExt;

public record Dx12ShaderVariableMetaExt : ShaderVariableMetaExt
{
    public uint Flags { get; set; }
}

public record ShaderBindMeta
{
    public required string Name { get; set; }
    public ShaderResourceFlags Flags { get; set; }
    public uint Point { get; set; }
    public uint Count { get; set; }
    public uint Space { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderSrvBindMeta? Srv { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ShaderBindMetaExt[]? Exts { get; set; }
}

[JsonDerivedType(typeof(Dx12ShaderBindMetaExt), typeDiscriminator: "dx12")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "t")]
public abstract record ShaderBindMetaExt;

public record Dx12ShaderBindMetaExt : ShaderBindMetaExt
{
    public uint Id { get; set; }
    public uint Flags { get; set; }
}

public record ShaderSrvBindMeta
{
    public ShaderSrvStorage Storage { get; set; }
    public ShaderSrvDimension Dimension { get; set; }
    public uint NumSamples { get; set; }
}

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<ShaderResourceFlags>))]
public enum ShaderResourceFlags
{
    None,

    Cbv = 1 << 0,
    Srv = 1 << 1,
    Uav = 1 << 2,

    Buffer = 1 << 3,
    Texture = 1 << 4,
    Sampler = 1 << 5,

    Uniform = 1 << 6,

    CBuffer = Cbv | Buffer | Uniform,
    TBuffer = Srv | Buffer | Uniform,

    IsStructured = 1 << 10,
    IsByteAddress = 1 << 11,

    RwBuffer = Buffer | Uav,
    Structured = IsStructured | Buffer | Srv,
    RwStructured = IsStructured | Buffer | Uav,
    ByteAddress = IsByteAddress | Buffer | Srv,
    RwByteAddress = IsByteAddress | Buffer | Uav,

    Counter = 1 << 16,
    Append = 1 << 17,
    Consume = 1 << 18,

    AppendStructuredBuffer = Append | RwStructured,
    ConsumeStructuredBuffer = Consume | RwStructured,

    RwStructuredWithCounter = Counter | RwStructured,

    AccelerationStructure = 1 << 20,

    FeedbackTexture = 1 << 21,
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderSrvDimension>))]
public enum ShaderSrvDimension
{
    Unknown,
    Buffer,
    Texture1D,
    Texture1DArray,
    Texture2D,
    Texture2DMs,
    Texture2DArray,
    Texture2DArrayMs,
    Texture3D,
    TextureCube,
    TextureCubeArray,
    BufferEx,
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderSrvStorage>))]
public enum ShaderSrvStorage
{
    Unknown,
    UNorm,
    SNorm,
    SInt,
    UInt,
    Float,
    Double,
    Continued,
    Mixed,
}

public record ShaderParam
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ShaderSemantic Sv { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Slot { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Index { get; set; }
    public uint Register { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public required ShaderVectorMask[] Mask { get; set; }
    public ShaderVectorComponentType Storage { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Stream { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ShaderMinPrecision Precision { get; set; }
}

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<ShaderVectorMask>))]
public enum ShaderVectorMask : byte
{
    x = 1 << 0,
    y = 1 << 1,
    z = 1 << 2,
    w = 1 << 3,

    xy = x | y,
    xz = x | z,
    xw = x | w,
    yz = y | z,
    yw = y | w,
    zw = z | w,

    xyz = x | y | z,
    xyw = x | y | w,
    xzw = x | z | w,
    yzw = y | z | w,

    xyzw = x | y | z | w,
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderVectorComponentType>))]
public enum ShaderVectorComponentType : byte
{
    Unknown,
    UInt32,
    SInt32,
    Float32,
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderSemantic>))]
public enum ShaderSemantic
{
    Undefined,
    Position,
    ClipDistance,
    CullDistance,
    RenderTargetArrayIndex,
    ViewportArrayIndex,
    VertexId,
    PrimitiveId,
    InstanceId,
    IsFrontFace,
    SampleIndex,
    FinalQuadEdgeTessFactor,
    FinalQuadInsideTessFactor,
    FinalTriEdgeTessFactor,
    FinalTriInsideTessFactor,
    FinalLineDetailTessFactor,
    FinalLineDensityTessFactor,
    Barycentrics,
    ShadingRate,
    CullPrimitive,
    Target,
    Depth,
    Coverage,
    DepthGreaterEqual,
    DepthLessEqual,
    StencilRef,
    InnerCoverage,
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderMinPrecision>))]
public enum ShaderMinPrecision
{
    Default,
    Float16,
    Float10,
    SInt16,
    UInt16,
    Any16,
    Any10,
}

public record ShaderTypeDef
{
    public required string Name { get; set; }
    public ShaderTypeKind Kind { get; set; }
    public ShaderVarType Type { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Columns { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Rows { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Length { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint Offset { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Members { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderTypeKind>))]
public enum ShaderTypeKind
{
    Scalar,
    Vector,
    MatrixRowMajor,
    MatrixColumnMajor,
    Object,
    Struct,
    Class,
    Pointer,
    DWord,
}

[JsonConverter(typeof(JsonStringEnumConverter<ShaderVarType>))]
public enum ShaderVarType
{
    Unknown,
    Void,
    Bool,
    UInt8,
    Int,
    UInt,
    Float,
    Double,
    Min8Float,
    Min10Float,
    Min16Float,
    Min12Int,
    Min16Int,
    Min16UInt,
    Int16,
    UInt16,
    Float16,
    Int64,
    UInt64,
    DWord,
    String,
    Texture,
    Texture1D,
    Texture2D,
    Texture3D,
    TextureCube,
    Texture1DArray,
    Texture2DArray,
    TextureCubeArray,
    Texture2DMs,
    Texture2DArrayMs,
    Sampler,
    Sampler1D,
    Sampler2D,
    Sampler3D,
    SamplerCube,
    RwTexture1D,
    RwTexture2D,
    RwTexture3D,
    RwTexture1DArray,
    RwTexture2DArray,
    CBuffer,
    TBuffer,
    Buffer,
    RwBuffer,
    ByteAddressBuffer,
    RwByteAddressBuffer,
    StructuredBuffer,
    RwStructuredBuffer,
    AppendStructuredBuffer,
    ConsumeStructuredBuffer,
    RenderTargetView,
    DepthStencilView,
    Blend,
    Rasterizer,
    DepthStencil,
}
