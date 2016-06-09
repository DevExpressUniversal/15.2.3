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
using System.Diagnostics;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model.History;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region IDataContainer
	public interface IDataContainer : ICloneable<IDataContainer> {
		void SetData(byte[] data);
		byte[] GetData();
		event DataContainerPropertiesChangedEventHandler PropertiesChanged;
	}
	#endregion
	#region DataContainerPropertiesChangedEventHandler
	public delegate void DataContainerPropertiesChangedEventHandler(object sender, DataContainerPropertiesChangedEventArgs e);
	#endregion
	#region DataContainerRunPropertiesChangedEventArgs
	public class DataContainerPropertiesChangedEventArgs : EventArgs {
		public IDataContainerPropertiesChangedInfo ChangedInfo { get; set; }
	}
	#endregion
	#region IDataContainerRunPropertiesChangedInfo
	public interface IDataContainerPropertiesChangedInfo {
		void Undo(IDataContainer dataContainer);
		void Redo(IDataContainer dataContainer);
	}
	#endregion
	#region DataContainerRun
	public class DataContainerRun : TextRun, IDisposable {
		protected internal static readonly string DataContainerText = new string(Characters.ObjectMark, 1);
		IDataContainer dataContainer;
		public DataContainerRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public override bool CanPlaceCaretBefore { get { return true; } }
		public override int Length { get { return 1; } set { } }
		public IDataContainer DataContainer {
			get { return dataContainer; }
			set {
				if (object.ReferenceEquals(dataContainer, value))
					return;
				UnsubscribeDataContainerEvents();
				dataContainer = value;
				SubscribeDataContainerEvents();
			}
		}
		public override bool CanJoinWith(TextRunBase nextRun) {
			return false;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			FontInfo fontInfo = DocumentModel.FontCache[FontCacheIndex];
			measurer.MeasureText(boxInfo, DataContainerText, fontInfo);
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable pieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			pieceTable.InsertDataContainerRunCore(targetPosition.ParagraphIndex, targetPosition.LogPosition, DataContainer.Clone(), false);
			TextRunBase run = pieceTable.Runs[targetPosition.RunIndex];
			run.CopyCharacterPropertiesFrom(copyManager, this);
			return run;
		}
		public void SubscribeDataContainerEvents() {
			if (DataContainer == null)
				return;
			DataContainer.PropertiesChanged += new DataContainerPropertiesChangedEventHandler(OnDataContainerPropertiesChanged);
		}
		public void UnsubscribeDataContainerEvents() {
			if (DataContainer == null)
				return;
			DataContainer.PropertiesChanged -= new DataContainerPropertiesChangedEventHandler(OnDataContainerPropertiesChanged);
		}
		void OnDataContainerPropertiesChanged(object sender, DataContainerPropertiesChangedEventArgs e) {
			RunIndex thisRunIndex = PieceTable.CalculateRunIndex(this);
			if (e.ChangedInfo != null) {
				DataContainerPropertiesChangedHistoryItem historyItem = new DataContainerPropertiesChangedHistoryItem(PieceTable, thisRunIndex, e.ChangedInfo);
				PieceTable.DocumentModel.History.Add(historyItem);
			}
	   }
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DataContainerRun() {
			Dispose(false);
		}
		protected internal virtual void Dispose(bool disposing) {
			if (disposing) {
				if (DataContainer != null) {
					UnsubscribeDataContainerEvents();
#if !SL
					IDisposable disposable = DataContainer as IDisposable;
					if (disposable != null)
						disposable.Dispose();
#endif
					DataContainer = null;
				}
			}
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Model.History {
	#region DataContainerPropertiesChangedHistoryItem
	public class DataContainerPropertiesChangedHistoryItem : RichEditHistoryItem {
		RunIndex index;
		IDataContainerPropertiesChangedInfo info;
		public DataContainerPropertiesChangedHistoryItem(PieceTable pieceTable, RunIndex index, IDataContainerPropertiesChangedInfo info)
			: base(pieceTable) {
			Guard.ArgumentNotNull(info, "info");
			Debug.Assert(index >= RunIndex.Zero);
			this.index = index;
			this.info = info;
		}
		public RunIndex Index { get { return index; } }
		protected override void UndoCore() {
			DataContainerRun run = (DataContainerRun)PieceTable.Runs[Index];
			if (run.DataContainer != null)
				this.info.Undo(run.DataContainer);
		}
		protected override void RedoCore() {
			DataContainerRun run = (DataContainerRun)PieceTable.Runs[Index];
			if (run.DataContainer != null)
				this.info.Redo(run.DataContainer);
		}
	}
	#endregion
}
