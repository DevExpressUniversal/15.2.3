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

#if DebugTest
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Kpi;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
namespace DevExpress.ExpressApp.Kpi {
	public class KpiInstanceEx : KpiInstance, IChartTitleProvider {
		private DrilldownProperty currentDrilldown;
		private IList<object> GetAxisValues(DrilldownProperty drilldownProperty) {
			List<object> result = new List<object>();
			if(drilldownProperty != null) {
				Type type = drilldownProperty.PropertyInfo.LastMember.MemberType;
				if(drilldownProperty.PropertyInfo.LastMember.MemberTypeInfo.IsPersistent) {
					foreach(object item in new XPCollection(Session, type, LocalizedCriteriaWrapper.ParseCriteriaWithReadOnlyParameters(drilldownProperty.Criteria, type))) {
						result.Add(item);
					}
				} else {
					XPClassInfo classInfo = Session.GetClassInfo(KpiDefinitionEx.TargetObjectType);
					CriteriaOperatorCollection props = new CriteriaOperatorCollection();
					props.Add(new OperandProperty(drilldownProperty.PropertyName.Name));
					foreach(object item in Session.SelectData(classInfo, props, LocalizedCriteriaWrapper.ParseCriteriaWithReadOnlyParameters(drilldownProperty.Criteria, type), props, null, false, 0, new SortingCollection(new SortProperty(drilldownProperty.PropertyName.Name, DevExpress.Xpo.DB.SortingDirection.Descending)))) {
						result.Add(((object[])(item))[0]);
					}
				}
			}
			if(result.Count == 0) {
				result.Add(null);
			}
			return result;
		}
		private KpiWithDrillDownHistoryItem GetHistoryItem(DrilldownProperty drilldownProperty, object axisValue, DateTime rangeStart, DateTime rangeEnd) {
			string drilldownPropertyName = drilldownProperty != null && drilldownProperty.PropertyName != null ? drilldownProperty.PropertyName.Name : "";
			KpiWithDrillDownHistoryItem historyItem = (KpiWithDrillDownHistoryItem)FindHistoryItem(drilldownPropertyName, axisValue != null ? axisValue.ToString() : "", rangeStart, rangeEnd);
			if(historyItem == null) {
				CriteriaOperator drilldownCriteria = (!string.IsNullOrEmpty(drilldownPropertyName)) ? new BinaryOperator(drilldownPropertyName, axisValue) : null;
				float measurementValue = KpiDefinitionEx.Evaluate(drilldownCriteria, rangeStart, rangeEnd);
				historyItem = new KpiWithDrillDownHistoryItem(Session, this, drilldownPropertyName, axisValue != null ? axisValue.ToString() : "", measurementValue, rangeStart, rangeEnd);
				AddMeasurement(historyItem);
				historyItem.Save();
				RefreshKpiChartDataSource();
			}
			return historyItem;
		}
		private KpiMeasurementHistoryItem[] GetValuesFromHistoryForDrilldownProperty(DrilldownProperty drilldownProperty, MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd) {
			List<KpiMeasurementHistoryItem> result = new List<KpiMeasurementHistoryItem>();
			if(!(drilldownProperty != null && measurementType == MeasurementType.Previous)) {
				foreach(object axisValue in GetAxisValues(drilldownProperty)) {
					KpiWithDrillDownHistoryItem historyItem = GetHistoryItem(drilldownProperty, axisValue, rangeStart, rangeEnd);
					KpiMeasurementHistoryItem mhi = new KpiMeasurementHistoryItem(!string.IsNullOrEmpty(historyItem.AxisValue) ? historyItem.AxisValue : measurementType.ToString(), rangeEnd, historyItem.Value);
					result.Add(mhi);
				}
			}
			return result.ToArray();
		}
		protected KpiDefinitionEx KpiDefinitionEx {
			get { return (KpiDefinitionEx)KpiDefinition; }
		}
		public KpiInstanceEx(Session session)
			: base(session) { }
		public KpiInstanceEx(Session session, KpiDefinitionEx kpi)
			: base(session, kpi) { }
		public IKpiHistoryItem FindHistoryItem(string axisName, string axisValue, DateTime startDate, DateTime endDate) {
			bool findNow = (endDate - startDate) < new TimeSpan(0, 0, 1);
			foreach(KpiHistoryItem item in HistoryItems) {
				KpiWithDrillDownHistoryItem kpiWithDrillDownHistoryItem = item as KpiWithDrillDownHistoryItem;
				if(kpiWithDrillDownHistoryItem == null || kpiWithDrillDownHistoryItem.AxisName != axisName || kpiWithDrillDownHistoryItem.AxisValue != axisValue) {
					continue;
				}
				if(findNow) {
					if(item.RangeStart >= GetCacheEndBoundary()
						&& item.RangeEnd >= GetCacheEndBoundary()) {
						return item;
					}
				} else {
					if(item.RangeStart >= startDate.AddSeconds(-1) && item.RangeStart <= startDate.AddSeconds(1)
						&& item.RangeEnd >= endDate.AddSeconds(-1) && item.RangeEnd <= endDate.AddSeconds(1)) {
						return item;
					}
				}
			}
			return null;
		}
		public override float GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd) {
			return GetHistoryItem(null, null, rangeStart, rangeEnd).Value;
		}
		public override IList GetValuesFromHistory(MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd) {
			List<KpiMeasurementHistoryItem> result = new List<KpiMeasurementHistoryItem>();
			result.AddRange(GetValuesFromHistoryForDrilldownProperty(CurrentDrilldown, measurementType, rangeStart, rangeEnd));
			if(KpiDefinition.Range.StartPoint == KpiDefinition.Range.EndPoint && KpiDefinition.Compare == false && CurrentDrilldown == null) {
				DateTime range = KpiInstanceHelper.AddDuration(KpiDefinition.Range.StartPoint, KpiDefinition.MeasurementFrequency, true);
				while(range >= GetCacheStartBoundary()) {
					result.AddRange(GetValuesFromHistoryForDrilldownProperty(CurrentDrilldown, measurementType, range, range));
					range = KpiInstanceHelper.AddDuration(range, KpiDefinition.MeasurementFrequency, true);
				}
			}
			return result;
		}
		public override string Settings {
			get {
				if(CurrentDrilldown != null) {
					return CurrentDrilldown.Settings;
				}
				return base.Settings;
			}
			set {
				if(CurrentDrilldown != null) {
					CurrentDrilldown.Settings = value;
				} else {
					base.Settings = value;
				}
			}
		}
		[DataSourceProperty("AvailableDrilldowns")]
		[ImmediatePostData]
		[NonPersistent]
		public DrilldownProperty CurrentDrilldown {
			get { return currentDrilldown ?? KpiDefinitionEx.ActiveDrilldown; }
			set {
				if(currentDrilldown != value) {
					currentDrilldown = value;
					RefreshKpiChartDataSource();
					RaisePropertyChangedEvent("ChartDataSource");
				}
			}
		}
		[Browsable(false)]
		public IList<DrilldownProperty> AvailableDrilldowns {
			get { return KpiDefinitionEx.DrilldownProperties; }
		}
		public string Title {
			get {
				if(CurrentDrilldown != null) {
					return string.Format("{0}\r\n({1})", KpiDefinition.Name, CurrentDrilldown.Caption);
				}
				return string.Format("{0}\r\n({1})", KpiDefinition.Name, KpiDefinition.Period);
			}
		}
	}
	public class KpiWithDrillDownHistoryItem : KpiHistoryItem {
		private string axisName;
		private string axisValue;
		public KpiWithDrillDownHistoryItem(Session session)
			: base(session) { }
		public KpiWithDrillDownHistoryItem(Session session, KpiInstance kpiInstance, string axisName, string axisValue, float measurementValue, DateTime rangeStart, DateTime rangeEnd)
			: base(session, kpiInstance, measurementValue, rangeStart, rangeEnd) {
			this.axisName = axisName;
			this.axisValue = axisValue;
		}
		public string AxisName {
			get { return axisName; }
			set { SetPropertyValue("AxisName", ref axisName, value); }
		}
		public string AxisValue {
			get { return axisValue; }
			set { SetPropertyValue("AxisValue", ref axisValue, value); }
		}
	}
}
#endif
