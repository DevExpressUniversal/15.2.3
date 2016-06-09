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
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Data.Helpers;
using DevExpress.Compatibility.System.Drawing;
#if !DXPORTABLE
using DevExpress.Pdf;
using System.Security.Permissions;
#endif
#if SL
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	public class BrickDocumentPrinter : DocumentPrinter {
		readonly bool useGdiPlus;
		public BrickDocumentPrinter(DocumentModel documentModel, bool useGdiPlus)
			: base(documentModel) {
			this.useGdiPlus = useGdiPlus;
		}
		protected internal override BoxMeasurer CreateMeasurer(Graphics gr) {
#if !SL && !DXPORTABLE
			if (DevExpress.Office.Drawing.PrecalculatedMetricsFontCacheManager.ShouldUse() || (DocumentModel.PrintingOptions.DrawLayoutFromSilverlightRendering && DocumentModel.ModelForExport))
				return new PrecalculatedMetricsBoxMeasurer(DocumentModel);
			if (useGdiPlus)
				return new GdiPlusBoxMeasurer(DocumentModel, gr);
			else
				return CreateGdiBoxMeasurer(gr);
#else
			return new PrecalculatedMetricsBoxMeasurer(DocumentModel);
#endif
		}
#if !SL && !DXPORTABLE
		protected virtual BoxMeasurer CreateGdiBoxMeasurer(Graphics gr) {
			return new GdiBoxMeasurer(DocumentModel, gr);
		}
#endif
		protected internal override DocumentFormattingController CreateDocumentFormattingController(DocumentLayout documentLayout) {
			return new PrintLayoutViewDocumentFormattingController(documentLayout, PieceTable);
		}
		protected internal override DocumentPrinterController CreateDocumentPrinterController() {
			return new PlatformDocumentPrinterController();
		}
	}
#if !SL && !DXPORTABLE
	public class PdfBrickDocumentPrinter : BrickDocumentPrinter {
		PdfCreationOptions creationOptions;
		public PdfBrickDocumentPrinter(DocumentModel documentModel, bool useGdiPlus, PdfCreationOptions creationOptions) 
			: base(documentModel, useGdiPlus) {
				this.creationOptions = creationOptions;
		}
		protected override BoxMeasurer CreateGdiBoxMeasurer(Graphics gr) {
			return new PdfBoxMeasurer(DocumentModel, gr, gr, this.creationOptions);
		}
	}
 #endif
}
