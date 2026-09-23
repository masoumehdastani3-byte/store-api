using System;
using System.Collections.Generic;

namespace API.Models
{
    public class CompanySettings
    {
        public Nullable<int> PageShow { get; set; }
        public Nullable<int> MaxPageShow { get; set; }
        public string PageDescription { get; set; }
        public List<Company_Settings> Company_Settings { get; set; }
    }
    public class Company_Settings
    {
        public int CompanyID { get; set; }
        public string SettingName { get; set; }
        public string SettinCaption { get; set; }
        public string SettingValue { get; set; }
        public string Descriptions { get; set; }
        public string Category { get; set; }
        public Nullable<int> PageShow { get; set; }
        public Nullable<bool> Active { get; set; }
        public string PageDescription { get; set; }
    }
}