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
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class UniformStackPanel : PanelBase {
		public const double DefaultChildSpacing = 5;
		#region Dependency Properties
		public static readonly DependencyProperty ChildSpacingProperty =
			DependencyProperty.Register("ChildSpacing", typeof(double), typeof(UniformStackPanel),
				new PropertyMetadata(DefaultChildSpacing, (o, e) => ((UniformStackPanel)o).OnChildSpacingChanged()));
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformStackPanel),
				new PropertyMetadata(Orientation.Horizontal, (o, e) => ((UniformStackPanel)o).OnOrientationChanged()));
		#endregion Dependency Properties
		public double ChildSpacing {
			get { return (double)GetValue(ChildSpacingProperty); }
			set { SetValue(ChildSpacingProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			Size result = base.OnMeasure(availableSize);
			if(Orientation == Orientation.Horizontal)
				availableSize.Width = double.PositiveInfinity;
			else
				availableSize.Height = double.PositiveInfinity;
			var maxChildSize = SizeHelper.Zero;
			foreach(FrameworkElement child in GetLogicalChildren(false)) {
				child.Measure(availableSize);
				SizeHelper.UpdateMaxSize(ref maxChildSize, child.DesiredSize);
			}
			MaxChildSize = maxChildSize;
			int childCount = GetLogicalChildren(true).Count;
			if(Orientation == Orientation.Horizontal) {
				result.Width = Math.Max(result.Width, childCount * MaxChildSize.Width + (childCount - 1) * ChildSpacing);
				result.Height = Math.Max(result.Height, MaxChildSize.Height);
			}
			else {
				result.Width = Math.Max(result.Width, MaxChildSize.Width);
				result.Height = Math.Max(result.Height, childCount * MaxChildSize.Height + (childCount - 1) * ChildSpacing);
			}
			return result;
		}
		protected override Size OnArrange(Rect bounds) {
			Rect childBounds = bounds;
			foreach(FrameworkElement child in GetLogicalChildren(true)) {
				if(Orientation == Orientation.Horizontal)
					childBounds.Width = MaxChildSize.Width;
				else
					childBounds.Height = MaxChildSize.Height;
				child.Arrange(childBounds);
				if(Orientation == Orientation.Horizontal)
					childBounds.X = childBounds.Right + ChildSpacing;
				else
					childBounds.Y = childBounds.Bottom + ChildSpacing;
			}
			return base.OnArrange(bounds);
		}
		protected Size MaxChildSize { get; private set; }
		#endregion Layout
		protected virtual void OnChildSpacingChanged() {
			Changed();
		}
		protected virtual void OnOrientationChanged() {
			Changed();
		}
	}
}
