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
using System.Windows.Shapes;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_AxisGeometry", Type = typeof(Path)),
	TemplatePart(Name = "PART_MajorTickmarksGeometry", Type = typeof(Path)),
	TemplatePart(Name = "PART_MinorTickmarksGeometry", Type = typeof(Path))
	]
	public class Axis2DPresentation : ChartElementBase, IHitTestableElement, ILayoutElement, IFinishInvalidation {
		readonly Axis2DItem axis2DItem;
		public Axis2DItem AxisItem { get { return axis2DItem; } }
		public AxisBase Axis { get { return axis2DItem.Axis as AxisBase; } }
		ILayout ILayoutElement.Layout { get { return axis2DItem == null ? null : axis2DItem.Layout; } }
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return Axis; } }
		object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
		internal Axis2DPresentation(Axis2DItem axis2DItem) {
			DefaultStyleKey = typeof(Axis2DPresentation);
			this.axis2DItem = axis2DItem;
		}
		protected override Size MeasureOverride(Size availableSize) {
			double totalThickness = axis2DItem.TotalThickness;
			Size size = axis2DItem.Axis.IsVertical ? new Size(totalThickness, 0) : new Size(0, totalThickness);
			base.MeasureOverride(size);
			return size;
		}
	}
}
