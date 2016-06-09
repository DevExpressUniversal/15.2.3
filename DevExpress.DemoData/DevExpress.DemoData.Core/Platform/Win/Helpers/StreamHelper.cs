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
namespace DevExpress.DemoData.Helpers {
	public static class StreamHelper {
		public const int BufferSize = 1024;
		public static MemoryStream CopyToMemoryStream(Stream stream) {
			byte[] buffer;
			if(stream.CanSeek) {
				buffer = new byte[stream.Length];
				int length = stream.Read(buffer, 0, buffer.Length);
				return new MemoryStream(buffer, 0, length);
			}
			MemoryStream memoryStream = new MemoryStream();
			buffer = new byte[BufferSize];
			using(BinaryReader sr = new BinaryReader(stream)) {
				while(true) {
					int count = sr.Read(buffer, 0, buffer.Length);
					if(count == 0) break;
					memoryStream.Write(buffer, 0, count);
				}
			}
			memoryStream.Flush();
			memoryStream.Seek(0, SeekOrigin.Begin);
			return memoryStream;
		}
		public static string ReadToEndNoClose(Stream stream) {
			using(MemoryStream copy = CopyToMemoryStream(stream)) {
				using(StreamReader sr = new StreamReader(copy)) {
					return sr.ReadToEnd();
				}
			}
		}
	}
}
