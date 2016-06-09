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
using System.IO;
using System.IO.Packaging;
using System.Security;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Printing.Native;
namespace DevExpress.Xpf.Printing.Exports {
	class XpsExporter {
		public event EventHandler ProgressChanged;
		[SecuritySafeCritical]
		public void CreateDocument(DocumentPaginator paginator, Stream stream, XpsExportOptions options) {
			Guard.ArgumentNotNull(paginator, "paginator");
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			var indexesInPageRange = PageRangeParser.GetIndices(options.PageRange, paginator.PageCount);
			var paginatorWrapper = new PageRangeCustomPaginator(paginator, indexesInPageRange);
			using(Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite)) {
				var pack = string.Format("pack://document{0}.xps", Guid.NewGuid().ToString("N"));
				var uri = new Uri(pack);
				PackageStore.AddPackage(uri, package);
				SetPackageProperties(options, package);
				try {
					using(var document = new XpsDocument(package, (CompressionOption)options.Compression, pack)) {
						var writer = XpsDocument.CreateXpsDocumentWriter(document);
						writer.WritingProgressChanged += Writer_WritingProgressChanged;
						writer.Write(paginatorWrapper);
					}
				} finally {
					PackageStore.RemovePackage(uri);
				}
			}
		}
		static void SetPackageProperties(XpsExportOptions options, Package package) {
			if(options.DocumentOptions == null) {
				return;
			}
			var properties = package.PackageProperties;
			var documentOptions = options.DocumentOptions;
			properties.Creator = documentOptions.Creator;
			properties.Category = documentOptions.Category;
			properties.Title = documentOptions.Title;
			properties.Subject = documentOptions.Subject;
			properties.Keywords = documentOptions.Keywords;
			properties.Version = documentOptions.Version;
			properties.Description = documentOptions.Description;
		}
		void Writer_WritingProgressChanged(object sender, System.Windows.Documents.Serialization.WritingProgressChangedEventArgs e) {
#if DEBUGTEST
			var progress = string.Format("Writing progress: {0} - {1}, {2}", e.WritingLevel, e.Number, e.ProgressPercentage);
			System.Diagnostics.Debug.WriteLine(progress);
#endif
			if(ProgressChanged != null) {
				ProgressChanged(this, EventArgs.Empty);
			}
		}
	}
}
