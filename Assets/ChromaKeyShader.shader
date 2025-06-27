Shader "Unlit/ChromaKey"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ChromaColor("Chroma Key Color", Color) = (0.278, 0.878, 0, 1) // #47E000
        _Threshold("Threshold", Range(0, 1)) = 0.3
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100

            Pass
            {
                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _ChromaColor;
                float _Threshold;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float4 col = tex2D(_MainTex, i.uv);

                    float diff = distance(col.rgb, _ChromaColor.rgb);
                    if (diff < _Threshold)
                    {
                        col.a = 0; // Make transparent
                    }

                    return col;
                }
                ENDCG
            }
        }
}