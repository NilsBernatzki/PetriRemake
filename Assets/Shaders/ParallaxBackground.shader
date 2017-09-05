// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ParallaxBackground"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MainTex("_MainTex", 2D) = "white" {}
		_OpacityDivisions("OpacityDivisions", Float) = 3
		_NormalNoise("NormalNoise", 2D) = "bump" {}
		_Front("Front", Range( 0 , 0.1)) = 0.08
		_Middle("Middle", Range( 0 , 0.1)) = 0.04
		_Back("Back", Range( 0 , 0.1)) = 0.01
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 viewDir;
			INTERNAL_DATA
			float2 texcoord_0;
		};

		uniform sampler2D _MainTex;
		uniform float _Back;
		uniform sampler2D _NormalNoise;
		uniform float _Middle;
		uniform float _Front;
		uniform float _OpacityDivisions;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 2,2 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 temp_output_95_0 = ( UnpackScaleNormal( tex2D( _NormalNoise, (abs( i.texcoord_0+_Time.x * float2(1,1 ))) ) ,0.005 ) + UnpackScaleNormal( tex2D( _NormalNoise, (abs( ( 1.0 - i.texcoord_0 )+_Time.x * float2(1,1 ))) ) ,0.005 ) );
			float4 tex2DNode1 = tex2D( _MainTex, ( ( ( ( ( _Back * _WorldSpaceCameraPos ) - i.viewDir ) + temp_output_95_0 ) * ( 1.0 - ( _Back * ( _Back * 100.0 ) ) ) ) + 1.98 ).xy );
			float4 tex2DNode40 = tex2D( _MainTex, ( ( ( ( ( _Middle * _WorldSpaceCameraPos ) - i.viewDir ) + temp_output_95_0 ) * ( 1.0 - ( _Middle * ( _Middle * 100.0 ) ) ) ) + 0.2 ).xy );
			float4 tex2DNode67 = tex2D( _MainTex, ( ( ( ( _Front * _WorldSpaceCameraPos ) - i.viewDir ) + temp_output_95_0 ) * ( 1.0 - ( _Front * ( _Front * 100.0 ) ) ) ).xy );
			o.Albedo = abs( ( ( tex2DNode1 + tex2DNode40 + tex2DNode67 ) / _OpacityDivisions ) ).xyz;
			o.Alpha = ( ( ( tex2DNode1.a * _Back * 10.0 ) + ( _Middle * tex2DNode40.a * 10.0 ) + ( _Front * tex2DNode67.a * 10.0 ) ) / 3.0 );
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
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
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
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
-2366;35;2359;1364;1694.116;171.1071;1;True;True
Node;AmplifyShaderEditor.Vector2Node;102;-4009.302,-135.4948;Float;False;Constant;_Vector0;Vector 0;6;0;2,2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;100;-3800.302,-135.4948;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;101;-3516.302,305.5052;Float;True;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TimeNode;99;-3794.302,134.5052;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;97;-3232.302,162.5052;Float;True;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;37;-3035.195,455.4995;Float;False;Constant;_Float2;Float 2;2;0;0.005;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;94;-3224.3,-66.19583;Float;True;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;36;-2810.097,170.4994;Float;True;Property;_NormalNoise;NormalNoise;1;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;96;-2801.604,-28.995;Float;True;Property;_TextureSample2;Texture Sample 2;2;0;None;True;0;True;bump;Auto;True;Instance;36;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;76;-1533.296,56.4021;Float;False;Constant;_Float7;Float 7;3;0;100;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-1874.8,-480.6;Float;False;Property;_Back;Back;5;0;0.01;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceCameraPos;21;-2462.8,-420.6;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;44;-2603,485.4991;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-2315.395,167.3043;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;81;-1575.7,841.0847;Float;False;Constant;_Float10;Float 10;3;0;100;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-2017.401,335.4989;Float;False;Property;_Middle;Middle;4;0;0.04;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;86;-1562.3,1654.384;Float;False;Constant;_Float12;Float 12;3;0;100;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1535.8,-327.6;Float;False;2;2;0;FLOAT;0.0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-1344.296,-72.5979;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;63;-1892.406,1108.222;Float;False;Property;_Front;Front;3;0;0.08;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;17;-1855.8,-153.6;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1589,443.4989;Float;False;2;2;0;FLOAT;0.0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;45;-1715,667.4994;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;60;-2102.799,863.6024;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;62;-2053.406,1299.222;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1378.7,705.7847;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1590.406,1218.221;Float;False;2;2;0;FLOAT;0.0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1355.3,1549.384;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;-1129.597,138.4022;Float;False;Constant;_Float0;Float 0;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;56;-1167.795,103.202;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;33;-1273.8,-253.6;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-1201.302,-132.8956;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;64;-1716.406,1442.222;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1231.302,680.1044;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;79;-1137.001,914.7847;Float;False;Constant;_Float9;Float 9;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-1327,517.499;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.WireNode;59;-1211.598,930.7995;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-1020.298,-253.9009;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;92;-955.3025,-57.89563;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;93;-2294.008,1278.302;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-1207.302,1517.104;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;91;-940.3025,722.1044;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-1051.397,504.5019;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;84;-1237.601,1761.384;Float;False;Constant;_Float11;Float 11;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;66;-1328.407,1292.221;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;90;-1055.302,1507.104;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-841.4956,-256.6981;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-1111.305,1290.925;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-848.8974,519.6844;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;121;-820.1162,809.8929;Float;False;Constant;_Offset2;Offset2;6;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;119;-873.2268,228.3947;Float;False;Constant;_Offset;Offset;6;0;1.98;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-939.0989,1316.883;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-695.2268,-202.6053;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;120;-700.1162,563.8929;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;61;-613.2944,-538.8972;Float;False;Constant;_Float5;Float 5;2;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;-569.3997,339.5993;Float;False;Constant;_Float5;Float 5;2;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;67;-781.2033,1263.922;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;70;-726.7032,1198.122;Float;False;Constant;_Float8;Float 8;2;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;40;-580.899,485.1989;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-578.4004,-297.2001;Float;True;Property;_MainTex;_MainTex;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-6.098079,-381.5002;Float;False;3;3;0;FLOAT4;0.0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-296.5984,-498.101;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-227.0028,1174.621;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;48;-1.998099,-202.501;Float;False;Property;_OpacityDivisions;OpacityDivisions;2;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-225.5981,399.899;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;50;196.7019,233.0991;Float;False;Constant;_Float5;Float 5;2;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-39.2981,112.0991;Float;False;3;3;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;47;188.202,-373.0005;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.AbsOpNode;113;449.7732,-353.6053;Float;False;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleDivideOpNode;49;339.7019,108.0991;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1005.201,-416.8992;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ParallaxBackground;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;100;0;102;0
WireConnection;101;0;100;0
WireConnection;97;0;101;0
WireConnection;97;1;99;1
WireConnection;94;0;100;0
WireConnection;94;1;99;1
WireConnection;36;1;97;0
WireConnection;36;5;37;0
WireConnection;96;1;94;0
WireConnection;96;5;37;0
WireConnection;95;0;96;0
WireConnection;95;1;36;0
WireConnection;31;0;32;0
WireConnection;31;1;21;0
WireConnection;75;0;32;0
WireConnection;75;1;76;0
WireConnection;41;0;43;0
WireConnection;41;1;44;0
WireConnection;60;0;95;0
WireConnection;80;0;43;0
WireConnection;80;1;81;0
WireConnection;65;0;63;0
WireConnection;65;1;62;0
WireConnection;85;0;63;0
WireConnection;85;1;86;0
WireConnection;56;0;95;0
WireConnection;33;0;31;0
WireConnection;33;1;17;0
WireConnection;87;0;32;0
WireConnection;87;1;75;0
WireConnection;88;0;43;0
WireConnection;88;1;80;0
WireConnection;42;0;41;0
WireConnection;42;1;45;0
WireConnection;59;0;60;0
WireConnection;39;0;33;0
WireConnection;39;1;56;0
WireConnection;92;0;74;0
WireConnection;92;1;87;0
WireConnection;93;0;95;0
WireConnection;89;0;63;0
WireConnection;89;1;85;0
WireConnection;91;0;79;0
WireConnection;91;1;88;0
WireConnection;55;0;42;0
WireConnection;55;1;59;0
WireConnection;66;0;65;0
WireConnection;66;1;64;0
WireConnection;90;0;84;0
WireConnection;90;1;89;0
WireConnection;72;0;39;0
WireConnection;72;1;92;0
WireConnection;68;0;66;0
WireConnection;68;1;93;0
WireConnection;77;0;55;0
WireConnection;77;1;91;0
WireConnection;82;0;68;0
WireConnection;82;1;90;0
WireConnection;117;0;72;0
WireConnection;117;1;119;0
WireConnection;120;0;77;0
WireConnection;120;1;121;0
WireConnection;67;1;82;0
WireConnection;40;1;120;0
WireConnection;1;1;117;0
WireConnection;46;0;1;0
WireConnection;46;1;40;0
WireConnection;46;2;67;0
WireConnection;54;0;1;4
WireConnection;54;1;32;0
WireConnection;54;2;61;0
WireConnection;71;0;63;0
WireConnection;71;1;67;4
WireConnection;71;2;70;0
WireConnection;53;0;43;0
WireConnection;53;1;40;4
WireConnection;53;2;52;0
WireConnection;51;0;54;0
WireConnection;51;1;53;0
WireConnection;51;2;71;0
WireConnection;47;0;46;0
WireConnection;47;1;48;0
WireConnection;113;0;47;0
WireConnection;49;0;51;0
WireConnection;49;1;50;0
WireConnection;0;0;113;0
WireConnection;0;9;49;0
ASEEND*/
//CHKSM=269A33B130A74D086604FD5E3C3196C0A479B7B2