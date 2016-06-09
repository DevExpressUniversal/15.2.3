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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public static class DocCommandHelper {
		const ushort tablePropertiesOpcode = 0x646b;
		const ushort tableDefinitionOld = 0xd606;
		const ushort tableDefinitionNew = 0xd608;
		const ushort sprmPChgTabs = 0xc615;
		public static DocPropertyContainer Traverse(byte[] grpprl, DocCommandFactory factory, BinaryReader dataStreamReader) {
			List<IDocCommand> commands = CreateDocCommands(grpprl, factory, dataStreamReader);
			int count = commands.Count;
			ChangeActionTypes changeActions = ChangeActionTypes.None;
			for (int i = 0; i < count; i++)
				changeActions |= commands[i].ChangeAction;
			DocPropertyContainer result = factory.CreatePropertyContainer(changeActions);
			for (int i = 0; i < count; i++)
				commands[i].Execute(result, dataStreamReader);
			return result;
		}
		public static void Traverse(byte[] grpprl, DocPropertyContainer container, BinaryReader dataStreamReader) {
			List<IDocCommand> commands = CreateDocCommands(grpprl, container.Factory, dataStreamReader);
			int count = commands.Count;
			ChangeActionTypes changeActions = ChangeActionTypes.None;
			for (int i = 0; i < count; i++)
				changeActions |= commands[i].ChangeAction;
			container.Update(changeActions);
			for (int i = 0; i < count; i++)
				commands[i].Execute(container, dataStreamReader);
		}
		static List<IDocCommand> CreateDocCommands(byte[] grpprl, DocCommandFactory factory, BinaryReader dataStreamReader) {
			List<IDocCommand> result = new List<IDocCommand>();
			byte[] sprm;
			short sprmSize;
			int length = grpprl.Length - 2;
			for (int byteCounter = 0; byteCounter < length; ) {
				ushort opcode = BitConverter.ToUInt16(grpprl, byteCounter);
				byteCounter += 2;
				int spra = opcode >> 13; 
				switch (spra) { 
					case 0:
					case 1:
						sprmSize = 1;
						break;
					case 2:
					case 4:
					case 5:
						sprmSize = 2;
						break;
					case 7:
						sprmSize = 3;
						break;
					case 3:
						sprmSize = 4;
						break;
					case 6:
						if (opcode == tableDefinitionOld || opcode == tableDefinitionNew) {
							sprmSize = (short)(BitConverter.ToInt16(grpprl, byteCounter) - 1);
							byteCounter += 2;
						}
						else if (opcode == sprmPChgTabs) {
							sprmSize = grpprl[byteCounter];
							byteCounter++;
							if (sprmSize == 255) {
								int itbdDelMax = grpprl[byteCounter];
								int itbdAddMaxOffset = byteCounter + 1 + itbdDelMax * 4 * 2;
								int itbdAddMax = grpprl[itbdAddMaxOffset];
								sprmSize = (short)(2 + itbdDelMax * 4 + itbdAddMax * 3);
							}
						}
						else {
							sprmSize = grpprl[byteCounter];
							byteCounter++;
						}
						break;
					default:
						DocImporter.ThrowInvalidDocFile();
						sprmSize = -1;
						break;
				}
				sprm = new byte[sprmSize];
				if (byteCounter + sprm.Length > grpprl.Length)
					return result;
				Array.Copy(grpprl, byteCounter, sprm, 0, sprm.Length);
				byteCounter += sprmSize;
				IDocCommand command = factory.CreateCommand((short)opcode);
				command.Read(sprm);
				result.Add(command);
				if (opcode == tablePropertiesOpcode)
					return result;
			}
			return result;
		}
		public static byte[] CreateSinglePropertyModifier(short signedOpcode, byte[] parameters) {
			ushort opcode = (ushort)signedOpcode;
			byte[] result;
			if (opcode >> 13 != 6) {
				result = new byte[2 + parameters.Length];
				Array.Copy(BitConverter.GetBytes(opcode), 0, result, 0, 2);
				Array.Copy(parameters, 0, result, 2, parameters.Length);
			}
			else {
				result = new byte[2 + parameters.Length + 1];
				Array.Copy(BitConverter.GetBytes(opcode), 0, result, 0, 2);
				result[2] = (byte)parameters.Length;
				Array.Copy(parameters, 0, result, 3, parameters.Length);
			}
			return result;
		}
	}
}
