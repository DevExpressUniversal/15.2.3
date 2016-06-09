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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum SolidSides {
		Left,
		Top,
		Right,
		Bottom,
		Front
	}
	public class Pseudo3DBarModelPanel : Panel {
		public static readonly DependencyProperty SidesProperty = DependencyPropertyManager.RegisterAttached("Sides", 
			typeof(SolidSides), typeof(Pseudo3DBarModelPanel), new PropertyMetadata(SolidSides.Front));
		public static void SetSides(UIElement element, SolidSides value) {
			element.SetValue(SidesProperty, value);
		}
		[NonCategorized]
		public static SolidSides GetSides(UIElement element) {
			return (SolidSides)element.GetValue(SidesProperty);
		}
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement child in Children) {
				switch (GetSides(child)) {
					case SolidSides.Top:
					case SolidSides.Bottom:
						child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
						break;
					case SolidSides.Right:
					case SolidSides.Left:
						child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
						break;
					case SolidSides.Front:
					default:
						child.Measure(availableSize);
						break;
				}
			}
			Size constraint = new Size();
			constraint.Width = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;
			constraint.Height = double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height;
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				switch (GetSides(child)) {
					case SolidSides.Top:
						child.Arrange(new Rect(new Point(0, 0), new Size(finalSize.Width, child.DesiredSize.Height)));
						break;
					case SolidSides.Bottom:
						child.Arrange(new Rect(new Point(0, finalSize.Height - child.DesiredSize.Height), new Size(finalSize.Width, child.DesiredSize.Height)));
						break;
					case SolidSides.Left:
						child.Arrange(new Rect(new Point(0, 0), new Size(child.DesiredSize.Width, finalSize.Height)));
						break;
					case SolidSides.Right:
						child.Arrange(new Rect(new Point(finalSize.Width - child.DesiredSize.Width, 0), new Size(child.DesiredSize.Width, finalSize.Height)));
						break;
					case SolidSides.Front:
					default:
						child.Arrange(new Rect(new Point(0, 0), finalSize));
						break;
				}
			}
			return finalSize;
		}
	}
}
