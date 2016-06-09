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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region ShapeInfo
	public class ShapeInfo : ICloneable<ShapeInfo>, ISupportsCopyFrom<ShapeInfo>, ISupportsSizeOf {
		#region Fields
		Color fillColor;
		Color outlineColor;
		int outlineWidth;
		int rotation;
		#endregion
		#region Properties
		public Color FillColor { get { return fillColor; } set { fillColor = value; } }
		public Color OutlineColor { get { return outlineColor; } set { outlineColor = value; } }
		public int OutlineWidth { get { return outlineWidth; } set { outlineWidth = value; } }
		public int Rotation { get { return rotation; } set { rotation = value; } }
		#endregion
		public ShapeInfo Clone() {
			ShapeInfo result = new ShapeInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ShapeInfo val) {
			this.FillColor = val.FillColor;
			this.OutlineColor = val.OutlineColor;
			this.OutlineWidth = val.OutlineWidth;
			this.Rotation = val.Rotation;
		}
		public override bool Equals(object obj) {
			ShapeInfo info = (ShapeInfo)obj;
			return
				this.FillColor == info.FillColor &&
				this.OutlineColor == info.OutlineColor &&
				this.OutlineWidth == info.OutlineWidth &&
				this.Rotation == info.Rotation;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ShapeInfoCache
	public class ShapeInfoCache : UniqueItemsCache<ShapeInfo> {
		internal const int DefaultItemIndex = 0;
		public ShapeInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ShapeInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ShapeInfo item = new ShapeInfo();
			return item;
		}
	}
	#endregion
	#region ShapeOptions
	public class ShapeOptions : ICloneable<ShapeOptions>, ISupportsCopyFrom<ShapeOptions>, ISupportsSizeOf {
		#region Mask enumeration
		public enum Mask {
			UseNone = 0x00000000,
			UseFillColor = 0x00000001,
			UseOutlineColor = 0x00000002,
			UseOutlineWidth = 0x00000004,
			UseRotation = 0x00000008,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.UseNone;
		#region Properties
		internal Mask Value { get { return val; } set { val = value; } }
		public bool UseFillColor { get { return GetVal(Mask.UseFillColor); } set { SetVal(Mask.UseFillColor, value); } }
		public bool UseOutlineColor { get { return GetVal(Mask.UseOutlineColor); } set { SetVal(Mask.UseOutlineColor, value); } }
		public bool UseOutlineWidth { get { return GetVal(Mask.UseOutlineWidth); } set { SetVal(Mask.UseOutlineWidth, value); } }
		public bool UseRotation { get { return GetVal(Mask.UseRotation); } set { SetVal(Mask.UseRotation, value); } }
		#endregion
		#region GetVal/SetVal helpers
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				val |= mask;
			else
				val &= ~mask;
		}
		public bool GetVal(Mask mask) {
			return (val & mask) != 0;
		}
		#endregion
		public ShapeOptions() {
		}
		internal ShapeOptions(Mask val) {
			this.val = val;
		}
		public ShapeOptions Clone() {
			return new ShapeOptions(this.val);
		}
		public override bool Equals(object obj) {
			ShapeOptions opts = (ShapeOptions)obj;
			return opts.Value == this.Value;
		}
		public override int GetHashCode() {
			return (int)this.Value;
		}
		public void CopyFrom(ShapeOptions options) {
			this.val = options.Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ShapeOptionsCache
	public class ShapeOptionsCache : UniqueItemsCache<ShapeOptions> {
		internal const int EmptyShapeOptionIndex = 0;
		public ShapeOptionsCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ShapeOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ShapeOptions();
		}
	}
	#endregion
	#region ShapeFormatting
	public class ShapeFormatting : IndexBasedObjectB<ShapeInfo, ShapeOptions>, ICloneable<ShapeFormatting>, ISupportsCopyFrom<ShapeFormatting> {
		internal ShapeFormatting(PieceTable pieceTable, DocumentModel documentModel, int infoIndex, int optionsIndex)
			: base(pieceTable, documentModel, infoIndex, optionsIndex) {
		}
		protected override UniqueItemsCache<ShapeInfo> InfoCache { get { return ((DocumentModel)DocumentModel).Cache.ShapeInfoCache; } }
		protected override UniqueItemsCache<ShapeOptions> OptionsCache { get { return ((DocumentModel)DocumentModel).Cache.ShapeOptionsCache; } }
		#region FillColor
		public Color FillColor {
			get { return Info.FillColor; }
			set {
				if (Info.FillColor == value && Options.UseFillColor)
					return;
				SetPropertyValue(SetFillColorCore, value, SetUseFillColorCore);
			}
		}
		void SetFillColorCore(ShapeInfo info, Color value) {
			info.FillColor = value;
		}
		void SetUseFillColorCore(ShapeOptions options, bool value) {
			options.UseFillColor = value;
		}
		#endregion
		#region OutlineColor
		public Color OutlineColor {
			get { return Info.OutlineColor; }
			set {
				if (Info.OutlineColor == value && Options.UseOutlineColor)
					return;
				SetPropertyValue(SetOutlineColorCore, value, SetUseOutlineColorCore);
			}
		}
		void SetOutlineColorCore(ShapeInfo info, Color value) {
			info.OutlineColor = value;
		}
		void SetUseOutlineColorCore(ShapeOptions options, bool value) {
			options.UseOutlineColor = value;
		}
		#endregion
		#region OutlineWidth
		public int OutlineWidth {
			get { return Info.OutlineWidth; }
			set {
				if (Info.OutlineWidth == value && Options.UseOutlineWidth)
					return;
				SetPropertyValue(SetOutlineWidthCore, value, SetUseOutlineWidthCore);
			}
		}
		void SetOutlineWidthCore(ShapeInfo info, int value) {
			info.OutlineWidth = value;
		}
		void SetUseOutlineWidthCore(ShapeOptions options, bool value) {
			options.UseOutlineWidth = value;
		}
		#endregion
		#region Rotation
		public int Rotation {
			get { return Info.Rotation; }
			set {
				if (Info.Rotation == value && Options.UseRotation)
					return;
				SetPropertyValue(SetRotationCore, value, SetUseRotationCore);
			}
		}
		void SetRotationCore(ShapeInfo info, int value) {
			info.Rotation = value;
		}
		void SetUseRotationCore(ShapeOptions options, bool value) {
			options.UseRotation = value;
		}
		#endregion
		#region ICloneable<ShapeFormatting> Members
		public ShapeFormatting Clone() {
			return new ShapeFormatting(PieceTable, (DocumentModel)DocumentModel, InfoIndex, OptionsIndex);
		}
		#endregion
		public void CopyFrom(ShapeFormatting ShapeFormatting) {
			CopyFrom(ShapeFormatting.Info, ShapeFormatting.Options);
		}
		public void CopyFrom(ShapeInfo info, ShapeOptions options) {
			CopyFromCore(info, options);
		}
		protected override bool PropertyEquals(IndexBasedObject<ShapeInfo, ShapeOptions> other) {
			Guard.ArgumentNotNull(other, "other");
			return Options.Value == other.Options.Value &&
				Info.Equals(other.Info);
		}
		protected override void SetPropertyValue<U>(IndexBasedObjectB<ShapeInfo, ShapeOptions>.SetPropertyValueDelegate<U> setter, U newValue, IndexBasedObjectB<ShapeInfo, ShapeOptions>.SetOptionsValueDelegate optionsSetter) {
			if (((DocumentModel)DocumentModel).DocumentCapabilities.FloatingObjectsAllowed)
				base.SetPropertyValue<U>(setter, newValue, optionsSetter);
		}
	}
	#endregion
	#region ShapeFormattingCache
	public class ShapeFormattingCache : UniqueItemsCache<ShapeFormatting> {
		#region Fields
		public const int EmptyShapeFormattingIndex = 0;
		readonly DocumentModel documentModel;
		#endregion
		public ShapeFormattingCache(DocumentModel documentModel)
			: base(documentModel.UnitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			AppendItem(new ShapeFormatting(DocumentModel.MainPieceTable, DocumentModel, 0, ShapeOptionsCache.EmptyShapeOptionIndex));
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected override ShapeFormatting CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null;
		}
	}
	#endregion
	#region IShapeContainer
	public interface IShapeContainer {
		PieceTable PieceTable { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateShapeChangedHistoryItem();
		void OnShapeChanged();
	}
	#endregion
	#region Shape
	public class Shape : RichEditIndexBasedObject<ShapeFormatting> {
		readonly IShapeContainer owner;
		public Shape(IShapeContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(IShapeContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		#region Properties
		#region FillColor
		public Color FillColor {
			get { return Info.FillColor; }
			set {
				if (Info.FillColor == value && UseFillColor)
					return;
				SetPropertyValue(SetFillColorCore, value);
			}
		}
		public bool UseFillColor { get { return Info.Options.UseFillColor; } }
		protected internal virtual DocumentModelChangeActions SetFillColorCore(ShapeFormatting info, Color value) {
			info.FillColor = value;
			return ShapeChangeActionsCalculator.CalculateChangeActions(ShapeChangeType.FillColor);
		}
		#endregion
		#region OutlineColor
		public Color OutlineColor {
			get { return Info.OutlineColor; }
			set {
				if (Info.OutlineColor == value && UseOutlineColor)
					return;
				SetPropertyValue(SetOutlineColorCore, value);
			}
		}
		public bool UseOutlineColor { get { return Info.Options.UseOutlineColor; } }
		protected internal virtual DocumentModelChangeActions SetOutlineColorCore(ShapeFormatting info, Color value) {
			info.OutlineColor = value;
			return ShapeChangeActionsCalculator.CalculateChangeActions(ShapeChangeType.OutlineColor);
		}
		#endregion
		#region OutlineWidth
		public int OutlineWidth {
			get { return Info.OutlineWidth; }
			set {
				if (Info.OutlineWidth == value && UseOutlineWidth)
					return;
				SetPropertyValue(SetOutlineWidthCore, value);
			}
		}
		public bool UseOutlineWidth { get { return Info.Options.UseOutlineWidth; } }
		protected internal virtual DocumentModelChangeActions SetOutlineWidthCore(ShapeFormatting info, int value) {
			info.OutlineWidth = value;
			return ShapeChangeActionsCalculator.CalculateChangeActions(ShapeChangeType.OutlineWidth);
		}
		#endregion
		#region Owner
		public IShapeContainer ShapeOwner {
			get { return owner; }
		}
		#endregion
		#region Rotation
		public int Rotation {
			get { return Info.Rotation; }
			set {
				if (Info.Rotation == value && UseRotation)
					return;
				SetPropertyValue(SetRotationCore, value);
			}
		}
		public bool UseRotation { get { return Info.Options.UseRotation; } }
		protected internal virtual DocumentModelChangeActions SetRotationCore(ShapeFormatting info, int value) {
			info.Rotation = value;
			return ShapeChangeActionsCalculator.CalculateChangeActions(ShapeChangeType.Rotation);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<ShapeFormatting> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.ShapeFormattingCache;
		}
		protected internal bool UseVal(ShapeOptions.Mask mask) {
			return (Info.Options.Value & mask) != 0;
		}
		public void Reset() {
			ShapeFormatting info = GetInfoForModification();
			ShapeFormatting emptyInfo = GetCache(DocumentModel)[ShapeFormattingCache.EmptyShapeFormattingIndex];
			info.ReplaceInfo(emptyInfo.Info, emptyInfo.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override bool Equals(object obj) {
			Shape other = obj as Shape;
			if (ReferenceEquals(obj, null))
				return false;
			if (DocumentModel == other.DocumentModel)
				return Index == other.Index;
			else
				return Info.Equals(other.Info);
		}
		internal void ResetUse(ShapeOptions.Mask mask) {
			ShapeFormatting info = GetInfoForModification();
			ShapeOptions options = info.GetOptionsForModification();
			options.Value &= ~mask;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetAllUse() {
			ShapeFormatting info = GetInfoForModification();
			ShapeOptions options = info.GetOptionsForModification();
			options.Value = ShapeOptions.Mask.UseNone;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override int GetHashCode() {
			return Index;
		}
#if DEBUGTEST
		public override string ToString() {
			return String.Format("ShapeInfoIndex:{0}, InfoIndex:{1}, OptionsIndex:{2}", Index, Info.InfoIndex, Info.OptionsIndex);
		}
#endif
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return ShapeChangeActionsCalculator.CalculateChangeActions(ShapeChangeType.BatchUpdate);
		}
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			owner.OnShapeChanged();
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateShapeChangedHistoryItem();
		}
	}
	#endregion
	#region ShapeChangeType
	public enum ShapeChangeType {
		None = 0,
		FillColor,
		OutlineColor,
		OutlineWidth,
		Rotation,
		BatchUpdate
	}
	#endregion
	#region ShapeChangeActionsCalculator
	public static class ShapeChangeActionsCalculator {
		internal class ShapeChangeActionsTable : Dictionary<ShapeChangeType, DocumentModelChangeActions> {
		}
		internal static readonly ShapeChangeActionsTable shapeChangeActionsTable = CreateShapeChangeActionsTable();
		internal static ShapeChangeActionsTable CreateShapeChangeActionsTable() {
			ShapeChangeActionsTable table = new ShapeChangeActionsTable();
			table.Add(ShapeChangeType.None, DocumentModelChangeActions.None);
			table.Add(ShapeChangeType.FillColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ShapeChangeType.OutlineColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ShapeChangeType.OutlineWidth, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ShapeChangeType.Rotation, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ShapeChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(ShapeChangeType change) {
			return shapeChangeActionsTable[change];
		}
	}
	#endregion
}
