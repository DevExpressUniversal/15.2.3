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
using DevExpress.Web.Internal;
using System.Web.UI;
using System.ComponentModel;
namespace DevExpress.Web {
	public class FileManagerFileAccessRule : FileManagerAccessRuleBase {
		public FileManagerFileAccessRule() 
			: this(string.Empty) { }
		public FileManagerFileAccessRule(string path) 
			: this(path, Rights.Default) { }
		public FileManagerFileAccessRule(string path, Rights browse)
			: base(path, browse) { }
		protected override string GetDefaultPathValue() {
			return "*";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileAccessRuleDownload"),
#endif
DefaultValue(Rights.Default), Category("Permissions"), AutoFormatDisable]
		public Rights Download
		{
			get { return (Rights)GetEnumProperty("Download", Rights.Default); }
			set
			{
				if (value != Download)
					Changed();
				SetEnumProperty("Download", Rights.Default, value);
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FileManagerFileAccessRule src = source as FileManagerFileAccessRule;
			if(src != null) {
				Download = src.Download;
			}
		}
	}
	public class FileManagerFolderAccessRule : FileManagerAccessRuleBase {
		public FileManagerFolderAccessRule() 
			: this(string.Empty) { }
		public FileManagerFolderAccessRule(string path) 
			: this(path, Rights.Default) { }
		public FileManagerFolderAccessRule(string path, Rights browse)
			: base(path, browse) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFolderAccessRuleEditContents"),
#endif
DefaultValue(Rights.Default), Category("Permissions"), AutoFormatDisable]
		public Rights EditContents
		{
			get { return (Rights)GetEnumProperty("EditContents", Rights.Default); }
			set
			{
				if (value != EditContents)
					Changed();
				SetEnumProperty("EditContents", Rights.Default, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFolderAccessRuleUpload"),
#endif
DefaultValue(Rights.Default), Category("Permissions"), AutoFormatDisable]
		public Rights Upload
		{
			get { return (Rights)GetEnumProperty("Upload", Rights.Default); }
			set
			{
				if (value != Upload)
					Changed();
				SetEnumProperty("Upload", Rights.Default, value);
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FileManagerFolderAccessRule src = source as FileManagerFolderAccessRule;
			if(src != null) {
				Upload = src.Upload;
				EditContents = src.EditContents;
			}
		}
	}
	public abstract class FileManagerAccessRuleBase : CollectionItem {
		public FileManagerAccessRuleBase(string path, Rights browse) {
			Path = path;
			Browse = browse;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerAccessRuleBaseRole"),
#endif
DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string Role
		{
			get { return GetStringProperty("Role", string.Empty); }
			set
			{
				if (value != Role)
					Changed();
				SetStringProperty("Role", string.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerAccessRuleBasePath"),
#endif
DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public virtual string Path
		{
			get { return GetStringProperty("Path", GetDefaultPathValue()); }
			set
			{
				if (value != Path)
					Changed();
				SetStringProperty("Path", GetDefaultPathValue(), value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerAccessRuleBaseEdit"),
#endif
DefaultValue(Rights.Default), Category("Permissions"), AutoFormatDisable]
		public Rights Edit
		{
			get { return (Rights)GetEnumProperty("Edit", Rights.Default); }
			set
			{
				if (value != Edit)
					Changed();
				SetEnumProperty("Edit", Rights.Default, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerAccessRuleBaseBrowse"),
#endif
DefaultValue(Rights.Default), Category("Permissions"), AutoFormatDisable]
		public Rights Browse
		{
			get { return (Rights)GetEnumProperty("Browse", Rights.Default); }
			set
			{
				if (value != Edit)
					Changed();
				SetEnumProperty("Browse", Rights.Default, value);
			}
		}
		protected void Changed() {
			if(Collection != null)
				Collection.Changed();
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FileManagerAccessRuleBase src = source as FileManagerAccessRuleBase;
			if(src != null) {
				Path = src.Path;
				Role = src.Role;
				Edit = src.Edit;
				Browse = src.Browse;
			}
		}
		protected virtual string GetDefaultPathValue() {
			return string.Empty;
		}
	}
	public class AccessRulesCollection : Collection<FileManagerAccessRuleBase> {
		RestrictedAccessFileSystemProvider provider;
		public AccessRulesCollection(ASPxFileManager owner)
			:base(owner) { }
		internal void RegisterFileSystemProvider(RestrictedAccessFileSystemProvider provider) {
			this.provider = provider;
		}
		internal RestrictedAccessFileSystemProvider Provider { get { return provider; } }
		protected override void OnChanged() {
			base.OnChanged();
			if(Provider != null)
				Provider.ResetAccessModel();
		}
	}
	public enum Rights {
		Allow,
		Deny,
		Default
	}
}
