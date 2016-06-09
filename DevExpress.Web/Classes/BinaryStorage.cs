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
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.UI;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using System.ComponentModel;
	public enum BinaryStorageMode { Default, Cache, Session, Custom };
	[Serializable]
	public class BinaryStorageData {
		Type ownerStrategyType;
		byte[] content;
		string mimeType;
		string contentDisposition;
		public BinaryStorageData(byte[] content, string mimeType) : this(content, mimeType, null) {
		}
		public BinaryStorageData(byte[] content, string mimeType, string contentDisposition) {
			this.content = content;
			this.mimeType = mimeType;
			this.contentDisposition = contentDisposition;
		}
#if !SL
	[DevExpressWebLocalizedDescription("BinaryStorageDataContent")]
#endif
		public virtual byte[] Content {
			get { return content; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("BinaryStorageDataMimeType")]
#endif
		public string MimeType {
			get { return mimeType; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("BinaryStorageDataContentDisposition")]
#endif
		public string ContentDisposition {
			get { return contentDisposition; }
		}
		protected internal Type OwnerStrategyType {
			get { return ownerStrategyType; }
			set { ownerStrategyType = value; }
		}
	}
	public abstract class StorageStrategy {
		public abstract bool CanStoreData(ASPxWebControlBase control);
		[Obsolete("Use the GetResourceUrl(ASPxWebControlBase control, byte[] content, string mimeType, string contentDisposition) method instread.")]
		public virtual string GetResourceUrl(ASPxWebControlBase control, byte[] content, string mimeType) {
			return GetResourceUrl(control, content, mimeType, null);
		}
		public abstract string GetResourceUrl(ASPxWebControlBase control, byte[] content, string mimeType, string contentDisposition);
		protected internal virtual string GetResourceUrlInternal(ASPxWebControlBase control, string key) {
			return string.Empty;
		}
		public virtual BinaryStorageData GetResourceData(string key) {
			return null;
		}
		public virtual void StoreResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
		}
		public virtual string GetResourceKeyForResizedImage(ASPxWebControlBase control, byte[] content, string mimeType) {
			return GetControlUniqueName(control);
		}
		public abstract bool ProcessRequest(string key);
		public virtual bool ProcessRequestForResizedImage(string key) {
			return ProcessRequest(key);
		}
		protected virtual bool UseClientCache() {
			return false;
		}
		protected string GetRootComponentName(Page page) {
			IServiceProvider provider = (page.Site != null) ? (IServiceProvider)page.Site : null;
			if(provider != null) {
				IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null && host.RootComponent != null && host.RootComponent.Site != null)
					return host.RootComponent.Site.Name;
			}
			return page.GetType().FullName;
		}
		public string GetControlUniqueName(ASPxWebControlBase control) {
			if(control.Page != null) {
				string rootComponentName = GetRootComponentName(control.Page);
				if(!string.IsNullOrEmpty(rootComponentName))
					return rootComponentName + "_" + control.ClientID;
			}
			return control.ClientID;
		}
	}
	public abstract class RuntimeStorageStrategy : StorageStrategy {
		public override bool CanStoreData(ASPxWebControlBase control) {
			return true;
		}
		public override string GetResourceUrl(ASPxWebControlBase control, byte[] content, string mimeType, string contentDisposition) {
			string key = GetResourceKey(control, content, mimeType);
			if(!string.IsNullOrEmpty(key)) {
				BinaryStorageData data = CreateBinaryStorageData(content, mimeType, contentDisposition);
				data.OwnerStrategyType = GetType();
				StoreResourceData(control, key, data);
				return GetResourceUrlInternal(control, key);
			}
			return string.Empty;
		}
		public override bool ProcessRequest(string key) {
			return ProcessRequestInternal(key, true);
		}
		protected virtual bool ProcessRequestInternal(string key, bool callPreProcess) {
			BinaryStorageData data = GetResourceData(key);
			if(data != null && (data.OwnerStrategyType == null || data.OwnerStrategyType == GetType())) {
				if(callPreProcess)
					PreProcessRequest(key, data);
				ProcessResponse(data);
				return true;
			}
			return false;
		}
		public virtual string GetResourceKey(ASPxWebControlBase control, byte[] content, string mimeType) {
			return string.Empty;
		}
		protected virtual void PreProcessRequest(string key, BinaryStorageData data) {
		}
		[Obsolete("Use the CreateBinaryStorageData(byte[] content, string mimeType, string contentDisposition) method instreas.")]
		protected virtual BinaryStorageData CreateBinaryStorageData(byte[] content, string mimeType) {
			return CreateBinaryStorageData(content, mimeType, null);
		}
		protected virtual BinaryStorageData CreateBinaryStorageData(byte[] content, string mimeType, string contentDisposition) {
			return new BinaryStorageData(content, mimeType, contentDisposition);
		}
		protected virtual bool NeedRefreshKey() {
			return false;
		}
		protected virtual bool UseHttpModuleUrl() {
			return true;
		}
		protected internal override string GetResourceUrlInternal(ASPxWebControlBase control, string key) {
			string cacheParam = BinaryStorage.CacheParamName + "=" + key;
			if(NeedRefreshKey())
				cacheParam += "&" + BinaryStorage.CacheRefreshParamName + "=" + Guid.NewGuid().ToString();
			NameValueCollection query = HttpUtils.GetQuery(control);
			string url = string.Empty;
			if(UseHttpModuleUrl())
				url += HttpUtils.GetApplicationUrl(control) + BinaryStorage.BinaryDataHandlerName; 
			else
				url += HttpUtils.GetRequest(control).Path;
			url += ((query.Count == 0) ? "?" + cacheParam : "?" + query.ToString() + "&" + cacheParam);
			var binaryImageDisplayControl = control as BinaryImageDisplayControl;
			if(binaryImageDisplayControl != null && binaryImageDisplayControl.CanImageResize())
				url += "&" + BinaryStorage.ImageResizedParamName + "=1";
			return HttpUtils.GetResponse(control).ApplyAppPathModifier(url);
		}
		protected void ProcessResponse(BinaryStorageData data) {
			HttpResponse response = HttpUtils.GetResponse();
			response.Clear();
			response.BinaryWrite(data.Content);
			response.ContentType = data.MimeType;
			if(!string.IsNullOrEmpty(data.ContentDisposition))
				response.AddHeader("Content-Disposition", data.ContentDisposition);
			if(UseClientCache())
				response.Cache.SetExpires(DateTime.Now.AddHours(2.0));
			HttpUtils.EndResponse();
		}
		protected virtual bool IsBinaryEditingMode(ASPxWebControlBase control) {
			var binaryEditor = control as IBinaryImageEditor;
			return binaryEditor != null && binaryEditor.AllowEdit;
		}
	}
	public delegate bool SupportsStrategyMethod(ASPxWebControlBase control);
	public static class BinaryStorageConfigurator {
		public static BinaryStorageMode Mode;
		public static void RegisterCustomStorageStrategy(RuntimeStorageStrategy strategy) {
			BinaryStorage.Strategies.Add(BinaryStorageMode.Custom, strategy);
		}
		public static void RegisterStorageStrategy(RuntimeStorageStrategy strategy, string controlID) {
			BinaryStorage.Strategies.Add(controlID, strategy);
		}
		public static void RegisterStorageStrategy(RuntimeStorageStrategy strategy, SupportsStrategyMethod method) {
			BinaryStorage.Strategies.Add(method, strategy);
		}
	}
}
namespace DevExpress.Web.Internal {
	public static class BinaryStorage {
		public const string BinaryDataHandlerName = "DXB.axd";
		public const string CacheParamName = "DXCache";
		public const string CacheRefreshParamName = "DXRefresh";
		public const string DesignModeStrategyName = "DesignMode";
		public const string ImageResizedParamName = "DXResized";
		private static Dictionary<object, StorageStrategy> strategies = new Dictionary<object, StorageStrategy>();
		static BinaryStorage() {
			Strategies[BinaryStorageMode.Cache] = new CacheStorageStrategy();
			Strategies[BinaryStorageMode.Session] = new SessionStorageStrategy();
		}
		public static Dictionary<object, StorageStrategy> Strategies {
			get { return strategies; }
		}
		public static string GetResourceKeyForResizedImage(ASPxWebControlBase control, byte[] content, BinaryStorageMode mode) {
			StorageStrategy strategy = GetStrategy(control, mode);
			if(strategy != null && strategy.CanStoreData(control))
				return strategy.GetResourceKeyForResizedImage(control, content, string.Empty);
			return string.Empty;
		}
		private static string GetImageUrlForResizedImage(ASPxWebControlBase control, byte[] content, BinaryStorageMode mode) {
			StorageStrategy strategy = GetStrategy(control, mode);
			if(strategy != null && strategy.CanStoreData(control)) {
				BinaryImageDisplayControl displayControl = control as BinaryImageDisplayControl;
				return strategy.GetResourceUrlInternal(control, displayControl.Digest);
			}
			return string.Empty;
		}
		public static string GetImageUrl(ASPxWebControlBase control, byte[] content, BinaryStorageMode mode) {
			return GetResourceUrl(control, content, mode, GetImageMimeType(content));
		}
		public static string GetImageUrl(ASPxWebControlBase control, byte[] content, BinaryStorageMode mode, bool forResizedImage) {
			if(forResizedImage)
				return GetImageUrlForResizedImage(control, content, mode);
			else
				return GetResourceUrl(control, content, mode, GetImageMimeType(content));
		}
		public static string GetResourceUrl(ASPxWebControlBase control, byte[] content, BinaryStorageMode mode, string mimeType) {
			return GetResourceUrl(control, content, mode, mimeType, null);
		}
		public static string GetResourceUrl(ASPxWebControlBase control, byte[] content, BinaryStorageMode mode, string mimeType, string contentDisposition) {
			if(control == null || content == null)
				return string.Empty;
			if(control.DesignMode)
				return Strategies[DesignModeStrategyName].GetResourceUrl(control, content, mimeType, contentDisposition);
			else {
				StorageStrategy strategy = GetStrategy(control, mode);
				if(strategy != null && strategy.CanStoreData(control))
					return strategy.GetResourceUrl(control, content, mimeType, contentDisposition);
			}
			return string.Empty;
		}
		public static string GetResourceUrlInternal(ASPxWebControlBase control, BinaryStorageMode mode, string key) {
			if(control == null || String.IsNullOrEmpty(key))
				return string.Empty;
			StorageStrategy strategy = GetStrategy(control, mode);
			if(strategy != null && strategy.CanStoreData(control))
				return strategy.GetResourceUrlInternal(control, key);
			return string.Empty;
		}
		public static BinaryStorageData GetResourceData(ASPxWebControlBase control, BinaryStorageMode mode, string key) {
			StorageStrategy strategy = GetStrategy(control, mode);
			if(strategy != null && strategy.CanStoreData(control))
				return strategy.GetResourceData(key);
			return null;
		}
		public static void StoreResourceData(ASPxWebControlBase control, BinaryStorageMode mode, string key, BinaryStorageData data) {
			StorageStrategy strategy = GetStrategy(control, mode);
			if(strategy != null && strategy.CanStoreData(control))
				strategy.StoreResourceData(control, key, data);
		}
		private static StorageStrategy GetStrategy(ASPxWebControlBase control, BinaryStorageMode mode) {
			StorageStrategy strategy = GetStrategyByControl(control);
			if(strategy == null)
				strategy = GetStrategyByMethod(control);
			if(strategy == null)
				strategy = GetStrategyByMode(mode);
			return strategy;
		}
		private static StorageStrategy GetStrategyByControl(ASPxWebControlBase control) {
			if(control == null)
				return null;
			if(control.ID != null && Strategies.ContainsKey(control.ID))
				return Strategies[control.ID];
			if(control.UniqueID != null && Strategies.ContainsKey(control.UniqueID))
				return Strategies[control.UniqueID];
			return null;
		}
		private static StorageStrategy GetStrategyByMethod(ASPxWebControlBase control) {
			foreach(object obj in Strategies.Keys) {
				SupportsStrategyMethod method = obj as SupportsStrategyMethod;
				if(method != null && method(control))
					return Strategies[obj];
			}
			return null;
		}
		private static StorageStrategy GetStrategyByMode(BinaryStorageMode mode) {
			mode = GetResolvedMode(mode);
			if(Strategies.ContainsKey(mode))
				return Strategies[mode];
			return null;
		}
		public static bool ProcessRequest() {
			string key = GetRequestedResourceKey();
			if(string.IsNullOrEmpty(key))
				return false;
			foreach(object strategyKey in Strategies.Keys) {
				StorageStrategy strategy = Strategies[strategyKey];
				if(strategy.ProcessRequest(key))
					return true;
			}
			return false;
		}
		public static bool ProcessRequestForResizedImage() {
			string key = GetRequestedResourceKey();
			if(string.IsNullOrEmpty(key))
				return false;
			foreach(object strategyKey in Strategies.Keys) {
				StorageStrategy strategy = Strategies[strategyKey];
				if(strategy.ProcessRequestForResizedImage(key))
					return true;
			}
			return false;
		}
		public static string GetImageFileMimeType(string path) {
			using(var fileStream = File.OpenRead(path)) {
				var imgBytes = new byte[4];
				fileStream.Read(imgBytes, 0, 4);
				return GetImageMimeType(imgBytes);
			}
		}
		public static string GetImageMimeType(byte[] image) {
			if(IsMaskMatch(image, 0, 77, 77) || IsMaskMatch(image, 0, 73, 73))
				return "image/tiff";
			if(IsMaskMatch(image, 1, 80, 78, 71))
				return "image/png";
			if(IsMaskMatch(image, 0, 71, 73, 70, 56))
				return "image/gif";
			if(IsMaskMatch(image, 0, 255, 216))
				return "image/jpeg";
			if(IsMaskMatch(image, 0, 66, 77))
				return "image/bmp";
			return "image";
		}
		private static bool IsMaskMatch(byte[] bytes, int offset, params byte[] mask) {
			if(bytes == null || bytes.Length < mask.Length)
				return false;
			for(int i = 0; i < mask.Length; i++) {
				if(bytes[offset + i] != mask[i])
					return false;
			}
			return true;
		}
		private static string GetRequestedResourceKey() {
			NameValueCollection query = HttpUtils.GetQuery();
			return query != null ? query[CacheParamName] : null;
		}
		private static BinaryStorageMode GetResolvedMode(BinaryStorageMode mode) {
			if(mode != BinaryStorageMode.Default)
				return mode;
			if(BinaryStorageConfigurator.Mode != BinaryStorageMode.Default)
				return BinaryStorageConfigurator.Mode;
			return BinaryStorageMode.Cache;
		}
	}
	public class SessionStorageStrategy : RuntimeStorageStrategy {
		public override bool CanStoreData(ASPxWebControlBase control) {
			return HttpUtils.GetSession(control) != null;
		}
		public override string GetResourceKey(ASPxWebControlBase control, byte[] content, string mimeType) {
			if(IsBinaryEditingMode(control))
				return CommonUtils.GetMD5Hash(content);
			return GetControlUniqueName(control);
		}
		public override string GetResourceKeyForResizedImage(ASPxWebControlBase control, byte[] content, string mimeType) {
		   if(IsBinaryEditingMode(control))
			   return  GetResourceKey(control, content, mimeType);
		   return base.GetResourceKeyForResizedImage(control, content, mimeType);
		}
		public override void StoreResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
			HttpUtils.GetSession(control).Add(key, data);
		}
		public override BinaryStorageData GetResourceData(string key) {
			HttpSessionState session = HttpUtils.GetSession();
			if(session != null)
				return session[key] as BinaryStorageData;
			return null;
		}
		protected override bool UseHttpModuleUrl() {
			return false;
		}
		protected override bool NeedRefreshKey() {
			return true;
		}
	}
	public class CacheStorageStrategy : RuntimeStorageStrategy {
		const string BinaryImageEditModePostfix = "BIEM";
		public const int CacheExpirationTime = 5;
		public const int CacheExpirationTimeForResizedImage = 30;
		public override bool CanStoreData(ASPxWebControlBase control) {
			return HttpUtils.GetCache(control) != null;
		}
		public override string GetResourceKey(ASPxWebControlBase control, byte[] content, string mimeType) {
			if(IsBinaryEditingMode(control))
				return CommonUtils.GetMD5Hash(content) + BinaryImageEditModePostfix;
			return Guid.NewGuid().ToString();
		}
		public override string GetResourceKeyForResizedImage(ASPxWebControlBase control, byte[] content, string mimeType) {
			if(IsBinaryEditingMode(control))
				return GetResourceKey(control, content, mimeType);
			return base.GetResourceKeyForResizedImage(control, content, mimeType);
		}
		public override void StoreResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
			if(IsBinaryEditingMode(ref key)) 
				StoreEditModeResourceData(control, key, data);
			else 
				StoreViewModeResourceData(control, key, data);
		}
		protected void StoreViewModeResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
			var binaryImageDisplayControl = control as BinaryImageDisplayControl;
			var canResize = binaryImageDisplayControl != null && binaryImageDisplayControl.CanImageResize();
			if(canResize) {
				HttpUtils.GetCache(control).Insert(key, data, null, Cache.NoAbsoluteExpiration,TimeSpan.FromMinutes(CacheExpirationTimeForResizedImage), CacheItemPriority.NotRemovable, null);
			} else {
				HttpUtils.GetCache(control).Add(key, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(CacheExpirationTime), CacheItemPriority.NotRemovable, null);
			}
		}
		public override BinaryStorageData GetResourceData(string key) {
			if(IsBinaryEditingMode(ref key))
				return GetEditModeResourceData(key);
			Cache cache = HttpUtils.GetCache();
			if(cache == null) return null;
			return cache[key] as BinaryStorageData;
		}
		protected override void PreProcessRequest(string key, BinaryStorageData data) {
			if(IsBinaryEditingMode(ref key))
				return;
			Cache cache = HttpUtils.GetCache();
			if(cache != null && data != null)
				cache.Remove(key);
		}
		public override bool ProcessRequestForResizedImage(string key) {
			return ProcessRequestInternal(key, false);
		}
		protected override bool UseClientCache() {
			return true;
		}
		static bool IsBinaryEditingMode(ref string key) {
			if(string.IsNullOrEmpty(key) || !key.EndsWith(BinaryImageEditModePostfix))
				return false;
			key = key.Substring(0, key.Length - BinaryImageEditModePostfix.Length);
			return true;
		}
		static void StoreEditModeResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
			var editor = (IBinaryImageEditor) control;
			string tempFolderPath = UrlUtils.ResolvePhysicalPath(editor.TempFolder);
			BinaryImageTempFolderHelper.EnsureTemporaryFolderExists(tempFolderPath);
			string path = GetTempFilePath(tempFolderPath, key);
			var token = new UploadToken(path, editor.TempFileExpirationTime);
			if(token.TrySaveBinaryStorageData(data))
				SaveUploadTokenToCache(key, token);
			BinaryImageTempFolderHelper.SafeRemoveTempFilesHavingNoToken(tempFolderPath);
		}
		static string GetTempFilePath(string tempFolderPath, string key) {
			var filename = BinaryImageTempFolderHelper.TemporaryFileNamePrefix + key + BinaryImageTempFolderHelper.TempFileExtension;
			return Path.Combine(tempFolderPath, filename);
		}
		static void SaveUploadTokenToCache(string key, UploadToken token) {
			HttpUtils.GetCache().Insert(key, token, null, Cache.NoAbsoluteExpiration, token.ExpirationTime, CacheItemPriority.NotRemovable, OnUploadTokenRemove);
		}
		static void OnUploadTokenRemove(string key, object value, CacheItemRemovedReason reason) {
			var uploadToken = value as UploadToken;
			if(uploadToken == null || reason == CacheItemRemovedReason.Removed)
				return;
			uploadToken.DeleteFile();
		}
		static BinaryStorageData GetEditModeResourceData(string key) {
			var cache = HttpUtils.GetCache();
			if(cache == null || string.IsNullOrEmpty(key))
				return null;
			var token = cache[key] as UploadToken;
			if(token == null)
				return null;
			return token.GetBinaryStorageData();
		}
	}
	public interface IBinaryImageEditor {
		string TempFolder { get; }
		int TempFileExpirationTime { get; }
		bool AllowEdit { get; }
	}
}
