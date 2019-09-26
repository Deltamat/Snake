using RESTful_Webservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace RESTful_Webservice.Controllers
{
    public class HighScoreController : ApiController
    {
        static List<HighScore> highScores = new List<HighScore>();

        // GET: api/HighScore
        public IEnumerable<HighScore> Get()
        {
            return highScores;
        }

        // GET: api/HighScore/5
        public int Get(string ip)
        {
            foreach (var score in highScores)
            {
                if (score.Ip == ip)
                {
                    return score.Score;
                }
            }
            return 0;
        }

        // POST: api/HighScore
        public void Post([FromBody]HighScore paremHighscore)
        {
            if (paremHighscore.Score > highScores[0].Score)
            {
                highScores.Insert(0, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[1].Score)
            {
                highScores.Insert(1, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[2].Score)
            {
                highScores.Insert(2, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[3].Score)
            {
                highScores.Insert(3, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[4].Score)
            {
                highScores.Insert(4, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[5].Score)
            {
                highScores.Insert(5, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[6].Score)
            {
                highScores.Insert(6, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[7].Score)
            {
                highScores.Insert(7, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[8].Score)
            {
                highScores.Insert(8, paremHighscore);
            }
            else if (paremHighscore.Score > highScores[9].Score)
            {
                highScores.Insert(9, paremHighscore);
            }
            try
            {
                highScores.RemoveAt(10);
            }
            catch (Exception)
            {

                //yeet
            }
            //highScores.Add(paremHighscore);
        }


        // PUT: api/HighScore/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/HighScore/5
        public void Delete(int id)
        {

        }
    }
}
