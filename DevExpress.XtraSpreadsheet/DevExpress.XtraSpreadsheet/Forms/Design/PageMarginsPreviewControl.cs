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

using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class PageMarginsPreviewControl : XtraUserControl, INotifyPropertyChanged {
		#region Fields
		Graphics graphics;
		float topMargin;
		float bottomMargin;
		float leftMargin;
		float rightMargin;
		float headerMargin;
		float footerMargin;
		Pen topMarginPen = new Pen(Color.Black);
		Pen bottomMarginPen = new Pen(Color.Black);
		Pen leftMarginPen = new Pen(Color.Black);
		Pen rightMarginPen = new Pen(Color.Black);
		Pen headerMarginPen = new Pen(Color.Black);
		Pen footerMarginPen = new Pen(Color.Black);
		Pen cellsPen = new Pen(Color.Black);
		#endregion
		public PageMarginsPreviewControl() {
			InitializeComponent();
			SubscribeEvents();
		}
		protected internal void SubscribeEvents() {
			this.edtTop.EditValueChanged += OnTopChanged;
			this.edtTop.GotFocus += OnTopGotFocus;
			this.edtTop.LostFocus += OnTopLostFocus;
			this.edtBottom.EditValueChanged += OnBottomChanged;
			this.edtBottom.GotFocus += OnBottomGotFocus;
			this.edtBottom.LostFocus += OnBottomLostFocus;
			this.edtLeft.EditValueChanged += OnLeftChanged;
			this.edtLeft.GotFocus += OnLeftGotFocus;
			this.edtLeft.LostFocus += OnLeftLostFocus;
			this.edtRight.EditValueChanged += OnRightChanged;
			this.edtRight.GotFocus += OnRightGotFocus;
			this.edtRight.LostFocus += OnRightLostFocus;
			this.edtHeader.EditValueChanged += OnHeaderChanged;
			this.edtHeader.GotFocus += OnHeaderGotFocus;
			this.edtHeader.LostFocus += OnHeaderLostFocus;
			this.edtFooter.EditValueChanged += OnFooterChanged;
			this.edtFooter.GotFocus += OnFooterGotFocus;
			this.edtFooter.LostFocus += OnFooterLostFocus;
			this.drawPanelPortraitOrientation.VisibleChanged += OnDrawPanelVerticalVisibleChanged;
			this.drawPanelLandscapeOrientation.VisibleChanged += OnDrawPanelHorizontalVisibleChanged;
			this.chkHorizontally.CheckedChanged += OnHorizontallyCheckedChanged;
			this.chkVertically.CheckedChanged += OnVerticallyCheckedChanged;
			this.drawPanelPortraitOrientation.Paint += OnPaintPortraitOrientation;
			this.drawPanelLandscapeOrientation.Paint += OnPaintLandscapeOrientation;
		}
		#region Properties
		public float TopMargin {
			get { return topMargin; }
			set {
				if (TopMargin == value)
					return;
				topMargin = value;
				this.edtTop.EditValue = topMargin;
				OnPropertyChanged("TopMargin");
			}
		}
		public float BottomMargin {
			get { return bottomMargin; }
			set {
				if (BottomMargin == value)
					return;
				bottomMargin = value;
				this.edtBottom.EditValue = bottomMargin;
				OnPropertyChanged("BottomMargin");
			}
		}
		public float LeftMargin {
			get { return leftMargin; }
			set {
				if (LeftMargin == value)
					return;
				leftMargin = value;
				this.edtLeft.EditValue = leftMargin;
				OnPropertyChanged("LeftMargin");
			}
		}
		public float RightMargin {
			get { return rightMargin; }
			set {
				if (RightMargin == value)
					return;
				rightMargin = value;
				this.edtRight.EditValue = rightMargin;
				OnPropertyChanged("RightMargin");
			}
		}
		public float HeaderMargin {
			get { return headerMargin; }
			set {
				if (HeaderMargin == value)
					return;
				headerMargin = value;
				this.edtHeader.EditValue = headerMargin;
				OnPropertyChanged("HeaderMargin");
			}
		}
		public float FooterMargin {
			get { return footerMargin; }
			set {
				if (FooterMargin == value)
					return;
				footerMargin = value;
				this.edtFooter.EditValue = footerMargin;
				OnPropertyChanged("FooterMargin");
			}
		}
		public bool DrawPanelPortraitOrientation {
			get { return this.drawPanelPortraitOrientation.Visible; }
			set {
				this.drawPanelPortraitOrientation.Visible = value;
			}
		}
		public bool DrawPanelLandscapeOrientation {
			get { return this.drawPanelLandscapeOrientation.Visible; }
			set {
				this.drawPanelLandscapeOrientation.Visible = !value;
			}
		}
		public bool IsCenterHorizontally {
			get { return this.chkHorizontally.Checked; }
			set {
				if (IsCenterHorizontally == value)
					return;
				this.chkHorizontally.Checked = value;
			}
		}
		public bool IsCenterVertically {
			get { return this.chkVertically.Checked; }
			set {
				if (IsCenterVertically == value)
					return;
				this.chkVertically.Checked = value;
			}
		}
		#endregion
		#region Events
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		void OnTopChanged(object sender, EventArgs e) {
			TopMargin = float.Parse(edtTop.EditValue.ToString());
		}
		void OnTopGotFocus(object sender, EventArgs e) {
			topMarginPen.Color = Color.Red;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnTopLostFocus(object sender, EventArgs e) {
			topMarginPen.Color = Color.Black;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnBottomChanged(object sender, EventArgs e) {
			BottomMargin = float.Parse(edtBottom.EditValue.ToString());
		}
		void OnBottomGotFocus(object sender, EventArgs e) {
			bottomMarginPen.Color = Color.Red;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnBottomLostFocus(object sender, EventArgs e) {
			bottomMarginPen.Color = Color.Black;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnLeftChanged(object sender, EventArgs e) {
			LeftMargin = float.Parse(edtLeft.EditValue.ToString());
		}
		void OnLeftGotFocus(object sender, EventArgs e) {
			leftMarginPen.Color = Color.Red;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnLeftLostFocus(object sender, EventArgs e) {
			leftMarginPen.Color = Color.Black;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnRightChanged(object sender, EventArgs e) {
			RightMargin = float.Parse(edtRight.EditValue.ToString());
		}
		void OnRightGotFocus(object sender, EventArgs e) {
			rightMarginPen.Color = Color.Red;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnRightLostFocus(object sender, EventArgs e) {
			rightMarginPen.Color = Color.Black;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnHeaderChanged(object sender, EventArgs e) {
			HeaderMargin = float.Parse(edtHeader.EditValue.ToString());
		}
		void OnHeaderGotFocus(object sender, EventArgs e) {
			headerMarginPen.Color = Color.Red;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnHeaderLostFocus(object sender, EventArgs e) {
			headerMarginPen.Color = Color.Black;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnFooterChanged(object sender, EventArgs e) {
			FooterMargin = float.Parse(edtFooter.EditValue.ToString());
		}
		void OnFooterGotFocus(object sender, EventArgs e) {
			footerMarginPen.Color = Color.Red;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnFooterLostFocus(object sender, EventArgs e) {
			footerMarginPen.Color = Color.Black;
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnDrawPanelVerticalVisibleChanged(object sender, EventArgs e) {
			OnPropertyChanged("DrawPanelVerticalVisible");
		}
		void OnDrawPanelHorizontalVisibleChanged(object sender, EventArgs e) {
			OnPropertyChanged("DrawPanelHorizontalVisible");
		}
		void OnHorizontallyCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsCenterHorizontally");
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnVerticallyCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsCenterVertically");
			this.drawPanelPortraitOrientation.Refresh();
			this.drawPanelLandscapeOrientation.Refresh();
		}
		void OnPaintPortraitOrientation(object sender, System.Windows.Forms.PaintEventArgs e) {
			this.graphics = e.Graphics;
			DrawCellsPortraitOrientation();
			DrawTopMarginPortraitOrientation();
			DrawBottomMarginPortraitOrientation();
			DrawLeftMarginPortraitOrientation();
			DrawRightMarginPortraitOrientation();
			DrawHeaderMarginPortraitOrientation();
			DrawFooterMarginPortraitOrientation();
		}
		void OnPaintLandscapeOrientation(object sender, System.Windows.Forms.PaintEventArgs e) {
			this.graphics = e.Graphics;
			DrawCellsLandscapeOrientation();
			DrawTopMarginLandscapeOrientation();
			DrawBottomMarginLandscapeOrientation();
			DrawLeftMarginLandscapeOrientation();
			DrawRightMarginLandscapeOrientation();
			DrawHeaderMarginLandscapeOrientation();
			DrawFooterMarginLandscapeOrientation();
		}
		void DrawCellsPortraitOrientation() {
			int coordStartX = MarginsLayoutManagerPortraitOrientation.TopPointLeftMargins.X;
			int coordStartY = MarginsLayoutManagerPortraitOrientation.LeftPointTopMargins.Y;
			if (IsCenterHorizontally && !IsCenterVertically)
				coordStartX = 30;
			else if (IsCenterVertically && !IsCenterHorizontally)
				coordStartY = 30;
			else if (IsCenterHorizontally && IsCenterVertically) {
				coordStartX = 30;
				coordStartY = 30;
			}
			for (int i = 0; i < 15; i++) {
				for (int j = 0; j < 5; j++)
					graphics.DrawLine(cellsPen, new Point(coordStartX + j * 10, coordStartY), new Point(coordStartX + j * 10, coordStartY + 70));
				graphics.DrawLine(cellsPen, new Point(coordStartX, coordStartY + i * 5), new Point(coordStartX + 40, coordStartY + i * 5));
			}
		}
		void DrawTopMarginPortraitOrientation() {
			graphics.DrawLine(topMarginPen, MarginsLayoutManagerPortraitOrientation.LeftPointTopMargins, MarginsLayoutManagerPortraitOrientation.RightPointTopMargins);
		}
		void DrawBottomMarginPortraitOrientation() {
			graphics.DrawLine(bottomMarginPen, MarginsLayoutManagerPortraitOrientation.LeftPointBottomMargins, MarginsLayoutManagerPortraitOrientation.RightPointBottomMargins);
		}
		void DrawLeftMarginPortraitOrientation() {
			graphics.DrawLine(leftMarginPen, MarginsLayoutManagerPortraitOrientation.TopPointLeftMargins, MarginsLayoutManagerPortraitOrientation.BottomPointLeftMargins);
		}
		void DrawRightMarginPortraitOrientation() {
			graphics.DrawLine(rightMarginPen, MarginsLayoutManagerPortraitOrientation.TopPointRightMargins, MarginsLayoutManagerPortraitOrientation.BottomPointRightMargins);
		}
		void DrawHeaderMarginPortraitOrientation() {
			graphics.DrawLine(headerMarginPen, MarginsLayoutManagerPortraitOrientation.LeftPointHeaderMargins, MarginsLayoutManagerPortraitOrientation.RightPointHeaderMargins);
		}
		void DrawFooterMarginPortraitOrientation() {
			graphics.DrawLine(footerMarginPen, MarginsLayoutManagerPortraitOrientation.LeftPointFooterMargins, MarginsLayoutManagerPortraitOrientation.RightPointFooterMargins);
		}
		void DrawCellsLandscapeOrientation() {
			int coordStartX = MarginsLayoutManagerLandscapeOrientation.TopPointLeftMargins.X;
			int coordStartY = MarginsLayoutManagerLandscapeOrientation.LeftPointTopMargins.Y;
			if (IsCenterHorizontally && !IsCenterVertically)
				coordStartX = 30;
			else if (IsCenterVertically && !IsCenterHorizontally)
				coordStartY = 30;
			else if (IsCenterHorizontally && IsCenterVertically) {
				coordStartX = 30;
				coordStartY = 30;
			}
			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 8; j++)
					graphics.DrawLine(cellsPen, new Point(coordStartX + j * 10, coordStartY), new Point(coordStartX + j * 10, coordStartY + 40));
				graphics.DrawLine(cellsPen, new Point(coordStartX, coordStartY + i * 5), new Point(coordStartX + 70, coordStartY + i * 5));
			}
		}
		void DrawTopMarginLandscapeOrientation() {
			graphics.DrawLine(topMarginPen, MarginsLayoutManagerLandscapeOrientation.LeftPointTopMargins, MarginsLayoutManagerLandscapeOrientation.RightPointTopMargins);
		}
		void DrawBottomMarginLandscapeOrientation() {
			graphics.DrawLine(bottomMarginPen, MarginsLayoutManagerLandscapeOrientation.LeftPointBottomMargins, MarginsLayoutManagerLandscapeOrientation.RightPointBottomMargins);
		}
		void DrawLeftMarginLandscapeOrientation() {
			graphics.DrawLine(leftMarginPen, MarginsLayoutManagerLandscapeOrientation.TopPointLeftMargins, MarginsLayoutManagerLandscapeOrientation.BottomPointLeftMargins);
		}
		void DrawRightMarginLandscapeOrientation() {
			graphics.DrawLine(rightMarginPen, MarginsLayoutManagerLandscapeOrientation.TopPointRightMargins, MarginsLayoutManagerLandscapeOrientation.BottomPointRightMargins);
		}
		void DrawHeaderMarginLandscapeOrientation() {
			graphics.DrawLine(headerMarginPen, MarginsLayoutManagerLandscapeOrientation.LeftPointHeaderMargins, MarginsLayoutManagerLandscapeOrientation.RightPointHeaderMargins);
		}
		void DrawFooterMarginLandscapeOrientation() {
			graphics.DrawLine(footerMarginPen, MarginsLayoutManagerLandscapeOrientation.LeftPointFooterMargins, MarginsLayoutManagerLandscapeOrientation.RightPointFooterMargins);
		}
	}
	#region MarginsLayoutManagerPortraitOrientation
	static class MarginsLayoutManagerPortraitOrientation {
		#region Fields
		static int heightDrawPanel = 130;
		static int widthDrawPanel = 100;
		static int horizontalLeftRightOffset = 15;
		static int verticalHeaderFooterOffset = 7;
		static int verticalTopBottomOffset = 17;
		static Point leftPointTopMargins = new Point(0, verticalTopBottomOffset);
		static Point rightPointTopMargins = new Point(widthDrawPanel, verticalTopBottomOffset);
		static Point leftPointBottomMargins = new Point(0, heightDrawPanel - verticalTopBottomOffset);
		static Point rightPointBottomMargins = new Point(widthDrawPanel, heightDrawPanel - verticalTopBottomOffset);
		static Point topPointLeftMargins = new Point(horizontalLeftRightOffset, 0);
		static Point bottomPointLeftMargins = new Point(horizontalLeftRightOffset, heightDrawPanel);
		static Point topPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, 0);
		static Point bottomPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, heightDrawPanel);
		static Point leftPointHeaderMargins = new Point(0, verticalHeaderFooterOffset);
		static Point rightPointHeaderMargins = new Point(widthDrawPanel, verticalHeaderFooterOffset);
		static Point leftPointFooterMargins = new Point(0, heightDrawPanel - verticalHeaderFooterOffset);
		static Point rightPointFooterMargins = new Point(widthDrawPanel, heightDrawPanel - verticalHeaderFooterOffset);
		#endregion
		#region Properties
		public static Point LeftPointTopMargins { get { return leftPointTopMargins; } }
		public static Point RightPointTopMargins { get { return rightPointTopMargins; } }
		public static Point LeftPointBottomMargins { get { return leftPointBottomMargins; } }
		public static Point RightPointBottomMargins { get { return rightPointBottomMargins; } }
		public static Point TopPointLeftMargins { get { return topPointLeftMargins; } }
		public static Point BottomPointLeftMargins { get { return bottomPointLeftMargins; } }
		public static Point TopPointRightMargins { get { return topPointRightMargins; } }
		public static Point BottomPointRightMargins { get { return bottomPointRightMargins; } }
		public static Point LeftPointHeaderMargins { get { return leftPointHeaderMargins; } }
		public static Point RightPointHeaderMargins { get { return rightPointHeaderMargins; } }
		public static Point LeftPointFooterMargins { get { return leftPointFooterMargins; } }
		public static Point RightPointFooterMargins { get { return rightPointFooterMargins; } }
		#endregion
	}
	#endregion
	#region MarginsLayoutManagerLandscapeOrientation
	static class MarginsLayoutManagerLandscapeOrientation {
		#region Fields
		static int heightDrawPanel = 100;
		static int widthDrawPanel = 130;
		static int horizontalLeftRightOffset = 15;
		static int verticalHeaderFooterOffset = 7;
		static int verticalTopBottomOffset = 17;
		static Point leftPointTopMargins = new Point(0, verticalTopBottomOffset);
		static Point rightPointTopMargins = new Point(widthDrawPanel, verticalTopBottomOffset);
		static Point leftPointBottomMargins = new Point(0, heightDrawPanel - verticalTopBottomOffset);
		static Point rightPointBottomMargins = new Point(widthDrawPanel, heightDrawPanel - verticalTopBottomOffset);
		static Point topPointLeftMargins = new Point(horizontalLeftRightOffset, 0);
		static Point bottomPointLeftMargins = new Point(horizontalLeftRightOffset, heightDrawPanel);
		static Point topPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, 0);
		static Point bottomPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, heightDrawPanel);
		static Point leftPointHeaderMargins = new Point(0, verticalHeaderFooterOffset);
		static Point rightPointHeaderMargins = new Point(widthDrawPanel, verticalHeaderFooterOffset);
		static Point leftPointFooterMargins = new Point(0, heightDrawPanel - verticalHeaderFooterOffset);
		static Point rightPointFooterMargins = new Point(widthDrawPanel, heightDrawPanel - verticalHeaderFooterOffset);
		#endregion
		#region Properties
		public static Point LeftPointTopMargins { get { return leftPointTopMargins; } }
		public static Point RightPointTopMargins { get { return rightPointTopMargins; } }
		public static Point LeftPointBottomMargins { get { return leftPointBottomMargins; } }
		public static Point RightPointBottomMargins { get { return rightPointBottomMargins; } }
		public static Point TopPointLeftMargins { get { return topPointLeftMargins; } }
		public static Point BottomPointLeftMargins { get { return bottomPointLeftMargins; } }
		public static Point TopPointRightMargins { get { return topPointRightMargins; } }
		public static Point BottomPointRightMargins { get { return bottomPointRightMargins; } }
		public static Point LeftPointHeaderMargins { get { return leftPointHeaderMargins; } }
		public static Point RightPointHeaderMargins { get { return rightPointHeaderMargins; } }
		public static Point LeftPointFooterMargins { get { return leftPointFooterMargins; } }
		public static Point RightPointFooterMargins { get { return rightPointFooterMargins; } }
		#endregion
	}
	#endregion
}
