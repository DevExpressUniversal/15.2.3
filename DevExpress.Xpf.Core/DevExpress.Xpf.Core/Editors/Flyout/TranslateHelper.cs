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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Flyout.Native {
	public static class TranslateHelper {
		public static Rect TranslateBounds(UIElement baseElement, UIElement element) {
			var elementBounds = new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height);
			if (element.RenderTransform != null) {
				elementBounds = element.RenderTransform.TransformBounds(elementBounds);
				elementBounds.X = 0;
				elementBounds.Y = 0;
			}
			if (!((FrameworkElement)element).IsInVisualTree())
				return new Rect();
			Rect res = element.TransformToVisual(baseElement).TransformBounds(elementBounds);
			return res;
		}
		public static Rect ToScreen(Visual baseElement, Rect rect) {
			if (baseElement == null)
				return rect;
			return new Rect(baseElement.PointToScreen(rect.TopLeft), baseElement.PointToScreen(rect.BottomRight));
		}
	}
}
