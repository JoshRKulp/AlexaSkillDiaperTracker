using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AlexaDiaperSkill.Controllers
{
  public class AlexaController : ApiController
  {
    [HttpPost, Route("api/alexa/demo")]
    public dynamic Hello(dynamic request)
    {
      return new
      {
        version = "1.0",
        sessionAttribute = new { },
        response = new
        {
          outputSpeech = new
          {
            type = "PlainText",
            text = "Hello World"
          },
          card = new
          {
            type = "Simple",
            title = "JoshRKulp Hello World",
            content = "Hello World"
          },
          shouldEndSession = true
        }
      };
    }
  }
}