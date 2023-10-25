using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
	public class AdminBlogPostsController : Controller
	{
		private readonly ITagRepository tagRepository;
		private readonly IBlogPostRepository blogPostRepository;
		public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
		{
			this.tagRepository = tagRepository;
			this.blogPostRepository = blogPostRepository;
		}

		public async Task<IActionResult> Add()
		{
			// Get tags from repository
			var tags = await tagRepository.GetAllAsync();

			var model = new AddBlogPostRequest
			{
				Tags = tags.Select(x => new SelectListItem { Text = x.Name,
															 Value = x.Id.ToString()})
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
		{
			// Map view model to domain model
			var blogPost = new BlogPost
			{
				Heading = addBlogPostRequest.Heading,
				PageTitle = addBlogPostRequest.PageTitle,
				Content = addBlogPostRequest.Content,
				ShortDescription = addBlogPostRequest.ShortDescription,
				FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
				UrlHandle = addBlogPostRequest.UrlHandle,
				PublishedDate = addBlogPostRequest.PublishedDate,
				Author = addBlogPostRequest.Author,
				Visible = addBlogPostRequest.Visible,
			};

			// Map tags from selected tags
			var selectedTags = new List<Tag>();
			foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
			{
				var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
				var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

				if (existingTag != null)
				{
					selectedTags.Add(existingTag);
				}
			}

			// Maping tags back to domain model
			blogPost.Tags = selectedTags;

			await blogPostRepository.AddAsync(blogPost);
			
			return RedirectToAction("Add");
		}


		public async Task<IActionResult> List()
		{
			var blogPosts = await blogPostRepository.GetallAsync();

			return View(blogPosts);
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var blogPost = await blogPostRepository.GetAsync(id);
			var tagsDomainModel = await tagRepository.GetAllAsync();

			if(blogPost != null)
			{
				var model = new EditBlogPostRequest
				{
					Id = blogPost.Id,
					Heading = blogPost.Heading,
					PageTitle = blogPost.PageTitle,
					Content = blogPost.Content,
					Author = blogPost.Author,
					FeaturedImageUrl = blogPost.FeaturedImageUrl,
					UrlHandle = blogPost.UrlHandle,
					ShortDescription = blogPost.ShortDescription,
					PublishedDate = blogPost.PublishedDate,
					Visible = blogPost.Visible,
					Tags = tagsDomainModel.Select(x => new SelectListItem
					{
						Text = x.Name,
						Value = x.Id.ToString()
					}),
					SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray(),

				};

				return View(model);

			}
			


			return View(null);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
		{
			// map view model back to domain model
			var blogPostDomainModel = new BlogPost
			{
				Id = editBlogPostRequest.Id,
				Heading = editBlogPostRequest.Heading,
				PageTitle = editBlogPostRequest.PageTitle,
				Content = editBlogPostRequest.Content,
				Author = editBlogPostRequest.Author,
				FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
				UrlHandle = editBlogPostRequest.UrlHandle,
				ShortDescription = editBlogPostRequest.ShortDescription,
				PublishedDate = editBlogPostRequest.PublishedDate,
				Visible = editBlogPostRequest.Visible,
			};
			// map tags into domain model
			var selectedTags = new List<Tag>();
			foreach (var selectedTag in editBlogPostRequest.SelectedTags)
			{
				if (Guid.TryParse(selectedTag,out var tag))
				{
					var foundTag = await tagRepository.GetAsync(tag);

					if (foundTag != null)
					{
						selectedTags.Add(foundTag);
					}
				}
			}

			blogPostDomainModel.Tags = selectedTags;

			// Submit information to repository to update
			var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);

			if (updatedBlog != null)
			{
				// show success notification
				return RedirectToAction("Edit");
			}

			// show error notification
			return RedirectToAction("Edit");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
		{
			var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);
			if (deletedBlogPost != null)
			{
				//show a success notification
				return RedirectToAction("List");
			}
			// show error notification
			return RedirectToAction("Edit", new { id = editBlogPostRequest.Id});

		}
	}
}
