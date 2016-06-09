﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class LineChartSeriesDestination : SeriesBaseDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			result.Add("marker", OnMarker);
			AddDataLabelsAndPointsHandlers(result);
			AddTrendlinesAndErrorBarsHandlers(result);
			AddCatValHandlers(result);
			result.Add("smooth", OnSmooth);
			return result;
		}
		static LineChartSeriesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (LineChartSeriesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly LineSeries series;
		#endregion
		public LineChartSeriesDestination(SpreadsheetMLBaseImporter importer, LineSeries series, bool isFirstSeries)
			: base(importer, series, isFirstSeries) {
			this.series = series;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnMarker(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartMarkerDestination(importer, GetThis(importer).series.Marker);
		}
		static Destination OnSmooth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LineSeries series = GetThis(importer).series;
			return new OnOffValueDestination(importer,
				delegate(bool value) { series.SetSmoothCore(value); },
				"val", true);
		}
		#endregion
	}
}
