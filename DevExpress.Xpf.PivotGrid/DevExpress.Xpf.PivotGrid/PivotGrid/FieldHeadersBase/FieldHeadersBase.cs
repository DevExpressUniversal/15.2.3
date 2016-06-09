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
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Data;
using System.Collections.Specialized;
using PivotFieldsObservableCollection = DevExpress.Xpf.Core.ObservableCollectionCore<DevExpress.Xpf.PivotGrid.PivotGridField>;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
#if SL
using ApplicationException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	[
	TemplateVisualState(GroupName = FieldHeadersBase.DragState, Name = FieldHeadersBase.DragOverState),
	TemplateVisualState(GroupName = FieldHeadersBase.DragState, Name = FieldHeadersBase.DragLeaveState),
	]
	public abstract class FieldHeadersBase : Control, IDragOverHandler, IFieldHeaders {
		const string DragState = "Drag", DragOverState = "DragOver", DragLeaveState = "DragLeave";
		IHeadersFieldsSource fieldsSource;
		#region static stuff
		public static readonly DependencyProperty AreaProperty;
		public static readonly DependencyProperty ActualAreaProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty IsEmptyProperty;
		protected static readonly DependencyPropertyKey IsEmptyPropertyKey;
		public static readonly DependencyProperty EnableDragDropProperty;
		public static readonly DependencyProperty EnableHeaderMenuProperty;
		public static readonly DependencyProperty PivotGridProperty;
		public static readonly DependencyProperty ShowListSourceProperty;
		public static readonly DependencyProperty FieldListAreaProperty;
		public static readonly DependencyProperty DropPlaceProperty;
		static FieldHeadersBase() {
			Type ownerType = typeof(FieldHeadersBase);
			AreaProperty = DependencyPropertyManager.Register("Area", typeof(FieldListArea), ownerType, new UIPropertyMetadata(FieldListArea.FilterArea, (d, e) => ((FieldHeadersBase)d).OnAreaChanged()));
			ActualAreaProperty = DependencyPropertyManager.Register("ActualArea", typeof(FieldListActualArea), ownerType, new UIPropertyMetadata(FieldListActualArea.FilterArea));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(PivotFieldsReadOnlyObservableCollection), ownerType, new UIPropertyMetadata((d, e) => ((FieldHeadersBase)d).OnItemsSourceChanged(e)));
			IsEmptyPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsEmpty", typeof(bool), ownerType, new UIPropertyMetadata(false, (d, e) => ((FieldHeadersBase)d).OnIsEmptyChanged()));
			IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;
			EnableDragDropProperty = DependencyPropertyManager.Register("EnableDragDrop", typeof(bool), ownerType, new UIPropertyMetadata(false, (d, e) => ((FieldHeadersBase)d).EnableDragDropChanged()));
			EnableHeaderMenuProperty = DependencyPropertyManager.Register("EnableHeaderMenu", typeof(bool), ownerType, new UIPropertyMetadata(false));
			PivotGridProperty = DependencyPropertyManager.Register("PivotGrid", typeof(PivotGridControl), ownerType, new UIPropertyMetadata((d, e) => ((FieldHeadersBase)d).OnPivotGridChanged()));
			ShowListSourceProperty = DependencyPropertyManager.Register("ShowListSource", typeof(bool), ownerType, new UIPropertyMetadata(false, (d, e) => ((FieldHeadersBase)d).OnShowListSourceChanged()));
			FieldListAreaProperty = DependencyPropertyManager.RegisterAttached("FieldListArea", typeof(FieldListArea),
					ownerType, new FrameworkPropertyMetadata(FieldListArea.FilterArea, FrameworkPropertyMetadataOptions.None, (d, e)=> OnFieldListAreaChanged(d,e)));
			DropPlaceProperty = DependencyPropertyManager.RegisterAttached("DropPlace", typeof(DropPlace),
					ownerType, new FrameworkPropertyMetadata(DropPlace.None, FrameworkPropertyMetadataOptions.None));
		}
		static void OnFieldListAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FieldHeader header = d as FieldHeader;
			if(header != null && header.Field != null && header.Field.Group != null)
				header.OnFieldChanged(header.Field);
		}
		public static FieldListArea GetFieldListArea(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (FieldListArea)element.GetValue(FieldListAreaProperty);
		}
		public static void SetFieldListArea(DependencyObject element, FieldListArea value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(FieldListAreaProperty, value);
		}
		public static DropPlace GetDropPlace(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (DropPlace)element.GetValue(DropPlaceProperty);
		}
		public static void SetDropPlace(DependencyObject element, DropPlace value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DropPlaceProperty, value);
		}
		#endregion
		public FieldHeadersBase() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public FieldListArea Area {
			get { return (FieldListArea)GetValue(AreaProperty); }
			set { SetValue(AreaProperty, value); }
		}
		public FieldListActualArea ActualArea {
			get { return (FieldListActualArea)GetValue(ActualAreaProperty); }
			set { SetValue(ActualAreaProperty, value); }
		}
		public bool IsEmpty {
			get { return (bool)GetValue(IsEmptyProperty); }
			protected set { this.SetValue(IsEmptyPropertyKey, value); }
		}
		public bool EnableDragDrop {
			get { return (bool)GetValue(EnableDragDropProperty); }
			set { SetValue(EnableDragDropProperty, value); }
		}
		public bool EnableHeaderMenu {
			get { return (bool)GetValue(EnableHeaderMenuProperty); }
			set { SetValue(EnableHeaderMenuProperty, value); }
		}
		public PivotFieldsReadOnlyObservableCollection ItemsSource {
			get { return (PivotFieldsReadOnlyObservableCollection)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public PivotGridControl PivotGrid {
			get { return (PivotGridControl)GetValue(PivotGridProperty); }
			set { SetValue(PivotGridProperty, value); }
		}
		public bool ShowListSource {
			get { return (bool)GetValue(ShowListSourceProperty); }
			set { SetValue(ShowListSourceProperty, value); }
		}
		public PivotGridWpfData Data { get { return PivotGrid != null ? PivotGrid.Data : null; } }
		protected IHeadersFieldsSource FieldsSource {
			get {
				if(fieldsSource == null)
					FieldsSource = CreateFieldsSource();
				return fieldsSource;
			}
			set {
				if(fieldsSource != null)
					fieldsSource.Dispose();
				fieldsSource = value;
			}
		}
		protected virtual void OnAreaChanged() {
			RecreateFieldsSource();
			EnsureHasItems();
			UpdateFieldListActualArea();
		}
		public virtual bool GetActualShowAll() {
			return false;
		}
		protected virtual void OnActualShowAllChanged() {
			UpdateFieldListActualArea();
		}
		protected void UpdateFieldListActualArea(){
			if(Area == FieldListArea.All)
				if(GetActualShowAll())
					ActualArea = FieldListActualArea.AllFields;
				else
					ActualArea = FieldListActualArea.HiddenFields;
			else
				ActualArea = (FieldListActualArea)Area;
		}
		protected abstract void OnIsEmptyChanged();
		protected abstract System.Windows.Controls.Control GetEmtyStateElement();
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			FieldsSource.OnLoaded();
			SubscribeCollectionChangedEvent(ItemsSource);
			AddDragDropHandler();
			EnsureHasItems();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			FieldsSource.OnUnloaded();
			UnSubscribeCollectionChangedEvent(ItemsSource);
			OnItemsSourceCollectionChanged(null, null);
			RemoveDragDropHandler();
		}
		#region dragDrop
		void IDragOverHandler.DragOver(bool over) {
			if(GetEmtyStateElement() != null)
				VisualStateManager.GoToState(GetEmtyStateElement(), over ? DragOverState : DragLeaveState, false);
		}
		void EnableDragDropChanged() {
			if(IsLoaded)
				if(EnableDragDrop)
					AddDragDropHandler();
				else
					RemoveDragDropHandler();
		}
		void AddDragDropHandler() {
			if(EnableDragDrop && Data != null)
				Data.PivotGrid.RegisteredFieldListControl(this);
		}
		void RemoveDragDropHandler() {
			if(Data != null)
				Data.PivotGrid.UnregisteredFieldListControl(this);
		}
		#endregion
		#region isEmpty
		protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			SubscribeCollectionChangedEvent((PivotFieldsReadOnlyObservableCollection)e.NewValue);
			UnSubscribeCollectionChangedEvent((PivotFieldsReadOnlyObservableCollection)e.OldValue);
			EnsureHasItems();
		}
		bool subscribed;
		void SubscribeCollectionChangedEvent(PivotFieldsReadOnlyObservableCollection newValue) {
			if(newValue == null)
				return;
			if(ItemsSource as INotifyCollectionChanged != null && !subscribed) {
				((INotifyCollectionChanged)ItemsSource).CollectionChanged += OnItemsSourceCollectionChanged;
				subscribed = true;
			}
		}
		void UnSubscribeCollectionChangedEvent(PivotFieldsReadOnlyObservableCollection oldValue) {
			if(oldValue == null)
				return;
			if(ItemsSource as INotifyCollectionChanged != null) {
				((INotifyCollectionChanged)ItemsSource).CollectionChanged -= OnItemsSourceCollectionChanged;
				subscribed = false;
			}
		}
		protected virtual void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			EnsureHasItems();
		}
		void EnsureHasItems() {
			IsEmpty = ItemsSource == null || ItemsSource.Count == 0;
		}
		#endregion
		#region items
		protected virtual void OnPivotGridChanged() {
			PivotGridControl.SetPivotGrid(this, PivotGrid);
			EnsureItems();
		}
		protected virtual void EnsureItems() {
			FieldsSource.EnsureItems();
		}
		void IFieldHeaders.SetItems(PivotFieldsReadOnlyObservableCollection fields) {
			ItemsSource = fields;
		}
		void OnShowListSourceChanged() {
			RecreateFieldsSource();
		}
		void RecreateFieldsSource() {
			FieldsSource = null;
			FieldsSource.EnsureItems();
		}
		protected virtual IHeadersFieldsSource CreateFieldsSource() {
			return ShowListSource ?  (IHeadersFieldsSource)(new FieldListFieldsSoure(this)) : new RealFieldsSource(this);
		}
		#endregion
	}
}
