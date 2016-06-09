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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardWin.Native {
	public class PivotDashboardItemViewer : DataDashboardItemViewer, IPivotGridControl {
		readonly PivotGridControl pivotGridControl = new PivotGridControl();
		readonly PivotDashboardItemViewControl viewControl;
		readonly DashboardPivotRepositoryItemTextEdit textEdit = new DashboardPivotRepositoryItemTextEdit();
		readonly ToolTipController toolTipController = new ToolTipController();
		readonly HashSet<IntersectionLevelCacheKey> hiddenDisplayTextCache = new HashSet<IntersectionLevelCacheKey>();
#if DEBUGTEST
		public PivotGridControl PivotGridControl { get { return pivotGridControl; } }
		public PivotGridData PivotData { get { return ((IPivotGridViewInfoDataOwner)pivotGridControl).DataViewInfo; } }
#endif
		event EventHandler<PivotFieldDisplayTextEventArgsBase> fieldValueDisplayTextEventHandler;
		event EventHandler<PivotCellDisplayTextEventArgsBase> customCellDisplayTextEventHandler;
		event EventHandler<PivotCustomDrawCellEventArgsBase> customDrawCellEventHandler;
		event EventHandler<PivotFieldValueCollapseStateChangedEventArgs> fieldValueCollapseStateChanged;
		public PivotDashboardItemViewer() {
			viewControl = new PivotDashboardItemViewControl(this);
			pivotGridControl.OptionsMenu.EnableFieldValueMenu = pivotGridControl.OptionsMenu.EnableHeaderAreaMenu = false;
			toolTipController.GetActiveObjectInfo += OnToolTipControllerGetActiveObjectInfo;
			pivotGridControl.ToolTipController = toolTipController;
			SubscibePivotControlEvents();
		}
		void SubscibePivotControlEvents() {
			pivotGridControl.FieldValueDisplayText += OnFieldValueDisplayText;
			pivotGridControl.CustomCellDisplayText += OnCustomCellDisplayText;
			pivotGridControl.MouseClick += OnMouseClick;
			pivotGridControl.MouseDoubleClick += OnMouseDoubleClick;
			pivotGridControl.MouseMove += OnPivotGridMouseMove;
			pivotGridControl.MouseEnter += OnPivotGridMouseEnter;
			pivotGridControl.MouseLeave += OnPivotGridMouseLeave;
			pivotGridControl.MouseDown += OnPivotGridMouseDown;
			pivotGridControl.MouseUp += OnPivotGridMouseUp;
			pivotGridControl.MouseHover += OnPivotGridMouseHover;
			pivotGridControl.MouseWheel += OnPivotGridMouseWheel;
			pivotGridControl.CustomDrawFieldValue += OnPivotGridControlCustomDrawFieldValue;
			pivotGridControl.CustomCellEdit += OnPivotGridControlCustomCellEdit;
			textEdit.CustomDrawCell += OnTextEditCustomDrawCell;
			textEdit.DisplayTextHidden += OnTextEditDisplayTextHidden;
		}
		void UnsubscibePivotControlEvents() {
			if(pivotGridControl != null) {
				pivotGridControl.FieldValueDisplayText -= OnFieldValueDisplayText;
				pivotGridControl.CustomCellDisplayText -= OnCustomCellDisplayText;
				pivotGridControl.MouseClick -= OnMouseClick;
				pivotGridControl.MouseDoubleClick -= OnMouseDoubleClick;
				pivotGridControl.MouseMove -= OnPivotGridMouseMove;
				pivotGridControl.MouseEnter -= OnPivotGridMouseEnter;
				pivotGridControl.MouseLeave -= OnPivotGridMouseLeave;
				pivotGridControl.MouseDown -= OnPivotGridMouseDown;
				pivotGridControl.MouseUp -= OnPivotGridMouseUp;
				pivotGridControl.MouseHover -= OnPivotGridMouseHover;
				pivotGridControl.MouseWheel -= OnPivotGridMouseWheel;
				pivotGridControl.CustomDrawFieldValue -= OnPivotGridControlCustomDrawFieldValue;
				pivotGridControl.CustomCellEdit -= OnPivotGridControlCustomCellEdit;
			}
			if(textEdit != null)
				textEdit.CustomDrawCell -= OnTextEditCustomDrawCell;
		}
		DataPointInfo GetDataPoint(Point location) {
			DataPointInfo dataPoint = new DataPointInfo();
			PivotGridHitInfo hitInfo = pivotGridControl.CalcHitInfo(location);
			PivotCellEventArgs cellInfo = hitInfo.CellInfo;
			if(cellInfo != null) {
				int columnIndex = cellInfo.ColumnIndex;
				int rowIndex = cellInfo.RowIndex;
				PivotGridField columnField = cellInfo.ColumnField;
				PivotGridField rowField = cellInfo.RowField;
				if(columnField != null) {
					List<object> columnValues = new List<object>();
					List<string> columnFieldNames = new List<string>();
					PivotGridField[] columnFields = cellInfo.GetColumnFields();
					foreach(PivotGridField field in columnFields) {
						columnValues.Add(cellInfo.GetFieldValue(field));
					}
					dataPoint.DimensionValues.Add(DashboardDataAxisNames.PivotColumnAxis, columnValues);
				}
				if(rowField != null) {
					List<object> rowValues = new List<object>();
					List<string> rowFieldNames = new List<string>();
					PivotGridField[] rowFields = cellInfo.GetRowFields();
					foreach(PivotGridField field in rowFields) {
						rowValues.Add(cellInfo.GetFieldValue(field));
					}
					dataPoint.DimensionValues.Add(DashboardDataAxisNames.PivotRowAxis, rowValues);
				}
				string measureId = cellInfo.DataField != null ? PivotViewModel.Values[cellInfo.DataField.AreaIndex].DataId : string.Empty;
				dataPoint.Measures.Add(measureId);
				return dataPoint;
			}
			PivotFieldValueHitInfo valueInfo = hitInfo.ValueInfo;
			if(valueInfo != null) {
				string hierarchyName = valueInfo.IsColumn ? HierarchicalMetadata.ColumnHierarchy : HierarchicalMetadata.RowHierarchy;
				List<object> fieldValues = new List<object>();
				PivotGridField[] higherLevelFields = valueInfo.GetHigherLevelFields();
				foreach(PivotGridField field in higherLevelFields) {
					fieldValues.Add(valueInfo.GetHigherLevelFieldValue(field));
				}
				fieldValues.Add(valueInfo.Value);
				dataPoint.DimensionValues.Add(hierarchyName, fieldValues);
				return dataPoint;
			}
			return null;
		}
		protected override bool VisualInterractivitySupported { get { return false; } }
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			return GetDataPoint(location);
		}
		void OnPivotGridMouseMove(object sender, MouseEventArgs e) {
			RaiseMouseMove(e.Location);
		}
		void OnPivotGridMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnPivotGridMouseLeave(object sender, EventArgs e) {
			RaiseMouseLeave();
		}
		void OnPivotGridMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		void OnPivotGridMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		void OnPivotGridMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		void OnPivotGridMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnMouseClick(object sender, MouseEventArgs e) {
			RaiseClick(e.Location);			
		}
		void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			RaiseDoubleClick(e.Location);
		}
		void OnFieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
			if(fieldValueDisplayTextEventHandler != null)
				fieldValueDisplayTextEventHandler(this, new FieldValueDisplayTextEventArgsWrapper(e));
		}
		void OnCustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e) {
			if(customCellDisplayTextEventHandler != null)
				customCellDisplayTextEventHandler(this, new PivotCellDisplayTextEventArgsWrapper(e));
		}
		void OnPivotGridControlCustomDrawFieldValue(object sender, PivotCustomDrawFieldValueEventArgs eventArgs) {
			PivotGridField field = eventArgs.Field;
			if(field != null && field.Area == PivotArea.DataArea)
				return;
			string valueFieldName = field != null ? field.FieldName : null;
			bool isDarkSkin = DashboardWinHelper.IsDarkScheme(LookAndFeel);
			PivotDashboardItemCustomDrawCellEventArgs args = new PivotDashboardItemCustomDrawCellEventArgs(eventArgs.Item.CreateDrillDownDataSource(), valueFieldName, eventArgs.Appearance, false, isDarkSkin, StyleSettingsContainerPainter.GetDefaultBackColor(pivotGridControl.LookAndFeel));
			customDrawCellEventHandler(this, args);
			if(args.CustomBackColor)
				eventArgs.Info.AllowColoring = true;
			StyleSettingsInfo styleSettings = args.StyleSettings;
			if(styleSettings.Image != null) {
				GlyphElementInfoArgs indicatorInfoArgs = new GlyphElementInfoArgs(null, 0, styleSettings.Image);
				indicatorInfoArgs.Cache = eventArgs.GraphicsCache;
				eventArgs.Info.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), indicatorInfoArgs, StringAlignment.Far));
				int captionRectHeight = eventArgs.Info.CaptionRect.Height;
				eventArgs.Painter.CalcObjectBounds(eventArgs.Info);
				eventArgs.Info.CaptionRect = new Rectangle(
					eventArgs.Info.CaptionRect.Left,
					eventArgs.Info.CaptionRect.Top,
					eventArgs.Info.CaptionRect.Width,
					captionRectHeight
					);
				Rectangle indicatorBounds = indicatorInfoArgs.Bounds;
				PivotGridValueType valueType = eventArgs.ValueType;
				int yCoord;
				if(valueType == PivotGridValueType.Total && field.IsColumn) {
					Rectangle cellBounds = eventArgs.Info.Bounds;
					yCoord = cellBounds.Top + cellBounds.Height / 2 - indicatorBounds.Height / 2 - 1;
				}
				else
					yCoord = eventArgs.Info.CaptionRect.Top - 1;
				indicatorInfoArgs.Bounds = new Rectangle(indicatorBounds.X, yCoord, indicatorBounds.Width, indicatorBounds.Height);
			}
		}
		void OnPivotGridControlCustomCellEdit(object sender, PivotCustomCellEditEventArgs e) {
			PivotDrillDownDataSource drillDownDataSource = e.CreateDrillDownDataSource();
			string valueFieldName = e.DataField != null ? e.DataField.FieldName : null;
			DashboardPivotRepositoryItemTextEdit editor = e.RepositoryItem as DashboardPivotRepositoryItemTextEdit;
			if(editor == null)
				e.RepositoryItem = textEdit;
			textEdit.Update(drillDownDataSource, valueFieldName);
		}
		void OnToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e) {
			PivotGridHitInfo info = this.pivotGridControl.CalcHitInfo(e.ControlMousePosition);
			if(info.HitTest == PivotGridHitTest.Cell) {
				PivotCellEventArgs cellInfo = info.CellInfo;
				PivotDrillDownDataSource drillDownDataSource = pivotGridControl.CreateDrillDownDataSource(cellInfo.ColumnIndex, cellInfo.RowIndex);
				PropertyDescriptorCollection properties = drillDownDataSource.GetItemProperties(null);
				PropertyDescriptor columnTag = properties["ColumnTag"];
				AxisPoint columnAxisPoint = columnTag.GetValue(drillDownDataSource[0]) as AxisPoint;
				PropertyDescriptor rowTag = properties["RowTag"];
				AxisPoint rowAxisPoint = rowTag.GetValue(drillDownDataSource[0]) as AxisPoint;
				string fieldName = cellInfo.DataField != null ? cellInfo.DataField.FieldName : String.Empty;
				IntersectionLevelCacheKey key = new IntersectionLevelCacheKey(columnAxisPoint, rowAxisPoint, fieldName);
				if(hiddenDisplayTextCache.Contains(key))
					e.Info = new ToolTipControlInfo(cellInfo.Item, cellInfo.DisplayText, true, ToolTipIconType.None);
			}
		}
		void OnTextEditDisplayTextHidden(object sender, DisplayTextHiddenEventArgs e) {
			IntersectionLevelCacheKey key = new IntersectionLevelCacheKey(e.ColumnAxisPoint, e.RowAxisPoint, e.ValueFieldName);
			if(!hiddenDisplayTextCache.Contains(key))
				hiddenDisplayTextCache.Add(key);
		}
		void OnTextEditCustomDrawCell(object sender, PivotCustomDrawCellEventArgsBase e) {
			if(customDrawCellEventHandler != null)
				customDrawCellEventHandler(this, e);
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();
			Update((PivotDashboardItemViewModel)ViewModel, ConditionalFormattingModel, MultiDimensionalData);
		}
		internal void Update(PivotDashboardItemViewModel viewModel, ConditionalFormattingModel cfModel, MultiDimensionalData data) { 
			if(ViewModel.ShouldIgnoreUpdate)
				pivotGridControl.Invalidate();
			else
				viewControl.Update(viewModel, cfModel, data);
		}
		protected override Control GetViewControl() {
			return pivotGridControl;
		}
		protected override Control GetUnderlyingControl() {
			return pivotGridControl;
		}
		protected override void PrepareViewControl() {
		   base.PrepareViewControl(); 
			pivotGridControl.BeginUpdate();
			try {
				pivotGridControl.BorderStyle = BorderStyles.NoBorder;
				pivotGridControl.UseDisabledStatePainter = false;
				pivotGridControl.OptionsView.ShowColumnHeaders = false;
				pivotGridControl.OptionsView.ShowRowHeaders = false;
				pivotGridControl.OptionsView.ShowDataHeaders = false;
				pivotGridControl.OptionsView.ShowFilterHeaders = false;
				pivotGridControl.OptionsView.ShowTotalsForSingleValues = true;
				pivotGridControl.OptionsView.RowTotalsLocation = PivotRowTotalsLocation.Tree;
				pivotGridControl.OptionsView.ShowColumnGrandTotals = true;
				pivotGridControl.OptionsCustomization.AllowPrefilter = false;
				pivotGridControl.OptionsCustomization.AllowDrag = false;
				pivotGridControl.OptionsCustomization.AllowDragInCustomizationForm = false;
				pivotGridControl.OptionsBehavior.CopyToClipboardWithFieldValues = true;
				pivotGridControl.OptionsCustomization.AllowEdit = false;
				pivotGridControl.RepositoryItems.Add(textEdit);
			} finally {
				pivotGridControl.EndUpdate();
			}			
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(pivotGridControl);
			Point point = pivotGridControl.Cells.LeftTopCell;
			if(point.X > 0) {
				object[] columnPath = GetPath(PivotArea.ColumnArea, point.X);
				state.HScrollingState = new ScrollingState { PositionListSourceRow = columnPath };
			}
			if(point.Y > 0) {
				object[] rowPath = GetPath(PivotArea.RowArea, point.Y);
				state.VScrollingState = new ScrollingState { PositionListSourceRow = rowPath };
			}
			state.SpecificState = new Dictionary<string, object>();
			state.SpecificState.Add("PivotColumnTotalsLocation", (ExportPivotColumnTotalsLocation)pivotGridControl.OptionsView.ColumnTotalsLocation);
			state.SpecificState.Add("PivotRowTotalsLocation", (ExportPivotRowTotalsLocation)pivotGridControl.OptionsView.RowTotalsLocation);
		}
		object[] GetPath(PivotArea area, int visibleIndex) {
			List<PivotGridField> pivotGridFields = pivotGridControl.GetFieldsByArea(area);
			List<object> res = new List<object>();
			for(int i = 0; i < pivotGridFields.Count; i++) {
				object value = pivotGridControl.GetFieldValue(pivotGridFields[i], visibleIndex);
				if(value != null)
					res.Add(value);
			}
			return res.ToArray();
		}
		IEnumerable<IPivotGridField> IPivotGridControl.Fields {
			get {
				List<IPivotGridField> fields = new List<IPivotGridField>();
				foreach(PivotGridField field in pivotGridControl.Fields)
					fields.Add(new PivotGridFieldWrapper(field));
				return fields;
			}
		}
		bool IPivotGridControl.ShowColumnGrandTotals {
			get { return pivotGridControl.OptionsView.ShowColumnGrandTotals; }
			set { pivotGridControl.OptionsView.ShowColumnGrandTotals = value; }
		}
		bool IPivotGridControl.ShowRowGrandTotals {
			get { return pivotGridControl.OptionsView.ShowRowGrandTotals; }
			set { pivotGridControl.OptionsView.ShowRowGrandTotals = value; }
		}
		bool IPivotGridControl.ShowColumnTotals {
			get { return pivotGridControl.OptionsView.ShowColumnTotals; }
			set { pivotGridControl.OptionsView.ShowColumnTotals = value; } 
		}
		bool IPivotGridControl.ShowRowTotals {
			get { return pivotGridControl.OptionsView.ShowRowTotals; }
			set {
				if (value) {
					pivotGridControl.OptionsView.ShowRowTotals = true;
					pivotGridControl.OptionsView.RowTotalsLocation = PivotRowTotalsLocation.Tree;
				}
				else {
					pivotGridControl.OptionsView.RowTotalsLocation = PivotRowTotalsLocation.Far;
					pivotGridControl.OptionsView.ShowRowTotals = false;
				}
			}
		}
		event EventHandler<PivotFieldDisplayTextEventArgsBase> IPivotGridControl.FieldValueDisplayText {
			add { fieldValueDisplayTextEventHandler += value; }
			remove { fieldValueDisplayTextEventHandler -= value; }
		}
		event EventHandler<PivotCellDisplayTextEventArgsBase> IPivotGridControl.CustomCellDisplayText {
			add { customCellDisplayTextEventHandler += value; }
			remove { customCellDisplayTextEventHandler -= value; }
		}
		event EventHandler<PivotCustomDrawCellEventArgsBase> IPivotGridControl.CustomDrawCell {
			add { customDrawCellEventHandler += value; }
			remove { customDrawCellEventHandler -= value; }
		}
		event EventHandler<PivotFieldValueCollapseStateChangedEventArgs> IPivotGridControl.FieldValueCollapseStateChanged {
			add { fieldValueCollapseStateChanged += value; }
			remove { fieldValueCollapseStateChanged -= value; }
		}
		void IPivotGridControl.SetData(PivotGridThinClientData data) {
			PivotGridThinClientDataSource oldDataSource = pivotGridControl.DataSource as PivotGridThinClientDataSource;
			if(oldDataSource != null) {
				oldDataSource.ExpandValueRequested -= OnDataSourceExpandValueRequested;
				oldDataSource.CollapseValueRequested -= OnDataSourceCollapseValueRequested;
				((IDisposable)oldDataSource).Dispose();
			}
			PivotGridThinClientDataSource newDataSource = new PivotGridThinClientDataSource(data);
			newDataSource.ExpandValueRequested += OnDataSourceExpandValueRequested;
			newDataSource.CollapseValueRequested += OnDataSourceCollapseValueRequested;
			pivotGridControl.DataSource = newDataSource;
		}
		void IPivotGridControl.BeginUpdate() {
			pivotGridControl.BeginUpdate();
		}
		void IPivotGridControl.EndUpdate() {
			pivotGridControl.EndUpdate();
		}
		void IPivotGridControl.ClearFields() {
			pivotGridControl.Fields.Clear();
		}
		PivotDashboardItemViewModel PivotViewModel { get { return (PivotDashboardItemViewModel)ViewModel; } }
		IPivotGridField IPivotGridControl.AddField(string fieldName, PivotColumnViewModel column, PivotDashboardItemArea area) {
			PivotGridField field = new PivotGridField { FieldName = fieldName, Caption = column.Caption, Area = PivotDashboardItemViewControl.GetPivotArea(area) };
			pivotGridControl.Fields.Add(field);
			return new PivotGridFieldWrapper(field);
		}
		protected override void ViewerUpdated(DashboardPaneContent paneContent) {
			base.ViewerUpdated(paneContent);
			if(paneContent.ContentType == ContentType.PartialDataSource) {
				object[] values = (object[])paneContent.Parameters[0];
				bool isColumn = (bool)paneContent.Parameters[1];
				ExpandValueRequestedEventArgs expandArgs = new ExpandValueRequestedEventArgs(isColumn, values, paneContent.ItemData != null);
				if(fieldValueCollapseStateChanged != null)
					fieldValueCollapseStateChanged(this, new PivotFieldValueCollapseStateChangedEventArgs(expandArgs.IsColumn, false, expandArgs.Values));
				AxisPoint columnPoint = expandArgs.IsColumn ? MultiDimensionalData.GetAxisPointByUniqueValues(DashboardDataAxisNames.PivotColumnAxis, expandArgs.Values) : MultiDimensionalData.GetAxisRoot(DashboardDataAxisNames.PivotColumnAxis);
				AxisPoint rowPoint = !expandArgs.IsColumn ? MultiDimensionalData.GetAxisPointByUniqueValues(DashboardDataAxisNames.PivotRowAxis, expandArgs.Values) : MultiDimensionalData.GetAxisRoot(DashboardDataAxisNames.PivotRowAxis);
				PivotGridThinClientDataBuilder builder = viewControl.RefreshDataBuilder(MultiDimensionalData, columnPoint, rowPoint, PivotViewModel.Values.Select(v => v.DataId).ToArray());
				if(!expandArgs.IsDataRequired)
					return;
				Dictionary<ThinClientFieldValueItemsPairReferenceKey, PivotGridThinClientDataCollection> data = builder.GetData();
				List<ThinClientFieldValueItem> expandHierarchy = builder.GetExpandHierarchy(expandArgs.IsColumn);
				Dictionary<ThinClientFieldValueItem, object[]> valuesByFieldValue = builder.GetValues(expandArgs.IsColumn);
				expandHierarchy.ForEach(expand => expandArgs.AddExpandHierarchyItem(expand));
				foreach(KeyValuePair<ThinClientFieldValueItemsPairReferenceKey, PivotGridThinClientDataCollection> cellSetPair in data) {
					foreach(KeyValuePair<int, ThinClientValueItem> cellItemPair in cellSetPair.Value.Data) {
						ThinClientFieldValueItem expand = expandArgs.IsColumn ? cellSetPair.Key.First : cellSetPair.Key.Second;
						if(expand != null) {
							ThinClientFieldValueItem cross = expandArgs.IsColumn ? cellSetPair.Key.Second : cellSetPair.Key.First;
							var crossValues = cross == null || !valuesByFieldValue.ContainsKey(cross) ? null : valuesByFieldValue[cross];
							expandArgs.AddDataItem(expand, crossValues, cellItemPair.Key, cellItemPair.Value);
						}
					}
				}
				PivotGridThinClientDataSource dataSource = (PivotGridThinClientDataSource)pivotGridControl.DataSource;
				dataSource.ProcessExpandValue(expandArgs);
			}
		}
		void ServiceClient_ExpandValueCompleted(object sender, ServiceExpandValueCompletedEventArgs e) {
			UpdateMultiDimensionalData(e.ItemData);
		}
		void OnDataSourceExpandValueRequested(object sender, ExpandValueRequestedEventArgs e) {
			ServiceClient.ExpandValue(DashboardItemName, e.Values, e.IsColumn, true, e.IsDataRequired);
		}
		void OnDataSourceCollapseValueRequested(object sender, CollapseValueRequestedEventArgs e) {
			ServiceClient.ExpandValue(DashboardItemName, e.Values, e.IsColumn, false, false);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				UnsubscibePivotControlEvents();
				DisposeToopTipController();
				pivotGridControl.Dispose();
				textEdit.Dispose();
			}
			base.Dispose(disposing);
		}
		void DisposeToopTipController() {
			pivotGridControl.ToolTipController = null;
			toolTipController.GetActiveObjectInfo -= OnToolTipControllerGetActiveObjectInfo;
			toolTipController.Dispose();
		}
	}
}
