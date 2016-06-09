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

using System.Collections.Generic;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GenerateOutlineContent(Outline outline) {
			if (outline.IsDefault)
				return;
			WriteStartElement("ln", DrawingMLNamespace);
			GenerateOutlineContentCore(outline);
		}
		protected internal void GenerateOutlineContent(Outline outline, string tagName) {
			if (outline.IsDefault)
				return;
			WriteStartElement(tagName, DrawingMLNamespace);
			GenerateOutlineContentCore(outline);
		}
		void GenerateOutlineContentCore(Outline outline) {
			OutlineInfo defaultInfo = OutlineInfo.DefaultInfo;
			try {
				WriteIntValue("w", Workbook.UnitConverter.ModelUnitsToEmu(outline.Width), outline.HasWidth);
				WriteEnumValue("cap", outline.EndCapStyle, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.EndCapStyleTable, defaultInfo.EndCapStyle);
				WriteStringValue("cmpd", DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.CompoundTypeTable[outline.CompoundType], outline.HasCompoundType);
				WriteEnumValue("algn", outline.StrokeAlignment, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.StrokeAlignmentTable, defaultInfo.StrokeAlignment);
				GenerateDrawingFillContent(outline.Fill);
				GeneratePresetDash(outline);
				GenerateLineJoinStyle(outline);
				GenerateHeadEndStyle(outline);
				GenerateTailEndStyle(outline);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateLineJoinStyle(Outline outline) {
			if (!outline.HasLineJoinStyle)
				return;
			GenerateOutlineLineJoinTag("bevel", outline, LineJoinStyle.Bevel);
			GenerateOutlineLineJoinTag("round", outline, LineJoinStyle.Round);
			GenerateOutlineLineJoinTag("miter", outline, LineJoinStyle.Miter);
		}
		void GenerateOutlineLineJoinTag(string tagName, Outline outline, LineJoinStyle defaultLineJoin) {
			if (outline.JoinStyle != defaultLineJoin)
				return;
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				if (tagName == "miter")
					WriteIntValue("lim", outline.MiterLimit);
			}
			finally {
				WriteEndElement();
			}
		}
		void GeneratePresetDash(Outline outline) {
			if (!outline.HasDashing)
				return;
			WriteStartElement("prstDash", DrawingMLNamespace);
			try {
				WriteEnumValue("val", outline.Dashing, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.PresetDashTable);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateHeadEndStyle(Outline outline) {
			if (outline.Info.IsDefaultHeadEndStyle)
				return;
			GenerateHeadTailEndStyleCore("headEnd", outline.HeadType, outline.HeadWidth, outline.HeadLength);
		}
		void GenerateTailEndStyle(Outline outline) {
			if (outline.Info.IsDefaultTailEndStyle)
				return;
			GenerateHeadTailEndStyleCore("tailEnd", outline.TailType, outline.TailWidth, outline.TailLength);
		}
		void GenerateHeadTailEndStyleCore(string tagName, OutlineHeadTailType type, OutlineHeadTailSize width, OutlineHeadTailSize length) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				WriteEnumValue("type", type, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.HeadTailTypeTable, OutlineInfo.DefaultHeadTailType);
				WriteEnumValue("w", width, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.HeadTailSizeTable, OutlineInfo.DefaultHeadTailSize);
				WriteEnumValue("len", length, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.HeadTailSizeTable, OutlineInfo.DefaultHeadTailSize);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
