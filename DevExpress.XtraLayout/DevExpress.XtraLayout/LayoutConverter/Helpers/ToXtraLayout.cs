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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraTab;
namespace DevExpress.XtraLayout.Converter {
	public class ConversionInfoHelper {
		ConversionInfoHelperForm form;
		List<ConvertToXtraLayoutItemInfo> info;
		Control ownerControl;
		public ConversionInfoHelper(List<ConvertToXtraLayoutItemInfo> info, Control ownerControl) {
			this.ownerControl = ownerControl;
			this.info = info;
		}
		protected virtual ConversionInfoHelperForm CreteForm(List<ConvertToXtraLayoutItemInfo> info) {
			return new ConversionInfoHelperForm(info);
		}
		protected virtual void UpdateFormBounds() {
			form.Size = ownerControl.ClientSize;
			form.Location = ownerControl.PointToScreen(Point.Empty);
		}
		public void ShowInfo() {
			if (info.Count > 1) {
				if (form != null) form.Dispose();
				form = CreteForm(info);
				UpdateFormBounds();
				form.Show();
			}
		}
		public void HideInfo() {
			form.Hide();
			form.Dispose();
		}
	}
	public class ConversionInfoHelperForm : BaseDragHelperForm {
		List<ConvertToXtraLayoutItemInfo> info;
		public ConversionInfoHelperForm() : base(0,0) {
			this.Opacity = 0.666;
		}
		public ConversionInfoHelperForm(List<ConvertToXtraLayoutItemInfo> info)
			: this() {
			this.info = info;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			PaintInfo(e);
		}
		protected virtual void PaintInfo(PaintEventArgs e) {
			foreach (ConvertToXtraLayoutItemInfo tInfo in info) {
				Rectangle labelRect, controlRect, itemRect;
				labelRect = tInfo.LabelControl == null ? Rectangle.Empty : tInfo.LabelControl.Bounds;
				controlRect = tInfo.TargetControl == null ? Rectangle.Empty : tInfo.TargetControl.Bounds;
				if (labelRect != Rectangle.Empty && controlRect != Rectangle.Empty)
					itemRect = Rectangle.Union(labelRect, controlRect);
				else
					itemRect = labelRect == Rectangle.Empty ? controlRect : labelRect;
				itemRect.Inflate(2, 2);
				e.Graphics.FillRectangle(Brushes.Blue, itemRect);
				e.Graphics.FillRectangle(Brushes.Yellow, labelRect);
				e.Graphics.FillRectangle(Brushes.Green, controlRect);
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {}
	}
	public class ToXtraLayoutConverterHelper {
		LayoutConverter ownerCore;
		protected Hashtable controlsAndItems = new Hashtable();
		public ToXtraLayoutConverterHelper(LayoutConverter owner) {
			this.ownerCore = owner;
		}
		public static bool IsTabControl(Control control) {
			TabControl tabControl = control as TabControl;
			XtraTab.XtraTabControl xtraTabControl = control as XtraTab.XtraTabControl;
			return tabControl != null || xtraTabControl != null;
		}
		public static bool IsGroup(Control control) {
			GroupBox groupBox = control as GroupBox;
			GroupControl groupControl = control as GroupControl;
			SplitContainerControl sCC = control as SplitContainerControl;
			if(sCC != null) return false;
			return groupBox != null || groupControl != null;
		}
		public static bool IsGroupCaptionVisible(Control control) {
			GroupBox groupBox = control as GroupBox;
			GroupControl groupControl = control as GroupControl;
			if(groupBox != null) return true;
			if(groupControl != null) return groupControl.ShowCaption;
			return false;
		}
		protected virtual void ProcessTabbedGroup(BaseLayoutItem item, Control control) {
			TabControl tabControl = control as TabControl;
			XtraTab.XtraTabControl xtraTabControl = control as XtraTab.XtraTabControl;
			TabbedControlGroup tgroup = item as TabbedControlGroup;
			if(tgroup == null) return;
			if(tabControl != null) {
				foreach(TabPage page in tabControl.TabPages) {
					LayoutGroup tabPage = tgroup.AddTabPage();
					ConvertToXtraLayout(page, tabPage);
				}
			}
			if(xtraTabControl != null) {
				foreach(XtraTab.XtraTabPage page in xtraTabControl.TabPages) {
					LayoutGroup tabPage = tgroup.AddTabPage();
					tabPage.Text = page.Text;
					ConvertToXtraLayout(page, tabPage);
				}
			}
		}
		protected virtual void ProcessGroup(BaseLayoutItem item, Control group) {
			ConvertToXtraLayout(group, (LayoutGroup)item);
		}
		protected virtual BaseLayoutItem CreateLayoutElement(LayoutGroup parent, Control control) {
			if(IsTabControl(control))
				return parent.Owner.CreateTabbedGroup(null);
			if(IsGroup(control)) {
				LayoutGroup group = parent.Owner.CreateLayoutGroup(null);
				group.GroupBordersVisible = IsGroupCaptionVisible(control);
				group.Text = control.Text;
				return group;
			}
			return parent.Owner.CreateLayoutItem(null);
		}
		protected bool IsDesignerRootComponent(Control container) {
			if(container.Parent == null) return true;
			if(container is XtraTabPage) return true;
			if(container.Site == null) return false;
			IDesignerHost dh = container.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(dh != null) {
				return dh.RootComponent == container;
			}
			return false;
		}
		public virtual LayoutControl ConvertToXtraLayout(Control container) {
			controlsAndItems.Clear();
			LayoutControl newLayoutControl = CreateLayoutControl(container);
			try {
				newLayoutControl.BeginUpdate();
				newLayoutControl.Root.BeginUpdate();
				ConvertToXtraLayout(container, newLayoutControl.Root);
				newLayoutControl.Root.EndUpdate();
				newLayoutControl.EndUpdate();
				if(!IsDesignerRootComponent(container)) KillControl(container);
			}
			catch {
				return newLayoutControl;
			}
			return newLayoutControl;
		}
		ConversionInfoHelper helper = null;
		protected virtual ConversionInfoHelper CreateHelper(List<ConvertToXtraLayoutItemInfo> conversionInfo, Control container) {
			return new ConversionInfoHelper(conversionInfo, container);			
		}
		public virtual void ShowDetails(Control container) {
			List<ConvertToXtraLayoutItemInfo> conversionInfo = LayoutConversionHelper.CalculateConversionInfo(container, null);
			CheckHelperCreated(container, conversionInfo);
			helper.ShowInfo();
		}
		private void CheckHelperCreated(Control container, List<ConvertToXtraLayoutItemInfo> conversionInfo) {
			if (helper == null) helper = CreateHelper(conversionInfo, container);
		}
		public virtual void HideDetails() {
			if (helper == null) return;
			helper.HideInfo();
		}
		protected virtual LayoutControl CreateLayoutControl(Control container) {
			LayoutControl newLayoutControl = new LayoutControl();
			try {
				if(ownerCore.Site != null) ownerCore.Site.Container.Add(newLayoutControl, container.Name + "ConvertedLayout");
			}
			catch { }
			(newLayoutControl as ILayoutControl).SetControlDefaultsLast();
			if(IsDesignerRootComponent(container)) {
				newLayoutControl.Parent = container;
				newLayoutControl.Bounds = container.ClientRectangle;
				newLayoutControl.Dock = DockStyle.Fill;
				container.Update();
			}
			else {
				newLayoutControl.Parent = container.Parent;
				newLayoutControl.Bounds = container.Bounds;
				newLayoutControl.Root.GroupBordersVisible = newLayoutControl.Root.TextVisible = IsGroupCaptionVisible(container);
			}
			return newLayoutControl;
		}
		protected virtual void ConvertToXtraLayout(Control container, LayoutGroup parent) {
			BaseLayoutItem tempItem;
			List<ConvertToXtraLayoutItemInfo> conversionInfo = LayoutConversionHelper.CalculateConversionInfo(container, parent.Owner.Control);
			foreach(ConvertToXtraLayoutItemInfo tInfo in conversionInfo) {
				Control tControl = tInfo.TargetControl as Control;
				if(tControl == parent.Owner) continue;
				tempItem = CreateLayoutElement(parent, tControl);
				tempItem.Parent = parent;
				tempItem.Parent.Owner = parent.Owner;
				tempItem.Name = tControl.Name + "item";
				if(tInfo.LabelControl != null) {
					tempItem.Text = tInfo.LabelControl.Text;
					tempItem.TextLocation = tInfo.LabelLayout;
					tempItem.TextVisible = true;
					if(ownerCore.Site != null) tInfo.LabelControl.Site.Container.Remove(tInfo.LabelControl);
					tInfo.LabelControl.Parent = null;
					tInfo.LabelControl.Dispose();
				}
				controlsAndItems.Add(tControl, tempItem);
			}
			foreach (Control tempControl in container.Controls) if (!LayoutConversionHelper.AllowCreateLayoutItemForControl(tempControl)) parent.Owner.Control.Controls.Add(tempControl);
			foreach(ConvertToXtraLayoutItemInfo tInfo in conversionInfo) {
				Control tControl = tInfo.TargetControl as Control;
				if(tControl == parent.Owner) continue;
				tempItem = (BaseLayoutItem)controlsAndItems[tControl];
				if(tempItem is LayoutControlItem) ((LayoutControlItem)tempItem).Control = tControl;
				tControl.Parent = parent.Owner.Control;
				if(conversionInfo.Count == 1) {
					tempItem.Location = Point.Empty;
					tempItem.Size = parent.Owner.Control.Size;
				}
				else {
					tempItem.Location = tControl.Bounds.Location;
					tempItem.Size = new Size(tControl.Bounds.Width > 0 ? tControl.Bounds.Width : 1, tControl.Bounds.Height > 0 ? tControl.Bounds.Height : 1);
				}
				if(tempItem is LayoutControlItem) tempItem.TextVisible = tInfo.LabelControl != null;
				if(IsTabControl(tControl)) { ProcessTabbedGroup(tempItem, tControl); KillControl(tControl); }
				if(IsGroup(tControl)) { ProcessGroup(tempItem, tControl); KillControl(tControl); }
			}
		}
		protected virtual void KillControl(Control tControl) {
			try {
				if (ownerCore.Site != null) tControl.Site.Container.Remove(tControl);
				tControl.Parent = null;
				tControl.Dispose();
			} catch { }
		}
	}
}
