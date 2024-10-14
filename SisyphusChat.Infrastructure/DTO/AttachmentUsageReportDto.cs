using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.DTO
{
    public class AttachmentUsageReportDto
    {
        public Guid AttachmentId { get; set; }
        public string UserName { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }
        public string RelatedMessageContent { get; set; }
    }

}
