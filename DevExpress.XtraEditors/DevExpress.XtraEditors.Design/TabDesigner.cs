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
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.About;
namespace DevExpress.XtraTab.Design {
	public class XtraTabControlDesigner : BaseParentControlDesignerSimple {
		XtraTabPage selectedTabPage = null;
		private XtraTabPage SelectedTabPage {
			get {
				if(selectedTabPage != null && !TabControl.TabPages.Contains(selectedTabPage)) selectedTabPage = null;
				if(selectedTabPage == null) selectedTabPage = TabControl.SelectedTabPage;
				return selectedTabPage;
			}
			set {
				TabControl.SelectedTabPage = value;
				selectedTabPage = value;
			}
		}
		DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionLists != null) return actionLists;
				actionLists = CreateActionList();
				return base.ActionLists;
			}
		}
		DesignerActionListCollection CreateActionList() {
			DesignerActionListCollection res = new DesignerActionListCollection();
			DXSmartTagsHelper.CreateDefaultLinks(this, res);
			return res;
		}
		protected XtraTabPage AddPage() {
			XtraTabPage page = TabControl.TabPages.Add();
			TabControl.Container.Add(page);
			page.Text = page.Name;
			return page;
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
		public virtual XtraTabControl TabControl { get { return Control as XtraTabControl; } }
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			DesignTimeHelper.UpdateDesignTimeLookAndFeel(component);
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			LoaderPatcherService.InstallService(host);
			TabControl.SelectedPageChanged += new TabPageChangedEventHandler(OnTabControl_SelectedPageChanged);
		}
		const string SelectedTabPageProperty = "SelectedTabPage";
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			PropertyDescriptor pd = properties[SelectedTabPageProperty] as PropertyDescriptor;
			if(pd != null) properties[SelectedTabPageProperty] = TypeDescriptor.CreateProperty(typeof(XtraTabControlDesigner), pd, new Attribute[0]);
		}
		protected virtual void OnTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			if(SelectionService != null && e.Page != null && e.Page.Site != null) SelectionService.SetSelectedComponents(new object[] { TabControl },
				ControlConstants.SelectionClick
				);
		}
		protected override void Dispose(bool disposing) {
			LoaderPatcherService.UnInstallService(host);
			this.host = null;
			if(disposing) {
				TabControl.SelectedPageChanged -= new TabPageChangedEventHandler(OnTabControl_SelectedPageChanged);
			}
			base.Dispose(disposing);
		}
		public override bool CanParent(Control control) {
			return (control is XtraTabPage);
		}
		bool allowDrawGrid = true;
		protected override bool DrawGrid {
			get { return this.allowDrawGrid && base.DrawGrid; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			AddPage();
			AddPage();
			TabControl.SelectedTabPageIndex = 0;
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			try {
				this.allowDrawGrid = false;
				base.OnPaintAdornments(pe);
			}
			finally {
				this.allowDrawGrid = true;
			}
		}
		protected ISelectionService SelectionService {
			get {
				return GetService(typeof(ISelectionService)) as ISelectionService;
			}
		}
		protected bool IsSelected {
			get {
				if(SelectionService == null) return false;
				if(SelectionService.GetComponentSelected(TabControl)) return true;
				XtraTabPage page = SelectionService.PrimarySelection as XtraTabPage;
				if(page != null && page.TabControl == TabControl) return true;
				return false;
			}
		}
		protected override bool GetHitTest(Point point) {
			if(!IsSelected) return false;
			point = TabControl.PointToClient(point);
			XtraTabHitInfo hitInfo = TabControl.CalcHitInfo(point);
			if(hitInfo.HitTest == XtraTabHitTest.PageHeader || hitInfo.HitTest == XtraTabHitTest.PageHeaderButtons) return true;
			return false;
		}
	}
	public class TabSelectedPageConverter : ComponentConverter {
		public TabSelectedPageConverter(Type type) : base(type) { }
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			if(context == null || context.Instance == null) return false;
			XtraTabControl control = context.Instance as XtraTabControl;
			XtraTabPage page = value as XtraTabPage;
			return page == null || (control != null && page != null && page.TabControl == control);
		}
	}
	public class XtraTabPageDesigner : BaseScrollableControlDesigner {
		public override SelectionRules SelectionRules {
			get {
				SelectionRules rules = SelectionRules.Locked;
				return rules;
			}
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			TabPage.Text = TabPage.Name;
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			TabPage.Text = TabPage.Name;
		}
#endif
		public override bool CanBeParentedTo(IDesigner parentDesigner) {
			return (parentDesigner is XtraTabControlDesigner);
		}
		public override void Initialize(IComponent component) {
			bool prevVisible = ((Control)component).Visible;
			base.Initialize(component);
			Control.Visible = prevVisible;
			if(SelectionService != null) SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(SelectionService != null) SelectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
			}
			base.Dispose(disposing);
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
		bool prevSelected = false;
		protected void OnSelectionChanged(object sender, EventArgs e) {
			if(Control == null) return;
			if(this.prevSelected != IsSelected) {
				this.prevSelected = IsSelected;
				Control.Invalidate();
				if(IsSelected && TabControl != null) {
					TabControl.SelectedTabPage = TabPage;
				}
			}
		}
		public virtual XtraTabPage TabPage {
			get { return Control as XtraTabPage; }
		}
		public virtual XtraTabControl TabControl {
			get {
				if(TabPage != null) return TabPage.TabControl;
				return null;
			}
		}
		protected ISelectionService SelectionService {
			get {
				return GetService(typeof(ISelectionService)) as ISelectionService;
			}
		}
		protected bool IsSelected {
			get {
				if(SelectionService == null) return false;
				if(SelectionService.GetComponentSelected(Control)) return true;
				return false;
			}
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			Rectangle outside = Control.ClientRectangle, inside;
			inside = outside;
			inside.Inflate(-3, -3);
			if(IsSelected)
				ControlPaint.DrawSelectionFrame(pe.Graphics, true, outside, inside, SystemColors.Control);
			base.OnPaintAdornments(pe);
		}
	}
	public class TabPaintStyleNameConverter : TypeConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			XtraTabControl tab = context.Instance as XtraTabControl;
			if(tab == null) return null;
			ArrayList list = new ArrayList();
			list.Add(BaseViewInfoRegistrator.DefaultViewName);
			foreach(BaseViewInfoRegistrator info in PaintStyleCollection.DefaultPaintStyles) {
				list.Add(info.ViewName);
			}
			return new StandardValuesCollection(list);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
	}
	public class XtraTabPageCollectionEditor : DXCollectionEditorBase {
		Type realTabPageTypeCore = null;
		public XtraTabPageCollectionEditor(Type collectionType)
			: base(collectionType) {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(realTabPageTypeCore == null) TryGetRealTabPageType(value, ref realTabPageTypeCore);
			return base.EditValue(context, provider, value);
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { RealTabPageType };
		}
		protected Type RealTabPageType {
			get { return realTabPageTypeCore; }
		}
		void TryGetRealTabPageType(object value, ref Type pageType) {
			try {
				MethodInfo mi = value.GetType().GetMethod("CreatePage", BindingFlags.NonPublic | BindingFlags.Instance);
				if(mi != null) {
					object result = mi.Invoke(value, new object[] { });
					if(result != null) {
						pageType = result.GetType();
						if(result is IDisposable) ((IDisposable)result).Dispose();
						result = null;
					}
				}
			}
			catch { pageType = typeof(XtraTabPage); }
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
	}
}
