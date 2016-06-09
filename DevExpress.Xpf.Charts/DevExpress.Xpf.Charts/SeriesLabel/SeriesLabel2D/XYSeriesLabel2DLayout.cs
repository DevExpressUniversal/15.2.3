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

using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum ResolveOverlappingMode {
		None = ResolveOverlappingModeCore.None,
		Default = ResolveOverlappingModeCore.Default,
		HideOverlapped = ResolveOverlappingModeCore.HideOverlapped,
		JustifyAroundPoint = ResolveOverlappingModeCore.JustifyAroundPoint,
		JustifyAllAroundPoint = ResolveOverlappingModeCore.JustifyAllAroundPoint
	}
	public class XYSeriesLabel2DLayout : SeriesLabel2DLayout, IXYDiagramLabelLayout {
		readonly GRect2D validRectangle = GRect2D.Empty;
		readonly GRect2D excludedRectangle = GRect2D.Empty;
		readonly ResolveOverlappingModeCore resolveOverlappingMode = ResolveOverlappingModeCore.None;
		readonly Rect viewport;
		readonly bool isNavigationEnabled;
		bool isVisibleForResolveOverlapping;
		GRect2D ILabelLayout.LabelBounds { get { return LabelBounds; } set { LabelBounds = value; } }
		bool ILabelLayout.Visible { get { return isVisibleForResolveOverlapping; } set { isVisibleForResolveOverlapping = value; } }
		GPoint2D IXYDiagramLabelLayout.AnchorPoint { get { return new GPoint2D(MathUtils.StrongRound(AnchorPoint.X), MathUtils.StrongRound(AnchorPoint.Y)); } }
		GRect2D IXYDiagramLabelLayout.ExcludedRectangle { get { return excludedRectangle; } }
		GRect2D IXYDiagramLabelLayout.ValidRectangle { get { return validRectangle; } }
		ResolveOverlappingModeCore IXYDiagramLabelLayout.ResolveOverlappingMode { get { return resolveOverlappingMode; } }
		protected internal override bool IsVisibleForResolveOverlapping { get { return isVisibleForResolveOverlapping; } }
		internal GRect2D ValidRectangle { get { return validRectangle; } }
		public override bool Visible { get { return isVisibleForResolveOverlapping && (!isNavigationEnabled || viewport.IntersectsWith(Bounds)); } }
		internal XYSeriesLabel2DLayout(SeriesLabelItem labelItem, IMapping mapping, Point anchorPoint, double indent, double angle, GRect2D excludedRectangle)
			: base(labelItem, anchorPoint, indent, angle) {
			this.viewport = mapping.Viewport;
			this.isNavigationEnabled = mapping.NavigationEnabled;
			this.isVisibleForResolveOverlapping = mapping.IsLabelVisibleForResolveOverlapping(anchorPoint);
			this.resolveOverlappingMode = (ResolveOverlappingModeCore)labelItem.Label.ResolveOverlappingMode;
			this.excludedRectangle = excludedRectangle;
		}
		internal XYSeriesLabel2DLayout(SeriesLabelItem labelItem, PaneMapping mapping, Point anchorPoint, GRect2D validRectangle)
			: base(labelItem, anchorPoint, anchorPoint) {
			this.viewport = mapping.Viewport;
			this.isNavigationEnabled = mapping.NavigationEnabled;
			this.isVisibleForResolveOverlapping = mapping.IsLabelVisibleForResolveOverlapping(anchorPoint);
			this.resolveOverlappingMode = (ResolveOverlappingModeCore)labelItem.Label.ResolveOverlappingMode;
			this.validRectangle = validRectangle;
		}
		internal XYSeriesLabel2DLayout(SeriesLabelItem labelItem, PaneMapping mapping, Point centerPoint, GRect2D validRectangle, Point initialAnchorPoint)
			: base(labelItem, centerPoint, centerPoint) {
			this.viewport = mapping.Viewport;
			this.isNavigationEnabled = mapping.NavigationEnabled;
			this.isVisibleForResolveOverlapping = mapping.IsLabelVisibleForResolveOverlapping(initialAnchorPoint);
			this.resolveOverlappingMode = (ResolveOverlappingModeCore)labelItem.Label.ResolveOverlappingMode;
			this.validRectangle = validRectangle;
		}
		internal XYSeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint, PaneMapping mapping, Point centerPoint)
			: base(labelItem, anchorPoint, centerPoint) {
			this.viewport = mapping.Viewport;
			this.isNavigationEnabled = mapping.NavigationEnabled;
			this.isVisibleForResolveOverlapping = true;
			this.resolveOverlappingMode = (ResolveOverlappingModeCore)labelItem.Label.ResolveOverlappingMode;
		}
	}
}
