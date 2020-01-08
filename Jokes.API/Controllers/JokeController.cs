using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Jokes.Data.Memory;
using Jokes.Entities;
using Jokes.Interfaces;
using Jokes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Jokes.API.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class JokeController : BaseController<Joke>
    {
        private IJokesRepo _repo { get { return _repoFactory.JokesRepo; } }

        public JokeController(IRepoFactory repoFactory, JokesContext context, IConfiguration config, IHttpContextAccessor httpContext, IMapper mapper)
            : base(repoFactory, context, config, httpContext, mapper)
        {
            _baseRepo = repoFactory.JokesRepo;
        }

        /// <summary>
        /// Gets a Joke based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JokeModel object</returns>
        [HttpGet]
        public async Task<JsonResult> GetJoke(Guid id)
        {
            return await base.GetWithMapping<JokeModel, Guid>(id);
        }

        [HttpGet]
        public async Task<JsonResult> GetRandomJoke()
        {
            try
            {
                var result = await _repo.GetRandomJoke();
                return Json(_mapper.Map<JokeModel>(result));
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Seraches for jokes.  No filters supplied will result in the 1st page with 25 results.
        /// Can search/filter by type, text
        /// </summary>
        /// <param name="typeFilters">| delimited list of type ids to filter by</param>
        /// <param name="textFilters">| delimited list of joke text to search for</param>
        /// <param name="pageNumber">page number requested</param>
        /// <param name="pageSize">number of results per page requested</param>
        /// <returns>list of jokes</returns>
        [HttpGet]
        public JsonResult SearchJokes(string typeFilters, string textFilters, int pageNumber = 1, int pageSize = 25)
        {
            try
            {
                var results = _repo.SearchJokes(typeFilters, textFilters, pageNumber, pageSize);
                var output = new PaginatedModelList<JokeModel>();
                output.PageCount = results.TotalPages;
                output.PageNumber = pageNumber;
                output.TotalRecordCount = results.TotalRecords;
                results.ForEach(item =>
                {
                    output.Items.Add(_mapper.Map<JokeModel>(item));
                });
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(output);
            }
            catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Gets a list of all jokes types available.  Default is to return only active
        /// </summary>
        /// <param name="onlyActive">Specify to return active or inactive</param>
        /// <returns>list of joke types</returns>
        [HttpGet]
        public async Task<JsonResult> GetJokeTypes(bool onlyActive = true)
        {
            try
            {
                var results = await _repo.GetJokeTypes(onlyActive);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(_mapper.Map<IEnumerable<JokeTypeModel>>(results));
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Adds a joke
        /// </summary>
        /// <param name="newJoke">JokeModel object</param>
        /// <returns>Id of new joke object</returns>
        [HttpPost]
        public async Task<JsonResult> AddJoke([FromBody]JokeModel newJoke)
        {
            //give our new joke a guid
            newJoke.ID = Guid.NewGuid();
            return await base.AddNewWithMapping<JokeModel>(newJoke);
        }

        /// <summary>
        /// Updates a joke
        /// </summary>
        /// <param name="updatedJoke">joke with updated values</param>
        /// <returns>true = success, false = failure</returns>
        [HttpPut]
        public async Task<JsonResult> UpdateJoke([FromBody]JokeModel updatedJoke)
        {
            return await base.SaveWithMapping<JokeModel>(updatedJoke);
        }

        /// <summary>
        /// Deletes a joke from the databasee
        /// </summary>
        /// <param name="id">id of joke to delete</param>
        /// <returns>true = success, false = failure</returns>
        [HttpDelete]
        public async Task<JsonResult> DeleteJoke(Guid id)
        {
            try
            {
                var result = await _repo.DeleteJoke(id);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
        }
    }
}