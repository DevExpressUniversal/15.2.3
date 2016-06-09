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
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Ribbon.Customization;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public class ActionContainersTreeView : ItemsTreeView {
		protected override void UpdateTreeNodeText(TreeNode node, object item) {
			if(node.Tag as BarLinkActionControlContainer != null && node.Tag as BarLinkActionControlContainer == item) {
				BarLinkActionControlContainer container = node.Tag as BarLinkActionControlContainer;
				node.Text = GetFormattedNodeText(container.ActionCategory, container.BarContainerItem.Name);
			}
			if(node.Tag as RibbonGroupActionControlContainer != null && node.Tag as RibbonGroupActionControlContainer == item) {
				RibbonGroupActionControlContainer container = node.Tag as RibbonGroupActionControlContainer;
				node.Text = GetFormattedNodeText(container.ActionCategory, container.RibbonGroup.Name);
			}
		}
		public string GetFormattedNodeText(string id, string controlName) {
			return String.Format("{0}   <{1}>", id, controlName);
		}
		public override void OnDragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			e.AllowEffects = DragDropEffects.Move;
		}
		public override void OnDragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.Move;
		}
	}
}
