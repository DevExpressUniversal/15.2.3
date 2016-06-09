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
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public enum LookupEditCollectionSourceMode { Lookup, Link }
	public class LookupEditPropertyCollectionSource : PropertyCollectionSource {
		private Boolean canFilterCollection;
		private LookupEditCollectionSourceMode lookupMode = LookupEditCollectionSourceMode.Lookup;
		private DataSourcePropertyIsNullMode dataSourcePropertyIsNullMode;
		private string dataSourcePropertyIsNullCriteria;
		protected override void OnObjectSpaceReloaded() {
			if((masterObject != null) && (ObjectSpace != null)) {
				masterObject = ObjectSpace.GetObject(masterObject);
			}
			base.OnObjectSpaceReloaded();
		}
		protected override Object CreateCollection() {
			canFilterCollection = false;
			Object result = base.CreateCollection();
			if(result == null) {
				canFilterCollection = true;
				switch(dataSourcePropertyIsNullMode) {
					case DataSourcePropertyIsNullMode.SelectAll: {
							result = ObjectSpace.CreateCollection(ObjectTypeInfo.Type);
							break;
						}
					case DataSourcePropertyIsNullMode.SelectNothing: {
							result = null;
							break;
						}
					case DataSourcePropertyIsNullMode.CustomCriteria: {
							if(!string.IsNullOrEmpty(dataSourcePropertyIsNullCriteria)) {
								CriteriaWrapper criteriaWrapper = new CriteriaWrapper(dataSourcePropertyIsNullCriteria, MasterObject);
								result = ObjectSpace.CreateCollection(ObjectTypeInfo.Type, criteriaWrapper.CriteriaOperator);
							}
							else {
								result = ObjectSpace.CreateCollection(ObjectTypeInfo.Type);
							}
							break;
						}
				}
			}
			return result;
		}
		protected override void ApplyCriteriaCore(CriteriaOperator criteria) {
			if(canFilterCollection) {
				if(objectSpace.CanApplyFilter(originalCollection)) {
					objectSpace.ApplyFilter(originalCollection, criteria);
				}
			}
			else {
				base.ApplyCriteriaCore(criteria);
			}
		}
		public LookupEditPropertyCollectionSource(
			IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberDescriptor,
			DataSourcePropertyIsNullMode dataSourcePropertyIsNullMode, String dataSourcePropertyIsNullCriteria, CollectionSourceMode collectionSourceMode)
			: base(objectSpace, masterObjectType, masterObject, memberDescriptor, collectionSourceMode) {
			this.dataSourcePropertyIsNullMode = dataSourcePropertyIsNullMode;
			this.dataSourcePropertyIsNullCriteria = dataSourcePropertyIsNullCriteria;
		}
		public LookupEditPropertyCollectionSource(
			IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberDescriptor,
			DataSourcePropertyIsNullMode dataSourcePropertyIsNullMode, String dataSourcePropertyIsNullCriteria)
			: this(objectSpace, masterObjectType, masterObject, memberDescriptor, dataSourcePropertyIsNullMode,
			dataSourcePropertyIsNullCriteria, CollectionSourceMode.Normal) {
		}
		public LookupEditCollectionSourceMode LookupMode {
			get { return lookupMode; }
			set { lookupMode = value; }
		}
	}
}
