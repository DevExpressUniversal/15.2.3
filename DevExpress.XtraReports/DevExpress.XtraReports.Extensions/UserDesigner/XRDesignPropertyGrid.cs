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
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Design;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
namespace DevExpress.XtraReports.UserDesigner
{
	[ToolboxItem(false)]
	public class XRDesignPropertyGrid : XtraUserControl, IDesignControl
	{
		ComponentsComboBoxControler primarySelection;
		PropertyGridUserControl propertyGrid;
		System.ComponentModel.Container components = null;
		XRDesignPanel xrDesignPanel;
		IDesignerHost designerHost;
		internal PropertyGridUserControl PropertyGrid {
			get { return propertyGrid; }
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPropertyGridXRDesignPanel"),
#endif
		DefaultValue(null),
		]
		public XRDesignPanel XRDesignPanel {
			get { return xrDesignPanel; }
			set {
				if(xrDesignPanel == value)
					return;
				if(xrDesignPanel != null) {
					xrDesignPanel.Activated -= new EventHandler(OnActivate);
					xrDesignPanel.DesignerHostLoading -= new EventHandler(OnDesignerHostLoading);
					OnDeactivate(xrDesignPanel, EventArgs.Empty);
				}
				xrDesignPanel = value;
				if(xrDesignPanel != null) {
					xrDesignPanel.DesignerHostLoading += new EventHandler(OnDesignerHostLoading);
					xrDesignPanel.Activated += new EventHandler(OnActivate);
					primarySelection.ServiceProvider = value as IServiceProvider;
				}
			}
		}
		#region tests
#if DEBUGTEST
		public object Test_SelectedObject {
			get { return propertyGrid.SelectedObject; }
		}
#endif
		#endregion
		public XRDesignPropertyGrid()
		{
			InitializeComponent();
			InitPropertyGrid();
			Controls.Add(propertyGrid);
			Controls.SetChildIndex(propertyGrid, 0);
		}
		private void InitPropertyGrid() {
			propertyGrid = new PropertyGridUserControl();
			propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		}
		protected override void Dispose(bool disposing) {
			if( disposing ) {
				if(xrDesignPanel != null) {
					xrDesignPanel.Activated -= new EventHandler(OnActivate);
					xrDesignPanel.DesignerHostLoading -= new EventHandler(OnDesignerHostLoading);
				}
				if(designerHost != null) {
					designerHost.RemoveService(typeof(PropertyGridService));
				}
				UnSubscribeEvents();
				xrDesignPanel = null;
				ClearControls();
				propertyGrid = null;
				primarySelection = null;
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		void UnSubscribeEvents() {
			Application.Idle -= new EventHandler(OnIdle);
			if (xrDesignPanel == null)
				return;
			propertyGrid.ServiceProvider = null;
			xrDesignPanel.SelectionChanged -= new EventHandler(OnSelectionChanged);
			xrDesignPanel.ComponentAdded -= new ComponentEventHandler(OnReportContentChanged);
			xrDesignPanel.ComponentRemoved -= new ComponentEventHandler(OnReportContentChanged);
			xrDesignPanel.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
			xrDesignPanel.Deactivated -= new EventHandler(OnDeactivate);
			IDesignerHost designerHost = (IDesignerHost)xrDesignPanel.GetService(typeof(IDesignerHost));
			if (designerHost == null)
				return;
			designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed); 
		}
		private void SubscribeEvents() {
			propertyGrid.ServiceProvider = xrDesignPanel;
			xrDesignPanel.Deactivated += new EventHandler(OnDeactivate);
			xrDesignPanel.SelectionChanged += new EventHandler(OnSelectionChanged);
			xrDesignPanel.ComponentAdded += new ComponentEventHandler(OnReportContentChanged);
			xrDesignPanel.ComponentRemoved += new ComponentEventHandler(OnReportContentChanged);
			xrDesignPanel.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			designerHost = (IDesignerHost)xrDesignPanel.GetService(typeof(IDesignerHost));
			if(designerHost != null) 
				designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed); 
		}
		void ClearControls() {
			if(propertyGrid != null)
				propertyGrid.SelectedObject = null;
			if(primarySelection != null) {
				primarySelection.Items.Clear();
				primarySelection.SelectedItem = null;
			}
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.primarySelection = new ComponentsComboBoxControler((IServiceProvider)xrDesignPanel);
			((System.ComponentModel.ISupportInitialize)(this.primarySelection.Properties)).BeginInit(); 
			this.SuspendLayout();
			this.primarySelection.TabIndex = 0;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.primarySelection});
			this.Name = "XRDesignPropertyGrid";
			((System.ComponentModel.ISupportInitialize)(this.primarySelection.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(e.Member != null && e.Member.Name == "Name")
				UpdateContent();
		}
		private void OnReportContentChanged(object sender, ComponentEventArgs e) {
			UpdateContent();
		}
		void UpdateContent() {
			primarySelection.UpdateItems();
			propertyGrid.SelectedObjects = xrDesignPanel.GetSelectedComponents();
			UpdateSelectedObjects();
		}
		void UpdateSelectedObjects() {
			System.Diagnostics.Debug.Assert(xrDesignPanel != null);
			object[] selection = xrDesignPanel.GetSelectedComponents();
			SetPropertyGridState(selection);
			primarySelection.UpdateSelectedItem();
		}
		void SetPropertyGridState(ICollection selection) {
			bool locked = !LockService.GetInstance(designerHost).CanChangeComponents(selection);
			propertyGrid.Enabled = (designerHost != null && locked) ? false : true;
		}
		void OnIdle(object sender, EventArgs e) {
			Application.Idle -= new EventHandler(OnIdle);
			if(IsDisposed)
				return;
			try {
				object[] selection = xrDesignPanel.GetSelectedComponents();
				SetPropertyGridState(selection);
				propertyGrid.SelectedObjects = selection;
				UpdateSelectedObjects();
			} catch(Exception ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(DevExpress.XtraPrinting.Native.NativeSR.TraceSource, ex);
				NotificationService.ShowException<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(designerHost), FindForm(), ex);
			}
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if (!DesignMethods.IsDesignerInTransaction(designerHost) && xrDesignPanel.ReportState != ReportState.Opening) {
				Application.Idle -= new EventHandler(OnIdle);
				Application.Idle += new EventHandler(OnIdle);
			}
		}
		private void OnActivate(object sender, EventArgs e) {
			UnSubscribeEvents();
			SubscribeEvents();
			ClearControls();
			primarySelection.UpdateItems();
			propertyGrid.SelectedObjects = xrDesignPanel.GetSelectedComponents();
			UpdateSelectedObjects();
		}
		private void OnDeactivate(object sender, EventArgs e) {
			ClearControls();
			UnSubscribeEvents();
		}
		private void OnDesignerHostLoading(object sender, EventArgs e) {
			xrDesignPanel.AddService(typeof(PropertyGridService), new PropertyGridService(propertyGrid.PropertyGridControl));
		}
		private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			if( !DesignMethods.IsDesignerInTransaction(designerHost) ) {
				UpdateSelectedObjects();
			}
		}
	}
	[ToolboxItem(false)]
	public class ComponentsComboBoxControler : ComboBoxEdit {
		#region inner clases
		class CmbItem : IComparable {
			IComponent comp;
			string Name {
				get { return comp.Site != null ? comp.Site.Name : string.Empty; }
			}
			string DisplayTypeName {
				get { return DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(comp.GetType()); }
			}
			public IComponent Component {
				get { return comp; }
			}
			public CmbItem(IComponent comp) {
				this.comp = comp;
			}
			public override string ToString() {
				return string.Concat(Name, "   ", DisplayTypeName);
			}
			#region IComparable Members
			int IComparable.CompareTo(object obj) {
				CmbItem cmbItem = obj as CmbItem;
				return cmbItem != null ? Comparer.Default.Compare(this.Name, cmbItem.Name) :
					Comparer.Default.Compare(this, obj);
			}
			#endregion
		}
		#endregion
		IServiceProvider serviceProvider;
		public IServiceProvider ServiceProvider {
			get { return serviceProvider; }
			set { serviceProvider = value; }
		}
		public ComboBoxItemCollection Items { get { return this.Properties.Items; } }
		public ComponentsComboBoxControler(IServiceProvider serviceProvider)
			: base() {
			this.serviceProvider = serviceProvider;
			InitializeComponent();
			SubscribeEvents();
		}
		void InitializeComponent() {
			((ISupportInitialize)this.Properties).BeginInit();
			this.Dock = System.Windows.Forms.DockStyle.Top;
			this.Properties.Sorted = true;
			this.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			FontStyle fontStyle = AppearanceObject.DefaultMenuFont.Style & ~FontStyle.Bold;
			this.Font = new Font(new Font(AppearanceObject.DefaultMenuFont, fontStyle), fontStyle | FontStyle.Bold);
			this.Name = "primarySelection";
			this.Size = new System.Drawing.Size(150, 21);
			((ISupportInitialize)this.Properties).EndInit();
		}
		void SubscribeEvents() {
			this.SelectedIndexChanged += new EventHandler(PrimarySelection_SelectedIndexChanged);
		}
		void UnsubscribeEvents() {
			this.SelectedIndexChanged -= new EventHandler(PrimarySelection_SelectedIndexChanged);
		}
		void PrimarySelection_SelectedIndexChanged(object sender, EventArgs e) {
			SetSelectedComponent();
		}
		ISelectionService GetSelectionService() {
			return serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
		}
		CmbItem GetItemBy(IComponent c) {
			foreach(CmbItem item in Items)
				if(ReferenceEquals(item.Component, c)) return item;
			return null;
		}
		protected virtual bool ValidComponent(IComponent component) {
			return component != null;
		}
		protected virtual void ResetNonValidItem() {
			this.SelectedIndex = -1;
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				UnsubscribeEvents();
			base.Dispose(disposing);
		}
		protected internal void SetSelectedComponent() {
			if(serviceProvider == null)
				return;
			ISelectionService selectionService = GetSelectionService();
			if(selectionService == null)
				return;
			CmbItem item = (CmbItem)this.SelectedItem;
			if(item == null)
				return;
			selectionService.SetSelectedComponents(new object[] { item.Component }, SelectionTypes.Replace);
		}
		public void UpdateItems() {
			IContainer container = serviceProvider.GetService(typeof(IContainer)) as IContainer;
			if(container == null)
				return;
			ArrayList items = new ArrayList();
			foreach(IComponent c in container.Components) {
				if(ValidComponent(c))
					items.Add(new CmbItem(c));
			}
			((System.ComponentModel.ISupportInitialize)(this.Properties)).BeginInit();
			try {
				this.Items.Clear();
				this.Items.AddRange(items.ToArray());
			}
			finally {
				((System.ComponentModel.ISupportInitialize)(this.Properties)).EndInit();
			}
		}
		public void UpdateSelectedItem() {
			try {
				ArrayList components = new ArrayList();
				components.AddRange(GetSelectionService().GetSelectedComponents());
				if(components.Count == 1) {
					CmbItem item = GetItemBy((IComponent)components[0]);
					if(item == null) {
						ResetNonValidItem();
						return;
					}
					System.Diagnostics.Debug.Assert(item != null);
					int index = this.Items.IndexOf(item);
					if(index >= 0) {
						this.SelectedIndex = index;
						return;
					}
				}
				this.SelectedIndex = -1;
			}
			finally {
			}
		}
		public IComponent GetSelectedComponent() {
			if(SelectedIndex == -1)
				return null;
			CmbItem selectedCmbItem = Items[SelectedIndex] as CmbItem;
			return selectedCmbItem != null ? selectedCmbItem.Component : null;
		}
	}
}
