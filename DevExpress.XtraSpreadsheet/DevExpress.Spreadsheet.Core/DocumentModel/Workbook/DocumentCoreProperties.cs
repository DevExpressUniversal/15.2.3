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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ModelDocumentCoreProperties : SpreadsheetNotificationOptions {
		#region Fields
		string title;
		string subject;
		string creator;
		string keywords;
		string description;
		string lastModifiedBy;
		string category;
		DateTime created;
		DateTime modified;
		DateTime lastPrinted;
		#endregion
		#region Properties
		#region Title
		public string Title {
			get { return title; }
			set {
				if (title == value)
					return;
				string oldValue = Title;
				this.title = value;
				OnChanged("Title", oldValue, value);
			}
		}
		#endregion
		#region Subject
		public string Subject {
			get { return subject; }
			set {
				if (Subject == value)
					return;
				string oldValue = Subject;
				this.subject = value;
				OnChanged("Subject", oldValue, value);
			}
		}
		#endregion
		#region Creator
		public string Creator {
			get { return creator; }
			set {
				if (Creator == value)
					return;
				string oldValue = Creator;
				this.creator = value;
				OnChanged("Creator", oldValue, value);
			}
		}
		#endregion
		#region Keywords
		public string Keywords {
			get { return keywords; }
			set {
				if (Keywords == value)
					return;
				string oldValue = Keywords;
				this.keywords = value;
				OnChanged("Keywords", oldValue, value);
			}
		}
		#endregion
		#region Description
		public string Description {
			get { return description; }
			set {
				if (Description == value)
					return;
				string oldValue = Description;
				this.description = value;
				OnChanged("Description", oldValue, value);
			}
		}
		#endregion
		#region LastModifiedBy
		public string LastModifiedBy {
			get { return lastModifiedBy; }
			set {
				if (LastModifiedBy == value)
					return;
				string oldValue = LastModifiedBy;
				this.lastModifiedBy = value;
				OnChanged("LastModifiedBy", oldValue, value);
			}
		}
		#endregion
		#region Category
		public string Category {
			get { return category; }
			set {
				if (Category == value)
					return;
				string oldValue = Category;
				this.category = value;
				OnChanged("Category", oldValue, value);
			}
		}
		#endregion
		#region Created
		public DateTime Created {
			get { return created; }
			set {
				if (Created == value)
					return;
				DateTime oldValue = Created;
				this.created = value;
				OnChanged("Created", oldValue, value);
			}
		}
		#endregion
		#region Modified
		public DateTime Modified {
			get { return modified; }
			set {
				if (Modified == value)
					return;
				DateTime oldValue = Modified;
				this.modified = value;
				OnChanged("Modified", oldValue, value);
			}
		}
		#endregion
		#region LastPrinted
		public DateTime LastPrinted {
			get { return lastPrinted; }
			set {
				if (LastPrinted == value)
					return;
				DateTime oldValue = LastPrinted;
				this.lastPrinted = value;
				OnChanged("LastPrinted", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Title = String.Empty;
			this.Subject = String.Empty;
			this.Creator = String.Empty;
			this.Keywords = String.Empty;
			this.Description = String.Empty;
			this.LastModifiedBy = ObtainCurrentUserName();
			this.Category = String.Empty;
			this.Created = DateTime.Now;
			this.Modified = this.Created;
			this.LastPrinted = DateTime.MinValue;
		}
		protected internal void CopyFrom(ModelDocumentCoreProperties value) {
			this.Title = value.Title;
			this.Subject = value.Subject;
			this.Creator = value.Creator;
			this.Keywords = value.Keywords;
			this.Description = value.Description;
			this.LastModifiedBy = value.LastModifiedBy;
			this.Category = value.Category;
			this.Created = value.Created;
			this.Modified = value.Modified;
			this.LastPrinted = value.LastPrinted;
		}
		string ObtainCurrentUserName() {
			try {
				string userName = DocumentModel.GetUserName();
				if (!String.IsNullOrEmpty(userName))
					return userName;
#if !SL && !DOTNET
				return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
#else
				return String.Empty;
#endif
			}
			catch {
				return String.Empty;
			}
		}
		public void RegisterDocumentCreation(bool resetCreationInfo) {
			Reset();
			if (resetCreationInfo) {
				Creator = String.Empty;
				Created = DateTime.MinValue;
				Modified = DateTime.MinValue;
			}
			else
				Creator = ObtainCurrentUserName();
		}
		public void RegisterDocumentModification() {
			LastModifiedBy = ObtainCurrentUserName();
			Modified = DateTime.Now;
		}
	}
}
