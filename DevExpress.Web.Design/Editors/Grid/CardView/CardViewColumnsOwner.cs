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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Web.Design {
	class CardViewColumnsOwner : DataItemsEditorOwner {
		public CardViewColumnsOwner(ASPxCardView cardView) 
			: base(cardView, "Columns", ((IComponent)cardView).Site, cardView.Columns) {
		}
		protected ASPxCardView CardView { get { return (ASPxCardView)Component; } }
		protected override string KeyFieldName { get { return CardView.KeyFieldName; } set { CardView.KeyFieldName = value; } }
		public override IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			var item = base.CreateNewItem(designTimeItem);
			var binaryImageColumn = item as CardViewBinaryImageColumn;
			if(binaryImageColumn != null)
				binaryImageColumn.PropertiesBinaryImage.EditingSettings.Enabled = true;
			return item;
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(CardViewBinaryImageColumn), "Binary Image Column", "Binary Image", BinaryImageColumnImageResource);
			AddItemType(typeof(CardViewButtonEditColumn), "Button Edit Column", "Button Edit", ButtonEditColumnImageResource);
			AddItemType(typeof(CardViewCheckColumn), "Check Column", "Check Box", CheckColumnImageResource);
			AddItemType(typeof(CardViewColorEditColumn), "Color Edit Column", "Color Edit", ColorEditImageResource);
			AddItemType(typeof(CardViewComboBoxColumn), "Combo Box Column", "Combo Box", ComboboxImageResource);
			AddItemType(typeof(CardViewDateColumn), "Date Column", "Date Edit", DateEditImageResource);
			AddItemType(typeof(CardViewDropDownEditColumn), "DropDown Edit Column", "DropDown Edit", DropDownEditImageResource);
			AddItemType(typeof(CardViewHyperLinkColumn), "Hyperlink Column", "Hyperlink", HyperlinkImageResource);
			AddItemType(typeof(CardViewImageColumn), "Image Column", "Image", ImageImageResource);
			AddItemType(typeof(CardViewMemoColumn), "Memo Column", "Memo", MemoImageResource);
			AddItemType(typeof(CardViewProgressBarColumn), "Progress Bar Column", "Progress Bar", ProgressBarImageResource);
			AddItemType(typeof(CardViewSpinEditColumn), "Spin Edit Column", "Spin Edit", SpinEditImageResource);
			AddItemType(typeof(CardViewTextColumn), "Text Column", "Text Box", TextImageResource);
			AddItemType(typeof(CardViewTimeEditColumn), "Time Edit Column", "Time Edit", TimeEditImageResource);
			AddItemType(typeof(CardViewTokenBoxColumn), "Token Box Column", "Token Box", TokenBoxImageResource);
			AddItemType(typeof(CardViewEditColumn), "Data Column", DataColumnImageResource);
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.RetriveFields
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		protected override IDesignTimeCollectionItem CreateDataItemCore(string fieldName) {
			CardView.AutoGenerateColumns = false;
			return CreteDataColumn(fieldName);
		}
		CardViewEditColumn CreteDataColumn(string fieldName) {
			var fieldInfo = FieldInfoList.FirstOrDefault(f => f.Name == fieldName);
			if(fieldInfo == null)
				return new CardViewTextColumn() { FieldName = fieldName };
			var column = CardViewEditColumn.CreateColumn(fieldInfo.DataType);
			column.FieldName = fieldInfo.Name;
			column.ReadOnly = fieldInfo.IsPrimaryKey || fieldInfo.IsReadOnly;
			if(fieldInfo.Identity) 
				column.Visible = false;
			return column;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			return designTimeItem.ColumnType != typeof(CardViewEditColumn) ? base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar) : false;
		}
		protected internal override string GetItemPropertiesTabCaption() {
			return "Column Properties";
		}
		public override Type GetDefaultItemType() {
			return typeof(CardViewTextColumn);
		}
	}
	class CardViewFormLayoutItemsOwner : GridViewFormLayoutItemsOwner {
		public CardViewFormLayoutItemsOwner(ASPxWebControl component, CardViewFormLayoutProperties properties, ItemsEditorOwner editorItemsOwner)
			: base(component, properties, editorItemsOwner) {
		}
		internal override Type LayoutItemType { get { return typeof(CardViewColumnLayoutItem); } }
		protected override void FillItemTypes() {
			AddItemType(typeof(CardViewColumnLayoutItem), "Column Layout Item", string.Empty);
			AddItemType(typeof(CardViewCommandLayoutItem), "Command Layout Item", string.Empty);
			AddItemType(typeof(EmptyLayoutItem), "Empty Layout Item", string.Empty);
			AddItemType(typeof(CardViewLayoutGroup), "Layout Group", string.Empty);
			AddItemType(typeof(CardViewTabbedLayoutGroup), "Tabbed Layout Group", string.Empty);
		}
	}
	public class CardViewSummaryItemsOwner : FlatCollectionItemsOwner<ASPxCardViewSummaryItem> {
		public CardViewSummaryItemsOwner(ASPxCardView cardView, IServiceProvider provider, string summaryName, ASPxCardViewSummaryItemCollection summaryCollection)
			: base(cardView, provider, summaryCollection, summaryName) {
		}
	}
}
