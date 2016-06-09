#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.Linq;
using System.Text;
using DevExpress.Utils.Controls;
using System.ComponentModel;
namespace DevExpress.Compression {
	#region CompressionNotificationOptions (abstract class)
	public abstract class CompressionNotificationOptions : BaseOptions {
		protected CompressionNotificationOptions() {
			Reset();
		}
		#region Events
		protected internal event BaseOptionChangedEventHandler Changed { add { ChangedCore += value; } remove { ChangedCore -= value; } }
		#endregion
		public override void Reset() {
			BeginUpdate();
			try {
				ResetCore();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal abstract void ResetCore();
	}
	#endregion
	public enum AllowFileOverwriteMode { Custom, Allow, Forbidden }
	#region ZipArchiveOptionsBehavior
	public class ZipArchiveOptionsBehavior : CompressionNotificationOptions {
		AllowFileOverwriteMode allowFileOverwrite;
#if !SL
	[DevExpressDocsLocalizedDescription("ZipArchiveOptionsBehaviorAllowFileOverwrite")]
#endif
		public AllowFileOverwriteMode AllowFileOverwrite {
			get { return allowFileOverwrite; }
			set {
				if (allowFileOverwrite == value)
					return;
				AllowFileOverwriteMode oldValue = allowFileOverwrite;
				allowFileOverwrite = value;
				OnChanged("AllowFileOverwrite", oldValue, value);
			}
		}
		protected internal override void ResetCore() {
			this.allowFileOverwrite = AllowFileOverwriteMode.Allow;
		}
	}
	#endregion
}
