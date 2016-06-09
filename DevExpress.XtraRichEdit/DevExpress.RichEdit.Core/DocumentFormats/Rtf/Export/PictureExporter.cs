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
using System.Drawing;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Export.Rtf {
	public abstract class RtfPictureExportStrategy {
		protected abstract void ExportShapePicturePrefix(RtfBuilder rtfBuilder);
		protected abstract void ExportShapePicturePostfix(RtfBuilder rtfBuilder);
		protected abstract void ExportNonShapePicturePrefix(RtfBuilder rtfBuilder);
		protected abstract void ExportNonShapePicturePostfix(RtfBuilder rtfBuilder);
		public virtual void Export(RtfBuilder rtfBuilder, IPictureContainerRun run, bool duplicateObjectAsMetafile) {
			OfficeImageFormat format = run.PictureContent.Image.RawFormat;
			if (!format.Equals(OfficeImageFormat.Wmf)) {
				ExportShapePicturePrefix(rtfBuilder);
				RtfPictureExporter exporter = RtfPictureExporter.CreateRtfPictureExporter(rtfBuilder, run, format);
				exporter.Export();
				ExportShapePicturePostfix(rtfBuilder);
				if (duplicateObjectAsMetafile && run.PictureContent.Image.CanGetImageBytes(OfficeImageFormat.Wmf)) {
					RtfPictureExporter wmfExporter = RtfPictureExporter.CreateRtfPictureExporter(rtfBuilder, run, OfficeImageFormat.Wmf);
					if (wmfExporter != null) {
						ExportNonShapePicturePrefix(rtfBuilder);
						wmfExporter.Export();
						ExportNonShapePicturePostfix(rtfBuilder);
					}
				}
			}
			else {
				RtfPictureExporter wmfExporter = RtfPictureExporter.CreateRtfPictureExporter(rtfBuilder, run, OfficeImageFormat.Wmf);
				if (wmfExporter != null)
					wmfExporter.Export();
			}
		}
	}
	public class RtfInlinePictureExportStrategy : RtfPictureExportStrategy {
		protected override void ExportShapePicturePrefix(RtfBuilder rtfBuilder) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ShapePicture);
		}
		protected override void ExportShapePicturePostfix(RtfBuilder rtfBuilder) {
			rtfBuilder.CloseGroup();
		}
		protected override void ExportNonShapePicturePrefix(RtfBuilder rtfBuilder) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.NonShapePicture);
		}
		protected override void ExportNonShapePicturePostfix(RtfBuilder rtfBuilder) {
			rtfBuilder.CloseGroup();
		}
	}
	public class RtfFloatingObjectPictureExportStrategy : RtfPictureExportStrategy {
		protected override void ExportShapePicturePrefix(RtfBuilder rtfBuilder) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ShapeProperty);
			rtfBuilder.WriteShapePropertyName("pib");
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ShapePropertyValue);
		}
		protected override void ExportShapePicturePostfix(RtfBuilder rtfBuilder) {
			rtfBuilder.CloseGroup();
			rtfBuilder.CloseGroup();
		}
		protected override void ExportNonShapePicturePrefix(RtfBuilder rtfBuilder) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ShapeResult);
			rtfBuilder.WriteCommand(RtfExportSR.EndOfParagraph);
			rtfBuilder.WriteCommand(RtfExportSR.ResetParagraphProperties);
		}
		protected override void ExportNonShapePicturePostfix(RtfBuilder rtfBuilder) {
			rtfBuilder.CloseGroup();
		}
	}
	#region RtfPictureExporter (abstract class)
	public abstract class RtfPictureExporter {
		 internal static RtfPictureExporter CreateRtfPictureExporter(RtfBuilder rtfBuilder, IPictureContainerRun run, OfficeImageFormat imageFormat) {
			if (!run.PictureContent.Image.IsExportSupported(imageFormat))
				return null;
			if (imageFormat.Equals(OfficeImageFormat.Wmf))
				return new RtfWmfPictureExporter(rtfBuilder, run);
			if (imageFormat.Equals(OfficeImageFormat.Emf))
				return new RtfEmfPictureExporter(rtfBuilder, run);
			if (imageFormat.Equals(OfficeImageFormat.Jpeg))
				return new RtfJpegPictureExporter(rtfBuilder, run);
			return new RtfPngPictureExporter(rtfBuilder, run);
		}
		#region Fields
		readonly RtfBuilder rtfBuilder;
		readonly IPictureContainerRun run;
		#endregion
		protected RtfPictureExporter(RtfBuilder rtfBuilder, IPictureContainerRun run) {
			this.rtfBuilder = rtfBuilder;
			this.run = run;
		}
		#region Properties
		protected RtfBuilder RtfBuilder { get { return rtfBuilder; } }
		protected IPictureContainerRun Run { get { return run; } }
		protected OfficeImage Image { get { return run.PictureContent.Image; } }
		protected internal DocumentModelUnitConverter UnitConverter { get { return Run.PictureContent.Paragraph.DocumentModel.UnitConverter; } }
		#endregion
		public virtual void Export() {
			RtfBuilder.OpenGroup();
			WritePictureHeader();
			try {
				RtfBuilder.WriteStreamAsHex(GetImageBytesStream());
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected abstract Size GetPictureSize();
		protected abstract Size GetDesiredPictureSize();
		protected abstract SizeF GetPictureScale();
		protected abstract string RtfPictureType { get; }
		protected internal abstract Stream GetImageBytesStream();
		protected void WritePictureHeader() {
			Size pirctureSize = GetPictureSize();
			Size desiredPictureSize = GetDesiredPictureSize();
			SizeF pictureScale = GetPictureScale();
			while (desiredPictureSize.Width > 0x7FFF || desiredPictureSize.Height > 0x7FFF) {
				desiredPictureSize.Width /= 2;
				desiredPictureSize.Height /= 2;
				pictureScale.Width *= 2;
				pictureScale.Height *= 2;
				pirctureSize.Width /= 2;
				pirctureSize.Height /= 2;
			}
			RtfBuilder.WriteCommand(RtfExportSR.Picture);
			RtfBuilder.WriteCommand(RtfPictureType);
			RtfBuilder.WriteCommand(RtfExportSR.PictureWidth, Math.Max(pirctureSize.Width, 1));
			RtfBuilder.WriteCommand(RtfExportSR.PictureHeight, Math.Max(pirctureSize.Height, 1));
			RtfBuilder.WriteCommand(RtfExportSR.PictureDesiredWidth, Math.Max(desiredPictureSize.Width, 1));
			RtfBuilder.WriteCommand(RtfExportSR.PictureDesiredHeight, Math.Max(desiredPictureSize.Height, 1));
			RtfBuilder.WriteCommand(RtfExportSR.PictureScaleX, Math.Max((int)Math.Round(pictureScale.Width), 1));
			RtfBuilder.WriteCommand(RtfExportSR.PictureScaleY, Math.Max((int)Math.Round(pictureScale.Height), 1));
			if (!String.IsNullOrEmpty(Image.Uri)) {
				RtfBuilder.OpenGroup();
				try {
					RtfBuilder.WriteCommand(RtfExportSR.DxImageUri);
					RtfBuilder.WriteText(Image.Uri);
				}
				finally {
					RtfBuilder.CloseGroup();
				}
			}
		}
	}
	#endregion
}
