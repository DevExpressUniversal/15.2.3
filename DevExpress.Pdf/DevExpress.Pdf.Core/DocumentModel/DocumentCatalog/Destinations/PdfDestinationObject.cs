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
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfDestinationObject {
		PdfDestination destination;
		readonly string destinationName;
		public string DestinationName { get { return destinationName; } }
		public PdfDestinationObject(PdfDestination destination) {
			this.destination = destination;
		}
		public PdfDestinationObject(string destinationName) {
			this.destinationName = destinationName;
		}
		public PdfDestination GetDestination(PdfDocumentCatalog documentCatalog, bool isInternal) {
			if (destination == null && destinationName != null) {
				IDictionary<string, PdfDestination> destinations = documentCatalog.Destinations;
				if (destinations != null)
					if (!destinations.TryGetValue(destinationName, out destination))
						foreach (KeyValuePair<string, PdfDestination> pair in destinations)
							if (pair.Key.Equals(destinationName, StringComparison.OrdinalIgnoreCase)) {
								destination = pair.Value;
								break;
							}
			}
			if (isInternal && destination != null)
				destination.ResolveInternalPage();
			return destination; 
		}
		public object ToWriteableObject(PdfDocumentCatalog documentCatalog, PdfObjectCollection objects, bool isInternal) {
			objects.AddSavedDestinationName(this.destinationName);
			string destinationName = objects.GetDestinationName(this.destinationName);
			return destinationName == null ? (object)objects.AddObject(GetDestination(documentCatalog, isInternal)) : (object)new PdfNameTreeEncoding().GetBytes(destinationName);
		}
	}
}
