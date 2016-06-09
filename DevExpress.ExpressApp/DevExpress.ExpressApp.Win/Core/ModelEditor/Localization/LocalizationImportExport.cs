#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.IO;
using System.Text;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class LocalizationDocumentData {
		private string sourceAspect;
		private string destinationAspect;
		private IEnumerable<ILocalizationItem> localizationItems;
		public LocalizationDocumentData(IEnumerable<ILocalizationItem> localizationItems, string sourceAspect, string destinationAspect) {
			this.localizationItems = localizationItems;
			this.sourceAspect = sourceAspect;
			this.destinationAspect = destinationAspect;
		}
		public string SourceAspect {
			get { return sourceAspect; }
		}
		public string DestinationAspect {
			get { return destinationAspect; }
		}
		public IEnumerable<ILocalizationItem> LocalizationItems {
			get { return localizationItems; }
		}
	}
	public interface ILocalizationImportExport {
		string Extension { get; }
		string Filter { get; }
		LocalizationDocumentData Import(Stream stream);
		void Export(Stream stream, LocalizationDocumentData localizationDocumentData);
	}
	public class CsvImportExport : ILocalizationImportExport {
		private enum ParcerState { NotString, String, StringEnding };
		private const string columnDelimiter = ",";
		private void Export(StreamWriter streamWriter, LocalizationDocumentData localizationDocumentData) {
			streamWriter.WriteLine(CreateCSVLine(new string[] {
				"Property Name",
				localizationDocumentData.SourceAspect,
				localizationDocumentData.DestinationAspect,
				"Node Path",
				"Description",
				"IsTranslated"
			}));
			foreach(ILocalizationItem item in localizationDocumentData.LocalizationItems) {
				string tableLine = CreateCSVLine(new string[] {
				   item.PropertyName,
				   item.DefaultLanguageValue,
				   item.TranslatedValue,
				   item.NodePath,
				   item.Description,
				   item.IsTranslated.ToString()
				});
				streamWriter.WriteLine(tableLine);
			}
		}
		private string CreateCSVLine(string[] values) {
			string result = string.Empty;
			if(values.Length > 0) {
				result = EncodeString(values[0]);
				for(int i = 1; i < values.Length; i++) {
					result += columnDelimiter + EncodeString(values[i]);
				}
			}
			return result;
		}
		private string EncodeString(string str) {
			return string.IsNullOrEmpty(str) ? "" : "\"" + str.Replace("\"", "\"\"") + "\"";
		}
		private LocalizationDocumentData Import(StreamReader streamReader) {
			string sourceAspect;
			string destinationAspect;
			BindingList<ILocalizationItem> localizationItems = new BindingList<ILocalizationItem>();
			bool isString = false;
			int lineNumber = 1;
			string row = streamReader.ReadLine();
			if(row == null) {
				ThrowParceException(lineNumber);
			}
			List<string> values = new List<string>();
			ParseCSVLine(row, values, isString);
			if(values.Count < 4) {
				ThrowParceException(lineNumber);
			}
			sourceAspect = values[1];
			destinationAspect = values[2];
			while((row = streamReader.ReadLine()) != null) {
				values = new List<string>();
				lineNumber++;
				isString = false;
				while(ParseCSVLine(row, values, isString)) {
					isString = true;
					row = streamReader.ReadLine();
					lineNumber++;
				}
				if(values.Count < 4) {
					ThrowParceException(lineNumber);
				}
				string description = string.Empty;
				if(values.Count >= 5) {
					description = values[4];
				}
				string isTranslated = string.Empty;
				if(values.Count == 6) {
					isTranslated = values[5];
				}
				ILocalizationItem item = new LocalizationItemUnbind(values[0], values[1], values[2], values[3], description, isTranslated);
				localizationItems.Add(item);
			}
			return new LocalizationDocumentData(localizationItems, sourceAspect, destinationAspect);
		}
		private void ThrowParceException(int line) {
			throw new Exception("CSV parse error, line " + line.ToString());
		}
		private bool ParseCSVLine(string line, IList<string> values, bool isString) {
			ParcerState parcerState = isString ? ParcerState.String : ParcerState.NotString;
			string value = string.Empty;
			if(values.Count > 0) {
				value = values[values.Count - 1];
				values.RemoveAt(values.Count - 1);
			}
			if(isString) {
				value += Environment.NewLine;
			}
			for(int i = 0; i < line.Length; i++) {
				switch(line[i]) {
					case '"':
						switch(parcerState) {
							case ParcerState.NotString:
								parcerState = ParcerState.String;
								break;
							case ParcerState.StringEnding:
								parcerState = ParcerState.String;
								value += line[i];
								break;
							case ParcerState.String:
								parcerState = ParcerState.StringEnding;
								break;
						}
						break;
					case ',':
						if(parcerState == ParcerState.StringEnding) {
							parcerState = ParcerState.NotString;
						}
						if(parcerState == ParcerState.NotString) {
							values.Add(value);
							value = string.Empty;
						}
						else {
							value += line[i];
						}
						break;
					default:
						if(parcerState == ParcerState.StringEnding) {
							parcerState = ParcerState.NotString;
						}
						value += line[i];
						break;
				}
			}
			isString = parcerState == ParcerState.String;
			values.Add(value);
			return isString;
		}
		public string Extension {
			get { return ".csv"; }
		}
		public string Filter {
			get { return "Translation files (*.csv)|*.csv"; }
		}
		public void Export(Stream stream, LocalizationDocumentData data) {
			StreamWriter streamWriter = new StreamWriter(stream, new UTF8Encoding(false));
			Export(streamWriter, data);
			streamWriter.Flush();
		}
		public void Export(string fileName, LocalizationDocumentData data) {
			using(FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate)) {
				fileStream.SetLength(0);
				using(StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false))) {
					Export(streamWriter, data);
				}
			}
		}
		public LocalizationDocumentData Import(Stream stream) {
			StreamReader streamReader = new StreamReader(stream, new UTF8Encoding(false));
			return Import(streamReader);
		}
		public LocalizationDocumentData Import(string fileName) {
			using(FileStream fileStream = new FileStream(fileName, FileMode.Open))
			using(StreamReader streamReader = new StreamReader(fileStream, new UTF8Encoding(false))) {
				return Import(streamReader);
			}
		}
	}
}
