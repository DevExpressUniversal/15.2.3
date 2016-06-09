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

using DevExpress.Pdf.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Pdf {
	public class PdfOutputIntent {
		const string subtypeKey = "S";
		const string outputConditionKey = "OutputCondition";
		const string outputConditionIdentifierKey = "OutputConditionIdentifier";
		const string registryNameKey = "RegistryName";
		const string infoKey = "Info";
		const string destOutputProfileKey = "DestOutputProfile";
		const string defaultOutputConditionIdentifier = "sRGB IEC61966-2.1";
		const string defaultSubtype = "GTS_PDFA1";
		readonly string subtype;
		readonly string outputCondition;
		readonly string outputConditionIdentifier;
		readonly string registryName;
		readonly string info;
		readonly PdfICCBasedColorSpace destOutputProfile;
		public string Subtype { get { return subtype; } }
		public string OutputCondition { get { return outputCondition; } }
		public string OutputConditionIdentifier { get { return outputConditionIdentifier; } }
		public string RegistryName { get { return registryName; } }
		public string Info { get { return info; } }
		public PdfICCBasedColorSpace DestOutputProfile { get { return destOutputProfile; } }
		internal PdfOutputIntent() {
			subtype = defaultSubtype;
			outputConditionIdentifier = defaultOutputConditionIdentifier;
			destOutputProfile = new PdfICCBasedColorSpace();
		}
		internal PdfOutputIntent(PdfReaderDictionary dictionary) {
			subtype = dictionary.GetName(subtypeKey);
			outputCondition = dictionary.GetString(outputConditionKey);
			outputConditionIdentifier = dictionary.GetString(outputConditionIdentifierKey);
			registryName = dictionary.GetString(registryNameKey);
			info = dictionary.GetString(infoKey);
			PdfObjectCollection objects = dictionary.Objects;
			object value;
			if (dictionary.TryGetValue(destOutputProfileKey, out value)) {
				value = objects.TryResolve(value);
				IList<object> list = value as IList<object>;
				if (list != null)
					destOutputProfile = new PdfICCBasedColorSpace(dictionary.Objects, list);
				else {
					PdfStream stream = value as PdfStream;
					if (stream != null)
						destOutputProfile = new PdfICCBasedColorSpace(dictionary.Objects, stream);
				}
			}
		}
		internal object Write(PdfObjectCollection objects) {
			PdfWriterDictionary dic = new PdfWriterDictionary(objects);
			dic.Add(PdfDictionary.DictionaryTypeKey, new PdfName("OutputIntent"));
			dic.AddName(subtypeKey, subtype);
			dic.AddIfPresent(outputConditionKey, outputCondition);
			dic.AddIfPresent(outputConditionIdentifierKey, outputConditionIdentifier);
			dic.AddIfPresent(registryNameKey, registryName);
			dic.AddNotNullOrEmptyString(infoKey, info);
			if (destOutputProfile != null)
				dic.Add(destOutputProfileKey, destOutputProfile.CreateStream(objects));
			return dic;
		}
	}
}
