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

using System.Web.UI;
using DevExpress.Web;
using DevExpress.XtraScheduler.Drawing;
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region MenuControlBlockBase
	public abstract class MenuControlBlockBase : ASPxSchedulerControlBlock {
		ASPxSchedulerPopupMenu menu;
		protected MenuControlBlockBase(ASPxScheduler control)
			: base(control) {
		}
		protected internal ASPxSchedulerPopupMenu Menu { get { return menu; } }
		public abstract string MenuClickScriptFunction { get; }
		public abstract string MenuID { get; }
		protected internal abstract void PopulateMenu(ASPxSchedulerPopupMenu menu);
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.menu = new ASPxSchedulerPopupMenu(Owner);
			InitializeMenu();
			parent.Controls.Add(Menu);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
			PopulateMenu(Menu);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			PreparePopupMenuEventArgs args = new PreparePopupMenuEventArgs(Menu);
			Owner.RaisePopupMenuShowing(args);
			Owner.RaisePreparePopupMenu(args);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		protected internal virtual void InitializeMenu() {
			Menu.ID = MenuID;
			Menu.ClientSideEvents.ItemClick = MenuClickScriptFunction;
			Menu.ParentSkinOwner = Owner;
			Menu.ParentStyles = Owner.Styles.Menu;
			Menu.EnableScrolling = Owner.OptionsMenu.EnableMenuScrolling;
			Menu.PopupHorizontalOffset = 1;
			Menu.PopupVerticalOffset = 1;
		}
		protected internal override void PrepareControlHierarchyCore() {
			SchedulerCommonMenuImages images = ASPxScheduler.ActiveControl.Images.Menu.CommonImages;
			Menu.ItemImage.CopyFrom(images.Item);
			Menu.SubMenuItemImage.CopyFrom(images.SubMenuItem);
			Menu.VerticalPopOutImage.CopyFrom(images.VerticalPopOut);
			Menu.HorizontalPopOutImage.CopyFrom(images.HorizontalPopOut);
		}
		protected override bool IsCollapsedToZeroSize() {
			return true;
		}
	}
	#endregion
	public class AppointmentMenuBlock : MenuControlBlockBase {
		public AppointmentMenuBlock(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string ContentControlID { get { return "aptMenuBlock"; } }
		public override string MenuClickScriptFunction { get { return "ASPx.SchedulerOnAptMenuClick"; } }
		public override string MenuID { get { return "SMAPT"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderAppointmentMenu; } }
		#endregion
		protected internal override void PopulateMenu(ASPxSchedulerPopupMenu menu) {
			ASPxSchedulerDefaultPopupMenuBuilder builder = new ASPxSchedulerDefaultPopupMenuBuilder(new ASPxSchedulerMenuBuilderUIFactory(Owner), Owner, SchedulerHitTest.AppointmentContent);
			builder.PopulatePopupMenu(menu);
		}
	}
	public class ViewMenuBlock : MenuControlBlockBase {
		public ViewMenuBlock(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string ContentControlID { get { return "viewMenuBlock"; } }
		public override string MenuClickScriptFunction { get { return "ASPx.SchedulerOnViewMenuClick"; } }
		public override string MenuID { get { return "SMVIEW"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderViewMenu; } }
		#endregion
		protected internal override void PopulateMenu(ASPxSchedulerPopupMenu menu) {
			SchedulerHitTest hitTest = Owner.ActiveView is DayView ? SchedulerHitTest.Ruler : SchedulerHitTest.None;
			ASPxSchedulerDefaultPopupMenuBuilder builder = new ASPxSchedulerDefaultPopupMenuBuilder(new ASPxSchedulerMenuBuilderUIFactory(Owner), Owner, hitTest);
			builder.PopulatePopupMenu(menu);
		}
	}
}
