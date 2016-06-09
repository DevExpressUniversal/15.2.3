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
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data;
namespace DevExpress.XtraEditors.Designer.Utils {
	[ToolboxItem(false)]
	public class XtraPGFrame : XtraFrame, IEmbeddedFrame {
		protected DevExpress.XtraEditors.SplitterControl splMain;
		public DXPropertyGridEx pgMain;
		public PanelControl pnlControl;
		EmbeddedFrameInit embeddedFrameInit;
		public XtraPGFrame() : this(0) { }
		public XtraPGFrame(int i)
			: base(i) {
			InitializeComponent();
			pgMain.SelectedObjectsChanged += new EventHandler(OnPropertyGridSelectedObjectChanged);
			pgMain.PropertyValueChanged += new PropertyValueChangedEventHandler(OnPropertyGridPropertyValueChanged);
			pgMain.EventHandlerAdded += new EventHandler(OnPropertyGridEventHandlerAdded);
			pgMain.CommandsVisibleIfAvailable = false;
			pgMain.HelpVisible = true;
			pgMain.DrawFlat = true;
			HavePG = true;
			this.embeddedFrameInit = null;
		}
		static readonly object propertyValueChanged = new object();
		public event PropertyValueChangedEventHandler PropertyValueChanged {
			add { Events.AddHandler(propertyValueChanged, value); }
			remove { Events.RemoveHandler(propertyValueChanged, value); }
		}
		protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e) {
			if(FrameOwner != null)
				FrameOwner.SourceObjectChanged(this, e);
			PropertyValueChangedEventHandler handler = (PropertyValueChangedEventHandler)this.Events[propertyValueChanged];
			if(handler != null) handler(this, e);
		}
		public override void StoreGlobalProperties(PropertyStore globalStore) {
			if(AllowGlobalStore)
				globalStore.AddProperty("MainPanel", pnlMain.Width);
			globalStore.AddProperty("PropertyGridSort", pgMain.PropertySort);
			globalStore.AddProperty("PropertyGridHelpVisible", pgMain.HelpVisible);
		}
		public override void RestoreGlobalProperties(PropertyStore globalStore) {
			if(AllowGlobalStore)
				pnlMain.Width = globalStore.RestoreIntProperty("MainPanel", pnlMain.Width);
			pgMain.PropertySort = (PropertySort)globalStore.RestoreProperty("PropertyGridSort", typeof(PropertySort), pgMain.PropertySort);
			pgMain.HelpVisible = globalStore.RestoreBoolProperty("PropertyGridHelpVisible", pgMain.HelpVisible);
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			if(!AllowGlobalStore)
				localStore.AddProperty("MainPanel", pnlMain.Width);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			if(!AllowGlobalStore)
				pnlMain.Width = localStore.RestoreIntProperty("MainPanel", pnlMain.Width);
		}
		protected override DockStyle DescriptionPanelDock {
			get { return EmbeddedFrameInit != null ? EmbeddedFrameInit.DescriptionPanelDock : base.DescriptionPanelDock; }
		}
		protected virtual void UpdatePropertyGridSite() {
			if(DesignMode) return;
			pgMain.Site = null;
			IServiceProvider provider = GetPropertyGridServiceProvider();
			if(provider != null) {
				pgMain.Site = new MySite(provider, pgMain as IComponent);
				pgMain.PropertyTabs.AddTabType(typeof(DXEventsTabEx));
			}
		}
		protected virtual IServiceProvider GetPropertyGridServiceProvider() {
			object selObject = null;
			if(pgMain.SelectedObjects != null && pgMain.SelectedObjects.Length > 0)
				selObject = pgMain.SelectedObjects[0];
			else selObject = pgMain.SelectedObject;
			IDXObjectWrapper wrapper = selObject as IDXObjectWrapper;
			if(wrapper != null) selObject = wrapper.SourceObject;
			Component selComponent = selObject as Component;
			if(selComponent != null && selComponent.Site != null) {
				return selComponent.Site;
			}
			var editingComponent = (IComponent)EditingObject;
			if(selObject != null && editingComponent.Site != null) {
				return editingComponent.Site;
			}
			return null;
		}
		protected virtual void OnPropertyGridSelectedObjectChanged(object sender, EventArgs e) {
			UpdatePropertyGridSite();
			if(IsHandleCreated)
				BeginInvoke(new Action(delegate { pgMain.ShowTabEvents(true); }));
			else pgMain.ShowTabEvents(true);
		}
		protected virtual void OnPropertyGridEventHandlerAdded(object sender, EventArgs e) {
			if(!StackTraceHelper.CheckStackFrame("DoubleClickPropertyValue", null) && 
				!StackTraceHelper.CheckStackFrame("gridView_MouseDoubleClick", null) && 
				!StackTraceHelper.CheckStackFrame("editor_MouseDoubleClick", null))
				return;
			if(ParentForm != null)
				ParentForm.Close();
		}
		protected virtual void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			OnPropertyValueChanged(e);
		}
		protected virtual object[] SelectedObjects {
			get {
				if(SelectedObject == null) return new object[0];
				return new object[] { SelectedObject };
			}
		}
		protected virtual object SelectedObject { get { return EditingObject; } }
		protected virtual object GetPropertyGridObject(object obj) {
			if(!HasFilteredProperties) return GetNestedPropertyGridObject(obj);
			object[] sampleObjects = GetPropertyGridSampleObjects(obj);
			if(sampleObjects != null)
				for(int i = 0; i < sampleObjects.Length; i++)
					sampleObjects[i] = GetNestedPropertyGridObject(sampleObjects[i]);
			return new FilterObject(GetNestedPropertyGridObject(obj), sampleObjects, EmbeddedFrameInit.Properties);
		}
		protected virtual object GetNestedPropertyGridObject(object obj) {
			return obj;
		}
		protected virtual object[] GetPropertyGridSampleObjects(object obj) {
			return SampleObjects;
		}
		protected override string GetDescriptionTextCore() {
			if(EmbeddedFrameInit != null)
				return EmbeddedFrameInit.Description;
			else return base.GetDescriptionTextCore();
		}
		protected virtual void RefreshPropertyGrid() {
			RefreshPropertyGridCore();
		}
		protected virtual void RefreshPropertyGridCore() {
			object[] selObjects = SelectedObjects;
			if(pgMain.Parent == null) return;
			if(selObjects == null || selObjects.Length == 0)
				pgMain.SelectedObject = null;
			else {
				for(int i = 0; i < selObjects.Length; i++)
					selObjects[i] = GetPropertyGridObject(selObjects[i]);
				if(selObjects.Length == 1)
					pgMain.SelectedObject = selObjects[0];
				else {
					try {
						pgMain.SelectedObjects = selObjects; 
					}
					catch { }
				}
				SetupPropertyGridAfterRefresh();
			}
		}
		protected void SetupPropertyGridAfterRefresh() {
			if(EmbeddedFrameInit == null) return;
			if(EmbeddedFrameInit.ExpandAllProperties)
				pgMain.ExpandAllGridItems();
			else {
				for(int i = 0; i < EmbeddedFrameInit.ExpandedPropertiesOnStart.Length; i++)
					pgMain.ExpandProperty(EmbeddedFrameInit.ExpandedPropertiesOnStart[i]);
			}
			if(EmbeddedFrameInit.SelectedPropertyOnStart != string.Empty)
				pgMain.SelectItem(EmbeddedFrameInit.SelectedPropertyOnStart);
		}
		protected EmbeddedFrameInit EmbeddedFrameInit { get { return embeddedFrameInit; } }
		protected IEmbeddedFrameOwner FrameOwner { get { return EmbeddedFrameInit != null ? EmbeddedFrameInit.FrameOwner : null; } }
		protected bool IsEmbedded { get { return FrameOwner != null; } }
		protected object[] SampleObjects { get { return EmbeddedFrameInit != null ? EmbeddedFrameInit.SampleObjects : null; } }
		protected bool HasFilteredProperties {
			get {
				return IsEmbedded && EmbeddedFrameInit.Properties != null &&
					EmbeddedFrameInit.Properties.Length > 0;
			}
		}
		Control IEmbeddedFrame.Control { get { return this; } }
		void IEmbeddedFrame.InitEmbeddedFrame(EmbeddedFrameInit frameInit) {
			this.embeddedFrameInit = frameInit;
			InitFrame(frameInit.EditingObject, string.Empty, null);
		}
		void IEmbeddedFrame.RefreshPropertyGrid() {
			RefreshPropertyGrid();
		}
		void IEmbeddedFrame.SelectProperty(string propertyName) {
			pgMain.SelectItem(propertyName);
		}
		void IEmbeddedFrame.ShowPropertyGridToolBar(bool show) {
			pgMain.ToolbarVisible = show;
		}
		void IEmbeddedFrame.SetPropertyGridSortMode(PropertySort propertySort) {
			pgMain.PropertySort = propertySort;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(!pgMain.IsDisposed)
					pgMain.SelectedObject = null;
			}
			base.Dispose(disposing);
		}
		public class MySite : ISite {
			IServiceProvider sp;
			IComponent comp;
			public MySite(IServiceProvider sp, IComponent comp) {
				this.sp = sp;
				this.comp = comp;
			}
			IComponent ISite.Component {
				get { return comp; }
			}
			IContainer ISite.Container {
				get { return null; }
			}
			bool ISite.DesignMode {
				get { return false; }
			}
			string ISite.Name {
				get { return null; }
				set { }
			}
			object IServiceProvider.GetService(Type t) {
				if(t.Equals(typeof(System.ComponentModel.Design.IDesignerHost)))
					return sp.GetService(t);
				return null;
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.pgMain = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
			this.pnlControl = new DevExpress.XtraEditors.PanelControl();
			this.splMain = new DevExpress.XtraEditors.SplitterControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(456, 42);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlMain.Location = new System.Drawing.Point(0, 78);
			this.pnlMain.Size = new System.Drawing.Size(160, 234);
			this.horzSplitter.Location = new System.Drawing.Point(0, 42);
			this.horzSplitter.Size = new System.Drawing.Size(456, 4);
			this.pgMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgMain.DrawFlat = false;
			this.pgMain.HelpVisible = false;
			this.pgMain.Location = new System.Drawing.Point(165, 78);
			this.pgMain.Name = "pgMain";
			this.pgMain.Size = new System.Drawing.Size(291, 234);
			this.pgMain.TabIndex = 2;
			this.pgMain.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgMain_PropertyValueChanged);
			this.pnlControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlControl.Location = new System.Drawing.Point(0, 46);
			this.pnlControl.Name = "pnlControl";
			this.pnlControl.Padding = new System.Windows.Forms.Padding(4);
			this.pnlControl.Size = new System.Drawing.Size(456, 32);
			this.pnlControl.TabIndex = 0;
			this.splMain.Location = new System.Drawing.Point(160, 78);
			this.splMain.MinExtra = 170;
			this.splMain.MinSize = 150;
			this.splMain.Name = "splMain";
			this.splMain.Size = new System.Drawing.Size(5, 234);
			this.splMain.TabIndex = 3;
			this.splMain.TabStop = false;
			this.Controls.Add(this.pgMain);
			this.Controls.Add(this.splMain);
			this.Controls.Add(this.pnlControl);
			this.Name = "XtraPGFrame";
			this.Size = new System.Drawing.Size(456, 312);
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
		}
	}
	public class EmbeddedFrameInit {
		object editingObject;
		string description;
		object[] sampleObjects;
		IEmbeddedFrameOwner frameOwner;
		string[] properties;
		string[] expandedPropertiesOnStart;
		bool expandAllProperties;
		string selectedPropertyOnStart;
		DockStyle descriptionPanelDock;
		public EmbeddedFrameInit(IEmbeddedFrameOwner frameOwner) : this(frameOwner, null) { }
		public EmbeddedFrameInit(IEmbeddedFrameOwner frameOwner, object editingObject)
			: this(frameOwner, editingObject, string.Empty) {
		}
		public EmbeddedFrameInit(IEmbeddedFrameOwner frameOwner, object editingObject, string description) {
			this.frameOwner = frameOwner;
			this.editingObject = editingObject;
			this.description = description;
			this.sampleObjects = null;
			this.properties = new string[] { };
			this.expandedPropertiesOnStart = new string[] { };
			this.expandAllProperties = false;
			this.selectedPropertyOnStart = string.Empty;
			this.descriptionPanelDock = DockStyle.Bottom;
		}
		public DockStyle DescriptionPanelDock { get { return descriptionPanelDock; } set { descriptionPanelDock = value; } }
		public object EditingObject { get { return editingObject; } set { editingObject = value; } }
		public object[] SampleObjects { get { return sampleObjects; } set { sampleObjects = value; } }
		public string Description { get { return description; } set { description = value; } }
		public IEmbeddedFrameOwner FrameOwner { get { return frameOwner; } set { frameOwner = value; } }
		public string[] Properties {
			get { return properties; }
			set {
				if(value == null)
					value = new string[] { };
				properties = value;
			}
		}
		public string[] ExpandedPropertiesOnStart {
			get { return expandedPropertiesOnStart; }
			set {
				if(value == null)
					value = new string[] { };
				expandedPropertiesOnStart = value;
			}
		}
		public bool ExpandAllProperties { get { return expandAllProperties; } set { expandAllProperties = value; } }
		public string SelectedPropertyOnStart { get { return selectedPropertyOnStart; } set { selectedPropertyOnStart = value; } }
	}
	public interface IEmbeddedFrame {
		Control Control { get; }
		void InitEmbeddedFrame(EmbeddedFrameInit frameInit);
		void RefreshPropertyGrid();
		void SelectProperty(string propertyName);
		void ShowPropertyGridToolBar(bool show);
		void SetPropertyGridSortMode(PropertySort propertySort);
	}
	public interface IEmbeddedFrameOwner {
		void SourceObjectChanged(IEmbeddedFrame frame, PropertyValueChangedEventArgs e);
	}
}
