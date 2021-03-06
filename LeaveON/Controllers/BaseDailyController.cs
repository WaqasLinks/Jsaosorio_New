using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Data.SqlClient;
using jsaosorio.Models;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using System.Configuration;
using LeaveON.Models;
using System.Linq.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class BaseDailyController : Controller
  {
    //private CallCenterSalesEntities db = new CallCenterSalesEntities();
    private jsaosorioEntities db = new jsaosorioEntities();
    DataSet tempDataSet;
    // GET: DNCs

    // GET: DNCs/Create
    [Authorize(Roles = "Admin,Manager,User")]
    public ActionResult Upload()
    {
      //Response.Cookies[".AspNet.ApplicationCookie"].Expires = DateTime.Now.AddDays(-1);
      //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "SupplierName");

      //List<string> masterLst = System.IO.File.ReadAllLines(@"C:\temp\master.txt").ToList();
      //List<string> detailLst = System.IO.File.ReadAllLines(@"C:\temp\detail.txt").ToList();
      //string abcd="";
      //foreach (string itm1 in masterLst)
      //{
      //  List<string> foundLst = detailLst.Where(x => x.Contains(itm1)).ToList();
      //  if (foundLst.Count > 0)
      //  {
      //    abcd = foundLst.FirstOrDefault();
      //  }
      //  //foreach (string itm2 in detailLst)
      //  //{
      //  //}

      //}

      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,User")]
    public ActionResult Upload(IEnumerable<HttpPostedFileBase> files, string SupplierId)
    {
      int count = 0;
      DateTime dateTimeNow = DateTime.Now;
      string path = string.Empty;
      string FileName = string.Empty;
      if (files != null)
      {
        foreach (var file in files)
        {
          if (file != null && file.ContentLength > 0)
          {
            //var fileExt = dateTimeNow.Ticks + Path.GetExtension(file.FileName);
            path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), file.FileName);
            FileName = file.FileName;
            file.SaveAs(path);
            count++;
          }
          SupplierId = "1";
          BulkInsert(dateTimeNow, int.Parse(SupplierId), file.FileName);
          System.IO.File.Delete(path);
        }
      }

      TempData["Message1"] = "File Uploaded Successfully";
      TempData["FileName"] = FileName;
      //return new JsonResult { Data = "Successfully " + count + " file(s) uploaded" };
      //return RedirectToAction("Index","Uploads");
      return RedirectToAction("DNCSearch");
      //return View("SearchDNC");

    }
    private void BulkInsert(DateTime dateTimeNow, int SupplierId, String FileName)
    {
      string UploadId = Guid.NewGuid().ToString();
      db.Uploads.Add(new Upload() { Id = UploadId, FileName = FileName, DateCreated = dateTimeNow });




      using (var stream = System.IO.File.Open(Path.Combine(Server.MapPath("~/App_Data/Uploads"), FileName), FileMode.Open, FileAccess.Read))
      {
        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
        {
          tempDataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
          {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
          });
          //tableCollection = result.Tables;
          //dataGridView1.DataSource = result.Tables[0];//tableCollection[0];
        }


        //dx//dataGridView1.AutoGenerateColumns = false;
        //dtBOM.Rows.Add();
        //_TotExtCost = 0;
        tempDataSet.Tables[0].Columns.Add(new DataColumn("Id", typeof(string)));
        tempDataSet.Tables[0].Columns.Add(new DataColumn("AspNetUserId", typeof(string)));
        tempDataSet.Tables[0].Columns.Add(new DataColumn("UploadId", typeof(string)));
        string LoggedInUserId = User.Identity.GetUserId();
        foreach (DataRow dr in tempDataSet.Tables[0].Rows)
        {
          //string colName=gvr.Cells[0].OwningColumn.HeaderText;

          bool isAdd = false;
          for (int i = 0; i < tempDataSet.Tables[0].Columns.Count; i++)
          {
            //if (dr[i] == null || dr[i] == DBNull.Value || String.IsNullOrWhiteSpace(dr[i].ToString()))
            if (dr[i] == DBNull.Value)
            {
              isAdd = false;
            }
            else
            {
              isAdd = true;
              break;
            }
          }

          if (isAdd == true)
          {
            dr["Id"] = Guid.NewGuid();
            dr["AspNetUserId"] = LoggedInUserId;
            dr["UploadId"] = UploadId;

          }

        }

      }


      //this.Text += " - Inserting to SQL SERVER...";
      //Application.DoEvents();
      //string connection = @"Data Source=WQSLAPTOP;Initial Catalog=db865726324;User Id = sa; Password = abc;";


      var conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
      var csb = new SqlConnectionStringBuilder(conn);

      //string connection = @"Data Source="+ csb.DataSource + ";Initial Catalog=" + csb.InitialCatalog + ";User Id =" + csb.UserID + ";Password =" + csb.Password + ";";

      //string connection = @"Data Source=db865726324.hosting-data.io;Initial Catalog=db865726324;User Id = dbo865726324; Password = CallCenterDB1*;";
      //string connection = System.Configuration.ConfigurationManager.ConnectionStrings["CallCenterSalesEntities"].ConnectionString;
      SqlConnection con = new SqlConnection(conn);

      //create object of SqlBulkCopy which help to insert  
      SqlBulkCopy objbulk = new SqlBulkCopy(con);

      //assign Destination table name  
      objbulk.DestinationTableName = "BaseDaily";


      objbulk.ColumnMappings.Add("Id", "Id");
      objbulk.ColumnMappings.Add("AspNetUserId", "AspNetUserId");
      objbulk.ColumnMappings.Add("AdSense", "AdSense");
      objbulk.ColumnMappings.Add("Ages 18-24", "Ages_18_24");
      objbulk.ColumnMappings.Add("Ages 25-34", "Ages_25_34");
      objbulk.ColumnMappings.Add("Ages 35-44", "Ages_35_44");
      objbulk.ColumnMappings.Add("Ages 45-54", "Ages_45_54");
      objbulk.ColumnMappings.Add("Ages 55-64", "Ages_55_64");
      objbulk.ColumnMappings.Add("Ages 65+", "Ages_65_Plus");
      objbulk.ColumnMappings.Add("Bounce rate", "Bounce_Rate");
      objbulk.ColumnMappings.Add("Company name", "Company_name");
      objbulk.ColumnMappings.Add("Country", "Country");
      objbulk.ColumnMappings.Add("Desktop bounce rate", "Desktop_bounce_rate");
      objbulk.ColumnMappings.Add("Desktop pages per visit", "Desktop_pages_per_visit");
      objbulk.ColumnMappings.Add("Desktop total page views", "Desktop_total_page_views");
      objbulk.ColumnMappings.Add("Desktop traffic share", "Desktop_traffic_share");
      objbulk.ColumnMappings.Add("Desktop unique visitors", "Desktop_unique_visitors");
      objbulk.ColumnMappings.Add("Desktop visit duration", "Desktop_visit_duration");
      objbulk.ColumnMappings.Add("Direct traffic", "Direct_traffic");
      objbulk.ColumnMappings.Add("Direct traffic share", "Direct_traffic_share");
      objbulk.ColumnMappings.Add("Display ad traffic", "Display_ad_traffic");
      objbulk.ColumnMappings.Add("Display ads traffic share", "Display_ads_traffic_share");
      objbulk.ColumnMappings.Add("Domain", "Domain");
      objbulk.ColumnMappings.Add("Email address", "Email_address");
      objbulk.ColumnMappings.Add("Employees", "Employees");
      objbulk.ColumnMappings.Add("Female distribution", "Female_distribution");
      objbulk.ColumnMappings.Add("HQ address", "HQ_address");
      objbulk.ColumnMappings.Add("HQ city", "HQ_city");
      objbulk.ColumnMappings.Add("HQ country", "HQ_country");
      objbulk.ColumnMappings.Add("HQ postal code", "HQ_postal_code");
      objbulk.ColumnMappings.Add("HQ state", "HQ_state");
      objbulk.ColumnMappings.Add("Mail share", "Mail_share");
      objbulk.ColumnMappings.Add("Mail visits", "Mail_visits");
      objbulk.ColumnMappings.Add("Male distribution", "Male_distribution");
      objbulk.ColumnMappings.Add("Mobile traffic share", "Mobile_Share");
      objbulk.ColumnMappings.Add("Mobile web bounce rate", "Mobile_traffic_share");
      objbulk.ColumnMappings.Add("Mobile web monthly traffic", "Mobile_web_monthly_traffic");
      objbulk.ColumnMappings.Add("Mobile web pages/visit", "Mobile_web_pages_visit");
      objbulk.ColumnMappings.Add("Mobile web total page views", "Mobile_web_total_page_views");
      objbulk.ColumnMappings.Add("Mobile web unique visitors", "Mobile_web_unique_visitors");
      objbulk.ColumnMappings.Add("Mobile web visit duration", "Mobile_web_visit_duration");
      objbulk.ColumnMappings.Add("MoM Desktop page views change", "MoM_Desktop_page_views_change");
      objbulk.ColumnMappings.Add("MoM Desktop Traffic Change", "MoM_Desktop_Traffic_Change");
      objbulk.ColumnMappings.Add("MoM desktop unique visitors change", "MoM_desktop_unique_visitors_change");
      objbulk.ColumnMappings.Add("MoM direct traffic change", "MoM_direct_traffic_change");
      objbulk.ColumnMappings.Add("MoM display traffic change", "MoM_display_traffic_change");
      objbulk.ColumnMappings.Add("MoM mail visits change", "MoM_mail_visits_change");
      objbulk.ColumnMappings.Add("MoM mobile web page views change", "MoM_mobile_web_page_views_change");
      objbulk.ColumnMappings.Add("MoM mobile web traffic change", "MoM_mobile_web_traffic_change");
      objbulk.ColumnMappings.Add("MoM mobile web unique visitors change", "MoM_mobile_web_unique_visitors_change");
      objbulk.ColumnMappings.Add("MoM organic search change", "MoM_organic_search_change");
      objbulk.ColumnMappings.Add("MoM paid search change", "MoM_paid_search_change");
      objbulk.ColumnMappings.Add("MoM referrals visits change", "MoM_referrals_visits_change");
      objbulk.ColumnMappings.Add("MoM social visits change", "MoM_social_visits_change");
      objbulk.ColumnMappings.Add("MoM total page views change", "MoM_total_page_views_change");
      objbulk.ColumnMappings.Add("MoM traffic change", "MoM_traffic_change");
      objbulk.ColumnMappings.Add("MoM unique visitors change", "MoM_unique_visitors_change");
      objbulk.ColumnMappings.Add("Monthly desktop traffic", "Monthly_desktop_traffic");
      objbulk.ColumnMappings.Add("Monthly visits", "Monthly_Visits");
      objbulk.ColumnMappings.Add("Organic search share", "Organic_search_share");
      objbulk.ColumnMappings.Add("Organic search visits", "Organic_search_visits");
      objbulk.ColumnMappings.Add("Pages/Visit", "Pages_Visit_1");
      objbulk.ColumnMappings.Add("Paid search share", "Paid_search_share");
      objbulk.ColumnMappings.Add("Paid search visits", "Paid_search_visits");
      objbulk.ColumnMappings.Add("Phone number", "Phone_number");
      objbulk.ColumnMappings.Add("Rank", "Rank");
      objbulk.ColumnMappings.Add("Referral visits", "Referral_visits");
      objbulk.ColumnMappings.Add("Referrals share", "Referrals_share");
      objbulk.ColumnMappings.Add("Revenue", "Revenue");
      objbulk.ColumnMappings.Add("Social share", "Social_share");
      objbulk.ColumnMappings.Add("Social visits", "Social_visits");
      objbulk.ColumnMappings.Add("Top country", "Top_country");
      objbulk.ColumnMappings.Add("Total page views", "Total_page_views");
      objbulk.ColumnMappings.Add("Unique visits", "Unique_visits");
      objbulk.ColumnMappings.Add("Visit duration", "Visit_Duration");
      objbulk.ColumnMappings.Add("Website category", "Website_category");
      objbulk.ColumnMappings.Add("Website type", "Website_type");
      objbulk.ColumnMappings.Add("YoY desktop page views change", "YoY_desktop_page_views_change");
      objbulk.ColumnMappings.Add("YoY desktop traffic change", "YoY_desktop_traffic_change");
      objbulk.ColumnMappings.Add("YoY desktop unique visitors change", "YoY_desktop_unique_visitors_change");
      objbulk.ColumnMappings.Add("YoY Direct Traffic Change", "YoY_Direct_Traffic_Change");
      objbulk.ColumnMappings.Add("YoY display visits change", "YoY_display_visits_change");
      objbulk.ColumnMappings.Add("YoY mail visits change", "YoY_mail_visits_change");
      objbulk.ColumnMappings.Add("YoY mobile web page views change", "YoY_mobile_web_page_views_change");
      objbulk.ColumnMappings.Add("YoY mobile web traffic change", "YoY_mobile_web_traffic_change");
      objbulk.ColumnMappings.Add("YoY mobile web unique visitors change", "YoY_mobile_web_unique_visitors_change");
      objbulk.ColumnMappings.Add("YoY organic search change", "YoY_organic_search_change");
      objbulk.ColumnMappings.Add("YoY paid search change", "YoY_paid_search_change");
      objbulk.ColumnMappings.Add("YoY referrals visits change", "YoY_referrals_visits_change");
      objbulk.ColumnMappings.Add("YoY social visits change", "YoY_social_visits_change");
      objbulk.ColumnMappings.Add("YoY total page views change", "YoY_total_page_views_change");
      objbulk.ColumnMappings.Add("YoY traffic change", "YoY_traffic_change");
      objbulk.ColumnMappings.Add("YoY unique visitors change", "YoY_unique_visitors_change");
      objbulk.ColumnMappings.Add("UploadId", "UploadId");

      //objbulk.BatchSize = 10000;
      objbulk.BulkCopyTimeout = 0;
      con.Open();
      //insert bulk Records into DataBase.  
      objbulk.WriteToServer(tempDataSet.Tables[0]);
      con.Close();
      db.SaveChanges();//If any error during file upload, the file name should not be saved that why this is put in the end
      //ParseAPI();
      //Call APIFUNCTION inside Bulk (in LaSt)
      //var task = Task.Run(ParseAPI);
      Task.Run(ParseAPI);
      //ParseAPI();


    }
    public void ParseAPI()
    {
      //https://api.hunter.io/v2/domain-search?domain=stripe.com&api_key=2c170b8aa69a1c0e87cb25dfdade18b3b4bf25e7

      string SiteName = string.Empty;//"stripe.com";
      string API_URL = string.Empty;
      Recipient recipient = new Recipient();
      List<Recipient> LstRecipients = new List<Recipient>();
      foreach (DataRow dr in tempDataSet.Tables[0].Rows)
      {
        if (!(dr["Domain"] == DBNull.Value) && !(string.IsNullOrEmpty(dr["Domain"].ToString())))
        {
          SiteName = dr["Domain"].ToString().Trim();
          API_URL = "https://api.hunter.io/v2/domain-search?domain=" + SiteName + "&api_key=2c170b8aa69a1c0e87cb25dfdade18b3b4bf25e7";

          //------------please parse this Jason and save to Recipient table
          using (var client = new HttpClient())
          {

            client.BaseAddress = new Uri("https://api.hunter.io/v2/");
            var responseTask = client.GetAsync("domain-search?domain=" + SiteName + "&api_key=2c170b8aa69a1c0e87cb25dfdade18b3b4bf25e7");
            responseTask.Wait();
            var Res = responseTask.Result;
            if (Res.IsSuccessStatusCode)
            {
              var response = Res.Content.ReadAsStringAsync().Result;
              var obj = JsonConvert.DeserializeObject<ParentJSONTable>(response);
              for (int o = 0; o < obj.data.emails.Count; o++)
              {
                var newRec = new Recipient();
                newRec.Id = Guid.NewGuid().ToString();
                newRec.Domain = obj.data.domain;
                newRec.FirstName = obj.data.emails[o].first_name;
                newRec.LastName = obj.data.emails[o].last_name;
                newRec.Department = obj.data.emails[o].department;
                newRec.Position = obj.data.emails[o].position;
                newRec.Email = obj.data.emails[o].value;
                LstRecipients.Add(newRec);
              }


            }

          }


          //recipient = new Recipient { Id=Guid.NewGuid().ToString(),Department="", Domain="", Email="", FirstName="", LastName="", PhoneNumber="", Position="" };
        }
      }
      using (var dbContext = new jsaosorioEntities())
      {
        dbContext.Recipients.AddRange(LstRecipients);
        dbContext.SaveChanges();

      }

    }



    public class ParentJSONTable
    {
      public ChildJSONTable data { get; set; }
    }
    public class ChildJSONTable
    {
      public string domain { get; set; }
      public List<JSONRecords> emails { get; set; }
    }
    public class JSONRecords
    {
      public string value { get; set; }
      public string first_name { get; set; }
      public string last_name { get; set; }
      public string department { get; set; }
      public string phone_number { get; set; }
      public string position { get; set; }
    }



    [Authorize(Roles = "Admin,Manager,User")]
    public async Task<ActionResult> SearchDNC(string DNCPhone, string SupplierId)
    {
      ViewBag.Message1 = TempData["Message1"] as string;
      ViewBag.FileName = TempData["FileName"];
      string LoggedInUserId = User.Identity.GetUserId();
      if (string.IsNullOrEmpty(DNCPhone))
      {

        //return View();
        return View(await db.BaseDailies.Where(x => x.AspNetUserId == LoggedInUserId).Take(10).ToListAsync());
        //return View(await db.BaseDailies.Where(x => x.AspNetUserId == LoggedInUserId).ToListAsync());
        //var baseDailies = db.BaseDailies.Include(b => b.AspNetUser);
        //return View(await baseDailies.ToListAsync());
      }
      else
      {


        //int suppId = int.Parse(SupplierId);
        var dNCs = db.BaseDailies.Where(x => x.AspNetUserId == LoggedInUserId && (x.AdSense.Contains(DNCPhone) ||
                                                                        x.Ages_18_24.Contains(DNCPhone) ||
                                                                        x.Ages_25_34.Contains(DNCPhone) ||
                                                                        x.Ages_35_44.Contains(DNCPhone) ||
                                                                        x.Ages_45_54.Contains(DNCPhone) ||
                                                                        x.Ages_55_64.Contains(DNCPhone) ||
                                                                        x.Ages_65_Plus.Contains(DNCPhone) ||
                                                                        x.Bounce_Rate.Contains(DNCPhone) ||
                                                                        x.Company_name.Contains(DNCPhone) ||
                                                                        x.Country.Contains(DNCPhone) ||
                                                                        x.Desktop_bounce_rate.Contains(DNCPhone) ||
                                                                        x.Desktop_pages_per_visit.Contains(DNCPhone) ||
                                                                        x.Desktop_total_page_views.Contains(DNCPhone) ||
                                                                        x.Desktop_traffic_share.Contains(DNCPhone) ||
                                                                        x.Desktop_unique_visitors.Contains(DNCPhone) ||
                                                                        x.Desktop_visit_duration.Contains(DNCPhone) ||
                                                                        x.Direct_traffic.Contains(DNCPhone) ||
                                                                        x.Direct_traffic_share.Contains(DNCPhone) ||
                                                                        x.Display_ad_traffic.Contains(DNCPhone) ||
                                                                        x.Display_ads_traffic_share.Contains(DNCPhone) ||
                                                                        x.Domain.Contains(DNCPhone) ||
                                                                        x.Email_address.Contains(DNCPhone) ||
                                                                        x.Employees.Contains(DNCPhone) ||
                                                                        x.Female_distribution.Contains(DNCPhone) ||
                                                                        x.HQ_address.Contains(DNCPhone) ||
                                                                        x.HQ_city.Contains(DNCPhone) ||
                                                                        x.HQ_country.Contains(DNCPhone) ||
                                                                        x.HQ_postal_code.Contains(DNCPhone) ||
                                                                        x.HQ_state.Contains(DNCPhone) ||
                                                                        x.Mail_share.Contains(DNCPhone) ||
                                                                        x.Mail_visits.Contains(DNCPhone) ||
                                                                        x.Male_distribution.Contains(DNCPhone) ||
                                                                        x.Mobile_Share.Contains(DNCPhone) ||
                                                                        x.Mobile_traffic_share.Contains(DNCPhone) ||
                                                                        x.Mobile_web_monthly_traffic.Contains(DNCPhone) ||
                                                                        x.Mobile_web_pages_visit.Contains(DNCPhone) ||
                                                                        x.Mobile_web_total_page_views.Contains(DNCPhone) ||
                                                                        x.Mobile_web_unique_visitors.Contains(DNCPhone) ||
                                                                        x.Mobile_web_visit_duration.Contains(DNCPhone) ||
                                                                        x.MoM_Desktop_page_views_change.Contains(DNCPhone) ||
                                                                        x.MoM_Desktop_Traffic_Change.Contains(DNCPhone) ||
                                                                        x.MoM_desktop_unique_visitors_change.Contains(DNCPhone) ||
                                                                        x.MoM_direct_traffic_change.Contains(DNCPhone) ||
                                                                        x.MoM_display_traffic_change.Contains(DNCPhone) ||
                                                                        x.MoM_mail_visits_change.Contains(DNCPhone) ||
                                                                        x.MoM_mobile_web_page_views_change.Contains(DNCPhone) ||
                                                                        x.MoM_mobile_web_traffic_change.Contains(DNCPhone) ||
                                                                        x.MoM_mobile_web_unique_visitors_change.Contains(DNCPhone) ||
                                                                        x.MoM_organic_search_change.Contains(DNCPhone) ||
                                                                        x.MoM_paid_search_change.Contains(DNCPhone) ||
                                                                        x.MoM_referrals_visits_change.Contains(DNCPhone) ||
                                                                        x.MoM_social_visits_change.Contains(DNCPhone) ||
                                                                        x.MoM_total_page_views_change.Contains(DNCPhone) ||
                                                                        x.MoM_traffic_change.Contains(DNCPhone) ||
                                                                        x.MoM_unique_visitors_change.Contains(DNCPhone) ||
                                                                        x.Monthly_desktop_traffic.Contains(DNCPhone) ||
                                                                        x.Monthly_Visits.Contains(DNCPhone) ||
                                                                        x.Organic_search_share.Contains(DNCPhone) ||
                                                                        x.Organic_search_visits.Contains(DNCPhone) ||
                                                                        x.Pages_Visit_1.Contains(DNCPhone) ||
                                                                        x.Paid_search_share.Contains(DNCPhone) ||
                                                                        x.Paid_search_visits.Contains(DNCPhone) ||
                                                                        x.Phone_number.Contains(DNCPhone) ||
                                                                        x.Rank.Contains(DNCPhone) ||
                                                                        x.Referral_visits.Contains(DNCPhone) ||
                                                                        x.Referrals_share.Contains(DNCPhone) ||
                                                                        x.Revenue.Contains(DNCPhone) ||
                                                                        x.Social_share.Contains(DNCPhone) ||
                                                                        x.Social_visits.Contains(DNCPhone) ||
                                                                        x.Top_country.Contains(DNCPhone) ||
                                                                        x.Total_page_views.Contains(DNCPhone) ||
                                                                        x.Unique_visits.Contains(DNCPhone) ||
                                                                        x.Visit_Duration.Contains(DNCPhone) ||
                                                                        x.Website_category.Contains(DNCPhone) ||
                                                                        x.Website_type.Contains(DNCPhone) ||
                                                                        x.YoY_desktop_page_views_change.Contains(DNCPhone) ||
                                                                        x.YoY_desktop_traffic_change.Contains(DNCPhone) ||
                                                                        x.YoY_desktop_unique_visitors_change.Contains(DNCPhone) ||
                                                                        x.YoY_Direct_Traffic_Change.Contains(DNCPhone) ||
                                                                        x.YoY_display_visits_change.Contains(DNCPhone) ||
                                                                        x.YoY_mail_visits_change.Contains(DNCPhone) ||
                                                                        x.YoY_mobile_web_page_views_change.Contains(DNCPhone) ||
                                                                        x.YoY_mobile_web_traffic_change.Contains(DNCPhone) ||
                                                                        x.YoY_mobile_web_unique_visitors_change.Contains(DNCPhone) ||
                                                                        x.YoY_organic_search_change.Contains(DNCPhone) ||
                                                                        x.YoY_paid_search_change.Contains(DNCPhone) ||
                                                                        x.YoY_referrals_visits_change.Contains(DNCPhone) ||
                                                                        x.YoY_social_visits_change.Contains(DNCPhone) ||
                                                                        x.YoY_total_page_views_change.Contains(DNCPhone) ||
                                                                        x.YoY_traffic_change.Contains(DNCPhone) ||
                                                                        x.YoY_unique_visitors_change.Contains(DNCPhone)
                                                                        )).AsQueryable();
        //TempData["BaseDailyFilteredTable"] = await dNCs.ToListAsync();
        return PartialView("_SearchDNC", await dNCs.ToListAsync());


      }
    }

    /////// DOWNLOAD system.linq.dynamic Nuget Package
    [Authorize(Roles = "Admin,Manager,User")]
    public ActionResult DNCSearch()
    {

      return View();
    }

    [HttpPost]
    public ActionResult GetDNCList()
    {
      //Server Side Parameter
      int start = Convert.ToInt32(Request["start"]);
      int length = Convert.ToInt32(Request["length"]);
      string searchValue = Request["search[value]"];
      string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
      string sortDirection = Request["order[0][dir]"];

      List<BaseDaily> DNCList = new List<BaseDaily>();
      db.Configuration.ProxyCreationEnabled = false;
      DNCList = db.BaseDailies.Include("Upload").ToList<BaseDaily>();

      var result = new List<BaseDaily2>();
      for (int i = 0; i < DNCList.Count; i++)
      {
        var newRecord = new BaseDaily2();

        //newRecord.IsSelected = DNCList[i].IsSelected;
        newRecord.Domain = DNCList[i].Domain;
        newRecord.deal = DNCList[i].deal;
        newRecord.agent = DNCList[i].agent;
        newRecord.Cor = DNCList[i].Cor;
        newRecord.Neg = DNCList[i].Neg;
        newRecord.date = DNCList[i].date;
        newRecord.type = DNCList[i].type;
        newRecord.datechange = DNCList[i].datechange;
        //newRecord.other1 = DNCList[i].other1;
        //newRecord.other2 = DNCList[i].other2;
        //newRecord.other3 = DNCList[i].other3;
        newRecord.Traffic_Share = DNCList[i].Traffic_Share;
        newRecord.Change = DNCList[i].Change;
        newRecord.Rank = DNCList[i].Rank;
        newRecord.Monthly_Visits = DNCList[i].Monthly_Visits;
        newRecord.FileName = DNCList[i].Upload.FileName;
        result.Add(newRecord);
      }


      int totalrows = result.Count;
      if (!string.IsNullOrEmpty(searchValue))//filter
      {
        result = result.
            Where(x => (x.Domain == null ? "" : x.Domain.ToLower()).Contains(searchValue.ToLower()) ||
            (x.deal == null ? "" : x.deal.ToLower()).Contains(searchValue.ToLower())
               || (x.agent == null ? "" : x.agent.ToLower()).Contains(searchValue.ToLower())
               || (x.Cor == null ? "" : x.Cor.ToLower()).Contains(searchValue.ToLower())
               || (x.Neg == null ? "" : x.Neg.ToLower()).Contains(searchValue.ToLower())
              || (x.date == null ? "" : x.date.ToLower()).Contains(searchValue.ToLower())
             || (x.type == null ? "" : x.type.ToLower()).Contains(searchValue.ToLower())
             || (x.datechange == null ? "" : x.datechange.ToLower()).Contains(searchValue.ToLower())
              //|| (x.other1 == null ? "" : x.other1.ToLower()).Contains(searchValue.ToLower())
              //|| (x.other2 == null ? "" : x.other2.ToLower()).Contains(searchValue.ToLower())
              // || (x.other3 == null ? "" : x.other3.ToLower()).Contains(searchValue.ToLower())
              || (x.Traffic_Share == null ? "" : x.Traffic_Share.ToLower()).Contains(searchValue.ToLower())
               || (x.Change == null ? "" : x.Change.ToLower()).Contains(searchValue.ToLower())
             || (x.Rank == null ? "" : x.Rank.ToLower()).Contains(searchValue.ToLower())
             || (x.Monthly_Visits == null ? "" : x.Monthly_Visits.ToLower()).Contains(searchValue.ToLower())
             || (x.FileName == null ? "" : x.FileName.ToLower()).Contains(searchValue.ToLower()))
            .ToList<BaseDaily2>();
      }
      int totalrowsafterfiltering = result.Count;
      //sorting
      result = result.OrderBy(sortColumnName + " " + sortDirection).ToList<BaseDaily2>();
      //paging
      result = result.Skip(start).Take(length).ToList<BaseDaily2>();

      return Json(new { data = result, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);




    }

    public ActionResult EmailSearch()
    {

      return View();
    }

    [HttpPost]
    public ActionResult GetEmailsList()
    {
      //Server Side Parameter
      int start = Convert.ToInt32(Request["start"]);
      int length = Convert.ToInt32(Request["length"]);
      string searchValue = Request["search[value]"];
      string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
      string sortDirection = Request["order[0][dir]"];

      List<BaseDaily> DNCList = new List<BaseDaily>();
      db.Configuration.ProxyCreationEnabled = false;
      DNCList = db.BaseDailies.Where(x => x.IsHidden == false).Include("Upload").ToList<BaseDaily>();

      var result = new List<BaseDaily2>();
      for (int i = 0; i < DNCList.Count; i++)
      {
        var newRecord = new BaseDaily2();

        //newRecord.IsSelected = DNCList[i].IsSelected;
        newRecord.Domain = DNCList[i].Domain;
        newRecord.deal = DNCList[i].deal;
        newRecord.agent = DNCList[i].agent;
        newRecord.Cor = DNCList[i].Cor;
        newRecord.Neg = DNCList[i].Neg;
        newRecord.date = DNCList[i].date;
        newRecord.type = DNCList[i].type;
        newRecord.datechange = DNCList[i].datechange;
        //newRecord.other1 = DNCList[i].other1;
        //newRecord.other2 = DNCList[i].other2;
        //newRecord.other3 = DNCList[i].other3;
        newRecord.Traffic_Share = DNCList[i].Traffic_Share;
        newRecord.Change = DNCList[i].Change;
        newRecord.Rank = DNCList[i].Rank;
        newRecord.Monthly_Visits = DNCList[i].Monthly_Visits;
        newRecord.FileName = DNCList[i].Upload.FileName;
        result.Add(newRecord);
      }


      int totalrows = result.Count;
      if (!string.IsNullOrEmpty(searchValue))//filter
      {
        result = result.
            Where(x => (x.Domain == null ? "" : x.Domain.ToLower()).Contains(searchValue.ToLower()) ||
            (x.deal == null ? "" : x.deal.ToLower()).Contains(searchValue.ToLower())
               || (x.agent == null ? "" : x.agent.ToLower()).Contains(searchValue.ToLower())
               || (x.Cor == null ? "" : x.Cor.ToLower()).Contains(searchValue.ToLower())
               || (x.Neg == null ? "" : x.Neg.ToLower()).Contains(searchValue.ToLower())
              || (x.date == null ? "" : x.date.ToLower()).Contains(searchValue.ToLower())
             || (x.type == null ? "" : x.type.ToLower()).Contains(searchValue.ToLower())
             || (x.datechange == null ? "" : x.datechange.ToLower()).Contains(searchValue.ToLower())
              //|| (x.other1 == null ? "" : x.other1.ToLower()).Contains(searchValue.ToLower())
              //|| (x.other2 == null ? "" : x.other2.ToLower()).Contains(searchValue.ToLower())
              // || (x.other3 == null ? "" : x.other3.ToLower()).Contains(searchValue.ToLower())
              || (x.Traffic_Share == null ? "" : x.Traffic_Share.ToLower()).Contains(searchValue.ToLower())
               || (x.Change == null ? "" : x.Change.ToLower()).Contains(searchValue.ToLower())
             || (x.Rank == null ? "" : x.Rank.ToLower()).Contains(searchValue.ToLower())
             || (x.Monthly_Visits == null ? "" : x.Monthly_Visits.ToLower()).Contains(searchValue.ToLower())
             || (x.FileName == null ? "" : x.FileName.ToLower()).Contains(searchValue.ToLower()))
            .ToList<BaseDaily2>();
      }
      int totalrowsafterfiltering = result.Count;
      //sorting
      result = result.OrderBy(sortColumnName + " " + sortDirection).ToList<BaseDaily2>();
      //paging
      result = result.Skip(start).Take(length).ToList<BaseDaily2>();

      return Json(new { data = result, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);




    }

    //public ActionResult GetSelectedRows(List<string> obj)
    //{
    //  var result = new List<SelectedRows>();
    //  for (int i = 0; i < obj.Count; i++)
    //  {
    //    var myData = new SelectedRows();
    //    myData.Domain = obj[i];
    //    myData.Emails = db.Recipients.ToList().Where(x => x.Domain == obj[i]).Select(x => x.Email).ToList();

    //    result.Add(myData);
    //  }
    //  return Json(db.Recipients.ToList(), JsonRequestBehavior.AllowGet);
    //}

    [HttpPost]
    public ActionResult GetSelectedRows(List<string> obj)
    {
      List<Recipient> recipients = new List<Recipient>();
      List<Recipient> LstRecipients = new List<Recipient>();
      SelectedRow selectedRow;// = new SelectedRow();
      string email = string.Empty;
      //List<string> LstEmail; //= new List<string>();
      var LstSelectedRows = new List<SelectedRow>();
      if (obj != null)
      {
        foreach (string item in obj)
        {
          recipients = db.Recipients.Where(x => x.Domain == item && x.IsCustom!=true).ToList();
          if (recipients.Count() != 0)
          {
            LstRecipients.AddRange(recipients);
          }
          else
          {
            Recipient recipient= db.Recipients.FirstOrDefault(x => x.Domain == item && x.IsCustom == true);
            if (recipient!=null)
            {
              selectedRow = new SelectedRow { Domain = item, IsCustom = true, DefaultEmail = recipient.Email };
            }
            else
            {
              selectedRow = new SelectedRow { Domain = item, IsCustom = true};
            }
            
            LstSelectedRows.Add(selectedRow);
          }
        }

        foreach (Recipient item in LstRecipients.ToList())
        {

          if (item.FirstName != null && item.LastName != null) item.Email += ", " + item.FirstName + " " + item.LastName;
          db.Entry(item).State = EntityState.Detached;
        }

        var query = LstRecipients.GroupBy(x => x.Domain);

        foreach (var item in query)
        {
          //LstEmail = item.Select(x => x.Email).ToList();//.ToList<Recipient>();
          //Recipient recipient = (Recipient)item;
          string DefaultValue = null;
          Recipient recipient = item.FirstOrDefault(x => x.IsDefault == true);
          if (recipient != null) DefaultValue = recipient.Email;

           selectedRow = new SelectedRow { Domain = item.Key, Emails = item.Select(x => x.Email).ToList(),DefaultEmail= DefaultValue };
          LstSelectedRows.Add(selectedRow);
          List<BaseDaily> LstBaseDailies = db.BaseDailies.Where(x => x.Domain == item.Key).ToList();
          foreach (BaseDaily baseDaily in LstBaseDailies)
          {
            baseDaily.other2 = "true";
            db.Entry(baseDaily).State = EntityState.Modified;
            db.Entry(baseDaily).Property(x => x.other2).IsModified = true;

          }

        }
        db.SaveChanges();
      }
      //foreach (Recipient recipient in recipients)
      //  {
      //    if (LstSelectedRows.FirstOrDefault(x => x.Domain == recipient.Domain) == null)
      //    {
      //      //foreach (string email in recipient.Email)
      //      //{
      //      //  new SelectedRow { Domain = recipient.Domain };

      //      //  LstSelectedRows.Add(new se);
      //      //}
      //    }
      //  }


      //return Json(db.Recipients.ToList(), JsonRequestBehavior.AllowGet);
      //return Json(result, JsonRequestBehavior.AllowGet);
      return PartialView("_EmailSearch", LstSelectedRows);
    }
    public ActionResult DeleteSelected(List<string> obj)
    //public ActionResult UploadAndSend(List<MyList> fileData, List<MyList> selectedEmails)
    {
      if (obj != null)
      {
        foreach (string item in obj)
        {
          List<BaseDaily> LstBaseDaily = db.BaseDailies.Where(x => x.Domain == item).ToList();
          foreach (BaseDaily baseDaily in LstBaseDaily)
          {
            baseDaily.IsHidden = true;
            db.Entry(baseDaily).State = EntityState.Modified;
            db.Entry(baseDaily).Property(x => x.IsHidden).IsModified = true;
          }

        }
        db.SaveChanges();
      }

      //return RedirectToAction("EmailSearch");
      return Json(new { success = false, responseText = "Success" }, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult SetDefault(string domain, string email)
    {
      List<Recipient> LstRecipient = db.Recipients.Where(x => x.Domain == domain).ToList();
      Recipient recipient;
      if (LstRecipient != null && LstRecipient.Count > 0 && LstRecipient[0].IsCustom!=true)
      {

        // Marking IsDefault 0 to all emails that has same domain
        for (int i = 0; i < LstRecipient.Count; i++)
        {
          LstRecipient[i].IsDefault = false;
          db.Entry(LstRecipient[i]).State = EntityState.Modified;
          //db.SaveChanges();
        }

        email = email.Split(',').FirstOrDefault();
        recipient = db.Recipients.FirstOrDefault(x => x.Email == email);
        if (recipient != null)
        {
          recipient.IsDefault = true;
          db.Entry(recipient).State = EntityState.Modified;
          db.Entry(recipient).Property(x => x.IsDefault).IsModified = true;
        }
        else
        {//create new
          recipient = new Recipient { Id = Guid.NewGuid().ToString(), Domain = domain, Email = email, IsDefault = true, IsCustom = true };
          db.Recipients.Add(recipient);

        }

      }
      else
      {//create new
        email = email.Split(',').FirstOrDefault();
        recipient = db.Recipients.FirstOrDefault(x => x.IsCustom==true && ( x.Email == email || x.Domain==domain) );
        if (recipient != null)
        {
          recipient.IsDefault = true;
          recipient.Email = email;
          db.Entry(recipient).State = EntityState.Modified;
          db.Entry(recipient).Property(x => x.IsDefault).IsModified = true;
        }
        else
        {//create new
          recipient = new Recipient { Id = Guid.NewGuid().ToString(), Domain = domain, Email = email, IsDefault = true, IsCustom = true };
          db.Recipients.Add(recipient);

        }


      }
      db.SaveChanges();

      //return RedirectToAction("EmailSearch");
      return Json(new { success = false, responseText = "Success" }, JsonRequestBehavior.AllowGet);
    }
    public class MyList
    {
      public int Id { get; set; }
      public string Value { get; set; }
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult UploadAndSend()
    //public ActionResult UploadAndSend(List<MyList> fileData, List<MyList> selectedEmails)
    {
      if (Request.Files.Count > 0)
      {
        try
        {
          string emailBodyText = string.Empty;
          var emailLists = Request["emails"].ToString();
          var LstemailsWithName = new List<string>();
          var Lstemails = new List<string>();
          if (!string.IsNullOrEmpty(emailLists) && emailLists != "[]")
          {
            LstemailsWithName = JsonConvert.DeserializeObject<List<string>>(emailLists);
            foreach (string email in LstemailsWithName)
            {
              Lstemails.Add(email.Split(',').FirstOrDefault());
            }
          }
          else
          {
            return Json("No files selected.");
          }

          HttpFileCollectionBase postedFiles = Request.Files;
          HttpPostedFileBase postedFile = postedFiles[0];

          string filePath = string.Empty;
          if (postedFile != null)
          {
            string path = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
              Directory.CreateDirectory(path);
            }

            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            postedFile.SaveAs(filePath);

            //Read the contents of CSV file.
            emailBodyText = System.IO.File.ReadAllText(filePath);

          }
          SendEmail.SendEmailUsingSMTP(Lstemails, emailBodyText);
          foreach (string email in Lstemails)
          {
            string domainName = email.Split('@').ToList().Last();

            List<BaseDaily> LstBaseDaily = db.BaseDailies.Where(x => x.Domain == domainName).ToList();

            foreach (BaseDaily baseDaily in LstBaseDaily)
            {
              baseDaily.other2 = null;
              db.Entry(baseDaily).State = EntityState.Modified;
              db.Entry(baseDaily).Property(x => x.other2).IsModified = true;
            }
          }
          db.SaveChanges();
          //return PartialView("_DisplayAnnualLeaves", annualLeaves);
          //return View();
          return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
          //

          //return PartialView("_DisplayAnnualLeaves");
        }
        catch (Exception ex)
        {
          return Json("Error occurred. Error details: " + ex.Message);
        }
      }
      else
      {
        //return Json("No files selected.");
        return Json(new { success = false, responseText = "No files selected." }, JsonRequestBehavior.AllowGet);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
