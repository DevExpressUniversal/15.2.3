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
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.DrawingML;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.Model;
namespace DevExpress.Office.Drawing {
	#region DrawingBlipFillInfo
	public class DrawingBlipFillInfo : ICloneable<DrawingBlipFillInfo>, ISupportsCopyFrom<DrawingBlipFillInfo>, ISupportsSizeOf {
		#region Fields
		const uint maskDpi = 0x0000ffff;
		const uint maskRotateWithShape = 0x00010000;
		const uint maskStretch = 0x00020000;
		uint packedValues = 0x0000;
		#endregion
		#region Properties
		public int Dpi {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskDpi); }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Dpi");
				PackedValues.SetIntBitValue(ref this.packedValues, maskDpi, value);
			}
		}
		public bool RotateWithShape { 
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskRotateWithShape); } 
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskRotateWithShape, value); } 
		}
		public bool Stretch { 
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskStretch); } 
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskStretch, value); } 
		}
		#endregion
		#region ICloneable<DrawingBlipFillInfo> Members
		public DrawingBlipFillInfo Clone() {
			DrawingBlipFillInfo result = new DrawingBlipFillInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingBlipFillInfo> Members
		public void CopyFrom(DrawingBlipFillInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingBlipFillInfo info = obj as DrawingBlipFillInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues;
		}
		public override int GetHashCode() {
			return (int)packedValues;
		}
	}
	#endregion
	#region DrawingBlipFillInfoCache
	public class DrawingBlipFillInfoCache : UniqueItemsCache<DrawingBlipFillInfo> {
		public DrawingBlipFillInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingBlipFillInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingBlipFillInfo();
		}
	}
	#endregion
	#region RectangleAlignType
	public enum RectangleAlignType {
		TopLeft = 0,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}
	#endregion
	#region TileFlipType
	public enum TileFlipType {
		None = 0,
		Horizontal,
		Vertical,
		Both
	}
	#endregion
	#region DrawingBlipTileInfo
	public class DrawingBlipTileInfo : ICloneable<DrawingBlipTileInfo>, ISupportsCopyFrom<DrawingBlipTileInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetFlip = 4;
		const uint maskAlign = 0x000f;
		const uint maskFlip = 0x0030;
		uint packedValues;
		int scaleX;
		int scaleY;
		long offsetX;
		long offsetY;
		#endregion
		#region Properties
		public RectangleAlignType TileAlign {
			get { return (RectangleAlignType)PackedValues.GetIntBitValue(this.packedValues, maskAlign); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskAlign, (int)value); }
		}
		public TileFlipType TileFlip {
			get { return (TileFlipType)PackedValues.GetIntBitValue(this.packedValues, maskFlip, offsetFlip); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskFlip, offsetFlip, (int)value); }
		}
		public int ScaleX {
			get { return scaleX; }
			set {
				ValueChecker.CheckValue(value, 0, DrawingValueConstants.ThousandthOfPercentage, "ScaleX");
				scaleX = value;
			}
		}
		public int ScaleY {
			get { return scaleY; }
			set {
				ValueChecker.CheckValue(value, 0, DrawingValueConstants.ThousandthOfPercentage, "ScaleY");
				scaleY = value;
			}
		}
		public long OffsetX {
			get { return offsetX; }
			set {
				offsetX = value;
			}
		}
		public long OffsetY {
			get { return offsetY; }
			set {
				offsetY = value;
			}
		}
		#endregion
		#region ICloneable<DrawingBlipTileInfo> Members
		public DrawingBlipTileInfo Clone() {
			DrawingBlipTileInfo result = new DrawingBlipTileInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingBlipTileInfo> Members
		public void CopyFrom(DrawingBlipTileInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.scaleX = value.scaleX;
			this.scaleY = value.scaleY;
			this.offsetX = value.offsetX;
			this.offsetY = value.offsetY;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingBlipTileInfo other = obj as DrawingBlipTileInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues &&
				this.scaleX == other.scaleX &&
				this.scaleY == other.scaleY &&
				this.offsetX == other.offsetX &&
				this.offsetY == other.offsetY;
		}
		public override int GetHashCode() {
			return (int)(packedValues ^ scaleX ^ scaleY ^ offsetX.GetHashCode() ^ offsetY.GetHashCode());
		}
	}
	#endregion
	#region DrawingBlipTileInfoCache
	public class DrawingBlipTileInfoCache : UniqueItemsCache<DrawingBlipTileInfo> {
		public DrawingBlipTileInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingBlipTileInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingBlipTileInfo();
		}
	}
	#endregion
	#region DrawingBlipFillInfoIndexAccessor
	public class DrawingBlipFillInfoIndexAccessor : IIndexAccessor<DrawingBlipFill, DrawingBlipFillInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<DrawingBlipFill, DrawingBlipFillInfo, DocumentModelChangeActions> Members
		public int GetIndex(DrawingBlipFill owner) {
			return owner.FillInfoIndex;
		}
		public int GetDeferredInfoIndex(DrawingBlipFill owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(DrawingBlipFill owner, int value) {
			owner.AssignFillInfoIndex(value);
		}
		public int GetInfoIndex(DrawingBlipFill owner, DrawingBlipFillInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public DrawingBlipFillInfo GetInfo(DrawingBlipFill owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(DrawingBlipFill owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<DrawingBlipFillInfo> GetInfoCache(DrawingBlipFill owner) {
			return owner.DrawingCache.DrawingBlipFillInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(DrawingBlipFill owner) {
			return new DrawingBlipFillInfoIndexChangeHistoryItem(owner);
		}
		public DrawingBlipFillInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DrawingBlipFillBatchUpdateHelper)helper).BlipFillInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, DrawingBlipFillInfo info) {
			((DrawingBlipFillBatchUpdateHelper)helper).BlipFillInfo = info.Clone();
		}
		public void InitializeDeferredInfo(DrawingBlipFill owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(DrawingBlipFill owner, DrawingBlipFill from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(DrawingBlipFill owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region DrawingBlipTileInfoIndexAccessor
	public class DrawingBlipTileInfoIndexAccessor : IIndexAccessor<DrawingBlipFill, DrawingBlipTileInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<DrawingBlipFill, DrawingBlipTileInfo, DocumentModelChangeActions> Members
		public int GetIndex(DrawingBlipFill owner) {
			return owner.TileInfoIndex;
		}
		public int GetDeferredInfoIndex(DrawingBlipFill owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(DrawingBlipFill owner, int value) {
			owner.AssignTileInfoIndex(value);
		}
		public int GetInfoIndex(DrawingBlipFill owner, DrawingBlipTileInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public DrawingBlipTileInfo GetInfo(DrawingBlipFill owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(DrawingBlipFill owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<DrawingBlipTileInfo> GetInfoCache(DrawingBlipFill owner) {
			return owner.DrawingCache.DrawingBlipTileInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(DrawingBlipFill owner) {
			return new DrawingBlipTileInfoIndexChangeHistoryItem(owner);
		}
		public DrawingBlipTileInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DrawingBlipFillBatchUpdateHelper)helper).BlipTileInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, DrawingBlipTileInfo info) {
			((DrawingBlipFillBatchUpdateHelper)helper).BlipTileInfo = info.Clone();
		}
		public void InitializeDeferredInfo(DrawingBlipFill owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(DrawingBlipFill owner, DrawingBlipFill from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(DrawingBlipFill owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region DrawingBlipFillBatchUpdateHelper
	public class DrawingBlipFillBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		#region Fields
		DrawingBlipFillInfo blipFillInfo;
		DrawingBlipTileInfo blipTileInfo;
		int suppressDirectNotificationsCount;
		#endregion
		public DrawingBlipFillBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public DrawingBlipFillInfo BlipFillInfo { get { return blipFillInfo; } set { blipFillInfo = value; } }
		public DrawingBlipTileInfo BlipTileInfo { get { return blipTileInfo; } set { blipTileInfo = value; } }
		public bool IsDirectNotificationsEnabled { get { return suppressDirectNotificationsCount == 0; } }
		public void SuppressDirectNotifications() {
			suppressDirectNotificationsCount++;
		}
		public void ResumeDirectNotifications() {
			suppressDirectNotificationsCount--;
		}
	}
	#endregion
	#region DrawingBlipFillBatchInitHelper
	public class DrawingBlipFillBatchInitHelper : DrawingBlipFillBatchUpdateHelper {
		public DrawingBlipFillBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region DrawingBlipFill
	public class DrawingBlipFill : DrawingMultiIndexObject<DrawingBlipFill, DocumentModelChangeActions>, ICloneable<DrawingBlipFill>, ISupportsCopyFrom<DrawingBlipFill>, IDrawingFill, IUnderlineFill {
		#region Static Members
		readonly static DrawingBlipFillInfoIndexAccessor fillInfoIndexAccessor = new DrawingBlipFillInfoIndexAccessor();
		readonly static DrawingBlipTileInfoIndexAccessor tileInfoIndexAccessor = new DrawingBlipTileInfoIndexAccessor();
		readonly static IIndexAccessorBase<DrawingBlipFill, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<DrawingBlipFill, DocumentModelChangeActions>[] {
			fillInfoIndexAccessor,
			tileInfoIndexAccessor
		};
		public static DrawingBlipFillInfoIndexAccessor FillInfoIndexAccessor { get { return fillInfoIndexAccessor; } }
		public static DrawingBlipTileInfoIndexAccessor TileInfoIndexAccessor { get { return tileInfoIndexAccessor; } }
		public static DrawingBlipFill Create(IDocumentModel documentModel, DrawingBlipFillInfo fillInfo, DrawingBlipTileInfo tileInfo) {
			DrawingBlipFill result = new DrawingBlipFill(documentModel);
			result.AssignInfoes(fillInfo, tileInfo);
			return result;
		}
		public static DrawingBlipFill Create(IDocumentModel documentModel, DrawingBlipFillInfo fillInfo) {
			return Create(documentModel, fillInfo, new DrawingBlipTileInfo());
		}
		public static DrawingBlipFill Create(IDocumentModel documentModel, OfficeImage image) {
			DrawingBlipFill result = new DrawingBlipFill(documentModel);
			result.Blip.Image = image;
			result.Stretch = true;
			return result;
		}
#if !SL
		public static DrawingBlipFill Create(IDocumentModel documentModel, string fileName) {
			DrawingBlipFill result = new DrawingBlipFill(documentModel);
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				result.Blip.Image = documentModel.CreateImage(stream);
			}
			result.Stretch = true;
			return result;
		}
#endif
		#endregion
		#region Fields
		readonly DrawingBlip blip;
		int fillInfoIndex;
		int tileInfoIndex;
		RectangleOffset sourceRectangle;
		RectangleOffset fillRectangle;
		#endregion
		public DrawingBlipFill(IDocumentModel documentModel)
			: base(documentModel) {
			this.blip = new DrawingBlip(documentModel) { Parent = InnerParent };
			this.sourceRectangle = RectangleOffset.Empty;
			this.fillRectangle = RectangleOffset.Empty;
		}
		#region Properties
		#region MultiIndex
		internal int FillInfoIndex { get { return fillInfoIndex; } }
		internal int TileInfoIndex { get { return tileInfoIndex; } }
		protected override IIndexAccessorBase<DrawingBlipFill, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal new DrawingBlipFillBatchUpdateHelper BatchUpdateHelper { get { return (DrawingBlipFillBatchUpdateHelper)base.BatchUpdateHelper; } }
		protected internal DrawingBlipFillInfo FillInfo { get { return IsUpdateLocked ? BatchUpdateHelper.BlipFillInfo : FillInfoCore; } }
		protected internal DrawingBlipTileInfo TileInfo { get { return IsUpdateLocked ? BatchUpdateHelper.BlipTileInfo : TileInfoCore; } }
		DrawingBlipFillInfo FillInfoCore { get { return fillInfoIndexAccessor.GetInfo(this); } }
		DrawingBlipTileInfo TileInfoCore { get { return tileInfoIndexAccessor.GetInfo(this); } }
		#endregion
		#region Dpi
		public int Dpi {
			get { return FillInfo.Dpi; }
			set {
				if (Dpi == value)
					return;
				SetPropertyValue(fillInfoIndexAccessor, SetDpiCore, value);
			}
		}
		DocumentModelChangeActions SetDpiCore(DrawingBlipFillInfo info, int value) {
			info.Dpi = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RotateWithShape
		public bool RotateWithShape {
			get { return FillInfo.RotateWithShape; }
			set {
				if (RotateWithShape == value)
					return;
				SetPropertyValue(fillInfoIndexAccessor, SetRotateWithShapeCore, value);
			}
		}
		DocumentModelChangeActions SetRotateWithShapeCore(DrawingBlipFillInfo info, bool value) {
			info.RotateWithShape = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Stretch
		public bool Stretch {
			get { return FillInfo.Stretch; }
			set {
				if (Stretch == value)
					return;
				SetPropertyValue(fillInfoIndexAccessor, SetStretchCore, value);
			}
		}
		DocumentModelChangeActions SetStretchCore(DrawingBlipFillInfo info, bool value) {
			info.Stretch = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Blip
		public DrawingBlip Blip { get { return blip; } }
		#endregion
		#region SourceRectangle
		public RectangleOffset SourceRectangle {
			get { return sourceRectangle; }
			set {
				if(!sourceRectangle.Equals(value))
					ApplyHistoryItem(new SourceRectangleHistoryItem(this, sourceRectangle, value));
			}
		}
		protected internal void SetSourceRectangleCore(RectangleOffset value) {
			sourceRectangle = value;
			InvalidateParent();
		}
		#endregion
		#region FillRectangle
		public RectangleOffset FillRectangle {
			get { return fillRectangle; }
			set {
				if(fillRectangle.Equals(value))
					return;
				SetFillRectangle(value);
			}
		}
		void SetFillRectangle(RectangleOffset value) {
			DocumentModel.History.BeginTransaction();
			try {
				ApplyHistoryItem(new FillRectangleHistoryItem(this, fillRectangle, value));
				if(!Equals(value, RectangleOffset.Empty))
					Stretch = true;
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		protected internal void SetFillRectangleCore(RectangleOffset value) {
			fillRectangle = value;
			InvalidateParent();
		}
		#endregion
		#region TileAlign
		public RectangleAlignType TileAlign {
			get { return TileInfo.TileAlign; }
			set {
				if(TileAlign == value)
					return;
				SetPropertyValue(tileInfoIndexAccessor, SetTileAlignCore, value);
			}
		}
		DocumentModelChangeActions SetTileAlignCore(DrawingBlipTileInfo info, RectangleAlignType value) {
			info.TileAlign = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TileFlip
		public TileFlipType TileFlip {
			get { return TileInfo.TileFlip; }
			set {
				if(TileFlip == value)
					return;
				SetPropertyValue(tileInfoIndexAccessor, SetTileFlipCore, value);
			}
		}
		DocumentModelChangeActions SetTileFlipCore(DrawingBlipTileInfo info, TileFlipType value) {
			info.TileFlip = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ScaleX
		public int ScaleX {
			get { return TileInfo.ScaleX; }
			set {
				if(ScaleX == value)
					return;
				SetPropertyValue(tileInfoIndexAccessor, SetScaleXCore, value);
			}
		}
		DocumentModelChangeActions SetScaleXCore(DrawingBlipTileInfo info, int value) {
			info.ScaleX = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ScaleY
		public int ScaleY {
			get { return TileInfo.ScaleY; }
			set {
				if(ScaleY == value)
					return;
				SetPropertyValue(tileInfoIndexAccessor, SetScaleYCore, value);
			}
		}
		DocumentModelChangeActions SetScaleYCore(DrawingBlipTileInfo info, int value) {
			info.ScaleY = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OffsetX
		public long OffsetX {
			get { return TileInfo.OffsetX; }
			set {
				if(OffsetX == value)
					return;
				SetPropertyValue(tileInfoIndexAccessor, SetOffsetXCore, value);
			}
		}
		DocumentModelChangeActions SetOffsetXCore(DrawingBlipTileInfo info, long value) {
			info.OffsetX = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OffsetY
		public long OffsetY {
			get { return TileInfo.OffsetY; }
			set {
				if(OffsetY == value)
					return;
				SetPropertyValue(tileInfoIndexAccessor, SetOffsetYCore, value);
			}
		}
		DocumentModelChangeActions SetOffsetYCore(DrawingBlipTileInfo info, long value) {
			info.OffsetY = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region MultiIndexManagement
		protected override void SetPropertyValueCore<TInfo, U>(IIndexAccessor<DrawingBlipFill, TInfo, DocumentModelChangeActions> indexHolder, MultiIndexObject<DrawingBlipFill, DocumentModelChangeActions>.SetPropertyValueDelegate<TInfo, U> setter, U newValue) {
			base.SetPropertyValueCore<TInfo, U>(indexHolder, setter, newValue);
		}
		public override DrawingBlipFill GetOwner() {
			return this;
		}
		internal void AssignFillInfoIndex(int value) {
			this.fillInfoIndex = value;
		}
		internal void AssignTileInfoIndex(int value) {
			this.tileInfoIndex = value;
		}
		internal void AssignInfoes(DrawingBlipFillInfo fillInfo, DrawingBlipTileInfo tileInfo) {
			AssignFillInfoIndex(DrawingCache.DrawingBlipFillInfoCache.AddItem(fillInfo));
			AssignTileInfoIndex(DrawingCache.DrawingBlipTileInfoCache.AddItem(tileInfo));
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new DrawingBlipFillBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new DrawingBlipFillBatchInitHelper(this);
		}
		#endregion
		#region ISupportsCopyFrom<DrawingBlipFill> Members
		public void CopyFrom(DrawingBlipFill value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.blip.CopyFrom(value.blip);
			SourceRectangle = value.SourceRectangle;
			FillRectangle = value.FillRectangle;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingBlipFill other = obj as DrawingBlipFill;
			if (other == null)
				return false;
			if (other.FillType != DrawingFillType.Picture)
				return false;
			DrawingBlipFill fill = other as DrawingBlipFill;
			return base.Equals(fill) &&
				blip.Equals(fill.blip) &&
				sourceRectangle.Equals(fill.sourceRectangle) &&
				fillRectangle.Equals(fill.fillRectangle);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ blip.GetHashCode() ^ sourceRectangle.GetHashCode() ^ fillRectangle.GetHashCode();
		}
		#endregion
		#region IDrawingFill Members
		public DrawingFillType FillType { get { return DrawingFillType.Picture; } }
		public IDrawingFill CloneTo(IDocumentModel documentModel) {
			DrawingBlipFill result = new DrawingBlipFill(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingFillVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<DrawingBlipFill> Members
		public DrawingBlipFill Clone() {
			DrawingBlipFill result = new DrawingBlipFill(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region IUnderlineFill Members
		DrawingUnderlineFillType IUnderlineFill.Type { get { return DrawingUnderlineFillType.Fill; } }
		IUnderlineFill IUnderlineFill.CloneTo(IDocumentModel documentModel) {
			return CloneTo(documentModel) as IUnderlineFill;
		}
		#endregion
	}
	#endregion
}
