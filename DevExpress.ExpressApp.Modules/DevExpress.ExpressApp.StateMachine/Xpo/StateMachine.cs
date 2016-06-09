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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine.Utils;
using System.Reflection;
using DevExpress.ExpressApp.Xpo;
namespace DevExpress.ExpressApp.StateMachine.Xpo {
	[NavigationItem("State Machine")]
	[DefaultProperty("Name")]
	[System.ComponentModel.DisplayName("State Machine")]
	[ImageName("BO_StateMachine")]
	[RuleCriteria("XpoStateMachine.StartState", DefaultContexts.Save, "Active = false Or (StartState is not null And Active = true)", SkipNullOrEmptyValues = false)]
	public class XpoStateMachine : StateMachineObjectBase, IStateMachine, IStateMachineUISettings {
		private IObjectSpace objectSpace;
		protected override void OnLoaded() {
			base.OnLoaded();
			objectSpace = XPObjectSpace.FindObjectSpaceByObject(this);
		}
		public XpoStateMachine(Session session) : base(session) { }
		public override void AfterConstruction() {
			base.AfterConstruction();
			objectSpace = XPObjectSpace.FindObjectSpaceByObject(this);
		}
		public string Name {
			get { return GetPropertyValue<string>("Name");}
			set { SetPropertyValue<string>("Name", value);}
		}
		public bool Active {
			get { return GetPropertyValue<bool>("Active"); }
			set { SetPropertyValue<bool>("Active", value); }
		}
		[RuleRequiredField("XpoStateMachine.TargetObjectType", DefaultContexts.Save)]
		[ValueConverter(typeof(TypeToStringConverter))]
		[TypeConverter(typeof(StateMachineTypeConverter))]
		[ImmediatePostData]
		public Type TargetObjectType {
			get { return GetPropertyValue<Type>("TargetObjectType"); }
			set { SetPropertyValue<Type>("TargetObjectType", value); }
		}
		[RuleRequiredField("XpoStateMachine.StatePropertyName", DefaultContexts.Save)]
		[ValueConverter(typeof(StringObjectToStringConverter))]
		[DataSourceProperty("AvailableStatePropertyNames")]
		public StringObject StatePropertyName {
			get { return GetPropertyValue<StringObject>("StatePropertyName"); }
			set { SetPropertyValue<StringObject>("StatePropertyName", value); }
		}
		[Browsable(false)]
		public IList<StringObject> AvailableStatePropertyNames {
			get {
				List<StringObject> result = new List<StringObject>();
				if(TargetObjectType != null) {
					foreach(string item in new StateMachineLogic(objectSpace).FindAvailableStatePropertyNames(TargetObjectType)) {
						result.Add(new StringObject(item));
					}
				}
				return result; 
			}
		}
		[DataSourceProperty("States")]
		public XpoState StartState {
			get { return GetPropertyValue<XpoState>("StartState");}
			set { SetPropertyValue<XpoState>("StartState", value); }
		}
		[Association("StateMachine-States"), Aggregated]
		[RuleUniqueValue("XpoStateMachine.UniqueStateMarker", DefaultContexts.Save, TargetPropertyName = "MarkerValue")]
		public XPCollection<XpoState> States {
			get { return GetCollection<XpoState>("States"); }
		}
		public bool ExpandActionsInDetailView {
			get { return GetPropertyValue<bool>("ExpandActionsInDetailView"); }
			set { SetPropertyValue<bool>("ExpandActionsInDetailView", value); }
		}
		#region IStateMachine Members
		IList<IState> IStateMachine.States {
			get {
				List<IState> result = new List<IState>();
				foreach(XpoState state in States) {
					result.Add(state);
				}
				return result;
			}
		}
		string IStateMachine.StatePropertyName {
			get { return StatePropertyName != null ? StatePropertyName.Name : ""; }
		}
		public IState FindCurrentState(object targetObject) {
			return new StateMachineLogic(objectSpace).FindCurrentState(this, targetObject, StartState);
		}
		public void ExecuteTransition(object targetObject, IState targetState) {
			new StateMachineLogic(objectSpace).ExecuteTransition(targetObject, targetState);
		}
		#endregion
	}
}
