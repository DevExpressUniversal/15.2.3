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
namespace DevExpress.Utils.StructuredStorage.Internal {
	#region AbstractHeader (abstract class)
	[CLSCompliant(false)]
	public abstract class AbstractHeader {
		#region Fields
		public const UInt64 MAGIC_NUMBER = 0xE11AB1A1E011CFD0;
		AbstractIOHandler ioHandler;
		UInt16 sectorShift;
		UInt16 sectorSize;
		UInt16 miniSectorShift;
		UInt16 miniSectorSize;
		UInt32 noSectorsInDirectoryChain4KB;
		UInt32 noSectorsInFatChain;
		UInt32 directoryStartSector;
		UInt32 miniSectorCutoff;
		UInt32 miniFatStartSector;
		UInt32 noSectorsInMiniFatChain;
		UInt32 diFatStartSector;
		UInt32 noSectorsInDiFatChain;
		#endregion
		protected AbstractHeader(AbstractIOHandler ioHandler) {
			Guard.ArgumentNotNull(ioHandler, "ioHandler");
			this.ioHandler = ioHandler;
		}
		#region Properties
		public AbstractIOHandler IoHandler { get { return ioHandler; } }
		public UInt16 SectorShift {
			get { return sectorShift; }
			set {
				if (value != 9 && value != 12)
					ThrowUnsupportedSizeException("SectorShift");
				sectorShift = value;
				sectorSize = (UInt16)(1 << sectorShift);
			}
		}
		public UInt16 SectorSize { get { return sectorSize; } }
		public UInt16 MiniSectorShift {
			get { return miniSectorShift; }
			set {
				if (value != 6)
					ThrowUnsupportedSizeException("MiniSectorShift");
				miniSectorShift = value;
				miniSectorSize = (UInt16)(1 << miniSectorShift);
			}
		}
		public UInt16 MiniSectorSize { get { return miniSectorSize; } }
		public UInt32 NoSectorsInDirectoryChain4KB { 
			get { return noSectorsInDirectoryChain4KB; }
			set {
				if (SectorSize == 512 && value != 0)
					ThrowArgumentException("NoSectorsInDirectoryChain4KB", value);
				noSectorsInDirectoryChain4KB = value;
			}
		}
		public UInt32 NoSectorsInFatChain { 
			get { return noSectorsInFatChain; }
			set {
				if (value > ioHandler.IOStreamSize / SectorSize)
					ThrowInvalidHeaderValueException("NoSectorsInFatChain");
				noSectorsInFatChain = value;
			}
		}
		public UInt32 DirectoryStartSector { 
			get { return directoryStartSector; }
			set {
				if (value > ioHandler.IOStreamSize / SectorSize && value != SectorType.EndOfChain)
					ThrowInvalidHeaderValueException("DirectoryStartSector");
				directoryStartSector = value;
			}
		}
		public UInt32 MiniSectorCutoff { 
			get { return miniSectorCutoff; }
			set {
				if (value != 0x1000)
					ThrowUnsupportedSizeException("MiniSectorCutoff");
				miniSectorCutoff = value;
			}
		}
		public UInt32 MiniFatStartSector { 
			get { return miniFatStartSector; }
			set {
				if (value > ioHandler.IOStreamSize / SectorSize && value != SectorType.EndOfChain)
					ThrowInvalidHeaderValueException("MiniFatStartSector");
				miniFatStartSector = value;
			}
		}
		public UInt32 NoSectorsInMiniFatChain { 
			get { return noSectorsInMiniFatChain; }
			set {
				if (value > ioHandler.IOStreamSize / SectorSize)
					ThrowInvalidHeaderValueException("NoSectorsInMiniFatChain");
				noSectorsInMiniFatChain = value;
			}
		}
		public UInt32 DiFatStartSector { 
			get { return diFatStartSector; }
			set {
				if (value > ioHandler.IOStreamSize / SectorSize && value != SectorType.EndOfChain)
					ThrowInvalidHeaderValueException("DiFatStartSector");
				diFatStartSector = value;
			}
		}
		public UInt32 NoSectorsInDiFatChain { 
			get { return noSectorsInDiFatChain; }
			set {
				if (value > ioHandler.IOStreamSize / SectorSize)
					ThrowInvalidHeaderValueException("NoSectorsInDiFatChain");
				noSectorsInDiFatChain = value;
			}
		}
		#endregion
		public void ThrowUnsupportedSizeException(string name) {
			throw new Exception("The size of " + name + " is not supported.");
		}
		public void ThrowInvalidHeaderValueException(string name) {
			throw new Exception("The value for '" + name + "' in the header is invalid.");
		}
		protected virtual void ThrowArgumentException(string propName, object val) {
			string valueStr =
				Object.ReferenceEquals(val, string.Empty) ? "String.Empty" :
				Object.ReferenceEquals(val, null) ? "null" :
				val.ToString();
			string s = String.Format("'{0}' is not a valid value for '{1}'", valueStr, propName);
			throw new ArgumentException(s);
		}
	}
	#endregion
}
