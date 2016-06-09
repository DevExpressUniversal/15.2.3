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
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraReports.Native;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.XtraReports.Serialization;
namespace DevExpress.XtraReports.Design {
	public class StyleSheetEditorForm : ReportsEditorFormBase {
		#region static
		static void DrawPreviewBorders(Graphics gr, Rectangle bounds, XRControlStyle style) {
			if(style == null)
				return;
			if(style.BorderWidth == 0)
				return;
			BorderSide borders = style.Borders;
			if(borders == BorderSide.None)
				return;
			SolidBrush br = new SolidBrush(style.BorderColor);
			using(PrintingSystemBase ps = new PrintingSystemBase()) {
				GdiGraphics gdiGraphics = new GdiGraphics(gr, ps);
				gdiGraphics.PageUnit = GraphicsUnit.Pixel;
				BrickPaint.DrawDashStyleBorders(gdiGraphics, style.BorderDashStyle, bounds, br, borders, style.BorderWidth);
			}
		}
		protected static void DrawPreview(Graphics gr, Rectangle bounds, string text, XRControlStyle style) {
			StringFormat format = StringFormat.GenericDefault.Clone() as StringFormat;
			format.FormatFlags = StringFormatFlags.NoWrap;
			using(format) {
				if(style == null) {
					format.Alignment = StringAlignment.Near;
					format.LineAlignment = StringAlignment.Near;
					gr.DrawString(text, XRControlStyle.DefaultFont, Brushes.Black, bounds, format);
				} else {
					TextAlignment textAlignment = style.TextAlignment;
					format.Alignment = GraphicsConvertHelper.ToHorzStringAlignment(textAlignment);
					format.LineAlignment = GraphicsConvertHelper.ToVertStringAlignment(textAlignment);
					Brush brush;
					if(style.IsSetBackColor)
						brush = new SolidBrush(style.BackColor);
					else
						brush = new SolidBrush(Color.White);
					using(brush) {
						gr.FillRectangle(brush, bounds);
					}
					Rectangle previewBounds = new Rectangle(
						bounds.X + previewBoundsOffset,
						bounds.Y + previewBoundsOffset,
						bounds.Width - previewBoundsOffset * 2,
						bounds.Height - previewBoundsOffset * 2);
					if((previewBounds.Width - style.BorderWidth) > 0 && (previewBounds.Height - style.BorderWidth) > 0) {
						DrawPreviewBorders(gr, previewBounds, style);
						Rectangle textBounds = new Rectangle(
							previewBounds.X + (int)Math.Ceiling(style.BorderWidth) + style.Padding.Left,
							previewBounds.Y + (int)Math.Ceiling(style.BorderWidth) + style.Padding.Top,
							previewBounds.Width - (int)Math.Ceiling(style.BorderWidth * 2) - style.Padding.Left - style.Padding.Right,
							previewBounds.Height - (int)Math.Ceiling(style.BorderWidth * 2) - style.Padding.Top - style.Padding.Bottom);
						if(textBounds.Width > 0 && textBounds.Height > 0) {
							Brush textBrush;
							textBrush = new SolidBrush(style.ForeColor);
							using(textBrush) {
								gr.DrawString(text, style.Font, textBrush, textBounds, format);
							}
						}
					}
				}
			}
		}
		#endregion
		private System.ComponentModel.IContainer components;
		private XtraReport report;
		private static string noStyleSelected;
		private static string moreThanOneStyle;
		private static string styleSheetError;
		private static string styleNamePreviewPostfix;
		private static string fileFilter;
		private LabelControl bottomLine;
		private BaseButton btnCancel;
		private LayoutItemButton btAdd;
		private LayoutItemButton btRemove;
		private LayoutItemButton btClear;
		private LayoutItemButton btPurge;
		private LabelControl separator;
		private LayoutItemButton btSave;
		private LayoutItemButton btLoad;
		private ListBoxControl lboxItems;
		private PanelControl pnlPreview;
		private PanelControl pnlPreviewContainer;
		private SplitContainerControl splitContainer1;
		private PropertyGridUserControl propertyGrid;
		private DevExpress.Utils.ImageCollection imageCollection;
		private static int previewBoundsOffset = 10;
		private IComponentChangeService componentChangeService;
		IDesignerHost designerHost;
		protected XtraReport Report { get { return report; } }
		public XRControlStyleSheet StyleSheet { get { return report.StyleSheet; } }
		protected ListBoxControl LboxItems { get { return lboxItems; } }
		protected PanelControl PnlPreview { get { return pnlPreview; } }
		protected PropertyGridUserControl PropertyGrid { get { return propertyGrid; } }
		protected DevExpress.XtraEditors.BaseButton BtnCancel { get { return btnCancel; } }
		protected LayoutItemButton BtAdd { get { return btAdd; } }
		protected LayoutItemButton BtRemove { get { return btRemove; } }
		protected LayoutItemButton BtClear { get { return btClear; } }
		protected LayoutItemButton BtPurge { get { return btPurge; } }
		protected IDesignerHost DesignerHost { get { return designerHost; } }
		protected IComponentChangeService ComponentChangeService { get { return componentChangeService; } }
		protected virtual bool VisibleSaveLoadButtons { get { return true; } }
		protected virtual string ReportPropertyName { get { return XRComponentPropertyNames.StyleSheet; } }
		public StyleSheetEditorForm(XtraReport report, IServiceProvider provider)
			: base(provider) {
			InitializeComponent();
			MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(pnlPreview, new Object[] { ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true });
			InitializeStrings();
			this.report = report as XtraReport;
			if(report.Site != null) {
				componentChangeService = (IComponentChangeService)report.Site.GetService(typeof(IComponentChangeService));
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
				designerHost = (IDesignerHost)report.Site.GetService(typeof(IDesignerHost));
			}
			propertyGrid.ServiceProvider = designerHost;
			propertyGrid.ShowDescription = true;
			propertyGrid.SetLookAndFeel(designerHost);
			UpdateItemsListBox();
			SetItemsListBoxSelectionOnInitialize();	
			UpdatePropertyGrid();
			UpdateButtons();
		}
		protected virtual void SetItemsListBoxSelectionOnInitialize() {
			SetItemsListBoxSelection(GetSelectedItemIndex(typeof(XRControlStyle), x => ((XRControlStyle)x).Name));   
		}
		protected int GetSelectedItemIndex(Type type, Func<object, string> func) {
			int firstItemIndex = 0;
			if(designerHost != null) {
				ISelectionService selectionService = (ISelectionService)designerHost.GetService(typeof(ISelectionService));
				if(selectionService != null && selectionService.PrimarySelection.GetType() == type) {
					firstItemIndex = lboxItems.Items.IndexOf(func(selectionService.PrimarySelection));
				}
			}
			return firstItemIndex;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				componentChangeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.btAdd = new LayoutItemButton();
			this.btRemove = new LayoutItemButton();
			this.btClear = new LayoutItemButton();
			this.btPurge = new LayoutItemButton();
			this.separator = new LabelControl();
			this.btSave = new LayoutItemButton();
			this.btLoad = new LayoutItemButton();
			this.imageCollection = new DevExpress.Utils.ImageCollection();
			this.bottomLine = new LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.BaseButton();
			this.lboxItems = new ListBoxControl();
			this.propertyGrid = new PropertyGridUserControl();
			this.pnlPreview = new PanelControl();
			this.pnlPreviewContainer = new PanelControl();
			this.splitContainer1 = new SplitContainerControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			this.imageCollection = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("Images.StyleSheetEditorToolbar.png"), LocalResFinder.Assembly, new Size(16, 16));
			this.btAdd.Location = new Point(8, 4);
			this.btAdd.Size = new Size(24, 24);
			this.btAdd.Image = this.imageCollection.Images[0];
			this.btAdd.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.btAdd.NotDrawButtonOnNormalState = true;
			this.btAdd.Click += new EventHandler(btAdd_Click);
			this.btRemove.Location = new Point(36, 4);
			this.btRemove.Size = new Size(24, 24);
			this.btRemove.Image = this.imageCollection.Images[1];
			this.btRemove.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.btRemove.NotDrawButtonOnNormalState = true;
			this.btRemove.Click += new EventHandler(btRemove_Click);
			this.btClear.Location = new Point(64, 4);
			this.btClear.Size = new Size(24, 24);
			this.btClear.Image = this.imageCollection.Images[2];
			this.btClear.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.btClear.NotDrawButtonOnNormalState = true;
			this.btClear.Click += new EventHandler(btClear_Click);
			this.btPurge.Location = new Point(92, 4);
			this.btPurge.Size = new Size(24, 24);
			this.btPurge.Image = this.imageCollection.Images[3];
			this.btPurge.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.btPurge.NotDrawButtonOnNormalState = true;
			this.btPurge.Click += new EventHandler(btPurge_Click);
			this.separator.AutoSizeMode = LabelAutoSizeMode.None;
			this.separator.Visible = true;
			this.separator.LineVisible = true;
			this.separator.LineOrientation = LabelLineOrientation.Vertical;
			this.separator.Location = new Point(121, 6);
			this.separator.Size = new Size(4, 20);
			this.separator.Visible = VisibleSaveLoadButtons;
			this.btSave.Location = new Point(156, 4);
			this.btSave.Size = new Size(24, 24);
			this.btSave.Image = this.imageCollection.Images[4];
			this.btSave.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.btSave.NotDrawButtonOnNormalState = true;
			this.btSave.Click += new EventHandler(btSave_Click);
			this.btSave.Visible = VisibleSaveLoadButtons;
			this.btLoad.Location = new Point(128, 4);
			this.btLoad.Size = new Size(24, 24);
			this.btLoad.Image = this.imageCollection.Images[5];
			this.btLoad.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.btLoad.NotDrawButtonOnNormalState = true;
			this.btLoad.Click += new EventHandler(btLoad_Click);
			this.btLoad.Visible = VisibleSaveLoadButtons;
			this.bottomLine.Visible = true;
			this.bottomLine.AutoSizeMode = LabelAutoSizeMode.None;
			this.bottomLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.bottomLine.LineVisible = true;
			this.bottomLine.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.bottomLine.Name = "bottomLine";
			this.bottomLine.Location = new System.Drawing.Point(0, 525);
			this.bottomLine.Size = new System.Drawing.Size(762, 6);
			this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new Point(672, 542);
			this.btnCancel.Size = new Size(74, 24);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.lboxItems.Dock = DockStyle.Fill;
			this.lboxItems.Name = "lboxStyles";
			this.lboxItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lboxItems.TabIndex = 0;
			this.lboxItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lboxStyles_KeyDown);
			this.lboxItems.SelectedValueChanged += new System.EventHandler(this.lboxStyles_SelectedValueChanged);
			this.propertyGrid.Dock = DockStyle.Fill; 
			this.propertyGrid.PropertyGridControl.CellValueChanged += new CellValueChangedEventHandler(propertyGrid_PropertyValueChanged);
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(578, 431);
			this.propertyGrid.TabIndex = 2;
			this.propertyGrid.Text = "propertyGrid1";
			this.pnlPreviewContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPreviewContainer.Dock = DockStyle.Bottom;
			this.pnlPreviewContainer.Controls.Add(this.pnlPreview);
			this.pnlPreviewContainer.Location = new System.Drawing.Point(0, 434);
			this.pnlPreviewContainer.Name = "pnlPreviewContainer";
			this.pnlPreviewContainer.Padding = new Padding(0, 3, 0, 0);
			this.pnlPreviewContainer.Size = new System.Drawing.Size(578, 52);
			this.pnlPreviewContainer.TabStop = false;
			this.pnlPreview.Dock = DockStyle.Bottom; 
			this.pnlPreview.Location = new System.Drawing.Point(0, 0);
			this.pnlPreview.Name = "pnlPreview";
			this.pnlPreview.Size = new System.Drawing.Size(578, 49);
			this.pnlPreview.TabStop = false;
			this.pnlPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPreview_Paint);
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right | AnchorStyles.Top);
			this.splitContainer1.Location = new System.Drawing.Point(8, 32);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Size = new System.Drawing.Size(746, 486);
			this.splitContainer1.TabStop = false;
			this.splitContainer1.Panel1.Controls.Add(this.lboxItems);
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainer1.Panel2.Controls.Add(this.pnlPreviewContainer);
			this.splitContainer1.SplitterPosition = 164;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(762, 581);
			this.ControlBox = true;
			this.ShowIcon = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.splitContainer1,
																		  this.btnCancel,
																		  this.bottomLine,
																		  this.btAdd, 
																		  this.btRemove,
																		  this.btClear,
																		  this.btPurge,
																		  this.separator,
																		  this.btSave,
																		  this.btLoad});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(340, 252);
			this.Name = "StyleSheetEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		void btLoad_Click(object sender, EventArgs e) {
			DoLoadAction();
		}
		void btSave_Click(object sender, EventArgs e) {
			DoSaveAction();
		}
		void btPurge_Click(object sender, EventArgs e) {
			DoPurgeAction();
		}
		void btClear_Click(object sender, EventArgs e) {
			DoClearAction();
		}
		void btRemove_Click(object sender, EventArgs e) {
			DoRemoveAction();
		}
		void btAdd_Click(object sender, EventArgs e) {
			DoAddAction();
		}
		#endregion
		protected virtual void InitializeStrings() {
			noStyleSelected = ReportLocalizer.GetString(ReportStringId.SSForm_Msg_NoStyleSelected);
			moreThanOneStyle = ReportLocalizer.GetString(ReportStringId.SSForm_Msg_MoreThanOneStyle);
			styleSheetError = ReportLocalizer.GetString(ReportStringId.SSForm_Msg_StyleSheetError);
			styleNamePreviewPostfix = ReportLocalizer.GetString(ReportStringId.SSForm_Msg_StyleNamePreviewPostfix);
			fileFilter = ReportLocalizer.GetString(ReportStringId.SSForm_Msg_FileFilter);
			btnCancel.Text = ReportLocalizer.GetString(ReportStringId.SSForm_Btn_Close);
			this.Text = ReportLocalizer.GetString(ReportStringId.SSForm_Caption);
			btAdd.ToolTip = ReportLocalizer.GetString(ReportStringId.SSForm_TTip_AddStyle);
			btRemove.ToolTip = ReportLocalizer.GetString(ReportStringId.SSForm_TTip_RemoveStyle);
			btClear.ToolTip = ReportLocalizer.GetString(ReportStringId.SSForm_TTip_ClearStyles);
			btPurge.ToolTip = ReportLocalizer.GetString(ReportStringId.SSForm_TTip_PurgeStyles);
			btLoad.ToolTip = ReportLocalizer.GetString(ReportStringId.SSForm_TTip_LoadStyles);
			btSave.ToolTip = ReportLocalizer.GetString(ReportStringId.SSForm_TTip_SaveStyles);
		}
		void UpdateItemsListBox() {
			List<string> temp = GetSheetList();
			lboxItems.BeginUpdate();
			try {
				temp.AddRange(GetItemsNames(report));
				using(var comparer = new DevExpress.Utils.NaturalStringComparer(temp.Count))
					temp.Sort(comparer);
				lboxItems.Items.Clear();
				lboxItems.Items.AddRange(temp.ToArray());
			} finally {
				lboxItems.EndUpdate();
			}
		}
		protected virtual List<string> GetSheetList() {
			return new List<string>(StyleSheet.Count);
		}
		protected virtual string[] GetItemsNames(XtraReport report) {
			return StyleSheet.GetNames();
		}
		void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			if(e.Component != null) {
				UpdateItemsListBox();
				int index = lboxItems.Items.IndexOf(GetComponentName(e.Component));
				SetItemsListBoxSelection(index);
			}
		}
		protected virtual string GetComponentName(object component) {
			XRControlStyle style = component as XRControlStyle;
			return style == null ? string.Empty : style.Name;
		}
		protected void UpdatePropertyGrid() {
			if(lboxItems.SelectedItems.Count == 0) {
				propertyGrid.SelectedObject = null;
				return;
			}
			propertyGrid.Enabled = !IsReadOnlyStylesInItems(lboxItems.SelectedItems);
			object[] selectedObjects = new object[lboxItems.SelectedItems.Count];
			for(int i = 0; i < selectedObjects.Length; i++) {
				selectedObjects[i] = GetObjectByName((string)lboxItems.SelectedItems[i]);
			}
			propertyGrid.SelectedObjects = selectedObjects;
			propertyGrid.ExpandProperty(TypeDescriptor.GetProperties(typeof(FormattingRule))["Formatting"].DisplayName);
		}
		protected bool IsReadOnlyStylesInItems(ICollection items) {
			foreach(string name in items) {
				if(GetObjectSite(GetObjectByName(name)) == null) {
					return true;
				}
			}
			return false;
		}
		protected virtual object GetObjectByName(string name) {
			return StyleSheet[name];
		}
		protected virtual ISite GetObjectSite(object item) {
			if(item is XRControlStyle)
				return ((XRControlStyle)item).Site;
			return null;
		}
		bool IsInheritedStylesInItems(ICollection items) {
			foreach(string styleName in items) {
				InheritanceAttribute att = (InheritanceAttribute)TypeDescriptor.GetAttributes(GetObjectByName(styleName))[typeof(InheritanceAttribute)];
				if(att.InheritanceLevel != InheritanceLevel.NotInherited) {
					return true;
				}
			}
			return false;
		}
		void OnComponentChanging() {
			if(componentChangeService != null)
				componentChangeService.OnComponentChanging(report, TypeDescriptor.GetProperties(report)[ReportPropertyName]);
		}
		void OnComponentChanged() {
			if(componentChangeService != null)
				componentChangeService.OnComponentChanged(report, TypeDescriptor.GetProperties(report)[ReportPropertyName], null, null);
		}
		protected bool RemoveSelectedItems() {
			if(lboxItems.SelectedItems.Count == 0)
				return false;
			for(int i = 0; i < lboxItems.SelectedItems.Count; i++) {
				string name = (string)lboxItems.SelectedItems[i];
				DestroyItem(name);
			}
			return true;
		}
		bool PurgeItems() {
			bool result = false;
			for(int i = 0; i < lboxItems.Items.Count; i++) {
				string name = (string)lboxItems.Items[i];
				if(CanPurgeItem(name)) {
					DestroyItem(name);
					result = true;
				}
			}
			return result;
		}
		protected void SetItemsListBoxSelection(int selectedIndex) {
			lboxItems.UnSelectAll();
			if(selectedIndex >= lboxItems.Items.Count)
				selectedIndex = lboxItems.Items.Count - 1;
			if(selectedIndex >= 0)
				lboxItems.SetSelected(selectedIndex, true);
		}
		void UpdateButtons() {
			btRemove.Enabled = CanDeleteItems(lboxItems.SelectedItems);
			btClear.Enabled = CanDeleteItems(lboxItems.Items);
			btPurge.Enabled = IsPureButtonEnabled();
			btSave.Enabled = lboxItems.Items.Count > 0;
		}
		bool CanDeleteItems(ICollection items) {
			if(items == null || items.Count == 0)
				return false;
			return !IsReadOnlyStylesInItems(items) && !IsInheritedStylesInItems(items);
		}
		bool IsPureButtonEnabled() {
			for(int i = 0; i < lboxItems.Items.Count; i++) {
				string itemName = (string)lboxItems.Items[i];
				if(CanPurgeItem(itemName))
					return true;
			}
			return false;
		}
		protected bool CanPurgeItem(string itemName) {
			object item = GetObjectByName(itemName);
			InheritanceAttribute atrt = (InheritanceAttribute)TypeDescriptor.GetAttributes(item)[typeof(InheritanceAttribute)];
			return !IsAttachedObject(item) && GetObjectSite(item) != null && atrt.InheritanceLevel == InheritanceLevel.NotInherited;
		}
		protected virtual bool IsAttachedObject(object item) {
			return report.IsAttachedStyle((XRControlStyle)item);
		}
		protected void DoCommonChanges() {
			UpdateButtons();
			UpdatePropertyGrid();
			OnComponentChanged();
			pnlPreview.Invalidate();
		}
		void DoRemoveAction() {
			if(!RemoveSelectedItems())
				return;
			OnComponentChanging();
			int selectedIndex = lboxItems.SelectedIndex;
			UpdateItemsListBox();
			SetItemsListBoxSelection(selectedIndex);
			DoCommonChanges();
		}
		protected void DoAddAction() {
			OnComponentChanging();
			string name = AddItem();
			UpdateItemsListBox();
			SetItemsListBoxSelection(LboxItems.Items.IndexOf(name));
			DoCommonChanges();
		}
		void DoClearAction() {
			if(XtraMessageBox.Show(DevExpress.LookAndFeel.DesignService.DesignLookAndFeelHelper.GetLookAndFeel(designerHost),
				ReportLocalizer.GetString(ReportStringId.Msg_WarningRemoveStyles),
				ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
			return;
			OnComponentChanging();
			ClearItems();
			UpdateItemsListBox();
			DoCommonChanges();
			SetItemsListBoxSelection(0);
		}
		protected virtual void ClearItems() {
			XRControlStyle[] styles = new XRControlStyle[StyleSheet.Count];
			StyleSheet.CopyTo(styles, 0);
			foreach(XRControlStyle style in styles) {
				designerHost.DestroyComponent(style);
			}
		}
		void DoPurgeAction() {
			if(!PurgeItems())
				return;
			OnComponentChanging();
			UpdateItemsListBox();
			SetItemsListBoxSelection(0);
			DoCommonChanges();
		}
		void DoSaveAction() {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = fileFilter;
			if(DialogRunner.ShowDialog(dlg) == DialogResult.OK)
				StyleSheet.SaveToFile(dlg.FileName);
		}
		void DoLoadAction() {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = fileFilter;
			if(DialogRunner.ShowDialog(dlg) == DialogResult.OK) {
				OnComponentChanging();
				try {
					StyleSheet.LoadFromFile(dlg.FileName);
					StyleSheet.AddStylesToContainer();
				} catch(Exception ex) {
					if(ex is XRSerializationException || ex is ArgumentException) {
						DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
						NotificationService.ShowException<XtraReport>(LookAndFeel, this, new Exception(ReportStringId.SSForm_Msg_InvalidFileFormat.GetString(), ex));
						return;
					}
				}
				UpdateItemsListBox();
				SetItemsListBoxSelection(0);
				DoCommonChanges();
			}
		}
		void lboxStyles_SelectedValueChanged(object sender, System.EventArgs e) {
			UpdateButtons();
			UpdatePropertyGrid();
			pnlPreview.Invalidate();
		}
		void propertyGrid_PropertyValueChanged(object s, EventArgs e) {
			OnComponentChanged();
			pnlPreview.Invalidate();
		}
		void pnlPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			PaintPreview(e);
		}
		protected virtual void PaintPreview(System.Windows.Forms.PaintEventArgs e) {
			Graphics gr = e.Graphics;
			string text = String.Empty;
			XRControlStyle style = null;
			if(lboxItems.SelectedItems.Count == 0)
				text = noStyleSelected;
			else if(lboxItems.SelectedItems.Count > 1)
				text = moreThanOneStyle;
			else {
				text = (string)lboxItems.SelectedItem;
				style = StyleSheet[text];
				text += styleNamePreviewPostfix;
			}
			DrawPreview(gr, pnlPreview.ClientRectangle, text, style);
		}
		void lboxStyles_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyData == Keys.Delete && btRemove.Enabled)
				DoRemoveAction();
			if(e.KeyData == Keys.Insert)
				DoAddAction();
		}
		protected virtual void DestroyItem(string name) {
			designerHost.DestroyComponent(StyleSheet[name]);
		}
		protected virtual string AddItem() {
			XRControlStyle style = new XRControlStyle();
			StyleSheet.Add(style);
			designerHost.Container.Add(style);
			return style.Name;
		}
	}
}
