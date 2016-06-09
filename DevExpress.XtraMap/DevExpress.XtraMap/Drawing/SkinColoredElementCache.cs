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
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Skins;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public class LayerColoredSkinElementCache : MapDisposableObject {
		ColoredSkinElementCache customElementCache;
		ColoredSkinElementCache calloutCache;
		protected ColoredSkinElementCache CustomElementCache {
			get {
				if(customElementCache == null)
					customElementCache = new ColoredSkinElementCache();
				return customElementCache;
			}
		}
		protected ColoredSkinElementCache CalloutCache {
			get {
				if(calloutCache == null)
					calloutCache = new ColoredSkinElementCache();
				return calloutCache;
			}
		}
		protected override void DisposeOverride() {
			if(customElementCache != null) customElementCache.Dispose();
			if(calloutCache != null) calloutCache.Dispose();
		}
		public void Reset() {
			if(customElementCache != null) customElementCache.Reset();
			if(calloutCache != null) calloutCache.Reset();
		}
		public SkinElement GetCalloutSkinElement(SkinElement element, Color baseColor, Color color) {
			return CalloutCache.GetSkinElement(element, baseColor, color);
		}
		public SkinElement GetCustomElementSkinElement(SkinElement element, Color baseColor, Color color) {
			return CustomElementCache.GetSkinElement(element, baseColor, color);
		}
	}
	public class ColoredSkinElementCache : MapDisposableObject {
		Dictionary<Int64, SkinElement> elements = new Dictionary<Int64, SkinElement>();
		protected Dictionary<Int64, SkinElement> Elements { get { return elements; } }
		Int64 CalculateColorKey(Int64 baseColor, Int64 color) {
			return (baseColor << 32) | color;
		}
		SkinElement ColorizeElement(SkinElement element, Color baseColor, Color color) {
			return SkinElementColorer.PaintElementWithColor(element, baseColor, color);
		}
		public SkinElement GetSkinElement(SkinElement element, Color baseColor, Color color) {
			Int64 colorKey = CalculateColorKey(Convert.ToInt64(baseColor.ToArgb()), Convert.ToInt64(color.ToArgb()));
			SkinElement result = null;
			if(!Elements.TryGetValue(colorKey, out result)) {
				result = ColorizeElement(element, baseColor, color);
				Elements.Add(colorKey, result);
			}
			return result;
		}
		public void Reset() {
			Elements.Clear();
		}
		protected override void DisposeOverride() {
			if(Elements != null) {
				Elements.Clear();
				elements = null;
			}
		}
	}
}
