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

using DevExpress.Pdf.Localization;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DevExpress.Pdf.Drawing {
	public class PdfAttachmentsViewerNode : IEquatable<PdfAttachmentsViewerNode>, IComparable<PdfAttachmentsViewerNode>, INotifyPropertyChanged {
		readonly PdfFileAttachment fileAttachment;
		readonly int imageIndex;
		readonly string size;
		readonly string hint;
		readonly byte[] image;
		public string FileName { get { return fileAttachment.FileName; } }
		public string Size { get { return size; } }
		public DateTimeOffset? ModificationDate { get { return fileAttachment.ModificationDate; } }
		public string Description { get { return fileAttachment.Description; } }
		public string Hint { get { return hint; } }
		public PdfFileAttachment FileAttachment { get { return fileAttachment; } }
		public int ImageIndex { get { return imageIndex; } }
		public byte[] Image { get { return image; } }
		public PdfAttachmentsViewerNode(byte[] image, int imageIndex, PdfFileAttachment fileAttachment) {
			this.fileAttachment = fileAttachment;
			this.imageIndex = imageIndex;
			this.image = image;
			this.size = GetSize(fileAttachment.Data == null ? 0 : fileAttachment.Data.Length);
			this.hint = CreateHint();
		}
		public bool Equals(PdfAttachmentsViewerNode other) {
			return other.fileAttachment == fileAttachment;
		}
		public int CompareTo(PdfAttachmentsViewerNode other) {
			return FileName.CompareTo(other.FileName);
		}
		string CreateHint() {
			string text = String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgAttachmentHintFileName), FileName);
			if (ModificationDate.HasValue)
				text += String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgAttachmentHintModificationDate), ModificationDate.Value);
			text += String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgAttachmentHintSize), Size);
			if (!string.IsNullOrEmpty(Description))
				text += String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgAttachmentHintDescription), Regex.Replace(Description, "(.{50}\\s)", "$1\n", RegexOptions.None));
			return text;
		}
		string GetSize(double length) {
			string[] size = new string[] { "b", "kb", "Mb", "Gb", "Tb" };
			int i = 0;
			while (length > 1024) {
				length /= 1024;
				i++;
			}
			return length.ToString("0.##", CultureInfo.InvariantCulture) + " " + size[i];
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { }
			remove { }
		}
	}
}
