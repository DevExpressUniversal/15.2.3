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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region IDocumentModelObject
	public interface IDocumentModelObject {
		IDocumentModelPart DocumentModelPart { get;set; }
	}
	#endregion
	#region ModelObjectUndoableCollection<T>
	public class ModelObjectUndoableCollection<T> : UndoableCollection<T>
		where T : IDocumentModelObject {
		public ModelObjectUndoableCollection(IDocumentModelPart documentModelPart) 
			:base(documentModelPart) {
		}
		public override int AddCore(T item) {
			item.DocumentModelPart = DocumentModelPart;
			return base.AddCore(item);
		}
		public override void AddRangeCore(IEnumerable<T> collection) {
			foreach (T item in collection)
				item.DocumentModelPart = DocumentModelPart;
			base.AddRangeCore(collection);
		}
		protected override void InsertCore(int index, T item) {
			item.DocumentModelPart = DocumentModelPart;
			base.InsertCore(index, item);
		}
		public override void RemoveAtCore(int index) {
			this[index].DocumentModelPart = null;
			base.RemoveAtCore(index);
		}
		public override void ClearCore() {
			this.ForEach(i => i.DocumentModelPart = null);
			base.ClearCore();
		}
	}
	#endregion
	#region ModelShapeCustomGeometry
	public class ModelShapeCustomGeometry : ICloneable<ModelShapeCustomGeometry>, ISupportsCopyFrom<ModelShapeCustomGeometry>  {
		#region Fields
		IDocumentModelPart documentModelPart;
		#endregion
		public ModelAdjustHandlesList AdjustHandles { get; private set; }
		public ModelShapeGuideList AdjustValues { get; private set; }
		public ModelShapeConnectionList ConnectionSites { get; private set; }
		public ModelShapeGuideList Guides { get; private set; }
		public ModelShapePathsList Paths { get; private set; }
		public AdjustableRect ShapeTextRectangle { get; private set; }
		public ModelShapeCustomGeometry(IDocumentModelPart documentModelPart) {
			this.documentModelPart = documentModelPart;
			AdjustHandles = new ModelAdjustHandlesList(documentModelPart);
			AdjustValues = new ModelShapeGuideList(documentModelPart);
			ConnectionSites = new ModelShapeConnectionList(documentModelPart);
			Guides = new ModelShapeGuideList(documentModelPart);
			Paths = new ModelShapePathsList(documentModelPart);
			ShapeTextRectangle = new ModelAdjustableRect(documentModelPart);
		}
		#region ICloneable<ModelShapeCustomGeometry> Members
		public ModelShapeCustomGeometry Clone() {
			ModelShapeCustomGeometry result = new ModelShapeCustomGeometry(documentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeCustomGeometry> Members
		public void CopyFrom(ModelShapeCustomGeometry value) {
			Guard.ArgumentNotNull(value, "ModelShapeCustomGeometry");
			this.AdjustHandles.CopyFrom(value.AdjustHandles);
			this.AdjustValues.CopyFrom(value.AdjustValues);
			this.ConnectionSites.CopyFrom(value.ConnectionSites);
			this.Guides.CopyFrom(value.Guides);
			this.Paths.CopyFrom(value.Paths);
			this.ShapeTextRectangle.CopyFrom(value.ShapeTextRectangle);
		}
		#endregion
	}
	#endregion
	#region AdjustablePoint
	public class AdjustablePoint : IDocumentModelObject, ICloneable<AdjustablePoint>, ISupportsCopyFrom<AdjustablePoint> {
		#region Fields
		AdjustableCoordinate x;
		AdjustableCoordinate y;
		#endregion
		#region Properties
		public AdjustableCoordinate X {
			get { return x; }
			set {
				AdjustableCoordinate oldValue = this.X;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetX(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetX);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetX(AdjustableCoordinate x){
			this.x = x;
		} 
		public AdjustableCoordinate Y {
			get { return y; }
			set {
				AdjustableCoordinate oldValue = this.Y;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetY(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetY);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			} 
		}
		void SetY(AdjustableCoordinate y) {
			this.y = y;
		} 
		#endregion
		public AdjustablePoint() {
		}
		public AdjustablePoint(string x, string y) {
			X = AdjustableCoordinate.FromString(x);
			Y = AdjustableCoordinate.FromString(y);
		}
		#region static
		public static AdjustablePoint Clone(AdjustablePoint other) {
			if (other == null) return null;
			return other.Clone();
		}
		#endregion
		#region Overrides of Object
		public override bool Equals(object obj) {
			AdjustablePoint other = obj as AdjustablePoint;
			if (other == null) return false;
			return this.X == other.X && this.Y == other.Y;
		}
		public override int GetHashCode() {
			int result = base.GetHashCode();
			if(this.X != null) result ^= this.X.GetHashCode();
			if(this.Y != null) result ^= this.Y.GetHashCode();
			return result;
		}
		#endregion
		#region ICloneable<AdjustablePoint> Members
		public AdjustablePoint Clone() {
			AdjustablePoint result = new AdjustablePoint();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<AdjustablePoint> Members
		public void CopyFrom(AdjustablePoint value) {
			Guard.ArgumentNotNull(value, "AdjustablePoint");
			this.X = value.X;
			this.Y = value.Y;
		}
		#endregion
		#region IDocumentModelObject Members
		public IDocumentModelPart DocumentModelPart { get; set; }
		#endregion
	}
	#endregion
	#region PolarAdjustHandle
	public class PolarAdjustHandle : AdjustablePoint {
		#region Fields
		string angleGuide;
		string radialGuide;
		AdjustableAngle maximumAngle;
		AdjustableCoordinate maximumRadial;
		AdjustableAngle minimumAngle;
		AdjustableCoordinate minimumRadial;
		#endregion
		#region Properties
		public string AngleGuide {
			get { return angleGuide; }
			set {
				string oldValue = AngleGuide;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetAngleGuide(value);
				}
				else {
					DelegateHistoryItem<string> historyItem = new DelegateHistoryItem<string>(DocumentModelPart, oldValue, value, SetAngleGuide);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetAngleGuide(string angleGuide) {
			this.angleGuide = angleGuide;
		}
		public string RadialGuide {
			get { return radialGuide; }
			set {
				string oldValue = RadialGuide;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetRadialGuide(value);
				}
				else {
					DelegateHistoryItem<string> historyItem = new DelegateHistoryItem<string>(DocumentModelPart, oldValue, value, SetRadialGuide);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetRadialGuide(string radialGuide) {
			this.radialGuide = radialGuide;
		}
		public AdjustableAngle MaximumAngle {
			get { return maximumAngle; }
			set {
				AdjustableAngle oldValue = MaximumAngle;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMaximumAngle(value);
				}
				else {
					DelegateHistoryItem<AdjustableAngle> historyItem = new DelegateHistoryItem<AdjustableAngle>(DocumentModelPart, oldValue, value, SetMaximumAngle);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMaximumAngle(AdjustableAngle maximumAngle) {
			this.maximumAngle = maximumAngle;
		}
		public AdjustableCoordinate MaximumRadial {
			get { return maximumRadial; }
			set {
				AdjustableCoordinate oldValue = MaximumRadial;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMaximumRadial(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetMaximumRadial);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMaximumRadial(AdjustableCoordinate maximumRadial) {
			this.maximumRadial = maximumRadial;
		}
		public AdjustableAngle MinimumAngle {
			get { return minimumAngle; }
			set {
				AdjustableAngle oldValue = MinimumAngle;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMinimumAngle(value);
				}
				else {
					DelegateHistoryItem<AdjustableAngle> historyItem = new DelegateHistoryItem<AdjustableAngle>(DocumentModelPart, oldValue, value, SetMinimumAngle);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMinimumAngle(AdjustableAngle minimumAngle) {
			this.minimumAngle = minimumAngle;
		}
		public AdjustableCoordinate MinimumRadial {
			get { return minimumRadial; }
			set {
				AdjustableCoordinate oldValue = MinimumRadial;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMinimumRadial(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetMinimumRadial);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMinimumRadial(AdjustableCoordinate minimumRadial) {
			this.minimumRadial = minimumRadial;
		}
		#endregion
		public PolarAdjustHandle(string angleGuide, string minAng, string maxAng, string radialGuide, string minR, string maxR, string x, string y) {
			AngleGuide = angleGuide;
			RadialGuide = radialGuide;
			MaximumAngle = AdjustableAngle.FromString(maxAng);
			MaximumRadial = AdjustableCoordinate.FromString(maxR);
			MinimumAngle = AdjustableAngle.FromString(minAng);
			MinimumRadial = AdjustableCoordinate.FromString(minR);
			X = AdjustableCoordinate.FromString(x);
			Y = AdjustableCoordinate.FromString(y);
		}
		public PolarAdjustHandle() {
		}
	}
	#endregion
	#region XYAdjustHandle
	public class XYAdjustHandle : AdjustablePoint {
		#region Fields
		string horizontalGuide;
		string verticalGuide;
		AdjustableCoordinate minX;
		AdjustableCoordinate maxX;
		AdjustableCoordinate minY;
		AdjustableCoordinate maxY;
		#endregion
		#region Properties
		public string HorizontalGuide {
			get { return horizontalGuide; }
			set {
				string oldValue = HorizontalGuide;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetHorizontalGuide(value);
				}
				else {
					DelegateHistoryItem<string> historyItem = new DelegateHistoryItem<string>(DocumentModelPart, oldValue, value, SetHorizontalGuide);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetHorizontalGuide(string horizontalGuide) {
			this.horizontalGuide = horizontalGuide;
		}
		public string VerticalGuide {
			get { return verticalGuide; }
			set {
				string oldValue = VerticalGuide;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetVerticalGuide(value);
				}
				else {
					DelegateHistoryItem<string> historyItem = new DelegateHistoryItem<string>(DocumentModelPart, oldValue, value, SetVerticalGuide);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetVerticalGuide(string verticalGuide) {
			this.verticalGuide = verticalGuide;
		}
		public AdjustableCoordinate MinX {
			get { return minX; }
			set {
				AdjustableCoordinate oldValue = MinX;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMinX(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetMinX);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMinX(AdjustableCoordinate minX) {
			this.minX = minX;
		}
		public AdjustableCoordinate MaxX {
			get { return maxX; }
			set {
				AdjustableCoordinate oldValue = MaxX;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMaxX(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetMaxX);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMaxX(AdjustableCoordinate maxX) {
			this.maxX = maxX;
		}
		public AdjustableCoordinate MinY {
			get { return minY; }
			set {
				AdjustableCoordinate oldValue = MinY;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMinY(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetMinY);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMinY(AdjustableCoordinate minY) {
			this.minY = minY;
		}
		public AdjustableCoordinate MaxY {
			get { return maxY; }
			set {
				AdjustableCoordinate oldValue = MaxY;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetMaxY(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetMaxY);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetMaxY(AdjustableCoordinate maxY) {
			this.maxY = maxY;
		}
		#endregion
		public XYAdjustHandle() {
		}
		public XYAdjustHandle(string gdRefX, string minX, string maxX, string gdRefY, string minY, string maxY, string x, string y) {
			HorizontalGuide = gdRefX;
			VerticalGuide = gdRefY;
			MinX = AdjustableCoordinate.FromString(minX);
			MaxX = AdjustableCoordinate.FromString(maxX);
			MinY = AdjustableCoordinate.FromString(minY);
			MaxY = AdjustableCoordinate.FromString(maxY);
			X = AdjustableCoordinate.FromString(x);
			Y = AdjustableCoordinate.FromString(y);
		}
	}
	#endregion
	#region AdjustHandles
	public class ModelAdjustHandlesList : ModelObjectUndoableCollection<AdjustablePoint>, ICloneable<ModelAdjustHandlesList>, ISupportsCopyFrom<ModelAdjustHandlesList>  {
		public ModelAdjustHandlesList(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region ICloneable<ModelAdjustHandlesList> Members
		public ModelAdjustHandlesList Clone() {
			ModelAdjustHandlesList result = new ModelAdjustHandlesList(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelAdjustHandlesList> Members
		public void CopyFrom(ModelAdjustHandlesList value) {
			Guard.ArgumentNotNull(value, "ModelAdjustHandlesList");
			this.Clear();
			foreach (AdjustablePoint v in value) {
				this.Add(v.Clone());
			}
		}
		#endregion
	}
	#endregion
	#region AdjustableAngle
	public class AdjustableAngle {
		#region Properties
		public int Value { get; private set; }
		public string GuideName { get; private set; }
		public bool Constant { get { return String.IsNullOrEmpty(GuideName); } }
		#endregion
		#region Static
		public static AdjustableAngle FromString(string value) {
			if(String.IsNullOrEmpty(value))
				return null;
			AdjustableAngle result = new AdjustableAngle();
			int numericValue;
			if(int.TryParse(value, out numericValue))
				result.Value = numericValue;
			else
				result.GuideName = value;
			return result;
		}
		#endregion
		#region Overrides of Object
		public override string ToString() {
			if(!String.IsNullOrEmpty(GuideName))
				return GuideName;
			return Value.ToString();
		}
		public override bool Equals(object obj) {
			AdjustableAngle other = obj as AdjustableAngle;
			if (other == null) return false;
			return this.Value == other.Value && string.Equals(this.GuideName, other.GuideName);
		}
		public override int GetHashCode() {
			int result = this.Value.GetHashCode();
			if (this.GuideName != null)
				result ^= this.GuideName.GetHashCode();
			return result;
		}
		#endregion
#if !DXPORTABLE
		public double Evaluate(ShapeGuideCalculator calculator) {
			return Constant ? Value : calculator.GetGuideValue(GuideName);
		}
#endif
	}
	#endregion
	#region AdjustableCoordinate
	public class AdjustableCoordinate {
		public long ValueEMU { get; private set; } 
		public string GuideName { get; private set; }
		public bool Constant { get { return String.IsNullOrEmpty(GuideName); } }
		#region Static
		public static AdjustableCoordinate FromString(string value) {
			if(String.IsNullOrEmpty(value))
				return null;
			AdjustableCoordinate result = new AdjustableCoordinate();
			long numericValue;
			if(long.TryParse(value, out numericValue))
				result.ValueEMU = numericValue;
			else
				result.GuideName = value;
			return result;
		}
		#endregion
		#region Overrides of Object
		public override string ToString() {
			if(!String.IsNullOrEmpty(GuideName))
				return GuideName;
			return ValueEMU.ToString();
		}
		public override bool Equals(object obj) {
			AdjustableCoordinate other = obj as AdjustableCoordinate;
			if (other == null) return false;
			return this.ValueEMU == other.ValueEMU && string.Equals(this.GuideName, other.GuideName);
		}
		public override int GetHashCode() {
			int result = this.ValueEMU.GetHashCode();
			if (this.GuideName != null)
				result ^= this.GuideName.GetHashCode();
			return result;
		}
		#endregion
#if !DXPORTABLE
		public double Evaluate(ShapeGuideCalculator calculator) {
			return Constant ? ValueEMU : calculator.GetGuideValue(GuideName);
		}
#endif
	}
	#endregion
	#region ModelShapeGuide
	public class ModelShapeGuide : IDocumentModelObject, ICloneable<ModelShapeGuide>, ISupportsCopyFrom<ModelShapeGuide> {
		#region Fields
		string formula;
		string name;
		#endregion
		#region Properties
		public string Formula { 
			get { return formula; }
			set {
				string oldValue = Formula;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetFormula(value);
				}
				else {
					DelegateHistoryItem<string> historyItem = new DelegateHistoryItem<string>(DocumentModelPart, oldValue, value, SetFormula);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetFormula(string formula) {
			this.formula = formula;
		}
		public string Name {
			get { return name; }
			set {
				string oldValue = Name;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetName(value);
				}
				else {
					DelegateHistoryItem<string> historyItem = new DelegateHistoryItem<string>(DocumentModelPart, oldValue, value, SetName);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetName(string name) {
			this.name = name;
		}
		#endregion
		public ModelShapeGuide(){
		}
		public ModelShapeGuide(string name, string formula) {
			Name = name;
			Formula = formula;
		}
		#region ICloneable<ModelShapeGuide> Members
		public ModelShapeGuide Clone() {
			ModelShapeGuide result = new ModelShapeGuide();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeGuide> Members
		public void CopyFrom(ModelShapeGuide value) {
			Guard.ArgumentNotNull(value, "ModelShapeGuide");
			this.Formula = value.Formula;
			this.Name = value.Name;
		}
		#endregion
		#region IDocumentModelObject Members
		public IDocumentModelPart DocumentModelPart { get; set; }
		#endregion
	}
	#endregion
	#region ModelShapeGuideList
	public class ModelShapeGuideList : ModelObjectUndoableCollection<ModelShapeGuide>, ICloneable<ModelShapeGuideList>, ISupportsCopyFrom<ModelShapeGuideList>  {
		public ModelShapeGuideList(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region ICloneable<ModelShapeGuideList> Members
		public ModelShapeGuideList Clone() {
			ModelShapeGuideList result = new ModelShapeGuideList(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeGuideList> Members
		public void CopyFrom(ModelShapeGuideList value) {
			Guard.ArgumentNotNull(value, "ModelShapeGuideList");
			this.Clear();
			foreach (ModelShapeGuide v in value) {
				this.Add(v.Clone());
			}
		}
		#endregion
	}
	#endregion
	#region ModelShapeConnectionList
	public class ModelShapeConnectionList : ModelObjectUndoableCollection<ModelShapeConnection>, ICloneable<ModelShapeConnectionList>, ISupportsCopyFrom<ModelShapeConnectionList>  {
		public ModelShapeConnectionList(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region ICloneable<ModelShapeConnectionList> Members
		public ModelShapeConnectionList Clone() {
			ModelShapeConnectionList result = new ModelShapeConnectionList(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeConnectionList> Members
		public void CopyFrom(ModelShapeConnectionList value) {
			Guard.ArgumentNotNull(value, "ModelShapeConnectionList");
			this.Clear();
			foreach (ModelShapeConnection v in value) {
				this.Add(v.Clone());
			}
		}
		#endregion
	}
	#endregion
	#region ModelShapeConnection
	public class ModelShapeConnection : AdjustablePoint, ICloneable<ModelShapeConnection>, ISupportsCopyFrom<ModelShapeConnection> {
		#region Fields
		AdjustableAngle angle;
		#endregion
		#region Properties
		public AdjustableAngle Angle {
			get { return angle; }
			set {
				AdjustableAngle oldValue = Angle;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetAngle(value);
				}
				else {
					DelegateHistoryItem<AdjustableAngle> historyItem = new DelegateHistoryItem<AdjustableAngle>(DocumentModelPart, oldValue, value, SetAngle);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetAngle(AdjustableAngle angle) {
			this.angle = angle;
		}
		#endregion
		public ModelShapeConnection() {}
		public ModelShapeConnection(string angle, string x, string y) {
			Angle = AdjustableAngle.FromString(angle);
			X = AdjustableCoordinate.FromString(x);
			Y = AdjustableCoordinate.FromString(y);
		}
		#region ICloneable<ModelShapeConnection> Members
		public new ModelShapeConnection Clone() {
			ModelShapeConnection result = new ModelShapeConnection();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeConnection> Members
		public void CopyFrom(ModelShapeConnection value) {
			Guard.ArgumentNotNull(value, "ModelShapeConnection");
			base.CopyFrom(value);
			this.Angle = value.Angle;
		}
		#endregion
	}
	#endregion
	#region ModelShapePathsList
	public class ModelShapePathsList : UndoableCollection<ModelShapePath>, ICloneable<ModelShapePathsList>, ISupportsCopyFrom<ModelShapePathsList>  {
		public ModelShapePathsList(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region ICloneable<ModelShapePathsList> Members
		public ModelShapePathsList Clone() {
			ModelShapePathsList result = new ModelShapePathsList(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapePath> Members
		public void CopyFrom(ModelShapePathsList value) {
			Guard.ArgumentNotNull(value, "ModelShapePathsList");
			this.Clear();
			foreach (ModelShapePath v in value) {
				this.Add(v.Clone());
			}
		}
		#endregion
	}
	#endregion
	#region PathInstructionList
	public class ModelPathInstructionList : UndoableCollection<IPathInstruction>, ICloneable<ModelPathInstructionList>, ISupportsCopyFrom<ModelPathInstructionList>  {
		public ModelPathInstructionList(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region ICloneable<ModelPathInstructionList> Members
		public ModelPathInstructionList Clone() {
			ModelPathInstructionList result = new ModelPathInstructionList(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<IPathInstructionList> Members
		public void CopyFrom(ModelPathInstructionList value) {
			Guard.ArgumentNotNull(value, "ModelPathInstructionList");
			this.Clear();
			foreach (IPathInstruction v in value) {
				this.Add(v.Clone());
			}
		}
		#endregion
	}
	#endregion
	#region ModelShapePath
	public class ModelShapePath : ICloneable<ModelShapePath>, ISupportsCopyFrom<ModelShapePath> {
		#region Fields
		IDocumentModelPart documentModelPart;
		long width;
		long height;
		PathFillMode fillMode;
		bool stroke;
		bool extrusionOK;
		#endregion
		#region Propeties
		public long Width {
			get { return width; }
			set {
				long oldValue = Width;
				if (oldValue == value) return;
				DelegateHistoryItem<long> historyItem = new DelegateHistoryItem<long>(documentModelPart, oldValue, value, SetWidth);
				documentModelPart.DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetWidth(long width) {
			this.width = width;
		}
		public long Height {
			get { return height; }
			set {
				long oldValue = Height;
				if (oldValue == value) return;
				DelegateHistoryItem<long> historyItem = new DelegateHistoryItem<long>(documentModelPart, oldValue, value, SetHeight);
				documentModelPart.DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetHeight(long height) {
			this.height = height;
		}
		public PathFillMode FillMode {
			get { return fillMode; }
			set {
				PathFillMode oldValue = fillMode;
				if (oldValue == value) return;
				DelegateHistoryItem<PathFillMode> historyItem = new DelegateHistoryItem<PathFillMode>(documentModelPart, oldValue, value, SetFillMode);
				documentModelPart.DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetFillMode(PathFillMode fillMode) {
			this.fillMode = fillMode;
		}
		public bool Stroke {
			get { return stroke; }
			set {
				bool oldValue = stroke;
				if (oldValue == value) return;
				DelegateHistoryItem<bool> historyItem = new DelegateHistoryItem<bool>(documentModelPart, oldValue, value, SetStroke);
				documentModelPart.DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetStroke(bool stroke) {
			this.stroke = stroke;
		}
		public bool ExtrusionOK {
			get { return extrusionOK; }
			set {
				bool oldValue = ExtrusionOK;
				if (oldValue == value) return;
				DelegateHistoryItem<bool> historyItem = new DelegateHistoryItem<bool>(documentModelPart, oldValue, value, SetExtrusionOK);
				documentModelPart.DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetExtrusionOK(bool extrusionOK) {
			this.extrusionOK = extrusionOK;
		}
		public IDocumentModelPart DocumentModelPart { get { return documentModelPart; } }
		public ModelPathInstructionList Instructions { get; private set; }
		#endregion
		public ModelShapePath(IDocumentModelPart documentModelPart) {
			this.documentModelPart = documentModelPart;
			SetFillMode(PathFillMode.Norm);
			SetStroke(true);
			SetExtrusionOK(true);
			Instructions = new ModelPathInstructionList(documentModelPart);
		}
		#region ICloneable<ModelShapePath> Members
		public ModelShapePath Clone() {
			ModelShapePath result = new ModelShapePath(documentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapePath> Members
		public void CopyFrom(ModelShapePath value) {
			Guard.ArgumentNotNull(value, "ModelShapePath");
			this.Width = value.Width;
			this.Height = value.Height;
			this.FillMode = value.FillMode;
			this.Stroke = value.Stroke;
			this.ExtrusionOK = value.ExtrusionOK;
			this.Instructions.CopyFrom(value.Instructions);
		}
		#endregion
	}
	#endregion
	#region FillMode
	public enum PathFillMode {
		None,
		Norm,
		Lighten,
		LightenLess,
		Darken,
		DarkenLess
	}
	#endregion
	#region IPathInstructionWalker
	public interface IPathInstructionWalker {
		void Visit(PathArc pathArc);
		void Visit(PathClose value);
		void Visit(PathCubicBezier value);
		void Visit(PathLine pathLine);
		void Visit(PathMove pathMove);
		void Visit(PathQuadraticBezier value);
	}
	#endregion
	#region IPathInstruction
	public interface IPathInstruction : ICloneable<IPathInstruction> {
		void Visit(IPathInstructionWalker visitor);
	}
	#endregion
	#region PathArc
	public class PathArc : IDocumentModelObject, IPathInstruction, ISupportsCopyFrom<PathArc> {
		#region Fields
		AdjustableCoordinate heightRadius;
		AdjustableCoordinate widthRadius;
		AdjustableAngle startAngle;
		AdjustableAngle swingAngle;
		#endregion
		#region Properties
		public AdjustableCoordinate HeightRadius {
			get { return heightRadius; }
			set {
				AdjustableCoordinate oldValue = HeightRadius;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetHeightRadius(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetHeightRadius);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetHeightRadius(AdjustableCoordinate heightRadius) {
			this.heightRadius = heightRadius;
		}
		public AdjustableCoordinate WidthRadius {
			get { return widthRadius; }
			set {
				AdjustableCoordinate oldValue = WidthRadius;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetWidthRadius(value);
				}
				else {
					DelegateHistoryItem<AdjustableCoordinate> historyItem = new DelegateHistoryItem<AdjustableCoordinate>(DocumentModelPart, oldValue, value, SetWidthRadius);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetWidthRadius(AdjustableCoordinate widthRadius) {
			this.widthRadius = widthRadius;
		}
		public AdjustableAngle StartAngle {
			get { return startAngle; }
			set {
				AdjustableAngle oldValue = StartAngle;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetStartAngle(value);
				}
				else {
					DelegateHistoryItem<AdjustableAngle> historyItem = new DelegateHistoryItem<AdjustableAngle>(DocumentModelPart, oldValue, value, SetStartAngle);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetStartAngle(AdjustableAngle startAngle) {
			this.startAngle = startAngle;
		}
		public AdjustableAngle SwingAngle {
			get { return swingAngle; }
			set {
				AdjustableAngle oldValue = SwingAngle;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetSwingAngle(value);
				}
				else {
					DelegateHistoryItem<AdjustableAngle> historyItem = new DelegateHistoryItem<AdjustableAngle>(DocumentModelPart, oldValue, value, SetSwingAngle);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		void SetSwingAngle(AdjustableAngle swingAngle) {
			this.swingAngle = swingAngle;
		}
		#endregion
		public PathArc() {
		}
		public PathArc(string widthRadius, string heightRadius, string startAngle, string swingAngle) {
			HeightRadius = AdjustableCoordinate.FromString(heightRadius);
			WidthRadius = AdjustableCoordinate.FromString(widthRadius);
			StartAngle = AdjustableAngle.FromString(startAngle);
			SwingAngle = AdjustableAngle.FromString(swingAngle);
		}
		#region Implementation of IPathInstruction
		public void Visit(IPathInstructionWalker visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ISupportsCopyFrom<PathArc> Members
		public void CopyFrom(PathArc value) {
			Guard.ArgumentNotNull(value, "PathArc");
			this.HeightRadius = value.HeightRadius;
			this.WidthRadius = value.WidthRadius;
			this.StartAngle = value.StartAngle;
			this.SwingAngle = value.SwingAngle;
		}
		#endregion
		#region ICloneable<IPathInstruction> Members
		public IPathInstruction Clone() {
			PathArc result = new PathArc();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region IDocumentModelObject Members
		public IDocumentModelPart DocumentModelPart { get; set; }
		#endregion
	}
	#endregion
	#region PathClose
	public class PathClose : IPathInstruction {
		#region Implementation of IPathInstruction
		public void Visit(IPathInstructionWalker visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<IPathInstruction> Members
		public IPathInstruction Clone() {
			return new PathClose();
		}
		#endregion
	}
	#endregion
	#region AdjustablePointsList
	public class ModelAdjustablePointsList : UndoableCollection<AdjustablePoint>, ICloneable<ModelAdjustablePointsList>, ISupportsCopyFrom<ModelAdjustablePointsList> {
		public ModelAdjustablePointsList(IDocumentModelPart documentModelPart) 
			: base(documentModelPart) {
		}
		#region ICloneable<ModelAdjustablePointsList> Members
		public ModelAdjustablePointsList Clone() {
			ModelAdjustablePointsList result = new ModelAdjustablePointsList(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelAdjustablePointsList> Members
		public void CopyFrom(ModelAdjustablePointsList value) {
			Guard.ArgumentNotNull(value, "AdjustablePointsList");
			this.Clear();
			foreach (AdjustablePoint v in value) {
				this.Add(v.Clone());
			}
		}
		#endregion
	}
	#endregion
	#region PathCubicBezier
	public class PathCubicBezier : IDocumentModelObject, IPathInstruction, ISupportsCopyFrom<PathCubicBezier> {
		#region
		IDocumentModelPart documentModelPart;
		#endregion
		#region Properties
		public ModelAdjustablePointsList Points { get; private set; }
		#endregion
		public PathCubicBezier(IDocumentModelPart documentModelPart) {
			this.documentModelPart = documentModelPart;
			Points = new ModelAdjustablePointsList(documentModelPart);
		}
		public PathCubicBezier(IDocumentModelPart documentModelPart, string x1, string y1, string x2, string y2, string x3, string y3)
			: this(documentModelPart) {
			Points.AddCore(new AdjustablePoint(x1, y1));
			Points.AddCore(new AdjustablePoint(x2, y2));
			Points.AddCore(new AdjustablePoint(x3, y3));
		}
		#region Implementation of IPathInstruction
		public void Visit(IPathInstructionWalker visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<IPathInstruction> Members
		public IPathInstruction Clone() {
			PathCubicBezier result = new PathCubicBezier(documentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PathCubicBezier> Members
		public void CopyFrom(PathCubicBezier value) {
			Guard.ArgumentNotNull(value, "PathCubicBezier");
			this.Points.CopyFrom(value.Points);
		}
		#endregion
		#region IDocumentModelObject Members
		public IDocumentModelPart DocumentModelPart { get; set; }
		#endregion
	}
	#endregion
	#region PathLine
	public class PathLine : IDocumentModelObject, IPathInstruction, ISupportsCopyFrom<PathLine> {
		#region Fields
		AdjustablePoint point;
		#endregion
		#region Properties
		public AdjustablePoint Point {
			get { return point; }
			set {
				AdjustablePoint oldValue = Point;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetPoint(value);
				}
				else {
					DelegateHistoryItem<AdjustablePoint> historyItem = new DelegateHistoryItem<AdjustablePoint>(DocumentModelPart, oldValue, value, SetPoint);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			} 
		}
		void SetPoint(AdjustablePoint point) {
			this.point = point;
		}
		#endregion
		public PathLine() {
		}
		public PathLine(string x, string y) {
			Point = new AdjustablePoint();
			Point.X = AdjustableCoordinate.FromString(x);
			Point.Y = AdjustableCoordinate.FromString(y);			
		}
		#region Implementation of IPathInstruction
		public void Visit(IPathInstructionWalker visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<IPathInstruction> Members
		public IPathInstruction Clone() {
			PathLine result = new PathLine();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PathLine> Members
		public void CopyFrom(PathLine value) {
			Guard.ArgumentNotNull(value, "PathLine");
			this.Point = AdjustablePoint.Clone(value.Point);
		}
		#endregion
		#region IDocumentModelObject Members
		public IDocumentModelPart DocumentModelPart { get; set; }
		#endregion
	}
	#endregion
	#region PathMove
	public class PathMove : IDocumentModelObject, IPathInstruction, ISupportsCopyFrom<PathMove> {
		#region Fields
		AdjustablePoint point;
		#endregion
		#region Properties
		public AdjustablePoint Point {
			get { return point; }
			set {
				AdjustablePoint oldValue = Point;
				if (oldValue == value) return;
				if (DocumentModelPart == null) {
					SetPoint(value);
				}
				else {
					DelegateHistoryItem<AdjustablePoint> historyItem = new DelegateHistoryItem<AdjustablePoint>(DocumentModelPart, oldValue, value, SetPoint);
					DocumentModelPart.DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			} 
		}
		void SetPoint(AdjustablePoint point) {
			this.point = point;
		}
		#endregion
		public PathMove() {
		}
		public PathMove(string x, string y) {
			Point = new AdjustablePoint();
			Point.X = AdjustableCoordinate.FromString(x);
			Point.Y = AdjustableCoordinate.FromString(y);
		}
		#region Implementation of IPathInstruction
		public void Visit(IPathInstructionWalker visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<IPathInstruction> Members
		public IPathInstruction Clone() {
			PathMove result = new PathMove();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PathMove> Members
		public void CopyFrom(PathMove value) {
			Guard.ArgumentNotNull(value, "PathMove");
			this.Point = AdjustablePoint.Clone(value.Point);
		}
		#endregion
		#region IDocumentModelObject Members
		public IDocumentModelPart DocumentModelPart { get; set; }
		#endregion
	}
	#endregion
	#region PathQuadratic
	public class PathQuadraticBezier : IPathInstruction, ISupportsCopyFrom<PathQuadraticBezier> {
		#region
		IDocumentModelPart documentModelPart;
		#endregion
		#region Properties
		public ModelAdjustablePointsList Points { get; private set; }
		#endregion
		public PathQuadraticBezier(IDocumentModelPart documentModelPart) {
			this.documentModelPart = documentModelPart;
			Points = new ModelAdjustablePointsList(documentModelPart);
		}
		public PathQuadraticBezier(IDocumentModelPart documentModelPart, string x1, string y1, string x2, string y2) 
			: this(documentModelPart) {
			Points.AddCore(new AdjustablePoint(x1, y1));
			Points.AddCore(new AdjustablePoint(x2, y2));
		}
		#region Implementation of IPathInstruction
		public void Visit(IPathInstructionWalker visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<IPathInstruction> Members
		public IPathInstruction Clone() {
			PathQuadraticBezier result = new PathQuadraticBezier(documentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PathQuadraticBezier> Members
		public void CopyFrom(PathQuadraticBezier value) {
			Guard.ArgumentNotNull(value, "PathQuadraticBezier");
			this.Points.CopyFrom(value.Points);
		}
		#endregion
	}
	#endregion
	#region ModelAdjustableRect
	public class AdjustableRect : ICloneable<AdjustableRect>, ISupportsCopyFrom<AdjustableRect> {
		#region Fields
		const int idxLeft = 0;
		const int idxRight = 1;
		const int idxTop = 2;
		const int idxBottom = 3;
		AdjustableCoordinate[] coordinates = new AdjustableCoordinate[4];
		#endregion
		#region Properties
		protected AdjustableCoordinate[] Coordinates { get { return coordinates; } }
		public AdjustableCoordinate Left {
			get { return coordinates[idxLeft]; }
			set { SetCoordinate(idxLeft, value); }
		}
		public AdjustableCoordinate Right {
			get { return coordinates[idxRight]; }
			set { SetCoordinate(idxRight, value); }
		}
		public AdjustableCoordinate Top {
			get { return coordinates[idxTop]; }
			set { SetCoordinate(idxTop, value); }
		}
		public AdjustableCoordinate Bottom {
			get { return coordinates[idxBottom]; }
			set { SetCoordinate(idxBottom, value); }
		}
		#endregion
		protected internal virtual void SetCoordinate(int index, AdjustableCoordinate value) {
			SetCoordinateCore(index, value);
		} 
		protected void SetCoordinateCore(int index, AdjustableCoordinate coordinate) {
			coordinates[index] = coordinate;
		}
		public void FromString(string left, string top, string right, string bottom) {
			coordinates[idxLeft] = AdjustableCoordinate.FromString(left);
			coordinates[idxRight] = AdjustableCoordinate.FromString(right);
			coordinates[idxTop] = AdjustableCoordinate.FromString(top);
			coordinates[idxBottom] = AdjustableCoordinate.FromString(bottom);
		}
		public bool IsEmpty() {
			return Left == null || Right == null || Top == null || Bottom == null;
		}
		#region ICloneable<AdjustableRect> Members
		public AdjustableRect Clone() {
			AdjustableRect result = new AdjustableRect();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<AdjustableRect> Members
		public void CopyFrom(AdjustableRect value) {
			Guard.ArgumentNotNull(value, "AdjustableRect");
			this.Left = value.Left;
			this.Right = value.Right;
			this.Top = value.Top;
			this.Bottom = value.Bottom;
		}
		#endregion
	}
	public class ModelAdjustableRect : AdjustableRect {
		#region Fields
		IDocumentModelPart documentModelPart;
		#endregion
		public ModelAdjustableRect(IDocumentModelPart documentModelPart) {
			this.documentModelPart = documentModelPart;
		}
		#region Properties
		public IDocumentModelPart DocumentModelPart { get { return documentModelPart; } }
		#endregion
		protected internal override void SetCoordinate(int index, AdjustableCoordinate value) {
			AdjustableCoordinate oldValue = Coordinates[index];
			if (oldValue == value) return;
			base.SetCoordinate(index, value);
			ModelAdjustableRectHistoryItem historyItem = new ModelAdjustableRectHistoryItem(this, index, oldValue, value);
			DocumentModelPart.DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
	}
	#endregion
}
