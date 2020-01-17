namespace Foster.GuiSystem
{
    public static class ImguiHeader
    {
        public static bool BeginHeader(this Imgui imgui, string label, bool startOpen = false)
        {
            var style = imgui.Style.Header;
            var id = imgui.Id(label);
            var enabled = imgui.Storage.GetBool(id, 0, startOpen);

            imgui.PushSpacing(0);

            // do button behavour first
            var position = imgui.Cell(Size.Fill(), Size.Explicit(imgui.FontSize + style.Idle.Padding.Y * 2));
            if (imgui.ButtonBehaviour(id, position))
                enabled = !enabled;

            imgui.PopSpacing();

            // draw
            if (position.Intersects(imgui.Clip))
            {
                var inner = imgui.Box(position, style, id);
                var state = style.Current(imgui.ActiveId, imgui.HotId, id);
                var content = new Text((enabled ? "v " : "> ") + label);
                content.Draw(imgui, imgui.Batcher, state, inner);
            }

            // store result
            imgui.Storage.SetBool(id, 0, enabled);

            // indent tab
            if (enabled)
            {
                imgui.PushIndent(imgui.Spacing);
                imgui.PushId(id);
            }

            return enabled;
        }

        public static void EndHeader(this Imgui imgui)
        {
            imgui.PopIndent();
            imgui.PopId();
        }
    }
}
