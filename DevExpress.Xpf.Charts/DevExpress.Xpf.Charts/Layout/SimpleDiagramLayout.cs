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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum LayoutDirection {
		Horizontal = SimpleDiagramLayoutDirection.Horizontal,
		Vertical = SimpleDiagramLayoutDirection.Vertical
	}
	public abstract class SimpleDiagramPanelBase : Panel {
		IList<GRect2D> seriesBounds = null;
		int CalculateCellCount() {
			int cellCount = 0;
			foreach (UIElement child in Children) {
				if (ShouldShowElement(child))
					cellCount++;
			}
			return cellCount;
		}
		IList<GRect2D> CalculateSeriesBounds(ISimpleDiagram diagram, Size constraint) {
			List<GRect2D> result = new List<GRect2D>();
			int cellCount = CalculateCellCount();
			GRect2D diagramBounds = new GRect2D(0, 0, (int)Math.Floor(constraint.Width), (int)Math.Floor(constraint.Height));
			IList<GRect2D> bounds = cellCount > 0 ? SimpleDiagramLayout.Calculate(diagram, diagramBounds, cellCount) : null;
			int currentIndex = 0;
			foreach (UIElement child in Children) {
				if (ShouldShowElement(child))
					result.Add(bounds[currentIndex++]);
				else
					result.Add(new GRect2D(0, 0, 0, 0));
			}
			return result;
		}
		protected abstract ISimpleDiagram GetDiagram();
		protected abstract bool ShouldShowElement(UIElement child);
		protected override Size MeasureOverride(Size constraint) {
			if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height))
				return base.MeasureOverride(constraint);
			ISimpleDiagram diagram = GetDiagram();
			if (diagram != null && Children.Count > 0) {
				seriesBounds = CalculateSeriesBounds(diagram, constraint);
				for (int i = 0; i < Children.Count; i++) {
					GRect2D bounds = seriesBounds[i];
					Children[i].Measure(new Size(bounds.Width, bounds.Height));
				}
			}
			return constraint;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			ISimpleDiagram diagram = GetDiagram();
			if (diagram != null && Children.Count > 0) {
				for (int i = 0; i < Children.Count; i++) {
					GRect2D bounds = seriesBounds[i];
					Children[i].Arrange(new Rect(bounds.Left, bounds.Top, bounds.Width, bounds.Height));
				}
			}
			return arrangeBounds;
		}		
	}
}
