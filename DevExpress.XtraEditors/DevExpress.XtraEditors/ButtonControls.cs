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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	public enum DropDownArrowStyle { Default, SplitButton, Hide, Show }
	[ToolboxItem(false)]
	public class BaseButton : BaseStyleControl, IButtonControl, DevExpress.Utils.MVVM.ISupportCommandBinding {
		bool isDefault;
		DialogResult _dialogResult;
		public BaseButton() {
			SetStyle(ControlStyles.StandardClick, false);
			SetStyle(ControlStyles.UserMouse | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
			this._dialogResult = DialogResult.None;
			this.isDefault = false;
		}
		protected virtual bool IsTransparentControl {
			get {
				if(ViewInfo == null || !ViewInfo.IsTransparent) return false;
				return true;
			}
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.ButtonControlAccessible(this);
		}
		protected override bool CanAnimateCore {
			get {
				if(!ViewInfo.AllowAnimation) return false;
				return XtraAnimator.Current.CanAnimate(LookAndFeel) && LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		bool ShouldSerializeBackColor() { return IsTransparentControl ? false : BackColor != Color.Empty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseStyleControlBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override Color BackColor {
			get { return IsTransparentControl ? Color.Transparent : base.BackColor; }
			set {
				if(IsTransparentControl) return;
				base.BackColor = value;
			}
		}
		protected internal virtual bool DownCore { get { return false; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseButtonDialogResult"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(DialogResult.None)]
		public DialogResult DialogResult {
			get { return _dialogResult; }
			set {
				if(DialogResult == value) return;
				this._dialogResult = value;
				OnPropertiesChanged(false);
			}
		}
		public void NotifyDefault(bool value) {
			if(this.IsDefault != value) {
				this.IsDefault = value;
			}
		}
		public void PerformClick() {
			if(this.CanSelect) {
				bool fireClick = true;
				ContainerControl container = GetContainerControl() as ContainerControl;
				if(container != null) {
					while(container.ActiveControl == null) {
						Control parent = container.Parent;
						if(parent != null) {
							ContainerControl cc = parent.GetContainerControl() as ContainerControl;
							if(cc != null) container = cc;
							else {
								break;
							}
						}
						else {
							break;
						}
					}
					fireClick = container.Validate(true) || container.AutoValidate == AutoValidate.EnableAllowFocusChange;
				}
				if(fireClick) OnClick(EventArgs.Empty);
			}
		}
		protected internal virtual bool IsDefault {
			get { return isDefault; }
			set {
				if(IsDefault == value) return;
				this.isDefault = value;
				OnPropertiesChanged(false);
			}
		}
		protected internal new BaseButtonViewInfo ViewInfo { get { return base.ViewInfo as BaseButtonViewInfo; } }
		protected override BaseControlPainter CreatePainter() { return new BaseButtonPainter(); }
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new BaseButtonViewInfo(this); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseButtonButtonStyle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default), SmartTagProperty("Button Style", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual BorderStyles ButtonStyle {
			get { return this.BorderStyle; }
			set { this.BorderStyle = value; }
		}
		protected override Size DefaultSize {
			get { return new Size(75, 23); }
		}
		public virtual Size CalcBestFit(Graphics g) {
			return ViewInfo.CalcBestFit(g);
		}
		public virtual Size CalcBestSize(int maxWidth) {
			ViewInfo.SetMaxTextWidth(maxWidth);
			try {
				return base.CalcBestSize();
			}
			finally {
				ViewInfo.SetMaxTextWidth(0);
			}
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			ViewInfo.IsKeyPressed = false;
			UpdatePressedState(false);
			UpdateViewInfoState();
			Capture = false;
		}
		protected override void OnParentVisibleChanged(EventArgs e) {
			UpdateViewInfoState(false);
			base.OnParentVisibleChanged(e);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			UpdateViewInfoState();
		}
		protected override void OnPaint(PaintEventArgs e) {
			CheckViewInfo(e.Graphics);
			base.OnPaint(e);
		}
		protected virtual void CheckViewInfo(Graphics g) {
			if(ViewInfo.ButtonInfo.Button.Caption != ViewInfo.Caption) {
				ViewInfo.CalcViewInfo(g);
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			UpdateViewInfo(e.Graphics);
			if(!IsTransparentControl) return;
			base.OnPaintBackground(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			UpdateViewInfoState();
			UpdateCursor(e);
		}
		protected virtual void UpdateCursor(MouseEventArgs e) {
		}
		protected bool isMouseDown = false;
		protected bool isDblClick = false;
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left) {
				if(FocusOnMouseDown) {
					if(!GetStyle(ControlStyles.UserMouse)) Select();
				}
				this.isMouseDown = true;
				UpdatePressedState(true);
			}
			if(e.Clicks == 2) isDblClick = true;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				UpdatePressedState(false);
				if(this.isMouseDown && ClientRectangle.Contains(new Point(e.X, e.Y)) && !GetValidationCanceled()) {
					OnClick(e);
					base.OnMouseClick(e);
					if(isDblClick)
						base.OnDoubleClick(EventArgs.Empty);
				}
				this.isMouseDown = false;
			}
			if(isDblClick) {
				MouseEventArgs mea = new MouseEventArgs(e.Button, 2, e.X, e.Y, e.Delta);
				base.OnMouseDoubleClick(mea);
			}
			this.isDblClick = false;
			base.OnMouseUp(e);
		}
		protected virtual void UpdatePressedState(bool setPressed) {
			if(DownCore) setPressed = true;
			ViewInfo.IsPressed = setPressed;
			UpdateViewInfoState();
		}
		protected override void OnLostCapture() {
			this.isMouseDown = false;
			if(ViewInfo.IsPressed) {
				UpdatePressedState(false);
			}
			base.OnLostCapture();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			if(e.KeyData == Keys.Space) {
				ViewInfo.IsKeyPressed = true;
				UpdateViewInfoState();
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			if(e.KeyData == Keys.Space) {
				bool prevPressed = ViewInfo.IsKeyPressed;
				ViewInfo.IsKeyPressed = false;
				UpdateViewInfoState();
				if(prevPressed) OnClick(EventArgs.Empty);
			}
		}
		internal void FireClick() {
			if(!Enabled) return;
			OnClick(EventArgs.Empty);
		}
		protected override void OnClick(EventArgs e) {
			Form form = FindForm();
			if(form != null) form.DialogResult = DialogResult;
			AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
			AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
			base.OnClick(e);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			LayoutChanged();
		}
		#region Commands
		public virtual IDisposable BindCommand(object command, Func<object> queryCommandParameter = null,
			Action<BaseButton, Func<bool>> updateState = null) {
			if(updateState == null)
				updateState = (button, canExecute) => button.Enabled = canExecute();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				updateState,
				command, queryCommandParameter);
		}
		public virtual IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source,
			Action<BaseButton, Func<bool>> updateState, Func<object> queryCommandParameter = null) {
			return BindCommand(commandSelector, source, queryCommandParameter, updateState);
		}
		public virtual IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source,
			Func<object> queryCommandParameter = null, Action<BaseButton, Func<bool>> updateState = null) {
			if(updateState == null)
				updateState = (button, canExecute) => button.Enabled = canExecute();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				updateState,
				commandSelector, source, queryCommandParameter);
		}
		public virtual IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source,
			Action<BaseButton, Func<bool>> updateState, Func<T> queryCommandParameter = null) {
			return BindCommand(commandSelector, source, queryCommandParameter, updateState);
		}
		public virtual IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source,
			Func<T> queryCommandParameter = null, Action<BaseButton, Func<bool>> updateState = null) {
			if(updateState == null)
				updateState = (button, canExecute) => button.Enabled = canExecute();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				updateState,
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		IDisposable DevExpress.Utils.MVVM.ISupportCommandBinding.BindCommand(object command, Func<object> queryCommandParameter) {
			return BindCommand(command, queryCommandParameter);
		}
		IDisposable DevExpress.Utils.MVVM.ISupportCommandBinding.BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter) {
			return BindCommand(commandSelector, source, queryCommandParameter);
		}
		IDisposable DevExpress.Utils.MVVM.ISupportCommandBinding.BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter) {
			return BindCommand<T>(commandSelector, source, queryCommandParameter);
		}
		#endregion Commands
	}
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.SimpleButtonDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Raises an event when a user clicks it."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(SimpleButtonActions), "Image", "Image", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "SimpleButton")
   ]
	public class SimpleButton : BaseButton, IAnimatedItem, ISupportXtraAnimation {
		ImageLocation imageLocation;
		Image image;
		int imageIndex;
		object imageList;
		bool allowFocus;
		DefaultBoolean allowHtmlDraw, showFocusRectangle;
		public SimpleButton() {
			this.allowFocus = true;
			this.allowHtmlDraw = DefaultBoolean.Default;
			this.imageLocation = ImageLocation.Default;
			this.image = null;
			this.imageIndex = -1;
			this.imageList = null;
			this.AutoSizeInLayoutControl = true;
			this.showFocusRectangle = DefaultBoolean.Default;
		}
		protected internal override void LayoutChanged() {
			base.LayoutChanged();
			StopAnimation();
			StartAnimation();
		}
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(Image);
				return imageHelper;
			}
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		Rectangle IAnimatedItem.AnimationBounds {
			get { return ClientRectangle; }
		}
		int IAnimatedItem.AnimationInterval {
			get { return ImageHelper.AnimationInterval; }
		}
		int[] IAnimatedItem.AnimationIntervals {
			get { return ImageHelper.AnimationIntervals; }
		}
		AnimationType IAnimatedItem.AnimationType {
			get { return ImageHelper.AnimationType; }
		}
		int IAnimatedItem.FramesCount {
			get { return ImageHelper.FramesCount; }
		}
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return ImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() {
		}
		void IAnimatedItem.OnStop() {
		}
		object IAnimatedItem.Owner {
			get { return this; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(this, BaseButtonViewInfo.GifAnimationId);
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = this;
			if(IsDesignMode || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(BaseButtonViewInfo.GifAnimationId, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(Image == null || info == null) return;
			if(!info.IsFinalFrame) {
				Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				Invalidate(animItem.AnimationBounds);
			}
			else {
				StopAnimation();
				StartAnimation();
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			StartAnimation();
		}
		Cursor PrevCursor { get; set; }
		protected override void UpdateCursor(MouseEventArgs e) {
			if(AllowHtmlDraw != DefaultBoolean.True || (ViewInfo.ButtonInfo.StringInfo != null && ViewInfo.ButtonInfo.StringInfo.GetLinkByPoint(e.Location) == null)) {
				if(PrevCursor != null) Cursor = PrevCursor;
				PrevCursor = null;
				return;
			}
			else {
				if(PrevCursor == null)
					PrevCursor = Cursor;
				Cursor = Cursors.Hand;
			}
		}
		protected override bool IsTransparentControl {
			get {
				return base.IsTransparentControl || LookAndFeel.ActiveLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new SimpleButtonViewInfo(this); }
		[Obsolete(ObsoleteText.SRObsoleteImageAlignment), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public HorzAlignment GlyphAlignment {
			get {
				return HorzAlignment.Default;
			}
			set { 
			}
		}
		[Obsolete(ObsoleteText.SRObsoleteImageLocation), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual HorzAlignment ImageAlignment {
			get { 
				if(Enum.IsDefined(typeof(HorzAlignment), (int)ImageLocation))
					return (HorzAlignment)(int)ImageLocation;
				return HorzAlignment.Default;
			}
			set {
				ImageLocation = (ImageLocation)(int)value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonImageLocation"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(ImageLocation.Default), SmartTagProperty("Image Location", "")]
		public virtual ImageLocation ImageLocation {
			get { return imageLocation; }
			set {
				if(ImageLocation == value) return;
				imageLocation = value;
				OnPropertiesChanged(true);
			}
		}
		ImageAlignToText imageAlignToText = ImageAlignToText.None;
		[DefaultValue(ImageAlignToText.None)]
		public virtual ImageAlignToText ImageToTextAlignment {
			get { return imageAlignToText; }
			set {
				if(ImageToTextAlignment == value)
					return;
				imageAlignToText = value;
				OnPropertiesChanged(true);
			}
		}
		protected internal override void OnPropertiesChanged() {
			base.OnPropertiesChanged();
			AdjustSize();
		}
		protected virtual void AdjustSize() {
			if(AutoSize && IsHandleCreated)
				Size = CalcBestSize();
		}
		int indentBetweenImageAndText = -1;
		[DefaultValue(-1)]
		public int ImageToTextIndent {
			get { return indentBetweenImageAndText; }
			set {
				if(ImageToTextIndent == value)
					return;
				indentBetweenImageAndText = value;
				OnPropertiesChanged(true);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonAllowFocus"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool AllowFocus {
			get { return allowFocus; }
			set {
				if(AllowFocus == value) return;
				allowFocus = value;
				FocusOnMouseDown = AllowFocus;
				SetStyle(ControlStyles.Selectable, AllowFocus);
				SetStyle(ControlStyles.UserMouse, AllowFocus);
				OnPropertiesChanged(false);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonShowFocusRectangle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowFocusRectangle {
			get { return showFocusRectangle; }
			set {
				if(ShowFocusRectangle == value) return;
				showFocusRectangle = value;
				OnPropertiesChanged(false);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonAllowHtmlDraw"),
#endif
 DefaultValue(DefaultBoolean.Default), DXCategory(CategoryName.Appearance)]
		public DefaultBoolean AllowHtmlDraw {
			get { return allowHtmlDraw; }
			set {
				if (AllowHtmlDraw == value) return;
				allowHtmlDraw = value;
				LayoutChanged();
			}
		}
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default), DXCategory(CategoryName.Appearance)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonText"),
#endif
 Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		 Editor(ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor)), SmartTagProperty("Text", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			OnPropertiesChanged(true);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonImage"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				StopAnimation();
				image = value;
				ImageHelper.Image = value;
				OnPropertiesChanged(true);
				ImageHelper.Image = value;
				StartAnimation();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonImageList"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public virtual object ImageList {
			get { return imageList; }
			set {
				if(ImageList == value) return;
				imageList = value;
				OnPropertiesChanged(true);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonImageIndex"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), ImageList("ImageList")]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnPropertiesChanged(false);
			}
		}
		protected override bool SizeableIsCaptionVisible { get { return false; } }
		protected override bool ProcessMnemonic(char charCode) {
			if(Control.IsMnemonic(charCode, this.Text) && this.CanSelect) {
				if(GetStyle(ControlStyles.UserMouse)) {
					if(Form.ActiveForm == FindForm() || (Form.ActiveForm != null && Form.ActiveForm.ActiveMdiChild == FindForm()))
						Focus();
				}
				if(!GetValidationCanceled())
					PerformClick();
				return true;
			}
			return false;
		}
		protected override void OnChangeUICues(UICuesEventArgs e) {
			LayoutChanged();
			base.OnChangeUICues(e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonAutoSizeInLayoutControl"),
#endif
 DXCategory(CategoryName.Properties), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DefaultValue(true)]
		public override bool AutoSizeInLayoutControl {
			get { return AutoWidthInLayoutControl || base.AutoSizeInLayoutControl; }
			set {
				base.AutoSizeInLayoutControl = value;
				OnPropertiesChanged(true);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonAutoSize"),
#endif
 DXCategory(CategoryName.Properties), RefreshProperties(RefreshProperties.All)]
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(false)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		protected override void OnAutoSizeChanged(EventArgs e) {
			base.OnAutoSizeChanged(e);
			OnPropertiesChanged(true);
		}
		bool autoWidthInLayoutCore;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SimpleButtonAutoWidthInLayoutControl"),
#endif
 DXCategory(CategoryName.Properties)]
		[DefaultValue(false), RefreshProperties(RefreshProperties.All)]
		public bool AutoWidthInLayoutControl {
			get { return autoWidthInLayoutCore; }
			set {
				if(autoWidthInLayoutCore == value) return;
				autoWidthInLayoutCore = value;
				OnPropertiesChanged(true);
			}
		}
		public override Size GetPreferredSize(Size proposedSize) {
			Size size = CalcBestSize();
			if(MaximumSize.Width > 0)
				size.Width = Math.Min(size.Width, MaximumSize.Width);
			if(MinimumSize.Width > 0)
				size.Width = Math.Max(size.Width, MinimumSize.Width);
			if(MaximumSize.Height > 0)
				size.Height = Math.Min(size.Height, MaximumSize.Height);
			if(MinimumSize.Height > 0)
				size.Height = Math.Max(size.Height, MinimumSize.Height);
			return size;
		}
		protected override Size CalcSizeableMaxSize() { 
			Size bestSize = CalcBestSize();
			return new Size(AutoWidthInLayoutControl ? bestSize.Width : 0, bestSize.Height);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public SimpleButtonViewInfo GetButtonViewInfo() {
			return ViewInfo as SimpleButtonViewInfo;
		}
	}
	[ToolboxItem(false)]
	public class CloseButton : SimpleButton {
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new CloseButtonViewInfo(this); }
		protected override Size DefaultSize {
			get { return new Size(16, 16); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
	[ToolboxItem(false)]
	public class CalculatorButton : SimpleButton {
		bool pushed;
		CalcButtonType buttonType;
		Color textColor;
		public CalculatorButton() {
			this.pushed = false;
			this.buttonType = CalcButtonType.None;
			this.TabStop = this.AllowFocus = false;
			this.textColor = Appearance.ForeColor;
		}
		[DXCategory(CategoryName.Behavior)]
		public virtual bool Pushed {
			get { return pushed; }
			set {
				if(Pushed == value) return;
				pushed = value;
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Behavior)]
		public CalcButtonType ButtonType { get { return buttonType; } set { buttonType = value; } }
		[DXCategory(CategoryName.Appearance)]
		public Color TextColor { get { return textColor; } set { textColor = value; } }
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new CalculatorButtonViewInfo(this); }
	}
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultEvent("CheckedChanged"),
	Description("Has two states - elevated and depressed, which are toggled on clicking the button."),
	ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(CheckButtonFilter)), SmartTagAction(typeof(CheckButtonActions), "Checked", "Checked", SmartTagActionType.RefreshContentAfterExecute), SmartTagAction(typeof(CheckButtonActions), "Unchecked", "Unchecked", SmartTagActionType.RefreshContentAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "CheckButton")
	]
	public class CheckButton : SimpleButton {
		static object checkedChanged;
		bool isChecked, allowAllUnchecked;
		int groupIndex;
		static CheckButton() {
			checkedChanged = new object();
		}
		[DXCategory(CategoryName.Events), Description("")]
		public event EventHandler CheckedChanged {
			add { base.Events.AddHandler(checkedChanged, value); }
			remove { base.Events.RemoveHandler(checkedChanged, value); }
		}
		public CheckButton()
			: base() {
			this.isChecked = this.allowAllUnchecked = false;
			this.groupIndex = -1;
		}
		public CheckButton(bool check)
			: this() {
			this.isChecked = check;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckButtonChecked"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(false)]
		public virtual bool Checked {
			get { return isChecked; }
			set {
				if(Checked == value) return;
				if(IsGrouped) {
					if(!AllowAllUnchecked) {
						if(!value) return;
						isChecked = value;
						UnToggleAll();
					}
					else {
						isChecked = value;
						if(value) UnToggleAll();
					}
				}
				isChecked = value;
				UpdateButtonState();
				RaiseCheckedChanged();
			}
		}
		protected virtual void UpdateButtonState() {
			UpdatePressedState(Checked);
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(false),  SmartTagProperty("Allow All Unchecked", "")]
		public virtual bool AllowAllUnchecked {
			get { return allowAllUnchecked; }
			set {
				if(AllowAllUnchecked == value) return;
				allowAllUnchecked = value;
				if(GroupIndex != -1) SetAllowAllUnchecked(value);
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckButtonGroupIndex"),
#endif
 DefaultValue(-1)]
		public virtual int GroupIndex {
			get { return groupIndex; }
			set {
				if(GroupIndex == value) return;
				groupIndex = value;
				LayoutChanged();
			}
		}
		protected bool IsGrouped { get { return GroupIndex > -1; } }
		protected internal override bool DownCore { get { return Checked; } }
		public virtual void Toggle() {
			Checked = !Checked;
		}
		protected virtual void UnToggleAll() {
			if((Parent != null) && IsGrouped) {
				foreach(Control control in Parent.Controls) {
					CheckButton button = control as CheckButton;
					if((button != null) && (button.GroupIndex == GroupIndex)) {
						button.TabStop = (button == this);
						if(button == this) continue;
						button.isChecked = false;
						button.UpdateButtonState();
					}
				}
			}
		}
		protected void SetAllowAllUnchecked(bool value) {
			if((Parent != null) && IsGrouped) {
				foreach(Control control in Parent.Controls) {
					CheckButton button = control as CheckButton;
					if((button != null) && (button.GroupIndex == GroupIndex)) {
						if(button == this) continue;
						button.allowAllUnchecked = value;
					}
				}
			}
		}
		protected virtual void RaiseCheckedChanged() {
			EventHandler handler = (EventHandler)base.Events[checkedChanged];
			if(handler != null) {
				handler(this, new EventArgs());
			}
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(IsGrouped) TabStop = Checked;
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.KeyData == Keys.Space) Toggle();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			Toggle();
		}
		protected override bool ProcessMnemonic(char charCode) {
			if(Control.IsMnemonic(charCode, Text) && CanSelect) {
				if(Form.ActiveForm == FindForm() || (Form.ActiveForm != null && Form.ActiveForm.ActiveMdiChild == FindForm()))
					Select();
				if(Focused) Toggle();
				return true;
			}
			return false;
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new CheckButtonViewInfo(this);
		}
		#region Commands
		public override IDisposable BindCommand(object command, Func<object> queryCommandParameter = null,
			Action<BaseButton, Func<bool>> updateState = null) {
			if(updateState == null)
				updateState = (button, canExecute) => ((CheckButton)button).Checked = !canExecute();
			return base.BindCommand(command, queryCommandParameter, updateState);
		}
		public override IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source,
			Func<object> queryCommandParameter = null, Action<BaseButton, Func<bool>> updateState = null) {
			if(updateState == null)
				updateState = (button, canExecute) => ((CheckButton)button).Checked = !canExecute();
			return base.BindCommand(commandSelector, source, queryCommandParameter, updateState);
		}
		public override IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source,
			Func<T> queryCommandParameter = null, Action<BaseButton, Func<bool>> updateState = null) {
			if(updateState == null)
				updateState = (button, canExecute) => ((CheckButton)button).Checked = !canExecute();
			return base.BindCommand<T>(commandSelector, source, queryCommandParameter, updateState);
		}
		#endregion Commands
	}
	public interface IDXDropDownControlOwner {
		IDXDropDownControl DropDownControl { get; set; }
	}
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.DropDownButtonDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	Description("Allows you to display a popup control on clicking the button's down arrow."),
	ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "DropDownButton")
	]
	public class DropDownButton : SimpleButton, IDXMenuSupport, IDXDropDownControlOwner {
		static object arrowButtonClick;
		static object showDropDownControl;
		bool arrowButtonDown, dropDownVisible;
		bool actAsDropDown = true;
		DropDownArrowStyle dropDownArrowStyle;
		IDXDropDownControl dropDownControl;
		IDXMenuManager menuManager;
		static DropDownButton() {
			arrowButtonClick = new object();
			showDropDownControl = new object();
		}
		[DXCategory(CategoryName.Events), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DropDownButtonArrowButtonClick")
#else
	Description("")
#endif
]
		public event EventHandler ArrowButtonClick {
			add { base.Events.AddHandler(arrowButtonClick, value); }
			remove { base.Events.RemoveHandler(arrowButtonClick, value); }
		}
		[DXCategory(CategoryName.Events), Description("")]
		public event ShowDropDownControlEventHandler ShowDropDownControl {
			add { base.Events.AddHandler(showDropDownControl, value); }
			remove { base.Events.RemoveHandler(showDropDownControl, value); }
		}
		public DropDownButton()
			: base() {
			this.dropDownControl = null;
			this.arrowButtonDown = false;
			this.dropDownArrowStyle = DropDownArrowStyle.Default;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DropDownButtonDropDownControl"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(null), TypeConverter(typeof(ReferenceConverter))]
		public virtual IDXDropDownControl DropDownControl {
			get { return dropDownControl; }
			set {
				if(DropDownControl == value) return;
				if(DropDownControl != null && DropDownControl is IDXDropDownControlEx) {
					(DropDownControl as IDXDropDownControlEx).CloseUp -= new EventHandler(OnDropDownCloseUp);
				}
				dropDownControl = value;
				if(DropDownControl != null && DropDownControl is IDXDropDownControlEx) {
					(DropDownControl as IDXDropDownControlEx).CloseUp += new EventHandler(OnDropDownCloseUp);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DropDownButtonMenuManager"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue((string)null)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set { menuManager = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DropDownButtonDropDownArrowStyle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(DropDownArrowStyle.Default), SmartTagProperty("DropDown Arrow Style", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public DropDownArrowStyle DropDownArrowStyle {
			get { return dropDownArrowStyle; }
			set {
				if(DropDownArrowStyle == value)
					return;
				dropDownArrowStyle = value;
				OnPropertiesChanged(true);
			}
		}
		protected internal virtual DropDownArrowStyle GetDropDownArrowStyle() {
			if(DropDownArrowStyle == DropDownArrowStyle.Default)
				return DropDownArrowStyle.SplitButton;
			return DropDownArrowStyle;
		}
		protected internal bool IsSplitButtonVisible {
			get { return GetDropDownArrowStyle() == DropDownArrowStyle.SplitButton; }
		}
		[Obsolete("Use the DropDownArrowStyle property instead"), DXCategory(CategoryName.Behavior), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Show Arrow Button", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool ShowArrowButton {
			get { return GetDropDownArrowStyle() == DropDownArrowStyle.SplitButton; }
			set { DropDownArrowStyle = value ? DropDownArrowStyle.Default : DropDownArrowStyle.Hide; }
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public bool ActAsDropDown {
			get { return actAsDropDown; }
			set {
				if(ActAsDropDown == value) return;
				actAsDropDown = value;
				LayoutChanged();
			}
		}
		protected internal new DropDownButtonViewInfo ViewInfo { get { return base.ViewInfo as DropDownButtonViewInfo; } }
		[Browsable(false)]
		public bool IsOpened {
			get {
				if(dropDownControl == null) return false;
				if(IsDropDownControlEx) return DropDownControl.Visible;
				return dropDownVisible;
			}
		}
		bool shouldUnsubscribeOnCloseUpEvent = false;
		bool IsDropDownControlEx { get { return dropDownControl is IDXDropDownControlEx; } }
		protected internal override bool DownCore { get { return IsOpened && hasShown; } }
		protected override Size DefaultSize {
			get { return new Size(base.DefaultSize.Width + 60, base.DefaultSize.Height); }
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				if(IsDropDownControlEx) {
					(DropDownControl as IDXDropDownControlEx).CloseUp -= new EventHandler(OnDropDownCloseUp);
				}
				dropDownControl = null;
				menuManager = null;
			}
			base.Dispose(disposing);
		}
		protected override bool ProcessKeyMessage(ref Message m) {
			hasClosed = false;
			return base.ProcessKeyMessage(ref m);
		}
		protected internal bool IsArrowButtonDown { get { return arrowButtonDown || DownCore; } }
		protected virtual bool IsPressButtonContainsPoint(Point point) {
			if(IsSplitButtonVisible) return IsArrowButtonContainsPoint(point);
			return ViewInfo.Bounds.Contains(point);
		}
		public virtual bool IsArrowButtonContainsPoint(Point point) {
			return ViewInfo.IsArrowButtonContainsPoint(point);
		}
		protected override void UpdatePressedState(bool setPressed) {
			ViewInfo.UpdateArrowButtonState();
			base.UpdatePressedState(setPressed);
		}
		bool hasClosed = false, hasShown = false;
		protected void OnDropDownCloseUp(object sender, EventArgs e) {
			if(IsPressButtonContainsPoint(PointToClient(Cursor.Position)))
				hasClosed = true;
			hasShown = false;
			UpdatePressedState(false);
			if(shouldUnsubscribeOnCloseUpEvent) {
				IDXDropDownControlEx dropDown = sender as IDXDropDownControlEx;
				if(dropDown != null)
					dropDown.CloseUp -= new EventHandler(OnDropDownCloseUp);
				shouldUnsubscribeOnCloseUpEvent = false;
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && IsArrowButtonContainsPoint(new Point(e.X, e.Y))) {
				arrowButtonDown = true;
				UpdatePressedState(true);
				return;
			}
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && arrowButtonDown) {
				if(IsArrowButtonContainsPoint(new Point(e.X, e.Y)) && !GetValidationCanceled())
					OnArrowClick();
				arrowButtonDown = false;
				UpdatePressedState(false);
				return;
			}
			base.OnMouseUp(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			if(e.Handled) return;
			if(e.KeyData == Keys.Space && ViewInfo.IsKeyPressed) {
				if(!IsSplitButtonVisible) {
					OnClick(EventArgs.Empty);
					ViewInfo.IsKeyPressed = false;
					UpdateViewInfoState();
					return;
				}
			}
			base.OnKeyUp(e);
		}
		protected virtual void OnArrowClick() {
			RaiseArrowButtonClick();
			ShowHideDropDown();
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			if(!IsSplitButtonVisible)
				ShowHideDropDown();
		}
		void ShowHideDropDown() {
			if(CheckHasClosed()) return;
			if(IsOpened)
				DoHideDropDown();
			else
				DoShowDropDown();
		}
		bool CheckHasClosed() {
			if(hasClosed) {
				hasClosed = false;
				return true;
			}
			return false;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContextMenu ContextMenu {
			get { return null; }
			set { }
		}
		const int WM_ENTERMENULOOP = 0x211;
		const int WM_EXITMENULOOP = 530;
		protected override void WndProc(ref Message m) {
			if(wndProcHandler != null) wndProcHandler(this, ref m);
			if(m.Msg == WM_ENTERMENULOOP) {
				if(CheckWParam(m))
					dropDownVisible = true;
			}
			if(m.Msg == WM_EXITMENULOOP) {
				if(CheckWParam(m)) {
					dropDownVisible = false;
					OnDropDownCloseUp(this, EventArgs.Empty);
				}
			}
			base.WndProc(ref m);
		}
		bool CheckWParam(Message m) {
			return ((((int)((long)m.WParam)) == 0) ? 0 : 1) != 0;
		}
		protected virtual Point CalcMenuLocation() {
			Point location = new Point(0, Bounds.Height);
			if(!IsDropDownControlEx) {
				location = DevExpress.Utils.ControlUtils.CalcLocation(PointToScreen(location), PointToScreen(Location), Size.Empty);
				return PointToClient(location);
			}
			var control = DropDownControl as Control;
			if(control != null && ShouldShowDropDownControlAboveDropDownButton)
				location.Y = -(control.Height + control.Margin.Bottom + 1);
			return location;
		}
		protected virtual bool ShouldShowDropDownControlAboveDropDownButton {
			get {
				var control = DropDownControl as Control;
				if(control == null) return false;
				var dropDownButtonBottomLocation = PointToScreen(new Point(0, Bounds.Height));
				return (ScreenWorkingArea.Bottom < (dropDownButtonBottomLocation.Y + control.Height));
			}
		}
		protected Rectangle ScreenWorkingArea {
			get {
				Point pt = PointToScreen(new Point(0, 0));
				Screen ptScreen = Screen.FromPoint(pt);
				return ptScreen.WorkingArea;
			}
		}
		public void HideDropDown() {
			DoHideDropDown();
			UpdatePressedState(false);
			UpdateViewInfoState();
		}
		public void ShowDropDown() {
			if(IsOpened) return;
			UpdatePressedState(true);
			DoShowDropDown();
			if(!IsOpened) UpdatePressedState(false);
			UpdateViewInfoState();
		}
		protected void DoHideDropDown() {
			HideDropDownCore(dropDownControl);
			hasClosed = hasShown = false;
		}
		protected void DoShowDropDown() {
			if(!ActAsDropDown) return;
			ShowDropDownControlEventArgs e = new ShowDropDownControlEventArgs(dropDownControl, true);
			RaiseShowDropDownControl(e);
			if((e.DropDownControl != null && e.Allow)) {
				DropDownControl = e.DropDownControl;
				hasShown = true;
				ShowDropDownCore(dropDownControl, CalcMenuLocation());
			}
		}
		protected virtual void ShowDropDownCore(IDXDropDownControl control, Point location) {
			if(control == null) return;
			if((!IsDesignMode) && (MenuManager != null) && (control is IDXDropDownControlEx))
				control.Show(MenuManager, this, location);
			else {
				IDXDropDownMenuManager manager = MenuManager as IDXDropDownMenuManager;
				if(manager != null && DropDownControl is DXPopupMenu) {
					IDXDropDownControlEx dropDownContolEx = manager.ShowDropDownMenu((DXPopupMenu)DropDownControl, this, location) as IDXDropDownControlEx;
					if(dropDownContolEx != null) {
						dropDownContolEx.CloseUp += new EventHandler(OnDropDownCloseUp);
						this.shouldUnsubscribeOnCloseUpEvent = true;
						return;
					}
				}
				if(control is IDXDropDownControlEx)
					(control as IDXDropDownControlEx).Show(this, location);
				else
					control.Show(MenuManagerHelper.GetMenuManager(this.LookAndFeel), this, location);
			}
		}
		protected virtual void HideDropDownCore(IDXDropDownControl control) {
			if(control != null) control.Hide();
		}
		protected virtual void RaiseArrowButtonClick() {
			EventHandler handler = (EventHandler)base.Events[arrowButtonClick];
			if(handler != null) {
				handler(this, new EventArgs());
			}
		}
		protected virtual void RaiseShowDropDownControl(ShowDropDownControlEventArgs e) {
			ShowDropDownControlEventHandler handler = (ShowDropDownControlEventHandler)base.Events[showDropDownControl];
			if(handler != null) {
				handler(this, e);
			}
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new DropDownButtonViewInfo(this);
		}
		#region IDXMenuSupport Members
		event DXMenuWndProcHandler IDXMenuSupport.WndProc {
			add { wndProcHandler += value; }
			remove {
				if(wndProcHandler != null)
					wndProcHandler -= value;
			}
		}
		event DXMenuWndProcHandler wndProcHandler;
		DXPopupMenu IDXMenuSupport.Menu { get { return DropDownControl as DXPopupMenu; } }
		void IDXMenuSupport.ShowMenu(Point pos) { ShowDropDownCore(DropDownControl, pos); }
		#endregion
	}
	public delegate void ShowDropDownControlEventHandler(object sender, ShowDropDownControlEventArgs e);
	public class ShowDropDownControlEventArgs : EventArgs {
		bool allow;
		IDXDropDownControl dropDownControl;
		public ShowDropDownControlEventArgs(IDXDropDownControl dropDownControl, bool allow) {
			this.dropDownControl = dropDownControl;
			this.allow = allow;
		}
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
		public IDXDropDownControl DropDownControl {
			get { return dropDownControl; }
			set { dropDownControl = value; }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public enum CalcButtonType {
		MC, MR, MS, MAdd, Seven, Four, One, Zero, Eight,
		Five, Two, Sign, Nine, Six, Three, Decimal,
		Div, Mul, Sub, Add, Sqrt, Percent, Fract, Equal,
		Back, Cancel, Clear, None
	};
	public class CalculatorButtonViewInfo : SimpleButtonViewInfo {
		public CalculatorButtonViewInfo(CalculatorButton owner)
			: base(owner) {
		}
		public new CalculatorButton OwnerControl { get { return base.OwnerControl as CalculatorButton; } }
		protected override ObjectState CalcButtonState(EditorButtonObjectInfoArgs buttonInfo, ObjectState state) {
			ObjectState res = base.CalcButtonState(buttonInfo, state);
			if(state != ObjectState.Disabled && OwnerControl.Pushed) {
				res = ObjectState.Pressed;
			}
			return res;
		}
	}
	public class BaseButtonViewInfo : BaseStyleControlViewInfo {
		EditorButtonPainter buttonPainter;
		EditorButtonObjectInfoArgs buttonInfo;
		int maxTextWidth = 0;
		bool isPressed, focused, isKeyPressed;
		public BaseButtonViewInfo(BaseButton owner)
			: base(owner) {
		}
		public bool IsTransparent {
			get {
				if(Appearance.BackColor == Color.Transparent) return true;
				return ButtonInfo == null ? false : ButtonInfo.Transparent;
			}
		}
		protected override Font GetDefaultFont() {
			if(!IsSkinLookAndFeel) return base.GetDefaultFont();
			return GetDefaultSkinFont(CommonSkins.SkinButton);
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(GetForeColor(), GetSystemColor(SystemColors.Control), GetDefaultFont());
		}
		protected virtual Color GetForeColor() {
			if(!IsSkinLookAndFeel) {
				return GetSystemColor(SystemColors.ControlText);
			}
			SkinElement element = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinButton];
			return element.GetForeColor(State);
		}
		public override void Reset() {
			base.Reset();
			this.buttonInfo = CreateEditorButtonObjectInfoArgs(new EditorButton(), PaintAppearance);  
			this.buttonInfo.LookAndFeel = LookAndFeel;
			this.buttonInfo.FillBackground = true;
			this.buttonInfo.BuiltIn = false;
			this.isKeyPressed = this.focused = this.isPressed = false;
			ButtonInfo.SetAppearance(PaintAppearance);
			ButtonPainter.CalcObjectBounds(ButtonInfo);
		}
		protected virtual EditorButtonObjectInfoArgs CreateEditorButtonObjectInfoArgs(EditorButton editorButton, AppearanceObject appearance) {
			return new EditorButtonObjectInfoArgs(editorButton, appearance);
		}
		public override bool Focused {
			get { return focused; }
			set { focused = value; }
		}
		protected virtual ObjectState CalcButtonState(EditorButtonObjectInfoArgs buttonInfo, ObjectState state) {
			if(state != ObjectState.Disabled && (!OwnerControl.IsDesignMode && OwnerControl.IsDefault))
				state |= ObjectState.Selected;
			return state;
		}
		protected virtual void UpdateButton(EditorButtonObjectInfoArgs buttonInfo) {
			buttonInfo.Button.Appearance.Assign(PaintAppearance);
			buttonInfo.RightToLeft = RightToLeft;
			if(OwnerControl != null)
				buttonInfo.Padding = OwnerControl.Padding;
			buttonInfo.Button.Kind = ButtonPredefines.Glyph;
			buttonInfo.Button.Caption = Caption;
			buttonInfo.Bounds = Bounds;
			buttonInfo.BackAppearance = PaintAppearance;
			buttonInfo.UpdateButtonAppearance();
			buttonInfo.DrawFocusRectangle = Focused;
			buttonInfo.State = CalcButtonState(buttonInfo, State);
		}
		protected override BorderPainter GetBorderPainterCore() { return new EmptyBorderPainter(); }
		protected override void CalcFocusRect(Rectangle bounds) {
			this.fFocusRect = Rectangle.Empty;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(Bounds);
			UpdateButton(ButtonInfo);
			ButtonInfo.Cache = GInfo.Cache;
			ButtonPainter.CalcObjectBounds(ButtonInfo);
			ButtonInfo.Cache = null;
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			CalcTextBounds(g);
		}
		protected override int MaxTextWidth { get { return maxTextWidth; } }
		protected internal void SetMaxTextWidth(int width) { this.maxTextWidth = width; }
		public override Size CalcBestFit(Graphics g) {
			EditorButtonObjectInfoArgs buttonInfo = new EditorButtonObjectInfoArgs(new EditorButton(), null);
			buttonInfo.MaxTextWidth = MaxTextWidth;
			buttonInfo.BuiltIn = false;
			UpdateButton(buttonInfo);
			UpdatePainters();
			g = GInfo.AddGraphics(g);
			Size size = new Size(16, 16);
			try {
				buttonInfo.Cache = GInfo.Cache;
				size = ButtonPainter.CalcObjectMinBounds(buttonInfo).Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.buttonPainter = GetButtonPainter();
		}
		protected virtual EditorButtonPainter GetButtonPainter() {
			EditorButtonPainter painter = EditorButtonHelper.GetPainter(OwnerControl.ButtonStyle, OwnerControl.LookAndFeel);
			if(painter is WindowsXPEditorButtonPainter) {
				painter = new WindowsXPEditorButtonPainterEx();
			}
			return painter;
		}
		protected virtual HorzAlignment ImageAlignment { get { return HorzAlignment.Near; } }
		protected internal virtual string Caption { get { return OwnerControl.Text; } }
		public virtual EditorButtonPainter ButtonPainter { get { return buttonPainter; } }
		protected internal new BaseButton OwnerControl { get { return base.OwnerControl as BaseButton; } }
		public virtual EditorButtonObjectInfoArgs ButtonInfo { get { return buttonInfo; } }
		protected override void UpdateEnabledState() {
			State = ObjectState.Normal;
			if(!OwnerControl.Enabled && !OwnerControl.IsDesignMode)
				State = ObjectState.Disabled;
		}
		protected override void UpdateFromOwner() {
			base.UpdateFromOwner();
			UpdateButton(ButtonInfo);
		}
		public virtual bool IsPressed {
			get { return isPressed; }
			set { isPressed = value; }
		}
		public virtual bool IsKeyPressed {
			get { return isKeyPressed; }
			set { isKeyPressed = value; }
		}
		protected virtual internal bool UpdateObjectState(bool checkDesignMode) {
			bool inside = Bounds.Contains(MousePosition);
			ObjectState prevState = State, prevButtonState = ButtonInfo.State, newState;
			Focused = OwnerControl.Focused;
			if(Focused)
				newState = ObjectState.Selected;
			else
				newState = ObjectState.Normal;
			if(checkDesignMode && OwnerControl.IsDesignMode) newState = ObjectState.Normal;
			else {
				if(OwnerControl.Enabled) {
					newState |= inside ? ObjectState.Hot : ObjectState.Normal;
					if((IsPressed && inside) || OwnerControl.DownCore) newState |= ObjectState.Pressed;
					if(IsKeyPressed && Focused) newState |= ObjectState.Pressed;
				}
				else {
					newState = ObjectState.Disabled;
				}
			}
			ObjectState newButtonState = CalcButtonState(ButtonInfo, newState);
			bool changed = false;
			if(State != newState || ButtonInfo.State != newButtonState) {
				changed = true;
				OnBeforeStateChanged(newButtonState);
			}
			State = newState;
			ButtonInfo.State = newButtonState;
			return changed;
		}
		protected override bool UpdateObjectState() { 
			return UpdateObjectState(true);
		}
		internal static readonly object StateAnimationId = new object();
		internal static readonly object GifAnimationId = new object();
		protected virtual void OnBeforeStateChanged(ObjectState newState) {
			if(!IsReady || Bounds.Width < 1 || Bounds.Height < 1) return;
			if(!AllowAnimation) return;
			XtraAnimator.Current.AddBitmapAnimation(OwnerControl, StateAnimationId, XtraAnimator.Current.CalcAnimationLength(ButtonInfo.State, newState),
				null, new ObjectPaintInfo(ObjectPainterBaseControl.Default, new ObjectInfoArgsBaseControlViewInfo(this)),
				new BitmapAnimationImageCallback(RequestDestinationImage));
		}
		protected Bitmap RequestDestinationImage(BaseAnimationInfo info) {
			BaseButtonPainter painter = (BaseButtonPainter)Painter;
			painter.allowAnimation = false;
			try {
				return XtraAnimator.Current.CreateBitmap(ObjectPainterBaseControl.Default, new ObjectInfoArgsBaseControlViewInfo(this));
			}
			finally {
				painter.allowAnimation = true;
			}
		}
		public Rectangle TextBounds { get; set; }
		public void CalcTextBounds(Graphics gr) {
			CalcTextSize(gr);
			ObjectInfoArgs args = this.ButtonInfo;
			int TextGlyphIndent = ButtonInfo.Button.ImageToTextIndent >= 0? ButtonInfo.Button.ImageToTextIndent: ButtonPainter.TextGlyphIndent;
			args.Bounds = Bounds;
			args.Cache = new GraphicsCache(gr);
			args.Graphics = gr;
			Rectangle r = ButtonPainter.GetObjectClientRectangle(args);
			EditorButtonObjectInfoArgs ee = args as EditorButtonObjectInfoArgs;
			Size kindSize = ButtonPainter.CalcKindSize(ee);
			Rectangle captionRect = Rectangle.Empty, kindRect = new Rectangle(r.X, r.Y + (r.Height - kindSize.Height) / 2, kindSize.Width, kindSize.Height);
			captionRect = r;
			if(ButtonInfo.Button.ImageToTextAlignment != ImageAlignToText.None) {
				TextBounds = CalcImageAlignedToTextBounds(r, TextSize, kindSize, ButtonInfo.Button.ImageToTextAlignment, TextGlyphIndent, ButtonInfo.Appearance);
				return;
			}
			if(ButtonInfo.Button.Image != null) {
				switch(ee.Button.ImageLocation) {
					case ImageLocation.Default:
					case ImageLocation.MiddleLeft:
						captionRect.Width = r.Width - kindSize.Width - TextGlyphIndent;
						captionRect.X = kindRect.Right + TextGlyphIndent;
						break;
					case ImageLocation.MiddleRight:
						captionRect.X = r.X;
						captionRect.Width = r.Width - kindSize.Width - TextGlyphIndent;
						break;
					case ImageLocation.TopLeft:
					case ImageLocation.TopRight:
					case ImageLocation.TopCenter:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						captionRect.Y = r.Bottom - captionRect.Height;
						break;
					case ImageLocation.BottomCenter:
					case ImageLocation.BottomLeft:
					case ImageLocation.BottomRight:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						break;
				}
			}
			else {
				Size captSize = TextSize;
				if(ee.Button.Image != null) {
					switch(ee.Button.ImageLocation) {
						case ImageLocation.MiddleRight:
							captionRect.X = captionRect.Right - captSize.Width;
							captionRect.Width = captSize.Width;
							break;
						case ImageLocation.MiddleCenter:
						case ImageLocation.Default:
							captionRect.X += (captionRect.Width - captSize.Width) / 2;
							captionRect.Width = captSize.Width;
							break;
					}
				}
			}
			captionRect.Y = captionRect.Y + (captionRect.Height - GetTextAscentHeight()) / 2;
			captionRect.Height = GetTextAscentHeight();
			TextBounds = captionRect;
		}
		protected virtual Rectangle CalcImageAlignedToTextBounds(Rectangle contentBounds, Size textSize, Size imageSize, ImageAlignToText align, int indentBetweenTextAndImage, AppearanceObject textAppearance) {
			return ImageAndTextLayoutHelper.CalcTextBounds(contentBounds, textSize, imageSize, align, indentBetweenTextAndImage, textAppearance);
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
	}
	public class SimpleButtonViewInfo : BaseButtonViewInfo {
		public SimpleButtonViewInfo(SimpleButton owner)
			: base(owner) {
		}
		protected new SimpleButton OwnerControl { get { return base.OwnerControl as SimpleButton; } }
		protected override void UpdateButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.UpdateButton(buttonInfo);
			if(OwnerControl.IsHandleCreated && (OwnerControl.Parent == null || OwnerControl.Parent.IsHandleCreated) && (OwnerControl.TopLevelControl == null || OwnerControl.TopLevelControl.IsHandleCreated))
				buttonInfo.ShowHotKeyPrefix = OwnerControl.ShowKeyboardCuesCore;
			buttonInfo.Indent = new Size(2, 1);
			buttonInfo.Button.Kind = ButtonPredefines.Glyph;
			buttonInfo.Button.Caption = Caption;
			buttonInfo.Button.ImageLocation = OwnerControl.ImageLocation;
			buttonInfo.Button.ImageToTextAlignment = OwnerControl.ImageToTextAlignment;
			buttonInfo.Button.ImageToTextIndent = OwnerControl.ImageToTextIndent;
			buttonInfo.Button.Image = OwnerControl.Image;
			buttonInfo.Button.ImageList = OwnerControl.ImageList;
			buttonInfo.Button.ImageIndex = OwnerControl.ImageIndex;
			buttonInfo.AllowHtmlDraw = AllowHtmlDraw;
			buttonInfo.AllowGlyphSkinning = AllowGlyphSkinning;
			buttonInfo.ShowFocusRectangle = ShowFocusRectangle;
			buttonInfo.DrawFocusRectangle = AllowShowFocusRectangle();
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = base.CreateDefaultAppearance();
			if(OwnerControl.LookAndFeel.ActiveLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP)
				res.BackColor = Color.Transparent;
			return res;
		}
		protected virtual bool AllowHtmlDraw {
			get { return OwnerControl.AllowHtmlDraw == DefaultBoolean.True; }
		}
		protected virtual bool ShowFocusRectangle {
			get { return OwnerControl.ShowFocusRectangle != DefaultBoolean.False; }
		}
		protected virtual bool AllowGlyphSkinning {
			get { return OwnerControl.AllowGlyphSkinning == DefaultBoolean.True; }
		}
		protected internal virtual bool AllowShowFocusRectangle() {
			return ShowFocusRectangle && Focused;
		}
	}
	public class CloseButtonViewInfo : BaseButtonViewInfo {
		public CloseButtonViewInfo(CloseButton owner)
			: base(owner) {
		}
		protected override void UpdateButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.UpdateButton(buttonInfo);
			buttonInfo.Button.Kind = ButtonPredefines.Close;
			buttonInfo.Button.ImageLocation = ImageLocation.MiddleCenter;
			buttonInfo.Button.Caption = "";
			buttonInfo.Button.Image = null;
		}
	}
	public class CheckButtonViewInfo : SimpleButtonViewInfo {
		public CheckButtonViewInfo(CheckButton owner) : base(owner) { }
		protected override bool UpdateObjectState() {
			return base.UpdateObjectState(false);
		}
		public override bool AllowAnimation { get { return false; } }
		protected override EditorButtonPainter GetButtonPainter() {
			if(OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && OwnerControl.ButtonStyle == BorderStyles.Default) return new SkinCheckButtonPainter(OwnerControl.LookAndFeel);
			return base.GetButtonPainter();
		}
	}
	public class DropDownButtonViewInfo : SimpleButtonViewInfo {
		static int defaultArrowButtonWidth = 16;
		public DropDownButtonViewInfo(DropDownButton owner)
			: base(owner) {
		}
		public new DropDownButtonPainter ButtonPainter { get { return this.painter; } }
		public virtual int ArrowButtonWidth { get { return defaultArrowButtonWidth; } }
		protected override EditorButtonObjectInfoArgs CreateEditorButtonObjectInfoArgs(EditorButton editorButton, AppearanceObject appearance) {
			return new DropDownButtonObjectInfoArgs(null, this, editorButton, appearance);
		}
		protected override EditorButtonPainter GetButtonPainter() {
			if(!OwnerControl.IsSplitButtonVisible) {
				return base.GetButtonPainter();
			}
			if(OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && OwnerControl.ButtonStyle == BorderStyles.Default) {
				return new DropDownButtonPainter(new SkinDropDownButtonPainter(OwnerControl.LookAndFeel));
			}
			return new DropDownButtonPainter(EditorButtonHelper.GetPainter(OwnerControl.ButtonStyle, OwnerControl.LookAndFeel));
		}
		protected internal new DropDownButton OwnerControl { get { return base.OwnerControl as DropDownButton; } }
		protected override void UpdateButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.UpdateButton(buttonInfo);
			buttonInfo.DrawDropDownArrow = OwnerControl.GetDropDownArrowStyle() == DropDownArrowStyle.Show;
			if(!buttonInfo.DrawDropDownArrow && buttonInfo is DropDownButtonObjectInfoArgs) {
				((DropDownButtonObjectInfoArgs)buttonInfo).ArrowButtonInfo.Bounds = Rectangle.Empty;
			}
			UpdateArrowButtonState(buttonInfo as DropDownButtonObjectInfoArgs);
		}
		protected internal virtual void UpdateArrowButtonState(DropDownButtonObjectInfoArgs buttonInfo) {
			if(buttonInfo == null) return;
			if(OwnerControl.IsSplitButtonVisible) {
				if(OwnerControl.IsHandleCreated)
				buttonInfo.BaseButtonInfo.ShowHotKeyPrefix = OwnerControl.ShowKeyboardCuesCore;
				buttonInfo.BaseButtonInfo.Indent = new Size(2, 1);
				buttonInfo.BaseButtonInfo.Button.Caption = Caption;
				buttonInfo.BaseButtonInfo.Button.ImageToTextAlignment = OwnerControl.ImageToTextAlignment;
				buttonInfo.BaseButtonInfo.Button.ImageToTextIndent = OwnerControl.ImageToTextIndent;
				buttonInfo.BaseButtonInfo.Button.ImageLocation = OwnerControl.ImageLocation;
				buttonInfo.BaseButtonInfo.Button.Image = OwnerControl.Image;
				buttonInfo.BaseButtonInfo.Button.ImageList = OwnerControl.ImageList;
				buttonInfo.BaseButtonInfo.Button.ImageIndex = OwnerControl.ImageIndex;
				buttonInfo.BaseButtonInfo.AllowHtmlDraw = AllowHtmlDraw;
				buttonInfo.BaseButton.Appearance.Assign(PaintAppearance);
				buttonInfo.ArrowButton.Appearance.Assign(PaintAppearance);
				buttonInfo.BaseButtonInfo.BackAppearance = PaintAppearance;
				buttonInfo.ArrowButtonInfo.BackAppearance = PaintAppearance;
				buttonInfo.BaseButtonInfo.RightToLeft = buttonInfo.RightToLeft;
				buttonInfo.ArrowButtonInfo.RightToLeft = buttonInfo.RightToLeft;
				buttonInfo.BaseButtonInfo.UpdateButtonAppearance();
				buttonInfo.ArrowButtonInfo.UpdateButtonAppearance();
				buttonInfo.BaseButtonInfo.DrawFocusRectangle = AllowShowFocusRectangle();
			}
		}
		protected override bool UpdateObjectState() {
			if(!OwnerControl.IsSplitButtonVisible) return base.UpdateObjectState(false);
			return base.UpdateObjectState();
		}
		public new DropDownButtonObjectInfoArgs ButtonInfo { get { return base.ButtonInfo as DropDownButtonObjectInfoArgs; } }
		protected internal virtual void UpdateArrowButtonState() {
			UpdateArrowButtonState(ButtonInfo);
		}
		public DropDownButtonPainter DropDownButtonPainter { get { return this.painter; } }
		DropDownButtonPainter painter;
		protected override void UpdatePainters() {
			base.UpdatePainters();
			if(!OwnerControl.IsSplitButtonVisible) return;
			painter = this.GetButtonPainter() as DropDownButtonPainter;
		}
		protected internal virtual bool IsButtonEnabled(EditorButton button) {
			return ((button != null) && button.Enabled);
		}
		public bool IsArrowButtonContainsPoint(Point point) {
			EditorButtonObjectInfoArgs info = GetButtonInfoByPoint(point);
			if(info == null) return false;
			return info.Button.Kind == ButtonPredefines.DropDown;
		}
		protected virtual EditorButtonObjectInfoArgs GetButtonInfoByPoint(Point point) {
			DropDownButtonObjectInfoArgs ee = ButtonInfo as DropDownButtonObjectInfoArgs;
			if(ee == null) return null;
			if(ee.ArrowButtonInfo.Bounds.Contains(point)) return ee.ArrowButtonInfo;
			if(ee.BaseButtonInfo.Bounds.Contains(point)) return ee.BaseButtonInfo;
			return null;
		}
		private bool IsEqualsButtons(EditorButtonObjectInfoArgs b1, EditorButtonObjectInfoArgs b2) {
			if((b1 == null) || (b2 == null)) return false;
			return (b1.Button.Kind == b2.Button.Kind);
		}
		public override Size CalcBestFit(Graphics g) {
			if(!OwnerControl.IsSplitButtonVisible) {
				return base.CalcBestFit(g);
			}
			DropDownButtonObjectInfoArgs buttonInfo = new DropDownButtonObjectInfoArgs(null, this, new EditorButton(), null);
			buttonInfo.BuiltIn = false;
			UpdateButton(buttonInfo);
			g = GInfo.AddGraphics(g);
			Size size = new Size(16, 16);
			try {
				buttonInfo.Cache = GInfo.Cache;
				size = ButtonPainter.CalcObjectMinBounds(buttonInfo).Size;
				buttonInfo.Cache = null;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		bool CanUseGeneralHotState {
			get {
				if(OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.UltraFlat || OwnerControl.ButtonStyle == BorderStyles.HotFlat
					|| OwnerControl.ButtonStyle == BorderStyles.UltraFlat) return true;
				return false;
			}
		}
		internal bool IsOwnerDrawnSelectedFrame {
			get {
				if(OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Flat ||
					OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Style3D ||
					OwnerControl.ButtonStyle == BorderStyles.Flat ||
					OwnerControl.ButtonStyle == BorderStyles.Style3D) 
					return true;
				return false;
			}
		}
		protected override internal bool UpdateObjectState(bool checkDesignMode) {
			DropDownButtonObjectInfoArgs ee = ButtonInfo as DropDownButtonObjectInfoArgs;
			if(ee == null || !OwnerControl.IsSplitButtonVisible) {
				return base.UpdateObjectState(checkDesignMode);
			}
			ObjectState prevBaseState = ee.BaseButtonInfo.State;
			ObjectState prevArrowState = ee.ArrowButtonInfo.State;
			ObjectState newBaseState = ObjectState.Normal;
			ObjectState newArrowState = ObjectState.Normal;
			ObjectState prevState = State, newState;
			if(!(IsButtonEnabled(ButtonInfo.Button) && (State != ObjectState.Disabled))) {
				ee.BaseButtonInfo.State = ee.ArrowButtonInfo.State = ee.State = ObjectState.Disabled;
				return ((prevBaseState != ee.BaseButtonInfo.State) || (prevArrowState != ee.ArrowButtonInfo.State));
			}
			Focused = OwnerControl.Focused;
			if(Focused)
				newState = ObjectState.Selected;
			else
				newState = ObjectState.Normal;
			bool inside = Bounds.Contains(MousePosition);
			EditorButtonObjectInfoArgs hotButton = GetButtonInfoByPoint(MousePosition);
			EditorButtonObjectInfoArgs pressedButton = null;
			if(IsPressed || OwnerControl.DownCore) {
				if(OwnerControl.IsArrowButtonDown)
					pressedButton = ee.ArrowButtonInfo;
				else
					pressedButton = ee.BaseButtonInfo;
				ObjectState st = ObjectState.Pressed;
				if(IsEqualsButtons(pressedButton, hotButton)) {
					st |= ObjectState.Hot;
				}
				if(pressedButton == ee.ArrowButtonInfo) {
					if(inside || OwnerControl.DownCore) newArrowState = st;
				}
				else {
					if(inside) newBaseState = st;
				}
			}
			else if(IsEqualsButtons(hotButton, ee.BaseButtonInfo)) {
				newBaseState = ObjectState.Hot;
			}
			else if(IsEqualsButtons(hotButton, ee.ArrowButtonInfo)) {
				newArrowState = ObjectState.Hot;
			}
			if(Focused && IsKeyPressed) {
				if((newBaseState & ObjectState.Pressed) == 0) newBaseState |= ObjectState.Pressed;
			}
			if(Focused && !IsOwnerDrawnSelectedFrame) {
				newBaseState |= ObjectState.Selected;
				newArrowState |= ObjectState.Selected;
			}
			if(CanUseGeneralHotState && inside) {
				if((newArrowState & ObjectState.Hot) == 0) newArrowState |= ObjectState.Hot;
				if((newBaseState & ObjectState.Hot) == 0) newBaseState |= ObjectState.Hot;
			}
			if(prevBaseState != newBaseState) {
				OnBeforeStateChanged(ee.BaseButtonInfo, newBaseState, -10000);
			}
			if(prevArrowState != newArrowState) {
				OnBeforeStateChanged(ee.ArrowButtonInfo, newArrowState, -10001);
			}
			State = newState;
			ee.BaseButtonInfo.State = newBaseState;
			ee.ArrowButtonInfo.State = newArrowState;
			return ((prevBaseState != ee.BaseButtonInfo.State) || (prevArrowState != ee.ArrowButtonInfo.State) || prevState != State);
		}
		protected virtual void OnBeforeStateChanged(EditorButtonObjectInfoArgs infoArgs, ObjectState newState, int id) {
			if(((!this.Bounds.IsEmpty && (((infoArgs.State | newState) & ObjectState.Pressed) == ObjectState.Normal)) && (newState != ObjectState.Pressed)) && this.AllowAnimation) {
				XtraAnimator.Current.AddBitmapAnimation(OwnerControl, id, XtraAnimator.Current.CalcAnimationLength(infoArgs.State, newState), null, new ObjectPaintInfo(painter.ButtonPainter, infoArgs), new BitmapAnimationImageCallback(this.OnGetButtonBitmap));
			}
		}
		internal EditorButtonObjectInfoArgs ButtonInfoById(int buttonId) {
			if(ButtonInfo != null) {
				if(buttonId == -10000) return ButtonInfo.BaseButtonInfo;
				if(buttonId == -10001) return ButtonInfo.ArrowButtonInfo;
			}
			return null;
		}
		Bitmap OnGetButtonBitmap(BaseAnimationInfo info) {
			if(!(info.AnimationId is int)) return null;
			EditorButtonObjectInfoArgs actButton = ButtonInfoById((int)info.AnimationId);
			if(actButton == null) return null;
			return XtraAnimator.Current.CreateBitmap(painter.ButtonPainter, actButton);
		}
	}
	public class DropDownButtonObjectInfoArgs : EditorButtonObjectInfoArgs {
		DropDownButtonViewInfo viewInfo;
		EditorButtonObjectInfoArgs arrowButtonInfo;
		EditorButtonObjectInfoArgs baseButtonInfo;
		public DropDownButtonObjectInfoArgs(DropDownButtonViewInfo viewInfo, EditorButton button, AppearanceObject style)
			: base(button, style) {
		}
		public DropDownButtonObjectInfoArgs(GraphicsCache cache, DropDownButtonViewInfo viewInfo, EditorButton button, AppearanceObject backStyle)
			: base(cache, button, backStyle) {
			this.viewInfo = viewInfo;
			base.SetAppearance(button.Appearance);
			EditorButton baseButton = new EditorButton(ButtonPredefines.Glyph);
			EditorButton arrowButton = new EditorButton(ButtonPredefines.DropDown);
			baseButton.Appearance.Assign(button.Appearance);
			arrowButton.Appearance.Assign(button.Appearance);
			baseButtonInfo = new EditorButtonObjectInfoArgs(baseButton, backStyle);
			arrowButtonInfo = new EditorButtonObjectInfoArgs(arrowButton, backStyle);
			baseButtonInfo.BuiltIn = false;
			this.baseButtonInfo.ParentButtonInfo = this.arrowButtonInfo.ParentButtonInfo = this;
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			DropDownButtonObjectInfoArgs bInfo = info as DropDownButtonObjectInfoArgs;
			if(bInfo != null) {
				this.baseButtonInfo.Assign(bInfo.baseButtonInfo);
				this.arrowButtonInfo.Assign(bInfo.arrowButtonInfo);
				this.viewInfo = bInfo.ViewInfo;
			}
		}
		public override bool Compare(EditorButtonObjectInfoArgs info) {
			if(!base.Compare(info)) {
				return false;
			}
			DropDownButtonObjectInfoArgs e = info as DropDownButtonObjectInfoArgs;
			if(e == null) {
				return false;
			}
			return (this.BaseButtonInfo.Compare(e.BaseButtonInfo) && this.ArrowButtonInfo.Compare(e.ArrowButtonInfo));
		}
		protected override EditorButtonObjectInfoArgs CreateInstance() {
			return new DropDownButtonObjectInfoArgs(this.ViewInfo, this.Button, null);
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			this.BaseButtonInfo.OffsetContent(x, y);
			this.ArrowButtonInfo.OffsetContent(x, y);
		}
		public override GraphicsCache Cache {
			get {
				return base.Cache;
			}
			set {
				base.Cache = value;
				this.BaseButtonInfo.Cache = value;
				this.ArrowButtonInfo.Cache = value;
			}
		}
		public virtual EditorButton BaseButton { get { return this.BaseButtonInfo.Button; } }
		public virtual EditorButton ArrowButton { get { return this.ArrowButtonInfo.Button; } }
		public virtual EditorButtonObjectInfoArgs ArrowButtonInfo { get { return this.arrowButtonInfo; } }
		public virtual EditorButtonObjectInfoArgs BaseButtonInfo { get { return this.baseButtonInfo; } }
		public override bool FillBackground {
			get { return false; }
			set {
				this.BaseButtonInfo.FillBackground = this.ArrowButtonInfo.FillBackground = value;
			}
		}
		public override bool Transparent {
			get { return true; }
			set {
				this.BaseButtonInfo.Transparent = this.ArrowButtonInfo.Transparent = value;
			}
		}
		public DropDownButtonViewInfo ViewInfo { get { return this.viewInfo; } }
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class BaseButtonPainter : BaseControlPainter {
		internal bool allowAnimation = true;
		protected virtual bool DrawAnimated(ControlGraphicsInfoArgs info) {
			if(!allowAnimation) return false;
			BaseButtonViewInfo vi = info.ViewInfo as BaseButtonViewInfo;
			BaseAnimationInfo ani = XtraAnimator.Current.Get(vi.OwnerControl, BaseButtonViewInfo.StateAnimationId);
			if(ani != null) {
				if(!XtraAnimator.Current.DrawFrame(info.Cache, ani)) return false;
				vi.ButtonInfo.Cache = info.Cache;
				vi.ButtonPainter.DrawTextOnly(vi.ButtonInfo);
				vi.ButtonInfo.Cache = null;
				return true;
			}
			return false;
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			BaseButtonViewInfo vi = info.ViewInfo as BaseButtonViewInfo;
			if(DrawAnimated(info)) return;
			if(vi.ButtonInfo.FillBackground)
				vi.PaintAppearance.FillRectangle(info.Cache, vi.Bounds);
			vi.ButtonInfo.Cache = info.Cache;
			vi.ButtonPainter.DrawObject(vi.ButtonInfo);
			vi.ButtonInfo.Cache = null;
		}
	}
}
