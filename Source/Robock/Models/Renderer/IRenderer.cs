using System;

using Robock.Models.CaptureSources;

namespace Robock.Models.Renderer
{
    public interface IRenderer
    {
        /// <summary>
        ///     human readable renderer name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     priority for default selection
        /// </summary>
        uint Priority { get; }

        /// <summary>
        ///     the feature is supported in this environment in it is running
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        ///     window picker is supported in this renderer
        /// </summary>
        bool HasOwnWindowPicker { get; }

        /// <summary>
        ///     configure capture source, 1st phase for rendering, called by ViewModel (user action)
        /// </summary>
        /// <param name="parameters"></param>
        void ConfigureCaptureSource(params object[] parameters);

        // XXX: This method called from ViewModel, but this method returns ICaptureSource. You should returns void or Task and use observed property.
        /// <summary>
        ///     if HasOwnPicker set to true, ViewModel call this method for showing window picker
        /// </summary>
        ICaptureSource ShowWindowPicker();

        /// <summary>
        ///     initialize DirectX resources, 2nd phase for rendering, called by DxInterop
        /// </summary>
        void InitializeRenderer();

        /// <summary>
        ///     render capture surface to shared surface, 3rd phase for rendering, called by DxInterop
        /// </summary>
        void Render(IntPtr hSurface, bool isNewSurface);

        /// <summary>
        ///     releases capture source, last phase for rendering, called by ViewModel
        /// </summary>
        void ReleaseCaptureSource();

        /// <summary>
        ///     same as dispose, but the instance should be reusable
        /// </summary>
        void Release();
    }
}