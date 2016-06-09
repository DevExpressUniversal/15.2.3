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

using DevExpress.Utils.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design {
	public class TabFormControlDesigner : BaseParentControlDesigner, IKeyCommandProcessInfo {
		protected override bool AllowHookDebugMode { get { return true; } }
		public TabFormControlDesigner() {
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected override bool AllowEditInherited { get { return false; } }
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			LoaderPatcherService.InstallService(host);
		}
		protected override void Dispose(bool disposing) {
			LoaderPatcherService.UnInstallService(host);
			this.host = null;
			base.Dispose(disposing);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected override bool DrawGrid { get { return false; } }
		protected override bool EnableDragRect { get { return false; } }
		protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize) {
			Type ownerType = GetOwnerType(tool);
			if(ownerType != null && typeof(Component).IsAssignableFrom(ownerType) && !typeof(Control).IsAssignableFrom(ownerType)) {
				return base.CreateToolCore(tool, x, y, width, height, hasLocation, hasSize);
			}
			return null;
		}
		protected Type GetOwnerType(ToolboxItem tool) {
			Type res = null;
			try {
				res = tool.GetType(DesignerHost);
			}
			catch { }
			return res;
		}
		protected virtual bool GetHitTestCore(Point client) {
			TabFormControlHitInfo hitInfo = TabFormControl.ViewInfo.CalcHitInfo(client);
			if(hitInfo.HitTest == TabFormControlHitTest.Link || hitInfo.HitTest == TabFormControlHitTest.AddPage || hitInfo.HitTest == TabFormControlHitTest.Page)
				return true;
			return false;
		}
		protected override bool GetHitTest(Point point) {
			bool res = base.GetHitTest(point);
			if(!AllowDesigner || DebuggingState) return res;
			if(Control == null || res) return res;
			Point client = Control.PointToClient(point);
			return GetHitTestCore(client);
		}
		protected TabFormControlBase TabFormControl { get { return Control as TabFormControlBase; } }
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		DesignerActionListCollection smartTagCore = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(smartTagCore == null) {
					smartTagCore = new DesignerActionListCollection();
					smartTagCore.Add(new TabFormControlDesignerActionList(Component));
					DXSmartTagsHelper.CreateDefaultLinks(this, smartTagCore);
				}
				return smartTagCore;
			}
		}
		public override ICollection AssociatedComponents {
			get {
				if(Control == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				AddBase(controls);
				return controls;
			}
		}
		void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		protected override bool CanUseComponentSmartTags { get { return true; } } 
		public IServiceProvider ServiceProvider {
			get { return Component.Site; }
		}
		public BaseDesignTimeManager DesignTimeManager {
			get { return null; }
		}
	}
	public class TabFormControlDesignerActionList : DesignerActionList {
		public TabFormControlDesignerActionList(IComponent component)
			: base(component) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			return res;
		}
	}
}
