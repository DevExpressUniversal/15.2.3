extern float4 mParam;
extern float4x4 mViewProjection;
extern float4x4 mWorld;

struct VS_OUTPUT{
	float4 Pos  : POSITION;
};

float2 ds_set(float a){
   return float2(a, .0f);
}

float2 ds_set(float a, float b){
   return float2(a, b);
}

float2 ds_add (float2 dsa, float2 dsb){
   float2 dsc;
   float t1, t2, e;
 
   t1 = dsa.x + dsb.x;
   e = t1 - dsa.x;
   t2 = ((dsb.x - e) + (dsa.x - (t1 - e))) + dsa.y + dsb.y;
   dsc.x = t1 + t2;
   dsc.y = t2 - (dsc.x - t1);
   return dsc;
}

float2 ds_mul (float2 dsa, float2 dsb){
   float2 dsc;
   float c11, c21, c2, e, t1, t2;
   float a1, a2, b1, b2, cona, conb;
   float split = 8193.0f;
 
   cona = dsa.x * split;
   conb = dsb.x * split;
   a1 = cona - (cona - dsa.x);
   b1 = conb - (conb - dsb.x);
   a2 = dsa.x - a1;
   b2 = dsb.x - b1;
   c11 = dsa.x * dsb.x;
   c21 = a2 * b2 + (a2 * b1 + (a1 * b2 + (a1 * b1 - c11)));
   c2 = dsa.x * dsb.y + dsa.y * dsb.x;
   t1 = c11 + c2;
   e = t1 - c11;
   t2 = dsa.y * dsb.y + ((c2 - e) + (c11 - (t1 - e))) + c21;
   dsc.x = t1 + t2;
   dsc.y = t2 - (dsc.x - t1);
   return dsc;
}

float4 CalculatePositionUseTransform(float4 pos){
	pos = mul(mWorld, pos);
	return pos;
}

float4 CalculatePositionUseDoublePrecision(float4 pos, float2 dpos){
    	
	float2 scaleX = float2(mWorld[0][0],mWorld[1][0]);
	float2 scaleY = float2(mWorld[2][0],mWorld[3][0]);
	float2 offsetX = float2(mWorld[0][1],mWorld[1][1]);
	float2 offsetY = float2(mWorld[2][1],mWorld[3][1]);

	float2 X = ds_add(ds_mul(ds_set(pos.x, dpos.x), scaleX), offsetX); 
	float2 Y = ds_add(ds_mul(ds_set(pos.y, dpos.y), scaleY), offsetY);
	pos.x = X.x;
	pos.y = Y.x;
	return pos;
}

VS_OUTPUT main(float4 vertex  : POSITION0, float2 normal : NORMAL0)
{
        VS_OUTPUT Out;

	float4 pos = float4(vertex.x, vertex.y, .0f, 1);
	float2 dpos = float2(vertex.z, vertex.w);
	
	if(mParam.x < 0.0f) pos = CalculatePositionUseTransform(pos);
	if(mParam.x > 0.0f) pos = CalculatePositionUseDoublePrecision(pos, dpos); 

	float3 Normal = float3(normal.x, normal.y, .0f);
	float3 factor = Normal * mParam.y;         
	float len = length(factor);

    pos.x += factor.x;
	pos.y += factor.y;    

	pos.x += (int)mParam.z;
	pos.y += -(int)mParam.w;

	pos = mul(pos, mViewProjection);

	Out.Pos = pos;	
 
	return Out;
}

