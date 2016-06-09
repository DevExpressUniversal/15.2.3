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

using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.ToolBoxIcons;
namespace DevExpress.Utils.Animation {
	[
		Designer("DevExpress.Utils.Design.TransitionManagerDesigner, " + AssemblyInfo.SRAssemblyDesignFull),
	Description("Allows you to implement animated transitions between the target control's states."),
	ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Regular),
		ToolboxBitmap(typeof(ToolboxIconsRootNS), "TransitionManager")
	]
	public class TransitionManager : Component {
		TransitionController controller;
		TransitionCollection transitionCollection;
		TransitionManagerProperties propertiesCache;
		static readonly object customTransitionCore = new object();		
		public TransitionManager() : base() {
			propertiesCache = new TransitionManagerProperties();
		}
		[ Category("Behavior"),
		Editor("DevExpress.Utils.Design.TransitionCollectionEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TransitionCollection Transitions {
			get {
				if(transitionCollection == null) transitionCollection = new TransitionCollection();
				return transitionCollection;
			}
		}
		[ DefaultValue(10000), Category("Behavior")]
		public int FrameInterval {
			get { return propertiesCache.FrameInterval; }
			set { propertiesCache.FrameInterval = value; }
		}
		[ DefaultValue(1000), Category("Behavior")]
		public int FrameCount {
			get { return propertiesCache.FrameCount; }
			set { propertiesCache.FrameCount = value; }
		}
		[ Category("Behavior"), DefaultValue(true)]
		public bool ShowWaitingIndicator {
			get { return propertiesCache.ShowWaitingIndicator; }
			set { propertiesCache.ShowWaitingIndicator = value; }
		}
		TransitionController Controller {
			get {
				if(controller == null) controller = new TransitionController();
				return controller;
			}
		}
		[Browsable(false)]
		public bool IsTransition { get { return Controller.IsTransition; } }	 
		bool IsDesignMode() {
			return this.DesignMode || DevExpress.Utils.Design.DesignTimeTools.IsDesignMode;
		}
		protected virtual bool CanStartTransition {
			get {
				return !IsDesignMode();
			}
		}
		public void StartTransition(Control control) {
			if(!CanStartTransition) return;
			ITransition animation = Transitions[control];			
			Controller.StartTransition(animation, RaiseCustomTransition, propertiesCache);
		}
		public void StartWaitingIndicator(Control control, IWaitingIndicatorProperties indicatorProperties) {
			Controller.StartWaitingIndicator(control, indicatorProperties);
		}
		public void StartWaitingIndicator(Control control, WaitingAnimatorType indicatorType) {
			IWaitingIndicatorProperties waitingIndicatorProperties = null;
			switch(indicatorType) {
				case WaitingAnimatorType.Line :
					waitingIndicatorProperties = new LineWaitingIndicatorProperties();
					break;
				case WaitingAnimatorType.Ring :
					waitingIndicatorProperties = new RingWaitingIndicatorProperties();
					break;
				case WaitingAnimatorType.Default :
					waitingIndicatorProperties = new WaitingIndicatorProperties();
					break;
			}
			Controller.StartWaitingIndicator(control, waitingIndicatorProperties);
		}
		public void EndTransition() {
			Controller.EndTransition();			
		}
		public void StopWaitingIndicator() {
			Controller.StopWaitingIndicator();
		}	  
		protected virtual CustomTransitionEventArgs RaiseCustomTransition(ITransition transition) {
			CustomTransitionEventArgs args = new CustomTransitionEventArgs();
			CustomTransitionEventHandler handler = Events[customTransitionCore] as CustomTransitionEventHandler;
			if(handler != null)
				handler(transition, args);
			return args;
		}
		[ Category("Events")]
		public event CustomTransitionEventHandler CustomTransition {
			add { Events.AddHandler(customTransitionCore, value); }
			remove { Events.RemoveHandler(customTransitionCore, value); }
		}
		[ Category("Events")]
		public event BeforeTransitionStartsEventHandler BeforeTransitionStarts {
			add { Controller.BeforeTransitionStarts += value; }
			remove { Controller.BeforeTransitionStarts -= value; }
		}
		[ Category("Events")]
		public event AfterTransitionEndsEventHandler AfterTransitionEnds {
			add { Controller.AfterTransitionEnds += value; }
			remove { Controller.AfterTransitionEnds -= value; }
		}
		bool isDisposing;
		protected override void Dispose(bool disposing) {
			if(disposing && !isDisposing) {
				isDisposing = true;
				Ref.Dispose(ref controller);
				Ref.Dispose(ref transitionCollection);
			}
			base.Dispose(disposing);
		}
		[Browsable(false)]
		public bool IsDisposing { get { return isDisposing; } }
	}
	class TransitionManagerProperties : System.IDisposable {
		System.Collections.Hashtable propertyBag;
		bool isDisposing;
		public TransitionManagerProperties() {
			propertyBag = new System.Collections.Hashtable();
			propertyBag.Add("FrameInterval", 10000);
			propertyBag.Add("FramesCount", 1000);
			propertyBag.Add("ShowWaitingIndicator", true);
		}
		object GetValue(string propertyName) {
			return propertyBag[propertyName];
		}
		void SetValue(string propertyName, object value) {
			propertyBag[propertyName] = value;
		}
		public int FrameInterval {
			get { return (int)GetValue("FrameInterval"); }
			set { SetValue("FrameInterval", value); }
		}		
		public int FrameCount {
			get { return (int)GetValue("FramesCount"); }
			set { SetValue("FramesCount", value); }
		}		
		public bool ShowWaitingIndicator {
			get { return (bool)GetValue("ShowWaitingIndicator"); }
			set { SetValue("ShowWaitingIndicator", value); }
		}
		#region IDisposable Members
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				propertyBag.Clear();
				propertyBag = null;
				System.GC.SuppressFinalize(this);
			}
		}
		#endregion
	}
}
