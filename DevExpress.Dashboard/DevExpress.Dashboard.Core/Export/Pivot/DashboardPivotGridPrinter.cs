#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DashboardExport {
	public class DashboardPivotGridPrinter : DashboardCellItemPrinter, IPivotGridPrinterOwner {
		event EventHandler<PivotCustomDrawCellEventArgsBase> customDrawCellEventHandler;
		public bool CreatesIntersectedBricks {
			get { return false; }
		}
		public event EventHandler<PivotCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCellEventHandler += value; }
			remove { customDrawCellEventHandler -= value; }
		}
		public DashboardPivotGridPrinter(PivotGridData data, PrintAppearance appearance, ExportPivotColumnTotalsLocation columnTotalsLocation, ExportPivotRowTotalsLocation rowTotalsLocation, HeadersOptions options, DashboardExportMode mode, ItemViewerClientState clientState, int pathRowIndex, int pathColumnIndex, bool showColumnGrandTotals, bool showRowGrandTotals, bool showColumnTotals, bool showRowTotals)
			: base(null, data, appearance) {
			Owner = this;
			Data.OptionsPrint.PrintColumnHeaders = DefaultBoolean.False;
			Data.OptionsPrint.PrintDataHeaders = DefaultBoolean.False;
			Data.OptionsPrint.PrintFilterHeaders = DefaultBoolean.False;
			Data.OptionsPrint.PrintRowHeaders = DefaultBoolean.False;
			Data.OptionsView.ShowRowHeaders = false;
			Data.OptionsView.ShowDataHeaders = false;
			Data.OptionsView.ShowColumnHeaders = false;
			Data.OptionsView.ShowColumnGrandTotals = showColumnGrandTotals;
			Data.OptionsView.ShowRowGrandTotals = showRowGrandTotals;
			Data.OptionsView.ShowColumnTotals = showColumnTotals;
			Data.OptionsView.ShowRowTotals = showRowTotals;
			Data.OptionsView.ShowTotalsForSingleValues = true;
			Data.OptionsView.ShowGrandTotalsForSingleValues = true;
			Data.OptionsView.ColumnTotalsLocation = (PivotTotalsLocation)columnTotalsLocation;
			Data.OptionsView.RowTotalsLocation = (PivotRowTotalsLocation)rowTotalsLocation;
			Data.OptionsBehavior.BestFitMode = PivotGridBestFitMode.FieldValue | PivotGridBestFitMode.Cell;
			Data.OptionsPrint.PrintHeadersOnEveryPage = options != null && options.PrintHeadersOnEveryPage;
			VisibleDataAreaHeight = clientState.ViewerArea.Height - RowAreaY;
			VisibleDataAreaWidth = clientState.ViewerArea.Width - ColumnAreaX;
			VisibleRowCount = CalcVisibleRowCount(mode, pathRowIndex);
			if(mode == DashboardExportMode.SingleItem)
				BestFitter.BestFit();
			else {
				CalcScrollVisibilityAndColumnsWidths(mode, clientState, pathRowIndex, pathColumnIndex);
			}
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
			Release();
		}
		bool IPivotGridPrinterOwner.CustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			return false;
		}
		bool IPivotGridPrinterOwner.CustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem fieldValue, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return false;
		}
		bool IPivotGridPrinterOwner.CustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearanc, ref Rectangle recte) {
			return false;
		}
		protected override IVisualBrick DrawCellBrick(IPivotPrintAppearance appearance, Rectangle bounds, PivotGridCellItem cellItem) {
			PivotDrillDownDataSource drillDownDataSource = cellItem.Data.CreateDrillDownDataSource(cellItem.ColumnIndex, cellItem.RowIndex);
			string valueFieldName = cellItem.DataField != null ? cellItem.DataField.FieldName : null;
			ExportPivotCustomDrawCellEventArgs args = new ExportPivotCustomDrawCellEventArgs(drillDownDataSource, valueFieldName, appearance.BackColor, appearance.ForeColor, appearance.Font, true, ExportHelper.GetDefaultBackColor());
			StyleSettingsInfo styleSettings = args.StyleSettings;
			if(customDrawCellEventHandler != null)
				customDrawCellEventHandler(this, args);
			if(styleSettings.Image != null) {
				FormatInfo cellFormat = cellItem.GetCellFormatInfo();
				string formatString = cellFormat != null ? cellFormat.FormatString : String.Empty;
				IVisualBrick panel = ExportImageStyleSettingsPainter.CreatePanelBrick(appearance, bounds, formatString, cellItem.Text, cellItem.Value, styleSettings);
				return panel;
			}
			if(styleSettings.Bar != null) {
				BrickStyle brickStyle = new BrickStyle(BorderSide.All, 1, appearance.BorderColor, styleSettings.BackColor, styleSettings.ForeColor, styleSettings.Font, new BrickStringFormat(appearance.StringFormat));
				FormatConditionBarBrick barBrick = new FormatConditionBarBrick(brickStyle) { BarValue = cellItem.Value, Text = cellItem.Text };
				barBrick.Assign(styleSettings.Bar);
				return barBrick;
			}
			IVisualBrick brick = base.DrawCellBrick(appearance, bounds, cellItem);
			ApplyStyleSettings(styleSettings, brick);
			return brick;
		}
		protected override IVisualBrick DrawFieldValueBrick(PivotFieldValueItem fieldValue, Rectangle bounds, Rectangle textBounds, string formatString) {
			IPivotPrintAppearance appearance = GetValueAppearance(fieldValue.ValueType, fieldValue.Field);
			PivotDrillDownDataSource drillDownDataSource = fieldValue.CreateDrillDownDataSource();
			string valueFieldName = fieldValue.Field != null ? fieldValue.Field.FieldName : null;
			ExportPivotCustomDrawCellEventArgs args = new ExportPivotCustomDrawCellEventArgs(drillDownDataSource, valueFieldName, appearance.BackColor, appearance.ForeColor, appearance.Font, false, ExportHelper.GetDefaultBackColor());
			StyleSettingsInfo styleSettings = args.StyleSettings;
			if(customDrawCellEventHandler != null)
				customDrawCellEventHandler(this, args);
			bool emptyBrick = fieldValue.IsRowTree && fieldValue.CellCount > 0;
			if(!emptyBrick && styleSettings.Image != null) {
				IVisualBrick panel = ExportImageStyleSettingsPainter.CreatePanelBrick(appearance, textBounds.IsEmpty ? bounds : textBounds, String.Empty, fieldValue.Text, fieldValue.Value, styleSettings);
				return panel;
			}
			IVisualBrick brick = base.DrawFieldValueBrick(fieldValue, bounds, textBounds, formatString);
			ApplyStyleSettings(styleSettings, brick);
			return brick;
		}
		void ApplyStyleSettings(StyleSettingsInfo styleSettings, IVisualBrick brick) {
			ITextBrick textBrick = brick as ITextBrick;
			if(textBrick != null) {
				textBrick.ForeColor = styleSettings.ForeColor;
				textBrick.BackColor = styleSettings.BackColor;
				textBrick.Font = styleSettings.Font;
			}
		}
	}
}
