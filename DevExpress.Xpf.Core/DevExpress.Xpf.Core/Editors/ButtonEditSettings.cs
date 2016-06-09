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
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Collections;
using System.Windows.Markup;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Settings {
	public partial class ButtonEditSettings : TextEditSettings {
		#region static
		public static readonly DependencyProperty IsTextEditableProperty;
		public static readonly DependencyProperty AllowDefaultButtonProperty;
		public static readonly DependencyProperty NullValueButtonPlacementProperty;
		public static readonly DependencyProperty ButtonsSourceProperty;
		public static readonly DependencyProperty ButtonTemplateProperty;
		public static readonly DependencyProperty ButtonTemplateSelectorProperty;
		static ButtonEditSettings() {
			Type ownerType = typeof(ButtonEditSettings);
			IsTextEditableProperty = DependencyPropertyManager.Register("IsTextEditable", typeof(bool?), ownerType, new PropertyMetadata(null, OnSettingsPropertyChanged));
			AllowDefaultButtonProperty = DependencyPropertyManager.Register("AllowDefaultButton", typeof(bool?), ownerType, new PropertyMetadata(null, OnSettingsPropertyChanged));
			NullValueButtonPlacementProperty = DependencyPropertyManager.Register("NullValueButtonPlacement", typeof(EditorPlacement?), ownerType, new PropertyMetadata(OnSettingsPropertyChanged));
			ButtonsSourceProperty = DependencyPropertyManager.Register("ButtonsSource", typeof(IEnumerable), ownerType, new PropertyMetadata(OnButtonsChanged));
			ButtonTemplateProperty = DependencyPropertyManager.Register("ButtonTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(OnButtonsChanged));
			ButtonTemplateSelectorProperty = DependencyPropertyManager.Register("ButtonTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnButtonsChanged));
		}
		#endregion
		[Category(EditSettingsCategories.Appearance)]
		public EditorPlacement? NullValueButtonPlacement {
			get { return (EditorPlacement?)GetValue(NullValueButtonPlacementProperty); }
			set { SetValue(NullValueButtonPlacementProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public bool? IsTextEditable {
			get { return (bool?)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditSettingsAllowDefaultButton"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public bool? AllowDefaultButton {
			get { return (bool?)GetValue(AllowDefaultButtonProperty); }
			set { SetValue(AllowDefaultButtonProperty, value); }
		}
		public IEnumerable ButtonsSource {
			get { return (IEnumerable)GetValue(ButtonsSourceProperty); }
			set { SetValue(ButtonsSourceProperty, value); }
		}
		public DataTemplate ButtonTemplate {
			get { return (DataTemplate)GetValue(ButtonTemplateProperty); }
			set { SetValue(ButtonTemplateProperty, value); }
		}
		public DataTemplateSelector ButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ButtonTemplateSelectorProperty); }
			set { SetValue(ButtonTemplateSelectorProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			var inplaceBaseEdit = edit as IInplaceBaseEdit;
			if (inplaceBaseEdit != null) {
				inplaceBaseEdit.AllowDefaultButton = AllowDefaultButton ?? true;
				return;
			}
			ButtonEdit be = edit as ButtonEdit;
			if (be == null)
				return;
			SetValueFromSettings(IsTextEditableProperty, () => be.IsTextEditable = IsTextEditable);
			SetValueFromSettings(AllowDefaultButtonProperty, () => be.AllowDefaultButton = AllowDefaultButton);
			SetValueFromSettings(NullValueButtonPlacementProperty, () => be.NullValueButtonPlacement = NullValueButtonPlacement);
			SetValueFromSettings(ButtonsSourceProperty,
				() => be.ButtonsSource = ButtonsSource,
				() => ClearEditorPropertyIfNeeded(be, ButtonEdit.ButtonsSourceProperty, ButtonsSourceProperty));
			SetValueFromSettings(ButtonTemplateProperty,
				() => be.ButtonTemplate = ButtonTemplate,
				() => ClearEditorPropertyIfNeeded(be, ButtonEdit.ButtonTemplateProperty, ButtonTemplateProperty));
			SetValueFromSettings(ButtonTemplateSelectorProperty,
				() => be.ButtonTemplateSelector = ButtonTemplateSelector,
				() => ClearEditorPropertyIfNeeded(be, ButtonEdit.ButtonTemplateSelectorProperty, ButtonTemplateSelectorProperty));
#if SL
			if(ButtonStyle != null)
				be.ButtonStyle = ButtonStyle;
#endif
			if (!Equals(be.Buttons, Buttons)) {
				be.Buttons.Clear();
				foreach (ButtonInfoBase info in Buttons) {
					be.Buttons.Add((ButtonInfoBase)((ICloneable)info).Clone());
				}
			}
		}
		ButtonInfoCollection buttons;
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditSettingsButtons"),
#endif
 Category(EditSettingsCategories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SkipPropertyAssertion]
		public ButtonInfoCollection Buttons {
			get {
				if (buttons == null) {
					buttons = new ButtonInfoCollection();
					buttons.CollectionChanged += ButtonsCollectionChanged;
				}
				return buttons;
			}
		}
		IEnumerable<ButtonInfoBase> buttonsSourceButtons;
		IEnumerable<ButtonInfoBase> ButtonsSourceButtons {
			get {
				if (buttonsSourceButtons == null)
					buttonsSourceButtons = CreateButtonsSourceButtons(ButtonsSource, ButtonTemplate, ButtonTemplateSelector);
				return buttonsSourceButtons;
			}
		}
		static void OnButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			OnSettingsPropertyChanged(d, e);
			((ButtonEditSettings)d).ClearButtonsSourceButtons();
		}
		static readonly object DefaultButtonClickKey = new object();
		public event RoutedEventHandler DefaultButtonClick {
			add { AddHandler(DefaultButtonClickKey, value); }
			remove { RemoveHandler(DefaultButtonClickKey, value); }
		}
		protected internal virtual void RaiseDefaultButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			Delegate handler;
			if (Events.TryGetValue(DefaultButtonClickKey, out handler))
				((RoutedEventHandler)handler)(sender, e);
		}
		protected override IEnumerator LogicalChildren {
			get {
				List<object> children = new List<object>();
				if (base.LogicalChildren != null) {
					IEnumerator enumerator = base.LogicalChildren;
					while (enumerator.MoveNext())
						children.Add(enumerator.Current);
				}
				if (Buttons != null)
					foreach (ButtonInfoBase info in Buttons)
						if (info != null)
							children.Add(info);
				return children.GetEnumerator();
			}
		}
		void ButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseChangedEventIfNotLoading();
			if (e.NewItems != null)
				foreach (ButtonInfoBase info in e.NewItems)
					if (info != null) {
						AddLogicalChild(info);
					}
			if (e.OldItems != null)
				foreach (ButtonInfoBase info in e.OldItems) {
					if (info != null)
						RemoveLogicalChild(info);
				}
		}
		protected internal virtual ButtonInfoBase CreateDefaultButtonInfo() {
			return new ButtonInfo() { GlyphKind = GlyphKind.Regular, IsDefaultButton = true };
		}
		protected internal virtual ButtonInfoBase CreateNullValueButtonInfo() {
			return new ButtonInfo() {
				GlyphKind = GlyphKind.Cancel,
				Content = EditorLocalizer.Active.GetLocalizedString(EditorStringId.SetNullValue),
			};
		}
		protected internal override bool IsPasteGesture(Key key, ModifierKeys modifiers) {
			return IsTextEditable.GetValueOrDefault(true) && base.IsPasteGesture(key, modifiers);
		}
		protected internal virtual IEnumerable<ButtonInfoBase> GetActualButtons() {
			List<ButtonInfoBase> buttons = new List<ButtonInfoBase>(Buttons);
			buttons.AddRange(ButtonsSourceButtons);
			return buttons;
		}
		protected internal List<ButtonInfoBase> CreateButtonsSourceButtons(IEnumerable buttonsSource, DataTemplate template, DataTemplateSelector templateSelector) {
			List<ButtonInfoBase> buttons = new List<ButtonInfoBase>();
			if (buttonsSource != null) {
				foreach (var item in buttonsSource) {
					DataTemplate buttonTemplate = GetActualteButtonTemplate(item, template, templateSelector);
					ButtonInfoBase info = GetButtonInfoFromTemplate(buttonTemplate).Return(x => x, () => new ButtonInfo());
					info.DataContext = item;
					buttons.Add(info);
				}
			}
			return buttons;
		}
		void ClearButtonsSourceButtons() {
			buttonsSourceButtons = null;
		}
		ButtonInfoBase GetButtonInfoFromTemplate(DataTemplate template) {
			return template != null ? TemplateHelper.LoadFromTemplate<ButtonInfoBase>(template) : null;
		}
		DataTemplate GetActualteButtonTemplate(object item, DataTemplate template, DataTemplateSelector templateSelector) {
			if (templateSelector != null)
				return templateSelector.SelectTemplate(item, this);
			return template;
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class ButtonSettingsExtension : TextSettingsExtension {
		public ButtonSettingsExtension() {
			IsTextEditable = (bool?)ButtonEditSettings.IsTextEditableProperty.DefaultMetadata.DefaultValue;
			AllowDefaultButton = (bool?)ButtonEditSettings.AllowDefaultButtonProperty.DefaultMetadata.DefaultValue;
			NullValueButtonPlacement = (EditorPlacement?)ButtonEditSettings.NullValueButtonPlacementProperty.DefaultMetadata.DefaultValue;
		}
		protected sealed override TextEditSettings CreateTextEditSettings() {
			ButtonEditSettings be = CreateButtonEditSettings();
			be.IsTextEditable = GetIsTextEditable();
			be.AllowDefaultButton = AllowDefaultButton;
			be.NullValueButtonPlacement = NullValueButtonPlacement;
			return be;
		}
		protected virtual ButtonEditSettings CreateButtonEditSettings() {
			ButtonEditSettings settings = new ButtonEditSettings();
			return settings;
		}
		protected virtual bool? GetIsTextEditable() {
			return IsTextEditable;
		}
		public bool? IsTextEditable { get; set; }
		public bool? AllowDefaultButton { get; set; }
		public EditorPlacement? NullValueButtonPlacement { get; set; }
	}
	public class ButtonEditButtonInfoExtension : MarkupExtension {
		public ClickMode ClickMode { get; set; }
		public object CommandParameter { get; set; }
		public ICommand Command { get; set; }
		public IInputElement CommandTarget { get; set; }
		public GlyphKind ButtonKind { get; set; }
		public bool IsDefaultButton { get; set; }
		public object Content { get; set; }
		public Style Style { get; set; }
		public ControlTemplate Template { get; set; }
		public DataTemplate ContentTemplate { get; set; }
		public ButtonEditButtonInfoExtension() {
			ClickMode = (ClickMode)ButtonBase.ClickModeProperty.DefaultMetadata.DefaultValue;
			CommandParameter = ButtonInfo.CommandParameterProperty.DefaultMetadata.DefaultValue;
			Command = (ICommand)ButtonInfo.CommandProperty.DefaultMetadata.DefaultValue;
			CommandTarget = (IInputElement)ButtonInfo.CommandTargetProperty.DefaultMetadata.DefaultValue;
			ButtonKind = (GlyphKind)ButtonInfo.GlyphKindProperty.DefaultMetadata.DefaultValue;
			IsDefaultButton = (bool)ButtonInfo.IsDefaultButtonProperty.DefaultMetadata.DefaultValue;
			Content = ButtonInfo.ContentProperty.DefaultMetadata.DefaultValue;
			Template = (ControlTemplate)ButtonInfo.TemplateProperty.DefaultMetadata.DefaultValue;
			ContentTemplate = (DataTemplate)ButtonInfo.ContentTemplateProperty.DefaultMetadata.DefaultValue;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			ButtonInfo info = new ButtonInfo();
			info.ClickMode = ClickMode;
			info.CommandParameter = CommandParameter;
			info.Command = Command;
			info.CommandTarget = CommandTarget;
			info.IsDefaultButton = IsDefaultButton;
			info.Content = Content;
			info.ContentTemplate = ContentTemplate;
			return info;
		}
	}
}
#endif
