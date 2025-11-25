using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
using Microsoft.AspNetCore.Http;

namespace LzqNet.Contracts.Notify.QQMail.Commands;
public record QQMailSendCommand : Command
{
    public string Subject { get; set; }
    public string Content { get; set; }
    public bool IsHtml { get; set; }
    public List<string> Tos { get; set; }
    public List<string>? Ccs { get; set; }
    public IFormFileCollection Attachments { get; set; }
}
public class QQMailSendCommandValidator : MasaAbstractValidator<QQMailSendCommand>
{
    public QQMailSendCommandValidator()
    {
        // 主题验证
        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("邮件主题不能为空")
            .MaximumLength(200).WithMessage("邮件主题长度不能超过200个字符");

        // 内容验证
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("邮件内容不能为空")
            .MaximumLength(5000).WithMessage("邮件内容长度不能超过5000个字符");

        // 收件人验证
        RuleFor(x => x.Tos)
            .NotEmpty().WithMessage("收件人列表不能为空")
            .Must(tos => tos.Count <= 50).WithMessage("收件人数量不能超过50个");

        RuleForEach(x => x.Tos)
            .NotEmpty().WithMessage("收件人邮箱不能为空")
            .EmailAddress().WithMessage("收件人邮箱格式不正确");

        // 抄送人验证（可选）
        When(x => x.Ccs != null && x.Ccs.Any(), () =>
        {
            RuleFor(x => x.Ccs)
                .Must(ccs => ccs.Count <= 20).WithMessage("抄送人数量不能超过20个");

            RuleForEach(x => x.Ccs)
                .NotEmpty().WithMessage("抄送人邮箱不能为空")
                .EmailAddress().WithMessage("抄送人邮箱格式不正确");
        });

        // 附件验证（可选）
        When(x => x.Attachments != null && x.Attachments.Any(), () =>
        {
            RuleFor(x => x.Attachments)
                .Must(attachments => attachments.Count <= 10).WithMessage("附件数量不能超过10个")
                .Must(attachments => attachments.Sum(a => a.Length) <= 20 * 1024 * 1024)
                .WithMessage("附件总大小不能超过20MB");

            RuleForEach(x => x.Attachments)
                .ChildRules(attachment =>
                {
                    attachment.RuleFor(a => a.FileName)
                        .NotEmpty().WithMessage("附件文件名不能为空")
                        .Must(name => !Path.GetInvalidFileNameChars().Any(name.Contains))
                        .WithMessage("附件文件名包含非法字符");

                    attachment.RuleFor(a => a)
                        .NotNull().WithMessage("附件内容不能为空")
                        .Must(file => file.Length <= 10 * 1024 * 1024)
                        .WithMessage("单个附件大小不能超过10MB")
                        .Must(file => IsAllowedFileType(file.FileName))
                        .WithMessage("不支持的文件类型");
                });
        });
    }

    private bool IsAllowedFileType(string fileName)
    {
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".png", ".txt" };
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}