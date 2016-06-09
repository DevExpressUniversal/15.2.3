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

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Popups;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System;
using System.Windows.Controls;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using System.Windows.Media.Animation;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
using System.Windows.Media.Animation;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class DataAreaPopupEdit : PopupBaseEdit, IDragOverHandler {
		bool lockShow;
		DispatcherTimer leaveTimer, openTimer;
		#region static stuff
		public bool IsSelfMouseOver {
			get { return (bool)GetValue(IsSelfMouseOverProperty); }
			set { SetValue(IsSelfMouseOverProperty, value); }
		}
		public static readonly DependencyProperty IsSelfMouseOverProperty =
			DependencyPropertyManager.Register("IsSelfMouseOver", typeof(bool), typeof(DataAreaPopupEdit), new PropertyMetadata(false, (d, e) => ((DataAreaPopupEdit)d).IsSelfMouseOverChanged()));
		public bool IsPopupMouseOver {
			get { return (bool)GetValue(IsPopupMouseOverProperty); }
			set { SetValue(IsPopupMouseOverProperty, value); }
		}
		public static readonly DependencyProperty IsPopupMouseOverProperty =
			DependencyPropertyManager.Register("IsPopupMouseOver", typeof(bool), typeof(DataAreaPopupEdit), new PropertyMetadata(false, (d, e) => ((DataAreaPopupEdit)d).IsPopupMouseOverChanged()));
		public bool IsPopupOpenCore {
			get { return (bool)GetValue(IsPopupOpenCoreProperty); }
			set { SetValue(IsPopupOpenCoreProperty, value); }
		}
		public static readonly DependencyProperty IsPopupOpenCoreProperty =
			DependencyPropertyManager.Register("IsPopupOpenCore", typeof(bool), typeof(DataAreaPopupEdit), new PropertyMetadata(false, (d, e) => ((DataAreaPopupEdit)d).IsPopupOpenCoreChanged()));
		#endregion
		protected internal PivotGridControl PivotGrid {
			get {
				FieldHeadersPanel panel = GetHeadersPanel();
				if(panel != null)
					return PivotGridControl.GetPivotGrid(panel);
				else
					return null;
			}
		}
		void EnsureIsOpen() {
			FieldHeadersPanel panel = GetHeadersPanel();
			PivotGridControl pivot = null;
			if(panel != null)
				pivot = PivotGridControl.GetPivotGrid(panel);
			Window window = null;
			DependencyObject topVisual = null;
#if !SL
			if(Popup != null && Popup.IsOpen && Popup.Child != null)
				topVisual = LayoutHelper.GetTopLevelVisual(Popup.Child);
			window = GetWindow();
#else
			topVisual = Popup != null && Popup.IsOpen ? Application.Current.RootVisual : null;
#endif
			IsPopupOpenCore = (window == null || window.IsActive || pivot.IsDragging) &&
								 (pivot != null && IsPopupOpenCore && pivot.GridMenu.IsOpen ||
										!pivot.GridMenu.IsOpen && (IsSelfMouseOver ||
										IsPopupMouseOver) && panel != null && panel.IsVisible() &&
										NeedShowPopup(panel)
							   || topVisual != null && DragManager.GetIsDragging(topVisual)
								 );
		}
		internal bool NeedShowPopup(FieldHeadersPanel panel) {
			return panel.IsCutted;
		}
		public DataAreaPopupEdit() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
#if !SL
			this.leaveTimer = new DispatcherTimer(DispatcherPriority.ContextIdle);
			this.openTimer = new DispatcherTimer(DispatcherPriority.ContextIdle); 
#else
			this.leaveTimer = new DispatcherTimer(); 
			this.openTimer = new DispatcherTimer(); 
#endif
			this.leaveTimer.Interval = TimeSpan.FromMilliseconds(100);
			this.openTimer.Interval = TimeSpan.FromMilliseconds(300);
			openTimer.Tick += (d, e) => { openTimer.Stop(); IsPopupOpen = IsPopupOpenCore; };
#if !SL
			PopupHeight = double.NaN;
			PopupMinHeight = 30;
#endif
		}
		protected override void OnPopupOpened() {
			BeginShowAnimation();
			leaveTimer.Tick += OnLeaveTimerTick;  
#if !SL 
			((UIElement)Popup.PopupContent).MouseEnter += Child_MouseEnter;
			Window window = GetWindow();
			if(window != null)
				window.Deactivated += window_Deactivated;
			Popup.Placement = PlacementMode.Relative;
#else    
			Application.Current.RootVisual.MouseMove += RootVisual_MouseMove;  
			Popup.Child.MouseMove += RootVisual_MouseMove;
			Popup.Child.MouseLeave += RootVisual_MouseMove;
			Popup.Placement2 = PlacementMode2.Relative;
#endif
			Popup.VerticalOffset = -2 + (GetHeaders().RenderSize.Height - GetHeadersPanel().RenderSize.Height - 10) / 2;
			Popup.HorizontalOffset = -2 + GetMenuDropAlignment();
			Popup.Width = double.NaN;
			Popup.Height = double.NaN;
			Popup.MinHeight = 30;
			Popup.MinWidth = RenderSize.Width + 6;
#if !SL
			Popup.MaxWidth = SystemParameters.PrimaryScreenWidth - 100;
			if(Popup.Child as FrameworkElement != null)
				((FrameworkElement)Popup.Child).Margin = new Thickness(10);
#else
			Popup.MaxWidth = Application.Current.IsRunningOutOfBrowser ? Application.Current.MainWindow.Width : Application.Current.Host.Content.ActualWidth - 100;
			base.OnPopupOpened();
#endif
			leaveTimer.Start();
			if(PivotGrid.IsDragging && PivotGrid.CurrenDragDropElementHelper != null) {
				PivotGrid.CurrenDragDropElementHelper.RecreatePopup();
			}
#if !SL
			CommandManager.AddCanExecuteHandler(Popup, OnCommandCanExecute);
			CommandManager.AddExecutedHandler(Popup, OnCommandExecuted);
#endif
		}
		double GetMenuDropAlignment() {
			if(SystemParameters.MenuDropAlignment)
				if(FlowDirection == FlowDirection.RightToLeft) 
					return -RenderSize.Width;
				else
					return RenderSize.Width;
			return 0;
		}
#if !SL
		void OnCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			RoutedCommand command = e.Command as RoutedCommand;
			if(command != null)
				e.CanExecute = command.CanExecute(e.Parameter, PivotGrid);
		}
		void OnCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
			RoutedCommand command = e.Command as RoutedCommand;
			if(command != null && command.CanExecute(e.Parameter, PivotGrid))
				command.Execute(e.Parameter, PivotGrid);
		}
#endif
		private void BeginShowAnimation() {
#if DEBUGTEST
			if(ForcePopupOpen)
				return;
#endif
			Popup.Child.Opacity = 0;
			DoubleAnimation opacityAnimation = new DoubleAnimation();
			opacityAnimation.From = 0;
			opacityAnimation.To = 1;
			opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
			Storyboard.SetTarget(opacityAnimation, Popup.Child);
			Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(opacityAnimation);
			storyboard.Begin();
		}
		protected override void OnPopupClosed() {
			base.OnPopupClosed();
			if(Popup == null || Popup.Child == null)
				return;  
			leaveTimer.Stop();
			leaveTimer.Tick -= OnLeaveTimerTick;
#if !SL 
			((UIElement)Popup.PopupContent).MouseEnter -= Child_MouseEnter;
			Window window = GetWindow();
			if(window != null)
				window.Deactivated -= window_Deactivated;
#else
			Application.Current.RootVisual.MouseMove -= RootVisual_MouseMove;
			Popup.Child.MouseMove -= RootVisual_MouseMove;
			Popup.Child.MouseLeave -= RootVisual_MouseMove;
#endif
			IsPopupOpenCore = false;
#if !SL
			CommandManager.RemoveCanExecuteHandler(Popup, OnCommandCanExecute);
			CommandManager.RemoveExecutedHandler(Popup, OnCommandExecuted);
#endif
		}
#if !SL
		Window GetWindow() {
			FrameworkElement topElement = (FrameworkElement)LayoutHelper.GetTopLevelVisual(this);
			return Window.GetWindow(topElement);
		}
		void window_Deactivated(object sender, EventArgs e) {
			HidePopup();
		}
#else
		void RootVisual_MouseMove(object sender, MouseEventArgs e) {
			OnMouseOverOrLeave(e);
		}
#endif
		protected override void ClosePopupOnClick() {
			PivotGridControl pivot = PivotGrid;
			if(pivot != null && pivot.GridMenu.IsOpen)
				return;
			base.ClosePopupOnClick();
		}
		void IsSelfMouseOverChanged() {
			if(lockShow)
				return;
			if(Popup != null && Popup.Child != null && IsPopupOpenCore) {
#if !SL
				IsPopupMouseOver = Popup.Child.IsMouseOver;
#endif
			}
			EnsureIsOpen();
		}
		void IsPopupMouseOverChanged() {
			if(lockShow)
				return;
			EnsureIsOpen();
		}
#if DEBUGTEST
		public static bool ForcePopupOpen { get; set; }
#endif
		void IsPopupOpenCoreChanged() {
#if DEBUGTEST
			if(ForcePopupOpen) {
				IsPopupOpen = IsPopupOpenCore;
			} else {
#endif
				if(openTimer.IsEnabled)
					openTimer.Stop();
				if(IsPopupOpenCore)
					openTimer.Start();
				else
					IsPopupOpen = false;
#if DEBUGTEST
			}
#endif
		}
		void OnMouseOverOrLeave(MouseEventArgs e 
#if !SL
			= null
#endif     
			) {
		   Point mousePoint = 
#if !SL
		   Mouse
#else
			e
#endif
			.GetPosition(this);
			double popupWidth = 0;
			double popupHeigth = 0;
			if(Popup != null && IsPopupOpenCore && Popup.PopupContent != null) {
				Size size = ((UIElement)Popup.PopupContent).RenderSize;
				popupWidth = size.Width;
				popupHeigth = size.Height;
			}
			bool isSelfOver = !(mousePoint.Y > RenderSize.Height || mousePoint.Y < 0 || mousePoint.X < 0 || mousePoint.X > RenderSize.Width);
			bool isPopupOver = false;
			if(Popup != null && Popup.IsOpen) {
							mousePoint =
#if !SL
			Mouse
#else
			e
#endif
			.GetPosition(Popup.PopupContent as
#if SL
			UIElement
#else
			IInputElement
#endif   
			);
				 isPopupOver = !(mousePoint.Y > popupHeigth || mousePoint.Y < 0 || mousePoint.X < 0 || mousePoint.X > popupWidth);
			}
			if(!isPopupOver && !isSelfOver)
				HidePopup();
			else {
				IsSelfMouseOver = true;
			}
		}
		void HidePopup() {
			if(!IsSelfMouseOver && !IsPopupMouseOver) {
				EnsureIsOpen();
				return;
			}
			lockShow = true;
			IsSelfMouseOver = false;
			lockShow = false;
			IsPopupMouseOver = false;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			MouseEnter -= DataAreaPopupEdit_MouseEnter;
			MouseLeave -= DataAreaPopupEdit_MouseLeave;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			MouseEnter += DataAreaPopupEdit_MouseEnter;
			MouseLeave += DataAreaPopupEdit_MouseLeave;
		}
		protected virtual void OnDragTargetMouseOver(object sender, PivotDropTargetMouseOverEventArgs e) {
			IsSelfMouseOver = true;
		}
		void Child_MouseEnter(object sender, MouseEventArgs e) {
			IsPopupMouseOver = true;
		}
		void DataAreaPopupEdit_MouseLeave(object sender, MouseEventArgs e) {
#if SL
			IsPopupMouseOver = true;
#endif
			IsSelfMouseOver = false;
		}
		void DataAreaPopupEdit_MouseEnter(object sender, MouseEventArgs e) {
			OnMouseOverOrLeave(e);
		}
		void OnLeaveTimerTick(object sender, EventArgs e) {
#if !SL
			OnMouseOverOrLeave();
#else
			if(!IsSelfMouseOver && !IsPopupMouseOver)
				EnsureIsOpen();
#endif
		}
		public void ForceTimer() {
			OnLeaveTimerTick(null, null);
		}
		internal new FrameworkElement GetPopupContent() {
			return Popup != null ? Popup.PopupContent as FrameworkElement : null;
		}
		internal FieldHeadersPanel GetHeadersPanel() {
			return (FieldHeadersPanel)LayoutHelper.FindElement(this, (d) => d is FieldHeadersPanel);
		}
		internal FieldHeaders GetHeaders() {
			return LayoutHelper.FindElement(this, (d) => d is FieldHeaders) as FieldHeaders;
		}
		void IDragOverHandler.DragOver(bool over) {
			if(!over)
				return;
			FieldHeadersPanel panel = GetHeadersPanel();
			if(!IsPopupOpenCore && panel == null || !NeedShowPopup(panel)) {
				FieldHeaders headers = GetHeaders();
				if(headers != null)
					((IDragOverHandler)headers).DragOver(over);
			} else {
				IsSelfMouseOver = true;
			}
		}
	}
}
