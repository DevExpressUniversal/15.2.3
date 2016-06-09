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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Xpf.RichEdit.UI {
	#region TableRowHeightControl
	public partial class TableRowHeightControl : UserControl, IBatchUpdateable, IBatchUpdateHandler, IDisposable {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		TableRowHeightProperties properties;
		#endregion
		public TableRowHeightControl() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.properties = new TableRowHeightProperties();
			InitializeComponent();
			Loaded += OnLoaded;
			SubscribeEvents();
		}
		#region Properties
		public TableRowHeightProperties Properties { get { return properties; } }
		#endregion
		#region Events
		#region TableRowHeightControlChanged
		public event EventHandler TableRowHeightControlChanged;
		protected internal virtual void RaiseTableRowHeightControlChanged() {
			if (TableRowHeightControlChanged != null)
				TableRowHeightControlChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateControl();
		}
		protected internal virtual void SubscribeEvents() {
			Properties.PropertiesChanged += OnPropertiesChanged;
			this.chkSpecifyHeight.EditValueChanged += OnSpecifyHeightChanged;
			this.spnHeight.ValueChanged += OnSpnHeightValueChanged;
			this.edtRowHeightType.EditValueChanged += OnHeightTypeChanged;
		}
		protected internal virtual void UnsubscribeEvents() {
			Properties.PropertiesChanged -= OnPropertiesChanged;
			this.chkSpecifyHeight.EditValueChanged -= OnSpecifyHeightChanged;
			this.spnHeight.ValueChanged -= OnSpnHeightValueChanged;
			this.edtRowHeightType.EditValueChanged -= OnHeightTypeChanged;
		}
		void UpdateControl() {
			UnsubscribeEvents();
			try {
				this.spnHeight.ValueUnitConverter = Properties.ValueUnitConverter;
				this.spnHeight.DefaultUnitType = Properties.UnitType;
				this.spnHeight.MinValue = Properties.MinValue;
				this.spnHeight.MaxValue = Properties.MaxValue;
				bool? useDefaultHeight = Properties.UseDefaultValue;
				bool enabledControls = useDefaultHeight.HasValue && !useDefaultHeight.Value;
				UpdateEnabledControls(enabledControls);
				UpdateSpecifyHeightCheckEdit();
				this.spnHeight.Value = Properties.Height;
				HeightUnitType? heightType = Properties.HeightType;
				this.edtRowHeightType.Value = (heightType.HasValue && heightType.Value == HeightUnitType.Exact) ? heightType : HeightUnitType.Minimum;
			} finally {
				SubscribeEvents();
			}
		}
		void UpdateEnabledControls(bool enabled) {
			this.spnHeight.IsEnabled = enabled;
			this.edtRowHeightType.IsEnabled = enabled;
		}
		protected internal virtual void UpdateSpecifyHeightCheckEdit() {
			bool? check = !Properties.UseDefaultValue;
			chkSpecifyHeight.IsThreeState = check.HasValue ? false : true;
			chkSpecifyHeight.IsChecked = check;
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			UpdateControl();
			OnTableRowHeightControlChanged();
		}
		void OnTableRowHeightControlChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseTableRowHeightControlChanged();
		}
		void OnSpecifyHeightChanged(object sender, EventArgs e) {
			bool? check = chkSpecifyHeight.IsChecked;
			Properties.UseDefaultValue = !check;
			bool enabledControls = check.HasValue && check.Value;
			UpdateEnabledControls(enabledControls);
		}
		void OnSpnHeightValueChanged(object sender, EventArgs e) {
			Properties.Height = spnHeight.Value;
		}
		void OnHeightTypeChanged(object sender, EventArgs e) {
			Properties.HeightType = edtRowHeightType.Value;
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
		~TableRowHeightControl() {
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
				RaiseTableRowHeightControlChanged();
			SubscribeEvents();
		}
		#endregion
	}
	#endregion
}
