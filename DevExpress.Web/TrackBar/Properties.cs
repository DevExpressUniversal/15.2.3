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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	 public class TrackBarProperties : EditProperties {
		private TrackBarItemCollection items = null;
		public TrackBarProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesAllowMouseWheel"),
#endif
		DefaultValue(true), NotifyParentProperty(true), Category("Behavior"),
		AutoFormatDisable, Localizable(true)]
		public bool AllowMouseWheel {
			get { return GetBoolProperty("AllowMouseWheel", true); }
			set { SetBoolProperty("AllowMouseWheel", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesAllowRangeSelection"),
#endif
		DefaultValue(false), NotifyParentProperty(true), Category("Behavior"),
		AutoFormatDisable, Localizable(true)]
		public bool AllowRangeSelection {
			get { return GetBoolProperty("AllowRangeSelection", false); }
			set { SetBoolProperty("AllowRangeSelection", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesEnableAnimation"),
#endif
		DefaultValue(true), NotifyParentProperty(true), Category("Behavior"),
		AutoFormatDisable, Localizable(true)]
		public bool EnableAnimation {
			get { return GetBoolProperty("EnableAnimation", true); }
			set { SetBoolProperty("EnableAnimation", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesValueChangedDelay"),
#endif
		DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true), Category("Behavior")]
		public int ValueChangedDelay {
			get { return GetIntProperty("ValueChangedDelay", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "ValueChangedDelay");
				SetIntProperty("ValueChangedDelay", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesClientSideEvents"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), Category("Client-Side")]
		public new TrackBarClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as TrackBarClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public TrackBarItemCollection Items {
			get {
				if(items == null)
					items = new TrackBarItemCollection(this);
				return items;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesValueField"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), Category("Data"),]
		public string ValueField {
			get { return GetStringProperty("ValueField", ""); }
			set { SetStringProperty("ValueField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesToolTipField"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), Category("Data"),]
		public string ToolTipField {
			get { return GetStringProperty("ToolTipField", ""); }
			set { SetStringProperty("ToolTipField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesTextField"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), Category("Data"),]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set { SetStringProperty("TextField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesValueType"),
#endif
		DefaultValue(typeof(Decimal)), NotifyParentProperty(true), Category("Data"),
		TypeConverter(typeof(TrackBarValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get { return (Type)GetObjectProperty("ValueType", typeof(Decimal)); }
			set {
				if(ValueType != value) {
					SetObjectProperty("ValueType", typeof(Decimal), value);
					RefreshValues();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesDecrementButtonImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonImageProperties DecrementButtonImage {
			get { return Images.TrackBarDecrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesIncrementButtonImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonImageProperties IncrementButtonImage {
			get { return Images.TrackBarIncrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesMainDragHandleImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonImageProperties MainDragHandleImage {
			get { return Images.TrackBarMainDragHandle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesSecondaryDragHandleImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonImageProperties SecondaryDragHandleImage {
			get { return Images.TrackBarSecondaryDragHandle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesDecrementButtonToolTip"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true),
		DefaultValue(StringResources.TrackBar_Decrement)]
		public string DecrementButtonToolTip {
			get { return GetStringProperty("DecrementButtonToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TrackBar_Decrement)); }
			set { SetStringProperty("DecrementButtonToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TrackBar_Decrement), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesDirection"),
#endif
		DefaultValue(Direction.Normal), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public Direction Direction {
			get { return (Direction)GetEnumProperty("Direction", Direction.Normal); }
			set { SetEnumProperty("Direction", Direction.Normal, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesDragHandleToolTip"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true),
		DefaultValue(StringResources.TrackBar_Drag)]
		public string DragHandleToolTip {
			get { return GetStringProperty("DragHandleToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TrackBar_Drag)); }
			set { SetStringProperty("DragHandleToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TrackBar_Drag), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesEqualTickMarks"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public bool EqualTickMarks {
			get { return GetBoolProperty("EqualTickMarks", false); }
			set { SetBoolProperty("EqualTickMarks", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesIncrementButtonToolTip"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true),
		DefaultValue(StringResources.TrackBar_Increment)]
		public string IncrementButtonToolTip {
			get { return GetStringProperty("IncrementButtonToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TrackBar_Increment)); }
			set { SetStringProperty("IncrementButtonToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TrackBar_Increment), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesLargeTickInterval"),
#endif
		DefaultValue(typeof(decimal), "50"), NotifyParentProperty(true), AutoFormatEnable]
		public decimal LargeTickInterval {
			get { return GetDecimalProperty("LargeTickInterval", 50); }
			set {
				CommonUtils.CheckNegativeOrZeroValue((double)value, "LargeTickInterval");
				SetDecimalProperty("LargeTickInterval", 50, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesLargeTickEndValue"),
#endif
		DefaultValue(typeof(decimal), "100"), NotifyParentProperty(true), AutoFormatEnable]
		public decimal LargeTickEndValue {
			get { return GetDecimalProperty("LargeTickEndValue", MaxValue); }
			set { SetDecimalProperty("LargeTickEndValue", MaxValue, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesLargeTickStartValue"),
#endif
		DefaultValue(typeof(decimal), "0"), NotifyParentProperty(true), AutoFormatEnable]
		public decimal LargeTickStartValue {
			get { return GetDecimalProperty("LargeTickStartValue", MinValue); }
			set { SetDecimalProperty("LargeTickStartValue", MinValue, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesOrientation"),
#endif
		DefaultValue(Orientation.Horizontal), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public Orientation Orientation {
			get { return (Orientation)GetEnumProperty("Orientation", Orientation.Horizontal); }
			set { SetEnumProperty("Orientation", Orientation.Horizontal, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesScaleLabelHighlightMode"),
#endif
		DefaultValue(ScaleLabelHighlightMode.None), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public ScaleLabelHighlightMode ScaleLabelHighlightMode {
			get { return (ScaleLabelHighlightMode)GetEnumProperty("ScaleLabelHighlightMode", ScaleLabelHighlightMode.None); }
			set { SetEnumProperty("ScaleLabelHighlightMode", ScaleLabelHighlightMode.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesScalePosition"),
#endif
		DefaultValue(ScalePosition.None), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public ScalePosition ScalePosition {
			get { return (ScalePosition)GetEnumProperty("ScalePosition", ScalePosition.None); }
			set { SetEnumProperty("ScalePosition", ScalePosition.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesShowChangeButtons"),
#endif
		DefaultValue(true), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public bool ShowChangeButtons {
			get { return GetBoolProperty("ShowChangeButtons", true); }
			set { SetBoolProperty("ShowChangeButtons", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesShowDragHandles"),
#endif
		DefaultValue(true), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public bool ShowDragHandles {
			get { return GetBoolProperty("ShowDragHandles", true); }
			set { SetBoolProperty("ShowDragHandles", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesSmallTickFrequency"),
#endif
		DefaultValue(5), NotifyParentProperty(true), AutoFormatEnable]
		public int SmallTickFrequency {
			get { return GetIntProperty("SmallTickFrequency", 5); }
			set {
				CommonUtils.CheckNegativeOrZeroValue((double)value, "SmallTickFrequency");
				SetIntProperty("SmallTickFrequency", 5, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesIncrementButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarButtonStyle IncrementButtonStyle {
			get { return Styles.TrackBarIncrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesItemStyle"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TrackBarTickStyle ItemStyle {
			get { return Styles.TrackBarItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesDecrementButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarButtonStyle DecrementButtonStyle {
			get { return Styles.TrackBarDecrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesLargeTickStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarTickStyle LargeTickStyle {
			get { return Styles.TrackBarLargeTick; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesLeftTopLabelStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase LeftTopLabelStyle {
			get { return Styles.TrackBarLeftTopLabel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesMainDragHandleStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarButtonStyle MainDragHandleStyle {
			get { return Styles.TrackBarMainDragHandle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesRightBottomLabelStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase RightBottomLabelStyle {
			get { return Styles.TrackBarRightBottomLabel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesScaleStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase ScaleStyle {
			get { return Styles.TrackBarScale; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesSelectedItemStyle"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TrackBarTickStyle SelectedItemStyle {
			get { return Styles.TrackBarSelectedItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesSelectedTickStyle"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TrackBarTickStyle SelectedTickStyle {
			get { return Styles.TrackBarSelectedTick; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesSecondaryDragHandleStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarButtonStyle SecondaryDragHandleStyle {
			get { return Styles.TrackBarSecondaryDragHandle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesBarHighlightStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarTrackElementStyle BarHighlightStyle {
			get { return Styles.TrackBarBarHighlight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesSmallTickStyle"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TrackBarTickStyle SmallTickStyle {
			get { return Styles.TrackBarSmallTick; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesTrackStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TrackBarTrackElementStyle TrackStyle {
			get { return Styles.TrackBarTrack; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesValueToolTipStyle"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TrackBarValueToolTipStyle ValueToolTipStyle {
			get { return Styles.TrackBarValueToolTip; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesValueToolTipFormatString"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string ValueToolTipFormatString {
			get { return GetStringProperty("ValueToolTipFormatString", string.Empty); }
			set { SetStringProperty("ValueToolTipFormatString", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesValueToolTipPosition"),
#endif
		DefaultValue(ValueToolTipPosition.LeftOrTop), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public ValueToolTipPosition ValueToolTipPosition {
			get { return (ValueToolTipPosition)GetEnumProperty("ValueToolTipPosition", ValueToolTipPosition.LeftOrTop); }
			set { SetEnumProperty("ValueToolTipPosition", ValueToolTipPosition.LeftOrTop, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesMaxValue"),
#endif
		DefaultValue(typeof(decimal), "100"), NotifyParentProperty(true), AutoFormatDisable]
		public decimal MaxValue {
			get {
				return IsItemMode ? Items.Count - 1 : GetDecimalProperty("MaxValue", 100); }
			set {
				if(!IsItemMode)
					SetDecimalProperty("MaxValue", 100, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesMinValue"),
#endif
		DefaultValue(typeof(decimal), "0"), NotifyParentProperty(true), AutoFormatDisable]
		public decimal MinValue {
			get {
				return IsItemMode ? 0 : GetDecimalProperty("MinValue", 0);
			}
			set {
				if(!IsItemMode)
					 SetDecimalProperty("MinValue", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesScaleLabelFormatString"),
#endif
		DefaultValue("{0}"), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string ScaleLabelFormatString {
			get { return GetStringProperty("ScaleLabelFormatString", "{0}"); }
			set { SetStringProperty("ScaleLabelFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarPropertiesStep"),
#endif
		DefaultValue(typeof(decimal), "2"), NotifyParentProperty(true), AutoFormatEnable]
		public decimal Step {
			get { return IsItemMode ? 1 : GetDecimalProperty("Step", GetDefaultStep()); }
			set {
				if(!IsItemMode) {
					CommonUtils.CheckNegativeOrZeroValue((double)value, "Step");
					SetDecimalProperty("Step", GetDefaultStep(), value);
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				TrackBarProperties src = source as TrackBarProperties;
				if(src != null) {
					ValueType = src.ValueType;
					Orientation = src.Orientation;
					EnableAnimation = src.EnableAnimation;
					ScalePosition = src.ScalePosition;
					AllowMouseWheel = src.AllowMouseWheel;
					AllowRangeSelection = src.AllowRangeSelection;
					Direction = src.Direction;
					DragHandleToolTip = src.DragHandleToolTip;
					EqualTickMarks = src.EqualTickMarks;
					IncrementButtonToolTip = src.IncrementButtonToolTip;
					Items.Assign(src.Items);
					LargeTickInterval = src.LargeTickInterval;
					MaxValue = src.MaxValue;
					MinValue = src.MinValue;
					LargeTickStartValue = src.LargeTickStartValue;
					LargeTickEndValue = src.LargeTickEndValue;
					ScaleLabelFormatString = src.ScaleLabelFormatString;
					ScaleLabelHighlightMode = src.ScaleLabelHighlightMode;
					ScalePosition = src.ScalePosition;
					ShowChangeButtons = src.ShowChangeButtons;
					ShowDragHandles = src.ShowDragHandles;
					SmallTickFrequency = src.SmallTickFrequency;
					Step = src.Step;
					TextField = src.TextField;
					ToolTipField = src.ToolTipField;
					ValueChangedDelay = src.ValueChangedDelay;
					ValueField = src.ValueField;
					ValueToolTipFormatString = src.ValueToolTipFormatString;
					ValueToolTipPosition = src.ValueToolTipPosition;
					DecrementButtonToolTip = src.DecrementButtonToolTip;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected decimal GetDefaultStep() {
			return SmallTickFrequency != 0 ? LargeTickInterval / SmallTickFrequency : LargeTickInterval;
		}
		protected internal bool IsItemMode {
			get { return Items.Count > 0; }
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new TrackBarClientSideEvents(this);
		}
		protected override ASPxEditBase CreateEditInstance() {
			return null; 
		}
		protected void RefreshValues() {
			foreach(TrackBarItem item in Items) {
				if(item.Value != null && item.Value.GetType() != ValueType)
					item.Value = CommonUtils.ConvertToType(item.Value, ValueType, false);
			}
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			return null; 
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
			   new IStateManager[] { Items });
		}
	}
}
