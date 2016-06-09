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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
#if !SL
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Helpers;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public class PopupBaseEditSettings : ButtonEditSettings, IPopupContentOwner {
		#region static
		public static readonly DependencyProperty PopupWidthProperty;
		public static readonly DependencyProperty PopupHeightProperty;
		public static readonly DependencyProperty PopupMaxWidthProperty;
		public static readonly DependencyProperty PopupMaxHeightProperty;
		public static readonly DependencyProperty PopupMinWidthProperty;
		public static readonly DependencyProperty PopupMinHeightProperty;
		public static readonly DependencyProperty PopupFooterButtonsProperty;
		public static readonly DependencyProperty ShowSizeGripProperty;
		public static readonly DependencyProperty PopupContentTemplateProperty;
		public static readonly DependencyProperty IsSharedPopupSizeProperty;
		public static readonly DependencyProperty PopupTopAreaTemplateProperty;
		public static readonly DependencyProperty PopupBottomAreaTemplateProperty;
		static PopupBaseEditSettings() {
			Type ownerType = typeof(PopupBaseEditSettings);
			PopupWidthProperty = DependencyPropertyManager.Register("PopupWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN));
			PopupHeightProperty = DependencyPropertyManager.Register("PopupHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN));
			PopupMaxWidthProperty = DependencyPropertyManager.Register("PopupMaxWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.PositiveInfinity));
			PopupMaxHeightProperty = DependencyPropertyManager.Register("PopupMaxHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(double.PositiveInfinity));
			PopupMinWidthProperty = DependencyPropertyManager.Register("PopupMinWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(17d));
			PopupMinHeightProperty = DependencyPropertyManager.Register("PopupMinHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(35d));
			PopupFooterButtonsProperty = DependencyPropertyManager.Register("PopupFooterButtons", typeof(PopupFooterButtons?), ownerType, new FrameworkPropertyMetadata(null));
			ShowSizeGripProperty = DependencyPropertyManager.Register("ShowSizeGrip", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			PopupContentTemplateProperty = DependencyPropertyManager.Register("PopupContentTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			IsSharedPopupSizeProperty = DependencyPropertyManager.Register("IsSharedPopupSize", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PopupTopAreaTemplateProperty = DependencyPropertyManager.Register("PopupTopAreaTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			PopupBottomAreaTemplateProperty = DependencyPropertyManager.Register("PopupBottomAreaTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
		}
		protected internal bool IsValueChangedViaPopup { get; set; }
		#endregion
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsIsSharedPopupSize"),
#endif
 Category(EditSettingsCategories.Behavior), SkipPropertyAssertion]
		public bool IsSharedPopupSize {
			get { return (bool)GetValue(IsSharedPopupSizeProperty); }
			set { SetValue(IsSharedPopupSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupWidth"),
#endif
 Category(EditSettingsCategories.Layout)]
		public double PopupWidth {
			get { return (double)GetValue(PopupWidthProperty); }
			set { SetValue(PopupWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupHeight"),
#endif
 Category(EditSettingsCategories.Layout)]
		public double PopupHeight {
			get { return (double)GetValue(PopupHeightProperty); }
			set { SetValue(PopupHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupMinWidth"),
#endif
 Category(EditSettingsCategories.Layout)]
		public double PopupMinWidth {
			get { return (double)GetValue(PopupMinWidthProperty); }
			set { SetValue(PopupMinWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupMinHeight"),
#endif
 Category(EditSettingsCategories.Layout)]
		public double PopupMinHeight {
			get { return (double)GetValue(PopupMinHeightProperty); }
			set { SetValue(PopupMinHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupMaxWidth"),
#endif
 Category(EditSettingsCategories.Layout)]
		public double PopupMaxWidth {
			get { return (double)GetValue(PopupMaxWidthProperty); }
			set { SetValue(PopupMaxWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupMaxHeight"),
#endif
 Category(EditSettingsCategories.Layout)]
		public double PopupMaxHeight {
			get { return (double)GetValue(PopupMaxHeightProperty); }
			set { SetValue(PopupMaxHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupFooterButtons"),
#endif
 Category(EditSettingsCategories.Behavior)]
#if SL
		[TypeConverter(typeof(NullableConverter<PopupFooterButtons>))]
#endif
		public PopupFooterButtons? PopupFooterButtons {
			get { return (PopupFooterButtons?)GetValue(PopupFooterButtonsProperty); }
			set { SetValue(PopupFooterButtonsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsShowSizeGrip"),
#endif
 Category(EditSettingsCategories.Behavior)]
		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShowSizeGrip {
			get { return (bool?)GetValue(ShowSizeGripProperty); }
			set { SetValue(ShowSizeGripProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditSettingsPopupContentTemplate"),
#endif
 Category(EditSettingsCategories.Appearance), SkipPropertyAssertion]
		public ControlTemplate PopupContentTemplate {
			get { return (ControlTemplate)GetValue(PopupContentTemplateProperty); }
			set { SetValue(PopupContentTemplateProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public ControlTemplate PopupTopAreaTemplate {
			get { return (ControlTemplate)GetValue(PopupTopAreaTemplateProperty); }
			set { SetValue(PopupTopAreaTemplateProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public ControlTemplate PopupBottomAreaTemplate {
			get { return (ControlTemplate)GetValue(PopupBottomAreaTemplateProperty); }
			set { SetValue(PopupBottomAreaTemplateProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			PopupBaseEdit pbe = edit as PopupBaseEdit;
			if (pbe == null)
				return;
			SetValueFromSettings(PopupWidthProperty, () => pbe.PopupWidth = PopupWidth);
			SetValueFromSettings(PopupHeightProperty, () => pbe.PopupHeight = PopupHeight);
			SetValueFromSettings(PopupMinWidthProperty, () => pbe.PopupMinWidth = PopupMinWidth);
			SetValueFromSettings(PopupMinHeightProperty, () => pbe.PopupMinHeight = PopupMinHeight);
			SetValueFromSettings(PopupMaxWidthProperty, () => pbe.PopupMaxWidth = PopupMaxWidth);
			SetValueFromSettings(PopupMaxHeightProperty, () => pbe.PopupMaxHeight = PopupMaxHeight);
			SetValueFromSettings(PopupFooterButtonsProperty, () => pbe.PopupFooterButtons = PopupFooterButtons);
			SetValueFromSettings(ShowSizeGripProperty, () => pbe.ShowSizeGrip = ShowSizeGrip);
			SetValueFromSettings(PopupContentTemplateProperty,
				() => pbe.PopupContentTemplate = PopupContentTemplate,
				() => ClearEditorPropertyIfNeeded(pbe, PopupBaseEdit.PopupContentTemplateProperty, PopupContentTemplateProperty));
			SetValueFromSettings(PopupTopAreaTemplateProperty,
				() => pbe.PopupTopAreaTemplate = PopupTopAreaTemplate,
				() => ClearEditorPropertyIfNeeded(pbe, PopupBaseEdit.PopupTopAreaTemplateProperty, PopupTopAreaTemplateProperty));
			SetValueFromSettings(PopupBottomAreaTemplateProperty,
				() => pbe.PopupBottomAreaTemplate = PopupBottomAreaTemplate,
				() => ClearEditorPropertyIfNeeded(pbe, PopupBaseEdit.PopupBottomAreaTemplateProperty, PopupBottomAreaTemplateProperty));
		}
		protected internal override ButtonInfoBase CreateDefaultButtonInfo() {
			return new ButtonInfo() { GlyphKind = GlyphKind.DropDown, ButtonKind = ButtonKind.Toggle, IsDefaultButton = true };
		}
#if !SL
		#region IPopupOwner Members
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				List<object> list = new List<object>();
				IEnumerator enumerator = base.LogicalChildren;
				if (enumerator != null) {
					while (enumerator.MoveNext())
						list.Add(enumerator.Current);
				}
				if (child != null)
					list.Add(child);
				return list.GetEnumerator();
			}
		}
		FrameworkElement child;
		FrameworkElement IPopupContentOwner.Child {
			get { return child; }
			set {
				if (value == child)
					return;
				RemoveLogicalChild(child);
				child = value;
				AddLogicalChild(child);
			}
		}
		#endregion
#else
		FrameworkElement IPopupContentOwner.Child { get; set; }
#endif
		protected internal virtual bool IsTogglePopupOpenGesture(Key key, ModifierKeys modifiers) {
			return (key == Key.Down && ModifierKeysHelper.IsAltPressed(modifiers)) || (key == Key.F4 && ModifierKeysHelper.NoModifiers(modifiers));
		}
		protected internal override bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (IsTogglePopupOpenGesture(key, modifiers))
				return true;
			return base.IsActivatingKey(key, modifiers);
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (e.Property.Name == "PopupHeight" || e.Property.Name == "PopupWidth")
				return;
			base.OnPropertyChanged(e);
		}
#endif
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class PopupBaseSettingsExtension : ButtonSettingsExtension {
		public double PopupWidth { get; set; }
		public double PopupHeight { get; set; }
		public double PopupMinWidth { get; set; }
		public double PopupMinHeight { get; set; }
		public double PopupMaxWidth { get; set; }
		public double PopupMaxHeight { get; set; }
		public bool IsSharedPopupSize { get; set; }
		public PopupFooterButtons? PopupFooterButtons { get; set; }
		public bool? ShowSizeGrip { get; set; }
		public ControlTemplate PopupTopAreaTemplate { get; set; }
		public ControlTemplate PopupBottomAreaTemplate { get; set; }
		public PopupBaseSettingsExtension() {
			PopupWidth = (double)PopupBaseEditSettings.PopupWidthProperty.DefaultMetadata.DefaultValue;
			PopupHeight = (double)PopupBaseEditSettings.PopupHeightProperty.DefaultMetadata.DefaultValue;
			PopupMinWidth = (double)PopupBaseEditSettings.PopupMinWidthProperty.DefaultMetadata.DefaultValue;
			PopupMinHeight = (double)PopupBaseEditSettings.PopupMinHeightProperty.DefaultMetadata.DefaultValue;
			PopupMaxWidth = (double)PopupBaseEditSettings.PopupMaxWidthProperty.DefaultMetadata.DefaultValue;
			PopupMaxHeight = (double)PopupBaseEditSettings.PopupMaxHeightProperty.DefaultMetadata.DefaultValue;
			IsSharedPopupSize = true;
			PopupFooterButtons = (PopupFooterButtons?)PopupBaseEditSettings.PopupFooterButtonsProperty.DefaultMetadata.DefaultValue;
			ShowSizeGrip = (bool?)PopupBaseEditSettings.ShowSizeGripProperty.DefaultMetadata.DefaultValue;
			PopupTopAreaTemplate = (ControlTemplate)PopupBaseEditSettings.PopupTopAreaTemplateProperty.DefaultMetadata.DefaultValue;
			PopupBottomAreaTemplate = (ControlTemplate)PopupBaseEditSettings.PopupBottomAreaTemplateProperty.DefaultMetadata.DefaultValue;
		}
		protected sealed override ButtonEditSettings CreateButtonEditSettings() {
			PopupBaseEditSettings settings = CreatePopupBaseEditSettings();
			Assign(settings);
			return settings;
		}
		protected virtual void Assign(PopupBaseEditSettings settings) {
			settings.PopupWidth = PopupWidth;
			settings.PopupHeight = PopupHeight;
			settings.PopupMinWidth = PopupMinWidth;
			settings.PopupMinHeight = PopupMinHeight;
			settings.PopupMaxWidth = PopupMaxWidth;
			settings.PopupMaxHeight = PopupMaxHeight;
			settings.PopupFooterButtons = PopupFooterButtons;
			settings.ShowSizeGrip = ShowSizeGrip;
			settings.IsSharedPopupSize = IsSharedPopupSize;
			settings.PopupTopAreaTemplate = PopupTopAreaTemplate;
			settings.PopupBottomAreaTemplate = PopupBottomAreaTemplate;
		}
		protected virtual PopupBaseEditSettings CreatePopupBaseEditSettings() {
			return new PopupBaseEditSettings();
		}
	}
}
#endif
