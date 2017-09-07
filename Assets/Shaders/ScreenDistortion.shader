// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ScreenDistortion"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Petri_Actors_RimNormal("Petri_Actors_RimNormal", 2D) = "bump" {}
		_Intensity("Intensity", Range( 0 , 1)) = 0
		_Contrast_Tune("Contrast_Tune", Range( 0 , 3)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "_GrabTexture" }
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _GrabTexture;
		uniform float _Intensity;
		uniform sampler2D _Petri_Actors_RimNormal;
		uniform float4 _Petri_Actors_RimNormal_ST;
		uniform float _Contrast_Tune;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_Petri_Actors_RimNormal = i.uv_texcoord * _Petri_Actors_RimNormal_ST.xy + _Petri_Actors_RimNormal_ST.zw;
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
			float4 screenColor2 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ( float4( UnpackScaleNormal( tex2D( _Petri_Actors_RimNormal, uv_Petri_Actors_RimNormal ) ,_Intensity ) , 0.0 ) + ase_screenPos3 ) ) );
			c.rgb = ( screenColor2 / _Contrast_Tune ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
-2366;35;2359;1364;1258.5;448;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;-959.5,-108;Float;False;Property;_Intensity;Intensity;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-597.5,-194;Float;True;Property;_Petri_Actors_RimNormal;Petri_Actors_RimNormal;0;0;Assets/Textures/Player/Petri_Actors_RimNormal.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GrabScreenPosition;3;-595.5,225;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-270.5,-59;Float;True;2;2;0;FLOAT3;0.0;False;1;FLOAT4;0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;7;-63.5,155;Float;False;Property;_Contrast_Tune;Contrast_Tune;2;0;1;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;2;-7.5,-61;Float;False;Global;_GrabScreen0;Grab Screen 0;1;0;Object;-1;False;1;0;FLOAT4;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;6;265.5,111;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;590,-69;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;ScreenDistortion;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;5;5;0
WireConnection;4;0;1;0
WireConnection;4;1;3;0
WireConnection;2;0;4;0
WireConnection;6;0;2;0
WireConnection;6;1;7;0
WireConnection;0;2;6;0
ASEEND*/
//CHKSM=9D1B52E13CCB5B7020AE45553DE474A2357DF7D2