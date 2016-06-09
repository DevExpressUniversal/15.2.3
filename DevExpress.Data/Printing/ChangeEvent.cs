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
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting {
	public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);
	public class ChangeEventArgs : EventArgs {
		#region inner classes
		class EventInfo {
			internal static EventInfo Create(string name, object value) { return new EventInfo(name, value); }
			private string name = "";
			private object value = null;
			public string Name { get { return name; } }
			public virtual object Value { get { return value; } set { this.value = value; } }
			public EventInfo(string name, object value) {
				this.name = name;
				this.value = value;
			}
		}
		#endregion
		private string eventName;
#if DXRESTRICTED
		private DevExpress.Xpf.Collections.SortedList infoList;
#else
		private SortedList infoList;
#endif
		public string EventName {
			get { return eventName; }
		}
		public ChangeEventArgs(string eventName) {
			this.eventName = eventName;
#if DXRESTRICTED
			infoList = new DevExpress.Xpf.Collections.SortedList();
#else
			infoList = new SortedList();
#endif
		}
		public void Add(string name, object value) {
			infoList.Add(name, EventInfo.Create(name, value));
		}
		public object ValueOf(string name) {
			try {
				EventInfo o = infoList[name] as EventInfo;
				return o != null ? o.Value : null;
			}
			catch {
				return null;
			}
		}
	}
}
