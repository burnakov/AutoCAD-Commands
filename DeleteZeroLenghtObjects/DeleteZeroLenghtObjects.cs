using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(ACADCommands.DeleteZeroLenghtObjects))]

namespace ACADCommands
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class DeleteZeroLenghtObjects
    {
        [CommandMethod("delzeroes")]
        public void DelZeroes()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            if (acDoc == null)
                return;
            Database acCurrDb = acDoc.Database;
            int delQnt = 0;
            using (Transaction acTrans = acCurrDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = acTrans.GetObject(
                    acCurrDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                foreach (ObjectId acBlkTblRecId in acBlkTbl)
                {
                    BlockTableRecord acBlkTblRec = acTrans.GetObject(
                        acBlkTblRecId, OpenMode.ForRead) as BlockTableRecord;
                    foreach (ObjectId acEntityId in acBlkTblRec)
                    {
                        Curve acCurve = acTrans.GetObject(acEntityId, OpenMode.ForWrite) as Curve;
                        if (acCurve != null &&
                            acCurve.GetDistanceAtParameter(acCurve.EndParam) < Tolerance.Global.EqualPoint)
                        {
                            acCurve.Erase();
                            delQnt++;
                        }
                    }
                }
                acTrans.Commit();
            }
            acDoc.Editor.WriteMessage("\nErased {0} objects.", delQnt);
        }
    }
}
