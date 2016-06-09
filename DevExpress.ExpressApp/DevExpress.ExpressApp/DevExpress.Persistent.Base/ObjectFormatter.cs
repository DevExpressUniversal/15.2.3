#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.Persistent.Base {
	public class CustomFormatObjectEventArgs : HandledEventArgs {
		private string formatString;
		private object obj;
		private string result;
		private EmptyEntriesMode emptyEntriesMode;
		public CustomFormatObjectEventArgs(string formatString, object obj, EmptyEntriesMode emptyEntriesMode) {
			this.formatString = formatString;
			this.obj = obj;
			this.emptyEntriesMode = emptyEntriesMode;
		}
		public string FormatString {
			get { return formatString; }
		}
		public object Object {
			get { return obj; }
		}
		public string Result {
			get { return result; }
			set { result = value; }
		}
		public EmptyEntriesMode EmptyEntriesMode {
			get { return emptyEntriesMode; }
		}
	}
	public class CustomGetValueEventArgs : HandledEventArgs {
		private string memberPath;
		private object obj;
		private object memberValue;
		public CustomGetValueEventArgs(string memberPath, object obj) {
			this.memberPath = memberPath;
			this.obj = obj;
		}
		public string MemberPath {
			get { return memberPath; }
		}
		public object Object {
			get { return obj; }
		}
		public object Value {
			get { return memberValue; }
			set { memberValue = value; }
		}
	}
	public enum EmptyEntriesMode { 
		Default, 
		RemoveDelimiterWhenEntryIsEmpty,
		[Obsolete("Use RemoveDelimiterWhenEntryIsEmpty instead")]
		RemoveDelimeterWhenEntryIsEmpty
	}
	public class ObjectFormatter : ICustomFormatter, IFormatProvider {
		private const string UniqueStringForOpenBrace = "BBBD1F2C-3E69-44D7-96B6-A303A423EDFE";
		private const string UniqueStringForEndBrace = "4759C22A-02A2-419C-9DDA-997BF192F38E";
		private static object nullObject = new object();
		private object GetValueRecursive(string format, object arg) {
			object propertyValue = nullObject;
			object probableParameterValue = CriteriaWrapper.TryGetReadOnlyParameterValue(format);
			if(!(probableParameterValue is string) || (format != (string)probableParameterValue)) {
				propertyValue = probableParameterValue;
			}
			else {
				object valueByPath;
				if(TryGetValueByPathDynamic(format, arg, out valueByPath)) {
					propertyValue = valueByPath;
				}
			}
			if(CustomGetValue != null) {
				CustomGetValueEventArgs args = new CustomGetValueEventArgs(format, arg);
				CustomGetValue(null, args);
				if(args.Handled) {
					propertyValue = args.Value;
				}
			}
			if(propertyValue is string && format != (string)propertyValue) {
				if(((string)propertyValue).StartsWith(CriteriaWrapper.ParameterPrefix)) {
					object value = GetValueRecursive((string)propertyValue, arg);
					if(value != nullObject) {
						propertyValue = value;
					}
				}
				else {
					if(((string)propertyValue).Contains("{@")) {
						propertyValue = Format((string)propertyValue, arg);
					}
				}
			}
			return propertyValue;
		}
		private bool TryGetValueByPathDynamic(string propertyPath, object from, out object result) {
			Guard.ArgumentNotNull(from, "from");
			Guard.ArgumentNotNull(propertyPath, "propertyPath");
			result = null;
			string[] members = propertyPath.Split('.');
			object currObj = from;
			foreach(string member in members) {
				if(currObj == null) {
					return true;
				}
				ITypeInfo objType = XafTypesInfo.Instance.FindTypeInfo(currObj.GetType());
				IMemberInfo currMember = objType.FindMember(member);
				if(currMember == null) {
					return false;
				}
				currObj = currMember.GetValue(currObj);
			}
			result = currObj;
			return true;
		}
		public static string Format(string format, object obj) {
			return Format(format, obj, EmptyEntriesMode.Default);
		}
		public static string Format(string format, object obj, EmptyEntriesMode mode) {
			if(string.IsNullOrEmpty(format)) {
				return string.Empty;
			}
			try {
				if(CustomFormatObject != null) {
					CustomFormatObjectEventArgs args = new CustomFormatObjectEventArgs(format, obj, mode);
					CustomFormatObject(null, args);
					if(args.Handled) {
						return args.Result;
					}
				}
				format = format.Replace("{{", UniqueStringForOpenBrace).Replace("}}", UniqueStringForEndBrace);
				char[] chArray = format.ToCharArray(0, format.Length);
				int length = chArray.Length;
				StringBuilder partBuilder = new StringBuilder();
				List<string> parts = new List<string>();
				bool isBraceOpened = false;
				for(int i = 0; i < length; i++) {
					char ch = chArray[i];
					if(ch == '{') {
						if(isBraceOpened) {
							throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
						}
						isBraceOpened = true;
						if(partBuilder.Length > 0) {
							parts.Add(partBuilder.ToString());
						}
						partBuilder = new StringBuilder();
						partBuilder.Append(ch);
					}
					else if(ch == '}') {
						if(!isBraceOpened) {
							throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
						}
						if(partBuilder.Length <= 1) {
							throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
						}
						isBraceOpened = false;
						partBuilder.Append(ch);
						parts.Add(partBuilder.ToString());
						partBuilder = new StringBuilder();
					}
					else {
						partBuilder.Append(ch);
					}
				}
				if(partBuilder.Length > 0) {
					parts.Add(partBuilder.ToString());
				}
				if(isBraceOpened) {
					throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
				}
				StringBuilder result = new StringBuilder();
				string lastDelimiter = null;
				bool isLastFormattedPartEmpty = false;
				bool hasNonEmptyFormattedPart = false;
				for(int i = 0; i < parts.Count; i++) {
					if(parts[i].StartsWith("{")) {
						string part = parts[i];
						if(!part.StartsWith("{0:")) {
							part = "{0:" + part.Substring(1, part.Length - 1);
						}
						string formattedPart = string.Format(new ObjectFormatter(), part, obj);
						if(!string.IsNullOrEmpty(formattedPart)) {
							if(!string.IsNullOrEmpty(lastDelimiter) &&
								((mode == EmptyEntriesMode.Default) || hasNonEmptyFormattedPart || !isLastFormattedPartEmpty)) {
								result.Append(lastDelimiter);
							}
							hasNonEmptyFormattedPart = true;
							result.Append(formattedPart);
						}
						else {
							isLastFormattedPartEmpty = true;
							if(!string.IsNullOrEmpty(lastDelimiter) && (mode == EmptyEntriesMode.Default)) {
								result.Append(lastDelimiter);
							}
						}
						lastDelimiter = null;
					}
					else {
						lastDelimiter = parts[i];
					}
				}
				if(!string.IsNullOrEmpty(lastDelimiter) && ((mode == EmptyEntriesMode.Default) || !isLastFormattedPartEmpty)) {
					result.Append(lastDelimiter);
				}
				return result.ToString().Replace(UniqueStringForOpenBrace, "{").Replace(UniqueStringForEndBrace, "}");
			}
			catch(Exception e) {
				Tracing.Tracer.LogValue("format", format);
				if(e is FormatException) {
					Tracing.Tracer.LogValue("obj", e.Message);
				}
				Tracing.Tracer.LogValue("mode", mode);
				throw;
			}
		}
		public static event EventHandler<CustomFormatObjectEventArgs> CustomFormatObject;
		public static event EventHandler<CustomGetValueEventArgs> CustomGetValue;
		object IFormatProvider.GetFormat(Type formatType) {
			return (formatType == typeof(ICustomFormatter)) ? this : null;
		}
		string ICustomFormatter.Format(string format, object arg, IFormatProvider provider) {
			if(!string.IsNullOrEmpty(format) && arg != null) {
				object propertyValue = nullObject;
				string memberPath = format;
				string formatString = "";
				if(format.Contains(":")) {
					memberPath = format.Substring(0, format.IndexOf(":")).Trim();
					formatString = format.Substring(format.IndexOf(":"));
				}
				propertyValue = GetValueRecursive(memberPath, arg);
				if(propertyValue == nullObject) {
					throw new MemberNotFoundException(arg.GetType(), memberPath);
				}
				if(propertyValue != null) {
					if(string.IsNullOrEmpty(formatString)) {
						string displayText = ReflectionHelper.GetObjectDisplayText(propertyValue);
						if(!string.IsNullOrEmpty(displayText)) {
							propertyValue = displayText;
						}
					}
				}
				return string.Format("{0" + formatString + "}", propertyValue);
			}
			return string.Format(provider, format, arg);
		}
	}
}
