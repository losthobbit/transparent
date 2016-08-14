using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transparent.ViewModels.DbAdmin
{
    public class CsvUploadViewModel
    {
        [Display(Name = "Table name")]
        public string TableName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Data { get; set; }

        public string Status { get; set; }
    }
}