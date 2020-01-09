﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Models.Dto
{
    public class DownloadableDto
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}
