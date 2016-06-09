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
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class EmfPlusStringFormat {
		uint formatFlags;
		public StringFormatFlags FormatFlags { get { return (StringFormatFlags)(formatFlags & 0x7FFFF); } }
		public int Language;
		public StringAlignment Alignment;
		public StringAlignment LineAlignment;
		public int DigitSubstitution;
		public int DigitLanguage;
		public float FirstTabOffset;
		public System.Drawing.Text.HotkeyPrefix HotkeyPrefix;
		float LeadingMargin;
		float TrailingMargin;
		float Tracking;
		public StringTrimming Trimming;
		public int TabStopCount;
		public int RangeCount;
		public float[] TabStops;
		public CharacterRange[] CharRange;
		public bool IsGenericDefault {
			get { return LeadingMargin == 1f / 6f && TrailingMargin == 1f / 6f && Tracking == 1.03f; } 
		}
		public bool IsGenericTypographic {
			get { return LeadingMargin == 0f && TrailingMargin == 0f && Tracking == 1.0f; }
		}
		public EmfPlusStringFormat(MetaReader reader) {
			new EmfPlusGraphicsVersion(reader);
			formatFlags		= reader.ReadUInt32();
			Language		   = reader.ReadInt32();
			Alignment		  = (StringAlignment)reader.ReadUInt32(); 
			LineAlignment	  = (StringAlignment)reader.ReadUInt32();
			DigitSubstitution  = reader.ReadInt32();
			DigitLanguage	  = reader.ReadInt32();
			FirstTabOffset	 = reader.ReadSingle();
			HotkeyPrefix	   = (System.Drawing.Text.HotkeyPrefix)reader.ReadUInt32();
			LeadingMargin	  = reader.ReadSingle();
			TrailingMargin	 = reader.ReadSingle();
			Tracking		   = reader.ReadSingle();
			Trimming		   = (StringTrimming)reader.ReadUInt32();
			TabStopCount	   = reader.ReadInt32();
			RangeCount		 = reader.ReadInt32();
			TabStops		   = reader.ReadSingleArray(TabStopCount);
			CharRange		  = ReadCharacterRanges(reader, RangeCount);
		}
		static CharacterRange[] ReadCharacterRanges(MetaReader reader, int rangeCount) {
			CharacterRange[] result = new CharacterRange[rangeCount];
			for(int i = 0; i < rangeCount; i++)
				result[i] = new CharacterRange(reader.ReadInt32(), reader.ReadInt32());
			return result;
		}
	}
}
