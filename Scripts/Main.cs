using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene BackgroundPersonScene = GD.Load<PackedScene>("res://Scenes/BackgroundPerson.tscn");
	public PackedScene InteractingPersonScene = GD.Load<PackedScene>("res://Scenes/InteractingPerson.tscn");
	public PackedScene OrderScene = GD.Load<PackedScene>("res://Scenes/Order.tscn");
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	private Timer _bpTimer;
	private Timer _ipTimer;
	
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
		_bpTimer = GetNode<Timer>("BackgroundPersonTimer");
		_ipTimer = GetNode<Timer>("InteractingPersonTimer");
		
		_bpTimer.WaitTime = GD.RandRange(1, 5);
		_ipTimer.WaitTime = GD.RandRange(1, 5);
		
		_bpTimer.Start();
		_ipTimer.Start();
		GD.Print("Background Zindex : " + GetNode<TextureRect>("UIBackground").ZIndex);
	}
	
	private void OnBackgroundPersonTimerTimeout()
	{
		var person = BackgroundPersonScene.Instantiate<BackgroundPerson>();
		person.front = Convert.ToBoolean(GD.RandRange(0,1));
		
		if (!person.front) {
			var personPosition = GetNode<PathFollow2D>("BehindPath/BehindPathFollow");
			personPosition.ProgressRatio = _startingPositions[GD.Randi() % _startingPositions.Length];
			person.Speed = 25;
			person.Position = personPosition.Position;
			person.Position = new Vector2(
				x: person.Position.X,
				y: person.Position.Y + GD.RandRange(-20, 20)
			);
		} else {
			var personPosition = GetNode<PathFollow2D>("FrontPath/FrontPathFollow");
			var personSprite = person.GetNode<Sprite2D>("BPSprite");
			personPosition.ProgressRatio = _startingPositions[GD.Randi() % _startingPositions.Length];
			person.Speed = 50;
			person.Position = personPosition.Position;
			person.Position = new Vector2(
				x: person.Position.X,
				y: person.Position.Y + GD.RandRange(-10, 10)
			);
			personSprite.Scale = new Vector2(
				2.5f,
				2.5f
			);
		}
		if (person.Position.X == 0) {
			person.startPos = "left";
		} else {
			person.startPos = "right";
		}
		//person.ZIndex = 1;
		//person.Modulate = new Color("red");
		GD.Print("Background Person ZIndex : " + person.ZIndex);
		AddChild(person);
		
		
		_bpTimer.WaitTime = GD.RandRange(1, 5);
		
	}
	
	private void OnInteractingPersonTimerTimeout()
	{
		var person = InteractingPersonScene.Instantiate<InteractingPerson>();
		var personHitbox = person.GetNode<CollisionShape2D>("CollisionShape2D");
		var personPosition = GetNode<PathFollow2D>("FrontPath/FrontPathFollow");
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
		person.ZIndex = 1;
		GD.Print("Interacting Person ZIndex : " + person.ZIndex);
		AddChild(person);
		
		_ipTimer.Stop();
	}
	
	//private void AddPerson(string personType)
	//{
		//if (personType == "BackgroundPerson") {
//
		//} else if (personType == "InteractingPerson") {
			//
		//}
	//}
	
	private async void GiveOrder(InteractingPerson person)
	{
		person.Stop = true;
		person.ZIndex = 1;
		_currentInteractingPerson = person;
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
			_ipTimer.Start();
		} else {
			_completedOrderTasks++;
		}
	}

}
