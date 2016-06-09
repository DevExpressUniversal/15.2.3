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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.ViewInfo;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraVerticalGrid {
	public class VGridAppearanceCollection : BaseAppearanceCollection {
		AppearanceObject rowHeaderPanel, pressedRow, hideSelectionRow, category, horzLine, vertLine,
			recordValue, bandBorder, focusedRow, focusedRecord, focusedCell,
			expandButton, categoryExpandButton, empty, disabledRecordValue, disabledRow,
			readOnlyRecordValue, readOnlyRow, modifiedRecordValue, modifiedRow, fixedLine;
		VGridControlBase grid;
		public VGridAppearanceCollection(VGridControlBase grid) {
			this.grid = grid;
			this.rowHeaderPanel = CreateAppearance(GridStyles.RowHeaderPanel);
			this.pressedRow = CreateAppearance(GridStyles.PressedRow);
			this.hideSelectionRow = CreateAppearance(GridStyles.HideSelectionRow);
			this.category = CreateAppearance(GridStyles.Category);
			this.horzLine = CreateAppearance(GridStyles.HorzLine);
			this.vertLine = CreateAppearance(GridStyles.VertLine);
			this.recordValue = CreateAppearance(GridStyles.RecordValue);
			this.bandBorder = CreateAppearance(GridStyles.BandBorder);
			this.focusedRow = CreateAppearance(GridStyles.FocusedRow);
			this.focusedRecord = CreateAppearance(GridStyles.FocusedRecord);
			this.focusedCell = CreateAppearance(GridStyles.FocusedCell);
			this.expandButton = CreateAppearance(GridStyles.ExpandButton);
			this.categoryExpandButton = CreateAppearance(GridStyles.CategoryExpandButton);
			this.empty = CreateAppearance(GridStyles.Empty);
			this.disabledRecordValue = CreateAppearance(GridStyles.DisabledRecordValue);
			this.disabledRow = CreateAppearance(GridStyles.DisabledRow);
			this.readOnlyRecordValue = CreateAppearance(GridStyles.ReadOnlyRecordValue);
			this.readOnlyRow = CreateAppearance(GridStyles.ReadOnlyRow);
			this.modifiedRecordValue = CreateAppearance(GridStyles.ModifiedRecordValue);
			this.modifiedRow = CreateAppearance(GridStyles.ModifiedRow);
			this.fixedLine = CreateAppearance(GridStyles.FixedLine);
		}
		void ResetRowHeaderPanel() { RowHeaderPanel.Reset(); }
		bool ShouldSerializeRowHeaderPanel() { return RowHeaderPanel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject RowHeaderPanel { get { return rowHeaderPanel; } }
		void ResetPressedRow() { PressedRow.Reset(); }
		bool ShouldSerializePressedRow() { return PressedRow.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject PressedRow { get { return pressedRow; } }
		void ResetHideSelectionRow() { HideSelectionRow.Reset(); }
		bool ShouldSerializeHideSelectionRow() { return HideSelectionRow.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HideSelectionRow { get { return hideSelectionRow; } }
		void ResetHideCategory() { Category.Reset(); }
		bool ShouldSerializeCategory() { return Category.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Category { get { return category; } }
		void ResetHorzLine() { HorzLine.Reset(); }
		bool ShouldSerializeHorzLine() { return HorzLine.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HorzLine { get { return horzLine; } }
		void ResetVertLine() { VertLine.Reset(); }
		bool ShouldSerializeVertLine() { return VertLine.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject VertLine { get { return vertLine; } }
		void ResetRecordValue() { RecordValue.Reset(); }
		bool ShouldSerializeRecordValue() { return RecordValue.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject RecordValue { get { return recordValue; } }
		void ResetBandBorder() { BandBorder.Reset(); }
		bool ShouldSerializeBandBorder() { return BandBorder.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BandBorder { get { return bandBorder; } }
		void ResetFixedLine() { FixedLine.Reset(); }
		bool ShouldSerializeFixedLine() { return FixedLine.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FixedLine { get { return fixedLine; } }
		void ResetFocusedRow() { FocusedRow.Reset(); }
		bool ShouldSerializeFocusedRow() { return FocusedRow.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedRow { get { return focusedRow; } }
		void ResetFocusedRecord() { FocusedRecord.Reset(); }
		bool ShouldSerializeFocusedRecord() { return FocusedRecord.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedRecord { get { return focusedRecord; } }
		void ResetFocusedCell() { FocusedCell.Reset(); }
		bool ShouldSerializeFocusedCell() { return FocusedCell.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FocusedCell { get { return focusedCell; } }
		void ResetExpandButton() { ExpandButton.Reset(); }
		bool ShouldSerializeExpandButton() { return ExpandButton.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ExpandButton { get { return expandButton; } }
		void ResetCategoryExpandButton() { CategoryExpandButton.Reset(); }
		bool ShouldSerializeCategoryExpandButton() { return CategoryExpandButton.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CategoryExpandButton { get { return categoryExpandButton; } }
		void ResetEmpty() { Empty.Reset(); }
		bool ShouldSerializeEmpty() { return Empty.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Empty { get { return empty; } }
		void ResetDisabledRecordValue() { DisabledRecordValue.Reset(); }
		bool ShouldSerializeDisabledRecordValue() { return DisabledRecordValue.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DisabledRecordValue { get { return disabledRecordValue; } }
		void ResetDisabledRow() { DisabledRow.Reset(); }
		bool ShouldSerializeDisabledRow() { return DisabledRow.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DisabledRow { get { return disabledRow; } }
		void ResetReadOnlyRecordValue() { ReadOnlyRecordValue.Reset(); }
		bool ShouldSerializeReadOnlyRecordValue() { return ReadOnlyRecordValue.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ReadOnlyRecordValue { get { return readOnlyRecordValue; } }
		void ResetReadOnlyRow() { ReadOnlyRow.Reset(); }
		bool ShouldSerializeReadOnlyRow() { return ReadOnlyRow.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ReadOnlyRow { get { return readOnlyRow; } }
		void ResetModifiedRecordValue() { ModifiedRecordValue.Reset(); }
		bool ShouldSerializeModifiedRecordValue() { return ModifiedRecordValue.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ModifiedRecordValue { get { return modifiedRecordValue; } }
		void ResetModifiedRow() { ModifiedRow.Reset(); }
		bool ShouldSerializeModifiedRow() { return ModifiedRow.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ModifiedRow { get { return modifiedRow; } }
		protected VGridControlBase Grid { get { return grid; } }
		public override bool IsLoading { get { return Grid.IsLoading; } }
	}
	public class PDescControlAppearanceCollection : BaseAppearanceCollection {
		readonly PropertyDescriptionControl owner;
		const string CaptionName = "Caption";
		const string DescriptionName = "Description";
		const string PanelName = "Panel";
		protected PropertyDescriptionControl Owner { get { return owner; } }
		public PDescControlAppearanceCollection(PropertyDescriptionControl owner) {
			this.owner = owner;
		}
		protected override void CreateAppearances() {
			CreateAppearance(CaptionName);
			CreateAppearance(DescriptionName);
			CreateAppearance(PanelName);
		}
		protected override AppearanceObject CreateAppearance(string name, AppearanceObject parent) {
			ValidateAppearanceName(name);
			AppearanceObject appearance = CreateAppearanceCore(parent, name);
			InitAppearance(appearance, name);
			return appearance;
		}
		protected virtual AppearanceObject CreateAppearanceCore(AppearanceObject parentAppearance, string name) {
			switch(name){
				case CaptionName:
					return new CaptionAppearanceObject(this, parentAppearance, name);
				case DescriptionName:
					return new TextAppearanceObject(this, parentAppearance, name);
				case PanelName:
					return new PanelAppearanceObject(this, parentAppearance, name);
				default:
					return null;
			}
		}
		void ResetCaption() { Caption.Reset(); }
		bool ShouldSerializeCaption() { return Caption.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CaptionAppearanceObject Caption { get { return (CaptionAppearanceObject)GetAppearance(CaptionName); } }
		void ResetDescription() { Description.Reset(); }
		bool ShouldSerializeDescription() { return Description.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TextAppearanceObject Description { get { return (TextAppearanceObject)GetAppearance(DescriptionName); } }
		void ResetPanel() { Panel.Reset(); }
		bool ShouldSerializePanel() { return Panel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelAppearanceObject Panel { get { return (PanelAppearanceObject)GetAppearance(PanelName); } }
	}
	public class TextAppearanceObject : AppearanceObject {
		public TextAppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name) : base(owner, parentAppearance, name) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor2 { get { return base.BackColor2; } set { base.BackColor2 = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BorderColor { get { return base.BorderColor; } set { base.BorderColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LinearGradientMode GradientMode { get { return base.GradientMode; } set { base.GradientMode = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image Image { get { return base.Image; } set { base.Image = value; } }
	}
	public class CaptionAppearanceObject : TextAppearanceObject {
		static Font defaultFont;
		public CaptionAppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name) : base(owner, parentAppearance, name) { }
		protected override Font InnerDefaultFont {
			get {
				if(defaultFont == null)
					defaultFont = new Font(AppearanceObject.DefaultFont, FontStyle.Bold);
				return defaultFont;
			}
		}
		void ResetFont() { Font = InnerDefaultFont; }
		[XtraSerializableProperty,  RefreshProperties(RefreshProperties.All)]
		public new Font Font { get { return base.Font; } set { base.Font = value; } }
	}
	public class PanelAppearanceObject : AppearanceObject {
		public PanelAppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name) : base(owner, parentAppearance, name) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Font Font { get { return base.Font; } set { base.Font = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new TextOptions TextOptions { get { return base.TextOptions; } }
	}
}
