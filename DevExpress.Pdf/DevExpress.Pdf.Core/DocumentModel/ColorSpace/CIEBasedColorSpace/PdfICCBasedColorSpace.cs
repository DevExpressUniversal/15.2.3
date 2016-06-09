#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfICCBasedColorSpace : PdfCustomColorSpace {
		internal const string TypeName = "ICCBased";
		const string componentsCountDictionaryKey = "N";
		const string alternateDictionaryKey = "Alternate";
		const string rangeDictionaryKey = "Range";
		readonly int componentsCount;
		readonly PdfColorSpace alternate;
		readonly IList<PdfRange> range;
		readonly PdfMetadata metadata;
		readonly byte[] data;
		bool IsValidComponentsCount { get { return componentsCount == 1 || componentsCount == 3 || componentsCount == 4; } }
		public PdfColorSpace Alternate { get { return alternate; } }
		public IEnumerable<PdfRange> Range { get { return range; } }
		public PdfMetadata Metadata { get { return metadata; } }
		public byte[] Data { get { return data; } }
		public override int ComponentsCount { get { return componentsCount; } }
		internal PdfICCBasedColorSpace() {
			componentsCount = 3;
			alternate = CreateAlternateColorSpace();
			range = CreateRange();
			data = Convert.FromBase64String(@"
                AAAMSExpbm8CEAAAbW50clJHQiBYWVogB84AAgAJAAYAMQAAYWNzcE1TRlQAAAAASUVDIHNSR0IAAAAAAAAAAAAAAAAAAPb
                WAAEAAAAA0y1IUCAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARY3BydAAAAVAAAA
                AzZGVzYwAAAYQAAABsd3RwdAAAAfAAAAAUYmtwdAAAAgQAAAAUclhZWgAAAhgAAAAUZ1hZWgAAAiwAAAAUYlhZWgAAAkAAA
                AAUZG1uZAAAAlQAAABwZG1kZAAAAsQAAACIdnVlZAAAA0wAAACGdmlldwAAA9QAAAAkbHVtaQAAA/gAAAAUbWVhcwAABAwA
                AAAkdGVjaAAABDAAAAAMclRSQwAABDwAAAgMZ1RSQwAABDwAAAgMYlRSQwAABDwAAAgMdGV4dAAAAABDb3B5cmlnaHQgKGM
                pIDE5OTggSGV3bGV0dC1QYWNrYXJkIENvbXBhbnkAAGRlc2MAAAAAAAAAEnNSR0IgSUVDNjE5NjYtMi4xAAAAAAAAAAAAAA
                ASc1JHQiBJRUM2MTk2Ni0yLjEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFhZW
                iAAAAAAAADzUQABAAAAARbMWFlaIAAAAAAAAAAAAAAAAAAAAABYWVogAAAAAAAAb6IAADj1AAADkFhZWiAAAAAAAABimQAA
                t4UAABjaWFlaIAAAAAAAACSgAAAPhAAAts9kZXNjAAAAAAAAABZJRUMgaHR0cDovL3d3dy5pZWMuY2gAAAAAAAAAAAAAABZ
                JRUMgaHR0cDovL3d3dy5pZWMuY2gAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAZGVzYw
                AAAAAAAAAuSUVDIDYxOTY2LTIuMSBEZWZhdWx0IFJHQiBjb2xvdXIgc3BhY2UgLSBzUkdCAAAAAAAAAAAAAAAuSUVDIDYxO
                TY2LTIuMSBEZWZhdWx0IFJHQiBjb2xvdXIgc3BhY2UgLSBzUkdCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGRlc2MAAAAAAAAA
                LFJlZmVyZW5jZSBWaWV3aW5nIENvbmRpdGlvbiBpbiBJRUM2MTk2Ni0yLjEAAAAAAAAAAAAAACxSZWZlcmVuY2UgVmlld2l
                uZyBDb25kaXRpb24gaW4gSUVDNjE5NjYtMi4xAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB2aWV3AAAAAAATpP4AFF8uAB
                DPFAAD7cwABBMLAANcngAAAAFYWVogAAAAAABMCVYAUAAAAFcf521lYXMAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAKPA
                AAAAnNpZyAAAAAAQ1JUIGN1cnYAAAAAAAAEAAAAAAUACgAPABQAGQAeACMAKAAtADIANwA7AEAARQBKAE8AVABZAF4AYwBo
                AG0AcgB3AHwAgQCGAIsAkACVAJoAnwCkAKkArgCyALcAvADBAMYAywDQANUA2wDgAOUA6wDwAPYA+wEBAQcBDQETARkBHwE
                lASsBMgE4AT4BRQFMAVIBWQFgAWcBbgF1AXwBgwGLAZIBmgGhAakBsQG5AcEByQHRAdkB4QHpAfIB+gIDAgwCFAIdAiYCLw
                I4AkECSwJUAl0CZwJxAnoChAKOApgCogKsArYCwQLLAtUC4ALrAvUDAAMLAxYDIQMtAzgDQwNPA1oDZgNyA34DigOWA6IDr
                gO6A8cD0wPgA+wD+QQGBBMEIAQtBDsESARVBGMEcQR+BIwEmgSoBLYExATTBOEE8AT+BQ0FHAUrBToFSQVYBWcFdwWGBZYF
                pgW1BcUF1QXlBfYGBgYWBicGNwZIBlkGagZ7BowGnQavBsAG0QbjBvUHBwcZBysHPQdPB2EHdAeGB5kHrAe/B9IH5Qf4CAs
                IHwgyCEYIWghuCIIIlgiqCL4I0gjnCPsJEAklCToJTwlkCXkJjwmkCboJzwnlCfsKEQonCj0KVApqCoEKmAquCsUK3ArzCw
                sLIgs5C1ELaQuAC5gLsAvIC+EL+QwSDCoMQwxcDHUMjgynDMAM2QzzDQ0NJg1ADVoNdA2ODakNww3eDfgOEw4uDkkOZA5/D
                psOtg7SDu4PCQ8lD0EPXg96D5YPsw/PD+wQCRAmEEMQYRB+EJsQuRDXEPURExExEU8RbRGMEaoRyRHoEgcSJhJFEmQShBKj
                EsMS4xMDEyMTQxNjE4MTpBPFE+UUBhQnFEkUahSLFK0UzhTwFRIVNBVWFXgVmxW9FeAWAxYmFkkWbBaPFrIW1hb6Fx0XQRd
                lF4kXrhfSF/cYGxhAGGUYihivGNUY+hkgGUUZaxmRGbcZ3RoEGioaURp3Gp4axRrsGxQbOxtjG4obshvaHAIcKhxSHHscox
                zMHPUdHh1HHXAdmR3DHeweFh5AHmoelB6+HukfEx8+H2kflB+/H+ogFSBBIGwgmCDEIPAhHCFIIXUhoSHOIfsiJyJVIoIir
                yLdIwojOCNmI5QjwiPwJB8kTSR8JKsk2iUJJTglaCWXJccl9yYnJlcmhya3JugnGCdJJ3onqyfcKA0oPyhxKKIo1CkGKTgp
                aymdKdAqAio1KmgqmyrPKwIrNitpK50r0SwFLDksbiyiLNctDC1BLXYtqy3hLhYuTC6CLrcu7i8kL1ovkS/HL/4wNTBsMKQ
                w2zESMUoxgjG6MfIyKjJjMpsy1DMNM0YzfzO4M/E0KzRlNJ402DUTNU01hzXCNf02NzZyNq426TckN2A3nDfXOBQ4UDiMOM
                g5BTlCOX85vDn5OjY6dDqyOu87LTtrO6o76DwnPGU8pDzjPSI9YT2hPeA+ID5gPqA+4D8hP2E/oj/iQCNAZECmQOdBKUFqQ
                axB7kIwQnJCtUL3QzpDfUPARANER0SKRM5FEkVVRZpF3kYiRmdGq0bwRzVHe0fASAVIS0iRSNdJHUljSalJ8Eo3Sn1KxEsM
                S1NLmkviTCpMcky6TQJNSk2TTdxOJU5uTrdPAE9JT5NP3VAnUHFQu1EGUVBRm1HmUjFSfFLHUxNTX1OqU/ZUQlSPVNtVKFV
                1VcJWD1ZcVqlW91dEV5JX4FgvWH1Yy1kaWWlZuFoHWlZaplr1W0VblVvlXDVchlzWXSddeF3JXhpebF69Xw9fYV+zYAVgV2
                CqYPxhT2GiYfViSWKcYvBjQ2OXY+tkQGSUZOllPWWSZedmPWaSZuhnPWeTZ+loP2iWaOxpQ2maafFqSGqfavdrT2una/9sV
                2yvbQhtYG25bhJua27Ebx5veG/RcCtwhnDgcTpxlXHwcktypnMBc11zuHQUdHB0zHUodYV14XY+dpt2+HdWd7N4EXhueMx5
                KnmJeed6RnqlewR7Y3vCfCF8gXzhfUF9oX4BfmJ+wn8jf4R/5YBHgKiBCoFrgc2CMIKSgvSDV4O6hB2EgITjhUeFq4YOhnK
                G14c7h5+IBIhpiM6JM4mZif6KZIrKizCLlov8jGOMyo0xjZiN/45mjs6PNo+ekAaQbpDWkT+RqJIRknqS45NNk7aUIJSKlP
                SVX5XJljSWn5cKl3WX4JhMmLiZJJmQmfyaaJrVm0Kbr5wcnImc951kndKeQJ6unx2fi5/6oGmg2KFHobaiJqKWowajdqPmp
                Fakx6U4pammGqaLpv2nbqfgqFKoxKk3qamqHKqPqwKrdavprFys0K1ErbiuLa6hrxavi7AAsHWw6rFgsdayS7LCszizrrQl
                tJy1E7WKtgG2ebbwt2i34LhZuNG5SrnCuju6tbsuu6e8IbybvRW9j74KvoS+/796v/XAcMDswWfB48JfwtvDWMPUxFHEzsV
                LxcjGRsbDx0HHv8g9yLzJOsm5yjjKt8s2y7bMNcy1zTXNtc42zrbPN8+40DnQutE80b7SP9LB00TTxtRJ1MvVTtXR1lXW2N
                dc1+DYZNjo2WzZ8dp22vvbgNwF3IrdEN2W3hzeot8p36/gNuC94UThzOJT4tvjY+Pr5HPk/OWE5g3mlucf56noMui86Ubp0
                Opb6uXrcOv77IbtEe2c7ijutO9A78zwWPDl8XLx//KM8xnzp/Q09ML1UPXe9m32+/eK+Bn4qPk4+cf6V/rn+3f8B/yY/Sn9
                uv5L/tz/bf//");
		}
		internal PdfICCBasedColorSpace(PdfObjectCollection collection, IList<object> array) {
			if (array.Count != 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			object value = array[1];
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream == null) {
				PdfObjectReference reference = value as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				stream = collection.GetStream(reference.Number);
				if (stream == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			PdfReaderDictionary dictionary = stream.Dictionary;
			int? n = dictionary.GetInteger(componentsCountDictionaryKey);
			if (!n.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			componentsCount = n.Value;
			if (!IsValidComponentsCount)
				PdfDocumentReader.ThrowIncorrectDataException();
			if (dictionary.TryGetValue(alternateDictionaryKey, out value)) {
				alternate = PdfColorSpace.Parse(collection, value);
				if (alternate.ComponentsCount != componentsCount)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else
				alternate = CreateAlternateColorSpace();
			IList<object> rangeArray = dictionary.GetArray(rangeDictionaryKey);
			if (rangeArray == null)
				range = CreateRange();
			else {
				if (rangeArray.Count != componentsCount * 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				range = PdfDocumentReader.CreateDomain(rangeArray);
			}
			metadata = dictionary.GetMetadata();
			data = stream.GetData(true);
		}
		internal PdfICCBasedColorSpace(PdfObjectCollection collection, PdfStream stream) : this(collection, new object[] { 0, stream }) {
		}
		PdfColorSpace CreateAlternateColorSpace() {
			switch (componentsCount) {
				case 1:
					return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
				case 4:
					return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.CMYK);
				default:
					return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.RGB);
			}
		}
		IList<PdfRange> CreateRange() {
			List<PdfRange> range = new List<PdfRange>(componentsCount);
			for (int i = 0; i < componentsCount; i++)
				range.Add(new PdfRange(0, 1));
			return range;
		}
		internal PdfObjectReference CreateStream(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(componentsCountDictionaryKey, componentsCount);
			dictionary.Add(alternateDictionaryKey, alternate.Write(collection));
			bool shouldWriteRange = false;
			List<object> rangeArray = new List<object>(range.Count * 2);
			foreach (PdfRange element in range) {
				double min = element.Min;
				double max = element.Max;
				rangeArray.Add(min);
				rangeArray.Add(max);
				if (min != 0 || max != 1)
					shouldWriteRange = true;
			}
			if (shouldWriteRange)
				dictionary.Add(rangeDictionaryKey, rangeArray);
			dictionary.Add(PdfMetadata.Name, metadata);
			return collection.AddStream(dictionary, data);
		}
		protected internal override PdfColor Transform(PdfColor color) {
			return alternate.Transform(color);
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			return alternate.Transform(data, width, height, bitsPerComponent, colorKeyMask);
		}
		protected internal override PdfRange[] CreateDefaultDecodeArray(int bitsPerComponent) {
			PdfRange[] array = new PdfRange[range.Count];
			int index = 0;
			foreach (PdfRange item in range)
				array[index++] = new PdfRange(item.Min, item.Max);
			return array;
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return new List<object> { new PdfName(TypeName), CreateStream(collection) };
		}
	}
}
