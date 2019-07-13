Shader "Unluck Software/Zippy2DBlend (Desktop)" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Blend("Blend", Range(0, 1)) = 0.5
		_MainTex("Texture 1", 2D) = ""
		_Texture2("Texture 2", 2D) = ""
	}
	Category{
		Tags{ "RenderType" = "Opaque" }
		Lighting Off
		SubShader{
			Pass{
				SetTexture[_MainTex]
				SetTexture[_Texture2]{
					ConstantColor(0,0,0,[_Blend])
					Combine texture Lerp(constant) previous	
				}
				SetTexture[_]{
					constantColor[_TintColor]
					combine constant lerp(texture) previous DOUBLE
				}
			}
		}
	}
}