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
    const int diaperCount = 25;

    [HttpPost, Route("api/alexa/demo")]
    public AlexaResponse Hello(AlexaRequest request)
    {
      FillRequestData(request);
      switch (request.Request.Intent.Name)
      {
        case "HelloWorldIntent":
          return HelloWorld();
        case "AddDiapers":
          return AddDiapers(request);
        case "checkDiapers":
          return CheckDiapers(request);
        default:
          return new AlexaResponse("What you need, YO", true);
      }
      
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

    private void FillRequestData(AlexaRequest request)
    {
      using (var ctx = new Data.JoshRKulpEntities())
      {
        if (ctx.Users.Where(w => w.AmazonId == request.Session.User.UserId).Count() < 1)
        {
          var user = new Data.User();
          user.Id = ctx.Users.Count() + 1;
          user.AmazonId = request.Session.User.UserId;
          ctx.Users.Add(user);
        }

        var alexaRequest = new Data.AlexaRequest();
        alexaRequest.Id = ctx.AlexaRequests.Count() + 1;
        alexaRequest.ApplicationId = request.Session.Application.ApplicationId;
        alexaRequest.Intent = request.Request.Intent.Name;
        alexaRequest.RequestId = request.Request.RequestId;
        alexaRequest.SessionId = request.Session.SessionId;

        ctx.AlexaRequests.Add(alexaRequest);
        ctx.SaveChanges();

      }
    }

    private AlexaResponse HelloWorld()
    {
      var response = new AlexaResponse("Thanks for using Diaper Tracker by Josh R Kulp", true);
      response.Response.Card.Content = "Thanks for using Diaper Tracker \n by Josh R Kulp";
      response.Response.Card.Title = "Diaper Tracker";

      return response;
    }

    private AlexaResponse AddDiapers(AlexaRequest request)
    {
      var stuff = request.Request.Intent.GetSlots();

      var name = stuff.FirstOrDefault(s => s.Key == "Name").Value;
      var date = stuff.FirstOrDefault(s => s.Key == "Date").Value;
      var number = stuff.FirstOrDefault(s => s.Key == "Number").Value;


      var response = new AlexaResponse("Added for "+name+" on "+date, true);

      using (var ctx = new Data.JoshRKulpEntities())
      {
        var userID = ctx.Users.FirstOrDefault(w => w.AmazonId == request.Session.User.UserId).Id;
        var kid = new Data.Kid();

        if (ctx.Kids.Where(w => w.UserId == userID && w.Name == name).Count() < 1)
        {
          kid.Name = name;
          kid.UserId = userID;
          kid.Id = ctx.Kids.Count() + 1;
          ctx.Kids.Add(kid);
        }
        else
        {
          kid = ctx.Kids.FirstOrDefault(w => w.UserId == userID && w.Name == name);
        }

        var diapers = new Data.Diaper();
        diapers.Count = number == null ? diaperCount : int.Parse(number);
        diapers.KidId = kid.Id;
        diapers.StartDate = Convert.ToDateTime(date);
        diapers.Id = ctx.Diapers.Count() + 1;

        ctx.Diapers.Add(diapers);

        ctx.SaveChanges();
      }

      return response;
    }

    private AlexaResponse CheckDiapers(AlexaRequest request)
    {
      string returnString = "";
      using (var ctx = new Data.JoshRKulpEntities())
      {
        var userID = ctx.Users.FirstOrDefault(w => w.AmazonId == request.Session.User.UserId).Id;
        var kids = ctx.Kids.Where(w => w.UserId == userID).ToList();

        foreach (var kid in kids)
        {
          var maxDate = ctx.Diapers.Where(w => w.KidId == kid.Id).Max(d => d.StartDate);
          var totallDiapers = ctx.Diapers.FirstOrDefault(w => w.KidId == kid.Id && w.StartDate == maxDate).Count;
          var todaysDate = DateTime.Now;
          var howManyLeft = totallDiapers - ((todaysDate.Date - maxDate.Date).TotalDays *3);

          if (howManyLeft <= 7)
          {
            returnString = returnString + kid.Name + " has " + howManyLeft.ToString() + " diapers left. It is time to bring more. ";
          }
          else
          {
            returnString = returnString + kid.Name + " has " + howManyLeft.ToString() + " diapers left. ";
          }
        }
      }

      var response = new AlexaResponse(returnString, true);
      return response;
    }


  }
}