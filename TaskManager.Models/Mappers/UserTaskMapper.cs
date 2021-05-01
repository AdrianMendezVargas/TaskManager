using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Models;
using TaskManager.Shared.Requests;

namespace TaskManager.Models.Mappers {
    public static class UserTaskMapper {

        public static UserTaskDetails ToUserTaskDetails(this UserTask task) {
            return new UserTaskDetails {
                Id = task.Id,
                Name = task.Name,
                State = task.State,
                CreatedOn = task.CreatedOn
            };
        }

        public static List<UserTaskDetails> ToUserTaskDetailsList(this List<UserTask> tasks) {
            var taskDetailsList = new List<UserTaskDetails>();
            tasks.ForEach(t => {
                taskDetailsList.Add(t.ToUserTaskDetails());
            });
            return taskDetailsList;
        }

        public static UserTask ToUserTask(this CreateTaskRequest request) {
            return new UserTask() {
                Name = request.Name    
            };
        }

        public static UserTask ToUserTask(this UpdateTaskRequest request) {
            return new UserTask() {
              Id = request.Id,
              Name = request.Name,
              State = request.State
            };
        }

    }
}
