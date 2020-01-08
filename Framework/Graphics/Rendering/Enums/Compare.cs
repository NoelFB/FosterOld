using System;
namespace Foster.Framework
{
    /// <summary>
    /// Compare Methods used during Rendering
    /// </summary>
    public enum Compare
    {
        /// <summary>
        /// The Comparison is ignored
        /// </summary>
        None,

        /// <summary>
        /// The Comparison always passes.
        /// </summary>
        Always,

        /// <summary>
        /// The Comparison never passes.
        /// </summary>
        Never,

        /// <summary>
        /// Passes if the value is less than the stored value.
        /// </summary>
        Less,

        /// <summary>
        /// Passes if the value is equal to the stored value.
        /// </summary>
        Equal,

        /// <summary>
        /// Passes if the value is less than or equal to the stored value.
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// Passes if the value is greater than the stored value.
        /// </summary>
        Greater,

        /// <summary>
        /// Passes if the value is not equal to the stored value.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Passes if the value is greater than or equal to the stored value.
        /// </summary>
        GreaterOrEqual
    }
}
