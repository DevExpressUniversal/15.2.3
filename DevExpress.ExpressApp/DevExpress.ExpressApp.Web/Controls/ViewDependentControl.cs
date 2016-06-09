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
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Web.Controls {
	public interface IViewDependentControl {
		void SetView(View view);
	}
	public abstract class ViewDependentControl<ControlType> : Control, IViewDependentControl, INamingContainer where ControlType : Control {
		private ControlType control;
		private void RecreateControls() {
			if(control == null) {
				control = CreateControl();
				SetupControl();
				Controls.Add(Control);
			}
		}
		protected virtual void SetupControl() { }
		protected abstract ControlType CreateControl();
		public abstract void SetView(View view);
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(!DesignMode) {
				IViewDependentControlsHolder holder = WebActionContainerHelper.FindParent<IViewDependentControlsHolder>(this);
				if(holder != null) {  
					holder.Register(this);
				}
			}
		}
		public override void Dispose() {
			if(control != null) {
				control.Dispose();
				control = null;
			}
			base.Dispose();
		}
		public ControlType Control {
			get { return control; }
		}
		public ViewDependentControl()
			: base() {
			RecreateControls();
		}
	}
	public abstract class SimpleViewDependentControl<ControlType> : ViewDependentControl<ControlType> where ControlType : Control, new() {
		protected override ControlType CreateControl() {
			return new ControlType();
		}
	}
}
