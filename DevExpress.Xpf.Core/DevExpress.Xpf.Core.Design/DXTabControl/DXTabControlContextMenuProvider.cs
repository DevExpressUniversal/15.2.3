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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Core.Design {
	internal class DXTabControlContextMenuProvider : PrimarySelectionContextMenuProvider {
		MenuAction AddDXTabMenuAction {get; set;}
		public DXTabControlContextMenuProvider() {
			AddDXTabMenuAction = new MenuAction("Add Tab");
			AddDXTabMenuAction.Execute += OnAddDXTabMenuActionExecute;
			UpdateItemStatus += OnUpdateItemStatus;
			Items.Add(AddDXTabMenuAction);
		}
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			if(e.Selection.SelectionCount == 1) {
				ModelItem primary = e.Selection.PrimarySelection;
				if(primary.IsItemOfType(typeof(DXTabItem))) {
					ModelItem parent = primary.Parent;
					AddDXTabMenuAction.Visible = parent != null && parent.IsItemOfType(typeof(DXTabControl));
				} else if(primary.IsItemOfType(typeof(DXTabControl))) {
					AddDXTabMenuAction.Visible = true;
				} else {
					ModelItem parent = primary.Parent;
					if(parent != null && parent.IsItemOfType(typeof(DXTabItem))) {
						ModelItem dxTabControl = parent != null ? parent.Parent : null;
						AddDXTabMenuAction.Visible = dxTabControl != null && dxTabControl.IsItemOfType(typeof(DXTabControl));
					}
				}
			} else {
				AddDXTabMenuAction.Visible = false;
			}
		}
		void OnAddDXTabMenuActionExecute(object sender, MenuActionEventArgs e) {
			ModelItem primary = e.Selection.PrimarySelection;
			ModelItem tabControl = null;
			if(primary.IsItemOfType(typeof(DXTabControl))) {
				tabControl = primary;
			} else if(primary.IsItemOfType(typeof(DXTabItem))) {
				tabControl = primary.Parent != null && primary.Parent.IsItemOfType(typeof(DXTabControl)) ?
					primary.Parent : null;				
			}
			if(tabControl != null) {
				using(ModelEditingScope scope = tabControl.BeginEdit("Add a new DXTabItem")) {
					ModelItem tabItem = ModelFactory.CreateItem(e.Context, typeof(DXTabItem), CreateOptions.InitializeDefaults);
					tabControl.Properties["Items"].Collection.Add(tabItem);
					Selection sel = new Selection(tabItem);
					e.Context.Items.SetValue(sel);
					scope.Complete();
				}
			}
		}
	}
}
