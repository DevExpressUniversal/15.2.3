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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Text;
using DevExpress.Skins;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemHyperLinkEdit : RepositoryItemButtonEdit {
		protected Color fLinkColor;
		protected KeyShortcut fStartKey;
		protected const Keys defStartKey = Keys.Control | Keys.Enter; 
		protected string fCaption;
		Image image;
		HorzAlignment imageAlignment;
		bool singleClick, startLinkOnClickingEmptySpace, linkColorUseParentAppearance;
		ProcessWindowStyle browserWindowStyle;
		public RepositoryItemHyperLinkEdit() {
			this.fLinkColor = Color.Empty;
			this.fStartKey = new KeyShortcut(defStartKey);
			this.image = null;
			this.imageAlignment = HorzAlignment.Near;
			this.singleClick = DefaultSingleClick;
			this.startLinkOnClickingEmptySpace = true;
			this.linkColorUseParentAppearance = false;
			this.browserWindowStyle = ProcessWindowStyle.Normal;
			this.fCaption = string.Empty;
			base.fTextEditStyle = TextEditStyles.DisableTextEditor;
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			style.ForeColor = GetLinkColor();
			return style;
		}
		protected override void SetupTextBrickStyleProperties(PrintCellHelperInfo info, BrickStyle style) {
			base.SetupTextBrickStyleProperties(info, style);
			style.ForeColor = GetLinkColor();
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			HyperLinkEditViewInfo ceVi = PreparePrintViewInfo(info, true) as HyperLinkEditViewInfo;
			IPanelBrick panel = CreatePanelBrick(info, true, CreateBrickStyle(info, "panel"));
			ITextBrick textBrick = CreateTextBrick(info);
			panel.Url = textBrick.Url = info.EditValue == null ? string.Empty : info.EditValue.ToString();
			panel.Style = textBrick.Style;
			textBrick.Sides = XtraPrinting.BorderSide.None;
			textBrick.Rect = ceVi.Bounds;
			panel.Bricks.Add(textBrick);
			IImageBrick imageBrick = CreateImageBrick(info, CreateBrickStyle(info, "image"));
			imageBrick.Image = GetBrickImage(ceVi, info);
			imageBrick.Rect = ceVi.PictureRect;
			imageBrick.Sides = XtraPrinting.BorderSide.None;
			panel.Bricks.Add(imageBrick);
			return panel;
		}
		protected Image GetBrickImage(HyperLinkEditViewInfo ceVi, PrintCellHelperInfo info) {
			return ceVi.Item.Image;
		}
		public override void CreateDefaultButton() {
		}
		public override void Assign(RepositoryItem item) {
			BeginUpdate(); 
			try {
				base.Assign(item);
				RepositoryItemHyperLinkEdit source = item as RepositoryItemHyperLinkEdit;
				if(source == null) return;
				this.fLinkColor = source.LinkColor;
				this.linkColorUseParentAppearance = source.LinkColorUseParentAppearance;
				this.fStartKey = new KeyShortcut(source.StartKey.Key);
				this.fCaption = source.Caption;
				this.image = source.Image;
				this.imageAlignment = source.ImageAlignment;
				this.singleClick = source.SingleClick;
				this.startLinkOnClickingEmptySpace = source.StartLinkOnClickingEmptySpace;
				this.browserWindowStyle = source.BrowserWindowStyle;
				Events.AddHandler(openLink, source.Events[openLink]);
			}
			finally {
				EndUpdate();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemHyperLinkEdit Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "HyperLinkEdit"; } }
		bool ShouldSerializeReadOnly() {
			if(Caption != String.Empty) return false;
			return ReadOnly;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditReadOnly")
#else
	Description("")
#endif
]
		public override bool ReadOnly {
			get { return base.ReadOnly || Caption != string.Empty; } 
			set { base.ReadOnly = value; } 
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle { get { return base.TextEditStyle; } set { base.TextEditStyle = value; } }
		bool ShouldSerializeLinkColor() { return !LinkColor.IsEmpty; }
		void ResetLinkColor() { LinkColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditLinkColor")
#else
	Description("")
#endif
]
		public virtual Color LinkColor {
			get { return fLinkColor; }
			set {
				if(LinkColor == value) return;
				fLinkColor = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditLinkColorUseParentAppearance"),
#endif
 DefaultValue(false)]
		public virtual bool LinkColorUseParentAppearance {
			get { return linkColorUseParentAppearance; }
			set {
				if(linkColorUseParentAppearance == value) return;
				linkColorUseParentAppearance = value;
				OnPropertiesChanged();
			}
		}
		Color GetColor(Color color, Skin skin, string objectName) {
			if(!color.IsEmpty) return color;
			return skin.Colors.GetColor(objectName);
		}
		public Color GetLinkColor() {
			Color color = LinkColor;
			if(LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				Skin skin = EditorsSkins.GetSkin(LookAndFeel);
				color = GetColor(color, skin, EditorsSkins.SkinHyperlinkTextColor);
			}
			if(color.IsEmpty) color = Color.Blue;
			return color;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditBrowserWindowStyle"),
#endif
 DefaultValue(ProcessWindowStyle.Normal), SmartTagProperty("Browser Window Style", "")]
		public virtual ProcessWindowStyle BrowserWindowStyle {
			get { return browserWindowStyle; }
			set {
				if(BrowserWindowStyle == value) return;
				browserWindowStyle = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeStartKey() { return StartKey.Key != defStartKey; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditStartKey"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Editor("DevExpress.XtraEditors.Design.EditorButtonShortcutEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), SmartTagProperty("Start Key", "")]
		public virtual KeyShortcut StartKey { 
			get { return fStartKey; }
			set {
				if(StartKey == value) return;
				fStartKey = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditCaption"),
#endif
 DefaultValue(""), Localizable(true)]
		public virtual string Caption {
			get { return fCaption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				fCaption = value;
				OnPropertiesChanged();
			}
		}
		[Obsolete(ObsoleteText.SRObsoleteImage), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Image Picture {
			get { return Image; }
			set { Image = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditImage"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = ImageHelper.MakeTransparent(value);
				OnPropertiesChanged();
			}
		}
		[Obsolete(ObsoleteText.SRObsoleteImageAlignment), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public HorzAlignment GlyphAlignment {
			get { return ImageAlignment; }
			set { ImageAlignment = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditImageAlignment"),
#endif
 DefaultValue(HorzAlignment.Near)]
		public HorzAlignment ImageAlignment {
			get { return imageAlignment; }
			set {
				if(ImageAlignment == value) return;
				imageAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditStartLinkOnClickingEmptySpace"),
#endif
 DefaultValue(true)]
		public bool StartLinkOnClickingEmptySpace {
			get { return startLinkOnClickingEmptySpace; }
			set {
				if(StartLinkOnClickingEmptySpace == value) return;
				startLinkOnClickingEmptySpace = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeSingleClick() { return SingleClick != DefaultSingleClick; }
		protected internal virtual bool DefaultSingleClick { get { return OwnerEdit != null; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditSingleClick")
#else
	Description("")
#endif
]
		public bool SingleClick {
			get { return singleClick; }
			set {
				if(SingleClick == value) return;
				singleClick = value;
				OnPropertiesChanged();
			}
		}
		static object openLink = new object();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHyperLinkEditOpenLink"),
#endif
 DXCategory(CategoryName.Events)]
		public event OpenLinkEventHandler OpenLink {
			add { this.Events.AddHandler(openLink, value); }
			remove { this.Events.RemoveHandler(openLink, value); }
		}
		protected internal virtual void RaiseOpenLink(OpenLinkEventArgs e) {
			OpenLinkEventHandler handler = (OpenLinkEventHandler)this.Events[openLink];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(StartKey != null && key == StartKey.Key)
				return true;
			return base.NeededKeysContains(key);
		}
		protected internal override bool ActivationKeysContains(Keys key) {
			if(StartKey != null && key == StartKey.Key)
				return true;
			return base.ActivationKeysContains(key);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(Caption != string.Empty && !IsNullValue(editValue)) {
				editValue = Caption;
			}
			return base.GetDisplayText(format.FormatType == FormatType.None ? DisplayFormat : format, editValue);
		}
		protected internal override AppearanceObject GetOverrideAppearance() {
			return new AppearanceObject(new AppearanceDefault(GetLinkColor(), Color.Empty));
		}
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraEditors.Design.HyperLinkEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign), DefaultEvent("OpenLink"),
	 Description("Provides hyperlink functionality."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(HyperLinkEditFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "HyperLinkEdit")
	]
	public class HyperLinkEdit : ButtonEdit {
		bool leftPressed;
		public HyperLinkEdit() {
			this.leftPressed = false;
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "HyperLinkEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("HyperLinkEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemHyperLinkEdit Properties { get { return base.Properties as RepositoryItemHyperLinkEdit; } }
		protected internal new HyperLinkEditViewInfo ViewInfo { get { return base.ViewInfo as HyperLinkEditViewInfo; } }
		protected override void CreateMaskBox() {
			base.CreateMaskBox();
			if(MaskBox != null) MaskBox.Cursor = Cursors.Hand;
		}
		protected override void InitializeDefaultProperties() {
			base.InitializeDefaultProperties();
			Properties.BeginUpdate();
			try { Properties.SingleClick = true; }
			finally { Properties.CancelUpdate(); }
		}
		protected override void UpdateMaskBoxProperties(bool always) {
			base.UpdateMaskBoxProperties(always);
			if(ViewInfo.IsShowNullValuePrompt()) {
				MaskBox.Font = Properties.Appearance.Font;
			}
			else {
			MaskBox.Font = new Font(Properties.Appearance.Font, Properties.Appearance.Font.Style | FontStyle.Underline);
			if(InplaceType == InplaceType.Standalone && (always || MaskBox.ForeColor != Properties.GetLinkColor())) MaskBox.ForeColor = Properties.GetLinkColor();
		}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("HyperLinkEditText"),
#endif
 RefreshProperties(RefreshProperties.All), Bindable(true), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get {
				if(EditValue == null || EditValue is DBNull) return string.Empty;
				return EditValue.ToString(); 
			}
			set { EditValue = value; }
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			leftPressed = (e.Button & MouseButtons.Left) != 0;
			CheckMouseCursorShape(new Point(e.X, e.Y));
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			CheckMouseCursorShape(new Point(e.X, e.Y));
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(Properties.StartKey != null && Properties.StartKey.IsExist && e.KeyData == Properties.StartKey.Key)
				ShowBrowser();
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			if(Properties.SingleClick)
				DoClick();
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			if(!Properties.SingleClick)
				DoClick();
		}
		protected virtual void DoClick() {
			if(!leftPressed || Properties.TextEditStyle == TextEditStyles.HideTextEditor) return;
			Point pt = PointToClient(Control.MousePosition);
			if(ViewInfo.ButtonInfoByPoint(pt) != null) return;
			if(AllowStartLinkOnClickingEmptySpace(pt) || 
				ViewInfo.PictureRect.Contains(pt) || 
				(Properties.TextEditStyle != TextEditStyles.DisableTextEditor && ViewInfo.TextRectangle.Contains(pt)))
				ShowBrowser();
		}
		public void ShowBrowser() {
			ShowBrowser(EditValue);
		}
		public virtual void ShowBrowser(object linkValue) {
			OpenLinkEventArgs e = new OpenLinkEventArgs(linkValue);
			Properties.RaiseOpenLink(e);
			if(!CanShowBrowser(e)) return;
			Process process = new Process();
			process.StartInfo.FileName = (e.EditValue == null ? string.Empty : e.EditValue.ToString());
			process.StartInfo.WindowStyle = Properties.BrowserWindowStyle;
			try {
				process.Start();
			}
			catch {}
		}
		protected virtual bool CanShowBrowser(OpenLinkEventArgs e) {
			return !(e.Handled || e.EditValue == null || e.EditValue == DBNull.Value || Properties.TextEditStyle == TextEditStyles.HideTextEditor);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("HyperLinkEditOpenLink"),
#endif
 DXCategory(CategoryName.Events)]
		public event OpenLinkEventHandler OpenLink {
			add { Properties.OpenLink += value; }
			remove { Properties.OpenLink -= value; }
		}
		protected virtual void CheckMouseCursorShape(Point pt) {
			Cursor current = this.Cursor;
			if(AllowStartLinkOnClickingEmptySpace(pt) || ViewInfo.PictureRect.Contains(pt)) {
				current = Cursors.Hand;
			}
			else if(ViewInfo.MaskBoxRect.Contains(pt)) current = Cursors.Default;
			if(!IsDesignMode && Properties.TextEditStyle != TextEditStyles.Standard)
				this.Cursor = current;
		}
		protected virtual bool AllowStartLinkOnClickingEmptySpace(Point pt) {
			return Properties.TextEditStyle == TextEditStyles.DisableTextEditor &&
				(Properties.StartLinkOnClickingEmptySpace || ViewInfo.TextRectangle.Contains(pt));
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class HyperLinkEditViewInfo : ButtonEditViewInfo {
		Rectangle pictureRect, textRectangle;
		public HyperLinkEditViewInfo(RepositoryItem item) : base(item) {
		}
		public override Cursor GetMouseCursor(Point point) {
			if(!MaskBoxRect.IsEmpty || Item.ImageAlignment == HorzAlignment.Center) { 
				if(InplaceType == InplaceType.Standalone) {
					if(MaskBoxRect.Contains(point)) return Cursors.Hand;
				} else {
					if(GetTextMouseRectangle().Contains(point)) return Cursors.Hand;
				}
			}
			return base.GetMouseCursor(point);
		}
		Rectangle GetTextMouseRectangle() {
			if(Item.StartLinkOnClickingEmptySpace) return ContentRect;
			if(!TextRectangle.IsEmpty) return TextRectangle;
			if(string.IsNullOrEmpty(this.fDisplayText)) return Rectangle.Empty;
			Graphics g = GInfo.AddGraphics(null);
			try {
				textRectangle = CalcTextRectangle(g);
			} finally {
				GInfo.ReleaseGraphics();
			}
			return textRectangle;
		}
		protected virtual Rectangle CalcTextRectangle(Graphics g) {
			Rectangle textRect = Rectangle.Empty;
			Size textSize = CalcTextSizeCore(g, this.fDisplayText, MaskBoxRect.Width);
			textRect = new Rectangle(MaskBoxRect.X, MaskBoxRect.Y, textSize.Width, MaskBoxRect.Height);
			if(PaintAppearance.TextOptions.HAlignment == HorzAlignment.Far) {
				textRect.X = MaskBoxRect.X + MaskBoxRect.Width - textSize.Width;
			} else if(PaintAppearance.TextOptions.HAlignment == HorzAlignment.Center) {
				textRect.X = MaskBoxRect.X + (MaskBoxRect.Width - textSize.Width) / 2;
			}
			return textRect;
			}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			textRectangle = CalcTextRectangle(g);
		}
		public Rectangle TextRectangle { get { return textRectangle; } }
		protected override Size CalcContentSize(Graphics g) {
			Size textSize = base.CalcContentSize(g);
			if(Item.Image == null) return textSize;
			if(Item.ImageAlignment == HorzAlignment.Center) return Item.Image.Size;
			return new Size(Math.Max(textSize.Width, Item.Image.Size.Width + 2 * HorzImageIndent), Math.Max(textSize.Height, Item.Image.Size.Height + 2 * VertImageIndent));
		}
		public override void Offset(int x, int y) { 
			base.Offset(x, y);
			if(!PictureRect.IsEmpty) pictureRect.Offset(x, y);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			if(Item.TextEditStyle == TextEditStyles.HideTextEditor) return;
			if(Item.ImageAlignment == HorzAlignment.Center && Item.Image != null) {
				pictureRect = GetPictureRect(ContentRect, Item.ImageAlignment);
				fMaskBoxRect = Rectangle.Empty;
				return;
			}
			if(Item.Image == null || Item.Image.Size.Width + HorzImageIndent > ContentRect.Width) return;
			fMaskBoxRect.Width -= Item.Image.Size.Width + HorzImageIndent;
			if((Item.ImageAlignment == HorzAlignment.Near && RightToLeft) ||
			   (Item.ImageAlignment == HorzAlignment.Far && !RightToLeft)) {
				pictureRect = GetPictureRect(new Rectangle(MaskBoxRect.Right + HorzImageIndent, ContentRect.Top, Item.Image.Size.Width, ContentRect.Height), Item.ImageAlignment);
			}
			else {
				fMaskBoxRect.X += Item.Image.Size.Width + HorzImageIndent;
				pictureRect = GetPictureRect(new Rectangle(ContentRect.Location, new Size(Item.Image.Size.Width, ContentRect.Height)), Item.ImageAlignment);
			}
		}
		protected override bool CanFastRecalcViewInfo(Rectangle bounds, Point mousePosition) {
			bool res = base.CanFastRecalcViewInfo(bounds, mousePosition);
			return res && ((Item.Image != null && !PictureRect.Size.IsEmpty) || (Item.Image == null && PictureRect.Size.IsEmpty));
		}
		private Rectangle GetPictureRect(Rectangle bounds, HorzAlignment imageAlignment) {
			if(Item.Image == null) return Rectangle.Empty;
			int cx = 0, cy = 0;
			switch(PaintAppearance.GetTextOptions().VAlignment) {
				case VertAlignment.Center:
				case VertAlignment.Default:
					cx = (bounds.Width - Item.Image.Size.Width) / 2;
					cy = (bounds.Height - Item.Image.Size.Height) / 2;
					break;
				case VertAlignment.Bottom:
					cx = (imageAlignment == HorzAlignment.Center ? (bounds.Width - Item.Image.Size.Width) / 2 : bounds.Width - Item.Image.Size.Width);
					cy = bounds.Height - Item.Image.Size.Height;
					break;
				case VertAlignment.Top:
					if(imageAlignment == HorzAlignment.Center) cx = (bounds.Width - Item.Image.Size.Width) / 2;
					break;
			}
			return new Rectangle(bounds.Left + cx, bounds.Top + cy, Item.Image.Size.Width, Item.Image.Size.Height);
		}
		public override void Clear() {
			base.Clear();
			pictureRect = Rectangle.Empty;
			textRectangle = Rectangle.Empty;
		}
		public new RepositoryItemHyperLinkEdit Item { get { return base.Item as RepositoryItemHyperLinkEdit; } }
		public virtual int HorzImageIndent { get { return 12; } }
		public virtual int VertImageIndent { get { return 1; } }
		public Rectangle PictureRect { get { return pictureRect; } }
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault baseAppearance = base.CreateDefaultAppearance();
			Font font = baseAppearance.Font == null ? AppearanceObject.DefaultFont : baseAppearance.Font;
			baseAppearance.ForeColor = Item.GetLinkColor();
			baseAppearance.Font = ResourceCache.DefaultCache.GetFont(font, font.Style | FontStyle.Underline);
			return baseAppearance;
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
		}
		public virtual Color GetTextColor() {
			if(Item.LinkColorUseParentAppearance && PaintAppearance.GetForeColor() != Color.Empty) return PaintAppearance.GetForeColor();
			if(Appearance.GetForeColor() == Color.Empty) return DefaultAppearance.ForeColor;
			return Appearance.GetForeColor();
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class HyperLinkEditPainter : ButtonEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawPicture(info);
		}
		protected virtual void DrawPicture(ControlGraphicsInfoArgs info) {
			HyperLinkEditViewInfo vi = info.ViewInfo as HyperLinkEditViewInfo;
			if(vi.PictureRect.IsEmpty) return;
			info.Cache.Paint.DrawImage(info.Graphics, vi.Item.Image, vi.PictureRect, new Rectangle(Point.Empty, vi.PictureRect.Size), vi.Enabled);
		}
		protected override void DrawTextBoxArea(ControlGraphicsInfoArgs info) {
			base.DrawTextBoxArea(info);
			HyperLinkEditViewInfo vi = info.ViewInfo as HyperLinkEditViewInfo;
			if(!vi.FillBackground || vi.MaskBoxRect == vi.ContentRect || vi.MaskBoxRect.IsEmpty || vi.PictureRect.IsEmpty) return;
			Rectangle r = new Rectangle(vi.ContentRect.Left, vi.MaskBoxRect.Top, 0, vi.MaskBoxRect.Height);
			if(vi.MaskBoxRect.Left > vi.ContentRect.Left) {
				r.Width = vi.MaskBoxRect.Left - vi.ContentRect.Left;
			}
			else if(vi.MaskBoxRect.Right < vi.ContentRect.Right) {
				r.Width = vi.ContentRect.Right - vi.MaskBoxRect.Right;
				r.X = vi.MaskBoxRect.Right;
			}
			if(r.Width > 0) info.Paint.FillRectangle(info.Graphics, vi.PaintAppearance.GetBackBrush(info.Cache), r);
		}
		protected override void DrawString(ControlGraphicsInfoArgs info, Rectangle bounds, string text, AppearanceObject appearance) {
			HyperLinkEditViewInfo vi = info.ViewInfo as HyperLinkEditViewInfo;
			if(vi.OwnerEdit != null || info.ViewInfo.AllowHtmlString) {
				if(info.ViewInfo.AllowHtmlString)
					appearance.ForeColor = vi.IsCellSelected ? appearance.ForeColor : vi.GetTextColor();
				base.DrawString(info, bounds, text, appearance);
				return;
			}
			Brush foreBrush = info.Cache.GetSolidBrush(vi.IsCellSelected ? appearance.ForeColor : vi.GetTextColor());
			Font font = ResourceCache.DefaultCache.GetFont(appearance.Font, appearance.Font.Style | FontStyle.Underline);
			appearance.DrawString(info.Cache, text, bounds, font, foreBrush, appearance.GetStringFormat(info.ViewInfo.DefaultTextOptions));
		}
	}
} 
namespace DevExpress.XtraEditors.Controls {
	public class OpenLinkEventArgs : EventArgs {
		object editValue;
		bool handled;
		public OpenLinkEventArgs(object editValue) {
			this.handled = false;
			this.editValue = editValue;
		}
		public object EditValue { get { return editValue; } set { editValue = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public delegate void OpenLinkEventHandler(object sender, OpenLinkEventArgs e);
}
