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
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class SqlGeometryItemCollection : OwnedCollection<SqlGeometryItem> {
		public SqlGeometryItemCollection(SqlGeometryItemStorage sqlGeometryItemStorage)
			 : base(sqlGeometryItemStorage) {
		}
		protected override void OnCollectionChanged(CollectionChangedEventArgs<SqlGeometryItem> e) {
			base.OnCollectionChanged(e);
			SqlGeometryItemStorage storage = Owner as SqlGeometryItemStorage;
			if(storage != null) storage.PrepareDataLoading();
		}
	}
	public class SqlGeometryItem : IOwnedElement  {
		readonly MapItemAttributeCollection attributes;
		object owner;
		string wktString;
		int srid;
		SqlGeometryItemStorage Owner { get { return owner as SqlGeometryItemStorage; } } 
		public static SqlGeometryItem FromWkt(string wktString, int srid) {
			return new SqlGeometryItem(wktString, srid);
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("SqlGeometryItemSrid"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(0)]
		public int Srid {
			get { return srid; }
			set {
				if(srid == value)
					return;
				srid = value;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("SqlGeometryItemWktString"),
#endif
		Category(SRCategoryNames.Data), DefaultValue("")]
		public string WktString {
			get { return wktString; }
			set {
				if(wktString == value)
					return;
				wktString = value;
			}
		}
	   [
#if !SL
	DevExpressXtraMapLocalizedDescription("SqlGeometryItemAttributes"),
#endif
	   Category(SRCategoryNames.Data)]
	   public MapItemAttributeCollection Attributes { get { return attributes; } }
		#region IOwnedElement Members
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				owner = value;
			}
		}
		#endregion
		public SqlGeometryItem() : this(string.Empty, 0){			
		}
		public SqlGeometryItem(string wktString, int srid){
			attributes = new MapItemAttributeCollection();
			attributes.CollectionChanged += OnAttributesCollectionChanged;
			this.wktString = wktString;
			this.srid = srid;
		}
		void OnAttributesCollectionChanged(object sender, CollectionChangedEventArgs<MapItemAttribute> e) {
			if(Owner != null)
				Owner.PrepareDataLoading();
		}
		protected internal IList<IMapItemAttribute> GetAttributes() {
			return Attributes.Cast<IMapItemAttribute>().ToList();
		}
		protected internal string AsWkt() {
			return this.wktString;
		}
		public override string ToString() {
			return "(SqlGeometryItem)";
		} 
	}
}
