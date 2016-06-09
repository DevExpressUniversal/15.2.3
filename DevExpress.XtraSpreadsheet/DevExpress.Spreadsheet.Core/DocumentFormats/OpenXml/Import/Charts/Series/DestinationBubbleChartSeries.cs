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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class BubbleChartSeriesDestination  : SeriesBaseDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			result.Add("invertIfNegative", OnInvertIfNegative);
			AddDataLabelsAndPointsHandlers(result);
			AddTrendlinesAndErrorBarsHandlers(result);
			AddXYValHandlers(result);
			result.Add("bubbleSize", OnBubbleSize);
			result.Add("bubble3D", OnBubble3D);
			return result;
		}
		static BubbleChartSeriesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BubbleChartSeriesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly BubbleSeries series;
		IOpenXmlChartDataReference bubbleSize;
		#endregion
		public BubbleChartSeriesDestination(SpreadsheetMLBaseImporter importer, BubbleSeries series, bool isFirstSeries)
			: base(importer, series, isFirstSeries) {
			this.series = series;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override bool ArgumentAxisIsNumber { get { return true; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (bubbleSize != null)
				series.BubbleSize = bubbleSize.ToDataReference(Importer.DocumentModel, series.View.SeriesDirection, true);
		}
		#region Handlers
		static Destination OnInvertIfNegative(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleSeries series = GetThis(importer).series;
			return new OnOffValueDestination(importer,
				delegate(bool value) { series.SetInvertIfNegativeCore(value); },
				"val", true);
		}
		static Destination OnBubbleSize(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleChartSeriesDestination thisDestination = GetThis(importer);
			return new ChartDataReferenceDestination(importer,
				DataReferenceTypeOpenXml.NumberReference | DataReferenceTypeOpenXml.NumberLiteral,
				delegate(IOpenXmlChartDataReference value) { thisDestination.bubbleSize = value; });
		}
		static Destination OnBubble3D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleSeries series = GetThis(importer).series;
			return new OnOffValueDestination(importer,
				delegate(bool value) { series.SetBubble3DCore(value); },
				"val", true);
		}
		#endregion
	}
}
