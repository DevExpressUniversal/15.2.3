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

using System.Collections.Specialized;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.XtraPrinting.Native {
	public static class BitVector32Helper {
		public static int CreateMask(BitVector32.Section prevSection) {
			BitVector32.Section tmpSection = BitVector32.CreateSection(1, prevSection);
			return tmpSection.Mask << (tmpSection.Offset & 0x1f);
		}
		public static BitVector32.Section CreateSection(short maxValue, int previousMask) {
			if(previousMask == 0)
				return BitVector32.CreateSection(maxValue);
			return BitVector32.CreateSection(maxValue, CreateTempSection(previousMask));
		}
		static BitVector32.Section CreateTempSection(int previousMask) {
			if((previousMask & 0x7FFF) != 0)
				return BitVector32.CreateSection((short)(previousMask & 0x7FFF));
			BitVector32.Section tmpSection = BitVector32.CreateSection(0x7FFF);
			previousMask >>= 15;
			if((previousMask & 0x7FFF) != 0)
				return BitVector32.CreateSection((short)(previousMask & 0x7FFF), tmpSection);
			tmpSection = BitVector32.CreateSection(0x7FFF, tmpSection);
			previousMask >>= 15;
			return BitVector32.CreateSection((short)previousMask, tmpSection);
		}
	}
	public class PageSizeAccuracyComaprer : FloatsComparer {
		public static readonly FloatsComparer Instance = new PageSizeAccuracyComaprer(1.5);
		protected PageSizeAccuracyComaprer(double epsilon)
			: base(epsilon) {
		}
	}
}
