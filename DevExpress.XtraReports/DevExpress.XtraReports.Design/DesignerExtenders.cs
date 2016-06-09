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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Design;
using System.Globalization;
using System.Reflection;
namespace DevExpress.XtraReports.Design
{
	public class DesignerExtenders : IDisposable 
	{
		#region inner classes
		[ProvideProperty("Modifiers", typeof(IComponent))]
		class ModifiersInheritedExtenderProvider : IExtenderProvider 
		{
			protected IComponent baseComponent;
			protected virtual bool ShouldExtendInheritedObject() {
				return true;
			}
			protected virtual bool ShouldExtendNotInheritedObject() {
				return false;
			}
			[
			DesignOnly(true),
			TypeConverter(typeof(ModifierConverter)),
			DefaultValue(MemberAttributes.Private),
			Category("Design")
			]
			public virtual MemberAttributes GetModifiers(IComponent comp) {
				System.Diagnostics.Debug.Assert(baseComponent != null);
				Type baseType = baseComponent.GetType();
				ISite site = comp.Site;
				if (site != null) {
					string name = site.Name;
					if (name != null) {
						FieldInfo field = baseType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
						if (field != null) {
							if (field.IsPrivate) return MemberAttributes.Private;
							if (field.IsPublic) return MemberAttributes.Public;
							if (field.IsFamily) return MemberAttributes.Family;
							if (field.IsAssembly) return MemberAttributes.Assembly;
						}
						else {
							PropertyInfo prop = baseType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
							if (prop != null) {
								MethodInfo[] accessors = prop.GetAccessors(true);
								if (accessors != null && accessors.Length > 0) {
									MethodInfo mi = accessors[0];
									if (mi != null) {
										if (mi.IsPrivate) return MemberAttributes.Private;
										if (mi.IsPublic) return MemberAttributes.Public;
										if (mi.IsFamily) return MemberAttributes.Family;
										if (mi.IsAssembly) return MemberAttributes.Assembly;
									}
								}
							}
						}
					}
				}
				return MemberAttributes.Private;
			}
			public bool CanExtend(object extendee) {
				if (!(extendee is IComponent)) {
					return false;
				}
				if (baseComponent == null) {
					ISite site = ((IComponent)extendee).Site;
					if (site != null) {
						IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
						if (host != null) {
							baseComponent = host.RootComponent;
						}
					}
				}
				if (extendee == baseComponent)
					return false;
				return IsObjectInherited(extendee) ? ShouldExtendInheritedObject() : ShouldExtendNotInheritedObject();
			}
			bool IsObjectInherited(object obj) {
				return !TypeDescriptor.GetAttributes(obj)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited);
			}
		}
		[ProvideProperty("Modifiers", typeof(IComponent))]
		class ModifiersExtenderProvider : ModifiersInheritedExtenderProvider {
			protected override bool ShouldExtendInheritedObject() {
				return false;
			}
			protected override bool ShouldExtendNotInheritedObject() {
				return true;
			}
			[
			DesignOnly(true),
			TypeConverter(typeof(ModifierConverter)),
			DefaultValue(MemberAttributes.Private),
			Category("Design")
			]
			public override MemberAttributes GetModifiers(IComponent comp) {
				ISite site = comp.Site;
				if (site != null) {
					IDictionaryService dictionary = (IDictionaryService)site.GetService(typeof(IDictionaryService));
					if (dictionary != null) {
						object value = dictionary.GetValue(GetType());
						if (value is MemberAttributes) {
							return (MemberAttributes)value;
						}
					}
				}
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
				PropertyDescriptor prop = props["DefaultModifiers"];
				if (prop != null && prop.PropertyType == typeof(MemberAttributes)) {
					return (MemberAttributes)prop.GetValue(comp);
				}
				return MemberAttributes.Private;
			}
			public void SetModifiers(IComponent comp, MemberAttributes modifiers) {
				ISite site = comp.Site;
				if (site != null) {
					IDictionaryService dictionary = (IDictionaryService)site.GetService(typeof(IDictionaryService));
					if (dictionary != null) {
						dictionary.SetValue(GetType(), modifiers);
					}
				}
			}
		}
		private class ModifierConverter : TypeConverter 
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return GetConverter(context).CanConvertFrom(context, sourceType);
			}
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return GetConverter(context).CanConvertTo(context, destinationType);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				return GetConverter(context).ConvertFrom(context, culture, value);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				return GetConverter(context).ConvertTo(context, culture, value, destinationType);
			}
			public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
				return GetConverter(context).CreateInstance(context, propertyValues);
			}
			private TypeConverter GetConverter(ITypeDescriptorContext context) {
				TypeConverter modifierConverter = null;
				if (context != null) {
					CodeDomProvider provider = (CodeDomProvider)context.GetService(typeof(CodeDomProvider));
					if (provider != null) {
						modifierConverter = provider.GetConverter(typeof(MemberAttributes));
					}
				}
				if (modifierConverter == null) {
					modifierConverter = TypeDescriptor.GetConverter(typeof(MemberAttributes));
				}
				return modifierConverter;
			}
			public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
				return GetConverter(context).GetCreateInstanceSupported(context);
			}
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
				return GetConverter(context).GetProperties(context, value, attributes);
			}
			public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
				return GetConverter(context).GetPropertiesSupported(context);
			}
			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
				StandardValuesCollection values = GetConverter(context).GetStandardValues(context);
				if (values != null && values.Count > 0) {
					bool needMassage = false;
					foreach(MemberAttributes value in values) {
						if ((value & MemberAttributes.AccessMask) == 0) {
							needMassage = true;
							break;
						}
					}
					if (needMassage) {
						ArrayList list = new ArrayList(values.Count);
						foreach(MemberAttributes value in values) {
							if ((value & MemberAttributes.AccessMask) != 0 && value != MemberAttributes.AccessMask) {
								list.Add(value);
							}
						}
						values = new StandardValuesCollection(list);
					}
				}
				return values;
			}
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
				return GetConverter(context).GetStandardValuesExclusive(context);
			}
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
				return GetConverter(context).GetStandardValuesSupported(context);
			}
			public override bool IsValid(ITypeDescriptorContext context, object value) {
				return GetConverter(context).IsValid(context, value);
			}
		}
		#endregion
		IExtenderProvider[] providers;
		IExtenderProviderService extenderProviderService;
		public DesignerExtenders(IExtenderProviderService extenderProviderService) {
			this.extenderProviderService = extenderProviderService;
			if (providers == null) {
				providers = new IExtenderProvider[] { new ModifiersExtenderProvider(), new ModifiersInheritedExtenderProvider() };
			}
			for (int i = 0; i < providers.Length; i++) {
				extenderProviderService.AddExtenderProvider(providers[i]);
			}
		}
		public void Dispose() {
			if (extenderProviderService != null && providers != null) {
				for (int i = 0; i < providers.Length; i++) {
					extenderProviderService.RemoveExtenderProvider(providers[i]);
				}
				providers = null;
				extenderProviderService = null;
			}
		}
 	}
}
