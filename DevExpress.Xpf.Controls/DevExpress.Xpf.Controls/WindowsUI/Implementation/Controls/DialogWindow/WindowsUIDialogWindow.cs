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

using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using DevExpress.Mvvm.UI.Native;
using System;
using DevExpress.Mvvm.Native;
using System.Windows.Threading;
namespace DevExpress.Xpf.WindowsUI {
	[TemplatePart(Name = TitleElementName, Type = typeof(ContentControl))]
	[TemplatePart(Name = ContentElementName, Type = typeof(ContentControl))]
	public class WinUIDialogWindow : DXDialogWindow, IWindowSurrogate {
		internal const string TitleElementName = "PART_Title";
		internal const string ContentElementName = "PART_Content";
		public static readonly DependencyProperty TitleStyleProperty =
			DependencyProperty.Register("TitleStyle", typeof(Style), typeof(WinUIDialogWindow), new PropertyMetadata(null));
		public Style TitleStyle {
			get { return (Style)GetValue(TitleStyleProperty); }
			set { SetValue(TitleStyleProperty, value); }
		}
		static WinUIDialogWindow() {
			WindowStyleProperty.OverrideMetadata(typeof(WinUIDialogWindow), new FrameworkPropertyMetadata(WindowStyle.None));
			AllowsTransparencyProperty.OverrideMetadata(typeof(WinUIDialogWindow), new FrameworkPropertyMetadata(true));
			SizeToContentProperty.OverrideMetadata(typeof(WinUIDialogWindow), new FrameworkPropertyMetadata(SizeToContent.Manual));
			ResizeModeProperty.OverrideMetadata(typeof(WinUIDialogWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
			ShowInTaskbarProperty.OverrideMetadata(typeof(WinUIDialogWindow), new FrameworkPropertyMetadata(false));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WinUIDialogWindow), new FrameworkPropertyMetadata(typeof(WinUIDialogWindow)));
		}
		public WinUIDialogWindow() : base() {
			Activated += (d, e) => activatedEvent.Do(x => x(d, e));
			Deactivated += (d, e) => deactivatedEvent.Do(x => x(d, e));
			Closing += (d, e) => closingEvent.Do(x => x(d, e));
			Closed += (d, e) => closedEvent.Do(x => x(d, e));
		}
		public WinUIDialogWindow(string title) :
			this() {
			Title = title;
		}
		public WinUIDialogWindow(string title, MessageBoxButton dialogButtons, MessageBoxResult? defaultButton = null, MessageBoxResult? cancelButton = null) :
			this(title, GenerateUICommands(dialogButtons, defaultButton, cancelButton)) { }
		public WinUIDialogWindow(string title, IEnumerable<UICommand> commands) :
			this() {
			Title = title;
			CommandsSource = commands;
		}
		AdornerContainer adornerContainer = null;
		public new void Show() {
			BeginInit();
			BeforeShow();
			EndInit();
			FrameworkElement rootElement = (FrameworkElement)Template.LoadContent();
			WindowStorage windowStorage = (WindowStorage)rootElement.Resources["windowStorage"];
			windowStorage.Window = this;
			AdornerDecorator adornerDecorator = LayoutTreeHelper.GetVisualChildren(Owner).OfType<AdornerDecorator>().First();
			adornerContainer = new AdornerContainer(adornerDecorator, rootElement);
			adornerContainer.Tag = new object[] { adornerDecorator.Child, this };
			AdornerLayer.GetAdornerLayer(adornerDecorator.Child).Add(adornerContainer);
			rootElement.Dispatcher.BeginInvoke(
				new Action(() => 
					LayoutTreeHelper.GetVisualChildren(rootElement)
					.OfType<Control>()
					.FirstOrDefault(x => x.Focusable && x.IsEnabled)
					.Do(x => x.Focus())), 
				DispatcherPriority.Loaded);
			activatedEvent.Do(x => x(this, EventArgs.Empty));
		}
		public new void Close() {
			if(!IsDialogMode && adornerContainer != null) {
				CancelEventArgs args = new CancelEventArgs();
				closingEvent.Do(x => x(this, args));
				if(args.Cancel) return;
				deactivatedEvent.Do(x => x(this, EventArgs.Empty));
				closedEvent.Do(x => x(this, EventArgs.Empty));
				var adornerParams = (object[])adornerContainer.Tag;
				var adornerLayer = AdornerLayer.GetAdornerLayer((FrameworkElement)adornerParams[0]);
				adornerLayer.Remove(adornerContainer);
				adornerContainer.Tag = null;
				adornerContainer = null;
				OnClose();
				var adorner = LayoutTreeHelper.GetVisualChildren(adornerLayer).OfType<AdornerContainer>().LastOrDefault();
				if(adorner != null && adorner.Tag != null) {
					WinUIDialogWindow w = ((object[])adorner.Tag)[1] as WinUIDialogWindow;
					w.Activate();
				}
				return;
			}
			base.Close();
		}
		public new bool Activate() {
			if(IsDialogMode) return base.Activate();
			activatedEvent.Do(x => x(this, EventArgs.Empty));
			return true;
		}
		protected override void BeforeShow() {
			base.BeforeShow();
			Init();
		}
		protected override void CloseCore(UICommand command) {
			if(!IsDialogMode) 
				Close();
			else 
				base.CloseCore(command);
		}
		void Init() {
			if(Owner == null) {
				var owner = GetOwner();
				Owner = owner != this ? owner : null;
			}
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
			SizeToContent = SizeToContent.Manual;
			WindowStartupLocation = WindowStartupLocation.Manual;
			Rect screenRect = LayoutHelper.GetScreenRect(Owner);
			Left = screenRect.Left;
			Top = screenRect.Top;
			Width = screenRect.Width;
			Height = screenRect.Height;
		}
		static Window GetOwner() {
			Window owner = null;
			if(Application.Current != null && Application.Current.Dispatcher.CheckAccess()) {
				foreach(Window w in Application.Current.Windows) {
					if(w.IsActive) {
						owner = w;
						break;
					}
				}
				if(owner == null && Application.Current.Windows.Count > 0) {
					owner = Application.Current.Windows[0];
				}
			}
			return owner;
		}
		Window IWindowSurrogate.RealWindow { get { return this; } }
		void IWindowSurrogate.Show() {
			Show();
		}
		void IWindowSurrogate.Close() {
			Close();
		}
		bool IWindowSurrogate.Activate() {
			return Activate();
		}
		event EventHandler activatedEvent;
		event EventHandler deactivatedEvent;
		event CancelEventHandler closingEvent;
		event EventHandler closedEvent;
		event EventHandler IWindowSurrogate.Activated {
			add { activatedEvent += value; }
			remove { activatedEvent -= value; }
		}
		event EventHandler IWindowSurrogate.Deactivated {
			add { deactivatedEvent += value; }
			remove { deactivatedEvent -= value; }
		}
		event CancelEventHandler IWindowSurrogate.Closing {
			add { closingEvent += value; }
			remove { closingEvent -= value; }
		}
		event EventHandler IWindowSurrogate.Closed {
			add { closedEvent += value; }
			remove { closedEvent -= value; }
		}
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class WindowStorage : Behavior<FrameworkElement> {
		public static readonly DependencyProperty WindowProperty =
			DependencyProperty.Register("Window", typeof(WinUIDialogWindow), typeof(WindowStorage), new PropertyMetadata(null));
		public WinUIDialogWindow Window {
			get { return (WinUIDialogWindow)GetValue(WindowProperty); }
			set { SetValue(WindowProperty, value); }
		}
	}
}
