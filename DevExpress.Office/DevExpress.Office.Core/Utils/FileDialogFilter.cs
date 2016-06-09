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
using DevExpress.Office.Localization;
namespace DevExpress.Office.Internal {
	#region FileExtensionCollection
	public class FileExtensionCollection : List<string> {
	}
	#endregion
	#region FileDialogFilter
	public class FileDialogFilter {
		#region Fields
		internal static readonly FileDialogFilter AllFiles = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_AllFiles), "*");
		internal static readonly FileDialogFilter Empty = new FileDialogFilter(String.Empty, new String[] { });
		string description = String.Empty;
		FileExtensionCollection extensions = new FileExtensionCollection();
		#endregion
		public FileDialogFilter() {
		}
		public FileDialogFilter(string description, string extension)
			: this(description, new string[] { extension }) {
		}
		public FileDialogFilter(string description, string[] extensions) {
			this.description = description;
			this.extensions.AddRange(extensions);
		}
		#region Properties
		public string Description { get { return description; } set { description = value; } }
		public FileExtensionCollection Extensions { get { return extensions; } }
		#endregion
		public override string ToString() {
			if (extensions.Count <= 0)
				return AllFiles.ToString();
			return CreateFilterString();
		}
		protected internal virtual string CreateFilterString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(Description);
			sb.Append(" (");
			AppendExtensions(sb);
			sb.Append(")|");
			AppendExtensions(sb);
			return sb.ToString();
		}
		protected internal virtual void AppendExtensions(StringBuilder sb) {
			AppendExtension(sb, extensions[0]);
			int count = extensions.Count;
			for (int i = 1; i < count; i++) {
				sb.Append("; ");
				AppendExtension(sb, extensions[i]);
			}
		}
		protected internal virtual void AppendExtension(StringBuilder sb, string extension) {
			sb.Append("*.");
			sb.Append(extension);
		}
	}
	#endregion
	#region FileDialogFilterCollection
	public class FileDialogFilterCollection : List<FileDialogFilter> {
		public string CreateFilterString() {
			int count = this.Count;
			if (count <= 0)
				return String.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(this[0].ToString());
			for (int i = 1; i < count; i++) {
				sb.Append('|');
				sb.Append(this[i].ToString());
			}
			return sb.ToString();
		}
	}
	#endregion
}
