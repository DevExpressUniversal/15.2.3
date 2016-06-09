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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTableChartFormatCollectionDestination
	public class PivotTableChartFormatCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotChartFormatsCollection chartFormats;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("chartFormat", OnChartFormat);
			return result;
		}
		#endregion
		public PivotTableChartFormatCollectionDestination(SpreadsheetMLBaseImporter importer, PivotChartFormatsCollection chartFormats, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.chartFormats = chartFormats;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotChartFormatsCollection ChartFormats { get { return chartFormats; } }
		#endregion
		static PivotTableChartFormatCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableChartFormatCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnChartFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableChartFormatCollectionDestination self = GetThis(importer);
			return new PivotTableChartFormatDestination(importer, self.ChartFormats, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTableChartFormatDestination
	public class PivotTableChartFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotChartFormatsCollection chartFormats;
		PivotChartFormat chartFormat;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotArea", OnPivotArea);
			return result;
		}
		#endregion
		public PivotTableChartFormatDestination(SpreadsheetMLBaseImporter importer, PivotChartFormatsCollection chartFormats, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.chartFormats = chartFormats;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				chartFormat = new PivotChartFormat(Worksheet.Workbook);
				ChartFormats.Add(chartFormat);
				ChartFormat.SetChartIndexCore(Importer.GetWpSTIntegerValue(reader, "chart"));
				ChartFormat.SetPivotFormatIdCore(Importer.GetIntegerValue(reader, "format"));
				ChartFormat.SetSeriesFormatCore(Importer.GetWpSTOnOffValue(reader, "series", false));
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotChartFormatsCollection ChartFormats { get { return chartFormats; } }
		public PivotChartFormat ChartFormat { get { return chartFormat; } }
		#endregion
		static PivotTableChartFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableChartFormatDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableChartFormatDestination self = GetThis(importer);
			return new PivotTablePivotAreaDestination(importer, self.ChartFormat.PivotArea, self.Worksheet);
		}
		#endregion
	}
	#endregion
}
