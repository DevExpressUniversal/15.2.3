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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.TreeMap {
	[NonCategorized]
	public class ToolTipPanel : Panel {
		public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
			typeof(Point), typeof(ToolTipPanel), new PropertyMetadata(InvalidateLayout));
		public static readonly DependencyProperty ShadowPaddingProperty = DependencyProperty.RegisterAttached("ShadowPadding",
			typeof(Thickness), typeof(ToolTipPanel), new PropertyMetadata(InvalidateLayout), new ValidateValueCallback(ValidateShadowPadding));
		static void InvalidateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ToolTipPanel panel = d as ToolTipPanel;
			if (panel != null)
				panel.InvalidateMeasure();
		}
		static bool ValidateShadowPadding(object value) {
			Thickness padding = (Thickness)value;
			return padding.Left >= 0.0 && padding.Top >= 0.0 && padding.Bottom >= 0.0 && padding.Right >= 0.0;
		}
		public Point Position {
			get { return (Point)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		public static Thickness GetShadowPadding(DependencyObject target) {
			return (Thickness)target.GetValue(ShadowPaddingProperty);
		}
		public static void SetShadowPadding(DependencyObject target, Thickness value) {
			target.SetValue(ShadowPaddingProperty, value);
		}
		TreeMapControl treeMapControl;
		Popup popup;
		TreeMapControl TreeMapControl {
			get {
				if (treeMapControl == null)
					treeMapControl = LayoutHelper.FindLayoutOrVisualParentObject(this, typeof(TreeMapControl)) as TreeMapControl;
				return treeMapControl;
			}
		}
		Popup Popup {
			get {
				if (popup == null)
					popup = FindPopup();
				return popup;
			}
		}
		Popup FindPopup() {
			System.Diagnostics.Debug.Assert(Children.Count == 1);
			UIElement child = Children[0];
			Popup popup = child as Popup;
			return child as Popup;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Popup != null && Popup.Child != null)
				Popup.Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return base.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (TreeMapControl != null && Popup != null && Popup.Child != null) {
				UIElement toolTipContent = Popup.Child;
				toolTipContent.InvalidateArrange();
				Size childSize = toolTipContent.DesiredSize;
				toolTipContent.Arrange(new Rect(new Point(0, 0), childSize));
				ToolTipInfo info = TreeMapControl.ToolTipInfo;
				if (info != null && info.Layout != null) {
					ToolTipLayoutStrategy strategy = ToolTipLayoutStrategy.Create(this.FlowDirection);
					Size scaledSize = ToolTipLayoutHelper.CorrectSizeByTransform(childSize, TreeMapControl);
					Point popupPosition = strategy.CalculateToolTipPosition(Position, scaledSize, info.Layout.ScreenBounds);
					info.HorizontalBeakAlignment = strategy.CalculateHorizontalBeakPosition(Position, scaledSize, info.Layout.ScreenBounds);
					info.VerticalBeakAlignment = strategy.CalculateVerticalBeakPosition(Position, scaledSize, info.Layout.ScreenBounds);
					if (VisualTreeHelper.GetChildrenCount(popup.Child) > 0) {
						DependencyObject userTreeRoot = VisualTreeHelper.GetChild(popup.Child, 0);
						Thickness shadowPadding = ToolTipPanel.GetShadowPadding(userTreeRoot);
						popupPosition = strategy.CorrectPositionByShadow(popupPosition, TreeMapControl, info.VerticalBeakAlignment, info.HorizontalBeakAlignment, shadowPadding);
					}
					Popup.HorizontalOffset = popupPosition.X;
					Popup.VerticalOffset = popupPosition.Y;
				}
			}
			return base.ArrangeOverride(finalSize);
		}
	}
	[NonCategorized]
	public class ToolTipInfo : INotifyPropertyChanged, IPatternHolder {
		string toolTipText;
		Point toolTipPosition;
		bool visible;
		DataTemplate contentTemplate;
		object item;
		ToolTipLayout layout;
		HorizontalAlignment horizontalBeakAlignment = HorizontalAlignment.Center;
		VerticalAlignment verticalBeakAlignment = VerticalAlignment.Bottom;
		public event PropertyChangedEventHandler PropertyChanged;
		internal ToolTipLayout Layout {
			get { return layout; }
			set {
				layout = value;
				ToolTipPosition = layout.AnchorPoint;
			}
		}
		public string ToolTipText {
			get { return toolTipText; }
			internal set {
				if (toolTipText == value)
					return;
				toolTipText = value;
				RaisePropertyChanged("ToolTipText");
			}
		}
		public Point ToolTipPosition {
			get { return toolTipPosition; }
			private set {
				if (toolTipPosition == value)
					return;
				toolTipPosition = value;
				RaisePropertyChanged("ToolTipPosition");
			}
		}
		public bool Visible {
			get { return visible; }
			internal set {
				if (visible == value)
					return;
				visible = value;
				RaisePropertyChanged("Visible");
			}
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			internal set {
				if (contentTemplate == value)
					return;
				contentTemplate = value;
				RaisePropertyChanged("ContentTemplate");
			}
		}
		public object Item {
			get { return item; }
			internal set {
				if (item == value)
					return;
				item = value;
				RaisePropertyChanged("Item");
			}
		}
		public HorizontalAlignment HorizontalBeakAlignment {
			get { return horizontalBeakAlignment; }
			internal set {
				if (horizontalBeakAlignment != value) {
					horizontalBeakAlignment = value;
					RaisePropertyChanged("HorizontalBeakAlignment");
				}
			}
		}
		public VerticalAlignment VerticalBeakAlignment {
			get { return verticalBeakAlignment; }
			internal set {
				if (verticalBeakAlignment != value) {
					verticalBeakAlignment = value;
					RaisePropertyChanged("VerticalBeakAlignment");
				}
			}
		}
		protected void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		internal void HideToolTip() {
			Visible = false;
		}
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
 			return new ToolTipPatternProvider(patternConstant);
		}
	}
	public abstract class GenericEnumToVisibilityConverter<TEnum> : IValueConverter where TEnum : struct {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Visibility) && parameter != null) {
				TEnum enumValue;
				if (Enum.TryParse<TEnum>(parameter.ToString(), out enumValue) && (value is TEnum))
					return enumValue.Equals((TEnum)value) ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalBeakAlignmentToVisibilityConverter : GenericEnumToVisibilityConverter<HorizontalAlignment> {
	}
	public class VerticalBeakAlignmentToVisibilityConverter : GenericEnumToVisibilityConverter<VerticalAlignment> {
	}
}
