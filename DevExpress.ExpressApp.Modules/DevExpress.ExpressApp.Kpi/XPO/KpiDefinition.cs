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
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Persistent.Base.Kpi;
namespace DevExpress.ExpressApp.Kpi {
	public class IDateRangeToStringConverter : ValueConverter {
		public override object ConvertFromStorageType(object range) {
			IDateRange result = DateRangeRepository.FindRange((string)range);
			if(result == null && !string.IsNullOrEmpty((string)range)) {
				throw new UserFriendlyException(string.Format(CaptionHelper.GetLocalizedText("Exceptions", "RangeNotFound"), (string)range));
			}
			return result;
		}
		public override object ConvertToStorageType(object objectType) {
			if(objectType == null) {
				return null;
			}
			return ((IDateRange)objectType).Name;
		}
		public override Type StorageType {
			get { return typeof(string); }
		}
	}
	[NavigationItem(KpiModule.KpiNavigationGroupName)]
	[DefaultProperty("Name")]
	[System.ComponentModel.DisplayName("Definition")]
	[ImageName("BO_KPI_Definition")]
	public class KpiDefinition : BaseKpiObject, IKpiDefinition {
		private string name;
		private bool active = true;
		private string criteria;
		private string expression = "Count";
		[Persistent("TargetObjectType")]
		private string targetObjectType;
		private List<IFilterParameter> criteriaParameters;
		private float greenZone;
		private float redZone;
		private string suppressedSeries = MeasurementType.Previous.ToString();
		private bool enableCustomizeRepresentation = true;
		private bool compare;
		private Direction direction = Direction.HighIsBetter;
		private TimeIntervalType measurementFrequency = TimeIntervalType.Day;
		private MeasurementMode measurementMode = MeasurementMode.Interval;
		[Persistent("Changed")]
		private DateTime changedOn = DateTime.Now;
		[Persistent("KpiInstance")]
		private KpiInstance kpiInstance;
		private CriteriaOperator GetCriteriaOperator(IDateRange range) {
			return range != null ? GetCriteriaOperator(range.StartPoint, range.EndPoint) : null;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore)]
		public CriteriaWrapper GetCriteriaWrapper(DateTime rangeStart, DateTime rangeEnd) {
			return KpiHelper.GetCriteriaWrapper(XPObjectSpace.FindObjectSpaceByObject(this), Criteria, TargetObjectType, rangeStart, rangeEnd);
		}
		public CriteriaOperator GetCriteriaOperator(DateTime rangeStart, DateTime rangeEnd) {
			return KpiHelper.GetCriteriaOperator(XPObjectSpace.FindObjectSpaceByObject(this), Criteria, TargetObjectType, rangeStart, rangeEnd);
		}
		protected override void OnChanged(string propertyName, object oldValue, object newValue) {
			base.OnChanged(propertyName, oldValue, newValue);
			if(propertyName == "Criteria"
			|| propertyName == "Expression"
			|| propertyName == "TargetObjectType"
			|| propertyName == "MeasurementFrequency"
			|| propertyName == "Range"
			|| propertyName == "Compare"
			|| propertyName == "RangeToCompare") {
				changedOn = DateTime.Now;
				OnChanged("Preview");
			}
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
		public KpiDefinition(DevExpress.Xpo.Session session)
			: base(session) {
		}
		public override void AfterConstruction() {
			base.AfterConstruction();
			kpiInstance = new KpiInstance(Session, this);
			Range = DateRangeRepository.FindRange("Now");
			RangeToCompare = DateRangeRepository.FindRange("Now");
		}
		protected override void OnDeleted() {
			base.OnDeleted();
			kpiInstance.Delete();
		}
		public float Evaluate(DateTime rangeStart, DateTime rangeEnd) {
			return Evaluate(null, rangeStart, rangeEnd);
		}
		public float Evaluate(CriteriaOperator additionalCriteria, DateTime rangeStart, DateTime rangeEnd) {
			if(TargetObjectType == null) {
				return 0;
			}
			CriteriaOperator expression = KpiHelper.GetExpressionOperator(Expression, rangeStart, rangeEnd);
			CriteriaOperator criteriaOperator = GetCriteriaOperator(rangeStart, rangeEnd);
			if(!object.ReferenceEquals(additionalCriteria, null)) {
				criteriaOperator = new GroupOperator(criteriaOperator, additionalCriteria);
			}
			Type objectType = TargetObjectType;
			if(TargetObjectType.IsInterface) {
				objectType = PersistentInterfaceHelper.GetPersistentInterfaceDataType(TargetObjectType);
			}
			float result = Convert.ToSingle(Session.Evaluate(objectType, expression, criteriaOperator));
			return result;
		}
		[RuleRequiredField("KpiDefinitionXpo.Name", DefaultContexts.Save)]
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public bool Active {
			get { return active; }
			set { SetPropertyValue("Active", ref active, value); }
		}
		[RuleRequiredField("KpiDefinitionXpo.TargetObjectType", DefaultContexts.Save)]
		[ImmediatePostData]
		public Type TargetObjectType {
			get { return ReflectionHelper.FindType(targetObjectType); }
			set {
				string typeFullName = value != null ? value.FullName : null;
				if(targetObjectType != typeFullName) {
					criteria = string.Empty;
					expression = "Count";
					SetPropertyValue("TargetObjectType", ref targetObjectType, typeFullName);
					OnChanged("Criteria");
					OnChanged("Expression");
					OnChanged("Current");
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
		[Size(SizeAttribute.Unlimited)]
		public string Criteria {
			get { return criteria; }
			set {
				SetPropertyValue("Criteria", ref criteria, value);
				OnChanged("Current");
				OnChanged("Objects");
			}
		}
		[ElementTypeProperty("TargetObjectType")]
		[Size(SizeAttribute.Unlimited)]
		[Appearance("KpiDefinitionXpoExpression", Enabled = false, Criteria = "TargetObjectType is null")]
		public string Expression {
			get { return expression; }
			set { SetPropertyValue("Expression", ref expression, value); }
		}
		public float GreenZone {
			get { return greenZone; }
			set { SetPropertyValue("GreenZone", ref greenZone, value); }
		}
		public float RedZone {
			get { return redZone; }
			set { SetPropertyValue("RedZone", ref redZone, value); }
		}
		[Browsable(false)]
		public IEnumerable<IDateRange> AvailableRanges {
			get { return DateRangeRepository.GetRegisteredRanges(); }
		}
		[DataSourceProperty("AvailableRanges")]
		[ValueConverter(typeof(IDateRangeToStringConverter))]
		[RuleRequiredField("KpiDefinitionXpo.Range", DefaultContexts.Save)]
		public IDateRange Range {
			get { return GetPropertyValue<IDateRange>("Range"); }
			set { SetPropertyValue<IDateRange>("Range", value); }
		}
		[ImmediatePostData]
		public bool Compare {
			get { return compare; }
			set { SetPropertyValue("Compare", ref compare, value); }
		}
		[Appearance("KpiDefinitionXpoRangeToCompare", Enabled = false, Criteria = "Compare = False")]
		[DataSourceProperty("AvailableRanges")]
		[ValueConverter(typeof(IDateRangeToStringConverter))]
		public IDateRange RangeToCompare {
			get {
				if(!Compare) {
					return null;
				}
				return GetPropertyValue<IDateRange>("RangeToCompare");
			}
			set { SetPropertyValue<IDateRange>("RangeToCompare", value); }
		}
		public TimeIntervalType MeasurementFrequency {
			get { return measurementFrequency; }
			set { SetPropertyValue("MeasurementFrequency", ref measurementFrequency, value); }
		}
		[Browsable(false)]
		public MeasurementMode MeasurementMode {
			get { return measurementMode; }
			set { SetPropertyValue("MeasurementMode", ref measurementMode, value); }
		}
		public Direction Direction {
			get { return direction; }
			set { SetPropertyValue("Direction", ref direction, value); }
		}
		[Browsable(false)]
		public string DrillDownListViewId {
			get {
				string kpiName = string.IsNullOrEmpty(Name) ? string.Empty : Name;
				return kpiName.Replace(' ', '_') + "_" + TargetObjectType.Name + "_Drilldown_ListView";
			}
		}
		[VisibleInDetailView(false)]
		public string Period {
			get {
				if(Range != null) {
					string rangeString = Range.Caption;
					return (Compare && RangeToCompare != null) ? rangeString + " " + CaptionHelper.GetLocalizedText("Texts", "VersusText") + " " + RangeToCompare.Caption : rangeString;
				}
				return string.Empty;
			}
		}
		[Browsable(false)]
		public DateTime ChangedOn {
			get { return changedOn; }
			set { SetPropertyValue<DateTime>("ChangedOn", ref changedOn, value); }
		}
		public CollectionSource GetDrilldownCollectionSource(IObjectSpace os) {
			if(TargetObjectType == null || Range == null) {
				return null;
			}
			CollectionSource collectionSource = new CollectionSource(os, TargetObjectType, false, CollectionSourceMode.Proxy);
			collectionSource.Criteria.Add("KPI Drilldown", GetCriteriaOperator(Range));
			return collectionSource;
		}
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public float Current {
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
				if(new IsValidKpiCriteriaCodeRule().Validate(this).State == ValidationState.Valid) {
					CollectionSource collectionSource = GetDrilldownCollectionSource(XPObjectSpace.FindObjectSpaceByObject(this));
					return collectionSource != null ? (IList)collectionSource.Collection : null;
				} else {
					return null;
				}
			}
		}
		[Browsable(false), MemberDesignTimeVisibility(false)]
		public KpiInstance KpiInstance {
			get { return kpiInstance; }
		}
		protected void SetKpiInstance(KpiInstance kpiInstance) {
			Guard.ArgumentNotNull(kpiInstance, "kpiInstance");
			this.kpiInstance.Delete();
			this.kpiInstance = kpiInstance;
		}
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public string SuppressedSeries {
			get { return suppressedSeries; }
			set { SetPropertyValue("SuppressedSeries", ref suppressedSeries, value); }
		}
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public bool EnableCustomizeRepresentation {
			get { return enableCustomizeRepresentation; }
			set { enableCustomizeRepresentation = value; }
		}
	}
}
