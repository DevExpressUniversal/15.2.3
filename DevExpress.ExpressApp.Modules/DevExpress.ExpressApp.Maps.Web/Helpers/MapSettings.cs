#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Maps.Web.Helpers {
	public class MapSettings {
		public string LocalizedShowDetailsString { get; set; }
		public bool IsShowDetailsEnabled { get; set; }
		public bool IsMarkersTooltipsEnabled { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public MapProvider Provider { get; set; }
		public IList<IMapsMarker> Markers { get; private set; }
		public MapPoint Center { get; set; }
		public int ZoomLevel { get; set; }
		public MapType Type { get; set; }
		public bool IsControlsEnabled { get; set; }
		public string GoogleApiKey { get; set; }
		public string GoogleStaticApiKey { get; set; }
		public string BingApiKey { get; set; }
		public RouteSettings RouteSettings { get; set; }
		public bool isRouteEnabled {
			get {
				return RouteSettings != null && RouteSettings.Enabled;
			}
		}
		public MapSettings() {
			Markers = new List<IMapsMarker>();
			RouteSettings = new RouteSettings();
		}
	}
	public class RouteSettings {
		public bool Enabled { get; set; }
		public Color Color { get; set; }
		public int Weight { get; set; }
		public RouteMode Mode { get; set; }
		public double Opacity { get; set; }
	}
}
