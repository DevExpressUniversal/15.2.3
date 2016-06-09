#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfReaderDictionary : PdfDictionary {
		const string timeZonePattern = "OHH'mm'";
		static bool IsDigit(char chr) {
			return chr >= '0' && chr <= '9';
		}
		static int ConvertToDigit(char chr) {
			if (!IsDigit(chr))
				PdfDocumentReader.ThrowIncorrectDataException();
			return chr - '0';
		}
		static int GetDateComponent(string str, int offset) {
			return ConvertToDigit(str[offset]) * 10 + ConvertToDigit(str[offset + 1]);
		}
		static int GetTimeComponent(string str, int offset, char delimiter) {
			int result = ConvertToDigit(str[offset]);
			char digit = str[offset + 1];
			return digit == delimiter ? result : (result * 10 + ConvertToDigit(digit));
		}
		static DateTimeOffset ParseDate(string str) {
			int length = str.Length;
			if (length != 8 && length != 12 && length < 14)
				PdfDocumentReader.ThrowIncorrectDataException();
			int year = ConvertToDigit(str[0]) * 1000 + ConvertToDigit(str[1]) * 100 + ConvertToDigit(str[2]) * 10 + ConvertToDigit(str[3]);
			int month = GetDateComponent(str, 4);
			int day = GetDateComponent(str, 6);
			int hour = 0;
			int minute = 0;
			int second = 0;
			int utcHours = 0;
			int utcMinutes = 0;
			if (length > 8) {
				hour = GetTimeComponent(str, 8, ':');
				minute = GetTimeComponent(str, 10, ':');
				if (length >= 14)
					second = GetTimeComponent(str, 12, ' ');
				bool isNegativeOffset = false;
				if (length > 14 && (length != 20 || str[16] != '\'' || str[19] != '\'')) {
					switch (str[14]) {
						case '-':
							isNegativeOffset = true;
							goto case '+';
						case '+':
							switch (length) {
								case 22:
									if (str[15] != '-')
										PdfDocumentReader.ThrowIncorrectDataException();
									str = str.Remove(15, 1);
									goto case 21;
								case 21:
									if (str[20] != '\'')
										PdfDocumentReader.ThrowIncorrectDataException();
									goto case 20;
								case 20:
									utcMinutes = ConvertToDigit(str[18]) * 10 + ConvertToDigit(str[19]);
									goto case 18;
								case 18:
									if (str[17] != '\'')
										PdfDocumentReader.ThrowIncorrectDataException();
									utcHours = ConvertToDigit(str[16]);
									char highHourChar = str[15];
									if (highHourChar != '+' && highHourChar != '-')
										utcHours += ConvertToDigit(highHourChar) * 10;
									break;
								default:
									if (length > 21) {
										if (str[18] == '0' && str[length - 1] == '\'') {
											bool isNegative = false;
											switch (str[19]) {
												case '+':
													break;
												case '-':
													isNegative = true;
													break;
												default:
													PdfDocumentReader.ThrowIncorrectDataException();
													break;
											}
											if (!Int32.TryParse(str.Substring(20, length - 21), out utcMinutes))
												PdfDocumentReader.ThrowIncorrectDataException();
											if (isNegative)
												utcMinutes = -utcMinutes;
											goto case 18;
										}
									}
									break;
							}
							break;
						case 'Z':
						case ' ':
							switch (length) {
								case 15:
									break;
								case 21:
									if (str[15] != '0' || str[16] != '0' || str[17] != '\'' && str[18] != '0' && str[19] != '0' && str[20] != '\'')
										PdfDocumentReader.ThrowIncorrectDataException();
									break;
								case 22:
									return ParseDate(str.Remove(14, 1));
								default:
									PdfDocumentReader.ThrowIncorrectDataException();
									break;
							}
							break;
						default:
							if (str.EndsWith(timeZonePattern))
								return ParseDate(str.Substring(0, length - timeZonePattern.Length));
							if (length != 15 || !IsDigit(str[14]))
								PdfDocumentReader.ThrowIncorrectDataException();
							year = (ConvertToDigit(str[0]) * 10 + ConvertToDigit(str[1]) + ConvertToDigit(str[2])) * 100 + ConvertToDigit(str[3]) * 10 + ConvertToDigit(str[4]);
							month = GetDateComponent(str, 5);
							day = GetDateComponent(str, 7);
							hour = GetTimeComponent(str, 9, ':');
							minute = GetTimeComponent(str, 11, ':');
							second = GetTimeComponent(str, 13, ' ');
							break;
					}
					if (utcHours > 14 || (utcHours == 14 && utcMinutes > 0)) {
						utcHours = 0;
						utcMinutes = 0;
					}
					if (isNegativeOffset) {
						utcHours = -utcHours;
						utcMinutes = -utcMinutes;
					}
				}
			}
			return new DateTimeOffset(year, month, day, hour, minute, second, new TimeSpan(utcHours, utcMinutes, 0));
		}
		public static CultureInfo ConvertToCultureInfo(string language) {
			if (!String.IsNullOrEmpty(language))
				try {
					return new CultureInfo(language);
				}
				catch {
				}
			return CultureInfo.InvariantCulture;
		}
		readonly PdfObjectCollection objects;
		int number;
		int generation;
		public PdfObjectCollection Objects { get { return objects; } }
		public int Number {
			get { return number; }
			internal set { number = value; }
		}
		public int Generation {
			get { return generation; }
			internal set { generation = value; }
		}
		public PdfReaderDictionary(PdfObjectCollection objects, int number, int generation) {
			this.objects = objects;
			this.number = number;
			this.generation = generation;
		}
		public string GetName(string key) {
			PdfName res = Resolve<PdfName>(key);
			return res == null ? null : res.Name;
		}
		public bool? GetBoolean(string key) {
			return Resolve<bool?>(key);
		}
		public int? GetInteger(string key) {
			object value;
			if (!TryGetValue(key, out value))
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				value = objects.GetObjectData(reference.Number);
			if (value is int)
				return (int)value;
			if (value is double) {
				double v = (double)value;
				double intValue = Math.Truncate(v);
				if (v != intValue)
					PdfDocumentReader.ThrowIncorrectDataException();
				return (int)intValue;
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		public double? GetNumber(string key) {
			object value;
			if (!TryGetValue(key, out value))
				return null;
			if (value is int)
				return (int)value;
			if (value is double)
				return (double)value;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfDocumentReader.ConvertToDouble(objects.GetObjectData(reference.Number));
		}
		public byte[] GetBytes(string key) {
			return Resolve<byte[]>(key);
		}
		public string GetString(string key) {
			byte[] bytes = GetBytes(key);
			return bytes == null ? null : PdfDocumentReader.ConvertToString(bytes);
		}
		public string GetStringAdvanced(string key) {
			object value;
			if (!TryGetValue(key, out value))
				return null;
			value = objects.TryResolve(value);
			byte[] bytes = value as byte[];
			if (bytes != null)
				return PdfDocumentReader.ConvertToString(bytes);
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			byte[] data = stream.GetData(true);
			return Encoding.UTF8.GetString(data, 0, data.Length);
		}
		public DateTimeOffset? GetDate(string key) {
			string str = GetString(key);
			if (String.IsNullOrEmpty(str))
				return null;
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			DateTimeOffset dateTime;
			if (DateTimeOffset.TryParse(str, invariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime))
				return dateTime;
			string[] splittedDateTime = Regex.Split(str, "[0-9][0-9][:][0-9][0-9][:][0-9][0-9]", RegexOptions.CultureInvariant);
			if (splittedDateTime.Length == 2)
				if (String.IsNullOrEmpty(splittedDateTime[1])) {
					string[] splittedDate = Regex.Split(splittedDateTime[0], "[.\\/-]", RegexOptions.CultureInvariant);
					if (splittedDate.Length == 3) {
						string timeString = str.Remove(0, splittedDateTime[0].Length);
						string dayString = splittedDate[0];
						int day;
						if (Int32.TryParse(dayString, NumberStyles.Integer, invariantCulture, out day) && day > 12) {
							splittedDate[0] = splittedDate[1];
							splittedDate[1] = dayString;
							if (DateTimeOffset.TryParse(String.Join("/", splittedDate) + timeString, invariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime))
								return dateTime;
						}
					}
				}
				else {
					string startString = splittedDateTime[0];
					if (str.StartsWith(startString)) {
						string timeString = str.Remove(0, startString.Length);
						string endString = splittedDateTime[1];
						if (str.EndsWith(endString)) {
							timeString = timeString.Remove(timeString.Length - endString.Length);
							if (DateTimeOffset.TryParse(String.Concat(splittedDateTime) + ' ' + timeString, invariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime))
								return dateTime;
						}
					}
				}
			int length = str.Length;
			if (length > 2 && str[0] == 'D' && str[1] == ':') {
				str = str.Substring(2);
				length -= 2;
			}
			try {
				switch (length) {
					case 6:
						return ParseDate(str.Insert(4, "0").Insert(6, "0"));
					case 7:
						return ParseDate(str.Insert(4, "0"));
					case 21:
						char c = str[17];
						if (c == '+' || c == '-')
							return ParseDate(str.Remove(17, 1).Insert(14, new String(c, 1)));
						break;
				}
				return ParseDate(str);
			}
			catch {
			}
			try {
				return ParseDate(str.Insert(6, "0"));
			}
			catch {
			}
			return null;
		}
		public PdfObjectReference GetObjectReference(string key) {
			object value;
			if (!TryGetValue(key, out value) || value == null)
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return reference;
		}
		public IList<object> GetArray(string key) {
			return Resolve<IList<object>>(key);
		}
		public IList<double> GetDoubleArray(string key) {
			IList<object> array = GetArray(key);
			if (array == null)
				return null;
			List<double> result = new List<double>();
			foreach (object o in array)
				result.Add(PdfDocumentReader.ConvertToDouble(o));
			return result;
		}
		public IList<T> GetArray<T>(string key, Func<object, T> create) {
			IList<T> result = new List<T>();
			IList<object> array = GetArray(key);
			if (array == null)
				return null;
			foreach (object item in array)
				result.Add(create(item));
			return result;
		}
		public IList<PdfRange> GetRanges(string key) {
			IList<double> array = GetDoubleArray(key);
			if (array == null)
				return null;
			int count = array.Count;
			if (count % 2 > 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			List<PdfRange> result = new List<PdfRange>();
			for (int i = 0; i < count; ) {
				double min = array[i++];
				double max = array[i++];
				result.Add(new PdfRange(min, max));
			}
			return result;
		}
		public PdfRectangle GetRectangle(string key) {
			IList<object> array = GetArray(key);
			return array == null ? null : new PdfRectangle(array);
		}
		public PdfRectangle GetPadding(PdfRectangle bounds) {
			PdfRectangle padding = GetRectangle(DictionaryPaddingKey);
			if (padding == null)
				return null;
			double left = Math.Max(0, padding.Left);
			double right = Math.Max(0, padding.Right);
			double paddingWidth = left + right;
			double remainWidth = paddingWidth - bounds.Width;
			if (remainWidth > 0) {
				double factor = remainWidth / paddingWidth;
				left -= left * factor;
				right -= right * factor;
			}
			double top = Math.Max(0, padding.Top);
			double bottom = Math.Max(0, padding.Bottom);
			double paddingHeight = top + bottom;
			double remainHeight = paddingHeight - bounds.Height;
			if (remainHeight > 0) {
				double factor = remainHeight / paddingHeight;
				top -= top * factor;
				bottom -= bottom * factor;
			}
			return new PdfRectangle(left, bottom, right, top);
		}
		public PdfReaderDictionary GetDictionary(string key) {
			return Resolve<PdfReaderDictionary>(key, i => objects.GetDictionary(i));
		}
		public PdfReaderStream GetStream(string key) {
			return Resolve<PdfReaderStream>(key, i => {
				PdfReaderStream value = objects.GetStream(i);
				if (value == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return value;
			});
		}
		public PdfMetadata GetMetadata() {
			PdfReaderStream metadataStream = Resolve<PdfReaderStream>(PdfMetadata.Name, i => objects.GetStream(i));
			return metadataStream == null ? null : new PdfMetadata(metadataStream);
		}
		public CultureInfo GetLanguageCulture() {
			object value;
			if (TryGetValue(DictionaryLanguageKey, out value)) {
				value = Objects.TryResolve(value);
				byte[] bytes = value as byte[];
				if (bytes != null)
					return ConvertToCultureInfo(PdfDocumentReader.ConvertToString(bytes));
			}
			return CultureInfo.InvariantCulture;
		}
		public PdfTextJustification GetTextJustification() {
			return PdfEnumToValueConverter.Parse<PdfTextJustification>(GetInteger(DictionaryJustificationKey), PdfTextJustification.LeftJustified);
		}
		public PdfAnnotationHighlightingMode GetAnnotationHighlightingMode() {
			string name = GetName(DictionaryAnnotationHighlightingModeKey);
			if (name == null)
				return PdfAnnotationHighlightingMode.Invert;
			return PdfEnumToStringConverter.Parse<PdfAnnotationHighlightingMode>(name);
		}
		public PdfRichMediaContentType? GetRichMediaContentType() {
			string name = GetName(PdfDictionary.DictionarySubtypeKey);
			return name == null ? (PdfRichMediaContentType?)null : PdfEnumToStringConverter.Parse<PdfRichMediaContentType>(name);
		}
		public PdfOptionalContent GetOptionalContent() {
			object value;
			return TryGetValue(PdfOptionalContent.DictionaryKey, out value) ? objects.GetOptionalContent(value) : null;
		}
		public PdfOptionalContentIntent GetOptionalContentIntent(string key) {
			object value;
			if (!TryGetValue(key, out value))
				return PdfOptionalContentIntent.View;
			value = objects.TryResolve(value);
			PdfName intentName = value as PdfName;
			if (intentName != null)
				return PdfEnumToStringConverter.Parse<PdfOptionalContentIntent>(intentName.Name);
			IList<object> list = value as IList<object>;
			if (list == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfOptionalContentIntent intent = (PdfOptionalContentIntent)0;
			foreach (object item in list) {
				intentName = item as PdfName;
				if (intentName == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				intent |= PdfEnumToStringConverter.Parse<PdfOptionalContentIntent>(intentName.Name);
			}
			if (intent == (PdfOptionalContentIntent)0)
				PdfDocumentReader.ThrowIncorrectDataException();
			return intent;
		}
		public PdfGraphicsStateParameters GetGraphicsStateParameters(string key) {
			object value;
			if (!TryGetValue(key, out value))
				return null;
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary != null)
				return new PdfGraphicsStateParameters(dictionary);
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return objects.GetGraphicsStateParameters(reference.Number);
		}
		public IList<PdfFilter> GetFilters(string key, string decodeParamsKey) {
			object value;
			if (!TryGetValue(key, out value))
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				value = objects.GetObjectData(reference.Number);
			List<PdfFilter> filters = new List<PdfFilter>();
			IList<object> filterList = value as IList<object>;
			if (filterList == null) {
				PdfName filterName = value as PdfName;
				if (filterName == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				filters.Add(PdfFilter.Create(filterName.Name, GetDictionary(decodeParamsKey)));
			}
			else {
				int filtersCount = filterList.Count;
				IList<object> filterParametersList = GetArray(decodeParamsKey);
				if (filterParametersList != null && filterParametersList.Count != filtersCount)
					PdfDocumentReader.ThrowIncorrectDataException();
				for (int i = 0; i < filtersCount; i++) {
					PdfName filterName = filterList[i] as PdfName;
					if (filterName == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					PdfReaderDictionary filterParameters = null;
					if (filterParametersList != null) {
						PdfObjectReference parametersReference = filterParametersList[i] as PdfObjectReference;
						if (parametersReference != null)
							filterParameters = objects.GetObjectData(parametersReference.Number) as PdfReaderDictionary;
						else
							filterParameters = filterParametersList[i] as PdfReaderDictionary;
					}
					filters.Add(PdfFilter.Create(filterName.Name, filterParameters));
				}
			}
			return filters;
		}
		public PdfCommandList GetAppearance(PdfResources resources) {
			byte[] da = GetBytes(DictionaryAppearanceKey);
			if (da == null)
				return null;
			if (resources == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfContentStreamParser.GetContent(resources, da);
		}
		public PdfObjectList<PdfCustomFunction> GetFunctions(string functionDictionaryKey, bool mustBeArray) {
			object value;
			if (!TryGetValue(functionDictionaryKey, out value))
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			return reference == null ? CreateFunctions(mustBeArray, value) :
				objects.ResolveObject<PdfObjectList<PdfCustomFunction>>(reference.Number, () => CreateFunctions(mustBeArray, objects.GetObjectData(reference.Number)));
		}
		public PdfDestinationObject GetDestination(string key) {
			object destination;
			if (!TryGetValue(key, out destination))
				return null;
			object destinationValue = objects.TryResolve(destination);
			byte[] bytes = destinationValue as byte[];
			if (bytes != null)
				return new PdfDestinationObject(new PdfNameTreeEncoding().GetString(bytes, 0, bytes.Length));
			PdfName name = destinationValue as PdfName;
			return name == null ? new PdfDestinationObject(objects.GetDestination(destination)) : new PdfDestinationObject(name.Name);
		}
		public PdfAction GetAction(string actionDictionary) {
			object value;
			if (TryGetValue(actionDictionary, out value))
				return Objects.GetAction(value);
			return null;
		}
		public PdfAdditionalActions GetAdditionalActions(PdfAnnotation parent) {
			object value;
			if (TryGetValue(PdfAdditionalActions.DictionaryAdditionalActionsKey, out value))
				return Objects.GetAdditionalActions(value, parent);
			return null;
		}
		public PdfJavaScriptAction GetJavaScriptAction(string key) {
			PdfAction action = GetAction(key);
			if (action == null)
				return null;
			PdfJavaScriptAction javaScriptAction = action as PdfJavaScriptAction;
			if (javaScriptAction == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return javaScriptAction;
		}
		public PdfResources GetResources(string key, PdfResources parentResources, bool shouldBeWritten) { 
			object value;
			if (!TryGetValue(key, out value))
				value = null;
			return Objects.GetResources(value, parentResources, shouldBeWritten);
		}
		public bool ContainsArrayNamedElement(string key, string name) {
			IList<object> list = GetArray(key);
			if (list != null)
				foreach (object o in list) {
					PdfName n = o as PdfName;
					if (n != null && n.Name == name)
						return true;
				}
			return false;
		}
		public T GetEnumName<T>(string key) where T : struct {
			return PdfEnumToStringConverter.Parse<T>(GetName(key), true);
		}
		T Resolve<T>(string key) {
			return Resolve<T>(key, i => {
				object value = objects.GetObjectData(i);
				if (value == null)
					return default(T);
				if (!(value is T))
					PdfDocumentReader.ThrowIncorrectDataException();
				return (T)value;
			});
		}
		T Resolve<T>(string key, Func<int, T> resolve) {
			object value;
			if (!TryGetValue(key, out value) || value == null)
				return default(T);
			if (value is T)
				return (T)value;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return resolve(reference.Number);
		}
		PdfObjectList<PdfCustomFunction> CreateFunctions(bool mustBeArray, object value) {
			PdfObjectList<PdfCustomFunction> functions = new PdfObjectList<PdfCustomFunction>(Objects);
			IList<object> funcArray = value as IList<object>;
			if (funcArray == null) {
				if (mustBeArray)
					PdfDocumentReader.ThrowIncorrectDataException();
				functions.Add(PdfCustomFunction.Parse(objects, value));
			}
			else
				foreach (object fn in funcArray)
					functions.Add(PdfCustomFunction.Parse(objects, fn));
			return functions;
		}
	}
}
