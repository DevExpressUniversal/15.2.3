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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CellFormatsDestination
	public class CellFormatsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("xf", OnCellFormat);
			return result;
		}
		static CellFormatsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CellFormatsDestination)importer.PeekDestination();
		}
		static Destination OnCellFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CellFormatsDestination destination = GetThis(importer); 
			if (destination.IsStyleFormats)
				return new CellStyleFormatDestination(importer);
			else
				return new CellFormatDestination(importer);
		}
		#endregion
		#region Fields
		readonly bool isStyleFormats;
		#endregion
		public CellFormatsDestination(SpreadsheetMLBaseImporter importer, bool isStyleFormats)
			: base(importer) {
			this.isStyleFormats = isStyleFormats;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected bool IsStyleFormats { get { return isStyleFormats; } }
		#endregion
	}
	#endregion
	#region CellFormatDestination
	public class CellFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("alignment", OnCellAlignment);
			result.Add("protection", OnCellProtection);
			result.Add("extLst", OnFutureFeatureDataStorageArea);
			return result;
		}
		static CellFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CellFormatDestination)importer.PeekDestination();
		}
		static Destination OnCellAlignment(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CellAlignmentDestination(importer, GetThis(importer).info);
		}
		static Destination OnCellProtection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CellProtectionDestination(importer, GetThis(importer).info);
		}
		static Destination OnFutureFeatureDataStorageArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
		readonly ImportCellFormatInfo info;
		public CellFormatDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
				this.info = CreateCellFormatInfo();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ImportCellFormatInfo Info { get { return info; } }
		protected virtual bool DefaultApplyValue { get { return false; } }
		protected internal virtual ImportCellFormatInfo CreateCellFormatInfo() {
			return new ImportCellFormatInfo(CellFormatFlagsInfo.DefaultFormat);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			info.FontId = Importer.GetWpSTIntegerValue(reader, "fontId", 0);
			info.FillId = Importer.GetWpSTIntegerValue(reader, "fillId", 0);
			info.BorderId = Importer.GetWpSTIntegerValue(reader, "borderId", 0);
			info.NumberFormatId = Importer.GetWpSTIntegerValue(reader, "numFmtId", 0);
			info.StyleId = Importer.GetWpSTIntegerValue(reader, "xfId", ImportCellFormatInfo.DefaultStyleId);
			info.QuotePrefix = Importer.GetWpSTOnOffValue(reader, "quotePrefix", false);
			info.PivotButton = Importer.GetWpSTOnOffValue(reader, "pivotButton", false);
			info.ApplyNumberFormat = Importer.GetWpSTOnOffValue(reader, "applyNumberFormat", DefaultApplyValue);
			info.ApplyFont = Importer.GetWpSTOnOffValue(reader, "applyFont", DefaultApplyValue);
			info.ApplyFill = Importer.GetWpSTOnOffValue(reader, "applyFill", DefaultApplyValue);
			info.ApplyBorder = Importer.GetWpSTOnOffValue(reader, "applyBorder", DefaultApplyValue);
			info.ApplyAlignment = Importer.GetWpSTOnOffValue(reader, "applyAlignment", DefaultApplyValue);
			info.ApplyProtection = Importer.GetWpSTOnOffValue(reader, "applyProtection", DefaultApplyValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			RegisterFormat(info);
		}
		protected virtual void RegisterFormat(ImportCellFormatInfo info) {
			Importer.StyleSheet.RegisterCellFormat(info);
		}
	}
	#endregion
	#region CellStyleFormatDestination
	public class CellStyleFormatDestination : CellFormatDestination {
		public CellStyleFormatDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override void RegisterFormat(ImportCellFormatInfo info) {
			Importer.StyleSheet.RegisterCellStyleFormat(info);
		}
		protected internal override ImportCellFormatInfo CreateCellFormatInfo() {
			return new ImportCellFormatInfo(CellFormatFlagsInfo.DefaultStyle);
		}
		protected override bool DefaultApplyValue { get { return true; } }
	}
	#endregion
}
