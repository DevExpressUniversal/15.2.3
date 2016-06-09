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
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Core {
	public class ActionManipulationEventArgs : EventArgs {
		private ActionBase action;
		public ActionManipulationEventArgs(ActionBase action) {
			this.action = action;
		}
		public ActionBase Action {
			get { return action; }
		}
	}
	public class ActionList : IEnumerable<ActionBase>, ICollection {
		private LightDictionary<string, ActionBase> items = new LightDictionary<string, ActionBase>();
		public void Add(ActionBase action) {
			if(!items.ContainsValue(action)) {
				items.Add(action.Id, action);
				if(ActionAdded != null) {
					ActionAdded(this, new ActionManipulationEventArgs(action));
				}
			}
		}
		public void AddRange(IEnumerable actions) {
			foreach(ActionBase action in actions) {
				Add(action);
			}
		}
		public bool Contains(ActionBase action) {
			return items.ContainsValue(action);
		}
		public void Clear() {
			items.Clear();
		}
		public override string ToString() {
			return base.ToString() + " " + items.ToString();
		}
		public ActionBase this[string id] {
			get {
				ActionBase result = null;
				if(!items.TryGetValue(id, out result)) {
					foreach(ActionBase item in items.Values) {
						if(item.Id == id) {
							return item;
						}
					}
				}
				return result;
			}
		}
		public int Count {
			get { return items.Count; }
		}
		public event EventHandler<ActionManipulationEventArgs> ActionAdded;
		IEnumerator<ActionBase> IEnumerable<ActionBase>.GetEnumerator() {
			return items.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			((ICollection)items.GetValues()).CopyTo(array, index);
		}
		int ICollection.Count {
			get { return Count; }
		}
		bool ICollection.IsSynchronized {
			get { return ((ICollection)items.GetValues()).IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return ((ICollection)items.GetValues()).SyncRoot; }
		}
	}
}
