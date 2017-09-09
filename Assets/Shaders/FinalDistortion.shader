// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FinalDistortion"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_NormalNoise("NormalNoise", 2D) = "bump" {}
		_scale("scale", Range( 0 , 0.05)) = 0
		_Float1("Float 1", Range( 0 , 1)) = 0
		_Speed("Speed", Range( 0 , 1)) = 0
		_PerlinNoise("PerlinNoise", 2D) = "white" {}
		_Color0("Color 0", Color) = (0.3233131,0.4779412,0.4521698,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "_GrabTexture" }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 texcoord_0;
			float4 screenPos;
			float2 texcoord_1;
		};

		uniform float4 _Color0;
		uniform sampler2D _PerlinNoise;
		uniform float _Float1;
		uniform sampler2D _GrabTexture;
		uniform float _scale;
		uniform sampler2D _NormalNoise;
		uniform float _Speed;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 5,5 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_33_0 = ( _Time.x * _Float1 );
			o.Albedo = ( _Color0 + ( tex2D( _PerlinNoise, (abs( i.texcoord_0+temp_output_33_0 * float2(1,1 ))) ) * tex2D( _PerlinNoise, (abs( ( 1.0 - i.texcoord_0 )+temp_output_33_0 * float2(1,1 ))) ) * 1.0 * _Color0 ) ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos3 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale3 = -1.0;
			#else
			float scale3 = 1.0;
			#endif
			float halfPosW3 = ase_screenPos3.w * 0.5;
			ase_screenPos3.y = ( ase_screenPos3.y - halfPosW3 ) * _ProjectionParams.x* scale3 + halfPosW3;
			#ifdef UNITY_SINGLE_PASS_STEREO
			ase_screenPos3.xy = TransformStereoScreenSpaceTex(ase_screenPos3.xy, ase_screenPos3.w);
			#endif
			ase_screenPos3.xyzw /= ase_screenPos3.w;
			float temp_output_23_0 = ( _Time.x * _Speed );
			float4 screenColor2 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ( ase_screenPos3 + float4( UnpackScaleNormal( tex2D( _NormalNoise, (abs( i.texcoord_1+temp_output_23_0 * float2(1,1 ))) ) ,_scale ) , 0.0 ) + float4( UnpackScaleNormal( tex2D( _NormalNoise, (abs( ( 1.0 - i.texcoord_1 )+temp_output_23_0 * float2(1,1 ))) ) ,_scale ) , 0.0 ) ) ) );
			o.Emission = screenColor2.rgb;
			o.Alpha = 0.3;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
-2366;35;2359;1364;1643.774;805.4254;1;True;True
Node;AmplifyShaderEditor.Vector2Node;28;-2172.824,90.2005;Float;False;Constant;_Vector0;Vector 0;3;0;5,5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-1663.824,-591.7995;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;31;-1703.024,-353.9995;Float;False;Property;_Float1;Float 1;2;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;11;-1816.5,369;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-1710.824,648.2005;Float;False;Property;_Speed;Speed;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1857.5,143;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;32;-1338.024,-569.9995;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1264.024,-419.9995;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;25;-1590.824,256.2005;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1516.824,406.2005;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;22;-1369.824,197.2005;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;26;-1314.824,329.2005;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;34;-1097.024,-479.9995;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;6;-1360.5,-80;Float;False;Property;_scale;scale;1;0;0;0;0.05;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;35;-1117.024,-628.9995;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;40;-439.824,-191.7996;Float;False;Constant;_Float0;Float 0;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;36;-772.824,-362.7996;Float;True;Property;_TextureSample2;Texture Sample 2;4;0;Assets/Textures/PerlinNoise.png;True;0;False;white;Auto;False;Instance;29;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;27;-831.824,136.2005;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;Assets/Textures/NormalNoise.png;True;0;True;bump;Auto;True;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GrabScreenPosition;3;-768.5,438;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;29;-754.824,-547.7995;Float;True;Property;_PerlinNoise;PerlinNoise;4;0;Assets/Textures/PerlinNoise.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-811.5,-105;Float;True;Property;_NormalNoise;NormalNoise;0;0;Assets/Textures/NormalNoise.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;41;-222.824,-565.7996;Float;False;Property;_Color0;Color 0;5;0;0.3233131,0.4779412,0.4521698,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-408.824,-427.7996;Float;True;4;4;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0;False;2;FLOAT;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-332.5,221;Float;False;3;3;0;FLOAT4;0,0,0;False;1;FLOAT3;0.0,0,0,0;False;2;FLOAT3;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;42;87.17603,-319.7996;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ScreenColorNode;2;-77.5,193;Float;False;Global;_GrabScreen0;Grab Screen 0;1;0;Object;-1;False;1;0;FLOAT4;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-60.82397,-51.79956;Float;False;Constant;_Float2;Float 2;5;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;271,-175;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;FinalDistortion;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;28;0
WireConnection;32;0;30;0
WireConnection;33;0;11;1
WireConnection;33;1;31;0
WireConnection;25;0;8;0
WireConnection;23;0;11;1
WireConnection;23;1;24;0
WireConnection;22;0;8;0
WireConnection;22;1;23;0
WireConnection;26;0;25;0
WireConnection;26;1;23;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;35;0;30;0
WireConnection;35;1;33;0
WireConnection;36;1;34;0
WireConnection;27;1;26;0
WireConnection;27;5;6;0
WireConnection;29;1;35;0
WireConnection;1;1;22;0
WireConnection;1;5;6;0
WireConnection;37;0;29;0
WireConnection;37;1;36;0
WireConnection;37;2;40;0
WireConnection;37;3;41;0
WireConnection;16;0;3;0
WireConnection;16;1;1;0
WireConnection;16;2;27;0
WireConnection;42;0;41;0
WireConnection;42;1;37;0
WireConnection;2;0;16;0
WireConnection;0;0;42;0
WireConnection;0;2;2;0
WireConnection;0;9;43;0
ASEEND*/
//CHKSM=9CE9B9E86B4F41DD1BC8DB3AE01EAF214E6018D0