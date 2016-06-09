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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Export.Xl {
	#region XlCondFmtType
	public enum XlCondFmtType {
		AboveOrBelowAverage,
		BeginsWith,
		CellIs,
		ColorScale,
		ContainsBlanks,
		ContainsErrors,
		ContainsText,
		DataBar,
		DuplicateValues,
		EndsWith,
		Expression,
		IconSet,
		NotContainsBlanks,
		NotContainsErrors,
		NotContainsText,
		TimePeriod,
		Top10,
		UniqueValues
	}
	#endregion
	#region XlCondFmtOperator
	public enum XlCondFmtOperator {
		BeginsWith,
		Between,
		ContainsText,
		EndsWith,
		Equal,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		NotBetween,
		NotContains,
		NotEqual
	}
	#endregion
	#region XlCondFmtValueObjectType
	public enum XlCondFmtValueObjectType {
		Number,
		Percent,
		Max,
		Min,
		Formula,
		Percentile,
		AutoMin,
		AutoMax
	}
	#endregion
	#region XlCondFmtValueObject
	public class XlCondFmtValueObject {
		XlValueObject valueObject = XlValueObject.Empty;
		public XlCondFmtValueObjectType ObjectType { get; set; }
		public XlValueObject Value {
			get { return valueObject; }
			set {
				if(value == null)
					valueObject = XlValueObject.Empty;
				else
					valueObject = value;
			}
		}
	}
	#endregion
	#region XlCondFmtRule (abstract)
	public abstract class XlCondFmtRule {
		int priority = 1;
		protected XlCondFmtRule(XlCondFmtType ruleType) {
			RuleType = ruleType;
		}
		public XlCondFmtType RuleType { get; private set; }
		public int Priority {
			get { return priority; }
			set {
				Guard.ArgumentPositive(value, "Priority");
				priority = value;
			}
		}
		public bool StopIfTrue { get; set; }
	}
	#endregion
	#region XlCondFmtRuleWithGuid (abstract)
	public abstract class XlCondFmtRuleWithGuid : XlCondFmtRule {
		readonly Guid id;
		protected XlCondFmtRuleWithGuid(XlCondFmtType ruleType) 
			: base(ruleType) {
			this.id = Guid.NewGuid();
		}
		internal string GetRuleId() {
			return id.ToString("B").ToUpper();
		}
	}
	#endregion
	#region XlCondFmtRuleWithFormatting (abstract)
	public abstract class XlCondFmtRuleWithFormatting : XlCondFmtRule {
		protected XlCondFmtRuleWithFormatting(XlCondFmtType ruleType)
			: base(ruleType) {
		}
		public XlDifferentialFormatting Formatting { get; set; }
	}
	#endregion
	#region XlCondFmtAverageCondition
	public enum XlCondFmtAverageCondition {
		Above,
		AboveOrEqual,
		Below,
		BelowOrEqual
	}
	#endregion
	#region XlCondFmtRuleAboveAverage
	public class XlCondFmtRuleAboveAverage : XlCondFmtRuleWithFormatting {
		int stdDev;
		public XlCondFmtRuleAboveAverage()
			: base(XlCondFmtType.AboveOrBelowAverage) {
			Condition = XlCondFmtAverageCondition.Above;
		}
		public XlCondFmtAverageCondition Condition { get; set; }
		public int StdDev {
			get { return stdDev; }
			set {
				if(value < 0 || value > 3)
					throw new ArgumentOutOfRangeException("StdDev out of range 0...3!");
				stdDev = value;
			}
		}
		protected internal bool AboveAverage {
			get {
				return Condition == XlCondFmtAverageCondition.Above || Condition == XlCondFmtAverageCondition.AboveOrEqual;
			}
		}
		protected internal bool EqualAverage {
			get {
				return Condition == XlCondFmtAverageCondition.AboveOrEqual || Condition == XlCondFmtAverageCondition.BelowOrEqual;
			}
		}
	}
	#endregion
	#region XlCondFmtRuleCellIs
	public class XlCondFmtRuleCellIs : XlCondFmtRuleWithFormatting {
		XlValueObject value = XlValueObject.Empty;
		XlValueObject secondValue = XlValueObject.Empty;
		public XlCondFmtRuleCellIs()
			: base(XlCondFmtType.CellIs) {
		}
		public XlCondFmtOperator Operator { get; set; }
		public XlValueObject Value { 
			get { return value; }
			set {
				if(value == null)
					this.value = XlValueObject.Empty;
				else
					this.value = value;
			}
		}
		public XlValueObject SecondValue { 
			get { return secondValue; }
			set {
				if(value == null)
					this.secondValue = XlValueObject.Empty;
				else
					this.secondValue = value;
			}
		}
	}
	#endregion
	#region XlCondFmtRuleBlanks
	public class XlCondFmtRuleBlanks : XlCondFmtRuleWithFormatting {
		public XlCondFmtRuleBlanks(bool containsBlanks)
			: base(containsBlanks ? XlCondFmtType.ContainsBlanks : XlCondFmtType.NotContainsBlanks) {
		}
	}
	#endregion
	#region XlCondFmtSpecificTextType
	public enum XlCondFmtSpecificTextType {
		Contains = XlCondFmtType.ContainsText,
		NotContains = XlCondFmtType.NotContainsText,
		BeginsWith = XlCondFmtType.BeginsWith,
		EndsWith = XlCondFmtType.EndsWith
	}
	#endregion
	#region XlCondFmtRuleSpecificText
	public class XlCondFmtRuleSpecificText : XlCondFmtRuleWithFormatting {
		string text = string.Empty;
		public XlCondFmtRuleSpecificText(XlCondFmtSpecificTextType ruleType, string text)
			: base((XlCondFmtType)ruleType) {
			Text = text;
		}
		public string Text {
			get { return text; }
			set {
				if (string.IsNullOrEmpty(value))
					text = string.Empty;
				else
					text = value;
			}
		}
	}
	#endregion
	#region XlCondFmtRuleDuplicates
	public class XlCondFmtRuleDuplicates : XlCondFmtRuleWithFormatting {
		public XlCondFmtRuleDuplicates()
			: base(XlCondFmtType.DuplicateValues) {
		}
	}
	#endregion
	#region XlCondFmtRuleUnique
	public class XlCondFmtRuleUnique : XlCondFmtRuleWithFormatting {
		public XlCondFmtRuleUnique()
			: base(XlCondFmtType.UniqueValues) {
		}
	}
	#endregion
	#region XlCondFmtRuleTop10
	public class XlCondFmtRuleTop10 : XlCondFmtRuleWithFormatting {
		bool percent;
		int rank = 10;
		public XlCondFmtRuleTop10()
			: base(XlCondFmtType.Top10) {
		}
		public bool Bottom { get; set; }
		public bool Percent {
			get { return percent; }
			set {
				percent = value;
				if(percent && (rank > 100))
					rank = 100;
			}
		}
		public int Rank {
			get { return rank; }
			set {
				int maxValue = percent ? 100 : 1000;
				if(value < 1 || value > maxValue)
					throw new ArgumentOutOfRangeException(string.Format("Rank out of range 1..{0}", maxValue));
				rank = value;
			}
		}
	}
	#endregion
	#region XlCondFmtRuleExpression
	public class XlCondFmtRuleExpression : XlCondFmtRuleWithFormatting {
		readonly XlExpression expression;
		readonly string formula;
		public XlCondFmtRuleExpression(XlExpression expression)
			: base(XlCondFmtType.Expression) {
			this.expression = expression;
			this.formula = null;
		}
		public XlCondFmtRuleExpression(string formula)
			: base(XlCondFmtType.Expression) {
			this.expression = null;
			if(!string.IsNullOrEmpty(formula) && formula[0] == '=')
				this.formula = formula.Substring(1);
			else
				this.formula = formula;
		}
		public XlExpression Expression { get { return expression; } }
		public string Formula { get { return formula; } }
	}
	#endregion
	#region XlCondFmtTimePeriod
	public enum XlCondFmtTimePeriod {
		Last7Days,
		LastMonth,
		LastWeek,
		NextMonth,
		NextWeek,
		ThisMonth,
		ThisWeek,
		Today,
		Tomorrow,
		Yesterday
	}
	#endregion
	#region XlCondFmtRuleTimePeriod
	public class XlCondFmtRuleTimePeriod : XlCondFmtRuleWithFormatting {
		public XlCondFmtRuleTimePeriod()
			: base(XlCondFmtType.TimePeriod) {
		}
		public XlCondFmtTimePeriod TimePeriod { get; set; }
	}
	#endregion
	#region XlCondFmtAxisPosition
	public enum XlCondFmtAxisPosition {
		Automatic,
		Midpoint,
		None
	}
	#endregion
	#region XlDataBarDirection
	public enum XlDataBarDirection {
		Context,
		LeftToRight,
		RightToLeft
	}
	#endregion
	#region XlCondFmtRuleDataBar
	public class XlCondFmtRuleDataBar : XlCondFmtRuleWithGuid {
		XlCondFmtValueObject minValue = new XlCondFmtValueObject();
		XlCondFmtValueObject maxValue = new XlCondFmtValueObject();
		int minLength = 0;
		int maxLength = 100;
		XlColor fillColor;
		XlColor borderColor;
		XlColor negativeFillColor;
		XlColor negativeBorderColor;
		XlColor axisColor;
		public XlCondFmtRuleDataBar()
			: base(XlCondFmtType.DataBar) {
			Direction = XlDataBarDirection.Context;
			fillColor = XlColor.Empty;
			borderColor = XlColor.Empty;
			negativeFillColor = DXColor.Red;
			negativeBorderColor = XlColor.Empty;
			AxisPosition = XlCondFmtAxisPosition.Automatic;
			axisColor = DXColor.Black;
			ShowValues = true;
			this.minValue.ObjectType = XlCondFmtValueObjectType.AutoMin;
			this.maxValue.ObjectType = XlCondFmtValueObjectType.AutoMax;
		}
		public XlDataBarDirection Direction { get; set; }
		public bool GradientFill { get; set; }
		public XlColor FillColor {
			get { return fillColor; }
			set { fillColor = value ?? XlColor.Empty; }
		}
		public XlColor BorderColor {
			get { return borderColor; }
			set { borderColor = value ?? XlColor.Empty; }
		}
		public XlColor NegativeFillColor {
			get { return negativeFillColor; }
			set { negativeFillColor = value ?? XlColor.Empty; }
		}
		public XlColor NegativeBorderColor {
			get { return negativeBorderColor; }
			set { negativeBorderColor = value ?? XlColor.Empty; }
		}
		public XlCondFmtAxisPosition AxisPosition { get; set; }
		public XlColor AxisColor {
			get { return axisColor; }
			set { axisColor = value ?? XlColor.Empty; }
		}
		public XlCondFmtValueObject MinValue { get { return minValue; } }
		public XlCondFmtValueObject MaxValue { get { return maxValue; } }
		public bool ShowValues { get; set; }
		public int MinLength {
			get { return minLength; }
			set {
				CheckLength(value, "MinLength");
				minLength = value;
			}
		}
		public int MaxLength {
			get { return maxLength; }
			set {
				CheckLength(value, "MaxLength");
				maxLength = value;
			}
		}
		void CheckLength(int value, string name) {
			if (value < 0 || value > 100)
				throw new ArgumentException(name);
		}
	}
	#endregion
	#region XlCondFmtIconSetType
	public enum XlCondFmtIconSetType {
		Arrows3,
		ArrowsGray3,
		Flags3,
		TrafficLights3,
		TrafficLights3Black,
		Signs3,
		Symbols3,
		Symbols3Circled,
		Stars3,
		Triangles3,
		Arrows4,
		ArrowsGray4,
		RedToBlack4,
		Rating4,
		TrafficLights4,
		Arrows5,
		ArrowsGray5,
		Rating5,
		Quarters5,
		Boxes5
	}
	#endregion
	#region XlCondFmtRuleIconSet
	public class XlCondFmtRuleIconSet : XlCondFmtRuleWithGuid {
		static Dictionary<XlCondFmtIconSetType, int> iconSetCountTable = CreateIconSetCountTable();
		static Dictionary<XlCondFmtIconSetType, int> CreateIconSetCountTable() {
			Dictionary<XlCondFmtIconSetType, int> result = new Dictionary<XlCondFmtIconSetType, int>();
			result.Add(XlCondFmtIconSetType.Arrows3, 3);
			result.Add(XlCondFmtIconSetType.ArrowsGray3, 3);
			result.Add(XlCondFmtIconSetType.Flags3, 3);
			result.Add(XlCondFmtIconSetType.TrafficLights3, 3);
			result.Add(XlCondFmtIconSetType.TrafficLights3Black, 3);
			result.Add(XlCondFmtIconSetType.Signs3, 3);
			result.Add(XlCondFmtIconSetType.Symbols3, 3);
			result.Add(XlCondFmtIconSetType.Symbols3Circled, 3);
			result.Add(XlCondFmtIconSetType.Stars3, 3);
			result.Add(XlCondFmtIconSetType.Triangles3, 3);
			result.Add(XlCondFmtIconSetType.Arrows4, 4);
			result.Add(XlCondFmtIconSetType.ArrowsGray4, 4);
			result.Add(XlCondFmtIconSetType.RedToBlack4, 4);
			result.Add(XlCondFmtIconSetType.Rating4, 4);
			result.Add(XlCondFmtIconSetType.TrafficLights4, 4);
			result.Add(XlCondFmtIconSetType.Arrows5, 5);
			result.Add(XlCondFmtIconSetType.ArrowsGray5, 5);
			result.Add(XlCondFmtIconSetType.Rating5, 5);
			result.Add(XlCondFmtIconSetType.Quarters5, 5);
			result.Add(XlCondFmtIconSetType.Boxes5, 5);
			return result;
		}
		XlCondFmtIconSetType iconSetType;
		readonly List<XlCondFmtValueObject> values = new List<XlCondFmtValueObject>();
		public XlCondFmtRuleIconSet()
			: base(XlCondFmtType.IconSet) {
			iconSetType = XlCondFmtIconSetType.TrafficLights3;
			Percent = true;
			ShowValues = true;
			SetupValues();
		}
		public XlCondFmtIconSetType IconSetType {
			get { return iconSetType; }
			set {
				if (iconSetType == value)
					return;
				iconSetType = value;
				SetupValues();
			}
		}
		public bool Percent { get; set; }
		public bool Reverse { get; set; }
		public bool ShowValues { get; set; }
		public IList<XlCondFmtValueObject> Values { get { return values; } }
		void SetupValues() {
			int count = iconSetCountTable[iconSetType];
			Values.Clear();
			if (count == 3) {
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 0 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 33 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 67 });
			}
			else if (count == 4) {
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 0 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 25 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 50 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 75 });
			}
			else if (count == 5) {
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 0 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 20 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 40 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 60 });
				Values.Add(new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percent, Value = 80 });
			}
		}
	}
	#endregion
	#region XlCondFmtColorScaleType
	public enum XlCondFmtColorScaleType {
		ColorScale2,
		ColorScale3
	}
	#endregion
	#region XlCondFmtRuleColorScale
	public class XlCondFmtRuleColorScale : XlCondFmtRule {
		#region Fields
		XlCondFmtValueObject minValue = new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Min };
		XlCondFmtValueObject midpointValue = new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Percentile, Value = 50 };
		XlCondFmtValueObject maxValue = new XlCondFmtValueObject() { ObjectType = XlCondFmtValueObjectType.Max };
		XlColor minColor;
		XlColor midpointColor;
		XlColor maxColor;
		#endregion
		public XlCondFmtRuleColorScale()
			: base(XlCondFmtType.ColorScale) {
			ColorScaleType = XlCondFmtColorScaleType.ColorScale3;
			minColor = DXColor.FromArgb(255, 248, 105, 107);
			midpointColor = DXColor.FromArgb(255, 255, 235, 132);
			maxColor = DXColor.FromArgb(255, 99, 190, 123);
		}
		#region Properties
		public XlCondFmtColorScaleType ColorScaleType { get; set; }
		public XlCondFmtValueObject MinValue { get { return minValue; } }
		public XlCondFmtValueObject MidpointValue { get { return midpointValue; } }
		public XlCondFmtValueObject MaxValue { get { return maxValue; } }
		public XlColor MinColor {
			get { return minColor; }
			set { minColor = value ?? XlColor.Empty; }
		}
		public XlColor MidpointColor {
			get { return midpointColor; }
			set { midpointColor = value ?? XlColor.Empty; }
		}
		public XlColor MaxColor {
			get { return maxColor; }
			set { maxColor = value ?? XlColor.Empty; }
		}
		#endregion
	}
	#endregion
	#region XlConditionalFormatting
	public class XlConditionalFormatting {
		readonly List<XlCellRange> ranges = new List<XlCellRange>();
		readonly List<XlCondFmtRule> rules = new List<XlCondFmtRule>();
		public XlConditionalFormatting() {
		}
		public IList<XlCellRange> Ranges { get { return ranges; } }
		public IList<XlCondFmtRule> Rules { get { return rules; } }
		internal XlCellPosition GetTopLeftCell() {
			int columnIndex = -1;
			int rowIndex = -1;
			foreach(XlCellRange range in Ranges) {
				if(columnIndex == -1)
					columnIndex = range.TopLeft.Column;
				else
					columnIndex = Math.Min(columnIndex, range.TopLeft.Column);
				if(rowIndex == -1)
					rowIndex = range.TopLeft.Row;
				else
					rowIndex = Math.Min(rowIndex, range.TopLeft.Row);
			}
			return new XlCellPosition(columnIndex, rowIndex);
		}
	}
	#endregion
}
