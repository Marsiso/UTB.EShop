using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTB.EShop.Infrastructure.Entities;

public abstract class AuditableEntity
{
    /// <summary>
    /// The date the entity was created
    /// </summary>
    [Column("date_created")]
    [DataType(DataType.DateTime)]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// The date the entity was last modified
    /// </summary>
    [Column("date_modified")]
    [DataType(DataType.DateTime)]

    public DateTime DateModified { get; set; }

    /// <summary>
    /// The user who created the entity
    /// </summary>
    [Column("created_by")]
    [DataType(DataType.Text)]
    public string CreatedBy { get; set; }

    /// <summary>
    /// The user who last modified the entity
    /// </summary>
    [Column("modified_by")]
    [DataType(DataType.Text)]
    public string ModifiedBy { get; set; }
}
