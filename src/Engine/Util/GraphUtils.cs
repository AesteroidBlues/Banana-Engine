namespace BananaEngine.Util;

using System;
using System.Collections.Generic;
using System.Linq;

public class BlackboardVertex : BlackboardObject, IGraphNode
{
    static int s_NextId = 0;

    public int Id { get; private set; }
    List<BlackboardDirectedEdge> m_Edges = new List<BlackboardDirectedEdge>();

    public BlackboardVertex()
    {
        Id = s_NextId++;
    }

    public void AddEdge(BlackboardDirectedEdge edge)
    {
        if (!m_Edges.Contains(edge))
        {
            m_Edges.Add(edge);
        }
    }

    public IEnumerable<IGraphEdge> GetEdges()
    {
        return m_Edges.ConvertAll(x => x as IGraphEdge);
    }

    public override string ToString()
    {
        return $"Vertex {Id}";
    }
}

public class BlackboardDirectedEdge : BlackboardObject, IGraphEdge
{
    static int s_Id;

    public int Id { get; private set; }
    BlackboardVertex m_Source;
    public BlackboardVertex m_Destination { get; set; }
    Dictionary<string, Object> m_Parameters = new Dictionary<string, Object>();

    public BlackboardDirectedEdge(BlackboardVertex source)
    {
        Id = s_Id++;
        m_Source = source;
    }

    public IGraphNode GetDestination()
    {
        return m_Destination;
    }
}