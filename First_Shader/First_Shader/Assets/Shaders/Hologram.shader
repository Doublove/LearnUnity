
// 所在目录，无须修改文件名也能改变其在unity软件中的所属目录、显示名
Shader "Unlit/SpecialFX/Cool Hologram"
{
	// ShaderLab
	// 面向unity，告知unity如何进行渲染
    Properties
    {
        _MainTex ("Albedo Texture", 2D) = "white" {}
		_TintColor("Tint Color", color) = (1,1,1,1)// 为unlit_hologram这个材质material添加一个颜色选项
		_Transparency("Transparency", Range(0.0,0.5)) = 0.25// 为材质添加的透明度选项，默认值为0.25，范围0-0.5
		_CutoutThresh( "Cutout Threshold", Range(0.0,1.0) ) = 0.2// 为材质添加的改变颜色功能
		// 以下4个变量用来在shader中直接渲染出glitch效果
		_Distance( "Distance", float ) = 1// glitch的距离
		_Amplitude( "Amplitude", float ) = 1// 振幅(?) 用于sin函数
		_Speed( "Speed", float ) = 1// 频率/速度
		_Amount( "Amount", Range(0.0,1.0)) = 1// 
    }

	// 面向unity，渲染如何进行的实际代码
    SubShader
    {
		// 渲染类型 opaque不透明 transparent透明
		// Queue渲染的顺序。要渲染透明物体表示该物体要渲染在其他物体上，即其他物体先渲染
        // Tags { "RenderType"="Opaque" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

		// 关闭写入深度缓冲。通常透明物品关
		ZWrite off
		// 如何进行混合。SrcAlpha-通过alpha通道混合
		Blend SrcAlpha OneMinusSrcAlpha

		// 直接面向GPU
        Pass
        {
			// C for graphics GPU中的实际着色器
            CGPROGRAM
            #pragma vertex vert// vertex function
            #pragma fragment frag// fragment function 片段处理
            // make fog work
            #pragma multi_compile_fog

			// unityCG头文件
            #include "UnityCG.cginc"

			// vertices data
            struct appdata
            {
                float4 vertex : POSITION;// vertex's cordinates
                float2 uv : TEXCOORD0;// texture cordinates
            };

			// vert to frag
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;// screen space position
            };

			// 
            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _TintColor;// 把property的颜色选项声明为CG里的变量
			float _Transparency;// 把alpha通道的透明度声明为CG里的变量
			float _CutoutThresh;// 
			float _Distance;// 
			float _Amplitude;// 
			float _Speed;
			float _Amount;

			// vertex function
            v2f vert (appdata v)
            {
                v2f o;
				// time.y秒  实现顶点的x坐标随着时间按参数抖动
				v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Amplitude) * _Distance * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);// 改变顶点的坐标，从vertex到fragment
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);// 转换texture，获取vertex的uv数据，获取propertie的maintex(ture)
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			// fragment function 片段处理，输入v2f对象数据，输出到SV_target，通常是帧缓冲
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				// 颜色RGBA或XYZW类型fixed4  由tex2D函数将maintex 纹理数据和uv数据转换成颜色，即纹理采样
                // fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;// * 的效果
                fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;// + 的效果
				// col的alpha通道
				col.a = _Transparency;
				// clip函数
				clip(col.r - _CutoutThresh);
				// if (col.r < _CutoutThresh) discard;// 如果红色值不够，就discard
				// apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
