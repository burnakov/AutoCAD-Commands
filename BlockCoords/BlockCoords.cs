using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(ACADCommands.BlockCoords))]

namespace ACADCommands
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class BlockCoords
    {
        [CommandMethod("bcoords")]
        public static void BlkCoords()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            if (acDoc == null)
                return;
            Database acCurrDb = acDoc.Database;

            using (Transaction acTrans = acCurrDb.TransactionManager.StartTransaction())
            {
                TypedValue[] acTypValArr = new TypedValue[1];
                acTypValArr.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                SelectionFilter acSelFilter = new SelectionFilter(acTypValArr);
                PromptSelectionResult acSSPromptRes = acDoc.Editor.GetSelection(acSelFilter);
                if (acSSPromptRes.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPromptRes.Value;
                    foreach (SelectedObject acSObj in acSSet)
                    {
                        if (acSObj != null)
                        {
                            BlockReference acBlockRef = acTrans.GetObject(acSObj.ObjectId, OpenMode.ForWrite) as BlockReference;
                            if (acBlockRef != null)
                            {
                                BlockTable acBlkTbl = acTrans.GetObject(
                                            acCurrDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                                BlockTableRecord acBlkTblRec = acTrans.GetObject(
                                            acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                                MLeader acMLeader = new MLeader();
                                acMLeader.SetDatabaseDefaults();
                                acMLeader.ContentType = ContentType.MTextContent;

                                MText acMText = new MText();

                                double xCoord = acBlockRef.Position.X;
                                double yCoord = acBlockRef.Position.Y;
                                double zCoord = acBlockRef.Position.Z;

                                acMText.Contents = "X= " + xCoord.ToString("F2") +
                                                   "\nY= " + yCoord.ToString("F2");
                                acMText.Height = 1;
                                acMLeader.MText = acMText;

                                acMLeader.TextHeight = 1;
                                acMLeader.TextLocation = new Point3d(xCoord + 5, yCoord + 5, zCoord);
                                acMLeader.ArrowSize = 1;
                                acMLeader.EnableDogleg = true;
                                acMLeader.DoglegLength = 0;
                                acMLeader.EnableLanding = true;
                                acMLeader.LandingGap = 1;
                                acMLeader.ExtendLeaderToText = true;
                                acMLeader.SetTextAttachmentType(TextAttachmentType.AttachmentBottomOfTopLine, LeaderDirectionType.LeftLeader);
                                acMLeader.SetTextAttachmentType(TextAttachmentType.AttachmentBottomOfTopLine, LeaderDirectionType.RightLeader);

                                acMLeader.AddLeaderLine(acBlockRef.Position);

                                acBlkTblRec.AppendEntity(acMLeader);
                                acTrans.AddNewlyCreatedDBObject(acMLeader, true);
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
