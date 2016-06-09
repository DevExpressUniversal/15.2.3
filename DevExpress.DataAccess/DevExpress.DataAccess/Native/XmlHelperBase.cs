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
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Native {
	public static class XmlHelperBase {
		public static string GetAttributeValue(this XElement element, string attributeName) {
			Guard.ArgumentNotNull(element, "element");
			Guard.ArgumentIsNotNullOrEmpty(attributeName, "attributeName");
			XAttribute attribute = element.Attribute(attributeName);
			return attribute == null ? null : attribute.Value;
		}
		public static object FromString(Type type, string str) {
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if(converter == null)
				throw new XmlException();
			try {
				if(type == typeof(TimeSpan))
					return XmlConvert.ToTimeSpan(str);
				else
					return converter.ConvertFrom(null, CultureInfo.InvariantCulture, str);
			}
			catch {
				try {
					return converter.ConvertTo(null, CultureInfo.InvariantCulture, str, type);
				}
				catch {
					throw new XmlException();
				}
			}
		}
		public static TEnum EnumFromString<TEnum>(string str) {
			return (TEnum)Enum.Parse(typeof(TEnum), str);
		}	 
	}
}
