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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.About;
using DevExpress.Utils.Text;
using DevExpress.Accessibility;
using Microsoft.Win32;
namespace DevExpress.Utils {
	public class AboutForm12 : XtraForm {
		int topIndent = 16;
		int indent = 19;
		int productInfoHeight = 176;
		int copyrightHeight = 57;
		int linkIndent = 40;
		int linkHeight = 35;
		int labelIndent1 = 15;
		int labelIndent2 = 17;
		Color borderColor = Color.FromArgb(164, 164, 164);
		internal static Color logoBackColor1 = Color.FromArgb(247, 148, 30);
		internal static Color logoBackColor2 = Color.FromArgb(60, 60, 60);
		Color copyrightColorBackground = Color.FromArgb(238, 238, 238);
		Color linkColor = Color.FromArgb(247,129,25);
		Color linkColorHover = Color.FromArgb(123, 65, 12);
		Color foreColor = Color.White;
		bool formMoving = false;
		int MouseDownX, MouseDownY;
		Rectangle productInfoRect = Rectangle.Empty;
		Image logo;
		internal static Font linkFont = new Font("Segoe UI", 12, FontStyle.Underline);
		internal static Font textFont = new Font("Segoe UI", 9);
		static Font productNameFont = new Font("Segoe UI Light", 30);
		static Font productPlatformFont = new Font("Segoe UI", 10);
		const string copyrightText = "Copyright © 2000-{0} Developer Express Inc.\r\nAll rights reserved";
		string supportCenter = AccLocalizer.Active.GetLocalizedString(AccStringId.AboutSupportCenter);
		string chatOnline = AccLocalizer.Active.GetLocalizedString(AccStringId.AboutChart);
		protected ProductInfo info;
		double heightLargeFontFactor = 1;
		public AboutForm12(ProductInfo info) {
			this.info = info;
			InitializeComponent();
			heightLargeFontFactor = GetLargeFontFactor();
			if(!IsTrial) {
				int registrationHeight = 422;
				this.Height = registrationHeight;
			}
			if(heightLargeFontFactor > 1) {
				this.Height = (int)(this.Height * heightLargeFontFactor);
				this.Width = (int)(this.Width * (heightLargeFontFactor + 0.1));
				copyrightHeight = (int)(copyrightHeight *heightLargeFontFactor);
			}
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
			this.Icon = DevExpress.Utils.ResourceImageHelper.CreateIconFromResources("DevExpress.Utils.Images.DX.ico", typeof(AboutForm12).Assembly);
			productInfoRect = new Rectangle(ClientRectangle.X + indent, ClientRectangle.Y + topIndent, ClientRectangle.Width - indent * 2, productInfoHeight);
			Text = "About DevExpress";
		}
		double GetLargeFontFactor() {
			if(!DevExpress.Utils.ScaleUtils.IsLargeFonts) {
				double res = (double)DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(100, 100)).Height / 100;
				if (res >= 1.99) res = 1.7;
				else if (res >= 1.4) res = 1.2;
				else if(res >= 1.2) res = 1.05;
				else res = 1;
				return res;
			}
			return 1;
		}
		protected virtual bool AllowMouseEvents { get { return this.Parent == null; } }
		protected override void OnShown(EventArgs e) {
			logo = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.DevExpress-Logo.png", typeof(AboutForm12).Assembly);
			if(AllowMouseEvents) {
				CloseButton button = new CloseButton(this);
				button.Parent = this;
				button.Location = new Point(ClientRectangle.Width - button.standard.Width - 3, ClientRectangle.Y + 2);
			} else FormBorderEffect = XtraEditors.FormBorderEffect.None;
			CreateLinks();
			CreateLabels();
			base.OnShown(e);
		}
		protected virtual void CreateLabels() {
			LabelHtmlText info = new LabelHtmlText();
			info.Text = GetInfoLabelText();
			info.Bounds = new Rectangle(productInfoRect.X, productInfoRect.Bottom + labelIndent1,
				productInfoRect.Width, info.AppearanceFontHeight);
			info.Parent = this;
			info.Calc();
			if(IsTrial) return;
			LabelHtmlText question = new LabelHtmlText();
			question.Text = AccLocalizer.Active.GetLocalizedString(AccStringId.AboutQuestions);
			question.Bounds = new Rectangle(productInfoRect.X, productInfoRect.Bottom + linkIndent + linkHeight + labelIndent2,
				productInfoRect.Width, question.AppearanceFontHeight);
			question.Parent = this;
			question.Calc();
			LabelInfo labelQuestion = new LabelInfo();
			labelQuestion.Parent = this;
			labelQuestion.Font = textFont;
			labelQuestion.BackColor = Color.Transparent;
			labelQuestion.TextAlign = ContentAlignment.MiddleCenter;
			if(!LocalizationHelper.IsJapanese)
				labelQuestion.Texts.Add(string.Format("{0} ", AccLocalizer.Active.GetLocalizedString(AccStringId.AboutVisit)), Color.Black);
			labelQuestion.Texts.Add(supportCenter, LogoBackColor1, true);
			labelQuestion.Texts.Add(string.Format(" {0} ", AccLocalizer.Active.GetLocalizedString(AccStringId.AboutOr)), Color.Black);
			labelQuestion.Texts.Add(chatOnline, LogoBackColor1, true);
			if(LocalizationHelper.IsJapanese)
				labelQuestion.Texts.Add(string.Format(" {0}", AccLocalizer.Active.GetLocalizedString(AccStringId.AboutVisit)), Color.Black);
			GraphicsInfo.Default.AddGraphics(null);
			try {
				int width = (int)GraphicsInfo.Default.Graphics.MeasureString(labelQuestion.Text, textFont).Width;
				labelQuestion.Bounds = new Rectangle(productInfoRect.X + (productInfoRect.Width - width) / 2, (int)(question.Bounds.Y + textFont.Height * 1.1),
					productInfoRect.Width, textFont.Height);
			} finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
			labelQuestion.ItemClick += new LabelInfoItemClickEvent(OnLabelClick);
		}
		protected internal virtual string GetInfoLabelText() {
			if(IsTrial)
				return string.Format(AccLocalizer.Active.GetLocalizedString(AccStringId.AboutSubscription), GetVersionLabel()) +
					(GetDays() > 0 ? " " + string.Format(AccLocalizer.Active.GetLocalizedString(AccStringId.AboutSubscriptionExp), GetDays()) : "");
			else return AccLocalizer.Active.GetLocalizedString(AccStringId.AboutRegistrationCode);
		}
		string GetVersionLabel() {
			if(IsExpired) return AccLocalizer.Active.GetLocalizedString(AccStringId.AboutExpiredVersion);
			return AccLocalizer.Active.GetLocalizedString(AccStringId.AboutTrialVersion);
		}
		void OnLabelClick(object sender, LabelInfoItemClickEventArgs e) {
			BaseButton.ProcessStart(GetProcessName(e));
		}
		string GetProcessName(LabelInfoItemClickEventArgs e) {
			if(e.InfoText.Text == supportCenter) return AssemblyInfo.DXLinkGetSupport;
			if(e.InfoText.Text == chatOnline) return AssemblyInfo.DXLinkChat;
			return null;
		}
		protected virtual void CreateLinks() {
			if(IsTrial) {
				int buttonWidth = (ClientRectangle.Width - indent * 2) / 3;
				int buttonTop = ClientRectangle.Y + topIndent + productInfoHeight + linkHeight + textFont.Height + labelIndent1;
				int middleButtonWidth = ClientRectangle.Width - indent * 2 - buttonWidth * 2 - 2;
				BaseLabelButton btn1 = new SupportLableButton(this, buttonWidth, new Point(ClientRectangle.X + indent, buttonTop));
				BaseLabelButton btn2 = new BuyLableButton(this, middleButtonWidth, new Point(ClientRectangle.X + indent + buttonWidth + 1, buttonTop));
				BaseLabelButton btn3 = new DiscountLableButton(this, buttonWidth, new Point(ClientRectangle.Right - indent - buttonWidth, buttonTop));
				RegisterButton rButton = new RegisterButton(this, new Size(middleButtonWidth, linkHeight));
				rButton.Location = new Point(ClientRectangle.X + indent + buttonWidth + 1, buttonTop - linkHeight); 
				rButton.Parent = this;
			} else {
				LabelHtmlText info = new LabelHtmlText();
				info.SetLargeFont();
				info.Text = Serial;
				info.MaxWidth = ClientRectangle.Width - indent * 2;
				info.Bounds = new Rectangle(productInfoRect.X, productInfoRect.Bottom + linkIndent,
					productInfoRect.Width, info.AppearanceFontHeight);
				info.Parent = this;
				info.Calc();
				info.Click += new EventHandler(info_Click);
			}
		}
		ContextMenu serialMenu;
		protected ContextMenu SerialMenu {
			get {
				if(serialMenu == null) serialMenu = new ContextMenu(new MenuItem[] { new MenuItem(AccLocalizer.Active.GetLocalizedString(AccStringId.AboutCopyToClipboard), new EventHandler(OnMenuClick)) });
				return serialMenu;
			}
			set {
				if(serialMenu != null) serialMenu.Dispose();
				serialMenu = null;
			}
		}
		void OnMenuClick(object sender, EventArgs e) {
			if(!IsTrial) Clipboard.SetDataObject(Serial);
		}
		void info_Click(object sender, EventArgs e) {
			SerialMenu.Show(this, PointToClient(Control.MousePosition));
		}
		protected override void OnPaint(PaintEventArgs e) {
			Rectangle clientRect = ClientRectangle;
			DrawContent(e.Graphics, clientRect);
			DrawProductInfo(e.Graphics, productInfoRect);
			DrawCopyright(e.Graphics, new Rectangle(productInfoRect.X, clientRect.Bottom - indent - copyrightHeight, productInfoRect.Width, copyrightHeight));
			base.OnPaint(e);
		}
		string CopyrightText { get { return string.Format(copyrightText, DateTime.Now.Year); } }
		void DrawCopyright(Graphics graphics, Rectangle rect) {
			using(Brush brush = new SolidBrush(copyrightColorBackground))
				graphics.FillRectangle(brush, rect);
			if(logo != null)
				graphics.DrawImage(logo, rect.Right - logo.Width, rect.Bottom - logo.Height, logo.Width, logo.Height);
			rect.Inflate(-indent, 0);
			using(StringFormat sf = new StringFormat()) {
				sf.LineAlignment = StringAlignment.Center;
				graphics.DrawString(CopyrightText, textFont, Brushes.Black, rect, sf);
			}
		}
		protected internal virtual Image LabelImage {
			get {
				if(IsCustom) return AboutHelper.EmptyImage;
				if(!IsTrial) return Properties.Resources.Licensed;
				if(IsExpired) return Properties.Resources.Expired;
				return Properties.Resources.TrialVersion;
			}
		}
		protected internal Rectangle ProductInfoRect {
			get { return productInfoRect; }
		}
		protected virtual Color LogoBackColor1 {
			get { return logoBackColor1; }
		}
		protected virtual Color LogoBackColor2 {
			get { return logoBackColor2; }
		}
		void DrawProductInfo(Graphics graphics, Rectangle rect) {
			using(Brush brush = new SolidBrush(LogoBackColor1))
				graphics.FillRectangle(brush, rect);
			graphics.DrawImage(LabelImage, productInfoRect.Right - LabelImage.Width, productInfoRect.Y + indent, LabelImage.Width, LabelImage.Height);
			rect.Inflate(-indent, -indent);
			using(StringFormat sf = new StringFormat()) {
				sf.Alignment = StringAlignment.Far;
				sf.LineAlignment = StringAlignment.Far;
				graphics.DrawString(AssemblyInfo.Version, productPlatformFont, Brushes.White, rect, sf);
			}
			ProductStringInfo sInfo = GetProductInfo(info);
			using(StringFormat sf = new StringFormat()) {
				sf.LineAlignment = StringAlignment.Far;
				if(!string.IsNullOrEmpty(sInfo.ProductName)) {
					graphics.DrawString(sInfo.ProductName, productPlatformFont, Brushes.White, rect, sf);
					rect.Inflate(0, -productPlatformFont.Height);
				}
				rect.Inflate(5, 10);
				graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				if(!string.IsNullOrEmpty(sInfo.ProductPlatform))
					graphics.DrawString(sInfo.ProductPlatform, productNameFont, Brushes.White, rect, sf);
			}
		}
		protected internal virtual ProductStringInfo GetProductInfo(ProductInfo info) {
			return info.StringInfo;
		}
		void DrawContent(Graphics graphics, Rectangle rect) {
			rect.Width--;
			rect.Height--;
			using(Pen pen = new Pen(borderColor))
				graphics.DrawRectangle(pen, rect);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyData == Keys.Escape) {
				e.Handled = true;
				Close();
			}
			base.OnKeyDown(e);
		}
		#region From Moving
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left && productInfoRect.Contains(e.Location)) StartMoving(e.X, e.Y);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Left) StopMoving();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			MoveTo();
		}
		internal void StartMoving(int x, int y) {
			if(!AllowMouseEvents) return;
			formMoving = true;
			MouseDownX = x;
			MouseDownY = y;
		}
		internal void StopMoving() {
			formMoving = false;
		}
		internal void MoveTo() {
			if(formMoving)
				this.Location = new Point(Cursor.Position.X - MouseDownX, Cursor.Position.Y - MouseDownY);
		}
		#endregion
		#region Buttons
		class BaseLabelButton : Label {
			public event EventHandler ButtonClick;
			AppearanceObject appearance;
			StringInfo info = null;
			internal Image normal = null, hover = null, pressed = null;
			string caption = string.Empty, processStartLink = string.Empty;
			public BaseLabelButton(Control parent, int width, Point location) {
				CreateResources();
				this.BackColor = Color.Transparent;
				appearance = new AppearanceObject();
				appearance.Font = new Font("Segoe UI", 9);
				appearance.ForeColor = Color.Black;
				appearance.TextOptions.HAlignment = HorzAlignment.Center;
				appearance.TextOptions.VAlignment = VertAlignment.Bottom;
				appearance.TextOptions.WordWrap = WordWrap.Wrap;
				this.Size = new Size(width, appearance.FontHeight * 4 + normal.Height);
				this.Location = location;
				this.Parent = parent;
				SetActive(normal, false);
			}
			public string Caption { get; set; }
			public string ProcessStartLink { get; set; }
			protected virtual void CreateResources() { }
			protected override void OnMouseEnter(EventArgs e) {
				base.OnMouseEnter(e);
				SetActive(Active, true);
			}
			protected override void OnMouseLeave(EventArgs e) {
				base.OnMouseLeave(e);
				SetActive(normal, false);
			}
			protected override void OnMouseDown(MouseEventArgs e) {
				base.OnMouseDown(e);
				if(e.Button != MouseButtons.Left) return;
				SetActive(Pressed, true);
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				base.OnMouseUp(e);
				if(e.Button != MouseButtons.Left) return;
				if(new Rectangle(0, 0, this.Width, this.Height).Contains(new Point(e.X, e.Y))) {
					SetActive(Active, true);
					RaiseButtonClick();
				} else {
					SetActive(normal, false);
				}
			}
			protected override void OnResize(EventArgs e) {
				base.OnResize(e);
				UpdateInfo();
			}
			Image Active { get { return hover == null ? normal : hover; } }
			Image Pressed { get { return pressed == null ? Active : pressed; } }
			void SetActive(Image image, bool active) {
				Image = image;
				Text = string.Format("{1}{0}", Caption, active ? "<u>" : string.Empty);
				UpdateInfo();
			}
			protected virtual void RaiseButtonClick() {
				if(ButtonClick != null)
					ButtonClick(this, EventArgs.Empty);
			}
			public void UpdateInfo() {
				using(Graphics g = Graphics.FromHwnd(Handle))
					CalcInfo(g);
			}
			protected override void OnPaint(PaintEventArgs e) {
				if(info == null) CalcInfo(e.Graphics);
				GraphicsCache cache = new GraphicsCache(e);
				StringPainter.Default.DrawString(cache, info);
				if(Image != null)
					e.Graphics.DrawImage(Image, (Width - Image.Width) / 2, 10, Image.Width, Image.Height);
			}
			void CalcInfo(Graphics g) {
				info = StringPainter.Default.Calculate(g, appearance, Text, ClientRectangle);
			}
		}
		class BaseButton : PictureBox {
			public event EventHandler ButtonClick;
			internal Image standard = null, active = null;
			protected Size imageSize = Size.Empty;
			AboutForm12 aboutForm;
			public BaseButton(AboutForm12 aboutForm) : this(aboutForm, Size.Empty) { }
			public BaseButton(AboutForm12 aboutForm, Size size) {
				this.aboutForm = aboutForm;
				this.imageSize = size;
				this.BackColor = Color.Transparent;
				this.SizeMode = PictureBoxSizeMode.AutoSize;
				CreateImages();
				this.Image = standard;
			}
			protected virtual void CreateImages() { }
			[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public new Image Image {
				get { return base.Image; }
				set { base.Image = value; }
			}
			protected AboutForm12 AboutForm { get { return aboutForm; } }
			protected override void OnMouseEnter(EventArgs e) {
				base.OnMouseEnter(e);
				this.Image = active;
			}
			protected override void OnMouseLeave(EventArgs e) {
				base.OnMouseLeave(e);
				this.Image = standard;
			}
			protected override void OnMouseDown(MouseEventArgs e) {
				base.OnMouseDown(e);
				if(e.Button != MouseButtons.Left) return;
				this.Image = active;
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				base.OnMouseUp(e);
				if(e.Button != MouseButtons.Left) return;
				if(new Rectangle(0, 0, this.Width, this.Height).Contains(new Point(e.X, e.Y))) {
					this.Image = active;
					RaiseButtonClick();
				} else {
					this.Image = standard;
				}
			}
			protected virtual void RaiseButtonClick() {
				if(ButtonClick != null)
					ButtonClick(this, EventArgs.Empty);
			}
			protected void CreateImages(string text) {
				standard = CreateTextImage(text, imageSize.Width, imageSize.Height, Color.White, AboutForm.linkColor, AboutForm12.linkFont);
				active = CreateTextImage(text, imageSize.Width, imageSize.Height, Color.White, AboutForm.linkColorHover, AboutForm12.linkFont);
			}
			protected Image CreateTextImage(string text, int width, int height, Color backColor, Color foreColor, Font font) {
				Image ret = new Bitmap(width, height);
				Rectangle rect = new Rectangle(0, 0, width, height);
				using(Graphics g = Graphics.FromImage(ret)) {
					using(Brush brush = new SolidBrush(backColor))
						g.FillRectangle(brush, rect);
					using(Brush brush = new SolidBrush(foreColor)) {
						using(StringFormat sf = new StringFormat()) {
							sf.LineAlignment = StringAlignment.Center;
							sf.Alignment = StringAlignment.Center;
							sf.FormatFlags |= StringFormatFlags.NoWrap;
							g.DrawString(text, 
								GetDPIFont(g, text, font, rect.Width), 
								brush, rect, sf);
						}
					}
				}
				return ret;
			}
			Font GetDPIFont(Graphics g, string text, Font parentFont, int width) {
				const int lastSize = 5;
				SizeF size = g.MeasureString(text, parentFont);
				if(size.Width <= width) return parentFont;
				for(int i = (int)parentFont.Size - 1; i > lastSize; i--) {
					Font font = new Font(parentFont.FontFamily, i);
					size = g.MeasureString(text,  font);
					if(size.Width <= width || i == lastSize) return font;
				}
				return parentFont;
			}
			public static void ProcessStart(string name) {
				ProcessStart(name, string.Empty);
			}
			public static void ProcessStart(string name, string arguments) {
				try {
					var process = new System.Diagnostics.Process();
					process.StartInfo.FileName = name;
					process.StartInfo.Arguments = arguments;
					process.StartInfo.Verb = "Open";
					process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
					process.Start();
				}
				catch { }
			}
		}
		class CloseButton : BaseButton {
			public CloseButton(AboutForm12 aboutForm) : base(aboutForm) { }
			protected override void CreateImages() {
				standard = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.Close-Button.png", typeof(AboutForm12).Assembly);
				active = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.Close-Button-Active.png", typeof(AboutForm12).Assembly);
			}
			protected override void RaiseButtonClick() {
				((Form)this.Parent).Close();
			}
		}
		class RegisterButton : BaseButton {
			public RegisterButton(AboutForm12 aboutForm, Size size) : base(aboutForm, size) { }
			protected override void CreateImages() {
				this.Cursor = Cursors.Hand;
				CreateImages(AccLocalizer.Active.GetLocalizedString(AccStringId.AboutRegisterYourProduct));
			}
			protected override void RaiseButtonClick() {
				RegisterClick();
			}
			public static void RegisterClick() {
				string setupFilePath = CalcSetupFilePath(AssemblyInfo.InstallationRegistryKey, "SetupFilePath");
				bool showSite = false;
				if(setupFilePath == string.Empty)
					showSite = true;
				else {
					try {
						ProcessStart(setupFilePath, "/Register");
					} catch {
						showSite = true;
					}
				}
				if(showSite)
					ProcessStart(AssemblyInfo.DXLinkTrial);
			}
			static string CalcSetupFilePath(string keyPath, string keyName) {
				string ret = GetSetupFilePath(keyPath, keyName);
				if(ret == string.Empty) ret = GetSetupFilePath(keyPath.Replace("SOFTWARE", "SOFTWARE\\Wow6432Node"), keyName);
				return ret;
			}
			static string GetSetupFilePath(string keyPath, string keyName) {
				RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath);
				if(key != null) {
					string keyValue = string.Format("{0}", key.GetValue(keyName));
					if(System.IO.File.Exists(keyValue)) return keyValue;
				}
				return string.Empty;
			}
		}
		class DiscountLableButton : BaseLabelButton {
			public DiscountLableButton(Control parent, int width, Point point) : base(parent, width, point) { }
			protected override void CreateResources() {
				normal = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.About-Discount.png", typeof(AboutForm12).Assembly);
				Caption = Caption = "<b>Competitive Discounts</b></u><br>Switch and save.<br>Discounts are available.";
			}
			protected override void RaiseButtonClick() {
				BaseButton.ProcessStart(AssemblyInfo.DXLinkCompetitiveDiscounts);
			}
		}
		class SupportLableButton : BaseLabelButton {
			public SupportLableButton(Control parent, int width, Point point) : base(parent, width, point) { }
			protected override void CreateResources() {
				normal = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.About-Support.png", typeof(AboutForm12).Assembly);
				Caption = Caption = "<b>Get Free Support</b></u><br>30 days of free support<br>during your trial.";
			}
			protected override void RaiseButtonClick() {
				BaseButton.ProcessStart(AssemblyInfo.DXLinkGetSupport);
			}
		}
		class BuyLableButton : BaseLabelButton {
			public BuyLableButton(Control parent, int width, Point point) : base(parent, width, point) { }
			protected override void CreateResources() {
				normal = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.About-Buy.png", typeof(AboutForm12).Assembly);
				Caption = Caption = "<b>Buy Now</b></u><br>Prices start at only $899.99<br>for a 1 year subscription.";
			}
			protected override void RaiseButtonClick() {
				BaseButton.ProcessStart(AssemblyInfo.DXLinkBuyNow);
			}
		}
		#endregion
		[ToolboxItem(false)]
		protected class LabelHtmlText : Label {
			int maxWidth = 0;
			AppearanceObject appearance;
			StringInfo info = null;
			public LabelHtmlText() {
				appearance = new AppearanceObject();
				appearance.Font = new Font("Segoe UI", 9);
				appearance.ForeColor = Color.Black;
				appearance.TextOptions.HAlignment = HorzAlignment.Center;
			}
			public void SetLargeFont() { 
				appearance.Font = new Font("Segoe UI", 16);
			}
			public void Calc() {
				using(Graphics g = Graphics.FromHwnd(Handle)) 
					CalcInfo(g);
			}
			[Obsolete("Use AppearanceFontHeight"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public int AppearenceFontHeight { get { return AppearanceFontHeight; } }
			public int AppearanceFontHeight { get { return appearance.Font.Height; } }
			protected override void OnPaint(PaintEventArgs e) {
				if(info == null) CalcInfo(e.Graphics);
				GraphicsCache cache = new GraphicsCache(e);
				StringPainter.Default.DrawString(cache, info);
			}
			void CalcInfo(Graphics g) {
				info = StringPainter.Default.Calculate(g, appearance, Text, ClientRectangle);
				if(MaxWidth > 0) {
					while(info.Bounds.Width >= MaxWidth) {
						appearance.Font = new Font(appearance.Font.FontFamily, appearance.Font.Size - 1);
						info = StringPainter.Default.Calculate(g, appearance, Text, ClientRectangle);
						if(appearance.Font.Size < 10) break;
					}
				}
			}
			public int MaxWidth {
				get { return maxWidth; }
				set { maxWidth = value; }
			}
		}
		protected virtual string Serial { get { return AboutHelper.GetSerial(info); } }
		protected bool IsTrial { get { return Serial == AboutHelper.TrialVersion; } }
		protected bool IsCustom { get { return Serial == AboutHelper.CustomVersion; } }
		internal bool IsExpired {
			get {
				bool res = false;
				return res;
			}
		}
		int GetDays() {
			return 0;
		}
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.SuspendLayout();
			this.Appearance.BackColor = System.Drawing.Color.White;
			this.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
			this.Appearance.Options.UseBackColor = true;
			this.Appearance.Options.UseFont = true;
			this.ClientSize = new System.Drawing.Size(600, 502);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.LookAndFeel.UseWindowsXPTheme = true;
			this.Name = "AboutForm12";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);
		}
		#endregion
	}
}
