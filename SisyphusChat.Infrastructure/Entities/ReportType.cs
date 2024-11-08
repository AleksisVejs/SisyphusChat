using System.ComponentModel.DataAnnotations;

namespace SisyphusChat.Infrastructure.Entities
{
    public enum ReportType
    {
        [Display(Name = "Spam")]
        Spam,

        [Display(Name = "Scam")]
        Scam,

        [Display(Name = "Harassment or Bullying")]
        HarassmentOrBullying,

        [Display(Name = "Hate Speech or Offensive Language")]
        HateSpeechOrOffensiveLanguage,

        [Display(Name = "Violence or Threats")]
        ViolenceOrThreats,

        [Display(Name = "Inappropriate Content")]
        InappropriateContent,

        [Display(Name = "Misinformation")]
        Misinformation,

        [Display(Name = "Unauthorized Sharing of Personal Information")]
        UnauthorizedSharingOfPersonalInformation,

        [Display(Name = "Other")]
        Other
    }
}
