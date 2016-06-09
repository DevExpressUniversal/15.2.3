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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Office.Model;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Office.Internal;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region AgFontSizeListBox
	public class AgFontSizeListBox : Control {
		public AgFontSizeListBox()
			: base() {
			DefaultStyleKey = typeof(AgFontSizeListBox);
		}
		TextBox Editor { get { return GetTemplateChild("Editor") as TextBox; } }
		ListBox FontSizeListBox { get { return GetTemplateChild("lbFontSize") as ListBox; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Populate();
			FontSizeListBox.SelectionChanged += OnFontSizeListBoxSelectionChanged;
			Editor.TextChanged += (d, e) => { TextChanged(); };
			OnValueChanged();
		}
		void OnFontSizeListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
			SetEditorText();
		}
		void SetEditorText() {
			if (FontSizeListBox == null || Editor == null) return;
			if (FontSizeListBox.SelectedIndex < 0) {
				Editor.Text = string.Empty;
			}
			else
				Editor.Text = ((string)FontSizeListBox.SelectedItem);
		}
		void TextChanged() {
			Value = GetEditorValue();
			RaiseValueChanged();
			SyncListBox();
		}
		void RaiseValueChanged() {
			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}
		void SyncListBox() {
			FontSizeListBox.SelectionChanged -= OnFontSizeListBoxSelectionChanged;
			FontSizeListBox.SelectedIndex = -1;
			foreach (string item in FontSizeListBox.Items) {
				if (item == Editor.Text) {
					FontSizeListBox.SelectedItem = item;
					EnsureSelectedItemVisible();
					break;
				}
			}
			FontSizeListBox.SelectionChanged += OnFontSizeListBoxSelectionChanged;
		}
		void EnsureSelectedItemVisible() {
			Dispatcher.BeginInvoke((Action)EnsureSelectedItemVisibleCore);
		}
		void EnsureSelectedItemVisibleCore() {
			if (FontSizeListBox == null)
				return;
			if (FontSizeListBox.SelectedItem == null)
				return;
			FontSizeListBox.ScrollIntoView(FontSizeListBox.SelectedItem);
		}
		void Populate() {
			foreach (int fontSize in new PredefinedFontSizeCollection())
				FontSizeListBox.Items.Add(fontSize.ToString());
		}
		public event EventHandler ValueChanged;
		int value = -1;
		public int Value {
			get {
				return value;
			}
			set {
				if (value == Value)
					return;
				this.value = value;
				OnValueChanged();
			}
		}
		protected virtual void OnValueChanged() {
			if (Editor == null)
				return;
			if (Value < 0) {
				Editor.Text = string.Empty;
			}
			else {
				float newValue = Value / 2f;
				Editor.Text = newValue.ToString();
			}
		}
		int GetEditorValue() {
			if (Editor == null)
				return -1;
			int val = -1;
			if (OfficeFontSizeEditHelper.TryGetHalfSizeValue(Editor.Text, out val)) {
				return val;
			}
			return -1;
		}
	}
	#endregion
	#region RichEditFontSizeComboBoxEditSettings
	public class RichEditFontSizeComboBoxEditSettings : ComboBoxEditSettings, IRichEditControlDependencyPropertyOwner {
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(RichEditFontSizeComboBoxEditSettings), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditFontSizeComboBoxEditSettings instance = d as RichEditFontSizeComboBoxEditSettings;
			if (instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			Populate();
		}
		static RichEditFontSizeComboBoxEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(RichEditFontSizeComboBoxEdit), typeof(RichEditFontSizeComboBoxEditSettings), delegate() { return new RichEditFontSizeComboBoxEdit(); }, delegate() { return new RichEditFontSizeComboBoxEditSettings(); });
		}
		void Populate() {
			Items.Clear();
			if (RichEditControl != null) {
				foreach (int fontSize in RichEditControl.GetPredefinedFontSizeCollection())
					Items.Add(fontSize);
			}
		}
		#region IRichEditControlDependencyPropertyOwner Members
		DependencyProperty IRichEditControlDependencyPropertyOwner.DependencyProperty { get { return RichEditControlProperty; } }
		#endregion
	}
	#endregion
	#region RichEditFontSizeComboBoxEdit
	[DXToolboxBrowsableAttribute(false)]
	public class RichEditFontSizeComboBoxEdit : ComboBoxEdit {
		static RichEditFontSizeComboBoxEdit() {
			RichEditFontSizeComboBoxEditSettings.RegisterEditor();
		}
		public RichEditFontSizeComboBoxEdit() {
			DefaultStyleKey = typeof(RichEditFontSizeComboBoxEdit);
		}
		RichEditFontSizeComboBoxEditSettings InnerSettings { get { return Settings as RichEditFontSizeComboBoxEditSettings; } }
		public RichEditControl RichEditControl {
			get {
				if (InnerSettings != null)
					return InnerSettings.RichEditControl;
				else
					return null;
			}
			set {
				if (InnerSettings != null)
					InnerSettings.RichEditControl = value;
			}
		}
		protected override BaseEditSettings CreateEditorSettings() {
			return new RichEditFontSizeComboBoxEditSettings();
		}
	}
	#endregion
}
