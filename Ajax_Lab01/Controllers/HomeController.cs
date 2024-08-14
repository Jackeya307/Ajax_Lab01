using Ajax_Lab01.Models;
using Ajax_Lab01.Models.DTO;
using Ajax_Lab01.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

using System.Diagnostics;

namespace Ajax_Lab01.Controllers {
    public class HomeController : Controller {
        private readonly IWebHostEnvironment _webHost;
        private MyDBContext _dbContext;
        public HomeController(MyDBContext dbContext,IWebHostEnvironment webHost)
        {
            _dbContext = dbContext;
            _webHost = webHost;
        }

        [HttpGet("/Home/SiteId/{City?}")] //
        public IActionResult SiteId(string? City)
        {
            var c = _dbContext.Addresses
            .Where(Address => Address.SiteId.Contains(City))
            .Select(Address => Address.SiteId)
            .Distinct()
            .ToList();

            if(!c.Any()) {
                return NotFound(new {
                    error = "搜尋不到此城市"
                });
            }
            return Json(c);
        }


        [HttpGet("/Home/Road/{SiteId?}")] //
        public IActionResult Road(string? SiteId)
        {
            //var c = _dbContext.Addresses.Where(Address => Address.SiteId.Contains(City)).Select(Address => Address.SiteId).Distinct().ToList();
            var c = _dbContext.Addresses
                .Where(Address => Address.SiteId.Contains(SiteId))
                .Select(Address => Address.Road)
                .Distinct()
                .ToList();

            if(!c.Any()) {
                return NoContent();
            }

            return Json(c);
        }
        public IActionResult Index()
        {
            var c = _dbContext.Addresses;
            return View(c);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult homework()
        {
            return View();
        }
        public IActionResult UserPhoto()
        {

            ViewBag.MemberID = new SelectList(_dbContext.Members.Select(c => c.MemberId));
            return View();
        }

        public async Task<IActionResult> getImg(int? id)
        {
            try {
                if(id == null)
                    return NotFound(new {
                        error = "id不能為空"
                    });
                //var c = _dbContext.Members.Find(id).FileData; 如果Find找不到就會回傳null 但繼續找filedata會跳異常

                Member? c = await _dbContext.Members.FindAsync(id);

                if(c != null && c.FileData != null) {
                    return File(c.FileData,"image/jpeg");
                }

                return NotFound(new {
                    error = "無此ID"
                });
            }
            catch(Exception ex) {
                return NotFound("發生例外的狀況 " + ex.Message);
            }
        }
        public IActionResult Register(UserInfo user)
        {
            if(user == null || user.UserPhoto == null || user.UserPhoto.Length == 0)
                return BadRequest(new {
                    error = "圖片或資料未填入"
                });
            try {
                Byte[]? data;
                using(var memory = new MemoryStream()) {
                    user.UserPhoto[0].CopyTo(memory);
                    data = memory.ToArray();
                }

                string path = $"{_webHost.WebRootPath}/Images/{user.UserPhoto[0].FileName}";
                using(var filestream = new FileStream(path,FileMode.Create)) {
                    user.UserPhoto[0].CopyTo(filestream);
                }

                Member member = new Member() {
                    Email = user.Email,
                    Name = user.Name,
                    Age = user.Age,
                    Password = user.Password,
                    FileData = data,
                    FileName = user.UserPhoto[0].FileName,
                };
                _dbContext.Members.Add(member);
                _dbContext.SaveChanges();

                return Json("Done");
            }
            catch(Exception ex) {
                return NotFound(new {
                    error = ex.Message + "發生例外的狀況",
                });

            }
        }

        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO searchDTO)
        {
            try {
                var spot = searchDTO.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots
                                                                                .Where(c => c.CategoryId == searchDTO.categoryId);
                if(!string.IsNullOrEmpty(searchDTO.keyword))
                    spot = _dbContext.SpotImagesSpots.Where(c => c.SpotTitle.Contains(searchDTO.keyword) ||
                    c.SpotDescription.Contains(searchDTO.keyword));

                switch(searchDTO.sortBy) {
                    case "spotTitle":
                    spot = searchDTO.sortType == "asc" ? spot.OrderBy(s => s.SpotTitle) :
                                                         spot.OrderByDescending(s => s.SpotTitle);
                    break;
                    default:
                    spot = searchDTO.sortType == "asc" ? spot.OrderBy(s => s.SpotId) :
                                                         spot.OrderByDescending(s => s.SpotId);
                    break;
                }
                // 總筆數/一頁大小並無條件進位  
                int dataCount = spot.Count();
                
                int PagesSize = searchDTO.pageSize ?? 9;
                int Page = searchDTO.page ?? 1;
                int TotalPages = (int)Math.Ceiling(((decimal)dataCount / PagesSize));
                //跳過幾筆資料 意思是指定頁-1後*一頁大小
                spot = spot.Skip((Page - 1) * PagesSize).Take(PagesSize);
                SpotsPagingDTO pagingDTO = new SpotsPagingDTO();
                pagingDTO.TotalCount = dataCount;
                pagingDTO.TotalPages = TotalPages;
                pagingDTO.SpotsResult = spot.ToList();
                return Json(pagingDTO);
            }
            catch(Exception ex) {
                throw new Exception(ex.Message,ex);
            }

        }
        public IActionResult travel()
        {
            var c = _dbContext.Categories;
            return View(c);
        }

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}