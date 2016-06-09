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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System.Data;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.spnResourcesPerPage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.chkPrintCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lblResourcesKind")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lblResourcesPerPage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lbResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lbCustomResourceCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.cbGroupBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lblGroupBy")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.btnToCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.btnFromCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.btnAllToCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.btnAllFromCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.btnMoveUp")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.btnMoveDown")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.cbResourcesKind")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.chkPrintAllResourcesOnOnePage")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.chkUseActiveViewGroupType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.grpCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lblCustomResourcesCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl.lblAvailableResource")]
#endregion
namespace DevExpress.XtraScheduler.Design {
	public enum ListBoxUpdateType {
		None,
		KeepSelectedIndices,
		KeepSelectedResources
	};
	[DXToolboxItem(false)]
	public class PageSetupResourcesTabControl : DevExpress.XtraEditors.XtraUserControl {
		#region Fields
		protected SpinEdit spnResourcesPerPage;
		protected CheckEdit chkPrintCustomCollection;
		protected DevExpress.XtraEditors.LabelControl lblResourcesKind;
		protected DevExpress.XtraEditors.LabelControl lblResourcesPerPage;
		protected ListBoxControl lbResources;
		protected ListBoxControl lbCustomResourceCollection;
		IContainer components = null;
		ResourceOptions resourceOptions;
		protected ImageComboBoxEdit cbGroupBy;
		protected DevExpress.XtraEditors.LabelControl lblGroupBy;
		protected SimpleButton btnToCustomCollection;
		protected SimpleButton btnFromCustomCollection;
		protected SimpleButton btnAllToCustomCollection;
		protected SimpleButton btnAllFromCustomCollection;
		protected SimpleButton btnMoveUp;
		protected SimpleButton btnMoveDown;
		protected ImageComboBoxEdit cbResourcesKind;
		protected CheckEdit chkPrintAllResourcesOnOnePage;
		protected CheckEdit chkUseActiveViewGroupType;
		protected GroupControl grpCustomCollection;
		protected DevExpress.XtraEditors.LabelControl lblCustomResourcesCollection;
		protected DevExpress.XtraEditors.LabelControl lblAvailableResource;
		ISchedulerStorageBase storage;
		#endregion
		public PageSetupResourcesTabControl() {
			InitializeComponent();
			FillResourcesKind();
			FillGroupBy();
			SubscribeEvents();
			this.resourceOptions = null;
			this.storage = null;
		}
		#region Properties
		public ISchedulerStorageBase Storage {
			get { return storage; }
			set {
				if (storage == value)
					return;
				storage = value;
				UpdateData(ListBoxUpdateType.None);
			}
		}
		public ResourceOptions ResourceOptions {
			get { return resourceOptions; }
			set {
				if (value == null) {
					this.Enabled = false;
					resourceOptions = null;
					return;
				}
				if (resourceOptions == value)
					return;
				this.Enabled = true;
				resourceOptions = value;
				UpdateData(ListBoxUpdateType.None);
			}
		}
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupResourcesTabControl));
			this.spnResourcesPerPage = new DevExpress.XtraEditors.SpinEdit();
			this.chkPrintCustomCollection = new DevExpress.XtraEditors.CheckEdit();
			this.lbResources = new DevExpress.XtraEditors.ListBoxControl();
			this.lbCustomResourceCollection = new DevExpress.XtraEditors.ListBoxControl();
			this.lblResourcesKind = new DevExpress.XtraEditors.LabelControl();
			this.lblResourcesPerPage = new DevExpress.XtraEditors.LabelControl();
			this.btnToCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnFromCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnAllToCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnAllFromCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnMoveUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnMoveDown = new DevExpress.XtraEditors.SimpleButton();
			this.cbGroupBy = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lblGroupBy = new DevExpress.XtraEditors.LabelControl();
			this.cbResourcesKind = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.chkPrintAllResourcesOnOnePage = new DevExpress.XtraEditors.CheckEdit();
			this.chkUseActiveViewGroupType = new DevExpress.XtraEditors.CheckEdit();
			this.grpCustomCollection = new DevExpress.XtraEditors.GroupControl();
			this.lblAvailableResource = new DevExpress.XtraEditors.LabelControl();
			this.lblCustomResourcesCollection = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.spnResourcesPerPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintCustomCollection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbResources)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCustomResourceCollection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbGroupBy.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbResourcesKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintAllResourcesOnOnePage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUseActiveViewGroupType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpCustomCollection)).BeginInit();
			this.grpCustomCollection.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.spnResourcesPerPage, "spnResourcesPerPage");
			this.spnResourcesPerPage.Name = "spnResourcesPerPage";
			this.spnResourcesPerPage.Properties.AccessibleName = resources.GetString("spnResourcesPerPage.Properties.AccessibleName");
			this.spnResourcesPerPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnResourcesPerPage.Properties.IsFloatValue = false;
			this.spnResourcesPerPage.Properties.Mask.EditMask = resources.GetString("spnResourcesPerPage.Properties.Mask.EditMask");
			this.spnResourcesPerPage.Properties.MaxLength = 2;
			this.spnResourcesPerPage.Properties.MaxValue = new decimal(new int[] {
			99,
			0,
			0,
			0});
			this.spnResourcesPerPage.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.chkPrintCustomCollection, "chkPrintCustomCollection");
			this.chkPrintCustomCollection.Name = "chkPrintCustomCollection";
			this.chkPrintCustomCollection.Properties.AutoWidth = true;
			this.chkPrintCustomCollection.Properties.Caption = resources.GetString("chkPrintCustomCollection.Properties.Caption");
			resources.ApplyResources(this.lbResources, "lbResources");
			this.lbResources.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbResources.Name = "lbResources";
			this.lbResources.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			resources.ApplyResources(this.lbCustomResourceCollection, "lbCustomResourceCollection");
			this.lbCustomResourceCollection.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbCustomResourceCollection.Name = "lbCustomResourceCollection";
			this.lbCustomResourceCollection.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			resources.ApplyResources(this.lblResourcesKind, "lblResourcesKind");
			this.lblResourcesKind.Name = "lblResourcesKind";
			resources.ApplyResources(this.lblResourcesPerPage, "lblResourcesPerPage");
			this.lblResourcesPerPage.Name = "lblResourcesPerPage";
			resources.ApplyResources(this.btnToCustomCollection, "btnToCustomCollection");
			this.btnToCustomCollection.Name = "btnToCustomCollection";
			resources.ApplyResources(this.btnFromCustomCollection, "btnFromCustomCollection");
			this.btnFromCustomCollection.Name = "btnFromCustomCollection";
			resources.ApplyResources(this.btnAllToCustomCollection, "btnAllToCustomCollection");
			this.btnAllToCustomCollection.Name = "btnAllToCustomCollection";
			resources.ApplyResources(this.btnAllFromCustomCollection, "btnAllFromCustomCollection");
			this.btnAllFromCustomCollection.Name = "btnAllFromCustomCollection";
			resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
			this.btnMoveUp.Name = "btnMoveUp";
			resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
			this.btnMoveDown.Name = "btnMoveDown";
			resources.ApplyResources(this.cbGroupBy, "cbGroupBy");
			this.cbGroupBy.Name = "cbGroupBy";
			this.cbGroupBy.Properties.AccessibleName = resources.GetString("cbGroupBy.Properties.AccessibleName");
			this.cbGroupBy.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbGroupBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbGroupBy.Properties.Buttons"))))});
			resources.ApplyResources(this.lblGroupBy, "lblGroupBy");
			this.lblGroupBy.Name = "lblGroupBy";
			resources.ApplyResources(this.cbResourcesKind, "cbResourcesKind");
			this.cbResourcesKind.Name = "cbResourcesKind";
			this.cbResourcesKind.Properties.AccessibleName = resources.GetString("cbResourcesKind.Properties.AccessibleName");
			this.cbResourcesKind.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbResourcesKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbResourcesKind.Properties.Buttons"))))});
			resources.ApplyResources(this.chkPrintAllResourcesOnOnePage, "chkPrintAllResourcesOnOnePage");
			this.chkPrintAllResourcesOnOnePage.Name = "chkPrintAllResourcesOnOnePage";
			this.chkPrintAllResourcesOnOnePage.Properties.AutoWidth = true;
			this.chkPrintAllResourcesOnOnePage.Properties.Caption = resources.GetString("chkPrintAllResourcesOnOnePage.Properties.Caption");
			resources.ApplyResources(this.chkUseActiveViewGroupType, "chkUseActiveViewGroupType");
			this.chkUseActiveViewGroupType.Name = "chkUseActiveViewGroupType";
			this.chkUseActiveViewGroupType.Properties.AutoWidth = true;
			this.chkUseActiveViewGroupType.Properties.Caption = resources.GetString("chkUseActiveViewGroupType.Properties.Caption");
			this.grpCustomCollection.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpCustomCollection.Controls.Add(this.lblAvailableResource);
			this.grpCustomCollection.Controls.Add(this.lblCustomResourcesCollection);
			this.grpCustomCollection.Controls.Add(this.lbResources);
			this.grpCustomCollection.Controls.Add(this.btnToCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnAllToCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnAllFromCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnFromCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnMoveUp);
			this.grpCustomCollection.Controls.Add(this.btnMoveDown);
			this.grpCustomCollection.Controls.Add(this.lbCustomResourceCollection);
			resources.ApplyResources(this.grpCustomCollection, "grpCustomCollection");
			this.grpCustomCollection.Name = "grpCustomCollection";
			resources.ApplyResources(this.lblAvailableResource, "lblAvailableResource");
			this.lblAvailableResource.Name = "lblAvailableResource";
			resources.ApplyResources(this.lblCustomResourcesCollection, "lblCustomResourcesCollection");
			this.lblCustomResourcesCollection.Name = "lblCustomResourcesCollection";
			this.Controls.Add(this.grpCustomCollection);
			this.Controls.Add(this.chkUseActiveViewGroupType);
			this.Controls.Add(this.chkPrintAllResourcesOnOnePage);
			this.Controls.Add(this.cbResourcesKind);
			this.Controls.Add(this.lblGroupBy);
			this.Controls.Add(this.cbGroupBy);
			this.Controls.Add(this.lblResourcesPerPage);
			this.Controls.Add(this.lblResourcesKind);
			this.Controls.Add(this.chkPrintCustomCollection);
			this.Controls.Add(this.spnResourcesPerPage);
			this.Name = "PageSetupResourcesTabControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.spnResourcesPerPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintCustomCollection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbResources)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCustomResourceCollection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbGroupBy.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbResourcesKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintAllResourcesOnOnePage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUseActiveViewGroupType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpCustomCollection)).EndInit();
			this.grpCustomCollection.ResumeLayout(false);
			this.grpCustomCollection.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		#region Fill... methods
		protected internal virtual void FillResourcesKind() {
			cbResourcesKind.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_AllResources), ResourcesKind.All));
			cbResourcesKind.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_VisibleResources), ResourcesKind.Visible));
			cbResourcesKind.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_OnScreenResources), ResourcesKind.OnScreen));
		}
		protected internal virtual void FillGroupBy() {
			cbGroupBy.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_GroupByNone), SchedulerGroupType.None));
			cbGroupBy.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_GroupByDate), SchedulerGroupType.Date));
			cbGroupBy.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_GroupByResources), SchedulerGroupType.Resource));
		}
		#endregion
		#region Update data
		protected internal virtual void UpdateData(ListBoxUpdateType updateType) {
			if (ResourceOptions == null || Storage == null)
				return;
			UnsubscribeEvents();
			UpdateListBoxes(updateType);
			UpdateResourcesKind();
			UpdateResourcesPerPage();
			UpdateGroupBy();
			UpdatePrintCustomCollection();
			UpdateNavigationButtonState();
			SubscribeEvents();
		}
		protected internal virtual void SubscribeEvents() {
			spnResourcesPerPage.EditValueChanged += new System.EventHandler(OnSpnResourcesPerPageEditValueChanged);
			chkPrintCustomCollection.CheckedChanged += new System.EventHandler(OnChkPrintCustomCollectionCheckedChanged);
			btnToCustomCollection.Click += new System.EventHandler(OnBtnToCustomCollectionClick);
			btnFromCustomCollection.Click += new System.EventHandler(OnBtnFromCustomCollectionClick);
			btnAllToCustomCollection.Click += new System.EventHandler(OnBtnAllToCustomCollectionClick);
			btnAllFromCustomCollection.Click += new System.EventHandler(OnBtnAllFromCustomCollectionClick);
			btnMoveUp.Click += new System.EventHandler(OnBtnMoveUpClick);
			btnMoveDown.Click += new System.EventHandler(OnBtnMoveDownClick);
			cbGroupBy.SelectedIndexChanged += new System.EventHandler(OnCbGroupBySelectedIndexChanged);
			cbResourcesKind.SelectedIndexChanged += new System.EventHandler(OnCbResourcesKindSelectedIndexChanged);
			chkPrintAllResourcesOnOnePage.CheckedChanged += new System.EventHandler(OnChkPrintAllResourcesOnOnePageCheckedChanged);
			chkUseActiveViewGroupType.CheckedChanged += new System.EventHandler(OnChkUseActiveViewGroupTypeCheckedChanged);
		}
		protected internal virtual void UnsubscribeEvents() {
			spnResourcesPerPage.EditValueChanged -= new System.EventHandler(OnSpnResourcesPerPageEditValueChanged);
			chkPrintCustomCollection.CheckedChanged -= new System.EventHandler(OnChkPrintCustomCollectionCheckedChanged);
			btnToCustomCollection.Click -= new System.EventHandler(OnBtnToCustomCollectionClick);
			btnFromCustomCollection.Click -= new System.EventHandler(OnBtnFromCustomCollectionClick);
			btnAllToCustomCollection.Click -= new System.EventHandler(OnBtnAllToCustomCollectionClick);
			btnAllFromCustomCollection.Click -= new System.EventHandler(OnBtnAllFromCustomCollectionClick);
			btnMoveUp.Click -= new System.EventHandler(OnBtnMoveUpClick);
			btnMoveDown.Click -= new System.EventHandler(OnBtnMoveDownClick);
			cbGroupBy.SelectedIndexChanged -= new System.EventHandler(OnCbGroupBySelectedIndexChanged);
			cbResourcesKind.SelectedIndexChanged -= new System.EventHandler(OnCbResourcesKindSelectedIndexChanged);
			chkPrintAllResourcesOnOnePage.CheckedChanged -= new System.EventHandler(OnChkPrintAllResourcesOnOnePageCheckedChanged);
			chkUseActiveViewGroupType.CheckedChanged -= new System.EventHandler(OnChkUseActiveViewGroupTypeCheckedChanged);
		}
		protected internal virtual void UpdateListBoxes(ListBoxUpdateType updateType) {
			UpdateResourcesListBox(updateType);
			UpdateCustomResourcesListBox(updateType);
		}
		protected internal virtual void UpdateResourcesListBox(ListBoxUpdateType updateType) {
			ResourceBaseCollection resources = GetResources();
			ResourceBaseCollection customResourcesCollection = GetCustomResourcesCollection();
			int count = resources.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (customResourcesCollection.ResourceExists(resources[i].Id))
					resources.RemoveAt(i);
			}
			UpdateListBox(lbResources, resources, updateType);
		}
		protected internal virtual ResourceBaseCollection GetCustomResourcesCollection() {
			ResourceBaseCollection customResourcesCollection = ResourceOptions.CustomResourcesCollection;
			return customResourcesCollection;
		}
		protected internal virtual ResourceBaseCollection GetResources() {
			ResourceBaseCollection resources = new ResourceBaseCollection();
			for (int i = 0; i < Storage.Resources.Count; i++) {
				resources.Add(Storage.Resources[i]);
			}
			return resources;
		}
		protected internal virtual void UpdateCustomResourcesListBox(ListBoxUpdateType updateType) {
			ResourceBaseCollection resources = GetResources();
			ResourceBaseCollection customResourcesCollection = GetCustomResourcesCollection();
			ResourceBaseCollection result = new ResourceBaseCollection();
			int count = customResourcesCollection.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = customResourcesCollection[i];
				if (resources.ResourceExists(resource.Id))
					result.Add(resource);
			}
			UpdateListBox(lbCustomResourceCollection, result, updateType);
		}
		protected internal virtual void UpdateListBox(ListBoxControl listBox, ResourceBaseCollection resources, ListBoxUpdateType updateType) {
			ResourceBaseCollection selectedResources = GetSelectedResources(listBox, updateType);
			int[] selectedIndicies = GetSelectedIndices(listBox, updateType);
			listBox.BeginUpdate();
			try {
				listBox.Items.Clear();
				int count = resources.Count;
				for (int i = 0; i < count; i++)
					listBox.Items.Add(new ObjectWrapper(resources[i], resources[i].Caption));
				listBox.SetSelected(0, false);
				switch (updateType) {
					case ListBoxUpdateType.KeepSelectedResources:
						RestoreListBoxItemSelection(listBox, selectedResources);
						break;
					case ListBoxUpdateType.KeepSelectedIndices:
						RestoreListBoxItemSelection(listBox, selectedIndicies);
						break;
				}
				if (listBox.SelectedIndices.Count == 0)
					listBox.SetSelected(0, true);
			}
			finally {
				listBox.EndUpdate();
			}
		}
		protected internal virtual void RestoreListBoxItemSelection(ListBoxControl listBox, ResourceBaseCollection selectedResources) {
			XtraSchedulerDebug.Assert(listBox.SelectedIndices.Count == 0);
			int listBoxItemsCount = listBox.Items.Count;
			for (int i = 0; i < listBoxItemsCount; i++) {
				Resource resource = (Resource)((ObjectWrapper)listBox.Items[i]).Object;
				if (selectedResources.Contains(resource))
					listBox.SetSelected(i, true);
			}
		}
		protected internal virtual void RestoreListBoxItemSelection(ListBoxControl listBox, int[] selectedIndicies) {
			XtraSchedulerDebug.Assert(listBox.SelectedIndices.Count == 0);
			int listBoxItemsCount = listBox.Items.Count;
			int selectedIndiciesLength = selectedIndicies.Length;
			for (int i = 0; i < selectedIndiciesLength; i++) {
				int newSelectedIndex = selectedIndicies[i];
				if (newSelectedIndex > listBoxItemsCount)
					break;
				listBox.SetSelected(newSelectedIndex, true);
			}
		}
		protected internal virtual ResourceBaseCollection GetSelectedResources(ListBoxControl listBoxControl, ListBoxUpdateType updateType) {
			XtraSchedulerDebug.Assert(listBoxControl != null);
			bool useSelectedResources = (updateType == ListBoxUpdateType.KeepSelectedResources);
			if (!useSelectedResources)
				return null;
			int count = listBoxControl.SelectedIndices.Count;
			ResourceBaseCollection resources = new ResourceBaseCollection();
			for (int i = 0; i < count; i++) {
				int index = listBoxControl.SelectedIndices[i];
				ObjectWrapper objectWrapper = (ObjectWrapper)listBoxControl.Items[index];
				resources.Add((Resource)objectWrapper.Object);
			}
			return resources;
		}
		protected internal virtual int[] GetSelectedIndices(ListBoxControl listBox, ListBoxUpdateType updateType) {
			bool useSelectedIndicies = (updateType == ListBoxUpdateType.KeepSelectedIndices);
			if (!useSelectedIndicies)
				return null;
			int[] indicies = GetSelectedIndicesCore(listBox);
			return indicies;
		}
		protected internal virtual void UpdateResourcesKind() {
			ResourcesKind resourcesKind = ResourceOptions.ResourcesKind;
			SetComboboxSelectedValue(cbResourcesKind, resourcesKind);
		}
		protected internal virtual void UpdateResourcesPerPage() {
			EnableResourcesPerPage(!ResourceOptions.PrintAllResourcesOnOnePage);
			chkPrintAllResourcesOnOnePage.Checked = ResourceOptions.PrintAllResourcesOnOnePage;
			spnResourcesPerPage.Value = ResourceOptions.ResourcesPerPage;
		}
		protected internal virtual void EnableResourcesPerPage(bool enable) {
			spnResourcesPerPage.Enabled = enable;
		}
		protected internal virtual void UpdateGroupBy() {
			SchedulerGroupType groupType = ResourceOptions.GroupType;
			SetComboboxSelectedValue(cbGroupBy, groupType);
			chkUseActiveViewGroupType.Checked = ResourceOptions.UseActiveViewGroupType;
			cbGroupBy.Enabled = !ResourceOptions.UseActiveViewGroupType;
		}
		protected internal virtual void SetComboboxSelectedValue(ImageComboBoxEdit comboBox, object value) {
			ImageComboBoxItemCollection items = comboBox.Properties.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ImageComboBoxItem item = items[i];
				if (item.Value.Equals(value)) {
					comboBox.SelectedIndex = i;
					break;
				}
			}
		}
		protected internal virtual void UpdatePrintCustomCollection() {
			chkPrintCustomCollection.Checked = ResourceOptions.PrintCustomCollection;
			grpCustomCollection.Enabled = ResourceOptions.PrintCustomCollection;
		}
		protected internal virtual void UpdateNavigationButtonState() {
			bool isLbCustomResourceCollectionItemsSelected = IsListBoxItemsSelected(lbCustomResourceCollection);
			btnMoveUp.Enabled = isLbCustomResourceCollectionItemsSelected;
			btnMoveDown.Enabled = isLbCustomResourceCollectionItemsSelected;
			btnFromCustomCollection.Enabled = isLbCustomResourceCollectionItemsSelected;
			btnAllFromCustomCollection.Enabled = lbCustomResourceCollection.Items.Count != 0;
			btnToCustomCollection.Enabled = IsListBoxItemsSelected(lbResources);
			btnAllToCustomCollection.Enabled = lbResources.Items.Count != 0;
		}
		protected internal virtual bool IsListBoxItemsSelected(ListBoxControl listBox) {
			return listBox.SelectedIndices.Count != 0 && listBox.Enabled;
		}
		#endregion
		protected internal virtual void OnChkPrintAllResourcesOnOnePageCheckedChanged(object sender, System.EventArgs e) {
			ResourceOptions.PrintAllResourcesOnOnePage = chkPrintAllResourcesOnOnePage.Checked;
			UpdateData(ListBoxUpdateType.KeepSelectedResources);
		}
		protected internal virtual void OnChkPrintCustomCollectionCheckedChanged(object sender, System.EventArgs e) {
			ResourceOptions.PrintCustomCollection = chkPrintCustomCollection.Checked;
			UpdateData(ListBoxUpdateType.KeepSelectedResources);
		}
		protected internal virtual void OnChkUseActiveViewGroupTypeCheckedChanged(object sender, System.EventArgs e) {
			ResourceOptions.UseActiveViewGroupType = chkUseActiveViewGroupType.Checked;
			UpdateData(ListBoxUpdateType.KeepSelectedResources);
		}
		protected internal virtual void OnBtnToCustomCollectionClick(object sender, System.EventArgs e) {
			ResourceBaseCollection customResourcesCollections = GetCustomResourcesCollection();
			int count = lbResources.SelectedIndices.Count;
			for (int i = 0; i < count; i++) {
				int index = lbResources.SelectedIndices[i];
				ObjectWrapper listBoxItem = (ObjectWrapper)lbResources.Items[index];
				customResourcesCollections.Add((Resource)listBoxItem.Object);
			}
			UpdateData(ListBoxUpdateType.None);
		}
		protected internal virtual void OnBtnFromCustomCollectionClick(object sender, System.EventArgs e) {
			ResourceBaseCollection customResourcesCollections = GetCustomResourcesCollection();
			int count = lbCustomResourceCollection.SelectedIndices.Count;
			for (int i = 0; i < count; i++) {
				int index = lbCustomResourceCollection.SelectedIndices[i];
				ObjectWrapper listBoxItem = (ObjectWrapper)lbCustomResourceCollection.Items[index];
				customResourcesCollections.Remove((Resource)listBoxItem.Object);
			}
			UpdateData(ListBoxUpdateType.None);
		}
		protected internal virtual void OnBtnAllToCustomCollectionClick(object sender, System.EventArgs e) {
			ResourceBaseCollection resources = GetResources();
			ResourceBaseCollection customResources = GetCustomResourcesCollection();
			customResources.AddRange(resources);
			UpdateData(ListBoxUpdateType.None);
		}
		protected internal virtual void OnBtnAllFromCustomCollectionClick(object sender, System.EventArgs e) {
			ResourceBaseCollection customResource = GetCustomResourcesCollection();
			customResource.Clear();
			UpdateData(ListBoxUpdateType.None);
		}
		protected internal virtual void OnBtnMoveUpClick(object sender, System.EventArgs e) {
			int[] selectedIndices = GetSelectedIndicesCore(lbCustomResourceCollection);
			if (selectedIndices[0] <= 0)
				return;
			ResourceBaseCollection customResourceCollection = GetCustomResourcesCollection();
			int count = lbCustomResourceCollection.SelectedIndices.Count;
			for (int i = 0; i < count; i++) {
				int index = selectedIndices[i];
				Resource resource = GetResourceFromListBoxByIndex(lbCustomResourceCollection, index);
				MoveUpResourceCollectionItem(customResourceCollection, resource);
			}
			UpdateData(ListBoxUpdateType.KeepSelectedResources);
		}
		protected internal virtual Resource GetResourceFromListBoxByIndex(ListBoxControl listBox, int index) {
			ObjectWrapper objectWrapper = (ObjectWrapper)listBox.Items[index];
			XtraSchedulerDebug.Assert(objectWrapper != null);
			Resource resource = (Resource)objectWrapper.Object;
			XtraSchedulerDebug.Assert(resource != null);
			return resource;
		}
		protected internal virtual void MoveUpResourceCollectionItem(ResourceBaseCollection resources, Resource resource) {
			int resourceIndex = resources.IndexOf(resource);
			XtraSchedulerDebug.Assert(resourceIndex != 0);
			XtraSchedulerDebug.Assert(resourceIndex != -1);
			resources.RemoveAt(resourceIndex);
			resources.Insert(resourceIndex - 1, resource);
		}
		protected internal virtual int[] GetSelectedIndicesCore(ListBoxControl listBox) {
			int count = listBox.SelectedIndices.Count;
			int[] selectedIndices = new int[count];
			listBox.SelectedIndices.CopyTo(selectedIndices, 0);
			Array.Sort(selectedIndices);
			return selectedIndices;
		}
		protected internal virtual void OnBtnMoveDownClick(object sender, System.EventArgs e) {
			int[] selectedIndices = GetSelectedIndicesCore(lbCustomResourceCollection);
			int selectedIndicesCount = selectedIndices.Length;
			ResourceBaseCollection customResourceCollection = GetCustomResourcesCollection();
			if (selectedIndices[selectedIndicesCount - 1] >= customResourceCollection.Count - 1)
				return;
			for (int i = selectedIndicesCount - 1; i >= 0; i--) {
				int index = selectedIndices[i];
				Resource resource = GetResourceFromListBoxByIndex(lbCustomResourceCollection, index);
				MoveDownResourceCollectionItem(customResourceCollection, resource);
			}
			UpdateData(ListBoxUpdateType.KeepSelectedResources);
		}
		protected internal virtual void MoveDownResourceCollectionItem(ResourceBaseCollection customResourceCollection, Resource resource) {
			int resourceIndex = customResourceCollection.IndexOf(resource);
			customResourceCollection.RemoveAt(resourceIndex);
			customResourceCollection.Insert(resourceIndex + 1, resource);
		}
		protected internal virtual void OnCbResourcesKindSelectedIndexChanged(object sender, System.EventArgs e) {
			ImageComboBoxItem selectedItem = (ImageComboBoxItem)cbResourcesKind.SelectedItem;
			ResourceOptions.ResourcesKind = (ResourcesKind)selectedItem.Value;
			UpdateData(ListBoxUpdateType.None);
		}
		protected internal virtual void OnSpnResourcesPerPageEditValueChanged(object sender, System.EventArgs e) {
			ResourceOptions.ResourcesPerPage = Convert.ToInt32(spnResourcesPerPage.Value);
			UpdateData(ListBoxUpdateType.None);
		}
		protected internal virtual void OnCbGroupBySelectedIndexChanged(object sender, System.EventArgs e) {
			ImageComboBoxItem selectedItem = (ImageComboBoxItem)cbGroupBy.SelectedItem;
			ResourceOptions.GroupType = (SchedulerGroupType)selectedItem.Value;
			UpdateData(ListBoxUpdateType.None);
		}
		public void SwitchToDesignTimeMode() {
			chkPrintCustomCollection.Enabled = false;
			grpCustomCollection.Enabled = false;
		}
	}
}
