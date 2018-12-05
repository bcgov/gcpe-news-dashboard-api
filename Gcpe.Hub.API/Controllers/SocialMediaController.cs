using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Gcpe.Hub.API.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Gcpe.Hub.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SocialMediaController : ControllerBase
    {
        private readonly HubDbContext dbContext;
        private readonly ILogger<SocialMediaController> logger;
        private readonly IMapper mapper;

        public SocialMediaController(HubDbContext dbContext, ILogger<SocialMediaController> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAll()
        {
            try
            {
                var posts = dbContext.SocialMediaPost.ToList();
                mapper.Map<List<SocialMediaPost>, List<SocialMediaPostViewModel>>(posts);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get social media posts: {ex}");
                return BadRequest("Failed to get social media posts");
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult Post(SocialMediaPostViewModel postVM)
        {
            try
            {
                if (postVM.Id != Guid.Empty)
                {
                    throw new ValidationException("Invalid parameter (id)");
                }
                SocialMediaPost post = mapper.Map<SocialMediaPostViewModel, SocialMediaPost>(postVM);
                post.Id = Guid.NewGuid();
                dbContext.SocialMediaPost.Add(post);
                dbContext.SaveChanges();
                return CreatedAtRoute("GetPost", new { id = post.Id }, mapper.Map<SocialMediaPost, SocialMediaPostViewModel>(post));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to create social media post: {ex}");
                return BadRequest("Failed to create social media post");
            }
        }

        [HttpGet("{id}", Name = "GetPost")]
        [Produces(typeof(SocialMediaPostViewModel))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Get(Guid id)
        {
            try
            {
                var post = dbContext.SocialMediaPost.Find(id);
                if (post != null)
                {
                    return Ok(mapper.Map<SocialMediaPost, SocialMediaPostViewModel>(post));
                }
                return NotFound($"Social media post not found with id: {id}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to retrieve social media post: {ex}");
                return BadRequest("Failed to retrieve social media post");
            }
        }

        [HttpPut("{id}")]
        [Produces(typeof(SocialMediaPostViewModel))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Put(Guid id, SocialMediaPostViewModel postVM)
        {
            try
            {
                SocialMediaPost dbPost = dbContext.SocialMediaPost.Find(id);
                if (dbPost != null)
                {
                    dbPost = mapper.Map(postVM, dbPost);
                    dbPost.Timestamp = DateTime.Now;
                    dbPost.Id = id;
                    dbContext.SocialMediaPost.Update(dbPost);
                    dbContext.SaveChanges();
                    return Ok(mapper.Map<SocialMediaPost, SocialMediaPostViewModel>(dbPost));
                }
                return NotFound($"Social media post not found with id: {id}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update social media post: {ex}");
                return BadRequest("Failed to update social media post");
            }
        }
    }
}
