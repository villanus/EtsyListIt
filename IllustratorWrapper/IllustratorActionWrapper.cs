﻿using System;
using System.IO;
using Illustrator;

namespace IllustratorWrapper
{
    public class IllustratorActionWrapper : IIllustratorActionWrapper
    {
        private readonly dynamic _application;

        public IllustratorActionWrapper()
        {
            var type = Type.GetTypeFromProgID("Illustrator.Application");
            _application = Activator.CreateInstance(type);
        }
        public void ExportFileAsJPEG(string baseFile, string outputDirectory, string newFileName = null)
        {
            var fullFileName = GetFullFileName(baseFile, outputDirectory, newFileName, ".jpg");
            dynamic document = _application.Open(baseFile);
            ExportFileAsJPEG(document, fullFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        private void ExportFileAsJPEG(dynamic document, string fullFileName)
        {
            FitArtboardToCurrentDocument(document);
            //CenterItemsOnArtboard(document);
            dynamic jpgOptions = new ExportOptionsJPEG();
            jpgOptions.QualitySetting = 100;
            jpgOptions.AntiAliasing = false;
            jpgOptions.Optimization = false;
            document.Export(fullFileName, AiExportType.aiJPEG, jpgOptions);
        }

        public void ExportFileAsDXF(string baseFile, string outputDirectory, string extension, string newFileName = null)
        {
            var fullFileName = GetFullFileName(baseFile, outputDirectory, newFileName, extension);
            dynamic document = _application.Open(baseFile);
            ExportFileAsDXF(document, extension, fullFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        private void ExportFileAsDXF(dynamic document, string extension, string newFileName)
        {
            newFileName += extension;
            FitArtboardToCurrentDocument(document);
            //CenterItemsOnArtboard(document);
            dynamic dxfOptions = new ExportOptionsAutoCAD();
            Type enumType = typeof(AiAutoCADExportFileFormat);
            dynamic enumValue = enumType.GetField("aiDXF").GetValue(null);
            dxfOptions.ExportFileFormat = enumValue;
            document.Export(newFileName, AiExportType.aiAutoCAD, dxfOptions);
        }

        public void SaveFileAsEPS(string baseFile, string outputDirectory, string newFileName = null)
        {
            var fullFileName = GetFullFileName(baseFile, outputDirectory, newFileName, ".eps");
            dynamic document = _application.Open(baseFile);
            SaveFileAsEPS(document, fullFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }
        private void SaveFileAsEPS(dynamic document, string newFileName)
        {
            FitArtboardToCurrentDocument(document);
            //CenterItemsOnArtboard(document);
            document.SaveAs(newFileName, new EPSSaveOptions());
        }

        public void SaveFileAsPDF(string baseFile, string outputDirectory, string newFileName = null)
        {
            var fullFileName = GetFullFileName(baseFile, outputDirectory, newFileName, ".pdf");
            dynamic document = _application.Open(baseFile);
            SaveFileAsPDF(document, fullFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        private void SaveFileAsPDF(dynamic document, string newFileName)
        {
            FitArtboardToCurrentDocument(document);
            document.SaveAs(newFileName, new PDFSaveOptions());
        }

        private string SaveFileWithWatermark(string watermarkFile, dynamic sourceDocument, string fullFileName)
        {
            FitArtboardToCurrentDocument(sourceDocument);

            //select all and group
            sourceDocument.SelectObjectsOnActiveArtboard();
            dynamic sourceDocSelection = sourceDocument.Selection;
            if (sourceDocSelection.Length == 0)
            {
                throw new IllustratorException("Selected image is empty!");
            }
            dynamic imageGroup = sourceDocument.GroupItems.Add();
            for (var i = 0; i < sourceDocSelection.Length; i++)
            {
                sourceDocSelection[i].moveToEnd(imageGroup);
            }

            var imageHeight = imageGroup.Height;
            var imageWidth = imageGroup.Width;

            // select all and group
            dynamic watermarkDocument = _application.Open(watermarkFile,
                AiDocumentColorSpace.aiDocumentRGBColor, null);
            watermarkDocument.SelectObjectsOnActiveArtboard();
            dynamic watermarkDocSelection = watermarkDocument.Selection;
            if (watermarkDocSelection.Length == 0)
            {
                throw new IllustratorException("Selected image is empty!");
            }
            dynamic watermarkGroup = watermarkDocument.groupItems.add();
            for (var i = 0; i < watermarkDocSelection.Length; i++)
            {
                watermarkDocSelection[i].moveToEnd(watermarkGroup);
            }

            //scale watermark to fit graphic
            int scale;

            var watermarkHeight = watermarkGroup.Height;
            var watermarkWidth = watermarkGroup.Width;

            if (imageHeight > imageWidth)
            {
                scale = int.Parse(Math.Round(imageHeight / watermarkHeight * 100).ToString()) + 5;
            }
            else
            {
                scale = int.Parse(Math.Round(imageWidth / watermarkWidth * 100).ToString()) + 5;
            }
            watermarkGroup.Resize(scale, scale);

            // copy watermark to grpahic
            watermarkDocument.Copy();
            sourceDocument.Paste();

            watermarkDocument.Close(AiSaveOptions.aiDoNotSaveChanges);
            CenterItemsOnArtboard(sourceDocument);

            dynamic jpgOptions = new ExportOptionsJPEG();
            jpgOptions.QualitySetting = 100;
            jpgOptions.AntiAliasing = false;
            jpgOptions.Optimization = false;
            sourceDocument.Export(fullFileName, AiExportType.aiJPEG, jpgOptions);

            return fullFileName;
        }
        public string SaveFileWithWatermark(string watermarkFile, string baseFile, string outputDirectory)
        {

            var fullFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(baseFile) + "Watermarked.jpg");

            dynamic sourceDocument = _application.Open(baseFile);

            dynamic watermark = SaveFileWithWatermark(watermarkFile, sourceDocument, fullFileName);

            sourceDocument.Close();

            return watermark;

        }

        private string GetFullFileName(string baseFile, string outputDirectory, string newFileName, string extension)
        {
            if (newFileName != null)
            {
                if (!newFileName.Contains(extension))
                {
                    newFileName = newFileName + extension;
                }
                return Path.Combine(outputDirectory, newFileName);
            }
            else
            {
                return Path.Combine(outputDirectory,
                    Path.GetFileNameWithoutExtension(baseFile) + extension);
            }
        }

        public void ExportFileAsPNG(string baseFile, string outputDirectory, string newFileName = null)
        {
            var fullFileName = GetFullFileName(baseFile, outputDirectory, newFileName, ".png");
            dynamic document = _application.Open(baseFile);
            ExportFileAsPNG(document, fullFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        private void ExportFileAsPNG(dynamic document, string newFileName)
        {
            FitArtboardToCurrentDocument(document);
            //CenterItemsOnArtboard(document);
            dynamic pngOptions = new ExportOptionsPNG24();
            pngOptions.Transparency = true;
            pngOptions.AntiAliasing = false;
            pngOptions.SaveAsHTML = false;
            document.Export(newFileName, AiExportType.aiPNG24, pngOptions);
        }

        public void CenterItemsOnArtboard(dynamic document)
        {
            document.SelectObjectsOnActiveArtboard();
            var sel = document.Selection;

            if (sel.Length > 0)
            {
                foreach (var s in sel)
                {
                    dynamic objectToCenter = s;
                    var artboardIndex = document.Artboards.GetActiveArtboardIndex() + 1;
                    var activeArtboard = document.Artboards[artboardIndex].ArtboardRect;
                    var vertCenter = activeArtboard[1] / 2;
                    var horizCenter = activeArtboard[2] / 2;

                    var left = horizCenter - (objectToCenter.Width / 2);
                    var top = vertCenter + (objectToCenter.Height / 2);
                    
                    dynamic newPosition = new[]
                    {
                        left, top
                    };

                    objectToCenter.position = newPosition;
                }
            }
            else
            {
                throw new IllustratorException("There is no selection!");
            }
        }

        public void FitArtboardToCurrentDocument(dynamic document)
        {
            var index = document.Artboards.GetActiveArtboardIndex() + 1;
            document.Artboards[index].ArtboardRect = new[]
            {
                document.VisibleBounds[0] - document.VisibleBounds[1] * .05,
                document.VisibleBounds[1] + document.VisibleBounds[1] * .05,
                document.VisibleBounds[2] + document.VisibleBounds[1] * .05,
                document.VisibleBounds[3] - document.VisibleBounds[1] * .05
            };
        }

        public void UngroupItems(dynamic groupItems)
        {
            foreach (dynamic item in groupItems)
            {
                groupItems.Remove(item);
            }
        }

        public void ExportAll(string baseFile, string tempDirectoryPath, string newFileName = null)
        {
            dynamic document = _application.Open(baseFile);
            newFileName = newFileName ?? Path.GetFileNameWithoutExtension(baseFile);
            ExportAll(document, tempDirectoryPath, newFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        public void ExportFileAsSVG(string baseFile, string tempDirectoryPath, string newFileName = null)
        {
            var fullFileName = GetFullFileName(baseFile, tempDirectoryPath, newFileName, ".svg");
            dynamic document = _application.Open(baseFile);
            ExportFileAsSVG(baseFile, fullFileName);
            document.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        private void ExportFileAsSVG(dynamic document, string newFileName)
        {
            
            FitArtboardToCurrentDocument(document);
            document.Export(newFileName, AiExportType.aiSVG, new ExportOptionsSVG());
        }

        public void ExportMultipleArtboards(string baseFile, string tempDirectoryPath)
        {
            dynamic oldDoc = _application.Open(baseFile);
            for (int count = 1; count <= oldDoc.Artboards.Count; count++)
            {
                oldDoc.Artboards.SetActiveArtboardIndex(count-1);
                var artboardName = oldDoc.Artboards[count].Name;
                oldDoc.SelectObjectsOnActiveArtboard();
                oldDoc.Copy();

                dynamic newDoc = _application.Documents.Add();
                newDoc.Paste();

                newDoc.SelectObjectsOnActiveArtboard();
                FitArtboardToCurrentDocument(newDoc);

                ExportAll(newDoc, tempDirectoryPath, artboardName);

                newDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
            }

            oldDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
        }

        private void ExportAll(dynamic document, string tempDirectoryPath, string newFileName = null)
        {
            ExportFileAsJPEG(document, Path.Combine(tempDirectoryPath, newFileName));
            ExportFileAsPNG(document, Path.Combine(tempDirectoryPath, newFileName));
            ExportFileAsDXF(document, ".dxf", Path.Combine(tempDirectoryPath, newFileName));
            SaveFileAsEPS(document, Path.Combine(tempDirectoryPath, newFileName));
            SaveFileAsPDF(document, Path.Combine(tempDirectoryPath, newFileName));
            ExportFileAsSVG(document, Path.Combine(tempDirectoryPath, newFileName));
            ExportFileAsDXF(document, ".studio3", Path.Combine(tempDirectoryPath, newFileName));
        }
        
        public int CountArtboards(string baseFile)
        {
            dynamic document = _application.Open(baseFile);
            var count = document.Artboards.Count;
            document.Close(AiSaveOptions.aiDoNotSaveChanges);

            return count;
        }

        public string CreateWatermarkForArtboard(string baseFile, string tempDirectoryPath, string watermarkPath,
            int artboardIndex)
        {
            dynamic oldDoc = _application.Open(baseFile);
            oldDoc.Artboards.SetActiveArtboardIndex(artboardIndex-1);
            var artboardName = oldDoc.Artboards[artboardIndex].Name;
            oldDoc.SelectObjectsOnActiveArtboard();
            oldDoc.Copy();

            dynamic newDoc = _application.Documents.Add();
            newDoc.Paste();
            var fullFileName = Path.Combine(tempDirectoryPath, artboardName + ".jpg");
            SaveFileWithWatermark(watermarkPath, newDoc, fullFileName);
            
            newDoc.Close(AiSaveOptions.aiDoNotSaveChanges);
            oldDoc.Close(AiSaveOptions.aiDoNotSaveChanges);

            return fullFileName;
        }

        public dynamic GroupItems(dynamic document)
        {
            document.SelectObjectsOnActiveArtboard();
            dynamic selection = document.Selection;
            dynamic imageGroup = document.GroupItems.Add();
            for (var i = 0; i < selection.Length; i++)
            {
                selection[i].moveToEnd(imageGroup);
            }

            return document.GroupItems;
        }
    }
}