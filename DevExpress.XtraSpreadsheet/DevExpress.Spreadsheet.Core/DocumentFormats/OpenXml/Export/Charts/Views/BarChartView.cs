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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region BarChartShared
		void GenerateBarChartShared(BarChartViewBase view) {
			GenerateChartSimpleStringAttributeTag("barDir", BarChartDirectionTable[view.BarDirection]);
			GenerateChartSimpleStringAttributeTag("grouping", BarChartGroupingTable[view.Grouping]);
			GenerateChartSimpleBoolAttributeTag("varyColors", view.VaryColors);
			GenerateChartViewSeries(view);
			GenerateDataLabels(view.DataLabels);
		}
		#endregion
		#region BarChart
		internal void GenerateBarChartView(BarChartView view) {
			WriteStartElement("barChart", DrawingMLChartNamespace);
			try {
				GenerateBarChartShared(view);
				GenerateChartSimpleIntAttributeTag("gapWidth", view.GapWidth);
				if(view.Overlap != 0)
					GenerateChartSimpleIntAttributeTag("overlap", view.Overlap);
				foreach (ShapeProperties shapeProperties in view.SeriesLines)
					GenerateSeriesLines(shapeProperties);
				GenerateAxisGroupRef(view);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateSeriesLines(ShapeProperties shapeProperties) {
			WriteStartElement("serLines", DrawingMLChartNamespace);
			try {
				GenerateChartShapeProperties(shapeProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region Bar3DChart
		internal void GenerateBar3DChartView(Bar3DChartView view) {
			WriteStartElement("bar3DChart", DrawingMLChartNamespace);
			try {
				GenerateBarChartShared(view);
				GenerateChartSimpleIntAttributeTag("gapWidth", view.GapWidth);
				GenerateChartSimpleIntAttributeTag("gapDepth", view.GapDepth);
				GenerateChartSimpleStringAttributeTag("shape", BarChartShapeTable[view.Shape]);
				GenerateAxisGroupRef(view);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
