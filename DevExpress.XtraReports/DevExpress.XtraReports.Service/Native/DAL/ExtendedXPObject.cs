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
using System.Linq.Expressions;
using System.Threading;
using DevExpress.Xpo;
namespace DevExpress.XtraReports.Service.Native.DAL {
	[NonPersistent]
	[DeferredDeletion(false)]
	public abstract class ExtendedXPObject : XPObject {
		protected ExtendedXPObject(Session session)
			: base(session) {
		}
		protected bool SetPropertyValue<T>(Expression<Func<T>> property, ref T propertyValueHolder, T newValue) {
			var propertyName = property.GetPropertyName();
			var result = SetPropertyValue(propertyName, ref propertyValueHolder, newValue);
			if(result) {
				OnChangedProperty(propertyName);
			}
			return result;
		}
		protected bool SetDelayedPropertyValue<T>(Expression<Func<T>> property, T newValue) {
			var propertyName = property.GetPropertyName();
			var result = SetDelayedPropertyValue(propertyName, newValue);
			if(result) {
				OnChangedProperty(propertyName);
			}
			return result;
		}
		protected T GetDelayedPropertyValue<T>(Expression<Func<T>> property) {
			var propertyName = property.GetPropertyName();
			return GetDelayedPropertyValue<T>(propertyName);
		}
		protected XPCollection<T> GetCollection<T>(Expression<Func<IEnumerable<T>>> property)
			where T : class {
			var propertyName = property.GetPropertyName();
			return GetCollection<T>(propertyName);
		}
		protected IList<T> GetList<T>(Expression<Func<IEnumerable<T>>> property)
			where T : class {
			var propertyName = property.GetPropertyName();
			return GetList<T>(propertyName);
		}
		protected virtual void OnChangedProperty(string propertyName) {
		}
		protected string GetInformation() {
			return string.Format(
				"Oid:{0}; OptimisticLockField:{1}; OptimisticLockFieldInDataLayer:{2}; ThreadId:{3}",
				Oid,
				GetMember(ClassInfo.OptimisticLockFieldName),
				GetMember(ClassInfo.OptimisticLockFieldInDataLayerName),
				Thread.CurrentThread.ManagedThreadId);
		}
		int GetMember(string memberName) {
			var value = ClassInfo.GetMember(memberName).GetValue(this);
			return value is int ? (int)value : -1;
		}
	}
}
