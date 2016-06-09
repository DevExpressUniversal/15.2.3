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
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ScatterChartViewDestination
	public class ScatterChartViewDestination : ChartViewDestinationBase {
		static Dictionary<string, ScatterChartStyle> scatterChartStyleTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporter.ScatterChartStyleTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			result.Add("scatterStyle", OnScatterStyle);
			result.Add("varyColors", OnVaryColors);
			result.Add("ser", OnScatterChartSeries);
			result.Add("dLbls", OnDataLabels);
			return result;
		}
		static ScatterChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ScatterChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ScatterChartView view;
		#endregion
		public ScatterChartViewDestination(SpreadsheetMLBaseImporter importer, ScatterChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal ScatterChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnScatterStyle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScatterChartView view = GetThis(importer).view;
			return new EnumValueDestination<ScatterChartStyle>(importer,
				scatterChartStyleTable,
				delegate(ScatterChartStyle value) { view.SetScatterStyleCore(value); },
				"val", ScatterChartStyle.Marker);
		}
		static Destination OnScatterChartSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewBase view = GetThis(importer).view;
			ScatterSeries series = new ScatterSeries(view);
			view.Series.AddCore(series);
			return new ScatterChartSeriesDestination(importer, series, view.Series.Count <= 1);
		}
		static Destination OnVaryColors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScatterChartView view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.VaryColors = value; },
				"val", true);
		}
		#endregion
	}
	#endregion
}
