using Godot;
using System;

public partial class Global : Node
{
    public static PlayerScript player { get; set; }
    public static Global Instance { get; private set; }
    public override void _Ready()
	{
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
