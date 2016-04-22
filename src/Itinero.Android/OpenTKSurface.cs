using System;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Rendering.OpenTK;
using Mapsui.Styles;
using OpenTK;
using OpenTK.Graphics.ES11;
using OpenTK.Platform.Android;

namespace Itinero.Android
{
    public class OpenTKSurface : AndroidGameView
    {
        private readonly MapRenderer _renderer;
        private bool _refreshGraphics;
        private Color _backColor;
        private IViewport _viewport;
        private IEnumerable<ILayer> _layers;
        private Action _updateMarkers;

        public OpenTKSurface(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _renderer = new MapRenderer();
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Disable(All.DepthTest);
            Run(60); 
        }
    
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected void UnloadContent()
        {
        }

        void Set2DViewport()
        {
            GL.MatrixMode(All.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, 0, 1);
            GL.MatrixMode(All.Modelview);
        }

        public void RefreshGraphics(IViewport viewport, IEnumerable<ILayer> layers, Color backColor, Action updateMarkers)
        {
            _backColor = backColor;
            _viewport = viewport;
            _layers = layers;
            _updateMarkers = updateMarkers;
            _refreshGraphics = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (!_refreshGraphics) return;
            _refreshGraphics = false;
            
            Set2DViewport();

            GL.ClearColor(_backColor.R, _backColor.G, _backColor.B, _backColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _renderer.Render(_viewport, _layers);
            _updateMarkers();

            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}