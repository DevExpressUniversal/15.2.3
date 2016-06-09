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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	[TargetType(typeof(ButtonInfo))]
	[TargetType(typeof(Button))]
	[TargetType(typeof(BarButtonItem))]
	[TargetType(typeof(ButtonEdit))]
	[TargetType(typeof(ButtonEditSettings))]
	public abstract class DialogServiceEditorBehaviorBase : EditorBehavior {
		public static readonly DependencyProperty DialogServiceTemplateProperty;
		public static readonly DependencyProperty TitleProperty;
		static DialogServiceEditorBehaviorBase() {
			TitleProperty = DependencyPropertyRegistrator.Register<DialogServiceEditorBehaviorBase, string>(owner => owner.Title, null);
			DialogServiceTemplateProperty = DependencyPropertyRegistrator.Register<DialogServiceEditorBehaviorBase, DataTemplate>(owner => owner.DialogServiceTemplate, null);
		}
		public event BeforeDialogServiceDialogShownEventHandler BeforeDialogShown;
		public event AfterDialogServiceDialogClosedEventHandler AfterDialogClosed;
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public DataTemplate DialogServiceTemplate {
			get { return (DataTemplate)GetValue(DialogServiceTemplateProperty); }
			set { SetValue(DialogServiceTemplateProperty, value); }
		}
		protected DialogServiceEditorBehaviorBase() {
		}
		protected void RaiseDialogShown(UITypeEditorValue value) {
			var args = new BeforeDialogServiceDialogShownEventArgs(value);
			var dialogShown = BeforeDialogShown;
			dialogShown.Do(x => x(this, args));
		}
		protected AfterDialogServiceDialogClosedEventArgs RaiseDialogClosed(UITypeEditorValue value, object dialogResult) {
			var args = new AfterDialogServiceDialogClosedEventArgs(value, dialogResult);
			var dialogClosed = AfterDialogClosed;
			dialogClosed.Do(x => x(this, args));
			return args;
		}
		protected override void ProcessClick(DependencyObject owner) {
			base.ProcessClick(owner);
			var buttonEdit = owner as ButtonEdit;
			if (buttonEdit != null) {
				ProcessClickForFrameworkElement(buttonEdit, buttonEdit.EditValue);
				return;
			}
			var buttonInfo = owner as ButtonInfo;
			if (buttonInfo != null) {
				ProcessClickForButtonInfo(buttonInfo);
				return;
			}
			var fre = owner as FrameworkElement;
			if (fre != null) {
				ProcessClickForFrameworkElement(fre, null);				
				return;
			}
		}
		void ProcessClickForButtonInfo(ButtonInfo buttonInfo) {
			var editor = BaseEdit.GetOwnerEdit(buttonInfo) as ButtonEdit;
			if (editor == null)
				return;
			ProcessClickForFrameworkElement(editor, editor.EditValue);
		}
		void ProcessClickForFrameworkElement(FrameworkElement owner, object editValue) {
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ProcessClickForFrameworkElementInternal(owner, editValue)));
		}
		protected abstract void ProcessClickForFrameworkElementInternal(FrameworkElement owner, object editValue);
	}
	public delegate void BeforeDialogServiceDialogShownEventHandler(object d, BeforeDialogServiceDialogShownEventArgs e);
	public class BeforeDialogServiceDialogShownEventArgs : EventArgs {
		public UITypeEditorValue Value { get; private set; }
		public BeforeDialogServiceDialogShownEventArgs(UITypeEditorValue value) : base() {
			Value = value;
		}
	}
	public delegate void AfterDialogServiceDialogClosedEventHandler(object d, AfterDialogServiceDialogClosedEventArgs e);
	public class AfterDialogServiceDialogClosedEventArgs : EventArgs {
		public UITypeEditorValue Value { get; private set; }
		public object Result { get; private set; }
		public bool Handled { get; set; }
		public bool? PostValue { get; set; }
		public AfterDialogServiceDialogClosedEventArgs(UITypeEditorValue value, object result) : base() {
			Value = value;
			Result = result;
		}
	}
	public class UICommandContainerCollection : ObservableCollection<UICommandContainer> {
		public UICommandContainerCollection() {
		}
		public UICommandContainerCollection(IEnumerable<UICommandContainer> collection) : base(collection) {
		}
	}
	public class DialogServiceUITypeEditorSelector : DataTemplateSelector {
		readonly DataTemplateSelector innerSelector;
		public DialogServiceUITypeEditorSelector(DataTemplateSelector innerSelector) {
			this.innerSelector = innerSelector;
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if (this.innerSelector == null)
				return (DataTemplate)ResourceHelper.FindResource((FrameworkElement)container,
					new PopupBaseEditThemeKeyExtension() {
						ResourceKey = PopupBaseEditThemeKeys.DialogServiceContentTemplate,
						ThemeName = ThemeHelper.GetEditorThemeName(container)
					});
			UITypeEditorValue value = item as UITypeEditorValue;
			if (value != null)
				return this.innerSelector.SelectTemplate(value.Content, container);
			return this.innerSelector.SelectTemplate(value, container);
		}
	}
	public class UICommandContainerGeneratorExtension : MarkupExtension {
		public MessageBoxButton Buttons { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var buttons = UICommand.GenerateFromMessageBoxButton(Buttons, new DXDialogWindowMessageBoxButtonLocalizer());
			UICommandContainerCollection collection = new UICommandContainerCollection(buttons.Select(x => new UICommandContainer(x)));
			return collection;
		}
	}
}
