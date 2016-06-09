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
namespace DevExpress.Utils.Design.Taskbar {
	public partial class ThumbnailClipRegionControl : UserControl {
		public ThumbnailClipRegionControl() {
			InitializeComponent();
		}
		protected override void OnEnter(EventArgs e) {
			Cursor oldCursor = Cursor;
		}
		protected override void OnLoad(EventArgs e) {
			InitRectangles();
		}
		protected override void OnPaint(PaintEventArgs e) {
			InitFormScreen(e.Graphics);
		}
		void InitRectangles() {
			formRectangle = new Rectangle((ClientRectangle.Width - FormScreen.Width) / 2, (ClientRectangle.Height - FormScreen.Height) / 2, FormScreen.Width, FormScreen.Height - 45);
			ClientArea = clientAreaDefault = new Rectangle((ClientRectangle.Width - formRectangle.Width) / 2 + 8, (ClientRectangle.Height - formRectangle.Height) / 2 + 28 - 35, FormScreen.Width - 17, FormScreen.Height - 36);
			if(StartValue != Rectangle.Empty)
				RestoreResult();
		}
		public Rectangle StartValue { get; set; }
		Rectangle clientAreaDefault;
		Rectangle clientArea;
		Rectangle ClientArea {
			get {
				return clientArea;
			}
			set {
				if(ClientArea == value) return;
				clientArea = value;
				Invalidate();
				SetResult(clientArea);
			}
		}
		void RestoreResult() {
			Rectangle rect = new Rectangle();
			rect.X = clientAreaDefault.X + StartValue.X;
			rect.Y = clientAreaDefault.Y + StartValue.Y;
			rect.Width = StartValue.Width - StartValue.X;
			rect.Height = StartValue.Height - StartValue.Y;
			ClientArea = rect;
		}
		void SetResult(Rectangle clientArea) {
			Rectangle rect = new Rectangle();
			rect.X = clientArea.X - clientAreaDefault.X;
			rect.Y = clientArea.Y - clientAreaDefault.Y;
			rect.Width = rect.X + clientArea.Width;
			rect.Height = rect.Y + clientArea.Height;
			((ThumbnailClipRegionEditorForm)Parent).Result = rect;
		}
		public object Control;
		Rectangle formRectangle;
		public Bitmap FormScreen { get; set; }
		void InitFormScreen(Graphics g) {
			g.DrawImage(FormScreen, formRectangle);
			DrawSelection(g);
		}
		void DrawSelection(Graphics g) {
			Pen pen = new Pen(Color.FromKnownColor(KnownColor.Highlight));
			g.DrawRectangle(pen, ClientArea);
			FillInvisibleArea(g);
			DrawIcons(g);
		}
		Rectangle ptTop {
			get { return new Rectangle(ClientArea.X + ClientArea.Width / 2 - 8, ClientArea.Y - 8, 16, 16); }
		}
		Rectangle ptLeft {
			get { return new Rectangle(ClientArea.X - 8, ClientArea.Y + ClientArea.Height / 2 - 8, 16, 16); }
		}
		Rectangle ptRight {
			get { return new Rectangle(ClientArea.X + ClientArea.Width - 8, ClientArea.Y + ClientArea.Height / 2 - 8, 16, 16); }
		}
		Rectangle ptBottom {
			get { return new Rectangle(ClientArea.X + ClientArea.Width / 2 - 8, ClientArea.Y + ClientArea.Height - 8, 16, 16); }
		}
		void DrawIcons(Graphics g) {
			Bitmap img = new Bitmap(16, 16);
			for(int i = 0; i < img.Width; i++) {
				for(int j = 0; j < img.Height; j++) {
					img.SetPixel(i, j, Color.Red);
				}
			}
			g.DrawImage(img, ptTop);
			g.DrawImage(img, ptBottom);
			g.DrawImage(img, ptLeft);
			g.DrawImage(img, ptRight);
		}
		void FillInvisibleArea(Graphics g) {
			SolidBrush brush = new SolidBrush(Color.FromArgb(150, Color.FromKnownColor(KnownColor.Highlight)));
			Region region = new Region(ClientRectangle);
			region.Exclude(ClientArea);
			g.FillRegion(brush, region);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(ptBottom.Contains(e.Location) || resizeBottom)
				ActionBottomButton(e);
			else if(ptTop.Contains(e.Location) || resizeTop)
				ActionBottomTop(e);
			else if(ptLeft.Contains(e.Location) || resizeLeft)
				ActionBottomLeft(e);
			else if(ptRight.Contains(e.Location) || resizeRight)
				ActionBottomRight(e);
			else
				Cursor = oldCursor;
			base.OnMouseMove(e);
			oldLocation = e.Location;
		}
		Point oldLocation;
		Cursor oldCursor = null;
		void ActionBottomButton(MouseEventArgs e) {
			Cursor = Cursors.SizeNS;
			if(e.Button == System.Windows.Forms.MouseButtons.Left) {
				resizeBottom = true;
				Rectangle rect = new Rectangle(ClientArea.Location, ClientArea.Size);
				int delta = e.Location.Y - oldLocation.Y;
				if(rect.Height + delta < clientAreaDefault.Height && rect.Height + delta > 0 && rect.Y + rect.Height + delta < clientAreaDefault.Y + clientAreaDefault.Height)
					rect.Height += delta;
				ClientArea = rect;
			}
		}
		void ActionBottomTop(MouseEventArgs e) {
			Cursor = Cursors.SizeNS;
			if(e.Button == System.Windows.Forms.MouseButtons.Left) {
				resizeTop = true;
				Rectangle rect = new Rectangle(ClientArea.Location, ClientArea.Size);
				int delta = e.Location.Y - oldLocation.Y;
				if(rect.Y + delta > clientAreaDefault.Y && rect.Y + delta < rect.Y + rect.Height){
					rect.Y += delta;
					rect.Height -= delta;
				}
				ClientArea = rect;
			}
		}
		void ActionBottomLeft(MouseEventArgs e) {
			Cursor = Cursors.SizeWE;
			if(e.Button == System.Windows.Forms.MouseButtons.Left) {
				resizeLeft = true;
				Rectangle rect = new Rectangle(ClientArea.Location, ClientArea.Size);
				int delta = e.Location.X - oldLocation.X;
				if(rect.X + delta > clientAreaDefault.X && rect.X + delta < rect.X + rect.Width){
					rect.X += delta;
					rect.Width -= delta;
				}
				ClientArea = rect;
			}
		}
		void ActionBottomRight(MouseEventArgs e) {
			Cursor = Cursors.SizeWE;
			if(e.Button == System.Windows.Forms.MouseButtons.Left) {
				resizeRight = true;
				Rectangle rect = new Rectangle(ClientArea.Location, ClientArea.Size);
				int delta = e.Location.X - oldLocation.X;
				if(rect.Width + delta < clientAreaDefault.Width && rect.Width + delta > 0 && rect.X + rect.Width + delta < clientAreaDefault.X + clientAreaDefault.Width)
					rect.Width += delta;
				ClientArea = rect;
			}
		}
		bool resizeTop = false;
		bool resizeBottom = false;
		bool resizeLeft = false;
		bool resizeRight = false;
		protected override void OnMouseUp(MouseEventArgs e) {
			resizeTop = resizeBottom = resizeLeft = resizeRight = false;
		}
	}
}
