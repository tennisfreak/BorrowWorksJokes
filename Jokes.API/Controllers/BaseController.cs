using AutoMapper;
using Jokes.Data.Memory;
using Jokes.Entities;
using Jokes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Jokes.API.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseEntity
    {
        private bool hasLoadedData = false;

        protected IRepoFactory _repoFactory;
        protected IBaseRepository<T> _baseRepo;
        protected IConfiguration _config;
        protected IHttpContextAccessor _httpContext;
        protected IMapper _mapper;

        public BaseController(IRepoFactory repoFactory, JokesContext context, IConfiguration config, IHttpContextAccessor httpContext, IMapper mapper)
        {
            if (!hasLoadedData)
                AddTestData(context);
            _repoFactory = repoFactory;
            _repoFactory.Context = context;
            _config = config;
            _httpContext = httpContext;
            _mapper = mapper;
        }

        protected async Task<JsonResult> Get<A>(A id)
        {
            var result = await _baseRepo.GetOne<A>(id);
            if (result == null)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Json(result);
        }

        protected async Task<JsonResult> GetWithMapping<T, A>(A id)
        {
            var result = await _baseRepo.GetOne<A>(id);
            if (result == null)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Json(_mapper.Map<T>(result));
        }

        protected JsonResult GetAll()
        {
            //get all 
            var result = _baseRepo.Get();
            if (result == null)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Json(result);
        }

        protected JsonResult GetAllWithMapping<T>()
        {
            //get all 
            var result = _baseRepo.Get();
            if (result == null)
                Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Json(_mapper.Map<IEnumerable<T>>(result));
        }

        protected async Task<JsonResult> AddNew(T item)
        {
            try
            {
                //make sure viewmodel validates
                if (ModelState.IsValid)
                {
                    //set user
                    //item.ModifiedBy = this.UserName;
                    item.ModifiedDate = DateTime.UtcNow;
                    await _baseRepo.Add(item);
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json(item.ID);
                }
            }
            catch (Exception ex)
            {
                //LoggerWrapper.LogError(this.GetType(), "Error adding new in base controller", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ProcessException(ex) });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Invalue values input", ModelState = ModelState });
        }

        protected async Task<JsonResult> AddNewWithMapping<A>(A item) where A : class
        {
            try
            {
                //make sure viewmodel validates
                if (ModelState.IsValid)
                {
                    var newItem = _mapper.Map<T>(item);
                    //set user
                    //newItem.ModifiedBy = this.UserName;
                    newItem.ModifiedDate = DateTime.UtcNow;
                    await _baseRepo.Add(newItem);
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json(newItem.ID);
                }
            }
            catch (Exception ex)
            {
                //LoggerWrapper.LogError(this.GetType(), "Error adding new in base controller", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ProcessException(ex) });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Invalue values input", ModelState = ModelState });
        }

        protected async Task<JsonResult> Save(T item)
        {
            try
            {
                //make sure viewmodel validates
                if (ModelState.IsValid)
                {
                    //set user
                    //item.ModifiedBy = this.UserName;
                    item.ModifiedDate = DateTime.UtcNow;
                    var result = await _baseRepo.Save(item);
                    if (result)
                    {
                        Response.StatusCode = (int)HttpStatusCode.NoContent;
                        return Json(true);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return Json(new { Message = "Item not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                //LoggerWrapper.LogError(this.GetType(), "Error saving in base controller", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ProcessException(ex) });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Invalue values input", ModelState = ModelState });
        }

        protected async Task<JsonResult> SaveWithMapping<A>(A item)
        {
            try
            {
                //make sure viewmodel validates
                if (ModelState.IsValid)
                {
                    var updateItem = _mapper.Map<T>(item);
                    //set user
                    //newItem.ModifiedBy = this.UserName;
                    updateItem.ModifiedDate = DateTime.UtcNow;
                    var result = await _baseRepo.Save(updateItem);
                    if (result)
                    {
                        Response.StatusCode = (int)HttpStatusCode.NoContent;
                        return Json(true);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return Json(new { Message = "Item not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                //LoggerWrapper.LogError(this.GetType(), "Error saving in base controller", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ProcessException(ex) });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Invalue values input", ModelState = ModelState });
        }

        protected string ProcessException(Exception ex)
        {
            if (ex.InnerException != null)
                return ProcessException(ex.InnerException);
            else
            {
                if (ex.Message.Contains("UNIQUE KEY"))
                    return "Value already exists, please choose a unique value";
                else
                    return ex.Message;
            }
        }


        private void AddTestData(JokesContext context)
        {
            var adminID = Guid.NewGuid();
            context.Users.Add(new User { ID = adminID, ModifiedBy = adminID, ModifiedDate = DateTime.UtcNow });
            context.JokeTypes.Add(new JokeType { ID = 1, IsActive = true, Value = "Dad Jokes" });
            context.JokeTypes.Add(new JokeType { ID = 2, IsActive = true, Value = "Funny Jokes" });
            context.JokeTypes.Add(new JokeType { ID = 3, IsActive = false, Value = "Blonde Jokes" });
            context.JokeTypes.Add(new JokeType { ID = 4, IsActive = true, Value = "Knock Knock Jokes" });
            context.JokeTypes.Add(new JokeType { ID = 5, IsActive = true, Value = "Food Jokes" });
            context.Jokes.Add(new Joke
            {
                ID = Guid.Parse("4cb4e6a8-e7ba-42b9-b10b-f99e438dcc48"),
                JokeType_ID = 3,
                Text = @"A blonde, a redhead, and a brunette were all lost in the desert. 
                They found a lamp and rubbed it. A genie popped out and granted them each one wish. 
                The redhead wished to be back home. Poof! She was back home. The brunette wished to be at home with her family. 
                Poof! She was back home with her family. The blonde said, 'Awwww, I wish my friends were here.'",
                DislikeCount = 10,
                LikeCount = 75,
                ModifiedBy = adminID,
                ModifiedDate = DateTime.UtcNow
            });
            context.Jokes.Add(new Joke
            {
                ID = Guid.Parse("ae8cc9d0-66c4-488e-9f32-81d36369ca37"),
                JokeType_ID = 1,
                Text = @"What time did the man go to the dentist? Tooth hurt-y.",
                DislikeCount = 956,
                LikeCount = 233,
                ModifiedBy = adminID,
                ModifiedDate = DateTime.UtcNow
            });
            context.Jokes.Add(new Joke
            {
                ID = Guid.Parse("5a680e6e-7ebd-46a1-8127-e1557074f658"),
                JokeType_ID = 2,
                Text = @"The Teacher says to the class: Who ever stands up is stupid
                    *Nobody stands up*
                    Teacher: I said who ever stands up is STUPID!
                    *Little Johnny stands up*
                    Teacher: Johnny, do you really think that you are stupid?
                    Little Johnny: No Mrs, I just thought that maybe you are lonely being the only one standing.",
                DislikeCount = 99,
                LikeCount = 199,
                ModifiedBy = adminID,
                ModifiedDate = DateTime.UtcNow
            });
            context.Jokes.Add(new Joke
            {
                ID = Guid.Parse("5e1a4248-ff96-443c-a68a-fc03a32addcc"),
                JokeType_ID = 4,
                Text = @"Knock knock. Who's there? Hawaii. Hawaii who? I'm fine, Hawaii you?",
                DislikeCount = 0,
                LikeCount = 0,
                ModifiedBy = adminID,
                ModifiedDate = DateTime.UtcNow
            });
            context.Jokes.Add(new Joke
            {
                ID = Guid.Parse("6de160d4-40d4-4bac-a439-cac7ea16c8a0"),
                JokeType_ID = 5,
                Text = @"My friend thinks he is smart. He told me an onion is the only food that makes you cry, so I threw a coconut at his face.",
                DislikeCount = 12,
                LikeCount = 22,
                ModifiedBy = adminID,
                ModifiedDate = DateTime.UtcNow
            });
            context.SaveChanges();
            hasLoadedData = true;
        }
    }
}