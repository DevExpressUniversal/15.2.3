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
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using DevExpress.Data.Filtering.Helpers;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Helpers;
using System.Globalization;
using DevExpress.Utils;
namespace DevExpress.XtraEditors.Helpers {
	public interface IPopupOutlookDateFilterOwner {
		void OnCheckedChanged();
		void OnDateModified();
		void RaiseFilterListUpdate(List<FilterDateElement> list);
	}
	public class PopupOutlookDateFilterControl : OutlookDateFilterControl {
		IPopupOutlookDateFilterOwner owner;
		PopupContainerEdit ownerEdit;
		public PopupOutlookDateFilterControl(IPopupOutlookDateFilterOwner owner) {
			this.owner = owner;
			this.ownerEdit = null;
		}
		public PopupContainerEdit OwnerEdit { get { return ownerEdit; } set { ownerEdit = value; } }
		protected override void OnCancelPopup() {
			base.OnCancelPopup();
			if(OwnerEdit != null)
				OwnerEdit.CancelPopup();
		}
		protected override void OnApplyFilter(DateFilterResult filter) {
			base.OnApplyFilter(filter);
			if(OwnerEdit != null)
				OwnerEdit.ClosePopup(PopupCloseMode.ButtonClick);
		}
		protected override void OnDateModified() {
			base.OnDateModified();
			if(owner != null) owner.OnDateModified();
		}
		protected override void OnCheckedChanged() {
			base.OnCheckedChanged();
			if(owner != null) owner.OnCheckedChanged();
		}
		protected override void RaisePrepareList(List<FilterDateElement> list) {
			if (owner != null) owner.RaiseFilterListUpdate(list);
		}
	}
	[ToolboxItem(false)]
	public class OutlookDateFilterControl : XtraScrollableControl {
		private static readonly object requestClose = new object();
		UserLookAndFeel lookAndFeel;
		int maxLabelWidth = 0;
		List<Control> labels = new List<Control>();
		List<CheckEdit> checks = new List<CheckEdit>();
		DateControlEx dateControl;
		DateFilterInfoCache cache;
		DateFilterResult result = null;
		FilterColumn field = null;
		bool showEmptyFilter = false;
		string fieldName = string.Empty, fieldCaption = string.Empty;
		public OutlookDateFilterControl() {
			this.lookAndFeel = UserLookAndFeel.Default;
		}
		public DateFilterInfoCache Cache { get { return cache; } set { cache = value; } }
		public DateFilterResult Result { get { return result; } }
		public FilterColumn Field { get { return field; } set { field = value; } }
		public string FieldName { get { return Field.FieldName; } }
		public string FieldCaption { get { return Field.ColumnCaption; } }
		public UserLookAndFeel ElementsLookAndFeel { get { return lookAndFeel; } set { lookAndFeel = value; } }
		protected int IndentText { get { return 2; } }
		protected int IndentControls { get { return 0; } }
		protected int IndentDateControlY { get { return 5; } }
		protected int IndentDateControlX { get { return 20; } }
		public virtual bool ShowEmptyFilter { get { return showEmptyFilter; } set { showEmptyFilter = value; } }
		protected int FilterBySpecificIndex { get { return ShowEmptyFilter ? 2 : 1; } }
		DateFilterResult prevFilter = null;
		protected DateFilterResult PrevFilter { get { return prevFilter; } }
		bool useAltFilter = false;
		List<CriteriaOperator> currentNonProcessedFilterList = new List<CriteriaOperator>();
		public void Init(DateFilterResult prevFilter, CriteriaOperator currentFilter) {
			if(prevFilter != null) {
				if(!ReferenceEquals(prevFilter.FilterCriteria, null) &&
							!ReferenceEquals(currentFilter, null) &&
							 prevFilter.FilterCriteria.ToString() == currentFilter.ToString()) {
					this.prevFilter = prevFilter;
					return;
				}
			}
			this.prevFilter = FilterDateTypeHelper.FromCriteria(currentFilter, FieldName);
		}
		public void CreateControls() {
			this.lockEvents++;
			Controls.Clear();
			CreateControlsCore();
			this.lockEvents--;
		}
		protected virtual List<FilterDateElement> PrepareList() {
			List<FilterDateElement> res = new List<FilterDateElement>();
			FilterDateType[] ftypes = GetFilterDateTypes();
			for(int n = 0; n < ftypes.Length; n++) {
				FilterDateElement el = CreateFilterElement(GetFilterText(ftypes[n], n), ftypes[n], n == FilterBySpecificIndex);
				if(el == null) continue;
				res.Add(el);
			}
			RaisePrepareList(res);
			UpdateUserFiltersChecks(res);
			Ensure(0, res, FilterDateType.None);
			if(ShowEmptyFilter) {
				Ensure(1, res, FilterDateType.Empty);
			}
			Ensure(FilterBySpecificIndex, res, FilterDateType.SpecificDate);
			return res;
		}
		void Ensure(int maxIndex, List<FilterDateElement> res, FilterDateType filterDateType) {
			int current = FindIndex(res, filterDateType);
			if(current == -1) {
				res.Insert(Math.Min(maxIndex, res.Count), CreateFilterElement(GetFilterText(filterDateType, Array.IndexOf(GetFilterDateTypes(), filterDateType)),
					filterDateType, filterDateType == FilterDateType.SpecificDate));
			}
			else {
				if(current > maxIndex) {
					FilterDateElement el = res[current];
					res.RemoveAt(current);
					res.Insert(maxIndex, el);
				}
			}
		}
		int FindIndex(List<FilterDateElement> res, FilterDateType filterDateType) {
			for(int n = 0; n < res.Count; n ++) {
				if(res[n].FilterType == filterDateType) return n;
			}
			return -1;
		}
		void UpdateUserFiltersChecks(List<FilterDateElement> list) {
			if(PrevFilter == null) return;
			foreach(FilterDateElement element in list) {
				if(element.FilterType == FilterDateType.User) {
					if(PrevFilter.UserFilters.Contains(element.Criteria))
						element.Checked = true;
				}
			}
		}
		protected virtual void RaisePrepareList(List<FilterDateElement> list) {
		}
		protected virtual void CreateControlsCore() {
			LookAndFeel.ParentLookAndFeel = ElementsLookAndFeel;
			Rectangle bounds = Rectangle.Empty;
			bounds.Y = 0;
			bounds.X = 10;
			List<FilterDateElement> list = PrepareList();
			foreach(FilterDateElement element in list) {
				CreateCheckLabel(element, ref bounds, object.Equals(element.FilterType, FilterDateType.SpecificDate));
			}
			foreach(Control control in labels) {
				control.Width = this.maxLabelWidth;
			}
			if(PrevFilter != null) {
				if(IsPrevFilterIsEmpty) checks[0].Checked = list.Count(q => q.Checked) == 0;
				if(PrevFilter.FilterType == FilterDateType.SpecificDate) {
					checks[FilterBySpecificIndex].Checked = true;
					this.dateControl.SetSelection(PrevFilter.StartDate, PrevFilter.EndDate);
				}
			}
			Size = new Size(bounds.Width + 30, bounds.Y);
		}
		bool IsPrevFilterIsEmpty {
			get {
				if(object.ReferenceEquals(PrevFilter.FilterCriteria, null) && PrevFilter.FilterType == FilterDateType.None)
					return true;
				return false;
			}
		}
		FilterDateElement CreateFilterElement(string text, FilterDateType filterType, bool createCalendar) {
			CriteriaOperator filter = FilterDateTypeHelper.ToCriteria(new OperandProperty(FieldName), filterType);
			CriteriaOperator tooltipFilter = FilterDateTypeHelper.ToTooltipCriteria(new OperandProperty(FieldName), filterType);
			if(filterType == FilterDateType.Empty && !ShowEmptyFilter) return null;
			if(filterType != FilterDateType.None && filterType != FilterDateType.SpecificDate && filterType != FilterDateType.User &&
				filterType != FilterDateType.Empty && !FilterDateTypeHelper.IsFilterValid(filterType)) {
				tooltipFilter = null;
			}
			if(!createCalendar && filterType != FilterDateType.None && (
				object.ReferenceEquals(filter, null) || (Cache != null && !Cache.IsAllowFilter(filterType)))) {
				return null;
			}
			string tooltip = string.Empty;
			if(!object.Equals(null, tooltipFilter)) {
				FilterColumnCollection fcc = new FilterColumnCollection();
				fcc.Add(Field);
				CriteriaOperator op = DisplayCriteriaGeneratorPathed.Process(fcc, tooltipFilter);
				if(!object.Equals(null, op))
					tooltip = LocalaizableCriteriaToStringProcessor.Process(Localizer.Active, op);
			}
			FilterDateElement res = new FilterDateElement(text, tooltip, filter, filterType);
			if(PrevFilter != null && filterType != FilterDateType.None) {
				if((PrevFilter.FilterType & filterType) != 0) {
					res.Checked = true;
				}
			}
			return res;
		}
		void CreateCheckLabel(FilterDateElement element, ref Rectangle bounds, bool createCalendar) {
			Size size;
			ButtonCheckEdit ce = CreateCheck(bounds, element, out size);
			this.maxLabelWidth = Math.Max(this.maxLabelWidth, size.Width);
			ce.Checked = element.Checked;
			bounds.Y += size.Height + IndentControls;
			bounds.Width = Math.Max(bounds.Width, (size.Width) - bounds.X);
			if(createCalendar) CreateCalendar(ref bounds, bounds.X + IndentDateControlX);
		}
		ButtonCheckEdit CreateCheck(Rectangle bounds, FilterDateElement element, out Size size) {
			ButtonCheckEdit ce = new ButtonCheckEdit();
			ce.ButtonLabelClick += new EventHandler(OnLabelClick);
			ce.Text = element.Caption;
			ce.ToolTip = element.Tooltip;
			ce.LookAndFeel.ParentLookAndFeel = ElementsLookAndFeel;
			ce.Location = bounds.Location;
			ce.Tag = element;
			Controls.Add(ce);
			size = ce.CalcBestSize();
			ce.Size = size;
			ce.CheckedChanged += new EventHandler(OnCheckedChanged);
			this.checks.Add(ce);
			this.labels.Add(ce);
			return ce;
		}
		void CreateCalendar(ref Rectangle bounds, int x) {
			this.dateControl = new DateControlEx();
			this.dateControl.LookAndFeel.ParentLookAndFeel = ElementsLookAndFeel;
			this.dateControl.SelectionChanged += new EventHandler(OnDateModified);
			this.dateControl.DoubleClick += OnDateControlDoubleClick;
			this.dateControl.UpdateDateTimeWhenNavigating = false;
			this.dateControl.Bounds = bounds;
			this.dateControl.Left = x;
			this.dateControl.ShowTodayButton = false;
			this.dateControl.SelectionMode = CalendarSelectionMode.Multiple;
			Controls.Add(this.dateControl);
			Size size = CalcDateControlSize();
			this.dateControl.Size = size;
			bounds.Y += size.Height + IndentDateControlY;
			bounds.Width = Math.Max(bounds.Width, this.dateControl.Right - bounds.X);
			this.maxLabelWidth = Math.Max(this.maxLabelWidth, bounds.Width);
		}
		void OnDateControlDoubleClick(object sender, EventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this.dateControl, e);
			var hitInfo = dateControl.GetHitInfo(ee);
			if(hitInfo.HitTest == Calendar.CalendarHitInfoType.Cell && hitInfo.HitObject is DevExpress.XtraEditors.Calendar.CalendarCellViewInfo) {
				BeginInvoke(new MethodInvoker(() => {
					ApplyFilter();
				}));
			}
		}
		Size CalcDateControlSize() {
			return this.dateControl.CalcBestSize();
		}
		bool IsAllUnchecked() {
			for(int n = 0; n < checks.Count; n++) {
				if(checks[n].Checked) return false;
			}
			return true;
		}
		int lockEvents = 0;
		void OnCheckedChanged(object sender, EventArgs e) {
			if(this.lockEvents != 0) return;
			lockEvents++;
			try {
				if(sender == checks[0] && checks[0].Checked) ResetCheckState(checks[0]);
				if(sender == checks[FilterBySpecificIndex] && checks[FilterBySpecificIndex].Checked) ResetCheckState(checks[FilterBySpecificIndex]);
				if(sender != checks[0]) {
					checks[0].Checked = false;
					if(sender != checks[FilterBySpecificIndex])
						 this.dateControl.ResetSelection();
				}
				if(sender != checks[FilterBySpecificIndex]) checks[FilterBySpecificIndex].Checked = false;
				if(IsAllUnchecked() && sender != checks[0]) {
					checks[0].Checked = true;
				}
			}
			finally {
				lockEvents--;
			}
			OnCheckedChanged();
		}
		protected virtual void OnCheckedChanged() {
		}
		protected virtual void OnDateModified() {
		}
		void OnDateModified(object sender, EventArgs e) {
			if(this.lockEvents != 0) return;
			ResetCheckState(this.checks[FilterBySpecificIndex]);
			OnDateModified();
		}
		void ResetCheckState(CheckEdit clickedCheck) {
			foreach(CheckEdit ce in this.checks) {
				if(clickedCheck != ce)
					ce.Checked = false;
				else
					ce.Checked = true;
			}
		}
		void OnLabelClick(object sender, EventArgs e) {
			ResetCheckState(sender as CheckEdit);
			OnApplyFilter(CalcFilterResult((FilterDateElement)(sender as CheckEdit).Tag));
		}
		public DateFilterResult CalcFilterResult() {
			FilterDateType filter = FilterDateType.None;
			CriteriaOperator oper = null;
			List<CriteriaOperator> userFilters = new List<CriteriaOperator>();
			foreach(CheckEdit ce in checks) {
				FilterDateElement element = (FilterDateElement)ce.Tag;
				FilterDateType current = element.FilterType;
				if(!ce.Checked) continue;
				if(current == FilterDateType.None || current == FilterDateType.SpecificDate) {
					return CalcFilterResult(element);
				}
				CriteriaOperator op = element.Criteria;
				if(object.ReferenceEquals(op, null)) continue;
				if(current == FilterDateType.User) {
					userFilters.Add(op);
					oper = oper | op;
				}
				filter |= current;
			}
			if(filter == FilterDateType.None) {
				return null;
			}
			if((filter & (~FilterDateType.User)) != FilterDateType.None)
				oper = oper | FilterDateTypeHelper.ToCriteria(new OperandProperty(FieldName), filter);
			DateFilterResult res = new DateFilterResult(filter);
			res.FilterCriteria = oper;
			res.UserFilters.AddRange(userFilters);
			return res;
		}
		DateFilterResult CalcFilterResult(FilterDateElement element) {
			DateFilterResult filter = new DateFilterResult(element.FilterType);
			if(element.FilterType == FilterDateType.None) {
				return filter;
			}
			if(element.FilterType == FilterDateType.SpecificDate) {
				DateTime min = dateControl.SelectionStart, max = dateControl.SelectionEnd;
				filter.SetDates(min, max);
				filter.FilterCriteria = (new OperandProperty(FieldName) >= min) & (new OperandProperty(FieldName) < max);
			}
			else {
				filter.FilterCriteria = element.Criteria;
				if(element.FilterType == FilterDateType.User)
					filter.UserFilters.Add(element.Criteria);
			}
			return filter;
		}
		public void ApplyFilter() {
			this.result = null;
			DateFilterResult res = CalcFilterResult();
			if(res == null) {
				OnCancelPopup();
			}
			OnApplyFilter(res);
		}
		protected virtual string FormatDisplayText(string displayCriteria) {
			if(string.IsNullOrEmpty(displayCriteria)) return string.Empty;
			return string.Format("{0} {1} ({2})", FieldCaption, Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringIn), displayCriteria);
		}
		protected virtual string GetFilterDisplayText(FilterDateElement current) {
			int index = Array.IndexOf(GetFilterDateTypes(), current.FilterType);
			if(index == -1) return current.Caption;
			return GetFilterText(current.FilterType, index);
		}
		protected virtual void OnApplyFilter(DateFilterResult filter) {
			this.result = filter;
		}
		protected virtual void OnCancelPopup() {
			this.result = null;
		}
		FilterDateType[] filterTypesAlt = new FilterDateType[] {
			FilterDateType.None, FilterDateType.Empty,
			FilterDateType.SpecificDate, 
			FilterDateType.Beyond, FilterDateType.MonthAfter2, FilterDateType.MonthAfter1,
			FilterDateType.NextWeek,
			FilterDateType.Today, 
			FilterDateType.ThisWeek, FilterDateType.ThisMonth, FilterDateType.MonthAgo1,
			FilterDateType.MonthAgo2, FilterDateType.MonthAgo3, FilterDateType.MonthAgo4,
			FilterDateType.MonthAgo5, FilterDateType.MonthAgo6, FilterDateType.Earlier };
		FilterDateType[] filterTypes = new FilterDateType[] {
			FilterDateType.None, FilterDateType.Empty,
			FilterDateType.SpecificDate, 
			FilterDateType.BeyondThisYear, FilterDateType.LaterThisYear, FilterDateType.LaterThisMonth, 
			FilterDateType.NextWeek, FilterDateType.LaterThisWeek, FilterDateType.Tomorrow, 
			FilterDateType.Today, FilterDateType.Yesterday, FilterDateType.EarlierThisWeek, FilterDateType.LastWeek, FilterDateType.EarlierThisMonth,
			FilterDateType.EarlierThisYear, FilterDateType.PriorThisYear };
		public virtual bool UseAltFilter { get { return useAltFilter; } set { useAltFilter = value; } }
		protected FilterDateType[] GetFilterDateTypes() {
			if(UseAltFilter) return filterTypesAlt;
			return filterTypes;
		}
		protected virtual string GetFilterText(FilterDateType filterType, int index) {
			if(!UseAltFilter) return FilterText[index];
			if(filterType >= FilterDateType.MonthAfter1 && filterType <= FilterDateType.MonthAgo6) {
				return string.Format(FilterTextAlt[FilterTextAlt.Length - 1], FilterDateTypeHelper.GetMonthAgo(DateTime.Now, filterType));
			}
			if(filterType == FilterDateType.Earlier) return FilterTextAlt[FilterTextAlt.Length - 2];
			if(index >= FilterTextAlt.Length) return string.Empty;
			return FilterTextAlt[index];
		}
		protected string[] FilterText {
			get {
				if(_filterText == null) {
					_filterText = Localizer.Active.GetLocalizedString(StringId.FilterOutlookDateText).Split('|');
					if(_filterText.Length != filterTypes.Length) {
						throw new InvalidOperationException("Localized 'StringId.FilterOutlookDateText' incorrect");
					}
				}
				return _filterText;
			}
		}
		protected string[] FilterTextAlt {
			get {
				if(_filterTextAlt == null) {
					_filterTextAlt = Localizer.Active.GetLocalizedString(StringId.FilterDateTextAlt).Split('|');
				}
				return _filterTextAlt;
			}
		}
		string[] _filterText = null, _filterTextAlt = null;
	}
	[ToolboxItem(false)]
	public class ButtonCheckEdit : CheckEdit {
		private static readonly object buttonLabelClickEvent = new object();
		protected override void CreateRepositoryItem() { 
			this.fProperties = new RepositoryItemButtonCheckEdit(this);
			this.fProperties.SetOwnerEdit(this);
		}
		protected internal virtual void RaiseButtonLabelClick() {
			EventHandler handler = (EventHandler)Events[buttonLabelClickEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		public event EventHandler ButtonLabelClick {
			add { Events.AddHandler(buttonLabelClickEvent, value); }
			remove { Events.RemoveHandler(buttonLabelClickEvent, value); }
		}
	}
	[ToolboxItem(false)]
	public class RepositoryItemButtonCheckEdit : RepositoryItemCheckEdit {
		public RepositoryItemButtonCheckEdit(BaseEdit owner) {
			SetOwnerEdit(owner);
		}
		public override BaseEditViewInfo CreateViewInfo() { return new ButtonCheckEditViewInfo(this); }
		public override BaseEditPainter CreatePainter() { return new ButtonCheckEditPainter(); }
		protected internal override void RaiseCheckedChanged(EventArgs e) {
			base.RaiseCheckedChanged(e);
			if(OwnerEdit == null) return;
			ButtonCheckEditViewInfo vi = OwnerEdit.GetViewInfo() as ButtonCheckEditViewInfo;
			if(vi.MouseInButtonBounds) ((ButtonCheckEdit)OwnerEdit).RaiseButtonLabelClick();
		}
	}
	public class ButtonCheckEditPainter : CheckEditPainter {
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) { }
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			ButtonCheckEditViewInfo vi = info.ViewInfo as ButtonCheckEditViewInfo;
			Rectangle bounds = vi.ButtonBounds;
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(info.Cache);
			args.Bounds = bounds;
			args.State = vi.State;
			if(args.State == ObjectState.Normal || !vi.Bounds.Contains(vi.MousePosition)) return;
			vi.LookAndFeel.Painter.Button.DrawObject(args);
			if(!vi.MouseInButtonBounds)
				info.Cache.FillRectangle(info.Cache.GetSolidBrush(Color.FromArgb(150, LookAndFeelHelper.GetSystemColor(vi.LookAndFeel, SystemColors.Control))), bounds);
			vi.CheckInfo.Cache = info.Cache;
			vi.CheckPainter.DrawCaption(vi.CheckInfo);
		}
	}
	public class ButtonCheckEditViewInfo : CheckEditViewInfo {
		bool mouseInButtonBounds = false;
		public ButtonCheckEditViewInfo(RepositoryItem item) : base(item) { }
		protected override BaseCheckObjectInfoArgs CreateCheckArgs() {
			BaseCheckObjectInfoArgs res = base.CreateCheckArgs();
			res.TextGlyphIndent = 10;
			return res;
		}
		protected override bool UpdateObjectState() { 
			bool res = base.UpdateObjectState();
			if(!ButtonBounds.IsEmpty) {
				bool mouseInButton = ButtonBounds.Contains(MousePosition);
				if(mouseInButton != MouseInButtonBounds) {
					res = true;
					this.mouseInButtonBounds = mouseInButton;
				}
			}
			return res;
		}
		protected internal bool MouseInButtonBounds { get { return mouseInButtonBounds; } }
		public Rectangle ButtonBounds {
			get {
				if(!IsReady || CheckInfo == null || CheckInfo.CaptionRect.IsEmpty) return Rectangle.Empty;
				Rectangle bounds = Bounds;
				bounds.X = CheckInfo.GlyphRect.Right + 3;
				bounds.Width = Bounds.Right - bounds.X;
				return bounds;
			}
		}
		public override Size CalcBestFit(Graphics g) {
			Size size = base.CalcBestFit(g);
			size.Height += 2;
			return size;
		}
	}
	[ToolboxItem(false)]
	public class DateControlEx : VistaCalendarControl {
		public DateControlEx() {
			ShowClearButton = false;
			ShowTodayButton = false;
			SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse, true);
		}
		DateTime GetDate(DateTime dateTime) {
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
		}
		public override void SetSelection(DateTime start, DateTime end) {
			if(end < start) end = start;
			TimeSpan span = end.Subtract(start);
			if(span.TotalDays > 300) {
				if(end == DateTime.MaxValue)
					end = start.Add(TimeSpan.FromDays(300));
				else
					start = end.Subtract(TimeSpan.FromDays(300));
			}
			base.SetSelection(new DateRange(start, end));
		}
		protected override bool IsInputKey(Keys key) {
			if((key & Keys.KeyCode) == Keys.Tab) return false;
			if((key & Keys.KeyCode) == Keys.F4) return false;
			return true;
		}
		internal void ResetSelection() {
			DateTime dt = DateTime.Now;
			dt = new DateTime(dt.Year, dt.Month, dt.Day);
			SetSelection(dt, dt);
		}
	}
	public class DateFilterInfoCache {
		Hashtable allowedFilterTypes;
		EvaluatorContext context;
		FilterEvaluatorContext filterContext;
		public DateFilterInfoCache() {
			this.allowedFilterTypes = new Hashtable();
			this.filterCriteria = new CriteriaOperator[RangeFilterTypes.Length];
			this.filterContext = new FilterEvaluatorContext();
			this.context = new EvaluatorContext(this.filterContext, this.filterContext);
		}
		internal static FilterDateType[] RangeFilterTypes = new FilterDateType[] {
			FilterDateType.BeyondThisYear, FilterDateType.LaterThisYear, FilterDateType.LaterThisMonth, 
			FilterDateType.NextWeek, FilterDateType.LaterThisWeek, FilterDateType.Tomorrow, 
			FilterDateType.Today, FilterDateType.Yesterday, FilterDateType.EarlierThisWeek, FilterDateType.LastWeek, FilterDateType.EarlierThisMonth,
			FilterDateType.EarlierThisYear, FilterDateType.PriorThisYear,
			FilterDateType.ThisWeek, FilterDateType.ThisMonth, FilterDateType.Earlier,
			FilterDateType.MonthAgo1, FilterDateType.MonthAgo2, FilterDateType.MonthAgo3, FilterDateType.MonthAgo4,
			FilterDateType.MonthAgo5, FilterDateType.MonthAgo6, FilterDateType.MonthAfter1, FilterDateType.MonthAfter2,
			FilterDateType.Beyond
		};
		CriteriaOperator[] filterCriteria;
		ExpressionEvaluatorCore evaluator = new ExpressionEvaluatorCore(false);
		public void Init(object[] values) {
			InitFilterCriterias("Value");
			if(values == null) { 
				for(int n = 0; n < filterCriteria.Length; n++) { filterCriteria[n] = null; }
				return;
			}
			for(int n = 0; n < values.Length; n++) {
				object val = values[n];
				if(!(val is DateTime)) continue;
				if(CheckValue((DateTime)val)) break;
			}
		}
		public CriteriaOperator[] FilterCriteriaList { get { return filterCriteria; } }
		public bool IsAllowFilter(FilterDateType filterType) {
			int index = Array.IndexOf(RangeFilterTypes, filterType);
			if(index == -1) return true;
			return object.ReferenceEquals(filterCriteria[index], null);
		}
		public void InitFilterCriterias(string fieldName) {
			for(int n = 0; n < RangeFilterTypes.Length; n++) {
				FilterDateType filterType = RangeFilterTypes[n];
				CriteriaOperator op = FilterDateTypeHelper.ToCriteria(new OperandProperty(fieldName), filterType);
				this.filterCriteria[n] = op;
			}
		}
		bool CheckValue(DateTime val) {
			bool allZero = true;
			for(int n = 0; n < RangeFilterTypes.Length; n++) {
				CriteriaOperator op = filterCriteria[n];
				if(object.ReferenceEquals(op, null)) continue;
				allZero = false;
				this.filterContext.Value = val;
				if(evaluator.Fit(context, op)) {
					filterCriteria[n] = null;
				}
			}
			return allZero;
		}
		class FilterEvaluatorContext : EvaluatorContextDescriptor {
			public object Value = null;
			public override IEnumerable GetCollectionContexts(object source, string collectionName) {
				throw new Exception("The method or operation is not implemented.");
			}
			public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
				throw new Exception("The method or operation is not implemented.");
			}
			public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) {
				throw new Exception("The method or operation is not implemented.");
			}
			public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) { return Value; }
		}
	}
}
