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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class SerializationResult {
		private string propertiesTree;
		private string typeInfoTable;
		private string typeNameTable;
		public SerializationResult(StringBuilder propertiesTree, StringBuilder typeInfoTable, StringBuilder typeNameTable) {
			this.propertiesTree = propertiesTree.ToString();
			this.typeInfoTable = typeInfoTable.ToString();
			this.typeNameTable = typeNameTable.ToString();
		}
		public string PropertiesTree {
			get { return propertiesTree; }
		}
		public string TypeInfoTable {
			get { return typeInfoTable; }
		}
		public string TypeNameTable {
			get { return typeNameTable; }
		}
	}
	public static class HiddenFieldUtils {
		private const char Separator = '|';
		private const char Sentinel = '#';
		private const string TopLevelKeyPrefix = "dxp";
		public static void AssertPropertyNameIsValid(string propertyName) {
			if(string.IsNullOrEmpty(propertyName))
				ExceptionHelper.ThrowPropertyNameIsNullOrEmpty();
		}
		public static class Serializer {
			private const bool ValidateKeys = true;
			public static SerializationResult SerializeToScript(Dictionary<string, object> properties) {
				StringBuilder propertiesTree = new StringBuilder();
				TypeInfoTable typeInfoTable = new TypeInfoTable();
				List<string> typeNameTable = new List<string>();
				SerializeToScriptCore(properties, string.Empty, propertiesTree, typeInfoTable, typeNameTable, TopLevelKeyPrefix, ValidateKeys, false);
				StringBuilder serializedTypeInfoTable = new StringBuilder();
				SerializeToScriptCore(typeInfoTable, string.Empty, serializedTypeInfoTable, null, null, string.Empty, !ValidateKeys, false);
				StringBuilder serializedTypeNameTable = new StringBuilder();
				SerializeToScriptCore(typeNameTable, string.Empty, serializedTypeNameTable, null, null, string.Empty, !ValidateKeys, false);
				return new SerializationResult(propertiesTree, serializedTypeInfoTable, serializedTypeNameTable);
			}
			private static void SerializeToScriptCore(object value, string pathInPropertiesTree, StringBuilder propertiesTree, TypeInfoTable typeInfoTable,
				List<string> typeNameTable, string keyNamePrefix, bool validateKeys, bool skipAtomValue) {
				if(typeInfoTable != null && typeNameTable != null && pathInPropertiesTree.Length > 0)
					AppendTypeInfo(value, pathInPropertiesTree, typeInfoTable, typeNameTable, ref skipAtomValue);
				if(value == null || value == DBNull.Value || value is ValueType || value is string || value is Regex) {
					propertiesTree.Append(HtmlConvertor.ToScript(value));
				} else {
					IDictionary dictionary = value as IDictionary;
					if(dictionary != null) {
						Tab(propertiesTree, pathInPropertiesTree, -1); 
						propertiesTree.Append('{');
						NewLine(propertiesTree); 
						foreach(DictionaryEntry entry in dictionary) {
							string keyStr = entry.Key.ToString();
							if(keyNamePrefix != null)
								keyStr = keyNamePrefix + keyStr;
							if(validateKeys)
								AssertPropertyNameIsValid(keyStr);
							Tab(propertiesTree, GetItemPathInPropertiesTree(pathInPropertiesTree, keyStr), 1); 
							propertiesTree.Append(HtmlConvertor.ToScript(keyStr, typeof(string)));
							propertiesTree.Append(':');
							Whitespace(propertiesTree); 
							SerializeToScriptCore(entry.Value, GetItemPathInPropertiesTree(pathInPropertiesTree, keyStr),
								propertiesTree, typeInfoTable, typeNameTable, string.Empty, validateKeys, skipAtomValue);
							propertiesTree.Append(',');
							NewLine(propertiesTree); 
						}
						RemoveCommaAfterLastItem(propertiesTree, dictionary);
						Tab(propertiesTree, pathInPropertiesTree, !string.IsNullOrEmpty(pathInPropertiesTree) ? 1 : 0); 
						propertiesTree.Append('}');
					} else {
						IList list = value as IList;
						if(list != null) {
							Tab(propertiesTree, pathInPropertiesTree, -1); 
							propertiesTree.Append('[');
							NewLine(propertiesTree); 
							int itemIndex = 0;
							foreach(object item in list) {
								Tab(propertiesTree, GetItemPathInPropertiesTree(pathInPropertiesTree, itemIndex.ToString(CultureInfo.InvariantCulture)), 1); 
								SerializeToScriptCore(item, GetItemPathInPropertiesTree(pathInPropertiesTree, itemIndex.ToString(CultureInfo.InvariantCulture)),
									propertiesTree, typeInfoTable, typeNameTable, string.Empty, validateKeys, skipAtomValue);
								propertiesTree.Append(',');
								NewLine(propertiesTree); 
								itemIndex++;
							}
							RemoveCommaAfterLastItem(propertiesTree, list);
							Tab(propertiesTree, pathInPropertiesTree, !string.IsNullOrEmpty(pathInPropertiesTree) ? 1 : 0); 
							propertiesTree.Append(']');
						} else {
							ExceptionHelper.ThrowUnableToConvertValueToScript(value);
						}
					}
				}
			}
			private static string GetItemPathInPropertiesTree(string pathInPropertiesTree, string itemKeyStr) {
				return pathInPropertiesTree.Length > 0 ? (pathInPropertiesTree + Separator + itemKeyStr) : itemKeyStr;
			}
			private static void AppendTypeInfo(object value, string pathInPropertiesTree, TypeInfoTable typeInfoTable, List<string> typeNameTable, ref bool skipAtomValue) {
				if(value == null || value == DBNull.Value)
					return;
				Type valueType = value.GetType();
				object arrayItem = null;
				var sameItemTypeArray = valueType.IsArray && KnownTypesRepository.IsValuesOfSameType(value as IEnumerable, out arrayItem);
				var sameAtomItemTypeArray = sameItemTypeArray && arrayItem != null && IsAtomValue(arrayItem);
				if(sameAtomItemTypeArray) {
					AppendTypeInfoCore(arrayItem.GetType(), pathInPropertiesTree + "_itemType", typeInfoTable, typeNameTable);
					skipAtomValue = true;
				}
				if(!KnownTypesRepository.IsTypeInfoRequiredFor(valueType))
					return;
				if(!skipAtomValue || !IsAtomValue(value))
					AppendTypeInfoCore(valueType, pathInPropertiesTree, typeInfoTable, typeNameTable);
			}
			private static bool IsAtomValue(object value) {
				return value is ValueType || value is string || value is Regex;
			}
			private static void AppendTypeInfoCore(Type valueType, string pathInPropertiesTree, TypeInfoTable typeInfoTable, List<string> typeNameTable) {
				int knownTypeCode = -1;
				if(!KnownTypesRepository.TryGetKnownTypeCode(valueType, out knownTypeCode)) {
					string valueTypeName = valueType.AssemblyQualifiedName;
					int typeNameIndex = typeNameTable.IndexOf(valueTypeName);
					if(typeNameIndex < 0) {
						typeNameTable.Add(valueTypeName);
						typeNameIndex = typeNameTable.Count - 1;
					}
					typeInfoTable.Add(pathInPropertiesTree, KnownTypesRepository.MinUnknownTypeCode + typeNameIndex);
				}
				else
					typeInfoTable.Add(pathInPropertiesTree, knownTypeCode);
			}
			private static void RemoveCommaAfterLastItem(StringBuilder sb, ICollection items) {
				if(items.Count > 0)
					sb.Remove(sb.Length - LastItemCommaOffset, 1);
			}
			private static int LastItemCommaOffset {
				get {
#if DEBUG
					return 3; 
#else
					return 1; 
#endif
				}
			}
			[Conditional("DEBUG")]
			private static void NewLine(StringBuilder sb) {
				sb.AppendLine();
			}
			[Conditional("DEBUG")]
			private static void Whitespace(StringBuilder sb) {
				sb.Append(' ');
			}
			[Conditional("DEBUG")]
			private static void Tab(StringBuilder sb, string pathInPropertiesTree, int additionalTabCount) {
				int separatorOccurencesCount = 0;
				if(!string.IsNullOrEmpty(pathInPropertiesTree)) {
					for(int i = 0; i < pathInPropertiesTree.Length; i++)
						if(pathInPropertiesTree[i] == Separator)
							separatorOccurencesCount++;
				}
				for(int i = separatorOccurencesCount + additionalTabCount; i > 0; i--)
					sb.Append("    ");
			}
		}
		public static class Deserializer {
			internal enum ValueKind { Undefined, Atom, List, Dictionary }
			private static readonly DateTime BaseDateTime = new DateTime(1970, 1, 1);
			public static Dictionary<string, object> Deserialize(string clientData) {
				clientData = clientData.Replace("\r", "");
				int pos = 0;
				List<string> typeNameTable = ParseTypeNameTable(clientData, ref pos);
				Dictionary<string, object> properties = ParseProperties(clientData, typeNameTable, ref pos);
				return properties;
			}
			internal static List<string> ParseTypeNameTable(string serializedData, ref int pos) {
				List<string> typeNameTable = new List<string>();
				return (List<string>)ParseValue(serializedData, ref pos, null, typeNameTable, ValueKind.List);
			}
			internal static Dictionary<string, object> ParseProperties(string serializedProperties, List<string> typeNameTable, ref int pos) {
				Dictionary<string, object> properties = new Dictionary<string, object>();
				return (Dictionary<string, object>)ParseValue(serializedProperties, ref pos, typeNameTable, properties, ValueKind.Dictionary);
			}
			internal static object ParseValue(string serializedData, ref int pos, List<string> typeNameTable) {
				return ParseValue(serializedData, ref pos, typeNameTable, null, ValueKind.Undefined);
			}
			private static bool TypeSupportsInterface(Type type, string interfaceName) {
				return type != null && type.GetInterface(interfaceName) != null;
			}
			internal static object ParseValue(string serializedData, ref int pos, List<string> typeNameTable, object valueInstance, ValueKind valueKind) {
				int typeCode = ParseInt32(serializedData, ref pos);
				Type type = GetValueType(typeCode, typeNameTable);
				if(valueInstance != null && valueKind == ValueKind.List || KnownTypesRepository.IsListTypeCode(typeCode) || TypeSupportsInterface(type, "IList"))
					return ParseListValue(serializedData, ref pos, typeCode, typeNameTable, valueInstance);
				else if(valueInstance != null && valueKind == ValueKind.Dictionary || KnownTypesRepository.IsDictionaryTypeCode(typeCode) || TypeSupportsInterface(type, "IDictionary"))
					return ParseDictionaryValue(serializedData, ref pos, typeCode, typeNameTable, valueInstance);
				else
					return ParseAtomValue(serializedData, ref pos, typeCode, typeNameTable);
			}
			internal static IList ParseListValue(string serializedData, ref int pos, int typeCode, List<string> typeNameTable, object listInstance) {
				IList list = listInstance as IList;
				if(list == null)
					list = CreateListInstance(typeCode, typeNameTable);
				while(serializedData[pos] != Sentinel) {
					object itemValue = ParseValue(serializedData, ref pos, typeNameTable);
					list.Add(itemValue);
				}
				pos++;
				ArrayProxy arrayProxy = list as ArrayProxy;
				return arrayProxy != null ? arrayProxy.ExtractArray() : list;
			}
			internal static IDictionary ParseDictionaryValue(string serializedData, ref int pos, int typeCode, List<string> typeNameTable, object dictionaryInstance) {
				IDictionary dictionary = dictionaryInstance as IDictionary;
				if(dictionary == null)
					dictionary = CreateDictionaryInstance(typeCode, typeNameTable);
				while(serializedData[pos] != Sentinel) {
					string key = ParseString(serializedData, ref pos);
					AssertPropertyNameIsValid(key);
					object value = ParseValue(serializedData, ref pos, typeNameTable);
					dictionary.Add(key, value);
				}
				pos++;
				return dictionary;
			}
			internal static object ParseAtomValue(string serializedData, ref int pos, int typeCode, List<string> typeNameTable) {
				int valueStrLength = ParseInt32(serializedData, ref pos);
				string valueString = serializedData.Substring(pos, valueStrLength);
				pos += valueStrLength;
				return ParseAtomValueCore(valueString, typeCode, typeNameTable);
			}
			internal static object ParseAtomValueCore(string valueStr, int typeCode, List<string> typeNameTable) {
				if(typeCode == 1)
					return null;
				Type type = GetValueType(typeCode, typeNameTable);
				if(type == typeof(String)) {
					if(valueStr[0] == '0')
						return null;
					else
						return valueStr.Substring(1).Replace("\n", "\r\n");
				}
				if(IsNullableType(type) && string.IsNullOrEmpty(valueStr))
					return null;
				if(type == typeof(Int32))
					return Int32.Parse(valueStr, CultureInfo.InvariantCulture);
				if(type == typeof(Double))
					return Double.Parse(valueStr, CultureInfo.InvariantCulture);
				if(type == typeof(Boolean))
					return valueStr == "1";
				if(type == typeof(Byte))
					return Byte.Parse(valueStr, CultureInfo.InvariantCulture);
				if(type == typeof(Single))
					return Single.Parse(valueStr, CultureInfo.InvariantCulture);
				if(type == typeof(Decimal))
					return Decimal.Parse(valueStr, CultureInfo.InvariantCulture); 
				if(type == typeof(Char))
					return valueStr[0];
				if(type == typeof(DateTime))
					return BaseDateTime.AddMilliseconds(Double.Parse(valueStr, CultureInfo.InvariantCulture));
				if(type == typeof(Int64))
					return Int64.Parse(valueStr, CultureInfo.InvariantCulture);
				if(type == typeof(Regex)) {
					int indexOfComma = valueStr.IndexOf(',');
					RegexOptions options = RegexOptions.None;
					for(int i = 0; i < indexOfComma; i++) {
						if(valueStr[i] == 'i')
							options |= RegexOptions.IgnoreCase;
						else if(valueStr[i] == 'm')
							options |= RegexOptions.Multiline;
					}
					return new Regex(valueStr.Substring(indexOfComma + 1), options);
				}
				if(type == typeof(Int16))
					return Int16.Parse(valueStr, CultureInfo.InvariantCulture);
				if(type == typeof(Guid))
					return new Guid(valueStr);
				if(type == typeof(DBNull))
					return DBNull.Value;
				ExceptionHelper.ThrowUnableToParseAtomValue(type);
				return null;
			}
			internal static int ParseInt32(string serializedData, ref int pos) {
				string str = ParseString(serializedData, ref pos);
				return !string.IsNullOrEmpty(str) ? Int32.Parse(str, CultureInfo.InvariantCulture) : 0;
			}
			internal static string ParseString(string serializedData, ref int pos) {
				int indexOfSeparator = serializedData.IndexOf(Separator, pos);
				string str = serializedData.Substring(pos, indexOfSeparator - pos);
				pos = indexOfSeparator + 1;
				return str;
			}
			private static bool IsNullableType(Type type) {
				return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
			}
			private static IList CreateListInstance(int listTypeCode, List<string> typeNameTable) {
				if(KnownTypesRepository.IsKnownTypeCode(listTypeCode))
					return KnownTypesRepository.CreateKnownListTypeInstance(listTypeCode);
				else
					return (IList)Activator.CreateInstance(GetValueType(listTypeCode, typeNameTable));
			}
			private static IDictionary CreateDictionaryInstance(int dictionaryTypeCode, List<string> typeNameTable) {
				if(KnownTypesRepository.IsKnownTypeCode(dictionaryTypeCode))
					return KnownTypesRepository.CreateKnownDictionaryTypeInstance(dictionaryTypeCode);
				else
					return (IDictionary)Activator.CreateInstance(GetValueType(dictionaryTypeCode, typeNameTable));
			}
			private static Type GetValueType(int typeCode, List<string> typeNameTable) {
				if(KnownTypesRepository.IsKnownTypeCode(typeCode))
					return KnownTypesRepository.GetKnownType(typeCode);
				else
					return Type.GetType(typeNameTable[typeCode - KnownTypesRepository.MinUnknownTypeCode]);
			}
		}
		[DebuggerTypeProxy(typeof(TypeInfoTableDebuggerProxy))]
		private class TypeInfoTable : Dictionary<string, int> { }
		private class TypeInfoTableDebuggerProxy {
			public class Item {
				private string key;
				private string value;
				public Item(string key, string value) {
					this.key = key;
					this.value = value;
				}
				public override string ToString() {
					return string.Format("{0}: {1}", this.key, this.value);
				}
			}
			private TypeInfoTable typeInfoTable;
			private Item[] items;
			public TypeInfoTableDebuggerProxy(TypeInfoTable typeInfoTable) {
				this.typeInfoTable = typeInfoTable;
			}
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public Item[] Items {
				get {
					if(items == null)
						items = CreateItems();
					return items;
				}
			}
			private Item[] CreateItems() {
				if(this.typeInfoTable == null)
					return null;
				List<Item> items = new List<Item>();
				foreach(KeyValuePair<string, int> pair in this.typeInfoTable) {
					string key = pair.Key;
					int typeCode = pair.Value;
					string value = typeCode.ToString(CultureInfo.InvariantCulture);
					if(KnownTypesRepository.IsKnownTypeCode(typeCode))
						value += " (" + KnownTypesRepository.GetKnownType(typeCode).Name + ")";
					items.Add(new Item(key, value));
				}
				return items.ToArray();
			}
		}
	}
	public static class ExceptionHelper {
		private static class ExceptionMessageFormat {
			public const string
				PropertyNameIsNullOrEmpty = "Property name is null or empty.",
				UnableToConvertValueToScript = "Unable to convert the value \"{0}\" to script.",
				UnableToParseAtomValueOfType = "Unable to parse an atom value of the type \"{0}\".",
				UnableToInstantiateDictionary = "Unable to instantiate a dictionary with the type code {0}.",
				UnableToInstantiateList = "Unable to instantiate a list with the type code {0}.",
				KnownTypesCollectionHasInvalidSize = "Size of the known type collection is invalid.";
		}
		public static void ThrowPropertyNameIsNullOrEmpty() {
			ThrowArgumentException(ExceptionMessageFormat.PropertyNameIsNullOrEmpty);
		}
		public static void ThrowUnableToConvertValueToScript(object value) {
			ThrowArgumentException(ExceptionMessageFormat.UnableToConvertValueToScript, value.ToString());
		}
		public static void ThrowUnableToInstantiateDictionary(int typeCode) {
			ThrowArgumentException(ExceptionMessageFormat.UnableToInstantiateDictionary, typeCode);
		}
		public static void ThrowUnableToInstantiateList(int typeCode) {
			ThrowArgumentException(ExceptionMessageFormat.UnableToInstantiateList, typeCode);
		}
		public static void ThrowKnownTypesCollectionHasInvalidSize() {
			ThrowInvalidOperationException(ExceptionMessageFormat.KnownTypesCollectionHasInvalidSize);
		}
		public static void ThrowUnableToParseAtomValue(Type type) {
			ThrowInvalidOperationException(ExceptionMessageFormat.UnableToParseAtomValueOfType, type.FullName);
		}
		private static void ThrowArgumentException(string format, params object[] args) {
			throw new ArgumentException(string.Format(format, args));
		}
		private static void ThrowInvalidOperationException(string format, params object[] args) {
			throw new InvalidOperationException(string.Format(format, args));
		}
	}
}
