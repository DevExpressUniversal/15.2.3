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
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleValueControlPresenter : FormatRuleControlPresenter {
		protected override string Caption {
			get { return SpecificCondition.Condition.Localize(); }
		}
		protected override string DescriptionFormat {
			get { return SpecificCondition.Condition.Descript(); }
		}
		IFormatRuleControlValueView SpecificView { get { return (IFormatRuleControlValueView)base.View; } }
		FormatConditionValue SpecificCondition {
			get { return (FormatConditionValue)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		public FormatRuleValueControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleValueControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardFormatCondition type)
			: base(serviceProvider, dashboardItem, dataItem, null) {
			SpecificCondition = new FormatConditionValue() { Condition = type };
		}
		protected override FormatRuleControlViewInitializationContext CreateViewInitializationContext() {
			DashboardFormatCondition type = SpecificCondition.Condition;
			Dimension dimension = DataItem as Dimension;
			return new FormatRuleControlViewValueContext() {
				IsValue2Required = type == DashboardFormatCondition.Between || type == DashboardFormatCondition.NotBetween || 
					type == DashboardFormatCondition.BetweenOrEqual || type == DashboardFormatCondition.NotBetweenOrEqual,
				DataType = DataItem.ActualDataFieldType,
				DateTimeGroupInterval = dimension != null ? dimension.DateTimeGroupInterval : DateTimeGroupInterval.None
			};
		}
		protected override bool? InitializeView() {
			base.InitializeView();
			FormatConditionValue condition = SpecificCondition;
			SpecificView.Value = condition.Value1;
			SpecificView.Value2 = condition.Value2;
			SpecificView.Style = StyleSettingsContainer.ToStyleContainer(SpecificCondition.StyleSettings);
			return null;
		}
		protected override void ApplyView() {
			base.ApplyView();
			FormatConditionValue condition = SpecificCondition;
			condition.Value1 = SpecificView.Value;
			condition.Value2 = SpecificView.Value2;
			condition.StyleSettings = SpecificView.Style.ToCoreStyle();
		}
	}
}
