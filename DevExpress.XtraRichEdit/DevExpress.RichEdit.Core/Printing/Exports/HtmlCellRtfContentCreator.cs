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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export;
using DevExpress.Office.Utils;
using DevExpress.Office.Export.Html;
namespace DevExpress.XtraPrinting.Exports {
	#region HtmlCellRtfContentCreator
	public class HtmlCellRtfContentCreator {
		readonly DXHtmlContainerControl cell;
		public HtmlCellRtfContentCreator(DXHtmlContainerControl cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		public void CreateContent(DocumentModel documentModel, HtmlExportContext context) {
			cell.Controls.Add(CreatePlainHtmlTextControl(documentModel, context));
		}
		DXWebControlBase CreatePlainHtmlTextControl(DocumentModel documentModel, HtmlExportContext context) {
			DXWebControlBase parent = new DXHtmlGenericControl(DXHtmlTextWriterTag.Div);
			RichEditImageRepositoryWrapper imageRepository = new RichEditImageRepositoryWrapper(context.ImageRepository);
			HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
			options.CssPropertiesExportType = CssPropertiesExportType.Style;
			options.FontUnit = HtmlFontUnit.Pixel;
			HtmlContentExporter htmlContentExporter = new HtmlContentExporter(documentModel, context.ScriptContainer, imageRepository, options);
			htmlContentExporter.Export(parent);
			return parent;
		}
	}
	#endregion
	#region RichEditImageRepositoryWrapper
	public class RichEditImageRepositoryWrapper : IOfficeImageRepository {
		readonly IImageRepository repository;
		public RichEditImageRepositoryWrapper(IImageRepository repository) {
			Guard.ArgumentNotNull(repository, "repository");
			this.repository = repository;
		}
		#region IOfficeImageRepository Members
		public string GetImageSource(OfficeImage img, bool autoDisposeImage) {
			return repository.GetImageSource(img.NativeImage, autoDisposeImage);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	#endregion
}
