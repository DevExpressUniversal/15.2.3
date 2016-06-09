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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class SqlGeometryItemCollection : MapDependencyObjectCollection<SqlGeometryItem> {
	}
	public class SqlGeometryItem : MapDependencyObject {
	   public static readonly DependencyProperty WktStringProperty = DependencyPropertyManager.Register("WktString",
	   typeof(string), typeof(SqlGeometryItem), new PropertyMetadata(null));
	   public static readonly DependencyProperty SridProperty = DependencyPropertyManager.Register("Srid",
		   typeof(int), typeof(SqlGeometryItem), new PropertyMetadata(0));
	   internal static readonly DependencyPropertyKey AttributesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Attributes",
			typeof(MapItemAttributeCollection), typeof(SqlGeometryItem), new PropertyMetadata());
	   public static readonly DependencyProperty AttributesProperty = AttributesPropertyKey.DependencyProperty;
		[Category(Categories.Data)]
		public MapItemAttributeCollection Attributes {
			get { return (MapItemAttributeCollection)GetValue(AttributesProperty); }
		}
		[Category(Categories.Data)]
		public int Srid {
			get { return (int)GetValue(SridProperty); }
			set { SetValue(SridProperty, value); }
		}
		[Category(Categories.Data)]
		public string WktString {
			get { return (string)GetValue(WktStringProperty); }
			set { SetValue(WktStringProperty, value); }
		}
		public SqlGeometryItem(): this(string.Empty, 0) {			
		}
		public SqlGeometryItem(string wktString, int srid) {
			SetValue(AttributesPropertyKey, new MapItemAttributeCollection(this));
			SetValue(WktStringProperty, wktString);
			SetValue(SridProperty, srid);
		}
		protected internal IList<IMapItemAttribute> GetAttributes() {
			return Attributes.Cast<IMapItemAttribute>().ToList();
		}
		protected internal string AsWkt() {
			return WktString;
		}
		protected override MapDependencyObject CreateObject() {
			return new SqlGeometryItem();
		}
	}
}
