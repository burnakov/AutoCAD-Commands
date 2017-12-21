// (C) Copyright 2017 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(ACADCommands.MLeaderCoords))]

namespace ACADCommands
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class MLeaderCoords
    {
        [CommandMethod("mlcoords")]
        public static void MLCoords()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            if (acDoc == null)
                return;
            Database acCurrDb = acDoc.Database;

            using (Transaction acTrans = acCurrDb.TransactionManager.StartTransaction())
            {
                TypedValue[] acTypValArr = new TypedValue[1];
                acTypValArr.SetValue(new TypedValue((int)DxfCode.Start, "MULTILEADER"), 0);
                SelectionFilter acSelFilter = new SelectionFilter(acTypValArr);
                PromptSelectionResult acSSPromptRes = acDoc.Editor.GetSelection(acSelFilter);
                if (acSSPromptRes.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPromptRes.Value;
                    foreach (SelectedObject acSObj in acSSet)
                    {
                        if (acSObj != null)
                        {
                            MLeader acMLeader = acTrans.GetObject(acSObj.ObjectId, OpenMode.ForWrite) as MLeader;
                            if (acMLeader != null)
                            {
                                MText acMText = acMLeader.MText;
                                acMText.Contents = "X= " + acMLeader.GetFirstVertex(0).X.ToString("F2") +
                                                   "\nY= " + acMLeader.GetFirstVertex(0).Y.ToString("F2");
                                acMLeader.MText = acMText;
                            }
                        }
                    }
                    acTrans.Commit();
                    acDoc.Editor.Regen();
                }
                else
                {
                    acDoc.Editor.WriteMessage("\nCanceled.");
                }
            }
        }
    }
}
