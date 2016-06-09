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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfViewerClickController : PdfDisposableObject {
		int clickCount = 0;
		Timer clickTimer = new Timer();
		Point lastClickPoint;
		MouseButtons lastButton;
		public int ClickCount { get { return clickCount; } }
		void OnClickTimerTick(object sender, EventArgs e) {
			clickCount = 0;
			clickTimer.Stop();
		}
		public PdfViewerClickController() {
			clickTimer.Interval = SystemInformation.DoubleClickTime;
			clickTimer.Tick += OnClickTimerTick;
		}
		public void Click(MouseEventArgs e) {
			clickTimer.Stop();
			clickTimer.Interval = SystemInformation.DoubleClickTime;
			clickTimer.Start();
			clickCount = e.Button != lastButton ? 1 : clickCount + 1;
			Size doubleClickSize = SystemInformation.DoubleClickSize;
			if (Math.Abs(lastClickPoint.X - e.X) > doubleClickSize.Width || Math.Abs(lastClickPoint.Y - e.Y) > doubleClickSize.Height || clickCount == 5)
				clickCount = 1;
			lastClickPoint = e.Location;
			lastButton = e.Button;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && clickTimer != null) {
				clickTimer.Tick -= OnClickTimerTick;
				clickTimer.Dispose();
				clickTimer = null;
			}
		}
	}
}
