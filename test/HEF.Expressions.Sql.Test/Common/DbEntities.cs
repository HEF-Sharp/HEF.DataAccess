﻿using System;

namespace HEF.Expressions.Sql.Test
{
    public class Customer
    {
        public long id { get; set; }
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime createTime { get; set; }
    }
}