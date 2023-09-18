namespace BananaEngine.Util;

using System;
using System.Collections.Generic;

public interface IGraphNode
{
    IEnumerable<IGraphEdge> GetEdges();
}

public interface IGraphEdge
{
    IGraphNode GetDestination();
}

public class GraphIterator
{
    public static IEnumerable<IGraphNode> Bfs(IGraphNode root)
    {
        return GraphIterator.Bfs(new List<IGraphNode> { root });
    }

    public static IEnumerable<IGraphNode> Bfs(List<IGraphNode> rootNodes)
    {
        Queue<IGraphNode> queue = new Queue<IGraphNode>();
        HashSet<IGraphNode> visited = new HashSet<IGraphNode>();

        foreach (var node in rootNodes)
        {
            queue.Enqueue(node);
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (visited.Contains(node))
            {
                continue;
            }

            visited.Add(node);
            yield return node;

            foreach (var edge in node.GetEdges())
            {
                queue.Enqueue(edge.GetDestination());
            }
        }
    }
}