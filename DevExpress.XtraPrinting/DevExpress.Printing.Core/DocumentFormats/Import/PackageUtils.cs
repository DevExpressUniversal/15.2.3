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
namespace DevExpress.Office.Utils {
	#region PackageFile
	public class PackageFile {
		string fileName;
		Stream stream;
		int streamLength;
		MemoryStream seekableStream; 
		public PackageFile(string fileName, Stream stream, int streamLength) {
			Guard.ArgumentNotNull(fileName, "fileName");
			Guard.ArgumentNotNull(stream, "stream");
			this.fileName = fileName;
			this.stream = stream;
			this.streamLength = streamLength;
		}
		public string FileName { get { return fileName; } }
		public Stream Stream { get { return stream; } }
		public int StreamLength { get { return streamLength; } set { streamLength = value; } }
		public MemoryStream SeekableStream {
			get {
				if (seekableStream == null) {
					byte[] bytes = new byte[StreamLength];
					Stream.Read(bytes, 0, bytes.Length);
					seekableStream = new MemoryStream(bytes);
				}
				return seekableStream;
			}
		}
	}
	#endregion
	#region PackageFileCollection
	public class PackageFileCollection : List<PackageFile> {
	}
	#endregion
	#region PackageFileStream
	public class PackageFileStreams : Dictionary<string, Stream> {
	}
	#endregion
}
