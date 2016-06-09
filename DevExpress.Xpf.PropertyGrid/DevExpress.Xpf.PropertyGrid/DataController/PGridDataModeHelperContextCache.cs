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

using System.Collections.Generic;
#if SL || Wpf
using DevExpress.Xpf.PropertyGrid;
#endif
namespace DevExpress.XtraVerticalGrid.Data {
	public class PGridDataModeHelperContextCache {
		Dictionary<object, DescriptorContext> singleSourceCache;
		Dictionary<object, DescriptorContext> multiSourceCache;
		internal protected Dictionary<object, DescriptorContext> SingleSourceCache {
			get {
				if(singleSourceCache == null)
					singleSourceCache = new Dictionary<object, DescriptorContext>();
				return singleSourceCache;
			}
		}
		internal protected Dictionary<object, DescriptorContext> MultiSourceCache {
			get {
				if(multiSourceCache == null)
					multiSourceCache = new Dictionary<object, DescriptorContext>();
				return multiSourceCache;
			}
		}
		protected Dictionary<object, DescriptorContext> GetCache(bool isMultiSource) {
			return isMultiSource ? MultiSourceCache : SingleSourceCache;
		}
		public DescriptorContext this[bool isMultiSource, object propName] {
			get {
				Dictionary<object, DescriptorContext> cache = GetCache(isMultiSource);
				DescriptorContext context;
				if(!cache.TryGetValue(propName, out context)) {
					return null;
				}
				return context;
			}
			set {
				Dictionary<object, DescriptorContext> cache = GetCache(isMultiSource);
				cache.Remove(propName);
				cache.Add(propName, value);
			}
		}
		public void Clear() {
			singleSourceCache = null;
			multiSourceCache = null;
		}
		public void Clear(bool isMultiSource) {
			if(isMultiSource) {
				multiSourceCache = null;
			} else {
				singleSourceCache = null;
			}
		}
	}
}
