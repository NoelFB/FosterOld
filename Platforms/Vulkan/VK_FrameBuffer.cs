using Foster.Framework;

namespace Foster.Vulkan;

internal sealed class VK_FrameBuffer : FrameBuffer.Platform
{
    public VK_FrameBuffer(VK_Graphics graphics, int width, int height, TextureFormat[] attachments)
    {
        for (int i = 0; i < attachments.Length; i++)
        {
            var attachment = new Texture(graphics, width, height, attachments[i]);
            var texture = (VK_Texture)attachment.Implementation;
            texture.isRenderTexture = true;
            Attachments.Add(attachment);
        }
    }

    protected override void Dispose()
    {

    }

    protected override void Resize(int width, int height)
    {
        Dispose();

        for (int i = 0; i < Attachments.Count; i++)
            Attachments[i].Resize(width, height);
    }
}
