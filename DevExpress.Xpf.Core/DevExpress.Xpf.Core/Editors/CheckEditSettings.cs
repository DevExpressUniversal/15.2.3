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
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
#if !SL
using DevExpress.Xpf.Editors.Settings;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
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
	public class CheckEditSettings : BaseEditSettings {
		#region static
		public static readonly DependencyProperty ClickModeProperty;
		public static readonly DependencyProperty IsThreeStateProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
#if !SL
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
#endif
		static CheckEditSettings() {
			ClickModeProperty = CheckEdit.ClickModeProperty.AddOwner(typeof(CheckEditSettings));
			IsThreeStateProperty = CheckEdit.IsThreeStateProperty.AddOwner(typeof(CheckEditSettings));
			ContentProperty = CheckEdit.ContentProperty.AddOwner(typeof(CheckEditSettings));
			ContentTemplateProperty = CheckEdit.ContentTemplateProperty.AddOwner(typeof(CheckEditSettings));
#if !SL
			ContentTemplateSelectorProperty = CheckEdit.ContentTemplateSelectorProperty.AddOwner(typeof(CheckEditSettings));
#endif
		}
		#endregion
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditSettingsClickMode"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public ClickMode ClickMode {
			get { return (ClickMode)GetValue(ClickModeProperty); }
			set { SetValue(ClickModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditSettingsIsThreeState"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { SetValue(IsThreeStateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditSettingsContent"),
#endif
 Category(EditSettingsCategories.Data)
#if !SL
, TypeConverter(typeof(DevExpress.Xpf.Core.ObjectConverter))
#endif
]
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditSettingsContentTemplate"),
#endif
 Category(EditSettingsCategories.Appearance)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditSettingsContentTemplateSelector"),
#endif
 Category(EditSettingsCategories.Appearance)]
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
#endif
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			CheckEdit ce = edit as CheckEdit;
			if (ce == null)
				return;
			SetValueFromSettings(ClickModeProperty, () => ce.ClickMode = ClickMode);
			SetValueFromSettings(IsThreeStateProperty, () => ce.IsThreeState = IsThreeState);
			SetValueFromSettings(ContentProperty, () => ce.Content = Content);
			SetValueFromSettings(ContentTemplateProperty, () => ce.ContentTemplate = ContentTemplate);
#if !SL
			SetValueFromSettings(ContentTemplateSelectorProperty, () => ce.ContentTemplateSelector = ContentTemplateSelector);
#endif
		}
		public bool IsToggleCheckGesture(Key key) {
			return key == Key.Space;
		}
		protected internal override bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (IsToggleCheckGesture(key))
				return true;
			return base.IsActivatingKey(key, modifiers);
		}
		protected override EditSettingsHorizontalAlignment GetActualHorizontalContentAlignment() {
			return EditSettingsHorizontalAlignment.Center;
		}
		protected internal static bool? GetBooleanFromEditValue(object editValue, bool isThreeState) {
			if (editValue is Boolean)
				return (Boolean)editValue;
			if (IsNativeNullValue(editValue))
				return new bool?();
			try {
				if (editValue is string) {
					bool result;
					if (bool.TryParse(editValue as string, out result))
						return result;
					return GetDefaultBooleanValue(isThreeState);
				}
				return Convert.ToBoolean(editValue);
			}
			catch {
				return GetDefaultBooleanValue(isThreeState);
			}
		}
		static bool? GetDefaultBooleanValue(bool isThreeState) {
			return isThreeState ? new bool?() : false;
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class CheckSettingsExtension : BaseSettingsExtension {
		public ClickMode ClickMode { get; set; }
		public bool IsThreeState { get; set; }
		public object Content { get; set; }
		public DataTemplate ContentTemplate { get; set; }
		public DataTemplateSelector ContentTemplateSelector { get; set; }
		public CheckSettingsExtension() {
			ClickMode = (ClickMode)CheckEditSettings.ClickModeProperty.GetMetadata(typeof(CheckEditSettings)).DefaultValue;
			IsThreeState = (bool)CheckEditSettings.IsThreeStateProperty.GetMetadata(typeof(CheckEditSettings)).DefaultValue;
			Content = CheckEditSettings.ContentProperty.GetMetadata(typeof(CheckEditSettings)).DefaultValue;
			ContentTemplate = (DataTemplate)CheckEditSettings.ContentTemplateProperty.GetMetadata(typeof(CheckEditSettings)).DefaultValue;
			ContentTemplateSelector = (DataTemplateSelector)CheckEditSettings.ContentTemplateSelectorProperty.GetMetadata(typeof(CheckEditSettings)).DefaultValue;
		}
		protected override BaseEditSettings CreateEditSettings() {
			CheckEditSettings settings = new CheckEditSettings() {
				ClickMode = this.ClickMode,
				IsThreeState = this.IsThreeState,
				Content = this.Content,
				ContentTemplate = this.ContentTemplate,
				ContentTemplateSelector = this.ContentTemplateSelector
			};
			return settings;
		}
	}
}
#endif
