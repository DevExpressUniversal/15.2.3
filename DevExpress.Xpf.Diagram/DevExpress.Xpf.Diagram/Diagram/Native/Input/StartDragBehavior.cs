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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	public class ToolBoxItem : BarCheckItem {
		public Style LinkControlStyle {
			get { return (Style)GetValue(LinkControlStyleProperty); }
			set { SetValue(LinkControlStyleProperty, value); }
		}
		public static readonly DependencyProperty LinkControlStyleProperty =
			DependencyProperty.Register("LinkControlStyle", typeof(Style), typeof(ToolBoxItem), new PropertyMetadata(null));
		static ToolBoxItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(ToolBoxItem), typeof(ToolBoxItemLink), i => new ToolBoxItemLink());
			BarItemLinkControlCreator.Default.RegisterObject(typeof(ToolBoxItemLink), typeof(ToolBoxItemLinkControl), i => new ToolBoxItemLinkControl());
		}
	}
	public class ToolBoxItemLink : BarCheckItemLink {
	}
	public class ToolBoxItemLinkControl : BarCheckItemLinkControl {
		protected override void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
			base.OnLinkInfoChanged(oldValue);
			OnItemChanged(oldValue.With(x => x.Item), LinkInfo.With(x => x.Item));
		}
		void OnItemChanged(BarItem oldItem, BarItem newItem) {
			if(newItem != null) {
				SetBinding(StyleProperty, new Binding() { Source = newItem, Path = new PropertyPath(ToolBoxItem.LinkControlStyleProperty) });
			} else {
				ClearValue(StyleProperty);
			}
		}
	}
	public class ToolboxSplitItem : BarSplitCheckItem {
		public Style LinkControlStyle {
			get { return (Style)GetValue(LinkControlStyleProperty); }
			set { SetValue(LinkControlStyleProperty, value); }
		}
		public static readonly DependencyProperty LinkControlStyleProperty =
			DependencyProperty.Register("LinkControlStyle", typeof(Style), typeof(ToolboxSplitItem), new PropertyMetadata(null));
		static ToolboxSplitItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(ToolboxSplitItem), typeof(ToolboxSplitItemLink), i => new ToolboxSplitItemLink());
			BarItemLinkControlCreator.Default.RegisterObject(typeof(ToolboxSplitItemLink), typeof(ToolboxSplitItemLinkControl), i => new ToolboxSplitItemLinkControl());
		}
	}
	public class ToolboxSplitItemLink : BarSplitCheckItemLink {
	}
	public class ToolboxSplitItemLinkControl : BarSplitCheckItemLinkControl {
		protected override void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
			base.OnLinkInfoChanged(oldValue);
			OnItemChanged(oldValue.With(x => x.Item), LinkInfo.With(x => x.Item));
		}
		void OnItemChanged(BarItem oldItem, BarItem newItem) {
			if (newItem != null) {
				SetBinding(StyleProperty, new Binding() { Source = newItem, Path = new PropertyPath(ToolboxSplitItem.LinkControlStyleProperty) });
			} else {
				ClearValue(StyleProperty);
			}
		}
	}
	public class StartDragBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandAlternateProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty MinDragDistanceProperty;
		static StartDragBehavior() {
			DependencyPropertyRegistrator<StartDragBehavior>.New()
				.Register(x => x.Command, out CommandProperty, null)
				.Register(x => x.CommandAlternate, out CommandAlternateProperty, null)
				.Register(x => x.CommandParameter, out CommandParameterProperty, null)
				.Register(x => x.MinDragDistance, out MinDragDistanceProperty, 0)
				;
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public ICommand CommandAlternate {
			get { return (ICommand)GetValue(CommandAlternateProperty); }
			set { SetValue(CommandAlternateProperty, value); }
		}
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public double MinDragDistance {
			get { return (double)GetValue(MinDragDistanceProperty); }
			set { SetValue(MinDragDistanceProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
			AssociatedObject.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);
			AssociatedObject.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);
		}
		protected override void OnDetaching() {
			AssociatedObject.RemoveHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown));
			AssociatedObject.RemoveHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp));
			AssociatedObject.RemoveHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
			base.OnDetaching();
		}
		Point? startPosition;
		MouseButton startButton;
		void OnMouseMove(object sender, MouseEventArgs e) {
			if(startPosition != null) {
				var currentPosition = e.GetPosition(AssociatedObject);
				if(MathHelper.IsDragGesture(currentPosition, startPosition.Value, MinDragDistance)) {
					startPosition = null;
					GetCommand().Do(x => x.Execute(CommandParameter));
				}
			}
		}
		ICommand GetCommand() {
			if(startButton == MouseButton.Left)
				return Command;
			if(startButton == MouseButton.Right)
				return CommandAlternate;
			return null;
		}
		void OnMouseUp(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == startButton)
				startPosition = null;
		}
		void OnMouseDown(object sender, MouseButtonEventArgs e) {
			if((e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right) && startPosition == null) {
				startPosition = e.GetPosition(AssociatedObject);
				startButton = e.ChangedButton;
			}
		}
	}
}
