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

namespace DevExpress.XtraPrinting.XamlExport {
	public class XamlExportContext {
		readonly Page page;
		readonly int pageNumber;
		readonly int pageCount;
		readonly ResourceCache resourceCache;
		readonly ResourceMap resourceMap;
		readonly XamlCompatibility compatibility;
		readonly bool isPartialTrustMode;
		readonly bool embedImagesToXaml;
		readonly TextMeasurementSystem textMeasurementSystem;
		public Page Page { get { return page; } }
		public int PageNumber { get { return pageNumber; } }
		public int PageCount { get { return pageCount; } }
		public ResourceCache ResourceCache { get { return resourceCache; } }
		public ResourceMap ResourceMap { get { return resourceMap; } }
		public XamlCompatibility Compatibility { get { return compatibility; } }
		public bool IsPartialTrustMode { get { return isPartialTrustMode; } }
		public bool EmbedImagesToXaml { get { return embedImagesToXaml;  } }
		public TextMeasurementSystem TextMeasurementSystem { get { return textMeasurementSystem; } }
		public XamlExportContext(Page page, int pageNumber, int pageCount, ResourceCache resourceCache, ResourceMap resourceMap, XamlCompatibility compatibility, TextMeasurementSystem textMeasurementSystem, bool isPartialTrustMode, bool embedImagesToXaml) {
			this.page = page;
			this.pageNumber = pageNumber;
			this.pageCount = pageCount;
			this.resourceCache = resourceCache;
			this.resourceMap = resourceMap;
			this.compatibility = compatibility;
			this.textMeasurementSystem = textMeasurementSystem;
			this.isPartialTrustMode = isPartialTrustMode;
			this.embedImagesToXaml = embedImagesToXaml;
		}
	}
}
