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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	[
	DXWebToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess),
	ToolboxBitmapAccess.BitmapPath + "ASPxViewSelector.bmp")
	]
	public class ASPxViewSelector : ASPxSchedulerRelatedControl {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.SchedulerViewSelector.js";
		ViewSelectorItemsControl itemsControl;
		#endregion
		public ASPxViewSelector() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewSelectorButtonsRepeatDirection"),
#endif
Category("Behavior"), DefaultValue(RepeatDirection.Horizontal)]
		public RepeatDirection ButtonsRepeatDirection
		{
			get { return (RepeatDirection)GetEnumProperty("ButtonsRepeatDirection", RepeatDirection.Horizontal); }
			set
			{
				SetEnumProperty("ButtonsRepeatDirection", RepeatDirection.Horizontal, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewSelectorStyles"),
#endif
Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty)]
		public ViewSelectorStyles Styles { get { return (ViewSelectorStyles)StylesInternal; } }
		internal ViewSelectorItemsControl ItemsControl { get { return itemsControl; } }
		protected internal override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyActiveViewChanged; } }
		protected override StylesBase CreateStyles() {
			return new ViewSelectorStyles(this);
		}
		protected internal override void CreateControlContentHierarchy() {
			List<SchedulerViewBase> views = GetViews();
			int count = views.Count;
			if (count <= 0)
				return;
			this.itemsControl = new ViewSelectorItemsControl(this, views);
			MainCell.Controls.Add(itemsControl);
		}
		protected internal override void PrepareControlContentHierarchy() {
			if(this.itemsControl != null) {
				this.itemsControl.ItemSpacing = Styles.GetButtonSpacing();
				this.itemsControl.RepeatDirection = ButtonsRepeatDirection;
			}
			if(MainTable != null)
				RenderUtils.AssignAttributes(this, MainTable);
			if(MainCell != null) {
				Styles.GetControlStyle().AssignToControl(MainCell, true);
				MainCell.CssClass = RenderUtils.CombineCssClasses(MainCell.CssClass, ButtonsRepeatDirection == RepeatDirection.Horizontal ? "dxscVSHorz" : "dxscVSVert");
			}
		}
		protected internal virtual string GetButtonId(int index) {
			return "c" + index.ToString();
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxViewSelector.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if(SchedulerControl != null)
				sb.AppendFormat("{0}.schedulerControlId='{1}';", localVarName, SchedulerControl.ClientID);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSchedulerViewSelector";
		}
		#endregion
		protected internal List<SchedulerViewBase> GetViews() {
			List<SchedulerViewBase> result = new List<SchedulerViewBase>();
			ASPxScheduler schedulerControl = SchedulerControl;
			if (schedulerControl == null)
				return result;
			SchedulerViewRepository views = schedulerControl.Views;
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = views[i];
				if (view.Enabled)
					result.Add(view);
			}
			return result;
		}
		protected override string GetSkinControlName() {
			return "Scheduler";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Editors" };
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class ViewSelectorItemsControl : ItemsControl<SchedulerViewBase> {
		ASPxViewSelector owner;
		public ViewSelectorItemsControl(ASPxViewSelector owner, List<SchedulerViewBase> items)
			: base(items, 0, RepeatDirection.Horizontal, RepeatLayout.Table) {
			this.owner = owner;
		}		
		protected override Control CreateItemControl(int index, SchedulerViewBase item) {
			ViewSelectorButton button = new ViewSelectorButton();
			button.EnableViewState = false;
			button.ParentSkinOwner = owner;
			return button;
		}
		protected override void PrepareItemControl(Control control, int index, SchedulerViewBase item) {
			base.PrepareItemControl(control, index, item);
			ViewSelectorButton button = (ViewSelectorButton)control;
			button.ControlStyle.CopyFrom(this.owner.Styles.GetButtonStyle());
			button.Text = item.InnerView.ShortDisplayName;
			button.Checked = (item.Control.ActiveViewType == item.Type);
			button.Wrap = DefaultBoolean.False;
			button.ClientSideEvents.Click = String.Format("function() {{ ASPx.SchedulerSelectView('{0}', ASPxSchedulerViewType.{1}); }}", owner.ClientID, item.Type);
		}
		protected internal virtual CallbackResult CalcCallbackResultCore() {
			EnsureChildControls();
			PrepareControlHierarchy();
			CalcCallbackResultHelper helper = new CalcCallbackResultHelper(owner, MainCell);
			return helper.CalcCallbackResult();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (this.owner == null || this.owner.Styles == null)
				return;
			AppearanceStyleBase style = this.owner.Styles.CreateDefaultViewSelectorIndentStyle();
			IndentCells.ForEach(cell => style.AssignToControl(cell.Cell));
		}
	}
	public class ViewSelectorBlock : ASPxSchedulerControlBlock {
		ASPxViewSelector selector;
		TemplateContainerBase templateContainer;
		public ViewSelectorBlock(ASPxScheduler owner)
			: base(owner) {
		}
		public override string ContentControlID { get { return "viewSelectorBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyActiveViewChanged; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.templateContainer  = TemplatesHelper.GetToolbarViewSelectorTemplateContainer(Owner);
			if(templateContainer != null) 
				parent.Controls.Add(templateContainer);
			else {
				this.selector = CreateViewSelector();
				SetupRelatedControl(this.selector);
				parent.Controls.Add(this.selector);
			}
		}
		protected virtual ASPxViewSelector CreateViewSelector() {
			return new ASPxViewSelector();
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
			if(this.selector != null) {
				this.selector.Styles.Assign(Owner.Styles.ViewSelector);
			}
		}
	}
}
