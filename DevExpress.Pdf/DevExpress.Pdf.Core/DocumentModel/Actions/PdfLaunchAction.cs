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
using DevExpress.Utils;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfLaunchOperation.None, false)]
	public enum PdfLaunchOperation { 
		None, 
		[PdfFieldName("open")]
		Open, 
		[PdfFieldName("print")]
		Print 
	}
	public class PdfLaunchAction : PdfAction {
		internal const string Name = "Launch";
		const string fileDictionaryKey = "F";
		const string windowsDictionaryKey = "Win";
		const string fileNameDictionaryKey = "F";
		const string defaultDirectoryDictionaryKey = "D";
		const string operationDictionaryKey = "O";
		const string parametersDictionaryKey = "P";
		const string newWindowDictionaryKey = "NewWindow";
		readonly PdfFileSpecification fileSpecification;
		readonly string fileName;
		readonly string defaultDirectory;
		readonly PdfLaunchOperation operation;
		readonly string parameters;
		readonly DefaultBoolean newWindow;
		protected override string ActionType { get { return Name; } }
		public PdfFileSpecification FileSpecification { get { return fileSpecification; } }
		public string FileName { get { return fileName; } }
		public string DefaultDirectory { get { return defaultDirectory; } }
		public PdfLaunchOperation Operation { get { return operation; } }
		public string Parameters { get { return parameters; } }
		public DefaultBoolean NewWindow { get { return newWindow; } }
		internal PdfLaunchAction(PdfReaderDictionary dictionary) : base(dictionary) {
			fileSpecification = PdfFileSpecification.Parse(dictionary, fileDictionaryKey, false);
			PdfReaderDictionary launchDictionary = dictionary.GetDictionary(windowsDictionaryKey);
			if (launchDictionary != null) {
				fileName = launchDictionary.GetString(fileNameDictionaryKey);
				defaultDirectory = launchDictionary.GetString(defaultDirectoryDictionaryKey);
				operation = PdfEnumToStringConverter.Parse<PdfLaunchOperation>(launchDictionary.GetString(operationDictionaryKey));
				parameters = launchDictionary.GetString(parametersDictionaryKey);
			}
			bool? newWindowValue = dictionary.GetBoolean(newWindowDictionaryKey);
			if (newWindowValue.HasValue)
				newWindow = newWindowValue.Value ? DefaultBoolean.True : DefaultBoolean.False;
			else 
				newWindow = DefaultBoolean.Default;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(fileDictionaryKey, fileSpecification);
			if (!String.IsNullOrEmpty(fileName) || !String.IsNullOrEmpty(defaultDirectory) || operation != PdfLaunchOperation.None || !String.IsNullOrEmpty(parameters)) {
				PdfWriterDictionary launchDictionary = new PdfWriterDictionary(objects);
				launchDictionary.AddIfPresent(fileDictionaryKey, fileName);
				launchDictionary.AddIfPresent(defaultDirectoryDictionaryKey, defaultDirectory);
				launchDictionary.AddASCIIString(operationDictionaryKey, PdfEnumToStringConverter.Convert<PdfLaunchOperation>(operation));
				launchDictionary.AddIfPresent(parametersDictionaryKey, parameters);
				dictionary.Add(windowsDictionaryKey, launchDictionary);
			}
			if (newWindow != DefaultBoolean.Default)
				dictionary.Add(newWindowDictionaryKey, newWindow == DefaultBoolean.True);
			return dictionary;
		}
	}
}
