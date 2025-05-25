using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace First3DGamebyme.Engine.Camera;

public class Camera2D
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; private set; }
    public Viewport Viewport { get; private set; }
    
    private float _minZoom = 0.5f;
    private float _maxZoom = 3f;
    
    public Camera2D(Viewport viewport)
    {
        Viewport = viewport;
        Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        Position = Vector2.Zero;
        Zoom = 1f;
        Rotation = 0f;
    }
    
    public Matrix GetTransformMatrix()
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(Zoom, Zoom, 1) *
               Matrix.CreateTranslation(Origin.X, Origin.Y, 0);
    }
    
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Matrix inverseTransform = Matrix.Invert(GetTransformMatrix());
        return Vector2.Transform(screenPosition, inverseTransform);
    }
    
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, GetTransformMatrix());
    }
    
    public void Follow(Vector2 target, float lerpAmount = 0.1f)
    {
        Position = Vector2.Lerp(Position, target, lerpAmount);
    }
    
    public void ZoomIn(float amount = 0.1f)
    {
        Zoom = MathHelper.Clamp(Zoom + amount, _minZoom, _maxZoom);
    }
    
    public void ZoomOut(float amount = 0.1f)
    {
        Zoom = MathHelper.Clamp(Zoom - amount, _minZoom, _maxZoom);
    }
    
    public void UpdateViewport(Viewport viewport)
    {
        Viewport = viewport;
        Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
    }
}