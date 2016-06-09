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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Collections;
namespace DevExpress.Xpf.Core {
	public class StackVisibleIndexPanel : OrderPanelBase {
		public static Size ArrangeElements(Size finalSize, IList<UIElement> elements, SizeHelperBase sizeHelper) {
			return ArrangeElements<UIElement>(new Rect(new Point(), finalSize), elements, sizeHelper, child => child.DesiredSize, (double panelSize, double childSize) => childSize, (child, rect, isVisible) => child.Arrange(rect));
		}
		public static Size ArrangeElements<T>(Rect finalRect, IList<T> elements, SizeHelperBase sizeHelper, Func<T, Size> getDesiredSizeFunc, Func<double, double, double> getActualSize, Action<T, Rect, bool> arrangeAction) {
			double panelSize = sizeHelper.GetDefineSize(finalRect.Size());
			Size finalSize = finalRect.Size();
			double defineSize = 0;
			foreach(T child in elements) {
				Point location = sizeHelper.CreatePoint(sizeHelper.GetDefinePoint(finalRect.Location()) + defineSize, sizeHelper.GetSecondaryPoint(finalRect.Location()));
				finalRect.X = location.X;
				finalRect.Y = location.Y;
				Size desiredSize = getDesiredSizeFunc(child);
				defineSize = sizeHelper.GetDefineSize(desiredSize);
				Size size = sizeHelper.CreateSize(defineSize, Math.Max(sizeHelper.GetSecondarySize(finalRect.Size()), sizeHelper.GetSecondarySize(desiredSize)));
				RectHelper.SetSize(ref finalRect, sizeHelper.CreateSize(getActualSize(panelSize, sizeHelper.GetDefineSize(size)), sizeHelper.GetSecondarySize(size)));
				arrangeAction(child, finalRect, panelSize > 0);
				panelSize -= sizeHelper.GetDefineSize(finalRect.Size());
			}
			return finalSize;
		}
		public static Size MeasureElements<T>(Size availableSize, IList<T> elements, SizeHelperBase sizeHelper, Func<Size, T, Size> measureFunc) {
			availableSize = sizeHelper.CreateSize(double.PositiveInfinity, sizeHelper.GetSecondarySize(availableSize));
			double defineSize = 0;
			double secondarySize = 0;
			foreach(T child in elements) {
				Size desiredSize = measureFunc(availableSize, child);
				secondarySize = Math.Max(secondarySize, sizeHelper.GetSecondarySize(desiredSize));
				defineSize += sizeHelper.GetDefineSize(desiredSize);
			}
			return sizeHelper.CreateSize(defineSize, secondarySize);
		}
		public static Size MeasureElements(Size availableSize, IList<UIElement> elements, SizeHelperBase sizeHelper) {
			return MeasureElements<UIElement>(availableSize, elements, sizeHelper, (size, child) => { child.Measure(size); return child.DesiredSize; });
		}
		protected override Size MeasureSortedChildrenOverride(Size availableSize, IList<UIElement> sortedChildren) {
			return MeasureElements(availableSize, sortedChildren, SizeHelper);
		}
		protected override Size ArrangeSortedChildrenOverride(Size finalSize, IList<UIElement> sortedChildren) {
			return ArrangeElements(finalSize, sortedChildren, SizeHelper);
		}
	}
}
