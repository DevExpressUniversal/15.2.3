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

using System.Drawing;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	class EmptyViewDockingAdornerInfo : BaseElementInfo, IDockingAdornerInfo {
		public EmptyViewDockingAdornerInfo(WidgetView view)
			: base(view) {
		}
		protected WidgetView View {
			get { return Owner as WidgetView; }
		}
		public override System.Type GetUIElementKey() {
			return typeof(IDockingAdornerInfo);
		}
		AdornerElementInfo DockingAdornerInfo;
		public void UpdateDocking(Adorner adorner, Point point, BaseDocument dragItem) {
			DockingAdornerInfoArgs args = DockingAdornerInfoArgs.EnsureInfoArgs(ref DockingAdornerInfo, adorner, Owner, dragItem, View.Bounds);
			args.Adorner = Owner.Manager.GetDockingRect();
			args.Container = Owner.Manager.Bounds;
			args.Bounds = View.Bounds;
			args.MousePosition = point;
			args.DragItem = dragItem;
			if(args.Calc())
				adorner.Invalidate();
		}
		public bool CanDock(Point point) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				return args.IsOverDockHint(point, out hint);
			}
			return false;
		}
		public void Dock(Point point, BaseDocument document) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				if(args.IsOverDockHint(point, out hint)) {
					IWidgetViewController controller = View.Controller;
					Docking.FloatForm fForm = document.Form as Docking.FloatForm;
					switch(hint) {
						case DockHint.SideLeft:
						case DockHint.SideTop:
						case DockHint.SideRight:
						case DockHint.SideBottom:
							new DockHelper(Owner).DockSide(document, fForm, hint);
							break;
					}
				}
			}
		}
		public void ResetDocking(Adorner adorner) {
			adorner.Reset(DockingAdornerInfo);
			DockingAdornerInfo = null;
		}
	}
}
