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
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class SeriesPointItemContainer : Panel  {
		ChartContentPresenter SeriesPointItemPresenter { get { return FindName("PART_PointPresenter") as ChartContentPresenter; } }
		SeriesPointItem SeriesPointItem { get { return DataContext as SeriesPointItem; } }
		Series Series { get { return SeriesPointItem == null ? null : SeriesPointItem.Series; } }
		bool CanLayout { get { return SeriesPointItem != null && SeriesPointItem.Layout != null && Series != null; } }
		protected override Size MeasureOverride(Size availableSize) {
			Size size;
			if (CanLayout) {
				SeriesPointItem item = SeriesPointItem;
				Series.CompletePointLayout(item);
				Rect bounds = item.Layout.Bounds;
				size = new Size(bounds.Width, bounds.Height);
			}
			else
				size = new Size(0, 0);
			SeriesPointItemPresenter.Measure(size);
			return new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, SeriesPointItemPresenter.DesiredSize.Width),
				MathUtils.ConvertInfinityToDefault(availableSize.Height, SeriesPointItemPresenter.DesiredSize.Height));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			ChartContentPresenter presenter = SeriesPointItemPresenter;
			if (CanLayout) {
				SeriesPointLayout pointLayout = SeriesPointItem.Layout;
				presenter.RenderTransform = pointLayout.Transform;
				presenter.Clip = pointLayout.ClipGeometry;
				presenter.Arrange(pointLayout.Bounds);
			}
			else
				presenter.Arrange(RectExtensions.Zero);
			return finalSize;
		}
	}
}
