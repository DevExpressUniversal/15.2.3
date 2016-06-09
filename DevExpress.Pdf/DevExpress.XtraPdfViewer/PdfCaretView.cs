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
using System.Security;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.XtraPdfViewer.Interop;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfCaretView : PdfDisposableObject {
		const double doublePi = Math.PI * 2;
		const double defaultCaretAngle = Math.PI / 2;
		readonly PdfViewer viewer;
		IntPtr bitmapHandle;
		public PdfCaretView(PdfViewer viewer) {
			this.viewer = viewer;
		}
		[SecuritySafeCritical]
		public void DrawCaret(PdfPage page, float scale, PointF location) {			
			PdfCaret caret = viewer.Caret;
			if (caret != null) {
				DeleteCaret();
				PdfCaretViewData viewData = caret.ViewData;
				int rotationAngle = viewer.RotationAngle;
				double angle = (PdfPageTreeNode.NormalizeRotate(rotationAngle + page.Rotate) * Math.PI / 180 - viewData.Angle) % doublePi;
				if (angle < 0)
					angle += doublePi;
				double angleDifference = defaultCaretAngle - angle;
				double visibleHeight = viewData.Height * scale;
				PdfPoint offset = page.ToUserSpace(viewData.TopLeft, scale, scale, rotationAngle);
				double xOffset = offset.X;
				double yOffset = offset.Y;
				using (Bitmap caretBitmap = new Bitmap((int)(Math.Abs(visibleHeight * Math.Cos(angleDifference))) + 1, (int)(Math.Abs(visibleHeight * Math.Sin(angleDifference))) + 1)) {
					using (Graphics graphics = Graphics.FromImage(caretBitmap)) {
						graphics.Clear(Color.Black);
						int bitmapWidth = caretBitmap.Width - 1;
						int bitmapHeight = caretBitmap.Height - 1;
						if (angle >= 0 && angle <= defaultCaretAngle) {
							graphics.DrawLine(Pens.White, 0, bitmapHeight, bitmapWidth, 0);
							xOffset -= bitmapWidth;
						}
						else if (angle > defaultCaretAngle && angle <= Math.PI) {
							graphics.DrawLine(Pens.White, 0, 0, bitmapWidth, bitmapHeight);
							xOffset -= bitmapWidth;
							yOffset -= bitmapHeight;
						}
						else if (angle > Math.PI && angle <= 3 * defaultCaretAngle) {
							graphics.DrawLine(Pens.White, 0, bitmapHeight, bitmapWidth, 0);
							yOffset -= bitmapHeight;
						}
						else
							graphics.DrawLine(Pens.White, 0, 0, bitmapWidth, bitmapHeight);
					}
					bitmapHandle = caretBitmap.GetHbitmap();
					CaretInterop.CreateCaret(viewer.Viewer.ViewControl.Handle, bitmapHandle, 0, 0);
					CaretInterop.SetCaretPos(Convert.ToInt32(location.X * scale + xOffset), Convert.ToInt32(location.Y * scale + yOffset));
				}
			}
		}
	   [SecuritySafeCritical]
	   public void DeleteCaret() {
			if (bitmapHandle != IntPtr.Zero) {
				CaretInterop.DeleteObject(bitmapHandle);
				bitmapHandle = IntPtr.Zero;
				CaretInterop.DestroyCaret();
			}
	   }
	   protected override void Dispose(bool disposing) {
		   DeleteCaret();
	   }
	}
}
