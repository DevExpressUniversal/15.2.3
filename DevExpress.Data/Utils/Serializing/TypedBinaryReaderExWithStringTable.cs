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

using DevExpress.Compatibility.System.Collections.Specialized;
using System;
using System.Collections.Specialized;
using System.IO;
namespace DevExpress.Utils.Serializing {
	#region TypedBinaryReaderExWithStringTable
	public class TypedBinaryReaderExWithStringTable : TypedBinaryReaderEx {
		int startPosition = -1;
		int endPosition = -1;
		int stringTablePosition = -1;
		StringCollection stringTable;
		public TypedBinaryReaderExWithStringTable(BinaryReader input)
			: base(input) {
			this.stringTable = new StringCollection();
		}
		public override object ReadObject() {
			if(startPosition == -1) {
				int offset = Input.ReadInt32();
				startPosition = (int)Input.BaseStream.Position;
				Input.BaseStream.Seek(offset, SeekOrigin.Current);
				stringTablePosition = (int)Input.BaseStream.Position;
				ReadStringTable();
				endPosition = (int)Input.BaseStream.Position;
				Input.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			}
			object obj = base.ReadObject();
			if(Input.BaseStream.Position == stringTablePosition)
				Input.BaseStream.Seek(endPosition, SeekOrigin.Begin);
			return obj;
		}
		protected internal virtual void ReadStringTable() {
			int count = (int)ReadObject();
			for(int i = 0; i < count; i++)
				stringTable.Add(Input.ReadString());
		}
		protected internal override string ReadString() {
			int stringIndex = (int)ReadObject();
			return stringTable[stringIndex];
		}
	}
	#endregion
}
