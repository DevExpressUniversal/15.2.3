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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
namespace DevExpress.DashboardWin.Native {
	public class InteractivityOptions {
		public bool HighlightEnable { get; set; }
		public DashboardSelectionMode SelectionMode { get; set; }
		public List<AxisPointTuple> DefaultSelection { get; set; }
	}
	public interface IInteractivityControllerClient {
		void PerformMasterFilterOperation(List<AxisPointTuple> values);
		void PerformDrillDownOperation(AxisPointTuple value, bool trySetMasterFilter);
		void PerformDrillUpOperation(bool clearMasterFilter);
		void PerformClearMasterFilterOperation(bool isRangeFilter);
		void SetSelection(List<AxisPointTuple> values);
		void SetHighlight(List<AxisPointTuple> values);
		void RaiseSelectionChanged(List<AxisPointTuple> values);
		InteractivityOptions RequestInteractivityOptions(InteractivityOptions defaultOptions);
	}
	[
	DXToolboxItem(false),
	DashboardItemDesigner(typeof(DataDashboardItemDesigner))
	]
	public abstract class DataDashboardItemViewer : DashboardItemViewer, IInteractivityControllerClient, ISupportVisualInteractivity {
		const DashboardSelectionMode DefaultCustomSelectionMode = DashboardSelectionMode.None;
		const bool DefaultCustomAllowHighlight = false;
		static IList ExtractValues(ClientHierarchicalMetadata hierarchicalMetadata, bool isColumn, IList<AxisPoint> points) {
			string axisName = isColumn ? hierarchicalMetadata.ColumnHierarchy : hierarchicalMetadata.RowHierarchy;
			List<object> values = new List<object>();
			foreach(AxisPoint point in points) {
				if(point != null && point.AxisName == axisName) {
					foreach(DimensionDescriptor dimension in point.GetDimensions()) {
						DimensionValue dimensionValue = point.GetDimensionValue(dimension);
						if(dimensionValue != null)
							values.Add(dimensionValue.UniqueValue);
					}
					return values;
				}
			}
			return null;
		}
		static IList ExtractValues(ClientHierarchicalMetadata hierarchicalMetadata, bool isColumn, IList<UnderlyingDataTargetValue> values) {
			string hierarchyName = isColumn ? hierarchicalMetadata.ColumnHierarchy : hierarchicalMetadata.RowHierarchy;
			if(!string.IsNullOrEmpty(hierarchyName) && values != null) {
				DimensionDescriptorInternalCollection dimensionDescriptors = hierarchicalMetadata.DimensionDescriptors[hierarchyName];
				IList extractedValues = new List<object>();
				foreach(DimensionDescriptorInternal dimensionDescriptor in dimensionDescriptors) {
					bool found = false;
					foreach(UnderlyingDataTargetValue dimValue in values) {
						if(dimValue.DimensionID == dimensionDescriptor.ID) {
							extractedValues.Add(dimValue.Value);
							found = true;
							break;
						}
					}
					if(!found) {
						break;
					}
				}
				return extractedValues.Count > 0 ? extractedValues : null;
			}
			return null;
		}
		List<string> targetAxes;
		InteractivityController interactivityController;
		public event DashboardItemSelectionChangedEventHandler SelectionChanged;
		public event DashboardItemVisualInteractivityEventHandler VisualInteractivity;
		public bool CanSetMultipleMasterFilter { get { return interactivityController.CanSetMultipleMasterFilter; } }
		public bool CanSetSingleMasterFilter { get { return interactivityController.CanSetSingleMasterFilter; } }
		public bool CanSetMasterFilter { get { return interactivityController.CanSetMasterFilter; } }
		public bool CanClearMasterFilter { get { return interactivityController.CanClearMasterFilter; } }
		public bool CanDrillDown { get { return interactivityController.CanDrillDown; } }
		public bool CanDrillUp { get { return interactivityController.CanDrillUp; } }
		public virtual IEnumerable<string> TargetAxisCore { get { return new [] { DashboardDataAxisNames.DefaultAxis }; } }
		protected virtual string ElementName { get { return String.Empty; } }
		protected virtual bool VisualInterractivitySupported { get { return true; } }
		protected bool IsElementSelectionEnabled {
			get {
				ContentDescriptionViewModel contentDescription = ViewModel.ContentDescription;
				return contentDescription != null && contentDescription.ElementSelectionEnabled;
			}
		}
		protected List<string> TargetAxes { get { return targetAxes; } set { targetAxes = value; } }
		internal InteractivityController InteractivityController { get { return interactivityController; } }
		protected DataDashboardItemViewer() {
		}
		public void SetMasterFilter(IEnumerable<IEnumerable<object>> rows) {
			interactivityController.SetMasterFilter(GetTuples(rows));
		}
		public void ClearMasterFilter() {
			interactivityController.ClearMasterFilter();
		}
		public void PerformDrillDown(IEnumerable<object> row) {
			interactivityController.DrillDown(GetTuple(row));
		}
		public void PerformDrillUp() {
			interactivityController.DrillUp();
		}
		public void ClearSelection() {
			interactivityController.ClearSelection();
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(IList<UnderlyingDataTargetValue> targetValues, IList<string> columnNames) {
			IList columnValues = ExtractValues(HierarchicalMetadata, true, targetValues);
			IList rowValues = ExtractValues(HierarchicalMetadata, false, targetValues);
			return ServiceClient.GetUnderlyingData(DashboardItemName, columnValues, rowValues, columnNames);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(IList<AxisPoint> targetValues, IList<string> columnNames) {
			if(targetValues.Select(p => p.AxisName).Distinct().Count() != targetValues.Count())
				throw new ArgumentException("A list of axis points cannot contain points from the same axis.");
			IList columnValues = ExtractValues(HierarchicalMetadata, true, targetValues);
			IList rowValues = ExtractValues(HierarchicalMetadata, false, targetValues);
			return ServiceClient.GetUnderlyingData(DashboardItemName, columnValues, rowValues, columnNames);
		}
		public AxisPoint GetAxisPoint(string axisName, IList dimensionValues) {
			if(!MultiDimensionalData.IsEmpty) {
				if(dimensionValues == null || (dimensionValues.Count == 1 && dimensionValues[0] == null)) {
					return MultiDimensionalData.GetAxisRoot(axisName);
				}
				else {
					IList<object> valueList = DataUtils.GetFullValue(axisName, dimensionValues, GetDrillDownState());
					return MultiDimensionalData.GetAxisPointByUniqueValues(axisName, DataUtils.CheckOlapNullValues(valueList).ToArray());
				}
			}
			return null;
		}
		public int GetDrillDownLevel() {
			if(DrillDownUniqueValues != null)
				return DrillDownUniqueValues.Count;
			return 0;
		}
		public DashboardDataSet GetDrillDownDataSet() {
			if(DrillDownUniqueValues != null && TargetAxisCore.Count() == 1) {
				int drillDownLevel = DrillDownUniqueValues.Count;				
				List<string> columnNames =
					MultiDimensionalData.GetDimensions(TargetAxisCore.Single()).
					Take(drillDownLevel).
					Select<DimensionDescriptor, string>(descriptor => descriptor.Name).
					ToList();
				DashboardDataSetInternal dataSetInternal = new DashboardDataSetInternal(columnNames, new List<object>(DrillDownUniqueValues));
				return new DashboardDataSet(dataSetInternal);
			}
			return null;
		}
		public DashboardDataSet GetCurrentSelection(IEnumerable<IEnumerable<object>> rows) {
			IEnumerable<IEnumerable<object>> actualSelectedValues = null;
			if(rows != null)
				actualSelectedValues = rows;
			else if(SelectedValues != null)
				actualSelectedValues = SelectedValues.Cast<IEnumerable<object>>();
			if(actualSelectedValues != null && actualSelectedValues.Count() > 0) {
				int drillDownLevel = DrillDownUniqueValues != null ? DrillDownUniqueValues.Count : 0;
				IList<DimensionDescriptor> descriptors = new List<DimensionDescriptor>();
				foreach(string axisName in TargetAxisCore)
					descriptors.AddRange(MultiDimensionalData.GetDimensions(axisName));
				List<string> columnNames = new List<string>();
				int firstIndex = drillDownLevel;
				int lastIndex = drillDownLevel + actualSelectedValues.Max(row => row.Count()) - 1;
				for(int i = firstIndex; i <= lastIndex; i++)
					columnNames.Add(descriptors[i].Name);
				DashboardDataSetInternal dataSetInternal = new DashboardDataSetInternal(columnNames);
				foreach(IList value in actualSelectedValues)
					dataSetInternal.AddRow(value);
				return new DashboardDataSet(dataSetInternal);
			}
			return new DashboardDataSet(new DashboardDataSetInternal());
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				UnSubsribePaintEvents(ViewControl);
			}
		}
		protected override void PrepareViewControl() {			
			SubsribePaintEvents(ViewControl);
		}
		protected void DrawDisabledControl(object sender, PaintEventArgs e) {
			if(ViewModel.ShouldIgnoreUpdate)
				DashboardWinHelper.DrawIgnoreUpdatesState(e.Graphics, LookAndFeel, ClientRectangle);
		}
		protected override void UpdateViewer() {
			DataDashboardItemDesigner designer = DesignerProvider.GetDesigner<DataDashboardItemDesigner>();			
			if(designer != null)
				designer.SetDataObsoleteImage(ViewModel.ShouldIgnoreUpdate);	  
		}
		protected virtual void SetHighlight(List<AxisPointTuple> higtlight) { }
		protected virtual void SetSelection(List<AxisPointTuple> selection) { }
		protected internal override IEnumerable<DashboardItemCaptionButtonInfoCreator> GetDataButtonInfoCreators() {
			List<DashboardItemCaptionButtonInfoCreator> creators = new List<DashboardItemCaptionButtonInfoCreator>();
			if (IsElementSelectionEnabled) {
				ContentDescriptionViewModel contentDescription = ViewModel.ContentDescription;
				if (contentDescription != null)			
					creators.Add(new SelectedItemElementsBarItemsPopupMenuCreator(contentDescription));
			}
			foreach (DashboardItemCaptionButtonInfoCreator creator in interactivityController.GetPopupMenuCreators())
				creators.Insert(0, creator);			 
			return creators;
		}
		protected override void ViewerUpdated(DashboardPaneContent paneContent) {
			List<AxisPointTuple> serverSelection = null;
			ActionModel actionModel = null;
			bool containsActionModel = ContainsContent(paneContent, ContentType.ActionModel);
			bool containsViewModel = ContainsContent(paneContent, ContentType.ViewModel);
			if(containsActionModel) {
				actionModel = paneContent.ActionModel;
			}
			if(containsViewModel || paneContent.ContentType == ContentType.CompleteDataSource) {
				TargetAxes = TargetAxisCore.ToList();
				if(VisualInterractivitySupported) {
					if(paneContent.SelectedValues != null && !MultiDimensionalData.IsEmpty)
						serverSelection = GetTuples(paneContent.SelectedValues);
					else
						serverSelection = new List<AxisPointTuple>();
				}
			}
			interactivityController.Update(serverSelection, actionModel);
		}
		protected virtual InteractivityController CreateInteractivityController() {
			return new InteractivityController(this);
		}
		protected virtual void PerformClearMasterFilterOperationInternal(bool isRangeFilter) {
			ServiceClient.ClearMasterFilter(DashboardItemName, isRangeFilter);
		}
		protected override void InitializeInternal() {
			base.InitializeInternal();
			interactivityController = CreateInteractivityController();
		}
		protected void OnDashboardItemViewerKeyDown(KeyEventArgs e) {
			interactivityController.ProcessKeyDown(e);
		}
		protected void OnDashboardItemViewerKeyUp(KeyEventArgs e) {
			interactivityController.ProcessKeyUp(e);
		}
		protected void OnDashboardItemViewerMouseMove(MouseEventArgs e) {
			if(ItemContainer.IsSelected && interactivityController.ShouldProcessInteractivity && TargetAxes.Count > 0)
				interactivityController.ProcessMouseMove(GetAxisPointTuple(e.Location));
		}
		protected void OnDashboardItemViewerMouseLeave() {
			if(ItemContainer.IsSelected)
				interactivityController.ProcessMouseLeave();
		}
		protected void OnDashboardItemViewerMouseClick(MouseEventArgs e) {
			if(ItemContainer.IsSelected && interactivityController.ShouldProcessInteractivity && TargetAxes.Count > 0)
				interactivityController.ProcessMouseClick(e.Button, GetAxisPointTuple(e.Location));
		}
		protected void OnDashboardItemViewerMouseDoubleClick(MouseEventArgs e) {
			if(ItemContainer.IsSelected && interactivityController.ShouldProcessInteractivity && TargetAxes.Count > 0)
				interactivityController.ProcessMouseDoubleClick(GetAxisPointTuple(e.Location));
		}
		protected void OnDashboardItemViewerLostFocus() {
			interactivityController.ProcessLostFocus();
		}
		protected IDictionary<string, IList> GetDrillDownState() {
			if(DrillDownUniqueValues != null) {
				Dictionary<string, IList> drillDownState = new Dictionary<string, IList>();
				string targetAxis = TargetAxisCore.SingleOrDefault();
				if(!String.IsNullOrEmpty(targetAxis))
					drillDownState.Add(targetAxis, new List<object>(DrillDownUniqueValues));
				return drillDownState;
			}
			return null;
		}
		protected List<object> GetDimestionValueByAxis(List<AxisPointTuple> tupleList, List<string> axes) {
			List<object> dimensionValues = new List<object>();
			foreach(string axis in axes) {
				foreach(AxisPointTuple tuple in tupleList) {
					AxisPoint axisPoint = tuple.GetAxisPoint(axis);
					if(axisPoint != null)
						dimensionValues.Add(GetAxisPointDimensionValues(axisPoint));
				}
			}
			return dimensionValues;
		}
		List<object> GetAxisPointDimensionValues(AxisPoint axisPoint) {
			Dictionary<string, IList> drillDownState = (Dictionary<string, IList>)GetDrillDownState();
			if(drillDownState != null && drillDownState.Keys.Contains(axisPoint.AxisName)) {
				return new List<object>() { axisPoint.DimensionValue.UniqueValue };
			}
			List<object> values = new List<object>();
			foreach(AxisPoint point in axisPoint.RootPath) {
				if(IsMasterFilterDataMember(point)) {
					values.Add(point.UniqueValue);
				}
			}
			return values;
		}
		List<AxisPointTuple> GetTuples(IEnumerable<IEnumerable> values) {
			List<AxisPointTuple> tupleList = new List<AxisPointTuple>();
			foreach(IEnumerable<object> value in values) {
				AxisPointTuple tuple = GetTuple(value);
				if(tuple != null)
					tupleList.Add(tuple);
			}
			return tupleList;
		}
		AxisPointTuple GetTuple(IEnumerable<object> value) { 
			IList<AxisPoint> axisPoints = new List<AxisPoint>();
			int startIndex = 0;
			foreach(string axis in TargetAxisCore) {
				int axisValuesCount = MultiDimensionalData.GetAxis(axis).Dimensions.Count;
				IList<object> axisValues = value.Skip(startIndex).Take(axisValuesCount).ToList();
				startIndex += axisValuesCount;
				AxisPoint axisPoint = GetAxisPoint(axis, (IList)axisValues);
				if(axisPoint != null)
					axisPoints.Add(axisPoint);
			}
			return axisPoints.Count > 0 ? new AxisPointTuple(axisPoints) : null;
		}
		IEnumerable<IEnumerable<object>> ConvertTuples(List<AxisPointTuple> tupleList) { 
			List<object> values = new List<object>();
			foreach(AxisPointTuple tuple in tupleList) {
				IList<object> tupleValues = new List<object>();
				foreach(string axis in TargetAxisCore) {
					AxisPoint axisPoint = tuple.GetAxisPoint(axis);
					if(axisPoint != null)
						tupleValues.AddRange(GetAxisPointDimensionValues(axisPoint));
				}
				values.Add(tupleValues);
			}
			return values.Cast<IEnumerable<object>>();
		}
		protected virtual bool IsMasterFilterDataMember(AxisPoint axisPoint) {
			return true;
		}
		AxisPointTuple GetAxisPointTuple(Point location) {
			DataPointInfo dataPointInfo = GetDataPointInfo(location, true);
			List<AxisPoint> axisPoints = new List<AxisPoint>();
			if(dataPointInfo != null) {
				foreach(KeyValuePair<string, IList> dimensionValue in dataPointInfo.DimensionValues) {
					AxisPoint axisPoint = GetAxisPoint(dimensionValue.Key, dimensionValue.Value);
					if(axisPoint != null && TargetAxes.Contains(axisPoint.AxisName)) {
						while(!IsMasterFilterDataMember(axisPoint)) {
							axisPoint = axisPoint.Parent;
							if(axisPoint == null)
								break;
						}
						if(axisPoint != null)
							axisPoints.Add(axisPoint);
					}
				}
			}
			return new AxisPointTuple(axisPoints);
		}
		void IInteractivityControllerClient.PerformMasterFilterOperation(List<AxisPointTuple> values) {
			ServiceClient.SetMasterFilter(DashboardItemName, ConvertTuples(values));
		}
		void IInteractivityControllerClient.PerformDrillDownOperation(AxisPointTuple value, bool trySetMasterFilter) {
			if(TargetAxisCore.Count() == 1) {
				AxisPoint axisPoint = value.GetAxisPoint(TargetAxisCore.Single());
				if(axisPoint != null)
					ServiceClient.DrillDown(DashboardItemName, new List<object> { axisPoint.UniqueValue }, trySetMasterFilter);
			}
		}
		void IInteractivityControllerClient.PerformDrillUpOperation(bool clearMasterFilter) {
			ServiceClient.DrillUp(DashboardItemName, clearMasterFilter);
		}
		void IInteractivityControllerClient.PerformClearMasterFilterOperation(bool isRangeFilter) {
			PerformClearMasterFilterOperationInternal(isRangeFilter);
		}
		void IInteractivityControllerClient.RaiseSelectionChanged(List<AxisPointTuple> selection) {
			if(SelectionChanged != null) {
				List<AxisPointTuple> selectionByAxis = new List<AxisPointTuple>();
				foreach(AxisPointTuple tuple in selection) {
					List<AxisPoint> axisPoints = new List<AxisPoint>();
					foreach(string axis in TargetAxes) {
						AxisPoint axisPoint = tuple.GetAxisPoint(axis);
						if(axisPoint != null)
							axisPoints.Add(axisPoint);
					}
					if(axisPoints.Count > 0)
						selectionByAxis.Add(new AxisPointTuple(axisPoints));
				}
				SelectionChanged(this, new DashboardItemSelectionChangedEventArgs(DashboardItemName, selectionByAxis));
			}
		}
		void IInteractivityControllerClient.SetSelection(List<AxisPointTuple> values) {
			SetSelection(values);
			if(!CanSetMasterFilter) {
				RefreshCaptionButtons();
			}
		}
		void IInteractivityControllerClient.SetHighlight(List<AxisPointTuple> values) {
			SetHighlight(values);
		}
		InteractivityOptions IInteractivityControllerClient.RequestInteractivityOptions(InteractivityOptions defaultOptions) {
			if(VisualInteractivity != null) {
				DashboardItemVisualInteractivityEventArgs e = new DashboardItemVisualInteractivityEventArgs(this, defaultOptions, TargetAxes);
				VisualInteractivity.Invoke(this, e);
				TargetAxes = e.TargetAxes;
				return e.InteractivityOptions;
			}
			return null;
		}
		void SubsribePaintEvents(Control viewControl) {
			IWinContentProvider contentProvider = this as IWinContentProvider;
			if(contentProvider != null)
				contentProvider.Painted += DrawDisabledControl;
			else
				if(viewControl != null)
					viewControl.Paint += DrawDisabledControl;
		}
		void UnSubsribePaintEvents(Control viewControl) {
			IWinContentProvider contentProvider = this as IWinContentProvider;
			if(contentProvider != null)
				contentProvider.Painted -= DrawDisabledControl;
			else
				if(viewControl != null)
					viewControl.Paint -= DrawDisabledControl;
		}
	}
}
