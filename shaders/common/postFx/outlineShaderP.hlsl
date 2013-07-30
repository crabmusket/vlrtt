#include "postFx.hlsl"
#include "shadergen:/autogenConditioners.h"

float4 main( PFXVertToPix IN, 
             uniform sampler2D edgeBuffer :register(S0), uniform sampler2D backBuffer : register(S1) ) : COLOR0
{
   float4 e = float4( tex2D( edgeBuffer, IN.uv0 ).rrr, 1.0 );
   e.r = e.g = e.b = 1.0 - e.r;
   return tex2D(backBuffer, IN.uv0) * e;
}