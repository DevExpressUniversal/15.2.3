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
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.DashboardWin.Native {
	public interface IFormatRuleBaseControlView {
		event ViewStateChangedEventHandler StateUpdated;
		bool IsValid { get; }
		void Initialize(IFormatRuleBaseViewInitializationContext initializationContext);
	}
	public interface IFormatRuleManagerView : IFormatRuleBaseControlView {
		event FormatRuleEditingEventHandler Editing;
		event FormatRuleMovingEventHandler Moving;
		event FormatRuleDeletingEventHandler Deleting;
		event FormatRuleEnablingEventHandler Enabling;
		event EventHandler FilterDataItemChanged;
		event EventHandler CalculatedByDataItemChanged;
		void SetRules(IList<IFormatRuleView> rules);
		void SetFilterDataItems(IEnumerable items);
		void SetCalculatedByDataItems(IEnumerable items);
		void SetPopupMenuItems(IList<BarItem> items, bool addSeparator);
		void ClearPopupMenuItems();
		int SelectedFilterDataItemIndex { get; set; }
		int SelectedCalculatedByDataItemIndex { get; set; }
	}
	public interface IFormatRuleControlView : IFormatRuleBaseControlView {
		event EventHandler ItemApplyToChanged;
		void SetItemsApplyTo(IEnumerable items);
		bool? ApplyToRow { get; set; }
		bool? ApplyToColumn { get; set; }
		int SelectedItemApplyToIndex { get; set; }
		IFormatRuleIntersectionLevel IntersectionLevel { get; }
	}
	public interface IFormatRuleControlRangeBaseView : IFormatRuleControlView {
		event FormatRuleRangeViewChangingEventHandler RangeViewChanged;
		event EventHandler UsePercentChanged;
		bool IsPercent { get; set; }
		IList<IFormatRuleRangeView> Ranges { get; set; }
	}
	public interface IFormatRuleControlBarOptionsView {
		IFormatRuleControlBarOptions BarOptions { get; }
	}
	public interface IFormatRuleControlBarOptions {
		event EventHandler OptionsChanged;
		bool DrawAxis { get; }
		bool AllowNegativeAxis { get; }
		bool ShowBarOnly { get; }
	}
	public interface IFormatRuleControlColorRangeBarView : IFormatRuleControlRangeSetView, IFormatRuleControlBarOptionsView { 
	}
	public interface IFormatRuleControlRangeSetView : IFormatRuleControlRangeBaseView {
		event FormatRuleRangeViewCreatingEventHandler RangeViewCreating;
		event FormatRuleRangeSetPredefinedStyleChangedEventHandler PredefinedStyleChanged;
		FormatConditionRangeSetPredefinedType PredefinedType { get; set; }
	}
	public interface IFormatRuleControlRangeGradientView : IFormatRuleControlRangeBaseView {
		event FormatRuleRangeGradientViewGeneratingEventHandler RangeGradientViewGenerating;
	}
	public interface IFormatRuleControlRangeView : IFormatRuleControlView {
		bool IsPercent { get; set; }
		IEnumerable<object> Values { get; set; }
		IEnumerable<StyleSettingsContainer> Styles { get; set; }
	}
	public interface IFormatRuleControlStyleView : IFormatRuleControlView {
		StyleSettingsContainer Style { get; set; }
	}
	public interface IFormatRuleControlBarView : IFormatRuleControlStyleView, IFormatRuleControlBarOptionsView {
		StyleSettingsContainer NegativeStyle { get; set; }
		DashboardFormatConditionValueType MinimumType { get; set; }
		DashboardFormatConditionValueType MaximumType { get; set; }
		decimal Minimum { get; set; }
		decimal Maximum { get; set; }
	}
	public interface IFormatRuleControlValueView : IFormatRuleControlStyleView {
		object Value { get; set; }
		object Value2 { get; set; }
	}
	public interface IFormatRuleControlTopBottomView : IFormatRuleControlStyleView {
		bool IsPercent { get; set; }
		decimal Rank { get; set; }
	}
	public interface IFormatRuleControlExpressionView : IFormatRuleControlStyleView {
		event EventHandler<ShowValueEditorEventArgs> FilterEditorControlInitialize;
		string Expression { get; set; }
	}
	public interface IFormatRuleControlDateOccurringView : IFormatRuleControlStyleView {
		DevExpress.XtraEditors.FilterDateType DateType { get; set; }
	}
	public interface IFormatRuleIntersectionLevel {
		event EventHandler ModeChanged;
		int SelectedModeIndex { get; set; }
		int SelectedAxis1ItemIndex { get; set; }
		int SelectedAxis2ItemIndex { get; set; }
		void SetModes(IEnumerable modes);
		void SetAxis1Items(IEnumerable items);
		void SetAxis2Items(IEnumerable items);
		void Enable(bool enabled);
	}
	public interface IFormatRuleView {
		object Rule { get; }
		bool IsValid { get; }
		bool Enabled { get; set; }
		string Caption { get; }
		string DataItemCaption { get; }
		string DataItemApplyToCaption { get; }
	}
	public interface IFormatRuleRangeView {
		StyleSettingsContainer Style { get; set; }
		ComparisonTypeItem ComparisonTypeItem { get; set; }
		object LeftValue { get; set; }
		object RightValue { get; set; }
	}
	public interface IFormatRuleControlChanger {
		event EventHandler Changed;
		event EventHandler Created;
		event EventHandler Destroyed;
		void Enable(bool? enabled);
		void Refresh(string title);
	}
}
