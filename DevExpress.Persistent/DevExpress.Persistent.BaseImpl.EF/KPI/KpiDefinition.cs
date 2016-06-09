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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Filtering;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base.Kpi;
namespace DevExpress.Persistent.BaseImpl.EF.Kpi {
	[NavigationItem(DevExpress.ExpressApp.Kpi.KpiModule.KpiNavigationGroupName)]
	[DefaultProperty("Name")]
	[DisplayName("Definition")]
	[ImageName("BO_KPI_Definition")]
	public class KpiDefinition : BaseKpiObject, DevExpress.ExpressApp.Kpi.IKpiDefinition, IObjectSpaceLink, IXafEntityObject {
		private List<IFilterParameter> criteriaParameters;
		private DevExpress.ExpressApp.Kpi.Direction direction = DevExpress.ExpressApp.Kpi.Direction.HighIsBetter;
		private DevExpress.ExpressApp.Kpi.TimeIntervalType measurementFrequency = DevExpress.ExpressApp.Kpi.TimeIntervalType.Day;
		private DevExpress.ExpressApp.Kpi.MeasurementMode measurementMode = DevExpress.ExpressApp.Kpi.MeasurementMode.Interval;
		private IObjectSpace objectSpace;
		private CriteriaOperator GetCriteriaOperator(DevExpress.ExpressApp.Kpi.IDateRange range) {
			return range != null ? GetCriteriaOperator(range.StartPoint, range.EndPoint) : null;
		}
		protected void SetKpiInstance(KpiInstance kpiInstance) {
			Guard.ArgumentNotNull(kpiInstance, "kpiInstance");
			if(KpiInstances.Count == 1) {
				objectSpace.Delete(KpiInstances[0]);
				KpiInstances.Clear();
			}
			KpiInstances.Add(kpiInstance);
		}
		static KpiDefinition() {
			RegisterParameters();
		}
		public static void RegisterParameters() {
			ParametersFactory.RegisterParameter(new UpdatableParameter(KpiHelper.RangeStartParameterName, typeof(DateTime)));
			if(CriteriaOperator.GetCustomFunction(KpiHelper.RangeStartParameterName) == null) {
				CriteriaOperator.RegisterCustomFunction(
					new DevExpress.ExpressApp.SystemModule.ParameterCustomFunctionOperator(ParametersFactory.FindParameter(KpiHelper.RangeStartParameterName)));
			}
			ParametersFactory.RegisterParameter(new UpdatableParameter(KpiHelper.RangeEndParameterName, typeof(DateTime)));
			if(CriteriaOperator.GetCustomFunction(KpiHelper.RangeEndParameterName) == null) {
				CriteriaOperator.RegisterCustomFunction(
					new DevExpress.ExpressApp.SystemModule.ParameterCustomFunctionOperator(ParametersFactory.FindParameter(KpiHelper.RangeEndParameterName)));
			}
		}
		public KpiDefinition()
			: base() {
				SuppressedSeries = DevExpress.ExpressApp.Kpi.MeasurementType.Previous.ToString();
			Active = true;
			Expression = "Count";
			ChangedOn = DateTime.Now;
			EnableCustomizeRepresentation = true;
			KpiInstances = new List<KpiInstance>();
		}
		public String Name { get; set; }
		public Boolean Active { get; set; }
		[Browsable(false)]
		public String TargetObjectTypeFullName { get; set; }
		[NotMapped]
		[ImmediatePostData]
		public Type TargetObjectType {
			get { return ReflectionHelper.FindType(TargetObjectTypeFullName); }
			set {
				String typeFullName = (value != null) ? value.FullName : null;
				if(TargetObjectTypeFullName != typeFullName) {
					Criteria = "";
					Expression = "Count";
					TargetObjectTypeFullName = typeFullName;
				}
			}
		}
		[Browsable(false)]
		public IEnumerable<IFilterParameter> CriteriaParameters {
			get {
				if(criteriaParameters == null) {
					criteriaParameters = new List<IFilterParameter>();
					criteriaParameters.Add(new XafFilterParameter(KpiHelper.RangeStartParameterName, typeof(DateTime)));
					criteriaParameters.Add(new XafFilterParameter(KpiHelper.RangeEndParameterName, typeof(DateTime)));
				}
				return criteriaParameters;
			}
		}
		[CriteriaOptions("TargetObjectType", "CriteriaParameters")]
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public String Criteria { get; set; }
		[ElementTypeProperty("TargetObjectType")]
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[Appearance("KpiDefinitionExpression", Enabled = false, Criteria = "TargetObjectType is null")]
		public String Expression { get; set; }
		public Single GreenZone { get; set; }
		public Single RedZone { get; set; }
		[Browsable(false)]
		public IEnumerable<DevExpress.ExpressApp.Kpi.IDateRange> AvailableRanges {
			get { return DevExpress.ExpressApp.Kpi.DateRangeRepository.GetRegisteredRanges(); }
		}
		[ImmediatePostData]
		public Boolean Compare { get; set; }
		[Browsable(false)]
		public String RangeName { get; set; }
		[DataSourceProperty("AvailableRanges")]
		[NotMapped]
		public DevExpress.ExpressApp.Kpi.IDateRange Range {
			get { return DevExpress.ExpressApp.Kpi.DateRangeRepository.FindRange(RangeName); }
			set {
				if(value != null) {
					RangeName = value.Name;
				}
				else {
					RangeName = "";
				}
			}
		}
		[Browsable(false)]
		public String RangeToCompareName { get; set; }
		[DataSourceProperty("AvailableRanges")]
		[Appearance("KpiDefinitionRangeToCompare", Enabled = false, Criteria = "Compare = False")]
		[NotMapped]
		public DevExpress.ExpressApp.Kpi.IDateRange RangeToCompare {
			get {
				if(!Compare) {
					return null;
				}
				else {
					return DevExpress.ExpressApp.Kpi.DateRangeRepository.FindRange(RangeToCompareName);
				}
			}
			set {
				if(value != null) {
					RangeToCompareName = value.Name;
				}
				else {
					RangeToCompareName = "";
				}
			}
		}
		public DevExpress.ExpressApp.Kpi.TimeIntervalType MeasurementFrequency {
			get { return measurementFrequency; }
			set { measurementFrequency = value; }
		}
		[Browsable(false)]
		public DevExpress.ExpressApp.Kpi.MeasurementMode MeasurementMode {
			get { return measurementMode; }
			set { measurementMode = value; }
		}
		public DevExpress.ExpressApp.Kpi.Direction Direction {
			get { return direction; }
			set { direction = value; }
		}
		[Browsable(false)]
		public String DrillDownListViewId {
			get {
				String kpiName = String.IsNullOrWhiteSpace(Name) ? "" : Name;
				return kpiName.Replace(' ', '_') + "_" + TargetObjectType.Name + "_Drilldown_ListView";
			}
		}
		[VisibleInDetailView(false)]
		public String Period {
			get {
				if(Range != null) {
					String rangeString = Range.Caption;
					return (Compare && RangeToCompare != null) ? rangeString + " " + CaptionHelper.GetLocalizedText("Texts", "VersusText") + " " + RangeToCompare.Caption : rangeString;
				}
				return "";
			}
		}
		[Browsable(false)]
		public DateTime ChangedOn { get; set; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public Single Current {
			get {
				try {
					return Evaluate(Range.StartPoint, Range.EndPoint);
				}
				catch(Exception) {
					return 0;
				}
			}
		}
		[ElementTypeProperty("TargetObjectType")]
		public IList Objects {
			get {
				if(new DevExpress.ExpressApp.Kpi.IsValidKpiCriteriaCodeRule().Validate(this).State == ValidationState.Valid) {
					CollectionSource collectionSource = GetDrilldownCollectionSource(objectSpace);
					return (collectionSource != null) ? (IList)collectionSource.Collection : null;
				}
				else {
					return null;
				}
			}
		}
		[Browsable(false)]
		public KpiInstance KpiInstance {
			get {
				if(KpiInstances.Count == 1) {
					return KpiInstances[0];
				}
				else {
					return null;
				}
			}
		}
		[Browsable(false), Aggregated, InverseProperty("KpiDefinition")]
		public virtual IList<KpiInstance> KpiInstances { get; set; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public String SuppressedSeries { get; set; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public Boolean EnableCustomizeRepresentation { get; set; }
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		void IXafEntityObject.OnCreated() {
			KpiInstance kpiInstance = (KpiInstance)objectSpace.CreateObject(typeof(KpiInstance));
			kpiInstance.KpiDefinition = this;
			KpiInstances.Add(kpiInstance);
			Range = DevExpress.ExpressApp.Kpi.DateRangeRepository.FindRange("Now");
			RangeToCompare = DevExpress.ExpressApp.Kpi.DateRangeRepository.FindRange("Now");
		}
		void IXafEntityObject.OnSaving() {
		}
		void IXafEntityObject.OnLoaded() {
		}
		public Single Evaluate(DateTime rangeStart, DateTime rangeEnd) {
			return Evaluate(null, rangeStart, rangeEnd);
		}
		public Single Evaluate(CriteriaOperator additionalCriteria, DateTime rangeStart, DateTime rangeEnd) {
			Single result = 0;
			if(TargetObjectType != null) {
				CriteriaOperator expression = KpiHelper.GetExpressionOperator(Expression, rangeStart, rangeEnd);
				CriteriaOperator criteriaOperator = GetCriteriaOperator(rangeStart, rangeEnd);
				if(!Object.ReferenceEquals(additionalCriteria, null)) {
					criteriaOperator = new GroupOperator(criteriaOperator, additionalCriteria);
				}
				List<DataViewExpression> expressions = new List<DataViewExpression>();
				expressions.Add(new DataViewExpression("A", expression));
				IList dataView = objectSpace.CreateDataView(TargetObjectType, expressions, criteriaOperator, null);
				if(dataView.Count > 0) {
					Object val = ((XafDataViewRecord)dataView[0])["A"];
					if((val != null) && (val != DBNull.Value)) {
						result = Convert.ToSingle(val);
					}
				}
			}
			return result;
		}
		public CollectionSource GetDrilldownCollectionSource(IObjectSpace os) {
			if(TargetObjectType == null || Range == null) {
				return null;
			}
			CollectionSource collectionSource = new CollectionSource(os, TargetObjectType, false, CollectionSourceMode.Proxy);
			collectionSource.Criteria.Add("KPI Drilldown", GetCriteriaOperator(Range));
			return collectionSource;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore)]
		public CriteriaWrapper GetCriteriaWrapper(DateTime rangeStart, DateTime rangeEnd) {
			return KpiHelper.GetCriteriaWrapper(objectSpace, Criteria, TargetObjectType, rangeStart, rangeEnd);
		}
		public CriteriaOperator GetCriteriaOperator(DateTime rangeStart, DateTime rangeEnd) {
			return KpiHelper.GetCriteriaOperator(objectSpace, Criteria, TargetObjectType, rangeStart, rangeEnd);
		}
	}
}
