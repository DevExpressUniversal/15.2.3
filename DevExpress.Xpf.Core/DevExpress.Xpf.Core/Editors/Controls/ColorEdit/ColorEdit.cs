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
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Utils;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Internal;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Data;
using DevExpress.Utils;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Internal;
#endif
namespace DevExpress.Xpf.Editors {
	public enum ColorDisplayFormat { Default, Hex, ARGB }
	public enum ChipSize { Default, Small, Medium, Large }
	public interface IColorEdit {
		Color Color { get; set; }
		Color DefaultColor { get; set; }
		object EditValue { get; set; }
		void AddCustomColor(Color color);
		CircularList<Color> RecentColors { get; }
		event RoutedEventHandler ColorChanged;
		PaletteCollection Palettes { get; set; }
	}
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class PopupColorEdit : PopupBaseEdit, IColorEdit {
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty DefaultColorProperty;
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty ShowMoreColorsButtonProperty;
		public static readonly DependencyProperty ShowNoColorButtonProperty;
		public static readonly DependencyProperty ShowDefaultColorButtonProperty;
		public static readonly DependencyProperty DefaultColorButtonContentProperty;
		public static readonly DependencyProperty MoreColorsButtonContentProperty;
		public static readonly DependencyProperty NoColorButtonContentProperty;
		public static readonly DependencyProperty ChipSizeProperty;
		public static readonly DependencyProperty ChipMarginProperty;
		public static readonly DependencyProperty ChipBorderBrushProperty;
		public static readonly DependencyProperty PalettesProperty;
		public static readonly DependencyProperty ColorDisplayFormatProperty;
		public static readonly DependencyProperty OwnerPopupEditProperty;
		static readonly DependencyPropertyKey OwnerPopupEditPropertyKey;
		public static readonly RoutedEvent ColorChangedEvent;
		public static readonly RoutedEvent GetColorNameEvent;
		static PopupColorEdit() {
			Type ownerType = typeof(PopupColorEdit);
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), ownerType, new PropertyMetadata(new Color(), OnColorChanged, CoerceColor));
			ColumnCountProperty = DependencyPropertyManager.Register("ColumnCount", typeof(int), ownerType, new PropertyMetadata(10, OnColumnCountChanged));
			DefaultColorProperty = ColorEdit.DefaultColorProperty.AddOwner(ownerType);
			ShowMoreColorsButtonProperty = ColorEdit.ShowMoreColorsButtonProperty.AddOwner(ownerType);
			ShowNoColorButtonProperty = ColorEdit.ShowNoColorButtonProperty.AddOwner(ownerType);
			ShowDefaultColorButtonProperty = ColorEdit.ShowDefaultColorButtonProperty.AddOwner(ownerType);
			DefaultColorButtonContentProperty = ColorEdit.DefaultColorButtonContentProperty.AddOwner(ownerType);
			MoreColorsButtonContentProperty = ColorEdit.MoreColorsButtonContentProperty.AddOwner(ownerType);
			NoColorButtonContentProperty = ColorEdit.NoColorButtonContentProperty.AddOwner(ownerType);
			ChipSizeProperty = ColorEdit.ChipSizeProperty.AddOwner(ownerType);
			ChipMarginProperty = ColorEdit.ChipMarginProperty.AddOwner(ownerType);
			ChipBorderBrushProperty = ColorEdit.ChipBorderBrushProperty.AddOwner(ownerType);
			PalettesProperty = ColorEdit.PalettesProperty.AddOwner(ownerType);
			ColorDisplayFormatProperty = DependencyPropertyManager.Register("ColorDisplayFormat", typeof(ColorDisplayFormat), ownerType, new PropertyMetadata(ColorDisplayFormat.Default, OnColorDisplayFormatChanged));
			OwnerPopupEditPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("OwnerPopupEdit", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			OwnerPopupEditProperty = OwnerPopupEditPropertyKey.DependencyProperty;
			ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChangedEvent", RoutingStrategy.Direct, typeof(RoutedEventArgs), ownerType);
			GetColorNameEvent = EventManager.RegisterRoutedEvent("GetColorName", RoutingStrategy.Direct, typeof(GetColorNameEventHandler), ownerType);
		}
		static object CoerceColor(DependencyObject d, object value) {
			return ((PopupColorEdit)d).CoerceColor((Color)value);
		}
		static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PopupColorEdit)d).OnColorChanged((Color)e.OldValue, (Color)e.NewValue);
		}
		static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PopupColorEdit)d).OnColumnCountChanged((int)e.NewValue);
		}
		static void OnColorDisplayFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PopupColorEdit)d).OnColorDisplayFormatChanged();
		}
		internal static void SetOwnerPopupEdit(DependencyObject element, PopupColorEdit value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(OwnerPopupEditPropertyKey, value);
		}
		public static PopupColorEdit GetOwnerPopupEdit(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PopupColorEdit)DependencyObjectHelper.GetValueWithInheritance(element, OwnerPopupEditProperty);
		}
		public event RoutedEventHandler ColorChanged {
			add { this.AddHandler(ColorChangedEvent, value); }
			remove { this.RemoveHandler(ColorChangedEvent, value); }
		}
		public event GetColorNameEventHandler GetColorName {
			add { this.AddHandler(GetColorNameEvent, value); }
			remove { this.RemoveHandler(GetColorNameEvent, value); }
		}
		ColorEdit colorEdit;
		public PopupColorEdit() {
			this.SetDefaultStyleKey(typeof(PopupColorEdit));
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			InitializePaletteCollection();
		}
		void InitializePaletteCollection() {
			if(Palettes == null)
				Palettes = PredefinedPaletteCollections.Office;
		}
		#region public properties
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditColor")]
#endif
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditDefaultColor")]
#endif
		public Color DefaultColor {
			get { return (Color)GetValue(DefaultColorProperty); }
			set { SetValue(DefaultColorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditRecentColors")]
#endif
		public CircularList<Color> RecentColors {
			get { return Settings.RecentColors; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditColumnCount")]
#endif
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditShowDefaultColorButton")]
#endif
		public bool ShowDefaultColorButton {
			get { return (bool)GetValue(ShowDefaultColorButtonProperty); }
			set { SetValue(ShowDefaultColorButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditShowMoreColorsButton")]
#endif
		public bool ShowMoreColorsButton {
			get { return (bool)GetValue(ShowMoreColorsButtonProperty); }
			set { SetValue(ShowMoreColorsButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditShowNoColorButton")]
#endif
		public bool ShowNoColorButton {
			get { return (bool)GetValue(ShowNoColorButtonProperty); }
			set { SetValue(ShowNoColorButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupColorEditDefaultColorButtonContent"),
#endif
TypeConverter(typeof(ObjectConverter))]
		public object DefaultColorButtonContent {
			get { return (object)GetValue(DefaultColorButtonContentProperty); }
			set { SetValue(DefaultColorButtonContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupColorEditMoreColorsButtonContent"),
#endif
TypeConverter(typeof(ObjectConverter))]
		public object MoreColorsButtonContent {
			get { return (object)GetValue(MoreColorsButtonContentProperty); }
			set { SetValue(MoreColorsButtonContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupColorEditNoColorButtonContent"),
#endif
TypeConverter(typeof(ObjectConverter))]
		public object NoColorButtonContent {
			get { return (object)GetValue(NoColorButtonContentProperty); }
			set { SetValue(NoColorButtonContentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditChipSize")]
#endif
		public ChipSize ChipSize {
			get { return (ChipSize)GetValue(ChipSizeProperty); }
			set { SetValue(ChipSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditChipMargin")]
#endif
		public Thickness ChipMargin {
			get { return (Thickness)GetValue(ChipMarginProperty); }
			set { SetValue(ChipMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditChipBorderBrush")]
#endif
		public Brush ChipBorderBrush {
			get { return (Brush)GetValue(ChipBorderBrushProperty); }
			set { SetValue(ChipBorderBrushProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditPalettes")]
#endif
		public PaletteCollection Palettes {
			get { return (PaletteCollection)GetValue(PalettesProperty); }
			set { SetValue(PalettesProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupColorEditColorDisplayFormat")]
#endif
		public ColorDisplayFormat ColorDisplayFormat {
			get { return (ColorDisplayFormat)GetValue(ColorDisplayFormatProperty); }
			set { SetValue(ColorDisplayFormatProperty, value); }
		}
		#endregion
		protected internal new PopupColorEditSettings Settings { get { return (PopupColorEditSettings)base.Settings; } }
		protected internal ColorEdit ColorEditControl {
			get { return colorEdit; }
			private set {
				if(ColorEditControl == value)
					return;
				colorEdit = value;
				EditStrategy.SyncProperties();
			}
		}
		protected new PopupColorEditStrategy EditStrategy {
			get { return base.EditStrategy as PopupColorEditStrategy; }
			set { base.EditStrategy = value; }
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new PopupColorEditStrategy(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new PopupColorEditPropertyProvider(this);
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			EditStrategy.AcceptPopupValue();
		}
		protected virtual object CoerceColor(Color value) {
			return EditStrategy.CoerceColor(value);
		}
		protected virtual void OnColorChanged(Color oldValue, Color newValue) {
			EditStrategy.OnColorChanged(oldValue, newValue);
		}
		protected internal virtual string GetColorNameCore(Color color) {
			string colorName = ColorEditHelper.GetColorName(color, ColorDisplayFormat);
			GetColorNameEventArgs e = new GetColorNameEventArgs(color, colorName) { RoutedEvent = PopupColorEdit.GetColorNameEvent };
			RaiseEvent(e);
			return e.ColorName;
		}
		protected override void OnIsReadOnlyChanged() {
			base.OnIsReadOnlyChanged();
			if (ColorEditControl != null)
				ColorEditControl.IsReadOnly = IsReadOnly;
		}
		protected internal virtual void RaiseColorChanged() {
			this.RaiseEvent(new RoutedEventArgs() { RoutedEvent = ColorChangedEvent });
		}
		protected internal override bool ShouldApplyPopupSize { get { return false; } }
		protected internal override BaseEditSettings CreateEditorSettings() {
			return new PopupColorEditSettings();
		}
		protected virtual void OnColumnCountChanged(int columnCount) {
			RecentColors.SetSize(columnCount);
		}
		protected void OnColorDisplayFormatChanged() {
			EditStrategy.UpdateDisplayText();
		}
		#region IColorChooserWindowOwner Members
		void IColorEdit.AddCustomColor(Color color) {
			EditStrategy.OnAddCustomColor(color);
		}
		#endregion
		internal void SetInnerColorEdit(ColorEdit colorEdit) {
			ColorEditControl = colorEdit;
		}
#if SL
		protected override bool GetIsTabStopCore() {
			return IsTabStop;
		}
#endif
	}
	public class PopupColorEditPropertyProvider : PopupBaseEditPropertyProvider {
		static PopupColorEditPropertyProvider() {
			Type ownerType = typeof(PopupColorEditPropertyProvider);
			IsTextEditableProperty.OverrideMetadata(ownerType, new PropertyMetadata(false));
		}
		public PopupColorEditPropertyProvider(PopupColorEdit editor) : base(editor) {
		}
		public override bool CalcSuppressFeatures() {
			return false;
		}
	}
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[TemplatePart(Name = ElementGalleryName, Type = typeof(Gallery))]
	[TemplatePart(Name = ElementResetButtonName, Type = typeof(BarButtonItem))]
	[TemplatePart(Name = ElementMoreColorsButtonName, Type = typeof(BarButtonItem))]
	[TemplatePart(Name = ElementNoColorButtonName, Type = typeof(BarButtonItem))]
	public class ColorEdit : BaseEdit, IColorEdit {
		const string ElementGalleryName = "PART_Gallery",
					 ElementResetButtonName = "PART_ResetButton",
					 ElementMoreColorsButtonName = "PART_MoreColorsButton",
					 ElementNoColorButtonName = "PART_NoColorButton";
		#region static
		public static readonly Color EmptyColor = new Color();
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty DefaultColorProperty;
		public static readonly DependencyProperty ShowMoreColorsButtonProperty;
		public static readonly DependencyProperty ShowNoColorButtonProperty;
		public static readonly DependencyProperty ShowDefaultColorButtonProperty;
		public static readonly DependencyProperty DefaultColorButtonContentProperty;
		public static readonly DependencyProperty MoreColorsButtonContentProperty;
		public static readonly DependencyProperty NoColorButtonContentProperty;
		public static readonly DependencyProperty RecentColorsCaptionProperty;
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty ChipSizeProperty;
		public static readonly DependencyProperty ChipMarginProperty;
		public static readonly DependencyProperty ChipBorderBrushProperty;
		public static readonly DependencyProperty PalettesProperty;
		public static readonly DependencyProperty ToolTipColorDisplayFormatProperty;
		public static readonly DependencyProperty CloseOwnerPopupOnClickProperty;
		public static readonly RoutedEvent ColorChangedEvent;
		public static readonly RoutedEvent GetColorNameEvent;
		static ColorEdit() {
			Type ownerType = typeof(ColorEdit);
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), ownerType, new PropertyMetadata(new Color(), OnColorChanged, CoerceColor));
			DefaultColorProperty = DependencyPropertyManager.Register("DefaultColor", typeof(Color), ownerType, new PropertyMetadata(Colors.Black, OnDefaultColorChanged));
			ShowMoreColorsButtonProperty = DependencyPropertyManager.Register("ShowMoreColorsButton", typeof(bool), ownerType, new PropertyMetadata(true, OnShowMoreColorsButtonChanged));
			ShowNoColorButtonProperty = DependencyPropertyManager.Register("ShowNoColorButton", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowDefaultColorButtonProperty = DependencyPropertyManager.Register("ShowDefaultColorButton", typeof(bool), ownerType, new PropertyMetadata(true));
			DefaultColorButtonContentProperty = DependencyPropertyManager.Register("DefaultColorButtonContent", typeof(object), ownerType, new PropertyMetadata(EditorLocalizer.GetString(EditorStringId.ColorEdit_AutomaticButtonCaption)));
			MoreColorsButtonContentProperty = DependencyPropertyManager.Register("MoreColorsButtonContent", typeof(object), ownerType, new PropertyMetadata(EditorLocalizer.GetString(EditorStringId.ColorEdit_MoreColorsButtonCaption)));
			NoColorButtonContentProperty = DependencyPropertyManager.Register("NoColorButtonContent", typeof(object), ownerType, new PropertyMetadata(EditorLocalizer.GetString(EditorStringId.ColorEdit_NoColorButtonCaption)));
			RecentColorsCaptionProperty = DependencyPropertyManager.Register("RecentColorsCaption", typeof(string), ownerType, new PropertyMetadata(EditorLocalizer.GetString(EditorStringId.ColorEdit_RecentColorsCaption), OnRecentColorsCaptionChanged));
			ColumnCountProperty = DependencyPropertyManager.Register("ColumnCount", typeof(int), ownerType, new PropertyMetadata(10, OnColumnCountChanged));
			ChipSizeProperty = DependencyPropertyManager.Register("ChipSize", typeof(ChipSize), ownerType, new PropertyMetadata(ChipSize.Default));
			ChipMarginProperty = DependencyPropertyManager.Register("ChipMargin", typeof(Thickness), ownerType, new PropertyMetadata(new Thickness(2, 0, 2, 0), OnChipMarginChanged));
			ChipBorderBrushProperty = DependencyPropertyManager.Register("ChipBorderBrush", typeof(Brush), ownerType, new PropertyMetadata(null, OnChipBorderBrushChanged));
			PalettesProperty = DependencyPropertyManager.Register("Palettes", typeof(PaletteCollection), ownerType, new PropertyMetadata(null, OnPalettesChanged));
			ToolTipColorDisplayFormatProperty = DependencyPropertyManager.Register("ToolTipColorDisplayFormat", typeof(ColorDisplayFormat), ownerType, new PropertyMetadata(ColorDisplayFormat.Default, OnToolTipColorDisplayFormatChanged));
			CloseOwnerPopupOnClickProperty = DependencyPropertyManager.Register("CloseOwnerPopupOnClick", typeof(bool), ownerType, new PropertyMetadata(false));
			ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChangedEvent", RoutingStrategy.Direct, typeof(RoutedEventArgs), ownerType);
			GetColorNameEvent = EventManager.RegisterRoutedEvent("GetColorName", RoutingStrategy.Direct, typeof(GetColorNameEventHandler), ownerType);
#if !SL
			EditValueProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null, true, UpdateSourceTrigger.PropertyChanged));
#endif
		}
		static void OnShowMoreColorsButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).UpdateActualShowRecentColors();
		}
		static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).OnColorChanged((Color)e.OldValue, (Color)e.NewValue);
		}
		static object CoerceColor(DependencyObject d, object value) {
			return ((ColorEdit)d).CoerceColor((Color)value);
		}
		static void OnDefaultColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).OnDefaultColorChanged((Color)e.NewValue);
		}
		static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).OnColumnCountChanged((int)e.NewValue);
		}
		static void OnChipBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).UpdateResetButtonGlyph();
			((ColorEdit)d).UpdateNoColorButtonGlyph();
		}
		static void OnPalettesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).AddGroups();
		}
		static void OnRecentColorsCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).OnRecentColorsCaptionChanged();
		}
		static void OnToolTipColorDisplayFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).OnToolTipColorDisplayFormatChanged();
		}
		static void OnChipMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColorEdit)d).OnChipMarginChanged();
		}
		#endregion
		Locker updateLocker = new Locker();
		public ColorEdit() {
			this.SetDefaultStyleKey(typeof(ColorEdit));
			RecentColors = new CircularList<Color>((int)ColumnCountProperty.GetMetadata(typeof(ColorEdit)).DefaultValue);
			RecentColors.CollectionChanged += OnRecentColorsCollectionChanged;
#if !SL
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
#endif
		}
#if !SL
		Popup ParentPopup { get; set; }
		void OnUnloaded(object sender, RoutedEventArgs e) {
			if(ParentPopup != null) {
				ParentPopup.Closed -= new EventHandler(OnParentPopupClosed);
			}
		}
		void OnParentPopupClosed(object sender, EventArgs e) {
			if(ColorChooserDialog != null) {
				ColorChooserDialog.Close();
				ColorChooserDialog = null;
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if (OwnerPopupEdit != null)
				OwnerPopupEdit.SetInnerColorEdit(this);
			PopupBorderControl popupBorderControl = LayoutHelper.FindParentObject<PopupBorderControl>(this);
			if(popupBorderControl != null && popupBorderControl.Popup != null) {
				ParentPopup = popupBorderControl.Popup;
				ParentPopup.Closed += new EventHandler(OnParentPopupClosed);
			}
		}
#endif
		protected internal FloatingContainer ColorChooserDialog { get; set; }
		void InitializePaletteCollection() {
			if(Palettes == null)
				Palettes = PredefinedPaletteCollections.Office;
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			InitializePaletteCollection();
		}
		protected internal Gallery Gallery { get; private set; }
		protected internal BarButtonItem ResetButton { get; private set; }
		protected internal BarButtonItem MoreColorsButton { get; private set; }
		protected internal BarButtonItem NoColorButton { get; private set; }
		protected PopupColorEdit OwnerPopupEdit { 
			get { 
				DependencyObject parent = LayoutHelper.FindLayoutOrVisualParentObject(this, (element) => BaseEdit.GetOwnerEdit(element) is PopupColorEdit);
				if (parent == null)
					return null;
				return BaseEdit.GetOwnerEdit(parent) as PopupColorEdit;
			} 
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditRecentColors")]
#endif
		public CircularList<Color> RecentColors { get; private set; }
		protected GalleryItemGroup RecentColorsItemGroup { get; private set; }
		#region public properties
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditColor")]
#endif
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditDefaultColor")]
#endif
		public Color DefaultColor {
			get { return (Color)GetValue(DefaultColorProperty); }
			set { SetValue(DefaultColorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditShowDefaultColorButton")]
#endif
		public bool ShowDefaultColorButton {
			get { return (bool)GetValue(ShowDefaultColorButtonProperty); }
			set { SetValue(ShowDefaultColorButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditShowMoreColorsButton")]
#endif
		public bool ShowMoreColorsButton {
			get { return (bool)GetValue(ShowMoreColorsButtonProperty); }
			set { SetValue(ShowMoreColorsButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditShowNoColorButton")]
#endif
		public bool ShowNoColorButton {
			get { return (bool)GetValue(ShowNoColorButtonProperty); }
			set { SetValue(ShowNoColorButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ColorEditDefaultColorButtonContent"),
#endif
 TypeConverter(typeof(ObjectConverter))]
		public object DefaultColorButtonContent {
			get { return (object)GetValue(DefaultColorButtonContentProperty); }
			set { SetValue(DefaultColorButtonContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ColorEditMoreColorsButtonContent"),
#endif
 TypeConverter(typeof(ObjectConverter))]
		public object MoreColorsButtonContent {
			get { return (object)GetValue(MoreColorsButtonContentProperty); }
			set { SetValue(MoreColorsButtonContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ColorEditNoColorButtonContent"),
#endif
 TypeConverter(typeof(ObjectConverter))]
		public object NoColorButtonContent {
			get { return (object)GetValue(NoColorButtonContentProperty); }
			set { SetValue(NoColorButtonContentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditRecentColorsCaption")]
#endif
		public string RecentColorsCaption {
			get { return (string)GetValue(RecentColorsCaptionProperty); }
			set { SetValue(RecentColorsCaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditColumnCount")]
#endif
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditChipSize")]
#endif
		public ChipSize ChipSize {
			get { return (ChipSize)GetValue(ChipSizeProperty); }
			set { SetValue(ChipSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditChipMargin")]
#endif
		public Thickness ChipMargin {
			get { return (Thickness)GetValue(ChipMarginProperty); }
			set { SetValue(ChipMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditChipBorderBrush")]
#endif
		public Brush ChipBorderBrush {
			get { return (Brush)GetValue(ChipBorderBrushProperty); }
			set { SetValue(ChipBorderBrushProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditPalettes")]
#endif
		public PaletteCollection Palettes {
			get { return (PaletteCollection)GetValue(PalettesProperty); }
			set { SetValue(PalettesProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditToolTipColorDisplayFormat")]
#endif
		public ColorDisplayFormat ToolTipColorDisplayFormat {
			get { return (ColorDisplayFormat)GetValue(ToolTipColorDisplayFormatProperty); }
			set { SetValue(ToolTipColorDisplayFormatProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ColorEditCloseOwnerPopupOnClick")]
#endif
		public bool CloseOwnerPopupOnClick {
			get { return (bool)GetValue(CloseOwnerPopupOnClickProperty); }
			set { SetValue(CloseOwnerPopupOnClickProperty, value); }
		}
		#endregion
		public event RoutedEventHandler ColorChanged {
			add { this.AddHandler(ColorChangedEvent, value); }
			remove { this.RemoveHandler(ColorChangedEvent, value); }
		}
		public event GetColorNameEventHandler GetColorName {
			add { this.AddHandler(GetColorNameEvent, value); }
			remove { this.RemoveHandler(GetColorNameEvent, value); }
		}
		protected new ColorEditStrategy EditStrategy { get { return base.EditStrategy as ColorEditStrategy; } }
		protected internal new ColorEditSettings Settings { get { return (ColorEditSettings)base.Settings; } }
		protected override void SubscribeEditEventsCore() {
			base.SubscribeEditEventsCore();
			ResetButton = EditCore.FindName(ElementResetButtonName) as BarButtonItem;
			MoreColorsButton = EditCore.FindName(ElementMoreColorsButtonName) as BarButtonItem;
			NoColorButton = EditCore.FindName(ElementNoColorButtonName) as BarButtonItem;
			Gallery = EditCore.FindName(ElementGalleryName) as Gallery;
			InitGallery();
			SubscribeElementsEvents();
			UpdateResetButtonGlyph();
			UpdateNoColorButtonGlyph();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(OwnerPopupEdit != null)
				OwnerPopupEdit.SetInnerColorEdit(this);
		}
		protected virtual void InitGallery() {
			if(Gallery == null)
				return;
			ColorEdit.SetOwnerEdit(Gallery, this);
			AddGroups();
		}
		void AddGroups() {
			if(Gallery == null || Palettes == null)
				return;
			Gallery.Groups.Clear();
			foreach(ColorPalette palette in Palettes)
				Gallery.Groups.Add(CreateGalleryItemGroup(palette.Name, palette.Colors, palette.CalcBorder && !IsTopBottomChipMarginSet()));
			UpdateActualShowRecentColors();
		}
		bool IsTopBottomChipMarginSet() {
			return ChipMargin.Top > 0 || ChipMargin.Bottom > 0;
		}
		protected virtual GalleryItemGroup CreateGalleryItemGroup(string caption, IList<Color> colors, bool calcBorder) {
			GalleryItemGroup colorGroup = new GalleryItemGroup();
			if(caption != null)
				colorGroup.Caption = caption;
			for(int i = 0; i < colors.Count; i++) {
				ColorGalleryItem item = CreateColorItem(colors[i]);
				colorGroup.Items.Add(item);
				if(calcBorder)
					UpdateItemBorderVisibility(item, i, colors.Count);
			}
			colorGroup.SetBinding(GalleryItemGroup.IsCaptionVisibleProperty, new Binding("Caption") { Source = colorGroup, Converter = new CaptionToVisibilityConverter() });
			return colorGroup;
		}
		protected virtual ColorGalleryItem CreateColorItem(Color color) {
			return new ColorGalleryItem() { Color = color, Hint = GetColorNameCore(color), HideBorderSide = HideBorderSide.None };
		}
		protected internal virtual string GetColorNameCore(Color color) {
			if(OwnerPopupEdit != null)
				return OwnerPopupEdit.GetColorNameCore(color);
			string colorName = ColorEditHelper.GetColorName(color, ToolTipColorDisplayFormat);
			GetColorNameEventArgs e = new GetColorNameEventArgs(color, colorName) { RoutedEvent = ColorEdit.GetColorNameEvent };
			RaiseEvent(e);
			return e.ColorName;
		} 
		protected virtual void UpdateItemBorderVisibility(ColorGalleryItem item, int index, int count) {
			int columnCount = ColumnCount;
			if(columnCount == 0) return;
			int elementIndex = index;
			if(elementIndex < columnCount)
				item.HideBorderSide = HideBorderSide.Bottom;
			else if(elementIndex > ((count - columnCount) - 1))
				item.HideBorderSide = HideBorderSide.Top;
			else
				item.HideBorderSide = HideBorderSide.Top | HideBorderSide.Bottom;
		}
		protected override void UnsubscribeEditEventsCore() {
			base.UnsubscribeEditEventsCore();
			UnsubscribeElementsEvents();
		}
		protected void OnRecentColorsCaptionChanged() {
			if(RecentColorsItemGroup != null)
				RecentColorsItemGroup.Caption = RecentColorsCaption;
		}
		protected virtual void SubscribeElementsEvents() {
			if(Gallery != null)
				Gallery.ItemClick += OnGalleryItemClick;
			if(ResetButton != null)
				ResetButton.ItemClick += OnResetButtonClick;
			if(MoreColorsButton != null)
				MoreColorsButton.ItemClick += OnMoreColorsButtonClick;
			if(NoColorButton != null)
				NoColorButton.ItemClick += OnNoColorButtonClick;
		}
		void OnGalleryItemClick(object sender, GalleryItemEventArgs e) {
			EditStrategy.OnGalleryColorChanged(((ColorGalleryItem)e.Item).Color);
			CloseOwnedPopup(true);
		}
		protected virtual void UnsubscribeElementsEvents() {
			if(NoColorButton != null)
				NoColorButton.ItemClick -= OnNoColorButtonClick;
			if(ResetButton != null)
				ResetButton.ItemClick -= OnResetButtonClick;
			if(MoreColorsButton != null)
				MoreColorsButton.ItemClick -= OnMoreColorsButtonClick;
			if(Gallery != null)
				Gallery.ItemClick -= OnGalleryItemClick;
		}
		void OnResetButtonClick(object sender, ItemClickEventArgs e) {
			EditStrategy.OnResetButtonClick();
			CloseOwnedPopup(true);
		}
		void OnMoreColorsButtonClick(object sender, ItemClickEventArgs e) {
			IColorEdit actualOwner = OwnerPopupEdit != null ? (IColorEdit)OwnerPopupEdit : this;
			CloseOwnedPopup(false);
			ColorChooserDialog = ColorEditHelper.ShowColorChooserDialog(actualOwner);
		}
		void OnNoColorButtonClick(object sender, ItemClickEventArgs e) {
			EditStrategy.OnNoColorButtonClick();
			CloseOwnedPopup(true);
		}
		protected internal virtual void RaiseColorChanged() {
			this.RaiseEvent(new RoutedEventArgs() { RoutedEvent = ColorChangedEvent });
		}
		protected virtual void OnColorChanged(Color oldValue, Color newValue) {
			EditStrategy.OnColorChanged(oldValue, newValue);
		}
		protected virtual object CoerceColor(Color color) {
			return EditStrategy.CoerceColor(color);
		}
		protected virtual void OnDefaultColorChanged(Color newValue) {
			UpdateResetButtonGlyph();
		}
		protected void UpdateResetButtonGlyph() {
			if(ResetButton == null) return;
			ResetButton.Glyph = ColorEditHelper.CreateGlyph(DefaultColor, ChipBorderBrush, new Size(16, 16));
		}
		protected void UpdateNoColorButtonGlyph() {
			if(NoColorButton == null) return;
			NoColorButton.Glyph = ColorEditHelper.CreateGlyph(EmptyColor, ChipBorderBrush, new Size(16, 16));
		}
		protected virtual void OnColumnCountChanged(int columnCount) {
			RecentColors.SetSize(columnCount);
			AddGroups();
		}
		protected virtual void OnChipMarginChanged() {
			AddGroups();
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new ColorEditStrategy(this);
		}
		protected internal override BaseEditSettings CreateEditorSettings() {
			return new ColorEditSettings();
		}
		protected virtual void OnRecentColorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateActualShowRecentColors();
		}
		protected virtual void OnToolTipColorDisplayFormatChanged() {
			EditStrategy.UpdateItemsState();
		}
		protected override bool FocusEditCore() {
			if(OwnerPopupEdit != null) return false;
			return base.FocusEditCore();
		}
		void UpdateActualShowRecentColors() {
			if(Gallery == null) return;
			bool actualShowRecentColors = ShowMoreColorsButton && RecentColors.Count > 0;
			if(RecentColorsItemGroup != null) {
				Gallery.Groups.Remove(RecentColorsItemGroup);
			}
			if(actualShowRecentColors) {
				RecentColorsItemGroup = CreateGalleryItemGroup(RecentColorsCaption, RecentColors.ToList<Color>(), false);
				Gallery.Groups.Add(RecentColorsItemGroup);
			}
		}
		protected internal void CloseOwnedPopup(bool accept) {
			if(OwnerPopupEdit != null) {
				OwnerPopupEdit.Focus();
				if(accept)
					OwnerPopupEdit.ClosePopup();
				else
					OwnerPopupEdit.CancelPopup();
			}
			else {
				if(CloseOwnerPopupOnClick && accept) 
					FindOwnedPopupAndClose();
			}
		}
		void FindOwnedPopupAndClose() {
			DependencyObject element = this;
			while(element != null) {
				Popup popup = element as Popup;
				if(popup != null) {
					if (popup is BarPopupBase)
						PopupMenuManager.ClosePopupBranch((BarPopupBase)popup);
					else
						popup.IsOpen = false;
					break;
				}
#if SL
				SLPopup slPopup = element as SLPopup;
				if(slPopup != null) {
					slPopup.IsOpen = false;
					break;
				}
#endif
				element = GetParent(element);
			}
		}
		DependencyObject GetParent(DependencyObject d) {
#if !SL
			return LayoutHelper.GetParent(d, true);
#else
			return LayoutHelper.GetParent(d);
#endif
		}
		void IColorEdit.AddCustomColor(Color color) {
			RecentColors.Add(color);
			Color = color;
			CloseOwnedPopup(true);
		}
	}
	#region CircularList
	public class CircularList<T> : ICollection<T>, INotifyCollectionChanged {
		List<T> storage = new List<T>();
		int startIndex = -1;
		public int Size { get; private set; }
		public int Count { get { return storage.Count; } }
		NotifyCollectionChangedEventHandler collectionChanged;
		public CircularList(int size) {
			Size = size;
		}
		public T Add(T item) {
			GetNextIndex();
			if(startIndex >= 0) {
				if(storage.Count < Size)
					storage.Insert(startIndex, item);
				else
					storage[startIndex] = item;
			}
			else {
				storage.Add(item);
			}
			OnCollectionChanged();
			return item;
		}
		public T this[int index] {
			get { return ItemAt(index); }
		}
		int GetNextIndex() {
			if(startIndex >= 0 || storage.Count == Size) {
				startIndex = (startIndex + 1) % Size;
				if(startIndex > storage.Count)
					startIndex = storage.Count;
				return startIndex;
			}
			else
				return 0;
		}
		int ResolveIndex(int index) {
			if(startIndex < 0)
				return index;
			else
				return (startIndex + 1 + index) % storage.Count;
		}
		T ItemAt(int index) {
			if(index < 0 || index >= storage.Count)
				throw new IndexOutOfRangeException();
			int i = ResolveIndex((storage.Count - 1) - index);
			return storage[i];
		}
		#region ICollection<T> Members
		void ICollection<T>.Add(T item) {
			Add(item);
		}
		public void Clear() {
			storage.Clear();
			startIndex = 0;
			OnCollectionChanged();
		}
		public bool Contains(T item) {
			return storage.Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			foreach(var t in this) {
				array[arrayIndex++] = t;
			}
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(T item) {
			int i = storage.IndexOf(item);
			if(i >= 0) {
				storage.Remove(item);
				if(i <= startIndex)
					startIndex--;
				OnCollectionChanged();
				return true;
			}
			return false;
		}
		#endregion
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			for(int i = storage.Count - 1; i >= 0; i--)
				yield return storage[ResolveIndex(i)];
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		#region INotifyCollectionChanged Members
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}
		#endregion
		protected virtual void OnCollectionChanged() {
			if(collectionChanged != null)
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		internal void Assign(CircularList<T> b) {
			storage.Clear();
			foreach(T item in b.storage)
				storage.Add(item);
			startIndex = b.startIndex;
			Size = b.Size;
			OnCollectionChanged();
		}
		internal void SetSize(int size) {
			Size = size;
			startIndex = 0;
			if(Count > Size)
				storage = new List<T>(storage.GetRange(0, Size));
			OnCollectionChanged();
		}
	}
	#endregion
	public class ColorEditHelper {
		public static FloatingContainer ShowColorChooserDialog(IColorEdit owner) {
			string title = EditorLocalizer.GetString(EditorStringId.ColorEdit_ColorChooserWindowTitle);
			ColorChooser colorChooser = new ColorChooser() { Foreground = new SolidColorBrush(Colors.Black), Color = owner.Color, FlowDirection = ((FrameworkElement)owner).FlowDirection, Width = 300 };
			DialogClosedDelegate closedHandler = delegate(bool? dialogResult) {
				if(dialogResult == true)
					owner.AddCustomColor(colorChooser.Color);
			};
#if !SILVERLIGHT
			FloatingContainerParameters parameters = new FloatingContainerParameters() { Title = title, ContainerFocusable = false, ClosedDelegate =  closedHandler };
#else 
			FloatingContainerParameters parameters = new FloatingContainerParameters() { Title = title, ClosedDelegate =  closedHandler };
#endif
#if !SL
			return FloatingContainer.ShowDialogContent(colorChooser, owner as FrameworkElement, Size.Empty, parameters, owner as FrameworkElement);
#else
			FloatingContainer.ShowDialogContent(colorChooser, owner as FrameworkElement, Size.Empty, parameters);
			return null;
#endif
		}
		public static Color GetColorFromValue(object value) {
			if(value is Color)
				return (Color)value;
			return ColorEdit.EmptyColor;
		}
		public static ImageSource CreateGlyph(Color color, Brush borderBrush, Size size) {
#if !SL
			DrawingVisual v = new DrawingVisual();
			DrawingContext c = v.RenderOpen();
			c.DrawRectangle(new SolidColorBrush(color), new Pen(borderBrush, 2), new Rect(0, 0, size.Width, size.Height));
			c.Close();
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(v);
			return rtb;
#else 
			WriteableBitmap bitmap = new WriteableBitmap((int)size.Width, (int)size.Height);
			Rectangle r = new Rectangle() { Stroke = borderBrush, Fill = new SolidColorBrush(color), Stretch = Stretch.Fill, Width = size.Width, Height = size.Height };
			bitmap.Render(r, new TranslateTransform());
			bitmap.Invalidate();
			return bitmap;
#endif
		}
		static string GetDefaultColorName(Color color) {
#if SL
			foreach(KeyValuePair<string, Color> pair in DXColor.PredefinedColors) {
				if(pair.Value == color)
					return pair.Key;
			}
#else
			foreach(KeyValuePair<string, System.Drawing.Color> pair in DXColor.PredefinedColors) {
				if(Color.AreClose(Color.FromArgb(pair.Value.A, pair.Value.R, pair.Value.G, pair.Value.B), color))
					return pair.Key;
			}
#endif
			return null;
		}
		public static string GetColorName(Color color, ColorDisplayFormat format) {
			string defaultName = ColorEditDefaultColors.GetColorName(color);
			if(!string.IsNullOrEmpty(defaultName))
				return defaultName;
			if(format == ColorDisplayFormat.ARGB)
				return string.Format("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);
			return color.ToString();
		}
	}
	public delegate void GetColorNameEventHandler(object sender, GetColorNameEventArgs e);
	public class GetColorNameEventArgs : RoutedEventArgs {
		public GetColorNameEventArgs(Color color, string colorName) {
			Color = color;
			ColorName = colorName;
		}
		public Color Color { get; private set; }
		public string ColorName { get; set; }
	}
}
