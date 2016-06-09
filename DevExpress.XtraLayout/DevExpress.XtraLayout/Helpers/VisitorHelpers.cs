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
using System.IO;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DXUtils = DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Printing;
using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Accessibility;
using System.ComponentModel.Design;
using DevExpress.XtraLayout.Localization;
namespace DevExpress.XtraLayout.Helpers {
	public enum ComponentsUpdateHelperRoles { Add, Remove }
	public class ComponentsUpdateHelper : BaseVisitor {
		protected ComponentsUpdateHelperRoles helperRole;
		protected ILayoutControl controlOwner;
		public ComponentsUpdateHelper(ComponentsUpdateHelperRoles role, ILayoutControl owner)
			: base() {
			helperRole = role;
			controlOwner = owner;
		}
		protected virtual void AddComponentToOwnerCompnents(IComponent component) {
			if(!controlOwner.Components.Contains(component)) controlOwner.Components.Add(component);
		}
		protected virtual void RemoveComponentFromOwnerCompnents(IComponent component) {
			if(controlOwner.Components.Contains(component)) controlOwner.Components.Remove(component);
		}
		protected virtual bool CheckInheritanceRestrictions(IComponent component) {
			IInheritanceService service = (IInheritanceService)controlOwner.Site.GetService(typeof(IInheritanceService));
			InheritanceAttribute ia = InheritanceAttribute.Default;
			if (service != null) {
				ia = service.GetInheritanceAttribute(component);
			}
			if (ia == InheritanceAttribute.InheritedReadOnly) return false;
			else return true;
		}
		protected virtual void AddComponentToDesignSurface(IComponent component) {
			if (controlOwner.Site != null) {
				ArrayList components = new ArrayList(controlOwner.Site.Container.Components);
				if (!components.Contains(component) && ((ILayoutControl)controlOwner).AllowManageDesignSurfaceComponents && CheckInheritanceRestrictions(component)) {
					controlOwner.Site.Container.Add(component);
					BaseLayoutItem item = component as BaseLayoutItem;
					if (item != null) {
						string oldName = item.Name;
						if (oldName != String.Empty) {
							try {
								if (item.Name != oldName) item.Name = oldName;
								component.Site.Name = item.Name;
							} catch {
								item.Name = component.Site.Name;
							}
						}
					}
				}
			}
		}
		protected virtual void RemoveComponentFromDesignSurface(IComponent component) {
			if(controlOwner.Site != null) {
				ArrayList components = new ArrayList(controlOwner.Site.Container.Components);
				if(components.Contains(component) && ((ILayoutControl)controlOwner).AllowManageDesignSurfaceComponents) {
					controlOwner.Site.Container.Remove(component);
				}
			}
		}
		public override void Visit(BaseLayoutItem item) {
			if(helperRole == ComponentsUpdateHelperRoles.Add) {
				AddComponentToDesignSurface(item);
				AddComponentToOwnerCompnents(item);
			}
			if(helperRole == ComponentsUpdateHelperRoles.Remove) {
				RemoveComponentFromDesignSurface(item);
				RemoveComponentFromOwnerCompnents(item);
			}
		}
	}
	public class ItemToDefaultState : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			item.SetPropertiesDefault();
		}
	}
	class FillItemsHelper : BaseVisitor {
		ILayoutControl owner = null;
		ReadOnlyItemCollection items;
		public ReadOnlyItemCollection Items {
			get { return items; }
		}
		public FillItemsHelper(ILayoutControl owner) {
			this.owner = owner;
			this.items = new ReadOnlyItemCollection();
		}
		public override void Visit(BaseLayoutItem item) {
			items.Add(item);
			if(item != null) {
				UpdateName(item, owner);
			}
		}
		public static void UpdateName(BaseLayoutItem item, ILayoutControl owner) {
			LayoutGroup group = item as LayoutGroup;
			if(group != null) {
				if(group.Parent != null || group.IsHidden) {
					if(group.ParentTabbedGroup == null) {
						if(group.IsHidden && group.Parent == null) {
							group.SetCustomizationParentName();
							group.TabbedGroupParentName = "";
						}
						else {
							group.ParentName = group.Parent.Name;
							group.TabbedGroupParentName = "";
						}
					}
					else {
						group.TabbedGroupParentName = group.ParentTabbedGroup.Name;
						group.ParentName = "";
					}
				}
				else {
					group.ParentName = "";
					group.TabbedGroupParentName = "";
					if(group.Name == "")
						group.Name = (owner as ISupportImplementor).Implementor.CreateRootName();
				}
			}
			else {
				if(item.Parent != null) {
					item.ParentName = item.Parent.Name;
				}
			}
		}
	}
	class HiddenItemsHelper : BaseVisitor {
		BaseItemCollection items;
		public HiddenItemsHelper() {
			items = new BaseItemCollection();
		}
		public BaseItemCollection Items {
			get { return items; }
		}
		public override void Visit(BaseLayoutItem item) {
			items.Add(item);
		}
	}
	class HiddenItemsVisibilityHelper : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			LayoutControlItem citem = item as LayoutControlItem;
			if(citem != null) {
				if(citem.Control != null)
					citem.Control.Visible = false;
			}
		}
	}
	class ItemByControlSearcher : BaseVisitor {
		Control controlCore;
		LayoutControlItem controlItem = null;
		public ItemByControlSearcher(Control control) {
			controlCore = control;
		}
		public LayoutControlItem ResultItem {
			get {
				return controlItem;
			}
		}
		public override void Visit(BaseLayoutItem item) {
			LayoutControlItem citem = item as LayoutControlItem;
			if(citem != null)
				if(citem.Control != null)
					if(citem.Control == controlCore)
						controlItem = citem;
		}
	}
	class ParentsAndOwnersSetter : BaseVisitor {
		ILayoutControl Owner;
		public ParentsAndOwnersSetter(ILayoutControl owner) {
			Owner = owner;
		}
		public override void Visit(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			TabbedGroup tGroup = item as TabbedGroup;
			if(tGroup != null) {
				tGroup.Owner = Owner;
				if(tGroup.Parent == null) throw new LayoutControlInternalException("tabbed group has no parent");
				foreach(LayoutGroup tab in tGroup.TabPages) {
					tab.SetTabbedGroupParent(tGroup);
					tab.Parent = tGroup.Parent;
					tab.Owner = Owner;
				}
			}
			if(group != null) {
				foreach(BaseLayoutItem nitem in new ArrayList(group.Items)) {
					nitem.Parent = group;
					nitem.Owner = Owner;
				}
				group.Owner = Owner;
			}
		}
	}
	class SplitterItemLayoutTypeUpdater : BaseVisitor {
		ILayoutControl Owner;
		public SplitterItemLayoutTypeUpdater(ILayoutControl owner) {
			Owner = owner;
		}
		public override void Visit(BaseLayoutItem item) {
			SplitterItem sItem = item as SplitterItem;
			if(sItem != null) {
				sItem.UpdateLayoutType();
			}
		}
	}
	public class ViewInfoResetHelper : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			item.SetViewInfoAndPainter(null, null);
		}
	}
}
