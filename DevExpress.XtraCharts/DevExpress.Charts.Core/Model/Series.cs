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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Charts.Native;
using System.Reflection;
namespace DevExpress.Charts.Model {
	public interface ISupportMarkerSeries {
		Marker Marker { get; set; }
	}
	public interface ISupportColorEachSeries {
		bool ColorEach { get; set; }
	}
	public interface ISupportTransparencySeries {
		byte Transparency { get; set; }
	}
	public interface ISupportBarWidthSeries {
		double BarWidth { get; set; }
	}
	public interface ISupportBar3DModelSeries {
		Bar3DModel Model { get; set; }
	}
	public interface IDataLabelFormatter {
		string GetDataLabelText(LabelPointData pointData);
	}
	public enum MarkerType {
		Square,
		Diamond,
		Triangle,
		InvertedTriangle,
		Circle,
		Plus,
		Cross,
		Star,
		Pentagon,
		Hexagon
	}
	public class Marker : ModelElement {
		int size = 0;
		bool visible = true;
		ColorARGB color = ColorARGB.Empty;
		MarkerType type;
		public int Size {
			get { return size; }
			set {
				if(size != value) {
					size = value;
					NotifyParent(this, "Size", value);
				}
			}
		}
		public bool Visible {
			get { return visible; }
			set {
				if(visible != value) {
					visible = value;
					NotifyParent(this, "Visible", value);
				}
			}
		}
		public ColorARGB Color {
			get { return color; }
			set {
				if(color != value) {
					color = value;
					NotifyParent(this, "Color", value);
				}
			}
		}
		public MarkerType MarkerType {
			get { return type; }
			set {
				if(type != value) {
					type = value;
					NotifyParent(this, "MarkerType", value);
				}
			}
		}
		public Marker(ModelElement parent)
			: base(parent) {
		}
	}
	public enum SeriesLabelPosition {
		Left,
		Top,
		Righ,
		Bottom,
		Center,
		InsideBase,
		InsideEnd,
		OutsideEnd,
		BestFit
	}
	public class SeriesLabel : ModelElement {
		SeriesLabelPosition position;
		IDataLabelFormatter formatter;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		public SeriesLabelPosition Position {
			get { return position; }
			set {
				if (position != value) {
					position = value;
					NotifyParent(this, "Position", value);
				}
			}
		}
		public IDataLabelFormatter Formatter {
			get { return formatter; }
			set {
				if (formatter != value) {
					formatter = value;
					NotifyParent(this, "Formatter", value);
				}
			}
		}
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing != value) {
					enableAntialiasing = value;
					NotifyParent(this, "EnableAntialiasing", value);
				}
			}
		}
		public SeriesLabel(ModelElement parent) : base(parent) {
		}
	}
	public struct LabelPointData {
		readonly object context;
		object argument;
		readonly double normalizedValue;
		public object Context { get { return context; } }
		public object Argument { get { return argument; } }
		public double NormalizedValue { get { return normalizedValue; } }
		public LabelPointData(object context, object argument, double normalizedValue) {
			this.context = context;
			this.argument = argument;
			this.normalizedValue = normalizedValue;
		}
	}
	public enum Bar3DModel {
		Box,
		Cylinder,
		Cone,
		Pyramid
	}
}
namespace DevExpress.Charts.Model.Native {
	public abstract class SeriesPropertyBagBase : ModelElement {
		public SeriesPropertyBagBase() { 
		}
		public SeriesPropertyBagBase(ModelElement parent) : base(parent) { 
		}
		public virtual TInterface GetSupport<TInterface>() where TInterface : class {
			if(typeof(TInterface).IsAssignableFrom(this.GetType()))
				return this as TInterface;
			return default(TInterface);
		}
	}
	public sealed class EmptyPropertyBag : SeriesPropertyBagBase {
		public static readonly EmptyPropertyBag Instance = new EmptyPropertyBag();
		public EmptyPropertyBag() {
		}
	}
	public class SeriesPropertyBagSupport : SeriesPropertyBagBase {
		public SeriesPropertyBagSupport() { 
		}
	}
	public sealed class TransparencyBagSupport : SeriesPropertyBagSupport, ISupportTransparencySeries {
		byte transparency = 120;
		public byte Transparency {
			get { return transparency; }
			set {
				if(transparency != value) {
					transparency = value;
					NotifyParent(this, "Transparency", value);
				}
			}
		}
	}
	public sealed class ColorEachBagSupport : SeriesPropertyBagSupport, ISupportColorEachSeries {
		bool colorEach;
		public bool ColorEach {
			get { return colorEach; }
			set {
				if(value != colorEach) {
					colorEach = value;
					NotifyParent(this, "ColorEach", value);
				}
			}
		}
	}
	public sealed class MarkerBagSupport : SeriesPropertyBagSupport, ISupportMarkerSeries {
		Marker marker;
		public Marker Marker {
			get { return marker; }
			set {
				if(marker != value) {
					marker = value;
					NotifyParent(this, "Marker", value);
				}
			}
		}
	}
	public sealed class BarWidthBagSupport : SeriesPropertyBagSupport, ISupportBarWidthSeries {
		double barWidth = 0.6;
		public double BarWidth {
			get { return barWidth; }
			set {
				if (value != barWidth) {
					barWidth = value;
					NotifyParent(this, "BarWidth", value);
				}
			}
		}
	}
	public sealed class Bar3DModelBagSupport : SeriesPropertyBagSupport, ISupportBar3DModelSeries {
		Bar3DModel model = Bar3DModel.Box;
		public Bar3DModel Model {
			get { return model; }
			set {
				if (value != model) {
					model = value;
					NotifyParent(this, "Model", value);
				}
			}
		}
	}
	public class GenericPropertyBag<T> : SeriesPropertyBagBase where T : SeriesPropertyBagSupport, new() {
		T support1;
		protected T Support1 { get { return support1; } }
		public GenericPropertyBag(ModelElement parent) : base(parent) {
			this.support1 = new T() { Parent = parent };
		}
		public override TInterface GetSupport<TInterface>() {
			TInterface result = support1.GetSupport<TInterface>();
			if(result != null) return result;
			return default(TInterface);
		}
	}
	public class GenericPropertyBag2<T, U> : GenericPropertyBag<T>
		where T : SeriesPropertyBagSupport, new()
		where U : SeriesPropertyBagSupport, new() {
		U support2;
		protected U Support2 { get { return support2; } }
		public GenericPropertyBag2(ModelElement parent)
			: base(parent) {
			this.support2 = new U() { Parent = parent }; 
		}
		public override TInterface GetSupport<TInterface>() {
		   TInterface result = support2.GetSupport<TInterface>();
		   if(result != null) return result;
		   return base.GetSupport<TInterface>();
		}
	}
	public class GenericPropertyBag3<T, U, V> : GenericPropertyBag2<T, U>
		where T : SeriesPropertyBagSupport, new()
		where U : SeriesPropertyBagSupport, new()
		where V : SeriesPropertyBagSupport, new() {
		V support3;
		protected V Support3 { get { return support3; } }
		public GenericPropertyBag3(ModelElement parent)
			: base(parent) {
			this.support3 = new V() { Parent = parent }; 
		}
		public override TInterface GetSupport<TInterface>() {
			TInterface result = support3.GetSupport<TInterface>();
			if(result != null) return result;
			return base.GetSupport<TInterface>();
		}
	}
	public class GenericPropertyBag4<T, U, V, M> : GenericPropertyBag3<T, U, V>
		where T : SeriesPropertyBagSupport, new()
		where U : SeriesPropertyBagSupport, new()
		where V : SeriesPropertyBagSupport, new()
		where M : SeriesPropertyBagSupport, new() {
		M support4;
		protected M Support4 { get { return support4; } }
		public GenericPropertyBag4(ModelElement parent)
			: base(parent) {
			this.support4 = new M() { Parent = parent };
		}
		public override TInterface GetSupport<TInterface>() {
			TInterface result = support4.GetSupport<TInterface>();
			if (result != null) return result;
			return base.GetSupport<TInterface>();
		}
	}
}
