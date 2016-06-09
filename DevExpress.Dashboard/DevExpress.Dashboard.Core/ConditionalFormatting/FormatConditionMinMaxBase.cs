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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
namespace DevExpress.DashboardCommon {
	public abstract class FormatConditionMinMaxBase : FormatConditionStyleBase {
		const string XmlMinimumType = "MinimumType";
		const string XmlMaximumType = "MaximumType";
		const string XmlMinimum = "Minimum";
		const string XmlMaximum = "Maximum";
		const DashboardFormatConditionValueType DefaultType = DashboardFormatConditionValueType.Automatic;
		static decimal DefaultMinimum = 0M;
		static decimal DefaultMaximum = 0M;
		DashboardFormatConditionValueType minimumType = DefaultType;
		DashboardFormatConditionValueType maximumType = DefaultType;
		decimal minimum;
		decimal maximum;
		[DefaultValue(DefaultType)]
		public DashboardFormatConditionValueType MinimumType {
			get { return minimumType; }
			set {
				if(MinimumType != value) {
					minimumType = value;
					OnChanged();
				}
			}
		}
		[DefaultValue(DefaultType)]
		public DashboardFormatConditionValueType MaximumType {
			get { return maximumType; }
			set {
				if(MaximumType != value) {
					maximumType = value;
					OnChanged();
				}
			}
		}
		public decimal Minimum {
			get { return minimum; }
			set {
				if(Minimum == value) return;
				minimum = value;
				OnChanged();
			}
		}
		public decimal Maximum {
			get { return maximum; }
			set {
				if(Maximum == value) return;
				maximum = value;
				OnChanged();
			}
		}
		protected override decimal Min { get { return Minimum; } }
		protected override decimal Max { get { return Maximum; } }
		protected override DashboardFormatConditionValueType MinType { get { return MinimumType; } }
		protected override DashboardFormatConditionValueType MaxType { get { return MaximumType; } }
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlMinimumType, MinimumType, DefaultType);
			XmlHelper.Save(element, XmlMaximumType, MaximumType, DefaultType);
			XmlHelper.Save(element, XmlMinimum, Minimum, DefaultMinimum);
			XmlHelper.Save(element, XmlMaximum, Maximum, DefaultMaximum);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.LoadEnum<DashboardFormatConditionValueType>(element, XmlMinimumType, x => minimumType = x);
			XmlHelper.LoadEnum<DashboardFormatConditionValueType>(element, XmlMaximumType, x => maximumType = x);
			XmlHelper.Load<decimal>(element, XmlMinimum, x => minimum = x);
			XmlHelper.Load<decimal>(element, XmlMaximum, x => maximum = x);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionMinMaxBase;
			if(source != null) {
				MinimumType = source.MinimumType;
				MaximumType = source.MaximumType;
				Minimum = source.Minimum;
				Maximum = source.Maximum;
			}
		}
		protected override IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			if(MinimumType != DashboardFormatConditionValueType.Number || MaximumType != DashboardFormatConditionValueType.Number) {
				yield return SummaryItemTypeEx.Min;			
				yield return SummaryItemTypeEx.Max;
			}
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			object value = ValueManager.ConvertToNumber(valueProvider.GetValue(this));
			if(value == null)
				return false;
			return true;
		}
		void ResetMinimum() { Minimum = DefaultMinimum; }
		void ResetMaximum() { Maximum = DefaultMaximum; }
		bool ShouldSerializeMinimum() { return Minimum != DefaultMinimum; }
		bool ShouldSerializeMaximum() { return Maximum != DefaultMaximum; }
	}
}
