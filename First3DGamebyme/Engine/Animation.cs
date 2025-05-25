using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace First3DGamebyme.Engine;

public class Animation
{
    public List<Rectangle> Frames { get; private set; }
    public float FrameDuration { get; set; }
    public bool IsLooping { get; set; }
    public bool IsPlaying { get; private set; }
    
    private float _timer;
    private int _currentFrame;
    
    public Rectangle CurrentFrame => Frames[_currentFrame];
    public bool IsFinished => !IsLooping && _currentFrame >= Frames.Count - 1;
    
    public Animation(List<Rectangle> frames, float frameDuration = 0.1f, bool isLooping = true)
    {
        Frames = frames;
        FrameDuration = frameDuration;
        IsLooping = isLooping;
        _currentFrame = 0;
        _timer = 0f;
        IsPlaying = true;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!IsPlaying || Frames.Count <= 1) return;
        
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_timer >= FrameDuration)
        {
            _timer -= FrameDuration;
            _currentFrame++;
            
            if (_currentFrame >= Frames.Count)
            {
                if (IsLooping)
                    _currentFrame = 0;
                else
                {
                    _currentFrame = Frames.Count - 1;
                    IsPlaying = false;
                }
            }
        }
    }
    
    public void Play()
    {
        IsPlaying = true;
    }
    
    public void Pause()
    {
        IsPlaying = false;
    }
    
    public void Reset()
    {
        _currentFrame = 0;
        _timer = 0f;
        IsPlaying = true;
    }
}

public class AnimationController
{
    private Dictionary<string, Animation> _animations;
    private Animation _currentAnimation;
    private string _currentAnimationName;
    
    public Animation CurrentAnimation => _currentAnimation;
    public string CurrentAnimationName => _currentAnimationName;
    
    public AnimationController()
    {
        _animations = new Dictionary<string, Animation>();
    }
    
    public void AddAnimation(string name, Animation animation)
    {
        _animations[name] = animation;
    }
    
    public void Play(string animationName)
    {
        if (_currentAnimationName == animationName) return;
        
        if (_animations.TryGetValue(animationName, out Animation animation))
        {
            _currentAnimation = animation;
            _currentAnimationName = animationName;
            _currentAnimation.Reset();
        }
    }
    
    public void Update(GameTime gameTime)
    {
        _currentAnimation?.Update(gameTime);
    }
    
    public Rectangle GetCurrentFrame()
    {
        return _currentAnimation?.CurrentFrame ?? Rectangle.Empty;
    }
}