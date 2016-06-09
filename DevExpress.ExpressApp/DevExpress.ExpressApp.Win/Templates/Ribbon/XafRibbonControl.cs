﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Ribbon {
	[ToolboxItem(false)]
	[Designer("DevExpress.ExpressApp.Win.Design.Ribbon.XafRibbonControlV2Designer, DevExpress.ExpressApp.Win.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerCategory("Component")]
	public class XafRibbonControlV2 : RibbonControl {
		private List<IActionControl> actionControls = new List<IActionControl>();
		private List<IActionControlContainer> actionContainers = new List<IActionControlContainer>();
		protected override RibbonBarManager CreateBarManager() {
			return new XafRibbonBarManager(this);
		}
		protected override void OnAddToToolbar(BarItemLink link) {
			base.OnAddToToolbar(link);
			OnToolbarCustomized(link, CollectionChangeAction.Add);
			XafRibbonControlV2 mergedRibbon = MergedRibbon as XafRibbonControlV2;
			if(mergedRibbon != null) {
				mergedRibbon.OnToolbarCustomized(link, CollectionChangeAction.Add);
			}
		}
		protected override void OnRemoveFromToolbar(BarItemLink link) {
			OnToolbarCustomized(link, CollectionChangeAction.Remove);
			XafRibbonControlV2 mergedRibbon = MergedRibbon as XafRibbonControlV2;
			if(mergedRibbon != null) {
				mergedRibbon.OnToolbarCustomized(link, CollectionChangeAction.Remove);
			}
			base.OnRemoveFromToolbar(link);
		}
		protected override void UpdateRegistrationInfo() {
			RegistrationInfo.Insert(0, new XafRibbonRegistrationInfo(this));
			base.UpdateRegistrationInfo();
		}
		protected virtual void OnToolbarCustomized(BarItemLink link, CollectionChangeAction action) {
			if(ToolbarCustomized != null && link.Item.Manager == Manager) {
				ToolbarCustomized(this, new ToolbarCustomizedEventArgs(link, action));
			}
		}
		public XafRibbonControlV2() {
			AllowInplaceLinks = true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void FireRibbonChanged(Component component) {
			if(!IsDesignMode) return;
			OnFireRibbonChanged(component);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IActionControl> ActionControls {
			get { return actionControls; }
			set { actionControls = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IActionControlContainer> ActionContainers {
			get { return actionContainers; }
			set { actionContainers = value; }
		}
		[DefaultValue(true)]
		public new bool AllowInplaceLinks {
			get { return base.AllowInplaceLinks; }
			set { base.AllowInplaceLinks = value; }
		}
		public event EventHandler<ToolbarCustomizedEventArgs> ToolbarCustomized;
	}
}
