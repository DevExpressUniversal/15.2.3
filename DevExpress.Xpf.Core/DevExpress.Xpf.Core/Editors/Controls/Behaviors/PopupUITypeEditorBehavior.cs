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
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public interface IPopupSource : IEditorSource {
		DataTemplate ContentTemplate { get; }
		DataTemplateSelector ContentTemplateSelector { get; }
	}
	[TargetType(typeof(ButtonInfo))]
	[TargetType(typeof(PopupContainer))]
	public class PopupEditorBehavior : EditorBehavior, IPopupSource {
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public event PopupClosedEventHandler PopupClosing;
		public event PopupOpenedEventHandler PopupOpened;
		public event EventHandler<EventArgs> PopupClosed;
		static PopupEditorBehavior() {
			ContentTemplateProperty = DependencyPropertyRegistrator.Register<PopupEditorBehavior, DataTemplate>(owner => owner.ContentTemplate, null);
			ContentTemplateSelectorProperty = DependencyPropertyRegistrator.Register<PopupEditorBehavior, DataTemplateSelector>(owner => owner.ContentTemplateSelector, null);
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		protected override void Subscribe() {
			base.Subscribe();
			var editor = BaseEdit.GetOwnerEdit(AssociatedObject) as PopupBaseEdit;
			if (editor != null)
				SubscribeForPopupContainerEdit(editor);
		}
		protected override void Unsubscribe() {
			base.Unsubscribe();
			var editor = BaseEdit.GetOwnerEdit(AssociatedObject) as PopupBaseEdit;
			if (editor != null)
				UnsubscribeForPopupContainerEdit(editor);
		}
		void SubscribeForPopupContainerEdit(PopupBaseEdit editor) {
			editor.PopupOpened += EditorOnPopupOpened;
			editor.PopupClosing += EditorOnPopupClosing;
			editor.PopupClosed += EditorOnPopupClosed;
		}
		void EditorOnPopupClosed(object sender, ClosePopupEventArgs e) {
			RaisePopupClosed();
		}
		void UnsubscribeForPopupContainerEdit(PopupBaseEdit editor) {
			editor.PopupOpened -= EditorOnPopupOpened;
			editor.PopupClosing -= EditorOnPopupClosing;
			editor.PopupClosed -= EditorOnPopupClosed;
		}
		void EditorOnPopupClosing(object sender, ClosePopupEventArgs e) {
			var editor = BaseEdit.GetOwnerEdit(AssociatedObject) as PopupBaseEdit;
			if (editor == null)
				return;
			var value = editor.PopupSettings.PopupValue;
			RaisePopupClosing(value);
		}
		void EditorOnPopupOpened(object sender, RoutedEventArgs e) {
			var editor = BaseEdit.GetOwnerEdit(AssociatedObject) as PopupBaseEdit;
			if (editor == null)
				return;
			var value = editor.PopupSettings.PopupValue;
			RaisePopupOpened(value);
		}
		void RaisePopupClosing(UITypeEditorValue value) {
			var popupClosed = PopupClosing;
			popupClosed.Do(x => x(this, new PopupClosedEventEventArgs(value)));
		}
		void RaisePopupOpened(UITypeEditorValue value) {
			var popupOpened = PopupOpened;
			popupOpened.Do(x => x(this, new PopupOpenedEventEventArgs(value)));
		}
		void RaisePopupClosed() {
			var popupClosed = PopupClosed;
			popupClosed.Do(x => x(this, EventArgs.Empty));
		}
		protected override void ProcessClick(DependencyObject owner) {
			var popupBaseEdit = owner as PopupBaseEdit;
			if (popupBaseEdit != null) {
				ProcessClickForPopupBaseEdit(popupBaseEdit);
				return;
			}
			var buttonInfo = owner as ButtonInfo;
			if (buttonInfo != null) {
				ProcessClickForButtonInfo(buttonInfo);
			}
			var fre = owner as FrameworkElement;
			if (fre != null) {
				ProcessClickForFrameworkElement(fre);
				return;
			}
		}
		void ProcessClickForFrameworkElement(FrameworkElement fre) {
		}
		void ProcessClickForButtonInfo(ButtonInfo buttonInfo) {
			var editor = BaseEdit.GetOwnerEdit(buttonInfo) as PopupBaseEdit;
			if (editor == null)
				return;
			var popupService = editor.PropertyProvider.GetService<PopupService>();
			var popupSource = buttonInfo.IsDefaultButton ? null : this;
			popupService.SetPopupSource(popupSource);
		}
		void ProcessClickForPopupBaseEdit(PopupBaseEdit popupBaseEdit) {
			var popupService = popupBaseEdit.PropertyProvider.GetService<PopupService>();
			popupService.SetPopupSource(null);
		}
	}
	public delegate void PopupClosedEventHandler(object d, PopupClosedEventEventArgs e);
	public class PopupClosedEventEventArgs : EventArgs {
		bool? postValue;
		public UITypeEditorValue Value { get; private set; }
		public bool Handled { get; set; }
		public bool? PostValue {
			get { return this.postValue; }
			set {
				if (this.postValue == value)
					return;
				this.postValue = value;
				Value.ForcePost = value;
			}
		}
		public PopupClosedEventEventArgs(UITypeEditorValue value) {
			Value = value;
		}
	}
	public delegate void PopupOpenedEventHandler(object d, PopupOpenedEventEventArgs e);
	public class PopupOpenedEventEventArgs : EventArgs {
		public UITypeEditorValue Value { get; private set; }
		public PopupOpenedEventEventArgs(UITypeEditorValue value) {
			Value = value;
		}
	}
}
