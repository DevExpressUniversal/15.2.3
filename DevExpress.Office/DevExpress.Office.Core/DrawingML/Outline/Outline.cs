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
using DevExpress.Office.DrawingML;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
namespace DevExpress.Office.Drawing {
	#region OutlineStrokeAlignment
	public enum OutlineStrokeAlignment {
		None = 0,
		Center = 1,
		Inset = 2,
	}
	#endregion
	#region OutlineHeadTailSize
	public enum OutlineHeadTailSize {
		Small = 0,
		Medium = 1,
		Large = 2,
	}
	#endregion
	#region OutlineHeadTailType
	public enum OutlineHeadTailType {
		None = 0,
		Arrow = 1,
		Diamond = 2,
		Oval = 3,
		StealthArrow = 4,
		TriangleArrow = 5,
	}
	#endregion
	#region OutlineInfo
	public class OutlineInfo : ICloneable<OutlineInfo>, ISupportsCopyFrom<OutlineInfo>, ISupportsSizeOf {
		readonly static OutlineInfo defaultInfo = new OutlineInfo();
		public static OutlineInfo DefaultInfo { get { return defaultInfo; } }
		public static OutlineHeadTailSize DefaultHeadTailSize { get { return OutlineHeadTailSize.Medium; } }
		public static OutlineHeadTailType DefaultHeadTailType { get { return OutlineHeadTailType.None; } }
		#region Fields
		const uint MaskJoinStyle			= 0x00000003; 
		const uint MaskDashing			  = 0x0000003C; 
		const uint MaskHeadLength		   = 0x000000C0; 
		const uint MaskHeadWidth			= 0x00000300; 
		const uint MaskTailLength		   = 0x00000C00; 
		const uint MaskTailWidth			= 0x00003000; 
		const uint MaskHeadType			 = 0x0001C000; 
		const uint MaskTailType			 = 0x000E0000; 
		const uint MaskStrokeAlignment	  = 0x00300000; 
		const uint MaskEndCapStyle		  = 0x00C00000; 
		const uint MaskCompoundType		 = 0x07000000; 
		const uint MaskHasLineJoinStyle	 = 0x08000000; 
		const uint MaskHasCompoundType	  = 0x10000000; 
		const uint MaskHasDashing		   = 0x20000000; 
		const uint MaskHasWidth			 = 0x40000000; 
		uint packedValues				   = 0x00401542;
		int width;
		int miterLimit = 800000;
		#endregion
		#region Properties
		public LineJoinStyle JoinStyle {
			get { return (LineJoinStyle)GetUIntValue(MaskJoinStyle, 0); }
			set { SetUIntValue(MaskJoinStyle, 0, (uint)value); } 
		}
		public OutlineDashing Dashing {
			get { return (OutlineDashing)GetUIntValue(MaskDashing, 2); }
			set { SetUIntValue(MaskDashing, 2, (uint)value); } 
		}
		public OutlineHeadTailSize HeadLength {
			get { return (OutlineHeadTailSize)GetUIntValue(MaskHeadLength, 6); }
			set { SetUIntValue(MaskHeadLength, 6, (uint)value); } 
		}
		public OutlineHeadTailSize HeadWidth { 
			get { return (OutlineHeadTailSize)GetUIntValue(MaskHeadWidth, 8); } 
			set { SetUIntValue(MaskHeadWidth, 8, (uint)value); } 
		}
		public OutlineHeadTailSize TailLength { 
			get { return (OutlineHeadTailSize)GetUIntValue(MaskTailLength, 10); } 
			set { SetUIntValue(MaskTailLength, 10, (uint)value); } 
		}
		public OutlineHeadTailSize TailWidth { 
			get { return (OutlineHeadTailSize)GetUIntValue(MaskTailWidth, 12); } 
			set { SetUIntValue(MaskTailWidth, 12, (uint)value); } 
		}
		public OutlineHeadTailType HeadType { 
			get { return (OutlineHeadTailType)GetUIntValue(MaskHeadType, 14); } 
			set { SetUIntValue(MaskHeadType, 14, (uint)value); } 
		}
		public OutlineHeadTailType TailType { 
			get { return (OutlineHeadTailType)GetUIntValue(MaskTailType, 17); } 
			set { SetUIntValue(MaskTailType, 17, (uint)value); } 
		}
		public OutlineStrokeAlignment StrokeAlignment { 
			get { return (OutlineStrokeAlignment)GetUIntValue(MaskStrokeAlignment, 20); } 
			set { SetUIntValue(MaskStrokeAlignment, 20, (uint)value); } 
		}
		public OutlineEndCapStyle EndCapStyle { 
			get { return (OutlineEndCapStyle)GetUIntValue(MaskEndCapStyle, 22); } 
			set { SetUIntValue(MaskEndCapStyle, 22, (uint)value); } 
		}
		public OutlineCompoundType CompoundType { 
			get { return (OutlineCompoundType)GetUIntValue(MaskCompoundType, 24); } 
			set { SetUIntValue(MaskCompoundType, 24, (uint)value); } 
		}
		public bool HasLineJoinStyle {
			get { return GetBooleanValue(MaskHasLineJoinStyle); }
			set { SetBooleanValue(MaskHasLineJoinStyle, value); }
		}
		public bool HasCompoundType {
			get { return GetBooleanValue(MaskHasCompoundType); }
			set { SetBooleanValue(MaskHasCompoundType, value); }
		}
		public bool HasDashing {
			get { return GetBooleanValue(MaskHasDashing); }
			set { SetBooleanValue(MaskHasDashing, value); }
		}
		public bool HasWidth {
			get { return GetBooleanValue(MaskHasWidth); }
			set { SetBooleanValue(MaskHasWidth, value); }
		}
		public int Width { get { return width; } set { width = value; } }
		public int MiterLimit { get { return miterLimit; } set { miterLimit = value; } }
		public bool IsDefaultHeadEndStyle {
			get {
				return
					HeadLength == DefaultInfo.HeadLength &&
					HeadType == DefaultInfo.HeadType &&
					HeadWidth == DefaultInfo.HeadWidth;
			}
		}
		public bool IsDefaultTailEndStyle {
			get {
				return
					TailLength == DefaultInfo.TailLength &&
					TailType == DefaultInfo.TailType &&
					TailWidth == DefaultInfo.TailWidth;
			}
		}
		#endregion
		#region Get/Set Value helpers
		void SetUIntValue(uint mask, int bits, uint value) {
			packedValues &= ~mask;
			packedValues |= (value << bits) & mask;
		}
		uint GetUIntValue(uint mask, int bits){
			return (packedValues & mask) >> bits;
		}
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		public override bool Equals(object obj) {
			OutlineInfo info = obj as OutlineInfo;
			if (info == null)
				return false;
			return
				this.packedValues == info.packedValues &&
				this.width == info.width &&
				this.miterLimit == info.miterLimit;
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode() ^ width ^ miterLimit;
		}
		#region ICloneable<OutlineInfo> Members
		public OutlineInfo Clone() {
			OutlineInfo result = new OutlineInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<BorderInfo> Members
		public void CopyFrom(OutlineInfo value) {
			this.packedValues = value.packedValues;
			this.width = value.width;
			this.miterLimit = value.miterLimit;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region OutlineInfoCache
	public class OutlineInfoCache : UniqueItemsCache<OutlineInfo> {
		public const int DefaultItemIndex = 0;
		public OutlineInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override OutlineInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new OutlineInfo();
		}
	}
	#endregion
	#region Outline
	public class Outline : DrawingUndoableIndexBasedObject<OutlineInfo>, ICloneable<Outline>, ISupportsCopyFrom<Outline>, IFillOwner, IStrokeUnderline {
		#region Fields
		IDrawingFill fill;
		#endregion
		public Outline(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
			this.fill = DrawingFill.Automatic;
		}
		#region Properties
		#region Fill
		public IDrawingFill Fill {
			get { return fill; }
			set {
				if (value == null)
					value = DrawingFill.Automatic;
				if (!IsValidOutlineFill(value))
					throw new ArgumentException("Wrong outline fill type.");
				if (fill.Equals(value))
					return;
				HistoryItem item = new DrawingFillPropertyChangedHistoryItem(DocumentModel.MainPart, this, fill, value);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		public void SetDrawingFillCore(IDrawingFill value) {
			RaiseSetFill(value);
			fill.Parent = null;
			fill = value;
			fill.Parent = InnerParent;
			InvalidateParent();
		}
		bool IsValidOutlineFill(IDrawingFill value) {
			return value.FillType == DrawingFillType.Automatic ||
				value.FillType == DrawingFillType.Gradient ||
				value.FillType == DrawingFillType.None ||
				value.FillType == DrawingFillType.Pattern ||
				value.FillType == DrawingFillType.Solid;
		}
		#endregion
		public bool HasLineJoinStyle { get { return Info.HasLineJoinStyle; } }
		#region JoinStyle
		public LineJoinStyle JoinStyle {
			get { return Info.JoinStyle; }
			set {
				if (JoinStyle == value && HasLineJoinStyle)
					return;
				SetPropertyValue(SetJoinStyleCore, value);
			}
		}
		DocumentModelChangeActions SetJoinStyleCore(OutlineInfo info, LineJoinStyle value) {
			info.JoinStyle = value;
			info.HasLineJoinStyle = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool HasDashing { get { return Info.HasDashing; } }
		#region Dashing
		public OutlineDashing Dashing {
			get { return Info.Dashing; }
			set {
				if (Dashing == value && HasDashing)
					return;
				SetPropertyValue(SetOutlineDashingCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineDashingCore(OutlineInfo info, OutlineDashing value) {
			info.Dashing = value;
			info.HasDashing = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HeadLength
		public OutlineHeadTailSize HeadLength {
			get { return Info.HeadLength; }
			set {
				if (HeadLength == value)
					return;
				SetPropertyValue(SetHeadLengthCore, value);
			}
		}
		DocumentModelChangeActions SetHeadLengthCore(OutlineInfo info, OutlineHeadTailSize value) {
			info.HeadLength = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HeadWidth
		public OutlineHeadTailSize HeadWidth {
			get { return Info.HeadWidth; }
			set {
				if (HeadWidth == value)
					return;
				SetPropertyValue(SetHeadWidthCore, value);
			}
		}
		DocumentModelChangeActions SetHeadWidthCore(OutlineInfo info, OutlineHeadTailSize value) {
			info.HeadWidth = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HeadType
		public OutlineHeadTailType HeadType {
			get { return Info.HeadType; }
			set {
				if (HeadType == value)
					return;
				SetPropertyValue(SetHeadTypeCore, value);
			}
		}
		DocumentModelChangeActions SetHeadTypeCore(OutlineInfo info, OutlineHeadTailType value) {
			info.HeadType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TailLength
		public OutlineHeadTailSize TailLength {
			get { return Info.TailLength; }
			set {
				if (TailLength == value)
					return;
				SetPropertyValue(SetTailLengthCore, value);
			}
		}
		DocumentModelChangeActions SetTailLengthCore(OutlineInfo info, OutlineHeadTailSize value) {
			info.TailLength = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TailWidth
		public OutlineHeadTailSize TailWidth {
			get { return Info.TailWidth; }
			set {
				if (TailWidth == value)
					return;
				SetPropertyValue(SetTailWidthCore, value);
			}
		}
		DocumentModelChangeActions SetTailWidthCore(OutlineInfo info, OutlineHeadTailSize value) {
			info.TailWidth = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TailType
		public OutlineHeadTailType TailType {
			get { return Info.TailType; }
			set {
				if (TailType == value)
					return;
				SetPropertyValue(SetTailTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTailTypeCore(OutlineInfo info, OutlineHeadTailType value) {
			info.TailType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region StrokeAlignment
		public OutlineStrokeAlignment StrokeAlignment {
			get { return Info.StrokeAlignment; }
			set {
				if (StrokeAlignment == value)
					return;
				SetPropertyValue(SetStrokeAlignmentCore, value);
			}
		}
		DocumentModelChangeActions SetStrokeAlignmentCore(OutlineInfo info, OutlineStrokeAlignment value) {
			info.StrokeAlignment = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EndCapStyle
		public OutlineEndCapStyle EndCapStyle {
			get { return Info.EndCapStyle; }
			set {
				if (EndCapStyle == value)
					return;
				SetPropertyValue(SetEndCapStyleCore, value);
			}
		}
		DocumentModelChangeActions SetEndCapStyleCore(OutlineInfo info, OutlineEndCapStyle value) {
			info.EndCapStyle = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool HasCompoundType { get { return Info.HasCompoundType; } }
		#region CompoundType
		public OutlineCompoundType CompoundType {
			get { return Info.CompoundType; }
			set {
				if (CompoundType == value && HasCompoundType)
					return;
				SetPropertyValue(SetCompoundTypeCore, value);
			}
		}
		DocumentModelChangeActions SetCompoundTypeCore(OutlineInfo info, OutlineCompoundType value) {
			info.CompoundType = value;
			info.HasCompoundType = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool HasWidth { get { return Info.HasWidth; } }
		#region Width
		public int Width {
			get { return Info.Width; }
			set {
				if (Width == value && HasWidth)
					return;
				SetPropertyValue(SetOutlineWidthCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineWidthCore(OutlineInfo info, int value) {
			info.Width = value;
			info.HasWidth = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MiterLimit
		public int MiterLimit {
			get { return Info.MiterLimit; }
			set {
				if (MiterLimit == value)
					return;
				SetPropertyValue(SetMiterLimitCore, value);
			}
		}
		DocumentModelChangeActions SetMiterLimitCore(OutlineInfo info, int value) {
			info.MiterLimit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		public bool IsDefault { get { return fill == DrawingFill.Automatic && Index == OutlineInfoCache.DefaultItemIndex; } }
		#endregion
		protected internal override UniqueItemsCache<OutlineInfo> GetCache(IDocumentModel documentModel) {
			return DrawingCache.OutlineInfoCache;
		}
		#region IClonable<Outline> Members
		public Outline Clone() {
			Outline result = new Outline(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<Outline> Members
		public void CopyFrom(Outline value) {
			base.CopyFrom(value);
			this.Fill = value.Fill.CloneTo(DocumentModel);
		}
		#endregion
		public Outline CloneTo(IDocumentModel documentModel) {
			Outline result = new Outline(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public override bool Equals(object obj) {
			Outline other = obj as Outline;
			if (other == null)
				return false;
			return Info.Equals(other.Info) && this.fill.Equals(other.fill);
		}
		public override int GetHashCode() {
			return Info.GetHashCode() ^ fill.GetHashCode();
		}
		#region IStrokeUnderline Members
		DrawingStrokeUnderlineType IStrokeUnderline.Type { get { return DrawingStrokeUnderlineType.Outline; } }
		IStrokeUnderline IStrokeUnderline.CloneTo(IDocumentModel documentModel) {
			Outline result = new Outline(documentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public void ResetToStyle() {
			if (IsUpdateLocked)
				Info.CopyFrom(DrawingCache.OutlineInfoCache.DefaultItem);
			else
				ChangeIndexCore(OutlineInfoCache.DefaultItemIndex, DocumentModelChangeActions.None);
			Fill = DrawingFill.Automatic;
		}
		#region SetFill
		SetFillEventHandler onSetFill;
		public event SetFillEventHandler SetFill { add { onSetFill += value; } remove { onSetFill -= value; } }
		protected internal void RaiseSetFill(IDrawingFill value) {
			if (onSetFill != null) {
				SetFillEventArgs args = new SetFillEventArgs(value);
				onSetFill(this, args);
			}
		}
		#endregion
	}
	#endregion
}
