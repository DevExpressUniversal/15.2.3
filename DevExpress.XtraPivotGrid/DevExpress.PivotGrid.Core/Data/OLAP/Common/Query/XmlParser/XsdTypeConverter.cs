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
using System.Xml;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.OLAP {
	static class XsdTypeConverter {
		internal readonly static Type DefaultType = typeof(string);
		readonly static Dictionary<string, Type> types = new Dictionary<string, Type>();
		readonly static Dictionary<string, Type> types2 = new Dictionary<string, Type>();
		readonly static NullableDictionary<string, Func<XmlReader, object>> readContentDic = new NullableDictionary<string, Func<XmlReader, object>>();
		static XsdTypeConverter() {
			FillTypes();
		}
		public static object ConvertTo(string value, string typeName) {
			Type type = TypeOf(typeName);
			return ConvertTo(value, type);
		}
		public static Func<XmlReader, object> GetConvertTo(string val) {
			return readContentDic[val];
		}
		public static object ConvertTo(string value, Type type) {
			if(type == typeof(double)) {
				switch(value) {
					case "NaN":
						return double.NaN;
					case "INF":
						return double.PositiveInfinity;
					case "-INF":
						return double.NegativeInfinity;
				}
			}
			return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
		}
		public static Type TypeOf(string typeName) {
			return TypeOf(typeName, DefaultType);
		}
		public static Type TypeOf(string typeName, Type defaultType) {
			if(string.IsNullOrEmpty(typeName))
				return defaultType;
			Type type;
			if(types2.TryGetValue(typeName, out type))
				return type;
			string[] strArray = typeName.Split(new char[] { ':' });
			if(strArray.Length != 2 || !types.TryGetValue(strArray[1], out type)) {
				types2.Add(typeName, defaultType);
				return defaultType;
			}
			types2.Add(typeName, type);
			return type;
		}
		static void FillTypes() {
			types.Add("string", typeof(string));
			types.Add("decimal", typeof(decimal));
			types.Add("float", typeof(float));
			types.Add("double", typeof(double));
			types.Add("int", typeof(int));
			types.Add("integer", typeof(int));
			types.Add("byte", typeof(sbyte));
			types.Add("long", typeof(long));
			types.Add("short", typeof(short));
			types.Add("boolean", typeof(bool));
			types.Add("dateTime", typeof(DateTime));
			types.Add("unsignedByte", typeof(byte));
			types.Add("unsignedShort", typeof(ushort));
			types.Add("unsignedInt", typeof(uint));
			types.Add("unsignedLong", typeof(ulong));
			types.Add("uuid", typeof(Guid));
			types.Add("base64Binary", typeof(byte[]));
			readContentDic.Add("xsd:string", (r) => r.ReadContentAsString());
			readContentDic.Add("xsd:decimal", (r) => r.ReadContentAsDecimal());
			readContentDic.Add("xsd:float", (r) => r.ReadContentAsFloat());
			readContentDic.Add("xsd:double", (r) => r.ReadContentAsDouble());
			readContentDic.Add("xsd:int", (r) => r.ReadContentAsInt());
			readContentDic.Add("xsd:integer", (r) => r.ReadContentAsInt());
			readContentDic.Add("xsd:byte", (r) => sbyte.Parse(r.ReadContentAsString()));
			readContentDic.Add("xsd:long", (r) => r.ReadContentAsLong());
			readContentDic.Add("xsd:short", (r) => short.Parse(r.ReadContentAsString()));
			readContentDic.Add("xsd:boolean", (r) => r.ReadContentAsBoolean());
			readContentDic.Add("xsd:dateTime", (r) => r.ReadContentAsDateTime());
			readContentDic.Add("xsd:unsignedByte", (r) => r.ReadContentAsString()[0]);
			readContentDic.Add("xsd:unsignedShort", (r) => ushort.Parse(r.ReadContentAsString()));
			readContentDic.Add("xsd:unsignedInt", (r) => uint.Parse(r.ReadContentAsString()));
			readContentDic.Add("xsd:unsignedLong", (r) => ulong.Parse(r.ReadContentAsString()));
			readContentDic.Add("xsd:uuid", (r) => Guid.Parse(r.ReadContentAsString()));
			readContentDic.Add("xsd:base64Binary", (r) => Convert.ChangeType(r.ReadContentAsString(), typeof(byte[])));
			readContentDic.Add(null, (r) => r.ReadContentAsString());
		}
	}
}
