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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region StyleSheetDestination
	public class StyleSheetDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("numFmts", OnNumberFormats);
			result.Add("fonts", OnFonts);
			result.Add("fills", OnFills);
			result.Add("borders", OnBorders);
			result.Add("cellStyleXfs", OnStyleFormats);
			result.Add("cellXfs", OnCellFormats);
			result.Add("cellStyles", OnCellStyles);
			result.Add("dxfs", OnDifferentialFormats); 
			result.Add("tableStyles", OnTableStyles);		 
			result.Add("colors", OnColors);
			result.Add("extLst", OnFutureFeatureDataStorageArea);
			return result;
		}
		static Destination OnNumberFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NumberFormatsDestination(importer);
		}
		static Destination OnFonts(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FontsDestination(importer);
		}
		static Destination OnFills(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FillsDestination(importer);
		}
		static Destination OnBorders(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BordersDestination(importer);
		} 
		static Destination OnStyleFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CellFormatsDestination(importer, true);
		}
		static Destination OnCellFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CellFormatsDestination(importer, false);
		}
		static Destination OnCellStyles(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CellStylesDestination(importer);
		}
		static Destination OnDifferentialFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialFormatsDestination(importer);
		}
		static Destination OnTableStyles(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableStylesDestination(importer);
		}
		static Destination OnColors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorsDestination(importer);
		}
		static Destination OnFutureFeatureDataStorageArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FutureFeatureDataStorageAreaDestination(importer);
		}
		#endregion
		public StyleSheetDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			Importer.StyleSheet.Update();
		}
	}
	#endregion
}
