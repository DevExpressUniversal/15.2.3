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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraWaitForm {
	[DXToolboxItem(DXToolboxItemKind.Regular)]
	[ToolboxTabName(AssemblyInfo.DXTabCommon)]
	[Description("A control that allows you to indicate the progress of any operation.")]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "ProgressPanel")]
	[Designer("DevExpress.XtraEditors.Design.ProgressPanelDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class ProgressPanel : BaseStyleControl, ITransparentBackgroundManager, ISupportLookAndFeel {
		const string defCaption = "Please Wait";
		const string defDescription = "Loading ...";
		DevExpress.Utils.Animation.DotWaitingIndicatorProperties dotWaitingIndicatorProperties;
		DevExpress.Utils.Animation.LineWaitingIndicatorProperties lineWaitingIndicatorProperties;
		DevExpress.Utils.Animation.RingWaitingIndicatorProperties ringWaitingIndicatorProperties;
		public ProgressPanel() {
			this.appearanceCaptionCore = CreateAppearanceCaption();
			this.appearanceDescriptionCore = CreateAppearanceDescription();
			DoubleBuffered = true;
			SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
			waitingAnimatorTypeCore = Utils.Animation.WaitingAnimatorType.Default;
			dotWaitingIndicatorProperties = new Utils.Animation.DotWaitingIndicatorProperties();
			dotWaitingIndicatorProperties.AppearanceCaption.Assign(AppearanceCaption);
			dotWaitingIndicatorProperties.AppearanceDescription.Assign(AppearanceDescription);
			dotWaitingIndicatorProperties.Appearance.Assign(Appearance);
			lineWaitingIndicatorProperties = new Utils.Animation.LineWaitingIndicatorProperties();
			ringWaitingIndicatorProperties = new Utils.Animation.RingWaitingIndicatorProperties();
			dotWaitingIndicatorProperties.Caption = defCaption;
			dotWaitingIndicatorProperties.Description = defDescription;
			lineWaitingIndicatorProperties.EnsureParentProperties(dotWaitingIndicatorProperties);
			ringWaitingIndicatorProperties.EnsureParentProperties(dotWaitingIndicatorProperties);
			lineWaitingIndicatorProperties.Changed += OnWaitingIndicatorPropertiesChanged;
			ringWaitingIndicatorProperties.Changed += OnWaitingIndicatorPropertiesChanged;
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			AnimationHelper = new LoadingAnimationHelper(this);
			AppearanceCaption.Changed += OnAppearanceCaptionChanged;
			AppearanceDescription.Changed += OnAppearanceDescriptionChanged;
			Appearance.Changed += OnAppearanceChanged;
			dotWaitingIndicatorProperties.AllowBackground = false;
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			dotWaitingIndicatorProperties.Appearance.AssignInternal(Appearance);
			AnimationHelper.SetAnimator();
		}
		void OnAppearanceDescriptionChanged(object sender, EventArgs e) {
			if(ViewInfo != null)
				ViewInfo.UpdateStyle();
			dotWaitingIndicatorProperties.AppearanceDescription.Assign(AppearanceDescription);
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			if(ViewInfo != null)
				ViewInfo.UpdateStyle();
			dotWaitingIndicatorProperties.AppearanceCaption.Assign(AppearanceCaption);
		}
		void OnWaitingIndicatorPropertiesChanged(object sender, EventArgs e) {
			AnimationHelper.SetAnimator();
			Invalidate();
			Update();
		}
		AppearanceDefault defaultAppearanceCaptionCore ;
		internal AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaptionCore == null)
					defaultAppearanceCaptionCore = new AppearanceDefault(Color.Empty, Color.Empty, CaptionDefaultFont);
				return defaultAppearanceCaptionCore;
			}
		}
		AppearanceDefault defaultAppearanceDescriptionCore;
		internal AppearanceDefault DefaultAppearanceDescription {
			get {
				if(defaultAppearanceDescriptionCore == null)
					defaultAppearanceDescriptionCore = new AppearanceDefault(Color.Empty, Color.Empty, DescriptionDefaultFont);
				return defaultAppearanceDescriptionCore;
			}
		}
		protected virtual AppearanceObject CreateAppearanceCaption() {
			return CreateAppearanceCore();
		}
		protected virtual AppearanceObject CreateAppearanceDescription() {
			return CreateAppearanceCore();
		}
		AppearanceObject CreateAppearanceCore() {
			AppearanceObject res = base.CreateAppearance();
			return res;
		}
		internal LoadingAnimationHelper AnimationHelper { get; set; }
		protected override BaseControlPainter CreatePainter() {
			return new ProgressPanelPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new ProgressPanelViewInfo(this);
		}
		protected override Size DefaultSize { get { return new Size(246, 66); } }
		Font captionDefaultFontCore;
		protected virtual Font CaptionDefaultFont {
			get {
				if(captionDefaultFontCore == null)
					captionDefaultFontCore = new Font("Microsoft Sans Serif", 12F);
				return captionDefaultFontCore;
			}
		}
		Font descriptionDefaultFontCore;
		protected virtual Font DescriptionDefaultFont {
			get {
				if(descriptionDefaultFontCore == null)
					descriptionDefaultFontCore = new Font("Microsoft Sans Serif", 8.25F);
				return descriptionDefaultFontCore;
			}
		}
		DevExpress.Utils.Animation.WaitingAnimatorType waitingAnimatorTypeCore;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelWaitAnimationType"),
#endif
 DefaultValue(DevExpress.Utils.Animation.WaitingAnimatorType.Default),
		Category("Behavior")]
		public DevExpress.Utils.Animation.WaitingAnimatorType WaitAnimationType {
			get { return waitingAnimatorTypeCore; }
			set { 
				waitingAnimatorTypeCore = value;
				AnimationHelper.SetAnimator();
				Invalidate();
			}
		}
		internal DevExpress.Utils.Animation.IWaitingIndicatorProperties WaitingIndicatorProperties {
			get {
				if(WaitAnimationType == Utils.Animation.WaitingAnimatorType.Line)
					return lineWaitingIndicatorProperties;
				if(WaitAnimationType == Utils.Animation.WaitingAnimatorType.Ring)
					return ringWaitingIndicatorProperties;
				return null;
			}
		}
		internal DevExpress.Utils.Animation.DotWaitingIndicatorProperties GetDotWaitingindicatorProperties() {
			return dotWaitingIndicatorProperties;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAnimationAcceleration"),
#endif
 DefaultValue((float)7.0), Category("Behavior")]
		public float AnimationAcceleration {
			get { return dotWaitingIndicatorProperties.AnimationAcceleration; }
			set { dotWaitingIndicatorProperties.AnimationAcceleration = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAnimationSpeed"),
#endif
 DefaultValue((float)5.5), Category("Behavior")]
		public float AnimationSpeed {
			get { return dotWaitingIndicatorProperties.AnimationSpeed; }
			set { dotWaitingIndicatorProperties.AnimationSpeed = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelFrameCount"),
#endif
 DefaultValue(38000), Category("Behavior")]
		public int FrameCount {
			get { return dotWaitingIndicatorProperties.FrameCount; }
			set { dotWaitingIndicatorProperties.FrameCount = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelFrameInterval"),
#endif
 DefaultValue(1000), Category("Behavior")]
		public int FrameInterval {
			get { return dotWaitingIndicatorProperties.FrameInterval; }
			set { dotWaitingIndicatorProperties.FrameInterval = value; }
		}
		[ DefaultValue(40), Category("Behavior")]
		public int RingAnimationDiameter {
			get { return ringWaitingIndicatorProperties.RingAnimationDiameter; }
			set { ringWaitingIndicatorProperties.RingAnimationDiameter = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAnimationElementImage"),
#endif
 DefaultValue(null), Category("Behavior")]
		public Image AnimationElementImage {
			get { return dotWaitingIndicatorProperties.AnimationElementImage; }
			set { dotWaitingIndicatorProperties.AnimationElementImage = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelLineAnimationElementHeight"),
#endif
 DefaultValue(10), Category("Behavior")]
		public int LineAnimationElementHeight {
			get { return lineWaitingIndicatorProperties.LineAnimationElementHeight; }
			set { lineWaitingIndicatorProperties.LineAnimationElementHeight = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelLineAnimationElementType"),
#endif
 DefaultValue(DevExpress.Utils.Animation.LineAnimationElementType.Circle), Category("Behavior")]
		public DevExpress.Utils.Animation.LineAnimationElementType LineAnimationElementType {
			get { return lineWaitingIndicatorProperties.LineAnimationElementType; }
			set { lineWaitingIndicatorProperties.LineAnimationElementType = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelCaption"),
#endif
 Category("Display Options"), DefaultValue(defCaption), Localizable(true), SmartTagProperty("Caption", "", SmartTagActionType.RefreshAfterExecute)]
		public string Caption {
			get { return dotWaitingIndicatorProperties.Caption; }
			set {
				dotWaitingIndicatorProperties.Caption = value;
				ChangeAutoSize();
			}
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			ChangeAutoSize();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelDescription"),
#endif
 Category("Display Options"), DefaultValue(defDescription), Localizable(true), SmartTagProperty("Description", "", SmartTagActionType.RefreshAfterExecute)]
		public string Description {
			get { return dotWaitingIndicatorProperties.Description; }
			set {
				dotWaitingIndicatorProperties.Description = value;
				ChangeAutoSize();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelShowCaption"),
#endif
 Category("Display Options"), DefaultValue(true), SmartTagProperty("Show Caption", "", SmartTagActionType.RefreshAfterExecute)]
		public bool ShowCaption {
			get { return dotWaitingIndicatorProperties.ShowCaption; }
			set {
				if(ShowCaption != value) {
					dotWaitingIndicatorProperties.ShowCaption = value;
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelShowDescription"),
#endif
 Category("Display Options"), DefaultValue(true), SmartTagProperty("Show Description", "", SmartTagActionType.RefreshAfterExecute)]
		public bool ShowDescription {
			get { return dotWaitingIndicatorProperties.ShowDescription; }
			set {
				if(ShowDescription != value) {
					dotWaitingIndicatorProperties.ShowDescription = value;
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelTextHorzOffset"),
#endif
 Category("Display Options"), DefaultValue(8),
		Obsolete("The TextHorzOffset property is now obsolete. Use the AnimationToTextDistance property instead."), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TextHorzOffset {
			get { return dotWaitingIndicatorProperties.ImageToTextDistance; }
			set {
				if(value < 0) throw new ArgumentException("TextHorzOffset");
				dotWaitingIndicatorProperties.ImageToTextDistance = value;
			}
		}
		[ Category("Behavior"), DefaultValue(5)]
		public int AnimationElementCount {
			get { return dotWaitingIndicatorProperties.AnimationElementCount; }
			set { dotWaitingIndicatorProperties.AnimationElementCount = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAnimationToTextDistance"),
#endif
 Category("Display Options"), DefaultValue(8)]
		public int AnimationToTextDistance {
			get { return dotWaitingIndicatorProperties.ImageToTextDistance; }
			set { dotWaitingIndicatorProperties.ImageToTextDistance = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelImageHorzOffset"),
#endif
 Category("Display Options"), DefaultValue(0)]
		public int ImageHorzOffset {
			get { return dotWaitingIndicatorProperties.ImageOffset; }
			set {
				if(value < 0) throw new ArgumentException("ImageOffset");
				if(dotWaitingIndicatorProperties.ImageOffset != value) {
					dotWaitingIndicatorProperties.ImageOffset = value;
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelCaptionToDescriptionDistance"),
#endif
 Category("Display Options"), DefaultValue(0)]
		public int CaptionToDescriptionDistance {
			get { return dotWaitingIndicatorProperties.CaptionToDescriptionDistance; }
			set {
				if(value < 0) throw new ArgumentException("CaptionToDescriptionDistance");
				dotWaitingIndicatorProperties.CaptionToDescriptionDistance = value;
			}
		}
		bool autoHeight;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAutoHeight"),
#endif
 DefaultValue(false), Category("Layout")]
		public bool AutoHeight {
			get { return this.autoHeight; }
			set {
				if(AutoHeight == value) return;
				this.autoHeight = value;
				ChangeAutoSize();
			}
		}
		bool autoWidth;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAutoWidth"),
#endif
 DefaultValue(false), Category("Layout")]
		public bool AutoWidth {
			get { return this.autoWidth; }
			set {
				if(AutoWidth == value) return;
				this.autoWidth = value;
				ChangeAutoSize();
			}
		}
		protected internal void ForceUpdateBestSize() {
			ChangeAutoSize();
		}
		protected virtual void ChangeAutoSize() {
			Height = ViewInfo.Height;
			Width = ViewInfo.Width;
			Invalidate();
		}
		public override Size GetPreferredSize(Size proposedSize) {
			if(this.AutoSize)
				return ViewInfo.PreferredSize;
			return base.GetPreferredSize(proposedSize);
		}
		new ProgressPanelViewInfo ViewInfo { get { return base.ViewInfo as ProgressPanelViewInfo; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelImageSize"),
#endif
 Browsable(false)]
		public Size ImageSize {
			get {
				if(AnimationHelper.Image != null)
					return AnimationHelper.Image.Size;
				return Size.Empty;
			}
		}
		AppearanceObject appearanceCaptionCore = null;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAppearanceCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public AppearanceObject AppearanceCaption { get { return appearanceCaptionCore; } set { appearanceCaptionCore = value; } }
		AppearanceObject appearanceDescriptionCore = null;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelAppearanceDescription"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public AppearanceObject AppearanceDescription { get { return appearanceDescriptionCore; } set { appearanceDescriptionCore = value; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressPanelBorderStyle"),
#endif
 DefaultValue(BorderStyles.Default)]
		public override BorderStyles BorderStyle {
			get { 
				if(BaseBorderStyle == BorderStyles.Default) {
					if(StyleController != null && StyleController.BorderStyle != BorderStyles.Default)
						return StyleController.BorderStyle;
				}
				return BaseBorderStyle;
			}
			set { base.BorderStyle = value; }
		}
		[ Category("Behavior"), DefaultValue(ContentAlignment.MiddleLeft)]
		public ContentAlignment ContentAlignment {
			get { return dotWaitingIndicatorProperties.ContentAlignment; }
			set { dotWaitingIndicatorProperties.ContentAlignment = value; }
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ViewInfo.UpdateOriginalSize(Size);
		}
		#region Handlers
		protected virtual void AdjustSize() {
			if(this.AutoSize)
				this.PerformLayout();
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			if(ViewInfo != null)
				ViewInfo.UpdateStyle();
			AnimationHelper.Reset();
			AnimationHelper.SetAnimator();
			AdjustSize();
			Invalidate();
		}
		#endregion
		#region ITransparentBackgroundManager
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return GetForeColorCore();
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return GetForeColorCore();
		}
		Color GetForeColorCore() {
			return LookAndFeelHelper.GetTransparentForeColor(LookAndFeel, this);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			LookAndFeel.StyleChanged -= LookAndFeel_StyleChanged;
			if(disposing) {
				if(AnimationHelper != null)
					AnimationHelper.Dispose();
				AnimationHelper = null;
				if(lineWaitingIndicatorProperties != null)
					lineWaitingIndicatorProperties.Changed -= OnWaitingIndicatorPropertiesChanged;
				if(ringWaitingIndicatorProperties != null)
					ringWaitingIndicatorProperties.Changed -= OnWaitingIndicatorPropertiesChanged;
				if(Appearance != null)
					Appearance.Changed -= OnAppearanceChanged;
				if(AppearanceCaption != null) {
					AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
					DestroyAppearance(AppearanceCaption);
				}
				AppearanceCaption = null;
				if(AppearanceDescription != null) {
					AppearanceDescription.Changed -= OnAppearanceDescriptionChanged;
					DestroyAppearance(AppearanceDescription);
				}
				AppearanceDescription = null;
				if(captionDefaultFontCore != null)
					captionDefaultFontCore.Dispose();
				captionDefaultFontCore = null;
				if(descriptionDefaultFontCore != null)
					descriptionDefaultFontCore.Dispose();
				descriptionDefaultFontCore = null;
			}
			base.Dispose(disposing);
		}
	}
	class LoadingAnimationHelper : IDisposable {
		public LoadingAnimationHelper(ProgressPanel progressPanel) {
			ProgressPanel = progressPanel;
		}
		~LoadingAnimationHelper() { Dispose(false); }
		DevExpress.Utils.Animation.BaseLoadingAnimator animatorCore = null;
		DevExpress.Utils.Animation.BaseWaitingIndicatorInfo customIndicatorInfo;
		public DevExpress.Utils.Animation.BaseLoadingAnimator Animator {
			get {
				if(this.animatorCore == null) {
					this.animatorCore = new LoadingAnimator(ProgressPanel, Image);
				}
				return this.animatorCore;
			}
		}
		public DevExpress.Utils.Animation.BaseWaitingIndicatorInfo CustomIndicatorInfo {
			get { return customIndicatorInfo; }
		}
		internal void SetAnimator() {
			if(animatorCore != null)
				animatorCore.StopAnimation();
			if(animatorCore is DevExpress.Utils.Animation.DotAnimator)
				(animatorCore as DevExpress.Utils.Animation.DotAnimator).Invalidate -= OnInvalidate;
			if(ProgressPanel.WaitAnimationType == Utils.Animation.WaitingAnimatorType.Line) {
				customIndicatorInfo = new DevExpress.Utils.Animation.LineAnimatorInfo(ProgressPanel.WaitingIndicatorProperties, ProgressPanel.LookAndFeel);
				this.animatorCore = customIndicatorInfo.WaitingAnimator as DevExpress.Utils.Animation.BaseLoadingAnimator;
				(animatorCore as DevExpress.Utils.Animation.LineAnimator).Invalidate += OnInvalidate;
				animatorCore.StartAnimation();
				return;
			}
			if(ProgressPanel.WaitAnimationType == Utils.Animation.WaitingAnimatorType.Ring) {
				customIndicatorInfo = new DevExpress.Utils.Animation.RingAnimatorInfo(ProgressPanel.WaitingIndicatorProperties, ProgressPanel.LookAndFeel);
				this.animatorCore = customIndicatorInfo.WaitingAnimator as DevExpress.Utils.Animation.BaseLoadingAnimator;
				(animatorCore as DevExpress.Utils.Animation.DotAnimator).Invalidate += OnInvalidate;
				animatorCore.StartAnimation();
				return;
			}
			this.animatorCore = new LoadingAnimator(ProgressPanel, Image);
			customIndicatorInfo = null;
			animatorCore.StartAnimation();
		}
		void OnInvalidate(object sender, EventArgs e) {
			ProgressPanel.Invalidate((animatorCore as DevExpress.Utils.Animation.DotAnimator).Bounds);
		}
		Image imageCore = null;
		public Image Image {
			get {
				if(this.imageCore != null) return this.imageCore;
				SkinElement element = CommonSkins.GetSkin(ProgressPanel.LookAndFeel)[CommonSkins.SkinLoadingBig];
				if(element != null && element.Image != null && element.Image.Image != null)
					this.imageCore = element.Image.Image;
				else this.imageCore = LoadingAnimator.LoadingImageBig;
				return this.imageCore;
			}
		}
		public ProgressPanel ProgressPanel { get; set; }
		public void Reset() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(animatorCore != null) {
					animatorCore.StopAnimation();
					animatorCore.Dispose();
				}
			}
			imageCore = null;
			animatorCore = null;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	class ProgressPanelPainterBase : BaseControlPainter {	}
	class ProgressPanelPainter : ProgressPanelPainterBase {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			if(viewInfo.OwnerControl.AnimationHelper.Animator is DevExpress.Utils.Animation.DotAnimator) {
				ProgressPanel panel = viewInfo.OwnerControl;
				ApplyForeColor(viewInfo, panel);
				DrawCustomAnimation(info);
				return;
			}
			DrawAnimation(info);
			DrawCaption(info);
			DrawDescription(info);
		}
		protected virtual void ApplyForeColor(ProgressPanelViewInfo viewInfo, ProgressPanel panel) {
			DevExpress.Utils.Animation.DotWaitingIndicatorProperties dotWaitingIndicatorProperties = panel.GetDotWaitingindicatorProperties();
			dotWaitingIndicatorProperties.Appearance.ForeColor = panel.Appearance.ForeColor == Color.Empty ? viewInfo.GetForecolor() : panel.Appearance.ForeColor;
			dotWaitingIndicatorProperties.AppearanceCaption.ForeColor = panel.AppearanceCaption.ForeColor == Color.Empty ? viewInfo.GetForecolor() : panel.AppearanceCaption.ForeColor;
			dotWaitingIndicatorProperties.AppearanceDescription.ForeColor = panel.AppearanceDescription.ForeColor == Color.Empty ? viewInfo.GetForecolor() : panel.AppearanceDescription.ForeColor;
		}
		protected virtual void DrawCustomAnimation(ControlGraphicsInfoArgs info) {
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			ObjectInfoArgs e = new ObjectInfoArgs(info.Cache, viewInfo.Bounds, ObjectState.Normal);
			viewInfo.OwnerControl.AnimationHelper.CustomIndicatorInfo.Painter.DrawObject(e);
		}
		Point GetOffset(ControlGraphicsInfoArgs info) {
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			ProgressPanel panel = viewInfo.OwnerControl;
			ContentAlignment contentAlignment = panel.ContentAlignment;
			int captionBoundsRight = panel.ShowCaption ? viewInfo.GetCaptionBounds(info).Right : viewInfo.GetDescriptionBounds(info).Right;
			int descriptionBoundsRight = panel.ShowDescription ? viewInfo.GetDescriptionBounds(info).Right : viewInfo.GetCaptionBounds(info).Right;
			int totalWidth = viewInfo.ImageBounds.Width;
			if(panel.ShowCaption || panel.ShowDescription)
				totalWidth = (int)Math.Max(captionBoundsRight, descriptionBoundsRight) - viewInfo.ImageBounds.Left;
			int totalHeight = (int)Math.Max(viewInfo.GetDescriptionBounds(info).Bottom, viewInfo.ImageBounds.Bottom) - (int)Math.Max(viewInfo.GetCaptionBounds(info).Top, viewInfo.ImageBounds.Top);
			Size totalSize = new Size(totalWidth, totalHeight);
			Point location = PlacementHelper.Arrange(totalSize, panel.Bounds, contentAlignment).Location;
			return new Point(location.X - panel.Location.X, location.Y - panel.Location.Y);
		}
		protected virtual void DrawAnimation(ControlGraphicsInfoArgs info) {
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			Rectangle rect = viewInfo.ImageBounds;
			rect.Offset(GetOffset(info));
			viewInfo.OwnerControl.AnimationHelper.Animator.DrawAnimatedItem(info.Cache, rect);
		}
		protected virtual void DrawCaption(ControlGraphicsInfoArgs info) {
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			if(!viewInfo.ShouldDrawCaption)
				return;
			Rectangle rect = viewInfo.GetCaptionBounds(info);
			rect.Offset(GetOffset(info));
			FrozenAppearance PaintAppearance = new FrozenAppearance();
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { viewInfo.OwnerControl.AppearanceCaption }, viewInfo.OwnerControl.DefaultAppearanceCaption);
			DrawStringCore(PaintAppearance, info, viewInfo.Caption, rect);
		}
		protected virtual void DrawDescription(ControlGraphicsInfoArgs info) {
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			if(!viewInfo.ShouldDrawDescription) return;
			Rectangle rect = viewInfo.GetDescriptionBounds(info);
			rect.Offset(GetOffset(info));
			FrozenAppearance PaintAppearance = new FrozenAppearance();
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { viewInfo.OwnerControl.AppearanceDescription} , viewInfo.OwnerControl.DefaultAppearanceDescription);
			DrawStringCore(PaintAppearance, info, viewInfo.Description, rect);
		}
		void DrawStringCore(AppearanceObject appearance, ControlGraphicsInfoArgs info, string text, Rectangle bounds) {
			ProgressPanelViewInfo viewInfo = (ProgressPanelViewInfo)info.ViewInfo;
			using(StringFormat format = appearance.GetStringFormat().Clone() as StringFormat) {
				appearance.DrawString(info.Cache, text, bounds, viewInfo.GetLabelForeBrush(appearance), format);
			}
		}
	}
	class ProgressPanelViewInfoBase : BaseStyleControlViewInfo {
		public ProgressPanelViewInfoBase(BaseStyleControl control) : base(control) { }
		protected Size CalcStringSizeCore(string value, Font font) {
			return CalcStringSizeCore(value, font, StringFormat.GenericDefault, 0);
		}
		protected Size CalcStringSizeCore(string value, Font font, StringFormat format, int maxWidth) {
			var g = GraphicsInfo.Default.AddGraphics(null);
			try {
				return TextUtils.GetStringSize(g, value, font, format, maxWidth);
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		public new ProgressPanel OwnerControl { get { return base.OwnerControl as ProgressPanel; } }
	}
	class ProgressPanelViewInfo : ProgressPanelViewInfoBase {
		Size originalSize;
		public ProgressPanelViewInfo(BaseStyleControl control) : base(control) {
			originalSize = OwnerControl.Size;
		}
		public Rectangle ClientRectCore {
			get {
				Rectangle rect = ClientRect;
				rect.X += OwnerControl.Padding.Left;
				rect.Y += OwnerControl.Padding.Top;
				rect.Width = Math.Max(rect.Width - OwnerControl.Padding.Left - OwnerControl.Padding.Right, 0);
				rect.Height = Math.Max(rect.Height - OwnerControl.Padding.Top - OwnerControl.Padding.Bottom, 0);
				return rect;
			}
		}
		protected internal virtual void UpdateOriginalSize(Size size) {
			if(!OwnerControl.AutoWidth) originalSize.Width = size.Width;
			if(!OwnerControl.AutoHeight) originalSize.Height = size.Height;
		}
		protected override BorderPainter GetBorderPainterCore() {
			if(OwnerControl == null || OwnerControl.BorderStyle == BorderStyles.Default)
				return new EmptyBorderPainter();
			return base.GetBorderPainterCore();
		}
		public Point GetCaptionCorner(Graphics g) {
			int yOffset = 0;
			if(!ShouldDrawDescription) {
				yOffset = ImageHeight >= SizeCaption.Height ? ImageHeight / 2 - SizeCaption.Height / 2 : 0;
				return new Point(TextOffsetX, yOffset);
			}
			int totalHeight = SizeCaption.Height + SizeDescription.Height + OwnerControl.CaptionToDescriptionDistance;
			yOffset = ImageHeight >= totalHeight ? (int)Math.Round((double)ImageHeight / 2 - totalHeight / 2) : 0;
			return new Point(TextOffsetX, ClientRectCore.Top + yOffset);
		}
		public Point GetDescriptionCorner(Graphics g) {
			int yOffset = 0;
			if(!ShouldDrawCaption) {
				yOffset = ImageHeight >= SizeDescription.Height ? ImageHeight / 2 - SizeDescription.Height / 2 : 0;
				return new Point(TextOffsetX, yOffset);
			}
			Point pt = GetCaptionCorner(g);
			return new Point(TextOffsetX, pt.Y + SizeCaption.Height + OwnerControl.CaptionToDescriptionDistance);
		}
		public Rectangle GetCaptionBounds(ControlGraphicsInfoArgs info) {
			Rectangle rect = new Rectangle(GetCaptionCorner(info.Graphics), SizeCaption);
			return Rectangle.Intersect(ClientRectCore, rect);
		}
		public Rectangle GetDescriptionBounds(ControlGraphicsInfoArgs info) {
			return Rectangle.Intersect(ClientRectCore, CalcDescriptionClipRectangle(info));
		}
		public string Caption { get { return OwnerControl.Caption; } }
		public string Description { get { return OwnerControl.Description; } }
		public bool ShouldDrawCaption { get { return OwnerControl.ShowCaption && OwnerControl.Caption.Length > 0; } }
		public bool ShouldDrawDescription { get { return OwnerControl.ShowDescription && OwnerControl.Description.Length > 0; } }
		public void UpdateStyle() {
			captionPaintAppearanceCore = null;
			descriptionPaintAppearance = null;
		}
		FrozenAppearance captionPaintAppearanceCore;
		public Font CaptionFont {
			get {
				if(captionPaintAppearanceCore == null) {
					captionPaintAppearanceCore = new FrozenAppearance();
					AppearanceHelper.Combine(captionPaintAppearanceCore, new AppearanceObject[] { OwnerControl.AppearanceCaption }, OwnerControl.DefaultAppearanceCaption);
				}
				return captionPaintAppearanceCore.Font;
			}
		}
		FrozenAppearance descriptionPaintAppearance;
		public Font DescriptionFont { 
			get {
				if(descriptionPaintAppearance == null) {
					descriptionPaintAppearance = new FrozenAppearance();
					AppearanceHelper.Combine(descriptionPaintAppearance, new AppearanceObject[] { OwnerControl.AppearanceDescription }, OwnerControl.DefaultAppearanceDescription);
				}
				return descriptionPaintAppearance.Font;
			} 
		}
		public Brush GetLabelForeBrush(AppearanceObject appearance) {
			if(appearance.ForeColor.IsEmpty)
				return TextForeground;
			return new SolidBrush(appearance.ForeColor);
		}
		protected Rectangle CalcDescriptionClipRectangle(ControlGraphicsInfoArgs info) {
			Point pt = GetDescriptionCorner(info.Graphics);
			return new Rectangle(pt, CalcDescriptionClipRectSize(pt));
		}
		protected Size CalcDescriptionClipRectSize(Point topLeftPt) {
			Size size = Size.Empty;
			AppearanceObject appearance = OwnerControl.AppearanceDescription;
			using(StringFormat format = appearance.GetStringFormat().Clone() as StringFormat) {
				size = CalcStringSizeCore(Description, DescriptionFont, format, Math.Max(OwnerControl.Width - topLeftPt.X, 0));
			}
			return size;
		}
		protected internal Size SizeCaption { get { return CalcStringSizeCore(Caption, CaptionFont); } }
		protected internal Size SizeDescription { get { return CalcStringSizeCore(Description, DescriptionFont); } }
		[Obsolete("Use PreferredSize"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Size PrefferedSize {
			get {
				return PreferredSize;
			}
		}
		public Size PreferredSize { 
			get {
				return CalcPreferredSize();
			} 
		}
		Size CalcPreferredSize() {
			int height = OwnerControl.AutoHeight ? PreferredHeight : OriginalHeight;
			int width = OwnerControl.AutoWidth ? PreferredWidth : OriginalWidth;
			return new Size(width, height);
		}
		public int PreferredHeight { get { return Math.Max(ImageHeight, SizeCaption.Height + SizeDescription.Height + OwnerControl.CaptionToDescriptionDistance) + OwnerControl.Padding.Top + OwnerControl.Padding.Bottom + 2; } }
		public int PreferredWidth { get { return Math.Max(SizeCaption.Width, SizeDescription.Width) + ImageWidth + OwnerControl.ImageHorzOffset + OwnerControl.AnimationToTextDistance + OwnerControl.Padding.Left + OwnerControl.Padding.Right + 2; } }
		public int OriginalWidth { get { return originalSize.Width; } }
		public int OriginalHeight { get { return originalSize.Height; } }
		public int Height { get { return OwnerControl.AutoHeight ? PreferredSize.Height : OriginalHeight; } }
		public int Width { get { return OwnerControl.AutoWidth ? PreferredSize.Width : OriginalWidth; } }
		Brush TextForeground {
			get {
				ITransparentBackgroundManager manager = ITransparentBackgroundManagerImplementer;
				return new SolidBrush(manager.GetForeColor(OwnerControl));
			}
		}
		internal Color GetForecolor() {
			ITransparentBackgroundManager manager = ITransparentBackgroundManagerImplementer;
			return manager.GetForeColor(OwnerControl);
		}
		ITransparentBackgroundManager ITransparentBackgroundManagerImplementer {
			get {
				ITransparentBackgroundManager parentForm = OwnerControl.FindForm() as ITransparentBackgroundManager;
				return parentForm ?? OwnerControl;
			}
		}
		protected virtual int TextOffsetX { 
			get { return ImageWidth + OwnerControl.ImageHorzOffset + OwnerControl.AnimationToTextDistance + ClientRectCore.Left; }
		}
		public Rectangle ImageBounds {
			get {
				int totalHeight = SizeCaption.Height + SizeDescription.Height + OwnerControl.CaptionToDescriptionDistance;
				int yOffset = ImageHeight >= totalHeight ? 0 : (int)Math.Round((double)totalHeight / 2 - ImageHeight / 2);
				Rectangle rect = new Rectangle(OwnerControl.ImageHorzOffset + ClientRectCore.Left, yOffset + ClientRectCore.Top, ImageWidth, ImageHeight);
				return Rectangle.Intersect(rect, ClientRectCore);
			}
		}
		protected internal virtual int ImageWidth { get { return OwnerControl.AnimationHelper.Image.Size.Width; } }
		protected internal virtual int ImageHeight { get { return OwnerControl.AnimationHelper.Image.Size.Height; } }
		protected int ControlHeight { get { return ClientRectCore.Height; } }
	}
}
