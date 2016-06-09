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
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.Design.Mvvm.Wizards {
	public interface ILogger {
		void SendException(string msg, Exception ex, bool display = false);
		void Send(string message, bool display, object[] args);
		void SendErrorWithStackTrace(string message, string stackTrace, bool display= false);
		void SendErrorWithStackTrace(string format, string stackTrace, bool display, params object[] args);
		void SendWarning(string message, bool display = false);
	}
	public static class Log {
		static List<ILogger> loggers;
		static string GetStackTrace(int framesToSkip) {
			try {
				StringBuilder builder = new StringBuilder();
				StackTrace stack = new StackTrace(framesToSkip, true);
				builder.AppendLine("Call stack:");
				builder.Append(stack.ToString());
				return builder.ToString();
			}
			catch (Exception ex) {
				SendException("LOG ERROR: Exception raised while retrieving StackTrace.", ex);
				return String.Empty;
			}
		}
		public static void Send(string message, bool display, params object[] args) {
			if (loggers != null)
				foreach (ILogger logger in loggers)
					logger.Send(message, display, args);
		}
		public static void SendErrorWithStackTrace(string message, bool display = false) {
			string stackTrace = GetStackTrace(3);
			if (loggers != null)
				foreach (ILogger logger in loggers)
					logger.SendErrorWithStackTrace(stackTrace, message, display);
		}
		public static void SendErrorWithStackTrace(string format, bool display, params object[] args) {
			string stackTrace = GetStackTrace(3);
			if (loggers != null)
				foreach (ILogger logger in loggers)
					logger.SendErrorWithStackTrace(format, stackTrace, display, args);
		}
		public static void SendException(Exception ex, bool display = false) {
			SendException(null, ex, display);
		}
		public static void SendException(string msg, Exception ex, bool display = false) {
			if (loggers == null)
				return;
			foreach (ILogger logger in loggers)
				logger.SendException(msg, ex, display);
		}
		public static void SendWarning(string message, bool display = false) {
			if (loggers != null)
				foreach (ILogger logger in loggers)
					logger.SendWarning(message, display);
		}
		public static void TryCatch(Action action) {
			if (action == null)
				return;
			try {
				action();
			}
			catch (Exception ex) {
				SendException(ex);
			}
		}
		public static void RegisterLogger(ILogger logger) {
			if (logger == null)
				return;
			if (loggers == null)
				loggers = new List<ILogger>();
			if (!loggers.Contains(logger))
				loggers.Add(logger);
		}
		public static void UnRegisterLogger(ILogger logger) {
			if (logger == null || loggers == null || !loggers.Contains(logger))
				return;
			loggers.Remove(logger);
		}
		}
	class DebugTestLogger : ILogger {
		public void SendException(string msg, Exception ex, bool display) {
			if (!String.IsNullOrEmpty(msg))
				Send(msg);
			if (ex == null)
				return;
			Send("Message: {0}", ex.Message);
			Send("Source: {0}", ex.Source);
			Send("StackTrace: {0}", ex.StackTrace);
			if (ex.InnerException != null) {
				Send("InnerException:");
				SendException(null, ex.InnerException, display);
			}
		}
		public void Send(string message, bool display, object[] args) {
			SendLine(message, args);
		}
		public void Send(string message, params object[] args) {
			SendLine(message, args);
			}
		public void SendErrorWithStackTrace(string message, string stackTrace, bool display) {
			Send(message);
			Send(stackTrace);
			}
		public void SendErrorWithStackTrace(string format, string stackTrace, bool display, params object[] args) {
			Send(String.Format(CultureInfo.CurrentCulture, format, args));
			Send(stackTrace);
		}
		public void SendWarning(string message, bool display) {
			Send(message);
	}
		void SendLine(string message, params object[] args) {
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, message, args));
}
	}
}
