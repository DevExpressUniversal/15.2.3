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

using System;
using System.Text;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfContentStreamParser : PdfDocumentParser {
		class CommandName {
			readonly string name;
			public string Name { get { return name; } }
			public CommandName(string name) {
				this.name = name;
			}
		}
		static readonly byte[] endImageToken = new byte[] { (byte)'E', (byte)'I' };
		public static PdfCommandList GetContent(PdfResources resources, byte[] data) {
			return new PdfContentStreamParser(resources, data).Parse();
		}
		static int ReadInteger(PdfReaderDictionary dictionary, string key, string alternativeKey) {
			object value = null;
			if ((!dictionary.TryGetValue(key, out value) && !dictionary.TryGetValue(alternativeKey, out value)) || !(value is int))
				PdfDocumentReader.ThrowIncorrectDataException();
			return (int)value;
		}
		readonly PdfResources resources;
		readonly PdfOperands operands = new PdfOperands();
		bool IsCommandNameTerminate {
			get {
				byte current = Current;
				return IsSpace || current == StartObject || current == StartArray || current == EndArray || current == StartString || current == NameIdentifier || current == Comment || current == EndObject;
			}
		}
		protected virtual bool IsType3FontParser { get { return false; } }
		protected PdfContentStreamParser(PdfResources resources, byte[] data) : base(null, 0, 0, new PdfArrayData(data), 0) {
			this.resources = resources;
		}
		protected PdfCommandList Parse() {
			PdfCommandList commands = new PdfCommandList();
			Parse(null, commands, false);
			return commands;
		}
		void Parse(string expectedName, IList<PdfCommand> commands, bool shouldIgnoreUnknownCommands) {
			bool isType3FontParser = IsType3FontParser;
			bool hasExpectedName = !String.IsNullOrEmpty(expectedName);
			while (SkipSpaces()) {
				object value = ReadObject(false, false);
				if (value != null) {
					PdfCommand command = value as PdfCommand;
					if (command == null) {
						CommandName commandName = value as CommandName;
						if (commandName == null)
							operands.Add(value);
						else {
							string name = commandName.Name;
							if (name == expectedName)
								break;
							else if (hasExpectedName && name.StartsWith(expectedName)) {
								commands.Add(PdfCommand.Create(resources, name.Substring(expectedName.Length), new PdfOperands(), isType3FontParser, shouldIgnoreUnknownCommands));
								break;
							}
							commands.Add(PdfCommand.Create(resources, name, operands, isType3FontParser, shouldIgnoreUnknownCommands));
						}
					}
					else
						commands.Add(command);
				}
			}
			operands.VerifyCount();
		}
		PdfPaintImageCommand ParseInlineImage() {
			PdfReaderDictionary dictionary = new PdfReaderDictionary(null, 0, 0);
			while (SkipSpaces()) {
				object value = ReadObject(false, false);
				PdfName parameterName = value as PdfName;
				if (parameterName == null) {
					CommandName commandName = value as CommandName;
					if (commandName == null || commandName.Name != "ID")
						PdfDocumentReader.ThrowIncorrectDataException();
					IList<PdfFilter> filters = dictionary.GetFilters("F", "DP") ?? dictionary.GetFilters(PdfReaderStream.FilterDictionaryKey, PdfReaderStream.DecodeParametersDictionaryKey) ?? new PdfFilter[0];
					int width = ReadInteger(dictionary, PdfImage.WidthDictionaryAbbreviation, PdfImage.WidthDictionaryKey);
					int height = ReadInteger(dictionary, PdfImage.HeightDictionaryAbbreviation, PdfImage.HeightDictionaryKey);
					object colorSpaceValue;
					PdfColorSpace colorSpace = 
						(dictionary.TryGetValue(PdfImage.ColorSpaceDictionaryAbbreviation, out colorSpaceValue) || dictionary.TryGetValue(PdfImage.ColorSpaceDictionaryKey, out colorSpaceValue)) ? 
						PdfColorSpace.Parse(dictionary.Objects, colorSpaceValue, resources) : null;
					bool isMask = dictionary.GetBoolean(PdfImage.ImageMaskDictionaryAbbreviation) ?? dictionary.GetBoolean(PdfImage.ImageMaskDictionaryKey) ?? false;
					int bitsPerComponent;
					if (isMask) {
						bitsPerComponent = 1;
						if ((dictionary.ContainsKey(PdfImage.BitsPerComponentDictionaryAbbreviation) || dictionary.ContainsKey(PdfImage.BitsPerComponentDictionaryKey)) &&
							ReadInteger(dictionary, PdfImage.BitsPerComponentDictionaryAbbreviation, PdfImage.BitsPerComponentDictionaryKey) != bitsPerComponent)
								PdfDocumentReader.ThrowIncorrectDataException();
					}
					else
						bitsPerComponent = ReadInteger(dictionary, PdfImage.BitsPerComponentDictionaryAbbreviation, PdfImage.BitsPerComponentDictionaryKey);
					byte[] imageData;
					if (filters.Count == 0) {
						int componentsCount = colorSpace == null ? 1 : colorSpace.ComponentsCount;
						int size = width * componentsCount * bitsPerComponent;
						int dataSize = size / 8;
						if (size % 8 > 0)
							dataSize++;
						imageData = ReadData(dataSize * height, endImageToken, null, false);
					}
					else {
						if (!ReadNext())
							PdfDocumentReader.ThrowIncorrectDataException();
						imageData = ReadData(endImageToken, false);
					}
					return new PdfPaintImageCommand(new PdfImage(filters, width, height, colorSpace, bitsPerComponent, isMask, imageData, dictionary, resources));
				}
				else {
					if (!SkipSpaces())
						PdfDocumentReader.ThrowIncorrectDataException();
					value = ReadObject(false, false);
					if (parameterName == null || value == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					dictionary.Add(parameterName.Name, value);
				}
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		void CheckCommandTermination() {
			if (!ReadNext() || !IsCommandNameTerminate)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected override object ReadAlphabeticalObject(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			object dictionaryOrStream = ReadDictonaryOrStream(isHexadecimalStringSeparatedUsingWhiteSpaces, isIndirect);
			if (dictionaryOrStream != null)
				return dictionaryOrStream;
			byte symbol = Current;
			if (symbol == 'B') {
				if (!ReadNext())
					return new CommandName(new String((char)symbol, 1));
				PdfCommandGroup commandGroup = null;
				string expectedName = null;
				symbol = Current;
				switch (symbol) {
					case (byte)'T':
						if (!ReadNext() || !IsCommandNameTerminate)
							PdfDocumentReader.ThrowIncorrectDataException();
						return new CommandName(PdfBeginTextCommand.Name);
					case (byte)'M':
					case (byte)'D':
						expectedName = "EMC";
						if (!ReadNext() || Current != 'C')
							PdfDocumentReader.ThrowIncorrectDataException();
						int parametersCount = operands.Count;
						if (parametersCount == 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						string tag = operands.PopName();
						if (String.IsNullOrEmpty(tag))
							PdfDocumentReader.ThrowIncorrectDataException();
						if (symbol == 'D') {
							if (parametersCount != 2)
								PdfDocumentReader.ThrowIncorrectDataException();
							object value = operands.PopObject();
							PdfReaderDictionary properties = value as PdfReaderDictionary;
							if (properties == null) {
								PdfName subdictionaryName = value as PdfName;
								if (subdictionaryName == null)
									PdfDocumentReader.ThrowIncorrectDataException();
								commandGroup = new PdfMarkedContentCommand(resources, tag, subdictionaryName.Name);
							}
							else
								commandGroup = new PdfMarkedContentCommand(tag, properties);
						}
						else {
							commandGroup = new PdfMarkedContentCommand(tag);
							if (parametersCount != 1)
								PdfDocumentReader.ThrowIncorrectDataException();
						}
						break;
					case (byte)'X':
						commandGroup = new PdfCompatibilityCommandGroup();
						expectedName = "EX";
						break;
					case (byte)'I':
						CheckCommandTermination();
						return ParseInlineImage();
					default:
						if (!ReadPrev())
							PdfDocumentReader.ThrowIncorrectDataException();
						break;
				}
				if (commandGroup != null) {
					CheckCommandTermination();
					Parse(expectedName, commandGroup.Children, commandGroup.ShouldIgnoreUnknownCommands);
					return commandGroup;
				}
			}
			StringBuilder sb = new StringBuilder();
			while (!IsCommandNameTerminate) {
				sb.Append((char)Current);
				if (!ReadNext())
					break;
			}
			return new CommandName(sb.ToString());
		}
	}
}
