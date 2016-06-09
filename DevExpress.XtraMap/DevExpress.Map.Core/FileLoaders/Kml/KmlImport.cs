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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Map.Kml.Model;
namespace DevExpress.Map.Kml {
	public enum KmlVersion { Unknown, v20, v21, v22 };
	public interface IKmlStyleProvider {
		Style ResolveStyle(Feature feature);
		ColorABGR CalculateRandomColor(ColorABGR color);
	}
	public class KmlImporter : IKmlStyleProvider {
		static Dictionary<string, KmlVersion> supportedKmlVersions;
		KmlModelParserBase parser;
		Root root;
		string kmlNamespace = string.Empty;
		KmlVersion version = KmlVersion.Unknown;
		protected KmlModelParserBase Parser { get { return parser; } }
		public string WorkingFolder { get; set; }
		public Root Root { get { return root; } }
		public string KmlNamespace { get { return kmlNamespace; } }
		public KmlVersion Version { get { return version; } }
		public KmlImporter() {
			PopulateSupportedKmlVersions();
		}
		void PopulateSupportedKmlVersions() {
			supportedKmlVersions = new Dictionary<string, KmlVersion>();
			supportedKmlVersions[KmlTokens.KmlOldNamespaceName + "2.0"] = KmlVersion.v20;
			supportedKmlVersions[KmlTokens.KmlOldNamespaceName + "2.1"] = KmlVersion.v21;
			supportedKmlVersions[KmlTokens.KmlOldNamespaceName + "2.2"] = KmlVersion.v22;
			supportedKmlVersions[KmlTokens.KmlNamespaceName + "2.2"] = KmlVersion.v22;
		}
		Style IKmlStyleProvider.ResolveStyle(Feature feature) {
			return Root != null ? Root.ResolveElementStyle(feature) : null;
		}
		ColorABGR IKmlStyleProvider.CalculateRandomColor(ColorABGR color) {
			return Parser.Convert.GenerateRandomColor(color);
		}
		public virtual bool CanImport(Stream stream) {
			return stream != null && !Object.Equals(stream, Stream.Null) && (!stream.CanSeek || stream.Length > 0);
		}
		public virtual object Import(Stream stream) {
			BeforeImport();
			if (CanImport(stream)) {
				ReadKmlStructure(stream);
			}
			return Root;
		}
		protected void BeforeImport() {
			this.root = null;
			this.version = KmlVersion.Unknown;
			this.kmlNamespace = string.Empty;
		}
		void ReadKmlStructure(Stream stream) {
			XDocument doc = XDocument.Load(stream);
			if (doc == null) return;
			XElement elRoot = doc.Root;
			if (elRoot == null) return;
			XName name = elRoot.Name;
			if (name == null) return;
			this.kmlNamespace = name.NamespaceName;
			if (name.LocalName != KmlTokens.Kml)
				return;
			this.root = new Root();
			this.parser = CreateParser();
			Parser.Parse(Root, doc);
		}
		KmlModelParserBase CreateParser() {
			return new KmlModelParser() { WorkingFolder = WorkingFolder };
		}
		protected internal KmlVersion ParseKmlVersion(string namespaceString) {
			if (string.IsNullOrEmpty(namespaceString))
				return KmlVersion.Unknown;
			foreach (KeyValuePair<string, KmlVersion> item in supportedKmlVersions) {
				if (namespaceString.StartsWith(item.Key))
					return item.Value;
			}
			return KmlVersion.Unknown;
		}
	}
}
