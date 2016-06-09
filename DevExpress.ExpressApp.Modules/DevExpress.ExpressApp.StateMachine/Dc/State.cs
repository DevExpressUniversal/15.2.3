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
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using DevExpress.ExpressApp.ConditionalAppearance;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.StateMachine.Utils;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.StateMachine.Dc {
	[DomainComponent]
	[ImageName("BO_State")]
	[RuleIsReferenced("StateIsReferencedDc", DefaultContexts.Delete, typeof(IDCTransition), "TargetState", InvertResult = true, MessageTemplateMustBeReferenced = "If you want to delete this State, you must be sure it is not referenced in any Transition as TargetState.", FoundObjectMessageFormat = "{0:SourceState.Caption}")]
	public interface IDCState : IState, IStateAppearancesProvider {
		new string Caption { get; set; }
		[Browsable(false)]
		string MarkerValue { get; set; }
		[NonPersistentDc, DataSourceProperty("AvailableMarkerObjects")]
		MarkerObject MarkerObject { get; set; }
		[CriteriaOptions("StateMachine.TargetObjectType")]
		[Size(SizeAttribute.Unlimited)]
		new string TargetObjectCriteria { get; set; }
		new IDCStateMachine StateMachine { get; set; }
		[BackReferenceProperty("SourceState")]
		new IList<IDCTransition> Transitions { get; }
		new IList<IDCStateAppearance> Appearances { get; }
		[Browsable(false)]
		IList<MarkerObject> AvailableMarkerObjects { get; }
		void AddTransition(IDCState dcTargetState);
	}
	[DomainLogic(typeof(IDCState))]
	public static class DCStateDomainLogic {
		public static IStateMachine Get_StateMachine(IDCState dcState) {
			return dcState.StateMachine;
		}
		public static object Get_Marker(IDCState dcState) {
			return dcState.MarkerObject != null ? dcState.MarkerObject.Marker : null;
		}
		public static IList<ITransition> Get_Transitions(IDCState dcState) {
			List<ITransition> result = new List<ITransition>();
			foreach(IDCTransition transition in dcState.Transitions) {
				result.Add(transition);
			}
			return result;
		}
		public static IList<IAppearanceRuleProperties> Get_Appearances(IDCState dcState) {
			List<IAppearanceRuleProperties> result = new List<IAppearanceRuleProperties>();
			foreach(IDCStateAppearance appearance in dcState.Appearances) {
				result.Add(appearance);
			}
			return result;
		}
		public static MarkerObject Get_MarkerObject(IDCState dcState, IObjectSpace objectSpace) {
			return new StateMachineLogic(objectSpace).GetMarkerObjectFromMarkerValue(dcState.MarkerValue, dcState, objectSpace);
		}
		public static void Set_MarkerObject(IDCState dcState, IObjectSpace objectSpace, MarkerObject value) {
			dcState.MarkerValue = new StateMachineLogic(objectSpace).GetMarkerValueFromMarkerObject(value, dcState, objectSpace);
		}
		public static IList<MarkerObject> Get_AvailableMarkerObjects(IDCState dcState, IObjectSpace objectSpace) {
			return new StateMachineLogic(objectSpace).GetAvailableMarkerObjects(dcState, objectSpace);
		}
		public static void AddTransition(IDCState dcState, IObjectSpace objectSpace, IDCState dcTargetState) {
			IDCTransition transition = objectSpace.CreateObject<IDCTransition>();
			transition.TargetState = dcTargetState;
			dcState.Transitions.Add(transition);
		}
	}
}
