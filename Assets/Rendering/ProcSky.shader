// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader"Skybox/ProcSky" {
Properties {
    [KeywordEnum(None, Simple, High Quality)] _SunDisk ("Sun", Int) = 2
    _SunSize ("Sun Size", Range(0,1)) = 0.04
    _SunSizeConvergence("Sun Size Convergence", Range(1,10)) = 5

    _AtmosphereThickness ("Atmosphere Thickness", Range(0,5)) = 1.0
    _SkyTint ("Sky Tint", Color) = (.5, .5, .5, 1)
    _GroundColor ("Ground", Color) = (.369, .349, .341, 1)

    _Exposure("Exposure", Range(0, 8)) = 1.3

    // --- Clouds ---
    _CloudTex ("Cloud Noise", 2D) = "white" {}
    _CloudTint ("Cloud Tint", Color) = (1,1,1,1)
    _CloudIntensity ("Cloud Intensity", Range(0,1)) = 0.5
    _CloudScale ("Cloud Scale", Float) = 0.2
    _CloudSpeed ("Cloud Speed", Float) = 0.01

// --- Stars ---
    _StarIntensity ("Star Intensity", Range(0,3)) = 1.0
    _StarDensity ("Star Density", Float) = 50.0
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
Cull Off

ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"

        #pragma multi_compile_local _SUNDISK_NONE _SUNDISK_SIMPLE _SUNDISK_HIGH_QUALITY

// Stars uniforms
half _StarIntensity;
half _StarDensity;

        // Simple hash for procedural stars
float hash21(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return frac(p.x * p.y);
}

uniform half _Exposure; // HDR exposure
uniform half3 _GroundColor;
uniform half _SunSize;
uniform half _SunSizeConvergence;
uniform half3 _SkyTint;
uniform half _AtmosphereThickness;

        // --- Clouds uniforms ---
sampler2D _CloudTex;
float4 _CloudTex_ST;
half3 _CloudTint;
half _CloudIntensity;
half _CloudScale;
half _CloudSpeed;

#if defined(UNITY_COLORSPACE_GAMMA)
#define GAMMA 2
#define COLOR_2_GAMMA(color) color
#define COLOR_2_LINEAR(color) color*color
#define LINEAR_2_OUTPUT(color) sqrt(color)
#else
#define GAMMA 2.2
            // HACK: to get gfx-tests in Gamma mode to agree until UNITY_ACTIVE_COLORSPACE_IS_GAMMA is working properly
#define COLOR_2_GAMMA(color) ((unity_ColorSpaceDouble.r>2.0) ? pow(color,1.0/GAMMA) : color)
#define COLOR_2_LINEAR(color) color
#define LINEAR_2_LINEAR(color) color
#endif

        // RGB wavelengths
static const float3 kDefaultScatteringWavelength = float3(.65, .57, .475);
static const float3 kVariableRangeForScatteringWavelength = float3(.15, .15, .15);

#define OUTER_RADIUS 1.025
static const float kOuterRadius = OUTER_RADIUS;
static const float kOuterRadius2 = OUTER_RADIUS * OUTER_RADIUS;
static const float kInnerRadius = 1.0;
static const float kInnerRadius2 = 1.0;

static const float kCameraHeight = 0.0001;

#define kRAYLEIGH (lerp(0.0, 0.0025, pow(_AtmosphereThickness,2.5)))      // Rayleigh constant
#define kMIE 0.0010             // Mie constant
#define kSUN_BRIGHTNESS 20.0    // Sun brightness

#define kMAX_SCATTER 50.0 // Prevent overflow

static const half kHDSundiskIntensityFactor = 15.0;
static const half kSimpleSundiskIntensityFactor = 27.0;

static const half kSunScale = 400.0 * kSUN_BRIGHTNESS;
static const float kKmESun = kMIE * kSUN_BRIGHTNESS;
static const float kKm4PI = kMIE * 4.0 * 3.14159265;
static const float kScale = 1.0 / (OUTER_RADIUS - 1.0);
static const float kScaleDepth = 0.25;
static const float kScaleOverScaleDepth = (1.0 / (OUTER_RADIUS - 1.0)) / 0.25;
static const float kSamples = 2.0; // unrolled

#define MIE_G (-0.990)
#define MIE_G2 0.9801

#define SKY_GROUND_THRESHOLD 0.02

        // sun disk modes
#define SKYBOX_SUNDISK_NONE 0
#define SKYBOX_SUNDISK_SIMPLE 1
#define SKYBOX_SUNDISK_HQ 2

#ifndef SKYBOX_SUNDISK
#if defined(_SUNDISK_NONE)
#define SKYBOX_SUNDISK SKYBOX_SUNDISK_NONE
#elif defined(_SUNDISK_SIMPLE)
#define SKYBOX_SUNDISK SKYBOX_SUNDISK_SIMPLE
#else
#define SKYBOX_SUNDISK SKYBOX_SUNDISK_HQ
#endif
#endif

#ifndef SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
#if defined(SHADER_API_MOBILE)
#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 1
#else
#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 0
#endif
#endif

        // Rayleigh phase
half getRayleighPhase(half eyeCos2)
{
    return 0.75 + 0.75 * eyeCos2;
}
half getRayleighPhase(half3 light, half3 ray)
{
    half eyeCos = dot(light, ray);
    return getRayleighPhase(eyeCos * eyeCos);
}

struct appdata_t
{
    float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 pos : SV_POSITION;

#if SKYBOX_SUNDISK == SKYBOX_SUNDISK_HQ
            float3  vertex          : TEXCOORD0; // per-pixel ray
#elif SKYBOX_SUNDISK == SKYBOX_SUNDISK_SIMPLE
            half3   rayDir          : TEXCOORD0;
#else
    half skyGroundFactor : TEXCOORD0;
#endif

    half3 groundColor : TEXCOORD1;
    half3 skyColor : TEXCOORD2;

#if SKYBOX_SUNDISK != SKYBOX_SUNDISK_NONE
    half3 sunColor : TEXCOORD3;
#endif

            // Always carry a ray for clouds, independent of sun mode
    half3 cloudRay : TEXCOORD4;

            UNITY_VERTEX_OUTPUT_STEREO
};

float scale(float inCos)
{
    float x = 1.0 - inCos;
    return 0.25 * exp(-0.00287 + x * (0.459 + x * (3.83 + x * (-6.80 + x * 5.25))));
}

v2f vert(appdata_t v)
{
    v2f OUT;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    OUT.pos = UnityObjectToClipPos(v.vertex);

    float3 kSkyTintInGammaSpace = COLOR_2_GAMMA(_SkyTint);
    float3 kScatteringWavelength = lerp(
                kDefaultScatteringWavelength - kVariableRangeForScatteringWavelength,
                kDefaultScatteringWavelength + kVariableRangeForScatteringWavelength,
                half3(1, 1, 1) - kSkyTintInGammaSpace);
    float3 kInvWavelength = 1.0 / pow(kScatteringWavelength, 4);

    float kKrESun = kRAYLEIGH * kSUN_BRIGHTNESS;
    float kKr4PI = kRAYLEIGH * 4.0 * 3.14159265;

    float3 cameraPos = float3(0, kInnerRadius + kCameraHeight, 0);
    float3 eyeRay = normalize(mul((float3x3) unity_ObjectToWorld, v.vertex.xyz));

    float far = 0.0;
    half3 cIn, cOut;

    if (eyeRay.y >= 0.0)
    {
                // Sky
        far = sqrt(kOuterRadius2 + kInnerRadius2 * eyeRay.y * eyeRay.y - kInnerRadius2) - kInnerRadius * eyeRay.y;

        float3 pos = cameraPos + far * eyeRay;

        float height = kInnerRadius + kCameraHeight;
        float depth = exp(kScaleOverScaleDepth * (-kCameraHeight));
        float startAngle = dot(eyeRay, cameraPos) / height;
        float startOffset = depth * scale(startAngle);

        float sampleLength = far / kSamples;
        float scaledLength = sampleLength * kScale;
        float3 sampleRay = eyeRay * sampleLength;
        float3 samplePoint = cameraPos + sampleRay * 0.5;

        float3 frontColor = float3(0.0, 0.0, 0.0);
                {
            float heightS = length(samplePoint);
            float depthS = exp(kScaleOverScaleDepth * (kInnerRadius - heightS));
            float lightAngle = dot(_WorldSpaceLightPos0.xyz, samplePoint) / heightS;
            float cameraAngle = dot(eyeRay, samplePoint) / heightS;
            float scatter = (startOffset + depthS * (scale(lightAngle) - scale(cameraAngle)));
            float3 attenuate = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));

            frontColor += attenuate * (depthS * scaledLength);
            samplePoint += sampleRay;
        }
                {
            float heightS = length(samplePoint);
            float depthS = exp(kScaleOverScaleDepth * (kInnerRadius - heightS));
            float lightAngle = dot(_WorldSpaceLightPos0.xyz, samplePoint) / heightS;
            float cameraAngle = dot(eyeRay, samplePoint) / heightS;
            float scatter = (startOffset + depthS * (scale(lightAngle) - scale(cameraAngle)));
            float3 attenuate = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));

            frontColor += attenuate * (depthS * scaledLength);
            samplePoint += sampleRay;
        }

        cIn = frontColor * (kInvWavelength * kKrESun);
        cOut = frontColor * kKmESun;
    }
    else
    {
                // Ground
        far = (-kCameraHeight) / (min(-0.001, eyeRay.y));
        float3 pos = cameraPos + far * eyeRay;

        float depth = exp((-kCameraHeight) * (1.0 / kScaleDepth));
        float cameraAngle = dot(-eyeRay, pos);
        float lightAngle = dot(_WorldSpaceLightPos0.xyz, pos);
        float cameraScale = scale(cameraAngle);
        float lightScale = scale(lightAngle);
        float cameraOffset = depth * cameraScale;
        float temp = (lightScale + cameraScale);

        float sampleLength = far / kSamples;
        float scaledLength = sampleLength * kScale;
        float3 sampleRay = eyeRay * sampleLength;
        float3 samplePoint = cameraPos + sampleRay * 0.5;

        float3 frontColor = float3(0.0, 0.0, 0.0);
        float3 attenuate;
                {
            float heightS = length(samplePoint);
            float depthS = exp(kScaleOverScaleDepth * (kInnerRadius - heightS));
            float scatter = depthS * temp - cameraOffset;
            attenuate = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));
            frontColor += attenuate * (depthS * scaledLength);
            samplePoint += sampleRay;
        }

        cIn = frontColor * (kInvWavelength * kKrESun + kKmESun);
        cOut = clamp(attenuate, 0.0, 1.0);
    }

#if SKYBOX_SUNDISK == SKYBOX_SUNDISK_HQ
            OUT.vertex          = -eyeRay;
#elif SKYBOX_SUNDISK == SKYBOX_SUNDISK_SIMPLE
            OUT.rayDir          = half3(-eyeRay);
#else
    OUT.skyGroundFactor = -eyeRay.y / SKY_GROUND_THRESHOLD;
#endif

    OUT.groundColor = _Exposure * (cIn + COLOR_2_LINEAR(_GroundColor) * cOut);
    OUT.skyColor = _Exposure * (cIn * getRayleighPhase(_WorldSpaceLightPos0.xyz, -eyeRay));

#if SKYBOX_SUNDISK != SKYBOX_SUNDISK_NONE
    half lightColorIntensity = clamp(length(_LightColor0.xyz), 0.25, 1);
#if SKYBOX_SUNDISK == SKYBOX_SUNDISK_SIMPLE
            OUT.sunColor    = kSimpleSundiskIntensityFactor * saturate(cOut * kSunScale) * _LightColor0.xyz / lightColorIntensity;
#else // SKYBOX_SUNDISK_HQ
    OUT.sunColor = kHDSundiskIntensityFactor * saturate(cOut) * _LightColor0.xyz / lightColorIntensity;
#endif
#endif

#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
            OUT.groundColor = sqrt(OUT.groundColor);
            OUT.skyColor    = sqrt(OUT.skyColor);
#if SKYBOX_SUNDISK != SKYBOX_SUNDISK_NONE
            OUT.sunColor    = sqrt(OUT.sunColor);
#endif
#endif

            // Always supply a ray for clouds
    OUT.cloudRay = half3(-eyeRay);

    return OUT;
}

        // Mie phase
half getMiePhase(half eyeCos, half eyeCos2)
{
    half temp = 1.0 + MIE_G2 - 2.0 * MIE_G * eyeCos;
    temp = pow(temp, pow(_SunSize, 0.65) * 10);
    temp = max(temp, 1.0e-4);
    temp = 1.5 * ((1.0 - MIE_G2) / (2.0 + MIE_G2)) * (1.0 + eyeCos2) / temp;
#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
            temp = pow(temp, .454545);
#endif
    return temp;
}

        // Sun shape
half calcSunAttenuation(half3 lightPos, half3 ray)
{
#if SKYBOX_SUNDISK == SKYBOX_SUNDISK_SIMPLE
            half3 delta = lightPos - ray;
            half dist = length(delta);
            half spot = 1.0 - smoothstep(0.0, _SunSize, dist);
            return spot * spot;
#else // SKYBOX_SUNDISK_HQ
    half focusedEyeCos = pow(saturate(dot(lightPos, ray)), _SunSizeConvergence);
    return getMiePhase(-focusedEyeCos, focusedEyeCos * focusedEyeCos);
#endif
}

// --- FRAG PATCH ---
half4 frag(v2f IN) : SV_Target
{
    half3 col = half3(0, 0, 0);

#if SKYBOX_SUNDISK == SKYBOX_SUNDISK_HQ
                half3 ray = normalize(IN.vertex.xyz);
                half y = ray.y / SKY_GROUND_THRESHOLD;
#elif SKYBOX_SUNDISK == SKYBOX_SUNDISK_SIMPLE
                half3 ray = IN.rayDir.xyz;
                half y = ray.y / SKY_GROUND_THRESHOLD;
#else
    half y = IN.skyGroundFactor;
#endif

    col = lerp(IN.skyColor, IN.groundColor, saturate(y));

#if SKYBOX_SUNDISK != SKYBOX_SUNDISK_NONE
    if (y < 0.0)
    {
        col += IN.sunColor * calcSunAttenuation(_WorldSpaceLightPos0.xyz, -ray);
    }
#endif

            //  sun height drives day/night blend
    float sunHeight = saturate(dot(_WorldSpaceLightPos0.xyz, float3(0, 1, 0)));
    float dayFactor = smoothstep(0.05, 0.2, sunHeight); // 1=day, 0=night
    float nightFactor = 1.0 - dayFactor;

            //  Clouds (day only)
    if (dayFactor > 0.01 && y < 0.0)
    {
        float2 uv = IN.cloudRay.xz * _CloudScale;
        uv += _Time.y * _CloudSpeed;
        half cloudNoise = tex2D(_CloudTex, uv).r;
        half cloudMask = smoothstep(0.4, 0.6, cloudNoise);
        half3 cloudCol = _CloudTint.rgb * cloudMask * _CloudIntensity * dayFactor;
        col = lerp(col, col + cloudCol, cloudMask);
    }

            // Stars (night only)
    if (nightFactor > 0.01 && y < 0.0)
    {
        float2 starUV = normalize(ray).xz * _StarDensity;
        float starVal = step(0.995, hash21(starUV));
        half3 starCol = half3(starVal, starVal, starVal) * _StarIntensity * nightFactor;
        col += starCol;
    }

#if defined(UNITY_COLORSPACE_GAMMA) && !SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
                col = LINEAR_2_OUTPUT(col);
#endif

    return half4(col, 1);
}
        ENDCG
    }
}
Fallback Off

CustomEditor"SkyboxProceduralShaderGUI"
}