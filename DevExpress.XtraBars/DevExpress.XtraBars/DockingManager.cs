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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Forms;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Helpers.Docking {
	public class DockingManager : CustomTopForm {
		private BarManager manager;
		private IDockableObject dockObject;
		private Point downPoint, startPoint, deltaPoint;
		public event EventHandler EndDock;
		bool stopped;
		CanDockTestInfo lastHitInfo;
		FrameInfo reversibleFrame = new FrameInfo();
		private ArrayList stopDispose;
		private Control mouseControl;
		public DockingManager(BarManager AManager) {
			stopped = false;
			stopDispose = new ArrayList();
			Text = "DockingManager";
			dockObject = null;
			lastHitInfo = null;
			manager = AManager;
			Size = new Size(0, 0);
			mouseControl = null;
			StartPosition = FormStartPosition.Manual;
			Location = BarManager.zeroPoint;
			startPoint = downPoint = Point.Empty;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		internal void SetCanDispose(bool value) {
			foreach(Control mouseControl in stopDispose) {
				if(mouseControl is ControlForm) 
					(mouseControl as ControlForm).CanDispose = value;
				if(mouseControl is CustomControl)
					(mouseControl as CustomControl).CanDisposeCore = value;
			}
		}
		bool fireChanged = false;
		protected internal void StartMoving(IDockableObject dockObject, Control mouseDown) {
			Focus();
			this.fireChanged = false;
			this.dockObject = dockObject;
			stopDispose.Clear();
			stopDispose.Add(mouseDown);
			if(mouseDown is FloatingBarControl) {
				stopDispose.Add((mouseDown as FloatingBarControl).Form);
			}
			mouseControl = mouseDown;
			SetCanDispose(false);
			DockObject.FloatMousePosition = Point.Empty;
			Capture = true;
			this.deltaPoint = Point.Empty;
			startPoint = downPoint = Control.MousePosition;
			Point startObjectPoint;
			if(DockObject.DockControl != null && DockObject.BarControl != null) {
				var sp = DockObject.BarControl.Location;
				if(DockObject.DockControl.IsRightToLeft) { 
					sp.X += DockObject.BarControl.Width;
				}
				startObjectPoint = DockObject.DockControl.PointToScreen(sp);
				deltaPoint = new Point(startPoint.X - startObjectPoint.X, startPoint.Y - startObjectPoint.Y);
			} else {
				startObjectPoint = DockObject.FloatLocation;
				deltaPoint = new Point(startPoint.X - startObjectPoint.X, startPoint.Y - startObjectPoint.Y);
			}
			DockObject.IsDragging = true;
			Cursor.Current = Cursors.SizeAll;
		}
		public virtual bool IsDragging {
			get { return !stopped && !Disposing; }
		}
		protected internal void StopMoving(bool cancel) {
			this.lastHitInfo = null;
			StopMoving();
		}
		protected internal void StopMoving() {
			if(stopped) return;
			stopped = true;
			reversibleFrame.Hide();
			if(mouseControl != null) {
				if(mouseControl is CustomFloatingForm) {
					(mouseControl as CustomFloatingForm).IsFormDragging = false;
				}
				SetCanDispose(true);
				if(mouseControl is CustomControl) {
					if(DockObject.BarControl == mouseControl) mouseControl = null;
				}
				if(DockObject.BarControl is FloatingBarControl) {
					if((DockObject.BarControl as FloatingBarControl).Form == mouseControl)
						mouseControl = null;
				}
				if(mouseControl != null) 
					mouseControl.Dispose();
				mouseControl = null;
			}
			OnStopDocking();
			DockObject.IsDragging = false;
			Cursor.Current = Cursors.Default;
			Capture = false;
			if(this.fireChanged)
				Manager.FireManagerChanged();
		}
		protected BarManager Manager { get { return manager; } } 
		protected IDockableObject DockObject { get { return dockObject; } }
		protected CustomControl BarControl { 
			get { return DockObject.BarControl; } 
		}
		protected virtual Size GetBarSize() {
			if(DockObject.BarControl is FloatingBarControl) return (DockObject.BarControl as FloatingBarControl).Form.Size;
			if(DockObject.BarControl != null) return DockObject.BarControl.Size;
			return Size.Empty;
		}
		protected virtual Point GetBarLocation() {
			if(DockObject.BarControl is FloatingBarControl) {
				return (DockObject.BarControl as FloatingBarControl).Form.Location;
			}
			if(DockObject.BarControl != null) {
				Point p = DockObject.BarControl.Location;
				return DockObject.BarControl.PointToScreen(p);
			}
			return Point.Empty;
		}
		protected virtual bool CanMoveBarTo(Point point) {
			if(DockObject.BarControl is FloatingBarControl) {
				return true;
			}
			if(!Manager.AllowMoveBarOnToolbar || DockObject.UseWholeRow) return false;
				return true;
			}
		protected virtual void SetBarLocation(Point p) {
			this.fireChanged = true;
			if(DockObject.BarControl is FloatingBarControl) {
				(DockObject.BarControl as FloatingBarControl).Form.Location = p;
			} 
			if(DockObject.DockControl != null) {
				BarDockControl dock = DockObject.DockControl;
				Point c = dock.PointToClient(p);
				if(!dock.IsVertical && Manager.IsRightToLeft) {
					c.X = dock.ClientRectangle.Right - c.X;
				}
				int newOffset = (dock.IsVertical ? c.Y : c.X);
				if(newOffset != DockObject.DockInfo.Offset) {
					DockObject.DockInfo.Offset = newOffset;
					dock.UpdateDockSize();
					dock.Update();
				}
			}
		}
		protected Point CalcMovePosition(Point downPoint) {
			Point p = Cursor.Position;
			Size size = GetBarSize();
			Point deltaPoint = new Point(UpdateValue(this.deltaPoint.X, size.Width), UpdateValue(this.deltaPoint.Y, size.Height));
			if(DockObject.DockControl != null) {
				p.Offset(-deltaPoint.X, -deltaPoint.Y);
				return p;
			}
			p.Offset(-deltaPoint.X, -deltaPoint.Y);
			return p;
		}
		int UpdateValue(int point, int maxRange) {
			int sign = point < 0 ? -1 : 1;
			if(point == 0 || maxRange == 0) return 0;
			point = Math.Abs(point);
			if(point > maxRange) return maxRange * sign;
			return point * sign;
		}
		protected Point CalcWindowOffset(Point p) {
			return p; 
		}
		protected virtual void MoveDragRectangleTo(Point p, Size size) {
			Rectangle r = new Rectangle(p, size);
			r.Location = CalcWindowOffset(p);
			reversibleFrame.Draw(r);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(downPoint == Cursor.Position) {
				Application.DoEvents();
				return;
			}
			Point p = CalcMovePosition(downPoint);
			downPoint = Cursor.Position;
			if(CanMoveBarTo(p)) {
				SetBarLocation(p);
			}
			CanDockTestInfo hi = CheckCanDock(downPoint);
			while(true) {
				if(!hi.IsEquals(DockObject)) {
					if((Control.ModifierKeys & Keys.Control) != 0) {
						if(DockObject.DockInfo.DockStyle == BarDockStyle.None) break;
					}
					if(DockObject.DockInfo.DockStyle != BarDockStyle.None && hi.DockControl == null) {
						Rectangle r = CalcDockingRectangle(DockObject.DockControl);
						if(DockObject.DockControl.IsVertical) 
							r.Inflate(0, 20);
						else
							r.Inflate(20, 0);
						if(r.Contains(downPoint)) {
							hi = null;
							break;
						}
					}
					MakeDocking(hi);
				}
				break;
			}
			lastHitInfo = hi;
			Application.DoEvents();
		}
		protected void MakeDocking(CanDockTestInfo hi) {
			if(hi.IsEquals(DockObject) && hi.DockControl != null) return;
			this.fireChanged = true;
			bool containsFocus = DockObject.BarControl != null && DockObject.BarControl.ContainsFocus;
			if(hi.DockControl == null) {
				Point bl = downPoint;
				bl.Offset(-DockObject.FloatMousePosition.X, -DockObject.FloatMousePosition.Y);
				DockObject.FloatLocation = bl;
				DockObject.DockStyle = BarDockStyle.None;
			} else {
				if(DockObject.DockStyle == BarDockStyle.None) {
					Point temp = downPoint;
					if(DockObject.BarControl != null) {
						temp.Offset( -DockObject.FloatLocation.X, -DockObject.FloatLocation.Y);
					} else
						temp = Point.Empty;
					DockObject.FloatMousePosition = temp;
				}
				if(DockObject.CanDock(hi.DockControl)) {
					if(DockObject.DockStyle == BarDockStyle.Standalone && hi.DockControl.GetDockableOnRowCount(DockObject) == 1 && hi.DockRow == DockObject.DockInfo.DockRow + 1) {
							hi.DockRow--;
							DockObject.DockInfo.UpdateDockPosition(DockObject.DockInfo.DockRow, -1);
					}
					DockObject.DockInfo = new BarDockInfo(hi.DockControl, hi.DockControl.DockStyle, hi.DockRow, hi.DockCol, DockObject.DockInfo.Offset);
				}
			}
			if(DockObject.BarControl == null) 
				DockObject.VisibleChanged();
			if(containsFocus) FocusDockObject(DockObject);
		}
		void FocusDockObject(IDockableObject dockObject) {
			if(dockObject == null || dockObject.BarControl == null) return;
			dockObject.BarControl.Focus();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) {
				StopMoving();
			}
			base.OnKeyDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			StopMoving();
		}
		protected virtual void OnStopDocking() {
			Bar bar = DockObject as Bar;
			if(bar != null && fireChanged) bar.RaiseDockChanged();
			if(EndDock != null)
				EndDock(this, EventArgs.Empty);
		}
		private void CalcDockRowIndex2(CanDockTestInfo hi, DockRow hitRow,  Rectangle rowRect, Point p) {
			int delta = 5; 
			bool top = (p.Y - rowRect.Y) < delta;
			bool bottom = (rowRect.Bottom - p.Y) < delta;
			bool left = (p.X - rowRect.X) < delta;
			bool right = (rowRect.Right - p.X) < delta;
			DockRow currentRow = hi.DockControl.Rows.RowByDockable(DockObject);
			int currentIndex = -1;
			if(currentRow != null) 
				currentIndex = hi.DockControl.Rows.IndexOf(currentRow);
			switch(hi.DockControl.DockStyle) {
				case BarDockStyle.Left :
					top = left;
					bottom = right;
					break;
				case BarDockStyle.Right :
					top = right;
					bottom = left;
					break;
				case BarDockStyle.Bottom :
					bool temp = top;
					top = bottom;
					bottom = temp;
					break;
			}
			switch(hi.DockControl.DockStyle) {
				case BarDockStyle.Left :
				case BarDockStyle.Right :
				case BarDockStyle.Bottom :
				case BarDockStyle.Top :
					if(currentIndex != -1) {
						if(currentIndex > hi.DockRow) {
							if(bottom && (currentIndex - hi.DockRow) == 1) {
								hi.DockCol = DockObject.DockInfo.DockCol;
								hi.DockRow = DockObject.DockInfo.DockRow;
								return;
							}
						} 
						if(currentIndex == hi.DockRow) {
							if(top && currentRow.Count > 1) {
								hi.DockCol = -1;
								if(currentIndex > hi.DockRow) hi.DockRow ++;
								break;
							}
						}
						if(currentIndex < hi.DockRow) {
							if(currentRow.Count == 1 && currentRow.CanAddDockable(DockObject)) {
								hi.DockRow --;
								hi.NotEqual = true;
								break;
							} else {
								hi.DockCol = -1;
							}
							if(top && (hi.DockRow - currentIndex) == 1) {
								hi.DockCol = DockObject.DockInfo.DockCol;
								hi.DockRow = DockObject.DockInfo.DockRow;
								return;
							}
						}
					}
					if(currentIndex == hi.DockRow) {
						hi.DockCol = DockObject.DockInfo.DockCol;
						hi.DockRow = DockObject.DockInfo.DockRow;
						return;
					} 
					DockRow newRow = null;
					if(hi.DockRow > -1 && hi.DockRow < hi.DockControl.Rows.Count)
						newRow = hi.DockControl.Rows[hi.DockRow];
					if(newRow != null) {
						if(!newRow.CanAddDockable(DockObject)) {
							if(top || bottom) hi.DockCol = -1;
							else {
								if(DockObject.DockControl == hi.DockControl) {
									if(Math.Abs(DockObject.DockInfo.DockRow - hi.DockRow) == 1) hi.DockRow = DockObject.DockInfo.DockRow;
								} else {
									if(hi.DockCol != -1) hi.DockCol = -1;
								}
							}
						}
					}
					break;
			}
		}
		private void CalcDockHitTest(CanDockTestInfo hi, Rectangle dockingRect, Point p) {
			if(hi.DockControl == null) return;
			p = hi.DockControl.PointToClient(p);
			DockRow row = null;
			int rowIndex;
			Rectangle rowRect = Rectangle.Empty;
			for(rowIndex = 0; rowIndex < hi.DockControl.Rows.Count; rowIndex++) {
				row = hi.DockControl.Rows[rowIndex];
				rowRect = row.RowRect;
				rowRect.Inflate(1, 1);
				if(rowRect.Contains(p)) {
					break;
				}
				row = null;
			}
			hi.DockRow = -1;
			hi.DockCol = -1;
			if(hi.DockControl.Rows.Count == 0) return;
			if(row == null) {
				bool left = (dockingRect.Width / 2) > (p.X - dockingRect.X);
				bool top = (dockingRect.Height / 2) > (p.Y - dockingRect.Y);
				switch(hi.DockControl.DockStyle) {
					case BarDockStyle.Standalone:
						hi.DockRow = CalcDockRowForStandaloneBarDockControl(hi, left, top);
						break;
					case BarDockStyle.Left : 
						if(left) hi.DockRow = 0;
						break;
					case BarDockStyle.Top : 
						if(top) hi.DockRow = 0;
						break;
					case BarDockStyle.Right : 
						if(!left) hi.DockRow = 0;
						break;
					case BarDockStyle.Bottom : 
						if(!top) hi.DockRow = 0;
						break;
				}
				return;
			}
			hi.DockRow = rowIndex;
			hi.DockCol = 0;
			CalcDockRowIndex(hi, row, rowRect, p);
			if(hi.DockRow > -1 && hi.DockRow < hi.DockControl.Rows.Count)
				row = hi.DockControl.Rows[hi.DockRow];
			CalcDockColIndex(hi, row, rowRect, p);
		}
		protected virtual int CalcDockRowForStandaloneBarDockControl(CanDockTestInfo hi, bool left, bool top) {
			if(hi.DockControl.Dock == DockStyle.Top || hi.DockControl.Dock == DockStyle.Bottom)
				return top ? 0 : hi.DockControl.Rows.Count - 1;
			if(hi.DockControl.Dock == DockStyle.Left || hi.DockControl.Dock == DockStyle.Right)
				return left ? 0 : hi.DockControl.Rows.Count - 1;
			return -1;
		}
		private void CalcDockRowIndex(CanDockTestInfo hi, DockRow hitRow,  Rectangle rowRect, Point p) {
			int delta = 5;
			bool top = (p.Y - rowRect.Y) < delta;
			bool bottom = (rowRect.Bottom - p.Y) < delta;
			bool left = (p.X - rowRect.X) < delta;
			bool right = (rowRect.Right - p.X) < delta;
			DockRow currentRow = hi.DockControl.Rows.RowByDockable(DockObject);
			int currentIndex = -1;
			if(currentRow != null) 
				currentIndex = hi.DockControl.Rows.IndexOf(currentRow);
			switch(hi.DockControl.DockStyle) {
				case BarDockStyle.Left :
					top = left;
					bottom = right;
					break;
				case BarDockStyle.Right :
					top = right;
					bottom = left;
					break;
				case BarDockStyle.Bottom :
					bool temp = top;
					top = bottom;
					bottom = temp;
					break;
			}
			if(currentIndex == -1) return;
			if(currentIndex == hi.DockRow) {
				if(top && currentRow.Count > 1) {
					hi.DockCol = -1;
					return;
				}
			}
			if(currentIndex > hi.DockRow) {
				if(bottom && (currentIndex - hi.DockRow) == 1) { 
					hi.DockCol = DockObject.DockInfo.DockCol;
					hi.DockRow = DockObject.DockInfo.DockRow;
					return;
				}
			}
			if(currentIndex < hi.DockRow) {
				if(top && (hi.DockRow - currentIndex) == 1) { 
					hi.DockCol = DockObject.DockInfo.DockCol;
					hi.DockRow = DockObject.DockInfo.DockRow;
					return;
				}
			}
			DockRow newRow = null;
			if(hi.DockRow > -1 && hi.DockRow < hi.DockControl.Rows.Count)
				newRow = hi.DockControl.Rows[hi.DockRow];
			if(newRow == null) return;
			hitRow = newRow;
			if((!newRow.Contains(DockObject) && !newRow.CanAddDockable(DockObject)) || DockObject.UseWholeRow) {
				hi.DockCol = -1;
				if(top || bottom) {
					hi.DockCol = -1;
					return;
				}
			}
		}
		private void CalcDockColIndex(CanDockTestInfo hi, DockRow hitRow,  Rectangle rowRect, Point p) {
			if(hi.DockCol == -1) return;
			DockRow currentRow = hi.DockControl.Rows.RowByDockable(DockObject);
			int currentIndex = -1;
			if(currentRow != null) 
				currentIndex = currentRow.IndexOf(DockObject);
			hi.DockCol = 0;
			if(currentRow != hitRow && !hitRow.CanAddDockable(DockObject)) { 
				hi.DockCol = -1;
				return;
			}
			bool isRightToLeft = hi.DockControl.IsRightToLeft;
			for(int n = 0; n < hitRow.Count; n++) {
				if(DockObject == hitRow[n].Dockable) continue;
				Rectangle rect = hitRow[n].Bounds;
				int start, end, d, delta;
				if(hi.DockControl.IsVertical) {
					start = rect.Top;
					end = rect.Bottom;
					d = p.Y;
				} else {
					start = rect.Left;
					end = rect.Right;
					d = p.X;
				}
				delta = Math.Abs(( end - start) / 4); 
				if(isRightToLeft) {
					CheckColRTL(hi, hitRow, currentRow, currentIndex, n, start, end, d, delta);
				}
				else {
					CheckCol(hi, hitRow, currentRow, currentIndex, n, start, end, d, delta);
				}
			}
		}
		void CheckCol(CanDockTestInfo hi, DockRow hitRow, DockRow currentRow, int currentIndex, int n, int start, int end, int d, int delta) {
			if(currentRow != hitRow) {
				if(d > end - delta) {
					hi.DockCol = n + 1;
				}
			}
			else {
				if(d > end - delta) {
					hi.DockCol = n + 1;
					if(currentIndex < n) {
						hi.DockCol += -1;
						hi.NotEqual = true;
					}
				}
				else {
					if(currentIndex > n) {
						if(d < start + delta)
							hi.DockCol = n;
						else
							hi.DockCol = n + 1;
					}
				}
			}
		}
		void CheckColRTL(CanDockTestInfo hi, DockRow hitRow, DockRow currentRow, int currentIndex, int n, int start, int end, int d, int delta) {
			if(currentRow != hitRow) {
				if(d < start + delta) {
					hi.DockCol = n + 1;
				}
			}
			else {
				if(d < start + delta) {
					hi.DockCol = n + 1;
					if(currentIndex < n) {
						hi.DockCol += -1;
						hi.NotEqual = true;
					}
				}
				else {
					if(currentIndex > n) {
						if(d > end - delta)
							hi.DockCol = n;
						else
							hi.DockCol = n + 1;
					}
				}
			}
		}
		protected Rectangle CalcDockingRectangle(BarDockControl dk) {
			return dk.CalcDockingRectangle();
		}
		internal CanDockTestInfo CheckCanDock(Point p) {
			CanDockTestInfo hi = new CanDockTestInfo();
			if((Control.ModifierKeys & Keys.Control) != 0) return hi;
			foreach(BarDockControl dk in Manager.DockControls) {
				Rectangle dockingRect = CalcDockingRectangle(dk);
				if(dockingRect.Contains(p)) {
					if(!DockObject.CanDock(dk)) continue;
					hi.DockControl = dk;
					dockingRect.Location = dk.PointToClient(dockingRect.Location);
					CalcDockHitTest(hi, dockingRect, p);
					break;
				}
			}
			if(hi.DockControl != null) {
				hi.DockControl.CalcMinMaxDockableRow(DockObject, ref hi.DockRow, ref hi.DockCol);
			}
			return hi;
		}
	}
	public class FrameInfo {
		const int tabWidth = 50, tabHeight = 20, lineSize = 2;
		Rectangle bounds;
		bool visible;
		bool drawDownTab, drawUpTab;
		public FrameInfo() {
			drawDownTab = drawUpTab = false;
			visible = false;
			bounds = Rectangle.Empty;
		}
		public virtual Rectangle Bounds {
			get { return bounds; }
			set {
				if(Bounds == value) return;
				Draw(value);
			}
		}
		public virtual bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				if(value) Draw();
				else Hide();
			}
		}
		public virtual bool DrawDownTab {
			get { return drawDownTab; }
			set { 
				if(DrawDownTab == value) return;
				bool prevVisible = Visible;
				if(prevVisible) Hide();
				drawDownTab = value; 
				if(prevVisible) Draw();
			}
		}
		public virtual bool DrawUpTab {
			get { return drawUpTab; }
			set { 
				if(DrawUpTab == value) return;
				bool prevVisible = Visible;
				if(prevVisible) Hide();
				drawUpTab = value; 
				if(prevVisible) Draw();
			}
		}
		public void Draw() {
			Draw(Rectangle.Empty);
		}
		public void Draw(Rectangle newBounds) {
			if(newBounds.IsEmpty) newBounds = Bounds;
			if(Visible && newBounds.Equals(Bounds)) return;
			if(newBounds.IsEmpty) return;
			Hide();
			bounds = newBounds;
			InternalDraw();
			visible = true;
		}
		public void Hide() {
			if(Visible) {
				InternalDraw();
			}
			visible = false;
		}
		protected void InternalDraw() {
			Rectangle real = Bounds, r;
			int tHeight = tabHeight, tWidth = tabWidth;
			if(real.Height < tHeight * 2) tHeight = real.Height / 3;
			if(real.Width < tabWidth + 20) tWidth = real.Width - 10;
			Rectangle upTab, downTab;
			upTab = downTab = Rectangle.Empty;
			upTab.Height = downTab.Height = tHeight;
			upTab.Width = downTab.Width = tWidth;
			if(DrawUpTab && tHeight > 0) {
				real.Y += tHeight;
				real.Height -= tHeight;
			} else 
				upTab = Rectangle.Empty;
			if(DrawDownTab && tHeight > 0) {
				real.Height -= tHeight;
			} else 
				downTab = Rectangle.Empty;
			upTab.X = downTab.X = real.X + tWidth / 4;
			upTab.Y = Bounds.Y;
			downTab.Y = real.Bottom;
			DrawTab(real, upTab.Size.IsEmpty, downTab.Size.IsEmpty);
			if(!upTab.Size.IsEmpty) {
				DrawTab(upTab, true, false);
				r = real;
				r.Height = lineSize;
				r.X += lineSize;
				r.Width = upTab.X - r.X + lineSize;
				FillRectangle(r);
				r.X = upTab.Right - lineSize;
				r.Width = real.Right - r.X - lineSize;
				FillRectangle(r);
			} 
			if(!downTab.Size.IsEmpty) {
				DrawTab(downTab, false, true);
				r = real;
				r.Y = real.Bottom - lineSize;
				r.Height = lineSize;
				r.X += lineSize;
				r.Width = downTab.X - r.X + lineSize;
				FillRectangle(r);
				r.X = downTab.Right - lineSize;
				r.Width = real.Right - r.X - lineSize;
				FillRectangle(r);
			} 
		}
		protected void DrawTab(Rectangle bounds, bool drawTop, bool drawBottom) {
			Rectangle r;
			r = bounds; r.Width = lineSize;
			FillRectangle(r);
			r.X = bounds.Right - lineSize;
			FillRectangle(r);
			if(drawTop) {
				r = bounds;
				r.Height = lineSize;
				r.Inflate(-lineSize, 0);
				FillRectangle(r);
			}
			if(drawBottom) {
				r = bounds;
				r.Y = r.Bottom - lineSize;
				r.Height = lineSize;
				r.Inflate(-lineSize, 0);
				FillRectangle(r);
			}
		}
		protected void FillRectangle(Rectangle r) {
			if(r.Width < 1 || r.Height < 1) return;
			ControlPaint.FillReversibleRectangle(r, SystemColors.Control);
		}
	}
	public class CanDockTestInfo {
		public BarDockControl DockControl;
		public int DockRow;
		public int DockCol;
		public bool NotEqual;
		public CanDockTestInfo() {
			NotEqual = false;
			DockControl = null;
			DockRow = -1;
			DockCol = -1;
		}
		public bool IsEquals(IDockableObject bar) {
			if(bar.DockControl != DockControl) return false;
			if(DockControl == null) return true;
			if(NotEqual) return false;
			bool eq = (bar.DockInfo.DockRow == DockRow && bar.DockInfo.DockCol == DockCol);
			if(eq) return true;
			if(bar.DockControl == null) return false;
			if(bar.DockInfo.DockRow == DockRow && DockCol == -1) {
				DockRow row = DockControl.Rows[bar.DockInfo.DockRow];
				if(row.Count == 1) return true;
			}
			if(bar.DockInfo.DockRow == DockControl.Rows.Count - 1 && (DockRow == -1 || DockRow == bar.DockInfo.DockRow)) {
				DockRow row = DockControl.Rows[bar.DockInfo.DockRow];
				if(row.Count == 1) return true;
			}
			return false;
		}
		public override string ToString() {
			return string.Format("Dock {0} Row:{1} Col: {2}, Target: {3}", 
				(DockControl != null ? DockControl.DockStyle.ToString() : "<Null>"),
				DockRow, DockCol, "<Null>");
		}
	}
}
