#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf {
	public class PdfFileAttachment {
		readonly PdfFileSpecification fileSpecification;
		internal PdfFileSpecification FileSpecification { get { return fileSpecification; } }
		public DateTimeOffset? CreationDate {
			get { return fileSpecification.CreationDate; }
			set { fileSpecification.CreationDate = value; }
		}
		public DateTimeOffset? ModificationDate {
			get { return fileSpecification.ModificationDate; }
			set { fileSpecification.ModificationDate = value; }
		}
		public string MimeType {
			get { return fileSpecification.MimeType; }
			set { fileSpecification.MimeType = value; }
		}
		public byte[] Data {
			get { return fileSpecification.FileData; }
			set { fileSpecification.FileData = value; }
		}
		public string FileName {
			get { return fileSpecification.FileName; }
			set { fileSpecification.FileName = value; }
		}
		public PdfAssociatedFileRelationship Relationship {
			get { return fileSpecification.Relationship; }
			set { fileSpecification.Relationship = value; }
		}
		public string Description {
			get { return fileSpecification.Description; }
			set { fileSpecification.Description = value; }
		}
		public PdfFileAttachment() : this(new PdfFileSpecification(String.Empty)) {
		}
		internal PdfFileAttachment(PdfFileSpecification fileSpecification) {
			this.fileSpecification = fileSpecification;
			fileSpecification.Attachment = this;
		}
	}
}
