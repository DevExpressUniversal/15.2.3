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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartLegendEntry
	public class XlsChartLegendEntry : IXlsChartBuilder, IXlsChartTextContainer, IXlsChartTextFormatContainer {
		#region Fields
		XlsChartTextBuilder text;
		XlsChartTextFormat textFormat = null;
		#endregion
		#region Properties
		public int Index { get; set; }
		public bool Deleted { get; set; }
		public bool Formatted { get; set; }
		public bool IsSeriesEntry { get { return Index == XlsChartDefs.PointIndexOfSeries; } }
		public XlsChartTextBuilder Text { get { return text; } }
		public XlsChartTextFormat TextFormat { get { return textFormat; } }
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
		}
		#endregion
		#region IXlsChartTextContainer Members
		void IXlsChartTextContainer.Add(XlsChartTextBuilder item) {
			this.text = item;
		}
		#endregion
		#region IXlsChartTextFormatContainer Members
		void IXlsChartTextFormatContainer.Add(XlsChartTextFormat properties) {
			textFormat = properties;
		}
		#endregion
	}
	#endregion
	#region XlsChartLegendBuilder
	public class XlsChartLegendBuilder : XlsChartTextBuilderBase, IXlsChartTextContainer {
		#region Fields
		XlsChartTextBuilder text;
		#endregion
		#region Properties
		public bool AutoPosition { get; set; }
		public bool AutoXPos { get; set; }
		public bool AutoYPos { get; set; }
		public bool IsVertical { get; set; }
		public bool WasDataTable { get; set; }
		public XlsChartTextBuilder Text { get { return text; } }
		#endregion
		public XlsChartLegendBuilder()
			: base() {
		}
		#region IXlsChartBuilder Members
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			Legend legend = contentBuilder.CurrentChart.Legend;
			legend.Visible = !WasDataTable;
			SetupLegendPosition(contentBuilder, legend);
			SetupLegendEntries(contentBuilder, legend);
			SetupShapeProperties(legend.ShapeProperties);
			SetupTextProperties(contentBuilder, legend.TextProperties);
		}
		#endregion
		void SetupLegendPosition(XlsContentBuilder contentBuilder, Legend legend) {
			legend.Overlay = Overlay;
			if (Layout12.Apply)
				legend.Position = Layout12.LegendPos;
			Layout12.SetupLayout(legend.Layout);
		}
		void SetupLegendEntries(XlsContentBuilder contentBuilder, Legend legend) {
			List<XlsChartSeriesBuilder> seriesFormats = contentBuilder.SeriesFormats;
			for (int i = 0; i < seriesFormats.Count; i++) {
				XlsChartSeriesBuilder series = seriesFormats[i];
				if (series.LegendEntries.Count == 1 && series.LegendEntries[0].IsSeriesEntry) {
					LegendEntry entry = new LegendEntry(legend.Parent);
					entry.Index = i;
					entry.Delete = series.LegendEntries[0].Deleted;
					SetupTextProperties(contentBuilder, series.LegendEntries[0], entry.TextProperties);
					legend.Entries.Add(entry);
				}
				else {
					foreach (XlsChartLegendEntry item in series.LegendEntries) {
						LegendEntry entry = new LegendEntry(legend.Parent);
						entry.Index = item.Index;
						entry.Delete = item.Deleted;
						SetupTextProperties(contentBuilder, item, entry.TextProperties);
						legend.Entries.Add(entry);
					}
				}
			}
		}
		void SetupShapeProperties(ShapeProperties shapeProperties) {
			if (Frame != null)
				Frame.SetupShapeProperties(shapeProperties);
		}
		void SetupTextProperties(XlsContentBuilder contentBuilder, TextProperties properties) {
			if (TextFormat != null )
				TextFormat.SetupTextProperties(properties);
		}
		void SetupTextProperties(XlsContentBuilder contentBuilder, XlsChartLegendEntry legendEntry, TextProperties properties) {
			if (!legendEntry.Formatted)
				return;
			if (legendEntry.TextFormat != null)
				legendEntry.TextFormat.SetupTextProperties(properties);
			else
				legendEntry.Text.SetupTextPropertiesBase(contentBuilder, properties);
		}
		#region IXlsChartTextContainer Members
		void IXlsChartTextContainer.Add(XlsChartTextBuilder item) {
			this.text = item;
		}
		#endregion
	}
	#endregion
}
