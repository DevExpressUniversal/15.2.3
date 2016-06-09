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
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Design.Internal;
using System.Windows;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
namespace DevExpress.Xpf.RichEdit.UI {
	#region TableSizeControl
	public partial class TableSizeControl : UserControl, IBatchUpdateable, IBatchUpdateHandler, IDisposable {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		TableSizeProperties properties;
		#endregion
		public TableSizeControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new TableSizeProperties();
			InitializeComponent();
			Loaded += OnLoaded;
			SubscribeEvents();
		}
		#region Properties
		public TableSizeProperties Properties { get { return properties; } }
		#endregion
		#region Events
		#region TableSizeControlChanged
		public event EventHandler TableSizeControlChanged;
		protected internal virtual void RaiseTableSizeControlChanged() {
			if (TableSizeControlChanged != null)
				TableSizeControlChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateControl();
		}
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.chkPreferredWidth.EditValueChanged += OnPreferredWidthChanged;
			this.spnPreferredWidth.ValueChanged += OnSpnPreferredWidthValueChanged;
			this.cbWidthType.EditValueChanged += OnWidthTypeChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.chkPreferredWidth.EditValueChanged -= OnPreferredWidthChanged;
			this.spnPreferredWidth.ValueChanged -= OnSpnPreferredWidthValueChanged;
			this.cbWidthType.EditValueChanged -= OnWidthTypeChanged;
		}
		void UpdateControl() {
			UnsubscribeEvents();
			try {
				this.spnPreferredWidth.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnPreferredWidth.MinValue = Properties.MinValue;
				this.spnPreferredWidth.MaxValue = Properties.MaxValue;
				DocumentUnit defaultUnitType = Properties.UnitType;
				this.spnPreferredWidth.DefaultUnitType = defaultUnitType;
				this.cbWidthType.UnitType = defaultUnitType;
				bool? useDefaultWidth = Properties.UseDefaultValue;
				bool enabledControls = useDefaultWidth.HasValue && !useDefaultWidth.Value;
				UpdateEnabledControls(enabledControls);
				UpdatePreferredWidthCheckEdit();
				WidthUnitType? widthType = Properties.WidthType;
				WidthUnitType actualWidhtUnitType = (widthType.HasValue && widthType.Value == WidthUnitType.FiftiethsOfPercent) ? widthType.Value : WidthUnitType.ModelUnits;
				this.spnPreferredWidth.IsValueInPercent = actualWidhtUnitType == WidthUnitType.FiftiethsOfPercent;
				this.spnPreferredWidth.Value = Properties.Width;
				this.cbWidthType.Value = actualWidhtUnitType;
			} finally {
				SubscribeEvents();
			}
		}
		void UpdateEnabledControls(bool enabled) {
			this.spnPreferredWidth.IsEnabled = enabled;
			this.cbWidthType.IsEnabled = enabled;
		}
		protected internal virtual void UpdatePreferredWidthCheckEdit() {
			bool? check = !Properties.UseDefaultValue;
			if (!check.HasValue)
				chkPreferredWidth.IsThreeState = true;
			chkPreferredWidth.IsChecked = check;
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnTableSizeControlChanged();
		}
		void OnTableSizeControlChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseTableSizeControlChanged();
		}
		void OnPreferredWidthChanged(object sender, EventArgs e) {
			bool? check = chkPreferredWidth.IsChecked;
			Properties.UseDefaultValue = !check;
			bool enabledControls = check.HasValue && check.Value;
			UpdateEnabledControls(enabledControls);
		}
		void OnSpnPreferredWidthValueChanged(object sender, EventArgs e) {
			Properties.Width = spnPreferredWidth.Value;
		}
		void OnWidthTypeChanged(object sender, EventArgs e) {
			BeginUpdate();
			Properties.WidthType = cbWidthType.Value;
			Properties.Width = CalculateActualWidth();
			EndUpdate();
		}
		int? CalculateActualWidth() {
			WidthUnitType? widthType = Properties.WidthType;
			if (!widthType.HasValue)
				return null;
			if (widthType.Value == WidthUnitType.FiftiethsOfPercent)
				return CalculateValueInPercent();
			else
				return CalculateValueInModelUnits();
		}
		protected internal virtual int? CalculateValueInPercent() {
			int? width = Properties.Width;
			int valueForPercent = Properties.ValueForPercent;
			if (!width.HasValue || valueForPercent == 0)
				return 0;
			return width.Value * 100 / valueForPercent;
		}
		protected internal virtual int? CalculateValueInModelUnits() {
			int? width = Properties.Width;
			if (!width.HasValue)
				return 0;
			return Properties.ValueForPercent * width.Value / 100;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~TableSizeControl() {
			Dispose(false);
		}
		#endregion
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			Properties.BeginUpdate();
			this.batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			Properties.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
			Properties.EndUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			UnsubscribeEvents();
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			SubscribeEvents();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseTableSizeControlChanged();
			SubscribeEvents();
		}
		#endregion
	}
	#endregion
}
