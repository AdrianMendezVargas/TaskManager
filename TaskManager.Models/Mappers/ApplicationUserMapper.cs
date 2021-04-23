using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Requests;

namespace TaskManager.Models.Mappers {
    public static class ApplicationUserMapper {
        
        public static ApplicationUser ToApplicationUser(RegisterUserRequest request) {

            return new ApplicationUser {
                Id = 0 ,
                Email = request.Email ,
                Password = request.Password ,
                CreatedOn = DateTime.Now
            };

        }

    }
}
