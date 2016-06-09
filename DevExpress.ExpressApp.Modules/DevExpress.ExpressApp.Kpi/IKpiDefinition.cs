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
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Kpi {
	public enum Direction { LowIsBetter, HighIsBetter }
	public enum TimeIntervalType { Hour, Day, Week, Month, Year, Quarter }
	public enum MeasurementMode { Interval, RunningTotal }
	[NavigationItem(KpiModule.KpiNavigationGroupName)]
	[ImageName("BO_KPI_Definition")]
	public interface IKpiDefinition {
		float Evaluate(DateTime rangeStart, DateTime rangeEnd);
		[RuleRequiredField("KpiDefinition.Name", DefaultContexts.Save)]
		string Name { get; set; }
		[DefaultValue(true)]
		bool Active { get; set; }
		[RuleRequiredField("KpiDefinition.TargetObjectType", DefaultContexts.Save)]
		[ImmediatePostData]
		Type TargetObjectType { get; set; }
		[CriteriaOptions("TargetObjectType")]
		string Criteria { get; set; }
		string Expression { get; set; }
		float GreenZone { get; set; }
		float RedZone { get; set; }
		[Browsable(false)]
		IEnumerable<IDateRange> AvailableRanges { get; }
		[DataSourceProperty("AvailableRanges")]
		[RuleRequiredField("KpiDefinition.Range", DefaultContexts.Save)]
		IDateRange Range { get; set; }
		bool Compare { get; set; }
		[Appearance("KpiDefinitionRangeToCompare", Enabled = false, Criteria = "Compare = False")]
		[DataSourceProperty("AvailableRanges")]
		IDateRange RangeToCompare { get; set; }
		TimeIntervalType MeasurementFrequency { get; set; }
		[Browsable(false)]
		[DefaultValue(MeasurementMode.Interval)]
		MeasurementMode MeasurementMode { get; set; }
		Direction Direction { get; set; }
		[Browsable(false)]
		string DrillDownListViewId { get; }
		[VisibleInDetailView(false)]
		string Period {get;}
		[Browsable(false)]
		DateTime ChangedOn { get; set; }
		CollectionSource GetDrilldownCollectionSource(IObjectSpace os);
		[VisibleInListView(false), VisibleInLookupListView(false)]
		float Current { get; }
		[ElementTypeProperty("TargetObjectType")]
		IList Objects { get; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		[DefaultValue("Previous")]
		string SuppressedSeries { get; set; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		[DefaultValue(true)]
		bool EnableCustomizeRepresentation { get; set; }
	}
	[CodeRule]
	public class IsValidKpiCriteriaCodeRule : RuleBase<IKpiDefinition> {
		protected override bool IsValidInternal(IKpiDefinition target, out string errorMessageTemplate) {
			try {
				Criteria = target.Criteria;
				target.Evaluate(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(-1));
			}
			catch(Exception e) {
				errorMessageTemplate = e.Message.Replace("{", "/").Replace("}", "/");
				return false;
			}
			errorMessageTemplate = "";
			return true;
		}
		public string Criteria { get; set; }
		public IsValidKpiCriteriaCodeRule()
			: base("", "Save") {
			Properties.SkipNullOrEmptyValues = true;
		}
		public IsValidKpiCriteriaCodeRule(IRuleBaseProperties properties)
			: base(properties) {
		}
	}
}
