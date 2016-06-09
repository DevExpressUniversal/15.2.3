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
using System.Linq;
using System.Xml.Linq;
using System.Globalization;
using System.Drawing;
namespace DevExpress.Data.XtraReports.Labels {
	public class XDocumentLabelProductRepository : ILabelProductRepository {
		readonly XDocument document;
		XNamespace ns;
		public XDocumentLabelProductRepository(XDocument document) {
			if(document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			this.ns = document.Root.Name.Namespace;
		}
		public IEnumerable<LabelProduct> GetSortedProducts() {
			var products = from p in document.Descendants(ns + "LabelProducts")
						   select new LabelProduct()
						   {
							   Id = Convert.ToInt32(p.Element(ns + "ID").Value, CultureInfo.InvariantCulture),
							   Name = Convert.ToString(p.Element(ns + "Name").Value, CultureInfo.InvariantCulture)
						   };
			return products.OrderBy(p => p.Name);
		}
		public IEnumerable<LabelDetails> GetSortedProductDetails(int productId) {
			var test = from d in document.Descendants(ns + "LabelDetails")
					   where Convert.ToInt32(d.Element(ns + "LabelProductID").Value, CultureInfo.InvariantCulture) == productId
					   select d;
			var details = from d in document.Descendants(ns + "LabelDetails")
						  where Convert.ToInt32(d.Element(ns + "LabelProductID").Value, CultureInfo.InvariantCulture) == productId
						  select new LabelDetails()
						  {
							  Id = Convert.ToInt32(d.Element(ns + "ID").Value, CultureInfo.InvariantCulture),
							  ProductId = Convert.ToInt32(d.Element(ns + "LabelProductID").Value, CultureInfo.InvariantCulture),
							  PaperKindId = Convert.ToInt32(d.Element(ns + "PaperKindID").Value, CultureInfo.InvariantCulture),
							  Name = Convert.ToString(d.Element(ns + "Name").Value, CultureInfo.InvariantCulture),
							  Width = Convert.ToSingle(d.Element(ns + "Width").Value, CultureInfo.InvariantCulture),
							  Height = Convert.ToSingle(d.Element(ns + "Height").Value, CultureInfo.InvariantCulture),
							  HPitch = Convert.ToSingle(d.Element(ns + "HPitch").Value, CultureInfo.InvariantCulture),
							  VPitch = Convert.ToSingle(d.Element(ns + "VPitch").Value, CultureInfo.InvariantCulture),
							  TopMargin = Convert.ToSingle(d.Element(ns + "TopMargin").Value, CultureInfo.InvariantCulture),
							  LeftMargin = Convert.ToSingle(d.Element(ns + "LeftMargin").Value, CultureInfo.InvariantCulture),
							  Unit = (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), Convert.ToString(d.Element(ns + "Unit").Value, CultureInfo.InvariantCulture)),
						  };
			return details.OrderBy(d => d.Name);
		}
		public IEnumerable<PaperKindData> GetSortedPaperKinds() {
			var results = from p in document.Descendants(ns + "PaperKinds")
						  select CreatePaperKindData(p);
			return results.OrderBy(x => x.Name);
		}
		public PaperKindData GetPaperKindData(int id) {
			var results = from p in document.Descendants(ns + "PaperKinds")
						  where Convert.ToInt32(p.Element(ns + "ID").Value, CultureInfo.InvariantCulture) == id
						  select CreatePaperKindData(p);
			return results.First();
		}
		PaperKindData CreatePaperKindData(XElement xElement) {
			var data = new PaperKindData()
			{
				Id = Convert.ToInt32(xElement.Element(ns + "ID").Value, CultureInfo.InvariantCulture),
				EnumId = Convert.ToInt32(xElement.Element(ns + "EnumID").Value, CultureInfo.InvariantCulture),
				Name = Convert.ToString(xElement.Element(ns + "Name").Value, CultureInfo.InvariantCulture),
				Width = Convert.ToSingle(xElement.Element(ns + "Width").Value, CultureInfo.InvariantCulture),
				Height = Convert.ToSingle(xElement.Element(ns + "Height").Value, CultureInfo.InvariantCulture),
				IsRollPaper = Convert.ToBoolean(xElement.Element(ns + "IsRollPaper").Value),
				Unit = (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), Convert.ToString(xElement.Element(ns + "Unit").Value, CultureInfo.InvariantCulture)),
			};
			return data;
		}
	}
}
