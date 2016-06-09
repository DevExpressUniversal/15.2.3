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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	public interface ISharedPart {
		Object Entity { get; }
	}
	internal class DataClassCode : BaseDataClass {
		public DataClassCode(string className)
			: base(className) {
			Virtuality = Virtuality.Abstract;
			AddISharedPartImplementation();
		}
		private void AddISharedPartImplementation() {
			String typeString = CodeBuilder.TypeToString(typeof(ISharedPart));
			ImplementedInterfaceFullNames.Add(typeString);
			CustomPropertyCode entityProperty = new CustomPropertyCode(CodeBuilder.TypeToString(typeof(Object)), typeString, "Entity");
			entityProperty.GetGetter = (codeBuilder) => { codeBuilder.AppendLine("return Instance;"); };
			AddPropertyCode(entityProperty);
		}
		private DataMetadata dataMetadata;
		public DataMetadata DataMetadata {
			get { return dataMetadata; }
		}
		protected override void FillClassCore(ClassMetadata classMetadata, DataMetadata _dataMetadata) {
			dataMetadata = _dataMetadata;
			base.FillClassCore(classMetadata, _dataMetadata);
			foreach(DataMetadata aggregatedData in _dataMetadata.AggregatedData) {
				if(NeedCreateField(aggregatedData.Name)) {
					string fieldTypeFullName = CodeBuilder.TypeToString(typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(aggregatedData.PrimaryInterface.InterfaceType));
					FieldCode fieldCode = new FieldCode(aggregatedData.Name, fieldTypeFullName);
					fieldCode.AddAttribute(new DevExpress.Xpo.AggregatedAttribute());
					fieldCode.AddAttribute(new DevExpress.Xpo.PersistentAttribute());
					fieldCode.Visibility = Visibility.Public;
					AddFieldCode(fieldCode);
				}
			}
			InstancePropertyCode instancePropertyCode = new InstancePropertyCode(CodeBuilder.TypeToString(_dataMetadata.PrimaryInterface.InterfaceType), "this", this);
			instancePropertyCode.Virtuality = Virtuality.Abstract;
			AddPropertyCode(instancePropertyCode);
		}
		protected override PropertyCode GenerateAggregatedNonListPropertyCodeCore(PropertyMetadata propertyMetadata, DataMetadata dataMetadata) {
			return new AggregatedDataClassPropertyCode(propertyMetadata, this, dataMetadata);
		}
		protected override PropertyCode GenerateAggregatedListPropertyCode(PropertyMetadata propertyMetadata, DataMetadata dataMetadata) {
			return new AggregatedDataClassPropertyCode(propertyMetadata, this, dataMetadata);
		}
		protected override PropertyCode GenerateNonAggregatedNonListPropertyCode(PropertyMetadata propertyMetadata) {
			if(propertyMetadata.AssociationInfo != null) {
				return new PersistentPropertyWithFieldCode(propertyMetadata, this);
			}
			else if(LogicHelper.IsLogicRequired(propertyMetadata)) {
				LogicDataClassPropertyCode property = new LogicDataClassPropertyCode(propertyMetadata, this);
				if(!propertyMetadata.IsPersistent) {
					bool hasNonPersistentInterface = false;
					foreach(Attribute attribute in property.Attributes) {
						if(attribute is DevExpress.Xpo.NonPersistentAttribute) {
							hasNonPersistentInterface = true;
							break;
						}
					}
					if(!hasNonPersistentInterface) {
						property.AddAttribute(new DevExpress.Xpo.NonPersistentAttribute());
					}
				}
				return property;
			}
			return base.GenerateNonAggregatedNonListPropertyCode(propertyMetadata);
		}
		protected override PropertyCode GenerateNonAggregatedListPropertyCode(InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata) {
			if(propertyMetadata.IsLogicRequired) {
				return new LogicDataClassPropertyCode(propertyMetadata, this);
			}
			return base.GenerateNonAggregatedListPropertyCode(interfaceMetadata, propertyMetadata);
		}
		protected override void FillAssociationAggregatedManyToManyCodeModel(PropertyCode propertyCode, InterfaceMetadata interfaceMetadata, PropertyMetadata propertyMetadata) {
			base.FillAssociationAggregatedManyToManyCodeModel(propertyCode, interfaceMetadata, propertyMetadata);
			if(interfaceMetadata.DataClass == null) {
				FillAliasPropertyCode(propertyCode, propertyMetadata, interfaceMetadata, true, false);
			}
			else {
				FillPropertyManyToManyAttribute(propertyCode, propertyMetadata, interfaceMetadata);
			}
			FillAssociationManyToManyLinksPropertyCode(propertyMetadata, interfaceMetadata);
		}
		protected override void FillClassPersistentInterfacesCodeModel(InterfaceMetadata interfaceMetadata, DataMetadata dataMetadata) {
			base.FillClassPersistentInterfacesCodeModel(interfaceMetadata, dataMetadata);
			if(interfaceMetadata.IsPersistent) {
				Type persistentInterfaceDataType = typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(interfaceMetadata.InterfaceType);
				ImplementedInterfaceFullNames.Add(CodeBuilder.TypeToString(persistentInterfaceDataType));
				InstancePropertyCode instancePropertyCode = new InstancePropertyCode(CodeBuilder.TypeToString(interfaceMetadata.InterfaceType), "Instance", null, this);
				instancePropertyCode.InterfaceFullName = CodeBuilder.TypeToString(persistentInterfaceDataType);
				AddPropertyCode(instancePropertyCode);
			}
		}
	}
}
