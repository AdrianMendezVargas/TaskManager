﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Responses {
    public class OperationResponse<T> : EmptyOperationResponse {
        public T Record { get; set; }
    }
}
