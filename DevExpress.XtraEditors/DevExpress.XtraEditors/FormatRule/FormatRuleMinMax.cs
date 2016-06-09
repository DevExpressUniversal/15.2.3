#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraEditors {
	public class FormatConditionRuleMinMaxBase : FormatConditionRuleBase {
		FormatConditionValueType minimumType = FormatConditionValueType.Automatic;
		FormatConditionValueType maximumType = FormatConditionValueType.Automatic;
		decimal minimum;
		decimal maximum;
		[DefaultValue(FormatConditionValueType.Automatic)]
		[XtraSerializableProperty]
		public FormatConditionValueType MinimumType {
			get { return minimumType; }
			set {
				if(MinimumType == value) return;
				minimumType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		[DefaultValue(FormatConditionValueType.Automatic)]
		[XtraSerializableProperty]
		public FormatConditionValueType MaximumType { 
			get { return maximumType; }
			set {
				if(MaximumType == value) return;
				maximumType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		void ResetMinimum() { Minimum = 0; }
		bool ShouldSerializeMinimum() { return Minimum != 0; }
		[XtraSerializableProperty]
		public decimal Minimum {
			get { return minimum; }
			set {
				if(Minimum == value) return;
				minimum = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		void ResetMaximum() { Maximum = 0; }
		bool ShouldSerializeMaximum() { return Maximum != 0; } 
		[XtraSerializableProperty]
		public decimal Maximum {
			get { return maximum; }
			set {
				if(Maximum == value) return;
				maximum = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleMinMaxBase();
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleMinMaxBase;
			if(source == null) return;
			MinimumType = source.MinimumType;
			MaximumType = source.MaximumType;
			Minimum = source.Minimum;
			Maximum = source.Maximum;
		}
		protected bool GetMinMaxRange(IFormatConditionRuleValueProvider valueProvider, out decimal minimum, out decimal maximum) {
			maximum = minimum = 0;
			decimal? min = Minimum, max = Maximum;
			if(MinimumType != FormatConditionValueType.Number) min = ConvertToNumeric(valueProvider.GetQueryValue(this, FormatRuleValueQueryKind.Minimum));
			if(MaximumType != FormatConditionValueType.Number) max = ConvertToNumeric(valueProvider.GetQueryValue(this, FormatRuleValueQueryKind.Maximum));
			if(min == null || max == null) return false;
			if(MinimumType == FormatConditionValueType.Percent) min = GetPercentValue(min.Value, max.Value, Minimum);
			if(MaximumType == FormatConditionValueType.Percent) max = GetPercentValue(min.Value, max.Value, Maximum);
			minimum = min.Value;
			maximum = max.Value;
			return true;
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			decimal? value = CheckQueryNumericValue(valueProvider);
			if(value == null) return false;
			decimal min, max;
			if(!GetMinMaxRange(valueProvider, out min, out max)) return false;
			if(value >= min && value <= max) return true;
			return IsAllowOutOfBoundsValue();
		}
		protected virtual bool IsAllowOutOfBoundsValue() {
			return false;
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			var kind = FormatRuleValueQueryKind.None;
			if(MinimumType != FormatConditionValueType.Number) kind |= FormatRuleValueQueryKind.Minimum;
			if(MaximumType != FormatConditionValueType.Number) kind |= FormatRuleValueQueryKind.Maximum;
			if(MinimumType == FormatConditionValueType.Percent || MaximumType == FormatConditionValueType.Percent)
				kind = FormatRuleValueQueryKind.Maximum | FormatRuleValueQueryKind.Minimum;
			if(MinimumType == FormatConditionValueType.Number && MaximumType == FormatConditionValueType.Number) kind = FormatRuleValueQueryKind.None;
			return new FormatConditionRuleState(this, kind);
		}
		protected override void DrawPreviewCore(Utils.Drawing.GraphicsCache cache, FormatConditionDrawPreviewArgs e) {
			throw new NotImplementedException();
		}
	}
}
