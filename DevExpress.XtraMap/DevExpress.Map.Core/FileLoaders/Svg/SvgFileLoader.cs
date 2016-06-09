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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using DevExpress.Data.Svg;
namespace DevExpress.Map.Native {
	public abstract class SvgFileLoaderCore<TItem> : MapFormatLoaderCore<TItem> where TItem : class {
		List<TItem> items;
		public override List<TItem> Items { get { return items; } }
		public ISvgPointConverter PointConverter { get; set; }
		public double StrokeScaleFactor { get; set; }
		protected SvgFileLoaderCore(MapLoaderFactoryCore<TItem> factory)
			: base(factory) {
			this.items = new List<TItem>();
			StrokeScaleFactor = 0.35;
		}
		TItem CreatePlatformLine(SvgLine svgLine) {
			if (svgLine == null) return null;
			CoordPoint coordPoint1 = SvgPointToCoordPoint(svgLine.Point1);
			CoordPoint coordPoint2 = SvgPointToCoordPoint(svgLine.Point2);
			TItem platformLine = Factory.CreateLine(coordPoint1, coordPoint2);
			return platformLine;
		}
		TItem CreatePlatformPolyline(SvgPolyline svgPolyline) {
			if (svgPolyline == null) return null;
			TItem platformPolyline = Factory.CreatePolyline();
			AddPointsToItem(svgPolyline, (IPointContainerCore)platformPolyline);
			return platformPolyline;
		}
		TItem CreatePlatformPolygon(SvgPolygon svgPolygon) {
			if (svgPolygon == null) return null;
			TItem platformPolygon = Factory.CreatePolygon();
			AddPointsToItem(svgPolygon, (IPointContainerCore)platformPolygon);
			return platformPolygon;
		}
		TItem CreatePlatformPath(SvgPath svgPath) {
			if (svgPath == null) return null;
			TItem platformPath = Factory.CreatePath();
			IPathCore path = (IPathCore)platformPath;
			SvgPoint segmentFirstPoint = new SvgPoint();
			IPathSegmentCore currentSegment = null;
			IList<SvgCommandBase> commandsList = svgPath.CommandCollection.Commands;
			if (commandsList.Count > 0 && commandsList[0].CommandAction != SvgCommandAction.Start)
				currentSegment = path.CreateSegment();
			foreach (SvgCommandBase command in commandsList) {
				if (command.CommandAction == SvgCommandAction.Start) {
					currentSegment = path.CreateSegment();
					segmentFirstPoint = command.GeneralPoint;
				}
				SvgPoint actualPoint = command.CommandAction == SvgCommandAction.Stop ? segmentFirstPoint : command.GeneralPoint;
				CoordPoint coordPoint = SvgPointToCoordPoint(actualPoint);
				currentSegment.AddPoint(coordPoint);
			}
			return platformPath;
		}
		TItem CreatePlatformRectangle(SvgRectangle svgRectangle) {
			if (svgRectangle == null) return null;
			CoordPoint location = SvgPointToCoordPoint(svgRectangle.Location);
			SvgSize size = SvgSizeToCoordSize(svgRectangle.Location, new SvgSize(svgRectangle.Width, svgRectangle.Height));
			TItem platformRectangle = Factory.CreateRectangle(location, size.Width, size.Height);
			return platformRectangle;
		}
		TItem CreatePlatformEllipse(SvgEllipse svgEllipse) {
			if (svgEllipse == null) return null;
			SvgPoint leftTopPoint = svgEllipse.Location - new SvgPoint(svgEllipse.Width, svgEllipse.Height);
			CoordPoint location = SvgPointToCoordPoint(leftTopPoint);
			SvgSize size = SvgSizeToCoordSize(leftTopPoint, new SvgSize(svgEllipse.Width * 2.0, svgEllipse.Height * 2.0));
			TItem platformEllipse = Factory.CreateEllipse(location, size.Width, size.Height); 
			return platformEllipse;
		}
		TItem CreatePlatformEllipse(SvgCircle svgCircle) {
			if (svgCircle == null) return null;
			return CreatePlatformEllipse(new SvgEllipse() { Location = svgCircle.Location, Width = svgCircle.Radius, Height = svgCircle.Radius });
		}
		void AddPointsToItem(SvgPointContainer svgContainer, IPointContainerCore container) {
			foreach (SvgPoint svgPoint in svgContainer.PointCollection.Points) {
				CoordPoint coordPoint = SvgPointToCoordPoint(svgPoint);
				container.AddPoint(coordPoint);
			}
		}
		void SetItemStyles(TItem platformItem, SvgShapeStyle svgShapeStyle) {
			IMapShapeStyleCore itemStyle = (IMapShapeStyleCore)platformItem;
			double scaledStrokeWidth = svgShapeStyle.StrokeWidth / StrokeScaleFactor;
			itemStyle.Fill = svgShapeStyle.Fill;
			itemStyle.Stroke = svgShapeStyle.Stroke;
			itemStyle.StrokeWidth = scaledStrokeWidth;
		}
		CoordPoint SvgPointToCoordPoint(SvgPoint svgPoint) {
			return PointConverter != null ? PointConverter.SvgToCoordPoint(svgPoint) : CoordFactory.CreatePoint(svgPoint.X, svgPoint.Y);
		}
		SvgSize SvgSizeToCoordSize(SvgPoint point, SvgSize svgSize) {
			return PointConverter != null ? PointConverter.SvgToCoordSize(point, svgSize) : svgSize;
		}
		void ProcessingFormatItems(IList<SvgItem> svgItems) {
			this.items = new List<TItem>();
			var matcher = SvgElementHelper.GetMatcher(
				line => CreatePlatformLine(line),
				ellipse => CreatePlatformEllipse(ellipse),
				circle => CreatePlatformEllipse(circle),
				rectangle => CreatePlatformRectangle(rectangle),
				path => CreatePlatformPath(path),
				polyline => CreatePlatformPolyline(polyline),
				polygon => CreatePlatformPolygon(polygon)
			);
			foreach (SvgElement svgItem in svgItems) {
				TItem platformItem = matcher(svgItem);
				if (platformItem != null) {
					SetItemStyles(platformItem, svgItem.ElementStyle);
					this.items.Insert(0, platformItem);
				}
			}
		}
		protected override void LoadStream(Stream stream) {
			SvgImporter importer = new SvgImporter();
			importer.Import(stream);
			RaiseBoundsCalculated(new CoordBounds(importer.Bounds));
			ProcessingFormatItems(importer.SvgItems);
		}
	}
	public interface ISvgPointConverter {
		SvgPoint CoordToSvgPoint(CoordPoint point);
		CoordPoint SvgToCoordPoint(SvgPoint point);
		SvgSize CoordToSvgSize(CoordPoint point, SvgSize size);
		SvgSize SvgToCoordSize(SvgPoint point, SvgSize size);
	}
}
