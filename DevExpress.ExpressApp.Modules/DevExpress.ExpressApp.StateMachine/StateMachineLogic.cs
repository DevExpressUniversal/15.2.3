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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.StateMachine.Utils;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.StateMachine.Resources;
namespace DevExpress.ExpressApp.StateMachine {
	public class StateMachineLogic {
		private IObjectSpace objectSpace;
		private IMemberInfo FindStateMemberInfo(Type targetObjectType, string statePropertyName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(targetObjectType);
			if(typeInfo != null) {
				return typeInfo.FindMember(statePropertyName);
			}
			return null;
		}
		public RuleSetValidationResult ValidateTransition(IState targetState, object targetObject) {
			if(!string.IsNullOrEmpty(targetState.TargetObjectCriteria)) {
				string context = targetState.Caption;
				string ruleId = "StateMachine." + context;
				RuleCriteria rule = new RuleCriteria(ruleId, context, targetObject.GetType());
				rule.Properties.SkipNullOrEmptyValues = false;
				string messageTemplate = CaptionHelper.GetLocalizedText("StateMachine", "TransitionValidationMessage", "To go to the '{StateCaption}' state, the '{TargetObject}' must satisfy the following criteria: {Criteria}");
				rule.Properties.MessageTemplateMustSatisfyCriteria = messageTemplate.Replace("{StateCaption}", targetState.Caption);
				rule.Properties.Criteria = targetState.TargetObjectCriteria;
				Validator.RuleSet.RegisteredRules.Add(rule);
				RuleSetValidationResult validationResult = null;
				if(objectSpace != null) {
					using(objectSpace.CreateParseCriteriaScope()) { 
						validationResult = Validator.RuleSet.ValidateTarget(objectSpace, targetObject, context);
					}
				} else {
					validationResult = Validator.RuleSet.ValidateTarget(objectSpace, targetObject, context);
				}
				Validator.RuleSet.RegisteredRules.Remove(rule);
				return validationResult;
			}
			return new RuleSetValidationResult();
		}
		protected virtual void ProcessTransition(object targetObject, string statePropertyName, IState targetState) {
			IMemberInfo stateMemberInfo = FindStateMemberInfo(targetObject.GetType(), statePropertyName);
			Guard.ArgumentNotNull(stateMemberInfo, "stateMemberInfo");
			stateMemberInfo.SetValue(targetObject, targetState.Marker);
		}
		protected internal bool StateMarkersAreEqual(object marker1, object marker2) {
			if(marker1 == null) {
				if(marker2 == null) {
					return true;
				} else {
					return false;
				}
			} else {
				if(marker2 == null) {
					return false;
				}
			}
			if(marker1.GetType() != marker2.GetType()) {
				return false;
			}
			DevExpress.ExpressApp.DC.ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(marker1.GetType());
			if(typeInfo != null && typeInfo.IsPersistent) {
				return objectSpace.GetKeyValueAsString(marker1) == objectSpace.GetKeyValueAsString(marker2);
			}
			return object.Equals(marker1, marker2);
		}
		public StateMachineLogic(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public virtual IList<string> FindAvailableStatePropertyNames(Type targetObjectType) {
			List<string> result = new List<string>();
			ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(targetObjectType);
			if(targetTypeInfo != null) {
				foreach(IMemberInfo memberInfo in targetTypeInfo.Members) {
					if(memberInfo.IsPublic && memberInfo.IsVisible && (memberInfo.MemberType.IsEnum || memberInfo.MemberTypeInfo.IsPersistent)) {
						result.Add(memberInfo.Name);
					}
				}
			}
			return result;
		}
		public virtual IState FindCurrentState(IStateMachine stateMachine, object targetObject, IState defauleState) {
			Guard.ArgumentNotNull(targetObject, "targetObject");
			Guard.TypeArgumentIs(stateMachine.TargetObjectType, targetObject.GetType(), "targetObject");
			IMemberInfo stateMemberInfo = FindStateMemberInfo(stateMachine.TargetObjectType, stateMachine.StatePropertyName);
			if (stateMemberInfo == null) {
				throw new ArgumentException(string.Format(StateMachineLocalizer.Active.GetLocalizedString(StateMachineLocalizerId.CannotFindCurrentStateMember), stateMachine.StatePropertyName, stateMachine.TargetObjectType));
			}
			object stateMarker = stateMemberInfo.GetValue(targetObject);
			if(stateMarker != null) {
				foreach(IState state in stateMachine.States) {
					if(StateMarkersAreEqual(stateMarker, state.Marker)) {
						return state;
					}
				}
			}
			return defauleState;
		}
		public virtual void ExecuteTransition(object targetObject, IState targetState) {
			Guard.ArgumentNotNull(targetObject, "targetObject");
			Guard.ArgumentNotNull(targetState, "targetState");
			if(!targetState.StateMachine.Active) {
				throw new InvalidOperationException(string.Format("Trying to execute transition with inactive state machine {0}.", targetState.StateMachine.Name));
			}
			RuleSetValidationResult validationResult = ValidateTransition(targetState, targetObject);
			if(validationResult.State == ValidationState.Invalid) {
				throw new ValidationException(validationResult);
			} else {
				ProcessTransition(targetObject, targetState.StateMachine.StatePropertyName, targetState);
			}
		}
		public Type GetStatePropertyType(IStateMachine stateMachine) {
			if(stateMachine != null && stateMachine.TargetObjectType != null && !string.IsNullOrEmpty(stateMachine.StatePropertyName)) {
				IMemberInfo stateMemberInfo = FindStateMemberInfo(stateMachine.TargetObjectType, stateMachine.StatePropertyName);
				if(stateMemberInfo != null) {
					return stateMemberInfo.MemberType;
				}
			}
			return null;
		}
		public IList<MarkerObject> GetAvailableMarkerObjects(IState state, IObjectSpace objectSpace) {
			List<MarkerObject> result = new List<MarkerObject>();
			Type statePropertyType = GetStatePropertyType(state.StateMachine);
			if(statePropertyType != null) {
				DevExpress.ExpressApp.DC.ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(statePropertyType);
				if(typeInfo != null) {
					if(typeInfo.IsPersistent) {
						foreach(object obj in objectSpace.GetObjects(statePropertyType)) {
							result.Add(new MarkerObject(obj));
						}
					} else if(statePropertyType.IsEnum) {
						foreach(object enumValue in Enum.GetValues(statePropertyType)) {
							result.Add(new MarkerObject(enumValue, new EnumDescriptor(statePropertyType).GetCaption(enumValue)));
						}
					}
				}
			}
			return result;
		}
		public MarkerObject GetMarkerObjectFromMarkerValue(string markerValue, IState state, IObjectSpace objectSpace) {
			Type statePropertyType = GetStatePropertyType(state.StateMachine);
			if(statePropertyType != null && statePropertyType.IsEnum) {
				object enumValue = markerValue != null ? Enum.Parse(statePropertyType, markerValue) : null;
				return enumValue != null ? new MarkerObject(enumValue) : null;
			} else {
				MarkerObject markerObject = null;
				try {
					if(!string.IsNullOrEmpty(markerValue)) {
						markerObject = new MarkerObject(objectSpace.GetObjectByHandle(markerValue));
					}
				}
				catch(Exception) { }
				return markerObject;
			}
		}
		public string GetMarkerValueFromMarkerObject(MarkerObject markerObject, IState state, IObjectSpace objectSpace) {
			Type statePropertyType = GetStatePropertyType(state.StateMachine);
			bool isEnum = false;
			if(statePropertyType != null) {
				isEnum = statePropertyType.IsEnum;
			}
			else {
				isEnum = (state.StateMachine == null && markerObject != null && markerObject.Marker != null && markerObject.Marker.GetType().IsEnum);
			}
			if(isEnum) {
				return markerObject.Marker != null ? markerObject.Marker.ToString() : null;
			} else {
				return markerObject.Marker != null ? objectSpace.GetObjectHandle(markerObject.Marker) : null;
			}
		}
		public static ITransition FindTransition(IStateMachine stateMachine, object targetObject, string caption) {
			IList<ITransition> availableTransitions = stateMachine.FindCurrentState(targetObject).Transitions;
			foreach(ITransition transition in availableTransitions) {
				if(string.Compare(transition.Caption, caption, true) == 0) {
					return transition;
				}
			}
			return null;
		}
	}
}
