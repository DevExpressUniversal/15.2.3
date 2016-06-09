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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Customization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraPivotGrid.Localization;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.XtraPivotGrid.Customization {
	[ToolboxItem(false)]
	public class CustomizationForm : CustomizationFormBase {
		const int LayoutDefaultPadding = 5,
			MinListHeight = 50, MinListWidth = 50;
		static ImageCollection icons = null;
		public static ImageCollection Icons {
			get {
				if(icons == null)
					icons = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources(
						"DevExpress.XtraPivotGrid.Images.customization2007icons.png",
						typeof(CustomizationFormLayoutButton).Assembly, new Size(16, 16));
				return icons;
			}
		}
		public CustomizationForm(PivotGridViewInfoData data, CustomizationFormStyle style) 
			: base() {
			this.data = data;
			this.style = style;
			this.customizationFields = new CustomizationFormFields(Data);
			SubscribeEvents();
		}
#if DEBUGTEST
		internal void InitTest() {
			InitCustomizationForm();
		}
#endif
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(CustomizationFields != null) {
					UnsubscribeEvents();
					CustomizationFields.Dispose();
					customizationFields = null;
				}
			}
			base.Dispose(disposing);
		}
		PivotGridViewInfoData data;
		public PivotGridViewInfoData Data { 
			get { return data; } 
		}
		CustomizationFormFields customizationFields;
		public CustomizationFormFields CustomizationFields {
			get { return customizationFields; }
		}
		CustomizationSplitControl splitContainer;
		public CustomizationSplitControl SplitContainer {
			get { return splitContainer; }
		}
		public virtual new PivotCustomizationTreeBox ActiveListBox {
			get { return (PivotCustomizationTreeBox)base.ActiveListBox; }
		}
		public override Control ControlOwner {
			get { return Data.ControlOwner; }
		}
		PanelControl containerPanel;
		public PanelControl ContainerPanel { 
			get { return containerPanel; } 
		}
		Panel pnlTop;
		public Panel TopPanel {
			get { return pnlTop; }
		}
		Label hintLabel;
		public Label HintLabel {
			get { return hintLabel; }
		}
		CustomizationFormBottomPanelBase bottomPanel;
		public CustomizationFormBottomPanelBase BottomPanel {
			get { return bottomPanel; }
		}
		CustomizationFormLayoutButton layoutButton;
		public CustomizationFormLayoutButton LayoutButton {
			get { return layoutButton; }
		}
		CustomizationFormArrangerBase formArranger;
		protected CustomizationFormArrangerBase FormArranger { 
			get { return formArranger; } 
		}
		public List<CustomizationFormLayout> SupportedLayouts {
			get { return FormArranger.SupportedLayouts; }
		}
		CustomizationFormLayout layout;
		public CustomizationFormLayout FormLayout {
			get { return layout; }
			set {
				if(FormLayout == value || !FormArranger.SupportedLayouts.Contains(value))
					return;
				layout = value;
				SuspendLayout();
				FormArranger.Arrange(this, layout);
				ResumeLayout();
			}
		}
		internal ExcelCustomizationFormBottomPanel ExcelBottomPanel {
			get { return BottomPanel as ExcelCustomizationFormBottomPanel; }
		}
		internal SimpleCustomizationFormBottomPanel SimpleBottomPanel {
			get { return BottomPanel as SimpleCustomizationFormBottomPanel; }
		}
		CustomizationFormStyle style;
		public CustomizationFormStyle Style {
			get { return style; }
		}
		public PivotFieldItem SelectedField {
			get {
				if(ActiveListBox == null || ActiveListBox.SelectedItem == null)
					return null;
				return (PivotFieldItem)ActiveListBox.SelectedItem.Field;
			}
		}
		protected override void InitCustomizationForm() {
			CreateContainerPanel();
			CreateFormArranger();
			CreateTopPanel();
			CreateBottomPanel();
			base.InitCustomizationForm();
			DockPadding.All = LayoutDefaultPadding;
			CreateSplitContainer();
			SetDefaultLayout();
			ContainerPanel.BorderStyle = BorderStyles.NoBorder;
			ContainerPanel.Padding = Padding;
			Padding = new Padding(0);
			ActiveListBox.Focus();
		}
		protected void CreateContainerPanel() {
			this.containerPanel = new PanelControl();
			ContainerPanel.Parent = this;
			ContainerPanel.Dock = DockStyle.Fill;
			ContainerPanel.BringToFront();
		}
		protected void SetDefaultLayout() {
			FormLayout = Data.OptionsCustomization.CustomizationFormLayout;
			if(FormLayout != Data.OptionsCustomization.CustomizationFormLayout)
				FormLayout = FormArranger.SupportedLayouts[0];
			FormArranger.Arrange(this, FormLayout);
		}
		void CreateSplitContainer() {
			this.splitContainer = new CustomizationSplitControl();
			SplitContainer.Parent = ContainerPanel;
			SplitContainer.Dock = DockStyle.Fill;
			SplitContainer.BringToFront();
			SplitContainer.FixedPanel = SplitFixedPanel.Panel2;						
			SplitContainer.SplitterPosition = BottomPanel.MinimumSize.Height;
			SplitContainer.SplitterPositionChanged += new EventHandler(SplitContainer_SplitterPositionChanged);
		}
		void SplitContainer_SplitterPositionChanged(object sender, EventArgs e) {
			if(!SplitContainer.Horizontal && SplitContainer.Panel2.Height < BottomPanel.MinimumSize.Height) {
				SplitContainer.SplitterPosition = BottomPanel.MinimumSize.Height;
			}
			if(SplitContainer.Horizontal && SplitContainer.Panel2.Width < BottomPanel.MinimumSize.Width) {
				SplitContainer.SplitterPosition = BottomPanel.MinimumSize.Width;
			}
		}
		void CreateTopPanel() {
			this.pnlTop = CreatePanel(DockStyle.Top, 20);
			this.hintLabel = new Label();
			HintLabel.Parent = TopPanel;
			HintLabel.BackColor = Color.Transparent;
			HintLabel.Dock = DockStyle.Fill;
			HintLabel.AutoEllipsis = true;
			HintLabel.AutoSize = false;			
			if(FormArranger.SupportedLayouts.Count > 1)
				CreateLayoutButton();
		}
		void CreateLayoutButton() {
			this.layoutButton = new CustomizationFormLayoutButton(this);
			LayoutButton.Parent = TopPanel;
			LayoutButton.Width = 40;
			LayoutButton.Left = TopPanel.Width - LayoutButton.Width;
			LayoutButton.Top = 0;
			LayoutButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			TopPanel.Height = LayoutButton.Height + LayoutDefaultPadding;
			HintLabel.Dock = DockStyle.None;
			HintLabel.Width = LayoutButton.Left - LayoutDefaultPadding;
			HintLabel.Top = (LayoutButton.Height - HintLabel.PreferredHeight) / 2;
			HintLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		}
		Panel CreatePanel(DockStyle style, int height) {
			Panel panel = new Panel();
			panel.Parent = ContainerPanel;
			panel.Dock = style;
			panel.Height = height;
			panel.BackColor = Color.Transparent;
			panel.BorderStyle = BorderStyle.None;
			return panel;
		}
		protected void CreateBottomPanel() {
			this.bottomPanel = CreateBottomPanelCore();
		}
		protected virtual CustomizationFormBottomPanelBase CreateBottomPanelCore() {
			switch(Style) {
				case CustomizationFormStyle.Simple:
					return new SimpleCustomizationFormBottomPanel(this);
				case CustomizationFormStyle.Excel2007:
					return new ExcelCustomizationFormBottomPanel(this);
				default:
					throw new ArgumentException("Style");
			}
		}
		protected void CreateFormArranger() {
			this.formArranger = CreateFormArrangerCore();
		}
		protected virtual CustomizationFormArrangerBase CreateFormArrangerCore() {
			switch(Style) {
				case CustomizationFormStyle.Simple:
					return new SimpleCustomizationFormArranger();
				case CustomizationFormStyle.Excel2007:
					return new ExcelCustomizationFormArranger();
				default:
					throw new ArgumentException("Style");
			}
		}
		public void Populate() {
			if(ActiveListBox == null)
				return;
			ActiveListBox.SelectedIndexChanged -= new EventHandler(OnListBoxSelectedChanged);
			ActiveListBox.Populate();
			BottomPanel.Populate();
			ActiveListBox.SelectedIndexChanged += new EventHandler(OnListBoxSelectedChanged);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ActiveListBox.BringToFront();
			ActiveListBox.SelectedIndexChanged += new EventHandler(OnListBoxSelectedChanged);
			BottomPanel.Populate();
		}
		protected override CustomizationListBoxBase CreateCustomizationListBox() {
			return new PivotCustomizationTreeBox(this);
		}
		protected override string FormCaption { get { return PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormCaption); } } 
		protected override void OnFormClosed() {
			if(Data == null)  return;
			PivotGridViewInfoData oldData = Data;
			this.data = null;
			oldData.DestroyCustomization();
		}
		protected override Rectangle CustomizationFormBounds { get { return Data.CustomizationFormBounds; } }
		protected override UserLookAndFeel ControlOwnerLookAndFeel { get { return Data.ActiveLookAndFeel; } }
		protected virtual void OnListBoxSelectedChanged(object sender, EventArgs e) {
			BottomPanel.OnSelectedFieldChanged();
		}
		protected void SubscribeEvents() {
			CustomizationFields.BeforeSettingFields += OnBeforeSettingFields;
			CustomizationFields.AfterSettingFields += OnAfterSettingFields;
			CustomizationFields.ListsChanged += OnCustomizationFieldsListChanged;
		}		
		protected void UnsubscribeEvents() {
			CustomizationFields.BeforeSettingFields -= OnBeforeSettingFields;
			CustomizationFields.AfterSettingFields -= OnAfterSettingFields;
		}
		protected virtual void OnCustomizationFieldsListChanged(object sender, CustomizationFormFieldsEventArgs e) {
			Populate();
		}
		protected virtual void OnBeforeSettingFields(object sender, CustomizationFormFieldsEventArgs e) {
			ActiveListBox.BeginUpdate();
		}
		protected virtual void OnAfterSettingFields(object sender, CustomizationFormFieldsEventArgs e) {
			ActiveListBox.EndUpdate();
			Populate();
		}
		public bool HandleDragComplete(PivotFieldItem field, Point lastMovePt) {
			if(ActiveListBox.ClientRectangle.Contains(ActiveListBox.PointToClient(lastMovePt))) {
				CustomizationFields.HideField(field);
				return true;
			}
			if(BottomPanel.ClientRectangle.Contains(BottomPanel.PointToClient(lastMovePt)))
				return BottomPanel.HandleDragComplete(field, lastMovePt);
			return false;
		}
		public void UpdateMinimumSize() {
			bool isVertical = !SplitContainer.Horizontal;
			int minWidth, minHeight;
			if(isVertical) {
				minWidth = Math.Max(BottomPanel.MinimumSize.Width, MinListWidth);
				minHeight = TopPanel.Height + MinListHeight + BottomPanel.MinimumSize.Height +
										SplitContainer.SplitterBounds.Height;
			} else {
				minWidth = MinListWidth + BottomPanel.MinimumSize.Width +
										SplitContainer.SplitterBounds.Width;
				minHeight = TopPanel.Height + Math.Max(MinListHeight, BottomPanel.MinimumSize.Height);
			}
			minWidth = Math.Max(minWidth + Width - ClientSize.Width, MinFormSize.Width);
			minHeight = Math.Max(minHeight + Height - ClientSize.Height, MinFormSize.Height);
			MinimumSize = new Size(minWidth, minHeight);
		}
		internal void SetDragComletedCallback(CustomizationFormFields.UpdateCompletedCallback callback) {
			CustomizationFields.SetUpdateCompletedCallback(callback);
		}
		public void SetEnabledState(bool enabled) {
			if(LayoutButton != null)
				LayoutButton.Enabled = enabled;
			BottomPanel.SetEnabledState(enabled);
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
	}
	[ToolboxItem(false)]
	public abstract class CustomizationFormBottomPanelBase : Panel {
		public CustomizationFormBottomPanelBase(CustomizationForm form) {
			this.customizationForm = form;
			Parent = form;
			Dock = DockStyle.Bottom;
			BackColor = Color.Transparent;
			BorderStyle = BorderStyle.None;
		}
		CustomizationForm customizationForm;
		protected CustomizationForm CustomizationForm {
			get { return customizationForm; }
		}
		protected internal PivotCustomizationTreeBox ActiveListBox { 
			get { return CustomizationForm.ActiveListBox; } 
		}
		protected Control ControlOwner {
			get { return CustomizationForm.ControlOwner; } 
		}
		protected PivotGridViewInfoData Data {
			get { return CustomizationForm.Data; }
		}
		protected PivotFieldItem SelectedField {
			get { return CustomizationForm.SelectedField; }
		}
		protected CustomizationFormFields CustomizationFields {
			get { return CustomizationForm.CustomizationFields; }
		}
		public virtual bool IsResizable {
			get { return false; }
		}
		public virtual void OnSelectedFieldChanged() {
		}
		public virtual void Populate() {
		}
		protected virtual string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
		public virtual bool HandleDragComplete(PivotFieldItem field, Point lastMovePt) {
			return false;
		}
		public virtual bool IsNodeVisible(ICustomizationTreeItem node) {
			return true;
		}
		public virtual void MoveFieldToPivotGrid(PivotFieldItem fd) {
			PivotGridFieldBase field = Data.GetField(fd);
			if(field == null) return;
			PivotArea area = GetSelectedArea(fd);
			int areaIndex = Data.Fields.GetVisibleFieldCount(area);
			if(Data.OnFieldAreaChanging(field, area, areaIndex)) {
				Data.SetFieldAreaPositionAsync(field, area, areaIndex, false, delegate(AsyncOperationResult result) {
					ActiveListBox.Populate();
					Populate();
				});
			}
		}
		protected virtual PivotArea GetSelectedArea(PivotFieldItem field) {			
			if(field.IsAreaAllowed(field.Area))
				return field.Area;
			if(field.IsAreaAllowed(PivotArea.FilterArea))
				return PivotArea.FilterArea;
			if(field.IsAreaAllowed(PivotArea.ColumnArea))
				return PivotArea.ColumnArea;
			if(field.IsAreaAllowed(PivotArea.RowArea))
				return PivotArea.RowArea;
			return PivotArea.DataArea;
		}
		public virtual void SetEnabledState(bool enabled) {
			ActiveListBox.Enabled = enabled;
		}
	}
	[ToolboxItem(false)]
	public class SimpleCustomizationFormBottomPanel : CustomizationFormBottomPanelBase {
		ComboBoxEdit cbArea;
		SimpleButton button;
		public ComboBoxEdit AreaComboBox {
			get { return cbArea; }
		}
		public SimpleCustomizationFormBottomPanel(CustomizationForm form)
			: base(form) {
			this.button = new SimpleButton();
			this.button.Parent = this;
			this.button.Click += new EventHandler(OnButtonClick);
			this.button.Text = GetLocalizedString(PivotGridStringId.CustomizationFormAddTo);
			this.button.Size = button.CalcBestSize();
			Height = button.Height + 6;
			MinimumSize = new Size(0, Height);
			this.button.Left = 0;			
			this.button.Top = (Height - button.Height) / 2;
			this.cbArea = new ComboBoxEdit();
			this.cbArea.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			this.cbArea.Parent = this;
			this.cbArea.Top = button.Top + (button.Height - cbArea.Height) / 2;
			this.cbArea.Left = button.Right + 3;
			this.cbArea.Width = ClientRectangle.Right - cbArea.Left;
			this.cbArea.Anchor = AnchorStyles.Left | AnchorStyles.Right;			
		}
		public override void OnSelectedFieldChanged() {
			base.OnSelectedFieldChanged();
			UpdateComboAndButton();
		}
		public override void Populate() {
			base.Populate();
			UpdateComboAndButton();
		}
		protected virtual void UpdateComboAndButton() {
			if(ActiveListBox.SelectedItem == null && ActiveListBox.ItemCount > 0)
				ActiveListBox.SelectedIndex = 0;
			this.button.Enabled = this.cbArea.Enabled = SelectedField != null;
			PopulateCombo(SelectedField);
			if(SelectedField != null)
				this.cbArea.SelectedItem = AreaToStr(GetSelectedArea(SelectedField));
		}
		protected virtual void PopulateCombo(PivotFieldItem field) {
			this.cbArea.Properties.Items.Clear();
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				if(field == null || field.IsAreaAllowed(area))
					this.cbArea.Properties.Items.Add(AreaToStr(area));
			}
		}
		protected virtual void OnButtonClick(object sender, EventArgs e) {
			MoveFieldToPivotGrid((PivotFieldItem)ActiveListBox.SelectedItem.Field);
		}
		protected override PivotArea GetSelectedArea(PivotFieldItem field) {
			PivotArea area;
			if(this.cbArea.SelectedItem != null) {
				area = StrToArea((string)this.cbArea.SelectedItem);
				if(field.IsAreaAllowed(area))
					return area;
			}
			return base.GetSelectedArea(field);
		}
		protected virtual PivotArea StrToArea(string localizedString) {
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				if(AreaToStr(area) == localizedString)
					return area;
			}
			throw new ArgumentException("localizedString");
		}
		protected virtual string AreaToStr(PivotArea area) {
			return GetLocalizedString(CustomizationFields.GetStringId(area));
		}
		public override void SetEnabledState(bool enabled) {
			base.SetEnabledState(enabled);
			AreaComboBox.Enabled = enabled;
		}
	}
}
