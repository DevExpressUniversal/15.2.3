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

using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class XRDiagramMarginsPanel : Panel {
		public static readonly DependencyProperty AdornerLeftMarginProperty;
		public static readonly DependencyProperty AdornerRightMarginProperty;
		static XRDiagramMarginsPanel() {
			DependencyPropertyRegistrator<XRDiagramMarginsPanel>.New()
				.Register(d => d.AdornerLeftMargin, out AdornerLeftMarginProperty, 0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.Register(d => d.AdornerRightMargin, out AdornerRightMarginProperty, 0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.OverrideDefaultStyleKey()
			;
		}
		public double AdornerLeftMargin {
			get { return (double)GetValue(AdornerLeftMarginProperty); }
			set { SetValue(AdornerLeftMarginProperty, value); }
		}
		public double AdornerRightMargin {
			get { return (double)GetValue(AdornerRightMarginProperty); }
			set { SetValue(AdornerRightMarginProperty, value); }
		}
		protected override Size MeasureOverride(Size constraint) {
			foreach(UIElement child in Children)
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return new Size(constraint.Width, constraint.Height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			bool firstLine = true;
			foreach(UIElement child in Children) {
				var line = child as Line;
				if(line != null) {
					child.Arrange(new Rect(new Point(0.0, 0.0), new Size(finalSize.Width, finalSize.Height)));
					line.Y1 = 0;
					line.Y2 = Math.Round(finalSize.Height);
					if(firstLine) {
						line.X1 = Math.Round(AdornerLeftMargin - 0.5) + 0.5;
						line.X2 = Math.Round(AdornerLeftMargin - 0.5) + 0.5;
						firstLine = false;
					} else {
						line.X1 = Math.Round(finalSize.Width - AdornerRightMargin - 0.5) + 0.5;
						line.X2 = Math.Round(finalSize.Width - AdornerRightMargin - 0.5) + 0.5;
					}
				}
			}
			return finalSize;
		}
	}
}
