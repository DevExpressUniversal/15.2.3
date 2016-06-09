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
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Templates.ActionContainers {
	public class SimpleActionContainer : IActionContainer {
		private string containerId;
		private List<ActionBase> actions = new List<ActionBase>();
		private void OnActionsClearing() {
			if(ActionsClearing != null) {
				ActionsClearing(this, EventArgs.Empty);
			}
		}
		public SimpleActionContainer() {
		}
		public SimpleActionContainer(string containerId) {
			this.containerId = containerId;
		}
		public virtual string ContainerId {
			get { return containerId; }
			set { containerId = value; }
		}
		public virtual void Register(ActionBase action) {
			actions.Add(action);
		}
		public virtual void Clear() {
			OnActionsClearing();
			actions.Clear();
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public System.Collections.ObjectModel.ReadOnlyCollection<ActionBase> Actions {
			get { return actions.AsReadOnly(); }
		}
		public virtual void Dispose() {
			actions.Clear();
		}
		public void BeginUpdate() {
		}
		public void EndUpdate() {
		}
		public event EventHandler<EventArgs> ActionsClearing;
	}
	public class ActionContainerCollection : List<IActionContainer> {
		public IActionContainer TryAdd(IActionContainer actionContainer) {
			if(!Contains(actionContainer)) {
				Add(actionContainer);
				return actionContainer;
			}
			return null;
		}
		public IList<IActionContainer> TryAdd(IEnumerable<IActionContainer> actionContainers) {
			List<IActionContainer> result = new List<IActionContainer>();
			foreach(IActionContainer actionContainer in actionContainers) {
				IActionContainer addedActionContainer = TryAdd(actionContainer);
				if(addedActionContainer != null) {
					result.Add(addedActionContainer);
				}
			}
			return result;
		}
	}
}
