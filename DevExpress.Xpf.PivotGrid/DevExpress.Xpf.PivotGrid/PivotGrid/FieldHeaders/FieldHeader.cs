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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Utils;
using System.Windows.Media;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public enum HeaderPosition { Left, Middle, Right, Single, RightPreGroup, MiddlePreGroup };
	public enum FirstHeaderPosition { None, RowArea, ColumnArea };
	public abstract class FieldHeaderBase : Control, ISupportDragDrop, ISupportDragDropColumnHeader, ISupportDragDropPlatformIndependent {
		#region static stuff
		public static readonly string DragElementTemplatePropertyName = "DragElementTemplate";
		public static readonly DependencyProperty FieldProperty; 
		public static readonly DependencyProperty DragElementTemplateProperty;
		static readonly DependencyPropertyKey DragElementSizePropertyKey;
		public static readonly DependencyProperty DragElementSizeProperty;
		public static readonly DependencyProperty ChangeFieldSortOrderProperty;
		static FieldHeaderBase() {
			Type ownerType = typeof(FieldHeaderBase);
			FieldProperty = DependencyPropertyManager.RegisterAttached("Field", typeof(PivotGridField),
				ownerType, new FrameworkPropertyMetadata(null,
					FrameworkPropertyMetadataOptions.AffectsParentMeasure,
					OnFieldPropertyChanged));
			DragElementTemplateProperty = DependencyPropertyManager.Register("DragElementTemplate", typeof(DataTemplate),
				ownerType, new UIPropertyMetadata(null));
			DragElementSizePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("DragElementSize", typeof(Size),
				ownerType, new UIPropertyMetadata(default(Size)));
			DragElementSizeProperty = DragElementSizePropertyKey.DependencyProperty;
			ChangeFieldSortOrderProperty = DependencyPropertyManager.Register("ChangeFieldSortOrder", typeof(ICommand),
				ownerType, new UIPropertyMetadata(null));
		}
		static void OnFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FieldHeaderBase header = d as FieldHeaderBase;
			if(header != null)
				header.OnFieldChanged(e.OldValue as PivotGridField);
		}
		#endregion
		protected FieldHeaderBase() {
			CanDragCore = true;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			DragDropHelper = null;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(DragDropHelper == null && VisualTreeHelper.GetParent(this) != null)
				CreateDragDropElementHelper();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateDragDropElementHelper();
		}
		DragDropElementHelper dragDropHelper;
		public DragDropElementHelper DragDropHelper {
			get { return dragDropHelper; }
			private set {
				if(dragDropHelper != null)
					dragDropHelper.Destroy();
				dragDropHelper = value;
			}
		}
		public FrameworkElement HeaderButton { get; protected set; }
		public bool CanDragCore { get; set; }
		public PivotGridField Field {
			get { return GetField(this); }
			set { SetField(this, value); }
		}
		public PivotGridControl PivotGrid {
			get { return Field != null ? Field.PivotGrid : null; }
		}
		public PivotGridWpfData Data {
			get { return Field != null ? Field.Data : null; }
		}
		public ICommand ChangeFieldSortOrder {
			get { return (ICommand)GetValue(ChangeFieldSortOrderProperty); }
			set { SetValue(ChangeFieldSortOrderProperty, value); }
		}
		public DataTemplate DragElementTemplate {
			get { return (DataTemplate)GetValue(DragElementTemplateProperty); }
			set { SetValue(DragElementTemplateProperty, value); }
		}
		public static Size GetDragElementSize(DependencyObject obj) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			return (Size)obj.GetValue(DragElementSizeProperty);
		}
		internal static void SetDragElementSize(DependencyObject obj, Size value) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			obj.SetValue(DragElementSizePropertyKey, value);
		}
		public static PivotGridField GetField(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PivotGridField)element.GetValue(FieldProperty);
		}
		public static void SetField(DependencyObject element, PivotGridField value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(FieldProperty, value);
		}
		protected internal virtual void OnFieldChanged(PivotGridField oldField) {
			EnsureChangeFieldSortCommand();
		}
		protected virtual void EnsureChangeFieldSortCommand() {
			EnsureChangeFieldSortCommand(true);
		}
		protected virtual void EnsureChangeFieldSortCommand(bool can) {
			ChangeFieldSortOrder = Field != null && can ? FieldSortCommand() : null;
		}
		ICommand FieldSortCommand() {
#if !SL
			return PivotGrid.Commands.ChangeFieldSortOrder;
#else
			return DelegateCommandFactory.Create<PivotGridField>(PivotGrid.OnChangeFieldSortOrder, delegate(PivotGridField field) { return DragDropHelper == null || DragDropHelper.DragElement == null; }, false);
#endif
		}
		protected virtual void CreateDragDropElementHelper() {
			DragDropHelper = new PivotDragDropElementHelper(this);
		}
		internal class PivotDragDropElementHelper : DragDropElementHelper {
			bool capturingMouse = false;
			FieldHeaderBase header;
			DataAreaPopupEdit lastEdit;
			public PivotDragDropElementHelper(FieldHeaderBase supportDragDrop, bool isRelativeMode = true)
				: base(supportDragDrop, isRelativeMode) {
					this.header = supportDragDrop;
			}
			protected override void OnLostMouseCapture(object sender, MouseEventArgs e) {
				BaseEdit edit = null;
				if(MouseHelper.Captured is DependencyObject)
					edit = MouseHelper.Captured != null ? BaseEdit.GetOwnerEdit((DependencyObject)MouseHelper.Captured) : null;
				if(edit is DataAreaPopupEdit && ((DataAreaPopupEdit)edit).IsPopupOpen && capturingMouse == false && !object.ReferenceEquals(sender, MouseHelper.Captured) && !(SourceElement is DataAreaPopupEdit)) {
					capturingMouse = true;
					SourceElement.CaptureMouse();
					capturingMouse = false;
				} else {
#if !SL
					if(!(SourceElement is DataAreaPopupEdit) && !(edit is DataAreaPopupEdit))
#endif
					base.OnLostMouseCapture(sender, e); 
					capturingMouse = false;
				}
			}
			protected override void StartDragging(Point offset, IndependentMouseEventArgs e) {
				RemoveDragElement();
				base.StartDragging(offset, e);
				if(header.Field == null || header.Field.PivotGrid == null)
					return;
				if(header.Field.PivotGrid.CurrenDragDropElementHelper != this && header.Field.PivotGrid.CurrenDragDropElementHelper != null)
					header.Field.PivotGrid.CurrenDragDropElementHelper.RemoveDragElement();
				header.Field.PivotGrid.CurrenDragDropElementHelper = this;
			}
			protected override void EndDragging(IndependentMouseButtonEventArgs e) {
				PivotGridControl oldPivot = header.Field == null ? null : header.Field.PivotGrid;
				base.EndDragging(e);
				if(oldPivot == null)
					return;
				if(oldPivot.DesignTimeAdorner != null)
					oldPivot.DesignTimeAdorner.PerformDragDrop();
				if(header.Field == null || header.Field.PivotGrid == null)
					return;
				header.Field.PivotGrid.CurrenDragDropElementHelper = null;
			}
			internal void RecreatePopup() {
				if(!IsDragging)
					return;
				RemoveDragElement();
				DragElement = ((ISupportDragDrop)header).CreateDragElement(DragElementLocation);
				UpdateDragElementLocation(DragElementLocation);
			}
			protected override void UpdateDragElementLocation(Point newPos) {
#if !SL
				if(SystemParameters.MenuDropAlignment) {
					DataAreaPopupEdit edit = CurrentDropTargetElement == null ? null : BaseEdit.GetOwnerEdit(CurrentDropTargetElement) as DataAreaPopupEdit;
					if(edit == null || !edit.IsPopupOpenCore) {
						if(header.FlowDirection == FlowDirection.RightToLeft) 
							newPos.X -= header.ActualWidth + HeaderDragElementBase.RemoveColumnDragIconMargin;
						else
							newPos.X += header.ActualWidth + HeaderDragElementBase.RemoveColumnDragIconMargin;
					}
				}
#endif
				base.UpdateDragElementLocation(newPos);
				if(lastEdit == null) {
					lastEdit = CurrentDropTargetElement as DataAreaPopupEdit;
				} else {
					if(CurrentDropTargetElement != lastEdit && (CurrentDropTargetElement == null || BaseEdit.GetOwnerEdit(CurrentDropTargetElement) != lastEdit)) {
						lastEdit.IsSelfMouseOver = false;
						lastEdit = null;
					}
				}
			}
		}
		#region ISupportDragDrop Members
		protected abstract IDragElement CreateDragElementCore(Point offset);
		bool ISupportDragDrop.CanStartDrag(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			return CanDrag;
		}
		protected virtual bool CanDrag {
			get {
				if(Field == null || !CanDragCore) return false;
				if(FieldHeadersBase.GetFieldListArea(this) == FieldListArea.All)
					return Field.CanDragInCustomizationForm;
				else
					return Field.CanDrag;
			}
		}
		IDragElement ISupportDragDrop.CreateDragElement(Point offset) {
			return CreateDragElementCore(offset);
		}
		IDropTarget ISupportDragDrop.CreateEmptyDropTarget() {
			return new RemoveColumnDropTarget();
		}
		IEnumerable<UIElement> ISupportDragDrop.GetTopLevelDropContainers() {
			return PivotGrid.GetTopLevelDropContainers();
		}
		FrameworkElement ISupportDragDrop.SourceElement {
			get { return this; }
		}
		bool ISupportDragDrop.IsCompatibleDropTargetFactory(IDropTargetFactory factory, UIElement dropTargetElement) {
			return factory is PivotGridDropTargetFactoryExtension;
		}
		FrameworkElement ISupportDragDropColumnHeader.RelativeDragElement {
			get { return VisualTreeHelper.GetChildrenCount(this) > 0 ? (FrameworkElement)VisualTreeHelper.GetChild(this, 0) : null; }
		}
		FrameworkElement ISupportDragDropColumnHeader.TopVisual { get { return (FrameworkElement)LayoutHelper.GetTopLevelVisual(this); } }
		#endregion
		bool ISupportDragDropPlatformIndependent.CanStartDrag(object sender, IndependentMouseButtonEventArgs e) {
			return CanDrag;
		}
		void ISupportDragDropColumnHeader.UpdateLocation(IndependentMouseEventArgs e) { }
		public bool SkipHitTestVisibleChecking {
			get { return PivotGrid == null || PivotGrid.DesignTimeAdorner == null ? false : PivotGrid.DesignTimeAdorner.IsDesignTime; }
		}
	}
	public class FieldHeader : FieldHeaderBase {
		#region static stuff
		internal const string TemplatePartHeaderButtonName = "PART_HeaderButton",
			TemplatePartPreviousDropPlace = "PART_PreviousDropPlace",
			TemplatePartNextDropPlace = "PART_NextDropPlace";
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty FilterPopupTemplateProperty;
		static readonly DependencyPropertyKey ActualAreaIndexPropertyKey;
		public static readonly DependencyProperty ActualAreaIndexProperty;
		protected static readonly DependencyPropertyKey HeaderPositionPropertyKey;
		public static readonly DependencyProperty HeaderPositionProperty;
		static readonly DependencyPropertyKey HasGroupPropertyKey;
		public static readonly DependencyProperty HasGroupProperty;
		static readonly DependencyPropertyKey ContentPropertyKey;
		public static readonly DependencyProperty ContentProperty;
		static readonly DependencyPropertyKey IsFilterButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsFilterButtonVisibleProperty;
		static readonly DependencyPropertyKey CanFilterButtonVisiblePropertyKey;
		public static readonly DependencyProperty CanFilterButtonVisibleProperty;
		static readonly DependencyPropertyKey IsSortUpButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsSortUpButtonVisibleProperty;
		static readonly DependencyPropertyKey IsSortDownButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsSortDownButtonVisibleProperty;
		static readonly DependencyPropertyKey IsFirstPropertyKey;
		public static readonly DependencyProperty IsFirstProperty;
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty IsSelectedInDesignTimeProperty;
		static FieldHeader() {
			Type ownerType = typeof(FieldHeader);
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(ControlTemplate),
				ownerType, new UIPropertyMetadata(null));
			FilterPopupTemplateProperty = DependencyPropertyManager.Register("FilterPopupTemplate", typeof(ControlTemplate),
				ownerType, new UIPropertyMetadata(null));
			ActualAreaIndexPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualAreaIndex", typeof(int),
				ownerType, new FrameworkPropertyMetadata(-1));
			ActualAreaIndexProperty = ActualAreaIndexPropertyKey.DependencyProperty;
			HeaderPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("HeaderPosition", typeof(HeaderPosition),
				ownerType, new PropertyMetadata((d, e) => ((FieldHeader)d).OnHeaderPositionPropertyChanged((HeaderPosition)e.NewValue)));
			HeaderPositionProperty = HeaderPositionPropertyKey.DependencyProperty;
			HasGroupPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("HasGroup", typeof(bool),
				ownerType, new UIPropertyMetadata(false));
			HasGroupProperty = HasGroupPropertyKey.DependencyProperty;
			ContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("Content", typeof(object),
				ownerType, new UIPropertyMetadata((d,e) => ((FieldHeader)d).OnContentChanged()));
			ContentProperty = ContentPropertyKey.DependencyProperty;
			IsFilterButtonVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsFilterButtonVisible", typeof(Visibility),
				ownerType, new UIPropertyMetadata(Visibility.Collapsed));
			IsFilterButtonVisibleProperty = IsFilterButtonVisiblePropertyKey.DependencyProperty;
			CanFilterButtonVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("CanFilterButtonVisible", typeof(bool),
				ownerType, new UIPropertyMetadata(false));
			CanFilterButtonVisibleProperty = CanFilterButtonVisiblePropertyKey.DependencyProperty;
			IsSortUpButtonVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsSortUpButtonVisible", typeof(Visibility),
				ownerType, new UIPropertyMetadata(Visibility.Collapsed));
			IsSortUpButtonVisibleProperty = IsSortUpButtonVisiblePropertyKey.DependencyProperty;
			IsSortDownButtonVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsSortDownButtonVisible", typeof(Visibility),
				ownerType, new UIPropertyMetadata(Visibility.Collapsed));
			IsSortDownButtonVisibleProperty = IsSortDownButtonVisiblePropertyKey.DependencyProperty;
			IsFirstPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsFirst", typeof(FirstHeaderPosition), ownerType, new UIPropertyMetadata(FirstHeaderPosition.None));
			IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
			DisplayTextProperty = DependencyPropertyManager.Register("DisplayText", typeof(string), ownerType, new UIPropertyMetadata());
			IsSelectedInDesignTimeProperty = DependencyPropertyManager.Register("IsSelectedInDesignTime", typeof(bool), ownerType, new UIPropertyMetadata(false, (d,e) => ((FieldHeader)d).OnIsSelectedInDesignTimeChanged() ));
		}
		protected virtual void OnContentChanged() {
			if(Content != null)
				SetBinding(DisplayTextProperty, new Binding("DisplayText") { Source = Content });
			else
				SetValue(DisplayTextProperty, string.Empty);
		}
		public static int GetActualAreaIndex(DependencyObject obj) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			return (int)obj.GetValue(ActualAreaIndexProperty);
		}
		internal static void SetActualAreaIndex(DependencyObject obj, int value) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			obj.SetValue(ActualAreaIndexPropertyKey, value);
		}
		public static HeaderPosition GetHeaderPosition(DependencyObject obj) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			return (HeaderPosition)obj.GetValue(HeaderPositionProperty);
		}
		static void SetHeaderPosition(DependencyObject obj, HeaderPosition value) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			obj.SetValue(HeaderPositionPropertyKey, value);
		}
		public static bool GetHasGroup(DependencyObject obj) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			return (bool)obj.GetValue(HasGroupProperty);
		}
		internal static void SetHasGroup(DependencyObject obj, bool value) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			obj.SetValue(HasGroupPropertyKey, value);
		}
		public static object GetContent(DependencyObject obj) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			return obj.GetValue(ContentProperty);
		}
		internal static void SetContent(DependencyObject obj, object value) {
			if(obj == null)
				throw new ArgumentNullException("obj");
			obj.SetValue(ContentPropertyKey, value);
		}
		#endregion
		public FieldHeader() {
			SetDefaultStyleKey();
		}
		protected virtual void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(FieldHeader));
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			SetBinding(BarManager.DXContextMenuProperty, new Binding("GridMenu") { Source = PivotGrid });
			SubscribeEvents();
			UpdateIsSortButtonVisible();
			UpdateIsFilterButtonVisible();
			UpdateWidthBinding();
			UpdateIsFirst();
			SetContent(this, GetContentCore(Field));
		}  
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			ClearValue(BarManager.DXContextMenuProperty);
			UnsubscribeEvents();
		}
		public FrameworkElement PreviousDropPlace { get; private set; }
		public FrameworkElement NextDropPlace { get; private set; }
		public ControlTemplate ContentTemplate {
			get { return (ControlTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public HeaderPosition HeaderPosition {
			get { return GetHeaderPosition(this); }
			internal set { SetHeaderPosition(this, value); }
		}
		public bool HasGroup {
			get { return GetHasGroup(this); }
			internal set { SetHasGroup(this, value); }
		}
		public object Content {
			get { return GetContent(this); }
			internal set { SetContent(this, value); }
		}
		public ControlTemplate FilterPopupTemplate {
			get { return (ControlTemplate)GetValue(FilterPopupTemplateProperty); }
			set { SetValue(FilterPopupTemplateProperty, value); }
		}
		public Visibility IsFilterButtonVisible {
			get { return (Visibility)GetValue(IsFilterButtonVisibleProperty); }
			internal set { this.SetValue(IsFilterButtonVisiblePropertyKey, value); }
		}
		public bool CanFilterButtonVisible {
			get { return (bool)GetValue(CanFilterButtonVisibleProperty); }
			internal set { this.SetValue(CanFilterButtonVisiblePropertyKey, value); }
		}
		public Visibility IsSortUpButtonVisible {
			get { return (Visibility)GetValue(IsSortUpButtonVisibleProperty); }
			internal set { this.SetValue(IsSortUpButtonVisiblePropertyKey, value); }
		}
		public Visibility IsSortDownButtonVisible {
			get { return (Visibility)GetValue(IsSortDownButtonVisibleProperty); }
			internal set { this.SetValue(IsSortDownButtonVisiblePropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FirstHeaderPosition IsFirst {
			get { return (FirstHeaderPosition)GetValue(IsFirstProperty); }
			internal set { this.SetValue(IsFirstPropertyKey, value); }
		}
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			set { this.SetValue(DisplayTextProperty, value); }
		}
		public bool IsSelectedInDesignTime {
			get { return (bool)GetValue(IsSelectedInDesignTimeProperty); }
			set { SetValue(IsSelectedInDesignTimeProperty, value); }
		}
#if DEBUGTEST
		public
#endif
		FilterPopupEdit FilterPopup {
			get {
				return LayoutHelper.FindElement(this, (d) => d is FilterPopupEdit) as FilterPopupEdit;
			}
		}
		void UpdateIsFirst() {
		   IsFirst = GetIsFirst();
		}
		protected virtual FirstHeaderPosition GetIsFirst() {
			if(!GetIsNotFieldListHeader() || !(HeaderPosition == Internal.HeaderPosition.Left || HeaderPosition == Internal.HeaderPosition.Single))
				return FirstHeaderPosition.None;
			if(Field.Area == FieldArea.RowArea) {
				return FirstHeaderPosition.RowArea;
			} else {
				if(Field.Area == FieldArea.ColumnArea && Field.Group == null)
					return FirstHeaderPosition.ColumnArea;
				else
					return FirstHeaderPosition.None;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			HeaderButton = GetTemplateChild(TemplatePartHeaderButtonName) as FrameworkElement;
			PreviousDropPlace = GetTemplateChild(TemplatePartPreviousDropPlace) as FrameworkElement;
			NextDropPlace = GetTemplateChild(TemplatePartNextDropPlace) as FrameworkElement;
			UpdateFilterPopup();
		}
	   void UpdateFilterPopup() {
			UpdateIsSortButtonVisible();
			UpdateIsFilterButtonVisible();
			if(Field == null) return;
			SubscribeEvents();
		}
		protected internal override void OnFieldChanged(PivotGridField oldField) {
			if(Field == null)
				UnsubscribeEvents(oldField);
			if(oldField == null && IsLoaded)
				SubscribeEvents();
			UpdateFilterPopup();
			SetHasGroup(this, Field != null && Field.Group != null);
			SetContent(this, GetContentCore(Field));
			UpdateWidthBinding();
#if SL
			SetValue(IsMouseOverProperty, false);
#endif
			UpdateIsFirst();
			UpdateIsFilterButtonVisible();
		}
		protected virtual object GetContentCore(PivotGridField field) {
			if(field == null || field.Group == null || FieldHeadersBase.GetFieldListArea(this) != FieldListArea.All)
				return field;
			else
				return field.Group;
		}
	   bool eventsSubscribed;
	   protected void SubscribeEvents() {
		   SubscribeEvents(null);
	   }
	   protected virtual void SubscribeEvents(PivotGridField field) {
		   PivotGridWpfData curData = Data;
		   if(curData == null && field != null)
			   curData = field.Data;
		   if(curData != null && !eventsSubscribed) {
			   curData.PivotGrid.PropertyChanged += OnPivotGridPropertyChanged;
			   curData.FieldListFields.DeferUpdatesChanged += OnFieldListFieldsDeferUpdatesChanged;
			   eventsSubscribed = true;
			}
		}
	   protected void UnsubscribeEvents() {
		   UnsubscribeEvents(null);
	   }
	   protected virtual void UnsubscribeEvents(PivotGridField field) {
		   PivotGridWpfData curData = Data;
		   if(curData == null && field != null)
			   curData = field.Data;
		   if(eventsSubscribed && curData != null) {
			   eventsSubscribed = false;
			   curData.PivotGrid.PropertyChanged -= OnPivotGridPropertyChanged;
			   curData.FieldListFields.DeferUpdatesChanged -= OnFieldListFieldsDeferUpdatesChanged;
		   }
	   }
	   void OnFieldListFieldsDeferUpdatesChanged(object sender, EventArgs e) {
		   if(!IsFieldListHeader)
			   return;
		   UpdateIsFilterButtonVisible();
	   }
		void OnPivotGridPropertyChanged(object sender, PivotPropertyChangedEventArgs e) {
			if(Field == null) return;
#if SL
			object source = e.Source;
#else
			object source = e.OriginalSource;
#endif
			if(source == PivotGrid) {
				if(e.Property == PivotGridControl.DeferredUpdatesProperty) {
					UpdateIsFilterButtonVisible();
					UpdateIsSortButtonVisible();
				}
				if(e.Property == PivotGridControl.AllowFilterInFieldListProperty)
					UpdateIsFilterButtonVisible();
				if(e.Property == PivotGridControl.AllowSortInFieldListProperty)
					UpdateIsSortButtonVisible();
				if(e.Property == PivotGridControl.RowTotalsLocationProperty)
					UpdateWidthBinding();
			}
			if(source == Field) {
				if(e.Property == PivotGridField.IsFilteredProperty || e.Property == PivotGridField.IsFilterButtonVisibleProperty)
					UpdateIsFilterButtonVisible();
				if(e.Property == PivotGridField.SortOrderProperty || e.Property == PivotGridField.IsSortButtonVisibleProperty)
					UpdateIsSortButtonVisible();
			}
		}
		void OnHeaderPositionPropertyChanged(Internal.HeaderPosition headerPosition) {
			UpdateIsFirst();
		}
		void OnIsSelectedInDesignTimeChanged() {
			Border selectedInDesignTimeControl = LayoutHelper.FindElementByName(this, "PART_DTBorder") as Border;
			if(selectedInDesignTimeControl != null)
				selectedInDesignTimeControl.Visibility = IsSelectedInDesignTime ? Visibility.Visible : Visibility.Collapsed;
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateIsFilterButtonVisible();
			if(CanDrag || IsFilterButtonVisible == System.Windows.Visibility.Visible || IsSortDownButtonVisible == System.Windows.Visibility.Visible || IsSortUpButtonVisible == System.Windows.Visibility.Visible)
				VisualStateManager.GoToState(this, "HandCursor", false);
			else
				VisualStateManager.GoToState(this, "NormalCursor", false);
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			if(FilterPopup == null || FilterPopup.IsPopupOpen)
				return;
			UpdateIsFilterButtonVisible();
		}
#if SL
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			SetValue(IsMouseOverProperty, false);
		}
#endif
		internal FieldListArea GetArea() {
			FieldHeadersBase innerList = LayoutHelper.FindParentObject<FieldHeadersBase>(this);
			return innerList != null ? innerList.Area : FieldListArea.All;
		}
		internal bool IsFieldListHeader {
			get {
				return FieldHeadersBase.GetFieldListArea(this) == FieldListArea.All;
			}
		}
		internal bool DeferUpdates {
			get {
				return PivotGrid.Data.FieldListFields.DeferUpdates && IsFieldListHeader;
			}
		}
		protected internal void UpdateIsFilterButtonVisible() {
			bool defereFiltered = Field != null && IsFieldListHeader && Field.Data.FieldListFields.IsFieldFiltered(Field.InternalField);
			if(Field == null || !Field.IsFilterButtonVisible ||
				FieldHeadersBase.GetFieldListArea(this) == FieldListArea.All &&
								   (!PivotGrid.AllowFilterInFieldList || GetArea() == FieldListArea.All)
						&& !Field.IsFiltered && !defereFiltered || Field.PivotGrid == null || Field.PivotGrid.IsDesignMode) {
				IsFilterButtonVisible = Visibility.Collapsed;
				CanFilterButtonVisible = false;
			} else {
				if(IsMouseOver || Field.IsFiltered || defereFiltered) {
					IsFilterButtonVisible = Visibility.Visible;
					CanFilterButtonVisible = true;
				} else {
#if !SL
					IsFilterButtonVisible = Visibility.Hidden;
#else
					IsFilterButtonVisible = Visibility.Collapsed;
#endif
					CanFilterButtonVisible = true;
				}
			}
		}
		protected void UpdateIsSortButtonVisible() {
			Visibility upVisible = Visibility.Collapsed, downVisible = Visibility.Collapsed;
			bool CanSort = GetCanSort();
			if(Field != null) {
				if(Field.SortOrder == FieldSortOrder.Ascending)
					upVisible = CanSort ? Visibility.Visible : Visibility.Collapsed;
				else
					downVisible = CanSort ? Visibility.Visible : Visibility.Collapsed;
			}
			EnsureChangeFieldSortCommand(CanSort);
			IsSortUpButtonVisible = upVisible;
			IsSortDownButtonVisible = downVisible;
		}
		protected bool GetCanSort() {
			if(Field == null || !Field.IsSortButtonVisible || FieldHeadersBase.GetFieldListArea(this) == FieldListArea.All &&
							(Data.FieldListFields.DeferUpdates == true || PivotGrid.AllowSortInFieldList == false) || !Field.InternalField.IsColumnOrRow)
				return false;
			else
				return true;
		}
		public void Bind(PivotGridField field, int index, FieldListArea area) {
			BindCore(field, index);
			FieldHeadersBase.SetFieldListArea(this, area);
		}
		protected virtual void BindCore(PivotGridField field, int index) {
			if(field == null)
				throw new ArgumentNullException("field");
			DataContext = field;
			SetActualAreaIndex(this, index);
			FieldHeaderContentPresenter presenter = GetHeaderPresenter();
			if(presenter != null)
				presenter.EnsureIsBinded();
		}
		public void Unbind() {
			FieldHeaderContentPresenter presenter = GetHeaderPresenter();
			if(presenter != null)
				presenter.Unbind();
			DataContext = null;
			SetActualAreaIndex(this, -1);
		}
		FieldHeaderContentPresenter GetHeaderPresenter() {
			return LayoutHelper.FindElement(this, (d) => d is FieldHeaderContentPresenter) as FieldHeaderContentPresenter;
		}
		protected override IDragElement CreateDragElementCore(Point offset) {
			PivotGrid.UserAction = UserAction.FieldDrag;
			return new FieldHeaderDragElement(this, Field, offset);
		}
		public override string ToString() {
		   return Content == null ? string.Empty : Content.ToString();
		}
		protected void UpdateWidthBinding() {
			bool? bind = GetIsMustBindToFieldWidth();
			if(bind == true && double.IsNaN(Width))
				SetBinding(WidthProperty, new Binding("Width") { Mode = BindingMode.OneWay, Source = GetWidthSource() });
			else if(bind == false)
				Width = double.NaN;
		}
		protected virtual object GetWidthSource() {
			return Field;
		}
		protected virtual bool? GetIsMustBindToFieldWidth() {
			return GetIsMustBindToFieldWidthCore();
		}
		protected virtual bool IsGroupHeader {
			get { return false; }
		}
		bool GetIsRowAreaNotFieldListHeader() {
			return GetIsNotFieldListHeader() && Field.Area == FieldArea.RowArea;
		}
		bool GetIsNotFieldListHeader() {
			if(Field == null || !Field.Visible ||
				FieldHeadersBase.GetFieldListArea(this) == FieldListArea.All ||
				!(IsGroupHeader || Field.Group == null))
				return false;
			return true;
		}
		bool? GetIsMustBindToFieldWidthCore() {
			if(!GetIsRowAreaNotFieldListHeader() || PivotGrid == null || PivotGrid.RowTotalsLocation == FieldRowTotalsLocation.Tree)
				return false;
			return true;
		}
	}
	public class FieldHeaderDragElement : HeaderDragElementBase {
		protected FieldHeaderBase FieldHeader { get { return (FieldHeaderBase)HeaderElement; } }
		public FieldHeaderDragElement(FieldHeaderBase fieldHeaderElement, DependencyObject dataContext, Point offset)
			: base(fieldHeaderElement, new PivotGridFieldGroupData(dataContext), offset
#if !SL
			, FloatingMode.Popup
#endif     
			) {
			if(container != null)
				container.UseActiveStateOnly = true;
			if(HeaderElement.GetValue(FieldHeaderBase.DragElementTemplateProperty) == null)
				throw new ArgumentNullException("DragElementTemplate");
		}
		protected override void AddGridChild(object child) {
			FieldHeader.PivotGrid.AddChild(child);
			if(child as DependencyObject != null) {
				FieldHeadersBase.SetFieldListArea((DependencyObject)child, FieldHeadersBase.GetFieldListArea(FieldHeader));
			}
		}
		protected override void RemoveGridChild(object child) {
			FieldHeader.PivotGrid.RemoveChild(child);
		}
		public override void Destroy() {
			base.Destroy();
			FieldHeader.PivotGrid.UserAction = UserAction.None;
		}
		protected override string DragElementTemplatePropertyName {
			get { return FieldHeaderBase.DragElementTemplatePropertyName; }
		}
		protected override FrameworkElement HeaderButton {
			get { return FieldHeader.HeaderButton; }
		}
		protected override void SetDragElementSize(FrameworkElement elem, Size size) {
			FieldHeaderBase.SetDragElementSize(elem, size);
		}
	}
	[
	DefaultProperty("Content"), ContentProperty("Content"),
	Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)
	]
	public class FieldHeaderContentPresenter : PivotContentPresenter {
		public PivotGridField Field {
			get { return (PivotGridField)GetValue(FieldProperty); }
			set { SetValue(FieldProperty, value); }
		}
		public static readonly DependencyProperty FieldProperty = DependencyPropertyManager.Register("Field", typeof(PivotGridField), typeof(FieldHeaderContentPresenter), new PropertyMetadata(null, (d, e) => ((FieldHeaderContentPresenter)d).UpdateContentBindings()));
		protected override void OnDataContextChanged() {
			base.OnDataContextChanged();
			UpdateContentTemplate(false);
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == FieldHeadersBase.FieldListAreaProperty && e.NewValue.Equals(FieldListArea.All))
				UpdateContentTemplate(true);
		}
#else
		public FieldHeaderContentPresenter () {
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			EnsureIsBinded();
		}
#endif
		bool binded;
		void UpdateContentTemplate(bool list) {
			UpdateContentBindings();
#if !SL
			object realContent = Content is PivotGridFieldGroupData ? ((PivotGridFieldGroupData)Content).Content : Content;
#else
			object realContent = DataContext is PivotGridFieldGroupData ? ((PivotGridFieldGroupData)DataContext).Content : DataContext;
#endif
			if(realContent as PivotGridField != null || realContent as PivotGridGroup != null) {
				if(!list)
					list = FieldHeadersBase.GetFieldListArea(this) == FieldListArea.All;
				binded = true;
#if SL
				if(list && realContent is PivotGridGroup)
					realContent = ((PivotGridGroup)realContent).FirstField;
#endif
				string template = list ? "ActualHeaderListTemplate" : "ActualHeaderTemplate";
				string selector = list ? "ActualHeaderListTemplateSelector" : "ActualHeaderTemplateSelector";
				SetBinding(ContentTemplateProperty, new Binding(template) {
					Mode = BindingMode.OneWay, Source = realContent
				});
				SetBinding(ContentTemplateSelectorProperty, new Binding(selector) {
					Mode = BindingMode.OneWay, Source = realContent
				});
			} else {
				if(!binded)
					return;
				Unbind();
			}
		}
		void UpdateContentBindings() { 
#if !SL
			PivotGridFieldGroupData pivotGridFieldGroupData = Content as PivotGridFieldGroupData;
#else
			PivotGridFieldGroupData pivotGridFieldGroupData = DataContext as PivotGridFieldGroupData;
#endif
			if(pivotGridFieldGroupData != null && Field != null)
				BindingOperations.SetBinding(pivotGridFieldGroupData, PivotGridFieldGroupData.ActualHeaderContentStyleProperty, new Binding("ActualHeaderContentStyle") { Source = Field });
		}
		protected internal void Unbind() {
			binded = false;
#if SL
			ClearValue(ContentTemplateProperty);
			ClearValue(ContentTemplateSelectorProperty);
#else
			BindingOperations.ClearBinding(this, ContentTemplateProperty);
			BindingOperations.ClearBinding(this, ContentTemplateSelectorProperty);
#endif
		}
		protected internal void EnsureIsBinded() {
			if(!binded)
				UpdateContentTemplate(false);
		}
	}
	public class FieldHeaderContentControl : ContentControl {
		protected static string MouseOverStateName = "MouseOver";
		protected static string PressedStateName = "Pressed";
		protected static string NormalStateName = "Normal";
		public static readonly DependencyProperty IsMouseOverOverrideProperty =
			DependencyPropertyManager.Register("IsMouseOverOverride", typeof(bool), typeof(FieldHeaderContentControl), new FrameworkPropertyMetadata(false, (d, e) => ((FieldHeaderContentControl)d).IsMouseOverChanged()));
		public static readonly DependencyProperty IsPressedProperty =
			DependencyPropertyManager.Register("IsPressed", typeof(bool), typeof(FieldHeaderContentControl), new FrameworkPropertyMetadata(false, 
											 (d, e) => ((FieldHeaderContentControl)d).IsPressedChanged()));
		public bool IsMouseOverOverride {
			get { return (bool)GetValue(IsMouseOverOverrideProperty); }
			set { SetValue(IsMouseOverOverrideProperty, value); }
		}
		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}
		public FieldHeaderContentControl() {
		}
		protected void IsPressedChanged() {
			if(!IsPressed)
				GoToMouseOverState();
			PivotGridField field = DataContext as PivotGridField;
			if(field == null) {
				GoToMouseOverState();
				return;
			}
			if(!field.IsSortButtonVisible) {
				GoToMouseOverState();
				return;
			}
			if(IsPressed)
				VisualStateManager.GoToState(this, PressedStateName, false);
			else
				GoToMouseOverState();
		}
		void IsMouseOverChanged() {
			GoToMouseOverState();
		}
		private void GoToMouseOverState() {
			VisualStateManager.GoToState(this, IsMouseOverOverride && !IsPressed ? MouseOverStateName : NormalStateName, false);
		}
	}
}
namespace DevExpress.Xpf.PivotGrid {
	public class PivotGridFieldGroupData : FrameworkElement {
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}
		public static readonly DependencyProperty DisplayTextProperty = DependencyPropertyManager.Register("DisplayText", typeof(string), typeof(PivotGridFieldGroupData), new PropertyMetadata(null));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Style ActualHeaderContentStyle {
			get { return (Style)GetValue(ActualHeaderContentStyleProperty); }
			set { SetValue(ActualHeaderContentStyleProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualHeaderContentStyleProperty = DependencyPropertyManager.Register("ActualHeaderContentStyle", typeof(Style), typeof(PivotGridFieldGroupData), new PropertyMetadata(null));
		readonly object content;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object Content { get { return content; } }
		public PivotGridFieldGroupData(object content) {
			this.content = content;
			BindingOperations.SetBinding(this, DisplayTextProperty, new Binding("DisplayText") { Source = content });
		}
		public PivotGridFieldGroupData(FieldHeader content)
			: this(content.Field) {
			BindingOperations.SetBinding(this, PaddingProperty, new Binding("Padding") { Source = content });
			BindingOperations.SetBinding(this, BorderThicknessProperty, new Binding("BorderThickness") { Source = content });
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty PaddingProperty = DependencyPropertyManager.Register("Padding", typeof(Thickness), typeof(PivotGridFieldGroupData), new PropertyMetadata(new Thickness()));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty BorderThicknessProperty = DependencyPropertyManager.Register("BorderThickness", typeof(Thickness), typeof(PivotGridFieldGroupData), new PropertyMetadata(new Thickness()));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Thickness BorderThickness {
			get { return (Thickness)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}
	}
}
