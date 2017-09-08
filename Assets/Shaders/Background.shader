// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Background"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_NormalIntens("Normal Intens", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		_MainTex("_MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalIntens;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,_NormalIntens );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = tex2D( _MainTex, uv_MainTex ).xyz;
			o.Metallic = float4(0,0,0,0).r;
			o.Smoothness = float4(0,0,0,0).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
-2366;35;2359;1364;1762.5;389;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;9;-1412.5,465;Float;False;Property;_NormalIntens;Normal Intens;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-1154.5,-126;Float;True;Property;_MainTex;_MainTex;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;8;-1151.5,277;Float;True;Property;_Normal;Normal;1;0;Assets/Textures/Background/BackgroundRimDistortion.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-1087.5,89;Float;False;Constant;_Color1;Color 1;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-1081.5,510;Float;False;Constant;_Color0;Color 0;1;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;20,-63;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Background;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;5;9;0
WireConnection;0;0;1;0
WireConnection;0;1;8;0
WireConnection;0;3;2;0
WireConnection;0;4;3;0
ASEEND*/
//CHKSM=ED96D48EB6DEC6FBD3C71D7FE171EFEB8F4FE346