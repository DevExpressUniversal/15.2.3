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
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
namespace DevExpress.Xpf.Core.Native {
#if DEBUGTEST
	public class LogEntry {
		public object Value { get; private set; }
		public string Message { get; private set; }
		public object Sender { get; private set; }
		public string MethodName { get; private set; }
		public LogEntry(object sender, object value, string message, string methodName) {
			Sender = sender;
			Value = value;
			Message = message;
			MethodName = methodName;
		}
	}
	public class LogAddedEntryArgs : EventArgs {
		public LogEntry Entry { get; private set; }
		public LogAddedEntryArgs(LogEntry entry) {
			Entry = entry;
		}
	}
	public abstract class LogBase {
		static Dictionary<DependencyObject, LogBase> logCache = new Dictionary<DependencyObject, LogBase>();
		public static LogBase GetLog(DependencyObject element) {
			LogBase log;
			if (logCache.TryGetValue(element, out log))
				return log;
			return new DummyLog();
		}
		public static LogBase StartLogging(DependencyObject element) {
			return StartLoggingInternal(element, CreateLog);
		}
		public static LogBase StartFullLogging(DependencyObject key) {
			return StartLoggingInternal(key, CreateFullLog);
		}
		protected static LogBase StartLoggingInternal(DependencyObject key, Func<LogBase> CreateDelegate) {
			if (logCache.ContainsKey(key))
				return logCache[key];
			LogBase log = CreateDelegate();
			logCache.Add(key, log);
			return log;
		}
		protected static LogBase CreateLog() {
			return new Log();
		}
		protected static LogBase CreateFullLog() {
			return new FullLog();
		}
		public static void StopLogging(DependencyObject element) {
			if (!logCache.ContainsKey(element))
				return;
			logCache.Remove(element);
		}
		public static void Reset() {
			logCache.Clear();
		}
		public static void Add(object sender, object value, string message = "") {
			foreach (var pair in logCache) {
				LogBase log = pair.Value;
				if (log.CanAdd((DependencyObject)pair.Key, sender as DependencyObject)) {
					log.AddEntry(sender, value, message);
				}
			}
		}
		protected abstract bool CanAdd(DependencyObject key, DependencyObject sender);
		public event EntryAddedEventHandler EntryAdded;
		protected abstract void AddEntry(object sender, object value, string message);
		protected virtual void RaiseEntryAdded(LogEntry logEntry) {
			if (EntryAdded != null)
				EntryAdded(this, new LogAddedEntryArgs(logEntry));
		}
	}
	public class FullLog : Log {
		protected override bool CanAdd(DependencyObject key, DependencyObject sender) {
			return true;
		}
	}
	public class Log : LogBase {
		List<LogEntry> log = new List<LogEntry>();
		protected override void AddEntry(object sender, object value, string message) {
			LogEntry logEntry = new LogEntry(sender, value, message, GetLogAddedMethodName());
			log.Add(logEntry);
			RaiseEntryAdded(logEntry);
		}
		protected override bool CanAdd(DependencyObject key, DependencyObject sender) {
			return LayoutHelper.IsChildElementEx(key, sender, true);
		}
		protected string GetLogAddedMethodName() {
			StackTrace stack = new StackTrace();
			MethodBase method = stack.GetFrame(3).GetMethod();
			return string.Format("{0}.{1}", method.ReflectedType.Name, method.Name);
		}
	}
	public class DummyLog : LogBase {
		protected override void AddEntry(object sender, object value, string message) {
		}
		protected override bool CanAdd(DependencyObject key, DependencyObject sender) {
			return false;
		}
	}
	public class DependencyPropertyValueSnapshot<T, ValueType> {
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty property;
		public DependencyProperty Property { get { return property; } }
		public ValueType Value { get; private set; }
		public T Object { get; private set; }
		public DependencyPropertyValueSnapshot(DependencyProperty property, T @object, ValueType value) {
			this.property = property;
			Value = value;
			Object = @object;
		}
	}
	public class PropertyValueSnapshot<T, ValueType> {
		public string Property { get; private set; }
		public ValueType Value { get; private set; }
		public T Object { get; private set; }
		public PropertyValueSnapshot(string property, T @object, ValueType value) {
			this.Property = property;
			Value = value;
			Object = @object;
		}
	}
	public class EventLogEventAddedEventArgs : EventArgs {
		public object Event { get; private set; }
		public EventLogEventAddedEventArgs(object @event) {
			Event = @event;
		}
	}
	public delegate void EntryAddedEventHandler(object sender, LogAddedEntryArgs args);
	public delegate void EventLogEventAddedEventHandler(object sender, EventLogEventAddedEventArgs e);
	public class EventLog {
		public static readonly EventLog Default = new EventLog();
		public bool IsEnabled { get; set; }
		public IList<object> Events { get; private set; }
		public event EventLogEventAddedEventHandler EventAdded;
		public EventLog() {
			Events = new List<object>();
		}
		public void Clear() {
			Events.Clear();
		}
		public void AddEvent(object @event) {
			if(!IsEnabled)
				return;
			Events.Add(@event);
			if(EventAdded != null)
				EventAdded(this, new EventLogEventAddedEventArgs(@event));
		}
		public T GetEvent<T>(int index) {
			return (T)Events[index];
		}
		public T GetLastEvent<T>() {
			return GetEvent<T>(Events.Count - 1);
		}
		public T GetFirstEvent<T>() {
			return GetEvent<T>(0);
		}
	}
#else
	public abstract class LogBase {
		[Conditional("DEBUGTEST")]
		public static void Add(object sender, object value, string message = "") { }
	}
#endif
}
