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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Design;
	using DevExpress.Web.Internal;
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ControlValueProperty("Value"), DataBindingHandler(typeof(TextDataBindingHandler)),
	Designer("DevExpress.Web.Design.ASPxTrackBarDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTrackBar.bmp")
	]
	public class ASPxTrackBar : ASPxEdit, IEditDataHelperOwner, IValueTypeHolder, IControlDesigner {
		protected internal const string
			TrackBarScriptResourceName = EditScriptsResourcePath + "TrackBar.js",
			IncrementButtonId = "IB",
			DecrementButtonId = "DB",
			MainDragHandleId = "MD",
			TrackId = "T",
			InputId = "I",
			SecondaryDragHandleId = "SD",
			BarHighlightId = "S";
		private
			TrackBarDataHelper trackBarDataHelper = null;
			decimal? savedPosition = null;
			bool positionChanged = false;
			bool valueChanged = false;
		private static readonly object PositionChangedEvent = new object();
		public ASPxTrackBar()
			: base() {
				trackBarDataHelper = new TrackBarDataHelper(this, delegate() {
					return this.ToolTipField;
				});
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDataSourceID"),
#endif
		Browsable(true), AutoFormatDisable, Themeable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDataSource"),
#endif
		Browsable(true), AutoFormatDisable, EditorBrowsable(EditorBrowsableState.Always)]
		public override object DataSource {
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}
		ListEditItemCollection IEditDataHelperOwner.Items { get { return Items; } }
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if(trackBarDataHelper.PerformDataBinding(data, ValueField, TextField, string.Empty)) {
				RestorePosition();
				ResetControlHierarchy();
			}
		}
		protected void RestorePosition() {
			if(savedPosition.HasValue && Items.Count > savedPosition)
				Position = savedPosition.Value;
			savedPosition = null;
		}
		protected void SavePosition(decimal position) {
			if(!trackBarDataHelper.ItemsAreFinal)
				savedPosition = position;
		}
		protected void OnPositionChanged(EventArgs e) {
			EventHandler handler = Events[PositionChangedEvent] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarPositionChanged"),
#endif
		Category("Action")]
		public event EventHandler PositionChanged
		{
			add { Events.AddHandler(PositionChangedEvent, value); }
			remove { Events.RemoveHandler(PositionChangedEvent, value); }
		}
		protected override void RaiseValueChanged() {
			if(valueChanged)
				OnValueChanged(EventArgs.Empty);
			if(positionChanged)
				OnPositionChanged(EventArgs.Empty);
		}
		protected string GetInternalValue(string value, int index) {
			value = value.Replace("[,", "[null,").Replace(",]", ",null]");
			object retValue = HtmlConvertor.FromJSON<ArrayList>(value)[index];
			return retValue != null ? retValue.ToString() : String.Empty;
		}
		protected override object GetPostBackValue(string controlName, NameValueCollection postCollection) {
			string value = base.GetPostBackValue(controlName, postCollection) as string;
			return ConvertValue(GetInternalValue(value, 0));
		}
		protected object ConvertValue(object value) {
			object result = null;
			value = value.ToString() == "null" ? null : value;
			if(value != null && !(value is DBNull))
				result = CommonUtils.GetConvertedArgumentValue(value, ValueType, "Value");
			return result;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			RestorePosition();
			valueChanged = positionChanged = base.LoadPostData(postCollection);
			if(AllowRangeSelection) {
				string postData = postCollection[UniqueID];
				string selectionEndString = GetInternalValue(postData, 1);
				decimal selectionEnd = -1;
				selectionEnd = IsItemMode ?  Items.IndexOfValue(ConvertValue(selectionEndString)) : 
					decimal.Parse(selectionEndString);
				if(selectionEnd != PositionEnd) {
					PositionEnd = selectionEnd;
					positionChanged = true;
				}
			}
			return valueChanged || positionChanged;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarAllowMouseWheel"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(true), AutoFormatEnable, Category("Behavior"),
		NotifyParentProperty(true)]
		public bool AllowMouseWheel {
			get { return Properties.AllowMouseWheel; }
			set {
				Properties.AllowMouseWheel = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarAllowRangeSelection"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Behavior"), DefaultValue(false), AutoFormatEnable, NotifyParentProperty(true)]
		public bool AllowRangeSelection {
			get { return Properties.AllowRangeSelection; }
			set {
				Properties.AllowRangeSelection = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarEnableAnimation"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Behavior"), DefaultValue(true), AutoFormatEnable, NotifyParentProperty(true)]
		public bool EnableAnimation {
			get { return Properties.EnableAnimation; }
			set { Properties.EnableAnimation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarValueChangedDelay"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable]
		public int ValueChangedDelay {
			get { return Properties.ValueChangedDelay; }
			set { Properties.ValueChangedDelay = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public TrackBarClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarItems"),
#endif
 Category("Data"),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public TrackBarItemCollection Items {
			get { return Properties.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(DataSourceViewSchemaConverter)),
		AutoFormatDisable, Themeable(false)]
		public string TextField {
			get { return Properties.TextField; }
			set {
				Properties.TextField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarToolTipField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(DataSourceViewSchemaConverter)),
		AutoFormatDisable, Themeable(false)]
		public string ToolTipField {
			get { return Properties.ToolTipField; }
			set {
				Properties.ToolTipField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarValueField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(DataSourceViewSchemaConverter)),
		AutoFormatDisable, Themeable(false)]
		public string ValueField {
			get { return Properties.ValueField; }
			set {
				Properties.ValueField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarValueType"),
#endif
		DefaultValue(typeof(Decimal)), TypeConverter(typeof(TrackBarValueTypeTypeConverter)),
		AutoFormatDisable, Themeable(false), Category("Data")]
		public Type ValueType {
			get { return Properties.ValueType; }
			set { Properties.ValueType = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDecrementButtonImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ButtonImageProperties DecrementButtonImage {
			get { return Properties.DecrementButtonImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarIncrementButtonImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ButtonImageProperties IncrementButtonImage {
			get { return Properties.IncrementButtonImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarMainDragHandleImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ButtonImageProperties MainDragHandleImage {
			get { return Properties.MainDragHandleImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarSecondaryDragHandleImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ButtonImageProperties SecondaryDragHandleImage {
			get { return Properties.SecondaryDragHandleImage; }
		}
		protected internal ButtonImageProperties GetDecrementButtonImage() {
			ButtonImageProperties result = new ButtonImageProperties();
			result.MergeWith(RenderImages.GetImageProperties(Page, EditorImages.TrackBarDecrementButtonImageName));
			result.MergeWith(Properties.DecrementButtonImage);
			return result;
		}
		protected internal ButtonImageProperties GetIncrementButtonImage() {
			ButtonImageProperties result = new ButtonImageProperties();
			result.MergeWith(RenderImages.GetImageProperties(Page, EditorImages.TrackBarIncrementButtonImageName));
			result.MergeWith(Properties.IncrementButtonImage);
			return result;
		}
		protected internal ButtonImageProperties GetMainDragHandleImage() {
			ButtonImageProperties result = new ButtonImageProperties();
			result.MergeWith(RenderImages.GetImageProperties(Page, EditorImages.TrackBarMainDragHandleImageName));
			result.MergeWith(Properties.MainDragHandleImage);
			return result;
		}
		protected internal ButtonImageProperties GetSecondaryDragHandleImage() {
			ButtonImageProperties result = new ButtonImageProperties();
			result.MergeWith(RenderImages.GetImageProperties(Page, EditorImages.TrackBarSecondaryDragHandleImageName));
			result.MergeWith(Properties.SecondaryDragHandleImage);
			return result;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDecrementButtonToolTip"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		DefaultValue(StringResources.TrackBar_Decrement)]
		public string DecrementButtonToolTip {
			get { return Properties.DecrementButtonToolTip; }
			set { Properties.DecrementButtonToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDirection"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(Direction.Normal), AutoFormatEnable,
		NotifyParentProperty(true)]
		public Direction Direction {
			get { return Properties.Direction; }
			set { Properties.Direction = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDragHandleToolTip"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		DefaultValue(StringResources.TrackBar_Drag)]
		public string DragHandleToolTip {
			get { return Properties.DragHandleToolTip; }
			set { Properties.DragHandleToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarEqualTickMarks"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(false), AutoFormatEnable, Category("Layout"),
		NotifyParentProperty(true)]
		public bool EqualTickMarks {
			get { return Properties.EqualTickMarks; }
			set {
				Properties.EqualTickMarks = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarIncrementButtonToolTip"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		DefaultValue(StringResources.TrackBar_Increment)]
		public string IncrementButtonToolTip {
			get { return Properties.IncrementButtonToolTip; }
			set { Properties.IncrementButtonToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarLargeTickInterval"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(typeof(decimal), "50"), AutoFormatEnable,
		NotifyParentProperty(true)]
		public decimal LargeTickInterval {
			get { return Properties.LargeTickInterval; }
			set {
				Properties.LargeTickInterval = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarLargeTickEndValue"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(typeof(decimal), "100"), AutoFormatEnable,
		NotifyParentProperty(true)]
		public decimal LargeTickEndValue {
			get { return Properties.LargeTickEndValue; }
			set {
				Properties.LargeTickEndValue = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarLargeTickStartValue"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(typeof(decimal), "0"), AutoFormatEnable,
		NotifyParentProperty(true)]
		public decimal LargeTickStartValue {
			get { return Properties.LargeTickStartValue; }
			set {
				Properties.LargeTickStartValue = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarOrientation"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(Orientation.Horizontal), AutoFormatEnable,
		NotifyParentProperty(true)]
		public Orientation Orientation {
			get { return Properties.Orientation; }
			set {
				Properties.Orientation = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft
		{
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarScaleLabelHighlightMode"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(ScaleLabelHighlightMode.None), AutoFormatEnable,
		NotifyParentProperty(true)]
		public ScaleLabelHighlightMode ScaleLabelHighlightMode {
			get { return Properties.ScaleLabelHighlightMode; }
			set { Properties.ScaleLabelHighlightMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarScalePosition"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(ScalePosition.None), AutoFormatEnable,
		NotifyParentProperty(true)]
		public ScalePosition ScalePosition {
			get { return Properties.ScalePosition; }
			set { Properties.ScalePosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarShowChangeButtons"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(true), AutoFormatEnable,
		NotifyParentProperty(true)]
		public bool ShowChangeButtons {
			get { return Properties.ShowChangeButtons; }
			set {
				Properties.ShowChangeButtons = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarShowDragHandles"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(true), AutoFormatEnable,
		NotifyParentProperty(true)]
		public bool ShowDragHandles {
			get { return Properties.ShowDragHandles; }
			set {
				Properties.ShowDragHandles = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarSmallTickFrequency"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(5), AutoFormatEnable,
		NotifyParentProperty(true)]
		public int SmallTickFrequency {
			get { return Properties.SmallTickFrequency; }
			set {
				Properties.SmallTickFrequency = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarDecrementButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle DecrementButtonStyle {
			get { return Properties.DecrementButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarIncrementButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle IncrementButtonStyle {
			get { return Properties.IncrementButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle ItemStyle {
			get { return Properties.ItemStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarLargeTickStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle LargeTickStyle {
			get { return Properties.LargeTickStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarLeftTopLabelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase LeftTopLabelStyle {
			get { return Properties.LeftTopLabelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarMainDragHandleStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle MainDragHandleStyle {
			get { return Properties.MainDragHandleStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarRightBottomLabelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase RightBottomLabelStyle {
			get { return Properties.RightBottomLabelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarScaleStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase ScaleStyle {
			get { return Properties.ScaleStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarSecondaryDragHandleStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle SecondaryDragHandleStyle {
			get { return Properties.SecondaryDragHandleStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarSelectedItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle SelectedItemStyle {
			get { return Properties.SelectedItemStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarSelectedTickStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle SelectedTickStyle {
			get { return Properties.SelectedTickStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarBarHighlightStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTrackElementStyle BarHighlightStyle {
			get { return Properties.BarHighlightStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarSmallTickStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle SmallTickStyle {
			get { return Properties.SmallTickStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarTrackStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase TrackStyle {
			get { return Properties.TrackStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarValueToolTipStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarValueToolTipStyle ValueToolTipStyle {
			get { return Properties.ValueToolTipStyle; }
		}
		protected internal TrackBarButtonStyle GetDecrementButtonStyle() {
			TrackBarButtonStyle style = new TrackBarButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBDecrementButtonStyle());
			style.CopyFrom(RenderStyles.TrackBarDecrementButton);
			return style;
		}
		protected internal TrackBarButtonStyle GetIncrementButtonStyle() {
			TrackBarButtonStyle style = new TrackBarButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBIncrementButtonStyle());
			style.CopyFrom(RenderStyles.TrackBarIncrementButton);
			return style;
		}
		protected internal TrackBarTrackElementStyle GetBarHighlightStyle() {
			TrackBarTrackElementStyle style = new TrackBarTrackElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBBarHighlightStyle());
			style.CopyFrom(RenderStyles.TrackBarBarHighlight);
			return style;
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			AppearanceStyle defaultStyle = new AppearanceStyle();
			defaultStyle.CopyFrom(RenderStyles.GetDefaultTrackBarStyle());
			return defaultStyle;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.TrackBar);
			return style;
		}
		protected internal EditButtonStyle GetScaleStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBScaleStyle());
			style.CopyFrom(RenderStyles.TrackBarScale);
			return style;
		}
		protected internal EditButtonStyle GetTrackStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBTrackStyle());
			style.CopyFrom(RenderStyles.TrackBarTrack);
			return style;
		}
		protected internal AppearanceStyle GetSystemStyle(string styleName) {
			return RenderStyles.CreateTrackBarDefaultStyle<AppearanceStyle>(styleName);
		}
		protected internal TrackBarTickStyle GetItemStyle() {
			TrackBarTickStyle style = new TrackBarTickStyle();
			style.CopyFrom(RenderStyles.GetDefaultTrackBarItemStyle());
			style.CopyFrom(RenderStyles.TrackBarItem);
			return style;
		}
		protected internal TrackBarTickStyle GetSelectedItemStyle() {
			TrackBarTickStyle style = new TrackBarTickStyle();
			style.CopyFrom(RenderStyles.GetDefaultTrackBarSelectedItemStyle());
			style.CopyFrom(RenderStyles.TrackBarSelectedItem);
			return style;
		}
		protected internal TrackBarValueToolTipStyle GetValueToolTipStyle() {
			TrackBarValueToolTipStyle style = new TrackBarValueToolTipStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBValueToolTipStyle());
			style.CopyFrom(RenderStyles.TrackBarValueToolTip);
			return style;
		}
		protected internal TrackBarTickStyle GetSelectedTickStyle() {
			TrackBarTickStyle style = new TrackBarTickStyle();
			style.CopyFrom(RenderStyles.GetDefaultTrackBarSelectedTickStyle());
			style.CopyFrom(RenderStyles.TrackBarSelectedTick);
			return style;
		}
		protected internal TrackBarButtonStyle GetSecondaryDragHandleStyle() {
			TrackBarButtonStyle style = new TrackBarButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBSecondaryDragHandleStyle());
			style.CopyFrom(RenderStyles.TrackBarSecondaryDragHandle);
			return style;
		}
		protected internal TrackBarButtonStyle GetMainDragHandleStyle() {
			TrackBarButtonStyle style = new TrackBarButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultTBMainDragHandleStyle());
			style.CopyFrom(RenderStyles.TrackBarMainDragHandle);
			return style;
		}
		protected internal TrackBarTickStyle GetTickStyle(bool isLarge) {
			TrackBarTickStyle style = new TrackBarTickStyle();
			style.CssClass += string.Format(" {0}", isLarge ? EditorStyles.TBLargeTickSystemClassName :
				EditorStyles.TBSmallTickSystemClassName);
			if(isLarge) {
				style.CopyFrom(RenderStyles.GetDefaultTBLargeTickStyle());
				style.CopyFrom(RenderStyles.TrackBarLargeTick);
			}
			else {
				style.CopyFrom(RenderStyles.GetDefaultTBSmallTickStyle());
				style.CopyFrom(RenderStyles.TrackBarSmallTick);
			}
			return style;
		}
		protected internal AppearanceStyle GetLabelStyle(bool isFirstLabel) {
			AppearanceStyle style = new AppearanceStyle();
			if(ScalePosition == ScalePosition.LeftOrTop || (ScalePosition == ScalePosition.Both && isFirstLabel)) {
				style.CopyFrom(RenderStyles.GetDefaultTBLeftTopLabelStyle());
				style.CopyFrom(RenderStyles.TrackBarLeftTopLabel);
			}
			else {
				style.CopyFrom(RenderStyles.GetDefaultTBRightBottomLabelStyle());
				style.CopyFrom(RenderStyles.TrackBarRightBottomLabel);
			}
			return style;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarValueToolTipFormatString"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("ValueToolTip"), DefaultValue(""), AutoFormatEnable, NotifyParentProperty(true)]
		public string ValueToolTipFormatString {
			get { return Properties.ValueToolTipFormatString; }
			set {
				Properties.ValueToolTipFormatString = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarValueToolTipPosition"),
#endif
		Category("ValueToolTip"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(ValueToolTipPosition.LeftOrTop), AutoFormatEnable, NotifyParentProperty(true)]
		public ValueToolTipPosition ValueToolTipPosition {
			get { return Properties.ValueToolTipPosition; }
			set { Properties.ValueToolTipPosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarMaxValue"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(typeof(decimal), "100"), AutoFormatEnable, NotifyParentProperty(true)]
		public decimal MaxValue {
			get { return Properties.MaxValue; }
			set {
				Properties.MaxValue = value;
				if(DesignMode && !IsLoading()) {
					CheckRestrictions();
					CheckPositionRangeAndCorrect();
				}
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarMinValue"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(typeof(decimal), "0"), AutoFormatEnable, NotifyParentProperty(true)]
		public decimal MinValue {
			get { return Properties.MinValue; }
			set {
				Properties.MinValue = value;
				if(DesignMode && !IsLoading()) {
					CheckRestrictions();
					CheckPositionRangeAndCorrect();
				}
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarPosition"),
#endif
		Bindable(true, BindingDirection.TwoWay), DefaultValue(typeof(decimal), "-1")]
		public decimal Position {
			get { return GetIndexByValue(); }
			set {
				CheckPosition(value, "Position");
				SavePosition(value);
				Value = GetValueByIndex(value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTrackBarValue")]
#endif
		public override object Value {
			get { return base.Value; }
			set {
				if (IsItemMode || IsValidValue(value))
					base.Value = value;
				else
					base.Value = MinValue;
			}
		}
		protected bool IsValidValue(object value) {
			decimal decimalValue = 0;
			return value != null && decimal.TryParse(value.ToString(), out decimalValue) && (decimalValue >= MinValue && decimalValue <= MaxValue || (!DesignMode || IsLoading()));
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarScaleLabelFormatString"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue("{0}"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		AutoFormatEnable, NotifyParentProperty(true)]
		public string ScaleLabelFormatString {
			get { return Properties.ScaleLabelFormatString; }
			set { Properties.ScaleLabelFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarPositionEnd"),
#endif
		Bindable(true, BindingDirection.TwoWay), DefaultValue(typeof(decimal), "0")]
		public decimal PositionEnd {
			get { return GetDecimalProperty("PositionEnd", MinValue); }
			set {
				CheckPosition(value, "PositionEnd");
				SetDecimalProperty("PositionEnd", MinValue, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarPositionStart"),
#endif
		Bindable(true, BindingDirection.TwoWay), DefaultValue(typeof(decimal), "-1")]
		public decimal PositionStart {
			get { return Position; }
			set {
				CheckPosition(value, "PositionStart");
				Position = value; 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTrackBarStep"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		AutoFormatEnable, NotifyParentProperty(true), DefaultValue(typeof(decimal), "2")]
		public decimal Step {
			get { return Properties.Step; }
			set { Properties.Step = value; }
		}
		protected void CheckPosition(decimal value, string propertyName) {
			if(DesignMode && !IsLoading()) {
				if(IsItemMode)
					CommonUtils.CheckValueRange((double)value, 0, Items.Count - 1, propertyName);
				else
					CommonUtils.CheckValueRange((double)value, (double)MinValue, (double)MaxValue, propertyName);
			}
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterFormatterScript();
			RegisterIncludeScript(typeof(ASPxTrackBar), TrackBarScriptResourceName);
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			helper.AddStyle(GetDecrementButtonStyle().DisabledStyle, DecrementButtonId, new string[0],
				GetDecrementButtonImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetIncrementButtonStyle().DisabledStyle, IncrementButtonId, new string[0],
				GetIncrementButtonImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetMainDragHandleStyle().DisabledStyle, MainDragHandleId, new string[0],
				GetMainDragHandleImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetSecondaryDragHandleStyle().DisabledStyle, SecondaryDragHandleId, new string[0],
				GetSecondaryDragHandleImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetDecrementButtonStyle().HoverStyle, DecrementButtonId, new string[0],
				GetDecrementButtonImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetIncrementButtonStyle().HoverStyle, IncrementButtonId, new string[0],
				GetIncrementButtonImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetMainDragHandleStyle().HoverStyle, MainDragHandleId, new string[0],
				GetMainDragHandleImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetSecondaryDragHandleStyle().HoverStyle, SecondaryDragHandleId, new string[0],
				GetSecondaryDragHandleImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
		 }
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetDecrementButtonStyle().PressedStyle, DecrementButtonId, new string[0],
				GetDecrementButtonImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetIncrementButtonStyle().PressedStyle, IncrementButtonId, new string[0],
				GetIncrementButtonImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetMainDragHandleStyle().PressedStyle, MainDragHandleId, new string[0],
				GetMainDragHandleImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
			helper.AddStyle(GetSecondaryDragHandleStyle().PressedStyle, SecondaryDragHandleId, new string[0],
				GetSecondaryDragHandleImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTrackBar";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(ValueToolTipPosition == ValueToolTipPosition.None)
				stb.AppendFormat("{0}.appearValueToolTip = false;\n", localVarName);
			if(!IsNormalDirection)
				stb.AppendFormat("{0}.direction = '{1}';\n", localVarName, Direction.Reversed);
			if(!EnableAnimation)
				stb.AppendFormat("{0}.animationEnabled = false;\n", localVarName);
			if(!AllowMouseWheel)
				stb.AppendFormat("{0}.enableMouseWheel = false;\n", localVarName);
			if(Orientation == Orientation.Vertical)
				stb.AppendFormat("{0}.isHorizontal = false;\n", localVarName);
			if(IsItemMode)
				stb.AppendFormat("{0}.items = {1};\n", localVarName, Items.Serialize());
			stb.AppendFormat("{0}.largeTickInterval = {1};\n", localVarName, HtmlConvertor.ToScript(LargeTickInterval));
			if(LargeTickEndValue != MaxValue)
				stb.AppendFormat("{0}.largeTickEndValue = {1};\n", localVarName, HtmlConvertor.ToScript(LargeTickEndValue));
			if(LargeTickStartValue != MinValue)
				stb.AppendFormat("{0}.largeTickStartValue = {1};\n", localVarName, HtmlConvertor.ToScript(LargeTickStartValue));
			stb.AppendFormat("{0}.maxValue = {1};\n", localVarName, HtmlConvertor.ToScript(MaxValue));
			stb.AppendFormat("{0}.minValue = {1};\n", localVarName, HtmlConvertor.ToScript(MinValue));
			if(ScalePosition != ScalePosition.None && ScaleLabelHighlightMode != ScaleLabelHighlightMode.None)
				GetScaleElementSelectedStyle(stb, localVarName);
			if(ScaleLabelFormatString != GetDefaultFormatString())
				stb.AppendFormat("{0}.scaleLabelFormatString = '{1}';\n", localVarName, ScaleLabelFormatString);
			if(ScaleLabelHighlightMode != ScaleLabelHighlightMode.None)
				stb.AppendFormat("{0}.scaleLabelHighlightMode = '{1}';\n", localVarName, ScaleLabelHighlightMode);
			if(ScalePosition != ScalePosition.RightOrBottom)
				stb.AppendFormat("{0}.scalePosition = '{1}';\n", localVarName, ScalePosition.ToString());
			if(!ShowDragHandles)
				stb.AppendFormat("{0}.showDragHandles = false;\n", localVarName);
			stb.AppendFormat("{0}.smallTickFrequency = {1};\n", localVarName, SmallTickFrequency);
			stb.AppendFormat("{0}.step = {1};\n", localVarName, HtmlConvertor.ToScript(Step));
			if(ValueChangedDelay != 0)
				stb.AppendFormat("{0}.valueChangedDelay = {1};\n", localVarName, ValueChangedDelay);
			GetValueToolTipFormatString(stb, localVarName);
			if(ValueToolTipPosition != ValueToolTipPosition.LeftOrTop)
				stb.AppendFormat("{0}.valueToolTipPosition = '{1}';\n", localVarName, ValueToolTipPosition);
			if(ValueToolTipPosition != ValueToolTipPosition.None)
				GetValueToolTipStyle(stb, localVarName);
		}
		protected static string GetDefaultFormatString() {
			return "{0}";
		}
		protected void GetScaleElementSelectedStyle(StringBuilder stb, string localVarName) {
			AppearanceStyleBase style = IsItemMode ? GetSelectedItemStyle() : GetSelectedTickStyle();
			if(!string.IsNullOrEmpty(style.CssClass))
				stb.AppendFormat("{0}.selectedClasses=[{1}];\n", localVarName, HtmlConvertor.ToScript(style.CssClass));
			string cssText = style.GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(cssText))
				stb.AppendFormat("{0}.selectedCssArray=[{1}];\n", localVarName, HtmlConvertor.ToScript(cssText));
		}
		protected void GetValueToolTipFormatString(StringBuilder stb, string localVarName) {
			string result = string.Empty;
			if(string.IsNullOrEmpty(ValueToolTipFormatString)) {
				if(IsRightToLeft() && AllowRangeSelection)
					result = "{1}..{0}";
				else
					return;
			}
			else
				result = ValueToolTipFormatString;
			stb.AppendFormat("{0}.valueToolTipFormat = '{1}';\n", localVarName, result);
		}
		protected void GetValueToolTipStyle(StringBuilder stb, string localVarName) {
			TrackBarValueToolTipStyle style = GetValueToolTipStyle();
			string cssText = style.GetStyleAttributes(Page).Value;
			stb.AppendFormat("{0}.valueToolTipStyle=[{1}, {2}];\n", localVarName, HtmlConvertor.ToScript(style.CssClass),
				HtmlConvertor.ToScript(cssText));
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasHoverScripts() {
			return IsScriptEnabled();
		}
		protected override bool HasPressedScripts() {
			return IsScriptEnabled();
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected internal string GetDesignImageUrl(string imageName) {
			if(string.IsNullOrEmpty(GetCssPostFix())) {
				imageName = string.Format("{0}{1}.png", ASPxEditBase.EditImagesResourcePath, imageName);
				return ResourceManager.GetResourceUrl(Page, typeof(ASPxTrackBar), imageName);
			}
			else if(ThemesProvider.IsThemePublic(GetCssPostFix())) {
				string spriteImageFilePath = string.Format(GetSpriteCssFilePath(), "Editors");
				spriteImageFilePath = spriteImageFilePath.Replace(".css", ".png");
				spriteImageFilePath = ResourceManager.GetResourceUrl(Page, spriteImageFilePath);
				return spriteImageFilePath.Replace(ImagesBase.SpriteImageName, imageName);
			}
			return string.Empty;
		}
		protected decimal GetIndexByValue() {
			object value = Value;
			if(IsItemMode) {
				TrackBarItem item = Items.FindByValue(value);
				return item != null ? item.Index : -1;
			}
			else
				return value != null ? decimal.Parse(value.ToString()) : MinValue;
		}
		protected internal object GetValueByIndex(decimal index) {
			return IsItemMode ? Items[(int)index].Value : index;
		}
		protected internal bool IsItemMode {
			get {
				return Properties.IsItemMode;
			}
		}
		protected internal bool IsNormalDirection {
			get {
				return !IsRightToLeft() && Direction == Direction.Normal ||
					IsRightToLeft() && Direction == Direction.Reversed; 
			}
		}
		protected internal bool ShowSmallTicks {
			get { return SmallTickFrequency > 1; }
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			RestorePosition();
		}
		protected internal new TrackBarProperties Properties {
			get { return (TrackBarProperties)base.Properties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new TrackBarProperties(this);
		}
		protected override bool IsWebSourcesRegisterRequired() {
			return true;
		}
		protected virtual TrackBarControl CreateTrackBarControl() {
			return new TrackBarControl(this);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(CreateTrackBarControl());
		}
		protected override bool IsRightToLeft() {
			return DesignMode ? false : base.IsRightToLeft();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			trackBarDataHelper.OnViewStateLoaded();
		}
		protected internal override void ValidateProperties() {
			CheckRestrictions();
			bool isValueCorrect = !IsItemMode || (IsItemMode && Position != -1); 
			if(isValueCorrect) {
				CommonUtils.CheckValueRange((double)Position, (double)MinValue, (double)MaxValue, 
					AllowRangeSelection ? "PositionStart" : "Position");
			}
			if(AllowRangeSelection) {
				CommonUtils.CheckValueRange((double)PositionEnd, (double)MinValue, (double)MaxValue, "PositionEnd");
				if(isValueCorrect && PositionStart > PositionEnd)
				throw new ArgumentException(string.Format(StringResources.ASPxEdit_InvalidRange,
					"PositionEnd", "PositionStart"));
			}
		}
		protected internal void CheckPositionRangeAndCorrect() {
			if(Position < MinValue)
				Position = MinValue;
			else if(Position > MaxValue)
				Position = MaxValue;
			if(PositionStart < MinValue)
				PositionStart = MinValue;
			else if(PositionStart > MaxValue)
				PositionStart = MaxValue;
			if(PositionEnd < MinValue)
				PositionEnd = MinValue;
			else if(PositionEnd > MaxValue)
				PositionEnd = MaxValue;
			PropertyChanged("Position");
			PropertyChanged("PositionStart");
			PropertyChanged("PositionEnd");
		}
		protected void CheckRestrictions() {
			if(MaxValue < MinValue)
				throw new ArgumentException(string.Format(StringResources.ASPxEdit_InvalidRange, "MaxValue", "MinValue"));
		}
		protected internal override string GetAssociatedControlID() {
			return string.Format("{0}_{1}", ClientID, InputId);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.TrackBarItemsOwner"; } }
	}
}
