using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Core2Request.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Core2Request.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()
    {
        //vendosim kushtin nese userid nga session eshte 0 pra nuk eshte regjistruar
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            //ktheje ne faqen e register pra coje te regjistrohet
            return RedirectToAction("Register");
        }
        int id = (int)HttpContext.Session.GetInt32("userId");
        //marrim emrin e userit te loguar nprm userid
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);

        // Marrim gjithe perdoruesit e tjere
        List<Request> rq = _context.Requests.Include(e => e.Reciver).Include(e => e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();
        // Ketu krijohet 1 liste, e cila merr te gjithe senderat dhe  receiverat dhe i filtron ne baze te receiverId
        var allUser = _context.Users
                        .Include(e => e.ReciverRequests)
                        .Include(e => e.SenderRequests)
                        .ToList();
        // e njejta gje dhe ketu krijon nje list userash qe ka reciverRequest, senderRequest dhe i filtron per userId(ku userid eshte e ndryshme nga id), reciverid dhe senderid. 
        List<User> LIST4 = _context.Users
                        .Include(e => e.ReciverRequests)
                        .Include(e => e.SenderRequests)
                        .Where(e => e.UserId != id)
                        .Where(e =>
                                     (e.SenderRequests.Any(f => f.ReciverId == id) == false)
                                    && (e.ReciverRequests.Any(f => f.SenderId == id) == false)
                        ).ToList();

        List<Request> miqte = _context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e => e.Reciver).Include(e => e.Sender).Where(e => e.Accepted == true).ToList();

        ViewBag.perdoruesit = LIST4;

        //shfaq postimin
        ViewBag.posts = _context.Posts.Include(e => e.Creator).ThenInclude(e => e.CreatedPost).Include(e => e.Likes).ThenInclude(e => e.UseriQePelqen).ToList();
        //  .Where(e => e.Creator.UserId != id).
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);
        // lista e filtruar ruhet ne viewbag
        ViewBag.perdoruesit = LIST4;
        // shfaqim gjith requests
        ViewBag.requests = _context.Requests.Include(e => e.Reciver).Include(e => e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();
        // shfaq gjith miqte
        ViewBag.miqte = _context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e => e.Reciver).Include(e => e.Sender).Where(e => e.Accepted == true).ToList();
        // Marr te loguarin me te dhena
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);
        return View();
    }

    [HttpGet("Send/{id}")]
    public IActionResult Send(int id) // int id eshte per te kap id e sendit
    {
        //krijojme id from session 
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        //bjm konstruktorin ku eshte by default 
        Request newRequest = new Models.Request()
        {
            //Id e senderit e merr nga idfromsession
            SenderId = idFromSession,
            //id e reciverit qe e mori
            ReciverId = id,
        };
        //shto request ne db, me cdo field qe permban ky request
        _context.Requests.Add(newRequest);
        // ruaji ato cka qe bem
        _context.SaveChanges();
        return RedirectToAction("index"); //Pas çdo ndryshimi me co me redirection ne index sepse pikerisht me redirection mund te editojme.
    }

    [HttpGet("Accept/{id}")]
    public IActionResult Accept(int id)
    {

        Request requestii = _context.Requests.First(e => e.RequestId == id);
        requestii.Accepted = true;
        _context.SaveChanges();
        return RedirectToAction("index");
    }

    [HttpGet("Decline/{id}")]
    public IActionResult Decline(int id)
    {
        Request requestii = _context.Requests.First(e => e.RequestId == id);
        _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("index");
    }

    [HttpGet("Remove/{id}")]
    public IActionResult Remove(int id)
    {
        Request requestii = _context.Requests.First(e => e.RequestId == id);
        _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet("/Post/Add")]
    public IActionResult PostAdd()
    {
        return View("PostAdd");
    }

    [HttpPost("/Post/Add")]
    public IActionResult PostAdd(Post marrNgaView)
    {
        if (ModelState.IsValid)
        {//ne menyre qe te shtohen nga i loguari dhe te kapet Id, fusim session me int id
            int id = (int)HttpContext.Session.GetInt32("userId");
            marrNgaView.UserId = id;
            _context.Posts.Add(marrNgaView);//i shtojme ne db
            _context.SaveChanges();// i savojme ne db
            return RedirectToAction("Index");
        }
        return View("PostAdd");
    }
    //tani punojme me "PostAdd.cshtml" dhe index.cshtml ku kapim postin me viewbag dhe e shtojme te index
    //pasi perfundojme na duhet te shtojme like, unlike dhe delete
    [HttpGet("/Post/Like/{id}")]
    public IActionResult LikeAdd(int id)
    {//krijojme id fromsession qe eshte id ime qe do pelqej
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        //krijojme instruktorin like ku id eshte by default, nga Key qe ka like(modeli) si klase
        Like likes = new Like()
        {
            //Id e pëlqyesit e merr nga idfromsession
            UserId = idFromSession,
            //id e postit qe pelqehet
            PostId = id,
        };
        //shton like ne databaze me id e vet si like, me id e userit qe e pëlqeu dhe id e postit që u pëlqye
        _context.Likes.Add(likes);
        //i savojme keto qe sapo bem
        _context.SaveChanges();
        //Pas çdo ndryshimi me co me redirection ne index
        //redirection perdoret kur duam te editojme, shtojme apo fshim pra nuk perdorim return view sepse nuk ruan ndryshim ne db
        //ndaj perdoret redirection pikerisht sepse bejme ndryshime
        return RedirectToAction("Index");
    }

    [HttpGet("/Post/Unlike/{id}/{PostId}")]
    public IActionResult Unlike(int id, int postId)
    {
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        Like unlike = _context.Likes.First(e => e.UserId == id && e.PostId == postId);
        _context.Likes.Remove(unlike);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet("/Post/Delete/{id}")]
    public IActionResult Fshi(int id)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        Post fshiPost = _context.Posts.First(e => e.PostId == id);
        _context.Posts.Remove(fshiPost);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return View();
        }
        return RedirectToAction("Index");
    }

    [HttpPost("Register")]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            //nqs ndonje user ne database (ku u kap username ) eshte e njejte me userin qe po logohet
            //ktheje prap tek registri dhe nxirrr errorin.
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                //errori kapet nga "Username" qe e merr nga Modeli, dhe pas presjes vendoset errori
                ModelState.AddModelError("Username", "already in use!");

                return View();
            }
            //hashohet paswordi para logimit
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            //kjo do te thote hasho paswordin e user duke e kap me user.Password.
            user.Password = Hasher.HashPassword(user, user.Password);
            //shton ne database te Users(qe e kemi tek mycontext) shton user nga Modeli User 
            _context.Users.Add(user);
            //ruan ndryshimet pasi i shtuam
            _context.SaveChanges();
            //vendoset session duke vendosur si kusht User id -> kapet me userId ne thonjza si te modeli, pas presjes.
            //user.UserId qe kontrollon UserId e e atij qe po logohet pra user qe krijuam te funksioni.
            HttpContext.Session.SetInt32("userId", user.UserId);
            //te ridrejton tek indeksi ku duam te logohemi
            return RedirectToAction("Index");
        }
        //perndryshe nese kushtet nuk jane plotesuar te con prap te register qe t'i fusesh dhe nje here te dhenat.
        return View();
    }

    [HttpPost("Login")]
    public IActionResult LoginSubmit(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            ////Nese Modeli eshte valid kerko nese ekziston i njejti username ne database qe nese eshte te logohet nese jo te registrohet tek registgri
            // If initial ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.Username == userSubmission.Username);
            // If no user exists with provided email
            if (userInDb == null)
            {
                //pra nqs ky qe po logohet nuk eshte ne database te regjisrohet se ndryshe s'futet.
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("User", "Invalid Username/Password");
                return View("Register");
            }
            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();

            // verify provided password against hash stored in db
            //verifikohet paswordi me paswordin e hashuar ne database
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
            // result can be compared to 0 for failure
            if (result == 0)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View("Register");
                // handle failure (this should be similar to how "existing email" is handled)
            }//pasi kalohet dhe ky kusht qe paswordi eshte i njejte vendoset session ne logim
            HttpContext.Session.SetInt32("userId", userInDb.UserId);
            return RedirectToAction("Index");
        }
        return View("Register");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        //ketu do bejme logout dhe i themi fshi session e me dergo ne faqen e register
        HttpContext.Session.Clear();
        return RedirectToAction("register");
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
