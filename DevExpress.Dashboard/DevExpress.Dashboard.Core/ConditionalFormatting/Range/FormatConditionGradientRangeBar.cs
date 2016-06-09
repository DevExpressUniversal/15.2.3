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
	public class FormatConditionGradientRangeBar : FormatConditionRangeGradient, IMinMaxInfo {
		const string XmlBarOptions = "BarOptions";
		readonly FormatConditionBarOptions barOptions;
		public FormatConditionGradientRangeBar() {
			this.barOptions = new FormatConditionBarOptions();
		}
		public FormatConditionGradientRangeBar(StyleSettingsBase startStyle, StyleSettingsBase endStyle, int segmentCount)
			: this() {
			Generate(startStyle, endStyle, segmentCount);
		}
		public FormatConditionGradientRangeBar(StyleSettingsBase startStyle, StyleSettingsBase endStyle, object[] values)
			: this() {
			Generate(startStyle, endStyle, values);
		}
		public FormatConditionGradientRangeBar(FormatConditionRangeGradientPredefinedType type)
			: this() {
			Generate(type);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormatConditionBarOptions BarOptions { get { return barOptions; } }
		public override bool IsValid {
			get {
				foreach(RangeInfo range in RangeSet)
					if(range.StyleSettings != null && !(range.StyleSettings is IBarColorStyleSettings))
						return false;
				return base.IsValid;
			}
		}
		protected internal override bool IsBarAggregationsRequired { get { return true; } }
		protected override string GradientStopException { get { return "Use colors with the 'Gradient' prefix from the FormatConditionAppearanceType enumeration to initialize the BarStyleSettings.PredefinedColor property or set the BarStyleSettings.PredefinedColor property to Custom and specify the BarStyleSettings.Color property."; } }
		DashboardFormatConditionValueType IMinMaxInfo.MinimumType { get { return DashboardFormatConditionValueType.Automatic; } }
		DashboardFormatConditionValueType IMinMaxInfo.MaximumType { get { return DashboardFormatConditionValueType.Automatic; } }
		public override FormatConditionRangeGradientPredefinedType ActualPredefinedType {
			get { return FormatConditionRangeGenerator.GetPredefinedType(this); }
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionGradientRangeBar();
		}
		protected override StyleSettingsBase ValidateStyle(StyleSettingsBase style) {
			if(!IsGradientStop(style))
				throw new ArgumentException("Color should be specified for BarStyleSettings objects used to specify FormatConditionGradientRangeBar start/end style settings.");
			return style;
		}
		protected override bool IsGradientStop(StyleSettingsBase style) {
			IBarColorStyleSettings colorStyle = style as IBarColorStyleSettings;
			return colorStyle != null &&
				(colorStyle.Color.HasValue && colorStyle.PredefinedColor == FormatConditionAppearanceType.Custom ||
				colorStyle.PredefinedColor != FormatConditionAppearanceType.None && FormatConditionAppearanceTypeGroups.GradientColors.ToAppearanceTypes().Contains(colorStyle.PredefinedColor));
		}
		public override void Generate(FormatConditionRangeGradientPredefinedType type) {
			FormatConditionRangeGenerator.GenerateBarGradientRangeSet(this, type, type.ToBarStyleSettings());
		}
		override internal ConditionModel CreateModel() {
			return new BarConditionModel() {
				FixedColors = GetFixedColors(),
				BarOptions = BarOptions.CreateModel()
			};
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
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			FormatConditionGradientRangeBar source = obj as FormatConditionGradientRangeBar;
			if(source != null) {
				BarOptions.Assign(source.BarOptions);
			}
		}
		protected override StyleSettingsBase GetRangeIndexSettings(int index) {
			return new RangeIndexSettings(this, index, true);
		}
	}
}
