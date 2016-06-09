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
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.Office.Internal {
#if !SL
	#region ImportSource<TFormat, TResult>
	public class ImportSource<TFormat, TResult> {
		#region Fields
		readonly string fileName;
		readonly IImporter<TFormat, TResult> importer;
		#endregion
		public ImportSource(string fileName, IImporter<TFormat, TResult> importer)
			: this(fileName, fileName, importer) {
		}
		public ImportSource(string storage, string fileName, IImporter<TFormat, TResult> importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
			this.fileName = fileName;
		}
		#region Properties
		public string FileName { get { return fileName; } }
		public IImporter<TFormat, TResult> Importer { get { return importer; } }
		public string Storage { get { return FileName; } }
		#endregion
		public virtual Stream GetStream() {
			return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
	}
	#endregion
#else
	#region ImportSource<TFormat, TResult>
	public class ImportSource<TFormat, TResult> {
	#region Fields
		readonly Stream storage;
		readonly string fileName;
		readonly IImporter<TFormat, TResult> importer;
		#endregion
		public ImportSource(Stream storage, string fileName, IImporter<TFormat, TResult> importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
			this.storage = storage;
			this.fileName = fileName;
		}
	#region Properties
		public string FileName { get { return fileName; } }
		public IImporter<TFormat, TResult> Importer { get { return importer; } }
		public Stream Storage { get { return storage; } }
		#endregion
		public virtual Stream GetStream() {
			return storage;
		}
	}
	#endregion
#endif
}
