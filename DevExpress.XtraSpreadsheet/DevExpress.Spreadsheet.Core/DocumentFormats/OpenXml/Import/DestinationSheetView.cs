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
using System.Xml;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SheetViewsDestination
	public class SheetViewsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sheetView", OnSheetView);
			return result;
		}
		public SheetViewsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnSheetView(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetViewDestination(importer);
		}
	}
	#endregion
	#region SheetViewDestination
	public class SheetViewDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("selection", OnSelection);
			result.Add("pivotSelection", OnPivotSelection);
			result.Add("pane", OnPane);
			return result;
		}
		public SheetViewDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			ModelWorksheetView viewOptions = Importer.CurrentSheet.ActiveView;
			viewOptions.BeginUpdate();
			viewOptions.WindowProtection = Importer.GetWpSTOnOffValue(reader, "windowProtection", false);
			viewOptions.ShowFormulas = Importer.GetWpSTOnOffValue(reader, "showFormulas", false);
			viewOptions.ShowGridlines = Importer.GetWpSTOnOffValue(reader, "showGridLines", true);
			viewOptions.ShowRowColumnHeaders = Importer.GetWpSTOnOffValue(reader, "showRowColHeaders", true);
			viewOptions.ShowZeroValues = Importer.GetWpSTOnOffValue(reader, "showZeros", true);
			viewOptions.TabSelected = Importer.GetWpSTOnOffValue(reader, "tabSelected", false);
			viewOptions.ShowRuler = Importer.GetWpSTOnOffValue(reader, "showRuler", true);
			viewOptions.ShowOutlineSymbols = Importer.GetWpSTOnOffValue(reader, "showOutlineSymbols", true);
			viewOptions.ShowWhiteSpace = Importer.GetWpSTOnOffValue(reader, "showWhiteSpace", true);
			viewOptions.ViewType = Importer.GetWpEnumValue<SheetViewType>(reader, "view", OpenXmlExporter.SheetViewTypeTable, SheetViewType.Normal);
			CellPosition topLeftCell = Importer.ReadCellPosition(reader, "topLeftCell");
			if (!topLeftCell.EqualsPosition(CellPosition.InvalidValue))
				viewOptions.TopLeftCell = topLeftCell;
			int zoomScale = Importer.GetWpSTIntegerValue(reader, "zoomScale", Int32.MinValue);
			if (zoomScale != Int32.MinValue)
				viewOptions.ZoomScale = NormalizeZoomValue(zoomScale);
			int zoomScaleNormal = Importer.GetWpSTIntegerValue(reader, "zoomScaleNormal", Int32.MinValue);
			if (zoomScaleNormal != Int32.MinValue)
				viewOptions.ZoomScaleNormal = NormalizeZoomValue(zoomScaleNormal);
			int zoomScaleSheetLayoutView = Importer.GetWpSTIntegerValue(reader, "zoomScaleSheetLayoutView", Int32.MinValue);
			if (zoomScaleSheetLayoutView != Int32.MinValue)
				viewOptions.ZoomScaleSheetLayoutView = NormalizeZoomValue(zoomScaleSheetLayoutView);
			int zoomScalePageLayoutView = Importer.GetWpSTIntegerValue(reader, "zoomScalePageLayoutView", Int32.MinValue);
			if (zoomScalePageLayoutView != Int32.MinValue)
				viewOptions.ZoomScalePageLayoutView = NormalizeZoomValue(zoomScalePageLayoutView);
			int workbookViewId = Importer.GetWpSTIntegerValue(reader, "workbookViewId", Int32.MinValue);
			if (workbookViewId < 0)
				Importer.ThrowInvalidFile("workbookViewId is not specified");
			viewOptions.WorkbookViewId = workbookViewId;
		}
		public override void ProcessElementClose(XmlReader reader) {
			ModelWorksheetView viewOptions = Importer.CurrentSheet.ActiveView;
			viewOptions.EndUpdate();
			base.ProcessElementClose(reader);
		}
		static Destination OnSelection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetViewSelectionDestination(importer);
		}
		static Destination OnPivotSelection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetViewPivotSelectionDestination(importer);
		}
		static Destination OnPane(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetViewPaneDestination(importer);
		}
		static int NormalizeZoomValue(int value) {
			if(value < 10 || value > 400)
				value = 100;
			return value;
		}
	}
	#endregion
	#region SheetViewSelectionDestination
	public class SheetViewSelectionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public SheetViewSelectionDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ViewPaneType pane = Importer.GetWpEnumValue<ViewPaneType>(reader, "pane", OpenXmlExporter.ViewPaneTypeTable, ViewPaneType.TopLeft);
			ModelWorksheetView viewOptions = Importer.CurrentWorksheet.ActiveView;
			if(pane != viewOptions.ActivePaneType)
				return;
			CellRangeBase cellRange = Importer.GetWpSTSqref(reader, "sqref", Importer.CurrentWorksheet);
			CellPosition activeCell = Importer.ReadCellPosition(reader, "activeCell");
			if(cellRange != null) {
				if(activeCell.IsValid)
					Importer.CurrentWorksheet.Selection.SetSelectionCore(activeCell, cellRange, true);
				else
					Importer.CurrentWorksheet.Selection.SetSelection(cellRange);
				int activeRangeIndex = Importer.GetIntegerValue(reader, "activeCellId", 0);
				Importer.CurrentWorksheet.Selection.SetActiveRangeIndex(activeRangeIndex);
			}
			else {
				if(activeCell.IsValid)
					Importer.CurrentWorksheet.Selection.SetSelectionCore(activeCell, new CellRange(Importer.CurrentWorksheet, activeCell, activeCell), true);
			}
		}
	}
	#endregion
	#region SheetViewPaneDestination
	public class SheetViewPaneDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public SheetViewPaneDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ModelWorksheetView viewOptions = Importer.CurrentWorksheet.ActiveView;
			viewOptions.ActivePaneType = Importer.GetWpEnumValue<ViewPaneType>(reader, "activePane", OpenXmlExporter.ViewPaneTypeTable, ViewPaneType.TopLeft);
			viewOptions.SplitState = Importer.GetWpEnumValue<ViewSplitState>(reader, "state", OpenXmlExporter.ViewSplitStateTable, ViewSplitState.Split);
			CellPosition topLeftCell = Importer.ReadCellPosition(reader, "topLeftCell");
			if (!topLeftCell.EqualsPosition(CellPosition.InvalidValue))
				viewOptions.SplitTopLeftCell = topLeftCell;
			int value = Importer.GetWpSTIntegerValue(reader, "xSplit", 0);
			if (value >= 0) {
				if (viewOptions.SplitState == ViewSplitState.Split)
					viewOptions.HorizontalSplitPosition = Importer.DocumentModel.UnitConverter.TwipsToModelUnits(value);
				else
					viewOptions.HorizontalSplitPosition = value;
			}
			value = Importer.GetWpSTIntegerValue(reader, "ySplit", 0);
			if (value >= 0) {
				if (viewOptions.SplitState == ViewSplitState.Split)
					viewOptions.VerticalSplitPosition = Importer.DocumentModel.UnitConverter.TwipsToModelUnits(value);
				else
					viewOptions.VerticalSplitPosition = value;
			}
		}
	}
	#endregion
	#region SheetViewPivotSelectionDestination
	public class SheetViewPivotSelectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		PivotSelection pivotSelection;
		public static Dictionary<ViewPaneType, string> pivotSelectionPivotTable = CreatePivotSelectionPivotTable();
		public static Dictionary<string, ViewPaneType> reversePivotSelectionPivotTable = DictionaryUtils.CreateBackTranslationTable(pivotSelectionPivotTable);
		static Dictionary<ViewPaneType, string> CreatePivotSelectionPivotTable() {
			Dictionary<ViewPaneType, string> result = new Dictionary<ViewPaneType, string>();
			result.Add(ViewPaneType.TopLeft, "topLeft");
			result.Add(ViewPaneType.TopRight, "topRight");
			result.Add(ViewPaneType.BottomLeft, "bottomLeft");
			result.Add(ViewPaneType.BottomRight, "bottomRight");
			return result;
		}
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotArea", OnPivotArea);
			return result;
		}
		public SheetViewPivotSelectionDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			pivotSelection = Importer.CurrentWorksheet.PivotSelection;
			PivotSelection.SetPaneCore((int)Importer.GetWpEnumValue<ViewPaneType>(
				reader, "pane", reversePivotSelectionPivotTable, ViewPaneType.TopLeft));
			PivotSelection.IsShowHeader = Importer.GetWpSTOnOffValue(reader, "showHeader", false);
			PivotSelection.IsLabel = Importer.GetWpSTOnOffValue(reader, "label", false);
			PivotSelection.IsDataSelection = Importer.GetWpSTOnOffValue(reader, "data", false);
			PivotSelection.IsExtendable = Importer.GetWpSTOnOffValue(reader, "extendable", false);
			PivotSelection.SetCountSelectionCore(Importer.GetWpSTIntegerValue(reader, "count", 0));
			PivotSelection.SetAxisCore((int)Importer.GetWpEnumValue<PivotTableAxis>(
				reader, "axis", PivotTablePivotFieldDestination.reversePivotTableAxisTable, PivotTableAxis.None));
			PivotSelection.SetDimensionCore(Importer.GetWpSTIntegerValue(reader, "dimension", 0));
			PivotSelection.SetStartCore(Importer.GetWpSTIntegerValue(reader, "start", 0));
			PivotSelection.SetMinimumCore(Importer.GetWpSTIntegerValue(reader, "min", 0));
			PivotSelection.SetMaximumCore(Importer.GetWpSTIntegerValue(reader, "max", 0));
			PivotSelection.SetActiveRowCore(Importer.GetWpSTIntegerValue(reader, "activeRow", 0));
			PivotSelection.SetActiveColumnCore(Importer.GetWpSTIntegerValue(reader, "activeCol", 0));
			PivotSelection.SetPreviousRowCore(Importer.GetWpSTIntegerValue(reader, "previousRow", 0));
			PivotSelection.SetPreviousColumnCore(Importer.GetWpSTIntegerValue(reader, "previousCol", 0));
			PivotSelection.SetCountClickCore(Importer.GetWpSTIntegerValue(reader, "click", 0));
			string relationId = reader.GetAttribute("id", Importer.RelationsNamespace);
			if (!String.IsNullOrEmpty(relationId)) {
				string relation = Importer.LookupRelationTargetById(Importer.DocumentRelations, relationId, Importer.DocumentRootFolder, string.Empty);
				Importer.PivotSelectionRelations.Add(relation, PivotSelection);
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			ModelWorksheetView viewOptions = Importer.CurrentWorksheet.ActiveView;
			if (PivotSelection.Pane == viewOptions.ActivePaneType) {
				Importer.CurrentWorksheet.PivotSelection.FromTo(PivotSelection);
			}
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotSelection PivotSelection { get { return pivotSelection; } }
		#endregion
		static SheetViewPivotSelectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SheetViewPivotSelectionDestination)importer.PeekDestination();
		}
		static Destination OnPivotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SheetViewPivotSelectionDestination self = GetThis(importer);
			return new PivotTablePivotAreaDestination(importer, self.PivotSelection.PivotArea, importer.CurrentWorksheet);
		}
	}
	#endregion
}
