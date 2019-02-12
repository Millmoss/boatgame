Shader "Custom/Water"
{
	//https://lindenreid.wordpress.com/2017/12/15/simple-water-shader-in-unity/
	//For shadowS:
	//https://alastaira.wordpress.com/2014/12/30/adding-shadows-to-a-unity-vertexfragment-shader-in-7-easy-steps/
	Properties
	{
		// color of the water
		_Color("Color", Color) = (1, 1, 1, 1)
		// color of the edge effect
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		// width of the edge effect
		_DepthFactor("Depth Factor", float) = 1.0

		_DepthRampTex("Depth Ramp", 2D) = "white" {}

		_WaveSpeed("Wave Speed", float) = 1.0
		_ExtraHeight("Wave Height", float) = 0.25
		_WaveAmp("Wave Amp", float) = 1.0
		_NoiseTex("Noise Texture", 2D) = "white"{}
	}
	SubShader
	{
		//Initial pass; generate the water.
		Pass
		{
			CGPROGRAM
			// required to use ComputeScreenPos()
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			// Unity built-in - NOT required in Properties
			sampler2D _CameraDepthTexture;

			sampler2D _DepthRampTex;

			float4 _MinColor;
			float4 _Color;
			float4 _EdgeColor;
			float _DepthFactor;
			float _WaveSpeed;
			float _WaveAmp;
			float _ExtraHeight;
			sampler2D _NoiseTex;
			uniform float4 _LightColor0;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texCoord : TEXCOORD1;
			};

			struct v2g
			{
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float3 normal : NORMAL;
			};

			v2g vert(vertexInput input)
			{
				v2g output;

				// convert obj-space position to camera clip space
				output.pos = (input.vertex);

				// compute depth (screenPos is a float4)
				output.screenPos = ComputeScreenPos(output.pos);

				// apply wave animation
				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp + _ExtraHeight;
				//output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;

				return output;
			}

			/*
			//Makes 9 verticies from 3, supposidely. Woah.
			[maxvertexcount(9)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> tristream) {
				// here goes the real logic.

				float4 p[9];
				p[0] = input[0].pos;
				p[1] = float4(
					(input[0].pos.x + input[1].pos.x) / 2,
					(input[0].pos.y + input[1].pos.y) / 2,
					(input[0].pos.z + input[1].pos.z) / 2,
					(input[0].pos.w + input[1].pos.w) / 2
					);
				p[2] = float4(
					(input[0].pos.x + input[2].pos.x) / 2,
					(input[0].pos.y + input[2].pos.y) / 2,
					(input[0].pos.z + input[2].pos.z) / 2,
					(input[0].pos.w + input[2].pos.w) / 2
					);
				p[3] = float4(
					(input[1].pos.x + input[2].pos.x) / 2,
					(input[1].pos.y + input[2].pos.y) / 2,
					(input[1].pos.z + input[2].pos.z) / 2,
					(input[1].pos.w + input[2].pos.w) / 2
					);
				p[4] = input[2].pos;
				p[5] = p[3];
				p[6] = p[4];
				p[7] = p[1];
				p[8] = input[1].pos;

				//top -> down -> right ->left 
				float3 normals[4];

				normals[0] = normalize(cross(p[1] - p[0], p[2] - p[0]));
				normals[1] = normalize(cross(p[3] - p[1], p[2] - p[1]));
				normals[2] = normalize(cross(p[3] - p[2], p[4] - p[2]));
				normals[3] = normalize(cross(p[8] - p[7], p[6] - p[7]));

				g2f o[9];

				[unroll]
				for (int i = 0; i < 9; i++) {
					
					int vert_pos = 0;
					switch (i)
					{
						case 3:
							vert_pos = 1;
							break;
						case 4:
							vert_pos = 2;
							break;
						case 6:
							vert_pos = 1;
							break;
						case 8:
							vert_pos = 3;
							break;
					}

					
					o[i].normal = normals[vert_pos];

					// convert obj-space position to camera clip space
					o[i].pos = UnityObjectToClipPos(p[i]);
					o[i].screenPos = p[i];
					tristream.Append(o[i]);
				}

				tristream.RestartStrip();
			}*/


			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> tristream) {
				// here goes the real logic.

				float4 p0 = input[0].pos;
				float4 p1 = input[1].pos;
				float4 p2 = input[2].pos;

				float3 p0_norm = float3(p0.x,p0.y,p0.z);
				float3 p1_norm = float3(p1.x,p1.y,p1.z);
				float3 p2_norm = float3(p2.x,p2.y,p2.z);

				float3 normal = normalize(cross(p1_norm - p0_norm, p2_norm - p0_norm));

				g2f o[3];

				[unroll]
				for (int i = 0; i < 3; i++) {
					o[i].normal = normal;
					// convert obj-space position to camera clip space
					o[i].pos = UnityObjectToClipPos(input[i].pos);
					o[i].screenPos = input[i].screenPos;
					tristream.Append(o[i]);
				}

				tristream.RestartStrip();
			}

			float4 frag(g2f input) : COLOR
			{
				// sample camera depth texture
				float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
				float depth = LinearEyeDepth(depthSample).r;
				float4 light_color = _LightColor0;
				float4 lighting = 
					clamp((max(0, dot(input.normal, _WorldSpaceLightPos0.xyz))) * _LightColor0 / 2 + 0.45,
						0,
						1);

				//wait im using_LightColor0 a lot; redundent?
				
				// apply the DepthFactor to be able to tune at what depth values
				// the foam line actually starts
				//NOTE: saturate = clamp(0,1).
				float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));

				// sample the ramp texture
				float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.5)).rgb, 1.0);

				//Lerp between minimum and max color based on the lighting.
				// multiply the edge color by the foam factor to get the edge,
				// then add that to the color of the water
				// also multiply by the light amount
				float4 col = _Color;
				col.x = (col.x  + light_color.x) * lighting.x;
				col.y = (col.y + light_color.y) * lighting.y;
				col.z = (col.z + light_color.z) * lighting.z;

				col.x /= 2;
				col.y /= 2;
				col.z /= 2;


				return col * foamRamp;
				
			}

			ENDCG
		} 
		

	}
}