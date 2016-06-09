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
	public class PdfMarkedContentCommand : PdfCommandGroup {
		internal const string EndToken = "EMC";
		readonly string tag;
		readonly PdfProperties properties;
		public string Tag { get { return tag; } }
		public PdfProperties Properties { get { return properties; } }
		protected override string Suffix { get { return EndToken; } }
		public PdfMarkedContentCommand(string tag) {
			this.tag = tag;
		}
		internal PdfMarkedContentCommand(string tag, PdfReaderDictionary properties) : this(tag) {
			this.properties = PdfProperties.Parse(properties);
		}
		internal PdfMarkedContentCommand(PdfResources resources, string tag, string propertiesName) : this(tag) {
			properties = resources.GetProperties(propertiesName);
		}
		protected override IEnumerable<object> GetPrefix(PdfResources resources) {
			List<object> result = new List<object>();
			result.Add(new PdfName(tag));
			if (properties == null)
				result.Add(new PdfToken("BMC"));
			else {
				result.Add(resources.FindPropertiesName(properties) ?? properties.ToWritableObject(null));
				result.Add(new PdfToken("BDC"));
			}
			return result;
		}
	}
}
