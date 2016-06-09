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

using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public abstract class PagesExporterBase : IPagesExporter {
		static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly ISerializationService serializationService;
		protected PagesExporterBase(ISerializationService serializationService) {
			Guard.ArgumentNotNull(serializationService, "serializationService");
			this.serializationService = serializationService;
		}
		#region IPagesExporter Members
		public abstract bool ExclusivelyDocumentUsing { get; }
		public string Export(Document document, int pageIndex) {
			if(pageIndex < 0 || pageIndex >= document.Pages.Count) {
				Logger.Error("Page index {0} is not exists in current document", pageIndex);
				return null;
			}
			return ExportCore(document, pageIndex);
		}
		public byte[] Export(Document document, int[] pageIndexes) {
			var result = new string[pageIndexes.Length];
			for(int i = 0; i < result.Length; i++) {
				result[i] = Export(document, pageIndexes[i]);
			}
			return serializationService.Serialize(result);
		}
		#endregion
		protected abstract string ExportCore(Document document, int pageIndex);
	}
}
