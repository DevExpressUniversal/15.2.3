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
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		internal void GenerateDrawingBlipFillContent(DrawingBlipFill fill) {
			WriteStartElement("blipFill", DrawingMLNamespace);
			try {
				WriteIntValue("dpi", fill.Dpi);
				WriteBoolValue("rotWithShape", fill.RotateWithShape);
				GenerateDrawingBlipContent(fill.Blip);
				GenerateRelativeRectContent("srcRect", fill.SourceRectangle);
				if (fill.Stretch)
					GenerateDrawingBlipFillStretch(fill);
				else
					GenerateDrawingBlipFillTile(fill);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingBlipContent(DrawingBlip blip) {
			if (IsDefaultDrawingBlip(blip))
				return;
			GenerateDrawingBlipContentCore(blip, blip.Image);
		}
		void GenerateDrawingBlipContentCore(DrawingBlip blip, OfficeImage currentImage) {
			WriteStartElement("blip", DrawingMLNamespace);
			try {
				WriteStringAttr("xmlns", "r", null, RelsNamespace);
				if (currentImage != null) {
					if (blip.Embedded) {
						string imageRelationId = ExportImageData(blip.DocumentModel, currentImage);
						WriteStringAttr("r", "embed", RelsNamespace, imageRelationId);
					} else {
						string imageRelationId = ExportExternalImageData(blip.Link, currentImage);
						WriteStringAttr("r", "link", RelsNamespace, imageRelationId);
					}
				}
				if (blip.CompressionState != CompressionState.None) {
					string cState;
					if (DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.CompressionStateTable.TryGetValue(blip.CompressionState, out cState))
						WriteStringValue("cstate", cState);
				}
				GenerateDrawingEffectCollectionContent(blip.Effects);
			} finally {
				WriteEndElement();
			}
		}
		bool IsDefaultDrawingBlip(DrawingBlip blip) {
			return blip.IsEmpty && blip.Image == null;
		}
		void GenerateDrawingBlipFillStretch(DrawingBlipFill fill) {
			WriteStartElement("stretch", DrawingMLNamespace);
			try {
				GenerateRelativeRectContent("fillRect", fill.FillRectangle);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingBlipFillTile(DrawingBlipFill fill) {
			WriteStartElement("tile", DrawingMLNamespace);
			try {
				WriteStringValue("algn", DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.RectangleAlignTypeTable[fill.TileAlign]);
				WriteStringValue("flip", DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.TileFlipTypeTable[fill.TileFlip]);
				WriteIntValue("sx", fill.ScaleX);
				WriteIntValue("sy", fill.ScaleY);
				WriteLongValue("tx", Workbook.UnitConverter.ModelUnitsToEmuL(fill.OffsetX));
				WriteLongValue("ty", Workbook.UnitConverter.ModelUnitsToEmuL(fill.OffsetY));
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
