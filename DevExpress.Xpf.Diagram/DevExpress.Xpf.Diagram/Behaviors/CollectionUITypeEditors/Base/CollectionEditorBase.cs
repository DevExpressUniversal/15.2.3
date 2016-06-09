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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Diagram.Core;
using System.Collections;
namespace DevExpress.Xpf.Diagram {
	public abstract class CollectionEditorBase : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemDetailedDescriptionTemplateProperty;
		public static readonly DependencyProperty ItemDetailedDescriptionTemplateWrapperProperty;
		public static readonly DependencyProperty ItemsControlHeaderTemplateProperty;
		public static readonly DependencyProperty DialogFooterTemplateProperty;
		public static readonly DependencyProperty ItemsControlTemplateProperty;
		public static readonly DependencyProperty ItemsListTemplateProperty;
		public static readonly DependencyProperty EditorProperty;
		public static readonly DependencyProperty NavigationPanelTemplateProperty;
		public static readonly DependencyProperty ActionButtonsTemplateProperty;
		static CollectionEditorBase() {
			DependencyPropertyRegistrator<CollectionEditorBase>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged())
				.Register(d => d.ItemTemplate, out ItemTemplateProperty, null)
				.Register(d => d.ItemDetailedDescriptionTemplate, out ItemDetailedDescriptionTemplateProperty, null)
				.Register(d => d.ItemDetailedDescriptionTemplateWrapper, out ItemDetailedDescriptionTemplateWrapperProperty, null)
				.Register(d => d.ItemsControlHeaderTemplate, out ItemsControlHeaderTemplateProperty, null)
				.Register(d => d.DialogFooterTemplate, out DialogFooterTemplateProperty, null)
				.Register(d => d.ItemsControlTemplate, out ItemsControlTemplateProperty, null)
				.Register(d => d.NavigationPanelTemplate, out NavigationPanelTemplateProperty, null)
				.Register(d => d.ActionButtonsTemplate, out ActionButtonsTemplateProperty, null)
				.Register(d => d.ItemsListTemplate, out ItemsListTemplateProperty, null)
				.RegisterAttached((DependencyObject d) => GetEditor(d), out EditorProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.OverrideDefaultStyleKey()
			;
		}
		public CollectionEditorBase() {
			AddItemCommand = DelegateCommandFactory.Create(() => AddItem(null));
			RemoveItemCommand = DelegateCommandFactory.Create(RemoveItem, CanExecuteRemoveItemCommand, true);
			MoveUpCommand = DelegateCommandFactory.Create(() => MoveItems(false), CanExecuteMoveUpCommand, true);
			MoveDownCommand = DelegateCommandFactory.Create(() => MoveItems(true), CanExecuteMoveDownCommand, true);
		}
		protected ICollectionEditorController controller;
		public static CollectionEditorBase GetEditor(DependencyObject d) { return (CollectionEditorBase)d.GetValue(EditorProperty); }
		public static void SetEditor(DependencyObject d, CollectionEditorBase v) { d.SetValue(EditorProperty, v); }
		public ICommand AddItemCommand { get; private set; }
		public ICommand RemoveItemCommand { get; private set; }
		public ICommand MoveUpCommand { get; private set; }
		public ICommand MoveDownCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplate ItemDetailedDescriptionTemplate {
			get { return (DataTemplate)GetValue(ItemDetailedDescriptionTemplateProperty); }
			set { SetValue(ItemDetailedDescriptionTemplateProperty, value); }
		}
		public DataTemplate ItemDetailedDescriptionTemplateWrapper {
			get { return (DataTemplate)GetValue(ItemDetailedDescriptionTemplateWrapperProperty); }
			set { SetValue(ItemDetailedDescriptionTemplateWrapperProperty, value); }
		}
		public DataTemplate ItemsControlHeaderTemplate {
			get { return (DataTemplate)GetValue(ItemsControlHeaderTemplateProperty); }
			set { SetValue(ItemsControlHeaderTemplateProperty, value); }
		}
		public DataTemplate DialogFooterTemplate {
			get { return (DataTemplate)GetValue(DialogFooterTemplateProperty); }
			set { SetValue(DialogFooterTemplateProperty, value); }
		}
		public DataTemplate ItemsControlTemplate {
			get { return (DataTemplate)GetValue(ItemsControlTemplateProperty); }
			set { SetValue(ItemsControlTemplateProperty, value); }
		}
		public DataTemplate NavigationPanelTemplate {
			get { return (DataTemplate)GetValue(NavigationPanelTemplateProperty); }
			set { SetValue(NavigationPanelTemplateProperty, value); }
		}
		public DataTemplate ActionButtonsTemplate {
			get { return (DataTemplate)GetValue(ActionButtonsTemplateProperty); }
			set { SetValue(ActionButtonsTemplateProperty, value); }
		}
		public DataTemplate ItemsListTemplate {
			get { return (DataTemplate)GetValue(ItemsListTemplateProperty); }
			set { SetValue(ItemsListTemplateProperty, value); }
		}
		IList items;
		protected IList Items { get { return items; } }
		protected abstract void SetSelected(object item);
		protected abstract bool ItemIsSelected(object item);
		protected abstract void MoveItems(bool moveDown);
		protected abstract bool CanExecuteMoveUpCommand();
		protected abstract bool CanExecuteMoveDownCommand();
		protected abstract bool CanExecuteRemoveItemCommand();
		public virtual Func<IMultiModel, bool> IsEditorItem { get { return x => true; } }
		protected void DoWithSelectedItem(Action<int> action, bool doInReverseOrder, bool breakOnFirstItemFound) {
			if(doInReverseOrder) {
				for(int i = Items.Count - 1; i >= 0; --i) {
					if(ItemIsSelected(Items[i])) {
						action(i);
						if(breakOnFirstItemFound)
							break;
					}
				}
			} else {
				for(int i = 0; i < Items.Count; ++i) {
					if(ItemIsSelected(Items[i])) {
						action(i);
						if(breakOnFirstItemFound)
							break;
					}
				}
			}
		}
		protected List<object> GetCollectionEditorItems() {
			var collectionItemsList = new List<object>();
			foreach(IMultiModel item in Items) {
				if(IsEditorItem(item))
					collectionItemsList.Add(item);
			}
			return collectionItemsList;
		}
		protected void DoWithEditorItems(Action<int> action) {
			for(int i = Items.Count - 1; i >= 0; --i) {
				var item = (IMultiModel)Items[i];
				if(IsEditorItem(item))
					action(i);
			}
		}
		public abstract object CreateItem();
		protected virtual void AddItem(object item) {
			controller.Add(item);
			SetSelected(Items[Items.Count - 1]);
		}
		protected void RemoveAllItems() {
			DoWithEditorItems(x => Items.RemoveAt(x));
		}
		protected virtual void RemoveItem() {
			int removedItemIndex = 0;
			DoWithSelectedItem(x => { removedItemIndex = x; Items.RemoveAt(x); }, true, false);
			var collectionEditorItems = GetCollectionEditorItems();
			if(collectionEditorItems.Count > 0)
				SetSelected(collectionEditorItems[0]);
		}
		void OnEditValueChanged() {
			var genericArguments = EditValue.GetType().GetGenericArguments();
			controller = (ICollectionEditorController)Activator.CreateInstance(typeof(CollectionEditorController<,>).MakeGenericType(genericArguments[1], genericArguments[2]), this);
			items = (IList)EditValue;
			var collectionEditorItems = GetCollectionEditorItems();
			if(collectionEditorItems.Any() )
				SetSelected(collectionEditorItems[0]);
		}
	}
}
