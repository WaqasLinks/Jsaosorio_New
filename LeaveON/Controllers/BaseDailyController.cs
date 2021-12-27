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

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class BaseDailyController : Controller
  {
    //private CallCenterSalesEntities db = new CallCenterSalesEntities();
    private jsaosorioEntities db = new jsaosorioEntities();
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
      string FileName=string.Empty;
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
      return RedirectToAction("SearchDNC");
      //return View("SearchDNC");

    }
    private void BulkInsert(DateTime dateTimeNow, int SupplierId, String FileName)
    {
      string UploadId = Guid.NewGuid().ToString();
      db.Uploads.Add(new Upload() {Id= UploadId, FileName = FileName, DateCreated = dateTimeNow });
      

      DataSet tempDataSet;

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
