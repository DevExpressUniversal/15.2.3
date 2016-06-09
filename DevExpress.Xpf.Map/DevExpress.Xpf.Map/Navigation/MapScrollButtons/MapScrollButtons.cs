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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class MapScrollButtons : Control {
		public static readonly DependencyProperty CommandProperty = DependencyPropertyManager.Register("Command",
			typeof(ICommand), typeof(MapScrollButtons), new PropertyMetadata(null));
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		readonly MapScrollButtonsNavigationController navigationController;
		bool isMouseOver;
		bool isFocused;
		public MapScrollButtons() {
			DefaultStyleKey = typeof(MapScrollButtons);
			navigationController = new MapScrollButtonsNavigationController(this);
			MouseLeftButtonUp += new MouseButtonEventHandler(navigationController.MouseLeftButtonUp);
			MouseLeftButtonDown += new MouseButtonEventHandler(navigationController.MouseLeftButtonDown);
			MouseMove += new MouseEventHandler(navigationController.MouseMove);
			MouseEnter += new MouseEventHandler(MapScrollButtons_MouseEnter);
			MouseLeave += new MouseEventHandler(MapScrollButtons_MouseLeave);
			GotFocus += new RoutedEventHandler(MapScrollButtons_GotFocus);
			LostFocus += new RoutedEventHandler(MapScrollButtons_LostFocus);
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(MapScrollButtons_IsEnabledChanged);
		}
		void MapScrollButtons_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateVisualState();
		}
		void MapScrollButtons_MouseEnter(object sender, MouseEventArgs e) {
			isMouseOver = true;
			UpdateVisualState();
		}
		void MapScrollButtons_MouseLeave(object sender, MouseEventArgs e) {
			isMouseOver = false;
			UpdateVisualState();
		}
		void MapScrollButtons_GotFocus(object sender, RoutedEventArgs e) {
			isFocused = true;
			UpdateVisualState();
		}
		void MapScrollButtons_LostFocus(object sender, RoutedEventArgs e) {
			isFocused = false;
			UpdateVisualState();
		}
		void UpdateVisualState(bool useTransitions) {
			if (!IsEnabled)
				VisualStateManager.GoToState(this, "Disabled", useTransitions);
			else if (navigationController.DraggingInProcess)
				VisualStateManager.GoToState(this, "Pressed", useTransitions);
			else if (isMouseOver)
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			else {
				VisualStateManager.GoToState(this, "Normal", useTransitions);
				VisualStateManager.GoToState(this, "Unfocused", useTransitions);
			}
			if (isFocused)
				VisualStateManager.GoToState(this, "Focused", useTransitions);
			else
				VisualStateManager.GoToState(this, "Unfocused", useTransitions);
		}
		internal void UpdateVisualState() {
			UpdateVisualState(true);
		}
		internal void ExecuteCommand(Point offset) {
			ICommand command = Command;
			if (command != null) {
				object commandParameter = offset;
				if (command.CanExecute(commandParameter))
					command.Execute(commandParameter);
			}
		}
		internal bool CanExecuteCommand(Point offset) {
			return Command.CanExecute(offset);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
		}
	}
	public class MapScrollButtonsLayoutControl : Control {
		public MapScrollButtonsLayoutControl() {
			DefaultStyleKey = typeof(MapScrollButtonsLayoutControl);
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class ScrollButtonsInfo : OverlayInfoBase {
		ICommand command;
		public ICommand Command {
			get { return command; }
			set {
				if (command != value) {
					command = value;
					RaisePropertyChanged("Command");
				}
			}
		}
		public ScrollButtonsInfo(MapControl map) : base(map) {
		}
		protected internal override Control CreatePresentationControl() {
			return new MapScrollButtonsLayoutControl();
		}
	}
}
