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
            //HighScore tmp = null;
            //foreach (HighScore highScore in highScores)
            //{
            //    if (paremHighscore.Score > highScore.Score)
            //    {
            //        tmp = highScore;
            //        break;
            //    }
            //}
            //if (tmp != null)
            //{
            //    highScores.Add(paremHighscore);
            //} 
            highScores.Add(paremHighscore);
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
