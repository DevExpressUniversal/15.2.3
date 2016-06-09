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
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class ChartsheetDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("customSheetViews", OnCustomSheetViews);
			result.Add("drawing", OnDrawing);
			result.Add("extLst", OnExtLst);
			result.Add("headerFooter", OnHeaderFooter);
			result.Add("legacyDrawing", OnLegacyDrawing);
			result.Add("legacyDrawingHF", OnLegacyDrawingHf);
			result.Add("pageMargins", OnPageMargins);
			result.Add("pageSetup", OnPageSetup);
			result.Add("picture", OnPicture);
			result.Add("sheetPr", OnSheetPr);
			result.Add("sheetProtection", OnSheetProtection);
			result.Add("sheetViews", OnSheetViews);
			result.Add("webPublishItems", OnWebPublishItems);
			return result;
		}
		public ChartsheetDestination(SpreadsheetMLBaseImporter importer, Chartsheet worksheet)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCustomSheetViews(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DestinationCustomViews(importer);
		}
		static Destination OnDrawing(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorksheetDrawingDestination(importer);
		}
		static Destination OnExtLst(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnHeaderFooter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chartsheet sheet = importer.CurrentSheet as Chartsheet;
			if (sheet != null) {
				return new HeaderFooterDestination(importer, sheet.Properties.HeaderFooter);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnLegacyDrawing(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnLegacyDrawingHf(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DestinationLegacyDrawingHf(importer);
		}
		static Destination OnPageMargins(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chartsheet sheet = importer.CurrentSheet as Chartsheet;
			if (sheet != null) {
				return new SheetMarginsDestination(importer, sheet.Properties.Margins);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnPageSetup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chartsheet sheet = importer.CurrentSheet as Chartsheet;
			if (sheet != null) {
				return new PageSetupDestination(importer, sheet.Properties.PrintSetup);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnPicture(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnSheetPr(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartsheetPropertiesDestination(importer);
		}
		static Destination OnSheetProtection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnSheetViews(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetViewsDestination(importer);
		}
		static Destination OnWebPublishItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DestinationWebPublishItems(importer);
		}
	}
	public class DestinationCustomViews : EmptyDestination<SpreadsheetMLBaseImporter> {
		public DestinationCustomViews(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
	}
	public class DestinationLegacyDrawingHf : EmptyDestination<SpreadsheetMLBaseImporter> {
		public DestinationLegacyDrawingHf(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
	}
	public class DestinationWebPublishItems : EmptyDestination<SpreadsheetMLBaseImporter> {
		public DestinationWebPublishItems(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
	}
	#region WorksheetPropertiesDestination
	public class ChartsheetPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tabColor", OnSheetTabColor);
			return result;
		}
		static ChartsheetPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartsheetPropertiesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ColorModelInfo tabColorInfo;
		#endregion
		public ChartsheetPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.tabColorInfo = new ColorModelInfo();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnSheetTabColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).tabColorInfo);
		}
		ChartsheetProperties GetProperties() {
			ChartsheetProperties properties = Importer.CurrentSheet.Properties as ChartsheetProperties;
			if (properties == null)
				Exceptions.ThrowInvalidOperationException("Unexpected tag sheetPr");
			return properties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ChartsheetProperties properties = GetProperties();
			string codeName = Importer.ReadAttribute(reader, "codeName");
			if (!string.IsNullOrEmpty(codeName)) {
				if (!CodeNameHelper.IsValidCodeName(codeName))
					codeName = CodeNameHelper.CleanUp(codeName);
				properties.CodeName = codeName;
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			ChartsheetProperties properties = GetProperties();
			ColorModelInfoCache cache = Importer.DocumentModel.Cache.ColorModelInfoCache;
			properties.TabColorIndex = cache.GetItemIndex(tabColorInfo);
		}
	}
	#endregion
	#region SheetViewDestination
	public class ChartsheetViewDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("extLst", OnExtensionList);
			return result;
		}
		bool tabSelected;
		int workbookViewId;
		int zoomScale;
		public ChartsheetViewDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			tabSelected = Importer.GetWpSTOnOffValue(reader, "tabSelected", false);
			zoomScale = Importer.GetWpSTIntegerValue(reader, "zoomScale", Int32.MinValue);
			if (zoomScale != Int32.MinValue)
				zoomScale = NormalizeZoomValue(zoomScale);
			workbookViewId = Importer.GetWpSTIntegerValue(reader, "workbookViewId", Int32.MinValue);
			if (workbookViewId < 0)
				Importer.ThrowInvalidFile("workbookViewId is not specified");
		}
		public override void ProcessElementClose(XmlReader reader) {
			ModelWorksheetView viewOptions = Importer.CurrentSheet.ActiveView;
			viewOptions.BeginUpdate();
			viewOptions.TabSelected = tabSelected;
			viewOptions.ZoomScale = NormalizeZoomValue(zoomScale);
			viewOptions.WorkbookViewId = workbookViewId;
			viewOptions.EndUpdate();
			base.ProcessElementClose(reader);
		}
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static int NormalizeZoomValue(int value) {
			if (value < 10 || value > 400)
				value = 100;
			return value;
		}
	}
	#endregion
}
