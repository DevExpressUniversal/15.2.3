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
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Utils.StructuredStorage.Internal.Writer;
namespace DevExpress.Utils.StructuredStorage.Writer {
	#region VirtualStream
	[CLSCompliant(false)]
	public class VirtualStream {
		#region Fields
		readonly AbstractFat fat;
		readonly Stream stream;
		readonly UInt16 sectorSize;
		readonly OutputHandler outputHander;
		UInt32 startSector = SectorType.Free;
		UInt32 sectorCount;
		#endregion
		public VirtualStream(Stream stream, AbstractFat fat, UInt16 sectorSize, OutputHandler outputHander) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(fat, "fat");
			this.stream = stream;
			this.fat = fat;
			this.sectorSize = sectorSize;
			this.outputHander = outputHander;
			this.sectorCount = (UInt32)Math.Ceiling((double)stream.Length / (double)sectorSize);
		}
		#region Properties
		public UInt32 StartSector { get { return startSector; } }
		public UInt64 Length { get { return (UInt64)stream.Length; } }
		public UInt32 SectorCount { get { return sectorCount; } }
		#endregion
		public void Write() {
			this.startSector = fat.WriteChain(SectorCount);
			byte[] buf = new byte[sectorSize];
			stream.Seek(0, SeekOrigin.Begin);
			while(true) {
				int bytesRead = stream.Read(buf, 0, sectorSize);
				outputHander.WriteSectors(buf, bytesRead, sectorSize, (byte)0x0);
				if(bytesRead != sectorSize) {
					break;
				}
			}
		}
	}
	#endregion
}
