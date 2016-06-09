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
using DevExpress.XtraReports.UI;
using System.Collections;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Serialization {
	public class EventSerializer {
		public const string ResourceName = "XtraReport.Events";
		XRControl rootControl;
		Hashtable targetHT;
		public string Serialize(XRControl rootControl) {
			this.rootControl = rootControl;
			StringBuilder resultBuilder = new StringBuilder();
			NestedComponentEnumerator en = new NestedComponentEnumerator(new object[] { rootControl });
			while(en.MoveNext())
				resultBuilder.Append(EventsToString(en.Current.Name, en.Current.GetEventHandlers()));
			if(rootControl is XtraReport)
				resultBuilder.Append(SerializeCalculatedFields((rootControl as XtraReport).CalculatedFields));
			return resultBuilder.ToString().TrimEnd(';');
		}
		public string SerializeCalculatedFields(CalculatedFieldCollection calculatedFields) {
			StringBuilder resultBuilder = new StringBuilder();
			foreach(CalculatedField calculatedField in calculatedFields)
				resultBuilder.Append(EventsToString(calculatedField.Name, calculatedField.GetEventHandlers()));
			return resultBuilder.ToString().TrimEnd(';');
		}
		string EventsToString(string name, Hashtable handlers) {
			string s = "";
			IDictionaryEnumerator en = handlers.GetEnumerator();
			while(en.MoveNext()) {
				string eventString = EventToString((System.Reflection.EventInfo)en.Key, (Delegate)en.Value);
				if(eventString.Length > 0)
					s += name + "," + eventString + ";";
			}
			return s;
		}
		string EventToString(System.Reflection.EventInfo eventInfo, Delegate handler) {
			StringBuilder resultBuilder = new StringBuilder();
			Delegate[] handlers = handler.GetInvocationList();
			for(int i = 0; i < handlers.Length; i++) {
				if(handlers[i] == null || Comparer.Equals(handlers[i].Target, rootControl)) {
					resultBuilder.Append(handlers[i].Method.Name);
					resultBuilder.Append(",");
				}
			}
			string result = string.Empty;
			if(resultBuilder.Length > 0)
				result = eventInfo.Name + "," + resultBuilder.ToString();
			return result.TrimEnd(',');
		}
		public void Deserialize(XRControl rootControl, string value) {
			if(String.IsNullOrEmpty(value))
				return;
			this.rootControl = rootControl;
			targetHT = new Hashtable();
			NestedComponentEnumerator en = new NestedComponentEnumerator(new object[] { rootControl });
			while(en.MoveNext())
				targetHT[en.Current.Name] = en.Current;
			if(rootControl is XtraReport)
				CollectCalculatedFields((rootControl as XtraReport).CalculatedFields);
			string[] items = value.Split(';');
			for(int i = 0; i < items.Length; i++)
				AddEventHandlers(items[i]);
		}
		void CollectCalculatedFields(CalculatedFieldCollection calculatedFields) {
			foreach(CalculatedField calculatedField in calculatedFields)
				targetHT[calculatedField.Name] = calculatedField;
		}
		void AddEventHandlers(string s) {
			string[] items = s.Split(',');
			System.Diagnostics.Debug.Assert(items.Length >= 3);
			object target = targetHT[items[0]];
			if(target != null) {
				for(int i = 2; i < items.Length; i++)
					AddEventHandler(target, items[i], items[1]);
			}
		}
		void AddEventHandler(object target, string method, string name) {
			if(method.Length > 0) {
				try {
					System.Reflection.EventInfo eventInfo = target.GetType().GetEvent(name);
					Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, rootControl, method);
					eventInfo.RemoveEventHandler(target, handler);
					eventInfo.AddEventHandler(target, handler);
				} catch { }
			}
		}
	}
}
