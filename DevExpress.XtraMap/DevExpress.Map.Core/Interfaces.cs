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
using System.Drawing;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Map.Native {
	public interface IMapItemAttribute {
		string Name { get; set; }
		Type Type { get; set; }
		object Value { get; set; }
	}
	public interface IMapItemAttributeOwner {
		IMapItemAttribute GetAttribute(string name);
	}
	public interface IMapUnit {
		double X { get; }
		double Y { get; }
	}
	public interface IMapItemCore {
		string Text { get; }
		Color TextColor { get; }
		int AttributesCount { get; }
		void AddAttribute(IMapItemAttribute attribute);
		IMapItemAttribute GetAttribute(int index);
		IMapItemAttribute GetAttribute(string name);
	}
	public interface IMapDataItem  {
		int RowIndex { get; set; }
		int[] ListSourceRowIndices { get; set; }
		void AddAttribute(IMapItemAttribute attribute);
	}
	public interface IMapChartItem : IMapChartDataItem {
		double ValueSizeInPixels { get; set; }
	}
	public interface IMapDataEnumerator : IEnumerator {
		void Accept(IMapDataItem item, int itemIndex);
		int GetCurrentRowIndex();
	}
	public interface IMapChartDataItem : IMapDataItem {
		object Argument { get; set; }
		double Value { get; set; }
	}
	public interface IMapChartGroupDataItem : IMapChartDataItem {
		object GroupKey { get; set; }
	}
	public interface IMapContainerDataItem : IMapDataItem {
		IMapChartDataItem CreateSegment();
		void AddSegment(IMapChartDataItem child);
	}
	public interface IPathCore {
		int SegmentCount { get; }
		IPathSegmentCore CreateSegment();
		IPathSegmentCore GetSegment(int index);
	}
	public interface IPointContainerCore {
		int PointCount { get; }
		void LockPoints();
		void AddPoint(CoordPoint point);
		void UnlockPoints();
		CoordPoint GetPoint(int index);
	}
	public interface IPolygonCore : IPointContainerCore {
		CoordBounds GetBounds();
	}
	public interface IPathSegmentCore : IPolygonCore {
		bool IsFilled { get; set; }
		bool IsClosed { get; set; }
		int InnerContourCount { get; }
		IPointContainerCore CreateInnerContour();
		IPointContainerCore GetInnerCountour(int index);
	}
	public interface IPointCore {
		CoordPoint Location { get; set; }
	}
	public interface ISupportIntermediatePoints {
		IList<CoordPoint> Vertices { get; }
	}
	public interface ISupportStyleCore {
		void SetStrokeWidth(double width);
		void SetFill(Color color);
		void SetStroke(Color color);
	}
	public interface IImageTransform {
		[SuppressMessage("Microsoft.Design", "CA1021: Avoid out parameters")]
		void CalcImageOrigin(double imageWidth, double imageHeight, out double originX, out double originY);
	}
	public interface IMapPointerStyleCore : IMapItemCore {
		Uri ImageUri { get; }
		Image Image { get; }
	}
	public interface IMapShapeStyleCore : IMapItemCore {
		Color Fill { get; set; }
		Color Stroke { get; set; }
		double StrokeWidth { get; set; }
	}
	public interface ISupportRectangle : ISupportCoordLocation {
		double Width { get; }
		double Height { get; }
	}
	public interface IEllipseCore : ISupportRectangle {
	}
	public interface IRectangleCore : ISupportRectangle {
	}
}
