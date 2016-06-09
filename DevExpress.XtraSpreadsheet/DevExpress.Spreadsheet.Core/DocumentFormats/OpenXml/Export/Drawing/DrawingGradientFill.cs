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
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		internal void GenerateDrawingGradientFillContent(DrawingGradientFill fill) {
			WriteStartElement("gradFill", DrawingMLNamespace);
			try {
				WriteEnumValue("flip", fill.Flip, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.TileFlipTypeTable, TileFlipType.None);
				WriteBoolValue("rotWithShape", fill.RotateWithShape);
				GenerateDrawingGradientStopList(fill);
				if (fill.GradientType == GradientType.Linear)
					GenerateDrawingGradientLinearContent(fill);
				else
					GenerateDrawingGradientPathContent(fill);
				GenerateRelativeRectContent("tileRect", fill.TileRect);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingGradientStopList(DrawingGradientFill fill) {
			WriteStartElement("gsLst", DrawingMLNamespace);
			try {
				foreach (DrawingGradientStop item in fill.GradientStops)
					GenerateDrawingGradientStop(item);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingGradientStop(DrawingGradientStop gradientStop) {
			WriteStartElement("gs", DrawingMLNamespace);
			try {
				WriteIntValue("pos", gradientStop.Position);
				GenerateDrawingColorContent(gradientStop.Color);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingGradientLinearContent(DrawingGradientFill fill) {
			WriteStartElement("lin", DrawingMLNamespace);
			try {
				WriteIntValue("ang", Workbook.UnitConverter.ModelUnitsToAdjAngle(fill.Angle));
				WriteBoolValue("scaled", fill.Scaled);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingGradientPathContent(DrawingGradientFill fill) {
			WriteStartElement("path", DrawingMLNamespace);
			try {
				WriteStringValue("path", DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.GradientTypeTable[fill.GradientType]);
				GenerateRelativeRectContent("fillToRect", fill.FillRect);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateRelativeRectContent(string tag, RectangleOffset rect) {
			WriteStartElement(tag, DrawingMLNamespace);
			try {
				WriteIntValue("l", rect.LeftOffset, 0);
				WriteIntValue("t", rect.TopOffset, 0);
				WriteIntValue("r", rect.RightOffset, 0);
				WriteIntValue("b", rect.BottomOffset, 0);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
