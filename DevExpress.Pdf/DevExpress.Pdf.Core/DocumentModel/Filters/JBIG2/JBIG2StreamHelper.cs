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

using System.IO;
namespace DevExpress.Pdf.Native {
	public class JBIG2StreamHelper {
		readonly Stream stream;
		public bool Finish { get { return stream.Position == stream.Length; } }
		public JBIG2StreamHelper(byte[] data) {
			this.stream = new MemoryStream(data);
		}
		public JBIG2StreamHelper(Stream stream) {
			this.stream = stream;
		}
		public byte[] ReadBytes() {
			return ReadBytes((int)(stream.Length - stream.Position));
		}
		public byte[] ReadBytes(int count) {
			byte[] res = new byte[count];
			stream.Read(res, 0, count);
			return res;
		}
		public int ReadInt16() {
			return 256 * ReadByte() + ReadByte();
		}
		public int ReadInt32() {
			return ReadInt(4);
		}
		public int ReadInt(int count) {
			int result = ReadByte();
			if (count == 1)
				return result;
			result = 256 * result + ReadByte();
			if (count == 2)
				return result;
			result = 256 * result + ReadByte();
			result = 256 * result + ReadByte();
			return result;
		}
		public byte ReadByte() {
			return (byte)stream.ReadByte();
		}
		internal int[] ReadAdaptiveTemplate(int length) {
			int[] result = new int[length];
			for (int i = 0; i < length; i++) {
				result[i] = (int)(sbyte)ReadByte();
			}
			return result;
		}
		 public JBIG2StreamHelper ReadData(long dataLength) {
			if (dataLength < 0 || dataLength > 1000000)
				dataLength = stream.Length - stream.Position;
			byte[] data = new byte[dataLength];
			stream.Read(data, 0, (int)dataLength);
			return new JBIG2StreamHelper(data);
		}
	}
}
