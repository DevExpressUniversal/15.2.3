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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.Blending;
using DevExpress.Utils.Design;
namespace DevExpress.XtraVerticalGrid.Design {
	public class PropertyGridDesigner : VerticalGridDesignerBase {
		PropertyGridControl PGrid { get { return Control as PropertyGridControl; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
		}
		public class PropertyGridBaseActionList : DesignerActionList {
			class SelectedObjectConverter : ReferenceConverter {
				public SelectedObjectConverter()
					: base(typeof(IComponent)) {
				}
			}
			PropertyGridDesigner designer;
			public PropertyGridBaseActionList(PropertyGridDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected PropertyGridControl PGrid { get { return Designer.PGrid; } }
			protected PropertyGridDesigner Designer { get { return designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionPropertyItem("SelectedObject", "Choose Object"));
				return res;
			}
			[TypeConverter(typeof(SelectedObjectConverter))]
			public object SelectedObject {
				get { return PGrid.SelectedObject; }
				set {
					EditorContextHelper.SetPropertyValue(PGrid.Site, PGrid, "SelectedObject", value);
				}
			}
		}
	}
	public class VerticalGridDesigner : VerticalGridDesignerBase {
		VGridControl VGrid { get { return Control as VGridControl; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new VGridActionList(this));
			base.RegisterActionLists(list);
		}
		public class VGridActionList : DesignerActionList {
			VerticalGridDesigner designer;
			public VGridActionList(VerticalGridDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected VGridControl VGrid { get { return Designer.VGrid; } }
			protected VerticalGridDesigner Designer { get { return designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DataSourceWizardDesignerActionMethodItem(this));
				res.Add(new DesignerActionPropertyItem("DataSource", "Choose Data Source"));
				return res;
			}
#if DXWhidbey
			[AttributeProvider(typeof(IListSource))]
#endif
			public object DataSource {
				get { return VGrid.DataSource; }
				set {
					EditorContextHelper.SetPropertyValue(VGrid.Site, VGrid, "DataSource", value);
				}
			}
			[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
			public void CreateDataSource() {
				Designer.CreateDataSource();
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			VGrid.DataSourceChanged += VGrid_DataSourceChanged;
			VGrid.Paint += VGrid_Paint;
		}
		protected override void Dispose(bool disposing) {
			if(VGrid != null) {
				VGrid.Paint -= VGrid_Paint;
				VGrid.DataSourceChanged -= VGrid_DataSourceChanged;
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(VGrid);
			}
			base.Dispose(disposing);
		}
		int rowsCount = -1;
		void VGrid_Paint(object sender, PaintEventArgs e) {
			bool rowsChanged = false;
			if(VGrid.Rows.Count != rowsCount) {
				rowsChanged = true;
				rowsCount = VGrid.Rows.Count;
			}
			if(rowsChanged) 
				OnRowsChanded();
		}
		void OnRowsChanded() {
			DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(VGrid);
			if(rowsCount > 0)
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(VGrid, ContentAlignment.BottomLeft, false, false);
			else
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(VGrid);
		}
		void VGrid_DataSourceChanged(object sender, EventArgs e) {
			DevExpress.Design.DataAccess.UI.DataSourceGlyph.DataSourceChanged(VGrid);
		}
	}
	public class VerticalGridDesignerBase : BaseControlDesigner {
		private DesignerVerbCollection verbs;
		private frmMain editor;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected override bool AllowEditInherited { get { return false; } }
		public VerticalGridDesignerBase() {
			verbs = new DesignerVerbCollection(
				new DesignerVerb[] {
									   new DesignerVerb("About...", new EventHandler(OnAboutClick)),
									   new DesignerVerb("Run Designer", new EventHandler(OnDesignerClick)),
			});
		}
		VGridControlBase VGrid { get { return Control as VGridControlBase; } }
		private void OnAboutClick(object sender, EventArgs e) {
			VGridControl.About();
		}
		protected frmMain Editor {
			get { return editor; }
			set {
				if(Editor == value)
					return;
				if(Editor != null)
					Editor.Dispose();
				editor = value;
			}
		}
		private void OnDesignerClick(object sender, EventArgs e) { RunDesigner(); }
		public void RunDesigner() {
			VGridControlBase vgrid = VGrid;
			if(vgrid == null)
				return;
			if(Editor == null) {
				editor = vgrid is PropertyGridControl ? new frmMainProperties() : new frmMain();
			}
			try {
				Editor.ShowInTaskbar = false;
				Editor.InitEditingObject(vgrid);
				Editor.ShowDialog();
			}
			finally {
				Editor = null;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
			}
			base.Dispose(disposing);
		}
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		public override ICollection AssociatedComponents {
			get {
				ICollection c = base.AssociatedComponents;
				VGridControlBase grid = VGrid;
				if(grid == null)
					return c;
				ArrayList components;
				if(c == null)
					components = new ArrayList();
				else
					components = new ArrayList(c);
				AddAssociatedComponents(components, grid.Rows);
				foreach(RepositoryItem repository in grid.RepositoryItems) {
					components.Add(repository);
				}
				return components;
			}
		}
		private void AddAssociatedComponents(ArrayList comp, VGridRows rows) {
			foreach(BaseRow row in rows) {
				if(row.HasChildren)
					AddAssociatedComponents(comp, row.ChildRows);
				comp.Add(row);
				MultiEditorRow multiEditorRow = row as MultiEditorRow;
				if(multiEditorRow != null) {
					comp.AddRange(multiEditorRow.PropertiesCollection);
				}
			}
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitialize();
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			OnInitialize();
		}
#endif
		protected virtual void OnInitialize() {
			VGridControlBase grid = VGrid;
			if(grid == null || grid.Container == null)
				return;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new VGridBaseActionList(this));
			base.RegisterActionLists(list);
		}
		public class VGridBaseActionList : DesignerActionList {
			VerticalGridDesignerBase designer;
			public VGridBaseActionList(VerticalGridDesignerBase designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected VGridControlBase VGrid { get { return Designer.VGrid; } }
			protected VerticalGridDesignerBase Designer { get { return designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer"));
				res.Add(new DesignerActionMethodItem(this, "AddRow", "Add Row"));
				return res;
			}
			public virtual void RunDesigner() {
				Designer.RunDesigner();
			}
			public virtual void AddRow() {
				VGrid.Rows.Add(new DevExpress.XtraVerticalGrid.Rows.EditorRow());
			}
		}
		#region Mouse Handling
		protected override bool GetHitTest(Point pt) {
			if(DebuggingState) return false;
			VGridHitInfo hi = VGrid.CalcHitInfo(VGrid.PointToClient(pt));
			return (hi.HitInfoType == HitInfoTypeEnum.BandEdge ||
				hi.HitInfoType == HitInfoTypeEnum.HeaderSeparator ||
				hi.HitInfoType == HitInfoTypeEnum.MultiEditorCellSeparator ||
				hi.HitInfoType == HitInfoTypeEnum.RecordValueEdge ||
				hi.HitInfoType == HitInfoTypeEnum.RowEdge ||
				hi.HitInfoType == HitInfoTypeEnum.ExpandButton ||
				hi.HitInfoType == HitInfoTypeEnum.VertScrollBar);
		}
		const int WM_MOUSEMOVE = 0x200, WM_LBUTTONUP = 0x202, MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002;
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(DebuggingState) return;
			if(m.Msg == WM_MOUSEMOVE || m.Msg == WM_LBUTTONUP) {
				Point p = new Point(m.LParam.ToInt32());
				int btns = m.WParam.ToInt32();
				MouseButtons buttons = MouseButtons.None;
				if((btns & MK_LBUTTON) != 0)
					buttons |= MouseButtons.Left;
				if((btns & MK_RBUTTON) != 0)
					buttons |= MouseButtons.Right;
				MethodInfo mi = GetMethodInfo(m.Msg == WM_MOUSEMOVE ? "OnMouseMove" : "OnMouseUp");
				if(mi != null)
					mi.Invoke(Control, new object[] { new MouseEventArgs(buttons, 0, p.X, p.Y, 0) });
			}
		}
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			MethodInfo mi = GetMethodInfo("OnMouseEnter");
			if(mi != null)
				mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			MethodInfo mi = GetMethodInfo("OnMouseLeave");
			if(mi != null)
				mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		#endregion Mouse Handling
	}
	internal class XtraVerticalGridBlendingDesigner : ComponentDesigner {
		DesignerVerbCollection verbs;
		public XtraVerticalGridBlendingDesigner() {
			verbs = new DesignerVerbCollection(
				new DesignerVerb[] {
									   new DesignerVerb("Preview", new EventHandler(OnPreviewClick))});
		}
		public override DesignerVerbCollection Verbs {
			get { return verbs; }
		}
		private XtraVertGridBlending Blending { get { return Component as XtraVertGridBlending; } }
		private void OnPreviewClick(object sender, EventArgs e) {
			if(Blending.VertGridControl != null) {
				Form dlg = new DevExpress.XtraVerticalGrid.Blending.Preview(Blending);
				dlg.ShowDialog();
			}
			else
				MessageBox.Show("The VertGridControl property is not initialized." +
					"\r\nPlease select a verticalgrid control from its dropdown list.",
					Blending.Site.Name + ".VertGridControl is null...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
#endif
			if(Blending == null || Blending.Container == null)
				return;
			if(Blending.VertGridControl == null)
				Blending.VertGridControl = GetVertGrid(Blending.Container);
		}
		private static VGridControlBase GetVertGrid(IContainer container) {
			return GetTypeFromContainer(container, typeof(VGridControlBase)) as VGridControlBase;
		}
		protected static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null)
				return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj))
					return obj;
			}
			return null;
		}
	}
}
