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
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
#if !DXPORTABLE
using DevExpress.Utils.Editors;
#endif
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class FormatConditionValue : FormatConditionStyleBase {
		const DashboardFormatCondition DefaultCondition = DashboardFormatCondition.Equal;
		const string XmlCondition = "Condition";
		const string XmlValue1 = "Value1";
		const string XmlValue2 = "Value2";
		DashboardFormatCondition condition = DefaultCondition;
		object value1, value2;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionValueCondition"),
#endif
		DefaultValue(DefaultCondition)
		]
		public DashboardFormatCondition Condition {
			get { return condition; }
			set {
				if (condition != value) {
					condition = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionValueValue1"),
#endif
		DefaultValue(null)
#if !DXPORTABLE
		,Editor(typeof(UIObjectEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectEditorTypeConverter))
#endif
		]
		public object Value1 {
			get { return value1; }
			set {
				if(Value1 != value) {
					value1 = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionValueValue2"),
#endif
		DefaultValue(null)
#if !DXPORTABLE
	   ,Editor(typeof(UIObjectEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectEditorTypeConverter))
#endif
		]
		public object Value2 {
			get { return value2; }
			set {
				if(Value2 != value) {
					value2 = value;
					OnChanged();
				}
			}
		}
		public FormatConditionValue() {
		}
		public FormatConditionValue(DashboardFormatCondition condition, object value) : this() {
			this.condition = condition;
			this.value1 = value;
		}
		public FormatConditionValue(DashboardFormatCondition condition, object value1, object value2) : this(condition, value1) {
			this.value2 = value2;
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionValue();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlCondition, Condition, DefaultCondition);
			XmlHelper.SaveObject(element, XmlValue1, Value1);
			XmlHelper.SaveObject(element, XmlValue2, Value2);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.LoadEnum<DashboardFormatCondition>(element, XmlCondition, x => condition = x);
			XmlHelper.LoadObject(element, XmlValue1, x => value1 = x);
			XmlHelper.LoadObject(element, XmlValue2, x => value2 = x);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionValue;
			if(source != null) {
				Condition = source.Condition;
				Value1 = source.Value1;
				Value2 = source.Value2;
			}
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			object val = valueProvider.GetValue(this);
			if(!CheckValue(val)) return false;
			object value1 = Value1, value2 = Value2;
			if(val != null && value1 != null && Condition == DashboardFormatCondition.ContainsText) {
				string strValue1 = Convert.ToString(value1),
					strValue = Convert.ToString(val);
				return strValue.Contains(strValue1);
			}
			int res1 = ValueManager.CompareValues(val, value1, true);
			switch(Condition) {
				case DashboardFormatCondition.Equal: return res1 == 0;
				case DashboardFormatCondition.NotEqual: return res1 != 0;
				case DashboardFormatCondition.Less: return res1 < 0;
				case DashboardFormatCondition.Greater: return res1 > 0;
				case DashboardFormatCondition.GreaterOrEqual: return res1 >= 0;
				case DashboardFormatCondition.LessOrEqual: return res1 <= 0;
				case DashboardFormatCondition.Between:
				case DashboardFormatCondition.NotBetween:
				case DashboardFormatCondition.BetweenOrEqual:
				case DashboardFormatCondition.NotBetweenOrEqual:
					int res2 = ValueManager.CompareValues(val, value2, true);
					if(Condition == DashboardFormatCondition.Between)
						return res1 > 0 && res2 < 0;
					if(Condition == DashboardFormatCondition.BetweenOrEqual)
						return res1 >= 0 && res2 <= 0;
					if(Condition == DashboardFormatCondition.NotBetween)
						return res1 <= 0 || res2 >= 0;
					if(Condition == DashboardFormatCondition.NotBetweenOrEqual)
						return res1 > 0 && res2 < 0;
					return false;
				default:
					throw new InvalidEnumArgumentException("Condition", (int)Condition, typeof(DashboardFormatCondition));
			}
		}
	}
}
