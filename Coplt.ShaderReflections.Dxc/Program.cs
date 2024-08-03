using System.Runtime.InteropServices;
using System.Text.Json;
using CommandLine;
using Coplt.ShaderReflections;
using Coplt.ShaderReflections.Dx;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D12;
using Spectre.Console;
using Spectre.Console.Json;
using Buffer = Silk.NET.Direct3D.Compilers.Buffer;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(App.Run);
return 0;

public record Options
{
    [Value(0, MetaName = "input path", HelpText = "Input dxc reflection file", Required = true)]
    public required string Input { get; set; }
    [Value(1, MetaName = "output path", HelpText = "Output json file")]
    public string? Output { get; set; }
}

internal static unsafe class App
{
    public static void Run(Options opts)
    {
        var input = Path.GetFullPath(opts.Input);
        var output = opts.Output is null ? null : Path.GetFullPath(opts.Output);
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow(new Text("Input: ", new Style(Color.Green)).RightJustified(),
            new TextPath(input));
        if (output is not null)
            grid.AddRow(new Text("Output: ", new Style(Color.Gold1)).RightJustified(),
                new TextPath(output));
        AnsiConsole.Write(grid);
        AnsiConsole.Write(new Rule());
        var dxc = DXC.GetApi();
        var utils_guid = new Guid("{6245D6AF-66E0-48FD-80B4-4D271796748C}");
        using var utils = dxc.CreateInstance<IDxcUtils>(&utils_guid);

        var data = File.ReadAllBytes(input);
        ComPtr<ID3D12ShaderReflection> _reflection;
        fixed (byte* p_data = data)
        {
            var buff = new Buffer(p_data, (nuint)data.Length);
            _reflection = utils.CreateReflection<ID3D12ShaderReflection>(&buff);
        }
        using var reflection = _reflection;

        var IsSampleFrequencyShader = reflection.IsSampleFrequencyShader();

        D3DFeatureLevel level;
        SilkMarshal.ThrowHResult(reflection.GetMinFeatureLevel(&level));

        var req_flags = reflection.GetRequiresFlags();

        uint ThreadGroupSizeX, ThreadGroupSizeY, ThreadGroupSizeZ;
        var ThreadGroupSize = reflection.GetThreadGroupSize(
            &ThreadGroupSizeX, &ThreadGroupSizeY, &ThreadGroupSizeZ);

        ShaderDesc desc;
        SilkMarshal.ThrowHResult(reflection.GetDesc(&desc));

        var types = new Dictionary<string, ShaderTypeDef>();

        var bindings = new Dictionary<string, ShaderBindMeta>();
        for (var i = 0u; i < desc.BoundResources; i++)
        {
            var b = BuildResBind(ref reflection.Get(), i);
            bindings[b.Name] = b;
        }

        var cbs = new Dictionary<string, ConstantBufferMeta>();
        for (var i = 0u; i < desc.ConstantBuffers; i++)
        {
            var cb = reflection.GetConstantBufferByIndex(i);
            var cbcb = BuildCb(types, cb);
            cbs[cbcb.Name] = cbcb;
        }

        var ip = new ShaderParam[desc.InputParameters];
        for (var i = 0u; i < desc.InputParameters; i++)
        {
            var p = BuildInputParam(ref reflection.Get(), i);
            ip[i] = p;
        }

        var op = new ShaderParam[desc.OutputParameters];
        for (var i = 0u; i < desc.OutputParameters; i++)
        {
            var p = BuildOutputParam(ref reflection.Get(), i);
            op[i] = p;
        }

        var pp = new ShaderParam[desc.PatchConstantParameters];
        for (var i = 0u; i < desc.PatchConstantParameters; i++)
        {
            var p = BuildPatchParam(ref reflection.Get(), i);
            op[i] = p;
        }

        var ext = new Dx12ShaderMetaExt
        {
            Version = Utils.VersionToString(desc.Version),
            Creator = Marshal.PtrToStringUTF8((IntPtr)desc.Creator),
            Flags = desc.Flags,
            MinFeatureLevel = level.ToString(),
            RequiresFlags = req_flags,
            IsSampleFrequencyShader = IsSampleFrequencyShader,
        };
        var meta = new ShaderMeta
        {
            Stage = Utils.VersionToStage(desc.Version),
            ThreadGroupSize = ThreadGroupSize == 0 ? null : [ThreadGroupSizeX, ThreadGroupSizeY, ThreadGroupSizeZ],
            Res = bindings.Count > 0 ? bindings : null,
            Cbs = cbs.Count > 0 ? cbs : null,
            Types = types.Count > 0 ? types : null,
            Inputs = ip.Length > 0 ? ip : null,
            Outputs = op.Length > 0 ? op : null,
            Patchs = pp.Length > 0 ? pp : null,
            Exts = [ext],
        };

        if (output is null)
        {
            var debug_json = JsonSerializer.Serialize(meta, ShaderMetaJsonDebugContext.Default.ShaderMeta);
            AnsiConsole.Write(new JsonText(debug_json));
        }
        else
        {
            var json = JsonSerializer.Serialize(meta, ShaderMetaJsonContext.Default.ShaderMeta);
            File.WriteAllText(output, json);
        }
    }

    public static ConstantBufferMeta BuildCb(Dictionary<string, ShaderTypeDef> types,
        ID3D12ShaderReflectionConstantBuffer* cb)
    {
        ShaderBufferDesc desc;
        SilkMarshal.ThrowHResult(cb->GetDesc(&desc));

        var vars = new Dictionary<string, ShaderVariableMeta>();
        for (var i = 0u; i < desc.Variables; i++)
        {
            var v = cb->GetVariableByIndex(i);
            var vv = BuildVar(types, v);
            vars.Add(vv.Name, vv);
        }

        return new ConstantBufferMeta
        {
            Name = Marshal.PtrToStringUTF8((IntPtr)desc.Name)!,
            Size = desc.Size,
            Vars = vars.Count > 0 ? vars : null,
            Exts =
            [
                new Dx12ConstantBufferMetaExt
                {
                    Type = desc.Type.ToString(),
                    Flags = desc.UFlags,
                }
            ]
        };
    }

    public static ShaderVariableMeta BuildVar(Dictionary<string, ShaderTypeDef> types,
        ID3D12ShaderReflectionVariable* v)
    {
        ShaderVariableDesc desc;
        SilkMarshal.ThrowHResult(v->GetDesc(&desc));

        var Data = desc.StartOffset != 0 || desc.Size != 0
            ? new ShaderVariableMetaOffsetSize
            {
                Offset = desc.StartOffset,
                Size = desc.Size,
            }
            : null;

        var Texture = desc.StartTexture != 0 || desc.TextureSize != 0
            ? new ShaderVariableMetaOffsetSize
            {
                Offset = desc.StartTexture,
                Size = desc.TextureSize,
            }
            : null;

        var Sampler = desc.StartSampler != 0 || desc.SamplerSize != 0
            ? new ShaderVariableMetaOffsetSize
            {
                Offset = desc.StartSampler,
                Size = desc.SamplerSize,
            }
            : null;

        var type = BuildType(types, v->GetType());

        return new ShaderVariableMeta
        {
            Name = Marshal.PtrToStringUTF8((IntPtr)desc.Name)!,
            Type = type.Name,
            Data = Data,
            Texture = Texture,
            Sampler = Sampler,
            Defv = desc.DefaultValue is not null ? new Span<byte>(desc.DefaultValue, (int)desc.Size).ToArray() : null,
            Exts =
            [
                new Dx12ShaderVariableMetaExt
                {
                    Flags = desc.UFlags,
                }
            ]
        };
    }

    public static ShaderBindMeta BuildResBind(ref ID3D12ShaderReflection reflection, uint i)
    {
        ShaderInputBindDesc desc;
        SilkMarshal.ThrowHResult(reflection.GetResourceBindingDesc(i, &desc));

        var texture = desc.ReturnType != D3DResourceReturnType.None || desc.NumSamples != 0 || desc.Dimension != 0
            ? new ShaderSrvBindMeta
            {
                Storage = desc.ReturnType.ToShaderSrvStorage(),
                Dimension = desc.Dimension.ToShaderSrvDimension(),
                NumSamples = desc.NumSamples,
            }
            : null;

        return new ShaderBindMeta
        {
            Name = Marshal.PtrToStringUTF8((IntPtr)desc.Name)!,
            Flags = desc.Type.ToShaderResourceFlags(),
            Point = desc.BindPoint,
            Count = desc.BindCount,
            Space = desc.Space,
            Srv = texture,
            Exts =
            [
                new Dx12ShaderBindMetaExt
                {
                    Id = desc.UID,
                    Flags = desc.UFlags,
                }
            ]
        };
    }

    public static ShaderParam BuildOutputParam(ref ID3D12ShaderReflection reflection, uint i)
    {
        SignatureParameterDesc desc;
        SilkMarshal.ThrowHResult(reflection.GetOutputParameterDesc(i, &desc));
        return BuildParam(ref desc);
    }

    public static ShaderParam BuildInputParam(ref ID3D12ShaderReflection reflection, uint i)
    {
        SignatureParameterDesc desc;
        SilkMarshal.ThrowHResult(reflection.GetInputParameterDesc(i, &desc));
        return BuildParam(ref desc);
    }

    public static ShaderParam BuildPatchParam(ref ID3D12ShaderReflection reflection, uint i)
    {
        SignatureParameterDesc desc;
        SilkMarshal.ThrowHResult(reflection.GetPatchConstantParameterDesc(i, &desc));
        return BuildParam(ref desc);
    }

    public static ShaderParam BuildParam(ref SignatureParameterDesc desc)
    {
        return new ShaderParam
        {
            Sv = desc.SystemValueType.ToShaderSemantic(),
            Slot = Marshal.PtrToStringUTF8((IntPtr)desc.SemanticName),
            Index = desc.SemanticIndex,
            Register = desc.Register,
            Mask = [(ShaderVectorMask)desc.Mask, (ShaderVectorMask)desc.ReadWriteMask],
            Storage = desc.ComponentType.ToShaderVectorComponentType(),
            Stream = desc.Stream,
            Precision = desc.MinPrecision.ToShaderMinPrecision(),
        };
    }

    public static ShaderTypeDef BuildType(Dictionary<string, ShaderTypeDef> types, ID3D12ShaderReflectionType* type)
    {
        ShaderTypeDesc desc;
        SilkMarshal.ThrowHResult(type->GetDesc(&desc));
        var name = Marshal.PtrToStringUTF8((IntPtr)desc.Name);
        if (name is null) name = Guid.NewGuid().ToString();
        else if (types.TryGetValue(name, out var tt)) return tt;
        var members = new string[desc.Members];
        var t = new ShaderTypeDef
        {
            Name = name,
            Kind = desc.Class.ToShaderTypeKind(),
            Type = desc.Type.ToShaderVarType(),
            Offset = desc.Offset,
            Columns = desc.Columns,
            Rows = desc.Rows,
            Length = desc.Elements,
            Members = members.Length > 0 ? members : null,
        };
        types.Add(t.Name, t);
        for (var i = 0u; i < desc.Members; i++)
        {
            var m = type->GetMemberTypeByIndex(i);
            members[i] = BuildType(types, m).Name;
        }
        return t;
    }
}
