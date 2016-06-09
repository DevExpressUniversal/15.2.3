﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

#if !MVVM
using DevExpress.Xpf.Core.Native;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Data;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Mvvm.UI {
	[TargetType(typeof(UserControl))]
	[TargetType(typeof(Window))]
	[ContentProperty("ThumbButtonInfos")]
	public class TaskbarButtonService : WindowAwareServiceBase, ITaskbarButtonService {
		#region Dependency Properties
		public static readonly DependencyProperty ProgressStateProperty =
			DependencyProperty.Register("ProgressState", typeof(TaskbarItemProgressState), typeof(TaskbarButtonService), 
			new FrameworkPropertyMetadata(TaskbarItemProgressState.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
				(d, e) => ((TaskbarButtonService)d).OnProgressStateChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemInfoProgressStateProperty =
			DependencyProperty.Register("ItemInfoProgressState", typeof(TaskbarItemProgressState), typeof(TaskbarButtonService), new PropertyMetadata(TaskbarItemProgressState.None,
				(d, e) => ((TaskbarButtonService)d).OnItemInfoProgressStateChanged(e)));
		public static readonly DependencyProperty ProgressValueProperty =
			DependencyProperty.Register("ProgressValue", typeof(double), typeof(TaskbarButtonService), 
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((TaskbarButtonService)d).OnProgressValueChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemInfoProgressValueProperty =
			DependencyProperty.Register("ItemInfoProgressValue", typeof(double), typeof(TaskbarButtonService), new PropertyMetadata(0.0,
				(d, e) => ((TaskbarButtonService)d).OnItemInfoProgressValueChanged(e)));
		public static readonly DependencyProperty OverlayIconProperty =
			DependencyProperty.Register("OverlayIcon", typeof(ImageSource), typeof(TaskbarButtonService),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((TaskbarButtonService)d).OnOverlayIconChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemInfoOverlayIconProperty =
			DependencyProperty.Register("ItemInfoOverlayIcon", typeof(ImageSource), typeof(TaskbarButtonService), new PropertyMetadata(null,
				(d, e) => ((TaskbarButtonService)d).OnItemInfoOverlayIconChanged(e)));
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(TaskbarButtonService), 
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((TaskbarButtonService)d).OnDescriptionChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemInfoDescriptionProperty =
			DependencyProperty.Register("ItemInfoDescription", typeof(string), typeof(TaskbarButtonService), new PropertyMetadata("",
				(d, e) => ((TaskbarButtonService)d).OnItemInfoDescriptionChanged(e)));
		public static readonly DependencyProperty ThumbButtonInfosProperty =
			DependencyProperty.Register("ThumbButtonInfos", typeof(TaskbarThumbButtonInfoCollection), typeof(TaskbarButtonService), new PropertyMetadata(null,
				(d, e) => ((TaskbarButtonService)d).OnThumbButtonInfosChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemInfoThumbButtonInfosProperty =
			DependencyProperty.Register("ItemInfoThumbButtonInfos", typeof(ThumbButtonInfoCollection), typeof(TaskbarButtonService), new PropertyMetadata(null,
				(d, e) => ((TaskbarButtonService)d).OnItemInfoThumbButtonInfosChanged(e)));
		public static readonly DependencyProperty ThumbnailClipMarginProperty =
			DependencyProperty.Register("ThumbnailClipMargin", typeof(Thickness), typeof(TaskbarButtonService), 
			new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((TaskbarButtonService)d).OnThumbnailClipMarginChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemInfoThumbnailClipMarginProperty =
			DependencyProperty.Register("ItemInfoThumbnailClipMargin", typeof(Thickness), typeof(TaskbarButtonService), new PropertyMetadata(new Thickness(),
				(d, e) => ((TaskbarButtonService)d).OnItemInfoThumbnailClipMarginChanged(e)));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty WindowItemInfoProperty =
			DependencyProperty.Register("WindowItemInfo", typeof(TaskbarItemInfo), typeof(TaskbarButtonService), new PropertyMetadata(null,
				(d, e) => ((TaskbarButtonService)d).OnWindowItemInfoChanged(e)));
		public static readonly DependencyProperty ThumbnailClipMarginCallbackProperty =
			DependencyProperty.Register("ThumbnailClipMarginCallback", typeof(Func<Size, Thickness>), typeof(TaskbarButtonService), new PropertyMetadata(null,
				(d, e) => ((TaskbarButtonService)d).OnThumbnailClipMarginCallbackChanged(e)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty InternalItemsProperty =
			DependencyProperty.RegisterAttached("InternalItems", typeof(FreezableCollection<TaskbarThumbButtonInfo>), typeof(TaskbarButtonService), new PropertyMetadata(null));
		internal static FreezableCollection<TaskbarThumbButtonInfo> GetInternalItems(TaskbarButtonService obj) {
			return (FreezableCollection<TaskbarThumbButtonInfo>)obj.GetValue(InternalItemsProperty);
		}
		internal static void SetInternalItems(TaskbarButtonService obj, FreezableCollection<TaskbarThumbButtonInfo> value) {
			obj.SetValue(InternalItemsProperty, value);
		}
		#endregion
		TaskbarItemInfo itemInfo;
		public TaskbarButtonService() {
			SetInternalItems(this, new FreezableCollection<TaskbarThumbButtonInfo>());
			ItemInfo = new TaskbarItemInfo();
		}
		public TaskbarItemProgressState ProgressState { get { return (TaskbarItemProgressState)GetValue(ProgressStateProperty); } set { SetValue(ProgressStateProperty, value); } }
		public double ProgressValue { get { return (double)GetValue(ProgressValueProperty); } set { SetValue(ProgressValueProperty, value); } }
		public ImageSource OverlayIcon { get { return (ImageSource)GetValue(OverlayIconProperty); } set { SetValue(OverlayIconProperty, value); } }
		public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
		public TaskbarThumbButtonInfoCollection ThumbButtonInfos { get { return (TaskbarThumbButtonInfoCollection)GetValue(ThumbButtonInfosProperty); } set { SetValue(ThumbButtonInfosProperty, value); } }
		public Thickness ThumbnailClipMargin { get { return (Thickness)GetValue(ThumbnailClipMarginProperty); } set { SetValue(ThumbnailClipMarginProperty, value); } }
		public Func<Size, Thickness> ThumbnailClipMarginCallback { get { return (Func<Size, Thickness>)GetValue(ThumbnailClipMarginCallbackProperty); } set { SetValue(ThumbnailClipMarginCallbackProperty, value); } }
		public void UpdateThumbnailClipMargin() {
			if(ActualWindow != null && ThumbnailClipMarginCallback != null)
				ThumbnailClipMargin = ThumbnailClipMarginCallback(new Size(ActualWindow.Width, ActualWindow.Height));
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateInternalItems();
		}
		protected override void OnDetaching() {
			GetInternalItems(this).Clear();
			base.OnDetaching();
		}
		protected override Freezable CreateInstanceCore() { return this; }
		protected virtual void OnProgressStateChanged(DependencyPropertyChangedEventArgs e) {
			ItemInfo.ProgressState = (TaskbarItemProgressState)e.NewValue;
		}
		protected virtual void OnProgressValueChanged(DependencyPropertyChangedEventArgs e) {
			double newValue = (double)e.NewValue;
			if(Math.Abs(ItemInfo.ProgressValue - newValue) > Double.Epsilon)
				ItemInfo.ProgressValue = newValue;
		}
		protected virtual void OnOverlayIconChanged(DependencyPropertyChangedEventArgs e) {
			ItemInfo.Overlay = (ImageSource)e.NewValue;
		}
		protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e) {
			ItemInfo.Description = (string)e.NewValue;
		}
		protected virtual void OnThumbnailClipMarginChanged(DependencyPropertyChangedEventArgs e) {
			ItemInfo.ThumbnailClipMargin = (Thickness)e.NewValue;
		}
		protected virtual void OnThumbnailClipMarginCallbackChanged(DependencyPropertyChangedEventArgs e) {
			UpdateThumbnailClipMargin();
		}
		protected virtual void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateThumbnailClipMargin();
		}
		TaskbarItemInfo ItemInfo {
			get { return itemInfo; }
			set {
				if(itemInfo == value) return;
				itemInfo = value;
				BindingOperations.SetBinding(this, ItemInfoProgressStateProperty, new Binding("ProgressState") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				BindingOperations.SetBinding(this, ItemInfoProgressValueProperty, new Binding("ProgressValue") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				BindingOperations.SetBinding(this, ItemInfoOverlayIconProperty, new Binding("Overlay") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				BindingOperations.SetBinding(this, ItemInfoDescriptionProperty, new Binding("Description") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				BindingOperations.SetBinding(this, ItemInfoThumbnailClipMarginProperty, new Binding("ThumbnailClipMargin") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				BindingOperations.SetBinding(this, ItemInfoThumbButtonInfosProperty, new Binding("ThumbButtonInfos") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			}
		}
		void OnItemInfoProgressStateChanged(DependencyPropertyChangedEventArgs e) {
			ProgressState = (TaskbarItemProgressState)e.NewValue;
		}
		void OnItemInfoProgressValueChanged(DependencyPropertyChangedEventArgs e) {
			double newValue = (double)e.NewValue;
			if(Math.Abs(ProgressValue - newValue) > Double.Epsilon)
				ProgressValue = (double)e.NewValue;
		}
		void OnItemInfoOverlayIconChanged(DependencyPropertyChangedEventArgs e) {
			OverlayIcon = (ImageSource)e.NewValue;
		}
		void OnItemInfoDescriptionChanged(DependencyPropertyChangedEventArgs e) {
			Description = (string)e.NewValue;
		}
		protected virtual void OnThumbButtonInfosChanged(DependencyPropertyChangedEventArgs e) {
			TaskbarThumbButtonInfoCollection collection = (TaskbarThumbButtonInfoCollection)e.NewValue;
			ItemInfo.ThumbButtonInfos = collection.InternalCollection;
			UpdateInternalItems();
		}
		bool lockUpdateInternalItems;
		void UpdateInternalItems() {
			if(lockUpdateInternalItems) return;
			if(!ShouldUpdateInternalItems()) return;
			try {
				lockUpdateInternalItems = true;
				UpdateInternalItemsCore();
			} finally { lockUpdateInternalItems = false; }
		}
		internal virtual void UpdateInternalItemsCore() {
			GetInternalItems(this).Clear();
			foreach(TaskbarThumbButtonInfo item in ThumbButtonInfos)
				GetInternalItems(this).Add(item);
		}
		bool ShouldUpdateInternalItems() {
			if(!IsAttached) return false;
			TaskbarThumbButtonInfoCollection collection = ThumbButtonInfos;
			FreezableCollection<TaskbarThumbButtonInfo> collection2 = GetInternalItems(this);
			if(collection.Count != collection2.Count) return true;
			for(int i = 0; i < collection.Count; i++) {
				var item1 = collection[i];
				var item2 = collection2[i];
				if(item1 != item2) return true;
			}
			return false;
		}
		void OnItemInfoThumbButtonInfosChanged(DependencyPropertyChangedEventArgs e) {
			ThumbButtonInfos = new TaskbarThumbButtonInfoCollection((ThumbButtonInfoCollection)e.NewValue);
		}
		void OnItemInfoThumbnailClipMarginChanged(DependencyPropertyChangedEventArgs e) {
			ThumbnailClipMargin = (Thickness)e.NewValue;
		}
		protected override void OnActualWindowChanged(Window oldWindow) {
			if(oldWindow != null)
				oldWindow.SizeChanged -= OnWindowSizeChanged;
			Window window = ActualWindow;
			if(window == null) {
				BindingOperations.ClearBinding(this, WindowItemInfoProperty);
				ItemInfo = new TaskbarItemInfo();
				return;
			}
			if(window.TaskbarItemInfo == null) {
				window.TaskbarItemInfo = ItemInfo;
			} else {
				window.TaskbarItemInfo.ProgressState = ItemInfo.ProgressState;
				window.TaskbarItemInfo.ProgressValue = ItemInfo.ProgressValue;
				window.TaskbarItemInfo.Description = ItemInfo.Description;
				window.TaskbarItemInfo.Overlay = ItemInfo.Overlay;
				window.TaskbarItemInfo.ThumbButtonInfos = ItemInfo.ThumbButtonInfos;
				window.TaskbarItemInfo.ThumbnailClipMargin = ItemInfo.ThumbnailClipMargin;
				ItemInfo = window.TaskbarItemInfo;
			}
			BindingOperations.SetBinding(this, WindowItemInfoProperty, new Binding("TaskbarItemInfo") { Source = window, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			window.SizeChanged -= OnWindowSizeChanged;
			window.SizeChanged += OnWindowSizeChanged;
			OnWindowSizeChanged(window, null);
		}
		void OnWindowItemInfoChanged(DependencyPropertyChangedEventArgs e) {
			if(ActualWindow == null) return;
			TaskbarItemInfo itemInfo = (TaskbarItemInfo)e.NewValue;
			if(itemInfo == null) {
				itemInfo = new TaskbarItemInfo();
				ActualWindow.TaskbarItemInfo = itemInfo;
			}
			ItemInfo = itemInfo;
		}
		IList<TaskbarThumbButtonInfo> ITaskbarButtonService.ThumbButtonInfos { get { return ThumbButtonInfos; } }
	}
}
