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

        string userName = "RohitBehera@SFDC.org";
        string password = "password@sfdc1";
        string securityToken = "3o4Aghwtcu8XvCwGXTpVJHZR";

        SforceService sfdcBinding = null;
        LoginResult currentLoginResult = null;

        public HomeController()
        {
            sfdcBinding = new SforceService();


            ///////////// SFDC

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

            
            //////////// SFDC
        }

        public ActionResult Index()
        {
            Response.Write(sfdcBinding.SessionHeaderValue.sessionId);

            return View();
        }


        public ActionResult Query()
        {

            QueryResult queryResult = null;
            String SOQL = "";
            string productid = "";
            SOQL = "    SELECT Id, Name, Email, AccountId FROM Contact";//where ProductCode = 'Test'";
            queryResult = sfdcBinding.query(SOQL);
             
            if (queryResult.size > 0)
            {
                //put some code in here to handle the records being returned
                int i = 0;
                Contact readContact = new Contact();  
                for (i = 0; i < queryResult.records.Count(); i++)
                {
                    readContact = (Contact)queryResult.records[i];
                    string productName = readContact.Name;
                    productid = readContact.Id; // save id show we can use in update and delete product
                    Response.Write("<hr/> </br/> Contact Found!!<br/>");
                    Response.Write("Name:" + readContact.Name);
                    Response.Write("<br/>Email:" + readContact.Email);
                    Response.Write("<br/>ID:" + readContact.Id);
                }
            }
            else
            {
                //put some code in here to handle no records being returned
                string message = "No records returned.";
                Response.Write("<br/>" + message);
            }

            return View();
        }

        public ActionResult Insert()
        {
            Product2 insertProduct = new Product2();
            insertProduct.IsActive = true;
            insertProduct.IsActiveSpecified = true;
            insertProduct.Name = "Test Product";
            insertProduct.ProductCode = "Test";

            Response.Write("Name:" + insertProduct.Name);
            Response.Write("<br/>ProductCode:" + insertProduct.ProductCode);

            SaveResult[] createResults = sfdcBinding.create(new sObject[] { insertProduct });

            if (createResults[0].success)
            {
                string id = createResults[0].id;
                Response.Write("<br/>ID:" + id);
                Response.Write("<br/>INSERT Product Successfully!!!");
            }
            else
            {
                string result = createResults[0].errors[0].message;
                Response.Write("<br/>ERROR:" + result);
            }
            return View("Index");
        }

        public ActionResult Update()
        {
            Product2 updateProduct = new Product2();
            updateProduct.Id = "01t28000004sWTyAAM";
            updateProduct.Name = "Rohit Behera";

            SaveResult[] updatedResults = sfdcBinding.update(new sObject[] { updateProduct });

            if (updatedResults[0].success)
            {
                string id = updatedResults[0].id;
                Response.Write("<br/>ID:" + id);
                Response.Write("Name:<br/>" + updateProduct.Name);
                Response.Write("<br/>UPDATE Product Successfully!!!");
            }
            else
            {
                string result = updatedResults[0].errors[0].message;
                Response.Write("<br/>ERROR:" + result);
            }

            return View("Index");
        }

        public ActionResult Delete()
        {
            DeleteResult[] deleteResults = sfdcBinding.delete(new string[] { "01t28000004sWTyAAM" });

            if (deleteResults[0].success)
            {
                string id = deleteResults[0].id;
                Response.Write("<br/>ID:" + id);
                Response.Write("<br/>DELETE Product Successfully!!!");
            }
            else
            {
                string result = deleteResults[0].errors[0].message;
                Response.Write("<br/>ERROR:" + result);
            }

            return View("Index");
        }

    }
}