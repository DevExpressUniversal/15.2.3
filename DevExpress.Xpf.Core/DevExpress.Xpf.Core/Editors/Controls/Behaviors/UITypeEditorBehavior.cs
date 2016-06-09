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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public interface IEditorSource {
		object Content { get; }
		UITypeEditorValue GetEditableValue(DependencyObject owner, object defaultValue);
		void AcceptEditableValue(UITypeEditorValue value);
	}
	[ContentProperty("Content")]
	public abstract class EditorBehavior : Behavior<DependencyObject>, IEditorSource {
		public static readonly DependencyProperty ValueProperty;
		public static readonly DependencyProperty ContentProperty;
		static EditorBehavior() {
			ContentProperty = DependencyPropertyRegistrator.Register<EditorBehavior, object>(owner => owner.Content, null);
			ValueProperty = DependencyPropertyRegistrator.Register<EditorBehavior, object>(owner => owner.Value, null);
		}
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value);}
		}
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			Subscribe();
		}
		protected override void OnDetaching() {
			Unsubscribe();
			base.OnDetaching();
		}
		protected virtual void Subscribe() {
			var buttonInfo = AssociatedObject as ButtonInfo;
			if (buttonInfo != null) {
				buttonInfo.Click += ButtonInfoClick;
				if (buttonInfo.IsDefaultButton) {
					var buttonEdit = BaseEdit.GetOwnerEdit(buttonInfo) as ButtonEdit;
					buttonEdit.Do(x => x.DefaultButtonClick += ButtonEditButtonClick);
				}
				return;
			}
			var editor = AssociatedObject as ButtonEdit;
			if (editor != null) {
				editor.DefaultButtonClick += ButtonEditButtonClick;
				return;
			}
			var fre = AssociatedObject as FrameworkElement;
			if (fre != null) {
				fre.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ButtonClick));
				return;
			}
			var barButtonItem = AssociatedObject as BarButtonItem;
			if (barButtonItem != null) {
				barButtonItem.ItemClick += BarButtonItemItemClick;
				return;
			}
		}
		protected virtual void Unsubscribe() {
			var buttonInfo = AssociatedObject as ButtonInfo;
			if (buttonInfo != null) {
				buttonInfo.Click -= ButtonInfoClick;
				if (buttonInfo.IsDefaultButton) {
					var buttonEdit = BaseEdit.GetOwnerEdit(buttonInfo) as ButtonEdit;
					buttonEdit.Do(x => x.DefaultButtonClick -= ButtonEditButtonClick);
				}
				return;
			}
			var editor = AssociatedObject as ButtonEdit;
			if (editor != null) {
				editor.DefaultButtonClick -= ButtonEditButtonClick;
				return;
			}
			var fre = AssociatedObject as FrameworkElement;
			if (fre != null) {
				fre.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ButtonClick));
				return;
			}
			var barButtonItem = AssociatedObject as BarButtonItem;
			if (barButtonItem != null) {
				barButtonItem.ItemClick -= BarButtonItemItemClick;
				return;
			}
		}
		void ButtonEditButtonClick(object sender, RoutedEventArgs e) {
			ProcessClick((DependencyObject)sender);
		}
		void BarButtonItemItemClick(object sender, ItemClickEventArgs e) {
			ProcessClick(e.Link.LinkInfos.FirstOrDefault());
		}
		void ButtonClick(object sender, RoutedEventArgs e) {
			ProcessClick((FrameworkElement)sender);
		}
		void ButtonInfoClick(object sender, RoutedEventArgs e) {
			var button = (FrameworkElement)sender;
			var buttonInfo = button.DataContext as ButtonInfo;
			ProcessClick(buttonInfo);
		}
		protected virtual void ProcessClick(DependencyObject owner) {
		}
		UITypeEditorValue IEditorSource.GetEditableValue(DependencyObject owner, object defaultValue) {
			return GetEditableValue(owner, defaultValue);
		}
		protected virtual UITypeEditorValue GetEditableValue(DependencyObject owner, object editValue) {
			var editor = owner as ButtonEdit;
			if (editor != null) {
				var convertedValue = GetEditValueForButtonEdit(editor, editValue);
				return new UITypeEditorValue(owner, this, convertedValue, Content ?? convertedValue);
			}
			return new UITypeEditorValue(owner, this, this.IsPropertySet(ValueProperty) ? Value : editValue, Content ?? editValue);
		}
		protected virtual object GetEditValueForButtonEdit(ButtonEdit editor, object defaultValue) {
			var editMode = editor.EditMode;
			if (editMode == EditMode.Standalone)
				return defaultValue;
			var inplaceEditorBase = (InplaceEditorBase)TreeHelper.GetParent(editor, x => x is InplaceEditorBase, false);
			if (inplaceEditorBase == null)
				return defaultValue;
			return inplaceEditorBase.GetEditableValueForExternalEditor();
		}
		protected virtual void PostEditValue(DependencyObject owner, object editValue) {
			var buttonEdit = owner as ButtonEdit;
			if (buttonEdit != null) {
				PostEditValueToButtonEdit(buttonEdit, editValue);
				return;
			}
			this.SetCurrentValue(ValueProperty, editValue);
		}
		protected virtual void PostEditValueToButtonEdit(ButtonEdit buttonEdit, object editValue) {
			EditMode editMode = buttonEdit.EditMode;
			if (editMode != EditMode.Standalone) {
				var inplaceEditorBase = (InplaceEditorBase)TreeHelper.GetParent(buttonEdit, x => x is InplaceEditorBase, false);
				if (inplaceEditorBase == null)
					return;
				inplaceEditorBase.SetEditableValueFromExternalEditor(editValue);
				return;
			}
			var valueContainerService = buttonEdit.PropertyProvider.GetService<ValueContainerService>();
			valueContainerService.SetEditValue(editValue, UpdateEditorSource.ValueChanging);
			buttonEdit.ForceChangeDisplayText();
		}
		public void AcceptEditableValue(UITypeEditorValue value) {
			if (value.ShouldPost())
				PostEditValue(value.Owner, value.Value);
		}
	}
}
