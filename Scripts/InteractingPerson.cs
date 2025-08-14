using Godot;
using System;

public partial class InteractingPerson : Area2D
{
	
	[Export]
	public float Speed { get; set; }
	[Export]
	public bool Stop = false;
	public string startPos;	
	public Vector2 ScreenSize;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}
	
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		
		if (Stop == false) {
			if (startPos == "left") {
				velocity.X += GD.RandRange(50, 100);
			} else {
				velocity.X -= GD.RandRange(50, 100);
			}
		} else if (Stop == true) {
			velocity.X = 0;
		}
		
		Position += velocity * (float)delta;
		
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
