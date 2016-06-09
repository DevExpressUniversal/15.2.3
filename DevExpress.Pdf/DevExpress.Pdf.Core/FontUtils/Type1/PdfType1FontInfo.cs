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
using System.Text;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfType1FontInfo {
		const string versionDictionaryKey = "version";
		const string noticeDictionaryKey = "Notice";
		const string copyrightDictionaryKey = "Copyright";
		const string fullNameDictionaryKey = "FullName";
		const string familyNameDictionaryKey = "FamilyName";
		const string baseFontNameDictionaryKey = "BaseFontName";
		const string weightDictionaryKey = "Weight";
		const string italicAngleDictionaryKey = "ItalicAngle";
		const string isFixedPitchDictionaryKey = "isFixedPitch";
		const string underlinePositionDictionaryKey = "UnderlinePosition";
		const string underlineThicknessDictionaryKey = "UnderlineThickness";
		const string emDictionaryKey = "em";
		const string ascentDictionaryKey = "ascent";
		const string descentDictionaryKey = "descent";
		static string ToString(object value) {
			byte[] bytes = value as byte[];
			if (bytes == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return Encoding.UTF8.GetString(bytes);
		}
		static void SerializeString(StringBuilder sb, string key, string value) {
			if (value != null)
				sb.Append(String.Format("/{0} ({1}) readonly def\n", key, value));
		}
		static void SerializeDouble(StringBuilder sb, string key, double value) {
			sb.Append(String.Format(CultureInfo.InvariantCulture, PdfType1FontProgram.SerializationPattern, key, value));
		}
		static void OptionalSerializeDouble(StringBuilder sb, string key, double value) {
			if (value != 0)
				SerializeDouble(sb, key, value);
		}
		readonly string version;
		readonly string notice;
		readonly string copyright;
		readonly string fullName;
		readonly string familyName;
		readonly string baseFontName;
		readonly string weight;
		readonly double italicAngle;
		readonly bool isFixedPitch;
		readonly double underlinePosition;
		readonly double underlineThickness;
		readonly double em;
		readonly double ascent;
		readonly double descent;
		public string Version { get { return version; } }
		public string Notice { get { return notice; } }
		public string Copyright { get { return copyright; } }
		public string FullName { get { return fullName; } }
		public string FamilyName { get { return familyName; } }
		public string BaseFontName { get { return baseFontName; } }
		public string Weight { get { return weight; } }
		public double ItalicAngle { get { return italicAngle; } }
		public bool IsFixedPitch { get { return isFixedPitch; } }
		public double UnderlinePosition { get { return underlinePosition; } }
		public double UnderlineThickness { get { return underlineThickness; } }
		public double Em { get { return em; } }
		public double Ascent { get { return ascent; } }
		public double Descent { get { return descent; } }
		internal PdfType1FontInfo(PdfPostScriptDictionary dictionary) {
			Encoding encoding = Encoding.UTF8;
			foreach (KeyValuePair<string, object> pair in dictionary)
				switch (pair.Key) {
					case versionDictionaryKey:
						version = ToString(pair.Value);
						break;
					case noticeDictionaryKey:
						notice = ToString(pair.Value);
						break;
					case copyrightDictionaryKey:
						copyright = ToString(pair.Value);
						break;
					case fullNameDictionaryKey:
						fullName = ToString(pair.Value);
						break;
					case familyNameDictionaryKey:
						familyName = ToString(pair.Value);
						break;
					case baseFontNameDictionaryKey:
						baseFontName = ToString(pair.Value);
						break;
					case weightDictionaryKey:
						weight = ToString(pair.Value);
						break;
					case italicAngleDictionaryKey:
						italicAngle = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					case isFixedPitchDictionaryKey:
						object value = pair.Value;
						if (!(value is bool))
							PdfDocumentReader.ThrowIncorrectDataException();
						isFixedPitch = (bool)value;
						break;
					case underlinePositionDictionaryKey:
						underlinePosition = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					case underlineThicknessDictionaryKey:
						underlineThickness = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					case emDictionaryKey:
						em = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					case ascentDictionaryKey:
						ascent = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					case descentDictionaryKey:
						descent = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						break;
				}
		}
		internal string Serialize() {
			StringBuilder sb = new StringBuilder();
			sb.Append("9 dict dup begin\n");
			SerializeString(sb, versionDictionaryKey, version);
			SerializeString(sb, noticeDictionaryKey, notice);
			SerializeString(sb, copyrightDictionaryKey, copyright);
			SerializeString(sb, fullNameDictionaryKey, fullName);
			SerializeString(sb, familyNameDictionaryKey, familyName);
			SerializeString(sb, baseFontNameDictionaryKey, baseFontName);
			SerializeString(sb, weightDictionaryKey, weight);
			SerializeDouble(sb, italicAngleDictionaryKey, italicAngle);
			sb.Append(String.Format(PdfType1FontProgram.SerializationPattern, isFixedPitchDictionaryKey, isFixedPitch ? "true" : "false"));
			SerializeDouble(sb, underlinePositionDictionaryKey, underlinePosition);
			SerializeDouble(sb, underlineThicknessDictionaryKey, underlineThickness);
			OptionalSerializeDouble(sb, emDictionaryKey, em);
			OptionalSerializeDouble(sb, ascentDictionaryKey, ascent);
			OptionalSerializeDouble(sb, descentDictionaryKey, descent);
			sb.Append("end");
			return sb.ToString();
		}
	}
}
