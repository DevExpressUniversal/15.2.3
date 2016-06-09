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
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Services.Internal;
namespace DevExpress.Web.ASPxScheduler {
	[
	DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess),
	ToolboxBitmapAccess.BitmapPath + "ASPxViewNavigator.bmp")
	]
	public class ASPxViewNavigator : ASPxSchedulerRelatedControl {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.SchedulerViewNavigator.js";
		ViewNavigatorItemsControl itemsControl;
		List<ViewNavigatorButtonItemBase> items;
		ViewNavigatorButtonItem gotoDateButtonItem;
		ASPxDateNavigatorCalendar gotoDateCalendar;
		WebControl calendarPopupDiv;
		#endregion
		public ASPxViewNavigator() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewNavigatorImages"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ViewNavigatorImages Images { get { return ImagesInternal as ViewNavigatorImages; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewNavigatorStyles"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty)]
		public ViewNavigatorStyles Styles { get { return (ViewNavigatorStyles)StylesInternal; } }
		internal ViewNavigatorItemsControl ItemsControl { get { return itemsControl; } }
		protected internal override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewNavigatorGotoDateButtonItem")]
#endif
		public ViewNavigatorButtonItem GotoDateButtonItem { get { return gotoDateButtonItem; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewNavigatorGotoDateCalendar")]
#endif
		public ASPxCalendar GotoDateCalendar { 
			get {
				if (gotoDateCalendar == null) {
					gotoDateCalendar = new ASPxDateNavigatorCalendar();
				}
				return gotoDateCalendar;
			}
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxViewNavigatorCalendarPopupDiv")]
#endif
		public WebControl CalendarPopupDiv { get { return calendarPopupDiv; } }
		protected override StylesBase CreateStyles() {
			return new ViewNavigatorStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new ViewNavigatorImages(this);
		}		
		protected internal override void CreateControlContentHierarchy() {
			this.items = new List<ViewNavigatorButtonItemBase>();
			items.Add(new ViewNavigatorButtonItem(String.Empty, "function() {{ ASPx.SchedulerNavigateViewBackward('{0}'); }}", GetImageCore(ViewNavigatorImages.BackwardName, Images.Backward), SchedulerCallbackCommandId.NavigateBackward, ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.TooltipViewNavigator_Backward)));
			if(SchedulerControl.OptionsBehavior.ShowViewNavigatorGotoDateButton) {
				ViewNavigatorButtonGroupItem buttonGroup = new ViewNavigatorButtonGroupItem();
				buttonGroup.Id = "BG";
				buttonGroup.Items.Add(new ViewNavigatorGotoTodayButtonItem(String.Format("{0}", ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.CaptionViewNavigator_Today)), "function() {{ ASPx.SchedulerGotoToday('{0}'); }}", GetImageCore(Images.Today), SchedulerCallbackCommandId.GotoToday, String.Empty));
				string showGotoDateCalendarAction = GetShowGotoDateCalendarAction();
				this.gotoDateButtonItem = new ViewNavigatorGotoDateButtonItem(String.Empty, showGotoDateCalendarAction, GetImageCore(ViewNavigatorImages.DownName, Images.Down), SchedulerCallbackCommandId.GotoToday, ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.TooltipViewNavigator_GotoDate));
				GotoDateButtonItem.Id = "GTDBI";
				buttonGroup.Items.Add(GotoDateButtonItem);
				items.Add(buttonGroup);
			}
			else {
				items.Add(new ViewNavigatorGotoTodayButtonItem(String.Format("{0}", ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.CaptionViewNavigator_Today)), "function() {{ ASPx.SchedulerGotoToday('{0}'); }}", GetImageCore(Images.Today), SchedulerCallbackCommandId.GotoToday, String.Empty));
			}
			items.Add(new ViewNavigatorButtonItem(String.Empty, "function() {{ ASPx.SchedulerNavigateViewForward('{0}'); }}", GetImageCore(ViewNavigatorImages.ForwardName, Images.Forward), SchedulerCallbackCommandId.NavigateForward, ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.TooltipViewNavigator_Forward)));
			this.itemsControl = new ViewNavigatorItemsControl(this, items);
			MainCell.Controls.Add(itemsControl);
			this.itemsControl.ID = "IC";
			if(SchedulerControl.OptionsBehavior.ShowViewNavigatorGotoDateButton && !DesignMode) {
				CreateGotoDatePopupCalendar(MainCell);
				CreateGotoDateCalendar(CalendarPopupDiv);
			}
		}
		string GetShowGotoDateCalendarAction() {
			ISchedulerScriptService scriptService = SchedulerControl.GetService(typeof(ISchedulerScriptService)) as ISchedulerScriptService;
			if (scriptService == null)
				return "function() {{ ASPx.SchedulerShowGotoDateCalendar('{0}'); }}";
			return scriptService.GetShowGotoDateCalendarAction();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (this.gotoDateCalendar == null || SchedulerControl == null)
				return;
			this.gotoDateCalendar.TodayDate = SchedulerControl.InnerControl.TimeZoneHelper.ToClientTime(DateTime.Now).Date;
		}
		protected ButtonImageProperties GetImageCore(string prefix, ButtonImageProperties userProps) {
			ButtonImageProperties props = new ButtonImageProperties();
			props.CopyFrom(Images.GetImageProperties(Page, prefix));
			props.CopyFrom(userProps);
			return props;
		}
		protected ButtonImageProperties GetImageCore(ButtonImageProperties userProps) {
			ButtonImageProperties props = new ButtonImageProperties();
			props.CopyFrom(userProps);
			return props;
		}
		protected internal override void PrepareControlContentHierarchy() {
			this.itemsControl.ItemSpacing = Styles.GetButtonSpacing();
		}
		void CreateGotoDatePopupCalendar(Control parent) {
			this.calendarPopupDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(CalendarPopupDiv);
			CalendarPopupDiv.ID = "calendarPopupDiv";
			CalendarPopupDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
			CalendarPopupDiv.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			CalendarPopupDiv.Style.Add(HtmlTextWriterStyle.ZIndex, "10000");
		}
		void CreateGotoDateCalendar(WebControl popupDiv) {
			GotoDateCalendar.ParentSkinOwner = ParentSkinOwner;
			if (SchedulerControl != null) {
				GotoDateCalendar.FirstDayOfWeek = (System.Web.UI.WebControls.FirstDayOfWeek)SchedulerControl.FirstDayOfWeek;
				GotoDateCalendar.MinDate = SchedulerControl.LimitInterval.Start;
				GotoDateCalendar.MaxDate = SchedulerControl.LimitInterval.End;
			}
			popupDiv.Controls.Add(GotoDateCalendar);
			GotoDateCalendar.ClientSideEvents.ValueChanged = String.Format("function(s,e) {{ ASPx.SchedulerGotoDate(s, '{0}'); }}", ClientID);
			GotoDateCalendar.ID = "gotodateCalendar";
			GotoDateCalendar.ShowClearButton = false;
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxViewNavigator.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if(SchedulerControl != null)
				sb.AppendFormat("{0}.schedulerControlId='{1}';", localVarName, SchedulerControl.ClientID);
			ViewNavigatorBlock viewNavigatorBlock = SchedulerControl.ViewNavigatorBlock;
			if(viewNavigatorBlock == null)
				return;
			ASPxCalendar calendar = GotoDateCalendar;
			if(calendar != null)
				sb.AppendFormat("{0}.calendarId='{1}';", localVarName, calendar.ClientID);
			if(GotoDateButtonItem != null) {
				ASPxWebControl gotoDateButton = GotoDateButtonItem.ButtonControl;
				if(gotoDateButton != null)
					sb.AppendFormat("{0}.gotoDateButtonId='{1}';", localVarName, gotoDateButton.ClientID);
			}
			WebControl popupDiv = CalendarPopupDiv;
			if (popupDiv != null)
				sb.AppendFormat("{0}.calendarPopupDivId='{1}';", localVarName, popupDiv.ClientID);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSchedulerViewNavigator";
		}
		#endregion
		protected override string GetSkinControlName() {
			return "Scheduler";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Editors" };
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public abstract class ViewNavigatorButtonItemBase  {
		ASPxWebControl buttonControl;
		string id;
		public ASPxWebControl CreateButton(ASPxViewNavigator owner) {
			if(ButtonControl != null)
				return ButtonControl;
			ASPxWebControl button = CreateButtonCore(owner);
			if(!String.IsNullOrEmpty(Id))
				button.ID = Id;
			button.EnableViewState = false;
			button.ParentSkinOwner = owner;
			this.buttonControl = button;
			return button;
		}
		public ASPxWebControl ButtonControl { get { return buttonControl; } }
		public string Id { get { return id; } set { id = value; } }
		protected virtual ASPxWebControl CreateButtonCore(ASPxViewNavigator owner) {
			return new ViewNavigatorButton();
		}
		public abstract void PrepareControl(Control control, ASPxViewNavigator owner);
	}
	public class ViewNavigatorGotoTodayButtonItem : ViewNavigatorButtonItem {
		public ViewNavigatorGotoTodayButtonItem(string text, string onButtonClickFormat, ButtonImageProperties image, string callabckCommandId, string toolTip)
			: base(text, onButtonClickFormat, image, callabckCommandId, toolTip) {
		}
		public override void PrepareControl(Control control, ASPxViewNavigator owner) {
			base.PrepareControl(control, owner);
			RenderUtils.AppendDefaultDXClassName((ViewNavigatorButton)control, "dxscViewNavigatorGotoTodayButton");
		}
	}
	public class ViewNavigatorGotoDateButtonItem : ViewNavigatorButtonItem {
		public ViewNavigatorGotoDateButtonItem(string text, string onButtonClickFormat, ButtonImageProperties image, string callabckCommandId, string toolTip)
			: base(text, onButtonClickFormat, image, callabckCommandId, toolTip) {
		}
		protected override ASPxWebControl CreateButtonCore(ASPxViewNavigator owner) {
			return new ViewNavigatorGotoDateButton();
		}
	}
	public class ViewNavigatorButtonItem : ViewNavigatorButtonItemBase {
		string text;
		string onButtonClickFormat;
		ButtonImageProperties image;
		string callbackCommandId;
		string toolTip;
		public ViewNavigatorButtonItem(string text, string onButtonClickFormat, ButtonImageProperties image, string callbackCommandId, string toolTip) {
			this.text = text;
			this.onButtonClickFormat = onButtonClickFormat;
			this.image = image;
			this.callbackCommandId = callbackCommandId;
			this.toolTip = toolTip;
		}
		public string Text { get { return text; } }
		public string OnButtonClickFormat { get { return onButtonClickFormat; } }
		public ButtonImageProperties Image { get { return image; } }
		public string CallbackCommandId { get { return callbackCommandId; } }
		public string ToolTip { get { return toolTip; } }
		protected override ASPxWebControl CreateButtonCore(ASPxViewNavigator owner) {
			ASPxWebControl control = base.CreateButtonCore(owner);
			return control;
		}
		public override void PrepareControl(Control control, ASPxViewNavigator owner) {
			ViewNavigatorButton button = (ViewNavigatorButton)control;
			button.ControlStyle.CopyFrom(owner.Styles.GetButtonStyle());
			if(Image.IsEmpty)
				button.Text = Text;
			button.ToolTip = ToolTip;
			button.ClientSideEvents.Click = GetClickHandler(owner);
			button.Image.Assign(Image);
			button.Wrap = DefaultBoolean.False;
			ASPxScheduler scheduler = owner.SchedulerControl;
			if(scheduler != null)
				button.Enabled = scheduler.CallbackCommandManager.IsCommandEnabled(CallbackCommandId);
		}
		protected virtual string GetClickHandler(ASPxViewNavigator owner) {
			return String.Format(OnButtonClickFormat, owner.ClientID);
		}
	}
	public class ViewNavigatorButtonGroupItem : ViewNavigatorButtonItemBase {
		List<ViewNavigatorButtonItem> items;
		public ViewNavigatorButtonGroupItem() {
			this.items = new List<ViewNavigatorButtonItem>();
		}
		public List<ViewNavigatorButtonItem> Items { get { return items; } }
		protected override ASPxWebControl CreateButtonCore(ASPxViewNavigator owner) {
			ButtonGroupControl control = new ButtonGroupControl();
			control.ViewNavigator = owner;
			foreach(ViewNavigatorButtonItem navigatorButtonItem in Items)
				control.ButtonItems.Add(navigatorButtonItem);
			return control;
		}
		public override void PrepareControl(Control control, ASPxViewNavigator owner) {
		}
	}
	[ToolboxItem(false)]
	public class ButtonGroupControl : ASPxWebControl {
		List<ViewNavigatorButtonItem> buttonItems;
		Dictionary<ViewNavigatorButtonItem, ViewNavigatorButton> hash;
		ASPxViewNavigator viewNavigator;
		public ButtonGroupControl() {
			this.buttonItems = new List<ViewNavigatorButtonItem>();
			this.hash = new Dictionary<ViewNavigatorButtonItem, ViewNavigatorButton>();
		}
		public List<ViewNavigatorButtonItem> ButtonItems { get { return buttonItems; } }
		public ASPxViewNavigator ViewNavigator { get { return viewNavigator; } set { viewNavigator = value; } }
		protected override void CreateControlHierarchy() {
			Table table = new InternalTable();
			Controls.Add(table);
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Style.Add(HtmlTextWriterStyle.BorderCollapse, "collapse");
			TableRow row = new InternalTableRow();
			table.Rows.Add(row);
			bool isFirst = false;
			foreach(ViewNavigatorButtonItem buttonItem in ButtonItems) {
				TableCell cell = new InternalTableCell();
				cell.BorderWidth = 0;
				row.Cells.Add(cell);
				ViewNavigatorButton button = buttonItem.CreateButton(ViewNavigator) as ViewNavigatorButton;
				if (isFirst)
					button.BorderLeft.BorderWidth = 0;
				isFirst = true;
				hash.Add(buttonItem, button);
				cell.Controls.Add(button);
			}
		}
		protected override void PrepareControlHierarchy() {
			foreach(ViewNavigatorButtonItem buttonItem in ButtonItems) {
				ViewNavigatorButton button = hash[buttonItem];
				buttonItem.PrepareControl(button, ViewNavigator);
			}
		}
	}
	public class ViewNavigatorItemsControl : ItemsControl<ViewNavigatorButtonItemBase> {
		ASPxViewNavigator owner;
		public ViewNavigatorItemsControl(ASPxViewNavigator owner, List<ViewNavigatorButtonItemBase> items)
			: base(items, 0, RepeatDirection.Horizontal, RepeatLayout.Table) {
			this.owner = owner;
		}
		protected override Control CreateItemControl(int index, ViewNavigatorButtonItemBase item) {
			return item.CreateButton(this.owner);
		}
		protected override void PrepareItemControl(Control control, int index, ViewNavigatorButtonItemBase item) {
			base.PrepareItemControl(control, index, item);
			item.PrepareControl(control, this.owner);
		}
		protected internal virtual CallbackResult CalcCallbackResultCore() {
			EnsureChildControls();
			PrepareControlHierarchy();
			CalcCallbackResultHelper helper = new CalcCallbackResultHelper(owner, MainCell);
			return helper.CalcCallbackResult();
		}
	}
	public class ViewNavigatorBlock : ASPxSchedulerControlBlock {
		ASPxViewNavigator navigator;
		TemplateContainerBase templateContainer;
		public ViewNavigatorBlock(ASPxScheduler owner)
			: base(owner) {
		}
		public override string ContentControlID { get { return "viewNavigatorBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.templateContainer = TemplatesHelper.GetToolbarViewNavigatorTempalteContainer(Owner);
			if(this.templateContainer != null) {
				parent.Controls.Add(this.templateContainer);
			}
			else {
				this.navigator = CreateViewNavigator();
				SetupRelatedControl(navigator);
				parent.Controls.Add(navigator);
			}
		}
		protected virtual ASPxViewNavigator CreateViewNavigator(){
			return new ASPxViewNavigator();
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
			if(this.navigator == null)
				return;
			navigator.Images.Assign(Owner.Images.ViewNavigator);
			navigator.Styles.Assign(Owner.Styles.ViewNavigator);
		}
	}
}
