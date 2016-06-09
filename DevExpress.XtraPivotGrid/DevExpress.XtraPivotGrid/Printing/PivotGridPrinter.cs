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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Frames;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPivotGrid.Printing {
	public class PivotGridPrinter : PivotGridPrinterBase {
		PivotGridControl pivotGridControl;
		PivotGridPrinting printControl;
		public PivotGridPrinter(PivotGridControl pivotGridControl) : base(pivotGridControl, pivotGridControl.Data) {
			this.pivotGridControl = pivotGridControl;
			this.printControl = null;
		}
		public PivotGridControl PivotGridControl { get { return pivotGridControl; } }
		protected PivotGridViewInfoData ViewInfoData { get { return (PivotGridViewInfoData)Data; } }
		public override void AcceptChanges() {
			printControl.ApplyOptions(true);
		}
		public override bool HasPropertyEditor() { return true; }
		public override UserControl PropertyEditorControl {
			get {
				if(printControl == null) {
					printControl = CreatePropertyEditorControl();
				}
				return printControl;
			}
		}
		PivotGridPrinting CreatePropertyEditorControl() {
			PivotGridPrinting ctrl = new PivotGridPrinting();
			ctrl.InitFrame(PivotGridControl, PivotGridLocalizer.GetString(PivotGridStringId.PrintDesigner), null);
			ctrl.lbCaption.Visible = false;
			ctrl.Size = ctrl.UserControlSize;
			ctrl.AutoApply = false;
			return ctrl;
		}
		protected virtual PivotGridAppearancesBase PrintAndPaintAppearance {
			get {
				return Data.OptionsPrint.UsePrintAppearance ? (PivotGridAppearancesBase)ViewInfoData.PaintAppearancePrint : ViewInfoData.PaintAppearance;
			}
		}
		protected override PivotPrintBestFitter CreatePivotPrintBestFitter() {
			return new PivotPrintBestFitter(Data, this, new WinPrintCellSizeProvider(Data, Data.VisualItems, this));
		}
		class WinPrintCellSizeProvider : PrintCellSizeProvider {
			public WinPrintCellSizeProvider(PivotGridData data, PivotVisualItemsBase visualItems, PivotGridPrinterBase printer)
				: base(data, visualItems, printer) {
			}
			protected override int GetColumnFieldWidth(PivotFieldValueItem item) {
				return ((PivotGridViewInfoData)Data).GetCustomColumnWidth(item, base.GetColumnFieldWidth(item));
			}
			protected override int GetRowFieldHeight(PivotFieldValueItem item) {
				return ((PivotGridViewInfoData)Data).GetCustomRowHeight(item, base.GetRowFieldHeight(item));
			}
		}
		protected override IPivotPrintAppearance GetFieldAppearance(PivotFieldItemBase field) {
			return CalculateAppearanceCore(GetFieldAndFieldValueAppearance().GetActualFieldAppearance((PivotFieldItem)field));
		}
		protected override IPivotPrintAppearance GetValueAppearance(PivotGridValueType valueType, PivotFieldItemBase field) {
			return CalculateAppearanceCore(GetFieldAndFieldValueAppearance().GetActualFieldValueAppearance(valueType, (PivotFieldItem)field));
		}
		protected override IPivotPrintAppearance GetCellAppearance(PivotGridCellItem cell, Rectangle? bounds) {
			return CalculateAppearanceCore(PrintAndPaintAppearance.GetCellAppearanceWithCustomCellAppearance(cell, bounds));
		}
		protected override IPivotPrintAppearance GetCellAppearance() {
			return CalculateAppearanceCore(PrintAndPaintAppearance.Cell);
		}
		protected override IPivotPrintAppearance GetTotalCellAppearance() {
			return CalculateAppearanceCore(PrintAndPaintAppearance.GetTotalCellAppearance());
		}
		protected override IPivotPrintAppearance GetGrandTotalCellAppearance() {
			return CalculateAppearanceCore(PrintAndPaintAppearance.GetGrandTotalCellAppearance());
		}
		protected ExportAppearanceObject CalculateAppearanceCore(AppearanceObject appearance) {
			ExportAppearanceObject res = new ExportAppearanceObject();
			res.Assign(appearance);
			if(OptionsPrint.UsePrintAppearance || ViewInfoData.Appearance.Lines.Options.UseForeColor)
				res.BorderColor = PrintAndPaintAppearance.Lines.ForeColor;
			else
				res.BorderColor = Color.Empty;	 
			return res;
		}
		protected virtual PivotGridAppearancesBase GetFieldAndFieldValueAppearance() {
			PivotGridAppearancesBase appearances;
			if(!OptionsPrint.UsePrintAppearance && ViewInfoData.ActiveLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				appearances = ViewInfoData.PaintAppearancePrint;
			else
				appearances = PrintAndPaintAppearance;
			return appearances;
		}
		protected override int GetFilterSeparatorHeight() {
			return OptionsPrint.FilterSeparatorBarPadding > 0 ? base.GetFilterSeparatorHeight() : ViewInfoData.OptionsView.FilterSeparatorBarPadding * 2 + 1;
		}
		protected override IVisualBrick DrawCellBrick(IPivotPrintAppearance appearance, Rectangle bounds, PivotGridCellItem pivotGridCellItem) {
			IVisualBrick brick = null;
			RepositoryItem rItem = ViewInfoData.GetCellEdit(pivotGridCellItem);
			if(rItem != null) {
				PrintCellHelperInfo info = new PrintCellHelperInfo(
					Graph.DefaultBrickStyle.BorderColor,
					PrintingSystem,
					pivotGridCellItem.Value,
					(AppearanceObject)appearance,
					pivotGridCellItem.Text,
					bounds,
					Graph,
					appearance.TextHorizontalAlignment,
					ShowHorzLines,
					ShowVertLines,
					pivotGridCellItem.GetCellFormatInfo() == null ? "" : pivotGridCellItem.GetCellFormatInfo().FormatString);
				brick = rItem.GetBrick(info);
			} else {
				brick = base.DrawCellBrick(appearance, bounds, pivotGridCellItem);
			}
			return brick;
		}
	}
}
