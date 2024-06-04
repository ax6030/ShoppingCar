using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCar.Data;
using ShoppingCar.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace ShoppingCar.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShoppingCarContext _context;
        

        public ProductsController(ShoppingCarContext context)
        {
            _context = context;
        }
        private int _page = 1;
        private int _pageSize = 4;
        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if(value < 1)
                    _currentPage = 1;
                else
                    _currentPage = value;
            }
        }
        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            _currentPage = _page;
            var result = from m in _context.Products select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(m => m.Name.Contains(searchString));
            }
            
            var pageModel = result.OrderBy(m=>m.Id).ToPagedList(_currentPage, _pageSize);
            return View(pageModel);
        }

        //將相關資料傳到前端，不只顯示頁碼
        public async Task<IActionResult> PageListDetail(string searchString)
        {
            //處理頁數
            ViewBag.PageList = GetPageProess(_page, _pageSize);

            //填入頁面資料
            return View(await _context.Products.Skip<Products>(_pageSize * ((_page == null ? 1 : _page ) - 1)).Take(_pageSize).ToListAsync()); ;
        }

        protected IPagedList<Products> GetPageProess(int? page, int pagesize)
        {
            // 過濾從client傳送過來有問題頁數
            if (page.HasValue && page < 1)
                return null;
            // 從資料庫取得資料
            IPagedList<Products> pageList = _context.Products.ToPagedList(page ?? 1, _pageSize);

            // 過濾從client傳送過來有問題頁數，包含判斷有問題的頁數邏輯
            if (pageList.PageNumber != 1 && page.HasValue && page > pageList.PageCount)
                return null;

            return pageList;
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DetailViewModel dvm = new DetailViewModel();

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            else
            {
                dvm.products = products;
                if(products.Image != null)
                {
                    dvm.imgsrc = ViewImage(products.Image);
                }
            }

            return View(dvm);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products products, IFormFile myimg)
        {
            /*if (Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    products.Image = ms.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categoties"] = new SelectList(_context.Set<Category>(), "Id", "Name", products.CategoryId);
            return View(products);*/
            ViewData["Categoties"] = new SelectList(_context.Set<Category>(), "Id", "Name", products.CategoryId);
            if (!ModelState.IsValid)
            {
                if (myimg != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        myimg.CopyTo(ms);
                        products.Image = ms.ToArray();
                    }

                }
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Products products)
        {
            if (id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return View();
        }

        private string ViewImage(byte[] arrayImage)
        {
            // 二進位圖檔轉字串
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
            return "data:image/png;base64," + base64String;
        }
    }
}
