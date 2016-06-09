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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Views.Grid {
	[ToolboxItem(false)]
	public class DetailTip : Control {
		Timer closeTimer;
		GridView view;
		bool mouseInTip;
		int rowHandle, selectedIndex, seconds;
		StringFormat stringFormat;
		List<TipInfo> relationInfo;
		public class TipInfo {
			public Rectangle Bounds;
			GridDetailInfo detailInfo;
			public TipInfo(GridDetailInfo detailInfo)
				: this(detailInfo, Rectangle.Empty) {
			}
			public TipInfo(GridDetailInfo detailInfo, Rectangle bounds) {
				this.detailInfo = detailInfo;
				this.Bounds = bounds;
			}
			public GridDetailInfo DetailInfo { get { return detailInfo; } }
			internal int CaptionLength = 20;
			public string GetCaption() {
				int maxLength = CaptionLength;
				if(maxLength < CaptionLength) maxLength = 20;
				string res = DetailInfo.Caption.Trim();
				if(res.Length > maxLength)
					res = res.Substring(0, maxLength);
				if(res.Length == 0)
					res = "Relation " + RelationIndex.ToString();
				return res;
			}
			public int RelationIndex { get { return DetailInfo.RelationIndex; } }
		}
		public DetailTip(GridView view, int rowHandle) {
			this.relationInfo = new List<TipInfo>();
			this.view = view;
			this.rowHandle = rowHandle;
			stringFormat = StringFormat.GenericTypographic.Clone() as StringFormat;
			stringFormat.Trimming = StringTrimming.None;
			stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
			stringFormat.FormatFlags = StringFormatFlags.NoWrap;
			Reset();
		}
		public GridView View {
			get { return view; }
		}
		public bool MouseInTip {
			get { return mouseInTip; }
		}
		public int RowHandle {
			get { return rowHandle; }
			set { rowHandle = value; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(stringFormat != null)
					stringFormat.Dispose();
				if(closeTimer != null) 
					DestroyTimer();
				view = null;
			}
			base.Dispose(disposing);
		}
		void CreateTimer() {
			closeTimer = new Timer();
			closeTimer.Interval = 1000;
			closeTimer.Tick += OnCloseTimer;
		}
		void DestroyTimer() {
			closeTimer.Tick -= OnCloseTimer;
			closeTimer.Dispose();
			closeTimer = null;
		}
		public void Reset() {
			if(closeTimer != null)
				DestroyTimer();
			RowHandle = DevExpress.Data.DataController.InvalidRow;
			mouseInTip = false;
			selectedIndex = -1;
			seconds = 0;
			CreateTimer();
		}
		public void ShowTip() {
			closeTimer.Start();
			Visible = true;
		}
		public void HideTip() {
			Visible = false;
			Reset();
		}
		protected void OnCloseTimer(Object sender, EventArgs e) {
			if(++seconds > 1) {
				if(!MouseInTip) {
					int saveRowHandle = RowHandle;
					View.HideDetailTip();
					RowHandle = saveRowHandle;
					return;
				}
			}
		}
		public bool CanShowTip {
			get { return relationInfo.Count > 0; }
		}
		public Size CalcSize(GridDetailInfo[] details) {
			Size res = Size.Empty;
			relationInfo.Clear();
			if(details == null || details.Length == 0) return res;
			using(Graphics g = CreateGraphics()) {
				res.Height = Convert.ToInt32(g.MeasureString("Wg", Font, 0, stringFormat).Height) + 6;
				int lastLeft = 4;
				foreach(GridDetailInfo detail in details) {
					if(!View.OptionsDetail.AllowExpandEmptyDetails && View.IsMasterRowEmptyEx(RowHandle, detail.RelationIndex))
						continue;
					TipInfo tip = new TipInfo(detail);
					tip.CaptionLength = 150 / details.Length;
					Size size = g.MeasureString(tip.GetCaption(), Font, 0, stringFormat).ToSize();
					size.Width += 1; size.Height += 1;
					tip.Bounds = new Rectangle(lastLeft, 2, size.Width, size.Height);
					lastLeft += tip.Bounds.Width + 7;
					relationInfo.Add(tip);
				}
				res.Width = lastLeft;
			}
			return res;
		}
		TipInfo GetTipInfo(int index) {
			if(index < 0 || index >= relationInfo.Count) return null;
			return relationInfo[index] as TipInfo;
		}
		int GetSelectedTipIndex(int x, int y) {
			for(int n = 0; n < relationInfo.Count; n++) {
				if(GetTipInfo(n).Bounds.Contains(x, y))
					return n;
			}
			return -1;
		}
		int SelectedIndex {
			get { return selectedIndex; }
			set {
				if(value == SelectedIndex) return;
				selectedIndex = value;
				Invalidate();
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) { }
		protected override void OnPaint(PaintEventArgs e) {
			Rectangle r = ClientRectangle;
			r.Width--;
			r.Height--;
			e.Graphics.DrawRectangle(SystemPens.WindowFrame, r);
			r = ClientRectangle;
			r.Inflate(-1, -1);
			using(Brush backBrush = new SolidBrush(BackColor))
				e.Graphics.FillRectangle(backBrush, r);
			e.Graphics.SetClip(r);
			using(Brush foreBrush = new SolidBrush(ForeColor)) {
				for(int i = 0; i < relationInfo.Count; i++) {
					TipInfo tip = GetTipInfo(i);
					if(tip != null) {
						e.Graphics.DrawString(tip.GetCaption(), Font, foreBrush, tip.Bounds, stringFormat);
						if(i == SelectedIndex) {
							using(Pen pen = new Pen(foreBrush)) {
								e.Graphics.DrawLine(pen, tip.Bounds.Left, tip.Bounds.Bottom,
									tip.Bounds.Right, tip.Bounds.Bottom);
							}
						}
					}
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			SelectedIndex = GetSelectedTipIndex(e.X, e.Y);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			int index = GetSelectedTipIndex(e.X, e.Y);
			int row = RowHandle;
			View.HideDetailTip();
			TipInfo tip = GetTipInfo(index);
			if(tip == null) return;
			View.VisualSetMasterRowExpandedEx(row, tip.RelationIndex, true);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			mouseInTip = true;
		}
		protected override void OnMouseLeave(EventArgs e) {
			View.HideDetailTip();
		}
	}
}
