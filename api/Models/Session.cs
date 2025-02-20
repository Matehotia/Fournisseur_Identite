using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table("sessions")]
public class Session
{
    [Key]
    [Column("session_id")]
    public int id {get; set;}

    [Column("token")]
    public string token {get; set;}

   [Column("expiration")]
    public DateTime expiration {get; set;}

    [ForeignKey("User")]
    [Column("user_id")]
    public int user_id {get; set;}
}