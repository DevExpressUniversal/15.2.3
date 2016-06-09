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
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region ShapePropertyDestination
	public class ShapePropertyDestination : DestinationBase {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("bin", OnBinKeyword);
			table.Add("sn", OnPropertyNameKeyword);
			table.Add("sv", OnPropertyValueKeyword);
			return table;
		}
		#endregion
		RtfShapeProperties properties;
		static List<string> integerPropertyNames = CreateIntegerPropertyNames();
		string propertyName;
		public ShapePropertyDestination(RtfImporter rtfImporter, RtfShapeProperties properties)
			: base(rtfImporter) {
			this.properties = properties;
		}
		static List<string> CreateIntegerPropertyNames() {
			List<string> result = new List<string>();
			result.Add("posh");
			result.Add("posrelh");
			result.Add("posv");
			result.Add("posrelv");
			result.Add("fLayoutInCell");
			result.Add("fAllowOverlap");
			result.Add("fBehindDocument");
			result.Add("fPseudoInline");
			result.Add("fHidden");
			result.Add("dxWrapDistLeft");
			result.Add("dxWrapDistRight");
			result.Add("dyWrapDistTop");
			result.Add("dyWrapDistBottom");
			result.Add("dxTextLeft");
			result.Add("dxTextRight");
			result.Add("dyTextTop");
			result.Add("dyTextBottom");
			result.Add("fFitShapeToText");
			result.Add("fRotateText");
			result.Add("WrapText");
			result.Add("fLockAspectRatio");
			result.Add("fillColor");
			result.Add("fLine");
			result.Add("fFilled");
			result.Add("lineWidth");
			result.Add("lineColor");
			result.Add("rotation");
			result.Add("pctHoriz");
			result.Add("pctVert");
			result.Add("sizerelh");
			result.Add("sizerelv");
			result.Add("pctHorizPos");
			result.Add("pctVertPos");
			return result;
		}
		protected override void ProcessControlCharCore(char ch) {
		}
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			TranslateKeywordHandler translator;
			if (KeywordHT.TryGetValue(keyword, out translator)) {
				translator(Importer, parameterValue, hasParameter);
				return true;
			}
			return false;
		}
		protected override void ProcessCharCore(char ch) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override DestinationBase CreateClone() {
			ShapePropertyDestination clone = new ShapePropertyDestination(Importer, properties);
			clone.propertyName = propertyName;
			return clone;
		}
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			ShapePropertyNameDestination nameDest = nestedDestination as ShapePropertyNameDestination;
			if (nameDest != null) {
				if (!String.IsNullOrEmpty(nameDest.Value)) {
					properties[nameDest.Value] = null;
					propertyName = nameDest.Value;
				}
			}
			ShapePropertyPIBDestination pibDest = nestedDestination as ShapePropertyPIBDestination;
			if (pibDest != null) {
				if (!String.IsNullOrEmpty(propertyName))
					properties[propertyName] = pibDest.ImageInfo;
			}
			ShapePropertyIntegerValueDestination intDest = nestedDestination as ShapePropertyIntegerValueDestination;
			if (intDest != null) {
				if (!String.IsNullOrEmpty(propertyName))
					properties[propertyName] = intDest.Value;
			}
			ShapePropertyStringValueDestination stringDest = nestedDestination as ShapePropertyStringValueDestination;
			if (stringDest != null) {
				if (!String.IsNullOrEmpty(propertyName))
					properties[propertyName] = stringDest.Value;
			}
		}
		static void OnPropertyNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ShapePropertyNameDestination(importer);
		}
		static void OnPropertyValueKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ShapePropertyDestination thisDestination = (ShapePropertyDestination)importer.Destination;
			string name = thisDestination.propertyName;
			if (String.IsNullOrEmpty(name))
				return;
			if (name == "pib")
				importer.Destination = new ShapePropertyPIBDestination(importer);
			else if(integerPropertyNames.Contains(name))
				importer.Destination = new ShapePropertyIntegerValueDestination(importer);
			else
				importer.Destination = new ShapePropertyStringValueDestination(importer);
		}
	}
	#endregion
	#region ShapePropertyIntegerValueDestination
	public class ShapePropertyIntegerValueDestination : DestinationBase {
		int value;
		string strValue;
		public ShapePropertyIntegerValueDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return null; } }
		public int Value { get { return value; } }
		protected override void ProcessCharCore(char ch) {
			strValue += ch;
		}
		public override void BeforePopRtfState() {
			int number;
			if (Int32.TryParse(strValue, out number))
				value = number;
		}
		protected override DestinationBase CreateClone() {
			ShapePropertyIntegerValueDestination clone = new ShapePropertyIntegerValueDestination(Importer);
			clone.value = this.value;
			return clone;
		}
	}
	#endregion
	#region ShapePropertyStringValueDestination
	public class ShapePropertyStringValueDestination : DestinationBase {
		string value = String.Empty;
		public ShapePropertyStringValueDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return null; } }
		public string Value { get { return value; } }
		protected override void ProcessCharCore(char ch) {
			value += ch;
		}
		protected override DestinationBase CreateClone() {
			ShapePropertyStringValueDestination clone = new ShapePropertyStringValueDestination(Importer);
			clone.value = this.value;
			return clone;
		}
	}
	#endregion
}
