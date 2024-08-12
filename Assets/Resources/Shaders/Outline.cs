using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TEE.Shaders {
    [Serializable, PostProcess(typeof(OutlinePostProcess), PostProcessEvent.BeforeStack, "Custom/Outline")]
    public sealed class Outline : PostProcessEffectSettings {
        [Range(1f, 5f), Tooltip("Outline thickness.")]
        public IntParameter thickness = new() { value = 2 };

        [Range(0f, 5f), Tooltip("Outline edge start.")]
        public FloatParameter edge = new() { value = 0.1f };

        [Range(0f, 1f), Tooltip("Outline smoothness transition on close objects.")]
        public FloatParameter transitionSmoothness = new() { value = 0.2f };

        [Tooltip("Outline color.")] public ColorParameter color = new() { value = Color.black };
    }

    public sealed class OutlinePostProcess : PostProcessEffectRenderer<Outline> {
        static readonly int ShaderPropertyThickness            = Shader.PropertyToID("_Thickness");
        static readonly int ShaderPropertyTransitionSmoothness = Shader.PropertyToID("_TransitionSmoothness");
        static readonly int ShaderPropertyEdge                 = Shader.PropertyToID("_Edge");
        static readonly int ShaderPropertyColor                = Shader.PropertyToID("_Color");

        public override void Render(PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Outline"));
            sheet.properties.SetInt(ShaderPropertyThickness, settings.thickness);
            sheet.properties.SetFloat(ShaderPropertyTransitionSmoothness, settings.transitionSmoothness);
            sheet.properties.SetFloat(ShaderPropertyEdge,                 settings.edge);
            sheet.properties.SetColor(ShaderPropertyColor, settings.color);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}