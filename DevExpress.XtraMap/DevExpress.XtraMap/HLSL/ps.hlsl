float4 NewColor;   //argb
float4 Parameters; // x - use a texture (0 \ 1)   y - opacity 

texture tex0;
sampler2D s_2D;

float4 main(float2 tex : TEXCOORD0) : COLOR0
{
	if (Parameters.x > 0.0f){
		float4 result = tex2D(s_2D, tex);
		result.a *= Parameters.y;
		return result;
	}
	return NewColor;
}