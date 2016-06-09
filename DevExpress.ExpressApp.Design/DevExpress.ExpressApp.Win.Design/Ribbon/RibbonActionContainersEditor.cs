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
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Design.Bars;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Design.Ribbon {
	public class RibbonActionContainersEditor : BaseActionContainersEditor {
		private const int BarLinkActionControlContainerImageIndex = 0;
		private const int RibbonGroupActionControlContainerImageIndex = 1;
		private void RemoveRibbonGroupActionControlContainer(RibbonGroupActionControlContainer actionContainer) {
			if(actionContainer != null) {
				Ribbon.Container.Remove(actionContainer);
				Ribbon.ActionContainers.Remove(actionContainer);
			}
		}
		private void InitializeImages() {
			ImageList actionsUiImageList = new ImageList();
			actionsUiImageList.Images.Add(ResourceImageHelper.CreateImageFromResources("DevExpress.ExpressApp.Win.Design.Images.BarLinkActionControlContainer.png", typeof(XafBarAndDockingDesigner).Assembly));
			actionsUiImageList.Images.Add(ResourceImageHelper.CreateImageFromResources("DevExpress.ExpressApp.Win.Design.Images.RibbonGroupActionControlContainer.png", typeof(XafBarAndDockingDesigner).Assembly));
			ActionsUiTree.ImageList = actionsUiImageList;
		}
		protected override void InitializeDesigner() {
			BarControlsList.Manager = Ribbon.Manager;
			InitializeImages();
			base.InitializeDesigner();
		}
		protected override void CreateTreeNode(string caption, object container) {
			int imageIndex = container is RibbonGroupActionControlContainer ? RibbonGroupActionControlContainerImageIndex : BarLinkActionControlContainerImageIndex;
			CreateTreeNode(caption, container, imageIndex);
		}
		protected override bool GetDeleteEnabled(TreeNode node) {
			return base.GetDeleteEnabled(node) || (node != null && node.Tag is RibbonGroupActionControlContainer);
		}
		protected override void DeleteActionUiElement(TreeNode[] selectedNodes) {
			base.DeleteActionUiElement(selectedNodes);
			if(selectedNodes == null) return;
			foreach(TreeNode node in selectedNodes) {
				RemoveRibbonGroupActionControlContainer(node.Tag as RibbonGroupActionControlContainer);
				ActionsUiTree.Nodes.Remove(node);
			}
			Ribbon.FireRibbonChanged(Ribbon);
		}
		protected override string GetNodeText(IActionControlContainer actionContainer) {
			string text = base.GetNodeText(actionContainer);
			if(string.IsNullOrEmpty(text) && actionContainer as RibbonGroupActionControlContainer != null) {
				RibbonGroupActionControlContainer container = actionContainer as RibbonGroupActionControlContainer;
				text = String.Format("{0} (Obsolete)", GetFormattedNodeText(container.ActionCategory, container.RibbonGroup.Name));
			}
			return text;
		}
		protected override BarItems BarManagerItems {
			get { return Ribbon.Manager.Items; }
		}
		protected override IList<IActionControlContainer> ActionContainers {
			get { return Ribbon.ActionContainers; }
		}
		protected override IContainer FormContainer {
			get { return Ribbon.Container; }
		}
		public XafRibbonControlV2 Ribbon {
			get {
				if(EditingObject == null) return null;
				return EditingObject as XafRibbonControlV2;
			}
		}
	}
}
