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
namespace DevExpress.Xpf.Core.Native {
	public class GridPanelLayoutProvider : LayoutProvider {
		public override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			Size desiredSize = new Size();
			for (int i = 0; i < context.RenderChildrenCount; i++) {
				var child = context.GetRenderChild(i);
				double wCoeff = 1d;
				double hCoeff = 1d;
				switch (GetDock(child)) {
					case Dock.Left:
					case Dock.Right:
						wCoeff = 0.5d;
						break;
					case Dock.Top:
					case Dock.Bottom:
						hCoeff = 0.5d;
						break;
				}
				child.Measure(new Size(wCoeff * availableSize.Width, hCoeff * availableSize.Height));
				desiredSize.Width = Math.Max(desiredSize.Width, child.DesiredSize.Width / wCoeff);
				desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height / hCoeff);
			}
			return desiredSize;
		}
		public override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			for (int i = 0; i < context.RenderChildrenCount; i++) {
				var child = context.GetRenderChild(i);
				var lCoeff = 0d;
				var tCoeff = 0d;
				double wCoeff = 1d;
				double hCoeff = 1d;
				switch (GetDock(child)) {
					case Dock.Left:
						wCoeff = 0.5d;
						break;
					case Dock.Right:
						lCoeff = 0.5d;
						wCoeff = 0.5d;
						break;
					case Dock.Top:
						hCoeff = 0.5d;
						break;
					case Dock.Bottom:
						tCoeff = 0.5d;
						hCoeff = 0.5d;
						break;
				}
				child.Arrange(new Rect(lCoeff * finalSize.Width, tCoeff * finalSize.Height, wCoeff * finalSize.Width, hCoeff * finalSize.Height));
			}
			return finalSize;
		}
		Dock? GetDock(FrameworkRenderElementContext element) {
			return element.Dock ?? element.Factory.Dock;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return GridInstance;
		}
	}
}
