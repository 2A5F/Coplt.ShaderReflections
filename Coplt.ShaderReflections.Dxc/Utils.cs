using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;

namespace Coplt.ShaderReflections.Dx;

public static class Utils
{
    public static string VersionToString(uint version)
    {
        var type = (ShaderVersionType)((version & 0xFFFF0000) >> 16);
        var t = type switch
        {
            ShaderVersionType.PixelShader => "ps",
            ShaderVersionType.VertexShader => "vs",
            ShaderVersionType.GeometryShader => "gs",
            ShaderVersionType.HullShader => "hs",
            ShaderVersionType.DomainShader => "ds",
            ShaderVersionType.ComputeShader => "cs",
            ShaderVersionType.Library => "lib",
            ShaderVersionType.RayGenerationShader => "ray_generation",
            ShaderVersionType.IntersectionShader => "intersection",
            ShaderVersionType.AnyHitShader => "any_hit",
            ShaderVersionType.ClosestHitShader => "closest_hit",
            ShaderVersionType.MissShader => "miss",
            ShaderVersionType.CallableShader => "callable",
            ShaderVersionType.MeshShader => "ms",
            ShaderVersionType.AmplificationShader => "as",
            _ => "unknown",
        };
        return $"{t}_{(version & 0x000000F0) >> 4}_{version & 0x0000000F}";
    }

    public static ShaderStage VersionToStage(uint version)
    {
        var type = (ShaderVersionType)((version & 0xFFFF0000) >> 16);
        return type switch
        {
            ShaderVersionType.PixelShader => ShaderStage.Pixel,
            ShaderVersionType.VertexShader => ShaderStage.Vertex,
            ShaderVersionType.GeometryShader => ShaderStage.Other,
            ShaderVersionType.HullShader => ShaderStage.Other,
            ShaderVersionType.DomainShader => ShaderStage.Other,
            ShaderVersionType.ComputeShader => ShaderStage.Compute,
            ShaderVersionType.Library => ShaderStage.Library,
            ShaderVersionType.RayGenerationShader => ShaderStage.Other,
            ShaderVersionType.IntersectionShader => ShaderStage.Other,
            ShaderVersionType.AnyHitShader => ShaderStage.Other,
            ShaderVersionType.ClosestHitShader => ShaderStage.Other,
            ShaderVersionType.MissShader => ShaderStage.Other,
            ShaderVersionType.CallableShader => ShaderStage.Other,
            ShaderVersionType.MeshShader => ShaderStage.Mesh,
            ShaderVersionType.AmplificationShader => ShaderStage.Task,
            _ => ShaderStage.Other,
        };
    }

    public static ShaderResourceFlags ToShaderResourceFlags(this D3DShaderInputType type) => type switch
    {
        D3DShaderInputType.D3DSitCbuffer => ShaderResourceFlags.CBuffer,
        D3DShaderInputType.D3DSitTbuffer => ShaderResourceFlags.TBuffer,
        D3DShaderInputType.D3DSitTexture => ShaderResourceFlags.Texture,
        D3DShaderInputType.D3DSitSampler => ShaderResourceFlags.Sampler,
        D3DShaderInputType.D3DSitUavRwtyped => ShaderResourceFlags.RwBuffer,
        D3DShaderInputType.D3DSitStructured => ShaderResourceFlags.Structured,
        D3DShaderInputType.D3DSitUavRwstructured => ShaderResourceFlags.RwStructured,
        D3DShaderInputType.D3DSitByteaddress => ShaderResourceFlags.ByteAddress,
        D3DShaderInputType.D3DSitUavRwbyteaddress => ShaderResourceFlags.RwByteAddress,
        D3DShaderInputType.D3DSitUavAppendStructured => ShaderResourceFlags.AppendStructuredBuffer,
        D3DShaderInputType.D3DSitUavConsumeStructured => ShaderResourceFlags.ConsumeStructuredBuffer,
        D3DShaderInputType.D3DSitUavRwstructuredWithCounter => ShaderResourceFlags.RwStructuredWithCounter,
        D3DShaderInputType.D3DSitRtaccelerationstructure => ShaderResourceFlags.AccelerationStructure,
        D3DShaderInputType.D3DSitUavFeedbacktexture => ShaderResourceFlags.FeedbackTexture,
        _ => ShaderResourceFlags.None,
    };

    public static ShaderTexStorage ToShaderTexStorage(this D3DResourceReturnType type) => type switch
    {
        D3DResourceReturnType.D3DReturnTypeUnorm => ShaderTexStorage.UNorm,
        D3DResourceReturnType.D3DReturnTypeSNorm => ShaderTexStorage.SNorm,
        D3DResourceReturnType.D3DReturnTypeSint => ShaderTexStorage.SInt,
        D3DResourceReturnType.D3DReturnTypeUint => ShaderTexStorage.UInt,
        D3DResourceReturnType.D3DReturnTypeFloat => ShaderTexStorage.Float,
        D3DResourceReturnType.D3DReturnTypeMixed => ShaderTexStorage.Mixed,
        D3DResourceReturnType.D3DReturnTypeDouble => ShaderTexStorage.Double,
        D3DResourceReturnType.D3DReturnTypeContinued => ShaderTexStorage.Continued,
        _ => ShaderTexStorage.Unknown,
    };

    public static ShaderResDimension ToShaderTexDimension(this D3DSrvDimension dimension) =>
        dimension switch
        {
            D3DSrvDimension.D3DSrvDimensionBuffer => ShaderResDimension.Buffer,
            D3DSrvDimension.D3DSrvDimensionTexture1D => ShaderResDimension.Texture1D,
            D3DSrvDimension.D3DSrvDimensionTexture1Darray => ShaderResDimension.Texture1DArray,
            D3DSrvDimension.D3DSrvDimensionTexture2D => ShaderResDimension.Texture2D,
            D3DSrvDimension.D3DSrvDimensionTexture2Darray => ShaderResDimension.Texture2DArray,
            D3DSrvDimension.D3DSrvDimensionTexture2Dms => ShaderResDimension.Texture2DMs,
            D3DSrvDimension.D3DSrvDimensionTexture2Dmsarray => ShaderResDimension.Texture2DArrayMs,
            D3DSrvDimension.D3DSrvDimensionTexture3D => ShaderResDimension.Texture3D,
            D3DSrvDimension.D3DSrvDimensionTexturecube => ShaderResDimension.TextureCube,
            D3DSrvDimension.D3DSrvDimensionTexturecubearray => ShaderResDimension.TextureCubeArray,
            D3DSrvDimension.D3DSrvDimensionBufferex => ShaderResDimension.BufferEx,
            _ => ShaderResDimension.Unknown,
        };

    public static ShaderVectorComponentType ToShaderVectorComponentType(this D3DRegisterComponentType type) =>
        type switch
        {
            D3DRegisterComponentType.D3DRegisterComponentUint32 => ShaderVectorComponentType.UInt32,
            D3DRegisterComponentType.D3DRegisterComponentSint32 => ShaderVectorComponentType.SInt32,
            D3DRegisterComponentType.D3DRegisterComponentFloat32 => ShaderVectorComponentType.Float32,
            _ => ShaderVectorComponentType.Unknown,
        };

    public static ShaderSemantic ToShaderSemantic(this D3DName name) => name switch
    {
        D3DName.D3DNamePosition => ShaderSemantic.Position,
        D3DName.D3DNameClipDistance => ShaderSemantic.ClipDistance,
        D3DName.D3DNameCullDistance => ShaderSemantic.CullDistance,
        D3DName.D3DNameRenderTargetArrayIndex => ShaderSemantic.RenderTargetArrayIndex,
        D3DName.D3DNameViewportArrayIndex => ShaderSemantic.ViewportArrayIndex,
        D3DName.D3DNameVertexID => ShaderSemantic.VertexId,
        D3DName.D3DNamePrimitiveID => ShaderSemantic.PrimitiveId,
        D3DName.D3DNameInstanceID => ShaderSemantic.InstanceId,
        D3DName.D3DNameIsFrontFace => ShaderSemantic.IsFrontFace,
        D3DName.D3DNameSampleIndex => ShaderSemantic.SampleIndex,
        D3DName.D3DNameFinalQuadEdgeTessfactor => ShaderSemantic.FinalQuadEdgeTessFactor,
        D3DName.D3DNameFinalQuadInsideTessfactor => ShaderSemantic.FinalQuadInsideTessFactor,
        D3DName.D3DNameFinalTriEdgeTessfactor => ShaderSemantic.FinalTriEdgeTessFactor,
        D3DName.D3DNameFinalTriInsideTessfactor => ShaderSemantic.FinalTriInsideTessFactor,
        D3DName.D3DNameFinalLineDetailTessfactor => ShaderSemantic.FinalLineDetailTessFactor,
        D3DName.D3DNameFinalLineDensityTessfactor => ShaderSemantic.FinalLineDensityTessFactor,
        D3DName.D3DNameBarycentrics => ShaderSemantic.Barycentrics,
        D3DName.D3DNameShadingrate => ShaderSemantic.ShadingRate,
        D3DName.D3DNameCullprimitive => ShaderSemantic.CullPrimitive,
        D3DName.D3DNameTarget => ShaderSemantic.Target,
        D3DName.D3DNameDepth => ShaderSemantic.Depth,
        D3DName.D3DNameCoverage => ShaderSemantic.Coverage,
        D3DName.D3DNameDepthGreaterEqual => ShaderSemantic.DepthGreaterEqual,
        D3DName.D3DNameDepthLessEqual => ShaderSemantic.DepthLessEqual,
        D3DName.D3DNameStencilRef => ShaderSemantic.StencilRef,
        D3DName.D3DNameInnerCoverage => ShaderSemantic.InnerCoverage,
        _ => ShaderSemantic.Undefined,
    };

    public static ShaderMinPrecision ToShaderMinPrecision(this D3DMinPrecision precision) => precision switch
    {
        D3DMinPrecision.Float16 => ShaderMinPrecision.Float16,
        D3DMinPrecision.Float28 => ShaderMinPrecision.Float10,
        D3DMinPrecision.Sint16 => ShaderMinPrecision.SInt16,
        D3DMinPrecision.Uint16 => ShaderMinPrecision.UInt16,
        D3DMinPrecision.Any16 => ShaderMinPrecision.Any16,
        D3DMinPrecision.Any10 => ShaderMinPrecision.Any10,
        _ => ShaderMinPrecision.Default,
    };

    public static ShaderTypeKind ToShaderTypeKind(this D3DShaderVariableClass @class) => @class switch
    {
        D3DShaderVariableClass.D3DSvcVector => ShaderTypeKind.Vector,
        D3DShaderVariableClass.D3DSvcMatrixRows => ShaderTypeKind.MatrixRowMajor,
        D3DShaderVariableClass.D3DSvcMatrixColumns => ShaderTypeKind.MatrixColumnMajor,
        D3DShaderVariableClass.D3DSvcObject => ShaderTypeKind.Object,
        D3DShaderVariableClass.D3DSvcStruct => ShaderTypeKind.Struct,
        D3DShaderVariableClass.D3DSvcInterfaceClass => ShaderTypeKind.Class,
        D3DShaderVariableClass.D3DSvcInterfacePointer => ShaderTypeKind.Pointer,
        D3DShaderVariableClass.D3DSvcForceDword => ShaderTypeKind.DWord,
        _ => ShaderTypeKind.Scalar,
    };

    public static ShaderVarType ToShaderVarType(this D3DShaderVariableType type) => type switch
    {
        D3DShaderVariableType.D3DSvtVoid => ShaderVarType.Void,
        D3DShaderVariableType.D3DSvtBool => ShaderVarType.Bool,
        D3DShaderVariableType.D3DSvtInt => ShaderVarType.Int,
        D3DShaderVariableType.D3DSvtFloat => ShaderVarType.Float,
        D3DShaderVariableType.D3DSvtString => ShaderVarType.String,
        D3DShaderVariableType.D3DSvtTexture => ShaderVarType.Texture,
        D3DShaderVariableType.D3DSvtTexture1D => ShaderVarType.Texture1D,
        D3DShaderVariableType.D3DSvtTexture2D => ShaderVarType.Texture2D,
        D3DShaderVariableType.D3DSvtTexture3D => ShaderVarType.Texture3D,
        D3DShaderVariableType.D3DSvtTexturecube => ShaderVarType.TextureCube,
        D3DShaderVariableType.D3DSvtSampler => ShaderVarType.Sampler,
        D3DShaderVariableType.D3DSvtSampler1D => ShaderVarType.Sampler1D,
        D3DShaderVariableType.D3DSvtSampler2D => ShaderVarType.Sampler2D,
        D3DShaderVariableType.D3DSvtSampler3D => ShaderVarType.Sampler3D,
        D3DShaderVariableType.D3DSvtSamplercube => ShaderVarType.SamplerCube,
        D3DShaderVariableType.D3DSvtUint => ShaderVarType.UInt,
        D3DShaderVariableType.D3DSvtUint8 => ShaderVarType.UInt8,
        D3DShaderVariableType.D3DSvtRasterizer => ShaderVarType.Rasterizer,
        D3DShaderVariableType.D3DSvtDepthstencil => ShaderVarType.DepthStencil,
        D3DShaderVariableType.D3DSvtBlend => ShaderVarType.Blend,
        D3DShaderVariableType.D3DSvtBuffer => ShaderVarType.Buffer,
        D3DShaderVariableType.D3DSvtCbuffer => ShaderVarType.CBuffer,
        D3DShaderVariableType.D3DSvtTbuffer => ShaderVarType.TBuffer,
        D3DShaderVariableType.D3DSvtTexture1Darray => ShaderVarType.Texture1DArray,
        D3DShaderVariableType.D3DSvtTexture2Darray => ShaderVarType.Texture2DArray,
        D3DShaderVariableType.D3DSvtRendertargetview => ShaderVarType.RenderTargetView,
        D3DShaderVariableType.D3DSvtDepthstencilview => ShaderVarType.DepthStencilView,
        D3DShaderVariableType.D3DSvtTexture2Dms => ShaderVarType.Texture2DMs,
        D3DShaderVariableType.D3DSvtTexture2Dmsarray => ShaderVarType.Texture2DArrayMs,
        D3DShaderVariableType.D3DSvtTexturecubearray => ShaderVarType.TextureCubeArray,
        D3DShaderVariableType.D3DSvtDouble => ShaderVarType.Double,
        D3DShaderVariableType.D3DSvtRwtexture1D => ShaderVarType.RwTexture1D,
        D3DShaderVariableType.D3DSvtRwtexture1Darray => ShaderVarType.RwTexture1DArray,
        D3DShaderVariableType.D3DSvtRwtexture2D => ShaderVarType.RwTexture2D,
        D3DShaderVariableType.D3DSvtRwtexture2Darray => ShaderVarType.RwTexture2DArray,
        D3DShaderVariableType.D3DSvtRwtexture3D => ShaderVarType.RwTexture3D,
        D3DShaderVariableType.D3DSvtRwbuffer => ShaderVarType.RwBuffer,
        D3DShaderVariableType.D3DSvtByteaddressBuffer => ShaderVarType.ByteAddressBuffer,
        D3DShaderVariableType.D3DSvtRwbyteaddressBuffer => ShaderVarType.RwByteAddressBuffer,
        D3DShaderVariableType.D3DSvtStructuredBuffer => ShaderVarType.StructuredBuffer,
        D3DShaderVariableType.D3DSvtRwstructuredBuffer => ShaderVarType.RwStructuredBuffer,
        D3DShaderVariableType.D3DSvtAppendStructuredBuffer => ShaderVarType.AppendStructuredBuffer,
        D3DShaderVariableType.D3DSvtConsumeStructuredBuffer => ShaderVarType.ConsumeStructuredBuffer,
        D3DShaderVariableType.D3DSvtMin8float => ShaderVarType.Min8Float,
        D3DShaderVariableType.D3DSvtMin10float => ShaderVarType.Min10Float,
        D3DShaderVariableType.D3DSvtMin16float => ShaderVarType.Min16Float,
        D3DShaderVariableType.D3DSvtMin12int => ShaderVarType.Min12Int,
        D3DShaderVariableType.D3DSvtMin16int => ShaderVarType.Min16Int,
        D3DShaderVariableType.D3DSvtMin16Uint => ShaderVarType.Min16UInt,
        D3DShaderVariableType.D3DSvtInt16 => ShaderVarType.Int16,
        D3DShaderVariableType.D3DSvtUint16 => ShaderVarType.UInt16,
        D3DShaderVariableType.D3DSvtFloat16 => ShaderVarType.Float16,
        D3DShaderVariableType.D3DSvtInt64 => ShaderVarType.Int64,
        D3DShaderVariableType.D3DSvtUint64 => ShaderVarType.UInt64,
        D3DShaderVariableType.D3DSvtForceDword => ShaderVarType.DWord,
        _ => ShaderVarType.Unknown,
    };
}
