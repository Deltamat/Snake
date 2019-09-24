using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTful_Webservice.Models
{
    public class HighScore
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public int Score { get; set; }

        public HighScore(string ip, int score)
        {
            Ip = ip;
            Score = score;
        }
    }
}