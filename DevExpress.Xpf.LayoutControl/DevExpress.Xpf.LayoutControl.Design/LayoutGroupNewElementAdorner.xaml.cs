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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.LayoutControl.Design {
	enum LayoutGroupNewElementsVisibility { All, GroupOnly }
	partial class LayoutGroupNewElementAdorner : UserControl {
		public static readonly DependencyProperty ShowGroupAsTabProperty =
			DependencyProperty.Register("ShowGroupAsTab", typeof(bool), typeof(LayoutGroupNewElementAdorner),
				new PropertyMetadata((o, e) => ((LayoutGroupNewElementAdorner)o).OnShowGroupAsTabChanged()));
		public LayoutGroupNewElementAdorner() {
			InitializeComponent();
		}
		public LayoutGroupNewElementAdorner(Orientation orientation, PlacementMode toolTipPlacement) : this() {
			Orientation = orientation;
			ToolTipPlacement = toolTipPlacement;
			UpdateButtons(false, false);
		}
		public LayoutGroupNewElementsVisibility NewElementsVisibility { get; set; }
		public Orientation Orientation {
			get { return LayoutRoot.Orientation; }
			set { LayoutRoot.Orientation = value; }
		}
		public bool ShowGroupAsTab {
			get { return (bool)GetValue(ShowGroupAsTabProperty); }
			set { SetValue(ShowGroupAsTabProperty, value); }
		}
		public PlacementMode ToolTipPlacement {
			set {
				foreach (UIElement child in LayoutRoot.Children)
					ToolTipService.SetPlacement(child, value);
			}
		}
		public Action<LayoutGroupNewElementAdorner, LayoutGroupNewElementKind> AddNewElement;
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateButtons(true, true);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateButtons(false, true);
		}
		void OnAddNewElement(LayoutGroupNewElementKind newElementKind) {
			if (AddNewElement != null)
				AddNewElement(this, newElementKind);
		}
		void OnButtonClick(object sender, RoutedEventArgs e) {
			var newElementKind = (LayoutGroupNewElementKind)Enum.Parse(typeof(LayoutGroupNewElementKind), (string)((FrameworkElement)sender).Tag);
			OnAddNewElement(newElementKind);
		}
		void OnReverseAnimationCompleted(object sender, EventArgs e) {
			UpdateButtonsVisibility(false);
		}
		void OnShowGroupAsTabChanged() {
			ImageGroup.Visibility = TooltipGroup.Visibility = ShowGroupAsTab ? Visibility.Collapsed : Visibility.Visible;
			ImageTab.Visibility = TooltipTab.Visibility = ShowGroupAsTab ? Visibility.Visible : Visibility.Collapsed;
		}
		void UpdateButtons(bool isActive, bool useAnimation) {
			if (NewElementsVisibility != LayoutGroupNewElementsVisibility.All)
				useAnimation = false;
			if (!useAnimation || isActive)
				UpdateButtonsVisibility(isActive);
			if (useAnimation)
				if (isActive) {
					ReverseAnimation.Stop();
					Animation.Begin();
				}
				else {
					Animation.Stop();
					ReverseAnimation.Begin();
				}
		}
		void UpdateButtonsVisibility(bool isActive) {
			ButtonPlus.Visibility = isActive ? Visibility.Collapsed : Visibility.Visible;
			ButtonGroup.Visibility = isActive ? Visibility.Visible : Visibility.Collapsed;
			ButtonItem.Visibility = ButtonTabbedGroup.Visibility =
				NewElementsVisibility == LayoutGroupNewElementsVisibility.All && isActive ? Visibility.Visible : Visibility.Collapsed;
		}
		Storyboard Animation {
			get { return (Storyboard)Resources["animation" + Orientation.ToString()]; }
		}
		Storyboard ReverseAnimation {
			get { return (Storyboard)Resources["animation" + Orientation.ToString() + "Reverse"]; }
		}
	}
}
