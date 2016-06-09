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

using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Native {
	public class RangeControlPaintArgsWrapper : IRangeControlPaint {
		RangeControlPaintEventArgs args;
		public RangeControlPaintArgsWrapper(RangeControlPaintEventArgs args) {
			this.args = args;
		}
		#region IRangeControlPaint Members
		int IRangeControlPaint.CalcX(double value) {
			return args.CalcX(value);
		}
		Rectangle IRangeControlPaint.ContentBounds {
			get {
				return args.ContentBounds;
			}
		}
		Graphics IRangeControlPaint.Graphics {
			get {
				return args.Graphics;
			}
		}
		#endregion
	}
	public class RangeControlClient : IRangeControlClientExtension {
		readonly ISupportRangeControl actualDelegate;
		ISupportRangeControl ActualDelegate {
			get {
				return actualDelegate;
			}
		}
		public event ClientRangeChangedEventHandler RangeControlRangeChanged;
		public RangeControlClient(ISupportRangeControl actualDelegate) {
			this.actualDelegate = actualDelegate;
		}
		public void RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) {
			RangeControlClientRangeEventArgs args = new RangeControlClientRangeEventArgs();
			args.Range = new RangeControlRange(minValue, maxValue);
			args.InvalidateContent = invalidate;
			if (RangeControlRangeChanged != null)
				RangeControlRangeChanged(this, args);
		}
		#region IRangeControlClient implementation
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add {
				RangeControlRangeChanged += value;
			}
			remove {
				RangeControlRangeChanged -= value;
			}
		}
		bool IRangeControlClient.IsValidType(Type type) {
			return ActualDelegate.CheckTypeSupport(type);
		}
		bool IRangeControlClient.IsValid {
			get {
				return ActualDelegate.IsValid;
			}
		}
		string IRangeControlClient.InvalidText {
			get {
				return ActualDelegate.InvalidText;
			}
		}
		int IRangeControlClient.RangeBoxTopIndent {
			get {
				return ActualDelegate.TopIndent;
			}
		}
		int IRangeControlClient.RangeBoxBottomIndent {
			get {
				return ActualDelegate.BottomIndent;
			}
		}
		bool IRangeControlClient.IsCustomRuler {
			get {
				return false;
			}
		}
		object IRangeControlClient.RulerDelta {
			get {
				return 0;
			}
		}
		double IRangeControlClient.NormalizedRulerDelta {
			get {
				return 1;
			}
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) {
			return true;
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return ActualDelegate.ProjectBackValue(normalizedValue);
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return ActualDelegate.ProjectValue(value);
		}
		string IRangeControlClient.RulerToString(int ruleIndex) {
			return ActualDelegate.RulerToString(ruleIndex);
		}
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) {
			return ActualDelegate.GetValuesInRange(e.MinValue, e.MaxValue, (int)e.RulerWidthInPixels);
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			RangeValidationBase validationBase = RangeValidationBase.Maximum;
			switch (info.ChangedBound) {
				case ChangedBoundType.Maximum:
					validationBase = RangeValidationBase.Maximum;
					break;
				case ChangedBoundType.Minimum:
					validationBase = RangeValidationBase.Minimum;
					break;
				case ChangedBoundType.Both:
					validationBase = RangeValidationBase.Average;
					break;
			}
			NormalizedRange range = new NormalizedRange(info.Range.Minimum, info.Range.Maximum);
			NormalizedRange validRange = ActualDelegate.ValidateNormalRange(range, validationBase);
			UpdateRangeInfo(info, validRange);
		}
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			ActualDelegate.RangeChanged(rangeMinimum, rangeMaximum);
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			ActualDelegate.DrawContent(new RangeControlPaintArgsWrapper(e) as IRangeControlPaint);
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return false;
		}
		object IRangeControlClient.GetOptions() {
			return ActualDelegate.GetOptions();
		}
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) { }
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) { }
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) { }
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			ActualDelegate.OnRangeControlChanged(rangeControl);
		}
		void IRangeControlClient.OnResize() { }
		void IRangeControlClient.Calculate(Rectangle contentRect) { }
		double IRangeControlClient.ValidateScale(double newScale) { return newScale; }
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return ActualDelegate.ValueToString(normalizedValue);
		}
		#endregion
		#region IRangeControlClientExtension
		object IRangeControlClientExtension.NativeValue(double normalizedValue) {
			return ActualDelegate.NativeValue(normalizedValue);
		}
		void UpdateRangeInfo(NormalizedRangeInfo info, NormalizedRange range) {
			info.Range.Minimum = range.Minimum;
			info.Range.Maximum = range.Maximum;
		}
		#endregion
	}
	public class SimpleChartContainer : IChartContainer, IRangeControlClientExtension {
		readonly Chart chart;
		readonly RangeControlClient rangeControlClient;
		IRangeControlClientExtension RangeControlClient { get { return rangeControlClient; } }
		public IChartDataProvider DataProvider {
			get {
				return null;
			}
		}
		public IChartRenderProvider RenderProvider {
			get {
				return null;
			}
		}
		public IChartEventsProvider EventsProvider {
			get {
				return null;
			}
		}
		public IChartInteractionProvider InteractionProvider {
			get {
				return null;
			}
		}
		public Chart Chart {
			get {
				return chart;
			}
		}
		public ChartContainerType ControlType {
			get {
				return ChartContainerType.WinControl;
			}
		}
		public bool DesignMode {
			get {
				return false;
			}
		}
		public bool Loading {
			get {
				return false;
			}
		}
		public bool ShowDesignerHints {
			get {
				return false;
			}
		}
		public bool IsDesignControl {
			get {
				return false;
			}
		}
		public bool IsEndUserDesigner {
			get {
				return false;
			}
		}
		public bool ShouldEnableFormsSkins {
			get {
				return false;
			}
		}
		public IServiceProvider ServiceProvider {
			get {
				return null;
			}
		}
		public ISite Site { get; set; }
		public IComponent Parent {
			get {
				return null;
			}
		}
		public CommandBasedKeyboardHandler<ChartCommandId> KeyboardHandler {
			get {
				return null;
			}
		}
		event EventHandler UpdateUI;
		public SimpleChartContainer(Chart chart) {
			this.chart = chart;
			this.rangeControlClient = new RangeControlClient(chart);
		}
		#region IChartContainer
		event EventHandler IChartContainer.EndLoading { add { } remove { } }
		bool IChartContainer.GetActualRightToLeft() { return false; }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		#endregion
		#region ICommandAwareControl
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { UpdateUI += value; } remove { UpdateUI -= value; } }
		#endregion
		#region IRangeControlClient
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add {
				RangeControlClient.RangeChanged += value;
			}
			remove {
				RangeControlClient.RangeChanged -= value;
			}
		}
		bool IRangeControlClient.IsValidType(Type type) {
			return RangeControlClient.IsValidType(type);
		}
		bool IRangeControlClient.IsValid {
			get {
				return RangeControlClient.IsValid;
			}
		}
		string IRangeControlClient.InvalidText {
			get {
				return RangeControlClient.InvalidText;
			}
		}
		int IRangeControlClient.RangeBoxTopIndent {
			get {
				return RangeControlClient.RangeBoxTopIndent;
			}
		}
		int IRangeControlClient.RangeBoxBottomIndent {
			get {
				return RangeControlClient.RangeBoxBottomIndent;
			}
		}
		bool IRangeControlClient.IsCustomRuler {
			get {
				return RangeControlClient.IsCustomRuler;
			}
		}
		object IRangeControlClient.RulerDelta {
			get {
				return RangeControlClient.RulerDelta;
			}
		}
		double IRangeControlClient.NormalizedRulerDelta {
			get {
				return RangeControlClient.NormalizedRulerDelta;
			}
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) {
			return RangeControlClient.SupportOrientation(orientation);
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return RangeControlClient.GetValue(normalizedValue);
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return RangeControlClient.GetNormalizedValue(value);
		}
		string IRangeControlClient.RulerToString(int ruleIndex) {
			return RangeControlClient.RulerToString(ruleIndex);
		}
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) {
			return RangeControlClient.GetRuler(e);
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			RangeControlClient.ValidateRange(info);
		}
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			RangeControlClient.OnRangeChanged(rangeMinimum, rangeMaximum);
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			RangeControlClient.DrawContent(e);
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return RangeControlClient.DrawRuler(e);
		}
		object IRangeControlClient.GetOptions() {
			return RangeControlClient.GetOptions();
		}
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) {
			RangeControlClient.UpdateHotInfo(hitInfo);
		}
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			RangeControlClient.UpdatePressedInfo(hitInfo);
		}
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) {
			RangeControlClient.OnClick(hitInfo);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			RangeControlClient.OnRangeControlChanged(rangeControl);
		}
		void IRangeControlClient.OnResize() {
			RangeControlClient.OnResize();
		}
		void IRangeControlClient.Calculate(Rectangle contentRect) {
			RangeControlClient.Calculate(contentRect);
		}
		double IRangeControlClient.ValidateScale(double newScale) {
			return RangeControlClient.ValidateScale(newScale);
		}
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return RangeControlClient.ValueToString(normalizedValue);
		}
		#endregion
		#region IRangeControlClientExtension
		object IRangeControlClientExtension.NativeValue(double normalizedValue) {
			return RangeControlClient.NativeValue(normalizedValue);
		}
		#endregion
		public void Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		public void Changing() {
			throw new NotImplementedException();
		}
		public void Changed() {
			throw new NotImplementedException();
		}
		public void LockChangeService() {
			throw new NotImplementedException();
		}
		public void UnlockChangeService() {
			throw new NotImplementedException();
		}
		public void ShowErrorMessage(string message, string title) {
			throw new NotImplementedException();
		}
		public void RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) {
			rangeControlClient.RaiseRangeControlRangeChanged(minValue, maxValue, invalidate);
		}
		public void RaiseUIUpdated() {
			if (UpdateUI != null)
				UpdateUI(this, new EventArgs());
		}
		public object GetService(Type serviceType) {
			return null;
		}
		public void CommitImeContent() { }
		public Command CreateCommand(ChartCommandId id) {
			return null;
		}
		public void Focus() { }
		public bool HandleException(Exception e) { return false; }
	}
}
