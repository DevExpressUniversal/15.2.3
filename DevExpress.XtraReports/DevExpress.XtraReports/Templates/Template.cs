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
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraReports.Templates {
	public class Author : ICloneable{
		Guid id = Guid.NewGuid();
		public Guid ID {
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		public string Name { get; set; }
		public object Clone() {
			return this.MemberwiseClone();
		}
	}
	public class Template : ICloneable {
		public static Template CreateTemplateFromArchive(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			Template template = new Template();
			InternalZipFileCollection filesCollection = InternalZipArchive.Open(stream);
			InternalZipFile manifest = filesCollection.First<InternalZipFile>(zipFile => {
				return zipFile.FileName == "manifest.xml";
			});
			if(manifest == null)
				throw new Exception("Wrong template structure");
			string xmlString = System.Text.Encoding.UTF8.GetString(ReadBytes(manifest));
			XmlNode node = GetTemplateNode(xmlString);
			FillTemplate(template, node);
			string layoutFile = GetNodeValue(node, "file");
			filesCollection.ForEach(zipFile => {
				if(zipFile.FileName == layoutFile) {
					template.LayoutBytes = ReadBytes(zipFile);
				} else if(zipFile.FileName == "icon.png") {
					template.IconBytes = ReadBytes(zipFile);
				} else if(zipFile.FileName == "preview.png") {
					template.PreviewBytes = ReadBytes(zipFile);
				}
			});
			return template;
		}
		static XmlNode GetTemplateNode(string xmlString) {
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlString);
			return xmlDoc.GetElementsByTagName("Template")[0];
		}
		static byte[] ReadBytes(InternalZipFile zipFile) {
			using(Stream stream = zipFile.FileDataStream) {
				int uncompressedSize = (int)zipFile.UncompressedSize;
				byte[] bytes = new byte[uncompressedSize];
				stream.Read(bytes, 0, uncompressedSize);
				return bytes;
			}
		}
		static void FillTemplate(Template template, XmlNode nodeItem) {
			template.Name = System.IO.Path.GetFileNameWithoutExtension(GetNodeValue(nodeItem, "file"));
			template.Description = nodeItem.FirstChild != null ? nodeItem.FirstChild.Value : null;
			string author = GetNodeValue(nodeItem, "author");
			template.Author = author != null ? new Author() { Name = author } : null;
		}
		static string GetNodeValue(XmlNode node, string name) {
			return node.Attributes[name].Value;
		}
		Image icon;
		byte[] iconBytes;
		Image preview;
		byte[] previewBytes;
		byte[] layoutBytes;
		Guid id = Guid.NewGuid();
		public Author Author { get; set; }
		public byte[] LayoutBytes {
			get {
				return layoutBytes;
			}
			set {
				layoutBytes = value;
			}
		}
		public string Description { get; set; }
		public Guid ID {
			get {					
				return id;
			}
			set {
				id = value;
			}
		}
		public string Name { get; set; }
		public int Rating { get; set; }
		public Image Icon {
			get {
				if(icon == null && IconBytes != null) {
					icon = Image.FromStream(new MemoryStream(IconBytes));
				}
				return icon;
			}
		}
		public byte[] IconBytes {
			get {
				return iconBytes;
			}
			set {
				icon = null;
				iconBytes = value;
			}
		}
		public Image Preview {
			get {
				if (preview == null && previewBytes != null) {
					preview = Image.FromStream(new MemoryStream(previewBytes));
				}
				return preview;
			}
		}
		public byte[] PreviewBytes {
			get {
				return previewBytes;
			}
			set {
				preview = null;
				previewBytes = value;
			}
		}
		public object Clone() {
			return this.MemberwiseClone();
		}
	}
}
