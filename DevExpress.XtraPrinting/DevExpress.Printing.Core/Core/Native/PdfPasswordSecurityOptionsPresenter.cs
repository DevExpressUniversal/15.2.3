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
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	public class PdfPasswordSecurityOptionsPresenter {
		enum ValidationResult {
			Valid,
			Invalid,
			Cancelled
		}
		PdfPasswordSecurityOptions options;
		IPdfPasswordSecurityOptionsView view;
		EventHandler<RepeatPasswordCompleteEventArgs> onRepeatPasswordComplete;
		public PdfPasswordSecurityOptionsPresenter(PdfPasswordSecurityOptions options, IPdfPasswordSecurityOptionsView view) {
			if(options == null)
				throw new ArgumentNullException("options");
			if(view == null)
				throw new ArgumentNullException("view");
			this.options = options;
			this.view = view;
		}
		public void Initialize() {
			view.RequireOpenPassword = !string.IsNullOrEmpty(options.OpenPassword);
			view.OpenPassword = options.OpenPassword;
			view.RestrictPermissions = !string.IsNullOrEmpty(options.PermissionsPassword);
			view.PermissionsPassword = options.PermissionsPassword;
			InitializePrintingPermissions();
			view.PrintingPermissions = options.PermissionsOptions.PrintingPermissions;
			InitializeChangingPermissions();
			view.ChangingPermissions = options.PermissionsOptions.ChangingPermissions;
			view.EnableCopying = options.PermissionsOptions.EnableCopying;
			view.EnableScreenReaders = options.PermissionsOptions.EnableScreenReaders;
			view.Submit += new EventHandler(view_Submit);
			view.Dismiss += new EventHandler(view_Dismiss);
			view.RequireOpenPasswordChanged += new EventHandler(view_RequireOpenPasswordChanged);
			view.RestrictPermissionsChanged += new EventHandler(view_RestrictPermissionsChanged);
			RefreshView();
		}
		void InitializePrintingPermissions() {
			List<KeyValuePair<PrintingPermissions, string>> permissions = new List<KeyValuePair<PrintingPermissions, string>>();
			permissions.Add(
				new KeyValuePair<PrintingPermissions, string>(
					PrintingPermissions.None,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPrintingPermissions_None)));
			permissions.Add(
				new KeyValuePair<PrintingPermissions, string>(
					PrintingPermissions.LowResolution,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPrintingPermissions_LowResolution)));
			permissions.Add(
				new KeyValuePair<PrintingPermissions, string>(
					PrintingPermissions.HighResolution,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPrintingPermissions_HighResolution)));
			view.InitializePrintingPermissions(permissions);
		}
		void InitializeChangingPermissions() {
			List<KeyValuePair<ChangingPermissions, string>> permissions = new List<KeyValuePair<ChangingPermissions, string>>();
			permissions.Add(
				new KeyValuePair<ChangingPermissions, string>(
					ChangingPermissions.None,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_None)));
			permissions.Add(
				new KeyValuePair<ChangingPermissions, string>(
					ChangingPermissions.InsertingDeletingRotating,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_InsertingDeletingRotating)));
			permissions.Add(
				new KeyValuePair<ChangingPermissions, string>(
					ChangingPermissions.FillingSigning,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_FillingSigning)));
			permissions.Add(
				new KeyValuePair<ChangingPermissions, string>(
					ChangingPermissions.CommentingFillingSigning,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_CommentingFillingSigning)));
			permissions.Add(
				new KeyValuePair<ChangingPermissions, string>(
					ChangingPermissions.AnyExceptExtractingPages,
					PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfChangingPermissions_AnyExceptExtractingPages)));
			view.InitializeChangingPermissions(permissions);
		}
		void view_RestrictPermissionsChanged(object sender, EventArgs e) {
			RefreshView();
		}
		void view_RequireOpenPasswordChanged(object sender, EventArgs e) {
			RefreshView();
		}
		void view_Dismiss(object sender, EventArgs e) {
			view.Close();
		}
		void view_Submit(object sender, EventArgs e) {
			ValidatePassword(view.RequireOpenPassword, view.OpenPassword, view.RepeatOpenPassword, delegate() {
				ValidatePassword(view.RestrictPermissions, view.PermissionsPassword, view.RepeatPermissionsPassword, delegate() {
					options.OpenPassword = view.RequireOpenPassword && !string.IsNullOrEmpty(view.OpenPassword) ? view.OpenPassword : string.Empty;
					options.PermissionsPassword = view.RestrictPermissions && !string.IsNullOrEmpty(view.PermissionsPassword) ? view.PermissionsPassword : string.Empty;
					options.PermissionsOptions.PrintingPermissions = view.PrintingPermissions;
					options.PermissionsOptions.ChangingPermissions = view.ChangingPermissions;
					options.PermissionsOptions.EnableCopying = view.EnableCopying;
					options.PermissionsOptions.EnableScreenReaders = view.EnableScreenReaders;
					view.Close();
				});
			});
		}
		void ValidatePassword(bool usePassword, string password, Action0 repeatPassword, Action0 postAction) {
			if(!usePassword || string.IsNullOrEmpty(password)) {
				postAction();
				return;
			}
			onRepeatPasswordComplete = delegate(object o, RepeatPasswordCompleteEventArgs e) {
				RepeatPasswordComplete(password, e.RepeatedPassword, postAction);
			};
			view.RepeatPasswordComplete += onRepeatPasswordComplete;
			repeatPassword();
		}
		void RepeatPasswordComplete(string password, string repeatedPassword, Action0 postAction) {
			view.RepeatPasswordComplete -= onRepeatPasswordComplete;
			ValidationResult validationResult = repeatedPassword == null ? ValidationResult.Cancelled :
				string.Equals(password, repeatedPassword) ? ValidationResult.Valid : ValidationResult.Invalid;
			if(validationResult == ValidationResult.Invalid)
				view.PasswordDoesNotMatch();
			if(validationResult == ValidationResult.Valid)
				postAction();
		}
		void OnRepeatPasswordComplete(object sender, RepeatPasswordCompleteEventArgs e) { 
		}
		void RefreshView() {
			view.EnableControl_OpenPassword(view.RequireOpenPassword);
			view.EnableControlGroup_Permissions(view.RestrictPermissions);
		}
	}
}
