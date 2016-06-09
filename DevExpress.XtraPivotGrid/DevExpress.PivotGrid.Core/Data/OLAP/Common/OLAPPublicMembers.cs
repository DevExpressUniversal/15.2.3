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
using System.Linq.Expressions;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.OLAP;
using System.Linq;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.XtraPivotGrid {
	public class PivotOLAPKPIValue {
		object value, goal;
		int status, trend;
		double weight;
		public PivotOLAPKPIValue(object value, object goal, int status, int trend, double weight) {
			this.value = value;
			this.goal = goal;
			this.status = status;
			this.trend = trend;
			this.weight = weight;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIValueValue")]
#endif
		public object Value { get { return value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIValueGoal")]
#endif
		public object Goal { get { return goal; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIValueStatus")]
#endif
		public int Status { get { return status; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIValueTrend")]
#endif
		public int Trend { get { return trend; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIValueWeight")]
#endif
		public double Weight { get { return weight; } }
	}
	public class PivotOLAPKPIMeasures {
		string kpiName;
		string value, goal, status, trend, weight;
		public PivotOLAPKPIMeasures(string kpiName, string value, string goal, string status, string trend, string weight)
			: this(kpiName) {
			Initialize(value, goal, status, trend, weight);
		}
		public PivotOLAPKPIMeasures(TypedBinaryReader reader) {
			this.kpiName = reader.ReadString();
			this.value = reader.ReadString();
			this.goal = reader.ReadString();
			this.status = reader.ReadString();
			this.trend = reader.ReadString();
			this.weight = reader.ReadString();
		}
		internal PivotOLAPKPIMeasures(DevExpress.PivotGrid.OLAP.SchemaEntities.KpisRowSet kpi)
			: this(kpi.Name, kpi.Value, kpi.Goal, kpi.Status, kpi.Trend, kpi.Weight) {
		}
		protected PivotOLAPKPIMeasures(string kpiName) {
			this.kpiName = kpiName;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIMeasuresKPIName")]
#endif
		public string KPIName { get { return kpiName; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIMeasuresValueMeasure")]
#endif
		public string ValueMeasure { get { return value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIMeasuresGoalMeasure")]
#endif
		public string GoalMeasure { get { return goal; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIMeasuresStatusMeasure")]
#endif
		public string StatusMeasure { get { return status; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIMeasuresTrendMeasure")]
#endif
		public string TrendMeasure { get { return trend; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotOLAPKPIMeasuresWeightMeasure")]
#endif
		public string WeightMeasure { get { return weight; } }
		protected void Initialize(string value, string goal, string status, string trend, string weight) {
			this.value = value;
			this.goal = goal;
			this.status = status;
			this.trend = trend;
			this.weight = weight;
		}
		public override string ToString() {
			return KPIName;
		}
		public void SaveToStream(TypedBinaryWriter writer) {
			writer.Write(KPIName);
			writer.Write(ValueMeasure);
			writer.Write(GoalMeasure);
			writer.Write(StatusMeasure);
			writer.Write(TrendMeasure);
			writer.Write(WeightMeasure);
		}
	}
	interface ITypedValue<TType> {
		TType Value { get; }
	}
	class OLAPTypedProperty<TType> : OLAPMemberProperty, ITypedValue<TType> {
		TType value;
		public override object Value {
			get { return value; }
		}
		public OLAPTypedProperty(OLAPPropertyDescriptor descr, TType value)
			: base(descr) {
			this.value = value;
		}
		TType ITypedValue<TType>.Value {
			get { return value; }
		}
	}
	public abstract class OLAPMemberProperty {
		static Dictionary<Type, Func<OLAPPropertyDescriptor, string, OLAPMemberProperty>> fromStringConstructors = new Dictionary<Type, Func<OLAPPropertyDescriptor, string, OLAPMemberProperty>>();
		static Dictionary<Type, Func<OLAPPropertyDescriptor, object, OLAPMemberProperty>> fromObjectConstructors = new Dictionary<Type, Func<OLAPPropertyDescriptor, object, OLAPMemberProperty>>();
		static OLAPMemberProperty() {
			fromStringConstructors.Add(typeof(object), (a, b) => new OLAPTypedProperty<object>(a, b));
		}
		internal static OLAPMemberProperty Create(OLAPPropertyDescriptor property, string value) {
			Func<OLAPPropertyDescriptor, string, OLAPMemberProperty> val;
			if(!fromStringConstructors.TryGetValue(property.Type, out val)) {
				Type type = typeof(OLAPTypedProperty<>).MakeGenericType(property.Type);
				ParameterExpression prop = Expression.Parameter(typeof(OLAPPropertyDescriptor), "");
				ParameterExpression strVal = Expression.Parameter(typeof(string), "");
				LabelTarget returnTarget = Expression.Label(typeof(OLAPMemberProperty));
				Expression convert;
				if(property.Type == typeof(string))
					convert = strVal;
				else
					convert = Expression.Call(property.Type, "Parse", null, strVal);
				BlockExpression block = Expression.Block(
					Expression.Return(returnTarget,
										  Expression.Convert(Expression.New(type.GetConstructors()[0], prop, convert), typeof(OLAPMemberProperty))),
										  Expression.Label(returnTarget, Expression.Constant(null, type))
										 );
				val = Expression.Lambda<Func<OLAPPropertyDescriptor, string, OLAPMemberProperty>>(
																									block,
																									new ParameterExpression[] { prop, strVal }
																								).Compile();
				fromStringConstructors[property.Type] = val;
			}
			return val(property, value);
		}
		internal static OLAPMemberProperty Create(OLAPPropertyDescriptor property, object value) {
			Func<OLAPPropertyDescriptor, object, OLAPMemberProperty> val;
			if(!fromObjectConstructors.TryGetValue(property.Type, out val)) {
				Type type = typeof(OLAPTypedProperty<>).MakeGenericType(property.Type);
				ParameterExpression prop = Expression.Parameter(typeof(OLAPPropertyDescriptor), "");
				ParameterExpression objVal = Expression.Parameter(typeof(object), "");
				LabelTarget returnTarget = Expression.Label(typeof(OLAPMemberProperty));
				Expression convert = Expression.Convert(Expression.Call(typeof(Convert), "ChangeType", null, objVal, Expression.Constant(property.Type)), property.Type);
				BlockExpression block = Expression.Block(
					Expression.Return(returnTarget,
										  Expression.Convert(Expression.New(type.GetConstructors()[0], prop, convert), typeof(OLAPMemberProperty))),
										  Expression.Label(returnTarget, Expression.Constant(null, type))
										 );
				val = Expression.Lambda<Func<OLAPPropertyDescriptor, object, OLAPMemberProperty>>(
																									block,
																									new ParameterExpression[] { prop, objVal }
																								).Compile();
				fromObjectConstructors[property.Type] = val;
			}
			return val(property, value);
		}
		public static bool operator ==(OLAPMemberProperty o1, OLAPMemberProperty o2) {
			return object.Equals(o1, o2);
		}
		public static bool operator !=(OLAPMemberProperty o1, OLAPMemberProperty o2) {
			return !(o1 == o2);
		}
		readonly OLAPPropertyDescriptor descr;
		protected OLAPMemberProperty(OLAPPropertyDescriptor descr) {
			this.descr = descr;
		}
		public string Name { get { return descr.Name; } }
		public Type Type { get { return descr.Type; } }
		public abstract object Value { get; }
		public override bool Equals(object obj) {
			return EqualsCore(this, obj as OLAPMemberProperty);
		}
		bool EqualsCore(OLAPMemberProperty object1, OLAPMemberProperty object2) {
			if(object.ReferenceEquals(object1, object2))
				return true;
			if((object1 == null) || (object2 == null))
				return false;
			return object1.Name == object2.Name && object1.Type == object2.Type &&
				object.Equals(object1.Value, object2.Value);
		}
		public override int GetHashCode() { return Name.GetHashCode(); }
		public override string ToString() { return this.Name; }
	}
	public class OLAPMemberProperties : IEnumerable<OLAPMemberProperty> {
		Dictionary<string, OLAPMemberProperty> dic = new Dictionary<string,OLAPMemberProperty>();
		internal OLAPMemberProperties() {
		}
		public OLAPMemberProperties(IList<OLAPMemberProperty> properties) {
			for(int i = 0; i < properties.Count; i++) {
				OLAPMemberProperty prop = properties[i];
				dic.Add(prop.Name, prop);
			}
		}
		public int Count { get { return dic.Count; } }
		public OLAPMemberProperty this[int index] {
			get { return ((IEnumerable<OLAPMemberProperty>)this).Skip(index - 1).First(); }
		}
		public OLAPMemberProperty this[string name] {
			get {
				OLAPMemberProperty res;
				return dic.TryGetValue(name, out res) ? res : null;
			}
			internal set {
				dic[name] = value;
			}
		}
		#region IEnumerable<OLAPMemberProperty> Members
		IEnumerator<OLAPMemberProperty> IEnumerable<OLAPMemberProperty>.GetEnumerator() {
			foreach(KeyValuePair<string, OLAPMemberProperty> pair in dic)
				yield return pair.Value;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			foreach(KeyValuePair<string, OLAPMemberProperty> pair in dic)
				yield return pair.Value;
		}
		#endregion
		internal bool TryGetValue(string attrName, out OLAPMemberProperty ap) {
			return dic.TryGetValue(attrName, out ap);
		}
		internal IEnumerable<KeyValuePair<string, OLAPMemberProperty>> GetDictionaryEnumerator() {
			return dic;
		}
		internal bool ContainsKey(string name) {
			return dic.ContainsKey(name);
		}
		internal void Add(string name, OLAPMemberProperty property) {
			dic.Add(name, property);
		}
	}
}
