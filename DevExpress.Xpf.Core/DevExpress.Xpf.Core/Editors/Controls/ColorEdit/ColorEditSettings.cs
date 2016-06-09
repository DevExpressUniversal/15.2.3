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
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Settings.Extension;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public class ColorEditSettings : BaseEditSettings {
		#region static
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty DefaultColorProperty;
		public static readonly DependencyProperty ShowMoreColorsButtonProperty;
		public static readonly DependencyProperty ShowNoColorButtonProperty;
		public static readonly DependencyProperty ShowDefaultColorButtonProperty;
		public static readonly DependencyProperty ChipSizeProperty;
		public static readonly DependencyProperty ChipMarginProperty;
		public static readonly DependencyProperty ChipBorderBrushProperty;
		public static readonly DependencyProperty PalettesProperty;
		static ColorEditSettings() {
			Type ownerType = typeof(ColorEditSettings);
			ColumnCountProperty = DependencyPropertyManager.Register("ColumnCount", typeof(int), ownerType, new PropertyMetadata(10, OnSettingsPropertyChanged));
			DefaultColorProperty = DependencyPropertyManager.Register("DefaultColor", typeof(Color), ownerType, new PropertyMetadata(Colors.Black, OnSettingsPropertyChanged));
			ShowMoreColorsButtonProperty = DependencyPropertyManager.Register("ShowMoreColorsButton", typeof(bool), ownerType, new PropertyMetadata(true, OnSettingsPropertyChanged));
			ShowNoColorButtonProperty = DependencyPropertyManager.Register("ShowNoColorButton", typeof(bool), ownerType, new PropertyMetadata(false, OnSettingsPropertyChanged));
			ShowDefaultColorButtonProperty = DependencyPropertyManager.Register("ShowDefaultColorButton", typeof(bool), ownerType, new PropertyMetadata(true, OnSettingsPropertyChanged));
			ChipSizeProperty = DependencyPropertyManager.Register("ChipSize", typeof(ChipSize), ownerType, new PropertyMetadata(ChipSize.Default, OnSettingsPropertyChanged));
			ChipMarginProperty = DependencyPropertyManager.Register("ChipMargin", typeof(Thickness), ownerType, new PropertyMetadata(new Thickness(2, 0, 2, 0), OnSettingsPropertyChanged));
			ChipBorderBrushProperty = DependencyPropertyManager.Register("ChipBorderBrush", typeof(Brush), ownerType, new PropertyMetadata(null, OnSettingsPropertyChanged));
			PalettesProperty = DependencyPropertyManager.Register("Palettes", typeof(PaletteCollection), ownerType, new PropertyMetadata(null, OnSettingsPropertyChanged));
		}
		#endregion
		readonly int defaultColumnCount;
		CircularList<Color> recentColors;
		public ColorEditSettings() {
			defaultColumnCount = (int)ColorEditSettings.ColumnCountProperty.GetMetadata(typeof(PopupColorEditSettings)).DefaultValue;
		}
		public CircularList<Color> RecentColors {
			get {
				if(recentColors == null)
					recentColors = new CircularList<Color>(defaultColumnCount);
				return recentColors;
			}
		}
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
		public Color DefaultColor {
			get { return (Color)GetValue(DefaultColorProperty); }
			set { SetValue(DefaultColorProperty, value); }
		}
		public bool ShowDefaultColorButton {
			get { return (bool)GetValue(ShowDefaultColorButtonProperty); }
			set { SetValue(ShowDefaultColorButtonProperty, value); }
		}
		public bool ShowMoreColorsButton {
			get { return (bool)GetValue(ShowMoreColorsButtonProperty); }
			set { SetValue(ShowMoreColorsButtonProperty, value); }
		}
		public bool ShowNoColorButton {
			get { return (bool)GetValue(ShowNoColorButtonProperty); }
			set { SetValue(ShowNoColorButtonProperty, value); }
		}
		public ChipSize ChipSize {
			get { return (ChipSize)GetValue(ChipSizeProperty); }
			set { SetValue(ChipSizeProperty, value); }
		}
		public Thickness ChipMargin {
			get { return (Thickness)GetValue(ChipMarginProperty); }
			set { SetValue(ChipMarginProperty, value); }
		}
		public Brush ChipBorderBrush {
			get { return (Brush)GetValue(ChipBorderBrushProperty); }
			set { SetValue(ChipBorderBrushProperty, value); }
		}
		public PaletteCollection Palettes {
			get { return (PaletteCollection)GetValue(PalettesProperty); }
			set { SetValue(PalettesProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			ColorEdit editor = edit as ColorEdit;
			if(editor == null)
				return;
			SetValueFromSettings(ColumnCountProperty, () => editor.ColumnCount = ColumnCount);
			SetValueFromSettings(DefaultColorProperty, () => editor.DefaultColor = DefaultColor);
			SetValueFromSettings(ShowDefaultColorButtonProperty, () => editor.ShowDefaultColorButton = ShowDefaultColorButton);
			SetValueFromSettings(ShowMoreColorsButtonProperty, () => editor.ShowMoreColorsButton = ShowMoreColorsButton);
			SetValueFromSettings(ShowNoColorButtonProperty, () => editor.ShowNoColorButton = ShowNoColorButton);
			SetValueFromSettings(ChipSizeProperty, () => editor.ChipSize = ChipSize);
			SetValueFromSettings(ChipMarginProperty, () => editor.ChipMargin = ChipMargin);
			if(Palettes != null)
				SetValueFromSettings(PalettesProperty, () => editor.Palettes = Palettes);
			if(ChipBorderBrush != null)
				SetValueFromSettings(ChipBorderBrushProperty, () => editor.ChipBorderBrush = ChipBorderBrush);
			if(editor.Settings != this)
				editor.RecentColors.Assign(RecentColors);
		}
	}
	public class PopupColorEditSettings : PopupBaseEditSettings {
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty DefaultColorProperty;
		public static readonly DependencyProperty ShowMoreColorsButtonProperty;
		public static readonly DependencyProperty ShowNoColorButtonProperty;
		public static readonly DependencyProperty ShowDefaultColorButtonProperty;
		public static readonly DependencyProperty ChipSizeProperty;
		public static readonly DependencyProperty ChipMarginProperty;
		public static readonly DependencyProperty ChipBorderBrushProperty;
		public static readonly DependencyProperty PalettesProperty;
		static PopupColorEditSettings() {
			Type ownerType = typeof(PopupColorEditSettings);
			ColumnCountProperty = ColorEditSettings.ColumnCountProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			DefaultColorProperty = ColorEditSettings.DefaultColorProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ShowMoreColorsButtonProperty = ColorEditSettings.ShowMoreColorsButtonProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ShowNoColorButtonProperty = ColorEditSettings.ShowNoColorButtonProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ShowDefaultColorButtonProperty = ColorEditSettings.ShowDefaultColorButtonProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ChipSizeProperty = ColorEditSettings.ChipSizeProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ChipMarginProperty = ColorEditSettings.ChipMarginProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ChipBorderBrushProperty = ColorEditSettings.ChipBorderBrushProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			PalettesProperty = ColorEditSettings.PalettesProperty.AddOwner(ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
#if !SL
			IsTextEditableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
#endif
		}
		CircularList<Color> recentColors;
		readonly int defaultColumnCount;
		public PopupColorEditSettings() {
			defaultColumnCount = (int)PopupColorEditSettings.ColumnCountProperty.GetMetadata(typeof(PopupColorEditSettings)).DefaultValue;
		}
		public CircularList<Color> RecentColors {
			get {
				if(recentColors == null)
					recentColors = new CircularList<Color>(defaultColumnCount);
				return recentColors;
			}
		}
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
		public Color DefaultColor {
			get { return (Color)GetValue(DefaultColorProperty); }
			set { SetValue(DefaultColorProperty, value); }
		}
		public bool ShowDefaultColorButton {
			get { return (bool)GetValue(ShowDefaultColorButtonProperty); }
			set { SetValue(ShowDefaultColorButtonProperty, value); }
		}
		public bool ShowMoreColorsButton {
			get { return (bool)GetValue(ShowMoreColorsButtonProperty); }
			set { SetValue(ShowMoreColorsButtonProperty, value); }
		}
		public bool ShowNoColorButton {
			get { return (bool)GetValue(ShowNoColorButtonProperty); }
			set { SetValue(ShowNoColorButtonProperty, value); }
		}
		public ChipSize ChipSize {
			get { return (ChipSize)GetValue(ChipSizeProperty); }
			set { SetValue(ChipSizeProperty, value); }
		}
		public Thickness ChipMargin {
			get { return (Thickness)GetValue(ChipMarginProperty); }
			set { SetValue(ChipMarginProperty, value); }
		}
		public Brush ChipBorderBrush {
			get { return (Brush)GetValue(ChipBorderBrushProperty); }
			set { SetValue(ChipBorderBrushProperty, value); }
		}
		public PaletteCollection Palettes {
			get { return (PaletteCollection)GetValue(PalettesProperty); }
			set { SetValue(PalettesProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			PopupColorEdit editor = edit as PopupColorEdit;
			if(editor == null)
				return;
			SetValueFromSettings(ColumnCountProperty, () => editor.ColumnCount = ColumnCount);
			SetValueFromSettings(DefaultColorProperty, () => editor.DefaultColor = DefaultColor);
			SetValueFromSettings(ShowDefaultColorButtonProperty, () => editor.ShowDefaultColorButton = ShowDefaultColorButton);
			SetValueFromSettings(ShowMoreColorsButtonProperty, () => editor.ShowMoreColorsButton = ShowMoreColorsButton);
			SetValueFromSettings(ShowNoColorButtonProperty, () => editor.ShowNoColorButton = ShowNoColorButton);
			SetValueFromSettings(ChipSizeProperty, () => editor.ChipSize = ChipSize);
			SetValueFromSettings(ChipMarginProperty, () => editor.ChipMargin = ChipMargin);
			if(Palettes != null)
				SetValueFromSettings(PalettesProperty, () => editor.Palettes = Palettes);
			if(ChipBorderBrush != null)
				SetValueFromSettings(ChipBorderBrushProperty, () => editor.ChipBorderBrush = ChipBorderBrush);
			if(editor.Settings != this)
				editor.RecentColors.Assign(RecentColors);
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class ColorEditSettingsExtension : BaseSettingsExtension {
		public int ColumnCount { get; set; }
		public Color DefaultColor { get; set; }
		public bool ShowMoreColorsButton { get; set; }
		public bool ShowNoColorButton { get; set; }
		public bool ShowDefaultColorButton { get; set; }
		public ChipSize ChipSize { get; set; }
		public Thickness ChipMargin { get; set; }
		public Brush ChipBorderBrush { get; set; }
		public ColorEditSettingsExtension() {
			ColumnCount = (int)ColorEditSettings.ColumnCountProperty.DefaultMetadata.DefaultValue;
			DefaultColor = (Color)ColorEditSettings.DefaultColorProperty.DefaultMetadata.DefaultValue;
			ShowMoreColorsButton = (bool)ColorEditSettings.ShowMoreColorsButtonProperty.DefaultMetadata.DefaultValue;
			ShowNoColorButton = (bool)ColorEditSettings.ShowNoColorButtonProperty.DefaultMetadata.DefaultValue;
			ShowDefaultColorButton = (bool)ColorEditSettings.ShowDefaultColorButtonProperty.DefaultMetadata.DefaultValue;
			ChipSize = (ChipSize)ColorEditSettings.ChipSizeProperty.DefaultMetadata.DefaultValue;
			ChipMargin = (Thickness)ColorEditSettings.ChipMarginProperty.DefaultMetadata.DefaultValue;
		}
		protected override BaseEditSettings CreateEditSettings() {
			ColorEditSettings settings = new ColorEditSettings();
			Assign(settings);
			return settings;
		}
		protected virtual void Assign(ColorEditSettings settings) {
			settings.ColumnCount = ColumnCount;
			settings.DefaultColor = DefaultColor;
			settings.ShowMoreColorsButton = ShowMoreColorsButton;
			settings.ShowNoColorButton = ShowNoColorButton;
			settings.ShowDefaultColorButton = ShowDefaultColorButton;
			settings.ChipSize = ChipSize;
			settings.ChipMargin = ChipMargin;
			settings.ChipBorderBrush = ChipBorderBrush;
		}
	}
	public class PopupColorEditSettingsExtension : PopupBaseSettingsExtension {
		public int ColumnCount { get; set; }
		public Color DefaultColor { get; set; }
		public bool ShowMoreColorsButton { get; set; }
		public bool ShowNoColorButton { get; set; }
		public bool ShowDefaultColorButton { get; set; }
		public ChipSize ChipSize { get; set; }
		public Thickness ChipMargin { get; set; }
		public Brush ChipBorderBrush { get; set; }
		public PopupColorEditSettingsExtension() {
			ColumnCount = (int)PopupColorEditSettings.ColumnCountProperty.DefaultMetadata.DefaultValue;
			DefaultColor = (Color)PopupColorEditSettings.DefaultColorProperty.DefaultMetadata.DefaultValue;
			ShowMoreColorsButton = (bool)PopupColorEditSettings.ShowMoreColorsButtonProperty.DefaultMetadata.DefaultValue;
			ShowNoColorButton = (bool)PopupColorEditSettings.ShowNoColorButtonProperty.DefaultMetadata.DefaultValue;
			ShowDefaultColorButton = (bool)PopupColorEditSettings.ShowDefaultColorButtonProperty.DefaultMetadata.DefaultValue;
			ChipSize = (ChipSize)PopupColorEditSettings.ChipSizeProperty.DefaultMetadata.DefaultValue;
			ChipMargin = (Thickness)PopupColorEditSettings.ChipMarginProperty.DefaultMetadata.DefaultValue;
		}
		protected override void Assign(PopupBaseEditSettings settings) {
			base.Assign(settings);
			PopupColorEditSettings colorSettings = (PopupColorEditSettings)settings;
			colorSettings.ColumnCount = ColumnCount;
			colorSettings.DefaultColor = DefaultColor;
			colorSettings.ShowMoreColorsButton = ShowMoreColorsButton;
			colorSettings.ShowNoColorButton = ShowNoColorButton;
			colorSettings.ShowDefaultColorButton = ShowDefaultColorButton;
			colorSettings.ChipSize = ChipSize;
			colorSettings.ChipMargin = ChipMargin;
			colorSettings.ChipBorderBrush = ChipBorderBrush;
		}
		protected override PopupBaseEditSettings CreatePopupBaseEditSettings() {
			return new PopupColorEditSettings();
		}
	}
}
#endif
