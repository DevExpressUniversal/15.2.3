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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors {
	public class FormatConditionIconSetPositiveNegativeTriangles : FormatConditionIconSetPredefined {
		public FormatConditionIconSetPositiveNegativeTriangles() : base(true, "PositiveNegativeTriangles") { }
		protected override void PopulateCore() {
			CategoryName = FormatPredefinedIconSets.CategoryPositiveNegative;
			ValueType = FormatConditionValueType.Number;
			Title = Localizer.Active.GetLocalizedString(StringId.IconSetTitlePositiveNegativeTriangles);
			Icons.AddRange(
				new FormatConditionIconSetIcon[] {
						new FormatConditionIconSetIcon() { Value = Decimal.MinValue, ValueComparison= FormatConditionComparisonType.GreaterOrEqual, PredefinedName = "Triangles3_3.png" },
						new FormatConditionIconSetIcon() { Value = 0, ValueComparison= FormatConditionComparisonType.GreaterOrEqual, PredefinedName = "Triangles3_2.png" },
						new FormatConditionIconSetIcon() { Value = 0, ValueComparison= FormatConditionComparisonType.Greater, PredefinedName = "Triangles3_1.png" }
					});
			base.PopulateCore();
		}
	}
	public class FormatConditionIconSetPositiveNegativeArrows : FormatConditionIconSetPredefined {
		public FormatConditionIconSetPositiveNegativeArrows() : base(true, "PositiveNegativeArrows") { }
		protected override void PopulateCore() {
			CategoryName = FormatPredefinedIconSets.CategoryPositiveNegative;
			ValueType = FormatConditionValueType.Number;
			Title = Localizer.Active.GetLocalizedString(StringId.IconSetTitlePositiveNegativeArrows);
			Icons.AddRange(
				new FormatConditionIconSetIcon[] {
						new FormatConditionIconSetIcon() { Value = Decimal.MinValue, ValueComparison= FormatConditionComparisonType.GreaterOrEqual, PredefinedName = "Arrows3_3.png" },
						new FormatConditionIconSetIcon() { Value = 0, ValueComparison= FormatConditionComparisonType.GreaterOrEqual, PredefinedName = "Arrows3_2.png" },
						new FormatConditionIconSetIcon() { Value = 0, ValueComparison= FormatConditionComparisonType.Greater, PredefinedName = "Arrows3_1.png" }
					});
			base.PopulateCore();
		}
	}
	public class FormatConditionIconSetPositiveNegativeArrowsGray : FormatConditionIconSetPredefined {
		public FormatConditionIconSetPositiveNegativeArrowsGray() : base(true, "PositiveNegativeArrowsGray") { }
		protected override void PopulateCore() {
			ValueType = FormatConditionValueType.Number;
			CategoryName = FormatPredefinedIconSets.CategoryPositiveNegative;
			Title = Localizer.Active.GetLocalizedString(StringId.IconSetTitlePositiveNegativeArrowsGray);
			Icons.AddRange(
				new FormatConditionIconSetIcon[] {
						new FormatConditionIconSetIcon() { Value = Decimal.MinValue, ValueComparison= FormatConditionComparisonType.GreaterOrEqual, PredefinedName = "ArrowsGray3_3.png" },
						new FormatConditionIconSetIcon() { Value = 0, ValueComparison= FormatConditionComparisonType.GreaterOrEqual, PredefinedName = "ArrowsGray3_2.png" },
						new FormatConditionIconSetIcon() { Value = 0, ValueComparison= FormatConditionComparisonType.Greater, PredefinedName = "ArrowsGray3_1.png" }
					});
			base.PopulateCore();
		}
	}
	public class FormatPredefinedIconSets : IEnumerable, IEnumerable<FormatConditionIconSet> {
		public static string CategoryRatings { get { return Localizer.Active.GetLocalizedString(StringId.IconSetCategoryRatings); } }
		public static string CategoryIndicators { get { return Localizer.Active.GetLocalizedString(StringId.IconSetCategoryIndicators); } }
		public static string CategorySymbols { get { return Localizer.Active.GetLocalizedString(StringId.IconSetCategorySymbols); } }
		public static string CategoryShapes { get { return Localizer.Active.GetLocalizedString(StringId.IconSetCategoryShapes); } }
		public static string CategoryDirectional { get { return Localizer.Active.GetLocalizedString(StringId.IconSetCategoryDirectional); } }
		public static string CategoryPositiveNegative { get { return Localizer.Active.GetLocalizedString(StringId.IconSetCategoryPositiveNegative); } }
		[ThreadStatic]
		static FormatPredefinedIconSets _default;
		public static FormatPredefinedIconSets Default {
			get {
				if(_default == null) _default = new FormatPredefinedIconSets();
				return _default;
			}
		}
		Dictionary<string, FormatConditionIconSet> sets;
		public FormatPredefinedIconSets() { }
		void RegisterSet(Dictionary<string, FormatConditionIconSet> sets, string setName, int imageCount, string imageFormat, string categoryName, string title) {
			var set = new FormatConditionIconSetPredefinedQuick(imageCount, imageFormat, categoryName, setName, title);
			sets[setName] = set;
		}
		void RegisterSet(Dictionary<string, FormatConditionIconSet> sets, FormatConditionIconSetPredefined set) {
			sets[set.Name] = set;
		}
		protected virtual void Register(Dictionary<string, FormatConditionIconSet> sets) {
			RegisterSet(sets, "Stars3", 3, "Stars3_{0}.png", FormatPredefinedIconSets.CategoryRatings, Localizer.Active.GetLocalizedString(StringId.IconSetTitleStars3));
			RegisterSet(sets, "Ratings4", 4, "Rating4_{0}.png", FormatPredefinedIconSets.CategoryRatings, Localizer.Active.GetLocalizedString(StringId.IconSetTitleRatings4));
			RegisterSet(sets, "Ratings5", 5, "Rating5_{0}.png", FormatPredefinedIconSets.CategoryRatings, Localizer.Active.GetLocalizedString(StringId.IconSetTitleRatings5));
			RegisterSet(sets, "Quarters5", 5, "Quarters5_{0}.png", FormatPredefinedIconSets.CategoryRatings, Localizer.Active.GetLocalizedString(StringId.IconSetTitleQuarters5));
			RegisterSet(sets, "Boxes5", 5, "Boxes5_{0}.png", FormatPredefinedIconSets.CategoryRatings, Localizer.Active.GetLocalizedString(StringId.IconSetTitleBoxes5));
			RegisterSet(sets, "Arrows3Colored", 3, "Arrows3_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleArrows3Colored)); 
			RegisterSet(sets, "Arrows3Gray", 3, "ArrowsGray3_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleArrows3Gray)); 
			RegisterSet(sets, "Triangles3", 3, "Triangles3_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleTriangles3)); 
			RegisterSet(sets, "Arrows4Colored", 4, "Arrows4_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleArrows4Colored)); 
			RegisterSet(sets, "Arrows4Gray", 4, "ArrowsGray4_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleArrows4Gray)); 
			RegisterSet(sets, "Arrows5Colored", 5, "Arrows5_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleArrows5Colored)); 
			RegisterSet(sets, "Arrows5Gray", 5, "ArrowsGray5_{0}.png", FormatPredefinedIconSets.CategoryDirectional, Localizer.Active.GetLocalizedString(StringId.IconSetTitleArrows5Gray)); 
			RegisterSet(sets, "TrafficLights3Rimmed", 3, "TrafficLights3_{0}.png", FormatPredefinedIconSets.CategoryShapes, Localizer.Active.GetLocalizedString(StringId.IconSetTitleTrafficLights3Rimmed)); 
			RegisterSet(sets, "TrafficLights3Unrimmed", 3, "TrafficLights23_{0}.png", FormatPredefinedIconSets.CategoryShapes, Localizer.Active.GetLocalizedString(StringId.IconSetTitleTrafficLights3Unrimmed)); 
			RegisterSet(sets, "Signs3", 3, "Signs3_{0}.png", FormatPredefinedIconSets.CategoryShapes, Localizer.Active.GetLocalizedString(StringId.IconSetTitleSigns3)); 
			RegisterSet(sets, "TrafficLights4", 4, "TrafficLights4_{0}.png", FormatPredefinedIconSets.CategoryShapes, Localizer.Active.GetLocalizedString(StringId.IconSetTitleTrafficLights4)); 
			RegisterSet(sets, "RedToBlack", 4, "RedToBlack4_{0}.png", FormatPredefinedIconSets.CategoryShapes, Localizer.Active.GetLocalizedString(StringId.IconSetTitleRedToBlack)); 
			RegisterSet(sets, "Symbols3Uncircled", 3, "Symbols3_{0}.png", FormatPredefinedIconSets.CategorySymbols, Localizer.Active.GetLocalizedString(StringId.IconSetTitleSymbols3Uncircled)); 
			RegisterSet(sets, "Symbols3Circled", 3, "Symbols23_{0}.png", FormatPredefinedIconSets.CategorySymbols, Localizer.Active.GetLocalizedString(StringId.IconSetTitleSymbols3Circled)); 
			RegisterSet(sets, "Flags3", 3, "Flags3_{0}.png", FormatPredefinedIconSets.CategorySymbols, Localizer.Active.GetLocalizedString(StringId.IconSetTitleFlags3)); 
			RegisterSet(sets, new FormatConditionIconSetPositiveNegativeArrowsGray() { RangeDescription = ">0, 0, <0" });
			RegisterSet(sets, new FormatConditionIconSetPositiveNegativeArrows() { RangeDescription = ">0, 0, <0" });
			RegisterSet(sets, new FormatConditionIconSetPositiveNegativeTriangles() { RangeDescription = ">0, 0, <0" });
		}
		public FormatConditionIconSet Stars3 { get { return this["Stars3"]; } }
		public FormatConditionIconSet Ratings4 { get { return this["Ratings4"]; } }
		public FormatConditionIconSet Ratings5 { get { return this["Ratings5"]; } }
		public FormatConditionIconSet Quarters5 { get { return this["Quarters5"]; } }
		public FormatConditionIconSet Boxes5 { get { return this["Boxes5"]; } }
		public FormatConditionIconSet Arrows3Colored { get { return this["Arrows3Colored"]; } }
		public FormatConditionIconSet Arrows3Gray { get { return this["Arrows3Gray"]; } }
		public FormatConditionIconSet Triangles3 { get { return this["Triangles3"]; } }
		public FormatConditionIconSet Arrows4Colored { get { return this["Arrows4Colored"]; } }
		public FormatConditionIconSet Arrows4Gray { get { return this["Arrows4Gray"]; } }
		public FormatConditionIconSet Arrows5Colored { get { return this["Arrows5Colored"]; } }
		public FormatConditionIconSet Arrows5Gray { get { return this["Arrows5Gray"]; } }
		public FormatConditionIconSet TrafficLights3Unrimmed { get { return this["TrafficLights3Unrimmed"]; } }
		public FormatConditionIconSet TrafficLights3Rimmed { get { return this["TrafficLights3Rimmed"]; } }
		public FormatConditionIconSet Signs3 { get { return this["Signs3"]; } }
		public FormatConditionIconSet TrafficLights4 { get { return this["TrafficLights4"]; } }
		public FormatConditionIconSet RedToBlack { get { return this["RedToBlack"]; } }
		public FormatConditionIconSet Symbols3Circled { get { return this["Symbols3Circled"]; } }
		public FormatConditionIconSet Symbols3Uncircled { get { return this["Symbols3Uncircled"]; } }
		public FormatConditionIconSet Flags3 { get { return this["Flags3"]; } }
		public FormatConditionIconSet PositiveNegativeArrows { get { return this["PositiveNegativeArrows"]; } }
		public FormatConditionIconSet PositiveNegativeArrowsGray { get { return this["PositiveNegativeArrowsGray"]; } }
		public FormatConditionIconSet PositiveNegativeTriangles { get { return this["PositiveNegativeTriangles"]; } }
		public FormatConditionIconSet this[string name] {
			get {
				EnsureSets();
				FormatConditionIconSet res;
				if(sets.TryGetValue(name, out res)) return res;
				return null;
			}
		}
		void EnsureSets() {
			if(sets == null) {
				sets = new Dictionary<string, FormatConditionIconSet>();
				Register(sets);
			}
		}
		#region IEnumerable<FormatConditionIconSet> Members
		IEnumerator<FormatConditionIconSet> IEnumerable<FormatConditionIconSet>.GetEnumerator() {
			EnsureSets();
			return sets.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			EnsureSets();
			return sets.Values.GetEnumerator();
		}
		#endregion
	}
}
namespace DevExpress.XtraEditors.Helpers {
	public class IconSetImageLoader  {
		string skinName;
		[ThreadStatic]
		static Dictionary<string, IconSetImageLoader> _defaults;
		static Dictionary<string, IconSetImageLoader> Defaults {
			get {
				if(_defaults == null) _defaults = new Dictionary<string, IconSetImageLoader>();
				return _defaults;
			}
		}
		[ThreadStatic]
		static IconSetImageLoader _default;
		static IconSetImageLoader Default {
			get {
				if(_default == null) _default = new IconSetImageLoader(null);
				return _default;
			}
		}
		public IconSetImageLoader(string skinName) {
			this.skinName = skinName;
		}
		public static IconSetImageLoader GetDefault(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel == null || lookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return Default;
			string skin = lookAndFeel.ActiveSkinName;
			IconSetImageLoader res;
			if(Defaults.TryGetValue(skin, out res)) return res;
			res = new IconSetImageLoader(skin);
			Defaults[skin] = res;
			return res;
		}
		public List<string> GetImageNames() {
			List<string> mapItems = null, items = null;
			if(imageMap != null) mapItems = imageMap.Keys.ToList();
			if(singleImages != null) items = singleImages.Keys.ToList();
			var res = mapItems == null ? items : (items == null ? mapItems : mapItems.Union(items).ToList());
			res.Sort();
			return res;
		}
		Dictionary<string, Image> singleImages;
		Dictionary<string, IconSetImage> images;
		public IconSetImage GetImageIconSet(string iconSetImageName) {
			if(images == null) images = new Dictionary<string, IconSetImage>();
			IconSetImage key; ;
			if(!images.TryGetValue(iconSetImageName, out key)) {
				key = new IconSetImage(iconSetImageName, new Size(16, 16), skinName);
				images[iconSetImageName] = key;
			}
			return key;
		}
		public Image GetImage(string iconSetImageName, int x) {
			return GetImage(iconSetImageName, new Point(x, 0)); 
		}
		public Image GetImage(string iconSetImageName, Point location) {
			var res = GetImageIconSet(iconSetImageName);
			if(res == null) return null;
			return res.GetImage(location.X, location.Y);
		}
		public Image GetImage(string resourceImageName) {
			var map = GetImageMap(resourceImageName);
			if(map != null) return GetImage(map.ImageName, new Point(map.Index, map.Row));
			Image res;
			if(singleImages == null) singleImages = new Dictionary<string, Image>();
			if(!singleImages.TryGetValue(resourceImageName, out res)) {
				res = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.FormatRule.Images." + resourceImageName, typeof(IconSetImage).Assembly);
				singleImages[resourceImageName] = res;
			}
			return res;
		}
		class ImageMapInfo {
			public int Row { get; set; }
			public string ImageName { get; set; }
			public string ImageSource { get; set; }
			public int Index { get; set; }
		}
		Dictionary<string, ImageMapInfo> imageMap;
		ImageMapInfo GetImageMap(string imageName) {
			if(imageMap == null) {
				imageMap = new Dictionary<string, ImageMapInfo>();
				RegisterImageMap(imageMap);
			}
			ImageMapInfo res = null;
			if(imageMap.TryGetValue(imageName, out res)) {
				return res;
			}
			return null;
		}
		void RegisterImageMap(Dictionary<string, ImageMapInfo> imageMap) {
			RegisterImageMap(imageMap, "Stars3_{0}.png", 0, 3, "Ratings.png");
			RegisterImageMap(imageMap, "Quarters5_{0}.png", 1, 5, "Ratings.png");
			RegisterImageMap(imageMap, "Rating4_{0}.png", 2, 4, "Ratings.png");
			RegisterImageMap(imageMap, "Rating5_{0}.png", 3, 5, "Ratings.png");
			RegisterImageMap(imageMap, "Boxes5_{0}.png", 4, 5, "Ratings.png");
			RegisterImageMap(imageMap, "Arrows3_{0}.png", 0, 3, "Directional.png"); 
			RegisterImageMap(imageMap, "Arrows4_{0}.png", 1, 4, "Directional.png"); 
			RegisterImageMap(imageMap, "Arrows5_{0}.png", 2, 5, "Directional.png"); 
			RegisterImageMap(imageMap, "ArrowsGray3_{0}.png", 3, 3, "Directional.png"); 
			RegisterImageMap(imageMap, "ArrowsGray4_{0}.png", 4, 4, "Directional.png"); 
			RegisterImageMap(imageMap, "ArrowsGray5_{0}.png", 5, 5,  "Directional.png"); 
			RegisterImageMap(imageMap, "Triangles3_{0}.png", 6, 3, "Directional.png"); 
			RegisterImageMap(imageMap, "TrafficLights3_{0}.png", 0, 3, "Shapes.png"); 
			RegisterImageMap(imageMap, "TrafficLights23_{0}.png", 1, 3, "Shapes.png"); 
			RegisterImageMap(imageMap, "TrafficLights4_{0}.png", 2, 4, "Shapes.png"); 
			RegisterImageMap(imageMap, "RedToBlack4_{0}.png", 3, 4, "Shapes.png"); 
			RegisterImageMap(imageMap, "Signs3_{0}.png", 4, 3, "Shapes.png"); 
			RegisterImageMap(imageMap, "Symbols3_{0}.png", 0, 3, "Indicators.png"); 
			RegisterImageMap(imageMap, "Symbols23_{0}.png", 1, 3, "Indicators.png"); 
			RegisterImageMap(imageMap, "Flags3_{0}.png", 2, 3, "Indicators.png"); 
		}
		void RegisterImageMap(Dictionary<string, ImageMapInfo> imageMap, string imageName, int row, int count, string categoryImageName) {
			for(int n = 0; n < count; n++) {
				string name = string.Format(imageName, n + 1);
				imageMap[name] = new ImageMapInfo() { ImageSource = name, ImageName = categoryImageName, Row = row, Index = n };
			}
		}
	}
	public class IconSetImage {
		string resourceImageName;
		Bitmap image;
		Size singleSize;
		Bitmap[,] images;
		string skinName;
		public IconSetImage(string resourceImageName, Size singleSize, string skinName) {
			this.singleSize = singleSize;
			this.resourceImageName = resourceImageName;
			this.skinName = skinName;
		}
		public Bitmap GetImage(int x, int y) {
			if(image == null) {
				image = TryGetLookAndFeelImage(skinName);
				if(image == null)
					image = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.FormatRule.ImageSets." + resourceImageName, typeof(IconSetImage).Assembly);
				if(!singleSize.IsEmpty) images = new Bitmap[image.Height / singleSize.Height, image.Width / singleSize.Width];
			}
			if(singleSize.IsEmpty) return image;
			if(y >= images.Length) return null;
			if(images[y, x] == null) {
				images[y, x] = GetImageCore(image, x, y);
			}
			return images[y, x];
		}
		Bitmap TryGetLookAndFeelImage(string skinName) {
			Bitmap res = TryGetLookAndFeelImageFromSkin(skinName);
			if(res != null || string.IsNullOrEmpty(skinName)) return res;
			if(DevExpress.Utils.Frames.FrameHelper.IsDarkSkin(skinName, Utils.Frames.FrameHelper.SkinDefinitionReason.Rules)) {
				res = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.FormatRule.ImageSets.Dark." + resourceImageName, typeof(IconSetImage).Assembly);
			}
			else {
				res = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.FormatRule.ImageSets.Light." + resourceImageName, typeof(IconSetImage).Assembly);
			}
			return res;
		}
		Bitmap TryGetLookAndFeelImageFromSkin(string skinName) {
			if(skinName == null) return null;
			string skinElement = null;
			switch(resourceImageName) {
				case "Indicators.png" : skinElement = EditorsSkins.SkinFormatRuleIndicators; break;
				case "Shapes.png" : skinElement = EditorsSkins.SkinFormatRuleShapes; break;
				case "Directional.png" : skinElement = EditorsSkins.SkinFormatRuleDirectional; break;
				case "Ratings.png" : skinElement = EditorsSkins.SkinFormatRuleRatings; break;
			}
			if(skinElement == null) return null;
			var skin = SkinManager.Default.GetSkin(SkinProductId.Editors, skinName);
			if(skin == null) return null;
			var element = skin[skinElement];
			if(element == null || !element.HasImage || element.Image.Image == null) return null;
			return element.Image.Image as Bitmap;
		}
		Bitmap GetImageCore(Bitmap image, int x, int y) {
			Bitmap res = new Bitmap(singleSize.Width, singleSize.Height);
			using(Graphics g = Graphics.FromImage(res)) {
				g.DrawImage(image, new Rectangle(Point.Empty, singleSize), new Rectangle(x * singleSize.Width, y * singleSize.Height, singleSize.Width, singleSize.Height),
					GraphicsUnit.Pixel);
			}
			return res;
		}
	}
}
