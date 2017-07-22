using DotNetWithSFDC.SFDC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotNetWithSFDC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {


            ///////////// SFDC
            string userName = "RohitasBehera1@sfdc.org";
            string password = "password@sfdc1";
            string securityToken = "3o4Aghwtcu8XvCwGXTpVJHZR";

            SforceService sfdcBinding = null;
            LoginResult currentLoginResult = null;
            sfdcBinding = new SforceService();
            try
            {
                currentLoginResult = sfdcBinding.login(userName, password + securityToken);
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                // This is likley to be caused by bad username or password
                sfdcBinding = null;
                throw (ex);
            }
            catch (Exception ex)
            {
                // This is something else, probably comminication
                sfdcBinding = null;
                throw (ex);
            }


            //Change the binding to the new endpoint
            sfdcBinding.Url = currentLoginResult.serverUrl;

            //Create a new session header object and set the session id to that returned by the login
            sfdcBinding.SessionHeaderValue = new SessionHeader();
            sfdcBinding.SessionHeaderValue.sessionId = currentLoginResult.sessionId;

            Response.Write(sfdcBinding.SessionHeaderValue.sessionId);
            //////////// SFDC
            return View();
        }
    }
}