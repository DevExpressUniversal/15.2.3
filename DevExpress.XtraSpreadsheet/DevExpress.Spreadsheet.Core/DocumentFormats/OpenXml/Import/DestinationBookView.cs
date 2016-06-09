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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using System.Xml;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region BookViewsDestination
	public class BookViewsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("workbookView", OnWorkbookView);
			return result;
		}
		public BookViewsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			importer.DocumentModel.Properties.WorkbookWindowPropertiesList.Clear();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnWorkbookView(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorkbookViewDestination(importer);
		}
	}
	#endregion
	#region WorkbookViewDestination
	public class WorkbookViewDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		WorkbookWindowProperties viewOptions;
		public WorkbookViewDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			viewOptions = new WorkbookWindowProperties(Importer.DocumentModel);
			WorkbookWindowInfo defaultItem = Importer.DocumentModel.Cache.WindowInfoCache.DefaultItem;
			viewOptions.BeginUpdate();
			try {
				viewOptions.Visibility = Importer.GetWpEnumValue<SheetVisibleState>(reader, "view", OpenXmlImporter.visibilityTypeTable, defaultItem.Visibility);
				viewOptions.Minimized = Importer.GetWpSTOnOffValue(reader, "minimized", defaultItem.Minimized);
				viewOptions.ShowHorizontalScroll = Importer.GetWpSTOnOffValue(reader, "showHorizontalScroll", defaultItem.ShowHorizontalScroll);
				viewOptions.ShowVerticalScroll = Importer.GetWpSTOnOffValue(reader, "showVerticalScroll", defaultItem.ShowVerticalScroll);
				viewOptions.ShowSheetTabs = Importer.GetWpSTOnOffValue(reader, "showSheetTabs", defaultItem.ShowSheetTabs);
				viewOptions.HorizontalPosition = Importer.GetWpSTIntegerValue(reader, "xWindow", defaultItem.HorizontalPosition);
				viewOptions.VerticalPosition = Importer.GetWpSTIntegerValue(reader, "yWindow", defaultItem.VerticalPosition);
				viewOptions.WidhtInTwips = Importer.GetWpSTIntegerValue(reader, "windowWidth", defaultItem.WidhtInTwips);
				viewOptions.HeightInTwips = Importer.GetWpSTIntegerValue(reader, "windowHeight", defaultItem.HeightInTwips); ;
				viewOptions.TabRatio = Importer.GetWpSTIntegerValue(reader, "tabRatio", defaultItem.TabRatio);
				viewOptions.FirstDisplayedTabIndex = Importer.GetWpSTIntegerValue(reader, "firstSheet", defaultItem.FirstDisplayedTabIndex);
				viewOptions.SelectedTabIndex = Importer.GetWpSTIntegerValue(reader, "activeTab", WorkbookWindowProperties.DefaultSelectedTabIndex);
				viewOptions.AutoFilterDateGrouping = Importer.GetWpSTOnOffValue(reader, "autoFilterDateGrouping", defaultItem.AutoFilterDateGrouping);
			}
			finally {
				viewOptions.EndUpdate();
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Importer.DocumentModel.Properties.WorkbookWindowPropertiesList.Add(viewOptions);
		}
	}
	#endregion
}
