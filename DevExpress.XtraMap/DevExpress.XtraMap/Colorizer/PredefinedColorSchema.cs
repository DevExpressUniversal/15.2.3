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
using System.Collections.Generic;
using DevExpress.Skins;
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap {
	public enum PredefinedColorSchema { None, Palette, Gradient, Criteria }
}
namespace DevExpress.XtraMap.Native {
	public static class ColorizerPaletteHelper {
		readonly static ColorCollection defaultPalette = GetDefaultPalette();
		readonly static ColorCollection defaultGradient = GetDefaultGradient();
		readonly static ColorCollection defaultCriteria = GetDefaultCriteria();
		static ColorCollection GetDefaultPalette() {
			ColorCollection colors = new ColorCollection();
			colors.Add(Color.FromArgb(255, 93, 106));
			colors.Add(Color.FromArgb(65, 124, 217));
			colors.Add(Color.FromArgb(255, 221, 116));
			colors.Add(Color.FromArgb(103, 191, 88));
			colors.Add(Color.FromArgb(140, 104, 195));
			colors.Add(Color.FromArgb(54, 170, 206));
			colors.Add(Color.FromArgb(255, 142, 96));
			colors.Add(Color.FromArgb(25, 204, 127));
			colors.Add(Color.FromArgb(226, 110, 74));
			colors.Add(Color.FromArgb(25, 138, 184));
			return colors;
		}
		static ColorCollection GetDefaultGradient() {
			return new ColorCollection() { Color.FromArgb(54, 170, 206), Color.FromArgb(255, 93, 106) };
		}
		static ColorCollection GetDefaultCriteria() {
			return new ColorCollection() { Color.FromArgb(245, 97, 95), Color.FromArgb(255, 214, 91), Color.FromArgb(47, 168, 113) };
		}
		static ColorCollection GetSkinPalette(ISkinProvider provider) {
			return new ColorCollection(){ SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette01),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette02),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette03),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette04),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette05),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette06),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette07),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette08),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette09),
										  SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorPalette10) };
		}
		static ColorCollection GetSkinGradient(ISkinProvider provider) {
			return new ColorCollection(){ SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorGradient01),
										   SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorGradient02) };
		}
		static ColorCollection GetSkinCriteria(ISkinProvider provider) {
			return new ColorCollection(){ SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorCriteria01),
										   SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorCriteria02), 
										   SkinPainterHelper.GetPredefinedColorizerColor(provider, MapSkins.ColorCriteria03) };
		}
		public static ColorCollection GetPredefinedColors(PredefinedColorSchema colors, ISkinProvider provider) {
			if(colors == PredefinedColorSchema.None)
				return new ColorCollection();
			switch(colors) {
				case PredefinedColorSchema.Palette:
					return GetPaletteColors(provider);
				case PredefinedColorSchema.Gradient:
					return GetGradientColors(provider);
				case PredefinedColorSchema.Criteria:
					return GetCriteriaColors(provider);
			}
			return new ColorCollection();
		}
		public static ColorCollection GetPaletteColors(ISkinProvider provider) {
			if(provider == null || string.IsNullOrEmpty(provider.SkinName))
				return defaultPalette;
			return GetSkinPalette(provider);
		}
		public static ColorCollection GetGradientColors(ISkinProvider provider) {
			if(provider == null || string.IsNullOrEmpty(provider.SkinName))
				return defaultGradient;
			return GetSkinGradient(provider);
		}
		public static ColorCollection GetCriteriaColors(ISkinProvider provider) {
			if(provider == null || string.IsNullOrEmpty(provider.SkinName))
				return defaultCriteria;
			return GetSkinCriteria(provider);
		}
	}
}
