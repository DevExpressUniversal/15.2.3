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

using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public enum ResolveOverlappingModeCore {
		None,
		Default,
		HideOverlapped,
		JustifyAroundPoint,
		JustifyAllAroundPoint
	}
	public interface ILabelLayout {
		bool Visible { get; set; }
		GRect2D LabelBounds { get; set; }		
	}
	public interface IXYDiagramLabelLayout : ILabelLayout {
		ResolveOverlappingModeCore ResolveOverlappingMode { get; }
		GPoint2D AnchorPoint { get; }
		GRect2D ValidRectangle { get; }
		GRect2D ExcludedRectangle { get; }
	}
	public class XYDiagramResolveOverlappingAlgorithm {
		static bool IsVisible(IXYDiagramLabelLayout label, GPoint2D point) {
			return label.ValidRectangle.IsEmpty || label.ValidRectangle.Contains(point);
		}
		public static void Process(IList<IXYDiagramLabelLayout> labels, GRect2D bounds, int overlappingIndent) {
			new XYDiagramResolveOverlappingAlgorithm(labels, bounds, overlappingIndent).Process();
		}
		readonly List<IXYDiagramLabelLayout> actualLabels = new List<IXYDiagramLabelLayout>();
		RectanglesLayoutAlgorithm layoutAlgorithm;
		XYDiagramResolveOverlappingAlgorithm(IList<IXYDiagramLabelLayout> labels, GRect2D bounds, int overlappingIndent) {
			layoutAlgorithm = new RectanglesLayoutAlgorithm(bounds, overlappingIndent);
			foreach (IXYDiagramLabelLayout label in labels) {
				if (label.Visible) {
					if (label.ResolveOverlappingMode != ResolveOverlappingModeCore.None) {
						actualLabels.Add(label);
						if (!label.ExcludedRectangle.IsEmpty && label.ResolveOverlappingMode != ResolveOverlappingModeCore.HideOverlapped)
							layoutAlgorithm.AddExcludedRectangle(label.ExcludedRectangle);
					}
					else
						layoutAlgorithm.AddOccupiedRectangle(label.LabelBounds);
				}
			}
		}		
		void Process() {
			if (actualLabels.Count != 0) {
				foreach (IXYDiagramLabelLayout label in actualLabels)
					Arrange(label);
			}
		}
		void Arrange(IXYDiagramLabelLayout label) {
			GPoint2D labelCenter = new GPoint2D(label.LabelBounds.Left + label.LabelBounds.Width / 2, 
				label.LabelBounds.Top + label.LabelBounds.Height / 2);
			GRect2D rect = new GRect2D(label.AnchorPoint.X - label.LabelBounds.Width / 2, label.AnchorPoint.Y - label.LabelBounds.Height / 2,
				label.LabelBounds.Width, label.LabelBounds.Height);
			switch (label.ResolveOverlappingMode) {
				case ResolveOverlappingModeCore.Default:
					if (!IsVisible(label, labelCenter) || !layoutAlgorithm.IsEmptyRegion(label.LabelBounds, false))
						label.LabelBounds = layoutAlgorithm.ArrangeRectangle(label.LabelBounds, label.ValidRectangle, true);
					else
						layoutAlgorithm.AddOccupiedRectangle(label.LabelBounds);
					break;
				case ResolveOverlappingModeCore.HideOverlapped:
					if (!IsVisible(label, labelCenter) || !layoutAlgorithm.IsEmptyRegionByList(label.LabelBounds, false))
						label.Visible = false;
					else
						layoutAlgorithm.AddOccupiedRectangle(label.LabelBounds);
					break;
				case ResolveOverlappingModeCore.JustifyAroundPoint:
					if (!IsVisible(label, labelCenter) || !layoutAlgorithm.IsEmptyRegion(label.LabelBounds, false)) {
						label.LabelBounds = layoutAlgorithm.ArrangeRectangle(rect, label.ValidRectangle, false, label.ExcludedRectangle);
					}
					else
						layoutAlgorithm.AddOccupiedRectangle(label.LabelBounds);
					break;
				case ResolveOverlappingModeCore.JustifyAllAroundPoint:
					label.LabelBounds = layoutAlgorithm.ArrangeRectangle(rect, label.ValidRectangle, true);
					break;
			}
		}
	}
}
