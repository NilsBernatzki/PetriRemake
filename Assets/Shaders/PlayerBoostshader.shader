// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PlayerBoostshader"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		_Vector0("Vector 0", Vector) = (0.5,0.5,0,0)
		_MainTex("MainTex", 2D) = "white" {}
		_BoostLevel("BoostLevel", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float2 _Vector0;
		uniform float _BoostLevel;
		uniform float4 _MainTex_ST;
		uniform float _MaskClipValue = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (0.1).xx;
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + temp_cast_0;
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_28_0 = ( _Time.y * 0.2 );
			float cos4 = cos( temp_output_28_0 );
			float sin4 = sin( temp_output_28_0 );
			float2 rotator4 = mul(i.texcoord_0 - _Vector0, float2x2(cos4,-sin4,sin4,cos4)) + _Vector0;
			float4 tex2DNode14 = tex2D( _MainTex, rotator4 );
			float cos3 = cos( ( 1.0 - temp_output_28_0 ) );
			float sin3 = sin( ( 1.0 - temp_output_28_0 ) );
			float2 rotator3 = mul(i.texcoord_1 - _Vector0, float2x2(cos3,-sin3,sin3,cos3)) + _Vector0;
			float4 tex2DNode13 = tex2D( _MainTex, rotator3 );
			float temp_output_18_0 = ( ( tex2DNode14.b + tex2DNode13.b ) * 1.0 * ( 0.2 + _BoostLevel ) );
			float3 temp_cast_0 = (temp_output_18_0).xxx;
			o.Albedo = temp_cast_0;
			float4 lerpResult50 = lerp( float4(0.9485294,0.5337161,0.4114944,0) , float4(0.4620999,0.8161765,0.6403592,0) , _BoostLevel);
			float3 _Vector1 = float3(0,1,0);
			o.Emission = ( lerpResult50 * ( ( temp_output_18_0 + (_Vector1.z + (( 1.0 - ( tex2DNode13.r * tex2DNode14.r ) ) - _Vector1.x) * (( _BoostLevel * 2.0 ) - _Vector1.z) / (_Vector1.y - _Vector1.x)) + 1.0 ) * 0.3 ) ).rgb;
			o.Alpha = 1;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			clip( tex2D( _MainTex, uv_MainTex ).g - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
-2366;35;2359;1364;2011.66;217.207;1;True;True
Node;AmplifyShaderEditor.TimeNode;5;-1450,354;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;29;-1436.015,507.5356;Float;False;Constant;_Float3;Float 3;4;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1172.015,422.5356;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;-1564,234;Float;False;Constant;_Float0;Float 0;3;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;7;-944,530;Float;False;Property;_Vector0;Vector 0;1;0;0.5,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1412,46;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;6;-1089,305;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1398,178;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RotatorNode;3;-768,189;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RotatorNode;4;-770,327;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;13;-549,166;Float;True;Property;_TextureSample2;Texture Sample 2;1;0;Assets/Textures/Player/Petri_Actors_BoostEffect.png;True;0;False;white;Auto;False;Instance;12;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;14;-555,370;Float;True;Property;_TextureSample3;Texture Sample 3;2;0;Assets/Textures/Player/Petri_Actors_BoostEffect.png;True;0;False;white;Auto;False;Instance;12;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-1607.723,648.5835;Float;False;Property;_BoostLevel;BoostLevel;3;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;53;-665.6599,938.793;Float;False;Constant;_Float6;Float 6;4;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-112,231;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;26;151.1418,754.4664;Float;False;Constant;_Float2;Float 2;5;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-441.5729,809.7709;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;22;318,281;Float;False;Constant;_Vector1;Vector 1;4;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;15;125,201;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;393.7433,667.8373;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-117,456;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;19;109.9069,595.0719;Float;False;Constant;_Float1;Float 1;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;20;532,300;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;313,472;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;613.9854,740.5356;Float;False;Constant;_Float4;Float 4;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;915.4271,752.7709;Float;False;Constant;_Float5;Float 5;4;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;40;876.8741,-79.89761;Float;False;Constant;_Color1;Color 1;4;0;0.4620999,0.8161765,0.6403592,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;38;859.6742,135.1025;Float;False;Constant;_Color0;Color 0;4;0;0.9485294,0.5337161,0.4114944,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;30;886.385,492.536;Float;True;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;1237.187,513.8356;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;50;1381.427,16.77087;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;12;-605,-110;Float;True;Property;_MainTex;MainTex;2;0;Assets/Textures/Player/Petri_Actors_BoostEffect.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;1658.175,483.0027;Float;True;2;2;0;COLOR;0.0;False;1;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2060.901,-79.40001;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PlayerBoostshader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;True;Transparent;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;5;2
WireConnection;28;1;29;0
WireConnection;6;0;28;0
WireConnection;16;1;17;0
WireConnection;3;0;2;0
WireConnection;3;1;7;0
WireConnection;3;2;6;0
WireConnection;4;0;16;0
WireConnection;4;1;7;0
WireConnection;4;2;28;0
WireConnection;13;1;3;0
WireConnection;14;1;4;0
WireConnection;11;0;13;1
WireConnection;11;1;14;1
WireConnection;52;0;53;0
WireConnection;52;1;23;0
WireConnection;15;0;11;0
WireConnection;27;0;23;0
WireConnection;27;1;26;0
WireConnection;10;0;14;3
WireConnection;10;1;13;3
WireConnection;20;0;15;0
WireConnection;20;1;22;1
WireConnection;20;2;22;2
WireConnection;20;3;22;3
WireConnection;20;4;27;0
WireConnection;18;0;10;0
WireConnection;18;1;19;0
WireConnection;18;2;52;0
WireConnection;30;0;18;0
WireConnection;30;1;20;0
WireConnection;30;2;35;0
WireConnection;34;0;30;0
WireConnection;34;1;51;0
WireConnection;50;0;38;0
WireConnection;50;1;40;0
WireConnection;50;2;23;0
WireConnection;37;0;50;0
WireConnection;37;1;34;0
WireConnection;0;0;18;0
WireConnection;0;2;37;0
WireConnection;0;10;12;2
ASEEND*/
//CHKSM=8A1B78646D64CC82A46F9AEBBAA2966C11A71CB4