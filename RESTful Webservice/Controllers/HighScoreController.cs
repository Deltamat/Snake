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
       // Dictionary<string, string> highScore = new Dictionary<string, string>();
        List<HighScore> highScores = new List<HighScore>();

        // GET: api/HighScore
        public IEnumerable<HighScore> Get()
        {
            //return new string[] = {  }
            return highScores;
        }

        // GET: api/HighScore/5
        public int Get(string ip)
        {
            //HighScore hs = new HighScore(ip, 0);
            //return highScores.Find(hs).ToString();
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
        public void Post([FromBody]string value)
        {
            string jsonString = JsonConvert.SerializeObject(value);
            //highScores.Add(new HighScore(name, value));
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
