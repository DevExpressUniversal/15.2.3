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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
using System.Globalization;
using DevExpress.Web.ASPxScheduler.Services;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	public interface IWebAppearance {
		ASPxSchedulerStylesBase InnerStyles { get; }
		SchedulerTemplates InnerTemplates { get; }
	}
	public interface ISchedulerCell {
		int RowSpan { get; set; }
		int ColumnSpan { get; set; }
		bool FixedRowSpan { get; }
		SchedulerTableCollection InnerTables { get; }
	}
	public class EmptyWebViewInfo : IWebViewInfo {
		#region IWebViewInfo Members
		public SchedulerTable CreateTable() {
			return new SchedulerTable();
		}
		#endregion
	}
	public enum IgnoreBorderSide {
		None = 0, Left = 1, Right = 2, Top = 4, Bottom = 8
	}
	#region InternalSchedulerCellUniqueIdProvider
	public class InternalSchedulerCellUniqueIdProvider {
		int counter;
		public InternalSchedulerCellUniqueIdProvider() {
			Reset();
		}
		int Counter { get { return counter; } set { counter = value; } }
		public virtual void Reset() {
			Counter = 0;
		}
		public virtual string GetNextId() {
			int idCounter = Counter;
			Counter++;
			return idCounter.ToString();
		}
	}
	#endregion
	public abstract class InternalSchedulerCellBase : InternalTableCell, ISchedulerCell {
		#region Fields
		int rowSpan;
		int columnSpan;
		readonly SchedulerTableCollection innerTables;
		#endregion
		protected InternalSchedulerCellBase() {
			innerTables = new SchedulerTableCollection();
		}
		#region ISchedulerCell Members
		int ISchedulerCell.RowSpan { get { return rowSpan; } set { rowSpan = value; } }
		int ISchedulerCell.ColumnSpan { get { return columnSpan; } set { columnSpan = value; } }
		public virtual bool FixedRowSpan { get { return false; } }
		public SchedulerTableCollection InnerTables { get { return innerTables; } }
		#endregion
	}
	#region InternalSchedulerCell (abstract class)
	public abstract class InternalSchedulerCell : InternalSchedulerCellBase, IWebViewInfo, INamingContainer {
		#region Fields
		string cellId = String.Empty;
		bool useDefaultContent = true;
		IgnoreBorderSide ignoreBorderSide;
		LiteralControl literalControl;
		bool isFinalized;
		string shadowText;
		string shadowToolTip;
		#endregion
		#region Properties
		internal string Id { get { return cellId; } set { cellId = value; } }
		public abstract WebElementType CellType { get; }
		public virtual bool IsAlternate { get { return false; } }
		internal bool UseDefaultContent { get { return useDefaultContent; } }
		public IgnoreBorderSide IgnoreBorderSide { get { return ignoreBorderSide; } set { ignoreBorderSide = value; } }
		LiteralControl LiteralControl {
			get {
				if (literalControl == null) {
					literalControl = new LiteralControl();
					literalControl.Text = String.Empty;
				}
				return literalControl;
			}
		}
		public override string Text {
			get {
				return LiteralControl.Text;
			}
			set {
				LiteralControl.Text = value;
			}
		}
		public bool IsFinalized { get { return isFinalized; } }
		public string ShadowText { get { return shadowText; } set { shadowText = value; } }
		public string ShadowToolTip { get { return shadowToolTip; } set { shadowToolTip = value; } }
		#endregion
		#region IWebViewInfo Members
		public abstract SchedulerTable CreateTable();
		#endregion
		#region CreateControlHierarchy
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ASPxScheduler control = ASPxScheduler.ActiveControl;
			TemplateContainerBase templateContainer = GetTemplateContainer(control);
			if (templateContainer != null) {
				templateContainer.ID = SchedulerIdHelper.GenerateInternalCellTemplateContainerId();
				if (String.IsNullOrEmpty(Id))
					Id = control.CellIdProvider.GetNextId();
				this.ID = Id;
				this.useDefaultContent = false;
				Controls.Add(templateContainer);
				templateContainer.DataBind();
			}
			else {
				Controls.Add(LiteralControl);
			}
			if (UseDefaultContent)
				SetDefaultContent();
		}
		#endregion
		#region OnPreRender
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			FinalizeCreateControlHierarchy();
		}
		#endregion
		#region FinalizeCreateControlHierarchy
		protected virtual void FinalizeCreateControlHierarchy() {
			this.isFinalized = true;
			ISchedulerCell cell = (ISchedulerCell)this;
			int cellRowSpan = cell.RowSpan;
			if (cellRowSpan > 1)
				RowSpan = cellRowSpan;
			int cellColumnSpan = cell.ColumnSpan;
			if (cellColumnSpan > 1)
				ColumnSpan = cellColumnSpan;
			ID = cellId;
			AppearanceStyleBase style = GetStyle();
			style.AssignToControl(this);
			CorrectUserStyle(Style, IgnoreBorderSide);
		}
		#endregion
		internal void ApplyStyleToWebControl(WebControl control) {
			AppearanceStyleBase style = GetStyle();
			style.AssignToControl(control);
			CorrectUserStyle(control.Style, IgnoreBorderSide);
		}
		#region GetTemplateContainer
		protected internal virtual TemplateContainerBase GetTemplateContainer(ASPxScheduler control) {
			TemplatesHelper helper = TemplatesHelper.Create(control.ActiveView);
			return helper.GetTemplateContainer(CellType, this);
		}
		#endregion
		#region PrepareControlHierarchy
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (!IsFinalized)
				FinalizeCreateControlHierarchy();
		}
		#endregion
		#region CorrectUserStyle
		private void CorrectUserStyle(CssStyleCollection style, IgnoreBorderSide ignoreBorderSide) {
			if ((ignoreBorderSide & IgnoreBorderSide.Left) == IgnoreBorderSide.Left)
				style.Add("border-left-width", "0px");
			if ((ignoreBorderSide & IgnoreBorderSide.Right) == IgnoreBorderSide.Right)
				style.Add("border-right-width", "0px");
			if ((ignoreBorderSide & IgnoreBorderSide.Top) == IgnoreBorderSide.Top)
				style.Add("border-top-width", "0px");
			if ((ignoreBorderSide & IgnoreBorderSide.Bottom) == IgnoreBorderSide.Bottom)
				style.Add("border-bottom-width", "0px");
		}
		#endregion
		#region GetStyle
		protected internal virtual AppearanceStyleBase GetStyle() {
			ASPxScheduler control = ASPxScheduler.ActiveControl;
			ISchedulerWebViewInfoBase viewInfo = control.ContainerControl.Renderer.ViewInfo;
			StylesHelper helper = CreateStylesHelper(control, viewInfo);
			ITimeCell timeCell = GetTimeCell();
			return helper.GetStyle(CellType, timeCell, IsAlternate);
		}
		#endregion
		#region CreateStylesHelper
		protected internal virtual StylesHelper CreateStylesHelper(ASPxScheduler control, ISchedulerWebViewInfoBase viewInfo) {
			StylesHelper helper = StylesHelper.Create(control.ActiveView, viewInfo, control.Styles);
			return helper;
		}
		#endregion
		#region GetTimeCell
		protected internal virtual ITimeCell GetTimeCell() {
			return null;
		}
		#endregion
		#region SetDefaultContent
		protected internal virtual void SetDefaultContent() {
			Text = "&nbsp;";
			ShadowText = String.Empty;
			ShadowToolTip = String.Empty;
		}
		#endregion
		#region AssignId
		public void AssignId(string id) {
			this.Id = id;
		}
		#endregion
	}
	#endregion
	public class SchedulerCellCollection : List<ISchedulerCell> {
	}
	#region SchedulerRow
	public class SchedulerRow {
		SchedulerCellCollection cells;
		public SchedulerRow() {
			cells = new SchedulerCellCollection();
		}
		public SchedulerCellCollection Cells { get { return cells; } }
	}
	#endregion
	public class SchedulerRowCollection : List<SchedulerRow> {
	}
	#region SchedulerInternalStyles
	public class SchedulerInternalStyles {
		#region Fields
		Dictionary<string, string> stylesDictionaryWithStringKey;
		Dictionary<HtmlTextWriterStyle, string> stylesDictionaryWithHtmlTextWriterStyleKey;
		#endregion
		public SchedulerInternalStyles() {
			this.stylesDictionaryWithStringKey = new Dictionary<string, string>();
			this.stylesDictionaryWithHtmlTextWriterStyleKey = new Dictionary<HtmlTextWriterStyle, string>();
		}
		#region Properties
		public Dictionary<string, string> StylesDictionaryWithStringKey { get { return stylesDictionaryWithStringKey; } }
		public Dictionary<HtmlTextWriterStyle, string> StylesDictionaryWithHtmlTextWriterStyleKey { get { return stylesDictionaryWithHtmlTextWriterStyleKey; } }
		#endregion
		public void Add(HtmlTextWriterStyle key, string value) {
			StylesDictionaryWithHtmlTextWriterStyleKey.Add(key, value);
		}
		public void Add(string key, string value) {
			StylesDictionaryWithStringKey.Add(key, value);
		}
		public void AssignToControl(WebControl table) {
			foreach (KeyValuePair<HtmlTextWriterStyle, string> item in StylesDictionaryWithHtmlTextWriterStyleKey)
				table.Style.Add(item.Key, item.Value);
			foreach (KeyValuePair<string, string> item in StylesDictionaryWithStringKey)
				table.Style.Add(item.Key, item.Value);
		}
	}
	#endregion
	#region SchedulerTable
	public class SchedulerTable {
		#region Fields
		SchedulerRowCollection rows;
		SchedulerInternalStyles innerStyles;
		#endregion
		public SchedulerTable() {
			this.rows = new SchedulerRowCollection();
		}
		#region Properties
		public SchedulerRowCollection Rows { get { return rows; } }
		public SchedulerInternalStyles InnerStyles {
			get {
				if (innerStyles == null)
					innerStyles = new SchedulerInternalStyles();
				return innerStyles;
			}
		}
		#endregion
		#region CreateTable
		public Table CreateTable() {
			Table table = RenderUtils.CreateTable();
			InnerStyles.AssignToControl(table);
			return table;
		}
		#endregion
	}
	#endregion
	#region SchedulerTableCollection
	public class SchedulerTableCollection : List<SchedulerTable> {
	}
	#endregion
	public interface IWebTimeCell : ITimeCell {
		void AssignId(string id);
		string Id { get; }
		Control Parent { get; }
		TableCell ContentCell { get; }
	}
	public enum CellContainerType { Horizontal, Vertical };
	public interface IWebCellContainer {
		int CellCount { get; }
		Resource Resource { get; }
		CellContainerType ContainerType { get; }
		IWebTimeCell this[int index] { get; }
	}
	public class Anchor {
		TableCell cell;
		XtraScheduler.Resource resource;
		AnchorType anchorType;
		public Anchor(TableCell cell, XtraScheduler.Resource resource, AnchorType anchorType) {
			this.cell = cell;
			this.resource = resource;
			this.anchorType = anchorType;
		}
		public TableCell Cell { get { return cell; } }
		public Resource Resource { get { return resource; } }
		public AnchorType AnchorType { get { return anchorType; } }
	}
	public class AnchorCollection : List<Anchor> {
	}
	public interface ISupportAnchors {
		void CreateLeftTopAnchor(AnchorCollection anchors);
		void CreateRightBottomAnchor(AnchorCollection anchors);
	}
	public interface ISchedulerWebViewInfoBase : IWebViewInfo, ISupportAnchors, ISupportAppointmentsBase {
		SchedulerViewBase View { get;}
		WebCellContainerCollection GetContainers();
		void AssignCellsIds();
		int GetResourceColorIndex(XtraScheduler.Resource resource);
		void Create();
		HeaderFormatSeparatorBase HeaderFormatSeparator { get; }
	}
	public interface IWebViewInfo {
		SchedulerTable CreateTable();
	}
	public abstract class MergingContainerBase : IWebViewInfo, ISupportAnchors {
		readonly SchedulerWebViewInfoCollection webObjects;
		protected MergingContainerBase() {
			webObjects = new SchedulerWebViewInfoCollection();
		}
		public SchedulerWebViewInfoCollection WebObjects { get { return webObjects; } }
		protected internal abstract SchedulerTable Merge(SchedulerTable table1, SchedulerTable table2);
		public virtual SchedulerTable CreateTable() {
			SchedulerTable result = new SchedulerTable();
			int webObjectsCount = webObjects.Count;
			for (int i = 0; i < webObjectsCount; i++)
				result = Merge(result, webObjects[i].CreateTable());
			return result;
		}
		public virtual void CreateLeftTopAnchor(AnchorCollection anchors) {
			int count = webObjects.Count;
			for (int i = 0; i < count; i++) {
				ISupportAnchors anchor = webObjects[i] as ISupportAnchors;
				if (anchor != null) {
					anchor.CreateLeftTopAnchor(anchors);
					return;
				}
			}
		}
		public virtual void CreateRightBottomAnchor(AnchorCollection anchors) {
			int count = webObjects.Count;
			for (int i = count - 1; i >= 0; i--) {
				ISupportAnchors anchor = webObjects[i] as ISupportAnchors;
				if (anchor != null) {
					anchor.CreateRightBottomAnchor(anchors);
					return;
				}
			}
		}
	}
	public class HorizontalMergingContainer : MergingContainerBase {
		protected internal override SchedulerTable Merge(SchedulerTable table1, SchedulerTable table2) {
			return TableHelper.MergeHorizontal(table1, table2);
		}
	}
	public class VerticalMergingContainer : MergingContainerBase {
		protected internal override SchedulerTable Merge(SchedulerTable table1, SchedulerTable table2) {
			return TableHelper.MergeVertical(table1, table2);
		}
	}
	#region WebLeftTopCornerBase
	public abstract class WebLeftTopCornerBase : InternalSchedulerCell {
		#region Fields
		string caption;
		int headerSpan;
		#endregion
		protected WebLeftTopCornerBase()
			: this(String.Empty, 1) {
		}
		protected WebLeftTopCornerBase(string caption, int headerSpan) {
			this.caption = caption;
			this.headerSpan = headerSpan;
			((ISchedulerCell)this).RowSpan = HeaderSpan;
		}
		#region Properties
		public InternalSchedulerCell Cell { get { return this; } }
		public string Caption { get { return caption; } }
		internal int HeaderSpan { get { return headerSpan; } }
		#endregion
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell, headerSpan);
		}
		protected internal override void SetDefaultContent() {
			if (!String.IsNullOrEmpty(Caption))
				this.Text = Caption;
			else
				base.SetDefaultContent();
		}
	}
	#endregion
	#region WebLeftTopCorner
	public class WebLeftTopCorner : WebLeftTopCornerBase {
		public WebLeftTopCorner() {
		}
		public WebLeftTopCorner(string caption, int headerSpan)
			: base(caption, headerSpan) {
		}
		public override WebElementType CellType { get { return WebElementType.LeftTopCorner; } }
	}
	#endregion
	#region WebTimeRulerHeader
	public class WebTimeRulerHeader : WebLeftTopCornerBase {
		TimeRuler timeRuler;
		public WebTimeRulerHeader(TimeRuler timeRuler, int headerSpan)
			: base(timeRuler.Caption, headerSpan) {
			this.timeRuler = timeRuler;
		}
		#region TimeRuler
		public TimeRuler TimeRuler { get { return timeRuler; } }
		public override WebElementType CellType { get { return WebElementType.TimeRulerHeader; } }
		#endregion
	}
	#endregion
	#region WebDayHeaderContainer
	public class WebDayHeaderContainer : HorizontalMergingContainer {
		#region Fields
		WebDayOfWeekHeaderCollection dayHeaders;
		XtraScheduler.Resource resource;
		#endregion
		public WebDayHeaderContainer(DayOfWeek[] daysOfWeek, WeekView view, XtraScheduler.Resource resource) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			if (daysOfWeek == null)
				Exceptions.ThrowArgumentNullException("daysOfWeek");
			if (daysOfWeek.Length == 0)
				Exceptions.ThrowArgumentException("daysOfWeek.Length", daysOfWeek.Length);
			this.resource = resource;
			bool compressWeekend = view.InnerView.CompressWeekendInternal;
			if (UseCompressedHeaders(daysOfWeek, compressWeekend))
				this.dayHeaders = CreateCompressedWebDayHeaders(daysOfWeek);
			else
				this.dayHeaders = CreateOrdinaryWebDayHeaders(daysOfWeek);
			int count = dayHeaders.Count;
			for (int i = 0; i < count; i++)
				WebObjects.Add(DayHeaders[i]);
			CalculateInvisibleCellsBorder(DayHeaders);
		}
		#region Properties
		public WebDayOfWeekHeaderCollection DayHeaders { get { return dayHeaders; } }
		public Resource Resource { get { return resource; } }
		#endregion
		bool UseCompressedHeaders(DayOfWeek[] daysOfWeek, bool compressWeekend) {
			if (!compressWeekend)
				return false;
			int sundayIndex = DateTimeHelper.FindDayOfWeekIndex(daysOfWeek, DayOfWeek.Sunday);
			int saturdayIndex = DateTimeHelper.FindDayOfWeekIndex(daysOfWeek, DayOfWeek.Saturday);
			return sundayIndex >= 0 && saturdayIndex >= 0;
		}
		WebDayOfWeekHeaderCollection CreateCompressedWebDayHeaders(DayOfWeek[] daysOfWeek) {
			WebDayOfWeekHeaderCollection result = new WebDayOfWeekHeaderCollection();
			int count = daysOfWeek.Length;
			for (int i = 0; i < count; i++) {
				if (daysOfWeek[i] == DayOfWeek.Sunday)
					continue;
				if (daysOfWeek[i] == DayOfWeek.Saturday)
					result.Add(new WebDayCompressedHeader(Resource));
				else
					result.Add(new WebDayOfWeekHeader(daysOfWeek[i], Resource));
			}
			return result;
		}
		WebDayOfWeekHeaderCollection CreateOrdinaryWebDayHeaders(DayOfWeek[] daysOfWeek) {
			WebDayOfWeekHeaderCollection result = new WebDayOfWeekHeaderCollection();
			int count = daysOfWeek.Length;
			for (int i = 0; i < count; i++)
				result.Add(new WebDayOfWeekHeader(daysOfWeek[i], Resource));
			return result;
		}
		protected internal virtual void CalculateInvisibleCellsBorder(WebDayOfWeekHeaderCollection dayHeaders) {
			if (dayHeaders.Count == 0)
				return;
			int count = dayHeaders.Count;
			dayHeaders[count - 1].IgnoreBorderSide |= IgnoreBorderSide.Right;
		}
	}
	#endregion
	#region WebDayHeader
	public class WebDayOfWeekHeader : InternalSchedulerCell, ITimeCell {
		#region Fields
		DayOfWeek dayOfWeek;
		XtraScheduler.Resource resource;
		#endregion
		public WebDayOfWeekHeader(DayOfWeek dayOfWeek, XtraScheduler.Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.dayOfWeek = dayOfWeek;
			this.resource = resource;
		}
		#region Properties
		public DayOfWeek DayOfWeek { get { return dayOfWeek; } }
		public InternalSchedulerCell Cell { get { return this; } }
		public Resource Resource { get { return resource; } }
		public override WebElementType CellType { get { return WebElementType.DayHeader; } }
		TimeInterval ITimeCell.Interval { get { return null; } }
		#endregion
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell);
		}
		protected internal override void SetDefaultContent() {
			base.SetDefaultContent();
			string caption = CalculateHeaderCaption();
			if(DesignMode) 
				Text = caption;
			ShadowText = caption;
			ShadowToolTip = CalculateHeaderToolTip();
		}
		protected internal virtual string CalculateHeaderCaption() {
			IHeaderCaptionService formatProvider = (IHeaderCaptionService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderCaptionService));
			if (formatProvider != null) {
				string format = formatProvider.GetDayOfWeekHeaderCaption(this);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, DayOfWeek);
			}
			if(DesignMode)
				return CalculateHeaderCaptionForDesignTime();
			return String.Empty;
		}
		protected virtual string CalculateHeaderCaptionForDesignTime() {
			return String.Format(CultureInfo.CurrentCulture, "{0}", DayOfWeek);
		}
		protected internal virtual string CalculateHeaderToolTip() {
			IHeaderToolTipService formatProvider = (IHeaderToolTipService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderToolTipService));
			if (formatProvider != null) {
				string format = formatProvider.GetDayOfWeekHeaderToolTip(this);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, DayOfWeek);
			}
			return String.Empty;
		}
		protected internal override ITimeCell GetTimeCell() {
			return this;
		}
	}
	#endregion
	#region WebDayCompressedHeader
	class WebDayCompressedHeader : WebDayOfWeekHeader {
		public WebDayCompressedHeader(XtraScheduler.Resource resource)
			: base(DayOfWeek.Saturday, resource) {
		}
		protected override string CalculateHeaderCaptionForDesignTime() {
			return String.Format(CultureInfo.CurrentCulture, "{0} / {1}", DayOfWeek.Saturday, DayOfWeek.Sunday);
		}
	}
	#endregion
	public abstract class WebResourceHeaderBase : InternalSchedulerCell, ITimeCell {
		#region Fields
		XtraScheduler.Resource resource;
		TimeIntervalCollection intervals;
		#endregion
		protected WebResourceHeaderBase(TimeInterval interval, XtraScheduler.Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			TimeIntervalCollection intervals = new TimeIntervalCollection();
			intervals.Add(interval);
			Initialize(intervals, resource);
		}
		protected WebResourceHeaderBase(TimeIntervalCollection intervals, XtraScheduler.Resource resource) {
			Initialize(intervals, resource);
		}
		void Initialize(TimeIntervalCollection intervals, XtraScheduler.Resource resource) {
			if (intervals == null)
				Exceptions.ThrowArgumentNullException("intervals");
			if (intervals.Count <= 0)
				Exceptions.ThrowArgumentException("intervals.Count", intervals.Count);
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.resource = resource;
			this.intervals = intervals;
		}
		#region Properties
		public InternalSchedulerCell Cell { get { return this; } }
		public Resource Resource { get { return resource; } }
		public TimeIntervalCollection Intervals { get { return intervals; } }
		TimeInterval ITimeCell.Interval { get { return Intervals[0]; } }
		#endregion
		#region IWebViewInfo Members
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(this);
		}
		#endregion
		#region SetDefaultContent
		protected internal override void SetDefaultContent() {
			this.Text = Resource.Caption;
		}
		#endregion
		protected internal override ITimeCell GetTimeCell() {
			return this;
		}
	}
	public class WebHorizontalResourceHeader : WebResourceHeaderBase {
		public WebHorizontalResourceHeader(TimeInterval interval, XtraScheduler.Resource resource)
			: base(interval, resource) {
		}
		public WebHorizontalResourceHeader(TimeIntervalCollection intervals, XtraScheduler.Resource resource)
			: base(intervals, resource) {
		}
		public override WebElementType CellType { get { return WebElementType.HorizontalResourceHeader; } }
	}
	public class WebVerticalResourceHeader : WebResourceHeaderBase {
		public WebVerticalResourceHeader(TimeInterval interval, XtraScheduler.Resource resource)
			: base(interval, resource) {
		}
		public WebVerticalResourceHeader(TimeIntervalCollection intervals, XtraScheduler.Resource resource)
			: base(intervals, resource) {
		}
		public override WebElementType CellType { get { return WebElementType.VerticalResourceHeader; } }
	}
	public class WebDateHeader : InternalSchedulerCell, ITimeCell, ISupportAnchors {
		#region Fields
		TimeInterval interval;
		ResourceBaseCollection resources;
		bool firstVisible;
		bool isAlternate;
		#endregion
		public WebDateHeader(TimeInterval interval, XtraScheduler.Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			ResourceBaseCollection resources = new ResourceBaseCollection();
			resources.Add(resource);
			Initialize(interval, resources);
		}
		public WebDateHeader(TimeInterval interval, ResourceBaseCollection resources) {
			Initialize(interval, resources);
		}
		#region Properties
		public TimeInterval Interval { get { return interval; } }
		public ResourceBaseCollection Resources { get { return resources; } }
		public override bool FixedRowSpan { get { return true; } }
		public override WebElementType CellType { get { return WebElementType.DateHeader; } }
		Resource ITimeCell.Resource { get { return Resources[0]; } }
		public bool FirstVisible { get { return firstVisible; } set { firstVisible = value; } }
		#region IsAlternate
		public override bool IsAlternate { get { return isAlternate; } }
		#endregion
		#endregion
		void Initialize(TimeInterval interval, ResourceBaseCollection resources) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			if (resources.Count == 0)
				Exceptions.ThrowArgumentException("resources.Count", resources);
			this.interval = interval;
			this.resources = resources;
			this.isAlternate = IsAlternateHeader();
		}
		#region IsAlternateHeader
		protected internal virtual bool IsAlternateHeader() {
			if (ASPxScheduler.ActiveControl == null)
				return false;
			DateTime clientNow = ASPxScheduler.ActiveControl.InnerControl.TimeZoneHelper.ToClientTime(DateTime.Now);
			return clientNow.Date == Interval.Start.Date;
		}
		#endregion
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(this);
		}
		protected internal override void SetDefaultContent() {
			base.SetDefaultContent();
			string caption = CalculateHeaderCaption();
			if(DesignMode) 
				Text = caption;
			ShadowText = caption;
			ShadowToolTip = CalculateHeaderToolTip();
		}
		protected internal virtual string CalculateHeaderCaption() {
			IHeaderCaptionService formatProvider = (IHeaderCaptionService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderCaptionService));
			if (formatProvider != null) {
				string format = formatProvider.GetDayColumnHeaderCaption(this);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, Interval.Start);
			}
			if(DesignMode) 
				return DesignTimeDateHeaderFormatter.FormatHeaderDate(this);
			return String.Empty;
		}
		protected internal virtual string CalculateHeaderToolTip() {
			IHeaderToolTipService formatProvider = (IHeaderToolTipService)ASPxScheduler.ActiveControl.GetService(typeof(IHeaderToolTipService));
			if(formatProvider != null) {
				string format = formatProvider.GetDayColumnHeaderToolTip(this);
				if(!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, Interval.Start);
			}
			return String.Empty;
		}
		#region ISupportAnchors Members
		public void CreateLeftTopAnchor(AnchorCollection anchors) {
			XtraSchedulerDebug.Assert(Resources.Count == 1);
			anchors.Add(new Anchor(this, Resources[0], AnchorType.Left));
		}
		public void CreateRightBottomAnchor(AnchorCollection anchors) {
			XtraSchedulerDebug.Assert(Resources.Count == 1);
			anchors.Add(new Anchor(this, Resources[0], AnchorType.Right));
		}
		#endregion
		protected internal override ITimeCell GetTimeCell() {
			return this;
		}
	}
	public abstract class WebGroupSeparator : InternalSchedulerCell, ISchedulerCell {
		#region Field
		readonly bool fixedRowSpan;
		#endregion
		protected WebGroupSeparator(bool fixedRowSpan) {
			this.fixedRowSpan = fixedRowSpan;
		}
		#region Properties
		public InternalSchedulerCell Cell { get { return this; } }
		bool ISchedulerCell.FixedRowSpan { get { return fixedRowSpan; } }
		#endregion
		#region IWebViewInfo Members
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell);
		}
		#endregion
	}
	public class WebGroupSeparatorHorizontal : WebGroupSeparator {
		public WebGroupSeparatorHorizontal() : base(false) { }
		public override WebElementType CellType { get { return WebElementType.GroupSeparatorHorizontal; } }
	}
	public class WebGroupSeparatorVertical : WebGroupSeparator {
		public WebGroupSeparatorVertical(bool fixedRowSpan, int rowSpan)
			: base(fixedRowSpan) {
			ISchedulerCell cell = ((ISchedulerCell)this);
			cell.RowSpan = rowSpan;
		}
		public WebGroupSeparatorVertical()
			: this(false, 0) {
		}
		public override WebElementType CellType { get { return WebElementType.GroupSeparatorVertical; } }
		public override SchedulerTable CreateTable() {
			SchedulerTable table = new SchedulerTable();
			SchedulerRow row = new SchedulerRow();
			table.Rows.Add(row);
			row.Cells.Add(this);
			int rowCount = ((ISchedulerCell)this).RowSpan;
			for (int i = 0; i < rowCount - 1; i++) {
				row = new SchedulerRow();
				table.Rows.Add(row);
			}
			return table;
		}
	}
	public abstract class WebGroupByResourceSingleElementBase : VerticalMergingContainer {
		readonly WebHorizontalResourceHeader header;
		readonly TimeIntervalCollection intervals;
		readonly XtraScheduler.Resource resource;
		readonly IWebViewInfo content;
		protected WebGroupByResourceSingleElementBase(InnerSchedulerViewBase view, IWebAppearance appearance, XtraScheduler.Resource resource) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			if (appearance == null)
				Exceptions.ThrowArgumentNullException("appearance");
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.intervals = view.InnerVisibleIntervals;
			this.resource = resource;
			this.header = new WebHorizontalResourceHeader(Intervals, Resource);
			WebObjects.Add(Header);
			this.content = CreateContent(view, appearance, Resource);
			WebObjects.Add(Content);
		}
		public WebHorizontalResourceHeader Header { get { return header; } }
		public TimeIntervalCollection Intervals { get { return intervals; } }
		public Resource Resource { get { return resource; } }
		protected internal abstract IWebViewInfo CreateContent(InnerSchedulerViewBase view, IWebAppearance appearance, XtraScheduler.Resource resource);
		public IWebViewInfo Content { get { return content; } }
		public override void CreateLeftTopAnchor(AnchorCollection anchors) {
			ISupportAnchors anchor = content as ISupportAnchors;
			if (anchor != null)
				anchor.CreateLeftTopAnchor(anchors);
		}
		public override void CreateRightBottomAnchor(AnchorCollection anchors) {
			ISupportAnchors anchor = content as ISupportAnchors;
			if (anchor != null)
				anchor.CreateRightBottomAnchor(anchors);
		}
	}
}
