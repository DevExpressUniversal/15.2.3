#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.XtraCharts.Native;
namespace DevExpress.ExpressApp.PivotChart {
	public class DataSourceCreatingEventArgs : HandledEventArgs {
		private IAnalysisInfo analysisInfo;
		private object dataSource;
		public DataSourceCreatingEventArgs(IAnalysisInfo analysisInfo) {
			this.analysisInfo = analysisInfo;
		}
		public IAnalysisInfo AnalysisInfo {
			get { return analysisInfo; }
		}
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
	}
	public class DataSourceCreatedEventArgs : EventArgs {
		private IAnalysisInfo analysisInfo;
		private object dataSource;
		public DataSourceCreatedEventArgs(IAnalysisInfo analysisInfo, object dataSource) {
			this.analysisInfo = analysisInfo;
			this.dataSource = dataSource;
		}
		public IAnalysisInfo AnalysisInfo {
			get { return analysisInfo; }
		}
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
	}
	public abstract class AnalysisEditorBase : PropertyEditor {
		private IAnalysisInfo analysisInfo;
		private bool isUpdatingDataSource;
		private bool isUpdatingSettings;
		private bool isDataSourceReady = true;
		private bool settingsAreExpired = false;
		private void OnObjectTypeChanged() {
			UpdateDataSource();
			SavePivotGridSettings();
			settingsAreExpired = true;
		}
		private void UpdateDataSourceAndControl() {
			if(Control != null) {
				try {
					Control.BeginUpdate();
					UpdateDataSource();
					UpdatePivotGridSettings();
				}
				finally {
					Control.EndUpdate();
				}
			}
		}
		private void OnCriteriaChanged() {
			if(IsDataSourceReady) {
				UpdateDataSourceAndControl();
			}
		}
		private void OnDimensionPropertiesChanged() {
			if(IsDataSourceReady) {
				UpdateDataSourceAndControl();
			}
		}
		private void analysisInfo_InfoChanged(object sender, AnalysisInfoChangedEventEventArgs e) {
			switch(e.ChangeType) {
				case AnalysisInfoChangeType.ObjectTypeChanged:
					OnObjectTypeChanged();
					break;
				case AnalysisInfoChangeType.CriteriaChanged:
					OnCriteriaChanged();
					break;
				case AnalysisInfoChangeType.DimensionPropertiesChanged:
					OnDimensionPropertiesChanged();
					break;
			}
			OnAnalysisInfoChanged();
		}
		private void UpdateDataSource() {
			try {
				isUpdatingDataSource = true;
				if(IsDataSourceReady) {
					object dataSource = null;
					DataSourceCreatingEventArgs dataSourceCreatingEventArgs = new DataSourceCreatingEventArgs(analysisInfo);
					OnDataSourceCreating(dataSourceCreatingEventArgs);
					if(dataSourceCreatingEventArgs.Handled) {
						dataSource = dataSourceCreatingEventArgs.DataSource;
					}
					else {
						if(analysisInfo != null && analysisInfo.DataType != null) {
							CriteriaOperator criteriaOperator = null;
							if(!string.IsNullOrEmpty(analysisInfo.Criteria)) {
								string convertedString = CriteriaStringHelper.ConvertFromOldFormat(analysisInfo.Criteria, View.ObjectSpace, XafTypesInfo.Instance.FindTypeInfo(analysisInfo.DataType));
								criteriaOperator = View.ObjectSpace.ParseCriteria(convertedString);
							}
							IList originalCollection = View.ObjectSpace.CreateCollection(analysisInfo.DataType, criteriaOperator);
							dataSource = new ProxyCollection(View.ObjectSpace, XafTypesInfo.Instance.FindTypeInfo(analysisInfo.DataType), originalCollection);
						}
						else {
							dataSource = null;
						}
					}
					DataSourceCreatedEventArgs dataSourceCreatedEventArgs = new DataSourceCreatedEventArgs(analysisInfo, dataSource);
					OnDataSourceCreated(dataSourceCreatedEventArgs);
					try {
						Control.DataSource = new AnalysisDataSource(analysisInfo, dataSourceCreatedEventArgs.DataSource);
					}
					catch {
						IsDataSourceReady = false;
						throw;
					}
				}
				else {
					Control.DataSource = new AnalysisDataSource(null, null);
				}
			}
			finally {
				isUpdatingDataSource = false;
			}
		}
		private void SubscribeToEvents() {
			if(analysisInfo != null) {
				analysisInfo.InfoChanged += new EventHandler<AnalysisInfoChangedEventEventArgs>(analysisInfo_InfoChanged);
			}
		}
		private void UnsubscribeFromEvents() {
			if(analysisInfo != null) {
				analysisInfo.InfoChanged -= new EventHandler<AnalysisInfoChangedEventEventArgs>(analysisInfo_InfoChanged);
			}
		}
		protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore();
		protected override object CreateControlCore() {
			IAnalysisControl analysisControl = CreateAnalysisControl();
			analysisControl.OptionsChartDataSource.ProvideColumnGrandTotals = false;
			analysisControl.OptionsChartDataSource.ProvideRowGrandTotals = false;
			analysisControl.PivotGridSettingsChanged += new EventHandler<EventArgs>(analysisControl_PivotGridSettingsChanged);
			analysisControl.ChartSettingsChanged += new EventHandler<EventArgs>(analysisControl_ChartSettingsChanged);
			if(analysisControl is ISupportPivotGridFieldBuilder) {
				((ISupportPivotGridFieldBuilder)analysisControl).FieldBuilder.SetModel(Model.Application);
			}
			return analysisControl;
		}
		private bool GetCanSaveSettings() {
			return Control != null && !Control.ReadOnly && PropertyValue != null && !inReadValue && !isUpdatingSettings && !isUpdatingDataSource && IsDataSourceReady;
		}
		internal void SavePivotGridSettings() {
			if(analysisInfo != null && GetCanSaveSettings()) {
				if(PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(), analysisInfo)) {
					OnControlValueChanged();
					settingsAreExpired = false;
				}
			}
		}
		private void SaveChartSettings() {
			if(analysisInfo != null && GetCanSaveSettings()) {
				if(ChartSettingsHelper.SaveChartSettings(Control.Chart, analysisInfo)) {
					OnControlValueChanged();
				}
			}
		}
		private void analysisControl_PivotGridSettingsChanged(object sender, EventArgs e) {
			SavePivotGridSettings();
		}
		private void analysisControl_ChartSettingsChanged(object sender, EventArgs e) {
			SaveChartSettings();
		}
		protected abstract IAnalysisControl CreateAnalysisControl();
		protected virtual void OnPivotGridSettingsLoaded() {
			if(PivotGridSettingsLoaded != null) {
				PivotGridSettingsLoaded(this, EventArgs.Empty);
			}
		}
		protected virtual void UpdatePivotGridSettings() {
			if(PropertyValue != null && !settingsAreExpired && analysisInfo != null) {
				try {
					isUpdatingSettings = true;
					if(PivotGridSettingsHelper.HasPivotGridSettings(analysisInfo)) {
						Control.Fields.Clear();
						PivotGridSettingsHelper.LoadPivotGridSettings(CreatePivotGridSettingsStore(), analysisInfo);
						OnPivotGridSettingsLoaded();
					}
					if(Control is ISupportPivotGridFieldBuilder) {
						((ISupportPivotGridFieldBuilder)Control).FieldBuilder.ApplySettings();
					}
				}
				finally {
					isUpdatingSettings = false;
				}
			}
		}
		private void BindDataSource(bool saveSettings) {
			if(Control != null) {
				Control.BeginUpdate();
				try {
					if(IsDataSourceReady) {
						UpdateChartSettings();
						UpdateDataSource();
						UpdatePivotGridSettings();
					}
					else {
						if(saveSettings) {
							SavePivotGridSettings();
							SaveChartSettings();
						}
						UpdateDataSource();
					}
				}
				finally {
					Control.EndUpdate();
				}
			}
		}
		protected override void ReadValueCore() {
			if(Control != null) {
				UnsubscribeFromEvents();
				analysisInfo = PropertyValue as IAnalysisInfo;
				SubscribeToEvents();
				BindDataSource(false);
			}
		}
		protected virtual void UpdateChartSettings() {
			if(analysisInfo != null) {
				ChartSettingsHelper.LoadChartSettings(Control.Chart, analysisInfo);
			}
		}
		protected override object GetControlValueCore() {
			return null;
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(Control != null) {
						Control.PivotGridSettingsChanged -= new EventHandler<EventArgs>(analysisControl_PivotGridSettingsChanged);
						Control.ChartSettingsChanged -= new EventHandler<EventArgs>(analysisControl_ChartSettingsChanged);
					}
					UnsubscribeFromEvents();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnDataSourceCreating(DataSourceCreatingEventArgs args) {
			if(DataSourceCreating != null)
				DataSourceCreating(this, args);
		}
		protected virtual void OnDataSourceCreated(DataSourceCreatedEventArgs args) {
			if(DataSourceCreated != null)
				DataSourceCreated(this, args);
		}
		protected virtual void OnIsDataSourceReadyChanged() {
			if(IsDataSourceReadyChanged != null) {
				IsDataSourceReadyChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnAnalysisInfoChanged() {
			if(AnalysisInfoChanged != null) {
				AnalysisInfoChanged(this, EventArgs.Empty);
			}
		}
		public DevExpress.XtraCharts.ViewType ChartType {
			get { return SeriesViewFactory.GetViewType(Control.Chart.DataContainer.SeriesTemplate.View); }
		}
		public new IAnalysisControl Control {
			get { return (IAnalysisControl)base.Control; }
		}
		public bool IsDataSourceReady {
			get { return isDataSourceReady; }
			set { 
				isDataSourceReady = value;
				OnIsDataSourceReadyChanged();
				BindDataSource(true);
			}
		}
		public AnalysisEditorBase(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
		}
		public event EventHandler<DataSourceCreatingEventArgs> DataSourceCreating;
		public event EventHandler<DataSourceCreatedEventArgs> DataSourceCreated;
		public event EventHandler<EventArgs> IsDataSourceReadyChanged;
		public event EventHandler PivotGridSettingsLoaded;
		public event EventHandler AnalysisInfoChanged;
#if DebugTest
		public void DebugTest_UpdateDataSource() {
			UpdateDataSource();
		}
		public IAnalysisInfo DebugTest_AnalysisInfo {
			get { return analysisInfo; }
			set { analysisInfo = value; }
		}
#endif
	}
}
