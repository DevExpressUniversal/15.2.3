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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.Windows.Automation.Peers;
namespace DevExpress.Xpf.Carousel {
	public enum PathSizingMode { Proportional, Stretch }
	internal class CarouselPropertyManager {
		public delegate object PropertyValueConverter(object baseValue);
		public CarouselPropertyManager(CarouselPanel owner) {
			Carousel = owner;
			RegisterProperties();
		}
		CarouselPanel Carousel { get; set; }
		readonly List<DependencyProperty> propertyList = new List<DependencyProperty>();
		readonly Dictionary<DependencyProperty, PropertyValueConverter> toBaseValueConverters = new Dictionary<DependencyProperty, PropertyValueConverter>();
		readonly Dictionary<DependencyProperty, PropertyValueConverter> fromBaseValueConverters = new Dictionary<DependencyProperty, PropertyValueConverter>();
		DependencyProperty syncProperty;
		DependencyProperty lastAssignedProperty;
		object GetActiveItemFromActiveItemIndex(object value) {
			int activeIndexIndex = (int)value;
			UIElement result = null;
			if (IsExistingItemIndex(activeIndexIndex))
				result = Carousel.Children[activeIndexIndex];
			return result;
		}
		object GetActiveItemIndexFromActiveItem(object value) {
			UIElement activeItem = (UIElement)value;
			int result = -1;
			if (IsExistingItem(activeItem))
				result = Carousel.Children.IndexOf(activeItem);
			return result;
		}
		object GetActiveItemIndexFromFirstVisibleItemIndex(object value) {
			int result = 0;
			int firstVisibleItemIndex = (int)value;
			if (IsExistingItemIndex(firstVisibleItemIndex))
				result = (firstVisibleItemIndex + Carousel.AttractorPointIndex) % Carousel.Children.Count;
			return result;
		}
		object GetFirstVisibleIndexFromActiveItemIndex(object value) {
			int result = -1;
			int activeItemIndex = (int)value;
			if (IsExistingItemIndex(activeItemIndex))
				result = Carousel.GetValidIndex(activeItemIndex - Carousel.AttractorPointIndex);
			return result;
		}
		bool IsExistingItem(UIElement item) {
			return item != null && Carousel.Children.Contains(item);
		}
		bool IsExistingItemIndex(int index) {
			return index > -1 && index < Carousel.Children.Count;
		}
		object GetValidIndex(int index) {
			if (Carousel.IsEmpty)
				return -1;
			return IsExistingItemIndex(index) ? index : 0;
		}
		UIElement GetValidItem(UIElement item) {
			if (Carousel.IsEmpty)
				return null;
			return IsExistingItem(item) ? item : Carousel.Children[0];
		}
		object GetValidValue(object value) {
			return (value == null || value is UIElement) ? GetValidItem((UIElement)value) : GetValidIndex((int)value);
		}
		object GetBaseSyncValue() {
			return toBaseValueConverters[syncProperty](GetValue(syncProperty));
		}
		void RegisterProperties() {
			Register(CarouselPanel.ActiveItemIndexProperty, (object baseValue) => { return GetValidIndex((int)baseValue); }, (object baseValue) => { return GetValidIndex((int)baseValue); });
			Register(CarouselPanel.ActiveItemProperty, GetActiveItemIndexFromActiveItem, GetActiveItemFromActiveItemIndex);
			Register(CarouselPanel.FirstVisibleItemIndexProperty, GetActiveItemIndexFromFirstVisibleItemIndex, GetFirstVisibleIndexFromActiveItemIndex);
			lastAssignedProperty = propertyList[0];
		}
		void Register(DependencyProperty property, PropertyValueConverter convertToBaseValue, PropertyValueConverter convertFromBaseValue) {
			propertyList.Add(property);
			toBaseValueConverters.Add(property, convertToBaseValue);
			fromBaseValueConverters.Add(property, convertFromBaseValue);
		}
		bool CanProcess {
			get { return Carousel.IsLoaded; }
		}
		bool HasSyncProperty { get { return syncProperty != null; } }
		bool IsSyncProperty(DependencyProperty property) { return HasSyncProperty && property == syncProperty; }
		void SetSyncProperty(DependencyProperty property) {
			syncProperty = property;
		}
		void CoerceAllProperties() {
			foreach (DependencyProperty property in propertyList)
				if (!IsSyncProperty(property))
					CoerceValue(property);
		}
		void ResetSyncProperty() {
			syncProperty = null;
		}
		void CoerceValue(DependencyProperty property) {
			Carousel.CoerceValue(property);
		}
		object GetValue(DependencyProperty property) {
			return Carousel.GetValue(property);
		}
		object ConvertTo(DependencyProperty property) {
			return fromBaseValueConverters[property](GetBaseSyncValue());
		}
		object CoerceCore(DependencyProperty property, object baseValue) {
			if (HasSyncProperty)
				return ConvertTo(property);
			return GetValidValue(baseValue);
		}
		void SyncCore(DependencyProperty property) {
			SetSyncProperty(property);
			CoerceAllProperties();
			ResetSyncProperty();
		}
		#region Public
		public bool IsValueProperty(DependencyProperty property) { return propertyList.Contains(property); }
		public object Coerce(DependencyProperty property, object baseValue) {
			if (!HasSyncProperty)
				lastAssignedProperty = property;
			if (CanProcess)
				return CoerceCore(property, baseValue);
			return baseValue;
		}
		public void Sync(DependencyProperty property) {
			if (CanProcess && !HasSyncProperty)
				SyncCore(property);
		}
		public void UpdateState() {
			CoerceValue(lastAssignedProperty);
			Sync(lastAssignedProperty);
		}
		#endregion
	}
	[DXToolboxBrowsable]
#if !SILVERLIGHT
#endif
	public class CarouselPanel : Panel, IScrollInfo, ICarouselPanel, IWeakEventListener {
		#region Static fields
		static readonly DependencyPropertyKey ParametersPropertyKey;
		public static readonly DependencyProperty ParametersProperty;
		public static readonly DependencyProperty ActiveItemProperty;
		public static readonly DependencyProperty VisibleItemCountProperty;
		public static readonly DependencyProperty ItemMovingPathProperty;
		public static readonly DependencyProperty OffsetDistributionFunctionProperty;
		public static readonly DependencyProperty OffsetAnimationAddFunctionProperty;
		public static readonly DependencyProperty AnimationTimeProperty;
		public static readonly DependencyProperty FirstVisibleItemIndexProperty;
		public static readonly DependencyProperty AttractorPointIndexProperty;
		public static readonly DependencyProperty ActiveItemIndexProperty;
		public static readonly DependencyProperty SmoothingTimeFractionProperty;
		public static readonly DependencyProperty AnimationDisabledProperty;
		public static readonly DependencyProperty PathVisibleProperty;
		public static readonly DependencyProperty PathPaddingProperty;
		public static readonly DependencyProperty ParameterSetProperty;
		public static readonly DependencyProperty ActivateItemOnClickProperty;
		public static readonly DependencyProperty IsRepeatProperty;
		public static readonly DependencyProperty SnapItemsToDevicePixelsProperty;
		static readonly DependencyPropertyKey CarouselPropertyKey;
		public static readonly DependencyProperty CarouselProperty;
		public static readonly DependencyProperty IsInvertedDirectionProperty;
		public static readonly DependencyProperty PointPathFunctionProperty;
		public static readonly DependencyProperty ItemSizeProperty;
		public static readonly DependencyProperty IsAutoSizeItemProperty;
		public static readonly DependencyProperty PathSizingModeProperty;
		public static readonly RoutedCommand Customize = new RoutedCommand("Customize", typeof(CarouselPanel));
		public static readonly RoutedCommand Next = new RoutedCommand("Next", typeof(CarouselPanel));
		public static readonly RoutedCommand Previous = new RoutedCommand("Previous", typeof(CarouselPanel));
		public static readonly RoutedCommand NextPage = new RoutedCommand("NextPage", typeof(CarouselPanel));
		public static readonly RoutedCommand PreviousPage = new RoutedCommand("PreviousPage", typeof(CarouselPanel));
		public static readonly RoutedCommand FirstItem = new RoutedCommand("FirstItem", typeof(CarouselPanel));
		public static readonly RoutedCommand LastItem = new RoutedCommand("LastItem", typeof(CarouselPanel));
		#endregion
		#region Constructors
		static CarouselPanel() {
			Type ownerType = typeof(CarouselPanel);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			VisibleItemCountProperty = DependencyProperty.Register("VisibleItemCount", typeof(int), ownerType, new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnVisibleItemCountChanged, CoerceVisibleItemCount));
			ItemMovingPathProperty = DependencyProperty.Register("ItemMovingPath", typeof(PathGeometry), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
			OffsetDistributionFunctionProperty = DependencyProperty.Register("OffsetDistributionFunction", typeof(FunctionBase), ownerType, new PropertyMetadata(new EqualFunction()));
			OffsetAnimationAddFunctionProperty = DependencyProperty.Register("OffsetAnimationAddFunction", typeof(FunctionBase), ownerType, new PropertyMetadata(new LinearFunction(0, 0)));
			AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(double), ownerType, new FrameworkPropertyMetadata(1000.0));
			SmoothingTimeFractionProperty = DependencyProperty.Register("SmoothingTimeFraction", typeof(double), ownerType, new FrameworkPropertyMetadata(0.3));
			AttractorPointIndexProperty = DependencyProperty.Register("AttractorPointIndex", typeof(int), ownerType, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange, OnAttractorPointIndexChanged, CoerceAttractorPointIndex));
			ActiveItemIndexProperty = DependencyProperty.Register("ActiveItemIndex", typeof(int), ownerType, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange, OnActiveItemIndexChanged, CoerceActiveItemIndex));
			FirstVisibleItemIndexProperty = DependencyProperty.Register("FirstVisibleItemIndex", typeof(int), ownerType, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange, OnFirstVisibleItemIndexChanged, CoerceFirstVisibleItemIndex));
			AnimationDisabledProperty = DependencyProperty.Register("AnimationDisabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			PathVisibleProperty = DependencyProperty.Register("PathVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
			PathPaddingProperty = DependencyProperty.Register("PathPadding", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
			ParameterSetProperty = DependencyProperty.Register("ParameterSet", typeof(ParameterCollection), ownerType, new FrameworkPropertyMetadata(null, OnParameterSetChanged));
			ActivateItemOnClickProperty = DependencyProperty.Register("ActivateItemOnClick", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsRepeatProperty = DependencyProperty.Register("IsRepeat", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
			IsInvertedDirectionProperty = DependencyProperty.Register("IsInvertedDirection", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			PointPathFunctionProperty = DependencyProperty.Register("PointPathFunction", typeof(PieceLinearFunction), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentArrange));
			SnapItemsToDevicePixelsProperty = DependencyProperty.Register("SnapItemsToDevicePixels", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(Size), ownerType, new FrameworkPropertyMetadata(new Size(50, 50)));
			IsAutoSizeItemProperty = DependencyProperty.Register("IsAutoSizeItem", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			PathSizingModeProperty = DependencyProperty.Register("PathSizingMode", typeof(PathSizingMode), ownerType, new FrameworkPropertyMetadata(PathSizingMode.Proportional, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
			ActiveItemProperty = DependencyProperty.Register("ActiveItem", typeof(UIElement), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, OnActiveItemChanged, CoerceActiveItem));
			ParametersPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Parameters", typeof(ParametersTypeDescriptor), typeof(CarouselPanel), new FrameworkPropertyMetadata(null));
			ParametersProperty = ParametersPropertyKey.DependencyProperty;
			CarouselPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Carousel", typeof(CarouselPanel), typeof(CarouselPanel), new FrameworkPropertyMetadata(null));
			CarouselProperty = CarouselPropertyKey.DependencyProperty;
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			CommandManager.RegisterClassCommandBinding(typeof(CarouselPanel), new CommandBinding(Next, new ExecutedRoutedEventHandler(OnBrowseForward), new CanExecuteRoutedEventHandler(CanMoveForward)));
			CommandManager.RegisterClassCommandBinding(typeof(CarouselPanel), new CommandBinding(Previous, new ExecutedRoutedEventHandler(OnBrowseBack), new CanExecuteRoutedEventHandler(CanMoveBack)));
			CommandManager.RegisterClassCommandBinding(typeof(CarouselPanel), new CommandBinding(NextPage, new ExecutedRoutedEventHandler(OnNextPage), new CanExecuteRoutedEventHandler(CanMoveForward)));
			CommandManager.RegisterClassCommandBinding(typeof(CarouselPanel), new CommandBinding(PreviousPage, new ExecutedRoutedEventHandler(OnPreviousPage), new CanExecuteRoutedEventHandler(CanMoveBack)));
			CommandManager.RegisterClassCommandBinding(typeof(CarouselPanel), new CommandBinding(FirstItem, new ExecutedRoutedEventHandler(OnFirstPage), new CanExecuteRoutedEventHandler(CanMoveBack)));
			CommandManager.RegisterClassCommandBinding(typeof(CarouselPanel), new CommandBinding(LastItem, new ExecutedRoutedEventHandler(OnLastPage), new CanExecuteRoutedEventHandler(CanMoveForward)));
		}
		public CarouselPanel() {
			renderListener.Rendering += OnRendered;
			PathScaleX = 1;
			PathScaleY = 1;
			Loaded += OnLoaded;
			InitializePropertyManager();
		}
		void InitializePropertyManager() {
			propertyManager = new CarouselPropertyManager(this);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			ResetParameters();
			UpdateState();
			SubscribeEvents();
		}
		void SubscribeEvents() {
			ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
			if (itemsControl != null)
				itemsControl.ItemContainerGenerator.ItemsChanged += OnOwnerItemsChanged;
		}
		void OnOwnerItemsChanged(object sender, ItemsChangedEventArgs e) {
			OnChildrenChanged();
		}
		void UpdateState() {
			propertyManager.UpdateState();
		}
		#endregion
		#region Static methods
		internal bool IsEmpty { get { return Children.Count == 0; } }
		static void CanMoveForward(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = ((CarouselPanel)sender).CanMoveForward();
		}
		static void CanMoveBack(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = ((CarouselPanel)sender).CanMoveBack();
		}
		static void OnParameterSetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue != null)
				ParameterCollectionChangedEventManager.AddListener((ParameterCollection)e.NewValue, (IWeakEventListener)obj);
			if (e.OldValue != null)
				ParameterCollectionChangedEventManager.RemoveListener((ParameterCollection)e.OldValue, (IWeakEventListener)obj);
			((CarouselPanel)obj).ResetParameters();
		}
		static void OnPaddingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CarouselPanel)obj).InvalidateVisual();
		}
		static void OnActiveItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CarouselPanel)obj).OnActiveItemChanged();
		}
		static void OnActiveItemIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CarouselPanel)obj).OnActiveItemIndexChanged();
		}
		static void OnAttractorPointIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CarouselPanel)obj).OnAttractorPointIndexChanged();
		}
		static void OnFirstVisibleItemIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CarouselPanel)obj).OnFirstVisibleItemIndexChanged();
		}
		static void OnVisibleItemCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
			((CarouselPanel)obj).OnVisibleItemCountChanged();
		}
		static void ResetParameters(CarouselPanel carousel) {
			if (carousel.IsLoaded) {
				foreach (UIElement element in carousel.Children)
					SetParameters(element, null);
				carousel.InvalidateArrange();
			}
		}
		void ResetParameters() {
			ResetParameters(this);
		}
		static void OnBrowseForward(object sender, ExecutedRoutedEventArgs e) {
			((CarouselPanel)sender).MoveNext();
		}
		static void OnBrowseBack(object sender, ExecutedRoutedEventArgs e) {
			((CarouselPanel)sender).MovePrev();
		}
		static void OnNextPage(object sender, ExecutedRoutedEventArgs e) {
			((CarouselPanel)sender).MoveNextPage();
		}
		static void OnPreviousPage(object sender, ExecutedRoutedEventArgs e) {
			((CarouselPanel)sender).MovePrevPage();
		}
		static void OnFirstPage(object sender, ExecutedRoutedEventArgs e) {
			((CarouselPanel)sender).MoveFirstPage();
		}
		static void OnLastPage(object sender, ExecutedRoutedEventArgs e) {
			((CarouselPanel)sender).MoveLastPage();
		}
		static object CoerceFirstVisibleItemIndex(DependencyObject obj, object value) {
			return ((CarouselPanel)obj).CoerceFirstVisibleItemIndex((int)value);
		}
		static object CoerceVisibleItemCount(DependencyObject obj, object value) {
			return ((CarouselPanel)obj).CoerceVisibleItemCount((int)value);
		}
		static object CoerceActiveItemIndex(DependencyObject obj, object value) {
			return ((CarouselPanel)obj).CoerceActiveItemIndex((int)value);
		}
		static object CoerceAttractorPointIndex(DependencyObject obj, object value) {
			return ((CarouselPanel)obj).CoerceAttractorPointIndex((int)value);
		}
		static object CoerceActiveItem(DependencyObject obj, object value) {
			return ((CarouselPanel)obj).CoerceActiveItem((UIElement)value);
		}
		static internal int GetItemVisibleIndex(int childIndex, int activeItemIndex, int childrenCount, int visibleItemsCount, int attractorPointIndex, int offset) {
			int firstVisibleItemIndex = (activeItemIndex - attractorPointIndex);
			if (firstVisibleItemIndex < 0)
				firstVisibleItemIndex += childrenCount;
			int visibleIndex = childIndex - firstVisibleItemIndex;
			if (childrenCount < visibleItemsCount) {
				visibleIndex = (childIndex - activeItemIndex + attractorPointIndex) % visibleItemsCount;
				if (visibleIndex < 0)
					visibleIndex += visibleItemsCount;
			} else {
				if (visibleIndex < 0)
					visibleIndex += childrenCount;
				if (visibleIndex >= visibleItemsCount && offset < 0)
					visibleIndex -= childrenCount;
				if (visibleIndex < 0 && offset >= 0)
					visibleIndex += childrenCount;
			}
			return visibleIndex;
		}
		internal UIElement GetChildByVisibleIndex(int visibleIndex) {
			UIElement result = null;
			for (int i = 0; i < Children.Count; i++)
				if (GetItemVisibleIndex(i, ActiveItemIndex, Children.Count, VisibleItemCountWrapper, AttractorPointIndex, 0) == visibleIndex)
					result = Children[i];
			return result;
		}
		static internal ItemTransfer CalculateItemTransfer(int itemIndex, int activeItemIndex, int childrenCount, int visibleItemsCount, int offset) {
			int visibleIndex = GetItemVisibleIndex(itemIndex, activeItemIndex, childrenCount, visibleItemsCount, 0, offset);
			return CalculateItemTransfer(visibleIndex, childrenCount, visibleItemsCount, offset);
		}
		static internal ItemTransfer CalculateItemTransfer(int visibleIndex, int childrenCount, int visibleItemsCount, int offset) {
			return new ItemTransfer(GetRanges(offset, visibleIndex, childrenCount, visibleItemsCount));
		}
		static IList<Range> GetRanges(int offset, int visibleIndex, int childrenCount, int visibleItemsCount) {
			List<Range> ranges = new List<Range>();
			childrenCount = Math.Max(visibleItemsCount, childrenCount);
			if (childrenCount > 0) {
				double distance = 1.0 / (visibleItemsCount - 1);
				if (offset == 0)
					ranges.Add(new Range(distance * visibleIndex, distance * visibleIndex));
				else if (offset > 0) {
					while (offset > 0) {
						int lowIndex = visibleIndex - offset > 0 ? visibleIndex - offset : 0;
						ranges.Add(new Range(distance * visibleIndex, lowIndex * distance));
						offset -= visibleIndex;
						visibleIndex = Math.Max(childrenCount, visibleItemsCount);
					}
				} else if (offset < 0) {
					while (offset < 0) {
						int hiIndex = visibleIndex - offset < visibleItemsCount - 1 ? visibleIndex - offset : visibleItemsCount - 1;
						ranges.Add(new Range(distance * visibleIndex, hiIndex * distance));
						offset += (visibleItemsCount - 1) - visibleIndex;
						visibleIndex = (visibleItemsCount - 1) - childrenCount;
					}
				}
			}
			return ranges;
		}
		public static ParametersTypeDescriptor GetParameters(UIElement element) {
			return (ParametersTypeDescriptor)element.GetValue(ParametersProperty);
		}
		internal static void SetParameters(UIElement element, ParametersTypeDescriptor value) {
			element.SetValue(ParametersPropertyKey, value);
		}
		public static CarouselPanel GetCarousel(ItemsControl itemsControl) {
			return (CarouselPanel)itemsControl.GetValue(CarouselProperty);
		}
		internal static void SetCarousel(ItemsControl itemsControl, CarouselPanel value) {
			itemsControl.SetValue(CarouselPropertyKey, value);
		}
		static UIElement GetActiveItem(CarouselPanel carousel) {
			if (carousel.IsEmpty)
				return null;
			int activeItemIndex = (carousel.FirstVisibleItemIndex + carousel.AttractorPointIndex) % carousel.Children.Count;
			if (activeItemIndex >= 0 && carousel.Children.Count > activeItemIndex) return carousel.Children[activeItemIndex];
			return null;
		}
		#endregion
		#region Fields
		List<DependencyProperty> affectingProperties = new List<DependencyProperty>(new DependencyProperty[] { VisibleItemCountProperty, AttractorPointIndexProperty });
		RenderEventListener renderListener = new RenderEventListener();
		StopwatchWrapper stopwatchCore = new StopwatchWrapper();
		CarouselPropertyManager propertyManager;
		#endregion
		#region Properties
		int currentOffset;
		internal int CurrentOffset {
			get { return currentOffset; }
			private set { currentOffset = value; }
		}
		[
#if !SL
	DevExpressXpfCarouselLocalizedDescription("CarouselPanelParameterSet"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ParameterCollection ParameterSet {
			get { return (ParameterCollection)GetValue(ParameterSetProperty); }
			set { SetValue(ParameterSetProperty, value); }
		}
		ParameterCollection parameterSetDefault = null;
		protected internal ParameterCollection ParameterSetWrapper {
			get {
				if (ParameterSet == null) {
					if (parameterSetDefault == null)
						parameterSetDefault = new ParameterCollection();
					return parameterSetDefault;
				}
				if (parameterSetDefault != null) {
					foreach (Parameter parameter in parameterSetDefault)
						ParameterSet.Add(parameter);
					parameterSetDefault = null;
				}
				return ParameterSet;
			}
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelAnimationTime")]
#endif
		public double AnimationTime {
			get { return (double)GetValue(AnimationTimeProperty); }
			set { SetValue(AnimationTimeProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelSmoothingTimeFraction")]
#endif
		public double SmoothingTimeFraction {
			get { return (double)GetValue(SmoothingTimeFractionProperty); }
			set { SetValue(SmoothingTimeFractionProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelVisibleItemCount")]
#endif
		public int VisibleItemCount {
			get { return (int)GetValue(VisibleItemCountProperty); }
			set { SetValue(VisibleItemCountProperty, value); }
		}
		internal int VisibleItemCountWrapper {
			get { return PointPathFunction != null ? PointPathFunction.Points.Count : VisibleItemCount; }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelItemMovingPath")]
#endif
		public PathGeometry ItemMovingPath {
			get { return (PathGeometry)GetValue(ItemMovingPathProperty); }
			set { SetValue(ItemMovingPathProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelOffsetDistributionFunction")]
#endif
		public FunctionBase OffsetDistributionFunction {
			get { return (FunctionBase)GetValue(OffsetDistributionFunctionProperty); }
			set { SetValue(OffsetDistributionFunctionProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelOffsetAnimationAddFunction")]
#endif
		public FunctionBase OffsetAnimationAddFunction {
			get { return (FunctionBase)GetValue(OffsetAnimationAddFunctionProperty); }
			set { SetValue(OffsetAnimationAddFunctionProperty, value); }
		}
		internal FunctionBase OffsetDistributionWrapper {
			get {
				if (PointPathFunction != null)
					return (FunctionBase)PointPathDistributionFunction;
				return OffsetDistributionFunction;
			}
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelPointPathFunction")]
#endif
		public PieceLinearFunction PointPathFunction {
			get { return (PieceLinearFunction)GetValue(PointPathFunctionProperty); }
			set { SetValue(PointPathFunctionProperty, value); }
		}
		public PathGeometry GetItemsMovingPath() {
			PathGeometry pg = new PathGeometry();
			PathFigure pf = new PathFigure();
			pg.Figures.Add(pf);
			pf.StartPoint = PointPathFunction.Points[0];
			for (int i = 0; i < PointPathFunction.Points.Count - 1; i++) {
				LineSegment ls = new LineSegment(PointPathFunction.Points[i], true);
				pf.Segments.Add(ls);
			}
			return pg;
		}
		internal PathGeometry PathFromPointPathFunction {
			get {
				if (PointPathFunction == null || PointPathFunction.Points.Count == 0)
					return null;
				PathGeometry pg = new PathGeometry();
				PathFigure pf = new PathFigure();
				pg.Figures.Add(pf);
				pf.StartPoint = PointPathFunction.Points[0];
				for (int i = 1; i < PointPathFunction.Points.Count; i++) {
					LineSegment ls = new LineSegment(PointPathFunction.Points[i], true);
					pf.Segments.Add(ls);
				}
				return pg;
			}
		}
		internal double CalcFixedLayoutLenghtAtPoint(int n) {
			double result = 0;
			for (int i = 0; i < n; i++)
				result += CalcLinearDistance(PointPathFunction.Points[i], PointPathFunction.Points[i + 1]);
			return result;
		}
		double CalcLinearDistance(Point p1, Point p2) {
			return Math.Sqrt(Math.Pow((p1.X - p2.X) * PathScaleX, 2) + Math.Pow((p1.Y - p2.Y) * PathScaleY, 2));
		}
		internal FunctionBase PointPathDistributionFunction {
			get {
				PieceLinearFunction result = new PieceLinearFunction();
				double pathLenght = CalcFixedLayoutLenghtAtPoint(PointPathFunction.Points.Count - 1);
				for (int i = 0; i < PointPathFunction.Points.Count; i++) {
					double x, y;
					x = i * 1d / (double)(PointPathFunction.Points.Count - 1);
					y = CalcFixedLayoutLenghtAtPoint(i) / pathLenght;
					result.Points.Add(new Point(x, y));
				}
				return result;
			}
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelFirstVisibleItemIndex")]
#endif
		public int FirstVisibleItemIndex {
			get { return (int)GetValue(FirstVisibleItemIndexProperty); }
			set { SetValue(FirstVisibleItemIndexProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelAttractorPointIndex")]
#endif
		public int AttractorPointIndex {
			get { return (int)GetValue(AttractorPointIndexProperty); }
			set { SetValue(AttractorPointIndexProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelActiveItemIndex")]
#endif
		public int ActiveItemIndex {
			get { return (int)GetValue(ActiveItemIndexProperty); }
			set { SetValue(ActiveItemIndexProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelActiveItem")]
#endif
		public UIElement ActiveItem {
			get { return (UIElement)GetValue(ActiveItemProperty); }
			set { SetValue(ActiveItemProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelAnimationDisabled")]
#endif
		public bool AnimationDisabled {
			get { return (bool)GetValue(AnimationDisabledProperty); }
			set { SetValue(AnimationDisabledProperty, value); }
		}
		[ DefaultValue(false)]
		internal bool ShowFps { get; set; }
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelPathVisible")]
#endif
		public bool PathVisible {
			get { return (bool)GetValue(PathVisibleProperty); }
			set { SetValue(PathVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelActivateItemOnClick")]
#endif
		public bool ActivateItemOnClick {
			get { return (bool)GetValue(ActivateItemOnClickProperty); }
			set { SetValue(ActivateItemOnClickProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelIsRepeat")]
#endif
		public bool IsRepeat {
			get { return (bool)GetValue(IsRepeatProperty); }
			set { SetValue(IsRepeatProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelSnapItemsToDevicePixels")]
#endif
		public bool SnapItemsToDevicePixels {
			get { return (bool)GetValue(SnapItemsToDevicePixelsProperty); }
			set { SetValue(SnapItemsToDevicePixelsProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelPathSizingMode")]
#endif
		public PathSizingMode PathSizingMode {
			get { return (PathSizingMode)GetValue(PathSizingModeProperty); }
			set { SetValue(PathSizingModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelIsInvertedDirection")]
#endif
		public bool IsInvertedDirection {
			get { return (bool)GetValue(IsInvertedDirectionProperty); }
			set { SetValue(IsInvertedDirectionProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelPathPadding")]
#endif
		public Thickness PathPadding {
			get { return (Thickness)GetValue(PathPaddingProperty); }
			set { SetValue(PathPaddingProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelIsAutoSizeItem")]
#endif
		public bool IsAutoSizeItem {
			get { return (bool)GetValue(IsAutoSizeItemProperty); }
			set { SetValue(IsAutoSizeItemProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselPanelItemSize")]
#endif
		public Size ItemSize {
			get { return (Size)GetValue(ItemSizeProperty); }
			set { SetValue(ItemSizeProperty, value); }
		}
		double pathScaleX;
		double pathScaleY;
		internal double PathScaleX {
			get {
				if (PathSizingMode == PathSizingMode.Proportional)
					return PathScale;
				return pathScaleX;
			}
			set { pathScaleX = value; }
		}
		internal double PathScaleY {
			get {
				if (PathSizingMode == PathSizingMode.Proportional)
					return PathScale;
				return pathScaleY;
			}
			set { pathScaleY = value; }
		}
		internal double PathScale {
			get {
				return Math.Min(pathScaleX, pathScaleY);
			}
		}
		internal double AnimationProgress { get; private set; }
		PathGeometry Path {
			get {
				if (PointPathFunction != null)
					return PathFromPointPathFunction;
				return ItemMovingPath ?? DefaultPath;
			}
		}
		internal PathGeometry TransformedPath {
			get {
				var transformedPath = Path.Clone();
				var actualPadding = ActualPathPadding;
				var transformGroup = new TransformGroup();
				var scaleTransform = new ScaleTransform(PathScaleX, PathScaleY);
				var translateTransformPadding = new TranslateTransform();
				translateTransformPadding.X = actualPadding.Left + (CarouselSize.Width - (actualPadding.Left + actualPadding.Right) - transformedPath.Bounds.Width * PathScaleX) / 2;
				translateTransformPadding.Y = actualPadding.Top + (CarouselSize.Height - (actualPadding.Top + actualPadding.Bottom) - transformedPath.Bounds.Height * PathScaleY) / 2;
				var translateTransformCenter = new TranslateTransform();
				var translateTransformPath = new TranslateTransform(-transformedPath.Bounds.X, -transformedPath.Bounds.Y);
				transformGroup.Children.Add(translateTransformPath);
				transformGroup.Children.Add(scaleTransform);
				transformGroup.Children.Add(translateTransformCenter);
				transformGroup.Children.Add(translateTransformPadding);
				transformedPath.Transform = transformGroup;
				return transformedPath;
			}
		}
		internal Thickness ActualPathPadding {
			get {
				var pathPadding = PathPadding;
				if (PathPadding.Left + PathPadding.Right > 100) {
					pathPadding.Left = 50;
					pathPadding.Right = 50;
				}
				if (PathPadding.Top + PathPadding.Bottom > 100) {
					pathPadding.Bottom = 50;
					pathPadding.Top = 50;
				}
				return new Thickness(pathPadding.Left / 100 * CarouselSize.Width, pathPadding.Top / 100 * CarouselSize.Height, pathPadding.Right / 100 * CarouselSize.Width, pathPadding.Bottom / 100 * CarouselSize.Height);
			}
		}
		protected bool IsDesignMode {
			get {
				return DesignerProperties.GetIsInDesignMode(this);
			}
		}
		internal Size CarouselSize { get; set; }
		protected internal StopwatchWrapper Stopwatch {
			get { return stopwatchCore; }
#if DEBUGTEST
			set { stopwatchCore = value; }
#endif
		}
		#endregion
		#region Methods
		internal void OnChildrenChanged() {
			if (IsLoaded)
				UpdateState();
		}
		protected virtual void OnVisibleItemCountChanged() {
			CoerceValue(AttractorPointIndexProperty);
		}
		protected virtual int CoerceAttractorPointIndex(int baseValue) {
			if (IsLoaded)
				return Math.Min(baseValue, VisibleItemCount - 1);
			return baseValue;
		}
		protected virtual int CoerceActiveItemIndex(int baseValue) {
			return (int)propertyManager.Coerce(ActiveItemIndexProperty, baseValue);
		}
		protected virtual UIElement CoerceActiveItem(UIElement baseValue) {
			return (UIElement)propertyManager.Coerce(ActiveItemProperty, baseValue);
		}
		protected virtual int CoerceFirstVisibleItemIndex(int baseValue) {
			return (int)propertyManager.Coerce(FirstVisibleItemIndexProperty, baseValue);
		}
		protected virtual int CoerceVisibleItemCount(int baseValue) {
			return Math.Max(baseValue, 2);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			UpdateCarouselState(e.Property);
		}
		void UpdateCarouselState(DependencyProperty property) {
			if (propertyManager.IsValueProperty(property)) {
				propertyManager.Sync(property);
				InvalidateScrollInfo();
			}
			if (affectingProperties.Contains(property))
				UpdateState();
		}
		protected virtual void OnAttractorPointIndexChanged() {
		}
		protected virtual void OnActiveItemIndexChanged() {
		}
		protected virtual void OnActiveItemChanged() {
		}
		protected virtual void OnFirstVisibleItemIndexChanged() {
		}
		internal int GetValidIndex(int index) {
			int result = -1;
			if (Children.Count > 0) {
				result = index % Children.Count;
				if (result < 0)
					result += Children.Count;
			}
			return result;
		}
		UIElement GetValidChild(UIElement element) {
			if (IsEmpty || !Children.Contains(element))
				return null;
			return element;
		}
		public void MoveNext() {
			Move(+1);
		}
		public void MovePrev() {
			Move(-1);
		}
		public void MoveNextPage() {
			Move(VisibleItemCountWrapper);
		}
		public void MovePrevPage() {
			Move(-VisibleItemCountWrapper);
		}
		int ActualChildrenCount { get { return Math.Max(Children.Count, VisibleItemCountWrapper); } }
		public void MoveFirstPage() {
			Move(-ActualChildrenCount);
		}
		public void MoveLastPage() {
			Move(ActualChildrenCount);
		}
		bool ActualCanRepeat {
			get { return IsRepeat && Children.Count >= VisibleItemCountWrapper; }
		}
		bool CanMoveForward() {
			return CanMoveCore && (ActualCanRepeat ? true : ActiveItemIndex + CurrentOffset != Children.Count - 1);
		}
		bool CanMoveBack() {
			return CanMoveCore && (ActualCanRepeat ? true : ActiveItemIndex + CurrentOffset != 0);
		}
		bool CanMoveCore {
			get { return IsLoaded && Children.Count > 1; }
		}
		bool CanMove(int offset) {
			bool result = CanMoveCore && offset != 0 && Math.Abs(CurrentOffset) < ActualChildrenCount;
			if (result)
				result = CurrentOffset + offset > 0 ? CanMoveForward() : CanMoveBack();
			return result;
		}
		public void Move(int offset) {
			offset = CorrectOffset(offset);
			if (CanMove(offset))
				MoveCore(offset);
		}
		void MoveCore(int offset) {
			if (IsDirectionChanged(offset, CurrentOffset))
				StopAnimation();
			CalculateTransfers(CurrentOffset + offset);
			UpdateCurrentOffset(offset);
			StartAnimation();
		}
		int CorrectOffset(int offset) {
			if (!ActualCanRepeat) {
				if (offset + CurrentOffset < 0) {
					if (ActiveItemIndex + offset + CurrentOffset < 0)
						offset = -ActiveItemIndex - CurrentOffset;
				} else
					if (ActiveItemIndex + offset + CurrentOffset >= Children.Count)
						offset = Children.Count - ActiveItemIndex - CurrentOffset - 1;
			}
			return offset;
		}
		bool IsDirectionChanged(int currentOffset, int newOffset) {
			return currentOffset != 0 && newOffset != 0 && Math.Sign(currentOffset) != Math.Sign(newOffset);
		}
		void UpdateCurrentOffset(int offset) {
			CurrentOffset += offset;
		}
		void CalculateTransfers(int offset) {
			foreach (UIElement child in Children)
				CalculateTransfer(child, offset);
		}
		void CalculateTransfer(UIElement child, int offset) {
			ParametersTypeDescriptor parameters = GetItemParameters(child);
			int visibleIndex = GetItemVisibleIndex(Children.IndexOf(child), ActiveItemIndex, Children.Count, VisibleItemCountWrapper, AttractorPointIndex, offset);
			ItemTransfer transfer = CalculateItemTransfer(visibleIndex, Children.Count, VisibleItemCountWrapper, offset);
			CorrectTransfer(parameters.Position, transfer);
			parameters.Transfer = transfer;
		}
		void CorrectTransfer(double position, ItemTransfer transfer) {
			if (CurrentOffset != 0)
				transfer.Truncate(position);
		}
		void OnRendered(object sender, EventArgs e) {
			frames++;
			if (IsDesignMode)
				return;
			if (renderListener.IsListening)
				InvalidateArrange();
		}
		int frames = 0;
		protected internal virtual void ProcessChildren() {
			AnimationProgress = GetAnimationProgress();
			if (AnimationProgress >= 1 || AnimationDisabled)
				StopAnimation();
			if (CurrentOffset == 0)
				CalculateTransfers(CurrentOffset);
			bool shouldNotifyAll = false;
			foreach (UIElement child in Children) {
				ProcessChild(child);
				if (child.Visibility == Visibility.Visible && !shouldNotifyAll) {
					if (CalcParametersSkiplist(child).Count == 0)
						shouldNotifyAll = true;
				}
			}
			List<String> skipList = new List<string>();
			if (!shouldNotifyAll)
				skipList.Add("ZIndex");
			foreach (UIElement child in Children) {
				if (child.Visibility == Visibility.Visible) {
					ParametersTypeDescriptor paramters = GetItemParameters(child);
					paramters.NotifyParametersChanged(skipList);
				}
			}
		}
		double GetAnimationProgress() {
			return Stopwatch.ElapsedMilliseconds / AnimationTime;
		}
		void ProcessChild(UIElement child) {
			UpdatePosition(child);
			UpdateChildVisibility(child);
		}
		void UpdatePosition(UIElement child) {
			ParametersTypeDescriptor paramters = GetItemParameters(child);
			double position = paramters.Transfer.GetPosition(AnimationProgress);
			double distortedPosition = GetDistortedPosition(position, paramters.Transfer, AnimationProgress);
			paramters.Position = position;
		}
		void UpdateChildVisibility(UIElement child) {
			Visibility visibility = CalcVisibility(child);
			if (child.Visibility != visibility)
				child.Visibility = visibility;
		}
		internal Visibility CalcVisibility(UIElement child) {
			Visibility visibility = Visibility.Collapsed;
			if (IsVisiblePosition(GetDistortedPosition(child))) {
				visibility = Visibility.Visible;
				if (!IsRepeat && IsVisiblePosition(GetDistortedPosition(Children[0])) && !(FirstChildPosition <= AttractorPointPosition != GetPosition(child) < FirstChildPosition))
					visibility = Visibility.Collapsed;
			}
			return visibility;
		}
		static bool IsVisiblePosition(double position) {
			return 0 <= position && position <= 1;
		}
		double GetDistortedPosition(UIElement child) {
			ParametersTypeDescriptor paramters = GetItemParameters(child);
			return GetDistortedPosition(GetPosition(child), paramters.Transfer, AnimationProgress);
		}
		double GetPosition(UIElement child) {
			ParametersTypeDescriptor paramters = GetItemParameters(child);
			return paramters.Transfer.GetPosition(AnimationProgress);
		}
		double AttractorPointPosition {
			get { return 1d / (VisibleItemCountWrapper - 1) * AttractorPointIndex; }
		}
		double FirstChildPosition {
			get { return GetItemParameters(Children[0]).Position; }
		}
		protected double GetRequaredValue(UIElement child, string pName) {
			return ParameterSet[pName].DistributionFunction.GetValue(GetItemParameters(child).ActualPosition);
		}
		protected List<String> CalcParametersSkiplist(UIElement child) {
			List<String> result = new List<string>();
			OptimizeZIndex(child, result);
			return result;
		}
		void OptimizeZIndex(UIElement child, List<String> result) {
			if (ParameterSet == null) return;
			if (ParameterSet["ZIndex"] == null) return;
			List<UIElement> intersectedChildren = new List<UIElement>();
			Rect childBounds = LayoutHelper.GetRelativeElementRect(child, this);
			bool intersects = false;
			foreach (UIElement element in Children) {
				if (element.Visibility != Visibility.Visible || element == child) continue;
				if (childBounds.IntersectsWith(LayoutHelper.GetRelativeElementRect(element, this))) {
					intersectedChildren.Add(element);
					intersects = true;
				}
			}
			if (!intersects)
				result.Add("ZIndex");
			else {
				bool needChangeZindex = false;
				int actualZindex = Panel.GetZIndex(child);
				double requaredZindex = GetRequaredValue(child, "ZIndex");
				foreach (UIElement element in intersectedChildren) {
					int actualZindexE = Panel.GetZIndex(element);
					double requaredZindexE = GetRequaredValue(element, "ZIndex");
					if (requaredZindex > requaredZindexE && actualZindex > actualZindexE) continue;
					if (requaredZindex < requaredZindexE && actualZindex < actualZindexE) continue;
					if (requaredZindex == requaredZindexE && actualZindex == actualZindexE) continue;
					needChangeZindex = true; break;
				}
				if (!needChangeZindex) result.Add("ZIndex");
			}
		}
		static int GetVisibleIndex(int childIndex, int firstVisibleItemIndex, int childrenCount) {
			int result = childIndex - firstVisibleItemIndex;
			if (Math.Abs(result) > childrenCount - 1)
				result %= childrenCount;
			if (result < 0)
				result += childrenCount;
			return result;
		}
		internal double GetDistortedPosition(double position, ItemTransfer transfer, double animationProgress) {
			double result = position;
			if (OffsetAnimationAddFunction != null) {
				double distortion = OffsetAnimationAddFunction.GetValue(animationProgress);
				result += distortion * transfer.Distance;
			}
			return result;
		}
		internal double GetDistortedPosition(double position, ItemTransfer transfer) {
			return GetDistortedPosition(position, transfer, AnimationProgress);
		}
		ParametersTypeDescriptor GetItemParameters(UIElement element) {
			ParametersTypeDescriptor parameters = GetParameters(element);
			if (parameters == null) {
				parameters = GetStubParameters();
				SetParameters(element, parameters);
			}
			return parameters;
		}
		ParametersTypeDescriptor GetStubParameters() {
			ParametersTypeDescriptor parameters = new ParametersTypeDescriptor(this);
			List<Range> ranges = new List<Range>();
			ranges.Add(new Range(0, 0));
			ItemTransfer fakeTransfer = new ItemTransfer(ranges);
			fakeTransfer.IsActual = false;
			parameters.Transfer = fakeTransfer;
			return parameters;
		}
		void StartAnimation() {
			ResetFrames();
			Stopwatch.Start();
			renderListener.Start();
		}
		void ResetFrames() {
			frames = 0;
		}
		void StopAnimation() {
			Stopwatch.Stop();
			renderListener.Stop();
			UpdateProperties();
			CurrentOffset = 0;
			AnimationProgress = 0;
		}
		void UpdateProperties() {
			ActiveItemIndex = GetValidIndex(ActiveItemIndex + CurrentOffset);
		}
		protected int CalcChildrenIndex(int visibleIndex) {
			int result = FirstVisibleItemIndex + visibleIndex;
			if (Math.Abs(result) >= Children.Count) result = result % Children.Count;
			if (result < 0) result += Children.Count;
			return result;
		}
		protected List<UIElement> VisibleItems {
			get {
				var visibleItems = new List<UIElement>();
				for (int i = 0; i < VisibleItemCountWrapper; i++)
					visibleItems.Add(Children[CalcChildrenIndex(i)]);
				return visibleItems;
			}
		}
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return new CarouselControlUIElementCollection(this, logicalParent);
		}
		protected Size CalculateCarouselSize(Size availableSize) {
			if (IsDesignMode)
				return new Size();
			Size carouselSize = availableSize;
			if (!double.IsNaN(Width) && !double.IsInfinity(Width))
				carouselSize.Width = Width;
			if (!double.IsNaN(Height) && !double.IsInfinity(Height))
				carouselSize.Height = Height;
			Size pathSize = Path.Bounds.Size;
			if (double.IsInfinity(carouselSize.Width)) {
				if (PathPadding.Left + PathPadding.Right < 100)
					carouselSize.Width = double.IsNaN(Width) ? ItemMovingPath.Bounds.Size.Width / (1 - (PathPadding.Left + PathPadding.Right) / 100) : Width;
				else
					carouselSize.Width = ItemMovingPath.Bounds.Size.Width;
			}
			if (double.IsInfinity(carouselSize.Height)) {
				if (PathPadding.Top + PathPadding.Bottom < 100)
					carouselSize.Height = double.IsNaN(Height) ? ItemMovingPath.Bounds.Size.Height / (1 - (PathPadding.Top + PathPadding.Bottom) / 100) : Height;
				else
					carouselSize.Height = ItemMovingPath.Bounds.Size.Height;
			}
			return carouselSize;
		}
		void CalculatePathScale(Size carouselSize) {
			Size pathSize = Path.Bounds.Size;
			double scaleX = 0;
			double scaleY = 0;
			if (PathPadding.Left + PathPadding.Right < 100)
				scaleX = (carouselSize.Width - (PathPadding.Left + PathPadding.Right) * carouselSize.Width / 100) / pathSize.Width;
			if (PathPadding.Top + PathPadding.Bottom < 100)
				scaleY = (carouselSize.Height - (PathPadding.Top + PathPadding.Bottom) * carouselSize.Height / 100) / pathSize.Height;
			PathScaleX = scaleX;
			PathScaleY = scaleY;
		}
		void OnVisualChildrenChangedProcessChild(UIElement uiElement) {
		}
		void InvalidateScrollInfo() {
			if (scrollOwner != null)
				scrollOwner.InvalidateScrollInfo();
		}
		virtual protected PathGeometry DefaultPath {
			get {
				Geometry g = Geometry.Parse("M466.5,145.5 C466.5,225.58129 362.18235,290.5 233.5,290.5 C104.81765,290.5 0.5,225.58129 0.5,145.5");
				PathGeometry pg = new PathGeometry();
				pg.AddGeometry(g);
				return pg;
			}
		}
		#endregion
		#region Override methods
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new CarouselPanelAutomationPeer(this);
		}
		protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost) {
			var itemsOwner = ItemsControl.GetItemsOwner(this);
			if (itemsOwner == null)
				return;
			if (itemsOwner.GetType() == typeof(CarouselItemsControl))
				((CarouselItemsControl)itemsOwner).Carousel = this;
			else
				SetCarousel(itemsOwner, this);
			base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
		}
		internal int renderCount = 0;
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (PathVisible)
				DrawPath(dc);
			if (ShowFps) {
				DrawFPS(dc);
			}
			renderCount++;
		}
		void DrawPath(DrawingContext dc) {
			PathGeometry pg = TransformedPath;
			dc.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Gray, 0.5), pg);
		}
		void DrawFPS(DrawingContext dc) {
			double fps = frames / ((AnimationTime) / 1000.0);
			string fpsText = fps.ToString("fps = 0.0");
			dc.DrawText(new FormattedText(fpsText, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 32, Brushes.Gray), new Point(0, 0));
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if (!ActivateItemOnClick || CurrentOffset != 0)
				return;
			DependencyObject item = (DependencyObject)e.Source;
			while (item != this && item != null) {
				UIElement uiElement = item as UIElement;
				if (uiElement != null)
					MoveTo(uiElement);
				item = (DependencyObject)VisualTreeHelper.GetParent(item);
			}
			Focus();
		}
		public void MoveTo(UIElement item) {
			if (item == null || !Children.Contains(item))
				return;
			MoveTo(Children.IndexOf(item));
		}
		public void MoveTo(int index) {
			if (index < 0 && index > Children.Count - 1)
				return;
			Move(CalcMoveCount(index, ActiveItemIndex, AttractorPointIndex, Children.Count, VisibleItemCountWrapper, IsRepeat, CurrentOffset));
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			switch (e.Key) {
				case Key.Right:
					MoveNext();
					e.Handled = true;
					break;
				case Key.Left:
					MovePrev();
					e.Handled = true;
					break;
				case Key.Up:
				case Key.Down:
					e.Handled = true;
					break;
			}
		}
		internal static int CalcMoveCount(int newActiveItemIndex, int activeItemIndex, int attractorPointIndex, int childrenCount, int visibleItemCount, bool isRepeat) {
			return CalcMoveCount(newActiveItemIndex, activeItemIndex, attractorPointIndex, childrenCount, visibleItemCount, isRepeat, 0);
		}
		internal static int CalcMoveCount(int newActiveItemIndex, int activeItemIndex, int attractorPointIndex, int childrenCount, int visibleItemCount, bool isRepeat, int offset) {
			int result = 0;
			if (newActiveItemIndex > childrenCount - 1 || newActiveItemIndex < 0)
				return result;
			if (!isRepeat)
				result = newActiveItemIndex - activeItemIndex - offset;
			else
				if (offset == 0) {
					int newActiveItemVisibleIndex = GetItemVisibleIndex(newActiveItemIndex, activeItemIndex, childrenCount, visibleItemCount, attractorPointIndex, 0);
					result = newActiveItemVisibleIndex - attractorPointIndex - offset;
					if (childrenCount < visibleItemCount && isRepeat && Math.Sign(attractorPointIndex - newActiveItemVisibleIndex) == Math.Sign(newActiveItemIndex - activeItemIndex))
						result += Math.Sign(attractorPointIndex - newActiveItemVisibleIndex) * visibleItemCount;
				} else
					result = newActiveItemIndex - activeItemIndex - offset;
			return result;
		}
		protected override Size MeasureOverride(Size availableSize) {
			CarouselSize = CalculateCarouselSize(availableSize);
			CalculatePathScale(CarouselSize);
			Size itemSize = ItemSize;
			if (IsAutoSizeItem)
				itemSize = new Size(ItemSize.Width * PathScale, ItemSize.Height * PathScale);
			foreach (UIElement item in Children)
				if (item != null && item.Visibility == Visibility.Visible && !double.IsNaN(itemSize.Width) && !double.IsNaN(itemSize.Height))
					item.Measure(itemSize);
			return CarouselSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (IsLoaded)
				ProcessChildren();
			Size itemSize = ItemSize;
			if (IsAutoSizeItem)
				itemSize = new Size(ItemSize.Width * PathScale, ItemSize.Height * PathScale);
			foreach (UIElement item in Children)
				if (item != null && item.Visibility == Visibility.Visible && !double.IsNaN(itemSize.Width) && !double.IsNaN(itemSize.Height))
					item.Arrange(new Rect(new Point(-itemSize.Width / 2, -itemSize.Height / 2), itemSize));
			return finalSize;
		}
		#endregion
		#region For tests
#if DEBUGTEST
		internal Size TestCalculateCarouselSize(Size availableSize) {
			return CalculateCarouselSize(availableSize);
		}
		internal void TestCalculatePathScale(Size availableSize) {
			CalculatePathScale(availableSize);
		}
#endif
		#endregion
		#region IScrollInfo Members
		Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle) {
			return Rect.Empty;
		}
		bool IScrollInfo.CanHorizontallyScroll {
			get { return true; }
			set { InvalidateScrollInfo(); }
		}
		bool IScrollInfo.CanVerticallyScroll {
			get { return true; }
			set { InvalidateScrollInfo(); }
		}
		double IScrollInfo.ExtentHeight {
			get { return Children.Count; }
		}
		double IScrollInfo.ExtentWidth {
			get { return Children.Count; }
		}
		double IScrollInfo.HorizontalOffset {
			get { return ActiveItemIndex + CurrentOffset; }
		}
		double IScrollInfo.VerticalOffset {
			get { return ActiveItemIndex + CurrentOffset; }
		}
		void IScrollInfo.LineDown() {
			MoveNext();
		}
		void IScrollInfo.LineUp() {
			MovePrev();
		}
		void IScrollInfo.LineRight() {
			MoveNext();
		}
		void IScrollInfo.LineLeft() {
			MovePrev();
		}
		void IScrollInfo.MouseWheelDown() {
			MoveNext();
		}
		void IScrollInfo.MouseWheelUp() {
			MovePrev();
		}
		void IScrollInfo.MouseWheelRight() {
			MoveNext();
		}
		void IScrollInfo.MouseWheelLeft() {
			MovePrev();
		}
		void IScrollInfo.PageDown() {
			MoveNextPage();
		}
		void IScrollInfo.PageUp() {
			MovePrevPage();
		}
		void IScrollInfo.PageRight() {
			MoveNextPage();
		}
		void IScrollInfo.PageLeft() {
			MovePrevPage();
		}
		ScrollViewer scrollOwner;
		ScrollViewer IScrollInfo.ScrollOwner {
			get { return scrollOwner; }
			set {
				scrollOwner = value;
				InvalidateScrollInfo();
			}
		}
		void IScrollInfo.SetHorizontalOffset(double offset) {
			SetScrollOffset(offset);
		}
		void IScrollInfo.SetVerticalOffset(double offset) {
			SetScrollOffset(offset);
		}
		double IScrollInfo.ViewportHeight {
			get { return 1d; }
		}
		double IScrollInfo.ViewportWidth {
			get { return 1d; }
		}
		void SetScrollOffset(double offset) {
			MoveTo((int)offset);
		}
		#endregion
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(ParameterCollectionChangedEventManager)) {
				ResetParameters();
				return true;
			}
			return false;
		}
	}
}
namespace DevExpress.Xpf.Carousel.Helpers {
	public static class CarouselPanelHelper {
		public static int GetRenderCount(CarouselPanel carousel) {
			return carousel.renderCount;
		}
	}
}
