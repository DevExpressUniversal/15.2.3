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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class FormatConditionColorRangeBar : FormatConditionRangeSet, IMinMaxInfo {
		const string XmlBarOptions = "BarOptions";
		readonly FormatConditionBarOptions barOptions;
		public FormatConditionColorRangeBar()
			: base() {
			this.barOptions = new FormatConditionBarOptions();
		}
		public FormatConditionColorRangeBar(FormatConditionRangeSetPredefinedType type)
			: this() {
			Generate(type);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormatConditionBarOptions BarOptions { get { return barOptions; } }
		public override bool IsValid {
			get {
				foreach(RangeInfo range in RangeSet)
					if(!(range.StyleSettings is IBarColorStyleSettings))
						return false;
				return (RangeSet.Count >= 2);
			}
		}
		protected internal override bool IsBarAggregationsRequired { get { return true; } }
		DashboardFormatConditionValueType IMinMaxInfo.MinimumType { get { return DashboardFormatConditionValueType.Automatic; } }
		DashboardFormatConditionValueType IMinMaxInfo.MaximumType { get { return DashboardFormatConditionValueType.Automatic; } }
		public override void Generate(FormatConditionRangeSetPredefinedType type) {
			FormatConditionRangeGenerator.Generate(this, type);
		}
		public override FormatConditionRangeSetPredefinedType ActualPredefinedType {
			get { return FormatConditionRangeGenerator.GetPredefinedType(this); }
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionColorRangeBar();
		}
		protected override IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			if(IsValid)
				return new SummaryItemTypeEx[] { SummaryItemTypeEx.Min, SummaryItemTypeEx.Max };
			else
				return EmptyAggregationTypes;
		}
		protected override decimal? CalcNormalizedValueCore(IFormatConditionValueProvider valueProvider) {
			return FormatConditionBarCalculator.CalcNormalizedValue(this, this, valueProvider, BarOptions.AllowNegativeAxis);
		}
		protected override decimal? CalcZeroPositionCore(IFormatConditionValueProvider valueProvider) {
			return FormatConditionBarCalculator.CalcZeroPosition(this, BarOptions.AllowNegativeAxis);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XElement optionsElement = new XElement(XmlBarOptions);
			barOptions.SaveToXml(optionsElement);
			element.Add(optionsElement);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XElement optionsElement = element.Element(XmlBarOptions);
			if(optionsElement != null)
				barOptions.LoadFromXml(optionsElement);
		}
		internal override ConditionModel CreateModel() {
			BarConditionModel model = new BarConditionModel() {
				BarOptions = BarOptions.CreateModel()
			};
			return model;
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			FormatConditionColorRangeBar source = obj as FormatConditionColorRangeBar;
			if(source != null) {
				BarOptions.Assign(source.BarOptions);
			}
		}
	}
}
