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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Forms;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraScheduler.Animation.Internal;
using DevExpress.XtraScheduler.Services.Internal.Implementation;
using System.Security;
using DevExpress.Utils.Internal;
using System.Linq;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System.Runtime.CompilerServices;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler {
	#region DragDropMode
	public enum DragDropMode {
		Standard,
		Manual
	}
	#endregion
	[
	System.Runtime.InteropServices.ComVisible(false),
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "schedulercontrol.bmp"),
	Designer("DevExpress.XtraScheduler.Design.SchedulerControlDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A control that displays scheduled data using one of the available views and provides the capability to edit, save and load appointments."),
 Docking(DockingBehavior.Ask)
]
	public partial class SchedulerControl : Control, ISupportLookAndFeel, ISupportInitialize, IToolTipControlClient, IBatchUpdateable, IBatchUpdateHandler, IDXManagerPopupMenu, IPrintable, IXtraSerializable, IXtraSerializableLayout, IXtraSerializableLayoutEx, IInnerSchedulerControlOwner, ISupportAppointmentEdit, ISupportAppointmentDependencyEdit, IServiceContainer, ISupportCustomDraw, ISchedulerCommandTarget, IInnerSchedulerCommandTarget, ISupportXtraSerializer, ICommandAwareControl<SchedulerCommandId>, IRangeControlClient {
		#region Fields
		const string XtraSchedulerAppName = "XtraScheduler";
		public const string DefaultPaintStyleName = "Default";
		static MethodInfo SetStateMethodInfo;
		bool isDisposing;
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		InnerSchedulerControl innerControl;
		DateTimeScrollBar dateTimeScrollBar;
		ViewDateTimeScrollController dateTimeScrollController;
		ResourceNavigator resourceNavigator;
		ResourceScrollController resourceScrollController;
		UserLookAndFeel lookAndFeel;
		SchedulerPaintStyleCollection paintStyles;
		BorderStyles borderStyle = BorderStyles.Default;
		string paintStyleName = DefaultPaintStyleName;
		SchedulerAppearance appearance;
		SchedulerPaintStyle paintStyle;
		Rectangle clientBounds;
		Rectangle viewBounds; 
		Rectangle viewAndDateTimeScrollBarSeparatorBounds;
		Rectangle viewAndResourceNavigatorSeparatorBounds;
		OptionsLayoutBase optionsLayout;
		SchedulerOptionsRangeControl optionsRangeControl;
		object appointmentImages;
		IDXMenuManager menuManager;
		ToolTipController toolTipController;
		DragDropMode dragDropMode;
		RemindersForm remindersForm;
		SchedulerPrinter printer;
		bool showFeaturesIndicator = true;
		SchedulerPrintStyleCollection printStyles;
		SchedulerOptionsPrint optionsPrint;
		SchedulerPrintStyle activePrintStyle;
		ColoredSkinElementCache coloredSkinElementCache;
		SchedulerServices serviceAccessor;
		CellScrollBarsRegistrator cellScrollBarsRegistrator;
		SchedulerColorSchemaCollection skinResourceColorSchemas;
		ISchedulerAnimationManager animationManager;
		ScaleBasedRangeControlClient rangeControlSupport;
		bool showAhonterAppointmentEditForm = false;
		Rectangle aptFormBounds = Rectangle.Empty;
		#endregion
		static SchedulerControl() {
			SchedulerControl.SetStateMethodInfo = typeof(SchedulerControl).GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance);
#if DEBUGTEST
			DebugConfig.Init();
#endif
		}
		public static void About() {
		}
		public SchedulerControl() {
			Initialize(null);
		}
		public SchedulerControl(SchedulerStorage storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			Initialize(storage);
		}
		#region Properties
		protected internal ISchedulerAnimationManager AnimationManager { get { return animationManager; } }
		internal bool InnerIsDisposed { get { return isDisposed; } }
		internal bool InnerIsDisposing { get { return isDisposing; } }
		internal DateTimeScrollBar DateTimeScrollBarObject { get { return dateTimeScrollBar; } }
		internal ViewDateTimeScrollController DateTimeScrollController { get { return dateTimeScrollController; } }
		internal ResourceScrollController ResourceScrollController { get { return resourceScrollController; } }
		internal SchedulerPaintStyle PaintStyle { get { return paintStyle; } }
		internal Rectangle ClientBounds { get { return clientBounds; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ViewBounds { get { return viewBounds; } }
		[Obsolete("Please, use ViewBounds property instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ViewRectangle { get { return ViewBounds; } }
		internal Rectangle ViewAndDateTimeScrollBarSeparatorBounds { get { return viewAndDateTimeScrollBarSeparatorBounds; } }
		internal Rectangle ViewAndResourceNavigatorSeparatorBounds { get { return viewAndResourceNavigatorSeparatorBounds; } }
		internal WinFormsSchedulerMouseHandler MouseHandler { get { return InnerControl == null ? null : (WinFormsSchedulerMouseHandler)InnerControl.MouseHandler; } }
		internal AppointmentChangeHelper AppointmentChangeHelper { get { return InnerControl == null ? null : InnerControl.AppointmentChangeHelper; } }
		#region AppointmentSelectionController
		internal AppointmentSelectionController AppointmentSelectionController {
			get {
				if (innerControl != null)
					return innerControl.AppointmentSelectionController;
				else
					return null;
			}
		}
		#endregion
		#region AppointmentDependencySelectionController
		internal AppointmentDependencySelectionController AppointmentDependencySelectionController {
			get {
				if (innerControl != null)
					return innerControl.AppointmentDependencySelectionController;
				else
					return null;
			}
		}
		#endregion
		internal SchedulerInplaceEditControllerEx InplaceEditController { get { return InnerControl == null ? null : (SchedulerInplaceEditControllerEx)InnerControl.InplaceEditController; } }
		internal ColoredSkinElementCache ColoredSkinElementCache { get { return coloredSkinElementCache; } }
		#region Selection
		internal SchedulerViewSelection Selection {
			get {
				if (innerControl != null)
					return innerControl.Selection;
				else
					return null;
			}
			set {
				if (innerControl != null)
					innerControl.Selection = value;
			}
		}
		#endregion
		internal RemindersForm RemindersForm { get { return remindersForm; } }
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		internal InnerSchedulerControl InnerControl { get { return innerControl; } }
		#region Storage
		[ Category(SRCategoryNames.Data), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), TypeConverter(typeof(ExpandableObjectConverter))]
		public SchedulerStorage Storage {
			get {
				return (SchedulerStorage)DataStorage;
			}
			set {
				DataStorage = value;
			}
		}
		#endregion
		[Category(SRCategoryNames.Data), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), TypeConverter(typeof(ExpandableObjectConverter))]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ISchedulerStorage DataStorage {
			get {
				if (innerControl == null)
					return null;
				return (ISchedulerStorage)innerControl.Storage;
			}
			set {
				if (innerControl == null)
					return;
				if (value != null && value.IsDisposed)
					return;
				if (DataStorage == value)
					return;
				innerControl.Storage = value;
			}
		}
		#region Views
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlViews"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SchedulerViewRepository Views {
			get {
				if (innerControl != null)
					return (SchedulerViewRepository)innerControl.Views;
				else
					return null;
			}
		}
		#endregion
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public DayView DayView { get { return Views.DayView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public WorkWeekView WorkWeekView { get { return Views.WorkWeekView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public WeekView WeekView { get { return Views.WeekView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public MonthView MonthView { get { return Views.MonthView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TimelineView TimelineView { get { return Views.TimelineView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public GanttView GanttView { get { return Views.GanttView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public FullWeekView FullWeekView { get { return Views.FullWeekView; } }
		#region ActiveView
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerViewBase ActiveView {
			get {
				if (innerControl != null)
					return (SchedulerViewBase)innerControl.ActiveView.Owner;
				else
					return null;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlLookAndFeel"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerPaintStyleCollection PaintStyles { get { return paintStyles; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlResourceNavigator"),
#endif
Category(SRCategoryNames.Behavior), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ResourceNavigator ResourceNavigator { get { return resourceNavigator; } }
		#region FirstDayOfWeek
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DayOfWeek FirstDayOfWeek {
			get {
				if (innerControl != null)
					return innerControl.FirstDayOfWeek;
				else
					return DateTimeHelper.FirstDayOfWeek;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlMenuManager"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(null)]
		public IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		internal ToolTipController ActualToolTipController { get { return toolTipController != null ? toolTipController : ToolTipController.DefaultController; } }
		#region ToolTipController
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlToolTipController"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if (value == ToolTipController.DefaultController)
					value = null;
				if (ToolTipController == value)
					return;
				UnsubscribeToolTipControllerEvents(ActualToolTipController);
				UnregisterToolTipClientControl(ActualToolTipController);
				this.toolTipController = value;
				RegisterToolTipClientControl(ActualToolTipController);
				SubscribeToolTipControllerEvents(ActualToolTipController);
				UpdateResourceNavigatorToolTipController(value);
			}
		}
		#endregion
		#region AppointmentImages
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppointmentImages"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object AppointmentImages {
			get { return appointmentImages; }
			set {
				if (appointmentImages == value)
					return;
				UnsubscribeAppointmentImagesEvents();
				appointmentImages = value;
				SubscribeAppointmentImagesEvents();
				ApplyChanges(SchedulerControlChangeType.ExternalAppointmentImages);
			}
		}
		#endregion
		protected internal SchedulerColorSchemaCollection SkinResourceColorSchemas {
			get { return skinResourceColorSchemas; }
		}
		protected internal SchedulerColorSchemaCollection ActualResourceColorSchemas {
			get {
				if (!ResourceColorSchemas.HasDefaultContent())
					return ResourceColorSchemas;
				return SkinResourceColorSchemas;
			}
		}
		#region ResourceColorSchemas
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlResourceColorSchemas"),
#endif
 Category(SRCategoryNames.Appearance)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public SchedulerColorSchemaCollection ResourceColorSchemas {
			get { return (innerControl != null) ? (SchedulerColorSchemaCollection)innerControl.ResourceColorSchemas : null; }
		}
		internal bool ShouldSerializeResourceColorSchemas() {
			if (innerControl != null)
				return innerControl.ShouldSerializeResourceColorSchemas();
			else
				return false;
		}
		internal bool XtraShouldSerializeResourceColorSchemas() {
			return ShouldSerializeResourceColorSchemas();
		}
		internal void ResetResourceColorSchemas() {
			if (innerControl != null)
				innerControl.ResetResourceColorSchemas();
		}
		internal object XtraCreateResourceColorSchemasItem(XtraItemEventArgs e) {
			if (innerControl != null)
				return innerControl.XtraCreateResourceColorSchemasItem(e);
			else
				return null;
		}
		internal void XtraSetIndexResourceColorSchemasItem(XtraSetItemIndexEventArgs e) {
			if (innerControl != null)
				innerControl.XtraSetIndexResourceColorSchemasItem(e);
		}
		#endregion
		#region ActiveViewType
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlActiveViewType"),
#endif
		DefaultValue(SchedulerViewType.Day),
		Category(SRCategoryNames.View),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue)
		]
		public SchedulerViewType ActiveViewType {
			get {
				SchedulerViewBase activeView = ActiveView;
				if (activeView != null)
					return activeView.Type;
				else
					return SchedulerViewType.Day;
			}
			set {
				if (innerControl != null)
					innerControl.ActiveViewType = value;
			}
		}
		#endregion
		#region GroupType
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlGroupType"),
#endif
DefaultValue(SchedulerGroupType.None), Category(SRCategoryNames.View), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public SchedulerGroupType GroupType {
			get {
				SchedulerViewBase activeView = this.ActiveView;
				if (activeView != null)
					return activeView.GroupType;
				else
					return SchedulerGroupType.None;
			}
			set {
				if (Views != null)
					Views.SetGroupType(value);
			}
		}
		#endregion
		#region PaintStyleName
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlPaintStyleName"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(DefaultPaintStyleName),
		TypeConverter("DevExpress.XtraScheduler.Design.PaintStyleNameConverter, " + AssemblyInfo.SRAssemblySchedulerDesign),
	   Localizable(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string PaintStyleName {
			get { return paintStyleName; }
			set {
				if (paintStyleName == value)
					return;
				paintStyleName = value;
				ApplyChanges(SchedulerControlChangeType.LookAndFeelChanged);
			}
		}
		#endregion
		#region BorderStyle
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlBorderStyle"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(BorderStyles.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public BorderStyles BorderStyle {
			get {
				return borderStyle;
			}
			set {
				if (borderStyle == value)
					return;
				borderStyle = value;
				ApplyChanges(SchedulerControlChangeType.BorderStyleChanged);
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppearance"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerAppearance Appearance { get { return appearance; } }
		#region OptionsView
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlOptionsView"),
#endif
Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SchedulerOptionsView OptionsView {
			get {
				if (innerControl != null)
					return (SchedulerOptionsView)innerControl.OptionsView;
				else
					return null;
			}
		}
		#endregion
		#region OptionsCustomization
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlOptionsCustomization"),
#endif
Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SchedulerOptionsCustomization OptionsCustomization {
			get {
				if (innerControl != null)
					return innerControl.OptionsCustomization;
				else
					return null;
			}
		}
		#endregion
		#region OptionsBehavior
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlOptionsBehavior"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SchedulerOptionsBehavior OptionsBehavior {
			get {
				if (innerControl != null)
					return (SchedulerOptionsBehavior)innerControl.OptionsBehavior;
				else
					return null;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlOptionsLayout"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsLayoutBase OptionsLayout { get { return optionsLayout; } }
		#region OptionsRangeControl
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlOptionsRangeControl"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerOptionsRangeControl OptionsRangeControl {
			get {
				if (optionsRangeControl == null)
					optionsRangeControl = new SchedulerOptionsRangeControl();
				return optionsRangeControl;
			}
		}
		#endregion
		#region Start
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlStart"),
#endif
Category(SRCategoryNames.Scheduler), XtraSerializableProperty(999)]
		public DateTime Start {
			get {
				if (innerControl != null)
					return innerControl.Start;
				else
					return DateTime.MinValue;
			}
			set {
				if (innerControl != null)
					innerControl.Start = value;
			}
		}
		#endregion
		#region LimitInterval
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlLimitInterval"),
#endif
Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TimeInterval LimitInterval {
			get {
				if (innerControl != null)
					return innerControl.LimitInterval;
				else
					return null;
			}
			set {
				if (innerControl != null) {
					NotificationTimeInterval newInterval = (value == null) ? NotificationTimeInterval.FullRange :
						new NotificationTimeInterval(value.Start, value.Duration);
					innerControl.LimitInterval = newInterval;
				}
			}
		}
		internal bool ShouldSerializeLimitInterval() {
			return !NotificationTimeInterval.FullRange.Equals(LimitInterval);
		}
		protected internal void ResetLimitInterval() {
			this.LimitInterval = NotificationTimeInterval.FullRange;
		}
		internal bool XtraShouldSerializeLimitInterval() {
			return ShouldSerializeLimitInterval();
		}
		#endregion
		#region WorkDays
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkDaysCollection WorkDays {
			get {
				if (innerControl != null)
					return innerControl.WorkDays;
				else
					return null;
			}
		}
		#endregion
		[Browsable(false)]
		public bool SupportsRecurrence { get { return DataStorage != null ? DataStorage.SupportsRecurrence : false; } }
		[Browsable(false)]
		public bool SupportsReminders { get { return DataStorage != null ? DataStorage.SupportsReminders : false; } }
		[Browsable(false)]
		public bool ResourceSharing { get { return DataStorage != null ? DataStorage.ResourceSharing : false; } }
		[Browsable(false)]
		public bool RemindersEnabled { get { return DataStorage != null ? DataStorage.RemindersEnabled : false; } }
		[Browsable(false)]
		public bool UnboundMode { get { return DataStorage != null ? ((IInternalSchedulerStorageBase)DataStorage).UnboundMode : true; } }
		[DefaultValue(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlDragDropMode"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DragDropMode.Standard)]
		public DragDropMode DragDropMode { get { return dragDropMode; } set { dragDropMode = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentBaseCollection SelectedAppointments { get { return InnerControl != null ? InnerControl.SelectedAppointments : new AppointmentBaseCollection(); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeInterval SelectedInterval { get { return Selection != null ? Selection.Interval.Clone() : TimeInterval.Empty; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Resource SelectedResource { get { return Selection != null ? Selection.Resource : ResourceBase.Empty; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentDependencyBaseCollection SelectedDependencies { get { return InnerControl != null ? InnerControl.SelectedDependencies : new AppointmentDependencyBaseCollection(); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		protected internal SchedulerPrinter Printer { get { return printer; } }
		[Obsolete("Please, use DateTimeScrollBar property instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DevExpress.XtraEditors.VScrollBar VScrollBar { get { return DateTimeScrollBar as DevExpress.XtraEditors.VScrollBar; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DevExpress.XtraEditors.ScrollBarBase DateTimeScrollBar { get { return dateTimeScrollBar != null ? dateTimeScrollBar.ScrollBar : null; } }
		[
DefaultValue(true), DesignOnly(true), Category(SRCategoryNames.Design)]
		public bool ShowFeaturesIndicator { get { return showFeaturesIndicator; } set { showFeaturesIndicator = value; Invalidate(); } }
		#region PrintStyles
		[
Category(SRCategoryNames.Printing),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraScheduler.Design.PrintStylesEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public SchedulerPrintStyleCollection PrintStyles { get { return printStyles; } }
		bool ShouldSerializePrintStyles() {
			return !PrintStyles.HasDefaultContent();
		}
		void ResetPrintStyles() {
			printStyles.DisposeCollectionElements();
			printStyles = new SchedulerPrintStyleCollection();
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlOptionsPrint"),
#endif
Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerOptionsPrint OptionsPrint { get { return optionsPrint; } }
		#region ActivePrintStyle
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerPrintStyle ActivePrintStyle {
			get {
				if (activePrintStyle != null)
					return activePrintStyle;
				SchedulerPrintStyleKind printStyleKind = OptionsPrint.PrintStyle;
				if (OptionsPrint.PrintStyle == SchedulerPrintStyleKind.Default) {
					printStyleKind = SchedulerPrintStyleKind.Daily;
					if (ActiveView is WeekView)
						printStyleKind = SchedulerPrintStyleKind.Weekly;
					if (ActiveView is MonthView)
						printStyleKind = SchedulerPrintStyleKind.Monthly;
				}
				SchedulerPrintStyle style = PrintStyles[printStyleKind];
				return style != null ? style : SchedulerPrintStyle.CreateInstance(printStyleKind, false);
			}
			set {
				activePrintStyle = value;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerServices Services { get { return serviceAccessor; } }
		protected internal CellScrollBarsRegistrator CellScrollBarsRegistrator { get { return cellScrollBarsRegistrator; } }
		internal int StateIdentity { get { return InnerControl != null ? InnerControl.StateIdentity : Int32.MaxValue; } }
		protected override bool ScaleChildren { get { return false; } }
		protected internal IRangeControlClient RangeControlClientSupport {
			get { return (IRangeControlClient)RangeControlSupport; }
		}
		protected internal ScaleBasedRangeControlClient RangeControlSupport {
			get {
				if (rangeControlSupport == null) {
					IRangeControlClientDataProvider dataProvider = null;
					if (IsDisposed || InnerControl == null)
						dataProvider = new EmptyTimeClientDataProvider();
					else
						dataProvider = new SchedulerControlClientDataProvider(this);
					rangeControlSupport = CreateScaleBasedRangeControlSupport(dataProvider);
					SubscribeRangeControlSupportEvents();
				}
				return rangeControlSupport;
			}
		}
		public TimeZoneHelper TimeZoneHelper { get { return InnerControl.TimeZoneHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowAnotherEditAppointmentForm {
			get { return this.showAhonterAppointmentEditForm; }
			set { this.showAhonterAppointmentEditForm = value; }
		}
		protected internal bool IsDesignMode {
			get { return DesignMode; }
		}
		#endregion
		#region Events
		#region GroupTypeChanged
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlGroupTypeChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler GroupTypeChanged {
			add {
				if (this.innerControl != null)
					this.innerControl.GroupTypeChanged += value;
			}
			remove {
				if (this.innerControl != null)
					this.innerControl.GroupTypeChanged -= value;
			}
		}
		#endregion
		#region SelectionChanged
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlSelectionChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler SelectionChanged {
			add {
				if (innerControl != null)
					innerControl.SelectionChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.SelectionChanged -= value;
			}
		}
		#endregion
		#region ViewUIChanged
		[Category(SRCategoryNames.Scheduler)]
		internal event SchedulerViewUIChangedEventHandler ViewUIChanged {
			add {
				if (innerControl != null)
					innerControl.ViewUIChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.ViewUIChanged -= value;
			}
		}
		#endregion
		#region StorageChanged
		static readonly object onStorageChanged = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlStorageChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler StorageChanged {
			add { Events.AddHandler(onStorageChanged, value); }
			remove { Events.RemoveHandler(onStorageChanged, value); }
		}
		protected internal virtual void RaiseStorageChanged() {
			EventHandler handler = (EventHandler)Events[onStorageChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region VisibleIntervalChanged
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlVisibleIntervalChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler VisibleIntervalChanged {
			add {
				if (innerControl != null)
					innerControl.VisibleIntervalChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.VisibleIntervalChanged -= value;
			}
		}
		#endregion
		#region ActiveViewChanging
		static readonly object onActiveViewChanging = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlActiveViewChanging"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ActiveViewChangingEventHandler ActiveViewChanging {
			add { Events.AddHandler(onActiveViewChanging, value); }
			remove { Events.RemoveHandler(onActiveViewChanging, value); }
		}
		protected internal virtual bool RaiseActiveViewChanging(SchedulerViewBase newView) {
			ActiveViewChangingEventHandler handler = (ActiveViewChangingEventHandler)Events[onActiveViewChanging];
			if (handler != null) {
				ActiveViewChangingEventArgs args = new ActiveViewChangingEventArgs(ActiveView, newView);
				handler(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region ActiveViewChanged
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlActiveViewChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler ActiveViewChanged {
			add {
				if (innerControl != null)
					innerControl.ActiveViewChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.ActiveViewChanged -= value;
			}
		}
		#endregion
		#region LimitIntervalChanged
		EventHandler onLimitIntervalChanged;
		internal event EventHandler LimitIntervalChanged { add { onLimitIntervalChanged += value; } remove { onLimitIntervalChanged -= value; } }
		void RaiseLimitIntervalChanged() {
			EventHandler handler = onLimitIntervalChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region QueryWorkTime
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlQueryWorkTime"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event QueryWorkTimeEventHandler QueryWorkTime {
			add {
				if (innerControl != null)
					innerControl.QueryWorkTime += value;
			}
			remove {
				if (innerControl != null)
					innerControl.QueryWorkTime -= value;
			}
		}
		#endregion
		#region QueryResourceColorSchema
		[Category(SRCategoryNames.Scheduler)]
		QueryResourceColorSchemaEventHandler onQueryResourceColorSchema;
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerControlQueryResourceColorSchema")]
#endif
		public event QueryResourceColorSchemaEventHandler QueryResourceColorSchema {
			add {
				onQueryResourceColorSchema += value;
			}
			remove {
				onQueryResourceColorSchema -= value;
			}
		}
		protected internal virtual void RaiseQueryResourceColorSchema(QueryResourceColorSchemaEventArgs e) {
			if (onQueryResourceColorSchema != null)
				onQueryResourceColorSchema(this, e);
		}
		#endregion
		#region MoreButtonClicked
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlMoreButtonClicked"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event MoreButtonClickedEventHandler MoreButtonClicked {
			add {
				if (innerControl != null)
					innerControl.MoreButtonClicked += value;
			}
			remove {
				if (innerControl != null)
					innerControl.MoreButtonClicked -= value;
			}
		}
		#endregion
		#region InitAppointmentImages
		static readonly object onInitAppointmentImages = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlInitAppointmentImages"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentImagesEventHandler InitAppointmentImages {
			add { Events.AddHandler(onInitAppointmentImages, value); }
			remove { Events.RemoveHandler(onInitAppointmentImages, value); }
		}
		protected internal virtual void RaiseInitAppointmentImages(AppointmentImagesEventArgs args) {
			AppointmentImagesEventHandler handler = (AppointmentImagesEventHandler)Events[onInitAppointmentImages];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region InitAppointmentDisplayText
		static readonly object onInitAppointmentDisplayText = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlInitAppointmentDisplayText"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentDisplayTextEventHandler InitAppointmentDisplayText {
			add { Events.AddHandler(onInitAppointmentDisplayText, value); }
			remove { Events.RemoveHandler(onInitAppointmentDisplayText, value); }
		}
		protected internal virtual void RaiseInitAppointmentDisplayText(AppointmentDisplayTextEventArgs e) {
			AppointmentDisplayTextEventHandler handler = (AppointmentDisplayTextEventHandler)Events[onInitAppointmentDisplayText];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region AppointmentViewInfoCustomizing
		static readonly object onAppointmentViewInfoCustomizing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppointmentViewInfoCustomizing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing {
			add { Events.AddHandler(onAppointmentViewInfoCustomizing, value); }
			remove { Events.RemoveHandler(onAppointmentViewInfoCustomizing, value); }
		}
		protected internal virtual void RaiseAppointmentViewInfoCustomizing(AppointmentViewInfoCustomizingEventArgs e) {
			AppointmentViewInfoCustomizingEventHandler handler = (AppointmentViewInfoCustomizingEventHandler)Events[onAppointmentViewInfoCustomizing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region AllowAppointmentDrag
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentDrag"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDrag {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDrag += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDrag -= value;
			}
		}
		#endregion
		#region AllowAppointmentDragBetweenResources
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentDragBetweenResources"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDragBetweenResources {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDragBetweenResources += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDragBetweenResources -= value;
			}
		}
		#endregion
		#region AllowAppointmentResize
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentResize"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentResize {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentResize += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentResize -= value;
			}
		}
		#endregion
		#region AllowAppointmentCopy
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentCopy"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCopy {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentCopy += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentCopy -= value;
			}
		}
		#endregion
		#region AllowAppointmentDelete
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentDelete"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDelete {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDelete += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDelete -= value;
			}
		}
		#endregion
		#region AllowAppointmentCreate
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentCreate"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCreate {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentCreate += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentCreate -= value;
			}
		}
		#endregion
		#region AllowAppointmentEdit
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentEdit"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentEdit {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentEdit += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentEdit -= value;
			}
		}
		#endregion
		#region AllowInplaceEditor
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowInplaceEditor"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowInplaceEditor {
			add {
				if (innerControl != null)
					innerControl.AllowInplaceEditor += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowInplaceEditor -= value;
			}
		}
		#endregion
		#region AppointmentDrag
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppointmentDrag"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentDragEventHandler AppointmentDrag {
			add {
				if (innerControl != null)
					innerControl.AppointmentDrag += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentDrag -= value;
			}
		}
		#endregion
		#region AppointmentDrop
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppointmentDrop"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentDragEventHandler AppointmentDrop {
			add {
				if (innerControl != null)
					innerControl.AppointmentDrop += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentDrop -= value;
			}
		}
		#endregion
		#region AppointmentResizing
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppointmentResizing"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentResizeEventHandler AppointmentResizing {
			add {
				if (innerControl != null)
					innerControl.AppointmentResizing += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentResizing -= value;
			}
		}
		#endregion
		#region AppointmentResized
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAppointmentResized"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentResizeEventHandler AppointmentResized {
			add {
				if (innerControl != null)
					innerControl.AppointmentResized += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentResized -= value;
			}
		}
		#endregion
		#region AllowAppointmentConflicts
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlAllowAppointmentConflicts"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentConflictEventHandler AllowAppointmentConflicts {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentConflicts += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentConflicts -= value;
			}
		}
		#endregion
		#region PrepareContextMenu
		internal static readonly object onPrepareContextMenu = new object();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category(SRCategoryNames.Scheduler), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event PrepareContextMenuEventHandler PrepareContextMenu {
			add { Events.AddHandler(onPrepareContextMenu, value); }
			remove { Events.RemoveHandler(onPrepareContextMenu, value); }
		}
		[Obsolete("You should use the 'RaisePopupMenuShowing' instead.")]
		protected internal virtual void RaisePrepareContextMenu(PrepareContextMenuEventArgs args) {
			PrepareContextMenuEventHandler handler = (PrepareContextMenuEventHandler)this.Events[onPrepareContextMenu];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region PreparePopupMenu
		internal static readonly object onPreparePopupMenu = new object();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category(SRCategoryNames.Scheduler), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event PreparePopupMenuEventHandler PreparePopupMenu {
			add { Events.AddHandler(onPreparePopupMenu, value); }
			remove { Events.RemoveHandler(onPreparePopupMenu, value); }
		}
		[Obsolete("You should use the 'RaisePopupMenuShowing' instead.")]
		protected internal virtual void RaisePreparePopupMenu(PreparePopupMenuEventArgs args) {
			PreparePopupMenuEventHandler handler = (PreparePopupMenuEventHandler)this.Events[onPreparePopupMenu];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region PopupMenuShowing
		internal static readonly object onPopupMenuShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlPopupMenuShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(onPopupMenuShowing, value); }
			remove { Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		protected internal virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs args) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region InitNewAppointment
		static readonly object onInitNewAppointment = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlInitNewAppointment"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentEventHandler InitNewAppointment {
			add { Events.AddHandler(onInitNewAppointment, value); }
			remove { Events.RemoveHandler(onInitNewAppointment, value); }
		}
		protected internal virtual void RaiseInitNewAppointment(AppointmentEventArgs args) {
			AppointmentEventHandler handler = (AppointmentEventHandler)Events[onInitNewAppointment];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region GotoDateFormShowing
		static readonly object onGotoDateFormShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlGotoDateFormShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event GotoDateFormEventHandler GotoDateFormShowing {
			add { Events.AddHandler(onGotoDateFormShowing, value); }
			remove { Events.RemoveHandler(onGotoDateFormShowing, value); }
		}
		protected internal virtual void RaiseGotoDateFormShowing(GotoDateFormEventArgs e) {
			GotoDateFormEventHandler handler = (GotoDateFormEventHandler)Events[onGotoDateFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region EditAppointmentFormShowing
		static readonly object onEditAppointmentFormShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlEditAppointmentFormShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentFormEventHandler EditAppointmentFormShowing {
			add { Events.AddHandler(onEditAppointmentFormShowing, value); }
			remove { Events.RemoveHandler(onEditAppointmentFormShowing, value); }
		}
		protected internal virtual void RaiseEditAppointmentFormShowing(AppointmentFormEventArgs e) {
			AppointmentFormEventHandler handler = (AppointmentFormEventHandler)Events[onEditAppointmentFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region EditAppointmentDependencyFormShowing
		static readonly object onEditAppointmentDependencyFormShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlEditAppointmentDependencyFormShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentDependencyFormEventHandler EditAppointmentDependencyFormShowing {
			add { Events.AddHandler(onEditAppointmentDependencyFormShowing, value); }
			remove { Events.RemoveHandler(onEditAppointmentDependencyFormShowing, value); }
		}
		protected internal virtual void RaiseEditAppointmentDependencyFormShowing(AppointmentDependencyFormEventArgs e) {
			AppointmentDependencyFormEventHandler handler = (AppointmentDependencyFormEventHandler)Events[onEditAppointmentDependencyFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InplaceEditorShowing
		static readonly object onInplaceEditorShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlInplaceEditorShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event InplaceEditorEventHandler InplaceEditorShowing {
			add { Events.AddHandler(onInplaceEditorShowing, value); }
			remove { Events.RemoveHandler(onInplaceEditorShowing, value); }
		}
		protected internal virtual void RaiseInplaceEditorShowing(InplaceEditorEventArgs e) {
			InplaceEditorEventHandler handler = (InplaceEditorEventHandler)Events[onInplaceEditorShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RemindersFormDefaultAction
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlRemindersFormDefaultAction"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event RemindersFormDefaultActionEventHandler RemindersFormDefaultAction {
			add {
				if (innerControl != null)
					innerControl.RemindersFormDefaultAction += value;
			}
			remove {
				if (innerControl != null)
					innerControl.RemindersFormDefaultAction -= value;
			}
		}
		#endregion
		#region RemindersFormShowing
		static readonly object onRemindersFormShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlRemindersFormShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event RemindersFormEventHandler RemindersFormShowing {
			add { Events.AddHandler(onRemindersFormShowing, value); }
			remove { Events.RemoveHandler(onRemindersFormShowing, value); }
		}
		protected internal virtual void RaiseRemindersFormShowing(RemindersFormEventArgs e) {
			RemindersFormEventHandler handler = (RemindersFormEventHandler)Events[onRemindersFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region DeleteRecurrentAppointmentFormShowing
		static readonly object onDeleteRecurrentAppointmentFormShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlDeleteRecurrentAppointmentFormShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event DeleteRecurrentAppointmentFormEventHandler DeleteRecurrentAppointmentFormShowing {
			add { Events.AddHandler(onDeleteRecurrentAppointmentFormShowing, value); }
			remove { Events.RemoveHandler(onDeleteRecurrentAppointmentFormShowing, value); }
		}
		protected internal virtual void RaiseDeleteRecurrentAppointmentFormShowing(DeleteRecurrentAppointmentFormEventArgs e) {
			DeleteRecurrentAppointmentFormEventHandler handler = (DeleteRecurrentAppointmentFormEventHandler)Events[onDeleteRecurrentAppointmentFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region EditRecurrentAppointmentFormShowing
		static readonly object onEditRecurrentAppointmentFormShowing = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlEditRecurrentAppointmentFormShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EditRecurrentAppointmentFormEventHandler EditRecurrentAppointmentFormShowing {
			add { Events.AddHandler(onEditRecurrentAppointmentFormShowing, value); }
			remove { Events.RemoveHandler(onEditRecurrentAppointmentFormShowing, value); }
		}
		protected internal virtual void RaiseEditRecurrentAppointmentFormShowing(EditRecurrentAppointmentFormEventArgs e) {
			EditRecurrentAppointmentFormEventHandler handler = (EditRecurrentAppointmentFormEventHandler)Events[onEditRecurrentAppointmentFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region CustomDrawAppointment
		internal static readonly object onCustomDrawAppointment = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawAppointment"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawAppointment {
			add { Events.AddHandler(onCustomDrawAppointment, value); }
			remove { Events.RemoveHandler(onCustomDrawAppointment, value); }
		}
		protected internal virtual void RaiseCustomDrawAppointment(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawAppointment, e);
		}
		#endregion
		#region CustomDrawAppointmentBackground
		internal static readonly object onCustomDrawAppointmentBackground = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawAppointmentBackground"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawAppointmentBackground {
			add { Events.AddHandler(onCustomDrawAppointmentBackground, value); }
			remove { Events.RemoveHandler(onCustomDrawAppointmentBackground, value); }
		}
		protected internal virtual void RaiseCustomDrawAppointmentBackground(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawAppointmentBackground, e);
		}
		#endregion
		#region CustomDrawTimeCell
		static readonly object onCustomDrawTimeCell = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawTimeCell"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawTimeCell {
			add { Events.AddHandler(onCustomDrawTimeCell, value); }
			remove { Events.RemoveHandler(onCustomDrawTimeCell, value); }
		}
		protected internal virtual void RaiseCustomDrawTimeCell(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawTimeCell, e);
		}
		#endregion
		#region CustomDrawDependency
		internal static readonly object onCustomDrawDependency = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawDependency"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDependency {
			add { Events.AddHandler(onCustomDrawDependency, value); }
			remove { Events.RemoveHandler(onCustomDrawDependency, value); }
		}
		protected internal virtual void RaiseCustomDrawDependency(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawDependency, e);
		}
		#endregion
		#region ISupportCustomDraw implementation
		void ISupportCustomDraw.RaiseCustomDrawTimeCell(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawTimeCell(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDayViewAllDayArea(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayViewAllDayArea(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawResourceHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawResourceHeader(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawGroupSeparator(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawGroupSeparator(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDayHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayHeader(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawWeekViewTopLeftCorner(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawWeekViewTopLeftCorner(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDayOfWeekHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayOfWeekHeader(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDayViewTimeRuler(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayViewTimeRuler(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawTimeIndicator(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawTimeIndicator(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawAppointmentBackground(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawAppointmentBackground(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawAppointment(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawAppointment(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDependency(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDependency(e);
		}
		#endregion
		#region CustomDrawDayHeader
		static readonly object onCustomDrawDayHeader = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawDayHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayHeader {
			add { Events.AddHandler(onCustomDrawDayHeader, value); }
			remove { Events.RemoveHandler(onCustomDrawDayHeader, value); }
		}
		protected internal virtual void RaiseCustomDrawDayHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawDayHeader, e);
		}
		#endregion
		#region CustomDrawDayOfWeekHeader
		static readonly object onCustomDrawDayOfWeekHeader = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawDayOfWeekHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayOfWeekHeader {
			add { Events.AddHandler(onCustomDrawDayOfWeekHeader, value); }
			remove { Events.RemoveHandler(onCustomDrawDayOfWeekHeader, value); }
		}
		protected internal virtual void RaiseCustomDrawDayOfWeekHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawDayOfWeekHeader, e);
		}
		#endregion
		#region CustomDrawResourceHeader
		static readonly object onCustomDrawResourceHeader = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawResourceHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawResourceHeader {
			add { Events.AddHandler(onCustomDrawResourceHeader, value); }
			remove { Events.RemoveHandler(onCustomDrawResourceHeader, value); }
		}
		protected internal virtual void RaiseCustomDrawResourceHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawResourceHeader, e);
		}
		#endregion
		#region CustomDrawGroupSeparator
		static readonly object onCustomDrawGroupSeparator = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawGroupSeparator"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawGroupSeparator {
			add { Events.AddHandler(onCustomDrawGroupSeparator, value); }
			remove { Events.RemoveHandler(onCustomDrawGroupSeparator, value); }
		}
		protected internal virtual void RaiseCustomDrawGroupSeparator(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawGroupSeparator, e);
		}
		#endregion
		#region CustomDrawDayViewTimeRuler
		static readonly object onCustomDrawDayViewTimeRuler = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawDayViewTimeRuler"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayViewTimeRuler {
			add { Events.AddHandler(onCustomDrawDayViewTimeRuler, value); }
			remove { Events.RemoveHandler(onCustomDrawDayViewTimeRuler, value); }
		}
		protected internal virtual void RaiseCustomDrawDayViewTimeRuler(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawDayViewTimeRuler, e);
		}
		#endregion
		#region CustomDrawTimeIndicator
		static readonly object onCustomDrawTimeIndicator = new object();
		[
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawTimeIndicator {
			add { Events.AddHandler(onCustomDrawTimeIndicator, value); }
			remove { Events.RemoveHandler(onCustomDrawTimeIndicator, value); }
		}
		protected internal virtual void RaiseCustomDrawTimeIndicator(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawTimeIndicator, e);
		}
		#endregion
		#region CustomDrawDayViewAllDayArea
		static readonly object onCustomDrawDayViewAllDayArea = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawDayViewAllDayArea"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayViewAllDayArea {
			add { Events.AddHandler(onCustomDrawDayViewAllDayArea, value); }
			remove { Events.RemoveHandler(onCustomDrawDayViewAllDayArea, value); }
		}
		protected internal virtual void RaiseCustomDrawDayViewAllDayArea(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawDayViewAllDayArea, e);
		}
		#endregion
		#region CustomDrawWeekViewTopLeftCorner
		static readonly object onCustomDrawWeekViewTopLeftCorner = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawWeekViewTopLeftCorner"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawWeekViewTopLeftCorner {
			add { Events.AddHandler(onCustomDrawWeekViewTopLeftCorner, value); }
			remove { Events.RemoveHandler(onCustomDrawWeekViewTopLeftCorner, value); }
		}
		protected internal virtual void RaiseCustomDrawWeekViewTopLeftCorner(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawWeekViewTopLeftCorner, e);
		}
		#endregion
		#region CustomDrawNavigationButton
		internal static readonly object onCustomDrawNavigationButton = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlCustomDrawNavigationButton"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawNavigationButton {
			add { Events.AddHandler(onCustomDrawNavigationButton, value); }
			remove { Events.RemoveHandler(onCustomDrawNavigationButton, value); }
		}
		protected internal virtual void RaiseCustomDrawNavigationButton(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(onCustomDrawNavigationButton, e);
		}
		#endregion
		#region BeforeDispose
		static readonly object onBeforeDispose = new object();
		internal event EventHandler BeforeDispose {
			add { Events.AddHandler(onBeforeDispose, value); }
			remove { Events.RemoveHandler(onBeforeDispose, value); }
		}
		protected internal virtual void RaiseBeforeDispose() {
			EventHandler handler = (EventHandler)Events[onBeforeDispose];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region BeforeLoadLayout
		static readonly object onBeforeLoadLayout = new object();
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerControlBeforeLoadLayout")]
#endif
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { Events.AddHandler(onBeforeLoadLayout, value); }
			remove { Events.RemoveHandler(onBeforeLoadLayout, value); }
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)Events[onBeforeLoadLayout];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region LayoutUpgrade
		static readonly object onLayoutUpgrade = new object();
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerControlLayoutUpgrade")]
#endif
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { Events.AddHandler(onLayoutUpgrade, value); }
			remove { Events.RemoveHandler(onLayoutUpgrade, value); }
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)Events[onLayoutUpgrade];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region DateNavigatorQueryActiveViewType
		internal static readonly object onDateNavigatorQueryActiveViewType = new object();
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerControlDateNavigatorQueryActiveViewType")]
#endif
		public event DateNavigatorQueryActiveViewTypeHandler DateNavigatorQueryActiveViewType {
			add { Events.AddHandler(onDateNavigatorQueryActiveViewType, value); }
			remove { Events.RemoveHandler(onDateNavigatorQueryActiveViewType, value); }
		}
		protected internal virtual SchedulerViewType RaiseDateNavigatorQueryActiveViewType(SchedulerViewType oldViewType, SchedulerViewType newViewType, DayIntervalCollection selectedDays) {
			DateNavigatorQueryActiveViewTypeHandler handler = Events[onDateNavigatorQueryActiveViewType] as DateNavigatorQueryActiveViewTypeHandler;
			if (handler == null)
				return newViewType;
			DateNavigatorQueryActiveViewTypeEventArgs e = new DateNavigatorQueryActiveViewTypeEventArgs(oldViewType, newViewType, selectedDays);
			handler(this, e);
			return e.NewViewType;
		}
		#endregion
		#region VisibleResourcesChanged
		public event VisibleResourcesChangedEventHandler VisibleResourcesChanged {
			add {
				if (InnerControl != null)
					InnerControl.VisibleResourcesChanged += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.VisibleResourcesChanged -= value;
			}
		}
		#endregion
		#region Refreshed
		static readonly object onRefreshed = new object();
		internal event EventHandler Refreshed {
			add { Events.AddHandler(onRefreshed, value); }
			remove { Events.RemoveHandler(onRefreshed, value); }
		}
		void RaiseRefreshed() {
			EventHandler handler = (EventHandler)Events[onRefreshed];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			this.isDisposing = true;
			try {
				if (disposing) {
					if (!IsDisposed)
						RaiseBeforeDispose();
					UnsubscribeSystemEvents();
					DisposeRemindersForm();
					this.serviceAccessor = null;
					if (rangeControlSupport != null) {
						UnsubscribeRangeControlSupportEvents();
						this.rangeControlSupport.Dispose();
						this.rangeControlSupport = null;
					}
					if (this.optionsRangeControl != null)
						this.optionsRangeControl = null;
					if (this.innerControl != null) {
						UnsubscribeInnerControlEvents();
						this.innerControl.Dispose();
						this.innerControl = null;
					}
					if (this.lookAndFeel != null) {
						UnsubscribeLookAndFeelEvents();
						this.lookAndFeel.Dispose();
						this.lookAndFeel = null;
					}
					if (this.paintStyles != null) {
						this.paintStyles.Dispose();
						this.paintStyles = null;
					}
					if (this.paintStyle != null) {
						this.paintStyle.Dispose();
						this.paintStyle = null;
					}
					if (this.animationManager != null) {
						this.animationManager.Dispose();
						this.animationManager = null;
					}
					ToolTipController = null;
					UnsubscribeToolTipControllerEvents(ToolTipController.DefaultController);
					UnregisterToolTipClientControl(ToolTipController.DefaultController);
					if (this.appearance != null) {
						UnsubscribeAppearanceEvents();
						appearance.Dispose();
						appearance = null;
					}
					this.optionsLayout = null;
					if (this.resourceScrollController != null)
						DestroyResourceScrollController();
					if (this.resourceNavigator != null) {
						UnsubscribeResourceNavigatorEvents();
						DestroyResourceNavigator();
					}
					if (this.dateTimeScrollController != null) {
						UnsubscribeDateTimeScrollControllerEvents();
						DestroyDateTimeScrollController();
					}
					if (this.dateTimeScrollBar != null)
						DestroyDateTimeScrollBar();
					if (this.appointmentImages != null) {
						UnsubscribeAppointmentImagesEvents();
						this.appointmentImages = null;
					}
					if (this.printer != null) {
						this.printer.Dispose();
						this.printer = null;
					}
					if (this.printStyles != null) {
						this.printStyles.DisposeCollectionElements();
						this.printStyles = null;
					}
					if (this.cellScrollBarsRegistrator != null) {
						this.cellScrollBarsRegistrator.Dispose();
						this.cellScrollBarsRegistrator = null;
					}
					this.menuManager = null;
					this.batchUpdateHelper = null;
				}
			} finally {
				base.Dispose(disposing);
				this.isDisposed = true;
				this.isDisposing = false;
			}
		}
		#endregion
		protected internal virtual void RaiseCustomDrawEvent(object cevent, CustomDrawObjectEventArgs e) {
			CustomDrawObjectEventHandler handler = (CustomDrawObjectEventHandler)this.Events[cevent];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void Initialize(SchedulerStorage storage) {
			this.mouseHelper = new MouseWheelScrollHelper(this);
			HandleCreated += OnSchedulerControlHandleCreated;
			GestureHelper = new Utils.Gesture.GestureHelper(this);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);
			printStyles = new SchedulerPrintStyleCollection();
			this.coloredSkinElementCache = new ColoredSkinElementCache();
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			InitSkinResourceColorSchemas();
			SubscribeLookAndFeelEvents();
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.paintStyles = new SchedulerPaintStyleCollection();
			RegisterPaintStyles();
			this.paintStyle = GetPaintStyle();
			this.appearance = new SchedulerAppearance();
			SubscribeAppearanceEvents();
			this.optionsLayout = CreateOptionsLayout();
			this.innerControl = CreateInnerControl();
			this.innerControl.Initialize();
			SubscribeInnerControlEvents();
			AddServices();
			this.serviceAccessor = new SchedulerServices(this.innerControl);
			this.animationManager = new EmptySchedulerAnimationManager();
			this.InnerControl.SetStorageCore(storage);
			this.dateTimeScrollBar = CreateDateTimeScrollBar();
			CreateDateTimeScrollController();
			SubscribeDateTimeScrollControllerEvents();
			CreateResourceScrollController();
			CreateResourceNavigator();
			SubscribeResourceNavigatorEvents();
			this.cellScrollBarsRegistrator = CreateCellScrollBarsRegistrator();
			AllowDrop = true;
			dragDropMode = DragDropMode.Standard;
			RegisterToolTipClientControl(ToolTipController.DefaultController); 
			optionsPrint = new SchedulerOptionsPrint();
			SubscribeSystemEvents();
		}
		protected internal ISupportInitialize GetRangeControlSupportInitialize() {
			return rangeControlSupport as ISupportInitialize;
		}
		protected virtual CellScrollBarsRegistrator CreateCellScrollBarsRegistrator() {
			return new CellScrollBarsRegistrator(this);
		}
		protected internal virtual InnerSchedulerControl CreateInnerControl() {
			return new WinFormsInnerSchedulerControl(this);
		}
		protected virtual WinFormsSchedulerMouseHandler CreateMouseHandler() {
			return new WinFormsSchedulerMouseHandler(this);
		}
		#region SubscribeLookAndFeelEvents
		protected internal virtual void SubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		#endregion
		#region UnsubscribeLookAndFeelEvents
		protected internal virtual void UnsubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
		}
		#endregion
		#region SubscribeInnerControlEvents
		protected internal virtual void SubscribeInnerControlEvents() {
			innerControl.AppointmentsSelectionChanged += new EventHandler(OnAppointmentsSelectionChanged);
			innerControl.AppointmentDependenciesSelectionChanged += new EventHandler(OnAppointmentDependenciesSelectionChanged);
			innerControl.StorageChanged += new EventHandler(OnStorageChanged);
			innerControl.ReminderAlert += new ReminderEventHandler(OnReminderAlert);
			innerControl.ActiveViewChanging += new InnerActiveViewChangingEventHandler(OnActiveViewChanging);
			innerControl.BeforeActiveViewChange += new EventHandler(OnBeforeActiveViewChange);
			innerControl.AfterActiveViewChange += new EventHandler(OnAfterActiveViewChange);
			innerControl.AfterApplyChanges += new AfterApplyChangesEventHandler(OnAfterApplyChanges);
			innerControl.LimitIntervalChanged += new EventHandler(OnLimitIntervalChanged);
		}
		#endregion
		#region UnsubscribeInnerControlEvents
		protected internal virtual void UnsubscribeInnerControlEvents() {
			innerControl.AppointmentsSelectionChanged -= new EventHandler(OnAppointmentsSelectionChanged);
			innerControl.AppointmentDependenciesSelectionChanged -= new EventHandler(OnAppointmentDependenciesSelectionChanged);
			innerControl.StorageChanged -= new EventHandler(OnStorageChanged);
			innerControl.ReminderAlert -= new ReminderEventHandler(OnReminderAlert);
			innerControl.ActiveViewChanging -= new InnerActiveViewChangingEventHandler(OnActiveViewChanging);
			innerControl.BeforeActiveViewChange -= new EventHandler(OnBeforeActiveViewChange);
			innerControl.AfterActiveViewChange -= new EventHandler(OnAfterActiveViewChange);
			innerControl.AfterApplyChanges -= new AfterApplyChangesEventHandler(OnAfterApplyChanges);
		}
		#endregion
		protected virtual void SubscribeRangeControlSupportEvents() {
			if (this.rangeControlSupport == null)
				return;
			this.rangeControlSupport.RangeControlAutoAdjusting += OnRangeControlAutoAdjusting;
		}
		protected virtual void UnsubscribeRangeControlSupportEvents() {
			ScaleBasedRangeControlClient schedulerRangeControlSupport = this.rangeControlSupport as ScaleBasedRangeControlClient;
			if (schedulerRangeControlSupport == null)
				return;
			schedulerRangeControlSupport.RangeControlAutoAdjusting -= OnRangeControlAutoAdjusting;
		}
		#region SubscribeAppearanceEvents
		protected internal virtual void SubscribeAppearanceEvents() {
			appearance.Changed += new EventHandler(OnAppearanceChanged);
		}
		#endregion
		#region UnsubscribeAppearanceEvents
		protected internal virtual void UnsubscribeAppearanceEvents() {
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
		}
		#endregion
		#region RegisterToolTipControllerClient
		protected internal virtual void RegisterToolTipClientControl(ToolTipController controller) {
			controller.AddClientControl(this);
		}
		#endregion
		#region UnregisterToolTipControllerClient
		protected internal virtual void UnregisterToolTipClientControl(ToolTipController controller) {
			controller.RemoveClientControl(this);
		}
		#endregion
		#region SubscribeToolTipControllerEvents
		protected internal virtual void SubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed += new EventHandler(OnToolTipControllerDisposed);
		}
		#endregion
		#region UnsubscribeToolTipControllerEvents
		protected internal virtual void UnsubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed -= new EventHandler(OnToolTipControllerDisposed);
		}
		#endregion
		protected internal virtual void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		[SecuritySafeCritical]
		protected internal virtual void SubscribeSystemEvents() {
			Microsoft.Win32.SystemEvents.TimeChanged += new EventHandler(OnSystemTimeChanged);
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
		}
		[SecuritySafeCritical]
		protected internal virtual void UnsubscribeSystemEvents() {
			Microsoft.Win32.SystemEvents.TimeChanged -= new EventHandler(OnSystemTimeChanged);
			Microsoft.Win32.SystemEvents.UserPreferenceChanged -= new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
		}
		protected internal virtual void OnUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			if (ActiveViewType == SchedulerViewType.Gantt || ActiveViewType == SchedulerViewType.Timeline)
				ApplyChanges(SchedulerControlChangeType.UserPreferenceChangedTimeline);
			else
				ApplyChanges(SchedulerControlChangeType.UserPreferenceChanged);
		}
		protected internal virtual void OnSystemTimeChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.SystemTimeChanged);
		}
		#region SubscribeDateTimeScrollControllerEvents
		protected internal virtual void SubscribeDateTimeScrollControllerEvents() {
			dateTimeScrollController.ScrollEvent += new DateTimeScrollEventHandler(OnDateTimeScroll);
		}
		#endregion
		#region UnsubscribeDateTimeScrollControllerEvents
		protected internal virtual void UnsubscribeDateTimeScrollControllerEvents() {
			dateTimeScrollController.ScrollEvent -= new DateTimeScrollEventHandler(OnDateTimeScroll);
		}
		#endregion
		#region SubscribeResourceNavigatorEvents
		protected internal virtual void SubscribeResourceNavigatorEvents() {
			resourceNavigator.ScrollValueChanged += new EventHandler(OnResourceNavigatorScrollValueChanged);
			resourceNavigator.VisibilityChanged += new EventHandler(OnResourceNavigatorVisibilityChanged);
			resourceNavigator.ScrollHappend += new ScrollEventHandler(OnResourceNavigatorScrollHappend);
		}
		#endregion
		#region UnsubscribeResourceNavigatorEvents
		protected internal virtual void UnsubscribeResourceNavigatorEvents() {
			resourceNavigator.ScrollValueChanged -= new EventHandler(OnResourceNavigatorScrollValueChanged);
			resourceNavigator.VisibilityChanged -= new EventHandler(OnResourceNavigatorVisibilityChanged);
			resourceNavigator.ScrollHappend -= new ScrollEventHandler(OnResourceNavigatorScrollHappend);
		}
		#endregion
		#region SubscribeAppointmentImagesEvents
		protected internal virtual void SubscribeAppointmentImagesEvents() {
			if (appointmentImages == null)
				return;
			ImageList imageList = appointmentImages as ImageList;
			if (imageList != null)
				imageList.Disposed += new EventHandler(OnAppointmentImagesDisposed);
			ImageCollection imageCollection = appointmentImages as ImageCollection;
			if (imageCollection != null)
				imageCollection.Disposed += new EventHandler(OnAppointmentImagesDisposed);
		}
		#endregion
		#region UnsubscribeAppointmentImagesEvents
		protected internal virtual void UnsubscribeAppointmentImagesEvents() {
			if (appointmentImages == null)
				return;
			ImageList imageList = appointmentImages as ImageList;
			if (imageList != null)
				imageList.Disposed -= new EventHandler(OnAppointmentImagesDisposed);
			ImageCollection imageCollection = appointmentImages as ImageCollection;
			if (imageCollection != null)
				imageCollection.Disposed -= new EventHandler(OnAppointmentImagesDisposed);
		}
		#endregion
		protected internal virtual void SubscribeAppointmentContentLayoutCalculatorEvents(AppointmentContentLayoutCalculator calculator) {
			if (onInitAppointmentImages != null)
				calculator.InitAppointmentImages += new AppointmentImagesEventHandler(this.OnInitAppointmentImages);
			if (onInitAppointmentDisplayText != null)
				calculator.InitAppointmentDisplayText += new AppointmentDisplayTextEventHandler(this.OnInitAppointmentDisplayText);
			if (onAppointmentViewInfoCustomizing != null)
				calculator.AppointmentViewInfoCustomizing += new AppointmentViewInfoCustomizingEventHandler(this.OnAppointmentViewInfoCustomizing);
		}
		protected internal virtual void UnsubscribeAppointmentContentLayoutCalculatorEvents(AppointmentContentLayoutCalculator calculator) {
			calculator.InitAppointmentImages -= new AppointmentImagesEventHandler(this.OnInitAppointmentImages);
			calculator.InitAppointmentDisplayText -= new AppointmentDisplayTextEventHandler(this.OnInitAppointmentDisplayText);
			calculator.AppointmentViewInfoCustomizing -= new AppointmentViewInfoCustomizingEventHandler(this.OnAppointmentViewInfoCustomizing);
		}
		protected internal virtual Color GetLabelColor(object labelId) {
			if (DataStorage == null)
				return Color.White;
			IAppointmentLabel label = DataStorage.Appointments.Labels.GetById(labelId);
			return label.GetColor();
		}
		protected internal virtual AppointmentStatus GetStatus(object statusId) {
			if (DataStorage == null)
				return AppointmentStatus.Empty;
			IAppointmentStatus status = DataStorage.Appointments.Statuses.GetById(statusId);
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject != null)
				return statusObject;
			return AppointmentStatus.Empty;
		}
		#region ISupportInitialize implementation
		public void BeginInit() {
			if (!DesignMode) {
				AnimationManager.SetRestrictions();
			} else
				AllowDrop = false;
			BeginUpdate();
			InnerControl.BeginInit();
			PrintStyles.DisposeCollectionElements();
			GanttView.Scales.Clear();
			OptionsRangeControl.Scales.Clear();
			ISupportInitialize rangeSupportInit = GetRangeControlSupportInitialize();
			if (rangeSupportInit != null)
				rangeSupportInit.BeginInit();
		}
		void OnSchedulerControlHandleCreated(object sender, EventArgs e) {
			if (!DesignMode) {
				this.animationManager = new SchedulerAnimationManager(this);
			}
		}
		public void EndInit() {
			InnerControl.EndInit();
			if (GanttView.Scales.Count <= 0)
				GanttView.Scales.LoadDefaults();
			if (OptionsRangeControl.Scales.Count <= 0)
				OptionsRangeControl.Scales.LoadDefaults();
			if (PrintStyles.Count <= 0)
				PrintStyles.LoadDefaults();
			CancelUpdate();
			ISupportInitialize rangeSupportInit = GetRangeControlSupportInitialize();
			if (rangeSupportInit != null)
				rangeSupportInit.EndInit();
			ApplyChanges(SchedulerControlChangeType.EndInit);
			if (DesignMode)
				OptionsRangeControl.AttachErrorProvider(new SchedulerOptionsErrorProvider());
			AnimationManager.RemoveRestrictions(true);
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			innerControl.BeginUpdate();
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			innerControl.EndUpdate();
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			innerControl.CancelUpdate();
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return innerControl.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		#region SuspendSelectionPaint / ResumeSelectionPaint implementation
		int suspendSelectionPaintCount;
		internal bool IsSelectionPaintLocked { get { return suspendSelectionPaintCount > 0; } }
		internal void SuspendSelectionPaint() {
			suspendSelectionPaintCount++;
		}
		internal void ResumeSelectionPaint() {
			if (IsSelectionPaintLocked) {
				suspendSelectionPaintCount--;
				if (!IsSelectionPaintLocked) {
					Invalidate();
					Update();
				}
			}
		}
		#endregion
		#region IToolTipControlClient implementation
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return CalculateToolTipInfo(point, OptionsView.ToolTipVisibility);
		}
		protected internal virtual ToolTipControlInfo CalculateToolTipInfo(Point point, ToolTipVisibility visibility) {
			return MouseHandler.CanShowToolTip ? ActiveView.CalculateToolTipInfo(point, visibility) : null;
		}
		#endregion
		#region ISupportAppointmentEdit implementation
		void ISupportAppointmentEdit.SelectNewAppointment(Appointment apt) {
			ActiveView.SelectAppointment(apt);
		}
		void ISupportAppointmentEdit.ShowAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType) {
			ShowEditAppointmentForm(apt, openRecurrenceForm, readOnly, commandSourceType);
		}
		void ISupportAppointmentEdit.BeginEditNewAppointment(Appointment apt) {
			IViewAsyncSupport viewAsyncSupport = ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport != null) {
				ActiveView.ThreadManager.WaitForAllThreads();
				viewAsyncSupport.ForceSyncMode();
			}
			AppointmentChangeHelper.BeginEditNewAppointment(apt);
		}
		void ISupportAppointmentEdit.RaiseInitNewAppointmentEvent(Appointment apt) {
			AppointmentEventArgs args = new AppointmentEventArgs(apt);
			RaiseInitNewAppointment(args);
		}
		#endregion
		#region ISupportAppointmentDependencyEdit
		void ISupportAppointmentDependencyEdit.ShowAppointmentDependencyForm(AppointmentDependency dep, bool readOnly, CommandSourceType commandSourceType) {
			ShowEditAppointmentDependencyForm(dep, readOnly, commandSourceType);
		}
		#endregion
		protected internal virtual void RegisterPaintStyles() {
			PaintStyles.Clear();
			PaintStyles.Add(new SchedulerFlatPaintStyle());
			PaintStyles.Add(new SchedulerUltraFlatPaintStyle());
			PaintStyles.Add(new SchedulerWindowsXPPaintStyle());
			PaintStyles.Add(new SchedulerOffice2003PaintStyle());
			PaintStyles.Add(new SchedulerSkinPaintStyle(LookAndFeel));
			int count = PaintStyles.Count;
			for (int i = 0; i < count; i++)
				PaintStyles[i].UserLookAndFeel.ParentLookAndFeel = this.LookAndFeel;
		}
		public SchedulerPaintStyle GetPaintStyle() {
			string styleName = GetActualPaintNameStyleName();
			SchedulerPaintStyle style = PaintStyles[styleName];
			if (style != null && !style.IsAvailable)
				style = null;
			return style != null ? style : PaintStyles[0];
		}
		internal string GetActualPaintNameStyleName() {
			string styleName = paintStyleName;
			if (styleName == DefaultPaintStyleName) {
				switch (lookAndFeel.ActiveStyle) {
					case ActiveLookAndFeelStyle.Office2003:
						styleName = "Office2003";
						break;
					case ActiveLookAndFeelStyle.Skin:
						styleName = "Skin";
						break;
					case ActiveLookAndFeelStyle.UltraFlat:
						styleName = "UltraFlat";
						break;
					case ActiveLookAndFeelStyle.WindowsXP:
						styleName = "WindowsXP";
						break;
					case ActiveLookAndFeelStyle.Flat:
					default:
						styleName = "Flat";
						break;
				}
			}
			return styleName;
		}
		public SchedulerColorSchemaCollection GetResourceColorSchemasCopy() {
			SchedulerColorSchemaCollection result = new SchedulerColorSchemaCollection();
			result.Assign(ActualResourceColorSchemas);
			return result;
		}
		protected internal virtual DateTimeScrollBar CreateDateTimeScrollBar() {
			return new DateTimeScrollBar(this);
		}
		protected internal virtual void DestroyDateTimeScrollBar() {
#if DEBUGTEST
			DevExpress.XtraEditors.ScrollBarBase scrollBar = dateTimeScrollBar.ScrollBar;
#endif
			this.dateTimeScrollBar.Dispose();
			this.dateTimeScrollBar = null;
#if DEBUGTEST
			XtraSchedulerDebug.Assert(Controls.Contains(scrollBar) == false);
#endif
		}
		protected internal virtual void CreateResourceScrollController() {
			this.resourceScrollController = CreateResourceScrollControllerCore();
		}
		protected internal virtual ResourceScrollController CreateResourceScrollControllerCore() {
			return new ResourceScrollController(ActiveView);
		}
		protected internal virtual void DestroyResourceScrollController() {
			this.resourceScrollController = null;
		}
		protected internal virtual void CreateResourceNavigator() {
			this.resourceNavigator = CreateResourceNavigatorCore();
			this.Controls.Add(resourceNavigator.NavigatorControl);
			resourceNavigator.NavigatorControl.CreateControl();
		}
		protected internal virtual ResourceNavigator CreateResourceNavigatorCore() {
			return new ResourceNavigator(this);
		}
		protected internal virtual void DestroyResourceNavigator() {
#if DEBUGTEST
			ResourceNavigatorControl resourceNavigatorControl = resourceNavigator.NavigatorControl;
#endif
			resourceNavigator.Dispose();
			resourceNavigator = null;
#if DEBUGTEST
			XtraSchedulerDebug.Assert(Controls.Contains(resourceNavigatorControl) == false);
#endif
		}
		protected internal virtual void UpdateResourceNavigatorToolTipController(ToolTipController controller) {
			ResourceNavigator.ToolTipController = controller;
		}
		protected internal virtual void CreateDateTimeScrollController() {
			this.dateTimeScrollController = ActiveView.CreateDateTimeScrollController();
		}
		protected internal virtual void DestroyDateTimeScrollController() {
			if (dateTimeScrollController != null) {
				dateTimeScrollController.Dispose();
				this.dateTimeScrollController = null;
			}
		}
		protected internal virtual void OnActiveViewChanging(object sender, InnerActiveViewChangingEventArgs e) {
			SchedulerViewBase newView = Views[e.NewView.Type];
			e.Cancel = !RaiseActiveViewChanging(newView);
		}
		protected internal virtual void OnAfterActiveViewChange(object sender, EventArgs e) {
			CreateDateTimeScrollController();
			CreateResourceScrollController();
		}
		protected internal virtual void OnBeforeActiveViewChange(object sender, EventArgs e) {
			DestroyResourceScrollController();
			UnsubscribeDateTimeScrollControllerEvents();
			DestroyDateTimeScrollController();
			DestroyViewInfoForActiveView();
		}
		internal void RecreateDateTimeScrollController() {
			UnsubscribeDateTimeScrollControllerEvents();
			DestroyDateTimeScrollController();
			CreateDateTimeScrollController();
			SubscribeDateTimeScrollControllerEvents();
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			InplaceEditController.OnSchedulerStorageChanged();
			RaiseStorageChanged();
		}
		protected internal virtual void OnLimitIntervalChanged(object sender, EventArgs e) {
			RaiseLimitIntervalChanged();
		}
		protected internal virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			InitSkinResourceColorSchemas();
			ApplyChanges(SchedulerControlChangeType.LookAndFeelChanged);
		}
		protected virtual void InitSkinResourceColorSchemas() {
			this.skinResourceColorSchemas = new SchedulerColorSchemaCollection();
			SchedulerColorSchemaCollection colors = SkinResourceColorSchemas;
			colors.LoadDefaults();
			if (LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
				return;
			colors.BeginUpdate();
			try {
				ColorSchemaTransformBase transform = GetColorSchemaSkinTransform();
				SetSchemaBaseColor(colors[0], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource01), transform);
				SetSchemaBaseColor(colors[1], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource02), transform);
				SetSchemaBaseColor(colors[2], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource03), transform);
				SetSchemaBaseColor(colors[3], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource04), transform);
				SetSchemaBaseColor(colors[4], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource05), transform);
				SetSchemaBaseColor(colors[5], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource06), transform);
				SetSchemaBaseColor(colors[6], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource07), transform);
				SetSchemaBaseColor(colors[7], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource08), transform);
				SetSchemaBaseColor(colors[8], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource09), transform);
				SetSchemaBaseColor(colors[9], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource10), transform);
				SetSchemaBaseColor(colors[10], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource11), transform);
				SetSchemaBaseColor(colors[11], SkinPainterHelper.GetResourceColorSchemaColor(LookAndFeel, SchedulerSkins.ColorResource12), transform);
			} finally {
				colors.CancelUpdate();
			}
		}
		protected virtual ColorSchemaTransformBase GetColorSchemaSkinTransform() {
			ColorSchemaTransformSkin transform = new ColorSchemaTransformSkin();
			int percent = SkinPainterHelper.GetSkinIntegerProperty(LookAndFeel, SchedulerSkins.ColorRatioCell);
			if (percent != 0)
				transform.SetCellPercent(percent);
			percent = SkinPainterHelper.GetSkinIntegerProperty(LookAndFeel, SchedulerSkins.ColorRatioCellBorder);
			if (percent != 0)
				transform.SetCellBorderPercent(percent);
			percent = SkinPainterHelper.GetSkinIntegerProperty(LookAndFeel, SchedulerSkins.ColorRatioCellBorderDark);
			if (percent != 0)
				transform.SetCellBorderDarkPercent(percent);
			percent = SkinPainterHelper.GetSkinIntegerProperty(LookAndFeel, SchedulerSkins.ColorRatioCellLight);
			if (percent != 0)
				transform.SetCellLightPercent(percent);
			percent = SkinPainterHelper.GetSkinIntegerProperty(LookAndFeel, SchedulerSkins.ColorRatioCellLightBorder);
			if (percent != 0)
				transform.SetCellLightBorderPercent(percent);
			percent = SkinPainterHelper.GetSkinIntegerProperty(LookAndFeel, SchedulerSkins.ColorRatioCellLightBorderDark);
			if (percent != 0)
				transform.SetCellLightBorderDarkPercent(percent);
			return transform;
		}
		protected void SetSchemaBaseColor(SchedulerColorSchema schema, Color color, ColorSchemaTransformBase ratio) {
			if (color != Color.Black)
				schema.SetBaseColor(color, ratio);
		}
		protected internal virtual void OnDateTimeScroll(object sender, DateTimeScrollEventArgs e) {
			if (IsDisposed)
				return;
			MouseHandler.OnDateTimeScroll();
		}
		protected internal virtual void OnResourceNavigatorScrollValueChanged(object sender, EventArgs e) {
			UnsubscribeResourceNavigatorEvents();
			try {
				resourceScrollController.ResourceNavigatorScrollValueChanged(resourceNavigator);
			} finally {
				SubscribeResourceNavigatorEvents();
			}
			ApplyChanges(SchedulerControlChangeType.ResourceScroll);
		}
		protected internal virtual void OnResourceNavigatorScrollHappend(object sender, ScrollEventArgs e) {
			resourceScrollController.ResourceNavigatorScrollHappend(e.Type);
		}
		protected internal virtual void OnResourceNavigatorVisibilityChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.ResourceNavigatorVisibilityChanged);
		}
		protected override void OnResize(EventArgs e) {
			if (this.isDisposing)
				return;
			base.OnResize(e);
			AnimationManager.CompleteAnimation();
			AnimationManager.SetRestrictions();
			try {
				ApplyChanges(SchedulerControlChangeType.BoundsChanged);
			} finally {
				AnimationManager.RemoveRestrictions(true);
			}
		}
		#region WndProc
		bool insideDblClick;
		bool resetDblClick;
		[SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override void WndProc(ref Message m) {
			if (GestureHelper.WndProc(ref m))
				return;
			const int WM_CONTEXTMENU = 0x7B;
			const int WM_LBUTTONDBLCLK = 0x0203;
			const int WM_KILLFOCUS = 0x0008;
			switch (m.Msg) {
				case WM_CONTEXTMENU:
					OnWmContextMenu((int)m.LParam);
					break;
				case WM_KILLFOCUS:
					if (insideDblClick)
						resetDblClick = true;
					break;
				case WM_LBUTTONDBLCLK:
					insideDblClick = true;
					break;
			}
			base.WndProc(ref m);
			UpdateDblClickState(m, WM_LBUTTONDBLCLK);
		}
		[SecuritySafeCritical]
		protected internal virtual void UpdateDblClickState(Message m, int WM_LBUTTONDBLCLK) {
			if (resetDblClick && m.Msg == WM_LBUTTONDBLCLK) {
				SetStateMethodInfo.Invoke(this, new object[] { 0x4000000, false });
				resetDblClick = false;
			}
			if (m.Msg == WM_LBUTTONDBLCLK)
				insideDblClick = false;
		}
		#endregion
		protected internal virtual Point CalculateContextMenuDefaultPosition() {
			if (SelectedAppointments.Count > 0) {
				AppointmentViewInfo appointmentViewInfo = SchedulerWinUtils.FindAppointmentViewInfoByAppointment(ActiveView.ViewInfo.CopyAllAppointmentViewInfos(), SelectedAppointments[0], ClientRectangle);
				if (appointmentViewInfo != null) {
					Rectangle intersectionBounds = Rectangle.Intersect(appointmentViewInfo.Bounds, ClientRectangle);
					return new Point(intersectionBounds.X + intersectionBounds.Width / 2, intersectionBounds.Y + intersectionBounds.Height / 2);
				}
			}
			return new Point(Width / 2, Height / 2);
		}
		protected internal virtual Point CalculateContextMenuPosition(int lParam) {
			if (lParam == -1)
				return CalculateContextMenuDefaultPosition();
			else
				return PointToClient(new Point((short)lParam, lParam >> 0x10));
		}
		protected internal virtual void OnWmContextMenu(int lParam) {
			Point pt = CalculateContextMenuPosition(lParam);
			if (ClientRectangle.Contains(pt))
				OnPopupMenu(pt);
		}
		protected internal virtual void OnPopupMenu(Point point) {
			bool canShowPopupMenu = MouseHandler.OnPopupMenu(point);
			if (!canShowPopupMenu)
				return;
			SchedulerPopupMenu menu = CreatePopupMenu(point);
			if (!MouseHandler.OnPopupMenuShowing())
				return;
			if (menu == null || menu.Items.Count <= 0)
				return;
			ISetSchedulerStateService setStateService = (ISetSchedulerStateService)GetService(typeof(ISetSchedulerStateService));
			if (setStateService != null) {
				setStateService.IsPopupMenuOpened = true;
				menu.CloseUp += new EventHandler(OnPopupMenuCloseUp);
			}
			this.Focus();
			MenuManagerHelper.ShowMenu(menu, paintStyle.UserLookAndFeel, MenuManager, this, point);
		}
		protected internal virtual void OnPopupMenuCloseUp(object sender, EventArgs e) {
			ISetSchedulerStateService setStateService = (ISetSchedulerStateService)GetService(typeof(ISetSchedulerStateService));
			if (setStateService != null)
				setStateService.IsPopupMenuOpened = false;
			SchedulerPopupMenu menu = (SchedulerPopupMenu)sender;
			menu.CloseUp -= new EventHandler(OnPopupMenuCloseUp);
		}
		protected internal virtual SchedulerPopupMenu CreatePopupMenu(Point point) {
			SchedulerHitInfo hitInfo = ActiveView.ViewInfo.CalculateHitInfo(point);
			SchedulerPopupMenuBuilder menuBuilder = CreatePopupMenuBuilder(hitInfo);
			SchedulerPopupMenu menu = (SchedulerPopupMenu)menuBuilder.CreatePopupMenu();
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			PrepareContextMenuEventArgs args = new PrepareContextMenuEventArgs(menu);
			RaisePopupMenuShowing(args);
			RaisePreparePopupMenu(args);
			RaisePrepareContextMenu(args);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			if (!Object.ReferenceEquals(menu, args.Menu)) {
				menu.Dispose();
				menu = args.Menu;
			}
			return menu;
		}
		protected internal virtual SchedulerPopupMenuBuilder CreatePopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return MouseHandler.CreatePopupMenuBuilder(hitInfo);
		}
		protected internal virtual void OnAppointmentsSelectionChanged(object sender, EventArgs e) {
			if (ActiveView != null && ActiveView.ViewInfo != null) {
				SchedulerViewCellContainerCollection cellContainers = ActiveView.ViewInfo.GetCellContainers();
				foreach (SchedulerViewCellContainer cellContainer in cellContainers)
					ActiveView.ViewInfo.UpdateAppointmentsSelection(cellContainer);
				Invalidate();
			}
		}
		protected internal virtual void OnAppointmentDependenciesSelectionChanged(object sender, EventArgs e) {
			GanttView view = ActiveView as GanttView;
			if (view == null)
				return;
			view.ViewInfo.UpdateDependenciesSelection(view.ViewInfo.DependencyViewInfos);
			Invalidate();
		}
		protected internal virtual void OnInitAppointmentImages(object sender, AppointmentImagesEventArgs e) {
			RaiseInitAppointmentImages(e);
		}
		protected internal virtual void OnInitAppointmentDisplayText(object sender, AppointmentDisplayTextEventArgs e) {
			RaiseInitAppointmentDisplayText(e);
		}
		protected internal virtual void OnAppointmentViewInfoCustomizing(object sender, AppointmentViewInfoCustomizingEventArgs e) {
			RaiseAppointmentViewInfoCustomizing(e);
		}
		protected internal virtual void OnRangeControlAutoAdjusting(object sender, RangeControlAdjustEventArgs e) {
			RaiseRangeControlAutoAdjusting(e);
		}
		protected internal virtual void ApplyChanges(SchedulerControlChangeType change) {
			innerControl.ApplyChanges(change);
		}
		protected internal virtual void ApplyChangesCore(SchedulerControlChangeType changeType, ChangeActions actions) {
			innerControl.ApplyChangesCore(changeType, actions);
		}
		protected internal virtual void OnAfterApplyChanges(object sender, AfterApplyChangesEventArgs e) {
			AnimationManager.SetRestrictions(e.ChangeTypes);
			try {
				Invalidate();
				Update();
			} finally {
				AnimationManager.RemoveRestrictions(false);
			}
		}
		protected internal virtual AppointmentChangeHelper CreateAppointmentChangeHelper(InnerSchedulerControl innerControl) {
			return new AppointmentChangeHelper(innerControl);
		}
		#region IInnerSchedulerControlOwner implementation
		ISupportAppointmentEdit IInnerSchedulerControlOwner.SupportAppointmentEdit { get { return this; } }
		ISupportAppointmentDependencyEdit IInnerSchedulerControlOwner.SupportAppointmentDependencyEdit { get { return this; } }
		AppointmentChangeHelper IInnerSchedulerControlOwner.CreateAppointmentChangeHelper(InnerSchedulerControl innerControl) {
			return CreateAppointmentChangeHelper(innerControl);
		}
		void IInnerSchedulerControlOwner.RecalcDraggingAppointmentPosition() {
		}
		NormalKeyboardHandlerBase IInnerSchedulerControlOwner.CreateKeyboardHandler() {
			return new NormalKeyboardHandler();
		}
		SchedulerMouseHandler IInnerSchedulerControlOwner.CreateMouseHandler(InnerSchedulerControl control) {
			return this.CreateMouseHandler();
		}
		SchedulerOptionsBehaviorBase IInnerSchedulerControlOwner.CreateOptionsBehavior() {
			return new SchedulerOptionsBehavior();
		}
		SchedulerOptionsViewBase IInnerSchedulerControlOwner.CreateOptionsView() {
			return new SchedulerOptionsView();
		}
		SchedulerViewRepositoryBase IInnerSchedulerControlOwner.CreateViewRepository() {
			return new SchedulerViewRepository();
		}
		ISchedulerInplaceEditController IInnerSchedulerControlOwner.CreateInplaceEditController() {
			return this.CreateInplaceEditController();
		}
		AppointmentSelectionController IInnerSchedulerControlOwner.CreateAppointmentSelectionController() {
			return this.CreateAppointmentSelectionController();
		}
		void IInnerSchedulerControlOwner.UpdatePaintStyle() {
			this.UpdatePaintStyle();
		}
		bool IInnerSchedulerControlOwner.IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return this.IsDateTimeScrollbarVisibilityDependsOnClientSize();
		}
		void IInnerSchedulerControlOwner.RecalcClientBounds() {
			this.RecalcClientBounds();
		}
		bool IInnerSchedulerControlOwner.ChangeResourceScrollBarOrientationIfNeeded() {
			return this.ChangeResourceScrollBarOrientationIfNeeded();
		}
		void IInnerSchedulerControlOwner.EnsureCalculationsAreFinished() {
			this.EnsureCalculationsAreFinished();
		}
		bool IInnerSchedulerControlOwner.ChangeDateTimeScrollBarOrientationIfNeeded() {
			return this.ChangeDateTimeScrollBarOrientationIfNeeded();
		}
		ISchedulerStorageBase IInnerSchedulerControlOwner.Storage { get { return DataStorage; } }
		bool IInnerSchedulerControlOwner.ChangeResourceScrollBarVisibilityIfNeeded() {
			return this.ChangeResourceScrollBarVisibilityIfNeeded();
		}
		bool IInnerSchedulerControlOwner.ChangeDateTimeScrollBarVisibilityIfNeeded() {
			return this.ChangeDateTimeScrollBarVisibilityIfNeeded();
		}
		event EventHandler ICommandAwareControl<SchedulerCommandId>.BeforeDispose {
			add {
				this.BeforeDispose += value;
			}
			remove {
				this.BeforeDispose -= value;
			}
		}
		void IInnerSchedulerControlOwner.RecalcViewBounds() {
			this.RecalcViewBounds();
		}
		void IInnerSchedulerControlOwner.RecalcScrollBarVisibility() {
			this.RecalcScrollBarVisibility();
		}
		void IInnerSchedulerControlOwner.RecreateViewInfo() {
			RecreateViewInfo();
		}
		void IInnerSchedulerControlOwner.RecalcFinalLayout() {
			this.RecalcFinalLayout();
		}
		void IInnerSchedulerControlOwner.RecalcPreliminaryLayout() {
			this.RecalcPreliminaryLayout();
		}
		void IInnerSchedulerControlOwner.ClearPreliminaryAppointmentsAndCellContainers() {
			ClearPreliminaryAppointmentsAndCellContainers();
		}
		void IInnerSchedulerControlOwner.RecalcAppointmentsLayout() {
		}
		bool IInnerSchedulerControlOwner.ObtainDateTimeScrollBarVisibility() {
			return this.ObtainDateTimeScrollBarVisibility();
		}
		void IInnerSchedulerControlOwner.UpdateScrollBarsPosition() {
			this.UpdateScrollBarsPosition();
		}
		void IInnerSchedulerControlOwner.UpdateDateTimeScrollBarValue() {
			this.UpdateDateTimeScrollBarValue();
		}
		void IInnerSchedulerControlOwner.UpdateResourceScrollBarValue() {
			this.UpdateResourceScrollBarValue();
		}
		void IInnerSchedulerControlOwner.RepaintView() {
			Invalidate();
			Update();
		}
		void IInnerSchedulerControlOwner.UpdateScrollMoreButtonsVisibility() {
			ActiveView.ViewInfo.UpdateScrollMoreButtonsVisibility();
		}
		void IInnerSchedulerControlOwner.ShowGotoDateForm(DateTime date) {
			this.ShowGotoDateForm(date);
		}
		ChangeActions IInnerSchedulerControlOwner.PrepareChangeActions() {
			return ActiveView.PrepareChangeActions();
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowDeleteRecurrentAppointmentForm(Appointment apt) {
			return this.ShowDeleteRecurrentAppointmentForm(apt);
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowDeleteRecurrentAppointmentsForm(AppointmentBaseCollection apts) {
			return OptionsBehavior.RecurrentAppointmentDeleteAction;
		}
		bool IInnerSchedulerControlOwner.QueryDeleteForEachRecurringAppointment { get { return true; } }
		#endregion
		#region SchedulerControlChangeManager support
		protected internal virtual bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return dateTimeScrollController.IsDateTimeScrollbarVisibilityDependsOnClientSize();
		}
		protected internal virtual void UpdatePaintStyle() {
			this.coloredSkinElementCache.Reset();
			this.paintStyle = GetPaintStyle();
			dateTimeScrollBar.UpdatePaintStyle();
			resourceNavigator.UpdatePaintStyle();
		}
		protected internal virtual void RecalcClientBounds() {
			BorderPainter painter = PaintStyle.CreateBorderPainter(BorderStyle);
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero))) {
				BorderObjectInfoArgs args = new BorderObjectInfoArgs(cache, ClientRectangle, null, ObjectState.Normal);
				this.clientBounds = painter.GetObjectClientRectangle(args);
			}
		}
		protected internal virtual bool ChangeResourceScrollBarOrientationIfNeeded() {
			return ActiveView.ChangeResourceScrollBarOrientationIfNeeded(resourceNavigator);
		}
		protected internal virtual bool ChangeDateTimeScrollBarOrientationIfNeeded() {
			return ActiveView.ChangeDateTimeScrollBarOrientationIfNeeded(dateTimeScrollBar);
		}
		protected internal virtual void EnsureCalculationsAreFinished() {
			ActiveView.EnsureCalculationsAreFinished();
		}
		protected internal virtual bool ChangeResourceScrollBarVisibilityIfNeeded() {
			bool oldVisibility = resourceNavigator.Visible;
			bool newVisibility = CalculateResourceScrollbarVisibility();
			resourceNavigator.Visible = newVisibility;
			return newVisibility != oldVisibility;
		}
		protected internal virtual bool CalculateResourceScrollbarVisibility() {
			return innerControl.CalculateResourceNavigatorVisibility(resourceNavigator.Visibility);
		}
		protected internal virtual bool ChangeDateTimeScrollBarVisibilityIfNeeded() {
			return dateTimeScrollController.ChangeDateTimeScrollBarVisibilityIfNeeded(dateTimeScrollBar);
		}
		protected internal virtual void RecalcViewBounds() {
			this.viewBounds = this.clientBounds;
			this.viewAndDateTimeScrollBarSeparatorBounds = Rectangle.Empty;
			this.viewAndResourceNavigatorSeparatorBounds = Rectangle.Empty;
			if (dateTimeScrollBar.Visible) {
				ViewPainterBase painter = CreateActiveViewPainter();
				if (dateTimeScrollBar.ScrollBarType == ScrollBarType.Vertical) {
					this.viewBounds.Width -= dateTimeScrollBar.ScrollBar.Width;
					int verticalSeparatorWidth = painter.ViewAndScrollbarVerticalSeparatorWidth;
					this.viewBounds.Width -= verticalSeparatorWidth;
					this.viewAndDateTimeScrollBarSeparatorBounds = new Rectangle(viewBounds.Right, viewBounds.Top, verticalSeparatorWidth, viewBounds.Height);
				} else {
					this.viewBounds.Height -= dateTimeScrollBar.ScrollBar.Height;
					int horizontalSeparatorHeight = painter.ViewAndScrollbarHorizontalSeparatorHeight;
					this.viewBounds.Height -= horizontalSeparatorHeight;
					this.viewAndDateTimeScrollBarSeparatorBounds = new Rectangle(viewBounds.X, viewBounds.Bottom, viewBounds.Width, horizontalSeparatorHeight);
				}
			}
			if (resourceNavigator.Visible) {
				ViewPainterBase painter = CreateActiveViewPainter();
				if (resourceNavigator.Vertical) {
					int resourceNavigatorWidth = resourceNavigator.NavigatorControl.CalcMinimalSize().Width;
					this.viewBounds.Width -= resourceNavigatorWidth;
					this.viewAndDateTimeScrollBarSeparatorBounds.Width -= resourceNavigatorWidth;
					int verticalSeparatorWidth = painter.ViewAndScrollbarVerticalSeparatorWidth;
					this.viewBounds.Width -= verticalSeparatorWidth;
					this.viewAndResourceNavigatorSeparatorBounds = new Rectangle(viewBounds.Right, viewBounds.Top, verticalSeparatorWidth, viewBounds.Height);
				} else {
					int resourceNavigatorHeight = resourceNavigator.NavigatorControl.CalcMinimalSize().Height;
					this.viewBounds.Height -= resourceNavigatorHeight;
					this.viewAndDateTimeScrollBarSeparatorBounds.Height -= resourceNavigatorHeight;
					int horizontalSeparatorHeight = painter.ViewAndScrollbarHorizontalSeparatorHeight;
					this.viewBounds.Height -= horizontalSeparatorHeight;
					this.viewAndResourceNavigatorSeparatorBounds = new Rectangle(viewBounds.X, viewBounds.Bottom, viewBounds.Width, horizontalSeparatorHeight);
				}
			}
		}
		protected internal virtual void RecalcScrollBarVisibility() {
			ActiveView.RecalcScrollBarVisibility();
		}
		void RecreateViewInfo() {
			ActiveView.RecreateViewInfo();
		}
		protected internal virtual void RecalcPreliminaryLayout() {
			ActiveView.RecalcPreliminaryLayout();
		}
		private void ClearPreliminaryAppointmentsAndCellContainers() {
			ActiveView.ClearPreliminaryAppointmentsAndCellContainers();
		}
		protected internal virtual void RecalcFinalLayout() {
			ActiveView.RecalcFinalLayout();
		}
		protected internal virtual bool ObtainDateTimeScrollBarVisibility() {
			return dateTimeScrollBar.Visible;
		}
		protected internal virtual void UpdateScrollBarsPosition() {
			Size cornerSize = CalculateCornerSize();
			if (dateTimeScrollBar.ScrollBarType == ScrollBarType.Vertical) {
				int scrollBarWidth = dateTimeScrollBar.ScrollBar.Width;
				int top = ActiveView.CalculateVerticalDateTimeScrollBarTop();
				Rectangle scrollBarBounds = Rectangle.FromLTRB(clientBounds.Right - scrollBarWidth, top, clientBounds.Right, clientBounds.Bottom - cornerSize.Height);
				dateTimeScrollBar.ScrollBar.Bounds = scrollBarBounds;
			} else {
				int scrollBarHeight = dateTimeScrollBar.ScrollBar.Height;
				Rectangle scrollBarBounds = new Rectangle(clientBounds.X, clientBounds.Bottom - scrollBarHeight, clientBounds.Width - cornerSize.Width, scrollBarHeight);
				dateTimeScrollBar.ScrollBar.Bounds = scrollBarBounds;
			}
			if (resourceNavigator.Vertical) {
				int resourceNavigatorWidth = cornerSize.Width;
				resourceNavigator.NavigatorControl.CornerSize = cornerSize.Height;
				Rectangle resourceNavigatorBounds = new Rectangle(clientBounds.Right - resourceNavigatorWidth, clientBounds.Y, resourceNavigatorWidth, clientBounds.Height);
				resourceNavigator.NavigatorControl.Bounds = resourceNavigatorBounds;
			} else {
				int resourceNavigatorHeight = cornerSize.Height;
				resourceNavigator.NavigatorControl.CornerSize = cornerSize.Width;
				Rectangle resourceNavigatorBounds = new Rectangle(clientBounds.X, clientBounds.Bottom - resourceNavigatorHeight, clientBounds.Width, resourceNavigatorHeight);
				resourceNavigator.NavigatorControl.Bounds = resourceNavigatorBounds;
			}
		}
		protected internal virtual Size CalculateCornerSize() {
			Size cornerSize = Size.Empty;
			if (dateTimeScrollBar.Visible) {
				if (dateTimeScrollBar.ScrollBarType == ScrollBarType.Vertical)
					cornerSize.Width = dateTimeScrollBar.ScrollBar.Width;
				else
					cornerSize.Height = dateTimeScrollBar.ScrollBar.Height;
			}
			if (resourceNavigator.Visible) {
				if (resourceNavigator.Vertical)
					cornerSize.Width = resourceNavigator.NavigatorControl.CalcMinimalSize().Width;
				else
					cornerSize.Height = resourceNavigator.NavigatorControl.CalcMinimalSize().Height;
			}
			return cornerSize;
		}
		protected internal virtual void UpdateDateTimeScrollBarValue() {
			dateTimeScrollController.UpdateScrollBarPosition();
		}
		protected internal virtual void UpdateResourceScrollBarValue() {
			UnsubscribeResourceNavigatorEvents();
			try {
				resourceScrollController.UpdateResourceNavigator(resourceNavigator);
				resourceNavigator.NavigatorControl.Navigator.Buttons.UpdateButtonsState();
			} finally {
				SubscribeResourceNavigatorEvents();
			}
		}
		#endregion
		protected virtual ScaleBasedRangeControlClient CreateScaleBasedRangeControlSupport(IRangeControlClientDataProvider dataProvider) {
			return new ScaleBasedRangeControlClient(dataProvider);
		}
		protected internal virtual ISchedulerInplaceEditController CreateInplaceEditController() {
			return new SchedulerInplaceEditControllerEx(InnerControl);
		}
		protected internal virtual AppointmentSelectionController CreateAppointmentSelectionController() {
			return new WinAppointmentSelectionController(this, OptionsCustomization.AllowAppointmentMultiSelect);
		}
		protected internal virtual AppointmentDependencySelectionController CreateAppointmentDependencySelectionController() {
			return new AppointmentDependencySelectionController( true);
		}
		protected internal virtual bool IsResourceNavigatorActionEnabled(NavigatorButtonType type) {
			return resourceScrollController.IsResourceNavigatorActionEnabled(type);
		}
		protected internal virtual void ExecuteResourceNavigatorAction(NavigatorButtonType type) {
			resourceScrollController.ExecuteResourceNavigatorAction(type);
		}
		protected internal virtual void CreateViewInfoForActiveView() {
			ActiveView.CreateViewInfo();
		}
		protected internal virtual void DestroyViewInfoForActiveView() {
			ActiveView.DestroyViewInfo();
		}
		protected internal virtual void OnAppearanceChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.AppearanceChanged);
		}
		protected internal virtual void OnAppointmentImagesDisposed(object sender, EventArgs e) {
			UnsubscribeAppointmentImagesEvents();
			appointmentImages = null;
		}
		protected internal virtual ViewPainterBase CreateActiveViewPainter() {
			ViewPainterBase painter = this.ActiveView.CreatePainter(PaintStyle);
			painter.HideSelection = IsSelectionPaintLocked || (OptionsView.HideSelection && !this.Focused) || SelectedAppointments.Count > 0 || SelectedDependencies.Count > 0 || AppointmentChangeHelper.HideSelection;
			return painter;
		}
		internal Bitmap CreateControlBitmap(Bitmap bmp) {
			AnimationManager.Lock();
			try {
				if (bmp == null || bmp.Size != Size) {
					bmp = new Bitmap(Math.Max(1, Width), Math.Max(1, Height));
				}
				if (bmp.Size == Size)
					DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
				return bmp;
			} finally {
				AnimationManager.Unlock();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if (InnerIsDisposing || InnerIsDisposed || ActiveView.ViewInfo == null)
				return;
			if (AnimationManager.Paint(e))
				return;
			PaintCore(e);
		}
		protected internal void PaintCore(PaintEventArgs e) {
			GraphicsInfoArgs info = new GraphicsInfoArgs(new GraphicsCache(e), this.Bounds);
			GraphicsCache cache = info.Cache;
			try {
				BorderPainter borderPainter = PaintStyle.CreateBorderPainter(BorderStyle);
				borderPainter.DrawObject(new BorderObjectInfoArgs(cache, ClientRectangle, null, ObjectState.Normal));
				ViewPainterBase painter = CreateActiveViewPainter();
				painter.DrawViewAndScrollBarSeparator(cache, this.ViewAndDateTimeScrollBarSeparatorBounds);
				painter.DrawViewAndScrollBarSeparator(cache, this.ViewAndResourceNavigatorSeparatorBounds);
				using (IntersectClipper clipper = new IntersectClipper(cache, ViewBounds)) {
					painter.Draw(info, this.ActiveView.ViewInfo);
				}
			} finally {
				cache.Dispose();
			}
			base.OnPaint(e);
		}
		protected override bool IsInputKey(Keys keyData) {
			if ((keyData & Keys.Escape) == Keys.Escape)
				return false;
			return true;
		}
		[SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override bool ProcessDialogChar(char charCode) {
			if (Control.ModifierKeys == Keys.None && OptionsCustomization.AllowInplaceEditor != UsedAppointmentType.None)
				return false;
			else
				return base.ProcessDialogChar(charCode);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null)
				svc.OnKeyDown(e);
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null)
				svc.OnKeyUp(e);
			base.OnKeyUp(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null)
				svc.OnKeyPress(e);
			base.OnKeyPress(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if (DesignMode) {
				base.OnDragEnter(e);
				return;
			}
			MouseHandler.OnDragEnter(e);
			base.OnDragEnter(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if (DesignMode) {
				base.OnDragOver(e);
				return;
			}
			MouseHandler.OnDragOver(e);
			base.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if (DesignMode) {
				base.OnDragDrop(e);
				return;
			}
			MouseHandler.OnDragDrop(e);
			base.OnDragDrop(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			if (DesignMode) {
				base.OnDragLeave(e);
				return;
			}
			MouseHandler.OnDragLeave(e);
			base.OnDragLeave(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			MouseHandler.OnGiveFeedback(e);
			base.OnGiveFeedback(e);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			MouseHandler.OnQueryContinueDrag(e);
			base.OnQueryContinueDrag(e);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			ISetSchedulerStateService service = GetService<ISetSchedulerStateService>();
			if (service != null && service.IsApplyChanges)
				return;
			if (OptionsView != null && OptionsView.HideSelection) {
				Invalidate();
				Update();
			}
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if (OptionsView != null && OptionsView.HideSelection) {
				Invalidate();
				Update();
			}
		}
		[Obsolete("This method is obsolete now", false), EditorBrowsable(EditorBrowsableState.Never)]
		public ToolTipController GetToolTipController() {
			return ActualToolTipController;
		}
		public virtual void GoToToday() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToToday();
		}
		public virtual void GoToDate(DateTime date) {
			IDateTimeNavigationService service = (IDateTimeNavigationService)GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date);
		}
		public virtual void GoToDate(DateTime date, SchedulerViewType viewType) {
			IDateTimeNavigationService service = (IDateTimeNavigationService)GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date, viewType);
		}
		public DialogResult ShowGotoDateForm() {
			return ShowGotoDateFormCore(this, Selection.Interval.Start.Date);
		}
		public DialogResult ShowGotoDateForm(IWin32Window parent) {
			return ShowGotoDateFormCore(parent, Selection.Interval.Start.Date);
		}
		protected internal DialogResult ShowGotoDateForm(DateTime date) {
			return ShowGotoDateFormCore(this, date);
		}
		protected internal virtual DialogResult ShowGotoDateFormCore(IWin32Window parent, DateTime date) {
			DialogResult result = DialogResult.OK;
			DateTime targetDate = date;
			SchedulerViewType targetViewType = ActiveViewType;
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				GotoDateFormEventArgs args = new GotoDateFormEventArgs();
				args.SchedulerViewType = targetViewType;
				args.Date = targetDate;
				args.Parent = parent;
				RaiseGotoDateFormShowing(args);
				if (args.Handled) {
					targetDate = args.Date;
					targetViewType = args.SchedulerViewType;
					result = args.DialogResult;
				} else {
					using (GotoDateForm form = new GotoDateForm(Views, args.Date, args.SchedulerViewType)) {
						form.SetMenuManager(MenuManager);
						result = registrator.ShowForm(form, parent);
						targetDate = form.Date;
						targetViewType = form.TargetView;
					}
				}
			}
			if (result == DialogResult.OK)
				GoToDate(targetDate, targetViewType);
			return result;
		}
		public virtual DialogResult ShowEditAppointmentForm(Appointment apt) {
			return ShowEditAppointmentForm(apt, false);
		}
		public virtual DialogResult ShowEditAppointmentForm(Appointment apt, bool openRecurrenceForm) {
			return ShowEditAppointmentForm(apt, openRecurrenceForm, false);
		}
		public virtual DialogResult ShowEditAppointmentDependencyForm(AppointmentDependency dependency) {
			return ShowEditAppointmentDependencyForm(dependency, false);
		}
		public virtual DialogResult ShowEditAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly) {
			return ShowEditAppointmentForm(apt, openRecurrenceForm, readOnly, CommandSourceType.Unknown);
		}
		protected internal virtual DialogResult ShowEditAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType) {
			DialogResult result = DialogResult.None;
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				result = ShowEditAppointmentFormCore(apt, openRecurrenceForm, readOnly, commandSourceType, registrator);
			}
			if (ShowAnotherEditAppointmentForm && this.SelectedAppointments.Count > 0) {
				ShowAnotherEditAppointmentForm = false;
				ShowEditAppointmentForm(this.SelectedAppointments[0]);
			}
			return result;
		}
		public virtual DialogResult ShowEditAppointmentDependencyForm(AppointmentDependency dependency, bool readOnly, CommandSourceType commandSourceType) {
			if (dependency == null)
				Exceptions.ThrowArgumentException("dependency", dependency);
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				return ShowEditAppointmentDependencyFormCore(dependency, readOnly, commandSourceType, registrator);
			}
		}
		public virtual DialogResult ShowEditAppointmentDependencyForm(AppointmentDependency dependency, bool readOnly) {
			return ShowEditAppointmentDependencyForm(dependency, readOnly, CommandSourceType.Unknown);
		}
		protected internal virtual DialogResult ShowEditAppointmentDependencyFormCore(AppointmentDependency dep, bool readOnly, CommandSourceType commandSourceType, ModalFormShowingRegistrator registrator) {
			DialogResult result;
			AppointmentDependencyFormEventArgs e = new AppointmentDependencyFormEventArgs(dep, commandSourceType, readOnly);
			e.Parent = this;
			RaiseEditAppointmentDependencyFormShowing(e);
			if (!e.Handled) {
				AppointmentDependencyForm form = CreateAppointmentDependencyForm(this, dep);
				form.ReadOnly = readOnly;
				result = registrator.ShowForm(form, this);
				form.Dispose();
			} else
				result = e.DialogResult;
			return result;
		}
		protected internal virtual DialogResult ShowEditAppointmentFormCore(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType, ModalFormShowingRegistrator registrator) {
			DialogResult result;
			AppointmentFormEventArgs e = new AppointmentFormEventArgs(apt, commandSourceType, readOnly, openRecurrenceForm);
			e.Parent = this;
			RaiseEditAppointmentFormShowing(e);
			if (!e.Handled) {
				AppointmentRibbonForm form = CreateAppointmentForm(this, apt, openRecurrenceForm);
				if (aptFormBounds != Rectangle.Empty) {
					form.StartPosition = FormStartPosition.Manual;
					form.Bounds = aptFormBounds;
				}
				form.ReadOnly = readOnly;
				form.SetMenuManager(MenuManager);
				result = registrator.ShowForm(form, this);
				aptFormBounds = form.Bounds;
				form.Dispose();
			} else
				result = e.DialogResult;
			return result;
		}
		protected internal virtual AppointmentRibbonForm CreateAppointmentForm(SchedulerControl control, Appointment apt, bool openRecurrenceForm) {
			return new AppointmentRibbonForm(control, apt, openRecurrenceForm);
		}
		protected internal virtual AppointmentDependencyForm CreateAppointmentDependencyForm(SchedulerControl control, AppointmentDependency dep) {
			return new AppointmentDependencyForm(control, dep);
		}
		protected internal virtual void ShowCustomizeTimeRulerForm(TimeRuler ruler) {
			if (ruler == null)
				Exceptions.ThrowArgumentException("ruler", ruler);
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				using (TimeRulerForm form = new TimeRulerForm()) {
					form.TimeRuler = ruler;
					form.SetMenuManager(MenuManager);
					registrator.ShowForm(form, this);
				}
			}
		}
		public virtual void DeleteSelectedAppointments() {
			DeleteAppointmentsQueryCommand command = new DeleteAppointmentsQueryCommand(InnerControl, SelectedAppointments);
			command.Execute();
		}
		public void CreateNewAppointment() {
			NewAppointmentCommand command = new NewAppointmentCommand(InnerControl);
			command.Execute();
		}
		public void CreateNewAllDayEvent() {
			NewAllDayAppointmentCommand command = new NewAllDayAppointmentCommand(InnerControl);
			command.Execute();
		}
		public void CreateNewRecurringAppointment() {
			NewRecurringAppointmentCommand command = new NewRecurringAppointmentCommand(InnerControl);
			command.Execute();
		}
		public void CreateNewRecurringEvent() {
			NewRecurringAllDayAppointmentCommand command = new NewRecurringAllDayAppointmentCommand(InnerControl);
			command.Execute();
		}
		public void CreateAppointment(bool allDay, bool recurring) {
			NewAppointmentCommandBase command;
			if (allDay) {
				if (recurring)
					command = new NewRecurringAllDayAppointmentCommand(InnerControl);
				else
					command = new NewAllDayAppointmentCommand(InnerControl);
			} else {
				if (recurring)
					command = new NewRecurringAppointmentCommand(InnerControl);
				else
					command = new NewAppointmentCommand(InnerControl);
			}
			command.Execute();
		}
		public void DeleteAppointment(Appointment apt) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.Add(apt);
			DeleteAppointmentsQueryCommand command = new DeleteAppointmentsQueryCommand(InnerControl, appointments);
			command.Execute();
		}
		[Obsolete("You should use the 'QueryDeleteAppointmentResult ShowRecurrentAppointmentDeleteForm(Appointment apt)' instead", true)]
		public virtual DialogResult ShowRecurrentAppointmentDeleteForm(Appointment apt, ref bool deleteSeries) {
			switch (ShowRecurrentAppointmentDeleteForm(apt)) {
				case QueryDeleteAppointmentResult.Series:
					deleteSeries = true;
					return DialogResult.OK;
				case QueryDeleteAppointmentResult.Occurrence:
					deleteSeries = false;
					return DialogResult.OK;
				case QueryDeleteAppointmentResult.Cancel:
				default:
					deleteSeries = false;
					return DialogResult.Cancel;
			}
		}
		[Obsolete("You should use the 'RecurrentAppointmentAction ShowDeleteRecurrentAppointmentForm(Appointment apt)' instead", true)]
		public virtual QueryDeleteAppointmentResult ShowRecurrentAppointmentDeleteForm(Appointment apt) {
			return (QueryDeleteAppointmentResult)ShowDeleteRecurrentAppointmentForm(apt);
		}
		public virtual RecurrentAppointmentAction ShowDeleteRecurrentAppointmentForm(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			DeleteRecurrentAppointmentFormEventArgs e = new DeleteRecurrentAppointmentFormEventArgs(apt);
			RaiseDeleteRecurrentAppointmentFormShowing(e);
			if (e.Handled)
				return CalculateRecurrentAppointmentAction(e.DialogResult, e.QueryResult);
			using (RecurrentAppointmentDeleteForm form = new RecurrentAppointmentDeleteForm(apt)) {
				DialogResult dialogResult = ShowDeleteRecurrentAppointmentFormCore(form);
				return CalculateRecurrentAppointmentAction(dialogResult, form.QueryResult);
			}
		}
		void IInnerSchedulerControlOwner.ShowPrintPreview() {
			ShowPrintPreview(ActivePrintStyle);
		}
		public virtual RecurrentAppointmentAction ShowEditRecurrentAppointmentForm(Appointment apt) {
			return ShowEditRecurrentAppointmentFormCore(apt, false, CommandSourceType.Unknown);
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowEditRecurrentAppointmentForm(Appointment apt, bool readOnly, CommandSourceType sourceType) {
			return ShowEditRecurrentAppointmentFormCore(apt, readOnly, sourceType);
		}
		protected internal virtual RecurrentAppointmentAction ShowEditRecurrentAppointmentFormCore(Appointment apt, bool readOnly, CommandSourceType sourceType) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			EditRecurrentAppointmentFormEventArgs e = new EditRecurrentAppointmentFormEventArgs(apt, sourceType, false, false);
			RaiseEditRecurrentAppointmentFormShowing(e);
			if (e.Handled)
				return CalculateRecurrentAppointmentAction(e.DialogResult, e.QueryResult);
			using (RecurrentAppointmentEditForm form = new RecurrentAppointmentEditForm(apt)) {
				DialogResult dialogResult = ShowEditRecurrentAppointmentFormCore(form);
				return CalculateRecurrentAppointmentAction(dialogResult, form.QueryResult);
			}
		}
		protected internal virtual DialogResult ShowDeleteRecurrentAppointmentFormCore(RecurrentAppointmentDeleteForm form) {
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				return registrator.ShowForm(form, this);
			}
		}
		protected internal virtual DialogResult ShowEditRecurrentAppointmentFormCore(RecurrentAppointmentEditForm form) {
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				return registrator.ShowForm(form, this);
			}
		}
		protected internal virtual RecurrentAppointmentAction CalculateRecurrentAppointmentAction(DialogResult dialogResult, RecurrentAppointmentAction queryResult) {
			return dialogResult == DialogResult.OK ? queryResult : RecurrentAppointmentAction.Cancel;
		}
		#region Reminder Form Support
		protected internal virtual void OnReminderAlert(object sender, ReminderEventArgs e) {
			if (OptionsBehavior.ShowRemindersForm) {
				RemindersFormEventArgs args = new RemindersFormEventArgs(e.AlertNotifications);
				RaiseRemindersFormShowing(args);
				if (!args.Handled) {
					if (remindersForm == null) {
						remindersForm = new RemindersForm(this);
						SubscribeRemindersFormEvents();
					}
					if (remindersForm != null)
						remindersForm.OnReminderAlert(e);
				} else {
					if (remindersForm != null)
						DisposeRemindersForm();
				}
			}
		}
		protected internal virtual void SubscribeRemindersFormEvents() {
			remindersForm.Closed += new EventHandler(OnRemindersFormClosed);
		}
		protected internal virtual void UnsubscribeRemindersFormEvents() {
			remindersForm.Closed -= new EventHandler(OnRemindersFormClosed);
		}
		protected internal virtual void OnRemindersFormClosed(object sender, EventArgs e) {
			DisposeRemindersForm();
		}
		protected internal virtual void DisposeRemindersForm() {
			if (remindersForm != null) {
				UnsubscribeRemindersFormEvents();
				remindersForm.Dispose();
				remindersForm = null;
			}
		}
		#endregion
		protected virtual PrintingSystemBase GetPrintingSystem(SchedulerPrinter printer) {
			return printer.PrintingSystemBase;
		}
		void ApplyPrintStylePageSettings(SchedulerPrinter printer) {
			PrintingSystemBase printingSystem = printer.PrintingSystemBase;
			if (printingSystem == null)
				return;
			if (System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count > 0) {
				try {
					XtraPageSettingsBase xPageSettings = printingSystem.PageSettings; 
					SchedulerPrintStyle printStyle = printer.PrintStyle;
					xPageSettings.PaperName = printStyle.PageSettings.PaperSize.PaperName;
					printer.SetPageSettings(printStyle.PageSettings);
					ApplyPageSettings(printStyle.PageSettings, printingSystem);
				} catch (System.Drawing.Printing.InvalidPrinterException) {
				}
			}
		}
		protected virtual void ApplyPageSettings(PageSettings settings, PrintingSystemBase printingSystem) {
			if (!ApplyPageSettingsCore(settings, printingSystem))
				printingSystem.PageSettings.Assign(settings.Margins, settings.PaperSize.Kind, settings.PaperSize.PaperName, settings.Landscape);
		}
		protected bool ApplyPageSettingsCore(PageSettings settings, PrintingSystemBase printingSystem) {
			Size customSize = CalculateCustomPageSize(settings);
			return XtraPageSettingsBase.ApplyPageSettings(printingSystem.PageSettings, settings.PaperSize.Kind, customSize, settings.Margins, XtraPageSettingsBase.DefaultMinMargins, settings.Landscape);
		}
		protected internal virtual Size CalculateCustomPageSize(PageSettings pageSettings) {
			if (pageSettings.PaperSize.Kind == PaperKind.Custom && pageSettings.PaperSize.Width != 0 && pageSettings.PaperSize.Height != 0)
				return new Size(pageSettings.PaperSize.Width, pageSettings.PaperSize.Height);
			else
				return Size.Empty;
		}
		bool CanPrintStyle(SchedulerPrintStyle printStyle) {
			if (!(printStyle is MemoPrintStyle))
				return true;
			if (SelectedAppointments.Count == 0) {
				XtraMessageBox.Show(DevExpress.XtraScheduler.Localization.SchedulerLocalizer.GetString(DevExpress.XtraScheduler.Localization.SchedulerStringId.Msg_MemoPrintNoSelectedItems), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}
			return true;
		}
		public void ShowPrintPreview() {
			ShowPrintPreview(ActivePrintStyle);
		}
		public void ShowPrintPreview(SchedulerPrintStyle printStyle) {
			RecreateSchedulerPrinter(printStyle);
			ShowPrintPreviewCore(Printer);
		}
		protected internal virtual Form ShowPrintPreviewCore(SchedulerPrinter printer) {
			Form form = null;
			ExecuteSchedulerPrinterAction(printer, delegate () {
				form = printer.ShowPreview(this.LookAndFeel);
			});
			return form;
		}
		protected internal virtual void CreateDocument(SchedulerPrinter printer) {
			ExecuteSchedulerPrinterAction(printer, delegate () {
				printer.CreateDocument();
			});
		}
		public void Print() {
			Print(ActivePrintStyle);
		}
		public void Print(SchedulerPrintStyle printStyle) {
			RecreateSchedulerPrinter(printStyle);
			PrintCore(Printer);
		}
		protected internal virtual void PrintCore(SchedulerPrinter printer) {
			ExecuteSchedulerPrinterAction(printer, delegate () { printer.Print(); });
		}
		protected void ExecuteSchedulerPrinterAction(SchedulerPrinter printer, Action0 action) {
			ApplyDefaultRange(printer.PrintStyle);
			ApplyPrintStylePageSettings(printer);
			if (CanPrintStyle(printer.PrintStyle)) {
				action();
			}
		}
		void ApplyDefaultRange(SchedulerPrintStyle printStyle) {
			PrintStyleWithDateRange printStyleWithDateRange = printStyle as PrintStyleWithDateRange;
			if (printStyleWithDateRange != null && printStyleWithDateRange.UseDefaultRange) {
				TimeIntervalCollection visibleIntervals = ActiveView.GetVisibleIntervals();
				printStyleWithDateRange.SetDefaultRange(visibleIntervals[0].Start.Date,
					visibleIntervals[visibleIntervals.Count - 1].Start.Date);
			}
		}
		public void ShowPrintOptionsForm() {
			using (ModalFormShowingRegistrator registrator = new ModalFormShowingRegistrator(this)) {
				using (PageSetupForm form = new PageSetupForm(this)) {
					form.SetMenuManager(MenuManager);
					registrator.ShowForm(form, this);
				}
			}
		}
		#region IPrintable implementation
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			RecreateSchedulerPrinter(ActivePrintStyle);
			XtraSchedulerDebug.Assert(Object.ReferenceEquals(ActivePrintStyle, printer.PrintStyle));
			ApplyDefaultRange(printer.PrintStyle);
			ApplyPrintStylePageSettings(printer);
			printer.Initialize(ps, link);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			this.printer.Finalize(ps, link);
			this.printer.Dispose();
			this.printer = null;
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graphics) {
			printer.CreateArea(areaName, graphics);
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return printer.CreatesIntersectedBricks; }
		}
		void IPrintable.AcceptChanges() {
			printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl { get { return printer.PropertyEditorControl; } }
		#endregion
		void ResetOptions() {
			OptionsView.Reset();
			OptionsBehavior.Reset();
			OptionsCustomization.Reset();
		}
		#region Xtra Serialization
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		protected internal virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		protected internal virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			this.GroupType = SchedulerGroupType.None;
			this.ActiveViewType = SchedulerViewType.Day;
			ResetOptions();
			Views.Reset();
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OnStartDeserializingCore(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			OnEndDeserializingCore(restoredVersion);
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		protected internal virtual void OnStartDeserializingCore(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if (!e.Allow)
				return;
			BeginUpdate();
		}
		protected internal virtual void OnEndDeserializingCore(string restoredVersion) {
			if (restoredVersion != OptionsLayout.LayoutVersion)
				RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
			EndUpdate();
		}
		protected internal virtual OptionsLayoutBase CreateOptionsLayout() {
			return new OptionsLayoutBase();
		}
		public void SaveLayoutToXml(string xmlFile) {
			SaveLayoutToXml(xmlFile, OptionsLayout);
		}
		public void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutFromXml(xmlFile, OptionsLayout);
		}
		public void SaveLayoutToRegistry(string path) {
			SaveLayoutToRegistry(path, OptionsLayout);
		}
		public void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutFromRegistry(path, OptionsLayout);
		}
		public void SaveLayoutToStream(Stream stream) {
			SaveLayoutToStream(stream, OptionsLayout);
		}
		public void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutFromStream(stream, OptionsLayout);
		}
		internal void SaveLayoutToXml(string xmlFile, OptionsLayoutBase options) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.SerializeObject(this, xmlFile, XtraSchedulerAppName, options);
		}
		internal void RestoreLayoutFromXml(string xmlFile, OptionsLayoutBase options) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.DeserializeObject(this, xmlFile, XtraSchedulerAppName, options);
		}
		internal void SaveLayoutToRegistry(string path, OptionsLayoutBase options) {
			RegistryXtraSerializer serializer = new RegistryXtraSerializer();
			serializer.SerializeObject(this, path, XtraSchedulerAppName, options);
		}
		internal void RestoreLayoutFromRegistry(string path, OptionsLayoutBase options) {
			RegistryXtraSerializer serializer = new RegistryXtraSerializer();
			serializer.DeserializeObject(this, path, XtraSchedulerAppName, options);
		}
		internal void SaveLayoutToStream(Stream stream, OptionsLayoutBase options) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.SerializeObject(this, stream, XtraSchedulerAppName, options);
		}
		internal void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.DeserializeObject(this, stream, XtraSchedulerAppName, options);
		}
		#endregion
		public virtual void RefreshData() {
			if (DataStorage == null)
				return;
			BeginUpdate();
			try {
				DataStorage.RefreshData();
			} finally {
				EndUpdate();
			}
		}
		public void SelectPrevAppointment() {
			SelectPrevAppointmentCommand command = new SelectPrevAppointmentCommand(this);
			command.Execute();
		}
		public void SelectNextAppointment() {
			SelectNextAppointmentCommand command = new SelectNextAppointmentCommand(this);
			command.Execute();
		}
		#region IRangeControlClient Members
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) {
			RangeControlClientSupport.UpdateHotInfo(hitInfo);
		}
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			RangeControlClientSupport.UpdatePressedInfo(hitInfo);
		}
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) {
			RangeControlClientSupport.OnClick(hitInfo);
		}
		string IRangeControlClient.InvalidText {
			get { return RangeControlClientSupport.InvalidText; }
		}
		bool IRangeControlClient.IsValidType(Type type) { return true; }
		bool IRangeControlClient.IsValid {
			get { return RangeControlClientSupport.IsValid; }
		}
		object IRangeControlClient.GetOptions() {
			return RangeControlClientSupport.GetOptions();
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return RangeControlClientSupport.DrawRuler(e);
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			RangeControlClientSupport.DrawContent(e);
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) {
			return RangeControlClientSupport.SupportOrientation(orientation);
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return RangeControlClientSupport.GetNormalizedValue(value);
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return RangeControlClientSupport.GetValue(normalizedValue);
		}
		int IRangeControlClient.RangeBoxBottomIndent { get { return RangeControlClientSupport.RangeBoxBottomIndent; } }
		int IRangeControlClient.RangeBoxTopIndent { get { return RangeControlClientSupport.RangeBoxTopIndent; } }
		string IRangeControlClient.RulerToString(int ruleIndex) { return RangeControlClientSupport.RulerToString(ruleIndex); }
		bool IRangeControlClient.IsCustomRuler { get { return RangeControlClientSupport.IsCustomRuler; } }
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) { return RangeControlClientSupport.GetRuler(e); }
		object IRangeControlClient.RulerDelta { get { return RangeControlClientSupport.RulerDelta; } }
		double IRangeControlClient.NormalizedRulerDelta { get { return RangeControlClientSupport.NormalizedRulerDelta; } }
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			RangeControlClientSupport.OnRangeChanged(rangeMinimum, rangeMaximum);
		}
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add { RangeControlClientSupport.RangeChanged += value; }
			remove { RangeControlClientSupport.RangeChanged -= value; }
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			RangeControlClientSupport.ValidateRange(info);
		}
		double IRangeControlClient.ValidateScale(double newScale) {
			return RangeControlClientSupport.ValidateScale(newScale);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			if (rangeControl == null) {
				if (this.rangeControlSupport != null) {
					this.rangeControlSupport.Dispose();
					this.rangeControlSupport = null;
				}
				return;
			}
			if (this.rangeControlSupport != null && this.rangeControlSupport.RangeControl != null)
				this.rangeControlSupport.RangeControl.Client = null;
			RangeControlClientSupport.OnRangeControlChanged(rangeControl);
		}
		void IRangeControlClient.OnResize() {
			RangeControlClientSupport.OnResize();
		}
		void IRangeControlClient.Calculate(Rectangle contentRect) {
			RangeControlClientSupport.Calculate(contentRect);
		}
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return string.Empty;
		}
		#endregion
		#region RangeControlAutoAdjusting
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerControlRangeControlAutoAdjusting"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event RangeControlAdjustEventHandler RangeControlAutoAdjusting {
			add { Events.AddHandler(onRangeControlAutoAdjusting, value); }
			remove { Events.RemoveHandler(onRangeControlAutoAdjusting, value); }
		}
		static readonly object onRangeControlAutoAdjusting = new object();
		private void RaiseRangeControlAutoAdjusting(RangeControlAdjustEventArgs e) {
			RangeControlAdjustEventHandler handler = (RangeControlAdjustEventHandler)Events[onRangeControlAutoAdjusting];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (innerControl != null)
				innerControl.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (innerControl != null)
				innerControl.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (innerControl != null)
				innerControl.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (innerControl != null)
				innerControl.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (innerControl != null)
				innerControl.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (innerControl != null)
				innerControl.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual T GetService<T>() {
			return (T)this.GetService(typeof(T));
		}
		public virtual new object GetService(Type serviceType) {
			if (innerControl != null)
				return innerControl.GetService(serviceType);
			else
				return null;
		}
		#endregion
		protected internal virtual void AddServices() {
			AddService(typeof(DevExpress.XtraScheduler.Services.ISelectionService), new WinSelectionService(this));
			AddService(typeof(IMouseHandlerService), new MouseHandlerService(MouseHandler));
			AddService(typeof(IKeyboardHandlerService), new SchedulerKeyboardHandlerService(InnerControl));
			SchedulerStateService stateService = new SchedulerStateService();
			AddService(typeof(ISchedulerStateService), stateService);
			AddService(typeof(ISetSchedulerStateService), stateService);
			AddService(typeof(IHeaderCaptionService), new HeaderCaptionService());
			AddService(typeof(IHeaderToolTipService), new HeaderToolTipService());
			AddService(typeof(ISchedulerPrintService), new SchedulerPrintService(this));
			AddService(typeof(IFileOperationService), new FileOperationService(this));
			AddService(typeof(IAnimationService), new AnimationService(this));
		}
		#region IInnerSchedulerCommandTarget Members
		InnerSchedulerControl IInnerSchedulerCommandTarget.InnerSchedulerControl { get { return InnerControl; } }
		#endregion
		public override void Refresh() {
			DateTimeScrollController.SuspendUpdateScrollValue();
			try {
				ActiveView.LayoutChanged();
				base.Refresh();
				RaiseRefreshed();
			} finally {
				DateTimeScrollController.ResumeUpdateScrollValue();
			}
		}
		protected internal SchedulerPrinter CreateSchedulerPrinter(SchedulerPrintStyle printStyle) {
			return new SchedulerPrinter(this, printStyle);
		}
		protected void RecreateSchedulerPrinter(SchedulerPrintStyle printStyle) {
			if (printer != null) {
				printer.Dispose();
			}
			this.printer = CreateSchedulerPrinter(printStyle);
		}
		public PrintingSystemBase GetPrintPreviewDocument(SchedulerPrintStyle printStyle) {
			RecreateSchedulerPrinter(printStyle);
			this.printer.CreateDocument();
			return this.printer.PrintingSystemBase;
		}
		#region ICommandAwareControl<SchedulerCommandId> Members
		void ICommandAwareControl<SchedulerCommandId>.CommitImeContent() {
		}
		Command ICommandAwareControl<SchedulerCommandId>.CreateCommand(SchedulerCommandId id) {
			return this.CreateCommand(id);
		}
		void ICommandAwareControl<SchedulerCommandId>.Focus() {
			this.Focus();
		}
		bool ICommandAwareControl<SchedulerCommandId>.HandleException(Exception e) {
			return false;
		}
		CommandBasedKeyboardHandler<SchedulerCommandId> ICommandAwareControl<SchedulerCommandId>.KeyboardHandler { get { return InnerControl != null ? InnerControl.KeyboardHandler as CommandBasedKeyboardHandler<SchedulerCommandId> : null; } }
		event EventHandler ICommandAwareControl<SchedulerCommandId>.UpdateUI { add { ((ICommandAwareControl<SchedulerCommandId>)InnerControl).UpdateUI += value; } remove { ((ICommandAwareControl<SchedulerCommandId>)InnerControl).UpdateUI -= value; } }
		#endregion
		SchedulerCommand CreateCommand(SchedulerCommandId id) {
			if (InnerControl != null)
				return InnerControl.CreateCommand(id);
			else
				return null;
		}
	}
}
namespace DevExpress.XtraScheduler.Native {
	#region WinFormsInnerSchedulerControl
	public class WinFormsInnerSchedulerControl : InnerSchedulerControl {
		public WinFormsInnerSchedulerControl(IInnerSchedulerControlOwner owner)
			: base(owner) {
		}
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; } }
		protected internal override void InitializeKeyboardHandlerDefaults(NormalKeyboardHandlerBase keyboardHandler) {
			base.InitializeKeyboardHandlerDefaults(keyboardHandler);
			SchedulerViewRepositoryBase views = this.Views;
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Tab, Keys.None, SchedulerCommandId.SelectNextAppointment);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Tab, Keys.Shift, SchedulerCommandId.SelectPrevAppointment);
		}
		protected override object CreateSchedulerCommandFactoryService() {
			return new WinSchedulerCommandFactoryService(this);
		}
		protected internal override object XtraCreateResourceColorSchemasItem(XtraItemEventArgs e) {
			return new SchedulerColorSchema();
		}
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemaCollection() {
			return new SchedulerColorSchemaCollection();
		}
		protected override ICollectionChangedListener CreateResourceColorSchemaChangedListener() {
			return new ResourceColorSchemasChangedListener(ResourceColorSchemas);
		}
		protected override void Dispose(bool disposing) {
			if (ActiveView != null) {
				SchedulerViewBase viewBase = ActiveView.Owner as SchedulerViewBase;
				if (viewBase != null && viewBase.ViewInfo != null)
					viewBase.ViewInfo.CancellationToken.Cancel();
			}
			base.Dispose(disposing);
		}
#pragma warning disable 618
		protected internal override void UpdateTimeMarkerVisibilityFromOptionBehavior() {
			base.UpdateTimeMarkerVisibilityFromOptionBehavior();
			SchedulerControl control = Owner as SchedulerControl;
			if (control == null)
				return;			
			control.DayView.TimeIndicatorDisplayOptions.Visibility = ConvertToTimeIndicatorVisibility(OptionsBehavior.ShowCurrentTime);
			control.WorkWeekView.TimeIndicatorDisplayOptions.Visibility = ConvertToTimeIndicatorVisibility(OptionsBehavior.ShowCurrentTime);
			control.FullWeekView.TimeIndicatorDisplayOptions.Visibility = ConvertToTimeIndicatorVisibility(OptionsBehavior.ShowCurrentTime);
			control.TimelineView.TimeIndicatorDisplayOptions.Visibility = ConvertToTimeIndicatorVisibility(OptionsBehavior.ShowCurrentTime);
		}
		TimeIndicatorVisibility ConvertToTimeIndicatorVisibility(CurrentTimeVisibility visibility) {
			switch (visibility) {
				case CurrentTimeVisibility.Always:
					return TimeIndicatorVisibility.Always;
				case CurrentTimeVisibility.Never:
					return TimeIndicatorVisibility.Never;
				default:
					return TimeIndicatorVisibility.TodayView;
			}
		}
#pragma warning restore 618
	}
	#endregion
	public class ResourceColorSchemasChangedListener : NotificationCollectionChangedListener<SchedulerColorSchema>, ICollectionChangedListener {
		public ResourceColorSchemasChangedListener(NotificationCollection<SchedulerColorSchema> collection)
			: base(collection) {
		}
	}
	#region ModalFormShowingRegistrator
	public class ModalFormShowingRegistrator : IDisposable {
		#region Fields
		SchedulerControl control;
		#endregion
		public ModalFormShowingRegistrator(SchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
			ISetSchedulerStateService setStateService = (ISetSchedulerStateService)control.GetService(typeof(ISetSchedulerStateService));
			if (setStateService != null)
				setStateService.IsModalFormOpened = true;
		}
		public SchedulerControl Control { get { return control; } }
		public DialogResult ShowForm(XtraForm form, IWin32Window parent) {
			Cursor cursor = control.Cursor;
			try {
				control.Cursor = Cursors.WaitCursor;
				form.LookAndFeel.ParentLookAndFeel = control.LookAndFeel;
				return FormTouchUIAdapter.ShowDialog(form, parent);
			} finally {
				control.Cursor = cursor;
			}
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				ISetSchedulerStateService setStateService = (ISetSchedulerStateService)control.GetService(typeof(ISetSchedulerStateService));
				if (setStateService != null)
					setStateService.IsModalFormOpened = false;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ModalFormShowingRegistrator() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	public class ColoredSkinElementCache {
		class NameHashTable : Dictionary<string, SkinElement> {
		}
		class ColorHashTable : Dictionary<Color, NameHashTable> {
		}
		class BrushHashTable : Dictionary<Brush, NameHashTable> {
		}
		ColorHashTable colorHashTable = new ColorHashTable();
		BrushHashTable brushHashTable = new BrushHashTable();
		[MethodImpl(MethodImplOptions.Synchronized)]
		public SkinElementInfo GetAppointmentSkinElementInfo(string skinElementName, Color color, UserLookAndFeel lookAndFeel, Rectangle bounds) {
			if (color == SystemColors.Window) {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				SkinElement skinEl = skin[skinElementName];
				return new SkinElementInfo(skinEl, bounds);
			} else {
				NameHashTable colorNameHash;
				if (!colorHashTable.TryGetValue(color, out colorNameHash)) {
					colorNameHash = new NameHashTable();
					colorHashTable.Add(color, colorNameHash);
				}
				SkinElement coloredElement;
				if (!colorNameHash.TryGetValue(skinElementName, out coloredElement)) {
					Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
					coloredElement = skin[skinElementName];
					coloredElement = SkinElementColorer.PaintElementWithColor(coloredElement, color);
					colorNameHash.Add(skinElementName, coloredElement);
				}
				return new SkinElementInfo(coloredElement, bounds);
			}
		}
		public SkinElementInfo GetBrushedSkinElementInfo(string skinElementName, Brush brush, Color baseColor, UserLookAndFeel lookAndFeel, Rectangle bounds) {
			NameHashTable brushNameHash;
			if (!brushHashTable.TryGetValue(brush, out brushNameHash)) {
				brushNameHash = new NameHashTable();
				brushHashTable.Add(brush, brushNameHash);
			}
			SkinElement brushedElement;
			if (!brushNameHash.TryGetValue(skinElementName, out brushedElement)) {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				brushedElement = skin[skinElementName];
				Image originalImage = brushedElement.Info.GetActualImage();
				Image newImage;
				newImage = originalImage == null ? new Bitmap(bounds.Width, bounds.Height) : (Image)originalImage.Clone();
				using (Graphics g = Graphics.FromImage(newImage)) {
					Rectangle rect = new Rectangle(Point.Empty, newImage.Size);
					if (originalImage == null) {
						using (Brush br = new SolidBrush(baseColor)) {
							g.FillRectangle(br, rect);
							g.FillRectangle(brush, rect);
						}
					} else
						g.FillRectangle(brush, rect);
				}
				brushedElement.Info.SetActualImage(newImage, true);
				brushNameHash.Add(skinElementName, brushedElement);
			}
			return new SkinElementInfo(brushedElement, bounds);
		}
		public void Reset() {
			colorHashTable.Clear();
		}
	}
}
namespace DevExpress.XtraScheduler.Drawing {
	public interface ISupportCustomDraw {
		void RaiseCustomDrawTimeCell(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawDayViewAllDayArea(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawResourceHeader(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawGroupSeparator(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawDayHeader(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawWeekViewTopLeftCorner(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawDayOfWeekHeader(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawDayViewTimeRuler(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawTimeIndicator(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawAppointmentBackground(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawAppointment(CustomDrawObjectEventArgs e);
		void RaiseCustomDrawDependency(CustomDrawObjectEventArgs e);
	}
}
