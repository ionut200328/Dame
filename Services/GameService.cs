using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dame.MVVM.Model;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Dame.Services
{
    public class GameData
    {
        public IEnumerable<Piece> Pieces { get; set; }
        public PieceColor Turn { get; set; }
        public bool AllowMultipleJumps { get; set; }
        public bool Ended { get; set; }

    }

    public class GameService
    {
        public void SaveGame(IEnumerable<Piece> pieces, PieceColor turn, bool allowMultipleJumps, bool ended)
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = "E:\\AN II Sem 2\\MVP\\Dame\\Games\\",
                Title = "Save Game"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var gameData = new GameData
                {
                    Pieces = pieces,
                    Turn = turn,
                    AllowMultipleJumps = allowMultipleJumps,
                    Ended = ended
                };

                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new StringEnumConverter() },
                    Formatting = Formatting.Indented
                };

                string json = JsonConvert.SerializeObject(gameData, settings);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }
        public (IEnumerable<Piece>, PieceColor, bool, bool) LoadGame()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = "E:\\AN II Sem 2\\MVP\\Dame\\Games\\",
                Title = "Load Game"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                };
                var gameData = JsonConvert.DeserializeObject<GameData>(json, settings);

                return (gameData?.Pieces ?? new List<Piece>(), gameData?.Turn ?? default(PieceColor), gameData?.AllowMultipleJumps ?? false, gameData?.Ended ?? false);
            }
            return (new List<Piece>(), default(PieceColor), false, false); // Return an empty list and default turn if no file is selected
        }


    }
}
