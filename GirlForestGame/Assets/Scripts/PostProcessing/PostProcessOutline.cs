using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Outline", typeof(UniversalRenderPipeline))]
public class PostProcessOutline : VolumeComponent, IPostProcessComponent
{
    public FloatParameter thickness = new FloatParameter(1);   
    public FloatParameter depthMin = new FloatParameter(0);   
    public FloatParameter depthMax = new FloatParameter (1);

    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}

//public sealed class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
//{
//    public override void Render(PostProcessRenderContext context)
//    {
//        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Outline"));

//        sheet.properties.SetFloat("_Thickness", settings.thickness);
//        sheet.properties.SetFloat("_MinDepth", settings.depthMin);
//        sheet.properties.SetFloat("_MaxDepth", settings.depthMax);

//        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
//    }
//}

public class OutlineRenderFeature : ScriptableRendererFeature
{
    private OutlinePass outlinePass;

    public override void Create()
    {
        outlinePass = new OutlinePass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
       renderer.EnqueuePass(outlinePass);
    }

    class OutlinePass : ScriptableRenderPass
    {
        private Material _mat;
        int outlineId = Shader.PropertyToID("_Temp");
        RenderTargetIdentifier src, outline;

        public OutlinePass()
        {
            _mat = CoreUtils.CreateEngineMaterial("Unlit/Outline");
            
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            src = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.GetTemporaryRT(outlineId, desc, FilterMode.Bilinear);
            outline = new RenderTargetIdentifier();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("OutlineRenderFeature");
            VolumeStack volumes = VolumeManager.instance.stack;

            PostProcessOutline outlineData = volumes.GetComponent<PostProcessOutline>();

            if (outlineData.IsActive())
            {
                _mat.SetFloat("_Thickness", (float)outlineData.thickness);
                _mat.SetFloat("_DepthMin", (float)outlineData.depthMin);
                _mat.SetFloat("_DepthMax", (float)outlineData.depthMax);


                Blit(commandBuffer, src, outline, _mat, 0);
                Blit(commandBuffer, outline, src);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(outlineId);
        }
    }
}



