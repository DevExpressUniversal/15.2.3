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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region IPropertiesContainer
	public interface IPropertiesContainer {
		void BeginChanging(Properties changedProperty);
		void EndChanging();
		void BeginPropertiesUpdate();
		void EndPropertiesUpdate();
		void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args);
		void ResetPropertyUse(Properties Property);
		void ApplyChanges(DocumentModelChangeActions changeActions);
	}
	#endregion
	public interface IPropertiesContainerWithMask<TOptionsMask> : IPropertiesContainer where TOptionsMask : struct {
		bool GetUse(TOptionsMask mask);
	}
	#region ICellPropertiesContainer
	public interface ICellPropertiesContainer : IPropertiesContainerWithMask<TableCellPropertiesOptions.Mask> {
	}
	#endregion
	#region ICellMarginsContainer
	public interface ICellMarginsContainer : IPropertiesContainer {
		bool UseLeftMargin { get; }
		bool UseRightMargin { get; }
		bool UseTopMargin { get; }
		bool UseBottomMargin { get; }
	}
	#endregion
	#region ICellPropertiesOwner
	public interface ICellPropertiesOwner {
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateCellPropertiesChangedHistoryItem(TableCellProperties properties);
	}
	#endregion
	#region ITablePropertiesContainer
	public interface ITablePropertiesContainer : IPropertiesContainerWithMask<TablePropertiesOptions.Mask> {
	}
	#endregion
	#region Properties
	public enum Properties {
		TopMargin,
		LeftMargin,
		RightMargin,
		BottomMargin,
		CellSpacing,
		PreferredWidth,
		TableIndent,
		TableLayout,
		TableLook,
		TableStyleColumnBandSize,
		TableStyleRowBandSize,
		IsTableOverlap,
		AvoidDoubleBorders,
		LeftBorder,
		RightBorder,
		TopBorder,
		BottomBorder,
		InsideHorizontalBorder,
		InsideVerticalBorder,
		TopLeftDiagonalBorder,
		TopRightDiagonalBorder,
		HideCellMark,
		NoWrap,
		FitText,
		TextDirection,
		VerticalAlignment,
		ColumnSpan,
		VerticalMerging,
		CellConditionalFormatting,
		RowHeight,
		WidthBefore,
		WidthAfter,
		Header,
		TableRowAlignment,
		TableRowConditionalFormatting,
		GridAfter,
		GridBefore,
		CantSplit,
		TableFloatingPosition,
		BackgroundColor,
		ForegroundColor,
		ShadingPattern,
#if THEMES_EDIT
		Shading,
#endif
		CellGeneralSettings,
		Borders,
		TableAlignment
	}
	#endregion
	public class PropertiesDictionary<T> : Dictionary<Properties, T> {
		class EqualityComparer : IEqualityComparer<Properties> {
			public bool Equals(Properties x, Properties y) {
				return x == y;
			}
			public int GetHashCode(Properties obj) {
				return (int)obj;
			}
			internal static EqualityComparer Default = new EqualityComparer();
		}
		public PropertiesDictionary()
			: base(EqualityComparer.Default) {
		}
	}
	#region Enumerators
	public enum HorizontalAlignMode {
		None,
		Center, 
		Inside, 
		Left, 
		Outside, 
		Right
	}
	public enum VerticalAlignMode {
		None,
		Bottom, 
		Center,
		Inline, 
		Inside, 
		Outside,
		Top
	}
	public enum VerticalAnchorTypes {
		Margin, 
		Page, 
		Paragraph
	}
	public enum HorizontalAnchorTypes {
		Margin, 
		Page, 
		Column
	}
	public enum WidthUnitType {
		Nil = 0, 
		Auto = 1,
		FiftiethsOfPercent = 2, 
		ModelUnits = 3
	}
	public enum TextWrapping {
		Never,
		Around
	}
	#endregion
	#region TableFloatingPositionInfo
	public class TableFloatingPositionInfo : ICloneable<TableFloatingPositionInfo>, ISupportsCopyFrom<TableFloatingPositionInfo>, ISupportsSizeOf {
		#region Fields
		int bottomFromText;
		int leftFromText;
		int topFromText;
		int rightFromText;
		int tableHorizontalPosition;
		int tableVerticalPosition;
		HorizontalAlignMode horizAlign;
		VerticalAlignMode vertAlign;
		HorizontalAnchorTypes horizAnchor;
		VerticalAnchorTypes vertAnchor;
		TextWrapping textWrapping;
		#endregion
		#region Properties
		public int BottomFromText { get { return bottomFromText; } set { bottomFromText = value; } }
		public int LeftFromText { get { return leftFromText; } set { leftFromText = value; } }
		public int TopFromText { get { return topFromText; } set { topFromText = value; } }
		public int RightFromText { get { return rightFromText; } set { rightFromText = value; } }
		public int TableHorizontalPosition { get { return tableHorizontalPosition; } set { tableHorizontalPosition = value; } }
		public int TableVerticalPosition { get { return tableVerticalPosition; } set { tableVerticalPosition = value; } }
		public HorizontalAlignMode HorizontalAlign { get { return horizAlign; } set { horizAlign = value; } }
		public VerticalAlignMode VerticalAlign { get { return vertAlign; } set { vertAlign = value; } }
		public HorizontalAnchorTypes HorizontalAnchor { get { return horizAnchor; } set { horizAnchor = value; } }
		public VerticalAnchorTypes VerticalAnchor { get { return vertAnchor; } set { vertAnchor = value; } }
		public TextWrapping TextWrapping { get { return textWrapping; } set { textWrapping = value; } }
		internal bool IsHorizontalAbsolutePositionUse { get { return HorizontalAlign == HorizontalAlignMode.None; } }
		internal bool IsVerticalAbsolutePositionUse { get { return VerticalAlign == VerticalAlignMode.None; } }
		#endregion
		public override bool Equals(object obj) {
			TableFloatingPositionInfo pos = obj as TableFloatingPositionInfo;
			if (pos == null)
				return false;
			return this.BottomFromText == pos.BottomFromText &&
				   this.LeftFromText == pos.LeftFromText &&
				   this.TopFromText == pos.TopFromText &&
				   this.RightFromText == pos.RightFromText &&
				   this.TableHorizontalPosition == pos.TableHorizontalPosition &&
				   this.TableVerticalPosition == pos.TableVerticalPosition &&
				   this.HorizontalAlign == pos.HorizontalAlign &&
				   this.VerticalAlign == pos.VerticalAlign &&
				   this.HorizontalAnchor == pos.HorizontalAnchor &&
				   this.VerticalAnchor == pos.VerticalAnchor &&
				   this.TextWrapping == pos.TextWrapping;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void CopyFrom(TableFloatingPositionInfo info) {
			this.BottomFromText = info.BottomFromText;
			this.LeftFromText = info.LeftFromText;
			this.TopFromText = info.TopFromText;
			this.RightFromText = info.RightFromText;
			this.TableHorizontalPosition = info.TableHorizontalPosition;
			this.TableVerticalPosition = info.TableVerticalPosition;
			this.HorizontalAlign = info.HorizontalAlign;
			this.VerticalAlign = info.VerticalAlign;
			this.HorizontalAnchor = info.HorizontalAnchor;
			this.VerticalAnchor = info.VerticalAnchor;
			this.TextWrapping = info.TextWrapping;
		}
		public TableFloatingPositionInfo Clone() {
			TableFloatingPositionInfo info = new TableFloatingPositionInfo();
			info.CopyFrom(this);
			return info;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TableFloatingPositionInfoCache
	public class TableFloatingPositionInfoCache : UniqueItemsCache<TableFloatingPositionInfo> {
		public TableFloatingPositionInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TableFloatingPositionInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			TableFloatingPositionInfo info = new TableFloatingPositionInfo();
			info.HorizontalAlign = HorizontalAlignMode.None;
			info.VerticalAlign = VerticalAlignMode.None;
			info.HorizontalAnchor = HorizontalAnchorTypes.Page;
			info.VerticalAnchor = VerticalAnchorTypes.Page;
			info.TableHorizontalPosition = 0;
			info.TableVerticalPosition = 0;
			info.TextWrapping = TextWrapping.Never;
			return info;
		}
	}
	#endregion
	#region TableFloatingPosition
	public class TableFloatingPosition : RichEditIndexBasedObject<TableFloatingPositionInfo> {
		readonly IPropertiesContainer owner;
		public TableFloatingPosition(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			this.owner = owner;
		}
		#region Properties
		#region BottomFromText
		public int BottomFromText {
			get { return Info.BottomFromText; }
			set {
				if (value == Info.BottomFromText) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetBottomFromText, value);
			}
		}
		DocumentModelChangeActions SetBottomFromText(TableFloatingPositionInfo info, int value) {
			info.BottomFromText = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.BottomFromText);
		}
		#endregion
		#region LeftFromText
		public int LeftFromText {
			get { return Info.LeftFromText; }
			set {
				if (value == Info.LeftFromText) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetLeftFromText, value);
			}
		}
		DocumentModelChangeActions SetLeftFromText(TableFloatingPositionInfo info, int value) {
			info.LeftFromText = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.LeftFromText);
		}
		#endregion
		#region TopFromText
		public int TopFromText {
			get { return Info.TopFromText; }
			set {
				if (value == Info.TopFromText) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetTopFromText, value);
			}
		}
		DocumentModelChangeActions SetTopFromText(TableFloatingPositionInfo info, int value) {
			info.TopFromText = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.TopFromText);
		}
		#endregion
		#region RightFromText
		public int RightFromText {
			get { return Info.RightFromText; }
			set {
				if (value == Info.RightFromText) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetRightFromText, value);
			}
		}
		DocumentModelChangeActions SetRightFromText(TableFloatingPositionInfo info, int value) {
			info.RightFromText = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.RightFromText);
		}
		#endregion
		#region TableHorizontalPosition
		public int TableHorizontalPosition {
			get { return Info.TableHorizontalPosition; }
			set {
				if (value == Info.TableHorizontalPosition) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetTableHorizontalPosition, value);
			}
		}
		DocumentModelChangeActions SetTableHorizontalPosition(TableFloatingPositionInfo info, int value) {
			info.TableHorizontalPosition = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.TableHorizontalPosition);
		}
		#endregion
		#region TableVerticalPosition
		public int TableVerticalPosition {
			get { return Info.TableVerticalPosition; }
			set {
				if (value == Info.TableVerticalPosition) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetTableVerticalPosition, value);
			}
		}
		DocumentModelChangeActions SetTableVerticalPosition(TableFloatingPositionInfo info, int value) {
			info.TableVerticalPosition = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.TableVerticalPosition);
		}
		#endregion
		#region HorizontalAlign
		public HorizontalAlignMode HorizontalAlign {
			get { return Info.HorizontalAlign; }
			set {
				if (value == Info.HorizontalAlign) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetHorizontalAlign, value);
			}
		}
		DocumentModelChangeActions SetHorizontalAlign(TableFloatingPositionInfo info, HorizontalAlignMode value) {
			info.HorizontalAlign = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.HorizontalAlign);
		}
		#endregion
		#region VerticalAlign
		public VerticalAlignMode VerticalAlign {
			get { return Info.VerticalAlign; }
			set {
				if (value == Info.VerticalAlign) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetVerticalAlign, value);
			}
		}
		DocumentModelChangeActions SetVerticalAlign(TableFloatingPositionInfo info, VerticalAlignMode value) {
			info.VerticalAlign = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.VerticalAlign);
		}
		#endregion
		#region HorizontalAnchor
		public HorizontalAnchorTypes HorizontalAnchor {
			get { return Info.HorizontalAnchor; }
			set {
				if (value == Info.HorizontalAnchor) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetHorizontalAnchor, value);
			}
		}
		DocumentModelChangeActions SetHorizontalAnchor(TableFloatingPositionInfo info, HorizontalAnchorTypes value) {
			info.HorizontalAnchor = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.HorizontalAnchor);
		}
		#endregion
		#region VerticalAnchor
		public VerticalAnchorTypes VerticalAnchor {
			get { return Info.VerticalAnchor; }
			set {
				if (value == Info.VerticalAnchor) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetVerticalAnchor, value);
			}
		}
		DocumentModelChangeActions SetVerticalAnchor(TableFloatingPositionInfo info, VerticalAnchorTypes value) {
			info.VerticalAnchor = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.VerticalAnchor);
		}
		#endregion
		#region TextWrapping
		public TextWrapping TextWrapping {
			get { return Info.TextWrapping; }
			set {
				if (value == Info.TextWrapping) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetTextWrapping, value);
			}
		}
		DocumentModelChangeActions SetTextWrapping(TableFloatingPositionInfo info, TextWrapping value) {
			info.TextWrapping = value;
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.TextWrapping);
		}
		#endregion
		internal bool IsHorizontalRelativePositionUse { get { return HorizontalAlign != HorizontalAlignMode.None; } }
		internal bool IsVerticalRelativePositionUse { get { return VerticalAlign != VerticalAlignMode.None; } }
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<TableFloatingPositionInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TableFloatingPositionInfoCache;
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			if (Owner != null)
				Owner.BeginChanging(Properties.TableFloatingPosition);
		}
		protected override void OnEndAssign() {
			if (Owner != null)
				Owner.EndChanging();
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType.BatchUpdate);
		}
		public bool CompareTo(object obj) {
			TableFloatingPosition other = obj as TableFloatingPosition;
			if (other == null)
				return false;
			return this.BottomFromText == other.BottomFromText &&
				this.HorizontalAlign == other.HorizontalAlign &&
				this.HorizontalAnchor == other.HorizontalAnchor &&
				this.LeftFromText == other.LeftFromText &&
				this.TableHorizontalPosition == other.TableHorizontalPosition &&
				this.TableVerticalPosition == other.TableVerticalPosition &&
				this.TopFromText == other.TopFromText &&
				this.VerticalAlign == other.VerticalAlign &&
				this.VerticalAnchor == other.VerticalAnchor;
		}
	}
	#endregion
	#region TableFloatingPositionChangeType
	public enum TableFloatingPositionChangeType {
		None = 0,
		LeftFromText,
		RightFromText,
		TopFromText,
		BottomFromText,
		TableHorizontalPosition,
		TableVerticalPosition,
		HorizontalAlign,
		VerticalAlign,
		HorizontalAnchor,
		VerticalAnchor,
		TextWrapping,
		BatchUpdate
	}
	#endregion
	#region TableFloatingPositionChangeActionsCalculator
	public static class TableFloatingPositionChangeActionsCalculator {
		internal class TableFloatingPositionChangeActionsTable : Dictionary<TableFloatingPositionChangeType, DocumentModelChangeActions> {
		}
		internal static readonly TableFloatingPositionChangeActionsTable tableFloatingPositionChangeActionsTable = CreateTableFloatingPositionChangeActionsTable();
		internal static TableFloatingPositionChangeActionsTable CreateTableFloatingPositionChangeActionsTable() {
			TableFloatingPositionChangeActionsTable table = new TableFloatingPositionChangeActionsTable();
			table.Add(TableFloatingPositionChangeType.None, DocumentModelChangeActions.None);
			table.Add(TableFloatingPositionChangeType.LeftFromText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.RightFromText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.TopFromText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.BottomFromText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.TableHorizontalPosition, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.TableVerticalPosition, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.HorizontalAlign, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.VerticalAlign, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.HorizontalAnchor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.VerticalAnchor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.TextWrapping, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableFloatingPositionChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(TableFloatingPositionChangeType change) {
			return tableFloatingPositionChangeActionsTable[change];
		}
	}
	#endregion
	#region WidthUnitInfo
	public class WidthUnitInfo : ICloneable<WidthUnitInfo>, ISupportsCopyFrom<WidthUnitInfo>, ISupportsSizeOf {
		#region Fields
		WidthUnitType type;
		int val;
		#endregion
		public WidthUnitInfo() {
		}
		public WidthUnitInfo(WidthUnitType type, int val) {
			this.Type = type;
			this.Value = val;
		}
		#region Properties
		public WidthUnitType Type { get { return type; } set { type = value; } }
		public int Value { get { return val; } set { val = value; } }
		#endregion
		public void CopyFrom(WidthUnitInfo info) {
			Type = info.Type;
			Value = info.Value;
		}
		public WidthUnitInfo Clone() {
			WidthUnitInfo info = new WidthUnitInfo();
			info.CopyFrom(this);
			return info;
		}
		public override bool Equals(object obj) {
			WidthUnitInfo info = obj as WidthUnitInfo;
			if (info == null)
				return false;
			return Type == info.Type && Value == info.Value;
		}
		public override int GetHashCode() {
			return ((int)Type << 3) | Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region WidthUnitInfoCache
	public class WidthUnitInfoCache : UniqueItemsCache<WidthUnitInfo> {
		public WidthUnitInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override WidthUnitInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new WidthUnitInfo();
		}
	}
	#endregion
	public abstract class WidthUnit : RichEditIndexBasedObject<WidthUnitInfo> {
		readonly IPropertiesContainer owner;
		protected WidthUnit(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		#region Properties
		#region Type
		public WidthUnitType Type {
			get { return Info.Type; }
			set {
				if (value == Info.Type) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetTypeCore, value);
			}
		}
		protected virtual DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			unit.Type = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Value
		public int Value {
			get { return Info.Value; }
			set {
				if (value == Info.Value) {
					NotifyFakeAssign();
					return;
				}
				SetPropertyValue(SetValueCore, value);
			}
		}
		protected virtual DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			unit.Value = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<WidthUnitInfo> GetCache(DocumentModel documentModel) {
			return DocumentModel.Cache.UnitInfoCache;
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
		}
		protected override void OnEndAssign() {
			Owner.EndChanging();
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected internal override void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			if (Owner != null)
				Owner.RaiseObtainAffectedRange(args);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			if (Owner != null)
				Owner.ApplyChanges(changeActions);
			else
				base.ApplyChanges(changeActions);
		}
	}
	#region PreferredWidth
	[HashtableConverter("DevExpress.Web.ASPxRichEdit.Export.PreferredWidthExporter", true, true)]
	public class PreferredWidth : WidthUnit {
		public PreferredWidth(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Properties.PreferredWidth);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			return base.SetTypeCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			return base.SetValueCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
	}
	#endregion
	#region TableIndent
	[HashtableConverter("DevExpress.Web.ASPxRichEdit.Export.TableIndentExporter", true, true)]
	public class TableIndent : WidthUnit {
		public TableIndent(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Properties.TableIndent);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			return base.SetTypeCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			return base.SetValueCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
	}
	#endregion
	#region CellSpacing
	public class CellSpacing : WidthUnit {
		public CellSpacing(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Properties.CellSpacing);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			return base.SetTypeCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			return base.SetValueCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
	}
	#endregion
	#region MarginUnitBase (abstract class)
	public abstract class MarginUnitBase : WidthUnit {
		public abstract class MarginPropertyAccessorBase {
			public abstract TableCellPropertiesOptions.Mask CellPropertiesMask { get; }
			public abstract TablePropertiesOptions.Mask TablePropertiesMask { get; }
			public abstract MarginUnitBase GetValue(CellMargins cellMargins);
		}
		protected MarginUnitBase(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected abstract Properties Property { get; }
		protected override void OnBeginAssign() {
			base.OnBeginAssign();
			Owner.BeginChanging(Property);
		}
		protected override void OnEndAssign() {
			base.OnEndAssign();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetTypeCore(WidthUnitInfo unit, WidthUnitType value) {
			return base.SetTypeCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
		protected override DocumentModelChangeActions SetValueCore(WidthUnitInfo unit, int value) {
			return base.SetValueCore(unit, value) | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler;
		}
	}
	#endregion
	#region LeftMarginUnit
	public class LeftMarginUnit : MarginUnitBase {
		static readonly MarginPropertyAccessorBase propertyAccessor = new LeftPropertyAccessor();
		public static MarginPropertyAccessorBase PropertyAccessor { get { return propertyAccessor; } }
		protected class LeftPropertyAccessor : MarginPropertyAccessorBase {
			public override TableCellPropertiesOptions.Mask CellPropertiesMask { get { return TableCellPropertiesOptions.Mask.UseLeftMargin; } }
			public override TablePropertiesOptions.Mask TablePropertiesMask { get { return TablePropertiesOptions.Mask.UseLeftMargin; } }
			public override MarginUnitBase GetValue(CellMargins cellMargins) {
				return cellMargins.Left;
			}
		}
		public LeftMarginUnit(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override Properties Property { get { return Properties.LeftMargin; } }
	}
	#endregion
	#region RightMarginUnit
	public class RightMarginUnit : MarginUnitBase {
		static readonly MarginPropertyAccessorBase propertyAccessor = new RightPropertyAccessor();
		public static MarginPropertyAccessorBase PropertyAccessor { get { return propertyAccessor; } }
		protected class RightPropertyAccessor : MarginPropertyAccessorBase {
			public override TableCellPropertiesOptions.Mask CellPropertiesMask { get { return TableCellPropertiesOptions.Mask.UseRightMargin; } }
			public override TablePropertiesOptions.Mask TablePropertiesMask { get { return TablePropertiesOptions.Mask.UseRightMargin; } }
			public override MarginUnitBase GetValue(CellMargins cellMargins) {
				return cellMargins.Right;
			}
		}
		public RightMarginUnit(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override Properties Property { get { return Properties.RightMargin; } }
	}
	#endregion
	#region TopMarginUnit
	public class TopMarginUnit : MarginUnitBase {
		static readonly MarginPropertyAccessorBase propertyAccessor = new TopPropertyAccessor();
		public static MarginPropertyAccessorBase PropertyAccessor { get { return propertyAccessor; } }
		protected class TopPropertyAccessor : MarginPropertyAccessorBase {
			public override TableCellPropertiesOptions.Mask CellPropertiesMask { get { return TableCellPropertiesOptions.Mask.UseTopMargin; } }
			public override TablePropertiesOptions.Mask TablePropertiesMask { get { return TablePropertiesOptions.Mask.UseTopMargin; } }
			public override MarginUnitBase GetValue(CellMargins cellMargins) {
				return cellMargins.Top;
			}
		}
		public TopMarginUnit(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override Properties Property { get { return Properties.TopMargin; } }
	}
	#endregion
	#region BottomMarginUnit
	public class BottomMarginUnit : MarginUnitBase {
		static readonly MarginPropertyAccessorBase propertyAccessor = new BottomPropertyAccessor();
		public static MarginPropertyAccessorBase PropertyAccessor { get { return propertyAccessor; } }
		protected class BottomPropertyAccessor : MarginPropertyAccessorBase {
			public override TableCellPropertiesOptions.Mask CellPropertiesMask { get { return TableCellPropertiesOptions.Mask.UseBottomMargin; } }
			public override TablePropertiesOptions.Mask TablePropertiesMask { get { return TablePropertiesOptions.Mask.UseBottomMargin; } }
			public override MarginUnitBase GetValue(CellMargins cellMargins) {
				return cellMargins.Bottom;
			}
		}
		public BottomMarginUnit(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable, owner) {
		}
		protected override Properties Property { get { return Properties.BottomMargin; } }
	}
	#endregion
	#region CellMarginsBase
	public class CellMargins {
		#region Fields
		readonly ICellMarginsContainer owner;
		readonly MarginUnitBase top;
		readonly MarginUnitBase left;
		readonly MarginUnitBase right;
		readonly MarginUnitBase bottom;
		#endregion
		public CellMargins(PieceTable pieceTable, ICellMarginsContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.top = CreateTopMargin(pieceTable);
			this.left = CreateLeftMargin(pieceTable);
			this.right = CreateRightMargin(pieceTable);
			this.bottom = CreateBottomMargin(pieceTable);
		}
		protected virtual TopMarginUnit CreateTopMargin(PieceTable pieceTable) {
			return new TopMarginUnit(pieceTable, owner);
		}
		protected virtual BottomMarginUnit CreateBottomMargin(PieceTable pieceTable) {
			return new BottomMarginUnit(pieceTable, owner);
		}
		protected virtual LeftMarginUnit CreateLeftMargin(PieceTable pieceTable) {
			return new LeftMarginUnit(pieceTable, owner);
		}
		protected virtual RightMarginUnit CreateRightMargin(PieceTable pieceTable) {
			return new RightMarginUnit(pieceTable, owner);
		}
		#region Properties
		protected internal ICellMarginsContainer Owner { get { return owner; } }
		public MarginUnitBase Top { get { return top; } }
		public MarginUnitBase Left { get { return left; } }
		public MarginUnitBase Right { get { return right; } }
		public MarginUnitBase Bottom { get { return bottom; } }
		public bool UseLeftMargin { get { return Owner.UseLeftMargin; } }
		public bool UseRightMargin { get { return Owner.UseRightMargin; } }
		public bool UseTopMargin { get { return Owner.UseTopMargin; } }
		public bool UseBottomMargin { get { return Owner.UseBottomMargin; } }
		#endregion
		public void CopyFrom(CellMargins newMargins) {
			Owner.BeginPropertiesUpdate();
			try {
				if (Object.ReferenceEquals(this.Left.DocumentModel, newMargins.Left.DocumentModel)) {
					Top.CopyFrom(newMargins.Top);
					Left.CopyFrom(newMargins.Left);
					Right.CopyFrom(newMargins.Right);
					Bottom.CopyFrom(newMargins.Bottom);
				}
				else {
					Top.CopyFrom(newMargins.Top.Info);
					Left.CopyFrom(newMargins.Left.Info);
					Right.CopyFrom(newMargins.Right.Info);
					Bottom.CopyFrom(newMargins.Bottom.Info);
				}
			}
			finally {
				Owner.EndPropertiesUpdate();
			}
		}
		public void CopyFrom(CombinedCellMarginsInfo newMargins) {
			Owner.BeginPropertiesUpdate();
			try {
				Top.CopyFrom(newMargins.Top);
				Left.CopyFrom(newMargins.Left);
				Right.CopyFrom(newMargins.Right);
				Bottom.CopyFrom(newMargins.Bottom);
			}
			finally {
				Owner.EndPropertiesUpdate();
			}
		}
		public void Merge(CellMargins margins) {
			if (!UseLeftMargin && margins.UseLeftMargin)
				Left.CopyFrom(margins.Left);
			if (!UseRightMargin && margins.UseRightMargin)
				Right.CopyFrom(margins.Right);
			if (!UseTopMargin && margins.UseTopMargin)
				Top.CopyFrom(margins.Top);
			if (!UseBottomMargin && margins.UseBottomMargin)
				Bottom.CopyFrom(margins.Bottom);
		}
	}
	#endregion
	#region TableGeneralSettingsInfo
	public class TableGeneralSettingsInfo : ICloneable<TableGeneralSettingsInfo>, ISupportsCopyFrom<TableGeneralSettingsInfo>, ISupportsSizeOf {
		#region Fields
		int tableStyleColumnBandSize;
		int tableStyleRowBandSize;
		bool isTableOverlap;
		bool avoidDoubleBorders;
		TableLayoutType tableLayout;
		TableLookTypes tableLook;
		Color backgroundColor;
#if THEMES_EDIT
		Shading shading;
#endif
		TableRowAlignment tableAlignment;
		#endregion
		#region Properties
		public int TableStyleColBandSize { get { return tableStyleColumnBandSize; } set { tableStyleColumnBandSize = value; } }
		public int TableStyleRowBandSize { get { return tableStyleRowBandSize; } set { tableStyleRowBandSize = value; } }
		public bool IsTableOverlap { get { return isTableOverlap; } set { isTableOverlap = value; } }
		public bool AvoidDoubleBorders { get { return avoidDoubleBorders; } set { avoidDoubleBorders = value; } }
		public TableLayoutType TableLayout { get { return tableLayout; } set { tableLayout = value; } }
		public TableRowAlignment TableAlignment { get { return tableAlignment; } set { tableAlignment = value; } }
		public TableLookTypes TableLook { get { return tableLook; } set { tableLook = value; } }
		public Color BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }
#if THEMES_EDIT
		public Shading Shading { get { return shading; } set { shading = value; } }
#endif
		#endregion
		public void CopyFrom(TableGeneralSettingsInfo info) {
			TableStyleColBandSize = info.TableStyleColBandSize;
			TableStyleRowBandSize = info.TableStyleRowBandSize;
			IsTableOverlap = info.IsTableOverlap;
			TableLayout = info.TableLayout;
			TableLook = info.TableLook;
			BackgroundColor = info.BackgroundColor;
			TableAlignment = info.TableAlignment;
			AvoidDoubleBorders = info.AvoidDoubleBorders;
#if THEMES_EDIT
			Shading = info.Shading;
#endif
		}
		public TableGeneralSettingsInfo Clone() {
			TableGeneralSettingsInfo info = new TableGeneralSettingsInfo();
			info.CopyFrom(this);
			return info;
		}
		public override bool Equals(object obj) {
			TableGeneralSettingsInfo info = obj as TableGeneralSettingsInfo;
			if (info == null)
				return false;
			return this.TableLayout == info.TableLayout &&
				   this.TableLook == info.TableLook &&
				   this.IsTableOverlap == info.IsTableOverlap &&
				   this.TableStyleColBandSize == info.TableStyleColBandSize &&
				   this.TableStyleRowBandSize == info.TableStyleRowBandSize &&
				   this.BackgroundColor == info.BackgroundColor &&
#if THEMES_EDIT
				   this.Shading.Equals(info.Shading) &&
#endif
				   this.TableAlignment == info.TableAlignment &&
				   this.AvoidDoubleBorders == info.AvoidDoubleBorders;
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
	#region TableGeneralSettingsInfoCache
	public class TableGeneralSettingsInfoCache : UniqueItemsCache<TableGeneralSettingsInfo> {
		public TableGeneralSettingsInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TableGeneralSettingsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			TableGeneralSettingsInfo result = new TableGeneralSettingsInfo();
			result.TableStyleColBandSize = 1;
			result.TableStyleRowBandSize = 1;
			result.IsTableOverlap = true;
			result.TableLayout = TableLayoutType.Autofit;
			result.TableLook = TableLookTypes.None;
			result.BackgroundColor = DXColor.Empty;
			result.TableAlignment = TableRowAlignment.Left;
#if THEMES_EDIT
			result.Shading = Shading.Create();
#endif
			return result;
		}
	}
	#endregion
	#region TableGeneralSettings
	public class TableGeneralSettings : RichEditIndexBasedObject<TableGeneralSettingsInfo> {
		readonly IPropertiesContainer owner;
		public TableGeneralSettings(PieceTable pieceTable, IPropertiesContainer owner)
			: base(pieceTable) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		#region Properties
		#region TableLayout
		public TableLayoutType TableLayout {
			get { return Info.TableLayout; }
			set {
				BeginChanging(Properties.TableLayout);
				if (Info.TableLayout == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableLayout, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableLayout(TableGeneralSettingsInfo settings, TableLayoutType value) {
			settings.TableLayout = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.TableLayout);
		}
		#endregion
		#region TableAlignment
		public TableRowAlignment TableAlignment {
			get { return Info.TableAlignment; }
			set {
				BeginChanging(Properties.TableAlignment);
				if (Info.TableAlignment == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableAlignment, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableAlignment(TableGeneralSettingsInfo settings, TableRowAlignment value) {
			settings.TableAlignment = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.TableAlignment);
		}
		#endregion
		#region TableLook
		public TableLookTypes TableLook {
			get { return Info.TableLook; }
			set {
				BeginChanging(Properties.TableLook);
				if (Info.TableLook == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableLook, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableLook(TableGeneralSettingsInfo settings, TableLookTypes value) {
			settings.TableLook = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.TableLook);
		}
		#endregion
		#region BackgroundColor
		public Color BackgroundColor {
			get { return Info.BackgroundColor; }
			set {
				BeginChanging(Properties.BackgroundColor);
				if (Info.BackgroundColor == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetBackgroundColor, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetBackgroundColor(TableGeneralSettingsInfo settings, Color value) {
			settings.BackgroundColor = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.BackgroundColor);
		}
		#endregion
#if THEMES_EDIT
		#region Shading
		public Shading Shading {
			get { return Info.Shading; }
			set {
				BeginChanging(Properties.ShadingPattern);
				if (Info.Shading == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetShadingColor, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetShadingColor(TableGeneralSettingsInfo settings, Shading value) {
			settings.Shading = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.BackgroundColor);
		}
		#endregion
#endif
		#region TableStyleColumnBandSize
		public int TableStyleColumnBandSize {
			get { return Info.TableStyleColBandSize; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("TableStyleColumnBandSize", value);
				BeginChanging(Properties.TableStyleColumnBandSize);
				if (Info.TableStyleColBandSize == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableStyleColumnBandSize, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableStyleColumnBandSize(TableGeneralSettingsInfo settings, int value) {
			settings.TableStyleColBandSize = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.TableStyleColumnBandSize);
		}
		#endregion
		#region TableStyleRowBandSize
		public int TableStyleRowBandSize {
			get { return Info.TableStyleRowBandSize; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("TableStyleRowBandSize", value);
				BeginChanging(Properties.TableStyleRowBandSize);
				if (Info.TableStyleRowBandSize == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableStyleRowBandSize, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableStyleRowBandSize(TableGeneralSettingsInfo settings, int value) {
			settings.TableStyleRowBandSize = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.TableStyleRowBandSize);
		}
		#endregion
		#region IsTableOverlap
		public bool IsTableOverlap {
			get { return Info.IsTableOverlap; }
			set {
				BeginChanging(Properties.IsTableOverlap);
				if (Info.IsTableOverlap == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetTableOverlap, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetTableOverlap(TableGeneralSettingsInfo settings, bool value) {
			settings.IsTableOverlap = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.IsTableOverlap);
		}
		#endregion
		#region AvoidDoubleBorders
		public bool AvoidDoubleBorders {
			get { return Info.AvoidDoubleBorders; }
			set {
				BeginChanging(Properties.AvoidDoubleBorders);
				if (Info.AvoidDoubleBorders == value) {
					EndChanging();
					return;
				}
				SetPropertyValue(SetAvoidDoubleBorders, value);
				EndChanging();
			}
		}
		DocumentModelChangeActions SetAvoidDoubleBorders(TableGeneralSettingsInfo settings, bool value) {
			settings.AvoidDoubleBorders = value;
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.AvoidDoubleBorders);
		}
		#endregion
		internal IPropertiesContainer Owner { get { return owner; } }
		#endregion
		protected internal override UniqueItemsCache<TableGeneralSettingsInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TableGeneralSettingsInfoCache;
		}
		public void CopyFrom(TableGeneralSettings newSettings) {
			Owner.BeginPropertiesUpdate();
			try {
				BeginUpdate();
				try {
					this.TableLayout = newSettings.TableLayout;
					this.TableLook = newSettings.TableLook;
					this.IsTableOverlap = newSettings.IsTableOverlap;
					this.TableStyleColumnBandSize = newSettings.TableStyleColumnBandSize;
					this.TableStyleRowBandSize = newSettings.TableStyleRowBandSize;
					this.BackgroundColor = newSettings.BackgroundColor;
					this.TableAlignment = newSettings.TableAlignment;
					this.AvoidDoubleBorders = newSettings.AvoidDoubleBorders;
#if THEMES_EDIT
					this.Shading = newSettings.Shading;
#endif
				}
				finally {
					EndUpdate();
				}
			}
			finally {
				Owner.EndPropertiesUpdate();
			}
		}
		protected virtual void BeginChanging(Properties changedProperty) {
			DocumentModel.BeginUpdate();
			Owner.BeginChanging(changedProperty);
		}
		protected virtual void EndChanging() {
			Owner.EndChanging();
			DocumentModel.EndUpdate();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableGeneralSettingsChangeType.BatchUpdate);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Owner.ApplyChanges(changeActions);
		}
		protected internal override void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			Owner.RaiseObtainAffectedRange(args);
		}
	}
	#endregion
	#region TableGeneralSettingsChangeType
	public enum TableGeneralSettingsChangeType {
		None = 0,
		TableLayout,
		TableLook,
		TableStyleColumnBandSize,
		TableStyleRowBandSize,
		IsTableOverlap,
		BackgroundColor,
		BatchUpdate,
		TableAlignment,
		AvoidDoubleBorders
	}
	#endregion
	#region TableGeneralSettingsChangeActionsCalculator
	public static class TableGeneralSettingsChangeActionsCalculator {
		internal class TableGeneralSettingsChangeActionsTable : Dictionary<TableGeneralSettingsChangeType, DocumentModelChangeActions> {
		}
		internal static readonly TableGeneralSettingsChangeActionsTable tableGeneralSettingsChangeActionsTable = CreateTableGeneralSettingsChangeActionsTable();
		internal static TableGeneralSettingsChangeActionsTable CreateTableGeneralSettingsChangeActionsTable() {
			TableGeneralSettingsChangeActionsTable table = new TableGeneralSettingsChangeActionsTable();
			table.Add(TableGeneralSettingsChangeType.None, DocumentModelChangeActions.None);
			table.Add(TableGeneralSettingsChangeType.TableLayout, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableGeneralSettingsChangeType.TableLook, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableGeneralSettingsChangeType.TableAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableGeneralSettingsChangeType.TableStyleColumnBandSize, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(TableGeneralSettingsChangeType.TableStyleRowBandSize, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(TableGeneralSettingsChangeType.IsTableOverlap, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(TableGeneralSettingsChangeType.BackgroundColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(TableGeneralSettingsChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableGeneralSettingsChangeType.AvoidDoubleBorders, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(TableGeneralSettingsChangeType change) {
			return tableGeneralSettingsChangeActionsTable[change];
		}
	}
	#endregion
	public enum TableChangeType {
		None = 0,
		BatchUpdate,
		TableStyle
	}
	public static class TableChangeActionCalculator {
		internal class TableChangeActionsTable : Dictionary<TableChangeType, DocumentModelChangeActions> {
		}
		internal static readonly TableChangeActionsTable tableChangeActionsTable = CreateTableChangeActionsTable();
		internal static TableChangeActionsTable CreateTableChangeActionsTable() {
			TableChangeActionsTable table = new TableChangeActionsTable();
			table.Add(TableChangeType.None, DocumentModelChangeActions.None);
			table.Add(TableChangeType.TableStyle, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TableChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(TableChangeType change) {
			return tableChangeActionsTable[change];
		}
	}
	#region PropertiesBase<T>
	public abstract class PropertiesBase<T> : RichEditIndexBasedObject<T>, IPropertiesContainer
		where T : ICloneable<T>, ISupportsCopyFrom<T>, ISupportsSizeOf {
		protected PropertiesBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected abstract PropertiesDictionary<BoolPropertyAccessor<T>> AccessorTable { get; }
		#region IPropertiesContainer<Properties> Members
		int suspendCount;
		DocumentModelChangeActions deferredChanges;
		protected bool IsSuspendUpdateOptions { get { return suspendCount > 0; } }
		void IPropertiesContainer.BeginPropertiesUpdate() {
			suspendCount++;
			if (suspendCount > 1)
				return;
			DocumentModel.History.BeginTransaction();
			BeginUpdate();
			deferredChanges = DocumentModelChangeActions.None;
		}
		void IPropertiesContainer.EndPropertiesUpdate() {
			if (IsSuspendUpdateOptions) {
				suspendCount--;
				if (suspendCount > 0)
					return;
			}
			if(deferredChanges != DocumentModelChangeActions.None)
				base.ApplyChanges(deferredChanges);
			deferredChanges = DocumentModelChangeActions.None;
			EndUpdate();
			DocumentHistory history = DocumentModel.History;
			Debug.Assert(history.Transaction != null);
			history.EndTransaction();
		}
		void IPropertiesContainer.BeginChanging(Properties changedProperty) {
			T options = ChangeOptionsCore(changedProperty);
			if (!IsSuspendUpdateOptions) {
				DocumentHistory history = DocumentModel.History;
				if(history != null)
					history.BeginTransaction();
			}
			if (options != null)
				ReplaceInfo(options, GetBatchUpdateChangeActions());
		}
		void IPropertiesContainer.ResetPropertyUse(Properties changedProperty) {
			T options = ResetOptionsCore(changedProperty);
			ReplaceInfo(options, GetBatchUpdateChangeActions());
		}
		void IPropertiesContainer.ApplyChanges(DocumentModelChangeActions changeActions) {
			this.ApplyChanges(changeActions);
		}
		void IPropertiesContainer.EndChanging() {
			if (!IsSuspendUpdateOptions) {
				DocumentHistory history = DocumentModel.History;
				if (history != null && history.Transaction != null)
					history.EndTransaction();
			}
		}
		#endregion
		protected virtual T ResetOptionsCore(Properties changedProperty) {
			BoolPropertyAccessor<T> accessor;
			if (AccessorTable.TryGetValue(changedProperty, out accessor))
				return ResetPropertiesOptions(accessor);
			else {
				Exceptions.ThrowInternalException();
				return default(T);
			}
		}
		protected virtual T ChangeOptionsCore(Properties changedProperty) {
			BoolPropertyAccessor<T> accessor;
			if (AccessorTable.TryGetValue(changedProperty, out accessor))
				return ChangePropertiesOptions(accessor);
			else {
				Exceptions.ThrowInternalException();
				return default(T);
			}
		}
		protected internal virtual T ChangePropertiesOptions(BoolPropertyAccessor<T> accessor) {
			T options = GetInfoForModification();
			if (!accessor.Get(options)) {
				accessor.Set(options, true);
				return options;
			}
			else
				return default(T);
		}
		protected internal virtual T ResetPropertiesOptions(BoolPropertyAccessor<T> accessor) {
			T options = GetInfoForModification();
			if (accessor.Get(options)) {
				accessor.Set(options, false);
				return options;
			}
			else
				return default(T);
		}
		void IPropertiesContainer.RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs e) {
			this.RaiseObtainAffectedRange(e);
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			if (suspendCount > 0) {
				deferredChanges |= changeActions;
			}
			else
				base.ApplyChanges(changeActions);
		}
	}
	#endregion
	public delegate bool GetOptionValueDelegate<T>(T options);
	public delegate void SetOptionValueDelegate<T>(T options, bool value);
	public class BoolPropertyAccessor<T> {
		readonly GetOptionValueDelegate<T> get;
		readonly SetOptionValueDelegate<T> set;
		public BoolPropertyAccessor(GetOptionValueDelegate<T> get, SetOptionValueDelegate<T> set) {
			this.get = get;
			this.set = set;
		}
		public GetOptionValueDelegate<T> Get { get { return get; } }
		public SetOptionValueDelegate<T> Set { get { return set; } }
	}
	#region TablePropertiesOptions
	public class TablePropertiesOptions : ICloneable<TablePropertiesOptions>, ISupportsCopyFrom<TablePropertiesOptions>, ISupportsSizeOf {
		#region Mask
		public enum Mask {
			UseNone = 0x00000000,
			UseLeftMargin = 0x00000001,
			UseRightMargin = 0x00000002,
			UseTopMargin = 0x00000004,
			UseBottomMargin = 0x00000008,
			UseCellSpacing = 0x00000010,
			UseTableIndent = 0x00000020,
			UseTableLayout = 0x00000040,
			UseTableLook = 0x00000080,
			UsePreferredWidth = 0x00000100,
			UseTableStyleColBandSize = 0x00000200,
			UseTableStyleRowBandSize = 0x00000400,
			UseIsTableOverlap = 0x00000800,
			UseFloatingPosition = 0x00001000,
			UseLeftBorder = 0x00002000,
			UseRightBorder = 0x00004000,
			UseTopBorder = 0x00008000,
			UseBottomBorder = 0x00010000,
			UseInsideHorizontalBorder = 0x00020000,
			UseInsideVerticalBorder = 0x00040000,
			UseBackgroundColor = 0x00080000,
			UseTableAlignment = 0x00100000,
			UseBorders = 0x0007E000,
			UseAvoidDoubleBorders = 0x00200000,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val;
		public TablePropertiesOptions() {
			val = Mask.UseNone;
		}
		internal TablePropertiesOptions(Mask val) {
			this.val = val;
		}
		#region Properties
		public bool UseLeftMargin { get { return GetVal(Mask.UseLeftMargin); } set { SetVal(Mask.UseLeftMargin, value); } }
		public bool UseRightMargin { get { return GetVal(Mask.UseRightMargin); } set { SetVal(Mask.UseRightMargin, value); } }
		public bool UseTopMargin { get { return GetVal(Mask.UseTopMargin); } set { SetVal(Mask.UseTopMargin, value); } }
		public bool UseBottomMargin { get { return GetVal(Mask.UseBottomMargin); } set { SetVal(Mask.UseBottomMargin, value); } }
		public bool UseCellSpacing { get { return GetVal(Mask.UseCellSpacing); } set { SetVal(Mask.UseCellSpacing, value); } }
		public bool UseTableIndent { get { return GetVal(Mask.UseTableIndent); } set { SetVal(Mask.UseTableIndent, value); } }
		public bool UseTableLayout { get { return GetVal(Mask.UseTableLayout); } set { SetVal(Mask.UseTableLayout, value); } }
		public bool UseTableAlignment { get { return GetVal(Mask.UseTableAlignment); } set { SetVal(Mask.UseTableAlignment, value); } }
		public bool UseTableLook { get { return GetVal(Mask.UseTableLook); } set { SetVal(Mask.UseTableLook, value); } }
		public bool UsePreferredWidth { get { return GetVal(Mask.UsePreferredWidth); } set { SetVal(Mask.UsePreferredWidth, value); } }
		public bool UseTableStyleColBandSize { get { return GetVal(Mask.UseTableStyleColBandSize); } set { SetVal(Mask.UseTableStyleColBandSize, value); } }
		public bool UseTableStyleRowBandSize { get { return GetVal(Mask.UseTableStyleRowBandSize); } set { SetVal(Mask.UseTableStyleRowBandSize, value); } }
		public bool UseIsTableOverlap { get { return GetVal(Mask.UseIsTableOverlap); } set { SetVal(Mask.UseIsTableOverlap, value); } }
		public bool UseFloatingPosition { get { return GetVal(Mask.UseFloatingPosition); } set { SetVal(Mask.UseFloatingPosition, value); } }
		public bool UseBackgroundColor { get { return GetVal(Mask.UseBackgroundColor); } set { SetVal(Mask.UseBackgroundColor, value); } }
		public bool UseLeftBorder { get { return GetVal(Mask.UseLeftBorder); } set { SetVal(Mask.UseLeftBorder, value); } }
		public bool UseRightBorder { get { return GetVal(Mask.UseRightBorder); } set { SetVal(Mask.UseRightBorder, value); } }
		public bool UseTopBorder { get { return GetVal(Mask.UseTopBorder); } set { SetVal(Mask.UseTopBorder, value); } }
		public bool UseBottomBorder { get { return GetVal(Mask.UseBottomBorder); } set { SetVal(Mask.UseBottomBorder, value); } }
		public bool UseInsideHorizontalBorder { get { return GetVal(Mask.UseInsideHorizontalBorder); } set { SetVal(Mask.UseInsideHorizontalBorder, value); } }
		public bool UseInsideVerticalBorder { get { return GetVal(Mask.UseInsideVerticalBorder); } set { SetVal(Mask.UseInsideVerticalBorder, value); } }
		public bool UseBorders { get { return GetVal(Mask.UseBorders); } set { SetVal(Mask.UseBorders, value); } }
		public bool UseAvoidDoubleBorders { get { return GetVal(Mask.UseAvoidDoubleBorders); } set { SetVal(Mask.UseAvoidDoubleBorders, value); } }
		internal Mask Value { get { return val; } set { val = value; } }
		#endregion
		public TablePropertiesOptions Clone() {
			TablePropertiesOptions clone = new TablePropertiesOptions();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(TablePropertiesOptions options) {
			this.val = options.val;
		}
		public override bool Equals(object obj) {
			TablePropertiesOptions opts = obj as TablePropertiesOptions;
			if (opts == null)
				return false;
			return Value == opts.Value;
		}
		public override int GetHashCode() {
			return (int)Value;
		}
		#region GetVal/SetVal helpers
		public bool GetVal(Mask mask) {
			return (val & mask) != 0;
		}
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				val |= mask;
			else
				val &= ~mask;
		}
		#endregion
		#region Options accessros
		internal static bool GetOptionsUseLeftBorder(TablePropertiesOptions options) { return options.UseLeftBorder; }
		internal static void SetOptionsUseLeftBorder(TablePropertiesOptions options, bool value) { options.UseLeftBorder = value; }
		internal static bool GetOptionsUseRightBorder(TablePropertiesOptions options) { return options.UseRightBorder; }
		internal static void SetOptionsUseRightBorder(TablePropertiesOptions options, bool value) { options.UseRightBorder = value; }
		internal static bool GetOptionsUseTopBorder(TablePropertiesOptions options) { return options.UseTopBorder; }
		internal static void SetOptionsUseTopBorder(TablePropertiesOptions options, bool value) { options.UseTopBorder = value; }
		internal static bool GetOptionsUseBottomBorder(TablePropertiesOptions options) { return options.UseBottomBorder; }
		internal static void SetOptionsUseBottomBorder(TablePropertiesOptions options, bool value) { options.UseBottomBorder = value; }
		internal static bool GetOptionsUseInsideVerticalBorder(TablePropertiesOptions options) { return options.UseInsideVerticalBorder; }
		internal static void SetOptionsUseInsideVerticalBorder(TablePropertiesOptions options, bool value) { options.UseInsideVerticalBorder = value; }
		internal static bool GetOptionsUseInsideHorizontalBorder(TablePropertiesOptions options) { return options.UseInsideHorizontalBorder; }
		internal static void SetOptionsUseInsideHorizontalBorder(TablePropertiesOptions options, bool value) { options.UseInsideHorizontalBorder = value; }
		internal static bool GetOptionsUseLeftMargin(TablePropertiesOptions options) { return options.UseLeftMargin; }
		internal static void SetOptionsUseLeftMargin(TablePropertiesOptions options, bool value) { options.UseLeftMargin = value; }
		internal static bool GetOptionsUseRightMargin(TablePropertiesOptions options) { return options.UseRightMargin; }
		internal static void SetOptionsUseRightMargin(TablePropertiesOptions options, bool value) { options.UseRightMargin = value; }
		internal static bool GetOptionsUseTopMargin(TablePropertiesOptions options) { return options.UseTopMargin; }
		internal static void SetOptionsUseTopMargin(TablePropertiesOptions options, bool value) { options.UseTopMargin = value; }
		internal static bool GetOptionsUseBottomMargin(TablePropertiesOptions options) { return options.UseBottomMargin; }
		internal static void SetOptionsUseBottomMargin(TablePropertiesOptions options, bool value) { options.UseBottomMargin = value; }
		internal static bool GetOptionsUseCellSpacing(TablePropertiesOptions options) { return options.UseCellSpacing; }
		internal static void SetOptionsUseCellSpacing(TablePropertiesOptions options, bool value) { options.UseCellSpacing = value; }
		internal static bool GetOptionsUsePreferredWidth(TablePropertiesOptions options) { return options.UsePreferredWidth; }
		internal static void SetOptionsUsePreferredWidth(TablePropertiesOptions options, bool value) { options.UsePreferredWidth = value; }
		internal static bool GetOptionsUseTableIndent(TablePropertiesOptions options) { return options.UseTableIndent; }
		internal static void SetOptionsUseTableIndent(TablePropertiesOptions options, bool value) { options.UseTableIndent = value; }
		internal static bool GetOptionsUseTableLayout(TablePropertiesOptions options) { return options.UseTableLayout; }
		internal static void SetOptionsUseTableLayout(TablePropertiesOptions options, bool value) { options.UseTableLayout = value; }
		internal static bool GetOptionsUseTableAlignment(TablePropertiesOptions options) { return options.UseTableAlignment; }
		internal static void SetOptionsUseTableAlignment(TablePropertiesOptions options, bool value) { options.UseTableAlignment = value; }
		internal static bool GetOptionsUseTableLook(TablePropertiesOptions options) { return options.UseTableLook; }
		internal static void SetOptionsUseTableLook(TablePropertiesOptions options, bool value) { options.UseTableLook = value; }
		internal static bool GetOptionsUseTableOverlap(TablePropertiesOptions options) { return options.UseIsTableOverlap; }
		internal static void SetOptionsUseTableOverlap(TablePropertiesOptions options, bool value) { options.UseIsTableOverlap = value; }
		internal static bool GetOptionsUseTableStyleColBandSize(TablePropertiesOptions options) { return options.UseTableStyleColBandSize; }
		internal static void SetOptionsUseTableStyleColBandSize(TablePropertiesOptions options, bool value) { options.UseTableStyleColBandSize = value; }
		internal static bool GetOptionsUseTableStyleRowBandSize(TablePropertiesOptions options) { return options.UseTableStyleRowBandSize; }
		internal static void SetOptionsUseTableStyleRowBandSize(TablePropertiesOptions options, bool value) { options.UseTableStyleRowBandSize = value; }
		internal static bool GetOptionsUseFloatingPosition(TablePropertiesOptions options) { return options.UseFloatingPosition; }
		internal static void SetOptionsUseFloatingPosition(TablePropertiesOptions options, bool value) { options.UseFloatingPosition = value; }
		internal static bool GetOptionsUseBackgroundColor(TablePropertiesOptions options) { return options.UseBackgroundColor; }
		internal static void SetOptionsUseBackgroundColor(TablePropertiesOptions options, bool value) { options.UseBackgroundColor = value; }
		internal static bool GetOptionsUseAvoidDoubleBorders(TablePropertiesOptions options) { return options.UseAvoidDoubleBorders; }
		internal static void SetOptionsUseAvoidDoubleBorders(TablePropertiesOptions options, bool value) { options.UseAvoidDoubleBorders = value; }
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TablePropertiesOptionsCache
	public class TablePropertiesOptionsCache : UniqueItemsCache<TablePropertiesOptions> {
		internal const int EmptyTableFormattingOptionsItem = 0;
		internal const int RootTableFormattingOptionsItem = 1;
		public TablePropertiesOptionsCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			AddRootStyleOptions();
		}
		protected override TablePropertiesOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new TablePropertiesOptions();
		}
		void AddRootStyleOptions() {
			AppendItem(new TablePropertiesOptions(TablePropertiesOptions.Mask.UseAll));
		}
	}
	#endregion
	#region TableProperties
	public class TableProperties : PropertiesBase<TablePropertiesOptions>, ITablePropertiesContainer, ICellMarginsContainer {
		public class TablePropertiesAccessorTable : PropertiesDictionary<BoolPropertyAccessor<TablePropertiesOptions>> {
		}
		#region Fields
		static readonly TablePropertiesAccessorTable accessorTable = CreateAccessorTable();
		readonly CellMargins cellMargins;
		readonly CellSpacing cellSpacing;
		readonly TableIndent indent;
		readonly PreferredWidth preferredWidth;
		readonly TableGeneralSettings generalSettings;
		readonly TableBorders borders;
		readonly TableFloatingPosition floatingPosition;
		#endregion
		public TableProperties(PieceTable pieceTable)
			: base(pieceTable) {
			this.cellMargins = new CellMargins(pieceTable, this);
			this.cellSpacing = new CellSpacing(pieceTable, this);
			this.indent = new TableIndent(pieceTable, this);
			this.preferredWidth = new PreferredWidth(pieceTable, this);
			this.generalSettings = new TableGeneralSettings(pieceTable, this);
			this.borders = new TableBorders(pieceTable, this);
			this.floatingPosition = new TableFloatingPosition(pieceTable, this);
		}
		#region Properties
		public CellMargins CellMargins { get { return cellMargins; } }
		public bool UseLeftMargin { get { return Info.UseLeftMargin; } }
		public bool UseRightMargin { get { return Info.UseRightMargin; } }
		public bool UseTopMargin { get { return Info.UseTopMargin; } }
		public bool UseBottomMargin { get { return Info.UseBottomMargin; } }
		public CellSpacing CellSpacing { get { return cellSpacing; } }
		public bool UseCellSpacing { get { return Info.UseCellSpacing; } }
		public TableIndent TableIndent { get { return indent; } }
		public bool UseTableIndent { get { return Info.UseTableIndent; } }
		public PreferredWidth PreferredWidth { get { return preferredWidth; } }
		public bool UsePreferredWidth { get { return Info.UsePreferredWidth; } }
		public TableBorders Borders { get { return borders; } }
		public TableLayoutType TableLayout { get { return GeneralSettings.TableLayout; } set { GeneralSettings.TableLayout = value; } }
		public bool UseTableLayout { get { return Info.UseTableLayout; } }
		public TableRowAlignment TableAlignment { get { return GeneralSettings.TableAlignment; } set { GeneralSettings.TableAlignment = value; } }
		public bool UseTableAlignment { get { return Info.UseTableAlignment; } }
		public TableLookTypes TableLook { get { return GeneralSettings.TableLook; } set { GeneralSettings.TableLook = value; } }
		public bool UseTableLook { get { return Info.UseTableLook; } }
		public int TableStyleColBandSize { get { return GeneralSettings.TableStyleColumnBandSize; } set { GeneralSettings.TableStyleColumnBandSize = value; } }
		public bool UseTableStyleColBandSize { get { return Info.UseTableStyleColBandSize; } }
		public int TableStyleRowBandSize { get { return GeneralSettings.TableStyleRowBandSize; } set { GeneralSettings.TableStyleRowBandSize = value; } }
		public bool UseTableStyleRowBandSize { get { return Info.UseTableStyleRowBandSize; } }
		public bool IsTableOverlap { get { return GeneralSettings.IsTableOverlap; } set { GeneralSettings.IsTableOverlap = value; } }
		public bool UseIsTableOverlap { get { return Info.UseIsTableOverlap; } }
		public TableFloatingPosition FloatingPosition { get { return floatingPosition; } }
		public bool UseFloatingPosition { get { return Info.UseFloatingPosition; } }
		public Color BackgroundColor { get { return GeneralSettings.BackgroundColor; } set { GeneralSettings.BackgroundColor = value; } }
#if THEMES_EDIT
		public Shading Shading { get { return GeneralSettings.Shading; } set { GeneralSettings.Shading = value; } }
#endif
		public bool UseBackgroundColor { get { return Info.UseBackgroundColor; } }
		public bool AvoidDoubleBorders { get { return GeneralSettings.AvoidDoubleBorders; } set { GeneralSettings.AvoidDoubleBorders = value; } }
		public bool UseAvoidDoubleBorders { get { return Info.UseAvoidDoubleBorders; } }
		public TablePropertiesOptions.Mask UseValue { get { return Info.Value; } set { Info.Value = value; } }
		internal TableGeneralSettings GeneralSettings { get { return generalSettings; } }
		protected override PropertiesDictionary<BoolPropertyAccessor<TablePropertiesOptions>> AccessorTable { get { return accessorTable; } }
		#endregion
		protected internal override UniqueItemsCache<TablePropertiesOptions> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TablePropertiesOptionsCache;
		}
		public virtual void CopyFrom(TableProperties newProperties) {
			IPropertiesContainer container = (IPropertiesContainer)this;
			DocumentModel.BeginUpdate();
			container.BeginPropertiesUpdate();
			try {
				if (Object.ReferenceEquals(this.DocumentModel, newProperties.DocumentModel)) {
					Borders.CopyFrom(newProperties.Borders);
					GeneralSettings.CopyFrom(newProperties.GeneralSettings);
					CellMargins.CopyFrom(newProperties.CellMargins);
					CellSpacing.CopyFrom(newProperties.CellSpacing);
					TableIndent.CopyFrom(newProperties.TableIndent);
					PreferredWidth.CopyFrom(newProperties.PreferredWidth);
					FloatingPosition.CopyFrom(newProperties.FloatingPosition);
					TablePropertiesOptions options = this.GetInfoForModification();
					options.CopyFrom(newProperties.Info);
					this.ReplaceInfo(options, DocumentModelChangeActions.None);
				}
				else {
					Borders.CopyFrom(newProperties.Borders);
					GeneralSettings.CopyFrom(newProperties.GeneralSettings.Info);
					CellMargins.CopyFrom(newProperties.CellMargins);
					CellSpacing.CopyFrom(newProperties.CellSpacing.Info);
					TableIndent.CopyFrom(newProperties.TableIndent.Info);
					PreferredWidth.CopyFrom(newProperties.PreferredWidth.Info);
					FloatingPosition.CopyFrom(newProperties.FloatingPosition.Info);
					this.Info.CopyFrom(newProperties.Info);
				}
			}
			finally {
				container.EndPropertiesUpdate();
				DocumentModel.EndUpdate();
			}
		}
		internal void ResetUse(TablePropertiesOptions.Mask mask) {
			TablePropertiesOptions newOptions = new TablePropertiesOptions(this.Info.Value & (~mask));
			ReplaceInfo(newOptions, GetBatchUpdateChangeActions());
		}
		public bool GetUse(TablePropertiesOptions.Mask mask) {
			return Info.GetVal(mask);
		}
		public void Reset() {
			DocumentModel.History.BeginTransaction();
			try {
				CopyFrom(DocumentModel.DefaultTableProperties);
				ResetAllUse();
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		internal void ResetAllUse() {
			ReplaceInfo(GetCache(DocumentModel)[TablePropertiesOptionsCache.EmptyTableFormattingOptionsItem], GetBatchUpdateChangeActions());
		}
		internal virtual void Merge(TableProperties properties) {
			IPropertiesContainer container = this as IPropertiesContainer;
			container.BeginPropertiesUpdate();
			try {
				Borders.Merge(properties.Borders);
				CellMargins.Merge(properties.CellMargins);
				if (!UseCellSpacing && properties.UseCellSpacing)
					CellSpacing.CopyFrom(properties.CellSpacing);
				if (!UseIsTableOverlap && properties.UseIsTableOverlap)
					IsTableOverlap = properties.IsTableOverlap;
				if (!UsePreferredWidth && properties.UsePreferredWidth)
					PreferredWidth.CopyFrom(properties.PreferredWidth);
				if (!UseTableIndent && properties.UseTableIndent)
					TableIndent.CopyFrom(properties.TableIndent);
				if (!UseTableLayout && properties.UseTableLayout)
					TableLayout = properties.TableLayout;
				if (!UseTableLook && properties.UseTableLook)
					TableLook = properties.TableLook;
				if (!UseTableStyleColBandSize && properties.UseTableStyleColBandSize)
					TableStyleColBandSize = properties.TableStyleColBandSize;
				if (!UseTableStyleRowBandSize && properties.UseTableStyleRowBandSize)
					TableStyleRowBandSize = properties.TableStyleRowBandSize;
				if (!UseFloatingPosition && properties.UseFloatingPosition)
					FloatingPosition.CopyFrom(properties.FloatingPosition);
				if (!UseBackgroundColor && properties.UseBackgroundColor) {
					BackgroundColor = properties.BackgroundColor;
#if THEMES_EDIT
					Shading = properties.Shading;
#endif
				}
				if (!UseTableAlignment && properties.UseTableAlignment)
					TableAlignment = properties.TableAlignment;
			}
			finally {
				container.EndPropertiesUpdate();
			}
		}
		static TablePropertiesAccessorTable CreateAccessorTable() {
			TablePropertiesAccessorTable result = new TablePropertiesAccessorTable();
			result.Add(Properties.LeftBorder, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseLeftBorder, TablePropertiesOptions.SetOptionsUseLeftBorder));
			result.Add(Properties.RightBorder, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseRightBorder, TablePropertiesOptions.SetOptionsUseRightBorder));
			result.Add(Properties.TopBorder, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTopBorder, TablePropertiesOptions.SetOptionsUseTopBorder));
			result.Add(Properties.BottomBorder, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseBottomBorder, TablePropertiesOptions.SetOptionsUseBottomBorder));
			result.Add(Properties.InsideHorizontalBorder, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseInsideHorizontalBorder, TablePropertiesOptions.SetOptionsUseInsideHorizontalBorder));
			result.Add(Properties.InsideVerticalBorder, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseInsideVerticalBorder, TablePropertiesOptions.SetOptionsUseInsideVerticalBorder));
			result.Add(Properties.LeftMargin, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseLeftMargin, TablePropertiesOptions.SetOptionsUseLeftMargin));
			result.Add(Properties.RightMargin, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseRightMargin, TablePropertiesOptions.SetOptionsUseRightMargin));
			result.Add(Properties.TopMargin, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTopMargin, TablePropertiesOptions.SetOptionsUseTopMargin));
			result.Add(Properties.BottomMargin, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseBottomMargin, TablePropertiesOptions.SetOptionsUseBottomMargin));
			result.Add(Properties.CellSpacing, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseCellSpacing, TablePropertiesOptions.SetOptionsUseCellSpacing));
			result.Add(Properties.PreferredWidth, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUsePreferredWidth, TablePropertiesOptions.SetOptionsUsePreferredWidth));
			result.Add(Properties.TableIndent, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableIndent, TablePropertiesOptions.SetOptionsUseTableIndent));
			result.Add(Properties.TableLayout, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableLayout, TablePropertiesOptions.SetOptionsUseTableLayout));
			result.Add(Properties.TableAlignment, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableAlignment, TablePropertiesOptions.SetOptionsUseTableAlignment));
			result.Add(Properties.TableLook, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableLook, TablePropertiesOptions.SetOptionsUseTableLook));
			result.Add(Properties.IsTableOverlap, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableOverlap, TablePropertiesOptions.SetOptionsUseTableOverlap));
			result.Add(Properties.TableStyleColumnBandSize, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableStyleColBandSize, TablePropertiesOptions.SetOptionsUseTableStyleColBandSize));
			result.Add(Properties.TableStyleRowBandSize, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseTableStyleRowBandSize, TablePropertiesOptions.SetOptionsUseTableStyleRowBandSize));
			result.Add(Properties.TableFloatingPosition, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseFloatingPosition, TablePropertiesOptions.SetOptionsUseFloatingPosition));
			result.Add(Properties.BackgroundColor, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseBackgroundColor, TablePropertiesOptions.SetOptionsUseBackgroundColor));
			result.Add(Properties.AvoidDoubleBorders, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseAvoidDoubleBorders, TablePropertiesOptions.SetOptionsUseAvoidDoubleBorders));
			result.Add(Properties.ShadingPattern, new BoolPropertyAccessor<TablePropertiesOptions>(TablePropertiesOptions.GetOptionsUseBackgroundColor, TablePropertiesOptions.SetOptionsUseBackgroundColor));
			return result;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
	}
	#endregion
	#region CombinedTablePropertiesInfo
	public class CombinedTablePropertiesInfo : ICloneable<CombinedTablePropertiesInfo> {
		readonly CombinedCellMarginsInfo cellMargins;
		readonly CombinedTableBordersInfo borders;
		readonly WidthUnitInfo cellSpacing;
		readonly WidthUnitInfo indent;
		readonly WidthUnitInfo preferredWidth;
		readonly TableGeneralSettingsInfo generalSettings;
		readonly TableFloatingPositionInfo floatingPosition;
		public CombinedTablePropertiesInfo(TableProperties tableProperties) {
			this.cellMargins = new CombinedCellMarginsInfo(tableProperties.CellMargins);
			this.borders = new CombinedTableBordersInfo(tableProperties.Borders);
			this.cellSpacing = tableProperties.CellSpacing.Info;
			this.indent = tableProperties.TableIndent.Info;
			this.preferredWidth = tableProperties.PreferredWidth.Info;
			this.generalSettings = tableProperties.GeneralSettings.Info;
			this.floatingPosition = tableProperties.FloatingPosition.Info;
		}
		internal CombinedTablePropertiesInfo() {
			this.cellMargins = new CombinedCellMarginsInfo();
			this.borders = new CombinedTableBordersInfo();
			this.cellSpacing = new WidthUnitInfo();
			this.indent = new WidthUnitInfo();
			this.preferredWidth = new WidthUnitInfo();
			this.generalSettings = new TableGeneralSettingsInfo();
			this.floatingPosition = new TableFloatingPositionInfo();
		}
		public CombinedCellMarginsInfo CellMargins { get { return cellMargins; } }
		public CombinedTableBordersInfo Borders { get { return borders; } }
		public WidthUnitInfo CellSpacing { get { return cellSpacing; } }
		public WidthUnitInfo TableIndent { get { return indent; } }
		public WidthUnitInfo PreferredWidth { get { return preferredWidth; } }
		public TableGeneralSettingsInfo GeneralSettings { get { return generalSettings; } }
		public TableFloatingPositionInfo FloatingPosition { get { return floatingPosition; } }
		public void CopyFrom(CombinedTablePropertiesInfo info) {
			CellMargins.CopyFrom(info.CellMargins);
			Borders.CopyFrom(info.Borders);
			CellSpacing.CopyFrom(info.CellSpacing);
			TableIndent.CopyFrom(info.TableIndent);
			PreferredWidth.CopyFrom(info.PreferredWidth);
			GeneralSettings.CopyFrom(info.GeneralSettings);
			FloatingPosition.CopyFrom(info.FloatingPosition);
		}
		public CombinedTablePropertiesInfo Clone() {
			CombinedTablePropertiesInfo clone = new CombinedTablePropertiesInfo();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region CombinedCellMarginsInfo
	public class CombinedCellMarginsInfo : ICloneable<CombinedCellMarginsInfo> {
		readonly WidthUnitInfo top;
		readonly WidthUnitInfo left;
		readonly WidthUnitInfo right;
		readonly WidthUnitInfo bottom;
		public CombinedCellMarginsInfo(CellMargins cellMargins) {
			this.left = cellMargins.Left.Info;
			this.top = cellMargins.Top.Info;
			this.right = cellMargins.Right.Info;
			this.bottom = cellMargins.Bottom.Info;
		}
		internal CombinedCellMarginsInfo() {
			this.left = new WidthUnitInfo();
			this.top = new WidthUnitInfo();
			this.right = new WidthUnitInfo();
			this.bottom = new WidthUnitInfo();
		}
		public WidthUnitInfo Top { get { return top; } }
		public WidthUnitInfo Left { get { return left; } }
		public WidthUnitInfo Right { get { return right; } }
		public WidthUnitInfo Bottom { get { return bottom; } }
		public void CopyFrom(CombinedCellMarginsInfo info) {
			this.left.CopyFrom(info.Left);
			this.top.CopyFrom(info.Top);
			this.right.CopyFrom(info.Right);
			this.bottom.CopyFrom(info.Bottom);
		}
		public CombinedCellMarginsInfo Clone() {
			CombinedCellMarginsInfo clone = new CombinedCellMarginsInfo();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region CombinedTableBordersInfo
	public class CombinedTableBordersInfo : ICloneable<CombinedTableBordersInfo>, ISupportsCopyFrom<CombinedTableBordersInfo> {
		readonly BorderInfo bottomBorder;
		readonly BorderInfo leftBorder;
		readonly BorderInfo rightBorder;
		readonly BorderInfo topBorder;
		readonly BorderInfo insideHorizontalBorder;
		readonly BorderInfo insideVerticalBorder;
		public CombinedTableBordersInfo(TableBorders tableBorders) {
			this.bottomBorder = tableBorders.BottomBorder.Info;
			this.leftBorder = tableBorders.LeftBorder.Info;
			this.rightBorder = tableBorders.RightBorder.Info;
			this.topBorder = tableBorders.TopBorder.Info;
			this.insideHorizontalBorder = tableBorders.InsideHorizontalBorder.Info;
			this.insideVerticalBorder = tableBorders.InsideVerticalBorder.Info;
		}
		internal CombinedTableBordersInfo() {
			this.bottomBorder = new BorderInfo();
			this.leftBorder = new BorderInfo();
			this.rightBorder = new BorderInfo();
			this.topBorder = new BorderInfo();
			this.insideHorizontalBorder = new BorderInfo();
			this.insideVerticalBorder = new BorderInfo();
		}
		public BorderInfo BottomBorder { get { return bottomBorder; } }
		public BorderInfo LeftBorder { get { return leftBorder; } }
		public BorderInfo RightBorder { get { return rightBorder; } }
		public BorderInfo TopBorder { get { return topBorder; } }
		public BorderInfo InsideHorizontalBorder { get { return insideHorizontalBorder; } }
		public BorderInfo InsideVerticalBorder { get { return insideVerticalBorder; } }
		public void CopyFrom(CombinedTableBordersInfo info) {
			this.bottomBorder.CopyFrom(info.BottomBorder);
			this.leftBorder.CopyFrom(info.LeftBorder);
			this.rightBorder.CopyFrom(info.RightBorder);
			this.topBorder.CopyFrom(info.TopBorder);
			this.insideHorizontalBorder.CopyFrom(info.InsideHorizontalBorder);
			this.insideVerticalBorder.CopyFrom(info.InsideVerticalBorder);
		}
		public CombinedTableBordersInfo Clone() {
			CombinedTableBordersInfo clone = new CombinedTableBordersInfo();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region MergedTableProperties
	public class MergedTableProperties : MergedProperties<CombinedTablePropertiesInfo, TablePropertiesOptions> {
		public MergedTableProperties(CombinedTablePropertiesInfo info, TablePropertiesOptions options)
			: base(info, options) {
		}
	}
	#endregion
	#region TablePropertiesMerger
	public class TablePropertiesMerger : PropertiesMergerBase<CombinedTablePropertiesInfo, TablePropertiesOptions, MergedTableProperties> {
		public TablePropertiesMerger(TableProperties initialProperties)
			: base(new MergedTableProperties(new CombinedTablePropertiesInfo(initialProperties), initialProperties.Info)) {
		}
		public TablePropertiesMerger(MergedTableProperties initialProperties)
			: base(new MergedTableProperties(initialProperties.Info, initialProperties.Options)) {
		}
		public void Merge(TableProperties properties) {
			MergeCore(new CombinedTablePropertiesInfo(properties), properties.Info);
		}
		protected internal override void MergeCore(CombinedTablePropertiesInfo info, TablePropertiesOptions options) {
			if (!OwnOptions.UseBottomBorder && options.UseBottomBorder) {
				OwnInfo.Borders.BottomBorder.CopyFrom(info.Borders.BottomBorder);
				OwnOptions.UseBottomBorder = true;
			}
			if (!OwnOptions.UseTopBorder && options.UseTopBorder) {
				OwnInfo.Borders.TopBorder.CopyFrom(info.Borders.TopBorder);
				OwnOptions.UseTopBorder = true;
			}
			if (!OwnOptions.UseLeftBorder && options.UseLeftBorder) {
				OwnInfo.Borders.LeftBorder.CopyFrom(info.Borders.LeftBorder);
				OwnOptions.UseLeftBorder = true;
			}
			if (!OwnOptions.UseRightBorder && options.UseRightBorder) {
				OwnInfo.Borders.RightBorder.CopyFrom(info.Borders.RightBorder);
				OwnOptions.UseRightBorder = true;
			}
			if (!OwnOptions.UseInsideHorizontalBorder && options.UseInsideHorizontalBorder) {
				OwnInfo.Borders.InsideHorizontalBorder.CopyFrom(info.Borders.InsideHorizontalBorder);
				OwnOptions.UseInsideHorizontalBorder = true;
			}
			if (!OwnOptions.UseInsideVerticalBorder && options.UseInsideVerticalBorder) {
				OwnInfo.Borders.InsideVerticalBorder.CopyFrom(info.Borders.InsideVerticalBorder);
				OwnOptions.UseInsideVerticalBorder = true;
			}
			if (!OwnOptions.UseLeftMargin && options.UseLeftMargin) {
				OwnInfo.CellMargins.Left.CopyFrom(info.CellMargins.Left);
				OwnOptions.UseLeftMargin = true;
			}
			if (!OwnOptions.UseRightMargin && options.UseRightMargin) {
				OwnInfo.CellMargins.Right.CopyFrom(info.CellMargins.Right);
				OwnOptions.UseRightMargin = true;
			}
			if (!OwnOptions.UseTopMargin && options.UseTopMargin) {
				OwnInfo.CellMargins.Top.CopyFrom(info.CellMargins.Top);
				OwnOptions.UseTopMargin = true;
			}
			if (!OwnOptions.UseBottomMargin && options.UseBottomMargin) {
				OwnInfo.CellMargins.Bottom.CopyFrom(info.CellMargins.Bottom);
				OwnOptions.UseBottomMargin = true;
			}
			if (!OwnOptions.UseCellSpacing && options.UseCellSpacing) {
				OwnInfo.CellSpacing.CopyFrom(info.CellSpacing);
				OwnOptions.UseCellSpacing = true;
			}
			if (!OwnOptions.UseFloatingPosition && options.UseFloatingPosition) {
				OwnInfo.FloatingPosition.CopyFrom(info.FloatingPosition);
				OwnOptions.UseFloatingPosition = true;
			}
			if (!OwnOptions.UseIsTableOverlap && options.UseIsTableOverlap) {
				OwnInfo.GeneralSettings.IsTableOverlap = info.GeneralSettings.IsTableOverlap;
				OwnOptions.UseIsTableOverlap = true;
			}
			if (!OwnOptions.UseTableLayout && options.UseTableLayout) {
				OwnInfo.GeneralSettings.TableLayout = info.GeneralSettings.TableLayout;
				OwnOptions.UseTableLayout = true;
			}
			if (!OwnOptions.UseTableLook && options.UseTableLook) {
				OwnInfo.GeneralSettings.TableLook = info.GeneralSettings.TableLook;
				OwnOptions.UseTableLook = true;
			}
			if (!OwnOptions.UseTableStyleColBandSize && options.UseTableStyleColBandSize) {
				OwnInfo.GeneralSettings.TableStyleColBandSize = info.GeneralSettings.TableStyleColBandSize;
				OwnOptions.UseTableStyleColBandSize = true;
			}
			if (!OwnOptions.UseTableStyleRowBandSize && options.UseTableStyleRowBandSize) {
				OwnInfo.GeneralSettings.TableStyleRowBandSize = info.GeneralSettings.TableStyleRowBandSize;
				OwnOptions.UseTableStyleRowBandSize = true;
			}
			if (!OwnOptions.UseTableIndent && options.UseTableIndent) {
				OwnInfo.TableIndent.CopyFrom(info.TableIndent);
				OwnOptions.UseTableIndent = true;
			}
			if (!OwnOptions.UsePreferredWidth && options.UsePreferredWidth) {
				OwnInfo.PreferredWidth.CopyFrom(info.PreferredWidth);
				OwnOptions.UsePreferredWidth = true;
			}
		}
	}
	#endregion
}
