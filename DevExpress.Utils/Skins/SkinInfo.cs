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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
namespace DevExpress.Skins.Info {
	[StructLayout(LayoutKind.Sequential)]
	public struct FileSkinInfoHeader {
		public int Signature;
		public short Version;
	}
	public class FileSkinImageInfo {
		public int NameSize, BlobSize;
		public string Name = string.Empty;
		public byte[] ImageBlob;
		public FileSkinImageInfo() {
		}
	}
	public class SkinAssemblyHelper {
		public static byte[] PublicKeyToken { get { return new byte[] { 0x95, 0xfc, 0x6c, 0x56, 0x21, 0x87, 0x8f, 0x0a }; } }
		static StrongNameKeyPair keyPair;
		public static StrongNameKeyPair KeyPair {
			get {
				if(keyPair == null) {
					Stream stream = typeof(Skin).Assembly.GetManifestResourceStream("DevExpress.Utils.Skins.UserSkin.snk");
					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, (int)stream.Length);
					keyPair = new StrongNameKeyPair(buffer);
					stream.Close();
				}
				return keyPair;
			}
		}
	}
	public class SkinXmlSerializer {
		XmlDocument document;
		Skin skin;
		public SkinXmlSerializer() {
			this.document = new XmlDocument();
		}
		public void Reset() {
			this.document = new XmlDocument();
		}
		public string ObjectToString(Skin skin, object obj) {
			if(obj is Color) {
				Color clr = (Color)obj;
				if(CommonColors.IsSystemColor(clr)) {
					return "$" + CommonColors.GetSystemColorName(clr.ToKnownColor());
				}
				if(clr.IsKnownColor) {
					return string.Format("{0}", clr.ToKnownColor().ToString());
				}
				if(clr.IsEmpty) return "";
				return clr.A == 255 ? String.Format("{0},{1},{2}", clr.R, clr.G, clr.B) : 
					String.Format("{0},{1},{2},{3}", clr.A, clr.R, clr.G, clr.B);
			}
			if(obj == null) return "";
			if(obj is Point || obj is Rectangle || obj is Size) {
				return SkinObjectConverter.IntStructureToString(obj);
			}
			return obj.ToString();
		}
		protected Skin Skin { get { return skin; } }
		protected XmlDocument Document { get { return document; } }
		protected XmlNode CreateNode(XmlNodeType nodeType, string name) {
			return Document.CreateNode(nodeType, name, "");
		}
		protected XmlNode CreateElementNode(string name) {
			return CreateNode(XmlNodeType.Element, name);
		}
		protected XmlAttribute CreateAttribute(string name, string val) {
			XmlAttribute res = Document.CreateAttribute(name);
			res.Value = val;
			return res;
		}
		public void Save(string fileName, Skin skin) {
			Save<string>(fileName, skin);
		}
		public void Save(Stream stream, Skin skin) {
			Save<Stream>(stream, skin);
		}
		protected void Save<T>(T destination, Skin skin) {
			this.skin = skin;
			XmlNode root = CreateNode(XmlNodeType.Element, SkinXml.DocumentName);
			Document.AppendChild(root);
			SaveSkin(root, root.Name);
			SaveDocument(destination);
		}
		protected void SaveDocument(object destination) {
			if(destination is string) {
				if(File.Exists(destination as string) && ((File.GetAttributes(destination as String) & FileAttributes.ReadOnly) != 0)) {
					MessageBox.Show("Unable to save the project. The '" + destination + "' file is readonly.");
					return;
				}
				Document.Save(destination as String);
			}
			if(destination is Stream) {
				Document.Save(destination as Stream);
			}
		}
		public void SaveAllSkins(string fileName, string skinName) {
			SaveAllSkins(fileName, skinName, skinName);
		}
		public void SaveAllSkins(string fileName, string skinName, string skinNameToSave) {
			SaveAllSkins<string>(fileName, skinName, skinNameToSave);
		}
		public void SaveAllSkins(Stream stream, string skinName) {
			SaveAllSkins<Stream>(stream, skinName, skinName);
		}
		public void SaveAllSkins<T>(T destination, string skinName, string skinNameToSave) {
			Array array = Enum.GetValues(typeof(SkinProductId));
			int count = 1;
			XmlNode multi = CreateNode(XmlNodeType.Element, SkinXml.MultiDocumentName);
			Document.AppendChild(multi);
			foreach(SkinProductId product in array) {
				this.skin = SkinManager.Default.GetSkin(product, skinName);
				if(this.skin == null) continue;
				XmlNode root = CreateNode(XmlNodeType.Element, string.Format("{0}{1}", SkinXml.DocumentName, count++));
				root.Attributes.Append(CreateAttribute(SkinXml.AttrSkinProductName, product.ToString()));
				if(!string.IsNullOrEmpty(skin.Container.Template))
					root.Attributes.Append(CreateAttribute(SkinXml.AttrBasedOn, skin.Container.Template));
				multi.AppendChild(root);
				SaveSkin(root, skinNameToSave);
			}
			SaveDocument(destination);
		}
		public void ExportAllSkins(string path) {
			foreach(SkinContainer container in SkinManager.Default.Skins) {
				Reset();
				ExportAllProducts(path, container.SkinName);
			}
		}
		public void ExportAllProducts(string path, string skinName) {
			ExportAllProducts(path, skinName, null);
		}
		public virtual void ExportAllProducts(string path, string skinName, string skinDirName) {
			if(skinName == null || skinName.Length == 0) return;
			string dest = GetDestinationPath(skinName, path, skinDirName);
			if(Path.GetFullPath(dest).Length < 4) return;
			OnDeleteCleanUpPath(dest);
			OnExportProducts(skinName, dest);
			SaveAllSkins(dest + "\\" + GetSkinXmlName(skinName), skinName);
		}
		public void ExportAllProducts(Stream stream, string skinName) {
			OnExportProducts(skinName, "temp");
		}
		protected virtual void OnExportProducts(string skinName, string dest) {
			Array array = Enum.GetValues(typeof(SkinProductId));
			foreach (SkinProductId product in array) {
				Skin skin = SkinManager.Default.GetSkin(product, skinName);
				if (skin == null)
					continue;
				ICollection collection = skin.GetElements();
				if (collection != null)
					OnExportSkinProduct(dest, product, collection);
			}
		}
		protected virtual bool DeleteFiles(string dest) {
			string[] files = Directory.GetFiles(dest);
			bool res = true;
			foreach(string file in files) {
				if(file.EndsWith("vssver2.scc")) {
					res = false;
					continue;
				}
				File.Delete(file);
			}
			return res;
		}
		protected virtual bool DeleteDirectories(string dest) {
			if(!Directory.Exists(dest))
				return true;
			string[] dirs = Directory.GetDirectories(dest);
			bool hasSvn = false;
			foreach(string dir in dirs) {
				if(dir.EndsWith(".svn")) {
					hasSvn = true;
					continue;
				}
				DeleteDirectories(dir);
			}
			bool hasFiles = !DeleteFiles(dest);
			if( hasFiles || hasSvn ) return false;
			try {
				Directory.Delete(dest);
			}
			catch(Exception) { }
			return true;
		}
		protected virtual void OnDeleteCleanUpPath(string dest) {
			try {
				DeleteDirectories(dest);
			}
			catch { }
		}
		protected virtual string GetSkinXmlName(string skinName) { return "skin.xml"; }
		protected virtual string GetDestinationPath(string skinName, string path, string skinDirName) {
			if(skinDirName == null) skinDirName = skinName;
			return string.Format("{0}\\{1}", Path.GetFullPath(path), skinDirName.ToLower());
		}
		protected virtual void OnExportSkinProduct(string dest, SkinProductId product, ICollection collection) {
			string subDir = product.ToString().ToLower();
			string destSkin = string.Format("{0}\\{1}", dest, subDir);
			Directory.CreateDirectory(destSkin);
			foreach(SkinElement element in collection) {
				if (element.IsCustomElement)
					continue;
				SaveCheckImageFile(product, element, dest, element.Info.Image, 1.0f);
				SaveCheckImageFile(product, element, dest, element.Info.Glyph, 1.0f);
				foreach(SkinBuilderElementInfo info in element.ScaledInfo.Values) {
					if(info.ScaleFactor != 1.0f) {
						SaveCheckImageFile(product, element, dest, info.ImageCore, info.ScaleFactor);
						SaveCheckImageFile(product, element, dest, info.GlyphCore, info.ScaleFactor);
					}
				}
			}
		}
		public virtual void SaveCheckImageFile(SkinProductId product, SkinElement element, string destPath, SkinImage image, float scaleFactor) {
			if(image == null || image.ImageCore == null) return;
			Image pic = image.ImageCore;
			string prevImageName, imageName;
			prevImageName = imageName = image.ImageName;
			bool isDefault = true;
			if(!IsDefaultPath(product, element, image, scaleFactor)) {
				imageName = GenerateCheckValidImageName(product, element, destPath, image, scaleFactor);
				image.SetImageNameCore(imageName);
				isDefault = false;
			}
			string fileName = Path.Combine(destPath, imageName);
			if(isDefault && File.Exists(fileName)) {
				OnAfterSaveImage(image, fileName);
				return;
			}
			try {
				pic.Save(fileName);
			}
			catch {
			}
			if(!isDefault && prevImageName.ToLower() != imageName.ToLower()) {
				image.SetImageProvider(null);
			}
			OnAfterSaveImage(image, fileName);
		}
		protected virtual void OnAfterSaveImage(SkinImage image, string fileName) {
			if(image.ImageProvider != null) image.ImageProvider.UpdateProviderInfo(fileName);
		}
		public static string GenerateDefaultImageName(SkinProductId product, SkinElement element, SkinImage image, float scaleFactor) {
			string xtra = "";
			if(image is SkinGlyph) xtra = "_glyph";
			if(scaleFactor == 1.0f)
				return product.ToString().ToLower() + @"\" + element.ElementName.Trim().ToLower() + xtra;
			return product.ToString().ToLower() + @"\" + String.Format("{0}dpi_{1}", (int)(scaleFactor * 100.0f), element.ElementName.Trim().ToLower() + xtra);
		}
		public static bool IsDefaultPath(SkinProductId product, SkinElement element, SkinImage image, float scaleFactor) {
			if(image == null || image.ImageCore == null) return false;
			return image.ImageName.ToLower() == (GenerateDefaultImageName(product, element, image, scaleFactor) + "." + GetImageExt(image.Image));
		}
		public virtual string GenerateCheckValidImageName(SkinProductId product, SkinElement element, string destPath, SkinImage img, float scaleFactor) {
			string ext = GetImageExt(img.Image);
			string imageName = Path.GetFileName(img.ImageName);
			for(int n = (imageName != "" ? -1 : 0); ; n++) {
				string file = imageName;
				if(n >= 0) {
					imageName = GenerateDefaultImageName(product, element, img, scaleFactor);
					file = string.Format("{0}{1}.{2}", imageName, n < 1 ? "" : n.ToString(), ext);
				}
				else {
					file = product.ToString().ToLower() + @"\" + file;
				}
				file = file.ToLower();
				if(!IsImageExists(file, img, destPath)) return file;
			}
		}
		protected virtual bool IsImageExists(string imageName, SkinImage image, string destPath) {
			if(File.Exists(destPath + "\\" + imageName)) return true;
			try {
				if(CompareImageCore(image, destPath, imageName)) return true;
			}
			catch {
			}
			return false;
		}
		protected virtual bool CompareImageCore(Image old, Image image) {
			if(CompareImage(old, image)) {
				Bitmap bmp1 = old as Bitmap;
				Bitmap bmp2 = image as Bitmap;
				if(bmp1 == null || bmp2 == null) { return bmp1 == bmp2; }
				MemoryStream ms1 = new MemoryStream(), ms2 = new MemoryStream();
				try {
					bmp1.Save(ms1, ImageCollectionStreamer.GetCodec(bmp1), null);
					bmp2.Save(ms2, ImageCollectionStreamer.GetCodec(bmp2), null);
					if(ms1.Length != ms2.Length) return false;
					ms1.Seek(0, SeekOrigin.Begin);
					ms2.Seek(0, SeekOrigin.Begin);
					while(true) {
						int b1 = ms1.ReadByte();
						if(b1 == -1) return true;
						if(b1 != ms2.ReadByte()) return false;
					}
				}
				finally {
					ms1.Close();
					ms2.Close();
				}
			}
			return false;
		}
		protected virtual bool CompareImageCore(SkinImage image, string destPath, string file) {
			if (!File.Exists(destPath + "\\" + file)) return false;
			using(Image old = Image.FromFile(destPath + "\\" + file)) {
				return CompareImageCore(image.Image, old);
			}
		}
		public static bool CompareImage(Image i1, Image i2) {
			if(i1 == i2) return true;
			if(i1.PhysicalDimension == i2.PhysicalDimension && i1.Size == i2.Size) return true;
			return false;
		}
		public static string GetImageExt(Image img) {
			ImageFormat format = img.RawFormat;
			PropertyInfo[] props = typeof(ImageFormat).GetProperties(BindingFlags.Static);
			foreach(PropertyInfo pi in props) {
				if(object.Equals(pi.GetValue(null, null), format)) return pi.Name.ToLower();
			}
			return "png";
		}
		protected virtual void SaveSkin(XmlNode root, string skinNameToSave) {
			root.Attributes.Append(CreateAttribute(SkinXml.AttrSkinName, skinNameToSave));
			SaveProperties(root, Skin.Properties.CustomProperties, SkinXml.PropertiesName, false);
			if(Skin.Properties.AllowScaleProperties) {
				foreach(float key in Skin.Properties.CustomPropertiesList.Keys) {
					int ikey = (int)(key * 100.0f);
					SaveProperties(root, Skin.Properties.CustomPropertiesList[key], SkinXml.PropertiesName + "_Dpi_" + ikey, false);
				}
			}
			if(SkinCollectionHelper.IsCustomSkin(skinNameToSave))
				SaveProperties(root, Skin.Colors.CustomProperties, SkinXml.ColorsName, true, new object[] { Skin.OptMaskColor, Skin.OptMaskColor2 });
			else 
				SaveProperties(root, Skin.Colors.CustomProperties, SkinXml.ColorsName, true, new object[] { Skin.OptMaskColor, Skin.OptMaskColor2, Skin.OptBaseColor, Skin.OptBaseColor2 });
			SaveProperties(root, Skin.Colors.RestrictedColors.CustomProperties, SkinXml.RestrictedColorsName, false);
			SaveSvgPaletteDictionary(root, Skin.SvgPalettes);
			root.AppendChild(SaveElements());
		}
		protected virtual void SaveSvgPaletteDictionary(XmlNode root, SvgPaletteDictionary svgPalettes) {
			XmlNode paletteNode = CreateElementNode(SkinXml.SvgPalette);
			root.AppendChild(paletteNode);
			foreach(var item in svgPalettes) {
				paletteNode.AppendChild(SaveSvgPalette(item));
			}
		}
		protected virtual void SaveProperties(XmlNode parent, Hashtable properties, string name, bool colors) {
			SaveProperties(parent, properties, name, colors, null);
		}
		protected virtual void SaveProperties(XmlNode parent, Hashtable properties, string name, bool colors, object[] restrictedProperties) {
			XmlNode node = SaveProperties(properties, name, colors, restrictedProperties);
			if(node != null) parent.AppendChild(node);
		}
		class KeyComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				return Comparer.Default.Compare(((DictionaryEntry)a).Key.ToString(), ((DictionaryEntry)b).Key.ToString());
			}
		}
		bool IsRestrictedProperty(object[] restrictedProp, object prop) {
			if(restrictedProp == null) return false;
			for(int i = 0; i < restrictedProp.Length; i++)
				if(restrictedProp[i].Equals(prop)) return true;
			return false;
		}
		protected virtual XmlNode SaveProperties(Hashtable properties, string name, bool colors, object[] restrictedProperties) {
			XmlNode node = null;
			if(properties == null) return null;
			ArrayList list = new ArrayList(properties);
			list.Sort(new KeyComparer());
			foreach(DictionaryEntry entry in list) {
				if(IsRestrictedProperty(restrictedProperties, entry.Key))
					continue;
				if(node == null) node = CreateElementNode(name);
				XmlNode property = CreateElementNode(entry.Key.ToString());
				property.Attributes.Append(CreateAttribute("Value", ObjectToString(Skin, entry.Value)));
				if(!colors)
					property.Attributes.Append(CreateAttribute("ValueType", entry.Value == null ? "" : entry.Value.GetType().FullName));
				node.AppendChild(property);
			}
			return node;
		}
		protected virtual XmlNode SaveElements() {
			XmlNode node = CreateElementNode("Elements");
			ArrayList list = new ArrayList();
			foreach(DictionaryEntry entry in Skin.Elements) {
				SkinElement element = (SkinElement)entry.Value;
				if(!element.IsCustomElement)
					list.Add(entry.Value);
			}
			list.Sort(new ElementComparer());
			foreach(SkinElement element in list) {
				node.AppendChild(SaveElement(element));
			}
			return node;
		}
		class ElementComparer : IComparer {
			int IComparer.Compare(object v1, object v2) {
				SkinElement el1 = v1 as SkinElement, el2 = v2 as SkinElement;
				return Comparer.Default.Compare(el1.ElementName, el2.ElementName);
			}
		}
		protected virtual XmlNode SaveElement(SkinElement element) {
			XmlNode node = CreateElementNode(element.ElementName);
			XmlNode edges = SaveEdges(SkinXml.ContentMargins, element.Info.ContentMargins);
			if(edges != null) node.AppendChild(edges);
			edges = SaveEdges(SkinXml.ContentMarginsTouch, element.Info.TouchContentMarginsCore);
			if(edges != null) node.AppendChild(edges);
			SaveImage(node, element.Info.Image, SkinXml.ImageName);
			SaveImage(node, element.Info.Glyph, SkinXml.GlyphName);
			XmlNode border = SaveBorder(SkinXml.BorderName, element.Info.Border);
			if(border != null) node.AppendChild(border);
			SaveProperties(node, element.Info.Properties.CustomProperties, SkinXml.PropertiesName, false);
			SaveOffset(node, element.Info.Offset, SkinXml.OffsetName);
			SaveColor(node, element.Info.Color, SkinXml.ColorName);
			SaveSize(node, element.Info.Size, SkinXml.SizeName);
			if(element.Info.SizeTouch.HasCustomSize)
				element.Info.SizeTouch.AllowScale = false;
				SaveSize(node, element.Info.SizeTouch, SkinXml.SizeTouchName);
				element.Info.SizeTouch.AllowScale = true;
			if(element.SvgPalettes.Count != 0)
				SaveSvgPaletteDictionary(node, element.SvgPalettes);
			if(ShouldSerializeScaledInfo(element)) {
				XmlNode scaledNode = CreateElementNode(SkinXml.ScaleInfo);
				node.AppendChild(scaledNode);
				foreach(SkinBuilderElementInfo info in element.ScaledInfo.Values) {
					if(info.ScaleFactor == 1.0f || !info.ShouldSerialize)
						continue;
					scaledNode.AppendChild(SaveScaledInfo(info));
				}
			}
			return node;
		}
		protected bool ShouldSerializeScaledInfo(SkinElement element) {
			foreach(SkinBuilderElementInfo info in element.ScaledInfo.Values) {
				if(info.ScaleFactor != 1.0f)
					return true;
			}
			return false;
		}
		protected virtual XmlNode SaveSvgPalette(KeyValuePair<ObjectState, SvgPalette> info) {
			XmlNode node = CreateSvgPaletteNode(info.Key);
			foreach(var item in info.Value.Colors) {
				node.AppendChild(SaveSvgPaletteColor(item));
			}
			return node;
		}
		XmlNode SaveSvgPaletteColor(SvgColor item) {
			return SaveObject("SvgColor", item, new string[] { "Name", "Value" });
		}
		protected virtual XmlNode SaveScaledInfo(SkinBuilderElementInfo info) {
			XmlNode node = CreateScaledInfoNode(info);
			if(info.ShouldSerializeContentMargins) {
				XmlNode edges = SaveEdges(SkinXml.ContentMargins, info.ContentMargins);
				if(edges != null) node.AppendChild(edges);
			}
			if(info.ShouldSerializeImage)
				SaveImage(node, info.Image, SkinXml.ImageName);
			if(info.ShouldSerializeGlyph)
				SaveImage(node, info.Glyph, SkinXml.GlyphName);
			if(info.ShouldSerializeProperties)
				SaveProperties(node, info.Properties.CustomProperties, SkinXml.PropertiesName, false);
			if(info.ShouldSerializeOffset)
				SaveOffset(node, info.Offset, SkinXml.OffsetName);
			SaveColor(node, info.Color, SkinXml.ColorName);
			if(info.ShouldSerializeSize)
				SaveSize(node, info.Size, SkinXml.SizeName);
			return node;
		}
		protected virtual XmlNode CreateSvgPaletteNode(ObjectState state) {
			XmlNode node = Document.CreateNode(XmlNodeType.Element, SkinXml.SvgPalette, "");
			XmlAttribute att = Document.CreateAttribute("ObjectState");
			att.Value = state.ToString();
			node.Attributes.Append(att);
			return node;
		}
		protected virtual XmlNode CreateScaledInfoNode(SkinBuilderElementInfo info) {
			XmlNode node = Document.CreateNode(XmlNodeType.Element, SkinXml.ScaleInfo, "");
			XmlAttribute att = Document.CreateAttribute(SkinXml.ScaleFactor);
				att.Value = info.ScaleFactor.ToString();
			node.Attributes.Append(att);
			return node;
		}
		protected virtual void SaveSize(XmlNode parent, SkinSize size, string name) {
			if(!size.ShouldSerialize()) return;
			SaveObject(parent, name, size, null);
		}
		protected virtual void SaveOffset(XmlNode parent, SkinOffset offset, string name) {
			if(offset.IsEmpty) return;
			SaveObject(parent, name, offset, new string[] { "Kind", "Offset" });
		}
		protected virtual void SaveColor(XmlNode parent, SkinColor color, string name) {
			if(color.IsEmpty) return;
			SaveObject(parent, name, color, null);
		}
		protected virtual XmlNode SaveBorder(string name, SkinBorder border) {
			if(border.IsEmpty) return null;
			XmlNode borderNode = SaveObject(name, border, new string[] { "Left", "Top", "Right", "Bottom" });
			XmlNode edges = SaveEdges(SkinXml.BorderThinName, border.Thin);
			if(edges != null) {
				if(borderNode == null) borderNode = CreateElementNode(name);
				borderNode.AppendChild(edges);
			}
			return borderNode;
		}
		protected void SaveEdges(XmlNode parent, string name, SkinPaddingEdges edges) {
			XmlNode node = SaveEdges(name, edges);
			if(node != null) parent.AppendChild(node);
		}
		protected virtual XmlNode SaveEdges(string name, SkinPaddingEdges edges) {
			if(edges.IsEmpty) return null;
			return SaveObject(name, edges, new String[] { "Left", "Top", "Right", "Bottom" });
		}
		class ColorMode {
			public SkinImageColorizationMode Value { get; set; }
		}
		protected virtual void SaveImage(XmlNode parent, SkinImage image, string name) {
			if(image == null || image.ImageCore == null) return;
			XmlNode node = SaveObject(name, image, new string[] { "ImageCount", "Layout", "Stretch", "ImageName", "TransparentColor" });
			XmlNode edges = SaveEdges(SkinXml.ImageSizingMargins, image.SizingMargins);
			if(edges != null) {
				if(node == null) node = CreateElementNode(name);
				node.AppendChild(edges);
			}
			if(ShouldSerializeColorModes(image)) {
				XmlNode colorModes = CreateElementNode(SkinXml.ColorModes);
				node.AppendChild(colorModes);
				for(int i = 0; i < image.ImageColorizationModes.Length; i++) {
					colorModes.AppendChild(SaveObject(SkinXml.ColorMode, new ColorMode(){ Value = image.ImageColorizationModes[i] }, null));
				}
			}
			if(node != null) parent.AppendChild(node);
		}
		private bool ShouldSerializeColorModes(SkinImage image) {
			if(image.ImageColorizationModes == null)
				return false;
			for(int i = 0; i < image.ImageColorizationModes.Length; i++)
				if(image.ImageColorizationModes[i] != SkinImageColorizationMode.Default)
					return true;
			return false;
		}
		protected virtual XmlNode SaveObject(string name, object instance, string[] properties) {
			if(instance == null) return null;
			XmlNode node = CreateElementNode(name);
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance);
			if(props.Count == 0) return null;
			foreach(PropertyDescriptor prop in props) {
				if(properties != null && Array.IndexOf(properties, prop.Name) == -1) continue;
				if(prop.IsReadOnly || !prop.ShouldSerializeValue(instance)) continue;
				string val = ObjectToString(Skin, prop.GetValue(instance));
				node.Attributes.Append(CreateAttribute(prop.Name, val));
			}
			if(node.Attributes.Count == 0) return null;
			return node;
		}
		protected void SaveObject(XmlNode parent, string name, object instance, string[] properties) {
			XmlNode node = SaveObject(name, instance, properties);
			if(node != null) parent.AppendChild(node);
		}
	}
	public class SkinBlobLoader {
		public static Hashtable LoadBlob(string path) {
			using(FileStream fs = new FileStream(path, FileMode.Open,  FileAccess.Read)) {
				return LoadBlob(fs, true);
			}
		}
		public static Hashtable LoadBlob(Stream fs) {
			return LoadBlob(fs, true);
		}
		public static Hashtable LoadBlob(Stream fs, bool asImage) {
			byte[] version = new byte[SkinBlobSerializer.blobVersion.Length];
			fs.Read(version, 0, version.Length);
			for(int n = 0; n < version.Length; n++) {
				if(version[n] != SkinBlobSerializer.blobVersion[n]) throw new System.InvalidOperationException("Invalid blob format");
			}
			Hashtable res = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			while(true) {
				if(fs.Position >= fs.Length - 1) break;
				int textSize = SkinBlobSerializer.ReadInt(fs);
				int imageSize = SkinBlobSerializer.ReadInt(fs);
				string imageName = SkinBlobSerializer.ReadString(fs, textSize);
				if(asImage) {
					MemoryStream ms = new MemoryStream(imageSize);
					ms.SetLength(imageSize);
					fs.Read(ms.GetBuffer(), 0, imageSize);
					ms.Seek(0, SeekOrigin.Begin);
					res[imageName] = ImageTool.ImageFromStream(ms); 
				} else
				{
					res[imageName] = new SkinImageStream(fs, fs.Position, imageSize);
					fs.Seek(imageSize, SeekOrigin.Current);
				}
			}
			return res;
		}
		public static void SaveBlob(string path, Hashtable blob) {
			using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				SkinBlobSerializer.SaveBlobHeader(fs);
				foreach(DictionaryEntry entry in blob) {
					if(entry.Value is byte[])
						SkinBlobSerializer.WriteResourceBlob(fs, entry.Value as byte[], entry.Key.ToString());
					else
						SkinBlobSerializer.WriteResourceImage(fs, entry.Value as Image, entry.Key.ToString());
				}
				fs.Close();
			}
		}
		public static void SaveImage(Image image, string path) {
			image.Save(path, ImageCollectionStreamer.GetCodec(image), null);
		}
	}
	public class SkinBlobSerializer : SkinXmlSerializer {
		Stream streamToSerialize = null;
		public SkinBlobSerializer() : this(null) {
		}
		public SkinBlobSerializer(Stream stream) {
			this.streamToSerialize = stream;
		}
		protected override string GetSkinXmlName(string skinName) { 
			return skinName.ToLower() + "." + "skin.xml"; 
		}
		protected override string GetDestinationPath(string skinName, string path, string skinDirName) {
			if(skinDirName == null) skinDirName = skinName;
			return Path.GetFullPath(path) + "\\SkinData\\";
		}
		Hashtable resources = new Hashtable();
		protected override void OnExportSkinProduct(string dest, SkinProductId product, ICollection collection) {
			this.resources = new Hashtable();
			foreach(SkinElement element in collection) {
				if (element.IsCustomElement)
					continue;
				SaveCheckImageFile(product, element, dest, element.Info.Image, 1.0f);
				SaveCheckImageFile(product, element, dest, element.Info.Glyph, 1.0f);
				foreach(SkinBuilderElementInfo info in element.ScaledInfo.Values) {
					if(info.ScaleFactor != 1.0f) {
						SaveCheckImageFile(product, element, dest, info.ImageCore, info.ScaleFactor);
						SaveCheckImageFile(product, element, dest, info.GlyphCore, info.ScaleFactor);
					}
				}
			}
		}
		protected override void OnDeleteCleanUpPath(string dest) { }
		protected override void OnExportProducts(string skinName, string dest) {
			if(streamToSerialize == null) Directory.CreateDirectory(dest);
			string file = string.Format("{0}\\{1}.blob", dest, skinName.ToLower());
			if(streamToSerialize == null) {
				this.blobStream = new FileStream(file, FileMode.Create);
			} else {
				this.blobStream = streamToSerialize;
			}
			SaveBlobHeader(blobStream);
			base.OnExportProducts(skinName, dest);
			if(streamToSerialize == null) {
				this.blobStream.Close();
			}
			this.blobStream = null;
		}
		protected override void OnAfterSaveImage(SkinImage image, string fileName) {
			if(image.ImageProvider != null) image.SetImageProvider(null);
		}
		public override void SaveCheckImageFile(SkinProductId product, SkinElement element, string destPath, SkinImage image, float scaleFactor) {
			if(image == null || image.Image == null) return;
			string prevImageName, imageName;
			prevImageName = imageName = image.ImageName;
			bool isDefault = true;
			if(!IsDefaultPath(product, element, image, scaleFactor)) {
				imageName = GenerateCheckValidImageName(product, element, destPath, image, scaleFactor);
				image.SetImageNameCore(imageName);
				isDefault = false;
			}
			imageName = imageName.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			prevImageName = prevImageName.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			if(isDefault && this.resources[imageName] != null) {
				OnAfterSaveImage(image, imageName);
				return;
			}
			this.resources[imageName] = image.Image;
			try {
				WriteResourceImage(this.blobStream, image.Image, imageName);
			}
			catch {
			}
			if(!isDefault && prevImageName.ToLower() != imageName.ToLower()) {
				image.SetImageProvider(null);
			}
			OnAfterSaveImage(image, imageName);
		}
		protected override bool IsImageExists(string imageName, SkinImage image, string destPath) {
			if(!this.resources.Contains(imageName)) return false;
			Image imagePrev = this.resources[imageName] as Image;
			try {
				if(CompareImageCore(image.Image, imagePrev)) return true;
			}
			catch {
			}
			return false;
		}
		Stream blobStream;
		public static byte[] blobVersion = new byte[] { (byte)'S', (byte)'K', (byte)'I', (byte)'N', (byte)'v', (byte)'1', 0, 0, 0, 0 };
		internal static void SaveBlobHeader(Stream stream) {
			stream.Write(blobVersion, 0, blobVersion.Length);
		}
		public static void WriteString(Stream stream, string s) {
			foreach(char ch in s) {
				stream.WriteByte((byte)ch);
			}
		}
		public static void WriteInt(Stream stream, int number) {
			byte[] bytes = new byte[4] { (byte)(number & 0xff), (byte)((number >> 8) & 0xff), (byte)((number >> 16) & 0xff), (byte)((number >> 24) & 0xff) };
			stream.Write(bytes, 0, 4);
		}
		public static int ReadInt(Stream stream) {
			return (int)stream.ReadByte() | (((int)stream.ReadByte()) << 8) | (((int)stream.ReadByte()) << 16) | (((int)stream.ReadByte()) << 24);
		}
		public static string ReadString(Stream stream, int len) {
			char[] sb = new char[len];
			for(int i = 0; i < len; i++) sb[i] = (char)stream.ReadByte();
			return new string(sb);
		}
		internal static void WriteResourceBlob(Stream stream, byte[] data, string fullName) {
			WriteInt(stream, fullName.Length);
			MemoryStream ms = new MemoryStream();
			ms.Write(data, 0, data.Length);
			WriteInt(stream, (int)ms.Position);
			WriteString(stream, fullName.ToLower(System.Globalization.CultureInfo.InvariantCulture));
			ms.Seek(0, SeekOrigin.Begin);
			ms.WriteTo(stream);
			ms.Close();
		}
		internal static void WriteResourceImage(Stream stream, Image image, string fullName) {
			WriteInt(stream, fullName.Length);
			MemoryStream ms = new MemoryStream();
			image.Save(ms, DevExpress.Utils.ImageCollectionStreamer.GetCodec(image), null);
			WriteInt(stream, (int)ms.Position);
			WriteString(stream, fullName);
			ms.Seek(0, SeekOrigin.Begin);
			ms.WriteTo(stream);
			ms.Close();
		}
	}
	public class SkinXml {
		public const string
			AttrBasedOn = "BasedOn",
			AttrSkinName = "SkinName",
			AttrSkinProductName = "SkinProduct",
			ColorsName = "Colors",
			RestrictedColorsName = "RestrictedColors",
			PropertiesName = "Properties",
			ElementsName = "Elements",
			DocumentName = "Skin",
			MultiDocumentName = "Skins",
			BorderName = "Border",
			BorderThinName = "BorderThin",
			ContentMargins = "ContentMargins",
			ContentMarginsTouch = "ContentMarginsTouch",
			ImageSizingMargins = "SizingMargins",
			ImageName = "Image",
			GlyphName = "Glyph",
			ColorName = "Color",
			SizeName = "Size",
			SizeTouchName = "SizeTouch",
			OffsetName = "Offset",
			ScaleInfo = "ScaleInfo",
			ScaleFactor = "ScaleFactor",
			ColorModes = "ColorModes",
			ColorMode = "ColorMode",
			SvgPalette = "SvgPalette",
			SvgColor = "SvgColor"
			;
	}
	public class SkinXmlLoader {
		XmlTextReader document;
		SkinBuilder skinBuilder;
		ISkinImageLoader imageLoader;
		public SkinXmlLoader() : this(null) { }
		public SkinXmlLoader(ISkinImageLoader imageLoader) {
			this.imageLoader = imageLoader;
		}
		protected ISkinImageLoader ImageLoader { get { return imageLoader; } }
		public virtual void Load(string fileName, string skinName) {
			using(FileStream fs = File.OpenRead(fileName)) {
				this.document = new XmlTextReader(fs);
				this.skinBuilder = new SkinBuilder(skinName);
				document.MoveToContent();
				LoadSkin(skinName);
			}
		}
		public void LoadAllSkinsFromText(string xmlText, string skinName) {
			this.document = new XmlTextReader(new StringReader(xmlText));
			LoadAllSkins(skinName);
		}
		public void LoadAllSkins(Stream stream, string skinName) {
			if(stream == null) return;
			this.document = new XmlTextReader(stream);
			LoadAllSkins(skinName);
		}
		public void LoadAllSkins(string fileName, string skinName) {
			using(FileStream fs = File.OpenRead(fileName)) {
				this.document = new XmlTextReader(fs);
				LoadAllSkins(skinName);
				this.document = null;
			}
		}
		protected virtual void LoadAllSkins(string skinName) {
			Document.MoveToContent();
			while(Document.Read()) {
				if(Document.IsStartElement()) {
					if(skinName == null) {
						skinName = Document.GetAttribute(SkinXml.AttrSkinName);
					}
					string basedOn = Document.GetAttribute(SkinXml.AttrBasedOn);
					string productName = Document.GetAttribute(SkinXml.AttrSkinProductName);
					this.skinBuilder = new SkinBuilder(skinName);
					LoadSkin(skinName);
					if(Enum.IsDefined(typeof(SkinProductId), productName)) {
						SkinProductId product = (SkinProductId)Enum.Parse(typeof(SkinProductId), productName);
						SkinManager.Default.RegisterSkin(product, skinBuilder, basedOn);
					} else {
						SkinManager.Default.RegisterSkin(productName, skinBuilder, basedOn);
					}
				}
			}
			if(SkinManager.Default.Skins[skinName].GetSkin(SkinProductId.Ribbon) == null) {
				Skin ribbon = SkinManager.Default.Skins["DevExpress Style"].GetSkin(SkinProductId.Ribbon).Clone(skinName) as Skin;
				ribbon.Cloned = "DevExpress Style";
				SkinBuilder sb = new SkinBuilder(skinName);
				sb.skin = ribbon;
				SkinManager.Default.RegisterSkin(SkinProductId.Ribbon, sb);
			}
		}
		protected virtual void LoadSkin(string skinName) {
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement()) {
					if(Document.Name.StartsWith(SkinXml.PropertiesName)) {
						int index = Document.Name.IndexOf("_Dpi_");
						float scale = 1.0f;
						if(index != -1) {
							index += "_Dpi_".Length;
							scale = float.Parse(Document.Name.Substring(index, Document.Name.Length - index)) / 100.0f;
						}
						LoadProperties(SkinBuilder.Skin.Properties, scale);
					}
					switch(Document.Name) {
						case SkinXml.ColorsName:
							LoadProperties(SkinBuilder.Skin.Colors, true);
							break;
						case SkinXml.RestrictedColorsName:
							LoadProperties(SkinBuilder.Skin.Colors.RestrictedColors, false);
							break;
						case SkinXml.ElementsName:
							LoadElements();
							break;
						case SkinXml.SvgPalette:
							LoadSvgPalettesList(SkinBuilder.Skin.SvgPalettes);
							break;
					}
				}
			}
		}
		protected XmlTextReader Document { get { return document; } }
		public SkinBuilder SkinBuilder { get { return skinBuilder; } }
		protected virtual void LoadProperties(SkinProperties properties, float scale) {
			if(scale == 1.0f) {
				LoadProperties(properties, false);
				return;
			}
			if(!properties.AllowScaleProperties)
				return;
			SkinProperties prop = new SkinProperties() { AllowScaleProperties = false };
			LoadProperties(prop, false);
			properties.CustomPropertiesList.Add(scale, prop.CustomProperties);
		}
		protected virtual void LoadProperties(SkinCustomProperties properties, bool colors) {
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement()) {
					string attrVal = Document.GetAttribute("Value");
					Type type = typeof(Color);
					if(!colors && Document.GetAttribute("ValueType") != "System.Drawing.Color") { 
						string attrVt = Document.GetAttribute("ValueType");
						type = Type.GetType(attrVt);
						if(type == null)
							type = typeof(Color).Assembly.GetType(attrVt);
					}
					if(type != null) {
						object val = StringToObject(type, attrVal);
						properties.SetProperty(Document.Name, val);
					}
				}
			}
		}
		protected virtual void LoadElements() {
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement())
					LoadElement();
			}
		}
		bool CheckLoadSvgPalette(out string name, out Color color) {
			if(Document.Name == SkinXml.SvgColor) {
				color = GetSvgColorValue(out name);
				return true;
			}
			name = string.Empty;
			color = Color.Empty;
			return false;
		}
		bool CheckLoadSkinBuilderInfoCore(SkinBuilderElementInfo info) {
			switch(Document.Name) {
				case SkinXml.ContentMargins:
					LoadMargins(info.ContentMargins);
					return true;
				case SkinXml.ContentMarginsTouch:
					LoadMargins(info.TouchContentMarginsCore);
					return true;
				case SkinXml.ImageName:
					info.Image = LoadImage(new SkinImage((Image)null));
					return true;
				case SkinXml.GlyphName:
					info.Glyph = LoadImage(new SkinGlyph((Image)null)) as SkinGlyph;
					return true;
				case SkinXml.BorderName:
					LoadBorder(info.Border);
					return true;
				case SkinXml.PropertiesName:
					LoadProperties(info.Properties, false);
					return true;
				case SkinXml.OffsetName:
					LoadOffset(info.Offset);
					return true;
				case SkinXml.ColorName:
					LoadColor(info.Color);
					return true;
				case SkinXml.SizeName:
					LoadSize(info.Size);
					return true;
				case SkinXml.SizeTouchName:
					LoadSize(info.SizeTouch);
					return true;
			}
			return false;
		}
		protected virtual void LoadElement() {
			int depth = Document.Depth;
			string name = Document.Name;
			List<SkinBuilderElementInfo> scaledInfo = null;
			SkinBuilderElementInfo info = new SkinBuilderElementInfo();
			SkinElement elem = SkinBuilder.CreateElement(name, info);
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement()) {
					if(CheckLoadSkinBuilderInfoCore(info))
						continue;
					if(Document.Name == SkinXml.ScaleInfo)
						scaledInfo = LoadScaledInfoList(info);
					if(Document.Name == SkinXml.SvgPalette)
						LoadSvgPalettesList(elem.SvgPalettes);
				}
			}
			info.OwnerElement = elem;
			if(scaledInfo != null) {
				foreach(SkinBuilderElementInfo scInfo in scaledInfo) {
					scInfo.OriginalInfo = info;
					scInfo.OwnerElement = elem;
					elem.ScaledInfo.Add(scInfo.ScaleFactor, scInfo);
				}
			}
		}
		protected virtual void LoadSvgPalettesList(SvgPaletteDictionary paletes) {
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement() && Document.Name == SkinXml.SvgPalette) {
					ObjectState state = (ObjectState)Enum.Parse(typeof(ObjectState), Document.GetAttribute("ObjectState"));
					paletes.Add(state, LoadSvgPalette());
				}
			}
		}
		SvgPalette LoadSvgPalette() {
			SvgPalette result = new SvgPalette();
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement()) {
					string name;
					Color color;
					if(CheckLoadSvgPalette(out name, out color))
						result.Colors.Add(new SvgColor(name, color));
				}
			}
			return result;
		}
		protected virtual List<SkinBuilderElementInfo> LoadScaledInfoList(SkinBuilderElementInfo origin) {
			List<SkinBuilderElementInfo> list = new List<SkinBuilderElementInfo>();
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement() && Document.Name == SkinXml.ScaleInfo) {
					float scaleFactor = float.Parse(Document.GetAttribute(SkinXml.ScaleFactor), CultureInfo.InvariantCulture);
					list.Add(LoadScaledInfo(origin, scaleFactor));
					list[list.Count - 1].ScaleFactor = scaleFactor;
				}
			}
			return list;
		}
		protected virtual SkinBuilderElementInfo LoadScaledInfo(SkinBuilderElementInfo origin, float scaleFactor) {
			SkinBuilderElementInfo info = origin.GenerateInfo(scaleFactor, false);
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement()) {
					CheckLoadSkinBuilderInfoCore(info);
					RemoveUnnecessaryPropertiesFromScaledInfo(info);
				}
			}
			return info;
		}
		private void RemoveUnnecessaryPropertiesFromScaledInfo(SkinBuilderElementInfo info)
		{
			info.Properties.CustomProperties.Remove("FontDelta");
		}
		protected virtual void LoadSize(SkinSize size) {
			size.AllowHGrow = GetBoolValue("AllowHGrow", true);
			size.AllowVGrow = GetBoolValue("AllowVGrow", true);
			size.MinSize = GetSizeValue("MinSize", SkinSize.DefaultSize);
		}
		protected virtual void LoadOffset(SkinOffset offset) {
			offset.Kind = GetSkinOffsetKindValue("Kind", SkinOffsetKind.Default);
			offset.Offset = GetPointValue("Offset", Point.Empty);
		}
		protected virtual void LoadColor(SkinColor color) {
			color.BackColor = GetColorValue("BackColor", Color.Empty);
			color.BackColor2 = GetColorValue("BackColor2", Color.Empty);
			color.FontBold = GetBoolValue("FontBold", false);
			color.ForeColor = GetColorValue("ForeColor", Color.Empty);
			color.FontSize = GetIntValue("FontSize", 0);
			color.GradientMode = GetLinearGradientModeValue("GradientMode", LinearGradientMode.Horizontal);
			color.SolidImageCenterColor = GetColorValue("SolidImageCenterColor", Color.Empty);
			color.SolidImageCenterColor2 = GetColorValue("SolidImageCenterColor2", Color.Empty);
			color.SolidImageCenterGradientMode = GetLinearGradientModeValue("SolidImageCenterGradientMode", LinearGradientMode.Horizontal);
		}
		protected virtual void LoadBorder(SkinBorder border) {
			border.Bottom = GetColorValue("Bottom", Color.Empty);
			border.Left = GetColorValue("Left", Color.Empty);
			border.Right = GetColorValue("Right", Color.Empty);
			border.Top = GetColorValue("Top", Color.Empty);
			if(!Document.IsEmptyElement) {
				Document.Read();
				if(Document.IsStartElement() && Document.Name == SkinXml.BorderThinName)
					LoadMargins(border.Thin);
				int depth = Document.Depth;
				while(Document.Read() && depth <= Document.Depth)
					;
			}
		}
		protected virtual SkinImage LoadImage(SkinImage image) {
			try {
				image.ImageCount = GetIntValue("ImageCount", 1);
				image.Layout = GetSkinImageLayoutValue("Layout", SkinImageLayout.Horizontal);
				image.Stretch = GetSkinImageStretchValue("Stretch", SkinImageStretch.Stretch);
				image.TransparentColor = GetColorValue("TransparentColor", Color.Empty);
				image.imageLoader = ImageLoader;
				image.ImageName = GetStringValue("ImageName", String.Empty);
				if(!Document.IsEmptyElement) {
					Document.Read();
					if(Document.IsStartElement() && Document.Name == SkinXml.ImageSizingMargins) {
						LoadMargins(image.SizingMargins);
						Document.Read();
					}
					if(Document.IsStartElement() && Document.Name == SkinXml.ColorModes) {
						LoadColorModes(image);
					}
					Document.Read();
				}
			}
			finally {
				image.imageLoader = null;
			}
			return image;
		}
		private void LoadColorModes(SkinImage image) {
			int index = 0;
			int depth = Document.Depth;
			while(Document.Read() && depth < Document.Depth) {
				if(Document.IsStartElement()) {
					string stringVal = GetStringValue("Value", "Default");
					image.ImageColorizationModes[index] = (SkinImageColorizationMode)Enum.Parse(typeof(SkinImageColorizationMode), stringVal);
					index++;
				}
			}
		}
		SkinOffsetKind GetSkinOffsetKindValue(string name, SkinOffsetKind defaultValue) {
			string att = Document.GetAttribute(name);
			if(att == null)
				return defaultValue;
			if(att == "Center")
				return SkinOffsetKind.Center;
			if(att == "Default")
				return SkinOffsetKind.Default;
			if(att == "Far")
				return SkinOffsetKind.Far;
			if(att == "Near")
				return SkinOffsetKind.Near;
			return defaultValue;
		}
		LinearGradientMode GetLinearGradientModeValue(string name, LinearGradientMode defaultValue) {
			string att = Document.GetAttribute(name);
			if(att == null)
				return defaultValue;
			if(att == "BackwardDiagonal")
				return LinearGradientMode.BackwardDiagonal;
			if(att == "ForwardDiagonal")
				return LinearGradientMode.ForwardDiagonal;
			if(att == "Horizontal")
				return LinearGradientMode.Horizontal;
			if(att == "Vertical")
				return LinearGradientMode.Vertical;
			return defaultValue;
		}
		SkinImageStretch GetSkinImageStretchValue(string name, SkinImageStretch defaultValue) {
			string att = Document.GetAttribute(name);
			if(att == null)
				return defaultValue;
			if(att == "NoResize")
				return SkinImageStretch.NoResize;
			if(att == "Stretch")
				return SkinImageStretch.Stretch;
			if(att == "Tile")
				return SkinImageStretch.Tile;
			return defaultValue;
		}
		SkinImageLayout GetSkinImageLayoutValue(string name, SkinImageLayout defaultValue) {
			string att = Document.GetAttribute(name);
			if(att == null)
				return defaultValue;
			if(att == "Horizontal")
				return SkinImageLayout.Horizontal;
			if(att == "Vertical")
				return SkinImageLayout.Vertical;
			return defaultValue;
		}
		Color GetSvgColorValue(out string name) {
			name = Document.GetAttribute("Name");
			return  GetColorValue("Value", Color.Empty);
		}
		Color GetColorValue(string name, Color defaultValue) {
			string att = Document.GetAttribute(name);
			if(att == null)
				return defaultValue;
			string val = att;
			if(val == "")
				return Color.Empty;
			if(val[0] == '$') {
				return CommonColors.GetSystemColor(val.Substring(1));
			}
			if(val.IndexOf(',') < 0) {
				return Color.FromName(val);
			}
			string[] split = val.Split(separators);
			switch (split.Length) {
				case 3:
					return Color.FromArgb(Int16.Parse(split[0], CultureInfo.InvariantCulture), 
						Int16.Parse(split[1], CultureInfo.InvariantCulture), Int16.Parse(split[2], CultureInfo.InvariantCulture));
				case 4:
					return Color.FromArgb(Int16.Parse(split[0], CultureInfo.InvariantCulture), Int16.Parse(split[1], CultureInfo.InvariantCulture), 
						Int16.Parse(split[2], CultureInfo.InvariantCulture), Int16.Parse(split[3], CultureInfo.InvariantCulture));
				default:
					return Color.Empty;
			}
		}
		Point GetPointValue(string name, Point defaultValue) {
			string att = Document.GetAttribute(name);
			return att == null ? defaultValue : SkinObjectConverter.IntStringToPoint(att);
		}
		Size GetSizeValue(string name, Size defaultValue) {
			string att = Document.GetAttribute(name);
			return att == null ? defaultValue : SkinObjectConverter.IntStringToSize(att);
		}
		string GetStringValue(string name, string defaultValue) {
			string att = Document.GetAttribute(name);
			return att == null ? defaultValue : att;
		}
		bool GetBoolValue(string name, bool defaultValue) {
			string att = Document.GetAttribute(name);
			return att == null ? defaultValue : bool.Parse(att);
		}
		int GetIntValue(string name, int defaultValue) {
			string att = Document.GetAttribute(name);
			return att == null ? defaultValue : int.Parse(att, CultureInfo.InvariantCulture);
		}
		protected virtual void LoadMargins(SkinPaddingEdges res) {
			res.Bottom = GetIntValue("Bottom", 0);
			res.Top = GetIntValue("Top", 0);
			res.Left = GetIntValue("Left", 0);
			res.Right = GetIntValue("Right", 0);
		}
		static char[] separators = new char[] { ',' };
		protected object StringToObject(Type type, string val) {
			if(type.Equals(typeof(SkinPaddingEdges))) {
				return String2SkinPaddingEdges(val);
			}
			else if(type.Equals(typeof(Color))) {
				if(val == "") return Color.Empty;
				if(val[0] == '$') {
					return CommonColors.GetSystemColor(val.Substring(1));
				}
				if(val.IndexOf(',') < 0) {
					return Color.FromName(val);
				}
				string[] split = val.Split(separators);
				switch(split.Length) {
					case 3:
						return Color.FromArgb(Convert.ToInt16(split[0]), Convert.ToInt16(split[1]), Convert.ToInt16(split[2]));
					case 4:
						return Color.FromArgb(Convert.ToInt16(split[0]),
							Convert.ToInt16(split[1]), Convert.ToInt16(split[2]), Convert.ToInt16(split[3]));
					default:
						return Color.Empty;
				}
			}
			if(type.IsEnum) return Enum.Parse(type, val);
			if(type.Equals(typeof(Point)) || type.Equals(typeof(Rectangle)) || type.Equals(typeof(Size))) {
				return SkinObjectConverter.IntStringToStructure(val, type);
			}
			return Convert.ChangeType(val, type, System.Globalization.CultureInfo.InvariantCulture);
		}
		private SkinPaddingEdges String2SkinPaddingEdges(string val) {
			int startIndex = val.IndexOf('(') + 1;
			int length = val.IndexOf(')') - startIndex;
			val = val.Substring(startIndex, length);
			if(val.Contains("All")) {
				String s = val.Substring(val.IndexOf("=") + 1);
				return new SkinPaddingEdges(int.Parse(s));
			}
			SkinPaddingEdges res = new SkinPaddingEdges();
			String[] values = val.Split(',');
			foreach(String s in values) {
				String[] nameValuePair = s.Split('=');
				PropertyInfo p = res.GetType().GetProperty(nameValuePair[0].Trim(), BindingFlags.Instance | BindingFlags.Public);
				p.SetValue(res, int.Parse(nameValuePair[1]), null);
			}
			return res;
		}
	}
	internal class SkinObjectConverter {
		public const string Base64Value = "~Xtra#Base64";
		static ColorConverter colorConverter = new ColorConverter();
		static BinaryFormatter formatter;
		static BinaryFormatter BinaryFormatter {
			get {
				if(formatter != null) return formatter;
				formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				return formatter;
			}
		}
		protected static TypeConverter GetToStringConverter(Type type) {
			TypeConverter tc = TypeDescriptor.GetConverter(type);
			if(tc.CanConvertTo(typeof(string)) && tc.CanConvertFrom(typeof(string))) return tc;
			return null;
		}
		public static string ObjectToString(object obj) {
			if(obj == null) return null;
			Type t = obj.GetType();
			if(t.Equals(typeof(Color))) return ColorToString((Color)obj);
			if(IsStructure(t)) return StructureToString(obj);
			TypeConverter tc = GetToStringConverter(t);
			if(tc != null) return tc.ConvertToInvariantString(obj);
			if(!t.IsSerializable) return null;
			MemoryStream ms = new MemoryStream();
			IFormatter formatter = BinaryFormatter;
			formatter.Serialize(ms, obj);
			return ToBase64String(ms.ToArray());
		}
		protected static string ToBase64String(byte[] array) {
			return string.Concat(Base64Value, Convert.ToBase64String(array));
		}
		protected static object Base64ToObject(string str, Type type) {
			byte[] array = Convert.FromBase64String(str.Substring(Base64Value.Length));
			return BinaryFormatter.Deserialize(new MemoryStream(array));
		}
		public static object StringToObject(string str, Type type) {
			if(str.StartsWith(Base64Value)) {
				return Base64ToObject(str, type);
			}
			if(type.Equals(typeof(int))) return StringToInt(str);
			if(type.Equals(typeof(Color))) return StringToColor(str);
			if(type.IsEnum) return EnumToObject(str, type);
			if(IsStructure(type)) return StringToStructure(str, type);
			TypeConverter tc = GetToStringConverter(type);
			if(tc != null) return Convert.ChangeType(tc.ConvertFromInvariantString(str), type, System.Globalization.CultureInfo.InvariantCulture);
			return Convert.ChangeType(str, type, System.Globalization.CultureInfo.InvariantCulture);
		}
		static int StringToInt(string str) {
			return int.Parse(str, CultureInfo.InvariantCulture);
		}
		static Color StringToColor(string str) {
			return (Color)colorConverter.ConvertFromInvariantString(str);
		}
		static string ColorToString(Color clr) {
			return colorConverter.ConvertToInvariantString(clr);
		}
		static Type[] structTypes = new Type[] { typeof(Point), typeof(PointF), typeof(Size), typeof(SizeF), typeof(Rectangle), typeof(RectangleF) };
		static bool IsStructure(Type t) {
			return Array.IndexOf(structTypes, t) != -1;
		}
		public static string IntStructureToString(object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			string res = "";
			foreach(PropertyDescriptor prop in props) {
				if(prop.IsReadOnly) continue;
				Type ttt = prop.PropertyType;
				if(!prop.PropertyType.IsPrimitive) continue;
				string val = ObjectToString(prop.GetValue(obj));
				if(val == null) val = "";
				res += String.Format("{2}{0}={1}", prop.Name, val, res.Length > 0 ? "," : string.Empty);
			}
			return res; 
		}
		static char[] separators = new char[] { ',', '=' };
		public static Size IntStringToSize(string str) {
			Size res = new Size();
			string[] strs = str.Split(separators);
			for(int n = 0; n < strs.Length; n += 2) {
				switch(strs[n]) {
					case "Height":
						res.Height = int.Parse(strs[n + 1], CultureInfo.InvariantCulture);
						break;
					case "Width":
						res.Width = int.Parse(strs[n + 1], CultureInfo.InvariantCulture);
						break;
				}
			}
			return res;
		}
		public static Point IntStringToPoint(string str) {
			Point res = new Point();
			string[] strs = str.Split(separators);
			for(int n = 0; n < strs.Length; n += 2) {
				switch(strs[n]) {
					case "X":
						res.X = int.Parse(strs[n + 1], CultureInfo.InvariantCulture);
						break;
					case "Y":
						res.Y = int.Parse(strs[n + 1], CultureInfo.InvariantCulture);
						break;
				}
			}
			return res;
		}
		public static object IntStringToStructure(string str, Type type) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(type);
			object res = Activator.CreateInstance(type);
			string[] strs = str.Split(separators);
			for(int n = 0; n < strs.Length; n += 2) {
				PropertyDescriptor prop = props[strs[n]];
				object realVal = StringToObject(strs[n + 1], prop.PropertyType);
				prop.SetValue(res, realVal);
			}
			return res;
		}
		static string StructureToString(object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			string res = "";
			foreach(PropertyDescriptor prop in props) {
				if(prop.IsReadOnly) continue;
				Type ttt = prop.PropertyType;
				if(!prop.PropertyType.IsPrimitive) continue;
				string val = ObjectToString(prop.GetValue(obj));
				if(val == null) val = "";
				res += String.Format("@{2},{0}={1}", prop.Name, val, val.Length);
			}
			return res; 
		}
		static object StringToStructure(string str, Type type) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(type);
			object res = Activator.CreateInstance(type);
			int n = 0;
			while(n < str.Length) {
				int start = str.IndexOf("@", n);
				if(start == -1) break;
				n = start + 1;
				string[] strs = str.Substring(n).Split(separators);
				if(strs == null || strs.Length < 2) break;
				int len = Convert.ToInt32(strs[0]);
				string fname = strs[1];
				start = n + str.Substring(n).IndexOf("=") + 1;
				PropertyDescriptor prop = props[fname];
				if(prop == null) continue;
				string val = str.Substring(start, len);
				n = start + len;
				object realVal = StringToObject(val, prop.PropertyType);
				prop.SetValue(res, realVal);
			}
			return res; 
		}
		static object EnumToObject(string str, Type type) {
			return Enum.Parse(type, str);
		}
	}
	public class SkinEmbeddedXmlCreator : SkinCreator, ISkinImageLoader {
		Assembly skinAssembly;
		string relativePath;
		public SkinEmbeddedXmlCreator(string skinName, string relativePath, Assembly skinAssembly)
			: base(skinName) {
			this.relativePath = relativePath;
			this.skinAssembly = skinAssembly;
		}
		public override SkinCreator Clone(string skinName) { return new SkinEmbeddedXmlCreator(skinName, RelativePath, SkinAssembly); }
		protected Assembly SkinAssembly { get { return skinAssembly; } }
		protected string SkinXml { get { return RelativePath + "skin.xml"; } }
		public string RelativePath {
			get { return relativePath; }
		}
		ResourceManager manager;
		public override void Load() {
			SetLoaded();
			this.manager = new ResourceManager("SkinData", SkinAssembly);
			string skinXml = (string)this.manager.GetObject(SkinXml);
			SkinXmlLoader loader = new SkinXmlLoader(this);
			loader.LoadAllSkinsFromText(skinXml, SkinName);
		}
		Image ISkinImageLoader.Load(SkinImage image, string imageName) {
			imageName = imageName.ToLower().Replace(@"\", "."); ;
			return (Image)this.manager.GetObject(RelativePath + imageName);
		}
	}
	public class SkinBlobXmlCreator : SkinXmlCreator {
		string sourceSkinName;
		public SkinBlobXmlCreator(string skinName, string relativePath) : this(skinName, relativePath, null, null) { }
		public SkinBlobXmlCreator(string skinName, string relativePath, Assembly skinAssembly, string skinXml) : base(skinName, relativePath, skinAssembly, skinXml) {
			this.sourceSkinName = skinName;
		}
		public override void Load() {
			LoadBlob();
			base.Load();
			this.images = null;
		}
		Hashtable images = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		protected virtual void LoadBlob() {
			Stream stream = SkinAssembly.GetManifestResourceStream(RelativePath + this.sourceSkinName.ToLower() + ".blob");
			if(stream == null) stream = SkinAssembly.GetManifestResourceStream(this.sourceSkinName.ToLower() + ".blob");
			this.images = SkinBlobLoader.LoadBlob(stream, false);
		}
		protected override string DefaultSkinXml { get { return sourceSkinName.ToLower() + ".skin.xml"; } }
		protected override Image LoadImage(SkinImage image, string imageName) {
			Stream res = this.images[imageName] as Stream;
			if(res != null) {
				image.SetRawData(res);
				return null;
			}
			System.Diagnostics.Debug.WriteLine("SkinEngine: " + imageName + " not found");
			return null;
		}
		public override SkinCreator Clone(string skinName) { 
			SkinBlobXmlCreator res = new SkinBlobXmlCreator(skinName, RelativePath, SkinAssembly, SkinXml);
			res.sourceSkinName = SkinName;
			return res;
		}
	}
	class SkinImageStream : Stream {
		Stream baseStream;
		long length;
		long offset;
		long position;
		public SkinImageStream(Stream baseStream, long offset, long length) {
			this.baseStream = baseStream;
			this.offset = offset;
			this.length = length;
		}
		public override bool CanRead {
			get { return true; }
		}
		public override bool CanSeek {
			get { return true; }
		}
		public override bool CanWrite {
			get { return false; }
		}
		public override void Flush() {
		}
		public override long Length {
			get { return length; }
		}
		public override long Position {
			get {
				return position;
			}
			set {
				if(value < 0)
					throw new Exception();
				position = value;
			}
		}
		public override int Read(byte[] buffer, int offset, int count) {
			lock(baseStream) {
				baseStream.Seek(this.offset + position, SeekOrigin.Begin);
				if(count > length - position)
					count = (int)(length - position);
				if(count <= 0)
					return 0;
				int read = baseStream.Read(buffer, offset, count);
				position += read;
				return read;
			}
		}
		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
			case SeekOrigin.Begin:
				if(offset < 0)
					throw new Exception();
				this.position = offset;
				break;
			case SeekOrigin.Current:
				if(this.offset + offset < 0)
					throw new Exception();
				this.position += offset;
				break;
			case SeekOrigin.End:
				if(this.length + offset < 0)
					throw new Exception();
				this.position = length + offset;
				break;
			}
			return position;
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
	}
	public class SkinXmlCreator : SkinCreator, ISkinImageLoader {
		Assembly skinAssembly;
		string skinXml, relativePath;
		public SkinXmlCreator(string skinName, string relativePath) : this(skinName, relativePath, null, null) { }
		public SkinXmlCreator(string skinName, string relativePath, Assembly skinAssembly, string skinXml)
			: base(skinName) {
			this.relativePath = relativePath;
			this.skinXml = skinXml;
			this.skinAssembly = skinAssembly;
		}
		public override SkinCreator Clone(string skinName) { return new SkinXmlCreator(skinName, RelativePath, SkinAssembly, SkinXml); }
		protected Assembly SkinAssembly {
			get {
				if(skinAssembly == null) skinAssembly = typeof(SkinCreator).Assembly;
				return skinAssembly;
			}
		}
		protected virtual string DefaultSkinXml { get { return "skin.xml"; } }
		protected string SkinXml {
			get { return skinXml == null ? DefaultSkinXml : skinXml; }
		}
		public string RelativePath {
			get { return relativePath; }
		}
		public override void Load() {
			SetLoaded();
			Stream stream = SkinAssembly.GetManifestResourceStream(RelativePath + SkinXml);
			if(stream == null) stream = SkinAssembly.GetManifestResourceStream(SkinXml);
			bool isCompressed = stream.ReadByte() == 0x1f && stream.ReadByte() == 0x8b;
			stream.Position = 0;
			if(isCompressed)
				stream = new GZipStream(stream, CompressionMode.Decompress);
			using(stream) {
				SkinXmlLoader loader = new SkinXmlLoader(this);
				loader.LoadAllSkins(stream, SkinName);
			}
		}
		Image ISkinImageLoader.Load(SkinImage image, string imageName) {
			return LoadImage(image, imageName);
		}
		protected virtual Image LoadImage(SkinImage image, string imageName) {
			imageName = imageName.ToLower().Replace(@"\", "."); ;
			return DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(RelativePath + imageName, SkinAssembly);
		}
	}
}
