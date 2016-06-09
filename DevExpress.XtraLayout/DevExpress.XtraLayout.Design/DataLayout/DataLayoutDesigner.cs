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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using DevExpress.XtraLayout;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraLayout.DesignTime;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraDataLayout.DesignTime {
	public class DataLayoutDesigner : DevExpress.XtraLayout.DesignTime.LayoutControlDesigner {
		DataSourceForm dataSourceDindingManager;
		public DataLayoutDesigner() {
		}
		public override void OnChanging(object sender, CancelEventArgs e) {
			foreach(Adorner adroner in BehaviorService.Adorners) {
				adroner.Enabled = false;
			}
			base.OnChanging(sender, e);
		}
		public override void OnChanged(object sender, EventArgs e) {
			base.OnChanged(sender, e);
			foreach(Adorner adroner in BehaviorService.Adorners) {
				adroner.Enabled = true;
			}
			InvalidateBehaviorService();
		}
		protected override void ShowDesignerElements(object sender, EventArgs e) {
			base.ShowDesignerElements(sender, e);
		}
		protected override void HideDesignerElements(object sender, EventArgs e) {
			base.HideDesignerElements(sender, e);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(ToolBoxService != null)
				ToolBoxService.SetSelectedToolboxItem(null);
			if(ShowWizardOnComponentAdding) ShowWizard();
		}
		public const string XtraDataLayoutRegistryPath = "Software\\Developer Express\\XtraDataLayout\\";
		public const string XtraDataLayoutShowWizardRegistryEntry = "ShowWizard";
		public static bool ShowWizardOnComponentAdding {
			get {
				PropertyStore store = new PropertyStore(XtraDataLayoutRegistryPath);
				if(store == null)
					return true;
				store.Restore();
				return store.RestoreBoolProperty(XtraDataLayoutShowWizardRegistryEntry, true);
			}
			set {
				PropertyStore store = new PropertyStore(XtraDataLayoutRegistryPath);
				if(store == null)
					return;
				store.AddProperty(XtraDataLayoutShowWizardRegistryEntry, value);
				store.Store();
			}
		}
		protected virtual void ShowWizard() {
			DataLayoutWizardForm wizardForm = new DataLayoutWizardForm(this);
			wizardForm.Init();
			wizardForm.ShowDialog();
		}
		protected DataSourceForm DataSourceBindingsManager {
			get {
				if(dataSourceDindingManager == null || !dataSourceDindingManager.Visible) dataSourceDindingManager = new DataSourceForm(this.Component);
				return dataSourceDindingManager;
			}
		}
		protected internal void ShowDataSourceBindingsManager(object sender, EventArgs e) {
			DataSourceForm form = DataSourceBindingsManager;
			form.RefreshView();
			form.ShowDialog();
			if(form.DialogResult == DialogResult.OK) UpdateLayout(form.BindingInfo);
		}
		protected internal void UpdateLayout(LayoutElementsBindingInfo bi, bool shouldUseFlow = false) {
			LayoutCreator creator = Component.CreateLayoutCreator();
			DisableUndoEngine();
			creator.CreateLayout(bi, shouldUseFlow);
			EnableUndoEngine();
		}
		public new DataLayoutControl Component {
			get {
				return (DataLayoutControl)base.Component;
			}
		}
		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			GlyphCollection glyphs = base.GetGlyphs(selectionType);
			glyphs.AddRange(CreateBindingGlyphs());
			return glyphs;
		}
		public override void OnSelectionChanged(object sender, EventArgs e) {
			base.OnSelectionChanged(sender, e);
			if(!isChangingInProgess) InvalidateBehaviorService();
		}
		protected void InvalidateBehaviorService() {
			ILayoutDesignerMethods methods = Component as ILayoutDesignerMethods;
			if(methods != null && methods.CanInvokePainting)
				BehaviorService.Invalidate();
		}
		protected string GetBindingGlyphCaption(Binding binding) {
			if(binding.DataSource == Component.DataSource) {
				ICollection col = ((DataLayoutControlDesignerMethods)Component).GetFieldsList();
				foreach(DataColumnInfo dci in col) {
					if(dci.Name == binding.BindingMemberInfo.BindingField)
						return dci.Caption;
				}
			}
			return "@" + binding.BindingMemberInfo.BindingField;
		}
		protected Glyph[] CreateBindingGlyphs() {
			ArrayList glyphs = new ArrayList();
			foreach(BaseLayoutItem item in Component.Items) {
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci != null) {
					if(!AllowShowGlyph(lci)) continue;
					if(lci.Control != null) {
						if(!lci.Control.Visible) continue;
						Rectangle controlRectangle = lci.Control.Bounds;
						String caption = "unbound";
						if(lci.Control.DataBindings.Count > 0)
							caption = GetBindingGlyphCaption(lci.Control.DataBindings[0]);
						BindingGlyph glyph = new BindingGlyph(visualizersBehaviourCore, controlRectangle, new Rectangle(Point.Empty, Component.Size), caption);
						glyphs.Add(glyph);
					}
				}
			}
			return (Glyph[])glyphs.ToArray(typeof(Glyph));
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner)
				list.Add(new ActionList(this));
			base.RegisterActionLists(list);
		}
		public class ActionList : DesignerActionList {
			DataLayoutDesigner designer;
			public ActionList(DataLayoutDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			public DataLayoutControl DataLayout { get { return designer.Component; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				if(DataLayout == null) return res;
				res.Add(new DesignerActionPropertyItem("DataSource", "Choose Data Source"));
				if(DataLayout.DataSource != null) res.Add(new DesignerActionMethodItem(this, "ShowDataSourceBindingsManager", "Run Data Source Bindings Manager", true));
				res.Add(new DesignerActionMethodItem(this, "ShowWizard", "Run Wizard", true));
				return res;
			}
			public void ShowWizard() {
				designer.ShowWizard();
			}
			public void ShowDataSourceBindingsManager() {
				designer.ShowDataSourceBindingsManager(null, EventArgs.Empty);
			}
			[AttributeProvider(typeof(IListSource))]
			public object DataSource {
				get {
					if(DataLayout == null) return null;
					return DataLayout.DataSource; }
				set {
					DevExpress.Utils.Design.EditorContextHelper.SetPropertyValue(designer, DataLayout, "DataSource", value);
				}
			}
		}
	}
	public class DragBehavior : VisualizersBehavior {
		public DragBehavior(LayoutControlDesigner owner) : base(owner) { }
		Point initialPoint = Point.Empty;
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			DragWidgetGlyph dgw = g as DragWidgetGlyph;
			if(dgw != null && button == MouseButtons.Left) {
				initialPoint = mouseLoc;
				return true;
			}
			return base.OnMouseDown(g, button, mouseLoc);
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			initialPoint = Point.Empty;
			return true;
		}
		protected Point ConvertToScreen(Point point) {
			Rectangle crectangle = OwnerRectInAdronerWindow();
			crectangle.Offset(point);
			return new Point(crectangle.X, crectangle.Y);
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) {
			if(button == MouseButtons.Left) {
				if((Math.Abs((int)(this.initialPoint.X - mouseLoc.X)) > DragDropDispatcherFactory.MinDragSize) || (Math.Abs((int)(this.initialPoint.Y - mouseLoc.Y)) > DragDropDispatcherFactory.MinDragSize)) {
					Point zeroPoint = ConvertToScreen(Point.Empty);
					mouseLoc = new Point(mouseLoc.X - zeroPoint.X, mouseLoc.Y - zeroPoint.Y);
					initialPoint = new Point(initialPoint.X - zeroPoint.X, initialPoint.Y - zeroPoint.Y);
					owner.Component.Capture = true;
					((ILayoutControl)owner.Component).ActiveHandler.ProcessMessage(DevExpress.Utils.Controls.EventType.MouseDown, new MouseEventArgs(MouseButtons.Left, 0, initialPoint.X, initialPoint.Y, 0));
					DragDropDispatcherFactory.Default.ProcessMouseEvent(((IDragDropDispatcherClient)owner.Component).ClientDescriptor, new ProcessEventEventArgs(EventType.MouseDown, new MouseEventArgs(MouseButtons.Left, 0, initialPoint.X, initialPoint.Y, 0)));
					DragDropDispatcherFactory.Default.ProcessMouseEvent(((IDragDropDispatcherClient)owner.Component).ClientDescriptor, new ProcessEventEventArgs(EventType.MouseMove, new MouseEventArgs(MouseButtons.Left, 0, mouseLoc.X, mouseLoc.Y, 0)));
					return true;
				}
			}
			return base.OnMouseMove(g, button, mouseLoc);
		}
	}
	public class ResizeEmptyPaddingBehaviour :VisualizersBehavior {
		public ResizeEmptyPaddingBehaviour(LayoutControlDesigner owner) : base(owner) { }
		public override bool OnMouseEnter(Glyph g) {
			owner.Component.Capture = true;
			return true;
		}
		public override bool OnMouseLeave(Glyph g) {
			if(((ILayoutControl)owner.Component).ActiveHandler.State == LayoutHandlerState.Normal) owner.Component.Capture = false;
			return true;
		}
		protected Point ConvertToScreen(Point point) {
			Rectangle crectangle = OwnerRectInAdronerWindow();
			crectangle.Offset(point);
			return new Point(crectangle.X, crectangle.Y);
		}
	}
	public class RightButtonBehavior :VisualizersBehavior {
		public RightButtonBehavior(LayoutControlDesigner owner) : base(owner) { }
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			RightButtonGlyph rbg = g as RightButtonGlyph;
			if(rbg != null && (button == MouseButtons.Right || button == MouseButtons.Left)) {
				try {
					Point zeroPoint = ConvertToScreen(Point.Empty);
					mouseLoc = new Point(mouseLoc.X - zeroPoint.X, mouseLoc.Y - zeroPoint.Y);
					owner.ShouldClearSelection = false;
					((ILayoutControl)owner.Component).CustomizationMenuManager.ShowPopUpMenu(mouseLoc);
				} catch { } finally {
					owner.ShouldClearSelection = true;
				}
				return true;
			}
			return base.OnMouseDown(g, button, mouseLoc);
		}
		protected Point ConvertToScreen(Point point) {
			Rectangle crectangle = OwnerRectInAdronerWindow();
			crectangle.Offset(point);
			return new Point(crectangle.X, crectangle.Y);
		}
	}
	public class VisualizersBehavior : Behavior {
		protected LayoutControlDesigner owner;
		public VisualizersBehavior(LayoutControlDesigner owner) {
			this.owner = owner;
		}
		public BehaviorService BehaviorService { get { return owner.BehaviorServiceCore; } }
		public Rectangle OwnerRectInAdronerWindow() {
			if (owner == null || owner.Component == null ||BehaviorService == null) return Rectangle.Empty;
			return this.BehaviorService.ControlRectInAdornerWindow(owner.Component);
		}
	}
}
