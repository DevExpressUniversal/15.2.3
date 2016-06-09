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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class ToolTipPanel : Panel {
		public static readonly DependencyProperty PositionProperty = DependencyPropertyManager.Register("Position",
			typeof(Point), typeof(ToolTipPanel), new PropertyMetadata(InvalidateLayout));
		public Point Position {
			get { return (Point)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		static void InvalidateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ToolTipPanel panel = d as ToolTipPanel;
			if (panel != null) {
				panel.InvalidateMeasure();
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size result = new Size(0, 0);
			System.Diagnostics.Debug.Assert(Children.Count == 1);
			UIElement child = Children[0];
			child.Measure(availableSize);
			result.Width = Math.Max(result.Width, child.DesiredSize.Width);
			result.Height = Math.Max(result.Height, child.DesiredSize.Height);
			result.Width = double.IsPositiveInfinity(availableSize.Width) ? result.Width : availableSize.Width;
			result.Height = double.IsPositiveInfinity(availableSize.Height) ? result.Height : availableSize.Height;
			return result;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			System.Diagnostics.Debug.Assert(Children.Count == 1);
			UIElement child = Children[0];
			double height = child.DesiredSize.Height;
			double halfWidth = 0.5 * child.DesiredSize.Width;
			child.Arrange(new Rect(Position.X - halfWidth, Position.Y - height, child.DesiredSize.Width, height));
			return finalSize;
		}
	}
	[NonCategorized]
	public class ToolTipInfo : INotifyPropertyChanged {
		string toolTipText;
		Point toolTipPosition;
		Visibility visibility = Visibility.Collapsed;
		DataTemplate contentTemplate;
		object item;
		public event PropertyChangedEventHandler PropertyChanged;
		public LayerBase Layer { get; internal set; }
		public GeoPoint GeoPoint { get; internal set; }
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
			internal set {
				if (toolTipPosition == value)
					return;
				toolTipPosition = value;
				RaisePropertyChanged("ToolTipPosition");
			}
		}
		public Visibility Visibility {
			get { return visibility; }
			internal set {
				if (visibility == value)
					return;
				visibility = value;
				RaisePropertyChanged("Visibility");
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
		protected void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		internal void HideToolTip() {
			Visibility = Visibility.Collapsed;
		}
		internal void UpdatePosition() {
			if (Visibility == Visibility.Visible && Layer != null && GeoPoint != null) {
				MapUnit point = Layer.GeoPointToMapUnit(GeoPoint);
				ToolTipPosition = Layer.MapUnitToScreenPoint(point, true);
			}
		}
	}
}
