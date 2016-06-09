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
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class PieceDescriptor {
		public const int PieceDescriptorSize = 8;
		public static PieceDescriptor FromByteArray(byte[] pieceDescriptor) {
			PieceDescriptor result = new PieceDescriptor();
			result.Read(pieceDescriptor);
			return result;
		}
		protected internal static PieceDescriptor FromFileOffset(int fc) {
			PieceDescriptor result = new PieceDescriptor();
			result.fc = fc;
			return result;
		}
		#region Fields
		Int16 descriptorStart;
		Int32 fc;
		Int16 descriptorEnd;
		#endregion
		#region Properties
		protected internal Int16 DescriptorStart { get { return this.descriptorStart; } }
		public Int32 FC { get { return this.fc; } } 
		protected internal Int16 DescriptorEnd { get { return this.descriptorEnd; } }
		#endregion
		protected void Read(byte[] pieceDescriptor) {
			this.descriptorStart = BitConverter.ToInt16(pieceDescriptor, 0);
			this.fc = BitConverter.ToInt32(pieceDescriptor, 2);
			this.descriptorEnd = BitConverter.ToInt16(pieceDescriptor, 6);
		}
		public byte[] ToByteArray() {
			byte[] buffer = new byte[8];
			Array.Copy(BitConverter.GetBytes((ushort)DescriptorStart), 0, buffer, 0, 2);
			Array.Copy(BitConverter.GetBytes((uint)FC), 0, buffer, 2, 4);
			Array.Copy(BitConverter.GetBytes((ushort)DescriptorEnd), 0, buffer, 6, 2); 
			return buffer;
		}
		public Encoding GetEncoding() {
			return ((FC & 0x40000000) != 0) ? DXEncoding.GetEncoding(1252) : Encoding.Unicode;
		}
		public int GetOffset() {
			if ((FC & 0x40000000) != 0)
				return (FC & ~(0x40000000)) / 2;
			return FC;
		}
	}
}
