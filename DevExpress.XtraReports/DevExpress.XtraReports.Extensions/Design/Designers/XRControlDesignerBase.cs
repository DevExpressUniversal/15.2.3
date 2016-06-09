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
using System.Text;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraReports.Native;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Design.MouseTargets;
namespace DevExpress.XtraReports.Design {
	public abstract class XRControlDesignerBase : XRComponentDesigner {
		#region static
		public static void RaiseCollectionChanging(XRControl control, IComponentChangeService componentChangeService) {
			componentChangeService.OnComponentChanging(control,
				XRAccessor.GetPropertyDescriptor(control, ReportDesignerHelper.GetDefaultCollectionName(control)));
		}
		public static void RaiseCollectionChanged(XRControl control, IComponentChangeService componentChangeService) {
			componentChangeService.OnComponentChanged(control,
				XRAccessor.GetPropertyDescriptor(control, ReportDesignerHelper.GetDefaultCollectionName(control)), null, null);
		}
		public static void RaiseComponentsChanging(IComponentChangeService changeServ, IList components, params string[] properties) {
			if(components != null) {
				foreach(IComponent component in components)
					RaiseComponentChanging(changeServ, component, properties);
			}
		}
		public static void RaiseComponentChanging(IComponentChangeService changeServ, IComponent component, params string[] properties) {
			if(changeServ != null && component != null) {
				foreach(string propName in properties) {
					RaiseComponentChanging(changeServ, component, propName);
				}
			}
		}
		public static void RaiseComponentChanging(IComponentChangeService changeServ, IComponent component, string propName) {
			PropertyDescriptor property = XRAccessor.GetPropertyDescriptor(component, propName);
			changeServ.OnComponentChanging(component, property);
		}
		public static void RaiseComponentChanged(IComponentChangeService changeServ, IComponent component) {
			RaiseComponentChanged(changeServ, component, null);
		}
		public static void RaiseComponentChanged(IComponentChangeService changeServ, IComponent component, MemberDescriptor member) {
			RaiseComponentChanged(changeServ, component, member, null, null);
		}
		public static void RaiseComponentChanged(IComponentChangeService changeServ, IComponent component, MemberDescriptor member, object oldValue, object newValue) {
			if(changeServ != null && component != null)
				changeServ.OnComponentChanged(component, member, oldValue, newValue);
		}
		public static void RaiseComponentChanged(IComponentChangeService changeServ, IComponent component, string propName, object oldValue, object newValue) {
			MemberDescriptor member = string.IsNullOrEmpty(propName) ? null : XRAccessor.GetPropertyDescriptor(component, propName);
			RaiseComponentChanged(changeServ, component, member, oldValue, newValue);
		}
		public static void RaiseComponentsChanged(IComponentChangeService changeServ, IList components) {
			if(changeServ != null && components != null) {
				foreach(IComponent component in components)
					changeServ.OnComponentChanged(component, null, null, null);
			}
		}
		static bool IsModifierKeysPressed() {
			return (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift);
		}
		public static bool CursorEquals(params Cursor[] cursors) {
			foreach(Cursor cursor in cursors) {
				if(Comparer.Equals(Cursor.Current, cursor))
					return true;
			}
			return false;
		}
		public static bool IsFirstChild(XRControl ctl) {
			return ctl.Index == 0;
		}
		public static bool IsLastChild(XRControl ctl) {
			return ctl.Index == ctl.Parent.Controls.Count - 1;
		}
		protected static void RaiseExpandedChanging(XRControlDesignerBase designer) {
			designer.RaiseComponentChanging(designer.XRControl, new string[] { DesignPropertyNames.Expanded });
		}
		protected static void RaiseExpandedChanged(XRControlDesignerBase designer) {
				designer.RaiseComponentChanged(designer.XRControl);
		}
		#endregion
		protected ISelectionService selectionService;
		protected ZoomService zoomService;
		ReportDesigner reportDesigner;
		public abstract Band Band { get; }
		public override ICollection AssociatedComponents {
			get {
				ArrayList comps = null;
				foreach(XRControl ctl in XRControl.Controls) {
					if(ctl != null && ctl.Site != null) {
						if(comps == null)
							comps = new ArrayList();
						comps.Add(ctl);
					}
				}
				return (comps != null) ? comps : base.AssociatedComponents;
			}
		}
		public IDesignerHost DesignerHost {
			get { return fDesignerHost; }
		}
		public bool IsComponentSelected {
			get { return selectionService.GetComponentSelected(Component); }
		}
		public override bool CanDragInReportExplorer {
			get { return LockService.GetInstance(DesignerHost).CanDeleteComponent(XRControl); }
		}
		protected virtual bool CanDrop(Type type) {
			return typeof(XRControl).IsAssignableFrom(type);
		}
		protected ResizeService ResizeService {
			get { return GetService(typeof(ResizeService)) as ResizeService; }
		}
		protected ReportDesigner ReportDesigner {
			get { return reportDesigner; }
		}
		protected XtraReport RootReport {
			get { return (XtraReport)fDesignerHost.RootComponent; }
		}
		protected virtual bool CanAddToSelection {
			get {
				foreach(object obj in selectionService.GetSelectedComponents())
					if(obj is Band)
						return false;
				return true;
			}
		}
		[
		Browsable(false),
		DefaultValue(false),
		DesignOnly(true),
		]
		public virtual bool UserDesignerLocked {
			get { return XRControl.LockedInUserDesigner; }
			set { XRControl.LockedInUserDesigner = value; }
		}
		public virtual RectangleF ControlBoundsRelativeToBand {
			get { return XRControl.BoundsRelativeToBand; }
		}
		public bool Locked {
			get { return !LockService.GetInstance(DesignerHost).CanChangeComponent(XRControl); }
		}
		public XRControlDesignerBase()
			: base() {
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			reportDesigner = fDesignerHost.GetDesigner(fDesignerHost.RootComponent) as ReportDesigner;
			fDesignerHost.Deactivated += new EventHandler(OnDeactivated);
			if(fDesignerHost.Loading)
				fDesignerHost.LoadComplete += new EventHandler(this.OnLoadComplete);
			selectionService = fDesignerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			selectionService.SelectionChanging += new EventHandler(OnSelectionChanging);
			zoomService = ZoomService.GetInstance(fDesignerHost);
		}
		void InvalidateBandViewInfo() {
			if(DesignerHost == null)
				return;
			IBandViewInfoService serv = DesignerHost.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
			if(serv == null)
				return;
			serv.InvalidateViewInfo();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				InvalidateBandViewInfo();
				fDesignerHost.Deactivated -= new EventHandler(OnDeactivated);
				selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
				selectionService.SelectionChanging -= new EventHandler(OnSelectionChanging);
			}
			base.Dispose(disposing);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			if(DesignToolHelper.IsEndUserDesigner(XRControl.Site))
				properties.Remove(XRComponentPropertyNames.LockedInUserDesigner);
			else
				PropInfoAccessor.SetDesignProperty(this, properties, DesignPropertyNames.UserDesignerLocked);
			base.PreFilterProperties(properties);
		}
		private void OnLoadComplete(object sender, EventArgs e) {
			((IDesignerHost)sender).LoadComplete -= new EventHandler(this.OnLoadComplete);
			OnLoadComplete(e);
		}
		public void SelectComponents(ICollection components, SelectionTypes selectionType) {
			selectionService.SetSelectedComponents(components, selectionType);
		}
		protected IList GetSelectedComponents() {
			return new ArrayList(selectionService.GetSelectedComponents());
		}
		protected void RaiseComponentsChanging(IList components, params string[] properties) {
			RaiseComponentsChanging(changeService, components, properties);
		}
		protected void RaiseComponentChanging(IComponent component, params string[] properties) {
			RaiseComponentChanging(changeService, component, properties);
		}
		protected void RaiseComponentsChanged(IList components) {
			if(components != null) {
				foreach(IComponent component in components)
					RaiseComponentChanged(component);
			}
		}
		protected void RaiseComponentChanged(IComponent component) {
			changeService.OnComponentChanged(component, null, null, null);
		}
		protected void RaiseComponentChanged(IComponent component, string propName, object oldValue, object newValue) {
			changeService.OnComponentChanged(component, XRAccessor.GetPropertyDescriptor(component, propName), oldValue, newValue);
		}
		protected virtual void OnDeactivated(object sender, EventArgs e) {
		}
		protected virtual void OnSelectionChanged(object sender, EventArgs e) {
		}
		protected virtual void OnSelectionChanging(object sender, EventArgs e) {
		}
		protected virtual void OnLoadComplete(EventArgs e) {
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			InitializeNewComponentCore();
		}
		public virtual void InitializeNewComponentCore() {
			InitializeParent();
		}
		protected virtual void InitializeParent() {
			if(XRControl.Parent == null) {
				XRControl parent = FindParent();
				if(parent != null && parent.CanAddComponent(XRControl)) {
					XRComponentDesigner designer = fDesignerHost.GetDesigner(parent) as XRComponentDesigner;
					if(designer != null) {
						designer.OnCollectionChanging(XRControl);
						designer.AddComponent(XRControl);
						designer.OnCollectionChanged(XRControl);
					} else 
						parent.Controls.Add(XRControl);
				}
			}
		}
		public virtual SelectionRules GetSelectionRules() {
			return SelectionRules.AllSizeable;
		}
		public virtual SelectionRules GetSelectionRules(Band band) {
			return GetSelectionRules();
		}
		public virtual void SetBounds(RectangleF rect) {
			ControlBoundsSetter.SetBounds(XRControl, rect, changeService);
		}
		public virtual void SetSize(SizeF value, bool raiseChanged) { 
		}
		public virtual void SetRightBottom(PointF value, SizeF stepSize, RectangleSpecified specified, bool raiseChanged) { 
		}
		public virtual bool CanCutControl {
			get { return true; }
		}
		public virtual bool CanHaveChildren {
			get { return XRControl.CanHaveChildren; }
		}
		public virtual bool CanAddControl(Type type) {
			return XRControl.CanAddControl(type, null);
		}
		public virtual string GetStatus() {
			try {
				return Component.Site.Name;
			} catch { return ""; }
		}
		public virtual string GetToolTip(XRDesignerVerb verb) {
			return "";
		}
		protected virtual XRControl FindParent() {
			return null;
		}
		protected ToolboxItem GetSelectedToolboxItem() {
			IToolboxService tbxService = GetToolboxService();
			return (tbxService != null) ? tbxService.GetSelectedToolboxItem(fDesignerHost) : null;
		}
		protected IToolboxService GetToolboxService() {
			return GetService(typeof(IToolboxService)) as IToolboxService;
		}
		public virtual void OnKeyCancel(CancelEventArgs e) {
		}
		public virtual void SelectComponent() {
			SelectComponents(new object[] { Component }, GetSelectionTypes());
		}
		protected virtual SelectionTypes GetSelectionTypes() {
			return CanAddToSelection ? (IsModifierKeysPressed() ? ControlConstants.SelectionTypeAuto : SelectionTypes.Primary) : SelectionTypes.Replace;
		}
		public void DeleteComponent() {
			if(XRControl == null || XRControl.IsDisposed)
				return;
			DesignerTransaction trans = DesignerHost.CreateTransaction(DesignSR.Trans_Delete);
			XRControl parent = XRControl.Parent;
			ArrayList comps = new ArrayList();
			selectionService.SetSelectedComponents(new object[0], SelectionTypes.Replace);
			CursorStorage.SetCursor(Cursors.WaitCursor);
			try {
				GetAssociatedComponents(XRControl, DesignerHost, comps);
				foreach(IComponent comp in comps) {
					XRControl control = comp as XRControl;
					if(control != null && control.IsDisposed)
						continue;
					changeService.OnComponentChanging(comp, null);
				}
				if(!XRControl.IsDisposed)
					XRControl.Dispose();
			} finally {
				trans.Commit();
				CursorStorage.RestoreCursor();
			}
			if(parent != null) {
				if(!parent.IsDisposed)
					selectionService.SetSelectedComponents(new object[] { parent }, SelectionTypes.Replace);
			} else
				selectionService.SetSelectedComponents(new object[] { DesignerHost.RootComponent }, SelectionTypes.Replace);
		}
		private void GetAssociatedComponents(IComponent component, IDesignerHost host, ArrayList list) {
			ComponentDesigner designer = host.GetDesigner(component) as ComponentDesigner;
			if(designer == null) {
				return;
			}
			foreach(IComponent childComp in designer.AssociatedComponents) {
				list.Add(childComp);
				GetAssociatedComponents(childComp, host, list);
			}
		}
		internal void SetStyle(string propName, XRControlStyle newStyle) {
			if(XRControl == null)
				return;
			string serializableStylePropertyName = string.Format("{0}Name", propName);
			DesignerTransaction transaction = DesignerHost.CreateTransaction(string.Format("Change {0}.Styles.{1}", XRControl.Name, propName));
			try {
				RaiseComponentChanging(XRControl, serializableStylePropertyName);
				TypeDescriptor.GetProperties(XRControl.Styles)[propName].SetValue(XRControl.Styles, newStyle);
				RaiseComponentChanged(XRControl, serializableStylePropertyName, null, null);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
		internal void AddRule(FormattingRule rule) {
			if(XRControl == null || XRControl.FormattingRules.Contains(rule))
				return;
			DesignerTransaction transaction = DesignerHost.CreateTransaction(string.Format(DesignSR.TransFmt_ChangeFormattingRules, XRControl.Name));
			try {
				RaiseComponentChanging(XRControl, XRComponentPropertyNames.FormattingRules);
				XRControl.FormattingRules.Add(rule);
				RaiseComponentChanged(XRControl, XRComponentPropertyNames.FormattingRules, null, null);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
	}
}
