using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Shared.Responses;

namespace TaskManager.Blazor.Services {
    public abstract class BaseService {

        protected OperationResponse<T> Error<T>(string message , T record) {
            return new OperationResponse<T> {
                Message = message ,
                Record = record ,
                IsSuccess = false
            };
        }

        protected OperationResponse<T> Success<T>(string message , T record) {
            return new OperationResponse<T> {
                Message = message ,
                Record = record ,
                IsSuccess = true
            };
        }

        protected OperationResponse<T> NoContactOperationResponse<T>() {
            return new OperationResponse<T> {
                IsSuccess = false ,
                Message = "Could not contact the server" ,
                Record = default
            };
        }

    }
}
