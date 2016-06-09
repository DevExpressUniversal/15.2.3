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
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Flyout.Native;
namespace DevExpress.Xpf.Editors.Flyout.Native {
	public partial class FlyoutBase {
		public partial class FlyoutStrategy {
			public virtual FlyoutPositionCalculator CreatePositionCalculator() {
				return new FlyoutPositionCalculator();
			}
			public virtual FrameworkElement GetMeasureElement(FlyoutBase flyoutControl) {
				return flyoutControl.ChildContainer;
			}
			public virtual Point GetOpenAnimationOffset(FlyoutBase flyoutControl) {
				Rect targetBounds = flyoutControl.PositionCalculator.TargetBounds;
				Rect flyoutBounds = flyoutControl.PositionCalculator.Result.Bounds;
				if (targetBounds.IsEmpty || flyoutBounds.IsEmpty)
					return new Point();
				Point targetLocation = targetBounds.Location();
				Point flyoutLocation = flyoutBounds.Location();
				Size minSize = new Size(
					Math.Min(targetBounds.Width, flyoutBounds.Width),
					Math.Min(targetBounds.Height, flyoutBounds.Height)
				);
				return new Point(minSize.Width * Math.Sign(targetLocation.X - flyoutLocation.X), minSize.Height * Math.Sign(targetLocation.Y - flyoutLocation.Y));
			}
			public virtual Rect GetDefaultTargetBounds(FlyoutBase flyoutControl) {
				return new Rect();
			}
			public virtual void UpdatePopupSize(FlyoutBase flyoutControl, Size size) {
				if (flyoutControl.RenderGrid == null)
					return;
				if (flyoutControl.VerticalAlignment == VerticalAlignment.Stretch)
					flyoutControl.RenderGrid.Height = size.Height;
				else
					flyoutControl.RenderGrid.ClearValue(FrameworkElement.HeightProperty);
				if (flyoutControl.HorizontalAlignment == HorizontalAlignment.Stretch)
					flyoutControl.RenderGrid.Width = size.Width;
				else
					flyoutControl.RenderGrid.ClearValue(FrameworkElement.WidthProperty);
			}
		}
		public class FlyinStrategy : FlyoutStrategy {
			public override FlyoutPositionCalculator CreatePositionCalculator() {
				return new FlyinPositionCalculator();
			}
			public override Point GetOpenAnimationOffset(FlyoutBase flyoutControl) {
				double x = flyoutControl.PositionCalculator.Result.Size.Width;
				double y = flyoutControl.PositionCalculator.Result.Size.Height;
				if (flyoutControl.VerticalAlignment == VerticalAlignment.Top)
					y = -y;
				if (flyoutControl.HorizontalAlignment == HorizontalAlignment.Left)
					x = -x;
				if (flyoutControl.VerticalAlignment == VerticalAlignment.Center || flyoutControl.VerticalAlignment == VerticalAlignment.Stretch)
					y = 0;
				if (flyoutControl.HorizontalAlignment == HorizontalAlignment.Center || flyoutControl.HorizontalAlignment == HorizontalAlignment.Stretch)
					x = 0;
				return new Point(x, y);
			}
			public override Rect GetDefaultTargetBounds(FlyoutBase flyoutControl) {
				return flyoutControl.PositionCalculator.GetScreenRect(ScreenHelper.GetScreenRect(flyoutControl.FlyoutContainer.Element).Center());
			}
		}
	}
}
