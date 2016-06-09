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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
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
		internal const string DefaultTileUriTemplate = @"http://{subdomain}.tile.openstreetmap.org/{tileLevel}/{tileX}/{tileY}.png";
		internal static readonly string[] DefaultSubdomains = new string[] { "a", "b", "c" };
		public static readonly DependencyProperty KindProperty = DependencyPropertyManager.Register("Kind",
			typeof(OpenStreetMapKind), typeof(OpenStreetMapDataProvider), new PropertyMetadata(OpenStreetMapKind.Basic, KindPropertyChanged));
		public static readonly DependencyProperty TileUriTemplateProperty = DependencyPropertyManager.Register("TileUriTemplate",
			typeof(string), typeof(OpenStreetMapDataProvider), new PropertyMetadata(DefaultTileUriTemplate, TileUriTemplatePropertyChanged));
		public static readonly DependencyProperty SubdomainsProperty = DependencyPropertyManager.Register("Subdomains",
			typeof(string[]), typeof(OpenStreetMapDataProvider), new PropertyMetadata(DefaultSubdomains, SubdomainsPropertyChanged));
		[Category(Categories.Behavior)]
		public OpenStreetMapKind Kind {
			get { return (OpenStreetMapKind)GetValue(KindProperty); }
			set { SetValue(KindProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string TileUriTemplate {
			get { return (string)GetValue(TileUriTemplateProperty); }
			set { SetValue(TileUriTemplateProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string[] Subdomains {
			get { return (string[])GetValue(SubdomainsProperty); }
			set { SetValue(SubdomainsProperty, value); }
		}
		static void KindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			OpenStreetMapDataProvider dataProvider = d as OpenStreetMapDataProvider;
			if (dataProvider != null && dataProvider.ShouldUpdateKind)
				dataProvider.UpdateTileSource();
		}
		static void TileUriTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			OpenStreetMapDataProvider dataProvider = d as OpenStreetMapDataProvider;
			if (dataProvider != null)
				dataProvider.UpdateTileSource();
		}
		static void SubdomainsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			OpenStreetMapDataProvider dataProvider = d as OpenStreetMapDataProvider;
			if(dataProvider != null)
				dataProvider.UpdateTileSource();
		}
		SphericalMercatorProjection projection = new SphericalMercatorProjection();
		bool ShouldUpdateKind { get { return String.IsNullOrEmpty(TileUriTemplate) || TileUriTemplate == DefaultTileUriTemplate; } }
		protected internal string ActualTileUriTemplate {
			get { return string.Equals(TileUriTemplate, DefaultTileUriTemplate) ? GetOSMTileTemplate(Kind) : TileUriTemplate; }
		}
		protected internal string[] ActualSubdomains {
			get { return SameSubdomains() ? GetOSMSubdomains(Kind) : Subdomains; }
		}
		public override ProjectionBase Projection { get { return projection; } }
		public OpenStreetMapDataProvider() {
			UpdateTileSource();
		}
		void UpdateTileSource() {
			SetTileSource(new OpenStreetMapTileSource(ActualTileUriTemplate, ActualSubdomains));
		}
		string GetOSMTileTemplate(OpenStreetMapKind kind) {
			string formattedTemplate = OSMUtils.GetOSMTileTemplate((OpenStreetMap)kind);
			return string.Format(formattedTemplate, "{subdomain}", "{tileLevel}", "{tileX}", "{tileY}");
		}
		string[] GetOSMSubdomains(OpenStreetMapKind kind) {
			return OSMUtils.GetOSMSubdomains((OpenStreetMap)kind);
		}
		bool SameSubdomains() {
			if(Subdomains.Length != DefaultSubdomains.Length)
				return false;
			for(int i = 0; i < Subdomains.Length; i++)
				if(!string.Equals(Subdomains[i], DefaultSubdomains[i]))
					return false;
			return true;
		}
		public override Size GetMapSizeInPixels(double zoomLevel) {
			double imageSize = OpenStreetMapTileSource.CalculateTotalImageSize(zoomLevel);
			return new Size(imageSize, imageSize);
		}
		protected override MapDependencyObject CreateObject() {
			return new OpenStreetMapDataProvider();
		}
	}
	public class OpenStreetMapTileSource : MapTileSourceBase {
		public const int maxZoomLevel = 20;
		public const int tileSize = 256;
		static int imageWidth = (int)Math.Pow(2.0, maxZoomLevel) * tileSize;
		static int imageHeight = (int)Math.Pow(2.0, maxZoomLevel) * tileSize;
		readonly string urlTemplate;
		readonly string[] subdomains;
		internal OpenStreetMapTileSource(string urlTemplate, string[] subdomains)
			: base(imageWidth, imageHeight, tileSize, tileSize) {
			this.urlTemplate = urlTemplate;
			this.subdomains = subdomains;
		}
		internal static double CalculateTotalImageSize(double zoomLevel) {
			if (zoomLevel < 1.0)
				return zoomLevel * tileSize * 2;
			return Math.Pow(2.0, zoomLevel) * tileSize;
		}
		public override Uri GetTileByZoomLevel(int zoomLevel, int tilePositionX, int tilePositionY) {
			string url = urlTemplate;
			string subdomain = subdomains.Length > 0 ? subdomains[GetSubdomainIndex(subdomains.Length)] : "";
			url = url.Replace("{tileX}", tilePositionX.ToString(CultureInfo.InvariantCulture));
			url = url.Replace("{tileY}", tilePositionY.ToString(CultureInfo.InvariantCulture));
			url = url.Replace("{tileLevel}", zoomLevel.ToString(CultureInfo.InvariantCulture));
			url = url.Replace("{subdomain}", subdomain);
			return new Uri(url);
		}
	}
}
