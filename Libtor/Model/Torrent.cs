﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libtor.Model
{
    public class Torrent : BaseModel
    {
        public string Name { get; set; }
        public string File { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }
        public string ResumeData { get; set; }
    }
}
