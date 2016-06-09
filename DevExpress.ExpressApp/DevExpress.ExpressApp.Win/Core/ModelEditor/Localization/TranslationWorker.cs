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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraSplashScreen;
using System.Drawing;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class TranslateWorker {
		public Dictionary<string, string> Start(ITranslatorProvider translator, Form parentForm, string sourceLanguageCode, string destinationLanguageCode, string[] texts) {
			string[] translation = null;
			translator.TranslationProgressChanged += new EventHandler<TranslationProgressEventArgs>(translator_TranslationProgressChanged);
			Exception translationException = null;
			try {
				SplashScreenManager.ShowForm(parentForm, typeof(TranslationProgressForm), true, false, true, SplashFormStartPosition.Default, Point.Empty, 0, true);
				translation = translator.Translate(texts, sourceLanguageCode, destinationLanguageCode);
			}
			catch (System.Net.WebException e) {
				translationException = e;
			}
			finally {
				translator.TranslationProgressChanged -= translator_TranslationProgressChanged;
				if(SplashScreenManager.Default != null) {
					try {
						SplashScreenManager.Default.SendCommand(TranslationProgressForm.WaitFormCommand.ManualClose, null);
					}
					catch {
					}
					SplashScreenManager.CloseForm(true, 0, null, false, true);
				}
			}
			if(translationException != null) {
				Messaging.GetMessaging(null).Show("Network Error", translationException.Message);
			}
			Dictionary<string, string> result = null;
			if (translation != null && translation.Length == texts.Length) {
				result = new Dictionary<string, string>();
				for (int i = 0; i < texts.Length; i++) {
					result.Add(texts[i], translation[i]);
				}
			}
			return result;
		}
		private void translator_TranslationProgressChanged(object sender, TranslationProgressEventArgs e) {
			if(SplashScreenManager.Default != null) {
				try {
					SplashScreenManager.Default.SendCommand(TranslationProgressForm.WaitFormCommand.SetProgress, e.Progress);
				}
				catch {
				}
			}
			else {
				e.Cancel = true;
			}
		}
	}
}
