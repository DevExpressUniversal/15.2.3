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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class BreakDescriptorTable {
		readonly List<int> characterPositions;
		readonly List<BreakDescriptor> breakDescriptors;
		short currentIndex;
		public static BreakDescriptorTable FromStream(BinaryReader reader, int offset, int size) {
			BreakDescriptorTable result = new BreakDescriptorTable();
			result.Read(reader, offset, size);
			return result;
		}
		public BreakDescriptorTable() {
			this.characterPositions = new List<int>();
			this.breakDescriptors = new List<BreakDescriptor>();
		}
		public List<int> CharacterPositions { get { return characterPositions; } }
		public List<BreakDescriptor> BreakDescriptors { get { return breakDescriptors; } }
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0 || offset + size > reader.BaseStream.Length) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + BreakDescriptor.Size);
			for (int i = 0; i <= count; i++)
				characterPositions.Add(reader.ReadInt32());
			for (int i = 0; i < count; i++)
				breakDescriptors.Add(BreakDescriptor.FromStream(reader));
		}
		public void Write(BinaryWriter writer) {
#if DEBUGTEST
			if (characterPositions.Count > 0)
				Debug.Assert(characterPositions.Count == breakDescriptors.Count + 1);
#endif
			int count = characterPositions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(characterPositions[i]);
			count = breakDescriptors.Count;
			for (int i = 0; i < count; i++)
				breakDescriptors[i].Write(writer);
		}
		public void AddEntry(int characterPosition) {
			characterPositions.Add(characterPosition);
			breakDescriptors.Add(new BreakDescriptor() { Index = currentIndex });
			currentIndex++;
		}
		public void Finish(int characterPosition) {
			characterPositions.Add(characterPosition);
			breakDescriptors.Add(BreakDescriptor.CreateLastDescriptor());
		}
	}
	public class BreakDescriptor {
		const int reservedDataSize = 4;
		const int lastIndex = 0xffff;
		public const int Size = 6;
		public static BreakDescriptor FromStream(BinaryReader reader) {
			BreakDescriptor result = new BreakDescriptor();
			result.Read(reader);
			return result;
		}
		public static BreakDescriptor CreateLastDescriptor(){
			return new BreakDescriptor() { Index = unchecked((short)lastIndex) };
		}
		public short Index { get; set; }
		protected void Read(BinaryReader reader) {
			Index = reader.ReadInt16();
			reader.BaseStream.Seek(reservedDataSize, SeekOrigin.Current);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(Index);
			writer.Seek(reservedDataSize, SeekOrigin.Current);
		}
	}
}
