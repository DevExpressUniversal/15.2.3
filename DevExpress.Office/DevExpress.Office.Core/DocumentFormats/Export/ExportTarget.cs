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
using System.IO;
using DevExpress.Utils;
namespace DevExpress.Office.Internal {
#if !SL
	#region ExportTarget<TFormat, TResult>
	public class ExportTarget<TFormat, TResult> {
		#region Fields
		readonly string fileName;
		readonly IExporter<TFormat, TResult> exporter;
		#endregion
		public ExportTarget(string fileName, IExporter<TFormat, TResult> exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
			this.fileName = fileName;
		}
		#region Properties
		public string FileName { get { return fileName; } }
		public IExporter<TFormat, TResult> Exporter { get { return exporter; } }
		public string Storage { get { return FileName; } }
		#endregion
		public virtual Stream GetStream() {
			return new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
		}
	}
	#endregion
#else
	#region ExportTarget<TFormat, TResult>
	public class ExportTarget<TFormat, TResult> {
	#region Fields
		readonly Stream storage;
		readonly IExporter<TFormat, TResult> exporter;
		#endregion
		public ExportTarget(Stream storage, IExporter<TFormat, TResult> exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
			this.storage = storage;
		}
	#region Properties
		public Stream Storage { get { return storage; } }
		public IExporter<TFormat, TResult> Exporter { get { return exporter; } }
		#endregion
		public virtual Stream GetStream() {
			return storage;
		}
	}
	#endregion
#endif
}
