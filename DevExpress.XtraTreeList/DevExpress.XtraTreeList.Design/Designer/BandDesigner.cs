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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraTreeList.Columns;
using System.Collections;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraTreeList.Helpers;
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)] 
	public partial class BandDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		readonly string defaultBandNamePrefix = "treeListBand";
		readonly int defaultMaxBandIndex = int.MaxValue;
		readonly Color defaultHighlightingColor = Color.FromArgb(100, 0, 255, 255);
		TreeListBand selectedBand;
		XtraTabControl tabControl;
		bool layoutUpdated = false;
		bool customizationFormVisible = false;
		public BandDesigner() {
			InitializeComponent();
		}
		protected TreeList EditingTreeList { get { return EditingObject as TreeList; } }
		protected TreeList PreviewTreeList { get { return treeListPreview; } }
		protected override string DescriptionText { get { return "You can add and delete bands and modify their settings. Add columns to bands using drag and drop."; } }
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("AutoWidth", ceAutoWidth.Checked);
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			ceAutoWidth.Checked = localStore.RestoreBoolProperty("AutoWidth", ceAutoWidth.Checked);
		}
		#region Initialization
		protected override void InitImages() {
			base.InitImages();
			btReset.Image = DesignerImages16.Images[DesignerImages16ResetIndex];
		}
		public override void InitComponent() {
			base.InitComponent();
			InitPreviewTreeList();
			InitControls();
		}
		protected virtual void InitControls() {
			btDel.Enabled = SelectedBand != null;
			UpdateAddNewBandButtonText(false);
			UpdateCustomizationButtonText();
			tabControl = FramesUtils.CreateTabProperty(this, new Control[] { pgMain, null }, new string[] { "Band properties", "Band options" });
			if(tabControl != null)
				tabControl.SelectedPageChanged += (sender, e) =>
				{
					e.Page.Controls.Add(pgMain);
					RefreshPropertyGrid();
				};
		}
		protected virtual void InitPreviewTreeList() {
			DevExpress.XtraTreeList.Design.TreeListAssign.TreeListControlAssign(EditingTreeList, PreviewTreeList, false, true);
			PreviewTreeList.OptionsView.ShowBandsMode = Utils.DefaultBoolean.True;
			PreviewTreeList.OptionsCustomization.AllowChangeBandParent = true;
			PreviewTreeList.OptionsCustomization.AllowChangeColumnParent = true;
			PreviewTreeList.OptionsView.AllowHtmlDrawHeaders = true;
			PreviewTreeList.OptionsView.AutoWidth = ceAutoWidth.Checked;
			AssignBands(EditingTreeList, PreviewTreeList);
			PreviewTreeList.OptionsView.AllowBandColumnsMultiRow = EditingTreeList.OptionsView.AllowBandColumnsMultiRow;
			SubsribePreviewTreeListEvents();
		}
		protected virtual void AssignBands(TreeList sourceTreeList, TreeList targetTreeList) {
			targetTreeList.BeginUpdate();
			try {
				ClearBands(targetTreeList);
				((IHeaderObjectCollection<TreeListColumn>)targetTreeList.Columns).Synchronize(sourceTreeList.Columns);
				((IHeaderObjectCollection<TreeListBand>)targetTreeList.Bands).Synchronize(sourceTreeList.Bands);
			}
			finally {
				targetTreeList.EndUpdate();
			}
		}
		void ClearBands(TreeList treeList) {
			List<TreeListBand> bands = GetAllBands(treeList.Bands);
			treeList.Bands.Clear();
			for(int n = bands.Count - 1; n >= 0; n--)
				bands[n].Dispose();
		}
		List<TreeListBand> GetAllBands(TreeListBandCollection bands) {
			List<TreeListBand> result = new List<TreeListBand>();
			return GetAllBandsCore(bands, result);
		}
		List<TreeListBand> GetAllBandsCore(TreeListBandCollection bands, List<TreeListBand> result) {
			foreach(TreeListBand band in bands) {
				result.Add(band);
				if(band.HasChildren) GetAllBandsCore(band.Bands, result);
			}
			return result;
		}
		protected virtual void SubsribePreviewTreeListEvents() {
			PreviewTreeList.MouseDown += OnPreviewTreeListMouseDown;
			PreviewTreeList.CustomDrawBandHeader += OnPreviewTreeListCustomDrawBandHeader;
			PreviewTreeList.ShowCustomizationForm += OnPreviewTreeListShowCustomizationForm;
			PreviewTreeList.HideCustomizationForm += OnPreviewTreeListHideCustomizationForm;
			PreviewTreeList.DragObjectDrop += OnPreviewTreeListDragObjectDrop;
			PreviewTreeList.Load += OnPreviewTreeListLoad;
		}
		protected virtual void UnsubscribePreviewTreeListEvents() {
			PreviewTreeList.MouseDown -= OnPreviewTreeListMouseDown;
			PreviewTreeList.CustomDrawBandHeader -= OnPreviewTreeListCustomDrawBandHeader;
			PreviewTreeList.ShowCustomizationForm -= OnPreviewTreeListShowCustomizationForm;
			PreviewTreeList.HideCustomizationForm -= OnPreviewTreeListHideCustomizationForm;
			PreviewTreeList.DragObjectDrop -= OnPreviewTreeListDragObjectDrop;
			PreviewTreeList.Load -= OnPreviewTreeListLoad;
			PreviewTreeList.LayoutUpdated -= OnPreviewTreeListLayoutUpdated;
		}
		#endregion
		protected override object SelectedObject { 
			get { 
				if(SelectedBand == null) return null;
				if(tabControl != null && tabControl.SelectedTabPageIndex == 1)
					return SelectedBand.OptionsBand;
				return new TreeListBandWrapper(SelectedBand); 
			} 
		}
		protected TreeListBand SelectedBand {
			get { return selectedBand; }
			set {
				if(SelectedBand == value) return;
				selectedBand = value;
				OnSelectedBandChanged();
			}
		}
		protected virtual void OnSelectedBandChanged() {
			btDel.Enabled = SelectedBand != null;
			RefreshPropertyGrid();
			PreviewTreeList.InvalidateBandPanel();
		}
		protected virtual TreeListBand GetNextBand(TreeListBand band) {
			TreeListBandCollection bands = PreviewTreeList.Bands;
			if(band != null && band.ParentBand != null) {
				bands = band.ParentBand.Bands;
				if(bands.Count <= 1) 
					return band.ParentBand;
			}
			int index = -1;
			if(band.Index > 0) index = band.Index - 1;
			if(band.Index == 0 && bands.Count > 1) index = 1;
			return (index > -1) ? bands[index] : null;
		}
		protected virtual TreeListBand AddBand() {
			TreeListBand band = CreateBand();
			PreviewTreeList.Bands.Add(band);
			return band;
		}
		protected virtual TreeListBand CreateBand() {
			TreeListBand band = PreviewTreeList.Bands.CreateBand();
			band.Caption = band.Name = GetBandName();
			return band;
		}
		protected virtual string GetBandName() {
			for(int i = 1; i < defaultMaxBandIndex; i++) {
				string name = string.Format("{0}{1}", defaultBandNamePrefix, i);
				if(!CheckNameExist(name)) return name;
			}
			return string.Empty;
		}
		protected virtual bool CheckNameExist(string name, TreeListBand excludeBand = null) {
			if(EditingTreeList.Site != null) {
				foreach(IComponent component in EditingTreeList.Site.Container.Components)
					if(name.ToLowerInvariant().Equals(component.Site.Name.ToLowerInvariant())) return true;
			}
			return CheckBandNameExist(name, PreviewTreeList.Bands, excludeBand);
		}
		protected virtual bool CheckBandNameExist(string name, TreeListBandCollection bands, TreeListBand excludeBand) {
			foreach(TreeListBand band in bands) {
				if(excludeBand != band && band.Name.ToLowerInvariant().Equals(name.ToLowerInvariant())) return true;
				if(band.HasChildren && CheckBandNameExist(name, band.Bands, excludeBand)) return true;
			}
			return false;
		}
		protected void UpdateCustomizationButtonText() {
			chColumns.Text = customizationFormVisible ? HideColumnsSelectorButtonText : ShowColumnsSelectorButtonText;
		}
		protected void UpdateAddNewBandButtonText(bool drag) {
			btAdd.Text = drag ? DragBandButtonText : AddNewButtonText;
		}
		protected void ceAutoWidth_CheckedChanged(object sender, EventArgs e) {
			PreviewTreeList.OptionsView.AutoWidth = ceAutoWidth.Checked;
		}
		protected override void Dispose(bool disposing) {
			UnsubscribePreviewTreeListEvents();
			if(layoutUpdated)
				AssignBands(PreviewTreeList, EditingTreeList);
			if(disposing && (components != null)) 
				components.Dispose();
			base.Dispose(disposing);
		}
		#region Handlers
		protected virtual void OnPreviewTreeListMouseDown(object sender, MouseEventArgs e) {
			TreeListHitInfo hi = ((TreeList)sender).CalcHitInfo(new Point(e.X, e.Y));
			if(hi != null && hi.HitInfoType == HitInfoType.Band && hi.Band != null)
				SelectedBand = hi.Band;
		}
		protected virtual void OnPreviewTreeListShowCustomizationForm(object sender, EventArgs e) {
			this.customizationFormVisible = true;
			UpdateCustomizationButtonText();
		}
		protected virtual void OnPreviewTreeListHideCustomizationForm(object sender, EventArgs e) {
			this.customizationFormVisible = false;
			UpdateCustomizationButtonText();
		}
		protected void btAdd_Click(object sender, EventArgs e) {
			SelectedBand = AddBand();
			UpdateAddNewBandButtonText(false);
		}
		protected void btDel_Click(object sender, EventArgs e) {
			if(SelectedBand != null) {
				TreeListBand nextBand = GetNextBand(SelectedBand);
				SelectedBand.Dispose();
				SelectedBand = nextBand;
			}
		}
		protected void btReset_Click(object sender, EventArgs e) {
			AssignBands(EditingTreeList, PreviewTreeList);
			layoutUpdated = false;
		}
		protected void chColumns_Click(object sender, EventArgs e) {
			if(this.customizationFormVisible)
				PreviewTreeList.DestroyCustomization();
			else
				PreviewTreeList.ColumnsCustomization();
		}
		protected virtual void OnPreviewTreeListDragObjectDrop(object sender, DragObjectDropEventArgs e) {
			UpdateAddNewBandButtonText(false);
		}
		protected void OnPreviewTreeListLayoutUpdated(object sender, EventArgs e) {
			layoutUpdated = true;
		}
		protected void OnPreviewTreeListLoad(object sender, EventArgs e) {
			layoutUpdated = false;
			PreviewTreeList.LayoutUpdated += OnPreviewTreeListLayoutUpdated;
		}
		protected void OnPreviewTreeListKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete)
				btDel_Click(this, EventArgs.Empty);
		}
		#endregion
		#region Custom draw
		protected virtual void OnPreviewTreeListCustomDrawBandHeader(object sender, CustomDrawBandHeaderEventArgs e) {
			if(e.Band != null && e.Band == SelectedBand && e.Band.Visible) {
				e.DefaultDraw();
				e.Graphics.FillRectangle(new SolidBrush(defaultHighlightingColor), e.Bounds);
				e.Handled = true;
			}
		}
		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e) {
			base.OnPropertyValueChanged(e);
			if(e.ChangedItem.Label == "Name") {
				string value = GetValidBandName(e.ChangedItem.Value);
				if(CheckBandNameExist(value, PreviewTreeList.Bands, GetSelectedBand())) {
					MessageBox.Show(string.Format(NameAlreadyExistsMessage, value), PropertyValueIsNotValidMessage, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					SetBandName(string.Format("{0}", e.OldValue));
				}
				else {
					if(!value.Equals(e.ChangedItem.Value))
						SetBandName(value);
					layoutUpdated = true;
				}
			}
		}
		string GetValidBandName(object val) {
			if(val == null) return GetBandName();
			return val.ToString().Replace(" ", "");
		}
		TreeListBand GetSelectedBand() {
			TreeListBandWrapper wrapper = pgMain.SelectedObject as TreeListBandWrapper;
			if(wrapper == null) return null;
			return wrapper.Band;
		}
		void SetBandName(string name) {
			TreeListBand band = GetSelectedBand();
			if(band == null) return;
			band.Name = name;
		}
		#endregion
		#region Dragging
		Point downPoint = Point.Empty;
		void btAdd_MouseMove(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				if(Math.Abs(e.X - downPoint.X) > SystemInformation.DragSize.Width || Math.Abs(e.Y - downPoint.Y) > SystemInformation.DragSize.Height) {
					UpdateAddNewBandButtonText(true);
					new TreeListBandDragHelper(PreviewTreeList).DragBand(CreateBand(), e);
				}
			}
		}
		private void btAdd_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) 
				downPoint = new Point(e.X, e.Y);
		}
		#endregion 
		#region Strings
		protected string AddNewButtonText = "Add New Band...",
			DragBandButtonText = "Drag the Band to the TreeList",
			HideColumnsSelectorButtonText = "&Hide Column/Band Selector",
			ShowColumnsSelectorButtonText = "&Show Column/Band Selector",
			NameAlreadyExistsMessage = "The name {0} is already in use by another component.",
			PropertyValueIsNotValidMessage = "Property value is not valid.";
		#endregion
		#region TreeListBandWrapper
		public class TreeListBandWrapper : ICustomTypeDescriptor {
			public TreeListBandWrapper(TreeListBand band) {
				Band = band;
			}
			public TreeListBand Band { get; private set; }
			System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
				return TypeDescriptor.GetAttributes(Band.GetType());
			}
			string ICustomTypeDescriptor.GetClassName() {
				return Band.GetType().Name;
			}
			string ICustomTypeDescriptor.GetComponentName() {
				return Band.GetType().FullName;
			}
			TypeConverter ICustomTypeDescriptor.GetConverter() {
				return TypeDescriptor.GetConverter(Band);
			}
			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
				return TypeDescriptor.GetDefaultEvent(Band);
			}
			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
				return TypeDescriptor.GetDefaultProperty(Band);
			}
			object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
				return TypeDescriptor.GetEditor(typeof(Object), editorBaseType);
			}
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
				return EventDescriptorCollection.Empty;
			}
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attribute) {
				return EventDescriptorCollection.Empty;
			}
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
				PropertyDescriptorCollection sourceProperies = TypeDescriptor.GetProperties(Band);
				ArrayList propertiesList = new ArrayList(sourceProperies);
				PropertyDescriptor nameProperty = sourceProperies["Name"];
				propertiesList.Remove(nameProperty);
				propertiesList.Add(TypeDescriptor.CreateProperty(typeof(TreeListBand), nameProperty, new BrowsableAttribute(true), new CategoryAttribute("Name")));
				PropertyDescriptorCollection collection = new PropertyDescriptorCollection(propertiesList.ToArray(typeof(PropertyDescriptor)) as PropertyDescriptor[]);
				return collection;
			}
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
				return ((ICustomTypeDescriptor)this).GetProperties();
			}
			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
				return Band;
			}
		}
		#endregion
	}
}
