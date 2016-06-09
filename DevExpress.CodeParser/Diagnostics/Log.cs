#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Diagnostics
#else
namespace DevExpress.CodeParser.Diagnostics
#endif
{
  public sealed class Log
  {
	#region Log
	private Log()
	{
	}
	#endregion
		static ILogger Logger
		{
			get
			{
				return StructuralParserServicesHolder.GetLogger();
			}
		}
		#region Send(string message)
		public static void Send(string message)
		{
			ILogger logger = Logger;
			if (logger != null)
				logger.Send(message);
		}
		#endregion
		#region SendError(string message)
		public static void SendError(string message)
		{
			ILogger logger = Logger;
			if (logger != null)
				logger.SendError(message);
		}
		#endregion
		#region SendErrorWithStackTrace(string message)
		public static void SendErrorWithStackTrace(string message)
		{
			ILogger logger = Logger;
			if (logger != null)
				logger.SendErrorWithStackTrace(message);
		}
		#endregion
		#region SendWarning(string message)
		public static void SendWarning(string message)
		{
			ILogger logger = Logger;
			if (logger != null)
				logger.SendWarning(message);
		}
		#endregion
		#region SendException(Exception value)
		public static void SendException(Exception value)
		{
			ILogger logger = Logger;
			if (logger != null)
				logger.SendException(value);
		}
		#endregion
		#region SendException(string message, Exception value)
		public static void SendException(string message, Exception value)
		{
			ILogger logger = Logger;
			if (logger != null)
				logger.SendException(message, value);
		}
		#endregion
  }
}
