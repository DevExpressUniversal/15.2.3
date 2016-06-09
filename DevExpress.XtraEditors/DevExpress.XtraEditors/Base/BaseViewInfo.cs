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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Text;
using DevExpress.Data;
namespace DevExpress.XtraEditors.ViewInfo {
	public enum EditHitTest { None, Bounds, MaskBox, Button, Button2, Glyph }
	public class EditHitInfo {
		protected EditHitTest fHitTest;
		protected Point fHitPoint;
		protected object fHitObject;
		public EditHitInfo(Point hitPoint)
			: this() {
			this.fHitPoint = hitPoint;
		}
		public EditHitInfo() {
			Clear();
		}
		static EditHitInfo empty = new EditHitInfo();
		public bool IsEmpty { get { return this.IsEquals(Empty); } }
		public static EditHitInfo Empty { get { return empty; } }
		public object HitObject { get { return fHitObject; } }
		public virtual Point HitPoint { get { return fHitPoint; } }
		public EditHitTest HitTest { get { return fHitTest; } }
		public virtual void Clear() {
			this.fHitTest = EditHitTest.None;
			this.fHitPoint = new Point(-10000, -10000);
			this.fHitObject = null;
		}
		public virtual bool IsEquals(EditHitInfo hitInfo) {
			if(hitInfo == null) return false;
			return (this.HitTest == hitInfo.HitTest && this.HitObject == hitInfo.HitObject);
		}
		public virtual bool IsInEdit { get { return HitTest != EditHitTest.None; } }
		protected internal void SetHitObject(object newValue) { this.fHitObject = newValue; }
		protected internal void SetHitTest(EditHitTest newValue) { this.fHitTest = newValue; }
		protected internal void SetHitPoint(Point newValue) { this.fHitPoint = newValue; }
		public virtual void Assign(EditHitInfo hitInfo) {
			this.fHitObject = hitInfo.HitObject;
			this.fHitPoint = hitInfo.fHitPoint;
			this.fHitTest = hitInfo.HitTest;
		}
	}
	public class BaseEditViewInfo : BaseControlViewInfo, IServiceProvider, IStringImageProvider {
		public static bool ShowFieldBindings = false;
		public const int FocusRectThin = 2;
		AppearanceObject prevParent;
		protected FormatInfo fFormat;
		DetailLevel _detailLevel;
		bool _fillBackground, _allowDrawFocusRect, _refreshDisplayText, isCellSelected, showErrorIcon,
			isPrinting, _showTextToolTip, _allowTextToolTip, _textUseFullBounds, matchedStringUseContains = false,
			matchedStringAllowPartial = false, useHighlightSearchAppearance = false;
		BorderStyles _defaultBorderStyle;
		protected Rectangle fErrorIconBounds;
		protected object fEditValue;
		protected string fDisplayText, fMatchedString;
		protected Size fTextSize, fErrorIconSize;
		protected Image fErrorIcon;
		InplaceType inplaceType;
		protected AppearanceObject fAppearanceDisabled;
		protected EditHitInfo fHotInfo, fPressedInfo;
		protected UserLookAndFeel fLookAndFeel;
		string errorIconText;
		bool allowOverridedState, isLoadingValue = false;
		ObjectState overridedState;
		object tag;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int PCount { get; set; }
		public override object Clone() {
			if(Item != null) {
				BaseEditViewInfo info = Item.CreateViewInfo();
				info.Assign(this);
				info.UpdatePainters();
				return info;
			}
			return null;
		}
		protected internal virtual object GetHtmlContext() {
			return this;
		}
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if(Item.HtmlImages == null) return null;
			return Item.HtmlImages.Images[id];
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IStringImageProvider)) return this;
			if(serviceType == typeof(ISite)) return OwnerControl == null ? Item.Site : OwnerControl.Site;
			return null;
		}
		#endregion
		protected override Font GetDefaultFont() {
			if(!IsSkinLookAndFeel) return base.GetDefaultFont();
			return GetDefaultSkinFont(CommonSkins.SkinTextControl);
		}
		public virtual bool RequireClipping { get { return false; } }
		protected override void ReCalcViewInfoCore(Graphics g, MouseButtons buttons, Point mousePosition, Rectangle bounds) {
			base.ReCalcViewInfoCore(g, buttons, mousePosition, bounds);
			this.isCellSelected = false;
		}
		protected override bool CanFastRecalcViewInfo(Rectangle bounds, Point mousePosition) {
			if(!base.CanFastRecalcViewInfo(bounds, mousePosition)) return false;
			if(ShowErrorIcon == ErrorIconBounds.IsEmpty) return false;
			return true;
		}
		public virtual string UpdateGroupValueDisplayText(string groupValueText, object value) {
			return groupValueText;
		}
		public virtual void UpdateBoundValues(DataController controller, int row) { }
		protected internal virtual bool AllowDrawParentBackground { get { return false; } }
		public bool UseParentBackground { get { return AllowDrawParentBackground && Item.UseParentBackground; } }
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			BaseEditViewInfo be = info as BaseEditViewInfo;
			if(be == null) return;
			this.inplaceType = be.InplaceType;
			this.prevParent = be.prevParent;
			this.fFormat = be.fFormat;
			this._detailLevel = be._detailLevel;
			this._fillBackground = be._fillBackground;
			this._allowDrawFocusRect = be._allowDrawFocusRect;
			this._refreshDisplayText = be._refreshDisplayText;
			this.isCellSelected = be.isCellSelected;
			this.showErrorIcon = be.showErrorIcon;
			this.isPrinting = be.isPrinting;
			this._showTextToolTip = be._showTextToolTip;
			this._allowTextToolTip = be._allowTextToolTip;
			this._defaultBorderStyle = be._defaultBorderStyle;
			this.fErrorIconBounds = be.fErrorIconBounds;
			this.fMatchedString = be.fMatchedString;
			this.fTextSize = be.fTextSize;
			this.fErrorIconSize = Size.Empty;
			this.fErrorIcon = be.fErrorIcon;
			this.fAppearanceDisabled = be.fAppearanceDisabled;
			this.fLookAndFeel = be.fLookAndFeel;
			this.errorIconText = be.errorIconText;
			this.allowOverridedState = be.allowOverridedState;
			this.overridedState = be.overridedState;
			this.tag = be.tag;
		}
		public BaseEditViewInfo(RepositoryItem item)
			: base(item) {
			this._defaultBorderStyle = BorderStyles.Default;
			this.isPrinting = false;
			this._allowTextToolTip = this._showTextToolTip = false;
			this.tag = null;
			this.errorIconText = "";
			this.showErrorIcon = false;
			this.isCellSelected = false;
			this._textUseFullBounds = false;
			this.allowOverridedState = false;
			this.overridedState = ObjectState.Normal;
			this._fillBackground = true;
			this._refreshDisplayText = false;
			this.fFormat = item.ActiveFormat;
			this.fLookAndFeel = item.LookAndFeel;
			this._detailLevel = DetailLevel.Full;
			this.fErrorIconSize = Size.Empty;
			this.inplaceType = InplaceType.Standalone;
		}
		bool editable = true; 
		public bool Editable {
			get { return editable; }
			set { editable = value; }
		}
		protected virtual bool ShowTextToolTip {
			get { return _showTextToolTip; }
			set {
				_showTextToolTip = value;
			}
		}
		[Browsable(false)]
		public virtual bool IsRequiredUpdateOnMouseMove {
			get {
				return BorderStyle == BorderStyles.HotFlat || BorderStyle == BorderStyles.UltraFlat ||
					BorderStyle == BorderStyles.Office2003 || BorderStyle == BorderStyles.Default;
			}
		}
		public virtual bool AllowTextToolTip {
			get { return _allowTextToolTip; }
			set { _allowTextToolTip = value; }
		}
		public bool IsLoadingValue {
			get { return isLoadingValue; }
			set {
				if(isLoadingValue == value) return;
				isLoadingValue = value;
				if(InplaceType == InplaceType.Standalone) UpdateAppearances();
			}
		}
		public virtual BorderStyles DefaultBorderStyle { get { return _defaultBorderStyle; } set { _defaultBorderStyle = value; } }
		public virtual bool IsPrinting { get { return isPrinting; } set { isPrinting = value; } } 
		public virtual object Tag { get { return tag; } set { tag = value; } }
		public virtual string ErrorIconText {
			get { return errorIconText; }
			set { errorIconText = value; }
		}
		public virtual bool ShowErrorIcon {
			get { return showErrorIcon; }
			set { showErrorIcon = value; }
		}
		public bool TextUseFullBounds {
			get { return _textUseFullBounds; }
			set { _textUseFullBounds = value; }
		}
		public virtual int GetTextAscentHeight() {
			GInfo.AddGraphics(null);
			try {
				return DevExpress.Utils.Text.TextUtils.GetFontAscentHeight(GInfo.Graphics, PaintAppearance.Font);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual Rectangle GetTextBounds() { return Rectangle.Empty; }
		public InplaceType InplaceType { get { return OwnerEdit != null ? OwnerEdit.InplaceType : inplaceType; } set { inplaceType = value; } }
		public Rectangle ErrorIconBounds { get { return fErrorIconBounds; } }
		public ErrorIconAlignment ErrorIconAlignment { get { return OwnerEdit != null ? OwnerEdit.ErrorIconAlignment : BaseEdit.DefaultErrorIconAlignment; } }
		public virtual Image GetErrorIcon() {
			if(OwnerEdit != null && OwnerEdit.ErrorIcon != null) return OwnerEdit.ErrorIcon;
			if(ErrorIcon == null) return BaseEdit.DefaultErrorIcon;
			return ErrorIcon;
		}
		public Size GetErrorIconSize() {
			if(this.fErrorIconSize == Size.Empty) {
				this.fErrorIconSize = GetErrorIcon().Size;
			}
			return this.fErrorIconSize;
		}
		public virtual Image ErrorIcon {
			get { return fErrorIcon; }
			set {
				if(ErrorIcon == value) return;
				fErrorIcon = value;
				this.fErrorIconSize = Size.Empty;
			}
		}
		public override UserLookAndFeel LookAndFeel {
			get {
				if(fLookAndFeel == null) fLookAndFeel = Item.LookAndFeel;
				if(fLookAndFeel == null) fLookAndFeel = UserLookAndFeel.Default;
				return fLookAndFeel;
			}
			set {
				if(value == null) value = Item.LookAndFeel;
				if(value == LookAndFeel) return;
				fLookAndFeel = value;
			}
		}
		public virtual Cursor GetMouseCursor(Point point) { return Cursors.Default; }
		public Cursor GetMouseCursor(int x, int y) { return GetMouseCursor(new Point(x, y)); }
		public virtual string GetDataBindingText() {
			if(OwnerEdit == null || OwnerEdit.InplaceType != InplaceType.Standalone || !OwnerEdit.IsDesignMode || OwnerEdit.DataBindings.Count == 0) return "";
			return string.Format("[{0}]", OwnerEdit.DataBindings[0].BindingMemberInfo.BindingMember);
		}
		public virtual bool IsCellSelected {
			get { return isCellSelected; }
			set {
				if(IsCellSelected == value) return;
				isCellSelected = value;
				OnIsCellSelectedChanged();
			}
		}
		protected virtual void OnIsCellSelectedChanged() { }
		public virtual bool RefreshDisplayText { get { return _refreshDisplayText; } set { _refreshDisplayText = value; } }
		public virtual bool CompareHitInfo(EditHitInfo h1, EditHitInfo h2) {
			return h1.IsEquals(h2);
		}
		public virtual EditHitInfo CalcHitInfo(Point p) {
			EditHitInfo hi = CreateHitInfo(p);
			if(Bounds.Contains(p)) {
				hi.SetHitTest(EditHitTest.Bounds);
			}
			return hi;
		}
		protected bool IsEqualsEditValueCore { get; set; }
		public virtual object EditValue {
			get { return fEditValue; }
			set {
				if(EditValue == value && OwnerEdit != null && !RefreshDisplayText) return;
				IsEqualsEditValueCore = EditValue == value;
				fEditValue = value;
				this._refreshDisplayText = false;
				OnEditValueChanged();
				IsEqualsEditValueCore = false;
			}
		}
		protected virtual void OnEditValueChanged() {
			this._refreshDisplayText = false;
			this.fDisplayText = GetDisplayText();
		}
		protected virtual string GetDisplayText() {
			if(BaseEdit.IsNotLoadedValue(EditValue)) return string.Empty;
			return Item.GetDisplayText(Format, EditValue);
		}
		protected virtual EditHitInfo CreateHitInfo(Point p) {
			return new EditHitInfo(p);
		}
		public virtual BorderStyles BorderStyle {
			get {
				if(Item.BorderStyle == BorderStyles.Default) return DefaultBorderStyle;
				return Item.BorderStyle;
			}
		}
		protected virtual BorderPainter GetDefaultSkinBorderPainter(ISkinProvider provider) {
			return new SkinTextBorderPainter(provider);
		}
		protected override bool IsOffice2003ExBorder {
			get {
				if(IsPrinting || InplaceType != InplaceType.Standalone) return false;
				if(BorderStyle == BorderStyles.Office2003 ||
					(BorderStyle == BorderStyles.Default && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)) return true;
				return false;
			}
		}
		InplaceBorderPainter inplaceBorderPainter = null;
		protected InplaceBorderPainter GetInplaceBorderPainter(ObjectPainter originalPainter) {
			if(inplaceBorderPainter == null) inplaceBorderPainter = new InplaceBorderPainter(this, originalPainter);
			else inplaceBorderPainter.OriginalBorder = originalPainter;
			return inplaceBorderPainter;
		}
		protected override BorderPainter GetBorderPainter() {
			BorderPainter painter = GetBorderPainterCore();
			if(InplaceType == InplaceType.Grid && Item.AllowInplaceBorderPainter) return GetInplaceBorderPainter(painter);
			return painter;
		}
		protected override BorderPainter GetBorderPainterCore() {
			if(IsOffice2003ExBorder) return Office2003BorderPainterEx.Default;
			return BorderHelper.GetPainter(BorderStyle, LookAndFeel, IsPrinting, GetDefaultSkinBorderPainter(LookAndFeel.ActiveLookAndFeel));
		}
		public virtual FormatInfo Format {
			get { return fFormat; }
			set { fFormat = value; }
		}
		public override string DisplayText { get { return fDisplayText; } }
		public void SetDisplayText(string newText) { SetDisplayText(newText, EditValue); }
		public virtual void SetDisplayText(string newText, object editValue) { fDisplayText = newText; }
		public virtual bool DrawFocusRect { get { return false; } }
		public virtual bool AllowDrawContent { get { return true; } }
		public virtual bool DefaultAllowDrawFocusRect { get { return false; } }
		public virtual bool AllowDrawFocusRect { get { return _allowDrawFocusRect; } set { _allowDrawFocusRect = value; } }
		public virtual bool FillBackground { get { return _fillBackground; } set { _fillBackground = value; } }
		public virtual bool IsInvertIncrementalSearchString { get { return false; } }
		public virtual bool IsSupportIncrementalSearch { get { return false; } }
		public bool UseHighlightSearchAppearance { get { return useHighlightSearchAppearance; } set { useHighlightSearchAppearance = value; } }
		public virtual string MatchedString { get { return fMatchedString; } set { fMatchedString = value; } }
		public virtual bool MatchedStringAllowPartial { get { return matchedStringAllowPartial; } set { matchedStringAllowPartial = value; } }
		public virtual bool MatchedStringUseContains { get { return matchedStringUseContains; } set { matchedStringUseContains = value; } }
		public virtual EditHitInfo HotInfo { get { return fHotInfo; } set { fHotInfo = value; } }
		public virtual EditHitInfo PressedInfo {
			get { return fPressedInfo; }
			set {
				if(PressedInfo.IsEquals(value)) return;
				if(!value.IsEmpty) value = CalcHitInfo(MousePosition);
				fPressedInfo = value;
			}
		}
		public virtual DetailLevel DetailLevel { get { return _detailLevel; } set { _detailLevel = value; } }
		public override void Clear() {
			this.fErrorIconBounds = Rectangle.Empty;
			this.fErrorIconSize = Size.Empty;
			base.Clear();
		}
		protected static object nullValue = new object();
		public override void Reset() {
			base.Reset();
			this._refreshDisplayText = true;
			this._showTextToolTip = false;
			this.fEditValue = nullValue;
			this.fDisplayText = "";
			this._allowDrawFocusRect = DefaultAllowDrawFocusRect;
			this.fTextSize = Size.Empty;
			this.fPressedInfo = this.fHotInfo = EditHitInfo.Empty;
			this._detailLevel = DetailLevel.Full;
			this.fMatchedString = "";
			this.fAppearance = Item.Appearance;
			this.fAppearanceDisabled = Item.AppearanceDisabled;
		}
		public virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			CheckShowHint();
			if(!ErrorIconBounds.IsEmpty && ErrorIconBounds.Contains(point)) {
				ToolTipControlInfo res = new ToolTipControlInfo("ErrorInfo", ErrorIconText, true, ToolTipIconType.None);
				res.ToolTipImage = ErrorIcon;
				return res;
			}
			if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone && (OwnerEdit.ToolTip.Length > 0 || OwnerEdit.ShouldSerializeSuperTip())) {
				if(!Bounds.Contains(point))
					return null;
				ToolTipControlInfo res = new ToolTipControlInfo(OwnerEdit, OwnerEdit.ToolTip, OwnerEdit.ToolTipTitle, OwnerEdit.ToolTipIconType);
				res.SuperTip = OwnerEdit.SuperTip;
				res.AllowHtmlText = OwnerEdit.AllowHtmlTextInToolTip;
				return res;
			}
			if(ShowTextToolTip && AllowTextToolTip) return CalcTextToolTipInfo(point);
			return null;
		}
		protected virtual ToolTipControlInfo CalcTextToolTipInfo(Point point) { return null; }
		public override ObjectState State {
			get {
				if(AllowOverridedState) return OverridedState;
				return base.State;
			}
			set {
				if(AllowOverridedState) return;
				base.State = value;
			}
		}
		public virtual ObjectState OverridedState {
			get { return overridedState; }
			set { overridedState = value; }
		}
		public virtual bool AllowOverridedState {
			get { return allowOverridedState; }
			set { allowOverridedState = value; }
		}
		public virtual bool Enabled {
			get {
				if(AllowOverridedState && OverridedState == ObjectState.Disabled) return false;
				return Item.Enabled;
			}
		}
		protected virtual bool UseDisabledAppearance {
			get {
				return !Enabled && Item != null && !Item.IsDesignMode;
			}
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				if(!UseDisabledAppearance) {
					return base.DefaultAppearance;
				}
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					Skin skin = CommonSkins.GetSkin(LookAndFeel);
					return new AppearanceDefault(skin.Colors.GetColor(CommonColors.DisabledText), skin.Colors.GetColor(CommonColors.DisabledControl));
				}
				return new AppearanceDefault(SystemColors.GrayText, SystemColors.Control);
			}
		}
		public override void UpdatePaintAppearance() {
			PaintAppearance.Reset();
			ResetAppearanceDefault();
			AppearanceObject readOnly = Item.ReadOnly ? Item.AppearanceReadOnly : null;
			bool useDisabled = !Enabled && !Item.IsDesignMode;
			if(StyleController != null) {
				AppearanceObject readOnlySC = Item.ReadOnly ? StyleController.AppearanceReadOnly : null;
				if(useDisabled) {
					AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { AppearanceDisabled, StyleController.AppearanceDisabled, Appearance, StyleController.Appearance }, DefaultAppearance);
				}
				else {
					if(CanUseFocusedAppearance) {
						AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { readOnly, readOnlySC, Item.AppearanceFocused, StyleController.AppearanceFocused, Item.Appearance, StyleController.Appearance }, DefaultAppearance);
					}
					else {
						AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { readOnly, readOnlySC, Item.Appearance, StyleController.Appearance }, DefaultAppearance);
					}
				}
			}
			else {
				AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { useDisabled ? AppearanceDisabled : null, readOnly, Appearance }, DefaultAppearance);
			}
			if(PaintAppearance.HAlignment == HorzAlignment.Default) PaintAppearance.TextOptions.HAlignment = Item.DefaultAlignment;
			PaintAppearance.TextOptions.RightToLeft = RightToLeft;
		}
		public virtual AppearanceObject AppearanceDisabled {
			get { return fAppearanceDisabled; }
			set {
				if(value == null) return;
				fAppearanceDisabled = value;
			}
		}
		public override BaseControlPainter Painter { get { return Item.Painter; } }
		public RepositoryItem Item { get { return Owner as RepositoryItem; } }
		protected internal override BaseControl OwnerControl { get { return OwnerEdit; } }
		public BaseEdit OwnerEdit { get { return Item.OwnerEdit; } }
		protected override void UpdateFromOwner() {
			UpdateFromEditor();
		}
		protected virtual void UpdateFromEditor() {
			Format = Item.ActiveFormat;
			if(UseParentBackground) FillBackground = false;
			UpdateEditValue();
			if(OwnerEdit == null) return;
			InplaceType = OwnerEdit.InplaceType;
			switch(InplaceType) {
				case InplaceType.Grid:
					AllowDrawFocusRect = false;
					DefaultBorderStyle = Item.DefaultBorderStyleInGrid;
					break;
				case InplaceType.Bars:
					AllowDrawFocusRect = OwnerEdit.Properties.AllowFocused;
					AllowOverridedState = true;
					OverridedState = ObjectState.Hot;
					break;
				case InplaceType.Standalone:
					this.fLookAndFeel = null;
					break;
			}
			RightToLeft = OwnerEdit.IsRightToLeft;
			ErrorIconText = OwnerEdit.ErrorText;
			ErrorIcon = OwnerEdit.ErrorIcon;
			ShowErrorIcon = ErrorIconText != null && ErrorIconText.Length > 0;
			Focused = OwnerEdit.EditorContainsFocus;
			Appearance = GetEditorAppearance();
			AppearanceDisabled = Item.AppearanceDisabled;
			UpdatePaintAppearance();
		}
		protected override bool CanUseFocusedAppearance { get { return OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone && Focused; } }
		protected AppearanceObject GetEditorAppearance() {
			if(CanUseFocusedAppearance) return new AppearanceObject(Item.AppearanceFocused, Item.Appearance);
			return Item.Appearance;
		}
		public virtual void UpdateEditValue() {
			if(OwnerEdit != null) {
				EditValue = OwnerEdit.EditValue;
				if(RefreshDisplayText) OnEditValueChanged();
			}
		}
		protected override void UpdateEnabledState() {
			State = ObjectState.Normal;
			if(!Enabled && (!Item.IsDesignMode))
				State = ObjectState.Disabled;
		}
		protected override ObjectState CalcObjectState() {
			EditHitInfo hitInfo = CalcHitInfo(MousePosition);
			ObjectState res = ObjectState.Normal;
			if(Focused)
				res = ObjectState.Selected;
			else
				res = ObjectState.Normal;
			if(Item.IsDesignMode) res = ObjectState.Normal;
			else {
				if(Enabled) {
					res |= IsHotEdit(hitInfo) ? ObjectState.Hot : ObjectState.Normal;
				}
				else {
					res = ObjectState.Disabled;
				}
				this.fHotInfo = hitInfo;
			}
			return res;
		}
		protected override bool UpdateObjectState() { 
			EditHitInfo hitInfo = CalcHitInfo(MousePosition);
			ObjectState prevState = State;
			State = CalcObjectState();
			return prevState != State;
		}
		protected virtual bool IsHotEdit(EditHitInfo hitInfo) {
			return hitInfo.IsInEdit;
		}
		protected override void CalcConstants() {
			CalcTextSize(null);
		}
		const int ErrorIconIndent = 4;
		protected override void CalcClientRect(Rectangle bounds) {
			this.fErrorIconBounds = Rectangle.Empty;
			if(ShowErrorIcon) {
				Size esize = GetErrorIconSize();
				if(bounds.Width > esize.Width + 2 && bounds.Height >= esize.Height) {
					this.fErrorIconBounds = bounds;
					this.fErrorIconBounds.Width = esize.Width + ErrorIconIndent;
					switch(GetErrorIconAlignment()) {
						case ErrorIconAlignment.MiddleLeft:
							bounds.X += this.fErrorIconBounds.Width;
							bounds.Width -= this.fErrorIconBounds.Width;
							break;
						case ErrorIconAlignment.MiddleRight:
							this.fErrorIconBounds.X = bounds.Right - this.fErrorIconBounds.Width;
							bounds.Width = this.fErrorIconBounds.X - bounds.X;
							break;
						case ErrorIconAlignment.TopLeft:
							this.fErrorIconBounds.Height = esize.Height + ErrorIconIndent;
							bounds.X += this.fErrorIconBounds.Width;
							bounds.Width -= this.fErrorIconBounds.Width;
							break;
						case ErrorIconAlignment.TopRight:
							this.fErrorIconBounds.Height = esize.Height + ErrorIconIndent;
							this.fErrorIconBounds.X = bounds.Right - this.fErrorIconBounds.Width;
							bounds.Width = this.fErrorIconBounds.X - bounds.X;
							break;
						case ErrorIconAlignment.BottomLeft:
							this.fErrorIconBounds.Height = esize.Height + ErrorIconIndent;
							this.fErrorIconBounds.Y = bounds.Bottom - this.fErrorIconBounds.Height;
							bounds.X += this.fErrorIconBounds.Width;
							bounds.Width -= this.fErrorIconBounds.Width;
							break;
						case ErrorIconAlignment.BottomRight:
							this.fErrorIconBounds.Height = esize.Height + ErrorIconIndent;
							this.fErrorIconBounds.Y = bounds.Bottom - this.fErrorIconBounds.Height;
							this.fErrorIconBounds.X = bounds.Right - this.fErrorIconBounds.Width;
							bounds.Width = this.fErrorIconBounds.X - bounds.X;
							break;
					}
				}
			}
			base.CalcClientRect(bounds);
		}
		private ErrorIconAlignment GetErrorIconAlignment() {
			if(!RightToLeft) return ErrorIconAlignment;
			switch(ErrorIconAlignment) {
				case ErrorIconAlignment.MiddleLeft: return System.Windows.Forms.ErrorIconAlignment.MiddleRight;
				case ErrorIconAlignment.MiddleRight: return System.Windows.Forms.ErrorIconAlignment.MiddleLeft;
				case ErrorIconAlignment.TopLeft: return System.Windows.Forms.ErrorIconAlignment.TopRight;
				case ErrorIconAlignment.TopRight: return System.Windows.Forms.ErrorIconAlignment.TopLeft;
				case ErrorIconAlignment.BottomLeft: return System.Windows.Forms.ErrorIconAlignment.BottomRight;
				case ErrorIconAlignment.BottomRight: return System.Windows.Forms.ErrorIconAlignment.BottomLeft;
			}
			return ErrorIconAlignment;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			this.fContentRect = bounds;
			UpdateContentRectByFocusRect();
		}
		protected virtual void UpdateContentRectByFocusRect() {
			if(AllowDrawFocusRect)
				this.fContentRect.Inflate(-FocusRectThin, -FocusRectThin);
		}
		protected override void CalcFocusRect(Rectangle bounds) {
			this.fFocusRect = Rectangle.Empty;
			if(AllowDrawFocusRect) {
				fFocusRect = bounds;
			}
		}
		public virtual bool Compare(BaseEditViewInfo vi) {
			if(vi == null) return false;
			if(this.Bounds == vi.Bounds &&
				this.FocusRect == vi.FocusRect &&
				this.ContentRect == vi.ContentRect &&
				this.State == vi.State) return true;
			return false;
		}
		protected virtual void CalcBestFitTextSize(Graphics g) {
			CalcTextSize(g, true);
		}
		public override Size CalcBestFit(Graphics g) {
			UpdatePainters();
			CalcBestFitTextSize(g);
			Size size = CalcClientSize(g);
			size = BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(null, new Rectangle(Point.Empty, size), null)).Size;
			return size;
		}
		protected virtual Size CalcContentSize(Graphics g) {
			if(TextSize.IsEmpty) CalcTextSize(g);
			Size size = TextSize;
			return size;
		}
		public override ObjectState CalcBorderState() {
			ObjectState state = State;
			return state;
		}
		protected virtual Size UpdateSizeByFocusRect(Size size) {
			if(AllowDrawFocusRect) {
				size.Width += FocusRectThin * 2;
				size.Height += FocusRectThin * 2;
			}
			return size;
		}
		protected virtual Size CalcClientSize(Graphics g) {
			Size size = CalcContentSize(g);
			size = UpdateSizeByFocusRect(size);
			if(ShowErrorIcon) {
				Size errorIcon = GetErrorIconSize();
				if(errorIcon.Width > 0) size.Width += errorIcon.Width + ErrorIconIndent;
				if(size.Height < errorIcon.Height) size.Height = errorIcon.Height;
			}
			return size;
		}
		public int CalcMinHeight(Graphics g) {
			UpdateFromEditor();
			return CalcMinHeightCore(g);
		}
		protected virtual int CalcMinHeightCore(Graphics g) {
			return CalcBestFit(g).Height;
		}
		public virtual void Offset(int x, int y) {
			if(!fBorderRect.IsEmpty) this.fBorderRect.Offset(x, y);
			if(!fErrorIconBounds.IsEmpty) this.fErrorIconBounds.Offset(x, y);
			if(!fBounds.IsEmpty) this.fBounds.Offset(x, y);
			if(!fFocusRect.IsEmpty) this.fFocusRect.Offset(x, y);
			if(!fClientRect.IsEmpty) this.fClientRect.Offset(x, y);
			if(!fContentRect.IsEmpty) this.fContentRect.Offset(x, y);
			if(StringInfo != null && !StringInfo.IsEmpty) StringInfo.Offset(x, y);
		}
		public void Offset(Point p) { Offset(p.X, p.Y); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowScaleWidth { get { return true; } }
	}
	public interface IHeightAdaptable {
		int CalcHeight(GraphicsCache cache, int width);
	}
}
