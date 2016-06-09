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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office;
using System.Globalization;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GenerateChartShapeProperties(ShapeProperties properties) {
			if(!ShouldExportChartShapeProperties(properties))
				return;
			WriteStartElement("spPr", DrawingMLChartNamespace);
			try {
				GenerateDrawingFillContent(properties.Fill);
				GenerateOutlineContent(properties.Outline, "ln");
				ExportDrawingEffectStyle(properties.EffectStyle);
			}
			finally {
				WriteEndElement();
			}
		}
		protected virtual bool ShouldExportChartShapeProperties(ShapeProperties properties) {
			return !properties.IsAutomatic;
		}
		protected internal virtual void GenerateDrawingFillContent(IDrawingFill fill) {
			if(fill.FillType == DrawingFillType.Automatic)
				return;
			DrawingFillExportWalker exportWalker = new DrawingFillExportWalker(this);
			fill.Visit(exportWalker);
		}
	}
	public class DrawingFillExportWalker : IDrawingFillVisitor {
		readonly OpenXmlExporter exporter;
		public DrawingFillExportWalker(OpenXmlExporter exporter) {
			this.exporter = exporter;
		}
		#region IDrawingFillVisitor Members
		public void Visit(DrawingFill fill) {
			if(fill.FillType == DrawingFillType.None)
				exporter.GenerateDrawingFillTag("noFill");
			else if(fill.FillType == DrawingFillType.Group)
				exporter.GenerateDrawingFillTag("grpFill");
		}
		public void Visit(DrawingSolidFill fill) {
			exporter.GenerateDrawingColorTag("solidFill", fill.Color);
		}
		public void Visit(DrawingPatternFill fill) {
			exporter.GenerateDrawingPatternFillContent(fill);
		}
		public void Visit(DrawingGradientFill fill) {
			exporter.GenerateDrawingGradientFillContent(fill);
		}
		public void Visit(DrawingBlipFill fill) {
			exporter.GenerateDrawingBlipFillContent(fill);
		}
		#endregion
	}
}
