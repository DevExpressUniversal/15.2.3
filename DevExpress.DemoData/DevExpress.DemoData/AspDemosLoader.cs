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

using DevExpress.DemoData.Model;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
namespace DevExpress.DemoData {
	public static class AspDemosLoader {
		class DemoInfo {
			public string DemoKey;
			public string GroupKey;
			public string ProductKey;
			public string DemoTitle;
			public bool IsNew;
			public bool IsUpdated;
		}
		static IEnumerable<DemoInfo> Parse(Stream stream) {
			XDocument doc = XDocument.Load(stream);
			var products = doc.DescendantNodes().OfType<XElement>().Where(x => x.Name.LocalName == "DemoProduct");
			foreach(var p in products) {
				string productName = p.Attributes().Single(x => x.Name.LocalName == "Key").Value;
				var groups = p.DescendantNodes().OfType<XElement>().Where(x => x.Name.LocalName == "DemoGroup");
				foreach(var g in groups) {
					var groupKey = g.Attributes().Single(x => x.Name.LocalName == "Key").Value;
					var demos = g.DescendantNodes().OfType<XElement>().Where(x => x.Name.LocalName == "Demo");
					foreach(var d in demos) {
						var demoKey = d.Attributes().Single(x => x.Name.LocalName == "Key").Value;
						var demoTitle = d.Attributes().Single(x => x.Name.LocalName == "Title").Value;
						var isNew = d.Attributes().FirstOrDefault(x => x.Name.LocalName == "IsNew");
						var isUpdated = d.Attributes().FirstOrDefault(x => x.Name.LocalName == "IsUpdated");
						yield return new DemoInfo {
							DemoKey = demoKey,
							GroupKey = groupKey,
							ProductKey = productName,
							DemoTitle = demoTitle,
							IsNew = isNew == null ? false : bool.Parse(isNew.Value),
							IsUpdated = isUpdated == null ? false : bool.Parse(isUpdated.Value)
						};
					}
				}
			}
		}
		public static void LoadASPDemos() {
			using(var s = AssemblyHelper.GetResourceStream(typeof(DemoImage).Assembly, "Data/Asp/Demos.Xml", true)) {
				LoadDemos(Repository.AspPlatform, s);
			}
		}
		public static void LoadMVCDemos() {
			using(var s = AssemblyHelper.GetResourceStream(typeof(DemoImage).Assembly, "Data/Mvc/Demos.Xml", true)) {
				LoadDemos(Repository.MvcPlatform, s);
			}
		}
		static void LoadDemos(Platform platform, Stream stream) {
			var infos = Parse(stream);
			var j = from product in platform.Products
					join info in infos on product.Name equals info.ProductKey
					select new { product, info };
			var grouped = j.GroupBy(x => x.product).ToList();
			Debug.Assert(grouped.Count == platform.Products.Count);
			foreach(var p in grouped) {
				var product = p.Key;
				Debug.Assert(product.Demos.Single() is WebDemo);
				var demo = product.Demos.Single();
				demo.Modules.AddRange(p.Select(x => 
					new WebModule(
						demo,
						x.info.DemoKey,
						x.info.DemoTitle,
						x.info.GroupKey,
						"",
						null,
						x.info.IsNew ? Repository.CurrentDXVersion : KnownDXVersion.Before142,
						x.info.IsUpdated ? Repository.CurrentDXVersion : KnownDXVersion.Before142)));
			}
		}
	}
}
