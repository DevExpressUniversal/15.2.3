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
using System.Windows;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet;
using InnerCapabilitiesOptions = DevExpress.XtraSpreadsheet.WorkbookCapabilitiesOptions;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetCapabilitiesOptions
	public class SpreadsheetCapabilitiesOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty UndoProperty;
		public static readonly DependencyProperty PicturesProperty;
		public static readonly DependencyProperty ChartsProperty;
		InnerCapabilitiesOptions source;
		#endregion
		static SpreadsheetCapabilitiesOptions() {
			Type ownerType = typeof(SpreadsheetCapabilitiesOptions);
			Type capabilityType = typeof(DocumentCapability);
			UndoProperty = DependencyProperty.Register("Undo", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			PicturesProperty = DependencyProperty.Register("Pictures", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ChartsProperty = DependencyProperty.Register("Charts", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Undo {
			get { return (DocumentCapability)GetValue(UndoProperty); }
			set { SetValue(UndoProperty, value); }
		}
		public DocumentCapability Pictures {
			get { return (DocumentCapability)GetValue(PicturesProperty); }
			set { SetValue(PicturesProperty, value); }
		}
		public DocumentCapability Charts {
			get { return (DocumentCapability)GetValue(ChartsProperty); }
			set { SetValue(ChartsProperty, value); }
		}
		#endregion
		internal void SetSource(InnerCapabilitiesOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Undo != (DocumentCapability)GetDefaultValue(UndoProperty))
				source.Undo = Undo;
			if (Pictures != (DocumentCapability)GetDefaultValue(PicturesProperty))
				source.Pictures = Pictures;
			if (Charts != (DocumentCapability)GetDefaultValue(ChartsProperty))
				source.Charts = Charts;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindUndoProperty();
			BindPicturesProperty();
			BindChartsProperty();
		}
		void BindUndoProperty() {
			Binding bind = new Binding("Undo") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UndoProperty, bind);
		}
		void BindPicturesProperty() {
			Binding bind = new Binding("Pictures") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, PicturesProperty, bind);
		}
		void BindChartsProperty() {
			Binding bind = new Binding("Charts") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ChartsProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
