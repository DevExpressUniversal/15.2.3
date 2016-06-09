#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	public static class DCIntermediateObjectSettings {
		internal const string LeftObjectPropertyName = "LeftObject";
		internal const string RightObjectPropertyName = "RightObject";
		internal const string LeftObjectTypePropertyName = "LeftObjectType";
		internal const string RightObjectTypePropertyName = "RightObjectType";
		private static readonly object locker = new object();
		private static Type baseType = typeof(DCIntermediateObject);
		public static Type BaseType {
			get { return baseType; }
			set {
				lock(locker) {
					Guard.TypeArgumentIs(typeof(IDCIntermediateObject), value, "value");
					CheckRequiredProperty(value, LeftObjectPropertyName, typeof(object));
					CheckRequiredProperty(value, RightObjectPropertyName, typeof(object));
					CheckRequiredProperty(value, LeftObjectTypePropertyName, typeof(Type));
					CheckRequiredProperty(value, RightObjectTypePropertyName, typeof(Type));
					baseType = value;
				}
			}
		}
		private static void CheckRequiredProperty(Type inType, string propertyName, Type propertyType) {
			PropertyInfo property = TypeHelper.ContainsProperty(inType, propertyName) ? TypeHelper.GetProperty(inType, propertyName) : null;
			if(property == null || !property.CanRead || property.CanWrite || property.PropertyType != propertyType || !property.GetGetMethod().IsAbstract) {
				throw new ArgumentException(string.Format("The {0} type does not contain a read-only abstract {1} property of the {2} type.", inType.FullName, propertyName, propertyType.FullName), "value");
			}
		}
	}
	public interface IDCIntermediateObject {
		object LeftObject { get; }
		object RightObject { get; }
		Type LeftObjectType { get; }
		Type RightObjectType { get; }
	}
	[DevExpress.Xpo.NonPersistent]
	public abstract class DCIntermediateObject : DevExpress.Xpo.XPObject, IDCIntermediateObject {
		public DCIntermediateObject(global::DevExpress.Xpo.Session session) : base(session) { }
		#region IDCIntermediateObject Members
		[DevExpress.Xpo.NonPersistent]
		public abstract object LeftObject { get; }
		[DevExpress.Xpo.NonPersistent]
		public abstract object RightObject { get; }
		[DevExpress.Xpo.NonPersistent]
		public abstract Type LeftObjectType { get; }
		[DevExpress.Xpo.NonPersistent]
		public abstract Type RightObjectType { get; }
		#endregion
	}
	internal class LinkClassCode : ClassCode {
		public LinkClassCode(string className) : base(className) { }
		protected override void FillClassCore(ClassMetadata classMetadata, DataMetadata dataMetadata) {
			base.FillClassCore(classMetadata, dataMetadata);
		}
		public override string BaseClassFullName {
			get {
				return CodeBuilder.TypeToString(DCIntermediateObjectSettings.BaseType);
			}
			set {
				base.BaseClassFullName = value;
			}
		}
		internal override void AddPropertyCode(PropertyCodeBase propertyCode) {
			base.AddPropertyCode(propertyCode);
			AddIntermediateObjectProperty((LinkPropertyCode)propertyCode);
			AddIntermediateObjectTypeProperty((LinkPropertyCode)propertyCode);
		}
		private void AddIntermediateObjectTypeProperty(LinkPropertyCode sourcePropertyCode) {
			PropertyMetadata typePropertyCode = new PropertyMetadata();
			typePropertyCode.IsReadOnly = true;
			typePropertyCode.IsLogicRequired = false;
			typePropertyCode.PropertyType = typeof(Type);
			typePropertyCode.Name = sourcePropertyCode.IsLeftField ? DCIntermediateObjectSettings.LeftObjectTypePropertyName : DCIntermediateObjectSettings.RightObjectTypePropertyName;
			PropertyCode intermediateObjectFieldType = new IntermediateObjectTypeProperty(typePropertyCode, this, sourcePropertyCode.TypeFullName);
			intermediateObjectFieldType.Virtuality = Virtuality.Override;
			base.AddPropertyCode(intermediateObjectFieldType);
		}
		private void AddIntermediateObjectProperty(LinkPropertyCode sourcePropertyCode) {
			PropertyMetadata typePropertyCode = new PropertyMetadata();
			typePropertyCode.Name = sourcePropertyCode.IsLeftField ? DCIntermediateObjectSettings.LeftObjectPropertyName : DCIntermediateObjectSettings.RightObjectPropertyName;
			typePropertyCode.IsReadOnly = true;
			typePropertyCode.IsLogicRequired = false;
			typePropertyCode.PropertyType = typeof(Object);
			PropertyCode intermediateObjectFieldType = new PersistentPropertyWithFieldCodeBase(typePropertyCode, this);
			intermediateObjectFieldType.Virtuality = Virtuality.Override;
			base.AddPropertyCode(intermediateObjectFieldType);
		}
	}
	internal class IntermediateObjectTypeProperty : PropertyCode {
		string returnValue;
		public IntermediateObjectTypeProperty(PropertyMetadata propertyMetadata, ClassCode owner, string returnValue)
			: base(propertyMetadata, owner) {
			this.returnValue = returnValue;
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return typeof({0});", returnValue);
		}
	}
}
