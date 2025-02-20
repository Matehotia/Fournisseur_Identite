using System.ComponentModel.DataAnnotations;       
using System.ComponentModel.DataAnnotations.Schema; 

namespace App.Models;

[Table("session_expirations")]
public class SessionExpiration
{
    [Key]
    [Column("session_expiration_id")]
    public int id {get; set;}

    [Column("expiration")]
    public TimeSpan time {get; set;}
}
