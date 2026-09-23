//using Stimulsoft.Report.Mvc;
namespace API.Models
{
    public class ReportParamsViewModel
    {
        public ReportParamsViewModel()
        {

        }
        public int? RestaurantID { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
        public string Lang { get; set; }
        //public StiMvcViewer StiMvcViewer { get; set; }
    }
}