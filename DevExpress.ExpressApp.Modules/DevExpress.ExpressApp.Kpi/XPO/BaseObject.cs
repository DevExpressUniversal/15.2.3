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
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Kpi {
	[NonPersistent]
	public abstract class BaseKpiObject : XPCustomObject {
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), MemberDesignTimeVisibility(false)]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
		[Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
		private Guid oid = Guid.Empty;
		[PersistentAlias("oid"), Browsable(false)]
		public Guid Oid {
			get { return oid; }
		}
#endif
		private bool isDefaultPropertyAttributeInit = false;
		private DevExpress.Xpo.Metadata.XPMemberInfo defaultPropertyMemberInfo;
		protected override void OnSaving() {
			base.OnSaving();
			if(!(Session is NestedUnitOfWork) && Session.IsNewObject(this)) {
				oid = XpoDefault.NewGuid();
			}
		}
		public BaseKpiObject(Session session) : base(session) { }
		public BaseKpiObject() : base() { }
		public override string ToString() {
			if(!isDefaultPropertyAttributeInit) {
				string defaultPropertyName = string.Empty;
				XafDefaultPropertyAttribute xafDefaultPropertyAttribute = XafTypesInfo.Instance.FindTypeInfo(GetType()).FindAttribute<XafDefaultPropertyAttribute>();
				if(xafDefaultPropertyAttribute != null) {
					defaultPropertyName = xafDefaultPropertyAttribute.Name;
				}
				else {
					DefaultPropertyAttribute defaultPropertyAttribute = XafTypesInfo.Instance.FindTypeInfo(GetType()).FindAttribute<DefaultPropertyAttribute>();
					if(defaultPropertyAttribute != null) {
						defaultPropertyName = defaultPropertyAttribute.Name;
					}
				}
				if(!string.IsNullOrEmpty(defaultPropertyName)) {
					defaultPropertyMemberInfo = ClassInfo.FindMember(defaultPropertyName);
				}
				isDefaultPropertyAttributeInit = true;
			}
			if(defaultPropertyMemberInfo != null) {
				object obj = defaultPropertyMemberInfo.GetValue(this);
				if(obj != null) {
					return obj.ToString();
				}
			}
			return base.ToString();
		}
		internal void OnPropertyChanged(string propertyName) {
			OnChanged(propertyName);
		}
	}
}
