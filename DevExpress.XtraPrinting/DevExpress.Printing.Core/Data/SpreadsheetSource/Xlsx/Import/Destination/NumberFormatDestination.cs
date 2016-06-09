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

using DevExpress.Office;
using System;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region NumberFormatsDestination
	public class NumberFormatsDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("numFmt", OnNumberFormat);
			return result;
		}
		#endregion
		public NumberFormatsDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnNumberFormat(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new NumberFormatDestination(importer);
		}
	}
	#endregion
	#region NumberFormatDestination
	public class NumberFormatDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		int numberFormatId;
		string numberFormatCode;
		public NumberFormatDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			numberFormatId = Importer.GetIntegerValue(reader, "numFmtId", Int32.MinValue);
			if (numberFormatId == Int32.MinValue)
				Importer.ThrowInvalidFile("numFmtId is not specified");
			numberFormatCode = reader.GetAttribute("formatCode");
			if (numberFormatCode == null)
				Importer.ThrowInvalidFile("formatCode is not specified");
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.Source.NumberFormatCodes.Add(numberFormatId, numberFormatCode);
		}
	}
	#endregion
}
