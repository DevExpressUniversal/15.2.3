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
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraPrinting;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native.Summary;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Native.Data;
namespace DevExpress.XtraReports.UI {
	[TypeConverter(typeof(DevExpress.XtraReports.Design.XRSummaryConverter))]
	public class XRSummary : IXRSerializable, IDisposable {
		#region inner classes
		internal static class EventNames {
			public const string RowChanged = "RowChanged";
			public const string GetResult = "GetResult";
			public const string Reset = "Reset";
		}
		internal class EventMethods {
			void RowChanged() {
			}
			object GetResult(ArrayList accumulatedValues) {
				return null;
			}
			void Reset() {
			}
		}
		#endregion
		const SummaryFunc defaultSummaryFunc = SummaryFunc.Sum;
		const bool defaultIgnoreNullValues = false;
		#region fields
		List<VisualBrick> bricks = new List<VisualBrick>();
		SummaryRunning running;
		ISummaryRunner fSummaryRunner;
		string formatString = String.Empty;
		XRLabel label;
		XRSummaryScripts funcScripts = new XRSummaryScripts();
		SummaryFunc func = defaultSummaryFunc;
		bool ignoreNullValues;
		ValuesRowsContainer valuesInfo = new ValuesRowsContainer();
		XtraReportBase[] appropriateReports;
		#endregion
		public XRSummary()
			: this(SummaryRunning.None) {
		}
		public XRSummary(SummaryRunning running, SummaryFunc func, string formatString)
			: base() {
			this.running = running;
			this.func = func;
			this.formatString = formatString;
		}
		public XRSummary(SummaryRunning running)
			: base() {
			this.running = running;
		}
		public XRSummary Clone() {
			XRSummary clone = new XRSummary(running, func, formatString);
			clone.label = label;
			clone.ignoreNullValues = ignoreNullValues;
			return clone;
		}
		#region properties
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRSummaryFormatString"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummary.FormatString"),
		Editor("DevExpress.XtraReports.Design.FormatStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(""),
		NotifyParentProperty(true),
		Localizable(true),
		XtraSerializableProperty
		]
		public string FormatString {
			get { return formatString; }
			set {
				StringHelper.ValidateFormatString(value);
				formatString = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRSummaryRunning"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummary.Running"),
		DefaultValue(SummaryRunning.None),
		RefreshProperties(RefreshProperties.Repaint),
		NotifyParentProperty(true),
		XtraSerializableProperty
		]
		public SummaryRunning Running {
			get { return running; }
			set { running = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummary.Func"),
		RefreshProperties(RefreshProperties.All),
		DefaultValue(defaultSummaryFunc),
		NotifyParentProperty(true),
		XtraSerializableProperty
		]
		public SummaryFunc Func {
			get { return func; }
			set { func = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSummary.IgnoreNullValues"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(defaultIgnoreNullValues),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool IgnoreNullValues {
			get { return ignoreNullValues; }
			set { ignoreNullValues = value; }
		}
		internal IList<VisualBrick> Bricks {
			get {
				return bricks;
			}
		}
		internal IList Values {
			get {
				return ValuesInfo.Values;
			}
		}
		ISummaryRunner SummaryRunner {
			get {
				if(fSummaryRunner == null) {
					switch (running) {
						case SummaryRunning.Group:
							if(Func == SummaryFunc.RunningSum)
								fSummaryRunner = new GroupSummaryRunner(this, new RunningSummaryUpdater(this));
							else if(Func == SummaryFunc.Percentage)
								fSummaryRunner = new GroupSummaryRunner(this, new SimplePercentageUpdater(this));
							else
								fSummaryRunner = new GroupSummaryRunner(this, new SummaryUpdater(this));
							break;
						case SummaryRunning.Report:
							if(Func == SummaryFunc.Percentage)
								fSummaryRunner = new ReportPercentageSummaryRunner(this, new ReportPercentageUpdater(this));
							else if(Func == SummaryFunc.RunningSum && (Control.Band is PageBand || Control.Band is MarginBand))
								fSummaryRunner = new ReportRunningPageSummaryRunner(this, new RunningSummaryUpdater(this));
							else if(Func == SummaryFunc.RunningSum)
								fSummaryRunner = new ReportRunningSummaryRunner(this, new RunningSummaryUpdater(this));
							else
								fSummaryRunner = new ReportSummaryRunner<SummaryUpdater>(this, new SummaryUpdater(this));
							break;
						case SummaryRunning.Page:
							if(Func == SummaryFunc.RunningSum && Control.Band is GroupBand)
								fSummaryRunner = new PageRunningGroupSummaryRunner(this, new RunningSummaryUpdater(this));
							else if(Func == SummaryFunc.RunningSum)
								fSummaryRunner = new PageRunningSummaryRunner(this, new RunningSummaryUpdater(this));
							else if (Func == SummaryFunc.Percentage && Control.Band is GroupBand)
								fSummaryRunner = new PagePercentageGroupRunner(this, new PercentageUpdater(this));
							else if(Func == SummaryFunc.Percentage)
								fSummaryRunner = new PageSummaryRunner<SummaryUpdater>(this, new SimplePercentageUpdater(this));
							else
								fSummaryRunner = new PageSummaryRunner<SummaryUpdater>(this, new SummaryUpdater(this));
							break;
						default:
							fSummaryRunner = new SummaryRunnerBase<SummaryUpdater>(this, new SummaryUpdater(this));
							break;
					}
				}
				return fSummaryRunner;
			}
		}
		protected internal XRSummaryScripts XRSummaryScripts {
			get { return funcScripts; }
		}
		internal XRLabel Control { get { return label; } set { label = value; } }
		internal ValuesRowsContainer ValuesInfo { get { return valuesInfo; } }
		#endregion
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			serializer.SerializeString("FormatString", formatString, String.Empty);
			serializer.SerializeEnum("Func", Func);
			serializer.SerializeEnum("Running", running);
			serializer.SerializeBoolean("IgnoreNullValues", IgnoreNullValues);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			formatString = serializer.DeserializeString("FormatString", String.Empty);
			Func = (SummaryFunc)serializer.DeserializeEnum("Func", typeof(SummaryFunc), SummaryFunc.Sum);
			Running = (SummaryRunning)serializer.DeserializeEnum("Running", typeof(SummaryRunning), SummaryRunning.None);
			IgnoreNullValues = serializer.DeserializeBoolean("IgnoreNullValues", defaultIgnoreNullValues);
			serializer.Deserialize("Scripts", funcScripts);
		}
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		#endregion
		public object GetResult() {
			return GetResultCore(this.Values);
		}
		object GetResultCore(IList values) {
			bool handled = false;
			object result = FireSummaryGetResult(ref handled);
			return handled ? result : SummaryHelper.CalcResult(Func, values);
		}
		internal void ResetRunningValues() {
			if(fSummaryRunner != null) {
				fSummaryRunner.Reset();
				fSummaryRunner = null;
			}
		}
		internal void Reset() {
			ValuesInfo.Clear();
			bricks.Clear();
			label.OnSummaryReset(EventArgs.Empty);
		}
		internal void AddSummaryBrickOnReport(VisualBrick brick) {
			SummaryRunner.AddSummaryBrickOnReport(brick);
		}
		internal void AddSummaryBrickOnPage(VisualBrick brick) {
			SummaryRunner.AddSummaryBrickOnPage(brick);
		}
		internal void OnDataRowChangedOnReport(XtraReportBase report) {
			SummaryRunner.OnDataRowChangedOnReport(report);
		}
		internal void OnDataRowChangedOnPage(XtraReportBase report) {
			SummaryRunner.OnDataRowChangedOnPage(report);
		}
		internal void OnGroupUpdate() {
			SummaryRunner.OnGroupUpdate();
		}
		internal void OnGroupFinished() {
			SummaryRunner.OnGroupFinished();
		}
		internal void OnGroupBegin() {
			SummaryRunner.OnGroupBegin();
		}
		internal void OnReportUpdate() {
			SummaryRunner.OnReportUpdate();
		}
		internal void OnReportFinished() {
			SummaryRunner.OnReportFinished();
		}
		internal void OnPageUpdate() {
			SummaryRunner.OnPageUpdate();
		}
		internal void OnPageFinished() {
			SummaryRunner.OnPageFinished();
		}
		internal void OnGroupFinishedOnPage() {
			SummaryRunner.OnGroupFinishedOnPage();
		}
		internal string GetDisplayText(string columnName) {
			string result = String.Format("{0}( [{1}] )", Func, columnName);
			return String.IsNullOrEmpty(FormatString) ?
				result : String.Format(FormatString, result);
		}
		internal void AddSummaryBrickCore(VisualBrick brick) {
			if(brick != null) bricks.Add(brick);
		}
		internal bool TryGetBinding(XtraReportBase report, out XRBinding binding) {
			binding = label != null ? label.DataBindings["Text"] : null;
			if(binding == null)
				return !Func.NeedsCalculation() && this.IsAppropriateReport(report);
			return IsAppropriateBinding(report, binding);
		}
		internal object GetNativeValue(IList values) {
			XRBinding binding = label != null ? label.DataBindings["Text"] : null;
			if((binding != null && this.Running.IsAppropriateBand(label.Band)) || !Func.NeedsCalculation()) {
				return GetResultCore(values);
			} else {
				bool handled = false;
				object result = FireSummaryGetResult(ref handled);
				object nativeResult = handled ? result : null;
				return nativeResult == null ? "None" : nativeResult;
			}
		}
		internal void AddValue(object value, int rowIndex) {
			ValuesInfo.Add(value, rowIndex);
		}
		object FireSummaryGetResult(ref bool handled) {
			SummaryGetResultEventArgs e = new SummaryGetResultEventArgs(Values);
			label.OnSummaryGetResult(e);
			handled = e.Handled;
			return e.Result;
		}
		internal void OnGroupBeginOnPage() {
			if(Func == SummaryFunc.RunningSum && Running == SummaryRunning.Group)
				Reset();
		}
		public void Dispose() {
			this.Control = null;
			this.funcScripts.Dispose();
		}
		internal bool IsAppropriateBinding(XtraReportBase report, XRBinding binding) {
			if(appropriateReports == null || appropriateReports.Length == 0)
				appropriateReports = binding.GetAppropriateReports();
			return appropriateReports != null && Array.IndexOf(appropriateReports, report) >= 0;
		}
		internal void ResetReportCache() {
			appropriateReports = null;
		}
	}
}
