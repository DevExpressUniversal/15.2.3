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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfOptionalContentVisibilityPolicy.AnyOn)]
	public enum PdfOptionalContentVisibilityPolicy { AllOn, AnyOn, AnyOff, AllOff }
	public class PdfOptionalContentMembership : PdfOptionalContent {
		internal const string Type = "OCMD";
		const string groupsDictionaryKey = "OCGs";
		const string visibilityPolicyDictionaryName = "P";
		const string visibilityExpressionDictionaryName = "VE";
		readonly IList<PdfOptionalContentGroup> groups;
		readonly PdfOptionalContentVisibilityPolicy visibilityPolicy;
		readonly PdfOptionalContentVisibilityExpression visibilityExpression;
		public IList<PdfOptionalContentGroup> Groups { get { return groups; } }
		public PdfOptionalContentVisibilityPolicy VisibilityPolicy { get { return visibilityPolicy; } }
		public PdfOptionalContentVisibilityExpression VisibilityExpression { get { return visibilityExpression; } }
		internal PdfOptionalContentMembership(PdfReaderDictionary dictionary) : base (dictionary.Number) {
			object value;
			if (dictionary.TryGetValue(groupsDictionaryKey, out value)) {
				groups = new List<PdfOptionalContentGroup>();
				PdfObjectCollection objects = dictionary.Objects;
				value = objects.TryResolve(value);
				PdfReaderDictionary groupDictionary = value as PdfReaderDictionary;
				if (groupDictionary == null) {
					IList<object> list = value as IList<object>;
					if (list == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					foreach (object item in list) {
						groupDictionary = objects.TryResolve(item) as PdfReaderDictionary;
						if (groupDictionary == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						AddGroup(groupDictionary);
					}
				}
				else
					AddGroup(groupDictionary);
			}
			visibilityPolicy = PdfEnumToStringConverter.Parse<PdfOptionalContentVisibilityPolicy>(dictionary.GetName(visibilityPolicyDictionaryName));
			IList<object> array = dictionary.GetArray(visibilityExpressionDictionaryName);
			if (array != null)
				visibilityExpression = new PdfOptionalContentVisibilityExpression(dictionary.Objects, array);
		}
		void AddGroup(PdfReaderDictionary dictionary) {
			PdfOptionalContentGroup group = PdfOptionalContent.Create(dictionary) as PdfOptionalContentGroup;
			if (group == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			groups.Add(group);
		}
		protected internal override object Write(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, Type);
			if (groups != null)
				dictionary[groupsDictionaryKey] = groups.Count == 1 ? (object)objects.AddObject(groups[0]) : new PdfWritableObjectArray(groups, objects);
			dictionary.AddEnumName(visibilityPolicyDictionaryName, visibilityPolicy);
			dictionary.Add(visibilityExpressionDictionaryName, visibilityExpression);
			return dictionary;
		}
	}
}
