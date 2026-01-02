using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models.ViewModels;
using PhapClinicX.Services;

namespace PhapClinicX.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<ActionResult<ChatReply>> Ask([FromBody] ChatQuery request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            request.UserId ??= userId;

            var reply = await _chatService.AskAsync(request);
            return Ok(reply);
        }
    }
}
