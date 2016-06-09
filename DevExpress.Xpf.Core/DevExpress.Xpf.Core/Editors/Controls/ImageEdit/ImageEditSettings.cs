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
using DevExpress.Xpf.Editors.Settings.Extension;
using DevExpress.Xpf.Bars;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Media.Effects;
#else
using System.Windows.Media;
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
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using System.Windows.Media.Effects;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public partial class ImageEditSettings : BaseEditSettings {
		public static readonly DependencyProperty ShowMenuProperty;
		public static readonly DependencyProperty ShowMenuModeProperty;
		public static readonly DependencyProperty StretchProperty;
		public static readonly DependencyProperty ShowLoadDialogOnClickModeProperty;
		public static readonly DependencyProperty ImageEffectProperty;
		static ImageEditSettings() {
			Type ownerType = typeof(ImageEditSettings);
			ShowMenuProperty = DependencyPropertyManager.Register("ShowMenu", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowMenuModeProperty = DependencyPropertyManager.Register("ShowMenuMode", typeof(ShowMenuMode), ownerType, new FrameworkPropertyMetadata(ShowMenuMode.Hover));
			StretchProperty = DependencyPropertyManager.Register("Stretch", typeof(Stretch), ownerType, new FrameworkPropertyMetadata(Stretch.Uniform));
			ShowLoadDialogOnClickModeProperty = DependencyPropertyManager.Register("ShowLoadDialogOnClickMode", typeof(ShowLoadDialogOnClickMode), ownerType, new FrameworkPropertyMetadata(ShowLoadDialogOnClickMode.Empty));
			ImageEffectProperty = DependencyPropertyManager.Register("ImageEffect", typeof(Effect), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool ShowMenu {
			get { return (bool)GetValue(ShowMenuProperty); }
			set { SetValue(ShowMenuProperty, value); }
		}
		public ShowMenuMode ShowMenuMode {
			get { return (ShowMenuMode)GetValue(ShowMenuModeProperty); }
			set { SetValue(ShowMenuModeProperty, value); }
		}
		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
		public ShowLoadDialogOnClickMode ShowLoadDialogOnClickMode {
			get { return (ShowLoadDialogOnClickMode)GetValue(ShowLoadDialogOnClickModeProperty); }
			set { SetValue(ShowLoadDialogOnClickModeProperty, value); }
		}
		public Effect ImageEffect {
			get { return (Effect)GetValue(ImageEffectProperty); }
			set { SetValue(ImageEffectProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			ImageEdit editor = edit as ImageEdit;
			if(editor == null)
				return;
			SetValueFromSettings(ShowMenuProperty, () => editor.ShowMenu = ShowMenu);
			SetValueFromSettings(ShowMenuModeProperty, () => editor.ShowMenuMode = ShowMenuMode);
			SetValueFromSettings(StretchProperty, () => editor.Stretch = Stretch);
			SetValueFromSettings(ShowLoadDialogOnClickModeProperty, () => editor.ShowLoadDialogOnClickMode = ShowLoadDialogOnClickMode);
			SetValueFromSettings(ImageEffectProperty, () => editor.ImageEffect = ImageEffect);
		}
		static object convertEditValue = new object();
		public event ConvertEditValueEventHandler ConvertEditValue {
			add { AddHandler(convertEditValue, value); }
			remove { RemoveHandler(convertEditValue, value); }
		}
		protected internal virtual void RaiseConvertEditValue(DependencyObject sender, ConvertEditValueEventArgs e) {
			Delegate handler;
			if(Events.TryGetValue(convertEditValue, out handler))
				((ConvertEditValueEventHandler)handler)(sender, e);
		}
	}
	public partial class PopupImageEditSettings : PopupBaseEditSettings {
		public static readonly DependencyProperty ShowMenuProperty;
		public static readonly DependencyProperty ShowMenuModeProperty;
		public static readonly DependencyProperty StretchProperty;
		public static readonly DependencyProperty ShowLoadDialogOnClickModeProperty;
		public static readonly DependencyProperty ImageEffectProperty;
		static PopupImageEditSettings() {
			Type ownerType = typeof(PopupImageEditSettings);
			ShowMenuProperty = ImageEditSettings.ShowMenuProperty.AddOwner(ownerType);
			ShowMenuModeProperty = ImageEditSettings.ShowMenuModeProperty.AddOwner(ownerType);
			StretchProperty = ImageEditSettings.StretchProperty.AddOwner(ownerType);
			ShowLoadDialogOnClickModeProperty = ImageEditSettings.ShowLoadDialogOnClickModeProperty.AddOwner(ownerType);
			ImageEffectProperty = ImageEditSettings.ImageEffectProperty.AddOwner(ownerType);
#if !SL
			IsTextEditableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
			PopupFooterButtonsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DevExpress.Xpf.Editors.PopupFooterButtons.OkCancel));
			ShowSizeGripProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(PopupImageEdit.DefaultPopupMinHeight));
			IsSharedPopupSizeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
#endif
		}
		public PopupImageEditSettings() {
#if SL
			IsSharedPopupSize = false;
#endif
		}
		public bool ShowMenu {
			get { return (bool)GetValue(ShowMenuProperty); }
			set { SetValue(ShowMenuProperty, value); }
		}
		public ShowMenuMode ShowMenuMode {
			get { return (ShowMenuMode)GetValue(ShowMenuModeProperty); }
			set { SetValue(ShowMenuModeProperty, value); }
		}
		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
		public ShowLoadDialogOnClickMode ShowLoadDialogOnClickMode {
			get { return (ShowLoadDialogOnClickMode)GetValue(ShowLoadDialogOnClickModeProperty); }
			set { SetValue(ShowLoadDialogOnClickModeProperty, value); }
		}
		public Effect ImageEffect {
			get { return (Effect)GetValue(ImageEffectProperty); }
			set { SetValue(ImageEffectProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			PopupImageEdit editor = edit as PopupImageEdit;
			if(editor == null)
				return;
			SetValueFromSettings(ShowMenuProperty, () => editor.ShowMenu = ShowMenu);
			SetValueFromSettings(ShowMenuModeProperty, () => editor.ShowMenuMode = ShowMenuMode);
			SetValueFromSettings(StretchProperty, () => editor.Stretch = Stretch);
			SetValueFromSettings(ShowLoadDialogOnClickModeProperty, () => editor.ShowLoadDialogOnClickMode = ShowLoadDialogOnClickMode);
			SetValueFromSettings(ImageEffectProperty, () => editor.ImageEffect = ImageEffect);
		}
		static object convertEditValue = new object();
		public event ConvertEditValueEventHandler ConvertEditValue {
			add { AddHandler(convertEditValue, value); }
			remove { RemoveHandler(convertEditValue, value); }
		}
		protected internal virtual void RaiseConvertEditValue(DependencyObject sender, ConvertEditValueEventArgs e) {
			Delegate handler;
			if(Events.TryGetValue(convertEditValue, out handler))
				((ConvertEditValueEventHandler)handler)(sender, e);
		}
	}
}
#if !SL
	namespace DevExpress.Xpf.Editors.Settings.Extension {
		public class ImageEditSettingsExtension : BaseSettingsExtension {
			public bool ShowMenu { get; set; }
			public ShowMenuMode ShowMenuMode { get; set; }
			public Stretch Stretch { get; set; }
			public ShowLoadDialogOnClickMode ShowLoadDialogOnClickMode { get; set; }
			public Effect ImageEffect { get; set; }
			public ImageEditSettingsExtension() {
				ShowMenu = (bool)ImageEdit.ShowMenuProperty.DefaultMetadata.DefaultValue;
				ShowMenuMode = (ShowMenuMode)ImageEdit.ShowMenuModeProperty.DefaultMetadata.DefaultValue;
				Stretch = (Stretch)ImageEdit.StretchProperty.DefaultMetadata.DefaultValue;
				ShowLoadDialogOnClickMode = (Editors.ShowLoadDialogOnClickMode)ImageEdit.ShowLoadDialogOnClickModeProperty.DefaultMetadata.DefaultValue;
				ImageEffect = (Effect)ImageEdit.ImageEffectProperty.DefaultMetadata.DefaultValue;
			}
			protected override BaseEditSettings CreateEditSettings() {
				ImageEditSettings settings = new ImageEditSettings();
				Assign(settings);
				return settings;
			}
			protected virtual void Assign(ImageEditSettings settings) {
				settings.ShowMenu = ShowMenu;
				settings.ShowMenuMode = ShowMenuMode;
				settings.Stretch = Stretch;
				settings.ShowLoadDialogOnClickMode = ShowLoadDialogOnClickMode;
				settings.ImageEffect = ImageEffect;
			}
		}
		public class PopupImageEditSettingsExtension : PopupBaseSettingsExtension {
			public bool ShowMenu { get; set; }
			public ShowMenuMode ShowMenuMode { get; set; }
			public Stretch Stretch { get; set; }
			public ShowLoadDialogOnClickMode ShowLoadDialogOnClickMode { get; set; }
			public Effect ImageEffect { get; set; }
			public PopupImageEditSettingsExtension() {
				IsTextEditable = false;
				ShowSizeGrip = true;
				PopupMinHeight = PopupImageEdit.DefaultPopupMinHeight;
				ShowMenu = (bool)PopupImageEdit.ShowMenuProperty.DefaultMetadata.DefaultValue;
				ShowMenuMode = (ShowMenuMode)ImageEdit.ShowMenuModeProperty.DefaultMetadata.DefaultValue;
				Stretch = (Stretch)PopupImageEdit.StretchProperty.DefaultMetadata.DefaultValue;
				ShowLoadDialogOnClickMode = (Editors.ShowLoadDialogOnClickMode)PopupImageEdit.ShowLoadDialogOnClickModeProperty.DefaultMetadata.DefaultValue;
				ImageEffect = (Effect)PopupImageEdit.ImageEffectProperty.DefaultMetadata.DefaultValue;
			}
			protected sealed override PopupBaseEditSettings CreatePopupBaseEditSettings() {
				PopupImageEditSettings settings = CreateImageEditSettings();
				settings.ShowMenu = ShowMenu;
				settings.ShowMenuMode = ShowMenuMode;
				settings.Stretch = Stretch;
				settings.ShowLoadDialogOnClickMode = ShowLoadDialogOnClickMode;
				settings.ImageEffect = ImageEffect;
				return settings;
			}
			protected virtual PopupImageEditSettings CreateImageEditSettings() {
				return new PopupImageEditSettings();
			}
		}
	}
#endif
