using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Controllers
{
	public interface IBlogPostCommentRepository
	{
		Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment);

		Task<IEnumerable<BlogPostComment>> GetCommnetsByBlogIdAsync(Guid blogPostId);


	}
}
