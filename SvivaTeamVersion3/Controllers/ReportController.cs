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

namespace SvivaTeamVersion3.Controllers
{
    public class ReportController : Controller
    {
        
        private readonly ILogger logger;

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
            string sqlStatement = "INSERT INTO dbo.Reports (category,title,description,statUrgence,cordsLat,cordsLong,path) VALUES (@category,@title,@description,@statUrgence,@cordsLat,@cordsLong,@path)";

            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sqlStatement, connection);

                command.Parameters.Add("@category",    SqlDbType.NChar, 32).Value    = upload.category;
                command.Parameters.Add("@title",       SqlDbType.NChar, 32).Value    = upload.title;
                command.Parameters.Add("@description", SqlDbType.NVarChar, -1).Value = upload.remarks;
                command.Parameters.Add("@statUrgence", SqlDbType.NVarChar, -1).Value = upload.statUrgence;
                command.Parameters.Add("@cordsLat",    SqlDbType.NChar, 100).Value   = upload.coordLat;
                command.Parameters.Add("@cordsLong",   SqlDbType.NChar, 100).Value   = upload.coordLong;

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
                    if (upload.File.Count != 0) { 

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
                }
                catch (Exception _)
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
                    Console.WriteLine(e.Message);
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
                command.CommandText = "SELECT [Id], [category], [title], [description], [statUrgence], [cordsLat], [cordsLong], [path] FROM dbo.Reports";
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
                                                    path = dr["path"].ToString()
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
    }
}
