using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace First3DGamebyme.Entities;

public class BasicEnemy : Enemy
{
    public BasicEnemy(Vector2 position) : base(position, 50f, 150f)
    {
        _damage = 10f;
        _attackRange = 60f;
        _detectionRange = 400f;
    }
    
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _texture = new Texture2D(graphicsDevice, 24, 24);
        Color[] data = new Color[24 * 24];
        
        for (int y = 0; y < 24; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                float distance = Vector2.Distance(new Vector2(12, 12), new Vector2(x, y));
                if (distance <= 12)
                {
                    data[y * 24 + x] = Color.DarkGreen;
                }
                else
                {
                    data[y * 24 + x] = Color.Transparent;
                }
            }
        }
        
        _texture.SetData(data);
        _origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    }
    
    protected override void UpdateBehavior(float deltaTime)
    {
        base.UpdateBehavior(deltaTime);
        
        if (_state == EnemyState.Hurt)
        {
            TintColor = Color.Red;
            _state = EnemyState.Chase;
        }
        else
        {
            TintColor = Color.White;
        }
    }
}