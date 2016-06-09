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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Data;
using System.Collections.ObjectModel;
namespace DevExpress.XtraPivotGrid.Customization {
	public abstract class CustomizationFormArrangerBase {
		List<CustomizationFormLayout> supportedLayouts;
		public List<CustomizationFormLayout> SupportedLayouts {
			get {
				if(supportedLayouts == null) {
					supportedLayouts = CreateSupportedLayouts();
				}
				return supportedLayouts;
			}
		}
		protected abstract List<CustomizationFormLayout> CreateSupportedLayouts();
		public abstract void Arrange(CustomizationForm form, CustomizationFormLayout layout);
	}
	public class SimpleCustomizationFormArranger : CustomizationFormArrangerBase {
		protected override List<CustomizationFormLayout> CreateSupportedLayouts() {
			List<CustomizationFormLayout> res = new List<CustomizationFormLayout>(1);
			res.Add(CustomizationFormLayout.StackedDefault);
			return res;
		}
		public override void Arrange(CustomizationForm form, CustomizationFormLayout layout) {
			switch(layout) {
				case CustomizationFormLayout.StackedDefault:
					ArrangeStackedDefault(form);
					break;
				default:
					throw new ArgumentException("unsupported layout");
			}
		}
		protected void ArrangeStackedDefault(CustomizationForm form) {
			form.HintLabel.Visible = true;
			form.HintLabel.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormText);
			form.SplitContainer.Horizontal = false;
			form.SplitContainer.IsLocked = !form.BottomPanel.IsResizable;
			form.SplitContainer.PanelVisibility = SplitPanelVisibility.Both;			
			form.SplitContainer.SuspendLayout();			
			form.ActiveListBox.Parent = form.SplitContainer.Panel1;
			form.ActiveListBox.Dock = DockStyle.Fill;
			form.BottomPanel.Parent = form.SplitContainer.Panel2;
			form.BottomPanel.Dock = DockStyle.Fill;
			form.SplitContainer.ResumeLayout();
			form.UpdateMinimumSize();
		}
	}
	public class ExcelCustomizationFormArranger : SimpleCustomizationFormArranger {
		protected override List<CustomizationFormLayout> CreateSupportedLayouts() {
			List<CustomizationFormLayout> res = new List<CustomizationFormLayout>(1);
			res.Add(CustomizationFormLayout.StackedDefault);
			res.Add(CustomizationFormLayout.StackedSideBySide);
			res.Add(CustomizationFormLayout.TopPanelOnly);
			res.Add(CustomizationFormLayout.BottomPanelOnly1by4);
			res.Add(CustomizationFormLayout.BottomPanelOnly2by2);
			return res;
		}
		public override void Arrange(CustomizationForm form, CustomizationFormLayout layout) {
			switch(layout) {
				case CustomizationFormLayout.StackedDefault:
					ArrangeStackedDefault(form);
					break;
				case CustomizationFormLayout.StackedSideBySide:
					ArrangeStackedSideBySide(form);
					break;
				case CustomizationFormLayout.BottomPanelOnly1by4:
					ArrangeBottomPanelOnly(form);
					break;
				case CustomizationFormLayout.BottomPanelOnly2by2:
					ArrangeBottomPanelOnly(form);
					break;
				case CustomizationFormLayout.TopPanelOnly:
					ArrangeTopPanelOnly(form);
					break;
				default:
					throw new ArgumentException("unsupported layout");
			}
			form.ExcelBottomPanel.PanelLayout = form.FormLayout;
			if(form.SplitContainer.Horizontal)
				form.SplitContainer.SplitterPosition = form.ExcelBottomPanel.MinimumSize.Width;
			else
				form.SplitContainer.SplitterPosition = form.ExcelBottomPanel.MinimumSize.Height;
			form.UpdateMinimumSize();
		}		
		protected void ArrangeStackedSideBySide(CustomizationForm form) {
			ArrangeStackedDefault(form);
			form.SplitContainer.Horizontal = true;
		}
		protected void ArrangeBottomPanelOnly(CustomizationForm form) {
			ArrangeStackedDefault(form);
			form.SplitContainer.PanelVisibility = SplitPanelVisibility.Panel2;
			form.HintLabel.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormHint);
		}
		protected void ArrangeTopPanelOnly(CustomizationForm form) {
			ArrangeStackedDefault(form);
			form.SplitContainer.IsLocked = true;
			form.SplitContainer.PanelVisibility = SplitPanelVisibility.Both;
		}
	}
	public abstract class ExcelPanelBaseArranger {
		protected const int MinListHeight = 80, MinListWidth = 150,
			MarginX = 10, MarginY = 5;
		public ExcelPanelBaseArranger(ExcelCustomizationFormBottomPanel owner) {
			this.owner = owner;
		}
		ExcelCustomizationFormBottomPanel owner;
		protected ExcelCustomizationFormBottomPanel Owner { get { return owner; } }
		protected int CaptionHeight {
			get { return Math.Max(GetAreaIcon(PivotArea.FilterArea).Height, GetAreaLabel(PivotArea.FilterArea).Height); }
		}
		protected int TotalListHeight {
			get {
				return CaptionHeight + MarginY + MinListHeight + MarginY;
			}
		}
		protected Label GetAreaLabel(PivotArea area) {
			return Owner.GetAreaLabel(area);
		}
		protected PictureBox GetAreaIcon(PivotArea area) {
			return Owner.GetAreaIcon(area);
		}
		protected ExcelFieldCustomizationList GetAreaList(PivotArea area) {
			return Owner.GetAreaList(area);
		}
		public abstract int MinWidth { get; }
		public abstract int MinHeight { get; }
		public abstract void Arrange(Size size);
		protected void SetAllListsVisibility(bool visible) {
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				GetAreaLabel(area).Visible = visible;
				GetAreaList(area).Visible = visible;
				GetAreaIcon(area).Visible = visible;
			}
		}
		protected void ArrangeLabel(PivotArea area, int width) {
			ArrangeLabel(GetAreaIcon(area), GetAreaLabel(area), width);
		}
		void ArrangeLabel(PictureBox icon, Label label, int width) {
			label.Left = icon.Bounds.Right + 1;
			label.Top = icon.Top + (icon.Height - label.Height) / 2 + 1;
			label.Width = width - icon.Width - 1;
		}
	}
	public class ExcelPanelStacked2by2Arranger : ExcelPanelBaseArranger {
		public ExcelPanelStacked2by2Arranger(ExcelCustomizationFormBottomPanel owner)
			: base(owner) {
		}
		protected virtual bool IsHintLabelVisible {
			get { return true; }
		}
		public override int MinWidth {
			get {
				return 0;
			}
		}
		public override int MinHeight {
			get {
				return (IsHintLabelVisible ? MarginY + Owner.HintLabel.Height + MarginY : 0) +
					2 * TotalListHeight + Owner.UpdateButton.Height;
			}
		}
		public override void Arrange(Size size) {
			int listWidth = (size.Width - MarginX) / 2,
				listHeight = (size.Height - MinHeight + 2 * MinListHeight) / 2,
				column1Left = 0,
				column2Left = listWidth + MarginX;
			if(IsHintLabelVisible) {
				Owner.HintLabel.Visible = true;
				Owner.HintLabel.Left = column1Left;
				Owner.HintLabel.Top = MarginY;
				Owner.HintLabel.Width = size.Width - column1Left;
			} else {
				Owner.HintLabel.Visible = false;
			}
			SetAllListsVisibility(true);
			GetAreaIcon(PivotArea.FilterArea).Left = column1Left;
			GetAreaIcon(PivotArea.FilterArea).Top = IsHintLabelVisible ? Owner.HintLabel.Bounds.Bottom + MarginY : 0;
			ArrangeLabel(PivotArea.FilterArea, listWidth);			
			GetAreaList(PivotArea.FilterArea).Left = column1Left;
			GetAreaList(PivotArea.FilterArea).Top = GetAreaIcon(PivotArea.FilterArea).Bounds.Bottom + MarginY;
			GetAreaList(PivotArea.FilterArea).Width = listWidth;
			GetAreaList(PivotArea.FilterArea).Height = listHeight;
			GetAreaList(PivotArea.FilterArea).TabIndex = 0;
			GetAreaIcon(PivotArea.RowArea).Left = column1Left;
			GetAreaIcon(PivotArea.RowArea).Top = GetAreaList(PivotArea.FilterArea).Bounds.Bottom + MarginY;
			ArrangeLabel(PivotArea.RowArea, listWidth);
			GetAreaList(PivotArea.RowArea).Left = column1Left;
			GetAreaList(PivotArea.RowArea).Top = GetAreaLabel(PivotArea.RowArea).Bounds.Bottom + MarginY;
			GetAreaList(PivotArea.RowArea).Width = listWidth;
			GetAreaList(PivotArea.RowArea).Height = listHeight;
			GetAreaList(PivotArea.RowArea).TabIndex = 2;
			GetAreaIcon(PivotArea.ColumnArea).Left = column2Left;
			GetAreaIcon(PivotArea.ColumnArea).Top = GetAreaIcon(PivotArea.FilterArea).Top;
			ArrangeLabel(PivotArea.ColumnArea, listWidth);
			GetAreaList(PivotArea.ColumnArea).Left = column2Left;
			GetAreaList(PivotArea.ColumnArea).Top = GetAreaList(PivotArea.FilterArea).Top;
			GetAreaList(PivotArea.ColumnArea).Width = listWidth;
			GetAreaList(PivotArea.ColumnArea).Height = listHeight;
			GetAreaList(PivotArea.ColumnArea).TabIndex = 1;
			GetAreaIcon(PivotArea.DataArea).Left = column2Left;
			GetAreaIcon(PivotArea.DataArea).Top = GetAreaIcon(PivotArea.RowArea).Top;
			ArrangeLabel(PivotArea.DataArea, listWidth);
			GetAreaList(PivotArea.DataArea).Left = column2Left;
			GetAreaList(PivotArea.DataArea).Top = GetAreaList(PivotArea.RowArea).Top;
			GetAreaList(PivotArea.DataArea).Width = listWidth;
			GetAreaList(PivotArea.DataArea).Height = listHeight;
			GetAreaList(PivotArea.DataArea).TabIndex = 3;
			Owner.UpdateButton.Visible = true;
			Owner.UpdateButton.Left = size.Width - Owner.UpdateButton.Width;
			Owner.UpdateButton.Top = size.Height - Owner.UpdateButton.Height;
			Owner.UpdateButton.TabIndex = 5;
			Owner.DeferredUpdatesCheck.Visible = true;
			Owner.DeferredUpdatesCheck.Left = column1Left;
			Owner.DeferredUpdatesCheck.Top = Owner.UpdateButton.Top +
				(Owner.UpdateButton.Height - Owner.DeferredUpdatesCheck.Height) / 2;
			Owner.DeferredUpdatesCheck.Width = Owner.UpdateButton.Left - MarginX;
			Owner.DeferredUpdatesCheck.TabIndex = 4;
		}		
	}
	public class ExcelPanelPanelOnly2by2Arranger : ExcelPanelStacked2by2Arranger {
		public ExcelPanelPanelOnly2by2Arranger(ExcelCustomizationFormBottomPanel owner)
			: base(owner) {
		}
		protected override bool IsHintLabelVisible {
			get { return false; }
		}
	}
	public class ExcelPanelStacked1By4Arranger : ExcelPanelBaseArranger {
		public ExcelPanelStacked1By4Arranger(ExcelCustomizationFormBottomPanel owner)
			: base(owner) {
		}
		protected virtual int ColumnLeft { 
			get { return MarginX / 2; } 
		}
		protected virtual bool IsHintLabelVisible {
			get { return true; }
		}
		public override int MinWidth {
			get { return ColumnLeft + MinListWidth; }
		}
		public override int MinHeight {
			get {
				return (IsHintLabelVisible ? Owner.HintLabel.Height + MarginY : 0) +
					4 * TotalListHeight + Owner.UpdateButton.Height;
			}
		}
		public override void Arrange(Size size) {
			int columnLeft = ColumnLeft,
				listWidth = size.Width - columnLeft,
				listHeight = (size.Height - MinHeight + 4 * MinListHeight) / 4;
			if(IsHintLabelVisible) {
				Owner.HintLabel.Visible = true;
				Owner.HintLabel.Left = columnLeft;
				Owner.HintLabel.Top = 0;
				Owner.HintLabel.Width = listWidth;
			} else {
				Owner.HintLabel.Visible = false;
			}
			SetAllListsVisibility(true);
			GetAreaIcon(PivotArea.FilterArea).Left = columnLeft;
			GetAreaIcon(PivotArea.FilterArea).Top = IsHintLabelVisible ? Owner.HintLabel.Bounds.Bottom + MarginY : 0;
			ArrangeLabel(PivotArea.FilterArea, listWidth);
			GetAreaList(PivotArea.FilterArea).Left = columnLeft;
			GetAreaList(PivotArea.FilterArea).Top = GetAreaIcon(PivotArea.FilterArea).Bounds.Bottom + MarginY;
			GetAreaList(PivotArea.FilterArea).Width = listWidth;
			GetAreaList(PivotArea.FilterArea).Height = listHeight;
			GetAreaList(PivotArea.FilterArea).TabIndex = 0;
			GetAreaIcon(PivotArea.RowArea).Left = columnLeft;
			GetAreaIcon(PivotArea.RowArea).Top = GetAreaList(PivotArea.FilterArea).Bounds.Bottom + MarginY;
			ArrangeLabel(PivotArea.RowArea, listWidth);
			GetAreaList(PivotArea.RowArea).Left = columnLeft;
			GetAreaList(PivotArea.RowArea).Top = GetAreaIcon(PivotArea.RowArea).Bounds.Bottom + MarginY;
			GetAreaList(PivotArea.RowArea).Width = listWidth;
			GetAreaList(PivotArea.RowArea).Height = listHeight;
			GetAreaList(PivotArea.RowArea).TabIndex = 1;
			GetAreaIcon(PivotArea.ColumnArea).Left = columnLeft;
			GetAreaIcon(PivotArea.ColumnArea).Top = GetAreaList(PivotArea.RowArea).Bounds.Bottom + MarginY;
			ArrangeLabel(PivotArea.ColumnArea, listWidth);
			GetAreaList(PivotArea.ColumnArea).Left = columnLeft;
			GetAreaList(PivotArea.ColumnArea).Top = GetAreaIcon(PivotArea.ColumnArea).Bounds.Bottom + MarginY;
			GetAreaList(PivotArea.ColumnArea).Width = listWidth;
			GetAreaList(PivotArea.ColumnArea).Height = listHeight;
			GetAreaList(PivotArea.ColumnArea).TabIndex = 2;
			GetAreaIcon(PivotArea.DataArea).Left = columnLeft;
			GetAreaIcon(PivotArea.DataArea).Top = GetAreaList(PivotArea.ColumnArea).Bounds.Bottom + MarginY;
			ArrangeLabel(PivotArea.DataArea, listWidth);
			GetAreaList(PivotArea.DataArea).Left = columnLeft;
			GetAreaList(PivotArea.DataArea).Top = GetAreaIcon(PivotArea.DataArea).Bounds.Bottom + MarginY;
			GetAreaList(PivotArea.DataArea).Width = listWidth;
			GetAreaList(PivotArea.DataArea).Height = listHeight;
			GetAreaList(PivotArea.DataArea).TabIndex = 3;
			Owner.UpdateButton.Visible = true;
			Owner.UpdateButton.Left = size.Width - Owner.UpdateButton.Width;
			Owner.UpdateButton.Top = size.Height - Owner.UpdateButton.Height;
			Owner.UpdateButton.TabIndex = 5;
			Owner.DeferredUpdatesCheck.Visible = true;
			Owner.DeferredUpdatesCheck.Left = columnLeft;
			Owner.DeferredUpdatesCheck.Top = Owner.UpdateButton.Top +
				(Owner.UpdateButton.Height - Owner.DeferredUpdatesCheck.Height) / 2;
			Owner.DeferredUpdatesCheck.Width = Owner.UpdateButton.Left - MarginX;
			Owner.DeferredUpdatesCheck.TabIndex = 4;
		}
	}
	public class ExcelPanelPanelOnly1By4Arranger : ExcelPanelStacked1By4Arranger {
		public ExcelPanelPanelOnly1By4Arranger(ExcelCustomizationFormBottomPanel owner)
			: base(owner) {
		}
		protected override int ColumnLeft { get { return 0; } }
		protected override bool IsHintLabelVisible {
			get { return false; }
		}
	}
	public class ExcelPanelEmptyArranger : ExcelPanelBaseArranger {
		public ExcelPanelEmptyArranger(ExcelCustomizationFormBottomPanel owner)
			: base(owner) {
		}
		public override int MinWidth {
			get { return 0; }
		}
		public override int MinHeight {
			get {
				return 0;
			}
		}
		public override void Arrange(Size size) {
			Owner.HintLabel.Visible = false;
			SetAllListsVisibility(false);
			Owner.UpdateButton.Visible = false;
			Owner.DeferredUpdatesCheck.Visible = false;
		}
	}
}
