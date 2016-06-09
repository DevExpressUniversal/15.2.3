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
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.ComponentModel.Design;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.Utils.Design {
	public class UniversalTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) return true;
			if(destinationType.Equals(typeof(byte[]))) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType.Equals(typeof(byte[]))) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		protected virtual Type GetObjectType(object val) {
			return val.GetType();
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is byte[]) return BinaryTypeConverter.ConvertFromBytes(value as byte[]);
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if(destinationType.Equals(typeof(byte[]))) return BinaryTypeConverter.ConvertToBytes(value);
			if(destinationType == typeof(InstanceDescriptor) && value != null) {
				Type ctorType = GetObjectType(value);
				ConstructorInfo ctor = ctorType.GetConstructor(new Type[] { });
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
				int serCount = 0;
				List<PropertyDescriptor> list = null;
				foreach(PropertyDescriptor pd in properties) {
					if(pd.SerializationVisibility != DesignerSerializationVisibility.Hidden && pd.ShouldSerializeValue(value)) {
						if (list == null) list = new List<PropertyDescriptor>();
						list.Add(pd);
						serCount++;
					}
				}
				object[] parameters = null;
				if(serCount > 0) {
					ctor = FindConstructor(properties, ctor, GetConstructors(ctorType), list);
					parameters = GenerateParameters(properties, ctor, value);
				}
				if(ctor != null)
					return new InstanceDescriptor(ctor, parameters);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected ConstructorInfo[] GetConstructors(Type ctorType) {
			return FilterConstructors(ctorType.GetConstructors());
		}
		protected virtual ConstructorInfo[] FilterConstructors(ConstructorInfo[] ctors) {
			return ctors;
		}
		protected string ExtractPropertyName(string valName) {
			string propertyName = valName;
			if(propertyName.StartsWith("_")) propertyName = propertyName.Substring(1);
			if(propertyName.Length > 0) {
				propertyName = propertyName.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture) + propertyName.Substring(1);
			}
			return propertyName;
		}
		protected virtual object[] GenerateParameters(PropertyDescriptorCollection properties, ConstructorInfo ctor, object val) {
			ParameterInfo[] pars = ctor.GetParameters();
			if(val == null || pars.Length == 0) return null;
			object[] res = new object[pars.Length];
			for(int n = 0; n < pars.Length; n++) {
				ParameterInfo par = pars[n];
				string propertyName = ExtractPropertyName(par.Name);
				PropertyDescriptor pd = properties[propertyName];
				if(pd == null) throw new WarningException("[UniConstructor]: Can't find property " + propertyName);
				res[n] = pd.GetValue(val);
			}
			return res;
		}
		protected virtual ConstructorInfo FindConstructor(PropertyDescriptorCollection properties, ConstructorInfo empty, ConstructorInfo[] ctors, List<PropertyDescriptor> list) {
			if(ctors == null || ctors.Length == 0 || list == null || list.Count == 0) return empty;
			ConstructorInfo res = null;
			int resParCount = -1;
			foreach(ConstructorInfo ctor in ctors) {
				ParameterInfo[] pars = ctor.GetParameters();
				bool valid = true;
				foreach(PropertyDescriptor pd in list) {
					if(!CheckParameter(pd.PropertyType, pd.Name, pars)) {
						valid = false;
						break;
					}
				}
				if(valid) {
					if(pars.Length == list.Count) return ctor; 
					if(res == null || resParCount > pars.Length) {
						foreach(ParameterInfo par in pars) {
							string propertyName = ExtractPropertyName(par.Name);
							PropertyDescriptor pd = properties[propertyName];
							if(pd == null) {
								valid = false;
								break;
							}
						}
						if(valid) {
							res = ctor;
							resParCount = pars.Length;
						}
						continue;
					}
				}
			}
			if(res == null) res = empty; 
			return res;
		}
		protected virtual bool CheckParameter(Type propertyType, string propertyName, ParameterInfo[] pars) {
			if(propertyName.Length > 0) propertyName = propertyName.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + propertyName.Substring(1);
			foreach(ParameterInfo par in pars) {
				if(par.Name == propertyName || par.Name == "_" + propertyName) {
					if(par.ParameterType.Equals(propertyType)) return true;
				}
			}
			return false;
		}
		public static void ResetObject(object checkObject) {
			if(checkObject == null) return;
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(checkObject);
			if(pdColl == null || pdColl.Count == 0) return;
			foreach(PropertyDescriptor pd in pdColl) {
				if(pd.SerializationVisibility == DesignerSerializationVisibility.Hidden) continue;
				pd.ResetValue(checkObject);
			}
		}
		public static bool ShouldSerializeObject(object checkObject, IComponent owner) {
			IInheritanceService service = owner == null || owner.Site == null ? null : owner.Site.GetService(typeof(IInheritanceService)) as IInheritanceService;
			if(service != null) {
				InheritanceAttribute attr = service.GetInheritanceAttribute(owner);
				if(attr != null && attr.InheritanceLevel != InheritanceLevel.NotInherited) return true;
			}
			return ShouldSerializeObject(checkObject);
		}
		public static bool ShouldSerializeObject(object checkObject) {
			if(checkObject == null) return false;
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(checkObject);
			if(pdColl == null || pdColl.Count == 0) return false;
			foreach(PropertyDescriptor pd in pdColl) {
				if(pd.SerializationVisibility == DesignerSerializationVisibility.Content) {
					if(ShouldSerializeObject(pd.GetValue(checkObject))) return true;
					continue;
				}
				if(pd.SerializationVisibility == DesignerSerializationVisibility.Hidden) continue;
				if(pd.ShouldSerializeValue(checkObject)) return true;
			}
			return false;
		}
	}
}
