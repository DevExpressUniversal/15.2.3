#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Design;
using System;
namespace DevExpress.DashboardWin.Design {
	public class DashboardDesignerControlDesigner : BaseControlDesigner {
		DesignerVerbCollection defaultVerbs;
		DesignerActionListCollection actionLists;
		public DashboardDesigner DesignerControl { get { return Control as DashboardDesigner; } }
		public override DesignerVerbCollection DXVerbs { get { return defaultVerbs; } }
		public DashboardDesignerControlDesigner() {
			defaultVerbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About", new EventHandler(OnAboutClick)) });
		}
		void OnAboutClick(object sender, EventArgs e) {
			DashboardDesigner.About();
		}
		protected override DesignerActionListCollection CreateActionLists() {
			if (actionLists == null) {
				actionLists = base.CreateActionLists();
				DashboardDesigner designerControl = DesignerControl;
				if (designerControl != null)
					actionLists.Insert(0, new DashboardDesignerActionList(this));
			}
			return actionLists;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IComponentChangeService changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if (changeService != null) {
				changeService.ComponentAdded += (sender, e) => AutomaticBindHelper.BindToComponent(this, "MenuManager", typeof(IDXMenuManager));
				changeService.ComponentRemoved += (sender, e) => AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "MenuManager", typeof(IDXMenuManager));
			}
		}
	}
}
