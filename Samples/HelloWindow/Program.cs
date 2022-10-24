
using Foster.Framework;
using Foster.GLFW;
using Foster.OpenGL;

class Program
{
    // C# Entry Point
    static void Main(string[] args)
    {
        // Register our System Module (GLFW in this case)
        App.Modules.Register<GLFW_System>();

        // Register our Graphics Module (OpenGL in this case)
        App.Modules.Register<GL_Graphics>();

        // Register our Custom Module, where we will run our own code
        App.Modules.Register<CustomModule>();

        // Begin Application
        App.Start("My Application", 1280, 720);
    }
}

public class CustomModule : Module
{
    public readonly Batch2D Batcher = new Batch2D();
    public float Offset = 0f;
    public float OffsetY = 0f;
    private Window secondWindow;

    // This is called when the Application has Started
    protected override void Startup()
    {
        // Add a Callback to the Primary Window's Render loop
        // By Default a single Window is created at startup
        // Alternatively App.System.Windows has a list of all open windows
        App.Window.OnRender += Render;

        secondWindow = new Window("Second Window", 1280, 720);
        secondWindow.OnRender += OnSecondaryWindowRender;
    }

    // This is called when the Application is shutting down
    // (or when the Module is removed)
    protected override void Shutdown()
    {
        // Remove our Callback
        App.Window.OnRender -= Render;
    }

    // This is called every frame of the Application
    protected override void Update()
    {
        if (App.Input.Keyboard.Down(Keys.Down))
        {
            OffsetY += 32 * Time.Delta;
        }

        Offset += 32 * Time.Delta;
    }

    private void Render(Window window)
    {
        // clear the Window
        App.Graphics.Clear(window, Color.Black);

        // clear the Batcher's data from the previous frame
        // (if you don't clear it, further calls will be added
        //  to the batcher, which will eventually create huge
        //  amounts of data)
        Batcher.Clear();

        // draw a rectangle
        Batcher.Rect(Offset, OffsetY, 32, 32, Color.Red);

        // draw the batcher to the Window
        Batcher.Render(window);
    }

    private void OnSecondaryWindowRender(Window window)
    {
        App.Graphics.Clear(window, Color.Yellow);

        Batcher.Render(window);
    }
}
