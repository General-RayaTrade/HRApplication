using HRApplication.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Localization;
using RestSharp;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace HRApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> localizer;

        public HomeController(ILogger<HomeController> logger , IStringLocalizer<SharedResource> _localizer)
        {
            _logger = logger;
            localizer = _localizer;
        }

        public IActionResult Index()
        {
            //sendEmail();
            return View();
        }
        public IActionResult GetApplicantApp()
        {
            //GetApplicantAppById(requestId);
            //sendEmail();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetApplicantAppById(string id)
        {
            DataTable dt_app = DataBaseLayer.ExecuteSelectSQLQuery(@$"select *,DATEADD(s, Create_Date, '1-1-1970 2:00:00') as createdDate ,
                                    DATEADD(s, School_Graduation_Date, '1-1-1970 2:00:00') as SchoolGraduationDate ,
                                    DATEADD(s, University_Graduation_Date, '1-1-1970 2:00:00') as UniversityGraduationDate ,
                                    DATEADD(s, Birth_Date, '1-1-1970 2:00:00') as BirthDate 
                                    from HR_Interview_Schedule where Request_ID = {id} "
                                );
            DataTable dt_appExperience = DataBaseLayer.ExecuteSelectSQLQuery(@$"select DATEADD(s,expr.From_x, '1-1-1970 2:00:00') as From_x ,DATEADD(s,expr.To_x ,'1-1-1970 2:00:00') as To_x ,expr.Name_of_Organization,expr.Reason_for_Leaving from HR_Interview_Schedule his
                    join HR_interview_schedule_experien expr on his.Primary_Key = expr.Primary_Key 
                    where his.Request_ID = {id} "
            );
            DataTable dt_appLanguages = DataBaseLayer.ExecuteSelectSQLQuery(@$"select lang.Name,lang.Written,lang.Spoken,lang.Understanding from HR_Interview_Schedule his
                    join HR_Interview_Schedule_Languang lang on his.Primary_Key = lang.Primary_Key
                    where his.Request_ID = {id} "
            );
            List<Experience> experienceList = new List<Experience>();
            List<Languages> languages = new List<Languages>();

            if (dt_appExperience.Rows.Count > 0)
            {
                foreach (DataRow experience in dt_appExperience.Rows)
                {
                    experienceList.Add(new Experience() {
                        from = Convert.ToDateTime( experience["From_x"].ToString()),
                        To = Convert.ToDateTime( experience["To_x"].ToString()),
                        organization = experience["Name_of_Organization"].ToString(),
                        reasonOfLeaving = experience["Reason_for_Leaving"].ToString()
                    });
                }

            }
            if (dt_appLanguages.Rows.Count > 0)
            {
                foreach (DataRow language in dt_appLanguages.Rows)
                {
                    languages.Add(new Languages()
                    {
                        Lang_Name = language["Name"].ToString(),
                        Lang_Spoken = language["Spoken"].ToString(),
                        Lang_Written = language["Written"].ToString(),
                        Lang_Understanding = language["Understanding"].ToString()
                    });
                }
            }
            if (dt_app.Rows.Count > 0)
            {
                ApplicantApp applicantApp = new ApplicantApp()
                {
                    additionalTraining = dt_app.Rows[0]["Additional_Training"].ToString(),
                    address = dt_app.Rows[0]["Address"].ToString(),
                    appliedPosition = dt_app.Rows[0]["Position_Applied_for"].ToString(),
                    applyAtRaya = dt_app.Rows[0]["How_did_you_apply_at_Raya"].ToString(),
                    arabicSpoken = dt_app.Rows[0]["Arabic_spoken"].ToString(),
                    arabicUnderstanding = dt_app.Rows[0]["Arabic_understanding"].ToString(),
                    arabicWritten = dt_app.Rows[0]["Arabic_written"].ToString(),
                    birth_date = Convert.ToDateTime(dt_app.Rows[0]["BirthDate"].ToString()),
                    computerSkills = dt_app.Rows[0]["Computer_Skills"].ToString(),
                    currentlyEmployed = dt_app.Rows[0]["Are_you_currently_employed"].ToString(),
                    email = dt_app.Rows[0]["Email"].ToString(),
                    englishSpoken = dt_app.Rows[0]["English_spoken"].ToString(),
                    englishUnderstanding = dt_app.Rows[0]["English_understanding"].ToString(),
                    englishWritten = dt_app.Rows[0]["English_written"].ToString(),
                    experiences = experienceList,//dt_app.Rows[0][""].ToString(),
                    Languages = languages,
                    gender = dt_app.Rows[0][""].ToString(),
                    Military = dt_app.Rows[0][""].ToString(),
                    highgSchoolDegree = dt_app.Rows[0]["School_degree"].ToString(),
                    highgSchoolGraduationDate = Convert.ToDateTime( dt_app.Rows[0]["SchoolGraduationDate"].ToString()),
                    highgSchoolMajor = dt_app.Rows[0]["School_Major"].ToString(),
                    highgSchoolName = dt_app.Rows[0]["High_School"].ToString(),
                    Marital = dt_app.Rows[0]["Marital_Status"].ToString(),
                    Name = dt_app.Rows[0]["Applicant_Name"].ToString(),
                    nationality = dt_app.Rows[0]["Nationality"].ToString(),
                    phone = dt_app.Rows[0]["Mobile_Number"].ToString(),
                    relatives = dt_app.Rows[0]["Do_you_have_any_relatives_work"].ToString(),
                    universityDegree = dt_app.Rows[0]["University_degree"].ToString(),
                    universityGraduationDate = Convert.ToDateTime( dt_app.Rows[0]["UniversityGraduationDate"].ToString()),
                    universityMajor = dt_app.Rows[0]["University_Major"].ToString(),
                    universityName = dt_app.Rows[0]["Institute_OR_University"].ToString()
                };
            return new JsonResult(new { Succeded = true, Message = "", Content = applicantApp});
            }
            return new JsonResult(new { Succeded = false, Message = "", Content = "" });

        }
        [HttpPost]
        public JsonResult HrAppSubmition([FromBody] ApplicantApp applicantApp)
        {
            string requestId = DataBaseLayer.GetRemedyNextID("HR Interview Schedule");

            DataBaseLayer.ExecuteSQLNonQuery($@"INSERT INTO [dbo].[B805] ([C1]) VALUES('{requestId}')");
            int app = DataBaseLayer.ExecuteSQLNonQuery(String.Format($@"INSERT INTO [dbo].[T805](C1,C2,C3,C4,C5,C6,C7,C8,C112,C536870914,C536870915,C536870916,C536870917,C536870918,C536870919,C536870920,C536870921,C536870922,C536870923,C536870924,C536870937,C536870940,C536870942,C536870943,C536870945,C536870946,C536870947,C536870948,C536870949,C536870950,C536870951,C536870953,C536870954,C536871084,C536870957,C536870958,C536870960,C536870961,C536870962,C536870963,C536870964,C536870965,C536870966,C536870967,C536870968,C536870971,C536870972,C536870973,C536870959,C536870969,C536870970,C536870975,C536870976,C536870977,C536870978,C536870979,C536870980,C536870981,C536870982 )
                                                VALUES  ('{requestId}', 'Online App', DATEDIFF(s, '1-1-1970 2:00:00',getdate()),
                                                ';0;', 'Online App',  DATEDIFF(s, '1-1-1970 2:00:00',getdate()),0,'.',  ';0;',
                                                '{applicantApp.Name}', DATEDIFF(s, '1-1-1970 2:00:00',getdate()),'10','{applicantApp.phone}',
                                                '{applicantApp.email}', '{applicantApp.appliedPosition}',NULL,NULL,NULL, NULL,NULL,NULL, 0,'online app',0,0, DATEDIFF(s, '1-1-1970 2:00:00',getdate()),
                                                NULL,NULL,DATEDIFF(s, '1-1-1970 2:00:00',getdate()), DATEDIFF(s, '1-1-1970 2:00:00',getdate()), 
                                                DATEDIFF(s, '1-1-1970 2:00:00',getdate()), '{applicantApp.phone}',0,'Raya Trade LOB',
                                                '{applicantApp.address}','{applicantApp.Marital}', '{applicantApp.nationality}', DATEDIFF(s,'1-1-1970 2:00:00','{applicantApp.birth_date.ToString("dd-MMM-yyyy hh:mm:ss", CultureInfo.InvariantCulture)}'),'{applicantApp.appliedPosition}','{applicantApp.highgSchoolName}',
                                                '{applicantApp.universityName}','{applicantApp.additionalTraining}','{applicantApp.computerSkills}', '{applicantApp.arabicSpoken}','{applicantApp.englishSpoken}',
                                                '{applicantApp.applyAtRaya}','{applicantApp.relatives}','{applicantApp.currentlyEmployed}',
                                                DATEDIFF(s, '1-1-1970 2:00:00',getdate()), '{applicantApp.englishWritten}','{applicantApp.arabicWritten}', '{applicantApp.englishUnderstanding}','{applicantApp.arabicUnderstanding}',
                                                '{applicantApp.universityDegree}',DATEDIFF(s,'1-1-1970 2:00:00','{applicantApp.universityGraduationDate.ToString("dd-MMM-yyyy hh:mm:ss", CultureInfo.InvariantCulture)}'),'{applicantApp.universityMajor}','{applicantApp.highgSchoolMajor}',
                                                DATEDIFF(s,'1-1-1970 2:00:00','{applicantApp.highgSchoolGraduationDate.ToString("dd-MMM-yyyy hh:mm:ss" , CultureInfo.InvariantCulture)}'),'{applicantApp.highgSchoolDegree}')").Replace("\r\n",""));
            if (app > 0)
            {
                if (applicantApp.experiences.Count > 0)
                {
                    DataTable insertedAppId = DataBaseLayer.ExecuteSelectSQLQuery("select top 1 Request_ID,Primary_Key from HR_Interview_Schedule order by Create_Date desc");

                    if (insertedAppId.Rows.Count > 0)
                    {
                        //int.TryParse(insertedAppId.Rows[0][0].ToString() , out int InterViewScheduleId);
                        int.TryParse(insertedAppId.Rows[0][1].ToString(), out int primaryKey);
                        foreach (var experience in applicantApp.experiences)
                        {
                            string requestIdExperince = DataBaseLayer.GetRemedyNextID("HR Interview Schedule_Experience");
                            int exp = DataBaseLayer.ExecuteSQLNonQuery($@"
                            INSERT INTO [dbo].[HR_Interview_Schedule_Experien]
                            ([Request_ID],[Submitter],[Create_Date],[Assigned_To],[Last_Modified_By],[Modified_Date]
                            ,[Status],[Short_Description],[Schedule_RID],[Name_of_Organization],[Contact_person]
                            ,[From_x],[To_x],[Reason_for_Leaving],[Primary_Key])
                             VALUES
	                         (
	                         {requestIdExperince},'Online APP',DATEDIFF(s, '1-1-1970 2:00:00',getdate()),';0;',
	                         'Online APP',DATEDIFF(s, '1-1-1970 2:00:00',getdate()),0,'.',
	                         {requestId},
	                         '{experience.organization}',
	                         NULL,
	                         DATEDIFF(s,'1-1-1970 2:00:00','{experience.from.ToString("dd-MMM-yyyy hh:mm:ss" , CultureInfo.InvariantCulture)}'),
	                         DATEDIFF(s,'1-1-1970 2:00:00','{experience.To.ToString("dd-MMM-yyyy hh:mm:ss", CultureInfo.InvariantCulture)}'),
	                         '{experience.reasonOfLeaving}',
	                         {primaryKey}
	                         )
                 ");
                        }
                    }
                }
                if (applicantApp.Languages.Count > 0)
                {
                    DataTable insertedAppId = DataBaseLayer.ExecuteSelectSQLQuery("select top 1 Request_ID,Primary_Key from HR_Interview_Schedule order by Create_Date desc");
                    if (insertedAppId.Rows.Count > 0)
                    {
                        int.TryParse(insertedAppId.Rows[0][1].ToString(), out int primaryKey);
                        foreach (var lang in applicantApp.Languages)
                        {
                            string requestIdExperince = DataBaseLayer.GetRemedyNextID("HR Interview Schedule_Experience");
                            int exp = DataBaseLayer.ExecuteSQLNonQuery($@"
                            INSERT INTO [dbo].[HR_Interview_Schedule_Languang]
                                   ([Request_ID]
                                   ,[Submitter]
                                   ,[Create_Date]
                                   ,[Assigned_To]
                                   ,[Last_Modified_By]
                                   ,[Modified_Date]
                                   ,[Status]
                                   ,[Short_Description]
                                   ,[Schedule_RID]
                                   ,[Name]
                                   ,[Written]
                                   ,[Spoken]
                                   ,[Understanding]
                                   ,[Primary_Key])
                             VALUES
	                         (
	                         {requestIdExperince},'Online APP',DATEDIFF(s, '1-1-1970 2:00:00',getdate()),';0;',
	                         'Online APP',DATEDIFF(s, '1-1-1970 2:00:00',getdate()),0,'.',
	                         {requestId},
	                         '{lang.Lang_Name}','{lang.Lang_Written}','{lang.Lang_Spoken}','{lang.Lang_Understanding}',{primaryKey})");
                        }
                    }
                }
                //insert experience 
                sendEmail(requestId , applicantApp);
                return new JsonResult(new { Succeded = true, Message = "Applications submited successfully ", Content =  new { reqId=requestId } });
            }
            return new JsonResult(new { Succeded = false, Message = "Somthing went wrong please try again!", Content ="" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public  void sendEmail(string requestId , ApplicantApp applicantApp)
        {
            var client = new RestClient("http://www.rayatrade.com/ProgrammmingUtilities/api/EmailNotification");
            client.Timeout = -1;
            var request = new RestRequest(RestSharp.Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
                " + "\n" +
                            @"    ""toList"": [
                " + "\n" +
                            @"        ""hr4@rayacorp.com""
                " + "\n" +
                            @"    ],
                " + "\n" +
                            @"    ""ccList"": [
                " + "\n" +
                            @"        ""ezzat_ahmed@rayacorp.com,hamdy_smohamed@rayacorp.com,hr4@rayacorp.com""
                " + "\n" +
                            @"    ],
                " + "\n" +
                            @"    ""bccList"": [],
                " + "\n" +
                            @"    ""subject"": ""string"",
                "
                     + "\n" +
                            @$"    ""body"": ""<h3>Dear HR team;</h3> <p> Kindly note that you have an interview for ${applicantApp.Name} on ${DateTime.Now}  for ${applicantApp.appliedPosition} with Interview ID:{requestId} </p> <p>please use the below link to check Applicant application form </p> <a href='{Request.Scheme}://{Request.Host}/GetApplicantApp?requestId={requestId}#collapseOne'>Click here</a>"",
                " + "\n" +
                            @"    ""title"": ""New Hr Interview Schedule"",
                " + "\n" +
                            @"    ""host"": ""172.29.0.141"",
                " + "\n" +
                            @"    ""port"": ""25"",
                " + "\n" +
                            @"    ""from"": ""rd_svrmgr@rayacorp.com"",
                " + "\n" +
                            @"    ""password"": ""4gettheolDtwoP@ssw0rds"",
                " + "\n" +
                            @"    ""isBodyHtml"": true
                " + "\n" +
                            @"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        #region Set Language
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
            return LocalRedirect(returnUrl);
        }
        #endregion

        public IActionResult CheckDate(DateTime to , DateTime from)
        {
            if (to < from)
                return Json(false);
            else
                return Json(true);
        }
        private CultureInfo Culture()
        {
            var currentCulture = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            return currentCulture.RequestCulture.Culture;
        }
    }
}