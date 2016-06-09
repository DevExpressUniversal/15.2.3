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
using DevExpress.Utils;
namespace DevExpress.ExpressApp.SystemModule {
	public class ActionCollection : IEnumerable<ActionBase> {
		private IDictionary<string, ActionBase> actionById;
		public ActionCollection() {
			actionById = new Dictionary<string, ActionBase>();
		}
		public void Add(ActionBase action) {
			Guard.ArgumentNotNull(action, "action");
			string actionId = action.Id;
			Guard.ArgumentNotNull(actionId, "action.Id");
			ActionBase registeredAction;
			if(!actionById.TryGetValue(actionId, out registeredAction)) {
				actionById.Add(actionId, action);
			}
			else if(action != registeredAction) {
				string message = string.Format("Actions with the duplicate '{0}' identifier are detected in the '{1}' and '{2}' controllers. For a solution, see http://www.devexpress.com/kb=T191034.", actionId, action.Controller, registeredAction.Controller);
				throw new InvalidOperationException(message);
			}
		}
		public void AddRange(IEnumerable<ActionBase> actions) {
			Guard.ArgumentNotNull(actions, "actions");
			foreach(ActionBase action in actions) {
				Add(action);
			}
		}
		public ActionBase Find(string id) {
			Guard.ArgumentNotNull(id, "id");
			ActionBase action;
			if(actionById.TryGetValue(id, out action)) {
				return action;
			}
			return null;
		}
		public void Clear() {
			actionById.Clear();
		}
		public int Count {
			get { return actionById.Count; }
		}
		IEnumerator<ActionBase> IEnumerable<ActionBase>.GetEnumerator() {
			return actionById.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<ActionBase>)this).GetEnumerator();
		}
	}
}
