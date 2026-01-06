Shader "UI/Gradient_Split_RightFade_Rotate"
{
    Properties
    {
        _LeftColor  ("Left Color", Color) = (1,1,1,1)
        _RightColor ("Right Color", Color) = (0,0,0,1)

        _Split   ("Split Position", Range(0,1)) = 0.5
        _Feather ("Transition Width", Range(0,0.5)) = 0.05

        _RightFadeStart ("Right Fade Start", Range(0,1)) = 0.9
        _RightFadeWidth ("Right Fade Width", Range(0,0.5)) = 0.1

        _Angle ("Gradient Angle (Deg)", Range(0,360)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed4 _LeftColor;
            fixed4 _RightColor;
            float  _Split;
            float  _Feather;
            float  _RightFadeStart;
            float  _RightFadeWidth;
            float  _Angle;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                
                float rad = radians(_Angle);
                float2 dir = float2(cos(rad), sin(rad));
                
                float x = dot(uv, dir) + 0.5;
                
                float t = smoothstep(
                    _Split - _Feather,
                    _Split + _Feather,
                    x
                );

                fixed4 col = lerp(_LeftColor, _RightColor, t);
                
                float fade = 1.0;
                if (_RightFadeWidth > 0)
                {
                    fade = 1.0 - smoothstep(
                        _RightFadeStart,
                        _RightFadeStart + _RightFadeWidth,
                        x
                    );
                }

                col.a *= fade;
                
                col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}
