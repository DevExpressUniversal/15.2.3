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
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid.Design {
	public class PivotGridControlDesigner : BaseControlDesigner {
		DesignerVerbCollection verbs;
		PivotGridEditorForm editor;
		ISelectionService selectionService;
		bool isFieldSelected;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected override bool AllowEditInherited { get { return false; } }
		public PivotGridControlDesigner() {
			verbs = new DesignerVerbCollection(new DesignerVerb[] {
			   new DesignerVerb("About", new EventHandler(OnAboutClick)),
			   new DesignerVerb("Run Designer", new EventHandler(OnDesignerClick)),
			   new DesignerVerb("&Add field", new EventHandler(OnAddFieldClick)),
			});
			this.selectionService = null;
			this.isFieldSelected = false;
			this.editor = null;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(SelectionService != null) {
				SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			}
			if(PivotGrid != null) {
				PivotGrid.DataSourceChanged += PivotGrid_DataSourceChanged;
				PivotGrid.CustomDrawEmptyArea += PivotGrid_CustomDrawEmptyArea;
			}
		}
		protected override void Dispose(bool disposing) {
			if(PivotGrid != null) {
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(PivotGrid);
				PivotGrid.CustomDrawEmptyArea -= PivotGrid_CustomDrawEmptyArea;
				PivotGrid.DataSourceChanged -= PivotGrid_DataSourceChanged;
			}
			if(this.selectionService != null) {
				this.selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
				this.selectionService = null;
			}
			if(disposing) {
				Editor = null;
			}
			base.Dispose(disposing);
		}
		public override ICollection AssociatedComponents {
			get {
				if(PivotGrid == null) return base.AssociatedComponents;
				ArrayList fields = new ArrayList();
				foreach(PivotGridField field in PivotGrid.Fields) {
					fields.Add(field);
				}
				return fields;
			}
		}
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		internal bool IsBoundToData {
			get {
				return PivotGrid != null && (PivotGrid.DataSource != null || !String.IsNullOrEmpty(PivotGrid.OLAPConnectionString));
			}
		}
		int fieldsCount = -1;
		void PivotGrid_CustomDrawEmptyArea(object sender, PivotCustomDrawEventArgs e) {
			bool fieldsCountChanged = false;
			if(fieldsCount != PivotGrid.Fields.Count) {
				fieldsCount = PivotGrid.Fields.Count;
				fieldsCountChanged = true;
			}
			if(fieldsCountChanged)
				UpdateDataSourceGlyph();
		}
		void UpdateDataSourceGlyph() {
			DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(PivotGrid);
			if(fieldsCount > 0) return;
			if(IsBoundToData)
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(PivotGrid, ContentAlignment.BottomLeft, false, false);
			else
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(PivotGrid);
		}
		bool isDataSourceInUse;
		bool isOlapConnectionStringInUse;
		void PivotGrid_DataSourceChanged(object sender, EventArgs e) {
			bool dataSourceInUseChanged = false;
			if(isDataSourceInUse != (IsBoundToData && PivotGrid.DataSource != null)) {
				isDataSourceInUse = (PivotGrid.DataSource != null);
				dataSourceInUseChanged = true;
			}
			bool olapConnectionStringInUseChanged = false;
			if(isOlapConnectionStringInUse != (IsBoundToData && !string.IsNullOrEmpty(PivotGrid.OLAPConnectionString))) {
				isOlapConnectionStringInUse = !string.IsNullOrEmpty(PivotGrid.OLAPConnectionString);
				olapConnectionStringInUseChanged = true;
			}
			if(dataSourceInUseChanged || olapConnectionStringInUseChanged)
				UpdateDataSourceGlyph();
			else
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.DataSourceChanged(PivotGrid);
		}
		public void AddField(PivotArea area) {
			if(PivotGrid == null) return;
			PivotGrid.Fields.Add(string.Empty, area);
		}
		public void RetrieveFields(bool visible) {
			if(PivotGrid == null)
				return;
			try {
				PivotGrid.RetrieveFields(PivotArea.FilterArea, visible);
			}
			catch(Exception e) {
				MessageBox.Show(String.Format("The exception: {0} was thrown when fields were retrieved", e.Message));
			}
		}
		public void RunDesigner() {
			if(PivotGrid == null) return;
			Editor = new PivotGridEditorForm();
			editor.InitEditingObject(PivotGrid);
			Editor.ShowDialog();
			Editor = null;
		}
		protected ISelectionService SelectionService {
			get {
				if(selectionService == null)
					selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
				return selectionService;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new PivotGridDataSourceActionList(this));
			list.Add(new PivotGridAddFieldsActionList(this));
			base.RegisterActionLists(list);
		}
		protected internal PivotGridControl PivotGrid { get { return Control as PivotGridControl; } }
		protected PivotGridEditorForm Editor {
			get { return editor; }
			set {
				if(Editor == value) return;
				if(Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		protected override bool EnableDragRect { get { return false; } }
		protected override void OnDragEnter(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override void OnDragOver(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected virtual bool GetHitTestCore(Point client) {
			if(DebuggingState) return false;
			if(!Rectangle.Inflate(Control.ClientRectangle, -3, -3).Contains(client)) return false;
			if(PivotGrid == null) return false;
			return PivotGrid.GetFieldAt(client) != null || PivotGrid.CanResizeField(client);
		}
		protected override bool GetHitTest(Point point) {
			bool res = base.GetHitTest(point);
			if(DebuggingState) return res;
			if(PivotGrid == null || res) return res;
			Point client = Control.PointToClient(point);
			return GetHitTestCore(client);
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if(SelectionService == null || PivotGrid == null) return;
			bool oldIsFieldSelected = isFieldSelected;
			PivotGridField field = SelectionService.PrimarySelection as PivotGridField;
			isFieldSelected = field != null ? PivotGrid.Fields.Contains(field) : false;
			if(oldIsFieldSelected && !isFieldSelected)
				PivotGrid.Invalidate();
		}
		void OnAboutClick(object sender, EventArgs e) {
			PivotGridControl.About();
		}
		void OnDesignerClick(object sender, EventArgs e) {
			RunDesigner();
		}
		void OnAddFieldClick(object sender, EventArgs e) {
			AddField(PivotArea.RowArea);
		}
		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			var res = base.GetGlyphs(selectionType);
			object selectedObject = null;
			ISelectionService service = GetService(typeof(ISelectionService)) as ISelectionService;
			if(service != null)
				selectedObject = service.PrimarySelection;
			AddColumnGlyph(res, selectedObject);
			return res;
		}
		void AddColumnGlyph(GlyphCollection res, object selectedObject) {
			PivotGridField field = selectedObject as PivotGridField;
			if(field != null && field.PivotGrid == PivotGrid) {
				PivotGridViewInfoData viewInfo = ((IPivotGridViewInfoDataOwner)PivotGrid).DataViewInfo;
				if(viewInfo == null)
					return;
				PivotHeadersViewInfoBase headers = viewInfo.ViewInfo.GetHeader(field.Area);
				PivotHeaderViewInfoBase ci = headers[viewInfo.GetFieldItem(field)];
				if(ci == null)
					return;
				ControlDesignerActionListGlyphHelper.CreateDesignerGlyphWrapper(res, field, PivotGrid, ci.ControlBounds);
			}
		}
		public class PivotGridDataSourceActionList : DesignerActionList {
			PivotGridControlDesigner designer;
			public PivotGridDataSourceActionList(PivotGridControlDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected PivotGridControl PivotGrid { get { return designer.PivotGrid; } }
			protected PivotGridControlDesigner Designer { get { return designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				string dataSourceCategory = "DataSource";
				res.Add(new DataSourceWizardDesignerActionMethodItem(this, dataSourceCategory));
				res.Add(new DesignerActionPropertyItem("DataSource", "Choose Data Source", dataSourceCategory));
				res.Add(new DesignerActionPropertyItem("OLAPConnectionString", "Choose OLAP Data Source", dataSourceCategory));
				return res;
			}
#if DXWhidbey
			[AttributeProvider(typeof(IListSource))]
#endif
			public object DataSource {
				get { return PivotGrid.DataSource; }
				set {
					EditorContextHelper.SetPropertyValue(PivotGrid.Site, PivotGrid, "DataSource", value);
					EditorContextHelperEx.RefreshSmartPanel(PivotGrid);
				}
			}
			[Editor("DevExpress.XtraPivotGrid.Design.OLAPConnectionEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))]
			public string OLAPConnectionString {
				get { return PivotGrid.OLAPConnectionString; }
				set {
					EditorContextHelper.SetPropertyValue(PivotGrid.Site, PivotGrid, "OLAPConnectionString", value);
					EditorContextHelperEx.RefreshSmartPanel(PivotGrid);
				}
			}
			[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
			public void CreateDataSource() {
				Designer.CreateDataSource();
			}
		}
		public class PivotGridAddFieldsActionList : DesignerActionList {
			PivotGridControlDesigner designer;
			bool hideRetrievedFields;
			public PivotGridAddFieldsActionList(PivotGridControlDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
				this.hideRetrievedFields = false;
			}
			protected PivotGridControlDesigner Designer { get { return designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionMethodItem(this, "AddFieldToFilterArea", "Add Field to Filter Area"));
				res.Add(new DesignerActionMethodItem(this, "AddFieldToColumnArea", "Add Field to Column Area"));
				res.Add(new DesignerActionMethodItem(this, "AddFieldToRowArea", "Add Field to Row Area"));
				res.Add(new DesignerActionMethodItem(this, "AddFieldToDataArea", "Add Field to Data Area"));
				if(Designer.IsBoundToData) {
					res.Add(new DesignerActionPropertyItem("HideRetrievedFields", "Hide retrieved fields"));
					res.Add(new DesignerActionMethodItem(this, "RetrieveFields", "Retrieve " + (hideRetrievedFields ? "and hide " : string.Empty) + "fields"));
				}
				res.Add(new DesignerActionMethodItem(this, "EditFields", "Run Designer..."));
				return res;
			}
			void OnDesignerActionValueChanged() {
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
			public void AddFieldToFilterArea() {
				designer.AddField(PivotArea.FilterArea);
			}
			public void AddFieldToColumnArea() {
				designer.AddField(PivotArea.ColumnArea);
			}
			public void AddFieldToRowArea() {
				designer.AddField(PivotArea.RowArea);
			}
			public void AddFieldToDataArea() {
				designer.AddField(PivotArea.DataArea);
			}
			public bool HideRetrievedFields {
				get { return hideRetrievedFields; }
				set {
					if(HideRetrievedFields == value)
						return;
					hideRetrievedFields = value;
					OnDesignerActionValueChanged();
				}
			}
			public void RetrieveFields() {
				Designer.RetrieveFields(!HideRetrievedFields);
			}
			public void EditFields() {
				designer.RunDesigner();
			}
		}
	}
}
