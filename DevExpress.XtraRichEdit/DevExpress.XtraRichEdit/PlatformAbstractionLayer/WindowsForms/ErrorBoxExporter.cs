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
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region WinFormsNotPrintableGraphicsBoxExporter
	public class WinFormsNotPrintableGraphicsBoxExporter : NotPrintableGraphicsBoxExporter {
		PageViewInfo currentPageInfo;
		RichEditView view;
		public WinFormsNotPrintableGraphicsBoxExporter(DocumentModel documentModel, Painter painter, RichEditView view, ICustomMarkExporter customMarkExporter)
			: base(documentModel, painter, view, customMarkExporter) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public RichEditView View { get { return view; } }
		protected internal PageViewInfo CurrentPageInfo { get { return currentPageInfo; } set { currentPageInfo = value; } }
		public virtual void ExportPage(PageViewInfo page) {
			this.currentPageInfo = page;
			ExportCurrentPage();
			this.currentPageInfo = null;
		}		
		protected internal virtual void ExportCurrentPage() {
			CurrentPageInfo.Page.ExportTo(this);
		}
		protected internal override Rectangle GetActualBounds(Rectangle bounds) {
			float zoomFactor = View.ZoomFactor;
			Rectangle result = new Rectangle((int)(bounds.X * zoomFactor), (int)(bounds.Y * zoomFactor), (int)(bounds.Width * zoomFactor), (int)(bounds.Height * zoomFactor));
			int dpi = Painter.DpiY;
			return UnitConverter.LayoutUnitsToPixels(result, dpi, dpi);
		}
		protected override Rectangle GetActualCustomMarkBounds(Rectangle bounds) {
			Rectangle result = View.CreatePhysicalRectangle(CurrentPageInfo, bounds);
			int dpi = Painter.DpiY;
			return UnitConverter.LayoutUnitsToPixels(result, dpi, dpi);
		}
		protected internal override float PixelsToDrawingUnits(float value) {
			return value;
		}
		protected internal override void ExportImeBoxes() {
			IImeService imeService = View.Control.GetService(typeof(IImeService)) as IImeService;
			if (imeService == null)
				return;
			ImeBoxExporter imeExporter = new ImeBoxExporter(this, imeService);
			imeExporter.Export(CurrentRow);
		}
	}
	#endregion
}
