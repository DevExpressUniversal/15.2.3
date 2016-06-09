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

using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Printing;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Native {
	public class DefaultPageSettingsProvider {
		PageSettings pageSettings;
		Thread obtainDefaultPageSettingsThread;
		public DefaultPageSettingsProvider() {
			ExecuteBackgroundThread();
			Application.ApplicationExit += new EventHandler(OnApplicationExit);
		}
		protected internal PageSettings PageSettings { get { return pageSettings; } }
		protected internal Thread ObtainDefaultPageSettingsThread { get { return obtainDefaultPageSettingsThread; } }
		public virtual PageSettings GetPageSettings() {
			if (obtainDefaultPageSettingsThread != null) {
				obtainDefaultPageSettingsThread.Join();
				obtainDefaultPageSettingsThread = null;
				Application.ApplicationExit -= new EventHandler(OnApplicationExit);
			}
			return pageSettings;
		}
		protected internal virtual void ExecuteBackgroundThread() {
			obtainDefaultPageSettingsThread = new Thread(new ThreadStart(ObtainDefaultPageSettings));
			obtainDefaultPageSettingsThread.Priority = ThreadPriority.Lowest;
			obtainDefaultPageSettingsThread.Start();
		}
		protected virtual void ObtainDefaultPageSettings() {
			try {
				pageSettings = SchedulerPrintStyle.ClonePageSettings(GetPSPageSettings());
			} catch {
			}
			if (pageSettings == null)
				pageSettings = new PageSettings();
		}
		protected internal virtual void OnApplicationExit(object sender, EventArgs args) {
			if (obtainDefaultPageSettingsThread != null) {
				if (!obtainDefaultPageSettingsThread.Join(TimeSpan.Zero)) {
					obtainDefaultPageSettingsThread.Abort();
					obtainDefaultPageSettingsThread = null;
				}
			}
		}
		protected internal virtual PageSettings GetPSPageSettings() {
			return ComponentPrinterBase.GetDefaultPageSettings();
		}
	}
}
