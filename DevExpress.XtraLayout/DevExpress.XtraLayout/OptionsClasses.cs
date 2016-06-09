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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Customization;
using DXUtils = DevExpress.Utils;
namespace DevExpress.XtraLayout {
	public enum AutoSizeModes {
		ResizeToMinSize = 0,
		UseMinSizeAndGrow = 1,
		UseMinAndMaxSize = 2
	}
	public enum DirectionNew { LeftToRightTopToBottom, RightToLeftTopToBottom, TopToBottomLefToRight, TopToBottomRightToLeft };
	public enum MoveFocusDirection { AcrossThenDown, DownThenAcross };
	public enum AutoAlignMode { AlignLocal, AlignGlobal, AutoSize };
	public enum TextAlignMode { AlignInLayoutControl, AlignInGroups, 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		AlignInGroupsRecursive, 
		AutoSize, CustomSize };
	public enum TextAlignModeGroup { UseParentOptions, AlignLocal, AutoSize, CustomSize, AlignWithChildren };
	public enum TextAlignModeItem { UseParentOptions, AutoSize, CustomSize };
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BaseLayoutOptions {
		protected ILayoutControl owner;
		public BaseLayoutOptions(ILayoutControl owner) {
			this.owner = owner;
		}
		protected virtual bool StartChange() {
			if(owner == null) return false;
			return owner.FireChanging(owner as IComponent);
		}
		protected virtual void EndChange() {
			if(owner == null) return;
			owner.FireChanged(owner as IComponent);
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
	public class OptionsPrintControl : BaseLayoutOptions {
		public OptionsPrintControl(ILayoutControl owner) : base(owner) {
			printAppearanceItem = PrintAppearanceCreator.ItemCaptionAppearance();
			printAppearanceGroup = PrintAppearanceCreator.GroupCaptionAppearance();
		}
		int textToControlDistance = -1;
		[ DefaultValue(-1)]
		[XtraSerializableProperty()]
		public int TextToControlDistance {
			get { return textToControlDistance; }
			set { if(value < -1) return; textToControlDistance = value; }
		}
		AppearanceObject printAppearanceItem,printAppearanceGroup;
		bool oldPrinting = false;
		[ DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool OldPrinting {
			get { return oldPrinting; }
			set { 
				oldPrinting = value;
				if(owner is LayoutControl) {
					(owner as LayoutControl).UpdateLayoutControlPrinter();
				}
			}
		}
		bool ShouldSerializeAppearanceItemCaption() { return !AppearanceItemCaption.IsEqual(PrintAppearanceCreator.ItemCaptionAppearance()); }
		void ResetAppearanceItemCaption() { printAppearanceItem = PrintAppearanceCreator.ItemCaptionAppearance(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceItemCaption {
			get { return printAppearanceItem; }
		}
		bool ShouldSerializeAppearanceGroupCaption() { return !AppearanceGroupCaptionEquals(AppearanceGroupCaption, PrintAppearanceCreator.GroupCaptionAppearance()); }
		bool AppearanceGroupCaptionEquals(AppearanceObject realAppearance, AppearanceObject compareToAppearance) {
			return (compareToAppearance.Options ?? AppearanceOptions.Empty).IsEqual(realAppearance.Options ?? AppearanceOptions.Empty) &&
				 realAppearance.ForeColor == compareToAppearance.ForeColor &&
				 realAppearance.BorderColor == compareToAppearance.BorderColor &&
				 realAppearance.BackColor == compareToAppearance.BackColor &&
				 realAppearance.BackColor2 == compareToAppearance.BackColor2 &&
				 realAppearance.GradientMode == compareToAppearance.GradientMode &&
				 realAppearance.Image == compareToAppearance.Image &&
				 IsEqualsFont(realAppearance.Font,compareToAppearance.Font) &&
				 (compareToAppearance.TextOptions ?? TextOptions.DefaultOptions).IsEqual(realAppearance.TextOptions ?? TextOptions.DefaultOptions);
		}
		bool IsEqualsFont(Font font1, Font font2) {
			return font1.Equals(font2);
		}
		void ResetAppearanceGroupCaption() { printAppearanceGroup = PrintAppearanceCreator.GroupCaptionAppearance(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceGroupCaption {
			get { return printAppearanceGroup; }
		}
		bool allowFitOnePageCore = true;
		[DefaultValue(true)]
		[XtraSerializableProperty()]
		public virtual bool AllowFitToPage {
			get { return allowFitOnePageCore; }
			set {
				if(!StartChange()) return;
				if(allowFitOnePageCore == value) return;
				this.allowFitOnePageCore = value;
				EndChange();
			}
		}
	}
	public class OptionsFocus : BaseLayoutOptions {
		bool enableAutoTabOrder = true;
		bool allowFocusGroups = true;
		bool allowFocusTabbedGroups = true;
		bool allowFocusReadOnlyEditorsCore = true;
		bool allowFocusControlOnActivatedTabPage = false;
		bool allowFocusControlOnLabelClick = false;
		bool activateSelectedControlOnGotFocus = true;
		MoveFocusDirection moveFocusDirection;
		bool moveFocusRightToLeft;
		public OptionsFocus(ILayoutControl owner) : base(owner) { }
		public OptionsFocus(MoveFocusDirection direction, bool rightToLeft)
			: this(null) {
			moveFocusDirection = direction;
			moveFocusRightToLeft = rightToLeft;
		}
		protected override void EndChange() {
			if(owner == null) return;
			owner.ShouldArrangeTextSize = true;
			owner.Invalidate();
			base.EndChange();
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusActivateSelectedControlOnGotFocus"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool ActivateSelectedControlOnGotFocus {
			get { return activateSelectedControlOnGotFocus; }
			set {
				if(!StartChange()) return;
				activateSelectedControlOnGotFocus = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusAllowFocusControlOnLabelClick"),
#endif
 DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool AllowFocusControlOnLabelClick {
			get { return allowFocusControlOnLabelClick; }
			set {
				if(!StartChange()) return;
				allowFocusControlOnLabelClick = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusEnableAutoTabOrder"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool EnableAutoTabOrder {
			get { return enableAutoTabOrder; }
			set {
				if(!StartChange()) return;
				enableAutoTabOrder = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusAllowFocusControlOnActivatedTabPage"),
#endif
 DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool AllowFocusControlOnActivatedTabPage {
			get { return allowFocusControlOnActivatedTabPage; }
			set {
				if(!StartChange()) return;
				allowFocusControlOnActivatedTabPage = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusAllowFocusGroups"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool AllowFocusGroups {
			get { return allowFocusGroups; }
			set {
				if(!StartChange()) return;
				allowFocusGroups = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusAllowFocusTabbedGroups"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool AllowFocusTabbedGroups {
			get { return allowFocusTabbedGroups; }
			set {
				if(!StartChange()) return;
				allowFocusTabbedGroups = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusAllowFocusReadonlyEditors"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool AllowFocusReadonlyEditors {
			get { return allowFocusReadOnlyEditorsCore; }
			set {
				if(!StartChange()) return;
				allowFocusReadOnlyEditorsCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusMoveFocusRightToLeft"),
#endif
 DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool MoveFocusRightToLeft {
			get { return moveFocusRightToLeft; }
			set {
				if(!StartChange()) return;
				moveFocusRightToLeft = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsFocusMoveFocusDirection"),
#endif
 DefaultValue(MoveFocusDirection.AcrossThenDown)]
		[XtraSerializableProperty()]
		public MoveFocusDirection MoveFocusDirection {
			get { return moveFocusDirection; }
			set {
				if(!StartChange()) return;
				moveFocusDirection = value;
				EndChange();
			}
		}
	}
	public class OptionsCustomizationForm : BaseLayoutOptions {
		bool showLoadButton;
		bool showSaveButton;
		bool showUndoButton;
		bool showRedoButton;
		bool showLayoutTreeView;
		bool enableUndoManager;
		bool showProperties;
		string defaultSaveDir, defaultRestoreDir;
		bool allowHandleDeleteKeyCore;
		Rectangle? designTimeCustomizationFormBoundsCore = null;
		int quickModeInitDelay;
		int quickModeLoadTime;
		Size quickModeLoadIndicatorSize;
		public OptionsCustomizationForm(ILayoutControl control)
			: base(control) {
			showLoadButton = true;
			showSaveButton = true;
			showUndoButton = true;
			showRedoButton = true;
			enableUndoManager = true;
			allowHandleDeleteKeyCore = true;
			showLayoutTreeView = true;
			defaultSaveDir = String.Empty;
			defaultRestoreDir = string.Empty;
			quickModeInitDelay = 300;
			quickModeLoadIndicatorSize = new Size(7, 22);
			quickModeLoadTime = 1500;
			showProperties = GetShowPropertiesDefaultValue();
		}
		protected virtual bool GetShowPropertiesDefaultValue() {
#if DEBUG
			return true;
#else
			return false;
#endif
		}
		protected override void EndChange() {
			if(owner == null) return;
			if(owner is LayoutControl) owner.LongPressControl.Update(this);
			base.EndChange();
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormDefaultSaveDirectory"),
#endif
 DefaultValue("")]
		public string DefaultSaveDirectory {
			get { return defaultSaveDir; }
			set {
				if (!StartChange()) return;
				defaultSaveDir = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormDesignTimeCustomizationFormPositionAndSize"),
#endif
 DefaultValue(null)]
		public Rectangle? DesignTimeCustomizationFormPositionAndSize {
			get { return designTimeCustomizationFormBoundsCore; }
			set {
				if (!StartChange()) return;
				designTimeCustomizationFormBoundsCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormDefaultRestoreDirectory"),
#endif
 DefaultValue("")]
		public string DefaultRestoreDirectory {
			get { return defaultRestoreDir; }
			set {
				if (!StartChange()) return;
				defaultRestoreDir = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormShowLoadButton"),
#endif
 DefaultValue(true)]
		public bool ShowLoadButton {
			get { return showLoadButton; }
			set {
				if (!StartChange()) return;
				showLoadButton = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormAllowHandleDeleteKey"),
#endif
 DefaultValue(true)]
		public bool AllowHandleDeleteKey {
			get { return allowHandleDeleteKeyCore; }
			set {
				if (!StartChange()) return;
				allowHandleDeleteKeyCore = value;
				EndChange();
			}
		}
		protected virtual bool ShouldSerializeShowPropertyGrid() { return GetShowPropertiesDefaultValue() != ShowPropertyGrid; }
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormShowPropertyGrid")]
#endif
		public bool ShowPropertyGrid {
			get { return showProperties; }
			set {
				if (!StartChange()) return;
				showProperties = value;
				EndChange();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the EnableUndoManager property instead"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowUndoManager {
			get { return EnableUndoManager; }
			set { EnableUndoManager = value; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormEnableUndoManager"),
#endif
 DefaultValue(true)]
		public bool EnableUndoManager {
			get { return enableUndoManager; }
			set {
				if (!StartChange()) return;
				enableUndoManager = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormShowSaveButton"),
#endif
 DefaultValue(true)]
		public bool ShowSaveButton {
			get { return showSaveButton; }
			set {
				if (!StartChange()) return;
				showSaveButton = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormShowUndoButton"),
#endif
 DefaultValue(true)]
		public bool ShowUndoButton {
			get { return showUndoButton; }
			set {
				if (!StartChange()) return;
				showUndoButton = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormShowRedoButton"),
#endif
 DefaultValue(true)]
		public bool ShowRedoButton {
			get { return showRedoButton; }
			set {
				if (!StartChange()) return;
				showRedoButton = value;
				EndChange();
			}
		}
		internal bool ShowButtonsPanel {
			get {
				return ShowLoadButton || showRedoButton || showUndoButton || ShowSaveButton;
			}
		}
		protected internal bool DesignerExpertMode { get { return true; } }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormShowLayoutTreeView"),
#endif
 DefaultValue(true)]
		public bool ShowLayoutTreeView {
			get { return showLayoutTreeView; }
			set {
				if (!StartChange()) return;
				showLayoutTreeView = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormQuickModeInitDelay"),
#endif
 DefaultValue(300)]
		public int QuickModeInitDelay {
			get { return quickModeInitDelay; }
			set {
				if(!StartChange()) return;
				if(value < 1) return;
				quickModeInitDelay = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormQuickModeLoadTime"),
#endif
 DefaultValue(1500)]
		public int QuickModeLoadTime {
			get { return quickModeLoadTime; }
			set {
				if(!StartChange()) return;
				if(value < 20) return;
				quickModeLoadTime = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsCustomizationFormQuickModeLoadIndicatorSize"),
#endif
 DefaultValue(typeof(Size), "7, 22")]
		public Size QuickModeLoadIndicatorSize {
			get { return quickModeLoadIndicatorSize; }
			set {
				if(!StartChange()) return;
				if(value.Width < 5 || value.Height < 5) return;
				quickModeLoadIndicatorSize = value;
				EndChange();
			}
		}
	}
	public enum PaddingMode { Default, MSGuidelines }
	public enum ControlMaxSizeCalcMode { UseControlMaximumSize, CombineControlMaximumSizeAndIXtraResizableMaxSize, UseControlMaximumSizeIfNotZero}
	public class OptionsView :BaseLayoutOptions {
		int dragframeFadeSpeed;
		int dragframeFadeFramesCount;
		bool shareLookAndFeelWithChildren;
		bool allowTransparentBackColor;
		Color itemBordersColorCore = Color.Black;
		bool enableItemBordersCore = false;
		bool enableHotTrack = false;
		bool allowScaleCore = false;
		bool enableLayoutItemsSkinning = true;
		bool alwaysScrollActiveControlIntoViewCore = true;
		bool enableHighlightFocusedCells = false;
		bool enableHighlightDisabledCells = false;
		bool enableIndentionsInGroupWithoutBorders = false;
		bool enableIndentsSkinningCore = true;
		bool rightToLeftMirrowingAppliedCore = false;
		ControlMaxSizeCalcMode controlMaxSizeCalcModeCore;
		bool useDefaultDragAndDropRenderingCore = !RDPHelper.IsRemoteSession;
		AutoSizeModes autoSizeModeCore = AutoSizeModes.UseMinSizeAndGrow;
		PaddingMode paddingHelperModeCore = PaddingMode.Default;
		DefaultBoolean isReadonlyCore = DefaultBoolean.Default;
		public OptionsView(ILayoutControl owner)
			: base(owner) {
			SetDefaults();
		}
		protected virtual void SetDefaults() {
			shareLookAndFeelWithChildren = true;
			allowTransparentBackColor = true;
			itemBordersColorCore = Color.Black;
			dragframeFadeFramesCount = 10;
			dragframeFadeSpeed = 1000;
			controlMaxSizeCalcModeCore = ControlMaxSizeCalcMode.CombineControlMaximumSizeAndIXtraResizableMaxSize;
		}
		protected override void EndChange() {
			owner.UpdateChildControlsLookAndFeel();
			base.EndChange();
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewUseSkinIndents"),
#endif
 DefaultValue(true)]
		public bool UseSkinIndents {
			get {
				return enableIndentsSkinningCore;
			}
			set {
				if(!StartChange()) return;
				enableIndentsSkinningCore = value;
				EndChange();
			}
		}
		bool fitControlsToDisplayAreaWidth = true;
		[DefaultValue(true)]
		public bool FitControlsToDisplayAreaWidth {
			get {
				return fitControlsToDisplayAreaWidth;
			}
			set {
				if(!StartChange()) return;
				fitControlsToDisplayAreaWidth = value;
				EndChange();
			}
		}
		bool fitControlsToDisplayAreaHeight = true;
		[DefaultValue(true)]
		public bool FitControlsToDisplayAreaHeight {
			get {
				return fitControlsToDisplayAreaHeight;
			}
			set {
				if(!StartChange()) return;
				fitControlsToDisplayAreaHeight = value;
				EndChange();
			}
		}
		[ DefaultValue(ControlMaxSizeCalcMode.CombineControlMaximumSizeAndIXtraResizableMaxSize)]
		public ControlMaxSizeCalcMode ControlDefaultMaxSizeCalcMode {
			get {
				return controlMaxSizeCalcModeCore;
			}
			set {
				if(!StartChange()) return;
				controlMaxSizeCalcModeCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewUseDefaultDragAndDropRendering"),
#endif
 DefaultValue(true)]
		public bool UseDefaultDragAndDropRendering {
			get {
				return useDefaultDragAndDropRenderingCore;
			}
			set {
				if(!StartChange()) return;
				useDefaultDragAndDropRenderingCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewAutoSizeModeInLayoutControl"),
#endif
 DefaultValue(AutoSizeMode.GrowOnly)]
		[Obsolete("Use AutoSizeInLayoutControl property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AutoSizeMode AutoSizeModeInLayoutControl {
			get {
				if(autoSizeModeCore == AutoSizeModes.UseMinAndMaxSize) return AutoSizeMode.GrowOnly;
				return (AutoSizeMode)(int)autoSizeModeCore;
			}
			set {
				if(!StartChange()) return;
				autoSizeModeCore = (AutoSizeModes)(int)value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewAutoSizeInLayoutControl"),
#endif
 DefaultValue(AutoSizeModes.UseMinSizeAndGrow)]
		public AutoSizeModes AutoSizeInLayoutControl {
			get {
				return autoSizeModeCore;
			}
			set {
				if(!StartChange()) return;
				autoSizeModeCore = value;
				if(owner is LayoutControl && owner.DesignMode) {
					((LayoutControl)owner).RaiseSizeableChangedCore(true);
				}
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewDragFadeAnimationSpeed"),
#endif
 DefaultValue(1000)]
		public int DragFadeAnimationSpeed {
			get {
				return dragframeFadeSpeed;
			}
			set {
				if(!StartChange()) return;
				dragframeFadeSpeed = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewDragFadeAnimationFrameCount"),
#endif
 DefaultValue(10)]
		public int DragFadeAnimationFrameCount {
			get {
				return dragframeFadeFramesCount;
			}
			set {
				if(!StartChange()) return;
				dragframeFadeFramesCount = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewIsReadOnly"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean IsReadOnly {
			get {
				return isReadonlyCore;
			}
			set {
				if(!StartChange()) return;
				isReadonlyCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewShareLookAndFeelWithChildren"),
#endif
 DefaultValue(true)]
		public bool ShareLookAndFeelWithChildren {
			get {
				return shareLookAndFeelWithChildren;
			}
			set {
				if(!StartChange()) return;
				shareLookAndFeelWithChildren = value;
				EndChange();
			}
		}
		[Obsolete("use EnableTransparentBackColor instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue(true)]
		public bool AllowTransparentBackColor {
			get { return EnableTransparentBackColor; }
			set { EnableTransparentBackColor = value; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewEnableTransparentBackColor"),
#endif
 DefaultValue(true)]
		public bool EnableTransparentBackColor {
			get {
				return allowTransparentBackColor;
			}
			set {
				if(!StartChange()) return;
				allowTransparentBackColor = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewPaddingSpacingMode"),
#endif
 DefaultValue(PaddingMode.Default)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PaddingMode PaddingSpacingMode {
			get {
				return paddingHelperModeCore;
			}
			set {
				if(!StartChange()) return;
				paddingHelperModeCore = value;
				EndChange();
			}
		}
		[ DefaultValue(false)]
		public bool EnableIndentsInGroupsWithoutBorders {
			get {
				return enableIndentionsInGroupWithoutBorders;
			}
			set {
				if(!StartChange()) return;
				enableIndentionsInGroupWithoutBorders = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewAlwaysScrollActiveControlIntoView"),
#endif
 DefaultValue(true)]
		public bool AlwaysScrollActiveControlIntoView {
			get {
				return alwaysScrollActiveControlIntoViewCore;
			}
			set {
				if(!StartChange()) return;
				alwaysScrollActiveControlIntoViewCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewAllowItemSkinning"),
#endif
 DefaultValue(true)]
		public bool AllowItemSkinning {
			get {
				return enableLayoutItemsSkinning;
			}
			set {
				if(!StartChange()) return;
				enableLayoutItemsSkinning = value;
				EndChange();
			}
		}
		[Obsolete("Use AllowItemSkinning instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowItemSkining {
			get {
				return AllowItemSkinning;
			}
			set {
				AllowItemSkinning = value;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewDrawItemBorders"),
#endif
 DefaultValue(false)]
		public bool DrawItemBorders {
			get {
				return enableItemBordersCore;
			}
			set {
				if(!StartChange()) return;
				enableItemBordersCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewAllowHotTrack"),
#endif
 DefaultValue(false)]
		public bool AllowHotTrack {
			get {
				return enableHotTrack;
			}
			set {
				if(!StartChange()) return;
				enableHotTrack = value;
				EndChange();
			}
		}
		[DefaultValue(false), Obsolete("Use the UseParentAutoScaleFactor option instead")]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowScaleControlsToDisplayResolution {
			get {
				return allowScaleCore;
			}
			set {
				if(!StartChange()) return;
				allowScaleCore = value;
				EndChange();
			}
		}
		[DefaultValue(false)]
		public bool UseParentAutoScaleFactor {
			get {
				return allowScaleCore;
			}
			set {
				if(!StartChange()) return;
				allowScaleCore = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewHighlightDisabledItem"),
#endif
 DefaultValue(false)]
		public bool HighlightDisabledItem {
			get {
				return enableHighlightDisabledCells;
			}
			set {
				if(!StartChange()) return;
				enableHighlightDisabledCells = value;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewHighlightFocusedItem"),
#endif
 DefaultValue(false)]
		public bool HighlightFocusedItem {
			get {
				return enableHighlightFocusedCells;
			}
			set {
				if(!StartChange()) return;
				enableHighlightFocusedCells = value;
				EndChange();
			}
		}
		protected virtual bool ShouldSerializeItemBorderColor() {
			return ItemBorderColor != Color.Black;
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("OptionsViewItemBorderColor")]
#endif
		public Color ItemBorderColor {
			get { return itemBordersColorCore; }
			set {
				if(!StartChange()) return;
				itemBordersColorCore = value;
				EndChange();
			}
		}
		bool allowGlyphSkinningCore;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewAllowGlyphSkinning"),
#endif
 DefaultValue(false)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(!StartChange()) return;
				allowGlyphSkinningCore = value;
				EndChange();
			}
		}
		DefaultBoolean drawAdornerLayer = DefaultBoolean.Default;
		[Obsolete("Use DrawAdornerLayer instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DefaultBoolean DrawAdornerLayered {
			get {
				return drawAdornerLayer;
			}
			set {
				drawAdornerLayer = value;
				if(owner is LayoutControl) {
					LayoutControl lc = owner as LayoutControl;
					lc.CreateOrDisposeAdorner(value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsViewDrawAdornerLayer"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean DrawAdornerLayer {
			get {
				return drawAdornerLayer;
			}
			set {
				drawAdornerLayer = value;
				if(owner is LayoutControl) {
					LayoutControl lc = owner as LayoutControl;
					lc.CreateOrDisposeAdorner(value);
				}
			}
		}
		[ DefaultValue(false)]
		public bool RightToLeftMirroringApplied { get { return rightToLeftMirrowingAppliedCore; } set { rightToLeftMirrowingAppliedCore = value; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class OptionsItemTextGroup : IDisposable {
		TextAlignModeGroup textAlignMode;
		bool alignControlsWithHiddenText;
		int textToControlDistanceCore;
		LayoutGroup owner;
		public OptionsItemTextGroup(LayoutGroup owner) {
			this.owner = owner;
			this.textToControlDistanceCore = -999;
			this.textAlignMode = TextAlignModeGroup.UseParentOptions;
			this.alignControlsWithHiddenText = false;
		}
		public virtual void Dispose() {
			this.owner = null;
		}
		protected internal void AssignInternal(OptionsItemTextGroup options) {
			this.textToControlDistanceCore = options.textToControlDistanceCore;
			this.textAlignMode = options.textAlignMode;
			this.alignControlsWithHiddenText = options.alignControlsWithHiddenText;
		}
		protected internal virtual int GetTextToControlDistance(BaseLayoutItem bli) {
			if(textToControlDistanceCore > -999) return textToControlDistanceCore;
			else {
				if(owner != null && owner.Owner != null) {
					if(bli == null) return owner.Owner.DefaultValues.TextToControlDistance.Top;
					else {
						switch(bli.TextLocation) {
							case Locations.Default:
							case Locations.Left:
								return owner.Owner.DefaultValues.TextToControlDistance.Left;
							case Locations.Bottom:
								return owner.Owner.DefaultValues.TextToControlDistance.Bottom;
							case Locations.Top:
								return owner.Owner.DefaultValues.TextToControlDistance.Top;
							case Locations.Right:
								return owner.Owner.DefaultValues.TextToControlDistance.Right;
						}
						return owner.Owner.DefaultValues.TextToControlDistance.Left;
					}
				}
				else {
					return 5;
				}
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsItemTextGroupTextToControlDistance"),
#endif
 DefaultValue(3)]
		[RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public int TextToControlDistance {
			get { return GetTextToControlDistance(null); }
			set {
				if(!StartChange()) return;
				if(value < 0) value = 0;
				textToControlDistanceCore = value;
				EndChange();
			}
		}
		protected virtual bool StartChange() {
			if(owner == null) return false;
			owner.BeginChangeUpdate();
			return true;
		}
		protected virtual void EndChange() {
			if(owner != null && owner.Owner != null) {
				owner.Owner.ShouldUpdateConstraints = true;
				owner.Owner.ShouldArrangeTextSize = true;
				owner.Owner.ShouldUpdateLookAndFeel = true;
			}
			owner.EndChangeUpdate();
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsItemTextGroupAlignControlsWithHiddenText"),
#endif
 DefaultValue(false)]
		public bool AlignControlsWithHiddenText {
			get {
				return alignControlsWithHiddenText;
			}
			set {
				if(!StartChange()) return;
				alignControlsWithHiddenText = value;
				EndChange();
			}
		}
		protected LayoutStyleManager GetLayoutStyleManager() {
			if(owner.Owner == null) return null;
			return owner.Owner.LayoutStyleManager;
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsItemTextGroupTextAlignMode"),
#endif
 DefaultValue(TextAlignModeGroup.UseParentOptions)]
		[RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual TextAlignModeGroup TextAlignMode {
			get {
				return GetLayoutStyleManager() != null ? GetLayoutStyleManager().CorrectGroupTextAlignMode(owner, textAlignMode) : textAlignMode;
			}
			set {
				if(!StartChange()) return;
				textAlignMode = value;
				EndChange();
			}
		}
	}
	public class OptionsItemText : BaseLayoutOptions {
		TextOptions textOptionsCore;
		TextAlignMode allignType;
		AppearanceObject ownerAppearance;
		public OptionsItemText(ILayoutControl owner)
			: base(owner) {
			ownerAppearance = new AppearanceObject();
			textOptionsCore = new TextOptions(ownerAppearance);
			SetDefaults();
			SubscribeTextOptionsEvents();
		}
		const TextAlignMode DefaultAllignType = TextAlignMode.AlignInLayoutControl;
		const int DefaultTextFieldToControlDistance = 5;
		const bool DefaultAlignControlsWithHiddenText = false;
		protected void SetDefaults() {
			allignType = DefaultAllignType;
		}
		protected void SubscribeTextOptionsEvents() {
			ownerAppearance.Changed += new EventHandler(OnChanged);
		}
		protected void UnSubscribeTextOptionsEvents() {
			ownerAppearance.Changed -= new EventHandler(OnChanged);
		}
		protected void OnChanging(object sender, EventArgs e) {
			if(!StartChange()) return;
		}
		protected void OnChanged(object sender, EventArgs e) {
			EndChange();
		}
		public void Dispose() {
			if(ownerAppearance != null) {
				UnSubscribeTextOptionsEvents();
				ownerAppearance.Dispose();
				ownerAppearance = null;
			}
			textOptionsCore = null;
		}
		bool CanChangeOptions {
			get {
				return owner != null && !owner.IsUpdateLocked;
			}
		}
		protected override void EndChange() {
			owner.ShouldArrangeTextSize = true;
			owner.ShouldUpdateLookAndFeel = true;
			owner.Invalidate();
			base.EndChange();
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsItemTextTextToControlDistance"),
#endif
 DefaultValue(DefaultTextFieldToControlDistance)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public int TextToControlDistance {
			get {
				if(owner != null && owner.RootGroup != null) {
					return owner.RootGroup.OptionsItemText.TextToControlDistance;
				}
				return 0;
			}
			set {
				if(owner != null && owner.RootGroup != null) {
					owner.RootGroup.OptionsItemText.TextToControlDistance = value;
				}
			}
		}
		protected bool ShouldSeralizeTextOptions {
			get {
				return
					textOptionsCore.HAlignment != DXUtils.HorzAlignment.Default ||
					textOptionsCore.VAlignment != DXUtils.VertAlignment.Default ||
					textOptionsCore.WordWrap != DXUtils.WordWrap.Default ||
					textOptionsCore.Trimming != DXUtils.Trimming.Default;
			}
		}
		[Obsolete("Use Root.AppearanceItemCaption.TextOptions instead of TextOptions")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TextOptions TextOptions {
			get { return textOptionsCore; }
		}
		bool enableAlignment = true;
		[DefaultValue(true)]
		[Browsable(false)]
		[Obsolete("use TextAlignMode instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EnableAutoAlignment {
			get { return enableAlignment; }
			set {
				if(!StartChange()) return;
				EndChange();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsItemTextAlignControlsWithHiddenText"),
#endif
 DefaultValue(DefaultAlignControlsWithHiddenText)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public bool AlignControlsWithHiddenText {
			get {
				if(owner != null && owner.RootGroup != null) {
					return owner.RootGroup.OptionsItemText.AlignControlsWithHiddenText;
				}
				return false;
			}
			set {
				if(owner != null && owner.RootGroup != null) {
					owner.RootGroup.OptionsItemText.AlignControlsWithHiddenText = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsItemTextTextAlignMode"),
#endif
 DefaultValue(DefaultAllignType)]
		public TextAlignMode TextAlignMode {
			get { return allignType; }
			set {
				if(!StartChange()) return;
				allignType = value;
				enableAlignment = TextAlignMode != TextAlignMode.CustomSize;
				EndChange();
			}
		}
		[Obsolete("use TextAlignMode instead")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AutoAlignMode AutoAlignMode {
			get {
				switch(TextAlignMode) {
					case TextAlignMode.AlignInGroups:
						return AutoAlignMode.AlignLocal;
					case TextAlignMode.AlignInLayoutControl:
						return AutoAlignMode.AlignGlobal;
					case TextAlignMode.AutoSize:
						return AutoAlignMode.AutoSize;
					case TextAlignMode.CustomSize:
						return AutoAlignMode.AutoSize;
				}
				return AutoAlignMode.AutoSize;
			}
			set {
				switch(value) {
					case AutoAlignMode.AlignGlobal:
						TextAlignMode = TextAlignMode.AlignInLayoutControl;
						break;
					case AutoAlignMode.AlignLocal:
						TextAlignMode = TextAlignMode.AlignInGroups;
						break;
					case AutoAlignMode.AutoSize:
						TextAlignMode = TextAlignMode.AutoSize;
						break;
				}
			}
		}
	}
	public class OptionsPrintBase :BaseOptions {
		protected BaseLayoutItem owner;
		int textToControlDistance = -1;
		public OptionsPrintBase(BaseLayoutItem owner) {
			allowPrint = true;
			this.owner = owner;
			printAppearanceCaptionCore = new AppearanceObject();
		}
		[ DefaultValue(-1)]
		[XtraSerializableProperty()]
		public int TextToControlDistance {
			get { return textToControlDistance; }
			set { if(value < -1) return; textToControlDistance = value; }
		}
		bool allowPrint;
		[ DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool AllowPrint {
			get { return allowPrint; }
			set { allowPrint = value; }
		}
		AppearanceObject printAppearanceCaptionCore;
		bool ShouldSerializeAppearanceItemCaption() { return AppearanceItemCaption.ShouldSerialize(); }
		void ResetAppearanceItemCaption() { AppearanceItemCaption.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public AppearanceObject AppearanceItemCaption { get { return printAppearanceCaptionCore; }}
	}
	public class OptionsPrintItem : OptionsPrintBase {
		public OptionsPrintItem(BaseLayoutItem owner)
			: base(owner) {
		}
		#region GetAppearance
		internal AppearanceObject GetItemAppearance() {
			AppearanceObject result = new AppearanceObject();
			AppearanceHelper.Combine(result, AppearanceArray(owner));
			return result;
		}
		private AppearanceObject[] AppearanceArray(BaseLayoutItem owner) {
			ArrayList list = new ArrayList();
			BaseLayoutItem currentItem = owner;
			while(currentItem != null) {
				if(currentItem is LayoutControlItem) {
					list.Add((currentItem as LayoutControlItem).OptionsPrint.AppearanceItemCaption);
				}
				if(currentItem is LayoutGroup) {
					list.Add((currentItem as LayoutGroup).OptionsPrint.AppearanceItemCaption);
				}
				currentItem = GetNextParent(currentItem);
			}
			if(owner.Owner != null && owner.Owner is LayoutControl) {
				list.Add((owner.Owner as LayoutControl).OptionsPrint.AppearanceItemCaption);
			}
			return (AppearanceObject[])list.ToArray(typeof(AppearanceObject));
		}
		private BaseLayoutItem GetNextParent(BaseLayoutItem currentItem) {
			return currentItem.Parent;
		}
		#endregion
	}
	public class OptionsPrintGroup : OptionsPrintBase {
		public OptionsPrintGroup(BaseLayoutItem owner)
			: base(owner) {
			this.owner = owner;
			printAppearanceCaptionCore = new AppearanceObject();
		}
		AppearanceObject printAppearanceCaptionCore;
		bool ShouldSerializeAppearanceGroupCaption() { return AppearanceGroupCaption.ShouldSerialize(); }
		void ResetAppearanceGroupCaption() { AppearanceGroupCaption.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public AppearanceObject AppearanceGroupCaption { get { return printAppearanceCaptionCore; } }
		bool allowPrintGroupCaptionCore = true;
		[ DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool AllowPrintGroupCaption {
			get { return allowPrintGroupCaptionCore; }
			set { allowPrintGroupCaptionCore = value; }
		}
		#region GetAppearance
		internal AppearanceObject GetGroupAppearance() {
			AppearanceObject result = new AppearanceObject();
			AppearanceHelper.Combine(result, AppearanceArray(owner));
			return result;
		}
		private AppearanceObject[] AppearanceArray(BaseLayoutItem owner) {
			ArrayList list = new ArrayList();
			BaseLayoutItem currentItem = owner;
			while(currentItem != null) {
				if(currentItem is LayoutGroup) {
					list.Add((currentItem as LayoutGroup).OptionsPrint.AppearanceGroupCaption);
				}
				currentItem = GetNextParent(currentItem);
			}
			if(owner.Owner != null && owner.Owner is LayoutControl) {
				list.Add((owner.Owner as LayoutControl).OptionsPrint.AppearanceGroupCaption);
			}
			return (AppearanceObject[])list.ToArray(typeof(AppearanceObject));
		}
		private BaseLayoutItem GetNextParent(BaseLayoutItem currentItem) {
			return currentItem.Parent;
		}
		#endregion
	}
	public class LayoutSerializationOptions : BaseOptions {
		bool restoreAppearanceTabPageCore;
		bool restoreLayoutGroupAppearanceGroupCore;
		bool restoreLayoutItemAppearanceCore;
		bool restoreLayoutItemTextCore;
		bool restoreLayoutItemCustomizationFormTextCore;
		bool discardOldItemsCore;
		bool restoreGroupPaddingCore = false;
		bool restoreGroupSpacingCore = false;
		bool restoreLayoutItemPaddingCore = false;
		bool restoreLayoutItemSpacingCore = false;
		bool restoreRootGroupPaddingCore = false;
		bool restoreRootGroupSpacingCore = false;
		bool restoreTabbedGroupPaddingCore = false;
		bool restoreTabbedGroupSpacingCore = false;
		bool restoreTextToControlDistanceCore = false;
		bool recreateIFixedItems = false;
		public LayoutSerializationOptions() {
			restoreAppearanceTabPageCore = false;
			restoreLayoutGroupAppearanceGroupCore = false;
			restoreLayoutItemAppearanceCore = false;
			restoreLayoutItemTextCore = true;
			restoreLayoutItemCustomizationFormTextCore = true;
			discardOldItemsCore = false;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				LayoutSerializationOptions opt = options as LayoutSerializationOptions;
				if(opt == null) return;
				this.restoreLayoutItemTextCore = opt.restoreLayoutItemTextCore;
				this.restoreLayoutItemCustomizationFormTextCore = opt.restoreLayoutItemCustomizationFormTextCore;
				this.restoreAppearanceTabPageCore = opt.restoreAppearanceTabPageCore;
				this.restoreLayoutGroupAppearanceGroupCore = opt.restoreLayoutGroupAppearanceGroupCore;
				this.restoreLayoutItemAppearanceCore = opt.restoreLayoutItemAppearanceCore;
				this.discardOldItemsCore = opt.discardOldItemsCore;
				this.restoreGroupPaddingCore = opt.restoreGroupPaddingCore;
				this.restoreGroupSpacingCore = opt.restoreGroupSpacingCore;
				this.restoreLayoutItemPaddingCore = opt.restoreLayoutItemPaddingCore;
				this.restoreLayoutItemSpacingCore = opt.restoreLayoutItemSpacingCore;
				this.restoreRootGroupPaddingCore = opt.restoreRootGroupPaddingCore;
				this.restoreRootGroupSpacingCore = opt.restoreRootGroupSpacingCore;
				this.restoreTabbedGroupPaddingCore = opt.restoreTabbedGroupPaddingCore;
				this.restoreTabbedGroupSpacingCore = opt.restoreTabbedGroupSpacingCore;
				this.restoreTextToControlDistanceCore = opt.restoreTextToControlDistanceCore;
				this.recreateIFixedItems = opt.recreateIFixedItems;
			}
			finally {
				EndUpdate();
			}
		}
		internal bool ShouldSerializeCore() { return ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutSerializationOptionsRestoreLayoutItemText"),
#endif
 DefaultValue(true)]
		public virtual bool RestoreLayoutItemText {
			get { return restoreLayoutItemTextCore; }
			set {
				if(restoreLayoutItemTextCore == value) return;
				this.restoreLayoutItemTextCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreLayoutItemText", !RestoreLayoutItemText, RestoreLayoutItemText));
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutSerializationOptionsRestoreLayoutItemCustomizationFormText"),
#endif
 DefaultValue(true)]
		public virtual bool RestoreLayoutItemCustomizationFormText {
			get { return restoreLayoutItemCustomizationFormTextCore; }
			set {
				if(restoreLayoutItemCustomizationFormTextCore == value) return;
				this.restoreLayoutItemCustomizationFormTextCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreLayoutItemCustomizationFormText", !RestoreLayoutItemCustomizationFormText, RestoreLayoutItemCustomizationFormText));
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutSerializationOptionsRestoreLayoutGroupAppearanceGroup"),
#endif
 DefaultValue(false)]
		public virtual bool RestoreLayoutGroupAppearanceGroup {
			get { return restoreLayoutGroupAppearanceGroupCore; }
			set {
				if(restoreLayoutGroupAppearanceGroupCore == value) return;
				this.restoreLayoutGroupAppearanceGroupCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreLayoutGroupAppearanceGroup", !RestoreLayoutGroupAppearanceGroup, RestoreLayoutGroupAppearanceGroup));
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutSerializationOptionsRestoreAppearanceTabPage"),
#endif
 DefaultValue(false)]
		public virtual bool RestoreAppearanceTabPage {
			get { return restoreAppearanceTabPageCore; }
			set {
				if(restoreAppearanceTabPageCore == value) return;
				this.restoreAppearanceTabPageCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreAppearanceTabPage", !RestoreAppearanceTabPage, RestoreAppearanceTabPage));
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutSerializationOptionsRestoreAppearanceItemCaption"),
#endif
 DefaultValue(false)]
		public virtual bool RestoreAppearanceItemCaption {
			get { return restoreLayoutItemAppearanceCore; }
			set {
				if(restoreLayoutItemAppearanceCore == value) return;
				this.restoreLayoutItemAppearanceCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreAppearanceItemCaption", !RestoreAppearanceItemCaption, RestoreAppearanceItemCaption));
			}
		}
		[ DefaultValue(false)]
		public virtual bool DiscardOldItems {
			get { return discardOldItemsCore; }
			set {
				if(discardOldItemsCore == value) return;
				this.discardOldItemsCore = value;
				OnChanged(new BaseOptionChangedEventArgs("DiscardOldItems", !DiscardOldItems, DiscardOldItems));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreGroupPadding {
			get { return restoreGroupPaddingCore; }
			set {
				if(restoreGroupPaddingCore == value) return;
				this.restoreGroupPaddingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreGroupPadding", !RestoreGroupPadding, RestoreGroupPadding));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreGroupSpacing {
			get { return restoreGroupSpacingCore; }
			set {
				if(restoreGroupSpacingCore == value) return;
				this.restoreGroupSpacingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreGroupSpacing", !RestoreGroupSpacing, RestoreGroupSpacing));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreLayoutItemPadding {
			get { return restoreLayoutItemPaddingCore; }
			set {
				if(restoreLayoutItemPaddingCore == value) return;
				this.restoreLayoutItemPaddingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreLayoutItemPadding", !RestoreLayoutItemPadding, RestoreLayoutItemPadding));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreLayoutItemSpacing {
			get { return restoreLayoutItemSpacingCore; }
			set {
				if(restoreLayoutItemSpacingCore == value) return;
				this.restoreLayoutItemSpacingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreLayoutItemSpacing", !RestoreLayoutItemSpacing, RestoreLayoutItemSpacing));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreRootGroupPadding {
			get { return restoreRootGroupPaddingCore; }
			set {
				if(restoreRootGroupPaddingCore == value) return;
				this.restoreRootGroupPaddingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreRootGroupPadding", !RestoreRootGroupPadding, RestoreRootGroupPadding));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreRootGroupSpacing {
			get { return restoreRootGroupSpacingCore; }
			set {
				if(restoreRootGroupSpacingCore == value) return;
				this.restoreRootGroupSpacingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreRootGroupSpacing", !RestoreRootGroupSpacing, RestoreRootGroupSpacing));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreTabbedGroupPadding {
			get { return restoreTabbedGroupPaddingCore; }
			set {
				if(restoreTabbedGroupPaddingCore == value) return;
				this.restoreTabbedGroupPaddingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreTabbedGroupPadding", !RestoreTabbedGroupPadding, RestoreTabbedGroupPadding));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreTabbedGroupSpacing {
			get { return restoreTabbedGroupSpacingCore; }
			set {
				if(restoreTabbedGroupSpacingCore == value) return;
				this.restoreTabbedGroupSpacingCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreTabbedGroupSpacing", !RestoreTabbedGroupSpacing, RestoreTabbedGroupSpacing));
			}
		}
		[ DefaultValue(false)]
		public virtual bool RestoreTextToControlDistance {
			get { return restoreTextToControlDistanceCore; }
			set {
				if(restoreTextToControlDistanceCore == value) return;
				this.restoreTextToControlDistanceCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreTextToControlDistance", !RestoreTextToControlDistance, RestoreTextToControlDistance));
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool RecreateIFixedItems {
			get { return recreateIFixedItems; }
			set {
				if(recreateIFixedItems == value) return;
				this.recreateIFixedItems = value;
				OnChanged(new BaseOptionChangedEventArgs("RecreateIFixedItems", !RecreateIFixedItems, RecreateIFixedItems));
			}
		}
	}
	internal static class PrintAppearanceCreator {
		internal static AppearanceObject ItemCaptionAppearance() {
			AppearanceObject result = new AppearanceObject();
			result.TextOptions.HAlignment = HorzAlignment.Near;
			result.TextOptions.VAlignment = VertAlignment.Center;
			result.Options.UseTextOptions = true;
			return result;
		}
		internal static AppearanceObject GroupCaptionAppearance() {
			AppearanceObject result = new AppearanceObject();
			result.BackColor = Color.LightGray;
			result.TextOptions.HAlignment = HorzAlignment.Center;
			result.TextOptions.VAlignment = VertAlignment.Center;
			result.Font = new Font(result.Font.FontFamily, 10.25f);
			result.Options.UseTextOptions = true;
			result.Options.UseFont = true;
			result.Options.UseTextOptions = true;
			return result;
		}
	}
}
