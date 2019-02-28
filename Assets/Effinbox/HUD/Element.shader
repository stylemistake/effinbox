Shader "Effinbox/HUD/Element" {
  Properties {
    _MainTex ("Font Texture", 2D) = "white" {}
    _Color ("Text Color", Color) = (1, 1, 1, 1)
  }

  SubShader {
    Tags {
      "Queue" = "Transparent"
      "RenderType" = "Transparent"
    }

    Cull Back
    Lighting Off
    ZWrite Off
    ZTest [unity_GUIZTestMode]
    // Blend SrcAlpha OneMinusSrcAlpha

    Fog {
      Mode Off
    }

    BindChannels {
      Bind "Color", color
      Bind "Vertex", vertex
      Bind "TexCoord", texcoord
    }

    Pass {
      Blend SrcAlpha One
      SetTexture [_MainTex] {
        ConstantColor[_Color]
        combine constant * constant, texture * constant
        // combine previous * primary, previous * primary
      }
    }
  }
}
