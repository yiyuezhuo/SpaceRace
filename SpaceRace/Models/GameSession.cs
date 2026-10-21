using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace SpaceRace.Models;

public class GameSession
{
    public GameState State{get;set;} = new();

    public void NewGame()
    {
        State = new GameState();
    }
}