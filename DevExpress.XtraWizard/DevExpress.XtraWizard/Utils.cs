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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
namespace DevExpress.XtraWizard.Internal {
	public class TransitionHelper {
		const int AW_HIDE = 0x10000,
				  AW_BLEND = 0x80000;
		const int SWP_NOSIZE = 0x0001,
				  SWP_NOMOVE = 0x0002, 
				  SWP_NOACTIVATE = 0x0010,
				  SWP_SHOWWINDOW = 0x0040,
				  SWP_HIDEWINDOW = 0x0080;
		Form animatableForm = null;
		Control parentControl;
		Color transitionColor;
		Rectangle transitionBounds;
		int animationInternal;
		public TransitionHelper(Control parentControl, Color transitionColor, Rectangle transitionBounds, int animationInterval) {
			this.parentControl = parentControl;
			this.transitionColor = transitionColor;
			this.transitionBounds = transitionBounds;
			this.animationInternal = animationInterval;
		}
		public void BeginTransition() {
			this.animatableForm = CreateAnimatableForm();
			WizardControlNativeMethods.SetWindowsPos(animatableForm.Handle, parentControl.Handle, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOMOVE | SWP_SHOWWINDOW);
			WizardControlNativeMethods.AnimateWindow(animatableForm.Handle, animationInternal / 2, AW_BLEND);
		}
		public void EndTransition() {
			WizardControlNativeMethods.AnimateWindow(animatableForm.Handle, animationInternal / 2, AW_BLEND | AW_HIDE);
			this.animatableForm.Dispose();
		}
		protected virtual Form CreateAnimatableForm() {
			AnimatableForm form = new AnimatableForm();
			form.ShowInTaskbar = false;
			form.StartPosition = FormStartPosition.Manual;
			form.FormBorderStyle = FormBorderStyle.None;
			form.Bounds = transitionBounds;
			form.BackColor = transitionColor;
			return form;
		}
		public class AnimatableForm : Form {
			public AnimatableForm() {
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			}
		}
	}
}
