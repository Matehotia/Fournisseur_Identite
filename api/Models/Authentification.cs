using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table("authentifications")]
public class Authentification
{
    [Key]
    [Column("auth_id")]
    public int id {get; set;}

    [Column("pin")]
    public string pin_code {get; set;}

    [Column("expiration")]
    public DateTime expiration {get; set;}

    [ForeignKey("User")]
    [Column("user_id")]
    public int user_id {get; set;}
}
