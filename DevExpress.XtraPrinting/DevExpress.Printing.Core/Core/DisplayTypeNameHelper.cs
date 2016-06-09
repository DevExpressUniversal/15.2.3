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
using System.Text;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.Compatibility.Utils.Design;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting.Native {
	public static class DisplayTypeNameHelper {
		static Dictionary<Type, DisplayNameAttribute> displayTypeNames = new Dictionary<Type, DisplayNameAttribute>();
		static Dictionary<Enum, string> enumDisplayNames = new Dictionary<Enum, string>();
		public static string GetDisplayTypeName(Type type) {
			if(!displayTypeNames.ContainsKey(type)) {
				object[] attributes =
#if DXRESTRICTED
					null;
#else
				type.GetCustomAttributes(typeof(DXDisplayNameAttribute), true);
#endif
				if(attributes != null && attributes.Length > 0) {
					DisplayNameAttribute displayNameAttribute = (DisplayNameAttribute)attributes[0];
					displayTypeNames.Add(type, displayNameAttribute);
				} else
					displayTypeNames.Add(type, new DisplayNameAttribute(type.FullName));
			}
			return displayTypeNames[type].DisplayName;
		}
		public static string GetDisplayTypeName(Enum value) {
			if(!enumDisplayNames.ContainsKey(value)) {
				Type enumType = value.GetType();
				EnumTypeConverter enumTypeConverter = new EnumTypeConverter(enumType);
				enumDisplayNames.Add(value, (string)enumTypeConverter.ConvertTo(new RuntimeTypeDescriptorContext(null, value), null, value, typeof(string)));
				}
			return enumDisplayNames[value];
			}
		public static Enum GetEnumValueFromDisplayName(Type enumType, string displayName) {
			EnumTypeConverter enumTypeConverter = new EnumTypeConverter(enumType);
			return (Enum)enumTypeConverter.ConvertFromString(displayName);
		}
	}
}
