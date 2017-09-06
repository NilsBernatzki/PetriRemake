// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Obstacle"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Petri_Obstacle_basecolor("Petri_Obstacle_basecolor", 2D) = "white" {}
		_Petri_Obstacle_Rough_Metal_FlowMask("Petri_Obstacle_Rough_Metal_FlowMask", 2D) = "white" {}
		_Petri_Obstacle_RimNormal("Petri_Obstacle_RimNormal", 2D) = "bump" {}
		_Petri_Obstacle_normal("Petri_Obstacle_normal", 2D) = "bump" {}
		_Petri_Obstacle_Emitt("Petri_Obstacle_Emitt", 2D) = "white" {}
		_NormalNoise("NormalNoise", 2D) = "bump" {}
		_RimContrastTune("RimContrastTune", Range( 0 , 2)) = 1.9
		_RimNormalLvl("RimNormalLvl", Range( 0 , 1)) = 0.01
		_InnerNoiseScale("InnerNoiseScale", Range( 0 , 1)) = 0
		_InnerNoiseSpeed("InnerNoiseSpeed", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "GrabScreen0" }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
			float4 screenPos;
			float2 texcoord_1;
		};

		uniform sampler2D _Petri_Obstacle_normal;
		uniform float4 _Petri_Obstacle_normal_ST;
		uniform sampler2D GrabScreen0;
		uniform float _RimNormalLvl;
		uniform sampler2D _Petri_Obstacle_RimNormal;
		uniform float4 _Petri_Obstacle_RimNormal_ST;
		uniform sampler2D _NormalNoise;
		uniform sampler2D _Petri_Obstacle_Rough_Metal_FlowMask;
		uniform float4 _Petri_Obstacle_Rough_Metal_FlowMask_ST;
		uniform float _InnerNoiseScale;
		uniform float _InnerNoiseSpeed;
		uniform sampler2D _Petri_Obstacle_basecolor;
		uniform float4 _Petri_Obstacle_basecolor_ST;
		uniform sampler2D _Petri_Obstacle_Emitt;
		uniform float4 _Petri_Obstacle_Emitt_ST;
		uniform float _RimContrastTune;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Petri_Obstacle_normal = i.uv_texcoord * _Petri_Obstacle_normal_ST.xy + _Petri_Obstacle_normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Petri_Obstacle_normal, uv_Petri_Obstacle_normal ) );
			float2 uv_Petri_Obstacle_RimNormal = i.uv_texcoord * _Petri_Obstacle_RimNormal_ST.xy + _Petri_Obstacle_RimNormal_ST.zw;
			float3 temp_output_31_0 = ( UnpackScaleNormal( tex2D( _NormalNoise, (abs( i.texcoord_0+_Time.x * float2(1,1 ))) ) ,0.025 ) + UnpackScaleNormal( tex2D( _NormalNoise, (abs( ( 1.0 - i.texcoord_0 )+_Time.x * float2(1,1 ))) ) ,0.025 ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos17 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale17 = -1.0;
			#else
			float scale17 = 1.0;
			#endif
			float halfPosW17 = ase_screenPos17.w * 0.5;
			ase_screenPos17.y = ( ase_screenPos17.y - halfPosW17 ) * _ProjectionParams.x* scale17 + halfPosW17;
			#ifdef UNITY_SINGLE_PASS_STEREO
			ase_screenPos17.xy = TransformStereoScreenSpaceTex(ase_screenPos17.xy, ase_screenPos17.w);
			#endif
			ase_screenPos17.xyzw /= ase_screenPos17.w;
			float2 uv_Petri_Obstacle_Rough_Metal_FlowMask = i.uv_texcoord * _Petri_Obstacle_Rough_Metal_FlowMask_ST.xy + _Petri_Obstacle_Rough_Metal_FlowMask_ST.zw;
			float4 tex2DNode2 = tex2D( _Petri_Obstacle_Rough_Metal_FlowMask, uv_Petri_Obstacle_Rough_Metal_FlowMask );
			float temp_output_43_0 = ( tex2DNode2.g * _InnerNoiseScale );
			float temp_output_45_0 = ( _Time.x * _InnerNoiseSpeed );
			float4 screenColor5 = tex2Dproj( GrabScreen0, UNITY_PROJ_COORD( ( float4( ( UnpackScaleNormal( tex2D( _Petri_Obstacle_RimNormal, uv_Petri_Obstacle_RimNormal ) ,_RimNormalLvl ) + temp_output_31_0 ) , 0.0 ) + ase_screenPos17 + float4( ( UnpackScaleNormal( tex2D( _NormalNoise, (abs( i.texcoord_1+temp_output_45_0 * float2(1,1 ))) ) ,temp_output_43_0 ) + UnpackScaleNormal( tex2D( _NormalNoise, (abs( ( 1.0 - i.texcoord_1 )+temp_output_45_0 * float2(1,1 ))) ) ,temp_output_43_0 ) ) , 0.0 ) ) ) );
			float2 uv_Petri_Obstacle_basecolor = i.uv_texcoord * _Petri_Obstacle_basecolor_ST.xy + _Petri_Obstacle_basecolor_ST.zw;
			float4 lerpResult6 = lerp( screenColor5 , tex2D( _Petri_Obstacle_basecolor, uv_Petri_Obstacle_basecolor ) , tex2DNode2.a);
			o.Albedo = lerpResult6.xyz;
			float4 _Color0 = float4(0,0,0,0);
			float2 uv_Petri_Obstacle_Emitt = i.uv_texcoord * _Petri_Obstacle_Emitt_ST.xy + _Petri_Obstacle_Emitt_ST.zw;
			float3 componentMask52 = temp_output_31_0.xyz;
			float4 lerpResult51 = lerp( _Color0 , tex2D( _Petri_Obstacle_Emitt, uv_Petri_Obstacle_Emitt ) , Luminance(componentMask52));
			float4 lerpResult8 = lerp( screenColor5 , _Color0 , tex2DNode2.a);
			o.Emission = ( lerpResult51 + ( lerpResult8 / _RimContrastTune ) ).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
-2366;35;2359;1364;3863.593;101.8342;1.570756;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-2933.999,1167.3;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;32;-2796.999,1631.3;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TimeNode;27;-2906.999,1355.3;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-2599.298,142.7008;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TimeNode;37;-2986.298,340.7008;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;46;-2942.298,652.3008;Float;False;Property;_InnerNoiseSpeed;InnerNoiseSpeed;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;29;-2330.999,1335.3;Float;False;Constant;_Float1;Float 1;6;0;0.025;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-2639.3,-299.1999;Float;True;Property;_Petri_Obstacle_Rough_Metal_FlowMask;Petri_Obstacle_Rough_Metal_FlowMask;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;30;-2594.999,1549.3;Float;True;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;44;-2596.298,6.300781;Float;False;Property;_InnerNoiseScale;InnerNoiseScale;8;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-2730.298,475.3008;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;38;-2456.298,644.7008;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;28;-2601.999,1271.3;Float;True;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;33;-2104.999,1362.3;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;None;True;0;True;bump;Auto;True;Instance;22;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-2456.7,947.5001;Float;False;Property;_RimNormalLvl;RimNormalLvl;7;0;0.01;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;40;-2261.298,284.7008;Float;True;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;39;-2254.298,562.7008;Float;True;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;22;-2123.999,1146.3;Float;True;Property;_NormalNoise;NormalNoise;5;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-2084.298,145.3008;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;42;-1918.298,502.3008;Float;True;Property;_TextureSample2;Texture Sample 2;6;0;None;True;0;True;bump;Auto;True;Instance;22;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;20;-2111.999,908.3004;Float;True;Property;_Petri_Obstacle_RimNormal;Petri_Obstacle_RimNormal;2;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;35;-1908.302,266.8006;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;None;True;0;True;bump;Auto;True;Instance;22;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1773.999,1491.3;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GrabScreenPosition;17;-1841.7,727.5001;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-1649.999,1013.3;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-1530.298,459.3008;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1441.7,731.5001;Float;False;3;3;0;FLOAT3;0.0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT3;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ScreenColorNode;5;-1258.7,616.5001;Float;False;Global;GrabScreen0;Grab Screen 0;3;0;Object;-1;True;1;0;FLOAT4;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;52;-1127.699,1529.5;Float;True;True;True;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;9;-795.7,724.5001;Float;False;Constant;_Color0;Color 0;2;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;47;-810.5996,1533.801;Float;True;Property;_Petri_Obstacle_Emitt;Petri_Obstacle_Emitt;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCGrayscale;49;-842.8,1362.501;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;8;-440.7,541.5001;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;11;-416.7,784.5001;Float;False;Property;_RimContrastTune;RimContrastTune;6;0;1.9;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-1725.302,-325.1999;Float;True;Property;_Petri_Obstacle_basecolor;Petri_Obstacle_basecolor;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;10;-136.7,727.5001;Float;False;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;51;-501.1992,1124.3;Float;False;3;0;COLOR;0.0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;6;-662.7002,249.5001;Float;False;3;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;48;62.19995,895.5006;Float;False;2;2;0;FLOAT4;0.0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;12;-500.7,-17.49988;Float;True;Property;_Petri_Obstacle_normal;Petri_Obstacle_normal;3;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;562.3997,387.1002;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Obstacle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.68;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;25;0
WireConnection;30;0;32;0
WireConnection;30;1;27;1
WireConnection;45;0;37;1
WireConnection;45;1;46;0
WireConnection;38;0;36;0
WireConnection;28;0;25;0
WireConnection;28;1;27;1
WireConnection;33;1;30;0
WireConnection;33;5;29;0
WireConnection;40;0;36;0
WireConnection;40;1;45;0
WireConnection;39;0;38;0
WireConnection;39;1;45;0
WireConnection;22;1;28;0
WireConnection;22;5;29;0
WireConnection;43;0;2;2
WireConnection;43;1;44;0
WireConnection;42;1;39;0
WireConnection;42;5;43;0
WireConnection;20;5;16;0
WireConnection;35;1;40;0
WireConnection;35;5;43;0
WireConnection;31;0;22;0
WireConnection;31;1;33;0
WireConnection;23;0;20;0
WireConnection;23;1;31;0
WireConnection;41;0;35;0
WireConnection;41;1;42;0
WireConnection;14;0;23;0
WireConnection;14;1;17;0
WireConnection;14;2;41;0
WireConnection;5;0;14;0
WireConnection;52;0;31;0
WireConnection;49;0;52;0
WireConnection;8;0;5;0
WireConnection;8;1;9;0
WireConnection;8;2;2;4
WireConnection;10;0;8;0
WireConnection;10;1;11;0
WireConnection;51;0;9;0
WireConnection;51;1;47;0
WireConnection;51;2;49;0
WireConnection;6;0;5;0
WireConnection;6;1;1;0
WireConnection;6;2;2;4
WireConnection;48;0;51;0
WireConnection;48;1;10;0
WireConnection;0;0;6;0
WireConnection;0;1;12;0
WireConnection;0;2;48;0
ASEEND*/
//CHKSM=366E1C82383DA57DC466E5352EC16995FA6A28C5