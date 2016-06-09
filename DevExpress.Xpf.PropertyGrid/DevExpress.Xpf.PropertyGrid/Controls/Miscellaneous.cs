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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.PropertyGrid {			
	public class PropertyDescriptionPresenterControl : ContentControl {				
		public static readonly DependencyProperty IsInTooltipProperty;
		public static readonly DependencyProperty ShowSelectedRowHeaderProperty;
		public static readonly DependencyProperty SelectedRowProperty;
		static PropertyDescriptionPresenterControl() {   
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyDescriptionPresenterControl), new FrameworkPropertyMetadata(typeof(PropertyDescriptionPresenterControl)));
			IsInTooltipProperty = DependencyPropertyManager.Register("IsInTooltip", typeof(bool), typeof(PropertyDescriptionPresenterControl), new FrameworkPropertyMetadata(false));
			ShowSelectedRowHeaderProperty = DependencyPropertyManager.Register("ShowSelectedRowHeader", typeof(bool), typeof(PropertyDescriptionPresenterControl), new FrameworkPropertyMetadata(true, (d, e) => ((PropertyDescriptionPresenterControl)d).OnShowSelectedRowHeaderChanged((bool)e.OldValue, (bool)e.NewValue)));
			SelectedRowProperty = DependencyPropertyManager.Register("SelectedRow", typeof(RowDataBase), typeof(PropertyDescriptionPresenterControl), new FrameworkPropertyMetadata(null, (d, e) => ((PropertyDescriptionPresenterControl)d).OnSelectedRowChanged((RowDataBase)e.OldValue, (RowDataBase)e.NewValue)));
		}
		public PropertyDescriptionPresenterControl() {
		}
		public RowDataBase SelectedRow {
			get { return (RowDataBase)GetValue(SelectedRowProperty); }
			set { SetValue(SelectedRowProperty, value); }
		}
		public bool ShowSelectedRowHeader {
			get { return (bool)GetValue(ShowSelectedRowHeaderProperty); }
			set { SetValue(ShowSelectedRowHeaderProperty, value); }
		}
		public bool IsInTooltip {
			get { return (bool)GetValue(IsInTooltipProperty); }
			set { SetValue(IsInTooltipProperty, value); }
		}
		protected virtual void OnSelectedRowChanged(RowDataBase oldValue, RowDataBase newValue) {
		}
		protected virtual void OnShowSelectedRowHeaderChanged(bool oldValue, bool newValue) {
		}
	}
	public class PropertyGridViewKindSelectorControl : Control, INavigationSupport {
		public static readonly DependencyProperty ShowCategoriesProperty;
		public static readonly DependencyProperty PropertyGridProperty;		
		static PropertyGridViewKindSelectorControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridViewKindSelectorControl), new FrameworkPropertyMetadata(typeof(PropertyGridViewKindSelectorControl)));
			ShowCategoriesProperty = DependencyPropertyManager.Register("ShowCategories", typeof(bool), typeof(PropertyGridViewKindSelectorControl), new FrameworkPropertyMetadata(true, (d, e) => ((PropertyGridViewKindSelectorControl)d).OnShowCategoriesChanged((bool)e.OldValue)));
			PropertyGridProperty = DependencyPropertyManager.Register("PropertyGrid", typeof(PropertyGridControl), typeof(PropertyGridViewKindSelectorControl), new FrameworkPropertyMetadata(null));
			EventManager.RegisterClassHandler(typeof(PropertyGridViewKindSelectorControl), FrameworkElement.MouseUpEvent, new MouseButtonEventHandler((o, args) => ((PropertyGridViewKindSelectorControl)o).OnMouseUpCore(args)), true);
		}		
		public bool ShowCategories {
			get { return (bool)GetValue(ShowCategoriesProperty); }
			set { SetValue(ShowCategoriesProperty, value); }
		}
		public PropertyGridControl PropertyGrid {
			get { return (PropertyGridControl)GetValue(PropertyGridProperty); }
			set { SetValue(PropertyGridProperty, value); }
		}
		public PropertyGridViewKindSelectorControl() {
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.SelectView, new ExecutedRoutedEventHandler(OnSelectViewCommandExecuted), new CanExecuteRoutedEventHandler(OnSelectViewCommandCanExecute)));
		}
		protected RadioButton ShowCategoriesButton { get; private set; }
		protected RadioButton HideCategoriesButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ShowCategoriesButton = GetTemplateChild("PART_ShowCategories") as RadioButton;
			HideCategoriesButton = GetTemplateChild("PART_HideCategories") as RadioButton;			
			UpdateButtons();
		}
		protected internal void OnSelectViewCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
			ShowCategories = ((ApplyingMode)e.Parameter) == ApplyingMode.WhenGrouping;
			UpdateButtons();
		}
		protected internal void OnSelectViewCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
		protected virtual void OnShowCategoriesChanged(bool oldValue) {
			UpdateButtons();
		}
		private void UpdateButtons() {
			(ShowCategories ? ShowCategoriesButton : HideCategoriesButton).Do(x => x.IsChecked = true);
		}				
		protected virtual void OnMouseUpCore(MouseEventArgs args) {
			PropertyGrid.With(x => x.View).With(x => x.CellEditorOwner).Do(x => x.ProcessIsKeyboardFocusWithinChanged());
		}
		#region INavigationSupport Members
		bool INavigationSupport.ProcessNavigation(NavigationDirection direction) {
			return false;
		}
		IEnumerable<FrameworkElement> INavigationSupport.GetChildren() {
			return null;
		}
		FrameworkElement INavigationSupport.GetParent() {
			return null;
		}
		bool INavigationSupport.GetSkipNavigation() {
			return true;
		}
		bool INavigationSupport.GetUseLinearNavigation() {
			return false;
		}
		#endregion
	}	
	public class MergedEnumerator<T> : IEnumerator<T> where T : class {
		IEnumerator<IEnumerator<T>> sourceEnumerator;
		public MergedEnumerator(params IEnumerator<T>[] args) {
			sourceEnumerator = args.ToList().GetEnumerator();
		}
		public T Current {
			get { return sourceEnumerator.Current.With(x=>x.Current); }
		}
		public void Dispose() {
			sourceEnumerator.Dispose();
		}
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (sourceEnumerator.Current == null)
				sourceEnumerator.MoveNext();
			if (sourceEnumerator.Current == null)
				return false;
			if (!sourceEnumerator.Current.MoveNext()) {
				sourceEnumerator.MoveNext();
				if(sourceEnumerator.Current==null)
					return false;
			}
			else
				return true;
			return sourceEnumerator.Current.MoveNext();
		}
		public void Reset() {
			sourceEnumerator.Reset();
			while(sourceEnumerator.MoveNext())
				sourceEnumerator.Current.Reset();
			sourceEnumerator.Reset();
		}
	}
	public class SingleElementEnumerator<T> : IEnumerator<T> where T: class {
		T source = null;
		bool useSource = false;
		public SingleElementEnumerator(T source) {
			this.source = source;   
		}
		public T Current {
			get { return useSource ? source : null; }
		}
		public void Dispose() {
			source = null;
			useSource = false;
		}
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (useSource || source==null)
				return false;
			useSource = true;
			return true;
		}
		public void Reset() {
			useSource = false;
		}
	}
	public static class EnumeratorExtensions {
		public static IEnumerator<T> OfType<T>(this System.Collections.IEnumerator enumerator) where T : class {
			return enumerator == null ? (IEnumerator<T>)new EmptyEnumerator<T>() : (IEnumerator<T>)new GenericEnumeratorWrapper<T>(enumerator);
		}
		class GenericEnumeratorWrapper<T> : IEnumerator<T> where T : class {
			System.Collections.IEnumerator source;
			public GenericEnumeratorWrapper(System.Collections.IEnumerator source) { this.source = source; }
			public T Current { get { return (T)source.Current; } }
			public void Dispose() { source = null; }
			object System.Collections.IEnumerator.Current { get { return source.Current; } }
			public bool MoveNext() { return source.MoveNext(); }
			public void Reset() { source.Reset(); }
		}
		class EmptyEnumerator<T> : IEnumerator<T> where T: class {
			T IEnumerator<T>.Current { get { return null; } }
			void IDisposable.Dispose() { }
			object System.Collections.IEnumerator.Current { get { return null; } }
			bool System.Collections.IEnumerator.MoveNext() { return false; }
			void System.Collections.IEnumerator.Reset() { }
		}
	}		
	[DXToolboxBrowsable(false)]
	public class PropertyGridSearchControl : DevExpress.Xpf.Editors.SearchControl {
		static PropertyGridSearchControl() {
			Type ownerType = typeof(PropertyGridSearchControl);						
			NavigationManager.NavigationModeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(NavigationMode.Auto));			
		}
		protected internal virtual bool GetIsPopupOpened() {
			var editorControl = GetEditorControl() as DevExpress.Xpf.Editors.PopupBaseEdit;
			return editorControl != null && editorControl.IsPopupOpen;
		}
	}
	public class PropertyGridVirtualizationModeExtension : MarkupExtension {
		static VirtualizationMode mode;
		static PropertyGridVirtualizationModeExtension() {
			mode = !Net45Detector.IsNet45() ? VirtualizationMode.Standard : VirtualizationMode.Recycling;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return mode;
		}
	}
	public enum CollectionButtonKind { Add, Remove };
}
