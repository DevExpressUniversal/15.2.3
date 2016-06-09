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
using System.Windows.Data;
namespace DevExpress.Xpf.WindowsUI.Base {
	public abstract class veContentSelector : veSelector, IContentSelector {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedContentProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedContentTemplateProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedContentTemplateSelectorProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		static veContentSelector() {
			var dProp = new DependencyPropertyRegistrator<veContentSelector>();
			dProp.Register("SelectedContent", ref SelectedContentProperty, (object)null,
				(dObj, e) => ((veContentSelector)dObj).OnSelectedContentChanged(e.NewValue, e.OldValue));
			dProp.Register("SelectedContentTemplate", ref SelectedContentTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((veContentSelector)dObj).OnSelectedContentTemplateChanged());
			dProp.Register("SelectedContentTemplateSelector", ref SelectedContentTemplateSelectorProperty, (DataTemplateSelector)null,
				(dObj, e) => ((veContentSelector)dObj).OnSelectedContentTemplateSelectorChanged());
			dProp.Register("ContentTemplate", ref ContentTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((veContentSelector)dObj).OnContentTemplateChanged());
			dProp.Register("ContentTemplateSelector", ref ContentTemplateSelectorProperty, (DataTemplateSelector)null);
		}
		void OnContentTemplateChanged() {
			UpdateSelectedContent();
		}
		#endregion static
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateSelectedContent();
		}
		protected override void OnDispose() {
			ClearValue(SelectedContentProperty);
			ClearValue(SelectedContentTemplateProperty);
			ClearValue(SelectedContentTemplateSelectorProperty);
			base.OnDispose();
		}
		#region Properties
		public object SelectedContent {
			get { return GetValue(SelectedContentProperty); }
			protected set { SetValue(SelectedContentProperty, value); }
		}
#if DEBUGTEST
		public bool HasSelectedContent { get { return SelectedContent != null; } }
#endif
		public DataTemplate SelectedContentTemplate {
			get { return (DataTemplate)GetValue(SelectedContentTemplateProperty); }
			private set { SetValue(SelectedContentTemplateProperty, value); }
		}
		public DataTemplateSelector SelectedContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(SelectedContentTemplateSelectorProperty); }
			private set { SetValue(SelectedContentTemplateSelectorProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		#endregion Properties
#if SILVERLIGHT
		protected override void OnSelectedItemChanged(object oldItem, object item) {
			base.OnSelectedItemChanged(oldItem, item);
			UpdateSelectedContent();
		}
#else
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			UpdateSelectedContent();
		}
#endif
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			UpdateSelectedContent();
		}
		protected virtual void OnSelectedContentChanged(object newValue, object oldValue) { }
		protected virtual void OnSelectedContentTemplateChanged() { }
		protected virtual void OnSelectedContentTemplateSelectorChanged() { }
		protected virtual void UpdateSelectedContent() {
			if(!IsLoaded) return;
			if(SelectedIndex < 0) {
				ClearValue(SelectedContentProperty);
				ClearValue(SelectedContentTemplateProperty);
				ClearValue(SelectedContentTemplateSelectorProperty);
			}
			else {
				IContentSelectorItem selectedItem = GetSelectedItem();
				if(selectedItem != null) {
					SelectedContent = selectedItem.Content;
					if(((selectedItem.ContentTemplate != null) || (selectedItem.ContentTemplateSelector != null))) {
						SelectedContentTemplate = selectedItem.ContentTemplate;
						SelectedContentTemplateSelector = selectedItem.ContentTemplateSelector;
					}
					else {
						SetBinding(SelectedContentTemplateProperty, new Binding() { Path = new PropertyPath("ContentTemplate"), Source = this });
						SetBinding(SelectedContentTemplateSelectorProperty, new Binding() { Path = new PropertyPath("ContentTemplateSelector"), Source = this });
					}
				}
				else {
					SelectedContent = SelectedItem;
					SetBinding(SelectedContentTemplateProperty, new Binding() { Path = new PropertyPath("ContentTemplate"), Source = this });
					SetBinding(SelectedContentTemplateSelectorProperty, new Binding() { Path = new PropertyPath("ContentTemplateSelector"), Source = this });
				}
			}
		}
		protected IContentSelectorItem GetSelectedItem() {
			if(SelectedItem == null) return null;
			object selectedItem = SelectedItem;
			IContentSelectorItem item = selectedItem as IContentSelectorItem;
			if(item == null)
				item = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as IContentSelectorItem;
			return item;
		}
		protected override void UpdateSelectionCore() {
			UpdateSelectedContent();
		}
	}
}
