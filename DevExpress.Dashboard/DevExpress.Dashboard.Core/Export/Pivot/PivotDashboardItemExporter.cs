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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraPrinting;
using System.Linq;
namespace DevExpress.DashboardExport {
	public class PivotDashboardItemExporter : DashboardItemExporter {
		readonly ExportPivotGridControl exportControl;
		readonly PivotDashboardItemViewControl viewControl;
		readonly DashboardPivotGridPrinter printer;
		internal static PivotGridThinClientData ScrollTo(PivotGridThinClientData data, ScrollingState leftScrollingState, ScrollingState topScrollingState, bool addChildrenToFoundColumn, bool addChildrenToFoundRow) {
			IList<ThinClientFieldValueItem> columns = ScrollTo(data.ColumnFieldValues, leftScrollingState, addChildrenToFoundColumn);
			IList<ThinClientFieldValueItem> rows = ScrollTo(data.RowFieldValues, topScrollingState, addChildrenToFoundRow);
			PivotGridThinClientData res = new PivotGridThinClientData(columns, rows);
			columns = GetPlainList(columns);
			rows = GetPlainList(rows);
			foreach(ThinClientFieldValueItem column in columns) {
				foreach(ThinClientFieldValueItem row in rows) {
					int dataIndex = 0;
					ThinClientValueItem cell;
					while(data.TryGetCell(column, row, dataIndex, out cell)) {
						res.AddCell(column, row, dataIndex, cell);
						dataIndex++;
					}
				}
			}
			return res;
		}
		internal static IList<ThinClientFieldValueItem> GetPlainList(IList<ThinClientFieldValueItem> valueItems) {
			IList<ThinClientFieldValueItem> res = new List<ThinClientFieldValueItem> { null };
			if(valueItems != null)
				GetPlainList(res, valueItems);
			return res;
		}
		static void GetPlainList(IList<ThinClientFieldValueItem> res, IList<ThinClientFieldValueItem> valueItems) {
			for(int i = 0; i < valueItems.Count; i++) {
				res.Add(valueItems[i]);
				if(valueItems[i].Children != null)
					GetPlainList(res, valueItems[i].Children);
			}
		}
		static IList<ThinClientFieldValueItem> ScrollTo(IList<ThinClientFieldValueItem> valueItems, ScrollingState scrollingState, bool addChildrenToFoundItem) {
			if(scrollingState == null || scrollingState.PositionListSourceRow == null)
				return valueItems;
			if(scrollingState.PositionListSourceRow.Length == 0)
				return null;
			int positionCount = 0;
			IList<ThinClientFieldValueItem> res = new List<ThinClientFieldValueItem>();
			CutRightPartTree(valueItems, scrollingState.PositionListSourceRow, positionCount, res, addChildrenToFoundItem);
			return res.Count > 0 ? res : null;
		}
		static void CutRightPartTree(IList<ThinClientFieldValueItem> values, object[] positionObjects, int positionCounter, IList<ThinClientFieldValueItem> res, bool addChildrenToFoundItem) {
			for(int i = 0; i < values.Count; i++) {
				if(CutRightPartTree(values[i], positionObjects, positionCounter, res, addChildrenToFoundItem)) {
					for(int j = i + 1; j < values.Count; j++)
						res.Add(values[j]);
					break;
				}
			}
		}
		static bool CutRightPartTree(ThinClientFieldValueItem current, object[] positionObjects, int positionCounter, IList<ThinClientFieldValueItem> res, bool addChildrenToFoundItem) {
			if(!EqualsFieldValues(current.Value.Value, positionObjects[positionCounter]))
				return false;
			positionCounter++;
			IList<ThinClientFieldValueItem> values = current.Children;
			current.Children = new List<ThinClientFieldValueItem>();
			res.Add(current);
			if(positionObjects.Length > positionCounter) {
				if(values == null)
					throw new NotSupportedException();
				CutRightPartTree(values, positionObjects, positionCounter, res[0].Children, addChildrenToFoundItem);
			}
			if(addChildrenToFoundItem && positionObjects.Length == positionCounter)
				current.Children = values;
			if(!addChildrenToFoundItem && current.Children.Count == 0)
				current.Children = null;
			return true;
		}
		internal static int GetPathIndex(ScrollingState scrollingState, IList<ThinClientFieldValueItem> fieldValues, bool isFarTotalsLocation, DashboardExportMode mode) {
			int pathIndex = 0;
			if(mode == DashboardExportMode.EntireDashboard && scrollingState != null) {
				object[] path = scrollingState.PositionListSourceRow;
				bool isGrandTotal = path == null || path.Length == 0;
				int columnPositionCounter = 0;
				int index = -1;
				if(isGrandTotal) {
					IterateChildren(fieldValues, ref index);
					pathIndex = index + 1;
				}
				else if(isFarTotalsLocation) {
					pathIndex = FindColumnIndexInListWithFarTotalsLocation(GetPlanListWithFarTotalsLocation(fieldValues), path, columnPositionCounter);
				}
				else {
					pathIndex = FindColumnIndex(fieldValues, path, columnPositionCounter, ref index);
				}
			}
			return pathIndex;
		}
		static int FindColumnIndex(IList<ThinClientFieldValueItem> fieldValues, object[] path, int positionCounter, ref int index) {
			for(int i = 0; i < fieldValues.Count; i++) {
				if(FindColumnIndexCore(fieldValues[i], path, positionCounter, ref index))
					return index;
			}
			return 0;
		}
		static bool FindColumnIndexCore(ThinClientFieldValueItem currentFieldValue, object[] path, int positionCounter, ref int index) {
			index++;
			IList<ThinClientFieldValueItem> children = currentFieldValue.Children;
			if(!EqualsFieldValues(currentFieldValue.Value.Value, path[positionCounter])) {
				IterateChildren(children, ref index);
				return false;
			}
			positionCounter++;
			if(positionCounter < path.Length) {
				if(children == null)
					throw new NotSupportedException();
				FindColumnIndex(children, path, positionCounter, ref index);
			}
			return true;
		}
		static void IterateChildren(IList<ThinClientFieldValueItem> fieldValues, ref int index) {
			if(fieldValues == null)
				return;
			for(int i = 0; i < fieldValues.Count; i++)
				IterateChildrenCore(fieldValues[i], ref index);
		}
		static void IterateChildrenCore(ThinClientFieldValueItem currentFieldValue, ref int index) {
			IList<ThinClientFieldValueItem> children = currentFieldValue.Children;
			index++;
			IterateChildren(children, ref index);
		}
		static IList<ThinClientFieldValueItem> GetPlanListWithFarTotalsLocation(IList<ThinClientFieldValueItem> fieldValues) {
			IList<ThinClientFieldValueItem> res = new List<ThinClientFieldValueItem>();
			for(int i = 0; i < fieldValues.Count; i++)
				GetPlanListWithFarTotalsLocationCore(fieldValues[i], res);
			return res;
		}
		static void GetPlanListWithFarTotalsLocationCore(ThinClientFieldValueItem currentFieldValue, IList<ThinClientFieldValueItem> res) {
			IList<ThinClientFieldValueItem> children = currentFieldValue.Children;
			if(children != null)
				GetPlanListWithFarTotalsLocationCore(children, res);
			res.Add(currentFieldValue);
		}
		static IList<ThinClientFieldValueItem> GetPlanListWithFarTotalsLocationCore(IList<ThinClientFieldValueItem> fieldValues, IList<ThinClientFieldValueItem> res) {
			for(int i = 0; i < fieldValues.Count; i++)
				GetPlanListWithFarTotalsLocationCore(fieldValues[i], res);
			return res;
		}
		static int FindColumnIndexInListWithFarTotalsLocation(IList<ThinClientFieldValueItem> fieldValues, object[] path, int positionCounter) {
			object rootElement = path.First();
			for(int i = 0; i < fieldValues.Count; i++) {
				ThinClientFieldValueItem currentFieldValue = fieldValues[i];
				if(EqualsFieldValues(currentFieldValue.Value.Value, rootElement)) {
					ThinClientFieldValueItem targetFieldValue = FindColumnIndexInListWithFarTotalsLocationCore(currentFieldValue, path, positionCounter);
					if(targetFieldValue != null)
						return fieldValues.IndexOf(targetFieldValue);
				}
			}
			return 0;
		}
		static ThinClientFieldValueItem FindColumnIndexInListWithFarTotalsLocationCore(ThinClientFieldValueItem currentFieldValue, object[] path, int positionCounter) {
			ThinClientFieldValueItem targetFieldValue = currentFieldValue;
			IList<ThinClientFieldValueItem> children = currentFieldValue.Children;
			if(!EqualsFieldValues(currentFieldValue.Value.Value, path[positionCounter])) {
				return null;
			}
			positionCounter++;
			if(positionCounter < path.Length) {
				if(children == null)
					throw new NotSupportedException();
				targetFieldValue = FindColumnIndexInListWithFarTotalsLocationCore(children, path, positionCounter);
			}
			return targetFieldValue;
		}
		static ThinClientFieldValueItem FindColumnIndexInListWithFarTotalsLocationCore(IList<ThinClientFieldValueItem> fieldValues, object[] path, int positionCounter) {
			for(int i = 0; i < fieldValues.Count; i++) {
				ThinClientFieldValueItem targetFieldValue = FindColumnIndexInListWithFarTotalsLocationCore(fieldValues[i], path, positionCounter);
				if(targetFieldValue != null)
					return targetFieldValue;
			}
			return null;
		}
		public override IPrintable PrintableComponent {
			get { return printer; }
		}
		public PivotDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data, DashboardReportOptions opts)
			: base(mode, data) {
			HeadersOptions options = null;
			if(HasItemContentOptions(opts)) {
				options = opts.ItemContentOptions.HeadersOptions;
			}
			this.exportControl = new ExportPivotGridControl();
			this.viewControl = new PivotDashboardItemViewControl(exportControl);
			PivotDashboardItemViewModel viewModel = (PivotDashboardItemViewModel)ServerData.ViewModel;
			viewControl.Update(viewModel, ServerData.ConditionalFormattingModel, CreateMultiDimensionalData());
			IPivotGridControl pivotControl = (IPivotGridControl)exportControl;
			ExportPivotColumnTotalsLocation columnTotalsLocation;
			Enum.TryParse<ExportPivotColumnTotalsLocation>(ClientState.SpecificState["PivotColumnTotalsLocation"].ToString(), out columnTotalsLocation);
			ExportPivotRowTotalsLocation rowTotalsLocation;
			Enum.TryParse<ExportPivotRowTotalsLocation>(ClientState.SpecificState["PivotRowTotalsLocation"].ToString(), out rowTotalsLocation);
			exportControl.DataBind();
			PrintAppearance appearance = new PrintAppearance();
			if(FontHelper.HasValue(data.FontInfo))
				ExportHelper.ApplyPivotPrintAppearance(appearance, data.FontInfo);
			int pathRowIndex = GetPathIndex(ClientState.VScrollingState, exportControl.Data.RowFieldValues, rowTotalsLocation == ExportPivotRowTotalsLocation.Far, mode);
			int pathColumnIndex = GetPathIndex(ClientState.HScrollingState, exportControl.Data.ColumnFieldValues, columnTotalsLocation == ExportPivotColumnTotalsLocation.Far, mode);
			bool dataScrolled = mode == DashboardExportMode.EntireDashboard && (pathRowIndex > 0 || pathColumnIndex > 0);
			this.printer = new DashboardPivotGridPrinter(exportControl.PivotData, appearance, columnTotalsLocation, rowTotalsLocation, dataScrolled ? null : options, mode, ClientState, pathRowIndex, pathColumnIndex, pivotControl.ShowColumnGrandTotals, pivotControl.ShowRowGrandTotals, pivotControl.ShowColumnTotals, pivotControl.ShowRowTotals);
			ShowHScroll = printer.ShowHScroll;
			ShowVScroll = printer.ShowVScroll;
			if(dataScrolled) {
				bool addChildrenToFoundColumn = columnTotalsLocation == ExportPivotColumnTotalsLocation.Near;
				bool addChildrenToFoundRow = rowTotalsLocation != ExportPivotRowTotalsLocation.Far;
				exportControl.Data = ScrollTo(exportControl.Data, ClientState.HScrollingState, ClientState.VScrollingState, addChildrenToFoundColumn, addChildrenToFoundRow);
				exportControl.DataBind();
			}
			printer.CustomDrawCell += OnPrinterCustomDrawCell;
		}
		void OnPrinterCustomDrawCell(object sender, PivotCustomDrawCellEventArgsBase e) {
			exportControl.OnRequestCustomDrawCell(sender, e);
		}
		protected internal override void CalculateShowScrollbars() {
		}
		protected internal override Rectangle GetViewerBounds() {
			ClientArea viewerArea = ClientState.ViewerArea;
			return new Rectangle(viewerArea.Left, viewerArea.Top, viewerArea.Width, viewerArea.Height);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				printer.CustomDrawCell -= OnPrinterCustomDrawCell;
				printer.Dispose();
				exportControl.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
