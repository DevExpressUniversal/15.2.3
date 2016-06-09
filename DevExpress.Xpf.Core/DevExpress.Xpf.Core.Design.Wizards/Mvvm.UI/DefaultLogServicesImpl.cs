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
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Text;
using System.Windows;
namespace DevExpress.Design.Mvvm.Wizards.Diagnostics {
	public enum LogItemType {
		Error,
		Warning,
		Exception,
		Message,
		None
	}
	public abstract class LogItem : ILogItem {
		public virtual LogItemType ItemType { get { return LogItemType.None; } }
		public virtual void OutText(System.Windows.Documents.BlockCollection blockCollection) {
			blockCollection.Add(new Paragraph(new Run(this.ToString())));
		}
		public virtual void OutText(StringBuilder sb) {
			Send(sb, this.ToString());
		}
		protected void Send(System.Windows.Documents.BlockCollection blockCollection, string format, params object[] args) {
			if (args != null && args.Length > 0)
				format = string.Format(format, args);
			blockCollection.Add(new Paragraph(new Run(format)));
		}
		protected void SendWithBold(System.Windows.Documents.BlockCollection blockCollection, string boldText, string text) {
			Paragraph p = new Paragraph(new Run(boldText) { FontWeight = FontWeights.Bold });
			if (!string.IsNullOrEmpty(text))
				p.Inlines.Add(new Run(text));
			blockCollection.Add(p);
		}
		protected void Send(StringBuilder sb, string format, params object[] args) {
			if (args != null && args.Length > 0)
				format = string.Format(format, args);
			sb.AppendLine(format);
		}
	}
	public class MessageItem : LogItem {
		public MessageItem(string message) {
			Message = message;
		}
		public override LogItemType ItemType {
			get { return LogItemType.Message; }
		}
		public string Message { get; set; }
		public override string ToString() {
			return Message;
		}
		public override void OutText(System.Windows.Documents.BlockCollection blockCollection) {
			if (!string.IsNullOrEmpty(Message))
				SendWithBold(blockCollection, "Message: ", Message);
		}
	}
	public class WarningItem : MessageItem {
		public WarningItem(string message)
			: base(message) {
		}
		public override LogItemType ItemType {
			get { return LogItemType.Warning; }
		}
	}
	public class ExceptionItem : MessageItem {
		public ExceptionItem(string msg, Exception ex)
			: base(msg) {
			Exception = ex;
		}
		public override LogItemType ItemType {
			get { return LogItemType.Exception; }
		}
		public Exception Exception { get; private set; }
		public override void OutText(System.Windows.Documents.BlockCollection blockCollection) {
			base.OutText(blockCollection);
			if (this.Exception == null)
				return;
			SendException(blockCollection, Exception);
		}
		void SendException(System.Windows.Documents.BlockCollection blockCollection, Exception ex) {
			SendWithBold(blockCollection, "Message: ", ex.Message);
			SendWithBold(blockCollection, "Source: ", ex.Source);
			SendWithBold(blockCollection, "StackTrace: ", ex.StackTrace);
			if (ex.InnerException != null) {
				SendWithBold(blockCollection, "InnerException:", null);
				SendException(blockCollection, ex.InnerException);
			}
		}
		public override void OutText(StringBuilder sb) {
			base.OutText(sb);
			if (this.Exception == null)
				return;
			SendException(sb, Exception);
		}
		void SendException(StringBuilder sb, Exception ex) {
			Send(sb, "Message: {0}", ex.Message);
			Send(sb, "Source: {0}", ex.Source);
			Send(sb, "StackTrace: {0}", ex.StackTrace);
			if (ex.InnerException != null) {
				Send(sb, "InnerException:");
				SendException(sb, ex.InnerException);
			}
		}
	}
	public class ErrorItem : MessageItem {
		public ErrorItem(string message, string stackTrace)
			: base(message) {
			StackTrace = stackTrace;
		}
		public override LogItemType ItemType {
			get { return LogItemType.Error; }
		}
		public string StackTrace { get; private set; }
		public override void OutText(System.Windows.Documents.BlockCollection blockCollection) {
			base.OutText(blockCollection);
			if (string.IsNullOrEmpty(this.StackTrace))
				return;
			Send(blockCollection, this.StackTrace);
		}
	}
	public class DefaultLogServicesImpl : ILogServices, ILogger {
		const int INT_StackSize = 100;
		List<ILogItem> logItems;
		public DefaultLogServicesImpl() {
			logItems = new List<ILogItem>();
			Log.RegisterLogger(this);
		}
		void Add(LogItem item) {
			logItems.Add(item);
			while (logItems.Count > INT_StackSize)
				logItems.RemoveAt(0);
		}
		public void Send(string message, bool display, params object[] args) {
			if (display)
				this.Add(new MessageItem(string.Format(CultureInfo.CurrentCulture, message, args)));
		}
		public void SendException(string msg, Exception ex, bool display) {
			if (display)
				this.Add(new ExceptionItem(msg, ex));
		}
		public void SendErrorWithStackTrace(string stackTrace, string message, bool display) {
			if (display)
				this.Add(new ErrorItem(message, stackTrace));
		}
		public void SendErrorWithStackTrace(string format, string stackTrace, bool display, params object[] args) {
			if(!display)
				return;
			string message = String.Format(CultureInfo.CurrentCulture, format, args);
			this.Add(new ErrorItem(message, stackTrace));
		}
		public void SendWarning(string message, bool display) {
			if(display)
				this.Add(new WarningItem(message));
		}
		public IEnumerable<ILogItem> GetItems() {
			return this.logItems.Reverse<ILogItem>();
		}
	}
}
