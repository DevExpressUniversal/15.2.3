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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Design {
	public class ModulesTreeViewTrayBase<DataSourceType> : TreeViewTray<DataSourceType> {
		protected override bool AllowAddItem(ToolboxItem item) {
			if(item != null) {
				Type itemType = Designer.GetToolboxItemType(item);
				return typeof(ModuleBase).IsAssignableFrom(itemType);
			}
			return false;
		}
		protected override bool GetHide(object enteredControl) {
			if(Designer is XafModuleRootDesigner) {
				return base.GetHide(enteredControl);
			}
			return enteredControl != this && !(enteredControl is ControllersTreeViewTray || enteredControl is BusinessClassesTreeViewTray);
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			if(SelectedNode != null && SelectedNode.Level > 0) {
				TreeNode node = FindNode(Nodes, ((Type)SelectedNode.Tag).FullName);
				if(node != null) {
					SelectedNode = node;
				}
			}
		}
		protected override void OnDragDrop(DragEventArgs e) {
			ToolboxItem item = Designer.DeserializeToolboxItem(e.Data);
			if(item != null) {
				ProcessToolboxItem(item);
			}
			RefreshNodes();
			base.OnDragDrop(e);
		}
		protected void ProcessToolboxItem(ToolboxItem item) {
			Designer.AddModule(item);
		}
		public ModulesTreeViewTrayBase() { }
		public ModulesTreeViewTrayBase(IContainer container)
			: base(container) {
			ToolTipMessage = "To add modules, drag them from the Toolbox, and use the Properties window to set their properties.";
			canShowPlaceholder = true;
			canShowToolTip = true;
			SetToolTip();
		}
	}
}
