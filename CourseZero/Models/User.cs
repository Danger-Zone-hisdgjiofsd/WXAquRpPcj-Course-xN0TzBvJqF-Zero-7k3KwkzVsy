using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{

    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public string password_salt { get; set; }
        public bool email_verified { get; set; }
        public string email_verifying_hash { get; set; }
        public DateTime email_verification_issue_datetime { get; set; }
        public string password_change_new_password { get; set; }
        public string password_change_hash { get; set; }
        public DateTime password_change_request_datatime { get; set; }

    }
}
