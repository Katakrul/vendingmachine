using Godot;
using System;

public partial class InteractingPersonTriggerZone : Area2D
{
	
	[Signal]
	public delegate void TriggerDialogueEventHandler(InteractingPerson area);
	
	private InteractingPerson currentPerson = null;

 	public override void _PhysicsProcess(double delta)
	{
		// Get all overlapping areas
		var overlapping = GetOverlappingAreas();

		if (overlapping.Count > 0)
		{
			foreach (Area2D area in overlapping)
			{
				//GD.Print("Overlapping with: " + area.Name);
			}
		}
	}
	//
	//public override void _Ready()
	//{
		//AreaEntered += OnIPTriggerZoneAreaEntered;
		//AreaExited += OnIPTriggerZoneAreaExited;
	//}
	
	private void OnIPTriggerZoneAreaEntered(Area2D area)
	{
		if (currentPerson != null)
		{
			return;
		}
		
		if (area is InteractingPerson person)
		{
			EmitSignal(SignalName.TriggerDialogue, person);
			currentPerson = person;
		}
	}
	
	private void OnIPTriggerZoneAreaExited(Area2D area)
	{
		if (area is InteractingPerson person && person == currentPerson)
		{
			currentPerson = null;
		}

	}
	
}
