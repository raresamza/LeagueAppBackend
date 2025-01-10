using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models;

public class AppUser
{

    public AppUser(int iD, string email, int age, string name, int phoneNumber)
    {
        ID = iD;
        Email = email;
        Age = age;
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public AppUser() { }

    public int ID {  get; set; }

    [Required]
    [StringLength(100)]
    public required string Email {  get; set; }
    [Range(1, 100)]
    [Required]
    public required int Age {  get; set; }
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    [Required]
    [StringLength(100)]
    public required int PhoneNumber {  get; set; }
}
