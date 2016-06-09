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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Design;
namespace DevExpress.XtraEditors.Repository {
	public interface IHorzAlignmentProvider {
		HorzAlignment Alignment { get; }
	}
	public class BaseRepositoryItemCheckEdit : RepositoryItem, IHorzAlignmentProvider {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new BaseRepositoryItemCheckEdit Properties { get { return this; } }
		HorzAlignment glyphAlignment;
		VertAlignment glyphVAlignment;
		bool fullFocusRect;
		string caption;
		bool autoWidth = false;
		public BaseRepositoryItemCheckEdit() {
			this.glyphAlignment = HorzAlignment.Center;
			this.glyphVAlignment = VertAlignment.Center;
			this.caption = "BaseCheck";
			this.fullFocusRect = false;
			this.fBorderStyle = BorderStyles.NoBorder;
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			if(type == "check" || type == "toggleSwitch") {
				SetupTextBrickStyleProperties(info, style);
			}
			return style;
		}
		public override void Assign(RepositoryItem item) {
			BaseRepositoryItemCheckEdit source = item as BaseRepositoryItemCheckEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.fBorderStyle = source.BorderStyle;
				this.fullFocusRect = source.FullFocusRect;
				this.glyphAlignment = source.GlyphAlignment;
				this.glyphVAlignment = source.GlyphVAlignment;
			}
			finally {
				EndUpdate();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseRepositoryItemCheckEditBorderStyle"),
#endif
 DefaultValue(BorderStyles.NoBorder)]
		public override BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseRepositoryItemCheckEditAutoWidth"),
#endif
 DefaultValue(false)]
		public bool AutoWidth {
			get { return autoWidth; }
			set {
				if(AutoWidth == value) return;
				autoWidth = value;
				OnAutoWidthChanged();
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
		Obsolete(ObsoleteText.SRObsoleteCheckEditAllowHtmlString)
		]
		public bool AllowHtmlString {
			get { return AllowHtmlDraw == DefaultBoolean.True; }
			set {
				if(AllowHtmlString == value) return;
				AllowHtmlDraw = value ? DefaultBoolean.True : DefaultBoolean.False;
			}
		}
		bool ShouldSerializeGlyphAlignment() {
			if(OwnerEdit == null) return GlyphAlignment != HorzAlignment.Center;
			return GlyphAlignment != ((BaseCheckEdit)OwnerEdit).DefaultGlyphAlignment;
		}
		void ResetGlyphAlignment() { GlyphAlignment = ((BaseCheckEdit)OwnerEdit).DefaultGlyphAlignment; }
		HorzAlignment IHorzAlignmentProvider.Alignment { get { return GlyphAlignment; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseRepositoryItemCheckEditGlyphAlignment"),
#endif
 Localizable(true), SmartTagProperty("GlyphAlignment", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual HorzAlignment GlyphAlignment {
			get { return glyphAlignment; }
			set {
				if(GlyphAlignment == value) return;
				glyphAlignment = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeGlyphVAlignment() {
			if(OwnerEdit == null) return GlyphVAlignment != VertAlignment.Center;
			return GlyphVAlignment != ((BaseCheckEdit)OwnerEdit).DefaultGlyphVAlignment;
		}
		void ResetGlyphVAlignment() { GlyphVAlignment = ((BaseCheckEdit)OwnerEdit).DefaultGlyphVAlignment; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseRepositoryItemCheckEditGlyphVAlignment"),
#endif
 Localizable(true)]
		public virtual VertAlignment GlyphVAlignment {
			get { return glyphVAlignment; }
			set {
				if(GlyphVAlignment == value) return;
				glyphVAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseRepositoryItemCheckEditFullFocusRect"),
#endif
 DefaultValue(false)]
		public bool FullFocusRect {
			get { return fullFocusRect; }
			set {
				if(FullFocusRect == value) return;
				fullFocusRect = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseRepositoryItemCheckEditCaption"),
#endif
 DefaultValue("BaseCheck"), Localizable(true)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(value == null) value = "";
				if(Caption == value) return;
				caption = value;
				OnCaptionChanged();
			}
		}
		protected void OnCaptionChanged() {
			OnPropertiesChanged();
			LayoutChanged();
			if(OwnerEdit != null)
				((BaseCheckEdit)OwnerEdit).OnCaptionChanged();
		}
		protected virtual Image TransformPicture(Image image) {
			return ImageHelper.MakeTransparent(image, false);
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(key == Keys.Space)
				return true;
			return base.NeededKeysContains(key);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "BaseCheckEdit"; } }
		protected virtual internal ArrayList CheckImages { get; private set; }
		[ThreadStatic]
		protected static ArrayList fCheckImages = null;
		protected static ArrayList fToggleImages = null;
		static protected void LoadImages(ImageCollection images, ref ArrayList imageCollection) {
			if(imageCollection == null) {
				imageCollection = new ArrayList();
				for(int n = 0; n < images.Images.Count; n++) {
					imageCollection.Add(images.Images[n]);
				}
				images.Dispose();
			}
		}
		protected internal bool IsEquals(object val1, object val2) {
			return val1 == val2 || (val1 != null && val2 != null && val1.Equals(val2));
		}
		[Browsable(false)]
		public virtual bool IsRadioButton {
			get {
				return false;
			}
		}
		protected override bool AllowFormatEditValue { get { return false; } }
		protected override bool AllowParseEditValue { get { return false; } }
		protected virtual void OnAutoWidthChanged() {
			if(IsLoading || IsLockUpdate) return;
			BaseCheckEdit OwnerCheckEdit = OwnerEdit as BaseCheckEdit;
			if(OwnerCheckEdit != null) OwnerCheckEdit.OnAutoWidthChanged();
		}
	}
}
namespace DevExpress.XtraEditors {
	public class BaseCheckEdit : BaseEdit {
		int _keyDownCounter = 0;
		Size preferredSize = Size.Empty;
		bool needUpdateLayout = false;
		public BaseCheckEdit() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		protected override bool AllowPaintBackground {
			get {
				if(BackColor == Color.Transparent) return true;
				return base.AllowPaintBackground;
			}
		}
		[ DXCategory(CategoryName.Properties), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(false)]
		public override bool AutoSizeInLayoutControl {
			get { return base.AutoSizeInLayoutControl; }
			set { base.AutoSizeInLayoutControl = value; OnPropertiesChanged(true); }
		}
		protected override void InitializeDefault() {
			base.InitializeDefault();
			this.fOldEditValue = this.fEditValue = false;
		}
		protected override bool CanAnimateCore {
			get {
				return LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && base.CanAnimateCore;
			}
		}
		protected internal override void OnAutoHeightChanged() {
			base.OnAutoHeightChanged();
			if(Properties.AutoHeight) {
				Height = ViewInfo.CalcCheckAutoHeight(Width);
			}
		}
		protected internal override void OnAppearancePaintChanged() {
			base.OnAppearancePaintChanged();
			if(!IsHandleCreated) return;
			LayoutChanged();
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			LayoutChanged();
		}
		protected internal override void OnPropertiesChanged() {
			base.OnPropertiesChanged();			
			AutoSize = Properties.AutoWidth;
			OnAutoWidthChanged();
		}
		protected override void OnTextChanged(EventArgs e) {
			using(LayoutTransactionHelper.CreateLayoutTransactionIf(this, AutoSize, "Text")) {
				base.OnTextChanged(e);
				Invalidate();
			}
		}
		protected override bool IsInputChar(char ch) {
			if(InplaceType != InplaceType.Standalone) return true;
			return base.IsInputChar(ch);
		}
		public override Size CalcBestSize() {
			bool addGraphics = false;
			if(ViewInfo.GInfo.Graphics == null) {
				ViewInfo.GInfo.AddGraphics(null);
				addGraphics = true;
			}
			try {
				ViewInfo.UpdatePaintAppearance();
				Size size = ViewInfo.CalcBestFit(ViewInfo.GInfo.Graphics);
				if(Properties.AutoWidth)
					size.Width += ViewInfo.ErrorIconBounds.Width;
				return size;
			}
			finally {
				if(addGraphics) ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected override Size CalcSizeableMaxSize() {
			return AutoSizeInLayoutControl ? CalcSizeableMinSize() : base.CalcSizeableMaxSize();
		}
		string prevText = string.Empty;
		protected override Size CalcSizeableMinSize() {
			if(ViewInfo != null && prevText != Text) {
				prevText = Text;
				EditHitInfo prevHi = ViewInfo.PressedInfo;
				ViewInfo.Reset();
				ViewInfo.MousePosition = prevHi.HitPoint;
				ViewInfo.PressedInfo = prevHi;
			}
			Size sz = CalcBestSize();
			if(Properties.AutoHeight) {
				sz.Height = ViewInfo.CalcCheckAutoHeight(sz.Width);
				LayoutChanged();
			}
			return sz;
		}
		public override Size GetPreferredSize(Size proposedSize) {
			Size sz = base.GetPreferredSize(proposedSize);
			if(Properties.AutoWidth) {
				sz.Width = CalcBestSize().Width;
			}
			if(Properties.AutoHeight) {
				sz.Height = ViewInfo.CalcCheckAutoHeight(sz.Width);
			}
			return sz;
		}
		bool DockedHorizontally { get { return Dock == DockStyle.Fill || Dock == DockStyle.Top || Dock == DockStyle.Bottom; } }
		bool DockedVertically { get { return Dock == DockStyle.Fill || Dock == DockStyle.Left || Dock == DockStyle.Right; } }
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(Properties.AutoWidth && !DockedHorizontally) {
				Size sz = CalcBestSize();
				width = sz.Width;
			}
			if(Properties.AutoHeight && !DockedVertically) {
				if(PreferredHeight != -1) {
					height = ViewInfo.CalcCheckAutoHeight(width);
				}
			}
			DoBaseSetBoundsCore(x, y, width, height, specified);
		}
		protected override void InitializeDefaultProperties() {
			base.InitializeDefaultProperties();
			Properties.GlyphAlignment = DefaultGlyphAlignment;
			Properties.GlyphVAlignment = DefaultGlyphVAlignment;
		}
		protected internal HorzAlignment DefaultGlyphAlignment { get { return HorzAlignment.Near; } }
		protected internal VertAlignment DefaultGlyphVAlignment { get { return VertAlignment.Center; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "BaseCheckEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new BaseRepositoryItemCheckEdit Properties { get { return base.Properties as BaseRepositoryItemCheckEdit; } }
		protected internal new BaseCheckEditViewInfo ViewInfo { get { return base.ViewInfo as BaseCheckEditViewInfo; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckEditText"),
#endif
 Bindable(false), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return Properties.Caption; }
			set {
				Properties.Caption = value;
				LayoutChanged();
				preferredSize = Size.Empty;
				OnTextChanged(EventArgs.Empty);
				if(!IsHandleCreated) needUpdateLayout = true;
			}
		}
		[Browsable(false)]
		public new Size PreferredSize {
			get {
				if(preferredSize == Size.Empty) preferredSize = CalcBestSize();
				return preferredSize;
			}
		}
		protected internal override void LayoutChanged() {
			if(IsDisposing)
				return;
			base.LayoutChanged();
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckEditBorderStyle"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(BorderStyles.NoBorder), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
		}
		protected override void OnEnter(EventArgs e) {
			if(Control.MouseButtons == MouseButtons.None) {
				if(Properties.IsRadioButton) Toggle();
			}
			base.OnEnter(e);
		}
		protected override void OnChangeUICues(UICuesEventArgs e) {
			LayoutChanged();
			base.OnChangeUICues(e);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			if(this._keyDownCounter == 0) this._keyDownCounter = 1;
			base.OnEditorKeyDown(e);
			if(e.KeyData == Keys.Enter && EnterMoveNextControl) {
				this.ProcessDialogKey(Keys.Tab);
				e.Handled = true;
			}
			if(e.Handled || Properties.ReadOnly) return;
			if(e.KeyData == Keys.Space) {
				ViewInfo.CheckPressed = true;
				UpdateViewInfoState();
			}			
		}
		protected override void OnEditorKeyUp(KeyEventArgs e) {
			base.OnEditorKeyUp(e);
			if(this._keyDownCounter == 0) return;
			this._keyDownCounter = 0; 
			if(Properties.ReadOnly) return;
			ViewInfo.CheckPressed = false;
			UpdateViewInfoState();
			if(e.Handled) return;
			if(e.KeyData == Keys.Space) {
				Toggle();
			}
		}
		protected virtual void TryUpdateCursorByHyperlink(MouseEventArgs e) { 
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			TryUpdateCursorByHyperlink(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ee);
			if(ee.Handled) {
				return;
			}
			if(ee.Button == MouseButtons.Left && !Properties.ReadOnly) {
				EditHitInfo hi = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
				if(hi.IsInEdit) {
					ViewInfo.MousePosition = hi.HitPoint;
					ViewInfo.PressedInfo = hi;
				}
			}
			UpdateViewInfoState();
		}
		protected virtual bool TryClickHyperlink(MouseEventArgs e) {
			return false;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ee.Handled) {
				base.OnMouseUp(ee);
				return;
			}
			if(ee.Button == MouseButtons.Left) {
				if(TryClickHyperlink(e)) {
					ViewInfo.PressedInfo = EditHitInfo.Empty;
				}
				else {
					EditHitInfo hi = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
					bool prevIsInEdit = ViewInfo.PressedInfo.IsInEdit;
					ViewInfo.PressedInfo = EditHitInfo.Empty;
					if(prevIsInEdit && hi.IsInEdit && !Properties.ReadOnly) {
						Toggle();
					}
				}
			}
			UpdateViewInfoState();
			base.OnMouseUp(ee);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(needUpdateLayout) OnPropertiesChanged();
		}
		protected override void ClearHotPressed() {
			ViewInfo.CheckPressed = false;
			base.ClearHotPressed();
		}
		public virtual void Toggle() { }
		protected override void OnParentForeColorChanged(EventArgs e) {
			base.OnParentForeColorChanged(e);
			if(InplaceType == InplaceType.Standalone) LayoutChanged();
		}
		protected override void OnParentBackColorChanged(EventArgs e) {
			if(InplaceType != InplaceType.Standalone) return;
			LayoutChanged();
		}
		protected virtual bool CanUseMnemonic(char charCode) {
			return Control.IsMnemonic(charCode, this.Text) && this.CanSelect;
		}
		protected override bool ProcessMnemonic(char charCode) {
			if(CanUseMnemonic(charCode)) {
				if(Form.ActiveForm == FindForm() || (Form.ActiveForm != null && Form.ActiveForm.ActiveMdiChild == FindForm()))
					Select();
				if(Focused) Toggle();
				return true;
			}
			return false;
		}
		protected override bool SizeableIsCaptionVisible { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new event ConvertEditValueEventHandler FormatEditValue {
			add { base.FormatEditValue += value; }
			remove { base.FormatEditValue -= value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new event ConvertEditValueEventHandler ParseEditValue {
			add { base.ParseEditValue += value; }
			remove { base.ParseEditValue -= value; }
		}
		protected internal virtual void OnCaptionChanged() {
			if(Properties.AutoWidth)
				OnTextChanged(EventArgs.Empty);
		}
		protected internal virtual void OnAutoWidthChanged() {
			if(CheckAutoWidth())
				LayoutChanged();
		}
		protected internal override void OnLoaded() {
			base.OnLoaded();
			if(IsHandleCreated) {
				OnAutoWidthChanged();
			}
		}
		int _lockAutoWidth = 0;		
		protected int PreferredWidth {
			get {
				if(IsLayoutLocked || !Properties.AutoWidth) return -1;
				return CalcBestSize().Width;
			}
		}
		protected virtual bool CheckAutoWidth() {
			if(IsLayoutLocked|| !Properties.AutoWidth) return false;
			if(PreferredWidth != Width) {
				this._lockAutoWidth++;
				try {
					Width = PreferredWidth;
				}
				finally {
					this._lockAutoWidth--;
				}				
			}
			return false;
		}
		protected override bool IsLayoutLocked { get { return base.IsLayoutLocked || _lockAutoWidth != 0; } }
	}   
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class BaseCheckEditViewInfo : BaseEditViewInfo {
		BaseCheckObjectInfoArgs checkInfo;
		BaseCheckObjectPainter checkPainter;
		bool checkPressed;
		public BaseCheckEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			BaseCheckEditViewInfo be = info as BaseCheckEditViewInfo;
			if(be == null) return;
			this.checkPainter = be.checkPainter;
			this.checkPressed = be.checkPressed;
			this.checkInfo.Assign(be.CheckInfo);
		}
		protected internal override bool AllowDrawParentBackground { get { return true; } }
		protected override BorderPainter GetBorderPainterCore() {
			BorderStyles bs = BorderStyle;
			if(bs == BorderStyles.Default) bs = BorderStyles.NoBorder;
			return BorderHelper.GetPainter(bs, LookAndFeel, IsPrinting);
		}
		protected override int MaxTextWidth {
			get {
				if(Item.AutoWidth || PaintAppearance.TextOptions.WordWrap != WordWrap.Wrap) return 0;
				return CheckInfo.CaptionRect.Width;
			}
		}
		protected internal virtual int CalcCheckAutoHeight(int width) {
			GInfo.AddGraphics(null);
			try {
				CalcContentRect(new Rectangle(0, 0, width, 1000));
				IsReady = false;
				return base.CalcMinHeightCore(GInfo.Graphics);
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(AllowHtmlString && CheckInfo.StringInfo != null) {
				CheckInfo.StringInfo.UpdateAppearanceColors(PaintAppearance);
			}
		}
		public override bool AllowHtmlString { get { return Item.GetAllowHtmlDraw(); } }
		protected override Size CalcHtmlTextSize(GraphicsCache cache, string text, int maxWidth) { return Size.Empty; }
		protected override void CalcBestFitTextSize(Graphics g) {
			string prevText = DisplayText;
			SetDisplayText(Item.GlyphAlignment == HorzAlignment.Center ? "" : Item.Caption, EditValue);
			try {
				base.CalcBestFitTextSize(g);
			}
			finally {
				SetDisplayText(prevText, EditValue);
			}
		}
		public override bool IsRequiredUpdateOnMouseMove { get { return true; } }
		public override Rectangle GetTextBounds() { return CheckInfo.CaptionRect; }
		public BaseCheckObjectInfoArgs CheckInfo { get { return checkInfo; } }
		public BaseCheckObjectPainter CheckPainter { get { return checkPainter; } }
		public virtual bool CheckPressed { get { return checkPressed; } set { checkPressed = value; } }
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			CheckInfo.OffsetContent(x, y);
		}
		protected override void UpdatePainters() {
			this.checkPainter = CreateCheckPainter();
			base.UpdatePainters();
		}
		protected virtual BaseCheckObjectPainter CreateCheckPainter() { return null; }
		protected virtual BaseCheckObjectInfoArgs CreateCheckArgs() {
			return new BaseCheckObjectInfoArgs(PaintAppearance);
		}
		public new BaseRepositoryItemCheckEdit Item { get { return base.Item as BaseRepositoryItemCheckEdit; } }
		public override bool DefaultAllowDrawFocusRect { get { return true; } }
		public override bool DrawFocusRect {
			get {
				if(OwnerEdit != null) {
					bool ret = Item.AllowFocused && OwnerEdit.EditorContainsFocus;
					if(Item.Caption == string.Empty && !Item.FullFocusRect) ret = false;
					return ret;
				}
				return false;
			}
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone && OwnerEdit.Parent != null) {
					AppearanceDefault res = base.DefaultAppearance.Clone() as AppearanceDefault;
					res.BackColor = Color.Transparent;
					return res;
				}
				return base.DefaultAppearance;
			}
		}
		protected virtual Color GetForeColor() {
			SkinElement element = CommonSkins.GetSkin(Item.LookAndFeel)[CommonSkins.SkinLabel];
			if(element == null)
				return SystemColors.WindowText;
			return LookAndFeelHelper.CheckTransparentForeColor(Item.LookAndFeel, element.GetAppearanceDefault().ForeColor,
				Item.OwnerEdit, Enabled);
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault app = base.CreateDefaultAppearance();
			if(Item.LookAndFeel != null &&
				Item.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) app.ForeColor = GetForeColor();
			return app;
		}
		protected override ObjectState CalcObjectState() {
			ObjectState newState = base.CalcObjectState();
			if(Enabled) {
				if(PressedInfo.IsInEdit) {
					newState |= ObjectState.Pressed;
				}
				if(CheckPressed)
					newState |= ObjectState.Pressed | ObjectState.Hot;
			}
			return newState;
		}
		protected override bool UpdateObjectState() { 
			ObjectState prevState = State;
			bool res = base.UpdateObjectState();
			res |= UpdateCheckState(CheckInfo);
			return res || prevState != State;
		}
		public override void Reset() {
			this.checkPressed = false;
			this.checkInfo = CreateCheckArgs();
			this.checkInfo.HtmlContext = this;
			base.Reset();
		}
		public HorzAlignment GlyphAlignment {
			get {
				HorzAlignment halign = Item.GlyphAlignment;
				if(halign == HorzAlignment.Default) halign = HorzAlignment.Near;
				return halign;
			}
		}
		protected override Size CalcContentSize(Graphics g) {
			BaseCheckObjectInfoArgs e = CreateCheckArgs();
			UpdateCheckProperties(e);
			GInfo.AddGraphics(g);
			Size size = Size.Empty;
			try {
				e.Cache = GInfo.Cache;
				size = CheckPainter.CalcObjectMinBounds(e).Size;
				size.Height = Math.Max(size.Height, 10);
				size.Width = Math.Max(size.Width, 5);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		protected virtual bool UpdateCheckState(BaseCheckObjectInfoArgs e) {
			ObjectState newState = State, prevState = e.State;
			if(newState == ObjectState.Selected) newState = ObjectState.Normal;
			newState &= (~ObjectState.Selected);
			if(e.State != newState) OnBeforeCheckStateChanged(e, newState);
			e.State = newState;
			return prevState != e.State;
		}
		protected virtual void OnBeforeCheckStateChanged(BaseCheckObjectInfoArgs e, ObjectState newState) {
			if(Bounds.IsEmpty || CheckInfo.Bounds.IsEmpty) return;
			if(!AllowAnimation) return;
			XtraAnimator.Current.AddBitmapAnimation(OwnerEdit, 0, XtraAnimator.Current.CalcAnimationLength(e.State, newState),
				null, new ObjectPaintInfo(CheckPainter, e), new BitmapAnimationImageCallback(OnRequestCheckImage));
		}
		Bitmap OnRequestCheckImage(BaseAnimationInfo info) {
			if(Bounds.IsEmpty || CheckInfo.Bounds.IsEmpty) return new Bitmap(1, 1);
			return XtraAnimator.Current.CreateBitmap(CheckPainter, CheckInfo);
		}
		protected virtual void UpdateCheckProperties(BaseCheckObjectInfoArgs e) {
			UpdateCheckState(e);
			Rectangle rect = CheckInfo.Bounds;
			e.RightToLeft = RightToLeft;
			e.ShowHotKeyPrefix = OwnerControl != null ? OwnerControl.ShowKeyboardCuesCore : true;
			CheckInfo.Bounds = rect;
			e.Caption = Item.Caption;
			e.SetAppearance(PaintAppearance);
			e.GlyphAlignment = GlyphAlignment;
			e.GlyphVAlignment = GetGlyphVAlignmentProperty(Item.GlyphVAlignment);
			e.LookAndFeel = LookAndFeel;
			e.StringInfo = CheckInfo.StringInfo;
			e.AllowHtmlString = Item.GetAllowHtmlDraw();
			if(!Item.AutoWidth && Item.AutoHeight)
				e.CaptionRect = CheckInfo.CaptionRect;
			e.AutoHeight = Item.AutoHeight;
		}
		VertAlignment GetGlyphVAlignmentProperty(VertAlignment va) {
			if(va != VertAlignment.Default) return va;
			return this.PaintAppearance.TextOptions.VAlignment;
		}
		protected override void OnPaintAppearanceChanged() {
			base.OnPaintAppearanceChanged();
			CheckInfo.SetAppearance(PaintAppearance);
		}
		protected override void UpdateContentRectByFocusRect() {
			if(AllowDrawFocusRect) {
				if(Item.FullFocusRect)
					base.UpdateContentRectByFocusRect();
				else {
					if(GlyphAlignment == HorzAlignment.Near)
						fContentRect.Width -= FocusRectThin;
					else if(GlyphAlignment == HorzAlignment.Far) {
						fContentRect.Inflate(-FocusRectThin, 0);
					}
				}
			}
		}
		protected override Size UpdateSizeByFocusRect(Size size) {
			if(AllowDrawFocusRect ) {
				size.Width += FocusRectThin * 2;
				size.Height += FocusRectThin * 2;
			}
			return size;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			CheckInfo.Bounds = ContentRect;
			UpdateCheckProperties(CheckInfo);
			CheckInfo.AllowHtmlString = AllowHtmlString;
			CheckInfo.Cache = GInfo.Cache;
			CheckPainter.CalcObjectBounds(CheckInfo);
			CheckInfo.Cache = null;
			if(DrawFocusRect) {
				if(!Item.FullFocusRect && CheckInfo.GlyphDrawMode != TextGlyphDrawModeEnum.Glyph) {
					fFocusRect.X = CheckInfo.CaptionRect.X;
					fFocusRect.Width = CheckInfo.CaptionRect.Width;
					fFocusRect.Inflate(2, 0);
				}
			}
		}
	}
}
