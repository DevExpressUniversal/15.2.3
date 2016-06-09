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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Drawing;
#if SL
using System.Windows.Media;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class DocumentSerializationOptions : IXtraSortableProperties, IXtraSupportShouldSerialize, IXtraSupportDeserializeCollectionItem {
		public static BookmarkNode GetRootBookmark(Document document) {
			return document.RootBookmark;
		}
		public static void AddImageEntryToCache(XtraItemEventArgs e) {
			((Document)e.RootObject).ImagesSerializationCache.AddDeserializationObject(e.Item.Value, e);
		}
		int pageCount;
		Document document;
		public EventHandler<ShouldSerializeEventArgs> ShouldSerialize;
		bool? RaiseShouldSerialize(string propertyName) {
			if(ShouldSerialize != null) {
				ShouldSerializeEventArgs e = new ShouldSerializeEventArgs(propertyName);
				ShouldSerialize(this, e);
				return e.ShouldSerialize;
			}
			return null;
		}
		public EventHandler PageCountChanged;
		void RaisePageCountChanged() {
			if(PageCountChanged != null)
				PageCountChanged(this, EventArgs.Empty);
		}
		[XtraSerializableProperty(DocumentSerializationOrders.Name)]
		public string Name { get { return document.Name; } set { document.Name = value; } }
		[XtraSerializableProperty(DocumentSerializationOrders.PageCount)]
		public int PageCount { 
			get { return pageCount; }
			set {
				this.pageCount = value;
				RaisePageCountChanged();
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, DocumentSerializationOrders.Styles, XtraSerializationFlags.Cached)]
		public ICollection SharedStyles { get { return document.StylesSerializationCache.SharedObjectsCollection; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, DocumentSerializationOrders.Images, XtraSerializationFlags.Cached | XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex)]
		public ICollection SharedImages { get { return document.ImagesSerializationCache.SharedObjectsCollection; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, DocumentSerializationOrders.SharedBricks, XtraSerializationFlags.Cached)]
		public ICollection SharedBricks { get { return document.BricksSerializationCache.SharedObjectsCollection; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content, DocumentSerializationOrders.Watermark)]
		public Watermark Watermark { get { return document.PrintingSystem.Watermark; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content, DocumentSerializationOrders.RootBookmark)]
		public BookmarkNode RootBookmark { get { return document.RootBookmark; } }
		[XtraSerializableProperty(DocumentSerializationOrders.PageBackColor)]
		public Color PageBackColor { get { return document.PrintingSystem.Graph.PageBackColor; } set { document.PrintingSystem.Graph.PageBackColor = value; } }
		[
		XtraSerializableProperty(DocumentSerializationOrders.ContinuousPageNumbering),
		DefaultValue(PrintingSystemBase.ContinuousPageNumberingDefaultValue),
		]
		public bool ContinuousPageNumbering { get { return document.PrintingSystem.ContinuousPageNumbering; } set { document.PrintingSystem.ContinuousPageNumbering = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, DocumentSerializationOrders.PageData, XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex)]
		public ICollection PageData { get { return document.PageData; } }
		public DocumentSerializationOptions(Document document)
			: this(document, document.PageCount) {
		}
		public DocumentSerializationOptions(Document document, int pageCount) {
			this.document = document;
			this.pageCount = pageCount;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.SharedImages)
				AddImageEntryToCache(e);
			if(propertyName == PrintingSystemSerializationNames.PageData) {
				PageDataWithIndices data = (PageDataWithIndices)e.Item.Value;
				foreach(int index in BrickPagePairHelper.ParseIndices(data.PageIndices)) {
					document.Pages[index].PageData = data;
				}
			}
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.SharedBricks)
				return BrickFactory.CreateBrick(e);
			if(propertyName == PrintingSystemSerializationNames.SharedImages)
				return new ImageEntry();
			if(propertyName == PrintingSystemSerializationNames.SharedStyles)
				return BrickFactory.CreateBrickStyle(e);
			if(propertyName == PrintingSystemSerializationNames.PageData)
				return new PageDataWithIndices();
			return null;
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			bool? result = RaiseShouldSerialize(propertyName);
			if(result.HasValue)
				return result.Value;
			switch(propertyName) {
				case PrintingSystemSerializationNames.SharedImages:
					return SharedImages.Count > 0;
				case PrintingSystemSerializationNames.PageBackColor:
					return PageBackColor != BrickGraphics.DefaultPageBackColor;
			}
			return true;
		}
		bool IXtraSortableProperties.ShouldSortProperties() {
			return true;
		}
		bool ShouldSerializePageBackColor() {
			return true;
		}
	}
}
