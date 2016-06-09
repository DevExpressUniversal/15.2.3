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

using System;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public enum ControlContentAnchorType { Fit, Snap };
	#region ControlLayoutOptions (abstract class)
	public abstract class ControlLayoutOptions : SchedulerNotificationOptions {
		#region Constants
		public static readonly ControlContentLayoutType DefaultLayoutType = ControlContentLayoutType.Fit;
		public static readonly ControlContentAnchorType DefaultAnchorType = ControlContentAnchorType.Fit;
		#endregion
		#region Fields
		ReportViewControlBase owner;
		ReportViewControlBase masterControl;
		ControlContentLayoutType layoutType = DefaultLayoutType;
		#endregion
		protected ControlLayoutOptions(ReportViewControlBase owner) {
			if (owner == null)
				Exceptions.ThrowArgumentNullException("owner");
			this.owner = owner;
		}
		protected ReportViewControlBase Owner { get { return owner; } }
		protected internal override void ResetCore() {
			this.masterControl = null;
			this.layoutType = DefaultLayoutType;
		}
		#region Properties
		public ControlContentLayoutType LayoutType {
			get { return layoutType; }
			set {
				if (layoutType == value)
					return;
				ControlContentLayoutType oldVal = layoutType;
				layoutType = value;
				OnChanged("LayoutType", oldVal, layoutType);
			}
		}
		public virtual ControlContentAnchorType AnchorType {
			get {
				if (MasterControl != null)
					return CalculateActualAnchorType();
				else
					return DefaultAnchorType;
			}
		}
		protected internal abstract ControlContentAnchorType CalculateActualAnchorType();
		public ReportViewControlBase MasterControl {
			get { return masterControl; }
			set {
				if (masterControl == value)
					return;
				if (RaiseMasterControlChanging(value)) {
					ReportViewControlBase oldVal = this.masterControl;
					UnsubscribeMasterControlEvents(masterControl);
					masterControl = value;
					SubscribeMasterControlEvents(masterControl);
					OnChanged(new BaseOptionChangedEventArgs("MasterControl", oldVal, masterControl));
				}
			}
		}
		#endregion
		#region events
		ReportViewControlCancelEventHandler onMasterControlChanging;
		public event ReportViewControlCancelEventHandler MasterControlChanging { add { onMasterControlChanging += value; } remove { onMasterControlChanging -= value; } }
		protected internal virtual bool RaiseMasterControlChanging(ReportViewControlBase newVal) {
			if (onMasterControlChanging != null) {
				ReportViewControlCancelEventArgs args = new ReportViewControlCancelEventArgs(newVal);
				onMasterControlChanging(this, args);
				return args.Cancel;
			}
			return false;
		}
		#endregion
		private void SubscribeMasterControlEvents(ReportViewControlBase control) {
			if (control == null)
				return;
			control.Disposed += new EventHandler(OnMasterControlDisposed);
		}
		private void UnsubscribeMasterControlEvents(ReportViewControlBase control) {
			if (control == null)
				return;
			control.Disposed -= new EventHandler(OnMasterControlDisposed);
		}
		void OnMasterControlDisposed(object sender, EventArgs e) {
			this.masterControl = null;
		}
	}
	#endregion
	#region ControlHorizontalLayoutOptions
	public class ControlHorizontalLayoutOptions : ControlLayoutOptions {
		public ControlHorizontalLayoutOptions(ReportViewControlBase owner)
			: base(owner) {
		}
		protected internal override ControlContentAnchorType CalculateActualAnchorType() {
			return Owner.CalculateHorizontalAnchorType();
		}
	}
	#endregion
	#region ControlVerticalLayoutOptions
	public class ControlVerticalLayoutOptions : ControlLayoutOptions {
		public ControlVerticalLayoutOptions(ReportViewControlBase owner)
			: base(owner) {
		}
		protected internal override ControlContentAnchorType CalculateActualAnchorType() {
			return Owner.CalculateVerticalAnchorType();
		}
	}
	#endregion
}
