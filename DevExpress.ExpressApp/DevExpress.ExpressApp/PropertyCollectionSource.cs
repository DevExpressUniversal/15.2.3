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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	[AttributeUsage(AttributeTargets.Property)]
	public class CollectionOperationSetAttribute : Attribute {
		private Boolean allowAdd;
		private Boolean allowRemove;
		public CollectionOperationSetAttribute() {
			allowAdd = true;
			allowRemove = true;
		}
		public Boolean AllowAdd {
			get { return allowAdd; }
			set { allowAdd = value; }
		}
		public Boolean AllowRemove {
			get { return allowRemove; }
			set { allowRemove = value; }
		}
	}
	public class PropertyCollectionSource : CollectionSourceBase {
		private IMemberInfo memberInfo;
		private Type masterObjectType;
		protected Object masterObject;
		private ITypeInfo objectTypeInfo;
		private void InitObjectTypeInfo() {
			if(memberInfo.ListElementTypeInfo != null) {
				objectTypeInfo = memberInfo.ListElementTypeInfo;
			}
			else {
				ElementTypePropertyAttribute elementTypePropertyAttribute = memberInfo.FindAttribute<ElementTypePropertyAttribute>();
				if(elementTypePropertyAttribute != null) {
					Object lastMemberOwner = memberInfo.GetOwnerInstance(masterObject);
					if(lastMemberOwner != null) {
						ITypeInfo lastMemberOwnerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(lastMemberOwner.GetType());
						IMemberInfo elementTypeMemberInfo = lastMemberOwnerTypeInfo.FindMember(elementTypePropertyAttribute.Name);
						if(elementTypeMemberInfo != null) {
							Type elementType = (Type)elementTypeMemberInfo.GetValue(lastMemberOwner);
							objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(elementType);
						}
					}
				}
			}
			if(((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) && !objectTypeInfo.IsPersistent) {
				throw new ArgumentException(
					String.Format("The '{0}' non-persistent type cannot be used in {1} mode.", objectTypeInfo.FullName, dataAccessMode.ToString()));
			}
		}
		private CriteriaOperator GetAssociatedCollectionCriteria() {
			Object actualMasterObject = objectSpace.GetObject(masterObject);
			return objectSpace.GetAssociatedCollectionCriteria(actualMasterObject, memberInfo);
		}
		protected override Object CreateCollection() {
			Object result = null;
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) && (objectTypeInfo != null)) {
				Object collectionOwner = memberInfo.GetOwnerInstance(masterObject);
				if(collectionOwner != null) {
					result = objectSpace.CreateServerCollection(objectTypeInfo.Type, GetAssociatedCollectionCriteria());
				}
			}
			else if((dataAccessMode == CollectionSourceDataAccessMode.DataView) && (objectTypeInfo != null)) {
				Object collectionOwner = memberInfo.GetOwnerInstance(masterObject);
				if(collectionOwner != null) {
					result = objectSpace.CreateDataView(objectTypeInfo.Type, "", GetAssociatedCollectionCriteria(), null);
				}
			}
			else {
				result = memberInfo.GetValue(masterObject);
			}
			return result;
		}
		protected internal override CriteriaOperator GetTotalCriteria() {
			CriteriaOperator originalCollectionCriteria = null;
			CriteriaOperator originalCollectionFilter = null;
			CriteriaOperator proxyCollectionFilter = null;
			if((masterObject != null) && (memberInfo.AssociatedMemberInfo != null)) {
				originalCollectionCriteria = objectSpace.GetAssociatedCollectionCriteria(masterObject, memberInfo);
			}
			else if(originalCollection != null) {
				originalCollectionCriteria = objectSpace.GetCriteria(originalCollection);
			}
			if(mode == CollectionSourceMode.Proxy) {
				if(originalCollection != null) {
					originalCollectionFilter = objectSpace.GetFilter(originalCollection);
				}
				proxyCollectionFilter = GetExternalCriteria();
			}
			else {
				if(CanApplyCriteria) {
					originalCollectionFilter = GetExternalCriteria();
				}
			}
			return CombineCriteria(originalCollectionCriteria, originalCollectionFilter, proxyCollectionFilter);
		}
		protected override void ApplyCriteriaCore(CriteriaOperator criteria) {
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				objectSpace.ApplyCriteria(originalCollection, CombineCriteria(GetAssociatedCollectionCriteria(), criteria));
			}
			else {
				if(mode == CollectionSourceMode.Proxy) {
					if(proxyCollection != null) {
						proxyCollection.Filter = criteria;
					}
				}
				else if(mode == CollectionSourceMode.Normal) {
					if(originalCollection != null) {
						if(objectSpace.CanApplyFilter(originalCollection)) {
							objectSpace.ApplyFilter(originalCollection, criteria);
						}
						else if(!Object.ReferenceEquals(criteria, null)) {
							throw new CannotApplyCriteriaException(originalCollection.GetType());
						}
					}
				}
			}
		}
		protected override void OnCriteriaChanging() {
			if(!CanApplyCriteria) {
				throw new CannotApplyCriteriaException(memberInfo.MemberType);
			}
		}
		protected override Boolean DefaultAllowAdd(out String diagnosticInfo) {
			Boolean result = true;
			diagnosticInfo = "";
			CollectionOperationSetAttribute operationSet = memberInfo.FindAttribute<CollectionOperationSetAttribute>();
			if(operationSet != null) {
				if(!operationSet.AllowAdd) {
					result = false;
					diagnosticInfo = "CollectionOperationSet.AllowAdd == false";
				}
			}
			else {
				if(dataAccessMode == CollectionSourceDataAccessMode.Server) {
					result = true;
					diagnosticInfo = String.Format("Always allowed in {0} mode", Enum.GetName(typeof(CollectionSourceDataAccessMode), dataAccessMode));
				}
				else {
					result = base.DefaultAllowAdd(out diagnosticInfo);
				}
				foreach(ModelDefaultAttribute attribute in memberInfo.FindAttributes<ModelDefaultAttribute>()) {
					if(attribute.PropertyName == "AllowAdd") {
						switch(attribute.PropertyValue.ToLower()) {
							case "true": {
									result = true;
									break;
								}
							case "false": {
									result = false;
									diagnosticInfo = "The AllowAdd ModelDefaultAttribute is false";
									break;
								}
						}
						break;
					}
				}
			}
			return result;
		}
		protected override Boolean DefaultAllowRemove(out String diagnosticInfo) {
			Boolean result = true;
			diagnosticInfo = "";
			CollectionOperationSetAttribute operationSet = memberInfo.FindAttribute<CollectionOperationSetAttribute>();
			if(operationSet != null) {
				if(!operationSet.AllowRemove) {
					result = false;
					diagnosticInfo = "CollectionOperationSet.AllowRemove == false";
				}
			}
			else {
				if(dataAccessMode == CollectionSourceDataAccessMode.Server) {
					result = true;
					diagnosticInfo = String.Format("Always allowed in {0} mode", Enum.GetName(typeof(CollectionSourceDataAccessMode), dataAccessMode));
				}
				else {
					result = base.DefaultAllowRemove(out diagnosticInfo);
				}
			}
			return result;
		}
		protected virtual void OnMasterObjectChanged() {
			if(MasterObjectChanged != null) {
				MasterObjectChanged(this, EventArgs.Empty);
			}
		}
		public PropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode)
			: base(objectSpace, dataAccessMode, mode) {
			Guard.CheckObjectFromObjectSpace(objectSpace, masterObject);
			this.masterObject = masterObject;
			this.memberInfo = memberInfo;
			this.masterObjectType = masterObjectType;
			InitObjectTypeInfo();
		}
		public PropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, Boolean isServerMode, CollectionSourceMode mode)
			: this(objectSpace, masterObjectType, masterObject, memberInfo, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, mode) {
		}
		public PropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, CollectionSourceMode mode)
			: this(objectSpace, masterObjectType, masterObject, memberInfo, CollectionSourceDataAccessMode.Client, mode) {
		}
		public PropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo)
			: this(objectSpace, masterObjectType, masterObject, memberInfo, CollectionSourceMode.Normal) {
		}
		public override void Dispose() {
			base.Dispose();
			MasterObjectChanged = null;
		}
		public override Boolean? IsObjectFitForCollection(Object obj) {
			CriteriaOperator criteria = null;
			if((masterObject != null) && (memberInfo.AssociatedMemberInfo != null)) {
				criteria = objectSpace.GetAssociatedCollectionCriteria(masterObject, memberInfo);
			}
			else if(originalCollection != null) {
				criteria = objectSpace.GetCriteria(originalCollection);
			}
			return objectSpace.IsObjectFitForCriteria(ObjectTypeInfo.Type, obj, criteria);
		}
		public override void Add(Object obj) {
			if(((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) && (memberInfo.AssociatedMemberInfo != null)) {
				Object collectionOwner = null;
				IMemberInfo collectionMemberInfo = null;
				if(memberInfo.GetPath().Count > 1) {
					collectionOwner = memberInfo.GetOwnerInstance(masterObject);
					collectionMemberInfo = memberInfo.LastMember;
				}
				else {
					collectionOwner = masterObject;
					collectionMemberInfo = memberInfo;
				}
				if(collectionOwner != null) {
					if(collectionMemberInfo.IsManyToMany) {
						IList list = ListHelper.GetList(collectionMemberInfo.AssociatedMemberInfo.GetValue(obj));
						if(list != null) {
							list.Add(collectionOwner);
						}
					}
					else {
						collectionMemberInfo.AssociatedMemberInfo.SetValue(obj, collectionOwner);
					}
				}
			}
			base.Add(obj);
		}
		public override void Remove(Object obj) {
			if(((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) && (memberInfo.AssociatedMemberInfo != null)) {
				if(memberInfo.IsManyToMany) {
					IList list = ListHelper.GetList(memberInfo.AssociatedMemberInfo.GetValue(obj));
					if(list != null) {
						list.Remove(masterObject);
					}
				}
				else {
					if(!objectSpace.IsDisposedObject(obj)) {
						memberInfo.AssociatedMemberInfo.SetValue(obj, null);
					}
				}
			}
			base.Remove(obj);
		}
		public override Boolean CanApplyCriteria {
			get {
				Boolean result = false;
				if(((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView))) {
					result = true;
				}
				else {
					if(mode == CollectionSourceMode.Proxy) {
						result = true;
					}
					else if(mode == CollectionSourceMode.Normal) {
						result = objectSpace.CanApplyCriteria(memberInfo.MemberType);
					}
				}
				return result;
			}
		}
		public override ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
		}
		public Object MasterObject {
			get { return masterObject; }
			set {
				if(masterObject != value) {
					masterObject = value;
					InitObjectTypeInfo();
					OnMasterObjectChanged();
					ResetCollection();
				}
			}
		}
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
		}
		public Type MasterObjectType {
			get { return masterObjectType; }
		}
		public Type DeclaredType {
			get { return memberInfo.Owner.Type; }
		}
		public event EventHandler MasterObjectChanged;
	}
}
