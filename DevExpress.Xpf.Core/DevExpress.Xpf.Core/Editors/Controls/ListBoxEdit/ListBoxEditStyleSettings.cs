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
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
#else
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Editors.Helpers;
#endif
namespace DevExpress.Xpf.Editors {
	public class ListBoxEditStyleSettings : BaseItemsControlStyleSettings<ListBoxEdit>, ISelectorEditStyleSettings {
		public static readonly DependencyProperty SelectionEventModeProperty;
		static ListBoxEditStyleSettings() {
			SelectionEventModeProperty = DependencyProperty.Register("SelectionEventMode", typeof(SelectionEventMode), typeof(ListBoxEditStyleSettings), new FrameworkPropertyMetadata(SelectionEventMode.MouseDown));
		}
		public SelectionEventMode SelectionEventMode {
			get { return (SelectionEventMode)GetValue(SelectionEventModeProperty); }
			set { SetValue(SelectionEventModeProperty, value); }
		}
		protected virtual SelectionMode GetSelectionMode(ListBoxEdit editor) {
			return editor.SelectionMode;
		}
		protected internal override Style GetItemContainerStyle(ListBoxEdit editor) {
#if !SL
			if(editor.ItemContainerStyle == null)
				return (Style)editor.FindResource(new EditorListBoxThemeKeyExtension() { ResourceKey = EditorListBoxThemeKeys.DefaultItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(editor) });
#endif
			return editor.ItemContainerStyle;
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			ListBoxEdit listBoxEditor = editor as ListBoxEdit;
			if(listBoxEditor == null)
				return;
			listBoxEditor.SelectionMode = GetSelectionMode(listBoxEditor);
			listBoxEditor.Settings.SelectionMode = GetSelectionMode(listBoxEditor);
			if(listBoxEditor.ListBoxCore == null)
				return;
			listBoxEditor.ListBoxCore.ItemContainerStyle = GetItemContainerStyle(listBoxEditor);
		}
		protected internal override bool ShowCustomItem(ListBoxEdit editor) {
			return editor.ShowCustomItems ?? ShowCustomItemInternal(editor);
		}
		protected virtual bool ShowCustomItemInternal(ListBoxEdit editor) {
			return false;
		}
		protected internal override IEnumerable<CustomItem> GetCustomItems(ListBoxEdit editor) {
			List<CustomItem> items = new List<CustomItem>(base.GetCustomItems(editor));
			if (GetSelectionMode(editor) == SelectionMode.Single)
				items.Add(new EmptyItem());
			return items;
		}
		protected internal override SelectionEventMode GetSelectionEventMode(ISelectorEdit ce) {
			return SelectionEventMode;
		}
		#region ISelectorEditStyleSettings Members
		Style ISelectorEditStyleSettings.GetItemContainerStyle(ISelectorEdit editor) {
			return GetItemContainerStyle((ListBoxEdit)editor);
		}
		#endregion
	}
	public class CheckedListBoxEditStyleSettings : ListBoxEditStyleSettings {
		protected internal override Style GetItemContainerStyle(ListBoxEdit control) {
#if !SL
			return (Style)control.FindResource(new EditorListBoxThemeKeyExtension() { ResourceKey = EditorListBoxThemeKeys.CheckBoxItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(control) });
#else 
			return control.CheckItemContainerStyle;
#endif
		}
		protected override bool ShowCustomItemInternal(ListBoxEdit editor) {
			return true;
		}
		protected override SelectionMode GetSelectionMode(ListBoxEdit editor) {
			return SelectionMode.Multiple;
		}
		protected internal override IEnumerable<CustomItem> GetCustomItems(ListBoxEdit editor) {
			List<CustomItem> items = new List<CustomItem>();
			items.Add(new SelectAllItem());
			return items;
		}
	}
	public class RadioListBoxEditStyleSettings : ListBoxEditStyleSettings {
		protected internal override Style GetItemContainerStyle(ListBoxEdit control) {
#if !SL
			return (Style)control.FindResource(new EditorListBoxThemeKeyExtension() { ResourceKey = EditorListBoxThemeKeys.RadioButtonItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(control) });
#else 
			return control.RadioItemContainerStyle;
#endif
		}
		protected override SelectionMode GetSelectionMode(ListBoxEdit editor) {
			return SelectionMode.Single;
		}
		protected internal override IEnumerable<CustomItem> GetCustomItems(ListBoxEdit editor) {
			return new List<CustomItem>();
		}
	}
	#if !SL
	public class ListBoxEditStyleSettingsExtension : MarkupExtension {
		public ListBoxEditStyleSettingsExtension() { }
		public sealed override object ProvideValue(IServiceProvider serviceProvider) {
			return ProvideValueCore(serviceProvider);
		}
		protected virtual ListBoxEditStyleSettings ProvideValueCore(IServiceProvider serviceProvider) {
			return new ListBoxEditStyleSettings();
		}
	}
	public class CheckedListBoxStyleSettingsExtension : ListBoxEditStyleSettingsExtension {
		public CheckedListBoxStyleSettingsExtension() { }
		protected override ListBoxEditStyleSettings ProvideValueCore(IServiceProvider serviceProvider) {
			return new CheckedListBoxEditStyleSettings();
		}
	}
	public class RadioListBoxEditStyleSettingsExtension : ListBoxEditStyleSettingsExtension {
		public RadioListBoxEditStyleSettingsExtension() { }
		protected override ListBoxEditStyleSettings ProvideValueCore(IServiceProvider serviceProvider) {
			return new RadioListBoxEditStyleSettings();
		}
	}
	#endif
}
