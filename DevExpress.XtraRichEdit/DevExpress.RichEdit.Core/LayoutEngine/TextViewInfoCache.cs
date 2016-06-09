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
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region TextViewInfo
	public class TextViewInfo : IDisposable {
		Size size;
		public Size Size { get { return size; } set { size = value; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
		}
		public virtual void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region TextViewInfoCache
	public class TextViewInfoCache : IDisposable {
		#region CacheDictionary
		class CacheDictionary : Dictionary<FontInfo, TextViewInfoCacheEntry> {
		}
		#endregion
		CacheDictionary entries;
		public TextViewInfoCache() {
			this.entries = new CacheDictionary();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (entries != null) {
				foreach (TextViewInfoCacheEntry entry in entries.Values)
					entry.Dispose();
				entries = null;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public TextViewInfo TryGetTextViewInfo(string text, FontInfo fontInfo) {
			TextViewInfoCacheEntry entry;
			if (!entries.TryGetValue(fontInfo, out entry)) {
				return null;
			}
			return entry.TryGetTextViewInfo(text);
		}
		public void AddTextViewInfo(string text, FontInfo fontInfo, TextViewInfo textInfo) {
			TextViewInfoCacheEntry entry;
			if (!entries.TryGetValue(fontInfo, out entry)) {
				entry = new TextViewInfoCacheEntry();
				entries.Add(fontInfo, entry);
			}
			entry.AddTextViewInfo(text, textInfo);
		}
	}
	#endregion
	#region TextViewInfoCacheEntry
	public class TextViewInfoCacheEntry {
		#region CacheDictionary
		class CacheDictionary : Dictionary<string, TextViewInfo> {
		}
		#endregion
		CacheDictionary textViewInfoCache;
		public TextViewInfoCacheEntry() {
			textViewInfoCache = new CacheDictionary();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (textViewInfoCache != null) {
				foreach (TextViewInfo textViewInfo in textViewInfoCache.Values)
					textViewInfo.Dispose();
				textViewInfoCache = null;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public TextViewInfo TryGetTextViewInfo(string text) {
			TextViewInfo result;
			textViewInfoCache.TryGetValue(text, out result);
			return result;
		}
		public void AddTextViewInfo(string text, TextViewInfo textInfo) {
			textViewInfoCache.Add(text, textInfo);
		}
	}
	#endregion
}
