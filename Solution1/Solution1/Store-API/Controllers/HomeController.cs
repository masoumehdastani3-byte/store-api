using API.Models;
using DataAccess;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace API.Controllers
{
    public class HomeController : Controller
    {
        StoreEntities db = new StoreEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

       
        public async Task<ActionResult> TestBaleMessage(
            int CompanyID = 1,
            string FullName = "تست بله",
            string Mobile = "09120000000",
            string Token = null,
            string ChatID = null)
        {
            string message;
            bool ok = false;
            string result = "";
            string chatId = "";

            try
            {
                if (string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(ChatID))
                {
                    var comp = db.Companies.Where(a => a.ID == CompanyID).FirstOrDefault();
                    if (comp == null)
                    {
                        message = "شرکت یافت نشد. CompanyID=" + CompanyID;
                        return RenderTestBale(ok, message, result, CompanyID, chatId);
                    }
                    if (string.IsNullOrWhiteSpace(Token))
                        Token = comp.TelegramBotToken;
                    if (string.IsNullOrWhiteSpace(ChatID))
                        ChatID = comp.TelegramUserChatID;
                }

                if (string.IsNullOrWhiteSpace(Token))
                {
                    message = "توکن بازوی بله خالی است.";
                    return RenderTestBale(ok, message, result, CompanyID, chatId);
                }
                if (string.IsNullOrWhiteSpace(ChatID))
                {
                    message = "ChatID بله خالی است.";
                    return RenderTestBale(ok, message, result, CompanyID, chatId);
                }

                chatId = ChatID.Trim();
                string text = (FullName ?? "تست بله") + " یک درخواست سفارش داد شماره تماس :" + (Mobile ?? "");

                // معادل نمونه کار شما:
                // new TelegramBotClient(token, baseUrl: "https://tapi.bale.ai/").SendTextMessageAsync(chatId, text)
                result = await Settings.SendBaleTextMessage(Token, chatId, text);
                ok = result == "0";
                message = ok ? "پیام تست بله ارسال شد." : ("نتیجه ارسال: " + result);
            }
            catch (Exception ex)
            {
                message = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "");
            }

            return RenderTestBale(ok, message, result, CompanyID, chatId);
        }

        private ActionResult RenderTestBale(bool ok, string message, string result, int companyId, string chatId)
        {
            if (Request.IsAjaxRequest() || string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Request["format"], "json", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    ok = ok,
                    message = message,
                    result = result,
                    CompanyID = companyId,
                    ChatID = chatId
                }, JsonRequestBehavior.AllowGet);
            }

            return Content(
                "<div style='direction:rtl;font-family:Tahoma;padding:24px;'>" +
                "<h2>نتیجه تست بله</h2>" +
                "<p><b>" + (ok ? "موفق" : "ناموفق") + "</b></p>" +
                "<p>" + Server.HtmlEncode(message ?? "") + "</p>" +
                "<p>CompanyID: " + companyId + "</p>" +
                "<p>ChatID: " + Server.HtmlEncode(chatId ?? "") + "</p>" +
                "<p>result: " + Server.HtmlEncode(result ?? "") + "</p>" +
                "<p><a href='" + Url.Action("Index", "Home") + "'>بازگشت</a></p>" +
                "</div>",
                "text/html; charset=utf-8");
        }

        string pathparams = @"E:\Project\Store\UI\Angular-UI\src\app\shared\service\ParamSample";
        string path_api_service = @"E:\Project\Store\UI\Angular-UI\src\app\shared\service\api.service.ts";
        string path_Controllers = @"E:\Project\Store\API\Controllers\";
        string path_modules = @"E:\Project\Store\UI\Angular-UI\src\app\shared\modules\";
        public void save(string TableName, string ProcedureName, string FlieName, string FolderName)
        {
            ViewBag.Title = "Home Page";


            if (ProcedureName != "" & ProcedureName != null)
            {
                ProcedureGenerator(TableName, ProcedureName, FlieName, FolderName);
            }

        }
        public void saveInsert(string DataAccessName, string EntityName, string FolderName, string TableName)
        {
            Insert(DataAccessName, FolderName, TableName, EntityName);
            Update(DataAccessName, FolderName, TableName, EntityName);
            Delete(DataAccessName, FolderName, TableName, EntityName);
            APIService_Insert_Update(TableName);
            ParamsMaker(TableName);
        }

        private void Insert(string DataAccessName, string FolderName, string TableName, string EntityName)
        {
            var gg = db.sp_SystemTableCOLUMN(TableName).ToList();
            string param = "";
            string model = "DataAccess." + DataAccessName + " model = new DataAccess." + DataAccessName + "();" + Environment.NewLine;
            string text = "";
            foreach (var item in gg)
            {
                if (item.COLUMN_NAME != null)
                {

                    if (item.COLUMN_NAME != "ID")
                    {
                        if (item.DATA_TYPE.Contains("varchar") || item.DATA_TYPE.Contains("text") || item.DATA_TYPE.Contains("xml") || item.DATA_TYPE.Contains("char"))
                        {
                            param = param + ", string " + item.COLUMN_NAME + Environment.NewLine;

                        }
                        if (item.DATA_TYPE.Contains("int"))
                        {
                            param = param + ", int ? " + item.COLUMN_NAME + Environment.NewLine;

                        }
                        if (item.DATA_TYPE.Contains("big"))
                        {
                            param = param + ", Int64 ? " + item.COLUMN_NAME + Environment.NewLine;
                        }
                        if (item.DATA_TYPE.Contains("decimal"))
                        {
                            param = param + ", decimal ? " + item.COLUMN_NAME + Environment.NewLine;
                        }
                        if (item.DATA_TYPE.Contains("short"))
                        {
                            param = param + ", byte ? " + item.COLUMN_NAME + Environment.NewLine;
                        }
                        if (item.DATA_TYPE.Contains("bit"))
                        {
                            param = param + ", bool ? " + item.COLUMN_NAME + Environment.NewLine;
                        }
                        if (item.DATA_TYPE.Contains("Date") && !item.COLUMN_NAME.Contains("CreateDate") && !item.COLUMN_NAME.Contains("UpdateDate"))
                        {
                            param = param + ", DateTime ? " + item.COLUMN_NAME + Environment.NewLine;

                        }
                        //------------------------------------
                        if (!item.COLUMN_NAME.Contains("Createdate") && !item.COLUMN_NAME.Contains("UpdateDate"))
                        {
                            if (item.DATA_TYPE.Contains("varchar") || item.DATA_TYPE.Contains("text") || item.DATA_TYPE.Contains("xml") || item.DATA_TYPE.Contains("char"))
                            {

                                model = model + "model." + item.COLUMN_NAME + " = Settings.SetNull(" + item.COLUMN_NAME + ");" + Environment.NewLine;
                            }

                            else
                            {
                                model = model + "model." + item.COLUMN_NAME + " = " + item.COLUMN_NAME + ";" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            model = model + "model.CreateDate = DateTime.Now;" + Environment.NewLine;

                        }
                    }
                }
            }
            param = param.Remove(0, 1);

            text = "using System;" + Environment.NewLine +
                          "using System.Collections.Generic; " + Environment.NewLine +
                          "using System.Linq; " + Environment.NewLine +
                          "using System.Net; " + Environment.NewLine +
                          "using System.Net.Http; " + Environment.NewLine +
                          "using System.Web.Http; " + Environment.NewLine +
                          "using DataAccess; " + Environment.NewLine +
                          "using API.Models; " + Environment.NewLine + Environment.NewLine +

                          "namespace API.Controllers" + Environment.NewLine +
                          "{" + Environment.NewLine +

                          " public class Insert" + TableName + "Controller : ApiController" + Environment.NewLine +
                          "{" + Environment.NewLine +

                             "StoreEntities db = new StoreEntities();" + Environment.NewLine +
                             "Models.PersianCulture pc = new Models.PersianCulture();" + Environment.NewLine +
                             "[HttpGet]" + Environment.NewLine +
                             "public string Get(" + param + "){" + Environment.NewLine +
                             "  try{" + Environment.NewLine + model +
                             "db." + EntityName + ".Add(model);" + Environment.NewLine +
           "var dd = db.SaveChanges();" + Environment.NewLine +
          " return \"0\";" + Environment.NewLine +
       "}" + Environment.NewLine +
          " catch (Exception ex)" + Environment.NewLine +
           "{" + Environment.NewLine +
              " Models.Log log = new Models.Log();" + Environment.NewLine +

      " log.WriteErrorLog(\" Insert" + TableName + " :\" + ex.Message);" + Environment.NewLine +

               "if(ex.InnerException!=null)" + Environment.NewLine +
               "{" + Environment.NewLine +

                  " log.WriteErrorLog(\" Insert" + TableName + " InnerException :\" + ex.InnerException.Message);" + Environment.NewLine +
                   "return ex.InnerException.Message;" + Environment.NewLine +
               "}" + Environment.NewLine +
               "else" + Environment.NewLine +
              " {" + Environment.NewLine +
               "    return ex.Message; " + Environment.NewLine +
              " }" + Environment.NewLine +

          " }" + Environment.NewLine +
      " }" + Environment.NewLine +
      " }" + Environment.NewLine +
  " }" + Environment.NewLine;

            SaveText(path_Controllers, text, "Insert" + TableName + "Controller.cs", FolderName);
        }
        private void Update(string DataAccessName, string FolderName, string TableName, string EntityName)
        {
            var hh = db.sp_SystemTableCOLUMN(TableName).ToList();
            string param = "";

            string model = "DataAccess." + DataAccessName + " model = db." + EntityName + ".Where(a => a.ID == ID).FirstOrDefault();" + Environment.NewLine;
            string text = "";
            foreach (var item in hh)
            {
                if (item.COLUMN_NAME != null)
                {


                    if (item.DATA_TYPE.Contains("varchar") || item.DATA_TYPE.Contains("text") || item.DATA_TYPE.Contains("xml") || item.DATA_TYPE.Contains("char"))
                    {
                        param = param + ", string " + item.COLUMN_NAME + Environment.NewLine;

                    }
                    if (item.DATA_TYPE.Contains("int"))
                    {
                        param = param + ", int ? " + item.COLUMN_NAME + Environment.NewLine;

                    }
                    if (item.DATA_TYPE.Contains("big"))
                    {
                        param = param + ", Int64 ? " + item.COLUMN_NAME + Environment.NewLine;
                    }
                    if (item.DATA_TYPE.Contains("decimal"))
                    {
                        param = param + ", decimal ? " + item.COLUMN_NAME + Environment.NewLine;
                    }
                    if (item.DATA_TYPE.Contains("short"))
                    {
                        param = param + ", byte ? " + item.COLUMN_NAME + Environment.NewLine;
                    }
                    if (item.DATA_TYPE.Contains("bit"))
                    {
                        param = param + ", bool ? " + item.COLUMN_NAME + Environment.NewLine;
                    }
                    if (item.DATA_TYPE.Contains("Date") && !item.COLUMN_NAME.Contains("CreateDate") && !item.COLUMN_NAME.Contains("UpdateDate"))
                    {
                        param = param + ", DateTime ? " + item.COLUMN_NAME + Environment.NewLine;

                    }
                    //------------------------------------
                    if (item.COLUMN_NAME != "ID")
                    {


                        if (!item.COLUMN_NAME.Contains("Createdate") && !item.COLUMN_NAME.Contains("UpdateDate"))
                        {
                            if (item.DATA_TYPE.Contains("varchar") || item.DATA_TYPE.Contains("text") || item.DATA_TYPE.Contains("xml") || item.DATA_TYPE.Contains("char"))
                            {

                                model = model + "model." + item.COLUMN_NAME + " = Settings.SetNull(" + item.COLUMN_NAME + ");" + Environment.NewLine;
                            }

                            else
                            {
                                model = model + "model." + item.COLUMN_NAME + " = " + item.COLUMN_NAME + ";" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            model = model + "model.UpdateDate = DateTime.Now;" + Environment.NewLine;

                        }
                    }

                }
            }
            param = param.Remove(0, 1);

            text = "using System;" + Environment.NewLine +
                          "using System.Collections.Generic; " + Environment.NewLine +
                          "using System.Linq; " + Environment.NewLine +
                          "using System.Net; " + Environment.NewLine +
                          "using System.Net.Http; " + Environment.NewLine +
                          "using System.Web.Http; " + Environment.NewLine +
                          "using DataAccess; " + Environment.NewLine +
                          "using API.Models; " + Environment.NewLine + Environment.NewLine +

                          "namespace API.Controllers" + Environment.NewLine +
                          "{" + Environment.NewLine +

                          " public class Update" + TableName + "Controller : ApiController" + Environment.NewLine +
                          "{" + Environment.NewLine +

                             "StoreEntities db = new StoreEntities();" + Environment.NewLine +
                             "Models.PersianCulture pc = new Models.PersianCulture();" + Environment.NewLine +
                             "[HttpGet]" + Environment.NewLine +
                             "public string Get(" + param + "){" + Environment.NewLine +
                             "  try{" + Environment.NewLine + model +

           "var dd = db.SaveChanges();" + Environment.NewLine +
          " return \"0\";" + Environment.NewLine +
       "}" + Environment.NewLine +
          " catch (Exception ex)" + Environment.NewLine +
           "{" + Environment.NewLine +
              " Models.Log log = new Models.Log();" + Environment.NewLine +

      " log.WriteErrorLog(\" Insert" + TableName + " :\" + ex.Message);" + Environment.NewLine +

               "if(ex.InnerException!=null)" + Environment.NewLine +
               "{" + Environment.NewLine +

                  " log.WriteErrorLog(\" Insert" + TableName + " InnerException :\" + ex.InnerException.Message);" + Environment.NewLine +
                   "return ex.InnerException.Message;" + Environment.NewLine +
               "}" + Environment.NewLine +
               "else" + Environment.NewLine +
              " {" + Environment.NewLine +
               "    return ex.Message; " + Environment.NewLine +
              " }" + Environment.NewLine +

          " }" + Environment.NewLine +
      " }" + Environment.NewLine +
      " }" + Environment.NewLine +
  " }" + Environment.NewLine;

            SaveText(path_Controllers, text, "Update" + TableName + "Controller.cs", FolderName);
        }
        private void Delete(string DataAccessName, string FolderName, string TableName, string EntityName)
        {



            string model = "DataAccess." + DataAccessName + " model = db." + EntityName + ".Where(a => a.ID == ID).FirstOrDefault();" + Environment.NewLine;
            string text = "";


            text = "using System;" + Environment.NewLine +
                          "using System.Collections.Generic; " + Environment.NewLine +
                          "using System.Linq; " + Environment.NewLine +
                          "using System.Net; " + Environment.NewLine +
                          "using System.Net.Http; " + Environment.NewLine +
                          "using System.Web.Http; " + Environment.NewLine +
                          "using DataAccess; " + Environment.NewLine +
                          "using API.Models; " + Environment.NewLine + Environment.NewLine +

                          "namespace API.Controllers" + Environment.NewLine +
                          "{" + Environment.NewLine +

                          " public class Delete" + TableName + "Controller : ApiController" + Environment.NewLine +
                          "{" + Environment.NewLine +

                             "StoreEntities db = new StoreEntities();" + Environment.NewLine +
                             "Models.PersianCulture pc = new Models.PersianCulture();" + Environment.NewLine +
                             "[HttpGet]" + Environment.NewLine +
                             "public string Get(string UserName,string Lang, int ID){" + Environment.NewLine +
                             "  try{" + Environment.NewLine + model + Environment.NewLine +
            "db." + EntityName + ".Remove(model);" + Environment.NewLine +
           "var dd = db.SaveChanges();" + Environment.NewLine +
          " return \"0\";" + Environment.NewLine +
       "}" + Environment.NewLine +
          " catch (Exception ex)" + Environment.NewLine +
           "{" + Environment.NewLine +
              " Models.Log log = new Models.Log();" + Environment.NewLine +

      " log.WriteErrorLog(\" Insert" + TableName + " :\" + ex.Message);" + Environment.NewLine +

               "if(ex.InnerException!=null)" + Environment.NewLine +
               "{" + Environment.NewLine +

                  " log.WriteErrorLog(\" Insert" + TableName + " InnerException :\" + ex.InnerException.Message);" + Environment.NewLine +
                   "return ex.InnerException.Message;" + Environment.NewLine +
               "}" + Environment.NewLine +
               "else" + Environment.NewLine +
              " {" + Environment.NewLine +
               "    return ex.Message; " + Environment.NewLine +
              " }" + Environment.NewLine +

          " }" + Environment.NewLine +
      " }" + Environment.NewLine +
      " }" + Environment.NewLine +
  " }" + Environment.NewLine;

            SaveText(path_Controllers, text, "Delete" + TableName + "Controller.cs", FolderName);
        }
        private void ProcedureGenerator(string TableName, string ProcedureName, string FlieName, string FolderName)
        {
            SelectSp(ProcedureName, FlieName, FolderName);
            CreateModules(TableName, ProcedureName, FlieName);
        }

        private void SelectSp(string ProcedureName, string FlieName, string FolderName)
        {
            var dd = db.sp_SystemParameterProcudure(ProcedureName).ToList();
            int count = dd.Count();
            string param = "";
            string paramNull = "";
            string paramSP = "";
            for (int i = 1; i <= count - 3; i++)
            {
                paramNull = paramNull + ",null";
            }

            foreach (var item in dd)
            {
                if (item.types_name.Contains("varchar") || item.types_name.Contains("text") || item.types_name.Contains("xml") || item.types_name.Contains("char"))
                {
                    param = param + ", string " + item.parameters_name;
                }
                if (item.types_name.Contains("int"))
                {
                    param = param + ", int ? " + item.parameters_name;
                }
                if (item.types_name.Contains("big"))
                {
                    param = param + ", Int64 ? " + item.parameters_name;
                }
                if (item.types_name.Contains("short"))
                {
                    param = param + ", byte ? " + item.parameters_name;
                }
                if (item.types_name.Contains("bit"))
                {
                    param = param + ", bool ? " + item.parameters_name;
                }
                if (item.types_name.Contains("Date"))
                {
                    param = param + ", DateTime ? " + item.parameters_name;
                }
                paramSP = paramSP + "," + item.parameters_name;
            }

            param = param.Remove(0, 1);
            paramSP = paramSP.Remove(0, 1);
            string text = "using System;" + Environment.NewLine +
                           "using System.Collections.Generic; " + Environment.NewLine +
                           "using System.Linq; " + Environment.NewLine +
                           "using System.Net; " + Environment.NewLine +
                           "using System.Net.Http; " + Environment.NewLine +
                           "using System.Web.Http; " + Environment.NewLine +
                           "using DataAccess; " + Environment.NewLine +
                           "using API.Models; " + Environment.NewLine + Environment.NewLine +

                           "namespace API.Controllers" + Environment.NewLine +
                           "{" + Environment.NewLine +

                           " public class " + FlieName + "Controller : ApiController" + Environment.NewLine +
                           "{" + Environment.NewLine +

                              "StoreEntities db = new StoreEntities();" + Environment.NewLine +
                              "Models.PersianCulture pc = new Models.PersianCulture();" + Environment.NewLine +
                              "[HttpGet]" + Environment.NewLine +

                               "public List<" + ProcedureName + "_Result> Get(" + param + ")" + Environment.NewLine +
                               "{" + Environment.NewLine +

                                   "var result = db." + ProcedureName + "(" + paramSP + ").ToList();" + Environment.NewLine +
                                   "return result;" + Environment.NewLine +
                               "} // List" + Environment.NewLine +

                               "[HttpGet]" + Environment.NewLine +
                               "public List<" + ProcedureName + "_Result> Get(string Lang, string UserName, int ID)" + Environment.NewLine +
                               "{" + Environment.NewLine +

                                  " var list = db." + ProcedureName + "(Settings.SetNull(Lang), Settings.SetNull(UserName), ID" + paramNull + ").ToList();" + Environment.NewLine +
                                   "return list;" + Environment.NewLine +
                               "}" + Environment.NewLine +
                           "} // class" + Environment.NewLine +
               "} //End namespace";



            SaveText(path_Controllers, text, FlieName + "Controller.cs", FolderName);
        }
        private void CreateModules(string TableName, string ProcedureName, string FlieName)
        {
            var dd = db.sp_SystemTableCOLUMN(ProcedureName).ToList();
            var bb = db.sp_SystemParameterProcudure(ProcedureName).ToList();
            string param = "";
            string param1 = "";
            string HtppParams = "params: {" + Environment.NewLine;

            string text = "export class " + TableName + Environment.NewLine +
            "{" + Environment.NewLine;
            foreach (var item in dd)
            {
                if (item.COLUMN_NAME != null)
                {

                    if (item.DATA_TYPE.Contains("varchar") || item.DATA_TYPE.Contains("text") || item.DATA_TYPE.Contains("xml") || item.DATA_TYPE.Contains("char") || item.DATA_TYPE.Contains("Date"))
                    {
                        text = text + item.COLUMN_NAME + ": string ;" + Environment.NewLine;

                    }
                    if (item.DATA_TYPE.Contains("int") || item.DATA_TYPE.Contains("big") || item.DATA_TYPE.Contains("short"))
                    {
                        text = text + item.COLUMN_NAME + " : Number ;" + Environment.NewLine;
                    }
                    if (item.DATA_TYPE.Contains("bit"))
                    {
                        text = text + item.COLUMN_NAME + " : boolean ;" + Environment.NewLine;
                    }


                }
            }
            foreach (var item in bb)
            {
                param1 = param1 + ",this.SearchForm.controls." + item.parameters_name + ".value" + Environment.NewLine;
                param = param + "," + item.parameters_name + ": string";
                HtppParams = HtppParams + item.parameters_name + ":" + item.parameters_name + "," + Environment.NewLine;
            }
            text = text + "}";
            text = text + Environment.NewLine + Environment.NewLine;
            HtppParams = HtppParams + "}";
            param = param.Remove(0, 1);

            string fff = "// SearchParam" + TableName + "//*****************" + Environment.NewLine +
                        param1 + Environment.NewLine;


            SaveText(pathparams, fff, ProcedureName + ".txt", ProcedureName);

            SaveText(path_modules, text, TableName + ".module.ts", "");

            APIServiceAddProcedure(TableName, FlieName, param, HtppParams);
        }

        private string APIServiceAddProcedure(string TableName, string FlieName, string param, string HtppParams)
        {
            string textApi = "";
            textApi = textApi + "//" + TableName + "*********************************************************************" + Environment.NewLine;
            textApi = textApi + "Fetch_Filter" + TableName + "Get(" + param + ") {" + Environment.NewLine +
                           "    this.configUrl = this.configUrlBasic + '/" + FlieName + "/Get'" + Environment.NewLine +

                           "    return this.http.get<" + TableName + "[]>(this.configUrl, {" + Environment.NewLine +
                           HtppParams + Environment.NewLine +


                   "}).pipe(" + Environment.NewLine +
                  " catchError(this.handleError)" + Environment.NewLine +
                 ");" + Environment.NewLine +

                "}" + Environment.NewLine +
            //---------------------------------------------------------------------
            "Fetch_Filter" + TableName + "(Lang: string, UserName: string) {" + Environment.NewLine +
                           "    this.configUrl = this.configUrlBasic + '/" + FlieName + "/Get'" + Environment.NewLine +

                           "    return this.http.get<" + TableName + "[]>(this.configUrl, {" + Environment.NewLine +
           " params: {" + Environment.NewLine +
                    "Lang: Lang," + Environment.NewLine +
                "UserName: UserName" + Environment.NewLine +
            "}" + Environment.NewLine +

            "}).pipe(" + Environment.NewLine +
               " catchError(this.handleError)" + Environment.NewLine +
              ");" + Environment.NewLine +

             "}" + Environment.NewLine +
             //-----------------------------------------------------
             "Fetch_Filter" + TableName + "ID(Lang: string, UserName: string,ID :string) {" + Environment.NewLine +
                           "    this.configUrl = this.configUrlBasic + '/" + FlieName + "/Get'" + Environment.NewLine +

                           "    return this.http.get<" + TableName + ">(this.configUrl, {" + Environment.NewLine +
           " params: {" + Environment.NewLine +
                    "Lang: Lang," + Environment.NewLine +
                    "UserName: UserName," + Environment.NewLine +
                    "ID: ID" + Environment.NewLine +
            "}" + Environment.NewLine +

            "}).pipe(" + Environment.NewLine +
               " catchError(this.handleError)" + Environment.NewLine +
              ");" + Environment.NewLine +

             "}" + Environment.NewLine +
            "}";

            WriteImport(path_api_service,
                        "import { " + TableName + " } from '../../shared/modules/" + TableName + ".module';"
                );
            using (StreamWriter w = System.IO.File.AppendText(path_api_service))
            {
                AppendTextAPI(textApi, w);

            }
            ;
            return textApi;
        }
        private string APIService_Insert_Update(string TableName)
        {


            string textApi = "";
            textApi = textApi + "//" + TableName + "*********************************************************************" + Environment.NewLine;
            textApi = textApi + "//Insert" + TableName + "---------------------------------------------------------------" + Environment.NewLine;
            textApi = textApi + "Insert" + TableName + "(params: any) {" + Environment.NewLine +
                           "this.configUrl = this.configUrlBasic + '/Insert" + TableName + "/Get'" + Environment.NewLine +
                            "return this.http.get(this.configUrl, { params }" + Environment.NewLine +

                 "  ).pipe(" + Environment.NewLine +
            "catchError(this.handleError)" + Environment.NewLine +
       " );" + Environment.NewLine +
       " }" + Environment.NewLine +
            //---------------------------------------------------------------------
            "//Update" + TableName + "---------------------------------------------------------------" + Environment.NewLine +
            "Update" + TableName + "(params: any) {" + Environment.NewLine +
                           "this.configUrl = this.configUrlBasic + '/Update" + TableName + "/Get'" + Environment.NewLine +
                            "return this.http.get(this.configUrl, { params }" + Environment.NewLine +


            "  ).pipe(" + Environment.NewLine +
            "catchError(this.handleError)" + Environment.NewLine +
       " );" + Environment.NewLine +
       " }" + Environment.NewLine +
             //-----------------------------------------------------
             "//Delete" + TableName + "---------------------------------------------------------------" + Environment.NewLine +
             "Delete" + TableName + "ID(ID :string,UserName: string,Lang :string) {" + Environment.NewLine +
                           "    this.configUrl = this.configUrlBasic + '/Delete" + TableName + "/Get'" + Environment.NewLine +
                "return this.http.get(this.configUrl, {" + Environment.NewLine +
                "params: {" + Environment.NewLine +
                "ID: ID," + Environment.NewLine +
                "UserName: UserName," + Environment.NewLine +
                "Lang: Lang " + Environment.NewLine +
            "}" + Environment.NewLine +
           " }).pipe(" + Environment.NewLine +
             "catchError(this.handleError)" + Environment.NewLine +
        " );" + Environment.NewLine +
       " }" + Environment.NewLine;


            textApi = textApi + " }" + Environment.NewLine;
            WriteImport(path_api_service, "");
            using (StreamWriter w = System.IO.File.AppendText(path_api_service))
            {
                AppendTextAPI(textApi, w);

            }
            ;
            return textApi;
        }


        private void ParamsMaker(string TableName)
        {
            var dd = db.sp_SystemTableCOLUMN(TableName).ToList();
            string HtppParams = "//params: {" + Environment.NewLine;
            string UpdateForm = "";
            string UpdateParams = "//params: {" + Environment.NewLine;
            string InsertParams = "//params: {" + Environment.NewLine;
            string HtmlInsertInput = "";
            string HtmlInsertUpdate = "";
            string HtmlInsertSerach = "List = List.filter(a =>" + Environment.NewLine;
            string formBuilderInsert = "//this.InsertForm = this.formBuilder.group({";
            string TableSerach = "";


            foreach (var item in dd)
            {
                if (item.COLUMN_NAME != null)
                {
                    HtppParams = HtppParams + "//" + item.COLUMN_NAME + ":" + item.COLUMN_NAME + "," + Environment.NewLine;

                    UpdateForm = UpdateForm + "// this.formControlsUpdate." + item.COLUMN_NAME + ".patchValue(data[0]." + item.COLUMN_NAME + ");" + Environment.NewLine;

                    UpdateParams = UpdateParams + "//" + item.COLUMN_NAME + ":" + "this.UpdateForm.controls." + item.COLUMN_NAME + ".value," + Environment.NewLine;

                    InsertParams = InsertParams + "//" + item.COLUMN_NAME + ":" + "this.InsertForm.controls." + item.COLUMN_NAME + ".value," + Environment.NewLine;

                    if (item.DATA_TYPE.Contains("varchar") || item.DATA_TYPE.Contains("text") || item.DATA_TYPE.Contains("xml") || item.DATA_TYPE.Contains("char") || item.DATA_TYPE.Contains("Date"))
                    {
                        formBuilderInsert = formBuilderInsert + "//" + item.COLUMN_NAME + ":['', [Validators.required]]," + Environment.NewLine;
                    }
                    else
                    {
                        formBuilderInsert = formBuilderInsert + "//" + item.COLUMN_NAME + ":[null, [Validators.required]]," + Environment.NewLine;
                    }
                    HtmlInsertInput = HtmlInsertInput + Environment.NewLine +
                    "< div class=\"col-md-6 form-group\">" + Environment.NewLine +
                    "<label>{{'Panel." + item.COLUMN_NAME + "' | translate}}</label>" + Environment.NewLine +
                    "<input type = \"text\" class=\"form-control\" name=\"" + item.COLUMN_NAME + "\" formControlName=\"" + item.COLUMN_NAME + "\" maxlength=\"" + item.MAXIMUM + "\" [ngClass]=\"{ 'has-error': isSubmitted && formControlsInsert." + item.COLUMN_NAME + ".errors }\" />" + Environment.NewLine +
                    "<samp *ngIf = \"isSubmitted && formControlsInsert." + item.COLUMN_NAME + ".errors\" class=\"error-red\">{{'Panel.required' | translate}}</samp>" + Environment.NewLine +
                    "</div>" + Environment.NewLine;

                    HtmlInsertUpdate = HtmlInsertUpdate + Environment.NewLine +
                    "< div class=\"col-md-6 form-group\">" + Environment.NewLine +
                    "<label>{{'Panel." + item.COLUMN_NAME + "' | translate}}</label>" + Environment.NewLine +
                    "<input type = \"text\" class=\"form-control\" name=\"" + item.COLUMN_NAME + "\" formControlName=\"" + item.COLUMN_NAME + "\" maxlength=\"" + item.MAXIMUM + "\" [ngClass]=\"{ 'has-error': isSubmittedUpdate && formControlsUpdate." + item.COLUMN_NAME + ".errors }\" />" + Environment.NewLine +
                    "<samp *ngIf = \"isSubmittedUpdate && formControlsUpdate." + item.COLUMN_NAME + ".errors\" class=\"error-red\">{{'Panel.required' | translate}}</samp>" + Environment.NewLine +
                    "</div>" + Environment.NewLine;

                    HtmlInsertSerach = HtmlInsertSerach + "||(a." + item.COLUMN_NAME + " === null ? false : a." + item.COLUMN_NAME + ".toString().toLowerCase().includes(searchTerm.toLowerCase()))" + Environment.NewLine;

                    TableSerach = TableSerach + "<th scope = \"col\" sortable = \"" + item.COLUMN_NAME + "\"(sort) = \"onSort($event)\" class=\"nowrap\"><i class=\"fa fa-sort\"></i> {{'Panel." + item.COLUMN_NAME + "' | translate}}</th>" + Environment.NewLine;

                }
            }
            HtppParams = HtppParams + "//}";
            UpdateParams = UpdateParams + "//}";
            InsertParams = InsertParams + "//}";
            HtmlInsertSerach = HtmlInsertSerach.Remove(0, 2);
            HtmlInsertSerach = HtmlInsertSerach + ");";
            formBuilderInsert = formBuilderInsert + "});";

            string text = "//" + TableName + "//*****************" + Environment.NewLine +
                        HtppParams + Environment.NewLine +
                        "//***UpdateForm " + TableName + "//*****************" + Environment.NewLine +
                        UpdateForm + Environment.NewLine +
                        "//***UpdateParams " + TableName + "//*****************" + Environment.NewLine +
                        UpdateParams + Environment.NewLine +
                        "//***InsertParams " + TableName + "//*****************" + Environment.NewLine +
                        InsertParams + Environment.NewLine +
                        "//***formBuilderInsert " + TableName + "//*****************" + Environment.NewLine +
                        formBuilderInsert + Environment.NewLine +
                        "//***HtmlInsertInput " + TableName + "//*****************" + Environment.NewLine +
                        HtmlInsertInput + Environment.NewLine +
                        "//***HtmlInsertInput " + TableName + "//*****************" + Environment.NewLine +
                        HtmlInsertUpdate + Environment.NewLine +
                        "//***HtmlInsertSerach " + TableName + "//*****************" + Environment.NewLine +
                        HtmlInsertSerach + Environment.NewLine +
                        "//***HtmlTableSerach " + TableName + "//*****************" + Environment.NewLine +
                        TableSerach + Environment.NewLine;

            SaveText(pathparams, text, TableName + ".txt", TableName);


        }

        public static void AppendTextAPI(string text, TextWriter w)
        {


            w.WriteLine($"//--{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");

            w.WriteLine($"  {text}");
            w.WriteLine("//-------------------------------");
        }
        static void WriteImport(string filename, string text)
        {
            int i = 1;
            var lines = System.IO.File.ReadAllLines(filename);

            string tempfile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempfile))
            using (var reader = new StreamReader(filename))
            {
                writer.WriteLine(text);
                while (lines.Length > i)
                {
                    writer.WriteLine(reader.ReadLine());
                    i++;
                }


            }

            System.IO.File.Copy(tempfile, filename, true);
        }

        private void SaveText(string path, string text, string FileName, string FolderName)
        {

            path = path + FolderName;

            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {

                    DirectoryInfo di = Directory.CreateDirectory(path);
                }

                System.IO.File.WriteAllText(path + @"\" + FileName, text);




                // Delete the directory.
                //di.Delete();
                //Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }


        }
    }
}