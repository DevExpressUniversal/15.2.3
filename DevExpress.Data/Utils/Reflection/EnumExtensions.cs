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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Utils {
	public static class EnumExtensions {
		public static Array GetValues(this Type enumType) {
#if SILVERLIGHT || DXPORTABLE
			FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
			object[] values = new object[fields.Length];
			for(int i = 0; i < fields.Length; i++) {
				values[i] = fields[i].GetValue(null);
			}
			return values;
#else
			return Enum.GetValues(enumType);
#endif
		}
		public static bool ToBoolean(this DefaultBoolean value, bool defaultValue) {
			if(value == DefaultBoolean.Default)
				return defaultValue;
			return (value == DefaultBoolean.True);
		}
		public static bool HasAnyFlag(this Enum value, params Enum[] flags) {
			foreach(Enum flag in flags)
				if(value.HasFlag(flag)) return true;
			return false;
		}  
		public static string GetEnumItemDisplayText(object current) {
			if(current == null) return string.Empty;
			string ret = current.ToString();
			MemberInfo[] info = current.GetType().GetMember(ret);
			if(info.Length == 0) return ret;
			foreach (Attribute attr in info[0].GetCustomAttributes(typeof(DescriptionAttribute), false)) {
				DescriptionAttribute descriptionAttribute = attr as DescriptionAttribute;
				if (descriptionAttribute != null) {
					ret = descriptionAttribute.Description;
					break;
				}
			}
			return ret;
		}
	}
}
