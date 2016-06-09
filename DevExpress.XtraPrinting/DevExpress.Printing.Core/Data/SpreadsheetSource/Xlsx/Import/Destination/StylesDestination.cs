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
using DevExpress.SpreadsheetSource.Implementation;
using DevExpress.SpreadsheetSource.Xlsx.Import.Internal;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region StylesDestination
	public class StylesDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("numFmts", OnNumberFormats);
			result.Add("cellXfs", OnCellFormats);
			return result;
		}
		#endregion
		public StylesDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnNumberFormats(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new NumberFormatsDestination(importer);
		}
		static Destination OnCellFormats(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new CellFormatsDestination(importer);
		}
	}
	#endregion
	#region CellFormatsDestination
	public class CellFormatsDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("xf", OnCellFormat);
			return result;
		}
		#endregion
		public CellFormatsDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCellFormat(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new CellFormatDestination(importer);
		}
	}
	#endregion
	#region CellFormatDestination
	public class CellFormatDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		public CellFormatDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int numberFormatId = Importer.GetWpSTIntegerValue(reader, "numFmtId", 0);
			Importer.Source.NumberFormatIds.Add(numberFormatId);
		}
	}
	#endregion
}
