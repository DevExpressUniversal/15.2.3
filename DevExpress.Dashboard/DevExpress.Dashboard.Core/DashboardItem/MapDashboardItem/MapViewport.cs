#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Map;
using DevExpress.Map.Native;
namespace DevExpress.DashboardCommon.Native {
	public class MapViewport {
		internal const double MinimumLatitude = -90;
		internal const double MaximumLatitude = 90;
		internal const double DefaultBottomLatitude = MinimumLatitude;
		internal const double DefaultTopLatitude = MaximumLatitude;
		internal const double DefaultLeftLongitude = -180;
		internal const double DefaultRightLongitude = 180;
		internal const double DefaultCenterPointLatitude = 0;
		internal const double DefaultCenterPointLongitude = 0;
		const bool DefaultCreateViewerPaddings = true;
		const string xmlTopLatitude = "TopLatitude";
		const string xmlBottomLatitude = "BottomLatitude";
		const string xmlLeftLongitude = "LeftLongitude";
		const string xmlRightLongitude = "RightLongitude";
		const string xmlCenterPointLatitude = "CenterPointLatitude";
		const string xmlCenterPointLongitude = "CenterPointLongitude";
		const string xmlCreateViewerPaddings = "CreateViewerPaddings";
		readonly MapCoordinateSystemCore coordinateSystem = new MapGeoCoordinateSystemCore(new GeoPointFactory());
		readonly MapDashboardItem dashboardItem;
		double topLatitude = DefaultTopLatitude;
		double bottomLatitude = DefaultBottomLatitude;
		double leftLongitude = DefaultLeftLongitude;
		double rightLongitude = DefaultRightLongitude;
		double centerPointLatitude = DefaultCenterPointLatitude;
		double centerPointLongitude = DefaultCenterPointLongitude;
		bool createViewerPaddings = DefaultCreateViewerPaddings;
		[
		DefaultValue(DefaultTopLatitude),
		Description("")
		]
		public double TopLatitude {
			get { return topLatitude; }
			set {
				CheckLatitude(value);
				if(topLatitude != value) {
					topLatitude = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultBottomLatitude),
		Description("")
		]
		public double BottomLatitude {
			get { return bottomLatitude; }
			set {
				CheckLatitude(value);
				if(bottomLatitude != value) {
					bottomLatitude = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultLeftLongitude),
		Description("")
		]
		public double LeftLongitude {
			get { return leftLongitude; }
			set {
				if(leftLongitude != value) {
					leftLongitude = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultRightLongitude),
		Description("")
		]
		public double RightLongitude {
			get { return rightLongitude; }
			set {
				if(rightLongitude != value) {
					rightLongitude = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultCenterPointLatitude),
		Description("")
		]
		public double CenterPointLatitude {
			get { return centerPointLatitude; }
			set {
				if(centerPointLatitude != value) {
					centerPointLatitude = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultCenterPointLongitude),
		Description("")
		]
		public double CenterPointLongitude {
			get { return centerPointLongitude; }
			set {
				if(centerPointLongitude != value) {
					centerPointLongitude = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultCreateViewerPaddings),
		Description("")
		]
		public bool CreateViewerPaddings {
			get { return createViewerPaddings; }
			set {
				if(createViewerPaddings != value) {
					createViewerPaddings = value;
					OnChanged();
				}
			}
		}
		internal bool IsDefault { 
			get { 
				return TopLatitude == DefaultTopLatitude && BottomLatitude == DefaultBottomLatitude 
					&& LeftLongitude == DefaultLeftLongitude && RightLongitude == DefaultRightLongitude && CreateViewerPaddings == DefaultCreateViewerPaddings; 
			} 
		}
		internal MapViewport(MapDashboardItem dashboardItem) {
			this.dashboardItem = dashboardItem;
			if(dashboardItem != null)
				ReCalculate();
		}
		internal void ReCalculate() {
			if(dashboardItem.MapItems == null)
				return;
			IList<MapShapePoint> points = dashboardItem.MapItems.SelectMany(item => {
				MapShapeDot dot = item as MapShapeDot;
				if(dot != null)
					return new[] { new MapShapePoint(dot.Latitude, dot.Longitude) }; ;
				MapShapePath path = item as MapShapePath;
				if(path != null)
					return path.Segments.SelectMany(s => s.Points).ToArray();
				return new MapShapePoint[0];
			}).ToList();
			if(points.Count == 0)
				return;
			topLatitude = points.Max(p => p.Latitude);
			bottomLatitude = points.Min(p => p.Latitude);
			leftLongitude = points.Min(p => p.Longitude);
			rightLongitude = points.Max(p => p.Longitude);
			InitializeCenterPoint();
			createViewerPaddings = true;
		}
		internal MapViewport Clone() {
			MapViewport viewport = new MapViewport(dashboardItem);
			viewport.bottomLatitude = bottomLatitude;
			viewport.topLatitude = topLatitude;
			viewport.leftLongitude = leftLongitude;
			viewport.rightLongitude = rightLongitude;
			viewport.centerPointLatitude = centerPointLatitude;
			viewport.centerPointLongitude = centerPointLongitude;
			viewport.createViewerPaddings = createViewerPaddings;
			return viewport;
		}
		internal void Apply(MapViewport viewport) {
			bottomLatitude = viewport.bottomLatitude;
			topLatitude = viewport.topLatitude;
			leftLongitude = viewport.leftLongitude;
			rightLongitude = viewport.rightLongitude;
			centerPointLatitude = viewport.centerPointLatitude;
			centerPointLongitude = viewport.centerPointLongitude;
			createViewerPaddings = viewport.createViewerPaddings;
			OnChanged();
		}
		public override bool Equals(object obj) {
			MapViewport viewport = obj as MapViewport;
			if(viewport == null)
				return base.Equals(viewport);
			return TopLatitude == viewport.TopLatitude && BottomLatitude == viewport.BottomLatitude && LeftLongitude == viewport.LeftLongitude && RightLongitude == viewport.RightLongitude &&
				CenterPointLatitude == viewport.CenterPointLatitude && CenterPointLongitude == viewport.CenterPointLongitude;
		}
		public override int GetHashCode() {
			return TopLatitude.GetHashCode() ^ BottomLatitude.GetHashCode() ^ LeftLongitude.GetHashCode() ^ 
				RightLongitude.GetHashCode() ^ CenterPointLatitude.GetHashCode() ^ CenterPointLongitude.GetHashCode();
		}
		void InitializeCenterPoint() {
			double centerPointDiff = 0;
			if(RightLongitude > DefaultRightLongitude) {
				centerPointDiff += RightLongitude - DefaultRightLongitude;
				rightLongitude = DefaultRightLongitude;
			}
			if(LeftLongitude < DefaultLeftLongitude) {
				centerPointDiff += LeftLongitude - DefaultLeftLongitude;
				leftLongitude = DefaultLeftLongitude;
			}
			IMapUnit minUnit = coordinateSystem.CoordPointToMapUnit(new GeoPointEx(BottomLatitude, LeftLongitude));
			IMapUnit maxUnit = coordinateSystem.CoordPointToMapUnit(new GeoPointEx(TopLatitude, RightLongitude));
			IMapUnit centerUnit = new MapUnitCore((minUnit.X + maxUnit.X) / 2.0, (minUnit.Y + maxUnit.Y) / 2.0);
			CoordPoint centerPoint = coordinateSystem.MapUnitToCoordPoint(centerUnit);
			centerPointLatitude = centerPoint.GetY();
			centerPointLongitude = centerPoint.GetX() + centerPointDiff;
		}
		internal void SaveToXml(XElement element) {
			if(TopLatitude != DefaultTopLatitude)
				element.Add(new XAttribute(xmlTopLatitude, TopLatitude));
			if(BottomLatitude != DefaultBottomLatitude)
				element.Add(new XAttribute(xmlBottomLatitude, BottomLatitude));
			if(LeftLongitude != DefaultLeftLongitude)
				element.Add(new XAttribute(xmlLeftLongitude, LeftLongitude));
			if(RightLongitude != DefaultRightLongitude)
				element.Add(new XAttribute(xmlRightLongitude, RightLongitude));
			if(CenterPointLatitude != DefaultCenterPointLatitude)
				element.Add(new XAttribute(xmlCenterPointLatitude, CenterPointLatitude));
			if(CenterPointLongitude != DefaultCenterPointLongitude)
				element.Add(new XAttribute(xmlCenterPointLongitude, CenterPointLongitude));
			if(CreateViewerPaddings != DefaultCreateViewerPaddings)
				element.Add(new XAttribute(xmlCreateViewerPaddings, CreateViewerPaddings));
		}
		internal void LoadFromXml(XElement element) {
			string topLatitudeAttr = XmlHelper.GetAttributeValue(element, xmlTopLatitude);
			if(!string.IsNullOrEmpty(topLatitudeAttr))
				topLatitude = XmlHelper.FromString<double>(topLatitudeAttr);
			string bottomLatitudeAttr = XmlHelper.GetAttributeValue(element, xmlBottomLatitude);
			if(!string.IsNullOrEmpty(bottomLatitudeAttr))
				bottomLatitude = XmlHelper.FromString<double>(bottomLatitudeAttr);
			string leftLongitudeAttr = XmlHelper.GetAttributeValue(element, xmlLeftLongitude);
			if(!string.IsNullOrEmpty(leftLongitudeAttr))
				leftLongitude = XmlHelper.FromString<double>(leftLongitudeAttr);
			string rightLongitudeAttr = XmlHelper.GetAttributeValue(element, xmlRightLongitude);
			if(!string.IsNullOrEmpty(rightLongitudeAttr))
				rightLongitude = XmlHelper.FromString<double>(rightLongitudeAttr);
			string centerPointLatitudeAttr = XmlHelper.GetAttributeValue(element, xmlCenterPointLatitude);
			if(!string.IsNullOrEmpty(centerPointLatitudeAttr))
				centerPointLatitude = XmlHelper.FromString<double>(centerPointLatitudeAttr);
			string centerPointLongitudeAttr = XmlHelper.GetAttributeValue(element, xmlCenterPointLongitude);
			if(!string.IsNullOrEmpty(centerPointLongitudeAttr))
				centerPointLongitude = XmlHelper.FromString<double>(centerPointLongitudeAttr);
			string createViewerPaddingsAttr = XmlHelper.GetAttributeValue(element, xmlCreateViewerPaddings);
			if(!string.IsNullOrEmpty(createViewerPaddingsAttr))
				createViewerPaddings = XmlHelper.FromString<bool>(createViewerPaddingsAttr);
		}
		void CheckLatitude(double value) {
			if(value < MinimumLatitude || value > MaximumLatitude)
				throw new ArgumentException(String.Format(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectMapLatitude), MinimumLatitude, MaximumLatitude));
		}
		void OnChanged() {
			if(dashboardItem != null && !dashboardItem.Loading)
				dashboardItem.OnChanged(ChangeReason.View);
		}
	}
}
