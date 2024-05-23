using Dame.Core;
using Dame.MVVM.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Dame.Services;
using Newtonsoft.Json;
using System;
using System.Windows;

namespace Dame.MVVM.ViewModel
{
    public class GameViewModel : Core.ViewModel
    {
        private readonly GameStateService _gameStateService;
        private readonly IDialogService _dialogService;
        private ChessCellViewModel _selectedCell;
        private List<Piece> _pieces;
        private PieceColor _turn;
        private bool _ended = false;
        private int _whiteCount;
        private int _blackCount;
        public ObservableCollection<ChessCellViewModel> ChessCells { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand EndTurn { get;}

        private GameService _gameService = new GameService();
        private StatisticsService _statisticsService = new StatisticsService();

        public List<Piece> Pieces
        {
            get => _pieces;
            set
            {
                _pieces = value;
                OnPropertyChanged();
            }
        }
        public PieceColor Turn { 
            get => _turn;
            set
            {
                _turn = value;
                OnPropertyChanged();
            }
        }

        public bool Ended
        {
            get => _ended;
            set
            {
                _ended = value;
                OnPropertyChanged();
            }
        }
        public int WhiteCount
        {
            get => _whiteCount;
            set
            {
                _whiteCount = value;
                OnPropertyChanged();
            }
        }
        public int BlackCount
        {
            get => _blackCount;
            set
            {
                _blackCount = value;
                OnPropertyChanged();
            }
        }
        private bool MultipleJumps { get;  set; }
        private bool AllowMultipleJumps { get; set; }
        private bool Moved { get; set; }

        public GameViewModel(GameStateService gameStateService, IDialogService dialogService)
        {
            _gameStateService = gameStateService;
            _dialogService = dialogService;
            ChessCells = new ObservableCollection<ChessCellViewModel>();
            GenerateChessGrid();
            SaveCommand = new RelayCommand(_ => SaveGame(), _ => CanSaveGame());
            LoadCommand = new RelayCommand(_ => LoadGame(), _ => CanLoadGame());
            EndTurn = new RelayCommand(_ => ChangeTurn(), _ => CanChangeTurn());
        }
        private void GenerateChessGrid()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var cellColor = (row + col) % 2 == 0 ? Colors.LightYellow : Colors.Brown;
                    ChessCells.Add(new ChessCellViewModel(cellColor, this));
                }
            }
        }
        public void NewGame()
        {
            ClearSelectionsAndTargets();
            //make Pices and place them on the board
            Pieces = PieceViewModel.GeneratePieces();
            Turn = PieceColor.Black;
            //clear the board
            ClearBoard();

            foreach (var piece in Pieces)
            {
                ChessCells[piece.Row * 8 + piece.Col].Piece = piece;
            }

            OnPropertyChanged(nameof(ChessCells));
            OnPropertyChanged(nameof(Pieces));
            OnPropertyChanged(nameof(Turn));

            AllowMultipleJumps = _dialogService.ShowConfirmation("Game settings", "Allow multiple jumps?") as bool? ?? false;
            UpdatePieceCounts();
        }

        public void MovePieceTo(ChessCellViewModel targetCell)
        {
            // Early exit if no piece is selected or trying to move the wrong piece during a multi-jump
            if (_selectedCell == null || (MultipleJumps && targetCell.Piece != _selectedCell.Piece)) return;

            Piece movingPiece = _selectedCell.Piece;
            Console.WriteLine(targetCell.IsJumpTarget);
            Console.WriteLine(targetCell.IsMoveTarget);

            // Calculate the new row and column based on the index of the target cell
            int newRow = ChessCells.IndexOf(targetCell) / 8;
            int newCol = ChessCells.IndexOf(targetCell) % 8;

            if (targetCell.IsJumpTarget)
            {
                // If it's a jump, calculate and remove the jumped piece

                // Calculate the position of the jumped piece
                int jumpedRow = (_selectedCell.Piece.Row + newRow) / 2;
                int jumpedCol = (_selectedCell.Piece.Col + newCol) / 2;

                // Find and remove the jumped piece
                ChessCellViewModel jumpedCell = ChessCells[jumpedRow * 8 + jumpedCol];
                jumpedCell.Piece = null;

                // Correctly set the moving piece's new position to the target cell, not the jumped cell
                movingPiece.Row = newRow; // Corrected to newRow, which is the target cell's row
                movingPiece.Col = newCol; // Corrected to newCol, which is the target cell's column
                Console.WriteLine("Jumped");

                UpdatePieceCounts();

                Moved = true;
            }
            else if (targetCell.IsMoveTarget)
            {
                // Handle regular move
                movingPiece.Row = newRow;
                movingPiece.Col = newCol;
                Console.WriteLine("Moved");

                Moved = true;
            }

            // Update the target cell with the moving piece and clear the original cell
            targetCell.Piece = movingPiece;

            _selectedCell.Piece = null;

            // Promote the piece if it reaches the opposite end of the board
            if (!movingPiece.IsQueen) PromotePieceIfNeeded(movingPiece);

            // Check for additional jumps
            if (targetCell.IsJumpTarget && AdditionalJumpsPossible(movingPiece, newRow, newCol) && AllowMultipleJumps)
            {
                // If more jumps are possible, reselect the piece and show available jumps
                MultipleJumps = true;
                _selectedCell = targetCell;
                SelectPiece(targetCell);
            }
            else
            {
                // No further actions possible, so change the turn
                MultipleJumps = false;
                UpdatePiecesList();
                ClearSelectionsAndTargets();
                ChangeTurn(); // Change turn after the move or jump series is complete
            }
            CheckWinCondition();
        }
        private bool AdditionalJumpsPossible(Piece piece, int currentRow, int currentCol)
        {
            // Use similar logic as in SelectPiece but specifically for detecting jumps
            // Assuming you have a method similar to JumpIsPossible but it takes Piece as a parameter
            var potentialDirections = piece.IsQueen ? new[] { -2, 2 } : (piece.Color == PieceColor.White ? new[] { 2 } : new[] { -2 });
            foreach (var direction in potentialDirections)
            {
                int targetRow = currentRow + direction;
                foreach (int deltaCol in new[] { -2, 2 })
                {
                    int targetCol = currentCol + deltaCol;
                    if (targetRow >= 0 && targetRow < 8 && targetCol >= 0 && targetCol < 8)
                    {
                        // Assuming JumpIsPossible method can also work with Piece parameters
                        
                        //convert piece to cell
                        var targetCell = ChessCells[piece.Row * 8 + piece.Col];
                        if (JumpIsPossible(targetCell, targetRow, targetCol))
                        {
                            return true; // If any jump is possible, return true
                        }
                    }
                }
            }
            return false; // If no jumps are possible, return false
        }

        private void PromotePieceIfNeeded(Piece piece)
        {
            if (piece.Color == PieceColor.White && piece.Row == 7 || piece.Color == PieceColor.Black && piece.Row == 0)
            {
                piece.IsQueen = true;
            }
        }




        private void ClearSelectionsAndTargets()
        {
            foreach (var cell in ChessCells)
            {
                cell.IsSelected = false;
                cell.IsMoveTarget = false;
                cell.IsJumpTarget = false;
            }
            _selectedCell = null;
        }
        public void SelectPiece(ChessCellViewModel cell)
        {
            // Check if the selected cell has a piece and if the piece matches the current turn's color
            // Ignore selection if it's not the player's turn or if in a multi-jump sequence with a different piece
            if (cell.Piece == null || cell.Piece.Color != Turn || (MultipleJumps && _selectedCell != null && cell != _selectedCell)) return; // Ignore the selection if there's no piece or it's not the player's turn
            
            // Clear previous selections and move targets
            ClearSelectionsAndTargets();

            _selectedCell = cell;
            cell.IsSelected = true;
            

            // Calculate and mark possible moves for queens (forward and backward)
            var directions = cell.Piece.IsQueen ? new[] { 1, -1 } : (cell.Piece.Color == PieceColor.White ? new[] { 1 } : new[] { -1 });

            foreach (var direction in directions)
            {
                int targetRow = cell.Piece.Row + direction;
                if (!MultipleJumps)
                {
                    foreach (int newCol in new[] { cell.Piece.Col - 1, cell.Piece.Col + 1 })
                    {
                        if (targetRow >= 0 && targetRow < 8 && newCol >= 0 && newCol < 8)
                        {
                            var targetCell = ChessCells[targetRow * 8 + newCol];
                            if (targetCell.Piece == null) // Only mark empty cells as move targets
                            {
                                targetCell.IsMoveTarget = true;
                            }
                        }
                    }
                }
                // Calculate and mark possible jumps for queens (forward and backward)
                targetRow = cell.Piece.Row + (2 * direction);
                foreach (int newCol in new[] { cell.Piece.Col - 2, cell.Piece.Col + 2 })
                {
                    if (targetRow >= 0 && targetRow < 8 && newCol >= 0 && newCol < 8)
                    {
                        var targetCell = ChessCells[targetRow * 8 + newCol];
                        if (JumpIsPossible(cell, targetRow, newCol))
                        {
                            targetCell.IsJumpTarget = true;
                        }
                    }
                }
            }
            MultipleJumps = false;
            
        }


        private bool JumpIsPossible(ChessCellViewModel currentCell, int targetRow, int targetCol)
        {
            // Calculate the row and column of the cell being jumped over
            int midRow = (currentCell.Piece.Row + targetRow) / 2;
            int midCol = (currentCell.Piece.Col + targetCol) / 2;

            // Check if the cell being jumped over is within bounds
            if (!IsCellWithinBounds(midRow, midCol))
            {
                return false;
            }

            // Get the cell being jumped over
            var midCell = ChessCells[midRow * 8 + midCol];

            // Check if the cell being jumped over contains an opponent's piece
            if (midCell.Piece == null || midCell.Piece.Color == currentCell.Piece.Color)
            {
                return false;
            }

            // Get the target cell
            var targetCell = ChessCells[targetRow * 8 + targetCol];

            // The jump is possible if the target cell is empty
            return targetCell.Piece == null;
        }


        // Utility method to check if a cell is within the bounds of the board.
        private bool IsCellWithinBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }



        public void SelectMoveTarget(ChessCellViewModel cell)
        {
            //foreach (var p in Pieces)
            //{
            //    Console.WriteLine(p);
            //    Console.WriteLine(p.IsQueen);
            //}
            if (cell.IsMoveTarget || cell.IsJumpTarget)
            {
                MovePieceTo(cell);
            }
            _gameStateService.Pieces = Pieces;
        }

        private void SaveGame()
        {
            _gameService.SaveGame(Pieces, Turn, AllowMultipleJumps, Ended);
        }

        private bool CanSaveGame()
        {
            // You can add more conditions here depending on game state
            return true;
        }

        private void LoadGame()
        {
            ClearBoard();
            ClearSelectionsAndTargets();


            var loadedData = _gameService.LoadGame();
            Pieces = loadedData.Item1.ToList();
            Turn = loadedData.Item2;
            AllowMultipleJumps = loadedData.Item3;
            Ended = loadedData.Item4;
            _gameStateService.Pieces = Pieces; // Assuming LoadGame returns IEnumerable<Piece>
            // Update ChessCells based on _gameStateService.Pieces
            foreach (var piece in Pieces)
            {
                var cell = ChessCells[piece.Row * 8 + piece.Col];
                cell.Piece = new Piece(piece.Type, piece.Color, piece.Row, piece.Col, piece.IsQueen);
            }
            OnPropertyChanged(nameof(ChessCells));
            OnPropertyChanged(nameof(Pieces));
            OnPropertyChanged(nameof(Turn));

            CheckWinCondition();
            UpdatePieceCounts();
        }

        private bool CanLoadGame()
        {
            // This method can check for the existence of the save file or other conditions
            // For simplicity, always return true or implement your own logic
            return true;
        }

        private void ClearBoard()
        {
            foreach (var cell in ChessCells)
            {
                cell.Piece = null;
            }
        }
        private void ChangeTurn()
        {
            Turn = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;
            ClearSelectionsAndTargets();
            Moved = false;
        }
        private bool CanChangeTurn()
        {
            return Moved;
        }


        private void UpdatePiecesList()
        {
            _pieces = ChessCells.Where(cell => cell.Piece != null)
                                .Select(cell => cell.Piece)
                                .ToList();
        }

        public override void SetParameter(object parameter)
        {
            base.SetParameter(parameter);

            if(parameter is string command)
            {
                if (command == "NewGame")
                {
                    NewGame();
                }
                else if (command == "SaveGame")
                {
                    SaveGame();
                }
                else if (command == "LoadGame")
                {
                    LoadGame();
                }
            }
            
        }

        private void CheckWinCondition()
        {
            // Count pieces for each player
            var whitePiecesCount = Pieces.Count(p => p.Color == PieceColor.White);
            var blackPiecesCount = Pieces.Count(p => p.Color == PieceColor.Black);


            if (whitePiecesCount == 0)
            {
                OnGameEnded(PieceColor.Black); // Black wins
            }
            else if (blackPiecesCount == 0)
            {
                OnGameEnded(PieceColor.White); // White wins
            }
        }

        private void OnGameEnded(PieceColor winner)
        {
            // Example: Show a message box or update the UI with the winner information
            MessageBox.Show($"{winner} wins!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            if (!Ended)
            {
                _statisticsService.SaveStatistics(winner, Pieces);
                _gameService.SaveGame(Pieces, Turn, AllowMultipleJumps, true);
            }

            // Additional logic to handle the end of the game, such as disabling moves
        }

        private void UpdatePieceCounts()
        {
            WhiteCount = Pieces.Count(p => p.Color == PieceColor.White);
            BlackCount = Pieces.Count(p => p.Color == PieceColor.Black);
        }

    }

}
