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
using System.Collections.Generic;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.XtraGauges.Core {
	public abstract class BaseSettings {
		public BaseSettings Clone() {
			BaseSettings clone = CloneCore();
			clone.Assign(this);
			return clone;
		}
		public virtual void Assign(BaseSettings settings) {
			if(settings != null)
				AssignCore(settings);
		}
		public void Reset() {
			BaseSettings settings = CloneCore();
			AssignCore(settings);
		}
		protected abstract BaseSettings CloneCore();
		protected abstract void AssignCore(BaseSettings settings);
		protected void Assign<T>(IList<T> target, IList<T> source) {
			if(source == target) return;
			target.Clear();
			foreach(T item in source)
				target.Add(item);
		}
	}
	public class TextSettings : BaseSettings {
		string textCore;
		public string Text {
			get { return textCore; }
			set { textCore = value; }
		}
		protected override BaseSettings CloneCore() {
			return new TextSettings();
		}
		protected override void AssignCore(BaseSettings settings) {
			TextSettings textSettings = settings as TextSettings;
			if(textSettings != null) 
				textCore = textSettings.Text;
		}
		public override string ToString() {
			return textCore;
		}
	}
	public class GaugeSettings : BaseSettings, IDisposable {
		IList<IBaseScale> scalesCore;
		IList<IScaleComponent> scaleComponentsCore;
		IList<Label> labelsCore;
		IList<ImageIndicator> imageIndicatorsCore;
		IList<StateIndicator> stateIndicatorsCore;
		TextSettings textSettingsCore;
		readonly IGauge gaugeCore;
		public GaugeSettings()
			: this(null) {
		}
		protected GaugeSettings(IGauge gauge) {
			gaugeCore = gauge;
			scalesCore = new List<IBaseScale>();
			scaleComponentsCore = new List<IScaleComponent>();
			labelsCore = new List<Label>();
			imageIndicatorsCore = new List<ImageIndicator>();
			stateIndicatorsCore = new List<StateIndicator>();
			textSettingsCore = new TextSettings();
		}
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				if(Gauge != null)
					ApplyCore(Gauge, this);
				Scales.Clear();
				ScaleComponents.Clear();
				Labels.Clear();
				ImageIndicators.Clear();
				StateIndicators.Clear();
			}
			GC.SuppressFinalize(this);
		}
		protected override BaseSettings CloneCore() {
			return new GaugeSettings(Gauge);
		}
		protected override void AssignCore(BaseSettings settings) {
			GaugeSettings gaugeSettings = settings as GaugeSettings;
			if(gaugeSettings != null) {
				TextSettings.Assign(gaugeSettings.TextSettings);
				Assign(Scales, gaugeSettings.Scales);
				Assign(ScaleComponents, gaugeSettings.ScaleComponents);
				Assign(Labels, gaugeSettings.Labels);
				Assign(ImageIndicators, gaugeSettings.ImageIndicators);
				Assign(StateIndicators, gaugeSettings.StateIndicators);
			}
		}
		public IGauge Gauge {
			get { return gaugeCore; }
		}
		public IList<IBaseScale> Scales {
			get { return scalesCore; }
		}
		public IList<IScaleComponent> ScaleComponents {
			get { return scaleComponentsCore; }
		}
		public IList<Label> Labels {
			get { return labelsCore; }
		}
		public IList<ImageIndicator> ImageIndicators {
			get { return imageIndicatorsCore; }
		}
		public IList<StateIndicator> StateIndicators {
			get { return stateIndicatorsCore; }
		}
		public TextSettings TextSettings {
			get { return textSettingsCore; }
		}
		public static GaugeSettings FromGauge(IGauge gauge) {
			GaugeSettings settings = new GaugeSettings(gauge);
			if(gauge != null) {
				IList children = GetChildren(gauge);
				foreach(object element in children) {
					IBaseScale scale = element as IBaseScale;
					if(scale != null) {
						settings.Scales.Add(scale);
						continue;
					}
					IScaleComponent scaleComponent = element as IScaleComponent;
					if(scaleComponent != null) {
						settings.ScaleComponents.Add(scaleComponent);
						continue;
					}
					Label label = element as Label;
					if(label != null) {
						settings.Labels.Add(label);
						continue;
					}
					ImageIndicator imageIndicator = element as ImageIndicator;
					if(imageIndicator != null) {
						settings.ImageIndicators.Add(imageIndicator);
						continue;
					}
					StateIndicator indicator = element as StateIndicator;
					if(indicator != null) {
						settings.StateIndicators.Add(indicator);
					}
				}
				BaseGauge baseGauge = gauge as BaseGauge;
				if(baseGauge != null)
					settings.TextSettings.Assign(baseGauge.ReadTextSettings());
			}
			return settings;
		}
		public void Apply() {
			if(Gauge == null)
				throw new ArgumentNullException("Gauge");
			ApplyCore(Gauge, this);
		}
		public void Apply(IGauge gauge) {
			if(gauge == null || (Gauge != null && gauge != Gauge))
				throw new ArgumentNullException("Gauge");
			ApplyCore(gauge, this);
		}
		static void ApplyCore(IGauge gauge, GaugeSettings settings) {
			IList children = GetChildren(gauge);
			gauge.BeginUpdate();
			EnsureAdded(settings.Scales, gauge, children);
			EnsureAdded(settings.ScaleComponents, gauge, children);
			EnsureAdded(settings.Labels, gauge, children);
			EnsureAdded(settings.ImageIndicators, gauge, children);
			EnsureAdded(settings.StateIndicators, gauge, children);
			BaseGauge baseGauge = gauge as BaseGauge;
			if(baseGauge != null)
				baseGauge.ApplyTextSettings(settings.TextSettings);
			gauge.EndUpdate();
		}
		static void EnsureAdded<T>(IEnumerable<T> collection, IGauge gauge, IList children) {
			foreach(T element in collection) {
				if(!children.Contains(element)) {
					EnsureName<T>(gauge, element);
					gauge.AddGaugeElement(element as BaseElement<IRenderableElement>);
				}
			}
		}
		static void EnsureName<T>(IGauge gauge, T element) {
			INamed named = element as INamed;
			if(named != null && string.IsNullOrEmpty(named.Name)) {
				named.Name = UniqueNameHelper.GetUniqueName("gaugeElement", gauge.GetNames(), 0);
			}
		}
		static IList GetChildren(IGauge gauge) {
			return (gauge as ISerizalizeableElement).GetChildren();
		}
	}
}
