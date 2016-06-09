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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public abstract class FormatConditionRangeBase : FormatConditionBase {
		const string XmlRangeSet = "RangeSet";
		const string XmlValueType = "ValueType";
		const DashboardFormatConditionValueType DefaultValueType = DashboardFormatConditionValueType.Automatic;
		readonly RangeSet rangeSet = new RangeSet();
		DashboardFormatConditionValueType valueType = DashboardFormatConditionValueType.Automatic;
		[
		DefaultValue(DefaultValueType)
		]
		public virtual DashboardFormatConditionValueType ValueType {
			get { return valueType; }
			set {
				if(ValueType != value) {
					valueType = value;
					OnChanged();
				}
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RangeSet RangeSet { get { return rangeSet; } }
		internal RangeSet SortedRanges { get { return rangeSet.SortRanges(); } }
		public override bool IsValid {
			get {
				foreach(RangeInfo range in RangeSet)
					if(range.StyleSettings == null)
						return false;
				return base.IsValid && (RangeSet.Count >= 2);
			} 
		}
		protected internal bool HasPercents { get { return RangeSet.HasRanges && ValueType == DashboardFormatConditionValueType.Percent; } }
		protected override IEnumerable<StyleSettingsBase> Styles { get { return IsValid ? SortedRanges.ActualStyles : new StyleSettingsBase[] { }; } }
		protected FormatConditionRangeBase() {
			rangeSet.CollectionChanged += OnRangeSetCollectionChanged;
		}
		public void SetValues(ICollection values) {
			if(values.Count > RangeSet.Count)
				throw new ArgumentException("The number of values exceeds the number of ranges.");
			int index = 0;
			foreach(object value in values)
				RangeSet[index++].Value = value;
		}
		public void SetValueComparison(DashboardFormatConditionComparisonType valueComparison) {
			for(int i = 0; i < RangeSet.Count; i++)
				RangeSet[i].ValueComparison = valueComparison;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XElement rangeSetElement = new XElement(XmlRangeSet);
			RangeSet.SaveToXml(rangeSetElement);
			element.Add(rangeSetElement);
			XmlHelper.Save(element, XmlValueType, ValueType, DefaultValueType);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.LoadEnum<DashboardFormatConditionValueType>(element, XmlValueType, x => valueType = x);
			XElement rangeSetElement = element.Element(XmlRangeSet);
			rangeSet.LoadFromXml(rangeSetElement);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			var source = obj as FormatConditionRangeBase;
			if(source != null) {
				ValueType = source.ValueType;
				RangeSet.Assign(source.RangeSet.Clone());
			}
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			return GetRangeInfo(valueProvider) != null;
		}
		protected override IStyleSettings CalcStyleSettingCore(IFormatConditionValueProvider valueProvider) {
			RangeInfo rangeInfo = GetRangeInfo(valueProvider);
			return rangeInfo != null ? rangeInfo.ActualStyleSettings : null;
		}
		protected RangeInfo GetRangeInfo(IFormatConditionValueProvider valueProvider) {
			object value = valueProvider.GetValue(this);
			if(value == null)
				return null;
			object min = null, max = null;
			if(HasPercents) {
				min = GetAggregationValue(SummaryItemTypeEx.Min);
				max = GetAggregationValue(SummaryItemTypeEx.Max);
			}
			if(ValueType == DashboardFormatConditionValueType.Percent && (min == null || max == null))
				return null;
			var sortedRanges = SortedRanges;
			for(int i = sortedRanges.Count - 1; i >= 0; i--) {
				RangeInfo range = sortedRanges[i];
				object number = range.Value;
				if(ValueType == DashboardFormatConditionValueType.Percent)
					number = ValueManager.CalculatePercent(min, max, number);
				int res = ValueManager.CompareValues(value, number, true);
				if(range.ValueComparison == DashboardFormatConditionComparisonType.Greater) {
					if(res > 0) return range;
				}
				if(range.ValueComparison == DashboardFormatConditionComparisonType.GreaterOrEqual) {
					if(res >= 0) return range;
				}
			}
			return null;
		}
		protected override IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			if(IsValid && HasPercents)
				return new SummaryItemTypeEx[] { SummaryItemTypeEx.Min, SummaryItemTypeEx.Max };
			else 
				return EmptyAggregationTypes;
		}
		void OnRangeSetCollectionChanged(object sender, DataAccess.NotifyingCollectionChangedEventArgs<RangeInfo> e) {
			foreach(RangeInfo item in e.AddedItems) {
				item.Owner = this;
			}
			foreach(RangeInfo item in e.RemovedItems) {
				item.Owner = null;
			}
			if(e.AddedItems.Count > 0 || e.RemovedItems.Count > 0) {
				OnChanged();
			}
		}
	}
}
