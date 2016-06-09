#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Web.Caching;
using System.Web.SessionState;
using DevExpress.Utils.Zip;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.XtraReports.Web.Native {
	public class ImageCache {
		const string SessionKeyPrefix = "XtraReportsImageCache_";
		internal static void Clean(ASPxWebControlBase control) {
			IImageCacheLogic logic = CreateLogic(control);
			if(logic != null && logic.CanClean) {
				logic.Clean(SessionKeyPrefix, XRFileStore.KeepInterval);
			}
		}
		public virtual void Add(string key, XRBinaryStorageData data, ASPxWebControlBase control) {
			IImageCacheLogic logic = CreateLogic(control);
			if(logic == null)
				return;
			data = logic.Get(key) ?? data;
			logic.Save(GetSessionKey(key), data, XRFileStore.KeepInterval);
		}
		public virtual BinaryStorageData Get(string key) {
			IImageCacheLogic logic = CreateLogic();
			if(logic == null)
				return null;
			return logic.Get(GetSessionKey(key));
		}
		public virtual string GetName(byte[] content) {
			return "img" + Adler32.CalculateChecksum(content);
		}
		public virtual void Remove(string key) {
			IImageCacheLogic logic = CreateLogic();
			if(logic != null) {
				logic.Remove(GetSessionKey(key));
			}
		}
		string GetSessionKey(string key) {
			return SessionKeyPrefix + key;
		}
		static IImageCacheLogic CreateLogic() {
			return CreateLogic(null);
		}
		static IImageCacheLogic CreateLogic(ASPxWebControlBase control) {
			HttpSessionState session = control != null
				? HttpUtils.GetSession(control)
				: HttpUtils.GetSession();
			if(session != null)
				return new SessionImageCacheLogic(CreateSessionProxy(session));
			Cache cache = control != null
				? HttpUtils.GetCache(control)
				: HttpUtils.GetCache();
			if(cache != null)
				return new CacheImageCacheLogic(cache);
			return null;
		}
		static IHttpSessionProxy CreateSessionProxy(System.Web.SessionState.HttpSessionState session) {
			return new HttpSessionStateProxy(session);
		}
	}
}
