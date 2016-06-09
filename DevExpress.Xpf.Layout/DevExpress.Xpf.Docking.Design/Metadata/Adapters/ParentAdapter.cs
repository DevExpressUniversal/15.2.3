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

#if SL
extern alias Platform;
#endif
using System;
using System.Windows;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
#if SL
using UIElement = Platform::System.Windows.UIElement;
#endif
namespace DevExpress.Xpf.Docking.Design {
	class DockLayoutManagerParentAdapter : ParentAdapter {
		public override bool CanParent(ModelItem parent, Type childType) {
			return base.CanParent(parent, childType) && childType.IsSubclassOf(typeof(UIElement));
		}
		public override void Parent(ModelItem newParent, ModelItem child) {
			DockLayoutManager manager = newParent.As<DockLayoutManager>();
			if(manager != null) {
				using(ModelEditingScope layouting = newParent.Root.BeginEdit()) {
					var service = DockLayoutManagerDesignServiceHelper.GetService(newParent.Context);
					BaseLayoutItem[] items = manager.GetItems();
					if(items.Length > 0) {
						BaseLayoutItem target = GetDropTarget(manager, MouseUtil.Position, newParent);
						ModelItem targetItem = ModelServiceHelper.FindModelItem(newParent.Context, target);
						if(targetItem == null) return;
						child.ResetLayout();
						if(target is LayoutControlItem) {
							service.SetContent(targetItem, child);
						}
						if(target is LayoutPanel) {
							service.SetPanelContent(targetItem, child);
						}
						if(target is LayoutGroup) {
							LayoutGroup group = (LayoutGroup)target;
							if(group.IsControlItemsHost) {
								service.AddLayoutControlItem(targetItem, child);
							}
							else {
								service.AddItem(targetItem, child);
							}
						}
					}
					else {
						manager.EnsureLayoutRoot();
						service.CreateNewPanelForChild(newParent, child);
					}
					layouting.Complete();
				}
			}
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) { }
		BaseLayoutItem GetDropTarget(DockLayoutManager manager, Point screenPosition, ModelItem model) {
#if !SL            
			var platformPoint = manager.PointFromScreen(screenPosition);
#else
			DesignerView designerView = DesignerView.FromContext(model.Context);
			var modelView = model.View;
			var designerPoint = designerView.PointFromScreen(screenPosition);
			var platformPoint = modelView.TransformFromVisual(designerView).Transform(designerPoint).ToPlatformPoint();
#endif
			LayoutElementHitInfo hitInfo = manager.CalcHitInfo(platformPoint);
			return hitInfo.IsEmpty ? null : ((IDockLayoutElement)hitInfo.Element).Item;
		}
	}
}
