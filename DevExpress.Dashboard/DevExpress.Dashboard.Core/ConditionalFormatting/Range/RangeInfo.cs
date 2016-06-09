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

using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
#if !DXPORTABLE // TODO dnxcore
	[TypeConverter(typeof(UniversalTypeConverterEx))]
#endif
	public sealed class RangeInfo {
		public const double NegativeInfinity = double.NegativeInfinity;
		const string XmlRangeInfo = "RangeInfo";
		const string XmlPredefinedName = "PredefinedName";
		const string XmlValueComparison = "ValueComparison";
		const string XmlValue = "Value";
		const DashboardFormatConditionComparisonType DefaultValueComparison = DashboardFormatConditionComparisonType.GreaterOrEqual;
		object value;
		FormatConditionRangeBase owner;
		StyleSettingsBase styleSettings;
		DashboardFormatConditionComparisonType valueComparison = DefaultValueComparison;
		[
		DefaultValue(null)
		]
		public StyleSettingsBase StyleSettings {
			get { return styleSettings; }
			set {
				if(StyleSettings == value) return;
				styleSettings = value;
				OnChanged();
			}
		}
		public object Value {
			get { return value; }
			set {
				if(Value == value) return;
				this.value = value;
				OnChanged();
			}
		}
		[
		DefaultValue(DefaultValueComparison)
		]
		public DashboardFormatConditionComparisonType ValueComparison {
			get { return valueComparison; }
			set {
				if(ValueComparison == value) return;
				valueComparison = value;
				OnChanged();
			}
		}
		internal StyleSettingsBase ActualStyleSettings {
			get {
				FormatConditionRangeGradient gradient = owner as FormatConditionRangeGradient; 
				return gradient != null ? gradient.GetActualStyleSettings(this) : styleSettings;
			}
		}
		internal FormatConditionRangeBase Owner { 
			get { return owner; }
			set { owner = value; }
		}
		public override string ToString() {
			return string.Format("{0} {1}{2}", ValueComparison.ToSign(), ValueToString(Value),
				(Owner != null && Owner.ValueType == DashboardFormatConditionValueType.Percent ? " %" : ""));
		}
		internal XElement SaveToXml() {
			XElement mainElement = new XElement(XmlRangeInfo);
			XmlHelper.Save(mainElement, XmlValueComparison, ValueComparison, DefaultValueComparison);
			XmlHelper.SaveObject(mainElement, XmlValue, Value);
			ConditionalFormattingSerializer.Save(styleSettings, mainElement);
			return mainElement;
		}
		internal void LoadFromXml(XElement element) {
			XmlHelper.LoadEnum<DashboardFormatConditionComparisonType>(element, XmlValueComparison, x => valueComparison = x);
			XmlHelper.LoadObject(element, XmlValue, x => value = x);
			this.styleSettings = ConditionalFormattingSerializer.LoadFirst<StyleSettingsBase>(element);
		}
		internal RangeInfo Clone() {
			RangeInfo range = new RangeInfo();
			range.StyleSettings = StyleSettings;
			range.Value = Value;
			range.ValueComparison = ValueComparison;
			return range;
		}
		void OnChanged() {
			if(Owner != null) Owner.OnChanged();
		}
		string ValueToString(object value) {
			if(object.Equals(value, decimal.MinValue)) return "-Minimum";
			if(object.Equals(value, decimal.MaxValue)) return "Maximum";
			return value.ToString();
		}
		void ResetValue() { 
			Value = null; 
		}
		bool ShouldSerializeValue() { 
			return Value != null;
		}
	}
}
