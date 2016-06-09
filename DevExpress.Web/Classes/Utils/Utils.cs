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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DevExpress.Data;
using System.Web.UI.Design;
using System.ComponentModel.Design;
using DevExpress.Data.Access;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	public interface IEndInitAccessor {
		void EndInit();
	}
	public interface IEndInitAccessorContainer {
	}
	public interface IStateManagerTracker {
		void ViewStateLoaded();
	}
	public interface IPropertiesDirtyTracker { 
		void SetPropertiesDirty();
	}
	public class LicenseUtils {
		public static bool IsTrial(Type type) {
			return false;
		}
		public static bool IsExpired(Type type) {
			return false;
		}
	}
	public class DemoUtils {
		static string lastdemo;
		public static void RegisterDemo(string module, string groupKey, string demoKey) {
			string text = string.Format("{0}.{1}.{2}", module, groupKey, demoKey);
			if(text == lastdemo) return;
			lastdemo = text;
		}
		public static void RegisterDemo(string demo) {
			if(lastdemo == demo) return;
			lastdemo = demo;
		}
	}
	public struct DefaultValueContainer {
		private object value;
		private bool hasValue;
		public static readonly DefaultValueContainer Empty = new DefaultValueContainer();
		public DefaultValueContainer(object value) {
			this.value = value;
			this.hasValue = true;
		}
		public object Value {
			get {
				if (!HasValue)
					throw new InvalidOperationException("DefaultValueContainer has no value, so it's Value property is inaccessible.");
				return value;
			}
		}
		public bool HasValue {
			get { return hasValue; }
		}
		public override string ToString() {
			if (!HasValue)
				return "[No Value]";
			else
				return Value != null ? Value.ToString() : "null";
		}
		public override int GetHashCode() {
			if (!HasValue)
				return -1;
			return Value != null ? Value.GetHashCode() : 0;
		}
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			if (obj is DefaultValueContainer) {
				DefaultValueContainer valueContainer = (DefaultValueContainer)obj;
				return !valueContainer.HasValue && !HasValue || valueContainer.HasValue && HasValue && CommonUtils.AreEqual(valueContainer.Value, Value, false);
			} else
				return CommonUtils.AreEqual(Value, obj, false);
		}
	}
	public static class CommonUtils {
		public const string CustomJSPropertyPrefix = "cp";
		private static readonly object ClientDateFormatInfoKey = new object();
		public static void AddNewItemToArrayWithSort<T>(T newItem, ref T[] array, Comparison<T> comparison) {
			if(!Array.Exists<T>(array, delegate(T item) { return AreEqual(item, newItem); })) {
				Array.Resize(ref array, array.Length + 1);
				array[array.Length - 1] = newItem;
				Array.Sort<T>(array, comparison);
			}
		}
		public static bool AreEqualsArrays(string[] array1, string[] array2) {
			if (array1.Length != array2.Length)
				return false;
			for (int i = 0; i < array1.Length; i++) {
				if (array1[i] != array2[i])
					return false;
			}
			return true;
		}
		public static bool AreEqualsStyles(IUrlResolutionService urlResolutionService, AppearanceStyleBase style1, AppearanceStyleBase style2) {
			return style1 == style2 || (style1 != null && style2 != null &&
				style1.CssClass == style2.CssClass &&
				style1.GetStyleAttributes(urlResolutionService).Value == style2.GetStyleAttributes(urlResolutionService).Value);
		}
		public static bool AreEqual(object value1, object value2, bool convertEmptyStringToNull) {
			bool equal = false;
			if(convertEmptyStringToNull)
				equal = IsNullOrEmpty(value1) && IsNullOrEmpty(value2);
			return equal || AreEqual(value1, value2);
		}
		public static bool IsNullOrEmpty(object val) {
			return val == null || val.ToString() == string.Empty;
		}
		private static bool AreEqual(object value1, object value2) {
			return Object.Equals(value1, value2) ||
				value1 is ValueType && value2 is ValueType && value1.Equals(value2);
		}
		public static object ConvertToType(object value, Type type, bool positive) {
			if (type == null)
				return value;
			object result = GetDefaultValue(type, positive);
			try {
				result = CommonUtils.GetConvertedArgumentValue(value, type, "");
			} catch {
			}
			return result;
		}
		public static object GetDefaultValue(Type type, bool positive) {
			object result = null;
			TypeCode code = Type.GetTypeCode(type);
			switch (code) {
				case TypeCode.Empty:
					break;
				case TypeCode.Boolean:
					result = positive ? true : false;
					break;
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.Int32:
					result = positive ? 1 : 0;
					break;
				case TypeCode.String:
					result = positive ? "True" : "";
					break;
				case TypeCode.DateTime:
					result = DateTime.Now;
					break;
			}
			return result;
		}
		public static string GetObjectText(object obj) {
			return GetObjectText(obj, false);
		}
		public static string GetObjectText(object obj, bool includeSubObjects) {
			string res = string.Empty;
			try {
				PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
				foreach (PropertyDescriptor pd in coll) {
					if (!pd.IsBrowsable || pd.SerializationVisibility == DesignerSerializationVisibility.Hidden)
						continue;
					if (pd.SerializationVisibility == DesignerSerializationVisibility.Content) {
						if (includeSubObjects) {
							object val = pd.GetValue(obj);
							string s = (val != null) ? val.ToString() : string.Empty;
							if (!string.IsNullOrEmpty(s)) {
								if (res.Length > 0)
									res += ", ";
								res += string.Format("{0} = {{ {1} }}", pd.Name, s);
							}
						}
					} else if (!pd.IsReadOnly && pd.ShouldSerializeValue(obj)) {
						if (res.Length > 0)
							res += ", ";
						res += pd.Name;
						object val = pd.GetValue(obj);
						if (pd.PropertyType.Equals(typeof(string)))
							res += string.Format(" = '{0}'", val);
						else
							res += string.Format(" = {0}", val);
					}
				}
			} catch {
			}
			return res;
		}
		#region B132204: String Array Deserialization
		private const char SerializedStringArraySeparator = '|';
		public static string SerializeStringArray(string[] array) {
			if (array == null || array.Length == 0)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < array.Length; i++) {
				string item = array[i];
				sb.Append(item.Length);
				sb.Append(SerializedStringArraySeparator);
				sb.Append(item);
			}
			return sb.ToString();
		}
		public static List<string> DeserializeStringArray(string serializedData) {
			List<string> items = new List<string>();
			if (!string.IsNullOrEmpty(serializedData)) {
				int currentPos = 0;
				int dataLength = serializedData.Length;
				while (currentPos < dataLength) {
					string item = DeserializeStringArrayItem(serializedData, ref currentPos);
					items.Add(item);
				}
			}
			return items;
		}
		private static string DeserializeStringArrayItem(string serializedData, ref int currentPos) {
			int indexOfFirstSeparator = serializedData.IndexOf(SerializedStringArraySeparator, currentPos);
			string itemLengthString = serializedData.Substring(currentPos, indexOfFirstSeparator - currentPos);
			int itemLength = Int32.Parse(itemLengthString);
			currentPos += itemLengthString.Length + 1;
			string item = serializedData.Substring(currentPos, itemLength);
			currentPos += itemLength;
			return item;
		}
		#endregion
		public static string ValueToString(object value) {
			return value == null ? "" : value.ToString();
		}
		public static bool IsNullValue(object value) {
			return value == null || (value is DBNull);
		}
		public static object GetConvertedArgumentValue(object value, Type targetType, string argumentName) {
			if (value == null)
				return null;
			if (targetType == null)
				return value;
			targetType = ReflectionUtils.StripNullableType(targetType);
			if (targetType != typeof(String) && value.ToString() == "")
				return null;
			try {
				if (targetType.IsEnum)
					return Enum.Parse(targetType, value.ToString());
				else if (targetType == typeof(String))
					return value.ToString();
				else if (targetType == typeof(Guid))
					return new Guid(value.ToString());
				else if(targetType == typeof(TimeSpan))
					return TimeSpan.Parse(value.ToString());
				else {
					if(targetType == typeof(Double)) {
						double retValue;
						if(!Double.TryParse(value.ToString(), out retValue))
							retValue = (double)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
						return retValue;
					}
					return Convert.ChangeType(value, targetType);
				}
			} catch (FormatException e) {
				throw new ArgumentException(String.Format(StringResources.UnableToCast, argumentName, targetType), e);
			}
		}
		public static object GetHtmlTextWriterTagObject(string tagName) {
			if (string.IsNullOrEmpty(tagName)) return null;
			try {
				return (HtmlTextWriterTag)Enum.Parse(typeof(HtmlTextWriterTag), tagName, true);
			} catch (Exception) {
			}
			return null;
		}
		public static string GetFormatString(string formatString) {
			bool shouldModifyFormatString = (formatString.IndexOf('{') == -1);
			return (!shouldModifyFormatString ? formatString : ("{0" + ((!string.IsNullOrEmpty(formatString)) ? ":" + formatString : string.Empty) + "}"));
		}
		public static string GetDefaultTextFormatString(int placeHolderCount, bool rtl) {
			if(placeHolderCount < 1)
				return String.Empty;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < placeHolderCount; i++) {
				if(i > 0)
					sb.Append(rtl ? " " : "; ");
				sb.Append('{');				
				sb.Append(rtl ? placeHolderCount - i - 1 : i);
				sb.Append('}');				
			}
			return sb.ToString();
		}
		public static string GetDayName(DayOfWeek day, CultureInfo culture, DayNameFormat dayNameFormat) {
			if (dayNameFormat == DayNameFormat.Short) {
				return culture.DateTimeFormat.GetAbbreviatedDayName(day);
			} else {
				string dayName = culture.DateTimeFormat.GetDayName(day);
				dayName = Capitalize(dayName);
				switch (dayNameFormat) {
					case DayNameFormat.FirstLetter:
					case DayNameFormat.Shortest:
						return dayName[0].ToString();
					case DayNameFormat.FirstTwoLetters:
						return dayName[0].ToString() + dayName[1].ToString();
					default:
						return dayName;
				}
			}
		}
		public static string GetDayName(DayOfWeek day, DayNameFormat dayNameFormat) {
			return GetDayName(day, CultureInfo.CurrentCulture, dayNameFormat);
		}
		public static DateTime GetFirstDateOfMonthView(int year, int month, DayOfWeek firstDay) {
			DateTime date = new DateTime(year, month, 1);
			int offset = (int)date.DayOfWeek - (int)firstDay;
			if (offset < 0)
				offset += 7;
			if(date != DateTime.MinValue)
				return date.AddDays(-offset);
			return date;
		}
		public static string Capitalize(string str) {
			return str.Substring(0, 1).ToUpper() + str.Substring(1);
		}
		public static void CheckCustomPropertyName(string name) {
			if (!name.StartsWith(CustomJSPropertyPrefix)) 
				throw new ArgumentException(string.Format("Wrong custom property name '{0}'. Should start with the '{1}' prefix.", name, CustomJSPropertyPrefix));
		}
		private static void RaiseArgumentOutOfRangeException(string message) {
			throw new ArgumentOutOfRangeException("value", message);
		}
		public static void CheckMinimumValue(double value, double min, string propertyName) {
			if (min > value)
				throw new ArgumentOutOfRangeException("value",
					string.Format(StringResources.InvalidMinimumValue, propertyName, min));
		}
		public static void CheckValueRange(double value, double min, double max, string propertyName) {
			if (min > value || value > max)
				throw new ArgumentOutOfRangeException("value",
					string.Format(StringResources.InvalidValueRange, propertyName, min, max));
		}
		public static void CheckValueRange(Decimal value, Decimal min, Decimal max, string propertyName) {
			CheckValueRange((double)value, (double)min, (double)max, propertyName);
		}
		public static void CheckValueRange(int value, int min, int max, string propertyName) {
			CheckValueRange((double)value, (double)min, (double)max, propertyName);
		}
		public static void CheckNegativeValue(double value, string propertyName) {
			if (value < 0)
				throw new ArgumentOutOfRangeException("value",
					string.Format(StringResources.InvalidNegativeValue, propertyName));
		}
		public static void CheckNegativeOrZeroValue(double value, string propertyName) {
			if (value <= 0)
				throw new ArgumentOutOfRangeException("value",
					string.Format(StringResources.InvalidNonPositiveValue, propertyName));
		}
		public static void CheckNegativeOrZeroItems<T>(T[] items, string propertyName) {
			for(int i = 0; i < items.Length; i++) {
				int figure = 0;
				if(items[i] == null || !int.TryParse(items[i].ToString(), out figure) || figure < 1)
					throw new ArgumentOutOfRangeException("value",
						string.Format(StringResources.InvalidNonPositiveCollectionValue, propertyName));
			}
		}
		public static void CheckDuplicateItems<T>(T[] items, string propertyName) {
			List<T> list = new List<T>(items);
			for(int i = 0; i < list.Count; i++) {
				if(list.FindAll(delegate(T value) { return list[i].Equals(value); }).Count > 1)
					throw new ArgumentOutOfRangeException("value",
						string.Format(StringResources.InvalidDuplicateCollectionValue, propertyName));
			}
		}
		public static void CheckGreaterOrEqual(double greaterValue, double lessValue, string greaterPropertyName, string lessPropertyName) {
			if (greaterValue < lessValue)
				throw new ArgumentOutOfRangeException("value",
					string.Format(StringResources.InvalidValuesRatio, lessPropertyName, greaterPropertyName));
		}
		public static string FormatXmlDocumentText(XmlDocument xmlDocument) {
			string text = "";
			using (MemoryStream stream = new MemoryStream()) {
				xmlDocument.Save(stream); 
				using (StreamReader sr = new StreamReader(stream)) {
					stream.Seek(0, SeekOrigin.Begin);
					text = sr.ReadToEnd();
				}
			}
			return text;
		}
		public static int[][] ArrangePartInRow(int[] partsCount, int columnCount, bool needCategorize,
			RepeatDirection repeatDirection) {
			List<int[]> ret = new List<int[]>();
			for (int i = 0; i < partsCount.Length; i++) {
				List<int> partsInRows = new List<int>();
				if ((needCategorize) && (repeatDirection == RepeatDirection.Horizontal))
					partsInRows.AddRange(ArrangePartInCategory(partsCount[i], columnCount));
				else {
					if (repeatDirection == RepeatDirection.Horizontal)
						partsInRows.AddRange(ArrangePartInCategory(partsCount[i], columnCount));
					else
						partsInRows.Add(partsCount[i]);
				}
				ret.Add(partsInRows.ToArray());
			}
			return ret.ToArray();
		}
		private static List<int> ArrangePartInCategory(int partCount, int columnCount) {
			List<int> ret = new List<int>();
			int columnCountActualForPart = GetColumnCountForPart(partCount, columnCount);
			int rowCount = GetRowCount(partCount, columnCountActualForPart);
			for (int i = 0; i < rowCount; i++) {
				if (i == rowCount - 1) 
					ret.Add(partCount - i * columnCountActualForPart);
				else
					ret.Add(columnCountActualForPart);
			}
			return ret;
		}
		private static int GetColumnCountForPart(int partCount, int columnCount) {
			int ret = columnCount;
			if (ret >= partCount)
				ret = partCount;
			if (ret < 1)
				ret = 1;
			return ret;
		}
		private static int GetRowCount(int partCount, int columnCount) {
			if (partCount > columnCount) {
				if (partCount % columnCount != 0)
					return (partCount / columnCount) + 1;
				else
					return partCount / columnCount;
			} else
				return 1;
		}
		public static int[] ArrangeParts(int[] partHeights, int columnCount) {
			int[] ret = null; 
			if (partHeights.Length == 0)
				ret = new int[0];
			else
				if (partHeights.Length <= columnCount)
					ret = ArrangePartInEachColumn(partHeights.Length);
				else {
					if (IsArrayContainsOnlyNumber(partHeights, 1))
						ret = ArrangePartsWithOneHeightInColumns(partHeights, columnCount);
					else
						ret = ArrangePartsInColumns(partHeights, columnCount); 
					if (ret.Length != columnCount)
						ret = CorrectionPartsInColumn(ret, columnCount);
				}
			return ret;
		}
		public static int[] ArrangePartsByStartingNumber(int[] partNumbers, int partCount, int columnCount) {
			int[] ret = new int[columnCount];
			if (partNumbers[0] != 0)
				partNumbers[0] = 0;
			for (int i = 0; i < partNumbers.Length - 1; i++) {
				ret[i] = partNumbers[i + 1] - partNumbers[i];
			}
			ret[partNumbers.Length - 1] = partCount == partNumbers[partNumbers.Length - 1] ? 1 : partCount - partNumbers[partNumbers.Length - 1];
			return ret;
		}
		public static int[] ArrangePartInEachColumn(int columnCount) {
			int[] ret = new int[columnCount];
			for (int i = 0; i < columnCount; i++)
				ret[i] = 1;
			return ret;
		}
		public static int[] ArrangePartsInColumns(int[] partHeights, int columnCount) {
			int sum = 0;
			for (int i = 0; i < partHeights.Length; i++)
				sum += partHeights[i];
			int a = 0;
			int b = sum / 2;
			int c = sum;
			int newColumnCount = 0;
			int lastHeight = 0;
			while (Math.Abs(c - b) > 1) {
				newColumnCount = CalcColumnCount(partHeights, b);
				if (newColumnCount > columnCount) {
					int newB = SeekForward(b, c);
					a = b;
					b = newB;
				} else {
					if (newColumnCount == columnCount)
						lastHeight = b;
					int newB = SeekBack(a, b);
					c = b;
					b = newB;
				}
			}
			if (lastHeight == 0)
				lastHeight = b;
			return CalcPartCountInColumns(partHeights, columnCount, lastHeight);
		}
		public static int[] ArrangePartsWithOneHeightInColumns(int[] partHeights, int columnCount) {
			List<int> partCountInColumns = new List<int>(columnCount);
			int maxPartCountInColumn = (int)Math.Ceiling((double)partHeights.Length / columnCount);
			int partCount = partHeights.Length;
			int curPartCount = 0;
			int curColumn = 1;
			while (curPartCount < partCount) {
				if (partCount - curPartCount - maxPartCountInColumn >= columnCount - curColumn) {
					partCountInColumns.Add(maxPartCountInColumn);
					curPartCount += maxPartCountInColumn;
					curColumn++;
				} else {
					if (curColumn < columnCount) {
						partCountInColumns.Add(1);
						curColumn++;
						curPartCount++;
					} else {
						partCountInColumns.Add(partCount - curPartCount);
						curPartCount += partCountInColumns[columnCount - 1];
					}
				}
			}
			return partCountInColumns.ToArray();
		}
		public static int CalcColumnCount(int[] partHeights, int height) {
			int sum = 0;
			int ret = 0;
			int partIndex = 0;
			int prevIndex = 0;
			while (partIndex < partHeights.Length) {
				sum += partHeights[partIndex];
				if (sum > height) {
					ret++;
					sum = 0;
					if (partIndex != prevIndex) {
						prevIndex = partIndex;
						partIndex--;
					} else
						prevIndex = partIndex + 1;
				}
				partIndex++;
			}
			if (sum != 0)
				ret++;
			return ret;
		}
		public static int SeekForward(int currentHeight, int endRange) {
			int ret = 0;
			ret = currentHeight + (endRange - currentHeight) / 2;
			return ret;
		}
		public static int SeekBack(int currentHeight, int startRange) {
			int ret = 0;
			ret = startRange + (currentHeight - startRange) / 2;
			return ret;
		}
		public static int[] CorrectionPartsInColumn(int[] partsInColumns, int columnCount) {
			List<int> retList = new List<int>(partsInColumns);
			if (partsInColumns.Length > columnCount) {
				for (int i = partsInColumns.Length - 1; i >= columnCount; i--) {
					if (i - 2 >= 0) {
						int partsCountForReplace = Math.Max(retList[i], retList[i - 1]);
						retList[i - 1] -= partsCountForReplace;
						retList[i - 2] += partsCountForReplace;
						retList[i - 1] += retList[i];
					} else
						retList[i - 1] += retList[i];
					retList.RemoveAt(i);
				}
			} else {
				for (int i = partsInColumns.Length; i < columnCount; i++) { 
					retList.Add(1);
					int d = columnCount - i;
					retList[partsInColumns.Length - d]--;
					while (retList[partsInColumns.Length - d] == 0 && d <= partsInColumns.Length) {
						retList[partsInColumns.Length - d]++;
						d++;
						retList[partsInColumns.Length - d]--;
					}
				}
			}
			return retList.ToArray();
		}
		public static int[] CalcPartCountInColumns(int[] partHeights, int columnCount, int height) {
			int sum = 0;
			List<int> retList = new List<int>();
			int partIndex = 0;
			int prevIndex = 0;
			int columnIndex = 0;
			while (partIndex < partHeights.Length) {
				sum += partHeights[partIndex];
				if (sum > height) {
					sum = 0;
					if (partIndex != prevIndex) {
						retList.Add(partIndex - prevIndex);
						prevIndex = partIndex;
						partIndex--;
					} else {
						retList.Add(1);
						prevIndex = partIndex + 1;
					}
					columnIndex++;
				}
				partIndex++;
			}
			if (partIndex - prevIndex != 0)
				retList.Add(partIndex - prevIndex);
			return retList.ToArray();
		}
		public static bool IsArrayContainsOnlyNumber(int[] array, int number) {
			for (int i = 0; i < array.Length; i++)
				if (array[i] != number)
					return false;
			return true;
		}
		public static string GetDefaultFormUrl(string formName, Type type) {
			return GetDefaultFormsFolder(type) + formName +
				RenderUtils.DefaultUserControlFileExtension;
		}
		public static string GetSPDefaultFormUrl(string formName, Type type) {
			return String.Format(RenderUtils.DefaultSPFormsAppRelativeDirectoryPathTemplate, type.Name) + formName +
				RenderUtils.DefaultUserControlFileExtension;
		}
		public static string GetDefaultFormCodeBehindUrl(string formName, string extension, Type type) {
			return GetDefaultFormsFolder(type) + formName + extension;
		}
		public static string GetDefaultFormsFolder(Type type) {
			return String.Format(RenderUtils.DefaultFormsAppRelativeDirectoryPathTemplate, type.Name);
		}
		public static string GetVersionSuffix() {
			return "_v" + AssemblyInfo.Version;
		}
		public static string SplitPascalCaseString(string value) {
			value = Regex.Replace(value, "(\\p{Ll})(\\p{Lu})", "$1 $2");
			value = Regex.Replace(value, "(\\p{Lu}{2})(\\p{Lu}\\p{Ll}{2})", "$1 $2");
			return value;
		}
		public static byte[] GetBytesFromStream(Stream stream) {
			byte[] ret = null;
			if((stream != null) && (stream != Stream.Null)) {
				if(stream.Length > 0x7fffffffL)
					throw new HttpException(StringResources.UploadControl_StreamTooLong);
				if(!stream.CanSeek)
					throw new HttpException(StringResources.UploadControl_StreamNotSeekable);
				int position = (int)stream.Position;
				int count = (int)stream.Length;
				try {
					BinaryReader reader = new BinaryReader(stream);
					stream.Seek(0L, SeekOrigin.Begin);
					ret = reader.ReadBytes((int)stream.Length);
				}
				finally {
					stream.Seek(position, SeekOrigin.Begin);
				}
				if(ret.Length != count)
					throw new HttpException(StringResources.UploadControl_StreamLengthNotReached);
			}
			else
				ret = new byte[0];
			return ret;
		}
		public static void CopyStream(Stream input, Stream output) {
			byte[] buffer = new byte[8 * 1024];
			int len;
			while((len = input.Read(buffer, 0, buffer.Length)) > 0) {
				output.Write(buffer, 0, len);
			}
		}
		public static string GetMD5Hash(string str) {
			return CommonUtils.GetMD5Hash(Encoding.ASCII.GetBytes(str));
		}
		public static string GetMD5Hash(byte[] data) {
			string hashedValue = string.Empty;
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			foreach(byte b in md5.ComputeHash(data))
				hashedValue += b.ToString("x2");
			return hashedValue;
		}
		public static string RaiseCallbackErrorInternal(object sender, Exception exc) {
			ASPxWebControl.AddErrorForHandler(exc);
			ASPxWebControl.CallbackErrorMessageInternal = ASPxWebControl.OnCallbackExceptionInternal(exc);
			ASPxWebControl.RaiseCallbackErrorInternal(sender);
			ASPxWebControl.ClearErrorForHandler();
			return ASPxWebControl.CallbackErrorMessageInternal;
		}
	}
	#region Nested types
	public class SPViewInfoBase {
		const string FieldSearchPattern = "//{0}/FieldRef";
		const string GroupBySearchPattern = "//GroupBy";
		const string RowLimitSearchPattern = "//RowLimit";
		bool isGroupExpanded = false;
		int rowCount;
		Dictionary<string, ColumnSortOrder> fieldGroupOrderArray;
		Dictionary<string, ColumnSortOrder> fieldSortOrderArray;
		Dictionary<string, SummaryItemType> fieldSummaryTypeArray;
		public SPViewInfoBase(string viewXml) {
			ParseViewXml(viewXml);
		}
		public Dictionary<string, ColumnSortOrder> FieldGroupOrderArray {
			get { return fieldGroupOrderArray; }
		}
		public Dictionary<string, ColumnSortOrder> FieldSortOrderArray {
			get { return fieldSortOrderArray; }
		}
		public Dictionary<string, SummaryItemType> FieldSummaryTypeArray {
			get { return fieldSummaryTypeArray; }
		}
		public bool IsGroupExpanded {
			get { return isGroupExpanded; }
		}
		public int RowCount {
			get { return rowCount; }
		}
		public bool IsGroupedField(string fieldName) {
			return FieldGroupOrderArray.ContainsKey(ProcessFieldName(fieldName));
		}
		public int GetGroupIndex(string fieldName) {
			fieldName = ProcessFieldName(fieldName);
			return GetKeyIndex(fieldName, fieldGroupOrderArray.Keys);
		}
		public ColumnSortOrder GetGroupOrder(string fieldName) {
			return IsGroupedField(fieldName) ? FieldGroupOrderArray[ProcessFieldName(fieldName)] : ColumnSortOrder.None;
		}
		public bool IsSortedField(string fieldName) {
			return FieldSortOrderArray.ContainsKey(ProcessFieldName(fieldName));
		}
		public int GetSortIndex(string fieldName) {
			fieldName = ProcessFieldName(fieldName);
			return GetKeyIndex(fieldName, FieldSortOrderArray.Keys);
		}
		public ColumnSortOrder GetSortOrder(string fieldName) {
			return IsSortedField(fieldName) ? FieldSortOrderArray[ProcessFieldName(fieldName)] : ColumnSortOrder.None;
		}
		public bool IsSummaryExist(string fieldName) {
			return FieldSummaryTypeArray.ContainsKey(ProcessFieldName(fieldName));
		}
		public SummaryItemType GetSummaryType(string fieldName) {
			return IsSummaryExist(fieldName) ? FieldSummaryTypeArray[ProcessFieldName(fieldName)] : SummaryItemType.None;
		}
		protected void ParseViewXml(string viewXml) {
			XmlDocument doc = new XmlDocument();
			doc.InnerXml = viewXml;
			this.fieldSortOrderArray = GetFields(doc, "OrderBy");
			this.fieldGroupOrderArray = GetFields(doc, "GroupBy");
			this.fieldSummaryTypeArray = GetFieldSummaryTypeArray(doc);
			rowCount = GetRowCount(doc);
			isGroupExpanded = GetIsExpanded(doc);
		}
		private Dictionary<string, ColumnSortOrder> GetFields(XmlDocument doc, string settingsSectionName) {
			Dictionary<string, ColumnSortOrder> ret = new Dictionary<string, ColumnSortOrder>();
			XmlNodeList fieldXmlNodes = doc.SelectNodes(string.Format(FieldSearchPattern, settingsSectionName));
			for (int i = 0; i < fieldXmlNodes.Count; i++) {
				XmlNode curNode = fieldXmlNodes[i];
				ColumnSortOrder order = curNode.Attributes["Ascending"] == null ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
				ret.Add(curNode.Attributes["Name"].Value, order);
			}
			return ret;
		}
		private Dictionary<string, SummaryItemType> GetFieldSummaryTypeArray(XmlDocument doc) {
			Dictionary<string, SummaryItemType> ret = new Dictionary<string, SummaryItemType>();
			XmlNodeList fieldXmlNodes = doc.SelectNodes(string.Format(FieldSearchPattern, "Aggregations"));
			for (int i = 0; i < fieldXmlNodes.Count; i++) {
				XmlNode curNode = fieldXmlNodes[i];
				SummaryItemType summaryType = curNode.Attributes["Type"] == null ? SummaryItemType.None :
					ParseSummaryItemType(curNode.Attributes["Type"].ToString());
				ret.Add(curNode.Attributes["Name"].Value, summaryType);
			}
			return ret;
		}
		private SummaryItemType ParseSummaryItemType(string str) {
			SummaryItemType ret = SummaryItemType.Count;
			try {
				ret = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), str, true);
			} catch (Exception) { }
			return ret;
		}
		private int GetRowCount(XmlDocument doc) {
			int ret = -1;
			XmlElement gropuByXmlElement = (XmlElement)doc.SelectSingleNode(GroupBySearchPattern);
			if (gropuByXmlElement != null && gropuByXmlElement.Attributes["GroupLimit"] != null &&
				!GetIsExpanded(doc))
				ret = int.Parse(gropuByXmlElement.Attributes["GroupLimit"].Value);
			else {
				XmlElement rowCountXmlElement = (XmlElement)doc.SelectSingleNode(RowLimitSearchPattern);
				if (rowCountXmlElement != null)
					ret = int.Parse(rowCountXmlElement.InnerText);
			}
			return ret;
		}
		private bool GetIsExpanded(XmlDocument doc) {
			bool isExpanded = false;
			XmlElement gropuByXmlElement = (XmlElement)doc.SelectSingleNode(GroupBySearchPattern);
			if (gropuByXmlElement != null && gropuByXmlElement.Attributes["Collapse"] != null)
				isExpanded = !bool.Parse(gropuByXmlElement.Attributes["Collapse"].Value);
			return isExpanded;
		}
		private int GetKeyIndex(string key, IEnumerable keyEnum) {
			int ret = -1;
			foreach (string name in keyEnum) {
				ret++;
				if (name == key)
					break;
			}
			return ret;
		}
		private string ProcessFieldName(string fieldName) {
			return fieldName.Replace(" ", "");
		}
	}
	#endregion
	public class DataUtils {
		public static ICollection ConvertEnumerableToCollection(IEnumerable data) {
			ICollection dataCollection = data as ICollection;
			if ((data != null) && (dataCollection == null)) {
				ArrayList list = new ArrayList();
				foreach (object item in data) {
					list.Add(item);
				}
				dataCollection = list;
			}
			return dataCollection;
		}
		public static IList ConvertEnumerableToList(IEnumerable source) {
			if(source == null)
				return null;
			ArrayList list = new ArrayList();
			foreach(object obj in source)
				list.Add(obj);
			return list;
		}
		public static object ExtractValueFromDataContainer(object dataContainer, string fieldName, out bool fieldFound) {
			object value = null;
			fieldFound = false;
			if(dataContainer is DataRowView) {
				DataRowView dataRowView = dataContainer as DataRowView;
				if(dataRowView.Row.Table.Columns.Contains(fieldName)) {
					fieldFound = true;
					value = dataRowView[fieldName];
				}
			}
			else if (dataContainer is System.Dynamic.DynamicObject) {
				System.Dynamic.DynamicObject dynamicObject = (System.Dynamic.DynamicObject)dataContainer;
				fieldFound = dynamicObject.TryGetMember(new DxGetMemberBinder(fieldName, true), out value);
				} else {
					if(ReflectionUtils.TryToGetPropertyValue(dataContainer, fieldName, out value))
						fieldFound = true;
				}
			return value;
		}
		public static object GetFieldValue(object obj, string fieldName, bool isFieldRequired, bool designMode) {
			bool fieldFound;
			return GetFieldValue(obj, fieldName, isFieldRequired, designMode, DefaultValueContainer.Empty, out fieldFound);
		}
		public static object GetFieldValue(object obj, string fieldName, bool isFieldRequired, bool designMode, object defaultValue) {
			bool fieldFound;
			return GetFieldValue(obj, fieldName, isFieldRequired, designMode, new DefaultValueContainer(defaultValue), out fieldFound);
		}
		public static object GetFieldValue(object obj, string fieldName, bool isFieldRequired, bool designMode, out bool fieldFound) {
			return GetFieldValue(obj, fieldName, isFieldRequired, designMode, new DefaultValueContainer(null), out fieldFound);
		}
		private static object GetFieldValue(object obj, string fieldName, bool isFieldRequired, bool designMode,
			DefaultValueContainer defaultValue, out bool fieldFound) {
			object value = ExtractValueFromDataContainer(obj, fieldName, out fieldFound);
			if(fieldFound) {
				if(value == null && defaultValue.HasValue)
					value = defaultValue.Value;
			}
			else {
				if(isFieldRequired) {
					if(designMode)
						value = StringResources.DataControl_BoundFieldText;
					else
						throw new HttpException(string.Format(StringResources.DataControl_FieldNotFound, fieldName));
				}
				else {
					if(defaultValue.HasValue)
						value = defaultValue.Value;
					else
						throw new InvalidOperationException(string.Format("The non-required field '{0}' without the default value specified hasn't been found.", fieldName));
				}
			}
			return value;
		}
		public static bool GetPropertyValue<T>(object obj, string propertyName, PropertyDescriptorCollection properties, Action<T> onValue) {
			if(ReflectionUtils.IsPropertyExist(properties, propertyName)) {
				object propertyValue = Convert.ToString(ReflectionUtils.GetPropertyValue(properties, obj, propertyName));
				onValue((T)Convert.ChangeType(propertyValue, typeof(T)));
				return true;
			}
			return false;
		}
		public static string FixFloatingPoint(string floatString, CultureInfo culture) {
			if(String.IsNullOrEmpty(floatString))
				return floatString;
			if(culture.NumberFormat.NumberDecimalSeparator == ",")
				return floatString.Replace('.', ',');
			return floatString.Replace(',', '.');
		}
		public static bool IsNumericType(Type type) {
			return IsFloatType(type) || IsIntegralType(type);
		}
		public static bool IsFloatType(Type type) {
			return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
		}
		public static bool IsIntegralType(Type type) {
			return
				type == typeof(short) ||
				type == typeof(int) ||
				type == typeof(long) ||
				type == typeof(ushort) ||
				type == typeof(uint) ||
				type == typeof(ulong) ||
				type == typeof(sbyte) ||
				type == typeof(byte);
		}
		public static bool IsNullValue(object value) {
			return value == null || value == DBNull.Value;
		}
		public static decimal? ConvertToDecimalValue(object value) {
			if(IsNullValue(value))
				return null;
			if(value is DateTime)
				return ((DateTime)value).Ticks;
			if(!IsNumericType(value.GetType()))
				return null;
			try {
				return Convert.ToDecimal(value);
			} catch {
				return null;
			}
		}
		public delegate int BinarySearchComparer<T1, T2>(T1 element, T2 value);
		public static int BinarySearch<T1, T2>(T1[] array, T2 value, BinarySearchComparer<T1, T2> comparer) {
			var start = 0;
			var end = array.Length - 1;
			while(start <= end) {
				var middle = start + (end - start) / 2;
				var result = comparer(array[middle], value);
				if(result == 0)
					return middle;
				if(result == -1)
					start = middle + 1;
				else
					end = middle - 1;
			}
			return -1;
		}
		public static string[] MergeStringArrays(string[] array1, string[] array2) {
			string[] result = new string[array1.Length + array2.Length];
			Array.Copy(array1, result, array1.Length);
			Array.Copy(array2, 0, result, array1.Length, array2.Length);
			return result;			
		}
	}
	public delegate object ValueParsingCallback(string name, string value);
	public static class DictionarySerializer {
		private const char Separator = '|';
		public static string Serialize(IDictionary<string, object> dictionary) {
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, object> pair in dictionary) {
				if (pair.Value != null) {
					string valueStr = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					sb.AppendFormat("{1}{0}{2}{0}{3}", Separator, pair.Key, valueStr.Length, valueStr);
				}
			}
			return sb.ToString();
		}
		public static bool Deserialize(string data, IDictionary<string, object> dictionary) {
			return Deserialize(data, dictionary, DefaultValueParsingCallback);
		}
		public static bool Deserialize(string data, IDictionary<string, object> dictionary, ValueParsingCallback parseValue) {
			dictionary.Clear();
			if (!string.IsNullOrEmpty(data)) {
				ParseData(data, dictionary, parseValue);
				return true;
			}
			return false;
		}
		internal static void ParseData(string data, IDictionary<string, object> dictionary) {
			ParseData(data, dictionary, DefaultValueParsingCallback);
		}
		internal static void ParseData(string data, IDictionary<string, object> dictionary, ValueParsingCallback parseValue) {
			int startIndex = 0;
			while (ParseNameValuePair(data, ref startIndex, dictionary, parseValue)) ;
		}
		private static string DefaultValueParsingCallback(string name, string value) {
			return value.ToString();
		}
		internal static bool ParseNameValuePair(string data, ref int startIndex, IDictionary<string, object> dictionary, ValueParsingCallback parseValue) {
			if (startIndex >= data.Length)
				return false;
			int indexOfFirstSeparator = data.IndexOf(Separator, startIndex);
			string fieldName = data.Substring(startIndex, indexOfFirstSeparator - startIndex);
			startIndex += fieldName.Length + 1;
			int indexOfSecondSeparator = data.IndexOf(Separator, startIndex);
			string fieldValueLengthStr = data.Substring(startIndex, indexOfSecondSeparator - startIndex);
			startIndex += fieldValueLengthStr.Length + 1;
			int fieldValueLength = Int32.Parse(fieldValueLengthStr);
			string fieldValue = data.Substring(startIndex, fieldValueLength);
			startIndex += fieldValueLength;
			if (fieldValue != null)
				dictionary.Add(fieldName, parseValue(fieldName, fieldValue));
			return true;
		}
	}
	public static class ReflectionUtils {
		private static Dictionary<TypePropertyPair, PropertyDescriptor> propertyDescriptorsCache =
			new Dictionary<TypePropertyPair, PropertyDescriptor>();
		public static Type GetNonPublicTypeFromAssembly(string assemblyName, string typeName) {
			Assembly assembly = DevExpress.Data.Utils.Helpers.LoadWithPartialName(assemblyName);
			Type[] types = assembly.GetTypes();
			foreach (Type type in types) {
				if (type.Name == typeName)
					return type;
			}
			return null;
		}
		public static T GetNonPublicInstanceFieldValue<T>(object source, string fieldName) {
			return (T)GetNonPublicInstanceFieldValue(source, fieldName);
		}
		public static PropertyDescriptorCollection GetProperties(object source) {
			return TypeDescriptor.GetProperties(source);
		}
		public static object GetNonPublicInstancePropertyValue(object source, string propertyName) {
			BindingFlags binding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName, binding);
			return propertyInfo.GetValue(source, null);
		}
		public static object GetPropertyValue(object source, string propertyName) {
			object value;
			TryToGetPropertyValue(source, propertyName, out value);
			return value;
		}
		public static object GetPropertyValue(PropertyDescriptorCollection props, object source, string propertyName) {
			PropertyDescriptor p = props.Find(propertyName, true);
			return p == null ? null : p.GetValue(source);
		}
		public static void SetNonPublicInstanceFieldValue(object target, string fieldName, object fieldValue) {
			FindFieldInfo(target.GetType(), fieldName, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, fieldValue);
		}
		public static void SetNonPublicStaticFieldValue(Type type, string fieldName, object fieldValue) {
			FindFieldInfo(type, fieldName, BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, fieldValue);
		}
		public static object GetNonPublicInstanceFieldValue(object target, string fieldName) {
			return FindFieldInfo(target.GetType(), fieldName, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
		}
		public static void SetPropertyValue(object target, string propertyName, object propertyValue) {
			PropertyDescriptor descriptor = GetPropertyDescriptor(target, propertyName);
			descriptor.SetValue(target, propertyValue);
		}
		public static bool IsPropertyExist(object obj, string propertyName) {
			return GetPropertyDescriptor(obj, propertyName) != null;
		}
		public static bool IsPropertyExist(PropertyDescriptorCollection props, string propertyName) {
			return props.Find(propertyName, true) != null;
		}
		public static object InvokeStaticMethod(Type type, string methodName, params object[] parameters) {
			return InvokeMethod(type, null, methodName, true, parameters);
		}
		public static object InvokeInstanceMethod(object target, string methodName, params object[] parameters) {
			return InvokeMethod(null, target, methodName, false, parameters);
		}
		private static object InvokeMethod(Type type, object target, string methodName, bool isStatic, params object[] parameters) {
			if (target == null && !isStatic)
				throw new NullReferenceException("target");
			Type targetType = isStatic ? type : target.GetType();
			BindingFlags bindingFlags = (isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo method = FindMethodInfo(targetType, methodName, bindingFlags, parameters);
			if (method == null)
				throw new InvalidOperationException("Method not found");
			return method.Invoke(isStatic ? null : target, parameters);
		}
		private static MethodInfo FindMethodInfo(Type targetType, string methodName, BindingFlags bindingFlags, object[] parameters) {
			MethodInfo[] candidateMethods = Array.FindAll(targetType.GetMethods(bindingFlags), delegate(MethodInfo candidate) {
				if (candidate.Name != methodName)
					return false;
				ParameterInfo[] cndidateParameters = candidate.GetParameters();
				if (cndidateParameters.Length != parameters.Length)
					return false;
				for (int i = 0; i < cndidateParameters.Length; i++)
					if (!cndidateParameters[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
						return false;
				return true;
			});
			if (candidateMethods.Length > 1)
				throw new AmbiguousMatchException();
			return candidateMethods.Length == 0 ? null : candidateMethods[0];
		}
		public static bool TryToGetPropertyValue(object source, string propertyName, out object value) {
			if (IsObjectTypeNonCacheable(source))
				return TryToGetPropertyValueNonCacheable(source, propertyName, out value);
			return TryToExtractValueFromDescriptor(source, GetPropertyDescriptor(source, propertyName), out value);
		}
		private static FieldInfo FindFieldInfo(Type type, string fieldName, BindingFlags binding) {
			FieldInfo fieldInfo = null;
			while (fieldInfo == null && type != typeof(object)) {
				fieldInfo = type.GetField(fieldName, binding);
				type = type.BaseType;
			}
			return fieldInfo;
		}
		private static bool IsObjectTypeNonCacheable(object obj) {
			return obj is ICustomTypeDescriptor;
		}
		private static PropertyDescriptor GetPropertyDescriptor(object obj, string propertyName) {
			if (IsObjectTypeNonCacheable(obj))
				return GetPropertyDescriptorNonCacheable(obj, propertyName);
			TypePropertyPair key = new TypePropertyPair(obj != null ? obj.GetType() : null, propertyName);
			if (key.IsEmpty)
				return null;
			if (propertyDescriptorsCache.ContainsKey(key))
				return propertyDescriptorsCache[key];
			else {
				PropertyDescriptor descriptor = GetPropertyDescriptorNonCacheable(obj, propertyName);
				lock (propertyDescriptorsCache) {
					if (!propertyDescriptorsCache.ContainsKey(key))
						propertyDescriptorsCache.Add(key, descriptor);
				}
				return descriptor;
			}
		}
		private static bool TryToExtractValueFromDescriptor(object source, PropertyDescriptor descriptor, out object value) {
			if (descriptor != null) {
				value = descriptor.GetValue(source);
				return true;
			} else {
				value = null;
				return false;
			}
		}
		private static PropertyDescriptor GetPropertyDescriptorNonCacheable(object obj, string propertyName) {
			if(obj != null && !string.IsNullOrEmpty(propertyName)) {
				if(propertyName.Contains("."))
					return new DevExpress.Data.Access.ComplexPropertyDescriptorReflection(obj, propertyName);
				if (ExpandoPropertyDescriptor.IsDynamicType(obj.GetType()))
					return ExpandoPropertyDescriptor.GetProperty(propertyName, obj, obj.GetType());
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
				if(properties != null)
					return properties.Find(propertyName, true);
			}
			return null;
		}
		private static bool IsPropertyExistNonCacheable(object obj, string propertyName) {
			return GetPropertyDescriptorNonCacheable(obj, propertyName) != null;
		}
		private static bool TryToGetPropertyValueNonCacheable(object source, string propertyName, out object value) {
			return TryToExtractValueFromDescriptor(source, GetPropertyDescriptorNonCacheable(source, propertyName), out value);
		}
		public static Type StripNullableType(Type type) {
			if (type == null)
				return null;
			Type underlying = Nullable.GetUnderlyingType(type);
			if (underlying != null)
				return underlying;
			return type;
		}
		public static bool IsGenericIEnumerable(Type type) {
			return ExtractGenericIEnumerable(type) != null;
		}
		public static Type ExtractGenericIEnumerable(Type type) {
			Func<Type, bool> isGenericIEnumerable = t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);
			return (isGenericIEnumerable(type)) ? type : type.GetInterfaces().FirstOrDefault(isGenericIEnumerable);
		}
		#region Nested types
		private struct TypePropertyPair {
			private Type type;
			private string propertyName;
			public TypePropertyPair(Type type, string propertyName) {
				this.type = type;
				this.propertyName = propertyName;
			}
			public bool IsEmpty {
				get { return this.type == null || this.propertyName == null; }
			}
			public string PropertyName {
				get { return this.propertyName; }
			}
			public Type Type {
				get { return this.type; }
			}
			public override bool Equals(object obj) {
				if (obj is TypePropertyPair) {
					TypePropertyPair pair = (TypePropertyPair)obj;
					if (pair.Type.Equals(this.Type) && pair.PropertyName == this.PropertyName) {
						return pair.GetType().Equals(base.GetType());
					}
				}
				return false;
			}
			public override int GetHashCode() {
				return type.GetHashCode() ^ propertyName.GetHashCode();
			}
		}
		#endregion
	}
	public class UnitUtils {
		private static readonly double FontDPI = 96; 
		private static readonly double FontMediumSize = 12; 
		private static readonly double FontSmallDenominator = 15;
		private static readonly double FontMediumDenominator = 19; 
		private static readonly double FontLargeDenominator = 20;
		private static readonly double FontXSmallDenominator = 12;
		private static readonly double FontXXSmallDenominator = 12;
		private static readonly double FontXLargeDenominator = 27;
		private static readonly double FontXXLargeDenominator = 36;
		public static void CheckNegativeUnit(Unit value, string propertyName) {
			CheckNegativeUnit(value, propertyName, false);
		}
		public static void CheckNegativeUnit(Unit value, string propertyName, bool allowOnlyPixels) {
			if (allowOnlyPixels && value.Type != UnitType.Pixel)
				throw new ArgumentOutOfRangeException("value",
					string.Format(StringResources.InvalidUnitType, propertyName));
			CommonUtils.CheckNegativeValue(value.Value, propertyName);
		}
		public static void CheckNegativeOrZeroUnit(Unit value, string propertyName) {
			CommonUtils.CheckNegativeOrZeroValue(value.Value, propertyName);
		}
		public static Unit GetPositiveUnit(double value, UnitType type) {
			return new Unit((value > 0) ? value : 0, type);
		}
		public static Unit GetCorrectedHeight(Unit height, AppearanceStyleBase style, Paddings paddings) {
			if (!height.IsEmpty) {
				double heightValue = height.Value;
				if (!style.GetBorderWidthTop().IsEmpty && style.GetBorderWidthTop().Type == height.Type &&
					style.GetBorderStyleTop() != BorderStyle.None)
					heightValue -= style.GetBorderWidthTop().Value;
				if (!style.GetBorderWidthBottom().IsEmpty && style.GetBorderWidthBottom().Type == height.Type &&
					style.GetBorderStyleBottom() != BorderStyle.None)
					heightValue -= style.GetBorderWidthBottom().Value;
				if (!paddings.GetPaddingTop().IsEmpty && paddings.GetPaddingTop().Type == height.Type)
					heightValue -= paddings.GetPaddingTop().Value;
				if (!paddings.GetPaddingBottom().IsEmpty && paddings.GetPaddingBottom().Type == height.Type)
					heightValue -= paddings.GetPaddingBottom().Value;
				return GetPositiveUnit(heightValue, height.Type);
			}
			return height;
		}
		public static Unit GetCorrectedWidth(Unit width, AppearanceStyleBase style, Paddings paddings) {
			if (!width.IsEmpty) {
				double widthValue = width.Value;
				if (!style.GetBorderWidthLeft().IsEmpty && style.GetBorderWidthLeft().Type == width.Type &&
					style.GetBorderStyleLeft() != BorderStyle.None)
					widthValue -= style.GetBorderWidthLeft().Value;
				if (!style.GetBorderWidthRight().IsEmpty && style.GetBorderWidthRight().Type == width.Type &&
					style.GetBorderStyleRight() != BorderStyle.None)
					widthValue -= style.GetBorderWidthRight().Value;
				if (!paddings.GetPaddingLeft().IsEmpty && paddings.GetPaddingLeft().Type == width.Type)
					widthValue -= paddings.GetPaddingLeft().Value;
				if (!paddings.GetPaddingRight().IsEmpty && paddings.GetPaddingRight().Type == width.Type)
					widthValue -= paddings.GetPaddingRight().Value;
				return GetPositiveUnit(widthValue, width.Type);
			}
			return width;
		}
		public static Paddings GetSelectedCssStylePaddings(AppearanceStyleBase style, AppearanceStyleBase selectedStyle, Paddings paddings) {
			return new Paddings(
				GetSelectedCssStylePadding(style.GetBorderWidthLeft(), selectedStyle.GetBorderWidthLeft(),
				style.GetBorderStyleLeft(), selectedStyle.GetBorderStyleLeft(), paddings.GetPaddingLeft()),
				GetSelectedCssStylePadding(style.GetBorderWidthTop(), selectedStyle.GetBorderWidthTop(),
					style.GetBorderStyleTop(), selectedStyle.GetBorderStyleTop(), paddings.GetPaddingTop()),
				GetSelectedCssStylePadding(style.GetBorderWidthRight(), selectedStyle.GetBorderWidthRight(),
					style.GetBorderStyleRight(), selectedStyle.GetBorderStyleRight(), paddings.GetPaddingRight()),
				GetSelectedCssStylePadding(style.GetBorderWidthBottom(), selectedStyle.GetBorderWidthBottom(),
					style.GetBorderStyleBottom(), selectedStyle.GetBorderStyleBottom(), paddings.GetPaddingBottom()));
		}
		public static Unit GetSelectedCssStylePadding(Unit borderWidth, Unit selectedBorderWidth, BorderStyle borderStyle, BorderStyle selectedBorderStyle, Unit padding) {
			if (!selectedBorderWidth.IsEmpty && !padding.IsEmpty && selectedBorderWidth.Type == padding.Type) {
				if (!borderWidth.IsEmpty) {
					if (borderWidth.Type == padding.Type) {
						double selectedBorderWidthValue = (selectedBorderStyle != BorderStyle.None) ? selectedBorderWidth.Value : 0;
						double borderWidthValue = (borderStyle != BorderStyle.None) ? borderWidth.Value : 0;
						double unitValue = padding.Value - (selectedBorderWidthValue - borderWidthValue);
						return GetPositiveUnit(unitValue, padding.Type);
					}
				} else {
					double selectedBorderWidthValue = (selectedBorderStyle != BorderStyle.None) ? selectedBorderWidth.Value : 0;
					double unitValue = padding.Value - selectedBorderWidthValue;
					return GetPositiveUnit(unitValue, padding.Type);
				}
			}
			return padding;
		}
		public static Unit GetPaddingsSum(Unit lhs, Unit rhs) {
			if(lhs.IsEmpty)
				return rhs;
			if(rhs.IsEmpty)
				return lhs;
			if(lhs.Type != rhs.Type) {
				lhs = ConvertToPixels(lhs);
				rhs = ConvertToPixels(rhs);
			}
			return new Unit(lhs.Value + rhs.Value, lhs.Type);
		}
		public static double GetFontHeight(FontInfo font) {
			UnitType dummy = UnitType.Pixel;
			return GetFontHeight(font, out dummy);
		}
		protected static double GetFontHeight(FontInfo font, out UnitType unitType) {
			unitType = font.Size.Unit.Type;
			double fontValue = font.Size.Unit.Value;
			if (font.Size.IsEmpty) {
				unitType = UnitType.Pixel;
				fontValue = 11;
			} else {
				if (font.Size.Type != FontSize.AsUnit) {
					unitType = UnitType.Point;
					fontValue = GetLogicalFontValue(font.Size.Type);
				}
				ConvertToPixels(ref unitType, ref fontValue);
			}
			return fontValue;
		}
		public static double GetLogicalFontValue(FontSize fontSize) {
			double fontValue = FontMediumSize;
			switch (fontSize) {
				case FontSize.Small:
				case FontSize.Smaller:
					fontValue = fontValue * (FontSmallDenominator / FontMediumDenominator);
					break;
				case FontSize.Large:
				case FontSize.Larger:
					fontValue = fontValue * (FontLargeDenominator / FontMediumDenominator);
					break;
				case FontSize.XSmall:
					fontValue = fontValue * (FontXSmallDenominator / FontMediumDenominator);
					break;
				case FontSize.XXSmall:
					fontValue = fontValue * (FontXXSmallDenominator / FontMediumDenominator);
					break;
				case FontSize.XLarge:
					fontValue = fontValue * (FontXLargeDenominator / FontMediumDenominator);
					break;
				case FontSize.XXLarge:
					fontValue = fontValue * (FontXXLargeDenominator / FontMediumDenominator);
					break;
			}
			return fontValue;
		}
		public static void ConvertToPixels(ref UnitType unitType, ref double fontValue) {
			switch (unitType) {
				case UnitType.Inch:
					fontValue = FontDPI * fontValue;
					unitType = UnitType.Pixel;
					break;
				case UnitType.Point:
					fontValue = FontDPI * (fontValue / 72);
					unitType = UnitType.Pixel;
					break;
				case UnitType.Pica:
					fontValue = FontDPI * (12 * fontValue / 72);
					unitType = UnitType.Pixel;
					break;
				case UnitType.Cm:
					fontValue = FontDPI * (fontValue / 2.54);
					unitType = UnitType.Pixel;
					break;
				case UnitType.Mm:
					fontValue = FontDPI * (fontValue / 25.4);
					unitType = UnitType.Pixel;
					break;
			}
		}
		public static Unit ConvertToPixels(Unit unit) {
			if (unit.IsEmpty)
				return unit;
			UnitType resultType = unit.Type;
			double resultValue = unit.Value;
			ConvertToPixels(ref resultType, ref resultValue);
			return new Unit(resultValue, resultType);
		}
		public static bool IsUnitTypesEqual(Unit unit1, Unit unit2, params Unit[] units) {
			if (unit1.Type == unit2.Type) {
				for (int i = 0; i < units.Length; i++) {
					if (unit1.Type != units[i].Type)
						return false;
				}
				return true;
			}
			return false;
		}
	}
	public class UrlUtils {
		private static string[] AbsolutePathPrefixes = new string[] { "about:", "file:///", "ftp://", "gopher://", 
			"http://", "https://", "javascript:", "mailto:", "news:", "res://", "telnet://", "view-source:" };
		public static string AppDomainAppVirtualPathString {
			get { return HttpRuntime.AppDomainAppVirtualPath + "/"; }
		}
		public static string Combine(string appPath, string basepath, string relative) {
			string ret = "";
			if (string.IsNullOrEmpty(relative))
				throw new ArgumentNullException("relative");
			if (string.IsNullOrEmpty(basepath))
				throw new ArgumentNullException("basepath");
			if ((basepath[0] == '~') && (basepath.Length == 1))
				basepath = "~/";
			else {
				int index = basepath.LastIndexOf('/');
				if (index < (basepath.Length - 1))
					basepath = basepath.Substring(0, index + 1);
			}
			if (IsRooted(relative))
				ret = relative;
			else {
				if ((relative.Length == 1) && (relative[0] == '~'))
					return appPath;
				if (IsAppRelativePath(relative)) {
					ret = appPath.Length > 1 ? appPath + "/" + relative.Substring(2) :
						ret = "/" + relative.Substring(2);
				} else
					ret = SimpleCombine(basepath, relative);
			}
			return Reduce(ret);
		}
		public static string CombineRelativePath(string basePath, string relative) {
			while(relative.StartsWith("..\\")) {
				if(!basePath.Contains("\\"))
					throw new Exception("Base path is not available");
				basePath = Path.GetDirectoryName(basePath.TrimEnd('\\'));
				relative = relative.Substring(3);
			}
			return Path.Combine(basePath, relative);
		}
		public static bool IsAbsoluteVirtualPath(string virtualPath) {
			return virtualPath != "" ? virtualPath[0] == '/' : false;
		}
		public static bool IsAbsolutePhysicalPath(string path) {
			if ((path == null) || (path.Length < 3))
				return false;
			if ((path[1] == ':') && IsDirectorySeparatorChar(path[2]))
				return true;
			return IsUncSharePath(path);
		}
		public static bool IsAppRelativePath(string path) {
			if ((path.Length > 0) && (path[0] == '~'))
				return (path.Length == 1) || IsRooted(path.Substring(1));
			else
				return false;
		}
		public static bool IsAbsoluteUrl(string url) {
			if (url != "") {
				foreach (string prefix in AbsolutePathPrefixes)
					if (url.StartsWith(prefix))
						return true;
			}
			return false;
		}
		public static bool IsCurrentUrl(string resolvedUrl) {
			return IsCurrentUrl(resolvedUrl, false);
		}
		public static bool IsCurrentUrl(string resolvedUrl, bool ignoreQueryString) {
			if (HttpContext.Current == null)
				return false;
			if (IsAbsoluteUrl(resolvedUrl)) {
				string rawUrl = GetAbsoluteUrlFromRawUrl(HttpContext.Current.Request.RawUrl,
					HttpContext.Current.Request.Url.Host);
				string absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
				return CompareUrls(HttpContext.Current.Response.ApplyAppPathModifier(rawUrl), resolvedUrl, ignoreQueryString) ||
					CompareUrls(HttpContext.Current.Response.ApplyAppPathModifier(absoluteUri), resolvedUrl, ignoreQueryString);
			} else {
				int pos = resolvedUrl.IndexOf("/?");
				if (pos != -1)
					resolvedUrl = resolvedUrl.Substring(pos + 1);
				string curentRawUrl =  HttpContext.Current.Response.ApplyAppPathModifier(HttpContext.Current.Request.RawUrl);
				string currentUrl = HttpContext.Current.Response.ApplyAppPathModifier(HttpContext.Current.Request.Url.PathAndQuery);
				if (resolvedUrl.Trim().StartsWith("?")) { 
					curentRawUrl = GetQueryFromRawUrl(curentRawUrl);
					currentUrl = HttpContext.Current.Request.Url.Query;
				}
				return CompareUrls(curentRawUrl, resolvedUrl, ignoreQueryString) ||
					CompareUrls(currentUrl, resolvedUrl, ignoreQueryString);
			}
		}
		public static string GetRootRelativeUrl(string url) {
			return GetRelativeUri(url, !IsRootRelativeUrl(url), true);
		}
		public static string GetPathRelativeUrl(string url) {
			return GetRelativeUri(url, !IsPathRelativeUrl(url), false);
		}
		static string GetRelativeUri(string url, bool isValid, bool isRootRelative) {
			if(CanProcessUrl(url) && isValid) {
				Uri requestURI = HttpContext.Current.Request.Url;
				Uri urlURI = new Uri(url, UriKind.RelativeOrAbsolute);
				if(!urlURI.IsAbsoluteUri || urlURI.Host == requestURI.Host && urlURI.Scheme == requestURI.Scheme) {
					urlURI = urlURI.IsAbsoluteUri ? urlURI : new Uri(requestURI, urlURI);
					return isRootRelative ? urlURI.PathAndQuery : GetPathRelativeUrl(urlURI);
				}
			}
			return url;
		}
		static string GetPathRelativeUrl(Uri urlUri) {
			return BuildPathRelativeUrl(HttpContext.Current.Request.Url.Segments.Where(x => x.EndsWith("/")).ToArray(), urlUri.Segments, 0, 0, "");
		}
		static string BuildPathRelativeUrl(string[] requestSegments, string[] urlSegments, int reqIndex, int urlIndex, string buffer) {
			if(urlIndex >= urlSegments.Length)
				return buffer;
			if(reqIndex >= requestSegments.Length)
				return BuildPathRelativeUrl(requestSegments, urlSegments, reqIndex, urlIndex + 1, buffer + urlSegments[urlIndex]);
			if(requestSegments[reqIndex] == urlSegments[urlIndex] && urlIndex == reqIndex)
				return BuildPathRelativeUrl(requestSegments, urlSegments, reqIndex + 1, urlIndex + 1, buffer);
			return BuildPathRelativeUrl(requestSegments, urlSegments, reqIndex + 1, urlIndex, buffer + "../");
		}
		public static bool IsPathRelativeUrl(string url) {
			return !string.IsNullOrEmpty(url) && !IsAbsoluteUrl(url) && !url.StartsWith("/");
		}
		public static bool IsRootRelativeUrl(string url) {
			return !string.IsNullOrEmpty(url) && !IsAbsoluteUrl(url) && url.StartsWith("/") && !url.StartsWith("//");
		}
		public static string GetAbsoluteUrl(string url) {
			if(!CanProcessUrl(url))
				return url;
			string relativeScheme = "//";
			if(!IsAbsoluteUrl(url) && !url.StartsWith(relativeScheme)) {
				Uri uri = HttpContext.Current.Request.Url;
				if(!url.StartsWith("/")) {
					string absolutePath = uri.AbsolutePath;
					if(!HasTrailingSlash(absolutePath))
						absolutePath = absolutePath.Remove(absolutePath.LastIndexOf("/"));
					url = SimpleCombine(absolutePath, url);
				}
				url = uri.Scheme + ":" + relativeScheme + Reduce(SimpleCombine(uri.Authority, url));
			} else
				url = ReduceVirtualPath(url);
			return url;
		}
		static bool CanProcessUrl(string url) {
			return !(string.IsNullOrEmpty(url) || HttpContext.Current == null || Regex.IsMatch(url, "data:([^;]+/?[^;]*)(;charset=[^;]*)?(;base64,)"));
		}
		protected static bool CompareUrls(string currentUrl, string resolvedUrl, bool ignoreQueryString) {
			if (ignoreQueryString) {
				int pos = currentUrl.IndexOf("?", StringComparison.Ordinal);
				if (pos != -1)
					currentUrl = currentUrl.Substring(0, pos);
			}
			return HttpUtility.UrlDecode(resolvedUrl.ToLower()) == HttpUtility.UrlDecode(currentUrl.ToLower());
		}
		protected static string GetAbsoluteUrlFromRawUrl(string rawUrl, string host) {
			string path = rawUrl;
			string query = "";
			int pos = rawUrl.IndexOf("?", StringComparison.Ordinal);
			if (pos != -1) {
				path = rawUrl.Substring(0, pos);
				query = rawUrl.Substring(pos + 1);
			}
			UriBuilder b = new UriBuilder();
			b.Host = host;
			b.Path = path;
			b.Query = query;
			return b.Uri.AbsoluteUri;
		}
		protected static string GetQueryFromRawUrl(string rawUrl) {
			string ret = "";
			int pos = rawUrl.IndexOf("?", StringComparison.Ordinal);
			if (pos != -1)
				ret = rawUrl.Substring(pos);
			return ret;
		}
		public static bool IsRelativeUrl(string virtualPath) {
			if (virtualPath.IndexOf(":", StringComparison.Ordinal) != -1)
				return false;
			return !IsRooted(virtualPath);
		}
		public static string ResolvePhysicalPath(string relativePath) {
			string path = relativePath ?? string.Empty;
			if(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath == null)
				return path;
			if(path.Length > 0 && path.Substring(1).StartsWith(":\\") || path.StartsWith("\\\\")) 
				return path;
			path = path.StartsWith(".\\") ? "~" + path.Substring(1) : path;
			if(path.StartsWith("~"))
				return System.Web.Hosting.HostingEnvironment.MapPath(path);
			return Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, relativePath.Trim('\\', '/'));
		}
		public static string MakeVirtualPathAppAbsolute(string virtualPath) {
			string ret = virtualPath;
			if (HttpRuntime.AppDomainAppId != null)
				ret = MakeVirtualPathAppAbsolute(virtualPath, HttpRuntime.AppDomainAppVirtualPath);
			return ret;
		}
		public static string MakeVirtualPathAppAbsolute(string virtualPath, string applicationPath) {
			string ret = virtualPath;
			if ((virtualPath.Length == 1) && (virtualPath[0] == '~'))
				ret = applicationPath;
			else {
				if (((virtualPath.Length >= 2) && (virtualPath[0] == '~')) && ((virtualPath[1] == '/') || (virtualPath[1] == '\\')))
					ret = applicationPath.Length > 1 ? applicationPath + "/" + virtualPath.Substring(2) :
						"/" + virtualPath.Substring(2);
				else {
					if (!IsRooted(virtualPath))
						new ArgumentException(string.Format(StringResources.InvalidVirtualPath, virtualPath));
				}
			}
			return ret;
		}
		internal static string FixVirtualPathSlashes(string virtualPath) {
			string ret = "";
			virtualPath = Replace(virtualPath, '\\', '/');
			string newVirtualPath = virtualPath.Replace("///", "/");
			newVirtualPath = newVirtualPath.Replace("//", "/");
			if (newVirtualPath == virtualPath)
				ret = virtualPath;
			ret = newVirtualPath;
			return ret;
		}
		protected static bool HasTrailingSlash(string virtualPath) {
			if(virtualPath.Length == 0)
				return false;
			return (virtualPath[virtualPath.Length - 1] == '/');
		}
		protected static bool IsDirectorySeparatorChar(char ch) {
			if (ch != '\\')
				return (ch == '/');
			return true;
		}
		protected static bool IsRooted(string basepath) {
			return !string.IsNullOrEmpty(basepath) && ((basepath[0] == '\\') || (basepath[0] == '/'));
		}
		protected static bool IsUncSharePath(string path) {
			return ((path.Length > 2) && IsDirectorySeparatorChar(path[0])) && IsDirectorySeparatorChar(path[1]);
		}
		protected static string Reduce(string path) {
			string pathPrefix = "";
			if (path != null) {
				int index = path.IndexOf('?');
				if (index >= 0) {
					pathPrefix = path.Substring(index);
					path = path.Substring(0, index);
				}
			}
			path = FixVirtualPathSlashes(path);
			path = ReduceVirtualPath(path);
			if (pathPrefix == "")
				return path;
			return (path + pathPrefix);
		}
		protected static string ReduceVirtualPath(string path) {
			int length = path.Length;
			int dotIndex = 0;
			while (true) { 
				dotIndex = path.IndexOf('.', dotIndex);
				if (dotIndex < 0)
					return path;
				if (((dotIndex == 0) || (path[dotIndex - 1] == '/')) && ((((dotIndex + 1) == length) ||
					(path[dotIndex + 1] == '/')) || ((path[dotIndex + 1] == '.') &&
					(((dotIndex + 2) == length) || (path[dotIndex + 2] == '/')))))
					break;
				dotIndex++;
			}
			ArrayList dirs = new ArrayList();
			StringBuilder strBuilder = new StringBuilder();
			dotIndex = 0;
			while (true) {
				int slashIndex = dotIndex;
				dotIndex = path.IndexOf('/', slashIndex + 1);
				if (dotIndex < 0)
					dotIndex = length;
				if ((((dotIndex - slashIndex) <= 3) && ((dotIndex < 1) || (path[dotIndex - 1] == '.'))) && (((slashIndex + 1) >= length) || (path[slashIndex + 1] == '.'))) {
					if ((dotIndex - slashIndex) == 3) {
						if (dirs.Count == 0)
							throw new HttpException("Cannot_exit_up_top_directory");
						if ((dirs.Count == 1) && IsAppRelativePath(path))
							return ReduceVirtualPath(MakeVirtualPathAppAbsolute(path));
						strBuilder.Length = (int)dirs[dirs.Count - 1];
						dirs.RemoveRange(dirs.Count - 1, 1);
					}
				} else {
					dirs.Add(strBuilder.Length);
					strBuilder.Append(path, slashIndex, dotIndex - slashIndex);
				}
				if (dotIndex == length) {
					string newVirtualPath = strBuilder.ToString();
					if (newVirtualPath.Length != 0)
						return newVirtualPath;
					if ((length > 0) && (path[0] == '/'))
						return "/";
					return ".";
				}
			}
		}
		protected static string Replace(string s, char c1, char c2) {
			int index = s.IndexOf(c1);
			if (index < 0)
				return s;
			return s.Replace(c1, c2);
		}
		protected static string SimpleCombine(string basepath, string relative) {
			if (HasTrailingSlash(basepath))
				return (basepath + relative);
			return (basepath + "/" + relative);
		}
		public static void ValidateFolderUrl(ref string url) {
			if (string.IsNullOrEmpty(url) || IsAbsoluteUrl(url)) return;
			if(url.EndsWith("\\"))
				url = url.Substring(0, url.Length - 1) + "/";
			else if(!url.EndsWith("/"))
				url += "/";
		}
		public static string ResolvePhysicalPath(IUrlResolutionService rs, string physicalPath) {
			string appRelativePath = string.Empty;
			if(TryGetAppRelativePath(physicalPath, ref appRelativePath))
				return rs.ResolveClientUrl(appRelativePath);
			throw new ArgumentException("Cannot resolve a specified physical path to a relative path.");
		}
		public static bool TryGetAppRelativePath(string path, ref string result) {
			string applicationAbsolutePath = new DirectoryInfo(System.Web.Hosting.HostingEnvironment.MapPath("~/")).FullName;
			if(path.StartsWith(applicationAbsolutePath)) {
				result = "~/" + path.Substring(applicationAbsolutePath.Length);
				return true;
			}
			result = path;
			return false;
		}
		public static string GetAppRelativePath(string path) {
			TryGetAppRelativePath(path, ref path);
			return path;
		}
		public static string ToAppRelative(string virtualPath) {
			int i = virtualPath.IndexOf('?');
			string validVirtualPath = i > -1 ? virtualPath.Substring(0, i) : virtualPath;
			string appRelativePath = VirtualPathUtility.ToAppRelative(validVirtualPath);
			if(i > -1)
				appRelativePath += virtualPath.Substring(i);
			return appRelativePath;
		}
		public static string GetPhysicalPath(string relativePath) {
			if(!string.IsNullOrEmpty(relativePath)) {
				string path = relativePath;
				if(path[0] != '~') {
					if(path[0] == '\\')
						path = "~/" + path.Substring(1);
					else if(path[0] == '/')
						path = "~" + path;
					else
						path = "~/" + path;
				}
				else if(path[1] == '\\')
					path = "~/" + path.Substring(2);
				return HttpContext.Current.Server.MapPath(path);
			}
			return "";
		}
		public static string NormalizeRelativePath(string relativePath) {
			UrlUtils.ValidateFolderUrl(ref relativePath);
			return Regex.Replace(relativePath, @"/+|^\.\./", "\\");
		}
	}
	public class RegExConst {
		public const string
			RegExPatchPublicKeyToken = @",\s*PublicKeyToken\s*=\s*(?<KeyToken>[\w]+)",
			RegExUpgradeVersionAndToken = @",\s*Version\s*=\s*[\w\.]+",
			RegExUpgradeAssemblyShortName = @"^[a-zA-Z\.]+(?<Version>.v\d\d\.\d)[,\.]",
			RegExRegisterASPX = @"<%@\s*Register([^"">]|(""[^""]*""))+Assembly=""(?<AssemblyName>[^""]+)""[^%]*\s*(%>){1}",
			RegExAssemblyASPX = @"<%@\s*Assembly([^"">]|(""[^""]*""))+Name=""(?<AssemblyName>[^""]+)""[^%]*\s*(%>){1}",
			RegExCodeFileASPX = "(<%@.*Control.*)(CodeFile=)([^<]*%>)",
			RegExInheritsASPX = "(<%@.*Control.*)\\s(Inherits=\")([^<]*%>)",
			RegExCodeBehindASPX = "(<%@.*Control.*)\\s(Codebehind=\")([^<]*%>)";
	}
	public interface IEditFormNotificationOwner {
		void Changed(string propertyName);
	}
	public static class SerializationUtils {
		static object fakeSerializableObject = new object();
		public static T GetSerializableInnerPropertyObject<T>(T value, Func<T> getDefaultValue, bool useDefaultValueIfNull) {
			if(value != null || !useDefaultValueIfNull)
				return value;
			return getDefaultValue();
		}
	}
	public static class DesignServices {
		public static void ComponentChanging(IServiceProvider serviceProvider, object component) {
			IComponentChangeService changeService = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			changeService.OnComponentChanging(component, null);
		}
		public static void ComponentChanged(IServiceProvider serviceProvider, object component) {
			IComponentChangeService changeService = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			changeService.OnComponentChanged(component, null, null, null);
		}
	}
	public class FakeDesignerHost : IDesignerHost {
		#region IDesignerHost Members
		void IDesignerHost.Activate() {
		}
		IContainer IDesignerHost.Container {
			get { return null; }
		}
		IComponent IDesignerHost.CreateComponent(Type componentClass, string name) {
			return null;
		}
		IComponent IDesignerHost.CreateComponent(Type componentClass) {
			return null;
		}
		DesignerTransaction IDesignerHost.CreateTransaction(string description) {
			return null;
		}
		DesignerTransaction IDesignerHost.CreateTransaction() {
			return null;
		}
		void IDesignerHost.DestroyComponent(IComponent component) {
		}
		IDesigner IDesignerHost.GetDesigner(IComponent component) {
			return null;
		}
		Type IDesignerHost.GetType(string typeName) {
			return null;
		}
		bool IDesignerHost.InTransaction {
			get { return false; }
		}
		bool IDesignerHost.Loading {
			get { return false; }
		}
		IComponent IDesignerHost.RootComponent {
			get { return null; }
		}
		string IDesignerHost.RootComponentClassName {
			get { return string.Empty; }
		}
		string IDesignerHost.TransactionDescription {
			get { return string.Empty; }
		}
#pragma warning disable
		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler LoadComplete;
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;
#pragma warning restore
		#endregion
		#region IServiceContainer Members
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
		}
		void IServiceContainer.RemoveService(Type serviceType) {
		}
		#endregion
		[SecuritySafeCritical]
		public virtual object GetService(Type serviceType) {
			return null;
		}
	}
	public static class AspxCodeUtils {
		public static string GetPublicKeyToken(Type type) {
			return GetPublicKeyToken(Assembly.GetAssembly(type).FullName);
		}
		public static string GetPublicKeyToken(string assmFullName) {
			string result = new Regex("(?i)publickeytoken(?-i)[\\s]*=[\\s]*[^,]*($|,)").Match(assmFullName).Value;
			return Regex.Replace(result.Replace(" ", string.Empty), "(?i)publickeytoken(?-i)=", string.Empty).Replace(",", string.Empty);
		}
		public static string GetPublicKeyToken(AssemblyName assmName) {
			string key = "";
			byte[] bKey = assmName.GetPublicKeyToken();
			for (int b = 0; b < bKey.GetLength(0); b++)
				key += string.Format("{0:x2}", bKey[b]);
			return key;
		}
		public static void SeparateContent(ref string content, ref string directives) {
			Regex regex = new Regex(@"\<%@[^(%\>)]+%\>", RegexOptions.Singleline);
			MatchCollection matches = regex.Matches(content);
			foreach(Match match in matches) {
				directives += match.Value + Environment.NewLine;
				content = content.Replace(match.Value, "");
			}
		}
		public static string GetDirectives(string directives, string newTagPrefix) {
			Regex regex = new Regex("TagPrefix=\\\".+?\\\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
			return regex.Replace(directives, "TagPrefix=\"" + newTagPrefix + "\"");
		}
		public static string GetPatchedDxControlVersionAndTokenInContent(string content) {
			return GetPatchedDxControlVersionAndTokenInContent(content, false);
		}
		public static string GetPatchedDxControlVersionAndTokenInContent(string content, bool pathAssemblyName) {
			content = Regex.Replace(content, RegExConst.RegExRegisterASPX, new MatchEvaluator(MatchPatcher), RegexOptions.IgnoreCase);
			content = Regex.Replace(content, RegExConst.RegExAssemblyASPX, new MatchEvaluator(MatchPatcher), RegexOptions.IgnoreCase);
			if(pathAssemblyName) {
				content = Regex.Replace(content, RegExConst.RegExRegisterASPX, new MatchEvaluator(MatchPatcherWithAssemblyName), RegexOptions.IgnoreCase);
				content = Regex.Replace(content, RegExConst.RegExAssemblyASPX, new MatchEvaluator(MatchPatcherWithAssemblyName), RegexOptions.IgnoreCase);
			}
			return content;
		}
		static string MatchPatcher(Match match) {
			string assemblyName = match.Groups["AssemblyName"].Value;
			assemblyName = Regex.Replace(assemblyName, RegExConst.RegExUpgradeVersionAndToken,
				", Version=" + AssemblyInfo.Version, RegexOptions.IgnoreCase);
			string replacement = ", PublicKeyToken=" + AspxCodeUtils.GetPublicKeyToken(typeof(AspxCodeUtils)).ToLower();
			assemblyName = Regex.Replace(assemblyName, RegExConst.RegExPatchPublicKeyToken, replacement, RegexOptions.IgnoreCase);
			return match.Value.Replace(match.Groups["AssemblyName"].Value, assemblyName);
		}
		static string MatchPatcherWithAssemblyName(Match match) {
			string assemblyName = match.Groups["AssemblyName"].Value;
			assemblyName = Regex.Replace(assemblyName, RegExConst.RegExUpgradeAssemblyShortName, new MatchEvaluator(AssemblyNameMatchPatcher));
			return match.Value.Replace(match.Groups["AssemblyName"].Value, assemblyName);
		}
		static string AssemblyNameMatchPatcher(Match match) {
			string assemblyVersion = match.Groups["Version"].Value;
			return match.Value.Replace(assemblyVersion, AssemblyInfo.VSuffix);
		}
	}
	public class FileUtils {
		public static void CopyFileFromResourceToFile(Assembly assembly, string resourceName,
			string targetFileName) {
			using(Stream fromStream = assembly.GetManifestResourceStream(resourceName)) {
				BinaryReader br = new BinaryReader(fromStream);
				using(Stream toStream = File.Create(targetFileName)) {
					BinaryWriter bw = new BinaryWriter(toStream);
					bw.Write(br.ReadBytes((int)fromStream.Length));
				}
			}
		}
		public static string GetFileText(string filePath) {
			string ret = "";
			using(FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
				using(StreamReader sr = new StreamReader(fileStream)) {
					fileStream.Seek(0, SeekOrigin.Begin);
					ret = sr.ReadToEnd();
				}
			}
			return ret;
		}
		public static string GetResourceFileText(Assembly assembly, string resourceName) {
			string ret = "";
			using(Stream fromStream = assembly.GetManifestResourceStream(resourceName)) {
				using(StreamReader sr = new StreamReader(fromStream)) {
					fromStream.Seek(0, SeekOrigin.Begin);
					ret = sr.ReadToEnd();
				}
			}
			return ret;
		}
		public static string GenerateTempFileName() {
			Guid guid = Guid.NewGuid();
			return guid.ToString() + ".tmp";
		}
		public static void SetFileText(string filePath, string text) {
			for(int i = 0; i < 5; i++) {
				try {
					using(StreamWriter sw = new StreamWriter(filePath))
						sw.Write(text);
				}
				catch {
					System.Threading.Thread.Sleep(1000);
				}
			}
		}
		public static void EnsureDirectoryInFileSystemCreated(string folderName, bool isHidden) {
			if(!Directory.Exists(folderName)) {
				Directory.CreateDirectory(folderName);
				if(isHidden) {
					DirectoryInfo di = new DirectoryInfo(folderName);
					di.Attributes = FileAttributes.Hidden;
				}
			}
		}
		public static void CheckOrCreateDirectory(string path, WebControl control, string propertyName) {
			if(string.IsNullOrWhiteSpace(path))
				throw new HttpException(GetControlExceptionMessage(StringResources.FileUtils_PathCannotBeEmpty, control, propertyName));
			string resolvedPath = UrlUtils.ResolvePhysicalPath(path);
			try {
				EnsureDirectoryInFileSystemCreated(resolvedPath, false);
			}
			catch {
				throw new HttpException(GetControlExceptionMessage(StringResources.FileUtils_ControlCannotCreateFolder, control, path));
			}
			bool hasAccess = true;
			try {
				hasAccess = CheckDirectoryAccess(resolvedPath);
			}
			catch(SecurityException) { }
			catch(Exception) {
				hasAccess = false;
			}
			if(!hasAccess)
				throw new HttpException(GetControlExceptionMessage(StringResources.FileUtils_ControlHasNoAccessToPath, control, path));
		}
		static bool CheckDirectoryAccess(string path) {
			DirectoryInfo dir = new DirectoryInfo(path);
			DirectorySecurity sec = dir.GetAccessControl(AccessControlSections.Access);
			System.Security.AccessControl.AuthorizationRuleCollection rules = sec.GetAccessRules(true, true, typeof(SecurityIdentifier));
			FileSystemRights rightsToCheck = FileSystemRights.Read | FileSystemRights.Write
											 | FileSystemRights.Delete | FileSystemRights.ListDirectory;
			WindowsIdentity user = WindowsIdentity.GetCurrent();
			foreach(FileSystemAccessRule rule in rules) {
				if((rule.FileSystemRights & rightsToCheck) > 0) {
					SecurityIdentifier sid = (SecurityIdentifier)rule.IdentityReference;
					if((sid.IsAccountSid() && user.User == sid) ||
					  (!sid.IsAccountSid() && user.Groups.Contains(sid))) {
						if(rule.AccessControlType == AccessControlType.Deny)
							return false;
					}
				}
			}
			return true;
		}
		static string GetControlExceptionMessage(string template, WebControl control, string details) {
			return string.Format(template, control.ID, control.GetType().Name, details);
		}
	}
	public static class TouchUtils {
		public static string TouchMouseDownEventName{
			get { return RenderUtils.Browser.Platform.IsWebKitTouchUI ? "ontouchstart" : "onmousedown"; }
		}
		public static string TouchMouseUpEventName{
			get { return RenderUtils.Browser.Platform.IsWebKitTouchUI ? "ontouchend" : "onmouseup"; }
		}
	}
	public static class ClientIDHelper {
		class TemplateContainerHolder : Control, INamingContainer {
			private TemplateContainerBase templateContainer;
			protected TemplateContainerBase TemplateContainer {
				get { return templateContainer; }
			}
			public TemplateContainerHolder(TemplateContainerBase container) {
				this.templateContainer = container;
				Controls.Add(TemplateContainer);
				SetClientIDMode(this, "Predictable");
				SetClientIDMode(TemplateContainer, "Predictable");
			}
			public override Control FindControl(string id) {
				return TemplateContainer.FindControl(id);
			}
			public override string ClientID {
				get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
			}
		}
		public static string GenerateClientID(Control ctrl, string baseClientID) {
			if(GetClientIDModeValue(ctrl) == "Static")
				return baseClientID;
			if(ctrl is IDataItemContainer && GetClientIDMode(ctrl) == "Predictable")
				return ctrl.NamingContainer.ClientID;
			if(ctrl.NamingContainer != null && ctrl.NamingContainer != ctrl.Page) {
				string containerClientID = ctrl.NamingContainer.ClientID;
				if(!string.IsNullOrEmpty(containerClientID))
					return string.Concat(containerClientID, "_", GetIDFromUniqueID(ctrl.UniqueID, ctrl.NamingContainer.UniqueID));
			}
			return ctrl.UniqueID;
		}
		static string GetIDFromUniqueID(string uniqueID, string namingContainerUniqueID) {
			return uniqueID.StartsWith(namingContainerUniqueID)
				? uniqueID.Substring(namingContainerUniqueID.Length + 1)
				: uniqueID;
		}
		public static void EnableClientIDGeneration(ASPxWebControlBase control) {
			control.GenerateClientID = true;
		}
		public static void DisableClientIDGeneration(ASPxWebControlBase control) {
			control.GenerateClientID = false;
		}
		public static void UpdateClientIDMode(UserControl control) {
			if(ClientIDModePropertyExists() && NeedUpdateClientIDRecursive(control.Parent))
				SetClientIDMode(control, "Predictable");
		}
		public static void UpdateClientIDMode(ASPxWebControlBase control) {
			if(ClientIDModePropertyExists() && (control.GenerateClientID || NeedUpdateClientIDRecursive(control.Parent))) {
				if(control is INamingContainer)
					SetClientIDMode(control, "Predictable");
				EnableClientIDGeneration(control);
			}
		}
		static bool NeedUpdateClientIDRecursive(Control parent) {
			if(parent is TemplateContainerBase || parent is ContentControl || parent is Page || parent == null)
				return false;
			return parent is ASPxWebControlBase
				? NeedSetClientIDMode(parent)
				: NeedUpdateClientIDRecursive(parent.Parent);
		}
		static bool NeedSetClientIDMode(Control namingContainer) {
			string clientIDMode = GetClientIDMode(namingContainer);
			return clientIDMode == "Static" || clientIDMode == "Predictable";
		}
		static System.Reflection.PropertyInfo clientIDModePropertyInfo = null;
		static Func<Control, string> getClientIDModeValueCore = null;
		static bool clientIDModePropertyInfoRetrieved = false;
		static bool ClientIDModePropertyExists() {
			if(!clientIDModePropertyInfoRetrieved) {
				clientIDModePropertyInfo = typeof(Control).GetProperty("ClientIDMode");
				if(clientIDModePropertyInfo != null)
					getClientIDModeValueCore = CreateGetClientIDModeDynamicMethod(clientIDModePropertyInfo.GetGetMethod());
				clientIDModePropertyInfoRetrieved = true;
			}
			return clientIDModePropertyInfo != null;
		}
		static Func<Control, string> CreateGetClientIDModeDynamicMethod(MethodInfo getter) { 
			System.Reflection.Emit.DynamicMethod method = new System.Reflection.Emit.DynamicMethod("GetClientIDModeDynamic", typeof(string), new Type[] { typeof(Control) });
			System.Reflection.Emit.ILGenerator il = method.GetILGenerator();
			il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
			il.EmitCall(System.Reflection.Emit.OpCodes.Callvirt, getter, null);
			il.Emit(System.Reflection.Emit.OpCodes.Box, getter.ReturnType);
			il.EmitCall(System.Reflection.Emit.OpCodes.Callvirt, typeof(object).GetMethod("ToString"), null);
			il.Emit(System.Reflection.Emit.OpCodes.Ret);
			return (Func<Control, string>)method.CreateDelegate(typeof(Func<Control, string>));
		}
		static string GetClientIDModeValue(Control control) {
			return ClientIDModePropertyExists()
				? getClientIDModeValueCore(control)
				: null;
		}
		static void SetClientIDMode(Control control, string clientIDMode) {
			if(ClientIDModePropertyExists())
				clientIDModePropertyInfo.SetValue(control, Enum.Parse(clientIDModePropertyInfo.PropertyType, clientIDMode), null);
		}
		static string GetClientIDMode(Control control) {
			string clientIDMode = GetClientIDModeValue(control);
			if(clientIDMode == "Inherit") {
				if(control.NamingContainer != null)
					return GetClientIDMode(control.NamingContainer);
				if(control is Page) {
					PagesSection section = WebConfigurationManager.GetSection("system.web/pages") as PagesSection;
					if(section != null)
						clientIDMode = ReflectionUtils.GetPropertyValue(section, "ClientIDMode").ToString() ?? "Predictable";
				}
			}
			return clientIDMode;
		}
		internal static void AddTemplateContainerToHierarchy(Control parent, TemplateContainerBase container, string containerID) {
			parent.Controls.Add(new TemplateHierarchyContainer(container, containerID));
		}
		class TemplateHierarchyContainer : Control, IASPxWebControl {
			TemplateContainerBase templateContainer;
			string containerID;
			protected TemplateContainerBase TemplateContainer {
				get { return templateContainer; }
			}
			public TemplateHierarchyContainer(TemplateContainerBase container, string containerID)
				: base() {
				this.templateContainer = container;
				this.containerID = containerID;
			}
			protected override void OnInit(EventArgs e) {
				EnsureChildControls();
			}
			protected override void CreateChildControls() {
				bool createHolder = ClientIDModePropertyExists() && NeedSetClientIDMode(NamingContainer);
				if(createHolder) 
					TemplateContainer.ID = "TC"; 
				Control containerControl = createHolder ? new TemplateContainerHolder(TemplateContainer) : (Control)TemplateContainer;
				containerControl.ID = this.containerID;
				Controls.Add(containerControl);
			}
			void IASPxWebControl.EnsureChildControls() {
				EnsureChildControls();
			}
			void IASPxWebControl.PrepareControlHierarchy() {
			}
		}
		public static void SetClientIDModeToAutoID(ASPxWebControlBase control) {
			if(ClientIDModePropertyExists())
				SetClientIDMode(control, "AutoID");
		}
	}
	public class DxGetMemberBinder : System.Dynamic.GetMemberBinder {
		public DxGetMemberBinder(string name, bool ignoreCase) : base(name, ignoreCase) {
		}
		public override System.Dynamic.DynamicMetaObject FallbackGetMember(System.Dynamic.DynamicMetaObject target, System.Dynamic.DynamicMetaObject errorSuggestion) {
			return errorSuggestion;
		}		
	}
	public class AccessibilityUtils {
		public const string InvisibleRowCssClassName = "dxAIR",
			InvisibleFocusableElementCssClassName = "dxAIFE",
			DefaultCursorCssClassName = "dxDefaultCursor";
		public const string AssistantID = "AcAs";
		public static string ClientCalendarMultiSelectText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_ClientCalendarMultiSelect); } }
		public static string GridViewHeaderLinkFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_GridViewHeaderLinkFormat); } }
		public static string PagerPreviousPageText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_PagerPreviousPage); } }
		public static string PagerNextPageText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_PagerNextPage); } }
		public static string PagerSummaryFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_PagerSummaryFormat); } }
		public static string PagerNavigationFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_PagerNavigationFormat); } }
		public static string PagerDescriptionText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_PagerDescription); } }
		public static string TableItemFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TableItemFormat); } }
		public static string EmptyItemText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_EmptyItem); } }
		public static string ItemPositionFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_ItemPositionFormat); } }
		public static string CheckBoxColumnHeaderText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_CheckBoxColumnHeader); } }
		public static string CheckBoxListItemFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_CheckBoxListItemFormat); } }
		public static string CalendarDescriptionText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_CalendarDescription); } }
		public static string TreeListDescriptionFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListDescriptionFormat); } }
		public static string TreeListNavigationDescriptionText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListNavigationDescription); } }
		public static string TreeListCheckBoxSelectionDescriptionText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListCheckBoxSelectionDescription); } }
		public static string TreeListHeaderLinkFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListHeaderLinkFormat); } }
		public static string TreeListSelectAllCheckBoxText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListSelectAllCheckBox); } }
		public static string TreeListCollapseExpandButtonFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListCollapseExpandButtonFormat); } }
		public static string TreeListCollapseText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListCollapse); } }
		public static string TreeListExpandText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListExpand); } }
		public static string TreeListDataCheckBoxDescriptionFormatString { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListDataCheckBoxDescriptionFormat); } }
		public static string Not { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_Not); } }
		public static string TreeListNodeExpandedStateText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListNodeExpandedState); } }
		public static string TreeListNodeCollapsedStateText { get { return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_TreeListNodeCollapsedState); } }
		public static string GetStringSortOrder(ColumnSortOrder order) {
			switch(order) {
				case ColumnSortOrder.Ascending:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_AscendingSortOrder);
				case ColumnSortOrder.Descending:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_DescendingSortOrder);
				default:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Accessibility_NoneSortOrder);
			}
		}
		public static Dictionary<string, string> CreateCheckBoxAttributes(string role, CheckState state) {
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes.Add("role", role);
			attributes.Add("aria-checked", GetStringCheckedState(state));
			return attributes;
		}
		public static string GetStringCheckedState(CheckState state) {
			switch(state) {
				case CheckState.Checked:
					return "true";
				case CheckState.Indeterminate:
					return "mixed";
				case CheckState.Unchecked:
					return "false";
				default:
					return "";
			}
		}
		public static string GetListBoxInternalCheckBoxItemLabel(List<ListBoxColumn> visibleColumns, ListEditItem item) {
			string label = "";
			for(int i = 0; i < visibleColumns.Count; i++) {
				string fieldName = visibleColumns[i].FieldName;
				object itemTextObj = item.GetValue(fieldName);
				string caption = visibleColumns[i].GetCaption();
				label += String.Format(TableItemFormatString, caption, itemTextObj != null ? itemTextObj : EmptyItemText);
			}
			return label;
		}
		public static string GetTreeListDescriptionText(int rowCount, int columnCount, bool isCallback, bool selectionEnabled) {
			string description = string.Format(TreeListDescriptionFormatString, rowCount, columnCount);
			if(!isCallback) {
				description += TreeListNavigationDescriptionText;
				if(selectionEnabled)
					description += TreeListCheckBoxSelectionDescriptionText;
			}
			return description;
		}
	}
}
namespace DevExpress.Web {
	internal class ToolboxBitmapAccess {
		public const string BitmapPath = "Bitmaps256.";
	}
}
