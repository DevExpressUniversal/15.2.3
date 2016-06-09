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
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Forms {
	public partial class PdfDocumentPropertiesForm : XtraForm {
		static readonly XtraPdfViewerStringId[] units = new XtraPdfViewerStringId[] { 
			XtraPdfViewerStringId.UnitKiloBytes, 
			XtraPdfViewerStringId.UnitMegaBytes,
			XtraPdfViewerStringId.UnitGigaBytes,
			XtraPdfViewerStringId.UnitTeraBytes,
			XtraPdfViewerStringId.UnitPetaBytes,
			XtraPdfViewerStringId.UnitExaBytes,
			XtraPdfViewerStringId.UnitZettaBytes
		};
		static string GetDateTimeString(DateTimeOffset? dateTimeOffset) {
			if (!dateTimeOffset.HasValue)
				return String.Empty;
			DateTimeOffset dateTime = dateTimeOffset.Value;
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString();
		}
		PdfDocumentPropertiesForm() {		 
			InitializeComponent();
		}
		public PdfDocumentPropertiesForm(PdfDocumentProperties properties, int pageCount, SizeF pageSize) : this() {
			lblFileText.Text = properties.FileName;
			lblTitleText.Text = properties.Title;
			lblAuthorText.Text = properties.Author;
			lblSubjectText.Text = properties.Subject;
			lblKeywordsText.Text = properties.Keywords;
			lblCreatedText.Text = GetDateTimeString(properties.CreationDate);
			lblModifiedText.Text = GetDateTimeString(properties.ModificationDate);
			lblApplicationText.Text = properties.Application;
			lblProducerText.Text = properties.Producer;
			lblVersionText.Text = properties.Version;
			lblLocationText.Text = properties.FileLocation;
			double fileSize = properties.FileSize;
			string fileSizeString = fileSize.ToString("N0");
			if (fileSize < 1024)
				lblFileSizeText.Text = String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FileSizeInBytes), fileSizeString);
			else {
				int i = 0;
				while ((fileSize /= 1024) >= 1024) 
					i++;
				lblFileSizeText.Text = String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FileSize), fileSize, XtraPdfViewerLocalizer.GetString(units[i]), fileSizeString);
			}
			lblNumberOfPagesText.Text = pageCount.ToString();
			lblPageSizeText.Text = String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.PageSize), pageSize.Width, pageSize.Height);
		}
	}
}
