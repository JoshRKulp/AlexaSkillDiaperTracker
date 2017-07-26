using AlexaDiaperSkill.Models;
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
    public AlexaResponse Hello(AlexaRequest request)
    {
      using (var ctx = new Data.JoshRKulpEntities())
      {
        if(ctx.Users.Where(w => w.AmazonId == request.Session.User.UserId).Count() < 1)
        {
          var user = new Data.User();
          user.Id = ctx.Users.Count() + 1;
          user.AmazonId = request.Session.User.UserId;
          ctx.Users.Add(user);

          ctx.SaveChanges();

          return new AlexaResponse("User Added To Database User Number is " + ctx.Users.Count().ToString(), true);
        }
      }
      if (request.Request.Intent.Name == "HelloWorldIntent")
      {
        return new AlexaResponse("Hello World", true);
      }
      return new AlexaResponse("What you need, YO",true);
      
      #region Return Dynamic JSON
      //return new
      //{
      //  version = "1.0",
      //  sessionAttribute = new { },
      //  response = new
      //  {
      //    outputSpeech = new
      //    {
      //      type = "PlainText",
      //      text = "Hello World"
      //    },
      //    card = new
      //    {
      //      type = "Simple",
      //      title = "JoshRKulp Hello World",
      //      content = "Hello World"
      //    },
      //    shouldEndSession = true
      //  }
      //};
      #endregion

    }
  }
}