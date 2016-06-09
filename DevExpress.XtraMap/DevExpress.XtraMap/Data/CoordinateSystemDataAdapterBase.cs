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

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public abstract class CoordinateSystemDataAdapterBase : MapDataAdapterBase {
		static protected internal SourceCoordinateSystem DefaultSourceCoordinateSystem { get { return new GeoSourceCoordinateSystem(); } }
		SourceCoordinateSystem sourceCoordSystem;
		protected bool AreItemsLoaded { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("CoordinateSystemDataAdapterBaseSourceCoordinateSystem"),
#endif
		Category(SRCategoryNames.Options),
		Editor("DevExpress.XtraMap.Design.SourceCoordinateSystemPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableObjectConverterShowsValueTypeNameInParentheses," + AssemblyInfo.SRAssemblyMapDesign)]
		public SourceCoordinateSystem SourceCoordinateSystem {
			get {
				if (sourceCoordSystem == null)
					sourceCoordSystem = DefaultSourceCoordinateSystem;
				return sourceCoordSystem;
			}
			set {
				if (value == null)
					value = DefaultSourceCoordinateSystem;
				if (Object.Equals(sourceCoordSystem, value))
					return;
				sourceCoordSystem = value;
				MapUtils.SetOwner(sourceCoordSystem, this);
				OnPropertyChanged();
			}
		}
		bool ShouldSerializeSourceCoordinateSystem() { return !SourceCoordinateSystem.IsDefault; }
		void ResetSourceCoordinateSystem() { SourceCoordinateSystem = DefaultSourceCoordinateSystem; }
		protected CoordinateSystemDataAdapterBase() {
			this.sourceCoordSystem = CreateDefaultCoordinateSystem();
		}
		protected virtual SourceCoordinateSystem CreateDefaultCoordinateSystem() {
			return DefaultSourceCoordinateSystem;
		}
		protected internal virtual SourceCoordinateSystem GetActualCoordinateSystem() {
		   return SourceCoordinateSystem; 
		}
		protected internal virtual void OnPropertyChanged() {
			PrepareDataLoading();
		}
		protected void LoadItems(List<MapItem> items) {
			InnerItems.SetRangeInternal(items);
			AreItemsLoaded = true;
			NotifyDataChanged(MapUpdateType.Render);
		}
		protected virtual void OnLoaderItemsLoaded(object sender, ItemsLoadedEventArgs<MapItem> e) {
			LoadItems(e.Items);
		}
		protected virtual void OnBoundsCalculated(object sender, BoundsCalculatedEventArgs e) {
		}
		protected internal void PrepareDataLoading() {
			AreItemsLoaded = false;
			NotifyDataChanged(MapUpdateType.Render);
		}
		public virtual void Load() {
			PrepareDataLoading();
		}
	}
}
