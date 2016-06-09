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
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.Utils.Menu;
namespace DevExpress.Utils.Design {
	public static class DesignTimeOptions {
		static UITypeEditor defaultImageEditorCore = null;
		public static UITypeEditor DefaultImageEditor {
			get {
				if(defaultImageEditorCore == null)
					defaultImageEditorCore = CreateDefaultImageEditor();
				return defaultImageEditorCore;
			}
		}
		static UITypeEditor CreateDefaultImageEditor() {
			return new DXImageEditor();
		}
	}
	public class ControlDesignerHelper {
		public static IDXMenuManager GetBarManager(IContainer container) {
			return GetTypeFromContainer(container, typeof(IDXMenuManager)) as IDXMenuManager;
		}
		protected static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null) return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj)) return obj;
			}
			return null;
		}
	}
	internal class DesignerHelper {
		public static Hashtable InitializeNestedInheritedProperties(object component, InheritanceAttribute attribute, IDictionary properties) {
#if DXWhidbey
			if(attribute.Equals(InheritanceAttribute.NotInherited)) return null;
			Hashtable hashtable1 = new Hashtable();
			if(!attribute.Equals(InheritanceAttribute.InheritedReadOnly)) {
				if(properties == null) properties = TypeDescriptor.GetProperties(component);
				PropertyDescriptor[] descriptorArray1 = new PropertyDescriptor[properties.Count];
				properties.Values.CopyTo(descriptorArray1, 0);
				for(int num1 = 0; num1 < descriptorArray1.Length; num1++) {
					PropertyDescriptor descriptor1 = descriptorArray1[num1];
					if(!object.Equals(descriptor1.Attributes[typeof(DesignOnlyAttribute)], DesignOnlyAttribute.Yes) && ((descriptor1.SerializationVisibility != DesignerSerializationVisibility.Hidden) || descriptor1.IsBrowsable || descriptor1.Attributes[typeof(HiddenInheritableCollectionAttribute)] != null)) {
						if(descriptor1.Attributes[typeof(InheritableCollectionAttribute)] != null) {
							PropertyDescriptor descriptor2 = (PropertyDescriptor)hashtable1[descriptor1.Name];
							if(descriptor2 == null) {
								hashtable1[descriptor1.Name] = new DXInheritedPropertyDescriptor(descriptor1, component, false);
							}
						}
					}
				}
			}
			return hashtable1;
#else
			return null;
#endif
		}
		public static void ResetBaseIheritedAttribute(ComponentDesigner designer) {
#if DXWhidbey
			FieldInfo fi = typeof(ComponentDesigner).GetField("inheritedProps", BindingFlags.Instance | BindingFlags.NonPublic);
			if(fi != null) fi.SetValue(designer, null);
#endif
		}
		public static void FilterOutBaseIherited(ComponentDesigner designer, Hashtable dxInheritedProps) {
#if DXWhidbey
			FieldInfo fi = typeof(ComponentDesigner).GetField("inheritedProps", BindingFlags.Instance | BindingFlags.NonPublic);
			if(fi == null || dxInheritedProps == null) return;
			Hashtable inh = (Hashtable)fi.GetValue(designer);
			if(inh == null) return;
			foreach(DictionaryEntry entry in dxInheritedProps) {
				if(inh.ContainsKey(entry.Key)) inh.Remove(entry.Key);
			}
#endif
		}
		public static void PostFilterProperties(Hashtable dxInheritedProps, InheritanceAttribute attribute, IDictionary properties) {
#if DXWhidbey
			if(dxInheritedProps != null) {
				if(!attribute.Equals(InheritanceAttribute.InheritedReadOnly)) {
					foreach(DictionaryEntry entry1 in dxInheritedProps) {
						DXInheritedPropertyDescriptor descriptor2 = entry1.Value as DXInheritedPropertyDescriptor;
						if(descriptor2 != null) {
							PropertyDescriptor descriptor3 = (PropertyDescriptor)properties[entry1.Key];
							if(descriptor3 != null) {
								descriptor2.PropertyDescriptor = descriptor3;
								properties[entry1.Key] = descriptor2;
							}
						}
					}
				}
			}
#endif
		}
	}
#if DXWhidbey
	public class DXControlCodeDomSerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer serializer1 = (CodeDomSerializer)manager.GetSerializer(typeof(Control), typeof(CodeDomSerializer));
			if(serializer1 == null) return null;
			return serializer1.Serialize(manager, value);
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			if((manager == null) || (codeObject == null)) {
				throw new ArgumentNullException((manager == null) ? "manager" : "codeObject");
			}
			object obj1 = null;
			CodeDomSerializer serializer1 = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
			if(serializer1 == null) return null;
			obj1 = serializer1.Deserialize(manager, codeObject);
			return obj1;
		}
	}
	public class DXCollectionCodeDomSerializer : CollectionCodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			if(manager == null) {
				throw new ArgumentNullException("manager");
			}
			if(value == null) {
				throw new ArgumentNullException("value");
			}
			object obj1 = null;
			CodeExpression expression1;
			ExpressionContext context1 = manager.Context[typeof(ExpressionContext)] as ExpressionContext;
			PropertyDescriptor descriptor1 = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;
			if(((context1 != null) && (context1.PresetValue == value)) && ((descriptor1 != null) && (descriptor1.PropertyType == context1.ExpressionType))) {
				expression1 = context1.Expression;
			}
			else {
				expression1 = null;
				context1 = null;
				descriptor1 = null;
			}
			ICollection collection1 = value as ICollection;
			if(collection1 == null) return obj1;
			ICollection collection2 = collection1;
			Type type1 = (context1 == null) ? collection1.GetType() : context1.ExpressionType;
			bool flag1 = typeof(Array).IsAssignableFrom(type1);
			if((expression1 == null) && !flag1) {
				bool flag2;
				expression1 = base.SerializeCreationExpression(manager, collection1, out flag2);
				if(flag2) {
					return expression1;
				}
			}
			if((expression1 == null) && !flag1) {
				return obj1;
			}
			if(IsInheritedDescriptor(descriptor1) && !flag1) {
				collection2 = GetCollectionDelta(GetOriginalValue(descriptor1) as ICollection, collection1);
			}
			obj1 = this.SerializeCollection(manager, expression1, type1, collection1, collection2);
			if(expression1 == null || !ShouldClearCollection(manager, collection1)) {
				return obj1;
			}
			CodeStatementCollection collection3 = obj1 as CodeStatementCollection;
			if((collection1.Count > 0) && ((obj1 == null) || ((collection3 != null) && (collection3.Count == 0)))) {
				return null;
			}
			if(collection3 == null) {
				collection3 = new CodeStatementCollection();
				CodeStatement statement1 = obj1 as CodeStatement;
				if(statement1 != null) {
					collection3.Add(statement1);
				}
				obj1 = collection3;
			}
			if(collection3 != null) {
				CodeMethodInvokeExpression expression2 = new CodeMethodInvokeExpression(expression1, "Clear", new CodeExpression[0]);
				CodeExpressionStatement statement2 = new CodeExpressionStatement(expression2);
				collection3.Insert(0, statement2);
			}
			return obj1;
		}
		public bool IsInheritedDescriptor(PropertyDescriptor descriptor) {
			return descriptor is DXInheritedPropertyDescriptor ||  descriptor.GetType().Name == "InheritedPropertyDescriptor";
		}
		public object GetOriginalValue(PropertyDescriptor descriptor) {
			DXInheritedPropertyDescriptor desc = descriptor as DXInheritedPropertyDescriptor;
			if(desc != null) return desc.OriginalValue;
			PropertyInfo pi = descriptor.GetType().GetProperty("OriginalValue", BindingFlags.Instance | BindingFlags.NonPublic);
			if(pi == null) return null;
			return pi.GetValue(descriptor, null);
		}
		public ICollection GetCollectionDelta(ICollection original, ICollection modified) {
			ICollection res = GetCollectionDeltaCore(original, modified);
			if(res == null) return res;
			ArrayList list = new ArrayList();
			foreach(object obj in res) {
				IComponent component = obj as IComponent;
				if(component != null && component.Site == null) continue;
				list.Add(obj);
			}
			return list;
		}
		public ICollection GetCollectionDeltaCore(ICollection original, ICollection modified) {
			if(original != null && modified != null && original.Count != 0) {
				IEnumerator enumerator1 = modified.GetEnumerator();
				if(enumerator1 != null) {
					IDictionary dictionary1 = new HybridDictionary();
					foreach(object obj1 in original) {
						if(dictionary1.Contains(obj1)) {
							int num1 = (int)dictionary1[obj1];
							dictionary1[obj1] = ++num1;
							continue;
						}
						dictionary1.Add(obj1, 1);
					}
					ArrayList res = null;
					for(int num2 = 0; (num2 < modified.Count) && enumerator1.MoveNext(); num2++) {
						object obj2 = enumerator1.Current;
						if(dictionary1.Contains(obj2)) {
							if(res == null) {
								res = new ArrayList();
								enumerator1.Reset();
								for(int num3 = 0; (num3 < num2) && enumerator1.MoveNext(); num3++) {
									res.Add(enumerator1.Current);
								}
								enumerator1.MoveNext();
							}
							int num4 = (int)dictionary1[obj2];
							if(--num4 == 0) {
								dictionary1.Remove(obj2);
							}
							else {
								dictionary1[obj2] = num4;
							}
						}
						else if(res != null) {
							res.Add(obj2);
						}
					}
					if(res != null) {
						return res;
					}
				}
			}
			return modified;
		}
		bool ShouldClearCollection(IDesignerSerializationManager manager, ICollection collection) {
			bool flag1 = false;
			PropertyDescriptor descriptor1 = manager.Properties["ClearCollections"];
			if(((descriptor1 != null) && (descriptor1.PropertyType == typeof(bool))) && ((bool)descriptor1.GetValue(manager))) {
				flag1 = true;
			}
			if(!flag1) {
				SerializeAbsoluteContext context1 = (SerializeAbsoluteContext)manager.Context[typeof(SerializeAbsoluteContext)];
				PropertyDescriptor descriptor2 = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;
				if((context1 != null) && context1.ShouldSerialize(descriptor2)) {
					flag1 = true;
				}
			}
			if(!flag1) {
				return flag1;
			}
			MethodInfo info1 = TypeDescriptor.GetReflectionType(collection).GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
			if((info1 != null) && this.MethodSupportsSerialization(info1)) {
				return flag1;
			}
			return false;
		}
	}
#endif
}
