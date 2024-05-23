using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Dame.MVVM.Model;

namespace Dame.Services
{
    public class Statistics
    {
        public int BlackWins { get; set; }
        public int WhiteWins { get; set; }

    }

    public class StatisticsService
    {
        private readonly string _statisticsFile = "E:\\AN II Sem 2\\MVP\\Dame\\Games\\Statitics\\Statitics.json";
        public int BlackWins { get; private set; }
        public int WhiteWins { get; private set; }



        public void SaveStatistics(PieceColor winner, List<Piece> pieces)
        {
            // Save statistics to a file
            
            LoadStatistics();
            string json = JsonConvert.SerializeObject(new Statistics
            {
                BlackWins = winner == PieceColor.Black ? BlackWins + 1 : BlackWins,
                WhiteWins = winner == PieceColor.White ? WhiteWins + 1 : WhiteWins
            });
            File.WriteAllText(_statisticsFile, json);
        }
        public void LoadStatistics()
        {
            // Load statistics from a file
            if (File.Exists(_statisticsFile))
            {
                //check if the file is empty
                if (new FileInfo(_statisticsFile).Length <= 10)
                {
                    BlackWins = 0;
                    WhiteWins = 0;
                }
                else
                {
                    string json = File.ReadAllText(_statisticsFile);
                    var statistics = JsonConvert.DeserializeObject<Statistics>(json);
                    BlackWins = statistics.BlackWins;
                    WhiteWins = statistics.WhiteWins;
                }
            }
        }
    }
}
