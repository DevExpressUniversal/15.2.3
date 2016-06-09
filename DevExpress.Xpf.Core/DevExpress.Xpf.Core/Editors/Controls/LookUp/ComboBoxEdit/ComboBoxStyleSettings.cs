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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core.Native;
#if !SL
using DevExpress.Xpf.Editors.Themes;
#else
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class BaseItemsControlStyleSettings<T> : PopupBaseEditStyleSettings {
		protected internal abstract SelectionEventMode GetSelectionEventMode(ISelectorEdit ce);
		protected internal abstract Style GetItemContainerStyle(T control);
		protected internal virtual bool ShowCustomItem(T editor) {
			return false;
		}
		protected internal virtual bool ShowMRUItem() {
			return false;
		}
		protected internal virtual IEnumerable<CustomItem> GetCustomItems(T editor) {
			return new List<CustomItem>();
		}
	}
	public abstract class BaseLookUpStyleSettings : BaseItemsControlStyleSettings<LookUpEditBase>, ISelectorEditStyleSettings, ITokenStyleSettings {
		protected internal virtual bool? ScrollToSelectionOnPopup { get; set; }
		internal bool GetActualScrollToSelectionOnPopup(LookUpEditBase editor) {
			return ScrollToSelectionOnPopup ?? GetSelectionMode(editor) == SelectionMode.Single;
		}
		protected internal virtual bool GetClosePopupOnMouseUp(LookUpEditBase editor) {
			return editor.PropertyProvider.GetPopupFooterButtons() != PopupFooterButtons.OkCancel;
		}
		protected internal virtual bool CloseUsingDispatcher { get { return false; }}
		protected internal virtual FilterCondition FilterCondition { get { return FilterCondition.StartsWith; } }
		protected internal virtual bool ProcessContentSelectionChanged(FrameworkElement sender, SelectionChangedEventArgs e) { return true; }
		protected internal abstract SelectionMode GetSelectionMode(LookUpEditBase editor);
		#region ISelectorEditStyleSettings Members
		Style ISelectorEditStyleSettings.GetItemContainerStyle(ISelectorEdit editor) {
			return GetItemContainerStyle((LookUpEditBase)editor);
		}
		#endregion
		protected internal virtual EditorPlacement GetFindButtonPlacement(LookUpEditBase editor) {
			return EditorPlacement.None;
		}
		protected internal virtual EditorPlacement GetAddNewButtonPlacement(LookUpEditBase editor) {
			return EditorPlacement.None;
		}
		protected internal virtual EditorPlacement GetNullValueButtonPlacement(LookUpEditBase editor) {
			return EditorPlacement.None;
		}
		protected internal virtual FindMode GetFindMode(LookUpEditBase editor) {
			return FindMode.Always;
		}
		protected internal virtual FilterByColumnsMode FilterByColumnsMode {
			get { return FilterByColumnsMode.Default; }
		}
		public virtual bool IsTokenStyleSettings() {
			return false;
		}
		protected internal virtual bool GetShowTokenButtons() {
			return ((ITokenStyleSettings)this).ShowTokenButtons ?? true;
		}
		protected internal virtual ButtonInfoCollection GetTokenButtons() {
			return null;
		}
		protected internal virtual bool GetEnableTokenWrapping() {
			return ((ITokenStyleSettings)this).EnableTokenWrapping ?? false;
		}
		protected internal virtual ControlTemplate GetTokenBorderTemplate() {
			return ((ITokenStyleSettings)this).TokenBorderTemplate ?? null;
		}
		protected internal virtual NewTokenPosition GetNewTokenPosition() {
			return ((ITokenStyleSettings)this).NewTokenPosition ?? NewTokenPosition.Near;
		}
		protected internal virtual TextTrimming GetTokenTextTrimming() {
			return ((ITokenStyleSettings)this).TokenTextTrimming ?? TextTrimming.None;
		}
		protected internal virtual double GetTokenMaxWidth() {
			return ((ITokenStyleSettings)this).TokenMaxWidth ?? 0d;
		}
		protected internal virtual bool GetAllowEditTokens() {
			return ((ITokenStyleSettings)this).AllowEditTokens ?? true;
		}
		protected internal virtual bool GetIncrementalFiltering() { return false; }
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			var lookup = (LookUpEditBase)editor;
			lookup.IsTokenMode = IsTokenStyleSettings();
			var tokenEditor = lookup.EditCore as TokenEditor;
			if (tokenEditor != null) {
				tokenEditor.ShowTokenButtons = GetShowTokenButtons();
				tokenEditor.EnableTokenWrapping = GetEnableTokenWrapping();
				tokenEditor.TokenBorderTemplate = GetTokenBorderTemplate();
				tokenEditor.NewTokenPosition = GetNewTokenPosition();
				tokenEditor.TokenTextTrimming = GetTokenTextTrimming();
				tokenEditor.TokenMaxWidth = GetTokenMaxWidth();
				tokenEditor.AllowEditTokens = GetAllowEditTokens();
			}
		}
		protected virtual object GetPropertyValue(DependencyProperty property) {
			return !IsDefaultValue(property) ? GetValue(property) : null;
		}
		protected virtual bool IsDefaultValue(DependencyProperty property) {
			object defaultValue = property.GetMetadata(GetType()).DefaultValue;
			defaultValue = defaultValue == DependencyProperty.UnsetValue ? null : defaultValue;
			return GetValue(property) == defaultValue;
		}
		bool? ITokenStyleSettings.AllowEditTokens {
			get { return null; }
		}
		bool? ITokenStyleSettings.EnableTokenWrapping {
			get { return null; }
		}
		bool? ITokenStyleSettings.AddTokenOnPopupSelection {
			get { return null; }
		}
		ControlTemplate ITokenStyleSettings.TokenBorderTemplate {
			get { return null; }
		}
		bool? ITokenStyleSettings.ShowTokenButtons {
			get { return null; }
		}
		NewTokenPosition? ITokenStyleSettings.NewTokenPosition {
			get { return null; }
		}
		TextTrimming? ITokenStyleSettings.TokenTextTrimming {
			get { return null; }
		}
		double? ITokenStyleSettings.TokenMaxWidth {
			get { return null; }
		}
		public override bool GetIsTextEditable(ButtonEdit editor) {
			if (IsTokenStyleSettings())
				return base.GetIsTextEditable(editor);
			ISelectorEditStrategy editStrategy = (ISelectorEditStrategy)editor.EditStrategy;
			return editStrategy.IsSingleSelection;
		}
	}
	public abstract class BaseComboBoxStyleSettings : BaseLookUpStyleSettings {
		protected internal override bool ShowCustomItem(LookUpEditBase editor) {
			ComboBoxEdit comboBox = (ComboBoxEdit)editor;
			return comboBox.ShowCustomItems ?? ShowCustomItemInternal(editor);
		}
		protected virtual bool ShowCustomItemInternal(LookUpEditBase editor) {
			return false;
		}
		protected internal override bool GetIncrementalFiltering() {
			return IsTokenStyleSettings();
		}
	}
	public interface ITokenStyleSettings {
		bool IsTokenStyleSettings();
		bool? EnableTokenWrapping { get; }
		bool? AllowEditTokens { get; }
		[Obsolete]
		bool? AddTokenOnPopupSelection { get; }
		ControlTemplate TokenBorderTemplate { get; }
		bool? ShowTokenButtons { get; }
		NewTokenPosition? NewTokenPosition { get; }
		TextTrimming? TokenTextTrimming { get; }
		double? TokenMaxWidth { get; }
	}
	public class TokenComboBoxStyleSettings : ComboBoxStyleSettings, ITokenStyleSettings {
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenBorderTemplateProperty;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty NewTokenPositionProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty TokenMaxWidthProperty;
		public static readonly DependencyProperty AddTokenOnPopupSelectionProperty;
		public static readonly DependencyProperty AllowEditTokensProperty;
		static TokenComboBoxStyleSettings() {
			Type ownerType = typeof(TokenComboBoxStyleSettings);
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool?), ownerType);
			TokenBorderTemplateProperty = DependencyProperty.Register("TokenBorderTemplate", typeof(ControlTemplate), ownerType);
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool?), ownerType, new FrameworkPropertyMetadata(true));
			NewTokenPositionProperty = DependencyProperty.Register("NewTokenPosition", typeof(NewTokenPosition?), ownerType, new FrameworkPropertyMetadata(null));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming?), ownerType, new FrameworkPropertyMetadata(null));
			TokenMaxWidthProperty = DependencyProperty.Register("TokenMaxWidth", typeof(double?), ownerType, new FrameworkPropertyMetadata(null));
			AddTokenOnPopupSelectionProperty = DependencyProperty.Register("AddTokenOnPopupSelection", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			AllowEditTokensProperty = DependencyProperty.Register("AllowEditTokens", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool? AllowEditTokens {
			get { return (bool?)GetValue(AllowEditTokensProperty); }
			set { SetValue(AllowEditTokensProperty, value); }
		}
		 [Obsolete]
		public bool? AddTokenOnPopupSelection {
			get { return (bool?)GetValue(AddTokenOnPopupSelectionProperty); }
			set { SetValue(AddTokenOnPopupSelectionProperty, value); }
		}
		public double? TokenMaxWidth{
			get { return (double?)GetValue(TokenMaxWidthProperty); }
			set { SetValue(TokenMaxWidthProperty, value); }
		}
		public TextTrimming? TokenTextTrimming {
			get { return (TextTrimming?)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public NewTokenPosition? NewTokenPosition {
			get { return (NewTokenPosition?)GetValue(NewTokenPositionProperty); }
			set { SetValue(NewTokenPositionProperty, value); }
		}
		public bool? EnableTokenWrapping {
			get { return (bool?)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public ControlTemplate TokenBorderTemplate {
			get { return (ControlTemplate)GetValue(TokenBorderTemplateProperty); }
			set { SetValue(TokenBorderTemplateProperty, value); }
		}
		public bool? ShowTokenButtons {
			get { return (bool?)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		protected internal override bool GetActualAllowDefaultButton(ButtonEdit editor) {
			LookUpEditBasePropertyProvider btn = (LookUpEditBasePropertyProvider)editor.PropertyProvider;
			return !btn.EnableTokenWrapping;
		}
		public override bool IsTokenStyleSettings() {
			return true;
		}
	}
	public class CheckedTokenComboBoxStyleSettings : CheckedComboBoxStyleSettings, ITokenStyleSettings {
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenBorderTemplateProperty;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty NewTokenPositionProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty TokenMaxWidthProperty;
		public static readonly DependencyProperty AddTokenOnPopupSelectionProperty;
		public static readonly DependencyProperty AllowEditTokensProperty;
		static CheckedTokenComboBoxStyleSettings() {
			Type ownerType = typeof(CheckedTokenComboBoxStyleSettings);
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool?), ownerType);
			TokenBorderTemplateProperty = DependencyProperty.Register("TokenBorderTemplate", typeof(ControlTemplate), ownerType);
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool?), ownerType, new FrameworkPropertyMetadata(true));
			NewTokenPositionProperty = DependencyProperty.Register("NewTokenPosition", typeof(NewTokenPosition?), ownerType, new FrameworkPropertyMetadata(null));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming?), ownerType, new FrameworkPropertyMetadata(null));
			TokenMaxWidthProperty = DependencyProperty.Register("TokenMaxWidth", typeof(double?), ownerType, new FrameworkPropertyMetadata(null));
			AddTokenOnPopupSelectionProperty = DependencyProperty.Register("AddTokenOnPopupSelection", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			AllowEditTokensProperty = DependencyProperty.Register("AllowEditTokens", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool? AllowEditTokens {
			get { return (bool?)GetValue(AllowEditTokensProperty); }
			set { SetValue(AllowEditTokensProperty, value); }
		}
		[Obsolete]
		public bool? AddTokenOnPopupSelection {
			get { return (bool?)GetValue(AddTokenOnPopupSelectionProperty); }
			set { SetValue(AddTokenOnPopupSelectionProperty, value); }
		}
		public double? TokenMaxWidth{
			get { return (double?)GetValue(TokenMaxWidthProperty); }
			set { SetValue(TokenMaxWidthProperty, value); }
		}
		public TextTrimming? TokenTextTrimming {
			get { return (TextTrimming?)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public NewTokenPosition? NewTokenPosition {
			get { return (NewTokenPosition?)GetValue(NewTokenPositionProperty); }
			set { SetValue(NewTokenPositionProperty, value); }
		}
		public bool? EnableTokenWrapping {
			get { return (bool?)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public ControlTemplate TokenBorderTemplate {
			get { return (ControlTemplate)GetValue(TokenBorderTemplateProperty); }
			set { SetValue(TokenBorderTemplateProperty, value); }
		}
		public bool? ShowTokenButtons {
			get { return (bool?)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		public override bool IsTokenStyleSettings() {
			return true;
		}
		protected internal override bool GetActualAllowDefaultButton(ButtonEdit editor) {
			LookUpEditBasePropertyProvider btn = (LookUpEditBasePropertyProvider)editor.PropertyProvider;
			return !btn.EnableTokenWrapping;
		}
		protected override bool ShowCustomItemInternal(LookUpEditBase editor) {
			return false;
		}
	}
	public class RadioTokenComboBoxStyleSettings : RadioComboBoxStyleSettings, ITokenStyleSettings {
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenBorderTemplateProperty;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty NewTokenPositionProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty TokenMaxWidthProperty;
		public static readonly DependencyProperty AddTokenOnPopupSelectionProperty;
		public static readonly DependencyProperty AllowEditTokensProperty;
		static RadioTokenComboBoxStyleSettings() {
			Type ownerType = typeof(RadioTokenComboBoxStyleSettings);
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool?), ownerType);
			TokenBorderTemplateProperty = DependencyProperty.Register("TokenBorderTemplate", typeof(ControlTemplate), ownerType);
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool?), ownerType, new FrameworkPropertyMetadata(true));
			NewTokenPositionProperty = DependencyProperty.Register("NewTokenPosition", typeof(NewTokenPosition?), ownerType, new FrameworkPropertyMetadata(null));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming?), ownerType, new FrameworkPropertyMetadata(null));
			TokenMaxWidthProperty = DependencyProperty.Register("TokenMaxWidth", typeof(double?), ownerType, new FrameworkPropertyMetadata(null));
			AddTokenOnPopupSelectionProperty = DependencyProperty.Register("AddTokenOnPopupSelection", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			AllowEditTokensProperty = DependencyProperty.Register("AllowEditTokens", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool? AllowEditTokens {
			get { return (bool?)GetValue(AllowEditTokensProperty); }
			set { SetValue(AllowEditTokensProperty, value); }
		}
		[Obsolete]
		public bool? AddTokenOnPopupSelection {
			get { return (bool?)GetValue(AddTokenOnPopupSelectionProperty); }
			set { SetValue(AddTokenOnPopupSelectionProperty, value); }
		}
		public double? TokenMaxWidth{
			get { return (double?)GetValue(TokenMaxWidthProperty); }
			set { SetValue(TokenMaxWidthProperty, value); }
		}
		public TextTrimming? TokenTextTrimming {
			get { return (TextTrimming?)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public NewTokenPosition? NewTokenPosition {
			get { return (NewTokenPosition?)GetValue(NewTokenPositionProperty); }
			set { SetValue(NewTokenPositionProperty, value); }
		}
		public bool? EnableTokenWrapping {
			get { return (bool?)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public ControlTemplate TokenBorderTemplate {
			get { return (ControlTemplate)GetValue(TokenBorderTemplateProperty); }
			set { SetValue(TokenBorderTemplateProperty, value); }
		}
		public bool? ShowTokenButtons {
			get { return (bool?)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		public override bool IsTokenStyleSettings() {
			return true;
		}
		protected internal override bool GetActualAllowDefaultButton(ButtonEdit editor) {
			LookUpEditBasePropertyProvider btn = (LookUpEditBasePropertyProvider)editor.PropertyProvider;
			return !btn.EnableTokenWrapping;
		}
	}
	public class ComboBoxStyleSettings : BaseComboBoxStyleSettings {
		protected internal override Style GetItemContainerStyle(LookUpEditBase cb) {
#if !SL
			if(cb.ItemContainerStyle == null)
				return (Style)cb.FindResource(new EditorListBoxThemeKeyExtension() { ResourceKey = EditorListBoxThemeKeys.DefaultItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(cb) });
#endif
			return cb.ItemContainerStyle;
		}
		protected internal override SelectionMode GetSelectionMode(LookUpEditBase editor) {
			return SelectionMode.Single;
		}
		protected internal override SelectionEventMode GetSelectionEventMode(ISelectorEdit ce) {
			if(!((LookUpEditBase)ce).AllowItemHighlighting)
				return SelectionEventMode.MouseDown;
			return SelectionEventMode.MouseEnter;
		}
		protected override bool ShowCustomItemInternal(LookUpEditBase editor) {
			return false;
		}
		protected internal override IEnumerable<CustomItem> GetCustomItems(LookUpEditBase editor) {
			return ((LookUpEditBasePropertyProvider)editor.PropertyProvider).IsSingleSelection ? new List<CustomItem>{ new EmptyItem() } : new List<CustomItem>();
		}
		protected internal override bool GetShowSizeGrip(PopupBaseEdit editor) {
			return false;
		}
	}
	public class SearchControlStyleSettings : ComboBoxStyleSettings {
		public override bool ShouldCaptureMouseOnPopup { get { return false; } }
	}
	public partial class CheckedComboBoxStyleSettings : BaseComboBoxStyleSettings {
		protected internal override Style GetItemContainerStyle(LookUpEditBase cb) {
#if !SL
			return (Style)cb.FindResource(new EditorListBoxThemeKeyExtension() { ResourceKey = EditorListBoxThemeKeys.CheckBoxItemStyle, ThemeName=ThemeHelper.GetEditorThemeName(cb) });
#else
			return cb.CheckItemContainerStyle;
#endif
		}
		protected internal override SelectionMode GetSelectionMode(LookUpEditBase editor) {
			return SelectionMode.Multiple;
		}
		protected internal override bool ShouldFocusPopup {
			get { return true; }
		}
		protected internal override bool GetClosePopupOnMouseUp(LookUpEditBase editor) {
			return false;
		}
		protected internal override SelectionEventMode GetSelectionEventMode(ISelectorEdit ce) {
			return SelectionEventMode.MouseUp;
		}
		protected internal override IEnumerable<CustomItem> GetCustomItems(LookUpEditBase editor) {
			return new List<CustomItem>{ new SelectAllItem() };
		}
		protected override bool ShowCustomItemInternal(LookUpEditBase editor) {
			return true;
		}
		public override PopupFooterButtons GetPopupFooterButtons(PopupBaseEdit editor) {
			return PopupFooterButtons.OkCancel;
		}
		protected internal override bool GetShowSizeGrip(PopupBaseEdit editor) {
			return true;
		}
	}
	public class RadioComboBoxStyleSettings : BaseComboBoxStyleSettings {
		protected internal override Style GetItemContainerStyle(LookUpEditBase cb) {
#if !SL
			return (Style)cb.FindResource(new EditorListBoxThemeKeyExtension() { ResourceKey = EditorListBoxThemeKeys.RadioButtonItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(cb) });
#else
			return cb.RadioItemContainerStyle;
#endif
		}
		protected internal override SelectionMode GetSelectionMode(LookUpEditBase editor) {
			return SelectionMode.Single;
		}
		protected internal override SelectionEventMode GetSelectionEventMode(ISelectorEdit ce) {
			return SelectionEventMode.MouseUp;
		}
		protected internal override bool ShowCustomItem(LookUpEditBase editor) {
			return false;
		}
		public override PopupFooterButtons GetPopupFooterButtons(PopupBaseEdit editor) {
			return PopupFooterButtons.OkCancel;
		}
		protected internal override bool GetShowSizeGrip(PopupBaseEdit editor) {
			return true;
		}
	}
	public abstract class BaseComboBoxStyleSettingsExtension : MarkupExtension {
		bool? scrollToSelectionOnPopup;
		protected BaseComboBoxStyleSettingsExtension() { }
		protected BaseComboBoxStyleSettingsExtension(bool? scrollToSelectionOnPopup) {
			this.scrollToSelectionOnPopup = scrollToSelectionOnPopup;
		}
		public sealed override object ProvideValue(IServiceProvider serviceProvider) {
			BaseComboBoxStyleSettings settings = ProvideValueCore(serviceProvider);
			settings.ScrollToSelectionOnPopup = ScrollToSelectionOnPopup;
			return settings;
		}
		protected abstract BaseComboBoxStyleSettings ProvideValueCore(IServiceProvider serviceProvider);
		[DefaultValue(null), ConstructorArgument("scrollToSelectionOnPopup")]
		public bool? ScrollToSelectionOnPopup {
			get { return scrollToSelectionOnPopup; }
			set { scrollToSelectionOnPopup = value; }
		}
	}
	public class ComboBoxStyleSettingsExtension : BaseComboBoxStyleSettingsExtension {
		public ComboBoxStyleSettingsExtension() { }
		public ComboBoxStyleSettingsExtension(bool? scrollToSelectionOnPopup)
			: base(scrollToSelectionOnPopup) { }
		protected override BaseComboBoxStyleSettings ProvideValueCore(IServiceProvider serviceProvider) {
			return new ComboBoxStyleSettings();
		}
	}
	public class CheckedComboBoxStyleSettingsExtension : BaseComboBoxStyleSettingsExtension {
		public CheckedComboBoxStyleSettingsExtension() { }
		public CheckedComboBoxStyleSettingsExtension(bool? scrollToSelectionOnPopup)
			: base(scrollToSelectionOnPopup) { }
		protected override BaseComboBoxStyleSettings ProvideValueCore(IServiceProvider serviceProvider) {
			return new CheckedComboBoxStyleSettings();
		}
	}
	public class RadioComboBoxStyleSettingsExtension : BaseComboBoxStyleSettingsExtension {
		public RadioComboBoxStyleSettingsExtension() { }
		public RadioComboBoxStyleSettingsExtension(bool? scrollToSelectionOnPopup)
			: base(scrollToSelectionOnPopup) { }
		protected override BaseComboBoxStyleSettings ProvideValueCore(IServiceProvider serviceProvider) {
			return new RadioComboBoxStyleSettings();
		}
	}
}
