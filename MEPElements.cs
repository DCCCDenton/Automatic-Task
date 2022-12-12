using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using Autodesk.Revit.DB.DirectContext3D;

namespace Semi_automatic_trace
{
    class MEPElements
    {
        private Element SelectionFirstElement(Document doc, UIDocument uidoc)
        {
            Selection selection = uidoc.Selection;
            ICollection<ElementId> allTray = uidoc.Selection.GetElementIds();
            Element tray = null;
            foreach (ElementId IDtray in allTray)
            {
                tray = doc.GetElement(IDtray);
            }
        
            return tray;
        }

        private ConnectorSet FindElementConnector(Element elm)
        {
            ConnectorSet connectors = new ConnectorSet();
            try
            {
                MEPCurve inst = elm as MEPCurve;
                connectors = inst.ConnectorManager.Connectors;
            }
            catch
            {
                FamilyInstance ins = elm as FamilyInstance;
                MEPModel i = ins.MEPModel;
                if (i != null && i.ConnectorManager != null)
                {
                    connectors = i.ConnectorManager.Connectors;
                }
            }

           
            return connectors;
        }

        private ICollection<Element> FindConnectorOwner(ConnectorSet connectors)
        {
            List<Element> allOwner = new List<Element>();
            foreach (Connector con in connectors)
            {
                ConnectorSet allref = con.AllRefs;
                foreach (Connector reff in allref)
                {
                    Element e = reff.Owner;
                    allOwner.Add(e);
                }
            }
            return allOwner;
        }


        public BidirectionalGraph<object, IEdge<object>> BuildGraph(Document doc, UIDocument uidoc)
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();
            List<string> closeElement = new List<string>();
            ConnectorSet ConSet = new ConnectorSet();
            ICollection<Element> allO = new List<Element>();
            Queue<Element> grq = new Queue<Element>();
            string elm_id = null;
            string base_elm_id = null;
            Element elm = SelectionFirstElement(doc, uidoc);
            grq.Enqueue(elm);
            while (grq.Count != 0)
            {
                Element t = grq.Dequeue();
                base_elm_id = t.Id.ToString();
                g.AddVertex(base_elm_id);
                ConSet = null;
                allO = null;
                ConSet = FindElementConnector(t);
                allO = FindConnectorOwner(ConSet);
                if (allO != null)
                {
                    foreach (Element e in allO)
                    {
                        elm_id = e.Id.ToString();
                        if (!closeElement.Contains(elm_id))
                        {
                            g.AddVertex(elm_id);
                            var edge = new Edge<object>(base_elm_id, elm_id);
                            g.AddEdge(edge);
                            grq.Enqueue(e);
                            closeElement.Add(elm_id);
                        }
                    }
                }
            }
            return g;
        }

        public List<object> graph_vertex(BidirectionalGraph<object, IEdge<object>> g)
        {
            List<object> vertex_list = new List<object>();
            vertex_list = g.Vertices.ToList();
            return vertex_list;
        }


    }
}


 