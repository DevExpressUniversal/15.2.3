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

using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotVisualItems : SelectionVisualItems {
		public PivotVisualItems(PivotGridViewInfoData data)
			: base(data) {
		}
		public new PivotTreeRowField RowTreeField {
			get { return (PivotTreeRowField)base.RowTreeField; }
		}
		public new PivotFieldItem RowTreeFieldItem {
			get { return (PivotFieldItem)base.RowTreeFieldItem; }
		}
		protected new PivotGridViewInfoData Data {
			get { return (PivotGridViewInfoData)base.Data; }
		}
		protected override void ChangeExpandedCore(PivotFieldValueItem item, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAsync(item, false, asyncCompleted);
		}
		protected override PivotFieldItemBase CreateFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridFieldBase field) {
			return new PivotFieldItem(data, groupItems, (PivotGridField)field);
		}
		protected override PivotGridCellDataProviderBase CreateCellDataProvider() {
			return new PivotGridEditCellDataProvider(Data);
		}
		protected override PivotGridFieldBase CreateRowTreeField() {
			return new PivotTreeRowField(this);
		}
		protected override int ViewportHeight {
			get {
				if(Data.ViewInfo == null || !Data.ViewInfo.IsVScrollBarVisible)
					return 0;
				return Data.ViewInfo.VScrollBarInfo.LargeChange;
			}
		}
	}
	public class PivotFieldItem : PivotFieldItemBase {
		bool canEdit;
		RepositoryItem fieldEdit;
		PivotGridFieldAppearances appearance;
		PivotGridFieldToolTips toolTips;
		bool canShowUnboundExpressionMenu;
		int imageIndex;
		public PivotFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridField field)
			: base(data, groupItems, field) {
			if(field.IsRowTreeField)
				return;
			this.canEdit = field.CanEdit;
			this.fieldEdit = field.FieldEdit;
			this.appearance = field.Appearance;
			this.toolTips = field.ToolTips;
			this.canShowUnboundExpressionMenu = field.CanShowUnboundExpressionMenu;
			this.imageIndex = field.ImageIndex;
		}
		public new PivotGridFieldOptionsEx Options {
			get { return base.Options as PivotGridFieldOptionsEx; }
		}
		public bool CanEdit {
			get { return canEdit; }
		}
		public RepositoryItem FieldEdit {
			get { return fieldEdit; }
		}
		public PivotGridFieldAppearances Appearance {
			get { return appearance; }
		}
		public PivotGridFieldToolTips ToolTips {
			get { return toolTips; }
		}
		public bool CanShowUnboundExpressionMenu {
			get { return canShowUnboundExpressionMenu; }
		}
		public int ImageIndex {
			get { return imageIndex; }
		}
	}
}
