﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Attachments")]
public class Attachment : BaseEntity
{
    [Required]
    public byte[] Content { get; set; }

    [Required]
    public string FileName { get; set; }

    public Message Message { get; set; }
}
