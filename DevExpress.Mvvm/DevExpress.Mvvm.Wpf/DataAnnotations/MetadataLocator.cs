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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace DevExpress.Mvvm.DataAnnotations {
	public interface IMetadataLocator {
		Type[] GetMetadataTypes(Type type);
	}
	public class MetadataLocator : IMetadataLocator {
		public static IMetadataLocator Default;
		public static MetadataLocator Create() {
			return new MetadataLocator(Enumerable.Empty<Tuple<Type, Type>>());
		}
		readonly IEnumerable<Tuple<Type, Type>> infoList;
		IDictionary<Type, Type[]> dictionary;
		IDictionary<Type, Type[]> Dictionary {
			get {
				return dictionary ?? (dictionary = MetadataHelper.InternalMetadataProviders.Union(infoList).GroupBy(x => x.Item1, x => x.Item2).ToDictionary(x => x.Key, x => x.ToArray()));
			}
		}
		MetadataLocator(IEnumerable<Tuple<Type, Type>> infoList) {
			this.infoList = infoList;
			MetadataHelper.RegisterLocator(this);
		}
		internal void Update() {
			dictionary = null;
		}
		Type[] IMetadataLocator.GetMetadataTypes(Type type) {
			Type[] result;
			Dictionary.TryGetValue(type, out result);
			return result;
		}
		public MetadataLocator AddMetadata(Type type, Type metadataType) {
			var tuple = new Tuple<Type, Type>(type, metadataType);
			return AddMetadata(new[] { tuple });
		}
		public MetadataLocator AddMetadata(Type metadataType) {
			return AddMetadata(MetadataHelper.GetMetadataInfoList(metadataType));
		}
		public MetadataLocator AddMetadata<T, TMetadata>() {
			return AddMetadata(typeof(T), typeof(TMetadata));
		}
		public MetadataLocator AddMetadata<TMetadata>() {
			return AddMetadata(typeof(TMetadata));
		}
		MetadataLocator AddMetadata(IEnumerable<Tuple<Type, Type>> newInfoList) {
			MetadataHelper.CheckMetadata(newInfoList);
			return new MetadataLocator(infoList.Union(newInfoList));
		}
	}
}
