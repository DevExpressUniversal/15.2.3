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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public abstract class BaseActionContainersEditor : BaseActionsUiEditor {
		private bool HasActionContainer(BarItem barItem) {
			foreach(IActionControlContainer actionContainer in ActionContainers) {
				if(actionContainer is BarLinkActionControlContainer && ((BarLinkActionControlContainer)actionContainer).BarContainerItem == barItem) return true;
			}
			return false;
		}
		private bool IsSuitableBarItem(BarItem barItem) {
			return (barItem.GetType() ==  typeof(BarLinkContainerItem) || barItem.GetType() == typeof(BarLinkContainerExItem)) && !HasActionContainer(barItem);
		}
		private BarLinkActionControlContainer CreateBarLinkActionControlContainer(BarLinkContainerItem barItem) {
			BarLinkActionControlContainer container = new BarLinkActionControlContainer(barItem.Caption, barItem);
			ActionContainers.Add(container);
			return container;
		}
		private void AddBarLinkActionControlContainerNode(BarLinkContainerItem barItem) {
			BarLinkActionControlContainer container = CreateBarLinkActionControlContainer(barItem);
			FormContainer.Add(container);
			string nodeText = GetFormattedNodeText(container.ActionCategory, barItem.Name);
			CreateTreeNode(nodeText, container);
			BarControlsList.Items.Remove(barItem);
			ActionsUiTree.Focus();
		}
		private void RemoveBarLinkActionControlContainer(BarLinkActionControlContainer actionContainer) {
			if(actionContainer != null) {
				BarControlsList.Items.Add(BarManagerItems[BarManagerItems.IndexOf(actionContainer.BarContainerItem)]);
				ActionContainers.Remove(actionContainer);
				FormContainer.Remove(actionContainer);
			}
		}
		private BarItem GetBarItem(IDataObject data) {
			BarItem barItem = (BarItem)data.GetData(typeof(BarLinkContainerExItem));
			if(barItem == null) {
				barItem = (BarItem)data.GetData(typeof(BarLinkContainerItem));
			}
			return barItem;
		}
		protected abstract void CreateTreeNode(string caption, object actionUiItem);
		protected virtual string GetNodeText(IActionControlContainer actionContainer) {
			if(actionContainer as BarLinkActionControlContainer == null) return string.Empty;
			BarLinkActionControlContainer container = actionContainer as BarLinkActionControlContainer;
			return GetFormattedNodeText(actionContainer.ActionCategory, container.BarContainerItem.Name);
		}
		protected override void FillControlsList() {
			BarControlsList.Items.Clear();
			foreach(BarItem barItem in BarManagerItems) {
				if(barItem.ShowInCustomizationForm && IsSuitableBarItem(barItem)) {
					BarControlsList.Items.Add(barItem);
				}
			}
		}
		protected override void FillActionUiElements() {
			ActionsUiTree.Nodes.Clear();
			foreach(IActionControlContainer actionContainer in ActionContainers) {
				CreateTreeNode(GetNodeText(actionContainer), actionContainer);
			}
		}
		protected override void OnControlDragDrop(DragEventArgs e) {
			BarLinkContainerItem barItem = (BarLinkContainerItem)GetBarItem(e.Data);
			if(barItem != null) {
				AddBarLinkActionControlContainerNode(barItem);
			}
		}
		protected override bool GetDeleteEnabled(TreeNode node) {
			return node != null && node.Tag is BarLinkActionControlContainer;
		}
		protected override void DeleteActionUiElement(TreeNode[] selectedNodes) {
			if(selectedNodes == null) return;
			foreach(TreeNode node in selectedNodes) {
				RemoveBarLinkActionControlContainer(node.Tag as BarLinkActionControlContainer);
				ActionsUiTree.Nodes.Remove(node);
			}
		}
		protected override string DescriptionText {
			get { return "You can add / delete Action control containers and customize them. Drag BarLinkContainerItem from the 'Bar Container Controls' panel to the Action Containers list to create an Action container for it."; }
		}
		protected abstract BarItems BarManagerItems { get; }
		protected abstract IList<IActionControlContainer> ActionContainers { get; }
		protected abstract IContainer FormContainer { get; }
	}
}
