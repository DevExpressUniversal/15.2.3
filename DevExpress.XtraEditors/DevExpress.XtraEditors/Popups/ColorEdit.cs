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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraTab;
namespace DevExpress.XtraEditors.Controls {
	public enum ColorText { Integer, Native }
}
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemColorEdit : RepositoryItemPopupBase {
		HorzAlignment colorAlignment;
		ColorText colorText;
		Color[] customColors;
		bool storeColorAsInteger, showColorDialog, showCustomColors, showWebColors, showSystemColors;
		HighlightStyle highlightedItemStyle;
		public RepositoryItemColorEdit() {
			this.storeColorAsInteger = false;
			this.showColorDialog = true;
			this.highlightedItemStyle = HighlightStyle.Default;
			this.fTextEditStyle = TextEditStyles.DisableTextEditor;
			this.colorAlignment = HorzAlignment.Near;
			this.colorText = ColorText.Native;
			this.customColors = new Color[16];
			this.showCustomColors = this.showWebColors = this.showSystemColors = true;
			for(int n = 0; n < 16; n++) customColors[n] = Color.Empty;
		}
		protected Image GetBrickImage(ColorEditViewInfo ceVi, PrintCellHelperInfo info) {
			MultiKey key = new MultiKey(new object[] { ceVi.Bounds.Size, info.EditValue, this.AutoHeight, this.BorderStyle, this.Enabled, this.EditorTypeName });
			Image img = GetCachedPrintImage(key, info.PS);
			if(img != null) return img;
			using(BitmapGraphicsHelper gHelper = new BitmapGraphicsHelper(ceVi.ColorBoxRect.Width, ceVi.ColorBoxRect.Height)) {
				Color color = ConvertToColor(info.EditValue);
				gHelper.Graphics.FillRectangle(new SolidBrush(color), gHelper.Graphics.ClipBounds);
				return AddImageToPrintCache(key, gHelper.MemSafeBitmap, info.PS);
			}
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			if(type != "panel") {
				style.Sides = DevExpress.XtraPrinting.BorderSide.None;
			}
			else {
				SetupTextBrickStyleProperties(info, style);
			}
			return style;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			ColorEditViewInfo ceVi = PreparePrintViewInfo(info, true) as ColorEditViewInfo;
			IPanelBrick panel = CreatePanelBrick(info, true, CreateBrickStyle(info, "panel"));
			ITextBrick textBrick = CreateTextBrick(info);
			textBrick.Rect = ceVi.ColorTextRect;
			panel.Bricks.Add(textBrick);
			IImageBrick imageBrick = CreateImageBrick(info, CreateBrickStyle(info, "image"));
			imageBrick.Image = GetBrickImage(ceVi, info);
			imageBrick.Rect = ceVi.ColorBoxRect;
			panel.Bricks.Add(imageBrick);
			return panel;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemColorEdit Properties { get { return this; } }
		private static object colorChanged = new object();
		[Browsable(false)]
		public override string EditorTypeName { get { return "ColorEdit"; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set {
				base.TextEditStyle = value;
			}
		}
		protected virtual bool ShouldSerializeCustomColors() {
			for(int i = 0; i < CustomColors.Length; i++)
				if(CustomColors[i] != Color.Empty) return true;
			return false;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditCustomColors"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Color[] CustomColors {
			get { return customColors; }
			set {
				const int arrayLength = 16;
				if(value == null || value.Length != arrayLength) return;
				if(ArraysEqual(CustomColors, value)) return;
				Array.Copy(value, CustomColors, arrayLength);
				FireChanged();
			}
		}
		internal string CustomColorsName { get { return "CustomColors"; } }
		PopupColorEditForm PopupForm {
			get {
				if(OwnerEdit == null) return null;
				return ((ColorEdit)OwnerEdit).PopupForm as PopupColorEditForm;
			}
		}
		bool ArraysEqual(Color[] a, Color[] b) {
			for(int i = 0; i < a.Length; i++) {
				if(a[i] != b[i]) return false;
			}
			return true;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditStoreColorAsInteger"),
#endif
 DefaultValue(false)]
		public virtual bool StoreColorAsInteger {
			get { return storeColorAsInteger; }
			set {
				if(StoreColorAsInteger == value) return;
				this.storeColorAsInteger = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditShowColorDialog"),
#endif
 DefaultValue(true), SmartTagProperty("Show Color Dialog", "")]
		public virtual bool ShowColorDialog {
			get { return showColorDialog; }
			set {
				if(ShowColorDialog == value) return;
				this.showColorDialog = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditShowCustomColors"),
#endif
 DefaultValue(true), SmartTagProperty("Show Custom Colors", "")]
		public virtual bool ShowCustomColors {
			get { return showCustomColors; }
			set {
				if(ShowCustomColors == value) return;
				this.showCustomColors = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditShowWebColors"),
#endif
 DefaultValue(true), SmartTagProperty("Show Web Colors", "")]
		public virtual bool ShowWebColors {
			get { return showWebColors; }
			set {
				if(ShowWebColors == value) return;
				this.showWebColors = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditShowSystemColors"),
#endif
 DefaultValue(true), SmartTagProperty("Show System Colors", "")]
		public virtual bool ShowSystemColors {
			get { return showSystemColors; }
			set {
				if(ShowSystemColors == value) return;
				this.showSystemColors = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormMinSize {
			get { return Size.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormSize {
			get { return Size.Empty; }
			set { }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxHighlightedItemStyle"),
#endif
 DefaultValue(HighlightStyle.Default)]
		public virtual HighlightStyle HighlightedItemStyle {
			get { return highlightedItemStyle; }
			set {
				if(HighlightedItemStyle == value) return;
				highlightedItemStyle = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditColorText"),
#endif
 DefaultValue(ColorText.Native), SmartTagProperty("Color Text", "")]
		public ColorText ColorText {
			get { return colorText; }
			set {
				if(ColorText == value) return;
				colorText = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditColorAlignment"),
#endif
 DefaultValue(HorzAlignment.Near), SmartTagProperty("Color Aligment", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public HorzAlignment ColorAlignment {
			get { return colorAlignment; }
			set {
				if(ColorAlignment == value) return;
				colorAlignment = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Near; } }
		protected internal virtual Color ConvertToColor(object editValue) {
			if(editValue is Color) return (Color)editValue;
			if(editValue is int) return Color.FromArgb((int)editValue);
			if(editValue is string) {
				try {
					return (Color)repositoryColorConverter.ConvertFromString(editValue.ToString());
				}
				catch {
				}
			}
			return Color.Empty;
		}
		protected internal virtual object ConvertToEditValue(object val) {
			if(StoreColorAsInteger) {
				if(val is int) return val;
				if(val is Color) {
					return ((Color)val).ToArgb();
				}
				if(val is string) {
					try {
						return ((Color)repositoryColorConverter.ConvertFromString(val.ToString())).ToArgb();
					}
					catch {
					}
				}
				return Color.Empty.ToArgb();
			}
			return ConvertToColor(val);
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemColorEdit source = item as RepositoryItemColorEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.colorAlignment = source.ColorAlignment;
				this.highlightedItemStyle = source.highlightedItemStyle;
				this.colorText = source.ColorText;
				this.customColors = source.CustomColors;
				this.storeColorAsInteger = source.StoreColorAsInteger;
				this.showColorDialog = source.ShowColorDialog;
				this.showCustomColors = source.ShowCustomColors;
				this.showWebColors = source.ShowWebColors;
				this.showSystemColors = source.ShowSystemColors;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(colorChanged, source.Events[colorChanged]);
		}
		protected override bool NeededKeysPopupContains(Keys key) {
			switch(key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case Keys.Enter:
				case Keys.Tab:
				case Keys.Tab | Keys.Shift:
					return true;
			}
			return base.NeededKeysPopupContains(key);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			ConvertEditValueEventArgs e = DoFormatEditValue(editValue);
			if(!(e.Value is Color) && !(e.Value is Int32)) {
				if(e.Handled) {
					if(e.Value is string) return (string)e.Value;
					if(e.Value == null || e.Value == DBNull.Value) return GetCustomDisplayValue(format, editValue, string.Empty);
					return GetCustomDisplayValue(format, editValue, e.Value.ToString());
				}
				return GetCustomDisplayValue(format, editValue, string.Empty);
			}
			Color clr = (e.Value is Color) ? (Color)e.Value : Color.FromArgb((Int32)e.Value);
			if(ColorText == ColorText.Integer) return GetCustomDisplayValue(format, editValue, clr.ToArgb().ToString());
			return GetCustomDisplayValue(format, editValue, GetColorDisplayText(clr));
		}
		string GetCustomDisplayValue(FormatInfo format, object editValue, string displayText) {
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(editValue, displayText);
			if(format != EditFormat)
				RaiseCustomDisplayText(e);
			return e.DisplayText;
		}
		static ColorConverter repositoryColorConverter = new ColorConverter();
		protected virtual string GetColorDisplayText(Color editValue) {
			return repositoryColorConverter.ConvertToString(editValue);
		}
		protected override bool AllowFormatEditValue { get { return false; } }
		protected override bool AllowParseEditValue { get { return false; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorEditColorChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ColorChanged {
			add { this.Events.AddHandler(colorChanged, value); }
			remove { this.Events.RemoveHandler(colorChanged, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseColorChanged(e);
		}
		protected internal virtual void RaiseColorChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[colorChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
	}
	public class RepositoryItemColorPickEdit : RepositoryItemColorEdit {
		Color autoColor, automaticBorderColor;
		string autoColorBtnCaption;
		bool showWebSafeColors;
		ColorTooltipFormat tooltipFormat;
		ColorPickEditControl.Matrix themeColors;
		ColorPickEditControl.Matrix standardColors;
		ColorPickEditControl.Matrix recentColors;
		static RepositoryItemColorPickEdit() {
			RegisterRepositoryItemColorPickEdit();
		}
		internal static void RegisterRepositoryItemColorPickEdit() {
			ColorPickEditRepositoryItemRegistrator.Register();
		}
		public RepositoryItemColorPickEdit() {
			this.autoColor = Color.Black;
			this.automaticBorderColor = Color.Empty;
			this.showWebSafeColors = false;
			this.tooltipFormat = ColorTooltipFormat.Argb;
			this.autoColorBtnCaption = DefaultAutomaticColorButtonCaption;
			this.ColorDialogOptions = CreateColorDialogOptions();
			this.themeColors = CreateThemeColors();
			this.standardColors = CreateStandardColors();
			this.recentColors = CreateRecentColors();
		}
		#region Events Ids
		static object tooltipShowing = new object();
		static object colorPickDialogShowing = new object();
		static object colorPickDialogClosed = new object();
		#endregion
		#region Events Locking
		bool eventsLock = false;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void LockEventsCore() {
			eventsLock = true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ReleaseEventsCore(bool reset) {
			this.eventsLock = false;
			if(reset && ShouldForceResetEvents) {
				ForceResetEvents();
			}
			this.resetEventsRequested = false;
		}
		bool resetEventsRequested = false;
		protected bool ShouldForceResetEvents {
			get {
				if(OwnerEdit == null) return false;
				return OwnerEdit.InplaceType != InplaceType.Standalone && resetEventsRequested;
			}
		}
		protected virtual void ForceResetEvents() {
			base.ResetEvents();
		}
		public override void ResetEvents() {
			if(eventsLock) {
				this.resetEventsRequested = true;
				return;
			}
			base.ResetEvents();
		}
		#endregion
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditAutomaticColor"),
#endif
 DefaultValue(typeof(Color), "Black"), SmartTagProperty("Automatic Color", "Editor Style")]
		public virtual Color AutomaticColor {
			get { return this.autoColor; }
			set {
				if(this.autoColor == value) return;
				this.autoColor = value;
				OnAutomaticColorChanged();
			}
		}
		protected virtual void OnAutomaticColorChanged() {
			if(OwnerEdit != null) OwnerEdit.OnAutomaticColorChanged();
			OnPropertiesChanged();
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditAutomaticBorderColor")]
#endif
		public virtual Color AutomaticBorderColor {
			get { return automaticBorderColor; }
			set {
				if(AutomaticBorderColor == value) return;
				automaticBorderColor = value;
				OnPropertiesChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditAutomaticColorButtonCaption")]
#endif
		public virtual string AutomaticColorButtonCaption {
			get { return this.autoColorBtnCaption; }
			set {
				if(this.autoColorBtnCaption == value) return;
				this.autoColorBtnCaption = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance),  DefaultValue(false), SmartTagProperty("Show Web Safe Color", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool ShowWebSafeColors {
			get { return this.showWebSafeColors; }
			set {
				if(this.showWebSafeColors == value) return;
				this.showWebSafeColors = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditTooltipFormat"),
#endif
 DefaultValue(ColorTooltipFormat.Argb)]
		public virtual ColorTooltipFormat TooltipFormat {
			get { return this.tooltipFormat; }
			set {
				if(this.tooltipFormat == value) return;
				this.tooltipFormat = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditThemeColors"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorPickEditControl.Matrix ThemeColors {
			get { return themeColors; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditStandardColors"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorPickEditControl.Matrix StandardColors {
			get { return standardColors; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditRecentColors"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorPickEditControl.Matrix RecentColors {
			get { return recentColors; }
		}
		protected virtual ColorPickEditControl.Matrix CreateThemeColors() {
			return ColorPickEditControl.Matrix.FromArray(ThemeColorsArray);
		}
		protected virtual ColorPickEditControl.Matrix CreateStandardColors() {
			return ColorPickEditControl.Matrix.FromArray(StandardColorsArray);
		}
		protected virtual ColorPickEditControl.Matrix CreateRecentColors() {
			return new ColorPickEditControl.Matrix(10, 1);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemColorPickEditColorDialogOptions"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorDialogOptions ColorDialogOptions { get; set; }
		[ DXCategory(CategoryName.Events)]
		public event EventHandler<ColorPickEditTooltipShowingEventArgs> TooltipShowing {
			add { Events.AddHandler(tooltipShowing, value); }
			remove { Events.RemoveHandler(tooltipShowing, value); }
		}
		[ DXCategory(CategoryName.Events)]
		public event EventHandler<ColorPickDialogShowingEventArgs> ColorPickDialogShowing {
			add { Events.AddHandler(colorPickDialogShowing, value); }
			remove { Events.RemoveHandler(colorPickDialogShowing, value); }
		}
		[ DXCategory(CategoryName.Events)]
		public event EventHandler<ColorPickDialogClosedEventArgs> ColorPickDialogClosed {
			add { Events.AddHandler(colorPickDialogClosed, value); }
			remove { Events.RemoveHandler(colorPickDialogClosed, value); }
		}
		protected virtual ColorDialogOptions CreateColorDialogOptions() {
			return new ColorDialogOptions();
		}
		#region ShouldSerialize & Reset
		protected virtual bool ShouldSerializeAutomaticColorButtonCaption() {
			return AutomaticColorButtonCaption != DefaultAutomaticColorButtonCaption;
		}
		protected virtual void ResetAutomaticColorButtonCaption() {
			AutomaticColorButtonCaption = DefaultAutomaticColorButtonCaption;
		}
		protected virtual bool ShouldSerializeAutomaticBorderColor() {
			return AutomaticBorderColor != Color.Empty;
		}
		protected virtual void ResetAutomaticBorderColor() {
			AutomaticBorderColor = Color.Empty;
		}
		#endregion
		#region Defaults
		protected virtual string DefaultAutomaticColorButtonCaption {
			get { return Localizer.Active.GetLocalizedString(StringId.ColorPickPopupAutomaticItemCaption); }
		}
		#endregion
		public override void Assign(RepositoryItem item) {
			RepositoryItemColorPickEdit source = item as RepositoryItemColorPickEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.autoColor = source.AutomaticColor;
				this.automaticBorderColor = source.AutomaticBorderColor;
				this.autoColorBtnCaption = source.AutomaticColorButtonCaption;
				this.showWebSafeColors = source.ShowWebSafeColors;
				this.tooltipFormat = source.TooltipFormat;
				this.ThemeColors.Assign(source.ThemeColors);
				this.StandardColors.Assign(source.StandardColors);
				this.RecentColors.Assign(source.RecentColors);
				Events.AddHandler(tooltipShowing, source.Events[tooltipShowing]);
				Events.AddHandler(colorPickDialogShowing, source.Events[colorPickDialogShowing]);
				Events.AddHandler(colorPickDialogClosed, source.Events[colorPickDialogClosed]);
				ColorDialogOptions.Assign(source.ColorDialogOptions);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void RaiseTooltipShowing(ColorPickEditTooltipShowingEventArgs e) {
			EventHandler<ColorPickEditTooltipShowingEventArgs> handler = (EventHandler<ColorPickEditTooltipShowingEventArgs>)this.Events[tooltipShowing];
			if(handler != null) handler(GetEventSender(), e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseColorPickDialogShowing(ColorPickDialogShowingEventArgs e) {
			EventHandler<ColorPickDialogShowingEventArgs> handler = (EventHandler<ColorPickDialogShowingEventArgs>)Events[colorPickDialogShowing];
			if(handler != null) handler(this, e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseColorPickDialogClosed(ColorPickDialogClosedEventArgs e) {
			EventHandler<ColorPickDialogClosedEventArgs> handler = (EventHandler<ColorPickDialogClosedEventArgs>)Events[colorPickDialogClosed];
			if(handler != null) handler(this, e);
		}
		public override string EditorTypeName {
			get { return "ColorPickEdit"; }
		}
		[Browsable(false)]
		public new ColorPickEdit OwnerEdit { get { return base.OwnerEdit as ColorPickEdit; } }
		#region Theme Colors
		Color[,] themeColorsArrayCore = null;
		protected Color[,] ThemeColorsArray {
			get {
				if(themeColorsArrayCore == null)
					themeColorsArrayCore = CreateThemeColorsCore();
				return themeColorsArrayCore;
			}
		}
		protected virtual Color[,] CreateThemeColorsCore() {
			return new Color[,] {
				{Color.FromArgb(0xFF, 0xFF, 0xFF),
				Color.FromArgb(0x00, 0x00, 0x00),
				Color.FromArgb(0xEE, 0xEC, 0xE1),
				Color.FromArgb(0x1F, 0x49, 0x7D),
				Color.FromArgb(0x4F, 0x81, 0xBD),
				Color.FromArgb(0xC0, 0x50, 0x4D),
				Color.FromArgb(0x9B, 0xBB, 0x59),
				Color.FromArgb(0x80, 0x64, 0xA2),
				Color.FromArgb(0x4B, 0xAC, 0xC6),
				Color.FromArgb(0xF7, 0x96, 0x46)},
				{Color.FromArgb(0xF2, 0xF2, 0xF2),
				Color.FromArgb(0x7F, 0x7F, 0x7F),
				Color.FromArgb(0xDD, 0xD9, 0xC3),
				Color.FromArgb(0xC6, 0xD9, 0xF0),
				Color.FromArgb(0xDB, 0xE5, 0xF1),
				Color.FromArgb(0xF2, 0xDC, 0xDB),
				Color.FromArgb(0xEB, 0xF1, 0xDD),
				Color.FromArgb(0xE5, 0xE0, 0xEC),
				Color.FromArgb(0xDB, 0xEE, 0xE3),
				Color.FromArgb(0xFD, 0xEA, 0xDA)},
				{Color.FromArgb(0xD8, 0xD8, 0xD8),
				Color.FromArgb(0x59, 0x59, 0x59),
				Color.FromArgb(0xC4, 0xBD, 0x97),
				Color.FromArgb(0x8D, 0xB3, 0xE2),
				Color.FromArgb(0xB8, 0xCC, 0xE4),
				Color.FromArgb(0xE5, 0xB9, 0xB7),
				Color.FromArgb(0xD7, 0xE3, 0xBC),
				Color.FromArgb(0xCC, 0xC1, 0xD9),
				Color.FromArgb(0xB7, 0xDD, 0xE8),
				Color.FromArgb(0xFB, 0xD5, 0xB5)},
				{Color.FromArgb(0xBF, 0xBF, 0xBF),
				Color.FromArgb(0x3F, 0x3F, 0x3F),
				Color.FromArgb(0x93, 0x89, 0x53),
				Color.FromArgb(0x54, 0x8D, 0xD4),
				Color.FromArgb(0x95, 0xB3, 0xD7),
				Color.FromArgb(0xD9, 0x96, 0x94),
				Color.FromArgb(0xC3, 0xD6, 0x9B),
				Color.FromArgb(0xB2, 0xA2, 0xC7),
				Color.FromArgb(0x92, 0xCD, 0xDC),
				Color.FromArgb(0xFA, 0xC0, 0x8F)},
				{Color.FromArgb(0xA5, 0xA5, 0xA5),
				Color.FromArgb(0x26, 0x26, 0x26),
				Color.FromArgb(0x49, 0x44, 0x29),
				Color.FromArgb(0x17, 0x36, 0x5D),
				Color.FromArgb(0x36, 0x60, 0x92),
				Color.FromArgb(0x95, 0x37, 0x34),
				Color.FromArgb(0x76, 0x92, 0x3C),
				Color.FromArgb(0x5F, 0x49, 0x7A),
				Color.FromArgb(0x31, 0x85, 0x9B),
				Color.FromArgb(0xE3, 0x6C, 0x09)},
				{Color.FromArgb(0x7F, 0x7F, 0x7F),
				Color.FromArgb(0x0C, 0x0C, 0x0C),
				Color.FromArgb(0x1D, 0x1B, 0x10),
				Color.FromArgb(0x0F, 0x24, 0x3E),
				Color.FromArgb(0x24, 0x40, 0x61),
				Color.FromArgb(0x63, 0x24, 0x23),
				Color.FromArgb(0x4F, 0x61, 0x28),
				Color.FromArgb(0x3F, 0x31, 0x51),
				Color.FromArgb(0x20, 0x58, 0x67),
				Color.FromArgb(0x97, 0x48, 0x06)}
			};
		}
		#endregion
		#region Standard Colors
		Color[,] standardColorsArrayCore = null;
		protected Color[,] StandardColorsArray {
			get {
				if(standardColorsArrayCore == null)
					standardColorsArrayCore = CreateStandardColorsCore();
				return standardColorsArrayCore;
			}
		}
		protected virtual Color[,] CreateStandardColorsCore() {
			return new Color[,] {
				{Color.FromArgb(0xC0, 0x00, 0x00),
				Color.FromArgb(0xF0, 0x00, 0x00),
				Color.FromArgb(0xFF, 0xC0, 0x00),
				Color.FromArgb(0xFF, 0xFF, 0x00),
				Color.FromArgb(0x92, 0xD0, 0x50),
				Color.FromArgb(0x00, 0xB0, 0x50),
				Color.FromArgb(0x00, 0xB0, 0xF0),
				Color.FromArgb(0x00, 0x70, 0xC0),
				Color.FromArgb(0x00, 0x20, 0x60),
				Color.FromArgb(0x70, 0x30, 0xA0)}
			};
		}
		#endregion
	}
}
namespace DevExpress.XtraEditors {
	[DefaultBindingPropertyEx("Color"), DefaultProperty("Color"),
	 Description("Displays colors in the drop-down window."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(ColorEditActions), "CustomColors", "Custom Colors", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ColorEdit")
	]
	public class ColorEdit : PopupBaseEdit {
		public ColorEdit() {
			fEditValue = fOldEditValue = Color.Empty;
		}
		protected internal override bool SuppressMouseWheel(MouseEventArgs e) {
			if(IsPopupOpen) {
				if(((PopupColorEditForm)PopupForm).TabControl.SelectedTabPageIndex != 0)
					return false;
			}
			return base.SuppressMouseWheel(e);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "ColorEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemColorEdit Properties { get { return base.Properties as RepositoryItemColorEdit; } }
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		bool ShouldSerializeEditValue() {
			if(!(EditValue is Color)) return true;
			return (Color != Color.Empty);
		}
		[Browsable(false)]
		public override object EditValue {
			get { return base.EditValue; }
			set { base.EditValue = Properties.ConvertToEditValue(value); }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorEditColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(ControlConstants.NonObjectBindable)]
		public Color Color {
			get { return Properties.ConvertToColor(EditValue); }
			set { EditValue = value; }
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupColorEditForm(this);
		}
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			return Properties.ConvertToColor(e.Value);
		}
		protected internal override bool AllowPopupTabOut { get { return false; } }
		protected override bool CanShowPopup {
			get {
				if(IsAllTabsHidden) return false;
				return base.CanShowPopup;
			}
		}
		protected virtual bool IsAllTabsHidden {
			get {
				return !Properties.ShowCustomColors && !Properties.ShowWebColors && !Properties.ShowSystemColors;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorEditColorChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ColorChanged {
			add { Properties.ColorChanged += value; }
			remove { Properties.ColorChanged -= value; }
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseWheel(ee);
				if(ee.Handled || Properties.ReadOnly || !Properties.AllowMouseWheel) return;
				int step = SystemInformation.MouseWheelScrollLines > 0 && IsPopupOpen ? SystemInformation.MouseWheelScrollLines : 1;
				if(e.Delta > 0) step *= -1;
				PopupColorEditForm form = PopupForm as PopupColorEditForm;
				if(IsPopupOpen && form != null && form.SelectedPageControl != null) {
					ee.Handled = true;
					ColorListBox colorListBox = form.SelectedPageControl as ColorListBox;
					if(colorListBox != null) {
						colorListBox.TopIndex += step;
					}
					else {
						ColorCellsControl colorCellControl = form.SelectedPageControl as ColorCellsControl;
						if(colorCellControl != null)
							colorCellControl.SelectedCellIndex += Math.Sign(step);
					}
				}
			}
			finally {
				ee.Sync();
			}
		}
	}
	[ToolboxItem(false), SmartTagFilter(typeof(ColorPickEditBaseFilter))]
	public class ColorPickEditBase : ColorEdit {
		public ColorPickEditBase() {
		}
		[Browsable(false)]
		public override string EditorTypeName {
			get { return "ColorPickEdit"; }
		}
		[ DXCategory(CategoryName.Events)]
		public event EventHandler<ColorPickEditTooltipShowingEventArgs> TooltipShowing {
			add { Properties.TooltipShowing += value; }
			remove { Properties.TooltipShowing -= value; }
		}
		[ DXCategory(CategoryName.Events)]
		public event EventHandler<ColorPickDialogShowingEventArgs> ColorPickDialogShowing {
			add { Properties.ColorPickDialogShowing += value; }
			remove { Properties.ColorPickDialogShowing -= value; }
		}
		[ DXCategory(CategoryName.Events)]
		public event EventHandler<ColorPickDialogClosedEventArgs> ColorPickDialogClosed {
			add { Properties.ColorPickDialogClosed += value; }
			remove { Properties.ColorPickDialogClosed -= value; }
		}
		[ DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemColorPickEdit Properties {
			get { return base.Properties as RepositoryItemColorPickEdit; }
		}
	}
	public class ColorPickEditTooltipShowingEventArgs : EventArgs {
		public ColorPickEditTooltipShowingEventArgs(Color color, string titleText, string contentText, ColorTooltipFormat format) {
			this.Color = color;
			this.TitleText = titleText;
			this.ContentText = contentText;
			this.Format = format;
		}
		public string TitleText { get; set; }
		public string ContentText { get; set; }
		public Color Color { get; private set; }
		public ColorTooltipFormat Format { get; private set; }
	}
	public class ColorPickDialogShowingEventArgs : EventArgs {
		XtraForm form;
		public ColorPickDialogShowingEventArgs(XtraForm form) {
			this.form = form;
		}
		public XtraForm Form { get { return form; } }
	}
	public class ColorPickDialogClosedEventArgs : EventArgs {
		DialogResult result;
		public ColorPickDialogClosedEventArgs(DialogResult result) {
			this.result = result;
		}
		public DialogResult Result { get { return result; } }
	}
	public enum ShowTabs { All, RGBModel, HSBModel }
	public enum ShowArrows { Default, True, False }
	public enum ColorTooltipFormat { Argb, Hex }
	namespace ColorPickEditControl {
		public class Matrix {
			public class MatrixItem {
				Color value;
				bool initialized;
				public void SetValue(Color value) {
					this.initialized = true;
					this.value = value;
				}
				public void Reset() {
					this.initialized = false;
					this.value = Color.Empty;
				}
				public override string ToString() {
					return string.Format("Value: {0}, IsInitialized: {1}", Value.ToString(), Initialized.ToString());
				}
				public Color Value { get { return value; } }
				public bool Initialized { get { return initialized; } }
			}
			MatrixItem[,] matrix;
			int rowCount, columnCount;
			public Matrix(int columnCount, int rowCount) {
				this.columnCount = columnCount;
				this.rowCount = rowCount;
				this.matrix = new MatrixItem[RowCount, ColumnCount];
				for(int i = 0; i < RowCount; i++) {
					for(int j = 0; j < ColumnCount; j++) {
						this.matrix[i, j] = new MatrixItem();
					}
				}
			}
			public int ColumnCount { get { return columnCount; } }
			public int RowCount { get { return rowCount; } }
			public int Size { get { return ColumnCount * RowCount; } }
			public Color this[int i, int j] {
				get { return matrix[i, j].Value; }
				set {
					if(i >= RowCount)
						throw new IndexOutOfRangeException("i");
					if(j >= ColumnCount)
						throw new IndexOutOfRangeException("j");
					matrix[i, j].SetValue(value);
				}
			}
			public bool IsEmpty {
				get {
					for(int i = 0; i < RowCount; i++) {
						for(int j = 0; j < ColumnCount; j++) {
							if(matrix[i, j].Initialized) return false;
						}
					}
					return true;
				}
			}
			public void Clear() {
				ForEachItem(item => item.Reset());
			}
			protected bool IsInitialized(int i, int j) {
				if(i >= RowCount)
					throw new IndexOutOfRangeException("i");
				if(j >= ColumnCount)
					throw new IndexOutOfRangeException("j");
				return matrix[i, j].Initialized;
			}
			public void InsertColor(Color value, int row) {
				if(ColumnCount == 0) return;
				MatrixItem[] newRow = new MatrixItem[ColumnCount];
				for(int i = 0; i < ColumnCount; i++) {
					newRow[i] = new MatrixItem();
				}
				for(int i = 1; i < ColumnCount; i++) {
					if(IsInitialized(row, i - 1)) newRow[i].SetValue(this[row, i - 1]);
				}
				newRow[0].SetValue(value);
				for(int i = 0; i < ColumnCount; i++) {
					if(newRow[i].Initialized) this[row, i] = newRow[i].Value;
				}
			}
			public void InsertColor(Color value) {
				InsertColor(value, 0);
			}
			[EditorBrowsable(EditorBrowsableState.Never)]
			public void ForEachItem(Action<MatrixItem> callback) {
				ForEachItem(callback, false);
			}
			[EditorBrowsable(EditorBrowsableState.Never)]
			public void ForEachItem(Action<MatrixItem> callback, bool filterEmptyItems) {
				for(int i = 0; i < RowCount; i++) {
					for(int j = 0; j < ColumnCount; j++) {
						var matrixItem = matrix[i, j];
						if(matrixItem.Initialized || !filterEmptyItems) callback(matrixItem);
					}
				}
			}
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static Matrix FromArray(Color[,] source) {
				int width = source.GetUpperBound(1) + 1;
				int height = source.GetUpperBound(0) + 1;
				Matrix matrix = new Matrix(width, height);
				for(int i = 0; i < matrix.RowCount; i++) {
					for(int j = 0; j < matrix.ColumnCount; j++) {
						matrix[i, j] = source[i, j];
					}
				}
				return matrix;
			}
			[EditorBrowsable(EditorBrowsableState.Never)]
			public virtual void Assign(Matrix source) {
				if(source.RowCount != RowCount || source.ColumnCount != ColumnCount)
					return;
				for(int i = 0; i < RowCount; i++) {
					for(int j = 0; j < ColumnCount; j++) {
						if(source.IsInitialized(i, j)) this[i, j] = source[i, j];
					}
				}
			}
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ReadOnlyCollection<Color> ToList(bool filterEmptyItems = false) {
				List<Color> list = new List<Color>();
				ForEachItem(item => list.Add(item.Value), filterEmptyItems);
				return new ReadOnlyCollection<Color>(list);
			}
			public override string ToString() {
				return string.Format("{0}, {1}", RowCount, ColumnCount);
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColorDialogOptions {
		public ColorDialogOptions() {
			this.ShowTabs = ShowTabs.All;
			this.ShowArrows = ShowArrows.Default;
			this.ShowMakeWebSafeButton = this.ShowPreview = this.AllowTransparency = true;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorDialogOptionsShowTabs"),
#endif
 DefaultValue(ShowTabs.All)]
		public virtual ShowTabs ShowTabs {
			get;
			set;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorDialogOptionsShowMakeWebSafeButton"),
#endif
 DefaultValue(true)]
		public virtual bool ShowMakeWebSafeButton {
			get;
			set;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorDialogOptionsShowPreview"),
#endif
 DefaultValue(true)]
		public virtual bool ShowPreview {
			get;
			set;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorDialogOptionsAllowTransparency"),
#endif
 DefaultValue(true)]
		public virtual bool AllowTransparency {
			get;
			set;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorDialogOptionsShowArrows"),
#endif
 DefaultValue(ShowArrows.Default)]
		public virtual ShowArrows ShowArrows {
			get;
			set;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ColorDialogOptionsFormIcon"),
#endif
 DefaultValue(null)]
		public virtual Icon FormIcon {
			get;
			set;
		}
		#region Should Serialize & Reset
		protected virtual bool ShouldSerializeShowTabs() {
			return true;
		}
		protected virtual void ResetShowTabs() {
			ShowTabs = ShowTabs.All;
		}
		protected virtual bool ShouldSerializeShowMakeWebSafeButton() {
			return true;
		}
		protected virtual void ResetShowMakeWebSafeButton() {
			ShowMakeWebSafeButton = true;
		}
		protected virtual bool ShouldSerializeShowPreview() {
			return true;
		}
		protected virtual void ResetShowPreview() {
			ShowPreview = true;
		}
		protected virtual bool ShouldSerializeAllowTransparency() {
			return true;
		}
		protected virtual void ResetAllowTransparency() {
			AllowTransparency = true;
		}
		protected virtual bool ShouldSerializeShowArrows() {
			return true;
		}
		protected virtual void ResetShowArrows() {
			ShowArrows = ShowArrows.Default;
		}
		#endregion
		#region Assign
		public virtual void Assign(ColorDialogOptions options) {
			ShowTabs = options.ShowTabs;
			ShowMakeWebSafeButton = options.ShowMakeWebSafeButton;
			ShowPreview = options.ShowPreview;
			AllowTransparency = options.AllowTransparency;
			ShowArrows = options.ShowArrows;
			FormIcon = options.FormIcon != null ? options.FormIcon.Clone() as Icon : null;
		}
		#endregion
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class ColorEditViewInfo : PopupBaseEditViewInfo {
		protected Color fColor;
		protected Rectangle fColorBoxRect, fColorTextRect;
		protected const int ColorIndent = 2;
		protected const int ColorBoxWidth = 22;
		public ColorEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			ColorEditViewInfo be = info as ColorEditViewInfo;
			if(be == null) return;
			this.fColor = be.fColor;
			this.fColorBoxRect = be.fColorBoxRect;
			this.fColorTextRect = be.fColorTextRect;
		}
		public virtual Color Color { get { return fColor; } }
		public override void Reset() {
			this.fColor = Color.Empty;
			base.Reset();
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			CalcColorBoxRect();
			CalcColorTextRect();
			this.fMaskBoxRect = ColorTextRect;
		}
		protected override bool IsTextToolTipPoint(Point point) {
			return ContentRect.Contains(point) && !MaskBoxRect.IsEmpty;
		}
		protected virtual void CalcColorBoxRect() {
			if(Item.ColorAlignment == HorzAlignment.Default) return;
			fColorBoxRect = Rectangle.Inflate(MaskBoxRect, -ColorIndent, -ColorIndent);
			fColorBoxRect.Width = ColorBoxWidth;
			if((Item.ColorAlignment == HorzAlignment.Far && !RightToLeft) ||
			   (Item.ColorAlignment == HorzAlignment.Near && RightToLeft)) fColorBoxRect.X = MaskBoxRect.Right - ColorIndent - fColorBoxRect.Width;
			if(Item.ColorAlignment == HorzAlignment.Center) fColorBoxRect.X = MaskBoxRect.Left + (MaskBoxRect.Width - fColorBoxRect.Width) / 2;
		}
		protected virtual void CalcColorTextRect() {
			if(Item.ColorAlignment == HorzAlignment.Default) fColorTextRect = MaskBoxRect;
			if(Item.ColorAlignment != HorzAlignment.Center) {
				fColorTextRect = new Rectangle(ColorBoxRect.Right + 2 * ColorIndent, MaskBoxRect.Top, MaskBoxRect.Width - ColorBoxRect.Width - 3 * ColorIndent, MaskBoxRect.Height);
				if((Item.ColorAlignment == HorzAlignment.Far && !RightToLeft) ||
				   (Item.ColorAlignment == HorzAlignment.Near && RightToLeft)) fColorTextRect = new Rectangle(MaskBoxRect.Left, MaskBoxRect.Top, MaskBoxRect.Width - ColorBoxRect.Width - 3 * ColorIndent, MaskBoxRect.Height);
			}
			if(ColorTextRect.Width <= 0) fColorTextRect = Rectangle.Empty;
		}
		public override void Clear() {
			this.fColorBoxRect = this.fColorTextRect = Rectangle.Empty;
			base.Clear();
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			if(!ColorBoxRect.IsEmpty) fColorBoxRect.Offset(x, y);
			if(!ColorTextRect.IsEmpty) fColorTextRect.Offset(x, y);
		}
		public Rectangle ColorBoxRect { get { return fColorBoxRect; } }
		public Rectangle ColorTextRect { get { return fColorTextRect; } }
		public new RepositoryItemColorEdit Item { get { return base.Item as RepositoryItemColorEdit; } }
		protected override void OnEditValueChanged() {
			this.RefreshDisplayText = false;
			this.fColor = Item.ConvertToColor(EditValue);
			this.fDisplayText = Item.GetDisplayText(Format, Color);
		}
		internal bool IsCenterColorAlignment {
			get { return Item.ColorAlignment == HorzAlignment.Center; }
		}
		protected override Size CalcClientSize(Graphics g) {
			Size size = base.CalcClientSize(g);
			size.Width = size.Width + ColorBoxWidth + ColorIndent * 3;
			if(IsCenterColorAlignment) size.Width = size.Width - TextSize.Width;
			return size;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class ColorEditPainter : ButtonEditPainter {
		protected override void DrawTextBoxArea(ControlGraphicsInfoArgs info) {
			base.DrawTextBoxArea(info);
			if(((ColorEditViewInfo)info.ViewInfo).IsCenterColorAlignment) {
				TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
				Brush brush = vi.PaintAppearance.GetBackBrush(info.Cache, vi.Bounds);
				info.Paint.FillRectangle(info.Graphics, brush, vi.ContentRect);
			}
		}
		protected override void DrawText(ControlGraphicsInfoArgs info) {
			DrawColorText(info);
		}
		protected override void DrawButtons(ControlGraphicsInfoArgs info) {
			DrawColorBox(info);
			base.DrawButtons(info);
		}
		protected virtual void DrawColorBox(ControlGraphicsInfoArgs info) {
			ColorEditViewInfo vi = info.ViewInfo as ColorEditViewInfo;
			if(vi.ColorBoxRect.IsEmpty) return;
			info.Paint.FillRectangle(info.Graphics, info.Cache.GetSolidBrush(vi.Color), vi.ColorBoxRect);
			info.Paint.DrawRectangle(info.Graphics, info.Cache.GetPen(vi.GetSystemColor(SystemColors.WindowText)), vi.ColorBoxRect);
		}
		protected virtual void DrawColorText(ControlGraphicsInfoArgs info) {
			ColorEditViewInfo vi = info.ViewInfo as ColorEditViewInfo;
			if(vi.ColorTextRect.IsEmpty) return;
			DrawString(info, vi.ColorTextRect, vi.DisplayText, vi.PaintAppearance);
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	public delegate void EnterColorEventHandler(object sender, EnterColorEventArgs e);
	public class EnterColorEventArgs : EventArgs {
		Color color;
		public EnterColorEventArgs(Color color) {
			this.color = color;
		}
		public Color Color { get { return color; } }
	}
	public class PainterColorListBox :  BaseListBoxPainter {
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			ColorListBoxViewInfo.ColorItemInfo colorItemInfo = (ColorListBoxViewInfo.ColorItemInfo)itemInfo;
			ColorListBoxViewInfo vi = info.ViewInfo as ColorListBoxViewInfo;
			base.DrawItemCore(info, itemInfo, e);
			info.Cache.FillRectangle(info.Cache.GetSolidBrush(colorItemInfo.Color), colorItemInfo.ColorRect);
			info.Cache.DrawRectangle(info.Cache.GetPen(vi.GetSystemColor(SystemColors.WindowText)), colorItemInfo.ColorRect);
		}
	}
	public class ColorListBoxViewInfo : BaseListBoxViewInfo {
		static object[] webColors, systemColors, webSafeColors;
		public const int ColorIndent = 2;
		public const int ColorRectWidth = 22;
		public ColorListBoxViewInfo(ColorListBox listBox) : base(listBox) { }
		public virtual int GetNearestBestClientHeight(int height) {
			int rows = height / ItemHeight;
			if(rows * ItemHeight == height) return height;
			return (rows + 1) * ItemHeight;
		}
		protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			Rectangle textRect = new Rectangle(bounds.Left + FullColorPlaceWidth, bounds.Top, bounds.Width - FullColorPlaceWidth, bounds.Height);
			if(RightToLeft) textRect.X -= FullColorPlaceWidth; 
			ColorItemInfo itemInfo = new ColorItemInfo(OwnerControl, textRect, item, string.Empty, index);
			itemInfo.Bounds = bounds;
			itemInfo.ColorRect = new Rectangle(new Point(bounds.Left + ColorIndent, bounds.Top + (bounds.Height - GetColorRectSize(bounds).Height) / 2), GetColorRectSize(bounds));
			if(RightToLeft) itemInfo.ColorRect.X += textRect.Width; 
			Color color = (Color)OwnerControl.Items[index];
			itemInfo.Color = color;
			itemInfo.Text = OwnerControl.GetColorName(color);
			return itemInfo;
		}
		protected virtual Size GetColorRectSize(Rectangle bounds) { return new Size(ColorRectWidth, Math.Min(8, bounds.Height - 2 * ColorIndent)); }
		protected new ColorListBox OwnerControl { get { return base.OwnerControl as ColorListBox; } }
		protected int FullColorPlaceWidth { get { return ColorRectWidth + 2 * ColorIndent; } }
		public class ColorItemInfo : ItemInfo {
			public Rectangle ColorRect;
			public Color Color;
			public ColorItemInfo(BaseListBoxControl ownerControl, Rectangle rect, object item, string text, int index)
				: base(ownerControl, rect, item, text, index) {
				ColorRect = Rectangle.Empty;
				Color = Color.Empty;
			}
		}
		#region Color Data
		public static object[] SystemColors {
			get {
				if(systemColors == null) systemColors = new object[] { System.Drawing.SystemColors.ActiveBorder,
																		System.Drawing.SystemColors.ActiveCaption,
																		System.Drawing.SystemColors.ActiveCaptionText,
																		System.Drawing.SystemColors.AppWorkspace,
																		System.Drawing.SystemColors.Control,
																		System.Drawing.SystemColors.ControlDark,
																		System.Drawing.SystemColors.ControlDarkDark,
																		System.Drawing.SystemColors.ControlLight,
																		System.Drawing.SystemColors.ControlLightLight,
																		System.Drawing.SystemColors.ControlText,
																		System.Drawing.SystemColors.Desktop,
																		System.Drawing.SystemColors.GrayText,
																		System.Drawing.SystemColors.Highlight,
																		System.Drawing.SystemColors.HighlightText,
																		System.Drawing.SystemColors.HotTrack,
																		System.Drawing.SystemColors.InactiveBorder,
																		System.Drawing.SystemColors.InactiveCaption,
																		System.Drawing.SystemColors.InactiveCaptionText,
																		System.Drawing.SystemColors.Info,
																		System.Drawing.SystemColors.InfoText,
																		System.Drawing.SystemColors.Menu,
																		System.Drawing.SystemColors.MenuText,
																		System.Drawing.SystemColors.ScrollBar,
																		System.Drawing.SystemColors.Window,
																		System.Drawing.SystemColors.WindowFrame,
																		System.Drawing.SystemColors.WindowText};
				return systemColors;
			}
		}
		public static object[] WebColors {
			get {
				if(webColors == null) webColors = new object[] {Color.Transparent,
																  Color.Black,
																  Color.DimGray,
																  Color.Gray,
																  Color.DarkGray,
																  Color.Silver,
																  Color.LightGray,
																  Color.Gainsboro,
																  Color.WhiteSmoke,
																  Color.White,
																  Color.RosyBrown,
																  Color.IndianRed,
																  Color.Brown,
																  Color.Firebrick,
																  Color.LightCoral,
																  Color.Maroon,
																  Color.DarkRed,
																  Color.Red,
																  Color.Snow,
																  Color.MistyRose,
																  Color.Salmon,
																  Color.Tomato,
																  Color.DarkSalmon,
																  Color.Coral,
																  Color.OrangeRed,
																  Color.LightSalmon,
																  Color.Sienna,
																  Color.SeaShell,
																  Color.Chocolate,
																  Color.SaddleBrown,
																  Color.SandyBrown,
																  Color.PeachPuff,
																  Color.Peru,
																  Color.Linen,
																  Color.Bisque,
																  Color.DarkOrange,
																  Color.BurlyWood,
																  Color.Tan,
																  Color.AntiqueWhite,
																  Color.NavajoWhite,
																  Color.BlanchedAlmond,
																  Color.PapayaWhip,
																  Color.Moccasin,
																  Color.Orange,
																  Color.Wheat,
																  Color.OldLace,
																  Color.FloralWhite,
																  Color.DarkGoldenrod,
																  Color.Goldenrod,
																  Color.Cornsilk,
																  Color.Gold,
																  Color.Khaki,
																  Color.LemonChiffon,
																  Color.PaleGoldenrod,
																  Color.DarkKhaki,
																  Color.Beige,
																  Color.LightGoldenrodYellow,
																  Color.Olive,
																  Color.Yellow,
																  Color.LightYellow,
																  Color.Ivory,
																  Color.OliveDrab,
																  Color.YellowGreen,
																  Color.DarkOliveGreen,
																  Color.GreenYellow,
																  Color.Chartreuse,
																  Color.LawnGreen,
																  Color.DarkSeaGreen,
																  Color.ForestGreen,
																  Color.LimeGreen,
																  Color.LightGreen,
																  Color.PaleGreen,
																  Color.DarkGreen,
																  Color.Green,
																  Color.Lime,
																  Color.Honeydew,
																  Color.SeaGreen,
																  Color.MediumSeaGreen,
																  Color.SpringGreen,
																  Color.MintCream,
																  Color.MediumSpringGreen,
																  Color.MediumAquamarine,
																  Color.Aquamarine,
																  Color.Turquoise,
																  Color.LightSeaGreen,
																  Color.MediumTurquoise,
																  Color.DarkSlateGray,
																  Color.PaleTurquoise,
																  Color.Teal,
																  Color.DarkCyan,
																  Color.Aqua,
																  Color.Cyan,
																  Color.LightCyan,
																  Color.Azure,
																  Color.DarkTurquoise,
																  Color.CadetBlue,
																  Color.PowderBlue,
																  Color.LightBlue,
																  Color.DeepSkyBlue,
																  Color.SkyBlue,
																  Color.LightSkyBlue,
																  Color.SteelBlue,
																  Color.AliceBlue,
																  Color.DodgerBlue,
																  Color.SlateGray,
																  Color.LightSlateGray,
																  Color.LightSteelBlue,
																  Color.CornflowerBlue,
																  Color.RoyalBlue,
																  Color.MidnightBlue,
																  Color.Lavender,
																  Color.Navy,
																  Color.DarkBlue,
																  Color.MediumBlue,
																  Color.Blue,
																  Color.GhostWhite,
																  Color.SlateBlue,
																  Color.DarkSlateBlue,
																  Color.MediumSlateBlue,
																  Color.MediumPurple,
																  Color.BlueViolet,
																  Color.Indigo,
																  Color.DarkOrchid,
																  Color.DarkViolet,
																  Color.MediumOrchid,
																  Color.Thistle,
																  Color.Plum,
																  Color.Violet,
																  Color.Purple,
																  Color.DarkMagenta,
																  Color.Magenta,
																  Color.Fuchsia,
																  Color.Orchid,
																  Color.MediumVioletRed,
																  Color.DeepPink,
																  Color.HotPink,
																  Color.LavenderBlush,
																  Color.PaleVioletRed,
																  Color.Crimson,
																  Color.Pink,
																  Color.LightPink };
				return webColors;
			}
		}
		public static object[] WebSafeColors {
			get {
				if(webSafeColors == null) webSafeColors = CreateWebSafeColors();
				return webSafeColors;
			}
		}
		#region WebSafe Colors
		static object[] CreateWebSafeColors() {
			unchecked {
				return new object[] {
					Color.FromArgb((int)0xFF000000), Color.FromArgb((int)0xFF000033), Color.FromArgb((int)0xFF000066), Color.FromArgb((int)0xFF000099), Color.FromArgb((int)0xFF0000CC), Color.FromArgb((int)0xFF0000FF),
					Color.FromArgb((int)0xFF003300), Color.FromArgb((int)0xFF003333), Color.FromArgb((int)0xFF003366), Color.FromArgb((int)0xFF003399), Color.FromArgb((int)0xFF0033CC), Color.FromArgb((int)0xFF0033FF),
					Color.FromArgb((int)0xFF006600), Color.FromArgb((int)0xFF006633), Color.FromArgb((int)0xFF006666), Color.FromArgb((int)0xFF006699), Color.FromArgb((int)0xFF0066CC), Color.FromArgb((int)0xFF0066FF),
					Color.FromArgb((int)0xFF009900), Color.FromArgb((int)0xFF009933), Color.FromArgb((int)0xFF009966), Color.FromArgb((int)0xFF009999), Color.FromArgb((int)0xFF0099CC), Color.FromArgb((int)0xFF0099FF),
					Color.FromArgb((int)0xFF00CC00), Color.FromArgb((int)0xFF00CC33), Color.FromArgb((int)0xFF00CC66), Color.FromArgb((int)0xFF00CC99),	Color.FromArgb((int)0xFF00CCCC), Color.FromArgb((int)0xFF00CCFF),
					Color.FromArgb((int)0xFF00FF00), Color.FromArgb((int)0xFF00FF33), Color.FromArgb((int)0xFF00FF66), Color.FromArgb((int)0xFF00FF99),	Color.FromArgb((int)0xFF00FFCC), Color.FromArgb((int)0xFF00FFFF),
					Color.FromArgb((int)0xFF330000), Color.FromArgb((int)0xFF330033), Color.FromArgb((int)0xFF330066), Color.FromArgb((int)0xFF330099), Color.FromArgb((int)0xFF3300CC), Color.FromArgb((int)0xFF3300FF),
					Color.FromArgb((int)0xFF333300), Color.FromArgb((int)0xFF333333), Color.FromArgb((int)0xFF333366), Color.FromArgb((int)0xFF333399),	Color.FromArgb((int)0xFF3333CC), Color.FromArgb((int)0xFF3333FF),
					Color.FromArgb((int)0xFF336600), Color.FromArgb((int)0xFF336633), Color.FromArgb((int)0xFF336666), Color.FromArgb((int)0xFF336699),	Color.FromArgb((int)0xFF3366CC), Color.FromArgb((int)0xFF3366FF),
					Color.FromArgb((int)0xFF339900), Color.FromArgb((int)0xFF339933), Color.FromArgb((int)0xFF339966), Color.FromArgb((int)0xFF339999),	Color.FromArgb((int)0xFF3399CC), Color.FromArgb((int)0xFF3399FF),
					Color.FromArgb((int)0xFF33CC00), Color.FromArgb((int)0xFF33CC33), Color.FromArgb((int)0xFF33CC66), Color.FromArgb((int)0xFF33CC99),	Color.FromArgb((int)0xFF33CCCC), Color.FromArgb((int)0xFF33CCFF),
					Color.FromArgb((int)0xFF33FF00), Color.FromArgb((int)0xFF33FF33), Color.FromArgb((int)0xFF33FF66), Color.FromArgb((int)0xFF33FF99),	Color.FromArgb((int)0xFF33FFCC), Color.FromArgb((int)0xFF33FFFF),
					Color.FromArgb((int)0xFF660000), Color.FromArgb((int)0xFF660033), Color.FromArgb((int)0xFF660066), Color.FromArgb((int)0xFF660099),	Color.FromArgb((int)0xFF6600CC), Color.FromArgb((int)0xFF6600FF),
					Color.FromArgb((int)0xFF663300), Color.FromArgb((int)0xFF663333), Color.FromArgb((int)0xFF663366), Color.FromArgb((int)0xFF663399),	Color.FromArgb((int)0xFF6633CC), Color.FromArgb((int)0xFF6633FF),
					Color.FromArgb((int)0xFF666600), Color.FromArgb((int)0xFF666633), Color.FromArgb((int)0xFF666666), Color.FromArgb((int)0xFF666699),	Color.FromArgb((int)0xFF6666CC), Color.FromArgb((int)0xFF6666FF),
					Color.FromArgb((int)0xFF669900), Color.FromArgb((int)0xFF669933), Color.FromArgb((int)0xFF669966), Color.FromArgb((int)0xFF669999),	Color.FromArgb((int)0xFF6699CC), Color.FromArgb((int)0xFF6699FF),
					Color.FromArgb((int)0xFF66CC00), Color.FromArgb((int)0xFF66CC33), Color.FromArgb((int)0xFF66CC66), Color.FromArgb((int)0xFF66CC99),	Color.FromArgb((int)0xFF66CCCC), Color.FromArgb((int)0xFF66CCFF),
					Color.FromArgb((int)0xFF66FF00), Color.FromArgb((int)0xFF66FF33), Color.FromArgb((int)0xFF66FF66), Color.FromArgb((int)0xFF66FF99),	Color.FromArgb((int)0xFF66FFCC), Color.FromArgb((int)0xFF66FFFF),
					Color.FromArgb((int)0xFF990000), Color.FromArgb((int)0xFF990033), Color.FromArgb((int)0xFF990066), Color.FromArgb((int)0xFF990099),	Color.FromArgb((int)0xFF9900CC), Color.FromArgb((int)0xFF9900FF),
					Color.FromArgb((int)0xFF993300), Color.FromArgb((int)0xFF993333), Color.FromArgb((int)0xFF993366), Color.FromArgb((int)0xFF993399),	Color.FromArgb((int)0xFF9933CC), Color.FromArgb((int)0xFF9933FF),
					Color.FromArgb((int)0xFF996600), Color.FromArgb((int)0xFF996633), Color.FromArgb((int)0xFF996666), Color.FromArgb((int)0xFF996699),	Color.FromArgb((int)0xFF9966CC), Color.FromArgb((int)0xFF9966FF),
					Color.FromArgb((int)0xFF999900), Color.FromArgb((int)0xFF999933), Color.FromArgb((int)0xFF999966), Color.FromArgb((int)0xFF999999),	Color.FromArgb((int)0xFF9999CC), Color.FromArgb((int)0xFF9999FF),
					Color.FromArgb((int)0xFF99CC00), Color.FromArgb((int)0xFF99CC33), Color.FromArgb((int)0xFF99CC66), Color.FromArgb((int)0xFF99CC99),	Color.FromArgb((int)0xFF99CCCC), Color.FromArgb((int)0xFF99CCFF),
					Color.FromArgb((int)0xFF99FF00), Color.FromArgb((int)0xFF99FF33), Color.FromArgb((int)0xFF99FF66), Color.FromArgb((int)0xFF99FF99),	Color.FromArgb((int)0xFF99FFCC), Color.FromArgb((int)0xFF99FFFF),
					Color.FromArgb((int)0xFFCC0000), Color.FromArgb((int)0xFFCC0033), Color.FromArgb((int)0xFFCC0066), Color.FromArgb((int)0xFFCC0099),	Color.FromArgb((int)0xFFCC00CC), Color.FromArgb((int)0xFFCC00FF),
					Color.FromArgb((int)0xFFCC3300), Color.FromArgb((int)0xFFCC3333), Color.FromArgb((int)0xFFCC3366), Color.FromArgb((int)0xFFCC3399),	Color.FromArgb((int)0xFFCC33CC), Color.FromArgb((int)0xFFCC33FF),
					Color.FromArgb((int)0xFFCC6600), Color.FromArgb((int)0xFFCC6633), Color.FromArgb((int)0xFFCC6666), Color.FromArgb((int)0xFFCC6699),	Color.FromArgb((int)0xFFCC66CC), Color.FromArgb((int)0xFFCC66FF),
					Color.FromArgb((int)0xFFCC9900), Color.FromArgb((int)0xFFCC9933), Color.FromArgb((int)0xFFCC9966), Color.FromArgb((int)0xFFCC9999),	Color.FromArgb((int)0xFFCC99CC), Color.FromArgb((int)0xFFCC99FF),
					Color.FromArgb((int)0xFFCCCC00), Color.FromArgb((int)0xFFCCCC33), Color.FromArgb((int)0xFFCCCC66), Color.FromArgb((int)0xFFCCCC99),	Color.FromArgb((int)0xFFCCCCCC), Color.FromArgb((int)0xFFCCCCFF),
					Color.FromArgb((int)0xFFCCFF00), Color.FromArgb((int)0xFFCCFF33), Color.FromArgb((int)0xFFCCFF66), Color.FromArgb((int)0xFFCCFF99),	Color.FromArgb((int)0xFFCCFFCC), Color.FromArgb((int)0xFFCCFFFF),
					Color.FromArgb((int)0xFFFF0000), Color.FromArgb((int)0xFFFF0033), Color.FromArgb((int)0xFFFF0066), Color.FromArgb((int)0xFFFF0099),	Color.FromArgb((int)0xFFFF00CC), Color.FromArgb((int)0xFFFF00FF),
					Color.FromArgb((int)0xFFFF3300), Color.FromArgb((int)0xFFFF3333), Color.FromArgb((int)0xFFFF3366), Color.FromArgb((int)0xFFFF3399),	Color.FromArgb((int)0xFFFF33CC), Color.FromArgb((int)0xFFFF33FF),
					Color.FromArgb((int)0xFFFF6600), Color.FromArgb((int)0xFFFF6633), Color.FromArgb((int)0xFFFF6666), Color.FromArgb((int)0xFFFF6699),	Color.FromArgb((int)0xFFFF66CC), Color.FromArgb((int)0xFFFF66FF),
					Color.FromArgb((int)0xFFFF9900), Color.FromArgb((int)0xFFFF9933), Color.FromArgb((int)0xFFFF9966), Color.FromArgb((int)0xFFFF9999),	Color.FromArgb((int)0xFFFF99CC), Color.FromArgb((int)0xFFFF99FF),
					Color.FromArgb((int)0xFFFFCC00), Color.FromArgb((int)0xFFFFCC33), Color.FromArgb((int)0xFFFFCC66), Color.FromArgb((int)0xFFFFCC99),	Color.FromArgb((int)0xFFFFCCCC), Color.FromArgb((int)0xFFFFCCFF),
					Color.FromArgb((int)0xFFFFFF00), Color.FromArgb((int)0xFFFFFF33), Color.FromArgb((int)0xFFFFFF66), Color.FromArgb((int)0xFFFFFF99),	Color.FromArgb((int)0xFFFFFFCC), Color.FromArgb((int)0xFFFFFFFF)
				};
			}
		}
		#endregion
		#endregion Color Data
	}
	[ToolboxItem(false)]
	public class ColorListBox : ListBoxControl {
		public ColorListBox() {
			SetStyle(ControlStyles.UserMouse, false);
		}
		public event EnterColorEventHandler EnterColor;
		protected override BaseControlPainter CreatePainter() { return new PainterColorListBox(); }
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new ColorListBoxViewInfo(this); }
		public virtual void ProcessKeyDown(KeyEventArgs e) { OnKeyDown(e); }
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode == Keys.Return) {
				e.Handled = true;
				RaiseEnterColor((Color)SelectedItem);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			bool wasMouseDown = Capture;
			base.OnMouseUp(e);
			if((e.Button & MouseButtons.Left) != 0 && wasMouseDown) {
				DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo selectedItem = ViewInfo.GetItemByIndex(ViewInfo.FocusedItemIndex);
				if(selectedItem != null && selectedItem.Bounds.Contains(new Point(e.X, e.Y)))
					RaiseEnterColor((Color)SelectedItem);
			}
		}
		int suspendCount = 0;
		public virtual void SetSelectedIndex(int index) {
			this.suspendCount++;
			try {
				SelectedIndex = index;
			}
			finally {
				this.suspendCount--;
			}
		}
		protected internal override void RaiseSelectedIndexChanged() {
			if(this.suspendCount != 0) return;
			base.RaiseSelectedIndexChanged();
			object res = SelectedValue;
			if(res == null) return;
		}
		protected virtual void RaiseEnterColor(Color clr) {
			if(EnterColor != null) EnterColor(this, new EnterColorEventArgs(clr));
		}
		protected override bool CanFocusListBox { get { return false; } }
		protected internal new ColorListBoxViewInfo ViewInfo { get { return base.ViewInfo as ColorListBoxViewInfo; } }
		protected internal virtual string GetColorName(Color color) {
			return color.Name;
		}
	}
	[ToolboxItem(false)]
	public class ColorCellsControl : BaseControl {
		ColorCellsControlViewInfo viewInfo;
		ColorCellsControlPainter painter;
		RepositoryItemColorEdit properties;
		bool lockFocus;
		PopupBaseForm shadowForm;
		public event EnterColorEventHandler EnterColor;
		public ColorCellsControl(PopupBaseForm shadowForm) : this(shadowForm, 8, 8) {
		}
		public ColorCellsControl(PopupBaseForm shadowForm, int columnCount, int rowCount) {
			if(columnCount * rowCount > 64)
				throw new Exception("Incorrect cells count.");
			SetStyle(ControlStyles.UserMouse, false);
			this.shadowForm = shadowForm;
			viewInfo = new ColorCellsControlViewInfo(this, columnCount, rowCount);
			painter = new ColorCellsControlPainter(columnCount * rowCount);
			properties = null;
			lockFocus = false;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
		}
		public Size GetBestSize() {
			return viewInfo.GetBestSize();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			e.Handled = true;
			int horzStep = 0, vertStep = 0;
			switch(e.KeyCode) {
				case Keys.Left: horzStep = -1; break;
				case Keys.Right: horzStep = 1; break;
				case Keys.Up: vertStep = -1; break;
				case Keys.Down: vertStep = 1; break;
				case Keys.Return:
				case Keys.Space: OnEnterColor(SelectedCellIndex); return;
				default: e.Handled = false; break;
			}
			if(horzStep != 0 || vertStep != 0) MoveSelectedCell(horzStep, vertStep);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if((e.Button & MouseButtons.Left) != 0) {
				int cellIndex = viewInfo.GetCellIndex(new Point(e.X, e.Y));
				if(cellIndex != -1) {
					Capture = true;
					SelectedCellIndex = cellIndex;
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if((e.Button & MouseButtons.Left) != 0 && Capture) {
				int cellIndex = viewInfo.GetCellIndex(new Point(e.X, e.Y));
				if(cellIndex != -1) SelectedCellIndex = cellIndex;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			int cellIndex = viewInfo.GetCellIndex(new Point(e.X, e.Y));
			if((e.Button & MouseButtons.Left) != 0 && Capture) {
				if(cellIndex != -1) {
					SelectedCellIndex = cellIndex;
					Capture = false;
					OnEnterColor(SelectedCellIndex);
				}
			}
			if((e.Button & MouseButtons.Right) != 0 && cellIndex >= ColorCellsControlViewInfo.CellColors.Length && Properties.ShowColorDialog) {
				ColorDialog cd = new ColorDialog();
				cd.FullOpen = true;
				cd.Color = viewInfo.GetCellColor(cellIndex);
				cd.CustomColors = GetCustomColorsForColorDialog();
				lockFocus = true;
				if(shadowForm != null)
					this.shadowForm.HideShadows();
				try {
					if(cd.ShowDialog() == DialogResult.OK) {
						AddCustomColors(cd, cellIndex - ColorCellsControlViewInfo.CellColors.Length);
					}
				}
				finally {
					lockFocus = false;
					if(shadowForm != null)
						this.shadowForm.ShowHideShadows();
				}
			}
		}
		private int[] GetCustomColorsForColorDialog() {
			int[] res = new int[16];
			for(int i = 0; i < 16; i++) {
				if(this.viewInfo.CustomColors[i].IsEmpty)
					res[i] = 0x00ffffff;
				else 
					res[i] = (((int)this.viewInfo.CustomColors[i].B) << 16) |
							(((int)this.viewInfo.CustomColors[i].G) << 8) |
							(((int)this.viewInfo.CustomColors[i].R));
			}
			return res;
		}
		protected virtual void AddCustomColors(ColorDialog cd, int clickedCell) {
			int cellIndex = 0;
			int selectedCellIndex = -1;
			foreach(int color in cd.CustomColors) {
				if(color == 0x00ffffff)
					this.viewInfo.CustomColors[cellIndex] = Color.Empty;
				else 
					this.viewInfo.CustomColors[cellIndex] = Color.FromArgb(255, (color & 0xff), ((color >> 8) & 0xff), ((color >> 16) & 0xff));
				if(cd.Color.R == this.viewInfo.CustomColors[cellIndex].R && 
					cd.Color.G == this.viewInfo.CustomColors[cellIndex].G &&
					cd.Color.B == this.viewInfo.CustomColors[cellIndex].B)
					selectedCellIndex = cellIndex;
				cellIndex++;
			}
			if(selectedCellIndex == -1) {
				selectedCellIndex = clickedCell;
				this.viewInfo.CustomColors[selectedCellIndex] = Color.FromArgb(255, cd.Color);
			}
			OnEnterColor(selectedCellIndex + ColorCellsControlViewInfo.CellColors.Length);
		}
		private int FindEmptyCell(int cellIndex) {
			for(int i = cellIndex + 1; i < viewInfo.CustomColors.Length; i++) {
				if(viewInfo.CustomColors[i].IsEmpty || viewInfo.CustomColors[i] == Color.Transparent)
					return i;
			}
			return -1;
		}
		private void MoveSelectedCell(int horzStep, int vertStep) {
			int vertIndex = SelectedCellIndex / 8;
			int horzIndex = SelectedCellIndex - vertIndex * 8;
			if(horzIndex + horzStep < 0) horzStep = 7;
			if(horzIndex + horzStep > 7) horzStep = -7;
			if(vertIndex + vertStep < 0) vertStep = 7;
			if(vertIndex + vertStep > 7) vertStep = -7;
			SelectedCellIndex = 8 * (vertIndex + vertStep) + (horzIndex + horzStep);
		}
		private void OnEnterColor(int index) {
			Color clr = viewInfo.GetCellColor(index);
			if(clr == Color.Empty) return;
			EnterColorEventArgs e = new EnterColorEventArgs(clr);
			e = new EnterColorEventArgs(clr);
			if(EnterColor != null) EnterColor(this, e);
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) { OnKeyDown(e); }
		protected internal override BaseControlPainter Painter { get { return painter; } }
		protected internal override BaseControlViewInfo ViewInfo { get { return viewInfo; } }
		[DXCategory(CategoryName.Properties)]
		public RepositoryItemColorEdit Properties { get { return properties; } set { properties = value; } }
		[Browsable(false)]
		public bool LockFocus { get { return lockFocus; } }
		[DXCategory(CategoryName.Appearance)]
		public virtual int SelectedCellIndex {
			get { return viewInfo.SelectedCellIndex; }
			set {
				if(value < 0) value = 0;
				if(value > 63) value = 63;
				if(SelectedCellIndex != value) {
					viewInfo.SelectedCellIndex = value;
					LayoutChanged();
				}
			}
		}
	}
	public class ColorCellsControlViewInfo : BaseControlViewInfo {
		[Obsolete("Use CellsInterval"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const int CellsInternval = 3;
		public const int CellsInterval = 3;
		public Color[] fCustomColors;
		int selectedCellIndex;
		int columnCount;
		int rowCount;
		#region cellColors
		static Color[] cellColors = new Color[] {Color.FromArgb(255, 255, 255),
																		 Color.FromArgb(255, 192, 192),
																		 Color.FromArgb(255, 224, 192),
																		 Color.FromArgb(255, 255, 192),
																		 Color.FromArgb(192, 255, 192),
																		 Color.FromArgb(192, 255, 255),
																		 Color.FromArgb(192, 192, 255),
																		 Color.FromArgb(255, 192, 255),
																		 Color.FromArgb(224, 224, 224),
																		 Color.FromArgb(255, 128, 128),
																		 Color.FromArgb(255, 192, 128),
																		 Color.FromArgb(255, 255, 128),
																		 Color.FromArgb(128, 255, 128),
																		 Color.FromArgb(128, 255, 255),
																		 Color.FromArgb(128, 128, 255),
																		 Color.FromArgb(255, 128, 255),
																		 Color.FromArgb(192, 192, 192),
																		 Color.FromArgb(255, 0, 0),
																		 Color.FromArgb(255, 128, 0),
																		 Color.FromArgb(255, 255, 0),
																		 Color.FromArgb(0, 255, 0),
																		 Color.FromArgb(0, 255, 255),
																		 Color.FromArgb(0, 0, 255),
																		 Color.FromArgb(255, 0, 255),
																		 Color.FromArgb(128, 128, 128),
																		 Color.FromArgb(192, 0, 0),
																		 Color.FromArgb(192, 64, 0),
																		 Color.FromArgb(192, 192, 0),
																		 Color.FromArgb(0, 192, 0),
																		 Color.FromArgb(0, 192, 192),
																		 Color.FromArgb(0, 0, 192),
																		 Color.FromArgb(192, 0, 192),
																		 Color.FromArgb(64, 64, 64),
																		 Color.FromArgb(128, 0, 0),
																		 Color.FromArgb(128, 64, 0),
																		 Color.FromArgb(128, 128, 0),
																		 Color.FromArgb(0, 128, 0),
																		 Color.FromArgb(0, 128, 128),
																		 Color.FromArgb(0, 0, 128),
																		 Color.FromArgb(128, 0, 128),
																		 Color.FromArgb(0, 0, 0),
																		 Color.FromArgb(64, 0, 0),
																		 Color.FromArgb(128, 64, 64),
																		 Color.FromArgb(64, 64, 0),
																		 Color.FromArgb(0, 64, 0),
																		 Color.FromArgb(0, 64, 64),
																		 Color.FromArgb(0, 0, 64),
																		 Color.FromArgb(64, 0, 64)};
		#endregion cellColors
		public ColorCellsControlViewInfo(ColorCellsControl owner, int columnCount, int rowCount)
			: base(owner) {
			this.fCustomColors = null;
			this.selectedCellIndex = 0;
			this.columnCount = columnCount;
			this.rowCount = rowCount;
		}
		protected override void UpdateFromOwner() {
			Appearance = OwnerControl.Properties.Appearance;
			UpdatePaintAppearance();
		}
		public int GetCellIndex(Point pt) {
			int x = (pt.X - CellsInterval) / (CellSize.Width + CellsInterval);
			int y = (pt.Y - CellsInterval) / (CellSize.Height + CellsInterval);
			if(x < 0 || x > columnCount - 1 || y < 0 || y > rowCount - 1) return -1;
			Rectangle r = GetCellRect(columnCount * y + x);
			if(r.Contains(pt)) return columnCount * y + x;
			return -1;
		}
		public Rectangle GetCellRect(int index) {
			int vertIndex = index / columnCount;
			int horzIndex = index - vertIndex * columnCount;
			return new Rectangle(Bounds.Left + ColorCellsControlViewInfo.CellsInterval + horzIndex * (ColorCellsControlViewInfo.CellsInterval + ColorCellsControlViewInfo.CellSize.Width), Bounds.Top + ColorCellsControlViewInfo.CellsInterval + vertIndex * (ColorCellsControlViewInfo.CellsInterval + ColorCellsControlViewInfo.CellSize.Height),
				ColorCellsControlViewInfo.CellSize.Width, ColorCellsControlViewInfo.CellSize.Height);
		}
		public Color GetCellColor(int cellIndex) {
			if(cellIndex < CellColors.Length) return CellColors[cellIndex];
			return GetCustomColor(cellIndex - CellColors.Length);
		}
		protected virtual Color GetCustomColor(int index) {
			if(CustomColors == null) return Color.Empty;
			return CustomColors[index];
		}
		internal Size GetBestSize() {
			return new Size(columnCount * CellSize.Width + (columnCount + 1) * CellsInterval, rowCount * CellSize.Height + (rowCount + 1) * CellsInterval);
		}
		protected new ColorCellsControl OwnerControl { get { return base.OwnerControl as ColorCellsControl; } }
		public BorderPainter CellBorderPainter { get { return OwnerControl.LookAndFeel.Painter.Border; } }
		public static Size CellSize { get { return new Size(19, 19); } }
		public static Size BestSize { get { return new Size(8 * CellSize.Width + 9 * CellsInterval, 8 * CellSize.Height + 9 * CellsInterval); } }
		public int SelectedCellIndex { get { return selectedCellIndex; } set { selectedCellIndex = value; } }
		public Color[] CustomColors { get { return OwnerControl.Properties.CustomColors; } }
		public static Color[] CellColors { get { return cellColors; } }
	}
	public class ColorCellsControlPainter : BaseControlPainter {
		int cellsCount;
		public ColorCellsControlPainter(int cellsCount) {
			this.cellsCount = cellsCount;
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			ColorCellsControlViewInfo vi = info.ViewInfo as ColorCellsControlViewInfo;
			vi.PaintAppearance.DrawBackground(info.Cache, vi.ClientRect);
			for(int i = 0; i < cellsCount; i++) {
				Rectangle cellBounds = vi.GetCellRect(i);
				info.Graphics.FillRectangle(new SolidBrush(vi.GetCellColor(i)), cellBounds);
				vi.CellBorderPainter.DrawObject(new BorderObjectInfoArgs(info.Cache, cellBounds, null, ObjectState.Pressed));
				if(i == vi.SelectedCellIndex) {
					info.Paint.DrawFocusRectangle(info.Graphics, Rectangle.Inflate(cellBounds, 2, 2),
							info.ViewInfo.PaintAppearance.ForeColor, Color.Empty);
				}
			}
		}
	}
	[ToolboxItem(false)]
	public class ColorEditTabControlBase : XtraTabControl {
		public ColorEditTabControlBase() {
			TabStop = false;
			AppearancePage.Header.TextOptions.HAlignment = HorzAlignment.Center;
			CreatePages();
		}
		protected virtual void CreatePages() {
			TabPages.AddRange(new XtraTabPage[] { new XtraTabPage(), new XtraTabPage(), new XtraTabPage() });
		}
		protected internal virtual void ProcessTabKey(KeyEventArgs tabArgs) {
			OnKeyDown(new KeyEventArgs(tabArgs.KeyData | Keys.Control));
		}
		protected internal virtual void SelectPageByCaption(string caption) {
			for (int i = 0; i < TabPages.Count; i++) {
				if (TabPages[i].Text == caption) {
					SelectedTabPage = TabPages[i];
					return;
				}
			}
		}
		protected internal virtual void InitShowTabHeader() {
			if (ViewInfo.VisiblePagesCount > 1)
				ShowTabHeader = DefaultBoolean.True;
			else
				ShowTabHeader = DefaultBoolean.False;
		}
		protected internal override bool AllowTabFocus { get { return false; } }
	}
	public class PopupColorBuilder {
		ColorEditTabControlBase tabControl;
		object resultColor;
		IPopupColorEdit owner;
		BaseControl control = null;
		public PopupColorBuilder(IPopupColorEdit owner) {
			this.owner = owner;
			this.resultColor = Color.Empty;
			this.tabControl = CreateTabControl();
			for (int i = 0; i < TabControl.TabPages.Count; i++)
				SetTabPageProperties(i, owner as PopupBaseForm);
			this.tabControl.InitShowTabHeader();
		}
		protected virtual IPopupColorEdit Owner { get { return owner; } }
		public ColorEditTabControlBase TabControl { get { return tabControl; } }
		public virtual Control EmbeddedControl { get { return TabControl; } }
		public ColorCellsControl CellsControl { get { return ((ColorCellsControl)TabControl.TabPages[0].Controls[0]); } }
		public ColorCellsControlViewInfo CellsViewInfo { get { return (((ColorCellsControl)TabControl.TabPages[0].Controls[0]).ViewInfo as ColorCellsControlViewInfo); } }
		public virtual object ResultValue { get { return resultColor; } }
		public RepositoryItemColorEdit Properties { get { return Owner.Properties; } }
		public virtual bool LockFocus { get { return CellsControl.LockFocus; } }
		protected virtual void SetTabPageProperties(int pageIndex, PopupBaseForm shadowForm) {
			XtraTabPage tabPage = this.TabControl.TabPages[pageIndex];
			ColorListBox colorBox = null;			
			switch (pageIndex) {
				case 0:
					tabPage.Text = GetLocalizedStringCore(StringId.ColorTabCustom);
					control = new ColorCellsControl(shadowForm);
					(control as ColorCellsControl).Properties = Owner.Properties;
					(control as ColorCellsControl).EnterColor += new EnterColorEventHandler(OnEnterColor);
					control.Size = (control as ColorCellsControl).GetBestSize();
					tabPage.PageVisible = Properties.ShowCustomColors;
					break;
				case 1:
					tabPage.Text = GetLocalizedStringCore(StringId.ColorTabWeb);
					colorBox = CreateColorListBox();
					colorBox.HighlightedItemStyle = Properties.HighlightedItemStyle;
					colorBox.Items.AddRange(ColorListBoxViewInfo.WebColors);
					colorBox.EnterColor += new EnterColorEventHandler(OnEnterColor);
					colorBox.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
					colorBox.Size = GetBestListBoxSize(colorBox);
					control = colorBox;
					tabPage.PageVisible = Properties.ShowWebColors;
					break;
				case 2:
					tabPage.Text = GetLocalizedStringCore(StringId.ColorTabSystem);
					colorBox = CreateColorListBox();
					colorBox.HighlightedItemStyle = Properties.HighlightedItemStyle;
					colorBox.Items.AddRange(ColorListBoxViewInfo.SystemColors);
					colorBox.EnterColor += new EnterColorEventHandler(OnEnterColor);
					colorBox.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
					colorBox.Size = GetBestListBoxSize(colorBox);
					control = colorBox;
					tabPage.PageVisible = Properties.ShowSystemColors;
					break;
			}
			control.Dock = DockStyle.Fill;
			control.BorderStyle = BorderStyles.NoBorder;
			if(Owner.LookAndFeel != null)
				control.LookAndFeel.Assign(Owner.LookAndFeel);
			tabPage.Controls.Add(control);
		}
		protected virtual string GetLocalizedStringCore(StringId id) {
			return Localizer.Active.GetLocalizedString(id);
		}
		protected virtual ColorEditTabControlBase CreateTabControl() {
			return Owner.CreateTabControl();
		}
		protected virtual ColorListBox CreateColorListBox() {
			return Owner.CreateColorListBox();
		}
		protected virtual void OnSelectedIndexChanged(object sender, EventArgs e) {
			ColorListBox lb = sender as ColorListBox;
			object clr = lb == null ? null : lb.SelectedItem;
			if (clr != null) {
				resultColor = (Color)clr;
				OnColorChanged();
			}
		}
		protected virtual void OnEnterColor(object sender, EnterColorEventArgs e) {
			if (!Owner.IsPopupOpen)
				return;
			resultColor = e.Color;
			Owner.ClosePopup();
			OnColorChanged();
		}
		protected virtual bool FindEditColor(Color clr) {
			if (Properties.ShowSystemColors) {
				for (int i = 0; i < ColorListBoxViewInfo.SystemColors.Length; i++) {
					if (ColorListBoxViewInfo.SystemColors[i].Equals(clr)) {
						this.tabControl.SelectPageByCaption(Localizer.Active.GetLocalizedString(StringId.ColorTabSystem));
						(TabControl.SelectedTabPage.Controls[0] as ColorListBox).SetSelectedIndex(i);
						return true;
					}
				}
			}
			if (Properties.ShowWebColors) {
				for (int i = 0; i < ColorListBoxViewInfo.WebColors.Length; i++) {
					if (ColorListBoxViewInfo.WebColors[i].Equals(clr) || 
						(Properties.StoreColorAsInteger && ((Color)ColorListBoxViewInfo.WebColors[i]).ToArgb().Equals(clr.ToArgb()))) {
						this.tabControl.SelectPageByCaption(Localizer.Active.GetLocalizedString(StringId.ColorTabWeb));
						(TabControl.SelectedTabPage.Controls[0] as ColorListBox).SetSelectedIndex(i);
						return true;
					}
				}
			}
			if (Properties.ShowCustomColors) {
				this.tabControl.SelectPageByCaption(Localizer.Active.GetLocalizedString(StringId.ColorTabCustom));
				ColorCellsControl ctl = TabControl.SelectedTabPage.Controls[0] as ColorCellsControl;
				for (int i = 0; i < ColorCellsControlViewInfo.CellColors.Length; i++) {
					if (ColorCellsControlViewInfo.CellColors[i] == clr) {
						ctl.SelectedCellIndex = i;
						return true;
					}
				}
				ColorCellsControlViewInfo vi = ctl.ViewInfo as ColorCellsControlViewInfo;
				for (int i = 0; i < vi.CustomColors.Length; i++) {
					if (vi.CustomColors[i] == clr) {
						ctl.SelectedCellIndex = i + ColorCellsControlViewInfo.CellColors.Length;
						return true;
					}
				}
				ctl.SelectedCellIndex = ColorCellsControlViewInfo.CellColors.Length;
			}
			TabControl.SelectedTabPageIndex = 0;
			return false;
		}
		protected virtual Size GetBestListBoxSize(ColorListBox colorBox) {
			Size size = CellsControl.GetBestSize();
			return new Size(size.Width, colorBox.ViewInfo.GetNearestBestClientHeight(size.Height));
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			if (e.Handled)
				return;
			if (e.KeyCode == Keys.Tab) {
				tabControl.ProcessTabKey(e);
				e.Handled = true;
				return;
			}
			MarshalKeyToPopupCore(e);
		}
		protected virtual void MarshalKeyToPopupCore(KeyEventArgs e) {
			if(TabControl.SelectedTabPage.Text == Localizer.Active.GetLocalizedString(StringId.ColorTabCustom))
				((ColorCellsControl)TabControl.SelectedTabPage.Controls[0]).ProcessKeyDown(e);
			else ((ColorListBox)TabControl.SelectedTabPage.Controls[0]).ProcessKeyDown(e);
		}
		public virtual void RefreshLookAndFeel() {
			if(Owner.LookAndFeel != null)
				control.LookAndFeel.Assign(Owner.LookAndFeel);
		}
		public virtual void OnShowPopup() {
			FindEditColor(Owner.Color);
			resultColor = Owner.EditValue;
		}
		public virtual Size CalcContentSize() {
			Size size = CalcBestSizeCore();
			return TabControl.CalcSizeByPageClient(size);
		}
		protected virtual Size CalcBestSizeCore() {
			Size size = CellsControl.GetBestSize();
			size.Height = (TabControl.TabPages[1].Controls[0] as ColorListBox).ViewInfo.GetNearestBestClientHeight(size.Height);
			return size;
		}
		protected virtual void OnColorChanged() {
			Owner.OnColorChanged();
		}
	}
	public interface IPopupColorEdit {
		RepositoryItemColorEdit Properties { get; }
		UserLookAndFeel LookAndFeel { get; }
		bool IsPopupOpen { get; }
		void ClosePopup();
		Color Color { get; }
		object EditValue { get; }
		void OnColorChanged();
		ColorEditTabControlBase CreateTabControl();
		ColorListBox CreateColorListBox();
	}
	public interface IPopupColorPickEdit : IPopupColorEdit {
		bool HasBorder { get; }
		void ClosePopup(PopupCloseMode mode);
		ColorPickEditBase OwnerEdit { get; }
		void SetSelectedColorItem(ColorItem colorItem);
	}
	public class PopupColorEditForm : PopupBaseForm, IPopupColorEdit {
		public class ColorEditTabControl : ColorEditTabControlBase {
		}
		PopupColorBuilder popupColorEditBuilder;
		public PopupColorEditForm(ColorEdit ownerEdit)
			: base(ownerEdit) {
			this.popupColorEditBuilder = CreatePopupColorEditBuilder();
			Initialize();
		}
		UserLookAndFeel IPopupColorEdit.LookAndFeel { get { return LookAndFeel; } }
		protected override Size MinFormSize { get { return Size.Empty; } }
		protected virtual void Initialize() { this.Controls.Add(PopupColorBuilder.EmbeddedControl); }
		protected virtual ColorEditTabControl CreateTabControl() { return new ColorEditTabControl(); }
		protected virtual ColorListBox CreateColorListBox() { return new ColorListBox(); }
		protected virtual PopupColorBuilder CreatePopupColorEditBuilder() { return new PopupColorBuilder(this); }
		public override void ShowPopupForm() {
			PopupColorBuilder.OnShowPopup();			
			base.ShowPopupForm();
		}
		public override void HidePopupForm() {
			base.HidePopupForm();
		}
		protected override Size CalcFormSizeCore() {
			return CalcFormSize(PopupColorBuilder.CalcContentSize());
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			if(base.AllowMouseClick(control, mousePosition)) return true;
			return CellsControl.LockFocus;
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			base.ProcessKeyDown(e);
			PopupColorBuilder.ProcessKeyDown(e);
		}
		protected virtual void OnRaisePopupAllowClick(PopupAllowClickEventArgs e) {
			OwnerEdit.RaisePopupAllowClick(e);
		}
		internal Control SelectedPageControl {
			get {
				if(TabControl != null && TabControl.SelectedTabPage != null)
					return TabControl.SelectedTabPage.Controls[0];
				return null;
			}
		}
		[DXCategory(CategoryName.Focus)]
		public override bool FormContainsFocus { get { return base.FormContainsFocus || CellsControl.LockFocus; } }
		protected override Control EmbeddedControl { get { return PopupColorBuilder.EmbeddedControl; } }
		[Browsable(false)]
		public XtraTabControl TabControl { get { return PopupColorBuilder.TabControl; } }
		[DXCategory(CategoryName.Properties)]
		public new RepositoryItemColorEdit Properties { get { return base.Properties as RepositoryItemColorEdit; } }
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue { get { return PopupColorBuilder.ResultValue; } }
		[Browsable(false)]
		public new ColorEdit OwnerEdit { get { return base.OwnerEdit as ColorEdit; } }
		private ColorCellsControl CellsControl { get { return PopupColorBuilder.CellsControl; } }
		protected PopupColorBuilder PopupColorBuilder { get { return popupColorEditBuilder; } }
		#region IPopupColorEdit Members
		bool IPopupColorEdit.IsPopupOpen { get { return OwnerEdit.IsPopupOpen; } }
		void IPopupColorEdit.ClosePopup() {
			OwnerEdit.ClosePopup();
		}
		Color IPopupColorEdit.Color {
			get { return OwnerEdit.Color; }
		}
		object IPopupColorEdit.EditValue {
			get { return OwnerEdit.EditValue; }
		}
		void IPopupColorEdit.OnColorChanged() {
		}
		ColorEditTabControlBase IPopupColorEdit.CreateTabControl() { return this.CreateTabControl(); }
		ColorListBox IPopupColorEdit.CreateColorListBox() { return this.CreateColorListBox(); }
		#endregion
	}
}
