using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WcfServiceApplication_FacebookParser
{
    public class Education
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int year_entrance { get; set; }
        public int year_graduation { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public List<User> students { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string tel { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string profileId { get; set; }
        public List<User> _friends { get; set; }
        public List<Education> _educations { get; set; }
        public List<Group> _groups { get; set; }
    }

    public class Group
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int _user_quantity { get; set; }
        public string manager { get; set; }
        public List<User> members { get; set; }
    }
    public class Music
    {
        public int Id { get; set; }
        public string song { get; set; }
        public List<User> subscribers { get; set; }
    }
    public class databasewcf : DbContext
    {
        public databasewcf() : base("name=placeholder") { }
        public DbSet<Education> educations { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Group> droups { get; set; }
        public DbSet<Music> musics { get; set; }
    }
}