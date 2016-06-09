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
using DevExpress.Office;
using DevExpress.Utils;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SheetFormatInfo
	public class SheetFormatInfo : ICloneable<SheetFormatInfo>, ISupportsCopyFrom<SheetFormatInfo>, ISupportsSizeOf {
		#region Fields
		int baseColumnWidth;
		float defaultColumnWidth;
		float defaultRowHeight;
		bool thickTopBorder;
		bool thickBottomBorder;
		bool zeroHeight;
		bool isCustomHeight;
		int outlineLevelCol;
		int outlineLevelRow;
		#endregion
		#region Properties
		public int BaseColumnWidth { get { return baseColumnWidth; } set { baseColumnWidth = value; } }
		public float DefaultColumnWidth { get { return defaultColumnWidth; } set { defaultColumnWidth = value; } }
		public float DefaultRowHeight { get { return defaultRowHeight; } set { defaultRowHeight = value; } }
		public bool ThickTopBorder { get { return thickTopBorder; } set { thickTopBorder = value; } }
		public bool ThickBottomBorder { get { return thickBottomBorder; } set { thickBottomBorder = value; } }
		public bool ZeroHeight { get { return zeroHeight; } set { zeroHeight = value; } }
		public bool IsCustomHeight { get { return isCustomHeight; } set { isCustomHeight = value; } }
		public int OutlineLevelCol { get { return outlineLevelCol; } set { outlineLevelCol = value; } }
		public int OutlineLevelRow { get { return outlineLevelRow; } set { outlineLevelRow = value; } }
		#endregion
		public SheetFormatInfo Clone() {
			SheetFormatInfo result = new SheetFormatInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(SheetFormatInfo value) {
			this.baseColumnWidth = value.baseColumnWidth;
			this.thickBottomBorder = value.thickBottomBorder;
			this.thickTopBorder = value.thickTopBorder;
			this.zeroHeight = value.zeroHeight;
			this.defaultColumnWidth = value.defaultColumnWidth;
			this.defaultRowHeight = value.defaultRowHeight;
			this.isCustomHeight = value.isCustomHeight;
			this.outlineLevelCol = value.outlineLevelCol;
			this.outlineLevelRow = value.outlineLevelRow;
		}
		public override bool Equals(object obj) {
			SheetFormatInfo other = obj as SheetFormatInfo;
			if (other == null)
				return false;
			return BaseColumnWidth == other.BaseColumnWidth &&
				ThickBottomBorder == other.ThickBottomBorder &&
				ThickTopBorder == other.ThickTopBorder &&
				ZeroHeight == other.ZeroHeight &&
				DefaultColumnWidth == other.DefaultColumnWidth &&
				DefaultRowHeight == other.DefaultRowHeight &&
				IsCustomHeight == other.IsCustomHeight &&
				OutlineLevelCol == other.OutlineLevelCol &&
				OutlineLevelRow == other.OutlineLevelRow;
		}
		public override int GetHashCode() {
			Office.Utils.CombinedHashCode result = new Office.Utils.CombinedHashCode();
			result.AddInt(BaseColumnWidth);
			result.AddInt(ThickTopBorder.GetHashCode());
			result.AddInt(ThickBottomBorder.GetHashCode());
			result.AddInt(ZeroHeight.GetHashCode());
			result.AddInt(DefaultRowHeight.GetHashCode());
			result.AddInt(DefaultColumnWidth.GetHashCode());
			result.AddInt(IsCustomHeight.GetHashCode());
			result.AddInt(OutlineLevelCol);
			result.AddInt(OutlineLevelRow);
			return result.CombinedHash32;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
	}
	#endregion
	#region SheetFormatInfoCache
	public class SheetFormatInfoCache : UniqueItemsCache<SheetFormatInfo> {
		public SheetFormatInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override SheetFormatInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			SheetFormatInfo info = new SheetFormatInfo();
			info.BaseColumnWidth = 8;
			return info;
		}
		protected override int AddItemCore(SheetFormatInfo item) {
			return base.AddItemCore(item);
		}
	}
	#endregion
	#region SheetFormatProperties
	public class SheetFormatProperties : SpreadsheetUndoableIndexBasedObject<SheetFormatInfo> {
		const float MaxWidthInCharacter = 255; 
		public const int MaxDefaultHeightInTwips = 8190; 
		public SheetFormatProperties(IDocumentModelPartWithApplyChanges sheet)
			: base(sheet) {
		}
		#region Properties
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		#region BaseColumnWidth
		public int BaseColumnWidth {
			get { return Info.BaseColumnWidth; }
			set {
				if (BaseColumnWidth == value)
					return;
				SetPropertyValue(SetBaseColumnWidthCore, value);
			}
		}
		DocumentModelChangeActions SetBaseColumnWidthCore(SheetFormatInfo info, int value) {
			info.BaseColumnWidth = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region DefaultColumnWidth
		public float DefaultColumnWidth {
			get { return Info.DefaultColumnWidth; }
			set {
				if (DefaultColumnWidth == value)
					return;
				if (value < 0 || value > MaxWidthInCharacter)
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnWidth));
				SetPropertyValue(SetDefaultColumnWidthCore, value);
			}
		}
		DocumentModelChangeActions SetDefaultColumnWidthCore(SheetFormatInfo info, float value) {
			info.DefaultColumnWidth = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region DefaultRowHeight
		public float DefaultRowHeight {
			get { return Info.DefaultRowHeight; }
			set {
				if (DefaultRowHeight == value)
					return;
				if (value < 0 || value > DocumentModel.UnitConverter.TwipsToModelUnits(MaxDefaultHeightInTwips))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeight));
				SetPropertyValue(SetDefaultRowHeightCore, value);
			}
		}
		DocumentModelChangeActions SetDefaultRowHeightCore(SheetFormatInfo info, float value) {
			info.DefaultRowHeight = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ThickTopBorder
		public bool ThickTopBorder {
			get { return Info.ThickTopBorder; }
			set {
				if (ThickTopBorder == value)
					return;
				SetPropertyValue(SetThickTopBorderCore, value);
			}
		}
		DocumentModelChangeActions SetThickTopBorderCore(SheetFormatInfo info, bool value) {
			info.ThickTopBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ThickBottomBorder
		public bool ThickBottomBorder {
			get { return Info.ThickBottomBorder; }
			set {
				if (ThickBottomBorder == value)
					return;
				SetPropertyValue(SetThickBottomBorderCore, value);
			}
		}
		DocumentModelChangeActions SetThickBottomBorderCore(SheetFormatInfo info, bool value) {
			info.ThickBottomBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ZeroHeight
		public bool ZeroHeight {
			get { return Info.ZeroHeight; }
			set {
				if (ZeroHeight == value)
					return;
				SetPropertyValue(SetZeroHeightCore, value);
			}
		}
		DocumentModelChangeActions SetZeroHeightCore(SheetFormatInfo info, bool value) {
			info.ZeroHeight = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsCustomHeight
		public bool IsCustomHeight {
			get { return Info.IsCustomHeight; }
			set {
				if (IsCustomHeight == value)
					return;
				SetPropertyValue(SetIsCustomHeightCore, value);
			}
		}
		DocumentModelChangeActions SetIsCustomHeightCore(SheetFormatInfo info, bool value) {
			info.IsCustomHeight = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region OutlineLevelCol
		public int OutlineLevelCol {
			get { return Info.OutlineLevelCol; }
			set {
				if (OutlineLevelCol == value)
					return;
				SetPropertyValue(SetOutlineLevelColCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineLevelColCore(SheetFormatInfo info, int value) {
			info.OutlineLevelCol = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region OutlineLevelRow
		public int OutlineLevelRow {
			get { return Info.OutlineLevelRow; }
			set {
				if (OutlineLevelRow == value)
					return;
				SetPropertyValue(SetOutlineLevelRowCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineLevelRowCore(SheetFormatInfo info, int value) {
			info.OutlineLevelRow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<SheetFormatInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.SheetFormatInfoCache;
		}
		public bool IsDefault() {
			return Index == 0;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Sheet.ApplyChanges(changeActions);
		}
	}
	#endregion
}
