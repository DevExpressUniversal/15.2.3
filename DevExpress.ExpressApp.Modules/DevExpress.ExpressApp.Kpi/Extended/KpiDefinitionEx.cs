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

#if DebugTest
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Kpi;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
namespace DevExpress.ExpressApp.Kpi {
	public class KpiDefinitionEx : KpiDefinition {
		private DrilldownProperty activeDrilldown;
		public KpiDefinitionEx(Session session) : base(session) { }
		public override void AfterConstruction() {
			base.AfterConstruction();
			SetKpiInstance(new KpiInstanceEx(Session, this));
		}
		[Association("KpiDefinition-Drilldowns"), DevExpress.Xpo.Aggregated]
		public XPCollection<DrilldownProperty> DrilldownProperties {
			get { return GetCollection<DrilldownProperty>("DrilldownProperties"); }
		}
		public DrilldownProperty ActiveDrilldown {
			get { return activeDrilldown; }
			set { SetPropertyValue("ActiveDrilldown", ref activeDrilldown, value); }
		}
	}
	[DefaultProperty("Caption")]
	public class DrilldownProperty : BaseKpiObject {
		private StringObject propertyName;
		private KpiDefinitionEx kpiDefinition;
		private string caption;
		private string criteria;
		private string settings;
		protected internal IMemberInfo PropertyInfo {
			get {
				if(KpiDefinition != null && KpiDefinition.TargetObjectType != null && PropertyName != null) {
					ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(KpiDefinition.TargetObjectType);
					return targetTypeInfo.FindMember(PropertyName.Name);
				}
				return null;
			}
		}
		public DrilldownProperty(Session session)
			: base(session) { }
		public string Caption {
			get {
				if(!string.IsNullOrEmpty(caption)) {
					return caption;
				}
				IMemberInfo memberInfo = PropertyInfo;
				if(memberInfo != null) {
					if(memberInfo.MemberType.IsClass) {
						return CaptionHelper.GetClassCaption(memberInfo.LastMember.MemberType.FullName);
					} else {
						return CaptionHelper.GetFullMemberCaption(memberInfo.Owner, memberInfo.Name);
					}
				}
				return "";
			}
			set {
				caption = value;
			}
		}
		[DataSourceProperty("AvailableProperties")]
		[ValueConverter(typeof(StringObjectToStringConverter))]
		[ImmediatePostData]
		public StringObject PropertyName {
			get { return propertyName; }
			set {
				SetPropertyValue("PropertyName", ref propertyName, value);
				OnChanged("Caption");
			}
		}
		[CriteriaOptions("TargetType")]
		[Size(SizeAttribute.Unlimited)]
		public string Criteria {
			get { return criteria; }
			set { SetPropertyValue("Criteria", ref criteria, value); }
		}
		[Browsable(false)]
		public Type TargetType {
			get {
				if(PropertyInfo != null) {
					return PropertyInfo.LastMember.MemberType;
				}
				return null;
			}
		}
		[Browsable(false)]
		[Association("KpiDefinition-Drilldowns")]
		public KpiDefinitionEx KpiDefinition {
			get { return kpiDefinition; }
			set { SetPropertyValue("KpiDefinition", ref kpiDefinition, value); }
		}
		[DataSourceProperty("AvailableProperties")]
		[Browsable(false)]
		public IList<StringObject> AvailableProperties {
			get {
				List<StringObject> result = new List<StringObject>();
				if(KpiDefinition != null && KpiDefinition.TargetObjectType != null) {
					ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(KpiDefinition.TargetObjectType);
					foreach(IMemberInfo memberInfo in targetTypeInfo.Members) {
						if(memberInfo.IsVisible && memberInfo.IsProperty && memberInfo.IsPublic && !memberInfo.IsList) {
							result.Add(new StringObject(memberInfo.Name));
						}
					}
				}
				return result;
			}
		}
		[Size(SizeAttribute.Unlimited)]
		[Browsable(false)]
		public virtual string Settings {
			get { return settings; }
			set { SetPropertyValue("Settings", ref settings, value); }
		}
	}
}
#endif
