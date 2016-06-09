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
	#region SurfaceChartViewDestination
	public class SurfaceChartViewDestination : ChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			result.Add("wireframe", OnWireframe);
			result.Add("ser", OnSeries);
			result.Add("bandFmts", OnBandFormats);
			return result;
		}
		static SurfaceChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SurfaceChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly SurfaceChartViewBase view;
		#endregion
		public SurfaceChartViewDestination(SpreadsheetMLBaseImporter importer, SurfaceChartViewBase view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnWireframe(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SurfaceChartViewBase view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.Wireframe = value; },
				"val", true);
		}
		static Destination OnSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewBase view = GetThis(importer).view;
			SurfaceSeries series = new SurfaceSeries(view);
			view.Series.AddCore(series);
			return new SurfaceChartSeriesDestination(importer, series, view.Series.Count <= 1);
		}
		static Destination OnBandFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BandFormatsDestination(importer, GetThis(importer).view.BandFormats);
		}
		#endregion
	}
	#endregion
	#region BandFormatsDestination
	public class BandFormatsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("bandFmt", OnBandFormat);
			return result;
		}
		static BandFormatsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BandFormatsDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly BandFormatCollection bandFormats;
		#endregion
		public BandFormatsDestination(SpreadsheetMLBaseImporter importer, BandFormatCollection bandFormats)
			: base(importer) {
			this.bandFormats = bandFormats;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnBandFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BandFormat format = new BandFormat(GetThis(importer).bandFormats.Parent, 0);
			GetThis(importer).bandFormats.AddCore(format);
			return new BandFormatDestination(importer, format);
		}
		#endregion
	}
	#endregion
	#region BandFormatDestination
	public class BandFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("idx", OnIndex);
			result.Add("spPr", OnShapeProperties);
			return result;
		}
		static BandFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BandFormatDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly BandFormat format;
		#endregion
		public BandFormatDestination(SpreadsheetMLBaseImporter importer, BandFormat format)
			: base(importer) {
			this.format = format;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BandFormat format = GetThis(importer).format;
			return new IntegerValueDestination(importer,
				delegate(int value) { format.SetBandIdCore(value); },
				"val", 0);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).format.ShapeProperties);
		}
		#endregion
	}
	#endregion
}
