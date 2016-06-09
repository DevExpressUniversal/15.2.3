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
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.DataAccess.Sql;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public abstract class XRModelBase : BindableBase {
		protected static class XRDefaults<T>
			where T : XRControl, new() {
			static XRDefaults() {
				using(var control = new T()) {
					Size = new Size(control.DefaultWidth, control.DefaultHeight);
				}
			}
			public static Size Size { get; private set; }
			public static Size GetBandWidthSize(XRDiagramControl diagramControl) {
				return new Size(diagramControl.PageSize.Width - diagramControl.RightPadding, Size.Width);
			}
		}
		protected XRModelBase(object xrObject, ModelFactoryData factoryData) {
			Guard.ArgumentNotNull(xrObject, "xrObject");
			InitializeXRObjectIfNeeded(xrObject, factoryData);
			this.xrObject = xrObject;
			Tracker.GetTracker(XRObject, out xrObjectTracker);
		}
		readonly object xrObject;
		public object XRObject { get { return xrObject; } }
		public object XRObjectBase { get { return XRObject; } }
		protected virtual void InitializeXRObjectIfNeeded(object xrObject, ModelFactoryData factoryData) { }
		DiagramItem diagramItem;
		public DiagramItem DiagramItemBase { get { return DiagramItem; } }
		public DiagramItem DiagramItem {
			get {
				if(diagramItem != null)
					return diagramItem;
				diagramItem = CreateDiagramItem();
				SetXRModel(diagramItem, this);
				AttachDiagramItem();
				return diagramItem;
			}
		}
		protected virtual void BeforeDelete(Transaction transaction) { }
		protected virtual void OnAttachItem() { }
		protected virtual void OnDetachItem() { }
		protected abstract DiagramItem CreateDiagramItem();
		protected virtual void AttachDiagramItem() {
			XRDiagramItemBase.SetOnAttachItemCallback(DiagramItem, OnAttachItem);
			XRDiagramItemBase.SetOnDetachItemCallback(DiagramItem, OnDetachItem);
			XRDiagramItemBase.SetBeforeDeleteCallback(DiagramItem, BeforeDelete);
			XRDiagramItemBase.SetEditableProperties(DiagramItem, GetEditableProperties());
		}
		public static readonly DependencyProperty XRModelProperty;
		static readonly DependencyPropertyKey XRModelPropertyKey;
		static XRModelBase() {
			DependencyPropertyRegistrator<XRModelBase>.New()
				.RegisterAttachedReadOnly((DiagramItem d) => GetXRModel(d), out XRModelPropertyKey, out XRModelProperty, null)
			;
		}
		public static XRModelBase GetXRModel(DiagramItem d) { return (XRModelBase)d.GetValue(XRModelProperty); }
		static void SetXRModel(DiagramItem d, XRModelBase v) { d.SetValue(XRModelPropertyKey, v); }
		protected virtual IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return XRProxyPropertyDescriptor.GetXRProxyDescriptors(DiagramItem, DiagramItem, x => GetXRModel(x).XRObject);
		}
		readonly IObjectTracker xrObjectTracker;
		#region XRPropertyModel
		protected XRPropertyModel<TProperty> CreateXRPropertyModel<TProperty, TXRProperty>(Expression<Func<TXRProperty>> xrObjectProperty, Func<TProperty> getValue, Action<TProperty> setValue, TypeConverter typeConverter = null) {
			return CreateXRPropertyModel(GetPropertyName(xrObjectProperty), getValue, setValue, XRObject, null, typeConverter);
		}
		protected XRPropertyModel<TProperty> CreateXRPropertyModel<TXRObject, TProperty, TXRProperty>(Expression<Func<TXRObject, TXRProperty>> xrObjectProperty, Func<TProperty> getValue, Action<TProperty> setValue, TXRObject xrObject, PropertyDescriptor sourceProperty = null, TypeConverter typeConverter = null) where TXRObject : class {
			return CreateXRPropertyModel(ExpressionHelper.GetPropertyName(xrObjectProperty), getValue, setValue, xrObject, sourceProperty, typeConverter);
		}
		protected XRPropertyModel<TProperty> CreateXRPropertyModel<TProperty>(string xrObjectPropertyName, Func<TProperty> getValue, Action<TProperty> setValue, object xrObject, PropertyDescriptor sourceProperty = null, TypeConverter typeConverter = null) {
			if(sourceProperty == null)
				sourceProperty = PropertyDescriptorHelper.GetPropertyDescriptors(xrObject)[xrObjectPropertyName];
			return new XRPropertyModel<TProperty>(xrObject, sourceProperty, xrObjectPropertyName, getValue, setValue, () => DiagramItem.GetXRDiagram(), typeConverter);
		}
		public sealed class XRPropertyModel<TProperty> : INotifyPropertyChanged {
			readonly Func<TProperty> getValue;
			readonly Action<TProperty> setValue;
			readonly PropertyDescriptor sourceProperty;
			readonly IObjectTracker sourceTracker;
			readonly Func<XRDiagramControl> diagramToInvalidate;
			readonly string propertyName;
			readonly TypeConverter typeConverter;
			internal XRPropertyModel(object source, PropertyDescriptor sourceProperty, string propertyName, Func<TProperty> getValue, Action<TProperty> setValue, Func<XRDiagramControl> diagramToInvalidate, TypeConverter typeConverter) {
				this.diagramToInvalidate = diagramToInvalidate;
				this.sourceProperty = sourceProperty;
				this.propertyName = propertyName;
				this.getValue = getValue;
				this.setValue = setValue;
				this.typeConverter = typeConverter;
				var sourceNotifier = source as INotifyPropertyChanged;
				if(sourceNotifier != null) {
					sourceNotifier.PropertyChanged += OnSourcePropertyChanged;
				} else {
					Tracker.GetTracker(source, out sourceTracker);
					sourceTracker.ObjectPropertyChanged += OnSourcePropertyChanged;
				}
			}
			public string SourcePropertyName { get { return sourceProperty.Name; } }
			public TProperty Value {
				get { return getValue(); }
				set { setValue(value); }
			}
			public event EventHandler ValueChanged;
			void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e) {
				if(e.PropertyName == propertyName) {
					if(ValueChanged != null)
						ValueChanged(this, EventArgs.Empty);
					RaisePropertyChanged(() => Value);
				}
				diagramToInvalidate.With(x => x()).Do(x => x.InvalidateRenderLayer());
			}
			public event PropertyChangedEventHandler PropertyChanged;
			void RaisePropertyChanged<T>(Expression<Func<T>> expression) {
				RaisePropertyChanged(BindableBase.GetPropertyName(expression));
			}
			void RaisePropertyChanged(string name) {
				if(PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
			#region ValuePropertyDescriptor
			public PropertyDescriptor GetValuePropertyDescriptor() {
				return new ValuePropertyDescriptor(sourceProperty, typeConverter);
			}
			class ValuePropertyDescriptor : PropertyDescriptorWrapper {
				static readonly new PropertyDescriptor baseDescriptor = TypeDescriptor.GetProperties(typeof(XRPropertyModel<TProperty>))[ExpressionHelper.GetPropertyName((XRPropertyModel<TProperty> x) => x.Value)];
				readonly string propertyName;
				readonly TypeConverter typeConverter;
				public ValuePropertyDescriptor(PropertyDescriptor sourceProperty, TypeConverter typeConverter = null)
					: base(baseDescriptor, sourceProperty.Attributes.Cast<Attribute>().ToArray(), sourceProperty.ComponentType, sourceProperty.Name, typeof(TProperty)) {
					this.propertyName = sourceProperty.Name;
					this.typeConverter = typeConverter;
				}
				public override TypeConverter Converter { get { return typeConverter ?? base.Converter; } }
				public override string Name { get { return propertyName; } }
			}
			#endregion
		}
		#endregion
		#region Bindings
		protected sealed class DiagramItemBindingSource {
			public static DiagramItemBindingSource Create<TProperty>(Expression<Func<TProperty>> modelProperty, object source) {
				return new DiagramItemBindingSource(BindableBase.GetPropertyName(modelProperty), source);
			}
			DiagramItemBindingSource(string property, object source) {
				Property = property;
				Source = source;
			}
			public readonly string Property;
			public readonly object Source;
		}
		protected void BindDiagramItem<TProperty, TDiagramItemProperty>(DependencyProperty diagramItemProperty, Expression<Func<TProperty>> modelProperty, Func<TProperty, TDiagramItemProperty> convertMethod, Func<TDiagramItemProperty, TProperty> convertBackMethod = null) {
			BindDiagramItem(diagramItemProperty, modelProperty, this, convertMethod);
		}
		protected void BindDiagramItem<TProperty, TDiagramItemProperty>(DependencyProperty diagramItemProperty, Expression<Func<TProperty>> modelProperty, object source, Func<TProperty, TDiagramItemProperty> convertMethod, Func<TDiagramItemProperty, TProperty> convertBackMethod = null) {
			BindDiagramItem(diagramItemProperty, modelProperty, source, convertBackMethod == null ? BindingMode.OneWay : BindingMode.TwoWay, GenericValueConverter.Create(convertMethod, convertBackMethod));
		}
		protected void BindDiagramItem<TProperty>(DependencyProperty diagramItemProperty, Expression<Func<TProperty>> modelProperty, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null) {
			BindDiagramItem(diagramItemProperty, modelProperty, this, mode, converter);
		}
		protected void BindDiagramItem<TProperty>(DependencyProperty diagramItemProperty, Expression<Func<TProperty>> modelProperty, object source, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null) {
			var binding = new Binding(GetPropertyName(modelProperty)) { Source = source, Mode = mode, Converter = converter };
			DiagramItem.SetBinding(diagramItemProperty, binding);
		}
		protected void BindDiagramItem(DependencyProperty diagramItemProperty, BindingMode mode, IMultiValueConverter converter, params DiagramItemBindingSource[] sources) {
			var binding = new MultiBinding() { Mode = mode, Converter = converter };
			foreach(var source in sources)
				binding.Bindings.Add(new Binding(source.Property) { Source = source.Source, Mode = mode });
			DiagramItem.SetBinding(diagramItemProperty, binding);
		}
		PropertyChangedEventHandler propertyChangedHandlers = null;
		protected void BindDiagramItemToXRObject<TProperty>(Action bindAction, Expression<Func<TProperty>> modelProperty) {
			string propertyName = GetPropertyName(modelProperty);
			PropertyChangedEventHandler eventHandler = (sender, e) => {
				if(e.PropertyName == propertyName) bindAction();
			};
			propertyChangedHandlers += eventHandler;
			xrObjectTracker.ObjectPropertyChanged += eventHandler;
			bindAction();
		}
		protected static void BindDiagramItems<TSource>(IList<TSource> source, DiagramItemCollection items) where TSource : XRModelBase {
			CollectionBindingHelper.Bind(items, x => x.DiagramItem, source, x => (TSource)GetXRModel(x), true);
		}
		#endregion
	}
}
