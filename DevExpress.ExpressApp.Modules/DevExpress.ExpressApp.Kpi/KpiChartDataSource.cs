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
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.Data.ChartDataSources;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Kpi {
	public class KpiChartDataSource : IChartDataSourceProvider, IChartTitleProvider, ISparklineProvider, ISettingsProvider, ICustomizationEnabledProvider, IDisposable {
		private IKpiInstance kpiInstance;
		private IKpiDefinition KpiDefinition {
			get { return kpiInstance.GetKpiDefinition(); }
		}
		private IList chartDataSource;
		private DateTimeMeasureUnitNative? GetDateTimeMeasureUnit() {
			switch(KpiDefinition.MeasurementFrequency) {
				case TimeIntervalType.Hour:
					return DateTimeMeasureUnitNative.Hour;
				case TimeIntervalType.Day:
					return DateTimeMeasureUnitNative.Day;
				case TimeIntervalType.Week:
					return DateTimeMeasureUnitNative.Week;
				case TimeIntervalType.Month:
					return DateTimeMeasureUnitNative.Month;
				case TimeIntervalType.Quarter:
					return DateTimeMeasureUnitNative.Quarter;
				case TimeIntervalType.Year:
					return DateTimeMeasureUnitNative.Year;
			}
			return DateTimeMeasureUnitNative.Year;
		}
		protected virtual IList CreateMeasurementHistoryList() {
			KpiMeasurementHistoryItemList<KpiMeasurementHistoryItem> result = new KpiMeasurementHistoryItemList<KpiMeasurementHistoryItem>();
			result.DateTimeArgumentMeasureUnit = GetDateTimeMeasureUnit();
			return result;
		}
		private void FillList(IList target, IList source) {
			foreach(object item in source) {
				target.Add(item);
			}
		}
		private DateTime FindNearestDateTime(IList values, DateTime originalDateTime) {
			DateTime result = originalDateTime;
			long duration = TimeSpan.MaxValue.Ticks;
			foreach(object currObj in values) {
				KpiMeasurementHistoryItem item = currObj as KpiMeasurementHistoryItem;
				if(item != null) {
					TimeSpan tmpDuration = item.DateTime - originalDateTime;
					long tmpDurationTicks = Math.Abs(tmpDuration.Ticks);
					if(tmpDurationTicks < duration) {
						duration = tmpDurationTicks;
						result = item.DateTime;
					}
				}
			}
			return result;
		}
		private IList CreateMeasurementHistory(IDateRange range, MeasurementType measurementType) {
			IList history = CreateMeasurementHistoryList();
			DateTime rangeMeasureEnd = range.EndPoint;
			DateTime intervalStart = range.StartPoint;
			DateTime intervalEnd = KpiInstanceHelper.AddDuration(intervalStart, KpiDefinition.MeasurementFrequency, false).AddSeconds(-1);
			while(intervalStart < rangeMeasureEnd) {
				if(KpiDefinition.MeasurementMode == MeasurementMode.Interval) {
					FillList(history, ((IKpiHistoryCacheManager)kpiInstance).GetValuesFromHistory(measurementType, intervalStart, intervalEnd, intervalStart, intervalEnd > rangeMeasureEnd ? rangeMeasureEnd : intervalEnd));
				}
				else {
					FillList(history, ((IKpiHistoryCacheManager)kpiInstance).GetValuesFromHistory(measurementType, range.StartPoint, intervalEnd, range.StartPoint, intervalEnd > rangeMeasureEnd ? rangeMeasureEnd : intervalEnd));
				}
				intervalStart = KpiInstanceHelper.AddDuration(intervalStart, KpiDefinition.MeasurementFrequency, false);
				intervalEnd = KpiInstanceHelper.AddDuration(intervalStart, KpiDefinition.MeasurementFrequency, false).AddSeconds(-1);
			}
			return history;
		}
		private IList CreateChartDataSource() {
			((IKpiHistoryCacheManager)kpiInstance).CleanupCache(false);
			IList history = CreateMeasurementHistoryList();
			FillList(history, CreateMeasurementHistory(KpiDefinition.Range, MeasurementType.Current));
			if(KpiDefinition.Compare) {
				IList previousValues = CreateMeasurementHistory(KpiDefinition.RangeToCompare, MeasurementType.Previous);
				foreach(KpiMeasurementHistoryItem previousValue in previousValues) {
					previousValue.DateTime = FindNearestDateTime(history, KpiDefinition.Range.StartPoint + (previousValue.DateTime - KpiDefinition.RangeToCompare.StartPoint));
				}
				FillList(history, previousValues);
			}
			return history;
		}
		private void OnDataChanged() {
			if(DataChanged != null) {
				DataChanged(this, new DataChangedEventArgs(DataChangedType.Reset));
			}
		}
		public KpiChartDataSource(IKpiInstance kpiInstance) {
			Guard.ArgumentNotNull(kpiInstance, "kpiInstance");
			this.kpiInstance = kpiInstance;
		}
		public void Refresh() {
			if(!creatingDataSource) {
				chartDataSource = null;
				OnDataChanged();
			}
		}
		private bool creatingDataSource = false;
		#region IChartDataSourceProvider Members
		public object DataSource {
			get {
				if(chartDataSource == null) {
					creatingDataSource = true;
					chartDataSource = CreateChartDataSource();
					creatingDataSource = false;
				}
				return chartDataSource;
			}
		}
		public event DataChangedEventHandler DataChanged;
		#endregion
		#region IChartTitleProvider Members
		public string Title {
			get {
				if(kpiInstance is IChartTitleProvider) {
					return ((IChartTitleProvider)kpiInstance).Title;
				}
				return KpiDefinition.Name + "\r\n(" + KpiDefinition.Period + ")";
			}
		}
		#endregion
		#region ISparklineProvider Members
		public string[] SuppressedSeries {
			get {
				Guard.ArgumentNotNull(KpiDefinition, "KpiDefinition");
				if(!string.IsNullOrEmpty(KpiDefinition.SuppressedSeries)) {
					List<string> result = new List<string>();
					foreach(string serieName in KpiDefinition.SuppressedSeries.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)) {
						result.Add(CaptionHelper.GetLocalizedText("Enums\\" + typeof(MeasurementType).FullName, serieName, serieName));
					}
					return result.ToArray();
				}
				return new string[] { };
			}
		}
		#endregion
		#region ISettingsProvider Members
		public string Settings {
			get {
				if(kpiInstance is ISettingsProvider) {
					return ((ISettingsProvider)kpiInstance).Settings;
				}
				return null;
			}
			set {
				if(kpiInstance is ISettingsProvider) {
					((ISettingsProvider)kpiInstance).Settings = value;
				}
			}
		}
		#endregion
		#region ICustomizationEnabledProvider Members
		public bool CustomizationEnabled {
			get { return KpiDefinition.EnableCustomizeRepresentation; }
			set { KpiDefinition.EnableCustomizeRepresentation = value; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			chartDataSource = null;
			kpiInstance = null;
		}
		#endregion
	}
	internal class KpiMeasurementHistoryItemList<T> : List<T>, IChartDataSource where T : KpiMeasurementHistoryItem {
		#region IChartDataSource Members
		public string ArgumentDataMember {
			get { return "DateTime"; }
		}
		public DateTimeMeasureUnitNative? DateTimeArgumentMeasureUnit { get; set; }
		public Dictionary<DateTime, DateTimeMeasureUnitNative> DateTimeMeasureUnitByArgument {
			get { throw new NotImplementedException(); }
		}
		public string SeriesDataMember {
			get { return "Serie"; }
		}
		public string ValueDataMember {
			get { return "Value"; }
		}
#pragma warning disable 0067
		public event DataChangedEventHandler DataChanged;
#pragma warning restore 0067
		#endregion
	}
	public class KpiMeasurementHistoryItem {
		public KpiMeasurementHistoryItem(MeasurementType measurementType, DateTime dateTime, float val) : this(measurementType.ToString(), dateTime, val) { }
		public KpiMeasurementHistoryItem(string serie, DateTime dateTime, float val) {
			Serie = CaptionHelper.GetLocalizedText("Enums\\" + typeof(MeasurementType).FullName, serie, serie);
			DateTime = dateTime;
			Value = val;
		}
		public override string ToString() {
			return string.Format("{0} - {1}, {2}", DateTime, Value, Serie);
		}
		public string Serie { get; set; }
		public DateTime DateTime { get; set; }
		public float Value { get; set; }
	}
}
