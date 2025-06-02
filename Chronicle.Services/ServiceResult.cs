﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    /// <summary>
    /// Service result with status and message
    /// </summary>
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ServiceResult<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult<T> FailureResult(string message)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }


}
