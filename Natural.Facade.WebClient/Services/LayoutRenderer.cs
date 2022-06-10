namespace Natural.Facade.WebClient
{
    public class LayoutRenderer
    {
        /// <summary>The root node to render.</summary>
        private Blazor.Extensions.Canvas.Canvas2D.Canvas2DContext m_context = null;

        /// <summary>The root node to render.</summary>
        private LayoutNodes.LayoutRootNode m_layoutRootNode = null;

        /// <summary>The root node to render.</summary>
        private System.Drawing.RectangleF m_canvasBounds;

        /// <summary>Constructor.</summary>
        public LayoutRenderer(Blazor.Extensions.Canvas.Canvas2D.Canvas2DContext context, LayoutNodes.LayoutRootNode layoutRootNode, int canvasWidth, int canvasHeight)
        {
            m_context = context;
            m_layoutRootNode = layoutRootNode;
            m_canvasBounds = new System.Drawing.RectangleF(0.0f, 0.0f, canvasWidth, canvasHeight);
        }

        /// <summary>Renders the layout.</summary>
        public async Task RenderAsync()
        {
            await m_context.BeginBatchAsync();
            await m_context.ClearRectAsync(0.0, 0.0, 1920.0, 1080.0);
            await RenderElementAsync(m_layoutRootNode.RootElementNode, m_canvasBounds);
            await m_context.EndBatchAsync();
        }

        /// <summary>Renders an element.</summary>
        private async Task RenderElementAsync(LayoutNodes.LayoutElementNode elementNode, System.Drawing.RectangleF bounds)
        {
            switch (elementNode.ElementType)
            {
                case LayoutNodes.ElementType.stack:
                    if (elementNode.ChildArray != null)
                    {
                        foreach (LayoutNodes.LayoutElementNode childNode in elementNode.ChildArray)
                        {
                            await RenderElementAsync(childNode, bounds);
                        }
                    }
                    break;
                case LayoutNodes.ElementType.image:
                    await RenderImageAsync(elementNode, bounds);
                    break;
            }
        }

        /// <summary>Renders an image element.</summary>
        private async Task RenderImageAsync(LayoutNodes.LayoutElementNode elementNode, System.Drawing.RectangleF bounds)
        {
            /*Microsoft.AspNetCore.Components..HTMLImageElement imageElement = null;
            Microsoft.AspNetCore.Components.ElementReference imageElement = null;*/

            await m_context.SetFillStyleAsync("#003366");
            await m_context.FillRectAsync(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        }
    }
}
