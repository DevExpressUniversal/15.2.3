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
#else
using DevExpress.Xpf.Utils;
using DevExpress.Office.Internal;
#endif
namespace DevExpress.Xpf.Office.UI {
#if !SL
	#region FontComboBoxEditItem
	public class FontComboBoxEditItem {
		public FontComboBoxEditItem(string editValue, string displayText, FontFamily fontFamily) {
			EditValue = editValue;
			DisplayText = displayText;
			FontFamily = fontFamily;
		}
		public string DisplayText { get; set; }
		public string EditValue { get; set; }
		public FontFamily FontFamily { get; set; }
		public override string ToString() {
			return DisplayText;
		}
	}
	#endregion
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
#if !SL
			DisplayMember = "DisplayText";
			ValueMember = "EditValue";
#endif
			Populate();
		}
		void Populate() {
			Items.Clear();
#if!SL
			List<FontComboBoxEditItem> items = new List<FontComboBoxEditItem>();
			foreach (string familyName in FontManager.GetLocalizedFontNames()) {
				FontFamily fontFamily = FontManager.GetFontFamily(familyName);
				items.Add(new FontComboBoxEditItem(fontFamily.Source, familyName, fontFamily));
			}
			items.Sort((x, y) => {
				return x.DisplayText.CompareTo(y.DisplayText);
			});
#else
			List<string> items = new List<string>();
			foreach (string familyName in FontManager.GetFontFamilyNames())
				items.Add(familyName);
			items.Sort(StringComparer.InvariantCultureIgnoreCase);
#endif
			Items.AddRange(items.ToArray());
		}
	#region IFontBasedElement Members
		public void OnFontsChanged() {
			Populate();
		}
	#endregion
		public static void SetTextBlockFontFamilty(TextBlock textBlock, string name) {
#if SL
			FontDescriptor fontDescriptor = FontManager.GetFontDescriptor(name, false, false);
			textBlock.FontFamily = fontDescriptor.FontFamily;
			textBlock.FontSource = fontDescriptor.FontSource;
#else
			textBlock.FontFamily = FontManager.GetFontFamily(name);
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
			ProcessNewValue += OnProcessNewValue;
		}
		protected internal override string GetDisplayText(object editValue, bool applyFormatting) {
			string result = base.GetDisplayText(editValue, applyFormatting);
			if (String.IsNullOrEmpty(result)) {
				string fontName = editValue as string;
				return String.IsNullOrEmpty(fontName) ? String.Empty : fontName;
			}
			else
				return result;
		}
		void OnProcessNewValue(DependencyObject sender, ProcessNewValueEventArgs e) {
			EditValue = e.DisplayText;
		}
		protected internal override BaseEditSettings CreateEditorSettings() {
			return new FontComboBoxEditSettings();
		}
	}
	#endregion
#endif
	#region FontSizeComboBoxEditSettings
	public interface IOfficeFontSizeProvider {
		List<int> GetPredefinedFontSizes();
	}
	public class FontSizeComboBoxEditSettings : ComboBoxEditSettings {
		public static readonly DependencyProperty OfficeFontSizeProviderProperty =
			DependencyPropertyManager.Register("OfficeFontSizeProvider", typeof(IOfficeFontSizeProvider), typeof(FontSizeComboBoxEditSettings), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOfficeFontSizeProviderChanged)));
		protected static void OnOfficeFontSizeProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FontSizeComboBoxEditSettings instance = d as FontSizeComboBoxEditSettings;
			if (instance != null)
				instance.OnOfficeFontSizeProviderChanged();
		}
		public IOfficeFontSizeProvider OfficeFontSizeProvider {
			get { return (IOfficeFontSizeProvider)GetValue(OfficeFontSizeProviderProperty); }
			set { SetValue(OfficeFontSizeProviderProperty, value); }
		}
		protected internal virtual void OnOfficeFontSizeProviderChanged() {
			Populate();
		}
		static FontSizeComboBoxEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(FontSizeComboBoxEdit), typeof(FontSizeComboBoxEditSettings), delegate() { return new FontSizeComboBoxEdit(); }, delegate() { return new FontSizeComboBoxEditSettings(); });
		}
		void Populate() {
			Items.Clear();
			if (OfficeFontSizeProvider != null) {
				foreach (int fontSize in OfficeFontSizeProvider.GetPredefinedFontSizes())
					Items.Add(fontSize);
			}
		}
	}
	#endregion
	#region FontSizeComboBoxEdit
	[DXToolboxBrowsableAttribute(false)]
	public class FontSizeComboBoxEdit : ComboBoxEdit {
		static FontSizeComboBoxEdit() {
			FontSizeComboBoxEditSettings.RegisterEditor();
		}
		public FontSizeComboBoxEdit() {
			DefaultStyleKey = typeof(FontSizeComboBoxEdit);
		}
		FontSizeComboBoxEditSettings InnerSettings { get { return Settings as FontSizeComboBoxEditSettings; } }
		public IOfficeFontSizeProvider OfficeFontSizeProvider {
			get {
				if (InnerSettings != null)
					return InnerSettings.OfficeFontSizeProvider;
				else
					return null;
			}
			set {
				if (InnerSettings != null)
					InnerSettings.OfficeFontSizeProvider = value;
			}
		}
		protected internal override BaseEditSettings CreateEditorSettings() {
			return new FontSizeComboBoxEditSettings();
		}
	}
	#endregion
}
