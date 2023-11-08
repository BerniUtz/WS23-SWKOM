using Microsoft.AspNetCore.Mvc;
using Mock_Server.Models;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api/")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "Root"), HttpGet]
    public IActionResult Root()
    {
        return Ok();
    }

    [HttpGet("statistics/", Name = "Statistics")]
    public IActionResult Statistics()
    {
        return Ok(new Statistics()
        {
            DocumentsInboxCount = 400,
            DocumentsTotalCount = 800,
            InboxTagCount = 200,
            CharacterCount = 100000,
            DocumentFileTypeCounts = new[]{
                new DocumentFileTypeCount(){
                    MimeType = "application/pdf",
                    Count = 400
                },
                new DocumentFileTypeCount(){
                    MimeType = "application/msword",
                    Count = 200
                },
            }
        });
    }

    [HttpPost("token", Name = "GetToken")]
    public IActionResult GetToken([FromBody] Login info)
    {
        if (info.Username == "user" && info.Password == "password")
            return this.Ok(new { Token = "asdasd" });
        else
            return this.Unauthorized();
    }

    [HttpGet("users/{id}", Name = "GetUser")]
    public IActionResult GetUser(int id)
    {
        _logger.LogWarning("GetUser called with id {id}", id);

        return Ok(new User()
        {
            Id = id,
            Username = "max",
            Email = "knor@technikum-wien.at",
            FirstName = "Max",
            LastName = "K",
            DateJoined = DateTime.Now.AddDays(-10),
            IsStaff = true,
            IsActive = true,
            IsSuperuser = true,
            Groups = new int[0],
            UserPermissions = new string[0],
            InheritedPermissions = new string[] {
                "auth.delete_permission",
                "paperless_mail.change_mailrule",
                "django_celery_results.add_taskresult",
                "documents.view_taskattributes",
                "documents.view_paperlesstask",
                "django_q.add_success",
                "documents.view_uisettings",
                "auth.change_user",
                "admin.delete_logentry",
                "django_celery_results.change_taskresult",
                "django_q.change_schedule",
                "django_celery_results.delete_taskresult",
                "paperless_mail.add_mailaccount",
                "auth.change_group",
                "documents.add_note",
                "paperless_mail.delete_mailaccount",
                "authtoken.delete_tokenproxy",
                "guardian.delete_groupobjectpermission",
                "contenttypes.delete_contenttype",
                "documents.change_correspondent",
                "authtoken.delete_token",
                "documents.delete_documenttype",
                "django_q.change_ormq",
                "documents.change_savedviewfilterrule",
                "auth.delete_group",
                "documents.add_documenttype",
                "django_q.change_success",
                "documents.delete_tag",
                "documents.change_note",
                "django_q.delete_task",
                "documents.add_savedviewfilterrule",
                "django_q.view_task",
                "paperless_mail.add_mailrule",
                "paperless_mail.view_mailaccount",
                "documents.add_frontendsettings",
                "sessions.change_session",
                "documents.view_savedview",
                "authtoken.add_tokenproxy",
                "documents.change_tag",
                "documents.view_document",
                "documents.add_savedview",
                "auth.delete_user",
                "documents.view_log",
                "documents.view_note",
                "guardian.change_groupobjectpermission",
                "sessions.delete_session",
                "django_q.change_failure",
                "guardian.change_userobjectpermission",
                "documents.change_storagepath",
                "documents.delete_document",
                "documents.delete_taskattributes",
                "django_celery_results.change_groupresult",
                "django_q.add_ormq",
                "guardian.view_groupobjectpermission",
                "admin.change_logentry",
                "django_q.delete_schedule",
                "documents.delete_paperlesstask",
                "django_q.view_ormq",
                "documents.change_paperlesstask",
                "guardian.delete_userobjectpermission",
                "auth.view_permission",
                "auth.view_user",
                "django_q.add_schedule",
                "authtoken.change_token",
                "guardian.add_groupobjectpermission",
                "documents.view_documenttype",
                "documents.change_log",
                "paperless_mail.delete_mailrule",
                "auth.view_group",
                "authtoken.view_token",
                "admin.view_logentry",
                "django_celery_results.view_chordcounter",
                "django_celery_results.view_groupresult",
                "documents.view_storagepath",
                "documents.add_storagepath",
                "django_celery_results.add_groupresult",
                "documents.view_tag",
                "guardian.view_userobjectpermission",
                "documents.delete_correspondent",
                "documents.add_tag",
                "documents.delete_savedviewfilterrule",
                "documents.add_correspondent",
                "authtoken.view_tokenproxy",
                "documents.delete_frontendsettings",
                "django_celery_results.delete_chordcounter",
                "django_q.change_task",
                "documents.add_taskattributes",
                "documents.delete_storagepath",
                "sessions.add_session",
                "documents.add_uisettings",
                "documents.change_taskattributes",
                "documents.delete_uisettings",
                "django_q.delete_ormq",
                "auth.change_permission",
                "documents.view_savedviewfilterrule",
                "documents.change_frontendsettings",
                "documents.change_documenttype",
                "documents.view_correspondent",
                "auth.add_user",
                "paperless_mail.change_mailaccount",
                "documents.add_paperlesstask",
                "django_q.view_success",
                "django_celery_results.delete_groupresult",
                "documents.delete_savedview",
                "authtoken.change_tokenproxy",
                "documents.view_frontendsettings",
                "authtoken.add_token",
                "django_celery_results.add_chordcounter",
                "contenttypes.change_contenttype",
                "admin.add_logentry",
                "django_q.delete_failure",
                "documents.change_uisettings",
                "django_q.view_failure",
                "documents.add_log",
                "documents.change_savedview",
                "paperless_mail.view_mailrule",
                "django_q.view_schedule",
                "documents.change_document",
                "django_celery_results.change_chordcounter",
                "documents.add_document",
                "django_celery_results.view_taskresult",
                "contenttypes.add_contenttype",
                "django_q.delete_success",
                "documents.delete_note",
                "django_q.add_failure",
                "guardian.add_userobjectpermission",
                "sessions.view_session",
                "contenttypes.view_contenttype",
                "auth.add_permission",
                "documents.delete_log",
                "django_q.add_task",
                "auth.add_group"
                }
        });
    }
}
