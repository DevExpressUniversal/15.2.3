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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(ScaleBreakTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ScaleBreak : ChartElementNamed, IScaleBreak, IScaleDiapason {
		const bool DefautlVisible = true;
		bool visible = DefautlVisible;
		ScaleBreakEdge scaleBreakEdge1;
		ScaleBreakEdge scaleBreakEdge2;
		internal Axis Axis { get { return (Axis)base.Owner; } }
		internal ScaleBreakEdge ScaleBreakEdge1 { get { return scaleBreakEdge1; } }
		internal ScaleBreakEdge ScaleBreakEdge2 { get { return scaleBreakEdge2; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleBreakEdge1"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleBreak.Edge1"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(ScaleBreakEdgeTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object Edge1 {
			get { return scaleBreakEdge1.AxisValue; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidEdge1));
				SendNotification(new ElementWillChangeNotification(this));
				AxisElementUpdateInfo updateInfo = new AxisElementUpdateInfo(this, Axis);
				scaleBreakEdge1.AxisValue = ConvertEdge(value);
				RaiseControlChanged(updateInfo);
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleBreakEdge2"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleBreak.Edge2"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(ScaleBreakEdgeTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object Edge2 {
			get { return scaleBreakEdge2.AxisValue; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidEdge2));
				SendNotification(new ElementWillChangeNotification(this));
				AxisElementUpdateInfo updateInfo = new AxisElementUpdateInfo(this, Axis);
				scaleBreakEdge2.AxisValue = ConvertEdge(value);
				RaiseControlChanged(updateInfo);
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string Edge1Serializable {
			get { return SerializingUtils.ConvertToSerializable(Edge1); }
			set {
				Edge1 = value;
				scaleBreakEdge1.AxisValueSerializable = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string Edge2Serializable {
			get { return SerializingUtils.ConvertToSerializable(Edge2); }
			set {
				Edge2 = value;
				scaleBreakEdge2.AxisValueSerializable = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleBreakVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleBreak.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis));
					RaiseControlChanged();
				}
			}
		}
		#region IScaleBreak implementation
		IAxisValueContainer IScaleBreak.Edge1 { get { return scaleBreakEdge1; } }
		IAxisValueContainer IScaleBreak.Edge2 { get { return scaleBreakEdge2; } }
		bool IScaleBreak.Visible { get { return visible; } }
		#endregion
		#region IScaleDiapason implementation
		double IScaleDiapason.Edge1 { get { return scaleBreakEdge1.Value; } }
		double IScaleDiapason.Edge2 { get { return scaleBreakEdge2.Value; } }
		#endregion
		ScaleBreak(string name, object edge1, object edge2)
			: base(name) {
			scaleBreakEdge1 = new ScaleBreakEdge(this, edge1);
			scaleBreakEdge2 = new ScaleBreakEdge(this, edge2);
		}
		public ScaleBreak() : this(string.Empty) { }
		public ScaleBreak(string name) : this(name, null, null) {
		}
		public ScaleBreak(string name, string edge1, string edge2) : this(name, (object)edge1, (object)edge2) {
		}
		public ScaleBreak(string name, double edge1, double edge2) : this(name, (object)edge1, (object)edge2) {
		}
		public ScaleBreak(string name, DateTime edge1, DateTime edge2) : this(name, (object)edge1, (object)edge2) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Visible" ? ShouldSerializeVisible() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return visible != DefautlVisible;
		}
		void ResetVisible() {
			Visible = DefautlVisible;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeVisible();
		}
		#endregion
		object ConvertEdge(object edge) {
			return (Axis == null || Loading) ? edge : Axis.ConvertBasedOnScaleType(edge);
		}
		protected override ChartElement CreateObjectForClone() {
			return new ScaleBreak();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ScaleBreak scaleBreak = obj as ScaleBreak;
			if (scaleBreak != null) {
				scaleBreakEdge1.Assign(scaleBreak.scaleBreakEdge1);
				scaleBreakEdge2.Assign(scaleBreak.scaleBreakEdge2);
				visible = scaleBreak.visible;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ScaleBreakCollection : ChartElementNamedCollection, IEnumerable<IScaleBreak> {
		Axis Axis { get { return (Axis)base.Owner; } }
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.ScaleBreakPrefix); } }
		public ScaleBreak this[int index] { get { return (ScaleBreak)List[index]; } }
		internal ScaleBreakCollection(Axis axis) : base(axis) { }
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<IScaleBreak>)this).GetEnumerator();
		}
		IEnumerator<IScaleBreak> IEnumerable<IScaleBreak>.GetEnumerator() {
			foreach (IScaleBreak scaleBreak in this)
				yield return scaleBreak;
		}
		void UpdateAxisValue(ScaleBreak scaleBreak) {
			if (Axis != null)
				AxisValueContainerHelper.UpdateAxisValue(scaleBreak.ScaleBreakEdge1, scaleBreak.ScaleBreakEdge2, Axis.ScaleTypeMap);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		public int Add(ScaleBreak scaleBreak) {
			UpdateAxisValue(scaleBreak);
			return base.Add(scaleBreak);
		}
		public void AddRange(ScaleBreak[] coll) {
			foreach (ScaleBreak scaleBreak in coll)
				UpdateAxisValue(scaleBreak);
			base.AddRange(coll);
		}
		public void Remove(ScaleBreak scaleBreak) {
			base.Remove(scaleBreak);
		}
		public void Insert(int index, ScaleBreak scaleBreak) {
			UpdateAxisValue(scaleBreak);
			base.Insert(index, scaleBreak);
		}
		public bool Contains(ScaleBreak scaleBreak) {
			return base.Contains(scaleBreak);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AutoScaleBreaks : ChartElement {
		const double ExtensionFactor = 20;
		const int DefaultMaxCount = 5;
		const bool DefaultEnabled = false;
		bool enabled = DefaultEnabled;
		int maxCount = DefaultMaxCount;
		List<AutomaticScaleBreak> scaleBreaks = new List<AutomaticScaleBreak>();
		DefaultAutomaticScaleBreaksCalculator defaultCalculator;
		IAutomaticScaleBreaksCalculator automaticScaleBreaksCalculator;
		internal List<AutomaticScaleBreak> ScaleBreaks { get { return scaleBreaks; } }
		internal IAutomaticScaleBreaksCalculator ActualAutomaticScaleBreaksCalculator {
			get {
				if (automaticScaleBreaksCalculator != null)
					return automaticScaleBreaksCalculator;
				if (defaultCalculator == null)
					defaultCalculator = new DefaultAutomaticScaleBreaksCalculator();
				return defaultCalculator;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AutoScaleBreaksEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AutoScaleBreaks.Enabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool Enabled {
			get { return enabled; }
			set {
				if (value != enabled) {
					SendNotification(new ElementWillChangeNotification(this));
					SetEnabled(value);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AutoScaleBreaksMaxCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AutoScaleBreaks.MaxCount"),
		XtraSerializableProperty
		]
		public int MaxCount {
			get { return maxCount; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidMaxCount));
				if (value != maxCount) {
					SendNotification(new ElementWillChangeNotification(this));
					maxCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AutoScaleBreaksAutomaticScaleBreaksCalculator"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Browsable(false),
		]
		public IAutomaticScaleBreaksCalculator AutomaticScaleBreaksCalculator {
			get { return automaticScaleBreaksCalculator; }
			set {
				if (value != automaticScaleBreaksCalculator) {
					SendNotification(new ElementWillChangeNotification(this));
					automaticScaleBreaksCalculator = value;
					RaiseControlChanged();
				}
			}
		}
		internal AutoScaleBreaks(Axis axis) : base(axis) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Enabled")
				return ShouldSerializeEnabled();
			if (propertyName == "MaxCount")
				return ShouldSerializeMaxCount();
			if (propertyName == "AutomaticScaleBreaksCalculator")
				return false;
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeEnabled() {
			return enabled != DefaultEnabled;
		}
		void ResetEnabled() {
			Enabled = DefaultEnabled;
		}
		bool ShouldSerializeMaxCount() {
			return maxCount != DefaultMaxCount;
		}
		void ResetMaxCount() {
			MaxCount = DefaultMaxCount;
		}
		bool ShouldSerializeAutomaticScaleBreaksCalculator() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeEnabled() || ShouldSerializeMaxCount();
		}
		#endregion
		void SetEnabled(bool enabled) {
			this.enabled = enabled;
			scaleBreaks.Clear();
		}
		internal void Calculate(List<double> initialValues, Transformation transform) {
			scaleBreaks.Clear();
			List<double> transformedValues = new List<double>(initialValues.Count);
			foreach (double value in initialValues)
				transformedValues.Add(transform.TransformForward(value));
			transformedValues.Sort();
			IAutomaticScaleBreaksCalculator calculator = ActualAutomaticScaleBreaksCalculator;
			IList<AutomaticScaleBreak> automaticScaleBreaks = calculator.Calculate(transformedValues, MaxCount);
			int scaleBreaksCount = Math.Min(MaxCount, automaticScaleBreaks.Count);
			for (int i = 0; i < scaleBreaksCount; i++) {
				double edge1 = transform.TransformBackward(automaticScaleBreaks[i].Edge1);
				double edge2 = transform.TransformBackward(automaticScaleBreaks[i].Edge2);
				scaleBreaks.Add(new AutomaticScaleBreak(edge1, edge2));
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new AutoScaleBreaks(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AutoScaleBreaks autoScaleBreaks = obj as AutoScaleBreaks;
			if (autoScaleBreaks != null) {
				SetEnabled(autoScaleBreaks.enabled);
				maxCount = autoScaleBreaks.maxCount;
				automaticScaleBreaksCalculator = autoScaleBreaks.automaticScaleBreaksCalculator;
			}
		}
	}
	public interface IAutomaticScaleBreaksCalculator {
		IList<AutomaticScaleBreak> Calculate(List<double> axisValues, int maxScaleBreaksCount);
	}
	public class AutomaticScaleBreak : IScaleDiapason {
		readonly double edge1;
		readonly double edge2;
		bool IScaleDiapason.Visible { get { return true; } }
		public double Edge1 { get { return edge1; } }
		public double Edge2 { get { return edge2; } }
		public AutomaticScaleBreak(double edge1, double edge2) {
			if (!GeometricUtils.IsValidDouble(edge1))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDoubleValue), "edge1");
			if (!GeometricUtils.IsValidDouble(edge2))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDoubleValue), "edge2");
			this.edge1 = edge1;
			this.edge2 = edge2;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class ScaleBreakEdge : IAxisValueContainer {
		readonly ScaleBreak scaleBreak;
		object axisValue;
		object axisValueSerializable;
		double value;
		public object AxisValue {
			get { return axisValue; }
			set { axisValue = value; }
		}
		public object AxisValueSerializable { set { axisValueSerializable = value; } }
		public double Value { get { return value; } }
		public ScaleBreakEdge(ScaleBreak scaleBreak)
			: this(scaleBreak, null) {
		}
		public ScaleBreakEdge(ScaleBreak scaleBreak, object axisValue) {
			this.scaleBreak = scaleBreak;
			this.axisValue = axisValue;
		}
		#region IAxisValueContainer implementation
		IAxisData IAxisValueContainer.Axis { get { return scaleBreak.Axis; } }
		bool IAxisValueContainer.IsEnabled { get { return scaleBreak.Visible; } }
		CultureInfo IAxisValueContainer.Culture {
			get {
				return (axisValueSerializable != null && axisValueSerializable == axisValue) ?
					CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
			}
		}
		object IAxisValueContainer.GetAxisValue() {
			return axisValue;
		}
		void IAxisValueContainer.SetAxisValue(object axisValue) {
			if (!Object.ReferenceEquals(axisValue, this.axisValue)) {
				this.axisValue = axisValue;
				axisValueSerializable = null;
			}
		}
		double IAxisValueContainer.GetValue() {
			return value;
		}
		void IAxisValueContainer.SetValue(double value) {
			this.value = value;
		}
		#endregion
		public void Assign(ScaleBreakEdge edge) {
			axisValue = edge.axisValue;
			value = edge.value;
			axisValueSerializable = null;
		}
	}
	public interface IScaleDiapason {
		bool Visible { get; }
		double Edge1 { get; }
		double Edge2 { get; }
	}
	public class DefaultAutomaticScaleBreaksCalculator : IAutomaticScaleBreaksCalculator {
		IList<AutomaticScaleBreak> IAutomaticScaleBreaksCalculator.Calculate(List<double> axisValues, int maxScaleBreaksCount) {
			List<AutomaticScaleBreak> scaleBreaks = new List<AutomaticScaleBreak>();
			Clusters clusters = new Clusters(axisValues, maxScaleBreaksCount + 1);
			if (clusters.Count > 1 && !double.IsNaN(clusters.MaxSize) && !double.IsNaN(clusters.MinDistance)) {
				double extension = Math.Min(clusters.MaxSize, clusters.MinDistance) / 10;
				for (int i = 0; i < clusters.Count - 1; i++) {
					double edge1 = clusters[i].LastValue + extension;
					double edge2 = clusters[i + 1].FirstValue - extension;
					scaleBreaks.Add(new AutomaticScaleBreak(edge1, edge2));
				}
			}
			return scaleBreaks;
		}
	}
}
