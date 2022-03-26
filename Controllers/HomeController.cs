using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using cSharpDemo.Models;
using dbChange.Repository;

namespace cSharpDemo.Controllers;

public class HomeController : Controller
{
    private readonly IEmployeeRepository _repository;
    public HomeController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        _repository.GetAllEmployees();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetEmployees()
    {
        return Ok(_repository.GetAllEmployees());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
