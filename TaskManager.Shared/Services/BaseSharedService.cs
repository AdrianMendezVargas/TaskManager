using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Shared.Responses;

namespace TaskManager.Shared.Services {
    public abstract class BaseSharedService {

        protected EmptyOperationResponse Error(string message) {
            return new EmptyOperationResponse {
                Message = message ,
                IsSuccess = false
            };
        }

        protected EmptyOperationResponse Success(string message) {
            return new EmptyOperationResponse{
                Message = message ,
                IsSuccess = true
            };
        }

    }
}
