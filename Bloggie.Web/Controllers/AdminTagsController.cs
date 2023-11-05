using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : Controller
	{
		private readonly ITagRepository _tagRepository;
		public AdminTagsController(ITagRepository tagRepository)
		{
			_tagRepository = tagRepository;
		}

		
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]//,ActionName("Add")]
		public async Task<IActionResult> Add(AddTagRequest addTagRequest)
		{
			ValidateAddTagRequest(addTagRequest);
			if (ModelState.IsValid == false)
			{
				return View();
			}

			var tag = new Tag
			{
				Name = addTagRequest.Name,
				DisplayName= addTagRequest.DisplayName,
			};

			await _tagRepository.AddAsync(tag);

			return RedirectToAction("List");
		}

		public async Task<IActionResult> List()
		{
			var tagList =await _tagRepository.GetAllAsync();

			return View(tagList);
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var tag = await _tagRepository.GetAsync(id);

			if(tag != null)
			{
				var editTagRequest = new EditTagRequest
				{
					Id= tag.Id,
					Name = tag.Name,
					DisplayName = tag.DisplayName,
				};

				return View(editTagRequest);
			}
		
			return View(null);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
		{
			var tag = new Tag
			{
				Id = editTagRequest.Id,
				Name = editTagRequest.Name,
				DisplayName = editTagRequest.DisplayName,
			};
			
			var updatedTag = await _tagRepository.UpdateAsync(tag);

			if (updatedTag != null)
			{
				return RedirectToAction("List");
				// show success notification
			}
			else
			{
				// show an error notification
			}

			return RedirectToAction("Edit", new { id = editTagRequest.Id });
			//return View(null);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
		{
			var deletedTag = await _tagRepository.DeleteAsync(editTagRequest.Id);
			if (deletedTag != null)
			{
				// show success notification
				return RedirectToAction("List");
			}
			
			return RedirectToAction("Edit" , new { id = editTagRequest.Id });
			//return View(null);
			
		}

		private void ValidateAddTagRequest(AddTagRequest request)
		{
			if (request.Name is not null && request.DisplayName != null)
			{
				if (request.Name == request.DisplayName)
				{
					ModelState.AddModelError("DisplayName", "Name can't be the same as DisplayName!");
				}
			}
		}
	}
}
