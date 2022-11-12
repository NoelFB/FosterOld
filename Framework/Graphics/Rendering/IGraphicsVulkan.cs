﻿using System;

namespace Foster.Framework;

/// <summary>
/// An Implementation of the Graphics Module that supports the Vulkan Graphics API
/// </summary>
public interface IGraphicsVulkan
{
    IntPtr GetVulkanInstancePointer();
}
