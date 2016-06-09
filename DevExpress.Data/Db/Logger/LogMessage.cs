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
using System.Threading;
using System.Globalization;
using System.Xml.Serialization;
using DevExpress.Xpo.DB.Helpers;
#if !SL
using System.Data;
#endif
using DevExpress.Xpo.DB;
using System.Diagnostics;
using DevExpress.Data.Filtering;
#if !CF
using DevExpress.Utils;
using System.Collections;
#endif
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
using DevExpress.Compatibility.System;
#else
using System.Runtime.Serialization;
#endif
namespace DevExpress.Xpo.Logger {
	public enum LogMessageType {		
		DbCommand,
		LoggingEvent,
		Statement,
		SessionEvent,
		Exception,
		Text
	}
#if CF
	public delegate TResult Function<TResult, TArg>(TArg arg);
#endif
	[Serializable]
	public class LogMessage {
		public const string LogParam_ConnectionProvider = "ConnectionProvider";
		public const string LogParam_CacheElement = "CacheElement";
		public const string LogParam_CookieGuid = "CookieGuid";
		public const string LogParam_CookieAge = "CookieAge";
		public const string LogParam_Provider = "Provider";
		string error;
		DateTime date;
		TimeSpan duration;
		int threadId;		
		string threadName;
		LogMessageType messageType;
		string messageText;
		List<LogMessageParameter> parameterList = new List<LogMessageParameter>();		
		public DateTime Date {
			get { return date; }
			set { date = value; }
		}
		[XmlIgnore]
		public TimeSpan Duration {
			get { return duration; }
			set { duration = value; }
		}
		public int ThreadId {
			get { return threadId; }
			set { threadId = value; }
		}
		public string ThreadName {
			get { return threadName; }
			set { threadName = value; }
		}
#if !SL && !DXPORTABLE
		[OptionalField]
#endif
		public int LogSessionId;
		public LogMessageType MessageType {
			get { return messageType; }
			set { messageType = value; }
		}
		public string MessageText {
			get { return messageText; }
			set { messageText = value; }
		}
		public int ParameterCount {
			get { return parameterList.Count; }
		}
		public string Error {
			get { return error; }
			set { error = value; }
		}
		[XmlIgnore]
		public List<LogMessageParameter> ParameterList {
			get {
				return parameterList;
			}
		}
		public long DurationTicks {
			get { return duration.Ticks; }
			set { duration = TimeSpan.FromTicks(value); }
		}
		[XmlArray("parameters")]
		public LogMessageParameter[] Parameters {
			get {
				return parameterList == null || parameterList.Count == 0 ? new LogMessageParameter[0] : parameterList.ToArray();
			}
			set {
				parameterList.Clear();
				if (value == null || value.Length == 0) return;
				parameterList.AddRange(value);
			}
		}
		public LogMessage() { }
		public LogMessage(LogMessageType messageType, string messageText, TimeSpan duration) {
			date = DateTime.Now;
			this.duration = duration;
			threadId = Thread.CurrentThread.ManagedThreadId;
			threadName = Thread.CurrentThread.Name;
			this.messageType = messageType;
			this.messageText = messageText;
#if !SL
			LogSessionId = LogManager.LogSessionId;
#endif
		}
		public LogMessage(LogMessageType messageType, string messageText) : this(messageType, messageText, TimeSpan.Zero) { }
#if !SL
		public static LogMessage CreateMessage(object connectionProvider, IDbCommand command, TimeSpan duration) {
			string paramString = string.Empty;
			if (command.Parameters.Count != 0) {
				StringBuilder sb = new StringBuilder(command.Parameters.Count * 10);
				int count = command.Parameters.Count;
				for (int i = 0; i < count; i++) {
					IDataParameter param = (IDataParameter)command.Parameters[i];
					string parameter = param.Value == null ? "Null" : param.Value.ToString();
					if (parameter.Length > 64)
						parameter = parameter.Substring(0, 64) + "...";
					sb.AppendFormat(CultureInfo.InvariantCulture, i == 0 ? "{{{0}}}" : ",{{{0}}}", parameter);
				}
				paramString = sb.ToString();
			}
			string commandText = string.Format(!String.IsNullOrEmpty(paramString) ?
					"Executing sql '{0}' with parameters {1}" :
					"Executing sql '{0}'",
					command.CommandText.Replace('\n', ' '), paramString);
			LogMessage result = new LogMessage(LogMessageType.DbCommand, commandText, duration);
			result.ParameterList.Add(new LogMessageParameter(LogParam_ConnectionProvider, connectionProvider.GetType().ToString()));
			foreach (IDataParameter param in command.Parameters) {
				result.ParameterList.Add(new LogMessageParameter(param.ParameterName, param.Value is DBNull ? null : param.Value));
			}
			return result;
		}
#endif
#if !CF
		public static LogMessage CreateMessage(object cacheElement, DataCacheCookie cookie, string statementResult, TimeSpan duration) {
			LogMessage result = new LogMessage(LogMessageType.Statement, statementResult, duration);
			result.ParameterList.Add(new LogMessageParameter(LogParam_CacheElement, cacheElement.GetType().FullName));
			result.ParameterList.Add(new LogMessageParameter(LogParam_CookieGuid, cookie.Guid));
			result.ParameterList.Add(new LogMessageParameter(LogParam_CookieAge, cookie.Age));
			return result;
		}
#endif
		public static LogMessage CreateMessage(object provider, string statementResult, TimeSpan duration) {
			LogMessage result = new LogMessage(LogMessageType.Statement, statementResult, duration);
			result.ParameterList.Add(new LogMessageParameter(LogParam_Provider, provider.GetType().FullName));
			return result;
		}
		public static string CriteriaOperatorCollectionToString<T>(IEnumerable<T> collection) where T : CriteriaOperator {
			if(collection == null) return string.Empty;
			StringBuilder resultString = new StringBuilder();
			foreach(CriteriaOperator co in collection) {
				if (resultString.Length > 0) {
					resultString.Append(";");
				}
				resultString.AppendFormat(CultureInfo.InvariantCulture, "{0}", co);
			}
			return resultString.ToString();
		}
		public static string CollectionToString<T>(ICollection<T> collection, Function<string, T> getString) {
			if(getString != null && collection != null && collection.Count > 0) {
				StringBuilder resultString = new StringBuilder();
				foreach(T item in collection) {
					if(resultString.Length > 0) {
						resultString.Append(";");
					}
					resultString.Append(getString(item));
				}
				return resultString.ToString();
			}
			return string.Empty;
		}
		public static string CollectionToString(ICollection collection, Function<string, object> getString) {
			if (getString != null && collection != null && collection.Count > 0) {
				StringBuilder resultString = new StringBuilder();
				foreach (object item in collection) {
					if (resultString.Length > 0) {
						resultString.Append(";");
					}
					resultString.Append(getString(item));
				}
				return resultString.ToString();
			}
			return string.Empty;
		}
		public static string CollectionToString(IList<string> nameCollection, IList valueCollection, Function2<string, string, object> getParamString) {
			if(getParamString != null && nameCollection != null && nameCollection.Count > 0 && valueCollection != null && valueCollection.Count > 0 && nameCollection.Count == valueCollection.Count) {
				StringBuilder resultString = new StringBuilder();
				for(int i = 0; i < nameCollection.Count; i++) {
					if(resultString.Length > 0) resultString.Append(";");
					resultString.Append(getParamString(nameCollection[i], valueCollection[i]));
				}
				return resultString.ToString();
			}
			return string.Empty;
		}
		public static LogMessage UpdateMessageWithTables(LogMessage message, DBTable[] tables) {
			if(tables != null && tables.Length > 0) {
				StringBuilder resultString = new StringBuilder();
				foreach(DBTable table in tables) {
					if(resultString.Length > 0) {
						resultString.Append(";");
					}
					resultString.Append(table.Name);
				}
				message.ParameterList.Add(new LogMessageParameter("Tables", resultString.ToString()));
			}
			return message;
		}
		public static LogMessage UpdateMessage(LogMessage message, params string[] parameters) {
			for(int i = 0; i < (parameters.Length - 1); i += 2) {
				message.ParameterList.Add(new LogMessageParameter(parameters[i], parameters[i + 1]));
			}
			return message;
		}
		public override string ToString() {
			string threadNameOut;
			if (string.IsNullOrEmpty(threadName))
				threadNameOut = "NoName";
			else
				threadNameOut = threadName;
			return string.Format("Date:{0:dd.MM.yy HH:mm:ss.fff} Duration:{1} MessageType:{2} ThreadId:{3} ThreadName:{4} MessageText:{5} ParametersCount:{6}.", date, duration, messageType, threadId, threadNameOut, messageText, ParameterCount );
		}
	}
	[Serializable]	
	public class LogMessageParameter {
		string name;
		object parameterValue;		
		public string Name {
			set { name = value; }
			get { return name; }
		}
		public object Value {
			set { parameterValue = value; }
			get { return parameterValue; }
		}
		public LogMessageParameter() { }
		public LogMessageParameter(string name, object value) {
			this.name = name;
			parameterValue = value;
		}
		public override string ToString() {
			return string.Format("Name: {0} Value: {1}", name, parameterValue);
		}
	}
	public class LogMessageCollection {		
		public List<LogMessage> Items = new List<LogMessage>();		
	}
	public class LogMessageTimer : IDisposable {
#if CF
		DateTime start;
#else
		Stopwatch sw;
#endif
		public LogMessageTimer() {
#if CF
			start = DateTime.Now;
#else
			sw = Stopwatch.StartNew();
#endif
		}
		public void Start() {
#if CF
#else
			sw.Start();
#endif
		}
		public TimeSpan Elapsed {
			get {
#if CF
				return DateTime.Now.Subtract(start);
#else
				return sw.Elapsed;
#endif
			}
		}
		public TimeSpan Stop() {
#if CF
			return DateTime.Now.Subtract(start);
#else
			TimeSpan result = sw.Elapsed;
			sw.Stop();
			return result;
#endif
		}
		public void Restart() {
#if CF
			start = DateTime.Now;
#else
			sw.Stop();
			sw.Reset();
			sw.Start();
#endif
		}
		public void Dispose() {
			Stop();
		}
	}
}
