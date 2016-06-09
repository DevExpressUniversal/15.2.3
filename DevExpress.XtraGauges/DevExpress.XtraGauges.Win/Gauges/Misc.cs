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
using System.Windows.Forms;
using DevExpress.XtraGauges.Core.Base;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Styles;
using DevExpress.XtraGauges.Base;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Base {
	public interface ISupportPropertyGridWrapper {
		Type PropertyGridWrapperType { get;}
	}
	public interface ISupportVisualDesigning : ISupportPropertyGridWrapper {
		void OnInitDesigner();
		void OnCloseDesigner();
		void RenderDesignerElements(Graphics g);
	}
	[TypeConverter(typeof(ComponentCollectionObjectTypeConverter))]
	[Editor("DevExpress.XtraGauges.Design.ComponentCollectionEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
	public class ComponentCollection<T> : BaseChangeableList<T>
		where T : class, INamed, IComponent, ISupportAcceptOrder {
		protected override void OnElementAdded(T element) {
			base.OnElementAdded(element);
			element.Disposed += OnComponentDisposed;
		}
		protected override void OnElementRemoved(T element) {
			element.Disposed -= OnComponentDisposed;
			base.OnElementRemoved(element);
		}
		void OnComponentDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<T>(sender as T, ElementChangedType.ElementDisposed));
			if(List != null) Remove(sender as T);
		}
	}
	public abstract class BasePropertyGridObjectWrapper {
		ISupportPropertyGridWrapper wrappedObjectCore = null;
		DesignerPropertyGrid propertyGridCore = null;
		public void SetWrappedObject(ISupportPropertyGridWrapper wrappedObject) {
			this.wrappedObjectCore = wrappedObject;
		}
		public void SetPropertyGrid(DesignerPropertyGrid propertyGrid) {
			this.propertyGridCore = propertyGrid;
		}
		protected ISupportPropertyGridWrapper WrappedObject {
			get { return wrappedObjectCore; }
		}
		protected internal DesignerPropertyGrid PropertyGrid {
			get { return propertyGridCore; }
		}
	}
	public class BindableObjectPropertyGridWrapper : BasePropertyGridObjectWrapper {
		protected IBindableComponent Component {
			get { return WrappedObject as IBindableComponent; }
		}
		[System.ComponentModel.Category("Name")]
		public string Name {
			get { return (Component.Site!=null) ? Component.Site.Name : null; }
		}
		[System.ComponentModel.Category("Data")]
		public ControlBindingsCollection DataBindings { 
			get { return Component.DataBindings; } 
		}
	}
	public class ComponentCollectionObjectTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			return "<Components...>";
		}
	}
	public abstract class BaseComponentWrapperTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
			if(CanFilterComponentProperties(value as BasePropertyGridObjectWrapper)) {
				properties = FilterPropertiesHelper.FilterProperties(properties, GetExpectedProperties(properties));
			}
			return properties;
		}
		protected virtual bool CanFilterComponentProperties(BasePropertyGridObjectWrapper wrapper) {
			return wrapper != null && (
					wrapper.PropertyGrid == null
					|| wrapper.PropertyGrid.ServiceProvider == null
					|| wrapper.PropertyGrid.ServiceProvider.Site == null
				);
		}
		protected abstract string[] GetPropertiesToRemove();
		protected string[] GetExpectedProperties(PropertyDescriptorCollection properties) {
			List<string> result = new List<string>();
			List<string> toRemove = new List<string>(GetPropertiesToRemove());
			foreach(PropertyDescriptor pd in properties) {
				if(!toRemove.Contains(pd.Name)) result.Add(pd.Name);
			}
			return result.ToArray();
		}
	}
	public class ScaleComponentWrapperTypeConverter : BaseComponentWrapperTypeConverter {
		protected override string[] GetPropertiesToRemove() {
			return new string[] { "ArcScale", "LinearScale", "IndicatorScale" };
		}
	}
}
