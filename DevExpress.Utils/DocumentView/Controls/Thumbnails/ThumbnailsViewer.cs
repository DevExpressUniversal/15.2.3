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

using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.DocumentView.Controls.Thumbnails {
	class ThumbnailsViewManager : ContinuousViewManager {
		public ThumbnailsViewManager(DocumentViewerBase pc, ViewControl viewControl)
			: base(pc, viewControl) {
		}
		void SetVertContinuousScroll(float val) {
			if(Pages.Count < 1) return;
			SetScrollPosY(val);
			SetScrollPosY(val);
			pc.UpdateScrollBars();
		}
		protected override void FillPagePlace(int columns, int rows, float zoom) {
			FillPagePlaceCore(columns, rows, zoom, PSUnitConverter.PixelToDoc(25) * 1f / zoom);
		}
		public override void OffsetVertScroll(float dy, bool canChangePage) {
			if(fScrollPos.Y < MaxScrollY || fScrollPos.Y > 0) {
				SetScrollPosY(fScrollPos.Y + dy);
				SetVertContinuousScroll(fScrollPos.Y);
			} else
				base.OffsetVertScroll(dy, canChangePage);
		}
		public override void SetVertScroll(float val) {
			SetVertContinuousScroll(val);
		}
	}
	class ThumbnailsScrollController : ScrollController {
		public ThumbnailsScrollController(DocumentViewerBase printControl, DevExpress.XtraEditors.HScrollBar hScrollBar, DevExpress.XtraEditors.VScrollBar vScrollBar)
			: base(printControl, hScrollBar, vScrollBar) {
		}
		protected override void ShowScrollInfo(float scrollVal) { }
	}
	public class ThumbnailsViewer : DocumentViewerBase {
		public ThumbnailsViewer() {
			Zoom = 0.1f;
			ShowPageMargins = false;
		}
		protected override ScrollController CreateScrollController(DevExpress.XtraEditors.HScrollBar hScrollBar, DevExpress.XtraEditors.VScrollBar vScrollBar) {
			return new ThumbnailsScrollController(this, hScrollBar, vScrollBar);
		}
		protected override ViewManager CreateViewManager(ViewControl viewControl) {
			return new ThumbnailsViewManager(this, viewControl);
		}
		protected override void DrawPage(IPage page, Graphics graph, PointF position) {
			page.Draw(graph, PSUnitConverter.DocToPixel(position));
			DrawPageNumber(graph, page, position, BackgroundFont, BackgroundForeColor);
		}
		void DrawPageNumber(Graphics graph, IPage page, PointF pagePosition, Font font, Color foreColor) {
			Point pos = Point.Round(PSUnitConverter.DocToPixel(pagePosition, Zoom));
			Size pageSize = Size.Round(PSUnitConverter.DocToPixel(page.PageSize, Zoom));
			TextRenderer.DrawText(graph, (page.Index + 1).ToString(), font,
				new Rectangle(pos.X, pos.Y + pageSize.Height + Edges.Bottom, pageSize.Width, (int)font.GetHeight() + 2),
				foreColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
		}
		protected override void BindPrintingSystem() {
		}
		protected override void UnbindPrintingSystem() {
		}
	}
}
