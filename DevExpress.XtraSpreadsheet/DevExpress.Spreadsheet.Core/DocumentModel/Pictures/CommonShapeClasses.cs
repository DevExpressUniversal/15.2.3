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
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region BlackWhiteMode
	public enum OpenXmlBlackWhiteMode { 
		Empty,
		Auto,
		Black,
		BlackGray,
		BlackWhite,
		Clr,
		Gray,
		GrayWhite,
		Hidden,
		InvGray,
		LtGray,
		White
	}
	#endregion
	#region OutlineType
	public enum OutlineType {
		None,
		Solid,
		Pattern,
		Gradient
	}
	#endregion
	#region ShapeStyle
	public class ShapeStyleInfo : ICloneable<ShapeStyleInfo>, ISupportsCopyFrom<ShapeStyleInfo>, ISupportsSizeOf {
		#region Fields
		int effectReferenceIdx;
		int fillReferenceIdx;
		byte fontReferenceIdx;
		int lineReferenceIdx;
		#endregion
		#region Properties
		public int EffectReferenceIdx { get { return effectReferenceIdx; } set { effectReferenceIdx = value; } }
		public int FillReferenceIdx { get { return fillReferenceIdx; } set { fillReferenceIdx = value; } }
		public XlFontSchemeStyles FontReferenceIdx { get { return (XlFontSchemeStyles)fontReferenceIdx; } set { fontReferenceIdx = (byte)value; } }
		public int LineReferenceIdx { get { return lineReferenceIdx; } set { lineReferenceIdx = value; } }
		#endregion
		#region ICloneable<ShapeStyleInfo> Members
		public ShapeStyleInfo Clone() {
			ShapeStyleInfo result = new ShapeStyleInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ShapeStyleInfo> Members
		public void CopyFrom(ShapeStyleInfo value) {
			Guard.ArgumentNotNull(value, "ShapeStyleInfo");
			this.effectReferenceIdx = value.effectReferenceIdx;
			this.fillReferenceIdx = value.fillReferenceIdx;
			this.fontReferenceIdx = value.fontReferenceIdx;
			this.lineReferenceIdx = value.lineReferenceIdx;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			ShapeStyleInfo info = obj as ShapeStyleInfo;
			if (info == null)
				return false;
			return this.effectReferenceIdx == info.effectReferenceIdx
				&& this.fillReferenceIdx == info.fillReferenceIdx
				&& this.fontReferenceIdx == info.fontReferenceIdx
				&& this.lineReferenceIdx == info.lineReferenceIdx;
		}
		public override int GetHashCode() {
			return (int) (effectReferenceIdx ^ fillReferenceIdx ^ (int)fontReferenceIdx ^ lineReferenceIdx);
		}
	}
	public class ShapeStyleInfoCache : UniqueItemsCache<ShapeStyleInfo> {
		public ShapeStyleInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ShapeStyleInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ShapeStyleInfo info = new ShapeStyleInfo();
			return info;
		}
	}
	public class ShapeStyle : SpreadsheetUndoableIndexBasedObject<ShapeStyleInfo> {
		public ShapeStyle(DocumentModel documentModel)
			: base(documentModel) {
			EffectColor = new DrawingColor(documentModel);
			LineColor = new DrawingColor(documentModel);
			FillColor = new DrawingColor(documentModel);
			FontColor = new DrawingColor(documentModel);
		}
		#region EffectReferenceIdx
		public int EffectReferenceIdx {
			get { return Info.EffectReferenceIdx; }
			set {
				if (EffectReferenceIdx == value)
					return;
				SetPropertyValue(SetEffectReferenceIdx, value);
			}
		}
		DocumentModelChangeActions SetEffectReferenceIdx(ShapeStyleInfo info, int value) {
			info.EffectReferenceIdx = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FillReferenceIdx
		public int FillReferenceIdx {
			get { return Info.FillReferenceIdx; }
			set {
				if (FillReferenceIdx == value)
					return;
				SetPropertyValue(SetFillReferenceIdx, value);
			}
		}
		DocumentModelChangeActions SetFillReferenceIdx(ShapeStyleInfo info, int value) {
			info.FillReferenceIdx = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FontReferenceIdx
		public XlFontSchemeStyles FontReferenceIdx {
			get { return Info.FontReferenceIdx; }
			set {
				if (FontReferenceIdx == value)
					return;
				SetPropertyValue(SetFontReferenceIdx, value);
			}
		}
		DocumentModelChangeActions SetFontReferenceIdx(ShapeStyleInfo info, XlFontSchemeStyles value) {
			info.FontReferenceIdx = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LineReferenceIdx
		public int LineReferenceIdx {
			get { return Info.LineReferenceIdx; }
			set {
				if (LineReferenceIdx == value)
					return;
				SetPropertyValue(SetLineReferenceIdx, value);
			}
		}
		DocumentModelChangeActions SetLineReferenceIdx(ShapeStyleInfo info, int value) {
			info.LineReferenceIdx = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EffectColor
		public DrawingColor EffectColor { get; set; }
		#endregion
		#region LineColor
		public DrawingColor LineColor { get; set; }
		#endregion
		#region FillColor
		public DrawingColor FillColor { get; set; }
		#endregion
		#region FontColor
		public DrawingColor FontColor { get; set; }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ShapeStyleInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ShapeStyleInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
		#endregion
	}
	#endregion
	#region Xfrm
	public class Transform2DInfo : ICloneable<Transform2DInfo>, ISupportsCopyFrom<Transform2DInfo>, ISupportsSizeOf {
		#region Fields
		int rotation;
		bool flipH;
		bool flipV;
		int offsetX;
		int offsetY;
		int cx;
		int cy;
		#endregion
		#region Properties
		public int Rotation { get { return rotation; } set { rotation = value; } }
		public bool FlipH { get { return flipH; } set { flipH = value; } }
		public bool FlipV { get { return flipV; } set { flipV = value; } }
		public int OffsetX { get { return offsetX; } set { offsetX = value; } }
		public int OffsetY { get { return offsetY; } set { offsetY = value; } }
		public int Cx { get { return cx; } set { cx = value; } }
		public int Cy { get { return cy; } set { cy = value; } }
		#endregion
		#region ICloneable<Transform2DInfo> Members
		public Transform2DInfo Clone() {
			Transform2DInfo result = new Transform2DInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<Transform2DInfo> Members
		public void CopyFrom(Transform2DInfo value) {
			Guard.ArgumentNotNull(value, "Transform2DInfo");
			this.rotation = value.rotation;
			this.flipH = value.flipH;
			this.flipV = value.flipV;
			this.offsetX = value.offsetX;
			this.offsetY = value.offsetY;
			this.cx = value.cx;
			this.cy = value.cy;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public bool IsEmpty { get { return Rotation == 0 && FlipH == false && FlipV == false && OffsetX == 0 && OffsetY == 0 && Cx == 0 && Cy == 0; } }
		public override bool Equals(object obj) {
			Transform2DInfo info = obj as Transform2DInfo;
			if (info == null)
				return false;
			return this.rotation == info.rotation
				&& this.flipH == info.flipH
				&& this.flipV == info.flipV
				&& this.offsetX == info.offsetX
				&& this.offsetY == info.offsetY
				&& this.cx == info.cx
				&& this.cy == info.cy;
		}
		public override int GetHashCode() {
			return (int)(rotation ^ offsetX ^ offsetY ^ cx ^ cy ^ flipH.GetHashCode() ^ flipV.GetHashCode());
		}
	}
	public class Transform2DInfoCache : UniqueItemsCache<Transform2DInfo> {
		public Transform2DInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override Transform2DInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			Transform2DInfo info = new Transform2DInfo();
			return info;
		}
	}
	public class Transform2D : SpreadsheetUndoableIndexBasedObject<Transform2DInfo> {
		public Transform2D(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		#region Rotation
		public int Rotation {
			get { return Info.Rotation; }
			set {
				if(Rotation == value)
					return;
				SetPropertyValue(SetRotationCore, value);
			}
		}
		DocumentModelChangeActions SetRotationCore(Transform2DInfo info, int value) {
			info.Rotation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FlipH
		public bool FlipH {
			get { return Info.FlipH; }
			set {
				if(FlipH == value)
					return;
				SetPropertyValue(SetFlipHCore, value);
			}
		}
		DocumentModelChangeActions SetFlipHCore(Transform2DInfo info, bool value) {
			info.FlipH = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FlipV
		public bool FlipV {
			get { return Info.FlipV; }
			set {
				if(FlipV == value)
					return;
				SetPropertyValue(SetFlipVCore, value);
			}
		}
		DocumentModelChangeActions SetFlipVCore(Transform2DInfo info, bool value) {
			info.FlipV = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OffsetX
		public int OffsetX {
			get { return Info.OffsetX; }
			set {
				if(OffsetX == value)
					return;
				SetPropertyValue(SetOffsetXCore, value);
			}
		}
		DocumentModelChangeActions SetOffsetXCore(Transform2DInfo info, int value) {
			info.OffsetX = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OffsetY
		public int OffsetY {
			get { return Info.OffsetY; }
			set {
				if(OffsetY == value)
					return;
				SetPropertyValue(SetOffsetYCore, value);
			}
		}
		DocumentModelChangeActions SetOffsetYCore(Transform2DInfo info, int value) {
			info.OffsetY = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Cx
		public int Cx {
			get { return Info.Cx; }
			set {
				if(Cx == value)
					return;
				SetPropertyValue(SetCxCore, value);
			}
		}
		DocumentModelChangeActions SetCxCore(Transform2DInfo info, int value) {
			info.Cx = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Cy
		public int Cy {
			get { return Info.Cy; }
			set {
				if(Cy == value)
					return;
				SetPropertyValue(SetCyCore, value);
			}
		}
		DocumentModelChangeActions SetCyCore(Transform2DInfo info, int value) {
			info.Cy = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool IsEmpty { get { return Info.IsEmpty; } }
		#endregion
		public float GetRotationAngleInDegrees() {
			return DocumentModel.UnitConverter.ModelUnitsToDegreeF(Rotation);
		}
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<Transform2DInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.Transform2DInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
		#endregion
	}
	public class GroupTransform2D : ISupportsCopyFrom<GroupTransform2D> {
		#region Fields
		readonly Transform2D mainTransform;
		readonly Transform2D childTransform;
		#endregion
		#region Properties
		public Transform2D MainTransform { get { return mainTransform; } }
		public Transform2D ChildTransform { get { return childTransform; } }
		public bool IsEmpty { get { return MainTransform.IsEmpty && ChildTransform.IsEmpty; } }
		#endregion
		public GroupTransform2D(DocumentModel documentModel) {
			mainTransform = new Transform2D(documentModel);
			childTransform = new Transform2D(documentModel);
		}
		#region ISupportsCopyFrom<GroupTransform2D> Members
		public void CopyFrom(GroupTransform2D value) {
			Guard.ArgumentNotNull(value, "GroupTransform2D");
			MainTransform.CopyFrom(value.MainTransform);
			ChildTransform.CopyFrom(value.ChildTransform);
		}
		#endregion
	}
	#endregion
	#region ShapeType
	public enum ShapeType { 
		None,
		Line,
		LineInv,
		Triangle,
		RtTriangle,
		Rect,
		Diamond,
		Parallelogram,
		Trapezoid,
		NonIsoscelesTrapezoid,
		Pentagon,
		Hexagon,
		Heptagon,
		Octagon,
		Decagon,
		Dodecagon,
		Star4,
		Star5,
		Star6,
		Star7,
		Star8,
		Star10,
		Star12,
		Star16,
		Star24,
		Star32,
		RoundRect,
		Round1Rect,
		Round2SameRect,
		Round2DiagRect,
		SnipRoundRect,
		Snip1Rect,
		Snip2SameRect,
		Snip2DiagRect,
		Plaque,
		Ellipse,
		Teardrop,
		HomePlate,
		Chevron,
		PieWedge,
		Pie,
		BlockArc,
		Donut,
		NoSmoking,
		RightArrow,
		LeftArrow,
		UpArrow,
		DownArrow,
		StripedRightArrow,
		NotchedRightArrow,
		BentUpArrow,
		LeftRightArrow,
		UpDownArrow,
		LeftUpArrow,
		LeftRightUpArrow,
		QuadArrow,
		LeftArrowCallout,
		RightArrowCallout,
		UpArrowCallout,
		DownArrowCallout,
		LeftRightArrowCallout,
		UpDownArrowCallout,
		QuadArrowCallout,
		BentArrow,
		UturnArrow,
		CircularArrow,
		LeftCircularArrow,
		LeftRightCircularArrow,
		CurvedRightArrow,
		CurvedLeftArrow,
		CurvedUpArrow,
		CurvedDownArrow,
		SwooshArrow,
		Cube,
		Can,
		LightningBolt,
		Heart,
		Sun,
		Moon,
		SmileyFace,
		IrregularSeal1,
		IrregularSeal2,
		FoldedCorner,
		Bevel,
		Frame,
		HalfFrame,
		Corner,
		DiagStripe,
		Chord,
		Arc,
		LeftBracket,
		RightBracket,
		LeftBrace,
		RightBrace,
		BracketPair,
		BracePair,
		StraightConnector1,
		BentConnector2,
		BentConnector3,
		BentConnector4,
		BentConnector5,
		CurvedConnector2,
		CurvedConnector3,
		CurvedConnector4,
		CurvedConnector5,
		Callout1,
		Callout2,
		Callout3,
		AccentCallout1,
		AccentCallout2,
		AccentCallout3,
		BorderCallout1,
		BorderCallout2,
		BorderCallout3,
		AccentBorderCallout1,
		AccentBorderCallout2,
		AccentBorderCallout3,
		WedgeRectCallout,
		WedgeRoundRectCallout,
		WedgeEllipseCallout,
		CloudCallout,
		Cloud,
		Ribbon,
		Ribbon2,
		EllipseRibbon,
		EllipseRibbon2,
		LeftRightRibbon,
		VerticalScroll,
		HorizontalScroll,
		Wave,
		DoubleWave,
		Plus,
		FlowChartProcess,
		FlowChartDecision,
		FlowChartInputOutput,
		FlowChartPredefinedProcess,
		FlowChartInternalStorage,
		FlowChartDocument,
		FlowChartMultidocument,
		FlowChartTerminator,
		FlowChartPreparation,
		FlowChartManualInput,
		FlowChartManualOperation,
		FlowChartConnector,
		FlowChartPunchedCard,
		FlowChartPunchedTape,
		FlowChartSummingJunction,
		FlowChartOr,
		FlowChartCollate,
		FlowChartSort,
		FlowChartExtract,
		FlowChartMerge,
		FlowChartOfflineStorage,
		FlowChartOnlineStorage,
		FlowChartMagneticTape,
		FlowChartMagneticDisk,
		FlowChartMagneticDrum,
		FlowChartDisplay,
		FlowChartDelay,
		FlowChartAlternateProcess,
		FlowChartOffpageConnector,
		ActionButtonBlank,
		ActionButtonHome,
		ActionButtonHelp,
		ActionButtonInformation,
		ActionButtonForwardNext,
		ActionButtonBackPrevious,
		ActionButtonEnd,
		ActionButtonBeginning,
		ActionButtonReturn,
		ActionButtonDocument,
		ActionButtonSound,
		ActionButtonMovie,
		Gear6,
		Gear9,
		Funnel,
		MathPlus,
		MathMinus,
		MathMultiply,
		MathDivide,
		MathEqual,
		MathNotEqual,
		CornerTabs,
		SquareTabs,
		PlaqueTabs,
		ChartX,
		ChartStar,
		ChartPlus,
	}
	#endregion
}
