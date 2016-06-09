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
using DevExpress.XtraPivotGrid.Customization.ViewInfo;
namespace DevExpress.XtraPivotGrid.Customization {
	[ToolboxItem(false)]
	public class ExcelCustomizationFormBottomPanel : CustomizationFormBottomPanelBase {
		internal ExcelFieldCustomizationList[] areaLists;
		Label[] areaLabels;
		PictureBox[] areaIcons;
		public ExcelCustomizationFormBottomPanel(CustomizationForm form)
			: base(form) {
			BackColor = Color.Transparent;
			CreateControls();
			PanelLayout = form.FormLayout;
		}
		public override bool IsResizable {
			get { return true; }
		}
		ExcelPanelBaseArranger panelArranger;
		protected ExcelPanelBaseArranger PanelArranger {
			get { return panelArranger; }
			set {
				if(panelArranger == value) return;
				panelArranger = value;
				if(PanelArranger.MinHeight > 0)
					Height = PanelArranger.MinHeight;
				if(PanelArranger.MinWidth > 0)
					Width = PanelArranger.MinWidth;
				MinimumSize = new Size(PanelArranger.MinWidth, PanelArranger.MinHeight);
				PanelArranger.Arrange(ClientSize);
			}
		}
		CustomizationFormLayout panelLayout;
		public CustomizationFormLayout PanelLayout {
			get { return panelLayout; }
			set {
				if(panelLayout == value && PanelArranger != null) return;
				panelLayout = value;
				PanelArranger = CreateArranger(panelLayout);
			}
		}
		Label hintLabel;
		public Label HintLabel {
			get { return hintLabel; }
		}
		SimpleButton updateButton;
		public SimpleButton UpdateButton {
			get { return updateButton; }
		}
		CheckEdit deferredUpdatesCheck;
		public CheckEdit DeferredUpdatesCheck {
			get { return deferredUpdatesCheck; }
		}
		public Label GetAreaLabel(PivotArea area) {
			return areaLabels[(int)area];
		}
		public PictureBox GetAreaIcon(PivotArea area) {
			return areaIcons[(int)area];
		}
		public ExcelFieldCustomizationList GetAreaList(PivotArea area) {
			return areaLists[(int)area];
		}
		void CreateControls() {
			this.areaLists = new ExcelFieldCustomizationList[Enum.GetValues(typeof(PivotArea)).Length];
			this.areaLabels = new Label[this.areaLists.Length];
			this.areaIcons = new PictureBox[this.areaLists.Length];
			for(int i = 0; i < areaLists.Length; i++) {
				this.areaLists[i] = CreateAreaListCore((PivotArea)i);
				this.areaLists[i].Parent = this;
				this.areaLabels[i] = new Label();
				this.areaLabels[i].Parent = this;
				this.areaLabels[i].BackColor = Color.Transparent;
				this.areaLabels[i].Text = PivotGridLocalizer.GetAreaText(i);
				this.areaLabels[i].Size = this.areaLabels[i].PreferredSize;
				this.areaLabels[i].AutoEllipsis = true;
				this.areaIcons[i] = new PictureBox();
				this.areaIcons[i].Parent = this;
				this.areaIcons[i].BackColor = Color.Transparent;
				this.areaIcons[i].Image = CustomizationForm.Icons.Images[i + 1];
				this.areaIcons[i].Size = this.areaIcons[i].Image.Size;
			}
			this.hintLabel = new Label();
			this.hintLabel.Parent = this;
			this.hintLabel.BackColor = Color.Transparent;
			this.hintLabel.AutoSize = false;
			this.hintLabel.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormHint);
			this.hintLabel.Size = this.hintLabel.PreferredSize;
			this.hintLabel.AutoEllipsis = true;
			this.deferredUpdatesCheck = new CheckEdit();
			this.deferredUpdatesCheck.Parent = this;
			this.deferredUpdatesCheck.BackColor = Color.Transparent;
			this.deferredUpdatesCheck.Text = PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.CustomizationFormDeferLayoutUpdate);
			this.deferredUpdatesCheck.Checked = CustomizationForm.Data.OptionsCustomization.DeferredUpdates;
			this.deferredUpdatesCheck.CheckedChanged += OnDeferredUpdatesChanged;
			this.updateButton = new SimpleButton();
			this.updateButton.Parent = this;
			this.updateButton.Text = PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.CustomizationFormUpdate);
			this.updateButton.Size = this.updateButton.CalcBestSize();
			this.updateButton.Click += new EventHandler(OnUpdateButtonClick);
			UpdateUpdateButtonEnabled();
		}
		protected virtual ExcelFieldCustomizationList CreateAreaListCore(PivotArea area) {
			return new ExcelFieldCustomizationList(CustomizationForm, area);
		}
		protected virtual void OnDeferredUpdatesChanged(object sender, EventArgs e) {
			UpdateUpdateButtonEnabled();
			if(!DeferredUpdatesCheck.Checked)
				CustomizationFields.SetFieldsToData();
		}
		protected void UpdateUpdateButtonEnabled() {
			CustomizationFields.DeferUpdates = DeferredUpdatesCheck.Checked;
			UpdateButton.Enabled = DeferredUpdatesCheck.Checked;
		}
		protected virtual void OnUpdateButtonClick(object sender, EventArgs e) {
			CustomizationFields.SetFieldsToData();
		}
		protected virtual ExcelPanelBaseArranger CreateArranger(CustomizationFormLayout formLayout) {
			switch(formLayout) {
				case CustomizationFormLayout.StackedDefault:
					return new ExcelPanelStacked2by2Arranger(this);
				case CustomizationFormLayout.StackedSideBySide:
					return new ExcelPanelStacked1By4Arranger(this);
				case CustomizationFormLayout.BottomPanelOnly1by4:
					return new ExcelPanelPanelOnly1By4Arranger(this);
				case CustomizationFormLayout.BottomPanelOnly2by2:
					return new ExcelPanelPanelOnly2by2Arranger(this);
				case CustomizationFormLayout.TopPanelOnly:
					return new ExcelPanelEmptyArranger(this);
				default:
					throw new ArgumentException("formLayout");
			}
		}
		protected override void OnResize(EventArgs eventargs) {
			base.OnResize(eventargs);
			if(PanelArranger != null) {
				SuspendLayout();
				PanelArranger.Arrange(ClientSize);
				ResumeLayout();
			}
		}
		public override void Populate() {
			base.Populate();
			for(int i = 0; i < areaLists.Length; i++) {
				areaLists[i].Populate();
			}
		}
		public override bool HandleDragComplete(PivotFieldItem field, Point lastMovePt) {
			Point localPt = PointToClient(lastMovePt);
			for(int i = 0; i < areaLists.Length; i++) {
				if(areaLists[i].Bounds.Contains(localPt))
					return areaLists[i].HandleDragComplete(field, lastMovePt);
			}
			return false;
		}
		public override bool IsNodeVisible(ICustomizationTreeItem node) {
			for(int i = 0; i < areaLists.Length; i++) {
				if(CustomizationFields[(PivotArea)i].Contains(node.Field))
					return false;
			}
			return true;
		}
		public override void SetEnabledState(bool enabled) {
			base.SetEnabledState(enabled);
			DeferredUpdatesCheck.Enabled = enabled;
			UpdateButton.Enabled = enabled;
			HintLabel.Enabled = enabled;
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea)))
				GetAreaList(area).Enabled = enabled;
		}
	}
	[ToolboxItem(false)]
	public class ExcelFieldCustomizationList : PivotCustomizationTreeBoxBase {
		public ExcelFieldCustomizationList(CustomizationForm form, PivotArea area)
			: base(form) {
			this.area = area;
			Dock = DockStyle.None;
		}
		PivotArea area;
		protected PivotArea Area { get { return area; } }
		protected override void PopulateCore() {
			ReadOnlyCollection<PivotFieldItemBase> newItems = CustomizationFields[Area];
			for(int i = 0; i < newItems.Count; i++) {
				Items.Add(new ExcelListPivotCustomizationTreeNode(CustomizationFields.FieldItems[newItems[i].Index]));
			}
		}
		public bool HandleDragComplete(PivotFieldItem field, Point lastMovePt) {
			int areaIndex = GetTargetItemIndex(lastMovePt);
			if(Data.OnFieldAreaChanging(Data.GetField(field), Area, areaIndex))
				return CustomizationFields.MoveField(field, Area, areaIndex);
			return false;
		}
		protected int GetTargetItemIndex(Point lastMovePt) {
			Point localPt = PointToClient(lastMovePt);
			int itemIndex = IndexFromPoint(localPt);
			if(itemIndex >= Items.Count)
				itemIndex = -1;
			return itemIndex;
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
		}
		protected override CustomizationItemViewInfo CreateItemViewInfo(IVisualCustomizationTreeItem node) {
			return new PivotCustomizationListItemViewInfo(this, node, Data.ViewInfo);
		}
		protected override int CalcItemHeight() {
			return PivotViewInfo.FieldMeasures.DefaultHeaderHeight;
		}
	}
	public class ExcelListPivotCustomizationTreeNode : PivotCustomizationTreeNodeBase, IVisualCustomizationTreeItem {
		public ExcelListPivotCustomizationTreeNode(PivotFieldItemBase field)
			: base(field) {
		}
		public int ImageIndex { get { return -1; } }
	}
}
