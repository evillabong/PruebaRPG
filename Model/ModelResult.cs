using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ModelResult
    {
        public int ResultCode { get; set; }
        public string Message { get; set; } = null!;

        public bool IsSuccess()
        {
            return ResultCode == 0;
        }
    }

    public class ModelResult<T> : ModelResult 
    {
        public List<T> Data { get; set; } = new List<T>();
    }

}
