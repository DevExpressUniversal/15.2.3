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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Editors {
	public class DateTimePickerItem : ContentControl {
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty IsFakeProperty;
		public static readonly DependencyProperty IsActiveProperty;
		public static readonly DependencyProperty UseTransitionsProperty;
		static DateTimePickerItem() {
			Type ownerType = typeof(DateTimePickerItem);
			IsFakeProperty = DependencyPropertyManager.Register("IsFake", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			IsActiveProperty = DependencyPropertyManager.Register("IsActive", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			UseTransitionsProperty = DependencyPropertyManager.Register("UseTransitions", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public bool UseTransitions {
			get { return (bool)GetValue(UseTransitionsProperty); }
			set { SetValue(UseTransitionsProperty, value); }
		}
		public bool IsFake {
			get { return (bool)GetValue(IsFakeProperty); }
			set { SetValue(IsFakeProperty, value); }
		}
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public DateTimePickerItem() {
			DefaultStyleKey = typeof(DateTimePickerItem);
			Loaded += OnLoaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateVisualStates();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if (IsFake)
				return;
			LoopedPanel panel = Parent as LoopedPanel;
			DateTimePickerSelector selector = panel.ItemsContainerGenerator as DateTimePickerSelector;
			if (selector.IsExpanded) {
				double itemOffset = e.GetPosition(this).Y;
				if (panel.ScrollOwner == null)
					return;
				IndexCalculator indexCalculator = panel.IndexCalculator;
				Point position = e.GetPosition(panel.ScrollOwner);
				double viewport = panel.Orientation == Orientation.Vertical ? panel.ScrollOwner.ActualHeight : panel.ScrollOwner.ActualWidth;
				double offset = indexCalculator.CalcIndexOffset(panel.Offset, panel.Viewport, viewport, itemOffset, position.Y);
				DXScrollViewer scrollViewer = ((DXScrollViewer)panel.ScrollOwner);
				scrollViewer.AnimateScrollToVerticalOffset(offset, () => {
					selector.SelectedItem = null;
					selector.IsAnimated = true;
				}, null, () => {
					scrollViewer.IsIntermediate = false;
				}, panel.IsLooped ? (Func<double, double>)scrollViewer.EnsureVerticalOffset : null);
			}
			base.OnMouseLeftButtonUp(e);
		}
		public virtual void UpdateVisualStates() {
			if (!IsLoaded)
				return;
			if (IsActive || IsExpanded)
				VisualStateManager.GoToState(this, "Normal", true);
			else
				VisualStateManager.GoToState(this, "Hidden", true);
		}
	}
}
