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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework {
	public interface IWcfEdmInfoProvider {
		Stream GetEdmItemCollectionStream(Type clrType);
		string GetSourceUrl(Type clrType);
	}
	public class WcfManifestResourceEdmInfoProvider : IWcfEdmInfoProvider {
		protected readonly string[] resourceNames;
		public WcfManifestResourceEdmInfoProvider(string[] resourceNames) {
			this.resourceNames = resourceNames;
		}
		public Stream GetEdmItemCollectionStream(Type clrType) {
			string edmxName = clrType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("edmx") && x.Contains("Service_References") && (resourceNames.Any(name => x.Contains(name)) || x.Contains(clrType.Name)));
			return clrType.Assembly.GetManifestResourceStream(edmxName);
		}
		public string GetSourceUrl(Type clrType) {
			string edmxName = clrType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("datasvcmap") && x.Contains("Service_References") && (resourceNames.Any(name => x.Contains(name)) || x.Contains(clrType.Name)));
			Stream stream = clrType.Assembly.GetManifestResourceStream(edmxName);   
			try
			{
				return GetSourceUrl(stream);
			}
			finally{
				stream.Close();
			}
		}
		public static string GetSourceUrl(Stream datasvcmapContent) {
			XDocument edmxDoc = XDocument.Load(datasvcmapContent);			
			try {
				XElement metadataFile = edmxDoc.Root.Elements().First(element => element.Name.LocalName == "Metadata").
					Elements().First(element => element.Name.LocalName == "MetadataFile");
				string result = metadataFile.Attributes().First(attr => attr.Name == "SourceUrl").Value;
				if(result.EndsWith(@"/$metadata"))
					result = result.Remove(result.Length - 10);
				else if(result.EndsWith(@"/") || result.EndsWith(@"\"))
					result = result.Remove(result.Length - 1);
				return result;
			}
			catch { }
			return string.Empty;
		}
	}
}
