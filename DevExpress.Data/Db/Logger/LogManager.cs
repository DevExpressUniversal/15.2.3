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
using DevExpress.Xpo.Helpers;
using System.Diagnostics;
#if !DXPORTABLE
using System.Configuration;
#endif
namespace DevExpress.Xpo.Logger {
	public class LogManager {
		public const string LogCategorySQL = "SQL";
		public const string LogParam_StackTrace = "StackTrace";
		const string STR_Dummy = "Dummy";
		static ILogger logServer = new DummyServer();
		public static ILogger LogServer {
			get { return logServer; }
		}
		static string serverType = STR_Dummy;
		public static string ServerType {
			get { return serverType; }
		}
		static bool hasTransport;
		public static bool HasTransport {
			get { return hasTransport; }
		}
		static bool hasCategoryList;
		public static bool HasCategoryList {
			get { return hasCategoryList; }
		}
		readonly static Dictionary<string, bool> categoryList = new Dictionary<string, bool>();
		public static bool HasCategory(string category) {
			return categoryList.ContainsKey(category);
		}
		static bool includeStackTrace;
		public static bool IncludeStackTrace {
			get { return includeStackTrace; }
			set { includeStackTrace = value; }
		}
		public static readonly int LogSessionId = Guid.NewGuid().GetHashCode();			
		public static bool IsLogActive(string category) {
			if(!initialized) {
				lock(initializeSyncRoot) {
					if(!initialized) {
						initialized = true;
						Init();
					}
				}
			}
			ILogger currentLogServer = logServer;
			return hasTransport && (!hasCategoryList || HasCategory(category)) && currentLogServer.IsServerActive;
		}
		static bool initialized = false;
		static readonly object initializeSyncRoot = new object();
		public static void SetCategories(string categories) {
			if(!string.IsNullOrEmpty(categories)) {
				hasCategoryList = true;
				categoryList.Clear();
				foreach(string category in categories.Split(';')) {
					categoryList[category] = true;
				}
			} else {
				hasCategoryList = false;
			}
		}
		public static void SetTransport(ILogger logger) {
			SetTransport(logger, null);
		}
		public static void SetTransport(ILogger logger, string categories) {
			if(logger == null) throw new ArgumentNullException("logger");
			SetTransport(logger, logger.GetType().FullName, categories);
		}
		static void SetTransport(ILogger logger, string serverType, string categories) {
			lock(initializeSyncRoot) {
				LogManager.initialized = true;
				if(!(logServer is DummyServer)) {
					ResetTransport();
				}
				LogManager.logServer = logger;
				LogManager.serverType = serverType;
				SetCategories(categories);
				LogManager.hasTransport = true;
			}
		}
		public static void ResetTransport() {
			lock(initializeSyncRoot) {
				IDisposable disposable = logServer as IDisposable;
				hasTransport = false;
				logServer = new DummyServer();
				if(disposable != null) {
					disposable.Dispose();
				}
				serverType = STR_Dummy;
			}
		}
		static void Init() {
#if !SL && !CF && !DXPORTABLE
			ProfilerConfigSection config = (ProfilerConfigSection)ConfigurationManager.GetSection("DevExpressXpoProfiler");
			if((config != null) && (!string.IsNullOrEmpty(config.ServerType)) && (!string.IsNullOrEmpty(config.ServerAssembly))) {
				Type type = XPTypeActivator.GetType(config.ServerAssembly, config.ServerType);
				SetTransport((ILogger)Activator.CreateInstance(type, config.Port), config.ServerType, config.Categories);
				return;
			}
			hasTransport = false;
#else
			hasCategoryList = false;
			initialized = true;
			hasTransport = false;
#endif
		}
		public delegate T LogHandler<T>();
		public delegate void LogHandlerVoid();
		public delegate T MessageHandler<T>(TimeSpan duration);
		public delegate bool ExceptionHandler(Exception ex);
		public static T LogMany<T>(string category, LogHandler<T> handler, MessageHandler<LogMessage[]> createMessageHandler) {
			LogMessageTimer sw = null;
			string exceptionText = null;
			if(LogManager.IsLogActive(category)) {
				sw = new LogMessageTimer();
			}
			try {
				if(handler == null) return default(T);
				return handler();
			} catch(Exception ex) {
				exceptionText = ex.ToString();
				throw;
			} finally {
				if(sw != null && createMessageHandler != null) {
					LogMessage[] messages = createMessageHandler(sw.Stop());
					LogMessageParameter stackParameter = null;
					if(includeStackTrace) {
#if !DXPORTABLE
#if !SL
						stackParameter = new LogMessageParameter(LogParam_StackTrace, new StackTrace(1).ToString());
#else
						stackParameter = new LogMessageParameter(LogParam_StackTrace, new StackTrace().ToString());
#endif
#endif
					}
					if(stackParameter != null || exceptionText != null) {
						foreach(LogMessage message in messages) {
							if(stackParameter != null) message.ParameterList.Add(stackParameter);
							if(exceptionText != null) message.Error = exceptionText;
						}
					}
					LogManager.LogServer.Log(messages);
				}
			}
		}
		public static T Log<T>(string category, LogHandler<T> handler, MessageHandler<LogMessage> createMessageHandler) {
			LogMessageTimer sw = null;
			string exceptionText = null;
			if(LogManager.IsLogActive(category)) {
				sw = new LogMessageTimer();
			}
			try {
				if(handler == null) return default(T);
				return handler();
			} catch(Exception ex) {
				exceptionText = ex.ToString();
				throw;
			} finally {
				if(sw != null && createMessageHandler != null) {
					LogMessage message = createMessageHandler(sw.Stop());
					if(IncludeStackTrace) {
#if !DXPORTABLE
#if !SL
						message.ParameterList.Add(new LogMessageParameter(LogParam_StackTrace, new StackTrace(1).ToString()));						
#else
						message.ParameterList.Add(new LogMessageParameter(LogParam_StackTrace, new StackTrace().ToString()));
#endif
#endif
					}
					if(exceptionText != null) {
						message.Error = exceptionText;
					}
					LogManager.LogServer.Log(message);
				}
			}
		}
		public static void Log(string category, LogHandlerVoid handler, MessageHandler<LogMessage> createMessageHandler, ExceptionHandler exceptionHandler) {
			LogMessageTimer sw = null;
			string exceptionText = null;
			if(LogManager.IsLogActive(category)) {
				sw = new LogMessageTimer();
			}
			try {
				if(handler == null) return;
				handler();
			} catch(Exception ex) {
				exceptionText = ex.ToString();
				if(exceptionHandler == null || exceptionHandler(ex)) {
					throw;
				}
			} finally {
				if(sw != null && createMessageHandler != null) {
					LogMessage message = createMessageHandler(sw.Stop());
					if(IncludeStackTrace) {
#if !DXPORTABLE
#if !SL
						message.ParameterList.Add(new LogMessageParameter(LogParam_StackTrace, new StackTrace(1).ToString()));
#else
						message.ParameterList.Add(new LogMessageParameter(LogParam_StackTrace, new StackTrace().ToString()));
#endif
#endif
					}
					if(exceptionText != null) {
						message.Error = exceptionText;
					}
					LogManager.LogServer.Log(message);
				}
			}
		}
	}
#if !SL && !CF && !DXPORTABLE
	public class ProfilerConfigSection : ConfigurationSection {
		public ProfilerConfigSection() { }
		[ConfigurationProperty("serverType")]
		public string ServerType {
			get { return (string)this["serverType"]; }
			set { this["serverType"] = value; }
		}
		[ConfigurationProperty("serverAssembly")]
		public string ServerAssembly {
			get { return (string)this["serverAssembly"]; }
			set { this["serverAssembly"] = value; }
		}
		[ConfigurationProperty("categories")]
		public string Categories {
			get { return (string)this["categories"]; }
			set { this["categories"] = value; }
		}
		[ConfigurationProperty("port")]
		public int Port {
			get { return (int)this["port"]; }
			set { this["port"] = value; }
		}
	}
#endif
}
