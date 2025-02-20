using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table("attempts")]
public class Attempt
{
    [Key]
    [Column("attempt_id")]
    public int id {get; set;}

    [Column("attempt")]
    public int attempt {get; set;}

    [ForeignKey("User")]
    [Column("user_id")]
    public int user_id {get; set;}
}
