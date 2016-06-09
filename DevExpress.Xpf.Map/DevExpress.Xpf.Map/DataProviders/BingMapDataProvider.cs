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
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.BingServices;
using DevExpress.Map.Native;
using System.Reflection;
namespace DevExpress.Xpf.Map {
	public enum BingMapKind {
		Road,
		Area,
		Hybrid
	}
	public class BingMapDataProvider : MapDataProviderBase {
		public static readonly DependencyProperty KindProperty = DependencyPropertyManager.Register("Kind",
			typeof(BingMapKind), typeof(BingMapDataProvider), new PropertyMetadata(BingMapKind.Hybrid, BingPropertyChanged));
		public static readonly DependencyProperty BingKeyProperty = DependencyPropertyManager.Register("BingKey",
			typeof(string), typeof(BingMapDataProvider), new PropertyMetadata(string.Empty, BingKeyChanged));
		public static readonly DependencyProperty CultureNameProperty = DependencyPropertyManager.Register("CultureName",
			typeof(string), typeof(BingMapDataProvider), new PropertyMetadata(string.Empty, BingPropertyChanged));
		bool isKeyRestricted = false;
		[Category(Categories.Behavior)]
		public BingMapKind Kind {
			get { return (BingMapKind)GetValue(KindProperty); }
			set { SetValue(KindProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string BingKey {
			get { return (string)GetValue(BingKeyProperty); }
			set { SetValue(BingKeyProperty, value); }
		}
		[Category(Categories.Behavior)]
		public string CultureName {
			get { return (string)GetValue(CultureNameProperty); }
			set { SetValue(CultureNameProperty, value); }
		}
		static void BingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingMapDataProvider dataProvider = d as BingMapDataProvider;
			if(dataProvider != null) {
				Assembly asm = Assembly.GetEntryAssembly();
				dataProvider.isKeyRestricted = DXBingKeyVerifier.IsKeyRestricted(dataProvider.BingKey, asm != null ? asm.FullName : string.Empty);
			}
			BingPropertyChanged(d, e);
		}
		static void BingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BingMapDataProvider dataProvider = d as BingMapDataProvider;
			bool inRuntime = !DesignerProperties.GetIsInDesignMode(d);
			if (dataProvider != null && inRuntime)
				dataProvider.UpdateTileSource();
		}
		const string bingRESTuri = "http://dev.virtualearth.net/REST/v1/Imagery/Metadata";
		SphericalMercatorProjection projection = new SphericalMercatorProjection();
		BingImageryServiceInfo serviceInfo = null;
		bool gettingImageryMetadataInProgress = false;
		Size ActualTileSize { get { return new Size(256, 256); } }
		BingMapTileSource BingTileSource { get { return TileSource as BingMapTileSource; } }
		public override ProjectionBase Projection { get { return projection; } }
		protected internal override bool ShouldShowInvalidKeyMessage {
			get {
				return !DesignerProperties.GetIsInDesignMode(this) &&
					(serviceInfo == null || serviceInfo.IsDefaultInfo) &&
					!gettingImageryMetadataInProgress;
			}
		}
		string ActualCulture { get { return string.IsNullOrEmpty(CultureName) ? Thread.CurrentThread.CurrentUICulture.Name : CultureName; } }
		string ActualBingKey { get { return isKeyRestricted ? string.Empty : BingKey; } }
		static BingMapDataProvider() {
			DXBingKeyVerifier.RegisterPlatform("Xpf");
		}
		public BingMapDataProvider() {
			UpdateTileSource();
		}
		BingMapTileSource CreateDefaultTileSource() {
			serviceInfo = new BingImageryServiceInfo((BingMap)Kind, ActualBingKey);
			return new BingMapTileSource(serviceInfo, Thread.CurrentThread.CurrentUICulture.Name);
		}
		void UpdateTileSource() {
			if (!DesignerProperties.GetIsInDesignMode(this) && ActualBingKey != string.Empty)
				RequestImageryData();
			else
				SetTileSource(CreateDefaultTileSource());
		}
		bool CheckServiceInfo(BingImageryServiceInfo serviceInfo) {
			switch (Kind) {
				case BingMapKind.Area:
					return serviceInfo.UrlTemplate.Contains("tiles/a");
				case BingMapKind.Road:
					return serviceInfo.UrlTemplate.Contains("tiles/r");
				case BingMapKind.Hybrid:
					return serviceInfo.UrlTemplate.Contains("tiles/h");
				default:
					goto case BingMapKind.Hybrid;
			}
		}
		string GetImagerySet() {
			switch (Kind) {
				case BingMapKind.Area:
					return "Aerial";
				case BingMapKind.Road:
					return "Road";
				case BingMapKind.Hybrid:
				default:
					return "AerialWithLabels";
			}
		}
		void SubscribeRestEvents(RESTClient client) {
			client.Response += OnImageryClientResponse;
			client.Error += OnImageryClientError;
			client.WebRequest += OnImageryClientWebRequest;
		}
		void UnsubscribeRestEvents(RESTClient client) {
			client.Response -= OnImageryClientResponse;
			client.Error -= OnImageryClientError;
			client.WebRequest -= OnImageryClientWebRequest;
		}
		void RequestImageryData() {
			gettingImageryMetadataInProgress = true;
			BingTileSource.IsProviderInfoValid = false;
			RESTClient client = new RESTClient(WebServiceHelper.CorrectScheme(bingRESTuri));
			client.Parameters.Add(GetImagerySet());
			client.Arguments.Add("key", ActualBingKey);
			client.Arguments.Add("output", "xml");
			client.Arguments.Add("orientation", 45.0);
			client.Arguments.Add("culture", ActualCulture);
			SubscribeRestEvents(client);
			client.BeginRequest();
		}
		void OnImageryClientWebRequest(object sender, MapWebRequestEventArgs e) {
			RaiseWebRequest(e);
		}
		void OnImageryClientError(object sender, AsyncCompletedEventArgs e) {
			gettingImageryMetadataInProgress = false;
			Dispatcher.Invoke((Action)(() => {
				BingTileSource.IsProviderInfoValid = false;
				SetTileSource(CreateDefaultTileSource());
			}));
			UnsubscribeRestEvents(sender as RESTClient);
		}
		void OnImageryClientResponse(object sender, RESTClientResponseEventArgs e) {
			gettingImageryMetadataInProgress = false;
			Dispatcher.Invoke((Action)(() => {
				BingTileSource.IsProviderInfoValid = true;
				try {
					serviceInfo = new BingImageryServiceInfo(e.XDocument, ActualBingKey);
					if (CheckServiceInfo(serviceInfo))
						SetTileSource(new BingMapTileSource(serviceInfo, ActualCulture));
				}
				catch {
					SetTileSource(CreateDefaultTileSource());
				}
			}));
			UnsubscribeRestEvents(sender as RESTClient);
		}
		protected internal override string GetInvalidKeyMessage() {
			return DXMapStrings.MsgIncorrectBingKey;
		}
		protected override MapDependencyObject CreateObject() {
			return new BingMapDataProvider();
		}
		public override Size GetMapSizeInPixels(double zoomLevel) {
			if (zoomLevel < 1.0)
				return new Size(zoomLevel * ActualTileSize.Width * 2, zoomLevel * ActualTileSize.Height * 2);
			return new Size(Math.Pow(2.0, zoomLevel) * ActualTileSize.Width, Math.Pow(2.0, zoomLevel) * ActualTileSize.Height);
		}
	}
	public class BingMapTileSource : MapTileSourceBase {
		readonly BingImageryServiceInfo serviceInfo;
		readonly string cultureName;
		bool isProviderInfoValid = true;
		internal bool IsProviderInfoValid {
			get { return isProviderInfoValid; }
			set { isProviderInfoValid = value; }
		}
		internal BingMapTileSource(BingImageryServiceInfo info, string culture)
			: base(info.ImageWidth, info.ImageHeight, info.TileWidth, info.TileHeight) {
			serviceInfo = info;
			cultureName = string.IsNullOrEmpty(culture) ? Thread.CurrentThread.CurrentUICulture.Name : culture;
		}
		string GetQuadKey(int tileLevel, int tileX, int tileY) {
			StringBuilder quadKey = new StringBuilder();
			for (int i = tileLevel; i > 0; i--) {
				char digit = '0';
				int mask = 1 << (i - 1 & 31);
				if ((tileX & mask) != 0)
					digit += '\u0001';
				if ((tileY & mask) != 0) {
					digit += '\u0001';
					digit += '\u0001';
				}
				quadKey.Append(digit);
			}
			return quadKey.ToString();
		}
		public override Uri GetTileByZoomLevel(int zoomLevel, int tilePositionX, int tilePositionY) {
			if (IsProviderInfoValid) {
				string url = serviceInfo.UrlTemplate;
				string quadKey = GetQuadKey(zoomLevel, tilePositionX, tilePositionY);
				url = WebServiceHelper.CorrectScheme(url);
				url = url.Replace("{token}", serviceInfo.Key);
				url = url.Replace("{culture}", this.cultureName);
				url = url.Replace("{quadkey}", quadKey);
				url = url.Replace("{subdomain}", serviceInfo.Subdomains[GetSubdomainIndex(serviceInfo.Subdomains.Count)]);
				return new Uri(url);
			}
			else
				return null;
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class RESTClientResponseEventArgs : EventArgs {
		readonly XDocument xDocument;
		public XDocument XDocument {
			get {
				return xDocument;
			}
		}
		public RESTClientResponseEventArgs(XDocument xDocument) {
			this.xDocument = xDocument;
		}
	}
	public class RESTClient {
		RESTClientCore restClientCore;
		WebClient webClient;
		public List<object> Parameters { get { return restClientCore.Parameters; } }
		public Dictionary<string, object> Arguments { get { return restClientCore.Arguments; } }
		public event MapWebRequestEventHandler WebRequest;
		public event EventHandler<RESTClientResponseEventArgs> Response;
		public event EventHandler<AsyncCompletedEventArgs> Error;
		public RESTClient(string uri) {
			this.restClientCore = new RESTClientCore(uri);
		}
		void RaiseError(AsyncCompletedEventArgs args) {
			if (Error != null)
				Error(this, args);
		}
		void RaiseResponse(XDocument xDocument) {
			if (Response != null)
				Response(this, new RESTClientResponseEventArgs(xDocument));
		}
		void RaiseWebRequest(Uri uri) {
			if (WebRequest != null)
				WebRequest(this, new MapWebRequestEventArgs(uri, webClient));
		}
		void ParseResponse(string response, AsyncCompletedEventArgs args) {
			XDocument xDocument;
			try {
				xDocument = XDocument.Parse(response);
			}
			catch (Exception) {
				RaiseError(args);
				return;
			}
			RaiseResponse(xDocument);
		}
		void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
			if (e.Error != null) {
				RaiseError(e);
				return;
			}
			Encoding encoding = Encoding.UTF8;
			string result = string.Empty;
			try {
				result = encoding.GetString(e.Result).Trim();
				for (int i = 0; i < result.Length; i++)
					if (result[i] == '<') {
						result = result.Substring(i);
						break;
					}
			}
			catch (Exception) {
				RaiseError(e);
				return;
			}
			ParseResponse(result, e);
		}
		public void BeginRequest() {
			Uri uri = restClientCore.CombineUri();
			webClient = new WebClient();
			RaiseWebRequest(uri);
			webClient.DownloadDataCompleted += OnDownloadDataCompleted;
			webClient.DownloadDataAsync(uri);
		}
		public void AbortRequest() {
			if(webClient != null) {
				webClient.DownloadDataCompleted -= OnDownloadDataCompleted;
				webClient.CancelAsync();
				webClient = null;
			}
		}
	}
}
