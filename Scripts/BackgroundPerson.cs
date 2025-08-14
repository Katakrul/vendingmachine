using Godot;
using System;

public partial class BackgroundPerson : Node2D
{
	
	[Export]
	public float Speed { get; set; }
	public string startPos;
	
	public Vector2 ScreenSize;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}
	
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		
		if (startPos == "left") {
			velocity.X += (float)GD.RandRange(50/2.5, 100/2.5);
		} else {
			velocity.X -= (float)GD.RandRange(50/2.5, 100/2.5);
		}
		
		Position += velocity * (float)delta;
		
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
	
}
