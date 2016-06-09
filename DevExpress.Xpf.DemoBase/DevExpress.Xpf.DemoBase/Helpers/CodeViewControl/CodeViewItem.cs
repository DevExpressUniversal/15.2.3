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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DemoBase.Helpers {
	[TemplatePart(Name = "ItemPresenter", Type = typeof(CodeViewItemPresenter))]
	class CodeViewItemContainer : ComboBoxItem {
		public static readonly DependencyProperty CodeViewItemProperty =
			DependencyPropertyManager.Register("CodeViewItem", typeof(CodeViewItem), typeof(CodeViewItemContainer), new PropertyMetadata(null));
		CodeViewItemPresenter itemPresenter;
		public CodeViewItemContainer() {
			DefaultStyleKey = typeof(CodeViewItemContainer);
		}
		public CodeViewItem CodeViewItem { get { return (CodeViewItem)GetValue(CodeViewItemProperty); } private set { SetValue(CodeViewItemProperty, value); } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.itemPresenter = GetTemplateChild("ItemPresenter") as CodeViewItemPresenter;
			if(this.itemPresenter != null) {
				Dispatcher.BeginInvoke((Action)(() => {
					SetBinding(CodeViewItemProperty, new Binding() { Path = new PropertyPath("VisualContent"), Source = this.itemPresenter, Mode = BindingMode.OneWay });
				}));
			}
		}
	}
	public class CodeLanguageText {
		public CodeLanguageText(CodeLanguage language, string text) : this(language, () => text) { }
		public CodeLanguageText(CodeLanguage language, Func<string> text) {
			Language = language;
			Text = text;
		}
		public CodeLanguage Language { get; private set; }
		public Func<string> Text { get; private set; }
	}
	class CodeViewItem : ContentControl {
		protected const string CSSuffix = ".cs";
		protected const string VBSuffix = ".vb";
		protected const string XamlSuffix = ".xaml";
		public static readonly DependencyProperty FileNameProperty =
			DependencyPropertyManager.Register("FileName", typeof(string), typeof(CodeViewItem), new PropertyMetadata(string.Empty,
				(d, e) => ((CodeViewItem)d).OnFileNameChanged(e)));
		public static readonly DependencyProperty CodeLanguageProperty =
			DependencyPropertyManager.Register("CodeLanguage", typeof(CodeLanguage?), typeof(CodeViewItem), new PropertyMetadata(null,
				(d, e) => ((CodeViewItem)d).OnCodeLanguageChanged(e)));
		public static readonly DependencyProperty CodeTextProperty =
			DependencyPropertyManager.Register("CodeText", typeof(Func<string>), typeof(CodeViewItem), new PropertyMetadata(null,
				(d, e) => ((CodeViewItem)d).OnCodeTextChanged(e)));
		public static readonly DependencyProperty IsDependentProperty =
			DependencyPropertyManager.Register("IsDependent", typeof(bool?), typeof(CodeViewItem), new PropertyMetadata(null,
				(d, e) => ((CodeViewItem)d).OnIsDependentChanged(e)));
		public static readonly DependencyProperty ActualCodeTextProperty =
			DependencyPropertyManager.Register("ActualCodeText", typeof(CodeLanguageText), typeof(CodeViewItem), new PropertyMetadata(null));
		public static readonly DependencyProperty ActualIsDependentProperty =
			DependencyPropertyManager.Register("ActualIsDependent", typeof(bool), typeof(CodeViewItem), new PropertyMetadata(false));
		bool actualCodeTextIsInvalid = false;
		public CodeViewItem() {
			DefaultStyleKey = typeof(CodeViewItem);
		}
		public string FileName { get { return (string)GetValue(FileNameProperty); } set { SetValue(FileNameProperty, value); } }
		public CodeLanguage? CodeLanguage { get { return (CodeLanguage?)GetValue(CodeLanguageProperty); } set { SetValue(CodeLanguageProperty, value); } }
		public Func<string> CodeText { get { return (Func<string>)GetValue(CodeTextProperty); } set { SetValue(CodeTextProperty, value); } }
		public bool? IsDependent { get { return (bool?)GetValue(IsDependentProperty); } set { SetValue(IsDependentProperty, value); } }
		public CodeLanguageText ActualCodeText { get { return (CodeLanguageText)GetValue(ActualCodeTextProperty); } private set { SetValue(ActualCodeTextProperty, value); } }
		public bool ActualIsDependent { get { return (bool)GetValue(ActualIsDependentProperty); } private set { SetValue(ActualIsDependentProperty, value); } }
		protected virtual void OnCodeLanguageChanged(DependencyPropertyChangedEventArgs e) {
			actualCodeTextIsInvalid = true;
			Dispatcher.BeginInvoke((Action)UpdateActualCodeText);
		}
		protected virtual void OnCodeTextChanged(DependencyPropertyChangedEventArgs e) {
			actualCodeTextIsInvalid = true;
			Dispatcher.BeginInvoke((Action)UpdateActualCodeText);
		}
		protected virtual void OnFileNameChanged(DependencyPropertyChangedEventArgs e) {
			actualCodeTextIsInvalid = true;
			UpdateActualIsDependent();
			Dispatcher.BeginInvoke((Action)UpdateActualCodeText);
		}
		void UpdateActualCodeText() {
			if(!actualCodeTextIsInvalid) return;
			actualCodeTextIsInvalid = false;
			CodeLanguage? actualCodeLanguage = CodeLanguage ?? GetCodeLanguage(FileName);
			if(actualCodeLanguage == null || CodeText == null)
				ActualCodeText = null;
			else
				ActualCodeText = new CodeLanguageText(actualCodeLanguage.Value, CodeText);
		}
		void OnIsDependentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualIsDependent();
		}
		void UpdateActualIsDependent() {
			if(IsDependent != null)
				ActualIsDependent = (bool)IsDependent;
			else
				ActualIsDependent = GetIsDependent(FileName);
		}
		protected virtual bool GetIsDependent(string fileName) {
			return fileName.EndsWith(XamlSuffix + CSSuffix) || fileName.EndsWith(XamlSuffix + VBSuffix);
		}
		protected virtual CodeLanguage? GetCodeLanguage(string fileName) {
			if(string.IsNullOrEmpty(FileName)) return null;
			if(fileName.EndsWith(CSSuffix)) return TextColorizer.CodeLanguage.CS;
			if(fileName.EndsWith(VBSuffix)) return TextColorizer.CodeLanguage.VB;
			if(fileName.EndsWith(XamlSuffix)) return TextColorizer.CodeLanguage.XAML;
			return TextColorizer.CodeLanguage.Plain;
		}
	}
	public class CodeViewItemPresenter : ContentPresenter {
		public static readonly DependencyProperty VisualContentProperty =
			DependencyPropertyManager.Register("VisualContent", typeof(UIElement), typeof(CodeViewItemPresenter), new PropertyMetadata(null));
		static readonly DependencyProperty ContentListenProperty =
			DependencyProperty.Register("ContentListen", typeof(object), typeof(CodeViewItemPresenter), new System.Windows.PropertyMetadata(null,
				(d, e) => ((CodeViewItemPresenter)d).OnContentChanged(e)));
		static readonly DependencyProperty ContentTemplateListenProperty =
			DependencyProperty.Register("ContentTemplateListen", typeof(object), typeof(CodeViewItemPresenter), new System.Windows.PropertyMetadata(null,
				(d, e) => ((CodeViewItemPresenter)d).OnContentTemplateChanged(e)));
		public CodeViewItemPresenter() {
			SetBinding(ContentListenProperty, new Binding("Content") { Source = this, Mode = BindingMode.OneWay });
		}
		public UIElement VisualContent { get { return (UIElement)GetValue(VisualContentProperty); } private set { SetValue(VisualContentProperty, value); } }
		protected virtual void OnContentChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			VisualContent = null;
		}
		protected virtual void OnContentTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			VisualContent = null;
		}
		protected override Size MeasureOverride(Size availableSize) {
			int childrenCount = VisualTreeHelper.GetChildrenCount(this);
			VisualContent = childrenCount == 0 ? null : VisualTreeHelper.GetChild(this, 0) as UIElement;
			return base.MeasureOverride(availableSize);
		}
	}
	public class CodeViewItemsPresenter : ComboBox {
		public static readonly DependencyProperty SelectedItemContainerContentProperty =
			DependencyPropertyManager.Register("SelectedItemContainerContent", typeof(UIElement), typeof(CodeViewItemsPresenter), new PropertyMetadata(null,
				(d, e) => ((CodeViewItemsPresenter)d).OnSelectedItemContainerContentChanged(e)));
		CodeViewItemPresenter selectedItemPresenter;
		public CodeViewItemsPresenter() {
			DefaultStyleKey = typeof(CodeViewItemsPresenter);
		}
		public UIElement SelectedItemContainerContent { get { return (UIElement)GetValue(SelectedItemContainerContentProperty); } set { SetValue(SelectedItemContainerContentProperty, value); } }
		public event DependencyPropertyChangedEventHandler SelectedItemContainerContentChanged;
		protected override DependencyObject GetContainerForItemOverride() {
			return new CodeViewItemContainer();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.selectedItemPresenter = GetTemplateChild("ContentPresenter") as CodeViewItemPresenter;
			if(this.selectedItemPresenter != null)
				SetBinding(SelectedItemContainerContentProperty, new Binding() { Path = new PropertyPath("VisualContent"), Source = this.selectedItemPresenter, Mode = BindingMode.OneWay });
		}
		void OnSelectedItemContainerContentChanged(DependencyPropertyChangedEventArgs e) {
			if(SelectedItemContainerContentChanged != null)
				SelectedItemContainerContentChanged(this, e);
		}
	}
}
