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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Frames {
	[ToolboxItem(false)]
	public partial class LinkPanel : XtraUserControl {
		const int TextToImageInterval = 10;
		const string HrefPattern = "<a[^>]*? href \\s*=\\s* \"([^\"]+)\"[^>]*?>([\\s\\S]*?)<\\/a>";
		LinkLabel llText;
		Image dxLogoCore;
		public LinkPanel() {
			InitializeLinkLabel();
			dxLogoCore = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.XtraFrames.dx-logo.png", typeof(LinkPanel).Assembly);
			Font = new Font("Segoe UI", 12);
			ForeColor = Color.FromArgb(60, 60, 60);
			MaxRows = 2;
			DoubleBuffered = true;
		}
		public virtual Image DXLogo {
			get { return dxLogoCore; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected Rectangle ImageBounds { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected Rectangle TextBounds { get; set; }
		[DefaultValue(2)]
		public int MaxRows { get; set; }
		protected void InitializeLinkLabel() {
			llText = new LinkLabel();
			llText.AutoSize = false;
			llText.AutoEllipsis = true;
			llText.LinkColor = Color.FromArgb(247, 148, 30);
			llText.ActiveLinkColor = Color.FromArgb(247, 148, 30);
			llText.LinkClicked += OnLinkClicked;
			Controls.Add(llText);
			CalcTextForLabel(Text);
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			CalcTextForLabel(Text);
		}
		void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if(e.Link != null && e.Link.Tag != null)
				StartProccess(e.Link.Tag.ToString());
		}
		protected static void StartProccess(string fileName) {
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = fileName;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			SendToBack();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Invalidate();
			SetDirty();
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			CaclElements(e.Graphics);
			DrawDXLogo(e);
		}
		void DrawDXLogo(PaintEventArgs e) {
			e.Graphics.DrawImage(DXLogo, ImageBounds);
		}
		void SetDirty() {
			isReadyCore = false;
		}
		bool isReadyCore;
		protected bool IsReady { get { return isReadyCore; } }
		protected Rectangle CalcClientRectangle(Rectangle bounds) {
			return new Rectangle(
				bounds.Left + Padding.Left,
				bounds.Top + Padding.Top,
				bounds.Width - Padding.Horizontal,
				bounds.Height - Padding.Vertical);
		}
		protected virtual void CaclElements(Graphics graphics) {
			if(IsReady) return;
			ImageBounds = Rectangle.Empty;
			TextBounds = Rectangle.Empty;
			Size imageSize = DXLogo.Size;
			SizeF size = graphics.MeasureString(llText.Text, Font, Width - imageSize.Width);
			int textHeight = Math.Min(Math.Max((int)size.Height, Font.Height), MaxHeight(Font.Height));
			Size contentSize = new Size(Width - Padding.Horizontal, Math.Max(textHeight, imageSize.Height));
			Height = contentSize.Height + Padding.Vertical;
			Rectangle contentRect = CalcClientRectangle(ClientRectangle);
			if(Width - Padding.Right >= imageSize.Width) {
				ImageBounds = new Rectangle(new Point(contentRect.Right - imageSize.Width, contentRect.Y + (contentSize.Height - imageSize.Height) / 2), imageSize);
			}
			if(contentRect.Width - imageSize.Width - TextToImageInterval > 0) {
				TextBounds = new Rectangle(new Point(contentRect.X, contentRect.Y + (contentSize.Height - textHeight) / 2), new Size(contentSize.Width - imageSize.Width - TextToImageInterval, textHeight));
			}
			llText.Bounds = TextBounds;
			isReadyCore = true;
		}
		protected int MaxHeight(int fontHeight) {
			return fontHeight * MaxRows;
		}
		public void CalcTextForLabel(string text) {
			if(string.IsNullOrEmpty(text)) return;
			Regex regex = new Regex(HrefPattern);
			for(Match match = regex.Match(text); match.Success; match = regex.Match(text)) {
				text = text.Remove(match.Index, match.Length);
				text = text.Insert(match.Index, match.Groups[2].Value);
				llText.Text = text;
				LinkLabel.Link link = llText.Links.Add(match.Index, match.Groups[2].Value.Length, match.Groups[2].Value);
				link.Tag = match.Groups[1].Value;
			}
			if(string.IsNullOrEmpty(llText.Text))
				llText.Text = text;
		}
		void InitializeComponent() {
			this.SuspendLayout();
			this.Name = "linkPanel";
			this.Size = new System.Drawing.Size(191, 165);
			this.ResumeLayout(false);
		}
	}
}
