using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static SvivaTeamVersion3.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using SvivaTeamVersion3.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace SvivaTeamVersion3.Models
{
    public class ReportModel
    {
        [Required]
        [DisplayName("Category")]
        public string category { get; set; }
        [Required]
        [DisplayName("Title")]
        public string title { get; set; }
        [Required]
        [DisplayName("Problem description")]
        public string remarks { get; set; }
        public int Id { get; set; }
        public string userID { get; set; }
        [Required]
        [DisplayName("Report Status")]
        public statusOfReport statReport { get; set; }
        [Required]
        [DisplayName("Priority")]
        public string statUrgence { get; set; }
        [Required]
        [DisplayName("Latitude")]
        public string coordLat { get; set; }
        [Required]
        [DisplayName("Longitude")]
        public string coordLong { get; set; }

        public List<string> Name { get; set; }
        public List<IFormFile> File { get; set; }

        public string path { get; set; }

        public string reDirectID { get; set; }
    }
}
