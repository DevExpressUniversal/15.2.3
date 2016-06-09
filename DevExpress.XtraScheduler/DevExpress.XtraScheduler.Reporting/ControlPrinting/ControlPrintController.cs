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
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region  PrintObject
	public class PrintObject {
		TimeInterval interval = TimeInterval.Empty;
		Resource resource = ResourceBase.Empty;
		#region Constructors
		public PrintObject() {
		}
		public PrintObject(TimeInterval interval) {
			this.interval = interval;
		}
		public PrintObject(Resource resource) {
			this.resource = resource;
		}
		public PrintObject(TimeInterval interval, Resource resource) {
			this.interval = interval;
			this.resource = resource;
		}
		#endregion
		#region Properties
		protected internal TimeInterval Interval { get { return interval; } }
		protected internal Resource Resource { get { return resource; } }
		#endregion
	}
	#endregion
	#region PrintObjectCollection
	public class PrintObjectCollection : List<PrintObject> {
	}
	public class ReportsTimeIntervalCollection : TimeIntervalCollection {
		public ReportsTimeIntervalCollection() {
			UniquenessProviderType = DevExpress.Utils.DXCollectionUniquenessProviderType.None;
			KeepSorted = false;
		}
	}
	#endregion
	#region ResourcePrintObjects
	public class ResourcePrintObjects {
		Resource resource;
		PrintObjectCollection printObjects;
		public ResourcePrintObjects(PrintObjectCollection printObjects) {
			if (printObjects == null)
				Exceptions.ThrowArgumentNullException("printObjects");
			this.printObjects = printObjects;
			this.resource = ResourceBase.Empty;
		}
		public ResourcePrintObjects(PrintObjectCollection printObjects, Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			if (printObjects == null)
				Exceptions.ThrowArgumentNullException("printObjects");
			this.resource = resource;
			this.printObjects = printObjects;
		}
		public Resource Resource { get { return resource; } }
		public PrintObjectCollection PrintObjects { get { return printObjects; } }
	}
	#endregion
	#region ResourcePrintObjectsCollection
	public class ResourcePrintObjectsCollection : List<ResourcePrintObjects> {
	}
	#endregion
	#region ControlLayoutInfo
	public class ControlLayoutInfo {
		Rectangle controlPrintBounds;
		AnchorCollection horizontalAnchors = new AnchorCollection();
		AnchorCollection verticalAnchors = new AnchorCollection();
		public AnchorCollection HorizontalAnchors { get { return horizontalAnchors; } }
		public AnchorCollection VerticalAnchors { get { return verticalAnchors; } }
		public Rectangle ControlPrintBounds { get { return controlPrintBounds; } set { controlPrintBounds = value; } }
	}
	#endregion
	#region ControlPrintControllerBase
	public abstract class ControlPrintControllerBase {
		#region Fields
		ReportViewControlBase control;
		readonly AnchorBuilderManager anchorBuilderManager;
		PrintDataIteratorBase dataIterator;
		PrintDataCache dataCache;
		#endregion
		protected ControlPrintControllerBase() {
			this.anchorBuilderManager = new AnchorBuilderManager();
		}
		protected virtual void Initialize() {
		}
		protected internal virtual void EnsureDataCaches() {
			if (dataCache == null)
				dataCache = CreateDataCache();
		}
		public ReportViewControlBase Control { get { return control; } }
		public ReportViewBase View { get { return Control.View; } }
		protected internal PrintDataIteratorBase DataIterator { get { return dataIterator; } }
		protected internal PrintDataCache DataCache { get { return dataCache; } }
		protected internal ControlPrintState PrintState { get { return DataCache.PrintState; } }
		protected internal int GroupLength { get { return DataCache.GroupLength; } set { DataCache.GroupLength = Math.Max(1, value); } }
		protected internal bool AllowMultiColumn { get { return DataCache.AllowMultiColumn; } set { DataCache.AllowMultiColumn = value; } }
		protected abstract PrintDataCache CreateDataCache();
		protected internal AnchorBuilderManager AnchorBuilderManager { get { return anchorBuilderManager; } }
		protected internal virtual AnchorCreationInfo CalculateHorizontalAnchorCreationInfo(Rectangle bounds) {
			AnchorCollection masterAnchors = CalculateHorizontalMasterAnchors();
			ResourcePrintObjectsCollection printObjects = GetHorizontalPrintObjects();
			return new AnchorCreationInfo(masterAnchors, printObjects, bounds, AnchorDirection.Horizontal);
		}
		protected internal virtual AnchorCreationInfo CalculateVerticalAnchorCreationInfo(AnchorCollection hAnchors, Rectangle bounds) {
			AnchorCollection masterAnchors = CalculateVerticalMasterAnchors();
			ResourcePrintObjectsCollection printObjects = GetVerticalPrintObjects(hAnchors);
			return new AnchorCreationInfo(masterAnchors, printObjects, bounds, AnchorDirection.Vertical);
		}
		protected internal virtual bool IsPrintingComplete() {
			return DataCache.IsPrintingComplete();
		}
		public void Print(ReportViewControlBase control) {
			SetControlInternal(control);
			if (MoveNext()) {
				PrintControlCore();
			}
		}
		internal virtual void SetControlInternal(ReportViewControlBase control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		protected virtual void PrintControlCore() {
			if (Control.CanWriteContent()) {
				CalculateControlLayout();
				ExecutePrinter();
			}
		}
		public virtual bool MoveNext() {
			if (this.dataIterator == null) {
				this.dataIterator = CreatePrintDataIterator();
			}
			return DataIterator.MoveNext(DataCache);
		}
		protected virtual PrintDataIteratorBase CreatePrintDataIterator() {
			if (Control.IsDesignMode)
				return new DesignPrintDataIterator();
			return new PrintDataIterator();
		}
		protected internal virtual void CalculateControlLayout() {
			Rectangle controlPrintBounds = CalculateControlPrintBounds();
			AnchorCollection hAnchors = CalculateHorizontalAnchors(controlPrintBounds);
			AnchorCollection vAnchors = CalculateVerticalAnchors(hAnchors, controlPrintBounds);
			CalculateControlLayoutCore(hAnchors, vAnchors, controlPrintBounds);
		}
		protected internal virtual AnchorCollection CalculateHorizontalAnchors(Rectangle controlPrintBounds) {
			AnchorBuilderBase hBuilder = AnchorBuilderManager.GetHorizontalAnchorBuilder(Control);
			XtraSchedulerDebug.Assert(hBuilder != null);
			AnchorCreationInfo hCreationInfo = CalculateHorizontalAnchorCreationInfo(controlPrintBounds);
			AnchorCollection hAnchors = hBuilder.CreateAnchors(hCreationInfo);
			return hAnchors;
		}
		protected internal virtual AnchorCollection CalculateVerticalAnchors(AnchorCollection hAnchors, Rectangle controlPrintBounds) {
			AnchorBuilderBase vBuilder = AnchorBuilderManager.GetVerticalAnchorBuilder(Control);
			XtraSchedulerDebug.Assert(vBuilder != null);
			AnchorCreationInfo vCreationInfo = CalculateVerticalAnchorCreationInfo(hAnchors, controlPrintBounds);
			AnchorCollection vAnchors = vBuilder.CreateAnchors(vCreationInfo);
			int topPx = XRConvert.Convert(Control.CornersOptionsInternal.Top, Control.Dpi, GraphicsDpi.Pixel);
			int bottomPx = XRConvert.Convert(Control.CornersOptionsInternal.Bottom, Control.Dpi, GraphicsDpi.Pixel);
			vBuilder.ApplyVerticalAnchorsIndent(vAnchors, topPx, bottomPx);
			return vAnchors;
		}
		protected virtual void CalculateControlLayoutCore(AnchorCollection horizontalAnchors, AnchorCollection verticalAnchors, Rectangle controlBounds) {
			ControlLayoutInfo info = new ControlLayoutInfo();
			info.HorizontalAnchors.Clear();
			info.HorizontalAnchors.AddRange(horizontalAnchors);
			info.VerticalAnchors.Clear();
			info.VerticalAnchors.AddRange(verticalAnchors);
			info.ControlPrintBounds = controlBounds;
			Control.CalculateLayout(info);
		}
		protected virtual Rectangle CalculateControlPrintBounds() {
			Size pixelSize = XRConvert.Convert(Control.Bounds.Size, Control.Dpi, GraphicsDpi.Pixel);
			return new Rectangle(Point.Empty, pixelSize);
		}
		protected internal virtual AnchorCollection CalculateHorizontalMasterAnchors() {
			AnchorCollection anchors = Control.GetHorizontalMasterPrintAnchors();
			return anchors;
		}
		protected internal virtual AnchorCollection CalculateVerticalMasterAnchors() {
			AnchorCollection anchors = Control.GetVerticalMasterPrintAnchors();
			return anchors;
		}
		protected virtual void ExecutePrinter() {
			Control.ExecutePrinter();
		}
		protected internal virtual ResourcePrintObjectsCollection GetHorizontalPrintObjects() {
			return new ResourcePrintObjectsCollection();
		}
		protected internal virtual ResourcePrintObjectsCollection GetVerticalPrintObjects(AnchorCollection hAnchors) {
			return new ResourcePrintObjectsCollection();
		}
		protected internal virtual bool IsEndOfData() {
			return DataCache.IsEndOfData();
		}
		protected internal virtual void SetPrintState(ControlPrintState printState) {
			DataCache.SetPrintState(printState);
		}
		protected internal virtual PrintObjectCollection CreatePrintObjects(ResourceBaseCollection resources) {
			PrintObjectCollection result = new PrintObjectCollection();
			int count = resources.Count;
			for (int i = 0; i < count; i++)
				result.Add(new PrintObject(resources[i]));
			return result;
		}
		protected internal virtual PrintObjectCollection CreatePrintObjects(TimeIntervalCollection intervals) {
			PrintObjectCollection result = new PrintObjectCollection();
			int count = intervals.Count;
			for (int i = 0; i < count; i++)
				result.Add(new PrintObject(intervals[i]));
			return result;
		}
		protected internal virtual ResourcePrintObjectsCollection CreateEmptyResourcePrintObjects(PrintObjectCollection printObjects) {
			ResourcePrintObjectsCollection result = new ResourcePrintObjectsCollection();
			ResourcePrintObjects resourcePrintObjects = new ResourcePrintObjects(printObjects);
			result.Add(resourcePrintObjects);
			return result;
		}
	}
	#endregion
	public class DependentPrintController : ControlPrintControllerBase {
		public DependentPrintController() {
		}
		protected override PrintDataCache CreateDataCache() {
			return new DependentDataCache();
		}
	}
	#region ResourcePrintControllerBase (abstract class)
	public abstract class ResourcePrintControllerBase : ControlPrintControllerBase {
		readonly ISchedulerResourceProvider resourceProvider;
		protected ResourcePrintControllerBase(ISchedulerResourceProvider provider) {
			if (provider == null)
				Exceptions.ThrowArgumentNullException("provider");
			this.resourceProvider = provider;
		}
		protected internal ISchedulerResourceProvider ResourceProvider { get { return resourceProvider; } }
		protected internal ResourceBaseCollection PrintResources { get { return DataCache.PrintResources; } }
		protected internal new ResourceDataCache DataCache { get { return (ResourceDataCache)base.DataCache; } }
		protected override PrintDataCache CreateDataCache() {
			return new ResourceDataCache(ResourceProvider);
		}
	}
	#endregion
	#region TimeIntervalPrintControllerBase
	public abstract class TimeIntervalPrintControllerBase : ControlPrintControllerBase {
		readonly ISchedulerTimeIntervalProvider timeIntervalProvider;
		protected TimeIntervalPrintControllerBase(ISchedulerTimeIntervalProvider provider) {
			if (provider == null)
				Exceptions.ThrowArgumentNullException("provider");
			this.timeIntervalProvider = provider;
		}
		protected internal ISchedulerTimeIntervalProvider TimeIntervalProvider { get { return timeIntervalProvider; } }
		protected internal new TimeIntervalDataCache DataCache { get { return (TimeIntervalDataCache)base.DataCache; } }
		protected internal TimeIntervalCollection PrintTimeIntervals { get { return DataCache.PrintTimeIntervals; } }
		protected override PrintDataCache CreateDataCache() {
			return new ColumnTimeIntervalDataCache(TimeIntervalProvider);
		}
	}
	#endregion
	#region WeekBasedPrintController
	public abstract class WeekControlBasePrintController : CellsControlPrintControllerBase {
		protected WeekControlBasePrintController(ISchedulerTimeIntervalProvider provider, ISchedulerResourceProvider resourceProvider)
			: base(provider, resourceProvider) {
		}
		public new WeekCellsControlBase Control { get { return (WeekCellsControlBase)base.Control; } }
		protected internal new WeekControlBaseDataCache DataCache { get { return (WeekControlBaseDataCache)base.DataCache; } }
		protected internal WeekCollection PrintWeeks { get { return DataCache.PrintWeeks; } }
		public DayOfWeek FirstDayOfWeek { get { return DataCache.FirstDayOfWeek; } set { DataCache.FirstDayOfWeek = value; } }
		public WeekDays VisibleWeekDays { get { return DataCache.VisibleWeekDays; } set { DataCache.VisibleWeekDays = value; } }
		public bool ExactlyOneMonth { get { return DataCache.ExactlyOneMonth; } set { DataCache.ExactlyOneMonth = value; } }
		public WeekControlDataType DataType { get { return Control.DataType; } }
		protected internal virtual TimeIntervalCollection GetWeekDaysIntervals(Week week) {
			TimeIntervalCollection result = new TimeIntervalCollection();
			int count = week.WeekDays.Length;
			for (int i = 0; i < count; i++)
				result.Add(new TimeInterval(week.WeekDays[i], TimeSpan.FromDays(1)));
			return result;
		}
		protected override PrintDataCache CreateDataCache() {
			if (DataType == WeekControlDataType.Months)
				return new WeekControlMonthsDataCache(TimeIntervalProvider);
			return new WeekControlWeeksDataCache(TimeIntervalProvider);
		}
	}
	#endregion
	public abstract class CellsControlPrintControllerBase : ComplexPrintControllerBase   {
		public new TimeCellsControlBase Control { get { return (TimeCellsControlBase)base.Control; } }
		protected CellsControlPrintControllerBase(ISchedulerTimeIntervalProvider provider, ISchedulerResourceProvider resourceProvider)
			: base(provider, resourceProvider) {
		}
		protected internal virtual bool ShouldUseResourcePrintObjects() {
			ISchedulerResourceIterator resourceIterator = Control.GetMasterResourceIterator();
			return resourceIterator == null ? PrintResources.Count != 0 : false;
		}
		protected internal virtual ResourcePrintObjectsCollection CreateResourceIntervalPrintObjects(PrintObjectCollection intervalPrintObjects) {
			if (ShouldUseResourcePrintObjects())
				return CreateResourceIntervalPrintObjectsCore(intervalPrintObjects);
			else {
				return CreateEmptyResourcePrintObjects(intervalPrintObjects);
			}
		}
		protected internal virtual ResourcePrintObjectsCollection CreateResourceIntervalPrintObjectsCore(PrintObjectCollection intervalsPrintObject) {
			ResourcePrintObjectsCollection result = new ResourcePrintObjectsCollection();
			int resCount = PrintResources.Count;
			for (int i = 0; i < resCount; i++) {
				ResourcePrintObjects objects = CreateResourceIntervalPrintObjectsCore(PrintResources[i], intervalsPrintObject);
				result.Add(objects);
			}
			return result;
		}
		protected internal virtual ResourcePrintObjects CreateResourceIntervalPrintObjectsCore(Resource resource, PrintObjectCollection intervalsPrintObjects) {
			PrintObjectCollection printObjects = new PrintObjectCollection();
			int count = intervalsPrintObjects.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervalsPrintObjects[i].Interval.Clone();
				PrintObject printObject = new PrintObject(interval, resource);
				printObjects.Add(printObject);
			}
			return new ResourcePrintObjects(printObjects, resource);
		}
		protected internal override ResourcePrintObjectsCollection GetVerticalPrintObjects(AnchorCollection hAnchors) {
			if (Control.IsTiledAtDesignMode)
				return GetStubVerticalPrintObjects(hAnchors);
			PrintObjectCollection intervalPrintObjects = GetVerticalPrintObjectsCore(hAnchors);
			return CreateResourceIntervalPrintObjects(intervalPrintObjects);
		}
		protected internal virtual ResourcePrintObjectsCollection GetStubVerticalPrintObjects(AnchorCollection hAnchors) {
			PrintObjectCollection intervalPrintObjects = GetVerticalPrintObjectsCore(hAnchors);
			PrintObjectCollection stubPrintObjects = new PrintObjectCollection();
			if (intervalPrintObjects.Count > 0)
				stubPrintObjects.Add(intervalPrintObjects[0]);
			return CreateEmptyResourcePrintObjects(stubPrintObjects);
		}
		protected internal override ResourcePrintObjectsCollection GetHorizontalPrintObjects() {
			PrintObjectCollection printObjects = GetHorizontalPrintObjectsCore();
			return CreateEmptyResourcePrintObjects(printObjects);
		}
		protected internal abstract PrintObjectCollection GetHorizontalPrintObjectsCore();
		protected internal abstract PrintObjectCollection GetVerticalPrintObjectsCore(AnchorCollection hAnchors);
		protected internal virtual TimeInterval GetPrintTimeInterval(PrintContentMode displayMode) {
			return DataCache.GetPrintTimeInterval(displayMode);
		}
	}
	#region ComplexPrintControllerBase
	public class ComplexPrintControllerBase : TimeIntervalPrintControllerBase {
		ResourceDataCache resourceDataCache;
		ISchedulerResourceProvider resourceProvider;
		bool resourcesEnabled;
		public ComplexPrintControllerBase(ISchedulerTimeIntervalProvider timeIntervalProvider, ISchedulerResourceProvider resourceProvider)
			: base(timeIntervalProvider) {
			if (resourceProvider == null)
				Exceptions.ThrowArgumentNullException("resourceProvider");
			this.resourceProvider = resourceProvider;
		}
		public bool ResourcesEnabled {
			get { return resourcesEnabled; }
			set {
				if (resourcesEnabled == value)
					return;
				resourcesEnabled = value;
				OnResourcesEnabledChanged();
			}
		}
		protected internal ISchedulerResourceProvider ResourceProvider { get { return resourceProvider; } }
		protected internal ResourceDataCache ResourceDataCache { get { return resourceDataCache; } }
		protected internal ResourceBaseCollection PrintResources {
			get {
				return ResourceDataCache != null ? ResourceDataCache.PrintResources : new ResourceBaseCollection();
			}
		}
		protected internal override void EnsureDataCaches() {
			base.EnsureDataCaches();
			if (ResourcesEnabled)
				EnsureResourceDataCache();
		}
		protected internal void EnsureResourceDataCache() {
			if (resourceDataCache == null)
				resourceDataCache = CreateResourceDataCache();
		}
		protected virtual void OnResourcesEnabledChanged() {
			if (ResourcesEnabled) {
				EnsureResourceDataCache();
			}
			else {
				if (resourceDataCache != null)
					resourceDataCache = null;
			}
		}
		protected internal virtual ResourceDataCache CreateResourceDataCache() {
			return new ResourceDataCache(ResourceProvider);
		}
		public override bool MoveNext() {
			bool timeIntervalResult = base.MoveNext();
			if (ResourcesEnabled) {
				bool resourceResult = DataIterator.MoveNext(ResourceDataCache);
				return timeIntervalResult | resourceResult;
			}
			return timeIntervalResult;
		}
		protected internal override bool IsEndOfData() {
			bool timeIntervalResult = base.IsEndOfData();
			if (ResourcesEnabled)
				return timeIntervalResult && ResourceDataCache.IsEndOfData();
			return timeIntervalResult;
		}
		protected internal override void SetPrintState(ControlPrintState printState) {
			base.SetPrintState(printState);
			if (ResourcesEnabled)
				ResourceDataCache.SetPrintState(printState);
		}
	}
	#endregion
}
