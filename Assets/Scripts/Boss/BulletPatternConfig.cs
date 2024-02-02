using UnityEngine;

public struct BulletPatternConfig
{
    public int columnNumber;
    public float baseAngle;
    public float speed;
    public Color color;
    public float lifetime;
    public float firerate;
    public float size;
    public bool shouldSpin;
    public float spinSpeed;
    public float direction;

    public BulletPatternConfig(int columnNumber, float baseAngle, float speed, Color color, float lifetime,
        float firerate, float size, bool shouldSpin, float spinSpeed, float direction)
    {
        this.columnNumber = columnNumber;
        this.baseAngle = baseAngle;
        this.speed = speed;
        this.color = color;
        this.lifetime = lifetime;
        this.firerate = firerate;
        this.size = size;
        this.shouldSpin = shouldSpin;
        this.spinSpeed = spinSpeed;
        this.direction = direction;
    }
}