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
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleBarControlPresenter : FormatRuleControlPresenter {
		protected override string Caption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleBar); }
		}
		protected override string DescriptionFormat {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleBarDescription); }
		}
		protected override bool IsApplyToReadOnly { get { return true; } }
		protected override bool IsApplyToColumnSupported { get { return false; } }
		IFormatRuleControlBarView SpecificView { get { return (IFormatRuleControlBarView)base.View; } }
		FormatConditionBar SpecificCondition {
			get { return (FormatConditionBar)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		FormatConditionBarOptions BarOptions { get { return SpecificCondition.BarOptions; } }
		public FormatRuleBarControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleBarControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem)
			: this(serviceProvider, dashboardItem, dataItem, null) {
				SpecificCondition = new FormatConditionBar();
		}
		protected override FormatRuleControlViewInitializationContext CreateViewInitializationContext() {
			Dimension dimension = DataItem as Dimension;
			FormatRuleViewBarContext context = new FormatRuleViewBarContext();
			context.DataType = DataItem.ActualDataFieldType;
			context.DateTimeGroupInterval = dimension != null ? dimension.DateTimeGroupInterval : DateTimeGroupInterval.None;
			IFormatRuleBarOptionsInitializationContext contextBarOptions = context.BarOptions;
			FormatConditionBarOptions barOptions = SpecificCondition.BarOptions;
			contextBarOptions.AllowNegativeAxis = barOptions.AllowNegativeAxis;
			contextBarOptions.DrawAxis = barOptions.DrawAxis;
			contextBarOptions.ShowBarOnly = barOptions.ShowBarOnly;
			return context;
		}
		protected override bool? InitializeView() {
			bool? result = base.InitializeView();
			SpecificView.NegativeStyle = StyleSettingsContainer.ToStyleContainer(SpecificCondition.NegativeStyleSettings);
			SpecificView.Style = StyleSettingsContainer.ToStyleContainer(SpecificCondition.StyleSettings);
			SpecificView.Minimum = SpecificCondition.Minimum;
			SpecificView.Maximum = SpecificCondition.Maximum;
			SpecificView.MinimumType = SpecificCondition.MinimumType;
			SpecificView.MaximumType = SpecificCondition.MaximumType;
			return result;
		}
		protected override void ApplyView() {
			base.ApplyView();
			SpecificCondition.StyleSettings = (BarStyleSettings)SpecificView.Style.ToCoreStyle();
			SpecificCondition.NegativeStyleSettings = (BarStyleSettings)SpecificView.NegativeStyle.ToCoreStyle();
			SpecificCondition.BarOptions.AllowNegativeAxis = SpecificView.BarOptions.AllowNegativeAxis;
			SpecificCondition.BarOptions.DrawAxis = SpecificView.BarOptions.DrawAxis;
			SpecificCondition.BarOptions.ShowBarOnly = SpecificView.BarOptions.ShowBarOnly;
			SpecificCondition.Minimum = SpecificView.Minimum;
			SpecificCondition.Maximum = SpecificView.Maximum;
			SpecificCondition.MinimumType = SpecificView.MinimumType;
			SpecificCondition.MaximumType = SpecificView.MaximumType;
		}
	}
}
