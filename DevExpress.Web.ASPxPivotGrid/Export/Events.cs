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

using System.Drawing;
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.ASPxPivotGrid {
	public class WebCustomExportHeaderEventArgs : CustomPrintEventArgs {
		PivotFieldItemBase fieldItem;
		PivotGridFieldBase field;
		WebPrintAppearanceObject appearance;
		public WebCustomExportHeaderEventArgs(IVisualBrick brick, PivotFieldItemBase fieldItem, WebPrintAppearanceObject appearance, PivotGridField field, ref Rectangle rect)
			: base(brick, ref rect) {
			this.fieldItem = fieldItem;
			this.field = field;
			this.appearance = appearance;
		}
		protected PivotFieldItemBase FieldItem { get { return fieldItem; } }
		public WebPrintAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				appearance = value;
			}
		}
		public string Caption { get { return FieldItem.HeaderDisplayText; } }
		public PivotGridField Field { get { return (PivotGridField)field; } }
	}
	public class WebCustomExportFieldValueEventArgs : CustomExportFieldValueEventArgsBase<PivotGridField> {
		WebPrintAppearanceObject appearance;
		public WebCustomExportFieldValueEventArgs(IVisualBrick brick, PivotFieldValueItem fieldValueItem, WebPrintAppearanceObject appearance, ref Rectangle rect)
			: base(brick, fieldValueItem, ref rect) {
			this.appearance = appearance;
		}
		public WebPrintAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				appearance = value;
			}
		}
		public new PivotGridCustomTotal CustomTotal { get { return (PivotGridCustomTotal)Item.CustomTotal; } }
	}
	public class WebCustomExportCellEventArgs : CustomExportCellEventArgsBase {
		PivotGridField columnField, rowField, dataField;
		WebPrintAppearanceObject appearance;
		public WebCustomExportCellEventArgs(IVisualBrick brick, PivotGridCellItem cellItem, WebPrintAppearanceObject appearance,
			PivotGridData data, PivotGridField columnField, PivotGridField rowField, PivotGridField dataField, GraphicsUnit graphicsUnit, PivotGridPrinterBase printer, ref Rectangle rect)
			: base(brick, cellItem, graphicsUnit, appearance, printer, ref rect) {
			this.columnField = columnField;
			this.rowField = rowField;
			this.dataField = dataField;
			this.appearance = appearance;
		}
		public WebPrintAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				appearance = value;
			}
		}
		public PivotGridField ColumnField { get { return columnField; } }
		public PivotGridField RowField { get { return rowField; } }
		public PivotGridField DataField { get { return dataField; } }
	}
}
