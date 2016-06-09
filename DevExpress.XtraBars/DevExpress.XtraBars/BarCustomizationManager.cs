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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraBars.Customization.Helpers;
namespace DevExpress.XtraBars.Utils {
	public class BarCustomizationManager : IDisposable {
		DevExpress.XtraBars.Customization.Helpers.DesignTimeManager designTimeManager;
		BarManager manager;
		CustomizationForm customizationForm;
		PopupMenuEditor menuEditor;
		bool isCustomizing, isHotCustomizing;
		ISelectionService selectionService;
		ICollection selectedObjects;
		public BarCustomizationManager(BarManager manager) {
			this.selectedObjects = null;
			this.selectionService = null;
			this.manager = manager;
			this.isCustomizing = this.isHotCustomizing = false;
			this.menuEditor = null;
			this.designTimeManager = null;
			this.customizationForm = null;
		}
		public virtual void Dispose() {
			if(this.designTimeManager != null) designTimeManager.Dispose();
			this.designTimeManager = null;
			this.selectedObjects = null;
			if(this.selectionService != null) {
				this.selectionService.SelectionChanged -= new EventHandler(OnDesignTimeSelectionChanged);
				this.selectionService = null;
			}
			if (CustomizationForm != null && !CustomizationForm.IsDisposed)
				CustomizationForm.Dispose();
		} 
		public virtual void DrawDesignTimeSelection(GraphicsInfoArgs e, Rectangle bounds, int alpha) {
			Pen pen = e.Cache.GetPen(Color.FromArgb(alpha, ColorUtils.FlatBarBorderColor));
			Brush brush = e.Cache.GetSolidBrush(Color.FromArgb(alpha, Manager.DrawParameters.Colors[BarColor.DesignTimeSelectColor]));
			e.Paint.DrawRectangle(e.Graphics, pen, bounds);
			bounds.Inflate(-1, -1);
			e.Paint.FillRectangle(e.Graphics, brush, bounds);
		}
		public ISelectionService SelectionService { get { return selectionService; } }
		public ICollection PrevSelectedObjects { get { return selectedObjects; } }
		public ICollection SelectedObjects { 
			get {
				InitSelectionService();
				if(SelectionService == null) return null;
				return SelectionService.GetSelectedComponents();
			}
		}
		public virtual void SelectObject(object obj) {
			if(!Manager.IsDesignMode) return;
			InitSelectionService();
			if(SelectionService == null) return;
			SelectionService.SetSelectedComponents(obj == null ? null : new object[] { obj });
		}
		public bool GetObjectSelected(object component) {
			if(component == null) return false;
			if(SelectionService != null) return SelectionService.GetComponentSelected(component);
			return false;
		}
		public virtual void InitSelectionService() {
			if(!Manager.IsDesignMode) return;
			if(SelectionService != null) return;
			this.selectionService = GetServiceCore(typeof(ISelectionService)) as ISelectionService;
			if(this.selectionService == null) return;
			this.selectionService.SelectionChanged += new EventHandler(OnDesignTimeSelectionChanged);
			this.selectedObjects = null;
		}
		protected virtual void OnDesignTimeSelectionChanged(object sender, EventArgs e) {
			if(SelectionService == null) return;
			ICollection current = SelectionService.GetSelectedComponents();
			ICollection diff = GetDifference(PrevSelectedObjects, current);
			this.selectedObjects = current;
			if(diff == null || diff.Count == 0) return;
			foreach(object obj in diff) OnDesignTimeSelectionChanged(obj);
		}
		void OnDesignTimeSelectionChanged(object component) {
			Control ctrl = null;
			Bar bar = component as Bar;
			BarDockControl barDock = component as BarDockControl;
			BarItem item = component as BarItem;
			if(item != null) {
				if(Manager.SelectionInfo.CustomizeSelectedLink != null && Manager.SelectionInfo.CustomizeSelectedLink.Item == item) {
					if(!SelectionService.GetComponentSelected(component))
						Manager.SelectionInfo.CustomizeSelectLink(null);
				}
			}
			if(barDock != null) ctrl = barDock;
			if(bar != null) ctrl = bar.BarControl;
			if(ctrl != null) ctrl.Invalidate();
		}
		ICollection GetDifference(ICollection oldSelection, ICollection newSelection) {
			if(oldSelection == null || oldSelection.Count == 0) {
				if(newSelection == null || newSelection.Count == 0) return null;
				return newSelection;
			}
			if(newSelection == null || newSelection.Count == 0) return oldSelection;
			Hashtable table = new Hashtable();
			foreach(object obj in oldSelection) table[obj] = 1;
			foreach(object obj in newSelection) table[obj] = 1;
			return table.Keys;
		}
		public virtual bool CanAddDesignTimeLink(Bar bar) {
			InitSelectionService();
			if(SelectionService == null) return false;
			if(Manager.Designer == null || !Manager.Designer.AllowDesignTimeEnhancements) return false;
			if(bar != null) {
				if(bar.ItemLinks.Count == 0) return true;
				if(bar.IsFloating) return true;
				return true;
			}
			if(bar != null && bar.DockControl != null && bar.DockControl.IsVertical) return false;
			object selObject = SelectionService.PrimarySelection;
			if(selObject == Manager) return true;
			IBarObject barObject = selObject as IBarObject;
			if(barObject != null && barObject.Manager == Manager) return true;
			BarItem item = selObject as BarItem;
			if(item != null && item.Manager == Manager) return true;
			Bar b = selObject as Bar;
			if(b != null && b.Manager == Manager) return true;
			return false;
		}
		public DesignTimeManager DesignTimeManager { get { return designTimeManager; } }
		public virtual PopupMenuEditor MenuEditor { get { return menuEditor; } set { menuEditor = value; } }
		public virtual CustomizationForm CustomizationForm { get { return customizationForm; } }
		public virtual bool IsCustomizing { get { return isCustomizing; } }
		public virtual bool IsHotCustomizing { get { return isHotCustomizing; } }
		public virtual bool IsCanHotCustomizing { get { return Manager != null && Manager.AllowCustomization && Manager.AllowHotCustomization; } }
		public virtual bool IsCanCustomizing { get { return true; } }
		public virtual BarManager Manager { get { return manager; } }
		public virtual void StartCustomization() {
			if(Manager.Form == null) {
				MessageBox.Show("You must set 'Form' property before.", "Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}
			if(!IsCanCustomizing) return;
			if(IsCustomizing || IsHotCustomizing || CustomizationForm != null) {
				if(CustomizationForm != null) CustomizationForm.Activate();
				return;
			}
			isCustomizing = true;
			Manager.SelectionInfo.CloseAllPopups();
			OnStartCustomization();
			customizationForm = Manager.OnCreateCustomizationForm();
			CustomizationForm.ShowCustomization(true);
		}
		public virtual void StartHotCustomization() {
			if(IsCustomizing || IsHotCustomizing) return;
			isCustomizing = true;
			isHotCustomizing = true;
			Manager.SelectionInfo.CloseAllPopups();
			OnStartCustomization();
		}
		public virtual void StopCustomization() {
			if(!IsCustomizing) return;
			if(CustomizationForm != null) {
				customizationForm.ShowCustomization(false);
				if(!customizationForm.Disposing) customizationForm.Dispose();
			}
			Manager.Helper.DragManager.StopLinkSizing(false);
			this.customizationForm = null;
			this.isHotCustomizing = this.isCustomizing = false;
			OnEndCustomization();
			if(Manager.Form != null)
				Manager.Form.Focus();
		}
		public void CustomizeMenu(BarLinksHolder editObject) {
			if(menuEditor != null) {
				menuEditor.Close();
			}
			menuEditor = new DevExpress.XtraBars.Design.PopupMenuEditor(Manager, editObject);
			menuEditor.Show();
		}
		protected virtual void UpdateBarSelection(bool customizing) {
			Manager.SelectionInfo.Clear();
			foreach(Bar bar in Manager.Bars) {
				if(bar.BarControl != null) {
					bar.BarControl.AllowDrop = customizing;
				}
			}
		}
		protected virtual void OnStartCustomization() {
			ISelectionService serv = GetServiceCore(typeof(ISelectionService)) as ISelectionService;
			if(serv != null)
				serv.SetSelectedComponents(null);
			foreach(Bar bar in Manager.Bars) {
				bar.OnCustomizationStart();
			}
			UpdateBarSelection(true);
			Manager.RaiseStartCustomization(EventArgs.Empty);
		}
		protected virtual void OnEndCustomization() {
			if(Manager.IsDesignMode) return;
			UpdateBarSelection(false);
			if(designTimeManager != null)
				designTimeManager.Dispose();
			designTimeManager = null;
			foreach(Bar bar in Manager.Bars) {
				bar.OnCustomizationEnd();
			}
			Manager.RaiseEndCustomization(EventArgs.Empty);
		}
		protected virtual object GetServiceCore(Type type) {
			ISite site = Manager.Site;
			if(site == null) {
				DevExpress.XtraBars.Ribbon.RibbonBarManager manager = Manager as DevExpress.XtraBars.Ribbon.RibbonBarManager;
				if(manager != null) site = manager.Ribbon.Site;
			}
			if(site != null) return site.GetService(type);
			return null;
		}
		internal virtual DesignTimeManager CreateDesignTimeManagerCore() {
			return new DevExpress.XtraBars.Customization.Helpers.DesignTimeManager(Manager);
		}
		protected internal virtual void CreateDesignTimeManager() {
			if(this.designTimeManager != null) return;
			if(this.designTimeManager == null) {
				this.designTimeManager = CreateDesignTimeManagerCore();
			}
		}
		public virtual void ShowCreateItemDesigner(BarDesignTimeItemLink link) {
			if(!Manager.IsDesignMode || link == null) return;
			CreateDesignTimeManager();
			DesignTimeManager.ShowCreateItemMenu(link);
		}
		public virtual void ShowItemDesigner() {
			if(!IsCustomizing && !Manager.IsDesignMode) return;
			if(Manager.SelectionInfo.CustomizeSelectedLink == null) {
				HideItemDesigner();
				return;
			}
			CreateDesignTimeManager();
			DesignTimeManager.ShowLinkMenu(Manager.SelectionInfo.CustomizeSelectedLink);
		}
		public virtual void HideItemDesigner() {
			if(DesignTimeManager != null) {
				DesignTimeManager.CloseMenu();
				DesignTimeManager.Dispose();
			}
			designTimeManager = null;
		}
		public virtual bool CheckAndCustomizationMouseMove(CustomLinksControl control, MouseEventArgs e, Point downPoint, BarItemLink linkByPoint) {
			Point p = Control.MousePosition;
			if(Manager.SelectionInfo.CanResizeCustomizeSelectedLink(p, linkByPoint) || Manager.IsLinkSizing) {
				Cursor.Current = (Cursor)Manager.GetController().DragCursors[BarManager.EditSizingCursor];
				if(Manager.IsLinkSizing) {
					Manager.Helper.DragManager.DoLinkSizing(Manager.SelectionInfo.CustomizeSelectedLink, p, false);
					return true;
				}
				return false;
			} else {
				if(!Manager.IsDragging) Cursor.Current = Cursors.Default;
			}
			if((e.Button & MouseButtons.Left) != 0) {
				if(Manager.IsDragging) {
					return true;
				}
				p.Offset(-downPoint.X, -downPoint.Y);
				if(Math.Abs(p.X) > DragInterval || Math.Abs(p.Y) > DragInterval) {
					if(Manager.SelectionInfo.CustomizeSelectedLink.CanDrag && linkByPoint != null && linkByPoint.CanDrag) {
						Manager.Helper.DragManager.StartDragging(control, e, Manager.SelectionInfo.CustomizeSelectedLink, control);
						return true;
					}
				}
			}
			return false;
		}
		public int DragInterval {
			get {
				if(Manager.IsDesignMode) return 7;
				return 3;
			}
		}
		public virtual void CheckAndStartHotCustomization(CustomLinksControl control, MouseEventArgs e, BarItemLink linkByPoint) {
			if(linkByPoint == null || !IsCanHotCustomizing  || Manager.IsDesignMode || IsCustomizing || (e.Button & MouseButtons.Left) == 0) return;
			if(!linkByPoint.CanDrag) return;
			if(!linkByPoint.CanSelectInCustomization) return;
			StartHotCustomization();
			Manager.SelectionInfo.CustomizeSelectedLink = linkByPoint;
			if(Manager.SelectionInfo.CustomizeSelectedLink == null) return;
			Manager.Helper.DragManager.StartDragging(control, e, Manager.SelectionInfo.CustomizeSelectedLink, control);
			StopCustomization();
		}
		public virtual void UpdateBar(Bar bar) {
			if(CustomizationForm != null) CustomizationForm.UpdateBarVisibility(bar);
		}
		public virtual void UpdateItem(BarItem item) {
			if(CustomizationForm != null) CustomizationForm.OnItemChanged(item);
		}
	}
}
