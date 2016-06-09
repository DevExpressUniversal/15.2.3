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

using DevExpress.Map.BingServices;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
namespace DevExpress.XtraMap {
	public enum BingMapKind {
		Road = BingMap.Road,
		Area = BingMap.Area,
		Hybrid = BingMap.Hybrid
	}
	public class BingMapDataProvider : MapDataProviderBase, IBingMapKindProvider {
		const BingMapKind DefaultKind = BingMapKind.Hybrid;
		const int MaxTryConnectCount = 5;
		readonly string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
		BingMapKind kind = DefaultKind;
		string bingKey = String.Empty;
		SphericalMercatorProjection projection = new SphericalMercatorProjection();
		BingImageryServiceInfo serviceInfo = null;
		bool gettingImageryMetadataInProgress = false;
		RESTClient restClient;
		Nullable<bool> shouldShowInvalidKeyMessage = null;
		bool isKeyRestricted = false;
		bool connectionAttemptsExpired = false;
		string cultureName = string.Empty;
		int TryConnectCount { get; set; }
		MapSize ActualTileSize { get { return new MapSize(256, 256); } }
		BingMapTileSource BingTileSource { get { return TileSource as BingMapTileSource; } }
		RESTClient RestClient {
			get { return restClient; }
			set {
				if(restClient == value)
					return;
				if(restClient != null)
					UnsubscribeRestEvents(restClient);
				restClient = value;
				if(restClient != null)
					SubscribeRestEvents(restClient);
			}
		}
		string ActualCulture { get { return string.IsNullOrEmpty(CultureName) ? this.currentCulture : CultureName; } }
		string ActualBingKey { get { return isKeyRestricted ? string.Empty : BingKey; } }
		protected internal override Size BaseSizeInPixels {
			get { return new Size(Convert.ToInt32(ActualTileSize.Width * 2), Convert.ToInt32(ActualTileSize.Height * 2)); }
		}
		protected internal override bool ShouldShowInvalidKeyMessage {
			get {
				if(!IsUpdateEnabled && serviceInfo.IsDefaultInfo && string.IsNullOrEmpty(ActualBingKey) || connectionAttemptsExpired)
					return true;
				return shouldShowInvalidKeyMessage.HasValue ? shouldShowInvalidKeyMessage.Value : false;
			}
		}
		protected internal BingImageryServiceInfo ServiceInfo { get { return serviceInfo; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("BingMapDataProviderProjection")]
#endif
		public override ProjectionBase Projection { get { return projection; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingMapDataProviderKind"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultKind)]
		public BingMapKind Kind {
			get { return kind; }
			set {
				if (value != kind) {
					kind = value;
					Update();
				}
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingMapDataProviderBingKey"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue("")]
		public string BingKey {
			get { return bingKey; }
			set {
				if (bingKey != value) {
					bingKey = value;
					UpdateBingKey();
				}
			}
		}
		[Category(SRCategoryNames.Behavior), DefaultValue("")]
		public string CultureName {
			get { return cultureName; }
			set {
				if (value != cultureName) {
					cultureName = value;
					Update();
				}
			}
		}
		static BingMapDataProvider() {
			DXBingKeyVerifier.RegisterPlatform("Win");
		}
		public BingMapDataProvider() {
			TileSource = CreateDefaultTileSource();
			ShouldUpdateTileSource = true;
		}
		protected override void DisposeOverride() {
			this.projection = null;
			RestClient = null;
			base.DisposeOverride();
		}
		BingMapTileSource CreateDefaultTileSource() {
			serviceInfo = new BingImageryServiceInfo((BingMap)Kind, ActualBingKey);
			return new BingMapTileSource(this, serviceInfo, this.currentCulture, this);
		}
		bool CheckServiceInfo(BingImageryServiceInfo serviceInfo) {
			switch(Kind) {
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
			switch(Kind) {
				case BingMapKind.Area:
					return "Aerial";
				case BingMapKind.Road:
					return "Road";
				case BingMapKind.Hybrid:
				default:
					return "AerialWithLabels";
			}
		}
		protected internal override void PrepareData() {
			if(CanUpdateTileSource()) {
				UpdateTileSource();
				ShouldUpdateTileSource = false;
			}
		}
		void UpdateTileSource() {
			RequestImageryData();
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
			connectionAttemptsExpired = false;
			gettingImageryMetadataInProgress = true;
			UpdateBingTileSOurceInfoValid(false);
			IUIThreadRunner runner = Layer != null ? Layer.View as IUIThreadRunner : null;
			RestClient = new RESTClient("http://dev.virtualearth.net/REST/v1/Imagery/Metadata", runner);
			RestClient.Parameters.Add(GetImagerySet());
			RestClient.Arguments.Add("key", ActualBingKey);
			RestClient.Arguments.Add("output", "xml");
			RestClient.Arguments.Add("orientation", 45.0);
			RestClient.Arguments.Add("culture", ActualCulture);
			RestClient.BeginRequest();
		}
		void OnImageryClientWebRequest(object sender, MapWebRequestEventArgs e) {
			RaiseWebRequest(e);
		}
		void OnIncorrectResponse() {
			if(TryConnectCount < MaxTryConnectCount) {
				RequestImageryData();
				TryConnectCount++;
			} else {
				connectionAttemptsExpired = true;
				UpdateShouldShowInvalidKeyMessage(true);
				TileSource = CreateDefaultTileSource();
			}
		}
		void UpdateBingKey() {
			Assembly asm = Assembly.GetEntryAssembly();
			this.isKeyRestricted = DXBingKeyVerifier.IsKeyRestricted(BingKey, asm != null ? asm.FullName : string.Empty);
			Update();
		}
		void Update() {
			ShouldUpdateTileSource = true;
			shouldShowInvalidKeyMessage = null;
			Invalidate();
		}
		void OnImageryClientError(object sender, UnhandledExceptionEventArgs e) {
			gettingImageryMetadataInProgress = false;
			UpdateBingTileSOurceInfoValid (false);
			UpdateShouldShowInvalidKeyMessage(true);
			UnsubscribeRestEvents(sender as RESTClient);
			OnIncorrectResponse();
		}
		void OnImageryClientResponse(object sender, RESTClientResponseEventArgs e) {
			gettingImageryMetadataInProgress = false;
			UnsubscribeRestEvents(sender as RESTClient);
			try {
				serviceInfo = new BingImageryServiceInfo(e.XDocument, ActualBingKey);
				if(CheckServiceInfo(serviceInfo)) {
					TileSource = new BingMapTileSource(this, serviceInfo, ActualCulture, this);
					UpdateBingTileSOurceInfoValid(true);
					UpdateShouldShowInvalidKeyMessage(false);
				}
			} catch {
				OnIncorrectResponse();
			}
		}
		void UpdateBingTileSOurceInfoValid(bool value) {
			if(BingTileSource != null)
				BingTileSource.IsProviderInfoValid = value;
		}
		void UpdateShouldShowInvalidKeyMessage(bool value) {
			if(IsUpdateEnabled || gettingImageryMetadataInProgress)
				return;
			this.shouldShowInvalidKeyMessage = value;
		}
		public override MapSize GetMapSizeInPixels(double zoomLevel) {
			if(zoomLevel < 1.0)
				return new MapSize(zoomLevel * ActualTileSize.Width * 2, zoomLevel * ActualTileSize.Height * 2);
			return new MapSize(Math.Pow(2.0, zoomLevel) * ActualTileSize.Width, Math.Pow(2.0, zoomLevel) * ActualTileSize.Height);
		}
		public override string ToString() {
			return "(BingMapDataProvider)";
		}
	}
	internal interface IBingMapKindProvider {
		BingMapKind Kind { get; }
	}
	internal class BingMapTileSource : MapTileSourceBase {
		readonly IBingMapKindProvider bingMapKindProvider;
		readonly BingImageryServiceInfo serviceInfo;
		readonly string cultureName;
		bool isProviderInfoValid = true;
		internal bool IsProviderInfoValid {
			get { return isProviderInfoValid; }
			set { isProviderInfoValid = value; }
		}
		protected internal override string Referer { get { return @"http://www.bing.com/maps/"; } }
		protected internal override string TilePrefix {
			get {
				string prefix = BingImageryServiceInfo.GetMapKindModificator((BingMap)bingMapKindProvider.Kind);
				return "bing_" + prefix;
			}
		}
		internal BingMapTileSource(IBingMapKindProvider bingMapKindProvider, BingImageryServiceInfo info, string culture, ICacheOptionsProvider cacheOptionsProvider)
			: base(info.ImageWidth, info.ImageHeight, info.TileWidth, info.TileHeight, cacheOptionsProvider) {
			this.bingMapKindProvider = bingMapKindProvider;
			serviceInfo = info;
			this.cultureName = string.IsNullOrEmpty(culture) ? Thread.CurrentThread.CurrentUICulture.Name : culture; 
		}
		string GetQuadKey(int tileLevel, int tileX, int tileY) {
			StringBuilder quadKey = new StringBuilder();
			for(int i = tileLevel; i > 0; i--) {
				char digit = '0';
				int mask = 1 << (i - 1 & 31);
				if((tileX & mask) != 0)
					digit += '\u0001';
				if((tileY & mask) != 0) {
					digit += '\u0001';
					digit += '\u0001';
				}
				quadKey.Append(digit);
			}
			return quadKey.ToString();
		}
		public override Uri GetTileByZoomLevel(int zoomLevel, int tilePositionX, int tilePositionY) {
			if(IsProviderInfoValid) {
				string url = serviceInfo.UrlTemplate;
				string quadKey = GetQuadKey(zoomLevel, tilePositionX, tilePositionY);
				url = WebServiceHelper.CorrectScheme(url);
				url = url.Replace("{token}", serviceInfo.Key);
				url = url.Replace("{culture}", this.cultureName);
				url = url.Replace("{quadkey}", quadKey);
				url = url.Replace("{subdomain}", serviceInfo.Subdomains[GetSubdomainIndex(serviceInfo.Subdomains.Count)]);
				return new Uri(url);
			} else
				return null;
		}
	}
}
namespace DevExpress.XtraMap.Native {
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
		readonly RESTClientCore restClientCore;
		readonly IUIThreadRunner uiRunner;
		HttpWebRequest webRequest;
		public List<object> Parameters { get { return restClientCore.Parameters; } }
		public Dictionary<string, object> Arguments { get { return restClientCore.Arguments; } }
		public event MapWebRequestEventHandler WebRequest;
		public event EventHandler<RESTClientResponseEventArgs> Response;
		public event EventHandler<UnhandledExceptionEventArgs> Error;
		public RESTClient(string uri, IUIThreadRunner uiRunner) {
			this.restClientCore = new RESTClientCore(uri);
			this.uiRunner = uiRunner;
		}
		void RaiseError(Exception ex) {
			if(this.uiRunner == null) return;
			this.uiRunner.BeginInvoke(() => {
				if(Error != null)
					Error(this, new UnhandledExceptionEventArgs(ex, false));
			});
		}
		void RaiseResponse(XDocument xDocument) {
			if(this.uiRunner == null) return;
			this.uiRunner.BeginInvoke(() => {
				if(Response != null)
					Response(this, new RESTClientResponseEventArgs(xDocument));
			});
		}
		void RaiseWebRequest(Uri uri) {
			if(WebRequest != null)
				WebRequest(this, new MapWebRequestEventArgs(uri, webRequest));
		}
		void ParseResponse(string response) {
			XDocument xDocument;
			try {
				xDocument = XDocument.Parse(response);
			} catch(Exception ex) {
				RaiseError(ex);
				return;
			}
			RaiseResponse(xDocument);
		}
		void BeginDownload(object target) {
			HttpWebResponse response = null;
			Stream stream = null;
			string result = string.Empty;
			try {
				response = (HttpWebResponse)webRequest.GetResponse();
				StreamReader reader = new StreamReader(response.GetResponseStream());
				result = reader.ReadToEnd().Trim();
				for(int i = 0; i < result.Length; i++)
					if(result[i] == '<') {
						result = result.Substring(i);
						break;
					}
			} catch(Exception ex) {
				RaiseError(ex);
				return;
			} finally {
				if(stream != null) 
					stream.Close();
				if(response != null)
					response.Close();
			}
			ParseResponse(result);
		}
		public void BeginRequest() {
			Uri uri = restClientCore.CombineUri();
			webRequest = HttpWebRequest.Create(uri) as HttpWebRequest;
			RaiseWebRequest(uri);
			ThreadPool.QueueUserWorkItem(new WaitCallback(BeginDownload));
		}
	}
}
