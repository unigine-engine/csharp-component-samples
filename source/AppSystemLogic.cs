using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Unigine;

namespace UnigineApp
{
	internal class AppSystemLogic : SystemLogic
	{
		public override bool Init()
		{
			InterpreterRegistrator.Initialize();
			Input.MouseHandle = Input.MOUSE_HANDLE.USER;
			return true;
		}

		public override bool Update()
		{
			return true;
		}

		public override bool Shutdown()
		{
			return true;
		}
	}
}
