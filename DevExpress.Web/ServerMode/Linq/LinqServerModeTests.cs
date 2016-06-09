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

#if DebugTest
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Filtering;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data.Helpers;
using System.Security;
namespace DevExpress {
	using System;
	class NonCoverAttribute : Attribute {
	}
}
namespace DevExpress.Data.Linq.Tests {
	[TestFixture]
	public class LinqServerModeTests {
		public static Victim[] Create3Victims() {
			return new Victim[]{
				new Victim(1, "A"),
				new Victim(2, "A"),
				new Victim(3, "B"),
			};
		}
		public static Victim[] CreateVictims(int amountOfVictims, int strModulo) {
			Victim[] res = new Victim[amountOfVictims];
			for(int i = 0; i < res.Length; i++)
				res[i] = new Victim(i, (i % strModulo).ToString());
			return res;
		}
		public static Victim[] Create3000Victims() {
			Victim[] res = new Victim[3000];
			for(int i = 0; i < res.Length; i++)
				res[i] = new Victim(i, i.ToString());
			return res;
		}
		[Test]
		public void LinqServerModeDataSource_Basic() {
			LinqServerModeDataSource ds = new LinqServerModeDataSource();
			ds.Selecting += new EventHandler<LinqServerModeDataSourceSelectEventArgs>(ds_Selecting);
			ds.GetView().Select(null, new System.Web.UI.DataSourceViewSelectCallback(ds_SelectTarget));
		}
		static void ds_SelectTarget(IEnumerable qq) {
			IListServer srv = qq as IListServer;
			Assert.IsNotNull(srv);
			Assert.AreEqual(3, srv.Count);
		}
		static void ds_Selecting(object sender, LinqServerModeDataSourceSelectEventArgs e) {
			e.QueryableSource = Create3Victims().AsQueryable();
			e.KeyExpression = "Key";
		}
		[SecuritySafeCritical]
		[Test]
		public void PrimaryKeyReported_S91845() {
			var viewSchema = new System.Web.UI.Design.DataSetViewSchema(DevExpress.Data.Linq.Design.LinqServerModeDataSourceDesignerView.CreateSchemaTable(typeof(Victim)));
			bool keyFound = false;
			foreach(var col in viewSchema.GetFields()) {
				if(col.Name == "Key") {
					keyFound = true;
					Assert.IsTrue(col.PrimaryKey, "'{0}' column does not marked PK", col.Name);
				} else {
					Assert.IsFalse(col.PrimaryKey, "'{0}' column should not be marked as PK", col.Name);
				}
			}
			Assert.IsTrue(keyFound, "Key column was not found");
		}
	}
}
#endif
