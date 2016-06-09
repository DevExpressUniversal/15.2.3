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
using System.Drawing;
using System.Xml;
using System.Text.RegularExpressions;
namespace DevExpress.XtraCharts.Native {
	public sealed class XmlUtils {
		const string emptyColorName = "Empty";
		const string xmlFontName = "Name";
		const string xmlFontSize = "Size";
		const string xmlFontBold = "Bold";
		const string xmlFontItalic = "Italic";
		const string xmlFontUnderline = "Underline";
		const string xmlFontStrikeout = "Strikeout";
		public static string Color2String(Color color) {
			if(color.IsEmpty)
				return emptyColorName;
			if(color.IsKnownColor)
				return color.Name;
			return color.A.ToString() + ", " + color.R.ToString() + ", " + color.G.ToString() + ", " + color.B.ToString();
		}
			static Regex regex = new Regex(@"^(\d+),\s*(\d+),\s*(\d+),\s*(\d+)$", RegexOptions.Compiled);
		public static Color String2Color(string colorString) {
			if(colorString == emptyColorName)
				return Color.Empty;
			Match m = regex.Match(colorString);
			if (m.Success) {
				int a = int.Parse(m.Groups[1].Value);
				int r = int.Parse(m.Groups[2].Value);
				int g = int.Parse(m.Groups[3].Value);
				int b = int.Parse(m.Groups[4].Value);
				return Color.FromArgb(a, r, g, b);
			} else
				return Color.FromName(colorString);
		}
		public static string Enum2String(Enum enumValue) {
			return Enum.GetName(enumValue.GetType(), enumValue);
		}
		public static object String2Enum(string enumString, Type enumType) {
			Array values = Enum.GetValues(enumType);
			foreach(object value in values) {
				string name = Enum.GetName(enumType, value);
				if(name == enumString)
					return value;
			}
			throw new InvalidCastException();
		}
		public static void WriteColor(XmlWriter xmlWriter, Color color, string key) {
			xmlWriter.WriteElementString(key, Color2String(color));
		}
		public static Color ReadColor(XmlReader xmlReader, string key) {
			string colorString = xmlReader.ReadElementString(key);
			return String2Color(colorString);
		}
		public static void WriteEnum(XmlWriter xmlWriter, Enum enumValue, string key) {
			xmlWriter.WriteElementString(key, Enum2String(enumValue));
		}
		public static object ReadEnum(XmlReader xmlReader, string key, Type enumType) {
			string enumString = xmlReader.ReadElementString(key);
			return String2Enum(enumString, enumType);
		}
		public static void WriteInteger(XmlWriter xmlWriter, int value, string key) {
			xmlWriter.WriteElementString(key, value.ToString());
		}
		public static int ReadInteger(XmlReader xmlReader, string key) {
			string integerString = xmlReader.ReadElementString(key);
			return Convert.ToInt32(integerString);
		}
		public static float ReadFloat(XmlReader xmlReader, string key) {
			string floatString = xmlReader.ReadElementString(key);
			return Convert.ToSingle(floatString);
		}
		public static void WriteBoolean(XmlWriter xmlWriter, bool value, string key) {
			xmlWriter.WriteElementString(key, value.ToString());
		}
		public static bool ReadBoolean(XmlReader xmlReader, string key) {
			string booleanString = xmlReader.ReadElementString(key);
			return Convert.ToBoolean(booleanString);
		}
		public static void WriteFont(XmlWriter xmlWriter, Font font, string key) {
			xmlWriter.WriteStartElement(key);
			xmlWriter.WriteElementString(xmlFontName, font.Name);
			xmlWriter.WriteElementString(xmlFontSize, font.Size.ToString());
			xmlWriter.WriteElementString(xmlFontBold, font.Bold.ToString());
			xmlWriter.WriteElementString(xmlFontItalic, font.Italic.ToString());
			xmlWriter.WriteElementString(xmlFontUnderline, font.Underline.ToString());
			xmlWriter.WriteElementString(xmlFontStrikeout, font.Strikeout.ToString());
			xmlWriter.WriteEndElement();
		}
		public static Font ReadFont(XmlReader xmlReader, string key) {
			xmlReader.ReadStartElement(key);
			string name = xmlReader.ReadElementString(xmlFontName);
			float size = ReadFloat(xmlReader, xmlFontSize);
			bool bold = ReadBoolean(xmlReader, xmlFontBold);
			bool italic = ReadBoolean(xmlReader, xmlFontItalic);
			bool underline = ReadBoolean(xmlReader, xmlFontUnderline);
			bool strikeout = ReadBoolean(xmlReader, xmlFontStrikeout);
			xmlReader.ReadEndElement();
			FontStyle style = FontStyle.Regular;
			if(bold)
				style |= FontStyle.Bold;
			if(italic)
				style |= FontStyle.Italic;
			if(underline)
				style |= FontStyle.Underline;
			if(strikeout)
				style |= FontStyle.Strikeout;
			return new Font(name, size, style);
		}
		public static void ReadFillStyle(XmlReader xmlReader, FillStyleBase fillStyle, string key) {
			xmlReader.ReadStartElement(key);
				fillStyle.ReadFromXml(xmlReader);
			xmlReader.ReadEndElement();
		}
		public static void ReadRectangleIndents(XmlReader xmlReader, RectangleIndents indents, string key) {
			xmlReader.ReadStartElement(key);
				int left = ReadInteger(xmlReader, "Left");
				int top = ReadInteger(xmlReader, "Top");
				int right = ReadInteger(xmlReader, "Right");
				int bottom = ReadInteger(xmlReader, "Bottom");
				indents.SetIndents(left, top, right, bottom);
			xmlReader.ReadEndElement();
		}
		XmlUtils() {
		}
	}	
}
