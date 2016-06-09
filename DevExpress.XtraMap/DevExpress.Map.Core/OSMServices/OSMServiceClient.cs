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
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public enum OpenStreetMap {
		Basic,
		Mapquest,
		MapquestSatellite,
		CycleMap,
		Hot,
		GrayScale,
		StamenToner,
		StamenTonerHybrid,
		StamenTonerLabels,
		StamenTonerLines,
		StamenTonerBackground,
		StamenTonerLight,
		StamenWatercolor,
		Transport,
		CartoDBLight,
		CartoDBDark,
		SkiMap,
		SeaMarks,
		HikingRoutes,
		CyclingRoutes,
		PublicTransport
	}
	public static class OSMUtils {
		public static string GetOSMTileTemplate(OpenStreetMap kind) {
			switch(kind) {
				case OpenStreetMap.Basic: return @"http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";
				case OpenStreetMap.Mapquest: return @"http://otile{0}.mqcdn.com/tiles/1.0.0/osm/{1}/{2}/{3}.png";
				case OpenStreetMap.MapquestSatellite: return @"http://otile{0}.mqcdn.com/tiles/1.0.0/sat/{1}/{2}/{3}.png";
				case OpenStreetMap.CycleMap: return @"http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png";
				case OpenStreetMap.Hot: return @"http://{0}.tile.openstreetmap.fr/hot/{1}/{2}/{3}.png";
				case OpenStreetMap.GrayScale: return @"http://{0}.www.toolserver.org/tiles/bw-mapnik/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenToner: return @"http://{0}.tile.stamen.com/toner/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenTonerHybrid: return @"http://{0}.tile.stamen.com/toner-hybrid/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenTonerLabels: return @"http://{0}.tile.stamen.com/toner-labels/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenTonerLines: return @"http://{0}.tile.stamen.com/toner-lines/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenTonerBackground: return @"http://{0}.tile.stamen.com/toner-background/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenTonerLight: return @"http://{0}.tile.stamen.com/toner-lite/{1}/{2}/{3}.png";
				case OpenStreetMap.StamenWatercolor: return @"http://{0}.tile.stamen.com/watercolor/{1}/{2}/{3}.jpg";
				case OpenStreetMap.Transport: return @"http://{0}.tile2.opencyclemap.org/transport/{1}/{2}/{3}.png";
				case OpenStreetMap.CartoDBLight: return @"http://{0}.basemaps.cartocdn.com/light_all/{1}/{2}/{3}.png";
				case OpenStreetMap.CartoDBDark: return @"http://{0}.basemaps.cartocdn.com/dark_all/{1}/{2}/{3}.png";
				case OpenStreetMap.SkiMap: return @"http://tiles.openpistemap.org/nocontours/{1}/{2}/{3}.png";
				case OpenStreetMap.SeaMarks: return @"http://tiles.openseamap.org/seamark/{1}/{2}/{3}.png";
				case OpenStreetMap.HikingRoutes: return @"http://tile.lonvia.de/hiking/{1}/{2}/{3}.png";
				case OpenStreetMap.CyclingRoutes: return @"http://tile.waymarkedtrails.org/cycling/{1}/{2}/{3}.png";
				case OpenStreetMap.PublicTransport: return @"http://www.openptmap.org/tiles/{1}/{2}/{3}.png";
				default: goto case OpenStreetMap.Basic;
			}
		}
		public static string[] GetOSMSubdomains(OpenStreetMap kind) {
			switch(kind) {
				case OpenStreetMap.Basic: return new string[] { "a", "b", "c" };
				case OpenStreetMap.Mapquest: return new string[] { "1", "2", "3", "4" };
				case OpenStreetMap.MapquestSatellite: return new string[] { "1", "2", "3", "4" };
				case OpenStreetMap.CycleMap: return new string[] { "a", "b" };
				case OpenStreetMap.Hot: return new string[] { "a", "b" };
				case OpenStreetMap.GrayScale: return new string[] { "a", "b" };
				case OpenStreetMap.StamenToner: return new string[] { "a" };
				case OpenStreetMap.StamenTonerHybrid: return new string[] { "a" };
				case OpenStreetMap.StamenTonerLabels: return new string[] { "a" };
				case OpenStreetMap.StamenTonerLines: return new string[] { "a" };
				case OpenStreetMap.StamenTonerBackground: return new string[] { "a" };
				case OpenStreetMap.StamenTonerLight: return new string[] { "a" };
				case OpenStreetMap.StamenWatercolor: return new string[] { "a" };
				case OpenStreetMap.Transport: return new string[] { "a", "b" };
				case OpenStreetMap.CartoDBLight: return new string[] { "a" };
				case OpenStreetMap.CartoDBDark: return new string[] { "a" };
				case OpenStreetMap.SkiMap: return new string[0];
				case OpenStreetMap.SeaMarks: return new string[0];
				case OpenStreetMap.CyclingRoutes: return new string[0];
				case OpenStreetMap.PublicTransport: return new string[0];
				default: goto case OpenStreetMap.Basic;
			}
		}
	}
}
