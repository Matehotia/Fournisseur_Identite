using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table("number_attempts")]
public class NumberAttempt
{
    [Key]
    [Column("number_attempt_id")]
    public int id {get; set;}

    [Column("number_attempt")]
    public int count {get; set;}
}
