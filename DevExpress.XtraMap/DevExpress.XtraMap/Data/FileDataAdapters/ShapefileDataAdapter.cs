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
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class ShapefileDataAdapter : FileDataAdapterBase, ISupportBoundingRectAdapter {
		#region nested classes
		protected interface IPrjContentProvider {
			string GetContent(Uri uri);
		}
		protected class PrjContentProvider : IPrjContentProvider {
			string IPrjContentProvider.GetContent(Uri uri) {
				StreamReader prjStream = null;
				try {
					if(uri.ToString().ToLower().Contains(@"file://")) {
						if(!File.Exists(uri.LocalPath))
							return string.Empty;
						prjStream = new StreamReader(uri.LocalPath);
						return prjStream.ReadToEnd();
					} else
						return string.Empty;
				} catch {
					return string.Empty;
				} finally {
					if(prjStream != null)
						prjStream.Dispose();
				}
			}
		}
		#endregion
		static IPrjContentProvider prjContentProvider;
		Encoding defaultEncoding = Encoding.Default;
		CoordBounds boundingRect = CoordBounds.Empty;
		SourceCoordinateSystem actualCoordinateSystem;
		CoordBounds ISupportBoundingRectAdapter.BoundingRect { get { return boundingRect; } } 
		protected new WinShapeFileLoader InnerLoader { get { return (WinShapeFileLoader)base.InnerLoader; } }
		[Category(SRCategoryNames.Data),
		TypeConverter("DevExpress.XtraMap.Design.EncodingTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapefileDataAdapterDefaultEncoding"),
#endif
]
		public Encoding DefaultEncoding {
			get { return defaultEncoding; }
			set {
				if (Object.Equals(value, defaultEncoding))
					return;
				defaultEncoding = value ?? Encoding.Default;
			}
		}
		bool ShouldSerializeDefaultEncoding() { return !Object.Equals(defaultEncoding, Encoding.Default); }
		void ResetDefaultEncoding() { defaultEncoding = Encoding.Default; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapefileDataAdapterFileUri"),
#endif
		Category(SRCategoryNames.Data),
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.ShapefileUrlEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public new Uri FileUri {
			get { return base.FileUri; }
			set { base.FileUri = value; }
		}
		#region static
		static SourceCoordinateSystem WrapCoreCoordinateSystem(CoordSystemCore coordSystem) {
			CartesianCoordSystemCore cartesianCS = coordSystem as CartesianCoordSystemCore;
			if(cartesianCS != null)
				return new CartesianSourceCoordinateSystem(cartesianCS);
			return new GeoSourceCoordinateSystem(coordSystem as GeoCoordSystemCore);
		}
		internal static SourceCoordinateSystem LoadPrjFile(string content) {
			if(!string.IsNullOrEmpty(content)) {
				ProjectionFileParser parser = new ProjectionFileParser(GeoPointFactory.Instance, CartesianPointFactory.Instance);
				CoordSystemCore coordSystem = parser.Parse(content);
				return WrapCoreCoordinateSystem(coordSystem);
			}
			return null;
		}
		protected static void SetPrjContentProvider(IPrjContentProvider provider) {
			prjContentProvider = provider;
		}
		public static SourceCoordinateSystem LoadPrjFile(Uri prjFileUri) {
			string content = prjContentProvider.GetContent(prjFileUri);
			return LoadPrjFile(content);
		}
		#endregion
		public ShapefileDataAdapter() {
			SetPrjContentProvider();
		}
		protected virtual void SetPrjContentProvider() {
			SetPrjContentProvider(new PrjContentProvider());
		}
		protected virtual SourceCoordinateSystem LoadPrjCoordinateSystem(Uri prjFileUri) {
			return LoadPrjFile(prjFileUri);
		}
		Uri GetPrjUri(Uri uri) {
			string text = uri != null ? uri.ToString().ToLower() : string.Empty;
			string ext = Path.GetExtension(text);
			return new Uri(string.IsNullOrEmpty(ext) ? text + ".prj" : text.Replace(ext, ".prj"), UriKind.RelativeOrAbsolute);
		}
		protected override void PrepareInnerLoader() {
			base.PrepareInnerLoader();
			InnerLoader.CoordinateSystem = GetActualCoordinateSystem().CoordSystemCore;
			InnerLoader.DefaultEncoding = DefaultEncoding;
		}
		protected override MapLoaderCore<MapItem> CreateInnerLoader() {
			return new WinShapeFileLoader();
		}
		protected override void OnBoundsCalculated(object sender, BoundsCalculatedEventArgs e) {
			base.OnBoundsCalculated(sender, e);
			UpdateBoundingRect(e.Bounds);
		}
		protected internal override void OnPropertyChanged() {
			base.OnPropertyChanged();
			this.actualCoordinateSystem = null;
		}
		protected internal override SourceCoordinateSystem GetActualCoordinateSystem() {
			if(actualCoordinateSystem == null)
				actualCoordinateSystem = GetActualCoordSystemCore();
			return actualCoordinateSystem;
		}
		SourceCoordinateSystem GetActualCoordSystemCore() {
			if(!SourceCoordinateSystem.IsDefault)
				return SourceCoordinateSystem;
			if(FileUri == null)
				return DefaultSourceCoordinateSystem;
			SourceCoordinateSystem sourceCS = LoadPrjCoordinateSystem(GetPrjUri(FileUri));
			return sourceCS ?? DefaultSourceCoordinateSystem;
		}
		protected internal void UpdateBoundingRect(CoordBounds rect) {
			this.boundingRect = rect;
			if(Layer != null)
				Layer.UpdateBoundingRect();
		}
		public void LoadFromStream(Stream shpStream, Stream dbfStream) {
			if (MapUtils.IsEmptyStream(shpStream))
				return;
			UriChecked = UriExists = true;
			PrepareInnerLoader();
			InnerLoader.Load(shpStream, dbfStream);
			PrepareDataLoading();
		}
		public override string ToString() {
			return "(ShapefileDataAdapter)";
		}
		#region Obsolete members
		[
		Obsolete("The parameterless LoadPrjFile method is obsolete now. Use the static LoadPrjFile with the Uri argument method instead"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public SourceCoordinateSystem LoadPrjFile() {
			Uri prjUri = GetPrjUri(FileUri);
			return LoadPrjCoordinateSystem(prjUri);
		}
		#endregion
	}
}
