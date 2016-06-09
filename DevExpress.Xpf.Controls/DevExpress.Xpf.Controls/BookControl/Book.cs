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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Services;
using DevExpress.Services.Internal;
using DevExpress.Xpf.Controls.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils; 
using DevExpress.Utils;
using Keys = System.Windows.Forms.Keys;
namespace DevExpress.Xpf.Controls {
	public enum PageType { Even, Odd }
	[DXToolboxBrowsable]
	public class Book : Control, IServiceContainer {
		#region Constants
		protected internal static readonly Size DefaultActiveAreaSize = new Size(100, 100);
		protected internal const double DefaultAnimationRate = 25.0;
		protected internal const double DefaultAnimationSpeed = 1200.0;
		protected internal const double DefaultShortAnimationSpeed = 50.0;
		#endregion
		#region DependencyProperty
		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(IList), typeof(Book), new PropertyMetadata(null, (d, e) => ((Book)d).UpdateAllProperties()));
		public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(Book), new PropertyMetadata(-1, (d, e) => ((Book)d).OnPageIndexChange((int)e.OldValue, (int)e.NewValue)));
		public static readonly DependencyProperty PageTemplateProperty = DependencyProperty.Register("PageTemplate", typeof(DataTemplate), typeof(Book), null);
		public static readonly DependencyProperty ActiveAreaSizeProperty = DependencyProperty.Register("ActiveAreaSize", typeof(Size), typeof(Book), new PropertyMetadata(DefaultActiveAreaSize));
		public static readonly DependencyProperty AnimationRateProperty = DependencyProperty.Register("AnimationRate", typeof(double), typeof(Book), new PropertyMetadata(DefaultAnimationRate));
		public static readonly DependencyProperty AnimationSpeedProperty = DependencyProperty.Register("AnimationSpeed", typeof(double), typeof(Book), new PropertyMetadata(DefaultAnimationSpeed));
		public static readonly DependencyProperty ShortAnimationSpeedProperty = DependencyProperty.Register("ShortAnimationSpeed", typeof(double), typeof(Book), new PropertyMetadata(DefaultShortAnimationSpeed));
		public static readonly DependencyProperty FirstPageProperty = DependencyProperty.Register("FirstPage", typeof(PageType), typeof(Book), new PropertyMetadata(PageType.Even, (d, e) => ((Book)d).UpdateAllProperties()));
		#endregion
		#region Property
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookDataSource")]
#endif
		public IList DataSource { get { return (IList)GetValue(DataSourceProperty); } set { SetValue(DataSourceProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookPageIndex")]
#endif
		public int PageIndex { get { return (int)GetValue(PageIndexProperty); } set { SetValue(PageIndexProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookPageTemplate")]
#endif
		public DataTemplate PageTemplate { get { return (DataTemplate)GetValue(PageTemplateProperty); } set { SetValue(PageTemplateProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookActiveAreaSize")]
#endif
		public Size ActiveAreaSize { get { return (Size)GetValue(ActiveAreaSizeProperty); } set { SetValue(ActiveAreaSizeProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookPageCount")]
#endif
		public int PageCount { get { return DataSource.Count; } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookAnimationRate")]
#endif
		public double AnimationRate { get { return (double)GetValue(AnimationRateProperty); } set { SetValue(AnimationRateProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookAnimationSpeed")]
#endif
		public double AnimationSpeed { get { return (double)GetValue(AnimationSpeedProperty); } set { SetValue(AnimationSpeedProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookShortAnimationSpeed")]
#endif
		public double ShortAnimationSpeed { get { return (double)GetValue(ShortAnimationSpeedProperty); } set { SetValue(ShortAnimationSpeedProperty, value); } }
#if !SL
	[DevExpressXpfControlsLocalizedDescription("BookFirstPage")]
#endif
		public PageType FirstPage { get { return (PageType)GetValue(FirstPageProperty); } set { SetValue(FirstPageProperty, value); } }
		public Point PartialTurnEndPoint { get; set; }
		protected internal BookGeometryParams GeometryParams { get; set; }
		protected internal BookEventHandler EventHandler { get; set; }
		protected internal BookDragTracker DragTracker { get; set; }
		protected internal BookPageManager PageManager { get; set; }
		protected internal BookAnimator Animator { get; set; }
		protected internal virtual double BookWidth { get { return ActualWidth; } }
		protected internal virtual double BookHeight { get { return ActualHeight; } }
		protected BookViewState ViewState { get { return EventHandler.ViewState; } }
		protected NormalKeyboardHandler KeyboardHandler { get; set; }
		protected ServiceManager ServiceManager { get; set; }
		#endregion
		#region Events
		public event EventHandler PageIndexChanged;
		public event PageIndexChangingEventHandler PageIndexChanging;
		#endregion
		#region Locals
		bool isPageIndexChanging = false;
		#endregion
		public Book() {
			DefaultStyleKey = typeof(Book);
			PageManager = CreatePageManager();
			Animator = CreateAnimator();
			GeometryParams = new BookGeometryParams(this);
			EventHandler = new BookEventHandler(this);
			DragTracker = new BookDragTracker(this);
			this.SizeChanged += (d, e) => UpdateAllProperties();
			KeyboardHandler = CreateKeyboardHandler();
			ServiceManager = CreateServiceManager();
			InitializeKeyboardHandler();
			AddServices();
		}
		protected virtual BookPageManager CreatePageManager() { return new BookPageManager(this); }
		protected virtual BookAnimator CreateAnimator() { return new BookAnimator(this); }
		protected virtual void InitializeKeyboardHandler() {
			KeyboardHandler.RegisterKeyHandler(new BookKeyHashProvider(), Keys.Left, Keys.None, typeof(BookPreviousPageCommand));
			KeyboardHandler.RegisterKeyHandler(new BookKeyHashProvider(), Keys.Up, Keys.None, typeof(BookPreviousPageCommand));
			KeyboardHandler.RegisterKeyHandler(new BookKeyHashProvider(), Keys.Right, Keys.None, typeof(BookNextPageCommand));
			KeyboardHandler.RegisterKeyHandler(new BookKeyHashProvider(), Keys.Down, Keys.None, typeof(BookNextPageCommand));
		}
		protected virtual void AddServices() {
			AddService(typeof(IKeyboardHandlerService), new BookKeyboardHandlerService(KeyboardHandler));
		}
		protected virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		protected internal virtual NormalKeyboardHandler CreateKeyboardHandler() {
			return new NormalKeyboardHandler(this);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PageManager.OnApplyTemplate();
			UpdateAllProperties();
		}
		protected internal virtual void UpdateAllProperties() {
			if(PageManager != null)
				PageManager.UpdateAllProperties();
		}
		protected virtual void OnPageIndexChange(int oldValue, int newValue) {
			if(!isPageIndexChanging) {
				int index = RaisePageIndexChanging(oldValue, newValue);
				if(index != newValue) {
					isPageIndexChanging = true;
					SetValue(PageIndexProperty, index);
					isPageIndexChanging = false;
				}
				if(index != oldValue)
					RaisePageIndexChanged(oldValue, newValue);
			}
			UpdateAllProperties();
		}
		protected virtual int RaisePageIndexChanging(int oldValue, int newValue) {
			if(PageIndexChanging == null)
				return newValue;
			PageIndexChangingEventArgs e = new PageIndexChangingEventArgs(oldValue, newValue);
			PageIndexChanging(this, e);
			return e.Cancel ? e.OldPageIndex : e.NewPageIndex;
		}
		protected virtual void RaisePageIndexChanged(int oldValue, int newValue) {
			if(PageIndexChanged == null)
				return;
			PageIndexChanged(this, EventArgs.Empty);
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			Focus();
			EventHandler.OnMouseLeftButtonDown(e.GetPosition(this));
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			EventHandler.OnMouseLeftButtonUp(e.GetPosition(this));
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseMove(e);
			EventHandler.OnMouseMove(e.GetPosition(this));
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			EventHandler.OnMouseLeave();
		}
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
			var ev = e.ToPlatformIndependent();
			IKeyboardHandlerService svc = (IKeyboardHandlerService)GetService(typeof(IKeyboardHandlerService));
			if(svc != null)
				svc.OnKeyDown(ev);
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e) {
			var ev = e.ToPlatformIndependent();
			IKeyboardHandlerService svc = (IKeyboardHandlerService)GetService(typeof(IKeyboardHandlerService));
			if(svc != null)
				svc.OnKeyUp(ev);
			base.OnKeyUp(e);
		}
		protected internal BookTemplateElement GetTemplateElement(string name) {
			FrameworkElement element = (FrameworkElement)GetTemplateChild(name);
			return element != null ? new BookTemplateElement(element) : new NullBookTemplateElement();
		}
		protected internal BookTemplateElement GetTemplateElement(BookTemplateElementType element, BookPageLayout layout) {
			string name = layout.ToString() + element.ToString();
			FrameworkElement fe = (FrameworkElement)GetTemplateChild(name);
			if(fe == null) return new NullBookTemplateElement();
			switch(element) {
				case BookTemplateElementType.BaseForeShadow:
				case BookTemplateElementType.OverlayForeShadow:
				case BookTemplateElementType.BackShadow:
					return new BookShadowTemplateElement(fe);
				default: 
					return new BookTemplateElement(fe);
			}
		}
		#region IServiceContainer
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if(ServiceManager != null)
				ServiceManager.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if(ServiceManager != null)
				ServiceManager.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if(ServiceManager != null)
				ServiceManager.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if(ServiceManager != null)
				ServiceManager.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if(ServiceManager != null)
				ServiceManager.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if(ServiceManager != null)
				ServiceManager.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			if(ServiceManager == null) return null;
			return ServiceManager.GetService(serviceType);
		}
		#endregion
	}
	public delegate void PageIndexChangingEventHandler(object sender, PageIndexChangingEventArgs e);
	public class PageIndexChangingEventArgs : CancelEventArgs {
		public int OldPageIndex { get; private set; }
		public int NewPageIndex { get; set; }
		public PageIndexChangingEventArgs(int oldValue, int newValue) {
			OldPageIndex = oldValue;
			NewPageIndex = newValue;
		}
	}
}
