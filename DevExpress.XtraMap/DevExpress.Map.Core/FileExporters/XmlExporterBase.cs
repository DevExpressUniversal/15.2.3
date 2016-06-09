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
using System.Globalization;
using System.IO;
using System.Xml;
#if DXPORTABLE
using DevExpress.Compatibility.System.Drawing.Imaging;
#else
using System.Drawing.Imaging;
#endif
namespace DevExpress.Map.Native {
	public abstract class XmlExporterBase<TItem> where TItem : IMapItemCore {
		protected internal const string CoordinatesSeparator = " ";
		readonly CultureInfo pointsExportCulture;
		readonly ImageFormat imageExportFormat;
		string workingPath;
		int saveImageIndex;
		protected internal XmlDocument Doc { get; set; }
		protected internal XmlElement DocumentNode { get; set; }
		protected internal CultureInfo PointsExportCulture { get { return pointsExportCulture; } }
		protected XmlExporterBase() {
			pointsExportCulture = CultureInfo.InvariantCulture;
			imageExportFormat = ImageFormat.Png;
		}
		string NextImageName(string formatExt) {
			saveImageIndex++;
			return string.Format("image_{0}.{1}", saveImageIndex, formatExt);
		}
		protected string GetExportImageHref(IMapPointerStyleCore item) {
			if(item.ImageUri != null)
				return item.ImageUri.ToString();
			if(item.Image == null || String.IsNullOrEmpty(workingPath))
				return String.Empty;
			string imageName = NextImageName(imageExportFormat.ToString().ToLower());
			string imagePath = Path.Combine(workingPath, imageName);
			using(FileStream imageStream = new FileStream(imagePath, FileMode.Create)) {
				item.Image.Save(imageStream, imageExportFormat);
			}
			return imageName;
		}
		protected IEnumerable<CoordPoint> GetPolygonPoints(IPointContainerCore pointsContainer) {
			for (int i = 0; i < pointsContainer.PointCount; i++)
				yield return pointsContainer.GetPoint(i);
		}
		protected XmlElement CreateElement(string name) {
			return Doc.CreateElement(name);
		}
		protected void AddElementToParent(XmlNode parent, XmlElement element) {
			parent.AppendChild(element);
		}
		protected XmlElement AppendNewElementToParent(XmlNode parent, string name) {
			XmlElement element = CreateElement(name);
			AddElementToParent(parent, element);
			return element;
		}
		protected bool IsEmptyElement(XmlElement element) {
			return element.ChildNodes.Count <= 0 && element.Attributes.Count <= 0;
		}
		protected internal virtual string PointToString(CoordPoint point) {
			return this.PointToString(point, ",");
		}
		protected internal virtual string PointToString(CoordPoint point, string pointsSeparator) {
			return String.Format("{0}{1}{2}", point.GetX().ToString(PointsExportCulture), pointsSeparator, point.GetY().ToString(PointsExportCulture));
		}
		protected virtual void ExportCore(XmlDocument userDoc, IEnumerable<TItem> items) {
			saveImageIndex = 0;
			Doc = userDoc;
			XmlDeclaration xmlDeclaration = Doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			Doc.AppendChild(xmlDeclaration);
		}
		public void Export(Stream stream, IEnumerable<TItem> items) {
			ExportCore(new XmlDocument(), items);
			Stream tempStream = new MemoryStream();
			using (BinaryWriter writer = new BinaryWriter(tempStream)) {
				Doc.Save(tempStream);
				tempStream.Seek(0, SeekOrigin.Begin);
				tempStream.CopyTo(stream);
			}
		}
		public void Export(string filePath, IEnumerable<TItem> items) {
			workingPath = new FileInfo(filePath).Directory.FullName;
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
				Export(fileStream, items);
			}
			workingPath = null;
		}
	}
}
