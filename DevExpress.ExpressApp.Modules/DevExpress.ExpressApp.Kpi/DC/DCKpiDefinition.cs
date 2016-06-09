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
using System.Diagnostics.CodeAnalysis;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Kpi;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.ExpressApp.Kpi {
	[DomainComponent]
	[NavigationItem(KpiModule.KpiNavigationGroupName)]
	[XafDisplayName("Definition")]
	[ImageName("BO_KPI_Definition")]
	public interface IDCKpiDefinition : IKpiDefinition {
		[Browsable(false), MemberDesignTimeVisibility(false)]
		IDCKpiInstance KpiInstance { get; set; }
		[RuleRequiredField("KpiDefinitionDC.Name", DefaultContexts.Save)]
		new string Name { get; set; }
		[DefaultValue(true)]
		new bool Active { get; set; }
		[Browsable(false)]
		IEnumerable<IFilterParameter> CriteriaParameters { get; }
		[RuleRequiredField("KpiDefinitionDC.TargetObjectType", DefaultContexts.Save)]
		[ValueConverter(typeof(TypeToStringConverter))]
		[ImmediatePostData]
		new Type TargetObjectType { get; set; }
		[CriteriaOptions("TargetObjectType", "CriteriaParameters")]
		[Size(SizeAttribute.Unlimited)]
		new string Criteria { get; set; }
		[ElementTypeProperty("TargetObjectType")]
		[Size(SizeAttribute.Unlimited)]
		[Appearance("KpiDefinitionExpression", Enabled = false, Criteria = "TargetObjectType is null")]
		new string Expression { get; set; }
		new float GreenZone { get; set; }
		new float RedZone { get; set; }
		[DataSourceProperty("AvailableRanges")]
		[ValueConverter(typeof(IDateRangeToStringConverter))]
		new IDateRange Range { get; set; }
		[ImmediatePostData]
		new bool Compare { get; set; }
		[Appearance("KpiDefinitionRangeToCompare", Enabled = false, Criteria = "Compare = False")]
		[DataSourceProperty("AvailableRanges")]
		[ValueConverter(typeof(IDateRangeToStringConverter))]
		new IDateRange RangeToCompare { get; set; }
		new TimeIntervalType MeasurementFrequency { get; set; }
		[Browsable(false)]
		[DefaultValue(MeasurementMode.Interval)]
		new MeasurementMode MeasurementMode { get; set; }
		new Direction Direction { get; set; }
		[Browsable(false)]
		new DateTime ChangedOn { get; set; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		[DefaultValue("Previous")]
		new string SuppressedSeries { get; set; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		[DefaultValue(true)]
		new bool EnableCustomizeRepresentation { get; set; }
	}
	[DomainLogic(typeof(IDCKpiDefinition))]
	public class KpiDefinitionLogic {
		private List<IFilterParameter> criteriaParameters;
		private static CriteriaOperator GetCriteriaOperator(IDCKpiDefinition entity, IDateRange range) {
			return range != null ? GetCriteriaOperator(entity, entity.Range.StartPoint, entity.Range.EndPoint) : null;
		}
		public static CriteriaOperator GetCriteriaOperator(IDCKpiDefinition entity, DateTime rangeStart, DateTime rangeEnd) {
			return KpiHelper.GetCriteriaOperator(XPObjectSpace.FindObjectSpaceByObject(entity), entity.Criteria, entity.TargetObjectType, rangeStart, rangeEnd);
		}
		public static float Evaluate(IDCKpiDefinition entity, DateTime rangeStart, DateTime rangeEnd) {
			if(entity.TargetObjectType == null) {
				return 0;
			}
			XPObjectSpace os = (XPObjectSpace)XPObjectSpace.FindObjectSpaceByObject(entity);
			CriteriaOperator expression = KpiHelper.GetExpressionOperator(entity.Expression, rangeStart, rangeEnd);
			float result = Convert.ToSingle(os.Session.Evaluate(entity.TargetObjectType, expression, GetCriteriaOperator(entity, rangeStart, rangeEnd)));
			return result;
		}
		public void AfterChange_Criteria(IDCKpiDefinition entity) {
			UpdatePreview(entity);
		}
		public void AfterChange_Expression(IDCKpiDefinition entity) {
			UpdatePreview(entity);
		}
		public void AfterChange_TargetObjectType(IDCKpiDefinition entity) {
			entity.Criteria = string.Empty;
			entity.Expression = "Count";
			UpdatePreview(entity);
		}
		public void AfterChange_MeasurementFrequency(IDCKpiDefinition entity) {
			UpdatePreview(entity);
		}
		public void AfterChange_Range(IDCKpiDefinition entity) {
			UpdatePreview(entity);
		}
		public void AfterChange_RangeToCompare(IDCKpiDefinition entity) {
			UpdatePreview(entity);
		}
		public void AfterChange_Compare(IDCKpiDefinition entity) {
			if(!entity.Compare) {
				entity.RangeToCompare = null; ;
			}
			UpdatePreview(entity);
		}
		private void UpdatePreview(IDCKpiDefinition entity) {
			entity.ChangedOn = DateTime.Now;
		}
		static KpiDefinitionLogic() {
			KpiDefinition.RegisterParameters();
		}
		public static void AfterConstruction(IDCKpiDefinition entry) {
			IDCKpiInstance kpiInstance = GetObjectSpace(entry).CreateObject<IDCKpiInstance>();
			kpiInstance.KpiDefinition = entry;
			entry.KpiInstance = kpiInstance;
			if(string.IsNullOrEmpty(entry.Expression)) {
				entry.Expression = "Count";
			}
			if(entry.Range == null) {
				entry.Range = DateRangeRepository.FindRange("Now");
			}
			entry.MeasurementFrequency = TimeIntervalType.Day;
			entry.MeasurementMode = MeasurementMode.Interval;
			entry.Direction = Direction.HighIsBetter;
		}
		public static void OnDeleted(IDCKpiDefinition entry) {
			GetObjectSpace(entry).Delete(entry.KpiInstance);
		}
		private static IObjectSpace GetObjectSpace(IKpiDefinition entity) {
			return XPObjectSpace.FindObjectSpaceByObject(entity);
		}
		public static IEnumerable<IDateRange> Get_AvailableRanges() {
			return DateRangeRepository.GetRegisteredRanges();
		}
		public static string Get_DrillDownListViewId(IDCKpiDefinition entity) {
			string kpiName = string.IsNullOrEmpty(entity.Name) ? string.Empty : entity.Name;
			return string.Format("{0}_{1}_Drilldown_ListView", kpiName.Replace(' ', '_'), entity.TargetObjectType.Name);
		}
		public static string Get_Period(IDCKpiDefinition entity) {
			if(entity.Range != null) {
				string rangeString = entity.Range.Caption;
				return (entity.Compare && entity.RangeToCompare != null) ? rangeString + " " + CaptionHelper.GetLocalizedText("Texts", "VersusText") + " " + entity.RangeToCompare.Caption : rangeString;
			}
			return string.Empty;
		}
		public static CollectionSource GetDrilldownCollectionSource(IDCKpiDefinition entity, IObjectSpace os) {
			if(entity.TargetObjectType == null || entity.Range == null) {
				return null;
			}
			CollectionSource collectionSource = new CollectionSource(os, entity.TargetObjectType, false, CollectionSourceMode.Proxy);
			collectionSource.Criteria.Add("KPI Drilldown", GetCriteriaOperator(entity, entity.Range));
			return collectionSource;
		}
		public IEnumerable<IFilterParameter> Get_CriteriaParameters() {
			if(criteriaParameters == null) {
				criteriaParameters = new List<IFilterParameter>();
				criteriaParameters.Add(new XafFilterParameter(KpiHelper.RangeStartParameterName, typeof(DateTime)));
				criteriaParameters.Add(new XafFilterParameter(KpiHelper.RangeEndParameterName, typeof(DateTime)));
			}
			return criteriaParameters;
		}
		public static float Get_Current(IDCKpiDefinition entity) {
			try {
				return entity.Evaluate(entity.Range.StartPoint, entity.Range.EndPoint);
			}
			catch(Exception) {
				return 0;
			}
		}
		public static IList Get_Objects(IDCKpiDefinition entity) {
			if(new IsValidKpiCriteriaCodeRule().Validate(entity).State == ValidationState.Valid) {
				CollectionSource collectionSource = entity.GetDrilldownCollectionSource(XPObjectSpace.FindObjectSpaceByObject(entity));
				return collectionSource != null ? (IList)collectionSource.Collection : null;
			}
			else {
				return null;
			}
		}
		#region Obsolete v15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		private static CriteriaWrapper GetCriteriaWrapper(IDCKpiDefinition entity, IDateRange range) {
			return range != null ? GetCriteriaWrapper(entity, entity.Range.StartPoint, entity.Range.EndPoint) : null;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static CriteriaWrapper GetCriteriaWrapper(IDCKpiDefinition entity, DateTime rangeStart, DateTime rangeEnd) {
			return KpiHelper.GetCriteriaWrapper(XPObjectSpace.FindObjectSpaceByObject(entity), entity.Criteria, entity.TargetObjectType, rangeStart, rangeEnd);
		}
		#endregion
		#region Obsolete 15.2
		private const string KpiNavigationGroupName = "KPI";
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static string Get_RangeStartParameterName(IKpiDefinition entity) {
			return KpiHelper.RangeStartParameterName;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static string Get_RangeEndParameterName(IKpiDefinition entity) {
			return KpiHelper.RangeEndParameterName;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static string Get_KpiNavigationGroupName(IKpiDefinition entity) {
			return KpiNavigationGroupName;
		}
		#endregion
	}
}
