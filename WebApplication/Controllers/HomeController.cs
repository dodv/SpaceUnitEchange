using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpaceTransfer;
using WebApplication.Models;


namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        

        public ActionResult Index()
        {
            var ViewModel = new ViewModel() { ListCurrencyUnit = SpaceTrading.Instance.GetAllCurrencyUnit(), ListItemTrading = SpaceTrading.Instance.GetAllItemsTrading() };

            return View(ViewModel);
        }

        [HttpPost]
        public ActionResult Index(FormCollection forms)
        {
            var model = new ViewModel() {
                                            InputString = forms["query"],
                                            ListCurrencyUnit = SpaceTrading.Instance.GetAllCurrencyUnit(),
                                            ListItemTrading = SpaceTrading.Instance.GetAllItemsTrading(),
                                            Status = false,
                                            
                                        };
            try
            {
                var exchangeResult = new ExchangeResult();
                if (!string.IsNullOrEmpty(forms["query"]))
                {
                    exchangeResult = SpaceTrading.Instance.ExchangeSpaceCredits(forms["query"]);
                }
                model.ListCurrencyUnit = SpaceTrading.Instance.GetAllCurrencyUnit();
                model.ListItemTrading = SpaceTrading.Instance.GetAllItemsTrading();
                model.Message = exchangeResult.Message;
                model.Result = exchangeResult.Result;
                model.Status = exchangeResult.Status;
                model.IsCredit = exchangeResult.IsCredit;
            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
            }
            return View("Index", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}