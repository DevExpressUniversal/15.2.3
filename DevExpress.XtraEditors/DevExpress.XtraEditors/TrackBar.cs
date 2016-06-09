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
#if DXWhidbey
using System.Collections.Generic;
#endif
using System.Text;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.Accessibility;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Design;
using System.Drawing.Drawing2D;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraPrinting;
using System.Windows.Forms.Layout;
using System.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing.Helpers;
using System.Collections;
using System.Security;
using DevExpress.XtraEditors.Native;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.Utils.Gesture;
namespace DevExpress.XtraEditors.Repository {
	public enum SmallChangeUseMode { ArrowKeys, ArrowKeysAndMouse }
	[ListBindable(false)]
	public class TrackBarLabelCollection : CollectionBase {
		public event CollectionChangeEventHandler CollectionChanged;
		int lockUpdate;
		public TrackBarLabelCollection() { this.lockUpdate = 0; }
		public TrackBarLabel this[int index] {
			get { return List[index] as TrackBarLabel; }
			set { List[index] = value; }
		}
		public TrackBarLabel IsItemExist(int _Value) {
			TrackBarLabel label = null;
			foreach(TrackBarLabel trackBarLabel in List) {
				if(trackBarLabel.Value == _Value) {
					label = trackBarLabel;
					break;
				}
			}
			if(label != null) return label;
			return new TrackBarLabel("", _Value);
		}
		public virtual void Assign(TrackBarLabelCollection collection) {
			if(collection == null) return;
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < collection.Count; n++) {
					Add(collection[n].Clone() as TrackBarLabel);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void AddRange(TrackBarLabel[] labels) {
			BeginUpdate();
			try {
				foreach(TrackBarLabel label in labels) Add(label);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void Add(TrackBarLabel label) {
			List.Add(label);
		}
		public virtual void Insert(int index, TrackBarLabel label) {
			List.Insert(index, label);
		}
		public virtual void Remove(TrackBarLabel label) {
			List.Remove(label);
		}
		public int IndexOf(TrackBarLabel label) {
			return List.IndexOf(label);
		}
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected override void OnInsertComplete(int index, object value) {
			TrackBarLabel ri = value as TrackBarLabel;
			ri.Changed += new EventHandler(OnLabelChanged);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		protected override void OnRemoveComplete(int index, object value) {
			TrackBarLabel ri = value as TrackBarLabel;
			ri.Changed -= new EventHandler(OnLabelChanged);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			TrackBarLabel ri = oldValue as TrackBarLabel;
			ri.Changed -= new EventHandler(OnLabelChanged);
			ri = newValue as TrackBarLabel;
			ri.Changed += new EventHandler(OnLabelChanged);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, newValue));
		}
		protected override void OnClear() {
			if(Count == 0) return;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void OnClearComplete() {
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected virtual void OnLabelChanged(object sender, EventArgs e) {
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdate != 0) return;
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
	}
	public class TrackBarLabel : ICloneable {
		public event EventHandler Changed;
		string _label;
		int _value;
		bool visible;
		public TrackBarLabel() : this(null, 0, true) { }
		public TrackBarLabel(string label, int _value) : this(label, _value, true) { }
		public TrackBarLabel(string label, int _value, bool visible) {
			this._label = label == null ? string.Empty : label;
			this._value = _value;
			this.visible = visible;
		}
		[ Localizable(true)]
		public virtual string Label {
			get { return _label; }
			set {
				if(value == null) value = string.Empty;
				if(Label != value) {
					_label = value;
					LabelChanged();
				}
			}
		}
		[ DefaultValue(0)]
		public virtual int Value {
			get { return _value; }
			set {
				if(Value != value) {
					_value = value;
					LabelChanged();
				}
			}
		}
		[ DefaultValue(true)]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(Visible != value) {
					visible = value;
					LabelChanged();
				}
			}
		}
		public virtual object Clone() { return new TrackBarLabel(Label, Value, Visible); }
		protected virtual void LabelChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		public override string ToString() {
			if(Label != string.Empty) return Label;
			return base.ToString();
		}
	}
	public class RepositoryItemTrackBar : RepositoryItem {
		TrackBarLabelCollection labels;
		AppearanceObject labelAppearance;
		static readonly object autoSizeChanged = new object();
		static readonly object valueChanged = new object();
		static readonly object beforeShowValueToolTip = new object();
		bool autoSize;
		bool showValueToolTip;
		int largeChange;
		int maximum;
		int minimum;
		Orientation orientation;
		bool rightToLeftLayout;
		int smallChange;
		int tickFrequency;
		bool invertLayout;
		SmallChangeUseMode smallChangeUseMode;
		TickStyle tickStyle;
		VertAlignment alignment;
		bool highlightSelectedRange;
		bool showLabels;
		bool showLabelsForHiddenTicks;
		int distanceFromTickToLabel;
		public RepositoryItemTrackBar() {
			this.autoSize = true;
			this.largeChange = 5;
			this.maximum = 10;
			this.minimum = 0;
			this.smallChange = 1;
			this.tickFrequency = 1;
			this.smallChangeUseMode = SmallChangeUseMode.ArrowKeys;
			this.invertLayout = false;
			this.highlightSelectedRange = true;
			this.tickStyle = TickStyle.BottomRight;
			this.alignment = VertAlignment.Default;
			this.labelAppearance = CreateLabelAppearance("Label");
			this.labels = CreateLabelCollection();
			this.labels.CollectionChanged += OnLabels_CollectionChanged;
			this.showLabels = false;
			this.showLabelsForHiddenTicks = false;
			this.distanceFromTickToLabel = 0;
		}
		protected override void DestroyAppearances() {
			base.DestroyAppearances();
			DestroyAppearance(LabelAppearance);
		}
		protected virtual TrackBarLabelCollection CreateItemCollection() {
			return new TrackBarLabelCollection();
		}
		protected virtual TrackBarLabelCollection CreateLabelCollection() {
			return new TrackBarLabelCollection();
		}
		protected virtual void LabelsLayoutChanged() {
			ChangeOwnerStyle();
			AdjustSize();
			OnAutoSizeChanged(EventArgs.Empty);
		}
		protected virtual void OnLabels_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(AutoSize && ShowLabels) LabelsLayoutChanged();
			OnPropertiesChanged();
		}	  
		protected virtual AppearanceObject CreateLabelAppearance(string name) {
			AppearanceObject res = CreateAppearanceCore(name);
			res.PaintChanged += OnAppearancePaintChanged;
			res.SizeChanged += OnLabelAppearanceSizeChanged;
			TextOptions textOptions = new TextOptions(HorzAlignment.Center, 0, 0, 0);
			res.TextOptions.Assign(textOptions);
			return res;
		}
		protected void OnLabelAppearanceSizeChanged(object sender, EventArgs e) {
			if(IsLoading || IsLockUpdate) return;
			AdjustSize();
			OnPropertiesChanged();
		}
		protected new TrackBarControl OwnerEdit { get { return base.OwnerEdit as TrackBarControl; } }
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			style.ForeColor = Color.Blue;
			return style;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			int pos = (int)ConvertValue(info.EditValue);			
			ITrackBarBrick brick = new TrackBarBrick(pos, Minimum, Maximum);
			SetCommonBrickProperties(brick, info);
			brick.Style = CreateBrickStyle(info, "track");
			if((Maximum - Minimum) == 0) return brick;
			brick.Position = Convert.ToInt32(pos);
			brick.TextValue = ConvertValue(info.EditValue);
			brick.Hint = pos.ToString();
			return brick;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemTrackBar trackBarItem = item as RepositoryItemTrackBar;
			BeginUpdate();
			try {
				base.Assign(item);
				if(trackBarItem == null) return;
				this.autoSize = trackBarItem.autoSize;
				this.largeChange = trackBarItem.largeChange;
				this.highlightSelectedRange = trackBarItem.highlightSelectedRange;
				this.maximum = trackBarItem.maximum;
				this.minimum = trackBarItem.minimum;
				this.orientation = trackBarItem.orientation;
				this.rightToLeftLayout = trackBarItem.rightToLeftLayout;
				this.smallChange = trackBarItem.smallChange;
				this.tickFrequency = trackBarItem.tickFrequency;
				this.tickStyle = trackBarItem.tickStyle;
				this.alignment = trackBarItem.alignment;
				this.showValueToolTip = trackBarItem.showValueToolTip;
				this.labelAppearance.Assign(trackBarItem.labelAppearance);
				this.distanceFromTickToLabel = trackBarItem.distanceFromTickToLabel;
				this.showLabels = trackBarItem.showLabels;
				this.showLabelsForHiddenTicks = trackBarItem.showLabelsForHiddenTicks;
				this.labels = trackBarItem.labels;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(RepositoryItemTrackBar.autoSizeChanged, trackBarItem.Events[RepositoryItemTrackBar.autoSizeChanged]);
			Events.AddHandler(RepositoryItemTrackBar.valueChanged, trackBarItem.Events[RepositoryItemTrackBar.valueChanged]);
			Events.AddHandler(RepositoryItemTrackBar.beforeShowValueToolTip, trackBarItem.Events[RepositoryItemTrackBar.beforeShowValueToolTip]);
		}
#if DEBUGTEST
		public void TestSetOwnerEdit(TrackBarControl obj) { SetOwnerEdit(obj); }
#endif
		void AdjustSize() {
			if(OwnerEdit == null) return;
			OwnerEdit.AdjustSize();
		}
		void ResetLabelAppearance() { LabelAppearance.Reset(); }
		bool ShouldSerializeLabelAppearance() { return LabelAppearance.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject LabelAppearance {
			get { return labelAppearance; }
		}
		[ DXCategory(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)), Localizable(true)]
		public virtual TrackBarLabelCollection Labels { get { return labels; } }
		[ DefaultValue(true), DXCategory(CategoryName.Appearance)]
		public bool HighlightSelectedRange {
			get { return highlightSelectedRange; }
			set {
				if(HighlightSelectedRange == value) return;
				highlightSelectedRange = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(false), DXCategory(CategoryName.Behavior)]
		public bool ShowValueToolTip {
			get { return showValueToolTip; }
			set {
				if(ShowValueToolTip == value) return;
				showValueToolTip = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(0), DXCategory(CategoryName.Behavior)]
		public virtual int DistanceFromTickToLabel {
			get { return distanceFromTickToLabel; }
			set {
				if(DistanceFromTickToLabel == value) return;
				distanceFromTickToLabel = value;
				if(AutoSize && ShowLabels) LabelsLayoutChanged();
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "TrackBarControl"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarAlignment"),
#endif
 DefaultValue(VertAlignment.Default), RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Appearance)]
		public VertAlignment Alignment {
			get { return alignment; }
			set {
				if(Alignment == value) return;
				alignment = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarMaximum"),
#endif
 DefaultValue(10), RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Behavior)]
			public virtual int Maximum {
			get { return this.maximum; }
			set {
				if(Maximum == value) return;
				if(!IsLoading && value < Minimum) value = Minimum;
				maximum = value;
				if(ShowLabels && OwnerEdit != null) OwnerEdit.RefreshLabels();
				OnPropertiesChanged();	
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarShowLabels"),
#endif
 DefaultValue(false), RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Behavior), SmartTagProperty("Show Labels", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool ShowLabels {
			get { return showLabels; }
			set {
				if(ShowLabels == value) return;
				showLabels = value;
				if(AutoSize) LabelsLayoutChanged();
				if(value && OwnerEdit != null) OwnerEdit.RefreshLabels();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarShowLabelsForHiddenTicks"),
#endif
 DefaultValue(false), RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Behavior)]
		public virtual bool ShowLabelsForHiddenTicks {
			get { return showLabelsForHiddenTicks; }
			set {
				if(ShowLabelsForHiddenTicks == value) return;
				showLabelsForHiddenTicks = value;
				if(AutoSize) LabelsLayoutChanged();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarMinimum"),
#endif
 DefaultValue(0), RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Behavior)]
		public virtual int Minimum {
			get { return this.minimum; }
			set {
				if(Minimum == value) return;
				if(!IsLoading && value > Maximum) value = Maximum;
				minimum = value;
				if(ShowLabels && OwnerEdit != null) OwnerEdit.RefreshLabels();
				OnPropertiesChanged();
			}
		}
		void ChangeOwnerStyle() {
			if(OwnerEdit != null) {
				if(this.orientation == Orientation.Horizontal) OwnerEdit.ChangeStyleHorizontal(false);
				else OwnerEdit.ChangeStyleVertical(false);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarOrientation"),
#endif
 DefaultValue(Orientation.Horizontal), Localizable(true), DXCategory(CategoryName.Appearance), SmartTagProperty("Orientation", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public Orientation Orientation {
			get { return this.orientation; }
			set {
				if(value != Orientation.Horizontal && value != Orientation.Vertical) value = Orientation.Horizontal;
				if(Orientation == value) return;
				this.orientation = value;
				OnOrientationChanged();
				OnPropertiesChanged();
			}
		}
		protected virtual void OnOrientationChanged() {
				if(OwnerEdit != null) {
				OwnerEdit.OnOrientationChanged();
			}
		}
		protected internal virtual bool ShouldConvertValue { get { return true; } }
		protected internal virtual int ConvertValue(object val) {
			try {
				if(!(val is IConvertible))
					return this.Minimum;
				return CheckValue(Convert.ToInt32(val));
			}
			catch {
			}
			return this.Minimum;
		}
		internal bool GetAutoSize() { return AutoSize && OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarAutoSize"),
#endif
 DefaultValue(true), DXCategory(CategoryName.Behavior)]
		public bool AutoSize {
			get { return autoSize; }
			set {
				if(AutoSize == value) return;
				autoSize = value;
				ChangeOwnerStyle();
				AdjustSize();
				OnAutoSizeChanged(EventArgs.Empty);
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarLargeChange"),
#endif
 DefaultValue(5), DXCategory(CategoryName.Behavior)]
		public int LargeChange {
			get { return largeChange; }
			set {
				if(value < 0) value = 0;
				if(LargeChange == value) return;
				largeChange = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarSmallChange"),
#endif
 DefaultValue(1), DXCategory(CategoryName.Appearance)]
		public int SmallChange {
			get { return smallChange; }
			set {
				if(value < 0) value = 0;
				if(SmallChange == value) return;
				smallChange = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarTickFrequency"),
#endif
 DefaultValue(1), DXCategory(CategoryName.Appearance)]
		public virtual int TickFrequency {
			get { return tickFrequency; }
			set {
				value = Math.Min(value, Maximum - Minimum);
				if(value <= 0) value = 1;
				if(TickFrequency == value) return;
				tickFrequency = value;
				OnPropertiesChanged();
			}
		}
		void ResetSmallChangeUseMode() { SmallChangeUseMode = SmallChangeUseMode.ArrowKeys; }
		bool ShouldSerializeSmallChangeUseMode() { return SmallChangeUseMode != SmallChangeUseMode.ArrowKeys; }
		[ DXCategory(CategoryName.Appearance)]
		public virtual SmallChangeUseMode SmallChangeUseMode {
			get { return smallChangeUseMode; }
			set {
				if(SmallChangeUseMode == value) return;
				smallChangeUseMode = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarInvertLayout"),
#endif
 DefaultValue(false), DXCategory(CategoryName.Appearance)]
		public virtual bool InvertLayout {
			get { return invertLayout; }
			set {
				if(InvertLayout == value) return;
				invertLayout = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarTickStyle"),
#endif
 DefaultValue(TickStyle.BottomRight), DXCategory(CategoryName.Appearance), SmartTagProperty("TickStyle", "", SmartTagActionType.RefreshAfterExecute)]
		public virtual TickStyle TickStyle {
			get { return this.tickStyle; }
			set {
				if(value != TickStyle.None && value != TickStyle.TopLeft && value != TickStyle.BottomRight && value != TickStyle.Both)
					value = TickStyle.None;
				if(TickStyle == value) return;
				tickStyle = value;
				if(AutoSize && ShowLabels) LabelsLayoutChanged();
				OnPropertiesChanged();
			}
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			AdjustSize();
		}
		protected internal virtual int CheckValue(int val) {
			if(val < Minimum) return Minimum;
			if(val > Maximum) return Maximum;
			return val;
		}
		protected internal virtual new BaseEditViewInfo CreateViewInfo() { return new TrackBarViewInfo(this); }
		protected internal virtual new BaseEditPainter CreatePainter() { return new TrackBarPainter(); }
		protected internal virtual void OnAutoSizeChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[RepositoryItemTrackBar.autoSizeChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void OnValueChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[RepositoryItemTrackBar.valueChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoHeight { get { return false; } }
		[ DXCategory(CategoryName.Events)]
		public event TrackBarValueToolTipEventHandler BeforeShowValueToolTip {
			add { Events.AddHandler(RepositoryItemTrackBar.beforeShowValueToolTip, value); }
			remove { Events.AddHandler(RepositoryItemTrackBar.beforeShowValueToolTip, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarAutoSizeChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler AutoSizeChanged {
			add { Events.AddHandler(RepositoryItemTrackBar.autoSizeChanged, value); }
			remove { Events.AddHandler(RepositoryItemTrackBar.autoSizeChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTrackBarValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ValueChanged {
			add { Events.AddHandler(RepositoryItemTrackBar.valueChanged, value); }
			remove { Events.RemoveHandler(RepositoryItemTrackBar.valueChanged, value); }
		}
		protected internal override bool NeededKeysContains(Keys key) {
			switch(key) {
				case Keys.Left | Keys.Control:
				case Keys.Right | Keys.Control:
				case Keys.Up | Keys.Control:
				case Keys.Down | Keys.Control:
				case Keys.Home:
				case Keys.End:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.OemMinus:
				case Keys.Oemplus:
				case Keys.Add:
				case Keys.Subtract:
					return true;
				default:
					return base.NeededKeysContains(key);
			}
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseValueChanged(e);
		}
		protected internal virtual void RaiseValueChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[valueChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseBeforeShowValue(TrackBarValueToolTipEventArgs e) {
			TrackBarValueToolTipEventHandler handler = (TrackBarValueToolTipEventHandler)this.Events[beforeShowValueToolTip];
			if(handler != null) handler(GetEventSender(), e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FormatInfo DisplayFormat {
			get { return base.DisplayFormat; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FormatInfo EditFormat {
			get { return base.EditFormat; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string NullText {
			get { return base.NullText; }
			set { base.NullText = value; }
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class TrackBarHelper {
		TrackBarViewInfo viewInfo;
		public TrackBarHelper(TrackBarViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected TrackBarViewInfo ViewInfo { get { return viewInfo; } }
		public bool IsHorizontal { get { return ViewInfo.Item.Orientation == Orientation.Horizontal; } }
		public int ClientHeight { get { return IsHorizontal ? ViewInfo.ClientRect.Height : ViewInfo.ClientRect.Width; } }
		public int ClientWidth { get { return IsHorizontal ? ViewInfo.ClientRect.Width : ViewInfo.ClientRect.Height; } }
		public int ContentWidth { get { return IsHorizontal ? ViewInfo.ContentRect.Width : ViewInfo.ContentRect.Height; } }
		public int ContentHeight { get { return IsHorizontal ? ViewInfo.ContentRect.Height : ViewInfo.ContentRect.Width; } }
		public int ClientX { get { return IsHorizontal ? ViewInfo.ClientRect.X - ViewInfo.Bounds.X : ViewInfo.ClientRect.Y - ViewInfo.Bounds.Y; } }
		public int ClientY { get { return IsHorizontal ? ViewInfo.ClientRect.Y - ViewInfo.Bounds.Y : ViewInfo.ClientRect.X - ViewInfo.Bounds.X; } }
		public int ContentX { get { return IsHorizontal ? ViewInfo.ContentRect.X - ViewInfo.Bounds.X : ViewInfo.ContentRect.Y - ViewInfo.Bounds.Y; } }
		public int ContentY { get { return IsHorizontal ? ViewInfo.ContentRect.Y - ViewInfo.Bounds.Y : ViewInfo.ContentRect.X - ViewInfo.Bounds.X; } }
		public Rectangle ClientRectangle { get { return new Rectangle(ClientX, ClientY, ClientWidth, ClientHeight); } }
		public Rectangle ContentRectangle { get { return new Rectangle(ContentX, ContentY, ContentWidth, ContentHeight); } }
		public int GetRectHeight(Rectangle rect) { return IsHorizontal? rect.Height: rect.Width; }
		public int GetRectWidth(Rectangle rect) { return IsHorizontal ? rect.Width : rect.Height; }
		public int ThumbHeight { get { return GetRectHeight(ViewInfo.ThumbBounds); } }
		public Rectangle Rotate(Rectangle r) {
			if(IsHorizontal) return r;
			Rectangle res = r;
			res.Offset(-ViewInfo.Bounds.X, -ViewInfo.Bounds.Y);
			res = new Rectangle(res.Y, ViewInfo.ClientRect.Height - (res.X + res.Width), res.Height, res.Width);
			res.Offset(ViewInfo.Bounds.X, ViewInfo.Bounds.Y);
			return res;
		}
		public Point Rotate(Point p) { 
			if(IsHorizontal) return p;
			Point res = p;
			res.Offset(-ViewInfo.Bounds.X, -ViewInfo.Bounds.Y);
			res = new Point(res.Y, ViewInfo.ClientRect.Height - res.X);
			res.Offset(ViewInfo.Bounds.X, ViewInfo.Bounds.Y);
			return res;
		}
		public Size Size(Size sz) {
			if(IsHorizontal) return sz;
			return new Size(sz.Height, sz.Width);
		}
		public Point RotateAndMirror(Point p, int axisY, bool bMirror) {
			Point mirrorP = p;
			if(bMirror) {
				mirrorP.X = p.X;
				mirrorP.Y = axisY - (p.Y - axisY);
			}
			if(IsHorizontal) return mirrorP;
			Point res = mirrorP;
			res.Offset(-ViewInfo.Bounds.X, -ViewInfo.Bounds.Y);
			res = new Point(res.Y, ViewInfo.ClientRect.Height - res.X);
			res.Offset(ViewInfo.Bounds.X, ViewInfo.Bounds.Y);
			return res;
		}
		public int GetWidth(Size size) { return IsHorizontal ? size.Width : size.Height; }
	}
	public class TrackBarInfoCalculator {
		static int distanceFromTopToThumb = 2;
		static int distanceFromThumbToTicks = 2;
		static int distanceFromTicksToBottom = 3;
		static int distanceFromLeftToThumb = 3;
		static int tickHeight = 3;
		static int thumbHeight = 20;
		TrackBarViewInfo viewInfo;
		TrackBarObjectPainter trackPainter;
		int realDistanceFromTopToThumb;
		int realThumbHeight;
		int realDistanceFromThumbToTicks;
		int realTickHeight;
		int realDistanceFromTicksToBottom;
		public virtual int DistanceFromTopToThumb { get { return distanceFromTopToThumb; } }
		public virtual int DistanceFromThumbToTicks { get { return distanceFromThumbToTicks; } }
		public virtual int DistanceFromTicksToBottom { get { return distanceFromTicksToBottom; } }
		public virtual int DistanceFromLeftToThumb { get { return distanceFromLeftToThumb; } }
		public virtual int TickHeight { get { return tickHeight; } }
		public virtual int ThumbHeight { get { return thumbHeight; } }
		public TrackBarInfoCalculator(TrackBarViewInfo viewInfo, TrackBarObjectPainter trackPainter) {
			this.viewInfo = viewInfo;
			this.trackPainter = trackPainter;
		}
		public virtual TrackBarViewInfo ViewInfo { get { return viewInfo; } }
		public virtual TrackBarObjectPainter TrackPainter { get { return trackPainter; } }
		public int RealDistanceFromTopToThumb { get { return realDistanceFromTopToThumb; } set { realDistanceFromTopToThumb = value; } }
		public int RealDistanceFromThumbToTicks { get { return realDistanceFromThumbToTicks; } set { realDistanceFromThumbToTicks = value; } }
		public int RealDistanceFromTicksToBottom { get { return realDistanceFromTicksToBottom; } set { realDistanceFromTicksToBottom = value; } }
		public int RealTickHeight { get { return realTickHeight; } set { realTickHeight = value; } }
		public int RealThumbHeight { get { return realThumbHeight; } set { realThumbHeight = value; } }
		[Obsolete("Use SummaryRealDistances"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int SummaryRealDistancies { get { return SummaryRealDistances; } }
		public int SummaryRealDistances { get { return RealDistanceFromTopToThumb + RealDistanceFromThumbToTicks + RealDistanceFromTicksToBottom; } }
		public int SummaryDistances { get { return DistanceFromTopToThumb + DistanceFromThumbToTicks + DistanceFromTicksToBottom; } }
		public virtual int RealTrackLineHeight { get { return ScaleHeightValue(GetLineSize().Height); } }
		protected virtual int ActualClientHeight { get { return Math.Min(ViewInfo.TrackBarHelper.ClientHeight, ViewInfo.PreferredDimension); } }
		protected virtual int ActualContentBottom { 
			get {
				TrackBarHelper tb = ViewInfo.TrackBarHelper;
				return Math.Min(ViewInfo.Bounds.Y + tb.ContentRectangle.Bottom, ViewInfo.Bounds.Y + tb.ClientRectangle.Top + ViewInfo.PreferredDimension - (tb.ClientRectangle.Bottom - tb.ContentRectangle.Bottom)); 
			} 
		}
		protected internal virtual SkinElementInfo GetLineInfo() {
			return null;
		}
		protected internal virtual Size GetLineSize() {
			SkinElementInfo lineInfo = GetLineInfo();
			if(lineInfo == null || lineInfo.Element == null || lineInfo.Element.Image == null) return new Size(5, 5);
			return lineInfo.Element.Image.GetImageBounds(0).Size;
		}
		protected internal virtual int CalcBestHeight() {
			return Math.Max(SummaryDistances + TrackPainter.GetThumbBestHeight(ViewInfo), ViewInfo.PreferredDimension);
		}
		protected virtual int ScaleHeightValue(int val) { return Math.Min(val, val * ViewInfo.TrackBarHelper.ClientHeight / CalcBestHeight()); }
		protected internal virtual int CalcBestContentHeight() { return SummaryDistances + ThumbHeight + TickHeight; }
		protected virtual int[] CreateHeights() { return new int[] { RealDistanceFromTopToThumb, RealDistanceFromThumbToTicks, RealTickHeight, RealDistanceFromTicksToBottom }; }
		protected virtual int[] CreateMinHeights() { return new int[] { 0, 2, 1, 1 }; }
		protected virtual int UpdateHeights(int[] heights, int[] minHeights, int delta) {
			int index = 0, lastDelta;
			for(; delta > 0; ) {
				lastDelta = delta;
				for(index = 0; delta > 0 && index < heights.Length; index++) {
					if(heights[index] > minHeights[index]) {
						heights[index]--;
						delta--;
					}
				}
				if(lastDelta == delta) return delta;
				lastDelta = delta;
			}
			return delta;
		}
		protected virtual void UpdateRealHeights(int[] heights) {
			RealDistanceFromTopToThumb = heights[0];
			RealDistanceFromThumbToTicks = heights[1];
			RealTickHeight = heights[2];
			RealDistanceFromTicksToBottom = heights[3];
		}
		public virtual void CalcHeights() {
			int bestHeight = CalcBestContentHeight();
			int actualHeight = ViewInfo.TrackBarHelper.ContentHeight;
			CalcBestHeights();
			if(actualHeight >= bestHeight) return;
			int[] heights = CreateHeights();
			int delta = UpdateHeights(heights, CreateMinHeights(), bestHeight - actualHeight);
			UpdateRealHeights(heights);
			if(delta != 0) RealThumbHeight -= delta;
			return;
		}
		protected internal virtual void CalcBestHeights() {
			RealDistanceFromTopToThumb = DistanceFromTopToThumb;
			RealThumbHeight = ThumbHeight;
			RealDistanceFromThumbToTicks = DistanceFromThumbToTicks;
			RealTickHeight = TickHeight;
			RealDistanceFromTicksToBottom = DistanceFromTicksToBottom;
		}
		protected internal virtual Rectangle CalcTrackLineRect() {
			TrackBarHelper tb = ViewInfo.TrackBarHelper;
			Rectangle trackLine = new Rectangle(ViewInfo.ContentRect.X, ViewInfo.ContentRect.Y, tb.ContentWidth, tb.ContentHeight);
			if(ViewInfo.IsTouchMode) {
				int addedWidth = CalcTouchTrackLineDeflate();
				trackLine.X += addedWidth / 2;
				trackLine.Width = Math.Max(trackLine.Width - addedWidth, 0);
			}
			trackLine.Inflate(-DistanceFromLeftToThumb, 0);
			trackLine.Height = RealTrackLineHeight;
			switch(ViewInfo.TickStyle) {
				case TickStyle.BottomRight:
				case TickStyle.None:
					trackLine.Y = ViewInfo.Bounds.Y + tb.ContentY + RealDistanceFromTopToThumb + ThumbUpperPartHeight - trackLine.Height / 2;
					break;
				case TickStyle.Both:
					trackLine.Y = ViewInfo.Bounds.Y + tb.ClientY + (ActualClientHeight - trackLine.Height) / 2;
					break;
				case TickStyle.TopLeft:
					trackLine.Y = ActualContentBottom - RealDistanceFromTopToThumb - ThumbUpperPartHeight - (trackLine.Height - trackLine.Height / 2);
					break;
			}
			trackLine.Y = ApplyAlignment(trackLine.Y, tb);
			return trackLine;
		}
		protected virtual int CalcTouchTrackLineDeflate() {
			return CalcTouchAddedSize(ViewInfo.ThumbContentBounds.Size).Width;
		}
		protected virtual int PointsRectY { get { return ViewInfo.TrackLineRect.Y + ViewInfo.TrackLineRect.Height / 2 + ThumbLowerPartHeight + RealDistanceFromThumbToTicks; } }
		protected virtual int GetTrackLineRectInflateWidth() { return ViewInfo.TrackBarHelper.GetRectWidth(ViewInfo.ThumbContentBounds); }
		protected virtual int PointsRectOffsetX { get { return ViewInfo.TrackBarHelper.GetRectWidth(ViewInfo.ThumbBounds) / 2; } }
		protected internal virtual Rectangle CalcPointsRect() {
			TrackBarHelper tb = ViewInfo.TrackBarHelper;
			Rectangle pointsRect = Rectangle.Empty;
			pointsRect.Height = RealTickHeight;
			pointsRect.Width = ViewInfo.TrackLineRect.Width;
			pointsRect.Width -= GetTrackLineRectInflateWidth();
			pointsRect.X = ViewInfo.TrackLineRect.X + PointsRectOffsetX;
			pointsRect.Y = PointsRectY;
			return pointsRect;
		}
		protected virtual int ThumbUpperPartHeight { 
			get { return ViewInfo.Orientation == Orientation.Horizontal? ViewInfo.ThumbBounds.Height / 2: ViewInfo.ThumbBounds.Width / 2; } 
		}
		protected virtual int ThumbLowerPartHeight { 
			get { 
				if(ViewInfo.Orientation == Orientation.Horizontal)
					return ViewInfo.ThumbContentBounds.Height - ThumbUpperPartHeight;
				return ViewInfo.ThumbContentBounds.Width - ThumbUpperPartHeight; 
			} 
		}
		protected virtual Rectangle GetThumbBoundsCore() {
			Point[] p = ViewInfo.RectThumbRegion;
			Rectangle r = Rectangle.FromLTRB(p[1].X, p[1].Y, p[3].X, p[3].Y);
			if(r.Height < 0) { r.Y += r.Height; r.Height = Math.Abs(r.Height); }
			if(r.Width < 0) { r.X += r.Width; r.Width = Math.Abs(r.Width); }
			return r;
		}
		public virtual Rectangle GetThumbBounds() {
			return GetThumbBoundsCore();
		}
		protected virtual int ApplyAlignment(int trackLineY, TrackBarHelper tb) {
			if(tb.ClientHeight <= ViewInfo.PreferredDimension) return trackLineY;
			if(ViewInfo.Item.Alignment == VertAlignment.Center) trackLineY = tb.ClientY + (tb.ClientHeight - RealTrackLineHeight) / 2;
			else if(ViewInfo.Item.Alignment == VertAlignment.Bottom) trackLineY = tb.ClientHeight - ViewInfo.PreferredDimension + trackLineY;
			return trackLineY;
		}
		protected internal virtual void TransformPoints(object offsetP1, Point[] polygon) {
			TransformPoints(offsetP1, polygon, ViewInfo.ThumbCriticalHeight, ViewInfo.MirrorPoint);
		}
		protected internal virtual void TransformPoints(object offsetP1, Point[] polygon, Point center) {
			TransformPoints(offsetP1, polygon, ViewInfo.ThumbCriticalHeight, center);
		}
		protected internal virtual void TransformPoints(object offsetP1, Point[] polygon, int criticalHeight, Point center) {
			Point p1 = Point.Empty;
			int pointIndex;
			int[,] offset = (int[,])offsetP1;
			for(pointIndex = 0; pointIndex < polygon.Length; pointIndex++) {
				p1 = center;
				p1.X += ViewInfo.ScaleValue(ViewInfo.TrackBarHelper.ClientHeight, criticalHeight, offset[pointIndex, 0]);
				p1.Y += ViewInfo.ScaleValue(ViewInfo.TrackBarHelper.ClientHeight, criticalHeight, offset[pointIndex, 1]);
				polygon[pointIndex] = ViewInfo.TrackBarHelper.RotateAndMirror(p1, ViewInfo.MirrorPoint.Y, ViewInfo.TickStyle == TickStyle.TopLeft);
			}
		}
		protected internal virtual void ScalePoints(object offsetP1, Point[] polygon) {
			ScalePoints(offsetP1, polygon, ViewInfo.ThumbCriticalHeight, ViewInfo.MirrorPoint);
		}
		protected internal virtual void ScalePoints(object offsetP1, Point[] polygon, Point center) {
			ScalePoints(offsetP1, polygon, ViewInfo.ThumbCriticalHeight, center);
		}
		protected internal virtual void ScalePoints(object offsetP1, Point[] polygon, int criticalHeight, Point center) {
			Point p1 = Point.Empty;
			int pointIndex;
			int[,] offset = (int[,])offsetP1;
			for(pointIndex = 0; pointIndex < polygon.Length; pointIndex++) {
				polygon[pointIndex] = center;
				polygon[pointIndex].X += ViewInfo.ScaleValue(ViewInfo.TrackBarHelper.ClientHeight, criticalHeight, offset[pointIndex, 0]);
				polygon[pointIndex].Y += ViewInfo.ScaleValue(ViewInfo.TrackBarHelper.ClientHeight, criticalHeight, offset[pointIndex, 1]);
			}
		}
		protected internal virtual int[,] ScalePoints(object offsetP1, int criticalHeight) {
			int[,] offset = (int[,])offsetP1;
			for(int pointIndex = 0; pointIndex < offset.Length / 2; pointIndex++) {
				offset[pointIndex, 0] = ViewInfo.ScaleValue(ViewInfo.TrackBarHelper.ClientHeight, criticalHeight, offset[pointIndex, 0]);
				offset[pointIndex, 1] = ViewInfo.ScaleValue(ViewInfo.TrackBarHelper.ClientHeight, criticalHeight, offset[pointIndex, 1]);
			}
			return offset;
		}
		protected virtual int GetThumbY() {
			return ViewInfo.TrackLineRect.Y + ViewInfo.TrackLineRect.Height / 2 - ThumbUpperPartHeight;
		}
		public virtual SkinElementInfo GetThumbElementInfo() { return null; }
		protected virtual Size ScaleSkinThumbSize(Size sz) { return new Size(sz.Width, RealThumbHeight); }
		protected virtual Size GetSkinThumbElementSize() { return ScaleSkinThumbSize(GetSkinThumbElementOriginSize()); }
		protected virtual Size GetSkinThumbElementOriginSize() {
			SkinElementInfo thumbInfo = GetThumbElementInfo();
			if(thumbInfo == null || thumbInfo.Element == null || thumbInfo.Element.Image == null) return GetThumbBoundsCore().Size;
			return thumbInfo.Element.Image.GetImageBounds(0).Size;
		}
		protected virtual Point GetSkinThumbElementOffset() { return ScaleSkinThumbElementOffset(GetSkinThumbElementOriginOffset()); }
		protected virtual Point GetSkinThumbElementOriginOffset() {
			SkinElementInfo thumbInfo = GetThumbElementInfo();
			if(thumbInfo == null) {
				Size sz = GetThumbBoundsCore().Size;
				return new Point(sz.Width / 2, sz.Height / 2);
			}
			return thumbInfo.Element.Offset.Offset;
		}
		protected virtual Point ScaleSkinThumbElementOffset(Point pt) {
			Size sz = GetSkinThumbElementSize(), originSize = GetSkinThumbElementOriginSize();
			if(ThumbHeight == RealThumbHeight) return pt;
			pt.Y = (int)(pt.Y * (float)RealThumbHeight / (float)ThumbHeight);
			return pt;
		}
		public virtual Rectangle GetSkinThumbBounds() {
			Point pt = GetSkinThumbElementOffset();
			Rectangle rect = Rectangle.Empty;
			if(ViewInfo.TickStyle == TickStyle.TopLeft)
				rect = new Rectangle(new Point(ViewInfo.ThumbPos.X - pt.X, GetThumbY()), GetSkinThumbElementSize());
			else
				rect = new Rectangle(new Point(ViewInfo.ThumbPos.X - pt.X, GetThumbY()), GetSkinThumbElementSize());
			return ViewInfo.TrackBarHelper.Rotate(rect);
		}
		static Size TouchMinSize = new Size(36, 36);
		protected internal Rectangle GetThumbClientBounds(Rectangle thumbRectangle) {
			Size addedSize = CalcTouchAddedSize(thumbRectangle.Size);
			thumbRectangle.Y -= addedSize.Height / 2;
			thumbRectangle.X -= addedSize.Width / 2;
			thumbRectangle.Height += addedSize.Height;
			thumbRectangle.Width += addedSize.Width;
			return thumbRectangle;
		}
		protected internal Size CalcTouchAddedSize(Size size) {
			return new Size(Math.Max(0, TouchMinSize.Width - size.Width), Math.Max(0, TouchMinSize.Height - size.Height));
		}
		protected internal Rectangle GetTrackLineClientBounds(Rectangle trackLineRect) {
			Size addedSize = CalcTouchAddedSize(trackLineRect.Size);
			trackLineRect.Y -= addedSize.Height / 2;
			trackLineRect.Height += addedSize.Height;
			return trackLineRect;
		}
	}
	public class TrackBarSkinInfoCalculator : TrackBarInfoCalculator {
		public TrackBarSkinInfoCalculator(TrackBarViewInfo viewInfo, SkinTrackBarObjectPainter trackPainter) : base(viewInfo, trackPainter) { }
		SkinElementInfo GetTrackLineInfo() {
			return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTrackBarTrack], Rectangle.Empty);
		}
		public new SkinTrackBarObjectPainter TrackPainter { get { return base.TrackPainter as SkinTrackBarObjectPainter; } }
		public override int ThumbHeight { get { return GetSkinThumbElementOriginSize().Height; } }
		public override int RealTrackLineHeight { get { return GetLineSize().Height; } }
		protected internal override SkinElementInfo GetLineInfo() { return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTrackBarTrack]); }
		public override SkinElementInfo GetThumbElementInfo() { 
			if(ViewInfo.TickStyle == TickStyle.Both)
				return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTrackBarThumbBoth], Rectangle.Empty);
			return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTrackBarThumb], Rectangle.Empty);
		}
		protected override int PointsRectOffsetX { get { return GetSkinThumbElementOffset().X; } }
		protected override int ThumbUpperPartHeight { get { return GetSkinThumbElementOffset().Y; } }
		public override Rectangle GetThumbBounds() {
			return GetSkinThumbBounds();
		}
	}
	public class TrackBarViewInfo : BaseEditViewInfo {
		public static int LabelDeltaX = 1;
		public static int LabelDeltaY = 1;
		public static int StandAloneThumbCriticalHeight = 32;
		public static int StandAloneTrackLineCriticalHeight = 32;
		public static int GridThumbCriticalHeight = 24;
		public static int GridTrackLineCriticalHeight = 19;
		public static int MinimalTrackBarHeightInGrid = 16;
		public static int TrackLineOffsetY = 13;
		public static int LabelDeltaDistanceToTick = 14;
		TrackBarHelper trackBarHelper;
		TrackBarObjectInfoArgs trackInfo;
		Point thumbPos;
		Rectangle trackLineRect;
		Rectangle pointsRect;
		float pointsDelta;
		int tickCount, value = 0;
		protected internal virtual bool IsRightToLeft {
			get {
				if(Orientation != Orientation.Horizontal) return Item.InvertLayout;
				if(IsRightToLeftCore()) return !Item.InvertLayout;
				return Item.InvertLayout;
			}
		}
		protected bool IsRightToLeftCore() {
			if(OwnerEdit == null) return RightToLeft;
			return OwnerEdit.IsRightToLeft;
		}
		public virtual int ThumbCriticalHeight {
			get {
				if(InplaceType == InplaceType.Standalone)
					return StandAloneThumbCriticalHeight;
				return GridThumbCriticalHeight;
			}
		}
		public virtual int TrackLineCriticalHeight {
			get {
				if(InplaceType == InplaceType.Standalone)
					return StandAloneTrackLineCriticalHeight;
				return GridTrackLineCriticalHeight;
			}
		}
		public TrackBarHelper TrackBarHelper { 
			get {
				if(trackBarHelper == null) trackBarHelper = new TrackBarHelper(this);
				return trackBarHelper; 
			} 
		}
		public override Size CalcBestFit(Graphics g) {
			Size size = new Size(0, TrackCalculator.CalcBestContentHeight());
			size = BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(null, new Rectangle(Point.Empty, size), null)).Size;
			return size;
		}
		public TrackBarViewInfo(RepositoryItem item) : base(item) {
			this.trackLineRect = Rectangle.Empty;
			this.pointsRect = Rectangle.Empty;
			this.thumbPos = Point.Empty;
		}
		public override BorderStyles DefaultBorderStyle {
			get {
				if(base.DefaultBorderStyle == BorderStyles.Default) {  
					if(InplaceType == InplaceType.Standalone && InplaceType == InplaceType.Standalone)
						return BorderStyles.NoBorder;
				}
				return base.DefaultBorderStyle;
			}
			set {
				base.DefaultBorderStyle = value;
			}
		}
		public override void Dispose() {
			trackBarHelper = null;
			base.Dispose();
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault res = base.DefaultAppearance;
				res.BackColor = CreateDefaultAppearance().BackColor;
				return res;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual int PreferredDimension {
			get {
				if(!Item.ShowLabels) return TrackBarControlUnsafeNativeMethods.PreferredDimension;
				return PreferredDimensionWithLabels();
			}
		}
		public virtual int CalcLabelSize(TrackBarLabelInfo info) {
			bool shouldReleaseGraphics = false;
			if(GInfo.Graphics == null) {
				shouldReleaseGraphics = true;
				GInfo.AddGraphics(null);
			}
			try {
				info.Bounds = new Rectangle(new Point(), Item.LabelAppearance.CalcTextSize(GInfo.Graphics, info.Label.Label, 0).ToSize());
				if(Orientation == Orientation.Horizontal) {
					if(TickStyle == TickStyle.Both)
						return (int)(info.Bounds.Height) * 2;
					else
						return (int)(info.Bounds.Height);
				}
				else {
					if(TickStyle == TickStyle.Both)
						return (int)(info.Bounds.Width) * 2;
					else
						return (int)(info.Bounds.Width);
				}
			}
			finally {
				if(shouldReleaseGraphics)
					GInfo.ReleaseGraphics();
			}
		}
		public virtual int CalcLabelsMaxLength() {
			int labelsMaxLength = 0;
			int labelLength;
			bool shouldReleaseGraphics = false;
			if(GInfo.Graphics == null) {
				shouldReleaseGraphics = true;
				GInfo.AddGraphics(null);
			}
			try {
				Labels = CreateLabelsInfo();
				foreach(TrackBarLabelInfo info in Labels) {
					if(info.Label.Value >= Item.Minimum && info.Label.Value <= Item.Maximum && info.Label.Visible) {
						labelLength = CalcLabelSize(info);
						labelsMaxLength = Math.Max(labelsMaxLength, labelLength);
					}
				}
				new LabelsLayoutCalculator(this).Calculate();
			}
			finally {
				if(shouldReleaseGraphics)
					GInfo.ReleaseGraphics();
			}
			return labelsMaxLength;
		}
		protected virtual int PreferredDimensionWithLabels() {			
			int labelsMaxLength = CalcLabelsMaxLength();
			if(labelsMaxLength > 0) {
				if(TickStyle == TickStyle.Both)
					labelsMaxLength += 2 * (Item.DistanceFromTickToLabel + LabelDeltaDistanceToTick);
				else
					labelsMaxLength += Item.DistanceFromTickToLabel + LabelDeltaDistanceToTick;
			}
			int scrollArrowHeight = EditorsNativeMethods.GetSystemMetrics(TrackBarControlUnsafeNativeMethods.SM_CYHSCROLL);
			int barSize = scrollArrowHeight * 8 / 3;
			int lengthMinimum = labelsMaxLength + barSize;
			return Math.Max(lengthMinimum, barSize);
		}
		protected internal override bool AllowDrawParentBackground { get { return true; } }
		public int Value { get { return value; } }
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			this.thumbPos.Offset(x, y);
			this.trackLineRect.Offset(x, y);
			this.pointsRect.Offset(x, y);
			OffsetLabels(x, y);
		}
		protected virtual void OffsetLabels(int x, int y) {
			if(Labels == null) return;
			foreach(TrackBarLabelInfo info in Labels) {
				if(!info.Bounds.IsEmpty)
					info.Bounds = new Rectangle(new Point(info.Bounds.X + x, info.Bounds.Y + y), info.Bounds.Size);
			}
		}
		public override bool IsRequiredUpdateOnMouseMove { get { return true; } }
		protected TrackBarControl TrackBar { get { return OwnerEdit as TrackBarControl; } }
		protected bool ShouldUpdateState { get { return TrackBar != null && TrackBar.ShouldUpdateViewInfoState; } }
		public override EditHitInfo CalcHitInfo(Point p) {
			EditHitInfo hi = base.CalcHitInfo(p);
			if((Bounds.Contains(p) && ThumbBounds.Contains(p)) || !ShouldUpdateState) {
				hi.SetHitTest(EditHitTest.Button);
				hi.SetHitObject(EditHitTest.Button);
			}
			return hi;
		}
		protected override bool IsHotEdit(EditHitInfo hitInfo) {
			return hitInfo.HitTest == EditHitTest.Button;
		}
		TrackBarObjectPainter trackPainter = null;
		TrackBarInfoCalculator trackCalculator = null;
		public TrackBarObjectPainter TrackPainter { get { return trackPainter; } }
		public TrackBarInfoCalculator TrackCalculator { 
			get {
				if(trackCalculator == null && TrackPainter != null) trackCalculator = TrackPainter.GetCalculator(this);
				return trackCalculator; 
			} 
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.trackPainter = GetTrackPainter();
			this.trackCalculator = TrackPainter.GetCalculator(this);
			TrackCalculator.CalcHeights();
		}
		protected override bool UpdateObjectState() { 
			ObjectState prevState = State;
			bool res = base.UpdateObjectState();
			if(Enabled) {
				if(PressedInfo.HitTest == EditHitTest.Button) {
					State |= ObjectState.Pressed;
				}
			}
			return res || prevState != State;
		}
		public override bool IsSupportFastViewInfo { get { return false; } }
		protected override int CalcMinHeightCore(Graphics g) { return TrackBarViewInfo.MinimalTrackBarHeightInGrid; }
		public new RepositoryItemTrackBar Item { get { return base.Item as RepositoryItemTrackBar; } }
		public Orientation Orientation { get { return Item.Orientation; } }
		public TickStyle TickStyle { get { return Item.TickStyle; } }
		public virtual TrackBarObjectInfoArgs TrackInfo { get { return trackInfo; } }
		[EditorPainterActivator(typeof(SkinTrackBarObjectPainter), typeof(TrackBarObjectPainter))]
		public virtual TrackBarObjectPainter GetTrackPainter() {
			if(IsPrinting)
				return new TrackBarObjectPainter();
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP)
				return new WindowsXPTrackBarObjectPainter();
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinTrackBarObjectPainter(LookAndFeel.ActiveLookAndFeel);
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)
				return new Office2003TrackBarObjectPainter();
			return new TrackBarObjectPainter();
		}
		public override void Reset() {
			base.Reset();
			this.trackInfo = new TrackBarObjectInfoArgs(PaintAppearance, this);
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new AppearanceDefault(Color.Empty, Color.Transparent);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)
				return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window));
			return new AppearanceDefault(GetSystemColor(SystemColors.ControlDark), GetSystemColor(SystemColors.Control));
		}
		public override bool DrawFocusRect {
			get {
				return OwnerEdit != null && Item.AllowFocused && OwnerEdit.EditorContainsFocus;
			}
		}
		public override bool DefaultAllowDrawFocusRect { get { return true; } }
		public Point ThumbPos { get { return thumbPos; } protected set { thumbPos = value; } }
		public virtual Point MirrorPoint { get { return new Point(0, TrackLineRect.Y + TrackLineRect.Height / 2); } }
		public Rectangle TrackLineContentRect { get { return trackLineRect; } }
		public Rectangle TrackLineRect {
			get {
				if(IsTouchMode) 
					return TrackCalculator.GetTrackLineClientBounds(TrackLineContentRect);
				return TrackLineContentRect;
			}
		}
		protected internal virtual bool IsTouchMode {
			get {
				if(LookAndFeel == null) return false;
				return LookAndFeel.ActiveLookAndFeel.GetTouchUI();
			}
		}
		public Rectangle PointsRect { get { return pointsRect; } }
		public float PointsDelta { get { return pointsDelta; } }
		public int TickCount { get { return tickCount; } }
		protected override void CalcRects() {
			base.CalcRects();
			CalcTrackInfo();
		}
		protected virtual void CalcTrackInfoCore() {
			CalcTrackLineRect();
			CalcPointsRect();
			CalcPointsDelta();
			CalcTickCount();
			CalcThumbPos();
			CalcLabels();
		}
		protected internal List<TrackBarLabelInfo> Labels {
			get;
			set;
		}
		protected virtual void CalcLabels() {
			if(Labels == null) {
				Labels = CreateLabelsInfo();
				CalcLabelsSize();
			}
			new LabelsLayoutCalculator(this).Calculate();
		}
		protected virtual void CalcLabelsSize() {
			foreach(TrackBarLabelInfo info in Labels) {
				CalcLabelSize(info);
			}
		}
		protected virtual List<TrackBarLabelInfo> CreateLabelsInfo() {
			List<TrackBarLabelInfo> res = new List<TrackBarLabelInfo>();
			foreach(TrackBarLabel label in Item.Labels) {
				res.Add(new TrackBarLabelInfo() { Label = label });
				res.Sort(new TrackBarLabelsComparer());
			}
			return res;
		}
		protected virtual void CalcTrackInfo() {
			TrackCalculator.CalcHeights();
			CalcTrackInfoCore();			
		}
		protected override void OnEditValueChanged() {
			this.value = Item.ConvertValue(EditValue);
			base.OnEditValueChanged();
		}
		public int ScaleValue(int critValue, int boundValue, int maxValue, int originHeight) {
			return critValue > boundValue ? maxValue : (int)((float)maxValue * critValue / originHeight);
		}
		public int ScaleValue(int critValue, int boundValue, int maxValue) {
			return ScaleValue(critValue, boundValue, maxValue, boundValue);
		}
		protected virtual void CalcTrackButtons() { }
		protected virtual void CalcTrackLineRect() {
			trackLineRect = TrackCalculator.CalcTrackLineRect();
		}
		void CalcPointsRect() {
			pointsRect = TrackCalculator.CalcPointsRect();
		}
		void CalcTickCount() {
			tickCount = (Item.Maximum - Item.Minimum) / Item.TickFrequency + 1;
			tickCount = Math.Min(pointsRect.Width / 2 + 1, tickCount);
		}
		void CalcPointsDelta() {
			if (Item.Maximum == Item.Minimum) 
				pointsDelta = 0;
			else {
			pointsDelta = (float)Item.TickFrequency * (float)PointsRect.Width / (float)(Item.Maximum - Item.Minimum);
			pointsDelta = Math.Max(2, pointsDelta);
		}
		}
		int ThumbLeftX { get { return trackLineRect.X + TrackPainter.ThumbOffsetX(this) + TrackPainter.GetThumbBestWidth(this) / 2; } }
		int ThumbChangeWidth { get { return ClientRect.Width - ThumbLeftX * 2; } }
		protected virtual void CalcThumbPos() {
			thumbPos = CalcThumbPosCore(Value);
		}
		protected virtual Point CalcThumbPosCore(int val) {
			Point tp = Point.Empty;
			TrackBarObjectPainter pt = TrackPainter as TrackBarObjectPainter;
			tp.Y = trackLineRect.Y + trackLineRect.Height / 2;
			if (IsRightToLeft) {
				if (Item.Maximum == Item.Minimum)
					tp.X = PointsRect.Right;
				else 
				tp.X = PointsRect.Left + (int)((float)PointsRect.Width * ((float)(Item.Maximum - val) / (float)(Item.Maximum - Item.Minimum)));
			}
			else {
				if (Item.Maximum == Item.Minimum)
					tp.X = PointsRect.Left;
			else
				tp.X = PointsRect.Left + (int)((float)PointsRect.Width * ((float)(val - Item.Minimum) / (float)(Item.Maximum - Item.Minimum)));
			}
			return tp;
		}
		public Point[] ThumbRegion {
			get {
				if(TickStyle == TickStyle.Both) return RectThumbRegion;
				return ArrowThumbRegion;
			}
		}
		protected Rectangle GetThumbBounds(Point[] p) {
			Rectangle r = Rectangle.FromLTRB(p[1].X, p[1].Y, p[3].X, p[3].Y);
			if(r.Height < 0) { r.Y += r.Height; r.Height = Math.Abs(r.Height); }
			if(r.Width < 0) { r.X += r.Width; r.Width = Math.Abs(r.Width); }
			if(Orientation == Orientation.Horizontal)
				r.Width++;
			else
				r.Height++;
			return r;
		}
		public Rectangle ThumbContentBounds {
			get {
				return TrackCalculator.GetThumbBounds();
			}
		}
		public Rectangle ThumbBounds { 
			get {
				if(IsTouchMode)
					return TrackCalculator.GetThumbClientBounds(ThumbContentBounds);
				return ThumbContentBounds;
			} 
		}
		public virtual Point[] ArrowThumbRegion {
			get {
				TrackBarObjectPainter pt = TrackPainter;
				int[,] offsetP1 = { { 0, 11 }, { -pt.GetThumbBestWidth(this) / 2, 6 }, { -pt.GetThumbBestWidth(this) / 2, -9 }, { pt.GetThumbBestWidth(this) / 2, -9 }, { pt.GetThumbBestWidth(this) / 2, 6 }, { 0, 11 } };
				Point[] polygon = new Point[6];
				TransformPoints(offsetP1, polygon, ThumbPos);
				return polygon;
			}
		}
		public virtual Point[] RectThumbRegion {
			get {
				TrackBarObjectPainter pt = TrackPainter;
				int[,] offsetP1 = { { -pt.GetThumbBestWidth(this) / 2, 11 }, { -pt.GetThumbBestWidth(this) / 2, -11 }, { pt.GetThumbBestWidth(this) / 2, -11 }, { pt.GetThumbBestWidth(this) / 2, 11 }, { -pt.GetThumbBestWidth(this) / 2, 11 } };
				Point[] polygon = new Point[5];
				TransformPoints(offsetP1, polygon, ThumbPos);
				return polygon;
			}
		}
		protected void TransformPoints(int[,] offsetP1, Point[] polygon) {
			TrackCalculator.TransformPoints(offsetP1, polygon);
		}
		protected void TransformPoints(int[,] offsetP1, Point[] polygon, Point center) {
			TrackCalculator.TransformPoints(offsetP1, polygon, center);
		}
		public Point ControlToClient(Point p) {
			if(Orientation == Orientation.Horizontal) return p;
			return new Point(ClientRect.Bottom - p.Y, p.X);
		}
		public virtual int ValueFromPoint(Point p) {
			int currentValue = GetValueFormPointCore(p);
			if(Item.SmallChangeUseMode == SmallChangeUseMode.ArrowKeys) return currentValue;
			int dif = currentValue - Value;
			int change = (Math.Abs(dif) / Item.SmallChange) * Item.SmallChange;
			if(dif < 0) return Value - change;
			return Value + change;
		}
		protected int GetValueFormPointCore(Point p) {
			if(IsRightToLeft)
				return Item.Minimum + (int)((float)(Item.Maximum - Item.Minimum) * (float)(PointsRect.Right - p.X) / (float)PointsRect.Width + 0.5f);
			return Item.Minimum + (int)((float)(Item.Maximum - Item.Minimum) * (float)(p.X - PointsRect.Left) / (float)PointsRect.Width + 0.5f);
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class TrackBarObjectInfoArgs : StyleObjectInfoArgs {
		TrackBarViewInfo viewInfo;
		public TrackBarObjectInfoArgs(AppearanceObject style) {
			SetAppearance(style);
			viewInfo = null;
		}
		public TrackBarObjectInfoArgs(AppearanceObject style, TrackBarViewInfo vi) {
			SetAppearance(style);
			viewInfo = vi;
		}
		public TrackBarViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class Office2003TrackBarThumbPainter : Office2003ButtonObjectPainter {
		bool IsHot(TrackBarObjectInfoArgs e) { return (e.State & ObjectState.Hot) != 0; }
		bool IsPressed(TrackBarObjectInfoArgs e) { return (e.State & ObjectState.Pressed) != 0; }
		public Brush GetThumbFillBrush(TrackBarObjectInfoArgs e) {
			return IsHot(e)? GetHotBackBrush(e, IsPressed(e)) : GetNormalBackBrush(e);
		}
		public Color GetThumbBorderColor(TrackBarObjectInfoArgs e) {
			return IsHot(e)? GetHotBorderColor(e, IsPressed(e)) : GetBorderColor(e);
		}
	}
	public class Office2003TrackBarObjectPainter : TrackBarObjectPainter {
		protected Brush GetFillBrush(TrackBarObjectInfoArgs e) {
			return new Office2003TrackBarThumbPainter().GetThumbFillBrush(e);
		}
		public override void FillThumb(TrackBarObjectInfoArgs e, bool bMirror, Point[] polygon, bool hot) {
			e.Graphics.FillPolygon(GetFillBrush(e), polygon);
		}
		protected virtual Color GetForeColor(TrackBarObjectInfoArgs e) {
			return new Office2003TrackBarThumbPainter().GetThumbBorderColor(e);
		}
		public override void DrawArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { 0, 11 }, { -xOffset, 6 }, { -xOffset, -9 }, { xOffset, -9 }, { xOffset, 6 } };
			int[,] offsetP2 = { { -xOffset, 6 }, { -xOffset, -9 }, { xOffset, -9 }, { xOffset, 6 }, { 0, 11 } };
			Color c = GetForeColor(e);
			Color[] colors = { c, c, c, c, c };
			DrawThumb(e, bMirror, offsetP1, offsetP2, colors, 5);
		}
		public override void DrawRectThumb(TrackBarObjectInfoArgs e) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { -xOffset, 11 }, { -xOffset, -11 }, { xOffset, -11 }, { xOffset, 11 } };
			int[,] offsetP2 = { { -xOffset, -11 }, { xOffset, -11 }, { xOffset, 11 }, { -xOffset, 11 } };
			Color c = GetForeColor(e);
			Color[] colors = { c, c, c, c };
			DrawThumb(e, false, offsetP1, offsetP2, colors, 4);
		}
	}
	public class LabelsLayoutCalculator {
		public int labelsTop;
		public int labelsMaxHeight;
		protected TrackBarViewInfo ViewInfo { get; set; }
		public LabelsLayoutCalculator(TrackBarViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		public void Calculate() {
			this.labelsTop = CalcLabelsTop();
			if(!ViewInfo.Item.AutoSize && ViewInfo.Item.TickStyle == TickStyle.Both)
				labelsMaxHeight = CalcLabelsMaxHeight();
			foreach(TrackBarLabelInfo info in ViewInfo.Labels) {
				Calculate(info);
			}
		}
		int CalcLabelsTop() {
			RepositoryItemTrackBar ri = ViewInfo.Item;
			if(ViewInfo.TickStyle == TickStyle.BottomRight)
				return ViewInfo.PointsRect.Bottom;
			else if(ViewInfo.TickStyle == TickStyle.TopLeft)
				return 2 * ViewInfo.MirrorPoint.Y - ViewInfo.PointsRect.Y;
			else if(ViewInfo.TickStyle == TickStyle.Both) 
				return ri.Orientation == Orientation.Horizontal ? ViewInfo.PointsRect.Bottom / 2 - ViewInfo.TrackBarHelper.ContentHeight : ViewInfo.PointsRect.Bottom / 2 - ViewInfo.TrackBarHelper.ContentWidth;
			else
				return ViewInfo.PointsRect.Y;
		}
		public void Calculate(TrackBarLabelInfo info) {
			if(!info.Label.Visible) {
				info.Bounds = Rectangle.Empty;
				return;
			}
			RepositoryItemTrackBar ri = ViewInfo.Item;
			float koeff = (float)ViewInfo.PointsRect.Width / (ri.Maximum - ri.Minimum);
			int x = 0;
			if(ri.Orientation == Orientation.Horizontal) {
				if(ri.Minimum != ri.Maximum) {
					int distanceToCurrentTick = ViewInfo.IsRightToLeft ? ri.Maximum - info.Label.Value : info.Label.Value - ri.Minimum;
					x = (int)(ViewInfo.PointsRect.X + distanceToCurrentTick * koeff + TrackBarViewInfo.LabelDeltaX - (info.Bounds.Width + 1) / 2);
				}
				if(x < ViewInfo.ContentRect.X) x = ViewInfo.ContentRect.X;
				if(x + info.Bounds.Width > ViewInfo.ContentRect.Right) x = ViewInfo.ContentRect.Right - info.Bounds.Width;
				info.Bounds = CalcRectangle(x, info, false);
				if(ViewInfo.Item.TickStyle == TickStyle.Both) info.MirrorBounds = CalcRectangle(x, info, true);
			}
			else {
				if(ri.Minimum != ri.Maximum) {
					int distanceToCurrentTick = ViewInfo.IsRightToLeft ? info.Label.Value - ri.Minimum : ri.Maximum - info.Label.Value;
					x = (int)(ViewInfo.PointsRect.X + distanceToCurrentTick * koeff + TrackBarViewInfo.LabelDeltaX - (info.Bounds.Height + 1) / 2) - 2 * ViewInfo.TrackBarHelper.ClientX;
				}
				if(x < ViewInfo.ContentRect.X) x = ViewInfo.ContentRect.X;
				if(x + info.Bounds.Height > ViewInfo.ContentRect.Bottom) x = ViewInfo.ContentRect.Bottom - info.Bounds.Height;
				info.Bounds = CalcRectangle(x, info, false);
				if(ViewInfo.Item.TickStyle == TickStyle.Both) info.MirrorBounds = CalcRectangle(x, info, true);
			}
		}
		public virtual Rectangle CalcRectangle(int x, TrackBarLabelInfo info, bool mirror) {
			int distanceFromTrackLineToLabel = ViewInfo.PointsRect.Y - (1 + ViewInfo.TrackLineRect.Height) / 2 - ViewInfo.TrackLineRect.Y + ViewInfo.Item.DistanceFromTickToLabel;
			if(ViewInfo.TickStyle == TickStyle.TopLeft){ 
				if(ViewInfo.Item.Orientation == Orientation.Horizontal)
					return new Rectangle(new Point(x, labelsTop - info.Bounds.Height - distanceFromTrackLineToLabel), info.Bounds.Size);
				return new Rectangle(new Point(labelsTop - info.Bounds.Width - distanceFromTrackLineToLabel - TrackBarViewInfo.LabelDeltaY, x), info.Bounds.Size);
			}
			else if(ViewInfo.TickStyle == TickStyle.BottomRight) {
				if(ViewInfo.Item.Orientation == Orientation.Horizontal)
					return new Rectangle(new Point(x, labelsTop + distanceFromTrackLineToLabel - 3 * TrackBarViewInfo.LabelDeltaY), info.Bounds.Size);
				return new Rectangle(new Point(labelsTop + distanceFromTrackLineToLabel, x), info.Bounds.Size);
			}
			else if(ViewInfo.TickStyle == TickStyle.Both) {
				if(mirror) {
					if(ViewInfo.Item.Orientation == Orientation.Horizontal)
						return new Rectangle(new Point(x, CalcTopMirror(info.Bounds.Height) - distanceFromTrackLineToLabel + 4 * TrackBarViewInfo.LabelDeltaY), info.Bounds.Size);
					return new Rectangle(new Point(CalcTopMirror(info.Bounds.Width) - distanceFromTrackLineToLabel + 3 * TrackBarViewInfo.LabelDeltaY, x + TrackBarViewInfo.LabelDeltaX), info.Bounds.Size);
				}
				if(ViewInfo.Item.Orientation == Orientation.Horizontal)
					return new Rectangle(new Point(x, ViewInfo.PointsRect.Bottom + distanceFromTrackLineToLabel - 3 * TrackBarViewInfo.LabelDeltaY), info.Bounds.Size);
				return new Rectangle(new Point(ViewInfo.PointsRect.Bottom + distanceFromTrackLineToLabel, x + TrackBarViewInfo.LabelDeltaX), info.Bounds.Size);
			}
			else {
				if(ViewInfo.Item.Orientation == Orientation.Horizontal)
					return new Rectangle(new Point(x, labelsTop + ViewInfo.Item.DistanceFromTickToLabel - ViewInfo.PointsRect.Height), info.Bounds.Size);
				return new Rectangle(new Point(labelsTop + ViewInfo.Item.DistanceFromTickToLabel - ViewInfo.PointsRect.Height + 3 * TrackBarViewInfo.LabelDeltaY, x), info.Bounds.Size);
			}
		}
		public virtual int CalcTopMirror(int x) {
			int preferredDimension = TrackBarControlUnsafeNativeMethods.PreferredDimension;
			int distanceFromTrackLineToLabel = ViewInfo.PointsRect.Y - (1 + ViewInfo.TrackLineRect.Height) / 2 - ViewInfo.TrackLineRect.Y + ViewInfo.Item.DistanceFromTickToLabel;
			int labelsWithIndentHeight = 2 * (labelsMaxHeight + distanceFromTrackLineToLabel + TrackBarViewInfo.LabelDeltaY);
			int preferredDimensionWithLabels = preferredDimension + labelsWithIndentHeight;
			int labelTop = ViewInfo.TrackBarHelper.ContentHeight - ViewInfo.PointsRect.Bottom - x;
			if(ViewInfo.Item.AutoSize
				|| (ViewInfo.TrackBarHelper.ContentHeight < preferredDimensionWithLabels)) 
				return ViewInfo.TrackBarHelper.ContentY + ViewInfo.TrackBarHelper.ClientY + labelTop;
			if(ViewInfo.Item.Alignment == VertAlignment.Center) 
				return labelTop + ViewInfo.TrackBarHelper.ContentY;
			if(ViewInfo.Item.Alignment == VertAlignment.Bottom) 
				return ViewInfo.TrackBarHelper.ContentHeight + ViewInfo.TrackBarHelper.ContentY - preferredDimension - labelsWithIndentHeight + labelTop;
			return preferredDimensionWithLabels - ViewInfo.PointsRect.Bottom - x;
		}
		public virtual int CalcLabelsMaxHeight() {
			int labelsMaxHeight = 0;
			int labelHeight;
			foreach(TrackBarLabelInfo info in ViewInfo.Labels) {
				if(info.Label.Value >= ViewInfo.Item.Minimum && info.Label.Value <= ViewInfo.Item.Maximum && info.Label.Visible) {
					labelHeight = ViewInfo.Item.Orientation == Orientation.Horizontal ? info.Bounds.Height : info.Bounds.Width;
					labelsMaxHeight = Math.Max(labelsMaxHeight, labelHeight);
				}
			}
			return labelsMaxHeight;
		}
	}
	public class TrackBarObjectPainter : ObjectPainter {
		public static int ThumbWidth = 11;
		public static int ThumbHeight = 21;
		public TrackBarObjectPainter() { }
		protected internal virtual TrackBarInfoCalculator GetCalculator(TrackBarViewInfo viewInfo) { return new TrackBarInfoCalculator(viewInfo, this); }
		public virtual Rectangle GetBackgroundRect(TrackBarObjectInfoArgs e) {
			return e.ViewInfo.ClientRect;
		}
		public virtual void DrawBackground(TrackBarObjectInfoArgs e) {
			if(!BaseEditPainter.DrawParentBackground(e.ViewInfo, e.Cache))
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.BackColor), GetBackgroundRect(e));
		}
		public virtual Rectangle GetFilledRect(TrackBarObjectInfoArgs e) {
			Rectangle rect = e.ViewInfo.TrackLineContentRect;
			if(e.ViewInfo.IsRightToLeft) {
				rect.X = e.ViewInfo.ThumbPos.X;
				rect.Width = rect.Width + rect.X - e.ViewInfo.ThumbPos.X;
			}
			else rect.Width = e.ViewInfo.ThumbPos.X - rect.X;
			return e.ViewInfo.TrackBarHelper.Rotate(rect);
		}
		public virtual Rectangle GetEmptyRect(TrackBarObjectInfoArgs e) {
			Rectangle rect = e.ViewInfo.TrackLineContentRect;
			rect.Width = rect.Right - e.ViewInfo.ThumbPos.X;
			rect.X = e.ViewInfo.ThumbPos.X;
			return e.ViewInfo.TrackBarHelper.Rotate(rect);
		}
		public virtual bool AllowTick(TrackBarViewInfo vi) {
			return true;
		}
		public virtual int GetThumbBestWidth(TrackBarViewInfo vi) { return TrackBarObjectPainter.ThumbWidth; }
		public virtual int GetThumbBestHeight(TrackBarViewInfo vi) { return TrackBarObjectPainter.ThumbHeight; }
		public virtual int ThumbOffsetX(TrackBarViewInfo vi) { return 0; }
		public virtual BorderPainter GetTrackLineBorderPainter(TrackBarObjectInfoArgs e) {
			if(e.ViewInfo.BorderStyle == BorderStyles.NoBorder || e.ViewInfo.BorderStyle == BorderStyles.UltraFlat)
				return BorderHelper.GetPainter(BorderStyles.Default, e.ViewInfo.Item.LookAndFeel);
			return e.ViewInfo.BorderPainter;
		}
		public virtual void DrawTrackLine(TrackBarObjectInfoArgs e) {
			DrawTrackLineCore(e, e.ViewInfo.TrackBarHelper.Rotate(e.ViewInfo.TrackLineContentRect));
		}
		protected virtual void DrawTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			GetTrackLineBorderPainter(e).DrawObject(new BorderObjectInfoArgs(e.Cache, e.ViewInfo.PaintAppearance, bounds, ObjectState.Hot));
		}
		protected virtual RotateFlipType GetRotateAngle(TrackBarViewInfo vi) { 
			if(vi.Orientation == Orientation.Horizontal) {
				if(vi.TickStyle == TickStyle.TopLeft) return RotateFlipType.RotateNoneFlipY;
				return RotateFlipType.RotateNoneFlipNone;
			}
			if(vi.TickStyle == TickStyle.TopLeft) return RotateFlipType.Rotate270FlipX;
			if(vi.TickStyle == TickStyle.BottomRight) return RotateFlipType.Rotate270FlipNone;
			return RotateFlipType.Rotate270FlipNone;
		}
		protected virtual RotateFlipType GetTrackLineRotateAngle(TrackBarViewInfo vi) {
			if(vi.Orientation == Orientation.Horizontal) return RotateFlipType.RotateNoneFlipNone;
			return RotateFlipType.Rotate270FlipNone;
		}
		protected virtual SkinElement GetTrack(TrackBarViewInfo vi) { return null; }
		protected virtual void DrawSkinTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			SkinElementInfo info = new SkinElementInfo(GetTrack(e.ViewInfo), bounds);
			info.ImageIndex = e.State == ObjectState.Disabled ? 2 : 0;
			new RotateObjectPaintHelper().DrawRotated(e.Cache, info, SkinElementPainter.Default, GetTrackLineRotateAngle(e.ViewInfo));
			Rectangle filled = GetFilledRect(e);
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(filled);
			if(e.ViewInfo.Item.HighlightSelectedRange)
				info.ImageIndex += 1;
			new RotateObjectPaintHelper().DrawRotated(e.Cache, info, SkinElementPainter.Default, GetTrackLineRotateAngle(e.ViewInfo));
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
		}
		public virtual void DrawPoints(TrackBarObjectInfoArgs e) {
			switch(e.ViewInfo.TickStyle) {
				case TickStyle.BottomRight:
					DrawPoints(e, false);
					break;
				case TickStyle.Both:
					DrawPoints(e, false);
					DrawPoints(e, true);
					break;
				case TickStyle.TopLeft:
					DrawPoints(e, true);
					break;
			}
		}
		public virtual void DrawPoints(TrackBarObjectInfoArgs e, bool bMirror) {
			Point p1 = Point.Empty, p2 = Point.Empty;
			float xPos;
			int tickCount;
			p1.Y = e.ViewInfo.PointsRect.Y;
			for(xPos = 0, tickCount = 0; tickCount < e.ViewInfo.TickCount; xPos += e.ViewInfo.PointsDelta, tickCount ++) {
				p2.X = p1.X = e.ViewInfo.IsRightToLeft ? (int)(e.ViewInfo.PointsRect.Right - xPos + 0.01f) : (int)(e.ViewInfo.PointsRect.X + xPos + 0.01f);
				if(tickCount != 0 && tickCount != e.ViewInfo.TickCount-1) p2.Y = p1.Y + Math.Max(e.ViewInfo.PointsRect.Height * 3 / 4, 1);
				else p2.Y = p1.Y + e.ViewInfo.PointsRect.Height;
				DrawLine(e, e.ViewInfo.TrackBarHelper.RotateAndMirror(p1, e.ViewInfo.MirrorPoint.Y, bMirror), e.ViewInfo.TrackBarHelper.RotateAndMirror(p2, e.ViewInfo.MirrorPoint.Y, bMirror));
			}
		}
		protected virtual void DrawLine(TrackBarObjectInfoArgs e, Point p1, Point p2) {
			e.Paint.DrawLine(e.Graphics, e.Cache.GetPen(e.ViewInfo.PaintAppearance.ForeColor), p1, p2);
		}
		protected virtual void DrawLabel(TrackBarObjectInfoArgs e, TrackBarLabelInfo info) {
			RepositoryItemTrackBar ri = e.ViewInfo.Item;
			ri.LabelAppearance.DrawString(e.Cache, info.Label.Label, info.Bounds, GetLabelsBrush(e));
			if(e.ViewInfo.TickStyle == TickStyle.Both) {
				ri.LabelAppearance.DrawString(e.Cache, info.Label.Label, info.MirrorBounds, GetLabelsBrush(e));
			}
		}
		protected virtual Brush GetLabelsBrush(TrackBarObjectInfoArgs e) {
			RepositoryItemTrackBar ri = e.ViewInfo.Item;
			if(!ri.LabelAppearance.ForeColor.IsEmpty)
				return new SolidBrush(ri.LabelAppearance.ForeColor);
			return new SolidBrush(e.ViewInfo.PaintAppearance.ForeColor);
		}
		protected virtual bool ShouldDrawLabel(TrackBarObjectInfoArgs e, TrackBarLabelInfo current, TrackBarLabelInfo prev) {
			if(!(e.ViewInfo.Item.Minimum <= current.Label.Value && e.ViewInfo.Item.Maximum >= current.Label.Value)) 
				return false;
			if(prev == null)
				return true;
			if(current.Bounds.IsEmpty)
				return false;
			if(!e.ViewInfo.Item.ShowLabelsForHiddenTicks && (current.Label.Value - e.ViewInfo.Item.Minimum) % e.ViewInfo.Item.TickFrequency != 0) return false;
			Rectangle currRect = current.Bounds; currRect.Inflate(3, 3);
			Rectangle prevRect = prev.Bounds; prevRect.Inflate(3, 3);
			return !currRect.IntersectsWith(prevRect);
		}
		protected virtual void DrawLabels(TrackBarObjectInfoArgs e) {			
			TrackBarLabelInfo prevInfo = null;			
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveClip();
			try {
				e.Cache.ClipInfo.SetClip(e.ViewInfo.ClientRect);
				foreach(TrackBarLabelInfo info in e.ViewInfo.Labels) {
					if(!ShouldDrawLabel(e, info, prevInfo))
						continue;
					DrawLabel(e, info);
					prevInfo = info;
				}
			}
			finally {
				e.Cache.ClipInfo.RestoreClip(clipState);
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			TrackBarObjectInfoArgs tbe = e as TrackBarObjectInfoArgs;
			RepositoryItemTrackBar ri = tbe.ViewInfo.Item;
			DrawBackground(tbe);
			DrawTrackLine(tbe);
			if(this.AllowTick(tbe.ViewInfo))
				DrawPoints(tbe);
			DrawThumb(tbe);
			if(ri.ShowLabels) DrawLabels(tbe);
		}
		public virtual void DrawThumb(TrackBarObjectInfoArgs e) {
			switch(e.ViewInfo.Item.TickStyle) {
				case TickStyle.None:
				case TickStyle.BottomRight:
					DrawArrowThumb(e, false);
					break;
				case TickStyle.Both:
					DrawRectThumb(e);
					break;
				case TickStyle.TopLeft:
					DrawArrowThumb(e, true);
					break;
			}
		}
		public virtual void FillThumb(TrackBarObjectInfoArgs e, bool bMirror, Point[] polygon, bool hot) {
			Color backColor = e.ViewInfo.PaintAppearance.BackColor;
			if(e.ViewInfo.PaintAppearance.BackColor == Color.Empty || e.ViewInfo.PaintAppearance.BackColor == Color.Transparent)
				backColor = e.ViewInfo.OwnerEdit.BackColor;
			e.Paint.FillPolygon(e.Graphics, e.Cache.GetSolidBrush(backColor), polygon);
			if(hot || (e.ViewInfo.Enabled == false))
				e.Paint.FillPolygon(e.Graphics, new HatchBrush(HatchStyle.Percent50, SystemColors.ControlLightLight, e.ViewInfo.PaintAppearance.BackColor), polygon);
			polygon = null;
		}
		public virtual void FillThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			FillThumb(e, bMirror, e.ViewInfo.ThumbRegion, e.ViewInfo.PressedInfo.HitTest == EditHitTest.Button);
		}
		public virtual void DrawArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { 0, 11 }, { -xOffset, 6 }, { -xOffset, -9 }, { xOffset, -9 }, { xOffset, 6 }, { xOffset - 1, -8 }, { xOffset - 1, 6 } };
			int[,] offsetP2 = { { -xOffset, 6 }, { -xOffset, -9 }, { xOffset, -9 }, { xOffset, 6 }, { 0, 11 }, { xOffset - 1, 6 }, { 0, 10 } };
			Color[] colors = { SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDark, SystemColors.ControlDark };
			DrawThumb(e, bMirror, offsetP1, offsetP2, colors, 7);
		}
		public virtual void DrawRectThumb(TrackBarObjectInfoArgs e) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { -xOffset, 11 }, { -xOffset, -11 }, { xOffset, -11 }, { xOffset, 11 }, { xOffset - 1, -10 }, { xOffset - 1, 10 } };
			int[,] offsetP2 = { { -xOffset, -11 }, { xOffset, -11 }, { xOffset, 11 }, { -xOffset, 11 }, { xOffset - 1, 10 }, { -(xOffset - 1), 10 } };
			Color[] colors = { SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDark, SystemColors.ControlDark };
			DrawThumb(e, false, offsetP1, offsetP2, colors, 6);
		}
		public virtual void DrawThumb(TrackBarObjectInfoArgs e, bool bMirror, int[,] offsetP1, int[,] offsetP2, Color[] colors, int lineCount) {
			Point p1 = Point.Empty, p2 = Point.Empty;
			int lineIndex;
			FillThumb(e, bMirror);
			for(lineIndex = 0; lineIndex < lineCount ; lineIndex++) {
				p1 = e.ViewInfo.ThumbPos; p2 = e.ViewInfo.ThumbPos;
				p1.X += e.ViewInfo.ScaleValue(e.ViewInfo.TrackBarHelper.ClientHeight, e.ViewInfo.ThumbCriticalHeight, offsetP1[lineIndex, 0]);
				p1.Y += e.ViewInfo.ScaleValue(e.ViewInfo.TrackBarHelper.ClientHeight, e.ViewInfo.ThumbCriticalHeight, offsetP1[lineIndex, 1]);
				p2.X += e.ViewInfo.ScaleValue(e.ViewInfo.TrackBarHelper.ClientHeight, e.ViewInfo.ThumbCriticalHeight, offsetP2[lineIndex, 0]);
				p2.Y += e.ViewInfo.ScaleValue(e.ViewInfo.TrackBarHelper.ClientHeight, e.ViewInfo.ThumbCriticalHeight, offsetP2[lineIndex, 1]);
				e.Paint.DrawLine(e.Graphics, e.Cache.GetPen(colors[lineIndex]), e.ViewInfo.TrackBarHelper.RotateAndMirror(p1, e.ViewInfo.MirrorPoint.Y, bMirror), e.ViewInfo.TrackBarHelper.RotateAndMirror(p2, e.ViewInfo.MirrorPoint.Y, bMirror));
			}
		}
		protected virtual Rectangle GetVerticalThumbRectangle(TrackBarViewInfo vi, Rectangle rect) {
			if(vi.Orientation == Orientation.Vertical) rect.Y++;
			return rect;
		}
		protected virtual ISkinProvider Provider { get { return null;} }
		protected SkinElement GetTrackThumb(TrackBarViewInfo vi) {
			if(Provider == null) return null;
			SkinElement element = null;
			if(vi.TickStyle == TickStyle.Both) {
				element = EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinTrackBarThumbBoth];
				if(element != null) return element;
			}
			return EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinTrackBarThumb];
		}
		protected virtual void UpdateSkinThumbState(SkinElementInfo info, TrackBarObjectInfoArgs e) {
			info.State = e.State;
			info.ImageIndex = -1;
		}
		protected virtual void DrawSkinThumb(TrackBarObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GetTrackThumb(e.ViewInfo), GetVerticalThumbRectangle(e.ViewInfo, e.ViewInfo.ThumbContentBounds));
			UpdateSkinThumbState(info, e);
			new RotateObjectPaintHelper().DrawRotated(e.Cache, info, SkinElementPainter.Default, GetRotateAngle(e.ViewInfo), true);
		}
	}
	public class XPTrackBarPainter : XPObjectPainter {
		public XPTrackBarPainter(Orientation orientation) {
			DrawArgs = new WXPPainterArgs("trackbar", orientation == Orientation.Vertical ? XPConstants.TKP_TRACKVERT : XPConstants.TKP_TRACK, 0);
		}
	}
	public class XPTrackLinePainter : XPObjectPainter {
		public XPTrackLinePainter(Orientation orientation) {
			DrawArgs = new WXPPainterArgs("trackbar", orientation == Orientation.Vertical ? XPConstants.TKP_TICSVERT : XPConstants.TKP_TICS, 0);
		}
	}
	public class XPTrackBarThumbPainter : XPObjectPainter {
		public XPTrackBarThumbPainter() {
			DrawArgs = new WXPPainterArgs("trackbar", 0, 0);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			TrackBarObjectInfoArgs tbe = e as TrackBarObjectInfoArgs;
			if(tbe.ViewInfo.Orientation == Orientation.Horizontal) {
				if(tbe.ViewInfo.TickStyle == TickStyle.Both)
					DrawArgs.PartId = XPConstants.TKP_THUMB;
				else
					DrawArgs.PartId = tbe.ViewInfo.TickStyle == TickStyle.TopLeft ? XPConstants.TKP_THUMBTOP : XPConstants.TKP_THUMBBOTTOM;
			}
			else {
				if(tbe.ViewInfo.TickStyle == TickStyle.Both)
					DrawArgs.PartId = XPConstants.TKP_THUMBVERT;
				else
					DrawArgs.PartId = tbe.ViewInfo.TickStyle == TickStyle.TopLeft ? XPConstants.TKP_THUMBLEFT : XPConstants.TKP_THUMBRIGHT;
			}
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) return 5;
			if((e.State & ObjectState.Pressed) != 0) return 3;
			if((e.State & ObjectState.Hot) != 0) return 2;
			if(e.State == ObjectState.Selected) return 4;
			return 1;
		}
	}
	public class WindowsXPTrackBarObjectPainter : TrackBarObjectPainter {
		XPTrackBarThumbPainter xpainter;
		protected XPTrackBarThumbPainter XPainter {
			get {
				if(xpainter == null) xpainter = new XPTrackBarThumbPainter();
				return xpainter;
			}
		}
		public override void DrawThumb(TrackBarObjectInfoArgs e) {
			Rectangle old = e.Bounds;
			Rectangle r = e.ViewInfo.ThumbContentBounds;
			r.Width++; r.Height++;
			e.Bounds = r;
			ObjectPainter.DrawObject(e.Cache, XPainter, e);
			e.Bounds = old;
		}
		protected override void DrawTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			new XPTrackBarPainter(e.ViewInfo.Orientation).DrawObject(new ObjectInfoArgs(e.Cache, bounds, ObjectState.Normal));
		}
		protected override void DrawLine(TrackBarObjectInfoArgs e, Point p1, Point p2) {
			Color clr = WXPPainter.Default.GetThemeColor(new WXPPainterArgs("trackbar", XPConstants.TKP_TICS, 0), XPConstants.TMT_COLOR);
			e.Paint.DrawLine(e.Graphics, e.Cache.GetPen(clr), p1, p2);
		}
		protected override Brush GetLabelsBrush(TrackBarObjectInfoArgs e) {
			RepositoryItemTrackBar ri = e.ViewInfo.Item;
			if(!ri.LabelAppearance.ForeColor.IsEmpty)
				return new SolidBrush(ri.LabelAppearance.ForeColor);
			return new SolidBrush(WXPPainter.Default.GetThemeColor(new WXPPainterArgs("trackbar", XPConstants.TKP_TICS, 0), XPConstants.TMT_COLOR));
		}
	}
	public class SkinTrackBarObjectPainter : TrackBarObjectPainter {
		ISkinProvider provider;
		public SkinTrackBarObjectPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		protected override ISkinProvider Provider { get { return provider; } }
		protected internal override TrackBarInfoCalculator GetCalculator(TrackBarViewInfo viewInfo) {
			return new TrackBarSkinInfoCalculator(viewInfo, this);
		}
		protected void Draw() { }
		protected override SkinElement GetTrack(TrackBarViewInfo vi) { return EditorsSkins.GetSkin(provider)[EditorsSkins.SkinTrackBarTrack]; }
		public override int ThumbOffsetX(TrackBarViewInfo vi) {
			return GetTrack(vi).ContentMargins.Left;
		}
		protected override void DrawLine(TrackBarObjectInfoArgs e, Point p1, Point p2) {
			e.Paint.DrawLine(e.Graphics, e.Cache.GetPen(EditorsSkins.GetSkin(provider)[EditorsSkins.SkinTrackBarTickLine].Color.GetForeColor()), p1, p2);
		}
		protected override Brush GetLabelsBrush(TrackBarObjectInfoArgs e) {
			RepositoryItemTrackBar ri = e.ViewInfo.Item;
			if(!ri.LabelAppearance.ForeColor.IsEmpty)
				return new SolidBrush(ri.LabelAppearance.ForeColor);
			return new SolidBrush(EditorsSkins.GetSkin(provider)[EditorsSkins.SkinTrackBarTickLine].Color.GetForeColor());
		}
		public override int GetThumbBestWidth(TrackBarViewInfo vi) {
			if(GetTrackThumb(vi).Image == null) return base.GetThumbBestWidth(vi);
			Size res = GetTrackThumb(vi).Image.GetImageBounds(0).Size;
			if(vi.Orientation == Orientation.Vertical) res = new Size(res.Height, res.Width);
			return vi.TrackBarHelper.GetWidth(res);
		}
		public override void DrawThumb(TrackBarObjectInfoArgs e) {
			DrawSkinThumb(e);
		}
		protected override void DrawTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			DrawSkinTrackLineCore(e, bounds);
		}
	}
	public class TrackBarPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			TrackBarViewInfo vi = info.ViewInfo as TrackBarViewInfo;
			vi.TrackInfo.Bounds = info.Bounds;
			vi.TrackInfo.State = vi.TrackInfo.ViewInfo.State;
			ObjectPainter.DrawObject(info.Cache, vi.TrackPainter, vi.TrackInfo);
		}
		protected override bool IsDrawBorderLast(ControlGraphicsInfoArgs info) { return true; }
	}
}
namespace DevExpress.Accessibility {
	public class TrackBarAccessible : BaseEditAccessible {
		public TrackBarAccessible(RepositoryItem item) : base(item) { }
		protected override AccessibleRole GetRole() { return AccessibleRole.Slider; }
		public new TrackBarControl Edit { get { return base.Edit as TrackBarControl; } }
		protected new RepositoryItemTrackBar Item { get { return base.Item as RepositoryItemTrackBar; } }
		public override string Value { get { return Edit.Value.ToString(); } }
		protected TrackBarViewInfo ViewInfo {
			get {
				if(this.Edit == null) return null;
				return this.Edit.ViewInfo as TrackBarViewInfo;
			}
		}
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo ci = base.GetChildrenInfo();
			if(ci == null) ci = new ChildrenInfo();
			ci[ChildType.Button] = 3;
			return ci;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			AddChild(new TrackBarIndicatorAccessible(this));
			AddChild(new TrackBarLeftPageAccessible(this));
			AddChild(new TrackBarRightPageAccessible(this));
		}
	}
	public class TrackBarItemAccessible : BaseAccessible { 
		protected TrackBarAccessible trackBarAcc;
		public TrackBarItemAccessible(TrackBarAccessible trackBarAcc) {
			this.trackBarAcc = trackBarAcc;
		}
		public virtual Rectangle GetBoundsCore() {
			return trackBarAcc.Edit.Bounds;
		}
		public override AccessibleObject Parent {
			get { return trackBarAcc.Accessible; }
		}
	}
	public class TrackBarRightPageAccessible : TrackBarItemAccessible {
		public TrackBarRightPageAccessible(TrackBarAccessible trAcc) : base(trAcc) { }
		protected override string GetName() { return "Page right"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		public override Rectangle ClientBounds {
			get {
				if(trackBarAcc == null) return Rectangle.Empty;
				return trackBarAcc.Edit.ViewInfo.TrackPainter.GetEmptyRect(trackBarAcc.Edit.ViewInfo.TrackInfo);
			}
		}
		protected override AccessibleStates GetState() {
			if(trackBarAcc == null) return base.GetState();
			if(trackBarAcc.Edit.Value == trackBarAcc.Edit.Properties.Maximum) return AccessibleStates.Invisible;
			return AccessibleStates.Default;
		}
	}
	public class TrackBarLeftPageAccessible : TrackBarItemAccessible {
		public TrackBarLeftPageAccessible(TrackBarAccessible trAcc) : base(trAcc) { }
		protected override string GetName() { return "Page left"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		public override Rectangle ClientBounds {
			get {
				if(trackBarAcc == null) return Rectangle.Empty;
				return trackBarAcc.Edit.ViewInfo.TrackPainter.GetFilledRect(trackBarAcc.Edit.ViewInfo.TrackInfo);
			}
		}
		protected override AccessibleStates GetState() {
			if(trackBarAcc == null) return base.GetState();
			if(trackBarAcc.Edit.Value == trackBarAcc.Edit.Properties.Minimum) return AccessibleStates.Invisible;
			return AccessibleStates.Default;
		}
	}
	public class TrackBarIndicatorAccessible : TrackBarItemAccessible {
		public TrackBarIndicatorAccessible(TrackBarAccessible trAcc) : base(trAcc) { }
		protected override string GetName() { return "Position"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.Indicator; }
		public override Rectangle ClientBounds {
			get {
				if(trackBarAcc == null) return Rectangle.Empty;
				return trackBarAcc.Edit.ViewInfo.ThumbBounds;
			}
		}
	}
}
namespace DevExpress.XtraEditors {
	class TrackBarControlUnsafeNativeMethods {
		public static int SM_CYHSCROLL = 3;
		public static int IDC_IO_INSERTCONTROL = 0x2114;
		public static int IDC_IO_ADDCONTROL = 0x2115;
		public static int PreferredDimension {
			get {
				int scrollArrowHeight = EditorsNativeMethods.GetSystemMetrics(TrackBarControlUnsafeNativeMethods.SM_CYHSCROLL);
				return scrollArrowHeight * 8 / 3;
			}
		}
	}
	class TrackBarControlSafeNativeMethods {
		public static int LOWORD(int dwParam) { return (dwParam & 0xffff); }
		public static int LOWORD(IntPtr dwParam) { return TrackBarControlSafeNativeMethods.LOWORD((int)((long)dwParam)); }
	}
	public class TrackBarValueToolTipEventArgs : CancelEventArgs {
		ToolTipControllerShowEventArgs showArgs;
		Rectangle thumbBounds;
		public TrackBarValueToolTipEventArgs() { }
		public TrackBarValueToolTipEventArgs(ToolTipControllerShowEventArgs showArgs) {
			this.showArgs = showArgs;
		}
		public ToolTipControllerShowEventArgs ShowArgs { get { return showArgs; } set { showArgs = value; } }
		public Rectangle ThumbBounds { get { return thumbBounds; } set { thumbBounds = value; } }
	}
	public delegate void TrackBarValueToolTipEventHandler(object sender, TrackBarValueToolTipEventArgs e);
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.TrackBarDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows a value to be selected by dragging a small thumb."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "TrackBarControl")
	]
	public class TrackBarControl : BaseEdit, IXtraResizableControl, ISupportInitialize, IGestureClient {
		static Point InvalidPoint = new Point(-10000, -10000);
		bool initializing = false;
		bool autoSize = true;
		protected int requestedDim = 0;
		public TrackBarControl() {
			SetControlStyle();
			this.requestedDim = ViewInfo.PreferredDimension;			
		}		
		protected virtual void SetControlStyle() {
			SetStyle(ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
		} 
		protected override bool AllowPaintBackground {
			get {
				if(BackColor == Color.Transparent || Properties.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return true;
				return base.AllowPaintBackground;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemTrackBar Properties { get { return base.Properties as RepositoryItemTrackBar; } }
		public override int CalcMinHeight() {
			if(Properties.GetAutoSize()) return ViewInfo.PreferredDimension;
			return this.requestedDim; 
		}
		public override bool AutoSize {
			get {
				return autoSize;
			}
			set {
				if(AutoSize == value)
					return;
				autoSize = value;
				if(Properties.Orientation == Orientation.Horizontal) {
					SetStyle(ControlStyles.FixedHeight, autoSize);
					SetStyle(ControlStyles.FixedWidth, false);
				}
				else {
					SetStyle(ControlStyles.FixedWidth, autoSize);
					SetStyle(ControlStyles.FixedHeight, false);
				}
				AdjustSize();
				OnAutoSizeChanged(EventArgs.Empty);
			}
		}
		bool ShouldAdjustSizeOnHandleCreated { get; set; }
		protected internal virtual void AdjustSize() {
			if(IsHandleCreated) {
				int requestedDim = this.requestedDim;
				try {
					if(Properties.Orientation == Orientation.Horizontal) {
						Height = CalcMinHeight();
					}
					else {
						Width = CalcMinHeight();
					}
				}
				finally {
					this.requestedDim = requestedDim;
				}
			}
			else
				ShouldAdjustSizeOnHandleCreated = true;
		}
		internal bool Initializing {
			get { return initializing; }
			set { initializing = value; }
		}
		public event EventHandler<CustomLabelEventArgs> CustomLabel;
		protected virtual void RaiseCustomLabel(CustomLabelEventArgs e) {
			if(CustomLabel != null)
				CustomLabel(this, e);
		}
		public virtual void RefreshLabels() {
			for(int i = Properties.Minimum; i <= Properties.Maximum; i++) {
				CustomLabelEventArgs e = new CustomLabelEventArgs(i);
				e.LabelInfo = Properties.Labels.IsItemExist(i);
				RaiseCustomLabel(e);
				if(!e.Handled)
					continue;
				Properties.Labels.Add(e.LabelInfo);
			}
		}
		void ISupportInitialize.BeginInit() {
			Initializing = true;
		}
		void ISupportInitialize.EndInit() {
			Initializing = false;
			OnEndInitCore();
		}
		protected virtual void OnEndInitCore() {
			if(Properties.Minimum >= Properties.Maximum)
				Properties.Minimum = Properties.Maximum;
			RefreshLabels();
		}
		public override InplaceType InplaceType {
			get { return base.InplaceType; }
			set {
				base.InplaceType = value;
				if(InplaceType != InplaceType.Standalone)
					AutoSize = false;
			}
		}
		protected override Size CalcSizeableMaxSize() {
			if(Properties.GetAutoSize() && !Initializing) {
				if(Properties.Orientation == Orientation.Horizontal)
					return new Size(0, ViewInfo.PreferredDimension);
				return new Size(ViewInfo.PreferredDimension, 0);
			}
			return Size.Empty;
		}
		protected virtual Size CorrectProposedSize(Size proposedSize) {
			if(proposedSize.Width == 1 || proposedSize.Width == int.MaxValue)
				proposedSize.Width = 0;
			if(proposedSize.Height == 1 || proposedSize.Height == int.MaxValue)
				proposedSize.Height = 0;
			return proposedSize;
		}
		public override Size GetPreferredSize(Size proposedSize) {
			proposedSize = CorrectProposedSize(proposedSize);
			if(Properties.Orientation == Orientation.Horizontal)
				return new Size(proposedSize.Width == 0 ? Width : proposedSize.Width, ViewInfo.PreferredDimension);
			return new Size(ViewInfo.PreferredDimension, proposedSize.Height == 0 ? Height : proposedSize.Height);
		}
		protected internal void ChangeStyleHorizontal(bool patchSize) {
			SetStyle(ControlStyles.FixedHeight, Properties.GetAutoSize());
			SetStyle(ControlStyles.FixedWidth, false);
			if(patchSize) {
				Width = this.requestedDim;
			}
		}
		protected internal void ChangeStyleVertical(bool patchSize) {
			SetStyle(ControlStyles.FixedHeight, false);
			SetStyle(ControlStyles.FixedWidth, Properties.GetAutoSize());
			if(patchSize) {
				Height = this.requestedDim;
		}
		}
		protected override bool CheckAutoHeight() { return false; }
		protected override void InitializeDefault() {
			base.InitializeDefault();
			SetStyle(ControlStyles.UseTextForAccessibility, false);
			SetStyle(ControlStyles.Selectable, true);
		}
		protected virtual object ConvertCheckValue(object val) {
			if((val == null) || (val is DBNull) || !Properties.ShouldConvertValue) {
				return val;
			}
			return this.Properties.ConvertValue(val);
		}
		[Browsable(false), DefaultValue(0)]
		public override object EditValue {
			get { return ConvertCheckValue(base.EditValue); }
			set { base.EditValue = ConvertCheckValue(value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlValue"),
#endif
 DefaultValue(0), DXCategory(CategoryName.Behavior), Bindable(true), SmartTagProperty("Value", "")]
		public int Value {
			get { return Properties.ConvertValue(EditValue); }
			set { 
				EditValue = Properties.CheckValue(value); 
				if(IsHandleCreated) Update();
			}
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "TrackBarControl"; } }
		protected virtual bool ShouldProcessTrackKeyDown { get { return true; } }
		protected override bool IsInputKey(Keys keyData) {
			if(!ShouldProcessTrackKeyDown) return base.IsInputKey(keyData);
			if((keyData & Keys.Alt) == Keys.Alt) return false;
			switch(keyData & Keys.KeyCode) {
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.End:
				case Keys.Home:
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
					return true;
			}
			return base.IsInputKey(keyData);
		}
		public void MoveLeft() {
			if(Properties.ReadOnly) return;
			Value -= Properties.SmallChange; 
		}
		public void MoveRight() {
			if(Properties.ReadOnly) return;
			Value += Properties.SmallChange; 
		}
		public void MoveLargeLeft() {
			if(Properties.ReadOnly) return;
			Value -= Properties.LargeChange; 
		}
		public void MoveLargeRight() {
			if(Properties.ReadOnly) return;
			Value += Properties.LargeChange; 
		}
		public void MoveBegin() {
			if(Properties.ReadOnly) return;
			Value = Properties.Minimum; 
		}
		public void MoveEnd() {
			if(Properties.ReadOnly) return;
			Value = Properties.Maximum; 
		}
		protected internal override void UpdateFixedHeight() { }
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseWheel(ee);
			if(ee.Handled || Properties.ReadOnly || !Properties.AllowMouseWheel) return;
			ee.Handled = true;
			int delta = (int)(e.Delta / 120f) * SystemInformation.MouseWheelScrollLines;
			int multiplier = Properties.SmallChangeUseMode == SmallChangeUseMode.ArrowKeys ? 1 : Properties.SmallChange;
			if(Properties.Maximum - Properties.Minimum < 11) delta = e.Delta < 0 ? -1 : 1;
			Value += delta * multiplier;
		}
		bool controlActivated = false;
		bool IsControlActivated {
			get { return InplaceType == InplaceType.Grid && controlActivated; }
		}
		protected override void OnEnter(EventArgs e) {
			controlActivated = true;
			base.OnEnter(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			this.requestedDim = Properties.Orientation == Orientation.Horizontal ? height : width;
			if(Properties.GetAutoSize() && !Initializing) {
				if(Properties.Orientation == Orientation.Horizontal) {
					if((specified & BoundsSpecified.Height) != BoundsSpecified.None) {
						height = CalcMinHeight();
					}
				}
				else if((specified & BoundsSpecified.Width) != BoundsSpecified.None) {
					width = CalcMinHeight();
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		public override string ToString() {
			string text = base.ToString();
			return (text + ", Minimum: " + this.Properties.Minimum.ToString(System.Globalization.CultureInfo.CurrentCulture) + ", Maximum: " + this.Properties.Maximum.ToString(System.Globalization.CultureInfo.CurrentCulture) + ", Value: " + this.Value.ToString(System.Globalization.CultureInfo.CurrentCulture));
		}
		protected override ImeMode DefaultImeMode { get { return ImeMode.Disable; } }
		protected override Size DefaultSize { get { return new Size(104, TrackBarControlUnsafeNativeMethods.PreferredDimension); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
#if DXWhidbey        
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
#endif
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get { return SystemColors.WindowText; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ImeMode ImeMode {
			get { return base.ImeMode; }
			set { base.ImeMode = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Bindable(false), Browsable(false)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
#if DXWhidbey        
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Padding Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
#endif
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlBeforeShowValueToolTip"),
#endif
 EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public event TrackBarValueToolTipEventHandler BeforeShowValueToolTip {
			add { Properties.BeforeShowValueToolTip += value; }
			remove { Properties.BeforeShowValueToolTip -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlAutoSizeChanged"),
#endif
 EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DXCategory(CategoryName.PropertyChanged)]
		public new event EventHandler AutoSizeChanged {
			add { Properties.AutoSizeChanged += value; }
			remove { Properties.AutoSizeChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageChanged {
			add { base.BackgroundImageChanged += value; }
			remove { base.BackgroundImageChanged -= value; }
		}
#if DXWhidbey                    
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageLayoutChanged {
			add { base.BackgroundImageLayoutChanged += value; }
			remove { base.BackgroundImageLayoutChanged -= value; }
		}
#endif
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler ForeColorChanged {
			add { base.ForeColorChanged += value; }
			remove { base.ForeColorChanged -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler ImeModeChanged {
			add { base.ImeModeChanged += value; }
			remove { base.ImeModeChanged -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler TextChanged {
			add { base.TextChanged += value; }
			remove { base.TextChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlValueChanged"),
#endif
 DXCategory(CategoryName.Action)]
		public event EventHandler ValueChanged {
			add { Properties.ValueChanged += value; }
			remove { Properties.ValueChanged -= value; }
		}
		protected bool Invert { 
			get {
				if((Properties.Orientation == Orientation.Horizontal && WindowsFormsSettings.GetIsRightToLeft(this)) ||
					(Properties.Orientation == Orientation.Vertical && RightToLeft == RightToLeft.No))
					return !Properties.InvertLayout;
				return Properties.InvertLayout;
			} 
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(!ShouldProcessTrackKeyDown) return;
			if(e.Handled) return;
			switch(e.KeyCode) { 
				case Keys.Up:
				case Keys.Left:
					if(!Invert)MoveLeft();
					else MoveRight();
					break;
				case Keys.Down:
				case Keys.Right:
					if(!Invert) MoveRight();
					else MoveLeft();
					break;
				case Keys.PageUp:
					if(!Invert) MoveLargeLeft();
					else MoveLargeRight();
					break;
				case Keys.PageDown:
					if(!Invert) MoveLargeRight();
					else MoveLargeLeft();
					break;
				case Keys.Home:
					if(!Invert) MoveBegin();
					else MoveEnd();
					break;
				case Keys.End:
					if(!Invert) MoveEnd();
					else MoveBegin();
					break;
			}
		}
		protected internal new TrackBarViewInfo ViewInfo { get { return base.ViewInfo as TrackBarViewInfo; } }
		protected virtual bool ShouldProcessHitTest(EditHitInfo hi) { return hi.HitTest == EditHitTest.Button; }
		protected virtual void ProcessHitTestOnMouseDown(EditHitInfo hi) {
			ViewInfo.PressedInfo = hi;
		}
		bool shouldUpdateViewInfoState = true;
		protected internal bool ShouldUpdateViewInfoState { get { return shouldUpdateViewInfoState; } set { shouldUpdateViewInfoState = value; } }
		protected override void UpdateViewInfoState(bool useValidMouse) {
			if(!ShouldUpdateViewInfoState) return;
			base.UpdateViewInfoState(useValidMouse);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ee);
			if(ee.Handled || ProcessGesture) return;
			bool isLeft = (e.Button == MouseButtons.Left);
			ProcessMouseDown(e.Location, isLeft);
		}
		void ProcessMouseDown(Point p, bool isLeft) {
			UpdateViewInfoState(true, p);
			if(isLeft) {
				EditHitInfo hi = ViewInfo.CalcHitInfo(p);
				if(ShouldProcessHitTest(hi)) {
					ProcessHitTestOnMouseDown(hi);
					ShouldUpdateViewInfoState = false;
					RefreshVisualLayout();
				}
				else {
					if(IsControlActivated) {
						controlActivated = false;
						return;
					}
					UpdateValueFromPoint(p);
				}
			}
		}
		protected virtual bool ShouldUpdateValueFromPoint(Point pt) {
			Rectangle rect = ViewInfo.TrackLineRect;
			if(Properties.Orientation == Orientation.Vertical)
				rect = ViewInfo.TrackBarHelper.Rotate(ViewInfo.TrackLineRect);
			return !Properties.ReadOnly && rect.Contains(pt);
		}
		protected virtual void UpdateValueFromPoint(Point pt) {
			if(!ShouldUpdateValueFromPoint(pt)) return;
			Value = ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(pt));
		}
		protected virtual void ProcessHitTestOnMouseUp(EditHitInfo hi) {
			ClearHotPressed();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseUp(ee);
			if(ee.Handled) return;
			bool isLeft = (e.Button == MouseButtons.Left);
			ProcessMouseUp(e.Location, isLeft);
		}
		void ProcessMouseUp(Point p, bool isLeft) {
			ShouldUpdateViewInfoState = true;
			UpdateViewInfoState();
			if(isLeft) {
				EditHitInfo hi = ViewInfo.CalcHitInfo(p);
				ProcessHitTestOnMouseUp(hi);
			}
			GetToolTipController().HideHint();
		}
		protected virtual bool ShouldProcessTrackBarMouseMove { get { return !Properties.ReadOnly; } }
		protected virtual void ShowValue(string valueString, Rectangle thumbBounds) {
			if(!Properties.ShowValueToolTip || GetToolTipController() == null) return;
			ToolTipControllerShowEventArgs info = new ToolTipControllerShowEventArgs(this, this, valueString, "", ToolTipLocation.RightTop, false, false, 0, ToolTipIconType.None, ToolTipIconSize.Small, null, -1, GetToolTipController().Appearance, GetToolTipController().AppearanceTitle);
			TrackBarValueToolTipEventArgs e = new TrackBarValueToolTipEventArgs();
			e.ShowArgs = info;
			e.ShowArgs.ToolTipType = ToolTipType.SuperTip;
			e.ThumbBounds = thumbBounds;
			Properties.RaiseBeforeShowValue(e);
			if(e.Cancel) {
				GetToolTipController().HideHint();
				return;
			}
			GetToolTipController().ShowHint(info, PointToScreen(new Point(thumbBounds.Right, thumbBounds.Top)));
		}
		protected virtual ToolTipController GetToolTipController() {
			if(ToolTipController != null)
				return ToolTipController;
			return ToolTipController.DefaultController;
		}
		protected virtual void ShowValue() {
			ShowValue(Value.ToString(), ViewInfo.ThumbBounds);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ee);
			if(ee.Handled || ProcessGesture) return;
			ProcessMouseMove(e.Location);
		}
		void ProcessMouseMove(Point p) {
			if(!ShouldProcessTrackBarMouseMove) return;
			if(ViewInfo.PressedInfo.HitTest == EditHitTest.Button) {
				OnProcessThumb(p);
			}
		}
		protected virtual void OnProcessThumb(Point p) {
			Value = ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(p));
			ShowValue();
		}
		Size IXtraResizableControl.MinSize {
			get {
				if(Properties.AutoSize) return new Size(ViewInfo.PreferredDimension, ViewInfo.PreferredDimension);
				if(Properties.Orientation == Orientation.Horizontal) return new Size(ViewInfo.PreferredDimension, 10);
				return new Size(10, ViewInfo.PreferredDimension);
			}
		}
		protected internal virtual void OnOrientationChanged() {
			if(Properties.Orientation == Orientation.Horizontal)
				ChangeStyleHorizontal(true);
			else
				ChangeStyleVertical(true);
			if(IsHandleCreated && !Initializing) {
				Rectangle newBounds = Bounds;
				SetBounds(newBounds.X, newBounds.Y, newBounds.Height, newBounds.Width, BoundsSpecified.All);
				AdjustSize();
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(NativeVista.IsWindows7) {
				TogglePressAndHold(false);
			}
			if(ShouldAdjustSizeOnHandleCreated) {
				ShouldAdjustSizeOnHandleCreated = false;				
				AdjustSize();
			}
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			if(NativeVista.IsWindows7) {
				EditorsNativeMethods.GlobalDeleteAtom(atomID);
			}
		}
		ushort atomID = 0;
		bool TogglePressAndHold(bool enable) {
			string tabletAtom = "MicrosoftTabletPenServiceProperty";
			atomID = EditorsNativeMethods.GlobalAddAtom(tabletAtom);
			if(atomID == 0) {
				return false;
			}
			if(enable)
				return EditorsNativeMethods.RemoveProp(Handle, tabletAtom) != IntPtr.Zero;
			return EditorsNativeMethods.SetProp(Handle, tabletAtom, new IntPtr(1));
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.KeyData == Keys.Enter && EnterMoveNextControl) {
				this.ProcessDialogKey(Keys.Tab);
				e.Handled = true;
			}
		}
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
		}
		protected virtual void ProcessGidBegin(Point p) {
			ProcessMouseDown(p, true);
		}
		protected virtual void ProcessGidPan(Point p) {
			ProcessMouseMove(p);
		}
		protected virtual void ProcessGidEnd(Point p) {
			ProcessMouseUp(p, true);
		}
		TrackBarGestureHelper gestureHelper;
		public TrackBarGestureHelper GestureHelper {
			get {
				if(gestureHelper == null)
					gestureHelper = new TrackBarGestureHelper(this);
				return gestureHelper;
			}
		}
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			return new GestureAllowArgs[] { GestureAllowArgs.Pan };
		}
		bool processGesture = false;
		protected bool ProcessGesture { get { return processGesture; } }
		void IGestureClient.OnEnd(GestureArgs info) {
			ProcessGidEnd(info.End.Point);
			processGesture = false;
		}
		Point prevGidPoint = Point.Empty;
		void IGestureClient.OnBegin(GestureArgs info) {
			processGesture = true;
			prevGidPoint = info.Start.Point;
			ProcessGidBegin(info.Start.Point);
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(prevGidPoint != info.Current.Point) {
				ProcessGidPan(info.Current.Point);
				prevGidPoint = info.Current.Point;
			}
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		IntPtr IGestureClient.OverPanWindowHandle { get { return TrackBarGestureHelper.FindOverpanWindow(this); } }
		IntPtr IGestureClient.Handle { get { return IsHandleCreated ? Handle : IntPtr.Zero; } }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
	}
	public class CustomLabelEventArgs : EventArgs {
		public CustomLabelEventArgs(int value) {
			this.LabelInfo = new TrackBarLabel(string.Empty, value);
			this.Handled = false;
		}
		public TrackBarLabel LabelInfo { get; set; }
		public bool Handled { get; set; }
	}
	public class TrackBarLabelInfo {
		public TrackBarLabel Label { get; set; }
		public Rectangle Bounds { get; set; }
		public Rectangle MirrorBounds { get; set; }
	}
	public class TrackBarLabelsComparer : IComparer<TrackBarLabelInfo> {
		#region IComparer<TrackBarLabelInfo> Members
		int IComparer<TrackBarLabelInfo>.Compare(TrackBarLabelInfo x, TrackBarLabelInfo y) {
			if(x.Label.Value < y.Label.Value) return -1;
			if(x.Label.Value > y.Label.Value) return 1;
			return 0;
		}
		#endregion
	}
	public class TrackBarGestureHelper : GestureHelper {
		public TrackBarGestureHelper(IGestureClient owner) : base(owner) { }
		protected override void ProcessGidEnd(ref NativeMethods.GESTUREINFO gi) {
			OnGidEnd(ref gi);
		}
	}
}
