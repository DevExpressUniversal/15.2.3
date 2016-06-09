#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.MiddleTier {
	public enum LogLevel {
		Error = 0,
		Warning = 1,
		Info = 2
	}
	public class CustomWriteWindowsLogEntryEventArgs : HandledEventArgs {
		public EventLogEntryType EntryType { get; set; }
		public string Message { get; set; }
		public int EventId { get; set; }
	}
	public class CustomSendMailEventArgs : HandledEventArgs {
		public SmtpClient Client { get; set; }
		public MailMessage Message { get; set; }
	}
	public class CustomWriteTextEntryEventArgs : HandledEventArgs {
		public string Message { get; set; }
	}
	public interface ILogger {
		void Log(string message, LogLevel level, int messageId);
	}
	public class CustomLogSettingsEventArgs : EventArgs {
		public LogLevel LogLevel { get; set; }
	}
	public class Logger : ILogger {
		private List<ILogWriter> logWriters = new List<ILogWriter>();
		private static ILogger instance = new Logger();
		public void Log(string message, LogLevel level, int eventId) {
			foreach(ILogWriter logWriter in LogWriters) {
				logWriter.WriteEntry(message, level, eventId);
			}
		}
		public IList<ILogWriter> LogWriters {
			get { return logWriters; }
		}
		public static LogLevel ConvertToLogLevel(TraceLevel level) {
			LogLevel result;
			switch(level) {
				case TraceLevel.Error:
					result = LogLevel.Error;
					break;
				case TraceLevel.Info:
					result = LogLevel.Info;
					break;
				case TraceLevel.Warning:
					result = LogLevel.Warning;
					break;
				default:
					result = LogLevel.Info;
					break;
			}
			return result;
		}
		public static ILogger Instance {
			get { return instance; }
			set { instance = value; }
		}
		public Logger() { }
		public Logger(params ILogWriter[] writer) {
			logWriters.AddRange(writer);
		}
	}
	public interface ILogWriter {
		void WriteEntry(string message, LogLevel level, int eventId);
	}
	public class WindowsEventLogWriter : ILogWriter {
		private EventLog eventLog;
		private LogLevel level;
		public WindowsEventLogWriter(LogLevel level, string logName, string sourceName) {
			if(!EventLog.SourceExists(sourceName)) {
				EventLog.CreateEventSource(sourceName, logName);
			}
			eventLog = new EventLog { Log = logName, Source = sourceName };
			this.level = level;
		}
		private EventLogEntryType GetLogEntryTypeByLogLevel(LogLevel level) {
			EventLogEntryType result;
			switch(level) {
				case LogLevel.Error:
					result = EventLogEntryType.Error;
					break;
				case LogLevel.Warning:
					result = EventLogEntryType.Warning;
					break;
				case LogLevel.Info:
					result = EventLogEntryType.Information;
					break;
				default:
					result = EventLogEntryType.Information;
					break;
			}
			return result;
		}
		public void WriteEntry(string message, LogLevel level, int eventId) {
			if(level > this.level) {
				return;
			}
			EventLogEntryType logEntryType = GetLogEntryTypeByLogLevel(level);
			if(CustomWriteEntry != null) {
				CustomWriteWindowsLogEntryEventArgs args = new CustomWriteWindowsLogEntryEventArgs() {
					EntryType = logEntryType,
					Message = message,
					EventId = eventId
				};
				CustomWriteEntry(this, args);
				if(!args.Handled) {
					eventLog.WriteEntry(args.Message, args.EntryType, args.EventId);
				}
			}
			else {
				eventLog.WriteEntry(message, logEntryType, eventId);
			}
		}
		public event EventHandler<CustomWriteWindowsLogEntryEventArgs> CustomWriteEntry;
	}
	public class EmailLogWriter : ILogWriter {
		public static int MaxSubjectLength = 120;
		private List<string> addressesList = new List<string>();
		private string from;
		private string applicationName;
		private readonly LogLevel level;
		private readonly SmtpClient smtpClient;
		private void SendMail(string subject, string from, string body, string address) {
			using(MailMessage mailMessage = new MailMessage(from, address, subject, body)) {
				if(CustomSendMessage != null) {
					CustomSendMailEventArgs args = new CustomSendMailEventArgs { Message = mailMessage, Client = smtpClient };
					CustomSendMessage(this, args);
					if(!args.Handled) {
						smtpClient.Send(mailMessage);
					}
				}
				else {
					smtpClient.Send(mailMessage);
				}
			}
		}
		public EmailLogWriter(LogLevel level, string from, string appName, SmtpClient smtpClient, params string[] addresses) {
			this.from = from;
			this.applicationName = appName;
			this.level = level;
			this.smtpClient = smtpClient;
			for(int i = 0; i < addresses.Length; ++i) {
				if(!addressesList.Contains(addresses[i])) {
					addressesList.Add(addresses[i]);
				}
			}
		}
		public EmailLogWriter(string from, string appName, SmtpClient smtpClient, params string[] addresses) :
			this(LogLevel.Error, from, appName, smtpClient, addresses) { }
		public virtual void WriteEntry(string message, LogLevel level, int eventId) {
			if(level > this.level) {
				return;
			}
			string subject = applicationName + " : " + message;
			if(subject.Length > MaxSubjectLength) {
				subject = subject.Substring(0, MaxSubjectLength - 3) + "...";
			}
			foreach(string address in addressesList) {
				string mailBody = String.Format(EmailBodyTemplate, level, eventId, message);
				SendMail(subject, from, mailBody, address);
			}
		}
		public static string EmailBodyTemplate = "Category: {0}\r\nCode: {1}\r\nText: {2}.";
		public event EventHandler<CustomSendMailEventArgs> CustomSendMessage;
	}
	public class SimpleTextLogWriter : ILogWriter {
		private readonly string logFileName;
		private readonly LogLevel level;
		private void SendMessage(string message) {
			using(StreamWriter logStream = new StreamWriter(logFileName, true)) {
				CustomWriteTextEntryEventArgs args = new CustomWriteTextEntryEventArgs() { Message = message };
				if(CustomWriteEntry != null) {
					CustomWriteEntry(this, args);
				}
				if(!args.Handled) {
					logStream.Write(message);
				}
				logStream.Flush();
			}
		}
		public SimpleTextLogWriter(LogLevel level, string logFileName) {
			this.logFileName = logFileName;
			this.level = level;
		}
		public virtual void WriteEntry(string message, LogLevel level, int eventId) {
			if(level > this.level) {
				return;
			}
			StringBuilder resultMessage = new StringBuilder(String.Format("=== {0} === {1} ===================", DateTime.Now, level));
			resultMessage.AppendLine();
			resultMessage.AppendLine(message);
			resultMessage.AppendLine();
			SendMessage(resultMessage.ToString());
		}
		public string LogFilename {
			get { return logFileName; }
		}
		public LogLevel Level {
			get { return level; }
		}
		public event EventHandler<CustomWriteTextEntryEventArgs> CustomWriteEntry;
	}
	public class ExpressAppTracingLogWriter : ILogWriter {
		private LogLevel level;
		public ExpressAppTracingLogWriter(LogLevel level) {
			this.level = level;
		}
		public void WriteEntry(string message, LogLevel level, int eventId) {
			switch(level) {
				case LogLevel.Error:
					Tracing.Tracer.LogError(message);
					break;
				case LogLevel.Warning:
					Tracing.Tracer.LogWarning(message);
					break;
				case LogLevel.Info:
					Tracing.Tracer.LogText(message);
					break;
				default:
					Tracing.Tracer.LogText(message);
					break;
			}
		}
	}
}
