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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public abstract class BaseActionControlsEditor : BaseActionsUiEditor {
		private const int ActionControlImageIndex = 0;
		private IList editingObjectItems;
		private IList<IActionControl> editingObjectActionControls;
		private IContainer container;
		private void InitBarManagerDependentProperties(XafBarManagerV2 manager) {
			BarControlsList.Manager = manager;
			container = manager.Container;
			editingObjectItems = manager.Items;
			editingObjectActionControls = manager.ActionControls;
		}
		private void InitRibbonManagerDependentProperties(XafRibbonControlV2 ribbon) {
			BarControlsList.Manager = ribbon.Manager;
			container = ribbon.Container;
			editingObjectItems = ribbon.Items;
			editingObjectActionControls = ribbon.ActionControls;
		}
		private void InitManagerDependentProperties() {
			if(EditingObject is XafBarManagerV2) {
				InitBarManagerDependentProperties((XafBarManagerV2)EditingObject);
			}
			else if(EditingObject is XafRibbonControlV2) {
				InitRibbonManagerDependentProperties((XafRibbonControlV2)EditingObject);
			}
		}
		private bool HasActionControl(BarItem barItem) {
			foreach(IActionControl actionControl in editingObjectActionControls) {
				if(actionControl.NativeControl == barItem) return true;
			}
			return false;
		}
		private IActionControl CreateActionControl(BarItem barItem) {
			IActionControl actionControl = CreateActionControlCore(barItem);
			if(actionControl != null) {
				editingObjectActionControls.Add(actionControl);
			}
			return actionControl;
		}
		private void AddActionControl(BarItem barItem) {
			IActionControl actionControl = CreateActionControl(barItem);
			if(actionControl != null) {
				container.Add((IComponent)actionControl);
				string nodeText = GetFormattedNodeText(actionControl.ActionId, ((BarItem)actionControl.NativeControl).Name);
				CreateTreeNode(nodeText, actionControl, ActionControlImageIndex);
				BarControlsList.Items.Remove(barItem);
			}
		}
		private void RemoveActionControl(IActionControl actionControl) {
			if(actionControl != null) {
				BarControlsList.Items.Add(editingObjectItems[editingObjectItems.IndexOf(actionControl.NativeControl)]);
				editingObjectActionControls.Remove(actionControl);
				container.Remove((IComponent)actionControl);
			}
		}
		protected abstract bool IsValidBarItem(BarItem barItem);
		protected abstract bool IsValidActionUiElement(IActionControl actionControl);
		protected abstract BarItem GetBarItem(IDataObject data);
		protected abstract IActionControl CreateActionControlCore(BarItem barItem);
		protected override void OnControlDragDrop(DragEventArgs e) {
			BarItem barItem = GetBarItem(e.Data);
			if(barItem != null) {
				AddActionControl(barItem);
				ActionsUiTree.Focus();
			}
		}
		protected override bool GetDeleteEnabled(TreeNode node) {
			return node != null && node.Tag is IActionControl;
		}
		protected override void DeleteActionUiElement(TreeNode[] selectedNodes) {
			if(selectedNodes == null) return;
			foreach(TreeNode node in selectedNodes) {
				RemoveActionControl(node.Tag as IActionControl);
				ActionsUiTree.Nodes.Remove(node);
				ActionsUiTree.UpdateSelection();
			}
		}
		protected override void FillControlsList() {
			BarControlsList.Items.Clear();
			foreach(BarItem barItem in editingObjectItems) {
				if(IsValidBarItem(barItem) && !HasActionControl(barItem)) {
					BarControlsList.Items.Add(barItem);
				}
			}
		}
		protected override void FillActionUiElements() {
			ActionsUiTree.Nodes.Clear();
			foreach(IActionControl actionControl in editingObjectActionControls) {
				if(IsValidActionUiElement(actionControl)) {
					string nodeText = GetFormattedNodeText(actionControl.ActionId, ((BarItem)actionControl.NativeControl).Name);
					CreateTreeNode(nodeText, actionControl, ActionControlImageIndex);
				}
			}
		}
		protected override void InitializeDesigner() {
			InitManagerDependentProperties();
			ActionsUiTree.ImageList = ResourceImageHelper.CreateImageListFromResources("DevExpress.ExpressApp.Win.Design.Images.ActionControl.png", typeof(XafBarAndDockingDesigner).Assembly, new Size(16, 16));
			base.InitializeDesigner();
		}
		protected override string BarControlsCaption {
			get { return "Bar Controls:"; }
		}
		protected override string ActionsUiCaption {
			get { return "Action Controls:"; }
		}
	}
}
