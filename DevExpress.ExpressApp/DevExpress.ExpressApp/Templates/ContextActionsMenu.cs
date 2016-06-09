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
using System.Text;
using DevExpress.ExpressApp.Actions;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Templates {
	public class ContextActionsMenuContainer : IActionContainer {
		private string name;
		private List<ActionBase> actions = new List<ActionBase>();
		public ContextActionsMenuContainer(string name) {
			this.name = name;
		}
		public string ContainerId {
			get { return name; }
			set { name = value; }
		}
		public void Register(ActionBase action) {
			actions.Add(action);
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<ActionBase> Actions {
			get { return actions.AsReadOnly(); }
		}
		public void Dispose() {
			if(actions != null) {
				actions.Clear();
				actions = null;
			}
		}
		public void BeginUpdate() {
		}
		public void EndUpdate() {
		}
	}
	public class ContextActionsMenu : IDisposable {
		private IFrameTemplate parent;
		private List<IActionContainer> containers;
		public ContextActionsMenu(IFrameTemplate parent, params IActionContainer[] containers) {
			this.parent = parent;
			this.containers = new List<IActionContainer>(containers);
		}
		public ContextActionsMenu(IFrameTemplate parent, params string[] containerNames) {
			this.parent = parent;
			this.containers = new List<IActionContainer>();
			foreach(string containerName in containerNames) {
				containers.Add(new ContextActionsMenuContainer(containerName));
			}
		}
		public void RegisterActionContainers(params string[] containerNames) {
			foreach(string name in containerNames) {
				containers.Add(new ContextActionsMenuContainer(name));
			}
		}
		public void CreateControls(View context) {
			ListView listView = context as ListView;
			if(listView != null && listView.Editor.ContextMenuTemplate != null) {
				listView.Editor.ContextMenuTemplate.CreateActionItems(parent, listView, containers);
			}
		}
		public void Dispose() {
			if(containers != null) {
				foreach(IActionContainer container in containers) {
					container.Dispose();
				}
				containers.Clear();
				containers = null;
			}
			parent = null;
		}
		public List<IActionContainer> Containers {
			get { return containers; }
		}
	}
}
