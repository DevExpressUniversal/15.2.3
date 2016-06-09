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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core {
#if SL
	public interface IPopupContainer {
		Decorator PopupContainer { get; set; }
	}
	public class PopupContainer : Decorator { }
#endif
	public class ColumnChooserBase : DependencyObject, IColumnChooser {
		#region static
		public static readonly DependencyProperty CaptionProperty;
		static ColumnChooserBase() {
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), typeof(ColumnChooserBase), new PropertyMetadata(string.Empty));
		}		
		#endregion static
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		bool isStateApplied;
		FloatingContainer container;
		ILogicalOwner owner;
		public ColumnChooserBase(ILogicalOwner owner) {
			if(owner as Control == null)
				throw new ArgumentException("owner should be a Control class descendant");
			this.owner = owner;
			container = FloatingContainerFactory.Create(FloatingContainerFactory.CheckPopupHost((Control)Owner));
#if!SILVERLIGHT
			container.UseActiveStateOnly = true;
#else
			((Control)Owner).Unloaded += OnOwnerUnloaded;
#endif
			Owner.AddChild(Container);
			Container.Owner = (Control)Owner;
			Container.LogicalOwner = (Control)Owner;
			Container.Content = CreateContentControl();
			Container.Hidden += OnContainerHidden;
			BindingOperations.SetBinding(Container, FloatingContainer.CaptionProperty, new Binding("Caption") { Source = this });
		}
#if SL
		protected virtual void OnOwnerUnloaded(object sender, RoutedEventArgs e) {
		}
#endif
		public FloatingContainer Container { get { return container; } }
		protected virtual ILogicalOwner Owner { get { return owner; } }
		protected IColumnChooserState ApplyStateParameter { get; private set; }
		protected virtual Control CreateContentControl() {
			return new Control();
		}
		protected virtual void OnContainerHidden(object sender, RoutedEventArgs e) { }
		protected virtual void OnShowingOwnerLoaded(object sender, RoutedEventArgs e) {
			Show();
			Owner.Loaded -= OnShowingOwnerLoaded;
		}
		protected virtual void OnApplyingStateOwnerLoaded(object sender, RoutedEventArgs e) {
			ApplyState(ApplyStateParameter);
			Owner.Loaded -= OnApplyingStateOwnerLoaded;
			ApplyStateParameter = null;
		}
		internal Point UpdateContainerLocation(Rect rect) {
#if SL
			return GetLocation(rect.Location());
#else
			if(System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
				return CheckLocationForXBAP(rect.Location);
			if(container != null && container.FlowDirection == FlowDirection.RightToLeft)
				rect.X = rect.X + rect.Width;
			rect.Location = ScreenHelper.GetScreenLocation(rect.Location, (FrameworkElement)Owner);
			ScreenHelper.UpdateContainerLocation(rect);
			return ScreenHelper.GetClientLocation(ScreenHelper.UpdateContainerLocation(rect), (FrameworkElement)Owner);
#endif
		}
#if SL
		Point GetLocation(Point point) {
			UIElement parent = Owner as UIElement;
			Rect rect = LayoutHelper.IsInVisualTree(parent) && FrameworkElementExtensions.IsVisible(parent as FrameworkElement) ? LayoutHelper.GetRelativeElementRect(parent as FrameworkElement, Application.Current.RootVisual) : Rect.Empty;
			return new Point(((rect.Left + point.X < 0) && rect.Left + point.X + Container.FloatSize.Width > 0) ? 1 - rect.Left : point.X,
				((rect.Top + point.Y < 0) && rect.Top + point.Y + Container.FloatSize.Height > 0) ? 1 - rect.Top : point.Y);
		}
#endif
		Point CheckLocationForXBAP(Point location) {
			location.X = (location.X < 0) ? 0 : location.X;
			location.Y = (location.Y < 0) ? 0 : location.Y;
			return location;
		}
		#region IColumnChooser Members
		public virtual void Show() {
			if(Owner.IsLoaded) {
				Container.IsOpen = true;
			} else {
				Owner.Loaded += OnShowingOwnerLoaded;
			}
		}
		public virtual void Hide() {
			Container.IsOpen = false;
		}
		public void ApplyState(IColumnChooserState istate) {
			istate = istate ?? DefaultColumnChooserState.DefaultState;
			DefaultColumnChooserState state = istate as DefaultColumnChooserState;
			if(state == null)
				return;
			if(Owner.IsLoaded) {
				isStateApplied = true;
				Size size = state.Size;
				double minWidth = state.MinWidth;
				double minHeight = state.MinHeight;
				if(ThemeHelper.IsTouchTheme(Container)) {
					if(state.Size == (Size)DefaultColumnChooserState.SizeProperty.GetDefaultValue())
						size = new Size(280, 360);
					if(state.MinWidth == (double)DefaultColumnChooserState.MinWidthProperty.GetDefaultValue())
						minWidth = 260;
					if(state.MinHeight == (double)DefaultColumnChooserState.MinHeightProperty.GetDefaultValue())
						minHeight = 260;
				}
				Container.FloatSize = size;
				Container.FloatLocation = UpdateContainerLocation(new Rect(new Point(
					double.IsNaN(state.Location.X)
						? Owner.ActualWidth - size.Width
						: state.Location.X,
					double.IsNaN(state.Location.Y)
						? Owner.ActualHeight - size.Height
						: state.Location.Y), size));
				Container.MinWidth = minWidth;
				Container.MaxWidth = state.MaxWidth;
				Container.MinHeight = minHeight;
				Container.MaxHeight = state.MaxHeight;
			} else {
				Owner.Loaded += OnApplyingStateOwnerLoaded;
				ApplyStateParameter = state;
			}
		}
		public void SaveState(IColumnChooserState istate) {
			if(!isStateApplied) return;
			DefaultColumnChooserState state = istate as DefaultColumnChooserState;
			if(state == null)
				return;
			state.Location = Container.FloatLocation;
			state.Size = Container.FloatSize;
		}
		public UIElement TopContainer { get { return (UIElement)Container.Content; } }
		public void Destroy() {
			Owner.RemoveChild(Container);
			Container.Close();
		}
		#endregion
	}
	public class DefaultColumnChooserState : DependencyObject, IColumnChooserState {
		public static readonly DependencyProperty LocationProperty =
			DependencyPropertyManager.Register("Location", typeof(Point), typeof(DefaultColumnChooserState), new UIPropertyMetadata(new Point(double.NaN, double.NaN)));
		public static readonly DependencyProperty SizeProperty =
			DependencyPropertyManager.Register("Size", typeof(Size), typeof(DefaultColumnChooserState), new UIPropertyMetadata(new Size(220, 250)));
		public static readonly DependencyProperty MinWidthProperty =
			DependencyPropertyManager.Register("MinWidth", typeof(double), typeof(DefaultColumnChooserState), new UIPropertyMetadata(200d));
		public static readonly DependencyProperty MaxWidthProperty =
			DependencyPropertyManager.Register("MaxWidth", typeof(double), typeof(DefaultColumnChooserState), new UIPropertyMetadata(double.MaxValue));
		public static readonly DependencyProperty MinHeightProperty =
			DependencyPropertyManager.Register("MinHeight", typeof(double), typeof(DefaultColumnChooserState), new UIPropertyMetadata(200d));
		public static readonly DependencyProperty MaxHeightProperty =
			DependencyPropertyManager.Register("MaxHeight", typeof(double), typeof(DefaultColumnChooserState), new UIPropertyMetadata(double.MaxValue));
		[ThreadStatic]
		static DefaultColumnChooserState defaultState;
		public static DefaultColumnChooserState DefaultState {
			get {
				if(defaultState == null)
					defaultState = new DefaultColumnChooserState();
				return defaultState;
			}
		}
		[XtraSerializableProperty]
		public Point Location {
			get { return (Point)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		[XtraSerializableProperty]
		public Size Size {
			get { return (Size)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}
		public double MinWidth {
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}
		public double MaxWidth {
			get { return (double)GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}
		public double MinHeight {
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}
		public double MaxHeight {
			get { return (double)GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}
	}
	public class NullColumnChooserException : Exception {
		public static void CheckColumnChooserNotNull(IColumnChooser columnChooser) {
			if(columnChooser == null)
				throw new NullColumnChooserException();
		}
	}
}
