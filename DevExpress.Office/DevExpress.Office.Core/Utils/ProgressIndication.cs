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
using DevExpress.Utils;
using DevExpress.Services;
using DevExpress.Compatibility.System;
namespace DevExpress.Office.Utils {
	#region ProgressIndicationState
	public enum ProgressIndicationState {
		Unknown,
		Allowed,
		Forbidden
	}
	#endregion
	#region ProgressIndication
	public class ProgressIndication : IProgressIndicationService {
		#region Fields
		static readonly TimeSpan progressShowDelay = TimeSpan.FromMilliseconds(500);
		static readonly TimeSpan minIndicationInterval = TimeSpan.FromMilliseconds(50);
		const int progressLimit = 30;
		readonly IServiceProvider provider;
		DateTime indicationTime;
		string displayName;
		int minProgress;
		int progressRange = 1;
		int normalizedProgress;
		ProgressIndicationState indicationState;
		#endregion
		public ProgressIndication(IServiceProvider provider) {
			Guard.ArgumentNotNull(provider, "provider");
			this.provider = provider;
		}
		protected IServiceProvider Provider { get { return provider; } }
		public virtual void Begin(string displayName, int minProgress, int maxProgress, int currentProgress) {
			this.displayName = displayName;
			this.minProgress = minProgress;
			this.progressRange = Math.Max(1, maxProgress - minProgress);
			this.normalizedProgress = CalculateProgress(currentProgress);
			this.indicationTime = DateTime.Now;
			this.indicationState = ProgressIndicationState.Unknown;
		}
		public virtual void SetProgress(int currentProgress) {
			int progress = CalculateProgress(currentProgress);
			if (this.indicationState == ProgressIndicationState.Unknown) {
				DateTime now = DateTime.Now;
				if (now - indicationTime >= progressShowDelay) {
					if (progress <= progressLimit) {
						this.indicationState = ProgressIndicationState.Allowed;
						this.normalizedProgress = progress;
						BeginIndicationCore();
						IndicateProgressCore();
						this.indicationTime = now;
					}
					else
						this.indicationState = ProgressIndicationState.Forbidden;
				}
			}
			if (progress != normalizedProgress) {
				this.normalizedProgress = progress;
				if (this.indicationState == ProgressIndicationState.Allowed) {
					DateTime now = DateTime.Now;
					if (now - this.indicationTime >= minIndicationInterval || normalizedProgress == 100) {
						IndicateProgressCore();
						this.indicationTime = now;
					}
				}
			}
		}
		public virtual void End() {
			IProgressIndicationService service = GetService();
			if (service != null)
				service.End();
		}
		protected internal virtual void BeginIndicationCore() {
			IProgressIndicationService service = GetService();
			if (service != null)
				service.Begin(displayName, 0, 100, normalizedProgress);
		}
		protected internal virtual void IndicateProgressCore() {
			IProgressIndicationService service = GetService();
			if (service != null)
				service.SetProgress(normalizedProgress);
		}
		protected internal virtual IProgressIndicationService GetService() {
			return (IProgressIndicationService)provider.GetService(typeof(IProgressIndicationService));
		}
		protected internal virtual int CalculateProgress(int value) {
			return 100 * (value - minProgress) / progressRange;
		}
	}
	#endregion
	#region EmptyProgressIndication
	public class EmptyProgressIndication : ProgressIndication {
		public EmptyProgressIndication(IServiceProvider provider)
			: base(provider) {
		}
		public override void Begin(string displayName, int minProgress, int maxProgress, int currentProgress) {
		}
		public override void SetProgress(int currentProgress) {
		}
		public override void End() {
		}
	}
	#endregion
}
