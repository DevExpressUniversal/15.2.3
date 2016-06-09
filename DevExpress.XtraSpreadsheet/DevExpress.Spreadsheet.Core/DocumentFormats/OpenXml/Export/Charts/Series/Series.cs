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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GenerateChartViewSeries(IChartView view) {
			SeriesExportWalker walker = new SeriesExportWalker(this);
			foreach (ISeries series in view.Series)
				series.Visit(walker);
		}
		void GenerateSeriesSharedContent(SeriesBase series) {
			GenerateChartSimpleIntAttributeTag("idx", series.Index);
			GenerateChartSimpleIntAttributeTag("order", series.Order);
			GenerateChartTextContent(series.Text);
			GenerateChartShapeProperties(series.ShapeProperties);
		}
	}
	#region SeriesExportWalker
	public class SeriesExportWalker : ISeriesVisitor {
		readonly OpenXmlExporter exporter;
		public SeriesExportWalker(OpenXmlExporter exporter) {
			this.exporter = exporter;
		}
		#region ISeriesVisitor Members
		public void Visit(AreaSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(BarSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(BubbleSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(LineSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(PieSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(RadarSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(ScatterSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		public void Visit(SurfaceSeries series) {
			exporter.GenerateSeriesContent(series);
		}
		#endregion
	}
	#endregion
}
