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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		void GenerateDataPoints(DataPointCollection dataPoints) {
			foreach (DataPoint item in dataPoints)
				GenerateDataPoint(item);
		}
		protected internal void GenerateDataPoint(DataPoint dataPoint) {
			WriteStartElement("dPt", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("idx", dataPoint.Index);
				GenerateChartSimpleBoolAttributeTag("invertIfNegative", dataPoint.InvertIfNegative);
				GenerateMarker(dataPoint.Marker);
				GenerateChartSimpleBoolAttributeTag("bubble3D", dataPoint.Bubble3D);
				if (dataPoint.HasExplosion)
					GenerateChartSimpleIntAttributeTag("explosion", dataPoint.Explosion);
				GenerateChartShapeProperties(dataPoint.ShapeProperties);
				if (dataPoint.ShapeProperties.Fill.FillType == DrawingFillType.Picture)
					GenerateChartPictureOptions(dataPoint.PictureOptions);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateMarker(Marker marker) {
			if (marker.Symbol == MarkerStyle.Auto)
				return;
			WriteStartElement("marker", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleStringAttributeTag("symbol", MarkerStyleTable[marker.Symbol]);
				if (marker.Symbol != MarkerStyle.None) {
					GenerateChartSimpleIntAttributeTag("size", marker.Size);
					GenerateChartShapeProperties(marker.ShapeProperties);
				}
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
