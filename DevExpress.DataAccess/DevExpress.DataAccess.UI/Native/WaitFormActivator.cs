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
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.DataAccess.Native;
using DevExpress.Utils.Design;
using System.Drawing;
namespace DevExpress.DataAccess.UI.Native {
	public class WaitFormActivator : IWaitFormActivator {
		const int defaultDelay = 1000;
		readonly Form ownerForm;
		readonly Type waitFormType;
		readonly bool allowInDT;
		SplashScreenManager manager;
		public WaitFormActivator(Form ownerForm, Type waitFormType)
			: this(ownerForm, waitFormType, false) {
		}
		public WaitFormActivator(Form ownerForm, Type waitFormType, bool allowInDT) {
			this.ownerForm = ownerForm;
			this.waitFormType = waitFormType;
			this.allowInDT = allowInDT;
		}
		bool CheckDesignTime() {
			return DesignTimeTools.IsDesignMode && !allowInDT;
		}
		public void SetWaitFormObject(ISupportCancel dataSourceLoader) {
			if(CheckDesignTime())
				return;
			if(manager != null )
				manager.SendCommand(WaitFormCommand.SetWaitFormObject, dataSourceLoader);
		}
		public void SetWaitFormCaption(string caption) {
			if(CheckDesignTime())
				return;
			if(manager != null )
				manager.SetWaitFormCaption(caption);
		}
		public void EnableWaitFormDescription(bool enableDescription) {
			if(CheckDesignTime())
				return;
			if(manager != null )
				manager.SendCommand(WaitFormCommand.EnableDescription, enableDescription);
		}
		public void EnableCancelButton(bool enableCancelButton) {
			if(CheckDesignTime())
				return;
			if(manager != null )
				manager.SendCommand(WaitFormCommand.EnableCancelButton, enableCancelButton);
		}
		public void SetWaitFormDescription(string description) {
			if(CheckDesignTime())
				return;
			if(manager != null && manager.IsSplashFormVisible)
				manager.SetWaitFormDescription(description);
		}
		public void ShowWaitForm(bool useFadeIn, bool useFadeOut, bool useDelay) {
			if(CheckDesignTime())
				return;
			SplashScreenManager.ShowForm(ownerForm, waitFormType, useFadeIn, useFadeOut, false, SplashFormStartPosition.Default, Point.Empty, defaultDelay, allowInDT);
			manager = SplashScreenManager.Default;
		}
		public void ShowWaitForm(bool useFadeIn, bool useFadeOut) {
			ShowWaitForm(useFadeIn, useFadeOut, true);
		}
		public void CloseWaitForm(bool throwException, int delay, bool waitForClose) {
			if(CheckDesignTime())
				return;
			SplashScreenManager.CloseForm(throwException, delay, ownerForm, waitForClose, allowInDT);
			manager = null;
		}
		public void CloseWaitForm() {
			CloseWaitForm(false, 0, false);
		}
	}
}
