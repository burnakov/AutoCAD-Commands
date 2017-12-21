using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(ACADCommands.ListAllObjects))]

namespace ACADCommands
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class ListAllObjects
    {
        [CommandMethod("listobjects")]
        public static void ListObjects()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            if (acDoc == null)
                return;
            Database acCurrDb = acDoc.Database;

            using (Transaction acTrans = acCurrDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = acTrans.GetObject(
                    acCurrDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = acTrans.GetObject(
                                    acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId acObjId in acBlkTblRec)
                {
                    acDoc.Editor.WriteMessage("\nDXF name: " + acObjId.ObjectClass.DxfName);
                    acDoc.Editor.WriteMessage("\nObjectID: " + acObjId.ToString());
                    acDoc.Editor.WriteMessage("\nHandle: " + acObjId.Handle.ToString());
                    acDoc.Editor.WriteMessage("\n");
                }
            }
        }
    }
}
