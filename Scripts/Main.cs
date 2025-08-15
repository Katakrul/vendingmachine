using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene BackgroundPersonScene = GD.Load<PackedScene>("res://Scenes/BackgroundPerson.tscn");
	public PackedScene InteractingPersonScene = GD.Load<PackedScene>("res://Scenes/InteractingPerson.tscn");
	public PackedScene OrderScene = GD.Load<PackedScene>("res://Scenes/Order.tscn");
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	
	private float[] _startingPositions = [0f, 0.5f];
	private int _availableOrderTasks = 1;
	private int _completedOrderTasks = 0;
	private InteractingPerson _currentInteractingPerson;
	private Order _currentOrder;
	
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
			var personHitbox = person.GetNode<CollisionShape2D>("CollisionShape2D");
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
				personHitbox.Position = new Vector2(
					x: personHitbox.Position.X + GD.RandRange(-50, 0),
					y: personHitbox.Position.Y
				);
			} else {
				person.startPos = "right";
				personHitbox.Position = new Vector2(
					x: personHitbox.Position.X + GD.RandRange(0, 50),
					y: personHitbox.Position.Y
				);
			}
			AddChild(person);
		}
	}
	
	private async void GiveOrder(InteractingPerson body)
	{
		body.Stop = true;
		_currentInteractingPerson = body;
		var order = (Order)OrderScene.Instantiate();
		var orderContainer = order.GetNode<CenterContainer>("OrderTextContainer");
		var orderText = order.GetNode<Label>("OrderTextContainer/OrderText");
		var orderSprite = order.GetNode<TextureRect>("OrderSprite");
		orderText.Text = "fasdfdasgasdgasfgs";
		_currentOrder = order;
		AddChild(order);
		
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		orderSprite.Position = new Vector2(
			x: orderContainer.Position.X - 50,
			y: (orderContainer.Position.Y + orderContainer.Size.Y) / -2 + 10
		);
		OrderConfirmed(_currentOrder);
	}

	public void OrderConfirmed(Order order)
	{
		PackedScene PlaceholderFeatureScene = GD.Load<PackedScene>("res://Scenes/PlaceholderFeature.tscn");
		var feature = (PlaceholderFeature)PlaceholderFeatureScene.Instantiate();
		feature.CurrentOrder = order;
		feature.OrderTaskCompleted += OrderTaskCompleted;
		AddChild(feature);
	}
	
	private void OrderTaskCompleted(PlaceholderFeature feature, Order order)
	{
		_completedOrderTasks++;
		if (_availableOrderTasks == _completedOrderTasks) {
			_currentInteractingPerson.Stop = false;
			feature.QueueFree();
			order.QueueFree();
			_completedOrderTasks = 0;
		} else {
			_completedOrderTasks++;
		}
	}

}
