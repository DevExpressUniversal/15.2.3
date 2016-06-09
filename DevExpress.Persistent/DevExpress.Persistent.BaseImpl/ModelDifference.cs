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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[ImageName("ModelEditor_ModelMerge")]
	[RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "UserId, ContextId")]
	public class ModelDifference : BaseObject, IModelDifference {
		private String userId;
		private String userName;
		private String contextId;
		private Int32 version;
		public ModelDifference(Session session)
			: base(session) {
		}
		[Browsable(false)]
		[ModelDefault("AllowEdit", "False")]
		public String UserId {
			get { return userId; }
			set {
				if(userId != value) {
					userName = "";
				}
				SetPropertyValue("UserId", ref userId, value);
			}
		}
		public String UserName {
			get {
				if(String.IsNullOrEmpty(userId)) {
					userName = ModelDifferenceDbStore.SharedModelDifferenceName;
				}
				else if(String.IsNullOrEmpty(userName)) {
					userName = Convert.ToString(Session.Evaluate(ModelDifferenceDbStore.UserTypeInfo.Type,
						new OperandProperty(ModelDifferenceDbStore.UserNamePropertyName),
						new BinaryOperator(
							ModelDifferenceDbStore.UserTypeInfo.KeyMember.Name,
							ModelDifferenceDbStore.UserIdTypeConverter.ConvertFromInvariantString(userId))
					));
				}
				return userName;
			}
		}
		public String ContextId {
			get { return contextId; }
			set { SetPropertyValue("ContextId", ref contextId, value); }
		}
		[Browsable(false)]
		public Int32 Version {
			get { return version; }
			set { SetPropertyValue("Version", ref version, value); }
		}
		[Association, Aggregated]
		public XPCollection<ModelDifferenceAspect> Aspects {
			get { return GetCollection<ModelDifferenceAspect>("Aspects"); }
		}
		IList<IModelDifferenceAspect> IModelDifference.Aspects {
			get { return Aspects.ToList<IModelDifferenceAspect>(); }
		}
	}
}
