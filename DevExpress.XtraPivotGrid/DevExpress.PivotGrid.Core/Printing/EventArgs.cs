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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
namespace DevExpress.PivotGrid.Printing {
	public class CustomPrintEventArgs : EventArgs {
		IVisualBrick brick;
		readonly Rectangle rect;
		bool applyAppearanceToBrickStyle = true;
		bool canApplyAppearanceToBrickStyle = true;
		public bool ApplyAppearanceToBrickStyle {
			get { return applyAppearanceToBrickStyle && canApplyAppearanceToBrickStyle; }
			set { applyAppearanceToBrickStyle = value; }
		}
		public IVisualBrick Brick {
			get { return brick; }
			set { 
				brick = value; 
				canApplyAppearanceToBrickStyle = false;
			}
		}
		public Rectangle Rect { get { return rect; } }
		public CustomPrintEventArgs(IVisualBrick brick, ref Rectangle rect) {
			this.brick = brick;
			this.rect = rect;
		}
	}
	public class CustomExportFieldValueEventArgsBase<T> : CustomPrintEventArgs where T : PivotGridFieldBase {
		PivotFieldValueItem item;
		public CustomExportFieldValueEventArgsBase(IVisualBrick brick, PivotFieldValueItem item, ref Rectangle rect)
			: base(brick, ref rect) {
			this.item = item;
		}
		public T DataField { get { return (T)Item.Data.GetField(Item.DataField); } }
		public PivotGridCustomTotalBase CustomTotal { get { return Item.CustomTotal; } }
		public T Field { get { return (T)Item.Data.GetField(Item.Field); } }
		protected PivotFieldValueItem Item { get { return item; } }
		public string Text { get { return Item.Text; } }
		public object Value { get { return Item.Value; } }
		public int MinIndex { get { return Item.MinLastLevelIndex; } }
		public int MaxIndex { get { return Item.MaxLastLevelIndex; } }
		public int StartLevel { get { return Item.StartLevel; } }
		public int EndLevel { get { return Item.EndLevel; } }
		public bool ContainsLevel(int level) { return Item.ContainsLevel(level); }
		public PivotGridValueType ValueType { get { return Item.ValueType; } }
		public bool IsTopMost { get { return Item.IsColumn ? Item.Level == 0 : Item.MinLastLevelIndex == 0; } }
		public bool IsCollapsed { get { return Item.IsCollapsed; } }
		public bool IsColumn { get { return Item.IsColumn; } }
		public bool IsOthersValue { get { return Item.IsOthersRow; } }
	}
	public class CustomExportCellEventArgsBase : CustomPrintEventArgs {
		PivotGridCellItem cellItem;
		GraphicsUnit graphicsUnit;
		IPivotPrintAppearance appearance;
		PivotGridPrinterBase printer;
		public CustomExportCellEventArgsBase(IVisualBrick brick, PivotGridCellItem cellItem, GraphicsUnit graphicsUnit, IPivotPrintAppearance appearance, PivotGridPrinterBase printer, ref Rectangle rect) : base(brick, ref rect) {
			this.cellItem = cellItem;
			this.graphicsUnit = graphicsUnit;
			this.appearance = appearance;
			this.printer = printer;
		}
		protected PivotGridCellItem CellItem { get { return cellItem; } }
		public string Text { get { return CellItem.Text; } }
		public object Value { get { return CellItem.Value; } }
		public PivotFieldValueItem ColumnValue { get { return CellItem.ColumnFieldValueItem; } }
		public PivotFieldValueItem RowValue { get { return CellItem.RowFieldValueItem; } }
		public int ColumnIndex { get { return CellItem.ColumnIndex; } }
		public int RowIndex { get { return CellItem.RowIndex; } }
		public int ColumnFieldIndex { get { return CellItem.ColumnFieldIndex; } }
		public int RowFieldIndex { get { return CellItem.RowFieldIndex; } }
		public FormatType FormatType {
			get {
				FormatInfo formatInfo = CellItem.GetCellFormatInfo();
				return formatInfo != null ? formatInfo.FormatType : FormatType.None;
			}
		}
		public bool IsTextFit {
			get {
				SizeF sizeF = PrintCellSizeProvider.MeasureString(printer, CellItem.Text, appearance.Font, graphicsUnit);  
				sizeF.Width += printer.CellSizeProvider.LeftCellPadding + printer.CellSizeProvider.RightCellPadding + (printer.ShowVertLines ? 0 : Brick.BorderWidth);
				sizeF.Height += printer.CellSizeProvider.BottomCellPadding + printer.CellSizeProvider.TopCellPadding + (printer.ShowHorzLines ? 0 : Brick.BorderWidth);
				RectangleF rect = new RectangleF(PointF.Empty, sizeF);
				Size size = Size.Ceiling(rect.Size);
				return size.Width <= printer.CellSizeProvider.GetWidthDifference(true, CellItem.ColumnIndex, CellItem.ColumnIndex + 1) &&
					   size.Height <= printer.CellSizeProvider.GetWidthDifference(false, CellItem.RowIndex, CellItem.RowIndex + 1);
			}
		}
	}
	public class CustomExportHeaderEventArgsBase<T> : CustomPrintEventArgs where T : PivotGridFieldBase {
		PivotFieldItemBase fieldItem;
		PivotGridFieldBase field;
		public CustomExportHeaderEventArgsBase(IVisualBrick brick, PivotFieldItemBase fieldItem, PivotGridFieldBase field, ref Rectangle rect)
			: base(brick, ref rect) {
			this.fieldItem = fieldItem;
			this.field = field;
		}
		protected PivotFieldItemBase FieldItem { get { return fieldItem; } }
		public string Caption { get { return FieldItem.HeaderDisplayText; } }
		public T Field { get { return (T)field; } }
	}
}
