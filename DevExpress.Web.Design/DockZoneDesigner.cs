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
using DevExpress.Web.Design;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.ComponentModel.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxDockZoneDesigner : ASPxWebControlDesigner {
		ASPxDockZone zone = null;
		public ASPxDockZone Zone { get { return this.zone; } }
		public DockZoneOrientation Orientation {
			get { return Zone.Orientation; }
			set {
				DockZoneOrientation oldValue = Orientation;
				Zone.Orientation = value;
				RaiseComponentChanged(TypeDescriptor.GetProperties(typeof(ASPxDockZone))["Orientation"], oldValue, value);
			}
		}
		public string ZoneUID {
			get { return Zone.ZoneUID; }
			set {
				string oldValue = ZoneUID;
				Zone.ZoneUID = value;
				RaiseComponentChanged(TypeDescriptor.GetProperties(typeof(ASPxDockZone))["ZoneUID"], oldValue, value);
			}
		}
		public override void Initialize(IComponent component) {
			this.zone = (ASPxDockZone)component;
			base.Initialize(component);
		}
		protected override string GetBaseProperty() {
			return "ZoneUID";
		}
		public override bool IsThemableControl() {
			return false;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new DockZoneDesignerActionList(this);
		}
	}
	public class DockZoneDesignerActionList : ASPxWebControlDesignerActionList {
		ASPxDockZoneDesigner designer = null;
		public DockZoneOrientation Orientation {
			get { return this.designer.Orientation; }
			set { this.designer.Orientation = value; }
		}
		public string ZoneUID {
			get { return this.designer.ZoneUID; }
			set { this.designer.ZoneUID = value; }
		}
		public DockZoneDesignerActionList(ASPxDockZoneDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Insert(1, new DesignerActionPropertyItem("ZoneUID",
			  StringResources.DockZoneActionList_ZoneUID,
			  StringResources.ActionList_MiscCategory,
			  StringResources.DockZoneActionList_EditZoneUIDDescription));
			collection.Insert(2, new DesignerActionPropertyItem("Orientation",
				StringResources.DockZoneActionList_Orientation,
				StringResources.ActionList_MiscCategory,
				StringResources.DockZoneActionList_EditOrientationDescription));
			return collection;
		}
	}
}
