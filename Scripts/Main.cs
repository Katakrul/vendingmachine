using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene BackgroundPersonScene = GD.Load<PackedScene>("res://Scenes/BackgroundPerson.tscn");
	public PackedScene InteractingPersonScene = GD.Load<PackedScene>("res://Scenes/InteractingPerson.tscn");
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	
	private float[] _startingPositions = [0f, 0.5f];
	
	public override void _Ready()
	{
		StartGame();
	}
	
	public void StartGame()
	{
		var bpTimer = GetNode<Timer>("BackgroundPersonTimer");
		var ipTimer = GetNode<Timer>("InteractingPersonTimer");
		
		bpTimer.WaitTime = GD.RandRange(1, 5);
		ipTimer.WaitTime = GD.RandRange(1, 5);
		
		bpTimer.Start();
		ipTimer.Start();
		
	}
	
	private void OnBackgroundPersonTimerTimeout()
	{
		
		AddPerson("BackgroundPerson");
		var bpTimer = GetNode<Timer>("BackgroundPersonTimer");
		bpTimer.WaitTime = GD.RandRange(1, 5);
		
	}
	
	private void OnInteractingPersonTimerTimeout()
	{
		AddPerson("InteractingPerson");
		var ipTimer = GetNode<Timer>("InteractingPersonTimer");
		ipTimer.WaitTime = GD.RandRange(1, 5);
	}
	
	private void AddPerson(string personType)
	{
		if (personType == "BackgroundPerson") {
			var person = BackgroundPersonScene.Instantiate<BackgroundPerson>();
			var personPosition = GetNode<PathFollow2D>("BPPath/BPPathFollow");
			personPosition.ProgressRatio = _startingPositions[GD.Randi() % _startingPositions.Length];
			person.Speed = _rng.RandfRange(0.01f, 0.1f);
			person.Position = personPosition.Position;
			person.Position = new Vector2(
				x: person.Position.X,
				y: person.Position.Y + GD.RandRange(-20, 20)
			);
			if (person.Position.X == 0) {
				person.startPos = "left";
			} else {
				person.startPos = "right";
			}
			AddChild(person);
		} else if (personType == "InteractingPerson") {
			var person = InteractingPersonScene.Instantiate<InteractingPerson>();
			var personPosition = GetNode<PathFollow2D>("IPPath/IPPathFollow");
			personPosition.ProgressRatio = _startingPositions[GD.Randi() % _startingPositions.Length];
			person.Speed = _rng.RandfRange(0.01f, 0.1f);
			person.Position = personPosition.Position;
			person.Position = new Vector2(
				x: person.Position.X,
				y: person.Position.Y + GD.RandRange(-10, 10)
			);
			if (person.Position.X == 0) {
				person.startPos = "left";
			} else {
				person.startPos = "right";
			}
			AddChild(person);
		}
	}
	
	private void GiveOrder(InteractingPerson body)
	{
		body.Stop = true;
	}
	
}
