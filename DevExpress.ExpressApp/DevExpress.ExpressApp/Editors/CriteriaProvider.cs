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
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Editors {
	[DomainComponent]
	public class CriteriaProvider {
		private object owner;
		private IMemberInfo filteredObjectsTypeMemberInfo;
		private Type dataType;
		private string filterString;
		public CriteriaProvider(object owner, IMemberInfo filterStringMemberInfo, IMemberInfo filteredObjectsTypeMemberInfo) {
			this.owner = owner;
			this.filteredObjectsTypeMemberInfo = filteredObjectsTypeMemberInfo;
			this.filterString = (string)filterStringMemberInfo.GetValue(owner);
			this.dataType = null;
		}
		public CriteriaProvider(Type criteriaObjectType, string criteriaString) {
			this.dataType = criteriaObjectType;
			this.filterString = criteriaString;
		}
		public CriteriaProvider(Type dataType) {
			this.dataType = dataType;
			this.filterString = null;
			this.owner = null;
			this.filteredObjectsTypeMemberInfo = null;
		}
		public Type FilteredObjectsType {
			get {
				return (dataType != null) ? dataType : 
					(filteredObjectsTypeMemberInfo != null && owner != null) ? (Type)filteredObjectsTypeMemberInfo.GetValue(owner) : null;
			}
		}
		[CriteriaOptions("FilteredObjectsType")]
		[EditorAlias(EditorAliases.CriteriaPropertyEditor)]
		public string FilterString {
			get { return filterString; }
			set { filterString = value; }
		}
	}
}
