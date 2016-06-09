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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.StateMachine.Dc {
	[DomainComponent]
	[ImageName("BO_StateMachine")]
	[RuleCriteria("IDCStateMachine.StartState", DefaultContexts.Save, "Active = false Or (StartState is not null And Active = true)", SkipNullOrEmptyValues = false)]
	public interface IDCStateMachine : IStateMachine, IStateMachineUISettings {
		new string Name { get;  set; }
		new bool Active { get;  set; }
		new bool ExpandActionsInDetailView { get; set; }
		[RuleRequiredField("IDCStateMachine.TargetObjectType", DefaultContexts.Save)]
		[ValueConverter(typeof(TypeToStringConverter))]
		[TypeConverter(typeof(StateMachineTypeConverter))]
		[ImmediatePostData]
		new Type TargetObjectType { get; set; }
		[Browsable(false)]
		IList<StringObject> AvailableStatePropertyNames { get; }
		[RuleRequiredField("IDCStateMachine.StatePropertyName", DefaultContexts.Save)]
		[ValueConverter(typeof(StringObjectToStringConverter))]
		[DataSourceProperty("AvailableStatePropertyNames")]
		new StringObject StatePropertyName { get; set; }
		[DataSourceProperty("States")]
		IDCState StartState { get; set; }
		[RuleUniqueValue("IDCStateMachine.UniqueStateMarker", DefaultContexts.Save, TargetPropertyName = "MarkerValue")]
		new IList<IDCState> States { get; }
	}
	[DomainLogic(typeof(IDCStateMachine))]
	public static class DCStateMachineDomainLogic {
		public static IState FindCurrentState(IDCStateMachine dcStateMachine, IObjectSpace objectSpace, object targetObject) {
			return new StateMachineLogic(objectSpace).FindCurrentState(dcStateMachine, targetObject, dcStateMachine.StartState);
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static void ExecuteTransition(IDCStateMachine dcStateMachine, IObjectSpace objectSpace, object targetObject, IState targetState) {
			new StateMachineLogic(objectSpace).ExecuteTransition(targetObject, targetState);
		}
		public static IList<IState> Get_States(IDCStateMachine dcStateMachine) {
			IList<IState> result = new List<IState>();
			foreach(IDCState persistentState in dcStateMachine.States) {
				result.Add(persistentState);
			}
			return result;
		}
		public static IList<StringObject> Get_AvailableStatePropertyNames(IDCStateMachine dcStateMachine, IObjectSpace objectSpace) {
			List<StringObject> result = new List<StringObject>();
			if(dcStateMachine.TargetObjectType != null) {
				foreach(string item in new StateMachineLogic(objectSpace).FindAvailableStatePropertyNames(dcStateMachine.TargetObjectType)) {
					result.Add(new StringObject(item));
				}
			}
			return result;
		}
		public static string Get_StatePropertyName(IDCStateMachine dcStateMachine) {
			return dcStateMachine.StatePropertyName != null ? dcStateMachine.StatePropertyName.Name : "";
		}
	}
}
