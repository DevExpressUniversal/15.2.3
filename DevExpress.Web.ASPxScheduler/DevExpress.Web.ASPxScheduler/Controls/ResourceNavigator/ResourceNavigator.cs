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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler {
	[
	DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess),
	ToolboxBitmapAccess.BitmapPath + "ASPxResourceNavigator.bmp")
	]
	public class ASPxResourceNavigator : ASPxSchedulerRelatedControl  {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.ResourceNavigator.js";
		private const string
			FirstButtonId = "F",
			PrevPageButtonId = "PP",
			PrevButtonId = "P",
			NextButtonId = "N",
			NextPageButtonId = "NP",
			LastButtonId = "L",
			DecreaseButtonId = "D",
			IncreaseButtonId = "I";
		Dictionary<string, TableCell> buttonCells = new Dictionary<string, TableCell>();
		Dictionary<string, ResourceNavigatorButton> buttons = new Dictionary<string, ResourceNavigatorButton>();
		WebControl containerDiv;
		Table table;
		TableCell comboCell;
		ASPxComboBox combo;
		ResourceNavigatorProperties properties;
		ResourceNavigatorButton lastButton;
		#endregion
		public ASPxResourceNavigator() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			this.properties = new ResourceNavigatorProperties(this);
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceNavigatorImages"),
#endif
PersistenceMode(PersistenceMode.InnerProperty)]
		public ResourceNavigatorImages Images { get { return ImagesInternal as ResourceNavigatorImages; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceNavigatorStyles"),
#endif
PersistenceMode(PersistenceMode.InnerProperty)]
		public ResourceNavigatorStyles Styles { get { return StylesInternal as ResourceNavigatorStyles; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceNavigatorProperties"),
#endif
PersistenceMode(PersistenceMode.InnerProperty)]
		public ResourceNavigatorProperties Properties { get { return properties; } }
		protected internal override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyResourceIntervalChanged; } }
		protected Dictionary<string, TableCell> ButtonCells {
			get { return buttonCells; }
		}
		protected Dictionary<string, ResourceNavigatorButton> Buttons {
			get { return buttons; }
		}
		protected WebControl ContainerDiv {
			get { return containerDiv; }
		}
		protected Table Table {
			get { return table; }
		}
		protected TableCell ComboCell {
			get { return comboCell; }
		}
		protected ASPxComboBox Combo {
			get { return combo; }
		}
		#endregion
		#region Render
		protected override void ClearControlFields() {
			ButtonCells.Clear();
			Buttons.Clear();
			this.containerDiv = null;
			this.table = null;
			this.combo = null;
			this.comboCell = null;
		}
		protected internal override void CreateControlContentHierarchy() {
			this.containerDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.table = RenderUtils.CreateTable();
			TableRow row = RenderUtils.CreateTableRow();
			Table.Rows.Add(row);
			ContainerDiv.Controls.Add(Table);
			MainCell.Controls.Add(ContainerDiv);
			if (Properties.EnableFirstLast)
				CreateButtonCell(row, FirstButtonId);
			if (Properties.EnablePrevNextPage)
				CreateButtonCell(row, PrevPageButtonId);
			if (Properties.EnablePrevNext) {
				CreateButtonCell(row, PrevButtonId);
				CreateButtonCell(row, NextButtonId);
			}
			if (Properties.EnablePrevNextPage)
				CreateButtonCell(row, NextPageButtonId);
			if (Properties.EnableFirstLast)
				CreateButtonCell(row, LastButtonId);
			if (Properties.EnableIncreaseDecrease) {
				CreateButtonCell(row, IncreaseButtonId);
				CreateButtonCell(row, DecreaseButtonId);
			}
			this.comboCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ComboCell);
			if(Properties.EnableResourceComboBox) {
			this.combo = new ASPxComboBox();
			this.combo.ParentSkinOwner = this;
			ComboCell.Controls.Add(Combo);
			}
			else
				ComboCell.Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected internal override void PrepareOuterControlHierarchy() {
			base.PrepareOuterControlHierarchy();
			RenderUtils.AssignAttributes(this, MainTable, true);
			if (MainTable.Width.IsEmpty)
				MainTable.Width = Unit.Percentage(100);
		}
		protected internal override void PrepareControlContentHierarchy() {
			GetControlStyle().AssignToControl(ContainerDiv, true);
			PrepareCombo();
			RenderUtils.SetHorizontalPaddings(ComboCell, Styles.GetComboSpacing(), Unit.Empty);
			RenderUtils.SetVerticalAlign(MainCell, VerticalAlign.Top);
			ComboCell.Width = Table.Width = Unit.Percentage(100);
			if(Properties.EnableResourceComboBox) {
				Combo.Width = Table.Width;
			}
			PrepareButton(FirstButtonId, ResourceNavigatorImages.FirstName);
			PrepareButton(PrevPageButtonId, ResourceNavigatorImages.PrevPageName);
			PrepareButton(PrevButtonId, ResourceNavigatorImages.PrevName);
			PrepareButton(NextButtonId, ResourceNavigatorImages.NextName);
			PrepareButton(NextPageButtonId, ResourceNavigatorImages.NextPageName);
			PrepareButton(LastButtonId, ResourceNavigatorImages.LastName);
			PrepareButton(DecreaseButtonId, ResourceNavigatorImages.DecreaseName);
			PrepareButton(IncreaseButtonId, ResourceNavigatorImages.IncreaseName);
		}
		protected void CreateButtonCell(TableRow row, string id) {
			TableCell cell = RenderUtils.CreateTableCell();
			ResourceNavigatorButton btn = new ResourceNavigatorButton();
			btn.ParentSkinOwner = this;
			cell.Controls.Add(btn);
			row.Cells.Add(cell);
			this.lastButton = Buttons[id] = btn;
			ButtonCells[id] = cell;
		}
		protected void PrepareButton(string id, string imageName) {
			if (Buttons.ContainsKey(id)) {
				ResourceNavigatorButton btn = Buttons[id];
				btn.ControlStyle.CopyFrom(Styles.GetButtonStyle());
				btn.Image.Assign(Images.GetImageProperties(Page, imageName));
				btn.UseSubmitBehavior = false;
				btn.AutoPostBack = false;
				btn.EnableViewState = false;
				btn.ToolTip = GetButtonTooltip(id);
				string callbackCommandId = GetCallbackCommandId(id);
				if(SchedulerControl != null) {
					btn.Enabled = SchedulerControl.CallbackCommandManager.IsCommandEnabled(callbackCommandId);
					btn.ID = id + (btn.Enabled ? "" : "D");
				}
				if (IsAlive())
					btn.ClientSideEvents.Click = string.Format("function() {{ {0} }}", GetButtonOnClick(callbackCommandId));
				if (btn != this.lastButton)
					RenderUtils.SetStyleStringAttribute(ButtonCells[id], "padding-right", Styles.GetButtonSpacing().ToString());
			}
		}
		protected void PrepareCombo() {
			if(!Properties.EnableResourceComboBox)
				return;
			Combo.ID = "cmb";
			if (IsAlive()) {
				ResourceBaseCollection filteredResources = SchedulerControl.GetFilteredResources();
				int count = filteredResources.Count;
				if (count > 0) {
					Combo.ValueType = filteredResources[0].Id.GetType();
					for (int i = 0; i < count; i++) {
						XtraScheduler.Resource res = filteredResources[i];
						Combo.Items.Add(res.Caption, res.Id);
					}
					Combo.ClientSideEvents.SelectedIndexChanged = GetComboOnChange();
					Combo.SelectedIndex = SchedulerControl.ActiveView.ActualFirstVisibleResourceIndex;
				}
			}
			Combo.ItemStyle.Assign(Styles.GetComboItemStyle());
			Combo.ListBoxStyle.Assign(Styles.GetComboListStyle());
			Combo.ControlStyle.CopyFrom(Styles.GetComboStyle());
			Combo.DropDownButton.Image.CopyFrom(Images.ComboBoxDropDown);
			Combo.ClientSideEvents.CloseUp = String.Format("function() {{ ASPx.SchedulerResNavDecorateCombo('{0}'); }}", ClientID);
		}
		#endregion
		protected string GetCallbackCommandId(string buttonId) {
			switch (buttonId) {
				case FirstButtonId:
					return SchedulerCallbackCommandId.NavigateResourceFirst;
				case PrevPageButtonId:
					return SchedulerCallbackCommandId.NavigateResourcePrevPage;
				case PrevButtonId:
					return SchedulerCallbackCommandId.NavigateResourcePrev;
				case NextButtonId:
					return SchedulerCallbackCommandId.NavigateResourceNext;
				case NextPageButtonId:
					return SchedulerCallbackCommandId.NavigateResourceNextPage;
				case LastButtonId:
					return SchedulerCallbackCommandId.NavigateResourceLast;
				case DecreaseButtonId:
					return SchedulerCallbackCommandId.DecrementVisibleResourceCount;
				case IncreaseButtonId:
					return SchedulerCallbackCommandId.IncrementVisibleResourceCount;
			}
			Exceptions.ThrowArgumentException("buttonId", buttonId);
			return String.Empty;
		}
		protected string GetButtonTooltip(string buttonId) {
			SchedulerStringId tooltipStringId = GetButtonTooltipStringId(buttonId);
			return SchedulerLocalizer.GetString(tooltipStringId);
		}
		protected SchedulerStringId GetButtonTooltipStringId(string buttonId) {
			switch (buttonId) {
				case FirstButtonId:
					return SchedulerStringId.Caption_FirstVisibleResources;
				case PrevPageButtonId:
					return SchedulerStringId.Caption_PrevVisibleResourcesPage;
				case PrevButtonId:
					return SchedulerStringId.Caption_PrevVisibleResources;
				case NextButtonId:
					return SchedulerStringId.Caption_NextVisibleResources;
				case NextPageButtonId:
					return SchedulerStringId.Caption_NextVisibleResourcesPage;
				case LastButtonId:
					return SchedulerStringId.Caption_LastVisibleResources;
				case DecreaseButtonId:
					return SchedulerStringId.Caption_DecreaseVisibleResourcesCount;
				case IncreaseButtonId:
					return SchedulerStringId.Caption_IncreaseVisibleResourcesCount;
			}
			Exceptions.ThrowArgumentException("buttonId", buttonId);
			return SchedulerStringId.Caption_IncreaseVisibleResourcesCount;
		}
		#region Client events
		protected internal string GetButtonOnClick(string cmdId) {
			return GetClientScript(cmdId, "''");
		}
		protected internal string GetComboOnChange() {
			return String.Format("function(s, e) {{ {0}; }}", GetClientScript(SchedulerCallbackCommandId.NavigateSpecificResource, "s.GetSelectedIndex()"));
		}
		protected internal string GetClientScript(string cmdId, string parameters) {
			return String.Format("ASPx.SchedulerResNavCmd('{0}', '{1}', {2})", ClientID, cmdId, parameters);
		}
		#endregion
		protected internal override CallbackResult CalcCallbackResultCore() {
			CallbackResult result = base.CalcCallbackResultCore();
			result.Parameters = SchedulerControl.ActiveView.ActualResourcesPerPage.ToString();
			return result;
		}
		protected override StylesBase CreateStyles() {
			return new ResourceNavigatorStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new ResourceNavigatorImages(this);
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return IsAlive();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxResourceNavigator.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if(SchedulerControl != null) {
				sb.AppendFormat("{0}.schedulerControlId='{1}';\n", localVarName, SchedulerControl.ClientID);
				sb.AppendFormat("{0}.visibleResCount = {1};\n", localVarName, SchedulerControl.ActiveView.ActualResourcesPerPage);
				sb.AppendFormat("{0}.RegisterScriptsRestartHandler();\n", localVarName);
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSchedulerResourceNavigator";
		}
		#endregion
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Properties });
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(Styles.GetDefaultControlStyle());
			MergeParentSkinOwnerControlStyle(style);
			((AppearanceStyle)style).Paddings.CopyFrom(Styles.Paddings);			
			style.CopyFrom(ControlStyle);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
	}
	public class ResourceNavigatorProperties : PropertiesBase {
		public ResourceNavigatorProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		#region Properties
		#region EnableFirstLast
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable()]
		public bool EnableFirstLast {
			get { return GetBoolProperty("EnableFirstLast", true); }
			set {
				if (EnableFirstLast != value) {
					SetBoolProperty("EnableFirstLast", true, value);
					Changed();
				}
			}
		}
		#endregion
		#region EnablePrevNextPage
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable()]
		public bool EnablePrevNextPage {
			get { return GetBoolProperty("EnablePrevNextPage", true); }
			set {
				if (EnablePrevNextPage != value) {
					SetBoolProperty("EnablePrevNextPage", true, value);
					Changed();
				}
			}
		}
		#endregion
		#region EnablePrevNext
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable()]
		public bool EnablePrevNext {
			get { return GetBoolProperty("EnablePrevNext", true); }
			set {
				if (EnablePrevNext != value) {
					SetBoolProperty("EnablePrevNext", true, value);
					Changed();
				}
			}
		}
		#endregion
		#region EnableIncreaseDecrease
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable()]
		public bool EnableIncreaseDecrease {
			get { return GetBoolProperty("EnableIncreaseDecrease", true); }
			set {
				if (EnableIncreaseDecrease != value) {
					SetBoolProperty("EnableIncreaseDecrease", true, value);
					Changed();
				}
			}
		}
		#endregion
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable()]
		public bool EnableResourceComboBox {
			get { return GetBoolProperty("EnableResourceComobBox", true); }
			set {
				if (EnableResourceComboBox != value) {
					SetBoolProperty("EnableResourceComobBox", true, value);
					Changed();
				}
			}
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ResourceNavigatorProperties src = source as ResourceNavigatorProperties;
			if (src != null) {
				BeginUpdate();
				try {
					this.EnableFirstLast = src.EnableFirstLast;
					this.EnableIncreaseDecrease = src.EnableIncreaseDecrease;
					this.EnablePrevNext = src.EnablePrevNext;
					this.EnablePrevNextPage = src.EnablePrevNextPage;
					this.EnableResourceComboBox = src.EnableResourceComboBox;
				}
				finally {
					EndUpdate();
				}
			}
		}
	}
	public class ResourceNavigator : ResourceNavigatorProperties {
		public ResourceNavigator(IPropertiesOwner owner)
			: base(owner) {
		}
		#region EnableFirstLast
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ResourceNavigatorVisibility"),
#endif
DefaultValue(ResourceNavigatorVisibility.Auto), NotifyParentProperty(true), AutoFormatEnable()]
		public ResourceNavigatorVisibility Visibility
		{
			get { return (ResourceNavigatorVisibility)GetEnumProperty("Visibility", ResourceNavigatorVisibility.Auto); }
			set
			{
				if (Visibility != value)
				{
					SetEnumProperty("Visibility", ResourceNavigatorVisibility.Auto, value);
					Changed();
				}
			}
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ResourceNavigator src = source as ResourceNavigator;
			if (src != null) {
				this.Visibility = src.Visibility;
			}
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class ResourceNavigatorBlock : ASPxSchedulerControlBlock {
		ASPxResourceNavigator navigator;
		public ResourceNavigatorBlock(ASPxScheduler owner)
			: base(owner) {
		}
		public override string ContentControlID { get { return "resourceNavigatorBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyResourceIntervalChanged; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.navigator = CreateResourceNavigator();
			SetupRelatedControl(navigator);
			parent.Controls.Add(navigator);
		}
		protected virtual ASPxResourceNavigator CreateResourceNavigator() {
			return new ASPxResourceNavigator();
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
			navigator.Images.Assign(Owner.Images.ResourceNavigator);
			navigator.Styles.Assign(Owner.Styles.ResourceNavigator);
			navigator.Properties.Assign(Owner.ResourceNavigator);
			navigator.Border.BorderWidth = 0;
			if(Browser.IsIE || Browser.Family.IsWebKit || Browser.Family.IsNetscape)
				navigator.BorderTop.BorderWidth = 1;
		}
	}
}
