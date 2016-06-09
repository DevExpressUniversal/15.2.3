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
using System.IO;
using System.Xml;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class LegendDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<string, LegendPosition> legendPositionTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.LegendPositionTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("layout", OnLayout);
			result.Add("legendEntry", OnLegendEntry);
			result.Add("legendPos", OnLegendPos);
			result.Add("overlay", OnOverlay);
			result.Add("spPr", OnShapeProperties);
			result.Add("txPr", OnTextProperties);
			return result;
		}
		static LegendDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (LegendDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Legend legend;
		#endregion
		public LegendDestination(SpreadsheetMLBaseImporter importer, Legend legend)
			: base(importer) {
				this.legend = legend;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.legend.Visible = true;
		}
		#region Handlers
		static Destination OnLegendEntry(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LegendEntryDestination(importer, GetThis(importer).legend);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).legend.ShapeProperties);
		}
		static Destination OnLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LayoutDestination(importer, GetThis(importer).legend.Layout);
		}
		static Destination OnLegendPos(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Legend legend = GetThis(importer).legend;
			return new EnumValueDestination<LegendPosition>(importer,
				legendPositionTable,
				delegate(LegendPosition value) { legend.SetPositionCore(value); },
				"val",
				LegendPosition.Right);
		}
		static Destination OnOverlay(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Legend legend = GetThis(importer).legend;
			return new OnOffValueDestination(importer,
				delegate(bool value) { legend.SetOverlayCore(value); },
				"val",
				true);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Legend legend = GetThis(importer).legend;
			return new TextPropertiesDestination(importer, legend.TextProperties);
		}
		#endregion
	}
	public class LegendEntryDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("delete", OnDelete);
			result.Add("idx", OnIdx);
			result.Add("txPr", OnTextProperties);
			return result;
		}
		static LegendEntryDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (LegendEntryDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		Legend legend;
		LegendEntry entry;
		bool isValidIndex;
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public LegendEntryDestination(SpreadsheetMLBaseImporter importer, Legend legend)
			: base(importer) {
			this.legend = legend;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			entry = new LegendEntry(legend.Parent);
			isValidIndex = true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if(isValidIndex)
				this.legend.Entries.AddWithoutHistoryAndNotifications(entry);
		}
		#region Handlers
		static Destination OnDelete(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LegendEntry entry = GetThis(importer).entry;
			return new OnOffValueDestination(importer,
				delegate(bool value) { entry.Delete = value; },
				"val", true);
		}
		static Destination OnIdx(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LegendEntryDestination destination = GetThis(importer);
			LegendEntry entry = destination.entry;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if(value >= 0)
						entry.Index = value;
					else
						destination.isValidIndex = false;
				},
				"val", 1);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LegendEntry entry = GetThis(importer).entry;
			return new TextPropertiesDestination(importer, entry.TextProperties);
		}
		#endregion
	}
}
