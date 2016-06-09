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
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.StateMachine.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.StateMachine.Xpo {
	[DefaultProperty("Caption")]
	[System.ComponentModel.DisplayName("State")]
	[ImageName("BO_State")]
	[RuleIsReferenced("StateIsReferencedXpo", DefaultContexts.Delete, typeof(XpoTransition), "TargetState", InvertResult = true, MessageTemplateMustBeReferenced = "If you want to delete this State, you must be sure it is not referenced in any Transition as TargetState.", FoundObjectMessageFormat = "{0:SourceState.Caption}")]
	public class XpoState : StateMachineObjectBase, IState, IStateAppearancesProvider {
		private XpoStateMachine machine;
		private IObjectSpace objectSpace;
		protected override void OnLoaded() {
			base.OnLoaded();
			objectSpace = XPObjectSpace.FindObjectSpaceByObject(this);
		}
		public XpoState(Session session)
			: base(session) {
		}
		public override void AfterConstruction() {
			base.AfterConstruction();
			objectSpace = XPObjectSpace.FindObjectSpaceByObject(this);
		}
		[RuleRequiredField("XpoState.Caption", DefaultContexts.Save)]
		public string Caption {
			get {
				string caption = GetPropertyValue<string>("Caption");
				if(string.IsNullOrEmpty(caption) && Marker != null) {
					return Marker.DisplayName;
				}
				return caption;
			}
			set { SetPropertyValue<string>("Caption", value); }
		}
		[Association("StateMachine-States")]
		public XpoStateMachine StateMachine {
			get { return machine; }
			set { SetPropertyValue("Machine", ref machine, value); }
		}
		[Browsable(false)]
		public string MarkerValue {
			get { return GetPropertyValue<string>("MarkerValue"); }
			set { SetPropertyValue<string>("MarkerValue", value); }
		}
		[NonPersistent, DataSourceProperty("AvailableMarkerObjects")]
		[ImmediatePostData]
		public MarkerObject Marker {
			get {
				return new StateMachineLogic(objectSpace).GetMarkerObjectFromMarkerValue(MarkerValue, this, objectSpace);
			}
			set {
				MarkerValue = new StateMachineLogic(objectSpace).GetMarkerValueFromMarkerObject(value, this, objectSpace);
				OnChanged("Caption");
			} 
		}
		[CriteriaOptions("StateMachine.TargetObjectType")]
		[Size(SizeAttribute.Unlimited)]
		public string TargetObjectCriteria {
			get { return GetPropertyValue<string>("TargetObjectCriteria"); }
			set { SetPropertyValue("TargetObjectCriteria", value); }
		}
		[Browsable(false)]
		public IList<MarkerObject> AvailableMarkerObjects {
			get { return new StateMachineLogic(objectSpace).GetAvailableMarkerObjects(this, objectSpace); }
		}
		[Association("State-Transitions"), Aggregated]
		public XPCollection<XpoTransition> Transitions {
			get { return GetCollection<XpoTransition>("Transitions"); }
		}
		[Association("State-AppearanceDescriptions"), Aggregated]
		public XPCollection<XpoStateAppearance> Appearance {
			get { return GetCollection<XpoStateAppearance>("Appearance"); }
		}
		#region IState Members
		object IState.Marker {
			get { return Marker != null ? Marker.Marker : null; }
		}
		IList<ITransition> IState.Transitions {
			get {
				List<ITransition> result = new List<ITransition>();
				foreach(XpoTransition transition in Transitions) {
					result.Add(transition);
				}
				return result;
			}
		}
		IStateMachine IState.StateMachine {
			get { return StateMachine; }
		}
		#endregion
		#region IStateAppearancesProvider Members
		IList<IAppearanceRuleProperties> IStateAppearancesProvider.Appearances {
			get {
				List<IAppearanceRuleProperties> result = new List<IAppearanceRuleProperties>();
				foreach(XpoStateAppearance xpoStateAppearance in Appearance) {
					result.Add(xpoStateAppearance);
				}
				return result;
			}
		}
		#endregion
		public void AddTransition(XpoState targetState) {
			XpoTransition transition = new XpoTransition(Session);
			transition.TargetState = targetState;
			Transitions.Add(transition);
		}
		public void AddTransition(XpoState targetState, string caption, int index) {
			XpoTransition transition = new XpoTransition(Session);
			transition.TargetState = targetState;
			transition.Caption = caption;
			transition.Index = index;
			Transitions.Add(transition);
		}
		public override String ToString() {
			return String.Format("{0} ({1})", Caption, StateMachine != null ? StateMachine.Name : string.Empty);
		}
	}
}
