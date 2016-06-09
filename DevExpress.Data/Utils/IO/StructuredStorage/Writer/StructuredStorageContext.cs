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
using System.IO;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Office.Utils;
namespace DevExpress.Utils.StructuredStorage.Internal.Writer {
	#region StructuredStorageContext
	[CLSCompliant(false)]
	public class StructuredStorageContext {
		#region Fields
		readonly Header header;
		readonly Fat fat;
		readonly MiniFat miniFat;
		OutputHandler tempOutputStream;
		readonly OutputHandler directoryStream;
		readonly InternalBitConverter internalBitConverter;
		readonly RootDirectoryEntry rootDirectoryEntry;
		UInt32 sidCounter = 0x0;
		#endregion
		public StructuredStorageContext() {
			this.tempOutputStream = new OutputHandler(new ChunkedMemoryStream());
			this.directoryStream = new OutputHandler(new ChunkedMemoryStream());
			this.header = new Header(this);
			this.internalBitConverter = InternalBitConverter.Create(true);
			this.fat = new Fat(this);
			this.miniFat = new MiniFat(this);
			this.rootDirectoryEntry = new RootDirectoryEntry(this);
		}
		#region Properties
		internal Header Header { get { return header; } }
		internal Fat Fat { get { return fat; } }
		internal MiniFat MiniFat { get { return miniFat; } }
		internal OutputHandler TempOutputStream { get { return tempOutputStream; } set { tempOutputStream = value; } }
		internal OutputHandler DirectoryStream { get { return directoryStream; } }
		internal InternalBitConverter InternalBitConverter { get { return internalBitConverter; } }
		public RootDirectoryEntry RootDirectoryEntry { get { return rootDirectoryEntry; } }
		#endregion
		internal UInt32 GetNewSid() {
			return ++sidCounter;
		}
	}
	#endregion
}
