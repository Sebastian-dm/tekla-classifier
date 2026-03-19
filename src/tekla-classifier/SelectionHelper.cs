using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace CCI {
    internal class SelectionHelper {

        public SelectionHelper() { }

        public List<Part> GetSelectedParts() {

            List<Part> selectedParts = new List<Part>();

            ModelObjectEnumerator moe = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();
            moe.SelectInstances = true;

            while (moe.MoveNext()) {
                ModelObject modelObject = moe.Current;
                if (modelObject != null && modelObject is Part) {
                    selectedParts.Add((Part)modelObject);
                }
            }

            return selectedParts;
        }

        public List<Part> GetAllParts() {

            Type[] Types = new Type[1];
            Types.SetValue(typeof(Part), 0);

            ModelObjectEnumerator moe = Program.TeklaModel.GetModelObjectSelector().GetAllObjectsWithType(Types);
            moe.SelectInstances = true;

            Part[] allParts = new Part[moe.GetSize()];
            for (int i = 0; i < moe.GetSize();i++)
                allParts[i] = (Part)moe.Current;

            return allParts.ToList();
        }
    }
}
