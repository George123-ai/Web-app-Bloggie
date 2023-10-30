using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
	public class BlogPostCommentRepository : IBlogPostCommentRepository
	{
		private readonly BloggieDbContext bloggieDbContext;

		public BlogPostCommentRepository(BloggieDbContext bloggieDbContext)
		{
			this.bloggieDbContext = bloggieDbContext;
		}


		public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
		{
			await bloggieDbContext.BlogPostComment.AddAsync(blogPostComment);
			await bloggieDbContext.SaveChangesAsync();
			return blogPostComment;
		}

		public async Task<IEnumerable<BlogPostComment>> GetCommnetsByBlogIdAsync(Guid blogPostId)
		{
			return await bloggieDbContext.BlogPostComment.Where(u => u.BlogPostId == blogPostId)
				.ToListAsync();
		}
	}
}
