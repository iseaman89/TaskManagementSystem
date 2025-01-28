using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagmentSystem.Data.Shared.Statics;
using TaskService.Data;
using TaskService.Models.Tasks;
using TaskService.Models.Topics;
using TaskService.Repositories;
using TaskService.Repositories.Topics;

namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TopicController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TopicController> _logger;

        public TopicController(ITopicRepository topicRepository, IMapper mapper, ILogger<TopicController> logger)
        {
            _topicRepository = topicRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TopicReadOnlyDto>>> GetTopics(string userId)
        {
            try
            {
                var topics = await _topicRepository.GetTopicsByUserIdAsync(userId);
                var topicDtos = _mapper.Map<IEnumerable<TopicReadOnlyDto>>(topics);
                return Ok(topicDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetTopics)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<TopicReadOnlyDto>> GetTopic(int id)
        {
            try
            {
                var topic = await _topicRepository.GetAsync(id);

                if (topic == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetTopic)} - ID: {id}");
                    return NotFound();
                }

                var topicDto = _mapper.Map<TaskReadOnlyDto>(topic);
                return Ok(topicDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetTopics)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTopic(int id, TopicUpdateDto topicDto)
        {
            if (id != topicDto.Id)
            {
                _logger.LogWarning($"Update ID invalid in {nameof(PutTopic)} - ID: {id}");
                return BadRequest();
            }
            
            var topic = await _topicRepository.GetAsync(id);

            if (topic == null)
            {
                _logger.LogWarning($"{nameof(Task)} record not found in {nameof(PutTopic)} - ID: {id}");
                return NotFound();
            }

            _mapper.Map(topicDto, topic);

            try
            {
                await _topicRepository.UpdateAsync(topic);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await TopicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error Performing GET in {nameof(PutTopic)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TopicCreateDto>> PostTopic(TopicCreateDto topicDto)
        {
            try
            {
                var topic = _mapper.Map<TopicModel>(topicDto);
                await _topicRepository.AddAsync(topic);
                return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostTopic)}", topicDto);
                return StatusCode(500, Messages.Error500Message);
            }
            
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                var topic = await _topicRepository.GetAsync(id);
                if (topic == null)
                {
                    _logger.LogWarning($"{nameof(Task)} record not found in {nameof(DeleteTopic)} - ID: {id}");
                    return NotFound();
                }

                await _topicRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteTopic)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> TopicExists(int id)
        {
            return await _topicRepository.Exists(id);
        }
    }
}
