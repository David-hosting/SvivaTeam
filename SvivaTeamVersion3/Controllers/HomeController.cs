using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NLog;
using SvivaTeamVersion3.Areas.Identity.Data;
using SvivaTeamVersion3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;


namespace SvivaTeamVersion3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        [Obsolete]
        private IHostingEnvironment hostingEnv;

        SqlCommand command = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<ReportModel> reports = new List<ReportModel>();

        [Obsolete]
        public HomeController(IHostingEnvironment env,
                             ILogger<HomeController> logger)
        {
            _logger = logger;
            this.hostingEnv = env;
            con.ConnectionString = SvivaTeamVersion3.Properties.Resources.ConnectionString;
        }

        public IActionResult Index()
        {
            fetchData();
            return View(reports);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TermsOfService()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        private void fetchData()
        {
            if (reports.Count > 0)
                reports.Clear();
            try
            {
                con.Open();
                command.Connection = con;
                command.CommandText = "SELECT TOP 4 [Id], [category], [title], [description], [path] FROM dbo.Reports ORDER BY ID Desc";
                command.ExecuteNonQuery();
                dr = command.ExecuteReader();
                while (dr.Read())
                {
                    reports.Add(new ReportModel()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        category = dr["category"].ToString(),
                        title = dr["title"].ToString(),
                        remarks = dr["description"].ToString(),
                        path = dr["path"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error While fetching data {e}");
                Console.WriteLine(e.Message);
            }

        }
    }
}
