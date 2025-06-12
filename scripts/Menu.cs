using Godot;
using System;

public partial class Menu : Node
{	
	Button buttonStart;
	Button buttonCredits;
	Button buttonDocumentation;
	Button buttonExit;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buttonStart = GetNode<Button>("VBoxContainer/ButtonStart");
		buttonCredits = GetNode<Button>("VBoxContainer/ButtonCredits");
		buttonDocumentation = GetNode<Button>("VBoxContainer/ButtonDocumentation");
		buttonExit = GetNode<Button>("VBoxContainer/ButtonExit");

		buttonStart.Pressed += OnButtonStartPressed;
		buttonCredits.Pressed += OnButtonCreditsPressed;
		buttonDocumentation.Pressed += OnButtonDocumentationPressed;
		buttonExit.Pressed += OnButtonExitPressed;
	}
	private void OnButtonStartPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
	}
	private void OnButtonCreditsPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/creditos.tscn");
	}
	private void OnButtonDocumentationPressed()
	{
		OS.ShellOpen("https://github.com/Javieraa05/Pixel-Wall-E-Compiler");
	}
	private void OnButtonExitPressed()
	{
		GetTree().Quit();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
