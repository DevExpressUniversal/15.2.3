#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Native;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
namespace DevExpress.DashboardWin.Design {
	public class DashboardViewerActionList : DashboardActionList {
		readonly DashboardViewer viewer;
		IComponentChangeService componentChangeService;
		IComponentChangeService ComponentChangeService {
			get {
				if (componentChangeService == null) {
					ISite site = viewer.Site;
					if (site != null)
						componentChangeService = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				}
				return componentChangeService;
			}
		}
		[
		TypeConverter(TypeNames.DashboardSourceConvertor),
		Editor(TypeNames.DashboardSourceEditor, typeof(UITypeEditor))
		]
		public object DashboardSource {
			get { return viewer.DashboardSource; }
			set {
				OnComponentChanging();
				viewer.DashboardSource = object.Equals(value, DashboardSourceConvertor.None) ? null : value;
				OnComponentChanged();
			}
		}
		public DashboardViewerActionList(DashboardViewer viewer)
			: base(viewer) {
			this.viewer = viewer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			collection.Add(new DesignerActionPropertyItem("DashboardSource", "Dashboard Source", "Dashboard"));
			foreach (DesignerActionItem item in base.GetSortedActionItems())
				collection.Add(item);
			return collection;
		}
		void OnComponentChanging() {
			IComponentChangeService componentChangeService = ComponentChangeService;
			if (componentChangeService != null)
				componentChangeService.OnComponentChanging(viewer, null);
		}
		void OnComponentChanged() {
			IComponentChangeService componentChangeService = ComponentChangeService;
			if (componentChangeService != null)
				componentChangeService.OnComponentChanged(viewer, null, null, null);
		}
	}
}
