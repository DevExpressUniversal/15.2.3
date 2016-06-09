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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class PdfSecurityOptionsUITypeEditor : Control {
		public readonly static DependencyProperty EditValueProperty;
		public readonly static DependencyProperty RequirePasswordProperty;
		public readonly static DependencyProperty OpenPasswordProperty;
		public readonly static DependencyProperty PermissionsPasswordProperty;
		public readonly static DependencyProperty PrintingPermissionsProperty;
		public readonly static DependencyProperty ChangingPermissionsProperty;
		public readonly static DependencyProperty EnableCopyingProperty;
		public readonly static DependencyProperty EnableScreenReadersProperty;
		static PdfSecurityOptionsUITypeEditor() {
			DependencyPropertyRegistrator<PdfSecurityOptionsUITypeEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.RequirePassword, out RequirePasswordProperty, false)
				.Register(d => d.OpenPassword, out OpenPasswordProperty, string.Empty, d => d.OnOpenPasswordChanged())
				.Register(d => d.PermissionsPassword, out PermissionsPasswordProperty, string.Empty, d => d.OnPermissionsPasswordChanged())
				.Register(d => d.PrintingPermissions, out PrintingPermissionsProperty, PrintingPermissions.None, d => d.OnPrintingPermissionsChanged())
				.Register(d => d.ChangingPermissions, out ChangingPermissionsProperty, ChangingPermissions.None, d => d.OnChangingPermissionsChanged())
				.Register(d => d.EnableCopying, out EnableCopyingProperty, false, d => d.OnEnableCopyingChanged())
				.Register(d => d.EnableScreenReaders, out EnableScreenReadersProperty, true, d => d.OnEnableScreenReadersChanged())
				.OverrideDefaultStyleKey()
			;
		}
		public bool RequirePassword {
			get { return (bool)GetValue(RequirePasswordProperty); }
			set { SetValue(RequirePasswordProperty, value); }
		}
		public string OpenPassword {
			get { return (string)GetValue(OpenPasswordProperty); }
			set { SetValue(OpenPasswordProperty, value); }
		}
		void OnOpenPasswordChanged() {
			EditValue.OpenPassword = OpenPassword;
		}
		public string PermissionsPassword {
			get { return (string)GetValue(PermissionsPasswordProperty); }
			set { SetValue(PermissionsPasswordProperty, value); }
		}
		void OnPermissionsPasswordChanged() {
			EditValue.PermissionsPassword = PermissionsPassword;
		}
		public PrintingPermissions PrintingPermissions {
			get { return (PrintingPermissions)GetValue(PrintingPermissionsProperty); }
			set { SetValue(PrintingPermissionsProperty, value); }
		}
		void OnPrintingPermissionsChanged() {
			EditValue.PermissionsOptions.PrintingPermissions = PrintingPermissions;
		}
		public ChangingPermissions ChangingPermissions {
			get { return (ChangingPermissions)GetValue(ChangingPermissionsProperty); }
			set { SetValue(ChangingPermissionsProperty, value); }
		}
		void OnChangingPermissionsChanged() {
			EditValue.PermissionsOptions.ChangingPermissions = ChangingPermissions;
		}
		public bool EnableCopying {
			get { return (bool)GetValue(EnableCopyingProperty); }
			set { SetValue(EnableCopyingProperty, value); }
		}
		void OnEnableCopyingChanged() {
			EditValue.PermissionsOptions.EnableCopying = EnableCopying;
		}
		public bool EnableScreenReaders {
			get { return (bool)GetValue(EnableScreenReadersProperty); }
			set { SetValue(EnableScreenReadersProperty, value); }
		}
		void OnEnableScreenReadersChanged() {
			EditValue.PermissionsOptions.EnableScreenReaders = EnableScreenReaders;
		}
		public PdfPasswordSecurityOptions EditValue {
			get { return (PdfPasswordSecurityOptions)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		void OnEditValueChanged() {
			RequirePassword = !string.IsNullOrEmpty(EditValue.With(x => x.OpenPassword));
			OpenPassword = EditValue.OpenPassword;
			PermissionsPassword = EditValue.PermissionsPassword;
			PrintingPermissions = EditValue.PermissionsOptions.PrintingPermissions;
			ChangingPermissions = EditValue.PermissionsOptions.ChangingPermissions;
			EnableCopying = EditValue.PermissionsOptions.EnableCopying;
			EnableScreenReaders = EditValue.PermissionsOptions.EnableScreenReaders;
		}
	}
	public class PrintingPermissionsDisplayNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var permissions = (PrintingPermissions)value;
			switch(permissions) {
				case PrintingPermissions.LowResolution: return "Low Resolution (150 dpi)";
				case PrintingPermissions.HighResolution: return "High Resolution";
				default: return "None";
			}
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class ChangingPermissionsDisplayNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var permissions = (ChangingPermissions)value;
			switch(permissions) {
				case ChangingPermissions.InsertingDeletingRotating: return "Inserting, deleting and rotating pages";
				case ChangingPermissions.FillingSigning: return "Filling in form fields and signing existing signature fields";
				case ChangingPermissions.CommentingFillingSigning: return "Commenting, filling in form fields, and signing existing signature fields";
				case ChangingPermissions.AnyExceptExtractingPages: return "Any except extracting pages";
				default: return "None";
			}
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
