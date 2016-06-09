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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public enum OpenStreetMapKind {
		Basic = OpenStreetMap.Basic,
		Mapquest = OpenStreetMap.Mapquest,
		MapquestSatellite = OpenStreetMap.MapquestSatellite,
		CycleMap = OpenStreetMap.CycleMap,
		Hot = OpenStreetMap.Hot,
		GrayScale = OpenStreetMap.GrayScale,
		Transport = OpenStreetMap.Transport,
		SkiMap = OpenStreetMap.SkiMap,
		SeaMarks = OpenStreetMap.SeaMarks,
		HikingRoutes = OpenStreetMap.HikingRoutes,
		CyclingRoutes = OpenStreetMap.CyclingRoutes,
		PublicTransport = OpenStreetMap.PublicTransport
	}
	public class OpenStreetMapDataProvider : MapDataProviderBase {
		internal const string DefaultOpenStreetTileUriTemplate = @"http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";
		internal const OpenStreetMapKind DefaultOpenStreetMapKind = OpenStreetMapKind.Basic;
		internal static readonly string[] DefaultSubdomains = new string[] { "a", "b", "c" };
		SphericalMercatorProjection projection = new SphericalMercatorProjection();
		string tileUriTemplate;
		string[] subdomains;
		OpenStreetMapKind kind = DefaultOpenStreetMapKind;
		bool ShouldUpdateKind { get { return String.IsNullOrEmpty(TileUriTemplate) || TileUriTemplate == DefaultOpenStreetTileUriTemplate; } }
		protected internal override Size BaseSizeInPixels { get { return new Size(OpenStreetMapTileSource.tileSize * 2, OpenStreetMapTileSource.tileSize * 2); } }
		protected internal string ActualTileUriTemplate {
			get { return string.Equals(TileUriTemplate, DefaultOpenStreetTileUriTemplate) ? GetOSMTileTemplate(Kind) : TileUriTemplate; }
		}
		protected internal string[] ActualSubdomains {
			get { return SameSubdomains() ? GetOSMSubdomains(Kind) : Subdomains; }
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("OpenStreetMapDataProviderTileUriTemplate"),
#endif
		DefaultValue(DefaultOpenStreetTileUriTemplate)]
		public string TileUriTemplate {
			get { return tileUriTemplate; }
			set {
				if (tileUriTemplate == value)
					return;
				tileUriTemplate = value;
				OnProviderPropertyChanged();
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("OpenStreetMapDataProviderProjection")]
#endif
		public override ProjectionBase Projection { get { return projection; } }
		[DefaultValue(DefaultOpenStreetMapKind)]
		public OpenStreetMapKind Kind {
			get { return kind; }
			set {
				if(kind == value)
					return;
				kind = value;
				if(ShouldUpdateKind)
					OnProviderPropertyChanged();
			}
		}
		public string[] Subdomains {
			get { return subdomains; }
			set {
				if(subdomains == value)
					return;
				subdomains = value;
				OnProviderPropertyChanged();
			}
		}
		void ResetSubdomains() {
			this.subdomains = DefaultSubdomains;
		}
		bool ShouldSerializeSubdomains() {
			return !SameSubdomains();
		}
		public OpenStreetMapDataProvider() {
			this.tileUriTemplate = DefaultOpenStreetTileUriTemplate;
			this.subdomains = DefaultSubdomains;
			UpdateTileSource();
		}
		string GetOSMTileTemplate(OpenStreetMapKind kind) {
			return OSMUtils.GetOSMTileTemplate((OpenStreetMap)kind);
		}
		string[] GetOSMSubdomains(OpenStreetMapKind kind) {
			return OSMUtils.GetOSMSubdomains((OpenStreetMap)kind);
		}
		void OnProviderPropertyChanged() {
			UpdateTileSource();
		}
		bool SameSubdomains() {
			if(this.subdomains.Length != DefaultSubdomains.Length)
				return false;
			for(int i = 0; i < subdomains.Length; i++)
				if(!string.Equals(subdomains[i], DefaultSubdomains[i]))
					return false;
			return true;
		}
		void UpdateTileSource() {
			TileSource = new OpenStreetMapTileSource(ActualTileUriTemplate, ActualSubdomains, this);
		}
		public override MapSize GetMapSizeInPixels(double zoomLevel) {
			double imageSize = OpenStreetMapTileSource.CalculateTotalImageSize(zoomLevel);
			return new MapSize(imageSize, imageSize);
		}
		public override string ToString() {
			return "(OpenStreetMapDataProvider)";
		}
	}
	internal class OpenStreetMapTileSource : MapTileSourceBase {
		public const int maxZoomLevel = 19;
		public const int tileSize = 256;
		readonly string urlTemplate;
		readonly string[] subdomains;
		static int imageWidth;
		static int imageHeight;
		protected internal override string Referer { get { return @"http://www.openstreetmap.org/"; } }
		protected internal override string TilePrefix { get { return "openstreet"; } }
		internal static double CalculateTotalImageSize(double zoomLevel) {
			if (zoomLevel < 1.0)
				return zoomLevel * tileSize * 2;
			return Math.Pow(2.0, zoomLevel) * tileSize;
		}
		static OpenStreetMapTileSource() { 
			int maxImageSize = Convert.ToInt32(CalculateTotalImageSize(maxZoomLevel));
			imageWidth = maxImageSize;
			imageHeight = maxImageSize;
		}
		internal OpenStreetMapTileSource(string urlTemplate, string[] subdomains, ICacheOptionsProvider cacheOptionsProvider)
			: base(imageWidth, imageHeight, tileSize, tileSize, cacheOptionsProvider) {
			this.urlTemplate = urlTemplate;
			this.subdomains = subdomains;
		}
		public override Uri GetTileByZoomLevel(int zoomLevel, int tilePositionX, int tilePositionY) {
			string subdomain = subdomains.Length > 0 ? subdomains[GetSubdomainIndex(subdomains.Length)] : "";
			return new Uri(string.Format(urlTemplate, subdomain, zoomLevel, tilePositionX, tilePositionY));
		}
	}
}
