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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.Collections;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Utils.Internal;
#endif
namespace DevExpress.Xpf.Editors.Settings {
#if !SL
	public static class FontUtility {
		public static ReadOnlyCollection<FontFamily> GetSystemFontFamilies() {
			List<FontFamily> systemFontFamilies = new List<FontFamily>();
			foreach (FontFamily fontFamily in Fonts.SystemFontFamilies) {
				try {
					var unused = fontFamily.FamilyNames;
					systemFontFamilies.Add(fontFamily);
				}
				catch (ArgumentException) {
				}
			}
			return systemFontFamilies.AsReadOnly();
		}
	}
#endif
	public class FontEditSettings : ComboBoxEditSettings {
		internal static List<FontFamily> CachedFonts { get; set; }
		public static readonly DependencyProperty AllowConfirmFontUseDialogProperty;
		static FontEditSettings() {
			Type ownerType = typeof(FontEditSettings);
			CachedFonts = GetSystemFonts();
			AllowConfirmFontUseDialogProperty = DependencyPropertyManager.Register("AllowConfirmFontUseDialog", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnSettingsPropertyChanged));
#if !SL
			AutoCompleteProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			ValidateOnTextInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
#endif
		}
		internal static List<FontFamily> GetSystemFonts() {
			List<FontFamily> result = new List<FontFamily>();
#if SL
			XmlLanguage lang = null;
			foreach (string fontFamily in FontManager.GetFontFamilyNames()) {
#else
			var lang = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
			foreach (FontFamily fontFamily in FontUtility.GetSystemFontFamilies()) {
				if (IsValid(fontFamily)) 
#endif
					result.Add(new FontFamily(GetFontFamilyName(fontFamily, lang)));
			}
			result.Sort(new FontFamilyComparer());
			return result;
		}
#if SL
		static string GetFontFamilyName(string fontFamily, XmlLanguage lang) {
			return fontFamily;
		}
#endif
#if !SL
		static string GetFontFamilyName(FontFamily fontFamily, XmlLanguage lang) {
			try {
				return fontFamily.FamilyNames.ContainsKey(lang) ? fontFamily.FamilyNames[lang] : fontFamily.ToString();
			}
			catch {
			}
			return fontFamily.ToString();
		}
		static bool IsValid(FontFamily fontFamily) {
			GlyphTypeface glyphTypeface;
			foreach (Typeface typeface in fontFamily.GetTypefaces()) {
				try {
					typeface.TryGetGlyphTypeface(out glyphTypeface);
				}
				catch {
					return false;
				}
			}
			return true;
		}
#endif
		class FontFamilyComparer : IComparer<FontFamily> {
			int IComparer<FontFamily>.Compare(FontFamily x, FontFamily y) {
				return x.ToString().CompareTo(y.ToString());
			}
		}
		public bool AllowConfirmFontUseDialog {
			get { return (bool)GetValue(AllowConfirmFontUseDialogProperty); }
			set { SetValue(AllowConfirmFontUseDialogProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			FontEdit fe = edit as FontEdit;
			if (fe == null)
				return;
			fe.AllowConfirmFontUseDialog = AllowConfirmFontUseDialog;
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class FontSettingsExtension : ComboBoxSettingsExtension {
		public bool AllowConfirmFontUseDialog { get; set; }
		protected override PopupBaseEditSettings CreatePopupBaseEditSettings() {
			return new FontEditSettings() { AllowConfirmFontUseDialog = this.AllowConfirmFontUseDialog };
		}
		public FontSettingsExtension() {
			AutoComplete = true;
			ValidateOnTextInput = false;
			AllowConfirmFontUseDialog = (bool)FontEdit.AllowConfirmFontUseDialogProperty.DefaultMetadata.DefaultValue;
		}
	}
}
#endif
