using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jsaosorio.Models;
using Microsoft.AspNet.Identity;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class BaseHistoricalsController : Controller
  {
    private jsaosorioEntities db = new jsaosorioEntities();

    // GET: BaseHistoricals
    public async Task<ActionResult> Index(string domain)
    {
      string DNCPhone = domain;
      string LoggedInUserId = User.Identity.GetUserId();
      //List<BaseDaily> BaseDailyFilteredTable = TempData["BaseDailyFilteredTable"] as List<BaseDaily>;
      //List<BaseDaily> BaseDailyFilteredTable = ;
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
      //return PartialView("_SearchDNC", await dNCs.ToListAsync());

      ViewBag.BaseDailyFilteredTable = await dNCs.ToListAsync();
      return View(await db.BaseHistoricals.Where(x => x.Domain == domain).ToListAsync());
    }

    // GET: BaseHistoricals/Details/5
    public async Task<ActionResult> Details(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      BaseHistorical baseHistorical = await db.BaseHistoricals.FindAsync(id);
      if (baseHistorical == null)
      {
        return HttpNotFound();
      }
      return View(baseHistorical);
    }

    // GET: BaseHistoricals/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: BaseHistoricals/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Domain,old,Current_Status,type,Super_Agency,datechange,new_data")] BaseHistorical baseHistorical)
    {
      if (ModelState.IsValid)
      {
        db.BaseHistoricals.Add(baseHistorical);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      return View(baseHistorical);
    }

    // GET: BaseHistoricals/Edit/5
    public async Task<ActionResult> Edit(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      BaseHistorical baseHistorical = await db.BaseHistoricals.FindAsync(id);
      if (baseHistorical == null)
      {
        return HttpNotFound();
      }
      return View(baseHistorical);
    }

    // POST: BaseHistoricals/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Domain,old,Current_Status,type,Super_Agency,datechange,new_data")] BaseHistorical baseHistorical)
    {
      if (ModelState.IsValid)
      {
        db.Entry(baseHistorical).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(baseHistorical);
    }

    // GET: BaseHistoricals/Delete/5
    public async Task<ActionResult> Delete(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      BaseHistorical baseHistorical = await db.BaseHistoricals.FindAsync(id);
      if (baseHistorical == null)
      {
        return HttpNotFound();
      }
      return View(baseHistorical);
    }

    // POST: BaseHistoricals/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(string id)
    {
      BaseHistorical baseHistorical = await db.BaseHistoricals.FindAsync(id);
      db.BaseHistoricals.Remove(baseHistorical);
      await db.SaveChangesAsync();
      return RedirectToAction("Index");
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
