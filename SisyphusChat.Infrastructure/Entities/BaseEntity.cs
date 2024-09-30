﻿using System.ComponentModel.DataAnnotations;

namespace SisyphusChat.Infrastructure.Entities;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime TimeCreated { get; set; }

    [Required]
    public DateTime LastUpdated { get; set; }
}