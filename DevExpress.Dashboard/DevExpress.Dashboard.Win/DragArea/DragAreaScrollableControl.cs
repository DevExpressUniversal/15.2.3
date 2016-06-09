#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DragAreaScrollableControl : XtraScrollableControl {
		enum AutoScrollMode { None, Left, Right, Top, Bottom }
		const int scrollArea = 30;
		readonly Timer scrollTimer = new Timer();
		readonly DragAreaControl dragAreaControl;
		AutoScrollMode autoScrollMode = AutoScrollMode.None;
		int lastX;
		int lastY;
		protected override bool IsOverlapVScrollBar {
			get {
				return false;
			}
		}
		public DragAreaControl DragArea { get { return dragAreaControl; } }
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
#endif
		public DragAreaScrollableControl(DashboardDesigner designer) {
			dragAreaControl = new DragAreaControl(designer, this);
			Dock = DockStyle.Fill;
			DashboardWinHelper.SetParentLookAndFeel(dragAreaControl, LookAndFeel);
			Controls.Add(dragAreaControl);
			scrollTimer.Interval = 15;
			scrollTimer.Tick += new EventHandler(OnTimer);
			AlwaysScrollActiveControlIntoView = false;
			AllowTouchScroll = true;
			InvertTouchScroll = true;
		}
		public void StartAutoScroll(Point point) {
			point = PointToClient(point);
			Rectangle rect = ClientRectangle;
			if (rect.Contains(point)) {
				int x = point.X;
				int y = point.Y;
				switch (autoScrollMode) {
					case AutoScrollMode.Left:
						if (x > lastX)
							DisableAutoScroll();
						break;
					case AutoScrollMode.Right:
						if (x < lastX)
							DisableAutoScroll();
						break;
					case AutoScrollMode.Top:
						if (y > lastY)
							DisableAutoScroll();
						break;
					case AutoScrollMode.Bottom:
						if (y < lastY)
							DisableAutoScroll();
						break;
					default:
						int horizontalDiff = Math.Min(x - rect.Left, rect.Right - x);
						int verticalDiff = Math.Min(y - rect.Top, rect.Bottom - y);
						if (horizontalDiff < verticalDiff)
							TurnHorizontalAutoScroll(x);
						else if (horizontalDiff > verticalDiff)
							TurnVerticalAutoScroll(y);
						else if (rect.Width < rect.Height)
							TurnHorizontalAutoScroll(x);
						else if (rect.Height > rect.Width)
							TurnVerticalAutoScroll(y);
						else
							DisableAutoScroll();
						break;
				}
				lastX = x;
				lastY = y;
			}
			else
				DisableAutoScroll();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (scrollTimer != null) {
					scrollTimer.Stop();
					scrollTimer.Dispose();
				}
			}
			base.Dispose(disposing);			
		}
		void DisableAutoScroll() {
			scrollTimer.Enabled = false;
			autoScrollMode = AutoScrollMode.None;
		}
		void TurnHorizontalAutoScroll(int x) {
			Rectangle rect = ClientRectangle;
			int leftDiff = x - rect.Left;
			int rightDiff = rect.Right - x;
			if (leftDiff < rightDiff)
				if (leftDiff <= scrollArea && rect.Width / 3 > leftDiff) {
					autoScrollMode = AutoScrollMode.Left;
					scrollTimer.Enabled = true;
				}
				else
					DisableAutoScroll();
			else if (leftDiff > rightDiff)
				if (rightDiff <= scrollArea && rect.Width / 3 > rightDiff) {
					autoScrollMode = AutoScrollMode.Right;
					scrollTimer.Enabled = true;
				}
				else
					DisableAutoScroll();
			else
				DisableAutoScroll();
		}
		void TurnVerticalAutoScroll(int y) {
			Rectangle rect = ClientRectangle;
			int topDiff = y - rect.Top;
			int bottomDiff = rect.Bottom - y;
			if (topDiff < bottomDiff)
				if (topDiff <= scrollArea && rect.Height / 3 > topDiff) {
					autoScrollMode = AutoScrollMode.Top;
					scrollTimer.Enabled = true;
				}
				else
					DisableAutoScroll();
			else if (topDiff > bottomDiff)
				if (bottomDiff <= scrollArea && rect.Height / 3 > bottomDiff) {
					autoScrollMode = AutoScrollMode.Bottom;
					scrollTimer.Enabled = true;
				}
				else
					DisableAutoScroll();
			else
				DisableAutoScroll();
		}
		void OnTimer(object sender, EventArgs e) {
			Point position = Control.MousePosition;
			StartAutoScroll(position);
			if (!dragAreaControl.DragManager.DragWindow.IsDragging)
				DisableAutoScroll();
			else {
				dragAreaControl.DragManager.UpdateDropAction();
				Point currentPosition = AutoScrollPosition;
				switch (autoScrollMode) {
					case AutoScrollMode.Left:
						AutoScrollPosition = new Point(-currentPosition.X - 1, -currentPosition.Y);
						break;
					case AutoScrollMode.Right:
						AutoScrollPosition = new Point(-currentPosition.X + 1, -currentPosition.Y);
						break;
					case AutoScrollMode.Top:
						AutoScrollPosition = new Point(-currentPosition.X, -currentPosition.Y - 1);
						break;
					case AutoScrollMode.Bottom:
						AutoScrollPosition = new Point(-currentPosition.X, -currentPosition.Y + 1);
						break;
					default:
						DisableAutoScroll();
						break;
				}
			}
		}
		public void DoMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected override void AdjustFormScrollbars(bool displayScrollbars) {
			if (dragAreaControl != null) {
				bool dragAreaIsEmpty = dragAreaControl.Area.Sections.Count == 0;
				HorizontalScroll.Visible = !dragAreaIsEmpty && ClientSize.Width < dragAreaControl.DrawingContext.SectionMinWidth;
				VerticalScroll.Visible = !dragAreaIsEmpty;
			}
			base.AdjustFormScrollbars(displayScrollbars);
		}
	}
}
