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
using System.Runtime.InteropServices;
using DevExpress.Utils.Controls;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region RichEditNotificationOptions (abstract class)
	[ComVisible(true)]
	public abstract class RichEditNotificationOptions : BaseOptions {
		protected RichEditNotificationOptions() {
			CreateInnerOptions();
			Reset();
		}
		#region Events
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditNotificationOptionsChanged")]
#endif
		public event BaseOptionChangedEventHandler Changed { add { ChangedCore += value; } remove { ChangedCore -= value; } }
		#endregion
		protected virtual void CreateInnerOptions() {
		}
		public override void Reset() {
			BeginUpdate();
			try {
				ResetCore();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnChanged<T>(string name, T oldValue, T newValue) {
			OnChanged(new BaseOptionChangedEventArgs(name, oldValue, newValue));
		}
		protected internal abstract void ResetCore();
	}
	#endregion
}
