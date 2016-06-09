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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.Model;
using System.Collections.Generic;
using DevExpress.Office.DrawingML;
namespace DevExpress.Office.Drawing {
	#region DrawingTextAlignmentType
	public enum DrawingTextAlignmentType {
		Left = 0, 
		Center = 1,
		Right = 2, 
		Justified = 3, 
		JustifiedLow = 4, 
		Distributed = 5, 
		ThaiDistributed = 6
	}
	#endregion
	#region DrawingFontAlignmentType
	public enum DrawingFontAlignmentType {
		Automatic = 0,
		Top = 1,
		Center = 2,
		Baseline = 3,
		Bottom = 4,
	}
	#endregion
	#region DrawingTextParagraphInfo
	public class DrawingTextParagraphInfo : ICloneable<DrawingTextParagraphInfo>, ISupportsCopyFrom<DrawingTextParagraphInfo>, ISupportsSizeOf {
		#region Static Members
		public static DrawingTextParagraphInfo DefaultInfo { get { return new DrawingTextParagraphInfo(); } }
		#endregion
		#region Fields
		public const int DefaultLeftMargin = 347663;
		public const int DefaultIndent = -342900;
		const int MaxTextIndentLevelType = 8;
		const uint MaskTextAlignment = 0x00000007;				  
		const uint MaskEastAsianLineBreak = 0x00000008;			 
		const uint MaskFontAlignment = 0x00000070;				  
		const uint MaskHangingPunctuation = 0x00000080;			 
		const uint MaskTextIndentLevel = 0x00000F00;				
		const uint MaskRightToLeft = 0x00001000;					
		const uint MaskLatinLineBreak = 0x00002000;				 
		const uint MaskHasDefaultTabSize = 0x00004000;			  
		const uint MaskHasTextIndentLevel = 0x00008000;			 
		const uint MaskHasTextAlignment = 0x00010000;			   
		const uint MaskHasEastAsianLineBreak = 0x00020000;		  
		const uint MaskHasFontAlignment = 0x00040000;			   
		const uint MaskHasHangingPunctuation = 0x00080000;		  
		const uint MaskHasRightToLeft = 0x00100000;				 
		const uint MaskHasLatinLineBreak = 0x00200000;			  
		const uint MaskHasIndent = 0x00400000;					  
		const uint MaskHasLeftMargin = 0x00800000;				  
		const uint MaskHasRightMargin = 0x01000000;				 
		const uint MaskApplyDefaultCharacterProperties = 0x02000000;  
		uint packedValues = 0x00002238;
		int defaultTabSize;
		int indent = DefaultIndent;
		int leftMargin = DefaultLeftMargin;
		int rightMargin;
		#endregion
		#region Properties
		public int DefaultTabSize { get { return defaultTabSize; } set { defaultTabSize = value; } }
		public int Indent { get { return indent; } set { indent = value; } }
		public int LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public DrawingTextAlignmentType TextAlignment {
			get { return (DrawingTextAlignmentType)PackedValues.GetIntBitValue(this.packedValues, MaskTextAlignment); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, MaskTextAlignment, (int)value); }
		}
		public DrawingFontAlignmentType FontAlignment {
			get { return (DrawingFontAlignmentType)PackedValues.GetIntBitValue(this.packedValues, MaskFontAlignment, 4); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, MaskFontAlignment, 4, (int)value); }
		}
		public int TextIndentLevel {
			get { return PackedValues.GetIntBitValue(this.packedValues, MaskTextIndentLevel, 8) - 2; }
			set { PackedValues.SetIntBitValue(ref this.packedValues, MaskTextIndentLevel, 8, value + 2); }
		}
		public bool EastAsianLineBreak {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskEastAsianLineBreak); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskEastAsianLineBreak, value); }
		}
		public bool LatinLineBreak {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskLatinLineBreak); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskLatinLineBreak, value); }
		}
		public bool HangingPunctuation {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHangingPunctuation); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHangingPunctuation, value); }
		}
		public bool RightToLeft {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskRightToLeft); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskRightToLeft, value); }
		}
		public bool HasDefaultTabSize {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasDefaultTabSize); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasDefaultTabSize, value); }
		}
		public bool HasTextIndentLevel {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasTextIndentLevel); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasTextIndentLevel, value); }
		}
		public bool HasTextAlignment {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasTextAlignment); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasTextAlignment, value); }
		}
		public bool HasEastAsianLineBreak {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasEastAsianLineBreak); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasEastAsianLineBreak, value); }
		}
		public bool HasFontAlignment {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasFontAlignment); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasFontAlignment, value); }
		}
		public bool HasHangingPunctuation {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasHangingPunctuation); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasHangingPunctuation, value); }
		}
		public bool HasRightToLeft {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasRightToLeft); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasRightToLeft, value); }
		}
		public bool HasLatinLineBreak {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasLatinLineBreak); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasLatinLineBreak, value); }
		}
		public bool HasIndent {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasIndent); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasIndent, value); }
		}
		public bool HasLeftMargin {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasLeftMargin); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasLeftMargin, value); }
		}
		public bool HasRightMargin {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskHasRightMargin); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskHasRightMargin, value); }
		}
		public bool ApplyDefaultCharacterProperties {
			get { return PackedValues.GetBoolBitValue(this.packedValues, MaskApplyDefaultCharacterProperties); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, MaskApplyDefaultCharacterProperties, value); }
		}
		#endregion
		#region ICloneable<DrawingTextParagraphInfo> Members
		public DrawingTextParagraphInfo Clone() {
			DrawingTextParagraphInfo result = new DrawingTextParagraphInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextParagraphInfo> Members
		public void CopyFrom(DrawingTextParagraphInfo value) {
			packedValues = value.packedValues;
			defaultTabSize = value.defaultTabSize;
			indent = value.indent;
			leftMargin = value.leftMargin;
			rightMargin = value.rightMargin;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextParagraphInfo other = obj as DrawingTextParagraphInfo;
			if (other == null)
				return false;
			return this.packedValues == other.packedValues &&
				this.defaultTabSize == other.defaultTabSize &&
				this.indent == other.indent &&
				this.leftMargin == other.leftMargin &&
				this.rightMargin == other.rightMargin;
		}
		public override int GetHashCode() {
			return 
				(int)this.packedValues ^ defaultTabSize ^ indent ^ leftMargin ^ rightMargin;
		}
		#endregion
	}
	#endregion
	#region DrawingTextParagraphInfoCache
	public class DrawingTextParagraphInfoCache : UniqueItemsCache<DrawingTextParagraphInfo> {
		public const int DefaultItemIndex = 0;
		public DrawingTextParagraphInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingTextParagraphInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingTextParagraphInfo();
		}
	}
	#endregion
	#region DrawingTextParagraphInfoIndexAccessor
	public class DrawingTextParagraphInfoIndexAccessor : IIndexAccessor<DrawingTextParagraphProperties, DrawingTextParagraphInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<DrawingTextParagraphProperties, DrawingTextParagraphInfo, DocumentModelChangeActions> Members
		public int GetIndex(DrawingTextParagraphProperties owner) {
			return owner.TextParagraphInfoIndex;
		}
		public int GetDeferredInfoIndex(DrawingTextParagraphProperties owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(DrawingTextParagraphProperties owner, int value) {
			owner.AssignTextParagraphInfoIndex(value);
		}
		public int GetInfoIndex(DrawingTextParagraphProperties owner, DrawingTextParagraphInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public DrawingTextParagraphInfo GetInfo(DrawingTextParagraphProperties owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(DrawingTextParagraphProperties owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<DrawingTextParagraphInfo> GetInfoCache(DrawingTextParagraphProperties owner) {
			return owner.DrawingCache.DrawingTextParagraphInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(DrawingTextParagraphProperties owner) {
			return new DrawingTextParagraphInfoIndexChangeHistoryItem(owner);
		}
		public DrawingTextParagraphInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DrawingTextParagraphPropertiesBatchUpdateHelper)helper).TextParagraphInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, DrawingTextParagraphInfo info) {
			((DrawingTextParagraphPropertiesBatchUpdateHelper)helper).TextParagraphInfo = info.Clone();
		}
		public void InitializeDeferredInfo(DrawingTextParagraphProperties owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(DrawingTextParagraphProperties owner, DrawingTextParagraphProperties from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(DrawingTextParagraphProperties owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region DrawingTextSpacingPropertyChangeManager
	public class DrawingTextSpacingPropertyChangeManager : IDrawingTextSpacing {
		int index;
		readonly IDrawingTextSpacingsChanger info;
		public DrawingTextSpacingPropertyChangeManager(IDrawingTextSpacingsChanger info) {
			Guard.ArgumentNotNull(info, "ITextSpacingsChanger");
			this.info = info;
		}
		#region Properties
		public DrawingTextSpacingValueType Type {
			get { return info.GetType(index); }
			set { info.SetType(index, value); }
		}
		public int Value {
			get { return info.GetValue(index); }
			set { info.SetValue(index, value); }
		}
		#endregion
		protected internal IDrawingTextSpacing GetFormatInfo(int index) {
			this.index = index;
			return this;
		}
	}
	#endregion
	#region DrawingTextSpacingInfoIndexAccessor
	public class DrawingTextSpacingInfoIndexAccessor : IIndexAccessor<DrawingTextParagraphProperties, DrawingTextSpacingInfo, DocumentModelChangeActions> {
		int index;
		public DrawingTextSpacingInfoIndexAccessor(int index) {
			this.index = index;
		}
		#region IIndexAccessor<DrawingTextParagraphProperties, DrawingTextSpacingInfo, DocumentModelChangeActions> Members
		public int GetIndex(DrawingTextParagraphProperties owner) {
			return owner.TextSpacingInfoIndexes[index];
		}
		public int GetDeferredInfoIndex(DrawingTextParagraphProperties owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(DrawingTextParagraphProperties owner, int value) {
			owner.AssignTextSpacingInfoIndex(index, value);
		}
		public int GetInfoIndex(DrawingTextParagraphProperties owner, DrawingTextSpacingInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public DrawingTextSpacingInfo GetInfo(DrawingTextParagraphProperties owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(DrawingTextParagraphProperties owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<DrawingTextSpacingInfo> GetInfoCache(DrawingTextParagraphProperties owner) {
			return owner.DrawingCache.DrawingTextSpacingInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(DrawingTextParagraphProperties owner) {
			return new DrawingTextSpacingInfoIndexChangeHistoryItem(owner, index);
		}
		public DrawingTextSpacingInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DrawingTextParagraphPropertiesBatchUpdateHelper)helper).TextSpacingInfoes[index];
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, DrawingTextSpacingInfo info) {
			((DrawingTextParagraphPropertiesBatchUpdateHelper)helper).TextSpacingInfoes[index] = info.Clone();
		}
		public void InitializeDeferredInfo(DrawingTextParagraphProperties owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(DrawingTextParagraphProperties owner, DrawingTextParagraphProperties from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(DrawingTextParagraphProperties owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region IDrawingTextMargin
	public interface IDrawingTextMargin {
		int Left { get; set; }
		int Right { get; set; }
	}
	#endregion
	#region IDrawingTextSpacings
	public interface IDrawingTextSpacings {
		IDrawingTextSpacing Line { get; }
		IDrawingTextSpacing SpaceAfter { get; }
		IDrawingTextSpacing SpaceBefore { get; }
	}
	#endregion 
	#region IDrawingTextBullets
	public interface IDrawingTextBullets {
		IDrawingBullet Common { get; set; }
		IDrawingBullet Color { get; set; }
		IDrawingBullet Typeface { get; set; }
		IDrawingBullet Size { get; set; }
	}
	#endregion 
	#region IDrawingTextSpacingsChanger
	public interface IDrawingTextSpacingsChanger {
		#region Type
		DrawingTextSpacingValueType GetType(int index);
		void SetType(int index, DrawingTextSpacingValueType value);
		#endregion
		#region Value
		int GetValue(int index);
		void SetValue(int index, int value);
		#endregion
	}
	#endregion
	#region IDrawingTextParagraphProperties
	public interface IDrawingTextParagraphProperties : IDrawingTextSpacingsChanger, IDrawingTextSpacings, IDrawingTextMargin, IDrawingTextBullets, IDrawingTextParagraphPropertiesOptions {
		IDrawingTextSpacings Spacings { get; }
		IDrawingTextMargin Margin { get; }
		IDrawingTextBullets Bullets { get; }
		DrawingTextCharacterProperties DefaultCharacterProperties { get; }
		bool ApplyDefaultCharacterProperties { get; set; }
		DrawingTextAlignmentType TextAlignment { get; set; }
		DrawingFontAlignmentType FontAlignment { get; set; }
		int TextIndentLevel { get; set; }
		int DefaultTabSize { get; set; }
		int Indent { get; set; }
		bool RightToLeft { get; set; }
		bool HangingPunctuation { get; set; }
		bool EastAsianLineBreak { get; set; }
		bool LatinLineBreak { get; set; }
		DrawingTextTabStopCollection TabStopList { get; }
		IDrawingTextParagraphPropertiesOptions Options { get; }
	}
	#endregion
	#region IDrawingTextParagraphPropertiesOptions
	public interface IDrawingTextParagraphPropertiesOptions {
		bool HasTextIndentLevel { get; }
		bool HasDefaultTabSize { get; }
		bool HasTextAlignment { get; }
		bool HasFontAlignment { get; }
		bool HasEastAsianLineBreak { get; }
		bool HasLatinLineBreak { get; }
		bool HasHangingPunctuation { get; }
		bool HasRightToLeft { get; }
		bool HasIndent { get; }
		bool HasLeftMargin { get; }
		bool HasRightMargin { get; }
	}
	#endregion
	#region DrawingTextParagraphPropertiesBatchUpdateHelper
	public class DrawingTextParagraphPropertiesBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		#region Fields
		readonly DrawingTextSpacingInfo[] textSpacingInfoes;
		DrawingTextParagraphInfo textParagraphInfo;
		int suppressDirectNotificationsCount;
		#endregion
		public DrawingTextParagraphPropertiesBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
			textSpacingInfoes = new DrawingTextSpacingInfo[DrawingTextParagraphProperties.TextSpacingCount];
		}
		public DrawingTextParagraphInfo TextParagraphInfo { get { return textParagraphInfo; } set { textParagraphInfo = value; } }
		public DrawingTextSpacingInfo[] TextSpacingInfoes { get { return textSpacingInfoes; } }
		public bool IsDirectNotificationsEnabled { get { return suppressDirectNotificationsCount == 0; } }
		public void SuppressDirectNotifications() {
			suppressDirectNotificationsCount++;
		}
		public void ResumeDirectNotifications() {
			suppressDirectNotificationsCount--;
		}
	}
	#endregion
	#region DrawingTextParagraphPropertiesBatchInitHelper
	public class DrawingTextParagraphPropertiesBatchInitHelper : DrawingTextParagraphPropertiesBatchUpdateHelper {
		public DrawingTextParagraphPropertiesBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region DrawingTextParagraphProperties
	public class DrawingTextParagraphProperties : DrawingMultiIndexObject<DrawingTextParagraphProperties, DocumentModelChangeActions>, ICloneable<DrawingTextParagraphProperties>, ISupportsCopyFrom<DrawingTextParagraphProperties>, IDrawingTextParagraphProperties {
		#region Static Members
		readonly static DrawingTextParagraphInfoIndexAccessor textParagraphInfoIndexAccessor = new DrawingTextParagraphInfoIndexAccessor();
		readonly static IIndexAccessorBase<DrawingTextParagraphProperties, DocumentModelChangeActions>[] indexAccessors = GetIndexAccessors();
		static IIndexAccessorBase<DrawingTextParagraphProperties, DocumentModelChangeActions>[] GetIndexAccessors() {
			IIndexAccessorBase<DrawingTextParagraphProperties, DocumentModelChangeActions>[] result = new IIndexAccessorBase<DrawingTextParagraphProperties, DocumentModelChangeActions>[TextSpacingCount + 1];
			for (int i = 0; i < TextSpacingCount; i++)
				result[i] = new DrawingTextSpacingInfoIndexAccessor(i);
			result[TextSpacingCount] = textParagraphInfoIndexAccessor;
			return result;
		}
		public static IIndexAccessorBase<DrawingTextParagraphProperties, DocumentModelChangeActions>[] TextParagraphPropertiesIndexAccessors { get { return indexAccessors; } }
		public static DrawingTextParagraphInfoIndexAccessor TextParagraphInfoIndexAccessor { get { return textParagraphInfoIndexAccessor; } }
		#endregion
		#region Fields
		public const int LineTextSpacingIndex = 0;
		public const int SpaceAfterTextSpacingIndex = 1;
		public const int SpaceBeforeTextSpacingIndex = 2;
		public const int TextSpacingCount = 3;
		readonly DrawingTextCharacterProperties defaultCharacterProperties;
		int textParagraphInfoIndex;
		int[] textSpacingInfoIndexes;
		DrawingTextSpacingPropertyChangeManager textSpacingsChangeManager;
		readonly Dictionary<DrawingBulletType, IDrawingBullet> bullets;
		readonly DrawingTextTabStopCollection tabStopList;
		#endregion
		public DrawingTextParagraphProperties(IDocumentModel documentModel)
			: base(documentModel) {
			this.defaultCharacterProperties = new DrawingTextCharacterProperties(documentModel) { Parent = InnerParent };
			this.textSpacingInfoIndexes = new int[TextSpacingCount];
			textSpacingsChangeManager = new DrawingTextSpacingPropertyChangeManager(this);
			bullets = new Dictionary<DrawingBulletType, IDrawingBullet>();
			AddDefaultBullets();
			this.tabStopList = new DrawingTextTabStopCollection(documentModel);
		}
		#region Properties
		public int TextParagraphInfoIndex { get { return textParagraphInfoIndex; } }
		public int[] TextSpacingInfoIndexes { get { return textSpacingInfoIndexes; } }
		public int LineSpacingInfoIndex { get { return textSpacingInfoIndexes[LineTextSpacingIndex]; } }
		public int SpaceAfterInfoIndex { get { return textSpacingInfoIndexes[SpaceAfterTextSpacingIndex]; } }
		public int SpaceBeforeInfoIndex { get { return textSpacingInfoIndexes[SpaceBeforeTextSpacingIndex]; } }
		protected override IIndexAccessorBase<DrawingTextParagraphProperties, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal new DrawingTextParagraphPropertiesBatchUpdateHelper BatchUpdateHelper { get { return (DrawingTextParagraphPropertiesBatchUpdateHelper)base.BatchUpdateHelper; } }
		internal DrawingTextParagraphInfo TextParagraphInfo { get { return IsUpdateLocked ? BatchUpdateHelper.TextParagraphInfo : TextParagraphInfoCore; } }
		DrawingTextParagraphInfo TextParagraphInfoCore { get { return textParagraphInfoIndexAccessor.GetInfo(this); } }
		IDrawingBullet CommonBullet { get { return bullets[DrawingBulletType.Common]; } }
		IDrawingBullet ColorBullet { get { return bullets[DrawingBulletType.Color]; } }
		IDrawingBullet TypefaceBullet { get { return bullets[DrawingBulletType.Typeface]; } }
		IDrawingBullet SizeBullet { get { return bullets[DrawingBulletType.Size]; } }
		public bool IsDefault {
			get {
				return
					TextParagraphInfoIndex == DrawingTextParagraphInfoCache.DefaultItemIndex &&
					LineSpacingInfoIndex == DrawingTextSpacingInfoCache.DefaultItemIndex &&
					SpaceAfterInfoIndex == DrawingTextSpacingInfoCache.DefaultItemIndex &&
					SpaceBeforeInfoIndex == DrawingTextSpacingInfoCache.DefaultItemIndex &&
					defaultCharacterProperties.IsDefault &&
					CommonBullet.Type == DrawingBulletType.Automatic &&
					ColorBullet.Type == DrawingBulletType.Automatic &&
					TypefaceBullet.Type == DrawingBulletType.Automatic &&
					SizeBullet.Type == DrawingBulletType.Automatic &&
					TabStopList.Count == 0;
			}
		}
		#endregion
		#region IDrawingTextParagraphProperties Members
		public IDrawingTextSpacings Spacings { get { return this; } }
		public IDrawingTextMargin Margin { get { return this; } }
		public IDrawingTextBullets Bullets { get { return this; } }
		public DrawingTextCharacterProperties DefaultCharacterProperties { get { return defaultCharacterProperties; } }
		public DrawingTextTabStopCollection TabStopList { get { return tabStopList; } }
		#region IDrawingTextParagraphProperties.TextAlignment
		public DrawingTextAlignmentType TextAlignment {
			get { return TextParagraphInfo.TextAlignment; }
			set {
				if (TextParagraphInfo.TextAlignment == value && TextParagraphInfo.HasTextAlignment)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetTextAlignment, value);
			}
		}
		DocumentModelChangeActions SetTextAlignment(DrawingTextParagraphInfo info, DrawingTextAlignmentType value) {
			info.HasTextAlignment = true;
			info.TextAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.FontAlignment
		public DrawingFontAlignmentType FontAlignment {
			get { return TextParagraphInfo.FontAlignment; }
			set {
				if (TextParagraphInfo.FontAlignment == value && TextParagraphInfo.HasFontAlignment)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetFontAlignment, value);
			}
		}
		DocumentModelChangeActions SetFontAlignment(DrawingTextParagraphInfo info, DrawingFontAlignmentType value) {
			info.HasFontAlignment = true;
			info.FontAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.TextIndentLeve
		public int TextIndentLevel {
			get { return TextParagraphInfo.TextIndentLevel; }
			set {
				if (TextParagraphInfo.TextIndentLevel == value && TextParagraphInfo.HasTextIndentLevel)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetTextIndentLevel, value);
			}
		}
		DocumentModelChangeActions SetTextIndentLevel(DrawingTextParagraphInfo info, int value) {
			info.HasTextIndentLevel = true;
			info.TextIndentLevel = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.DefaultTabSize
		public int DefaultTabSize {
			get { return TextParagraphInfo.DefaultTabSize; }
			set {
				if (TextParagraphInfo.DefaultTabSize == value && TextParagraphInfo.HasDefaultTabSize)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetDefaultTabSize, value);
			}
		}
		DocumentModelChangeActions SetDefaultTabSize(DrawingTextParagraphInfo info, int value) {
			info.HasDefaultTabSize = true;
			info.DefaultTabSize = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.Indent
		public int Indent {
			get { return TextParagraphInfo.Indent; }
			set {
				if (TextParagraphInfo.Indent == value && TextParagraphInfo.HasIndent)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetIndent, value);
			}
		}
		DocumentModelChangeActions SetIndent(DrawingTextParagraphInfo info, int value) {
			info.HasIndent = true;
			info.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.RightToLeft
		public bool RightToLeft {
			get { return TextParagraphInfo.RightToLeft; }
			set {
				if (TextParagraphInfo.RightToLeft == value && TextParagraphInfo.HasRightToLeft)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetRightToLeft, value);
			}
		}
		DocumentModelChangeActions SetRightToLeft(DrawingTextParagraphInfo info, bool value) {
			info.HasRightToLeft = true;
			info.RightToLeft = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.HangingPunctuation
		public bool HangingPunctuation {
			get { return TextParagraphInfo.HangingPunctuation; }
			set {
				if (TextParagraphInfo.HangingPunctuation == value && TextParagraphInfo.HasHangingPunctuation)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetHangingPunctuation, value);
			}
		}
		DocumentModelChangeActions SetHangingPunctuation(DrawingTextParagraphInfo info, bool value) {
			info.HasHangingPunctuation = true;
			info.HangingPunctuation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.EastAsianLineBreak
		public bool EastAsianLineBreak {
			get { return TextParagraphInfo.EastAsianLineBreak; }
			set {
				if (TextParagraphInfo.EastAsianLineBreak == value && TextParagraphInfo.HasEastAsianLineBreak)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetEastAsianLineBreak, value);
			}
		}
		DocumentModelChangeActions SetEastAsianLineBreak(DrawingTextParagraphInfo info, bool value) {
			info.HasEastAsianLineBreak = true;
			info.EastAsianLineBreak = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.LatinLineBreak
		public bool LatinLineBreak {
			get { return TextParagraphInfo.LatinLineBreak; }
			set {
				if (TextParagraphInfo.LatinLineBreak == value && TextParagraphInfo.HasLatinLineBreak)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetLatinLineBreak, value);
			}
		}
		DocumentModelChangeActions SetLatinLineBreak(DrawingTextParagraphInfo info, bool value) {
			info.HasLatinLineBreak = true;
			info.LatinLineBreak = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.ApplyDefaultCharacterProperties
		public bool ApplyDefaultCharacterProperties {
			get { return TextParagraphInfo.ApplyDefaultCharacterProperties; }
			set {
				if (TextParagraphInfo.ApplyDefaultCharacterProperties == value)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetApplyDefaultCharacterProperties, value);
			}
		}
		DocumentModelChangeActions SetApplyDefaultCharacterProperties(DrawingTextParagraphInfo info, bool value) {
			info.ApplyDefaultCharacterProperties = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextParagraphProperties.Options
		public IDrawingTextParagraphPropertiesOptions Options { get { return this; } }
		#endregion 
		#region IDrawingTextParagraphPropertiesOptions Members
		bool IDrawingTextParagraphPropertiesOptions.HasDefaultTabSize { get { return TextParagraphInfo.HasDefaultTabSize; } }
		bool IDrawingTextParagraphPropertiesOptions.HasEastAsianLineBreak { get { return TextParagraphInfo.HasEastAsianLineBreak; } }
		bool IDrawingTextParagraphPropertiesOptions.HasFontAlignment { get { return TextParagraphInfo.HasFontAlignment; } }
		bool IDrawingTextParagraphPropertiesOptions.HasHangingPunctuation { get { return TextParagraphInfo.HasHangingPunctuation; } }
		bool IDrawingTextParagraphPropertiesOptions.HasIndent { get { return TextParagraphInfo.HasIndent; } }
		bool IDrawingTextParagraphPropertiesOptions.HasLatinLineBreak { get { return TextParagraphInfo.HasLatinLineBreak; } }
		bool IDrawingTextParagraphPropertiesOptions.HasLeftMargin { get { return TextParagraphInfo.HasLeftMargin; } }
		bool IDrawingTextParagraphPropertiesOptions.HasRightMargin { get { return TextParagraphInfo.HasRightMargin; } }
		bool IDrawingTextParagraphPropertiesOptions.HasRightToLeft { get { return TextParagraphInfo.HasRightToLeft; } }
		bool IDrawingTextParagraphPropertiesOptions.HasTextAlignment { get { return TextParagraphInfo.HasTextAlignment; } }
		bool IDrawingTextParagraphPropertiesOptions.HasTextIndentLevel { get { return TextParagraphInfo.HasTextIndentLevel; } }
		#endregion
		#region IDrawingTextSpacings Members
		IDrawingTextSpacing IDrawingTextSpacings.Line { get { return textSpacingsChangeManager.GetFormatInfo(LineTextSpacingIndex); } }
		IDrawingTextSpacing IDrawingTextSpacings.SpaceAfter { get { return textSpacingsChangeManager.GetFormatInfo(SpaceAfterTextSpacingIndex); } }
		IDrawingTextSpacing IDrawingTextSpacings.SpaceBefore { get { return textSpacingsChangeManager.GetFormatInfo(SpaceBeforeTextSpacingIndex); } }
		#endregion
		#region IDrawingTextMargin Members
		#region IDrawingTextMargin.Left
		int IDrawingTextMargin.Left {
			get { return TextParagraphInfo.LeftMargin; }
			set {
				if (TextParagraphInfo.LeftMargin == value && TextParagraphInfo.HasLeftMargin)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetLeftMargin, value);
			}
		}
		DocumentModelChangeActions SetLeftMargin(DrawingTextParagraphInfo info, int value) {
			info.HasLeftMargin = true;
			info.LeftMargin = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDrawingTextMargin.Right
		int IDrawingTextMargin.Right {
			get { return TextParagraphInfo.RightMargin; }
			set {
				if (TextParagraphInfo.RightMargin == value && TextParagraphInfo.HasRightMargin)
					return;
				SetPropertyValue(TextParagraphInfoIndexAccessor, SetRightMargin, value);
			}
		}
		DocumentModelChangeActions SetRightMargin(DrawingTextParagraphInfo info, int value) {
			info.HasRightMargin = true;
			info.RightMargin = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IDrawingTextBullets Members
		IDrawingBullet IDrawingTextBullets.Common { get { return CommonBullet; } set { SetBullet(DrawingBulletType.Common, value); } }
		IDrawingBullet IDrawingTextBullets.Color { get { return ColorBullet; } set { SetBullet(DrawingBulletType.Color, value); } }
		IDrawingBullet IDrawingTextBullets.Typeface { get { return TypefaceBullet; } set { SetBullet(DrawingBulletType.Typeface, value); } }
		IDrawingBullet IDrawingTextBullets.Size { get { return SizeBullet; } set { SetBullet(DrawingBulletType.Size, value); } }
		void SetBullet(DrawingBulletType type, IDrawingBullet value) {
			if(value.Type != DrawingBulletType.Automatic && type != value.Type)
				throw new ArgumentException("Invalid DrawingBulletType.");
			IDrawingBullet bullet = bullets[type];
			if (bullet.Equals(value))
				return;
			HistoryItem item = new DrawingTextParagraphPropertiesBulletChangedHistoryItem(this, type, bullet, value);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal void SetBulletCore(DrawingBulletType type, IDrawingBullet value) {
			bullets[type] = value;
		}
		#endregion
		#region IDrawingTextSpacingsChanger Members
		#region Type
		DrawingTextSpacingValueType IDrawingTextSpacingsChanger.GetType(int index) {
			return GetTextSpacingInfo(index).Type;
		}
		void IDrawingTextSpacingsChanger.SetType(int index, DrawingTextSpacingValueType value) {
			if (GetTextSpacingInfo(index).Type == value)
				return;
			SetPropertyValue(GetTextSpacingInfoIndexAccessor(index), SetTypeCore, value);
		}
		DocumentModelChangeActions SetTypeCore(DrawingTextSpacingInfo info, DrawingTextSpacingValueType value) {
			info.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Value
		int IDrawingTextSpacingsChanger.GetValue(int index) {
			return GetTextSpacingInfo(index).Value;
		}
		void IDrawingTextSpacingsChanger.SetValue(int index, int value) {
			if (GetTextSpacingInfo(index).Value == value || GetTextSpacingInfo(index).Type == DrawingTextSpacingValueType.Automatic)
				return;
			SetPropertyValue(GetTextSpacingInfoIndexAccessor(index), SetValueCore, value);
		}
		DocumentModelChangeActions SetValueCore(DrawingTextSpacingInfo info, int value) {
			info.Value = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		void AddDefaultBullets() {
			SetBulletCore(DrawingBulletType.Common, DrawingBullet.Automatic);
			SetBulletCore(DrawingBulletType.Color, DrawingBullet.Automatic);
			SetBulletCore(DrawingBulletType.Typeface, DrawingBullet.Automatic);
			SetBulletCore(DrawingBulletType.Size, DrawingBullet.Automatic);
		}
		#region MultiIndexManagement
		internal void AssignTextParagraphInfoIndex(int value) {
			this.textParagraphInfoIndex = value;
		}
		public void AssignTextSpacingInfoIndex(int index, int value) {
			this.textSpacingInfoIndexes[index] = value;
		}
		public override DrawingTextParagraphProperties GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new DrawingTextParagraphPropertiesBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new DrawingTextParagraphPropertiesBatchInitHelper(this);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		internal DrawingTextSpacingInfo GetTextSpacingInfo(int index) {
			if (BatchUpdateHelper != null)
				return BatchUpdateHelper.TextSpacingInfoes[index];
			return DrawingCache.DrawingTextSpacingInfoCache[TextSpacingInfoIndexes[index]];
		}
		DrawingTextSpacingInfoIndexAccessor GetTextSpacingInfoIndexAccessor(int index) {
			return (DrawingTextSpacingInfoIndexAccessor)TextParagraphPropertiesIndexAccessors[index];
		}
		#endregion
		#region ICloneable<DrawingTextParagraphProperties> Members
		public DrawingTextParagraphProperties Clone() {
			DrawingTextParagraphProperties result = new DrawingTextParagraphProperties(DocumentModel);
			CloneCore(result);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextParagraphProperties>
		public void CopyFrom(DrawingTextParagraphProperties value) {
			base.CopyFrom(value);
			defaultCharacterProperties.CopyFrom(value.defaultCharacterProperties);
			SetBulletCore(DrawingBulletType.Common, value.CommonBullet);
			SetBulletCore(DrawingBulletType.Color, value.ColorBullet);
			SetBulletCore(DrawingBulletType.Typeface, value.TypefaceBullet);
			SetBulletCore(DrawingBulletType.Size, value.SizeBullet);
			tabStopList.CopyFrom(value.TabStopList);
		}
		#endregion 
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextParagraphProperties other = obj as DrawingTextParagraphProperties;
			if (other == null)
				return false;
			return
				base.Equals(other) && defaultCharacterProperties.Equals(other.defaultCharacterProperties) &&
				CommonBullet.Equals(other.CommonBullet) && ColorBullet.Equals(other.ColorBullet) &&
				TypefaceBullet.Equals(other.TypefaceBullet) && SizeBullet.Equals(other.SizeBullet) &&
				tabStopList.Equals(other.tabStopList);
		}
		public override int GetHashCode() {
			return 
				base.GetHashCode() ^ defaultCharacterProperties.GetHashCode() ^
				CommonBullet.GetHashCode() ^ ColorBullet.GetHashCode() ^
				TypefaceBullet.GetHashCode() ^ SizeBullet.GetHashCode() ^ tabStopList.GetHashCode();
		}
		#endregion
		public void ResetToStyle() {
			BeginUpdate();
			try {
				TextParagraphInfo.CopyFrom(DrawingCache.DrawingTextParagraphInfoCache.DefaultItem);
				GetTextSpacingInfo(LineTextSpacingIndex).CopyFrom(DrawingCache.DrawingTextSpacingInfoCache.DefaultItem);
				GetTextSpacingInfo(SpaceAfterTextSpacingIndex).CopyFrom(DrawingCache.DrawingTextSpacingInfoCache.DefaultItem);
				GetTextSpacingInfo(SpaceBeforeTextSpacingIndex).CopyFrom(DrawingCache.DrawingTextSpacingInfoCache.DefaultItem);
			}
			finally {
				EndUpdate();
			}
			this.defaultCharacterProperties.ResetToStyle();
			SetBullet(DrawingBulletType.Common, DrawingBullet.Automatic);
			SetBullet(DrawingBulletType.Color, DrawingBullet.Automatic);
			SetBullet(DrawingBulletType.Typeface, DrawingBullet.Automatic);
			SetBullet(DrawingBulletType.Size, DrawingBullet.Automatic);
			TabStopList.Clear();
		}
	}
	#endregion
}
