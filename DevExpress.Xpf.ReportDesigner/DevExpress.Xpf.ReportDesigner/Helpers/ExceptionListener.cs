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
using System.Diagnostics;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public static class ExceptionListener {
		public static IEnumerable<Exception> DoAndCollectExceptions(Action action, TraceSource traceSource) {
			var listener = new ExceptionTraceListener();
			traceSource.Listeners.Add(listener);
			try {
				action();
			} finally {
				traceSource.Listeners.Remove(listener);
			}
			return listener.GetCollectedExceptions();
		}
		sealed class ExceptionTraceListener : TraceListener {
			List<Exception> exceptions = new List<Exception>();
			public IEnumerable<Exception> GetCollectedExceptions() { return exceptions.AsReadOnly(); }
			public override void Close() { }
			public override void Fail(string message) { }
			public override void Fail(string message, string detailMessage) { }
			public override void Flush() { }
			protected override string[] GetSupportedAttributes() { return new string[] { }; }
			public override bool IsThreadSafe { get { return false; } }
			public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id) { }
			public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args) { }
			public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message) { }
			public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId) { }
			public override void Write(object o) { }
			public override void Write(object o, string category) { }
			public override void Write(string message) { }
			public override void Write(string message, string category) { }
			protected override void WriteIndent() { }
			public override void WriteLine(object o) { }
			public override void WriteLine(object o, string category) { }
			public override void WriteLine(string message) { }
			public override void WriteLine(string message, string category) { }
			public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
				TraceData(data);
			}
			public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data) {
				foreach(var dataItem in data)
					TraceData(dataItem);
			}
			void TraceData(object data) {
				var exception = data as Exception;
				if(exception != null)
					exceptions.Add(exception);
			}
		}
	}
}
