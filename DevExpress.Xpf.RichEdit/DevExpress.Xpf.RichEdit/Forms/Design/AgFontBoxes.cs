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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Markup;
using System.Globalization;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.Native;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Drawing;
using DevExpress.Utils.Internal;
using DevExpress.Xpf.Office.UI;
#else
using DevExpress.Xpf.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Office.UI;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region AgFontListBox
	public class AgFontListBox : ListBox, IFontBasedElement {
		public AgFontListBox()
			: base() {
			Populate();
			DefaultStyleKey = typeof(ListBox);
			FontManager.RegisterFontBasedElement(this);
		}
		void Populate() {
			int index = SelectedIndex;
			Items.Clear();
#if!SL
			List<string> fontNames = new List<string>(FontManager.GetLocalizedFontNames());
#else
			List<string> fontNames = new List<string>(FontManager.GetFontFamilyNames());
#endif
			fontNames.Sort(StringComparer.CurrentCultureIgnoreCase);
			foreach (string familyName in fontNames) {
				TextBlock textBlock = new TextBlock() { Text = familyName, FontSize = 13 };
				FontComboBoxEditSettings.SetTextBlockFontFamilty(textBlock, familyName);
				Items.Add(textBlock);
			}
			SelectedIndex = index;
		}
		public string Text {
			get {
				return (SelectedItem == null) ? String.Empty : ((TextBlock)SelectedItem).FontFamily.Source;
			}
			set {
				foreach (TextBlock tb in Items) {
					if (String.Compare(tb.FontFamily.Source, value, StringComparison.CurrentCultureIgnoreCase) == 0) {
						SelectedItem = tb;
						EnsureSelectedItemVisible();
						break;
					}
				}
			}
		}
		void EnsureSelectedItemVisible() {
			Dispatcher.BeginInvoke((Action)EnsureSelectedItemVisibleCore);
		}
		void EnsureSelectedItemVisibleCore() {
			if (SelectedItem != null)
				ScrollIntoView(SelectedItem);
		}
		#region IFontBasedElement Members
		public void OnFontsChanged() {
			Populate();
		}
		#endregion
	}
	#endregion
#if SL
	#region FontComboBoxEditSettings
	public class FontComboBoxEditSettings : ComboBoxEditSettings, IFontBasedElement {
		public static readonly DependencyProperty ItemFontFamilyProperty = DependencyPropertyManager.RegisterAttached("ItemFontFamily", typeof(string), typeof(FontComboBoxEditSettings), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemFontFamilyChanged)));
		protected static void OnItemFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TextBlock textBlock = d as TextBlock;
			string fontFamily = e.NewValue as string;
			if (textBlock != null && !String.IsNullOrEmpty(fontFamily)) {
				SetTextBlockFontFamilty(textBlock, fontFamily);
			}
		}
		public static string GetItemFontFamily(DependencyObject d) { return (string)d.GetValue(ItemFontFamilyProperty); }
		public static void SetItemFontFamily(DependencyObject d, string value) { d.SetValue(ItemFontFamilyProperty, value); }
		static FontComboBoxEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(FontComboBoxEdit), typeof(FontComboBoxEditSettings), delegate() { return new FontComboBoxEdit(); }, delegate() { return new FontComboBoxEditSettings(); });
		}
		public FontComboBoxEditSettings() {
			FontManager.RegisterFontBasedElement(this);
			Populate();
		}
		void Populate() {
			Items.Clear();
#if!SL
			List<string> fontNames = new List<string>(FontManager.GetFontNames());
#else
			List<string> fontNames = new List<string>(FontManager.GetFontFamilyNames());
#endif
			fontNames.Sort(StringComparer.CurrentCultureIgnoreCase);
			foreach (string familyName in fontNames)
				Items.Add(familyName);
		}
		#region IFontBasedElement Members
		public void OnFontsChanged() {
			Populate();
		}
		#endregion
		internal static void SetTextBlockFontFamilty(TextBlock textBlock, string name) {
#if SL
			FontDescriptor fontDescriptor = FontManager.GetFontDescriptor(name, false, false);
			textBlock.FontFamily = fontDescriptor.FontFamily;
			textBlock.FontSource = fontDescriptor.FontSource;
#else
			textBlock.FontFamily = new FontFamily(name);
#endif
		}
	}
	#endregion
	#region FontComboBoxEdit
	[DXToolboxBrowsableAttribute(false)]
	public class FontComboBoxEdit : ComboBoxEdit {
		static FontComboBoxEdit() {
			FontComboBoxEditSettings.RegisterEditor();
		}
		public FontComboBoxEdit()
			: base() {
			DefaultStyleKey = typeof(FontComboBoxEdit);
		}
		protected override BaseEditSettings CreateEditorSettings() {
			return new FontComboBoxEditSettings();
		}
	}
	#endregion
#endif
}
