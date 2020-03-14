using Facebook_project.Data;
using Facebook_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_project.Repositories
{
	public class PostRepository
	{
		ApplicationDbContext _context;
		public PostRepository(ApplicationDbContext _context)
		{
			this._context = _context;
		}
		public IEnumerable<Post> GetPosts()
		{
			return _context.Posts.Include(p => p.Publisher);
		}
		public void CreatePost(Post post)
		{
			post.isDeleted = false;
			post.numberOfLikes = 0;
			post.Date = DateTime.Now;
			_context.Posts.Add(post);
			_context.SaveChanges();
		}
		public Post GetPostByID(int? id)
		{
			var post = _context.Posts.Find(id);
			return post;
		}
		public void UpdatePost(int id, Post post)
		{
			Post p = _context.Posts.FirstOrDefault(po => po.PostId == id);
			if(p!=null)
			{
				p.Date = DateTime.Now;
				p.Comment = post.Comment;
				p.isDeleted = post.isDeleted;
				p.Text = post.Text;
				p.PictureURL = post.PictureURL;
				p.Like = post.Like;
				//p.numberOfLikes = post.numberOfLikes;
			}

			//_context.Entry(post).State = EntityState.Modified;

			_context.SaveChanges();
		}
		public Post DeletePosts(int id)
		{
			var posts = _context.Posts.Find(id);
			if (posts == null)
			{
				return posts;
			}

			_context.Posts.Remove(posts);
			_context.SaveChanges();

			return posts;
		}
		public bool PostExists(int id)
		{
			return _context.Posts.Any(e => e.PostId == id);
		}
	}
}