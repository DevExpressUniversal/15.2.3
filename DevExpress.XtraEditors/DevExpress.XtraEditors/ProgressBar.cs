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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Taskbar.Core;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraEditors.Repository {
	public class ProgressBarAppearanceOptions : AppearanceOptions {
		bool useForeColor2 = false;
		public ProgressBarAppearanceOptions() : base() { }
		protected override void ResetOptions() {
			base.ResetOptions();
			useForeColor2 = false;
		}
		[
		 DefaultValue(false), XtraSerializableProperty(),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseForeColor2 {
			get { return useForeColor2; }
			set {
				if(UseForeColor2 == value) return;
				bool prevValue = UseForeColor2;
				useForeColor2 = value;
				OnChanged(ProgressBarAppearanceObject.optUseForeColor2, prevValue, UseForeColor2);
			}
		}
		public override bool IsEqual(AppearanceOptions options) {
			ProgressBarAppearanceOptions po = options as ProgressBarAppearanceOptions;
			if(po == null)
				return base.IsEqual(options);
			return base.IsEqual(options) && this.useForeColor2 == po.useForeColor2;
		}
		protected override bool GetOptionValue(string name) {
			if(IsEqual(name,ProgressBarAppearanceObject.optUseForeColor2)) return UseForeColor2;
			return base.GetOptionValue(name);
		}
		public override void Assign(DevExpress.Utils.Controls.BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ProgressBarAppearanceOptions op = options as ProgressBarAppearanceOptions;
				if(op != null)
					useForeColor2 = op.useForeColor2;
			}
			finally {
				EndUpdate();
			}
		}
		protected override bool ShouldSerialize(IComponent owner) {
			return base.ShouldSerialize(owner) && this.useForeColor2 != false;
		}
	}
	public class ProgressBarAppearanceObject : AppearanceObject {
		internal static string optUseForeColor2 = "UseForeColor2";
		Color foreColor2 = Color.Empty;
		public ProgressBarAppearanceObject() : base() { }
		public ProgressBarAppearanceObject(AppearanceDefault def) : base(def) { }
		public ProgressBarAppearanceObject(AppearanceObject parent) : base(parent) { }
		public ProgressBarAppearanceObject(string name) : base(name) { }
		public ProgressBarAppearanceObject(AppearanceObject main, AppearanceDefault def) : base(main, def) { }
		public ProgressBarAppearanceObject(AppearanceObject main, AppearanceObject def) : base(main, def) { }
		public ProgressBarAppearanceObject(AppearanceObject parent, string name) : base(parent, name) { }
		public ProgressBarAppearanceObject(IAppearanceOwner owner, AppearanceObject parent) : base(owner, parent) { }
		public ProgressBarAppearanceObject(IAppearanceOwner owner, bool reserved) : base(owner, reserved) { }
		public ProgressBarAppearanceObject(IAppearanceOwner owner, AppearanceObject parent, string name) : base(owner, parent, name) { }
		void ResetForeColor() { ForeColor = Color.Empty; }
		protected bool ShouldSerializeForeColor2() { return ForeColor2 != Color.Empty; }
		[
		XtraSerializableProperty()
		]
		public Color ForeColor2 {
			get { return foreColor2; }
			set {
				if(ForeColor2 == value) return;
				foreColor2 = value;
				if(!IsLoading) {
					try { ProgressOptions.BeginUpdate(); ProgressOptions.UseForeColor2 = foreColor2 != Color.Empty; }
					finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		public new ProgressBarAppearanceOptions Options {
			get { return (ProgressBarAppearanceOptions)base.Options; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ProgressBarAppearanceOptions ProgressOptions { get { return Options as ProgressBarAppearanceOptions; } }
		protected override AppearanceOptions CreateOptions() {
			return new ProgressBarAppearanceOptions();
		}
		public override object Clone() {
			ProgressBarAppearanceObject obj = new ProgressBarAppearanceObject();
			obj.Assign(this);
			return obj;
		}
		public override void Assign(AppearanceObject val) {
			base.Assign(val);
			ProgressBarAppearanceObject po = val as ProgressBarAppearanceObject;
			if(po != null)
				this.foreColor2 = po.foreColor2;
		}
		public override bool IsEqual(AppearanceObject val) {
			ProgressBarAppearanceObject obj = val as ProgressBarAppearanceObject;
			if(obj == null)
				return false;
			return base.IsEqual(val) && this.foreColor2 == obj.foreColor2;
		}
		public Color GetForeColor2() {
			AppearanceObject obj = GetAppearanceByOption(optUseForeColor2);
			ProgressBarAppearanceObject po = obj as ProgressBarAppearanceObject;
			if(po != null)
				return po.ForeColor2;
			return obj.ForeColor;
		}
		public Brush GetFore2Brush(GraphicsCache cache) {
			if(cache == null) return new SolidBrush(GetForeColor2());
			return cache.GetSolidBrush(GetForeColor2());
		}
	}
	public class RepositoryItemBaseProgressBar : RepositoryItem {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemBaseProgressBar Properties { get { return this; } }
		Color _startColor, _endColor;
		ProgressViewStyle _progressViewStyle;
		ProgressKind _progressKind;
		bool _showTitle;
		TextOrientation _textOrientation;
		int maxHeight;
		bool forceFillBackground;
		public RepositoryItemBaseProgressBar() {
			this.maxHeight = 0;
			this.forceFillBackground = true;
			this.fAutoHeight = false; 
			this._startColor = SystemColors.Highlight;
			this._endColor = SystemColors.Highlight;
			this._progressViewStyle = ProgressViewStyle.Broken;
			this._progressKind = ProgressKind.Horizontal;
			this._showTitle = false;
			this._textOrientation = TextOrientation.Default;
		}
		Padding progressPadding;
		bool ShouldSerializeProgressPadding() { return !ProgressPadding.Equals(Padding.Empty); }
		void ResetProgressPadding() { ProgressPadding = Padding.Empty; }
		public virtual Padding ProgressPadding {
			get { return progressPadding; }
			set {
				if(ProgressPadding == value)
					return;
				progressPadding = value;
				OnPropertiesChanged();
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemBaseProgressBar source = item as RepositoryItemBaseProgressBar;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.forceFillBackground = source.ForceFillBackgroundCore;
				this.maxHeight = source.MaxHeight;
				this._endColor = source.EndColor;
				this._startColor = source.StartColor;
				this._progressViewStyle = source.ProgressViewStyle;
				this._progressKind = source.ProgressKind;
				this._showTitle = source.ShowTitle;
				this._textOrientation = source.TextOrientation;
				this.progressPadding = source.progressPadding;
			}
			finally {
				EndUpdate();
			}
		}
		protected override AppearanceObject CreateAppearanceCore(string name) {
			return new ProgressBarAppearanceObject(name);
		}
		protected override void OnAllowFocusedChanged() {
			base.OnAllowFocusedChanged();
			if(OwnerEdit != null) {
				ProgressBarBaseControl prog = (ProgressBarBaseControl)OwnerEdit;
				prog.OnAllowFocusedChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarMaxHeight"),
#endif
 DefaultValue(0)]
		public int MaxHeight {
			get { return maxHeight; }
			set {
				if(value < 1) value = 0;
				if(MaxHeight == value) return;
				maxHeight = value;
				LayoutChanged();
			}
		}
		bool ShouldSerializeStartColor() { return StartColor != SystemColors.Highlight; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarStartColor")
#else
	Description("")
#endif
]
		public Color StartColor {
			get { return _startColor; }
			set {
				if(StartColor == value) return;
				_startColor = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeEndColor() { return EndColor != SystemColors.Highlight; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarEndColor")
#else
	Description("")
#endif
]
		public Color EndColor {
			get { return _endColor; }
			set {
				if(EndColor == value) return;
				_endColor = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarProgressViewStyle"),
#endif
 DefaultValue(ProgressViewStyle.Broken)]
		public virtual ProgressViewStyle ProgressViewStyle {
			get { return _progressViewStyle; }
			set {
				if(ProgressViewStyle == value) return;
				_progressViewStyle = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarProgressKind"),
#endif
 DefaultValue(ProgressKind.Horizontal), SmartTagProperty("Progress Kind", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public ProgressKind ProgressKind {
			get { return _progressKind; }
			set {
				if(ProgressKind == value) return;
				_progressKind = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarShowTitle"),
#endif
 DefaultValue(false), SmartTagProperty("Show Title", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool ShowTitle {
			get { return _showTitle; }
			set {
				if(ShowTitle == value) return;
				_showTitle = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarTextOrientation"),
#endif
 DefaultValue(TextOrientation.Default), SmartTagProperty("Text Orientation", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public TextOrientation TextOrientation {
			get { return _textOrientation; }
			set {
				if(TextOrientation == value) return;
				_textOrientation = value;
				OnPropertiesChanged();
			}
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			if (ExportMode == Repository.ExportMode.DisplayText) {
				return CreateTextBrick(info);
			}
			BrickStyle style = CreateBrickStyle(info, "Progress");
			SetupTextBrickStyleProperties(info, style);
			IProgressBarBrick brick = new ProgressBarBrick();
			SetCommonBrickProperties(brick, info);
			brick.Style = style;
			brick.Hint = GetDisplayText(info.EditValue);
			return brick;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseProgressBarAutoHeight"),
#endif
 DefaultValue(false)]
		public override bool AutoHeight {
			get { return base.AutoHeight; }
			set { base.AutoHeight = value; }
		}
		protected internal override bool IsUseDisplayFormat { get { return true; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is obsolete. Use DrawBackground instead", false)]
		public bool ForceFillBackground {
			get { return ForceFillBackgroundCore;
			}
			set { ForceFillBackgroundCore = value; }
		}
		protected internal bool ForceFillBackgroundCore {
			get {
				if(DrawBackground == DrawBackgroundType.Default)
					if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone) return false;
				if(DrawBackground == DrawBackgroundType.False) return false;
				if(DrawBackground == DrawBackgroundType.True) return true;
				return forceFillBackground;
			}
			set { forceFillBackground = value; }
		}
		protected internal bool GetForceFillBackground() { return ProgressPadding.Horizontal > 0 || ProgressPadding.Vertical > 0 || ForceFillBackgroundCore; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(DrawBackgroundType.Default)]
		public DrawBackgroundType DrawBackground { get; set; }
		public enum DrawBackgroundType { Default, True, False }
	}
	public class RepositoryItemMarqueeProgressBar : RepositoryItemBaseProgressBar {
		int _marqueeAnimationSpeed;
		bool _stopped;
		bool _paused;
		ProgressAnimationMode _paMode;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemMarqueeProgressBar Properties { get { return this; } }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RepositoryItemMarqueeProgressBarEditorTypeName")]
#endif
		public override string EditorTypeName { get { return "MarqueeProgressBarControl"; } }
		public RepositoryItemMarqueeProgressBar() {
			this._marqueeAnimationSpeed = 100;
			this._paMode = ProgressAnimationMode.Default;
			this._stopped = false;
			this._paused = false;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMarqueeProgressBarStopped"),
#endif
 DefaultValue(false)]
		public bool Stopped {
			get { return _stopped; }
			set { 
				if(_stopped == value) return;
				_stopped = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMarqueeProgressBarPaused"),
#endif
 DefaultValue(false)]
		public bool Paused {
			get { return _paused; }
			set {
				if(_paused == value) return;
				_paused = value;
				OnPropertiesChanged();
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemMarqueeProgressBar source = item as RepositoryItemMarqueeProgressBar;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this._marqueeAnimationSpeed = source._marqueeAnimationSpeed;
				this._stopped = source._stopped;
				this._paused = source._paused;
				this._paMode = source._paMode;
				this.marqueeWidth = source.marqueeWidth;
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMarqueeProgressBarProgressAnimationMode"),
#endif
 DefaultValue(ProgressAnimationMode.Default), SmartTagProperty("Progress Animation Mode", "")]
		public ProgressAnimationMode ProgressAnimationMode { 
			get { return _paMode; } 
			set {
				if(_paMode == value) return;
				_paMode = value;
				OnPropertiesChanged();
			} 
		}
		bool ShouldSerializeMarqueeAnimationSpeed() { return MarqueeAnimationSpeed != 100; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMarqueeProgressBarMarqueeAnimationSpeed"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(100), SmartTagProperty("Animation Speed", "")]
		public int MarqueeAnimationSpeed {
			get { return _marqueeAnimationSpeed; }
			set {
				if(value < 1) value = 1;
				if(MarqueeAnimationSpeed == value) return;
				_marqueeAnimationSpeed = value;
				OnPropertiesChanged();
			}
		}
		int marqueeWidth = 100;
		[DefaultValue(100)]
		public int MarqueeWidth {
			get { return marqueeWidth; }
			set {
				if(MarqueeWidth == value)
					return;
				marqueeWidth = value;
				OnPropertiesChanged();
			}
		}
	}
	public class RepositoryItemProgressBar : RepositoryItemBaseProgressBar {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemProgressBar Properties { get { return this; } }
		private static object positionChanged = new object();
		[Browsable(false)]
		public override string EditorTypeName { get { return "ProgressBarControl"; } }
		int _minimum, _maximum, _step;
		bool _percentView;
		bool _flowAnimationEnabled;
		int _flowAnimationSpeed;
		int _flowAnimationDelay;
		LinearSequenceGenerator sequenceSource;
		public RepositoryItemProgressBar() {
			this._minimum = 0;
			this._maximum = 100;
			this._step = 10;
			this._percentView = true;
			this._flowAnimationEnabled = false;
			this._flowAnimationSpeed = 180;
			this._flowAnimationDelay = 1000;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemProgressBar source = item as RepositoryItemProgressBar;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this._maximum = source.Maximum;
				this._minimum = source.Minimum;
				this._percentView = source.PercentView;
				this._step = source.Step;
				this._flowAnimationEnabled = source.FlowAnimationEnabled;
				this._flowAnimationSpeed = source.FlowAnimationSpeed;
				this._flowAnimationDelay = source.FlowAnimationDelay;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(positionChanged, source.Events[positionChanged]);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public new ProgressBarAppearanceObject Appearance {
			get { return (ProgressBarAppearanceObject)base.Appearance; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearanceDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public new ProgressBarAppearanceObject AppearanceDisabled {
			get { return (ProgressBarAppearanceObject)base.AppearanceDisabled; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearanceFocused"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public new ProgressBarAppearanceObject AppearanceFocused {
			get { return (ProgressBarAppearanceObject)base.AppearanceFocused; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearanceReadOnly"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public new ProgressBarAppearanceObject AppearanceReadOnly {
			get { return (ProgressBarAppearanceObject)base.AppearanceReadOnly; }
		}
		protected new ProgressBarControl OwnerEdit { get { return base.OwnerEdit as ProgressBarControl; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemProgressBarMinimum"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(0), SmartTagProperty("Minimum", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public int Minimum {
			get { return _minimum; }
			set {
				if(!IsLoading) {
					if(value > Maximum) value = Maximum;
				}
				if(Minimum == value) return;
				_minimum = value;
				OnMinMaxChanged();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemProgressBarMaximum"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(100), SmartTagProperty("Maximum", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public int Maximum {
			get { return _maximum; }
			set {
				if(!IsLoading) {
					if(value < Minimum) value = Minimum;
				}
				if(Maximum == value) return;
				_maximum = value;
				OnMinMaxChanged();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemProgressBarStep"),
#endif
 DefaultValue(10), SmartTagProperty("Step", "")]
		public int Step {
			get { return _step; }
			set {
				if(Step == value) return;
				_step = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemProgressBarPercentView"),
#endif
 DefaultValue(true)]
		public bool PercentView {
			get { return _percentView; }
			set {
				if(PercentView == value) return;
				_percentView = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemProgressBarFlowAnimationEnabled"),
#endif
 DefaultValue(false)]
		public bool FlowAnimationEnabled {
			get { return _flowAnimationEnabled; }
			set {
				if(FlowAnimationEnabled == value) return;
				_flowAnimationEnabled = value;
				if(OwnerEdit != null) OwnerEdit.ConfigureAnimationTimer();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(180)]
		public int FlowAnimationSpeed {
			get { return _flowAnimationSpeed; }
			set {
				if(FlowAnimationSpeed == value) return;
				_flowAnimationSpeed = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(1000)]
		public int FlowAnimationDelay {
			get { return _flowAnimationDelay; }
			set {
				if(FlowAnimationDelay == value) return;
				_flowAnimationDelay = value;
				OnPropertiesChanged();
			}
		}
		protected internal LinearSequenceGenerator SequenceSource {
			get {
				if(sequenceSource == null) {
					sequenceSource = CreateSequenceSource();
				}
				return sequenceSource;
			}
		}
		protected internal virtual LinearSequenceGenerator CreateSequenceSource() {
			return new LinearSequenceGenerator();
		}
		protected internal string GetProgressDisplayText(FormatInfo format, object editValue) {
			string res = "";
			object val = editValue;
			if(PercentView)
				val = GetIntPercents(CalcPercents(ConvertValue(editValue)));
			res = GetDisplayText(format, val);
			if(PercentView && format.FormatType == FormatType.None) res += "%";
			return res;
		}
		protected internal int GetIntPercents(float percents) {
			return (int)(Math.Round(percents * 100.0F));
		}
		protected internal float CalcPercents(int position) {
			float res = 1;
			if(Maximum != Minimum) {
				res = Math.Abs((float)(position - Minimum) / (float)(Maximum - Minimum));
			}
			return res;
		}
		protected internal virtual int ConvertValue(object val) {
			try {
				return CheckValue(Convert.ToInt32(val));
			}
			catch { }
			return Minimum;
		}
		protected internal virtual int CheckValue(int val) {
			if(IsLoading) return val;
			val = Math.Max(val, Minimum);
			val = Math.Min(val, Maximum);
			return val;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			if(ExportMode == Repository.ExportMode.DisplayText) return base.GetBrick(info);
			IProgressBarBrick brick = (IProgressBarBrick)base.GetBrick(info);
			brick.Position = GetIntPercents(CalcPercents(ConvertValue(info.EditValue)));
			brick.Text = brick.Hint = GetProgressDisplayText(DisplayFormat, info.EditValue);
			return brick;
		}
		protected virtual void OnMinMaxChanged() {
			if(OwnerEdit != null) OwnerEdit.OnMinMaxChanged();
		}
		protected virtual void OnTaskBarProgressChanged() {
			if(OwnerEdit != null) OwnerEdit.OnTaskBarProgressChanged();
		}
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Center; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowFocusedAppearance { get { return GetForceFillBackground(); } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemProgressBarPositionChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler PositionChanged {
			add { this.Events.AddHandler(positionChanged, value); }
			remove { this.Events.RemoveHandler(positionChanged, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaisePositionChanged(e);
			if(OwnerEdit != null && OwnerEdit.ShowProgressInTaskBar) OnTaskBarProgressChanged();
		}
		protected internal virtual void RaisePositionChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[positionChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
	}
}
namespace DevExpress.XtraEditors {
	[ToolboxItem(false), Obsolete(ObsoleteText.SRObsoleteProgressBar)]
	public class ProgressBar :ProgressBarControl {
	}
	public class ProgressBarBaseControl :BaseEdit {
		public ProgressBarBaseControl() {
			this.TabStop = false;
			this.fOldEditValue = this.fEditValue = 0;
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		protected override bool AllowPaintBackground {
			get {
				return base.AllowPaintBackground || LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		protected bool IsTransparentControl { get { return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP; } }
		public override Color BackColor {
			get {
				return IsTransparentControl ? Color.Transparent : base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		protected internal virtual void UpdatePercent() {
			ProgressBarBaseViewInfo vi = ViewInfo as ProgressBarBaseViewInfo;
			if(vi == null) return;
			vi.UpdateProgressInfo(vi.ProgressInfo);
		}
		protected override Size CalcSizeableMaxSize() {
			return new Size(0, 18);
		}
		protected override bool SizeableIsCaptionVisible {
			get {
				return false;
			}
		}
		protected override internal bool PaintErrorIconBackground(Graphics g, Rectangle bounds) {
			RectangleF oldClip = g.ClipBounds;
			g.SetClip(bounds);
			bool result = false;
			try {
				result = base.PaintErrorIconBackground(g, bounds);
			}
			finally {
				g.SetClip(oldClip);
			}
			return result;
		}
		protected override Size DefaultSize { get { return new Size(100, 18); } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressBarBaseControlTabStop"),
#endif
 DefaultValue(false)]
		public new bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		protected internal void OnAllowFocusedChanged() {
			SetStyle(ControlStyles.Selectable, Properties.AllowFocused);
			SetStyle(ControlStyles.UserMouse, Properties.AllowFocused);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.KeyData == Keys.Enter && EnterMoveNextControl) {
				this.ProcessDialogKey(Keys.Tab);
				e.Handled = true;
			}
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultBindingPropertyEx("MarqueeAnimationSpeed"),
	 Description("Indicates that an operation is going on."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "MarqueeProgressBarControl")
	]
	public class MarqueeProgressBarControl :ProgressBarBaseControl {
		Timer _timer;
		[Browsable(false)]
		public override string EditorTypeName { get { return "MarqueeProgressBarControl"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MarqueeProgressBarControlProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemMarqueeProgressBar Properties { get { return base.Properties as RepositoryItemMarqueeProgressBar; } }
		public MarqueeProgressBarControl() {
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			base.Dispose(disposing);
			if(disposing && _timer != null) {
				_timer.Tick -= new EventHandler(OnTimer);
				_timer.Dispose();
				_timer = null;
			}
		}
		protected System.Windows.Forms.Timer Timer {
			get {
				if(_timer == null) {
					_timer = new Timer();
					_timer.Tick += new EventHandler(OnTimer);
				}
				return _timer;
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			bool visible = Visible;
			if(!visible)
				ResetTimer();
			else
				SetTimer();
		}
		void ResetTimer() {
			if(_timer != null)
				_timer.Stop();
		}
		void SetTimer() {
			if(IsDesignMode == true) return;
			Timer.Stop();
			Timer.Interval = 25;
			Timer.Start();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(!DesignMode)
				SetTimer();
		}
		void OnTimer(object sender, EventArgs e) {
			if(Properties.Paused) return;
			this.Invalidate();
			this.Update();
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultBindingPropertyEx("Position"),
	 Description("Indicates the progress of a lengthy operation or operation rate."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ProgressBarControl")
	]
	public class ProgressBarControl :ProgressBarBaseControl {
		[Browsable(false)]
		public override string EditorTypeName { get { return "ProgressBarControl"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressBarControlProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemProgressBar Properties { get { return base.Properties as RepositoryItemProgressBar; } }
		[Browsable(false), DefaultValue(0)]
		public override object EditValue {
			get { return base.EditValue; }
			set { base.EditValue = ConvertCheckValue(value); }
		}
		Timer _timer;
		bool showProgressInTaskBar;
		public ProgressBarControl() {
			this.showProgressInTaskBar = false;
		}
		protected override void OnParentVisibleChanged(EventArgs e) {
			base.OnParentVisibleChanged(e);
			if(ShowProgressInTaskBar && IsHandleCreated)
				BeginInvoke(new MethodInvoker(OnTaskBarProgressChanged));
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressBarControlShowProgressInTaskBar"),
#endif
 DefaultValue(false)]
		public bool ShowProgressInTaskBar {
			get { return showProgressInTaskBar; }
			set {
				if(ShowProgressInTaskBar == value) return;
				if(!IsSupportFeature) {
					if(!DesignMode)
						return;
					throw new ArgumentException("This property is supported in Windows Vista and more recent operating systems.");
				}
				showProgressInTaskBar = value;
				OnShowProgressInTaskBarChanged();
			}
		}
		static Version supportOS = new Version(6, 1);
		protected bool IsSupportFeature {
			get { return Environment.OSVersion.Version >= supportOS; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressBarControlPosition"),
#endif
 RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(0),
		Bindable(ControlConstants.NonObjectBindable), SmartTagProperty("Position", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual int Position {
			get { return Properties.ConvertValue(EditValue); }
			set { EditValue = Properties.CheckValue(value); }
		}
		protected virtual object ConvertCheckValue(object val) {
			if(val == null || val is DBNull) return val;
			int converted = Properties.ConvertValue(val);
			try {
				if(converted == Convert.ToInt32(val))
					return val;
			}
			catch { }
			return converted;
		}
		public virtual void PerformStep() {
			Position += Properties.Step;
		}
		public void Increment(int val) { Position += val; }
		public void Decrement(int val) { Position -= val; }
		protected internal virtual void OnMinMaxChanged() {
			if(IsLoading) return;
			Position = Properties.CheckValue(Position);
		}
		protected internal virtual void OnShowProgressInTaskBarChanged() {
			if(Parent == null || !Parent.IsHandleCreated) return;
			TaskbarHelper.SetProgressState(Parent.Handle, ShowProgressInTaskBar ? TaskbarButtonProgressMode.Normal : TaskbarButtonProgressMode.NoProgress);
			if(!ShowProgressInTaskBar) return;
			OnTaskBarProgressChanged();
		}
		protected internal virtual void OnTaskBarProgressChanged() {
			if(Position <= Properties.Minimum) {
				TaskbarHelper.SetProgressState(Parent.Handle, TaskbarButtonProgressMode.NoProgress);
				return;
			}
			TaskbarHelper.SetProgressState(Parent.Handle, TaskbarButtonProgressMode.Normal);
			TaskbarHelper.SetProgressValue(Parent.Handle, Position - Properties.Minimum, Properties.Maximum - Properties.Minimum);
			if(Position >= Properties.Maximum)
				TaskbarHelper.SetProgressState(Parent.Handle, TaskbarButtonProgressMode.NoProgress);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ProgressBarControlPositionChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler PositionChanged {
			add { Properties.PositionChanged += value; }
			remove { Properties.PositionChanged -= value; }
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			base.Dispose(disposing);
			if(disposing && _timer != null) {
				_timer.Tick -= new EventHandler(OnTimer);
				_timer.Dispose();
				_timer = null;
			}
		}
		protected System.Windows.Forms.Timer Timer {
			get {
				if(_timer == null) {
					_timer = new Timer();
					_timer.Tick += new EventHandler(OnTimer);
				}
				return _timer;
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			bool visible = Visible;
			if(!visible)
				ResetTimer();
			else
				SetTimer();
		}
		void ResetTimer() {
			if(_timer != null)
				_timer.Stop();
		}
		void SetTimer() {
			Timer.Stop();
			Timer.Interval = 25;
			if(Properties.FlowAnimationEnabled)
				Timer.Start();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			SetTimer();
		}
		void OnTimer(object sender, EventArgs e) {
			this.Invalidate();
			this.Update();
		}
		protected internal void ConfigureAnimationTimer() {
			if(Properties.FlowAnimationEnabled) SetTimer();
			else ResetTimer();
		}
		protected override Size CalcSizeableMaxSize() {
			if(Properties.ProgressKind == ProgressKind.Vertical) return new Size(18, 0);
			return new Size(0, 18);
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class ProgressBarBaseViewInfo : BaseEditViewInfo {
		ProgressBarObjectInfoArgs progressInfo;
		StringFormat strFormat;
		Rectangle progressBounds;
		public ProgressBarBaseViewInfo(RepositoryItem item) : base(item) {
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			ProgressBarBaseViewInfo be = info as ProgressBarBaseViewInfo;
			if(be == null) return;
			this.StrFormat = be.strFormat == null ? null : (StringFormat)be.strFormat.Clone();
			this.progressBounds = be.progressBounds;
		}
		public Rectangle ProgressBounds { get { return progressBounds; } }
		protected override void ReCalcViewInfoCore(Graphics g, MouseButtons buttons, Point mousePosition, Rectangle bounds) {
			base.ReCalcViewInfoCore(g, buttons, mousePosition, bounds);
			UpdateProgressInfo(ProgressInfo);
		}
		public override void Reset() {
			base.Reset();
			this.progressInfo = CreateInfoArgs();
		}
		public override AppearanceObject PaintAppearance {
			get { return base.PaintAppearance; }
			set {
				bool shouldUpdatePaintAppearance = false;
				if(!(value is ProgressBarAppearanceObject)) {
					ProgressBarAppearanceObject obj = new ProgressBarAppearanceObject();
					obj.BeginUpdate();
					obj.Assign(value);
					obj.EndUpdate();
					value = obj;
					shouldUpdatePaintAppearance = true;
				}
				base.PaintAppearance = value;
				if(shouldUpdatePaintAppearance)
					UpdateProgressPaintAppearance();
			}
		}
		public ProgressBarAppearanceObject ProgressAppearance { get { return PaintAppearance as ProgressBarAppearanceObject; } }
		public virtual ProgressBarObjectInfoArgs ProgressInfo { get { return progressInfo; } }
		public new RepositoryItemBaseProgressBar Item { get { return base.Item as RepositoryItemBaseProgressBar; } }
		protected override AppearanceObject CreatePaintAppearance() {
			return new ProgressBarAppearanceObject();
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault def = base.CreateDefaultAppearance();
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				def.ForeColor = EditorsSkins.GetSkin(LookAndFeel).Colors[EditorsSkins.SkinProgressBarEmptyTextColor];
			return def;
		}
		protected void UpdateProgressPaintAppearance() {
			ProgressBarAppearanceObject obj = (ProgressBarAppearanceObject)PaintAppearance;
			ProgressBarAppearanceObject itemApp = (ProgressBarAppearanceObject)Item.Appearance;
			ProgressBarAppearanceObject itemAppReadonly = (ProgressBarAppearanceObject)Item.AppearanceReadOnly;
			ProgressBarAppearanceObject itemAppFocused = (ProgressBarAppearanceObject)Item.AppearanceFocused;
			ProgressBarAppearanceObject itemAppDisabled = (ProgressBarAppearanceObject)Item.AppearanceDisabled;
			bool useDisabled = !Enabled && !Item.IsDesignMode;
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				obj.ForeColor2 = EditorsSkins.GetSkin(LookAndFeel).Colors[EditorsSkins.SkinProgressBarFilledTextColor];
				if(obj.ForeColor2 == Color.Empty && !(DevExpress.Utils.Paint.XPaint.Graphics is XPaintMixed))
					obj.ForeColor2 = obj.ForeColor;
			}
			else
				obj.ForeColor2 = SystemColors.HighlightText;
			if(useDisabled && itemAppDisabled.ProgressOptions.UseForeColor2)
				obj.ForeColor2 = itemAppDisabled.ForeColor2;
			else if(Item.ReadOnly && itemAppReadonly.ProgressOptions.UseForeColor2)
				obj.ForeColor2 = itemAppReadonly.ForeColor2;
			else if(Focused && itemAppFocused.ProgressOptions.UseForeColor2)
				obj.ForeColor2 = itemAppFocused.ForeColor2;
			else if(itemApp.ProgressOptions.UseForeColor2)
				obj.ForeColor2 = itemApp.ForeColor2;
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			UpdateProgressPaintAppearance();
			ProgressInfo.BackAppearance = PaintAppearance;
		}
		protected internal virtual void UpdateProgressInfo(ProgressBarObjectInfoArgs info) {
			info.RightToLeft = RightToLeft;
			info.Bounds = ProgressBounds;
			info.ProgressPadding = Item.ProgressPadding;
			info.SetAppearance(this.PaintAppearance);
			info.FillBackground = this.FillBackground;
			info.IsBroken = Item.ProgressViewStyle == ProgressViewStyle.Broken;
			info.IsVertical = Item.ProgressKind == ProgressKind.Vertical;
			info.StartColor = Item.StartColor;
			info.EndColor = Item.EndColor;
			info.TextOrientation = Item.TextOrientation;
			info.FlowAnimationEnabled = false;
			info.FlowAnimationSpeed = 180;
			info.FlowAnimationDelay = 1000;
		}
		protected override ObjectInfoArgs GetBorderArgs(Rectangle bounds) {
			ProgressBarObjectInfoArgs pi = new ProgressBarObjectInfoArgs(PaintAppearance);
			pi.Bounds = bounds;
			return pi;
		}
		protected override BorderPainter GetDefaultSkinBorderPainter(ISkinProvider provider) {
			return Item.ProgressKind == ProgressKind.Vertical ? SkinProgressBorderPainter.GetVert(provider) : SkinProgressBorderPainter.GetHorz(provider);
		}
		protected override BorderPainter GetBorderPainterCore() {
			BorderPainter bp = base.GetBorderPainterCore();
			if(ShouldForceDrawNativeBackground)
				return new WindowsXPProgressBarBorderPainter();
			if(bp is WindowsXPTextBorderPainter) bp = new WindowsXPProgressBarBorderPainter();
			return bp;
		}
		protected virtual bool ShouldForceDrawNativeBackground {
			get {
				return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP && InplaceType == InplaceType.Standalone;
			}
		}
		public virtual ProgressBarObjectPainter ProgressPainter {
			get {
				if(IsPrinting) return new ProgressBarObjectPainter();
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) return new SkinProgressBarObjectPainter(LookAndFeel.ActiveLookAndFeel);
				return LookAndFeel.Painter.ProgressBar;
			}
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			ProgressInfo.OffsetContent(x, y);
			this.progressBounds.Offset(x, y);
		}
		public override bool FillBackground {
			get {
				if(Item.GetForceFillBackground()) return base.FillBackground;
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && InplaceType != InplaceType.Grid
					&& BorderStyle == BorderStyles.Default)
					return false;
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP)
					return false;
				return true;
			}
			set { base.FillBackground = value; }
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			Rectangle r = ContentRect;
			if(Item.MaxHeight > 0 && Item.MaxHeight < ContentRect.Height) {
				r = RectangleHelper.GetCenterBounds(ContentRect, new Size(ContentRect.Width, Item.MaxHeight));
			}
			this.progressBounds = r;
			UpdateProgressInfo(ProgressInfo);
		}
		protected virtual ProgressBarObjectInfoArgs CreateInfoArgs() {
			return new ProgressBarObjectInfoArgs(PaintAppearance);
		}
		protected override Size CalcContentSize(Graphics g) {
			Size size = base.CalcContentSize(g);
			Size pSize = new Size(10, 2);
			GInfo.AddGraphics(g);
			try {
				ProgressBarObjectInfoArgs e = CreateInfoArgs();
				UpdateProgressInfo(e);
				e.Cache = GInfo.Cache;
				Size s = ProgressPainter.CalcObjectMinBounds(e).Size;
				pSize.Width = Math.Max(s.Width, pSize.Width);
				pSize.Height = Math.Max(s.Height, pSize.Height);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			size.Width = Math.Max(pSize.Width, size.Width);
			size.Height = Math.Max(pSize.Height, size.Height);
			if(Item.ShowTitle) return size;
			return pSize;
		}
		public virtual StringFormat StrFormat { 
			get { return strFormat; }
			set {
				if(StrFormat == value) return;
				if(strFormat != null) strFormat.Dispose();
				strFormat = value;
			}
		}
		protected override void CalcConstants() {
			StrFormat = PaintAppearance.GetStringFormat().Clone() as StringFormat;
			if(PaintAppearance.HAlignment == HorzAlignment.Default)
				this.strFormat.Alignment = StringAlignment.Center;
			base.CalcConstants();
			UpdateDisplayText();
		}
		protected virtual void UpdateDisplayText() {
			object val = EditValue == null ? 0 : EditValue;
			this.fDisplayText = Item.GetDisplayText(val);
		}
		public override bool AllowDrawFocusRect {
			get { return false; }
			set { base.AllowDrawFocusRect = value; }
		}
	}
	public class ProgressBarViewInfo : ProgressBarBaseViewInfo, IAnimatedItem {
		int position;
		float percents;
		public ProgressBarViewInfo(RepositoryItem item) : base(item) {
		}
		int IAnimatedItem.AnimationInterval { get { return 25; } }
		int[] IAnimatedItem.AnimationIntervals { get { return null; } }
		Rectangle IAnimatedItem.AnimationBounds { get { return ProgressBounds; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) { return 25; }
		bool IAnimatedItem.IsAnimated { get { return ShouldAnimateItem(); } }
		int IAnimatedItem.FramesCount { get { return Int32.MaxValue; } }
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) { }
		AnimationType IAnimatedItem.AnimationType { get { return AnimationType.Cycle; } }
		object IAnimatedItem.Owner { get { return OwnerEdit; } }
		void IAnimatedItem.OnStop() {
			if(ProgressInfo != null)
				ProgressInfo.FlowAnimationEnabled = ShouldAnimateItem();
		}
		void IAnimatedItem.OnStart() {
			if(ProgressInfo != null)
				ProgressInfo.FlowAnimationEnabled = ShouldAnimateItem();
		}
		protected virtual bool ShouldAnimateItem() {
			if((OwnerEdit != null && !OwnerEdit.Enabled) || !Item.FlowAnimationEnabled || Item.IsDesignMode) return false;
			if(Percents == 1.0 || InplaceType == InplaceType.Grid) return false;
			return true;
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			ProgressBarViewInfo be = info as ProgressBarViewInfo;
			if(be == null) return;
			this.percents = be.percents;
			this.position = be.position;
		}
		public override void Reset() {
			base.Reset();
			this.position = 0;
			this.percents = 0;
		}
		public override void Clear() {
			base.Clear();
		}
		public virtual int Position { get { return position; } }
		public virtual float Percents { get { return percents; } }
		public int GetPercents() {
			return Item.GetIntPercents(Percents);
		}
		protected override void UpdateDisplayText() {
			this.fDisplayText = Item.GetProgressDisplayText(Format, EditValue);
		}
		protected internal override void UpdateProgressInfo(ProgressBarObjectInfoArgs info) {
			base.UpdateProgressInfo(info);
			info.Percent = this.Percents;
			info.FlowAnimationEnabled = ShouldAnimateItem();
			info.FlowAnimationSpeed = Item.FlowAnimationSpeed;
			info.FlowAnimationDelay = Item.FlowAnimationDelay;
			info.InDesign = Item.IsDesignMode;
			info.SequenceSource = Item.SequenceSource;
		}
		public new RepositoryItemProgressBar Item { get { return base.Item as RepositoryItemProgressBar; } }
		public override object EditValue { 
			get { return base.EditValue; }
			set {
				this.fEditValue = value;
				OnEditValueChanged();
			}
		}
		protected override void OnEditValueChanged() {
			this.position = Item.ConvertValue(EditValue);
			this.percents = Item.CalcPercents(Position);
			UpdateDisplayText();
		}
	}
	public class MarqueeProgressBarViewInfo : ProgressBarBaseViewInfo, IAnimatedItem {
		public MarqueeProgressBarViewInfo(RepositoryItem item) : base(item) { }
		public new RepositoryItemMarqueeProgressBar Item { get { return base.Item as RepositoryItemMarqueeProgressBar; } }
		public override ProgressBarObjectPainter ProgressPainter {
			get {
				if(IsPrinting) return new MarqueeProgressBarObjectPainter();
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) return new SkinMarqueeProgressBarObjectPainter(LookAndFeel.ActiveLookAndFeel);
				return LookAndFeel.Painter.MarqueeProgressBar;
			}
		}
		int IAnimatedItem.AnimationInterval { get { return 25; } }
		int[] IAnimatedItem.AnimationIntervals { get { return null; } }
		Rectangle IAnimatedItem.AnimationBounds { get { return Bounds; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) { return 25; }
		bool IAnimatedItem.IsAnimated { get { return !Item.Stopped; } }
		int IAnimatedItem.FramesCount { get { return Int32.MaxValue; } }
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) { }
		AnimationType IAnimatedItem.AnimationType { get { return AnimationType.Cycle; } }
		object IAnimatedItem.Owner { get { return OwnerEdit; } }
		bool animated = false;
		void IAnimatedItem.OnStop() {
			animated = false;
			if(ProgressInfo != null) (ProgressInfo as MarqueeProgressBarObjectInfoArgs).Animated = false;
		}
		void IAnimatedItem.OnStart() {
			animated = true;
			if (ProgressInfo != null)
				(ProgressInfo as MarqueeProgressBarObjectInfoArgs).Animated = ShouldAnimateItem();
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			MarqueeProgressBarViewInfo vi = info as MarqueeProgressBarViewInfo;
			if(vi == null) return;
			this.animated = vi.animated;
		}
		protected override ProgressBarObjectInfoArgs CreateInfoArgs() {
			return new MarqueeProgressBarObjectInfoArgs(PaintAppearance);
		}
		protected virtual bool ShouldAnimateItem() {
			if(Item.Stopped) return false;
			return true;
		}
		protected internal override void UpdateProgressInfo(ProgressBarObjectInfoArgs info) {
			base.UpdateProgressInfo(info);
			MarqueeProgressBarObjectInfoArgs mInfo = info as MarqueeProgressBarObjectInfoArgs;
			if(mInfo == null) return;
			mInfo.InDesign = Item.IsDesignMode;
			mInfo.MarqueAnimationSpeed = Item.MarqueeAnimationSpeed;
			mInfo.AnimationMode = Item.ProgressAnimationMode;
			mInfo.Paused = Item.Paused;
			mInfo.MarqueeWidth = Item.MarqueeWidth;
			if(!ShouldAnimateItem() || (OwnerEdit == null && !animated)) mInfo.Animated = false;
			else mInfo.Animated = true;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class ProgressBarPainter : BaseEditPainter {
		protected override bool IsDrawBorderLast(ControlGraphicsInfoArgs info) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			if(vi.BorderPainter is WindowsXPProgressBarBorderPainter) return false;
			if(vi.ProgressPainter is SkinProgressBarObjectPainter) return false;
			return true;
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			if(vi.ProgressBounds != vi.ContentRect) DrawProgressBarBackground(info);
			ObjectPainter.DrawObject(info.Cache, vi.ProgressPainter, vi.ProgressInfo);
			if(vi.Item.ShowTitle && vi.DisplayText.Length > 0) DrawString(info, vi.ProgressInfo.TextOrientation);
		}
		void DrawProgressBarBackground(ControlGraphicsInfoArgs info) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			if(!vi.FillBackground) return;
			Rectangle r = vi.ContentRect;
			r.Height = vi.ProgressBounds.Y - vi.ContentRect.Y;
			vi.PaintAppearance.FillRectangle(info.Cache, r);
			r.Y = vi.ProgressBounds.Bottom;
			r.Height = vi.ContentRect.Bottom - r.Y;
			vi.PaintAppearance.FillRectangle(info.Cache, r);
		}
		private int GetAngleByProgressBarObjectInfoArgs(ProgressBarObjectInfoArgs info) {
			switch(info.TextOrientation) {
				case TextOrientation.Default:
					if(info.IsVertical) return 270;
					break;
				case TextOrientation.VerticalUpwards:
					return 270;
				case TextOrientation.VerticalDownwards:
					return 90;
			}
			return 0;
		}
		protected virtual void DrawString(ControlGraphicsInfoArgs info, TextOrientation orientation) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			var angle = GetAngleByProgressBarObjectInfoArgs(vi.ProgressInfo);
			Brush emptyBrush = vi.ProgressAppearance.GetForeBrush(info.Cache);
			Brush fillBrush = vi.ProgressAppearance.GetFore2Brush(info.Cache);
			RectangleF c = info.Cache.ClipInfo.Graphics.ClipBounds;
			Rectangle entireClip = new Rectangle((int)c.X, (int)c.Y, (int)c.Width, (int)c.Height);
			GraphicsClipState clip = info.Cache.ClipInfo.SaveAndSetClip(entireClip);
			try {
				if(vi.ProgressInfo.ProgressBounds.Height == 0) {
					info.Cache.DrawVString(vi.DisplayText, vi.PaintAppearance.Font, emptyBrush, vi.ContentRect, vi.StrFormat, angle);
					return;
				}
				Rectangle r = entireClip;
				r.Intersect(vi.ProgressInfo.ProgressBounds);
				info.Cache.ClipInfo.ExcludeClip(r);
				info.Cache.DrawVString(vi.DisplayText, vi.PaintAppearance.Font, emptyBrush, vi.ContentRect, vi.StrFormat, angle);
				if(vi.ProgressInfo.ProgressBounds.Width > 0) {
					info.Cache.ClipInfo.SetClip(r);
					info.Cache.DrawVString(vi.DisplayText, vi.PaintAppearance.Font, fillBrush, vi.ContentRect, vi.StrFormat, angle);
				}
			}
			finally {
				info.Cache.ClipInfo.RestoreClipRelease(clip);
			}
		}
		protected override void DrawAdornments(ControlGraphicsInfoArgs info) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			if(vi.FillBackground) {
				base.DrawAdornments(info);
				return;
			}
			else {
				if(vi.ErrorIconBounds.IsEmpty) return;
				if(vi.OwnerEdit != null) vi.OwnerEdit.PaintErrorIconBackground(info.Graphics, vi.ErrorIconBounds);
				DrawErrorIcon(info);
			}
		}
		protected virtual void DrawBackground(ControlGraphicsInfoArgs info) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			Rectangle saved = vi.ProgressInfo.Bounds;
			try {
				vi.ProgressInfo.Cache = info.Cache;
				vi.ProgressInfo.Bounds = info.Bounds;
				vi.ProgressPainter.DrawBackground(vi.ProgressInfo);
			}
			finally {
				vi.ProgressInfo.Cache = null;
				vi.ProgressInfo.Bounds = saved;
			}
		}
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
			ProgressBarBaseViewInfo vi = info.ViewInfo as ProgressBarBaseViewInfo;
			Rectangle saved = vi.ProgressInfo.Bounds;
			if(!IsDrawBorderLast(info))
				DrawBackground(info);
			if(vi.BorderPainter is WindowsXPProgressBarBorderPainter) {
				try {
					vi.ProgressInfo.Cache = info.Cache;
					vi.ProgressInfo.Bounds = info.Bounds;
					vi.BorderPainter.DrawObject(vi.ProgressInfo);
				}
				finally {
					vi.ProgressInfo.Cache = null;
					vi.ProgressInfo.Bounds = saved;
				}
			} else {
				base.DrawBorder(info);
			}
		}
	}
	public class SkinProgressBarObjectPainter : ProgressBarObjectPainter {
		ISkinProvider provider;
		public SkinProgressBarObjectPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		protected override int GetChunkSize(ProgressBarObjectInfoArgs e) {
			SkinElement skinElement = EditorsSkins.GetSkin(this.provider)[e.IsVertical ? EditorsSkins.SkinProgressChunkVert : EditorsSkins.SkinProgressChunk];
			if(skinElement.Image == null || skinElement.Image.Image == null)
				return base.GetChunkSize(e);
			if(skinElement.Image.Stretch == SkinImageStretch.Stretch)
				return 0;
			Rectangle bounds = skinElement.Image.GetImageBounds(0);
			return e.IsVertical ? bounds.Height : bounds.Width;
		}
		protected override void DrawBroken(ProgressBarObjectInfoArgs e, Rectangle pb) {
			DrawSkinnedBar(e, pb);
		}
		protected override void DrawSolid(ProgressBarObjectInfoArgs e, Rectangle pb) {
			DrawSkinnedBar(e, pb);
		}
		protected virtual void DrawSkinnedBar(ProgressBarObjectInfoArgs e, Rectangle rect) {
			Rectangle r = CalcProgressBounds(e);
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(this.provider)[e.IsVertical ? EditorsSkins.SkinProgressChunkVert : EditorsSkins.SkinProgressChunk], r);
			info.BackAppearance = e.BackAppearance;
			info.Bounds = rect;
			e.ProgressBounds = r;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override void DrawBackground(ProgressBarObjectInfoArgs ee) {
			if(ee.FillBackground) ee.BackAppearance.DrawBackground(ee.Cache, ee.Bounds);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ProgressBarObjectInfoArgs ee = e as ProgressBarObjectInfoArgs;
			DrawBar(ee);
		}
		protected override bool AllowBroken { get { return false; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			ProgressBarObjectInfoArgs ee = e as ProgressBarObjectInfoArgs;
			SkinElement chunk = EditorsSkins.GetSkin(this.provider)[ee.IsVertical ? EditorsSkins.SkinProgressChunkVert : EditorsSkins.SkinProgressChunk];
			return new Rectangle(Point.Empty, chunk.Size.MinSize);
		}
		protected override void DrawAnimation(ProgressBarObjectInfoArgs e, Rectangle pb) {
			SkinElement skinElement = EditorsSkins.GetSkin(this.provider)[e.IsVertical ? EditorsSkins.SkinProgressFlowIndicatorVert : EditorsSkins.SkinProgressFlowIndicator];
			if(skinElement == null)
				return;
			Rectangle r = CalcAnimation(e, pb);
			SkinElementInfo info = new SkinElementInfo(skinElement, r);
			info.BackAppearance = e.BackAppearance;
			info.Bounds = r;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected override int CalcFlowIndicatorLength(ProgressBarObjectInfoArgs e, Rectangle pb) {
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(this.provider)[e.IsVertical ? EditorsSkins.SkinProgressFlowIndicatorVert : EditorsSkins.SkinProgressFlowIndicator], Rectangle.Empty);
			if(e.IsVertical) return info.Element.Size.MinSize.Height;
			else return info.Element.Size.MinSize.Width;
		}
	}
	public class SkinMarqueeProgressBarObjectPainter : SkinProgressBarObjectPainter {
		public SkinMarqueeProgressBarObjectPainter(ISkinProvider provider) : base(provider){}
		protected override Rectangle CalcProgressBounds(ProgressBarObjectInfoArgs e) {
			MarqueeProgressBarObjectInfoArgs me = e as MarqueeProgressBarObjectInfoArgs;
			if(me == null) return Rectangle.Empty;
			return CalcMarqueProgressBounds(me);
		}
	}
	public class SkinProgressBorderPainter : SkinBorderPainter {
		public static SkinProgressBorderPainter GetHorz(ISkinProvider provider) {
			return new SkinProgressBorderPainter(provider, true);
		}
		public static SkinProgressBorderPainter GetVert(ISkinProvider provider) {
			return new SkinProgressBorderPainter(provider, false);
		}
		bool horz;
		public SkinProgressBorderPainter(ISkinProvider provider, bool horz) : base(provider) {
			this.horz = horz;
		}
		protected bool IsHorz { get { return horz; } }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(EditorsSkins.GetSkin(Provider)[IsHorz ? EditorsSkins.SkinProgressBorder : EditorsSkins.SkinProgressBorderVert]);
		}
	}
} 
