using System;
using System.Runtime.InteropServices;
using Foster.Framework;
using Foster.Engine;

// The Guid is used to reference this Component throughout the Project
// Changing it will break all references to this Component

[GuidAttribute("{Guid}")]
public class {Name} : Component, IUpdate
{

    public override void OnStart()
    {
        // Called before the first Update event
    }

    public void Update()
    {
        // Called every frame
    }

}
