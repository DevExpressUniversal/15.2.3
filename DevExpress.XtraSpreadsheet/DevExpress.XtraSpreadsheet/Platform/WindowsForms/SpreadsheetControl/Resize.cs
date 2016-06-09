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
using System.Windows.Forms;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		#region Fields
		Rectangle lastValidClientRectangle; 
		Rectangle clientBounds; 
		Rectangle cornerBounds; 
		Rectangle backgroundBounds; 
		Rectangle viewBounds; 
		Rectangle rightHeaderBounds; 
		#endregion
		#region Properties
		protected internal Rectangle ViewBounds { get { return viewBounds; } }
		Rectangle ISpreadsheetControl.ViewBounds { get { return viewBounds; } }
		Rectangle ISpreadsheetControl.LayoutViewBounds { get { return viewBounds; } }
		protected internal Rectangle ClientBounds { get { return clientBounds; } }
		protected internal Rectangle BackgroundBounds { get { return backgroundBounds; } }
		protected internal Rectangle CornerBounds { get { return cornerBounds; } }
		protected internal Rectangle RightHeaderBounds { get { return rightHeaderBounds; } }
		#endregion
		protected override void OnResize(EventArgs e) {
			if (!IsHandleCreated)
				return;
			try {
				SpreadsheetControlPainter oldPainter = this.painter;
				this.painter = new EmptySpreadsheetControlPainter(this);
				try {
					base.OnResize(e);
					OnResizeCore();
				}
				finally {
					this.painter = oldPainter;
				}
			}
			finally {
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (VerticalScrollBar != null)
				VerticalScrollBar.OnAction(ScrollNotifyAction.Resize);
			if (HorizontalScrollBar != null)
				HorizontalScrollBar.OnAction(ScrollNotifyAction.Resize);
		}
		void IInnerSpreadsheetControlOwner.OnResizeCore() {
			this.OnResizeCore();
		}
		protected internal virtual void OnResizeCore() {
			Rectangle initialClientBounds = CalculateInitialClientBounds();
			for (; ; ) {
				UpdateVerticalScrollbarVisibility();
				UpdateHorizontalScrollbarAndTabSelectorVisibility();
				if (!PerformResize(initialClientBounds))
					break;
			}
		}
		protected internal virtual Rectangle CalculateInitialClientBounds() {
			Form form = FindForm();
			if (form != null) {
				if (form.WindowState != FormWindowState.Minimized)
					this.lastValidClientRectangle = this.ClientRectangle;
			}
			else
				this.lastValidClientRectangle = this.ClientRectangle;
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero))) {
				BorderPainter borderPainter = BorderHelper.GetGridPainter(this.BorderStyle, LookAndFeel);
				BorderObjectInfoArgs args = new BorderObjectInfoArgs(cache, lastValidClientRectangle, null, ObjectState.Normal);
				return borderPainter.GetObjectClientRectangle(args);
			}
		}
		protected internal virtual bool PerformResize(Rectangle initialClientBounds) {
			this.clientBounds = initialClientBounds;
			int verticalScrollbarWidth = CalculateVerticalScrollbarWidth();
			int horizontalSplitContainerHeight = CalculateHorizontalSplitContainerHeight();
			if (!VerticalScrollBar.IsOverlapScrollBar)
				this.clientBounds.Width -= verticalScrollbarWidth;
			this.clientBounds.Height -= horizontalSplitContainerHeight;
			int clientRight = clientBounds.Right;
			int clientTop = clientBounds.Top;
			int clientBottom = clientBounds.Bottom;
			int headerHeight = CalculateHeaderHeightInPixels();
			int groupoffset = CalculateTopHeaderOffsetInPixels();
			if (CalculateVerticalScrollbarVisibility())
				verticalScrollbar.Bounds = new Rectangle(clientRight + (VerticalScrollBar.IsOverlapScrollBar ? -verticalScrollbarWidth : 0), clientTop + headerHeight + groupoffset, verticalScrollbarWidth, clientBounds.Height - headerHeight - groupoffset);
			if (CalculateHorizontalScrollbarVisibility() && IsTouchModeHorizontalScrollBar) {
				int horizontalScrollbarHeight = CalculateHorizontalScrollbarHeight();
				horizontalScrollbar.Bounds = new Rectangle(clientBounds.X, clientBounds.Bottom - horizontalScrollbarHeight, clientBounds.Width - verticalScrollbarWidth, horizontalScrollbarHeight);
			}
			Rectangle horizontalBounds = new Rectangle(clientBounds.Left, clientBottom, clientBounds.Width, horizontalSplitContainerHeight);
			this.horizontalSplitContainer.Bounds = horizontalBounds;
			this.cornerBounds = new Rectangle(clientRight, clientBottom, verticalScrollbarWidth, horizontalBounds.Height);
			this.rightHeaderBounds = new Rectangle(clientRight, clientTop + groupoffset, verticalScrollbarWidth, headerHeight);
			this.backgroundBounds = clientBounds;
			if (!IsHandleCreated)
				CreateHandle();
			ResizeView();
			return false; 
		}
		protected internal virtual int CalculateHeaderHeightInPixels() {
			float height = CalculateHeaderHeightInPixelsF();
			return (int)Math.Round(Math.Max(0, height) * ActiveView.ZoomFactor);
		}
		protected internal virtual int CalculateTopHeaderOffsetInPixels() {
			float height = CalculateTopHeaderOffsetInPixelsF();
			return (int)Math.Round(Math.Max(0, height) * ActiveView.ZoomFactor);
		}
		protected internal virtual float CalculateHeaderHeightInPixelsF() {
			Worksheet activeSheet = DocumentModel.ActiveSheet;
			if (!activeSheet.ShowColumnHeaders)
				return 0;
			if (DocumentModel.ViewOptions.ColumnHeaderHeight != 0)
				return DocumentModel.ViewOptions.ColumnHeaderHeight;
			IColumnWidthCalculationService service = GetService<IColumnWidthCalculationService>();
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(service.CalculateHeaderHeight(activeSheet), DocumentModel.DpiY);
		}
		protected internal virtual float CalculateTopHeaderOffsetInPixelsF() {
			return GroupItemsPage.CalculateGroupHeightInPixels(DocumentModel.ActiveSheet);
		}
		void IInnerSpreadsheetControlOwner.ResizeView() {
			this.ResizeView();
		}
		protected internal virtual void ResizeView() {
			this.viewBounds = CalculateViewBounds(clientBounds);
			Rectangle normalizedViewBounds = viewBounds;
			normalizedViewBounds.X = 0;
			normalizedViewBounds.Y = 0;
			ActiveView.OnResize(normalizedViewBounds);
		}
		protected internal virtual Rectangle CalculateViewBounds(Rectangle clientBounds) {
			Rectangle viewPixelBounds = CalculateViewPixelBounds(clientBounds);
			return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(viewPixelBounds, DpiX, DpiY);
		}
		protected internal virtual Rectangle CalculateViewPixelBounds(Rectangle clientBounds) {
			return clientBounds;
		}
	}
}
