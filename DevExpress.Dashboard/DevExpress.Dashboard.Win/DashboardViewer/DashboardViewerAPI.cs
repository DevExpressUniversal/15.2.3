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
using DevExpress.DashboardCommon.Layout;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraPrinting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DashboardWin {
	public partial class DashboardViewer {
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dashboard Dashboard {
			get {
				IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				return ownerService.Dashboard;
			}
			set {
				IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				Dashboard oldDashboard = ownerService.Dashboard;
				Dashboard newDashboard = value;
				if (newDashboard != oldDashboard) {
					dashboardSource = null;
					ownerService.SetDashboard(newDashboard);
				}
			}
		}
		[
		Obsolete("The DevExpress.DashboardWin.DashboardViewer.DashboardUri property is now obsolete. Use the DevExpress.DashboardWin.DashboardViewer.DashboardSource property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false)
		]
		public Uri DashboardUri {
			get { return DashboardSource as Uri; }
			set { DashboardSource = value; }
		}
		[
		Category(CategoryNames.Behavior),
		DefaultValue(null),
		TypeConverter(TypeNames.DashboardSourceConvertor),
		Editor(TypeNames.DashboardSourceEditor, typeof(UITypeEditor))
		]
		public object DashboardSource {
			get { return dashboardSource; }
			set {
				dashboardSource = value;
				if(DesignMode) 
					LayoutControl.Invalidate(); 
				else
					LoadDashboard();
			}
		}
		[
		Category(CategoryNames.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardPrintingOptions PrintingOptions { get { return printingOptions; } }
		[
		Category(CategoryNames.Behavior),
		DefaultValue(DefaultPrintPreviewType)
		]
		public DashboardPrintPreviewType PrintPreviewType { get { return printPreviewType; } set { printPreviewType = value; } }
		[
		Category(CategoryNames.Behavior),
		DefaultValue(DefaultAllowPrintDashboard)
		]
		public bool AllowPrintDashboard {
			get { return allowPrintDashboard; }
			set {
				if (value != allowPrintDashboard) {
					allowPrintDashboard = value;
					UpdateDashboardTitle();
				}
			}
		}
		[
		Category(CategoryNames.Behavior),
		DefaultValue(DefaultAllowPrintDashboardItmes)
		]
		public bool AllowPrintDashboardItems {
			get { return allowPrintDashboardItems; }
			set {
				if (value != allowPrintDashboardItems) {
					allowPrintDashboardItems = value;
					foreach (DashboardItemViewer itemViewer in ItemViewers)
						itemViewer.RefreshCaptionButtons();
				}
			}
		}
		[
		Category(CategoryNames.Behavior),
		DefaultValue(DefaultPopulateUnusedDataSources)
		]
		public bool PopulateUnusedDataSources {
			get { return populateUnusedDataSources; }
			set {
				if(populateUnusedDataSources != value) {
					populateUnusedDataSources = value;
					if(populateUnusedDataSources && HasUnusedDataSources)
						ReloadData(false);
				}
			}
		}
		[
		Category(CategoryNames.General),
		DefaultValue(DefaultUseDataAccessApi),
		Obsolete("This property is now obsolete. You no longer need to set it to true in order to use data access API."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool UseDataAccessApi {
			get { return useDataAccessApi; }
			set {
				if(useDataAccessApi != value) {
					useDataAccessApi = value;
				}
			}
		}
		[
		Category(CategoryNames.General),
		DefaultValue(DefaultCalculateHiddenTotals)
		]
		public bool CalculateHiddenTotals { 
			get { return calculateHiddenTotals; } 
			set { calculateHiddenTotals = value; } 
		}
		[Browsable(false)]
		public UserAction UserAction {
			get {
				if (layoutControl.AdornerWindowHandlerState != AdornerWindowHandlerStates.Normal)
					return UserAction.LayoutAction;
				else
					return UserAction.None;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DashboardParameters Parameters { get { return parameters; } }
		public event DashboardLoadedEventHandler DashboardLoaded;
		public event EventHandler DashboardChanged;
		public event EventHandler LayoutChanged;
		public event DashboardConfigureDataConnectionEventHandler ConfigureDataConnection;
		public event CustomFilterExpressionEventHandler CustomFilterExpression;
		public event CustomParametersEventHandler CustomParameters;
		public event DataLoadingEventHandler DataLoading;
		public event DashboardConnectionErrorEventHandler ConnectionError;
		public event MasterFilterSetEventHandler MasterFilterSet;
		public event MasterFilterClearedEventHandler MasterFilterCleared;
		public event SingleFilterDefaultValueEventHandler SingleFilterDefaultValue;
		public event FilterElementDefaultValuesEventHandler FilterElementDefaultValues;
		public event RangeFilterDefaultValueEventHandler RangeFilterDefaultValue;
		public event ValidateDashboardCustomSqlQueryEventHandler ValidateCustomSqlQuery;
		public event DrillActionEventHandler DrillDownPerformed;
		public event DrillActionEventHandler DrillUpPerformed;
		public event DashboardBeforeExportEventHandler BeforeExport;
		public event DashboardItemMouseActionEventHandler DashboardItemClick;
		public event DashboardItemMouseActionEventHandler DashboardItemDoubleClick;
		public event DashboardItemMouseActionEventHandler DashboardItemMouseUp;
		public event DashboardItemMouseActionEventHandler DashboardItemMouseDown;
		public event DashboardItemMouseEventHandler DashboardItemMouseWheel;
		public event DashboardItemMouseActionEventHandler DashboardItemMouseMove;
		public event DashboardItemMouseEventHandler DashboardItemMouseEnter;
		public event DashboardItemMouseEventHandler DashboardItemMouseLeave;
		public event DashboardItemMouseEventHandler DashboardItemMouseHover;
		public event DashboardItemVisualInteractivityEventHandler DashboardItemVisualInteractivity;
		public event DashboardItemSelectionChangedEventHandler DashboardItemSelectionChanged;
		public event DashboardItemControlUpdatedEventHandler DashboardItemControlUpdated;
		public event DashboardItemControlCreatedEventHandler DashboardItemControlCreated;
		public event DashboardItemBeforeControlDisposedEventHandler DashboardItemBeforeControlDisposed;
		public event DashboardItemElementCustomColorEventHandler DashboardItemElementCustomColor;
		public event CustomizeDashboardTitleTextEventHandler CustomizeDashboardTitleText;
		[
		Obsolete("The DevExpress.DashboardWin.DashboardViewer.GetAvailableSelections method is now obsolete. Use the DevExpress.DashboardWin.DashboardViewer.GetAvailableDrillDownValues and DevExpress.DashboardWin.DashboardViewer.GetAvailableFilterValues methods instead.")
		]
		public DashboardDataSet GetAvailableSelections(string dashboardItemName) {
			return null;
		}
		public IList<AxisPointTuple> GetAvailableDrillDownValues(string dashboardItemName) {
			return CanPerformDrillDown(dashboardItemName) ? GetAvailableTuples(dashboardItemName) : null;
		}
		public AxisPointTuple GetCurrentDrillDownValues(string dashboardItemName) {
			DataDashboardItem dataDashboardItem = FindDataDashboardItemForAPI(dashboardItemName);
			string[] dimensionIds = dataDashboardItem.GetDimensionIds();
			int dimensionCount = dimensionIds.Length;
			if(dimensionCount > 0) {
				DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
				string axisName = dataDashboardItem.GetAxisNames()[0];
				DataAxis axis = dataItemViewer.MultiDimensionalData.GetAxis(axisName);
				DimensionDescriptor dimension = axis.Dimensions.Where(d => d.ID == dimensionIds[0]).First();
				List<AxisPoint> axisPoints = axis.GetPointsByDimension(dimension).ToList();
				if(axisPoints.Count > 0) {
					AxisPoint axisPoint = axisPoints.First().Parent;
					return axisPoint.UniqueValue != null ? new AxisPointTuple(new List<AxisPoint> { axisPoint }) : null;
				} else {
					return null;
				}
			}
			return null;
		}
		public IList<AxisPointTuple> GetAvailableFilterValues(string dashboardItemName) {
			return CanSetMasterFilter(dashboardItemName) ? GetAvailableTuples(dashboardItemName) : null;
		}
		public IList<AxisPointTuple> GetCurrentFilterValues(string dashboardItemName) {
			DataDashboardItem dataDashboardItem = FindDataDashboardItemForAPI(dashboardItemName);
			string[] dimensionIds = dataDashboardItem.GetDimensionIds();
			int dimensionCount = dimensionIds.Length;
			if(dimensionCount > 0) {
				DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
				string axisName = dataDashboardItem.GetAxisNames()[0];
				DataAxis axis = dataItemViewer.MultiDimensionalData.GetAxis(axisName);
				List<AxisPointTuple> tuples = new List<AxisPointTuple>();
				IDashboardItemInteractivityService interactivityService = ServiceProvider.RequestServiceStrictly<IDashboardItemInteractivityService>();
				IValuesSet rows = interactivityService.GetCurrentSelection(dataDashboardItem.ComponentName);
				foreach (ISelectionRow row in rows.AsSelectionRows()) {
					object value = row[0];
					DimensionDescriptor dimension = axis.Dimensions.Where(d => d.ID == dimensionIds[0]).First();
					AxisPoint axisPoint = axis.GetPointsByDimension(dimension).Where(p => object.Equals(value, p.UniqueValue)).First();
					for(int i = 1; i < dimensionCount; i++) {
						value = row[i];
						IList<AxisPoint> points = axisPoint.ChildItems.ToList();
						axisPoint = points.Where(p => object.Equals(value, p.UniqueValue)).First();
					}
					tuples.Add(new AxisPointTuple(new List<AxisPoint> { axisPoint }));
				}
				return tuples;
			}
			return null;
		}
		IList<AxisPointTuple> GetAvailableTuples(string dashboardItemName) {
			DataDashboardItem dataDashboardItem = FindDataDashboardItemForAPI(dashboardItemName);
			string[] dimensionIds = dataDashboardItem.GetDimensionIds();
			int dimensionCount = dimensionIds.Length;
			if(dimensionCount > 0) {
				DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
				string axisName = dataDashboardItem.GetAxisNames()[0];
				DataAxis axis = dataItemViewer.MultiDimensionalData.GetAxis(axisName);
				DimensionDescriptor dimension = axis.Dimensions.Where(d => d.ID == dimensionIds[dimensionCount - 1]).First();
				List<AxisPointTuple> tuples = new List<AxisPointTuple>();
				foreach(AxisPoint axisPoint in axis.GetPointsByDimension(dimension)) {
					tuples.Add(new AxisPointTuple(new List<AxisPoint> { axisPoint }));
				};
				return tuples;
			}
			return null;
		}
		public void SetMasterFilter(string dashboardItemName, AxisPointTuple tuple) {
			SetMasterFilter(dashboardItemName, (IEnumerable<AxisPointTuple>)(new List<AxisPointTuple> { tuple }));
		}
		public void SetMasterFilter(string dashboardItemName, IEnumerable<AxisPointTuple> tuples) {
			DataDashboardItem dataDashboardItem = FindDataDashboardItemForAPI(dashboardItemName);
			string[] dimensionIds = dataDashboardItem.GetDimensionIds();
			int dimensionCount = dimensionIds.Length;
			List<IList> values = new List<IList>();
			foreach(AxisPointTuple tuple in tuples) {
				List<object> value = new List<object>();
				AxisPoint axisPoint = tuple.GetAxisPoint(dataDashboardItem.GetAxisNames()[0]);
				while(axisPoint.Value != null) {
					value.Add(axisPoint.UniqueValue);
					if(dimensionCount == 1) break;
					axisPoint = axisPoint.Parent;
				}
				value.Reverse();
				values.Add(value);
			}
			SetMasterFilter(dashboardItemName, values);
		}
		public void SetMasterFilter(string dashboardItemName, IEnumerable<DashboardDataRow> values) {
			SetMasterFilter(dashboardItemName, values.Select<DashboardDataRow, IList>(row => row.ToList()));
		}
		public void SetMasterFilter(string dashboardItemName, object values) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			IEnumerable pointList = values as IEnumerable;
			if(pointList != null && values.GetType() != typeof(string)) {
				IEnumerable<object> typedPointList = pointList.Cast<object>();
				if(typedPointList.Count() > 0) {
					object pointValue = typedPointList.ElementAt(0);
					IEnumerable pointValueList = pointValue as IEnumerable;
					if (pointValueList != null && pointValue.GetType() != typeof(string)) {
						if ((typedPointList.Count() == 1 && dataItemViewer.CanSetMasterFilter) || (typedPointList.Count() > 1 && dataItemViewer.CanSetMultipleMasterFilter)) {
							dataItemViewer.SetMasterFilter(typedPointList.Cast<IEnumerable<object>>());
							return;
						}
						throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityOperationNotAvailable));
					}
				}
			}
			if(dataItemViewer.CanSetMasterFilter) {
				if(pointList != null && values.GetType() != typeof(string)) {
					IEnumerable<object> typedPointList = pointList.Cast<object>();
					if(typedPointList.Count() > 1) {
						DataDashboardItem dashboardItem = FindDataDashboardItemForAPI(dashboardItemName);
						if(dashboardItem.GetSelectionLength() == 1) {
							if(CanSetMultiValueMasterFilter(dashboardItemName)) {
								List<IEnumerable<object>> parameter = new List<IEnumerable<object>>();
								foreach(object value in pointList)
									parameter.Add(new[] { value });
								dataItemViewer.SetMasterFilter(parameter);
							}
							else {
								throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityOperationNotAvailable));
							}
						}
						else {
							dataItemViewer.SetMasterFilter(new[] { typedPointList });
						}
					}
					else {
						dataItemViewer.SetMasterFilter(new[] { typedPointList });
					}
				}
				else {
					dataItemViewer.SetMasterFilter(new[] { new[] { values } });
				}
			}
			else {
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityOperationNotAvailable));
			}
		}
		public void ClearMasterFilter(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			if (dataItemViewer.CanClearMasterFilter)
				dataItemViewer.ClearMasterFilter();
			else
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityOperationNotAvailable));
		}
		public void PerformDrillDown(string dashboardItemName, AxisPointTuple tuple) {
			DataDashboardItem dataDashboardItem = FindDataDashboardItemForAPI(dashboardItemName);
			PerformDrillDown(dashboardItemName, tuple.GetAxisPoint(dataDashboardItem.GetAxisNames()[0]).UniqueValue);
		}
		public void PerformDrillDown(string dashboardItemName, DashboardDataRow value) {
			if (value != null && value.Length > 0)
				PerformDrillDown(dashboardItemName, value[0]);
			else
				throw new ArgumentException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageEmptyDrillDownValue));
		}
		public void PerformDrillDown(string dashboardItemName, object value) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			if (dataItemViewer.CanDrillDown)
				dataItemViewer.PerformDrillDown(new object[] { value });
			else
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityOperationNotAvailable));
		}
		public void PerformDrillUp(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			if (dataItemViewer.CanDrillUp)
				dataItemViewer.PerformDrillUp();
			else
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityOperationNotAvailable));
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName) {
			return GetUnderlyingData(dashboardItemName, (IList<UnderlyingDataTargetValue>)null, null);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName, IList<string> dataMembers) {
			return GetUnderlyingData(dashboardItemName, (IList<UnderlyingDataTargetValue>)null, dataMembers);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName, IList<AxisPoint> targetValues, IList<string> dataMembers) {
			DataDashboardItemViewer itemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			return itemViewer.GetUnderlyingData(targetValues, dataMembers);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName, IList<UnderlyingDataTargetValue> targetValues, IList<string> dataMembers) {
			DataDashboardItemViewer itemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			return itemViewer.GetUnderlyingData(targetValues, dataMembers);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName, IList<AxisPoint> targetValues) {
			return GetUnderlyingData(dashboardItemName, targetValues, null);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName, IList<UnderlyingDataTargetValue> targetValues) {
			return GetUnderlyingData(dashboardItemName, targetValues, null);
		}
		public MultiDimensionalData GetItemData(string dashboardItemName) {
			DataDashboardItemViewer itemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			if(itemViewer != null) {
				return itemViewer.MultiDimensionalData;
			}
			return null;
		}
		public RangeFilterSelection GetCurrentRange(string dashboardItemName) {
			RangeFilterDashboardItemViewer rangeViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true) as RangeFilterDashboardItemViewer;
			if (rangeViewer != null)
				return rangeViewer.GetCurrentRange(null, null);
			else
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityRangeFilterRequired));
		}
		public RangeFilterSelection GetEntireRange(string dashboardItemName) {
			RangeFilterDashboardItem rangeFilter = FindDashboardItemForAPI(dashboardItemName) as RangeFilterDashboardItem;
			if (rangeFilter == null)
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityRangeFilterRequired));
			IDashboardItemInteractivityService interactivityService = ServiceProvider.RequestServiceStrictly<IDashboardItemInteractivityService>();
			return interactivityService.GetEntireRange(dashboardItemName);
		}
		public void SetRange(string dashboardItemName, RangeFilterSelection range) {
			RangeFilterDashboardItemViewer rangeViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true) as RangeFilterDashboardItemViewer;
			if (rangeViewer != null)
				rangeViewer.SetRange(range.Minimum, range.Maximum);
			else
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityRangeFilterRequired));
		}
		public DashboardDataSet GetCurrentSelection(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, true);
			return dataItemViewer.GetCurrentSelection(null);
		}
		public bool CanSetMultiValueMasterFilter(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, false);
			if (dataItemViewer != null)
				return dataItemViewer.CanSetMultipleMasterFilter;
			else
				return false;
		}
		public bool CanSetMasterFilter(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, false);
			if (dataItemViewer != null)
				return dataItemViewer.CanSetMasterFilter;
			else
				return false;
		}
		public bool CanClearMasterFilter(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, false);
			if (dataItemViewer != null)
				return dataItemViewer.CanClearMasterFilter;
			else
				return false;
		}
		public bool CanPerformDrillDown(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, false);
			if (dataItemViewer != null)
				return dataItemViewer.CanDrillDown;
			else
				return false;
		}
		public bool CanPerformDrillUp(string dashboardItemName) {
			DataDashboardItemViewer dataItemViewer = FindDataDashboardItemViewerForAPI(dashboardItemName, false);
			if (dataItemViewer != null)
				return dataItemViewer.CanDrillUp;
			else
				return false;
		}
		public void LoadDashboard(string filePath) {
			IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
			ownerService.CreateDashboard(filePath);
		}
		public void LoadDashboard(Stream stream) {
			IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
			ownerService.CreateDashboard(stream);
		}
		public void SaveDashboardLayout(string filePath) {
			if(Dashboard == null)
				return;
			using(Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				SaveDashboardLayout(stream);
		}
		public void SaveDashboardLayout(Stream stream) {
			if (Dashboard == null)
				return;
			XmlHelper.CheckStream(stream);
			IDashboardPaneAdapter dashboardPaneAdapter = ServiceProvider.RequestServiceStrictly<IDashboardPaneAdapter>();
			DashboardPane rootPane = dashboardPaneAdapter.GetRootPane();
			XElement layoutRootElement = DashboardLayoutSerializer.SaveLayoutToXml(rootPane);
			XmlHelper.SaveXmlToStream(stream, layoutRootElement);
		}
		public void LoadDashboardLayout(string filePath) {
			if(Dashboard == null)
				return;
			using(Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				LoadDashboardLayout(stream);
		}
		public void LoadDashboardLayout(Stream stream) {
			if (Dashboard == null)
				return;
			XmlHelper.CheckStream(stream);
			IDashboardPaneAdapter dashboardPaneAdapter = ServiceProvider.RequestServiceStrictly<IDashboardPaneAdapter>();
			XElement layoutTreeElement = XmlHelper.LoadXmlFromStream(stream, DashboardLayoutSerializer.XmlLayoutTree);
			DashboardPane rootPane = (DashboardPane)DashboardLayoutSerializer.LoadLayoutFromXml(layoutTreeElement, new DashboardPaneFactory());
			if (rootPane != null)
				dashboardPaneAdapter.SetRootPane(rootPane);
		}		
		public void ReloadData() {
			ReloadData(false);
		}
		public void ReloadData(bool suppressWaitForm) {
			serviceClient.ReloadData(suppressWaitForm, Parameters);
		}
		[Obsolete("This method overload is now obsolete. To reload data in the data sources and specify current parameter values, use other method overloads and the DashboardViewer.Parameters property, respectively.")]
		public void ReloadData(IEnumerable<IParameter> parameters) {
			serviceClient.ReloadData(false, parameters);
		}
		public void BeginUpdateParameters() {
			parametersLocker.Lock();
		}
		public void EndUpdateParameters() {
			EndUpdateParameters(true);
		}
		public void ShowPrintPreview() {
			ShowPrintPreview(DashboardPrintPreviewType.StandardPreview);
		}
		public void ShowRibbonPrintPreview() {
			ShowPrintPreview(DashboardPrintPreviewType.RibbonPreview);
		}
		public void ShowDashboardParametersForm() {
			if(Dashboard != null && Dashboard.Parameters.Count > 0)
				new DashboardParametersCommand(this, new ParameterValueEditor()).Execute();
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			ExportToPdf(stream, options, this, this);
		}
		public void ExportToPdf(Stream stream) {
			ExportToPdf(stream, null, this, this);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			ExportToImage(stream, options, this, this);
		}
		public void ExportToImage(Stream stream) {
			ExportToImage(stream, null, this, this);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			ExportToPdf(filePath, options, this, this);
		}
		public void ExportToPdf(string filePath) {
			ExportToPdf(filePath, null, this, this);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			ExportToImage(filePath, options, this, this);
		}
		public void ExportToImage(string filePath) {
			ExportToImage(filePath, null, this, this);
		}
		public void ExportDashboardItemToPdf(string dashboardItemName, Stream stream, PdfExportOptions options) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToPdf(stream, options, item, item);
		}
		public void ExportDashboardItemToPdf(string dashboardItemName, Stream stream) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToPdf(stream, null, item, item);
		}
		public void ExportDashboardItemToImage(string dashboardItemName, Stream stream, ImageExportOptions options) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToImage(stream, options, item, item);
		}
		public void ExportDashboardItemToImage(string dashboardItemName, Stream stream) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToImage(stream, null, item, item);
		}
		public void ExportDashboardItemToExcel(string dashboardItemName, Stream stream, ExcelExportOptions options) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToExcel(stream, options, item, item);
		}
		public void ExportDashboardItemToPdf(string dashboardItemName, string filePath, PdfExportOptions options) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToPdf(filePath, options, item, item);
		}
		public void ExportDashboardItemToPdf(string dashboardItemName, string filePath) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToPdf(filePath, null, item, item);
		}
		public void ExportDashboardItemToImage(string dashboardItemName, string filePath, ImageExportOptions options) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToImage(filePath, options, item, item);
		}
		public void ExportDashboardItemToImage(string dashboardItemName, string filePath) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToImage(filePath, null, item, item);
		}
		public void ExportDashboardItemToExcel(string dashboardItemName, string filePath, ExcelExportOptions options) {
			DashboardItemViewer item = FindDashboardItemViewerForAPI(dashboardItemName, true);
			ExportToExcel(filePath, options, item, item);
		}
		internal void EndUpdateParameters(bool forceChanged) {
			parametersLocker.Unlock();
			if(forceChanged)
				Parameters.RaiseCollectionChanged();
		}
		internal void ClearParameters() {
			parameters = null;
		}
		internal void RaiseCustomizeDashboardTitleText(CustomizeDashboardTitleTextEventArgs args) {
			if (CustomizeDashboardTitleText != null)
				CustomizeDashboardTitleText(this, args);
		}
		IEnumerable<Control> IUnderlyingControlProvider.GetUnderlyingControls() {
			return ItemViewers.Select<DashboardItemViewer, Control>(itemViewer => itemViewer.ViewControl);
		}
		Control IUnderlyingControlProvider.GetUnderlyingControl(DashboardItem dashboardItem) {
			Guard.ArgumentNotNull(dashboardItem, "dashboardItem");
			return ((IUnderlyingControlProvider)this).GetUnderlyingControl(dashboardItem.ComponentName);
		}
		Control IUnderlyingControlProvider.GetUnderlyingControl(string componentName) {
			DashboardItemViewer itemViewer = ItemViewers.FirstOrDefault<DashboardItemViewer>(iViewer => iViewer.DashboardItemName == componentName);
			if (itemViewer != null)
				return itemViewer.ViewControl;
			return null;
		}
		protected internal virtual void OnDashboardItemClick(object sender, DashboardItemMouseActionEventArgs e) {
			if (DashboardItemClick != null)
				DashboardItemClick(this, e);
		}
		protected internal virtual void OnDashboardItemDoubleClick(object sender, DashboardItemMouseActionEventArgs e) {
			if (DashboardItemDoubleClick != null)
				DashboardItemDoubleClick(this, e);
		}
		protected internal virtual void OnDashboardItemMouseMove(object sender, DashboardItemMouseActionEventArgs e) {
			if (DashboardItemMouseMove != null)
				DashboardItemMouseMove(this, e);
		}
		protected internal virtual void OnDashboardItemMouseEnter(object sender, DashboardItemMouseEventArgs e) {
			if (DashboardItemMouseEnter != null)
				DashboardItemMouseEnter(this, e);
		}
		protected internal virtual void OnDashboardItemMouseLeave(object sender, DashboardItemMouseEventArgs e) {
			if (DashboardItemMouseLeave != null)
				DashboardItemMouseLeave(this, e);
		}
		protected internal virtual void OnDashboardItemMouseHover(object sender, DashboardItemMouseEventArgs e) {
			if (DashboardItemMouseHover != null)
				DashboardItemMouseHover(this, e);
		}
		protected internal virtual void OnDashboardItemMouseUp(object sender, DashboardItemMouseActionEventArgs e) {
			if (DashboardItemMouseUp != null)
				DashboardItemMouseUp(this, e);
		}
		protected internal virtual void OnDashboardItemMouseDown(object sender, DashboardItemMouseActionEventArgs e) {
			if (DashboardItemMouseDown != null)
				DashboardItemMouseDown(this, e);
		}
		protected internal virtual void OnDashboardItemMouseWheel(object sender, DashboardItemMouseEventArgs e) {
			if (DashboardItemMouseWheel != null)
				DashboardItemMouseWheel(this, e);
		}
		protected void OnDashboardItemVisualInteractivity(object sender, DashboardItemVisualInteractivityEventArgs e) {
			if (DashboardItemVisualInteractivity != null)
				DashboardItemVisualInteractivity(this, e);
		}
		protected void OnDashboardItemSelectionChanged(object sender, DashboardItemSelectionChangedEventArgs e) {
			if (DashboardItemSelectionChanged != null)
				DashboardItemSelectionChanged(this, e);
		}
		protected void OnDashboardItemControlUpdated(object sender, DashboardItemControlEventArgs e) {
			if(DashboardItemControlUpdated != null)
				DashboardItemControlUpdated(this, e);
		}
		protected void OnDashboardItemControlCreating(object sender, DashboardItemControlEventArgs e) {
			if(DashboardItemControlCreated != null)
				DashboardItemControlCreated(this, e);
		}
		protected void OnDashboardItemBeforeControlDisposed(object sender, DashboardItemControlEventArgs e) {
			if(DashboardItemBeforeControlDisposed != null)
				DashboardItemBeforeControlDisposed(this, e);
		}
		protected void  OnDashboardItemElementCustomColor(object sender, DashboardItemElementCustomColorEventArgs e) {
			if(DashboardItemElementCustomColor != null)
				DashboardItemElementCustomColor(this, e);
		}
		void RaiseDashboardLoaded(Dashboard dashboard) {
			if (DashboardLoaded != null)
				DashboardLoaded(this, new DashboardLoadedEventArgs(dashboard));
		}
		void RaiseDashboardChanged() {
			if (DashboardChanged != null)
				DashboardChanged(this, EventArgs.Empty);
		}
		DashboardItem FindDashboardItemForAPI(string componentName) {
			IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
			DashboardItem dashboardItem = ownerService.FindDashboardItemOrGroup(componentName);
			if (dashboardItem == null)
				throw new ArgumentException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityDashboardItemNotFound));
			return dashboardItem;
		}
		DataDashboardItem FindDataDashboardItemForAPI(string componentName) {
			DataDashboardItem dataDashboardItem = FindDashboardItemForAPI(componentName) as DataDashboardItem;
			if (dataDashboardItem == null)
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityDataDashboardItemRequired));
			return dataDashboardItem;
		}
		DashboardItemViewer FindDashboardItemViewerForAPI(string componentName, bool raiseExceptions) {
			DashboardItemViewer itemViewer = FindDashboardItemViewer(componentName);
			if (itemViewer == null && raiseExceptions)
				throw new ArgumentException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityDashboardItemNotFound));
			return itemViewer;
		}
		DataDashboardItemViewer FindDataDashboardItemViewerForAPI(string componentName, bool raiseExceptions) {
			DashboardItemViewer itemViewer = FindDashboardItemViewerForAPI(componentName, raiseExceptions);
			DataDashboardItemViewer dataItemViewer = itemViewer as DataDashboardItemViewer;
			if (dataItemViewer == null && raiseExceptions)
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageInteractivityDataDashboardItemRequired));
			return dataItemViewer;
		}
	}
}
