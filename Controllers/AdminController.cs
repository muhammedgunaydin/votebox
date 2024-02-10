using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vote_box.Models;

namespace votebox.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Where(user => user.Role != "Admin")
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var userToDel = await _context.Users.FindAsync(userId);
                if (userToDel == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(userToDel);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetAllUsers", "Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult CreatePoll()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoll(PollModel poll, string Options)
        {
            if (poll.StartTime >= poll.EndTime)
            {
                ModelState.AddModelError("EndTime", "End time should be after start time");
                return View(poll);
            }

            poll.StartTime = poll.StartTime.ToUniversalTime();
            poll.EndTime = poll.EndTime.ToUniversalTime();
            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            string[] optionNames = Options.Split(',');

            foreach (var optionName in optionNames)
            {
                var option = new OptionModel { Text = optionName.Trim(), PollId = poll.Id };
                _context.Options.Add(option);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }


        [HttpGet]
        public async Task<IActionResult> GetPolls()
        {
            var polls = await _context.Polls
                .Include(p => p.Options)
                .ToListAsync();

            return View(polls);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePoll(int pollId)
        {
            try
            {
                var pollToDelete = await _context.Polls
                    .Include(p => p.Options)
                    .Include(p => p.Votes)
                    .FirstOrDefaultAsync(p => p.Id == pollId);

                if (pollToDelete == null)
                {
                    return NotFound();
                }

                _context.Options.RemoveRange(pollToDelete.Options);

                _context.Votes.RemoveRange(pollToDelete.Votes);

                _context.Polls.Remove(pollToDelete);

                await _context.SaveChangesAsync();

                return RedirectToAction("GetPolls", "Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Login", "User");
        }
    }
}