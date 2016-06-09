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

using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.XtraReports.Service.Native.DAL {
	class SmartUnitOfWork : UnitOfWork {
		public SmartUnitOfWork(IDataLayer dataLayer)
			: base(dataLayer) {
		}
		protected override MemberInfoCollection GetPropertiesListForUpdateInsert(object theObject, bool isUpdate, bool addDelayedReference) {
			var enumerateChangedProperties = theObject as IEnumerableChangedProperties;
			return isUpdate && enumerateChangedProperties != null
				? GetPropertiesListForUpdate(enumerateChangedProperties)
				: base.GetPropertiesListForUpdateInsert(theObject, isUpdate, addDelayedReference);
		}
		MemberInfoCollection GetPropertiesListForUpdate(IEnumerableChangedProperties theObject) {
			var classInfo = GetClassInfo(theObject);
			var membersInfo = new XPMemberInfo[theObject.ChangedProperties.Count + 1];
			for(int i = 0; i < theObject.ChangedProperties.Count; i++) {
				membersInfo[i] = classInfo.GetMember(theObject.ChangedProperties[i]);
			}
			membersInfo[theObject.ChangedProperties.Count] = classInfo.GetMember(OptimisticLockingAttribute.DefaultFieldName);
			return new MemberInfoCollection(classInfo, membersInfo);
		}
	}
}
