using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vote_box.Models;

namespace votebox.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(i => i.Email == user.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email already in use.");
                ViewBag.Error = "This email is already in use.";
                return View(user);
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(i => i.Email == email && i.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            return RedirectToAction("ViewPolls", "User");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> ViewPolls()
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            string userId = HttpContext.Session.GetString("UserId");

            var votedPollIds = await _context.Votes
                .Where(vote => vote.UserId == userId)
                .Select(vote => vote.PollId)
                .ToListAsync();

            var availablePolls = await _context.Polls
                .Where(poll => !votedPollIds.Contains(poll.Id))
                .ToListAsync();

            return View(availablePolls);
        }

        [HttpGet]
        public IActionResult Vote(int pollId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (_context.Votes.Any(v => v.UserId == userId && v.PollId == pollId))
            {
                TempData["ErrorMessage"] = "You have already voted for this poll.";
                return RedirectToAction("ViewPolls", "User");
            }

            var poll = _context.Polls.Include(p => p.Options).SingleOrDefault(p => p.Id == pollId);


            if (poll == null || DateTime.Now < poll.StartTime || DateTime.Now > poll.EndTime)
            {
                TempData["ErrorMessage"] = "This poll is not available.";
                return RedirectToAction("ViewPolls", "User");
            }

            return View(poll);
        }


        [HttpPost]
        public async Task<IActionResult> Vote(int pollId, int optionId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (_context.Votes.Any(v => v.UserId == userId && v.PollId == pollId))
            {
                TempData["ErrorMessage"] = "You have already voted for this poll.";
                return RedirectToAction("ViewPolls", "User");
            }

            var poll = _context.Polls.Include(p => p.Options).FirstOrDefault(p => p.Id == pollId);

            if (poll == null || DateTime.Now < poll.StartTime || DateTime.Now > poll.EndTime)
            {
                TempData["ErrorMessage"] = "This poll is not available.";
                return RedirectToAction("ViewPolls", "User");
            }

            var selectedOption = poll.Options.FirstOrDefault(o => o.Id == optionId);

            if (selectedOption == null)
            {
                TempData["ErrorMessage"] = "Invalid option.";
                return RedirectToAction("ViewPolls", "User");
            }

            var vote = new VoteModel
            {
                UserId = userId,
                PollId = pollId,
                OptionId = optionId
            };

            _context.Votes.Add(vote);
            selectedOption.VoteCount++;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vote submitted successfully!";

            return RedirectToAction("ViewPolls", "User");
        }

        [HttpGet]
        public IActionResult ViewAllVotes()
        {
            var allPolls = _context.Polls.Include(p => p.Options).ToList();
            return View("ViewAllVotes", allPolls);
        }

        [HttpGet]
        public IActionResult ViewVotes(int pollId)
        {
            var poll = _context.Polls
                .Include(p => p.Options)
                .FirstOrDefault(p => p.Id == pollId);

            if (poll == null)
            {
                TempData["ErrorMessage"] = "Poll not found.";
                return RedirectToAction("ViewPolls", "User");
            }

            return View("ViewVotes", poll);
        }
    }
}