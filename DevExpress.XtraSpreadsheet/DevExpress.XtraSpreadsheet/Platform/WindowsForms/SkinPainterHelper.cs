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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SkinPaintHelper
	public static class SkinPaintHelper {
		public static SkinElement GetSkinElement(ISkinProvider provider, string elementName) {
			Skin skin = GridSkins.GetSkin(provider);
			return GetSkinElement(skin, elementName);
		}
		public static SkinElement GetSkinElement(Skin skin, string elementName) {
			return skin != null ? skin[elementName] : null;
		}
		public static SkinElementInfo GetSkinElementInfo(ISkinProvider skinProvider, string elementName, Rectangle bounds) {
			return new SkinElementInfo(GetSkinElement(skinProvider, elementName), bounds);
		}
		public static SkinElementInfo GetSkinElementInfo(Skin skin, string elementName, Rectangle bounds) {
			return new SkinElementInfo(GetSkinElement(skin, elementName), bounds);
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetSkinElementInfo(lookAndFeel, elementName, bounds));
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex) {
			DrawSkinElement(lookAndFeel, cache, elementName, bounds, imageIndex, -1);
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex, int glyphIndex) {
			SkinElementInfo skinElInfo = GetSkinElementInfo(lookAndFeel, elementName, bounds);
			skinElInfo.ImageIndex = imageIndex;
			skinElInfo.GlyphIndex = glyphIndex;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinElInfo);
		}
		public static void DrawSkinElement(Skin skin, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex, int glyphIndex) {
			SkinElementInfo skinElInfo = GetSkinElementInfo(skin, elementName, bounds);
			skinElInfo.ImageIndex = imageIndex;
			skinElInfo.GlyphIndex = glyphIndex;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinElInfo);
		}
		public static void DrawSkinElement(Skin skin, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex) {
			DrawSkinElement(skin, cache, elementName, bounds, imageIndex, -1);
		}
		public static SkinPaddingEdges GetSkinEdges(UserLookAndFeel lookAndFeel, string skinElementName) {
			SkinElement skinEl = GetSkinElement(lookAndFeel, skinElementName);
			return GetSkinEdges(lookAndFeel, skinEl);
		}
		public static SkinPaddingEdges GetSkinEdges(UserLookAndFeel lookAndFeel, SkinElement skinEl) {
			if (skinEl == null)
				return new SkinPaddingEdges();
			else
				if (skinEl.Image == null)
					return new SkinPaddingEdges();
				else
					return skinEl.Image.SizingMargins;
		}
	}
	#endregion
}
