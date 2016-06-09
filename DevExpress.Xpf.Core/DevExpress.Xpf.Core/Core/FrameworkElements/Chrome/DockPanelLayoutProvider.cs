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
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core.Native {
	public class DockPanelLayoutProvider : LayoutProvider {
		public bool LastChildFill { get; set; }
		public override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			double usedWidth = 0.0;
			double usedHeight = 0.0;
			double maximumWidth = 0.0;
			double maximumHeight = 0.0;
			for (int i = 0; i < context.RenderChildrenCount; i++) {
				var element = context.GetRenderChild(i);
				Size remainingSize = new Size(
					Math.Max(0.0, availableSize.Width - usedWidth),
					Math.Max(0.0, availableSize.Height - usedHeight));
				element.Measure(remainingSize);
				Size desiredSize = element.DesiredSize;
				switch (GetDock(element)) {
					case Dock.Left:
					case Dock.Right:
						maximumHeight = Math.Max(maximumHeight, usedHeight + desiredSize.Height);
						usedWidth += desiredSize.Width;
						break;
					case Dock.Top:
					case Dock.Bottom:
						maximumWidth = Math.Max(maximumWidth, usedWidth + desiredSize.Width);
						usedHeight += desiredSize.Height;
						break;
				}
			}
			maximumWidth = Math.Max(maximumWidth, usedWidth);
			maximumHeight = Math.Max(maximumHeight, usedHeight);
			return new Size(maximumWidth, maximumHeight);
		}
		Dock GetDock(FrameworkRenderElementContext element) {
			var dock = element.Dock ?? element.Factory.Dock;
			return dock ?? default(Dock);
		}
		public override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			double left = 0.0;
			double top = 0.0;
			double right = 0.0;
			double bottom = 0.0;
			int childrenCount = context.RenderChildrenCount;
			int dockedCount = childrenCount - (LastChildFill ? 1 : 0);
			int index = 0;
			for (int i = 0; i < childrenCount; i++) {
				var element = context.GetRenderChild(i);
				Rect remainingRect = new Rect(
					left,
					top,
					Math.Max(0.0, finalSize.Width - left - right),
					Math.Max(0.0, finalSize.Height - top - bottom));
				if (index < dockedCount) {
					Size desiredSize = element.DesiredSize;
					switch (GetDock(element)) {
						case Dock.Left:
							left += desiredSize.Width;
							remainingRect.Width = desiredSize.Width;
							break;
						case Dock.Top:
							top += desiredSize.Height;
							remainingRect.Height = desiredSize.Height;
							break;
						case Dock.Right:
							right += desiredSize.Width;
							remainingRect.X = Math.Max(0.0, finalSize.Width - right);
							remainingRect.Width = desiredSize.Width;
							break;
						case Dock.Bottom:
							bottom += desiredSize.Height;
							remainingRect.Y = Math.Max(0.0, finalSize.Height - bottom);
							remainingRect.Height = desiredSize.Height;
							break;
					}
				}
				element.Arrange(remainingRect);
				index++;
			}
			return finalSize;
		}
	}
}
