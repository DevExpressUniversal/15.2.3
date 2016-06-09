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

namespace DevExpress.Diagram.Core {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Globalization;
	using System.Diagnostics;
	using DevExpress.Utils;
	using DevExpress.Internal;
	using System.Linq.Expressions;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Runtime.CompilerServices;
	using System.Windows.Media;
	public static class EditCollectionHelper {
		public sealed class StandaloneCollectionModel<TRoot, TList, TItem> : CastList<IMultiModel, IMultiModel>, INotifyCollectionChanged where TList : IList<TItem> {
			CollectionModel<TRoot, TList, TItem> Model { get { return (CollectionModel<TRoot, TList, TItem>)keys; } }
			readonly PropertiesProvider<TRoot, TList> provider;
			readonly Transaction transaction;
			readonly Func<TRoot, IItemFinder<TRoot>> getFinder;
			internal StandaloneCollectionModel(PropertiesProvider<TRoot, TList> provider, Transaction transaction, Func<TRoot, IItemFinder<TRoot>> getFinder)
				: base(new CollectionModel<TRoot, TList, TItem>(provider)) {
				this.provider = provider;
				this.transaction = transaction;
				this.getFinder = getFinder;
			}
			public event NotifyCollectionChangedEventHandler CollectionChanged;
			void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
				var handler = CollectionChanged;
				if(handler != null)
					handler(this, e);
			}
			public TItem RemoveAt(int index) {
				var item = GetListFinder().FindItem()[index];
				((IList)this).RemoveAt(index);
				return item;
			}
			public void Add(TItem item) {
				Insert(keys.Count, item);
			}
			public void Insert(int index, TItem item) {
				AddRemoveActions2.InsertItem(transaction, GetListFinder(), item, index);
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, keys[index], index));
			}
			protected override void ClearCore() {
				var listFinder = GetListFinder();
				var list = listFinder.FindItem();
				while(list.Any())
					AddRemoveActions2.RemoveItem(transaction, listFinder, list.Count - 1);
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			protected override bool IsFixedSize { get { return false; } }
			protected override void RemoveAtCore(int index) {
				var item = keys[index];
				AddRemoveActions2.RemoveItem(transaction, GetListFinder(), index);
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			}
			IItemFinder<IList<TItem>> GetListFinder() {
				var getComponent = provider.Context.GetComponent;
				return new CompositeFinder<TRoot, IList<TItem>>(getFinder(provider.MainComponent), x => getComponent(x));
			}
		}
		public static void EditCollection<TRoot, TList, TItem>(
			TRoot item,
			Func<TRoot, TList> getList,
			Transaction transaction,
			MultiModelContext<TRoot, TList> context,
			Func<TRoot, IItemFinder<TRoot>> getFinder,
			Func<StandaloneCollectionModel<TRoot, TList, TItem>, bool> editAction
		) where TList : IList<TItem> {
				var provider = new PropertiesProvider<TRoot, TList>(item, item.Yield().ToArray(), context);
				var model = new StandaloneCollectionModel<TRoot, TList, TItem>(provider, transaction, getFinder);
				var result = editAction(model);
				if(!result)
					transaction.Rollback();
		}
	}
	public sealed class CollectionModel<TRoot, TList, TItem> : BridgeList<IMultiModel, TItem>, IMultiModel where TList : IList<TItem> {
		readonly PropertiesProvider<TRoot, TList> propertiesProvider;
		public CollectionModel(PropertiesProvider<TRoot, TList> propertiesProvider)
			: base(propertiesProvider.Values.Single()) {
			this.propertiesProvider = propertiesProvider;
		}
		IPropertiesProvider IMultiModel.PropertiesProvider { get { return propertiesProvider; } }
		protected override IMultiModel GetItemByKey(TItem key, int index) {
			return GetItem(index);
		}
		ConditionalWeakTable<object, IMultiModel> cache = new ConditionalWeakTable<object, IMultiModel>();
		IMultiModel GetItem(int index) {
			return cache.GetValue(keys[index], key => {
				return GetItemCore(index);
			});
		}
		private IMultiModel GetItemCore(int index) {
			var component = keys[index];
			return MultiPropertyHelper.CreateMultimodel(propertiesProvider.MainComponent, propertiesProvider.Components,
							propertiesProvider.Context.ChangeType(x => component));
		}
		public override string ToString() {
			return propertiesProvider.ToString();
		}
	}
	[DebuggerDisplay("{GetType()}")]
	public sealed class SelectionModel<T> : CustomTypeDescriptorBase, INotifyPropertyChanged, IRaisePropertyChangedMultiModel {
		internal static void RaisePropertyChanged(object sender, PropertyChangedEventHandler handler, string propertyName = "") {
			if(handler != null)
				handler(sender, new PropertyChangedEventArgs(propertyName));
		}
		readonly IPropertiesProvider propertiesProvider;
		public SelectionModel(IPropertiesProvider multiproperties) {
			this.propertiesProvider = multiproperties;
		}
		event PropertyChangedEventHandler propertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		public void RaisePropertyChanged(string propertyName = "") {
			RaisePropertyChanged(this, propertyChanged, propertyName);
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			return propertiesProvider.Properties;
		}
		IPropertiesProvider IMultiModel.PropertiesProvider { get { return propertiesProvider; } }
		void IRaisePropertyChangedMultiModel.RaisePropertyChanged(string propertyName) {
			RaisePropertyChanged(propertyName);
		}
		public override string ToString() {
			return propertiesProvider.ToString();
		}
		public TProperty GetPropertyValueEx<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> property) {
			return (TProperty)propertiesProvider.Properties[ExpressionHelper.GetPropertyName(property)].GetValue(this);
		}
		public void SetPropertyValueEx<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> property, TProperty value) {
			propertiesProvider.Properties[ExpressionHelper.GetPropertyName(property)].SetValue(this, value);
		}
		public SelectionModel<TProperty> GetPropertyModel<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> property) {
			return (SelectionModel<TProperty>)propertiesProvider.Properties[ExpressionHelper.GetPropertyName(property)].GetValue(this);
		}
	}
	public static class SelectionModelExtension {
		public static void SetPropertyValue<TOwner, TProperty>(this SelectionModel<TOwner> selectionModel, Expression<Func<TOwner, TProperty>> property, TProperty value) {
			selectionModel.SetPropertyValueEx(property, value);
		}
		public static TProperty GetPropertyValue<TOwner, TProperty>(this SelectionModel<TOwner> selectionModel, Expression<Func<TOwner, TProperty>> property) {
			return (TProperty)selectionModel.GetPropertyValueEx(property);
		}
	}
	public sealed class SelectionToolsModel<T> : INotifyPropertyChanged, IMultiModel {
		readonly IPropertiesProvider propertiesProvider;
		public SelectionToolsModel(IPropertiesProvider multiproperties) {
			this.propertiesProvider = multiproperties;
		}
		public object this[string propertyName] {
			get { return propertiesProvider.Properties[propertyName].With(x => x.GetValue(this)); }
			set { propertiesProvider.Properties[propertyName].Do(x => x.SetValue(this, value)); }
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertiesChanged() {
			SelectionModel<T>.RaisePropertyChanged(this, PropertyChanged);
		}
		IPropertiesProvider IMultiModel.PropertiesProvider { get { return propertiesProvider; } }
	}
	public interface IMultiModel {
		IPropertiesProvider PropertiesProvider { get; }
	}
	public interface IRaisePropertyChangedMultiModel : IMultiModel {
		void RaisePropertyChanged(string propertyName);
	}
	public interface IPropertiesProvider {
		PropertyDescriptorCollection Properties { get; }
		IEnumerable<object> Values { get; }
	}
	public sealed class MultiModelContext<TRoot, T> {
		readonly Func<TRoot, T> getComponent;
		readonly Func<TRoot, IItemFinder<TRoot>> getFinder;
		readonly Type modelType;
		readonly Type nextModelType;
		readonly Action<Action<Transaction>> executeTransactionAction;
		readonly Func<object, ITypeDescriptorContext, TypeConverter, Attribute[], IEnumerable<PropertyDescriptor>> getProperties;
		public MultiModelContext(Func<TRoot, IItemFinder<TRoot>> getFinder, Func<TRoot, T> getComponent, Type modelType, Type nextModelType, Action<Action<Transaction>> executeTransactionAction, Func<object, ITypeDescriptorContext, TypeConverter, Attribute[], IEnumerable<PropertyDescriptor>> getProperties) {
			this.getComponent = getComponent;
			this.getFinder = getFinder;
			this.modelType = modelType;
			this.nextModelType = nextModelType;
			this.executeTransactionAction = executeTransactionAction;
			this.getProperties = getProperties;
		}
		public void SetValues(IEnumerable<TRoot> components, PropertyDescriptor property, IEnumerable<object> values) {
			executeTransactionAction(transaction => {
				transaction.SetMultipleItemsPropertyValues(components, property, values, getFinder, getComponent);
			});
		}
		public void ResetValue(IEnumerable<TRoot> components, PropertyDescriptor property) {
			executeTransactionAction(transaction => {
				transaction.ResetMultipleItemsPropertyValues(components, property, getFinder, getComponent);
			});
		}
		public IEnumerable<PropertyDescriptor> GetProperties(T component, ITypeDescriptorContext descriptorContext, TypeConverter converter, Attribute[] attributes) {
			return getProperties(component, descriptorContext, converter, attributes);
		}
		public MultiModelContext<TRoot, TNew> ChangeType<TNew>(Func<TRoot, TNew> getComponentNew) {
			return new MultiModelContext<TRoot, TNew>(getFinder, getComponentNew, nextModelType, nextModelType, executeTransactionAction, getProperties);
		}
		public Type GetModelType() {
			return modelType.MakeGenericType(typeof(T));
		}
		public Func<TRoot, T> GetComponent { get { return getComponent; } }
	}
	public sealed class PropertiesProvider<TRoot, T> : IPropertiesProvider {
		public readonly TRoot MainComponent;
		public readonly TRoot[] Components;
		readonly Lazy<PropertyDescriptorCollection> properties;
		public readonly MultiModelContext<TRoot, T> Context;
		public PropertiesProvider(TRoot mainComponent, TRoot[] components, MultiModelContext<TRoot, T> context) {
			this.MainComponent = mainComponent;
			this.Components = components;
			this.properties = new Lazy<PropertyDescriptorCollection>(() => GetProperties(x => null, null, null));
			this.Context = context;
		}
		T Value { get { return Context.GetComponent(MainComponent); } }
		internal IEnumerable<T> Values { get { return Components.Select(Context.GetComponent); } }
		public PropertyDescriptorCollection Properties { get { return properties.Value; } }
		IEnumerable<object> IPropertiesProvider.Values { get { return Values.Cast<object>(); } }
		public PropertyDescriptorCollection GetProperties(Func<TRoot, ITypeDescriptorContext> getDescriptorContext, TypeConverter converter, Attribute[] attributes) {
			var propertiesMap = Components.Select(x => Context.GetProperties(Context.GetComponent(x), getDescriptorContext(x), converter, attributes));
			var sharedProperties = propertiesMap.Skip(1).Aggregate(propertiesMap.First(), (total, x) => total.Intersect(x, PropertyDescriptorHelper.Comparer)).ToArray();
			return new PropertyDescriptorCollection(sharedProperties.Select(x => MultiPropertyHelper.CreateMultiPropertyDescriptor(x, Context, Context.GetComponent(MainComponent))).ToArray());
		}
		public override string ToString() {
			return Values.AllEqual() ? Value.ToString() : null;
		}
	}
	public static class MultiPropertyHelper {
		class ValueSetter<TProperty> : IPropertiesProvider {
			readonly TProperty[] values;
			public ValueSetter(TProperty[] values) {
				this.values = values;
			}
			PropertyDescriptorCollection IPropertiesProvider.Properties { get { throw new NotImplementedException(); } }
			IEnumerable<object> IPropertiesProvider.Values { get { return values.Cast<object>(); } }
			public override string ToString() {
				return values.First().ToString();
			}
		}
		class MultiPropertyDescriptor<TRoot, T> : PropertyDescriptorWrapper {
			protected class MultiPropertyTypeConverter : TypeConverterWrapper {
				protected readonly MultiPropertyDescriptor<TRoot, T> property;
				protected override TypeConverter BaseConverter { get { return property.baseDescriptor.Converter; } }
				protected override ITypeDescriptorContext GetWrapperContext(ITypeDescriptorContext context) {
					return context == null ? null : new ProxyTypeDescriptorContext<IMultiModel, T>(context, model => {
						return property.GetRealComponentCore(model);
					}, property.baseDescriptor);
				}
				public MultiPropertyTypeConverter(MultiPropertyDescriptor<TRoot, T> property) {
					this.property = property;
				}
				public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
					return base.ConvertTo(context, culture, property.GetOriginalValue(context.Instance, value), destinationType);
				}
				public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
					return property.GetCustomProperties(value, context, BaseConverter, attributes) ?? 
						base.GetProperties(context, value, attributes);
				}
				public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
					return property.ConvertFrom(context, culture, value, BaseConverter.ConvertFrom);
				}
				public override bool IsValid(ITypeDescriptorContext context, object value) {
					return property.GetOriginalValues(context.Instance, value).All(x => base.IsValid(context, x));
				}
				public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
					return property.GetStandardValues(context, BaseConverter.GetStandardValues);
				}
			}
			protected readonly MultiModelContext<TRoot, T> context;
			public MultiPropertyDescriptor(PropertyDescriptor baseDescriptor, MultiModelContext<TRoot, T> context) 
				: base(baseDescriptor, true) {
				this.context = context;
			}
			public override bool SupportsChangeEvents { get { return false; } }
			public override void AddValueChanged(object component, EventHandler handler) {
				base.AddValueChanged(component, handler);
			}
			public override Type ComponentType { get { return context.GetModelType(); } }
			public override TypeConverter Converter { get { return new MultiPropertyTypeConverter(this); } }
			public override bool CanResetValue(object componentModel) {
				var components = GetComponents(componentModel);
				return components.Where(x => baseDescriptor.CanResetValue(x)).Any();
			}
			public override object GetValue(object component) {
				var model = GetModel(component);
				var baseValues = model.Values.Select(x => baseDescriptor.GetValue(x));
				return baseValues.AllEqual() ? baseValues.First() : null;
			}
			public override void ResetValue(object componentModel) {
				var model = GetModel(componentModel);
				context.ResetValue(model.Components.Where(x => baseDescriptor.CanResetValue(context.GetComponent(x))), baseDescriptor);
				(componentModel as IRaisePropertyChangedMultiModel).Do(x => x.RaisePropertyChanged(Name));
			}
			public sealed override void SetValue(object componentModel, object value) {
				var model = GetModel(componentModel);
				context.SetValues(model.Components, baseDescriptor, GetOriginalValues(componentModel, value));
				(componentModel as IRaisePropertyChangedMultiModel).Do(x => x.RaisePropertyChanged(Name));
			}
			public override bool ShouldSerializeValue(object componentModel) {
				var components = GetComponents(componentModel);
				return components.Where(x => baseDescriptor.ShouldSerializeValue(x)).Any();
			}
			protected static PropertiesProvider<TRoot, T> GetModel(object componentModel) {
				return GetModel<T>(componentModel);
			}
			protected static PropertiesProvider<TRoot, TModel> GetModel<TModel>(object componentModel) {
				return (PropertiesProvider<TRoot, TModel>)((IMultiModel)componentModel).PropertiesProvider;
			}
			IEnumerable<T> GetComponents(object componentModel) {
				return GetModel(componentModel).Components.Select(x => context.GetComponent(x));
			}
			protected sealed override object GetRealComponent(object component) {
				return GetRealComponentCore(component);
			}
			T GetRealComponentCore(object component) {
				return context.GetComponent(GetModel(component).MainComponent);
			}
			object GetOriginalValue(object component, object value) {
				return GetOriginalValues(component, value).First();
			}
			IEnumerable<object> GetOriginalValues(object component, object value) {
				var model = GetModel(component);
				return GetOriginalValuesCore(model, value);
			}
			protected virtual IEnumerable<object> GetOriginalValuesCore(PropertiesProvider<TRoot, T> model, object value) {
				return Enumerable.Repeat(value, model.Components.Length);
			}
			protected virtual PropertyDescriptorCollection GetCustomProperties(object value, ITypeDescriptorContext context, TypeConverter converter, Attribute[] attributes) {
				return null;
			}
			protected virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value, Func<ITypeDescriptorContext, CultureInfo, object, object> convert) {
				var realValue = convert(ChangeContext(context, GetModel(context.Instance).MainComponent), culture, value);
				return realValue;
			}
			TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context, Func<ITypeDescriptorContext, TypeConverter.StandardValuesCollection> getStandardValues) {
				var model = GetModel(context.Instance);
				var realStandardValuesArray = model.Components.With(components => components
				   .Select(rootComponent => getStandardValues(ChangeContext(context, rootComponent)))
				   .ToArray()
				);
				var allCollectionsEqual = realStandardValuesArray.AllEqual((x, y) => {
					return x.Count == y.Count
						&& Enumerable.Zip(x.Cast<object>(), y.Cast<object>(), (item1, item2) => new { item1, item2 })
							.All(pair => Equals(pair.item1, pair.item2));
				});
				if(!allCollectionsEqual)
					return new TypeConverter.StandardValuesCollection(new object[0]);
				return WrapStandardValues(model, realStandardValuesArray.First());
			}
			protected virtual TypeConverter.StandardValuesCollection WrapStandardValues(PropertiesProvider<TRoot, T> model, TypeConverter.StandardValuesCollection realValues) {
				return realValues;
			}
			protected ITypeDescriptorContext ChangeContext(ITypeDescriptorContext context, TRoot rootComponent) {
				return context.Change(this.context.GetComponent(rootComponent), baseDescriptor);
			}
		}
		abstract class MultiExpandableDescriptorBase<TRoot, T, TProperty> : MultiPropertyDescriptor<TRoot, T> {
			protected readonly MultiModelContext<TRoot, TProperty> nestedContext;
			public MultiExpandableDescriptorBase(PropertyDescriptor baseDescriptor, MultiModelContext<TRoot, T> context) 
				: base(baseDescriptor, context) {
				this.nestedContext = context.ChangeType(x => GetNested(context.GetComponent(x)));
			}
			protected sealed override IEnumerable<object> GetOriginalValuesCore(PropertiesProvider<TRoot, T> model, object value) {
				return value.With(x => ((IMultiModel)x).PropertiesProvider.Values) ?? Enumerable.Repeat<object>(null, model.Components.Length);
			}
			protected sealed override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value, Func<ITypeDescriptorContext, CultureInfo, object, object> convert) {
				var model = GetModel(context.Instance);
				var realValues = model.Components.With(components => components
					.Select(rootComponent => (TProperty)convert(ChangeContext(context, rootComponent), culture, value))
					.ToArray()
				);
				if(realValues.All(x => x == null))
					return null;
				return CreateMultimodel<TRoot, TProperty>(nestedContext, new ValueSetter<TProperty>(realValues));
			}
			protected sealed override TypeConverter.StandardValuesCollection WrapStandardValues(PropertiesProvider<TRoot, T> model, TypeConverter.StandardValuesCollection realValues) {
				var wrappedValues = realValues
					.Cast<TProperty>()
					.Select(x => CreateMultimodel<TRoot, TProperty>(nestedContext, new ValueSetter<TProperty>(Enumerable.Repeat(x, model.Components.Length).ToArray())))
					.ToArray();
				return new TypeConverter.StandardValuesCollection(wrappedValues);
			}
			public sealed override object GetValue(object component) {
				var model = GetModel(component);
				return nestedContext.GetComponent(model.MainComponent) != null ?
					CreateMultimodelCore(model.MainComponent, model.Components)
					: null;
			}
			protected abstract object CreateMultimodelCore(TRoot mainComponent, TRoot[] components);
			TProperty GetNested(T value) {
				return (TProperty)baseDescriptor.GetValue(value);
			}
		}
		sealed class MultiPropertyExpandableDescriptor<TRoot, T, TProperty> : MultiExpandableDescriptorBase<TRoot, T, TProperty> {
			public MultiPropertyExpandableDescriptor(PropertyDescriptor baseDescriptor, MultiModelContext<TRoot, T> context) 
				: base(baseDescriptor, context) {
			}
			public override Type PropertyType { get { return nestedContext.GetModelType(); } }
			protected override object CreateMultimodelCore(TRoot mainComponent, TRoot[] components) {
				return CreateMultimodel(mainComponent, components, nestedContext);
			}
			protected override PropertyDescriptorCollection GetCustomProperties(object value, ITypeDescriptorContext context, TypeConverter converter, Attribute[] attributes) {
				var model = GetModel<TProperty>(value);
				return model.GetProperties(rootComponent => ChangeContext(context, rootComponent), converter, attributes);
			}
		}
		sealed class MultiCollectionExpandableDescriptor<TRoot, T, TList, TItem> : MultiExpandableDescriptorBase<TRoot, T, TList> where TList : IList<TItem> {
			public MultiCollectionExpandableDescriptor(PropertyDescriptor baseDescriptor, MultiModelContext<TRoot, T> context) : base(baseDescriptor, context) {
			}
			public override Type PropertyType { get { return typeof(CollectionModel<TRoot, TList, TItem>); } }
			protected override object CreateMultimodelCore(TRoot mainComponent, TRoot[] components) {
				return !components.Skip(1).Any() 
					? new CollectionModel<TRoot, TList, TItem>(new PropertiesProvider<TRoot, TList>(mainComponent, components, nestedContext)) 
					: null;
			}
			protected override PropertyDescriptorCollection GetCustomProperties(object value, ITypeDescriptorContext context, TypeConverter converter, Attribute[] attributes) {
				return PropertyDescriptorCollection.Empty;
			}
		}
		public static IMultiModel CreateMultimodel<TRoot, T>(TRoot mainComponent, IEnumerable<TRoot> components, MultiModelContext<TRoot, T> context) {
			return CreateMultimodel(context, new PropertiesProvider<TRoot, T>(mainComponent, components.ToArray(), context));
		}
		static IMultiModel CreateMultimodel<TRoot, T>(MultiModelContext<TRoot, T> context, IPropertiesProvider provider) {
			return (IMultiModel)Activator.CreateInstance(context.GetModelType(), provider);
		}
		static Type MakeModelGenericType<T>(Type modelType) {
			return modelType.MakeGenericType(typeof(T));
		}
		internal static PropertyDescriptor CreateMultiPropertyDescriptor<TRoot, T>(PropertyDescriptor descriptor, MultiModelContext<TRoot, T> context, T mainComponent) {
			var listArgument = PropertyDescriptorHelper.GetListGenericParameter(descriptor);
			if(listArgument != null) {
				var type = typeof(MultiCollectionExpandableDescriptor<,,,>).MakeGenericType(typeof(TRoot), typeof(T), descriptor.PropertyType, listArgument);
				return (MultiPropertyDescriptor<TRoot, T>)Activator.CreateInstance(type, descriptor, context);
			}
			if(PropertyDescriptorHelper.IsComplexProperty(descriptor, mainComponent))
				return CreateMultiPropertyExpandableDescriptor(descriptor, context);
			return new MultiPropertyDescriptor<TRoot, T>(descriptor, context);
		}
		static MultiPropertyDescriptor<TRoot, T> CreateMultiPropertyExpandableDescriptor<TRoot, T>(PropertyDescriptor descriptor, MultiModelContext<TRoot, T> context) {
			var type = typeof(MultiPropertyExpandableDescriptor<,,>).MakeGenericType(typeof(TRoot), typeof(T), descriptor.PropertyType);
			return (MultiPropertyDescriptor<TRoot, T>)Activator.CreateInstance(type, descriptor, context);
		}
	}
	public class DiagramItemEditUnit {
		#region Static Properties
		public static readonly PropertyDescriptor StyleIdProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.ThemeStyleId);
		public static readonly PropertyDescriptor BackgroundProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.Background);
		public static readonly PropertyDescriptor BackgroundIdProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.BackgroundId);
		public static readonly PropertyDescriptor ForegroundProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.Foreground);
		public static readonly PropertyDescriptor ForegroundIdProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.ForegroundId);
		public static readonly PropertyDescriptor StrokeProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.Stroke);
		public static readonly PropertyDescriptor StrokeIdProperty = ExpressionHelper.GetProperty((IDiagramItem item) => item.StrokeId);
		public static readonly PropertyDescriptor[] StyleProperties = new PropertyDescriptor[] {
			BackgroundProperty,
			BackgroundIdProperty,
			ForegroundProperty,
			ForegroundIdProperty,
			StrokeProperty,
			StrokeIdProperty,
			StyleIdProperty,
			ExpressionHelper.GetProperty((IDiagramItem x) => x.FontSize),
			ExpressionHelper.GetProperty((IDiagramItem x) => x.FontWeight),
			ExpressionHelper.GetProperty((IDiagramItem x) => x.FontStyle),
			ExpressionHelper.GetProperty((IDiagramItem x) => x.TextDecorations),
			ExpressionHelper.GetProperty((IDiagramItem x) => x.StrokeDashArray),
			ExpressionHelper.GetProperty((IDiagramItem x) => x.StrokeThickness),
		};
		public static readonly PropertyDescriptor[] BackgroundProperties = new PropertyDescriptor[] { BackgroundProperty, BackgroundIdProperty };
		public static readonly PropertyDescriptor[] ForegroundProperties = new PropertyDescriptor[] { ForegroundProperty, ForegroundIdProperty };
		public static readonly PropertyDescriptor[] StrokeProperties = new PropertyDescriptor[] { StrokeProperty, StrokeIdProperty };
		#endregion
		#region Factory Methods
		public static DiagramItemEditUnit CreateStyleEditUnit(IDiagramItem item) {
			return CreateEditUnit(item, StyleProperties);
		}
		public static DiagramItemEditUnit CreateStyleEditUnit(DiagramItemStyleId styleId) {
			return new DiagramItemEditUnit(new KeyValuePair<PropertyDescriptor, object>(StyleIdProperty, styleId).Yield().ToArray());
		}
		public static DiagramItemEditUnit CreateEditUnit(IDiagramItem item, PropertyDescriptor[] properties) {
			var modifiedProperties = properties.Where(pd => item.GetRealProperty(pd).ShouldSerializeValue(item));
			var setters = modifiedProperties.Select(x => new KeyValuePair<PropertyDescriptor, object>(x, x.GetValue(item))).ToArray();
			return new DiagramItemEditUnit(setters);
		}
		public static DiagramItemEditUnit CreateEditUnit(PropertyDescriptor pd, object value) {
			return new DiagramItemEditUnit(new KeyValuePair<PropertyDescriptor, object>(pd, value).Yield().ToArray());
		}
		#endregion
		#region Static Methods
		internal static void ApplyStyleEditUnit(IDiagramItem item, DiagramItemEditUnit editUnit) {
			ApplyEditUnit(item, editUnit, StyleProperties);
		}
		public static void ApplyEditUnit(IDiagramItem item, DiagramItemEditUnit editUnit, PropertyDescriptor[] editableProperties) {
			editableProperties.Select(pd => item.GetRealProperty(pd)).ForEach(x => x.ResetValue(item));
			editUnit.Setters.ForEach(x => x.Key.SetValue(item, x.Value));
		}
		#endregion
		readonly Dictionary<PropertyDescriptor, object> setters;
		public IEnumerable<KeyValuePair<PropertyDescriptor, object>> Setters { get { return setters; } }
		public DiagramItemStyleId StyleId { get { return (DiagramItemStyleId)GetValue(StyleIdProperty); } }
		public DiagramItemEditUnit(IEnumerable<KeyValuePair<PropertyDescriptor, object>> setters) {
			this.setters = new Dictionary<PropertyDescriptor, object>();
			setters.ForEach(setter => this.setters.Add(setter.Key, setter.Value));
		}
		public object GetValue(PropertyDescriptor pd) {
			object value = null;
			setters.TryGetValue(pd, out value);
			return value;
		}
		public bool Contains(PropertyDescriptor pd) {
			return setters.ContainsKey(pd);
		}
		public override bool Equals(object obj) {
			DiagramItemEditUnit editUnit2 = obj as DiagramItemEditUnit;
			if(ReferenceEquals(editUnit2, null) || setters.Count != editUnit2.setters.Count)
				return false;
			return Setters.Aggregate(true, (areEqual, setter) => areEqual && ReferenceEquals(editUnit2.GetValue(setter.Key), setter.Value));
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode(setters.SelectMany(x => new object[] { x.Key, x.Value }).ToArray());
		}
	}
}
