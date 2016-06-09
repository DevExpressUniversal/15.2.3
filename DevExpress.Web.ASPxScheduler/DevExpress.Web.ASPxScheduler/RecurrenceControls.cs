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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using System.Drawing.Design;
namespace DevExpress.Web.ASPxScheduler.Controls { 
	#region RecurrencePropertyNames
	public static class RecurrencePropertyNames {
		public const string FirstDayOfWeek = "FirstDayOfWeek";
		public const string UseAbbreviatedDayNames = "UseAbbreviatedDayNames";
		public const string WeekDays = "WeekDays";
		public const string WeekOfMonth = "WeekOfMonth";
		public const string Periodicity = "Periodicity";
		public const string DayNumber = "DayNumber";
		public const string Start = "Start";
		public const string End = "End";
		public const string Month = "Month";
		public const string RecurrenceRange = "RecurrenceRange";
		public const string RecurrenceType = "RecurrenceType";
		public const string OccurrenceCount = "OccurrenceCount";
		public const string IsRecurring = "IsRecurring";
	}
	#endregion
	#region ASPxSchedulerRecurrenceControlBase
	[Designer("DevExpress.Web.ASPxScheduler.Design.RecurrenceControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)]
	public abstract class ASPxSchedulerRecurrenceControlBase : ASPxWebControl, IEditorsInfoOwner, ISupportRecurrenceValidator {
		#region static        
		internal static RecurrenceType GetValidRecurrenceType(RecurrenceType type, bool enableHourlyRecurrence) {
			bool isValid = (type != RecurrenceType.Minutely) && (type != RecurrenceType.Hourly || enableHourlyRecurrence);
			return isValid ? type : RecurrenceType.Daily;
		}
		internal static WeekDays GetValidWeekDays(RecurrenceType type, DayOfWeek startDayOfWeek) {
			return (type == RecurrenceType.Daily) ? WeekDays.EveryDay : DateTimeHelper.ToWeekDays(startDayOfWeek);
		}
		internal static string GetChildControlClientID(string parentClientID, ASPxSchedulerRecurrenceControlBase control) {
			return string.Format("{0}_{1}_{2}", parentClientID, control.ID, control.MainDivID);
		}
		#endregion
		#region Fields
		WebControl mainDiv;
		ASPxSchedulerRecurrenceValidator validator;
		EditorsInfo editorsInfo;
		bool canPrepareRecurrenceControls = true;
		List<ASPxWebControlBase> internalChildControls = new List<ASPxWebControlBase>();
		#endregion
		protected ASPxSchedulerRecurrenceControlBase() {
		}
		#region Properties
		public WebControl MainDiv { get { return mainDiv; } }
		protected string MainDivID { get { return "mainDiv"; } }
		protected ASPxSchedulerRecurrenceValidator Validator {
			get {
				if (validator == null)
					validator = CreateRecurrenceValidator();
				return validator;
			}
		}
		public EditorsInfo EditorsInfo { get { return editorsInfo; } set { editorsInfo = value; } }
		internal protected bool CanPrepareRecurrenceControls { get { return canPrepareRecurrenceControls; } set { canPrepareRecurrenceControls = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		#endregion
		#region CreateRecurrenceValidator
		protected internal virtual ASPxSchedulerRecurrenceValidator CreateRecurrenceValidator() {
			return new ASPxSchedulerRecurrenceValidator();
		}
		#endregion
		#region CreateControlHierarchy
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(MainDiv);
			CreateChildWebControls();
			AssignChildWebControlsIDs();
			AssignChildWebControlEditorsInfo();
			LayoutChildWebControls(MainDiv);
		}
		#endregion
		#region PrepareControlHierarchy
		protected override void PrepareControlHierarchy() {
			MainDiv.ID = MainDivID;
			MainDiv.EnableViewState = false;
			RenderUtils.SetVisibility(MainDiv, IsClientVisible(), true);
			PrepareCommonPropertiesChildWebControls();
			if (CanPrepareRecurrenceControls)
				PrepareRecurrencePropertiesChildWebControls();
			ApplyStyles();
		}
		#endregion
		#region CreateStyles
		protected override StylesBase CreateStyles() {
			return new ASPxSchedulerRecurrenceControlStyles(this);
		}
		#endregion
		#region ValidateValues
		public virtual void ValidateValues(ValidationArgs args) {
		}
		#endregion
		#region CheckForWarnings
		public virtual void CheckForWarnings(ValidationArgs args) {
		}
		#endregion
		#region RegisterChildControl
		protected void RegisterChildControl(ASPxWebControlBase webControl) {
			this.internalChildControls.Add(webControl);
		}
		#endregion
		#region ApplyStyles
		void ApplyStyles() {
			int count = this.internalChildControls.Count;
			for (int i = 0; i < count; i++)
				this.internalChildControls[i].MergeStyle(ControlStyle);
		}
		#endregion
		protected abstract void CreateChildWebControls();
		protected abstract void AssignChildWebControlsIDs();
		protected abstract void AssignChildWebControlEditorsInfo();
		protected abstract void LayoutChildWebControls(WebControl parent);
		protected abstract void PrepareCommonPropertiesChildWebControls();
		protected abstract void PrepareRecurrencePropertiesChildWebControls();
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptClientAppointmentResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptGlobalFunctionsResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptViewInfosResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptAPIResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptRecurrenceControlsResourceName);
		}
		ASPxSchedulerRecurrenceValidator ISupportRecurrenceValidator.Validator {
			get {
				return Validator;
			}
			set {
				this.validator = value;
			}
		}
	}
	#endregion
	#region ASPxSchedulerRecurrenceControlStyles
	public class ASPxSchedulerRecurrenceControlStyles : StylesBase {
		public ASPxSchedulerRecurrenceControlStyles(ASPxSchedulerRecurrenceControlBase control)
			: base(control) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxsri";
		}
	}
	#endregion
	#region WeekDaysEdit
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxScheduler.Design.RecurrenceControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "WeekDaysEdit.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class WeekDaysEdit : ASPxComboBox {
		public WeekDaysEdit() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			base.ValueType = typeof(Int32);
			PopulateItems();
			DayOfWeek = WeekDays.EveryDay;
		}
		#region Properties
		[Category(SRCategoryNames.Appearance), DefaultValue(WeekDays.EveryDay)]
		public WeekDays DayOfWeek {
			get { return (WeekDays)Value; }
			set { Value = (int)value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		#endregion
		protected virtual void PopulateItems() {
			Items.Clear();
			AddDayOfWeek(WeekDays.EveryDay);
			AddDayOfWeek(WeekDays.WorkDays);
			AddDayOfWeek(WeekDays.WeekendDays);
			AddDayOfWeek(WeekDays.Sunday);
			AddDayOfWeek(WeekDays.Monday);
			AddDayOfWeek(WeekDays.Tuesday);
			AddDayOfWeek(WeekDays.Wednesday);
			AddDayOfWeek(WeekDays.Thursday);
			AddDayOfWeek(WeekDays.Friday);
			AddDayOfWeek(WeekDays.Saturday);
		}
		protected void AddDayOfWeek(WeekDays dayOfWeek) {
			Items.Add(GetDisplayName(dayOfWeek), dayOfWeek);
		}
		protected string GetDisplayName(WeekDays val) {
			if (val == WeekDays.EveryDay)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysEveryDay);
			else if (val == WeekDays.WeekendDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWeekendDays);
			else if (val == WeekDays.WorkDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWorkDays);
			else {
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
				DayOfWeek dayOfWeek = DateTimeHelper.ToDayOfWeek(val);
				return dtfi.GetDayName(dayOfWeek);
			}
		}
	}
	#endregion
	#region MonthEdit
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxScheduler.Design.RecurrenceControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "MonthEdit.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class MonthEdit : ASPxComboBox {
		const int defaultMonth = 1;
		public MonthEdit() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			ValueType = typeof(Int32);
			Month = defaultMonth;
			PopulateItems();
		}
		#region Properties
		[Category(SRCategoryNames.Appearance), DefaultValue(defaultMonth)]
		public int Month { get { return Convert.ToInt32(Value); } set { Value = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		#endregion
		protected virtual void PopulateItems() {
			Items.Clear();
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
			int count = 12;
			for (int i = 1; i <= count; i++)
				Items.Add(dtfi.GetMonthName(i), i);
		}
	}
	#endregion
	#region WeekOfMonthEdit
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxScheduler.Design.RecurrenceControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "WeekOfMonthEdit.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class WeekOfMonthEdit : ASPxComboBox {
		public WeekOfMonthEdit() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			base.ValueType = typeof(Int32);
			WeekOfMonth = WeekOfMonth.First;
			PopulateItems();
		}
		#region Properties
		[Category(SRCategoryNames.Appearance), DefaultValue(WeekOfMonth.First)]
		public WeekOfMonth WeekOfMonth { get { return (WeekOfMonth)Value; } set { Value = (int)value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		[Themeable(false)]
		[MergableProperty(false)]
		[AutoFormatDisable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ListEditItemCollection Items { get { return base.Items; } }
		#endregion
		protected virtual void PopulateItems() {
			Items.Clear();
			AddWeekOfMonth(WeekOfMonth.First);
			AddWeekOfMonth(WeekOfMonth.Second);
			AddWeekOfMonth(WeekOfMonth.Third);
			AddWeekOfMonth(WeekOfMonth.Fourth);
			AddWeekOfMonth(WeekOfMonth.Last);
		}
		protected void AddWeekOfMonth(WeekOfMonth weekOfMonth) {
			Items.Add(GetDisplayName(weekOfMonth), weekOfMonth);
		}
		protected string GetDisplayName(WeekOfMonth val) {
			if (val == WeekOfMonth.First)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekOfMonthFirst);
			else if (val == WeekOfMonth.Second)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekOfMonthSecond);
			else if (val == WeekOfMonth.Third)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekOfMonthThird);
			else if (val == WeekOfMonth.Fourth)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekOfMonthFourth);
			else if (val == WeekOfMonth.Last)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekOfMonthLast);
			else
				return null;
		}
	}
	#endregion
	#region WeekDaysCheckEditItemsControl
	public class WeekDaysCheckEditItemsControl : ItemsControl<DayOfWeek> {
		#region Fields
		IEditorsInfoOwner editorsInfoOwner;
		WeekDays weekDays = WeekDays.EveryDay;
		DevExpress.XtraScheduler.FirstDayOfWeek firstDayOfWeek = DevExpress.XtraScheduler.FirstDayOfWeek.System;
		bool useAbbreviatedDayNames = true;
		bool canPrepareChildControls;
		#endregion
		public WeekDaysCheckEditItemsControl(IEditorsInfoOwner editorsInfoOwner, List<DayOfWeek> visibleDayOfWeek)
			: base(visibleDayOfWeek, 4, RepeatDirection.Horizontal, RepeatLayout.Table) {
			this.editorsInfoOwner = editorsInfoOwner;
		}
		#region Properties
		public WeekDays WeekDays { get { return weekDays; } set { weekDays = value; } }
		public bool UseAbbreviatedDayNames { get { return useAbbreviatedDayNames; } set { useAbbreviatedDayNames = value; } }
		public DevExpress.XtraScheduler.FirstDayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
		[Browsable(false)]
		public WeekDays WeekDaysValue { get { return CalculateWeekDaysValue(); } }
		protected EditorsInfo EditorsInfo { get { return editorsInfoOwner != null ? editorsInfoOwner.EditorsInfo : null; } }
		public bool CanPrepareChildControls { get { return canPrepareChildControls; } set { canPrepareChildControls = value; } }
		internal Dictionary<int, Control> InternalItemControls { get { return base.ItemControls; } }
		#endregion
		protected override Control CreateItemControl(int index, DayOfWeek item) {
			ASPxCheckBox check = new ASPxCheckBox();
			if (EditorsInfo != null) {
				EditorsInfo.Apply(check);
			}
			check.ID = String.Format("{0}{1}", item, index);
			return check;
		}
		protected override void PrepareItemControl(Control control, int index, DayOfWeek item) {
			ASPxCheckBox checkBox = (ASPxCheckBox)control;
			checkBox.EnableViewState = false;
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
			checkBox.Text = UseAbbreviatedDayNames ? dtfi.GetAbbreviatedDayName(item) : dtfi.GetDayName(item);
			checkBox.Wrap = DefaultBoolean.False;
			if (CanPrepareChildControls)
				checkBox.Checked = (WeekDays & DateTimeHelper.ToWeekDays(item)) != 0;
		}
		protected virtual WeekDays CalculateWeekDaysValue() {
			WeekDays result = (DevExpress.XtraScheduler.WeekDays)0;
			for (int i = 0; i < ItemControls.Count; i++) {
				ASPxCheckBox check = (ASPxCheckBox)ItemControls[i];
				if (check.Checked)
					result |= DateTimeHelper.ToWeekDays((DayOfWeek)Items[i]);
			}
			return result;
		}
	}
	#endregion
	#region WeekDaysCheckEdit
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxScheduler.Design.RecurrenceControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "WeekDaysCheckEdit.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class WeekDaysCheckEdit : ASPxWebControl, IEditorsInfoOwner {
		#region Fields
		WeekDaysCheckEditItemsControl itemsControl;
		EditorsInfo editorsInfo;
		bool canPrepareChildControls;
		bool enableScriptSupport = true;
		#endregion
		public WeekDaysCheckEdit() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected WeekDaysCheckEditItemsControl ItemsControl { get { return itemsControl; } }
		[Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[Category(SRCategoryNames.Appearance), DefaultValue(false)]
		public bool UseAbbreviatedDayNames {
			get { return GetBoolProperty(RecurrencePropertyNames.UseAbbreviatedDayNames, true); }
			set { SetBoolProperty(RecurrencePropertyNames.UseAbbreviatedDayNames, true, value); }
		}
		[Category(SRCategoryNames.Appearance), DefaultValue(DevExpress.XtraScheduler.FirstDayOfWeek.System)]
		public DevExpress.XtraScheduler.FirstDayOfWeek FirstDayOfWeek {
			get { return (DevExpress.XtraScheduler.FirstDayOfWeek)GetEnumProperty(RecurrencePropertyNames.FirstDayOfWeek, DevExpress.XtraScheduler.FirstDayOfWeek.System); }
			set { SetEnumProperty(RecurrencePropertyNames.FirstDayOfWeek, DevExpress.XtraScheduler.FirstDayOfWeek.System, value); }
		}
		[Browsable(false)]
		public WeekDays WeekDaysValue { get { return ItemsControl.WeekDaysValue; } }
		public EditorsInfo EditorsInfo { get { return editorsInfo; } set { editorsInfo = value; } }
		protected internal bool CanPrepareChildControls { get { return canPrepareChildControls; } set { canPrepareChildControls = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		public bool EnableScriptSupport { get { return enableScriptSupport; } set { enableScriptSupport = value; } }
		#endregion
		#region CreateControlHierarchy
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			List<DayOfWeek> visibleDayOfWeek = GetDayOfWeekList();
			int count = visibleDayOfWeek.Count;
			if (count <= 0)
				return;
			this.itemsControl = new WeekDaysCheckEditItemsControl(this, visibleDayOfWeek);
			this.itemsControl.WeekDays = WeekDays;
			ItemsControl.ID = "Items";
			ItemsControl.ItemSpacing = Unit.Pixel(5);
			Controls.Add(itemsControl);
		}
		#endregion
		#region PrepareControlHierarchy
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ItemsControl.CanPrepareChildControls = CanPrepareChildControls;
			ItemsControl.EnableViewState = false;
			if (CanPrepareChildControls)
				ItemsControl.WeekDays = WeekDays;
		}
		#endregion
		#region GetDayOfWeekList
		protected internal List<DayOfWeek> GetDayOfWeekList() {
			DayOfWeek[] values = DateTimeHelper.GetWeekDays(DateTimeHelper.ConvertFirstDayOfWeek(FirstDayOfWeek));
			return new List<DayOfWeek>(values);
		}
		#endregion
		protected override bool HasFunctionalityScripts() {
			return EnableScriptSupport;
		}
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientWeekDaysCheckEdit";
			return base.GetClientObjectClassName();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.itemsControl = new Object();\n", localVarName);
			int count = ItemsControl.Items.Count;
			for (int i = 0; i < count; i++) {
				DayOfWeek dayOfWeek = ItemsControl.Items[i];
				WeekDays weekDays = DateTimeHelper.ToWeekDays(dayOfWeek);
				ASPxCheckBox control = (ASPxCheckBox)ItemsControl.InternalItemControls[i];
				stb.AppendFormat("{0}.itemsControl[{1}] = {2};\n", localVarName, (int)weekDays, control.ClientID);
			}
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			if (EnableScriptSupport) {
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptClientAppointmentResourceName);
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptResourceName);
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptRecurrenceControlsResourceName);
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptViewInfosResourceName);
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptAPIResourceName);
				RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptGlobalFunctionsResourceName);
			}
		}
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region ClientSideSupportRecurrenceRuleControlBase
	public abstract class ClientSideSupportRecurrenceRuleControlBase : RecurrenceRuleControlBase {
		bool enableScriptSupport = true;
		#region ClientVisible
		[Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		#endregion
		#region ClientInstanceName
		[Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		#endregion
		#region EnableScriptSupport
		public bool EnableScriptSupport { get { return enableScriptSupport; } set { enableScriptSupport = value; } }
		#endregion
		#region HasFunctionalityScripts
		protected override bool HasFunctionalityScripts() {
			return EnableScriptSupport;
		}
		#endregion
	}
	#endregion
	#region HourlyRecurrenceControl
	[
#if DEBUG
DXWebToolboxItem(true),
ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "DailyRecurrenceControl.bmp"),
ToolboxTabName(AssemblyInfo.DXTabScheduling)
#else
	DXToolboxItem(false),
	DXWebToolboxItem(false),	
	ToolboxItem(false)
#endif
]
	public class HourlyRecurrenceControl : ClientSideSupportRecurrenceRuleControlBase {
		ASPxSpinEdit spinHourCount;
		RecurrenceControlLabel lblRecurrenceEvery;
		RecurrenceControlLabel lblHour;
		public HourlyRecurrenceControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		protected ASPxSpinEdit SpinHourCount { get { return spinHourCount; } }
		protected RecurrenceControlLabel LblRecurEvery { get { return lblRecurrenceEvery; } }
		protected RecurrenceControlLabel LblHour { get { return lblHour; } }
		[
		Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Periodicity {
			get { return GetIntProperty(RecurrencePropertyNames.Periodicity, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Periodicity, 1, value); }
		}
		[Browsable(false)]
		public int ClientPeriodicity { get { return CalculateClientPeriodicity(); } }
		#region CreateRuleValuesAccessor
		protected internal override RecurrenceRuleValuesAccessor CreateRuleValuesAccessor() {
			return new HourlyRecurrenceValuesAccessor(this);
		}
		#endregion
		#region CalculateClientPeriodicity
		protected virtual int CalculateClientPeriodicity() {
			return Convert.ToInt32(SpinHourCount.Number);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.spinHourCount = new ASPxSpinEdit();
			this.lblRecurrenceEvery = new RecurrenceControlLabel();
			this.lblHour = new RecurrenceControlLabel();
			RegisterChildControl(SpinHourCount);
			RegisterChildControl(LblRecurEvery);
			RegisterChildControl(LblHour);
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			SpinHourCount.ID = "SpnHourCount";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo == null)
				return;
			EditorsInfo.Apply(SpinHourCount);
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table t1 = TableHelper.CreateSingleRowTable(new WebControl[] { LblRecurEvery, SpinHourCount, LblHour });
			parent.Controls.Add(t1);
			RenderUtils.ApplyCellPadding(t1, 3);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			LblRecurEvery.EnableViewState = false;
			LblRecurEvery.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_RecurEvery);
			LblHour.EnableViewState = false;
			LblHour.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Hour);
			SpinHourCount.EnableViewState = false;
			SpinHourCount.NumberType = SpinEditNumberType.Integer;
			SpinHourCount.MinValue = 1;
			SpinHourCount.MaxValue = 999;
			SpinHourCount.AllowNull = false;
			SpinHourCount.Width = 60;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			SpinHourCount.Number = Periodicity;
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			Validator.ValidateDayCount(args, SpinHourCount, ClientPeriodicity);
		}
		#endregion
		#region GetClientObjectClassName
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientHourlyRecurrenceControl";
			return base.GetClientObjectClassName();
		}
		#endregion
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.SpinHourCount = {1};\n", localVarName, SpinHourCount.ClientID);
		}
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region HourlyRecurrenceValuesAccessor
	public class HourlyRecurrenceValuesAccessor : DefaultRecurrenceRuleValuesAccessor {
		public HourlyRecurrenceValuesAccessor(RecurrenceRuleControlBase recurrenceControl)
			: base(recurrenceControl) {
		}
		public new HourlyRecurrenceControl RecurrenceControl { get { return (HourlyRecurrenceControl)base.RecurrenceControl; } }
		public override int GetPeriodicity() {
			return RecurrenceControl.ClientPeriodicity;
		}
	}
	#endregion
	#region DailyRecurrenceControl
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "DailyRecurrenceControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class DailyRecurrenceControl : ClientSideSupportRecurrenceRuleControlBase {
		#region Fields
		ASPxRadioButton rbDay;
		ASPxRadioButton rbEveryWeekDay;
		ASPxSpinEdit spinDailyDaysCount;
		RecurrenceControlLabel lblDailyDaysCount;
		#endregion
		public DailyRecurrenceControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected ASPxRadioButton RbDay { get { return rbDay; } }
		protected ASPxRadioButton RbEveryWeekDay { get { return rbEveryWeekDay; } }
		protected ASPxSpinEdit SpinDailyDaysCount { get { return spinDailyDaysCount; } }
		protected RecurrenceControlLabel LblDailyDaysCount { get { return lblDailyDaysCount; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DailyRecurrenceControlPeriodicity"),
#endif
Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Periodicity {
			get { return GetIntProperty(RecurrencePropertyNames.Periodicity, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Periodicity, 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DailyRecurrenceControlWeekDays"),
#endif
Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (DevExpress.XtraScheduler.WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[Browsable(false)]
		public int ClientPeriodicity { get { return CalculateClientPeriodicity(); } }
		[Browsable(false)]
		public WeekDays ClientWeekDays { get { return CalculateClientWeekDays(); } }
		#endregion
		#region CreateRuleValuesAccessor
		protected internal override RecurrenceRuleValuesAccessor CreateRuleValuesAccessor() {
			return new DailyRecurrenceValuesAccessor(this);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.rbDay = new ASPxRadioButton();
			this.rbEveryWeekDay = new ASPxRadioButton();
			this.spinDailyDaysCount = new ASPxSpinEdit();
			this.lblDailyDaysCount = new RecurrenceControlLabel();
			RegisterChildControl(RbDay);
			RegisterChildControl(RbEveryWeekDay);
			RegisterChildControl(SpinDailyDaysCount);
			RegisterChildControl(LblDailyDaysCount);
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			RbDay.ID = "RbDay";
			RbEveryWeekDay.ID = "RbEvrDay";
			SpinDailyDaysCount.ID = "SpnDayCnt";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo == null)
				return;
			EditorsInfo.Apply(RbDay);
			EditorsInfo.Apply(RbEveryWeekDay);
			EditorsInfo.Apply(SpinDailyDaysCount);
			EditorsInfo.Apply(LblDailyDaysCount.Label);
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table t1 = TableHelper.CreateSingleRowTable(new WebControl[] { RbDay, SpinDailyDaysCount, LblDailyDaysCount });
			Table t2 = TableHelper.CreateSingleRowTable(new WebControl[] { RbEveryWeekDay });
			parent.Controls.Add(t1);
			parent.Controls.Add(t2);
			RenderUtils.ApplyCellPadding(t1, 3);
			RenderUtils.ApplyCellPadding(t2, 3);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			const string groupName = "rcDailyGr";
			RbDay.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Every);
			RbDay.EnableViewState = false;
			RbDay.GroupName = groupName;
			RbDay.Wrap = DefaultBoolean.False;
			RbEveryWeekDay.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_EveryWeekday);
			RbEveryWeekDay.EnableViewState = false;
			RbEveryWeekDay.GroupName = groupName;
			RbEveryWeekDay.EncodeHtml = false;
			RbEveryWeekDay.Wrap = DefaultBoolean.False;
			LblDailyDaysCount.EnableViewState = false;
			LblDailyDaysCount.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Days);
			SpinDailyDaysCount.EnableViewState = false;
			SpinDailyDaysCount.NumberType = SpinEditNumberType.Integer;
			SpinDailyDaysCount.MinValue = 1;
			SpinDailyDaysCount.MaxValue = 999;
			SpinDailyDaysCount.AllowNull = false;
			SpinDailyDaysCount.Width = 60;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			RbDay.Checked = false;
			RbEveryWeekDay.Checked = false;
			if (WeekDays == WeekDays.EveryDay)
				RbDay.Checked = true;
			else
				RbEveryWeekDay.Checked = true;
			SpinDailyDaysCount.Number = Periodicity;
		}
		#endregion
		#region CalculateClientPeriodicity
		protected virtual int CalculateClientPeriodicity() {
			return Convert.ToInt32(SpinDailyDaysCount.Number);
		}
		#endregion
		#region CalculateClientWeekDays
		protected virtual WeekDays CalculateClientWeekDays() {
			return RbDay.Checked ? WeekDays.EveryDay : WeekDays.WorkDays;
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			if (RbEveryWeekDay.Checked)
				return;
			Validator.ValidateDayCount(args, SpinDailyDaysCount, ClientPeriodicity);
		}
		#endregion
		#region GetClientObjectClassName
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientDailyRecurrenceControl";
			return base.GetClientObjectClassName();
		}
		#endregion
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.RbDay = {1};\n", localVarName, RbDay.ClientID);
			stb.AppendFormat("{0}.RbEveryWeekDay = {1};\n", localVarName, RbEveryWeekDay.ClientID);
			stb.AppendFormat("{0}.SpinDailyDaysCount = {1};\n", localVarName, SpinDailyDaysCount.ClientID);
		}
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region DailyRecurrenceValuesAccessor
	public class DailyRecurrenceValuesAccessor : DefaultRecurrenceRuleValuesAccessor {
		public DailyRecurrenceValuesAccessor(RecurrenceRuleControlBase recurrenceControl)
			: base(recurrenceControl) {
		}
		public new DailyRecurrenceControl RecurrenceControl { get { return (DailyRecurrenceControl)base.RecurrenceControl; } }
		public override int GetPeriodicity() {
			return RecurrenceControl.ClientPeriodicity;
		}
		public override WeekDays GetWeekDays() {
			return RecurrenceControl.ClientWeekDays;
		}
	}
	#endregion
	#region WeeklyRecurrenceControl
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "WeeklyRecurrenceControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class WeeklyRecurrenceControl : ClientSideSupportRecurrenceRuleControlBase {
		#region Fields
		RecurrenceControlLabel lblWeeklyWeeksText;
		RecurrenceControlLabel lblWeeklyWeeksCount;
		ASPxSpinEdit spinWeeklyWeeksCount;
		WeekDaysCheckEdit weekDaysCheckEdit;
		#endregion
		public WeeklyRecurrenceControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected RecurrenceControlLabel LblWeeklyWeeksText { get { return lblWeeklyWeeksText; } }
		protected RecurrenceControlLabel LblWeeklyWeeksCount { get { return lblWeeklyWeeksCount; } }
		protected ASPxSpinEdit SpinWeeklyWeeksCount { get { return spinWeeklyWeeksCount; } }
		protected WeekDaysCheckEdit WeekDaysCheckEdit { get { return weekDaysCheckEdit; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WeeklyRecurrenceControlPeriodicity"),
#endif
Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Periodicity {
			get { return GetIntProperty(RecurrencePropertyNames.Periodicity, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Periodicity, 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("WeeklyRecurrenceControlWeekDays"),
#endif
Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[Browsable(false)]
		public int ClientPeriodicity { get { return CalculateClientPeriodicity(); } }
		[Browsable(false)]
		public WeekDays ClientWeekDays { get { return CalculateClientWeekDays(); } }
		#endregion
		#region CreateRuleValuesAccessor
		protected internal override RecurrenceRuleValuesAccessor CreateRuleValuesAccessor() {
			return new WeeklyRecurrenceValuesAccessor(this);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.lblWeeklyWeeksCount = new RecurrenceControlLabel();
			this.lblWeeklyWeeksText = new RecurrenceControlLabel();
			this.spinWeeklyWeeksCount = new ASPxSpinEdit();
			this.weekDaysCheckEdit = new WeekDaysCheckEdit();
			this.WeekDaysCheckEdit.EnableScriptSupport = EnableScriptSupport;
			RegisterChildControl(lblWeeklyWeeksCount);
			RegisterChildControl(lblWeeklyWeeksText);
			RegisterChildControl(spinWeeklyWeeksCount);
			RegisterChildControl(weekDaysCheckEdit);
			RegisterChildControl(WeekDaysCheckEdit);
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			SpinWeeklyWeeksCount.ID = "SpnWeekCnt";
			WeekDaysCheckEdit.ID = "WeekDaysEdt";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo != null) {
				EditorsInfo.Apply(SpinWeeklyWeeksCount);
				EditorsInfo.Apply(LblWeeklyWeeksCount.Label);
				EditorsInfo.Apply(LblWeeklyWeeksText.Label);
			}
			WeekDaysCheckEdit.EditorsInfo = EditorsInfo;
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table t1 = TableHelper.CreateSingleRowTable(new WebControl[] { LblWeeklyWeeksText, SpinWeeklyWeeksCount, LblWeeklyWeeksCount });
			Table t2 = TableHelper.CreateSingleRowTable(new WebControl[] { WeekDaysCheckEdit });
			RenderUtils.ApplyCellPadding(t1, 3);
			RenderUtils.ApplyCellPadding(t2, 3);
			parent.Controls.Add(t1);
			parent.Controls.Add(t2);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			WeekDaysCheckEdit.EnableViewState = false;
			LblWeeklyWeeksText.EnableViewState = false;
			LblWeeklyWeeksText.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_RecurEvery);
			LblWeeklyWeeksText.AssociatedControlID = "SpnWeekCnt";
			LblWeeklyWeeksCount.EnableViewState = false;
			LblWeeklyWeeksCount.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_WeeksOn);
			SpinWeeklyWeeksCount.EnableViewState = false;
			SpinWeeklyWeeksCount.NumberType = SpinEditNumberType.Integer;
			SpinWeeklyWeeksCount.Width = 60;
			SpinWeeklyWeeksCount.MinValue = 1;
			SpinWeeklyWeeksCount.MaxValue = 99;
			SpinWeeklyWeeksCount.AllowNull = false;
			WeekDaysCheckEdit.CanPrepareChildControls = CanPrepareRecurrenceControls;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			WeekDaysCheckEdit.WeekDays = WeekDays;
			SpinWeeklyWeeksCount.Number = Periodicity;
		}
		#endregion
		#region CalculateClientPeriodicity
		protected virtual int CalculateClientPeriodicity() {
			return Convert.ToInt32(SpinWeeklyWeeksCount.Number);
		}
		#endregion
		#region CalculateClientWeekDays
		protected virtual WeekDays CalculateClientWeekDays() {
			return WeekDaysCheckEdit.WeekDaysValue;
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			Validator.ValidateWeekCount(args, SpinWeeklyWeeksCount, ClientPeriodicity);
			if (!args.Valid)
				return;
			Validator.ValidateDayOfWeek(args, WeekDaysCheckEdit, ClientWeekDays);
		}
		#endregion
		#region GetClientObjectClassName
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientWeeklyRecurrenceControl";
			return base.GetClientObjectClassName();
		}
		#endregion
		#region GetCreateClientObjectScript
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.SpinWeeklyWeeksCount = {1};\n", localVarName, SpinWeeklyWeeksCount.ClientID);
			stb.AppendFormat("{0}.WeekDaysCheckEdit = {1};\n", localVarName, WeekDaysCheckEdit.ClientID);
		}
		#endregion
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region WeeklyRecurrenceValuesAccessor
	public class WeeklyRecurrenceValuesAccessor : DefaultRecurrenceRuleValuesAccessor {
		public WeeklyRecurrenceValuesAccessor(RecurrenceRuleControlBase recurrenceControl)
			: base(recurrenceControl) {
		}
		public new WeeklyRecurrenceControl RecurrenceControl { get { return (WeeklyRecurrenceControl)base.RecurrenceControl; } }
		public override int GetPeriodicity() {
			return RecurrenceControl.ClientPeriodicity;
		}
		public override WeekDays GetWeekDays() {
			return RecurrenceControl.ClientWeekDays;
		}
	}
	#endregion
	#region MonthlyRecurrenceControl
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "MonthlyRecurrenceControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class MonthlyRecurrenceControl : ClientSideSupportRecurrenceRuleControlBase {
		#region Fields
		RecurrenceControlLabel lblMonthlyOfEvery;
		RecurrenceControlLabel lblDayMonthCount;
		RecurrenceControlLabel lblWeekOfMonthMonthCount;
		RecurrenceControlLabel lblOfEveryWeekOfMonth;
		WeekDaysEdit wdeMonthlyWeekDays;
		ASPxSpinEdit spinMonthlyDay;
		ASPxSpinEdit spinMonthlyWeekDaysMonthCount;
		ASPxSpinEdit spinMonthlyDayMonthCount;
		WeekOfMonthEdit wmeMonthlyWeekOfMonth;
		ASPxRadioButton rbDay;
		ASPxRadioButton rbWeekDays;
		#endregion
		public MonthlyRecurrenceControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected RecurrenceControlLabel LblMonthlyOfEvery { get { return lblMonthlyOfEvery; } }
		protected RecurrenceControlLabel LblDayMonthCount { get { return lblDayMonthCount; } }
		protected RecurrenceControlLabel LblWeekOfMonthMonthCount { get { return lblWeekOfMonthMonthCount; } }
		protected RecurrenceControlLabel LblOfEveryWeekOfMonth { get { return lblOfEveryWeekOfMonth; } }
		protected ASPxSpinEdit SpinMonthlyDay { get { return spinMonthlyDay; } }
		protected ASPxSpinEdit SpinMonthlyDayMonthCount { get { return spinMonthlyDayMonthCount; } }
		protected ASPxSpinEdit SpinMonthlyWeekDaysMonthCount { get { return spinMonthlyWeekDaysMonthCount; } }
		protected ASPxRadioButton RbDay { get { return rbDay; } }
		protected ASPxRadioButton RbWeekDays { get { return rbWeekDays; } }
		protected WeekDaysEdit WdeMonthlyWeekDays { get { return wdeMonthlyWeekDays; } }
		protected WeekOfMonthEdit WmeMonthlyWeekOfMonth { get { return wmeMonthlyWeekOfMonth; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthlyRecurrenceControlPeriodicity"),
#endif
Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Periodicity {
			get { return GetIntProperty(RecurrencePropertyNames.Periodicity, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Periodicity, 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthlyRecurrenceControlDayNumber"),
#endif
Category(SRCategoryNames.Data), DefaultValue(1)]
		public int DayNumber {
			get { return GetIntProperty(RecurrencePropertyNames.DayNumber, 1); }
			set { SetIntProperty(RecurrencePropertyNames.DayNumber, 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthlyRecurrenceControlWeekDays"),
#endif
Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthlyRecurrenceControlWeekOfMonth"),
#endif
Category(SRCategoryNames.Data), DefaultValue(WeekOfMonth.None)]
		public WeekOfMonth WeekOfMonth {
			get { return (WeekOfMonth)GetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("MonthlyRecurrenceControlStart"),
#endif
Category(SRCategoryNames.Data)]
		public DateTime Start {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today, value); }
		}
		[Browsable(false)]
		public int ClientPeriodicity { get { return CalculateClientPeriodicity(); } }
		[Browsable(false)]
		public int ClientDayNumber { get { return CalculateClientDayNumber(); } }
		[Browsable(false)]
		public WeekDays ClientWeekDays { get { return CalculateClientWeekDays(); } }
		[Browsable(false)]
		public WeekOfMonth ClientWeekOfMonth { get { return CalculateClientWeekOfMonth(); } }
		#endregion
		#region CreateRuleValuesAccessor
		protected internal override RecurrenceRuleValuesAccessor CreateRuleValuesAccessor() {
			return new MonthlyRecurrenceValuesAccessor(this);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.lblMonthlyOfEvery = new RecurrenceControlLabel();
			this.lblDayMonthCount = new RecurrenceControlLabel();
			this.lblWeekOfMonthMonthCount = new RecurrenceControlLabel();
			this.lblOfEveryWeekOfMonth = new RecurrenceControlLabel();
			this.spinMonthlyDay = new ASPxSpinEdit();
			this.spinMonthlyDayMonthCount = new ASPxSpinEdit();
			this.spinMonthlyWeekDaysMonthCount = new ASPxSpinEdit();
			this.rbDay = new ASPxRadioButton();
			this.rbWeekDays = new ASPxRadioButton();
			this.wdeMonthlyWeekDays = new WeekDaysEdit();
			this.wmeMonthlyWeekOfMonth = new WeekOfMonthEdit();
			RegisterChildControl(this.lblMonthlyOfEvery);
			RegisterChildControl(this.lblDayMonthCount);
			RegisterChildControl(this.lblWeekOfMonthMonthCount);
			RegisterChildControl(this.lblOfEveryWeekOfMonth);
			RegisterChildControl(this.spinMonthlyDay);
			RegisterChildControl(this.spinMonthlyDayMonthCount);
			RegisterChildControl(this.spinMonthlyWeekDaysMonthCount);
			RegisterChildControl(this.rbDay);
			RegisterChildControl(this.rbWeekDays);
			RegisterChildControl(this.wdeMonthlyWeekDays);
			RegisterChildControl(this.wmeMonthlyWeekOfMonth);
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			SpinMonthlyDay.ID = "SpnMnthDay";
			SpinMonthlyDayMonthCount.ID = "SpnDayMnthCnt";
			SpinMonthlyWeekDaysMonthCount.ID = "SpinWeekDaysMnthCnt";
			RbDay.ID = "RbDay";
			RbWeekDays.ID = "RbWeekDays";
			WdeMonthlyWeekDays.ID = "WdeWeekDays";
			WmeMonthlyWeekOfMonth.ID = "WmeWeekOfMnth";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo == null)
				return;
			EditorsInfo.Apply(SpinMonthlyDay);
			EditorsInfo.Apply(SpinMonthlyDayMonthCount);
			EditorsInfo.Apply(SpinMonthlyWeekDaysMonthCount);
			EditorsInfo.Apply(RbDay);
			EditorsInfo.Apply(RbWeekDays);
			EditorsInfo.Apply(WmeMonthlyWeekOfMonth);
			EditorsInfo.Apply(WdeMonthlyWeekDays);
			EditorsInfo.Apply(LblDayMonthCount.Label);
			EditorsInfo.Apply(LblMonthlyOfEvery.Label);
			EditorsInfo.Apply(LblWeekOfMonthMonthCount.Label);
			EditorsInfo.Apply(LblOfEveryWeekOfMonth.Label);
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table t1 = TableHelper.CreateSingleRowTable(new WebControl[] { SpinMonthlyDay, LblMonthlyOfEvery, SpinMonthlyDayMonthCount, LblDayMonthCount });
			Table t2 = TableHelper.CreateSingleRowTable(new WebControl[] { WmeMonthlyWeekOfMonth, WdeMonthlyWeekDays, LblOfEveryWeekOfMonth, SpinMonthlyWeekDaysMonthCount, LblWeekOfMonthMonthCount });
			Table resultTable = TableHelper.AddNewTableRow(RenderUtils.CreateTable(), new WebControl[] { RbDay, t1 });
			resultTable = TableHelper.AddNewTableRow(resultTable, new WebControl[] { RbWeekDays, t2 });
			RenderUtils.ApplyCellPadding(t1, 3);
			RenderUtils.ApplyCellPadding(t2, 3);
			RenderUtils.ApplyCellPadding(resultTable, 2);
			parent.Controls.Add(resultTable);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			const string groupName = "rcMonthlyGr";
			RbDay.EnableViewState = false;
			RbDay.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Day);
			RbDay.GroupName = groupName;
			RbWeekDays.EnableViewState = false;
			RbWeekDays.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_The);
			RbWeekDays.GroupName = groupName;
			RbWeekDays.Wrap = DefaultBoolean.False;
			LblMonthlyOfEvery.EnableViewState = false;
			LblMonthlyOfEvery.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_OfEvery);
			LblMonthlyOfEvery.AssociatedControlID = "SpnDayMnthCnt";
			LblOfEveryWeekOfMonth.EnableViewState = false;
			LblOfEveryWeekOfMonth.AssociatedControlID = "SpinWeekDaysMnthCnt";
			LblOfEveryWeekOfMonth.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_OfEvery);
			LblDayMonthCount.EnableViewState = false;
			LblDayMonthCount.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Months);
			LblWeekOfMonthMonthCount.EnableViewState = false;
			LblWeekOfMonthMonthCount.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Months);
			SpinMonthlyDay.EnableViewState = false;
			SpinMonthlyDay.NumberType = SpinEditNumberType.Integer;
			SpinMonthlyDay.MinValue = 1;
			SpinMonthlyDay.Width = 60;
			SpinMonthlyDay.MaxValue = 31;
			SpinMonthlyDayMonthCount.EnableViewState = false;
			SpinMonthlyDayMonthCount.NumberType = SpinEditNumberType.Integer;
			SpinMonthlyDayMonthCount.MinValue = 1;
			SpinMonthlyDayMonthCount.Width = 60;
			SpinMonthlyDayMonthCount.MaxValue = 99;
			SpinMonthlyWeekDaysMonthCount.EnableViewState = false;
			SpinMonthlyWeekDaysMonthCount.NumberType = SpinEditNumberType.Integer;
			SpinMonthlyWeekDaysMonthCount.MinValue = 1;
			SpinMonthlyWeekDaysMonthCount.Width = 60;
			SpinMonthlyWeekDaysMonthCount.MaxValue = 99;
			WdeMonthlyWeekDays.EnableViewState = false;
			WdeMonthlyWeekDays.Width = 105;
			WmeMonthlyWeekOfMonth.EnableViewState = false;
			WmeMonthlyWeekOfMonth.Width = 75;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			SpinMonthlyDayMonthCount.Number = Periodicity;
			SpinMonthlyWeekDaysMonthCount.Number = Periodicity;
			SpinMonthlyDayMonthCount.AllowNull = false;
			SpinMonthlyWeekDaysMonthCount.AllowNull = false;
			WdeMonthlyWeekDays.DayOfWeek = WeekDays;
			RbDay.Checked = false;
			RbWeekDays.Checked = false;
			if (WeekOfMonth == WeekOfMonth.None) {
				RbDay.Checked = true;
				WmeMonthlyWeekOfMonth.WeekOfMonth = CalcWeekOfMonth();
				SpinMonthlyDay.Number = DayNumber;
				SpinMonthlyDay.AllowNull = false;
			} else {
				RbWeekDays.Checked = true;
				WmeMonthlyWeekOfMonth.WeekOfMonth = WeekOfMonth;
				SpinMonthlyDay.Number = Start.Day;
			}
		}
		#endregion
		#region CalcWeekOfMonth
		protected internal virtual WeekOfMonth CalcWeekOfMonth() {
			int count = 0;
			DateTime date = new DateTime(Start.Year, Start.Month, 1);
			DayOfWeek dayOfWeek = DateTimeHelper.ToDayOfWeek(WeekDays);
			while (date <= Start) {
				if (date.DayOfWeek == dayOfWeek)
					count++;
				date = date.AddDays(1);
			}
			if (count >= 1 && count <= 4)
				return (WeekOfMonth)count;
			else if (count >= 5)
				return WeekOfMonth.Last;
			else
				return WeekOfMonth.First;
		}
		#endregion
		#region CalculateClientDayNumber
		protected virtual int CalculateClientDayNumber() {
			return Convert.ToInt32(SpinMonthlyDay.Number);
		}
		#endregion
		#region CalculateClientPeriodicity
		protected virtual int CalculateClientPeriodicity() {
			decimal result = RbDay.Checked ? SpinMonthlyDayMonthCount.Number : SpinMonthlyWeekDaysMonthCount.Number;
			return Convert.ToInt32(result);
		}
		#endregion
		#region CalculateClientWeekDays
		protected virtual WeekDays CalculateClientWeekDays() {
			return WdeMonthlyWeekDays.DayOfWeek;
		}
		#endregion
		#region CalculateClientWeekOfMonth
		protected virtual WeekOfMonth CalculateClientWeekOfMonth() {
			return RbDay.Checked ? WeekOfMonth.None : WmeMonthlyWeekOfMonth.WeekOfMonth;
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			if (RbDay.Checked) {
				Validator.ValidateMonthCount(args, SpinMonthlyDayMonthCount, ClientPeriodicity);
				if (!args.Valid)
					return;
				Validator.ValidateDayNumber(args, SpinMonthlyDay, ClientDayNumber, 31);
			} else
				Validator.ValidateMonthCount(args, SpinMonthlyWeekDaysMonthCount, ClientPeriodicity);
		}
		#endregion
		#region CheckForWarnings
		public override void CheckForWarnings(ValidationArgs args) {
			if (Validator.NeedToCheckLargeDayNumberWarning(ClientWeekOfMonth))
				Validator.CheckLargeDayNumberWarning(args, SpinMonthlyDay, ClientDayNumber);
		}
		#endregion
		#region GetClientObjectClassName
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientMonthlyRecurrenceControl";
			return base.GetClientObjectClassName();
		}
		#endregion
		#region GetCreateClientObjectScript
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.spinMonthlyDay = {1};\n", localVarName, SpinMonthlyDay.ClientID);
			stb.AppendFormat("{0}.spinMonthlyDayMonthCount = {1};\n", localVarName, SpinMonthlyDayMonthCount.ClientID);
			stb.AppendFormat("{0}.spinMonthlyWeekDaysMonthCount = {1};\n", localVarName, SpinMonthlyWeekDaysMonthCount.ClientID);
			stb.AppendFormat("{0}.rbDay = {1};\n", localVarName, RbDay.ClientID);
			stb.AppendFormat("{0}.rbWeekDays = {1};\n", localVarName, RbWeekDays.ClientID);
			stb.AppendFormat("{0}.wdeMonthlyWeekDays = {1};\n", localVarName, WdeMonthlyWeekDays.ClientID);
			stb.AppendFormat("{0}.wmeMonthlyWeekOfMonth = {1};\n", localVarName, WmeMonthlyWeekOfMonth.ClientID);
		}
		#endregion
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region MonthlyRecurrenceValuesAccessor
	public class MonthlyRecurrenceValuesAccessor : DefaultRecurrenceRuleValuesAccessor {
		public MonthlyRecurrenceValuesAccessor(RecurrenceRuleControlBase recurrenceControl)
			: base(recurrenceControl) {
		}
		public new MonthlyRecurrenceControl RecurrenceControl { get { return (MonthlyRecurrenceControl)base.RecurrenceControl; } }
		public override int GetDayNumber() {
			return RecurrenceControl.ClientDayNumber;
		}
		public override int GetPeriodicity() {
			return RecurrenceControl.ClientPeriodicity;
		}
		public override WeekDays GetWeekDays() {
			return RecurrenceControl.ClientWeekDays;
		}
		public override WeekOfMonth GetWeekOfMonth() {
			return RecurrenceControl.ClientWeekOfMonth;
		}
	}
	#endregion
	#region YearlyRecurrenceControl
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "YearlyRecurrenceControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class YearlyRecurrenceControl : ClientSideSupportRecurrenceRuleControlBase {
		#region Fields
		RecurrenceControlLabel lblYearlyOf;
		ASPxSpinEdit spinYearlyDayNumber;
		ASPxRadioButton rbDay;
		ASPxRadioButton rbWeekOfMonth;
		MonthEdit meYearlyWeekDaysMonth;
		MonthEdit meYearlyDayMonth;
		WeekDaysEdit wdeYearlyWeekDays;
		WeekOfMonthEdit wmeYearlyWeekOfMonth;
		#endregion
		public YearlyRecurrenceControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected RecurrenceControlLabel LblYearlyOf { get { return lblYearlyOf; } }
		protected ASPxSpinEdit SpinYearlyDayNumber { get { return spinYearlyDayNumber; } }
		protected ASPxRadioButton RbDay { get { return rbDay; } }
		protected ASPxRadioButton RbWeekOfMonth { get { return rbWeekOfMonth; } }
		protected MonthEdit MeYearlyDayMonth { get { return meYearlyDayMonth; } }
		protected MonthEdit MeYearlyWeekDaysMonth { get { return meYearlyWeekDaysMonth; } }
		protected WeekDaysEdit WdeYearlyWeekDays { get { return wdeYearlyWeekDays; } }
		protected WeekOfMonthEdit WmeYearlyWeekOfMonth { get { return wmeYearlyWeekOfMonth; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("YearlyRecurrenceControlDayNumber"),
#endif
Category(SRCategoryNames.Data), DefaultValue(1)]
		public int DayNumber {
			get { return GetIntProperty(RecurrencePropertyNames.DayNumber, 1); }
			set { SetIntProperty(RecurrencePropertyNames.DayNumber, 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("YearlyRecurrenceControlMonth"),
#endif
Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Month {
			get { return GetIntProperty(RecurrencePropertyNames.Month, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Month, 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("YearlyRecurrenceControlWeekDays"),
#endif
Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("YearlyRecurrenceControlWeekOfMonth"),
#endif
Category(SRCategoryNames.Data), DefaultValue(WeekOfMonth.None)]
		public WeekOfMonth WeekOfMonth {
			get { return (WeekOfMonth)GetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None, value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("YearlyRecurrenceControlStart"),
#endif
Category(SRCategoryNames.Data)]
		public DateTime Start {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today, value); }
		}
		[Browsable(false)]
		public int ClientDayNumber { get { return CalculateClientDayNumber(); } }
		[Browsable(false)]
		public int ClientMonth { get { return CalculateClientMonth(); } }
		[Browsable(false)]
		public WeekDays ClientWeekDays { get { return CalculateClientWeekDays(); } }
		[Browsable(false)]
		public WeekOfMonth ClientWeekOfMonth { get { return CalculateClientWeekOfMonth(); } }
		#endregion
		#region CreateRuleValuesAccessor
		protected internal override RecurrenceRuleValuesAccessor CreateRuleValuesAccessor() {
			return new YearlyRecurrenceValuesAccessor(this);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.lblYearlyOf = new RecurrenceControlLabel();
			this.spinYearlyDayNumber = new ASPxSpinEdit();
			this.rbDay = new ASPxRadioButton();
			this.rbWeekOfMonth = new ASPxRadioButton();
			this.meYearlyDayMonth = new MonthEdit();
			this.meYearlyWeekDaysMonth = new MonthEdit();
			this.wdeYearlyWeekDays = new WeekDaysEdit();
			this.wmeYearlyWeekOfMonth = new WeekOfMonthEdit();
			RegisterChildControl(LblYearlyOf);
			RegisterChildControl(SpinYearlyDayNumber);
			RegisterChildControl(RbDay);
			RegisterChildControl(RbWeekOfMonth);
			RegisterChildControl(MeYearlyDayMonth);
			RegisterChildControl(MeYearlyWeekDaysMonth);
			RegisterChildControl(WdeYearlyWeekDays);
			RegisterChildControl(WmeYearlyWeekOfMonth);
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			SpinYearlyDayNumber.ID = "SpnDayNo";
			RbDay.ID = "RbDay";
			RbWeekOfMonth.ID = "RbWeekOfMnth";
			MeYearlyDayMonth.ID = "MeDayMnth";
			MeYearlyWeekDaysMonth.ID = "MeWeekDaysMnth";
			WdeYearlyWeekDays.ID = "WdeWeekDays";
			WmeYearlyWeekOfMonth.ID = "WmeWeekOfMnth";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo == null)
				return;
			EditorsInfo.Apply(SpinYearlyDayNumber);
			EditorsInfo.Apply(RbDay);
			EditorsInfo.Apply(RbWeekOfMonth);
			EditorsInfo.Apply(MeYearlyDayMonth);
			EditorsInfo.Apply(MeYearlyWeekDaysMonth);
			EditorsInfo.Apply(WdeYearlyWeekDays);
			EditorsInfo.Apply(WmeYearlyWeekOfMonth);
			EditorsInfo.Apply(LblYearlyOf.Label);
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table t1 = TableHelper.CreateSingleRowTable(new WebControl[] { MeYearlyDayMonth, SpinYearlyDayNumber });
			Table t2 = TableHelper.CreateSingleRowTable(new WebControl[] { WmeYearlyWeekOfMonth, WdeYearlyWeekDays, LblYearlyOf, MeYearlyWeekDaysMonth });
			Table resultTable = TableHelper.AddNewTableRow(RenderUtils.CreateTable(), new WebControl[] { RbDay, t1 });
			resultTable = TableHelper.AddNewTableRow(resultTable, new WebControl[] { RbWeekOfMonth, t2 });
			RenderUtils.ApplyCellPadding(t1, 3);
			RenderUtils.ApplyCellPadding(t2, 3);
			RenderUtils.ApplyCellPadding(resultTable, 2);
			parent.Controls.Add(resultTable);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			const string groupName = "rcYearlyGr";
			RbDay.EnableViewState = false;
			RbDay.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Every);
			RbDay.GroupName = groupName;
			RbWeekOfMonth.EnableViewState = false;
			RbWeekOfMonth.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_The);
			RbWeekOfMonth.GroupName = groupName;
			RbWeekOfMonth.Wrap = DefaultBoolean.False;
			LblYearlyOf.EnableViewState = false;
			LblYearlyOf.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Of);
			LblYearlyOf.AssociatedControlID = "MeWeekDaysMnth";
			SpinYearlyDayNumber.EnableViewState = false;
			SpinYearlyDayNumber.NumberType = SpinEditNumberType.Integer;
			SpinYearlyDayNumber.MinValue = 1;
			SpinYearlyDayNumber.Width = 60;
			SpinYearlyDayNumber.MaxValue = 31;
			MeYearlyDayMonth.EnableViewState = false;
			MeYearlyDayMonth.Width = 100;
			MeYearlyWeekDaysMonth.EnableViewState = false;
			MeYearlyWeekDaysMonth.Width = 100;
			WdeYearlyWeekDays.EnableViewState = false;
			WdeYearlyWeekDays.Width = 105;
			WmeYearlyWeekOfMonth.EnableViewState = false;
			WmeYearlyWeekOfMonth.Width = 75;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			SpinYearlyDayNumber.AllowNull = false;
			MeYearlyDayMonth.Month = Month;
			MeYearlyWeekDaysMonth.Month = Month;
			WdeYearlyWeekDays.DayOfWeek = WeekDays;
			RbDay.Checked = false;
			RbWeekOfMonth.Checked = false;
			if (WeekOfMonth == DevExpress.XtraScheduler.WeekOfMonth.None) {
				RbDay.Checked = true;
				SpinYearlyDayNumber.Number = DayNumber; 
				WmeYearlyWeekOfMonth.WeekOfMonth = CalcWeekOfMonth();
			} else {
				RbWeekOfMonth.Checked = true;
				SpinYearlyDayNumber.Number = DayNumber;
				WmeYearlyWeekOfMonth.WeekOfMonth = WeekOfMonth;
			}
		}
		#endregion
		#region CalcWeekOfMonth
		protected internal virtual WeekOfMonth CalcWeekOfMonth() {
			int count = 0;
			DateTime date = new DateTime(Start.Year, Start.Month, 1);
			DayOfWeek dayOfWeek = DateTimeHelper.ToDayOfWeek(WeekDays);
			while (date <= Start) {
				if (date.DayOfWeek == dayOfWeek)
					count++;
				date = date.AddDays(1);
			}
			if (count >= 1 && count <= 4)
				return (WeekOfMonth)count;
			else if (count >= 5)
				return WeekOfMonth.Last;
			else
				return WeekOfMonth.First;
		}
		#endregion
		#region CalculateClientDayNumber
		protected internal virtual int CalculateClientDayNumber() {
			return Convert.ToInt32(SpinYearlyDayNumber.Number);
		}
		#endregion
		#region CalculateClientMonth
		protected virtual int CalculateClientMonth() {
			return RbDay.Checked ? MeYearlyDayMonth.Month : MeYearlyWeekDaysMonth.Month;
		}
		#endregion
		#region CalculateClientWeekDays
		protected internal virtual WeekDays CalculateClientWeekDays() {
			return WdeYearlyWeekDays.DayOfWeek;
		}
		#endregion
		#region CalculateClientWeekOfMonth
		protected internal virtual WeekOfMonth CalculateClientWeekOfMonth() {
			return RbDay.Checked ? WeekOfMonth.None : WmeYearlyWeekOfMonth.WeekOfMonth;
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			if (RbWeekOfMonth.Checked)
				return;
			Validator.ValidateMonthDayNumber(args, SpinYearlyDayNumber, ClientDayNumber, ClientMonth);
		}
		#endregion
		#region GetClientObjectClassName
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientYearlyRecurrenceControl";
			return base.GetClientObjectClassName();
		}
		#endregion
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.spinYearlyDayNumber = {1};", localVarName, SpinYearlyDayNumber.ClientID);
			stb.AppendFormat("{0}.rbDay = {1};", localVarName, RbDay.ClientID);
			stb.AppendFormat("{0}.rbWeekOfMonth = {1};", localVarName, RbWeekOfMonth.ClientID);
			stb.AppendFormat("{0}.meYearlyDayMonth = {1};", localVarName, MeYearlyDayMonth.ClientID);
			stb.AppendFormat("{0}.meYearlyWeekDaysMonth = {1};", localVarName, MeYearlyWeekDaysMonth.ClientID);
			stb.AppendFormat("{0}.wdeYearlyWeekDays = {1};", localVarName, WdeYearlyWeekDays.ClientID);
			stb.AppendFormat("{0}.wmeYearlyWeekOfMonth = {1};", localVarName, WmeYearlyWeekOfMonth.ClientID);
		}
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region YearlyRecurrenceValuesAccessor
	public class YearlyRecurrenceValuesAccessor : DefaultRecurrenceRuleValuesAccessor {
		public YearlyRecurrenceValuesAccessor(RecurrenceRuleControlBase recurrenceControl)
			: base(recurrenceControl) {
		}
		public new YearlyRecurrenceControl RecurrenceControl { get { return (YearlyRecurrenceControl)base.RecurrenceControl; } }
		public override int GetDayNumber() {
			return RecurrenceControl.ClientDayNumber;
		}
		public override int GetMonth() {
			return RecurrenceControl.ClientMonth;
		}
		public override WeekDays GetWeekDays() {
			return RecurrenceControl.ClientWeekDays;
		}
		public override WeekOfMonth GetWeekOfMonth() {
			return RecurrenceControl.ClientWeekOfMonth;
		}
	}
	#endregion
	#region RecurrenceRangeControl
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "RecurrenceRangeControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class RecurrenceRangeControl : ASPxSchedulerRecurrenceControlBase {
		#region Fields
		RecurrenceControlLabel lblRangeOccurrencesCount;
		ASPxDateEdit deRangeEnd;
		ASPxSpinEdit spinRangeOccurrencesCount;
		ASPxRadioButton rbNoEndDate;
		ASPxRadioButton rbEndAfterNumberOfOccurrences;
		ASPxRadioButton rbEndByDate;
		bool enableScriptSupport = true;
		#endregion
		public RecurrenceRangeControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected RecurrenceControlLabel LblRangeOccurrencesCount { get { return lblRangeOccurrencesCount; } }
		protected ASPxDateEdit DeRangeEnd { get { return deRangeEnd; } }
		protected ASPxSpinEdit SpinRangeOccurrencesCount { get { return spinRangeOccurrencesCount; } }
		protected ASPxRadioButton RbNoEndDate { get { return rbNoEndDate; } }
		protected ASPxRadioButton RbEndAfterNumberOfOccurrences { get { return rbEndAfterNumberOfOccurrences; } }
		protected ASPxRadioButton RbEndByDate { get { return rbEndByDate; } }
		public DateTime Start {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today); ; }
			set { SetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(RecurrenceRange.NoEndDate)]
		public RecurrenceRange Range {
			get { return (RecurrenceRange)GetEnumProperty(RecurrencePropertyNames.RecurrenceRange, RecurrenceRange.NoEndDate); }
			set { SetEnumProperty(RecurrencePropertyNames.RecurrenceRange, RecurrenceRange.NoEndDate, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int OccurrenceCount {
			get { return GetIntProperty(RecurrencePropertyNames.OccurrenceCount, 1); }
			set { SetIntProperty(RecurrencePropertyNames.OccurrenceCount, 1, value); }
		}
		[Category(SRCategoryNames.Data)]
		public DateTime End {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.End, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.End, DateTime.Today, value); }
		}
		[Browsable(false)]
		public RecurrenceRange ClientRange { get { return CalculateClientRange(); } }
		[Browsable(false)]
		public int ClientOccurrenceCount { get { return CalculateClientOccurrenceCount(); } }
		[Browsable(false)]
		public DateTime ClientEnd { get { return CalculateClientEnd(); } }
		#region ClientInstanceName
		[Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		#endregion
		public bool EnableScriptSupport { get { return enableScriptSupport; } set { enableScriptSupport = value; } }
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.lblRangeOccurrencesCount = new RecurrenceControlLabel();
			this.deRangeEnd = new ASPxDateEdit();
			this.spinRangeOccurrencesCount = new ASPxSpinEdit();
			this.spinRangeOccurrencesCount.Init += OnSpinRangeOccurrencesCountInit;
			this.rbNoEndDate = new ASPxRadioButton();
			this.rbEndAfterNumberOfOccurrences = new ASPxRadioButton();
			this.rbEndByDate = new ASPxRadioButton();
		}
		#endregion
		#region OnSpinRangeOccurrencesCountInit
		void OnSpinRangeOccurrencesCountInit(object sender, EventArgs e) {
			this.spinRangeOccurrencesCount.Number = OccurrenceCount;
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			DeRangeEnd.ID = "DeEnd";
			SpinRangeOccurrencesCount.ID = "SpnOccCnt";
			RbNoEndDate.ID = "DeNoEnd";
			RbEndAfterNumberOfOccurrences.ID = "DeEndByOccNo";
			RbEndByDate.ID = "DeEndByDate";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo == null)
				return;
			EditorsInfo.Apply(DeRangeEnd);
			EditorsInfo.Apply(SpinRangeOccurrencesCount);
			EditorsInfo.Apply(RbNoEndDate);
			EditorsInfo.Apply(RbEndAfterNumberOfOccurrences);
			EditorsInfo.Apply(RbEndByDate);
			EditorsInfo.Apply(LblRangeOccurrencesCount.Label);
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table t1 = TableHelper.CreateSingleRowTable(new WebControl[] { });
			Table t2 = TableHelper.CreateSingleRowTable(new WebControl[] { SpinRangeOccurrencesCount, LblRangeOccurrencesCount });
			Table t3 = TableHelper.CreateSingleRowTable(new WebControl[] { DeRangeEnd }, new String[] { "dxscRCEndByDateCell" });
			Table resultTable = TableHelper.AddNewTableRow(RenderUtils.CreateTable(), new WebControl[] { RbNoEndDate, t1 });
			resultTable = TableHelper.AddNewTableRow(resultTable, new WebControl[] { RbEndAfterNumberOfOccurrences, t2 });
			resultTable = TableHelper.AddNewTableRow(resultTable, new WebControl[] { RbEndByDate, t3 });
			RenderUtils.ApplyCellPadding(t2, 3);
			RenderUtils.ApplyCellPadding(t3, 3);
			resultTable.CssClass = "dxscRangeControl";
			parent.Controls.Add(resultTable);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			const string groupName = "rcRangeGr";
			RbNoEndDate.EnableViewState = false;
			RbNoEndDate.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_RecurrenceRange_NoEndDate);
			RbNoEndDate.GroupName = groupName;
			RbNoEndDate.EncodeHtml = false;
			RbNoEndDate.Wrap = DefaultBoolean.False;
			RbEndAfterNumberOfOccurrences.EnableViewState = false;
			RbEndAfterNumberOfOccurrences.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_RecurrenceRange_OccurrenceCount);
			RbEndAfterNumberOfOccurrences.GroupName = groupName;
			RbEndAfterNumberOfOccurrences.EncodeHtml = false;
			RbEndAfterNumberOfOccurrences.Wrap = DefaultBoolean.False;
			RbEndByDate.EnableViewState = false;
			RbEndByDate.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_RecurrenceRange_EndBy);
			RbEndByDate.GroupName = groupName;
			RbEndByDate.EncodeHtml = false;
			RbEndByDate.Wrap = DefaultBoolean.False;
			LblRangeOccurrencesCount.EnableViewState = false;
			LblRangeOccurrencesCount.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Occurrences);
			SpinRangeOccurrencesCount.EnableViewState = false;
			SpinRangeOccurrencesCount.NumberType = SpinEditNumberType.Integer;
			SpinRangeOccurrencesCount.MinValue = 1;
			SpinRangeOccurrencesCount.Width = 60;
			SpinRangeOccurrencesCount.MaxValue = 999;
			SpinRangeOccurrencesCount.EnableClientSideAPI = EnableScriptSupport;
			SpinRangeOccurrencesCount.AllowNull = false;
			DeRangeEnd.EnableViewState = false;
			DeRangeEnd.EditFormat = EditFormat.Date;
			DeRangeEnd.Width = Unit.Percentage(100);
			DeRangeEnd.EnableClientSideAPI = EnableScriptSupport;
			DeRangeEnd.AllowNull = false;
			DeRangeEnd.ValidationSettings.Display = Display.Static;
			DeRangeEnd.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			DeRangeEnd.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
			DeRangeEnd.ValidationSettings.EnableCustomValidation = true;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			SpinRangeOccurrencesCount.Number = OccurrenceCount;
			DeRangeEnd.Date = End;
			ClearRange(); 
			switch (Range) {
				case RecurrenceRange.NoEndDate:
					RbNoEndDate.Checked = true;
					break;
				case RecurrenceRange.OccurrenceCount:
					RbEndAfterNumberOfOccurrences.Checked = true;
					break;
				case RecurrenceRange.EndByDate:
				default:
					RbEndByDate.Checked = true;
					break;
			}
		}
		#endregion
		#region ClearRange
		protected internal void ClearRange() {
			RbNoEndDate.Checked = false;
			RbEndAfterNumberOfOccurrences.Checked = false;
			RbEndByDate.Checked = false;
		}
		#endregion
		#region CalculateClientRange
		protected virtual RecurrenceRange CalculateClientRange() {
			if (RbNoEndDate.Checked)
				return RecurrenceRange.NoEndDate;
			else if (RbEndAfterNumberOfOccurrences.Checked)
				return RecurrenceRange.OccurrenceCount;
			else
				return RecurrenceRange.EndByDate;
		}
		#endregion
		#region CalculateClientOccurrenceCount
		protected virtual int CalculateClientOccurrenceCount() {
			return Convert.ToInt32(SpinRangeOccurrencesCount.Number);
		}
		#endregion
		#region CalculateClientEnd
		protected virtual DateTime CalculateClientEnd() {
			return DeRangeEnd.Date;
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			if (ClientRange == RecurrenceRange.OccurrenceCount) {
				Validator.ValidateOccurrencesCount(args, RbEndAfterNumberOfOccurrences, ClientOccurrenceCount);
			}
			if (ClientRange == RecurrenceRange.EndByDate)
				DeRangeEnd.IsValid = Validator.ValidateRecurrenceEndDate(args, DeRangeEnd, ClientEnd);
		}
		#endregion
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientRecurrenceRangeControl";
			return base.GetClientObjectClassName();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.start = {1};\n", localVarName, HtmlConvertor.ToScript(Start, typeof(DateTime)));
			stb.AppendFormat("{0}.rbNoEndDate = {1};\n", localVarName, RbNoEndDate.ClientID);
			stb.AppendFormat("{0}.rbEndAfterNumberOfOccurrences = {1};\n", localVarName, RbEndAfterNumberOfOccurrences.ClientID);
			stb.AppendFormat("{0}.rbEndByDate = {1};\n", localVarName, RbEndByDate.ClientID);
			stb.AppendFormat("{0}.spinRangeOccurrencesCount = {1};\n", localVarName, SpinRangeOccurrencesCount.ClientID);
			stb.AppendFormat("{0}.deRangeEnd = {1};\n", localVarName, DeRangeEnd.ClientID);
			stb.AppendFormat("{0}.SubscribeEvents();\n", localVarName);
		}
		public override void RegisterStyleSheets() {
		}
	}
	#endregion
	#region RecurrenceTypeEdit
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxScheduler.Design.RecurrenceControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "RecurrenceTypeEdit.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class RecurrenceTypeEdit : ASPxRadioButtonList {
		bool enableScriptSupport = true;
		bool enableHourlyRecurrence = false;
		Dictionary<RecurrenceType, ASPxSchedulerStringId> recurrenceItems;
		public RecurrenceTypeEdit() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			base.ValueType = typeof(Int32);
			Type = RecurrenceType.Daily;
			TextWrap = false;
			PopulateItems();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ListEditItemCollection Items { get { return base.Items; } }
		[DefaultValue(typeof(Int32)), TypeConverter(typeof(ValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public new Type ValueType { get { return base.ValueType; } set { base.ValueType = value; } }
		[Category(SRCategoryNames.Data), DefaultValue(RecurrenceType.Daily)]
		public RecurrenceType Type {
			get { return (RecurrenceType)Value; }
			set {
				RecurrenceType type = ASPxSchedulerRecurrenceControlBase.GetValidRecurrenceType(value, EnableHourlyRecurrence);
				Value = (int)type;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		public bool EnableScriptSupport { get { return enableScriptSupport; } set { enableScriptSupport = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EnableHourlyRecurrence {
			get {
				return enableHourlyRecurrence;
			}
			set {
				if (value == enableHourlyRecurrence)
					return;
				enableHourlyRecurrence = value;
				PopulateItems();
			}
		}
		protected Dictionary<RecurrenceType, ASPxSchedulerStringId> RecurrenceItems { get { return recurrenceItems; } }
		#endregion
		protected override string GetClientObjectClassName() {
			if (EnableScriptSupport)
				return "ASPxClientRecurrenceTypeEdit";
			return base.GetClientObjectClassName();
		}
		protected virtual void PopulateItems() {
			this.recurrenceItems = new Dictionary<RecurrenceType, ASPxSchedulerStringId>();
			if (EnableHourlyRecurrence)
				RecurrenceItems.Add(RecurrenceType.Hourly, ASPxSchedulerStringId.Caption_RecurrenceType_Hourly);
			RecurrenceItems.Add(RecurrenceType.Daily, ASPxSchedulerStringId.Caption_RecurrenceType_Daily);
			RecurrenceItems.Add(RecurrenceType.Weekly, ASPxSchedulerStringId.Caption_RecurrenceType_Weekly);
			RecurrenceItems.Add(RecurrenceType.Monthly, ASPxSchedulerStringId.Caption_RecurrenceType_Monthly);
			RecurrenceItems.Add(RecurrenceType.Yearly, ASPxSchedulerStringId.Caption_RecurrenceType_Yearly);
			Items.Clear();
			foreach (var item in RecurrenceItems)
				Items.Add(String.Empty, item.Key);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			foreach (ListEditItem item in Items) {
				ASPxSchedulerStringId stringId = RecurrenceItems[(RecurrenceType)item.Value];
				item.Text = ASPxSchedulerLocalizer.GetString(stringId);
			}
			this.ItemSpacing = Unit.Pixel(10);
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptClientAppointmentResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptRecurrenceTypeEditResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptRecurrenceControlsResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptViewInfosResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptAPIResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptGlobalFunctionsResourceName);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			if (EnableHourlyRecurrence)
				stb.AppendFormat("{0}.EnableHourlyRecurrence = true;\n", localVarName);
		}
	}
	#endregion
	#region AppointmentRecurrenceForm
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "AppointmentRecurrenceForm.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class AppointmentRecurrenceForm : ASPxSchedulerRecurrenceControlBase {
		#region Fields
		ASPxCheckBox chkRecurrence;
		AppointmentRecurrenceControl recurrenceControl;
		bool enableScriptSupport = true;
		bool enableHourlyRecurrence;
		#endregion
		public AppointmentRecurrenceForm() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		protected internal AppointmentRecurrenceControl RecurrenceControl { get { return recurrenceControl; } }
		protected ASPxCheckBox ChkRecurrence { get { return chkRecurrence; } }
		protected virtual string RecurrenceControlID { get { return "AptRecCtl"; } }
		[DefaultValue(true)]
		public bool EnableScriptSupport { get { return enableScriptSupport; } set { enableScriptSupport = value; } }
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Periodicity {
			get { return GetIntProperty(RecurrencePropertyNames.Periodicity, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Periodicity, 1, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (DevExpress.XtraScheduler.WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(WeekOfMonth.None)]
		public WeekOfMonth WeekOfMonth {
			get { return (WeekOfMonth)GetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int DayNumber {
			get { return GetIntProperty(RecurrencePropertyNames.DayNumber, 1); }
			set { SetIntProperty(RecurrencePropertyNames.DayNumber, 1, value); }
		}
		[Category(SRCategoryNames.Data)]
		public DateTime Start {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Month {
			get { return GetIntProperty(RecurrencePropertyNames.Month, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Month, 1, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int OccurrenceCount {
			get { return GetIntProperty(RecurrencePropertyNames.OccurrenceCount, 1); }
			set { SetIntProperty(RecurrencePropertyNames.OccurrenceCount, 1, value); }
		}
		[Category(SRCategoryNames.Data)]
		public DateTime End {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.End, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.End, DateTime.Today, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(RecurrenceRange.NoEndDate)]
		public RecurrenceRange RecurrenceRange {
			get { return (RecurrenceRange)GetEnumProperty(RecurrencePropertyNames.RecurrenceRange, RecurrenceRange.NoEndDate); }
			set { SetEnumProperty(RecurrencePropertyNames.RecurrenceRange, RecurrenceRange.NoEndDate, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(RecurrenceType.Daily)]
		public RecurrenceType RecurrenceType {
			get { return (RecurrenceType)GetEnumProperty(RecurrencePropertyNames.RecurrenceType, RecurrenceType.Daily); }
			set { SetEnumProperty(RecurrencePropertyNames.RecurrenceType, RecurrenceType.Daily, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(false)]
		public bool IsRecurring {
			get { return GetBoolProperty(RecurrencePropertyNames.IsRecurring, false); }
			set { SetBoolProperty(RecurrencePropertyNames.IsRecurring, false, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(false)]
		public bool IsFormRecreated { get { return !CanPrepareRecurrenceControls; } set { CanPrepareRecurrenceControls = !value; } }
		[Browsable(false)]
		public bool ClientIsRecurrence { get { return ChkRecurrence.Checked; } }
		[Browsable(false)]
		public int ClientDayNumber { get { return RecurrenceControl.ClientDayNumber; } }
		[Browsable(false)]
		public int ClientMonth { get { return RecurrenceControl.ClientMonth; } }
		[Browsable(false)]
		public int ClientPeriodicity { get { return RecurrenceControl.ClientPeriodicity; } }
		[Browsable(false)]
		public WeekDays ClientWeekDays { get { return RecurrenceControl.ClientWeekDays; } }
		[Browsable(false)]
		public WeekOfMonth ClientWeekOfMonth { get { return RecurrenceControl.ClientWeekOfMonth; } }
		[Browsable(false)]
		public RecurrenceType ClientRecurrenceType { get { return RecurrenceControl.ClientRecurrenceType; } }
		[Browsable(false)]
		public RecurrenceRange ClientRecurrenceRange { get { return RecurrenceControl.ClientRecurrenceRange; } }
		[Browsable(false)]
		public DateTime ClientEnd { get { return RecurrenceControl.EndValue; } }
		[Browsable(false)]
		public int ClientOccurrenceCount { get { return RecurrenceControl.OccurrenceCountValue; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableHourlyRecurrence {
			get {
				return enableHourlyRecurrence;
			}
			set {
				if (value == enableHourlyRecurrence)
					return;
				enableHourlyRecurrence = value;
				if (RecurrenceControl == null)
					return;
				RecurrenceControl.EnableHourlyRecurrence = value;
			}
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			RecurrenceControl.ValidateValues(args);
		}
		#endregion
		#region CheckForWarnings
		public override void CheckForWarnings(ValidationArgs args) {
			RecurrenceControl.CheckForWarnings(args);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.chkRecurrence = new ASPxCheckBox();
			this.recurrenceControl = new AppointmentRecurrenceControl();
			this.recurrenceControl.EnableHourlyRecurrence = EnableHourlyRecurrence;
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			ChkRecurrence.ID = "ChkRecurrence";
			RecurrenceControl.ID = RecurrenceControlID;
			RecurrenceControl.ParentSkinOwner = this;
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo != null) {
				EditorsInfo.Apply(ChkRecurrence);
			}
			RecurrenceControl.EditorsInfo = EditorsInfo;
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table table = TableHelper.AddNewTableRow(RenderUtils.CreateTable(), new WebControl[] { ChkRecurrence });
			table = TableHelper.AddNewTableRow(table, new WebControl[] { RecurrenceControl });
			parent.Controls.Add(table);
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			RecurrenceControl.CanPrepareRecurrenceControls = !IsFormRecreated;
			RecurrenceControl.EnableViewState = false;
			ChkRecurrence.EnableViewState = false;
			ChkRecurrence.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Recurrence);
			if (ChkRecurrence != null) {
				if (ChkRecurrence.Checked)
					SchedulerWebStyleHelper.SetDisplayBlock(RecurrenceControl.MainDiv);
				else
					SchedulerWebStyleHelper.SetDisplayNone(RecurrenceControl.MainDiv);
			}
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			if (IsRecurring)
				SchedulerWebStyleHelper.SetDisplayBlock(RecurrenceControl.MainDiv);
			else
				SchedulerWebStyleHelper.SetDisplayNone(RecurrenceControl.MainDiv);
			RecurrenceControl.DayNumber = DayNumber;
			RecurrenceControl.End = End;
			RecurrenceControl.Month = Month;
			RecurrenceControl.OccurrenceCount = OccurrenceCount;
			RecurrenceControl.Periodicity = Periodicity;
			RecurrenceControl.RecurrenceRange = RecurrenceRange;
			RecurrenceControl.Start = Start;
			RecurrenceControl.WeekDays = WeekDays;
			RecurrenceControl.WeekOfMonth = WeekOfMonth;
			RecurrenceControl.RecurrenceType = RecurrenceType;
			ChkRecurrence.Checked = IsRecurring;
		}
		#endregion
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		#endregion
		public override void RegisterStyleSheets() {
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientAppointmentRecurrenceForm";
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.RecurrenceControl = {1};", localVarName, RecurrenceControl.ClientID);
			stb.AppendFormat("{0}.ChkRecurrence = {1};", localVarName, ChkRecurrence.ClientID);
		}
	}
	#endregion
	#region AppointmentRecurrenceControl
	[DXWebToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "AppointmentRecurrenceControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling)
]
	public class AppointmentRecurrenceControl : RecurrenceRuleControlBase {
		#region Fields
		RecurrenceTypeEdit recurrenceTypeEdit;
		RecurrenceRangeControl recurrenceRangeControl;
		HourlyRecurrenceControl hourlyRecurrenceControl;
		DailyRecurrenceControl dailyRecurrenceControl;
		WeeklyRecurrenceControl weeklyRecurrenceControl;
		MonthlyRecurrenceControl monthlyRecurrenceControl;
		YearlyRecurrenceControl yearlyRecurrenceControl;
		bool enableScriptSupport = true;
		bool enableHourlyRecurrence = false;
		#endregion
		public AppointmentRecurrenceControl() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		#region Properties
		[Category("Client-Side"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppointmentRecurrenceControlClientSideEvents ClientSideEvents { get { return (AppointmentRecurrenceControlClientSideEvents)base.ClientSideEventsInternal; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxSchedulerImages Images { get { return (ASPxSchedulerImages)ImagesInternal; } }
		protected HourlyRecurrenceControl HourlyRecurrenceControl { get { return hourlyRecurrenceControl; } }
		protected DailyRecurrenceControl DailyRecurrenceControl { get { return dailyRecurrenceControl; } }
		protected WeeklyRecurrenceControl WeeklyRecurrenceControl { get { return weeklyRecurrenceControl; } }
		protected MonthlyRecurrenceControl MonthlyRecurrenceControl { get { return monthlyRecurrenceControl; } }
		protected YearlyRecurrenceControl YearlyRecurrenceControl { get { return yearlyRecurrenceControl; } }
		protected RecurrenceTypeEdit RecurrenceTypeEdit { get { return recurrenceTypeEdit; } }
		protected RecurrenceRangeControl RecurrenceRangeControl { get { return recurrenceRangeControl; } }
		[DefaultValue(true)]
		public bool EnableScriptSupport { get { return enableScriptSupport; } set { enableScriptSupport = value; } }
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Periodicity {
			get { return GetIntProperty(RecurrencePropertyNames.Periodicity, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Periodicity, 1, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(WeekDays.EveryDay)]
		public WeekDays WeekDays {
			get { return (DevExpress.XtraScheduler.WeekDays)GetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekDays, WeekDays.EveryDay, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(WeekOfMonth.None)]
		public WeekOfMonth WeekOfMonth {
			get { return (WeekOfMonth)GetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None); }
			set { SetEnumProperty(RecurrencePropertyNames.WeekOfMonth, WeekOfMonth.None, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int DayNumber {
			get { return GetIntProperty(RecurrencePropertyNames.DayNumber, 1); }
			set { SetIntProperty(RecurrencePropertyNames.DayNumber, 1, value); }
		}
		[Category(SRCategoryNames.Data)]
		public DateTime Start {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.Start, DateTime.Today, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int Month {
			get { return GetIntProperty(RecurrencePropertyNames.Month, 1); }
			set { SetIntProperty(RecurrencePropertyNames.Month, 1, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(1)]
		public int OccurrenceCount {
			get { return GetIntProperty(RecurrencePropertyNames.OccurrenceCount, 1); }
			set { SetIntProperty(RecurrencePropertyNames.OccurrenceCount, 1, value); }
		}
		[Category(SRCategoryNames.Data)]
		public DateTime End {
			get { return (DateTime)GetObjectProperty(RecurrencePropertyNames.End, DateTime.Today); }
			set { SetObjectProperty(RecurrencePropertyNames.End, DateTime.Today, value); }
		}
		[Browsable(false)]
		public DateTime ClientEnd { get { return EndValue; } }
		[Category(SRCategoryNames.Data), DefaultValue(RecurrenceRange.NoEndDate)]
		public RecurrenceRange RecurrenceRange {
			get { return (RecurrenceRange)GetEnumProperty(RecurrencePropertyNames.RecurrenceRange, RecurrenceRange.NoEndDate); }
			set { SetEnumProperty(RecurrencePropertyNames.RecurrenceRange, RecurrenceRange.NoEndDate, value); }
		}
		[Category(SRCategoryNames.Data), DefaultValue(RecurrenceType.Daily)]
		public RecurrenceType RecurrenceType {
			get { return (RecurrenceType)GetEnumProperty(RecurrencePropertyNames.RecurrenceType, RecurrenceType.Daily); }
			set { SetEnumProperty(RecurrencePropertyNames.RecurrenceType, RecurrenceType.Daily, value); }
		}
		[Browsable(false)]
		public int ClientDayNumber { get { return ValuesAccessor.GetDayNumber(); } }
		[Browsable(false)]
		public int ClientMonth { get { return ValuesAccessor.GetMonth(); } }
		[Browsable(false)]
		public int ClientPeriodicity { get { return ValuesAccessor.GetPeriodicity(); } }
		[Browsable(false)]
		public WeekDays ClientWeekDays { get { return ValuesAccessor.GetWeekDays(); } }
		[Browsable(false)]
		public WeekOfMonth ClientWeekOfMonth { get { return ValuesAccessor.GetWeekOfMonth(); } }
		[Browsable(false)]
		public RecurrenceType ClientRecurrenceType { get { return RecurrenceTypeEdit.Type; } }
		[Browsable(false)]
		public RecurrenceRange ClientRecurrenceRange { get { return RecurrenceRangeControl.ClientRange; } }
		[Browsable(false)]
		public DateTime EndValue { get { return RecurrenceRangeControl.ClientEnd; } }
		[Browsable(false)]
		public int OccurrenceCountValue { get { return RecurrenceRangeControl.ClientOccurrenceCount; } }
		[Browsable(false)]
		public int ClientOccurrenceCount { get { return OccurrenceCountValue; } }
		[Browsable(false)]
		public bool IsFormRecreated { get { return !CanPrepareRecurrenceControls; } set { CanPrepareRecurrenceControls = !value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableHourlyRecurrence {
			get {
				return enableHourlyRecurrence;
			}
			set {
				if (value == enableHourlyRecurrence)
					return;
				enableHourlyRecurrence = value;
				if (RecurrenceTypeEdit == null)
					return;
				RecurrenceTypeEdit.EnableHourlyRecurrence = value;
			}
		}
		#endregion
		#region CreateRuleValuesAccessor
		protected internal override RecurrenceRuleValuesAccessor CreateRuleValuesAccessor() {
			RecurrenceRuleControlBase control = GetCurrentRecurrenceRuleControl();
			RecurrenceRuleValuesAccessor result = control.CreateRuleValuesAccessor();
			return result != null ? result : new DefaultRecurrenceRuleValuesAccessor(this);
		}
		#endregion
		#region GetCurrentRecurrenceRuleControl
		protected RecurrenceRuleControlBase GetCurrentRecurrenceRuleControl() {
			RecurrenceType type = ClientRecurrenceType;
			switch (type) {
				case RecurrenceType.Weekly:
					return WeeklyRecurrenceControl;
				case RecurrenceType.Monthly:
					return MonthlyRecurrenceControl;
				case RecurrenceType.Yearly:
					return YearlyRecurrenceControl;
				case RecurrenceType.Hourly:
					return HourlyRecurrenceControl;
				case RecurrenceType.Daily:
				default:
					return DailyRecurrenceControl;
			}
		}
		#endregion
		#region ValidateValues
		public override void ValidateValues(ValidationArgs args) {
			RecurrenceRuleControlBase control = GetCurrentRecurrenceRuleControl();
			if (control != null)
				control.ValidateValues(args);
			if (args.Valid)
				RecurrenceRangeControl.ValidateValues(args);
		}
		#endregion
		#region CheckForWarnings
		public override void CheckForWarnings(ValidationArgs args) {
			RecurrenceRuleControlBase control = GetCurrentRecurrenceRuleControl();
			if (control != null)
				control.CheckForWarnings(args);
		}
		#endregion
		#region CreateChildWebControls
		protected override void CreateChildWebControls() {
			this.recurrenceTypeEdit = new RecurrenceTypeEdit();
			RecurrenceTypeEdit.EnableHourlyRecurrence = EnableHourlyRecurrence;
			this.hourlyRecurrenceControl = CreateHourlyRecurrenceControl();
			this.dailyRecurrenceControl = CreateDailyRecurrenceControl();
			this.weeklyRecurrenceControl = CreateWeeklyRecurrenceControl();
			this.monthlyRecurrenceControl = CreateMonthlyRecurrenceControl();
			this.yearlyRecurrenceControl = CreateYearlyRecurrenceControl();
			this.recurrenceRangeControl = CreateRecurrenceRangeControl();
			HourlyRecurrenceControl.EnableScriptSupport = EnableScriptSupport;
			DailyRecurrenceControl.EnableScriptSupport = EnableScriptSupport;
			weeklyRecurrenceControl.EnableScriptSupport = EnableScriptSupport;
			monthlyRecurrenceControl.EnableScriptSupport = EnableScriptSupport;
			yearlyRecurrenceControl.EnableScriptSupport = EnableScriptSupport;
			recurrenceTypeEdit.EnableScriptSupport = EnableScriptSupport;
			recurrenceRangeControl.EnableScriptSupport = EnableScriptSupport;
			SetValidator(this.recurrenceRangeControl);
		}
		void SetValidator(ISupportRecurrenceValidator controlWithValidator) {
			if (controlWithValidator == null)
				return;
			controlWithValidator.Validator = Validator;
		}
		protected virtual HourlyRecurrenceControl CreateHourlyRecurrenceControl() {
			return new HourlyRecurrenceControl();
		}
		protected virtual DailyRecurrenceControl CreateDailyRecurrenceControl() {
			return new DailyRecurrenceControl();
		}
		protected virtual WeeklyRecurrenceControl CreateWeeklyRecurrenceControl() {
			return new WeeklyRecurrenceControl();
		}
		protected virtual RecurrenceRangeControl CreateRecurrenceRangeControl() {
			return new RecurrenceRangeControl();
		}
		protected virtual MonthlyRecurrenceControl CreateMonthlyRecurrenceControl() {
			return new MonthlyRecurrenceControl();
		}
		protected virtual YearlyRecurrenceControl CreateYearlyRecurrenceControl() {
			return new YearlyRecurrenceControl();
		}
		#endregion
		#region AssignChildWebControlsIDs
		protected override void AssignChildWebControlsIDs() {
			HourlyRecurrenceControl.ID = "Hourly";
			DailyRecurrenceControl.ID = "Daily";
			WeeklyRecurrenceControl.ID = "Weekly";
			MonthlyRecurrenceControl.ID = "Monthly";
			YearlyRecurrenceControl.ID = "Yearly";
			RecurrenceTypeEdit.ID = "TypeEdt";
			RecurrenceRangeControl.ID = "RangeCtl";
		}
		#endregion
		#region AssignChildWebControlEditorsInfo
		protected override void AssignChildWebControlEditorsInfo() {
			if (EditorsInfo == null) {
				EditorsInfo = new EditorsInfo(this, new EditorStyles(this), new EditorImages(this), new ButtonControlStyles(this));
			}
			HourlyRecurrenceControl.EditorsInfo = EditorsInfo;
			DailyRecurrenceControl.EditorsInfo = EditorsInfo;
			WeeklyRecurrenceControl.EditorsInfo = EditorsInfo;
			MonthlyRecurrenceControl.EditorsInfo = EditorsInfo;
			YearlyRecurrenceControl.EditorsInfo = EditorsInfo;
			RecurrenceRangeControl.EditorsInfo = EditorsInfo;
			if (EditorsInfo != null) {
				EditorsInfo.Apply(RecurrenceTypeEdit);
			}
		}
		#endregion
		#region LayoutChildWebControls
		protected override void LayoutChildWebControls(WebControl parent) {
			Table table1 = TableHelper.CreateSingleRowTable(new WebControl[] { RecurrenceTypeEdit, null });
			Table table2 = TableHelper.CreateSingleRowTable(new WebControl[] { RecurrenceRangeControl });
			TableCell containerCell = table1.Rows[0].Cells[1]; 
			if (CanDisplayRecurrenceControl(RecurrenceType.Hourly))
				containerCell.Controls.Add(HourlyRecurrenceControl);
			if (CanDisplayRecurrenceControl(RecurrenceType.Daily))
				containerCell.Controls.Add(DailyRecurrenceControl);
			if (CanDisplayRecurrenceControl(RecurrenceType.Weekly))
				containerCell.Controls.Add(WeeklyRecurrenceControl);
			if (CanDisplayRecurrenceControl(RecurrenceType.Monthly))
				containerCell.Controls.Add(MonthlyRecurrenceControl);
			if (CanDisplayRecurrenceControl(RecurrenceType.Yearly))
				containerCell.Controls.Add(YearlyRecurrenceControl);
			parent.Controls.Add(table1);
			parent.Controls.Add(table2);
		}
		#endregion
		#region CanDisplayRecurrenceControl
		protected bool CanDisplayRecurrenceControl(RecurrenceType type) {
			return DesignMode ? GetValidRecurrenceType(RecurrenceType, EnableHourlyRecurrence) == type : true;
		}
		#endregion
		#region PrepareCommonPropertiesChildWebControls
		protected override void PrepareCommonPropertiesChildWebControls() {
			RecurrenceTypeEdit.EnableViewState = false;
			RecurrenceType reccurenceType = GetValidRecurrenceType(RecurrenceType, EnableHourlyRecurrence);
			if (!CanPrepareRecurrenceControls)
				reccurenceType = (RecurrenceType)RecurrenceTypeEdit.Value;
			if (!DesignMode) {
				UpdateRecurrenceControlsVisibility(reccurenceType);
				string[] ids = GetRecurrenceControlsIDs();
				RecurrenceTypeEdit.ClientSideEvents.SelectedIndexChanged = SetRecurrenceControlsVisibilityFunction(ids);
			}
			RecurrenceTypeEdit.Border.BorderStyle = BorderStyle.None;
			HourlyRecurrenceControl.CanPrepareRecurrenceControls = CanPrepareRecurrenceControls;
			DailyRecurrenceControl.CanPrepareRecurrenceControls = CanPrepareRecurrenceControls;
			WeeklyRecurrenceControl.CanPrepareRecurrenceControls = CanPrepareRecurrenceControls;
			MonthlyRecurrenceControl.CanPrepareRecurrenceControls = CanPrepareRecurrenceControls;
			YearlyRecurrenceControl.CanPrepareRecurrenceControls = CanPrepareRecurrenceControls;
			RecurrenceRangeControl.CanPrepareRecurrenceControls = CanPrepareRecurrenceControls;
		}
		#endregion
		#region PrepareRecurrencePropertiesChildWebControls
		protected override void PrepareRecurrencePropertiesChildWebControls() {
			UpdateRecurrenceControlsValues();
		}
		#endregion
		#region UpdateRecurrenceControlsVisibility
		protected virtual void UpdateRecurrenceControlsVisibility(RecurrenceType type) {
			SchedulerWebStyleHelper.SetDisplayAttribute(HourlyRecurrenceControl, type == RecurrenceType.Hourly);
			SchedulerWebStyleHelper.SetDisplayAttribute(HourlyRecurrenceControl.MainDiv, type == RecurrenceType.Hourly);
			SchedulerWebStyleHelper.SetDisplayAttribute(DailyRecurrenceControl, type == RecurrenceType.Daily);
			SchedulerWebStyleHelper.SetDisplayAttribute(DailyRecurrenceControl.MainDiv, type == RecurrenceType.Daily);
			SchedulerWebStyleHelper.SetDisplayAttribute(WeeklyRecurrenceControl, type == RecurrenceType.Weekly);
			SchedulerWebStyleHelper.SetDisplayAttribute(WeeklyRecurrenceControl.MainDiv, type == RecurrenceType.Weekly);
			SchedulerWebStyleHelper.SetDisplayAttribute(MonthlyRecurrenceControl, type == RecurrenceType.Monthly);
			SchedulerWebStyleHelper.SetDisplayAttribute(MonthlyRecurrenceControl.MainDiv, type == RecurrenceType.Monthly);
			SchedulerWebStyleHelper.SetDisplayAttribute(YearlyRecurrenceControl, type == RecurrenceType.Yearly);
			SchedulerWebStyleHelper.SetDisplayAttribute(YearlyRecurrenceControl.MainDiv, type == RecurrenceType.Yearly);
		}
		#endregion
		#region SetRecurrenceControlsVisibilityFunction
		protected string SetRecurrenceControlsVisibilityFunction(string[] recControlsIds) {
			string s = HtmlConvertor.ToJSON(recControlsIds);
			string compensationString = EnableHourlyRecurrence ? String.Empty : "+1";
			return string.Format("function(s, e) {{ ASPx.SchedulerSetRecurrenceControlsVisibility({0}, '{1}', s.GetSelectedIndex(){2}); }}", s, MainDiv.ClientID, compensationString);
		}
		#endregion
		#region GetRecurrenceControlsIDs
		protected virtual string[] GetRecurrenceControlsIDs() {
			return new string[] { 
				ASPxSchedulerRecurrenceControlBase.GetChildControlClientID(ClientID, HourlyRecurrenceControl), 
				ASPxSchedulerRecurrenceControlBase.GetChildControlClientID(ClientID, DailyRecurrenceControl), 
				ASPxSchedulerRecurrenceControlBase.GetChildControlClientID(ClientID, WeeklyRecurrenceControl),
				ASPxSchedulerRecurrenceControlBase.GetChildControlClientID(ClientID, MonthlyRecurrenceControl), 
				ASPxSchedulerRecurrenceControlBase.GetChildControlClientID(ClientID, YearlyRecurrenceControl) 
			};
		}
		#endregion
		#region UpdateRecurrenceControlsValues
		protected virtual void UpdateRecurrenceControlsValues() {
			RecurrenceType reccurenceType = GetValidRecurrenceType(RecurrenceType, EnableHourlyRecurrence);
			RecurrenceTypeEdit.Type = reccurenceType;
			HourlyRecurrenceControl.Periodicity = Periodicity;
			DailyRecurrenceControl.Periodicity = Periodicity;
			DailyRecurrenceControl.WeekDays = WeekDays;
			WeeklyRecurrenceControl.Periodicity = Periodicity;
			WeeklyRecurrenceControl.WeekDays = CalcRecurrenceControlWeekDaysValue(RecurrenceType.Weekly); ;
			MonthlyRecurrenceControl.Periodicity = Periodicity;
			MonthlyRecurrenceControl.WeekDays = CalcRecurrenceControlWeekDaysValue(RecurrenceType.Monthly);
			MonthlyRecurrenceControl.WeekOfMonth = WeekOfMonth;
			MonthlyRecurrenceControl.DayNumber = DayNumber;
			MonthlyRecurrenceControl.Start = Start;
			YearlyRecurrenceControl.WeekDays = CalcRecurrenceControlWeekDaysValue(RecurrenceType.Yearly);
			YearlyRecurrenceControl.WeekOfMonth = WeekOfMonth;
			YearlyRecurrenceControl.DayNumber = DayNumber;
			YearlyRecurrenceControl.Start = Start;
			YearlyRecurrenceControl.Month = Month;
			RecurrenceRangeControl.Start = Start;
			RecurrenceRangeControl.Range = RecurrenceRange;
			RecurrenceRangeControl.OccurrenceCount = OccurrenceCount;
			RecurrenceRangeControl.End = End;
		}
		#endregion
		#region CalcRecurrenceControlWeekDaysValue
		protected WeekDays CalcRecurrenceControlWeekDaysValue(RecurrenceType controlType) {
			return (RecurrenceType == controlType) ? WeekDays : GetValidWeekDays(controlType, Start.DayOfWeek);
		}
		#endregion
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		#endregion
		protected override string GetClientObjectClassName() {
			return "ASPxClientAppointmentRecurrenceControl";
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!EnableScriptSupport)
				return;
			stb.AppendFormat("{0}.DailyRecurrenceControl = {1};", localVarName, DailyRecurrenceControl.ClientID);
			stb.AppendFormat("{0}.WeeklyRecurrenceControl = {1};", localVarName, WeeklyRecurrenceControl.ClientID);
			stb.AppendFormat("{0}.MonthlyRecurrenceControl = {1};", localVarName, MonthlyRecurrenceControl.ClientID);
			stb.AppendFormat("{0}.YearlyRecurrenceControl = {1};", localVarName, YearlyRecurrenceControl.ClientID);
			stb.AppendFormat("{0}.RecurrenceRangeControl = {1};", localVarName, RecurrenceRangeControl.ClientID);
		}
		public override void RegisterStyleSheets() {
		}
		protected override ImagesBase CreateImages() {
			return new ASPxSchedulerImages(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new AppointmentRecurrenceControlClientSideEvents();
		}
	}
	#endregion
	#region RecurrenceRuleControlBase
	public abstract class RecurrenceRuleControlBase : ASPxSchedulerRecurrenceControlBase {
		RecurrenceRuleValuesAccessor valuesAccessor;
		public RecurrenceRuleValuesAccessor ValuesAccessor {
			get {
				if (valuesAccessor == null)
					valuesAccessor = CreateRuleValuesAccessor();
				return valuesAccessor;
			}
		}
		protected internal abstract RecurrenceRuleValuesAccessor CreateRuleValuesAccessor();
	}
	#endregion
	#region RecurrenceRuleValuesAccessor
	public abstract class RecurrenceRuleValuesAccessor {
		readonly RecurrenceRuleControlBase recurrenceControl;
		protected RecurrenceRuleValuesAccessor(RecurrenceRuleControlBase recurrenceControl) {
			if (recurrenceControl == null)
				Exceptions.ThrowArgumentNullException("recurrenceControl");
			this.recurrenceControl = recurrenceControl;
		}
		public RecurrenceRuleControlBase RecurrenceControl { get { return recurrenceControl; } }
		public abstract int GetDayNumber();
		public abstract int GetMonth();
		public abstract int GetPeriodicity();
		public abstract WeekDays GetWeekDays();
		public abstract WeekOfMonth GetWeekOfMonth();
	}
	#endregion
	#region DefaultRecurrenceRuleValuesAccessor
	public class DefaultRecurrenceRuleValuesAccessor : RecurrenceRuleValuesAccessor {
		public DefaultRecurrenceRuleValuesAccessor(RecurrenceRuleControlBase recurrenceControl)
			: base(recurrenceControl) {
		}
		public override int GetPeriodicity() {
			return RecurrenceInfo.DefaultPeriodicity;
		}
		public override int GetDayNumber() {
			return 1;
		}
		public override int GetMonth() {
			return 1;
		}
		public override WeekDays GetWeekDays() {
			return RecurrenceInfo.DefaultWeekDays;
		}
		public override WeekOfMonth GetWeekOfMonth() {
			return RecurrenceInfo.DefaultWeekOfMonth;
		}
	}
	#endregion
	#region RecurrenceControlLabel
	public class RecurrenceControlLabel : ASPxInternalWebControl {
		ASPxLabel label;
		public String Text { get { return Label.Text; } set { Label.Text = value; } }
		public String AssociatedControlID { get { return Label.AssociatedControlID; } set { Label.AssociatedControlID = value; } }
		public ASPxLabel Label { get { return label; } }
		public RecurrenceControlLabel() {
			this.label = new ASPxLabel();
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.Controls.Add(label);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetStyleStringAttribute(Label, "white-space", "nowrap");
			Label.EncodeHtml = false;
			Label.Wrap = DefaultBoolean.False;
			Label.ControlStyle.MergeWith(ControlStyle);
		}
	}
	#endregion
	public class AppointmentRecurrenceControlClientSideEvents : ClientSideEvents {
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ValidationCompleted {
			get { return GetEventHandler("ValidationCompleted"); }
			set { SetEventHandler("ValidationCompleted", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ValidationCompleted");
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region EditorsInfo
	public class EditorsInfo {
		#region Fields
		ISkinOwner skinOwner;
		EditorStyles editorStyles;
		EditorImages images;
		ButtonControlStyles buttonStyles;
		#endregion
		public EditorsInfo(ISkinOwner skinOwner, EditorStyles editorStyles, EditorImages images, ButtonControlStyles buttonStyles) {
			this.skinOwner = skinOwner;
			this.editorStyles = editorStyles;
			this.images = images;
			this.buttonStyles = buttonStyles;
		}
		#region Properties
		public ISkinOwner SkinOwner { get { return skinOwner; } }
		public EditorStyles EditorStyles { get { return editorStyles; } }
		public EditorImages Images { get { return images; } }
		public ButtonControlStyles ButtonStyles { get { return buttonStyles; } }
		#endregion
		public void Apply(ASPxEditBase edit) {
			edit.ParentSkinOwner = SkinOwner;
			edit.ParentStyles = EditorStyles;
			edit.ParentImages = Images;
		}
		public void Apply(ASPxButton button) {
			button.ParentSkinOwner = SkinOwner;
			button.ParentStyles = ButtonStyles;
		}
	}
	#endregion
	#region IEditorsInfoOwner
	public interface IEditorsInfoOwner {
		EditorsInfo EditorsInfo { get; set; }
	}
	#endregion
	public class ASPxSchedulerRecurrenceValidator : SchedulerRecurrenceValidator {
		DateTime? start;
		public DateTime? Start {
			get { return start; }
			set { start = value; }
		}
		public bool ValidateRecurrenceEndDate(ValidationArgs args, object control, DateTime clientEnd) {
			if (start == null)
				return true;
			args.Valid = clientEnd > Start.Value;
			return args.Valid;
		}
	}
	public interface ISupportRecurrenceValidator {
		ASPxSchedulerRecurrenceValidator Validator { get; set; }
	}
}
