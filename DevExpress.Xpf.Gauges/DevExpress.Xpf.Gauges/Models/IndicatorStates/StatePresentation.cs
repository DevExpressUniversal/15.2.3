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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class StatePresentation : PresentationBase {
		protected internal abstract PresentationControl CreateStatePresentationControl();
	}
	public abstract class PredefinedStatePresentation : StatePresentation {
	}
	public class TrafficLightsOffStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Traffic Lights Off State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new TrafficLightsOffStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new TrafficLightsOffStatePresentation();
		}
	}
	public class TrafficLightsRedStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Traffic Lights Red State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new TrafficLightsRedStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new TrafficLightsRedStatePresentation();
		}
	}
	public class TrafficLightsYellowStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Traffic Lights Yellow State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new TrafficLightsYellowStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new TrafficLightsYellowStatePresentation();
		}
	}
	public class TrafficLightsGreenStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Traffic Lights Green State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new TrafficLightsGreenStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new TrafficLightsGreenStatePresentation();
		}
	}
	public class TrafficLightsDefaultStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Traffic Lights Default State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new TrafficLightsDefaultStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new TrafficLightsDefaultStatePresentation();
		}
	}
	public class ArrowUpStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Up State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowUpStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowUpStatePresentation();
		}
	}
	public class ArrowDownStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Down State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowDownStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowDownStatePresentation();
		}
	}
	public class ArrowLeftStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Left State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowLeftStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowLeftStatePresentation();
		}
	}
	public class ArrowRightStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Right State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowRightStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowRightStatePresentation();
		}
	}
	public class ArrowLeftUpStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Left Up State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowLeftUpStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowLeftUpStatePresentation();
		}
	}
	public class ArrowRightUpStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Right Up State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowRightUpStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowRightUpStatePresentation();
		}
	}
	public class ArrowRightDownStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Right Down State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowRightDownStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowRightDownStatePresentation();
		}
	}
	public class ArrowLeftDownStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Left Down State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowLeftDownStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowLeftDownStatePresentation();
		}
	}
	public class ArrowDefaultStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Arrow Default Down State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new ArrowDefaultStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArrowDefaultStatePresentation();
		}
	}
	public class LampOffStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Lamp Off State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new LampOffStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LampOffStatePresentation();
		}
	}
	public class LampRedStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Lamp Red State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new LampRedStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LampRedStatePresentation();
		}
	}
	public class LampYellowStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Lamp Yellow State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new LampYellowStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LampYellowStatePresentation();
		}
	}
	public class LampGreenStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Lamp Green State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new LampGreenStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LampGreenStatePresentation();
		}
	}
	public class LampDefaultStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Lamp Default State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new LampDefaultStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LampDefaultStatePresentation();
		}
	}
	public class SmileHappyStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Smile Happy State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new SmileHappyStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmileHappyStatePresentation();
		}
	}
	public class SmileGladStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Smile Glad State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new SmileGladStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmileGladStatePresentation();
		}
	}
	public class SmileIndifferentStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Smile Indifferent State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new SmileIndifferentStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmileIndifferentStatePresentation();
		}
	}
	public class SmileSadStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Smile Sad State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new SmileSadStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmileSadStatePresentation();
		}
	}
	public class SmileDefaultStatePresentation : PredefinedStatePresentation {
		public override string PresentationName { get { return "Smile Default State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new SmileDefaultStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmileDefaultStatePresentation();
		}
	}
	public class DefaultStatePresentation : StatePresentation {
		public override string PresentationName { get { return "Default State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			return new DefaultStatePresentationControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultStatePresentation();
		}
	}
	public class CustomStatePresentation : StatePresentation {
		public static readonly DependencyProperty StateTemplateProperty = DependencyPropertyManager.Register("StateTemplate",
			typeof(ControlTemplate), typeof(CustomStatePresentation));
		[Category(Categories.Common)]
		public ControlTemplate StateTemplate {
			get { return (ControlTemplate)GetValue(StateTemplateProperty); }
			set { SetValue(StateTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom State"; } }
		protected internal override PresentationControl CreateStatePresentationControl() {
			CustomPresentationControl stateControl = new CustomPresentationControl();
			stateControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("StateTemplate") { Source = this });
			return stateControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomStatePresentation();
		}
	}
}
