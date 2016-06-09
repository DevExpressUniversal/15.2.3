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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	public abstract class iCalendarObject : ICalendarNamedObject {
		protected iCalendarObject() {
		}
		#region ICalendarNamedObject Members
		public abstract string Name { get;	}
		#endregion
	}
	public enum iCalendarBodyItemType {
		ComponentStart,
		Property,
		ComponentEnd
	}
	public abstract class iCalendarBodyItem : iCalendarObject, ISupportCalendarTimeZone, IWritable {
		public virtual iCalendarBodyItemType BodyItemType { get { return iCalendarBodyItemType.Property; } }
		protected abstract void ApplyTimeZone(TimeZoneManager timeZoneManager);
		protected abstract void WriteToStream(iCalendarWriter cw);
		void ISupportCalendarTimeZone.ApplyTimeZone(TimeZoneManager manager) {
			ApplyTimeZone(manager);
		}
		void IWritable.WriteToStream(iCalendarWriter cw) {
			WriteToStream(cw);
		}
	}
	public interface ICalendarBodyItemContainer {
		void AddObject(iCalendarBodyItem item);
	}
	public interface ICalendarNamedObject {
		string Name { get; }
	}
	#region iCalendarNamedObjectCollection
	public class iCalendarNamedObjectCollection<T> : DXCollection<T> where T : ICalendarNamedObject {
		Dictionary<string, T> entries = new Dictionary<string, T>();
		#region Properties
		public T this[string name] {
			get {
				if (Entries.ContainsKey(name))
					return (T)Entries[name];
				return default(T);
			}
		}
		protected Dictionary<string, T> Entries { get { return entries; } }
		#endregion
		#region ContainKeys
		public bool ContainKeys(string name) {
			return Entries.ContainsKey(name);
		}
		#endregion
		#region OnInsertComplete
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			Entries[value.Name] = value;
		}
		#endregion
		#region OnRemoveComplete
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			T val;
			if (Entries.TryGetValue(value.Name, out val))
				Entries.Remove(value.Name);
		}
		#endregion
		#region OnClearComplete
		protected override void OnClearComplete() {
			base.OnClearComplete();
			Entries.Clear();
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.iCalendar.Internal {
	public interface ISupportCalendarTimeZone {
		void ApplyTimeZone(TimeZoneManager timeZoneManager);
	}
}
