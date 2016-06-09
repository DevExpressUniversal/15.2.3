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

using System.Collections.Generic;
namespace DevExpress.Xpf.Gauges {
	public abstract class StateIndicatorModel : GaugeModelBase {
		protected State defaultState;
		protected List<State> predefinedStates = new List<State>();		
		internal List<State> PredefinedStates { get { return predefinedStates; } }
		internal State DefaultState { get { return defaultState; } }
	}
	public class EmptyStateIndicatorModel : StateIndicatorModel {
		public override string ModelName { get { return "Empty"; } }
		public EmptyStateIndicatorModel() {
			DefaultStyleKey = typeof(EmptyStateIndicatorModel);
			defaultState = new State() { Presentation = new DefaultStatePresentation() };
		}
	}
	public class TrafficLightsStateIndicatorModel : StateIndicatorModel {
		public override string ModelName { get { return "Traffic Lights"; } }
		public TrafficLightsStateIndicatorModel() {
			DefaultStyleKey = typeof(TrafficLightsStateIndicatorModel);
			defaultState = new State() { Presentation = new TrafficLightsDefaultStatePresentation() };
			predefinedStates.Add(new State() { Presentation = new TrafficLightsOffStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new TrafficLightsGreenStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new TrafficLightsYellowStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new TrafficLightsRedStatePresentation() });			
		}
	}
	public class LampStateIndicatorModel : StateIndicatorModel {
		public override string ModelName { get { return "Lamp"; } }
		public LampStateIndicatorModel() {
			DefaultStyleKey = typeof(LampStateIndicatorModel);
			defaultState = new State() { Presentation = new LampDefaultStatePresentation() };
			predefinedStates.Add(new State() { Presentation = new LampOffStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new LampGreenStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new LampYellowStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new LampRedStatePresentation() });			
		}
	}
	public class SmileStateIndicatorModel : StateIndicatorModel {
		public override string ModelName { get { return "Smile"; } }
		public SmileStateIndicatorModel() {
			DefaultStyleKey = typeof(SmileStateIndicatorModel);
			defaultState = new State() { Presentation = new SmileDefaultStatePresentation() };
			predefinedStates.Add(new State() { Presentation = new SmileHappyStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new SmileGladStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new SmileIndifferentStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new SmileSadStatePresentation() });
		}
	}
	public class ArrowStateIndicatorModel : StateIndicatorModel {
		public override string ModelName { get { return "Arrow"; } }
		public ArrowStateIndicatorModel() {
			DefaultStyleKey = typeof(ArrowStateIndicatorModel);
			defaultState = new State() { Presentation = new ArrowDefaultStatePresentation() };
			predefinedStates.Add(new State() { Presentation = new ArrowUpStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowDownStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowLeftStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowRightStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowLeftUpStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowRightUpStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowRightDownStatePresentation() });
			predefinedStates.Add(new State() { Presentation = new ArrowLeftDownStatePresentation() });
		}
	}
}
