using Godot;
using System;

public partial class PlaceholderFeature : CanvasLayer
{
	[Signal]
	public delegate void OrderTaskCompletedEventHandler(PlaceholderFeature feature, Order order);
	[Export]
	public Order CurrentOrder;

	public void OnPlaceholderFeatureButtonPressed()
	{
		EmitSignal(SignalName.OrderTaskCompleted, this, CurrentOrder);
	}
	
	
}
