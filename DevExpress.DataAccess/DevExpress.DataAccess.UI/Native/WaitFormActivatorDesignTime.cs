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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraSplashScreen;
namespace DevExpress.DataAccess.UI.Native {
	public class WaitFormActivatorDesignTime : IWaitFormActivator {
		const int defaultDelay = 1000;
		readonly IWin32Window ownerWindow;
		readonly Type waitFormType;
		readonly Rectangle ownerRect;
		readonly string skinName;
		SplashScreenManager manager;
		public WaitFormActivatorDesignTime(IWin32Window ownerWindow, Type waitFormType, Rectangle ownerRect, string skinName) {
			Guard.ArgumentNotNull(ownerWindow, "ownerWindow");
			this.ownerWindow = ownerWindow;
			this.waitFormType = waitFormType;
			this.ownerRect = ownerRect;
			this.skinName = skinName;
		}
		public WaitFormActivatorDesignTime(IWin32Window ownerWindow, Type waitFormType, string skinName) : this(ownerWindow, waitFormType, new Rectangle(), skinName) { }
		#region IWaitFormActivator Members
		public void CloseWaitForm(bool throwException, int delay, bool waitForClose) {
			SplashScreenManager.CloseForm(throwException, delay, null, waitForClose, true);
			manager = null;
		}
		public void CloseWaitForm() {
			CloseWaitForm(false, 0, false);
		}
		public void EnableCancelButton(bool enable) {
			if(manager != null && manager.IsSplashFormVisible)
				manager.SendCommand(WaitFormCommand.EnableCancelButton, enable);
		}
		public void EnableWaitFormDescription(bool show) {
			if(manager != null && manager.IsSplashFormVisible)
				manager.SendCommand(WaitFormCommand.EnableDescription, show);
		}
		public void SetWaitFormCaption(string caption) {
			if(manager != null && manager.IsSplashFormVisible)
				manager.SetWaitFormCaption(caption);
		}
		public void SetWaitFormDescription(string message) {
			if(manager != null && manager.IsSplashFormVisible)
				manager.SetWaitFormDescription(message);
		}
		public void SetWaitFormObject(ISupportCancel dataSourceLoader) {
			if(manager != null && manager.IsSplashFormVisible)
				manager.SendCommand(WaitFormCommand.SetWaitFormObject, dataSourceLoader);
		}
		public void ShowWaitForm(bool fadeIn, bool fadeOut, bool useDelay) {
			Rectangle rect = this.ownerRect;
			if(!(rect.Width > 0 && rect.Height > 0)) {
				NativeMethods.RECT windowRect = new NativeMethods.RECT();
				if(NativeMethods.GetWindowRect(ownerWindow.Handle, ref windowRect))
					rect = windowRect.ToRectangle();
			}
			SplashScreenManager.SkinName = skinName;
			if(rect.Width > 0 && rect.Height > 0) {
				int x = (rect.Left + rect.Right - 282) / 2;
				int y = (rect.Top + rect.Bottom - 63) / 2;
				SplashScreenManager.ShowForm((Form)null, waitFormType, fadeIn, fadeOut, false, SplashFormStartPosition.Manual, new Point(x, y), useDelay ? defaultDelay : 0, true);
			}
			else
				SplashScreenManager.ShowForm(null, waitFormType, fadeIn, fadeOut, false, useDelay ? defaultDelay : 0);
			manager = SplashScreenManager.Default;
		}
		#endregion
	}
}
