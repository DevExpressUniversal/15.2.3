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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
namespace DevExpress.DashboardCommon {
	public class FormatConditionTopBottom : FormatConditionStyleBase {
		const string XmlRank = "Rank";
		const string XmlRankType = "RankType";
		const string XmlTopBottomType = "TopBottomType";
		const DashboardFormatConditionValueType DefaultRankType = DashboardFormatConditionValueType.Automatic;
		const DashboardFormatConditionTopBottomType DefaultTopBottomType = DashboardFormatConditionTopBottomType.Top;
		static decimal DefaultRank = 5M;
		DashboardFormatConditionTopBottomType topBottom = DefaultTopBottomType;
		DashboardFormatConditionValueType rankType = DefaultRankType;
		decimal rank = DefaultRank;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionTopBottomRank")
#else
	Description("")
#endif
		]
		public decimal Rank {
			get { return rank; }
			set {
				if(Rank != value) {
					rank = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionTopBottomRankType"),
#endif
		DefaultValue(DefaultRankType)
		]
		public DashboardFormatConditionValueType RankType {
			get { return rankType; }
			set {
				if(RankType != value) {
					rankType = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionTopBottomTopBottom"),
#endif
		DefaultValue(DefaultTopBottomType)
		]
		public DashboardFormatConditionTopBottomType TopBottom {
			get { return topBottom; }
			set {
				if(TopBottom != value) {
					topBottom = value;
					OnChanged();
				}
			}
		}
		public FormatConditionTopBottom() { 
		}
		public FormatConditionTopBottom(decimal rank) : this() {
			this.rank = rank;
		}
		public FormatConditionTopBottom(decimal rank, DashboardFormatConditionTopBottomType topBottom) : this(rank) {
			this.topBottom = topBottom;
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionTopBottom();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlTopBottomType, TopBottom, DefaultTopBottomType);
			XmlHelper.Save(element, XmlRankType, RankType, DefaultRankType);
			XmlHelper.Save(element, XmlRank, Rank, DefaultRank);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.LoadEnum<DashboardFormatConditionTopBottomType>(element, XmlTopBottomType, x => topBottom = x);
			XmlHelper.LoadEnum<DashboardFormatConditionValueType>(element, XmlRankType, x => rankType = x);
			XmlHelper.Load<decimal>(element, XmlRank, x => rank = x);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionTopBottom;
			if(source != null) {
				TopBottom = source.TopBottom;
				RankType = source.RankType;
				Rank = source.Rank;
			}
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			object val = valueProvider.GetValue(this);
			if(!CheckValue(val)) return false;
			object numeric = ValueManager.ConvertToNumber(val);
			if(numeric == null) return false;
			IList list = GetAggregationValue() as IList;
			if(list == null || list.Count == 0) return false;
			int count = ActualRankType == DashboardFormatConditionValueType.Number ? (int)Rank : 1;
			if(count == 0) return false;
			if(count > list.Count) return true;
			int targetIndex = list.Count - 1;
			object targetValue = list[targetIndex];
			if(!CheckValue(targetValue)) return false;
			object targetNumeric = ValueManager.ConvertToNumber(targetValue);
			if(targetNumeric != null) {
				int res = ValueManager.CompareValues(numeric, targetNumeric, true);
				return (TopBottom == DashboardFormatConditionTopBottomType.Bottom) ? res <= 0 : res >= 0;
			} else {
				return false;
			}
		}
		protected DashboardFormatConditionValueType ActualRankType {
			get {
				DashboardFormatConditionValueType rt = RankType;
				return (rt == DashboardFormatConditionValueType.Automatic) ? DashboardFormatConditionValueType.Number : rt;
			}
		}
		protected override IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			return new SummaryItemTypeEx[] { TopBottom.ToSummaryItemType(ActualRankType) };
		}
		protected override decimal GetAggregationArgument() {
			return Rank;
		}
		void ResetRank() { Rank = DefaultRank; }
		bool ShouldSerializeRank() { return Rank != DefaultRank; }
	}
}
