using Shader = UnityEngine.Shader;

/// <summary> Holds global refs for shader property hashes </summary>
public sealed class ShaderID
{
	/// <summary> Arbitrary colour ID </summary>
	public static readonly int _Color = Shader.PropertyToID("_Color");
	/// <summary> Custom depth colour ID </summary>
	public static readonly int _DepthColor = Shader.PropertyToID("_DepthColor");

	/// <summary> Used for manual dissolve of pickupables </summary>
	public static readonly int _ManualDissolve = Shader.PropertyToID("_ManualDissolve");
	/// <summary> Used for watercolour fresnel effect </summary>
	public static readonly int _ViewDir = Shader.PropertyToID("_ViewDir");
	/// <summary> Used for mask heart world texture </summary>
	public static readonly int _Heart = Shader.PropertyToID("_Heart");
	/// <summary> Used for mask heart world texture </summary>
	public static readonly int _HeartDepthNormals = Shader.PropertyToID("_HeartDepthNormals");

	/// <summary> Used for ripple anim </summary>
	public static readonly int _RippleOffset = Shader.PropertyToID("_RippleOffset");
	/// <summary> Used for mask </summary>
	public static readonly int _Mask = Shader.PropertyToID("_Mask");
	/// <summary> Used for mask ramp anim </summary>
	public static readonly int _MaskCutoff = Shader.PropertyToID("_MaskCutoff");
	/// <summary> Used for mask ramp anim </summary>
	public static readonly int _RampTex = Shader.PropertyToID("_RampTex");

	/// <summary> Used to store bird draw calls </summary>
	public static readonly int _BirdMap = Shader.PropertyToID("_BirdMap");
	/// <summary> Used to store bird command buffer </summary>
	public static readonly int _BirdTemp = Shader.PropertyToID("_BirdTemp");
	/// <summary> Used to stop glow draw calls </summary>
	public static readonly int _GlowMap = Shader.PropertyToID("_GlowMap");
	/// <summary> Used to store glow command buffer </summary>
	public static readonly int _GlowTemp = Shader.PropertyToID("_GlowTemp");

	/// <summary> Used for page flip, page rotation </summary>
	public static readonly int _Rho = Shader.PropertyToID("_Rho");
	/// <summary> Used for page flip, page curvature </summary>
	public static readonly int _Theta = Shader.PropertyToID("_Theta");
	/// <summary> Used for page flip, page position </summary>
	public static readonly int _BottomLeft = Shader.PropertyToID("_BottomLeft");

	/// <summary> Used for frost shader, drives anim </summary>
	public static readonly int _TransitionCutoff = Shader.PropertyToID("_TransitionCutoff");

	/// <summary> Used for wave pos </summary>
	public static readonly int _WaveOrigin = Shader.PropertyToID("_WaveOrigin");
	/// <summary> Used for tracking cut wave distance </summary>
	public static readonly int _WaveDistance = Shader.PropertyToID("_WaveDistance");
	/// <summary> Used for depth buffer world pos </summary>
	public static readonly int _ViewProjectionInverse = Shader.PropertyToID("_ViewProjectionInverse");
}
