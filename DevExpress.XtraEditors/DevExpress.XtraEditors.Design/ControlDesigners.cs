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
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System.Windows.Forms.Design.Behavior;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.Design {
	public class PersistentRepositoryDesigner : BaseComponentDesigner {
		protected override bool AllowInheritanceWrapper { get { return true; } }
		public override ICollection AssociatedComponents {
			get {
				PersistentRepository rep = Component as PersistentRepository;
				if(rep == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				controls.AddRange(rep.Items);
				return controls;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new PersistentRepositoryDesignerActionList(this));
			base.RegisterActionLists(list);
		}
	}
	public class PersistentRepositoryDesignerActionList : DesignerActionList {
		IDesigner designer;
		public PersistentRepositoryDesignerActionList(IDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "EditItemsCmd", "Edit Items", true));
			return res;
		}
		void EditItemsCmd() {
			EditorContextHelper.EditValue(designer, Component, "Items");
		}
	}
	public class ScrollBarBaseDesigner:  ControlDesigner {
		public override SelectionRules SelectionRules {
			get {
				ScrollBarBase scrollBar = (Control as ScrollBarBase);
				if(!scrollBar.ScrollBarAutoSize)
					return SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible;
				if (ScrollBarType.Horizontal == scrollBar.ScrollBarType) 
					return SelectionRules.Moveable | SelectionRules.Visible | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
				else return SelectionRules.Moveable | SelectionRules.Visible | SelectionRules.TopSizeable | SelectionRules.BottomSizeable;
			}
		}
	}
	public abstract class ListControlActionList : DesignerActionList {
		public ListControlActionList(IComponent listSourceControl)
			: base(listSourceControl) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			result.Add(new DataSourceWizardDesignerActionMethodItem(this, DataBindingCategoryName));
			result.Add(new DesignerActionPropertyItem("DataSource", Properties.Resources.DataSourceCaption, DataBindingCategoryName));
			result.Add(new DesignerActionPropertyItem("DisplayMember", Properties.Resources.DisplayMemberCaption, DataBindingCategoryName));
			result.Add(new DesignerActionPropertyItem("ValueMember", Properties.Resources.ValueMemberCaption, DataBindingCategoryName));
			return result;
		}
		protected virtual void CreateDataSource() { }
		protected void RefreshContent() {
			DesignerActionUIService serv = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			if(serv != null)
				serv.Refresh(ListControl);
		}
#if DXWhidbey
		[AttributeProvider(typeof(IListSource))]
#endif
		public object DataSource {
			get { return DataSourceCore; }
			set {
				if(DataSource == value) return;
				DataSourceCore = value;
				if(DataSource == null) {
					DisplayMember = string.Empty;
					ValueMember = string.Empty;
					OnRefreshContent();
				}
				FireChanged();
			}
		}
		protected virtual void OnRefreshContent() {
			RefreshContent();
		}
		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		public string DisplayMember {
			get { return DisplayMemberCore; }
			set {
				if(DisplayMember == value) return;
				DisplayMemberCore = value;
				FireChanged();
			}
		}
		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		public string ValueMember {
			get { return ValueMemberCore; }
			set {
				if(ValueMember == value) return;
				ValueMemberCore = value;
				FireChanged();
			}
		}
		protected abstract void FireChanged();
		protected string DataBindingCategoryName { get { return Properties.Resources.DataBindingModeCaption; } }
		protected abstract object DataSourceCore { get; set; }
		protected abstract string DisplayMemberCore { get; set; }
		protected abstract string ValueMemberCore { get; set; }
		protected abstract Control ListControl { get; }
	}
	public class BaseListBoxDesigner : BaseControlDesigner {
		protected override bool CanUseComponentSmartTags { get { return true; } }
		protected class ListBoxActionList : ListControlActionList {
			BaseListBoxDesigner designer;
			bool boundMode;
			public ListBoxActionList(BaseListBoxDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
				this.boundMode = (DataSource != null);
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(Designer.ListBox == null)
					return new DesignerActionItemCollection();
				DesignerActionItem firstItem = new DesignerActionPropertyItem("BoundMode", Properties.Resources.UseDataBoundItemsCaption, DataBindingCategoryName);
				if(boundMode || (DataSource != null)) {
					boundMode = true;
					DesignerActionItemCollection result = base.GetSortedActionItems();
					result.Insert(0, firstItem);
					return result;
				}
				DesignerActionItemCollection unboundsResult = new DesignerActionItemCollection();
				unboundsResult.Add(firstItem);			
				return unboundsResult;
			}
			protected override void FireChanged() { EditorContextHelper.FireChanged(Designer, ListControl); }
			protected override void CreateDataSource() {
				designer.CreateDataSource();
			}
			protected override object DataSourceCore { get { return ListBox.DataSource; } set { ListBox.DataSource = value; } }
			protected override string DisplayMemberCore { get { return ListBox.DisplayMember; } set { ListBox.DisplayMember = value; } }
			protected override string ValueMemberCore { get { return ListBox.ValueMember; } set { ListBox.ValueMember = value; } }
			protected override Control ListControl { get { return ListBox; } }
			protected BaseListBoxDesigner Designer { get { return designer; } }
			protected BaseListBoxControl ListBox { get { return Designer.ListBox; } }
			public bool BoundMode {
				get { return boundMode; }
				set {
					if(!value) DataSource = null;
					if(DataSource == null) this.boundMode = value;
					OnRefreshContent();
					if(value)
						DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(ListBox, ContentAlignment.BottomLeft, false, false);
					else
						DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(ListBox);
				}
			}
			protected override void OnRefreshContent() {
				if(!Designer.CanUseComponentSmartTags) base.OnRefreshContent();
				else BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(this.Component);
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IComponentChangeService cs = GetService((typeof(IComponentChangeService))) as IComponentChangeService;
			if(cs != null)
				cs.ComponentRename += OnRenameComponent;
			IDesignerHost host = GetService((typeof(IDesignerHost))) as IDesignerHost;
			if(host != null)
				host.LoadComplete += OnDesignerHostLoadComplete;
			ListBox.DataSourceChanged += ListBox_DataSourceChanged;
		}
		void OnDesignerHostLoadComplete(object sender, EventArgs e) {
			if(ListBox != null && ListBox.DataSource != null)
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(ListBox, ContentAlignment.BottomLeft, false, false);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ListBox != null)
					ListBox.DataSourceChanged -= ListBox_DataSourceChanged;
				IDesignerHost host = GetService((typeof(IDesignerHost))) as IDesignerHost;
				if(host != null)
					host.LoadComplete -= OnDesignerHostLoadComplete;
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(ListBox);
				IComponentChangeService cs = GetService((typeof(IComponentChangeService))) as IComponentChangeService;
				if(cs != null)
					cs.ComponentRename -= OnRenameComponent;
			}
			base.Dispose(disposing);
		}
		void ListBox_DataSourceChanged(object sender, EventArgs e) {
			DevExpress.Design.DataAccess.UI.DataSourceGlyph.DataSourceChanged(ListBox);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new ListBoxActionList(this));
			base.RegisterActionLists(list);
		}
		protected virtual void OnRenameComponent(object sender, ComponentRenameEventArgs e) {
			if(e.Component == ListBox) ListBox.Refresh();
		}
		public override IList SnapLines {
			get {
				ArrayList res = new ArrayList();
				res.AddRange(base.SnapLines);
				if(ListBox != null) {
					Rectangle tb = ListBox.GetBaseTextBounds();
					res.Add(new SnapLine(SnapLineType.Baseline, tb.Bottom, SnapLinePriority.Medium));
					UpdateSnapLine(res, SnapLineType.Baseline, tb.Bottom);
				}
				return res;
			}
		}
		protected virtual bool UpdateSnapLine(ArrayList lines, SnapLineType type, int newOffset) {
			if(lines == null || lines.Count == 0) return false;
			foreach(SnapLine line in lines) {
				if(line.SnapLineType == type) {
					line.AdjustOffset(newOffset - line.Offset);
					return true;
				}
			}
			return false;
		}
		public BaseListBoxControl ListBox { get { return Control as BaseListBoxControl; } }
	}
}
