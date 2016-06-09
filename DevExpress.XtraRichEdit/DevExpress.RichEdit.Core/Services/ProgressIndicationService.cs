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
using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Services.Implementation {
	#region IRichEditProgressIndicationService
	public interface IRichEditProgressIndicationService : IProgressIndicationService {
		void SuspendProgressIndication();
		void ResumeProgressIndication();
	}
	#endregion
	#region RichEditProgressIndicationService
	public class RichEditProgressIndicationService : IRichEditProgressIndicationService {
		#region Fields
		readonly IServiceProvider provider;
		int suspendCount;
		#endregion
		public RichEditProgressIndicationService(IServiceProvider provider) {
			Guard.ArgumentNotNull(provider, "provider");
			this.provider = provider;
		}
		protected internal virtual bool IsProgressIndicationSuspended { get { return suspendCount != 0; } }
		public void Begin(string displayName, int minProgress, int maxProgress, int currentProgress) {
			if (!IsProgressIndicationSuspended) {
				IProgressIndicationService service = (IProgressIndicationService)provider.GetService(typeof(IProgressIndicationService));
				if (service != null)
					service.Begin(displayName, minProgress, maxProgress, currentProgress);
			}
		}
		public void SetProgress(int currentProgress) {
			if (!IsProgressIndicationSuspended) {
				IProgressIndicationService service = (IProgressIndicationService)provider.GetService(typeof(IProgressIndicationService));
				if (service != null)
					service.SetProgress(currentProgress);
			}
		}
		public void End() {
			if (!IsProgressIndicationSuspended) {
				IProgressIndicationService service = (IProgressIndicationService)provider.GetService(typeof(IProgressIndicationService));
				if (service != null)
					service.End();
			}
		}
		public void SuspendProgressIndication() {
			suspendCount++;
		}
		public void ResumeProgressIndication() {
			suspendCount--;
		}
	}
	#endregion
}
