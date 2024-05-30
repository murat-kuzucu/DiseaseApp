using DiseaseApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiseaseApp.ViewComponents{

    public class NewPosts:ViewComponent{

        private IPostRepository _PostRepository;

        public NewPosts(IPostRepository PostRepository){
            _PostRepository = PostRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(){
            return View(await _PostRepository
                .Posts
                .OrderByDescending(i => i.PublishedOn)
                .Take(3)
                .ToListAsync());
        }
    }
}