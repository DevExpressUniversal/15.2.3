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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public delegate void ViewStateChangedEventHandler(object sender, ViewStateChangedEventArgs e);
	public class ViewStateChangedEventArgs : EventArgs {
		public ViewStateChangedEventArgs() { }
	}
	public delegate Form FormatRuleEditingEventHandler(object sender, FormatRuleEditingEventArgs e);
	public class FormatRuleEditingEventArgs : EventArgs {
		readonly IFormatRuleView ruleView;
		public IFormatRuleView RuleView { get { return ruleView; } }
		public FormatRuleEditingEventArgs(IFormatRuleView ruleView) {
			this.ruleView = ruleView;
		}
	}
	public delegate void FormatRuleMovingEventHandler(object sender, FormatRuleMovingEventArgs e);
	public class FormatRuleMovingEventArgs : EventArgs {
		readonly IFormatRuleView oldRuleView;
		readonly IFormatRuleView newRuleView;
		public IFormatRuleView OldRuleView { get { return oldRuleView; } }
		public IFormatRuleView NewRuleView { get { return newRuleView; } }
		public FormatRuleMovingEventArgs(IFormatRuleView oldRuleView, IFormatRuleView newRuleView) {
			this.oldRuleView = oldRuleView;
			this.newRuleView = newRuleView;
		}
	}
	public delegate void FormatRuleDeletingEventHandler(object sender, FormatRuleDeletingEventArgs e);
	public class FormatRuleDeletingEventArgs : EventArgs {
		readonly IFormatRuleView ruleView;
		public IFormatRuleView RuleView { get { return ruleView; } }
		public FormatRuleDeletingEventArgs(IFormatRuleView ruleView) {
			this.ruleView = ruleView;
		}
	}
	public delegate void FormatRuleEnablingEventHandler(object sender, FormatRuleEnablingEventArgs e);
	public class FormatRuleEnablingEventArgs : EventArgs {
		readonly IFormatRuleView ruleView;
		readonly bool enabled;
		public IFormatRuleView RuleView { get { return ruleView; } }
		public bool Enabled { get { return enabled; } }
		public FormatRuleEnablingEventArgs(IFormatRuleView ruleView, bool enabled) {
			this.ruleView = ruleView;
			this.enabled = enabled;
		}
	}
	public delegate IFormatRuleRangeView FormatRuleRangeViewCreatingEventHandler(object sender, EventArgs e);
	public delegate void FormatRuleRangeSetPredefinedStyleChangedEventHandler(object sender, FormatRuleRangeSetPredefinedStyleChangedEventArgs e);
	public class FormatRuleRangeSetPredefinedStyleChangedEventArgs : EventArgs {
		public FormatConditionRangeSetPredefinedType PredefinedStyleType { get; private set; }
		public FormatRuleRangeSetPredefinedStyleChangedEventArgs(FormatConditionRangeSetPredefinedType predefinedStyleType) {
			PredefinedStyleType = predefinedStyleType;
		}
	}
	public delegate void FormatRuleRangeGradientViewGeneratingEventHandler(object sender, FormatRuleRangeGradientViewGeneratingEventArgs e);
	public class FormatRuleRangeGradientViewGeneratingEventArgs : EventArgs {
		public int Count { get; private set; }
		public FormatRuleRangeGradientViewGeneratingEventArgs(int count) {
			Count = count;
		}
	}
	public delegate void FormatRuleRangeViewChangingEventHandler(object sender, FormatRuleRangeViewChangingEventArgs e);
	public class FormatRuleRangeViewChangingEventArgs : EventArgs {
		public int Index { get; private set; }
		public StyleSettingsContainer Style { get; private set; }
		public object Value { get; private set; }
		public DashboardFormatConditionComparisonType? ValueComparison { get; private set; }
		public FormatRuleRangeViewChangingEventArgs(int index, object value)
			: this(index) {
			Value = value;
		}
		public FormatRuleRangeViewChangingEventArgs(int index, DashboardFormatConditionComparisonType valueComparison)
			: this(index) {
			ValueComparison = valueComparison;
		}
		public FormatRuleRangeViewChangingEventArgs(int index, StyleSettingsContainer style)
			: this(index) {
			Style = style;
		}
		protected FormatRuleRangeViewChangingEventArgs(int index) {
			Index = index;
		}
	}
}
