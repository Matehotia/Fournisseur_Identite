using System.ComponentModel.DataAnnotations;       
using System.ComponentModel.DataAnnotations.Schema; 

namespace App.Models;

[Table("pin_expirations")]
public class PinExpiration
{
    [Key]
    [Column("expiration_id")]
    public int id {get; set;}

    [Column("expiration")]
    public TimeSpan time {get; set;}
}
