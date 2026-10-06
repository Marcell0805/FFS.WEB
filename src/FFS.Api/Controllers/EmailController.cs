using Microsoft.AspNetCore.Mvc;

namespace FFS.Api.Controllers;

[ApiController]
[Route("v1")]
[Tags("Email")]
public sealed class EmailController : ControllerBase
{
    private readonly EmailOutbox _outbox;

    public EmailController(EmailOutbox outbox) => _outbox = outbox;

    [HttpGet("templates")]
    public ActionResult<IReadOnlyList<EmailTemplate>> Templates() => Ok(_outbox.Templates);

    [HttpPost("messages/preview")]
    public ActionResult<EmailPreview> Preview([FromBody] EmailRequest request)
    {
        try
        {
            return Ok(_outbox.Preview(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("messages")]
    public ActionResult<SentEmail> Send([FromBody] EmailRequest request)
    {
        try
        {
            return Ok(_outbox.Send(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("messages")]
    public ActionResult<IReadOnlyList<SentEmail>> Sent() => Ok(_outbox.Sent);

    [HttpDelete("messages")]
    public IActionResult Clear()
    {
        _outbox.Clear();
        return NoContent();
    }
}
