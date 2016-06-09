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
using DevExpress.XtraSpreadsheet.Export.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region AreaChartViewDestinationBase
	public abstract class AreaChartViewDestinationBase : ChartViewDestinationBase {
		#region Handler Table
		protected static void AddCommonHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("grouping", OnGroupping);
			table.Add("dLbls", OnDataLabels);
			table.Add("dropLines", OnDropLines);
			table.Add("ser", OnSeries);
			table.Add("varyColors", OnVaryColours);
		}
		static AreaChartViewDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (AreaChartViewDestinationBase)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ChartViewWithGroupingAndDropLines view;
		#endregion
		protected AreaChartViewDestinationBase(SpreadsheetMLBaseImporter importer, ChartViewWithGroupingAndDropLines view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Handlers
		static Destination OnGroupping(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithGroupingAndDropLines view = GetThis(importer).view;
			return new EnumValueDestination<ChartGrouping>(importer,
				chartGroupingTable,
				delegate(ChartGrouping value) { view.Grouping = value; },
				"val",
				ChartGrouping.Standard);
		}
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnDropLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithGroupingAndDropLines view = GetThis(importer).view;
			view.ShowDropLines = true;
			return new InnerShapePropertiesDestination(importer, view.DropLinesProperties);
		}
		static Destination OnSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithGroupingAndDropLines view = GetThis(importer).view;
			AreaSeries series = new AreaSeries(view);
			view.Series.AddCore(series);
			return new AreaChartSeriesDestination(importer, series, view.Series.Count <= 1);
		}
		static Destination OnVaryColours(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithVaryColors view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.VaryColors = value; },
				"val",
				true);
		}
		#endregion
	}
	#endregion
	#region AreaChartViewDestination
	public class AreaChartViewDestination : AreaChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		public AreaChartViewDestination(SpreadsheetMLBaseImporter importer, AreaChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
	#region Area3DChartViewDestination
	public class Area3DChartViewDestination : AreaChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			AddCommonHandlers(result);
			result.Add("gapDepth", OnGapDepth);
			return result;
		}
		static Area3DChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (Area3DChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Area3DChartView view;
		#endregion
		public Area3DChartViewDestination(SpreadsheetMLBaseImporter importer, Area3DChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnGapDepth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Area3DChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 500)
						importer.ThrowInvalidFile();
					view.SetGapDepthCore(value);
				},
				"val",
				150);
		}
		#endregion
	}
	#endregion
}
