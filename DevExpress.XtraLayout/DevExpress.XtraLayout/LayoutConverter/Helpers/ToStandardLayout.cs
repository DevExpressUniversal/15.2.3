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
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraEditors;
using DevExpress.Utils;
namespace DevExpress.XtraLayout.Converter {
	public class ToStandardLayoutConverterHelper : BaseVisitor {
		LayoutConverter ownerCore;
		protected Control currentParent = null;
		public ToStandardLayoutConverterHelper(LayoutConverter owner) {
			ownerCore = owner;
		}
		public override void Visit(BaseLayoutItem item) {
			base.Visit(item);
			LayoutClassificationArgs classification = LayoutClassifier.Default.Classify(item);
			if(classification.IsLayoutControlItem && classification.Splitter == null && classification.EmptySpaceItem == null) {
				ProcessLayoutControlItem(classification.LayoutControlItem);
			}
		}
		protected Rectangle CalculateGroupOffset(Rectangle rect) {
			GroupControl groupControl = currentParent as GroupControl;
			if(groupControl != null && groupControl.ShowCaption) {
				switch(groupControl.CaptionLocation) {
					case Locations.Top:
						rect.Y += 20;
						break;
					case Locations.Left:
						rect.X += 20;
						break;
				}
			}
			return rect;
		}
		protected virtual void ProcessLayoutControlItem(LayoutControlItem citem) {
			citem.BeginInit();
			if(citem.TextVisible && citem.Text.Length != 0) {
				LabelControl label = new LabelControl();
				Rectangle rect = citem.ViewInfo.TextArea;
				rect.Offset(citem.Location);
				rect = CalculateGroupOffset(rect);
				label.Bounds = rect;
				label.Text = citem.Text;
				String labelName = citem.Name + "label";
				label.Parent = currentParent;
				AddElement(label, labelName);
			}
			if(citem.Control != null) {
				Control control = citem.Control;
				control.Parent = currentParent;
				citem.Control = null;
				Rectangle rect = citem.ViewInfo.ClientArea;
				rect.Offset(citem.Location);
				rect = CalculateGroupOffset(rect);
				control.Bounds = rect; 
				control.Visible = true;
			}
			citem.EndInit();
		}
		protected virtual void KillElement(Component component) {
			try {
				if (component.Site != null) ownerCore.Site.Container.Remove(component);
			} catch { }
			finally { }
		}
		protected virtual void AddElement(Component component, String name) {
			try {
				if(ownerCore.Site != null) ownerCore.Site.Container.Add(component);
			}
			catch { }
		}
		public override bool StartVisit(BaseLayoutItem item) {
			LayoutClassificationArgs classification = LayoutClassifier.Default.Classify(item);
			if(classification.IsGroup && !classification.IsTabPage)
				CreateGroupObject(classification.Group, classification.Group.TextVisible & classification.Group.GroupBordersVisible);
			if(classification.IsGroup && classification.IsTabPage)
				CreateTabPage(classification.Group);
			if(classification.IsTabbedGroup)
				CreateTabControl(classification.TabbedGroup);
			return true;
		}
		public override void EndVisit(BaseLayoutItem item) {
			LayoutClassificationArgs classification = LayoutClassifier.Default.Classify(item);
			if(classification.IsContainer)
				currentParent = currentParent.Parent;
		}
		protected virtual void CreateGroupObject(LayoutGroup item, bool showCaption) {
			GroupControl groupControl = new GroupControl();
			groupControl.BeginInit();
			Rectangle rect = item.Bounds;
			if(item.Parent == null) rect.Offset(item.Owner.Control.Location);
			else rect = CalculateGroupOffset(rect);
			groupControl.Bounds = rect;
			groupControl.Text = item.Text;
			AddElement(groupControl, item.Name + "GroupObject");
			groupControl.CaptionLocation = item.TextLocation;
			groupControl.ShowCaption = showCaption;
			groupControl.Parent = currentParent;
			currentParent = groupControl;
			groupControl.EndInit();
		}
		protected virtual void CreateTabPage(LayoutGroup item) {
			if(currentParent is XtraTab.XtraTabControl) {
				XtraTab.XtraTabControl tabParent = currentParent as XtraTab.XtraTabControl;
				XtraTab.XtraTabPage page = new DevExpress.XtraTab.XtraTabPage();
				page.Text = item.Text;
				page.Bounds = item.ViewInfo.ClientAreaRelativeToControl;
				AddElement(page, item.Name + "TabPage");
				tabParent.TabPages.Add(page);
				currentParent = page;
			}
		}
		protected virtual void CreateTabControl(TabbedGroup item) {
			XtraTab.XtraTabControl tabControl = new DevExpress.XtraTab.XtraTabControl();
			tabControl.Bounds = item.ViewInfo.BoundsRelativeToControl;
			AddElement(tabControl, item.Name + "TabControl");
			tabControl.Parent = currentParent;
			currentParent = tabControl;
		}
		public void ConvertToStandardLayout(LayoutControl layoutControl) {
			if(layoutControl == null) throw new Exception("Invalid layoutControl!");
			if(layoutControl.HiddenItems.Count > 0) { XtraMessageBox.Show("Move hidden items to layout"); return; }
			currentParent = layoutControl.Parent;
			layoutControl.Root.BeginInit();
			((ILayoutDesignerMethods)layoutControl).AllowHandleControlRemovedEvent = false;
			layoutControl.Root.Accept(this);
			((ILayoutDesignerMethods)layoutControl).AllowHandleControlRemovedEvent = true;
			layoutControl.Root.EndInit();
			layoutControl.Parent = null;
			KillXtraLayout(layoutControl);
		}
		protected virtual void KillXtraLayout(LayoutControl layoutControl) {
			ArrayList list = new ArrayList(layoutControl.Items);
			foreach(BaseLayoutItem item in list) {
				KillElement(item);
				item.Dispose();
			}
			KillElement(layoutControl);
			layoutControl.Dispose();
		}
	}
}
