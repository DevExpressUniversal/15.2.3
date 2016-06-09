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
using System.Collections;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Utils.Serializing.Helpers {
	static class SerializationStrings {
		public const string Reference = "Ref";
		public const string ReferenceLink = "#Ref-";
	}
	public class ReferenceSerializationHelper {
		#region inner classes
		class ReferenceLinkPropertyDescriptor : PropertyDescriptor {
			static XtraSerializableProperty visibleAttribute = new XtraSerializableProperty(XtraSerializationVisibility.Visible);
			string referenceLink;
			PropertyDescriptor realProperty;
			public PropertyDescriptor RealProperty {
				get { return realProperty; }
			}
			public ReferenceLinkPropertyDescriptor(PropertyDescriptor realProperty, string referenceLink)
				: base(realProperty.Name, new Attribute[] { visibleAttribute }) {
				this.realProperty = realProperty;
				this.referenceLink = referenceLink;
			}
			public override object GetValue(object component) {
				return referenceLink;
			}
			public override bool CanResetValue(object component) {
				return true;
			}
			public override Type ComponentType {
				get { return null; }
			}
			public override bool IsReadOnly {
				get { return false; }
			}
			public override Type PropertyType {
				get { return referenceLink.GetType(); }
			}
			public override void ResetValue(object component) {
				referenceLink = null;
			}
			public override void SetValue(object component, object value) {
				this.referenceLink = value as string;
			}
			public override bool ShouldSerializeValue(object component) {
				return realProperty.ShouldSerializeValue(component);
			}
		}
		class ReferencePropertyDescriptor : PropertyDescriptor {
			static XtraSerializableProperty visibleAttribute = new XtraSerializableProperty(XtraSerializationVisibility.Visible, -100);
			string reference;
			public ReferencePropertyDescriptor(string reference)
				: base(SerializationStrings.Reference, new Attribute[] { visibleAttribute }) {
				this.reference = reference;
			}
			public override object GetValue(object component) {
				return reference;
			}
			public override bool CanResetValue(object component) {
				return true;
			}
			public override Type ComponentType {
				get { return null; }
			}
			public override bool IsReadOnly {
				get { return false; }
			}
			public override Type PropertyType {
				get { return typeof(string); }
			}
			public override void ResetValue(object component) {
				reference = null;
			}
			public override void SetValue(object component, object value) {
				this.reference = value as string;
			}
			public override bool ShouldSerializeValue(object component) {
				return true;
			}
		}
		#endregion
		List<object> serializedObjects = new List<object>();
		List<object> referencedObjects = new List<object>();
		Dictionary<object, string> objectRefereces = new Dictionary<object, string>();
		int refID;
		public List<object> ReferencedObjects {
			get { return referencedObjects; }
		}
		public List<object> SerializedObjects {
			get { return serializedObjects; }
		}
		public PropertyDescriptorCollection ProcessProperties(object obj, PropertyDescriptorCollection properties) {
			serializedObjects.Add(obj);
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor item in properties) {
				XtraSerializableProperty attr = item.Attributes[typeof(XtraSerializableProperty)] as XtraSerializableProperty;
				if(attr == null || attr.Visibility != XtraSerializationVisibility.Reference) {
					result.Add(item);
					continue;
				}
				object value = item.GetValue(obj);
				if(value != null)
					result.Add(new ReferenceLinkPropertyDescriptor(item, SerializationStrings.ReferenceLink + GetReference(value)));
			}
			result.Add(new ReferencePropertyDescriptor(GetReference(obj)));
			return new PropertyDescriptorCollection(result.ToArray());
		}
		public void OnPropertySerialize(object component, PropertyDescriptor property) {
			if(component != null && property is ReferenceLinkPropertyDescriptor) {
				object value = ((ReferenceLinkPropertyDescriptor)property).RealProperty.GetValue(component);
				if(value != null && !referencedObjects.Contains(value))
					referencedObjects.Add(value);
			}
		}
		public string GetReference(object obj) {
			string reference = string.Empty;
			if(obj != null && !objectRefereces.TryGetValue(obj, out reference)) {
				reference = refID.ToString();
				objectRefereces.Add(obj, reference);
				refID++;
			}
			return reference;
		}
		public bool IsSerilizedObject(object obj) {
			return serializedObjects.Contains(obj);
		}
	}
	public class ReferenceDeserializationHelper {
		Dictionary<string, object> referencedObjects = new Dictionary<string, object>();
		List<Pair<object, XtraPropertyInfo>> referencedProperties = new List<Pair<object, XtraPropertyInfo>>();
		public void AddReferencedObject(object obj, string reference) {
			if(!Object.Equals(obj, XtraSerializer.NullValueString))
				referencedObjects.Add(reference, obj);
		}
		public void ProcessProperties(object obj, IXtraPropertyCollection store) {
			foreach(XtraPropertyInfo prop in store) {
				if(prop.Name == SerializationStrings.Reference) {
					AddReferencedObject(obj, (string)prop.Value);
					continue;
				}
				if((prop.Value is string) && ((string)prop.Value).StartsWith(SerializationStrings.ReferenceLink))
					referencedProperties.Add(new Pair<object, XtraPropertyInfo>(obj, prop));
			}
		}
		public void AssignReferecedObjects() {
			foreach(Pair<object, XtraPropertyInfo> item in referencedProperties) {
				object obj = item.First;
				XtraPropertyInfo property = item.Second;
				PropertyInfo[] destinationPrperties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach(PropertyInfo destinationProperty in destinationPrperties) {
					object value = null;
					if(destinationProperty.Name == property.Name && referencedObjects.TryGetValue(ParseReference(property.Value as string), out value)) {
						destinationProperty.SetValue(obj, value, new object[] { });
						break;
					}
				}
			} 
		}
		public static string ParseReference(string value) { 
			return value.Replace(SerializationStrings.ReferenceLink, string.Empty);
		} 
	}
}
