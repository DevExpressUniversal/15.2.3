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
using System.Data;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Wizards;
namespace DevExpress.XtraReports.Design {
	public class PreviewControl : PreviewControlBase {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRSideMargins { get { return base.SRSideMargins; } set { base.SRSideMargins = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRTopMargin { get { return base.SRTopMargin; } set { base.SRTopMargin = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRVertPitch { get { return base.SRVertPitch; } set { base.SRVertPitch = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRHorizPitch { get { return base.SRHorizPitch; } set { base.SRHorizPitch = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRWidth { get { return base.SRWidth; } set { base.SRWidth = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRHeight { get { return base.SRHeight; } set { base.SRHeight = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRNumberDown { get { return base.SRNumberDown; } set { base.SRNumberDown = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SRNumberAcross { get { return base.SRNumberAcross; } set { base.SRNumberAcross = value; } }
		public PreviewControl() {
			InitializeComponent();
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewControl));
			this.SuspendLayout();
			this.Name = "PreviewControl";
			resources.ApplyResources(this, "$this");
			this.ResumeLayout(false);
		}
	}
	[ToolboxItem(false)]
	public class PreviewControlBase : System.Windows.Forms.UserControl {
		#region static
		static GraphicsPath CreateRoundRectPath(float x, float y, float width, float height, float radius) {
			GraphicsPath path = new GraphicsPath();
			float right = x + width - radius;
			float bottom = y + height - radius;
			path.AddArc(x, y, radius, radius, 180, 90);
			path.AddLine(x + radius, y, right, y);
			path.AddArc(right, y, radius, radius, 270, 90);
			path.AddLine(x + width, y + radius, x + width, bottom);
			path.AddArc(right, bottom, radius, radius, 0, 90);
			path.AddLine(right, y + height, x + radius, y + height);
			path.AddArc(x, bottom, radius, radius, 90, 90);
			path.CloseFigure();
			return path;
		}
		static void RoundedRectangle(Graphics gr, Brush br, Pen pen, RectangleF rect, float radius, bool fill) {
			GraphicsPath path = CreateRoundRectPath(rect.X, rect.Y, rect.Width, rect.Height, radius);
			if(fill)
				gr.FillPath(br, path);
			gr.DrawPath(pen, path);
		}
		#endregion
		Pen blackPen = new Pen(Color.Black);
		Pen doubleArrowPen = new Pen(Color.Black);
		Pen arrowPen = new Pen(Color.Black);
		Brush grayBrush = new SolidBrush(Color.LightGray);
		Brush lightBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Window));
		Brush blackBrush = new SolidBrush(Color.Black);
		Font font = new Font("Arial", 8);
		LabelInfo labelInfo;
		string srSideMargins = ReportLocalizer.GetString(ReportStringId.SR_Side_Margins);
		string srTopMargin = ReportLocalizer.GetString(ReportStringId.SR_Top_Margin);
		string srVPitch = ReportLocalizer.GetString(ReportStringId.SR_Vertical_Pitch);
		string srHPitch = ReportLocalizer.GetString(ReportStringId.SR_Horizontal_Pitch);
		string srWidth = ReportLocalizer.GetString(ReportStringId.SR_Width);
		string srHeight = ReportLocalizer.GetString(ReportStringId.SR_Height);
		string srNumberDown = ReportLocalizer.GetString(ReportStringId.SR_Number_Down);
		string srNumberAcross = ReportLocalizer.GetString(ReportStringId.SR_Number_Across);
		[Localizable(true), Category("String Resources")]
		public virtual string SRSideMargins { get { return srSideMargins; } set { srSideMargins = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRTopMargin { get { return srTopMargin; } set { srTopMargin = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRVertPitch { get { return srVPitch; } set { srVPitch = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRHorizPitch { get { return srHPitch; } set { srHPitch = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRWidth { get { return srWidth; } set { srWidth = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRHeight { get { return srHeight; } set { srHeight = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRNumberDown { get { return srNumberDown; } set { srNumberDown = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRNumberAcross { get { return srNumberAcross; } set { srNumberAcross = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LabelInfo LabelInfo { get { return labelInfo; } set { labelInfo = value; } }
		public PreviewControlBase() {
			arrowPen.EndCap = LineCap.ArrowAnchor;
			doubleArrowPen.StartCap = LineCap.ArrowAnchor;
			doubleArrowPen.EndCap = LineCap.ArrowAnchor;
			this.Dock = DockStyle.Fill;
			this.BackColor = Color.FromKnownColor(KnownColor.Window);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				blackPen.Dispose();
				arrowPen.Dispose();
				doubleArrowPen.Dispose();
				grayBrush.Dispose();
				lightBrush.Dispose();
				blackBrush.Dispose();
				font.Dispose();
			}
			base.Dispose(disposing);
		}
		void DrawPageSample(Graphics gr, float totalWidth, float totalHeight, bool fill) {
			RectangleF rect = new RectangleF(0, 0, totalWidth, totalHeight);
			if (fill)
				gr.FillRectangle(grayBrush, rect);
			gr.DrawRectangle(blackPen, rect.X, rect.Y, rect.Width, rect.Height);
		}
		void DrawLabelSamples(Graphics gr, float totalWidth, float totalHeight, bool fill) {
			Region oldClip = gr.Clip;
			try {
				float radius = Math.Min(totalWidth, totalHeight) / 5;
				RectangleF rect = new RectangleF(0, 0, totalWidth, totalHeight);
				gr.Clip = new Region(rect);
				rect = new RectangleF(labelInfo.LeftMargin, labelInfo.TopMargin, labelInfo.LabelWidth, labelInfo.LabelHeight);
				RoundedRectangle(gr, lightBrush, blackPen, rect, radius, fill);
				rect.Offset(labelInfo.HPitch, 0);
				RoundedRectangle(gr, lightBrush, blackPen, rect, radius, fill);
				rect.Offset(0, labelInfo.VPitch);
				RoundedRectangle(gr, lightBrush, blackPen, rect, radius, fill);
				rect.Offset(-labelInfo.HPitch, 0);
				RoundedRectangle(gr, lightBrush, blackPen, rect, radius, fill);
			}
			finally {
				gr.Clip = oldClip;
				oldClip.Dispose();
			}
		}
		float GetTextWidth(Graphics gr, string text) {
			return gr.MeasureString(text, font).Width;
		}
		float GetTextHeight(Graphics gr, string text) {
			return gr.MeasureString(text, font).Height;
		}
		void DrawString(Graphics gr, string text, RectangleF rect, StringAlignment hAlign, StringAlignment vAlign) {
			StringFormat sf = StringFormat.GenericDefault.Clone() as StringFormat;
			sf.Alignment = hAlign;
			sf.LineAlignment = vAlign;
			SizeF sz = gr.MeasureString(text, font, rect.Size, sf);
			if (rect.Width < sz.Width) rect.Width = sz.Width;
			if (rect.Height < sz.Height) rect.Height = sz.Height;
			RectangleF textRect = rect;
			textRect.Width = sz.Width;
			textRect.Height = sz.Height;
			if (hAlign == StringAlignment.Center)
				textRect.Offset((rect.Width - textRect.Width) / 2, 0);
			else if (hAlign == StringAlignment.Far)
				textRect.Offset(rect.Width - textRect.Width, 0);
			if (vAlign == StringAlignment.Center)
				textRect.Offset(0, (rect.Height - textRect.Height) / 2);
			else if (hAlign == StringAlignment.Far)
				textRect.Offset(0, rect.Height - textRect.Height);
			gr.FillRectangle(lightBrush, textRect);
			gr.DrawString(text, font, blackBrush, rect, sf);
			sf.Dispose();
		}
		void DrawPreview(Graphics gr, float totalWidth, float totalHeight) {
			DrawPageSample(gr, totalWidth, totalHeight, true);
			DrawLabelSamples(gr, totalWidth, totalHeight, true);
			float hPitchStringHeight = GetTextHeight(gr, SRHorizPitch);
			float widthStringHeight = GetTextHeight(gr, SRWidth);
			float heightStringWidth = GetTextWidth(gr, SRHeight);
			float vPitchStringWidth = GetTextWidth(gr, SRVertPitch);
			float topMarginStringWidth = GetTextWidth(gr, SRTopMargin);
			float topMarginStringHeight = GetTextHeight(gr, SRTopMargin);
			float sideMarginsStringHeight = GetTextHeight(gr, SRSideMargins);
			gr.DrawLine(doubleArrowPen, labelInfo.LeftMargin, -0.5f * hPitchStringHeight, labelInfo.LeftMargin + labelInfo.HPitch, -0.5f * hPitchStringHeight);
			DrawString(gr, SRHorizPitch, new RectangleF(labelInfo.LeftMargin, -hPitchStringHeight, labelInfo.HPitch, hPitchStringHeight),
				StringAlignment.Center, StringAlignment.Center);
			gr.DrawLine(doubleArrowPen, labelInfo.LeftMargin + 0.7f * labelInfo.LabelWidth, labelInfo.TopMargin, labelInfo.LeftMargin + 0.7f * labelInfo.LabelWidth, labelInfo.TopMargin + labelInfo.LabelHeight);
			gr.DrawLine(doubleArrowPen, labelInfo.LeftMargin, labelInfo.TopMargin + widthStringHeight, labelInfo.LeftMargin + labelInfo.LabelWidth, labelInfo.TopMargin + widthStringHeight);
			DrawString(gr, SRWidth, new RectangleF(labelInfo.LeftMargin, labelInfo.TopMargin + widthStringHeight / 2, 0.7f * labelInfo.LabelWidth, widthStringHeight),
				StringAlignment.Center, StringAlignment.Center);
			DrawString(gr, SRHeight, new RectangleF(labelInfo.LeftMargin + 0.7f * labelInfo.LabelWidth - heightStringWidth / 2, labelInfo.TopMargin, heightStringWidth, labelInfo.LabelHeight),
				StringAlignment.Center, StringAlignment.Center);
			gr.DrawLine(doubleArrowPen, -0.7f * hPitchStringHeight,  labelInfo.TopMargin, -0.7f * hPitchStringHeight, labelInfo.TopMargin + labelInfo.VPitch);
			DrawString(gr, SRVertPitch, new RectangleF(-vPitchStringWidth, labelInfo.TopMargin, vPitchStringWidth, labelInfo.VPitch),
				StringAlignment.Center, StringAlignment.Center);
			DrawString(gr, SRTopMargin, new RectangleF(-topMarginStringWidth, -topMarginStringHeight, topMarginStringWidth, topMarginStringHeight),
				StringAlignment.Near, StringAlignment.Center);
			gr.DrawLine(arrowPen, labelInfo.LeftMargin / 2, -1.5f * hPitchStringHeight, labelInfo.LeftMargin / 2, -0.5f * hPitchStringHeight);
			gr.DrawLine(arrowPen, totalWidth + hPitchStringHeight, 0, totalWidth + hPitchStringHeight, totalHeight);
			DrawString(gr, SRNumberDown, new RectangleF(totalWidth, 0, GetTextWidth(gr, SRNumberDown), totalHeight),
				StringAlignment.Near, StringAlignment.Center);
			gr.DrawLine(arrowPen, 0, totalHeight + 0.5f * hPitchStringHeight, totalWidth, totalHeight + 0.5f * hPitchStringHeight);
			DrawString(gr, SRNumberAcross, new RectangleF(0, totalHeight, totalWidth, hPitchStringHeight),
				StringAlignment.Center, StringAlignment.Center);
			DrawString(gr, SRSideMargins, new RectangleF(0, -1.5f * hPitchStringHeight - sideMarginsStringHeight, GetTextWidth(gr, SRSideMargins), sideMarginsStringHeight),
				StringAlignment.Near, StringAlignment.Near);
			gr.DrawLine(blackPen, 0, -0.1f * hPitchStringHeight, 0, -hPitchStringHeight);
			gr.DrawLine(blackPen, labelInfo.LeftMargin, 0.9f * labelInfo.TopMargin, labelInfo.LeftMargin, -hPitchStringHeight);
			gr.DrawLine(blackPen, labelInfo.LeftMargin + labelInfo.HPitch, 0.9f * labelInfo.TopMargin, labelInfo.LeftMargin + labelInfo.HPitch, -hPitchStringHeight);
			gr.DrawLine(blackPen, 0.9f * labelInfo.LeftMargin, labelInfo.TopMargin, -hPitchStringHeight, labelInfo.TopMargin);
			gr.DrawLine(blackPen, 0.9f * labelInfo.LeftMargin, labelInfo.TopMargin + labelInfo.VPitch, -hPitchStringHeight, labelInfo.TopMargin + labelInfo.VPitch);
			gr.DrawLine(blackPen, -0.1f * hPitchStringHeight, 0, -hPitchStringHeight, 0);
			gr.DrawLine(blackPen, -1.4f * hPitchStringHeight, 0, -1.4f * hPitchStringHeight, labelInfo.TopMargin / 2);
			gr.DrawLine(arrowPen, -1.4f * hPitchStringHeight, labelInfo.TopMargin / 2, -0.5f * hPitchStringHeight, labelInfo.TopMargin / 2);
			DrawLabelSamples(gr, totalWidth, totalHeight, false);
			DrawPageSample(gr, totalWidth, totalHeight, false);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(labelInfo == null)
				return;
			float totalWidth = labelInfo.LeftMargin + labelInfo.HPitch + labelInfo.LabelWidth / 3;
			float totalHeight = labelInfo.TopMargin + labelInfo.VPitch + labelInfo.LabelHeight / 3;
			if (totalWidth <= 0) return;
			if (totalHeight <= 0) return;
			Graphics gr = e.Graphics;
			float textWidth = Math.Max(GetTextWidth(gr, SRTopMargin), GetTextWidth(gr, SRVertPitch)) +
				GetTextWidth(gr, SRNumberDown);
			float textHeight = 2 * GetTextHeight(gr, SRHorizPitch) + GetTextHeight(gr, SRNumberAcross) +
				GetTextHeight(gr, SRSideMargins);
			float clientWidth = 0.95f * ClientRectangle.Width - textWidth;
			float clientHeight = 0.95f * ClientRectangle.Height - textHeight;
			if (clientWidth <= 0) return;
			if (clientHeight <= 0) return;
			float scale = Math.Min(clientWidth / totalWidth, clientHeight / totalHeight);
			gr.PageScale = scale;
			int x = (int)((ClientRectangle.Width / scale - totalWidth) / 2);
			int y = (int)((ClientRectangle.Height / scale - totalHeight) / 2);
			gr.TranslateTransform(x, y);
			DrawPreview(gr, totalWidth, totalHeight);
		}
	}
}
