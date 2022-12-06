using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAuth.Api.Configuration;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Services;

namespace RestAuth.Api.Controllers
{
    [Authorize("Bearer")]
    [TokenValidation]
    [Route("api/v1/[controller]")]
    public abstract class EntityController<TEntity> : ControllerBase
        where TEntity : Entity
    {
        private readonly IServiceBase<TEntity> _service;

        public EntityController(IServiceBase<TEntity> service)
        {
            _service = service;
        }

        // GET api/v1/entities
        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            try
            {
                var entities = await _service.GetAllAsync();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return DefaultBadRequest(ex.Message);
            }
        }

        // GET api/v1/entities/5
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var entity = await _service.GetByIdAsync(id);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return DefaultBadRequest(ex.Message);
            }
        }

        // POST api/v1/entities
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TEntity entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Model inválido" });

                await _service.AddOrUpdateAsync(entity);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return DefaultBadRequest(ex.Message);
            }
        }

        // PUT api/v1/entities/5
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(Guid id, [FromBody] TEntity entity)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Model inválido" });

                entity.Id = id;
                await _service.AddOrUpdateAsync(entity);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return DefaultBadRequest(ex.Message);
            }
        }

        // DELETE api/v1/entities/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _service.Delete(id);
                return Ok(id);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(406, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return DefaultBadRequest(ex.Message);
            }
        }

        protected IActionResult DefaultBadRequest(string message)
        {
            return BadRequest(new { message });
        }
    }
}