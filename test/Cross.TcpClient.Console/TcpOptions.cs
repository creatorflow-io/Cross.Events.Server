﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.TcpClient.Console
{
	internal class TcpOptions
	{
		public string ServerAddress { get; set; } = "";
		public int ServerPort { get; set; } = 5000;
	}
}
