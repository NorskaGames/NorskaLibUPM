using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.UI
{
    public enum ShowWindowMode
    {
        /// <summary>
        /// Show this window and hide all others.
        /// </summary>
        Single,

        /// <summary>
        /// Show this window, other remain in their shown/hidden state.
        /// </summary>
        Additive,

        /// <summary>
        /// Show this window and hide all other screens with same order value.
        /// </summary>
        SoloInLayer
    }
}