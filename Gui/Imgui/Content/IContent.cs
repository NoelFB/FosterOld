using Foster.Framework;

namespace Foster.GuiSystem
{
    public interface IContent
    {


        void Draw(Imgui imgui, Batch2D batcher, StyleState style, Rect position);

        float Width(Imgui imgui);
        float Height(Imgui imgui);

        Imgui.Name UniqueInfo();

    }
}
