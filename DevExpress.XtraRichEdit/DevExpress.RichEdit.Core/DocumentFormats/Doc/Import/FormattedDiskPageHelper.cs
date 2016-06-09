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
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class FormattedDiskPageHelper  : IDisposable{
		#region Fields
		const int padByte = 1;
		const int styleIndexSize = 2;
		Dictionary<int, CHPXFormattedDiskPage> characterFKPs;
		Dictionary<int, PAPXFormattedDiskPage> paragraphsFKPs;
		VirtualStreamBinaryReader mainStreamReader;
		BinaryReader dataStreamReader;
		BinTable paragraphsBinTable;
		BinTable charactersBinTable;
		#endregion
		public FormattedDiskPageHelper(VirtualStreamBinaryReader mainStreamReader, BinaryReader dataStreamReader, BinTable paragraphsBinTable, BinTable charactersBinTable) {
			this.mainStreamReader = mainStreamReader.Clone();
			this.dataStreamReader = dataStreamReader;
			this.paragraphsBinTable = paragraphsBinTable;
			this.charactersBinTable = charactersBinTable;
			this.characterFKPs = new Dictionary<int, CHPXFormattedDiskPage>();
			this.paragraphsFKPs = new Dictionary<int, PAPXFormattedDiskPage>();
		}
		#region Properties
		protected internal VirtualStreamBinaryReader MainStreamReader { get { return this.mainStreamReader; } }
		protected internal BinaryReader DataStreamReader { get { return this.dataStreamReader; } }
		protected internal Dictionary<int, CHPXFormattedDiskPage> CharacterFormattedDiskPages { get { return this.characterFKPs; } }
		protected internal Dictionary<int, PAPXFormattedDiskPage> ParagraphFormattedDiskPages { get { return this.paragraphsFKPs; } }
		#endregion
		public DocPropertyContainer UpdateCharacterProperties(int fc, DocCommandFactory factory) {
			int fkpOffset = this.charactersBinTable.GetFKPOffset(fc);
			CHPXFormattedDiskPage chFkp;
			if (!CharacterFormattedDiskPages.TryGetValue(fkpOffset, out chFkp)) {
				chFkp = CHPXFormattedDiskPage.FromStream(MainStreamReader, fkpOffset);
				this.characterFKPs.Add(fkpOffset, chFkp);
			}
			int innerOffset = chFkp.GetInnerOffset(fc);
			byte[] grpprl;
			if (innerOffset == 0)
				grpprl = new byte[] { };
			else {
				int pxOffset = fkpOffset + (innerOffset << 1);
				MainStreamReader.BaseStream.Seek(pxOffset, SeekOrigin.Begin);
				byte grpprlSize = MainStreamReader.ReadByte();
				grpprl = MainStreamReader.ReadBytes(grpprlSize);
			}
			return DocCommandHelper.Traverse(grpprl, factory, dataStreamReader);
		}
		public void UpdateParagraphProperties(int fc, DocPropertyContainer propertyContainer) {
			int fkpOffset = this.paragraphsBinTable.GetFKPOffset(fc);
			PAPXFormattedDiskPage papFkp;
			if (!ParagraphFormattedDiskPages.TryGetValue(fkpOffset, out papFkp)) {
				papFkp = PAPXFormattedDiskPage.FromStream(MainStreamReader, fkpOffset);
				ParagraphFormattedDiskPages.Add(fkpOffset, papFkp);
			}
			int papInnerOffset = papFkp.GetInnerOffset(fc);
			if (papInnerOffset == 0)
				return;
			int pxOffset = fkpOffset + (papInnerOffset << 1);
			mainStreamReader.BaseStream.Seek(pxOffset, SeekOrigin.Begin); 
			int grpprlSize = MainStreamReader.ReadByte(); 
			if (grpprlSize == 0) 
				grpprlSize = (MainStreamReader.ReadByte() * 2) - styleIndexSize; 
			else
				grpprlSize = (grpprlSize * 2) - padByte - styleIndexSize;
			propertyContainer.Update(ChangeActionTypes.Paragraph);
			propertyContainer.ParagraphInfo.ParagraphStyleIndex = (short)MainStreamReader.ReadUInt16();
			if (grpprlSize <= 0 || MainStreamReader.BaseStream.Position + grpprlSize > MainStreamReader.BaseStream.Length)
				return;
			byte[] grpprl = MainStreamReader.ReadBytes(grpprlSize);
			DocCommandHelper.Traverse(grpprl, propertyContainer, dataStreamReader);
		}
		public List<TextRunBorder> GetTextRunBorders() {
			List<TextRunBorder> textRunBorders = new List<TextRunBorder>();
			AddTextRunBorders(this.charactersBinTable, TextRunStartReasons.TextRunMark, textRunBorders);
			AddTextRunBorders(this.paragraphsBinTable, TextRunStartReasons.ParagraphMark, textRunBorders);
			return textRunBorders;
		}
		void AddTextRunBorders(BinTable binTable, TextRunStartReasons reason, List<TextRunBorder> textRunBorders) {
			List<int> borderOffsets = binTable.GetBorders(MainStreamReader);
			int count = borderOffsets.Count;
			for (int i = 0; i < count; i++) {
				int currentOffset = borderOffsets[i];
				int index = Algorithms.BinarySearch(textRunBorders, new TextRunBorderComparable(currentOffset));
				if (index >= 0) {
					textRunBorders[index].Reason = textRunBorders[index].Reason | reason;
				}
				else {
					TextRunBorder textRunBorder = new TextRunBorder(currentOffset, reason);
					textRunBorders.Insert(~index, textRunBorder);
				}
			}
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~FormattedDiskPageHelper() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = MainStreamReader as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		#endregion
	}
}
