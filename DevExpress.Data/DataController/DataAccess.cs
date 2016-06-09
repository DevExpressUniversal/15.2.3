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
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Threading;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using System.Globalization;
using DevExpress.Utils;
using System.ComponentModel;
using System.Linq.Expressions;
using DevExpress.Data.Helpers;
using System.Collections.Concurrent;
namespace DevExpress.Data.Access {
	[AttributeUsage(AttributeTargets.Class)]
	public class DataPrimitiveAttribute : Attribute {
	}
	public class SimpleListPropertyDescriptor : PropertyDescriptor {
		public SimpleListPropertyDescriptor() : base("Column", null) {
		}
		public override bool IsReadOnly { get {	return true; }	}
		public override string Category { get {	return string.Empty; } }
		public override Type PropertyType { get { return typeof(object); } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) { return component; }
		public override void SetValue(object component, object value) { 
		}
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public abstract class ComplexPropertyDescriptor : PropertyDescriptor {
		protected DataControllerBase controller;
		protected string path;
		bool isReady = false;
		protected string[] split = null;
		protected object sourceObject = null;
		public ComplexPropertyDescriptor(DataControllerBase controller, string path)
			: base(path, null) {
			this.path = path;
			this.controller = controller;
			Prepare();
		}
		public ComplexPropertyDescriptor(object sourceObject, string path)
			: base(path, null) {
			this.path = path;
			this.controller = null;
			this.sourceObject = sourceObject;
			Prepare();
		}
		protected virtual void Prepare() {
		}
		protected void PrepareSplit() {
			if(split == null) split = path.Split(new char[] {'.'});
			if(split == null || split.Length == 0) return;
		}
		protected bool IsReady { get { return isReady; } set { isReady = value; } }
		public override string Category { get { return string.Empty; } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override bool ShouldSerializeValue(object component) { return false; }
		public abstract object GetOwnerOfLast(object component);
	}
	public class ComplexPropertyDescriptorLinq : ComplexPropertyDescriptor {
		PropertyInfo[] descriptors = null;
		public ComplexPropertyDescriptorLinq(DataControllerBase controller, string path) : base(controller, path) { }
		public ComplexPropertyDescriptorLinq(object sourceObject, string path) : base(sourceObject, path) { }
		protected override void Prepare() {
			if(IsReady) return;
			try {
				object prevObject = null;
				PrepareSplit();
				if(split == null || split.Length == 0) return;
				if(this.descriptors == null || this.descriptors.Length == 0) this.descriptors = new PropertyInfo[split.Length];
				for(int n = 0; n < split.Length; n++) {
					string name = split[n];
					if(n == 0) {
						if(controller != null && sourceObject == null) {
							DataColumnInfo info = controller.Columns[name];
							if(info == null) return;
							descriptors[0] = info.PropertyDescriptor.ComponentType.GetProperty(info.PropertyDescriptor.Name);
							prevObject = controller.Helper.Count > 0 ? info.PropertyDescriptor.GetValue(controller.Helper.GetRow(0)) : null;
						}
						else {
							if(sourceObject == null) return;
							descriptors[0] = GetDescriptor(name, sourceObject, null);
							if(descriptors[0] == null) return;
							prevObject = descriptors[0].GetValue(sourceObject, null);
						}
						continue;
					}
					PropertyInfo prev = descriptors[n - 1];
					Type propertyType = prev.PropertyType;
					descriptors[n] = GetDescriptor(name, prevObject, propertyType);
					if(descriptors[n] == null)
						return;
					if(prevObject != null && prevObject != DBNull.Value)
						prevObject = descriptors[n].GetValue(prevObject, null);
				}
			}
			catch {
				return;
			}
			this.IsReady = true;
			Compile();
		}
		void Compile() {
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "d");
			var result = Expression.Variable(typeof(object), "final");
			var linex = GenerateTree(result, instanceParameter, 0);
			var block = Expression.Block(new[] { result }, linex, result);
			Func = Expression.Lambda<Func<object, object>>(block, instanceParameter).Compile();
		}
		PropertyInfo GetDescriptor(string name, object obj, Type type) {
			if(obj == null || obj == DBNull.Value) {
				PropertyInfo pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
				return pi;
			}
			else {
				type = obj.GetType();
				PropertyInfo pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
				return pi;
			}
		}
		public Func<object, object> Func;
		Expression GenerateTree(ParameterExpression result, Expression propertyResult, int c) {
			if(c >= descriptors.Length) return Expression.Assign(result, propertyResult);
			var convert = Expression.Convert(propertyResult, this.descriptors[c].DeclaringType);
			var call = Expression.Property(convert, this.descriptors[c]);
			var nullValue = Expression.Constant(null);
			var dbNull = Expression.Convert(Expression.Constant(DBNull.Value), typeof(object));
			var variable = Expression.Variable(typeof(object), "temp");
			var callResult = Expression.Assign(variable, call);
			var line1 = Expression.Or(Expression.ReferenceEqual(variable, nullValue), Expression.ReferenceEqual(variable, dbNull));
			var line2 = Expression.IfThenElse(line1, Expression.Assign(result, nullValue), GenerateTree(result, variable, c + 1));
			return Expression.Block(new[] { variable }, callResult, line2);
		}
		PropertyInfo Last { get { return descriptors == null || descriptors.Length == 0 ? null : descriptors[descriptors.Length - 1]; } }
		PropertyInfo Root { get { return descriptors == null || descriptors.Length == 0 ? null : descriptors[0]; } }
		protected override AttributeCollection CreateAttributeCollection() {
			AttributeCollection attributeCollection = base.CreateAttributeCollection();
			if(Last != null) {
#if DXRESTRICTED
				List<Attribute> attributes = new List<Attribute>(Last.GetCustomAttributes(true));
				Attribute[] newCollection = new Attribute[attributeCollection.Count + attributes.Count];
#else
				var attributes = Last.GetCustomAttributes(true);
				Attribute[] newCollection = new Attribute[attributeCollection.Count + attributes.Length];
#endif
				attributeCollection.CopyTo(newCollection, 0);
				attributes.CopyTo(newCollection, attributeCollection.Count);
				return new AttributeCollection(newCollection);
			}
			else {
				return attributeCollection;
			}
		}
		public override bool IsReadOnly {
			get {
				if(Last == null) return true;
				return Last.CanWrite;
			}
		}
		public override Type PropertyType { get { return Last == null ? typeof(object) : Last.PropertyType; } }
		public override Type ComponentType { get { return Root == null ? typeof(object) : Root.DeclaringType; } }
		public override string DisplayName {
			get {
				Prepare();
				if(IsReady)
					return this.descriptors[this.descriptors.Length - 1].Name; 
				return base.DisplayName;
			}
		}
		public override object GetValue(object component) {
			if(controller == null || Func == null) this.sourceObject = component;
			Prepare();
			return Func == null ? null : Func(component);
		}
		public override void SetValue(object component, object value) {
			if(internalSet == null) {
				internalSet = new ComplexPropertyDescriptorReflection(controller, path);
			}
			internalSet.SetValue(component, value);
		}
		ComplexPropertyDescriptorReflection internalSet = null;
		public override object GetOwnerOfLast(object component) {
			throw new NotImplementedException();
		}
	}
	public class ComplexPropertyDescriptorReflection : ComplexPropertyDescriptor {
		PropertyDescriptor[] descriptors = new PropertyDescriptor[0];
		public ComplexPropertyDescriptorReflection(DataControllerBase controller, string path) : base(controller, path) { }
		public ComplexPropertyDescriptorReflection(object sourceObject, string path) : base(sourceObject, path) { }
		protected PropertyDescriptor Root {
			get {
				Prepare();
				return descriptors.Length > 0 ? descriptors[0] : null;
			}
		}
		protected PropertyDescriptor Last {
			get {
				Prepare();
				return descriptors.Length > 0 ? descriptors[descriptors.Length - 1] : null;
			}
		}
		protected override AttributeCollection CreateAttributeCollection() {
			AttributeCollection attributeCollection = base.CreateAttributeCollection();
			if(Last != null) {
				Attribute[] newCollection = new Attribute[attributeCollection.Count + Last.Attributes.Count];
				attributeCollection.CopyTo(newCollection, 0);
				Last.Attributes.CopyTo(newCollection, attributeCollection.Count);
				return new AttributeCollection(newCollection);
			}
			else {
				return attributeCollection;
			}
		}
		public override bool IsReadOnly {
			get {
				if(Last == null) return true;
				return Last.IsReadOnly;
			}
		}
		public override Type PropertyType { get { return Last == null ? typeof(object) : Last.PropertyType; } }
		public override Type ComponentType { get { return Root == null ? typeof(object) : Root.ComponentType; } }
		public override string DisplayName {
			get {
				Prepare();
				if(IsReady)
					return this.descriptors[this.descriptors.Length - 1].DisplayName;
				return base.DisplayName;
			}
		}
		public override object GetValue(object component) {
			if(controller == null) this.sourceObject = component;
			Prepare();
			if(!IsReady) return null;
			object owner = GetOwnerOfLast(component);
			return (owner != null) ? Last.GetValue(owner) : null;
		}
		public override void SetValue(object component, object value) {
			if(controller == null) this.sourceObject = component;
			Prepare();
			if(!IsReady) return;
			object owner = GetOwnerOfLast(component);
			if(owner != null) {
				Last.SetValue(owner, value);
			}
		}
		protected override void Prepare() {
			if(IsReady) return;
			try {
				object prevObject = null;
				PrepareSplit();
				if(split == null || split.Length == 0) return;
				if(this.descriptors.Length == 0) this.descriptors = new PropertyDescriptor[split.Length];
				for(int n = 0; n < split.Length; n++) {
					string name = split[n];
					if(n == 0) {
						if(controller == null) {
							if(sourceObject == null) return;
							descriptors[0] = GetDescriptor(name, sourceObject, null);
							if(descriptors[0] == null) return;
							prevObject = descriptors[0].GetValue(sourceObject);
						}
						else {
							DataColumnInfo info = controller.Columns[name];
							if(info == null) return;
							descriptors[n] = info.PropertyDescriptor;
							prevObject = controller.Helper.Count > 0 ? info.PropertyDescriptor.GetValue(controller.Helper.GetRow(0)) : null;
						}
						continue;
					}
					PropertyDescriptor prev = descriptors[n - 1];
					Type propertyType = prev.PropertyType;
					descriptors[n] = GetDescriptor(name, prevObject, propertyType);
					if(descriptors[n] == null)
						return;
					if(prevObject != null && prevObject != DBNull.Value)
						prevObject = descriptors[n].GetValue(prevObject);
				}
			} catch {
				return;
			}
			this.IsReady = true;
		}
		protected virtual PropertyDescriptor GetDescriptor(string name, object obj, Type type) {
			if(ExpandoPropertyDescriptor.IsDynamicType(type)) {
				return ExpandoPropertyDescriptor.GetProperty(name, obj, type);
			}
			if(obj == null || obj == DBNull.Value) {
				PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(type);
				if(coll == null)
					return null;
				return coll[name];
			} else {
				PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
				if(coll == null)
					return null;
				PropertyDescriptor pd = coll[name];
				if(pd == null)
					return null;
				if(typeof(ITypedList).IsAssignableFrom(pd.PropertyType) && typeof(IList).IsAssignableFrom(pd.PropertyType)) {
					return new ReferenceAsCollectionPropertyDescriptor(pd);
				} else {
					return pd;
				}
			}
		}
		public override object GetOwnerOfLast(object component) {
			for(int n = 0; n < this.descriptors.Length - 1; n++) {
				component = this.descriptors[n].GetValue(component);
				if(component == null || component == DBNull.Value) return null;
			}
			return component;
		}
	}
	public class ExpandoPropertyDescriptor : PropertyDescriptor {
		Type propertyType = null;
		public ExpandoPropertyDescriptor(DataControllerBase controller, string name, Type propertyType)
			: base(name, null) {
			this.propertyType = propertyType;
		}
		public static bool IsDynamicType(Type type) {
			if(type == null) return false;
			return typeof(System.Dynamic.ExpandoObject).IsAssignableFrom(type);
		}
		public override bool IsReadOnly { get { return false; } }
		public override string Category { get { return string.Empty; } }
		public override Type PropertyType {  get {  return propertyType == null ? typeof(object) : propertyType; } }
		public override Type ComponentType { get { return typeof(object); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			IDictionary<string, object> dict = component as IDictionary<string, object>;
			object rv;
			dict.TryGetValue(Name, out rv);
			return rv;
		}
		public override void SetValue(object component, object value) {
			IDictionary<string, object> dict = component as IDictionary<string, object>;
			if(dict != null) dict[Name] = value;
		}
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string DisplayName { get { return base.DisplayName; } }
		public static PropertyDescriptor GetProperty(string name, object obj, Type type) {
			Type ptype = null;
			IDictionary<string, object> dict = obj as IDictionary<string, object>;
			if(dict != null) {
				if(dict.ContainsKey(name)) {
					object value = dict[name];
					ptype = value == null ? null : value.GetType();
				}
			}
			return new ExpandoPropertyDescriptor(null, name, ptype);
		}
	}
	public class ReferenceAsCollectionPropertyDescriptor: PropertyDescriptor {
		public readonly PropertyDescriptor CollectionDescriptor;
		public ReferenceAsCollectionPropertyDescriptor(PropertyDescriptor originalDescriptor)
			: base(originalDescriptor.Name, null) {
			CollectionDescriptor = originalDescriptor;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return CollectionDescriptor.ComponentType; }
		}
		public override object GetValue(object component) {
			IList collection = (IList)CollectionDescriptor.GetValue(component);
			if(collection == null || collection.Count == 0)
				return null;
			return collection[0];
		}
		public override bool IsReadOnly {
			get { return true; }
		}
		public override Type PropertyType {
			get { return GenericTypeHelper.GetGenericIListTypeArgument(CollectionDescriptor.PropertyType) ?? typeof(object); }
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
			throw new NotSupportedException();
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
	public class DisplayTextPropertyDescriptor : PropertyDescriptor {
		DataColumnInfo info;
		DataController controller;
		IDataControllerSort sortClient;
		public const string DxFtsPropertyPrefix = "DxFts_";
		internal DisplayTextPropertyDescriptor(DataController controller, DataColumnInfo info) : this(controller, info, info.Name) { }
		internal DisplayTextPropertyDescriptor(DataController controller, DataColumnInfo info, string name) : base(name, null) {
			this.controller = controller;
			this.info = info;
			this.sortClient = Controller.SortClient;
		}
		public DataController Controller { get { return controller; } }
		public DataColumnInfo Info { get { return info; } }
		public override bool IsReadOnly { get {	return Info.ReadOnly; }	}
		public override string Category { get {	return string.Empty; } }
		public override Type PropertyType { get { return typeof(string); } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public string GetDisplayText(int listRowIndex, object value) {
			return this.sortClient.GetDisplayText(listRowIndex, Info, value, Name);
		}
		public object GetValue(int listRowIndex, object component) {
			return GetDisplayText(listRowIndex, Info.PropertyDescriptor.GetValue(component));
		}
		public override object GetValue(object component) { 
			return GetValue(DataController.InvalidRow, component);
		}
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public class UnboundPropertyDescriptor : PropertyDescriptor {
		DataColumnInfo info;
		UnboundColumnInfo unboundInfo;
		DataControllerBase controller;
		IDataControllerData data;
		ExpressionEvaluator evaluator;
		bool evaluatorCreated;
		Type dataType;
		Exception evaluatorCreateException = null;
		protected internal UnboundPropertyDescriptor(DataControllerBase controller, UnboundColumnInfo unboundInfo)
			: base(unboundInfo.Name, null) {
			this.evaluatorCreated = false;
			this.evaluator = null;
			this.controller = controller;
			this.data = controller.DataClient;
			this.unboundInfo = unboundInfo;
			this.dataType = UnboundInfo.DataType;
			this.info = null;
		}
		void CreateEvaluator() {
			this.evaluatorCreated = true;
			this.evaluator = Controller.CreateExpressionEvaluator(CriteriaOperator.TryParse(UnboundInfo.Expression), true, out evaluatorCreateException);
		}
		protected ExpressionEvaluator Evaluator {
			get {
				if(!evaluatorCreated) CreateEvaluator();
				return evaluator;
			}
		}
		protected DataControllerBase Controller { get { return controller; } }
		public UnboundColumnInfo UnboundInfo { get { return unboundInfo; } }
		public IDataControllerData Data { get { return data; } }
		public DataColumnInfo Info { get { return info; } }
		bool RequireValueConversion { get { return UnboundInfo.RequireValueConversion; } }
		internal void SetInfo(DataColumnInfo info) { this.info = info; }
		public override bool IsBrowsable {
			get {
				return base.IsBrowsable && (UnboundInfo != null && UnboundInfo.Visible);
			}
		}
		public override bool IsReadOnly { get {	return UnboundInfo.ReadOnly; }	}
		public override string Category { get {	return string.Empty; } }
		public override Type PropertyType { get { return UnboundInfo.DataType; } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		int inEvaluatorGet = 0;
		public sealed override object GetValue(object component) {
			return GetValueFromRowNumber((int)component);
		}
		public object GetValueFromRowNumber(int rowNumber) {
			object value = null;
			if(Evaluator != null) {
				value = GetEvaluatorValue(rowNumber);
			} else {
				if(this.evaluatorCreateException != null) value = UnboundErrorObject.Value;
			}
			return Data.GetUnboundData(rowNumber, this.info, value);
		}
		public static bool IsErrorValue(object value) {
			return object.ReferenceEquals(value, UnboundErrorObject.Value);
		}
		protected virtual object GetEvaluatorValue(int row) {
			if(inEvaluatorGet > 0) return UnboundErrorObject.Value;
			object res = null;
			inEvaluatorGet++;
			try {
				try {
					res = Evaluator.Evaluate(row);
				}
				catch {
					return UnboundErrorObject.Value;
				}
				res = Convert(res);
			}
			catch {
			}
			finally {
				inEvaluatorGet--;
			}
			return res;
		}
		protected object Convert(object toConvertValue) {
			if(!RequireValueConversion || toConvertValue == null) return toConvertValue;
			if(IsErrorValue(toConvertValue)) return toConvertValue;
			try {
				Type type = toConvertValue.GetType();
				if(type.Equals(dataType)) return toConvertValue;
				return System.Convert.ChangeType(toConvertValue, dataType, CultureInfo.CurrentCulture);
			}
			catch {
			}
			return null;
		}
		public override void SetValue(object component, object value) { 
			Data.SetUnboundData((int)component, this.info, value);
		}
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public class DataListDescriptor {
		class FastPropertyDescriptor: PropertyDescriptor, PropertyDescriptorCriteriaCompilationSupport.IHelper {
			PropertyDescriptor source;
			Action<object, object> setter;
			Func<object, object> getter;
			public FastPropertyDescriptor(PropertyDescriptor source, Func<object, object> getter, Action<object, object> setter)
				: base(source) {
				this.getter = getter;
				this.setter = setter;
				this.source = source;
			}
			protected PropertyDescriptor Source { get { return source; } }
			public override bool IsReadOnly { get {	return Source.IsReadOnly; }	}
			public override string DisplayName { get { return Source.DisplayName; } }
			public override string Category { get {	return Source.Category; } }
			public override Type PropertyType { get { return Source.PropertyType; } }
			public override Type ComponentType { get { return Source.ComponentType; } }
			public override void ResetValue(object component) { Source.ResetValue(component); }
			public override bool CanResetValue(object component) { return Source.CanResetValue(component); }
			public override object GetValue(object component) {
				if(component == null)
					return null;
				return getter(component);
			}
			public override void SetValue(object component, object value) {
				setter(component, value);
			}
			public override bool ShouldSerializeValue(object component) { return Source.ShouldSerializeValue(component); }
			System.Linq.Expressions.Expression PropertyDescriptorCriteriaCompilationSupport.IHelper.TryMakeFastExpression(System.Linq.Expressions.Expression baseExpression) {
				return PropertyDescriptorCriteriaCompilationSupport.TryMakeFastAccessFromDescriptor(baseExpression, Source);
			}
			Delegate PropertyDescriptorCriteriaCompilationSupport.IHelper.TryGetFastGetter(out Type rowType, out Type valueType) {
				return PropertyDescriptorCriteriaCompilationSupport.TryGetFastGetter(Source, out rowType, out valueType);
			}
		}
		public static PropertyDescriptorCollection GetFastProperties(Type type) {
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(type);
			return GetFastProperties(pdColl);
		}
		public static PropertyDescriptorCollection GetFastProperties(PropertyDescriptorCollection sourceCollection) {
			PropertyDescriptorCollection res = new PropertyDescriptorCollection(null);
			for(int n = 0; n < sourceCollection.Count; n++)
				res.Add(GetFastProperty(sourceCollection[n]));
			return res;
		}
		public static PropertyDescriptor GetFastProperty(PropertyDescriptor source) {
			return TryCreateFastProperty(source) ?? source;
		}
		static Tuple<Func<object, object>, Action<object, object>> TryCreateAccessors(Type rowType, string propertyName) {
			if(rowType.IsValueType())
				return null;
			if(!CompileHelper.IsPublicExposable(rowType))
				return null;
			var property = CompileHelper.FindPropertyOrField(rowType, propertyName, false, false) as PropertyInfo;
			if(property == null)
				return null;
			if(property.Name != propertyName)
				return null;
			if(!property.CanRead)
				return null;
			if(!CompileHelper.IsPublicExposable(property.PropertyType) || !CompileHelper.IsPublicExposable(property.DeclaringType))
				return null;
			Func<object, object> getter;
			Action<object, object> setter;
			if(CompileHelper.CanDynamicMethodWithSkipVisibility()) {
				getter = TryCreateFastGetter(property);
				setter = TryCreateFastSetter(property);
			} else {
				getter = TryCreateSecureGetter(property);
				setter = TryCreateSecureSetter(property);
			}
			if(getter == null && setter == null)
				return null;
			return Tuple.Create(getter, setter);
		}
		static Action<object, object> TryCreateSecureSetter(PropertyInfo property) {
			var setMethod = property.GetSetMethod();
			if(setMethod == null)
				return null;
			var rowParameter = Expression.Parameter(typeof(object), "row");
			var valueParameter = Expression.Parameter(typeof(object), "value");
			Expression typedValue;
			if(property.PropertyType == typeof(object)) {
				typedValue = valueParameter;
			} else {
				typedValue = Expression.Convert(valueParameter, property.PropertyType);
			}
			Expression typedRow = Expression.Convert(rowParameter, property.DeclaringType);
			var assignmentCore = Expression.Call(typedRow, setMethod, typedValue);
			var assignment = Expression.Lambda<Action<object, object>>(assignmentCore, rowParameter, valueParameter).Compile();
			if(NullableHelpers.CanAcceptNull(property.PropertyType))
				return assignment;
			else
				return (object row, object value) => {
					if(value != null)
						assignment(row, value);
				};
		}
		static Func<object, object> TryCreateSecureGetter(PropertyInfo property) {
			if(property.GetGetMethod() == null)
				return null;
			var par = Expression.Parameter(typeof(object), "row");
			var row = Expression.Convert(par, property.DeclaringType);
			var memberAccess = Expression.MakeMemberAccess(row, property);
			Expression body = memberAccess;
			if(body.Type != typeof(object))
				body = Expression.Convert(body, typeof(object));
			var lmbd = Expression.Lambda<Func<object, object>>(body, par);
			return lmbd.Compile();
		}
		static Action<object, object> TryCreateFastSetter(PropertyInfo property) {
			var setMethod = property.GetSetMethod();
			if(setMethod == null)
				return null;
			DynamicMethod method = new DynamicMethod(String.Empty, null, new Type[] { typeof(object), typeof(object) }, property.DeclaringType.IsInterface() ? typeof(DataListDescriptor) : property.DeclaringType, true);
			var ilGen = method.GetILGenerator();
			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Castclass, property.DeclaringType);
			ilGen.Emit(OpCodes.Ldarg_1);
			if(property.PropertyType.IsValueType()) {
				ilGen.Emit(OpCodes.Unbox, property.PropertyType);
				ilGen.Emit(OpCodes.Ldobj, property.PropertyType);
			} else if(property.PropertyType != typeof(object)) {
				ilGen.Emit(OpCodes.Castclass, property.PropertyType);
			}
			ilGen.Emit(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod);
			ilGen.Emit(OpCodes.Ret);
			var assignment = (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
			if(NullableHelpers.CanAcceptNull(property.PropertyType))
				return assignment;
			else
				return (object row, object value) => {
					if(value != null)
						assignment(row, value);
				};
		}
		static Func<object, object> TryCreateFastGetter(PropertyInfo property) {
			Func<object, object> getter;
			var getMethod = property.GetGetMethod();
			if(getMethod == null) {
				getter = null;
			} else {
				DynamicMethod method = new DynamicMethod(String.Empty, typeof(object), new Type[] { typeof(object) }, property.DeclaringType.IsInterface() ? typeof(DataListDescriptor) : property.DeclaringType, true);
				var ilGen = method.GetILGenerator();
				ilGen.Emit(OpCodes.Ldarg_0);
				ilGen.Emit(OpCodes.Castclass, property.DeclaringType);
				ilGen.Emit(getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getMethod);
				if(property.PropertyType.IsValueType())
					ilGen.Emit(OpCodes.Box, property.PropertyType);
				ilGen.Emit(OpCodes.Ret);
				getter = (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
			}
			return getter;
		}
		static readonly ConcurrentDictionary<Tuple<Type, string>, Tuple<Func<object, object>, Action<object, object>>> accessorsCache = new ConcurrentDictionary<Tuple<Type, string>, Tuple<Func<object, object>, Action<object, object>>>();
		static FastPropertyDescriptor TryCreateFastProperty(PropertyDescriptor source) {
			if(!PropertyDescriptorCriteriaCompilationSupport.IsReflectPropertyDescriptor(source))
				return null;
			if(source is FastPropertyDescriptor)
				return null;	
			var key = Tuple.Create(source.ComponentType, source.Name);
			Tuple<Func<object, object>, Action<object, object>> accessors = accessorsCache.GetOrAdd(key, k => TryCreateAccessors(k.Item1, k.Item2));
			if(accessors == null)
				return null;
			Func<object, object> getter = accessors.Item1;
			if(getter == null)
				getter = (object component) => source.GetValue(component);
			Action<object, object> setter = accessors.Item2;
			if(setter == null)
				setter = (object component, object value) => source.SetValue(component, value);
			return new FastPropertyDescriptor(source, getter, setter);
		}
	}
}
