﻿namespace WebService.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public List<User> Students { get; set; } = new List<User>();
    }
}
