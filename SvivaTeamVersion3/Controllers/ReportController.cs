using Microsoft.AspNetCore.Mvc;
using SvivaTeamVersion3.Models;
using SvivaTeamVersion3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SvivaTeamVersion3.Controllers
{
    public class ReportController : Controller
    {
        
        private readonly ILogger logger;
        private Timer _timer;

        [Obsolete]
        private IHostingEnvironment hostingEnv;

        SqlCommand command = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<ReportModel> reports = new List<ReportModel>();

        [Obsolete]
        public ReportController(IHostingEnvironment env,
            ILogger<ReportController> logger)
        {
            this.hostingEnv = env;
            this.logger = logger;
            con.ConnectionString = SvivaTeamVersion3.Properties.Resources.ConnectionString;
        }

        [HttpGet]
        public IActionResult Index(string id)
        {
            fetchData(id);
            return View(reports);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        private static string TextGenerator(int bytes=64)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[bytes];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];

            var finalString = new String(stringChars);

            return finalString;
        }

        [Obsolete]
        public string GetPath(string path)
        {
            return Path.Combine(hostingEnv.WebRootPath, path);
        }

        [HttpPost]
        [Obsolete]
        public ActionResult Upload(ReportModel upload)
        {
            string sqlStatement = "INSERT INTO dbo.Reports (category,title,description,statUrgence,cordsLat,cordsLong,path,PostOwner,ExpireDate) VALUES (@category,@title,@description,@statUrgence,@cordsLat,@cordsLong,@path,@PostOwner,@ExpireDate)";

            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sqlStatement, connection);

                command.Parameters.Add("@category",    SqlDbType.NChar, 32).Value    = upload.category;
                command.Parameters.Add("@title",       SqlDbType.NChar, 32).Value    = upload.title;
                command.Parameters.Add("@description", SqlDbType.NVarChar, -1).Value = upload.remarks;
                command.Parameters.Add("@statUrgence", SqlDbType.NVarChar, -1).Value = upload.statUrgence;
                command.Parameters.Add("@cordsLat",    SqlDbType.NChar, 100).Value   = upload.coordLat;
                command.Parameters.Add("@cordsLong",   SqlDbType.NChar, 100).Value   = upload.coordLong;
                command.Parameters.Add("@PostOwner",   SqlDbType.NChar, 36).Value    = upload.UserId;
                command.Parameters.Add("@ExpireDate",  SqlDbType.Date).Value         = upload.ExpireDate;

                var FileDic = "Files";
                string[] Categories         = { "Road_Problems", "Urban_Problems", "Other" };
                string[] TitlesRoadProblems = { "Accident", "Disruptive_object", "Disruptive_parking", "Defective_indication", "Other" };
                string[] TitleUrbanProblems = { "Neighborhood_problem", "Pollution", "Animals", "Distruptive_object", "Tag", "Other" };
                string[] Other              = { "Other" };
                List<string[]> TitlesList = new List<string[]>() { TitlesRoadProblems, TitleUrbanProblems, Other };
                
                string FilePath = Path.Combine(hostingEnv.WebRootPath, FileDic);

                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                var counter = 0;

                foreach (var Category in Categories)
                {
                    if (!Directory.Exists($"{FilePath}/{Category}")) 
                        Directory.CreateDirectory($"{FilePath}/{Category}"); //Createing category folder
                    else
                        break;

                    for (int i = 0; i < TitlesList[counter].Length; i++)
                    
                        foreach (var Title in TitlesList[counter].Where(Title => !Directory.Exists($"{FilePath}/{Category}/{Title}")))
                            Directory.CreateDirectory($"{FilePath}/{Category}/{Title}"); //Createing title folders

                    counter++;
                }

                try
                {
                    if (upload.File == null) throw new ArgumentNullException("value", "No files were uploaded");

                    var RandomFolder = TextGenerator();

                    var finalDirectory = $"{FilePath}/{upload.category}/{upload.title}/{RandomFolder}";

                    Directory.CreateDirectory(finalDirectory);
                    foreach (var (file, filePath) in from file in upload.File
                                                     let filePath = Path.Combine(FilePath, upload.category, upload.title, RandomFolder, file.FileName)
                                                     select (file, filePath))
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(fs);
                    }

                    command.Parameters.Add("@path", System.Data.SqlDbType.NVarChar, -1).Value = Path.Combine(FilePath, upload.category, upload.title, RandomFolder); //finalDirectory
                }
                catch (ArgumentNullException _)
                {
                    command.Parameters.Add("@path", System.Data.SqlDbType.NVarChar, -1).Value = Path.Combine(FilePath, "No Images were uploaded.");
                }
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return Redirect("/Report/Index");
                }
                catch (Exception e)
                {
                    logger.LogError($"Error while writing data: {e}");
                    return Redirect("/Report/Create");
                }
            }
        }
        private void fetchData(string id)
        {
            if (reports.Count > 0)
                reports.Clear();
            if (id != null)
                reports.Add(new ReportModel()
                {
                    reDirectID = id,
                    path = Path.Combine("Files", "No Images were uploaded.") //Anti error tank. xD
                });
            try
            {
                con.Open();
                command.Connection = con;
                command.CommandText = "SELECT [Id], [category], [title], [description], [statUrgence], [cordsLat], [cordsLong], [path], [PostOwner], [ExpireDate] FROM dbo.Reports";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    reports.Add(new ReportModel() { Id = Convert.ToInt32(dr["Id"]),
                                                    category = dr["category"].ToString(),
                                                    title = dr["title"].ToString(),
                                                    remarks = dr["description"].ToString(),
                                                    statUrgence = dr["statUrgence"].ToString(),
                                                    coordLat = dr["cordsLat"].ToString(),
                                                    coordLong = dr["cordsLong"].ToString(),
                                                    path = dr["path"].ToString(),
                                                    UserId = dr["PostOwner"].ToString(),
                    });
                }
                con.Close();
            }
            catch (Exception e)
            {
                logger.LogError($"Error While fetching data {e}");
                Console.WriteLine(e.Message);
            }
        }

        [HttpPost]
        public IActionResult OnPostDelete(int id)
        {
            string sqlStatement = "DELETE FROM dbo.Reports WHERE ID = @id";

            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    // Deletion successful
                    return RedirectToAction("Index", "Report");
                }
                else
                {
                    // No record found with the specified ID
                    // Handle the error or return an appropriate response
                    return NotFound();
                }
            }
        }
        public IActionResult StartRecurringTask()
        {
            // Calculate the time until the next midnight
            TimeSpan timeUntilMidnight = GetTimeUntilNextMidnight();

            // Create a timer that will trigger at the next midnight
            _timer = new Timer(async state =>
            {
                await CheckExperationDateOnPost();
            }, null, timeUntilMidnight, TimeSpan.FromDays(1));

            return Ok("Recurring task started");
        }

        private TimeSpan GetTimeUntilNextMidnight()
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.Date.AddDays(1); // Next midnight without considering time
            TimeSpan timeUntilMidnight = nextMidnight - now;
            return timeUntilMidnight;
        }

        private async Task CheckExperationDateOnPost()
        {
            // Perform your task here
            // Retrieve data from your table and process it
            try
            {
                con.Open();
                command.Connection = con;
                command.CommandText = "SELECT [Id] [ExpireDate] FROM dbo.Reports";
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    reports.Add(new ReportModel()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        ExpireDate = Convert.ToDateTime(dr["ExpireDate"])
                    });
                }
                con.Close();
            }
            catch (Exception e)
            {
                logger.LogError($"Error while fetching data {e}");
                Console.WriteLine(e.Message);
            }

            foreach (ReportModel report in reports)
            {
                bool isExpired = CheckDates(report.ExpireDate);
                if (isExpired)
                {
                    string sqlStatement = "DELETE FROM dbo.Reports WHERE ID = @id";

                    using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand(sqlStatement, connection);
                            command.Parameters.AddWithValue("@id", report.Id);

                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (Exception e)
                        {
                            logger.LogError($"Error while Trying to delete data {e}");
                        }
                    }
                }
            }

            await Task.Delay(1000); // Simulate some async work

            // Log or handle the processed data
            Console.WriteLine("Task executed at " + DateTime.Now);
        }
        public bool CheckDates(DateTime date)
        {
            DateTime currentDate = DateTime.Today;
            return date >= currentDate;
        }
    }
}
