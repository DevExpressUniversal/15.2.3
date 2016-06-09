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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export;
using System.IO;
namespace DevExpress.XtraPrinting.Export {
	public class EmailSender : EmailSenderBase {
		System.Windows.Forms.Control control;
		delegate void SendMailCoreDelegate(string[] files, EmailOptions options, IntPtr windowHandle);
		public EmailSender(System.Windows.Forms.Control control) {
			if(control == null)
				throw new ArgumentNullException("control");
			this.control = control;
		}
		protected override void SendCore(string[] files, EmailOptions options) {
			control.BeginInvoke(new SendMailCoreDelegate(SendEmail), new object[] { files, options, control.Handle });  
		}
		void SendEmail(string[] files, EmailOptions options, IntPtr windowHandle) {
			string path = Directory.GetCurrentDirectory();
			try {
				SendViaMAPI(files, options, control.Handle);
			} finally {
				Directory.SetCurrentDirectory(path);
			}
		}
	}
}
