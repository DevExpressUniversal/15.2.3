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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Resources {
	static class DockGuideResourceLoader {
		static IDictionary<object, Image> images;
		static IDictionary<object, Image> glyphs;
		static IDictionary<object, Image> hotGlyphs;
		static DockGuideResourceLoader() {
			images = new Dictionary<object, Image>();
			images.Add(DockGuide.Center, LoadImageFromResourcesAndScale("CenterDockGuide"));
			images.Add(DockGuide.CenterDock, LoadImageFromResourcesAndScale("CenterDockGuide.Panel"));
			Image side = LoadImageFromResourcesAndScale("SideDockGuide");
			images.Add(DockGuide.Left, side);
			images.Add(DockGuide.Top, side);
			images.Add(DockGuide.Right, side);
			images.Add(DockGuide.Bottom, side);
			images.Add(dockZone, LoadImageFromResources("Snap"));
			glyphs = new Dictionary<object, Image>();
			glyphs.Add(DockGuide.Center, LoadImageFromResourcesAndScale("CenterDockGuide.Glyph"));
			glyphs.Add(DockGuide.CenterDock, LoadImageFromResourcesAndScale("CenterDockGuide.Panel.Glyph"));
			glyphs.Add(DockGuide.Left, LoadImageFromResourcesAndScale("SideDockGuide.Left.Glyph"));
			glyphs.Add(DockGuide.Top, LoadImageFromResourcesAndScale("SideDockGuide.Top.Glyph"));
			glyphs.Add(DockGuide.Right, LoadImageFromResourcesAndScale("SideDockGuide.Right.Glyph"));
			glyphs.Add(DockGuide.Bottom, LoadImageFromResourcesAndScale("SideDockGuide.Bottom.Glyph"));
			hotGlyphs = new Dictionary<object, Image>();
			hotGlyphs.Add(DockGuide.Center, LoadImageFromResourcesAndScale("CenterDockGuide.Glyph.Hot"));
			hotGlyphs.Add(DockGuide.CenterDock, LoadImageFromResourcesAndScale("CenterDockGuide.Panel.Glyph.Hot"));
			hotGlyphs.Add(DockGuide.Left, LoadImageFromResourcesAndScale("SideDockGuide.Left.Glyph.Hot"));
			hotGlyphs.Add(DockGuide.Top, LoadImageFromResourcesAndScale("SideDockGuide.Top.Glyph.Hot"));
			hotGlyphs.Add(DockGuide.Right, LoadImageFromResourcesAndScale("SideDockGuide.Right.Glyph.Hot"));
			hotGlyphs.Add(DockGuide.Bottom, LoadImageFromResourcesAndScale("SideDockGuide.Bottom.Glyph.Hot"));
		}
		static Image LoadImageFromResourcesAndScale(string name) {
			Image img = LoadImageFromResources(name);
			var factor = DevExpress.Skins.DpiProvider.Default.DpiScaleFactor;
			if(factor != 1.0f) {
				Bitmap bmp = new Bitmap(Round((float)img.Width * factor), Round((float)img.Height * factor), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
				using(Graphics g = Graphics.FromImage(bmp))
					g.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
				img.Dispose();
				img = bmp;
			}
			return img;
		}
		static int Round(float value) {
			return value > 0 ? (int)(value + 0.5f) : (int)(value - 0.5f);
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Docking2010.Resources.{0}.png", name), typeof(DockGuideResourceLoader).Assembly);
		}
		static object dockZone = new object();
		public static Image GetSnapImage() {
			Image img;
			return images.TryGetValue(dockZone, out img) ? img : null;
		}
		public static Image GetImage(DockGuide guide) {
			Image img;
			return images.TryGetValue(guide, out img) ? img : null;
		}
		public static Image GetGlyph(DockGuide guide) {
			Image img;
			return glyphs.TryGetValue(guide, out img) ? img : null;
		}
		public static Image GetHotGlyph(DockGuide guide) {
			Image img;
			return hotGlyphs.TryGetValue(guide, out img) ? img : null;
		}
	}
	static class CommandResourceLoader {
		static IDictionary<object, Image> commands;
		static CommandResourceLoader() {
			commands = new Dictionary<object, Image>();
			commands.Add(DocumentManagerStringId.CommandNewHorizontalDocumentGroup, LoadImageFromResources("NewHorizontalTabGroup"));
			commands.Add(DocumentManagerStringId.CommandNewVerticalDocumentGroup, LoadImageFromResources("NewVerticalTabGroup"));
			commands.Add(DocumentManagerStringId.CommandPinTab, LoadImageFromResources("PinTab"));
			commands.Add(DocumentManagerStringId.CommandUnpinTab, LoadImageFromResources("UnpinTab"));
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Docking2010.Resources.{0}.png", name), typeof(CommandResourceLoader).Assembly);
		}
		public static Image GetCommandGlyph(DocumentManagerStringId command) {
			Image img;
			return commands.TryGetValue(command, out img) ? img : null;
		}
	}
	static class CommonResourceLoader {
		static IDictionary<object, Image> images;
		static CommonResourceLoader() {
			images = new Dictionary<object, Image>();
			images.Add("PreviewMask", LoadImageFromResources("PreviewMask"));
			images.Add("ButtonGlyphs", LoadImageFromResources("WindowsUIButtonGlyphs"));
			images.Add("NavigationPageButtonGlyphs", LoadImageFromResources("NavigationPageButtonsDefaultGlyphs"));
			images.Add("OverviewButtonGlyphs", LoadImageFromResources("OverviewButtonGlyphs"));
			images.Add("NoPreviewAvailable", LoadImageFromResources("DocumentSelectorNoPreviewAvailable"));
			images.Add("SplashScreenLogo", LoadImageFromResources("DevExpressSplashScreenLogo"));
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Docking2010.Resources.{0}.png", name), typeof(CommandResourceLoader).Assembly);
		}
		public static Image GetImage(object key) {
			Image img;
			return images.TryGetValue(key, out img) ? img : null;
		}
	}
	static class ContentContainterActionResourceLoader {
		static IDictionary<object, Image> images;
		static ContentContainterActionResourceLoader() {
			images = new Dictionary<object, Image>();
			images.Add("Default", LoadImageFromResources("Default"));
			images.Add(DocumentManagerStringId.CommandBack, LoadImageFromResources("Back"));
			images.Add(DocumentManagerStringId.CommandHome, LoadImageFromResources("Home"));
			images.Add(DocumentManagerStringId.CommandDetail, LoadImageFromResources("Detail"));
			images.Add(DocumentManagerStringId.CommandOverview, LoadImageFromResources("Overview"));
			images.Add(DocumentManagerStringId.CommandExit, LoadImageFromResources("Exit"));
			images.Add(DocumentManagerStringId.CommandFlip, LoadImageFromResources("Flip"));
			images.Add(DocumentManagerStringId.CommandRotate, LoadImageFromResources("Rotate"));
			images.Add(DocumentManagerStringId.CommandTileSmallSize, LoadImageFromResources("Small"));
			images.Add(DocumentManagerStringId.CommandTileLargeSize, LoadImageFromResources("Large"));
			images.Add(DocumentManagerStringId.CommandTileMediumSize, LoadImageFromResources("Medium"));
			images.Add(DocumentManagerStringId.CommandTileWideSize, LoadImageFromResources("Wide"));
			images.Add(DocumentManagerStringId.CommandTileSizeFlyoutPanel, LoadImageFromResources("TileSize"));
			images.Add(DocumentManagerStringId.CommandToggleTileSize, LoadImageFromResources("ToggleTileSize"));
			images.Add(DocumentManagerStringId.CommandClearSelection, LoadImageFromResources("ClearSelection"));
			images.Add(typeof(Views.WindowsUI.SlideGroup), LoadImageFromResources("SlideGroup"));
			images.Add(typeof(Views.WindowsUI.SplitGroup), LoadImageFromResources("SplitGroup"));
			images.Add(typeof(Views.WindowsUI.PageGroup), LoadImageFromResources("PageGroup"));
			images.Add(typeof(Views.WindowsUI.Page), LoadImageFromResources("Page"));
			images.Add(typeof(Views.WindowsUI.TileContainer), LoadImageFromResources("TileContainer"));
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Docking2010.Resources.WindowsUI.{0}.png", name), typeof(CommandResourceLoader).Assembly);
		}
		public static Image GetImage(object key) {
			Image img;
			return images.TryGetValue(key, out img) ? img : null;
		}
	}
}
