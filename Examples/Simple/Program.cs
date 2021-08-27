using Foster.Framework;
using Foster.OpenGL;
using Foster.SDL2;
class Program
{
    // C# Entry Point
    static void Main(string[] args)
    {
        // Register our System Module (SDL in this case)
        App.Modules.Register<SDL_System>();

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
    public float Offsetx = 640f;
    public float Offsety = 360f;

    public VirtualAxis movementx;
    public VirtualAxis movementy;
    // This is called when the Application has Started
    protected override void Startup()
    {
        // Add a Callback to the Primary Window's Render loop
        // By Default a single Window is created at startup
        // Alternatively App.System.Windows has a list of all open windows
        App.Window.OnRender += Render;


        // Create inputs for checking left/right and up/down directions
        movementx = new VirtualAxis(App.Input);
        movementx.Add(Keys.Left, Keys.Right)
                .Add(0, Buttons.Left, Buttons.Right);

        movementy = new VirtualAxis(App.Input);
        movementy.Add(Keys.Up, Keys.Down)
                .Add(0, Buttons.Up, Buttons.Down);
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
        // Increase or decrease offset based on inputs
        Offsetx += movementx.IntValue * 32 * Time.Delta;
        Offsety += movementy.IntValue * 32 * Time.Delta;
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
        Batcher.Rect(Offsetx, Offsety, 32, 32, Color.Red);

        // draw the batcher to the Window
        Batcher.Render(window);
    }
}