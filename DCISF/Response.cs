using System;

namespace DCIS
{
    public class Response
    {
        public int Id { get; set; }
        public DateTime RefDateTime { get; set; }
        public float Value { get; set; }
        public string State { get; set; }
        public string Message { get; set; }
    }
}
