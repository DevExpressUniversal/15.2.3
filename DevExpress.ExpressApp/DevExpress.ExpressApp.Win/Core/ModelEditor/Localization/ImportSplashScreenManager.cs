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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ImportSplashScreenManager {
		bool progressStarted = false;
		public void Start(Form parentForm) {
			if(SplashScreenManager.Default == null && !progressStarted) {
				progressStarted = true;
				SplashScreenManager.ShowForm(parentForm, typeof(TranslationProgressForm), true, false, true, SplashFormStartPosition.Default, Point.Empty, 0, true);
				SplashScreenManager.Default.SendCommand(TranslationProgressForm.WaitFormCommand.SetCaption, "Apply localization");
			}
		}
		public bool Abort {
			get {
				return SplashScreenManager.Default == null && progressStarted;
			}
		}
		public void Stop() {
			if(SplashScreenManager.Default != null) {
				SplashScreenManager.Default.SendCommand(TranslationProgressForm.WaitFormCommand.ManualClose, null);
				SplashScreenManager.CloseForm(true, 0, null, false, true);
			}
		}
		int oldPercent = 0;
		public void SetProgress(decimal allItems, decimal remainingElements) {
			if(SplashScreenManager.Default != null) {
				decimal onePercent = allItems / 100;
				decimal percent = (allItems - remainingElements) / onePercent;
				if(oldPercent < (int)percent) {
					oldPercent = (int)percent;
					decimal newPercent = percent / 100;
					try {
						SplashScreenManager.Default.SendCommand(TranslationProgressForm.WaitFormCommand.SetProgress, newPercent);
					}
					catch {
					}
				}
			}
		}
	}
}
