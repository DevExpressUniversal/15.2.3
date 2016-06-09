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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace DevExpress.XtraEditors {
	public class TileControlTemplateHelper {
		public TileControlTemplateHelper(ITileItem tileItem) {
			if(tileItem == null)
				throw new ArgumentNullException("tileItem");
			Item = tileItem;
		}
		public ITileItem Item { get; internal set; }
		public TileItemTemplate GetTemplate() {
			return new TileItemTemplate(Item);
		}
		public void SetTemplate(TileItemTemplate template) {
			if(template == null) return;
			ApplyTemplate(template);
		}
		public void SaveTemplate(TileItemTemplate template, string path) {
			if(template == null) return;
			SaveTemplateToFile(template, path);
		}
		void SaveTemplateToFile(TileItemTemplate template, string path) {
			XmlSerializer serializer = new XmlSerializer(typeof(TileItemTemplate));
			using(StreamWriter writer = new StreamWriter(path)) {
				serializer.Serialize(writer, template);
			}
		}
		public TileItemTemplate LoadTemplate(string path) {
			if(File.Exists(path))
				return LoadTemplateCore(path);
			else
				throw new FileNotFoundException();
		}
		public TileItemTemplate LoadTemplate(Stream stream) {
			if(stream == null)
				return null;
			return LoadTemplateCore(stream);
		}
		TileItemTemplate LoadTemplateCore(object input) {
			Stream stream = input as Stream;
			if(stream != null) {
				XmlSerializer serializer = new XmlSerializer(typeof(TileItemTemplate));
				using(StreamReader reader = new StreamReader(stream)) {
					return serializer.Deserialize(reader) as TileItemTemplate;
				}
			}
			else {
				XmlSerializer serializer = new XmlSerializer(typeof(TileItemTemplate));
				using(StreamReader reader = new StreamReader(input.ToString())) {
					return serializer.Deserialize(reader) as TileItemTemplate;
				}
			}
		}
		void ApplyTemplate(TileItemTemplate template) {
			Item.Elements.Clear();
			Item.Frames.Clear();
			Item.BackgroundImage = null;
			Item.Properties.ItemSize = template.ItemSize;
			Item.Properties.AllowHtmlDraw = true;
			int frameCount = Math.Max(1, template.FrameCount);
			CreateFrames(frameCount);
			List<object> templateElements = new List<object>();
			if(template.TextElements != null)
				templateElements.AddRange(template.TextElements);
			if(template.ImageElements != null)
				templateElements.AddRange(template.ImageElements);
			for(int i = 0; i <= frameCount - 1; i++) {
				TileItem tempItem = new TileItem();
				AssignElementsToTempItem(tempItem, templateElements, i);
				ApplyTemplateCore(tempItem.Elements, i, frameCount);
				tempItem.Dispose();
				tempItem = null;
			}
			templateElements = null;
			if(ShouldScale)
				ScaleEverything();
		}
		void AssignElementsToTempItem(TileItem tempItem, List<object> list, int frameIndex) {
			var templateElements = list.Where(t => (t is TileItemElementText && ((TileItemElementText)t).Frame == frameIndex) ||
				(t is TileItemElementImage && ((TileItemElementImage)t).Frame == frameIndex));
			foreach(object element in templateElements) {
				tempItem.Elements.Add(GetElement(element));
			}
		}
		void CreateFrames(int frameCount) {
			if(frameCount <= 1)
				return;
			for(int i = 1; i <= frameCount; i++) {
				TileItemFrame frame = new TileItemFrame();
				Item.Frames.Add(frame);
			}
		}
		void ApplyTemplateCore(TileItemElementCollection elements, int frameIndex, int frameCount) {
			if(frameCount <= 1) {
				Item.Elements.Assign(elements);
				return;
			}
			if(Item.Frames.Count < frameIndex + 1)
				return;
			Item.Frames[frameIndex].Animation = frameIndex % 2 == 0 ? TileItemContentAnimationType.ScrollDown : TileItemContentAnimationType.ScrollTop;
			Item.Frames[frameIndex].Elements.Assign(elements);
		}
		const int defaultItemSize = 120;
		const int defaultIndent = 8;
		bool ShouldScale {
			get {
				try {
					return Item.Control.Properties.ItemSize != defaultItemSize | Item.Control.Properties.IndentBetweenItems != defaultIndent; 
				}
				catch { return false; }
			}
		}
		void ScaleEverything() {
			if(Item.Control == null)
				return;
			bool isWide = Item.Properties.ItemSize == TileItemSize.Wide ? true : false;
			int indent = Item.Control.Properties.IndentBetweenItems;
			int itemSize = Item.Control.Properties.ItemSize;
			double scaleY = (double)itemSize / defaultItemSize;
			double scaleX = (double)((itemSize * 2) + indent) / ((defaultItemSize * 2) + defaultIndent);
			foreach(TileItemElement element in Item.Elements) {
				ScaleElement(element, scaleX, scaleY, isWide);
			}
			foreach(TileItemFrame frame in Item.Frames) {
				foreach(TileItemElement element in frame.Elements) {
					ScaleElement(element, scaleX, scaleY, isWide);
				}
			}
		}
		void ScaleElement(TileItemElement element, double scaleX, double scaleY, bool isWide) {
			element.Appearance.Normal.Font = new Font(element.Appearance.Normal.Font.FontFamily, Math.Abs((float)(element.Appearance.Normal.Font.Size * scaleY)), element.Appearance.Normal.Font.Style);
			element.MaxWidth = (int)(element.MaxWidth * scaleX);
			element.TextLocation = ScalePoint(element.TextLocation, scaleX, scaleY, isWide);
			element.ImageLocation = ScalePoint(element.ImageLocation, scaleX, scaleY, isWide);
			element.ImageSize = ScaleSize(element.ImageSize, scaleX, scaleY, isWide);
			element.ImageAlignment = TileItemContentAlignment.Manual;
			element.Appearance.Selected.Assign(element.Appearance.Normal);
			element.Appearance.Hovered.Assign(element.Appearance.Normal);
		}
		const int zeroOffsetX = 12;
		const int zeroOffsetY = 8;
		Point ScalePoint(Point pt, double scaleX, double scaleY, bool isWide) {
			int x = pt.X + zeroOffsetX;
			int y = pt.Y + zeroOffsetY;
			if(!isWide) 
				scaleX = scaleY;
			x = ((int)(x * scaleX)) - zeroOffsetX;
			y = ((int)(y * scaleY)) - zeroOffsetY;
			return new Point(x, y);
		}
		Size ScaleSize(Size size, double scaleX, double scaleY, bool isWide) {
			if(!isWide)
				return new Size((int)(size.Width * scaleY), (int)(size.Height * scaleY));
			return new Size((int)(size.Width * scaleX), (int)(size.Height * scaleY));
		}
		TileItemElement GetElement(object element) {
			return element is TileItemElementText ? GetTextElement((TileItemElementText)element) : GetImageElement((TileItemElementImage)element);
		}
		TileItemElement GetTextElement(TileItemElementText elementText) {
			TileItemElement newElement = new TileItemElement();
			newElement.TextLocation = elementText.TextLocation;
			newElement.TextAlignment = elementText.TextAlignment;
			newElement.MaxWidth = elementText.MaxWidth;
			newElement.Text = String.IsNullOrEmpty(elementText.DefaultText) ? "default text" : elementText.DefaultText;
			newElement.Appearance.Normal.TextOptions.HAlignment = elementText.HAlignment;
			newElement.Appearance.Normal.TextOptions.WordWrap = elementText.Wrap;
			newElement.Appearance.Normal.TextOptions.Trimming = elementText.TrimmingStyle;
			newElement.Appearance.Normal.Font = elementText.Font.Value;
			newElement.Appearance.Selected.Assign(newElement.Appearance.Normal);
			newElement.Appearance.Hovered.Assign(newElement.Appearance.Normal);
			return newElement;
		}
		TileItemElement GetImageElement(TileItemElementImage elementImage) {
			TileItemElement newElement = new TileItemElement();
			newElement.Image = GetImageByName(elementImage.Name);
			newElement.ImageSize = elementImage.ImageSize;
			newElement.ImageAlignment = TileItemContentAlignment.Manual;
			newElement.ImageLocation = elementImage.ImageLocation;
			newElement.ImageScaleMode = TileItemImageScaleMode.ZoomOutside;
			return newElement;
		}
		Image GetImageByName(string name) {
			string path = "DevExpress.XtraEditors.TileControl.Images.";
			string file = path + name + ".png";
			try { return ResourceImageHelper.CreateImageFromResources(file, typeof(TileControl).Assembly) as Bitmap; }
			catch { return DefaultImage; }
		}
		Image defaultImage;
		Image DefaultImage {
			get { 
				if(defaultImage == null)
					defaultImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.TileControl.Images.120x120.png", typeof(TileControl).Assembly) as Bitmap;
				return defaultImage;
			}
		}
	}
	public class TileItemTemplate {
		public string Name { get; set; }
		public int FrameCount { get; set; }
		public TileItemSize ItemSize { get; set; }
		public List<TileItemElementText> TextElements { get; set; }
		public List<TileItemElementImage> ImageElements { get; set; }
		public TileItemTemplate() { }
		public TileItemTemplate(ITileItem tileItem) {
			if(tileItem == null) return;
			TextElements = new List<TileItemElementText>();
			ImageElements = new List<TileItemElementImage>();
			Name = tileItem.Tag == null ? "DefaultName" : tileItem.Tag as String;
			ItemSize = tileItem.Properties.ItemSize;
			if(tileItem.Frames.Count > 0) {
				FrameCount = tileItem.Frames.Count;
				for(int i = 0; i < FrameCount; i++) {
					foreach(TileItemElement itemElement in tileItem.Frames[i].Elements) {
						if(itemElement.Image == null)
							AddTextElement(itemElement, i);
						else
							AddImageElement(itemElement, i);
					}
				}
			}
			else {
				FrameCount = 0;
				foreach(TileItemElement itemElement in tileItem.Elements) {
					if(itemElement.Image == null)
						AddTextElement(itemElement, 0);
					else
						AddImageElement(itemElement, 0);
				}
			}
		}
		public void AddTextElement(TileItemElement itemElement, int frameIndex) {
			if(TextElements == null)
				TextElements = new List<TileItemElementText>();
			TextElements.Add(new TileItemElementText(itemElement) { Frame = frameIndex });
		}
		public void AddImageElement(TileItemElement itemElement, int frameIndex) {
			if(ImageElements == null)
				ImageElements = new List<TileItemElementImage>();
			ImageElements.Add(new TileItemElementImage(itemElement) { Frame = frameIndex });
		}
	}
	public class TileItemElementText {
		public Point TextLocation { get; set; }
		public TileItemContentAlignment TextAlignment { get; set; }
		public HorzAlignment HAlignment { get; set; }
		public SFont Font { get; set; }
		public WordWrap Wrap { get; set; }
		public Trimming TrimmingStyle { get; set; }
		public int MaxWidth { get; set; }
		public string DefaultText { get; set; }
		public int Frame {
			get { return frame; }
			set { frame = value; } 
		}
		int frame = 0;
		public TileItemElementText() { }
		public TileItemElementText(TileItemElement owner) {
			TextLocation = owner.TextLocation;
			TextAlignment = owner.TextAlignment;
			HAlignment = owner.Appearance.Normal.TextOptions.HAlignment;
			Font = new SFont(owner.Appearance.Normal.Font);
			Wrap = owner.Appearance.Normal.TextOptions.WordWrap;
			TrimmingStyle = owner.Appearance.Normal.TextOptions.Trimming;
			MaxWidth = owner.MaxWidth;
			DefaultText = owner.Text;
		}
	}
	public class TileItemElementImage {
		public string Name { get; set; }
		public Point ImageLocation { get; set; }
		public Size ImageSize { get; set; }
		public int Frame {
			get { return frame; }
			set { frame = value; }
		}
		int frame = 0;
		public TileItemElementImage() { }
		public TileItemElementImage(TileItemElement owner) {
			ImageLocation = owner.ImageLocation;
			ImageSize = owner.ImageSize;
		}
	}
	public class SFont {
		public SFont() {
			Value = null;
		}
		public SFont(Font font) {
			Value = font;
		}
		[XmlIgnore]
		public Font Value { get; set; }
		[XmlElement("Value")]
		public string SFontAttribute {
			get { return FontSerializer.ToString(Value); }
			set { Value = FontSerializer.ToFont(value); }
		}
		public static implicit operator Font(SFont sFont) {
			return sFont == null ? null : sFont.Value;
		}
		public static implicit operator SFont(Font font) {
			return new SFont(font);
		}
	}
	public static class FontSerializer {
		public static string ToString(Font font) {
			try {
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				return font == null ? null : tc.ConvertToInvariantString(font);
			}
			catch { }
			return null;
		}
		public static Font ToFont(string fontString) {
			try {
				TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
				return (Font)tc.ConvertFromInvariantString(fontString);
			}
			catch { }
			return null;
		}
	}
}
