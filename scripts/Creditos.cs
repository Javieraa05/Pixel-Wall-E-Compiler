using Godot;
using System;

public partial class Creditos : CanvasLayer
{
	private Button BotonAtras;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BotonAtras = GetNode<Button>("Atras");
		BotonAtras.Pressed += BotonAtras_Pressed;
	}
	private void BotonAtras_Pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
