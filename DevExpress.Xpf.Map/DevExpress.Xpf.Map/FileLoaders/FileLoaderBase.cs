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
using System.IO;
using System.Windows;
using System.Windows.Resources;
using System.Collections.Generic;
using DevExpress.Map.Native;
using DevExpress.Map;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class ShapesLoadedEventArgs : EventArgs {
		readonly List<MapItem> shapes;
		public List<MapItem> Shapes { get { return shapes; } }
		internal ShapesLoadedEventArgs(List<MapItem> shapes) {
			this.shapes = shapes;
		}
	}
	public delegate void ShapesLoadedEventHandler(object sender, ShapesLoadedEventArgs e);
}
namespace DevExpress.Xpf.Map.Native {
	public class XpfMapLoaderFactory : MapLoaderFactoryCore<MapItem> {
		const string templateSource = "<StackPanel Orientation=\"Horizontal\" RenderTransform=\"{Binding Transform}\"><Image VerticalAlignment=\"Top\" Source=\"{Binding Bitmap}\" Width=\"{Binding ImageWidth}\" Height=\"{Binding ImageHeight}\"/><TextBlock Text=\"{Binding TitleText}\" FontSize=\"{Binding TextSize}\" Foreground=\"{Binding Foreground}\"/></StackPanel>";
		static readonly DataTemplate customElementTemplate = XamlHelper.GetTemplate(templateSource);
		public override MapItem CreateDot(CoordPoint coordPoint) {
			return new MapDot() { Location = coordPoint };
		}
		public override MapItem CreateDot(CoordPoint coordPoint, double size) {
			return new MapDot() { Location = coordPoint, Size = size };
		}
		public override MapItem CreateImageAndText(CoordPoint point, ImageTextInfoCore info) {
			KmlPointData pointData = KmlPointData.Create(info);
			return new MapCustomElement() { Content = pointData, ContentTemplate = customElementTemplate, Location = (GeoPoint)point };
		}
		public override MapItem CreateLine(CoordPoint point1, CoordPoint point2) {
			return new MapLine() { Point1 = point1, Point2 = point2 };
		}
		public override MapItem CreatePolyline() {
			MapPolyline polyline = new MapPolyline();
			polyline.Points = new CoordPointCollection();
			return polyline;
		}
		public override MapItem CreatePolygon() {
			MapPolygon polygon = new MapPolygon();
			polygon.Points = new CoordPointCollection();
			return polygon;
		}
		public override MapItem CreatePath() {
			MapPath path = new MapPath();
			MapPathGeometry geometry = new MapPathGeometry();
			path.Data = geometry;
			return path;
		}
		public override MapItem CreateRectangle(CoordPoint location, double width, double height) {
			return new MapRectangle() { Location = location, Width = width, Height = height };
		}
		public override MapItem CreateEllipse(CoordPoint location, double width, double height) {
			return new MapEllipse() { Location = location, Width = width, Height = height };
		}
	}
	public abstract class WpfResourceLoaderBehaviorBase : ResourceLoaderBehaviorBase {
		public static WpfResourceLoaderBehaviorBase GetResourceLoaderBehavior(Uri resourceUri) {
			string lowerUri = resourceUri.ToString().ToLower();
			if (lowerUri.Contains(";component/"))
				return new ResourceLoaderBehavior();
			else if (lowerUri.Contains("pack://application:"))
				return new ApplicationLoaderBehavior();
			else if (lowerUri.Contains("pack://siteoforigin:"))
				return new SiteOfOriginLoaderBehavior();
			else if (!resourceUri.IsAbsoluteUri)
				return new ResourceLoaderBehavior();
			return null;
		}
		public abstract StreamResourceInfo GetResourceInfo(Uri resourceUri);
		public override Stream OpenResource(Uri resourceUri) {
			StreamResourceInfo streamInfo = null;
			try {
				streamInfo = GetResourceInfo(resourceUri);
			}
			catch { }
			if (streamInfo != null)
				return streamInfo.Stream;
			return null;
		}
	}
	public class ResourceLoaderBehavior : WpfResourceLoaderBehaviorBase {
		public override StreamResourceInfo GetResourceInfo(Uri resourceUri) {
			return Application.GetResourceStream(resourceUri);
		}
	}
	public class SiteOfOriginLoaderBehavior : WpfResourceLoaderBehaviorBase {
		public override StreamResourceInfo GetResourceInfo(Uri resourceUri) {
			return Application.GetRemoteStream(resourceUri);
		}
	}
	public class ApplicationLoaderBehavior : WpfResourceLoaderBehaviorBase {
		public override StreamResourceInfo GetResourceInfo(Uri resourceUri) {
			return Application.GetContentStream(resourceUri);
		}
	}
}
