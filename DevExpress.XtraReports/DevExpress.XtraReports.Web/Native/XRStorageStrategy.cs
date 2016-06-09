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

using System;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web.Native {
	public class XRStorageStrategy : RuntimeStorageStrategy {
		public override string GetResourceKey(ASPxWebControlBase control, byte[] content, string mimeType) {
			ImageCache imageCache = CreateImageCache(control);
			return imageCache.GetName(content);
		}
		public override void StoreResourceData(ASPxWebControlBase control, string key, BinaryStorageData data) {
			ImageCache imageCache = CreateImageCache(key);
			imageCache.Add(key, (XRBinaryStorageData)data, control);
		}
		public override BinaryStorageData GetResourceData(string key) {
			ImageCache imageCache = CreateImageCache(key);
			return imageCache.Get(key);
		}
		protected override BinaryStorageData CreateBinaryStorageData(byte[] content, string mimeType, string contentDisposition) {
			return new XRBinaryStorageData(content, mimeType, DateTime.UtcNow);
		}
		protected override bool UseClientCache() {
			return true;
		}
		protected override bool UseHttpModuleUrl() {
			return false;
		}
		static bool IsFileCache(object conditionSource) {
			ReportViewer control = conditionSource as ReportViewer;
			return control != null
				? control.FileImageCache
				: ((string)conditionSource).StartsWith(FileImageCache.NamePrefix);
		}
		static ImageCache CreateImageCache(object conditionSource) {
			bool fileCache = IsFileCache(conditionSource);
			return fileCache
				? new FileImageCache()
				: new ImageCache();
		}
#if DEBUGTEST
		internal static ImageCache CreateImageCache_TEST(object conditionSource) {
			return CreateImageCache(conditionSource);
		}
#endif
	}
}
