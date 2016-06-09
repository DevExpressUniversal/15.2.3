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
	#region BubbleChartViewDestination
	public class BubbleChartViewDestination : ChartViewDestinationBase {
		static Dictionary<string, SizeRepresentsType> sizeRepresentsTable = DictionaryUtils.CreateBackTranslationTable<SizeRepresentsType>(OpenXmlExporter.SizeRepresentsTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			result.Add("varyColors", OnVaryColors);
			result.Add("ser", OnSeries);
			result.Add("dLbls", OnDataLabels);
			result.Add("bubble3D", OnBubble3D);
			result.Add("bubbleScale", OnBubbleScale);
			result.Add("showNegBubbles", OnShowNegBubbles);
			result.Add("sizeRepresents", OnSizeRepresents);
			return result;
		}
		static BubbleChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BubbleChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly BubbleChartView view;
		#endregion
		public BubbleChartViewDestination(SpreadsheetMLBaseImporter importer, BubbleChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnVaryColors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithVaryColors view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.VaryColors = value; },
				"val", true);
		}
		static Destination OnSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewBase view = GetThis(importer).view;
			BubbleSeries series = new BubbleSeries(view);
			view.Series.AddCore(series);
			return new BubbleChartSeriesDestination(importer, series, view.Series.Count <= 1);
		}
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnBubble3D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleChartView view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.Bubble3D = value; },
				"val", true);
		}
		static Destination OnBubbleScale(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 300)
						importer.ThrowInvalidFile();
					view.SetBubbleScaleCore(value);
				},
				"val", 100);
		}
		static Destination OnShowNegBubbles(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleChartView view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.ShowNegBubbles = value; },
				"val", true);
		}
		static Destination OnSizeRepresents(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleChartView view = GetThis(importer).view;
			return new EnumValueDestination<SizeRepresentsType>(importer,
				sizeRepresentsTable,
				delegate(SizeRepresentsType value) { view.SizeRepresents = value; },
				"val", SizeRepresentsType.Area);
		}
		#endregion
	}
	#endregion
}
