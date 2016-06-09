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
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using System.Windows.Markup;
using DevExpress.Xpf.Grid.Native;
using System.Collections;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class GroupPanelColumnHeaderDropTargetFactory : ColumnHeaderDropTargetFactory, IDropTargetFactoryEx {
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new GroupPanelDropTarget(panel, true);
		}
		protected override bool IsCompatibleDataControl(DataControlBase sourceDataControl, DataControlBase targetDataControl) {
			return sourceDataControl.GetRootDataControl() == targetDataControl.GetRootDataControl();
		}
		#region IDropTargetFactoryEx Members
		IDropTarget IDropTargetFactoryEx.CreateDropTarget(UIElement dropTargetElement, UIElement sourceElement) {
			GroupPanelControl groupPanelControl = LayoutHelper.FindParentObject<GroupPanelControl>(dropTargetElement);
			if(groupPanelControl == null)
				return new GroupPanelDropTarget((Panel)dropTargetElement, true);
			BaseGridColumnHeader header = (BaseGridColumnHeader)sourceElement;
			if(!((TableView)header.GridView).ShowGroupPanel)
				return DevExpress.Xpf.Core.EmptyDropTarget.Instance;
			GroupPanelContainer groupPanelContainer = (GroupPanelContainer)LayoutHelper.FindElement(groupPanelControl, e => {
				GroupPanelContainer container = e as GroupPanelContainer;
				return container != null && container.View != null && container.View.EventTargetView == header.GridView.EventTargetView;
			});
			GroupPanelColumnItemsControl itemsControl = (GroupPanelColumnItemsControl)groupPanelContainer.GetTemplateChildInternal("groupPanelItemsControl");
			Panel targetPanel = (Panel)LayoutHelper.FindElement(itemsControl, e => e is Panel);
			return new GroupPanelDropTarget(targetPanel, targetPanel == dropTargetElement);
		}
		#endregion
	}
	#region obsolete
	[Obsolete("Instead use the GroupPanelColumnHeaderDropTargetFactory class.")]
	public class GroupPanelColumnHeaderDropTargetFactoryExtension : GroupPanelColumnHeaderDropTargetFactory { }
	#endregion
}
namespace DevExpress.Xpf.Grid.Native {
	public class GroupPanelDropTarget : ColumnHeaderDropTargetBase {
		readonly bool mouseOverTargetPanel;
		public GroupPanelDropTarget(Panel panel, bool mouseOverTargetPanel)
			: base(panel) {
			this.mouseOverTargetPanel = mouseOverTargetPanel;
		}
		protected override bool DenyDropIfGroupingIsNotAllowed(HeaderPresenterType sourceType) {
			return true;
		}
		protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType){
			return true;
		}
		protected override int DropIndexCorrection { get { return 0; } }
		protected override int GetDropIndexFromDragSource(UIElement element, Point pt) {
			return mouseOverTargetPanel ? base.GetDropIndexFromDragSource(element, pt) : ChildrenCount;
		}
	}
}
