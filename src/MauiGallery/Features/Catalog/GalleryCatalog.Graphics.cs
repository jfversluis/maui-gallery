using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Graphics;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static readonly NativeMapping[] GraphicsCanvasMappings =
    [
        new(GalleryPlatforms.iOS, "PlatformGraphicsView (UIView + Core Graphics)"),
        new(GalleryPlatforms.Android, "PlatformGraphicsView (View + Canvas)"),
        new(GalleryPlatforms.MacCatalyst, "PlatformGraphicsView (UIView + Core Graphics)"),
        new(GalleryPlatforms.Windows, "W2DGraphicsView (Win2D CanvasControl)"),
    ];

    private static GalleryItem[] GraphicsItems() =>
    [
        new()
        {
            Id = "shapes",
            Title = "Shapes",
            Description = "Draws rectangles, ellipses, lines, polylines and polygons with fills, strokes and dashes.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.Shapes24,
            PageType = typeof(ShapesPage),
            Keywords = ["Rectangle", "Ellipse", "Circle", "Line", "Polyline", "Polygon", "Fill", "Stroke", "StrokeDashArray", "StrokeLineCap", "StrokeLineJoin", "FillRule", "vector", "drawing"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MauiShapeView (PlatformGraphicsView)"),
                new(GalleryPlatforms.Android, "MauiShapeView (PlatformGraphicsView)"),
                new(GalleryPlatforms.MacCatalyst, "MauiShapeView (PlatformGraphicsView)"),
                new(GalleryPlatforms.Windows, "MauiShapeView (W2DGraphicsView)"),
            ],
            RelatedIds = ["paths", "brushes", "graphics-view", "border"],
            DocsUrl = new(DocsRoot + "user-interface/controls/shapes/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.shapes.shape"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/Shapes"),
        },
        new()
        {
            Id = "paths",
            Title = "Paths and geometries",
            Description = "Draws any outline from path markup or geometries such as ellipses, rectangles and Bezier curves.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.DrawShape24,
            PageType = typeof(PathsPage),
            Keywords = ["Path", "Data", "path markup", "SVG", "icon", "Geometry", "EllipseGeometry", "RectangleGeometry", "PathGeometry", "GeometryGroup", "PathFigure", "BezierSegment", "ArcSegment", "Aspect", "Stretch", "FillRule", "vector"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MauiShapeView (PlatformGraphicsView)"),
                new(GalleryPlatforms.Android, "MauiShapeView (PlatformGraphicsView)"),
                new(GalleryPlatforms.MacCatalyst, "MauiShapeView (PlatformGraphicsView)"),
                new(GalleryPlatforms.Windows, "MauiShapeView (W2DGraphicsView)"),
            ],
            RelatedIds = ["shapes", "clipping", "graphics-view"],
            DocsUrl = new(DocsRoot + "user-interface/controls/shapes/path"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.shapes.path"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/Paths"),
        },
        new()
        {
            Id = "brushes",
            Title = "Brushes",
            Description = "Paints backgrounds, fills and strokes with solid colors and linear or radial gradients.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.PaintBrush24,
            PageType = typeof(BrushesPage),
            Keywords = ["Brush", "SolidColorBrush", "LinearGradientBrush", "RadialGradientBrush", "gradient", "GradientStop", "Background", "Fill", "color", "transparency"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CAGradientLayer"),
                new(GalleryPlatforms.Android, "MauiDrawable (LinearGradient / RadialGradient shader)"),
                new(GalleryPlatforms.MacCatalyst, "CAGradientLayer"),
                new(GalleryPlatforms.Windows, "LinearGradientBrush / RadialGradientBrush"),
            ],
            RelatedIds = ["shapes", "shadows", "graphics-view", "border"],
            DocsUrl = new(DocsRoot + "user-interface/brushes/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.brush"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/Brushes"),
        },
        new()
        {
            Id = "shadows",
            Title = "Shadows",
            Description = "Adds a blurred, offset, colored shadow to any view that follows the outline of what it draws.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.SquareShadow24,
            PageType = typeof(ShadowsPage),
            Keywords = ["Shadow", "drop shadow", "elevation", "glow", "blur", "Offset", "Radius", "Opacity", "depth", "card"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CALayer shadow"),
                new(GalleryPlatforms.Android, "WrapperView (blurred shadow bitmap)"),
                new(GalleryPlatforms.MacCatalyst, "CALayer shadow"),
                new(GalleryPlatforms.Windows, "Composition DropShadow"),
            ],
            RelatedIds = ["border", "brushes", "shapes"],
            DocsUrl = new(DocsRoot + "user-interface/shadow"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.shadow"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/Shadows"),
        },
        new()
        {
            Id = "clipping",
            Title = "Clipping",
            Description = "Crops any view to a circle, a rounded rectangle or an outline of your own with a geometry.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.Crop24,
            PageType = typeof(ClippingPage),
            Keywords = ["Clip", "crop", "mask", "avatar", "circle", "rounded corners", "EllipseGeometry", "RoundRectangleGeometry", "PathGeometry", "image"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CAShapeLayer mask"),
                new(GalleryPlatforms.Android, "WrapperView (clip path)"),
                new(GalleryPlatforms.MacCatalyst, "CAShapeLayer mask"),
                new(GalleryPlatforms.Windows, "Composition geometric clip"),
            ],
            RelatedIds = ["paths", "transforms", "border"],
            DocsUrl = new(DocsRoot + "user-interface/controls/shapes/geometries#clip-with-a-geometry"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.visualelement.clip"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/Clipping"),
        },
        new()
        {
            Id = "transforms",
            Title = "Transforms and animation",
            Description = "Rotates, scales and moves views, in 2D or 3D, around an anchor point, and animates them.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.ArrowRotateClockwise24,
            PageType = typeof(TransformsPage),
            Keywords = ["Rotation", "RotationX", "RotationY", "Scale", "ScaleX", "ScaleY", "TranslationX", "TranslationY", "AnchorX", "AnchorY", "transform", "animation", "RotateToAsync", "ScaleToAsync", "TranslateToAsync", "3D", "perspective", "flip"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CALayer transform (CATransform3D)"),
                new(GalleryPlatforms.Android, "View.Rotation, ScaleX, TranslationX, PivotX"),
                new(GalleryPlatforms.MacCatalyst, "CALayer transform (CATransform3D)"),
                new(GalleryPlatforms.Windows, "RenderTransform / PlaneProjection"),
            ],
            RelatedIds = ["clipping", "shadows", "graphics-view"],
            DocsUrl = new(DocsRoot + "user-interface/animation/basic"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.viewextensions"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/Transforms"),
        },
        new()
        {
            Id = "graphics-view",
            Title = "GraphicsView",
            Description = "A canvas for your own drawing code: charts, gauges, signatures and sketches, with touch input.",
            CategoryId = CategoryIds.Graphics,
            IconGlyph = FluentIconsRegular.InkingTool24,
            PageType = typeof(GraphicsViewPage),
            Keywords = ["canvas", "IDrawable", "ICanvas", "Microsoft.Maui.Graphics", "draw", "chart", "gauge", "sketch", "signature", "Invalidate", "StartInteraction", "DragInteraction", "custom drawing"],
            NativeMappings = GraphicsCanvasMappings,
            RelatedIds = ["shapes", "paths", "brushes"],
            DocsUrl = new(DocsRoot + "user-interface/controls/graphicsview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.graphicsview"),
            SourceUrl = new(SourceRoot + "Controls/Graphics/GraphicsView"),
        },
    ];
}
