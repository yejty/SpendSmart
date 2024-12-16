using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;

namespace SpendSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SpendSmartDbContext _context; //dependency injection, aby sme mohli ukladat do databazy

        public HomeController(ILogger<HomeController> logger, SpendSmartDbContext context)
        {
            _logger = logger;
            _context = context; //to tu prislo ako dependency injection, ked sa zacne aplikacia, context je injected do HomeControlleru, ked sa vytvori
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Expenses()
        {
            var allExpenses = _context.Expenses.ToList();
            var totalExpenses = allExpenses.Sum(x => x.Value);
            ViewBag.Expenses = totalExpenses;
            return View(allExpenses);
        }
        //nemozeme mat 2 prvky s rovnakym id v sql datazbazach
        public IActionResult CreateEditExpense(int? id) {
           
            if (id != null)
            {
                //editing -> load
                var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
                return View(expenseInDb);
            }
            
            return View();
        }
        public IActionResult DeleteExpense(int id)
        {
            var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id); //toto ide cez list expenses a hlada podla query id
            _context.Expenses.Remove(expenseInDb); //toto odstrani tu entitu, preto sa to vola EntityFramework
            _context.SaveChanges();
            return RedirectToAction("Expenses");
        }

        public IActionResult CreateEditExpenseForm(Expense model) {

            if (model.Id == 0) 
            {
                //create
                _context.Expenses.Add(model);
               
            } else
            {
                //edit
                _context.Expenses.Update(model);
            }
            _context.SaveChanges(); //add nestaci, musime save changes vzdy pri praci s databazou
            return RedirectToAction("Expenses");

        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
