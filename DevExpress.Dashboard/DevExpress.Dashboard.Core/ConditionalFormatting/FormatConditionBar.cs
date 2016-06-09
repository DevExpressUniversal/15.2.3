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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class FormatConditionBar : FormatConditionMinMaxBase {
		const string XmlNegativeStyleSettings = "NegativeStyleSettings";
		const string XmlStyleSettings = "StyleSettings";
		const string XmlBarOptions = "BarOptions";
		readonly FormatConditionBarOptions barOptions;
		BarStyleSettings negativeStyleSettings;
		BarStyleSettings styleSettings;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new BarStyleSettings StyleSettings {
			get { return styleSettings; }
			set {
				if(StyleSettings == value)
					return;
				if(value != null && value.Owner != null && value.Owner != this) {
					throw new InvalidOperationException("StyleSettings already has an Owner.");
				}
				if(HasStyleSettings)
					styleSettings.Owner = null;
				styleSettings = value;
				if(HasStyleSettings)
					styleSettings.Owner = this;
				OnChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarStyleSettings NegativeStyleSettings {
			get { return negativeStyleSettings; }
			set {
				if(NegativeStyleSettings == value)
					return;
				if(value != null && value.Owner != null && value.Owner != this) {
					throw new InvalidOperationException("StyleSettings already has an Owner.");
				}
				if(HasNegativeStyleSettings)
					negativeStyleSettings.Owner = null;
				negativeStyleSettings = value;
				if(HasNegativeStyleSettings)
					negativeStyleSettings.Owner = this;
				OnChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormatConditionBarOptions BarOptions { get { return barOptions; } }
		bool HasNegativeStyleSettings { get { return negativeStyleSettings != null; } }
		protected override IEnumerable<StyleSettingsBase> Styles { 
			get { 
				yield return StyleSettings;
				yield return NegativeStyleSettings;
			} 
		}
		public override bool IsValid { get { return StyleSettings is IBarColorStyleSettings && NegativeStyleSettings is IBarColorStyleSettings; } }
		protected internal override bool IsBarAggregationsRequired { get { return true; } }
		public FormatConditionBar() {
			this.barOptions = new FormatConditionBarOptions() { Owner = this };
			this.styleSettings = new BarStyleSettings() { Owner = this };
			this.negativeStyleSettings = new BarStyleSettings() { Owner = this };
		}
		public FormatConditionBar(FormatConditionAppearanceType barColor, FormatConditionAppearanceType negativeBarColor)
			: this() {
			this.StyleSettings = new BarStyleSettings(barColor);
			this.NegativeStyleSettings = new BarStyleSettings(negativeBarColor);
		}
		public FormatConditionBar(Color barColor, Color negativeBarColor)
			: this() {
			this.StyleSettings = new BarStyleSettings(barColor);
			this.NegativeStyleSettings = new BarStyleSettings(negativeBarColor);
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionBar();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XElement negativeStyleSettingsElement = new XElement(XmlNegativeStyleSettings);
			((IXmlSerializableElement)negativeStyleSettings).SaveToXml(negativeStyleSettingsElement);
			element.Add(negativeStyleSettingsElement);
			XElement styleSettingsElement = new XElement(XmlStyleSettings);
			((IXmlSerializableElement)styleSettings).SaveToXml(styleSettingsElement);
			element.Add(styleSettingsElement);
			XElement optionsElement = new XElement(XmlBarOptions);
			barOptions.SaveToXml(optionsElement);
			element.Add(optionsElement);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XElement negativeStyleSettingsElement = element.Element(XmlNegativeStyleSettings);
			if(negativeStyleSettingsElement != null) {
				if(negativeStyleSettings == null)
					negativeStyleSettings = new BarStyleSettings();
				((IXmlSerializableElement)negativeStyleSettings).LoadFromXml(negativeStyleSettingsElement);
			}
			XElement styleSettingsElement = element.Element(XmlStyleSettings);
			if(styleSettingsElement != null) {
				if(styleSettings == null)
					styleSettings = new BarStyleSettings();
				((IXmlSerializableElement)styleSettings).LoadFromXml(styleSettingsElement);
			}
			XElement optionsElement = element.Element(XmlBarOptions);
			if(optionsElement != null)
				barOptions.LoadFromXml(optionsElement);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionBar;
			if(source != null) {
				BarOptions.Assign(source.BarOptions);
				if(source.NegativeStyleSettings != null) {
					NegativeStyleSettings = (BarStyleSettings)((IStyleSettings)source.NegativeStyleSettings).Clone();
				}
				if(source.StyleSettings != null) {
					StyleSettings = (BarStyleSettings)((IStyleSettings)source.StyleSettings).Clone();
				}
			}
		}
		protected override IStyleSettings CalcStyleSettingCore(IFormatConditionValueProvider valueProvider) {
			object value = ValueManager.ConvertToNumber(valueProvider.GetValue(this));
			if(value == null)
				return null;
			return ValueManager.CompareValues(value, 0, true) >= 0 ? StyleSettings : NegativeStyleSettings;
		}
		internal override ConditionModel CreateModel() {
			BarConditionModel model = new BarConditionModel() {
				BarOptions = BarOptions.CreateModel()
			};
			return model;
		}
		protected override decimal? CalcNormalizedValueCore(IFormatConditionValueProvider valueProvider) {
			return FormatConditionBarCalculator.CalcNormalizedValue(this, this, valueProvider, BarOptions.AllowNegativeAxis);
		}
		protected override decimal? CalcZeroPositionCore(IFormatConditionValueProvider valueProvider) {
			return FormatConditionBarCalculator.CalcZeroPosition(this, BarOptions.AllowNegativeAxis);
		}
	}
}
