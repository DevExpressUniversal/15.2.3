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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SheetVisibleState
	public enum SheetVisibleState {
		Visible = 0,
		Hidden = 1,
		VeryHidden = 2
	}
	#endregion
	#region InternalSheetBase (abstract class)
	public abstract partial class InternalSheetBase : SheetBase, IDrawingObjectByZOrderSorter, IDrawingObjectsContainer {
		#region Fields
		SheetVisibleState visibleState;
		SheetPropertiesBase properties;
		readonly DrawingObjectsCollection drawingObjects;
		readonly DrawingObjectsByZOrderCollections drawingObjectsByZOrderCollections;
		readonly ModelWorksheetView activeView;
		#endregion
		protected InternalSheetBase(DocumentModel workbook)
			: base(workbook) {
			this.drawingObjects = new DrawingObjectsCollection(this);
			this.drawingObjectsByZOrderCollections = new DrawingObjectsByZOrderCollections(this);
			this.activeView = new ModelWorksheetView(workbook);
		}
		#region Properties
		public SheetVisibleState VisibleState {
			get { return visibleState; }
			set {
				if (visibleState == value)
					return;
				ChangeVisibleState(value);
			}
		}
		public SheetPropertiesBase Properties { get { return this.properties; } }
		public abstract bool IsSelected { get; set; }
		public virtual DrawingObjectsCollection DrawingObjects { get { return drawingObjects; } }
		public virtual DrawingObjectsByZOrderCollections DrawingObjectsByZOrderCollections { get { return drawingObjectsByZOrderCollections; } }
		public ModelWorksheetView ActiveView { get { return activeView; } }
		public virtual bool ReadOnly { get { return false; } }
		#endregion
		protected internal void SetProperties(SheetPropertiesBase properties) {
			this.properties = properties;
		}
		protected internal virtual void ChangeVisibleState(SheetVisibleState newValue) {
			this.visibleState = newValue;
		}
		public bool IsProtected { get { return properties.Protection.SheetLocked; } }
		public event EventHandler SheetProtectedChanged;
		protected void RaiseSheetProtectedChanged() {
			if (SheetProtectedChanged != null)
				SheetProtectedChanged(this, EventArgs.Empty);
		}
		public void Protect(string password) {
			if (IsProtected)
				throw new InvalidOperationException("Sheet is already protected");
			WorksheetProtectionOptions protectionOptions = Properties.Protection;
			protectionOptions.BeginUpdate();
			try {
				protectionOptions.SheetLocked = true;
				DocumentModel documentModel = Workbook as DocumentModel;
				if (documentModel != null) {
					SpreadsheetProtectionOptions options = documentModel.ProtectionOptions;
					protectionOptions.Credentials = new ProtectionCredentials(password, false, options.UseStrongPasswordVerifier, options.SpinCount);
					documentModel.InnerApplyChanges(DocumentModelChangeActions.RaiseUpdateUI);
				}
				else
					protectionOptions.Credentials = new ProtectionCredentials(password, false);
			}
			finally {
				protectionOptions.EndUpdate();
			}
			ResetProtectedRanges();
			ClearHistory();
			RaiseSheetProtectedChanged();
		}
		protected virtual void ResetProtectedRanges() { }
		public ModelErrorInfo UnProtect(string password) {
			if (!IsProtected)
				throw new InvalidOperationException("Sheet is not protected");
			WorksheetProtectionOptions protectionOptions = Properties.Protection;
			if (!protectionOptions.CheckPassword(password)) {
				return new ModelErrorInfo(ModelErrorType.IncorrectPassword);
			}
			protectionOptions.BeginUpdate();
			try {
				protectionOptions.SheetLocked = false;
				protectionOptions.Credentials = ProtectionCredentials.NoProtection;
				DocumentModel documentModel = Workbook as DocumentModel;
				if (documentModel != null)
					documentModel.InnerApplyChanges(DocumentModelChangeActions.RaiseUpdateUI);
			}
			finally {
				protectionOptions.EndUpdate();
			}
			RaiseSheetProtectedChanged();
			return null;
		}
		void ClearHistory() {
			DocumentModel workbook = (this as IDocumentModelPartWithApplyChanges).Workbook;
			if (!workbook.IsUpdateLocked)
				workbook.History.Clear();
		}
	}
	#endregion
}
