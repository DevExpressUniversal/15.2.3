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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region FloatingObjectHorizontalPositionType
	public enum FloatingObjectHorizontalPositionType {
		Page = 0,
		Character,
		Column,
		Margin,
		LeftMargin,
		RightMargin,
		InsideMargin,
		OutsideMargin
	}
	#endregion
	#region FloatingObjectHorizontalPositionAlignment
	public enum FloatingObjectHorizontalPositionAlignment {
		None = 0,
		Left,
		Center,
		Right,
		Inside,
		Outside
	}
	#endregion
	#region FloatingObjectVerticalPositionType
	public enum FloatingObjectVerticalPositionType {
		Page = 0,
		Line,
		Paragraph,
		Margin,
		TopMargin,
		BottomMargin,
		InsideMargin,
		OutsideMargin
	}
	#endregion
	#region FloatingObjectVerticalPositionAlignment
	public enum FloatingObjectVerticalPositionAlignment {
		None = 0,
		Top,
		Center,
		Bottom,
		Inside,
		Outside
	}
	#endregion
	#region FloatingObjectTextWrapType
	public enum FloatingObjectTextWrapType {
		None,  
		TopAndBottom,
		Tight,
		Through,
		Square,
		Inline,
	}
	#endregion
	#region FloatingObjectTextWrapSide
	public enum FloatingObjectTextWrapSide {
		Both,
		Left,
		Right,
		Largest,
	}
	#endregion
	#region FloatingObjectRelativeFromHorizontal
	public enum FloatingObjectRelativeFromHorizontal {
		Margin = 0,
		Page,
		LeftMargin,
		RightMargin,
		InsideMargin,
		OutsideMargin
	}
	#endregion
	#region FloatingObjectRelativeFromVertical
	public enum FloatingObjectRelativeFromVertical {
		Margin = 0,
		Page,
		TopMargin,
		BottomMargin,
		InsideMargin,
		OutsideMargin
	}
	#endregion
	#region FloatingObjectRelativeWidth
	public struct FloatingObjectRelativeWidth {
		readonly int width;
		readonly FloatingObjectRelativeFromHorizontal from;
		public FloatingObjectRelativeWidth(FloatingObjectRelativeFromHorizontal from, int width) {
			this.width = width;
			this.from = from;
		}
		public int Width { get { return width; } }
		public FloatingObjectRelativeFromHorizontal From { get { return from; } }
		public override bool Equals(object obj) {
			if (!(obj is FloatingObjectRelativeWidth))
				return false;
			FloatingObjectRelativeWidth other = (FloatingObjectRelativeWidth)obj;
			return other == this;
		}
		public override int GetHashCode() {
			return width + ((int)from << 24);
		}
		public static bool operator ==(FloatingObjectRelativeWidth width1, FloatingObjectRelativeWidth width2) {
			return width1.Width == width2.Width && width1.From == width2.From;
		}
		public static bool operator !=(FloatingObjectRelativeWidth width1, FloatingObjectRelativeWidth width2) {
			return !(width1 == width2);
		}
		public override string ToString() {
			return String.Format("{0}: {1}%", from, width / 1000.0);
		}
	}
	#endregion
	#region FloatingObjectRelativeHeight
	public struct FloatingObjectRelativeHeight {
		readonly int height;
		readonly FloatingObjectRelativeFromVertical from;
		public FloatingObjectRelativeHeight(FloatingObjectRelativeFromVertical from, int height) {
			this.height = height;
			this.from = from;
		}
		public int Height { get { return height; } }
		public FloatingObjectRelativeFromVertical From { get { return from; } }
		public override bool Equals(object obj) {
			if (!(obj is FloatingObjectRelativeHeight))
				return false;
			FloatingObjectRelativeHeight other = (FloatingObjectRelativeHeight)obj;
			return other == this;
		}
		public override int GetHashCode() {
			return height + ((int)from << 24);
		}
		public static bool operator ==(FloatingObjectRelativeHeight height1, FloatingObjectRelativeHeight height) {
			return height1.Height == height.Height && height1.From == height.From;
		}
		public static bool operator !=(FloatingObjectRelativeHeight width1, FloatingObjectRelativeHeight width2) {
			return !(width1 == width2);
		}
		public override string ToString() {
			return String.Format("{0}: {1}%", from, height / 1000.0);
		}
	}
	#endregion
	#region FloatingObjectInfo
	public class FloatingObjectInfo : ICloneable<FloatingObjectInfo>, ISupportsCopyFrom<FloatingObjectInfo>, ISupportsSizeOf {
		#region Fields
		bool allowOverlap;
		bool hidden;
		bool layoutInTableCell;
		bool locked;
		bool lockAspectRatio;
		int leftDistance;
		int rightDistance;
		int topDistance;
		int bottomDistance;
		int zOrder;
		Size actualSize;
		FloatingObjectRelativeWidth relativeWidth;
		FloatingObjectRelativeHeight relativeHeight;
		FloatingObjectTextWrapType textWrapType;
		FloatingObjectTextWrapSide textWrapSide;
		FloatingObjectHorizontalPositionType horizontalPositionType;
		FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment;
		FloatingObjectVerticalPositionType verticalPositionType;
		FloatingObjectVerticalPositionAlignment verticalPositionAlignment;
		Point offset;
		Point percentOffset;
		bool isBehindDoc;
		bool pseudoInline;
		#endregion
		#region Properties
		public bool AllowOverlap { get { return allowOverlap; } set { allowOverlap = value; } }
		public bool Hidden { get { return hidden; } set { hidden = value; } }
		public bool LayoutInTableCell { get { return layoutInTableCell; } set { layoutInTableCell = value; } }
		public bool Locked { get { return locked; } set { locked = value; } }
		public bool LockAspectRatio { get { return lockAspectRatio; } set { lockAspectRatio = value; } }
		public int LeftDistance { get { return leftDistance; } set { leftDistance = value; } }
		public int RightDistance { get { return rightDistance; } set { rightDistance = value; } }
		public int TopDistance { get { return topDistance; } set { topDistance = value; } }
		public int BottomDistance { get { return bottomDistance; } set { bottomDistance = value; } }
		public int ZOrder { get { return zOrder; } set { zOrder = value; } }
		public Size ActualSize { get { return actualSize; } set { actualSize = value; } }
		public FloatingObjectTextWrapType TextWrapType { get { return textWrapType; } set { textWrapType = value; } }
		public FloatingObjectTextWrapSide TextWrapSide { get { return textWrapSide; } set { textWrapSide = value; } }
		public FloatingObjectHorizontalPositionType HorizontalPositionType { get { return horizontalPositionType; } set { horizontalPositionType = value; } }
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return horizontalPositionAlignment; } set { horizontalPositionAlignment = value; } }
		public FloatingObjectVerticalPositionType VerticalPositionType { get { return verticalPositionType; } set { verticalPositionType = value; } }
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return verticalPositionAlignment; } set { verticalPositionAlignment = value; } }
		public Point Offset { get { return offset; } set { offset = value; } }
		public Point PercentOffset { get { return percentOffset; } set { percentOffset = value; } }
		public FloatingObjectRelativeWidth RelativeWidth { get { return relativeWidth; } set { relativeWidth = value; } }
		public FloatingObjectRelativeHeight RelativeHeight { get { return relativeHeight; } set { relativeHeight = value; } }
		public bool IsBehindDoc { get { return isBehindDoc; } set { isBehindDoc = value; } }
		public bool PseudoInline { get { return pseudoInline; } set { pseudoInline = value; } }
		#endregion
		public FloatingObjectInfo Clone() {
			FloatingObjectInfo result = new FloatingObjectInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(FloatingObjectInfo val) {
			this.AllowOverlap = val.AllowOverlap;
			this.Hidden = val.Hidden;
			this.LayoutInTableCell = val.LayoutInTableCell;
			this.Locked = val.Locked;
			this.LockAspectRatio = val.LockAspectRatio;
			this.LeftDistance = val.LeftDistance;
			this.RightDistance = val.RightDistance;
			this.TopDistance = val.TopDistance;
			this.BottomDistance = val.BottomDistance;
			this.ZOrder = val.ZOrder;
			this.ActualSize = val.ActualSize;
			this.TextWrapType = val.TextWrapType;
			this.TextWrapSide = val.TextWrapSide;
			this.HorizontalPositionType = val.HorizontalPositionType;
			this.HorizontalPositionAlignment = val.HorizontalPositionAlignment;
			this.VerticalPositionType = val.VerticalPositionType;
			this.VerticalPositionAlignment = val.VerticalPositionAlignment;
			this.Offset = val.Offset;
			this.IsBehindDoc = val.IsBehindDoc;
			this.PseudoInline = val.PseudoInline;
			this.RelativeWidth = val.RelativeWidth;
			this.RelativeHeight = val.RelativeHeight;
			this.PercentOffset = val.PercentOffset;
		}
		public override bool Equals(object obj) {
			FloatingObjectInfo info = (FloatingObjectInfo)obj;
			return
				this.Offset == info.Offset &&
				this.ActualSize == info.ActualSize &&
				this.AllowOverlap == info.AllowOverlap &&
				this.Hidden == info.Hidden &&
				this.LayoutInTableCell == info.LayoutInTableCell &&
				this.Locked == info.Locked &&
				this.LockAspectRatio == info.LockAspectRatio &&
				this.LeftDistance == info.LeftDistance &&
				this.RightDistance == info.RightDistance &&
				this.TopDistance == info.TopDistance &&
				this.BottomDistance == info.BottomDistance &&
				this.ZOrder == info.ZOrder &&
				this.TextWrapType == info.TextWrapType &&
				this.TextWrapSide == info.TextWrapSide &&
				this.HorizontalPositionType == info.HorizontalPositionType &&
				this.HorizontalPositionAlignment == info.HorizontalPositionAlignment &&
				this.VerticalPositionType == info.VerticalPositionType &&
				this.VerticalPositionAlignment == info.VerticalPositionAlignment &&
				this.IsBehindDoc == info.IsBehindDoc &&
				this.PseudoInline == info.PseudoInline &&
				this.RelativeWidth == info.RelativeWidth &&
				this.RelativeHeight == info.RelativeHeight &&
				this.PercentOffset == info.PercentOffset;
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
	#region FloatingObjectInfoCache
	public class FloatingObjectInfoCache : UniqueItemsCache<FloatingObjectInfo> {
		internal const int DefaultItemIndex = 0;
		public FloatingObjectInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override FloatingObjectInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			FloatingObjectInfo item = new FloatingObjectInfo();
			return item;
		}
	}
	#endregion
	#region FloatingObjectOptions
	public class FloatingObjectOptions : ICloneable<FloatingObjectOptions>, ISupportsCopyFrom<FloatingObjectOptions>, ISupportsSizeOf {
		#region Mask enumeration
		public enum Mask {
			UseNone = 0x00000000,
			UseAllowOverlap = 0x00000001,
			UseHidden = 0x00000002,
			UseLayoutInTableCell = 0x00000004,
			UseLocked = 0x00000008,
			UseLeftDistance = 0x00000010,
			UseRightDistance = 0x00000020,
			UseTopDistance = 0x00000040,
			UseBottomDistance = 0x00000080,
			UseZOrder = 0x00000100,
			UseActualSize = 0x00000200,
			UseTextWrapType = 0x00000400,
			UseTextWrapSide = 0x00000800,
			UseHorizontalPositionType = 0x00001000,
			UseHorizontalPositionAlignment = 0x00002000,
			UseVerticalPositionType = 0x00004000,
			UseVerticalPositionAlignment = 0x00008000,
			UseOffset = 0x00010000,
			UseLockAspectRatio = 0x00020000,
			UseIsBehindDoc = 0x00040000,
			UsePseudoInline = 0x00080000,
			UseRelativeWidth = 0x00100000,
			UseRelativeHeight = 0x00200000,
			UsePercentOffset = 0x00400000,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.UseNone;
		#region Properties
		internal Mask Value { get { return val; } set { val = value; } }
		public bool UseAllowOverlap { get { return GetVal(Mask.UseAllowOverlap); } set { SetVal(Mask.UseAllowOverlap, value); } }
		public bool UseHidden { get { return GetVal(Mask.UseHidden); } set { SetVal(Mask.UseHidden, value); } }
		public bool UseLayoutInTableCell { get { return GetVal(Mask.UseLayoutInTableCell); } set { SetVal(Mask.UseLayoutInTableCell, value); } }
		public bool UseLocked { get { return GetVal(Mask.UseLocked); } set { SetVal(Mask.UseLocked, value); } }
		public bool UseLockAspectRatio { get { return GetVal(Mask.UseLockAspectRatio); } set { SetVal(Mask.UseLockAspectRatio, value); } }
		public bool UseLeftDistance { get { return GetVal(Mask.UseLeftDistance); } set { SetVal(Mask.UseLeftDistance, value); } }
		public bool UseRightDistance { get { return GetVal(Mask.UseRightDistance); } set { SetVal(Mask.UseRightDistance, value); } }
		public bool UseTopDistance { get { return GetVal(Mask.UseTopDistance); } set { SetVal(Mask.UseTopDistance, value); } }
		public bool UseBottomDistance { get { return GetVal(Mask.UseBottomDistance); } set { SetVal(Mask.UseBottomDistance, value); } }
		public bool UseZOrder { get { return GetVal(Mask.UseZOrder); } set { SetVal(Mask.UseZOrder, value); } }
		public bool UseActualSize { get { return GetVal(Mask.UseActualSize); } set { SetVal(Mask.UseActualSize, value); } }
		public bool UseTextWrapType { get { return GetVal(Mask.UseTextWrapType); } set { SetVal(Mask.UseTextWrapType, value); } }
		public bool UseTextWrapSide { get { return GetVal(Mask.UseTextWrapSide); } set { SetVal(Mask.UseTextWrapSide, value); } }
		public bool UseHorizontalPositionType { get { return GetVal(Mask.UseHorizontalPositionType); } set { SetVal(Mask.UseHorizontalPositionType, value); } }
		public bool UseHorizontalPositionAlignment { get { return GetVal(Mask.UseHorizontalPositionAlignment); } set { SetVal(Mask.UseHorizontalPositionAlignment, value); } }
		public bool UseVerticalPositionType { get { return GetVal(Mask.UseVerticalPositionType); } set { SetVal(Mask.UseVerticalPositionType, value); } }
		public bool UseVerticalPositionAlignment { get { return GetVal(Mask.UseVerticalPositionAlignment); } set { SetVal(Mask.UseVerticalPositionAlignment, value); } }
		public bool UseOffset { get { return GetVal(Mask.UseOffset); } set { SetVal(Mask.UseOffset, value); } }
		public bool UseIsBehindDoc { get { return GetVal(Mask.UseIsBehindDoc); } set { SetVal(Mask.UseIsBehindDoc, value); } }
		public bool UsePseudoInline { get { return GetVal(Mask.UsePseudoInline); } set { SetVal(Mask.UsePseudoInline, value); } }
		public bool UseRelativeWidth { get { return GetVal(Mask.UseRelativeWidth); } set { SetVal(Mask.UseRelativeWidth, value); } }
		public bool UseRelativeHeight { get { return GetVal(Mask.UseRelativeHeight); } set { SetVal(Mask.UseRelativeHeight, value); } }
		public bool UsePercentOffset { get { return GetVal(Mask.UsePercentOffset); } set { SetVal(Mask.UsePercentOffset, value); } }
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
		public FloatingObjectOptions() {
		}
		internal FloatingObjectOptions(Mask val) {
			this.val = val;
		}
		public FloatingObjectOptions Clone() {
			return new FloatingObjectOptions(this.val);
		}
		public override bool Equals(object obj) {
			FloatingObjectOptions opts = (FloatingObjectOptions)obj;
			return opts.Value == this.Value;
		}
		public override int GetHashCode() {
			return (int)this.Value;
		}
		public void CopyFrom(FloatingObjectOptions options) {
			this.val = options.Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region FloatingObjectOptionsCache
	public class FloatingObjectOptionsCache : UniqueItemsCache<FloatingObjectOptions> {
		internal const int EmptyFloatingObjectOptionIndex = 0;
		public FloatingObjectOptionsCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override FloatingObjectOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new FloatingObjectOptions();
		}
	}
	#endregion
	#region FloatingObjectFormatting
	public class FloatingObjectFormatting : IndexBasedObjectB<FloatingObjectInfo, FloatingObjectOptions>, ICloneable<FloatingObjectFormatting>, ISupportsCopyFrom<FloatingObjectFormatting> {
		internal FloatingObjectFormatting(PieceTable pieceTable, DocumentModel documentModel, int infoIndex, int optionsIndex)
			: base(pieceTable, documentModel, infoIndex, optionsIndex) {
		}
		protected override UniqueItemsCache<FloatingObjectInfo> InfoCache { get { return ((DocumentModel)DocumentModel).Cache.FloatingObjectInfoCache; } }
		protected override UniqueItemsCache<FloatingObjectOptions> OptionsCache { get { return ((DocumentModel)DocumentModel).Cache.FloatingObjectOptionsCache; } }
		#region AllowOverlap
		public bool AllowOverlap {
			get { return Info.AllowOverlap; }
			set {
				if (Info.AllowOverlap == value && Options.UseAllowOverlap)
					return;
				SetPropertyValue(SetAllowOverlapCore, value, SetUseAllowOverlapCore);
			}
		}
		void SetAllowOverlapCore(FloatingObjectInfo info, bool value) {
			info.AllowOverlap = value;
		}
		void SetUseAllowOverlapCore(FloatingObjectOptions options, bool value) {
			options.UseAllowOverlap = value;
		}
		#endregion
		#region Hidden
		public bool Hidden {
			get { return Info.Hidden; }
			set {
				if (Info.Hidden == value && Options.UseHidden)
					return;
				SetPropertyValue(SetHiddenCore, value, SetUseHiddenCore);
			}
		}
		void SetHiddenCore(FloatingObjectInfo info, bool value) {
			info.Hidden = value;
		}
		void SetUseHiddenCore(FloatingObjectOptions options, bool value) {
			options.UseHidden = value;
		}
		#endregion
		#region LayoutInTableCell
		public bool LayoutInTableCell {
			get { return Info.LayoutInTableCell; }
			set {
				if (Info.LayoutInTableCell == value && Options.UseLayoutInTableCell)
					return;
				SetPropertyValue(SetLayoutInTableCellCore, value, SetUseLayoutInTableCellCore);
			}
		}
		void SetLayoutInTableCellCore(FloatingObjectInfo info, bool value) {
			info.LayoutInTableCell = value;
		}
		void SetUseLayoutInTableCellCore(FloatingObjectOptions options, bool value) {
			options.UseLayoutInTableCell = value;
		}
		#endregion
		#region Locked
		public bool Locked {
			get { return Info.Locked; }
			set {
				if (Info.Locked == value && Options.UseLocked)
					return;
				SetPropertyValue(SetLockedCore, value, SetUseLockedCore);
			}
		}
		void SetLockedCore(FloatingObjectInfo info, bool value) {
			info.Locked = value;
		}
		void SetUseLockedCore(FloatingObjectOptions options, bool value) {
			options.UseLocked = value;
		}
		#endregion
		#region LockAspectRatio
		public bool LockAspectRatio {
			get { return Info.LockAspectRatio; }
			set {
				if (Info.LockAspectRatio == value && Options.UseLockAspectRatio)
					return;
				SetPropertyValue(SetLockAspectRatioCore, value, SetUseLockAspectRatioCore);
			}
		}
		void SetLockAspectRatioCore(FloatingObjectInfo info, bool value) {
			info.LockAspectRatio = value;
		}
		void SetUseLockAspectRatioCore(FloatingObjectOptions options, bool value) {
			options.UseLockAspectRatio = value;
		}
		#endregion
		#region LeftDistance
		public int LeftDistance {
			get { return Info.LeftDistance; }
			set {
				if (Info.LeftDistance == value && Options.UseLeftDistance)
					return;
				SetPropertyValue(SetLeftDistanceCore, value, SetUseLeftDistanceCore);
			}
		}
		void SetLeftDistanceCore(FloatingObjectInfo info, int value) {
			info.LeftDistance = value;
		}
		void SetUseLeftDistanceCore(FloatingObjectOptions options, bool value) {
			options.UseLeftDistance = value;
		}
		#endregion
		#region RightDistance
		public int RightDistance {
			get { return Info.RightDistance; }
			set {
				if (Info.RightDistance == value && Options.UseRightDistance)
					return;
				SetPropertyValue(SetRightDistanceCore, value, SetUseRightDistanceCore);
			}
		}
		void SetRightDistanceCore(FloatingObjectInfo info, int value) {
			info.RightDistance = value;
		}
		void SetUseRightDistanceCore(FloatingObjectOptions options, bool value) {
			options.UseRightDistance = value;
		}
		#endregion
		#region TopDistance
		public int TopDistance {
			get { return Info.TopDistance; }
			set {
				if (Info.TopDistance == value && Options.UseTopDistance)
					return;
				SetPropertyValue(SetTopDistanceCore, value, SetUseTopDistanceCore);
			}
		}
		void SetTopDistanceCore(FloatingObjectInfo info, int value) {
			info.TopDistance = value;
		}
		void SetUseTopDistanceCore(FloatingObjectOptions options, bool value) {
			options.UseTopDistance = value;
		}
		#endregion
		#region BottomDistance
		public int BottomDistance {
			get { return Info.BottomDistance; }
			set {
				if (Info.BottomDistance == value && Options.UseBottomDistance)
					return;
				SetPropertyValue(SetBottomDistanceCore, value, SetUseBottomDistanceCore);
			}
		}
		void SetBottomDistanceCore(FloatingObjectInfo info, int value) {
			info.BottomDistance = value;
		}
		void SetUseBottomDistanceCore(FloatingObjectOptions options, bool value) {
			options.UseBottomDistance = value;
		}
		#endregion
		#region ZOrder
		public int ZOrder {
			get { return Info.ZOrder; }
			set {
				if (Info.ZOrder == value && Options.UseZOrder)
					return;
				SetPropertyValue(SetZOrderCore, value, SetUseZOrderCore);
			}
		}
		void SetZOrderCore(FloatingObjectInfo info, int value) {
			info.ZOrder = value;
		}
		void SetUseZOrderCore(FloatingObjectOptions options, bool value) {
			options.UseZOrder = value;
		}
		#endregion
		#region ActualSize
		public Size ActualSize {
			get { return Info.ActualSize; }
			set {
				if (Info.ActualSize == value && Options.UseActualSize)
					return;
				SetPropertyValue(SetActualSizeCore, value, SetUseActualSizeCore);
			}
		}
		void SetActualSizeCore(FloatingObjectInfo info, Size value) {
			info.ActualSize = value;
		}
		void SetUseActualSizeCore(FloatingObjectOptions options, bool value) {
			options.UseActualSize = value;
		}
		#endregion
		#region RelativeWidth
		public FloatingObjectRelativeWidth RelativeWidth {
			get { return Info.RelativeWidth; }
			set {
				if (Info.RelativeWidth == value && Options.UseRelativeWidth)
					return;
				SetPropertyValue(SetRelativeWidthCore, value, SetUseRelativeWidthCore);
			}
		}
		void SetRelativeWidthCore(FloatingObjectInfo info, FloatingObjectRelativeWidth value) {
			info.RelativeWidth = value;
		}
		void SetUseRelativeWidthCore(FloatingObjectOptions options, bool value) {
			options.UseRelativeWidth = value;
		}
		#endregion
		#region RelativeHeight
		public FloatingObjectRelativeHeight RelativeHeight {
			get { return Info.RelativeHeight; }
			set {
				if (Info.RelativeHeight == value && Options.UseRelativeHeight)
					return;
				SetPropertyValue(SetRelativeHeightCore, value, SetUseRelativeHeightCore);
			}
		}
		void SetRelativeHeightCore(FloatingObjectInfo info, FloatingObjectRelativeHeight value) {
			info.RelativeHeight = value;
		}
		void SetUseRelativeHeightCore(FloatingObjectOptions options, bool value) {
			options.UseRelativeHeight = value;
		}
		#endregion
		#region TextWrapType
		public FloatingObjectTextWrapType TextWrapType {
			get { return Info.TextWrapType; }
			set {
				if (Info.TextWrapType == value && Options.UseTextWrapType)
					return;
				SetPropertyValue(SetTextWrapTypeCore, value, SetUseTextWrapTypeCore);
			}
		}
		void SetTextWrapTypeCore(FloatingObjectInfo info, FloatingObjectTextWrapType value) {
			info.TextWrapType = value;
		}
		void SetUseTextWrapTypeCore(FloatingObjectOptions options, bool value) {
			options.UseTextWrapType = value;
		}
		#endregion
		#region TextWrapSide
		public FloatingObjectTextWrapSide TextWrapSide {
			get { return Info.TextWrapSide; }
			set {
				if (Info.TextWrapSide == value && Options.UseTextWrapSide)
					return;
				SetPropertyValue(SetTextWrapSideCore, value, SetUseTextWrapSideCore);
			}
		}
		void SetTextWrapSideCore(FloatingObjectInfo info, FloatingObjectTextWrapSide value) {
			info.TextWrapSide = value;
		}
		void SetUseTextWrapSideCore(FloatingObjectOptions options, bool value) {
			options.UseTextWrapSide = value;
		}
		#endregion
		#region HorizontalPositionType
		public FloatingObjectHorizontalPositionType HorizontalPositionType {
			get { return Info.HorizontalPositionType; }
			set {
				if (Info.HorizontalPositionType == value && Options.UseHorizontalPositionType)
					return;
				SetPropertyValue(SetHorizontalPositionTypeCore, value, SetUseHorizontalPositionTypeCore);
			}
		}
		void SetHorizontalPositionTypeCore(FloatingObjectInfo info, FloatingObjectHorizontalPositionType value) {
			info.HorizontalPositionType = value;
		}
		void SetUseHorizontalPositionTypeCore(FloatingObjectOptions options, bool value) {
			options.UseHorizontalPositionType = value;
		}
		#endregion
		#region HorizontalPositionAlignment
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return Info.HorizontalPositionAlignment; }
			set {
				if (Info.HorizontalPositionAlignment == value && Options.UseHorizontalPositionAlignment)
					return;
				SetPropertyValue(SetHorizontalPositionAlignmentCore, value, SetUseHorizontalPositionAlignmentCore);
			}
		}
		void SetHorizontalPositionAlignmentCore(FloatingObjectInfo info, FloatingObjectHorizontalPositionAlignment value) {
			info.HorizontalPositionAlignment = value;
		}
		void SetUseHorizontalPositionAlignmentCore(FloatingObjectOptions options, bool value) {
			options.UseHorizontalPositionAlignment = value;
		}
		#endregion
		#region VerticalPositionType
		public FloatingObjectVerticalPositionType VerticalPositionType {
			get { return Info.VerticalPositionType; }
			set {
				if (Info.VerticalPositionType == value && Options.UseVerticalPositionType)
					return;
				SetPropertyValue(SetVerticalPositionTypeCore, value, SetUseVerticalPositionTypeCore);
			}
		}
		void SetVerticalPositionTypeCore(FloatingObjectInfo info, FloatingObjectVerticalPositionType value) {
			info.VerticalPositionType = value;
		}
		void SetUseVerticalPositionTypeCore(FloatingObjectOptions options, bool value) {
			options.UseVerticalPositionType = value;
		}
		#endregion
		#region VerticalPositionAlignment
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment {
			get { return Info.VerticalPositionAlignment; }
			set {
				if (Info.VerticalPositionAlignment == value && Options.UseVerticalPositionAlignment)
					return;
				SetPropertyValue(SetVerticalPositionAlignmentCore, value, SetUseVerticalPositionAlignmentCore);
			}
		}
		void SetVerticalPositionAlignmentCore(FloatingObjectInfo info, FloatingObjectVerticalPositionAlignment value) {
			info.VerticalPositionAlignment = value;
		}
		void SetUseVerticalPositionAlignmentCore(FloatingObjectOptions options, bool value) {
			options.UseVerticalPositionAlignment = value;
		}
		#endregion
		#region Offset
		public Point Offset {
			get { return Info.Offset; }
			set {
				if (Info.Offset == value && Options.UseOffset)
					return;
				SetPropertyValue(SetOffsetCore, value, SetUseOffsetCore);
			}
		}
		void SetOffsetCore(FloatingObjectInfo info, Point value) {
			info.Offset = value;
		}
		void SetUseOffsetCore(FloatingObjectOptions options, bool value) {
			options.UseOffset = value;
		}
		#endregion
		#region PercentOffset
		public Point PercentOffset {
			get { return Info.PercentOffset; }
			set {
				if (Info.PercentOffset == value && Options.UsePercentOffset)
					return;
				SetPropertyValue(SetPercentOffsetCore, value, SetUsePercentOffsetCore);
			}
		}
		void SetPercentOffsetCore(FloatingObjectInfo info, Point value) {
			info.PercentOffset = value;
		}
		void SetUsePercentOffsetCore(FloatingObjectOptions options, bool value) {
			options.UsePercentOffset = value;
		}
		#endregion
		#region IsBehindDoc
		public bool IsBehindDoc {
			get { return Info.IsBehindDoc; }
			set {
				if (Info.IsBehindDoc == value && Options.UseIsBehindDoc)
					return;
				SetPropertyValue(SetIsBehindDocCore, value, SetUseIsBehindDocCore);
			}
		}
		void SetIsBehindDocCore(FloatingObjectInfo info, bool value) {
			info.IsBehindDoc = value;
		}
		void SetUseIsBehindDocCore(FloatingObjectOptions options, bool value) {
			options.UseIsBehindDoc = value;
		}
		#endregion
		#region PseudoInline
		public bool PseudoInline {
			get { return Info.PseudoInline; }
			set {
				if (Info.PseudoInline == value && Options.UsePseudoInline)
					return;
				SetPropertyValue(SetPseudoInlineCore, value, SetUsePseudoInlineCore);
			}
		}
		void SetPseudoInlineCore(FloatingObjectInfo info, bool value) {
			info.PseudoInline = value;
		}
		void SetUsePseudoInlineCore(FloatingObjectOptions options, bool value) {
			options.UsePseudoInline = value;
		}
		#endregion
		#region ICloneable<FloatingObjectFormatting> Members
		public FloatingObjectFormatting Clone() {
			return new FloatingObjectFormatting(PieceTable, (DocumentModel)DocumentModel, InfoIndex, OptionsIndex);
		}
		#endregion
		public void CopyFrom(FloatingObjectFormatting floatingObjectFormatting) {
			CopyFrom(floatingObjectFormatting.Info, floatingObjectFormatting.Options);
		}
		public void CopyFrom(FloatingObjectInfo info, FloatingObjectOptions options) {
			CopyFromCore(info, options);
		}
		protected override bool PropertyEquals(IndexBasedObject<FloatingObjectInfo, FloatingObjectOptions> other) {
			Guard.ArgumentNotNull(other, "other");
			return Options.Value == other.Options.Value &&
				Info.Equals(other.Info);
		}
		protected override void SetPropertyValue<U>(IndexBasedObjectB<FloatingObjectInfo, FloatingObjectOptions>.SetPropertyValueDelegate<U> setter, U newValue, IndexBasedObjectB<FloatingObjectInfo, FloatingObjectOptions>.SetOptionsValueDelegate optionsSetter) {
			if (((DocumentModel)DocumentModel).DocumentCapabilities.FloatingObjectsAllowed)
				base.SetPropertyValue<U>(setter, newValue, optionsSetter);
		}
	}
	#endregion
	#region FloatingObjectFormattingCache
	public class FloatingObjectFormattingCache : UniqueItemsCache<FloatingObjectFormatting> {
		#region Fields
		public const int EmptyFloatingObjectFormattingIndex = 0;
		readonly DocumentModel documentModel;
		#endregion
		public FloatingObjectFormattingCache(DocumentModel documentModel)
			: base(documentModel.UnitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			AppendItem(new FloatingObjectFormatting(DocumentModel.MainPieceTable, DocumentModel, 0, FloatingObjectOptionsCache.EmptyFloatingObjectOptionIndex));
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected override FloatingObjectFormatting CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null;
		}
	}
	#endregion
	public interface IFloatingObjectLocation {
		int OffsetX { get; }
		int OffsetY { get; }
		int PercentOffsetX { get; }
		int PercentOffsetY { get; }
		FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get; }
		FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get; }
		FloatingObjectHorizontalPositionType HorizontalPositionType { get; }
		FloatingObjectVerticalPositionType VerticalPositionType { get; }
		FloatingObjectTextWrapType TextWrapType { get; }
		int ActualWidth { get; }
		int ActualHeight { get; }
		bool UseRelativeWidth { get; }
		bool UseRelativeHeight { get; }
		FloatingObjectRelativeWidth RelativeWidth { get; }
		FloatingObjectRelativeHeight RelativeHeight { get; }
	}
	public class ExplicitFloatingObjectLocation : IFloatingObjectLocation {
		int offsetX;
		int offsetY;
		FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment;
		FloatingObjectVerticalPositionAlignment verticalPositionAlignment;
		FloatingObjectHorizontalPositionType horizontalPositionType;
		FloatingObjectVerticalPositionType verticalPositionType;
		FloatingObjectTextWrapType textWrapType;
		int actualWidth;
		int actualHeight;
		#region IFloatingObjectLocation Members
		public int OffsetX { get { return offsetX; } set { offsetX = value; } }
		public int OffsetY { get { return offsetY; } set { offsetY = value; } }
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return horizontalPositionAlignment; } set { horizontalPositionAlignment = value; } }
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return verticalPositionAlignment; } set { verticalPositionAlignment = value; } }
		public FloatingObjectHorizontalPositionType HorizontalPositionType { get { return horizontalPositionType; } set { horizontalPositionType = value; } }
		public FloatingObjectVerticalPositionType VerticalPositionType { get { return verticalPositionType; } set { verticalPositionType = value; } }
		public FloatingObjectTextWrapType TextWrapType { get { return textWrapType; } set { textWrapType = value; } }
		public int ActualWidth { get { return actualWidth; } set { actualWidth = value; } }
		public int ActualHeight { get { return actualHeight; } set { actualHeight = value; } }
		public bool UseRelativeWidth { get; set; }
		public bool UseRelativeHeight { get; set; }
		public FloatingObjectRelativeWidth RelativeWidth { get; set; }
		public FloatingObjectRelativeHeight RelativeHeight { get; set; }
		public int PercentOffsetX { get; set; }
		public int PercentOffsetY { get; set; }
		#endregion
	}
	#region IFloatingObjectPropertiesContainer
	public interface IFloatingObjectPropertiesContainer {
		PieceTable PieceTable { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateFloatingObjectPropertiesChangedHistoryItem();
		void OnFloatingObjectChanged();
	}
	#endregion
	#region FloatingObjectProperties
	public class FloatingObjectProperties : RichEditIndexBasedObject<FloatingObjectFormatting>, IZOrderedObject, IFloatingObjectLocation {
		readonly IFloatingObjectPropertiesContainer owner;
		bool disableHistory;
		public FloatingObjectProperties(IFloatingObjectPropertiesContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(IFloatingObjectPropertiesContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		#region Properties
		public bool DisableHistory { get { return disableHistory; } set { disableHistory = value; } }
		#region AllowOverlap
		public bool AllowOverlap {
			get { return Info.AllowOverlap; }
			set {
				if (Info.AllowOverlap == value && UseAllowOverlap)
					return;
				SetPropertyValue(SetAllowOverlapCore, value);
			}
		}
		public bool UseAllowOverlap { get { return Info.Options.UseAllowOverlap; } }
		protected internal virtual DocumentModelChangeActions SetAllowOverlapCore(FloatingObjectFormatting info, bool value) {
			info.AllowOverlap = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.AllowOverlap);
		}
		#endregion
		#region Hidden
		public bool Hidden {
			get { return Info.Hidden; }
			set {
				if (Info.Hidden == value && UseHidden)
					return;
				SetPropertyValue(SetHiddenCore, value);
			}
		}
		public bool UseHidden { get { return Info.Options.UseHidden; } }
		protected internal virtual DocumentModelChangeActions SetHiddenCore(FloatingObjectFormatting info, bool value) {
			info.Hidden = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.Hidden);
		}
		#endregion
		#region LayoutInTableCell
		public bool LayoutInTableCell {
			get { return Info.LayoutInTableCell; }
			set {
				if (Info.LayoutInTableCell == value && UseLayoutInTableCell)
					return;
				SetPropertyValue(SetLayoutInTableCellCore, value);
			}
		}
		public bool UseLayoutInTableCell { get { return Info.Options.UseLayoutInTableCell; } }
		protected internal virtual DocumentModelChangeActions SetLayoutInTableCellCore(FloatingObjectFormatting info, bool value) {
			info.LayoutInTableCell = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.LayoutInTableCell);
		}
		#endregion
		#region Locked
		public bool Locked {
			get { return Info.Locked; }
			set {
				if (Info.Locked == value && UseLocked)
					return;
				SetPropertyValue(SetLockedCore, value);
			}
		}
		public bool UseLocked { get { return Info.Options.UseLocked; } }
		protected internal virtual DocumentModelChangeActions SetLockedCore(FloatingObjectFormatting info, bool value) {
			info.Locked = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.Locked);
		}
		#endregion
		#region LockAspectRatio
		public bool LockAspectRatio {
			get { return Info.LockAspectRatio; }
			set {
				if (Info.LockAspectRatio == value && UseLockAspectRatio)
					return;
				SetPropertyValue(SetLockAspectRatioCore, value);
			}
		}
		public bool UseLockAspectRatio { get { return Info.Options.UseLockAspectRatio; } }
		protected internal virtual DocumentModelChangeActions SetLockAspectRatioCore(FloatingObjectFormatting info, bool value) {
			info.LockAspectRatio = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.LockAspectRatio);
		}
		#endregion
		#region LeftDistance
		public int LeftDistance {
			get { return Info.LeftDistance; }
			set {
				if (Info.LeftDistance == value && UseLeftDistance)
					return;
				SetPropertyValue(SetLeftDistanceCore, value);
			}
		}
		public bool UseLeftDistance { get { return Info.Options.UseLeftDistance; } }
		protected internal virtual DocumentModelChangeActions SetLeftDistanceCore(FloatingObjectFormatting info, int value) {
			info.LeftDistance = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.LeftDistance);
		}
		#endregion
		#region RightDistance
		public int RightDistance {
			get { return Info.RightDistance; }
			set {
				if (Info.RightDistance == value && UseRightDistance)
					return;
				SetPropertyValue(SetRightDistanceCore, value);
			}
		}
		public bool UseRightDistance { get { return Info.Options.UseRightDistance; } }
		protected internal virtual DocumentModelChangeActions SetRightDistanceCore(FloatingObjectFormatting info, int value) {
			info.RightDistance = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.RightDistance);
		}
		#endregion
		#region TopDistance
		public int TopDistance {
			get { return Info.TopDistance; }
			set {
				if (Info.TopDistance == value && UseTopDistance)
					return;
				SetPropertyValue(SetTopDistanceCore, value);
			}
		}
		public bool UseTopDistance { get { return Info.Options.UseTopDistance; } }
		protected internal virtual DocumentModelChangeActions SetTopDistanceCore(FloatingObjectFormatting info, int value) {
			info.TopDistance = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.TopDistance);
		}
		#endregion
		#region BottomDistance
		public int BottomDistance {
			get { return Info.BottomDistance; }
			set {
				if (Info.BottomDistance == value && UseBottomDistance)
					return;
				SetPropertyValue(SetBottomDistanceCore, value);
			}
		}
		public bool UseBottomDistance { get { return Info.Options.UseBottomDistance; } }
		protected internal virtual DocumentModelChangeActions SetBottomDistanceCore(FloatingObjectFormatting info, int value) {
			info.BottomDistance = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.BottomDistance);
		}
		#endregion
		#region ZOrder
		public int ZOrder {
			get { return Info.ZOrder; }
			set {
				if (Info.ZOrder == value)
					return;
				SetPropertyValue(SetZOrderCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetZOrderCore(FloatingObjectFormatting info, int value) {
			info.ZOrder = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.ZOrder);
		}
		#endregion
		#region ActualSize
		public Size ActualSize {
			get { return Info.ActualSize; }
			set {
				if (Info.ActualSize == value && UseActualSize)
					return;
				SetPropertyValue(SetActualSizeCore, value);
			}
		}
		public bool UseActualSize { get { return Info.Options.UseActualSize; } }
		protected internal virtual DocumentModelChangeActions SetActualSizeCore(FloatingObjectFormatting info, Size value) {
			info.ActualSize = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.ActualSize);
		}
		#endregion
		#region ActualWidth
		public int ActualWidth {
			get { return ActualSize.Width; }
			set {
				if (ActualWidth == value && UseActualSize)
					return;
				Size actualSize = ActualSize;
				actualSize.Width = value;
				ActualSize = actualSize;
			}
		}
		#endregion
		#region ActualHeight
		public int ActualHeight {
			get { return ActualSize.Height; }
			set {
				if (ActualHeight == value && UseActualSize)
					return;
				Size actualSize = ActualSize;
				actualSize.Height = value;
				ActualSize = actualSize;
			}
		}
		#endregion
		#region RelativeWidth
		public FloatingObjectRelativeWidth RelativeWidth {
			get { return Info.RelativeWidth; }
			set {
				if (Info.RelativeWidth == value && UseRelativeWidth)
					return;
				SetPropertyValue(SetRelativeWidthCore, value);
			}
		}
		public bool UseRelativeWidth { get { return Info.Options.UseRelativeWidth; } }
		protected internal virtual DocumentModelChangeActions SetRelativeWidthCore(FloatingObjectFormatting info, FloatingObjectRelativeWidth value) {
			info.RelativeWidth = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.RelativeWidth);
		}
		#endregion
		#region RelativeHeight
		public FloatingObjectRelativeHeight RelativeHeight {
			get { return Info.RelativeHeight; }
			set {
				if (Info.RelativeHeight == value && UseRelativeHeight)
					return;
				SetPropertyValue(SetRelativeHeightCore, value);
			}
		}
		public bool UseRelativeHeight { get { return Info.Options.UseRelativeHeight; } }
		protected internal virtual DocumentModelChangeActions SetRelativeHeightCore(FloatingObjectFormatting info, FloatingObjectRelativeHeight value) {
			info.RelativeHeight = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.RelativeHeight);
		}
		#endregion
		#region TextWrapType
		public FloatingObjectTextWrapType TextWrapType {
			get { return Info.TextWrapType; }
			set {
				if (Info.TextWrapType == value && UseTextWrapType)
					return;
				SetPropertyValue(SetTextWrapTypeCore, value);
			}
		}
		public bool UseTextWrapType { get { return Info.Options.UseTextWrapType; } }
		protected internal virtual DocumentModelChangeActions SetTextWrapTypeCore(FloatingObjectFormatting info, FloatingObjectTextWrapType value) {
			info.TextWrapType = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.TextWrapType);
		}
		#endregion
		#region TextWrapSide
		public FloatingObjectTextWrapSide TextWrapSide {
			get { return Info.TextWrapSide; }
			set {
				if (Info.TextWrapSide == value && UseTextWrapSide)
					return;
				SetPropertyValue(SetTextWrapSideCore, value);
			}
		}
		public bool UseTextWrapSide { get { return Info.Options.UseTextWrapSide; } }
		protected internal virtual DocumentModelChangeActions SetTextWrapSideCore(FloatingObjectFormatting info, FloatingObjectTextWrapSide value) {
			info.TextWrapSide = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.TextWrapSide);
		}
		#endregion
		#region HorizontalPositionType
		public FloatingObjectHorizontalPositionType HorizontalPositionType {
			get { return Info.HorizontalPositionType; }
			set {
				if (Info.HorizontalPositionType == value && UseHorizontalPositionType)
					return;
				SetPropertyValue(SetHorizontalPositionTypeCore, value);
			}
		}
		public bool UseHorizontalPositionType { get { return Info.Options.UseHorizontalPositionType; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalPositionTypeCore(FloatingObjectFormatting info, FloatingObjectHorizontalPositionType value) {
			info.HorizontalPositionType = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.HorizontalPositionType);
		}
		#endregion
		#region HorizontalPositionAlignment
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return Info.HorizontalPositionAlignment; }
			set {
				if (Info.HorizontalPositionAlignment == value && UseHorizontalPositionAlignment)
					return;
				SetPropertyValue(SetHorizontalPositionAlignmentCore, value);
			}
		}
		public bool UseHorizontalPositionAlignment { get { return Info.Options.UseHorizontalPositionAlignment; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalPositionAlignmentCore(FloatingObjectFormatting info, FloatingObjectHorizontalPositionAlignment value) {
			info.HorizontalPositionAlignment = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.HorizontalPositionAlignment);
		}
		#endregion
		#region VerticalPositionType
		public FloatingObjectVerticalPositionType VerticalPositionType {
			get { return Info.VerticalPositionType; }
			set {
				if (Info.VerticalPositionType == value && UseVerticalPositionType)
					return;
				SetPropertyValue(SetVerticalPositionTypeCore, value);
			}
		}
		public bool UseVerticalPositionType { get { return Info.Options.UseVerticalPositionType; } }
		protected internal virtual DocumentModelChangeActions SetVerticalPositionTypeCore(FloatingObjectFormatting info, FloatingObjectVerticalPositionType value) {
			info.VerticalPositionType = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.VerticalPositionType);
		}
		#endregion
		#region VerticalPositionAlignment
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment {
			get { return Info.VerticalPositionAlignment; }
			set {
				if (Info.VerticalPositionAlignment == value && UseVerticalPositionAlignment)
					return;
				SetPropertyValue(SetVerticalPositionAlignmentCore, value);
			}
		}
		public bool UseVerticalPositionAlignment { get { return Info.Options.UseVerticalPositionAlignment; } }
		protected internal virtual DocumentModelChangeActions SetVerticalPositionAlignmentCore(FloatingObjectFormatting info, FloatingObjectVerticalPositionAlignment value) {
			info.VerticalPositionAlignment = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.VerticalPositionAlignment);
		}
		#endregion
		#region Offset
		public Point Offset {
			get { return Info.Offset; }
			set {
				if (Info.Offset == value && UseOffset)
					return;
				SetPropertyValue(SetOffsetCore, value);
			}
		}
		public bool UseOffset { get { return Info.Options.UseOffset; } }
		protected internal virtual DocumentModelChangeActions SetOffsetCore(FloatingObjectFormatting info, Point value) {
			info.Offset = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.Offset);
		}
		#endregion
		#region OffsetX
		public int OffsetX {
			get { return Offset.X; }
			set {
				if (OffsetX == value && UseOffset)
					return;
				Point offset = Offset;
				offset.X = value;
				Offset = offset;
			}
		}
		#endregion
		#region OffsetY
		public int OffsetY {
			get { return Offset.Y; }
			set {
				if (OffsetY == value && UseOffset)
					return;
				Point offset = Offset;
				offset.Y = value;
				Offset = offset;
			}
		}
		#endregion
		#region PercentOffset
		public Point PercentOffset {
			get { return Info.PercentOffset; }
			set {
				if (Info.PercentOffset == value && UsePercentOffset)
					return;
				SetPropertyValue(SetPercentOffsetCore, value);
			}
		}
		public bool UsePercentOffset { get { return Info.Options.UsePercentOffset; } }
		protected internal virtual DocumentModelChangeActions SetPercentOffsetCore(FloatingObjectFormatting info, Point value) {
			info.PercentOffset = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.PercentOffset);
		}
		#endregion
		#region PercentOffsetX
		public int PercentOffsetX {
			get { return PercentOffset.X; }
			set {
				if (PercentOffsetX == value && UsePercentOffset)
					return;
				Point percentOffset = PercentOffset;
				percentOffset.X = value;
				PercentOffset = percentOffset;
			}
		}
		#endregion
		#region PercentOffsetY
		public int PercentOffsetY {
			get { return PercentOffset.Y; }
			set {
				if (PercentOffsetY == value && UsePercentOffset)
					return;
				Point percentOffset = PercentOffset;
				percentOffset.Y = value;
				PercentOffset = percentOffset;
			}
		}
		#endregion
		protected bool AllowTextWrapAround { get { return TextWrapType == FloatingObjectTextWrapType.Square || TextWrapType == FloatingObjectTextWrapType.Through || TextWrapType == FloatingObjectTextWrapType.Tight; } }
		#region CanPutTextAtLeft
		public bool CanPutTextAtLeft {
			get {
				return AllowTextWrapAround && TextWrapSide != FloatingObjectTextWrapSide.Right;
			}
		}
		#endregion
		#region CanPutTextAtRight
		public bool CanPutTextAtRight {
			get {
				return AllowTextWrapAround && TextWrapSide != FloatingObjectTextWrapSide.Left;
			}
		}
		#endregion
		#region PutTextAtLargestSide
		public bool PutTextAtLargestSide { get { return AllowTextWrapAround && TextWrapSide == FloatingObjectTextWrapSide.Largest; } }
		#endregion
		#region IsBehindDoc
		public bool IsBehindDoc {
			get { return Info.IsBehindDoc; }
			set {
				if (Info.IsBehindDoc == value)
					return;
				SetPropertyValue(SetIsBehindDocCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetIsBehindDocCore(FloatingObjectFormatting info, bool value) {
			info.IsBehindDoc = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.IsBehindDoc);
		}
		#endregion
		#region PseudoInline
		public bool PseudoInline {
			get { return Info.PseudoInline; }
			set {
				if (Info.PseudoInline == value && UsePseudoInline)
					return;
				SetPropertyValue(SetPseudoInlineCore, value);
			}
		}
		public bool UsePseudoInline { get { return Info.Options.UsePseudoInline; } }
		protected internal virtual DocumentModelChangeActions SetPseudoInlineCore(FloatingObjectFormatting info, bool value) {
			info.PseudoInline = value;
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.PseudoInline);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<FloatingObjectFormatting> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.FloatingObjectFormattingCache;
		}
		protected internal bool UseVal(FloatingObjectOptions.Mask mask) {
			return (Info.Options.Value & mask) != 0;
		}
		public override void ChangeIndexCore(int newIndex, DocumentModelChangeActions changeActions) {
			if (disableHistory)
				SetIndexInitial(newIndex);
			else
				base.ChangeIndexCore(newIndex, changeActions);
		}
		public void Reset() {
			FloatingObjectFormatting info = GetInfoForModification();
			FloatingObjectFormatting emptyInfo = GetCache(DocumentModel)[FloatingObjectFormattingCache.EmptyFloatingObjectFormattingIndex];
			info.ReplaceInfo(emptyInfo.Info, emptyInfo.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override bool Equals(object obj) {
			FloatingObjectProperties other = obj as FloatingObjectProperties;
			if (ReferenceEquals(obj, null))
				return false;
			if (DocumentModel == other.DocumentModel)
				return Index == other.Index;
			else
				return Info.Equals(other.Info);
		}
		internal void ResetUse(FloatingObjectOptions.Mask mask) {
			FloatingObjectFormatting info = GetInfoForModification();
			FloatingObjectOptions options = info.GetOptionsForModification();
			options.Value &= ~mask;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetAllUse() {
			FloatingObjectFormatting info = GetInfoForModification();
			FloatingObjectOptions options = info.GetOptionsForModification();
			options.Value = FloatingObjectOptions.Mask.UseNone;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override int GetHashCode() {
			return Index;
		}
#if DEBUGTEST
		public override string ToString() {
			return String.Format("FloatingObjectInfoIndex:{0}, InfoIndex:{1}, OptionsIndex:{2}", Index, Info.InfoIndex, Info.OptionsIndex);
		}
#endif
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return FloatingObjectChangeActionsCalculator.CalculateChangeActions(FloatingObjectChangeType.BatchUpdate);
		}
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			owner.OnFloatingObjectChanged();
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateFloatingObjectPropertiesChangedHistoryItem();
		}
		protected override void OnFirstBeginUpdateCore() {
			base.OnFirstBeginUpdateCore();
			DeferredInfo.BeginUpdate();
		}
		protected override void OnLastEndUpdateCore() {
			DeferredInfo.EndUpdate();
			base.OnLastEndUpdateCore();
		}
	}
	#endregion
	#region FloatingObjectChangeType
	public enum FloatingObjectChangeType {
		None = 0,
		AllowOverlap,
		Hidden,
		LayoutInTableCell,
		Locked,
		LockAspectRatio,
		LeftDistance,
		RightDistance,
		TopDistance,
		BottomDistance,
		ZOrder,
		ActualSize,
		TextWrapType,
		TextWrapSide,
		HorizontalPositionType,
		HorizontalPositionAlignment,
		VerticalPositionType,
		VerticalPositionAlignment,
		Offset,
		PercentOffset,
		BatchUpdate,
		IsBehindDoc,
		PseudoInline,
		RelativeWidth,
		RelativeHeight
	}
	#endregion
	#region FloatingObjectChangeActionsCalculator
	public static class FloatingObjectChangeActionsCalculator {
		internal class FloatingObjectChangeActionsTable : Dictionary<FloatingObjectChangeType, DocumentModelChangeActions> {
		}
		internal static readonly FloatingObjectChangeActionsTable floatingObjectChangeActionsTable = CreateFloatingObjectChangeActionsTable();
		internal static FloatingObjectChangeActionsTable CreateFloatingObjectChangeActionsTable() {
			FloatingObjectChangeActionsTable table = new FloatingObjectChangeActionsTable();
			table.Add(FloatingObjectChangeType.None, DocumentModelChangeActions.None);
			table.Add(FloatingObjectChangeType.AllowOverlap, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.Hidden, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.LayoutInTableCell, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.Locked, DocumentModelChangeActions.RaiseContentChanged);
			table.Add(FloatingObjectChangeType.LockAspectRatio, DocumentModelChangeActions.RaiseContentChanged);
			table.Add(FloatingObjectChangeType.LeftDistance, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.RightDistance, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.TopDistance, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.BottomDistance, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.ZOrder, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.ActualSize, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.TextWrapType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.TextWrapSide, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.HorizontalPositionType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.HorizontalPositionAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.VerticalPositionType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.VerticalPositionAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.Offset, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.PercentOffset, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.IsBehindDoc, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.PseudoInline, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.RelativeWidth, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(FloatingObjectChangeType.RelativeHeight, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(FloatingObjectChangeType change) {
			return floatingObjectChangeActionsTable[change];
		}
	}
	#endregion
	#region FloatingObjectCollection
	public class FloatingObjectCollection : List<FloatingObjectProperties> {
	}
	#endregion
	#region FloatingObject
	public class FloatingObject : IFloatingObjectPropertiesContainer, IShapeContainer {
		#region Fields
		readonly FloatingObjectAnchorRun anchorRun;
		readonly FloatingObjectProperties properties;
		readonly Shape shape;
		FloatingObjectContent content = FloatingObjectContent.Empty;
		#endregion
		public FloatingObject(FloatingObjectAnchorRun anchorRun) {
			Guard.ArgumentNotNull(anchorRun, "anchorRun");
			this.anchorRun = anchorRun;
			this.properties = new FloatingObjectProperties(this);
			this.properties.ObtainAffectedRange += OnObtainAffectedRange;
			this.shape = new Shape(this);
			this.shape.ObtainAffectedRange += OnObtainAffectedRange;
		}
		#region Properties
		public FloatingObjectProperties Properties { get { return properties; } }
		public Shape Shape { get { return shape; } }
		public FloatingObjectContent Content { get { return content; } }
		public PieceTable PieceTable { get { return AnchorRun.PieceTable; } }
		public FloatingObjectAnchorRun AnchorRun { get { return anchorRun; } }
		DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		#endregion
		#region IFloatingObjectPropertiesContainer Members
		PieceTable IFloatingObjectPropertiesContainer.PieceTable { get { return this.PieceTable; } }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IFloatingObjectPropertiesContainer.CreateFloatingObjectPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(PieceTable, Properties);
		}
		void IFloatingObjectPropertiesContainer.OnFloatingObjectChanged() {
		}
		#endregion
		#region IShapeContainer Members
		PieceTable IShapeContainer.PieceTable { get { return this.PieceTable; } }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IShapeContainer.CreateShapeChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(PieceTable, Shape);
		}
		void IShapeContainer.OnShapeChanged() {
		}
		#endregion
		void OnObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			AnchorRun.OnCharacterPropertiesObtainAffectedRange(sender, e);
		}
		internal void SetContent(FloatingObjectContent content) {
			Guard.ArgumentNotNull(content, "content");
			Debug.Assert(Content is EmptyFloatingObjectContent);
			Debug.Assert(Object.ReferenceEquals(content.Run, AnchorRun));
			AssignTextBoxAnchorRun(null);
			this.content = content;
			AssignTextBoxAnchorRun(AnchorRun);
			RegisterTextBoxPieceTable();
		}
		void AssignTextBoxAnchorRun(FloatingObjectAnchorRun run) {
			TextBoxFloatingObjectContent textBoxContent = content as TextBoxFloatingObjectContent;
			if (textBoxContent != null)
				textBoxContent.SetAnchorRun(run);
		}
		void RegisterTextBoxPieceTable() {
			TextBoxFloatingObjectContent textBoxContent = content as TextBoxFloatingObjectContent;
			if (textBoxContent != null && textBoxContent.TextBox != null)
				PieceTable.TextBoxes.Add(textBoxContent.TextBox);
		}
		protected internal virtual void BeforeRunRemoved() {
			TextBoxFloatingObjectContent textBoxContent = content as TextBoxFloatingObjectContent;
			if (textBoxContent != null && IsTextBoxActive(textBoxContent.TextBox))
				DeactivateTextBox();
			AssignTextBoxAnchorRun(null);
		}
		bool IsTextBoxActive(TextBoxContentType textBox) {
			return Object.ReferenceEquals(textBox.PieceTable, DocumentModel.ActivePieceTable);
		}
		void DeactivateTextBox() {
			if (PieceTable.IsHeaderFooter) {
				SectionHeaderFooterBase headerFooter = PieceTable.ContentType as SectionHeaderFooterBase;
				DocumentModel.SetActivePieceTable(PieceTable, headerFooter.GetSection());
			}
			else
				DocumentModel.SetActivePieceTable(DocumentModel.MainPieceTable, null);
			SetSelectionByAnchorPosition();
		}
		void SetSelectionByAnchorPosition() {
			if (!Object.ReferenceEquals(PieceTable, DocumentModel.ActivePieceTable))
				return;
			RunIndex runIndex = AnchorRun.GetRunIndex();
			DocumentModelPosition anchorPosition = DocumentModelPosition.FromRunStart(PieceTable, runIndex);
			DocumentModel.Selection.Start = anchorPosition.LogPosition;
			DocumentModel.Selection.End = anchorPosition.LogPosition;
		}
		protected internal virtual void AfterRunInserted() {
			AssignTextBoxAnchorRun(AnchorRun);
		}
		public virtual void CopyFrom(FloatingObject floatingObject, DocumentModelCopyManager copyManager) {
			Properties.CopyFrom(floatingObject.Properties.Info);
			Shape.CopyFrom(floatingObject.Shape.Info);
			SetContent(floatingObject.Content.Clone(AnchorRun, copyManager));
			TextBoxFloatingObjectContent textBoxContent = content as TextBoxFloatingObjectContent;
			if (textBoxContent != null && textBoxContent.TextBox != null)
				textBoxContent.TextBox.PieceTable.FieldUpdater.UpdateFields(DocumentModel.ModelForExport ? UpdateFieldOperationType.CreateModelForExport : UpdateFieldOperationType.Copy);
		}
	}
	#endregion
	#region FloatingObjectAnchorRun
	public class FloatingObjectAnchorRun : TextRunBase, IPictureContainerRun, IRectangularScalableObject {
		readonly FloatingObject floatingObject;
		string name = String.Empty;
		bool excludeFromLayout;
		public FloatingObjectAnchorRun(Paragraph paragraph)
			: base(paragraph) {
			this.floatingObject = new FloatingObject(this);
		}
		#region Properties
		public FloatingObjectProperties FloatingObjectProperties { get { return floatingObject.Properties; } }
		public FloatingObject FloatingObject { get { return floatingObject; } }
		public Shape Shape { get { return floatingObject.Shape; } }
		public FloatingObjectContent Content { get { return floatingObject.Content; } }
		public override bool CanPlaceCaretBefore { get { return true; } }
		public string Name { get { return name; } set { name = value; } }
		public bool ExcludeFromLayout { get { return excludeFromLayout; } set { excludeFromLayout = value; } }
		#endregion
		internal void SetContent(FloatingObjectContent content) {
			floatingObject.SetContent(content);
		}
		protected internal override void BeforeRunRemoved() {
			floatingObject.BeforeRunRemoved();
		}
		protected internal override void AfterRunInserted() {
			floatingObject.AfterRunInserted();
		}
		public override bool CanJoinWith(TextRunBase nextRun) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			boxInfo.Size = Size.Empty;
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable targetPieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			Debug.Assert(this.DocumentModel == copyManager.SourceModel);
			Debug.Assert(targetPosition.PieceTable == targetPieceTable);
			Debug.Assert(targetPosition.RunOffset == 0);
			if (!copyManager.TargetModel.DocumentCapabilities.FloatingObjectsAllowed) {
				targetPieceTable.InsertText(targetPosition.LogPosition, " ");
				return targetPieceTable.Runs[targetPosition.RunIndex];
			}
			targetPieceTable.DocumentModel.BeginUpdate();
			FloatingObjectAnchorRun run;
			try {
				run = targetPieceTable.InsertFloatingObjectAnchorCore(targetPosition.ParagraphIndex, targetPosition.LogPosition);
				run.floatingObject.CopyFrom(this.floatingObject, copyManager);
				run.Name = this.Name;
			}
			finally {
				targetPieceTable.DocumentModel.EndUpdate();
			}
			return run;
		}
		#region IPictureContainerRun Members
		TextRunBase IPictureContainerRun.Run { get { return this; } }
		Size IPictureContainerRun.ActualSize { get { return FloatingObjectProperties.ActualSize; } set { FloatingObjectProperties.ActualSize = value; } }
		void IPictureContainerRun.SetActualSizeInternal(Size actualSize) { ((IPictureContainerRun)this).ActualSize = actualSize; }
		SizeF IPictureContainerRun.ActualSizeF { get { return FloatingObjectProperties.ActualSize; } }
		DocumentModelChangeActions IPictureContainerRun.GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler;
		}
		#endregion
		#region IRectangularObject
		Size IRectangularObject.ActualSize {
			get { return FloatingObjectProperties.ActualSize; }
			set {
				FloatingObjectProperties.ActualSize = value;
			}
		}
		#endregion
		#region IRectangularScalableObject Members
		Size IRectangularScalableObject.OriginalSize { get { return Content.OriginalSize; } }
		float IRectangularScalableObject.ScaleX {
			get { return 100.0f * FloatingObjectProperties.ActualSize.Width / Math.Max(1, Content.OriginalSize.Width); }
			set {
				int val = (int)Math.Round(Math.Abs(value) * Math.Max(1, Content.OriginalSize.Width) / 100.0f);
				if (FloatingObjectProperties.ActualSize.Width == val)
					return;
				FloatingObjectProperties.ActualSize = new Size(val, FloatingObjectProperties.ActualSize.Height);
			}
		}
		float IRectangularScalableObject.ScaleY {
			get { return 100.0f * FloatingObjectProperties.ActualSize.Height / Math.Max(1, Content.OriginalSize.Height); }
			set {
				int val = (int)Math.Round(Math.Abs(value) * Math.Max(1, Content.OriginalSize.Height) / 100.0f);
				if (FloatingObjectProperties.ActualSize.Height == val)
					return;
				FloatingObjectProperties.ActualSize = new Size(FloatingObjectProperties.ActualSize.Width, val);
			}
		}
		#endregion
		void OnObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			OnCharacterPropertiesObtainAffectedRange(sender, e);
		}
		public void Select() {
			DocumentLogPosition logPosition = PieceTable.GetRunLogPosition(this);
			Selection selection = DocumentModel.Selection;
			selection.BeginUpdate();
			try {
				selection.Start = logPosition;
				selection.End = logPosition + 1;
			}
			finally {
				selection.EndUpdate();
			}
		}
		#region IPictureContainerRun Members
		public PictureFloatingObjectContent PictureContent {
			get { return Content as PictureFloatingObjectContent; }
		}
		#endregion
		#region IPictureContainerRun Members
		float IPictureContainerRun.ScaleX {
			get {
				if (PictureContent == null)
					return 100;
				else if (PictureContent.InvalidActualSize)
					return 100;
				else
					return Math.Max(1, 100 * FloatingObjectProperties.ActualSize.Width / PictureContent.OriginalSize.Width);
			}
		}
		float IPictureContainerRun.ScaleY {
			get {
				if (PictureContent == null)
					return 100;
				else if (PictureContent.InvalidActualSize)
					return 100;
				else
					return Math.Max(1, 100 * FloatingObjectProperties.ActualSize.Height / PictureContent.OriginalSize.Height);
			}
		}
		#endregion
		#region IPictureContainerRun Members
		DocumentModel IPictureContainerRun.DocumentModel { get { return this.DocumentModel; } }
		#endregion
	}
	#endregion
	public class FloatingObjectDrawingObject : IOpenXMLDrawingObject {
		FloatingObjectAnchorRun run;
		bool isFloatingObject = true;
		public FloatingObjectDrawingObject(FloatingObjectAnchorRun run) {
			this.run = run;
		}
		#region IOpenXMLDrawingObject Members
		public bool IsFloatingObject { get { return isFloatingObject; } } 
		public string Name {
			get { return run.Name; }
		}
		public Size ActualSize {
			get { return run.FloatingObjectProperties.ActualSize; }
		}
		public int Rotation {
			get { return run.Shape.Rotation; }
		}
		public bool LockAspectRatio {
			get { return run.FloatingObjectProperties.LockAspectRatio; }
		}
		public bool AllowOverlap {
			get { return run.FloatingObjectProperties.AllowOverlap; }
		}
		public bool IsBehindDoc {
			get { return run.FloatingObjectProperties.IsBehindDoc; }
		}
		public bool LayoutInTableCell {
			get { return run.FloatingObjectProperties.LayoutInTableCell; }
		}
		public bool Locked {
			get { return run.FloatingObjectProperties.Locked; }
		}
		public int ZOrder {
			get { return run.FloatingObjectProperties.ZOrder; }
		}
		public bool UseBottomDistance {
			get { return run.FloatingObjectProperties.UseBottomDistance; }
		}
		public int BottomDistance {
			get { return run.FloatingObjectProperties.BottomDistance; }
		}
		public bool UseLeftDistance {
			get { return run.FloatingObjectProperties.UseLeftDistance; }
		}
		public int LeftDistance {
			get { return run.FloatingObjectProperties.LeftDistance; }
		}
		public bool UseRightDistance {
			get { return run.FloatingObjectProperties.UseRightDistance; }
		}
		public int RightDistance {
			get { return run.FloatingObjectProperties.RightDistance; }
		}
		public bool UseTopDistance {
			get { return run.FloatingObjectProperties.UseTopDistance; }
		}
		public int TopDistance {
			get { return run.FloatingObjectProperties.TopDistance; }
		}
		public bool UseHidden {
			get { return run.FloatingObjectProperties.UseHidden; }
		}
		public bool Hidden {
			get { return run.FloatingObjectProperties.Hidden; }
		}
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return run.FloatingObjectProperties.HorizontalPositionAlignment; }
		}
		public bool UsePercentOffset {
			get { return run.FloatingObjectProperties.UsePercentOffset; }
		}
		public int PercentOffsetX {
			get { return run.FloatingObjectProperties.PercentOffsetX; }
		}
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment {
			get { return run.FloatingObjectProperties.VerticalPositionAlignment; }
		}
		public int PercentOffsetY {
			get { return run.FloatingObjectProperties.PercentOffsetY; }
		}
		public bool UseLockAspectRatio {
			get { return run.FloatingObjectProperties.UseLockAspectRatio; }
		}
		public bool UseRotation {
			get { return run.Shape.UseRotation; }
		}
		public FloatingObjectTextWrapType TextWrapType {
			get { return run.FloatingObjectProperties.TextWrapType; }
		}
		public FloatingObjectTextWrapSide TextWrapSide {
			get { return run.FloatingObjectProperties.TextWrapSide; }
		}
		public Shape Shape {
			get { return run.Shape; }
		}
		public bool UseRelativeWidth {
			get { return run.FloatingObjectProperties.UseRelativeWidth; }
		}
		public bool UseRelativeHeight {
			get { return run.FloatingObjectProperties.UseRelativeHeight; }
		}
		public FloatingObjectRelativeWidth RelativeWidth {
			get { return run.FloatingObjectProperties.RelativeWidth; }
		}
		public FloatingObjectRelativeHeight RelativeHeight {
			get { return run.FloatingObjectProperties.RelativeHeight; }
		}
		public FloatingObjectHorizontalPositionType HorizontalPositionType {
			get { return run.FloatingObjectProperties.HorizontalPositionType; }
		}
		public FloatingObjectVerticalPositionType VerticalPositionType {
			get { return run.FloatingObjectProperties.VerticalPositionType; }
		}
		public Point Offset {
			get { return run.FloatingObjectProperties.Offset; }
		}
		public Point PercentOffset {
			get { return run.FloatingObjectProperties.PercentOffset; }
		}
		#endregion
	}
	public class FloatingObjectDrawingObjectContent : IOpenXMLDrawingObjectContent {
		PictureFloatingObjectContent content;
		public FloatingObjectDrawingObjectContent(PictureFloatingObjectContent content) {
			this.content = content;
		}
		public OfficeImage Image {
			get { return content.Image; }
		}
	}
	#region FloatingObjectContent (abstract class)
	public abstract class FloatingObjectContent : IDisposable {
		readonly TextRunBase run;
		static readonly FloatingObjectContent empty = new EmptyFloatingObjectContent();
		public static FloatingObjectContent Empty { get { return empty; } }
		protected FloatingObjectContent(TextRunBase run) {
			this.run = run;
		}
		public TextRunBase Run { get { return run; } }
		public Paragraph Paragraph { get { return run.Paragraph; } }
		public DocumentModel DocumentModel { get { return run.DocumentModel; } }
		public PieceTable RunPieceTable { get { return run.PieceTable; } }
		public abstract Size OriginalSize { get; }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal abstract void SetOriginalSize(Size value);
		public abstract FloatingObjectContent Clone(FloatingObjectAnchorRun run, DocumentModelCopyManager copyManager);
	}
	#endregion
	#region EmptyFloatingObjectContent
	public class EmptyFloatingObjectContent : FloatingObjectContent {
		public EmptyFloatingObjectContent()
			: base(null) {
		}
		public override Size OriginalSize { get { return Size.Empty; } }
		protected internal override void SetOriginalSize(Size value) {
		}
		public override FloatingObjectContent Clone(FloatingObjectAnchorRun run, DocumentModelCopyManager copyManager) {
			return new EmptyFloatingObjectContent();
		}
	}
	#endregion
	#region PictureFloatingObjectContent
	public class PictureFloatingObjectContent : FloatingObjectContent {
		OfficeImage image;
		Size originalSize;
		bool isScreenDpiUsed;
		readonly IPictureContainerRun pictureContainerRun;
		public PictureFloatingObjectContent(IPictureContainerRun run, OfficeImage image)
			: base(run.Run) {
			Guard.ArgumentNotNull(image, "image");
			this.image = image;
			this.pictureContainerRun = run;
			Size imageSizeInModelUnits = InternalOfficeImageHelper.CalculateImageSizeInModelUnits(image, Paragraph.DocumentModel.UnitConverter);
			SetOriginalSize(imageSizeInModelUnits);
			SubscribePicturePropertiesEvents();
		}
		public OfficeImage Image { get { return image; } }
		public override Size OriginalSize { get { return originalSize; } }
		public bool InvalidActualSize { get; set; }
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (image != null) {
					UnsubscribePicturePropertiesEvents();
					image.Dispose();
					image = null;
				}
			}
		}
		#endregion
		protected internal override void SetOriginalSize(Size value) {
			if (value.Width <= 0 || value.Height <= 0)
				Exceptions.ThrowArgumentException("OriginalSize", value);
			this.originalSize = value;
		}
		protected internal virtual void SubscribePicturePropertiesEvents() {
			Image.NativeImageChanging += OnImageChanging;
			Image.NativeImageChanged += OnImageChanged;
		}
		protected internal virtual void UnsubscribePicturePropertiesEvents() {
			Image.NativeImageChanging -= OnImageChanging;
			Image.NativeImageChanged -= OnImageChanged;
		}
		protected internal virtual void OnImageChanging(object sender, EventArgs e) {
			if (DocumentModel.IsDisposed)
				return;
			DocumentModel.BeginUpdate();
		}
		protected internal virtual void OnImageChanged(object sender, NativeImageChangedEventArgs e) {
			if (DocumentModel.IsDisposed)
				return;
			if (image.NativeImage != null) {
				DocumentHistory history = DocumentModel.ReplaceHistory(new EmptyHistory(DocumentModel));
				try {
					HandleImageChanged(e);
				}
				finally {
					DocumentModel.ReplaceHistory(history);
				}
			}
			DocumentModel.EndUpdate();
		}
		protected internal virtual void HandleImageChanged(NativeImageChangedEventArgs e) {
			RunIndex runIndex = RunPieceTable.CalculateRunIndex(Run, RunIndex.DontCare);
			if (runIndex == RunIndex.DontCare)
				return;
			Size imageSize = InternalOfficeImageHelper.CalculateImageSizeInModelUnits(image, DocumentModel.UnitConverter);
			SetOriginalSize(imageSize);
			SetActualSize(e.DesiredImageSizeInTwips);
			if (image.IsLoaded && image.ShouldSetDesiredSizeAfterLoad)
				pictureContainerRun.ActualSize = image.DesiredSizeAfterLoad;
			RunPieceTable.ApplyChangesCore(DocumentModelChangeActions.PerformActionsOnIdle | pictureContainerRun.GetBatchUpdateChangeActions(), runIndex, runIndex);
		}
		Size CalculateActualSize(Size desiredSize) {
			if (desiredSize.Width <= 0 && desiredSize.Height <= 0)
				return ApplyScales(GetImageSizeWithActualDpi());
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			Size actualSize = new Size();
			actualSize.Width = unitConverter.TwipsToModelUnits(desiredSize.Width);
			actualSize.Height = unitConverter.TwipsToModelUnits(desiredSize.Height);
			float aspect = image.SizeInOriginalUnits.Width / ((float)(image.SizeInOriginalUnits.Height));
			if (actualSize.Width <= 0)
				actualSize.Width = (int)Math.Round(aspect * actualSize.Height);
			if (actualSize.Height <= 0)
				actualSize.Height = (int)Math.Round(actualSize.Width / aspect);
			return actualSize;
		}
		Size GetImageSizeWithActualDpi() {
			if (this.isScreenDpiUsed)
				return GetImageSizeWithDocumentModelDpi();
			else
				return InternalOfficeImageHelper.CalculateImageSizeInModelUnits(Image, DocumentModel.UnitConverter);
		}
		internal void EnsureActualSize(bool useScreenDpi) {
			this.isScreenDpiUsed = useScreenDpi;
			if (!Image.IsLoaded)
				return;
			IDesiredSizeSupport desiredSizeSupport = Image as IDesiredSizeSupport;
			Size desiredSize = desiredSizeSupport != null ? desiredSizeSupport.DesiredSize : Size.Empty;
			SetActualSize(desiredSize);
		}
		void SetActualSize(Size desiredSize) {
			Size actualSize = CalculateActualSize(desiredSize);
			if (actualSize != this.pictureContainerRun.ActualSize)
				this.pictureContainerRun.SetActualSizeInternal(actualSize);
		}
		Size ApplyScales(Size imageSize) {
			float scaleX = this.pictureContainerRun.ScaleX;
			float scaleY = this.pictureContainerRun.ScaleY;
			if (scaleX > 0.0f)
				imageSize.Width = Math.Max(1, (int)Math.Round(scaleX * imageSize.Width / 100.0f));
			if (scaleY > 0.0f)
				imageSize.Height = Math.Max(1, (int)Math.Round(scaleY * imageSize.Height / 100.0f));
			return imageSize;
		}
		protected internal virtual Size GetImageSizeWithDocumentModelDpi() {
			int widthInPixels = image.SizeInPixels.Width;
			int heightInPixels = image.SizeInPixels.Height;
			int resultWidth = Units.PixelsToTwips(widthInPixels, DocumentModel.ScreenDpiX);
			int resultHeight = Units.PixelsToTwips(heightInPixels, DocumentModel.ScreenDpiY);
			return new Size(resultWidth, resultHeight);
		}
		protected internal virtual void BeforeRunRemoved() {
			UnsubscribePicturePropertiesEvents();
		}
		protected internal virtual void AfterRunInserted() {
			UnsubscribePicturePropertiesEvents();
			SubscribePicturePropertiesEvents();
		}
		public override FloatingObjectContent Clone(FloatingObjectAnchorRun run, DocumentModelCopyManager copyManager) {
			return new PictureFloatingObjectContent(run, Image.Clone(run.PieceTable.DocumentModel));
		}
	}
	#endregion
	public interface IZOrderedObject {
		int ZOrder { get; set; }
		bool IsBehindDoc { get; set; }
	}
	public class ZOrderManager {
		public virtual void BringToFront(IList<IZOrderedObject> objects, int objectIndex) {
			int maxIndex = objects.Count - 1;
			if ((objectIndex >= maxIndex) || (objectIndex < 0))
				return;
			objects[objectIndex].ZOrder = objects[maxIndex].ZOrder + 1;
		}
		public virtual void BringForward(IList<IZOrderedObject> objects, int objectIndex) {
			int maxIndex = objects.Count - 1;
			if ((objectIndex >= maxIndex) || (objectIndex < 0))
				return;
			if (objectIndex < maxIndex) {
				if ((objectIndex + 2 <= maxIndex) && ((objects[objectIndex + 2].ZOrder - objects[objectIndex + 1].ZOrder == 1)))
					Swap(objects, objectIndex, objectIndex + 1);
				objects[objectIndex].ZOrder = objects[objectIndex + 1].ZOrder + 1;
			}
		}
		void Swap(IList<IZOrderedObject> objects, int objectIndex, int secondIndex) {
			int zOrder = objects[objectIndex].ZOrder;
			objects[objectIndex].ZOrder = objects[secondIndex].ZOrder;
			objects[secondIndex].ZOrder = zOrder;
		}
		void IncrementZOrders(IList<IZOrderedObject> floatingObjectList, int from, int to) {
			for (int i = from + 1; i < to; i++) {
				if (floatingObjectList[i].ZOrder - floatingObjectList[i - 1].ZOrder >= 1)
					break;
				floatingObjectList[i].ZOrder++;
			}
		}
		public virtual void SendBackward(IList<IZOrderedObject> objects, int objectIndex) {
			if ((objectIndex <= 0) || (objectIndex > objects.Count - 1))
				return;
			if ((objectIndex - 2 >= 0) && ((Math.Abs(objects[objectIndex - 1].ZOrder - objects[objectIndex - 2].ZOrder) == 1)))
				Swap(objects, objectIndex, objectIndex - 1);
			objects[objectIndex].ZOrder = objects[objectIndex - 1].ZOrder - 1;
		}
		public virtual void SendToBack(IList<IZOrderedObject> objects, int objectIndex) {
			int minZorder = objects[0].ZOrder;
			if ((objectIndex < 0) || (objectIndex > objects.Count - 1))
				return;
			if (objectIndex == 0)
				IncrementZOrders(objects, 0, objects.Count - 1);
			else if (objects[0].ZOrder >= 1)
				objects[objectIndex].ZOrder = minZorder - 1;
			else {
				objects[objectIndex].ZOrder = 0;
				objects[0].ZOrder++;
				IncrementZOrders(objects, 0, objectIndex);
			}
		}
		public void Normalize(IList<IZOrderedObject> objects) {
			List<IZOrderedObject> front = new List<IZOrderedObject>(objects.Count);
			List<IZOrderedObject> back = new List<IZOrderedObject>(objects.Count);
			List<IZOrderedObject> backmost = new List<IZOrderedObject>(objects.Count);
			foreach (IZOrderedObject obj in objects) {
				if (obj.IsBehindDoc)
					backmost.Add(obj);
				else if (obj.ZOrder >= 0)
					front.Add(obj);
				else
					back.Add(obj);
			}
			Comparison<IZOrderedObject> comparison = (x, y) => Math.Sign(x.ZOrder - y.ZOrder);
			front.Sort(comparison);
			back.Sort(comparison);
			backmost.Sort(comparison);
			backmost.AddRange(back);
			int index = 0;
			IZOrderedObject prev = null;
			foreach (IZOrderedObject obj in front) {
				if (prev == null) {
					index = (obj.ZOrder == 0) ? 0 : 1;
				}
				else if (obj.ZOrder != prev.ZOrder)
					index++;
				obj.ZOrder = index;
				prev = obj;
			}
			prev = null;
			foreach (IZOrderedObject obj in backmost) {
				if (prev == null)
					index = 1;
				else if (obj.ZOrder != prev.ZOrder)
					index++;
				obj.IsBehindDoc = true;
				obj.ZOrder = index;
				prev = obj;
			}
		}
	}
	#region FloatingObjectImportInfo
	public class FloatingObjectImportInfo : IFloatingObjectPropertiesContainer, IShapeContainer, ITextBoxPropertiesContainer {
		#region Fields
		readonly FloatingObjectProperties floatingObjectProperties;
		readonly Shape shape;
		readonly TextBoxProperties textBoxProperties;
		readonly PieceTable pieceTable;
		TextBoxContentType textBoxContent;
		int width = Int32.MinValue;
		int height = Int32.MinValue;
		OfficeImage image;
		string name = String.Empty;
		bool isFloatingObject;
		bool isTextBox;
		bool shouldIgnore;
		#endregion
		public FloatingObjectImportInfo(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.floatingObjectProperties = new FloatingObjectProperties(this);
			this.shape = new Shape(this);
			this.textBoxProperties = new TextBoxProperties(this);
			floatingObjectProperties.BeginUpdate(); 
			shape.BeginUpdate(); 
			textBoxProperties.BeginUpdate(); 
		}
		#region Properties
		public FloatingObjectProperties FloatingObjectProperties { get { return floatingObjectProperties; }  }
		public TextBoxContentType TextBoxContent { get { return textBoxContent; } set { textBoxContent = value; } }
		public Shape Shape { get { return shape; } }
		public TextBoxProperties TextBoxProperties { get { return textBoxProperties; } }
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }
		public OfficeImage Image { get { return image; } set { image = value; } }
		public bool IsFloatingObject { get { return isFloatingObject && PieceTable.DocumentModel.DocumentCapabilities.FloatingObjectsAllowed; } set { isFloatingObject = value; } }
		public bool IsTextBox { get { return isTextBox; } set { isTextBox = value; } }
		public string Name { get { return name; } set { name = value; } }
		public bool ShouldIgnore { get { return shouldIgnore; } set { shouldIgnore = value; } }
		#endregion
		#region IFloatingObjectPropertiesContainer Members
		PieceTable IFloatingObjectPropertiesContainer.PieceTable { get { return this.PieceTable; } }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IFloatingObjectPropertiesContainer.CreateFloatingObjectPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(PieceTable, FloatingObjectProperties);
		}
		void IFloatingObjectPropertiesContainer.OnFloatingObjectChanged() {
		}
		#endregion
		#region IShapeContainer Members
		PieceTable IShapeContainer.PieceTable { get { return this.PieceTable; } }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IShapeContainer.CreateShapeChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(PieceTable, Shape);
		}
		void IShapeContainer.OnShapeChanged() {
		}
		#endregion
		#region ITextBoxPropertiesContainer Members
		PieceTable ITextBoxPropertiesContainer.PieceTable { get { return this.PieceTable; } }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ITextBoxPropertiesContainer.CreateTextBoxChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(PieceTable, TextBoxProperties);
		}
		void ITextBoxPropertiesContainer.OnTextBoxChanged() {
		}
		#endregion
		public bool InsertFloatingObject(InputPosition pos) {
			bool result = InsertFloatingObjectCore(pos);
			textBoxProperties.CancelUpdate();
			shape.CancelUpdate();
			floatingObjectProperties.CancelUpdate();
			return result;
		}
		bool InsertFloatingObjectCore(InputPosition pos) {
			if (!IsFloatingObject)
				return false;
			if (IsTextBox) {
				if (TextBoxContent == null)
					return false;
				FloatingObjectAnchorRun run = PieceTable.InsertFloatingObjectAnchorCore(pos);
				run.FloatingObjectProperties.CopyFrom(FloatingObjectProperties.Info);
				run.Shape.CopyFrom(Shape.Info);
				TextBoxFloatingObjectContent content = new TextBoxFloatingObjectContent(run, TextBoxContent);
				content.TextBoxProperties.CopyFrom(this.TextBoxProperties.Info);
				run.SetContent(content);
				run.Name = Name;
				return true;
			}
			else {
				if (Image == null)
					return false;
				FloatingObjectAnchorRun run = PieceTable.InsertFloatingObjectAnchorCore(pos);
				run.FloatingObjectProperties.CopyFrom(FloatingObjectProperties.Info);
				run.Shape.CopyFrom(Shape.Info);
				run.SetContent(new PictureFloatingObjectContent(run, Image));
				run.Name = Name;
				return true;
			}
		}
	}
	#endregion
}
