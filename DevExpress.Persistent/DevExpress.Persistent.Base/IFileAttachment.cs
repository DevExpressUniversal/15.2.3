#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Persistent.Base {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public class FileAttachmentAttribute : Attribute {
		string fileDataPropertyName;
		public FileAttachmentAttribute(string fileDataPropertyName) {
			this.fileDataPropertyName = fileDataPropertyName;
		}
		public string FileDataPropertyName {
			get {
				return fileDataPropertyName;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class FileTypeFilterAttribute : Attribute {
		private int index;
		private string filterID;
		private string filterCaption;
		private List<string> extensions;
		public FileTypeFilterAttribute(string filterID, params string[] extensions) : this(filterID, CaptionHelper.ConvertCompoundName(filterID), -1, extensions) { }
		public FileTypeFilterAttribute(string filterID, int index, params string[] extensions) : this(filterID, CaptionHelper.ConvertCompoundName(filterID), index, extensions) { }
		public FileTypeFilterAttribute(string filterID, string filterCaption, params string[] extensions) : this(filterID, filterCaption, -1, extensions) { }
		public FileTypeFilterAttribute(string filterID, string filterCaption, int index, params string[] extensions) {
			this.index = index;
			this.filterID = filterID;
			this.filterCaption = filterCaption;
			this.extensions = new List<string>(extensions);
		}
		public List<string> GetExtensions() {
			return extensions;
		}
		public int Index {
			get { return index; }
		}
		public string FilterID {
			get { return filterID; }
		}
		public string FilterCaption {
			get { return filterCaption; }
		}
		public string FileTypesFilter {
			get {
				string extensionsList = string.Join("; ", extensions.ToArray());
				return (filterCaption + " (" + extensionsList + ")|" + extensionsList);
			}
		}
	}
}
