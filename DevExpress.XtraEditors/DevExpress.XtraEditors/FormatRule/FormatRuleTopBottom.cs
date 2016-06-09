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
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.XtraEditors {
	public class FormatConditionRuleTopBottom : FormatConditionRuleAppearanceBase, IFormatConditionRuleTopBottom {
		FormatConditionValueType rankType = FormatConditionValueType.Automatic;
		decimal rank;
		FormatConditionTopBottomType topBottom = FormatConditionTopBottomType.Top;
		[DefaultValue(FormatConditionValueType.Automatic)]
		[XtraSerializableProperty]
		public FormatConditionValueType RankType {
			get { return rankType; }
			set {
				if(RankType == value) return;
				rankType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		void ResetRank() { Rank = 0; }
		bool ShouldSerializeRank() { return Rank != 0; }
		[XtraSerializableProperty]
		public decimal Rank {
			get { return rank; }
			set {
				if(Rank == value) return;
				rank = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		[DefaultValue(FormatConditionTopBottomType.Top)]
		[XtraSerializableProperty]
		public FormatConditionTopBottomType TopBottom {
			get { return topBottom; }
			set {
				if(TopBottom == value) return;
				topBottom = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleTopBottom();
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleTopBottom;
			if(source == null) return;
			RankType = source.RankType;
			Rank = source.Rank;
			TopBottom = source.TopBottom;
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			object val;
			if(!CheckQueryValue(valueProvider, out val)) return false;
			decimal? numeric = ConvertToNumeric(val);
			if(numeric == null) return false;
			IList list = valueProvider.GetQueryValue(this) as IList;
			if(list == null || list.Count == 0) return false;
			int count = ResolveRankType() == FormatConditionValueType.Number ? (int)Rank : 1;
			if(count == 0) return false;
			if(count > list.Count) return true;
			int targetIndex = list.Count - 1;
			object targetValue;
			if(!CheckQueryValue(list[targetIndex], out targetValue)) return false;
			decimal? targetNumeric = ConvertToNumeric(targetValue);
			if(targetNumeric == null) return false;
			int res = decimal.Compare(numeric.Value, targetNumeric.Value);
			if(TopBottom == FormatConditionTopBottomType.Bottom) return res <= 0;
			return res >= 0;
		}
		int GetCount(IList list, FormatConditionValueType rankType, decimal rank) {
			if(rank <= 0) return 0;
			if(rankType == FormatConditionValueType.Number) return (int)rank;
			int itemCount = (int)((((decimal)list.Count) / 100) * rank);
			return itemCount;
		}
		FormatConditionValueType ResolveRankType() {
			var rt = RankType;
			if(rt == FormatConditionValueType.Automatic) rt = FormatConditionValueType.Number;
			return rt;
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			FormatRuleValueQueryKind kind = FormatRuleValueQueryKind.None;
			switch(ResolveRankType()) {
				case FormatConditionValueType.Number:
					kind = TopBottom == FormatConditionTopBottomType.Top ? FormatRuleValueQueryKind.Top : FormatRuleValueQueryKind.Bottom;
					break;
				case FormatConditionValueType.Percent:
					kind = TopBottom == FormatConditionTopBottomType.Top ? FormatRuleValueQueryKind.TopPercent : FormatRuleValueQueryKind.BottomPercent;
					break;
				default:
					throw new InvalidEnumArgumentException("RankType", (int)ResolveRankType(), typeof(FormatConditionValueType));
			}
			var res = new FormatConditionRuleState(this, kind);
			res.CountPercent = Rank;
			return res;
		}
		#region IFormatConditionRuleTopBottom
		XlDifferentialFormatting IFormatConditionRuleTopBottom.Appearance {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		}
		bool IFormatConditionRuleTopBottom.Bottom {
			get { return TopBottom == FormatConditionTopBottomType.Bottom; }
		}
		bool IFormatConditionRuleTopBottom.Percent {
			get { return RankType == FormatConditionValueType.Percent; }
		}
		int IFormatConditionRuleTopBottom.Rank {
			get { return (int)Rank; }
		}
		#endregion
	}
}
