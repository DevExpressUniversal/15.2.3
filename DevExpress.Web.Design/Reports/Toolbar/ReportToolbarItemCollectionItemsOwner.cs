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

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.Web;
namespace DevExpress.Web.Design.Reports.Toolbar {
	public class ReportToolbarItemCollectionItemsOwner : ItemsEditorOwner {
		readonly ReportToolbarItemCollection toolbarItems;
		public ReportToolbarItemCollectionItemsOwner(ASPxWebControl control, string collectionPropertyName, IServiceProvider provider, ReportToolbarItemCollection items)
			: base(control, collectionPropertyName, provider, items) {
			this.toolbarItems = items;
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(ReportToolbarButton), "Button", ButtonEditColumnImageResource);
			AddItemType(typeof(ReportToolbarLabel), "Label", HyperlinkImageResource);
			AddItemType(typeof(ReportToolbarTextBox), "TextBox", TextImageResource);
			AddItemType(typeof(ReportToolbarComboBox), "ComboBox", ComboboxImageResource);
			AddItemType(typeof(ReportToolbarSeparator), "Separator", ItemImageResource);
			AddItemType(typeof(ReportToolbarTemplateItem), "TemplateItem", RibbonTemplateItemImageResource);
		}
		public override Type GetDefaultItemType() {
			return typeof(ReportToolbarButton);
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType> {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.CreateDefaultItems
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType> {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return CreateDefaultItemsItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected DesignEditorDescriptorItem CreateDefaultItemsItem() {
			var item = new DesignEditorDescriptorItem {
				ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems,
				Caption = "Default Items",
				EditorType = DesignEditorDescriptorItemType.Button,
				Enabled = true,
				BeginGroup = true
			};
			PopulateChildItems(item);
			return item;
		}
		public void CreateDefaultItems() {
			BeginUpdate();
			try {
				toolbarItems.AddRange(ReportToolbar.CreateDefaultItemCollection());
				SetFocusedItem(toolbarItems.FirstOrDefault(), true);
			} finally {
				EndUpdate();
			}
		}
		public override List<string> GetViewDependedProperties() {
			var list = base.GetViewDependedProperties();
			list.Add("ItemKind"); 
			return list;
		}
	}
}
