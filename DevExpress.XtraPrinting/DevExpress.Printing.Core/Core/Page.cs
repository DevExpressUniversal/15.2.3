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
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
#if SL
using DevExpress.Xpf.Drawing.Printing;
using System.Windows.Media;
using DevExpress.Data.Browsing;
#else
using System.Drawing.Printing;
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting {
	using DevExpress.DocumentView;
	using DevExpress.Utils.StoredObjects;
#if SL
	public abstract class Page : CompositeBrick, IEnumerable, IXtraSerializable, IPage {
#else
	[
	BrickExporter(typeof(PageExporter))
	]
	public abstract class Page : BrickBase, IEnumerable, IXtraSerializable, IPage, IPageItem {
		static object synch = new object();
		static Int32 globalID;
		static Int32 GetGlobalID() {
			if(globalID < Int32.MaxValue)
				return globalID++;
			throw new InvalidOperationException();
		}
		int IPageItem.PageCount {
			get {
				return Document != null ? Document.PageCount : -1;
			}
		}
		void IPage.Draw(Graphics gr, PointF position) {
			if(Document != null && Document.PrintingSystem != null) {
				using(GdiGraphics gdiGraph = new GdiGraphics(gr, Document.PrintingSystem)) {
					PageExporter exporter = Document.PrintingSystem.ExportersFactory.GetExporter(this) as PageExporter;
					exporter.DrawPage(gdiGraph, PSUnitConverter.Convert(position, GraphicsUnit.Pixel, gdiGraph.PageUnit));
				}
			}
		}
		RectangleF IPage.UsefulPageRectF {
			get {
				return UsefulPageRectF;
			}
		}
		RectangleF IPage.ApplyMargins(RectangleF pageRect) {
			return PageData.GetMarginRect(pageRect);
		}
#endif
		internal const string DefaultPageNumberFormat = "{0}";
		List<BrickBase> innerBricks = new List<BrickBase>();
		ReadonlyPageData pageData;
		PageWatermark watermark;
		int originalIndex;
		int originalPageCount;
		float scaleFactor = 1f;
		bool loaded;
		int index = -1;
		internal override IList InnerBrickList {
			get { return innerBricks; }
		}
		internal bool Loaded { get { return loaded; } }		
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageInnerBricks"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.None)]
		public List<BrickBase> InnerBricks { get { return innerBricks; } }		
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PagePageSize")]
#endif
		public SizeF PageSize { get { return pageData.PageSize; } }
		internal RectangleF UsefulPageRectF { get { return pageData.UsefulPageRectF; } }
		internal Margins Margins { get { return pageData.Margins; } }
		internal ReadonlyPageData PageData { get { return pageData; } set { pageData = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageMarginsF")]
#endif
		public MarginsF MarginsF { get { return pageData.MarginsF; } }
		internal PageList Owner { get; set; }
		internal Int32 ID { get; private set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageDocument")]
#endif
		public Document Document {
			get {
				return Owner != null ? Owner.Document : null;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageIndex")]
#endif
		public int Index {
			get {
				if(Owner == null)
					return 0;
				if(index == -1)
					index = Owner.IndexOf(this);
				return index;
			}
		}
		internal string PageNumberFormat {
			get;
			set;
		}
		internal int PageNumberOffset {
			get;
			set;
		}
		internal PageInfo PageInfo {
			get;
			set;
		}
		internal string PageNumber {
			get {
				string pageNumberValue = ConvertPageNumberValueToString(Index + PageNumberOffset, PageInfo);
				try {
					return string.Format(PageNumberFormat, pageNumberValue);
				} catch(FormatException) {
					return string.Format(DefaultPageNumberFormat, pageNumberValue);
				}
			}
		}
		static string ConvertPageNumberValueToString(int pageNumberValue, PageInfo pageInfo) {
			switch(pageInfo) {
				case PageInfo.RomHiNumber:
					return PSConvert.ToRomanString(pageNumberValue);
				case PageInfo.RomLowNumber:
					return PSConvert.ToRomanString(pageNumberValue).ToLower();
				default:
					return pageNumberValue.ToString();
			}
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		]
		public int OriginalIndex { get { return originalIndex; } set { originalIndex = value; } }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		]
		public int OriginalPageCount { get { return originalPageCount; } set { originalPageCount = value; } }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		DefaultValue(1f),
		]
		public float ScaleFactor { get { return scaleFactor; } set { scaleFactor = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermark"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PageWatermark Watermark { get { return watermark; } }
		internal PageWatermark ActualWatermark {
			get {
				if(watermark != null)
					return watermark;
				if(Document == null || Document.PrintingSystem == null)
					return null;
				return (PageWatermark)Document.PrintingSystem.Watermark;
			}
		}
		protected Page(ReadonlyPageData pageData) {
			this.pageData = pageData;
			PageNumberFormat = DefaultPageNumberFormat;
			PageNumberOffset = 1;
			PageInfo = PageInfo.None;
			lock(synch) 
				ID = GetGlobalID();
		}
		protected Page() {
			PageNumberFormat = DefaultPageNumberFormat;
			PageNumberOffset = 1;
			PageInfo = PageInfo.None;
		}
		protected override void StoreValues(System.IO.BinaryWriter writer, IRepositoryProvider provider) {
			base.StoreValues(writer, provider);
			writer.WriteStoredObjects<BrickBase, BrickBase>(InnerBricks, provider);
			writer.WritePageData(pageData);
			writer.Write(originalIndex);
			writer.Write(originalPageCount);
			writer.Write(scaleFactor);
			writer.Write(ID);
		}
		protected override void RestoreValues(System.IO.BinaryReader reader, IRepositoryProvider provider) {
			base.RestoreValues(reader, provider);
			reader.ReadStoredObjects<BrickBase, BrickBase>(InnerBricks, provider);
			pageData = reader.ReadPageData();
			originalIndex = reader.ReadInt32();
			originalPageCount = reader.ReadInt32();
			scaleFactor = reader.ReadSingle();
			ID = reader.ReadInt32();
		}
		#region serialization
		protected override void SetIndexCollectionItemCore(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.InnerBricks)
				innerBricks.Add((BrickBase)e.Item.Value);
			base.SetIndexCollectionItemCore(propertyName, e);
		}
		protected override object CreateCollectionItemCore(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.InnerBricks)
				return new CompositeBrick();
			return base.CreateCollectionItemCore(propertyName, e);
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			switch(propertyName) {
				case PrintingSystemSerializationNames.OriginalIndex:
					return OriginalIndex != Index;
				case PrintingSystemSerializationNames.OriginalPageCount:
					return OriginalPageCount != Document.PageCount;
			}
			return base.ShouldSerializeCore(propertyName);
		}
		PageWatermark serializationWatermark;
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			serializationWatermark = new PageWatermark();
			AssignWatermark(serializationWatermark);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			if(serializationWatermark.Equals(watermark))
				AssignWatermark(null);
			serializationWatermark.Dispose();
			IncProgressReflector(this.Document);
			loaded = true;
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
			IncProgressReflector(this.Document);
		}
		static void IncProgressReflector(Document document) {
			if(document != null)
				document.ProgressReflector.RangeValue++;
		}
		#endregion
		internal void IvalidateIndex() {
			index = -1;
		}
		internal void PerformLayout(IPrintingSystemContext context) {
			foreach(Brick brick in this) {
				brick.PerformLayout(context);
			}
		}
		protected internal virtual CompositeBrick CreateWrapperBrick(IList<Brick> innerBricks, PointF offset) {			
			return new CompositeBrick(innerBricks, offset);
		}
		public RectangleF GetBrickBounds(Brick brick) {
			NestedBrickIterator iterator = new NestedBrickIterator(this.InnerBrickList);
			while(iterator.MoveNext()) {
				if(iterator.CurrentBrick == brick)
					return iterator.CurrentBrickRectangle;
			}
			return RectangleF.Empty;
		}
		public void AssignWatermark(PageWatermark source) {
			if(source == null) {
				DisposeWatermark();
			} else {
				if(watermark == null)
					watermark = new PageWatermark();
				watermark.CopyFromInternal(source);
			}
		}
		internal void AssignWatermarkReference(PageWatermark watermark) {
			DisposeWatermark();
			this.watermark = watermark;
		}
		internal void Initialize() {
			originalIndex = Index;
			originalPageCount = Document.PageCount;
			scaleFactor = Document.ScaleFactor;
		}
		void DisposeWatermark() {
			if(watermark != null) {
				watermark.Dispose();
				watermark = null;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerBricks.GetEnumerator();
		}
		public BrickEnumerator GetEnumerator() {
			return new BrickEnumerator(this);
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	using DevExpress.XtraPrinting.Native.Enumerators;
	public static class PageExtension {
		public static RectangleF GetRect(this Page page, PointF offset) {
			return new RectangleF(offset, page.PageSize);
		}
		public static RectangleF DeflateMinMargins(this Page page, RectangleF rect) {
			return RectHelper.DeflateRect(rect, page.PageData.MinMarginsF);
		}
	}
	public class BrickNavigator {
		BrickBase brick;
		Func<BrickBase, PointF, IIndexedEnumerator> method;
		public PointF BrickPosition { get; set; }
		public RectangleF ClipRect { get; set; }
		public BrickNavigator(BrickBase brick, Func<BrickBase, PointF, IIndexedEnumerator> method) {
			this.brick = brick;
			this.method = method;
			BrickPosition = PointF.Empty;
			ClipRect = RectangleF.Empty;
		}
		public virtual void IterateBricks(Func<Brick, RectangleF, RectangleF, bool> predicate) {
			NestedBrickIterator iterator = new NestedBrickIterator(method(brick, BrickPosition), method) { Offset = BrickPosition, ClipRect = ClipRect };
			while(iterator.MoveNext()) {
				Brick currentBrick = iterator.CurrentBrick as Brick;
				if(currentBrick != null && !(currentBrick is EmptyBrick) && currentBrick.IsVisible) {
					if(predicate(currentBrick, iterator.CurrentBrickRectangle, iterator.CurrentClipRectangle))
						break;
				}
			}
		}
		public Tuple<Brick, RectangleF> FindBrick(PointF pt) {
			float delta = GraphicsUnitConverter.PixelToDoc(1);
			Tuple<Brick, RectangleF> result = null;
			IterateBricks((brick, brickRect, clipRect) => {
				RectangleF brickRect2 = RectangleF.Inflate(brickRect, -delta, -delta);
				if(brick.AllowHitTest && brickRect2.Contains(pt) && clipRect.Contains(pt)) {
					result = new Tuple<Brick,RectangleF>(brick, brickRect);
					return true;
				}
				return false;
			});
			return result;
		}
	}
}
