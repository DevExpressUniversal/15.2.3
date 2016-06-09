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
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Localization;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Design.Internal {
	#region UIUnitConverter
	public class UIUnitConverter {
		readonly UnitPrecisionDictionary unitPrecisionDictionary;
		public UIUnitConverter(UnitPrecisionDictionary unitPrecisionDictionary) {
			Guard.ArgumentNotNull(unitPrecisionDictionary, "unitPrecisionDictionary");
			this.unitPrecisionDictionary = unitPrecisionDictionary;
		}
		public UIUnit ToUIUnit(int value, DocumentUnit type) {
			return ToUIUnit(value, type, false);
		}
		internal UIUnit ToUIUnit(int value, DocumentUnit type, bool isValueInPercent) {
			return ToUIUnitF(value, type, isValueInPercent);
		}
		public UIUnit ToUIUnitF(float value, DocumentUnit type) {
			return ToUIUnitF(value, type, false);
		}
		public UIUnit ToUIUnitF(float value, DocumentUnit type, bool isValueInPercent) {
			if (isValueInPercent) {
				UIUnit result = new UIUnit(value, type, unitPrecisionDictionary);
				result.IsValueInPercent = true;
				return result;
			}
			switch (type) {
				case DocumentUnit.Centimeter:
					return new UIUnit(Units.TwipsToCentimetersF(value), DocumentUnit.Centimeter, unitPrecisionDictionary);
				case DocumentUnit.Inch:
					return new UIUnit(Units.TwipsToInchesF(value), DocumentUnit.Inch, unitPrecisionDictionary);
				case DocumentUnit.Millimeter:
					return new UIUnit(Units.TwipsToMillimetersF(value), DocumentUnit.Millimeter, unitPrecisionDictionary);
				case DocumentUnit.Point:
					return new UIUnit(Units.TwipsToPointsF(value), DocumentUnit.Point, unitPrecisionDictionary);
				default:
					return null;
			}
		}
		internal int ToInt(double value) {
			if (value > int.MaxValue)
				return int.MaxValue;
			else if (value < int.MinValue)
				return int.MinValue;
			return (int)value;
		}
		internal int ToTwipsUnit(UIUnit value) {
			return ToTwipsUnit(value, false);
		}
		public int ToTwipsUnit(UIUnit value, bool isValueInPercent) {
			float result = ToTwipsUnitF(value, isValueInPercent);
			if (result < 0)
				return ToInt(Math.Floor(result));
			return ToInt(Math.Ceiling(result));
		}
		public float ToTwipsUnitF(UIUnit value) {
			return ToTwipsUnitF(value, false);
		}
		internal float ToTwipsUnitF(UIUnit value, bool isValueInPercent) {
			if (isValueInPercent)
				return value.Value;
			switch (value.UnitType) {
				case DocumentUnit.Centimeter:
					return Units.CentimetersToTwipsF(value.Value);
				case DocumentUnit.Inch:
					return Units.InchesToTwipsF(value.Value);
				case DocumentUnit.Millimeter:
					return Units.MillimetersToTwipsF(value.Value);
				case DocumentUnit.Point:
					return Units.PointsToTwipsF(value.Value);
				default:
					return 0;
			}
		}
		public UIUnit CreateUIUnit(string text, DocumentUnit defaultUnitType) {
			return CreateUIUnit(text, defaultUnitType, false);
		}
		internal UIUnit CreateUIUnit(string text, DocumentUnit defaultUnitType, bool isValueInPercent) {
			return UIUnit.Create(text, defaultUnitType, unitPrecisionDictionary, isValueInPercent);
		}
	}
	#endregion
	#region UnitAbbreviationDictionary
	public class UnitAbbreviationDictionary : Dictionary<DocumentUnit, OfficeStringId> {
		public UnitAbbreviationDictionary() {
			PopulateByDefaultValues();
		}
		void PopulateByDefaultValues() {
			Add(DocumentUnit.Centimeter, OfficeStringId.UnitAbbreviation_Centimeter);
			Add(DocumentUnit.Inch, OfficeStringId.UnitAbbreviation_Inch);
			Add(DocumentUnit.Millimeter, OfficeStringId.UnitAbbreviation_Millimeter);
			Add(DocumentUnit.Point, OfficeStringId.UnitAbbreviation_Point);
		}
	}
	#endregion
	#region UnitPrecisionDictionary
	public class UnitPrecisionDictionary : Dictionary<DocumentUnit, int> {
		static readonly UnitPrecisionDictionary defaultPrecisions;
		static UnitPrecisionDictionary() {
			defaultPrecisions = new UnitPrecisionDictionary();
			defaultPrecisions.Add(DocumentUnit.Centimeter, 1);
			defaultPrecisions.Add(DocumentUnit.Inch, 1);
			defaultPrecisions.Add(DocumentUnit.Millimeter, 0);
			defaultPrecisions.Add(DocumentUnit.Point, 0);
		}
		public static UnitPrecisionDictionary DefaultPrecisions { get { return defaultPrecisions; } }
		protected UnitPrecisionDictionary() {
		}
	}
	#endregion
	#region UnitCaptionDictionary
	public class UnitCaptionDictionary : Dictionary<DocumentUnit, OfficeStringId> {
		public UnitCaptionDictionary() {
			PopulateByDefaultValues();
		}
		void PopulateByDefaultValues() {
			Add(DocumentUnit.Inch, OfficeStringId.Caption_UnitInches);
			Add(DocumentUnit.Centimeter, OfficeStringId.Caption_UnitCentimeters);
			Add(DocumentUnit.Millimeter, OfficeStringId.Caption_UnitMillimeters);
			Add(DocumentUnit.Point, OfficeStringId.Caption_UnitPoints);
		}
	}
	#endregion
	public class UIUnit {
		static readonly UnitAbbreviationDictionary unitAbbreviationDictionary;
		readonly UnitPrecisionDictionary unitPrecisionDictionary;
		static readonly UnitCaptionDictionary unitCaptionDictionary;
		static UIUnit() {
			unitAbbreviationDictionary = new UnitAbbreviationDictionary();
			unitCaptionDictionary = new UnitCaptionDictionary();
		}
		public static UnitAbbreviationDictionary UnitAbbreviationDictionary { get { return unitAbbreviationDictionary; } }
		public UnitPrecisionDictionary UnitPrecisionDictionary { get { return unitPrecisionDictionary; } }
		public static UnitCaptionDictionary UnitCaptionDictionary { get { return unitCaptionDictionary; } }
		public static char DecimalSeparator { get { return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]; } }
		public static UIUnit operator ++(UIUnit unit) {
			string resultValue = unit.OperationValidStringValue;
			if (unit.IsNegative && unit.RoundedStringValue != unit.StringValue) {
				unit.TruncValue();
				return unit;
			}
			if (IsZeroValue(resultValue))
				unit.SetIsNegative(false);
			if (unit.isNegative) {
				resultValue = Decrement(resultValue);
				if (IsZeroValue(resultValue))
					unit.SetIsNegative(false);
			}
			else
				resultValue = Increment(resultValue);
			unit.SetStringValue(resultValue);
			return unit;
		}
		public static UIUnit operator --(UIUnit unit) {
			string resultValue = unit.OperationValidStringValue;
			if (IsZeroValue(resultValue))
				unit.SetIsNegative(true);
			if (!unit.IsNegative && unit.RoundedStringValue != unit.StringValue) {
				unit.TruncValue();
				return unit;
			}
			if (unit.isNegative) {
				resultValue = Increment(resultValue);
			}
			else
				resultValue = Decrement(resultValue);
			unit.SetStringValue(resultValue);
			return unit;
		}
		internal static bool TryParse(string text, DocumentUnit defaultUnitType, out UIUnit result) {
			return TryParse(text, defaultUnitType, out result, false);
		}
		internal static bool TryParse(string text, DocumentUnit defaultUnitType, out UIUnit result, bool isValueInPercent) {
			result = null;
			DocumentUnit unitType = defaultUnitType;
			String stringValue = GetUnitTypeAndStringValue(text, defaultUnitType, out unitType, isValueInPercent);
			bool isNegative = false;
			string resultValue = String.Empty;
			bool isValid = TryParseFloatValue(stringValue, out resultValue, out isNegative);
			if (!isValid)
				return false;
			result = new UIUnit(resultValue, unitType, isNegative);
			result.IsValueInPercent = isValueInPercent;
			return true;
		}
		protected static bool TryParseFloatValue(string sourceStringValue, out string resultStringValue, out bool isNegative) {
			isNegative = false;
			resultStringValue = "0";
			string decimalSeparatorPattern = DecimalSeparator.ToString();
			decimalSeparatorPattern.Replace(".", "\\.");
			string pattern = String.Format("^(?<sign>\\+|-)?(?<value>\\d*{0}?(\\d+)?)$", decimalSeparatorPattern);
			Match match = Regex.Match(sourceStringValue.Trim(), pattern);
			if (!match.Success)
				return false;
			GroupCollection groups = match.Groups;
			Group signGroup = groups["sign"];
			Group valueGroup = groups["value"];
			if (signGroup.Success && signGroup.Value == "-")
				isNegative = true;
			resultStringValue = valueGroup.Value.Trim();
			if (resultStringValue.Length == 0 || (resultStringValue.Length == 1 && resultStringValue[0] == DecimalSeparator))
				isNegative = false;
			return true;
		}
		public static UIUnit Create(string text, DocumentUnit defaultUnitType, UnitPrecisionDictionary unitPrecisionDictionary) {
			return Create(text, defaultUnitType, unitPrecisionDictionary, false);
		}
		public static UIUnit Create(string text, DocumentUnit defaultUnitType, UnitPrecisionDictionary unitPrecisionDictionary, bool isValueInPercent) {
			DocumentUnit unitType = defaultUnitType;
			String stringValue = GetUnitTypeAndStringValue(text, defaultUnitType, out unitType, isValueInPercent);
			UIUnit result = new UIUnit(stringValue, unitType, unitPrecisionDictionary);
			result.IsValueInPercent = isValueInPercent;
			return result;
		}
		private static string GetUnitTypeAndStringValue(string text, DocumentUnit defaultUnitType, out DocumentUnit type, bool isValueInPercent) {
			if (text == null)
				text = String.Empty;
			type = defaultUnitType;
			string stringValue = text;
			foreach (DocumentUnit unitType in UnitAbbreviationDictionary.Keys) {
				string testAbbreviationText = GetTextAbbreviation(unitType, isValueInPercent).Trim();
				int index = text.LastIndexOf(testAbbreviationText);
				if (index == -1)
					continue;
				if (index == text.Length - testAbbreviationText.Length) {
					stringValue = text.Substring(0, index);
					type = unitType;
					break;
				}
			}
			return stringValue;
		}
		protected internal static string GetTextAbbreviation(DocumentUnit unitType) {
			return GetTextAbbreviation(unitType, false);
		}
		protected internal static string GetTextAbbreviation(DocumentUnit unitType, bool isValueInPercent) {
			if (isValueInPercent)
				return OfficeLocalizer.GetString(OfficeStringId.UnitAbbreviation_Percent);
			return OfficeLocalizer.GetString(UnitAbbreviationDictionary[unitType]);
		}
		protected internal static string GetTextCaption(DocumentUnit unitType) {
			return OfficeLocalizer.GetString(UnitCaptionDictionary[unitType]);
		}
		static bool IsZeroValue(string resultValue) {
			String trimZerosString = resultValue.Trim(new char[] { '0' });
			return trimZerosString.Length == 0 || trimZerosString == DecimalSeparator.ToString();
		}
		static string Increment(string number) {
			string result = string.Empty;
			for (int pos = number.Length - 1; ; --pos) {
				if (pos < 0) {
					result = '1' + result;
					return result;
				}
				char currentChar = number[pos];
				if (currentChar == '9') {
					result = '0' + result;
				}
				else if (currentChar >= '0' && currentChar <= '9') {
					char nextChar = (char)(currentChar + 1);
					result = number.Substring(0, pos) + nextChar + result;
					return result;
				}
				else {
					result = currentChar + result;
				}
			}
		}
		static string Decrement(string number) {
			string result = string.Empty;
			for (int pos = number.Length - 1; ; --pos) {
				if (pos < 0) {
					return null;
				}
				char currentChar = number[pos];
				if (currentChar == '0') {
					result = '9' + result;
				}
				else if (currentChar >= '0' && currentChar <= '9') {
					char nextChar = (char)(currentChar - 1);
					result = number.Substring(0, pos) + nextChar + result;
					return result;
				}
				else {
					result = currentChar + result;
				}
			}
		}
		DocumentUnit unitType;
		string stringValue;
		bool isNegative;
		bool isValueInPercent;
		public UIUnit(string stringValue, DocumentUnit type, UnitPrecisionDictionary unitPrecisionDictionary) {
			this.unitPrecisionDictionary = unitPrecisionDictionary;
			this.unitType = GetValidType(type);
			TryParseFloatValue(stringValue, out this.stringValue, out this.isNegative);
			this.stringValue = GetValidValueString(this.stringValue, UnitPrecisionDictionary[UnitType] + 1);
			this.stringValue = TrimInsignificantZeros(StringValue);
			this.isValueInPercent = false;
		}
		public UIUnit(float value, DocumentUnit type, UnitPrecisionDictionary unitPrecisionDictionary) {
			this.unitPrecisionDictionary = unitPrecisionDictionary;
			this.unitType = GetValidType(type);
			float roundedValue = (float)Math.Round(value, UnitPrecisionDictionary[UnitType] + 1);
			this.stringValue = Math.Abs(roundedValue).ToString();
			this.isNegative = value < 0;
			this.isValueInPercent = false;
		}
		protected UIUnit(string value, DocumentUnit type, bool isNegative) {
			this.stringValue = value;
			this.unitType = GetValidType(type);
			this.isNegative = isNegative;
			this.isValueInPercent = false;
		}
		public DocumentUnit UnitType { get { return unitType; } }
		protected internal bool IsValueInPercent { get { return isValueInPercent; } set { isValueInPercent = value; } }
		internal string OperationValidStringValue {
			get {
				string result = GetValidValueString(stringValue, UnitPrecisionDictionary[UnitType]);
				return result;
			}
		}
		internal string StringValue {
			get {
				return TrimInsignificantZeros(stringValue);
			}
		}
		internal string RoundedStringValue {
			get {
				string result = TrimInsignificantZeros(OperationValidStringValue);
				return result;
			}
		}
		public bool IsNegative { get { return isNegative; } }
		public float Value {
			get {
				float result = 0;
				bool isParsed = float.TryParse(StringValue, out result);
				if (!isParsed)
					return 0;
				if (IsNegative)
					return -1 * result;
				return result;
			}
		}
		public virtual bool IsValidValue {
			get {
				float resultValue = 0;
				return float.TryParse(StringValue, out resultValue);
			}
		}
		public override string ToString() {
			string signString = (IsNegative) ? "-" : String.Empty;
			string resultStringValue = TrimInsignificantZeros(StringValue);
			return String.Format("{0}{1}{2}", signString, resultStringValue, GetTextAbbreviation(UnitType, IsValueInPercent));
		}
		DocumentUnit GetValidType(DocumentUnit type) {
			if (type == DocumentUnit.Document)
				return DocumentUnit.Inch;
			return type;
		}
		protected internal virtual void SetIsNegative(bool isNegative) {
			this.isNegative = isNegative;
		}
		protected internal virtual void SetStringValue(string stringValue) {
			this.stringValue = stringValue;
		}
		string TrimInsignificantZeros(string stringValue) {
			String result = TrimEndInsignificantZeros(stringValue);
			return TrimBeginInsignificantZeros(result);
		}
		string TrimEndInsignificantZeros(string stringValue) {
			if (stringValue.IndexOf(DecimalSeparator) == -1)
				return stringValue;
			string newValue = stringValue.TrimEnd(new char[] { '0' });
			int length = newValue.Length;
			if (newValue[length - 1] == DecimalSeparator)
				return newValue.Substring(0, length - 1);
			return newValue;
		}
		string TrimBeginInsignificantZeros(string stringValue) {
			int count = stringValue.Length;
			int position = 0;
			for (position = 0; position < count; position++) {
				if (stringValue[position] != '0')
					break;
			}
			if (position == count || position == 0)
				return stringValue;
			if (!Char.IsDigit(stringValue[position]))
				position--;
			return stringValue.Substring(position, count - position);
		}
		protected internal virtual string GetValidValueString(string stringValue, int maxDigitsAfterDecimalSeparator) {
			if (String.IsNullOrEmpty(stringValue))
				return CreateZeroStringValue(maxDigitsAfterDecimalSeparator);
			string[] stringValueParts = stringValue.Split(new char[] { DecimalSeparator });
			int partCount = stringValueParts.Length;
			if (partCount > 2)
				return CreateZeroStringValue(maxDigitsAfterDecimalSeparator);
			string leftPart = (String.IsNullOrEmpty(stringValueParts[0])) ? "0" : stringValueParts[0];
			string rightPart = GetRightPart(stringValueParts, maxDigitsAfterDecimalSeparator);
			if (!IsIntegerValue(leftPart) || (!String.IsNullOrEmpty(rightPart) && !IsIntegerValue(rightPart)))
				return CreateZeroStringValue(maxDigitsAfterDecimalSeparator);
			rightPart = GetValueOfDesiredLength(rightPart, maxDigitsAfterDecimalSeparator);
			if (maxDigitsAfterDecimalSeparator < 1)
				return leftPart;
			return String.Format("{0}{1}{2}", leftPart, DecimalSeparator, rightPart);
		}
		bool IsIntegerValue(string value) {
			int count = value.Length;
			for (int i = 0; i < count; i++) {
				char item = value[i];
				if (item < '0' || item > '9')
					return false;
			}
			return true;
		}
		string GetRightPart(string[] stringValueParts, int maxDigitsAfterDecimalSeparator) {
			int partCount = stringValueParts.Length;
			return (partCount == 2) ? stringValueParts[1] : String.Empty;
		}
		string GetValueOfDesiredLength(string value, int desiredLength) {
			if (value.Length > desiredLength)
				return value.Substring(0, desiredLength);
			int zeroCount = desiredLength - value.Length;
			return value + new String('0', zeroCount);
		}
		string CreateZeroStringValue(int maxDigitsAfterDecimalSeparator) {
			string result = "0";
			if (maxDigitsAfterDecimalSeparator > 0) {
				result += DecimalSeparator;
				string zeros = new string('0', maxDigitsAfterDecimalSeparator);
				result += zeros;
			}
			return result;
		}
		internal void TruncValue() {
			this.stringValue = OperationValidStringValue;
		}
	}
}
