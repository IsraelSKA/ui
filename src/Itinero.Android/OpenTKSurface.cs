using System;
using Android.Content;
using Android.Util;
using Mapsui;
using Mapsui.Rendering.OpenTK;
using OpenTK;
using OpenTK.Graphics.ES11;
using OpenTK.Platform.Android;

namespace Itinero.Android
{
    public class OpenTKSurface : AndroidGameView
    {
        private readonly MapRenderer _renderer;
        bool _refreshGraphics;
        public bool ViewportInitialized { get; set; }
        public Map Map { get; set; }

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

        public void RefreshGraphics()
        {
            _refreshGraphics = true;
            Invalidate();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (!_refreshGraphics) return;
            _refreshGraphics = false;

            if (!ViewportInitialized) return;

            Set2DViewport();

            GL.ClearColor(Map.BackColor.R, Map.BackColor.G, Map.BackColor.B, Map.BackColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

           _renderer.Render(Map.Viewport, Map.Layers);

            SwapBuffers();
        }
    }
}