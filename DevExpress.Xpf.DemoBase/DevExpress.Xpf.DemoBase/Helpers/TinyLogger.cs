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

using DevExpress.Xpf.DemoBase.DemoTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace DevExpress.Xpf.DemoBase.Helpers {
	interface ITinyLoggerSink {
		void Info(string message);
		void Error(string message);
	}
	class DebugSink : ITinyLoggerSink {
		public void Info(string message) {
			Debug.WriteLine(message);
		}
		public void Error(string message) {
			Info(message);
		}
	}
	class FileSink : ITinyLoggerSink {
		string path;
		public FileSink(string path = null) {
			if(path == null) {
				path = string.Format("{0}_{1}_log.txt", Assembly.GetExecutingAssembly().FullName, DateTime.Now.ToString("hh.mm.ss", CultureInfo.InvariantCulture));
			}
			this.path = path;
		}
		public void Info(string message) {
			using(var fs = File.Open(path, FileMode.Append, FileAccess.Write))
			using(var sw = new StreamWriter(fs)) {
				sw.Write(message);
			}
		}
		public void Error(string message) {
			Info(message);
		}
	}
	class DemoTestServiceSink : ITinyLoggerSink {
		IWPFDemoTestService service;
		public DemoTestServiceSink(IWPFDemoTestService service) {
			this.service = service;
		}
		public void Info(string message) {
			service.IAmAlive(message);
		}
		public void Error(string message) {
			service.OnError(message);
		}
	}
	class TinyLogger {
		List<ITinyLoggerSink> sinks;
		public TinyLogger(params ITinyLoggerSink[] sinks) {
			this.sinks = sinks.ToList();
		}
		public void AddSink(ITinyLoggerSink sink) {
			sinks.Add(sink);
		}
		public void Info(string message) {
			FlushInfo(message);
		}
		public void Info(string format, params object[] args) {
			FlushInfo(string.Format(format, args));
		}
		public void Error(string message) {
			FlushError(message);
		}
		public void Error(object exception) {
			FlushError("Exception: " + GetExceptionDetails(exception));
		}
		public void Error(string format, params object[] args) {
			FlushError(string.Format(format, args));
		}
		void FlushInfo(string message) {
			sinks.ForEach(s => s.Info(FormatMessage(message, "Info")));
		}
		void FlushError(string message) {
			sinks.ForEach(s => s.Error(FormatMessage(message, "Error")));
		}
		private static string FormatMessage(string message, string level) {
			return string.Format("{0} {1}: {2}\n", level, DateTime.Now.ToString("hh.mm.ss.ff", CultureInfo.InvariantCulture), message);
		}
		string GetExceptionDetails(object exceptionObject) {
			if(exceptionObject == null) return "Null";
			Exception exception = exceptionObject as Exception;
			if(exception == null) return "Unknown exception: " + exceptionObject.GetType().Name;
			StringBuilder s = new StringBuilder();
			for(Exception e = exception; e != null; e = e.InnerException)
				s.AppendLine(e.Message);
			s.AppendLine();
			s.AppendLine(exception.StackTrace);
			return s.ToString();
		}
	}
}
