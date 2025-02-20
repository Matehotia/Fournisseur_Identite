using System.ComponentModel.DataAnnotations;       
using System.ComponentModel.DataAnnotations.Schema; 

namespace App.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public int id {get; set;}

    [Column("user_name")]
    public string name {get; set;}

    [Column("user_email")]
    public string email {get; set;}

    [Column("user_password")]
    public string password {get; set;}
}
