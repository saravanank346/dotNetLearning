using System;
using dotNetAPi.Models;

namespace dotNetAPi.Dtos
{
	public class Iuser
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string?Password { get; set; }
		//public DateTime? Created_At { get; set; }

    }

	public class Ilogin
	{
		public string ?Email { get; set; }
		public string ?Password { get; set; }
	}

    public class IloginResponse
    {
        public User Data { get; set; }
        public string accessToken { get; set; }
		public List<string> permissions { get; set; }
    }
}

