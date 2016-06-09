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
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrinting.Native {
	public interface IPdfPasswordSecurityOptionsView {
		bool RequireOpenPassword { get; set; }
		string OpenPassword { get; set; }
		bool RestrictPermissions { get; set; }
		string PermissionsPassword { get; set; }
		PrintingPermissions PrintingPermissions { get; set; }
		ChangingPermissions ChangingPermissions { get; set; }
		bool EnableCopying { get; set; }
		bool EnableScreenReaders { get; set; }
		event EventHandler RequireOpenPasswordChanged;
		event EventHandler RestrictPermissionsChanged;
		event EventHandler<RepeatPasswordCompleteEventArgs> RepeatPasswordComplete;
		event EventHandler Submit;
		event EventHandler Dismiss;
		void EnableControl_OpenPassword(bool enable);
		void EnableControlGroup_Permissions(bool enable);
		void InitializePrintingPermissions(IEnumerable<KeyValuePair<PrintingPermissions, string>> availablePermissions);
		void InitializeChangingPermissions(IEnumerable<KeyValuePair<ChangingPermissions, string>> availablePermissions);
		void Close();
		void RepeatOpenPassword();
		void RepeatPermissionsPassword();
		void PasswordDoesNotMatch();
	}
}
