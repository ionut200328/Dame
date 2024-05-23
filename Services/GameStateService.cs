using System;
using System.Collections.Generic;
using Dame.MVVM.Model; // Ensure your models are referenced correctly
using System.ComponentModel;

public class GameStateService : INotifyPropertyChanged
{
    private List<Piece> _pieces = new List<Piece>();

    public event PropertyChangedEventHandler PropertyChanged;

    public List<Piece> Pieces
    {
        get => _pieces;
        set
        {
            _pieces = value;
            OnPropertyChanged(nameof(Pieces));
        }
    }

    public GameStateService()
    {
        // Initialize your game state here
    }

    // Example operation: Resetting the game
    public void ResetGame()
    {
        // Logic to reset the game state
        Pieces = new List<Piece>(); // Or however you choose to reset the state
        // Add more initialization as needed

        OnPropertyChanged(nameof(Pieces)); // Notify observers of the change
    }

    // Add more operations as needed...

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
