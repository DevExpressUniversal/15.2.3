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
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PieChartViewDestinationBase
	public abstract class PieChartViewDestinationBase : ChartViewDestinationBase {
		#region Handler Table
		protected static void AddCommonHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("dLbls", OnDataLabels);
			table.Add("ser", OnPieChartSeries);
			table.Add("varyColors", OnVaryColours);
		}
		static PieChartViewDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (PieChartViewDestinationBase)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ChartViewWithVaryColors view;
		#endregion
		protected PieChartViewDestinationBase(SpreadsheetMLBaseImporter importer, ChartViewWithVaryColors view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Handlers
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnPieChartSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewBase view = GetThis(importer).view;
			PieSeries series = new PieSeries(view);
			view.Series.AddCore(series);
			return new PieChartSeriesDestination(importer, series, view.Series.Count <= 1);
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
	#region PieChartViewDestination
	public class PieChartViewDestination : PieChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			result.Add("firstSliceAng", OnFirstSliceAngle);
			return result;
		}
		static PieChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PieChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly PieChartView view;
		#endregion
		public PieChartViewDestination(SpreadsheetMLBaseImporter importer, PieChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal PieChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnFirstSliceAngle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PieChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 360)
						importer.ThrowInvalidFile();
					view.SetFirstSliceAngleCore(value);
				},
				"val",
				0);
		}
		#endregion
	}
	#endregion
	#region Pie3DChartViewDestination
	public class Pie3DChartViewDestination : PieChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		public Pie3DChartViewDestination(SpreadsheetMLBaseImporter importer, Pie3DChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
}
