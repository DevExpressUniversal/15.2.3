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
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
namespace DevExpress.XtraExport {
	public interface IXmlWriteTo {
		void WriteTo(XmlWriter writer);
	}
	public class SharedStringsDocument : IXmlWriteTo {
		const string systemStringExcludePattern = "_x005F$1";
		const string systemStringAttributePattern = "_x00";
		static readonly Regex excludeRegEx = new Regex("(_x00[0-9a-fA-F]{2}_)", RegexOptions.Compiled);
		static readonly Regex systemCharRegEx = new Regex("([\x00-\x08\x0B-\x0C\x0E-\x1F])", RegexOptions.Compiled);
		Dictionary<string, int> indexes = new Dictionary<string, int>();
		List<string> list = new List<string>();
		public int GetStringIndex(string text) {
			int index;
			if(indexes.TryGetValue(text, out index)) {
				return index;
			}
			index = indexes.Count;
			indexes.Add(text, index);
			list.Add(Validate(text));
			return index;
		}
		public virtual void WriteTo(XmlWriter writer) {
			writer.WriteStartDocument(true);
			writer.WriteStartElement("", "sst", NamespaceURI);
			writer.WriteAttributeString("uniqueCount", indexes.Count.ToString());
			for(int n = 0; n < this.indexes.Count; n++) {
				writer.WriteStartElement("", "si", NamespaceURI);
				writer.WriteStartElement("", "t", NamespaceURI);
				writer.WriteAttributeString("xml:space", "preserve");
				writer.WriteString(list[n]);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
		string Validate(string text) {
			text = excludeRegEx.Replace(text, systemStringExcludePattern);
			return systemCharRegEx.Replace(text, new MatchEvaluator(Evaluator));
		}
		string Evaluator(Match match) {
			return string.Concat(systemStringAttributePattern, ((int)match.Groups[1].Value[0]).ToString("X2"), "_");
		}
		public string NamespaceURI {
			get { return XlsxHelper.MainNs; }
		}
	}
}
