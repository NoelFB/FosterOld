using System;
namespace Foster.Framework
{
    public enum DepthFunctions
    {
        /// <summary>
        /// The Depth Function is disabled / ignored
        /// </summary>
        None,

        /// <summary>
        /// The depth test always passes.
        /// </summary>
        Always,

        /// <summary>
        /// The depth test never passes.
        /// </summary>
        Never,

        /// <summary>
        /// Passes if the fragment's depth value is less than the stored depth value.
        /// </summary>
        Less,

        /// <summary>
        /// Passes if the fragment's depth value is equal to the stored depth value.
        /// </summary>
        Equal,

        /// <summary>
        /// Passes if the fragment's depth value is less than or equal to the stored depth value.
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// Passes if the fragment's depth value is greater than the stored depth value.
        /// </summary>
        Greater,

        /// <summary>
        /// Passes if the fragment's depth value is not equal to the stored depth value.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Passes if the fragment's depth value is greater than or equal to the stored depth value.
        /// </summary>
        GreaterOrEqual
    }
}
