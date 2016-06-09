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
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Features;
using System.Runtime.Remoting.Contexts;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Ribbon.Design {
	class RibbonPageGroupContextMenuProvider : ContainerContextMenuProvider {
		MenuAction AddRibbonPageGroup { get; set; }
		MenuAction AddRibbonGalleryItem { get; set; }
		MenuAction AddBarButtonGroup { get; set; }
		protected virtual void OnAddBarMenuItemExecute(object sender, MenuActionEventArgs e) {
			ModelItem group = e.Selection.PrimarySelection;
			ModelItem barItem = null;
			if(sender.Equals(AddRibbonGalleryItem))
				barItem = RibbonDesignTimeHelper.CreateRibbonGalleryItem(e.Context);
			else if(sender.Equals(AddBarButtonGroup))
				barItem = BarManagerDesignTimeHelper.CreateBarItem(e.Context, typeof(BarButtonGroup));
			if(barItem == null) return;
			using(ModelEditingScope scope = group.BeginEdit(((MenuAction)sender).DisplayName)) {
				group.Properties["Items"].Collection.Add(barItem);
				scope.Complete();
			}
		}
		protected override void OnDeleteActionExecute(object sender, MenuActionEventArgs e) {
			ModelItem ribbonPageGroup = e.Selection.PrimarySelection;
			RibbonDesignTimeHelper.RemoveRibbonPageGroup(ribbonPageGroup);
		}
		protected override void AddItemsForGroup() {
			AddRibbonPageGroup = new MenuAction("Add RibbonPageGroup") { Visible = false };
			Items.Add(AddRibbonPageGroup);
			base.AddItemsForGroup();
			AddBarButtonGroup = new MenuAction("BarButtonGroup");
			AddBarButtonGroup.Execute += OnAddBarMenuItemExecute;
			AddRibbonGalleryItem = new MenuAction("RibbonGalleryBarItem");
			AddRibbonGalleryItem.Execute += OnAddBarMenuItemExecute;
			AddBarItemGroup.Items.Add(AddBarButtonGroup);
			AddBarItemGroup.Items.Add(AddRibbonGalleryItem);
		}
	}
}
