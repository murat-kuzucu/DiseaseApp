using DiseaseApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DiseaseApp.ViewComponents{

    public class TagsMenu:ViewComponent{
        private readonly ITagRepository _TagRepository;

        public TagsMenu(ITagRepository TagRepository){
            _TagRepository = TagRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(){
            return View(await _TagRepository.Tags.ToListAsync());
        }
        
    }
}