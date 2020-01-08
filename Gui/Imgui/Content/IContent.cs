using Foster.Framework;

namespace Foster.GuiSystem
{
    public interface IContent
    {

        Vector2 PreferredSize(Imgui imgui);

        void Draw(Imgui imgui, Batch2D batcher, StyleState style, Rect position);
        
        Imgui.Name UniqueInfo();

        public Vector2 PreferredPaddedSize(Imgui imgui, Vector2 padding)
        {
            var size = PreferredSize(imgui);
            size += padding * 2;
            return size;
        }

        public Vector2 PreferredPaddedSize(Imgui imgui, StyleState style)
        {
            var size = PreferredSize(imgui);
            size += style.Padding * 2;
            return size;
        }

    }
}
