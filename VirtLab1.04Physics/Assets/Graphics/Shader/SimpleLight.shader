// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Prosto/SimpleLight"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _ShadowColor("ShadowColor",Color) = (1,1,1,1)
        _SpecColor("SpecularColor", Color) = (1,1,1,1)
        _Shinness("Shinness", float) = 0.5
        _LightDir("LightDir", Vector) = (1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 col : COLOR;
            };

            fixed4 _Color;
            fixed4 _ShadowColor;
            fixed4 _SpecColor;
            float3 _LightDir;
            float _Shinness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);        
                _LightDir = -_WorldSpaceLightPos0.xyz;
                float3 normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                float3 viewDirection = normalize( float3(float4(_WorldSpaceCameraPos.xyz, 1.0) - UnityObjectToClipPos(v.vertex).xyz));

                fixed3 specularReflection = _SpecColor.rgb * max(0.0, dot(normalDir, -_LightDir))*pow(max(0,dot(reflect(-_LightDir, normalDir),viewDirection)), _Shinness);
                fixed3 diffuseReflection = lerp(_Color,_ShadowColor ,dot(normalDir, _LightDir) * 0.8 + 0.5);

                float3 lightFinal = diffuseReflection + specularReflection;
                //float3 lightFinal = dot(reflect(-_LightDir,normalDir),viewDirection);

                o.col = lightFinal*_Color.rgb;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                return fixed4(i.col.rgb,1);
            }
            ENDCG
        }
    }
}
