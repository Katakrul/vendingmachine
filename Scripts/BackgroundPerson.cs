using Godot;
using System;

public partial class BackgroundPerson : Node2D
{
	
	[Export]
	public float Speed { get; set; }
	public string startPos;
	public bool front;
	
	public Vector2 ScreenSize;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}
	
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		
		if (startPos == "left") {
			velocity.X += (float)GD.RandRange(Speed, Speed*2);
		} else {
			velocity.X -= (float)GD.RandRange(Speed, Speed*2);
		}
		
		Position += velocity * (float)delta;
		
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
	
}
