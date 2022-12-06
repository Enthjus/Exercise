using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;

namespace LibraryCad.ObjectsFunc.BlockObject
{
    public class BlockNestedEntityHighlighter : IDisposable
    {
        private Entity _entClone = null;
        private readonly TransientManager _tsManager =
            TransientManager.CurrentTransientManager;
        private int _colorIndex = 2;

        public void HighlightEntityInBlock(ObjectId nestedEntId, Matrix3d transform)
        {
            ClearHighlight();
            using (var tran = nestedEntId.Database.TransactionManager.StartTransaction())
            {
                var ent = (Entity)tran.GetObject(nestedEntId, OpenMode.ForRead);
                _entClone = ent.Clone() as Entity;
                tran.Commit();
            }

            _entClone.ColorIndex = _colorIndex;
            _entClone.TransformBy(transform);

            _tsManager.AddTransient(
                _entClone,
                TransientDrawingMode.Highlight,
                128,
                new IntegerCollection());
        }

        public void Dispose()
        {
            ClearHighlight();
        }

        private void ClearHighlight()
        {
            if (_entClone != null)
            {
                _tsManager.EraseTransient(
                    _entClone, new IntegerCollection());
                _entClone.Dispose();
                _entClone = null;
            }
        }
    }
}
