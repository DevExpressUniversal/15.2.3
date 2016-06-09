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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	internal static class PdfEnumToStringConverter {
#if WINRT || DXPORTABLE
		static IEnumerable<FieldInfo> GetFields(this Type type) {
			return type.GetTypeInfo().DeclaredFields;
		}
		static FieldInfo GetField(this Type type, string name) {
			return type.GetTypeInfo().GetDeclaredField(name);
		}
		static IEnumerable GetCustomAttributes(this Type type, Type t, bool inherit) {
			return type.GetTypeInfo().GetCustomAttributes(t, inherit);
		}
#endif
		static A GetAttribute<T, A>() where T : struct where A : Attribute {
			Type type = typeof(T);
			foreach (A attribute in type.GetCustomAttributes(typeof(A), false))
				return attribute;
			return null;
		}
		public static bool TryParse<T>(string str, out T value, bool useDefault = true) where T : struct {
			PdfDefaultFieldAttribute defaultValue = GetAttribute<T, PdfDefaultFieldAttribute>();
			if (String.IsNullOrEmpty(str)) {
				if (useDefault && defaultValue != null) {
					value = (T)defaultValue.Value;
					return true;
				}
				value = default(T);
				return false;
			}
			else {
				Type type = typeof(T);
				foreach (FieldInfo fi in type.GetFields())
					foreach (PdfFieldNameAttribute attribute in fi.GetCustomAttributes(typeof(PdfFieldNameAttribute), false)) {
						string alternateText = attribute.AlternateText;
						if (str == attribute.Text || (alternateText != null && str == alternateText)) {
							value = (T)Enum.Parse(type, fi.Name);
							return true;
						}
					}
				return Enum.TryParse<T>(str, out value) && (defaultValue == null || defaultValue.CanUsed || !value.Equals(defaultValue.Value));
			}
		}
		public static T Parse<T>(string str, bool useDefault = true) where T : struct {
			T result;
			if (!TryParse(str, out result, useDefault)) {
				if (GetAttribute<T, PdfSupportUndefinedValueAttribute>() != null) {
					PdfDefaultFieldAttribute defaultValue = GetAttribute<T, PdfDefaultFieldAttribute>();
					if (defaultValue != null)
						return (T)defaultValue.Value;
				}
				PdfDocumentReader.ThrowIncorrectDataException();
			}
			return result;
		}
		public static string Convert<T>(T value, bool useDefault = true) where T : struct {
			if (useDefault) {
				PdfDefaultFieldAttribute defaultValue = GetAttribute<T, PdfDefaultFieldAttribute>();
				if (defaultValue != null && value.Equals(defaultValue.Value))
					return null;
			}
			Type type = typeof(T);
			string name = Enum.GetName(type, value);
			FieldInfo fi = type.GetField(name);
			if (fi == null)
				return null;
			foreach (PdfFieldNameAttribute attribute in fi.GetCustomAttributes(typeof(PdfFieldNameAttribute), false))
				return attribute.Text;
			return name;
		}
	}
}
