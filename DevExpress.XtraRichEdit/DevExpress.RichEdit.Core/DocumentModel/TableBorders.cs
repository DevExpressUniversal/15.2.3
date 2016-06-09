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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region BorderInfo
	[HashtableConverter("DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter", true)]
	public class BorderInfo : ICloneable<BorderInfo>, ISupportsCopyFrom<BorderInfo>, ISupportsSizeOf {
		static readonly BorderInfo empty;
		public static BorderInfo Empty { get { return empty; } }
		static BorderInfo() {
			empty = new BorderInfo();
			empty.Color = DXColor.Empty;
			empty.Style = BorderLineStyle.None;
			empty.Width = 0;
			empty.Frame = false;
			empty.Offset = 0;
			empty.Shadow = false;
#if THEMES_EDIT
			empty.ColorModelInfo = ColorModelInfo.Create(DXColor.Empty);
#endif
		}
		#region Fields
		BorderLineStyle style;
		Color color;
		int width;
		int offset;
		bool frame;
		bool shadow;
#if THEMES_EDIT
		ColorModelInfo colorModelInfo = ColorModelInfo.Create(DXColor.Empty);
#endif
		#endregion
		#region Properties
		[JSONEnum((int)JSONBorderBaseProperty.Style)]
		public BorderLineStyle Style { get { return style; } set { style = value; } }
		[JSONEnum((int)JSONBorderBaseProperty.Color)]
		public Color Color { get { return color; } set { color = value; } }
		[JSONEnum((int)JSONBorderBaseProperty.Width)]
		public int Width { get { return width; } set { width = value; } }
		[JSONEnum((int)JSONBorderBaseProperty.Offset)]
		public int Offset { get { return offset; } set { offset = value; } }
		[JSONEnum((int)JSONBorderBaseProperty.Frame)]
		public bool Frame { get { return frame; } set { frame = value; } }
		[JSONEnum((int)JSONBorderBaseProperty.Shadow)]
		public bool Shadow { get { return shadow; } set { shadow = value; } }
#if THEMES_EDIT
		public ColorModelInfo ColorModelInfo { get { return colorModelInfo; } set { colorModelInfo = value; } }
#endif
		#endregion
		public override bool Equals(object obj) {
			BorderInfo info = obj as BorderInfo;
			if (info == null)
				return false;
			return Style == info.Style &&
				   Color == info.Color &&
				   Width == info.Width &&
				   Frame == info.Frame &&
				   Shadow == info.Shadow &&
#if THEMES_EDIT
				   ColorModelInfo.Equals(info.ColorModelInfo) &&
#endif
				   Offset == info.Offset;
		}
		public static bool operator ==(BorderInfo info1, BorderInfo info2) {
			if (Object.ReferenceEquals(info1, info2))
				return true;
			if (Object.ReferenceEquals(info1, null))
				return false;
			return info1.Equals(info2);
		}
		public static bool operator !=(BorderInfo info1, BorderInfo info2) {
			return !(info1 == info2);
		}
		public override int GetHashCode() {
			return (((int)Style << 20) | (Width << 15) | (Offset << 10) | (frame ? 1 : 0 << 5) | (shadow ? 1 : 0 << 4)) ^ Color.GetHashCode()
#if THEMES_EDIT
				^ ColorModelInfo.GetHashCode()
#endif
				;
		}
		public void CopyFrom(BorderInfo info) {
			Style = info.Style;
			Color = info.Color;
			Width = info.Width;
			Frame = info.Frame;
			Shadow = info.Shadow;
			Offset = info.Offset;
#if THEMES_EDIT
			ColorModelInfo = info.ColorModelInfo;
#endif
		}
		public BorderInfo Clone() {
			BorderInfo border = new BorderInfo();
			border.CopyFrom(this);
			return border;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region BorderInfoCache
	public class BorderInfoCache : UniqueItemsCache<BorderInfo> {
		public BorderInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override BorderInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			BorderInfo result = new BorderInfo();
#if THEMES_EDIT
			result.ColorModelInfo = ColorModelInfo.Create(DXColor.Black);
#endif            
			result.Style = BorderLineStyle.Nil;
			result.Color = DXColor.Black;
			result.Width = 0; 
			return result;
		}
	}
	#endregion
	#region BorderBase
	[HashtableConverter("BorderBaseExporter", true)]
	public abstract class BorderBase : RichEditIndexBasedObject<BorderInfo> {
		readonly IPropertiesContainer owner;
		protected BorderBase(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			this.owner = owner;
		}
		protected BorderBase(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected abstract Properties Property { get; }
		#region Properties
		#region Style
		[JSONEnum((int)JSONBorderBaseProperty.Style)]
		public BorderLineStyle Style {
			get { return Info.Style; }
			set {
				if (value == Info.Style) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetStyleCore, value);
			}
		}
		DocumentModelChangeActions SetStyleCore(BorderInfo info, BorderLineStyle value) {
			info.Style = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Style);
		}
		#endregion
		#region Color
		[JSONEnum((int)JSONBorderBaseProperty.Color)]
		public Color Color {
			get { return Info.Color; }
			set {
				if (value == Info.Color) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetColorCore, value);
			}
		}
		DocumentModelChangeActions SetColorCore(BorderInfo info, Color value) {
			info.Color = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Color);
		}
		#endregion
#if THEMES_EDIT
		#region ColorModelInfo
		public ColorModelInfo ColorModelInfo {
			get { return Info.ColorModelInfo; }
			set {
				if (value == Info.ColorModelInfo) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetColorModelInfoCore, value);
			}
		}
		DocumentModelChangeActions SetColorModelInfoCore(BorderInfo info, ColorModelInfo value) {
			info.ColorModelInfo = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Color);
		}
		#endregion
#endif
		#region Width
		[JSONEnum((int)JSONBorderBaseProperty.Width)]
		public int Width {
			get { return Info.Width; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Width", value);
				if (value == Info.Width) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetWidthCore, value);
			}
		}
		DocumentModelChangeActions SetWidthCore(BorderInfo info, int value) {
			info.Width = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Width);
		}
		#endregion
		#region Offset
		[JSONEnum((int)JSONBorderBaseProperty.Offset)]
		public int Offset {
			get { return Info.Offset; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Offset", value);
				if (value == Info.Offset) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetOffsetCore, value);
			}
		}
		DocumentModelChangeActions SetOffsetCore(BorderInfo info, int value) {
			info.Offset = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Offset);
		}
		#endregion
		#region Frame
		[JSONEnum((int)JSONBorderBaseProperty.Frame)]
		public bool Frame {
			get { return Info.Frame; }
			set {
				if (value == Info.Frame) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetFrameCore, value);
			}
		}
		DocumentModelChangeActions SetFrameCore(BorderInfo info, bool value) {
			info.Frame = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Frame);
		}
		#endregion
		#region Shadow
		[JSONEnum((int)JSONBorderBaseProperty.Shadow)]
		public bool Shadow {
			get { return Info.Shadow; }
			set {
				if (value == Info.Shadow) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetShadowCore, value);
			}
		}
		DocumentModelChangeActions SetShadowCore(BorderInfo info, bool value) {
			info.Shadow = value;
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.Shadow);
		}
		#endregion
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<BorderInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.BorderInfoCache;
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			if (Owner != null)
				Owner.BeginChanging(Property);
		}
		protected override void OnEndAssign() {
			if (Owner != null)
				Owner.EndChanging();
			base.OnEndAssign();
		}
		public void ResetBorder() {
			Owner.ResetPropertyUse(Property);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.BatchUpdate);
		}
		protected internal override void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			if(Owner != null)
				Owner.RaiseObtainAffectedRange(args);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			if (Owner != null)
				Owner.ApplyChanges(changeActions);
			else
				base.ApplyChanges(changeActions);
		}
	}
	public enum JSONBorderBaseProperty {
		Style = 0,
		Color = 1,
		Width = 2,
		Offset = 3,
		Frame = 4,
		Shadow = 5
	}
	#endregion
	#region LeftBorder
	[HashtableConverter("BorderBaseExporter")]
	public class LeftBorder : BorderBase {
		public LeftBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public LeftBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.LeftBorder; } }
	}
	#endregion
	#region RightBorder
	[HashtableConverter("BorderBaseExporter")]
	public class RightBorder : BorderBase {
		public RightBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public RightBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.RightBorder; } }
	}
	#endregion
	#region TopBorder
	[HashtableConverter("BorderBaseExporter")]
	public class TopBorder : BorderBase {
		public TopBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public TopBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.TopBorder; } }
	}
	#endregion
	#region BottomBorder
	[HashtableConverter("BorderBaseExporter")]
	public class BottomBorder : BorderBase {
		public BottomBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public BottomBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.BottomBorder; } }
	}
	#endregion
	#region InsideHorizontalBorder
	[HashtableConverter("BorderBaseExporter")]
	public class InsideHorizontalBorder : BorderBase {
		public InsideHorizontalBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public InsideHorizontalBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.InsideHorizontalBorder; } }
	}
	#endregion
	#region InsideVerticalBorder
	[HashtableConverter("BorderBaseExporter")]
	public class InsideVerticalBorder : BorderBase {
		public InsideVerticalBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public InsideVerticalBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.InsideVerticalBorder; } }
	}
	#endregion
	#region TopLeftDiagonalBorder
	[HashtableConverter("BorderBaseExporter")]
	public class TopLeftDiagonalBorder : BorderBase {
		public TopLeftDiagonalBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public TopLeftDiagonalBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.TopLeftDiagonalBorder; } }
	}
	#endregion
	#region TopRightDiagonalBorder
	[HashtableConverter("BorderBaseExporter")]
	public class TopRightDiagonalBorder : BorderBase {
		public TopRightDiagonalBorder(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		public TopRightDiagonalBorder(PieceTable pieceTable)
			: this(pieceTable, null) {
		}
		protected override Properties Property { get { return Properties.TopRightDiagonalBorder; } }
	}
	#endregion
	#region BorderChangeType
	public enum BorderChangeType {
		None = 0,
		Style,
		Color,
		Width,
		Offset,
		Frame,
		Shadow,
		BatchUpdate
	}
	#endregion
	#region BorderChangeActionsCalculator
	public static class BorderChangeActionsCalculator {
		internal class BorderChangeActionsTable : Dictionary<BorderChangeType, DocumentModelChangeActions> {
		}
		internal static readonly BorderChangeActionsTable borderChangeActionsTable = CreateBorderChangeActionsTable();
		internal static BorderChangeActionsTable CreateBorderChangeActionsTable() {
			BorderChangeActionsTable table = new BorderChangeActionsTable();
			table.Add(BorderChangeType.None, DocumentModelChangeActions.None);
			table.Add(BorderChangeType.Style, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(BorderChangeType.Color, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(BorderChangeType.Width, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(BorderChangeType.Offset, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(BorderChangeType.Frame, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(BorderChangeType.Shadow, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(BorderChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(BorderChangeType change) {
			return borderChangeActionsTable[change];
		}
	}
	#endregion
	public interface ITableCellBorders {
		BorderBase TopBorder { get; }
		BorderBase BottomBorder { get; }
		BorderBase LeftBorder { get; }
		BorderBase RightBorder { get; }
	}
	public interface ITableBorders : ITableCellBorders {
		BorderBase InsideHorizontalBorder { get; }
		BorderBase InsideVerticalBorder { get; }
	}
	#region BordersBase<TOptionsMask> (abstract class)
	public abstract class BordersBase<TOptionsMask> : ITableBorders where TOptionsMask : struct {
		#region Fields
		readonly IPropertiesContainerWithMask<TOptionsMask> owner;
		readonly BottomBorder bottomBorder;
		readonly LeftBorder leftBorder;
		readonly RightBorder rightBorder;
		readonly TopBorder topBorder;
		readonly InsideHorizontalBorder insideHorizontalBorder;
		readonly InsideVerticalBorder insideVerticalBorder;
		#endregion
		protected BordersBase(PieceTable pieceTable, IPropertiesContainerWithMask<TOptionsMask> owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.topBorder = new TopBorder(pieceTable, owner);
			this.leftBorder = new LeftBorder(pieceTable, owner);
			this.rightBorder = new RightBorder(pieceTable, owner);
			this.bottomBorder = new BottomBorder(pieceTable, owner);
			this.insideHorizontalBorder = new InsideHorizontalBorder(pieceTable, owner);
			this.insideVerticalBorder = new InsideVerticalBorder(pieceTable, owner);
		}
		#region Properties
		public TopBorder TopBorder { get { return topBorder; } }
		public LeftBorder LeftBorder { get { return leftBorder; } }
		public RightBorder RightBorder { get { return rightBorder; } }
		public BottomBorder BottomBorder { get { return bottomBorder; } }
		public InsideHorizontalBorder InsideHorizontalBorder { get { return insideHorizontalBorder; } }
		public InsideVerticalBorder InsideVerticalBorder { get { return insideVerticalBorder; } }
		public bool UseLeftBorder { get { return owner.GetUse(UseLeftBorderMask); } }
		public bool UseRightBorder { get { return owner.GetUse(UseRightBorderMask); } }
		public bool UseTopBorder { get { return owner.GetUse(UseTopBorderMask); } }
		public bool UseBottomBorder { get { return owner.GetUse(UseBottomBorderMask); } }
		public bool UseInsideHorizontalBorder { get { return owner.GetUse(UseInsideHorizontalBorderMask); } }
		public bool UseInsideVerticalBorder { get { return owner.GetUse(UseInsideVerticalBorderMask); } }
		internal IPropertiesContainerWithMask<TOptionsMask> Owner { get { return owner; } }
		protected internal abstract TOptionsMask UseLeftBorderMask { get; }
		protected internal abstract TOptionsMask UseRightBorderMask { get; }
		protected internal abstract TOptionsMask UseTopBorderMask { get; }
		protected internal abstract TOptionsMask UseBottomBorderMask { get; }
		protected internal abstract TOptionsMask UseInsideHorizontalBorderMask { get; }
		protected internal abstract TOptionsMask UseInsideVerticalBorderMask { get; }
		BorderBase ITableCellBorders.TopBorder { get { return TopBorder; } }
		BorderBase ITableCellBorders.RightBorder { get { return RightBorder; } }
		BorderBase ITableCellBorders.BottomBorder { get { return BottomBorder; } }
		BorderBase ITableCellBorders.LeftBorder { get { return LeftBorder; } }
		BorderBase ITableBorders.InsideHorizontalBorder { get { return InsideHorizontalBorder; } }
		BorderBase ITableBorders.InsideVerticalBorder { get { return insideVerticalBorder; } }
		#endregion
		public void CopyFrom(BordersBase<TOptionsMask> borders) {
			Owner.BeginPropertiesUpdate();
			try {
				CopyFromCore(borders);
			}
			finally {
				Owner.EndPropertiesUpdate();
			}
		}
		protected internal virtual void CopyFromCore(BordersBase<TOptionsMask> borders) {
			if (Object.ReferenceEquals(TopBorder.DocumentModel, borders.TopBorder.DocumentModel)) {
				if (borders.UseTopBorder)
					TopBorder.CopyFrom(borders.TopBorder);
				if (borders.UseLeftBorder)
					LeftBorder.CopyFrom(borders.LeftBorder);
				if (borders.UseRightBorder)
					RightBorder.CopyFrom(borders.RightBorder);
				if (borders.UseBottomBorder)
					BottomBorder.CopyFrom(borders.BottomBorder);
				if (borders.UseInsideHorizontalBorder)
					InsideHorizontalBorder.CopyFrom(borders.InsideHorizontalBorder);
				if (borders.UseInsideVerticalBorder)
					InsideVerticalBorder.CopyFrom(borders.InsideVerticalBorder);
			}
			else {				
				if (borders.UseTopBorder)
					TopBorder.CopyFrom(borders.TopBorder.Info);
				if (borders.UseLeftBorder)
					LeftBorder.CopyFrom(borders.LeftBorder.Info);
				if (borders.UseRightBorder)
					RightBorder.CopyFrom(borders.RightBorder.Info);
				if (borders.UseBottomBorder)
					BottomBorder.CopyFrom(borders.BottomBorder.Info);
				if (borders.UseInsideHorizontalBorder)
					InsideHorizontalBorder.CopyFrom(borders.InsideHorizontalBorder.Info);
				if (borders.UseInsideVerticalBorder)
					InsideVerticalBorder.CopyFrom(borders.InsideVerticalBorder.Info);
			}
		}
		public virtual void Merge(BordersBase<TOptionsMask> borders) {
			if (!UseLeftBorder && borders.UseLeftBorder)
				LeftBorder.CopyFrom(borders.LeftBorder);
			if (!UseRightBorder && borders.UseRightBorder)
				RightBorder.CopyFrom(borders.RightBorder);
			if (!UseTopBorder && borders.UseTopBorder)
				TopBorder.CopyFrom(borders.TopBorder);
			if (!UseBottomBorder && borders.UseBottomBorder)
				BottomBorder.CopyFrom(borders.BottomBorder);
			if (!UseInsideHorizontalBorder && borders.UseInsideHorizontalBorder)
				InsideHorizontalBorder.CopyFrom(borders.InsideHorizontalBorder);
			if (!UseInsideVerticalBorder && borders.UseInsideVerticalBorder)
				InsideVerticalBorder.CopyFrom(borders.InsideVerticalBorder);
		}
	}
	#endregion
	#region TableBorders
	[HashtableConverter("DevExpress.Web.ASPxRichEdit.Export.TableBordersExporter", true, true)]
	public class TableBorders : BordersBase<TablePropertiesOptions.Mask> {
		public TableBorders(PieceTable pieceTable, ITablePropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected internal override TablePropertiesOptions.Mask UseLeftBorderMask { get { return TablePropertiesOptions.Mask.UseLeftBorder; } }
		protected internal override TablePropertiesOptions.Mask UseRightBorderMask { get { return TablePropertiesOptions.Mask.UseRightBorder; } }
		protected internal override TablePropertiesOptions.Mask UseTopBorderMask { get { return TablePropertiesOptions.Mask.UseTopBorder; } }
		protected internal override TablePropertiesOptions.Mask UseBottomBorderMask { get { return TablePropertiesOptions.Mask.UseBottomBorder; } }
		protected internal override TablePropertiesOptions.Mask UseInsideHorizontalBorderMask { get { return TablePropertiesOptions.Mask.UseInsideHorizontalBorder; } }
		protected internal override TablePropertiesOptions.Mask UseInsideVerticalBorderMask { get { return TablePropertiesOptions.Mask.UseInsideVerticalBorder; } }
	}
	#endregion
	#region TableCellBorders
	[HashtableConverter("TableCellBordersExporter", true, true)]
	public class TableCellBorders : BordersBase<TableCellPropertiesOptions.Mask>, ITableCellBorders {
		#region Fields
		readonly TopLeftDiagonalBorder topLeftDiagonalBorder;
		readonly TopRightDiagonalBorder topRightDiagonalBorder;
		#endregion
		public TableCellBorders(PieceTable pieceTable, ICellPropertiesContainer owner)
			: base(pieceTable, owner) {
			this.topLeftDiagonalBorder = new TopLeftDiagonalBorder(pieceTable, owner);
			this.topRightDiagonalBorder = new TopRightDiagonalBorder(pieceTable, owner);
		}
		#region Properties
		public TopLeftDiagonalBorder TopLeftDiagonalBorder { get { return topLeftDiagonalBorder; } }
		public TopRightDiagonalBorder TopRightDiagonalBorder { get { return topRightDiagonalBorder; } }
		public bool UseTopLeftDiagonalBorder { get { return Owner.GetUse(TableCellPropertiesOptions.Mask.UseTopLeftDiagonalBorder); } }
		public bool UseTopRightDiagonalBorder { get { return Owner.GetUse(TableCellPropertiesOptions.Mask.UseTopRightDiagonalBorder); } }
		protected internal override TableCellPropertiesOptions.Mask UseLeftBorderMask { get { return TableCellPropertiesOptions.Mask.UseLeftBorder; } }
		protected internal override TableCellPropertiesOptions.Mask UseRightBorderMask { get { return TableCellPropertiesOptions.Mask.UseRightBorder; } }
		protected internal override TableCellPropertiesOptions.Mask UseTopBorderMask { get { return TableCellPropertiesOptions.Mask.UseTopBorder; } }
		protected internal override TableCellPropertiesOptions.Mask UseBottomBorderMask { get { return TableCellPropertiesOptions.Mask.UseBottomBorder; } }
		protected internal override TableCellPropertiesOptions.Mask UseInsideHorizontalBorderMask { get { return TableCellPropertiesOptions.Mask.UseInsideHorizontalBorder; } }
		protected internal override TableCellPropertiesOptions.Mask UseInsideVerticalBorderMask { get { return TableCellPropertiesOptions.Mask.UseInsideVerticalBorder; } }
		#endregion
		protected internal override void CopyFromCore(BordersBase<TableCellPropertiesOptions.Mask> borders) {
			base.CopyFromCore(borders);
			TableCellBorders cellBorders = borders as TableCellBorders;
			if (cellBorders == null)
				return;
			if (Object.ReferenceEquals(LeftBorder.DocumentModel, borders.LeftBorder.DocumentModel)) {
				if (cellBorders.UseTopLeftDiagonalBorder)
					TopLeftDiagonalBorder.CopyFrom(cellBorders.TopLeftDiagonalBorder);
				if (cellBorders.UseTopRightDiagonalBorder)
					TopRightDiagonalBorder.CopyFrom(cellBorders.TopRightDiagonalBorder);
			}
			else {
				if (cellBorders.UseTopLeftDiagonalBorder)
					TopLeftDiagonalBorder.CopyFrom(cellBorders.TopLeftDiagonalBorder.Info);
				if (cellBorders.UseTopRightDiagonalBorder)
					TopRightDiagonalBorder.CopyFrom(cellBorders.TopRightDiagonalBorder.Info);
			}
		}
		public override void Merge(BordersBase<TableCellPropertiesOptions.Mask> borders) {
			base.Merge(borders);
			TableCellBorders cellBorders = borders as TableCellBorders;
			if (cellBorders == null)
				return;
			if (!UseTopLeftDiagonalBorder && cellBorders.UseTopLeftDiagonalBorder)
				TopLeftDiagonalBorder.CopyFrom(cellBorders.TopLeftDiagonalBorder);
			if (!UseTopRightDiagonalBorder && cellBorders.UseTopRightDiagonalBorder)
				TopRightDiagonalBorder.CopyFrom(cellBorders.TopRightDiagonalBorder);
		}
		public void CopyFrom(CombinedCellBordersInfo borders) {
			Owner.BeginPropertiesUpdate();
			try {
				TopBorder.CopyFrom(borders.TopBorder);
				LeftBorder.CopyFrom(borders.LeftBorder);
				RightBorder.CopyFrom(borders.RightBorder);
				BottomBorder.CopyFrom(borders.BottomBorder);
				InsideHorizontalBorder.CopyFrom(borders.InsideHorizontalBorder);
				InsideVerticalBorder.CopyFrom(borders.InsideVerticalBorder);
				TopLeftDiagonalBorder.CopyFrom(borders.TopLeftDiagonalBorder);
				TopRightDiagonalBorder.CopyFrom(borders.TopRightDiagonalBorder);
			}
			finally {
				Owner.EndPropertiesUpdate();
			}
		}
	}
	#endregion
	#region PageBorderDisplay
	public enum PageBorderDisplay {
		AllPages,
		FirstPage,
		NotFirstPage
	}
	#endregion
	#region PageBorderOffset
	public enum PageBorderOffset {
		Page,
		Text
	}
	#endregion
}
