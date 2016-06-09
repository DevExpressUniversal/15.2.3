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
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Xpf.Grid.Native {
	public interface IDesignTimeAdornerBase {
		void OnColumnMoved();
		void OnColumnHeaderClick();
		void OnColumnResized();
		void OnColumnsLayoutChanged();
		void UpdateDesignTimeInfo();
		bool ForceAllowUseColumnInFilterControl { get; }
		bool IsSelectGridArea(Point point);
		DataViewBase GetDefaultView(DataControlBase dataControl);
		void InvalidateDataSource();
		bool IsDesignTime { get; }
		IBandMoveProvider BandMoveProvider { get; }
		IColumnMoveToBandProvider ColumnMoveToBandProvider { get; }
		bool SkipColumnXamlGenerationProperties { get; set; }
		void RemoveGeneratedColumns(DataControlBase dataControl);
		void UpdateVisibleIndexes(DataControlBase dataControl);
		IModelItem GetDataControlModelItem(DataControlBase dataControl);
		IModelItem CreateModelItem(object obj, IModelItem parent);
		Type GetDefaultColumnType(ColumnBase column);
	}
	public class BandMoveProvider : IBandMoveProvider {
		void IBandMoveProvider.StartMovingBand() { }
		void IBandMoveProvider.RemoveBand(BandBase band) {
			band.Owner.BandsCore.Remove(band);
		}
		void IBandMoveProvider.AddBand(IBandsOwner owner, ref BandBase band) {
			owner.BandsCore.Add(band);
		}
		void IBandMoveProvider.MoveColumns(BandBase source, BandBase target) {
			foreach(ColumnBase column in source.ColumnsCore)
				target.ColumnsCore.Add(column);
			source.ColumnsCore.Clear();
		}
		void IBandMoveProvider.EndMovingBand() { }
		void IBandMoveProvider.MoveBands(BandBase source, BandBase target) {
			foreach(var b in source.BandsCore)
				target.BandsCore.Add(b);
			source.BandsCore.Clear();
		}
		void IBandMoveProvider.SetVisibleIndex(BaseColumn source, int value) {
			source.VisibleIndex = value;
		}
	}
	public class ColumnMoveToBandProvider : IColumnMoveToBandProvider {
		void IColumnMoveToBandProvider.StartMoving() { }
		void IColumnMoveToBandProvider.EndMoving() { }
		void IColumnMoveToBandProvider.SetRow(BaseColumn column, int value) {
			BandBase.SetGridRow(column, value);
		}
		void IColumnMoveToBandProvider.SetVisibleIndex(ColumnBase column, int value) {
			column.VisibleIndex = value;
		}
		void IColumnMoveToBandProvider.MoveColumnToBand(BaseColumn column, BandBase from, BandBase target) {
			if(from != target) {
				from.ColumnsCore.Remove(column);
				target.ColumnsCore.Add(column);
			}
		}
	}
	public class EmptyDesignTimeAdornerBase : IDesignTimeAdornerBase {
		public static EmptyDesignTimeAdornerBase Instance = new EmptyDesignTimeAdornerBase();
		protected EmptyDesignTimeAdornerBase() { }
		void IDesignTimeAdornerBase.OnColumnMoved() { }
		void IDesignTimeAdornerBase.OnColumnHeaderClick() { }
		void IDesignTimeAdornerBase.OnColumnResized() { }
		void IDesignTimeAdornerBase.OnColumnsLayoutChanged() { }
		void IDesignTimeAdornerBase.UpdateDesignTimeInfo() { }
		bool IDesignTimeAdornerBase.ForceAllowUseColumnInFilterControl { get { return false; } }
		bool IDesignTimeAdornerBase.IsSelectGridArea(Point point) { return false; }
		bool IDesignTimeAdornerBase.SkipColumnXamlGenerationProperties { get { return false; } set { } }
		DataViewBase IDesignTimeAdornerBase.GetDefaultView(DataControlBase dataControl) {
			if(DesignerProperties.GetIsInDesignMode(dataControl)) {
				return dataControl.CreateDefaultView();
			}
#if DEBUGTEST
			throw new InvalidOperationException();
#else
			return null;
#endif
		}
		void IDesignTimeAdornerBase.InvalidateDataSource() { }
		bool IDesignTimeAdornerBase.IsDesignTime { get { return false; } }
		IBandMoveProvider bandMoveProvider = new BandMoveProvider();
		IBandMoveProvider IDesignTimeAdornerBase.BandMoveProvider {
			get { return bandMoveProvider; }
		}
		IColumnMoveToBandProvider columnMoveToBandProvider = new ColumnMoveToBandProvider();
		IColumnMoveToBandProvider IDesignTimeAdornerBase.ColumnMoveToBandProvider {
			get { return columnMoveToBandProvider; }
		}
		void IDesignTimeAdornerBase.RemoveGeneratedColumns(DataControlBase dataControl) { }
		void IDesignTimeAdornerBase.UpdateVisibleIndexes(DataControlBase dataControl) { }
		IModelItem IDesignTimeAdornerBase.GetDataControlModelItem(DataControlBase dataControl) {
			IEditingContext context = new RuntimeEditingContext(dataControl);
			return context.GetRoot();
		}
		IModelItem IDesignTimeAdornerBase.CreateModelItem(object obj, IModelItem parent) {
			EditingContextBase context = parent.Context as EditingContextBase;
			return context != null ? context.CreateModelItem(obj, parent) : null;
		}
		Type IDesignTimeAdornerBase.GetDefaultColumnType(ColumnBase column) {
			return typeof(object);
		}
	}
	public interface IBandMoveProvider {
		void StartMovingBand();
		void RemoveBand(BandBase band);
		void AddBand(IBandsOwner owner, ref BandBase band);
		void MoveColumns(BandBase source, BandBase target);
		void EndMovingBand();
		void MoveBands(BandBase source, BandBase target);
		void SetVisibleIndex(BaseColumn source, int value);
	}
	public interface IColumnMoveToBandProvider {
		void StartMoving();
		void EndMoving();
		void SetRow(BaseColumn column, int value);
		void SetVisibleIndex(ColumnBase column, int value);
		void MoveColumnToBand(BaseColumn column, BandBase from, BandBase target);
	}
	internal static class DesignerHelper {
		public static bool GetValue(DependencyObject source, bool currentValue, bool designerValue) {
			if(DesignerProperties.GetIsInDesignMode(source))
				return designerValue;
			return currentValue;
		}
	}
}
