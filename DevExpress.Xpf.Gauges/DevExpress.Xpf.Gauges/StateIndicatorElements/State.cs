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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class State : GaugeDependencyObject {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
		   typeof(StatePresentation), typeof(State), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty ElementInfoProperty = DependencyPropertyManager.Register("ElementInfo",
		   typeof(StateInfo), typeof(State), new PropertyMetadata(null));
		[Category(Categories.Presentation)]
		public StatePresentation Presentation {
			get { return (StatePresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public StateInfo ElementInfo {
			get { return (StateInfo)GetValue(ElementInfoProperty); }
			set { SetValue(ElementInfoProperty, value); }
		}
		static void PresentationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			State state = d as State;
			if (state != null && state.ElementInfo != null) {
				StatePresentation presentation = e.NewValue as StatePresentation;
				state.ElementInfo.Presentation = presentation;
				state.ElementInfo.PresentationControl = presentation != null ? presentation.CreateStatePresentationControl() : null;
			}
		}
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedStatePresentations.PresentationKinds; }
		}
		public State() {
		}
		protected override GaugeDependencyObject CreateObject() {
			return new State();
		}
	}
	public class StateCollection : GaugeDependencyObjectCollection<State> {
		StateIndicatorControl StateIndicator { get { return Owner as StateIndicatorControl; } }
		public StateCollection(StateIndicatorControl stateIndicator) {
			((IOwnedElement)this).Owner = stateIndicator;
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (StateIndicator != null)
				StateIndicator.UpdateStates();
		}
	}
	public class StateInfo : ElementInfoBase {
		public StateInfo (ILayoutCalculator layoutCalculator, int zIndex, PresentationControl presentationControl, PresentationBase presentation)
			: base(layoutCalculator, zIndex, presentationControl, presentation) {
		}
	}
}
