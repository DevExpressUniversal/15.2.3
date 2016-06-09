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
using System.ComponentModel;
using System.Collections;
using DevExpress.Utils.Drawing;
namespace DevExpress.Utils.Win {
	public class ToolTipCustomDrawEventArgs : EventArgs { 
		PaintEventArgs paintArgs;
		bool handled;
		public ToolTipCustomDrawEventArgs(PaintEventArgs paintArgs) {
			this.handled = false;
			this.paintArgs = paintArgs;
		}
		public bool Handled { 
			get { return handled; }
			set {
				handled = value;
			}
		}
		public PaintEventArgs PaintArgs { 
			get { return paintArgs; }
		}
	}
	public class ToolTipCalcSizeEventArgs : EventArgs {
		Size size;
		Point topPosition, bottomPosition;
		public ToolTipCalcSizeEventArgs(Point bottomPosition, Point topPosition, Size size) {
			this.topPosition = topPosition;
			this.bottomPosition = bottomPosition;
			this.size = size;
		}
		public Point TopPosition { 
			get { return topPosition; }
			set { topPosition = value;
			}
		}
		public Point BottomPosition { 
			get { return bottomPosition; }
			set { bottomPosition = value;
			}
		}
		public Size Size {
			get { return size; }
			set {
				size = value;
			}
		}
	}
	public class ToolTipCanShowEventArgs : EventArgs {
		string text;
		bool show;
		Point position;
		StringAlignment windowAlignment;
		public ToolTipCanShowEventArgs(bool show, string text, Point position) : this(show, text, position, StringAlignment.Near) {
		}
		public ToolTipCanShowEventArgs(bool show, string text, Point position, StringAlignment windowAlignment) {
			this.text = text;
			this.show = show;
			this.position = position;
			this.windowAlignment = windowAlignment;
		}
		public string Text { get { return text; } set { text = value; } }
		public bool Show { get { return show; } set { show = value; } }
		public Point Position { get { return position; } set { position = value; } }
		public StringAlignment WindowAlignment { get { return windowAlignment; } set { windowAlignment = value; } }
	}
	public delegate void ToolTipCanShowEventHandler(object sender, ToolTipCanShowEventArgs e);
	public class ToolTipEx : IDisposable {
		ToolTipWindow toolWindow;
		int initialDelay, autoPopDelay, reshowDelay;
		bool allowAutoPop;
		Timer initialTimer, autoPopTimer;
		public event ToolTipCanShowEventHandler ToolTipCanShow;
		public event ToolTipCustomDrawEventHandler ToolTipCustomDraw;
		public event ToolTipCalcSizeEventHandler ToolTipCalcSize;
		object activeObject;
		static AppearanceObject defaultStyle;
		AppearanceObject style;
		static ToolTipEx() {
			CreateStyle();
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferencesChanged);
		}
		static void OnUserPreferencesChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			CreateStyle();
		}
		static void CreateStyle() {
			defaultStyle = new AppearanceObject("ToolTip");
			defaultStyle.BackColor = SystemColors.Info;
			defaultStyle.ForeColor = SystemColors.InfoText;
		}
		public ToolTipEx() {
			this.style = null;
			this.toolWindow = new ToolTipWindow();
			this.ToolWindow.ToolTipCalcSize += new ToolTipCalcSizeEventHandler(OnToolTipCalcSize);
			this.ToolWindow.ToolTipCustomDraw += new ToolTipCustomDrawEventHandler(OnToolTipCustomDraw);
			this.activeObject = null;
			this.reshowDelay = 100;
			this.initialDelay = 500;
			this.autoPopDelay = 5000;
			this.allowAutoPop = true;
			this.initialTimer = new Timer();
			this.autoPopTimer = new Timer();
			AutoPopTimer.Tick += new EventHandler(OnAutoPopTimerTick);
			InitialTimer.Tick += new EventHandler(OnInitialTimerTick);
			AutoPopTimer.Interval = AutoPopDelay;
			InitialTimer.Interval = InitialDelay;
			UpdateToolStyle();
		}
		public virtual void Dispose() {
			if(ToolWindow != null) {
				ToolWindow.Dispose();
				this.toolWindow = null;
			}
		}
		public virtual void ObjectEnter(object obj) {
			ActiveObjectCore = obj;
		}
		public virtual void ObjectLeave(object newObject) {
			ActiveObjectCore = newObject;
		}
		protected virtual void OnInitialTimerTick(object sender, EventArgs e) {
			ShowHint();
		}
		protected virtual void OnAutoPopTimerTick(object sender, EventArgs e) {
			if(ToolWindow.Visible && AllowAutoPop) {
				ToolWindow.HideTip();
			}
		}
		public virtual object ActiveObject { get { return ActiveObjectCore; } }
		protected virtual object ActiveObjectCore {
			get { return activeObject; }
			set {
				if(ActiveObjectCore == value) return;
				object prevObject = ActiveObjectCore;
				activeObject = value;
				if(activeObject != null) {
					InitialTimer.Interval = prevObject != null ? ReshowDelay : InitialDelay;
					InitialTimer.Start();
				} else {
					HideHint();
				}
			}
		}
		protected virtual Timer InitialTimer { get { return initialTimer; } }
		protected virtual Timer AutoPopTimer { get { return autoPopTimer; } }
		public virtual ToolTipWindow ToolWindow { get { return toolWindow; } }
		protected virtual void OnToolTipCustomDraw(object sender, ToolTipCustomDrawEventArgs e) {
			if(ToolTipCustomDraw != null) ToolTipCustomDraw(this, e);
		}
		protected virtual void OnToolTipCalcSize(object sender, ToolTipCalcSizeEventArgs e) {
			if(ToolTipCalcSize != null) ToolTipCalcSize(this, e);
		}
		protected virtual AppearanceObject ActiveStyle {
			get { return Style == null ? defaultStyle : Style; }
		}
		protected virtual void UpdateToolStyle() {
			ToolWindow.Font = ActiveStyle.Font;
			ToolWindow.BackColor = ActiveStyle.BackColor;
			ToolWindow.ForeColor = ActiveStyle.ForeColor;
		}
		public virtual void ShowHint() {
			ToolTipCanShowEventArgs e = new ToolTipCanShowEventArgs(false, "", Point.Empty);
			if(ToolTipCanShow != null) ToolTipCanShow(this, e);
			ShowHint(e);
		}
		public virtual void ShowHint(ToolTipCanShowEventArgs e) {
			InitialTimer.Stop();
			if(e.Show) {
				ToolWindow.ToolTipAlignment = e.WindowAlignment;
				ToolWindow.Font = ActiveStyle.Font;
				ToolWindow.BackColor = ActiveStyle.BackColor;
				ToolWindow.ForeColor = ActiveStyle.ForeColor;
				ToolWindow.ToolTip = e.Text;
				ToolWindow.ShowTip(e.Position, new Point(e.Position.X, e.Position.Y + 10));
				if(AllowAutoPop) AutoPopTimer.Start();
			} else {
				HideHint();
			}
		}
		public virtual void HideHint() {
			this.activeObject = null;
			bool prevVisible = ToolWindow.Visible;
			ToolWindow.HideTip();
			AutoPopTimer.Stop();
			InitialTimer.Stop();
		}
		public virtual AppearanceObject Style {
			get { return style; }
			set {
				if(Style == value) return;
				style = value;
				UpdateToolStyle();
			}
		}
		public bool AllowAutoPop {
			get { return allowAutoPop; }
			set { allowAutoPop = value; }
		}
		public int InitialDelay { 
			get { return initialDelay; } 
			set { 
				if(value < 1) value = 1;
				if(InitialDelay == value) return;
				initialDelay = value; 
				InitialTimer.Interval = InitialDelay;
			}
		}
		public int ReshowDelay { 
			get { return reshowDelay; } 
			set { 
				if(value < 1) value = 1;
				if(ReshowDelay == value) return;
				reshowDelay = value; 
			}
		}
		public int AutoPopDelay {
			get { return autoPopDelay; }
			set { 
				if(value < 1) value = 1;
				if(AutoPopDelay == value) return;
				autoPopDelay = value;
				AutoPopTimer.Interval = AutoPopDelay;
			}
		}
	}
	public delegate void ToolTipCalcSizeEventHandler(object sender, ToolTipCalcSizeEventArgs e);
	public delegate void ToolTipCustomDrawEventHandler(object sender, ToolTipCustomDrawEventArgs e);
	[ToolboxItem(false)]
	public class ToolTipWindow : CustomTopForm {
		string toolTip;
		StringAlignment toolTipAlignment;
		Point bottomPosition, topPosition;
		bool mouseTransparent;
		public event ToolTipCustomDrawEventHandler ToolTipCustomDraw;
		public event ToolTipCalcSizeEventHandler ToolTipCalcSize;
		public ToolTipWindow() {
			mouseTransparent = true;
			toolTipAlignment = StringAlignment.Near;
			Font = SystemInformation.MenuFont;
			toolTip = "";
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			ControlBox = false;
			BackColor = SystemColors.Info;
			ForeColor = SystemColors.InfoText;
			SetStyle(ControlStyles.Opaque, true);
		}
		public void ShowTip(Point bottomPosition, Point topPosition) {
			this.bottomPosition = bottomPosition;
			this.topPosition = topPosition;
			ToolTipChanged(true);
			Visible = true;
		}
		public void HideTip() {
			Visible = false;
		}
		public virtual bool MouseTransparent { 
			get { return mouseTransparent; }
			set {
				mouseTransparent = value;
			}
		}
		public StringAlignment ToolTipAlignment {
			get { return toolTipAlignment; }
			set {
				if(ToolTipAlignment == value || value == StringAlignment.Center) return;
				toolTipAlignment = value;
				ToolTipChanged(false);
			}
		}
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			const int WM_NCHITTEST = 0x84, HTTRANSPARENT =(-1);
			base.WndProc(ref m);
			switch(m.Msg) {
				case WM_NCHITTEST : 
					if(MouseTransparent) m.Result = new IntPtr(HTTRANSPARENT);
					break;
			}
		}
		protected virtual Size CalcTipSize() {
			Graphics g = this.CreateGraphics();
			GraphicsCache cache = new GraphicsCache(g);
			ToolTipCalcSizeEventArgs sizeArgs = new ToolTipCalcSizeEventArgs(bottomPosition, topPosition, Size.Empty);
			try {
				sizeArgs.Size = cache.CalcTextSize(ToolTip, Font, TextOptions.DefaultStringFormat, 0).ToSize();
				sizeArgs.Size = new Size(sizeArgs.Size.Width + 4, sizeArgs.Size.Height + 4);
				if(ToolTipCalcSize != null) {
					ToolTipCalcSize(this, sizeArgs);
					bottomPosition = sizeArgs.BottomPosition;
					topPosition = sizeArgs.TopPosition;
				}
			}
			finally {
				g.Dispose();
				cache.Dispose();
			}
			return sizeArgs.Size;
		}
		public string ToolTip {
			get { return toolTip; }
			set {
				if(ToolTip == value) return;
				toolTip = value;
				ToolTipChanged(false);
			}
		}
		protected virtual void ToolTipChanged(bool makeVisible) {
			if(!Visible && !makeVisible) return;
			Point btm = bottomPosition, top = topPosition;
			Size size = CalcTipSize();
			if(size.Width < 1 || size.Height < 1) {
				Size = size;
				HideTip();
				return;
			}
			if(ToolTipAlignment == StringAlignment.Far) {
				btm.X -= size.Width;
				top.X -= size.Width;
			}
			Point loc = ControlUtils.CalcLocation(btm, top, size);
			ClientSize = size;
			Location = loc;
			Invalidate();
			if(makeVisible)
				Visible = true;
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(ToolTipCustomDraw != null) {
				ToolTipCustomDrawEventArgs args = new ToolTipCustomDrawEventArgs(e);
				ToolTipCustomDraw(this, args);
				if(args.Handled) return;
			}
			Brush backBrush = new SolidBrush(BackColor), foreBrush = new SolidBrush(ForeColor);
			GraphicsCache cache = new GraphicsCache(e);
			e.Graphics.FillRectangle(backBrush, ClientRectangle);
			e.Graphics.FillRectangle(Brushes.Black, new Rectangle(0, Height - 1, Width, 1));
			e.Graphics.FillRectangle(Brushes.Black, new Rectangle(Width - 1, 0, 1, Height));
			cache.DrawString(ToolTip, Font, foreBrush, new Rectangle(2, 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4), TextOptions.DefaultStringFormat);
			backBrush.Dispose();
			foreBrush.Dispose();
		}
	}
}
