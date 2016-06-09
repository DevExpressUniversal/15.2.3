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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Documents;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Mvvm.Native;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon.Internal {
	public abstract class ApplicationMenuStrategyBase : IDisposable {
		public static ApplicationMenuStrategyBase CreateStrategy(RibbonControl ribbon, DependencyObject appMenu) {
			if (appMenu is ApplicationMenu)
				return new ApplicationMenuStrategy(ribbon);
			if (appMenu is BackstageViewControl)
				return new BackstageViewMenuStrategy(ribbon);
			return new UndefinedMenuStrategy(ribbon);
		}
		public RibbonControl Ribbon { get; private set; }
		public bool IsMenuOpened {
			get { return isMenuOpened; }
			protected set {
				if (isMenuOpened == value)
					return;
				isMenuOpened = value;
				OnIsMenuOpenedChanged();
			}
		}
		public abstract void Open();
		public abstract void Close();
		public ApplicationMenuStrategyBase(RibbonControl ribbon) {
			if (ribbon == null)
				throw new ArgumentNullException();
			Ribbon = ribbon;
		}
		public void Dispose() {
			OnDispose();
		}
		protected virtual void OnIsMenuOpenedChanged() {
			UpdateApplicationButtonCheckedState();
		}
		protected virtual void OnDispose() {
			Ribbon = null;
		}
		protected virtual void UpdateApplicationButtonCheckedState() {
			if (Ribbon.ApplicationButton == null)
				return;
			Ribbon.ApplicationButton.SetCurrentValue(RibbonApplicationButtonControl.IsCheckedProperty, IsMenuOpened);
		}
		bool isMenuOpened;
	}
	public class ApplicationMenuStrategy : ApplicationMenuStrategyBase {
		public ApplicationMenu ApplicationMenu {
			get { return applicationMenu; }
			private set {
				if (applicationMenu == value)
					return;
				var oldValue = applicationMenu;
				applicationMenu = value;
				OnApplicationMenuChanged(oldValue);
			}
		}
		public ApplicationMenuStrategy(RibbonControl ribbon)
			: base(ribbon) {
			ApplicationMenu = (ApplicationMenu)Ribbon.ApplicationMenu;
		}
		public override void Open() {
			ApplicationMenu.Show(Ribbon.ApplicationButton);
		}
		public override void Close() {
			ApplicationMenu.ClosePopup();
		}
		protected override void OnDispose() {
			base.OnDispose();
			ApplicationMenu = null;
		}
		protected virtual void OnApplicationMenuOpened(object sender, EventArgs e) {
			IsMenuOpened = true;
		}
		protected virtual void OnApplicationMenuClosed(object sender, EventArgs e) {
			IsMenuOpened = false;
		}
		protected virtual void OnApplicationMenuChanged(ApplicationMenu oldValue) {
			if (oldValue != null) {
				oldValue.Opened -= OnApplicationMenuOpened;
				oldValue.Closed -= OnApplicationMenuClosed;
			}
			if (ApplicationMenu != null) {
				ApplicationMenu.Opened += OnApplicationMenuOpened;
				ApplicationMenu.Closed += OnApplicationMenuClosed;
			}
		}
		ApplicationMenu applicationMenu;
	}
	public class UndefinedMenuStrategy : ApplicationMenuStrategyBase {
		public ApplicationMenuContentControl BackstageViewContentControl {
			get { return backstageViewContentControl; }
			private set {
				if (backstageViewContentControl == value)
					return;
				var oldValue = backstageViewContentControl;
				backstageViewContentControl = value;
				OnBackstageViewContentControlChanged(oldValue);
			}
		}
		protected AdornerLayer BackstageViewContentControlAdornerLayer { get; private set; }
		protected AdornerContainer BackstageViewContentControlAdornerContainer { get; private set; }
		public UndefinedMenuStrategy(RibbonControl ribbon)
			: base(ribbon) {
		}
		public override void Open() {
			if (IsMenuOpened || Ribbon.IsInDesignTool())
				return;
			try {
				var root = GetBackstageViewRoot();
				BackstageViewContentControl = new ApplicationMenuContentControl();
				BackstageViewContentControlAdornerContainer = new AdornerContainer(root, BackstageViewContentControl);
				BackstageViewContentControlAdornerLayer = AdornerLayer.GetAdornerLayer(Ribbon);
				BackstageViewContentControlAdornerLayer.Add(BackstageViewContentControlAdornerContainer);
				UpdateBackstageViewOffset();
				IsMenuOpened = true;
			} catch {
				IsMenuOpened = false;
			}
		}
		public override void Close() {
			if (!IsMenuOpened)
				return;
			BackstageViewContentControl.Close();
		}
		protected virtual void OnBackstageViewContentControlChanged(ApplicationMenuContentControl oldValue) {
			if (oldValue != null) {
				oldValue.Content = null;
				oldValue.Closed -= OnClosed;
			}
			if (BackstageViewContentControl != null) {
				BackstageViewContentControl.Content = Ribbon.ApplicationMenu;
				BackstageViewContentControl.Closed += OnClosed;
			}
		}
		protected virtual void OnClosed(object sender, EventArgs e) {
			if (BackstageViewContentControl == null)
				return;
			BackstageViewContentControlAdornerLayer.Remove(BackstageViewContentControlAdornerContainer);
			BackstageViewContentControlAdornerLayer = null;
			BackstageViewContentControlAdornerContainer = null;
			BackstageViewContentControl = null;
			IsMenuOpened = false;
		}
		protected override void OnIsMenuOpenedChanged() {
			base.OnIsMenuOpenedChanged();
			Ribbon.SetValue(RibbonControl.IsBackStageViewOpenPropertyKey, IsMenuOpened);
		}
		protected virtual UIElement GetBackstageViewRoot() {
			if (Ribbon.Manager != null)
				return Ribbon.Manager;
			return !Ribbon.IsInRibbonWindow ? VisualTreeHelper.GetParent(Ribbon) as UIElement : (UIElement)LayoutHelper.FindLayoutOrVisualParentObject(Ribbon, x => x is AdornerDecorator || x is ScrollContentPresenter);
		}
		protected virtual void UpdateBackstageViewOffset() {
			if (BackstageViewContentControl == null)
				return;
			if (Ribbon.WindowHelper.HasRibbonWindowAsParent && Ribbon.WindowHelper.IsAeroMode)
				BackstageViewContentControl.SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(0, Ribbon.HeaderBorder.RenderSize.Height, 0, 0));
		}
		ApplicationMenuContentControl backstageViewContentControl;
	}
	public class BackstageViewMenuStrategy : UndefinedMenuStrategy {
		public BackstageViewControl BackstageView {
			get { return backstageView; }
			private set {
				if (value == backstageView)
					return;
				var oldValue = backstageView;
				backstageView = value;
				OnBackstageViewChanged(oldValue);
			}
		}
		WeakReference FocusedElementOnBackstageClosed { get; set; }
		public BackstageViewMenuStrategy(RibbonControl ribbon)
			: base(ribbon) {
			BackstageView = (BackstageViewControl)Ribbon.ApplicationMenu;
		}
		protected override void OnIsMenuOpenedChanged() {
			base.OnIsMenuOpenedChanged();
			BackstageView.SetCurrentValue(BackstageViewControl.IsOpenProperty, IsMenuOpened);
		}
		protected override UIElement GetBackstageViewRoot() {
			if(Ribbon.IsInRibbonWindow) {
				if(BackstageView.IsFullScreen && BackstageView.EnableWindowTitleShrink)
					return (UIElement)LayoutHelper.FindLayoutOrVisualParentObject(Ribbon, x => x is AdornerDecorator || x is ScrollContentPresenter);
				else
					return Ribbon.WindowHelper.GetRibbonWindowContent();
			}
			var rootPath = LayoutHelper.GetRootPath(AdornerLayer.GetAdornerLayer(Ribbon).GetVisualParent(), Ribbon).OfType<UIElement>();
			return rootPath.Last(elem => elem.RenderSize.Width == Ribbon.RenderSize.Width);
		}
		protected virtual void OnBackstageViewChanged(BackstageViewControl oldValue) {
			if (oldValue != null) {
				oldValue.Ribbon = null;
				oldValue.Loaded -= OnBackstageViewLoaded;
				((ILogicalChildrenContainer)Ribbon).RemoveLogicalChild(oldValue);
			}
			if (BackstageView != null) {
				((ILogicalChildrenContainer)Ribbon).AddLogicalChild(BackstageView);
				BackstageView.Loaded += OnBackstageViewLoaded;
				BackstageView.Ribbon = Ribbon;
				if (!Ribbon.IsBackStageViewOpen && BackstageView.IsOpen && Ribbon.IsInVisualTree() && Ribbon.IsLoaded)
					Open();
			}
		}
		protected virtual void OnBackstageViewLoaded(object sender, RoutedEventArgs e) {
			FocusedElementOnBackstageClosed = new WeakReference(Keyboard.FocusedElement);
			UpdateBackstageViewOffset();
			((BackstageViewControl)sender).UpdateFocus();
		}
		protected override void UpdateBackstageViewOffset() {
			if (BackstageViewContentControl == null)
				return;
			var root = GetBackstageViewRoot();
			if (Ribbon.WindowHelper.HasRibbonWindowAsParent && (Ribbon.WindowHelper.IsAeroMode || !BackstageView.EnableWindowTitleShrink) && BackstageView.IsFullScreen) {
				BackstageViewContentControl.SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(0, Ribbon.HeaderBorder.RenderSize.Height, 0, 0));
			} else if (!BackstageView.IsFullScreen && root != null && root.IsAncestorOf(Ribbon.SelectedPageControlContainer)) {
				Point offset = Ribbon.SelectedPageControlContainer.TransformToAncestor(root).Transform(new Point());
				BackstageViewContentControl.SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(0, offset.Y, 0, 0));
			} else
				BackstageViewContentControl.ClearValue(FrameworkElement.MarginProperty);
		}
		protected override void OnClosed(object sender, EventArgs e) {
			base.OnClosed(sender, e);
			FocusedElementOnBackstageClosed.With(x => x.Target as IInputElement).Do(x => x.Focus());
			FocusedElementOnBackstageClosed = null;
			BackstageView.With(x => x.Host).Do(x => x.ShowRibbonElements());
		}
		protected override void OnDispose() {
			BackstageView = null;
			base.OnDispose();
		}
		BackstageViewControl backstageView;
	}
}
