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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using System.Collections;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
#if SL
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using DevExpress.Xpf.ComponentModel;
#else
using DevExpress.Data.Native;
#endif
namespace DevExpress.XtraPrinting {	
	[
	TypeConverter(typeof(DevExpress.XtraPrinting.PdfPasswordSecurityOptions.PdfPasswordSecurityOptionsTypeConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPasswordSecurityOptions"),
#if !SL && !DXPORTABLE
	Editor("DevExpress.XtraPrinting.Design.PdfPasswordSecurityOptionsEditor, " + AssemblyInfo.SRAssemblyPrinting, typeof(System.Drawing.Design.UITypeEditor)),
	Editor("DevExpress.Xpf.Printing.Native.PdfPasswordSecurityOptionsEditor, " + AssemblyInfo.SRAssemblyXpfPrinting, typeof(DevExpress.Xpf.Core.Native.IDXTypeEditor)),
#endif
	]
	public class PdfPasswordSecurityOptions : ICloneable, IXtraSupportShouldSerialize {
		public class PdfPasswordSecurityOptionsTypeConverter : LocalizableObjectConverter {
			public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
				return false;
			}
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
				PdfPasswordSecurityOptions options = value as PdfPasswordSecurityOptions;
				if(destinationType == typeof(string) && value != null) {
					if(string.IsNullOrEmpty(options.OpenPassword) && string.IsNullOrEmpty(options.PermissionsPassword))
						return PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions_None);
					string openPassword = string.IsNullOrEmpty(options.OpenPassword) ? string.Empty : PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions_DocumentOpenPassword);
					string permissions = string.IsNullOrEmpty(options.PermissionsPassword) ? string.Empty : PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions_Permissions);
					return StringUtils.Join("; ", openPassword, permissions);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		PdfPermissionsOptions permissionsOptions = new PdfPermissionsOptions();
		string permissionsPassword = string.Empty;
		string openPassword = string.Empty;
		public PdfPasswordSecurityOptions()
			: base() {
		}
		#region properties
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPasswordSecurityOptions.PermissionsPassword"),
		DefaultValue(""),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public string PermissionsPassword {
			get { return permissionsPassword; }
			set { permissionsPassword = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPasswordSecurityOptions.OpenPassword"),
		DefaultValue(""),
		Localizable(true),
		XtraSerializableProperty
		]
		public string OpenPassword {
			get { return openPassword; }
			set { openPassword = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPasswordSecurityOptions.PermissionsOptions"),
		Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public PdfPermissionsOptions PermissionsOptions { get { return permissionsOptions; } }
		#endregion
		public void Assign(PdfPasswordSecurityOptions options) {
			if(options == null)
				throw new ArgumentNullException("options");
			this.permissionsPassword = options.permissionsPassword;
			this.openPassword = options.openPassword;
			permissionsOptions.Assign(options.permissionsOptions);
		}
		public object Clone() {
			PdfPasswordSecurityOptions options = new PdfPasswordSecurityOptions();
			options.Assign(this);
			return options;
		}
		bool ShouldSerializePermissionsOptions() {
			return permissionsOptions.ShouldSerialize();
		}
		internal bool ShouldSerialize() {
			return permissionsPassword != string.Empty || openPassword != string.Empty || ShouldSerializePermissionsOptions();
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			switch(propertyName) {
				case "PermissionsOptions":
					return ShouldSerializePermissionsOptions();
			}
			return true;
		}
		public override bool Equals(object obj) {
			PdfPasswordSecurityOptions options = obj as PdfPasswordSecurityOptions;
			if(options == null)
				return false;
			return this.permissionsPassword == options.permissionsPassword &&
				this.openPassword == options.openPassword && this.permissionsOptions.Equals(options.PermissionsOptions);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPermissionsOptions"),
	]
	public class PdfPermissionsOptions : ICloneable {
		PrintingPermissions printingPermissions = PrintingPermissions.None;
		ChangingPermissions changingPermissions = ChangingPermissions.None;
		bool enableCopying;
		bool enableScreenReaders = true;
		public PdfPermissionsOptions()
			: base() {
		}
		#region properties
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPermissionsOptions.PrintingPermissions"),
		DefaultValue(PrintingPermissions.None),
		Localizable(true),
		XtraSerializableProperty
		]
		public PrintingPermissions PrintingPermissions {
			get { return printingPermissions; }
			set { printingPermissions = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPermissionsOptions.ChangingPermissions"),
		DefaultValue(ChangingPermissions.None),
		Localizable(true),
		XtraSerializableProperty
		]
		public ChangingPermissions ChangingPermissions {
			get { return changingPermissions; }
			set { changingPermissions = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPermissionsOptions.EnableCopying"),
		DefaultValue(false),
		Localizable(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool EnableCopying {
			get { return enableCopying; }
			set { enableCopying = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty,
		Obsolete("Use the EnableCopying property instead"),
		]
		public bool EnableCoping {
			get { return EnableCopying; }
			set { EnableCopying = value; }
		}
		bool ShouldSerializeEnableCoping() {
			return false;
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfPermissionsOptions.EnableScreenReaders"),
		DefaultValue(true),
		Localizable(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool EnableScreenReaders {
			get { return enableScreenReaders; }
			set { enableScreenReaders = value; }
		}
		#endregion
		public void Assign(PdfPermissionsOptions options) {
			if(options == null)
				throw new ArgumentNullException("options");
			this.printingPermissions = options.printingPermissions;
			this.changingPermissions = options.changingPermissions;
			this.enableCopying = options.enableCopying;
			this.enableScreenReaders = options.enableScreenReaders;
		}
		public object Clone() {
			PdfPermissionsOptions options = new PdfPermissionsOptions();
			options.Assign(this);
			return options;
		}
		internal bool ShouldSerialize() {
			return printingPermissions != PrintingPermissions.None || changingPermissions != ChangingPermissions.None || 
				enableCopying != false || enableScreenReaders !=true;
		}
		public override bool Equals(object obj) {
			PdfPermissionsOptions options = obj as PdfPermissionsOptions;
			if(options == null)
				return false;
			return this.printingPermissions == options.printingPermissions && this.changingPermissions == options.changingPermissions &&
				this.enableCopying == options.enableCopying && this.enableScreenReaders == options.enableScreenReaders;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
