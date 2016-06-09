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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public class CollectionSource : CollectionSourceBase {
		private ITypeInfo objectTypeInfo;
		private Boolean isAsyncServerMode;
		protected override Object CreateCollection() {
			Object result = null;
			if(dataAccessMode == CollectionSourceDataAccessMode.Server) {
				if(isAsyncServerMode && (objectSpace is IAsyncServerDataSourceCreator)) {
					result = ((IAsyncServerDataSourceCreator)objectSpace).Create(ObjectTypeInfo.Type, null);
				}
				else {
					result = objectSpace.CreateServerCollection(ObjectTypeInfo.Type, null);
				}
			}
			else if(dataAccessMode == CollectionSourceDataAccessMode.Client) {
				result = objectSpace.CreateCollection(ObjectTypeInfo.Type, null, null);
			}
			else if(dataAccessMode == CollectionSourceDataAccessMode.DataView) {
				result = objectSpace.CreateDataView(ObjectTypeInfo.Type, "", null, null);
			}
			return result;
		}
		protected override Boolean DefaultAllowAdd(out String diagnosticInfo) {
			Boolean result = true;
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				result = true;
				diagnosticInfo = String.Format("Always allowed in {0} mode", Enum.GetName(typeof(CollectionSourceDataAccessMode), dataAccessMode));
			}
			else {
				result = base.DefaultAllowAdd(out diagnosticInfo);
			}
			return result;
		}
		protected override Boolean DefaultAllowRemove(out String diagnosticInfo) {
			Boolean result = true;
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				result = true;
				diagnosticInfo = String.Format("Always allowed in {0} mode", Enum.GetName(typeof(CollectionSourceDataAccessMode), dataAccessMode));
			}
			else {
				result = base.DefaultAllowRemove(out diagnosticInfo);
			}
			return result;
		}
		protected internal override CriteriaOperator GetTotalCriteria() {
			if(originalCollection == null) {
				return GetExternalCriteria();
			}
			else {
				return CombineCriteria(objectSpace.GetCriteria(originalCollection), objectSpace.GetFilter(originalCollection));
			}
		}
		protected override void DisposePreviousCollection(Object previousCollection) {
			if(previousCollection is IDisposable) {
				((IDisposable)previousCollection).Dispose();
			}
		}
		protected override void ApplyCriteriaCore(CriteriaOperator criteria) {
			objectSpace.ApplyCriteria(originalCollection, criteria);
			if((mode == CollectionSourceMode.Proxy) && (proxyCollection != null)
					&& (ListHelper.GetBindingList(proxyCollection.OriginalCollection) == null)) {
				proxyCollection.Refresh();
			}
		}
		protected override void ApplyTopReturnedObjects() {
			if(originalCollection != null) {
				ObjectSpace.SetTopReturnedObjectsCount(originalCollection, TopReturnedObjects);
			}
		}
		protected override void ApplyDeleteObjectOnRemove() {
			if(originalCollection != null) {
				ObjectSpace.EnableObjectDeletionOnRemove(originalCollection, DeleteObjectOnRemove);
			}
		}
		protected override Boolean NeedResetCollectionOnCriteriaChanged() {
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) && isAsyncServerMode) {
				return true;
			}
			else {
				return base.NeedResetCollectionOnCriteriaChanged();
			}
		}
		protected internal CollectionSource(IObjectSpace objectSpace, ITypeInfo objectTypeInfo, CollectionSourceDataAccessMode dataAccessMode, Boolean isAsyncServerMode, CollectionSourceMode mode)
			: base(objectSpace, dataAccessMode, mode) {
			canApplySorting = true;
			this.isAsyncServerMode = isAsyncServerMode;
			if(objectTypeInfo == null) {
				throw new ArgumentNullException("objectTypeInfo");
			}
			this.objectTypeInfo = objectTypeInfo;
			if(((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) && !objectTypeInfo.IsPersistent) {
				throw new ArgumentException(
					String.Format("The '{0}' non-persistent type cannot be used in {1} mode.", objectTypeInfo.FullName, dataAccessMode.ToString()));
			}
		}
		protected internal CollectionSource(IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode, Boolean isAsyncServerMode, CollectionSourceMode mode)
			: this(objectSpace, XafTypesInfo.Instance.FindTypeInfo(objectType), dataAccessMode, isAsyncServerMode, mode) { }
		protected internal CollectionSource(IObjectSpace objectSpace, ITypeInfo objectTypeInfo, Boolean isServerMode, Boolean isAsyncServerMode, CollectionSourceMode mode)
			: this(objectSpace, objectTypeInfo, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, isAsyncServerMode, mode) {
		}
		protected internal CollectionSource(IObjectSpace objectSpace, Type objectType, Boolean isServerMode, Boolean isAsyncServerMode, CollectionSourceMode mode)
			: this(objectSpace, objectType, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, isAsyncServerMode, mode) {
		}
		public CollectionSource(IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode)
			: this(objectSpace, objectType, dataAccessMode, false, mode) {
		}
		public CollectionSource(IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode)
			: this(objectSpace, objectType, dataAccessMode, CollectionSourceMode.Normal) {
		}
		public CollectionSource(IObjectSpace objectSpace, Type objectType, Boolean isServerMode, CollectionSourceMode mode)
			: this(objectSpace, objectType, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, mode) {
		}
		public CollectionSource(IObjectSpace objectSpace, Type objectType, Boolean isServerMode)
			: this(objectSpace, objectType, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client) {
		}
		public CollectionSource(IObjectSpace objectSpace, Type objectType)
			: this(objectSpace, objectType, CollectionSourceDataAccessMode.Client) {
		}
		public override Boolean? IsObjectFitForCollection(Object obj) {
			return objectSpace.IsObjectFitForCriteria(objectTypeInfo.Type, obj, GetExternalCriteria());
		}
		public override void Add(Object obj) {
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				Reload();
			}
			else {
				base.Add(obj);
			}
		}
		public override void Remove(Object obj) {
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				Reload();
			}
			else {
				base.Remove(obj);
			}
		}
		public override ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean IsAsyncServerMode {
			get { return isAsyncServerMode; }
		}
	}
}
